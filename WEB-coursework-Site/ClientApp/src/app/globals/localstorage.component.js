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
        return window.localStorage.setItem('user', '');
    };
    return LocalData;
}());
exports.LocalData = LocalData;
//# sourceMappingURL=localstorage.component.js.map