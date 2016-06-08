(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("countdown", ["$interval", "ngAudio", function ($interval, ngAudio) {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: "/app/dashboard/countdown/countdown.html",

            scope: {
                settings: "="
            },

            link: function(scope, element, attrs) {
                scope.promise = {};
                scope.time = {};
                var seconds;
                scope.settings.endDate = moment(scope.settings.endDate);

                var calcSeconds = function() {
                    seconds = 0;

                    var now = new Date();
                    if (scope.settings.endDate > now) {
                        seconds = (scope.settings.endDate - now) / 1000;
                    }
                };

                var updateTime = function() {

                    var zeroTime = {
                        'days': 0,
                        'hours': 0,
                        'minutes': 0,
                        'seconds': 0
                    }
                    if (!scope.settings.endDate) {
                        scope.time = zeroTime;
                        return;
                    }

                    var distance = moment(scope.settings.endDate) - new Date();
                    if (distance < 0) {
                        scope.time = zeroTime;
                        return;
                    }

                    var _second = 1000;
                    var _minute = _second * 60;
                    var _hour = _minute * 60;
                    var _day = _hour * 24;

                    // http://www.sitepoint.com/build-javascript-countdown-timer-no-dependencies/
                    var days = Math.floor(distance / _day);
                    var hours = Math.floor((distance % _day) / _hour);
                    var minutes = Math.floor((distance % _hour) / _minute);
                    var secs = Math.floor((distance % _minute) / _second);

                    scope.time = {
                        "days": days,
                        "hours": hours,
                        "minutes": minutes,
                        "seconds": secs
                    };
                };

                var tick = function() {
                    seconds = seconds - 1;
                    if (seconds <= 0) {
                        scope.settings.started = false;
                        scope.reset();
                    } else {
                        updateTime();
                    }
                }

                // toggle start/pause
                scope.start = function () {
                    $interval.cancel(scope.promise);
                    scope.promise = 0;

                    if (scope.startText === "PAUSE") {
                        scope.settings.started = false;
                        scope.startText = "START";
                    } else if (scope.startText === "START") {
                        calcSeconds();
                        if (seconds > 0) {
                            scope.settings.started = true;
                            scope.startText = "PAUSE";
                            scope.promise = $interval(tick, 1 * 1000);
                        }
                    }
                };

                scope.reset = function () {
                    $interval.cancel(scope.promise);
                    scope.promise = 0;
                    seconds = 0;
                    updateTime();
                    scope.startText = "START";
                };
                scope.reset();

                // autostart if not reached endDate yet
                calcSeconds();
                if (scope.settings.started) {
                    if (seconds > 0) {
                        scope.start();
                    }
                }

                var timeElements = element[0].querySelectorAll('.times');
                for (var index = 0; index < timeElements.length; ++index) {
                    var ele = angular.element(timeElements[index]);
                    ele.bind("click",
                        function () {
                            scope.start();
                        });
                }

                element.on("$destroy", function () {
                    $interval.cancel(scope.promise);
                });
            }
        };
    }]);

})();