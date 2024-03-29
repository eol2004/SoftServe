﻿var AppDependencies = [
  'ui.router',
  'googlechart',
  'gridster',
  'ui.bootstrap',
  'ui.utils',
  'ui.sortable',
  'ui.select',
  'ngAnimate',
  'ngStorage',
  'ngResource',
  'fiestah.money',
  'ngCookies',
  'angularMoment',
  'angularFileUpload',
  'ngSanitize',
  'ng-context-menu',
  'ui.grid', 'ui.grid.autoResize', 'ui.grid.resizeColumns', 'ui.grid.moveColumns', 'ui.grid.saveState', 'ui.grid.selection', 'ui.grid.pagination', 'ui.grid.pinning', 'ui.grid.grouping', 'ui.grid.edit', 'ui.grid.i18n',
  'ui.grid.draggable-rows',
  'ui.codemirror',
  'focusOn',
  'textAngular',
  'ngTagsInput',
  'pascalprecht.translate',
  'angular.filter',
  'ui.bootstrap.datetimepicker',
  'angularSpectrumColorpicker'
];

angular.module('platformWebApp', AppDependencies).
    controller('platformWebApp.appCtrl', ['$scope', '$window', 'platformWebApp.mainMenuService', 'platformWebApp.pushNotificationService', '$translate', '$timeout', 'platformWebApp.modules', 'platformWebApp.bladeNavigationService', 'platformWebApp.settings', 'platformWebApp.settings.helper', 'i18nService',
        function ($scope, $window, mainMenuService, pushNotificationService, $translate, $timeout, modules, bladeNavigationService, settings, settingsHelper, i18nService) {
      pushNotificationService.run();
      i18nService.setCurrentLang('ru');

      $scope.closeError = function () {
          $scope.platformError = undefined;
      };
      modules.query().$promise.then(function (results) {
          var modulesWithErrors = _.filter(results, function (x) { return _.any(x.validationErrors); });
          if (_.any(modulesWithErrors)) {
              $scope.platformError = {
                  title: modulesWithErrors.length + " modules are loaded with errors and require your attention.",
                  detail: ''
              };
              _.each(modulesWithErrors, function (x) {
                  var moduleErrors = "<br/><br/><b>" + x.id + "</b> " + x.version + "<br/>" + x.validationErrors.join("<br/>");
                  $scope.platformError.detail += moduleErrors;
              });
              $state.go('workspace.modularity');
          }
      });


      $scope.$on('httpError', function (event, error) {
          if (bladeNavigationService.currentBlade) {
              bladeNavigationService.setError(error.status + ': ' + error.statusText, bladeNavigationService.currentBlade);
          }
      });

      $scope.$on('httpRequestSuccess', function (event, data) {
          // clear error on blade cap
          if (bladeNavigationService.currentBlade) {
              bladeNavigationService.currentBlade.error = undefined;
          }
      });

      $scope.$on('loginStatusChanged', function (event, authContext) {
          $scope.isAuthenticated = authContext.isAuthenticated;
      });

      var userProfileSettings;

      var unwatchMenuChangesFn;
      $scope.$on('loginStatusChanged', function (event, authContext) {
          //unsubscribe from previous watch
          //We cannot use global watch because saveMenuState may be executed before menu initialized loaded persisted state
          if (unwatchMenuChangesFn) {
              unwatchMenuChangesFn();
              unwatchMenuChangesFn = undefined;
          }
          //reset menu to default state
          angular.forEach(mainMenuService.menuItems, function (menuItem) { mainMenuService.resetMenuItemDefaults(menuItem); });
          if (authContext.isAuthenticated) {
              settings.getCurrentUserProfile(function (currentUserProfileSettings) {
                  settingsHelper.fixValues(currentUserProfileSettings);
                  userProfileSettings = currentUserProfileSettings;

                  $translate.use(settingsHelper.getSetting(currentUserProfileSettings, "VirtoCommerce.Platform.UI.Language").value);

                  initializeMainMenu(userProfileSettings);

                  unwatchMenuChangesFn = $scope.$watch('mainMenu', function () { saveMenuState($scope.mainMenu, userProfileSettings); }, true);
              });

              $timeout(function () {
                  var currentLanguage = $translate.use();
                  var rtlLanguages = ['ar', 'arc', 'bcc', 'bqi', 'ckb', 'dv', 'fa', 'glk', 'he', 'lrc', 'mzn', 'pnb', 'ps', 'sd', 'ug', 'ur', 'yi'];
                  $scope.isRTL = rtlLanguages.indexOf(currentLanguage) >= 0;
              }, 100);
          }
      });

      $scope.mainMenu = {};
      $scope.mainMenu.items = mainMenuService.menuItems;

      function initializeMainMenu(profileSettings) {
          if (profileSettings) {
              var mainMenuStateSetting = settingsHelper.getSetting(profileSettings, "VirtoCommerce.Platform.UI.MainMenu.State");
              if (mainMenuStateSetting && mainMenuStateSetting.value) {
                  var menuState = angular.fromJson(mainMenuStateSetting.value);
                  $scope.mainMenu.isCollapsed = menuState.isCollapsed;
                  angular.forEach(menuState.items, function (x) {
                      var existItem = mainMenuService.findByPath(x.path);
                      if (existItem) {
                          angular.extend(existItem, x);
                      }
                  });
              }
          }
      }

      function saveMenuState(mainMenu, profileSettings) {
          if (mainMenu && profileSettings) {
              var menuState =
                  {
                      isCollapsed: mainMenu.isCollapsed,
                      items: _.map(_.filter(mainMenu.items,
                                          function (x) { return !x.isAlwaysOnBar; }),
                                          function (x) { return { path: x.path, isCollapsed: x.isCollapsed, isFavorite: x.isFavorite, order: x.order }; }
                                         )
                  };
              var mainMenuStateSetting = settingsHelper.getSetting(profileSettings, "VirtoCommerce.Platform.UI.MainMenu.State");
              mainMenuStateSetting.value = angular.toJson(menuState);
              settings.updateCurrentUserProfile(profileSettings);
          }
      }
  }])
