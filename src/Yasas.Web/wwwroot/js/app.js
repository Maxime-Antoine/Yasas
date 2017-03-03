(function () {
    var app = angular.module('YASAS', ['ngRoute']);

    app.config(["$routeProvider", function ($routeProvider) {
        $routeProvider
            .when('/', {
                name: 'home',
                templateUrl: 'templates/home.html',
            })
            .when('/login', {
                name: 'login',
                templateUrl: 'templates/login.html',
            })
            .when('/admin', {
                name: 'admin',
                templateUrl: 'templates/admin.html',
                authorize: true,
                requiredRoles: ['admin']
            })
            .otherwise({
                redirectTo: '/',
            })
    }]);

    app.run(['$rootScope','$location','authenticationSvc', function ($rootScope, $location, authenticationSvc) {
        $rootScope.$on('$routeChangeStart', function (event, next, current) {
            if (!authenticationSvc.isLoggedIn() && next.authorize)
                if (next.name !== 'login')
                    $location.path('/login');
        });
    }]);
})()