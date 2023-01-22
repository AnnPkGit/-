import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { LocalData } from '../globals/localstorage.component';
import { PostModel } from '../globals/PostModelInterfave';

@Component({
  selector: 'app-page',
  templateUrl: './page.component.html',
})
export class PageComponent implements OnInit {
  public postId: string = '';
  public post: PostModel = {} as PostModel;
  public comments: PostModel[] = [];
  public eldestDate: string = '';
  newDataRequested: boolean = false;

  constructor(private route: ActivatedRoute, private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.postId = params.get('id') || '';
    });

    var token = LocalData.isAuthorized() ? LocalData.GetUserToken() : '';
    this.http.get<any>(this.baseUrl + 'comment' + `?token=${token}` + `&postId=${this.postId}`).subscribe(result => {
      this.comments = result["comments"];
      this.eldestDate = result["eldestDate"];
      console.log(result);
    }, error => console.error(error));

    this.http.get<PostModel>(this.baseUrl + `content/${this.postId}?token=${LocalData.GetUserToken()}`).subscribe(result => {
      this.post = result;
    }, error => console.error(error));
  }

  public AddLike(like: boolean, commentId: string, type: string) {
    if (!LocalData.isAuthorized()) return;

    var params = `?token=${LocalData.GetUserToken()}&postId=${commentId}&like=${like}`;
    this.http?.get<string>(this.baseUrl + `reaction/${type}` + params).subscribe(result => {
      if (result == "Ok") {
        if (type == "comment") {
          var commentToUpdate = this.comments.find(e => e.id == commentId) || {} as PostModel;
          if (like) {
            commentToUpdate.likesCount += 1;
            commentToUpdate.isLiked = true;
          }
          if (!like) {
            commentToUpdate.likesCount -= 1;
            commentToUpdate.isLiked = false;
          }
        }
        if (type == "post") {
          if (like) {
            this.post.likesCount += 1;
            this.post.isLiked = true;
          }
          if (!like) {
            this.post.likesCount -= 1;
            this.post.isLiked = false;
          }
        }
      }
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
        this.http?.get<any>(this.baseUrl + 'comment' + `?startTime=${this.eldestDate}` + `&token=${token}` + `&postId=${this.postId}`).subscribe(result => {
          console.log(result);
          if (result["eldestDate"] != "0001-01-01T00:00:00") {
            this.comments = this.comments.concat(result["comments"]);
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
}
