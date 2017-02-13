using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using SMS_Council.Classes;

namespace SMS_Council.ajax
{
    /// <summary>
    /// User 的摘要说明
    /// </summary>
    public class User : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.RequestType == "GET")
            {
                if (context.Request["action"].ToString() == "list")
                {
                    string response = "[";
                    response = response + Helper.DataTableToJson(Helper.RetrieveDataTable(new SqlCommand("SELECT [id], [name], [class], [unit], [phone], [display] FROM [Users] WHERE [role] = 1 AND [activated] = 1 ORDER BY [class], [id]")));
                    response = response + ",";
                    response = response + Helper.DataTableToJson(Helper.RetrieveDataTable(new SqlCommand("SELECT [id], [name], [class], [unit], [phone], [display] FROM [Users] WHERE [role] = 2 AND [activated] = 1 ORDER BY [class], [id]")));
                    response = response + "]";
                    response = response.Replace("class", "cls");
                    context.Response.Write(response);
                }
                else if(context.Request["action"].ToString() == "listsequence")
                {
                    string sql = Classes.User.IsLogin && Classes.User.Current.Role == 0 ? "SELECT [id], [name], [class], [unit] FROM [Users] WHERE [activated] = 1 ORDER BY [id] ASC" : "SELECT [id], [name] FROM [Users] WHERE [activated] = 1 AND [display] = 1 ORDER BY [id] ASC";
                    var dt = Helper.RetrieveDataTable(new SqlCommand(sql));
                    if (Classes.User.IsLogin && Classes.User.Current.Role == 0)
                        dt.Columns["class"].ColumnName = "cls";
                    context.Response.Write(Helper.DataTableToJson(dt));
                }
                else if (context.Request["action"].ToString() == "logout")
                {
                    Classes.User.LogOut();
                    context.Response.Write("[1]");
                }
            }
            else if (context.Request.RequestType == "POST")
            {
                if (context.Request["action"].ToString() == "login")
                {
                    var flag = Classes.User.LogIn(context.Request.Form["username"].ToString(),context.Request.Form["password"].ToString());
                    int[] response = new int[1] { flag };
                    context.Response.Write(new JArray(response).ToString());
                }
                else if (context.Request["action"].ToString() == "changepassword")
                {
                    var flag = Classes.User.ChangePassword(context.Request.Form["old"].ToString(), context.Request.Form["new"].ToString());
                    int[] response = new int[1] { flag };
                    context.Response.Write(new JArray(response).ToString());
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}