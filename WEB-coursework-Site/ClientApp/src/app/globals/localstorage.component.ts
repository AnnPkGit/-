export class LocalData {
  static isAuthorized(): boolean {
    return window.localStorage.getItem('user') == '' ? false : true;
  }

  static unAuthorize() {
    var userLogin = window.localStorage.getItem('user') ?? '';
    if (userLogin != '') {
      window.localStorage.setItem('user', '');
    }
    var token = window.localStorage.getItem(`${userLogin}_token`) ?? '';
    if (token != '') {
      window.localStorage.setItem(`${userLogin}_token`, '');
    }
  }

  static saveAuthorized(userLogin: string, userToken: string) {
    window.localStorage.setItem('user', userLogin);
    window.localStorage.setItem(`${userLogin}_token`, userToken);
  }

  static GetUserLogin(): string {
    return window.localStorage.getItem('user') ?? '';
  }

  static GetUserToken(): string {
    var userLogin = window.localStorage.getItem('user') ?? '';
    return window.localStorage.getItem(`${userLogin}_token`) ?? '';
  }
}