// Specify SignalR server URL (application URL)
.factory('platformWebApp.signalRServerName', ['$location', function ($location) {
    var retVal = $location.url() ? $location.absUrl().slice(0, -$location.url().length - 1) : $location.absUrl();
    return retVal;
}])
.factory('platformWebApp.httpErrorInterceptor', ['$q', '$rootScope', function ($q, $rootScope) {
    var httpErrorInterceptor = {};

    httpErrorInterceptor.request = function (config) {
        // do something on success
        if (!config.cache) {
            $rootScope.$broadcast('httpRequestSuccess', config);
        }
        return config;
    };

    httpErrorInterceptor.responseError = function (rejection) {
        if (rejection.status === 401) {
            $rootScope.$broadcast('unauthorized', rejection);
        }
        else {
            $rootScope.$broadcast('httpError', rejection);
        }
        return $q.reject(rejection);
    };
    httpErrorInterceptor.requestError = function (rejection) {
        $rootScope.$broadcast('httpError', rejection);
        return $q.reject(rejection);
    };

    return httpErrorInterceptor;
}])
.factory('translateLoaderErrorHandler', function ($q, $log) {
    return function (part, lang) {
        $log.error('Localization "' + part + '" for "' + lang + '" was not loaded.');

        //todo add notification.

        //2) You have to either resolve the promise with a translation table for the given part and language or reject it 3) The partial loader will use the given translation table like it was successfully fetched from the server 4) If you reject the promise, then the loader will reject the whole loading process
        return $q.when({});
    };
})
.config(
  ['$stateProvider', '$httpProvider', 'uiSelectConfig', 'datepickerConfig', 'timepickerConfig', '$translateProvider', '$compileProvider',
      function ($stateProvider, $httpProvider, uiSelectConfig, datepickerConfig, timepickerConfig, $translateProvider, $compileProvider) {
          $stateProvider.state('workspace', {
              url: '/workspace',
              templateUrl: '$(Platform)/Scripts/app/workspace.tpl.html'
          });

          //Add interceptor
          $httpProvider.interceptors.push('platformWebApp.httpErrorInterceptor');
          //ui-select set selectize as default theme
          uiSelectConfig.theme = 'select2';

          datepickerConfig.showWeeks = false;
          datepickerConfig.startingDay = 1;
          timepickerConfig.showMeridian = false;
          //datepickerConfig.todayText = 'Сегодня';
          //uiDatetimePickerConfig.nowText = 'Сейчас';
          //uiDatetimePickerConfig.clearText = 'Очистить';
          //uiDatetimePickerConfig.dateText = 'Дата';
          //uiDatetimePickerConfig.timeText = 'Время';
          
          //Localization
          // https://angular-translate.github.io/docs/#/guide
          // var defaultLanguage = settings.getValues({ id: 'VirtoCommerce.Platform.General.ManagerDefaultLanguage' });
          $translateProvider.useUrlLoader('api/platform/localization')
            .useLoaderCache(true)
            .useSanitizeValueStrategy('escapeParameters')
            .preferredLanguage('ru')
            .fallbackLanguage('en')
            .useLocalStorage();

          // Disable Debug Data in DOM ("significant performance boost").
          // Comment the following line while debugging or execute this in browser console: angular.reloadWithDebugInfo();
          $compileProvider.debugInfoEnabled(false);
      }])

