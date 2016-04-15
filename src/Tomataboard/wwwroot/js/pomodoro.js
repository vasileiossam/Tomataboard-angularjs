(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("pomodoro", function () {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/views/pomodoro.html',

            link: function(scope, element, attrs) {

            }
        };
    });

})();