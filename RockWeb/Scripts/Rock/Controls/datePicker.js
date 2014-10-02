﻿(function ($) {
    'use strict';
    window.Rock = window.Rock || {};
    Rock.controls = Rock.controls || {};

    Rock.controls.datePicker = (function () {
        var exports = {
            initialize: function (options) {
                if (!options.id) {
                    throw 'id is required';
                }
                var dateFormat = 'mm/dd/yyyy';
                if (options.format) {
                    dateFormat = options.format;
                }

                // uses https://github.com/ianserlin/bootstrap-datepicker/tree/3.x
                $('#' + options.id).datepicker({
                    format: dateFormat,
                    autoclose: true,
                    todayBtn: true,
                    startView: options.startView || 'month'
                });

            }
        };

        return exports;
    }());
}(jQuery));