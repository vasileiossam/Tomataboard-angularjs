(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("weatherWidget", function () {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/views/weatherWidget.html',

            link: function(scope, element, attrs) {

                $(element).popover({
                    html: "true",
                    placement: "top",
                    trigger: "hover",
                    content: function () {
                        return $("#weather-popover").html();
                    }
                });
            }
        };
    });

})();