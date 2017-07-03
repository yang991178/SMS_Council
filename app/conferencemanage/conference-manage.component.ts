import { Component, OnInit, AfterViewInit, ViewChild } from '@angular/core';

import { User } from '../user.js';
import { Vote } from '../vote.js';
import { Delegation } from '../delegation.js';
import { Conference } from '../conference.js';
import { UserService } from '../user.service.js';
import { ConferenceService } from '../conference.service.js';

declare var $: any;
declare var parseInt: any;

@Component({
    selector: 'conference-manage',
    template: `<div class="container">
            <table class="bordered">
                <thead><tr><th>id</th><th>会议名称</th><th>状态</th><th><a [hidden]="!(cReady && uReady && u2Ready)" (click)="initModal(null)" href="javascript:void(0)" class="teal-text">添加</a></th></tr></thead>
                <tbody>
                    <tr *ngFor="let conference of conferences"><td>{{conference.id}}</td><td>{{conference.name}}</td><td>{{conference.state == 1 ? "进行中" : "已结束"}}</td><td><a href="javascript:void(0)" (click)="initModal(conference)">修改</a></td></tr>
                </tbody>
            </table>
        </div>
        <div id="modal" class="modal" style="overflow:visible;max-height:none;position:absolute;">
            <div class="modal-content">
                <h5 class="teal-text">编辑会议</h5><br />
                <div class="row">
                    <div class="col s12 input-field">
                        <input [(ngModel)]="name" placeholder="不超过50个字符" id="name" type="text" class="validate" />
                        <label for="name" class="active">会议名称</label>
                    </div>
                      <div class="input-field col s12 m6"> 
                        <select id="clsuser" multiple="multiple">
                          <option value="" disabled="disabled" selected="selected">选择班级议委</option>
                          <option *ngFor="let user of clsUser" value="{{user.id}}">{{user.name+"（"+user.cls+"班）"}}</option>
                        </select>
                         <label>班级议委</label>
                      </div>
                      <div class="input-field col s12 m6"> 
                        <select id="unituser" multiple="multiple">
                          <option value="" disabled="disabled" selected="selected">选择单元议委</option>
                          <option *ngFor="let user of unitUser" value="{{user.id}}">{{user.name+"（"+user.unit+"单元）"}}</option>
                        </select>
                         <label>单元议委</label>
                      </div>
                    <div class="input-field col s12 m6">
                      <input [(ngModel)]="ratio" placeholder="输入1至256之间的整数" title="单元议委票权" pattern="^[0-9]*$" id="ratio" type="text" class="validate" />
                      <label for="ratio" class="active">单元议委票权</label>
                    </div>
                    <div class="col s12 m6">
                      <div class="switch">
                        <label style="line-height: 2.6">
                          会议状态<br />已结束
                          <input [(ngModel)]="state" type="checkbox" />
                          <span class="lever"></span>
                          进行中
                        </label>
                      </div>
                    </div>
                </div>
                <div *ngIf="votes && delegations">
                    <h5 class="teal-text">委托管理<button [disabled]="newDelegation" (click)="addDelegation()" class="btn btn-flat teal-text waves-effect waves-ripple right" style="font-size:16px;border: 1px solid #009688;">添加</button></h5>
                    <table class="bordered">
                        <thead><tr><th>议委</th><th>目标</th><th>操作</th></tr></thead>
                        <tbody *ngIf="delegations.length > 0">
                            <tr *ngFor="let delegation of delegations"><td>{{getNameList([delegation.subject])}}</td><td>{{getNameList(delegation.object)}}</td><td><a (click)="deleteDelegation(delegation.id)" href="javascript:void(0)" class="red-text">删除</a></td></tr>
                        </tbody>
                        <tbody *ngIf="delegations.length == 0">
                            <tr><td>无委托</td><td></td><td></td></tr>
                        </tbody>
                    </table>
                    <div [hidden]="!newDelegation" style="margin-top:10px">
                        <div class="row">
                            <div class="input-field col s12 m6"><select id="subject" [(ngModel)]="subject"><option value="0" selected disabled>选择议委</option><option *ngFor="let user of unitUser" value="{{user.id}}">{{user.name+"（"+user.unit+"单元）"}}</option></select><label>议委</label></div>
                            <div class="input-field col s12 m6"><select id="object" multiple><option value="" selected disabled>选择目标</option><option *ngFor="let user of unitUser" value="{{user.id}}">{{user.name+"（"+user.unit+"单元）"}}</option></select><label>目标</label></div>
                        </div>
                        <button (click)="saveDelegation()" class="btn right" style="margin:-10px 0 10px">保存新委托</button>
                    </div>
                    <div style="clear:both"></div>
                    <h5 class="teal-text">表决管理<button [disabled]="newVote" (click)="addVote()" class="btn btn-flat teal-text waves-effect waves-ripple right" style="font-size:16px;border: 1px solid #009688;">添加</button></h5>
                    <table class="bordered">
                        <thead><tr><th>标题</th><th>操作</th><th></th></tr></thead>
                        <tbody>
                            <tr *ngIf="votes.length == 0"><td>无表决</td><td></td><td></td></tr>
                            <tr *ngFor="let vote of votes"><td>{{vote.title}}</td><td><a href="/conference/manage/vote/{{vote.id}}">管理</a></td><td><a (click)="deleteVote(vote.id)" href="javascript:void(0)" class="red-text">删除</a></td></tr>
                        </tbody>
                    </table>
                    <div [hidden]="!newVote">
                        <div class="input-field">
                            <input [(ngModel)]="vote" id="newvote" type="text" />
                            <label for="newvote">表决内容</label>
                        </div>
                        <div>如更改了会议信息，请先保存会议再添加表决
                            <button (click)="saveVote()" class="btn right">保存新表决</button>
                            <div class="input-field right" style="margin:-3px 5px 0"><select class="browser-default" [(ngModel)]="votetype"><option value="1" selected>可弃权</option><option value="2">不可弃权</option></select></div>
                            <div class="input-field right" style="margin:-3px 5px 0"><select class="browser-default" [(ngModel)]="passvote"><option value="1" selected>半数通过</option><option value="2">三分之二通过</option><option value="3">自定义阈值</option></select></div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-footer" style="clear:both">
                <button (click)="saveConference()" class="waves-effect waves-green btn-flat teal-text">保存</button>
                <button *ngIf="!editAction" (click)="deleteConference()" class="waves-effect waves-red btn-flat red-text">删除</button>
                <button class="modal-action modal-close waves-effect waves-ripple btn-flat">关闭</button>
            </div>
        </div>`
})

