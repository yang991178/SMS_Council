<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserManage.aspx.cs" Inherits="SMS_Council.UserManage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <base href="/" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=0" />
    <title>用户管理 - 深圳中学议事会</title>
    <link href="/favicon.ico" rel="shortcut icon" />
    <link href="/css/materialize.min.css" rel="stylesheet" />
    <link href="/css/general.css" rel="stylesheet" />
    <script src="/js/jquery-1.11.1.min.js"></script>
    <script src="/js/materialize.min.js"></script>
    <% Response.Write(SMS_Council.Classes.Helper.LoadAngular("adminusermanage")); %>
    <style>
        body {
            overflow-y:scroll !important;overflow-x:hidden
        }
        .modal {
            margin-bottom:40px
        }
    </style>
</head>
<body>
    <% Response.Write(SMS_Council.Classes.Helper.WriteMenu()); %>
    <main>
        <admin-user-manage></admin-user-manage>    
    </main>
 
    <script>
        $("#show-menu").sideNav()
    </script>
</body>
</html>
