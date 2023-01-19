import { HttpClient } from '@angular/common/http';
import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
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

    this.http.get<PostModel>(this.baseUrl + `content/${this.postId}`).subscribe(result => {
      this.post = result;
    }, error => console.error(error));
  }
}
