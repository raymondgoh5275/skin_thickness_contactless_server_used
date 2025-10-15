using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ATS_Global.XML;
using DAL;

using BLL.Properties;
using System.Linq;
//using ATS_Global;
using System.Globalization;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace BLL
{
    public static class StringExt
    {
        //public static bool IsNumeric(this string text) => double.TryParse(text, out _);

        public static bool IsNumeric(this string text)
        {
            double result;
            return double.TryParse(text, out result);
        }

    }
    public class ScanStorage : Scan
    {
        public ScanStorage()
        {

        }

        public ScanStorage(Scan ScanData)
        {
            this.id = ScanData.id;
            this.FilePath = ScanData.FilePath;
            this.SerialNo = ScanData.SerialNo;
            this.Batch = ScanData.Batch;
            this.Created = ScanData.Created;
            this.Resource = ScanData.Resource;
            this.TankNumber = ScanData.TankNumber;
            this.Inspector = ScanData.Inspector;
            this.Result = ScanData.Result;

            this.DeleteDate = ScanData.DeleteDate;

            this.InsideForm = ScanData.InsideForm;
            this.OutsideForm = ScanData.OutsideForm;

            this.ProfileInfo = ScanData.ProfileInfo;
        }

        private string RemapPath(string filePath)
        {
            return Path.Combine(Settings.Default.CSVPath, Path.GetFileName(filePath));
        }

        public bool Load(string fileName)
        {
            string DatFileName = string.Empty;
            string ProFileName = string.Empty;
            string CsvFileName = string.Empty;


            if (!File.Exists(fileName))
                return false;


            XmlDocument OEM2MESxml = new XmlDocument();
            OEM2MESxml.Load(fileName);

            XmlElement MessageInfo = (XmlElement)(OEM2MESxml.SelectSingleNode("//MessageInfo"));
            XmlNodeList FilesList = OEM2MESxml.SelectNodes("//MessageDetail[@Type='Files']/MessageDetailItem");

            this.SerialNo = XmlHelper.ReadAttrib(MessageInfo, "SerialNo");
            this.Batch = XmlHelper.ReadAttrib(MessageInfo, "Batch");
            this.Created = DateTime.Parse(XmlHelper.ReadAttrib(MessageInfo, "DateStart"));
            this.Inspector = XmlHelper.ReadAttrib(MessageInfo, "User");
            this.Resource = XmlHelper.ReadAttrib(MessageInfo, "Resource");
            var csvFileNames = new List<string>();
            foreach (XmlNode fileNode in FilesList)
            {
                var nodeFilename = Path.GetFileName(XmlHelper.ReadAttrib((XmlElement)fileNode, "Name"));
                switch (Path.GetExtension(nodeFilename).ToLowerInvariant())
                {
                    case ".dat":
                        DatFileName = nodeFilename;
                        break;
                    case ".pro":
                        ProFileName = nodeFilename;
                        break;
                    case ".csv":
                        csvFileNames.Add(nodeFilename);
                        break;
                    default:
                        break;
                }
            }

            if(!string.IsNullOrWhiteSpace(DatFileName) && !string.IsNullOrWhiteSpace(ProFileName))
            {
                ProDatScan(DatFileName, ProFileName);
            }
            else if (csvFileNames.Count > 0)
            {
                IsCsv = true;
                CSVScan(csvFileNames);
            }
            else
            {
                throw new ArgumentException("Unexpected XML file Arguments");
            }

            return true;
        }



        public void CSVScan(List<string> csvFileNames)
        {


            var sc = new Contacless.CsvScan
                 (
                    Settings.Default.CSVPath,
                    Settings.Default.OEMInputPath,
                    DAL.Properties.Settings.Default.DB_Connection,
                    Batch,
                    SerialNo,
                    Resource,
                    Inspector,
                    Created
                );
            sc.Parse(csvFileNames);

            //delete csv files
            foreach (var fname in csvFileNames)
            {
                try
                {
                    File.Delete(Path.Combine(Settings.Default.OEMInputPath, fname));
                }
                catch (Exception)
                {
                }
            }
        }

        private void ProDatScan(string DatFileName, string ProFileName)
        {
            this.FilePath = Path.Combine(Settings.Default.OEMInputPath, DatFileName);
            int[] numArray1 = new int[2];
            int[] numArray2 = new int[2];
            int[] numArray3 = new int[2];
            int[,] numArray4 = new int[2, 100];
            int[] numArray5 = new int[2];
            int[] numArray6 = new int[2];
            double[,,] numArray7 = new double[2, 100, 3000];
            int[,] numArray8 = new int[2, 100];
            byte[] DataIn = File.ReadAllBytes(this.FilePath);
            string str = File.ReadAllText(this.FilePath);
            int num1 = str.IndexOf("End of CSV section");
            int startIndex1 = 0;
            int length = str.IndexOf("\r\n");
            Dictionary<int, Dictionary<string, Dictionary<string, string>>> dictionary1 = new Dictionary<int, Dictionary<string, Dictionary<string, string>>>();
            string[] strArray1 = str.Substring(startIndex1, length).Split(',');
            int startIndex2 = length + 2;
            for (int index1 = str.IndexOf("\r\n", startIndex2); index1 < num1; index1 = str.IndexOf("\r\n", startIndex2))
            {
                string[] strArray2 = str.Substring(startIndex2, index1 - startIndex2).Split(',');
                int key = strArray2[0] == "O/F" ? 0 : 1;
                if (!dictionary1.ContainsKey(key))
                    dictionary1.Add(key, new Dictionary<string, Dictionary<string, string>>());
                int num2 = int.Parse(strArray2[4]);
                if (!dictionary1[key].ContainsKey(double.Parse(strArray2[1]).ToString("f1")))
                {
                    Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
                    for (int index2 = 0; index2 < num2; ++index2)
                    {
                        int index3 = 5 + index2 * 2;
                        float num3 = float.Parse(strArray2[index3]);
                        dictionary2.Add(num3.ToString("f2"), strArray2[index3 + 1]);
                    }
                    dictionary1[key].Add(double.Parse(strArray2[1]).ToString("f1"), dictionary2);
                }
                startIndex2 = index1 + 2;
            }
            this.TankNumber = int.Parse(strArray1[11]);
            if (strArray1[27].ToUpper() == "FAIL")
                this.Result = false;
            else
                this.Result = true;
            this.DeleteDate = new DateTime?();
            ProfileStorage profileStorage = new ProfileStorage();
            profileStorage.Name = Path.GetFileNameWithoutExtension(strArray1[4]);
            profileStorage.Pro_Filename = Path.Combine(Settings.Default.OEMInputPath, ProFileName);
            profileStorage.Omax_Filename = this.RemapPath(strArray1[5]);
            profileStorage.Omin_Filename = this.RemapPath(strArray1[6]);
            profileStorage.Onom_Filename = this.RemapPath(strArray1[7]);
            profileStorage.Imax_Filename = this.RemapPath(strArray1[8]);
            profileStorage.Imin_Filename = this.RemapPath(strArray1[9]);
            profileStorage.Inom_Filename = this.RemapPath(strArray1[10]);
            if (!profileStorage.Exists())
            {
                profileStorage.BuildProfile();
                profileStorage.Save();
            }
            this.ProfileInfo = (Profile)profileStorage;
            int DataPtr = num1 + 20;
            numArray1[0] = this.GetNext4BytesAsInteger(DataIn, ref DataPtr);
            numArray1[1] = this.GetNext4BytesAsInteger(DataIn, ref DataPtr);
            numArray3[0] = this.GetNext4BytesAsInteger(DataIn, ref DataPtr);
            numArray3[1] = this.GetNext4BytesAsInteger(DataIn, ref DataPtr);
            numArray5[0] = this.GetNext4BytesAsInteger(DataIn, ref DataPtr);
            numArray5[1] = this.GetNext4BytesAsInteger(DataIn, ref DataPtr);
            numArray6[0] = this.GetNext4BytesAsInteger(DataIn, ref DataPtr);
            numArray6[1] = this.GetNext4BytesAsInteger(DataIn, ref DataPtr);
            int next4BytesAsInteger = this.GetNext4BytesAsInteger(DataIn, ref DataPtr);
            this.GetNext4BytesAsInteger(DataIn, ref DataPtr);
            DataPtr = numArray1[0];
            numArray2[0] = this.GetNext4BytesAsInteger(DataIn, ref DataPtr);
            DataPtr = numArray1[1];
            numArray2[1] = this.GetNext4BytesAsInteger(DataIn, ref DataPtr);
            DataPtr = numArray3[0];
            for (int index = 0; index < numArray2[0]; ++index)
                numArray4[0, index] = this.GetNext4BytesAsInteger(DataIn, ref DataPtr);
            DataPtr = numArray3[1];
            for (int index = 0; index < numArray2[1]; ++index)
                numArray4[1, index] = this.GetNext4BytesAsInteger(DataIn, ref DataPtr);
            for (int index4 = 0; index4 < 2; ++index4)
            {
                DataPtr = numArray5[index4];
                for (int index5 = 0; index5 < numArray2[index4]; ++index5)
                {
                    for (int index6 = 0; index6 < numArray4[index4, index5]; ++index6)
                    {
                        if (index4 == 0)
                            numArray7[index4, index5, index6] = this.GetNext8BytesAsDouble(DataIn, ref DataPtr);
                        else
                            numArray7[index4, index5, numArray4[index4, index5] - 1 - index6] = this.GetNext8BytesAsDouble(DataIn, ref DataPtr);
                    }
                }
            }
            DataPtr = next4BytesAsInteger;
            for (int index7 = 0; index7 < 2; ++index7)
            {
                for (int index8 = 0; index8 < 100; ++index8)
                    numArray8[index7, index8] = this.GetNext4BytesAsInteger(DataIn, ref DataPtr);
            }
            for (int key = 0; key < 2; ++key)
            {
                List<ScanRow> scanRowList = new List<ScanRow>();
                for (int index9 = 0; index9 < numArray2[key]; ++index9)
                {
                    ScanRow scanRow = new ScanRow();
                    int num4 = 0;
                    scanRow.Form = key == 0 ? BladeForm.Outside : BladeForm.Inside;
                    scanRow.DeltaX = (float)numArray8[key, index9];
                    scanRow.Y = key == 0 ? this.ProfileInfo.OutsideForm[index9].Y : this.ProfileInfo.InsideForm[index9].Y;
                    List<ScanPoint> scanPointList = new List<ScanPoint>();
                    float num5 = key != 0 ? (float)(0.5 * -((double)scanRow.DeltaX + 1.0)) + this.ProfileInfo.InsideForm[index9].ScanStart : (float)(0.5 * -((double)scanRow.DeltaX - 1.0)) + this.ProfileInfo.OutsideForm[index9].ScanStart;
                    for (int index10 = 0; index10 < numArray4[key, index9]; ++index10)
                    {
                        ScanPoint scanPoint = new ScanPoint();
                        scanPoint.Thickness = (float)numArray7[key, index9, index10];
                        scanPoint.X = num5;
                        scanPoint.Y = scanRow.Y;
                        if (dictionary1[key][scanRow.Y.ToString("f1")].ContainsKey(string.Format("{0:f2}", (object)num5)))
                            scanPoint.TrafficLight = new int?(num4++);
                        if (key == 0)
                            num5 += 0.5f;
                        else
                            num5 -= 0.5f;
                        scanPointList.Add(scanPoint);
                    }
                    scanRow.Points = scanPointList.ToArray();
                    scanRowList.Add(scanRow);
                }
                if (key == 0)
                    this.OutsideForm = scanRowList.ToArray();
                else
                    this.InsideForm = scanRowList.ToArray();
            }
            try
            {
                File.Delete(Path.Combine(Settings.Default.OEMInputPath, DatFileName));
                File.Delete(Path.Combine(Settings.Default.OEMInputPath, ProFileName));
            }
            catch (Exception)
            {
            }

        }

        public static ScanStorage Load(int BladeID)
        {
            return new ScanStorage(new Scan(BladeID));
        }

        public static ScanStorage LoadForm(int BladeID, BladeForm form)
        {
            return new ScanStorage(new Scan(BladeID, form));
        }

        private double GetNext8BytesAsDouble(byte[] DataIn, ref int DataPtr)
        {
            DataPtr += 8;
            return BitConverter.ToDouble(DataIn, DataPtr - 8);
        }

        private Int32 GetNext4BytesAsInteger(byte[] DataIn, ref int DataPtr)
        {
            DataPtr += 4;
            return BitConverter.ToInt32(DataIn, DataPtr - 4);
        }
    }


}

