import { Component, OnInit, AfterViewInit, ViewChild } from '@angular/core';
import { Location, LocationStrategy, PathLocationStrategy } from '@angular/common';

import { User } from '../user.js';
import { Vote } from '../vote.js';
import { Conference } from '../conference.js';
import { UserService } from '../user.service.js';
import { ConferenceService } from '../conference.service.js';

declare var parseInt: any;

@Component({
    selector: 'vote-manage',
    template: `        <div class="header teal valign-wrapper">
            <div class="container">
                <h5 *ngIf="vote" class="white-text" style="width:100%;font-weight:300">{{vote.title}}</h5>
                <div class="switch right">
                    <label class="white-text">
                        已结束
                        <input (change)="setVoteState()" [disabled]="switching" [(ngModel)]="active" type="checkbox" />
                        <span class="lever"></span>
                        进行中
                    </label>
                </div>
            </div>
        </div>
        <div class="container">

            <div *ngIf="notvoted">
                <h5 class="teal-text">未投议委</h5>
                <table class="bordered">
                    <thead><tr><th>姓名</th><th>投票</th><th></th></tr><tr></tr></thead>
                    <tbody>
                        <tr *ngFor="let user of notvoted"><td>{{user.name + (user.unit ? "（" + user.unit + "单元）" : "（" + user.cls + "班）")}}</td><td><a (click)="adminVote('C', user.id)" class="green-text" href="javascript:void(0)">赞成</a></td><td><a (click)="adminVote('R', user.id)" class="red-text" href="javascript:void(0)">反对</a><td><a (click)="adminVote('A', user.id)" *ngIf="vote.type == 1" class="orange-text" href="javascript:void(0)">弃权</a></td></tr>
                        <tr *ngIf="notvoted.length == 0"><td>无未投议委</td><td></td><td></td></tr>
                    </tbody>
                </table>
            </div>
            <div *ngIf="consented">
                <h5 class="teal-text">赞成议委</h5>
                <table class="bordered">
                    <thead><tr><th>姓名</th></tr></thead>
                    <tbody>
                        <tr *ngFor="let user of consented"><td>{{user.name + (user.unit ? "（" + user.unit + "单元）" : "（" + user.cls + "班）")}}</td></tr>
                        <tr *ngIf="consented.length == 0"><td>无赞成议委</td></tr>
                    </tbody>
                </table>
            </div>
            <div *ngIf="rejected">
                <h5 class="teal-text">反对议委</h5>
                <table class="bordered">
                    <thead><tr><th>姓名</th></tr></thead>
                    <tbody>
                        <tr *ngFor="let user of rejected"><td>{{user.name + (user.unit ? "（" + user.unit + "单元）" : "（" + user.cls + "班）")}}</td></tr>
                        <tr *ngIf="rejected.length == 0"><td>无反对议委</td></tr>
                    </tbody>
                </table>
            </div>
            <div *ngIf="vote && vote.type == 1 && abstained">
                <h5 class="teal-text">弃权议委</h5>
                <table class="bordered">
                    <thead><tr><th>姓名</th></tr></thead>
                    <tbody>
                        <tr *ngFor="let user of abstained"><td>{{user.name}}</td></tr>
                        <tr *ngIf="rejected.length == 0"><td>无弃权议委</td></tr>
                    </tbody>
                </table>
            </div>
        </div>`
})

export class AppComponent implements OnInit, AfterViewInit {
    active = false;
    switching = true;

    vote: Vote;
    userlist: User[];
    useridlist: number[];
    consented: User[];
    rejected: User[];
    abstained: User[];
    notvoted: User[];
    fillIdList(users: User[]): void {
        var list = new Array<number>(users.length);
        for (var u in users) {
            list[u] = users[u].id;
        }
        this.useridlist = list;
    }

    constructor(private UserService: UserService, private ConferenceService: ConferenceService, private location: Location) { }

    setVoteState(): void {
        this.switching = true;
        this.ConferenceService.setVoteState(this.vote.id, this.active).then(flag => {
            switch (flag) {
                case 1:
                    this.Load();
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

    getUserList(type: number): User[] {
        var list = new Array<User>();
        var search: number[];
        switch (type) {
            case 1:
                search = this.vote.notvoted;
                break;
            case 2:
                search = this.vote.consented;
                break;
            case 3:
                search = this.vote.rejected;
                break;
            case 4:
                search = this.vote.abstained;
                break;
        }
        for (let id of search) {
            var result = this.binarySearch(this.useridlist, id);
            if (result != -1)
                list.push(this.userlist[result]);
        }
        var r = list.join("，");
        return list;
    }

    adminVote(decision: string, uid: number): void {
        if (confirm("投票后将无法更改，是否继续？"))
            this.ConferenceService.adminVote(this.vote.id, decision, uid).then(flag => this.handleFlag(flag));
    }

    handleFlag(flag: number): void {
        switch (flag) {
            case 1:
                alert("投票成功！");
                this.Load();
                break;
            case -1:
                alert("登录已过期，请重新登录");
                window.location.href = "/";
                break;
            case -2:
            case -10:
                alert("出现未知服务器错误。");
                break;
        }
    }

    Load(): void {
        this.UserService.listSequenceUsers().then(list => {
            this.userlist = list;
            this.ConferenceService.getVote(parseInt(this.location.path().replace(/^\/conference\/manage\/vote\//, ""))).then(vote => {
                this.vote = vote;
                this.fillIdList(this.userlist);
                this.active = vote.active;
                this.notvoted = this.getUserList(1);
                this.consented = this.getUserList(2);
                this.rejected = this.getUserList(3);
                this.abstained = this.getUserList(4);
                this.switching = false;
            });
        });
    }

    ngOnInit(): void {
        this.Load();
    }

    ngAfterViewInit(): void {
        
    }
}
