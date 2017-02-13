import { Component, OnDestroy } from '@angular/core';
import { Location, LocationStrategy, PathLocationStrategy } from '@angular/common';

import { Subscription } from 'rxjs/Subscription';

import { Vote } from '../vote.js';
import { ConferenceService } from '../conference.service.js';

@Component({
    selector: 'vote-list',
    template: `        <div class="container z-depth-3 timeline">
            <div class="axis">
                <div class="line"></div>
                <div class="start"></div>
            </div>
            <div class="contents">
                <button id="refresh" class="btn btn-flat teal-text" [disabled]="reloading" [class.active]="reloading" (click)="reload()"><svg id="refresh-icon" fill="#26a69a" height="20" viewBox="0 0 24 24" width="20"><path d="M12 6v3l4-4-4-4v3c-4.42 0-8 3.58-8 8 0 1.57.46 3.03 1.24 4.26L6.7 14.8c-.45-.83-.7-1.79-.7-2.8 0-3.31 2.69-6 6-6zm6.76 1.74L17.3 9.2c.44.84.7 1.79.7 2.8 0 3.31-2.69 6-6 6v-3l-4 4 4 4v-3c4.42 0 8-3.58 8-8 0-1.57-.46-3.03-1.24-4.26z"/><path d="M0 0h24v24H0z" fill="none"/></svg>刷新</button>
                <div *ngFor="let vote of votelist; let i = index" class="votecard">
                    <div class="card">
                        <div class="progress">
                            <div class="determinate" [style.width]="(vote.cvote / (vote.cvote + vote.rvote + vote.avote + vote.nvvote) * 100) + '%'"></div>
                            <div class="determinate red" [style.width]="(vote.rvote  / (vote.cvote + vote.rvote + vote.avote + vote.nvvote) *100) + '%'"></div>
                            <div class="determinate orange" [style.width]="(vote.avote  / (vote.cvote + vote.rvote + vote.avote + vote.nvvote) *100) + '%'"></div>
                            <div class="determinate grey" [style.width]="((1 - vote.avote  / (vote.cvote + vote.rvote + vote.avote + vote.nvvote) - vote.cvote  / (vote.cvote + vote.avote + vote.rvote + vote.nvvote) - vote.rvote  / (vote.cvote + vote.avote + vote.rvote + vote.nvvote))*100) + '%'"></div>
                        </div>
                        <div class="card-content">
                            <h5>{{vote.title}}</h5>
                        </div>
                        <div class="card-action">
                            {{vote.cvote < vote.passvote ? "未通过" : "已通过"}} · <a [routerLink]="['./vote', i]" class="teal-text">{{vote.active ? "进入表决" : "查看详情"}}</a>
                        </div>
                    </div>
                    <p class="time">{{vote.time}}</p>
                </div>
                <div class="votecard" *ngIf="!loaded"><div class="card"><div class="progress"></div><div class="card-content"><h5>暂无表决</h5></div><div class="card-action">会议即将开始</div></div></div>
            </div>
        </div>`
})

export class VoteListComponent implements OnDestroy {
    votelist: Vote[];
    loaded = false;
    subscription: Subscription;
    reloading = true;

    constructor(private ConferenceService: ConferenceService, private location: Location) { 
        this.subscription = ConferenceService.voteListStream$.subscribe(list => { this.votelist = list; this.loaded = !(list.length == 0); this.reloading = false });
    }

    reload(): void {
        this.reloading = true;
        this.ConferenceService.listVotes(parseInt(this.location.path().replace(/^\/conference\//, "").replace(/\/vote\/[0-9]*$/, ""))).then(list => this.ConferenceService.setVoteList(list));
    }

    ngOnDestroy(): void {
        this.subscription.unsubscribe()
    }
}