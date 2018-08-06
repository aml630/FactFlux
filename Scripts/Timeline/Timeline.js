$(".searchInput").keyup(function () {

    var wtf = $(".searchInput").val().toLowerCase();

    $(".resourceText").each(function () {

        var searchWord = $(this).text().toLowerCase();

        if (searchWord.indexOf(wtf) !== -1) {
            $(this).parent().parent().removeClass("hideSearch");
        } else {
            $(this).parent().parent().addClass("hideSearch");
        }
    })
});


$(".toggleVids").click(function () {

    $('[data-feedType="YouTube"]').toggleClass("hideVid");


    $(".toggleVids").toggleClass("inActive");
})

$(".toggleArticles").click(function () {

    $('[data-feedType="RSS"]').toggleClass("hideArticle");

    $(".toggleArticles").toggleClass("inActive");
})