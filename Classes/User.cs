using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;

namespace SMS_Council.Classes
{
    public class User
    {
        private int id;
        private int role;
        private string name;

        public User (int id, int role, string name)
        {
            this.id = id;
            this.role = role;
            this.name = name;
        }

        public int Id
        {
            get
            {
                return id;
            }
        }

        public int Role
        {
            get
            {
                return role;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public static User Current
        {
            get
            {
                if (HttpContext.Current.Session["User"] != null)
                    return (User)HttpContext.Current.Session["User"];
                else
                    return null;
            }
            set
            {
                HttpContext.Current.Session["User"] = value;
            }
        }
        public static bool IsLogin
        {
            get
            {
                return !(Current == null);
            }
        }

        public static int LogIn(string username, string rawpassword)
        {
            SqlCommand cmd = new SqlCommand("SELECT [id], [name], [password], [role] FROM [Users] WHERE [username] = @username AND [activated] = 1");
            cmd.Parameters.Add(new SqlParameter("@username",SqlDbType.VarChar, 30) { Value = username});
            DataTable dt = Helper.RetrieveDataTable(cmd);
            if (dt.Rows.Count > 0)
            {
                string password = "";
                string rightpassword = dt.Rows[0]["password"].ToString();
                MD5 md5 = MD5.Create();
                byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(rawpassword));
                for (int i = 0; i < s.Length; i++)
                {
                    password = password + s[i].ToString("x2");
                }
                if (password == rightpassword)
                {
                    Current = new User(Convert.ToInt32(dt.Rows[0]["id"].ToString()),Convert.ToInt32(dt.Rows[0]["role"].ToString()),dt.Rows[0]["name"].ToString());
                    return 1;
                }
                else
                {
                    return -2;
                }
            }
            else
            {
                return -1;
            }
        }

        public static void LogOut()
        {
            if (IsLogin)
            {
                Current = null;
            }
        }

        public static int ChangePassword(string oldpassword, string newpassword)
        {
            if (IsLogin)
            {
                SqlCommand cmd = new SqlCommand("SELECT [password] FROM [Users] WHERE [id] = @id");
                cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = Current.id });
                DataTable dt = Helper.RetrieveDataTable(cmd);
                if (dt.Rows.Count > 0 && MD5Encrypt(oldpassword) == dt.Rows[0]["password"].ToString())
                {
                    SqlConnection conn = Helper.Conn;
                    cmd = new SqlCommand("UPDATE [Users] SET [password]=@password WHERE [id]=@id", conn);
                    cmd.Parameters.AddRange(new SqlParameter[]
                    {
                        new SqlParameter("@password",SqlDbType.Char,32) { Value = MD5Encrypt(newpassword) },
                        new SqlParameter("@id", SqlDbType.Int) { Value = Current.id } 
                    });
                    conn.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        return 1;
                    }
                    catch
                    {
                        return -10;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                else
                    return -2;
            }
            else
                return -1;
        }

