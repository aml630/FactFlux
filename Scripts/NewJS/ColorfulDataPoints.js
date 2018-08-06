$(document).ready(function () {

    var c = 1;
    $('.block-chart').each(function () {
        var thisId = 'chart' + c;
        $(this).attr('id', thisId);
        makeChart('#' + thisId);
        c++;
    })

    function makeChart(id) {
        $(id).empty();
        var masterArray = $(id).data('numbers');
        var theTotal = calcTotal(id, masterArray);
        var blockArray = [];
        for (i = 0; i < masterArray.length; i++) {
            var noOfBlocks = Math.round(masterArray[i][1] / theTotal * 100);
            var blocksAdded = 0;
            blockArray = addBlocks(id, noOfBlocks, masterArray, blockArray);
        }
        console.log(blockArray);
        //i=0;
        addBlock(id, blockArray, 0, 15);
    }

    function addBlock(id, blockArray, i, timeout) {
        if (i < blockArray.length) {
            $(id).append(blockArray[i]);
            i++;
            timeout += 0.5;
            setTimeout(function () { addBlock(id, blockArray, i, timeout); }, timeout)
        }
    }

    function addBlocks(id, noOfBlocks, masterArray, blockArray) {
        for (b = 0; b < noOfBlocks; b++) {
            blockArray.push('<div class="block type' + i + '"><p>' + masterArray[i][0] + '</p></div>');
            //$(id).append('<div class="block type'+i+'"><p>'+ masterArray[i][0] +'</p></div>');
        }
        return blockArray;
    }

    function calcTotal(id, array) {
        var masterTotal = 0;
        for (i = 0; i < array.length; i++) {
            masterTotal += array[i][1];
        }
        return masterTotal;
    }

})