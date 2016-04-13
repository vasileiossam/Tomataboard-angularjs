(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("weatherWidget", function () {

        return {
            restrict: "E",
            replace: "true",
            templateUrl: '/views/weatherWidget.html',
        };
    });

})();