using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using BLL;
using DAL;

namespace Skin_Thickness_Evaluation.API
{
    public partial class CSV : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string strMethod = Request.QueryString["M"].ToString();

            switch (strMethod)
            {
                case "EAF" :
                    string BladeID = Request.QueryString["id"].ToString();
                    bool Pivot = bool.Parse(Request.QueryString["p"].ToString());

                    strError.InnerHtml =  Export_Analysis_Filtering_CSV(BladeID, Pivot);

                    break;
                case "CRAF":
                    int intProfile = int.Parse(Request.QueryString["id"].ToString());
                    
                    Profile tmpPro = new Profile(intProfile);
                    string strTitle = tmpPro.Name;
                    
                    float floatRow = float.Parse(Request.QueryString["Row"].ToString());
                    
                    string strSampleSize = Request.QueryString["SampleSize"].ToString();

                    DateTime dtDateStart;
                    if (string.IsNullOrEmpty(Request.QueryString["DateStart"].ToString()))
                    {
                        dtDateStart = DateTime.Parse("1/Feb/1753 12:00:00 AM");
                    }
                    else
                    {
                        dtDateStart = DateTime.Parse(Request.QueryString["DateStart"].ToString());
                    }

                    DateTime dtDateStop;
                    if (string.IsNullOrEmpty(Request.QueryString["DateStop"].ToString()))
                    {
                        dtDateStop = DateTime.Now;
                    }
                    else
                    {
                        dtDateStop = DateTime.Parse(Request.QueryString["DateStop"].ToString());
                    }


                    strError.InnerHtml = Cavity_Reports_Analysis_Filtering_CSV(intProfile, floatRow, strSampleSize, dtDateStart, dtDateStop);

                    break;
                default:

                    break;
            }
        }

        protected string Export_Analysis_Filtering_CSV(string BladeID, bool Pivot)
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
                            ReturnValue[i][2 + l] = Data.OutsideForm[i].Points[l].Thickness.ToString();
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
                            ReturnValue[Data.OutsideForm.Length + i][2 + l] = Data.InsideForm[i].Points[l].Thickness.ToString();
                        }
                    }
                }
                catch (Exception Ex)
                {
                    return string.Format("<ul><li><b>Data</b><br />{0}</li><li><b>HelpLink</b><br />{1}</li><li><b>TargetSite</b><br />{2}</li><li><b>Source</b><br />{3}</li><li><b>StackTrace</b><br />{4}</li><li><b>Message</b><br />{5}</li><li><b>InnerException</b><br />{6}</li></ul>", 
                        Ex.Data,
                        Ex.HelpLink,
                        Ex.TargetSite,
                        Ex.Source,
                        Ex.StackTrace,
                        Ex.Message,
                        Ex.InnerException);

                }
            }
            else
            {
                return string.Format("<div class=\"warning\" style=\"height:150px;margin:auto;vertical-align:middle;\"><strong>ERROR</strong> - Blade ID must be an Integer ({0}).</div>", BladeID);
            }

            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);

            if (Pivot)
            {
                for (int inner = 0; inner < maxThicknessCount; inner++)
                {
                    switch (inner)
                    {
                        case 0:
                            sw.Write("Form");
                            break;
                        case 1:
                            sw.Write("Y");
                            break;
                        case 2:
                            sw.Write("Thickness");
                            break;
                        default:
                            sw.Write(" ");
                            break;
                    }

                    for (int outer = 0; outer < ReturnValue.Length; outer++)
                    {
                        if (inner >= ReturnValue[outer].Length)
                            sw.Write(", ");
                        else
                            sw.Write(string.Format(",{0}", ReturnValue[outer][inner]));
                    }
                    sw.WriteLine("");
                }
            }
            else
            {
                sw.WriteLine("Form,Y,Thickness");
                for (int outer = 0; outer < ReturnValue.Length; outer++)
                {
                    for (int inner = 0; inner < ReturnValue[outer].Length; inner++)
                    {
                        if (inner > 0)
                            sw.Write(",");

                        sw.Write(string.Format("{0}", ReturnValue[outer][inner]));
                    }
                    sw.WriteLine("");
                }
            }

            sw.Flush();

            HttpResponse Response = this.Context.Response;
            Response.ContentType = "text/plain";
            Response.AppendHeader("Content-Disposition", "attachment; filename=Export_Analysis.csv");

            ms.WriteTo(Response.OutputStream);

            Response.End();
            return "OK";
        }

        private string Cavity_Reports_Analysis_Filtering_CSV(int intProfile, float floatRow, string strSampleSize, DateTime dtDateStart, DateTime dtDateStop)
        {
            BizLogic BusinessLogic = new BizLogic();

            SPC_Data InsideData, OutsideData;

            try
            {
                InsideData = BusinessLogic.Create_DeltaSPCReport(intProfile, 0, floatRow, strSampleSize, dtDateStart, dtDateStop);
                OutsideData = BusinessLogic.Create_DeltaSPCReport(intProfile, 180, floatRow, strSampleSize, dtDateStart, dtDateStop);
            }
            catch (Exception Ex)
            {
                return string.Format("<ul><li><b>Data</b><br />{0}</li><li><b>HelpLink</b><br />{1}</li><li><b>TargetSite</b><br />{2}</li><li><b>Source</b><br />{3}</li><li><b>StackTrace</b><br />{4}</li><li><b>Message</b><br />{5}</li><li><b>InnerException</b><br />{6}</li></ul>",
                    Ex.Data,
                    Ex.HelpLink,
                    Ex.TargetSite,
                    Ex.Source,
                    Ex.StackTrace,
                    Ex.Message,
                    Ex.InnerException);

            }

            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);

            Dictionary<string, double[]> ReturnValue = new Dictionary<string, double[]>();

            string[] arrBlade = InsideData.Blade;
            double[] arrIndata = InsideData.Value;
            double[] arrOutdata = OutsideData.Value;

            string[,] datalines = new string[arrBlade.Length, 3];

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
                        sw.Write("Blade ID");
                        break;
                    case 1:
                        sw.Write("Row Delta Inside Form");
                        break;
                    case 2:
                        sw.Write("Row Delta Outside Form");
                        break;
                }

                for (int i = 0; i < arrBlade.Length; i++)
                {
                    sw.Write(string.Format(", {0}", datalines[i, line]));
                }
                sw.WriteLine("");
            }

            sw.Flush();

            HttpResponse Response = this.Context.Response;
            Response.ContentType = "text/plain";
            Response.AppendHeader("Content-Disposition", "attachment; filename=Cavity_Reports_Analysis.csv");

            ms.WriteTo(Response.OutputStream);

            Response.End();
            return "OK";
        }
    }
}