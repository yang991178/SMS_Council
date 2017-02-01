import { Component, OnInit } from '@angular/core';

import { UserService } from '../user.service.js';

@Component({
  selector: 'login',
  template: `
        <div id="header" class="teal lighten-1">
            <div class="container">
                <div class="row center" style="margin-top:120px">
                    <h4 class="light white-text">&nbsp;&nbsp;{{greeting}}，{{disname}}！</h4>
                </div>
            </div>
        </div>
        <div class="container">
            <div id="login" class="z-depth-4">
                <p class="center"><svg width="72px" height="72px" style="fill:#808080;border:1px dashed #808080;border-radius:50%" viewBox="0 0 48 48"><path d="M24 24c4.42 0 8-3.59 8-8 0-4.42-3.58-8-8-8s-8 3.58-8 8c0 4.41 3.58 8 8 8zm0 4c-5.33 0-16 2.67-16 8v4h32v-4c0-5.33-10.67-8-16-8z"></path></svg></p>
                <form class="center" style="padding:0 30px">
                    <div class="row">
                        <div class="input-field col s12">
                            <input [(ngModel)]="id" name="id" (keyup)="onChange($event)" id="uid" type="text" />
                            <label for="uid">用户名</label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="input-field col s12">
                            <input [(ngModel)]="password" name="password" id="password" type="password" />
                            <label for="password">密码</label>
                        </div>
                    </div>
                      <button [disabled]="logging" (click)="logMeIn()" class="btn waves-effect waves-light" type="submit" name="action">登录</button>
                </form>
            </div>
        </div>
        <div *ngIf="warning" id="warning" class="z-depth-4 red lighten-2 white-text">
            <p class="center-align"><svg viewBox="0 0 24 24" width="22" height="22" style="vertical-align:middle"><path d="M1 21h22L12 2 1 21zm12-3h-2v-2h2v2zm0-4h-2v-4h2v4z"></path></svg>{{warning}}</p>
        </div>
  `,
})

export class AppComponent implements OnInit  {
    greeting: string;
    warning: string;
    disname = "议委";
    id = "";
    password = "";
    logging = false;

    constructor(private UserService: UserService) { }

    onChange(event: KeyboardEvent): void {
        var v = (<HTMLInputElement>event.target).value;
        if (v.length <= 10)
            this.disname = v == "" ? "议委" : v;
    }

    handleFlag(flag: number): void {
        switch (flag) {
            case 1:
                window.location.reload(true);
                break;
            case -1:
                this.warning = "用户不存在";
                break;
            case -2:
                this.warning = "密码错误";
                break;
        }
        this.logging = false;
    }

    logMeIn(): void {
        if (this.id.trim().length == 0 || this.password.length == 0)
            this.warning = "请填写用户名及密码";
        else if (this.id.trim().length > 10)
            this.warning = "用户名过长";
        else {
            this.logging = true;
            this.UserService.logIn(this.id, this.password).then(flag => this.handleFlag(flag));
        }
    }

    ngOnInit(): void {
        var h = new Date().getUTCHours() + 8;
        this.greeting = (h >= 5 && h < 12 ? "早上好" : (h < 5 || h >= 18 ? "晚上好" : "下午好"));
    }
}
