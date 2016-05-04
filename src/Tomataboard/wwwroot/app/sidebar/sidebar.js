(function () {
    "use strict";

    var app = angular.module("dashboard-app");
    
    app.directive("sidebar", function () {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/app/sidebar/sidebar.html',

            link: function (scope, element, attrs) {
                $(document).ready(function () {
                    $('nav').slideAndSwipe();
                });

            }
        };
    });

})();