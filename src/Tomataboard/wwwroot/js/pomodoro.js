(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("pomodoro", function ($interval, ngAudio) {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: "/views/pomodoro.html",

            scope: {
                pomodoroTime: "=",
                shortBreak: "=",
                longBreak: "=",
                taskDescription: "=",
                taskPlaceholder: "="
            },
            
            link: function (scope, element, attrs) {
                // states: 1 pomodoro, 2 short break, 3 long break
                var state = 1;

                scope.promise = {};
                var seconds;
                var audioStartPomodoro = ngAudio.load("/sounds/start.wav");
                //var audioStartBreak = ngAudio.load("/sounds/finish1.wav");
                
                scope.startPomodoro = function () {
                    state = 1;
                    scope.reset();
                    scope.start();
                };

                scope.startShortBreak = function () {
                    state = 2;
                    scope.reset();
                    scope.start();
                };

                scope.startLongBreak = function () {
                    state = 3;
                    scope.reset();
                    scope.start();
                };

                var calcSeconds = function () {
                    if (state === 1) {
                        seconds = scope.pomodoroTime * 60;
                    }
                    else
                        if (state === 2) {
                            seconds = scope.shortBreak * 60;
                        }
                        else
                            if (state === 3) {
                                seconds = scope.longBreak * 60;
                            }
                };

                var updateTime = function () {
                    var date = new Date(null);
                    date.setSeconds(seconds);
                    if (seconds >= 60) {
                        scope.time = date.toISOString().substr(14, 5);
                    }
                };

                var finished = function () {
                    if (scope === 1) {
                        if (scope.volumeOn) {
                            audioStartPomodoro.play();
                        }
                    }                     
                }

                var tick = function () {
                    seconds = seconds - 1;
                    if (seconds <= 0) {
                        finished();
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