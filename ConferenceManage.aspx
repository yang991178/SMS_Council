<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConferenceManage.aspx.cs" Inherits="SMS_Council.ConferenceManage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <base href="/" />
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <meta name="viewport" content="width=device-width, initial-scale=1, user-scalable=0" />
    <title>会议管理 - 深圳中学议事会</title>
    <link href="/favicon.ico" rel="shortcut icon" />
    <link href="/css/materialize.min.css" rel="stylesheet" />
    <link href="/css/general.css" rel="stylesheet" />
    <script src="/js/jquery-1.11.1.min.js"></script>
    <script src="/js/materialize.min.js"></script>
    <% Response.Write(SMS_Council.Classes.Helper.LoadAngular("conferencemanage")); %>
    <style>
        .header {
            height:100px;
            overflow:hidden;
            text-align:center;
            margin-bottom:20px;
        }
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
        <div class="header teal valign-wrapper">
            <h4 class="valign white-text" style="width:100%;font-weight:300">会议管理</h4>
        </div>
        <conference-manage></conference-manage>
        <%--<div class="container">
            <table class="bordered">
                <thead><tr><th>id</th><th>会议名称</th><th>状态</th><th></th></tr></thead>
                <tbody>
                    <tr><td>2</td><td>《手机管理办法》第四次会议</td><td>进行中</td><td><a class="modal-trigger" href="#modal">修改</a></td></tr>
                    <tr><td>1</td><td>《手机管理办法》第三次会议</td><td>已结束</td><td><a href="#">修改</a></td></tr>
                </tbody>
            </table>
        </div>
        <div id="modal" class="modal" style="overflow:visible;max-height:none;position:absolute;">
            <div class="modal-content">
                <h5 class="teal-text">编辑会议</h5><br />
                <div class="row">
                    <div class="col s12 input-field">
                        <input id="name" type="text" class="validate" />
                        <label for="name">会议名称</label>
                    </div>
                      <div class="input-field col s12 m6"> 
                        <select multiple="multiple">
                          <option value="" disabled="disabled" selected="selected">选择班级议委</option>
                          <option value="1">Option 1</option>
                          <option value="2">Option 2</option>
                          <option value="3">Option 3</option>
                        </select>
                         <label>班级议委</label>
                      </div>
                      <div class="input-field col s12 m6"> 
                        <select multiple="multiple">
                          <option value="" disabled="disabled" selected="selected">选择单元议委</option>
                          <option value="1">Option 1</option>
                          <option value="2">Option 2</option>
                          <option value="3">Option 3</option>
                        </select>
                         <label>单元议委</label>
                      </div>
                    <div class="input-field col s6">
                      <input placeholder="输入1至256之间的整数" title="单元议委票权" pattern="^[0-9]*$" id="ratio" type="text" class="validate" />
                      <label for="ratio">单元议委票权</label>
                    </div>
                    <div class="col s6">
                      <div class="switch">
                        <label style="line-height: 2.6">
                          会议状态<br />已结束
                          <input checked="checked" type="checkbox" />
                          <span class="lever"></span>
                          进行中
                        </label>
                      </div>
                    </div>
                </div>
                <h5 class="teal-text">管理表决<button class="btn waves-effect waves-light right" style="font-size:16px">添加</button></h5>
                <table class="bordered">
                    <thead><tr><th>标题</th><th>操作</th><th></th></tr></thead>
                    <tbody>
                        <tr><td>通过《手机管理办法》第三章内容</td><td><a href="#">管理</a></td><td><a href="#" class="red-text">删除</a></td></tr>
                        <tr><td>通过《手机管理办法》第四章内容</td><td><a href="#">管理</a></td><td><a href="#" class="red-text">删除</a></td></tr>
                    </tbody>
                </table>
            </div>
            <div class="modal-footer">
                <a href="#!" class="modal-action modal-close waves-effect waves-green btn-flat teal-text">保存</a>
                <a href="#!" class="modal-action modal-close waves-effect waves-ripple btn-flat">关闭</a>
            </div>
        </div>--%>
    </main>
 
    <script>
        $("#show-menu").sideNav()
    </script>
</body>
</html>