.run(
  ['$rootScope', '$state', '$stateParams', 'platformWebApp.authService', 'platformWebApp.mainMenuService', 'platformWebApp.pushNotificationService', '$animate', '$templateCache', 'gridsterConfig', 'taOptions', '$timeout', 'platformWebApp.bladeNavigationService',
    function ($rootScope, $state, $stateParams, authService, mainMenuService, pushNotificationService, $animate, $templateCache, gridsterConfig, taOptions, $timeout, bladeNavigationService) {
        //Disable animation
        $animate.enabled(false);

        $rootScope.$state = $state;
        $rootScope.$stateParams = $stateParams;

        var homeMenuItem = {
            path: 'home',
            title: 'platform.menu.home',
            icon: 'fa fa-home',
            action: function () { $state.go('workspace'); },
            // this item must always be at the top
            priority: 0,
            isAlwaysOnBar: true
        };
        mainMenuService.addMenuItem(homeMenuItem);

        var browseMenuItem = {
            path: 'browse',
            icon: 'fa fa-search',
            title: 'platform.menu.browse',
            priority: 90
        };
        mainMenuService.addMenuItem(browseMenuItem);

        var cfgMenuItem = {
            path: 'configuration',
            icon: 'fa fa-wrench',
            title: 'platform.menu.configuration',
            priority: 191
        };
        mainMenuService.addMenuItem(cfgMenuItem);

        var moreMenuItem = {
            path: 'more',
            title: 'platform.menu.more',
            headerTemplate: '$(Platform)/Scripts/app/navigation/menu/mainMenu-list-header.tpl.html',
            contentTemplate: '$(Platform)/Scripts/app/navigation/menu/mainMenu-list-content.tpl.html',
            // this item must always be at the bottom, so
            // don't use just 99 number: we have INFINITE list
            priority: 9007199254740991, // Number.MAX_SAFE_INTEGER,
            isAlwaysOnBar: true
        };
        mainMenuService.addMenuItem(moreMenuItem);

        $rootScope.$on('unauthorized', function (event, rejection) {
            if (!authService.isAuthenticated) {
                $state.go('loginDialog');
            }
        });

        //server error  handling
        //$rootScope.$on('httpError', function (event, rejection) {
        //    if (!(rejection.config.url.indexOf('api/platform/notification') + 1)) {
        //        pushNotificationService.error({ title: 'HTTP error', description: rejection.status + ' — ' + rejection.statusText, extendedData: rejection.data });
        //    }
        //});

        $rootScope.$on('loginStatusChanged', function (event, authContext) {
            //timeout need because $state not fully loading in run method and need to wait little time
            $timeout(function () {
                if (authContext.isAuthenticated) {
                    if (!$state.current.name || $state.current.name == 'loginDialog') {
                        $state.go('workspace');

                    }
                }
                else {
                    $state.go('loginDialog');
                }
            }, 500);

        });

        authService.fillAuthData();


        // cache application level templates
        $templateCache.put('pagerTemplate.html', '<div class="pagination"><pagination boundary-links="true" max-size="pageSettings.numPages" items-per-page="pageSettings.itemsPerPageCount" total-items="pageSettings.totalItems" ng-model="pageSettings.currentPage" class="pagination-sm" previous-text="&lsaquo;" next-text="&rsaquo;" first-text="&laquo;" last-text="&raquo;"></pagination></div>');

        gridsterConfig.columns = 4;
        gridsterConfig.colWidth = 130;
        gridsterConfig.defaultSizeX = 1;
        gridsterConfig.resizable = { enabled: false, handles: [] };
        gridsterConfig.maxRows = 8;
        gridsterConfig.mobileModeEnabled = false;
        gridsterConfig.outerMargin = false;

        String.prototype.hashCode = function () {
            var hash = 0, i, chr, len;
            if (this.length === 0) return hash;
            for (i = 0, len = this.length; i < len; i++) {
                chr = this.charCodeAt(i);
                hash = ((hash << 5) - hash) + chr;
                hash |= 0; // Convert to 32bit integer
            }
            return hash;
        };

        String.prototype.endsWith = function (suffix) {
            return this.indexOf(suffix, this.length - suffix.length) !== -1;
        };

        // textAngular
        taOptions.toolbar = [
        ['bold', 'italics', 'underline', 'strikeThrough', 'ul', 'ol', 'redo', 'undo', 'clear', 'quote'],
        ['justifyLeft', 'justifyCenter', 'justifyRight', 'indent', 'outdent', 'html', 'insertImage', 'insertLink', 'insertVideo']];

        //amMoment.changeLocale('ru');

        $rootScope.threeDecimalPlacesPattern = /^-?[0-9]+(\.[0-9]{1,3})?$/;
    }]);