export class AppComponent implements OnInit, AfterViewInit {
    cElement: HTMLSelectElement;
    uElement: HTMLSelectElement;

    conferences: Conference[];
    clsUser: User[];
    unitUser: User[];
    sUser: User[]; //Serialized Users
    sUserId: number[];
    cReady = false;
    uReady = false;
    u2Ready = false;
    editAction = true;

    id: number;
    name: string;
    ratio: any;
    state = true;
    votes: Vote[];

    vote = "";
    votetype = 1;
    passvote = 1;
    newVote = false;

    delegations: Delegation[];
    subject: number;
    newDelegation = false;

    constructor(private UserService: UserService, private ConferenceService: ConferenceService) { }

    setIdList(): void {
        var list = new Array<number>();
        for (let user of this.sUser) {
            list.push(user.id);
        }
        this.sUserId = list;
    }

    binarySearch(srcArray: number[], des: number): number {
        var low = 0;
        var high = srcArray.length - 1;
        while (low <= high) {
            var middle = parseInt((low + high) / 2);
            if (des == srcArray[middle]) {
                return middle;
            }
            else if (des < srcArray[middle]) {
                high = middle - 1;
            }
            else {
                low = middle + 1;
            }
        }
        return -1;
    }

    getNameList(search: number[]): string {
        var list = new Array<string>();
        for (let id of search) {
            var result = this.binarySearch(this.sUserId, id);
            if (result != -1)
                list.push(this.sUser[result].name + "（" + this.sUser[result].unit + "单元）");
        }
        var r = list.join("，");
        return r == "" ? "名单为空" : r;
    }

    addDelegation(): void {
        $('select').material_select();
        this.subject = 0;
        this.newDelegation = true;
    }

    saveDelegation(): void {
        if ($('#subject').val() == "")
            alert(this.subject);
        else if ($('#object').val() == "")
            alert("请选择目标议委")
        else {
            this.ConferenceService.saveDelegation(this.id, $('#subject').val(), $('#object').val().toString()).then(flag => this.handleModificationFlag(flag, "delegation"));
            this.newDelegation = false;
        }
    }

    deleteDelegation(id: number): void {
        if (confirm("确定要删除该委托吗？")) {
            this.ConferenceService.deleteDelegation(id).then(flag => this.handleModificationFlag(flag, "delegation"));
        }
    }

    addVote(): void{
        this.vote = "";
        this.votetype = 1;
        this.passvote = 1;
        this.newVote = true;
    }

    saveVote(): void {
        if (this.vote.trim().length == 0) {
            alert("内容不得为空。");
        }
        else if (this.vote.length > 50) {
            alert("表决内容过长。");
        }
        else {
            var getp = () => {
                var x = prompt("输入通过表决所需票数，当前会议票数为" + ($('#clsuser').val().length + $('#unituser').val().length * this.ratio) + "。（若修改过会议信息请先保存，否则该票数可能不准确）","0");
                if (!/^[0-9]*$/.test(x)) {
                    alert("阈值必须为纯数字。")
                    return "err";
                }
                return x;
            };
            var p = this.passvote == 3 ? getp() : this.passvote.toString();
            if (p == "err") return;
            this.ConferenceService.saveVote(this.id, this.vote, this.votetype, p).then(flag => this.handleModificationFlag(flag, "vote"));
            this.newVote = false;
        }
    }

