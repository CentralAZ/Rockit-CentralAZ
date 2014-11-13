(function () {
    'use strict';

    /** @namespace */
    window.central_calendarControls = (function () {
        var _currentView = '',
            _params = {
                start: new Date(),
                end: new Date(),
                keywords: '',
                topicIDs: '',
                campuses: [],
                campusNames: []
            };

        function getShortDateString(dateString) {
            var today = new Date();
            var date = new Date(Date.parse(dateString));

            if ((today.getMonth() == date.getMonth()) && (today.getDate() == date.getDate())) {
                return "Today";
            }

            return dateString.substring(dateString.indexOf(" ") + 1, dateString.length - 4);
        }

        function _setRange(event, ui) {
            var startDate = new Date();
            startDate.setDate(startDate.getDate() + ui.values[0]);
            _params.start = startDate;

            var endDate = new Date();
            endDate.setDate(endDate.getDate() + ui.values[1]);
            _params.end = endDate;

            $(".min").text(getShortDateString(startDate.toDateString()));
            $(".max").text(getShortDateString(endDate.toDateString()));
        }

        function notifySubscribers() {
            $(document).trigger('CALENDAR_INFO_CHANGED', _params);
        }

        function setCampuses() {
            _params.campuses = [];
            _params.campusNames = [];
            $('#campuses > li').removeClass();
            $('#campuses input:checkbox:checked').each(function () {
                var $that = $(this),
                    attribute = $that.attr('data-id'),
                    name = $that.next('label').text();
                $that.parent('li').removeClass().addClass('selected');

                if ($.inArray(attribute, _params.campuses) === -1) {
                    _params.campuses.push(attribute);
                }

                if ($.inArray(name, _params.campusNames) === -1) {
                    _params.campusNames.push(name);
                }
            });
        }

        function initSettings() {
            // Set initial date values
            var theStartDate = new Date();
            _params.start = theStartDate;

            $(".min").text(getShortDateString(theStartDate.toDateString()));
            var theEndDate = new Date();
            theEndDate.setDate(theEndDate.getDate() + 90);
            _params.end = theEndDate;

            $(".max").text(getShortDateString(theEndDate.toDateString()));

            setCampuses();
        }

        function bindCampusPicker() {
            $('#campuses input:checkbox').click(function () {
                setCampuses();
                notifySubscribers();
            });
        }

        function bindDateSlider() {
            $("#date-slider").slider({
                range: true,
                min: 0,
                max: 365,
                step: 7,
                values: [0, 90],
                slide: _setRange
            });

            $(document).bind("slidechange", function (event, ui) {
                notifySubscribers();
                return false;
            });
        }

        function bindKeyWordSearch() {
            $(".calendar-search").change(function () {
                _params.keywords = $(".calendar-search").val();
                notifySubscribers();
                $(document).unbind("submit").submit(function () { return false; });
                return false;
            });

            $(".calendar-search").keypress(function (event) {
                if (event.which === 13) {
                    $(".calendar-search").change();
                    return false;
                }

                return true;
            });
        }

        function bindViewPicker() {
            $("input[name='calendar-view']").click(function () {
                _currentView = $(this).attr("rel");
                $(document).trigger("CALENDAR_VIEW_CHANGED", [_currentView]);
                return true;
            });
        }

        function listen() {
            $(document).bind("CAMPUS_UPDATED", function () {
                $(document).trigger("CALENDAR_INFO_CHANGED", _params);
                return false;
            });

            $(document).bind("CALENDAR_VIEW_CHANGED", function () {
                if (_currentView.toLowerCase() == "calendar") {
                    $("#slider-wrap").slideUp();
                }
                else {
                    $("#slider-wrap").slideDown();
                }

                return false;
            });
        }

        return {
            init: function () {
                initSettings();
                $("input[name='calendar-view']:first").attr("checked", "checked");
                $(".calendar-search").val("");
                bindCampusPicker();
                bindDateSlider();
                bindKeyWordSearch();
                bindViewPicker();
                listen();
            },
            getParams: function () {
                return _params;
            },
            getCampuses: function () {
                return _params.campuses;
            },
            setTopics: function (topics) {
                _params.topicIDs = topics;
            }
        };
    })();

    $(function () {
        central_calendarControls.init();
    });
} ());