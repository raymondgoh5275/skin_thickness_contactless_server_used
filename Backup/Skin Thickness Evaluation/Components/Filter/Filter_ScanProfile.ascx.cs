using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BLL;

namespace Skin_Thickness_Evaluation.Components.Filter
{
    public partial class Filter_ScanProfile : System.Web.UI.UserControl
    {
        public DropDownList ScanProfile 
        { 
            get {return Select_ScanProfile; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Session.IsNewSession)
                {
                    Session.Add("BladeList", "");
                }
                Fetch_ScanProfiles();
            }
        }

        private void Fetch_ScanProfiles()
        {
            BizLogic BLL = new BizLogic();

            Dictionary<string, string> ProfileList = new Dictionary<string,string>();
                
            if (string.IsNullOrEmpty(Session["BladeList"].ToString()))
                ProfileList = BLL.Get_ProfileList();
            else
                ProfileList = BLL.Get_ProfileList(Session["BladeList"].ToString());

            foreach (KeyValuePair<string, string> profileItem in ProfileList)
            {
                ListItem litem = new ListItem(profileItem.Key.ToString(), profileItem.Value.ToString());
                Select_ScanProfile.Items.Add(litem);
            }
            Select_ScanProfile.SelectedIndex = 0;
        }
    }
}