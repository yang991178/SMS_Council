import { Component, OnInit } from '@angular/core';

import { User } from '../user.js';
import { UserService } from '../user.service.js';

@Component({
  selector: 'contacts',
  template: `<div class="col s12 m4 l4 contactlist">
                <h5 class="text-darken-1 grey-text center-align">班级议委</h5>
                <div class="collection">
                    <a href="javascript:void(0)" class="waves-effect waves-ripple collection-item" *ngFor="let user of clsUser" (click)="showDetail(user)">{{user.name}}<span class="new badge" data-badge-caption="班">{{user.cls}}</span></a>
                </div>
                <h5 class="text-darken-1 grey-text center-align">单元议委</h5>
                <div class="collection">
                    <a href="javascript:void(0)" class="waves-effect waves-ripple collection-item" *ngFor="let user of unitUser" (click)="showDetail(user)">{{user.name}}<span class="new badge" data-badge-caption="单元">{{user.unit}}</span></a>
                </div>
            </div>
            <div [class.on]="detailOn" [class.off]="detailOff" (click)="hideDetail()" class="c-overlay"></div>
            <div [class.on]="detailOn" [class.off]="detailOff" class="col s12 m8 l8 teal lighten-1 white-text cdetail">
                <div *ngIf="detailUser">
                    <span class="close" (click)="hideDetail()">×</span>
                    <h4>{{detailUser.name}}</h4>
                    <p class="teal-text text-lighten-4" *ngIf="!detailUser.unit">班级议委</p>
                    <p class="teal-text text-lighten-4" *ngIf="detailUser.unit">单元议委</p>
                    <p *ngIf="!detailUser.unit"><span class="h5">班级</span><br />高一（{{detailUser.cls}}）班</p>
                    <p *ngIf="detailUser.unit"><span class="h5">单元</span><br />高二 {{detailUser.unit}}单元</p>
                    <p><span class="h5">手机</span><br />{{detailUser.phone}}</p>
                </div>
            </div>`
})

export class AppComponent implements OnInit  {
    clsUser: User[];
    unitUser: User[];
    detailUser: User;
    public detailOn = false;
    public detailOff = false;

    constructor(private UserService: UserService) { }

    hideUsers(list: User[]): User[] {
        var result = new Array<User>();
        for (let user of list) {
            if (user.display)
                result.push(user);
        }
        return result;
    }

    listUsers(): void {
        var p = this.UserService.listUsers();
        p.then(users => { this.clsUser = users[0]; this.unitUser = this.hideUsers(users[1]); });
    }

    showDetail(user: User): void {
        this.detailUser = user;
        this.detailOn = true;
    }
    
    hideDetail(): void {
        this.detailOff = true;
        this.detailOn = false;
        setTimeout(() => this.detailOff = false, 300)
    }

    ngOnInit(): void {
        this.listUsers();
    }
}
