(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("timer", function () {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/views/timer.html',

            link: function(scope, element, attrs) {

            }
        };
    });

})();