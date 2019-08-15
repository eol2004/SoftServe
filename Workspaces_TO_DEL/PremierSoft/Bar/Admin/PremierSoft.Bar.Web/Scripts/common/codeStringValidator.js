angular.module('platformWebApp')
.factory('platformWebApp.codeStringValidatorService', function () {
    function validateInternal(value) {
        var pattern = /[$+;=%{}[\]|\\\/@ ~#!^*&()?:'<>,]/;
        return !pattern.test(value);
    }

    return {
        validateNullable: function (value) {
            if (!value || 0 === value.length)
                return true;
            return validateInternal(value);
        },
        validate: function (value) {
            return validateInternal(value);
        }
    }
});