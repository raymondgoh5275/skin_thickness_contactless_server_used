using Contacless.Model;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using Dapper;

namespace Contacless
{
    public class CsvScan
    {
        private readonly string csvPath;
        private readonly string oemInputPath;
        private readonly string constring;
        private readonly string batch;
        private readonly string serial;
        private readonly string resource;
        private readonly string inspector;
        private readonly DateTime created;
        private const int colx = 0;
        private const int coly = 1;
        private const int colact = 7;
        private const int colnom = 8;
        private const int colmin = 10;
        private const int colmax = 11;
        private const int coltl = 9;
        private const int colcsv = 6;
        private const int colflag = 12;

        public CsvScan(string csvPath, string oemInputPath, string constring, string batch, string serial, string resource, string inspector, DateTime created)
        {
            this.csvPath = csvPath;
            this.oemInputPath = oemInputPath;
            this.constring = constring;
            this.batch = batch;
            this.serial = serial;
            this.resource = resource;
            this.inspector = inspector;
            this.created = created;
        }

        private List<string[]> ParseCsv(string path)
        {
            List<string[]> parsedData = new List<string[]>();
            try
            {
                int MaxCols = 0;
                using (StreamReader readFile = new StreamReader(path))
                {
                    string line;
                    string[] row;
                    while ((line = readFile.ReadLine()) != null)
                    {
                        row = line.Split(',');

                        if (MaxCols < row.Length)
                            MaxCols = row.Length;

                        parsedData.Add(row);
                    }
                }
                for (int i = 0; i < parsedData.Count; i++)
                {
                    string[] row = parsedData[i];

                    if (row.Length < MaxCols)
                    {
                        Array.Resize<string>(ref row, MaxCols);

                        parsedData[i] = row;
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                throw new Exception(string.Format("{0}, CSV File {1} not found.", DateTime.Now, e.FileName));
            }
            return parsedData;
        }

        private string RemapPath(string filePath)
        {
            return Path.Combine(csvPath, Path.GetFileName(filePath));
        }

        public string Parse(List<string> csvFileNames)
        {

            //combine all file rows
            var rows = new List<string[]>();
            foreach (var fname in csvFileNames)
            {
                rows.AddRange(ParseCsv(Path.Combine(oemInputPath, fname)));
            }
            if (rows.Count < 1) { throw new ArgumentOutOfRangeException("Empthy rows in csv files"); }

            var profileFiles = rows[1][0].Split(';');

            //get profile name
            var profilename = Path.GetFileNameWithoutExtension(profileFiles[0]);

            if (string.IsNullOrWhiteSpace(profilename))
            {
                throw new Exception("profile name not found");
            }

            var profile_id = 0;
            using (var con = new SqlConnection(constring))
            {
                var qry = @"select id from profile where name = @profilename";
                profile_id = con.QueryFirstOrDefault<int>(qry, new { profilename});
            }

            if(profile_id < 1)
            {
                var profile = SaveProfile(BuildProfile(profileFiles));
                profile_id = profile.id;
            }

            var inScanRows = new List<ScanRow>();
            var outScanRows = new List<ScanRow>();

            for (int i = 0; i < rows.Count; i++)
            {
                if (rows[i][0].Trim() == "Side" && rows[i][1].ToLower().Contains("form"))
                {
                    var rowstart = i + 4;
                    var count = 0;
                    for (int j = 0; j < rows.Count; j++)
                    {
                        if (rows[j + rowstart][0].Trim() == "%") { count = j; break; }
                    }

                    var Number = rows[i + 1][1].Trim();
                    var line = rows[i + 2][1].Trim();
                    var LeadingEdgeTranslation = rows[i + 3][1].Trim();
                    var rowsrange = rows.GetRange(rowstart, count);
                    var side = rows[i][1].Trim() == "Inside form" ? BladeForm.Inside : BladeForm.Outside;
                    if (rowsrange.Count > 1)
                    {
                        decimal Y;
                        decimal.TryParse(line, out Y); // Ignore the return value
                        var profilePoints = GetProfilePoints(profilename, side, Y);
                        //_ = decimal.TryParse(line, out var Y);
                        //var profilePoints = GetProfilePoints(profilename, side, Y);
                        var scanPoints = ParseLine2(rowsrange, profilePoints, side, Y);
                        var scanRow = new ScanRow()
                        {
                            DeltaX = ParseCell(LeadingEdgeTranslation),
                            Y = Y,
                            Form = side,
                            Points = scanPoints.ToArray()
                        };
                        if (side == BladeForm.Outside) { outScanRows.Add(scanRow); }
                        else { inScanRows.Add(scanRow); }
                    }
                    i = i + count + 4;
                }
            }

            var scan = new Scan
            {
                InsideForm = inScanRows.ToArray(),
                OutsideForm = outScanRows.ToArray(),
                Inspector = inspector,
                Batch = batch,
                Resource = resource,
                Result = true,
                Created = created,
                Profile_id = profile_id,
                SerialNo = serial,
                FilePath = Path.Combine(oemInputPath, string.Join("_", csvFileNames))
                
            };

            scan = SaveScan(scan);
            return "";// JsonConvert.SerializeObject(scan);
        }


        private Scan SaveScan(Scan scan)
        {
            using (var con = new SqlConnection(constring))
            {
                con.Open();
                using (var trans = con.BeginTransaction())
                {
                    var qry = @"INSERT INTO [skin_thickness].[dbo].[scan]
                               ([filepath]
                               ,[SerialNo]
                               ,[Batch]
                               ,[created]
                               ,[Resource]
                               ,[tank_number]
                               ,[inspector]
                               ,[result]
                               ,[profile_id])
                         VALUES
                               (@filepath
                               ,@SerialNo
                               ,@Batch
                               ,@created
                               ,@Resource
                               ,@tank_number
                               ,@inspector
                               ,@result
                               ,@profile_id);
                        SELECT CAST(SCOPE_IDENTITY() as int)";
                    var paramScan = new
                    {
                        filepath = scan.FilePath,
                        SerialNo = scan.SerialNo,
                        Batch = scan.Batch,
                        created = scan.Created,
                        Resource = scan.Resource,
                        tank_number = scan.TankNumber,
                        inspector = scan.Inspector,
                        result = scan.Result,
                        profile_id = scan.Profile_id
                    };
                    scan.id = con.QuerySingle<int>(qry, paramScan, transaction:trans);

                    var scanrows = new List<ScanRow>();
                    scanrows.AddRange(scan.OutsideForm);
                    scanrows.AddRange(scan.InsideForm);
                    foreach (var row in scanrows)
                    {
                        qry = @"INSERT INTO [skin_thickness].[dbo].[scan_row]
		                    ([scan_id]
		                    ,[form]
		                    ,[y]
		                    ,[DeltaX])
	                    VALUES
		                    (@scan_id
		                    ,@form
		                    ,@y
		                    ,@DeltaX);
                        SELECT CAST(SCOPE_IDENTITY() as int)";
                        var paramRow = new
                        {
                            scan_id = scan.id,
                            form = row.Form,
                            y = row.Y,
                            DeltaX = row.DeltaX
                        };
                        row.id = con.QuerySingle<int>(qry, paramRow, transaction:trans);
                        foreach (var point in row.Points)
                        {
                            qry = @"INSERT INTO [skin_thickness].[dbo].[scan_point]
                                    ([scan_row_id]
                                    ,[x]
                                    ,[y]
                                    ,[thickness]
                                    ,[tl])
                                VALUES
                                    (@scan_row_id
                                    ,@x
                                    ,@y
                                    ,@thickness
                                    ,@tl_num);
                            SELECT CAST(SCOPE_IDENTITY() as int)";
                            var paramPoint = new
                            {
                                scan_row_id = row.id,
                                x = point.X,
                                y = point.Y,
                                thickness = point.Thickness,
                                tl_num = point.TrafficLight
                            };
                            point.id = con.QuerySingle<int>(qry, paramPoint, transaction: trans);
                        }
                    }
                    trans.Commit();
                    return scan;
                }
            }
        }

        private Pdata[] GetProfilePoints(string profilename, BladeForm side, decimal line)
        {
            using (var con = new SqlConnection(constring))
            {
                var qry = @"SELECT pp.x, pp.tl 
                            from profile p
                            join profile_row pr on pr.profile_id = p.id
                            join profile_point pp on pp.profile_row_id = pr.id
                            where p.name = @profilename and pr.form = @side and pr.y = @line";
                return con.Query<Pdata>(qry, new { profilename, side, line }).ToArray();
            }
        }


        private List<ScanPoint> ParseLine2(List<string[]> rows, Pdata[] pdatas, BladeForm form, decimal y)
        {
            if (pdatas.Length < 1) { throw new ArgumentException("No items fetched from profile point"); }
            if (rows.Count < 1) { throw new ArgumentException("No row found for scan point"); }
            if (form == BladeForm.Inside) { rows.Reverse(); }
            // find max delta - index
            decimal maxDelta = 0;

            decimal val1 = 0;
            decimal val2 = 0;



            //var ix = -1;
            //var iy = -1;

            //for (int i = 0; i < rows.Count; i++)
            //{
            //    var row = rows[i];

            //    if(i < rows.Count/2)
            //    {
            //        var v1 = ParseCell(row[colact]);
            //        if(v1 == 0)
            //        {
            //            ix = i;
            //        }
            //        else
            //        {
            //            ix = 0; //In case not strating with zero
            //        }


            //    }
            //    else
            //    {
            //        var v1 = ParseCell(row[colact]);
            //        if(v1 == 0 && iy == -1)
            //        {
            //            iy = i;
            //            break;
            //        }
            //        else
            //        {
            //            iy = rows.Count;  //In case not ending with zero
            //            //break;
            //        }

            //    }

            //}

            var ix = 0;
            var iy = rows.Count - 1;

            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                var v = ParseCell(row[colact]);
                if (i < rows.Count / 2)
                {
                    if (v == 0m)
                    {
                        ix = i;
                    }
                }
                else
                {
                    if (v == 0m)
                    {
                        iy = i;
                        break;
                    }
                }
            }



            rows = rows.GetRange(ix + 1, iy - ix);

            int idx = -1;


            for (int i = 0; i < rows.Count / 2; i++)
            {
                var row = rows[i];
                var rowN = rows[i + 1];
                var v1 = ParseCell(row[colact]);
                var v2 = ParseCell(rowN[colact]);
                var mx = Math.Abs(v1 - v2);
                if (mx > maxDelta) { maxDelta = mx; idx = i + 1; val1 = v1; val2 = v2; }
            }
            var res = new List<ScanPoint>();
            for (int i = 0; i < pdatas.Count(); i++)
            {
                if (i >= rows.Count - idx) { break; }

                var sp = new ScanPoint
                {
                    X = pdatas[i].x,
                    Y = y,
                    Thickness = ParseCell(rows[i + idx][colact]),
                    TrafficLight = pdatas[i].tl
                };
                res.Add(sp);
            }
            return res;
        }

        private List<ScanPoint> parseLine(List<string[]> rows, decimal [] points, BladeForm form, decimal y)
        {

            var sfound = false;
            var spoints = new List<decimal>();
            var frows = new List<string[]>();
            for (int i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                var x = ParseCell(row[colx]);
                //x = form == BladeForm.Outside? x: -x;
                
                if (!sfound)
                {
                    if (row[colcsv].Trim() != "0"  && row[colmax].Trim() != "0" && row[colmin].Trim() != "0" && row[colact].Trim() != "0" && row[colnom].Trim() != "0" && row[colflag].Trim() == "=")
                    {
                        frows.Add(row);
                        spoints.Add(x);
                        sfound = true;
                    }
                }
                else
                {
                    frows.Add(row);
                    spoints.Add(x);
                    if (row[colcsv].Trim() == "0" || row[colmax].Trim() == "0" || row[colmin].Trim() == "0" || row[colact].Trim() == "0" || row[colnom].Trim() == "0" || row[colflag].Trim() != "=")
                    {
                        frows.RemoveAt(frows.Count - 1);
                        spoints.RemoveAt(spoints.Count - 1);
                    }
                }
            }


            if(spoints.Count() < 1)
            {
                throw new ArgumentException("no items found from scan point");
            }

            if (points.Count() < 1)
            {
                throw new ArgumentException("no items fetched from profile point");
            }


            if (form == BladeForm.Outside) { 
                if(spoints[0] < points[0])
                {
                    var sidx2 = spoints.IndexOf(spoints.OrderBy(x => Math.Abs(x - points[0])).First());
                    spoints.RemoveRange(0, sidx2);
                    frows.RemoveRange(0, sidx2);
                }
            }
            else
            {
                if (spoints[0] > points[0])
                {
                    var sidx2 = spoints.IndexOf(spoints.OrderBy(x => Math.Abs(x - points[0])).First());
                    spoints.RemoveRange(0, sidx2);
                    frows.RemoveRange(0, sidx2);
                }
            }

            if (spoints.Count() > points.Count())
            {
                spoints.RemoveRange(points.Count(), (spoints.Count() - points.Count()));
            }

            //todo - add subroutine to filter the max delta ?? do we really need it? 
            //revese list for insideform

            var res = new List<ScanPoint>();
            var tl = 0;


            for (int i = 0; i < spoints.Count(); i++)
            {
                var spoint = spoints[i];
                var pval = points.OrderBy(x => Math.Abs(x - spoint)).First();
                var act = ParseCell(frows[i][colact]);
                int? tlc = null;
                if(frows[i][coltl].Trim().ToUpperInvariant() == "Y")
                {
                    tlc = tl++;
                }
                res.Add(new ScanPoint
                {
                    X = pval,
                    Y = y,
                    Thickness = act,
                    TrafficLight = tlc
                }); 
            }

            return res;
        }
        
        private Profile SaveProfile(Profile profile)
        {

            using (var con = new SqlConnection(constring))
            {
                con.Open();
                using (var trans = con.BeginTransaction())
                {
                    //insert profile
                    var qry = @"INSERT INTO [skin_thickness_contactless].[dbo].[profile]
		                        ([name]
		                        ,[pro_filename]
		                        ,[omin_filename]
		                        ,[omax_filename]
		                        ,[onom_filename]
		                        ,[imin_filename]
		                        ,[imax_filename]
		                        ,[inom_filename])
	                        VALUES
		                        (@name,
                                @pro_filename,
                                @omin_filename,
                                @omax_filename,
                                @onom_filename,
                                @imin_filename,
                                @imax_filename,
		                        @inom_filename);
                         SELECT CAST(SCOPE_IDENTITY() as int)";
                    var paramProfile = new
                    {
                        name = profile.Name,
                        pro_filename = profile.Pro_Filename,
                        omin_filename = profile.Omin_Filename,
                        omax_filename = profile.Omax_Filename,
                        onom_filename = profile.Onom_Filename,
                        imin_filename = profile.Imin_Filename,
                        imax_filename = profile.Imax_Filename,
                        inom_filename = profile.Inom_Filename
                    };
                    profile.id = con.QuerySingle<int>(qry, paramProfile, transaction: trans);
                    //insert rows
                    var rows = new List<ProfileRow>();
                    rows.AddRange(profile.InsideForm);
                    rows.AddRange(profile.OutsideForm);
                    foreach (var row in rows)
                    {
                        qry = @"INSERT INTO [skin_thickness].[dbo].[profile_row]
                               ([profile_id]
                               ,[form]
                               ,[y]
                               ,[le]
                               ,[te]
                               ,[scan_start]
                               ,[scan_stop]
                               ,[tolerances_start]
                               ,[tolerances_stop])
                         VALUES
                               (@profile_id
                               ,@form
                               ,@y
                               ,@le
                               ,@te
                               ,@scan_start
                               ,@scan_stop
                               ,@tolerances_start
                               ,@tolerances_stop);
	                    SELECT CAST(SCOPE_IDENTITY() as int)";
                        var paramRow = new
                        {
                            profile_id = profile.id,
                            form = row.Form,
                            y = row.Y,
                            le = row.le,
                            te = row.te,
                            scan_start = row.ScanStart,
                            scan_stop = row.ScanStop,
                            tolerances_start = row.TolerancesStart,
                            tolerances_stop = row.TolerancesStop
                        };
                        row.id = con.QuerySingle<int>(qry, paramRow, transaction:trans);
                        //insert points
                        foreach (var point in row.Points)
                        {
                            qry = @"INSERT INTO [skin_thickness].[dbo].[profile_point]
		                            ([profile_row_id]
                                    ,[x]
                                    ,[y]
                                    ,[min]
                                    ,[max]
                                    ,[nom]
                                    ,[tl])
	                            VALUES
		                            (@profile_row_id
                                    ,@x
                                    ,@y
                                    ,@min
                                    ,@max
                                    ,@nom
                                    ,@tl_num)
	                            SELECT CAST(SCOPE_IDENTITY() as int)";
                            var paramPoint = new
                            {
                                profile_row_id = row.id,
                                x = point.X,
                                y = point.Y,
                                min = point.Min,
                                max = point.Max,
                                nom = point.Nom,
                                tl_num = point.TL_Number
                            };
                            point.id = con.QuerySingle<int>(qry, paramPoint, transaction:trans);
                        }
                    }
                    trans.Commit();
                }
            }

            return profile;
        }

        private Profile BuildProfile(string [] profileFiles)
        {
            List<string[]> ominList, onomList, omaxList, iminList, inomList, imaxList;

            var profile = new Profile();
            profile.Name = Path.GetFileNameWithoutExtension(profileFiles[0]);
            profile.Pro_Filename = Path.Combine(oemInputPath, profileFiles[0]);
            profile.Imax_Filename = RemapPath(profileFiles[6]);
            profile.Imin_Filename = RemapPath(profileFiles[5]);
            profile.Inom_Filename = RemapPath(profileFiles[4]);
            profile.Omax_Filename = RemapPath(profileFiles[3]);
            profile.Omin_Filename = RemapPath(profileFiles[2]);
            profile.Onom_Filename = RemapPath(profileFiles[1]);

            ominList = ParseCsv(profile.Omin_Filename);
            onomList = ParseCsv(profile.Onom_Filename);
            omaxList = ParseCsv(profile.Omax_Filename);
            iminList = ParseCsv(profile.Imin_Filename);
            inomList = ParseCsv(profile.Inom_Filename);
            imaxList = ParseCsv(profile.Imax_Filename);


            //outside blade fprm
            var maxOutPoint = 0;
            for (int i = 12; i < ominList.Count; i++) //why not taking row 11
            {
                if (!string.IsNullOrWhiteSpace(ominList[i][0]))
                {
                    maxOutPoint++;
                }
                else
                {
                    break;
                }
            }

            var outProfRows = new List<ProfileRow>();
            for (var i = 1; i < ominList[1].Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(ominList[1][i]))
                {
                    var profRow = new ProfileRow();
                    profRow.Form = BladeForm.Outside;
                    profRow.Y = decimal.Parse(ominList[1][i]);
                    profRow.TolerancesStart = Math.Round(decimal.Parse(ominList[3][i]), 3);
                    profRow.TolerancesStop = Math.Round(decimal.Parse(ominList[6][i]), 3);
                    profRow.ScanStart = profRow.TolerancesStart;
                    profRow.ScanStop = profRow.TolerancesStop;
                    profRow.te = Math.Round(decimal.Parse(ominList[8][i]), 3);
                    profRow.le = 0.0m;
                    var tf = 0;
                    List<ProfilePoint> profPoints = new List<ProfilePoint>();
                    for (var j = 0; j < maxOutPoint; j++)
                    {
                        var line = j + 12;
                        if (!string.IsNullOrWhiteSpace(ominList[line][i]))
                        {
                            ProfilePoint profPoint = new ProfilePoint();
                            profPoint.X = Math.Round(profRow.TolerancesStart + decimal.Parse(ominList[line][0]), 3);
                            profPoint.Y = profRow.Y;
                            if (onomList[line][i].Trim().StartsWith("-"))
                            {
                                // strip the - & add a TLP to this row.
                                onomList[line][i] = onomList[line][i].Trim().Substring(1);
                                profPoint.TL_Number = tf++;
                            }
                            profPoint.Min = Math.Round(ParseCell(ominList[line][i]), 3);//float.Parse(string.IsNullOrEmpty(Imin_File[line][y + 1]) ? "0.0" : Imin_File[line][y + 1]);
                            profPoint.Nom = Math.Round(ParseCell(onomList[line][i]), 3);//float.Parse(string.IsNullOrEmpty(Inom_File[line][y + 1]) ? "0.0" : Inom_File[line][y + 1]);
                            profPoint.Max = Math.Round(ParseCell(omaxList[line][i]), 3);//float.Parse(string.IsNullOrEmpty(Imax_File[line][y + 1]) ? "0.0" : Imax_File[line][y + 1]);
                            profPoints.Add(profPoint);
                        }
                    }
                    profRow.Points = profPoints.ToArray();
                    outProfRows.Add(profRow);
                }
            }


            var maxinPoint = 0;
            for (var i = 12; i < iminList.Count; i++) //why not taking row 11
            {
                if (!string.IsNullOrWhiteSpace(iminList[i][0]))
                {
                    maxinPoint++;
                }
                else
                {
                    break;
                }
            }

            var inProfRows = new List<ProfileRow>();
            for (var i = 1; i < iminList[1].Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(iminList[1][i]))
                {
                    var profRow = new ProfileRow();
                    profRow.Form = BladeForm.Inside;
                    profRow.Y = decimal.Parse(iminList[1][i]);
                    profRow.TolerancesStart = Math.Round(-decimal.Parse(iminList[3][i]), 3);
                    profRow.TolerancesStop = Math.Round(-decimal.Parse(iminList[6][i]), 3);
                    profRow.ScanStart = profRow.TolerancesStart;
                    profRow.ScanStop = profRow.TolerancesStop;
                    profRow.te = Math.Round(decimal.Parse(iminList[8][i]), 3);
                    profRow.le = 0.0m;
                    var tf = 0;
                    List<ProfilePoint> profPoints = new List<ProfilePoint>();
                    for (var j = 0; j < maxinPoint; j++)
                    {
                        var line = j + 12;
                        if (!string.IsNullOrWhiteSpace(iminList[line][i]))
                        {
                            ProfilePoint profPoint = new ProfilePoint();
                            profPoint.X = Math.Round(profRow.TolerancesStart - decimal.Parse(iminList[line][0]), 3);
                            profPoint.Y = profRow.Y;
                            var tlv = inomList[line][i].Trim();
                            if (inomList[line][i].Trim().StartsWith("-"))
                            {
                                // strip the - & add a TLP to this row.
                                inomList[line][i] = inomList[line][i].Substring(1);
                                profPoint.TL_Number = tf++;
                            }
                            profPoint.Min = ParseCell(iminList[line][i]);//float.Parse(string.IsNullOrEmpty(Imin_File[line][y + 1]) ? "0.0" : Imin_File[line][y + 1]);
                            profPoint.Nom = ParseCell(inomList[line][i]);//float.Parse(string.IsNullOrEmpty(Inom_File[line][y + 1]) ? "0.0" : Inom_File[line][y + 1]);
                            profPoint.Max = ParseCell(imaxList[line][i]);//float.Parse(string.IsNullOrEmpty(Imax_File[line][y + 1]) ? "0.0" : Imax_File[line][y + 1]);
                            profPoints.Add(profPoint);
                        }
                    }
                    profRow.Points = profPoints.ToArray();
                    inProfRows.Add(profRow);
                }
            }


            profile.OutsideForm = outProfRows.ToArray();
            profile.InsideForm = inProfRows.ToArray();

            return profile;

        }

        private decimal ParseCell(string CellData)
        {
            decimal ReturnValue;
            if (string.IsNullOrEmpty(CellData) || string.IsNullOrWhiteSpace(CellData))
            {
                CellData = "0.0";
                return 0.0m;
            }

            if (!decimal.TryParse(CellData, out ReturnValue))
            {
                throw new InvalidDataException("Unable to Parse Cell Value.");
            }    
            return ReturnValue;
        }

    }

    public class Pdata
    {
        public int? tl { get; set; }
        public decimal x { get; set; }
    }
    public class Tdata
    {
        public decimal X { get; set; }
        public decimal X_nearest { get; set; }
        public string tlight { get; set; }
        public int tlcount { get; set; }
        public int? tlcount_profile { get; set; }
    }
}




