(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("greeting", function () {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/views/greeting.html',

            link: function(scope, element, attrs) {

            }
        };
    });

})();