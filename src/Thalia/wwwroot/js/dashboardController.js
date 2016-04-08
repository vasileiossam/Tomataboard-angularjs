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

        // get any data saved in cookies
        vm.dashboard = $cookies.getObject("dashboard");
        vm.settings = $cookies.getObject("settings");
        vm.isBusy = true;
        vm.errorMessage = "";

        // initialize settings
        if (!vm.settings) {
            vm.settings = {};
            vm.settings.defaultName = "Young Grasshopper";
            vm.settings.name = vm.settings.defaultName;
            vm.settings.defaultQuestion = "What is your goal for today?";
            vm.settings.question = vm.settings.defaultQuestion;
            vm.settings.location = "";
        }

        // watch for changes
        $scope.$watch("[vm.settings.name,vm.settings.question,vm.settings.answer,vm.settings.location]", function () {
            vm.saveSettings();
        }, true);

        vm.saveDashboard = function () {
            // expires in 30 mins
            var expireDate = new Date();
            expireDate.setTime(expireDate.getTime() + (30 * 60 * 1000));
            $cookies.putObject("dashboard", vm.dashboard, { expires: expireDate });
        };

        vm.saveSettings = function () {
            // never expire (10 years)
            var now = new Date();
            var expireDate = new Date(now.getFullYear() + 10, now.getMonth(), now.getDate()); 
            $cookies.putObject("settings", vm.settings, { expires: expireDate });
        };

        // gets a dashboard from server
        vm.getDashboard = function () {
            vm.isBusy = true;
            var offsetMins = new Date().getTimezoneOffset();
            var localMilliseconds = Date.now() - offsetMins * 60 * 1000;

            $http.get("/api/dashboard/" + localMilliseconds)
                .then(
                    function (response) {
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

                        if (!vm.settings.location) {
                            vm.settings.location = vm.dashboard.weather.location;
                        }

                        vm.saveDashboard();
                    },
                    function (error) {
                        // on failure
                        vm.errorMessage = "Failed to load data: " + error;
                    })
                    .finally(function () {
                        vm.isBusy = false;
                    });
        }

        vm.updateTime = function() {
            var tick = function() {
                vm.time = Date.now();
            }

            tick();
            $interval(tick, 1 * 60 * 1000);
        };
        vm.updateTime();

        // setup the interval to refresh the dashboard in 30 mins
        $interval(vm.getDashboard, 30 * 60 * 1000);

        if (!vm.dashboard) {
            vm.getDashboard();
        } else {
            vm.dashboard.quote = getRandomElement(vm.dashboard.quotes);
            vm.dashboard.photo = getRandomElement(vm.dashboard.photos);
            vm.isBusy = false;
        }
    }

    function getRandomElement(arr) {
        var index = Math.floor(Math.random() * ((arr.length - 1) - 0 + 1)) + 0;
        return arr[index];
    }
   
})();