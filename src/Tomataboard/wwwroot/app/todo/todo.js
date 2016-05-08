(function () {
    "use strict";

    var app = angular.module("dashboard-app");

    app.directive("todo", function () {

        return {
            restrict: "E",
            replace: "true",

            templateUrl: "/app/todo/todo.html",

            scope: {
                settings: "="
            },

            link: function (scope, element, attrs) {
                if (scope.settings.category) {
                    $('#todo-pills a[href="#tab' + scope.settings.category + '"]').tab("show");
                }
                
                scope.todos = scope.settings.todos;
                
                scope.addTodo = function () {
                    if (scope.todoText) {
                        scope.todos.push(
                        {
                            category: scope.settings.category,
                            date: new Date(),
                            text: scope.todoText,
                            done: false
                            
                        });
                        scope.todoText = "";
                    }
                };

                scope.remaining = function () {
                    var count = 0;
                    angular.forEach(scope.todos, function (todo) {
                        if (scope.settings.category === 0) {
                            count += todo.done ? 0 : 1;
                        }
                        else
                            if (scope.settings.category === todo.category) {
                                count += todo.done ? 0 : 1;
                            }
                    });
                    return count;
                };

                scope.total = function () {
                    if (scope.settings.category === 0) {
                        return scope.todos.length;
                    }

                    var count = 0;
                    angular.forEach(scope.todos, function (todo) {
                        if (scope.settings.category === todo.category) {
                            count += 1;
                        }
                    });

                    return count;
                };

                scope.archive = function () {
                    var oldTodos = scope.todos;
                    scope.todos = [];
                    angular.forEach(oldTodos, function (todo) {
                        if (!todo.done) scope.todos.push(todo);
                    });
                };

                scope.remove = function(todo) {
                    var index = jQuery.inArray(todo, scope.todos);
                    if (index > -1) {
                        scope.todos.splice(index, 1);
                    }
                }

                scope.filter = function (todo) {
                    if (scope.settings.category === 0) return true;
                    return todo.category === scope.settings.category;
                };

                scope.dragControlListeners = {
                    accept: function (sourceItemHandleScope, destSortableScope) { return true },
                    itemMoved: function (event) { },
                    orderChanged: function (event) { },
                    containment: "#todos",
                    clone: false,
                    allowDuplicates: false
                };
            }
        };
    });

})();