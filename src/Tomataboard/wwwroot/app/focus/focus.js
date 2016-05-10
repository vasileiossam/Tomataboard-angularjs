(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("focus", function () {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/app/focus/focus.html',

            link: function(scope, element, attrs) {

            }
        };
    });

})();