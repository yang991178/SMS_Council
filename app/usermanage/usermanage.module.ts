import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';

import { AppComponent }  from './usermanage.component';
import { UserService } from '../user.service.js';

@NgModule({
  imports:      [ BrowserModule, FormsModule, HttpModule ],
  declarations: [ AppComponent ],
  providers: [ UserService ],
  bootstrap:    [ AppComponent ]
})
export class AppModule { }
