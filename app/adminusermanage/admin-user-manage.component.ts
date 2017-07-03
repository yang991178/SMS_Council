import { Component, OnInit, AfterViewInit, ViewChild } from '@angular/core';

import { User } from '../user.js';
import { UserService } from '../user.service.js';

declare var $: any;
declare var parseInt: any;

@Component({
    selector: 'admin-user-manage',
    template: `<nav class="nav-extended teal" style="overflow:hidden;">
    <div class="nav-content">
      <ul class="tabs tabs-transparent">
        <li class="tab"><a (click)="showCurrent()" class="active" href="javascript:void(0)">当前议委</a></li>
        <li class="tab"><a (click)="showRetired()" href="javascript:void(0)">退休议委</a></li>
      </ul>
    </div>
  </nav>
        <div *ngIf="cReady && current == 0" class="container">
            <h5><button (click)="initTransitionModal()" class="btn btn-flat teal-text waves-effect waves-ripple right" style="font-size:16px;border:1px solid #009688">换届助手</button></h5>
            <table class="bordered">
                <thead><tr><th>姓名</th><th>用户名</th><th>职位</th><th><a class="teal-text" href="javascript:void(0)" (click)="initUserModal(null)">添加</a></th></tr></thead>
                <tbody>
                    <tr *ngFor="let user of currentUser[0]"><td>{{user.name}}</td><td>{{user.username}}</td><td>高一({{user.cls}})班议委</td><td><a href="/user-manage#" (click)="initUserModal(user)">管理</a></td></tr>
                    <tr *ngFor="let user of currentUser[1]"><td>{{user.name}}</td><td>{{user.username}}</td><td>{{user.unit}}单元议委</td><td><a href="/user-manage#" (click)="initUserModal(user)">管理</a></td></tr>
                </tbody>
            </table>
        </div>
        <div *ngIf="rReady && current == 1" class="container">
            <table class="bordered">
                <thead><tr><th>姓名</th><th>用户名</th><th>职位</th><th>操作</th></tr></thead>
                <tbody>
                    <tr *ngFor="let user of retiredUser"><td>{{user.name}}</td><td>{{user.username}}</td><td>{{user.unit ? user.unit + "单元议委" : "高一（" + user.cls + "）班议委"}}</td><td><a href="/user-manage#" (click)="initUserModal(user)">管理</a></td></tr>
                </tbody>
            </table>
        </div>
        <div id="user-modal" class="modal" style="overflow:visible;max-height:none;position:absolute;">
            <div class="modal-content">
                <h5 class="teal-text">编辑用户</h5><br />
                <div class="row">
                    <div class="col s12 input-field">
                        <input [(ngModel)]="name" id="name" type="text" />
                        <label class="active" for="name">姓名</label>
                    </div>
                      
                      <div class="input-field col s12 m6"> 
                        <select id="type">
                          <option value="" disabled="disabled" selected="selected">选择议委类型</option>
                          <option value="1">班级议委</option>
                          <option value="2">单元议委</option>
                        </select>
                         <label>议委类型</label>
                      </div>
                    <div class="input-field col s12 m6"> 
                        <select id="unit">
                          <option value="" disabled="disabled" selected="selected">选择议委单元</option>
                          <option value="0">班级议委</option>
                          <option value="1">一、二单元</option>
                            <option value="3">三、四单元</option>
                            <option value="5">五单元</option>
                            <option value="6">六单元</option>
                            <option value="7">七单元</option>
                            <option value="8">八单元</option>
                        </select>
                         <label>单元</label>
                      </div>
                    <div class="input-field col s12 m6"> 
                        <input [(ngModel)]="username" placeholder="输入用户名" title="用户名" type="text" />
                      <label class="active" for="username">用户名</label>
                      </div>
                    <div class="input-field col s12 m6">
                      <input [(ngModel)]="cls" placeholder="输入数字。单元议委填写单元数字（如三、四单填写3）" title="班级" pattern="^[0-9]*$" type="text" class="validate" />
                      <label class="active" for="ratio">班级</label>
                    </div>
                    <div class="input-field col s12 m6"> 
                        <input [(ngModel)]="phone" placeholder="输入议委电话" title="电话号码" pattern="^[0-9]*$" type="text" class="validate" />
                      <label class="active" for="phone">电话号码</label>
                      </div>
                    <div class="col s12 m6">
                      <div class="switch">
                        <label style="line-height: 2.6">
                          账号状态<br />禁用
                          <input [(ngModel)]="state" checked="checked" type="checkbox" />
                          <span class="lever"></span>
                          启用
                        </label>
                      </div>
                    </div>
                </div>
                <p style="font-size:14px">注意：<br />1. 议委添加后将无法删除，只能禁用，请谨慎添加；<br />2. 被禁用的议委将被移至“退休议委”；<br />3. 占位议委无法被修改或禁用；<br />4. 所有修改将在点击“保存”后生效；<br />5. 用户名命名规则为 名首字母+姓完整拼音+级数+0，如朱华伟17年入学则为hwzhu170，若用户名已被占用则为hwzhu171，以此类推；<br />6. 班级处单元议委填写单元数字（如三、四单填写3）；<br />7. 添加用户后将随机生成密码，可在“换届助手”中批量导出。</p>
                <div *ngIf="pReady"><br />
                    <h5 class="teal-text">密码管理</h5>
                    <p>默认密码：{{defaultPassword}}（{{passwordChanged ? "已" : "未"}}更改）</p>
                    <button (click)="resetPassword()" class="btn btn-flat teal-text waves-effect waves-ripple" style="font-size:16px;border:1px solid #009688">重置</button>
                </div>
            </div>
            <div class="modal-footer">
                <a href="javascript:void(0)" (click)="saveUser()" class="waves-effect waves-green btn-flat teal-text">保存</a>
                <a href="javascript:void(0)" [ngClass]="{'disabled':!selected || pReady}" (click)="initPasswordManagement()" class="waves-effect waves-ripple btn-flat">密码管理</a>
                <a href="javascript:void(0)" class="modal-action modal-close waves-effect waves-ripple btn-flat">关闭</a>
            </div>
        </div>
        <div id="transition-modal" class="modal" style="overflow:visible;max-height:none;position:absolute;">
            <div class="modal-content">
                <h5 class="teal-text">换届助手</h5><br />
                <div class="input-field">
                    <input [(ngModel)]="year" placeholder="输入级数。如2015年入学填15" title="级数" pattern="^[0-9]*$" type="text" class="validate" />
                    <label class="active" for="ratio">级数</label>
                </div>
                <a id="getcsv" download="user.csv" style="display:none"></a>
                <a href="javascript:void(0)" (click)="retireYear()" class="btn btn-flat teal-text waves-effect waves-ripple" style="font-size:16px;border:1px solid #009688">批量退休</a>
                <a href="javascript:void(0)" (click)="getDefaultPasswordYear()" class="btn btn-flat teal-text waves-effect waves-ripple" style="font-size:16px;border:1px solid #009688">导出默认密码</a>
                <br /><br /><span>导出密码需使用chrome浏览器。</span>
            </div>
            <div class="modal-footer">
                <a href="javascript:void(0)" class="modal-action modal-close waves-effect waves-ripple btn-flat">关闭</a>
            </div>
        </div>`
})

