using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace DAL
{
    public class CSVLine
    {
        public CSVLine(string side, string name, string number, string leadingEdgeTranslation, List<string[]> rows, string profileName)
        {
            this.profileName = profileName;
            this.side = side.Trim();
            this.name = name.Trim();
            this.number = number.Trim();
            this.leadingEdgeTranslation = leadingEdgeTranslation.Trim();
            this.rows = rows;
            ScanLine();
        }

        private const int colx = 0;
        private const int coly = 1;
        private const int colact = 7;
        private const int colnom = 8;
        private const int colmin = 10;
        private const int colmax = 11;
        private const int coltl = 9;
        private const int colcsv = 6;
        private const int colflag = 12;
        private readonly string side;
        private readonly string name;
        private readonly string number;
        private readonly string profileName;
        private readonly string leadingEdgeTranslation;
        private readonly List<string[]> rows = new List<string[]>();
        private ScanRow scanRow;
        //private ProfileRow profileRow;


        //public List<float> GetScanRows()
        //{
        //    string constring = DAL.strConn;
        //    using (var con = new SqlConnection(constring))
        //    {
        //        var qry = @"SELECT pp.x 
        //                    from profile p
        //                    join profile_row pr on pr.profile_id = p.id
        //                    join profile_point pp on pp.profile_row_id = pr.id
        //                    where p.name = @pname and pr.form = @Side and pr.y = @y";
        //        return con.Query<float>(qry, new { pname = profileName, Side = Side, y = name }).ToList();
        //    }

        //}

        private float GetColumnValueAsFloat(string DataIn,  float defaultvalue = 0.0f,  NumberStyles style = NumberStyles.Number,  CultureInfo culture = null)
        {
            culture = culture?? CultureInfo.InvariantCulture;
            return float.TryParse(DataIn, style, (IFormatProvider)culture, out float result) && !string.IsNullOrWhiteSpace(DataIn) ? result : defaultvalue;
        }
        private int GetColumnValueAsInt(string DataIn, int defaultvalue = 0, NumberStyles style = NumberStyles.Number, CultureInfo culture = null)
        {
            culture = culture ?? CultureInfo.InvariantCulture;
            return int.TryParse(DataIn, style, (IFormatProvider)culture, out int result) && !string.IsNullOrWhiteSpace(DataIn) ? result : defaultvalue;
        }
        private double GetColumnValueAsDouble(string DataIn, double defaultvalue = 0.0, NumberStyles style = NumberStyles.Number, CultureInfo culture = null)
        {
            culture = culture ?? CultureInfo.InvariantCulture;
            return double.TryParse(DataIn, style, (IFormatProvider)culture, out double result) && !string.IsNullOrWhiteSpace(DataIn) ? result : defaultvalue;
        }
        private void ScanLine()
        {
            var cps = new float[2]; // GetScanRows().ToArray();
            List<ScanPoint> scanPointList = new List<ScanPoint>();
            //List<ProfilePoint> profilePointList = new List<ProfilePoint>();
            var tlnum = 0;
            var frowfound = false;
            var lrowfound = false;
            //float curX = 0.0f;
            //float fle = 0.0f;
            //float fte = 0.0f;
            //float fscanstart = 0.0f;
            //float ftolstart = 0.0f;
            //float ftolstop = 0.0f;
            for (int i = 0; i < rows.Count; i++)
            {


                var row = rows[i];

                var idx = 0;
                var offset = 0;
                var x = GetColumnValueAsFloat(row[colx]);
                if (Side == BladeForm.Inside)
                {
                    x = -x;
                }
                if (!frowfound)
                {
                    if (row[colmax].Trim() != "0" && row[colmin].Trim() != "0" && row[colact].Trim() != "0" && row[colnom].Trim() != "0" && row[colflag].Trim() == "=")
                    {
                        frowfound = true;
                        idx = Array.IndexOf(cps, cps.OrderBy(a => Math.Abs(x - a)).First());
                        offset = i;
                    }
                }

                if (frowfound && !lrowfound && idx > 0)
                {
                    
                    ScanPoint scanPoint = new ScanPoint();
                    //ProfilePoint profilePoint = new ProfilePoint();


                    var act = GetColumnValueAsFloat(row[colact]);
                    var max = GetColumnValueAsFloat(row[colmax]);
                    var min = GetColumnValueAsFloat(row[colmin]);
                    var nom = GetColumnValueAsFloat(row[colnom]);
                    var stl = row[coltl].Trim().ToUpperInvariant();
                    scanPoint.Thickness = act;
                    scanPoint.X = x;
                    scanPoint.Y = Y;
                    scanPoint.XP = cps[idx + (i - offset)];
                    //profilePoint.Max = max;
                    //profilePoint.Min = min;
                    //profilePoint.Nom = nom;
                    //profilePoint.X = x;
                    //profilePoint.Y = Y;
                    if (stl == "Y")
                    {
                        scanPoint.TrafficLight = tlnum;
                        //profilePoint.TL_Number = tlnum;
                        ++tlnum;
                    }
                    scanPointList.Add(scanPoint);
                }







                //profilePointList.Add(profilePoint);

                //if (fscanstart == 0.0f)
                //{
                //    fscanstart = GetColumnValueAsFloat(row[colx]);
                //}
                //if (fle == 0.0f && GetColumnValueAsFloat(row[colact]) != 0.0f)
                //{
                //    fle = GetColumnValueAsFloat(row[colx], 0.0f);
                //}

                //if (ftolstart == 0.0f && GetColumnValueAsFloat(row[colnom]) != 0.0f)
                //{
                //    ftolstart = GetColumnValueAsFloat(row[colx]);
                //}

                //if (ftolstart != 0.0f)
                //{
                //    if (ftolstop == 0.0f && GetColumnValueAsFloat(row[colnom]) == 0.0f)
                //        ftolstop = curX;
                //    if (fte == 0.0f && GetColumnValueAsFloat(row[colact]) == 0.0f)
                //        fte = curX;
                //}
                //curX = GetColumnValueAsFloat(row[colx]);
            }

            ScanRow = new ScanRow()
            {
                Form = Side,
                Points = scanPointList.ToArray(),
                DeltaX = GetColumnValueAsFloat(leadingEdgeTranslation),
                Y = Y
            };
            //ProfileRow = new ProfileRow()
            //{
            //    Form = Side,
            //    Points = profilePointList.ToArray(),
            //    Y = Y,
            //    le = fle,
            //    te = fte,
            //    ScanStart = fscanstart,
            //    ScanStop = GetColumnValueAsFloat(rows[rows.Count-1][colx]),
            //    TolerancesStart = ftolstart,
            //    TolerancesStop = ftolstop
            //};
        }
        private BladeForm Side => side == "Inside form" ? BladeForm.Inside : BladeForm.Outside;
        private int Y => GetColumnValueAsInt(name);
        public int Count => rows.Count;
        public ScanRow ScanRow { get => scanRow; set => scanRow = value; }
        //public ProfileRow ProfileRow { get => profileRow; set => profileRow = value; }
    }
}
