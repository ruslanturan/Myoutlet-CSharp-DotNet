(function ($) {
    "use strict"; // Start of use strict
    let imgListItems = $(".img-list").children().length;
    $("#addImgs").on('click', function () {
        $('#GalleryEdit').click();
        return false;
    });
    $("#GalleryEdit").on("change", function (e) {
        for (var i = 0; i < $(this).get(0).files.length; ++i) {
            let file = this.files[i];
            let fileType = file["type"];
            let fileSize = this.files[i].size;
            let validImageTypes = ["image/jpeg", "image/png"];
            if ($.inArray(fileType, validImageTypes) < 0) {
                alert("შეგიძლიათ ატვირთოთ მხოლოდ .jpg ან .png ფაილი.");
                return
            }
            if (fileSize > 5242880) {
                alert("თქვენი ლოგოს მაქსიმალური ზომა უნდა იყოს 5mb.");
                return
            }
            if (fileSize < 102400 ) {
                alert("თქვენი ლოგოს მინიმალური ზომა უნდა იყოს 100kb.");
                return
            }
        }
        for (var i = 0; i < $(this).get(0).files.length; ++i) {
            var files = $(this).get(0).files;
            let imgName = files[i].name;
            if (imgListItems < 16) {
                ++imgListItems;
                let reader = new FileReader();
                reader.onload = function (e) {
                    var that = $("<li>").attr("data-name", $("#Product_ProductUniqueNum").val() + imgName).addClass("add-img img").css({ "background-image": 'url("' + e.target.result + '")', "cursor": "auto" }).insertBefore("#addImgs");
                    $("<div>").addClass("remove text-center").css("cursor", "pointer").text("x").appendTo(that).on("click", removeImg);
                }
                reader.readAsDataURL($(this).get(0).files[i]);
                var formData = new FormData();
                formData.append("image:", $(this).get(0).files[i]);
                $.ajax({
                    url: '/main/ajax/UploadImages?id=' + $("#Product_ProductUniqueNum").val(),
                    type: "POST",
                    contentType: false,
                    processData: false,
                    data: formData,
                    success: function (result) {
                    },
                    error: function (err) {
                    }
                });
            }
        }
        getCount();
    });
    $("#editProduct").on("click", function (e) {
        e.preventDefault();
        let count = 0;
        let descString = "";
        let headArr = [];
        $("strong").each(function () {
            headArr.push($(this).text());
        })
        for (var q = 1; q < headArr.length; q++) {
            if (headArr[q - 1].toUpperCase() === headArr[q - 1] && headArr[q].toUpperCase() === headArr[q]) {
                alert("არსებობს 2 სათაური ერთმანეთის ქვეშ, წაშალეთ ერთი.");
                return;
            }
        }
        if ($("#Category option:selected").val() < 1) {
            alert("თქვენ უნდა აირჩიოთ კატეგორია და ქვეკატეგორია.");
            return;
        }
        if ($("#Name").val().length < 1) {
            alert("თქვენ უნდა დაამატოთ პროდუქტის სახელი.");
            return;
        }
        if ($("#Cost").val().length < 1) {
            alert("თქვენ უნდა დაამატოთ პროდუქტის თანხა.");
            return;
        }
        if ($("#ShoppingLink").val().length < 1) {
            alert("თქვენ უნდა დაამატოთ პროდუქტის მაღაზიის ლინკი.");
            return;
        }
        $("strong").each(function () {
            let text = $(this).text();
            if (text.toUpperCase() === text) {
                descString += "h:" + text + ";";
            }
            if (text.startsWith("-")) {
                if (descString == "") {
                    descString += "h:;";
                }
                let lastDescString = descString.split(";");
                let lastString = lastDescString[lastDescString.length -2];
                if (!lastString.startsWith("h:")) {
                    descString += "h:;";
                }
                let desc_string = text.split(":");
                let desc = desc_string[0].replace("- ", "");
                descString += "d:" + desc + ";";
                let value = desc_string[1];
                descString += "v:" + value + "_;";
                ++count;
            }
        });
        if (count < 4) {
            alert("თქვენ უნდა დაამატოთ მინიმუმ 4 აღწერა.");
            return;
        }
        if ($(".img-list").children().length < 2) {
            alert("თქვენ უნდა დაამატოთ მინიმუმ 1 სურათი.");
            return;
        }
        $("#descStr").val(descString);
        $('#prdct').submit();

    });
    $(".remove").on("click", removeImg);
    $(".fa-eraser").on("click", function () {
        let that = $(this)
        if (confirm("გსურთ ამ ხაზის წასლა?")) {
            that.parent().remove();
        }
    })
    function getCount() {
        if (imgListItems > 1) {
            $("#more").text("მეტი " + (16 - imgListItems) + " ფოტო");
        }
        else {
            $("#more").text("მაქს. 15 სურათი");
        }
        if (imgListItems > 15) {
            $("#addImgs").css("display", "none");
        }
        if (imgListItems < 16) {
            $("#addImgs").css("display", "block");
        }
    }
    function removeImg() {
        let currentImgName = $(this).parent().attr("data-name");
        $(this).parent().remove();
        $.ajax({
            url: '/main/ajax/RemoveImage?id=' + $("#Product_ProductUniqueNum").val(),
            contentType: "application/json; charset=utf-8",
            data: { "name": currentImgName },
            type: 'GET',
            cache: false,
            success: function (result) {
            },
            error: function (err) {
            }
        });
        imgListItems--;
        getCount();
    }
})(jQuery); // End of use strict
