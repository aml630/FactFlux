$(document).ready(function () {
    $(".btnTweet").each(function (quote) {
        var quote = $(this).parent().parent().find(".quote").text();
        var encodedQuote = encodeURIComponent(quote);
        var tweetUrl = "https://twitter.com/intent/tweet?text=" + encodedQuote;
        $(this).attr('href', tweetUrl);
    });
});