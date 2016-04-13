(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("quoteWidget", function () {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/views/quoteWidget.html',

            link: function(scope, element, attrs) {

                $(element).popover({
                    html: "true",
                    placement: "top",
                    trigger: "hover",
                    content: function () {
                        return $("#quote-popover").html();
                    }
                });
            }
        };
    });

})();