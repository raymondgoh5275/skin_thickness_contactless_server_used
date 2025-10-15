using ATS_Global.CSV;
using DAL;
using System;
using System.Collections.Generic;
using System.IO;
namespace BLL
{
    public class ProfileStorage : Profile
    {
        public ProfileStorage()
            : base()
        {
        }

        public bool Exists()
        {
            if (id > -1) return true;

            if (string.IsNullOrEmpty(this.Name))
            {
                this.Name = Path.GetFileNameWithoutExtension(this.Pro_Filename);
            }

            base.LoadFromName();

            if (id > -1)
                return true;
            else
                return false;
        }

        //build profile for contactless us machine
        //2024-05-22 - Ganesan.Kalianan
        //        
        
        public void BuildProfileContactless()
        {
            List<string[]> ominList, onomList, omaxList, iminList, inomList, imaxList;

            if (string.IsNullOrWhiteSpace(Name))
            {
                Name = Path.GetFileNameWithoutExtension(Pro_Filename);
            }

            ominList = CsvHelper.Load(this.Omin_Filename);
            onomList = CsvHelper.Load(this.Onom_Filename);
            omaxList = CsvHelper.Load(this.Omax_Filename);
            iminList = CsvHelper.Load(this.Imin_Filename);
            inomList = CsvHelper.Load(this.Inom_Filename);
            imaxList = CsvHelper.Load(this.Imax_Filename);


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
                    profRow.Y = float.Parse(ominList[1][i]);
                    profRow.TolerancesStart = float.Parse(ominList[3][i]);
                    profRow.TolerancesStop = float.Parse(ominList[6][i]);
                    profRow.ScanStart = profRow.TolerancesStart;
                    profRow.ScanStop = profRow.TolerancesStop;
                    profRow.te = float.Parse(ominList[8][i]);
                    profRow.le = 0.0f;
                    var tf = 0;
                    List<ProfilePoint> profPoints = new List<ProfilePoint>();
                    for (var j = 0; j < maxOutPoint; j++)
                    {
                        var line = j + 12;
                        if (!string.IsNullOrWhiteSpace(ominList[line][i]))
                        {
                            ProfilePoint profPoint = new ProfilePoint();
                            profPoint.X = profRow.TolerancesStart + float.Parse(ominList[line][0]);
                            profPoint.Y = profRow.Y;
                            if (onomList[line][i].Trim().StartsWith("-"))
                            {
                                // strip the - & add a TLP to this row.
                                onomList[line][i] = onomList[line][i].Trim().Substring(1);
                                profPoint.TL_Number = tf++;
                            }
                            profPoint.Min = ParseCell(ref ominList[line][i]);//float.Parse(string.IsNullOrEmpty(Imin_File[line][y + 1]) ? "0.0" : Imin_File[line][y + 1]);
                            profPoint.Nom = ParseCell(ref onomList[line][i]);//float.Parse(string.IsNullOrEmpty(Inom_File[line][y + 1]) ? "0.0" : Inom_File[line][y + 1]);
                            profPoint.Max = ParseCell(ref omaxList[line][i]);//float.Parse(string.IsNullOrEmpty(Imax_File[line][y + 1]) ? "0.0" : Imax_File[line][y + 1]);
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
                    profRow.Y = float.Parse(iminList[1][i]);
                    profRow.TolerancesStart = -float.Parse(iminList[3][i]);
                    profRow.TolerancesStop = -float.Parse(iminList[6][i]);
                    profRow.ScanStart = profRow.TolerancesStart;
                    profRow.ScanStop = profRow.TolerancesStop;
                    profRow.te = float.Parse(iminList[8][i]);
                    profRow.le = 0.0f;
                    var tf = 0;
                    List<ProfilePoint> profPoints = new List<ProfilePoint>();

                    for (var j = 0; j < maxinPoint; j++)
                    {
                        var line = j + 12;

                        if (!string.IsNullOrWhiteSpace(iminList[line][i]))
                        {
                            ProfilePoint profPoint = new ProfilePoint();
                            profPoint.X = profRow.TolerancesStart - float.Parse(iminList[line][0]);
                            profPoint.Y = profRow.Y;

                            if (inomList[line][i].Trim().StartsWith("-"))
                            {
                                // strip the - & add a TLP to this row.
                                inomList[line][i] = inomList[line][i].Substring(1);
                                profPoint.TL_Number = tf++;
                            }
                            profPoint.Min = ParseCell(ref iminList[line][i]);//float.Parse(string.IsNullOrEmpty(Imin_File[line][y + 1]) ? "0.0" : Imin_File[line][y + 1]);
                            profPoint.Nom = ParseCell(ref inomList[line][i]);//float.Parse(string.IsNullOrEmpty(Inom_File[line][y + 1]) ? "0.0" : Inom_File[line][y + 1]);
                            profPoint.Max = ParseCell(ref imaxList[line][i]);//float.Parse(string.IsNullOrEmpty(Imax_File[line][y + 1]) ? "0.0" : Imax_File[line][y + 1]);
                            profPoints.Add(profPoint);
                        }
                    }
                    profRow.Points = profPoints.ToArray();
                    inProfRows.Add(profRow);
                }
            }


            OutsideForm = outProfRows.ToArray();
            InsideForm = inProfRows.ToArray();

        }


