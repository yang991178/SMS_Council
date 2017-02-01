import { Component } from '@angular/core';

import { UserService } from '../user.service.js';

@Component({
  selector: 'manage',
  template: `
        <div class="container">
            <div id="login" class="z-depth-4">
                <p class="center" style="margin-bottom:0"><svg width="64px" height="64px" style="fill:#808080;border:1px dashed #808080;border-radius:50%" viewBox="0 0 48 48"><path d="M24 24c4.42 0 8-3.59 8-8 0-4.42-3.58-8-8-8s-8 3.58-8 8c0 4.41 3.58 8 8 8zm0 4c-5.33 0-16 2.67-16 8v4h32v-4c0-5.33-10.67-8-16-8z"></path></svg></p>
                <form class="center" style="padding:0 30px">
                    <div class="row">
                        <div class="input-field col s12">
                            <input [(ngModel)]="oldpassword" name="oldpassword" type="password" />
                            <label for="oldpassword">旧密码</label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="input-field col s12">
                            <input [(ngModel)]="newpassword" name="newpassword" type="password" />
                            <label for="newpassword">新密码</label>
                        </div>
                    </div>
                        <button (click)="newPassword()" class="btn waves-effect waves-light" type="submit">修改密码</button><br />
                        <button (click)="logMeOut()" class="btn btn-flat waves-effect waves-ripple" type="button" style="margin-top:6px">登出</button>
                </form>
            </div>
        </div>
        <div *ngIf="warning" id="warning" class="z-depth-4 red lighten-2 white-text">
            <p class="center-align"><svg viewBox="0 0 24 24" width="22" height="22" style="vertical-align:middle"><path d="M1 21h22L12 2 1 21zm12-3h-2v-2h2v2zm0-4h-2v-4h2v4z"></path></svg>{{warning}}</p>
        </div>
  `,
})

export class AppComponent  {
    warning: string;
    oldpassword = "";
    newpassword = "";

    constructor(private UserService: UserService) { }

    handleFlag(flag: number): void {
        switch (flag) {
            case 1:
                alert("密码修改成功。");
                window.location.reload();
                break;
            case -1:
                alert("登录已过期，请重新登录。");
                window.location.reload();
                break;
            case -2:
                alert("密码错误。");
                break;
            case -10:
                alert("出现未知服务器错误，请稍后重试。")
                break;
        }
    }

    newPassword(): void {
        if (this.oldpassword.length == 0 || this.newpassword.length == 0)
            this.warning = "请填写旧密码及新密码";
        else if(!(/^[A-Za-z0-9]+$/.test(this.newpassword)))
            this.warning = "密码仅能由大小写字符及数字构成";
        else if (this.newpassword.length < 6 || this.newpassword.length > 30)
            this.warning = "密码必须在6-30字符之间";
        else {
            this.warning = null;
            this.UserService.changePassword(this.oldpassword, this.newpassword).then(flag => this.handleFlag(flag));
        }
    }

    logMeOut(): void {
        this.UserService.logOut().then(flag => { if (flag == 1) window.location.reload(true); })
    }
}
