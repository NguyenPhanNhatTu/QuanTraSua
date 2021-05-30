


$(document).ready(function () {
    $(".add-to-cart").click(function (e) {
        e.preventDefault();
        var MaMon = $(this).attr("data-IDMon");
        $.ajax({
            url: '/Menus/themgiohang',
            type: 'POST',
            async: true,
            data: {
                mamon: MaMon
            },
            success: function (KiemTra) {
                //alert(msg);
                if (KiemTra == "True") {
                    $("#test").click()
                    $('#ChonMon').load(" #load");
                }
            }
        });
    });
})
//Load wating bar
$(document).ajaxStart(function () {
    $("#waiting").show();
}).ajaxStop(function () {
    $("#waiting").hide();
});
//$(".quantiTy-increase").click(function (event) {
//    var quantity = $(this).attr("data-quantity");
//    quantity = Number(quantity);
//    quantity = quantity + 1;
//    $('.change-qty').html(quantity);
//});




//$(document).ready(function () {
//    $("#DatMon").submit(function (event) {
//        var quantity = $(this).attr("data-quantity");
//        quantity = Number(quantity);
//        quantity = quantity - 1;
//        $('.change-qty').html(quantity);
//    });
    
//})


