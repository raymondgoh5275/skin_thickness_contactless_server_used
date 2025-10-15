namespace Skin_Thickness_Evaluation.API
{
    using System;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Web;
    using System.Web.UI.DataVisualization.Charting;
    using System.Web.UI.WebControls;
    using BLL;
    using DAL;

    /// <summary>
    /// Summary description for ImageHandler
    /// </summary>
    public class ImageHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            byte[] returnBuffer = new byte[0];

            switch (context.Request.QueryString["I"])
            {
                case "ContourBladeImage":
                    returnBuffer = ContourBladeImage(
                        context.Request.QueryString["BladeIDs"],
                        context.Request.QueryString["Form"],
                        context.Request.QueryString["Calculate"],
                        context.Request.QueryString["CompareTo"],
                        context.Request.QueryString["Scale"],
                        context.Request.QueryString["Offset"]);
                    break;
                case "ReworkAnalysisBladeImage":
                    returnBuffer = ReworkAnalysisBladeImage(
                        context.Request.QueryString["BladeID"],
                        context.Request.QueryString["Form"],
                        context.Request.QueryString["Scale"],
                        context.Request.QueryString["Offset"]);
                    break;
                case "MetalRemovalBladeImage":
                    returnBuffer = MetalRemovalBladeImage(
                        context.Request.QueryString["ScanAid"],
                        context.Request.QueryString["ScanBid"],
                        context.Request.QueryString["Form"],
                        context.Request.QueryString["Scale"],
                        context.Request.QueryString["Offset"]);
                    break;
                case "Create_SPCReport":
                    returnBuffer = Create_SPCReport(
                        context.Request.QueryString["Profile"],
                        context.Request.QueryString["Form"],
                        context.Request.QueryString["Row"],
                        context.Request.QueryString["Col"],
                        context.Request.QueryString["SampleSize"],
                        context.Request.QueryString["DateStart"],
                        context.Request.QueryString["DateStop"]);
                    break;
                case "Create_DeltaSPCReport":
                    returnBuffer = Create_DeltaSPCReport(
                        context.Request.QueryString["Profile"],
                        context.Request.QueryString["Form"],
                        context.Request.QueryString["Row"],
                        context.Request.QueryString["SampleSize"],
                        context.Request.QueryString["DateStart"],
                        context.Request.QueryString["DateStop"]);
                    break;
            }

            context.Response.ContentType = "image/png";
            context.Response.BinaryWrite(returnBuffer);
            context.Response.Flush();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public byte[] ContourBladeImage(string BladeIDs, string Form, string Calculate, string CompareTo, string Scale, string Offset)
        {
            string[] tmp = BladeIDs.Split(',');
            int[] bladeids = new int[tmp.Length];

            for (int i = 0; i < tmp.Length; i++)
            {
                bladeids[i] = int.Parse(tmp[i]);
            }

            byte byteForm;
            byte.TryParse(Form, out byteForm);

            byte byteCalculate;
            byte.TryParse(Calculate, out byteCalculate);

            byte byteCompareTo;
            byte.TryParse(CompareTo, out byteCompareTo);

            float fScale;
            float.TryParse(Scale, out fScale);

            float fOffset;
            float.TryParse(Offset, out fOffset);

            BizLogic BusinessLogic = new BizLogic();

            return this.ImageEncode(this.ImageResize(BusinessLogic.ContourBladeImage(bladeids, byteForm, byteCalculate, byteCompareTo, fScale, fOffset), new Size(310, 600), Form));
        }

        public byte[] ReworkAnalysisBladeImage(string BladeID, string Form, string Scale, string Offset)
        {
            int bladeid = int.Parse(BladeID);

            byte byteForm;
            byte.TryParse(Form, out byteForm);

            float fScale;
            float.TryParse(Scale, out fScale);

            float fOffset;
            float.TryParse(Offset, out fOffset);

            BizLogic BusinessLogic = new BizLogic();

            return this.ImageEncode(this.ImageResize(BusinessLogic.ReworkAnalysisBladeImage(bladeid, byteForm, fScale, fOffset), new Size(310, 600), Form));
        }

        public byte[] MetalRemovalBladeImage(string ScanAid, string ScanBid, string Form, string Scale, string Offset)
        {
            int intBladeAid = int.Parse(ScanAid);
            int intBladeBid = int.Parse(ScanBid);

            byte byteForm;
            byte.TryParse(Form, out byteForm);

            float fScale;
            float.TryParse(Scale, out fScale);

            float fOffset;
            float.TryParse(Offset, out fOffset);

            BizLogic BusinessLogic = new BizLogic();

            return this.ImageEncode(this.ImageResize(BusinessLogic.MetalRemovalBladeImage(intBladeAid, intBladeBid, byteForm, fScale, fOffset), new Size(310, 600), Form));
        }

        public byte[] Create_SPCReport(
            string Profile,
            string Form,
            string Row,
            string Col,
            string SampleSize,
            string DateStart,
            string DateStop)
        {
            BizLogic BusinessLogic = new BizLogic();

            int intProfile;
            intProfile = int.Parse(Profile);

            Profile tmpPro = new Profile(intProfile);
            string strTitle = tmpPro.Name;

            byte byteForm;
            byteForm = byte.Parse(Form);

            float floatRow;
            floatRow = float.Parse(Row);

            float floatCol;
            floatCol = float.Parse(Col);

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

            return this.ImageEncode(BuildSpcChart(strTitle, BusinessLogic.Create_SPCReport(intProfile, byteForm, floatCol, floatRow, strSampleSize, dtDateStart, dtDateStop), new Size(925, 725)));
        }

        public byte[] Create_DeltaSPCReport(
            string Profile,
            string Form,
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

            byte byteForm;
            byteForm = byte.Parse(Form);

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

            return this.ImageEncode(BuildSpcChart(strTitle, BusinessLogic.Create_DeltaSPCReport(intProfile, byteForm, floatRow, strSampleSize, dtDateStart, dtDateStop), new Size(450, 450)));
        }

        private Bitmap ImageResize(Bitmap bitmap, Size size, string form)
        {
            int sourceWidth = bitmap.Width;
            int sourceHeight = bitmap.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((size.Width - (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((size.Height - (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(size.Width, size.Height);
            Graphics g = Graphics.FromImage((System.Drawing.Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBilinear;//.HighQualityBicubic;

            g.DrawImage(bitmap,
                        new Rectangle(destX, destY, destWidth, destHeight),
                        new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                        GraphicsUnit.Pixel
            );
            g.Dispose();

            //if (form == "180")
            //    b.RotateFlip(RotateFlipType.RotateNoneFlipX);

            return b;
        }

        private byte[] ImageEncode(Bitmap image)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png);

                stream.Flush();

                //byte[] imageData = 
                return stream.ToArray();
                //string imageBase64 = Convert.ToBase64String(imageData);
                //return string.Format("data:image/png;base64,{0}", imageBase64);
            }
        }

        protected Bitmap BuildSpcChart(string ChartTitle, SPC_Data ChartData, Size ImageSize)
        {
            Chart m_chart = new Chart();

            //Chart setting 
            m_chart.Width = Unit.Pixel(ImageSize.Width);
            m_chart.Height = Unit.Pixel(ImageSize.Height);
            m_chart.BorderlineColor = Color.Black;
            m_chart.BorderlineDashStyle = ChartDashStyle.Solid;
            m_chart.BorderlineWidth = 2;
            m_chart.BorderSkin.SkinStyle = BorderSkinStyle.Emboss;
            m_chart.ImageType = ChartImageType.Png;
            m_chart.Titles.Add(string.Format("STATISTICAL PROCESS CONTROL CHART FOR {0}", ChartTitle));
            m_chart.Titles.Add(string.Format("Cpk = {0}", ChartData.CPK));

            //Chart Area
            ChartArea mainArea = new ChartArea();
            mainArea.Name = "mainArea";
            mainArea.BackColor = Color.Silver;
            mainArea.BackGradientStyle = GradientStyle.None;
            mainArea.BorderDashStyle = ChartDashStyle.Solid;
            mainArea.AxisX.IsMarginVisible = false;
            mainArea.AxisX.LabelStyle.Angle = -45;
            mainArea.AxisX.LabelStyle.Interval = 1;
            mainArea.AxisY2.Maximum = 2.0f;
            mainArea.AxisX.MajorGrid.Enabled = false;
            mainArea.AxisY.MajorGrid.Enabled = false;
            mainArea.AxisY2.MajorGrid.Enabled = false;

            m_chart.ChartAreas.Add(mainArea);

            //Legend
            Legend mainLegend = new Legend();
            mainLegend.Name = "mainLegend";
            mainLegend.DockedToChartArea = "mainArea";
            mainLegend.Docking = Docking.Bottom;
            mainLegend.HeaderSeparator = LegendSeparatorStyle.Line;
            mainLegend.IsDockedInsideChartArea = false;
            m_chart.Legends.Add(mainLegend);

            //Chart Data
            Series seriesValue = new Series();			// Thickness
            seriesValue.ChartArea = "mainArea";
            seriesValue.Legend = "mainLegend";
            seriesValue.ChartType = SeriesChartType.Line;
            seriesValue.BorderWidth = 2;
            seriesValue.Name = "VALUE";
            seriesValue.Color = Color.Black;
            seriesValue.MarkerStyle = MarkerStyle.Diamond;

            Series seriesAvgValue = new Series();		// Avg Thickness
            seriesAvgValue.ChartArea = "mainArea";
            seriesAvgValue.Legend = "mainLegend";
            seriesAvgValue.ChartType = SeriesChartType.Line;
            seriesAvgValue.Name = "AVG VALUE";
            seriesAvgValue.BorderDashStyle = ChartDashStyle.Dash;
            seriesAvgValue.BorderWidth = 2;
            seriesAvgValue.Color = Color.Black;

            Series seriesMax = new Series();			// Max Tolerance
            seriesMax.ChartArea = "mainArea";
            seriesMax.Legend = "mainLegend";
            seriesMax.ChartType = SeriesChartType.Line;
            seriesMax.BorderWidth = 2;
            seriesMax.Name = "MAX";
            seriesMax.Color = Color.Red;

            Series seriesMin = new Series();			// Min Tolerance
            seriesMin.ChartArea = "mainArea";
            seriesMin.Legend = "mainLegend";
            seriesMin.ChartType = SeriesChartType.Line;
            seriesMin.BorderWidth = 2;
            seriesMin.Name = "MIN";
            seriesMin.Color = Color.Red;

            Series seriesNom = new Series();			// Nom Tolerance
            seriesNom.ChartArea = "mainArea";
            seriesNom.Legend = "mainLegend";
            seriesNom.ChartType = SeriesChartType.Line;
            seriesNom.BorderWidth = 2;
            seriesNom.Name = "NOM";
            seriesNom.BorderDashStyle = ChartDashStyle.Dot;
            seriesNom.BorderWidth = 2;
            seriesNom.Color = Color.Green;

            Series seriesUCL = new Series();			// avg(Thickness) + (0.5f * natTol)
            seriesUCL.ChartArea = "mainArea";
            seriesUCL.Legend = "mainLegend";
            seriesUCL.ChartType = SeriesChartType.Line;
            seriesUCL.BorderWidth = 2;
            seriesUCL.Name = "UCL";
            seriesUCL.Color = Color.DarkGreen;

            Series seriesLCL = new Series();			// avg(Thickness) - (0.5f * natTol)
            seriesLCL.ChartArea = "mainArea";
            seriesLCL.Legend = "mainLegend";
            seriesLCL.ChartType = SeriesChartType.Line;
            seriesLCL.BorderWidth = 2;
            seriesLCL.Name = "LCL";
            seriesLCL.Color = Color.DarkGreen;

            Series seriesOutOfControl = new Series();	// anything outside UCL || LCL
            seriesOutOfControl.ChartArea = "mainArea";
            seriesOutOfControl.Legend = "mainLegend";
            seriesOutOfControl.ChartType = SeriesChartType.Point;
            seriesOutOfControl.Name = "OUT OF CONTROL LIMITS";
            seriesOutOfControl.EmptyPointStyle.BorderWidth = 0;
            seriesOutOfControl.EmptyPointStyle.MarkerStyle = MarkerStyle.None;

            Series seriesRange = new Series();			// Difference between this and last thickness
            seriesRange.ChartArea = "mainArea";
            seriesRange.Legend = "mainLegend";
            seriesRange.ChartType = SeriesChartType.Line;
            seriesRange.BorderWidth = 2;
            seriesRange.Name = "RANGE";
            seriesRange.YAxisType = AxisType.Secondary;

            Series seriesAvgRange = new Series();		// Avg Range
            seriesAvgRange.ChartArea = "mainArea";
            seriesAvgRange.Legend = "mainLegend";
            seriesAvgRange.ChartType = SeriesChartType.Line;
            seriesAvgRange.Name = "AVG RANGE";
            seriesAvgRange.YAxisType = AxisType.Secondary;
            seriesAvgRange.BorderDashStyle = ChartDashStyle.Dash;
            seriesAvgRange.BorderWidth = 2;

            for (int i = 0; i < ChartData.Value.Length; i++)
            {
                seriesValue.Points.AddXY(ChartData.Blade[i], ChartData.Value[i]);
                seriesRange.Points.AddXY(ChartData.Blade[i], ChartData.Range[i]);
                seriesMax.Points.AddXY(ChartData.Blade[i], ChartData.Max[i]);
                seriesMin.Points.AddXY(ChartData.Blade[i], ChartData.Min[i]);
                seriesNom.Points.AddXY(ChartData.Blade[i], ChartData.Nom[i]);
                seriesAvgValue.Points.AddXY(ChartData.Blade[i], ChartData.AvgValue[i]);
                seriesAvgRange.Points.AddXY(ChartData.Blade[i], ChartData.AvgRange[i]);
                seriesUCL.Points.AddXY(ChartData.Blade[i], ChartData.UCL[i]);
                seriesLCL.Points.AddXY(ChartData.Blade[i], ChartData.LCL[i]);
                if (ChartData.OutOfControl[i] > 0)
                    seriesOutOfControl.Points.AddXY(ChartData.Blade[i], ChartData.OutOfControl[i]);
            }
            m_chart.Series.Add(seriesValue);
            m_chart.Series.Add(seriesRange);
            m_chart.Series.Add(seriesMax);
            m_chart.Series.Add(seriesMin);
            m_chart.Series.Add(seriesNom);
            m_chart.Series.Add(seriesAvgValue);
            m_chart.Series.Add(seriesAvgRange);
            m_chart.Series.Add(seriesUCL);
            m_chart.Series.Add(seriesLCL);
            m_chart.Series.Add(seriesOutOfControl);

            Bitmap ReturnImage;
            using (MemoryStream stream = new MemoryStream())
            {
                m_chart.SaveImage(stream);

                stream.Flush();

                ReturnImage = new Bitmap(stream);
            }

            return ReturnImage;
        }
    }
}