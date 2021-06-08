(function ($) {
    "use strict"; // Start of use strict
    // Geting Unique numbers of products for Add Product page
    let imgListItems = $(".img-list").children().length;
    getCount();
    let ProductUniqueNums = [];
    var http = new XMLHttpRequest();
    http.onreadystatechange = function (e) {
        if (http.readyState == 4 && http.status == 200) {
            for (var Product of JSON.parse(http.responseText)) {
                if (Product.ProductUniqueNum != null) {
                    ProductUniqueNums.push(Product.ProductUniqueNum);
                }
            }
        }
    }
    http.open("GET", "/Main/Ajax/GetUniqueNums", true);
    http.send();
    var newUniqueNum = Math.floor(100000000 + Math.random() * 900000000);
    while (jQuery.inArray(newUniqueNum, ProductUniqueNums) != -1) {
        newUniqueNum = Math.floor(100000000 + Math.random() * 900000000);
    }
    $("#prdtcUnique").val(newUniqueNum);
    $("#Category").on("change", function () {
        // Getting Kinds for Add New Product page
        $("#Kind").empty();
        var http = new XMLHttpRequest();
        http.onreadystatechange = function (e) {
            if (http.readyState == 4 && http.status == 200) {
                for (var Kind of JSON.parse(http.responseText)) {
                    $("<option>").attr("value", Kind.Id).text(Kind.Name).appendTo("#Kind");
                }
            }
        }
        http.open("GET", "/Main/Ajax/GetKinds?id=" + $(this).val(), true);
        http.send();
    });
    $("#Category").on("click", function () {
        $(this).children().first().attr("disabled", "true");
    });
    $("#dc").on("click", function (e) {
        e.preventDefault();
        $("#dcTxt").show(500);
        $("#hdTxt").show(500);
        $(this).hide();
    });
    $("#addImgs").on('click', function () {
        $('#Gallery').click();
        return false;
    });
    $("#desc").on("click", function (e) {
        e.preventDefault();
        let heading = $("#heading");
        let description = $("#description");
        let value = $("#value");
        if (heading.val().indexOf("_") > -1 || description.val().indexOf("_") > -1 || value.val().indexOf("_") > -1 || heading.val().indexOf(";") > -1 || description.val().indexOf(";") > -1 || value.val().indexOf(";") > -1) {
            alert("თქვენ არ შეგიძლიათ დაამატოთ ეს სიტყვები: ';' და '_' ");
            return;
        }
        if (description.val() == " " || description.val() == "" ) {
            alert("დაამატეთ აღწერა.");
            description.focus();
            return;
        }
        if (value.val() == " " || value.val() == "") {
            alert("დაამატეთ აღწერის მნიშვნელობა.");
            value.focus();
            return;
        }
        if (heading.val() != "" && heading.val() != " ") {
            var headDiv = $("<div>").addClass("headLine").insertBefore("#dcs");
            $("<strong>").addClass("ml-1 mb-0 dc-tx").css({ "color": "gray", "display": "inline-block" }).text(heading.val().toUpperCase()).appendTo(headDiv);
            $("<i>").addClass("fas fa-eraser ml-3").css({ "color": "#dc3545", "cursor": "pointer" }).appendTo(headDiv).on("click", function () {
                let that = $(this)
                if (confirm("გსურთ ამ ხაზის წაშლა?")) {
                    that.parent().remove();
                }
            });
        }
        var descDiv = $("<div>").addClass("descLine").insertBefore("#dcs");
        $("<strong>").addClass("ml-3 mb-0 dc-tx").css({ "color": "gray", "font-size": "13px", "display": "inline-block" }).text("- " + description.val() + ":" + value.val()).appendTo(descDiv);
        $("<i>").addClass("fas fa-eraser ml-3").css({ "color": "#dc3545", "cursor": "pointer" }).appendTo(descDiv).on("click", function () {
            let that = $(this)
            if (confirm("გსურთ ამ ხაზის წაშლა?")) {
                that.parent().remove();
            }
        });
        heading.val('');
        description.val('');
        value.val("");
        $("#dcTxt").hide(100);
        $("#hdTxt").hide(100);
        $("#dc").show(100);
    })
    $("#cls").on("click", function (e) {
        e.preventDefault();
        $("#dcTxt").hide(100);
        $("#hdTxt").hide(100);
        $("#dc").show(100);
    });
    $("#Gallery").on("change", function (e) {
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
                    var that = $("<li>").attr("data-name", $("#Product_ProductUniqueNum").val() + imgName).addClass("add-img img").css({ "background-image": 'url("' + e.target.result + '")', "cursor": "auto"}).insertBefore("#addImgs");
                    $("<div>").addClass("remove text-center").css("cursor", "pointer").text("x").appendTo(that).on("click", function () {
                        let currentImgName = $(this).parent().attr("data-name");
                        $(this).parent().remove();
                        $.ajax({
                            url: '/main/ajax/RemoveImage?id=' + $("#prdtcUnique").val(),
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
                    });
                    
                }
                reader.readAsDataURL($(this).get(0).files[i]);
                var formData = new FormData();
                formData.append("image:", $(this).get(0).files[i]);
                $.ajax({
                    url: '/main/ajax/UploadImages?id=' + newUniqueNum,
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
    $("#addProduct").on("click", function (e) {
        e.preventDefault();
        let count = 0;
        let descString = "";
        let headArr = []
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
                let lastString = lastDescString[lastDescString.length - 2];
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
        if (imgListItems < 2) {
            alert("თქვენ უნდა დაამატოთ მინიმუმ 1 სურათი.");
            return;
        }
        $("#descStr").val(descString);
        $('#prdct').submit();
    });    
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
}) (jQuery); // End of use strict
