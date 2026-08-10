import { NgModule, provideBrowserGlobalErrorListeners } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { MatFabButton, MatMiniFabButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { MatSelectModule } from '@angular/material/select';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
// import { ScrollingModule } from '@angular/cdk/scrolling';

import { AppRoutingModule } from './app-routing-module';
import { Landing_Upload } from './components/landing_upload/landing_upload';

@NgModule({
  declarations: [
    Landing_Upload
  ],
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    AppRoutingModule, 
    MatFabButton,
    MatMiniFabButton,
    MatIcon,
    MatTableModule,
    MatSelectModule,
    MatInputModule,
    MatFormFieldModule
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
  ],
  bootstrap: [Landing_Upload]
})
export class AppModule { }
