(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("photoWidget", function () {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/views/photoWidget.html',

            link: function(scope, element, attrs) {

                $(element).popover({
                    html: "true",
                    placement: "top",
                    trigger: "hover",
                    content: function () {
                        return $("#photo-popover").html();
                    }
                });
            }
        };
    });

})();