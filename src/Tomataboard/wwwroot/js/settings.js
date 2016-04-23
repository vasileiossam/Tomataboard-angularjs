(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("settings", function () {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/views/settings.html',

            link: function(scope, element, attrs) {
                var i;
                scope.workCollection = [];
                for (i = 10; i <= 60; i++) {
                    scope.workCollection.push(i);
                }

                scope.shortBreakCollection = [];
                for (i = 1; i <= 30; i++) {
                    scope.shortBreakCollection.push(i);
                }

                scope.longBreakCollection = [];
                for (i = 1; i <= 30; i++) {
                    scope.longBreakCollection.push(i);
                }
            }
        };
    });

})();