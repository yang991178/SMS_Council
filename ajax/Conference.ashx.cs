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
                    SqlCommand cmd = new SqlCommand("SELECT [id], [title], [notvoted], [consented], [rejected], [abstained], [time], [nvvote], [cvote], [rvote], [avote], [ratio], [halfmajority], [absmajority], [active], [type], [passvote] FROM [Votes] WHERE [conference] = @conference ORDER BY [time] DESC");
                    cmd.Parameters.Add(new SqlParameter("@conference", SqlDbType.Int) { Value = Convert.ToInt32(context.Request["conference"].ToString()) });
                    DataTable dt = Helper.RetrieveDataTable(cmd);
                    dt.Columns["notvoted"].ColumnName = "_notvoted";
                    dt.Columns["consented"].ColumnName = "_consented";
                    dt.Columns["rejected"].ColumnName = "_rejected";
                    dt.Columns["abstained"].ColumnName = "_abstained";
                    dt.Columns["time"].ColumnName = "_time";
                    dt.Columns.AddRange(new DataColumn[] {
                        new DataColumn("notvoted", typeof(int[])),
                        new DataColumn("consented", typeof(int[])),
                        new DataColumn("rejected", typeof(int[])),
                        new DataColumn("abstained", typeof(int[])),
                        new DataColumn("time", typeof(string))
                    });
                    foreach (DataRow r in dt.Rows)
                    {
                        r["notvoted"] = r["_notvoted"].ToString() == "" ? new int[0] : r["_notvoted"].ToString().Split(',').Select(int.Parse).ToArray();
                        r["consented"] = r["_consented"].ToString() == "" ? new int[0] : r["_consented"].ToString().Split(',').Select(int.Parse).ToArray();
                        r["rejected"] = r["_rejected"].ToString() == "" ? new int[0] : r["_rejected"].ToString().Split(',').Select(int.Parse).ToArray();
                        r["abstained"] = r["_abstained"].ToString() == "" ? new int[0] : r["_abstained"].ToString().Split(',').Select(int.Parse).ToArray();
                        r["time"] = DateTime.Parse(r["_time"].ToString()).ToString("yyyy年M月d日HH:mm");
                    }
                    dt.Columns.RemoveAt(2);
                    dt.Columns.RemoveAt(2);
                    dt.Columns.RemoveAt(2);
                    dt.Columns.RemoveAt(2);
                    dt.Columns.RemoveAt(2);
                    context.Response.Write(Helper.DataTableToJson(dt));
                }
                else if (context.Request["action"].ToString() == "getvote")
                {
                    SqlCommand cmd = new SqlCommand("SELECT [id], [title], [notvoted], [consented], [rejected], [abstained], [nvvote], [cvote], [rvote], [avote], [ratio], [halfmajority], [absmajority], [active], [type], [passvote] FROM [Votes] WHERE [id] = @id");
                    cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = Convert.ToInt32(context.Request["id"].ToString()) });
                    DataTable dt = Helper.RetrieveDataTable(cmd);
                    dt.Columns["notvoted"].ColumnName = "_notvoted";
                    dt.Columns["consented"].ColumnName = "_consented";
                    dt.Columns["rejected"].ColumnName = "_rejected";
                    dt.Columns["abstained"].ColumnName = "_abstained";
                    dt.Columns.AddRange(new DataColumn[] {
                        new DataColumn("notvoted", typeof(int[])),
                        new DataColumn("consented", typeof(int[])),
                        new DataColumn("rejected", typeof(int[])),
                        new DataColumn("abstained", typeof(int[])),
                    });
                    foreach (DataRow r in dt.Rows)
                    {
                        r["notvoted"] = r["_notvoted"].ToString() == "" ? new int[0] : r["_notvoted"].ToString().Split(',').Select(int.Parse).ToArray();
                        r["consented"] = r["_consented"].ToString() == "" ? new int[0] : r["_consented"].ToString().Split(',').Select(int.Parse).ToArray();
                        r["rejected"] = r["_rejected"].ToString() == "" ? new int[0] : r["_rejected"].ToString().Split(',').Select(int.Parse).ToArray();
                        r["abstained"] = r["_abstained"].ToString() == "" ? new int[0] : r["_abstained"].ToString().Split(',').Select(int.Parse).ToArray();
                    }
                    dt.Columns.RemoveAt(2);
                    dt.Columns.RemoveAt(2);
                    dt.Columns.RemoveAt(2);
                    dt.Columns.RemoveAt(2);
                    context.Response.Write(Helper.DataTableToJson(dt));
                }
                else if(Classes.User.IsLogin && Classes.User.Current.Role == 0) {
                    if(context.Request["action"].ToString() == "listconferences")
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
                    else if(context.Request["action"].ToString() == "listdelegations")
                    {
                        SqlCommand cmd = new SqlCommand("SELECT * FROM [Delegations] WHERE [conference]=@conference");
                        cmd.Parameters.Add(new SqlParameter("@conference", SqlDbType.Int) { Value = Convert.ToInt32(context.Request["conference"]) });
                        var dt = Helper.RetrieveDataTable(cmd);
                        dt.Columns["object"].ColumnName = "_object";
                        dt.Columns.Add(new DataColumn("object", typeof(int[])));
                        foreach (DataRow r in dt.Rows)
                            r["object"] = r["_object"].ToString() == "" ? new int[0] : r["_object"].ToString().Split(',').Select(int.Parse).ToArray();
                        dt.Columns.RemoveAt(3);
                        context.Response.Write(Helper.DataTableToJson(dt));
                    }
                }
            }
            else if (context.Request.RequestType == "POST")
            {
                if (context.Request["action"].ToString() == "vote" && Classes.User.IsLogin)
                {
                    var id = Convert.ToInt32(context.Request.Form["id"].ToString());
                    var uid = Classes.User.Current.Role == 0 && context.Request.Form["uid"] != null ? Convert.ToInt32(context.Request.Form["uid"].ToString()) : Classes.User.Current.Id;
                    var decision = context.Request.Form["decision"].ToString();
                    context.Response.Write(Classes.User.Vote(id, uid, decision, false));
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
                else if (Classes.User.IsLogin && Classes.User.Current.Role == 0)
                {
                    if (context.Request["action"].ToString() == "updateconference")
                    {
                        SqlConnection conn = Helper.Conn;
                        SqlCommand cmd = new SqlCommand("UPDATE [Conferences] SET [name]=@name, [cparticipants]=@cparticipants, [cnum]=@cnum, [uparticipants]=@uparticipants, [unum]=@unum, [ratio]=@ratio, [state]=@state, [time]=@time WHERE [id]=@id", conn);
                        var state = context.Request.Form["state"];
                        cmd.Parameters.AddRange(new SqlParameter[]
                        {
                        new SqlParameter("@id", SqlDbType.Int) {Value=context.Request.Form["id"].ToString()},
                        new SqlParameter("@name", SqlDbType.NVarChar, 50) {Value=context.Request.Form["name"].ToString()},
                        new SqlParameter("@cparticipants", SqlDbType.NVarChar) {Value=context.Request.Form["cparticipants"]},
                        new SqlParameter("@cnum", SqlDbType.SmallInt) {Value=context.Request.Form["cnum"]},
                        new SqlParameter("@uparticipants", SqlDbType.NVarChar) {Value=context.Request.Form["uparticipants"]},
                        new SqlParameter("@unum", SqlDbType.SmallInt) {Value=context.Request.Form["unum"]},
                        new SqlParameter("@ratio", SqlDbType.TinyInt) {Value=context.Request.Form["ratio"]},
                        new SqlParameter("@state", SqlDbType.TinyInt) {Value=state},
                        new SqlParameter("@time", SqlDbType.DateTime) {Value=DateTime.UtcNow.AddHours(8)}
                        });
                        conn.Open();
                        try
                        {
                            cmd.ExecuteNonQuery();
                            if (state == "2")
                            {
                                cmd = new SqlCommand("UPDATE [Votes] SET [active]=0 WHERE [conference]=@id", conn);
                                cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = context.Request.Form["id"].ToString() });
                                cmd.ExecuteNonQuery();
                            }
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
                    else if (context.Request["action"].ToString() == "addvote")
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
                            int passvote = Convert.ToInt32(context.Request.Form["passvote"].ToString());
                            if (passvote == 1)
                                passvote = Convert.ToInt32(r["halfmajority"].ToString());
                            else if (passvote == 2)
                                passvote = Convert.ToInt32(r["absmajority"].ToString());
                            var conn = Helper.Conn;
                            cmd = new SqlCommand("INSERT INTO [Votes] ([conference], [title], [notvoted], [nvvote], [cvote], [rvote], [ratio], [halfmajority], [absmajority], [active], [type], [passvote], [time]) VALUES(@conference, @title, @notvoted, @nvvote, 0, 0, @ratio, @halfmajority, @absmajority, 1, @type, @passvote, @time)", conn);
                            cmd.Parameters.AddRange(new SqlParameter[]
                            {
                            new SqlParameter("@conference", SqlDbType.Int) {Value=id},
                            new SqlParameter("@title", SqlDbType.NVarChar, 53) {Value=context.Request.Form["title"].ToString()},
                            new SqlParameter("@notvoted", SqlDbType.VarChar) {Value=notvoted},
                            new SqlParameter("@nvvote", SqlDbType.SmallInt) {Value=Convert.ToInt32(r["totalvote"].ToString())},
                            new SqlParameter("@ratio", SqlDbType.TinyInt) {Value=Convert.ToInt32(r["ratio"].ToString())},
                            new SqlParameter("@halfmajority", SqlDbType.SmallInt) {Value=Convert.ToInt32(r["halfmajority"].ToString())},
                            new SqlParameter("@absmajority", SqlDbType.SmallInt) {Value=Convert.ToInt32(r["absmajority"].ToString())},
                            new SqlParameter("@type", SqlDbType.TinyInt) {Value=Convert.ToInt32(context.Request.Form["type"])},
                            new SqlParameter("@passvote", SqlDbType.Int) {Value=passvote},
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
                    else if (context.Request["action"].ToString() == "deletevote")
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
                    else if (context.Request["action"].ToString() == "deleteconference")
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
                    else if (context.Request["action"].ToString() == "setvotestate")
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
                    else if (context.Request["action"] == "savedelegation")
                    {
                        var conn = Helper.Conn;
                        var cmd = new SqlCommand("INSERT INTO [Delegations] VALUES (@conference, @subject, @object)", conn);
                        cmd.Parameters.AddRange(new SqlParameter[]
                        {
                            new SqlParameter("@conference", SqlDbType.Int) { Value = Convert.ToInt32(context.Request.Form["conference"])},
                            new SqlParameter("@subject", SqlDbType.Int) { Value = Convert.ToInt32(context.Request.Form["subject"])},
                            new SqlParameter("@object", SqlDbType.VarChar) { Value = context.Request.Form["object"]}
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
                    else if (context.Request["action"].ToString() == "deletedelegation")
                    {
                        var conn = Helper.Conn;
                        var cmd = new SqlCommand("DELETE FROM [Delegations] WHERE [id]=@id", conn);
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