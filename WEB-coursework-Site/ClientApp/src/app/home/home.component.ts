import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { LocalData } from '../globals/localstorage.component';
import { PostModel } from '../globals/PostModelInterfave';

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
  public valueText: string = '';
  public error: string = '';

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, private route: Router) {
    this.imagesStoragePaths = '../assets/Images/PostImages/';
    this.http = http;
    this.baseUrl = baseUrl;

    var token = LocalData.isAuthorized() ? LocalData.GetUserToken() : '';
    http.get<any>(this.baseUrl + 'content' + `?token=${token}`).subscribe(result => {
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

        var token = LocalData.isAuthorized() ? LocalData.GetUserToken() : '';
        this.http?.get<any>(this.baseUrl + 'content' + `?startTime=${this.eldestDate}` + `&token=${token}`).subscribe(result => {
          if (result["eldestDate"] != "0001-01-01T00:00:00") {
            this.posts = this.posts.concat(result["postModels"]);
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

  isAuthorized(): boolean {
    return LocalData.isAuthorized();
  }

  addPost() {
    if (this.valueText.trim() == '')
      return;

    let newPost: PostToAddModel = { text: this.valueText, login: LocalData.GetUserLogin(), accessToken: LocalData.GetUserToken()}
    this.http?.post<string>(this.baseUrl + 'content', newPost).subscribe(result => {
      this.valueText = '';
      this.error = '';
      if (result != "Ok") {
        this.error = "Что-то пошло не так";
      }
    }, error => console.error(error));
  }

  public RedirectToPostPage(id: string) {
    console.log(id);
    var path = `/page/${id}`
    this.route?.navigate([path]);
  }

  public AddLike(like: boolean, postId: string) {
    if (!LocalData.isAuthorized()) return;

    var params = `?token=${LocalData.GetUserToken()}&postId=${postId}&like=${like}`;
    this.http?.get<string>(this.baseUrl + "reaction/post" + params).subscribe(result => {
      if (result == "Ok") {

        var postToUpdate = this.posts.find(e => e.id == postId) || {} as PostModel;
        if (typeof postToUpdate != undefined) {
          if (like) {
            postToUpdate.likesCount += 1;
            postToUpdate.isLiked = true;
          }
          else {
            postToUpdate.likesCount -= 1;
            postToUpdate.isLiked = false;
          }
        }

      }
    }, error => console.error(error));
  }
}

interface PostToAddModel {
  text: string;
  login: string;
  accessToken: string;
}
