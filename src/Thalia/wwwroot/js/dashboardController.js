$(function () {
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

    function dashboardController($cookies, $http, $interval) {
        var vm = this;
    
        vm.dashboard = $cookies.getObject("dashboard");

        if (vm.dashboard) {
           
            var offsetMins = new Date().getTimezoneOffset();
            var localMilliseconds = Date.now() - offsetMins * 60 * 1000;
        
            // get a default dashboard
            $http.get("/api/dashboard/" + localMilliseconds)
                .then(
                function (response) {
                   
                    // on sucess
                    vm.dashboard = response.data;
                    vm.dashboard.quote = vm.dashboard.quotes[vm.dashboard.quoteIndex];
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
                  
                    var tick = function () {
                        vm.time = Date.now();
                    }
                    tick();
                    $interval(tick, 60*1000);
                },
                function (error) {
                    // on failure
                   
                    vm.errorMessage = "Failed to load data: " + error;
                });
        };
    }

    
})();