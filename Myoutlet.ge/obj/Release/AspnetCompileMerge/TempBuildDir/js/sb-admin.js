(function($) {
  "use strict"; // Start of use strict
  // Toggle the side navigation
  $("#sidebarToggle").on('click', function(e) {
    e.preventDefault();
    $("body").toggleClass("sidebar-toggled");
    $(".sidebar").toggleClass("toggled");
  });

  // Prevent the content wrapper from scrolling when the fixed side navigation hovered over
  $('body.fixed-nav .sidebar').on('mousewheel DOMMouseScroll wheel', function(e) {
    if ($(window).width() > 768) {
      var e0 = e.originalEvent,
        delta = e0.wheelDelta || -e0.detail;
      this.scrollTop += (delta < 0 ? 1 : -1) * 30;
      e.preventDefault();
    }
  });

  // Scroll to top button appear
  $(document).on('scroll', function() {
    var scrollDistance = $(this).scrollTop();
    if (scrollDistance > 100) {
      $('.scroll-to-top').fadeIn();
    } else {
      $('.scroll-to-top').fadeOut();
    }
  });

  // Smooth scrolling using jQuery easing
    $(document).on('click', 'a.scroll-to-top', function (event) {
    var $anchor = $(this);
    $('html, body').stop().animate({scrollTop: 0}, 1000, 'easeInOutExpo');
    event.preventDefault();
  });

    $(".stsPrdtc").on("click", function (e) {
        e.preventDefault();
        if (confirm("გსურთ ამ პროდუქტს შეუცვალოთ სტატუსი?")) {
            window.location.replace($(this).attr("href"));
        }
    });

    $(".stsPrtnr").on("click", function (e) {
        e.preventDefault();
        if (confirm("გსურთ ამ პარტნიორისთვის სტატუსის შეცვლა?")) {
            window.location.replace($(this).attr("href"));
        }
    });

    $(".rmvPrtnr").on("click", function (e) {
        e.preventDefault();
        if (confirm("გსურთ ამ პარტნიორის წაშლა?")) {
            window.location.replace($(this).attr("href"));
        }
    });

    $(".edtPrdct").on("click", function (e) {
        e.preventDefault();
        if (confirm("გსურთ ამ პროდუქტის შესწორება?")) {
            window.location.replace($(this).attr("href"));
        }
    });

    $(".remvPrdct").on("click", function (e) {
        e.preventDefault();
        if (confirm("გსურთ ამ პროდუქტის წაშლა?")) {
            window.location.replace($(this).attr("href"));
        }
    });
    $("#logo").on('click', function () {
        $('#CompLogo').click();
        return false;
    });
	     var pathname = window.location.pathname.split("/");
	 if(pathname[pathname.length - 2].toUpperCase() == "SETTINGS"){
		     // Geting logo for Settings page
    var http = new XMLHttpRequest();
    http.onreadystatechange = function (e) {
        if (http.readyState == 4 && http.status == 200) {
            for (var Company of JSON.parse(http.responseText)) {
                if (Company.CompanyLogo != null) {
                    let imageUrl = "/Images/Partner-logos/" + Company.CompanyLogo;
                    $("<li>").addClass("add-img img").css({ "background-image": 'url("' + imageUrl + '")', "cursor": "auto" }).insertBefore("#logo");
                    $("#logo").children().last().text("Change logo");
                }
            }
        }
    }
    http.open("GET", "/Main/Ajax/GetLogo?id=" + $("#Id").val(), true);
    http.send();
	 }	 
	$("#ad").on('click', function (e) {
        e.preventDefault();
        if ($(".img-list").children().length < 2) {
            alert("თქვენ უნდა დაამატოთ მინიმუმ 1 სურათი.");
            return;
        }
	$('.register').submit();
    });
})(jQuery); // End of use strict
