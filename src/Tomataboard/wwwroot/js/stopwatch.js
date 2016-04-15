(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("stopwatch", function () {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/views/stopwatch.html',

            link: function(scope, element, attrs) {

            }
        };
    });

})();