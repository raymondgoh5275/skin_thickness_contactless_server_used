using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using DAL;
using System.Drawing;
using System.IO;
using System.Globalization;
using BLL.Drawing;

namespace BLL
{
    public class BizLogic
    {
        public BizLogic()
        {

        }

        public static void HouseKeeping()
        {
            DAL.DAL.HouseKeeping();
        }

        public void ProcessScanResults(XmlDocument MES_OpperationCompleat)
        {
            ScanStorage DATfile = new ScanStorage();
            DATfile.Load(@"Z:\skin thickness\RGX10448\RGX10448.dat");
            DATfile.Save();
        }

        public void ProcessScanResults(string ScanFilePath)
        {
            ScanStorage DATfile = new ScanStorage();
            if (DATfile.Load(ScanFilePath))
            {
                DATfile.Save();
            }
            else
            {
                throw new Exception(string.Format("{0}, XML File {1} not found.", DateTime.Now, ScanFilePath));
            }
        }

        public Dictionary<string, string> Get_ProfileList()
        {
            return Profile.Get_ProfileList();
        }

        public Dictionary<string, string> Get_ProfileList(string BladeList)
        {
            return Profile.Get_ProfileList(BladeList);
        }

        public Dictionary<string, string> GetBladeList_From_ProfileID(int id)
        {
            return Scan.GetList_From_ProfileID(id);
        }

        public Dictionary<string, string> GetBladeList_From_ProfileID(int id, string BladeList)
        {
            return Scan.GetList_From_ProfileID(id, BladeList);
        }

        public List<string> GetRowList_From_ProfileID(int profileId, byte byteForm)
        {
            BladeForm form;

            if (byteForm == 0)
                form = BladeForm.Outside;
            else
                form = BladeForm.Inside;

            return Scan.GetRows_From_ProfileID(profileId, form);
        }

        public List<string> GetColList_From_ProfileID(int profileId, byte byteForm, int intRow)
        {
            BladeForm form;

            if (byteForm == 0)
                form = BladeForm.Outside;
            else
                form = BladeForm.Inside;

            return Scan.GetCols_From_ProfileID(profileId, form, intRow);
        }

        public Bitmap ContourBladeImage(int[] bladeids, byte byteForm, byte byteCalculate, byte byteCompareTo, float Scale, float Offset)
        {
            List<pointData> BladeData;
            BladeForm form;

            if (byteForm == 0)
                form = BladeForm.Outside;
            else
                form = BladeForm.Inside;

            BladeData = Scan.ThicknessStatistics(bladeids, form);

            float minX = 65535f, maxX = 0f, minY = 65535f, maxY = 0f;

            if (byteCalculate != 0)
                byteCompareTo = 3;

            // iterate through results
            for (int i = 0; i < BladeData.Count; i++)
            {
                if (maxX < float.Parse(BladeData[i].X.ToString("F0", CultureInfo.InvariantCulture)))
                    maxX = float.Parse(BladeData[i].X.ToString("F0", CultureInfo.InvariantCulture));
                if (minX > float.Parse(BladeData[i].X.ToString("F0", CultureInfo.InvariantCulture)))
                    minX = float.Parse(BladeData[i].X.ToString("F0", CultureInfo.InvariantCulture));

                if (maxY < float.Parse(BladeData[i].Y.ToString("F0", CultureInfo.InvariantCulture)))
                    maxY = float.Parse(BladeData[i].Y.ToString("F0", CultureInfo.InvariantCulture));
                if (minY > float.Parse(BladeData[i].Y.ToString("F0", CultureInfo.InvariantCulture)))
                    minY = float.Parse(BladeData[i].Y.ToString("F0", CultureInfo.InvariantCulture));

                switch (byteCalculate)
                {
                    case 0: // Average
                        switch (byteCompareTo)
                        {
                            case 0: // min
                                BladeData[i].DrawValue = BladeData[i].Average - BladeData[i].MinT;
                                break;
                            case 1: // nom
                                BladeData[i].DrawValue = BladeData[i].Average - BladeData[i].NomT;
                                break;
                            case 2: // max
                                BladeData[i].DrawValue = BladeData[i].Average - BladeData[i].MaxT;
                                break;
                            case 3: // nothing
                                BladeData[i].DrawValue = BladeData[i].Average;
                                break;
                            default:
                                BladeData[i].DrawValue = -2f;
                                break;
                        }
                        break;
                    case 1: // Standard deviation
                        BladeData[i].DrawValue = BladeData[i].STDev;
                        break;
                    case 2: // Spread / max - min
                        BladeData[i].DrawValue = BladeData[i].Max - BladeData[i].Min;
                        break;
                    case 3: // Z usl
                        BladeData[i].DrawValue = Zusl(BladeData[i]);
                        break;
                    case 4: // Z lsl
                        BladeData[i].DrawValue = Zlsl(BladeData[i]);
                        break;
                    case 5: // Z min
                        BladeData[i].DrawValue = Zmin(BladeData[i]);
                        break;
                    case 6: // Pp
                        BladeData[i].DrawValue = Pp(BladeData[i]);
                        break;
                    case 7: // Ppk
                        BladeData[i].DrawValue = Ppk(BladeData[i]);
                        break;
                    default:
                        BladeData[i].DrawValue = 65535.0f;
                        break;
                }
            }

            return BuildBladeImage(maxX, maxY, minX, minY, Scale, Offset, BladeData, new Int32[] { -7798785, -11141291, -8913033, -6684775, -4456517, -2228259, -1, -8739, -17477, -26215, -34953, -43691, -28673, -16777216 });
        }

