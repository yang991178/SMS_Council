using System;
using System.Data;
using System.Data.SqlClient;
using SMS_Council.Classes;

namespace SMS_Council
{
    public partial class Conference : System.Web.UI.Page
    {
        public DataRow latest;

        protected void Page_Load(object sender, EventArgs e)
        {
            DataTable dt = Helper.RetrieveDataTable(new SqlCommand("SELECT [id], [name], [time], [totalvote], [state] FROM [Conferences] ORDER BY [time] DESC"));
            latest = dt.Copy().Rows[0];
            dt.Rows.RemoveAt(0);
            repeater.DataSource = dt;
            repeater.DataBind();
        }
    }
}