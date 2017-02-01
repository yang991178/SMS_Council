using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SMS_Council
{
    public partial class ConferenceManage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Classes.User.IsLogin || Classes.User.Current.Role != 0)
            {
                Context.Response.Redirect("/");
            }
        }
    }
}