(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("focusWidget", function () {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/views/focusWidget.html',

            link: function(scope, element, attrs) {

            }
        };
    });

})();