export class AppComponent implements OnInit, AfterViewInit {
    current = 0;
    currentUser: [User[], User[]];
    retiredUser: User[];
    cReady = false;
    rReady = false;
    pReady = false;
    defaultPassword = "";
    passwordChanged = false;
    year: string;

    selected: User;
    name: string;
    type: number = 1;
    unit: number = 0;
    username: string;
    cls: string;
    phone: string;
    state = true;

    constructor(private UserService: UserService) { }

    refresh() {
        if (this.current == 0)
            this.showCurrent();
        else
            this.showRetired();
    }

    showCurrent(): void {
        this.current = 0;
        this.cReady = false;
        this.UserService.listUsers().then(lists => { this.currentUser = this.removePlaceholders(lists); this.cReady = true; $('select').material_select(); })
    }

    showRetired(): void {
        this.current = 1;
        this.rReady = false;
        this.UserService.listRetiredUsers().then(result => { this.retiredUser = result; this.rReady = true; });
    }

    saveUser(): void {
        this.type = $('#type').val();
        this.unit = $('#unit').val();
        if (!this.name || this.name.trim().length == 0)
            alert("姓名不得为空");
        else if (this.name.length > 10)
            alert("姓名过长");
        else if (!this.type || !this.unit)
            alert("请选择议委类型及单元");
        else if ((this.type == 1 && this.unit > 0) || (this.type == 2 && this.unit == 0))
            alert("议委类型与所选单元冲突，若为班级议委则单元需选第一项");
        else if (!this.username || this.username.trim().length == 0)
            alert("用户名不得为空");
        else if (this.username.length > 30)
            alert("用户名过长");
        else if (this.cls.trim().length == 0)
            alert("班级不得为空")
        else if (!/^[0-9]*$/.test(this.cls))
            alert("班级必须为整数");
        else if (parseInt(this.cls) < 1 || parseInt(this.cls) > 256)
            alert("班级数不合法");
        else if (this.type == 2 && this.unit != parseInt(this.cls)) {
            alert("单元议委单元与班级不匹配，班级应填写"+this.unit+"。")
        }
        else if (!/^[0-9]*$/.test(this.phone) || this.phone.length > 13) {
            alert("电话号码必须为13位数字以内整数")
        }
        else {
            if (this.selected) {
                this.UserService.updateUser(this.selected.id, this.name.trim(), this.type, this.unit, this.username.trim(), parseInt(this.cls), this.phone.trim(), this.state).then(result => {
                    this.handleFlag(result);
                });
            }
            else {
                this.UserService.newUser(this.name.trim(), this.type, this.unit, this.username.trim(), parseInt(this.cls), this.phone.trim(), this.state).then(result => {
                    this.handleFlag(result);
                });
            }
        }
    }

