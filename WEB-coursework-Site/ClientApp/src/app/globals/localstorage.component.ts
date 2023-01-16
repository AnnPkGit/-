export class LocalData {
  static isAuthorized(): boolean {
    return window.localStorage.getItem('user') == '' ? false : true;
  }

  static unAuthorize() {
    return window.localStorage.setItem('user', '');
  }
}
