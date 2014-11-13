/// <reference path="../../../../../Include/scripts/jquery-1.3.2-vsdoc.js" />
/// <reference path="fullcalendar.js" />
/// <reference path="eventCalendar-controls.js"/>
/// <reference path="../../../../../Include/scripts/jquery.hoverintent.min.js" />

/**********************************************************************
* Description:  JavaScript for event calendar views
* Created By:   Jason Offutt @ Central Christian Church of the East Valley
* Date Created: 7/12/2010
*
* $Workfile: eventcalendar.js $
* $Revision: 22 $
* $Header: /trunk/Arena/UserControls/Custom/Cccev/Web2/js/eventcalendar.js   22   2011-11-14 15:39:43-07:00   nicka $
*
* $Log: /trunk/Arena/UserControls/Custom/Cccev/Web2/js/eventcalendar.js $
*  
*  Revision: 22   Date: 2011-11-14 22:39:43Z   User: nicka 
*  change from 4 to 0 featured images for the List view. 
*  
*  Revision: 21   Date: 2011-11-14 17:11:45Z   User: JasonO 
*  Minifying js files and setting calendar modules to use the minified script 
*  files. 
*  
*  Revision: 20   Date: 2011-11-11 00:51:01Z   User: JasonO 
*  Adding functionality to accept multiple campuses within event calendar 
*  filter controls. 
*  
*  Revision: 19   Date: 2011-11-10 18:16:40Z   User: JasonO 
*  Calendar behavior and CSS tweaks. 
*  
*  Revision: 18   Date: 2011-11-03 19:11:59Z   User: JasonO 
*  Performing surgery to remove event calendar module dependency on campus 
*  scripts library. 
*  
**********************************************************************/