    handleFlag(flag: number): void {
        switch (flag) {
            case 1:
                alert("保存成功！");
                this.refresh();
                $('#user-modal').modal('close');
                break;
            case -1:
                alert("用户名已被占用");
                break;
            case -10:
                alert("出现未知服务器错误。")
                break;
        }
    }

    initTransitionModal(): void {
        $('#transition-modal').modal('open');
    }

    initUserModal(u: User): void {
        if ((this.current == 0 && this.cReady) || (this.current == 1 && this.rReady)) {
            this.pReady = false;
            if (u) {
                this.selected = u;
                this.name = u.name;
                $('#type').val(u.unit ? 2 : 1);
                $('#unit').val(u.unit ? u.cls : 0);
                this.username = u.username;
                this.cls = u.cls.toString();
                this.phone = u.phone;
                this.state = this.current == 0;
            }
            else {
                this.selected = null;
                this.name = "";
                $('#type').val(-1);
                $('#unit').val(-1);
                this.username = "";
                this.cls = "";
                this.phone = "";
                this.state = true;
            }
            $('select').material_select();
            $('#user-modal').modal('open');
        }
    }

    initPasswordManagement(): void {
        this.UserService.getDefaultPassword(this.selected.id).then(result => { this.defaultPassword = result[0]; this.passwordChanged = result[1] == "True"; this.pReady = true; })
    }

    resetPassword(): void {
        if (confirm("确定要重置" + this.selected.name + "的账户密码吗？")) {
            this.UserService.resetPassword(this.selected.id).then(result => {
                switch (result[0]) {
                    case "1":
                        alert("用户密码已重置。");
                        this.defaultPassword = result[1];
                        this.passwordChanged = false;
                        break;
                    case "-10":
                        alert("出现未知服务器错误。")
                        break;
                }
            })
        }
    }

    retireYear(): void {
        var y = this.checkYear();
        if (y > 0) {
            this.UserService.retireYear(y).then(flag => {
                switch (flag) {
                    case 1:
                        alert("操作成功！");
                        this.refresh();
                        $('#transition-modal').modal('close');
                        break;
                    case -10:
                        alert("出现未知服务器错误。")
                        break;
                }
            });
        }
    }

    checkYear(): number {
        if (!this.year || this.year.trim().length == 0)
            alert("请填写级数");
        if (this.year.trim().length > 2)
            alert("级数过长");
        else if (!/^[0-9]*$/.test(this.year))
            alert("级数必须为整数");
        else if (parseInt(this.year) < 15)
            alert("级数需大于15");
        else
            return parseInt(this.year);
        return 0;
    }

    getDefaultPasswordYear(): void {
        var y = this.checkYear();
        var e = document.getElementById("getcsv") as HTMLLinkElement;
        if (y > 0) {
            this.UserService.getDefaultPasswordYear(y).then(list => {
                var str = "";
                for (let user of list) {
                    str = str + user.name + "," + user.username + "," + user.defaultpassword + "\n";
                }
                str = encodeURIComponent(str);
                e.href = "data:text/csv;charset=utf-8,\ufeff" + str;
                e.click();
            })
        }
    }

    removePlaceholders(list: [User[], User[]]): [User[], User[]] {
        var unit = new Array<User>();
        for (let u of list[1]) {
            if (!u.name.includes("占位")) {
                unit.push(u);
            }
        }
        return [list[0], unit];
    }

    ngOnInit(): void {
        this.UserService.listUsers().then(lists => { this.currentUser = this.removePlaceholders(lists); this.cReady = true; })
    }

    ngAfterViewInit(): void {
        $('.modal').modal();
        $('select').material_select()
        $('ul.tabs').tabs();
    }
}
