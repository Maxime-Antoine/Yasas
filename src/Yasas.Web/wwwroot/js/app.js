(function () {
    var app = angular.module('YASAS', ['ngRoute']);

    app.config(["$routeProvider", function ($routeProvider) {
        $routeProvider
            .when('/', {
                name: 'home',
                templateUrl: 'templates/home.html',
                allowAnonymous: true
            })
            .when('/login', {
                name: 'login',
                templateUrl: 'templates/login.html',
                allowAnonymous: true
            })
            .when('/admin', {
                name: 'admin',
                templateUrl: 'templates/admin.html'
            })
            .otherwise({
                redirectTo: '/',
                allowAnonymous: true //allow the REDIRECTION to anybody
            })
    }]);

    app.run(['$rootScope','$location','authenticationSvc', function ($rootScope, $location, authenticationSvc) {
        $rootScope.$on('$routeChangeStart', function (event, next, current) {
            if (!authenticationSvc.isLoggedIn() && !next.allowAnonymous)
                if (next.name !== 'login')
                    $location.path('/login');
        });
    }]);
})()