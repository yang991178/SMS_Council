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

    private headers = new Headers({ 'Content-Type': 'application/x-www-form-urlencoded' });

    private handleError(error: any): Promise<any> {
        console.error('An error occurred', error); // for demo purposes only
        return Promise.reject(error.message || error);
    }
}