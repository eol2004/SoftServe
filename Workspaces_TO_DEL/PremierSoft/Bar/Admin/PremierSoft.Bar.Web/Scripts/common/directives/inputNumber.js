'use strict';

angular.module('platformWebApp')
    .directive('vaNumber', ['$compile', function ($compile) {
        return {
            restrict: 'A',
            scope: {
                min: '=',
                max: '=',
                step: '=',
                nullable: '='
            },
            require: 'ngModel',
            link: function (scope, el, attrs, ctrl) {
                scope.html = '<a class="up" ng-click="sum(\'up\')" data-type="up"></a> <a class="down" ng-click="sum(\'down\')" data-type="down"></a>';

                el.after($compile(scope.html)(scope));

                el.on('keyup', function () {
                    var sum = ctrl.$modelValue;

                    el.val(sum);
                });

                scope.sum = function (type) {
                    var sum = parseFloat(ctrl.$modelValue) || (scope.min > 0 ? scope.min : 0),
                        step = parseFloat(scope.step);

                    if (type == 'up') {
                        sum += step;

                        if (sum > scope.max) {
                            sum = scope.max;
                        }
                    }
                    else {
                        sum -= step;

                        if (sum < scope.min) {
                            sum = scope.min || 0;
                        }
                    }

                    ctrl.$setViewValue(sum);
                    ctrl.$render();
                }

                //It need for support only numeric input
                ctrl.$parsers.push(function (inputValue) {
                    if (scope.nullable && inputValue === "")
                        return null;

                    var floatValue = parseFloat(inputValue) || (scope.min > 0 ? scope.min : 0);

                    return floatValue;
                });
            }
        };
    }])
;