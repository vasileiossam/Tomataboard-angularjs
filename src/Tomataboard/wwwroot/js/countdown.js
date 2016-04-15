(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("countdown", function () {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/views/countdown.html',

            link: function(scope, element, attrs) {

            }
        };
    });

})();