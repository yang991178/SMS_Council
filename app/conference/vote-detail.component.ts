import { Component, AfterViewInit , OnDestroy } from '@angular/core';
import { Location, LocationStrategy, PathLocationStrategy } from '@angular/common';

import { Subscription } from 'rxjs/Subscription';

import { User } from '../user.js';
import { Vote } from '../vote.js';
import { ConferenceService } from '../conference.service.js';

declare var $: any;
declare var parseInt: any;

@Component({
    selector: 'vote-detail',
    providers: [Location, { provide: LocationStrategy, useClass: PathLocationStrategy }],
    template: `<div class="container z-depth-3 votecontainer white"><div *ngIf="detailvote && userlist">
            <div class="progress" style="position:absolute;top:0;left:0;margin:0;height:300px">
                <div class="determinate lighten-1 green" [style.width]="(detailvote.cvote / (detailvote.cvote + detailvote.rvote + detailvote.avote + detailvote.nvvote) * 100) + '%'"></div>
                <div class="determinate red lighten-1" [style.width]="(detailvote.rvote  / (detailvote.cvote + detailvote.rvote + detailvote.avote + detailvote.nvvote) *100) + '%'"></div>
                <div class="determinate orange lighten-1" [style.width]="(detailvote.avote  / (detailvote.cvote + detailvote.rvote + detailvote.avote + detailvote.nvvote) *100) + '%'"></div>
                <div class="determinate grey" [style.width]="((1 - detailvote.avote  / (detailvote.cvote + detailvote.rvote + detailvote.avote + detailvote.nvvote) - detailvote.cvote  / (detailvote.cvote + detailvote.rvote + detailvote.avote + detailvote.nvvote) - detailvote.rvote  / (detailvote.cvote + detailvote.rvote + detailvote.avote + detailvote.nvvote))*100) + '%'"></div>
            </div>
            <nav class="white" style="position:absolute;top:0;left:0;">
                <button style="right:14px;top:70px;" id="refresh" class="btn btn-flat teal-text" [disabled]="reloading" [class.active]="reloading" (click)="reload()"><svg id="refresh-icon" fill="#26a69a" height="20" viewBox="0 0 24 24" width="20"><path d="M12 6v3l4-4-4-4v3c-4.42 0-8 3.58-8 8 0 1.57.46 3.03 1.24 4.26L6.7 14.8c-.45-.83-.7-1.79-.7-2.8 0-3.31 2.69-6 6-6zm6.76 1.74L17.3 9.2c.44.84.7 1.79.7 2.8 0 3.31-2.69 6-6 6v-3l-4 4 4 4v-3c4.42 0 8-3.58 8-8 0-1.57-.46-3.03-1.24-4.26z"/><path d="M0 0h24v24H0z" fill="none"/></svg>刷新</button>
                <a [routerLink]="'../../'" style="color:#808080;margin-left:0;margin-left:18.75px;margin-right:0" id="exit-vote" class="left top-nav waves-effect waves-ripple waves-circle"><svg width="24px" height="56px" viewBox="0 0 24 24" class="Huou3e "><path d="M20 11H7.83l5.59-5.59L12 4l-8 8 8 8 1.41-1.41L7.83 13H20v-2z"></path></svg></a>
                <div class="mobile-scroll" style="font-size:20px;font-weight:700;max-height:56px;white-space:nowrap">{{detailvote.title}}</div>
            </nav>
            <div class="votedetail card">
                <h3 class="center-align"><span class="green-text text-lighten-1">{{detailvote.cvote}}</span><span class="slash"> / </span><span class="red-text text-lighten-1">{{detailvote.rvote}}</span><span *ngIf="detailvote.type == 1" class="slash"> / </span><span *ngIf="detailvote.type == 1" class="orange-text text-lighten-1">{{detailvote.avote}}</span><span class="slash"> / </span><span class="grey-text">{{detailvote.nvvote}}</span></h3>
            </div>
            <div class="fixed-action-btn click-to-toggle horizontal" style="position:absolute;top:275px;bottom:unset;">
                <a class="btn-floating btn-large teal" title="投票"><svg style="vertical-align:middle;transform:rotate(180deg)" fill="#fff" height="30" viewBox="0 0 24 24" width="30"><path d="M0 0h24v24H0z" fill="none"/><path d="M19 4H5c-1.11 0-2 .9-2 2v12c0 1.1.89 2 2 2h4v-2H5V8h14v10h-4v2h4c1.1 0 2-.9 2-2V6c0-1.1-.89-2-2-2zm-7 6l-4 4h3v6h2v-6h3l-4-4z"/></svg></a>
                <ul>
                    <li><a class="btn-floating green darken-3 waves-effect waves-light" (click)="vote('C')" href="javascript:void(0)">赞成</a></li>
                    <li><a class="btn-floating red darken-3 waves-effect waves-light" (click)="vote('R')" href="javascript:void(0)">反对</a></li>
                    <li><a class="btn-floating orange waves-effect waves-light" *ngIf="detailvote.type == 1" (click)="vote('A')" href="javascript:void(0)">弃权</a></li>
                </ul>
            </div>
            <span class="teal-text" style="position:absolute;top:332px;right:40px;font-size:12px">投票</span>
            <ul class="collapsible" style="margin:0" data-collapsible="expandable">
                <li>
                    <div class="collapsible-header">赞成议委</div>
                    <div class="collapsible-body">{{getNameList(2,detailvote)}}</div>
                </li>
                <li>
                    <div class="collapsible-header">反对议委</div>
                    <div class="collapsible-body">{{getNameList(3,detailvote)}}</div>
                </li>
                <li *ngIf="detailvote && detailvote.type == 1">
                    <div class="collapsible-header">弃权议委</div>
                    <div class="collapsible-body">{{getNameList(4,detailvote)}}</div>
                </li>
                <li>
                    <div class="collapsible-header">未投议委</div>
                    <div class="collapsible-body">{{getNameList(1,detailvote)}}</div>
                </li>
            </ul>
        </div></div>`
})

