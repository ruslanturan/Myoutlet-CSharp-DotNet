(function ($) {
    "use strict"; // Start of use strict
    $(".dngr").on("click", function (e) {
        e.preventDefault();
        if (confirm("გსურთ ამის წაშლა?")) {
            window.location.replace($(this).attr("href"));
        }
    });
    $(".sccss").on("click", function (e) {
        e.preventDefault();
        if (confirm("გსურთ ამ პროდუქტის სიაში დამატება?")) {
            window.location.replace($(this).attr("href"));
        }
    });
    $(".prmry").on("click", function (e) {
        e.preventDefault();
        if (confirm("გსურთ ამ პროდუქტის სიიდან ამოშლა?")) {
            window.location.replace($(this).attr("href"));
        }
    });
    $(".cll").on("click", function (e) {
        e.preventDefault();
        if (confirm("ნამდვილად დარეკეთ ამ ნომერზე?")) {
            window.location.replace($(this).attr("href"));
        }
    });
    $(".rmvv").on("click", function (e) {
        e.preventDefault();
        if (confirm("გსურთ მისი სიიდან ამოშლა?")) {
            window.location.replace($(this).attr("href"));
        }
    });
    $(".cmntcn").on("click", function (e) {
        e.preventDefault();
        if (confirm("გსურთ ამ კომენტარის დადასტურება?")) {
            window.location.replace($(this).attr("href"));
        }
    });
    $(".cmntrmv").on("click", function (e) {
        e.preventDefault();
        if (confirm("გსურთ ამ კომენტარის წაშლა?")) {
            window.location.replace($(this).attr("href"));
        }
    });
    $("#CompLogo").change(function () {
        let file = this.files[0];
        let fileType = file["type"];
        let fileSize = this.files[0].size;
        let validImageTypes = ["image/jpeg", "image/png"];
        if ($.inArray(fileType, validImageTypes) < 0) {
            alert("შეგიძლიათ ატვირთოთ მხოლოდ .jpg ან .png ფაილი.");
            return
        }
        if (fileSize > 5242880) {
            alert("თქვენი ლოგოს მაქსიმალური ზომა უნდა იყოს 5 mb.");
            return
        }
        if (fileSize < (102400)/2) {
            alert("თქვენი ლოგოს მინიმალური ზომა უნდა იყოს 50 kb.");
            return
        }
        if (this.files && this.files[0]) {
            var reader = new FileReader();
            reader.onload = function (e) {
                if ($(".img").length < 1) {
                    $("<li>").addClass("add-img img").css("cursor", "auto").insertBefore("#logo");
                }
                $(".img").css("background-image", 'url("' + e.target.result + '")');
            };
            reader.readAsDataURL(this.files[0]);
        }
    });
})(jQuery); // End of use strict
