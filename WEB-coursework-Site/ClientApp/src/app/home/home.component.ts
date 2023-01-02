import { Component } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})

export class HomeComponent {
  public login: string = "";
  public password = "";

  public authorize() {
    window.alert("тут должно быть обращение к User контроллеру"); 
    
  }

  public redirectToRegistration() {
    window.alert("тут должен быть редщирект к регистрации");
  }
}