        public static int Vote(int id, int uid, string decision, bool delegation)
        {
            SqlCommand cmd = new SqlCommand("SELECT [notvoted], [consented], [rejected], [abstained], [nvvote], [cvote], [rvote], [avote], [ratio], [active], [conference] FROM [Votes] WHERE [id]=@id");
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = id });
            DataTable dt = Helper.RetrieveDataTable(cmd);
            var r = dt.Rows[0];
            List<int> notvoted = r["notvoted"].ToString() == "" ? new List<int>() : r["notvoted"].ToString().Split(',').Select(int.Parse).ToList();
            var role = Current.Role;
            if (role == 0 || delegation)
            {
                cmd = new SqlCommand("SELECT [role] FROM [Users] WHERE [id]=@id");
                cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.Int) { Value = uid });
                role = Convert.ToInt32(Helper.RetrieveDataTable(cmd).Rows[0][0].ToString());
            }
            if (r["active"].ToString() == "True" && notvoted.Contains(uid))
            {
                SqlConnection conn = Helper.Conn;
                var conference = Convert.ToInt32(r["conference"].ToString());
                if (decision == "C")
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
                        if (role == 2 && !delegation)
                        {
                            conn.Close();
                            cmd = new SqlCommand("SELECT [object] FROM [Delegations] WHERE [subject]=@uid AND [conference]=@id");
                            cmd.Parameters.AddRange(new SqlParameter[] {
                                new SqlParameter("@id", SqlDbType.Int) { Value = conference },
                                new SqlParameter("@uid", SqlDbType.Int) { Value = uid }
                            });
                            dt = Helper.RetrieveDataTable(cmd);
                            foreach (DataRow row in dt.Rows)
                            {
                                int[] obj = row[0].ToString().Split(',').Select(int.Parse).ToArray();
                                foreach (int duid in obj)
                                {
                                    Vote(id, duid, decision, true);
                                }
                            }
                        }
                        return 1;
                    }
                    catch
                    {
                        return -10;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                else if (decision == "R")
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
                        if (role == 2 && !delegation)
                        {
                            conn.Close();
                            cmd = new SqlCommand("SELECT [object] FROM [Delegations] WHERE [subject]=@uid AND [conference]=@id");
                            cmd.Parameters.AddRange(new SqlParameter[] {
                                new SqlParameter("@id", SqlDbType.Int) { Value = conference },
                                new SqlParameter("@uid", SqlDbType.Int) { Value = uid }
                            });
                            dt = Helper.RetrieveDataTable(cmd);
                            foreach (DataRow row in dt.Rows)
                            {
                                int[] obj = row[0].ToString().Split(',').Select(int.Parse).ToArray();
                                foreach (int duid in obj)
                                {
                                    Vote(id, duid, decision, true);
                                }
                            }
                        }
                        return 1;
                    }
                    catch
                    {
                        return -10;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                else if (decision == "A")
                {
                    List<int> abstained = r["abstained"].ToString() == "" ? new List<int>() : r["abstained"].ToString().Split(',').Select(int.Parse).ToList();
                    abstained.Add(uid);
                    notvoted.Remove(uid);
                    int vote = role == 2 ? Convert.ToInt32(r["ratio"].ToString()) : 1;
                    int nvvote = Convert.ToInt32(r["nvvote"].ToString()) - vote;
                    int avote = Convert.ToInt32(r["avote"].ToString()) + vote;
                    cmd = new SqlCommand("UPDATE [Votes] SET [notvoted]=@notvoted, [abstained]=@abstained, [nvvote]=@nvvote, [avote]=@avote WHERE [id]=@id", conn);
                    cmd.Parameters.AddRange(new SqlParameter[] {
                                new SqlParameter("@notvoted", SqlDbType.VarChar){ Value = string.Join(",",notvoted.Select(x => x.ToString()).ToArray()) },
                                new SqlParameter("@abstained", SqlDbType.VarChar){ Value = string.Join(",",abstained.Select(x => x.ToString()).ToArray()) },
                                new SqlParameter("@nvvote", SqlDbType.SmallInt){ Value = nvvote },
                                new SqlParameter("@avote", SqlDbType.SmallInt){ Value = avote },
                                new SqlParameter("@id", SqlDbType.Int) { Value = id }
                            });
                    conn.Open();
                    try
                    {
                        cmd.ExecuteNonQuery();
                        if (role == 2 && !delegation)
                        {
                            conn.Close();
                            cmd = new SqlCommand("SELECT [object] FROM [Delegations] WHERE [subject]=@uid AND [conference]=@id");
                            cmd.Parameters.AddRange(new SqlParameter[] {
                                new SqlParameter("@id", SqlDbType.Int) { Value = conference },
                                new SqlParameter("@uid", SqlDbType.Int) { Value = uid }
                            });
                            dt = Helper.RetrieveDataTable(cmd);
                            foreach (DataRow row in dt.Rows)
                            {
                                int[] obj = row[0].ToString().Split(',').Select(int.Parse).ToArray();
                                foreach (int duid in obj)
                                {
                                    Vote(id, duid, decision, true);
                                }
                            }
                        }
                        return 1;
                    }
                    catch
                    {
                        return -10;
                    }
                    finally
                    {
                        conn.Close();
                    }
                }
                else
                {
                    return -2;
                }
            }
            else
            {
                return -2;
            }
        }

        private static string MD5Encrypt(string str)
        {
            string result = "";
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            for (int i = 0; i < s.Length; i++)
            {
                result = result + s[i].ToString("x2");
            }
            return result;
        }
    }
}