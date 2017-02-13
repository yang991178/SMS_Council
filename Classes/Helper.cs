using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SMS_Council.Classes
{
    public static class Helper
    {
        public static string ConnStr = System.Configuration.ConfigurationManager.ConnectionStrings["CouncilConnectionString"].ToString();

        public static SqlConnection Conn = new SqlConnection(ConnStr);

        public static string WriteMenu()
        {
            return @"    <nav style=""position:fixed;top:0;left:0;z-index:1000"">
        <a style=""color:#808080;margin-left:0;margin-left:18.75px;margin-right:0"" href=""#"" data-activates=""nav-mobile"" id=""show-menu"" class=""button-collapse top-nav waves-effect waves-ripple waves-circle""><svg width=""24px"" height=""35px"" viewBox=""0 0 48 48""><path d=""M6 36h36v-4H6v4zm0-10h36v-4H6v4zm0-14v4h36v-4H6z""></path></svg></a>
        <a style=""color:#808080;margin-left:0;display:block;margin-left:18.75px"" data-activates=""nav-mobile"" class=""button-collapse top-nav hide-on-med-and-down""><svg width=""24px"" height=""35px"" viewBox=""0 0 48 48""><path d=""M6 36h36v-4H6v4zm0-10h36v-4H6v4zm0-14v4h36v-4H6z""></path></svg></a>
        <div class=""nav-wrapper"">
            <div class=""left""><img src=""/img/logo.png"" /></div>
            <div class=""left"" style=""font-size:20px;font-weight:700"">议事会</div>
        </div>
    </nav>
    <header>
        <ul id=""nav-mobile"" class=""side-nav fixed grey lighten-5"">
            <li class=""bold no-padding""><a href=""/"" class=""waves-effect waves-ripple""><svg width=""24px"" height=""24px"" viewBox=""0 0 48 48""><path d=""M20 40V28h8v12h10V24h6L24 6 4 24h6v16z""></path></svg>首页</a></li>
            <li><div class=""divider""></div></li>
            <li class=""bold""><a href=""/conference"" class=""waves-effect waves-ripple""><svg width=""24px"" height=""24px"" viewBox=""0 0 48 48"" class=""doawgc""><path d=""M43.98 8c0-2.21-1.77-4-3.98-4H8C5.79 4 4 5.79 4 8v24c0 2.21 1.79 4 4 4h28l8 8-.02-36zM36 28H12v-4h24v4zm0-6H12v-4h24v4zm0-6H12v-4h24v4z""></path></svg>会议</a></li>
            " + LatestVote +@"                        <li><div class=""divider""></div></li>
            <li class=""bold""><a href=""/contacts"" class=""waves-effect waves-ripple""><svg width=""24px"" height=""24px"" viewBox=""0 0 48 48""><path d=""M24 24c4.42 0 8-3.59 8-8 0-4.42-3.58-8-8-8s-8 3.58-8 8c0 4.41 3.58 8 8 8zm0 4c-5.33 0-16 2.67-16 8v4h32v-4c0-5.33-10.67-8-16-8z""></path></svg>议委</a></li>" + (User.IsLogin && User.Current.Role == 0 ? @"<li><div class=""divider""></div></li><li class=""bold""><a href=""/conference/manage"" class=""waves-effect waves-ripple""><svg width=""24px"" height=""24px"" viewBox=""0 0 24 24""><path d=""M19.43 12.98c.04-.32.07-.64.07-.98s-.03-.66-.07-.98l2.11-1.65c.19-.15.24-.42.12-.64l-2-3.46c-.12-.22-.39-.3-.61-.22l-2.49 1c-.52-.4-1.08-.73-1.69-.98l-.38-2.65C14.46 2.18 14.25 2 14 2h-4c-.25 0-.46.18-.49.42l-.38 2.65c-.61.25-1.17.59-1.69.98l-2.49-1c-.23-.09-.49 0-.61.22l-2 3.46c-.13.22-.07.49.12.64l2.11 1.65c-.04.32-.07.65-.07.98s.03.66.07.98l-2.11 1.65c-.19.15-.24.42-.12.64l2 3.46c.12.22.39.3.61.22l2.49-1c.52.4 1.08.73 1.69.98l.38 2.65c.03.24.24.42.49.42h4c.25 0 .46-.18.49-.42l.38-2.65c.61-.25 1.17-.59 1.69-.98l2.49 1c.23.09.49 0 .61-.22l2-3.46c.12-.22.07-.49-.12-.64l-2.11-1.65zM12 15.5c-1.93 0-3.5-1.57-3.5-3.5s1.57-3.5 3.5-3.5 3.5 1.57 3.5 3.5-1.57 3.5-3.5 3.5z""></path></svg>会议管理</a></li>" : "") + @"        
    <li style=""position:absolute;bottom:40px;left:0;width: 100%;background:rgba(51,51,51,0.08)""><p style=""margin:0 30px 20px;font-size:12px"">在GPL-3.0协议下<a target=""_blank"" href=""https://github.com/yang991178/SMS_Council"" style=""display:inline;   padding:0;font-size:12px;text-decoration: underline"">开放源代码</a>.</p></li></ul></header>";
        }

        public static string LoadAngular(string package)
        {
            return @"<script src=""/node_modules/core-js/client/shim.min.js""></script>
     <script src=""/node_modules/zone.js/dist/zone.min.js""></script>
     <script src=""/node_modules/systemjs/dist/system.js""></script>
     <script src=""/systemjs.config.js""></script>
     <script>
         System.import('" + package + @"').catch(function(err){ console.error(err); });
     </script>";
        }

        private static string LatestVote
        {
            get
            {
                var dt = RetrieveDataTable(new SqlCommand("SELECT TOP 1 [id] FROM [Conferences] WHERE [state] = 1 ORDER BY [time] DESC"));
                if (dt.Rows.Count > 0)
                {
                    var id = dt.Rows[0]["id"].ToString();
                    dt = RetrieveDataTable(new SqlCommand("SELECT TOP 1 [id] FROM [Votes] WHERE [conference] = '" + id + "' AND [active] = 1 ORDER BY [time]"));
                    if (dt.Rows.Count > 0)
                    {
                        return @"<li class=""bold""><a href=""/conference/" + id + @"/vote/0"" class=""waves-effect waves-ripple""><svg width=""24px"" height=""24px"" viewBox=""0 0 48 48""><path d=""M14 38h20V8H14v30zM4 34h8V12H4v22zm32-22v22h8V12h-8z""></path></svg>表决<span class=""new badge"" data-badge-caption=""正在进行"" style=""font-size:10px""></span></a></li>";
                    }
                    else
                        return @"<li class=""bold""><a><svg width=""24px"" height=""24px"" viewBox=""0 0 48 48""><path d=""M14 38h20V8H14v30zM4 34h8V12H4v22zm32-22v22h8V12h-8z""></path></svg>表决<span class=""badge"" data-badge-caption=""无表决"" style=""font-size:10px""></span></a></li>";
                }
                else
                    return @"<li class=""bold""><a><svg width=""24px"" height=""24px"" viewBox=""0 0 48 48""><path d=""M14 38h20V8H14v30zM4 34h8V12H4v22zm32-22v22h8V12h-8z""></path></svg>表决<span class=""badge"" data-badge-caption=""无表决"" style=""font-size:10px""></span></a></li>";
            }
        }

        public static DataTable RetrieveDataTable(SqlCommand cmd)
        {
            cmd.Connection = new SqlConnection(ConnStr);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return (dt);
        }

        public static string DataTableToJson(DataTable dt)
        {
            return JsonConvert.SerializeObject(dt, new DataTableConverter());
        }
    }
}