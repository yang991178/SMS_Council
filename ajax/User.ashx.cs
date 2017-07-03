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
        string[] units = new string[] { "", "一、二", "", "三、四", "", "五", "六", "七", "八" };
        int y;

        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.RequestType == "GET")
            {
                if (context.Request["action"].ToString() == "list")
                {
                    string response = "[";
                    response = response + Helper.DataTableToJson(Helper.RetrieveDataTable(new SqlCommand("SELECT [id], [username], [name], [class], [unit], [phone], [display] FROM [Users] WHERE [role] = 1 AND [activated] = 1 ORDER BY [class], [id]")));
                    response = response + ",";
                    response = response + Helper.DataTableToJson(Helper.RetrieveDataTable(new SqlCommand("SELECT [id], [username], [name], [class], [unit], [phone], [display] FROM [Users] WHERE [role] = 2 AND [activated] = 1 ORDER BY [class], [id]")));
                    response = response + "]";
                    response = response.Replace("class", "cls");
                    context.Response.Write(response);
                }
                else if (Classes.User.IsLogin && Classes.User.Current.Role == 0 && context.Request["action"].ToString() == "listretired")
                {
                    string response = Helper.DataTableToJson(Helper.RetrieveDataTable(new SqlCommand("SELECT [id], [username], [name], [class], [unit], [phone], [display] FROM [Users] WHERE [role] > 0 AND [activated] = 0 ORDER BY [id] DESC")));
                    response = response.Replace("class", "cls");
                    context.Response.Write(response);
                }
                else if (context.Request["action"].ToString() == "listsequence")
                {
                    string sql = Classes.User.IsLogin && Classes.User.Current.Role == 0 ? "SELECT [id], [name], [class], [unit] FROM [Users] ORDER BY [id] ASC" : "SELECT [id], [name] FROM [Users] WHERE [display] = 1 ORDER BY [id] ASC";
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
                else if (Classes.User.IsLogin && Classes.User.Current.Role == 0 && context.Request["action"].ToString() == "getdefaultpassword" && int.TryParse(context.Request["id"].ToString(), out y))
                {
                    var dt = Helper.RetrieveDataTable(new SqlCommand("SELECT [password], [defaultpassword] FROM [Users] WHERE [id]=" + y));
                    if (dt.Rows.Count > 0)
                    {
                        string currentpassword = dt.Rows[0][0].ToString();
                        string defaultpassword = dt.Rows[0][1].ToString();
                        string password = Helper.EncryptPassword(defaultpassword);
                        bool changed = password == currentpassword;
                        context.Response.Write(string.Format("[\"{0}\",\"{1}\"]", defaultpassword, !changed));
                    }
                }
                else if (Classes.User.IsLogin && Classes.User.Current.Role == 0 && context.Request["action"].ToString() == "getdefaultpasswordyear" && int.TryParse(context.Request["year"].ToString(), out y) && y > 14)
                {
                    var dt = Helper.RetrieveDataTable(new SqlCommand("SELECT [name], [username], [defaultpassword] FROM [Users] WHERE [role] > 0 AND [display] = 1 AND [username] LIKE'%" + y + "_'"));
                    context.Response.Write(Helper.DataTableToJson(dt));
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
                else if (Classes.User.IsLogin && Classes.User.Current.Role == 0)
                {
                    if (context.Request["action"].ToString() == "resetpassword")
                    {
                        var newpassword = Helper.GetRnd(6);
                        var encrypted = Helper.EncryptPassword(newpassword);
                        SqlConnection conn = Helper.Conn;
                        SqlCommand cmd = new SqlCommand("UPDATE [Users] SET [password]=@new, [defaultpassword]=@raw WHERE [id]=@id", conn);
                        cmd.Parameters.AddRange(new SqlParameter[] {
                        new SqlParameter("@new", SqlDbType.Char, 32) { Value = encrypted },
                        new SqlParameter("@raw", SqlDbType.VarChar, 6) { Value = newpassword },
                        new SqlParameter("@id", SqlDbType.Int) { Value = int.Parse(context.Request.Form["id"].ToString()) }
                    });
                        conn.Open();
                        try
                        {
                            cmd.ExecuteNonQuery();
                            context.Response.Write("[\"1\", \"" + newpassword + "\"]");
                        }
                        catch
                        {
                            context.Response.Write("[\"-10\"]");
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                    else if (context.Request["action"].ToString() == "updateuser")
                    {
                        int id = int.Parse(context.Request.Form["id"].ToString());
                        int role = int.Parse(context.Request.Form["type"].ToString());
                        string username = context.Request.Form["username"].ToString();
                        var cmd = new SqlCommand("SELECT [id] FROM [Users] WHERE [username]=@username");
                        cmd.Parameters.Add(new SqlParameter("@username", SqlDbType.VarChar, 30) { Value = username });
                        var dt = Helper.RetrieveDataTable(cmd);
                        if (dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != id.ToString())
                            context.Response.Write("[-1]");
                        else if (role == 0)
                            context.Response.Write("[-2]");
                        else
                        {
                            SqlConnection conn = Helper.Conn;
                            cmd = new SqlCommand("UPDATE [Users] SET [name]=@name, [username]=@username, [class]=@class, [unit]=@unit, [phone]=@phone, [role]=@role, [activated]=@activated WHERE [id]=@id", conn);
                            cmd.Parameters.AddRange(new SqlParameter[]
                            {
                                new SqlParameter("@name", SqlDbType.NVarChar, 10) { Value = context.Request.Form["name"].ToString() },
                                new SqlParameter("@username", SqlDbType.VarChar, 30) { Value = username },
                                new SqlParameter("@id", SqlDbType.Int) { Value = id },
                                new SqlParameter("@role", SqlDbType.TinyInt) { Value = role },
                                new SqlParameter("@class", SqlDbType.TinyInt) { Value = int.Parse(context.Request.Form["class"].ToString()) },
                                new SqlParameter("@phone", SqlDbType.VarChar, 11) { IsNullable = true, Value = context.Request.Form["phone"].ToString() },
                                new SqlParameter("@activated", SqlDbType.Bit) { Value = context.Request.Form["state"].ToString() == "true" ? 1 : 0 }
                            });
                            if (role == 2)
                                cmd.Parameters.Add(new SqlParameter("@unit", SqlDbType.NVarChar, 10) { Value = units[int.Parse(context.Request.Form["unit"].ToString())] });
                            else
                                cmd.Parameters.Add(new SqlParameter("@unit", DBNull.Value));
                            conn.Open();
                            try
                            {
                                cmd.ExecuteNonQuery();
                                context.Response.Write("[1]");
                            }
                            catch
                            {
                                context.Response.Write("[-10]");
                            }
                            finally
                            {
                                conn.Close();
                            }
                        }
                    }
                    else if (context.Request["action"].ToString() == "newuser")
                    {
                        int role = int.Parse(context.Request.Form["type"].ToString());
                        string username = context.Request.Form["username"].ToString();
                        var cmd = new SqlCommand("SELECT [id] FROM [Users] WHERE [username]=@username");
                        cmd.Parameters.Add(new SqlParameter("@username", SqlDbType.VarChar, 30) { Value = username });
                        var dt = Helper.RetrieveDataTable(cmd);
                        if (dt.Rows.Count > 0)
                            context.Response.Write("[-1]");
                        else if (role == 0)
                            context.Response.Write("[-2]");
                        else
                        {
                            string raw = Helper.GetRnd(6);
                            string password = Helper.EncryptPassword(raw);
                            SqlConnection conn = Helper.Conn;
                            cmd = new SqlCommand("INSERT INTO [Users] ([username], [password], [name], [class], [unit], [phone], [role], [activated], [defaultpassword]) VALUES (@username, @password, @name, @class, @unit, @phone, @role, @activated, @defaultpassword)", conn);
                            cmd.Parameters.AddRange(new SqlParameter[]
                            {
                                new SqlParameter("@name", SqlDbType.NVarChar, 10) { Value = context.Request.Form["name"].ToString() },
                                new SqlParameter("@username", SqlDbType.VarChar, 30) { Value = username },
                                new SqlParameter("@role", SqlDbType.TinyInt) { Value = role },
                                new SqlParameter("@class", SqlDbType.TinyInt) { Value = int.Parse(context.Request.Form["class"].ToString()) },
                                new SqlParameter("@phone", SqlDbType.VarChar, 11) { IsNullable = true, Value = context.Request.Form["phone"].ToString() },
                                new SqlParameter("@activated", SqlDbType.Bit) { Value = context.Request.Form["state"].ToString() == "true" ? 1 : 0 },
                                new SqlParameter("@password", SqlDbType.Char, 32) { Value = password },
                                new SqlParameter("@defaultpassword", SqlDbType.VarChar, 6) { Value = raw }
                            });
                            if (role == 2)
                                cmd.Parameters.Add(new SqlParameter("@unit", SqlDbType.NVarChar, 10) { Value = units[int.Parse(context.Request.Form["unit"].ToString())] });
                            else
                                cmd.Parameters.Add(new SqlParameter("@unit", DBNull.Value));
                            conn.Open();
                            try
                            {
                                cmd.ExecuteNonQuery();
                                context.Response.Write("[1]");
                            }
                            catch
                            {
                                context.Response.Write("[-10]");
                            }
                            finally
                            {
                                conn.Close();
                            }
                        }
                    }
                    else if (context.Request["action"].ToString() == "retireyear" && int.TryParse(context.Request.Form["year"].ToString(), out y) && y > 14)
                    {
                        var conn = Helper.Conn;
                        var cmd = new SqlCommand("UPDATE [Users] SET [activated]=0 WHERE [role] > 0 AND [display] = 1 AND [username] LIKE '%" + y + "_'", conn);
                        conn.Open();
                        try
                        {
                            cmd.ExecuteNonQuery();
                            context.Response.Write("[1]");
                        }
                        catch
                        {
                            context.Response.Write("[-10]");
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
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