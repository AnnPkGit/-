import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { LocalData } from '../app/globals/localstorage.component';
import { Menu } from './menu/menu'

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html'
})
export class AppComponent {
  title = 'app';

  constructor(private router: Router) { }

  public redirectToAuth() {
    console.log("redirectToAuth");
    this.router.navigate(['/authorization']);
  }
  public redirectToMain() {

    this.router.navigate(['']);
  }
  public redirectToProfile() {

    this.router.navigate(['/profile']);
  }

  public isAuthorized(): boolean {
    console.log("isAuthorized");
    return LocalData.isAuthorized();
  }

  public unAuthorize() {
    console.log("unAuthorize");
    LocalData.unAuthorize();
    this.router.navigate(['']);
  }
}
