<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=0" />
    <title>深圳中学议事会</title>
    <link href="favicon.ico" rel="shortcut icon" />
    <link href="css/materialize.min.css" rel="stylesheet" />
    <link href="css/general.css" rel="stylesheet" />
    <script src="js/jquery-1.11.1.min.js"></script>
    <script src="js/materialize.min.js"></script>
    <% 
        var package = SMS_Council.Classes.User.IsLogin ? "usermanage" : "login";
        Response.Write(SMS_Council.Classes.Helper.LoadAngular(package));
    %>
<%--    <style>@font-face{font-family:'Material Icons';font-style:normal;font-weight:400;src:local('Material Icons'),local('MaterialIcons-Regular'),url(fonts/mi.woff2) format('woff2')}.material-icons{font-family:'Material Icons';font-weight:normal;font-style:normal;font-size:24px;line-height:1;letter-spacing:normal;text-transform:none;display:inline-block;white-space:nowrap;word-wrap:normal;direction:ltr;-webkit-font-feature-settings:'liga';-webkit-font-smoothing:antialiased}</style>--%>
    <style>
        #header {
            height:400px;
            overflow:hidden
        }
        #login {
            height:370px;
            margin:-200px auto 10px;
            background:#fff;
            width:340px;
            max-width:90%;
            overflow:hidden;
            position:relative;
        }
        #warning {
            margin: 15px auto;
            width:340px;
            max-width:90%;
            padding: 7px;
            font-size:18px;
            fill:#fff;
        }
    </style>
</head>
<body>
    <% Response.Write(SMS_Council.Classes.Helper.WriteMenu()); %>
    <main>
        <%
            if (!SMS_Council.Classes.User.IsLogin)
            {
                Response.Write(@"        <login>
            <div id=""header"" class=""teal lighten-1"">
            <div class=""container"">
                <div class=""row center"" style=""margin-top:120px"">
                    <h4 class=""light white-text"">&nbsp;&nbsp;</h4>
                </div>
            </div>
        </div>
        <div class=""container"">
            <div id=""login"" class=""z-depth-4"">
              <div class=""preloader-wrapper active"" style=""position:absolute;left:50%;margin:-24px 0 0 -24px;top:50%;"">
                <div class=""spinner-layer spinner-green-only"">
                  <div class=""circle-clipper left"">
                    <div class=""circle""></div>
                  </div><div class=""gap-patch"">
                    <div class=""circle""></div>
                  </div><div class=""circle-clipper right"">
                    <div class=""circle""></div>
                  </div>
                </div>
              </div>
            </div>
        </div>
        </login>");
            }
            else
            {
                int h = DateTime.UtcNow.Hour + 8;
                var greeting = (h >= 5 && h < 12 ? "早上好" : (h < 5 || h >= 18 ? "晚上好" : "下午好")) + "，" + SMS_Council.Classes.User.Current.Name + "！";
                Response.Write(@"<div id=""header"" class=""teal lighten-1"">
            <div class=""container"">
                <div class=""row center"" style=""margin-top:120px"">
                    <h4 class=""light white-text"">"+ greeting + @"</h4>
                </div>
            </div>
        </div>
        <manage>
        <div class=""container"">
            <div id=""login"" class=""z-depth-4"">
              <div class=""preloader-wrapper active"" style=""position:absolute;left:50%;margin:-24px 0 0 -24px;top:50%;"">
                <div class=""spinner-layer spinner-green-only"">
                  <div class=""circle-clipper left"">
                    <div class=""circle""></div>
                  </div><div class=""gap-patch"">
                    <div class=""circle""></div>
                  </div><div class=""circle-clipper right"">
                    <div class=""circle""></div>
                  </div>
                </div>
              </div>
            </div>
        </div>
        </manage>");
            }
        %>        
    </main>
    <script>
        $("#show-menu").sideNav()
    </script>
</body>
</html>
