using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace protectTreesV2._mp
{
    public partial class mp_backstage : System.Web.UI.MasterPage
    {

        public string menuStr_def
        {
            get { return ViewState["mp_menuStr_def"] as string ?? string.Empty; }
            set { ViewState["mp_menuStr_def"] = value; }
        }

        public string modalStatic
        {
            get { return ViewState["modalStatic"] as string ?? string.Empty; }
            set { ViewState["modalStatic"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //throw new Exception("THIS IS THE REAL ONE");
        }
    }
}