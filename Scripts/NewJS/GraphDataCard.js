//! WORK IN PROGRESS !//

// TO DO //
// percentage on .bar to change based on percentage width.

$(document).ready(function () {

    var barOne = $('#bar-one');
    var barTwo = $('#bar-two');
    var barThree = $('#bar-three');
    var barFour = $('#bar-four');
    var barFive = $('#bar-five');

    var barOneWidth = 60;
    var barTwoWidth = 40;
    var barThreeWidth = 50;
    var barFourWidth = 90;
    var barFiveWidth = 70;

    // var getPercentWidth = function(element) {
    //   var parentWidth = element.offsetParent().width();
    //   var percent = 100 * width / parentWidth;
    //   return percent;
    // };

    $('.head-box').click(function () {
        $(this).toggleClass('max');
        barOne.toggleClass('one');
        barTwo.toggleClass('two');
        barThree.toggleClass('three');
        barFour.toggleClass('four');
        barFive.toggleClass('five');
    });

});