import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { LocalData } from '../globals/localstorage.component';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent {
  public posts: PostModel[] = [];
  public eldestDate: string = '';
  public imagesStoragePaths: string = '';
  newDataRequested: boolean = false;
  http: HttpClient | undefined;
  baseUrl: string = '';

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.imagesStoragePaths = '../assets/Images/PostImages/';
    this.http = http;
    this.baseUrl = baseUrl + 'content';
    http.get<any>(this.baseUrl).subscribe(result => {
      this.posts = result["postModels"];
      this.eldestDate = result["eldestDate"];
    }, error => console.error(error));
  }

  onScroll($event: any) {
    if ($event.target.offsetHeight + $event.target.scrollTop >= $event.target.scrollHeight) {
      console.log("End");

      if (this.eldestDate == '')
        return;

      if (this.newDataRequested == false) {
        this.newDataRequested = true;

        this.http?.get<any>(this.baseUrl + `?startTime=${this.eldestDate}`).subscribe(result => {
          this.posts = this.posts.concat(result["postModels"]);

          if (result["eldestDate"] != "0001-01-01T00:00:00") {
            this.eldestDate = result["eldestDate"];
          }
          else {
            setTimeout(() => this.releaseDataRequest(), 10000);
            return;
          }

          setTimeout(() => this.releaseDataRequest(), 10);
        }, error => console.error(error));
      }
    }
  }

  releaseDataRequest() {
    this.newDataRequested = false;
    console.log("data request avaliable again");
  }

  public isAuthorized(): boolean {
    return LocalData.isAuthorized();
  }
}

interface PostModel {
  id: string;
  text: string;
  likesCount: number;
  commentsCount: number;
  date: string;
  authorName: string;
  authorAvatar : string;
  images: string[];
}
