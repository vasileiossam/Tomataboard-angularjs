(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("stopwatch", ["$interval", function ($interval) {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/app/stopwatch/stopwatch.html',

            scope: {
                reset: "="
            },

            link: function (scope, element, attrs) {
                scope.promise = {};
                var seconds = 0;

                var tick = function () {
                    seconds = seconds + 1;
                    var date = new Date(null);
                    date.setSeconds(seconds);
                    scope.time = date.toISOString().substr(11, 8);
                }

                // toggle start/pause
                scope.start = function () {
                    $interval.cancel(scope.promise);
                    scope.promise = 0;

                    if (scope.startText === "PAUSE") {
                        scope.startText = "START";
                    }
                    else if (scope.startText === "START") {
                        scope.startText = "PAUSE";
                        scope.promise = $interval(tick, 1 * 1000);
                    }
                };
                
                scope.reset = function () {
                    $interval.cancel(scope.promise);
                    scope.promise = 0;
                    seconds = 0;
                    scope.time = "00:00:00";
                    scope.startText = "START";
                };

                scope.reset();

                var timeElement = angular.element(element[0].querySelector('.time'));
                timeElement.bind("click", function () {
                    scope.start();
                });

                element.on("$destroy", function () {
                    $interval.cancel(scope.promise);
                });
            }
        };
    }]);

})();