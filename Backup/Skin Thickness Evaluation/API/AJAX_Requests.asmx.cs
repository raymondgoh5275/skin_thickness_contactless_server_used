using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;
using System.Xml;
using BLL;
using DAL;
using ATS_Global.XML;
using Skin_Thickness_Evaluation.SITServiceExt;

namespace Skin_Thickness_Evaluation.API
{
    /// <summary>
    /// Summary description for AJAX_Requests
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [ScriptService]
    public class AJAX_Requests : System.Web.Services.WebService
    {
        public AJAX_Requests()
        {

        }

        [WebMethod]
        public string TestSQL(string guid)
        {
            string result;
            try
            {
                if (guid != "{DBB2D1FA-8E6E-4F83-A1CB-1819F29D0E65}")
                    return "";

                DAL.DAL testDAL = new DAL.DAL();

                result = testDAL.TestConnection();

                if (result != "Working.")
                    result = string.Format("Result = {0}<br />Conn = {1}", result, DAL.DAL.strConn);

            }
            catch (Exception Ex)
            {
                result = (DateTime.Now).ToString();
                result += string.Format("{1}Conn:-{0}{1}", DAL.DAL.strConn, Environment.NewLine);
                result += string.Format("Message:-{0}{1}", Ex.Message.ToString(), Environment.NewLine);
                result += string.Format("Data:-{0}{1}", Ex.Data.ToString(), Environment.NewLine);
                result += string.Format("Source:-{0}{1}", Ex.Source.ToString(), Environment.NewLine);
                result += string.Format("Stack Trace:-{0}{1}", Environment.NewLine, Ex.StackTrace.ToString());
            }

            return result;
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string Set_Resource_BladeSet(string BladeList)
        {
            if (Session["BladeList"] == null)
                Session.Add("BladeList", "");

            if (!string.IsNullOrEmpty(BladeList))
            {
                Session["BladeList"] = BladeList;
            }
            else
            {
                Session["BladeList"] = "";
            }
            return "";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string Select_Resource_BladeSet(string resourceName, string dateStart, string dateEnd)
        {
            string[][] ReturnValue = new string[1][];

            if ((!string.IsNullOrEmpty(resourceName)) && (!string.IsNullOrEmpty(dateStart)) && (!string.IsNullOrEmpty(dateEnd)))
            {
                string DataInput = Build_Siemens_Request(resourceName, dateStart, dateEnd);
                int errorCode = 0;
                string errorDescription = string.Empty;
                string dataOutput = string.Empty;
                DataSet dataRows = new DataSet();

                if (!string.IsNullOrEmpty(DataInput))
                {
                    SITServiceExtSoapClient SITService = new SITServiceExtSoapClient();
                    SITService.ServiceCaller(DataInput, ref errorCode, ref errorDescription, ref dataOutput, ref dataRows);

                    int i = 0;
                    ReturnValue = new string[dataRows.Tables[0].Rows.Count][];
                    foreach (DataRow dr in dataRows.Tables[0].Rows)
                    {
                        if (dr.ItemArray.Length > 0)
                            ReturnValue[i++] = new string[] { i.ToString(), dr["SerialNo"].ToString().Trim() };
                    }
                }
                else
                {
                    ReturnValue = new string[1][];
                    ReturnValue[0] = new string[] { "ERROR", "Error building input message" };
                }
            }
            else
            {
                ReturnValue = new string[1][];
                ReturnValue[0] = new string[] { "ERROR", "Missing parameter" };
            }

            // Return JSON data
            JavaScriptSerializer js = new JavaScriptSerializer();
            string strJSON = js.Serialize(ReturnValue);
            return strJSON;
        }

        private string Build_Siemens_Request(string resourceName, string dateStart, string dateEnd)
        {
            XmlDocument ReturnDoc = new XmlDocument();
            DateTime dtStart, dtStop;

            if (!DateTime.TryParse(dateStart, out dtStart))
                return "";

            if (!DateTime.TryParse(dateEnd, out dtStop))
                return "";

            XmlElement Root = XmlHelper.AddRootNode(ReturnDoc, "DataManager");
            XmlHelper.AddAttrib(Root, "MethodName", "MNG-OEM-GetSerialNo_By_Resource_DateRange");
            XmlHelper.AddAttrib(Root, "MethodType", "");
            XmlHelper.AddAttrib(Root, "MethodCallType", "");
            XmlHelper.AddAttrib(Root, "Language", "");
            XmlHelper.AddAttrib(Root, "User", "");

            XmlElement dataList = XmlHelper.AddNode(Root, "DataList");

            XmlElement data = XmlHelper.AddNode(dataList, "Data");
            XmlHelper.AddAttrib(data, "Resource", resourceName);
            XmlHelper.AddAttrib(data, "DateStart", dtStart.ToString("yyyy-MM-ddT00:00:00"));
            XmlHelper.AddAttrib(data, "DateEnd", dtStop.ToString("yyyy-MM-ddT23:59:59"));

            StringWriter sw = new StringWriter();
            XmlTextWriter tx = new XmlTextWriter(sw);
            ReturnDoc.WriteTo(tx);

            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            return sw.ToString();
        }

        [WebMethod(EnableSession = true)]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string Select_Blade_List(string profile)
        {
            if (Session["BladeList"] == null)
                Session["BladeList"] = "";

            int profileId;
            Dictionary<string, string> Data = new Dictionary<string, string>();
            string[][] ReturnValue;

            if (int.TryParse(profile, out profileId))
            {
                BizLogic Biz_Logic = new BizLogic();

                if (string.IsNullOrEmpty(Session["BladeList"].ToString()))
                    Data = Biz_Logic.GetBladeList_From_ProfileID(profileId);
                else
                    Data = Biz_Logic.GetBladeList_From_ProfileID(profileId, Session["BladeList"].ToString());

                ReturnValue = new string[Data.Count][];
                int i = 0;
                foreach (KeyValuePair<string, string> item in Data)
                {
                    ReturnValue[i++] = new string[] { item.Key, item.Value };
                }
            }
            else
            {
                ReturnValue = new string[1][];
                ReturnValue[0] = new string[] { "ERROR", "Profile ID must be an Integer" };
            }

            // Return JSON data
            JavaScriptSerializer js = new JavaScriptSerializer();
            string strJSON = js.Serialize(ReturnValue);
            return strJSON;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string Select_Row_List(string profile, string Form)
        {
            int profileId;
            List<string> Data = new List<string>();
            string[] ReturnValue;

            if (int.TryParse(profile, out profileId))
            {
                byte byteForm;
                byte.TryParse(Form, out byteForm);

                BizLogic Biz_Logic = new BizLogic();
                Data = Biz_Logic.GetRowList_From_ProfileID(profileId, byteForm);

                ReturnValue = Data.ToArray();

            }
            else
            {
                ReturnValue = new string[1];
                ReturnValue[0] = "Profile ID must be an Integer";
            }

            // Return JSON data
            JavaScriptSerializer js = new JavaScriptSerializer();
            string strJSON = js.Serialize(ReturnValue);
            return strJSON;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public string Select_Col_List(string profile, string Form, string Row)
        {
            int profileId;
            List<string> Data = new List<string>();
            string[] ReturnValue;

            if (int.TryParse(profile, out profileId))
            {
                byte byteForm;
                byte.TryParse(Form, out byteForm);

                int intRow;
                int.TryParse(Row, out intRow);

                BizLogic Biz_Logic = new BizLogic();
                Data = Biz_Logic.GetColList_From_ProfileID(profileId, byteForm, intRow);

                ReturnValue = Data.ToArray();
            }
            else
            {
                ReturnValue = new string[1];
                ReturnValue[0] = "Profile ID must be an Integer";
            }

            // Return JSON data
            JavaScriptSerializer js = new JavaScriptSerializer();
            string strJSON = js.Serialize(ReturnValue);
            return strJSON;
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
        public string Export_Analysis_Filtering(string BladeID, bool Pivot)
        {
            int bladeID;
            ScanStorage Data;
            string[][] ReturnValue;

            int maxThicknessCount = 0;
            if (int.TryParse(BladeID, out bladeID))
            {
                try
                {
                    Data = ScanStorage.Load(bladeID);

                    ReturnValue = new string[Data.InsideForm.Length + Data.OutsideForm.Length][];

                    for (int i = 0; i < Data.OutsideForm.Length; i++)
                    {
                        ReturnValue[i] = new string[Data.OutsideForm[i].Points.Length + 2];
                        ReturnValue[i][0] = "O";
                        ReturnValue[i][1] = ((int)Data.OutsideForm[i].Y).ToString();
                        if (maxThicknessCount < Data.OutsideForm[i].Points.Length) maxThicknessCount = Data.OutsideForm[i].Points.Length;
                        for (int l = 0; l < Data.OutsideForm[i].Points.Length; l++)
                        {
                            ReturnValue[i][2 + l] = Data.OutsideForm[i].Points[l].Thickness.ToString("0.00");
                        }
                    }
                    for (int i = 0; i < Data.InsideForm.Length; i++)
                    {
                        ReturnValue[Data.OutsideForm.Length + i] = new string[Data.InsideForm[i].Points.Length + 2];
                        ReturnValue[Data.OutsideForm.Length + i][0] = "I";
                        ReturnValue[Data.OutsideForm.Length + i][1] = ((int)Data.InsideForm[i].Y).ToString();
                        if (maxThicknessCount < Data.InsideForm[i].Points.Length) maxThicknessCount = Data.InsideForm[i].Points.Length;
                        for (int l = 0; l < Data.InsideForm[i].Points.Length; l++)
                        {
                            ReturnValue[Data.OutsideForm.Length + i][2 + l] = Data.InsideForm[i].Points[l].Thickness.ToString("0.00");
                        }
                    }
                }
                catch (Exception Ex)
                {
                    return string.Format("<div class=\"warning\" style=\"height:150px;margin:auto;vertical-align:middle;\"><strong>ERROR</strong> - {0}.</div>", Ex.Message);

                }
            }
            else
            {
                return string.Format("<div class=\"warning\" style=\"height:150px;margin:auto;vertical-align:middle;\"><strong>ERROR</strong> - Blade ID must be an Integer ({0}).</div>", BladeID);
            }

            StringBuilder strHTML = new StringBuilder();

            strHTML.Append("<table id=\"thicknessData\" class=\"ui-widget ui-widget-content ui-corner-all\" style=\"width: 98%;\">");

            if (Pivot)
            {
                bool highlight = false;

                for (int inner = 0; inner < maxThicknessCount; inner++)
                {
                    if (highlight)
                        strHTML.Append("<tr>");
                    else
                        strHTML.Append("<tr class=\"ui-state-default\">");

                    if (inner < 3)
                        switch (inner)
                        {
                            case 0:
                                strHTML.Append("<th class=\"ui-widget-header\">Form</th>");
                                break;
                            case 1:
                                strHTML.Append("<th class=\"ui-widget-header\">Y</th>");
                                break;
                            case 2:
                                strHTML.AppendFormat("<th class=\"ui-widget-header\" rowspan=\"{0}\">Thickness</th>", maxThicknessCount);
                                break;
                            default:
                                break;
                        }

                    for (int outer = 0; outer < ReturnValue.Length; outer++)
                    {
                        if (inner >= ReturnValue[outer].Length)
                            strHTML.Append("<td>&nbsp;</td>");
                        else
                            strHTML.AppendFormat("<td>{0}</td>", ReturnValue[outer][inner]);
                    }
                    strHTML.Append("</tr>");
                    highlight = !highlight;
                }
            }
            else
            {
                strHTML.AppendFormat("<thead id=\"ExportHeader\"><tr class=\"ui-widget-header\"><td>Form</td><td>Y</td><td colspan=\"{0}\">Thickness</td></tr></thead>", maxThicknessCount);
                strHTML.Append("<tbody id=\"ExportOutput\">");
                bool highlight = false;
                for (int outer = 0; outer < ReturnValue.Length; outer++)
                {
                    if (highlight)
                        strHTML.Append("<tr>");
                    else
                        strHTML.Append("<tr class=\"ui-state-default\">");

                    for (int inner = 0; inner < ReturnValue[outer].Length; inner++)
                    {
                        strHTML.AppendFormat("<td>{0}</td>", ReturnValue[outer][inner]);
                    }
                    strHTML.Append("</tr>");
                    highlight = !highlight;
                }
                strHTML.Append("</tbody>");
            }
            strHTML.Append("</table>");

            return strHTML.ToString();
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        //        [ScriptMethod(ResponseFormat = ResponseFormat.Xml)]
        public string Create_DeltaReport(
            string Profile,
            string Row,
            string SampleSize,
            string DateStart,
            string DateStop)
        {
            BizLogic BusinessLogic = new BizLogic();

            int intProfile;
            intProfile = int.Parse(Profile);

            Profile tmpPro = new Profile(intProfile);
            string strTitle = tmpPro.Name;

            float floatRow;
            floatRow = float.Parse(Row);

            string strSampleSize = SampleSize;

            DateTime dtDateStart;
            if (string.IsNullOrEmpty(DateStart))
            {
                dtDateStart = DateTime.Parse("1/Feb/1753 12:00:00 AM");
            }
            else
            {
                dtDateStart = DateTime.Parse(DateStart);
            }

            DateTime dtDateStop;

            if (string.IsNullOrEmpty(DateStop))
            {
                dtDateStop = DateTime.Now;
            }
            else
            {
                dtDateStop = DateTime.Parse(DateStop);
            }

            SPC_Data InsideData = BusinessLogic.Create_DeltaSPCReport(intProfile, 0, floatRow, strSampleSize, dtDateStart, dtDateStop);
            SPC_Data OutsideData = BusinessLogic.Create_DeltaSPCReport(intProfile, 180, floatRow, strSampleSize, dtDateStart, dtDateStop);

            Dictionary<string, double[]> ReturnValue = new Dictionary<string, double[]>();

            StringBuilder strHTML = new StringBuilder();


            string[] arrBlade = InsideData.Blade;
            double[] arrIndata = InsideData.Value;
            double[] arrOutdata = OutsideData.Value;

            strHTML.Append("<table id=\"DeltaData\" class=\"ui-widget ui-widget-content ui-corner-all\">");
            string[,] datalines = new string[arrBlade.Length, 3];

            bool highlight;
            for (int i = 0; i < arrBlade.Length; i++)
            {
                datalines[i, 0] = arrBlade[i];
                datalines[i, 1] = arrIndata[i].ToString();
                datalines[i, 2] = arrOutdata[i].ToString();
            }

            for (int line = 0; line < 3; line++)
            {
                switch (line)
                {
                    case 0:
                        strHTML.Append("<tr><th>Blade ID</th>");
                        break;
                    case 1:
                        strHTML.Append("<tr><th>Row Delta Inside Form</th>");
                        break;
                    case 2:
                        strHTML.Append("<tr><th>Row Delta Outside Form</th>");
                        break;
                }

                highlight = true;
                for (int i = 0; i < arrBlade.Length; i++)
                {
                    if (highlight)
                        strHTML.Append("<td>");
                    else
                        strHTML.Append("<td class=\"ui-state-default\">");

                    strHTML.AppendFormat("{0}</td>", datalines[i, line]);

                    highlight = !highlight;
                }
                strHTML.Append("</tr>");
            }
            strHTML.Append("</table>");

            return strHTML.ToString();
        }
    }
}
