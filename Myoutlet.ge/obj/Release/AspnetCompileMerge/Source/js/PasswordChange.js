(function ($) {
    "use strict"; // Start of use strict
    $("#confirmNewPass").on("keyup", function () {
        if ($(this).val() != $("#newPass").val()) {
            $(".match").css("display", "block");
        }
        else {
            $(".match").css("display", "none");
        }
    })
    $("#newPass").on("keyup", function () {
        if ($(this).val() != $("#confirmNewPass").val()) {
            $(".match").css("display", "block");
        }
        else {
            $(".match").css("display", "none");
        }
    })
    $("#svBtn").on("click", function (e) {
        e.preventDefault();
        if ($(".match").css("display") == "none" && $("#confirmNewPass").val() != "" && $("#newPass").val() != "") {
            $(".changePass").submit();
        }
        else {
            alert("ფასი დაამატეთ სწორად!");
        }
    })
}) (jQuery); // End of use strict