import { Injectable } from "@angular/core";
import { BehaviorSubject } from 'rxjs/BehaviorSubject';
import { Headers, Http, Response } from '@angular/http';

import 'rxjs/add/operator/toPromise';

import { User } from './user';
import { Vote } from './vote';
import { Conference } from './conference';

@Injectable()
export class ConferenceService {
    voteList = new BehaviorSubject<Vote[]>([]);
    userList = new BehaviorSubject<User[]>([]);
    conferenceId = new BehaviorSubject<number>(null);
    voteListStream$ = this.voteList.asObservable();
    userListStream$ = this.userList.asObservable();
    conferenceIdStream = this.conferenceId.asObservable();
    setVoteList(list: Vote[]) {
        this.voteList.next(list);
    }
    setUserList(list: User[]) {
        this.userList.next(list);
    }
    setConferenceId(id: number) {
        this.conferenceId.next(id);
    }

    constructor(private http: Http) { }

    listConferences(): Promise<Conference[]> {
        return this.http.get("/ajax/Conference.ashx?action=listconferences")
            .toPromise()
            .then(response => response.json() as Conference[])
            .catch(this.handleError);
    }

    addConference(name: string, ratio: string, cparticipants: string, cnum: number, uparticipants: string, unum: number, state: number): Promise<number> {
        return this.http.post("/ajax/Conference.ashx?action=addconference", "name=" + encodeURIComponent(name) + "&ratio=" + ratio + "&cparticipants=" + cparticipants + "&cnum=" + cnum +"&uparticipants=" + uparticipants + "&unum= " + unum + "&state=" + state, { headers: this.headers })
            .toPromise()
            .then(response => parseInt(response.text()) as number)
            .catch(this.handleError);
    }

    deleteConference(id: number): Promise<number> {
        return this.http.post("/ajax/Conference.ashx?action=deleteconference", "id=" + id, { headers: this.headers })
            .toPromise()
            .then(response => parseInt(response.text()) as number)
            .catch(this.handleError)
    }

    updateConference(id: number, name: string, ratio: string, cparticipants: string, cnum: number, uparticipants: string, unum: number, state: number): Promise<number> {
        return this.http.post("/ajax/Conference.ashx?action=updateconference", "id=" + id + "&name=" + encodeURIComponent(name) + "&ratio=" + ratio + "&cparticipants=" + cparticipants + "&cnum=" + cnum + "&uparticipants=" + uparticipants + "&unum= " + unum + "&state=" + state, { headers: this.headers })
            .toPromise()
            .then(response => parseInt(response.text()) as number)
            .catch(this.handleError);
    }

    saveVote(id: number, title: string): Promise<number> {
        return this.http.post("/ajax/Conference.ashx?action=addvote", "id=" + id + "&title=" + encodeURIComponent(title), { headers: this.headers })
            .toPromise()
            .then(response => parseInt(response.text()) as number)
            .catch(this.handleError)
    }

    deleteVote(id: number): Promise<number> {
        return this.http.post("/ajax/Conference.ashx?action=deletevote", "id=" + id, { headers: this.headers })
            .toPromise()
            .then(response => parseInt(response.text()) as number)
            .catch(this.handleError)
    }

    listVotes(conference: number): Promise<Vote[]> {
        return this.http.get("/ajax/Conference.ashx?action=listvotes&conference=" + conference)
            .toPromise()
            .then(response => response.json() as Vote[])
            .catch(this.handleError);
    }

    getVote(id: number): Promise<Vote> {
        return this.http.get("/ajax/Conference.ashx?action=getvote&id=" + id)
            .toPromise()
            .then(response => response.json()[0] as Vote)
            .catch(this.handleError);
    }

    setVoteState(id: number, state: boolean): Promise<number> {
        return this.http.post("/ajax/Conference.ashx?action=setvotestate", "id=" + id + "&active=" + state, { headers: this.headers })
            .toPromise()
            .then(response => parseInt(response.text()) as number)
            .catch(this.handleError);
    }

    vote(id: number, decision: string): Promise<number> {
        return this.http.post("/ajax/Conference.ashx?action=vote", "id=" + id + "&decision=" + decision, { headers: this.headers })
            .toPromise()
            .then(response => parseInt(response.text()) as number)
            .catch(this.handleError);
    }

    adminVote(id: number, decision: string, uid: number): Promise<number> {
        return this.http.post("/ajax/Conference.ashx?action=vote", "id=" + id + "&decision=" + decision + "&uid=" + uid, { headers: this.headers })
            .toPromise()
            .then(response => parseInt(response.text()) as number)
            .catch(this.handleError);
    }

    private headers = new Headers({ 'Content-Type': 'application/x-www-form-urlencoded' });

    private handleError(error: any): Promise<any> {
        console.error('An error occurred', error); // for demo purposes only
        return Promise.reject(error.message || error);
    }
}