using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Skin_Thickness_Evaluation.Components
{
    public partial class UC_SPCReport : System.Web.UI.UserControl
    {
        public string width { get; set; }
        public string height { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            //SPCReport.Style["background-color"] = "red";
            SPCReport.Style["width"] = this.width.ToString();
            SPCReport.Style["height"] = this.height.ToString();            
        }
    }
}