        public Bitmap ReworkAnalysisBladeImage(int bladeid, byte byteForm, float Scale, float Offset)
        {
            List<pointData> avarageBlade;
            BladeForm form;

            float minX = 65535f, maxX = 0f, minY = 65535f, maxY = 0f;

            if (byteForm == 0)
                form = BladeForm.Outside;
            else
                form = BladeForm.Inside;

            avarageBlade = Scan.Thickness(bladeid, form);

            for (int i = 0; i < avarageBlade.Count; i++)
            {
                if (maxX < float.Parse(avarageBlade[i].X.ToString("F0", CultureInfo.InvariantCulture)))
                    maxX = float.Parse(avarageBlade[i].X.ToString("F0", CultureInfo.InvariantCulture));
                if (minX > float.Parse(avarageBlade[i].X.ToString("F0", CultureInfo.InvariantCulture)))
                    minX = float.Parse(avarageBlade[i].X.ToString("F0", CultureInfo.InvariantCulture));

                if (maxY < float.Parse(avarageBlade[i].Y.ToString("F0", CultureInfo.InvariantCulture)))
                    maxY = float.Parse(avarageBlade[i].Y.ToString("F0", CultureInfo.InvariantCulture));
                if (minY > float.Parse(avarageBlade[i].Y.ToString("F0", CultureInfo.InvariantCulture)))
                    minY = float.Parse(avarageBlade[i].Y.ToString("F0", CultureInfo.InvariantCulture));

                avarageBlade[i].DrawValue = avarageBlade[i].Average - avarageBlade[i].MinT;
            }


            return BuildBladeImage(maxX, maxY, minX, minY, Scale, Offset, avarageBlade, new Int32[] { Int32.Parse("ff00ffff", System.Globalization.NumberStyles.HexNumber), Int32.Parse("ff880088", System.Globalization.NumberStyles.HexNumber), Int32.Parse("ff880000", System.Globalization.NumberStyles.HexNumber), Int32.Parse("ff008888", System.Globalization.NumberStyles.HexNumber), Int32.Parse("ff008800", System.Globalization.NumberStyles.HexNumber), Int32.Parse("ff000088", System.Globalization.NumberStyles.HexNumber), Int32.Parse("ffff0000", System.Globalization.NumberStyles.HexNumber), Int32.Parse("ffff0000", System.Globalization.NumberStyles.HexNumber), Int32.Parse("ffff0000", System.Globalization.NumberStyles.HexNumber), Int32.Parse("ffff0000", System.Globalization.NumberStyles.HexNumber), Int32.Parse("ffff0000", System.Globalization.NumberStyles.HexNumber), Int32.Parse("ffff0000", System.Globalization.NumberStyles.HexNumber), Int32.Parse("ffff0000", System.Globalization.NumberStyles.HexNumber), Int32.Parse("ffff0000", System.Globalization.NumberStyles.HexNumber) });
        }

        public Bitmap MetalRemovalBladeImage(int intBladeAid, int intBladeBid, byte byteForm, float Scale, float Offset)
        {
            List<pointData> BladeA, BladeB, BladeC;
            BladeForm form;

            if (byteForm == 0)
                form = BladeForm.Outside;
            else
                form = BladeForm.Inside;


            BladeA = Scan.Thickness(intBladeAid, form);
            BladeB = Scan.Thickness(intBladeBid, form);
            BladeC = new List<pointData>();

            float minX = 65535f, maxX = 0f, minY = 65535f, maxY = 0f;

            if (BladeA.Count != BladeB.Count)
                return new Bitmap(5, 5);

            for (int i = 0; i < BladeA.Count; i++)
            {
                pointData tmp = new pointData();
                tmp = BladeA[i];

                if (maxX < float.Parse(tmp.X.ToString("F0", CultureInfo.InvariantCulture)))
                    maxX = float.Parse(tmp.X.ToString("F0", CultureInfo.InvariantCulture));
                if (minX > float.Parse(tmp.X.ToString("F0", CultureInfo.InvariantCulture)))
                    minX = float.Parse(tmp.X.ToString("F0", CultureInfo.InvariantCulture));

                if (maxY < float.Parse(tmp.Y.ToString("F0", CultureInfo.InvariantCulture)))
                    maxY = float.Parse(tmp.Y.ToString("F0", CultureInfo.InvariantCulture));
                if (minY > float.Parse(tmp.Y.ToString("F0", CultureInfo.InvariantCulture)))
                    minY = float.Parse(tmp.Y.ToString("F0", CultureInfo.InvariantCulture));

                tmp.DrawValue = BladeA[i].Average - BladeB[i].Average;

                BladeC.Add(tmp);
            }

            return BuildBladeImage(maxX, maxY, minX, minY, Scale, Offset, BladeC, new Int32[] { -7798785, -11141291, -8913033, -6684775, -4456517, -2228259, -1, -8739, -17477, -26215, -34953, -43691, -28673, -16777216 });
        }