export class VoteDetailComponent implements AfterViewInit, OnDestroy {
    detailvote: Vote;
    userlist: User[];
    useridlist: number[];
    consented: string;
    rejected: string;
    unvoted: string;
    usub: Subscription;
    vsub: Subscription;
    reloading = true;

    constructor(
        private ConferenceService: ConferenceService,
        private location: Location
    ) {
        this.usub = ConferenceService.userListStream$.subscribe(list => { this.userlist = list; this.fillIdList(list); });
        this.vsub = ConferenceService.voteListStream$.subscribe(list => { this.detailvote = list[parseInt(this.location.path().replace(/^\/conference\/[0-9]*\/vote\//, ""))]; this.reloading = false; } );
    }

    reload(): void {
        this.reloading = true;
        this.ConferenceService.listVotes(parseInt(this.location.path().replace(/^\/conference\//, "").replace(/\/vote\/[0-9]*$/, ""))).then(list => this.ConferenceService.setVoteList(list));
    }

    fillIdList(users: User[]): void {
        var list = new Array<number>(users.length);
        for (var u in users) {
            list[u] = users[u].id;
        }
        this.useridlist = list;
    }

    handleFlag(flag: number): void {
        switch (flag) {
            case 1:
                alert("投票成功！");
                this.ConferenceService.listVotes(parseInt(this.location.path().replace(/^\/conference\//, "").replace(/\/vote\/[0-9]*$/, ""))).then(list => this.ConferenceService.setVoteList(list));
                break;
            case -1:
                alert("请先登录。");
                break;
            case -2:
                alert("您没有参加此会议或已投票。");
                break;
            case -10:
                alert("出现未知服务器错误，请稍后再试。");
                break;
        }
    }
    vote(decision: string): void {
        if (this.detailvote.active) {
            if(confirm("投票后将无法更改，是否继续？"))
                this.ConferenceService.vote(this.detailvote.id, decision).then(flag => this.handleFlag(flag));
        }
        else
            alert("该表决已结束。");
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

    getNameList(type: number, vote: Vote): string {
        var list = new Array<string>();
        var search: number[];
        switch (type) {
            case 1:
                search = vote.notvoted;
                break;
            case 2:
                search = vote.consented;
                break;
            case 3:
                search = vote.rejected;
                break;
            case 4:
                search = vote.abstained;
                break;
        }
        for (let id of search) {
            var result = this.binarySearch(this.useridlist, id);
            if (result != -1)
                list.push(this.userlist[result].name);
        }
        var r = list.join("，");
        return r == "" ? "名单为空" : r;
    }

    ngAfterViewInit(): void {
        $('.collapsible').collapsible()
        $('vote-detail').bind('DOMNodeInserted', () => $('.collapsible').collapsible())
    }

    ngOnDestroy(): void {
        this.usub.unsubscribe();
        this.vsub.unsubscribe();
    }
}