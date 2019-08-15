angular.module('platformWebApp')
.factory('platformWebApp.decimalHelper', function () {

    var PERCENT_VAL = 100;
    //var DECIMAL_ROUND_VAL = 1000;
    //var CURRENCY_ROUND_VAL = 100;

    function round(value, decimals) {
        return Number(Math.round(value + 'e' + decimals) + 'e-' + decimals);
    }

    function roundDecimal(value) {
        return round(value, 3);
    }

    function roundCurrency(value) {
        return round(value, 2);
    }

    return {
        reduceByPercent: function (value, percent) {
            return roundDecimal(value * (PERCENT_VAL - percent) / PERCENT_VAL);
        },
        vatValueFromSum: function (sum, vat) {
            if (isNaN(vat))
                return undefined;
            return roundCurrency(sum * vat / (vat + PERCENT_VAL));
        },
        sumWithoutVatFromSum: function (sum, vatValue) {
            if (isNaN(vatValue))
                return sum;
            return sum - vatValue;
        },
        vatValueFromSumWithoutVat: function (sumWithoutVat, vat) {
            if (isNaN(vat))
                return undefined;
            return roundCurrency(sumWithoutVat * vat / PERCENT_VAL);
        },
        percentByDivision: function (value1, value2) {
            return roundDecimal(PERCENT_VAL - value1 * PERCENT_VAL / value2);
        },
        roundDecimal: roundDecimal,
        roundCurrency: roundCurrency
    };
});