var central_eventCalendar = (function () {
    // Private variables
    var _eventProfiles = [];
    var _currentDate = new Date();
    var _months = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    var _currentView = '';

    // Common private functions
    function _loadView() {
        _clearViews();
        switch (_currentView.toLowerCase()) {
            case "calendar":
                $("#event-calendar").fullCalendar("destroy");
                $("#event-calendar").fadeIn("fast");
                _initEventCalendar(central_calendarControls.getParams());
                break;
            case "cloud":
                _initEventCloud(central_calendarControls.getParams());
                $("#event-cloud").fadeIn("fast");
                break;
            case "list":
                _initEventList(central_calendarControls.getParams());
                $("#event-list-wrapper").fadeIn("fast");
                break;
        }
    }

    function _clearViews() {
        $("html, body").animate({ scrollTop: 0 }, 1000, "easeOutCubic");
        _getOverlay(_currentView).css({ height: $(".event-view:visible").outerHeight() + 30 + "px" }).fadeIn("fast");
        $(".event-view").fadeOut("fast");
    }

    function _getOverlay(view) {
        switch (view) {
            case "cloud":
                return $("#cloud-overlay");
            case "list":
                return $("#event-list-overlay");
            case "calendar":
            default:
                return $("#calendar-overlay");
        }
    }

    function _toolTipOver(element, event, className) {
        var $div = element.children(className);

        if ($div.is(":hidden")) {
            var offset = element.offset();
            $div.css({
                left: event.pageX - offset.left + 20 + "px",
                top: event.pageY - offset.top + "px"
            }).fadeIn("fast");
        }
    }

    function _toolTipOut(element, className) {
        var $div = element.children(className);

        if ($div.is(":visible")) {
            element.children(className).fadeOut("fast");
        }
    }

    function _toolTipMove(element, event, className) {
        var $div = element.children(className);

        if ($div.is(":visible")) {
            var offset = element.offset();
            $div.css({
                left: event.pageX - offset.left + 20 + "px",
                top: event.pageY - offset.top + "px"
            });
        }
    }

    function _getEventTimeString(time) {
        var timeString = time.toTimeString();

        var timeParts = timeString.split(':');
        var hours = timeParts[0];
        var minutes = timeParts[1];
        var isAM = true;

        if (parseInt(hours) > 12) {
            isAM = false;
            hours = hours - 12;

        }

        if (hours.length > 1 && hours[0] == 0) {
            hours = hours[1];
        }

        var theTime = hours + ":" + minutes;
        theTime += isAM ? " AM" : " PM";
        return theTime;
    }

    function parseCampuses(campuses) {
        var c = campuses.length > 0 ? campuses : [-1];
        return c.join(',');
    }

    // Event Calendar private functions
    function _initEventCalendar(params) {
        $("#event-calendar").fullCalendar({
            viewDisplay: _bindCalendarViewEvents,
            events: function (start, end, callback) {
                var pageID = $("#ihCalendarEventDetails").val();
                var jsonData = '{ "start":"' + start.toDateString() + '", "end":"' + end.toDateString() + '", "keywords":"' + params.keywords + '", "topicIDs":"' + params.topicIDs + '", "campusIDs":"' + parseCampuses(params.campuses) + '", "pageID": ' + pageID + ' }';

                $.ajax({
                    type: "POST",
                    url: "webservices/custom/cccev/web2/eventsservice.asmx/GetEventList",
                    contentType: "application/json; charset=utf-8",
                    data: jsonData,
                    dataType: "json"
                })
                .success(function (result) {
                    _eventProfiles = result.d;
                    callback(_eventProfiles);
                    _bindCalendarViewEvents();
                    return false;
                })
                .error(_onAjaxError);
            },
            loading: function (isLoading, view) {
                if (isLoading) {
                    $("#calendar-overlay").css({ height: $("#event-calendar").outerHeight() + 20 + "px" }).fadeIn("fast");
                }
                else {
                    $(".spinner").fadeOut("fast");
                }

                return false;
            }
        });

        if (_currentDate.getMonth() != new Date().getMonth()) {
            // will not render current month's events if called on page load
            $("#event-calendar").fullCalendar('gotoDate', _currentDate);
        }
    }

    function _updateCalendarView(e, data) {
        var view = $("#event-calendar").fullCalendar("getView");
        var sDate = view.start;

        if (sDate.getFullYear < 1900) {
            sDate = new Date(sDate.getFullYear() + 1900, sDate.getMonth(), sDate.getDate());
        }

        _currentDate = sDate;

        $("#event-calendar").fullCalendar("destroy");
        _initEventCalendar(data);
        return false;
    }

    function _bindCalendarViewEvents(view) {
        $(document).trigger("CONTENT_RENDERED");

        $(".fc-event").hoverIntent(
	    function (event) {
	        _toolTipOver($(this), event, ".fc-event-description");
	        return false;
	    },
	    function () {
	        _toolTipOut($(this), ".fc-event-description");
	        return false;
	    })
	    .mousemove(function (event) {
	        _toolTipMove($(this), event, ".fc-event-description");
	        return false;
	    });
    }

    // Event Tag Cloud private functions
    function _getCondensedEvents(profiles) {
        var condensedEvents = [];

        for (var i = 0; i < profiles.length; i++) {
            var hasMatch = false;
            var event = profiles[i];

            for (var j = 0; j < condensedEvents.length; j++) {
                if (event.id == condensedEvents[j].id) {
                    hasMatch = true;
                    break;
                }
            }

            if (!hasMatch) {
                var date = new Date(event.start);
                event.time = _getMonth(date) + " " + _getDayOfMonth(date) + ", " + _getEventTimeString(date);
                condensedEvents.push(event);
            }
        }

        return condensedEvents;
    }

    function _onAjaxError(result, errorText, thrownError) {
        return false;
    }

    function _initEventCloud(params) {
        var pageID = $("#ihCloudEventDetails").val();
        var jsonData = '{ "start":"' + params.start.toDateString() + '", "end":"' + params.end.toDateString() + '", "keywords":"' + params.keywords + '", "topicIDs":"' + params.topicIDs + '", "campusIDs":"' + parseCampuses(params.campuses) + '", "pageID": ' + pageID + ' }';

        $.ajax({
            type: "POST",
            url: "webservices/custom/cccev/web2/eventsservice.asmx/GetAlphabeticalEventList",
            contentType: "application/json; charset=utf-8",
            data: jsonData,
            dataType: "json"
        })
        .success(function (result) {
            _eventProfiles = result.d;
            var condensed = _getCondensedEvents(_eventProfiles);
            $("#event-cloud ul").children().remove();
            $("#cloud-item").render(condensed).appendTo("#event-cloud ul");
            //$("#event-cloud .campus").html(central_calendarControls.getParams().campus.name);
            var campusNames = params.campusNames.length > 0 ? params.campusNames.join(', ') : 'All Campuses';
            $('#event-cloud .campus').html(campusNames);
            _bindCloudEvents();
            $(".spinner").fadeOut("fast");
            return false;
        })
        .error(_onAjaxError());
    }

    function _updateCloudView(e, data) {
        $("#cloud-overlay").fadeIn("fast");
        _initEventCloud(data);
        return false;
    }

    function _bindCloudEvents() {
        $(".tag").hoverIntent(
	    function (event) {
	        _toolTipOver($(this), event, ".tag-description");
	        return false;
	    },
	    function () {
	        _toolTipOut($(this), ".tag-description");
	        return false;
	    })
	    .mousemove(function (event) {
	        _toolTipMove($(this), event, ".tag-description");
	        return false;
	    });
    }

    // Event List private functions
    function _initEventList(params) {
        $("#event-list-overlay").height($("#event-list-overlay").parent().height() + 200);
        $("#event-list-overlay").fadeIn("fast");

        var pageID = $("#ihListEventDetails").val();
        var jsonData = '{ "start":"' + params.start.toDateString() + '", "end":"' + params.end.toDateString() + '", "keywords":"' + params.keywords + '", "topicIDs":"' + params.topicIDs + '", "campusIDs":"' + parseCampuses(params.campuses) + '", "pageID": ' + pageID + ' }';

        $.ajax({
            type: "POST",
            url: "webservices/custom/cccev/web2/eventsservice.asmx/GetEventList",
            contentType: "application/json; charset=utf-8",
            data: jsonData,
            dataType: "json"
        })
        .success(function (result) {
            _eventProfiles = result.d;
            _renderEventList(_eventProfiles);
            $(".spinner").fadeOut("fast");
            return false;
        })
        .error(_onAjaxError);
    }

    function _updateEventListView(e, data) {
        _initEventList(data);
        return false;
    }

    function _renderEventList(eventProfiles) {
        $("#event-featured-list").children().remove();
        $("#event-list").children().remove();

        if (typeof _eventProfiles === 'undefined' || _eventProfiles.length === 0) {
            return false;
        }

        // Post process json data to match what template needs.
        // Here we will use the "start" item and from it create a "date" and "month"
        for (var i = 0; i < eventProfiles.length; i++) {
            _eventProfiles[i].date = _getDayOfMonth(new Date(eventProfiles[i].start));
            _eventProfiles[i].month = _getMonth(new Date(eventProfiles[i].start));
        }

        // For featured items, only show the first occurrence of the event
        var featuredEvents = _getCondensedEvents(_eventProfiles);
        var maxItems = 0; // how many featured "images" to display?
        if (featuredEvents.length < maxItems) {
            maxItems = featuredEvents.length;
        }
        // ...also never show only 3 items.
        maxItems = (maxItems == 3) ? 2 : maxItems;

        featuredEvents = featuredEvents.slice(0, maxItems);

        // Render featured items
        $("#event-featured-list-template").render(featuredEvents).appendTo("#event-featured-list");

        // Render normal items
        $("#event-list-template").render(eventProfiles).appendTo("#event-list");
        return false;
    }

    function _getDayOfMonth(aDate) {
        var day = aDate.getDate();
        if (day < 10) {
            day = "0" + day;
        }
        return day;
    }

    function _getMonth(aDate) {
        return _months[aDate.getMonth()];
    }

    return {
        init: function () {
            $("#event-calendar").fadeIn("fast");
            //central_calendarControls.setCampus(central_calendarControls.getCampus());

            central_calendarControls.setTopics($("#ihTopicAreas").val().trim() != '' ? $("#ihTopicAreas").val() : '');
            _initEventCalendar(central_calendarControls.getParams());

            $(document).bind("CALENDAR_INFO_CHANGED", function (e, data) {
                switch (_currentView.toLowerCase()) {
                    case "cloud":
                        _updateCloudView(e, data);
                        break;
                    case "list":
                        _updateEventListView(e, data);
                        break;
                    case "calendar":
                    default:
                        _updateCalendarView(e, data);
                        break;
                }

                return false;
            });

            $(document).bind("CALENDAR_VIEW_CHANGED", function (event, viewName) {
                _currentView = viewName;
                _loadView();
                return false;
            });
        },
        getEventTime: function (time) {
            return _getEventTimeString(time);
        }
    };
})();

$(function () {
    central_eventCalendar.init();
});
