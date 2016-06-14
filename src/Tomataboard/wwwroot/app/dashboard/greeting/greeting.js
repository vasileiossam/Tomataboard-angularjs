(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("greeting", function () {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: '/app/dashboard/greeting/greeting.html',

            scope: {
                settings: "=",
                text: "=",
            },

            link: function(scope, element, attrs) {
                scope.name = "";
       
                if (scope.settings.names) {
                    var names = scope.settings.names.split(",");
                    scope.name = getRandomElement(names);
                }

                function getRandomElement(arr) {
                    var index = Math.floor(Math.random() * ((arr.length - 1) - 0 + 1)) + 0;
                    return arr[index];
                }
            }
        };
    });

})();