

$("button").click(function () {
    $(this).css("background-color", "green");
});

var pageHideAmount = 30;

paginate(pageHideAmount);

function paginate(amount) {

    if (amount == -1) {
        pageHideAmount = 30;
        amount = 30;
    }

    var howMany = 0;

    $(".singleWord").each(function () {

        $(this).removeClass("paginatedHide");

        howMany++;

        if (howMany > amount) {
            $(this).addClass("paginatedHide");
        }
    });
}

$(".showMore").click(function () {

    pageHideAmount += 30;
    paginate(pageHideAmount);
})

$(".today").click(function () {
    var items = $('.wordRow');
    items.sort(function (a, b) {
        return +$(b).data('day') - +$(a).data('day');
    });
    items.appendTo('.wordTableBody');

    $(".dateWords").removeClass("dateWordsActive");
    $(".today").addClass("dateWordsActive");
    paginate(-1);
})

$(".thisWeek").click(function () {
    var items = $('.wordRow');
    items.sort(function (a, b) {
        return +$(b).data('week') - +$(a).data('week');
    });
    items.appendTo('.wordTableBody');
    $(".dateWords").removeClass("dateWordsActive");
    $(".thisWeek").addClass("dateWordsActive");
    paginate(-1);
})

$(".thisMonth").click(function () {
    var items = $('.wordRow');
    items.sort(function (a, b) {
        return +$(b).data('month') - +$(a).data('month');
    });
    items.appendTo('.wordTableBody');
    $(".dateWords").removeClass("dateWordsActive");
    $(".thisMonth").addClass("dateWordsActive");
    paginate(-1);
})

$(".thisYear").click(function () {
    var items = $('.wordRow');
    items.sort(function (a, b) {
        return +$(b).data('year') - +$(a).data('year');
    });
    items.appendTo('.wordTableBody');
    $(".dateWords").removeClass("dateWordsActive");
    $(".thisYear").addClass("dateWordsActive");
    paginate(-1);
})

$(".questionDiv").click(function () {
    $(".explaination").slideToggle();
})

var pathArray = location.href.split('/');
var protocol = pathArray[0];
var host = pathArray[2];
var url = protocol + '//' + host;

$(".searchInput").keyup(throttle(function () {

    var containsLetters = $(".searchInput").val().toLowerCase();

    if (containsLetters == "") {
        $(".nonSearched:not(.paginatedHide)").show();
        $(".searchedUp").hide();
        paginate(-1);
        return;
    }

    var settings = {
        "async": true,
        "crossDomain": true,
        "url": url + "/Word/GetWordsThatContainInput?containsLetters=" + containsLetters,
        "method": "GET",
        "headers": {
            "cache-control": "no-cache"
        }
    }

    $.ajax(settings).done(function (response) {
        console.log(response);

        var makeArray = JSON.parse(response);

        $(".searchedUp").hide();

        makeArray.forEach(function (item) {
            console.log(item.Word);

            $(".wordTableBody").append("<tr class='wordRow singleWord searchedUp' data-slug='" + item.Slug + "'" + "data-day='" + item.DailyCount + "' data-week='" + item.WeeklyCount + "'" +
                "data-month='" + item.MonthlyCount + "' data-year='" + item.YearlyCount + "'>" +
                "<td class='letters'>" + item.Word + "</td>" +
                "<td>" + item.DailyCount + "</td>" +
                "<td>" + item.WeeklyCount + "</td>" +
                "<td>" + item.MonthlyCount + "</td>" +
                "<td>" + item.YearlyCount + "</td>" +
                "</tr>"
            )
        });

        $(".nonSearched:not(.paginatedHide)").hide();
    });
}));

$(".wordTableBody").on('click','.wordRow', function ()  {
   
    var slug = $(this).attr("data-slug");

    window.location.href = url + "/" + slug;
})

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
