$(document).ready(function() {

    $('#VaretypeDropDown').change(function(event) {
        var dropValue = $('#VaretypeDropDown option:selected').text();
        $('#VaretypeText').val(dropValue);

    });
    
//    var dNow = new Date();
//    var utcdatePlusAYear = (dNow.getMonth() + 1) + '-' + dNow.getDate() + '-' + (dNow.getFullYear() + 1);
//    $('#LifeOfItem')(
//    {
//        source: 'utcdatePlusAYear'
////    $('#LifeOfItem').text(utcdatePlusAYear);
//    });

    var dNow = new Date();
    var utcdatePlusAYear = (dNow.getMonth() + 1) + '-' + dNow.getDate() + '-' + (dNow.getFullYear() + 1);
    $('#addItemImgClicked').mouseover(function (event)
    {
        if ($('#LifeOfItem').text == null) {
            $('#LifeOfItem').text(utcdatePlusAYear);
        }
    $('#LifeOfItem').text(utcdatePlusAYear);
    });


});