using System;
using System.Web.Services;

namespace Skin_Thickness_Evaluation.Components.Filter
{
    public partial class Filter_BladeList : System.Web.UI.UserControl
    {
        public bool MultiSelect { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            divSelectedBlade.Visible = !this.MultiSelect;
        }


    }
}