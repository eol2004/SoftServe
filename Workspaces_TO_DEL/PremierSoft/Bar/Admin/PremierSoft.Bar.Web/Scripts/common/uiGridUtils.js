﻿angular.module('platformWebApp')
    .config(['$provide', 'uiGridConstants', function ($provide, uiGridConstants) {
        $provide.decorator('GridOptions', ['$delegate', '$localStorage', '$translate', 'platformWebApp.bladeNavigationService', function ($delegate, $localStorage, $translate, bladeNavigationService) {
            var gridOptions = angular.copy($delegate);
            gridOptions.initialize = function (options) {
                var initOptions = $delegate.initialize(options);
                var blade = bladeNavigationService.currentBlade;
                var $scope = blade.$scope;

                // restore saved state, if any
                var savedState = $localStorage['gridState:' + blade.template];
                if (savedState) {
                    // extend saved columns with custom columnDef information (e.g. cellTemplate, displayName)
                    var foundDef;
                    _.each(savedState.columns, function (x) {
                        if (foundDef = _.findWhere(initOptions.columnDefs, { name: x.name })) {
                            foundDef.sort = x.sort;
                            foundDef.width = x.width || foundDef.width;
                            _.extend(x, foundDef);
                            x.wasPredefined = true;
                            initOptions.columnDefs.splice(initOptions.columnDefs.indexOf(foundDef), 1);
                        } else {
                            x.wasPredefined = false;
                        }
                    });
                    // savedState.columns = _.reject(savedState.columns, function (x) { return x.cellTemplate && !x.wasPredefined; }); // not sure why was this, but it rejected custom templated fields
                    initOptions.columnDefs = _.union(initOptions.columnDefs, savedState.columns);
                } else {
                    // mark predefined columns
                    _.each(initOptions.columnDefs, function (x) { x.wasPredefined = true; })
                }
                
                // translate headers
                _.each(initOptions.columnDefs, function (x) { x.headerCellFilter = 'translate'; });

                var customOnRegisterApiCallback = initOptions.onRegisterApi;

                angular.extend(initOptions, {
                    rowHeight: initOptions.rowHeight === 30 ? 40 : initOptions.rowHeight,
                    enableGridMenu: true,
                    //enableVerticalScrollbar: uiGridConstants.scrollbars.NEVER,
                    //enableHorizontalScrollbar: uiGridConstants.scrollbars.NEVER,
                    saveFocus: false,
                    saveFilter: false,
                    saveGrouping: false,
                    savePinning: false,
                    saveSelection: false,
                    gridMenuTitleFilter: $translate,
                    onRegisterApi: function (gridApi) {
                        //set gridApi on scope
                        $scope.gridApi = gridApi;

                        if (gridApi.saveState) {
                            if (savedState) {
                                //$timeout(function () {
                                gridApi.saveState.restore($scope, savedState);
                                //}, 10);
                            }

                            if (gridApi.colResizable)
                                gridApi.colResizable.on.columnSizeChanged($scope, saveState);
                            if (gridApi.colMovable)
                                gridApi.colMovable.on.columnPositionChanged($scope, saveState);
                            gridApi.core.on.columnVisibilityChanged($scope, saveState);
                            gridApi.core.on.sortChanged($scope, saveState);
                            function saveState() {
                                $localStorage['gridState:' + blade.template] = gridApi.saveState.save();
                            }
                        }

                        gridApi.grid.registerDataChangeCallback(processMissingColumns, [uiGridConstants.dataChange.ROW]);

                        if (customOnRegisterApiCallback) {
                            customOnRegisterApiCallback(gridApi);
                        }
                    }
                });

                return initOptions;
            };

            function processMissingColumns(grid) {
                var gridOptions = grid.options;

                if (!gridOptions.columnDefsGenerated && _.any(grid.rows)) {
                    var filteredColumns = _.filter(_.pairs(grid.rows[0].entity), function (x) {
                        return !x[0].startsWith('$$') && !_.isObject(x[1]);
                    });

                    var allKeysFromEntity = _.map(filteredColumns, function (x) {
                        return x[0];
                    });
                    // remove non-existing columns
                    _.each(gridOptions.columnDefs.slice(), function (x) {
                        if (!_.contains(allKeysFromEntity, x.name) && !x.wasPredefined) {
                            gridOptions.columnDefs = _.reject(gridOptions.columnDefs, function (d) {
                                return d.name == x.name;
                            });
                        }
                    });

                    // generate columnDefs for each undefined property
                    _.each(allKeysFromEntity, function (x) {
                        if (!_.findWhere(gridOptions.columnDefs, { name: x })) {
                            gridOptions.columnDefs.push({ name: x, visible: false });
                        }
                    });
                    gridOptions.columnDefsGenerated = true;
                }
            }

            return gridOptions;
        }]);
    }])

    .factory('platformWebApp.uiGridHelper', ['$localStorage', '$timeout', 'uiGridConstants', '$translate', function ($localStorage, $timeout, uiGridConstants, $translate) {
        var retVal = {};
        retVal.uiGridConstants = uiGridConstants;
        retVal.initialize = function ($scope, gridOptions, externalRegisterApiCallback) {
            $scope.gridOptions = angular.extend({
                data: _.any(gridOptions.data) ? gridOptions.data : 'blade.currentEntities',
                onRegisterApi: function (gridApi) {
                    if (externalRegisterApiCallback) {
                        externalRegisterApiCallback(gridApi);
                    }
                }
            }, gridOptions);
        };

        retVal.getSortExpression = function ($scope) {
            var columnDefs = $scope.gridApi ? $scope.gridApi.grid.columns : $scope.gridOptions.columnDefs;
            var sorts = _.filter(columnDefs, function (x) {
                return x.name !== '$path' && x.sort && (x.sort.direction === uiGridConstants.ASC || x.sort.direction === uiGridConstants.DESC);
            });

            sorts = _.sortBy(sorts, function (x) {
                return x.sort.priority;
            });
            sorts = _.map(sorts, function (x) {
                return (x.field ? x.field : x.name) + ':' + (x.sort.direction === uiGridConstants.ASC ? 'asc' : 'desc');
            });
            return sorts.join(';');
        };

        retVal.bindRefreshOnSortChanged = function ($scope) {
            if ($scope.gridApi) {
                $scope.gridApi.core.on.sortChanged($scope, function () {
                    if (!$scope.blade.isLoading) $scope.blade.refresh();
                });
            }
        };

        return retVal;
    }])

    // ui-grid extension service. Used for extension grid options from other modules
    .factory('platformWebApp.ui-grid.extension', [function () {
        return {
            extensionsMap: [],
            registerExtension : function(gridId, extensionFn)
            {
                this.extensionsMap[gridId] = extensionFn;
            },
            tryExtendGridOptions: function(gridId, gridOptions)
            {
                if(this.extensionsMap[gridId])
                {
                    this.extensionsMap[gridId](gridOptions);
                }
            }
        };
    }])

    // auto height and additional class for ui-grid
    .directive('uiGridHeight', ['$timeout', '$window', function ($timeout, $window) {
        return {
            restrict: 'A',
            link: {
                pre: function (scope, element) {
                    var bladeInner = $(element).parents('.blade-inner');
                    bladeInner.addClass('ui-grid-no-scroll');

                    var setGridHeight = function () {
                        $timeout(function () {
                            $(element).height(bladeInner.height());
                        });
                    };
                    scope.$watch('pageSettings.totalItems', setGridHeight);
                    angular.element($window).bind('resize', setGridHeight);
                }
            }
        };
    }])
;