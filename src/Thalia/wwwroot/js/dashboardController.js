$(function () {
    $(function ($) {
        $(".editable").focusout(function () {
            var element = $(this);
            if (!element.text().replace(" ", "").length) {
                element.empty();
            }
        });
    });

    $("#quote a").popover({
        html: "true",
        placement: "top",
        trigger: "hover",
        content: function () {
            return $("#quote-popover").html();
        }
    });

    $("#weather a").popover({
        html: "true",
        placement: "top",
        trigger: "hover",
        content: function () {
            return $("#weather-popover").html();
        }
    });

    $("#photo a").popover({
        html: "true",
        placement: "top",
        trigger: "hover",
        content: function () {
            return $("#photo-popover").html();
        }
    });
});

(function () {
    "use strict";

    angular.module("dashboard-app")
        .controller("dashboardController", dashboardController);

    function dashboardController($scope, $cookies, $http, $interval) {
        var vm = this;
        vm.dashboard = $cookies.getObject("dashboard");
        
        $scope.$watch("[vm.dashboard.name,vm.dashboard.question,vm.dashboard.answer]", function () {
            vm.save();
        }, true);

        if (!vm.dashboard) {
            var offsetMins = new Date().getTimezoneOffset();
            var localMilliseconds = Date.now() - offsetMins * 60 * 1000;

            // get a default dashboard
            $http.get("/api/dashboard/" + localMilliseconds)
                .then(
                    function(response) {
                        // on sucess
                        vm.dashboard = response.data;
                        vm.dashboard.quote = getRandomElement(vm.dashboard.quotes);
                        vm.dashboard.photo = getRandomElement(vm.dashboard.photos);

                        // truncate photo names                    
                        for (var i = 0; i < vm.dashboard.photos.length; i++) {
                            var name = vm.dashboard.photos[i].name;
                            vm.dashboard.photos[i].shortName = name;
                            if (name.length > 20) {
                                vm.dashboard.photos[i].shortName = name.replace(/^(.{20}[^\s]*).*/, "$1");
                                if (vm.dashboard.photos[i].shortName.length < (name.length + 3)) {
                                    vm.dashboard.photos[i].shortName = vm.dashboard.photos[i].shortName + "...";
                                }
                            }
                        }

                        vm.save();
                    },
                    function(error) {
                        // on failure
                        vm.errorMessage = "Failed to load data: " + error;
                    });
        } else {
            vm.dashboard.quote = getRandomElement(vm.dashboard.quotes);
            vm.dashboard.photo = getRandomElement(vm.dashboard.photos);
        }

        // save dashboard to cookie
        vm.save = function () {

            // expires in one year
            var now = new Date(),
            expireDate = new Date(now.getFullYear() + 1, now.getMonth(), now.getDate());
            $cookies.putObject("dashboard", vm.dashboard, {expires: expireDate});
        };

        vm.updateTime = function() {
            var tick = function() {
                vm.time = Date.now();
            }
            tick();
            $interval(tick, 60 * 1000);
        };

        vm.updateTime();
    }

    function getRandomElement(arr) {
        var index = Math.floor(Math.random() * ((arr.length - 1) - 0 + 1)) + 0;
        return arr[index];
    }
   
})();