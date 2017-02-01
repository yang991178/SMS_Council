<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Contacts.aspx.cs" Inherits="_Contacts" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=0" />
    <title>议委 - 深圳中学议事会</title>
    <link href="favicon.ico" rel="shortcut icon" />
    <link href="css/materialize.min.css" rel="stylesheet" />
    <link href="css/general.css" rel="stylesheet" />
    <script src="js/jquery-1.11.1.min.js"></script>
    <script src="js/materialize.min.js"></script>
    <% Response.Write(SMS_Council.Classes.Helper.LoadAngular("contacts")); %>
<%--    <style>@font-face{font-family:'Material Icons';font-style:normal;font-weight:400;src:local('Material Icons'),local('MaterialIcons-Regular'),url(fonts/mi.woff2) format('woff2')}.material-icons{font-family:'Material Icons';font-weight:normal;font-style:normal;font-size:24px;line-height:1;letter-spacing:normal;text-transform:none;display:inline-block;white-space:nowrap;word-wrap:normal;direction:ltr;-webkit-font-feature-settings:'liga';-webkit-font-smoothing:antialiased}</style>--%>
    <style>
        #main {
            max-width:1200px
        }
        main, #main {
            height:100%;
            margin:0;
            position:relative
        }
        .cdetail {
            height:100%;
        }
        .collection a.collection-item  {
            color:#808080
        }
        h5 {
            font-size:1.4rem
        }
        .contactlist {
            padding:0 !important;
            overflow-y:scroll;
            height:100%;
            background:#fff
        }
        .c-overlay {
            display:none;
        }
        contacts {
            display:block
        }
        .cdetail>div>p {
            font-size: 12px;
        }
        .cdetail>div>p>span.h5{
            font-size:18px;
        }
        .close {
            display:none;
        }
        @keyframes overlay {
            0% {
                opacity:0
            }
            100% {
                opacity:1
            }
        }
        @keyframes cdetail {
            0% {
                bottom:-35%;
            }
            100% {
                bottom:0;
            }
        }
        @keyframes overlayr {
            0% {
                opacity:1
            }
            100% {
                opacity:0
            }
        }
        @keyframes cdetailr {
            0% {
                bottom:0;
            }
            100% {
                bottom:-35%;
            }
        }
        @media (max-width:600px){
            .c-overlay {
                position:absolute;
                left:0;
                top:0;
                width:100%;
                height:100%;
                background:rgba(0, 0, 0, 0.50);
                z-index:998;
                transition:opacity ease-in-out 0.4s;
            }
            .cdetail {
                position:fixed;
                bottom:0;
                left:0;
                height:35%;
                display:none;
                z-index:999;
                overflow-y:scroll;
            }
            .c-overlay.on {
                display:block;
                animation:overlay 0.3s linear;
            }
            .c-overlay.off {
                display:block;
                animation:overlayr 0.3s linear forwards;
            }
            .cdetail.on {
                display:block;
                animation:cdetail 0.4s ease-in-out;
            }
            .cdetail.off {
                display:block;
                animation:cdetailr 0.3s ease-in-out forwards;
            }
            .close {
                display:block;
                position:absolute;
                right:20px;
                top:20px;
                font-size:18px;
                font-weight:700;
                color:#e0f2f1;
                cursor:pointer;
            }
        }
    </style>
</head>
<body>
    <% Response.Write(SMS_Council.Classes.Helper.WriteMenu()); %>
    <main class="teal lighten-1">
        <contacts id="main" class="row">
            <%--<div class="col s12 m4 l4 contactlist">
                <h5 class="text-darken-1 grey-text center-align">班级议委</h5>
                <div class="collection">
                    <a href="#" class="collection-item">庄桂灏<span class="new badge" data-badge-caption="班">1</span></a>
                    <a href="#" class="collection-item">叶晓欣（候补）<span class="new badge" data-badge-caption="班">1</span></a>
                </div>
            </div>
            <div class="c-overlay"></div>
            <div class="col s12 m8 l8 teal lighten-1 cdetail">

            </div>--%>
        </contacts>
    </main>
    <script>
        $("#show-menu").sideNav()
    </script>
</body>
</html>
