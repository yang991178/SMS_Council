using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Data;
using System.Data.SqlClient;
using SMS_Council.Classes;

namespace SMS_Council.ajax
{
    /// <summary>
    /// Conference 的摘要说明
    /// </summary>
    public class Conference : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (context.Request.RequestType == "GET")
            {
                if(context.Request["action"].ToString() == "listvotes")
                {
                    SqlCommand cmd = new SqlCommand("SELECT [id], [title], [notvoted], [consented], [rejected], [time], [nvvote], [cvote], [rvote], [ratio], [halfmajority], [absmajority], [active] FROM [Votes] WHERE [conference] = @conference ORDER BY [time] DESC");
                    cmd.Parameters.Add(new SqlParameter("@conference", SqlDbType.Int) { Value = Convert.ToInt32(context.Request["conference"].ToString()) });
                    DataTable dt = Helper.RetrieveDataTable(cmd);
                    dt.Columns["notvoted"].ColumnName = "_notvoted";
                    dt.Columns["consented"].ColumnName = "_consented";
                    dt.Columns["rejected"].ColumnName = "_rejected";
                    dt.Columns["time"].ColumnName = "_time";
                    dt.Columns.AddRange(new DataColumn[] {
                        new DataColumn("notvoted", typeof(int[])),
                        new DataColumn("consented", typeof(int[])),
                        new DataColumn("rejected", typeof(int[])),
                        new DataColumn("time", typeof(string))
                    });
                    foreach (DataRow r in dt.Rows)
                    {
                        r["notvoted"] = r["_notvoted"].ToString() == "" ? new int[0] : r["_notvoted"].ToString().Split(',').Select(int.Parse).ToArray();
                        r["consented"] = r["_consented"].ToString() == "" ? new int[0] : r["_consented"].ToString().Split(',').Select(int.Parse).ToArray();
                        r["rejected"] = r["_rejected"].ToString() == "" ? new int[0] : r["_rejected"].ToString().Split(',').Select(int.Parse).ToArray();
                        r["time"] = DateTime.Parse(r["_time"].ToString()).ToString("yyyy年M月d日HH:mm");
                    }
                    dt.Columns.RemoveAt(2);
                    dt.Columns.RemoveAt(2);
                    dt.Columns.RemoveAt(2);
                    dt.Columns.RemoveAt(2);
                    context.Response.Write(Helper.DataTableToJson(dt));
                }
                else if (context.Request["action"].ToString() == "getvote")
                {
                    SqlCommand cmd = new SqlCommand("SELECT [id], [title], [notvoted], [consented], [rejected], [nvvote], [cvote], [rvote], [ratio], [halfmajority], [absmajority], [active] FROM [Votes] WHERE [id] = @id");
                    cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = Convert.ToInt32(context.Request["id"].ToString()) });
                    DataTable dt = Helper.RetrieveDataTable(cmd);
                    dt.Columns["notvoted"].ColumnName = "_notvoted";
                    dt.Columns["consented"].ColumnName = "_consented";
                    dt.Columns["rejected"].ColumnName = "_rejected";
                    dt.Columns.AddRange(new DataColumn[] {
                        new DataColumn("notvoted", typeof(int[])),
                        new DataColumn("consented", typeof(int[])),
                        new DataColumn("rejected", typeof(int[]))
                    });
                    foreach (DataRow r in dt.Rows)
                    {
                        r["notvoted"] = r["_notvoted"].ToString() == "" ? new int[0] : r["_notvoted"].ToString().Split(',').Select(int.Parse).ToArray();
                        r["consented"] = r["_consented"].ToString() == "" ? new int[0] : r["_consented"].ToString().Split(',').Select(int.Parse).ToArray();
                        r["rejected"] = r["_rejected"].ToString() == "" ? new int[0] : r["_rejected"].ToString().Split(',').Select(int.Parse).ToArray();
                    }
                    dt.Columns.RemoveAt(2);
                    dt.Columns.RemoveAt(2);
                    dt.Columns.RemoveAt(2);
                    context.Response.Write(Helper.DataTableToJson(dt));
                }
                else if(Classes.User.IsLogin && Classes.User.Current.Role == 0 && context.Request["action"].ToString() == "listconferences")
                {
                    SqlCommand cmd = new SqlCommand("SELECT [id], [name], [cparticipants], [uparticipants], [time], [ratio], [state] FROM [Conferences] ORDER BY [id] DESC");
                    DataTable dt = Helper.RetrieveDataTable(cmd);
                    dt.Columns["cparticipants"].ColumnName = "_cparticipants";
                    dt.Columns["uparticipants"].ColumnName = "_uparticipants";
                    dt.Columns["time"].ColumnName = "_time";
                    dt.Columns.AddRange(new DataColumn[] {
                        new DataColumn("cparticipants", typeof(int[])),
                        new DataColumn("uparticipants", typeof(int[])),
                        new DataColumn("time", typeof(string))
                    });
                    foreach (DataRow r in dt.Rows)
                    {
                        r["cparticipants"] = r["_cparticipants"].ToString() == "" ? new int[0] : r["_cparticipants"].ToString().Split(',').Select(int.Parse).ToArray();
                        r["uparticipants"] = r["_uparticipants"].ToString() == "" ? new int[0] : r["_uparticipants"].ToString().Split(',').Select(int.Parse).ToArray();
                        r["time"] = DateTime.Parse(r["_time"].ToString()).ToString("yyyy年M月d日HH:mm");
                    }
                    dt.Columns.RemoveAt(2);
                    dt.Columns.RemoveAt(2);
                    dt.Columns.RemoveAt(2);
                    context.Response.Write(Helper.DataTableToJson(dt));
                }
            }
            else if (context.Request.RequestType == "POST")
            {
                if (context.Request["action"].ToString() == "vote" && Classes.User.IsLogin)
                {
                    int id = Convert.ToInt32(context.Request.Form["id"].ToString());
                    SqlCommand cmd = new SqlCommand("SELECT [notvoted], [consented], [rejected], [nvvote], [cvote], [rvote], [ratio], [active] FROM [Votes] WHERE [id]=@id");
                    cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
                    DataTable dt = Helper.RetrieveDataTable(cmd);
                    var r = dt.Rows[0];
                    List<int> notvoted = r["notvoted"].ToString() == "" ? new List<int>() : r["notvoted"].ToString().Split(',').Select(int.Parse).ToList();
                    var uid = Classes.User.Current.Id;
                    var role = Classes.User.Current.Role;
                    if (role == 0 && context.Request.Form["uid"] != null)
                    {
                        uid = Convert.ToInt32(context.Request.Form["uid"].ToString());
                        cmd = new SqlCommand("SELECT [role] FROM [Users] WHERE [id]=@id");
                        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = uid });
                        role = Convert.ToInt32(Helper.RetrieveDataTable(cmd).Rows[0][0].ToString());
                    }
                    if (r["active"].ToString() == "True" && notvoted.Contains(uid))
                    {
                        SqlConnection conn = Helper.Conn;
                        if (context.Request.Form["decision"].ToString() == "C")
                        {
                            List<int> consented = r["consented"].ToString() == "" ? new List<int>() : r["consented"].ToString().Split(',').Select(int.Parse).ToList();
                            consented.Add(uid);
                            notvoted.Remove(uid);
                            int vote = role == 2 ? Convert.ToInt32(r["ratio"].ToString()) : 1;
                            int nvvote = Convert.ToInt32(r["nvvote"].ToString()) - vote;
                            int cvote = Convert.ToInt32(r["cvote"].ToString()) + vote;
                            cmd = new SqlCommand("UPDATE [Votes] SET [notvoted]=@notvoted, [consented]=@consented, [nvvote]=@nvvote, [cvote]=@cvote WHERE [id]=@id", conn);
                            cmd.Parameters.AddRange(new SqlParameter[] {
                                new SqlParameter("@notvoted", SqlDbType.VarChar){ Value = string.Join(",",notvoted.Select(x => x.ToString()).ToArray()) },
                                new SqlParameter("@consented", SqlDbType.VarChar){ Value = string.Join(",",consented.Select(x => x.ToString()).ToArray()) },
                                new SqlParameter("@nvvote", SqlDbType.SmallInt){ Value = nvvote },
                                new SqlParameter("@cvote", SqlDbType.SmallInt){ Value = cvote },
                                new SqlParameter("@id", SqlDbType.Int) { Value = id }
                            });
                            conn.Open();
                            try
                            {
                                cmd.ExecuteNonQuery();
                                context.Response.Write("1");
                            }
                            catch
                            {
                                context.Response.Write("-10");
                            }
                            finally
                            {
                                conn.Close();
                            }
                        }
                        else if (context.Request.Form["decision"].ToString() == "R")
                        {
                            List<int> rejected = r["rejected"].ToString() == "" ? new List<int>() : r["rejected"].ToString().Split(',').Select(int.Parse).ToList();
                            rejected.Add(uid);
                            notvoted.Remove(uid);
                            int vote = role == 2 ? Convert.ToInt32(r["ratio"].ToString()) : 1;
                            int nvvote = Convert.ToInt32(r["nvvote"].ToString()) - vote;
                            int rvote = Convert.ToInt32(r["rvote"].ToString()) + vote;
                            cmd = new SqlCommand("UPDATE [Votes] SET [notvoted]=@notvoted, [rejected]=@rejected, [nvvote]=@nvvote, [rvote]=@rvote WHERE [id]=@id", conn);
                            cmd.Parameters.AddRange(new SqlParameter[] {
                                new SqlParameter("@notvoted", SqlDbType.VarChar){ Value = string.Join(",",notvoted.Select(x => x.ToString()).ToArray()) },
                                new SqlParameter("@rejected", SqlDbType.VarChar){ Value = string.Join(",",rejected.Select(x => x.ToString()).ToArray()) },
                                new SqlParameter("@nvvote", SqlDbType.SmallInt){ Value = nvvote },
                                new SqlParameter("@rvote", SqlDbType.SmallInt){ Value = rvote },
                                new SqlParameter("@id", SqlDbType.Int) { Value = id }
                            });
                            conn.Open();
                            try
                            {
                                cmd.ExecuteNonQuery();
                                context.Response.Write("1");
                            }
                            catch
                            {
                                context.Response.Write("-10");
                            }
                            finally
                            {
                                conn.Close();
                            }
                        }
                    }
                    else
                    {
                        context.Response.Write("-2");
                    }
                }
                else if (Classes.User.IsLogin && Classes.User.Current.Role == 0 && context.Request["action"].ToString() == "addconference")
                {
                    SqlConnection conn = Helper.Conn;
                    SqlCommand cmd = new SqlCommand("INSERT INTO [Conferences] ([name], [cparticipants], [cnum], [uparticipants], [unum], [ratio], [state], [time]) VALUES(@name, @cparticipants, @cnum, @uparticipants, @unum, @ratio, @state, @time)", conn);
                    cmd.Parameters.AddRange(new SqlParameter[]
                    {
                        new SqlParameter("@name", SqlDbType.NVarChar, 50) {Value=context.Request.Form["name"].ToString()},
                        new SqlParameter("@cparticipants", SqlDbType.NVarChar) {Value=context.Request.Form["cparticipants"]},
                        new SqlParameter("@cnum", SqlDbType.SmallInt) {Value=context.Request.Form["cnum"]},
                        new SqlParameter("@uparticipants", SqlDbType.NVarChar) {Value=context.Request.Form["uparticipants"]},
                        new SqlParameter("@unum", SqlDbType.SmallInt) {Value=context.Request.Form["unum"]},
                        new SqlParameter("@ratio", SqlDbType.TinyInt) {Value=context.Request.Form["ratio"]},
                        new SqlParameter("@state", SqlDbType.TinyInt) {Value=context.Request.Form["state"]},
                        new SqlParameter("@time", SqlDbType.DateTime) {Value=DateTime.UtcNow.AddHours(8)}
                    });
                    conn.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        context.Response.Write("1");
                    }
                    catch
                    {
                        context.Response.Write("-10");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                else if (Classes.User.IsLogin && Classes.User.Current.Role == 0 && context.Request["action"].ToString() == "updateconference")
                {
                    SqlConnection conn = Helper.Conn;
                    SqlCommand cmd = new SqlCommand("UPDATE [Conferences] SET [name]=@name, [cparticipants]=@cparticipants, [cnum]=@cnum, [uparticipants]=@uparticipants, [unum]=@unum, [ratio]=@ratio, [state]=@state, [time]=@time WHERE [id]=@id", conn);
                    cmd.Parameters.AddRange(new SqlParameter[]
                    {
                        new SqlParameter("@id", SqlDbType.Int) {Value=context.Request.Form["id"].ToString()},
                        new SqlParameter("@name", SqlDbType.NVarChar, 50) {Value=context.Request.Form["name"].ToString()},
                        new SqlParameter("@cparticipants", SqlDbType.NVarChar) {Value=context.Request.Form["cparticipants"]},
                        new SqlParameter("@cnum", SqlDbType.SmallInt) {Value=context.Request.Form["cnum"]},
                        new SqlParameter("@uparticipants", SqlDbType.NVarChar) {Value=context.Request.Form["uparticipants"]},
                        new SqlParameter("@unum", SqlDbType.SmallInt) {Value=context.Request.Form["unum"]},
                        new SqlParameter("@ratio", SqlDbType.TinyInt) {Value=context.Request.Form["ratio"]},
                        new SqlParameter("@state", SqlDbType.TinyInt) {Value=context.Request.Form["state"]},
                        new SqlParameter("@time", SqlDbType.DateTime) {Value=DateTime.UtcNow.AddHours(8)}
                    });
                    conn.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        context.Response.Write("1");
                    }
                    catch
                    {
                        context.Response.Write("-10");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                else if (Classes.User.IsLogin && Classes.User.Current.Role == 0 && context.Request["action"].ToString() == "addvote")
                {
                    int id = Convert.ToInt32(context.Request.Form["id"].ToString());
                    SqlCommand cmd = new SqlCommand("SELECT * FROM [Conferences] WHERE [id]=@id");
                    cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
                    DataTable dt = Helper.RetrieveDataTable(cmd);
                    if (dt.Rows.Count > 0)
                    {
                        var r = dt.Rows[0];
                        List<string> nv = new List<string>();
                        nv.AddRange(r["cparticipants"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                        nv.AddRange(r["uparticipants"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                        string notvoted = String.Join(",", nv);
                        var conn = Helper.Conn;
                        cmd = new SqlCommand("INSERT INTO [Votes] ([conference], [title], [notvoted], [nvvote], [cvote], [rvote], [ratio], [halfmajority], [absmajority], [active], [time]) VALUES(@conference, @title, @notvoted, @nvvote, 0, 0, @ratio, @halfmajority, @absmajority, 1, @time)", conn);
                        cmd.Parameters.AddRange(new SqlParameter[]
                        {
                            new SqlParameter("@conference", SqlDbType.Int) {Value=id},
                            new SqlParameter("@title", SqlDbType.NVarChar, 53) {Value="表决："+ context.Request.Form["title"].ToString()},
                            new SqlParameter("@notvoted", SqlDbType.VarChar) {Value=notvoted},
                            new SqlParameter("@nvvote", SqlDbType.SmallInt) {Value=Convert.ToInt32(r["totalvote"].ToString())},
                            new SqlParameter("@ratio", SqlDbType.TinyInt) {Value=Convert.ToInt32(r["ratio"].ToString())},
                            new SqlParameter("@halfmajority", SqlDbType.SmallInt) {Value=Convert.ToInt32(r["halfmajority"].ToString())},
                            new SqlParameter("@absmajority", SqlDbType.SmallInt) {Value=Convert.ToInt32(r["absmajority"].ToString())},
                            new SqlParameter("@time", SqlDbType.DateTime) {Value=DateTime.UtcNow.AddHours(8)}
                        });
                        conn.Open();
                        try
                        {
                            cmd.ExecuteNonQuery();
                            context.Response.Write("1");
                        }
                        catch
                        {
                            context.Response.Write("-10");
                        }
                        finally
                        {
                            conn.Close();
                        }
                    }
                    else
                    {
                        context.Response.Write("-2");
                    }
                }
                else if (Classes.User.IsLogin && Classes.User.Current.Role == 0 && context.Request["action"].ToString() == "deletevote")
                {
                    var conn = Helper.Conn;
                    var cmd = new SqlCommand("DELETE FROM [Votes] WHERE [id]=@id", conn);
                    cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = Convert.ToInt32(context.Request.Form["id"].ToString()) });
                    conn.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        context.Response.Write("1");
                    }
                    catch
                    {
                        context.Response.Write("-10");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                else if (Classes.User.IsLogin && Classes.User.Current.Role == 0 && context.Request["action"].ToString() == "deleteconference")
                {
                    var conn = Helper.Conn;
                    var cmd = new SqlCommand("DELETE FROM [Conferences] WHERE [id]=@id", conn);
                    cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = Convert.ToInt32(context.Request.Form["id"].ToString()) });
                    conn.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        context.Response.Write("1");
                    }
                    catch
                    {
                        context.Response.Write("-10");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                else if (Classes.User.IsLogin && Classes.User.Current.Role == 0 && context.Request["action"].ToString() == "setvotestate")
                {
                    var conn = Helper.Conn;
                    var cmd = new SqlCommand("UPDATE [Votes] SET [active]=@active WHERE [id]=@id", conn);
                    cmd.Parameters.AddRange(new SqlParameter[]{
                        new SqlParameter("@id", SqlDbType.Int) { Value = Convert.ToInt32(context.Request.Form["id"].ToString()) },
                        new SqlParameter("@active", SqlDbType.Bit) { Value = context.Request.Form["active"].ToString() == "true" }
                    });
                    conn.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        context.Response.Write("1");
                    }
                    catch
                    {
                        context.Response.Write("-10");
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                else
                {
                    context.Response.Write("-1");
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