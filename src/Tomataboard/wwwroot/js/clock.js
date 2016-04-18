(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("clock",
        function ($interval, dateFilter) {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/views/clock.html',

            scope: {
                format: "=format"
            },

            link: function (scope, element, attrs) {
                var timeoutId;

                var tick = function () {
                    scope.time = Date.now();
                }
                tick();

                timeoutId = $interval(tick, 1 * 60 * 1000);

                element.on('$destroy', function () {
                    $interval.cancel(timeoutId);
                });
            }
        };
    });

})();