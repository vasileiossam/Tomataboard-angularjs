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
                scope.category = 0;
                scope.todoList = {};
                scope.todoList.todos = [];

                scope.todoList.addTodo = function () {
                    if (scope.todoList.todoText) {
                        scope.todoList.todos.push(
                        {
                            category: scope.category,
                            date: new Date(),
                            text: scope.todoList.todoText,
                            done: false
                            
                        });
                        scope.todoList.todoText = "";
                    }
                };

                scope.todoList.remaining = function () {
                    var count = 0;
                    angular.forEach(scope.todoList.todos, function (todo) {
                        if (scope.category === 0) {
                            count += todo.done ? 0 : 1;
                        }
                        else
                            if (scope.category === todo.category) {
                                count += todo.done ? 0 : 1;
                            }
                    });
                    return count;
                };

                scope.todoList.total = function () {
                    if (scope.category === 0) {
                        return scope.todoList.todos.length;
                    }

                    var count = 0;
                    angular.forEach(scope.todoList.todos, function (todo) {
                        if (scope.category === todo.category) {
                            count += 1;
                        }
                    });

                    return count;
                };

                scope.todoList.archive = function () {
                    var oldTodos = scope.todoList.todos;
                    scope.todoList.todos = [];
                    angular.forEach(oldTodos, function (todo) {
                        if (!todo.done) scope.todoList.todos.push(todo);
                    });
                };

                scope.filter = function (todo) {
                    if (scope.category === 0) return true;
                    return todo.category === scope.category;
                };
            }
        };
    });

})();