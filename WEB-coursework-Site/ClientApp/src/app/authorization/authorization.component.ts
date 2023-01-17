import { HttpClient } from '@angular/common/http';
import { Component, Inject } from '@angular/core';
import { Router } from '@angular/router';
import { LocalData } from '../globals/localstorage.component';

@Component({
  selector: 'app-authorization',
  templateUrl: './authorization.component.html',
})

export class AuthorizationComponent {
  public login: string = "";
  public password: string = "";
  public http: HttpClient | undefined;
  public baseUrl: string = '';
  public error: string = '';
  router: Router | undefined;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, router: Router) {
    this.http = http;
    this.baseUrl = baseUrl;
    this.router = router;
  }

  public authorize() {
    let userData: AuthorizationUserModel = { login: this.login, password: this.password };

    this.http?.post<string>(this.baseUrl + 'user/authorization', userData).subscribe(result => {
      if (result != 'User does not exist') {
        this.error = '';
        LocalData.saveAuthorized(userData.login, result);
        this.router?.navigate(['/']);
      }
      else {
        this.error = result;
      }
      this.login = '';
      this.password = '';
    }, error => console.error(error));
  }

  public isAuthorized(): boolean {
      return LocalData.isAuthorized();
  }

  public unAuthorize() {
    LocalData.unAuthorize();
  }
}

interface AuthorizationUserModel {
  login: string;
  password: string;
}
