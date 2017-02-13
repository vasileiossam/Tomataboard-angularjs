(function () {
    "use strict";

    dashboardController.$inject = ["$scope", "$localStorage", "$http", "$interval", "$timeout", "$attrs"];
    angular.module("dashboard-app")
        .controller("dashboardController", dashboardController);

    function dashboardController($scope, $localStorage, $http, $interval, $timeout, $attrs) {
        var vm = this;
        vm.isBusy = true;
        vm.canPost = false;
        vm.watcherCount = 0;

        vm.errorMessage = "";
        // 30 minutes
        vm.refreshTime = 30 * 60 * 1000;
        vm.stopwatchReset = {};

        vm.startFade = false;
        vm.fadeAndRefresh = function () {
            vm.startFade = true;
            $timeout(function () {
                vm.startFade = false;
                vm.refresh();
            }, 1000);
        };

        vm.showTimer = function (selector) {
            // reset stopwatch if its working
            if (selector !== "stopwatch") {
                vm.stopwatchReset();
            }
            vm.settings.activeTimer = selector;
            vm.settings.showTimers = true;
            $('.ssm-overlay').click();
        };

        vm.getNextElementIndex = function (array, index) {
            index = index + 1;
            if (index >= array.length) index = 0;
            return index;
        };

        vm.saveLocalSettings = function () {
            $localStorage.settings = vm.settings;
        };

        vm.saveDashboard = function () {
            var expireDate = new Date();
            expireDate.setTime(expireDate.getTime() + vm.refreshTime);

            $localStorage.expireDate = expireDate;
            $localStorage.dashboard = vm.dashboard;

            
        };

        // GET dashboard
        vm.getDashboard = function () {
            vm.isBusy = true;
            var offsetMins = new Date().getTimezoneOffset();
            // var localMilliseconds = Date.now() - offsetMins * 60 * 1000;
            // $http.get("/api/dashboard/" + localMilliseconds)

            var url = "/api/dashboard-public";
            if ($attrs.username) {
                url = "/api/dashboard";
            }
            
            $http.get(url)
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

        // GET settings
        vm.getSettings = function () {
            
            $http.get("/api/settings")
                     .then(
                         function (response) {
                             // expect response.data.defaultQuestion != null or we have received a "corrupted" saved object
                             if ((response.data) && (response.data.defaultQuestion)) {
                                 vm.settings = angular.merge({}, vm.getDefaultSettings(), response.data);
                                 vm.settings.canSync = true;
                             }
                             vm.init();
                         },
                         function (error) {
                             vm.errorMessage = "Failed to load settings: " + error.statusText;
                         })
                         .finally(function () {
                         });
        };

        vm.getDefaultSettings = function () {
            return {
                defaultQuestion: "What is your goal for today?",
                question: "What is your goal for today?",
                answer: "",
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

                canSync: false
            };
        };

        vm.getLocalSettings = function () {
            var settings = $localStorage.settings;

            // http://davidcai.github.io/blog/posts/copy-vs-extend-vs-merge/
            if (settings) {
                return angular.merge({}, vm.getDefaultSettings(), settings);
            }
            else {
                return vm.getDefaultSettings();
            }
        };

        vm.postSettings = function () {
            // console.log('Posting settings');

            $http.post("/api/settings", vm.settings)
                  .then(
                      function (response) {
                          
                      },
                      function (error) {
                          // on failure
                          vm.errorMessage = "Failed to post settings data: " + error.statusText;
                      })
                      .finally(function () {
                      });
        };

        vm.init = function() 
        {
            // load dashboard and setup the interval to refresh it every 30 mins
            vm.dashboard = $localStorage.dashboard;
            $interval(vm.getDashboard, vm.refreshTime);
         
            vm.refresh();

            // watch for changes
            $scope.$watch("[vm.settings]", function () {
                vm.saveLocalSettings();

                if (($attrs.username) && (vm.watcherCount > 0)) {
                    vm.postSettings();
                }
                vm.watcherCount = vm.watcherCount + 1;
            }, true);
        }
        
        // user is logged in
        if ($attrs.username) {
            vm.getSettings();
        }
        else {
            vm.settings = vm.getLocalSettings();

            // keep the sync logic simple: if user registered an account but currently is not logged in then force him to log in
            if (vm.settings.canSync) {
                window.location.href = "/Account/Login";
            }

            vm.init();
        }

        vm.canPost = true;
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
})();