(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("greetingWidget", function () {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/views/greetingWidget.html',

            link: function(scope, element, attrs) {

            }
        };
    });

})();