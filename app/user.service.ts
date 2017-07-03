import { Injectable } from "@angular/core";
import { Headers, Http, Response } from '@angular/http';

import 'rxjs/add/operator/toPromise';

import { User } from "./user";

@Injectable()
export class UserService {
    constructor(private http: Http) { }

    listUsers(): Promise<[User[], User[]]> {
        return this.http.get("/ajax/User.ashx?action=list")
            .toPromise()
            .then(response => response.json() as [User[], User[]])
            .catch(this.handleError);
    }
    listSequenceUsers(): Promise<User[]> {
        return this.http.get("/ajax/User.ashx?action=listsequence")
            .toPromise()
            .then(response => response.json() as User[])
            .catch(this.handleError);
    }
    listRetiredUsers(): Promise<User[]> {
        return this.http.get("/ajax/User.ashx?action=listretired")
            .toPromise()
            .then(response => response.json() as User[])
            .catch(this.handleError);
    }

    logIn(u : string, p : string): Promise<number> {
        return this.http.post("/ajax/User.ashx?action=login", "username=" + u + "&password=" + p, { headers: this.headers })
            .toPromise()
            .then(response => response.json()[0] as number)
            .catch(this.handleError);
    }

    logOut(): Promise<number> {
        return this.http.get("/ajax/User.ashx?action=logout")
            .toPromise()
            .then(response => response.json()[0] as number)
            .catch(this.handleError);
    }

    changePassword(o: string, n: string): Promise<number> {
        return this.http.post("/ajax/User.ashx?action=changepassword", "old=" + o + "&new=" + n, { headers: this.headers })
            .toPromise()
            .then(response => response.json()[0] as number)
            .catch(this.handleError);
    }

    getDefaultPassword(id: number): Promise<[string, string]> {
        return this.http.get("/ajax/User.ashx?action=getdefaultpassword&id=" + id)
            .toPromise()
            .then(response => response.json() as [string, string])
            .catch(this.handleError);
    }

    resetPassword(id: number): Promise<string[]> {
        return this.http.post("/ajax/User.ashx?action=resetpassword", "id=" + id, { headers: this.headers })
            .toPromise()
            .then(response => response.json() as string[])
            .catch(this.handleError);
    }

    newUser(name: string, type: number, unit: number, username: string, cls: number, phone: string, state: boolean): Promise<number> {
        return this.http.post("/ajax/User.ashx?action=newuser", "name=" + name + "&type=" + type + "&unit=" + unit + "&username=" + username + "&class=" + cls + "&phone=" + phone + "&state=" + state, { headers: this.headers })
            .toPromise()
            .then(response => response.json()[0] as number)
            .catch(this.handleError);
    }

    updateUser(id: number, name: string, type: number, unit: number, username: string, cls: number, phone: string, state: boolean): Promise<number> {
        return this.http.post("/ajax/User.ashx?action=updateuser", "id=" + id + "&name=" + name + "&type=" + type + "&unit=" + unit + "&username=" + username + "&class=" + cls + "&phone=" + phone + "&state=" + state, { headers: this.headers })
            .toPromise()
            .then(response => response.json()[0] as number)
            .catch(this.handleError);
    }

    retireYear(year: number): Promise<number> {
        return this.http.post("/ajax/User.ashx?action=retireyear", "year=" + year, { headers: this.headers })
            .toPromise()
            .then(response => response.json()[0] as number)
            .catch(this.handleError);
    }

    getDefaultPasswordYear(year: number): Promise<any> {
        return this.http.get("/ajax/User.ashx?action=getdefaultpasswordyear&year=" + year)
            .toPromise()
            .then(response => response.json() as any)
            .catch(this.handleError);
    }

    private headers = new Headers({ 'Content-Type': 'application/x-www-form-urlencoded' });

    private handleError(error: any): Promise<any> {
        console.error('An error occurred', error); // for demo purposes only
        return Promise.reject(error.message || error);
    }
}