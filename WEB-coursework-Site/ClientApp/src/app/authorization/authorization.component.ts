import { Component } from '@angular/core';

@Component({
  selector: 'app-authorization',
  templateUrl: './authorization.component.html',
})

export class AuthorizationComponent {
  public login: string = "";
  public password = "";

  public authorize() {
    window.alert("тут должно быть обращение к User контроллеру");
  }

  public redirectToRegistration() {
    window.alert("тут должен быть редщирект к регистрации");
  }
}
