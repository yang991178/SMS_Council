import { Component, OnInit } from '@angular/core';
import { Location, LocationStrategy, PathLocationStrategy } from '@angular/common';

import { User } from '../user.js';
import { Vote } from '../vote.js';
import { UserService } from '../user.service.js';
import { ConferenceService } from '../conference.service.js';

@Component({
    selector: 'conference',
    template: `<router-outlet></router-outlet>`
})

export class AppComponent implements OnInit {
    constructor(
        private UserService: UserService,
        private ConferenceService: ConferenceService,
        private location: Location
    ) {  }

    ngOnInit(): void {
        this.ConferenceService.listVotes(parseInt(this.location.path().replace(/^\/conference\//, "").replace(/\/vote\/\d$/, ""))).then(list => this.ConferenceService.setVoteList(list));
        this.UserService.listSequenceUsers().then(list => this.ConferenceService.setUserList(list))
    }
}
