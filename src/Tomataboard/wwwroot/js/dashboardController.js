$(function () {
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

    function dashboardController($scope, $cookies, $localStorage, $http, $interval, $timeout) {
        var vm = this;

        // get any persisted data 
        vm.dashboard = $cookies.getObject("dashboard");
        vm.settings = $localStorage.settings;
        vm.isBusy = true;
        vm.errorMessage = "";

        $localStorage.settings = vm.settings;
            
        // initialize settings
        if (!vm.settings) {
            vm.settings = {};
            vm.settings.defaultName = "Young Grasshopper";
            vm.settings.name = vm.settings.defaultName;
            vm.settings.defaultQuestion = "What is your goal for today?";
            vm.settings.question = vm.settings.defaultQuestion;
            vm.settings.location = "";
            vm.settings.showTime = true;
            vm.settings.timeFormat = '12-hour';
            vm.settings.showFocus = true;
            vm.settings.showWeather = true;
            vm.settings.temperatureUnits = 'fahrenheit';
            vm.settings.showQuote = true;
            vm.settings.showGreeting = true;
        }

        // watch for changes
        $scope.$watch("[vm.settings]", function () {
            vm.saveSettings();
        }, true);

        vm.saveDashboard = function () {
            // expires in 30 mins
            var expireDate = new Date();
            expireDate.setTime(expireDate.getTime() + (30 * 60 * 1000));
            $cookies.putObject("dashboard", vm.dashboard, { expires: expireDate });
        };

        vm.saveSettings = function () {
            $localStorage.settings = vm.settings;
        };

        vm.getNextElementIndex = function (array, index) {
            index = index + 1;
            if (index >= array.length) index = 0;
            return index;
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

                        vm.dashboard.quoteIndex = vm.getNextElementIndex(vm.dashboard.quotes, 0);
                        vm.dashboard.quote = vm.dashboard.quotes[vm.dashboard.quoteIndex];

                        vm.dashboard.photoIndex = vm.getNextElementIndex(vm.dashboard.photos, 0);
                        vm.dashboard.photo = vm.dashboard.photos[vm.dashboard.photoIndex];

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

        vm.refresh = function () {
            if (!vm.dashboard) {
                vm.getDashboard();
            } else {
                vm.dashboard.quoteIndex = vm.getNextElementIndex(vm.dashboard.quotes, vm.dashboard.quoteIndex);
                vm.dashboard.quote = vm.dashboard.quotes[vm.dashboard.quoteIndex];

                vm.dashboard.photoIndex = vm.getNextElementIndex(vm.dashboard.photos, vm.dashboard.photoIndex);
                vm.dashboard.photo = vm.dashboard.photos[vm.dashboard.photoIndex];

                vm.saveDashboard();

                vm.isBusy = false;
            }
        }
        vm.refresh();

        vm.startFade = false;
        vm.fadeAndRefresh = function () {
            vm.startFade = true;
            $timeout(function () {
                vm.startFade = false;
                vm.refresh();
            }, 1000);
        };
    }

    //function getRandomElement(arr) {
    //    var index = Math.floor(Math.random() * ((arr.length - 1) - 0 + 1)) + 0;
    //    return arr[index];
    //}
   
})();