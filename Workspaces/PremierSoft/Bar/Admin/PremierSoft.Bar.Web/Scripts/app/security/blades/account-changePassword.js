angular.module('platformWebApp')
.controller('platformWebApp.accountChangePasswordController', ['$scope', 'platformWebApp.bladeNavigationService', 'platformWebApp.accounts', function ($scope, bladeNavigationService, accounts) {
    var blade = $scope.blade;

    function initializeBlade() {
        blade.currentEntity = {
            oldPassword: '',
            newPassword: '',
            newPassword2: ''
        };
        
        blade.isLoading = false;
    };
    
    $scope.saveChanges = function () {
        if (blade.currentEntity.newPassword != blade.currentEntity.newPassword2) {
            blade.error = 'Ошибка: новые пароли не совпадают!';
            return;
        }

        if (blade.currentEntity.oldPassword == blade.currentEntity.newPassword) {
            blade.error = 'Ошибка: старый пароль совпадает с новым!';
            return;
        }

        blade.isLoading = true;
        blade.error = undefined;

        var postData = {
            oldPassword: blade.currentEntity.oldPassword,
            newPassword: blade.currentEntity.newPassword
        };

        accounts.changepassword({ id: blade.currentEntityId }, postData, function (data) {
            if (data.succeeded) {
                $scope.bladeClose();
            }
            else {
                bladeNavigationService.setError('Error: ' + data.errors[0], $scope.blade);
            }
        }, function (error) {
            bladeNavigationService.setError('Error: ' + error.status, $scope.blade);
        });
    };
    
    blade.headIcon = 'fa-key';

    // actions on load
    initializeBlade();
}]);