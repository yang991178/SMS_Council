using System;
using System.Data;
using System.Data.SqlClient;
using SMS_Council.Classes;

namespace SMS_Council
{
    public partial class ConferenceDetail : System.Web.UI.Page
    {
        public DataRow dr;

        protected void Page_Load(object sender, EventArgs e)
        {
            SqlCommand cmd = new SqlCommand("SELECT [name], [cnum], [unum], [totalvote], [state] FROM [Conferences] WHERE [id]=@id");
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = Convert.ToInt32(Context.Request["id"].ToString()) });
            DataTable dt = Helper.RetrieveDataTable(cmd);
            if(dt.Rows.Count > 0)
            {
                dr = dt.Rows[0];
            }
            else
            {
                Context.Response.Redirect("/conference");
            }
        }
    }
}