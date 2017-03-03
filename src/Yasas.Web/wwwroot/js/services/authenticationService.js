(function () {
    var app = angular.module('YASAS');

    app.service('authenticationSvc', function () {
        var loggedIn = false;

        this.login = function () {
            loggedIn = true;
        };

        this.logout = function () {
            loggedIn = false;
        };

        this.isLoggedIn = function () {
            return loggedIn;
        }
    });
})();