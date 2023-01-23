import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'profile',
  templateUrl: './profile.html',

})
export class Profile {
  public imagesStoragePaths: string = '';
  newDataRequested: boolean = false;
  http: HttpClient | undefined;
  baseUrl: string = '';
  login: string = '';

  authorAvatar: string = '';
  authorBanner: string[] = [''];


  public changeOption() {
    console.log("Option change here");
  }
}
