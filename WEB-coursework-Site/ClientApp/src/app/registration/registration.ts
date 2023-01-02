import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './registration.html',
})

export class Registration {
  public login: string = "";
  public password = "";
  public passwordRecall = "";
  public secretQuestion: string = "";
  public realName: string = "";
  public email = "";
  public avatar = "";

  public registration() {
    window.alert("тут должно быть обращение Registration");

  }

  public redirectToAuthorize() {
    window.alert("тут должен быть редщирект к авторизации");
  }
}




