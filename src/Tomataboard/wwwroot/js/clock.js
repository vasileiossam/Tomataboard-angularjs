(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("clock", function () {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/views/clock.html',

            link: function(scope, element, attrs) {

            }
        };
    });

})();