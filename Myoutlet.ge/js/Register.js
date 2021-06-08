(function ($) {
    "use strict"; // Start of use strict
    $("#pass2").on("keyup", function () {
        if ($(this).val() != $("#pass").val()) {
            $(".match").css("display", "block");
        }
        else {
            $(".match").css("display", "none");
        }
    });
	 $("#ade").on('click',function(e){
	 	e.preventDefault();
		 if ($("#Linkd").val().length < 1) {
            alert("გთხოვთ დაამატოთ სახელი.");
            return;
        }
		if ($(".img-list").children().length < 2) {
            alert("გთხოვთ დაამატოთ სურათი.");
            return;
        }
		$('.register').submit();
	 });
}) (jQuery); // End of use strict