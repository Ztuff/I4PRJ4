$(document).ready(function () {

    $('#VaretypeDropDown').change(function (event) {
        var dropValue = $('#VaretypeDropDown option:selected').text();
        $('#VaretypeText').val(dropValue);

    });


});