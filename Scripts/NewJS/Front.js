


//////////////Start///////////Sorting by Letters

$(".searchInput").keyup(throttle(function () {

    var containsLetters = $(".searchInput").val().toLowerCase();

    if (containsLetters == "") {
        $(".survey-item").hide();
        $(".original").show();
        return;
    }

    var settings = {
        "async": true,
        "crossDomain": true,
        "url": window.location.pathname + "/Word/GetWordsThatContainInput?containsLetters=" + containsLetters,
        "method": "GET",
        "headers": {
            "cache-control": "no-cache"
        }
    }

    clearOutWordsAndMakeCall(settings)
}));

function throttle(f, delay) {
    var timer = null;
    return function () {
        var context = this, args = arguments;
        clearTimeout(timer);
        timer = window.setTimeout(function () {
            f.apply(context, args);
        },
            delay || 500);
    };
}

//////////////Start///////////Sorting by time frame
$(".timeFrames").change(function () {
    callAjax($(this).val())
});

function callAjax(val) {

    var settings = {
        "async": true,
        "crossDomain": true,
        "url": window.location.pathname + "/Word/GetTimeFrameWords/?timeFrame=" + val,
        "method": "GET",
        "headers": {
            "cache-control": "no-cache"
        }
    }

    clearOutWordsAndMakeCall(settings)
}

function clearOutWordsAndMakeCall(settings) {
    $(".wordTableBody").fadeOut();

    $.ajax(settings).done(function (response) {
        var makeArray = JSON.parse(response);
        $(".survey-item").hide();
        makeArray.forEach(function (item) {
            appendItems(item)
        });
    });
}

function appendItems(item) {
    $(".wordTableBody").fadeIn();
    $(".wordTableBody").append(
        "<li class='survey-item' data-slug='" + item.Slug + "'" + "data-day='" + item.DailyCount + "' data-week='" + item.WeeklyCount + "'" + "data-month='" + item.MonthlyCount + "' data-year='" + item.YearlyCount + "'>" +
        "<a href='"+item.Slug+"'>"+
        "<span class='survey-name'>" + item.Word + "</span>" +
        "<div class='pull-right'>" +
        "<span class='dailyCount'>" +
        "Daily Count: " + item.DailyCount +
        "</span>" +
        "<span class='AllCount'>" +
        "Daily:" +
        "</span>" +
        "</div>" +
        "</a>"+
        "</li>")
};
//////////////End///////////Sorting by time frame
