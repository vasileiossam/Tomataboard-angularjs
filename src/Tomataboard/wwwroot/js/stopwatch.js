(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("stopwatch", function ($interval) {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/views/stopwatch.html',

            link: function (scope, element, attrs) {
                var timeoutId;
                var seconds = 0;

                var tick = function () {
                    seconds = seconds + 1;
                    var date = new Date(null);
                    date.setSeconds(seconds);
                    scope.time = date.toISOString().substr(11, 8);
                }

                scope.start = function () {
                    if (timeoutId) {
                        scope.reset();
                    } else {
                        scope.startText = "STOP";
                        timeoutId = $interval(tick, 1 * 1000);
                    }
                };

                scope.reset = function () {
                    scope.time = "00:00:00";
                    scope.startText = "START";
                    if (timeoutId) {
                        $interval.cancel(timeoutId);
                        timeoutId = 0;
                        seconds = 0;
                    }
                };

                scope.reset();
                var state = 0;
                


                element.on("$destroy", function () {
                    $interval.cancel(timeoutId);
                });
            }
        };
    });

})();