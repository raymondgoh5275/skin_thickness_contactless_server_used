using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Skin_Thickness_Evaluation
{
    public partial class _Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.IsPostBack)
            {
                string BladeList = Request.Form["mslBladeList"];

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

                Response.Redirect(string.Format("{0}.aspx", Request.Form["UseCase"]));
            }
        }
    }
}
