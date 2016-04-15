(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("photo", function () {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/views/photo.html',

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