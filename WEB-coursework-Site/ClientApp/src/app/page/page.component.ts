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

  constructor(private route: ActivatedRoute, private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe((params) => {
      this.postId = params.get('id') || '';
    });

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
        if (type == "post")
          if (like) {
            this.post.likesCount += 1;
            this.post.isLiked = true;
          }
          if (!like) {
            this.post.likesCount -= 1;
            this.post.isLiked = false;
          }
        }
    }, error => console.error(error));
  }
}
