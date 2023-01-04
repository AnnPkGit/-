import { Component } from '@angular/core';

@Component({
  selector: 'app-registration',
  templateUrl: './registration.html',
})

export class Registration {
  public login = "";
  public password = "";
  public passwordRecall = "";
  public secretQuestionAnswear = "";
  public secretQuestion = "";
  public realName  = "";
  public email = "";
  public avatar = "";

  public registration() {
    window.alert("тут должно быть обращение Registration");

  }

  public redirectToAuthorize() {
    window.alert("тут должен быть редщирект к авторизации");
  }
}
