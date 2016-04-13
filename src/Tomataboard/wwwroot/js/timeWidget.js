(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("timeWidget", function () {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/views/timeWidget.html',

            link: function(scope, element, attrs) {

            }
        };
    });

})();