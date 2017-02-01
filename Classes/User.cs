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