        public void BuildProfile()
        {
            List<string[]> Omin_File, Onom_File, Omax_File, Imin_File, Inom_File, Imax_File;

            if (string.IsNullOrEmpty(this.Name))
            {
                string strTemp = this.Pro_Filename;
                int lastSlash = strTemp.LastIndexOf('\\') + 1;

                strTemp = strTemp.Substring(lastSlash);
                int extStart = strTemp.LastIndexOf('.');

                this.Name = strTemp.Remove(extStart);
            }

            Omin_File = CsvHelper.Load(this.Omin_Filename);
            Onom_File = CsvHelper.Load(this.Onom_Filename);
            Omax_File = CsvHelper.Load(this.Omax_Filename);
            Imin_File = CsvHelper.Load(this.Imin_Filename);
            Inom_File = CsvHelper.Load(this.Inom_Filename);
            Imax_File = CsvHelper.Load(this.Imax_Filename);

            int Outside_rowCount = 0;
            for (int i = 1; i < (Omin_File[0]).Length; i++)
            {
                if ((string.IsNullOrEmpty(Omin_File[0][i])) || (string.IsNullOrWhiteSpace(Omin_File[0][i])))
                {
                    continue;
                }
                Outside_rowCount++;
            }

            int MaxPointCount = 0;
            for (int i = 12; i < Omin_File.Count; i++)
            {
                if ((string.IsNullOrEmpty(Omin_File[i][0])) || (string.IsNullOrWhiteSpace(Omin_File[i][0])))
                {
                    continue;
                }
                MaxPointCount++;
            }

            OutsideForm = new ProfileRow[Outside_rowCount];

            for (int y = 0; y < Outside_rowCount; y++)
            {
                OutsideForm[y] = new ProfileRow();
                OutsideForm[y].Form = (BladeForm)Enum.Parse(typeof(BladeForm), Omin_File[0][y + 1]);
                OutsideForm[y].Y = float.Parse(Omin_File[2][y + 1]);
                OutsideForm[y].le = float.Parse(Omin_File[10][y + 1]);
                OutsideForm[y].te = float.Parse(Omin_File[11][y + 1]);
                OutsideForm[y].ScanStart = float.Parse(Omin_File[6][y + 1]);
                OutsideForm[y].ScanStop = float.Parse(Omin_File[7][y + 1]);
                OutsideForm[y].TolerancesStart = float.Parse(Omin_File[4][y + 1]);
                OutsideForm[y].TolerancesStop = float.Parse(Omin_File[5][y + 1]);

                List<ProfilePoint> temp_Outside_Points = new List<ProfilePoint>();

                int TraficLightCount = 0;
                for (int x = 0; x < MaxPointCount; x++)
                {
                    int line = 12 + x;

                    if ((!string.IsNullOrEmpty(Omin_File[line][y + 1])) || (!string.IsNullOrWhiteSpace(Omin_File[line][y + 1])))
                    {
                        ProfilePoint newOutPoint = new ProfilePoint();
                        newOutPoint.X = OutsideForm[y].TolerancesStart + float.Parse(Omin_File[line][0]);
                        newOutPoint.Y = float.Parse(Omin_File[2][y + 1]);

                        if (Onom_File[line][y + 1].StartsWith("*"))
                        {
                            // strip the * & add a TLP to this row.
                            Onom_File[line][y + 1] = Onom_File[line][y + 1].Substring(1);
                            newOutPoint.TL_Number = TraficLightCount++;
                        } // Don't forget to add this to Inside!

                        newOutPoint.Min = ParseCell(ref Omin_File[line][y + 1]);//float.Parse(string.IsNullOrEmpty(Omin_File[line][y + 1]) ? "0.0" : Omin_File[line][y + 1]);
                        newOutPoint.Nom = ParseCell(ref Onom_File[line][y + 1]);//float.Parse(string.IsNullOrEmpty(Onom_File[line][y + 1]) ? "0.0" : Onom_File[line][y + 1]);
                        newOutPoint.Max = ParseCell(ref Omax_File[line][y + 1]);//float.Parse(string.IsNullOrEmpty(Omax_File[line][y + 1]) ? "0.0" : Omax_File[line][y + 1]);

                        temp_Outside_Points.Add(newOutPoint);
                    }
                }

                OutsideForm[y].Points = temp_Outside_Points.ToArray();
            }

            int Inside_rowCount = 0;
            for (int i = 1; i < (Imin_File[0]).Length; i++)
            {
                if ((string.IsNullOrEmpty(Imin_File[0][i])) || (string.IsNullOrWhiteSpace(Imin_File[0][i])))
                {
                    continue;
                }
                Inside_rowCount++;
            }

            MaxPointCount = 0;
            for (int i = 12; i < Imin_File.Count; i++)
            {
                if ((string.IsNullOrEmpty(Imin_File[i][0])) || (string.IsNullOrWhiteSpace(Imin_File[i][0])))
                {
                    continue;
                }
                MaxPointCount++;
            }

            InsideForm = new ProfileRow[Inside_rowCount];

            for (int y = 0; y < Inside_rowCount; y++)
            {
                InsideForm[y] = new ProfileRow();
                InsideForm[y].Form = (BladeForm)Enum.Parse(typeof(BladeForm), Imin_File[0][y + 1]);
                InsideForm[y].Y = float.Parse(Imin_File[2][y + 1]);
                InsideForm[y].le = float.Parse(Imin_File[10][y + 1]);
                InsideForm[y].te = float.Parse(Imin_File[11][y + 1]);
                InsideForm[y].ScanStart = -float.Parse(Imin_File[6][y + 1]);
                InsideForm[y].ScanStop = -float.Parse(Imin_File[7][y + 1]);
                InsideForm[y].TolerancesStart = -float.Parse(Imin_File[4][y + 1]);
                InsideForm[y].TolerancesStop = -float.Parse(Imin_File[5][y + 1]);

                List<ProfilePoint> temp_Inside_Points = new List<ProfilePoint>();

                int TraficLightCount = 0;
                for (int x = 0; x < MaxPointCount; x++)
                {
                    int line = x + 12;

                    if ((!string.IsNullOrEmpty(Imin_File[line][y + 1])) || (!string.IsNullOrWhiteSpace(Imin_File[line][y + 1])))
                    {
                        ProfilePoint newOutPoint = new ProfilePoint();
                        newOutPoint.X = InsideForm[y].TolerancesStart - float.Parse(Imin_File[line][0]);
                        newOutPoint.Y = float.Parse(Imin_File[2][y + 1]);

                        if (Inom_File[line][y + 1].StartsWith("*"))
                        {
                            // strip the * & add a TLP to this row.
                            Inom_File[line][y + 1] = Inom_File[line][y + 1].Substring(1);
                            newOutPoint.TL_Number = TraficLightCount++;
                        }

                        newOutPoint.Min = ParseCell(ref Imin_File[line][y + 1]);//float.Parse(string.IsNullOrEmpty(Imin_File[line][y + 1]) ? "0.0" : Imin_File[line][y + 1]);
                        newOutPoint.Nom = ParseCell(ref Inom_File[line][y + 1]);//float.Parse(string.IsNullOrEmpty(Inom_File[line][y + 1]) ? "0.0" : Inom_File[line][y + 1]);
                        newOutPoint.Max = ParseCell(ref Imax_File[line][y + 1]);//float.Parse(string.IsNullOrEmpty(Imax_File[line][y + 1]) ? "0.0" : Imax_File[line][y + 1]);

                        temp_Inside_Points.Add(newOutPoint);
                    }
                }

                InsideForm[y].Points = temp_Inside_Points.ToArray();
            }
        }

        private float ParseCell(ref string CellData)
        {
            float ReturnValue;
            if (string.IsNullOrEmpty(CellData) || string.IsNullOrWhiteSpace(CellData))
            {
                CellData = "0.0";
                return 0.0f;
            }

            if (!float.TryParse(CellData, out ReturnValue))
            {
                int tmp;
                if (int.TryParse(CellData, out tmp))
                    ReturnValue = (float)tmp;
                else
                    throw new InvalidDataException("Unable to Parse Cell Value.");
            }

            return ReturnValue;
        }
    }
}
