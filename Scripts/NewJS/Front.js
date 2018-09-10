
var pageNumber = 0;

$(".getMoreWords").click(function () {
    var val = $(".timeFrames").val();
    console.log(val);

    pageNumber += 1;
    var url = window.location.href + "/Word/GetTimeFrameWords/?timeFrame=" + val + "&pageNumber=" + pageNumber;

    var settings = {
        "async": true,
        "crossDomain": true,
        "url": url,
        "method": "GET",
        "headers": {
            "cache-control": "no-cache"
        }
    }

    $.ajax(settings).done(function (response) {
        var makeArray = JSON.parse(response);

        makeArray.forEach(function (item) {
            var amount = "";
            var filterWord = "";
            if (val == "1") {
                filterWord = "Daily";
                amount = item.DailyCount;
            }
            if (val == "2") {
                filterWord = "Weekly";
                amount = item.WeeklyCount;
            }
            if (val == "3") {
                filterWord = "Monthly";
                amount = item.MonthlyCount;
            }
            if (val == "4") {
                filterWord = "Yearly";
                amount = item.YearlyCount;
            }

            appendWordToTable(item, filterWord, amount)
        });
    });
})

function appendWordToTable(item, filterWord, amount) {

    if (item.Image == null || item.Image == "")
    {
        $(".wordTableBody").append(
            "<li class='survey-item' data-slug='" + item.Slug + "'" + "data-day='" + item.DailyCount + "' data-week='" + item.WeeklyCount + "'" + "data-month='" + item.MonthlyCount + "' data-year='" + item.YearlyCount + "'>" +
            "<a href='" + item.Slug + "'>" +
            "<span class='survey-name'>" + item.Word + "</span>" +
            "<div class='pull-right'>" +
            "<span class='dailyCount'>" +
            "<i class='fa fa-calendar' aria-hidden='true'></i>" +
            " " + filterWord + " Count: " + amount +
            "</span>" +
            "<span class='AllCount'>" +
            "Daily:" +
            "</span>" +
            "</div>" +
            "</a>" +
            "</li>")
    }else {
        $(".wordTableBody").append(
            "<li class='survey-item' data-slug='" + item.Slug + "'" + "data-day='" + item.DailyCount + "' data-week='" + item.WeeklyCount + "'" + "data-month='" + item.MonthlyCount + "' data-year='" + item.YearlyCount + "'>" +
            "<a href='" + item.Slug + "'>" +
            "<div class='topWord'>" +
            "<span class='survey-name'>" + item.Word +
            "</span>" +
            "<img class ='wordImg' src='" + window.location.href + item.Image + "'/>" +
            "</div>" +
            "<div class='pull-right'>" +
            "<span class='dailyCount'>" +
            "<i class='fa fa-calendar' aria-hidden='true'></i>" +
            " " + filterWord + " Count: " + amount +
            "</span>" +
            "<span class='AllCount'>" +
            "Daily:" +
            "</span>" +
            "</div>" +
            "</a>" +
            "</li>")
    }             
}

//////////////Start///////////Sorting by Letters

$(".searchInput").keyup(throttle(function () {

    var containsLetters = $(".searchInput").val().toLowerCase();

    if (containsLetters == "") {
        $(".survey-item").hide();
        $(".original").show();
        $(".timeFrames").val("1");
        return;
    }

    var url = window.location.href + "/Word/GetWordsThatContainInput?containsLetters=" + containsLetters;

    var settings = {
        "async": true,
        "crossDomain": true,
        "url": url,
        "method": "GET",
        "headers": {
            "cache-control": "no-cache"
        }
    }

    clearOutWordsAndMakeCall(settings, "4")
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

    var url = window.location.href + "/Word/GetTimeFrameWords/?timeFrame=" + val;

    var settings = {
        "async": true,
        "crossDomain": true,
        "url": url,
        "method": "GET",
        "headers": {
            "cache-control": "no-cache"
        }
    }

    clearOutWordsAndMakeCall(settings, val)
}

function clearOutWordsAndMakeCall(settings, val) {
    $(".wordTableBody").fadeOut();

    var filterWord = "";
    if (val == "1") {
        filterWord = "Daily";
    }
    if (val == "2") {
        filterWord = "Weekly";
    }
    if (val == "3") {
        filterWord = "Monthly";
    }
    if (val == "4") {
        filterWord = "Yearly";
    }

    $.ajax(settings).done(function (response) {
        var makeArray = JSON.parse(response);
        $(".survey-item").hide();
        makeArray.forEach(function (item) {
            fadeInAndAppend(item, filterWord)
        });
    });
}

function fadeInAndAppend(item, filterWord) {

    var amount = "";
    if (filterWord == "Daily") {
        amount = item.DailyCount;
    }
    if (filterWord == "Weekly") {
        amount = item.WeeklyCount;
    }
    if (filterWord == "Monthly") {
        amount = item.MonthlyCount;
    }
    if (filterWord == "Yearly") {
        amount = item.YearlyCount;
    }

    $(".wordTableBody").fadeIn();

    appendWordToTable(item, filterWord, amount)
};
//////////////End///////////Sorting by time frame
