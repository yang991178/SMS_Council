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
                <div class="determinate lighten-1 green" [style.width]="(detailvote.cvote / (detailvote.cvote + detailvote.rvote + detailvote.nvvote) * 100) + '%'"></div>
                <div class="determinate red lighten-1" [style.width]="(detailvote.rvote  / (detailvote.cvote + detailvote.rvote + detailvote.nvvote) *100) + '%'"></div>
                <div class="determinate grey" [style.width]="((1 - detailvote.cvote  / (detailvote.cvote + detailvote.rvote + detailvote.nvvote) - detailvote.rvote  / (detailvote.cvote + detailvote.rvote + detailvote.nvvote))*100) + '%'"></div>
            </div>
            <nav class="white" style="position:absolute;top:0;left:0;">
                <a [routerLink]="'../../'" style="color:#808080;margin-left:0;margin-left:18.75px;margin-right:0" id="exit-vote" class="left top-nav waves-effect waves-light circle"><svg width="24px" height="56px" viewBox="0 0 24 24" class="Huou3e "><path d="M20 11H7.83l5.59-5.59L12 4l-8 8 8 8 1.41-1.41L7.83 13H20v-2z"></path></svg></a>
                <div class="mobile-scroll" style="font-size:20px;font-weight:700;max-height:56px;white-space:nowrap">{{detailvote.title}}</div>
            </nav>
            <div class="votedetail card">
                <h3 class="center-align"><span class="green-text text-lighten-1">{{detailvote.cvote}}</span><span class="slash"> / </span><span class="red-text text-lighten-1">{{detailvote.rvote}}</span><span class="slash"> / </span><span class="grey-text">{{detailvote.nvvote}}</span></h3>
            </div>
            <div class="fixed-action-btn click-to-toggle horizontal" style="position:absolute;top:275px;bottom:unset;">
                <a class="btn-floating btn-large orange" title="投票"><svg style="vertical-align:middle;transform:rotate(180deg)" fill="#fff" height="30" viewBox="0 0 24 24" width="30"><path d="M0 0h24v24H0z" fill="none"/><path d="M19 4H5c-1.11 0-2 .9-2 2v12c0 1.1.89 2 2 2h4v-2H5V8h14v10h-4v2h4c1.1 0 2-.9 2-2V6c0-1.1-.89-2-2-2zm-7 6l-4 4h3v6h2v-6h3l-4-4z"/></svg></a>
                <ul>
                    <li><a class="btn-floating btn-large green darken-3 waves-effect waves-light" (click)="vote('C')" href="javascript:void(0)">赞成</a></li>
                    <li><a class="btn-floating btn-large red darken-3 waves-effect waves-light" (click)="vote('R')" href="javascript:void(0)">反对</a></li>
                </ul>
            </div>
            <span class="orange-text" style="position:absolute;top:332px;right:40px;font-size:12px">投票</span>
            <ul class="collapsible" style="margin:0" data-collapsible="expandable">
                <li>
                    <div class="collapsible-header">赞成议委</div>
                    <div class="collapsible-body">{{getNameList(2,detailvote)}}</div>
                </li>
                <li>
                    <div class="collapsible-header">反对议委</div>
                    <div class="collapsible-body">{{getNameList(3,detailvote)}}</div>
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

    constructor(
        private ConferenceService: ConferenceService,
        private location: Location
    ) {
        this.usub = ConferenceService.userListStream$.subscribe(list => { this.userlist = list; this.fillIdList(list); });
        this.vsub = ConferenceService.voteListStream$.subscribe(list => this.detailvote = list[parseInt(this.location.path().replace(/^\/conference\/\d\/vote\//, ""))] );
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
                this.ConferenceService.listVotes(parseInt(this.location.path().replace(/^\/conference\//, "").replace(/\/vote\/\d$/, ""))).then(list => this.ConferenceService.setVoteList(list));
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
        var search = type == 1 ? vote.notvoted : (type == 2 ? vote.consented : vote.rejected);
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