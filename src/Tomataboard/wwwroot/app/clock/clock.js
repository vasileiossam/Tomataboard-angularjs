(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("clock",
        ["$interval", "dateFilter", function ($interval, dateFilter) {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/app/clock/clock.html',

            scope: {
                format: "=format"
            },

            link: function (scope, element, attrs) {

                var setTimeFormat = function () {
                    if (scope.format === "12-hour") {
                        scope.timeFormat = "h:mm";
                    }
                    else
                        if (scope.format === "24-hour") {
                            scope.timeFormat = "HH:mm";
                        }
                }
                setTimeFormat();

                // toggle time format
                element.bind("click", function () {
                    if (scope.format === "12-hour") {
                        scope.format = "24-hour";
                    }
                    else
                        if (scope.format === "24-hour") {
                            scope.format = "12-hour";
                        }
                    setTimeFormat();
                    scope.$apply();
                });

                var tick = function () {
                    scope.time = Date.now();
                }
                tick();

                var timeoutId = $interval(tick, 1 * 60 * 1000);

                element.on("$destroy", function () {
                    $interval.cancel(timeoutId);
                });
            }
        };
    }]);

})();