        private static Bitmap BuildBladeImage(float MaxX, float MaxY, float MinX, float MinY, float Scale, float Offset, List<pointData> ResultData, Int32[] ColourLookup)
        {
            Rasterizer ReturnImage;
            ColourLookUp CLU = new ColourLookUp(Scale, Offset, ColourLookup);

            Dictionary<float, Dictionary<float, pointData>> pointGrid = new Dictionary<float, Dictionary<float, pointData>>();
            foreach (pointData item in ResultData)
            {
                if (!pointGrid.ContainsKey(item.Y))
                    pointGrid.Add(item.Y, new Dictionary<float, pointData>());

                //if (!pointGrid.ContainsKey(item.Y))
                    pointGrid[item.Y].Add(item.X, item);
            }
            pointData[][] arrPointData = new pointData[pointGrid.Count][];
            int Yindex = 0;
            foreach (KeyValuePair<float, Dictionary<float, pointData>> Ykvp in pointGrid)
            {
                arrPointData[Yindex] = new pointData[Ykvp.Value.Count];
                int Xindex = 0;
                foreach (KeyValuePair<float, pointData> Xkvp in Ykvp.Value)
                {
                    arrPointData[Yindex][Xindex++] = Xkvp.Value;
                }
                Yindex++;
            }

            ReturnImage = new Rasterizer((int)MinX, (int)MaxX, (int)MinY, (int)MaxY);
            for (int y = 1; y < arrPointData.Length; y++)
            {
                int Xo1 = 0, Xo2 = 0;
                while ((Xo2 < arrPointData[y].Length - 1) || (Xo1 < arrPointData[y - 1].Length - 1))
                {
                    if (Xo1 < (arrPointData[y - 1].Length - 1)) Xo1++;
                    if (Xo2 < (arrPointData[y].Length - 1)) Xo2++;

                    ReturnImage.DrawTriangle(
                        CLU.ScaledColour(arrPointData[y - 1][Xo1].DrawValue), (int)arrPointData[y - 1][Xo1].X, (int)arrPointData[y - 1][Xo1].Y,
                        CLU.ScaledColour(arrPointData[y][Xo2 - 1].DrawValue), (int)arrPointData[y][Xo2 - 1].X, (int)arrPointData[y][Xo2 - 1].Y,
                        CLU.ScaledColour(arrPointData[y - 1][Xo1 - 1].DrawValue), (int)arrPointData[y - 1][Xo1 - 1].X, (int)arrPointData[y - 1][Xo1 - 1].Y,
                        true);

                    ReturnImage.DrawTriangle(
                        CLU.ScaledColour(arrPointData[y][Xo2].DrawValue), (int)arrPointData[y][Xo2].X, (int)arrPointData[y][Xo2].Y,
                        CLU.ScaledColour(arrPointData[y][Xo2 - 1].DrawValue), (int)arrPointData[y][Xo2 - 1].X, (int)arrPointData[y][Xo2 - 1].Y,
                        CLU.ScaledColour(arrPointData[y - 1][Xo1].DrawValue), (int)arrPointData[y - 1][Xo1].X, (int)arrPointData[y - 1][Xo1].Y,
                        true);
                }
            }

            return ReturnImage.Result;
        }

        public SPC_Data Create_SPCReport(int profileId, byte byteForm, float intX, float intY, string SampleSize, DateTime DateStart, DateTime DateStop)
        {
            BladeForm form;

            if (byteForm == 0)
                form = BladeForm.Outside;
            else
                form = BladeForm.Inside;

            return DAL.DAL.SPC_Thickness(profileId, form, intX, intY, SampleSize, DateStart, DateStop);
        }

        public SPC_Data Create_DeltaSPCReport(int profileId, byte byteForm, float intY, string SampleSize, DateTime DateStart, DateTime DateStop)
        {

            BladeForm form;

            if (byteForm == 0)
                form = BladeForm.Outside;
            else
                form = BladeForm.Inside;

            return DAL.DAL.SPC_Delta(profileId, form, intY, SampleSize, DateStart, DateStop);
        }

        private float Zusl(pointData pd)
        {
            return (pd.MaxT - pd.Average) / pd.STDev;
        }

        private float Zlsl(pointData pd)
        {
            return (pd.Average - pd.MinT) / pd.STDev;
        }

        private float Zmin(pointData pd)
        {
            return Math.Min(Zlsl(pd), Zusl(pd));
        }

        private float Pp(pointData pd)
        {
            return (pd.MaxT - pd.MinT) / (6 * pd.STDev);
        }

        private float Ppk(pointData pd)
        {
            return Zmin(pd) / 3;
        }

    }
}
