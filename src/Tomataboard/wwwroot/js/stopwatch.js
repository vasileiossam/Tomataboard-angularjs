(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("stopwatch", function ($interval) {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/views/stopwatch.html',

            link: function (scope, element, attrs) {
                var promise;
                var seconds = 0;

                var tick = function () {
                    seconds = seconds + 1;
                    var date = new Date(null);
                    date.setSeconds(seconds);
                    scope.time = date.toISOString().substr(11, 8);
                }

                // toggle start/stop
                scope.start = function () {
                    $interval.cancel(promise);
                    promise = 0;

                    if (scope.startText === "STOP") {
                        scope.startText = "START";
                    }
                    else if (scope.startText === "START") {
                        scope.startText = "STOP";
                        promise = $interval(tick, 1 * 1000);
                    }
                };
                
                scope.reset = function () {
                    scope.time = "00:00:00";
                    scope.startText = "START";
                    if (promise) {
                        $interval.cancel(promise);
                        promise = 0;
                        seconds = 0;
                    }
                };

                scope.reset();

                var timeElement = angular.element(element[0].querySelector('.time'));
                timeElement.bind("click", function () {
                    scope.start();
                });

                element.on("$destroy", function () {
                    $interval.cancel(promise);
                });
            }
        };
    });

})();