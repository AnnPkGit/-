"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.LocalData = void 0;
var LocalData = /** @class */ (function () {
    function LocalData() {
    }
    LocalData.isAuthorized = function () {
        return window.localStorage.getItem('user') == '' ? false : true;
    };
    LocalData.unAuthorize = function () {
        var _a, _b;
        var userLogin = (_a = window.localStorage.getItem('user')) !== null && _a !== void 0 ? _a : '';
        if (userLogin != '') {
            window.localStorage.setItem('user', '');
        }
        var token = (_b = window.localStorage.getItem(userLogin + "_token")) !== null && _b !== void 0 ? _b : '';
        if (token != '') {
            window.localStorage.setItem(userLogin + "_token", '');
        }
    };
    LocalData.saveAuthorized = function (userLogin, userToken) {
        window.localStorage.setItem('user', userLogin);
        window.localStorage.setItem(userLogin + "_token", userToken);
    };
    LocalData.GetUserLogin = function () {
        var _a;
        return (_a = window.localStorage.getItem('user')) !== null && _a !== void 0 ? _a : '';
    };
    LocalData.GetUserToken = function () {
        var _a, _b;
        var userLogin = (_a = window.localStorage.getItem('user')) !== null && _a !== void 0 ? _a : '';
        return (_b = window.localStorage.getItem(userLogin + "_token")) !== null && _b !== void 0 ? _b : '';
    };
    return LocalData;
}());
exports.LocalData = LocalData;
//# sourceMappingURL=localstorage.component.js.map