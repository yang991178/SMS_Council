using System;
using System.Data;
using System.Data.SqlClient;
using SMS_Council.Classes;

namespace SMS_Council
{
    public partial class VoteManage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Classes.User.IsLogin || Classes.User.Current.Role != 0)
            {
                Context.Response.Redirect("/");
            }
            SqlCommand cmd = new SqlCommand("SELECT [active] FROM [Votes] WHERE [id]=@id");
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = Convert.ToInt32(Context.Request["id"].ToString()) });
            DataTable dt = Helper.RetrieveDataTable(cmd);
            if (dt.Rows.Count == 0)
                Context.Response.Redirect("/conference/manage");
        }
    }
}