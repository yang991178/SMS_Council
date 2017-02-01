import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { Location, LocationStrategy, PathLocationStrategy } from '@angular/common';

import { AppComponent } from './vote-manage.component';
import { UserService } from '../user.service.js';
import { ConferenceService } from '../conference.service.js';

@NgModule({
    imports: [BrowserModule, FormsModule, HttpModule],
    declarations: [AppComponent],
    providers: [UserService, ConferenceService, Location, { provide: LocationStrategy, useClass: PathLocationStrategy }],
    bootstrap: [AppComponent]
})
export class AppModule { }