    deleteVote(id: number): void {
        if (confirm("确定要删除该表决吗？")) {
            this.ConferenceService.deleteVote(id).then(flag => this.handleModificationFlag(flag, "vote"));
        }
    }

    deleteConference(): void {
        if (confirm("确定要删除该会议吗？")) {
            this.ConferenceService.deleteConference(this.id).then(flag => {
                switch (flag) {
                    case 1:
                        this.cReady = false;
                        this.ConferenceService.listConferences().then(list => { this.conferences = list; this.cReady = true; });
                        break;
                    case -1:
                        alert("登录已过期，请重新登录");
                        window.location.href = "/";
                        break;
                    case -10:
                        alert("出现未知服务器错误。");
                        break;
                }
            });
            $('#modal').modal('close');
        }
    }

    handleModificationFlag(flag: number, type: string): void {
        switch (flag) {
            case 1:
                if (type == "vote")
                    this.ConferenceService.listVotes(this.id).then(list => this.votes = list);
                else if (type == "delegation")
                    this.ConferenceService.listDelegations(this.id).then(list => this.delegations = list);
                break;
            case -1:
                alert("登录已过期，请重新登录");
                window.location.href = "/";
                break;
            case -10:
                alert("出现未知服务器错误。");
                break;
        }
    }

    handleFlag(flag: number): void {
        switch (flag) {
            case 1:
                alert("会议保存成功！");
                this.cReady = false;
                this.ConferenceService.listConferences().then(list => { this.conferences = list; this.cReady = true });
                $('#modal').modal('close');
                break;
            case -1:
                alert("登录已过期，请重新登录");
                window.location.href = "/";
                break;
            case -10:
                alert("出现未知服务器错误。");
                break;
        }
    }

    saveConference(): void {
        if (!this.name || this.name.trim().length == 0)
            alert("会议名称不得为空");
        else if (this.name.length > 50)
            alert("会议名称过长");
        else if (!/^[0-9]*$/.test(this.ratio))
            alert("单元议委票权必须为整数");
        else if (parseInt(this.ratio) < 1 || parseInt(this.ratio) > 256)
            alert("单元议委票权不合法")
        else {
            if (this.editAction) {
                this.ConferenceService.addConference(this.name, this.ratio, $('#clsuser').val() ? $('#clsuser').val().toString() : "", $('#clsuser').val() ? $('#clsuser').val().length : 0, $('#unituser').val() ? $('#unituser').val().toString() : "", $('#unituser').val() ? $('#unituser').val().length : 0,this.state ? 1 : 2).then(flag => this.handleFlag(flag));
            }
            else {
                this.ConferenceService.updateConference(this.id, this.name, this.ratio, $('#clsuser').val() ? $('#clsuser').val().toString() : "", $('#clsuser').val() ? $('#clsuser').val().length : 0, $('#unituser').val() ? $('#unituser').val().toString() : "", $('#unituser').val() ? $('#unituser').val().length : 0, this.state ? 1 : 2).then(flag => this.handleFlag(flag));
            }
        }
    }

    initModal(c: Conference): void {
        if (this.cReady && this.uReady && this.u2Ready) {
            if (c) {
                this.updateSelectList(this.cElement, c.cparticipants);
                this.updateSelectList(this.uElement, c.uparticipants);
                this.id = c.id;
                this.name = c.name;
                this.ratio = c.ratio;
                this.state = c.state == 1;
                this.editAction = false;
                this.votes = null;
                this.delegations = null;
                this.ConferenceService.listVotes(c.id).then(list => this.votes = list);
                this.ConferenceService.listDelegations(c.id).then(list => this.delegations = list);
            }
            else {
                if (!(this.editAction)) {
                    this.updateSelectList(this.cElement, []);
                    this.updateSelectList(this.uElement, []);
                    this.name = "";
                    this.ratio = null;
                    this.state = true;
                    this.votes = null;
                    this.delegations = null;
                }
                this.editAction = true;
            }
            this.newVote = false;
            this.newDelegation = false;
            $('select').material_select();
            $('#modal').modal('open')
        }
    }

    updateSelectList(element: HTMLSelectElement, selected: number[]) {
        let options: any = element.options;
        for (let i = 0; i < options.length; i++) {
            options[i].selected = selected.indexOf(parseInt(options[i].value)) > -1;
        }
    }

    userListToNumber(users: User[]): number[] {
        var result = new Array<number>(users.length);
        for (var u in users) {
            result[u] = users[u].id;
        }
        return result;
    }

    ngOnInit(): void {
        this.UserService.listSequenceUsers().then(list => { this.sUser = list; this.setIdList(); this.u2Ready = true; });
        this.ConferenceService.listConferences().then(list => { this.conferences = list; this.cReady = true });
        this.UserService.listUsers().then(lists => { this.clsUser = lists[0]; this.unitUser = lists[1]; this.uReady = true; })
    }

    ngAfterViewInit(): void {
        $('.modal').modal()
        this.cElement = <HTMLSelectElement>document.getElementById("clsuser");
        this.uElement = <HTMLSelectElement>document.getElementById("unituser");
    }
}
