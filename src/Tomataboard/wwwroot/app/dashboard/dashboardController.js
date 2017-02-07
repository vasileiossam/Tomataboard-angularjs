(function () {
    "use strict";

    dashboardController.$inject = ["$scope", "$localStorage", "$http", "$interval", "$timeout"];
    angular.module("dashboard-app")
        .controller("dashboardController", dashboardController);

    function dashboardController($scope, $localStorage, $http, $interval, $timeout) {
        var vm = this;

        vm.isBusy = true;
        vm.errorMessage = "";
        // 30 minutes
        vm.refreshTime = 30 * 60 * 1000;

        var defaultOptions = {
            defaultQuestion: "What is your goal for today?",
            question: "What is your goal for today?",
            location: "",
            showBackgroundPhoto: true,
            showFocus: true,
            showWeather: true,
            temperatureUnits: "",
            showQuote: true,
            showTodo: false,

            showTimers: true,
            clockFormat: "12-hour",
            activeTimer: "clock",
            volumeOn: true,

            timerMinutesSelection: 5,
            timerSecondsSelection: 0,

            pomodoroTime: 25,
            pomodoroShortBreak: 5,
            pomodoroLongBreak: 15,
            pomodoroTaskPlaceholder: "What task are you working on?",
            pomodoroTaskDescription: "What task are you working on?",
            pomodoroTotal: 0,

            countdown: {
                eventPlaceholder: "Event Name...",
                eventDescription: "Event Name...",
                endDate: new Date(),
                started: false
            },

            todo: {
                category: 0,
                categories: ["Work", "Personal", "Make a difference"],
                todos: []
            },

            greeting: {
                show: true,
                defaultName: "Young Grasshopper",
                name: "Young Grasshopper",
                randomName: false,
                names: "Champion, Winner, Lucky, Fighter, Rock Star",
                namesPlaceholder: "Comma separated names e.g Champion, Winner, Lucky",
            },
            newproperty: 1
        };

        // get any persisted data
        vm.dashboard = $localStorage.dashboard;
        var settings = $localStorage.settings;

        // initialize settings
        if (settings) {
            vm.settings = angular.merge({}, defaultOptions, settings)
        }
        else
        {
            vm.settings = defaultOptions;
        }

        vm.stopwatchReset = {};

        vm.showTimer = function (selector) {
            // reset stopwatch if its working
            if (selector !== "stopwatch") {
                vm.stopwatchReset();
            }
            vm.settings.activeTimer = selector;
            vm.settings.showTimers = true;
            $('.ssm-overlay').click();
        };

        // watch for changes
        $scope.$watch("[vm.settings]", function () {
            vm.saveSettings();
        }, true);

        vm.saveDashboard = function () {
            var expireDate = new Date();
            expireDate.setTime(expireDate.getTime() + vm.refreshTime);

            $localStorage.expireDate = expireDate;
            $localStorage.dashboard = vm.dashboard;
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
            // var localMilliseconds = Date.now() - offsetMins * 60 * 1000;
            // $http.get("/api/dashboard/" + localMilliseconds)

            $http.get("/api/dashboard")
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

                        if (vm.dashboard.weather) {
                            if (!vm.settings.location) {
                                vm.settings.location = vm.dashboard.weather.location;
                            }
                            if (!vm.settings.temperatureUnits) {
                                vm.settings.temperatureUnits = "celsius";
                                if (vm.dashboard.weather.usesFahrenheit) {
                                    vm.settings.temperatureUnits = "fahrenheit";
                                }
                            }
                        }
                        vm.dashboard.greeting = getGreeting();

                        vm.saveDashboard();
                    },
                    function (error) {
                        // on failure
                        vm.errorMessage = "Failed to load data: " + error.statusText;
                    })
                    .finally(function () {
                        vm.isBusy = false;
                    });
        }

        // setup the interval to refresh the dashboard in 30 mins
        $interval(vm.getDashboard, vm.refreshTime);

        vm.refresh = function () {
            if (!vm.dashboard || (vm.expireDate < new Date())) {
                vm.getDashboard();
            } else {
                vm.dashboard.quoteIndex = vm.getNextElementIndex(vm.dashboard.quotes, vm.dashboard.quoteIndex);
                vm.dashboard.quote = vm.dashboard.quotes[vm.dashboard.quoteIndex];

                vm.dashboard.photoIndex = vm.getNextElementIndex(vm.dashboard.photos, vm.dashboard.photoIndex);
                vm.dashboard.photo = vm.dashboard.photos[vm.dashboard.photoIndex];

                vm.saveDashboard();

                vm.isBusy = false;
            }

            if (vm.dashboard) {
                vm.dashboard.greeting = getGreeting();
            }
        };
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

    function getGreeting() {
        var hours = new Date().getHours();

        if (hours >= 5 && hours < 12) {
            return "Good morning";
        }
        if (hours >= 12 && hours < 17) {
            return "Good afternoon";
        }
        if (hours >= 17) {
            return "Good evening";
        }
    }

    //function getRandomElement(arr) {
    //    var index = Math.floor(Math.random() * ((arr.length - 1) - 0 + 1)) + 0;
    //    return arr[index];
    //}
})();