<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Conferences.aspx.cs" Inherits="SMS_Council.Conference" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=0" />
    <title>会议 - 深圳中学议事会</title>
    <link href="favicon.ico" rel="shortcut icon" />
    <link href="css/materialize.min.css" rel="stylesheet" />
    <link href="css/general.css" rel="stylesheet" />
    <script src="js/jquery-1.11.1.min.js"></script>
    <script src="js/materialize.min.js"></script>
<%--    <style>@font-face{font-family:'Material Icons';font-style:normal;font-weight:400;src:local('Material Icons'),local('MaterialIcons-Regular'),url(fonts/mi.woff2) format('woff2')}.material-icons{font-family:'Material Icons';font-weight:normal;font-style:normal;font-size:24px;line-height:1;letter-spacing:normal;text-transform:none;display:inline-block;white-space:nowrap;word-wrap:normal;direction:ltr;-webkit-font-feature-settings:'liga';-webkit-font-smoothing:antialiased}</style>--%>
    <style>
        #header {
            height:200px;
            overflow:hidden;
            text-align:center;
            margin-bottom:50px;
        }
        h5 {
            font-size:1.5rem;
            line-height:170%
        }
        .card-action>p {
            margin:0;
            font-size:12px;
        }
        .card .card-action a:not(.btn):not(.btn-large):not(.btn-floating) {
            color:#4db6ac;
            font-size:14px;
        }
        .card .card-action a:not(.btn):not(.btn-large):not(.btn-floating):hover {
            color:#80cbc4
        }
        @media (min-width: 993px) and (max-width:1280px){
             .container {
                 width:90%
            }
        }
        @media (max-width:600px) {
            .card-image {
                display:none;
            }
        }
    </style>
</head>
<body>
    <% Response.Write(SMS_Council.Classes.Helper.WriteMenu()); %>
    <main>
        <div id="header" class="teal lighten-1 valign-wrapper">
            <h4 class="valign light white-text" style="width:100%">会议列表<br /><small><small><small><i>Make SMS Great Again.</i></small></small></small></h4>
        </div>
        <div class="container" style="max-width:960px;">
            <div class="card horizontal">
                <div class="card-image">
                    <img src="img/latest-conference.png" />
                    <span class="card-title">最新会议</span>
                </div>
                <div class="card-stacked">
                    <div class="card-content">
                        <h5><% Response.Write(latest["name"].ToString()); %></h5>
                    </div>
                    <div class="card-action">
                        <p><% Response.Write(latest["state"].ToString() == "1" ? "开始" : "结束"); %>于<% Response.Write(DateTime.Parse(latest["time"].ToString()).ToString("yyyy年M月d日HH:mm")); %> · <% Response.Write(latest["totalvote"].ToString()); %>有效票<a class="right" href="conference/<% Response.Write(latest["id"].ToString()); %>"><% Response.Write(latest["state"].ToString() == "1" ? "进入" : "详情"); %></a></p>
                    </div>
                </div>
            </div>

            <div class="row">
                <asp:Repeater ID="repeater" runat="server">
                    <ItemTemplate>
                        <div class="col s12 m6 l6">
                            <div class="card">
                                <div class="card-content">
                                    <h5> <%#DataBinder.Eval(Container.DataItem,"name")%></h5>
                                </div>
                                <div class="card-action">
                                    <p><%#DataBinder.Eval(Container.DataItem,"state").ToString() == "1" ? "开始" : "结束" %>于<%#DataBinder.Eval(Container.DataItem,"time", "{0:yyyy年M月d日HH:mm}")%> · <%#DataBinder.Eval(Container.DataItem,"totalvote")%>有效票<a class="right" href="conference/<%#DataBinder.Eval(Container.DataItem,"id")%>"><%#DataBinder.Eval(Container.DataItem,"state").ToString() == "1" ? "进入" : "详情" %></a></p>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>
    </main>
    <script>
        $("#show-menu").sideNav()
    </script>
</body>
</html>

