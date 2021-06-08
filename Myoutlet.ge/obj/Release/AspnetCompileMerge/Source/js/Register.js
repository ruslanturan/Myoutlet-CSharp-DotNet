(function ($) {
    "use strict"; // Start of use strict
    $("#pass2").on("keyup", function () {
        if ($(this).val() != $("#pass").val()) {
            $(".match").css("display", "block");
        }
        else {
            $(".match").css("display", "none");
        }
    })
}) (jQuery); // End of use strict