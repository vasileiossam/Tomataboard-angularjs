(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("contenteditable", function () {
        return {
            restrict: "A",
            require: "ngModel",
            link: function (scope, element, attrs, ngModel) {
                function read() {
                    var html = element.html();
                    html = html.replace(/&nbsp;/g, "\u00a0");
                    ngModel.$setViewValue(html);
                }

                ngModel.$render = function () {
                    element.html(ngModel.$viewValue || "");
                };

                element.bind("keyup change", function () {
                    scope.$apply(read);
                });

                // make sure that empty string will set the default name
                element.bind("blur", function () {
                    var value = ngModel.$viewValue.trim();

                    // <BR> is added automaticall by Firefox
                    if ((!value) || (value === "<br>") || (value === "<BR>")) {
                        value = attrs.defaultvalue;
                        ngModel.$setViewValue(value);
                        ngModel.$render();
                    }
                    scope.$apply(read);
                });

                // prevent Enter key
                element.bind("keydown keypress", function (event) {
                    // erase the default value when start typing
                    var value = ngModel.$viewValue.trim();
                    if (value === attrs.defaultvalue) {
                        ngModel.$setViewValue("");
                        ngModel.$render();
                    };

                    // http://stackoverflow.com/questions/17470790/how-to-use-a-keypress-event-in-angularjs
                    var key = typeof event.which === "undefined" ? event.keyCode : event.which;
                    if (key === 13) {
                        element.blur();
                        event.preventDefault();
                    }
                });

                $(element).focusout(function () {
                    var element = $(this);
                    if (!element.text().replace(" ", "").length) {
                        element.empty();
                    }
                });
            }
        };
    });
})();