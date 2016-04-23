(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("pomodoro", function ($interval, ngAudio) {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/views/pomodoro.html',

            scope: {
                minutesSelection: "=minutesSelection",
                secondsSelection: "=secondsSelection",
                volumenOn: "=volumenOn"
            },

            link: function (scope, element, attrs) {
                scope.minutesCollection = ["mins"];
                for (var i = 0; i <= 90; i++) {
                    scope.minutesCollection.push(i);
                }
                scope.secondsCollection = ["secs", 0, 10, 15, 20, 25, 30, 35, 40, 45, 50, 55, 60];

                scope.promise = {};
                var seconds;
                var audio = ngAudio.load("/sounds/alarm_beep.wav");

                var calcSeconds = function () {
                    seconds = 0;
                    if (angular.isNumber(scope.minutesSelection)) {
                        seconds = seconds + scope.minutesSelection * 60;
                    }
                    if (angular.isNumber(scope.secondsSelection)) {
                        seconds = seconds + scope.secondsSelection;
                    }
                };

                var updateTime = function () {
                    var date = new Date(null);
                    date.setSeconds(seconds);
                    if (seconds >= 3600) {
                        scope.time = date.toISOString().substr(11, 8);
                    }
                    else
                        if (seconds >= 60) {
                            scope.time = date.toISOString().substr(14, 5);
                        }
                        else
                            if (seconds < 60) {
                                scope.time = date.toISOString().substr(17, 2);
                            }
                };

                var tick = function () {
                    seconds = seconds - 1;
                    if (seconds <= 0) {
                        if (scope.volumeOn) {
                            audio.play();
                        }
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
                        scope.startText = "START";
                    }
                    else if (scope.startText === "START") {
                        if (seconds > 0) {
                            scope.startText = "PAUSE";
                            scope.promise = $interval(tick, 1 * 1000);
                        }
                    }
                };

                scope.reset = function () {
                    $interval.cancel(scope.promise);
                    scope.promise = 0;
                    calcSeconds();
                    updateTime();
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
    });

})();