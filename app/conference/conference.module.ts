import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule, Routes } from '@angular/router';
import { Location, LocationStrategy, PathLocationStrategy } from '@angular/common';

import { AppComponent } from './conference.component';
import { VoteListComponent } from './vote-list.component';
import { VoteDetailComponent } from './vote-detail.component';
import { UserService } from '../user.service.js';
import { ConferenceService } from '../conference.service.js';

const routes: Routes = [
    { path: 'conference/:id', component: VoteListComponent },
    { path: 'conference/:id/vote/:vid', component: VoteDetailComponent }
]

@NgModule({
    imports: [BrowserModule, FormsModule, HttpModule, RouterModule.forRoot(routes)],
    declarations: [AppComponent, VoteListComponent, VoteDetailComponent],
    providers: [UserService, ConferenceService, Location, { provide: LocationStrategy, useClass: PathLocationStrategy }],
    bootstrap: [AppComponent]
})
export class AppModule { }
