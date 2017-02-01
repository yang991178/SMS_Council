<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConferenceDetail.aspx.cs" Inherits="SMS_Council.ConferenceDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <base href="/" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=0" />
    <title><% Response.Write(dr["name"].ToString()); %> - 会议 - 深圳中学议事会</title>
    <link href="/favicon.ico" rel="shortcut icon" />
    <link href="/css/materialize.min.css" rel="stylesheet" />
    <link href="/css/general.css" rel="stylesheet" />
    <script src="/js/jquery-1.11.1.min.js"></script>
    <script src="/js/materialize.min.js"></script>
    <% Response.Write(SMS_Council.Classes.Helper.LoadAngular("conferencep")); %>
    <style>
        main {
            background:#eee;
            overflow:hidden;
            min-height:100%;
        }
        .container {
            max-width:960px;
        }
        #header {
            padding:56px 0 345px;
            overflow:hidden;
        }
        .timeline {
            margin-top:-300px;
            overflow:hidden;
            min-height:450px;
            background:#fff;
            margin-bottom:40px;
            padding:50px 50px 0;
            display:flex;
            align-items:stretch;
            overflow:hidden;
        }
        .votecontainer {
            margin-top:-300px;
            overflow:hidden;
            min-height:356px;
            padding:356px 0 0;
            position:relative;
            margin-bottom:40px;
        }
        .axis {
            width:40px;
            flex-basis:40px;
            min-height:100%;
            position:relative;
        }
        .axis > .start {
            width:40px;
            height:40px;
            position:absolute;
            left:50%;
            top:0;
            margin-left:-20px;
            border-radius:50%;
            border:5px solid #80cbc4;
            background:#fff;
        }
        .axis > .line {
            height:100%;
            position:absolute;
            left:50%;
            margin-left:-3px;
            border-left:6px solid #80cbc4;
        }
        .timeline > .contents {
            flex-grow:1;
            padding-top:70px;
            position:relative;
        }
        .votecard {
            margin-bottom:20px;
        }
        .votecard > .card {
            width:94%;
            margin:-46px 0 0 6%;
        }
        .votecard:before {
            content:" ";
            display:block;
            width:90%;
            height:0;
            border-top:6px dotted #80cbc4;
            margin-top:40px;
            margin-left:-15px;
        }
        .votecard > .time {
            margin:2px 0;
            padding-bottom:25px;
            text-align:right;
            color:#666;
            font-size:14px;
            overflow:hidden;
        }
        h5 {
            line-height:150%;
            font-size:1.55rem;
        }
        #refresh {
            position:absolute;
            right:0;
            top:0;
            border:1px solid #26a69a;
            padding:0 15px;
        }
        #refresh-icon {
            margin:-4px 10px 0 0;
            vertical-align:middle;
        }
        #refresh.active>#refresh-icon {
            animation:rotating linear 1.5s infinite;
        }
        @keyframes rotating {
            0% {transform:rotate(0deg)}
            100% {transform:rotate(360deg)}
        }
        .card > .progress {
            position:absolute;
            top:0;
            left:0;
            margin:0;
        }
        .progress .determinate {
            float:left;
            position:relative;
            height:100%;
        }
        .votedetail {
            min-width:240px;
            width:50%;
            position:absolute;
            left:0;
            right:0;
            margin:auto;
            top:115px;
            background:rgba(255,255,255,0.75);
            padding:6px 0 26px;
        }
        .votedetail > h3 {            
            font-weight:300 !important;
        }
        .votedetail > h3 > span {
            position:relative;
        }
        .votedetail > h3 >span:before {
            position:absolute;
            font-size:12px;
            line-height:12px;
            width:30px;
            left:50%;
            margin-left:-15px;
            bottom:-12px;
        }
        .votedetail > h3 > span.green-text:before {
            content:"赞成";
        }
        .votedetail > h3 > span.red-text:before {
            content:"反对";
        }
        .votedetail > h3 > span.grey-text:before {
            content:"未投";
        }
        .slash {
            font-size:70%;
            vertical-align:top;
            margin:0 8px;
        }
        .collapsible-body {
            display: none;
            border-bottom: 1px solid #ddd;
            box-sizing: border-box;
            padding: 1.5rem;
            background:#fafafa;
        }
        .fixed-action-btn.horizontal ul li {
            margin:0 15px 0 0;
        }
        .mobile-scroll {
            padding-left: 62.75px;
        }
        @media (max-width:600px){
            #header>h5 {
                margin:-24px 0 30px;
            }
            #header>h4 {
                clear:both;
                line-height:150%;
            }
            .timeline {
                padding:15px 15px 0;
            }
            h5 {
                font-size:1.24rem;
            }
            .axis > .start {
                width:30px;
                height:30px;
                margin-left:-15px;
            }
            .mobile-scroll {
                padding-left:20px;
                overflow-x:scroll;
                overflow-y:hidden;
            }
        }
    </style>
</head>
<body>
    <% Response.Write(SMS_Council.Classes.Helper.WriteMenu()); %>
    <main>
        <div id="header" class="teal lighten-1">
            <h5 class="container"><span class="new badge white teal-text text-lighten-1" data-badge-caption="<% Response.Write(dr["state"].ToString() == "1" ? "进行中" : (dr["state"].ToString() == "2" ? "已结束" : "未开始")); %>"></span></h5>
            <h4 class="container light white-text"><% Response.Write(dr["name"].ToString()); %><br /><small class="teal-text text-lighten-5"><small><% Response.Write(dr["cnum"].ToString()); %> 班级议委 · <% Response.Write(dr["unum"].ToString()); %> 单元议委 · <% Response.Write(dr["totalvote"].ToString()); %> 有效票</small></small></h4>
        </div>
        <conference>
            <div class="container z-depth-3 timeline" style="position:relative">
                <div class="preloader-wrapper active" style="position:absolute;left:50%;margin:-24px 0 0 -24px;top:50%;">
                    <div class="spinner-layer spinner-green-only">
                        <div class="circle-clipper left"><div class="circle"></div></div>
                        <div class="gap-patch"><div class="circle"></div></div>
                        <div class="circle-clipper right"><div class="circle"></div></div>
                    </div>
                </div>
            </div>
        </conference>
    </main>
    <script>
        $("#show-menu").sideNav()
        $(document).ready(function () {
            $('.collapsible').collapsible();
        });
    </script>
</body>
</html>
