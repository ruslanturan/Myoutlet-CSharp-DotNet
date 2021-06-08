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
    $("#addImgs").on('click', function () {
        $('#Gallery').click();
        return false;
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
            if (fileSize < 40960) {
                alert("თქვენი ლოგოს მინიმალური ზომა უნდა იყოს 40kb.");
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
        if ($("iframe").contents().find('p').text() == '') {
            if ($("iframe").contents().find('li').text() == '') {
                alert("დაამატეთ აღწერილობა პროდუქტზე");
                return;
            }
        }
        if (imgListItems < 2) {
            alert("თქვენ უნდა დაამატოთ მინიმუმ 1 სურათი.");
            return;
        }
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
