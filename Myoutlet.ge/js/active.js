(function ($) {
    'use strict';

    var browserWindow = $(window);

    // :: 1.0 Preloader Active Code
    browserWindow.on('load', function () {
        $('.preloader').fadeOut('slow', function () {
            $(this).remove();
        });
    });

    // :: 2.0 Nav Active Code
    if ($.fn.classyNav) {
        $('#magNav').classyNav();
    }

    // :: 3.0 Sticky Active Code
    if ($.fn.sticky) {
        $("#sticker").sticky({
            topSpacing: 0
        });
    }

    // :: 4.0 Sliders Active Code
    if ($.fn.owlCarousel) {

        var welcomeSlides = $('.hero-area');

        welcomeSlides.owlCarousel({
            items: 1,
            margin: 0,
            loop: true,
            nav: true,
            navText: ['<i class="ti-angle-left"></i>', '<i class="ti-angle-right"></i>'],
            dots: false,
            autoplay: true,
            autoplayTimeout: 5000,
            smartSpeed: 1000
        });

        welcomeSlides.on('translate.owl.carousel', function () {
            var slideLayer = $("[data-animation]");
            slideLayer.each(function () {
                var anim_name = $(this).data('animation');
                $(this).removeClass('animated ' + anim_name).css('opacity', '0');
            });
        });

        welcomeSlides.on('translated.owl.carousel', function () {
            var slideLayer = welcomeSlides.find('.owl-item.active').find("[data-animation]");
            slideLayer.each(function () {
                var anim_name = $(this).data('animation');
                $(this).addClass('animated ' + anim_name).css('opacity', '1');
            });
        });

        $("[data-delay]").each(function () {
            var anim_del = $(this).data('delay');
            $(this).css('animation-delay', anim_del);
        });

        $("[data-duration]").each(function () {
            var anim_dur = $(this).data('duration');
            $(this).css('animation-duration', anim_dur);
        });

        $('.trending-post-slides').owlCarousel({
            items: 3,
            margin: 30,
            loop: true,
            nav: true,
            navText: ['<i class="ti-angle-left"></i>', '<i class="ti-angle-right"></i>'],
            dots: false,
            autoplay: true,
            autoplayTimeout: 4000,
            smartSpeed: 1000,
            responsive: {
                0: {
                    items: 1
                },
                992: {
                    items: 2
                },
                1500: {
                    items: 3
                }
            }
        });

        $('.featured-video-posts-slide').owlCarousel({
            items: 1,
            margin: 0,
            loop: true,
            nav: true,
            navText: ['<i class="ti-angle-left"></i>', '<i class="ti-angle-right"></i>'],
            dots: false,
            autoplay: true,
            autoplayTimeout: 4000,
            smartSpeed: 1000
        });

        $('.most-viewed-videos-slide').owlCarousel({
            items: 3,
            margin: 30,
            loop: true,
            nav: true,
            navText: ['<i class="ti-angle-left"></i>', '<i class="ti-angle-right"></i>'],
            dots: false,
            autoplay: true,
            autoplayTimeout: 4000,
            smartSpeed: 1000,
            responsive: {
                0: {
                    items: 1
                },
                992: {
                    items: 2
                },
                1500: {
                    items: 3
                }
            }
        });

        $('.sports-videos-slides').owlCarousel({
            items: 2,
            margin: 30,
            loop: true,
            nav: true,
            navText: ['<i class="ti-angle-left"></i>', '<i class="ti-angle-right"></i>'],
            dots: false,
            autoplay: true,
            autoplayTimeout: 4000,
            smartSpeed: 1000,
            responsive: {
                0: {
                    items: 1
                },
                992: {
                    items: 2
                },
                1500: {
                    items: 2
                }
            }
        });
        $('.sports-videos-slides_2').owlCarousel({
            items: 2,
            margin: 30,
            loop: true,
            nav: true,
            navText: ['<i class="ti-angle-left"></i>', '<i class="ti-angle-right"></i>'],
            dots: false,
            autoplay: true,
            autoplayTimeout: 4000,
            smartSpeed: 1000,
            responsive: {
                0: {
                    items: 1
                },
                992: {
                    items: 1
                },
                1500: {
                    items: 1
                }
            }
        });
    }

    // :: 5.0 ScrollUp Active Code
    if ($.fn.scrollUp) {
        browserWindow.scrollUp({
            scrollSpeed: "easeInOutExpo",
            scrollText: '<i class="ti-angle-up"></i>'
        });
    }

    // :: 6.0 Tooltip Active Code
    if ($.fn.tooltip) {
        $('[data-toggle="tooltip"]').tooltip();
    }

    // :: 7.0 Prevent Default a Click
    $('a[href="#"]').on('click', function (e) {
        e.preventDefault();
    });

    // :: 8.0 Wow Active Code
    if (browserWindow.width() > 767) {
        new WOW().init();
    }

    $("#unipay").on("click", function () {
        let desc = "";
        $(".desc").each(function () {
            desc += $(this).text().trim() + " / ";
        });
        $("#desc").attr("value", desc);
        $(".price").attr("value", $(".price").attr("value").replace(".", ""));
        if ($(".fullname").val() == "") {
            $(".fullname-error").css("display", "block");
            return;
        }
        if ($(".fullname").val().indexOf(" ") < 0) {
            $(".fullname-error").css("display", "block");
            return;
        }
        if ($(".email").val() == "") {
            $(".email-error").css("display", "block");
            return;
        }
        if ($(".email").val().indexOf("@") < 0) {
            $(".email-error").css("display", "block");
            return;
        }
        if ($(".phone").val() == "") {
            $(".phone-error").css("display", "block");
            return;
        }
        if ($(".phone").val().indexOf('5') != 0) {
            $(".phone-error").css("display", "block");
            return;
        }
        if ($(".phone").val().length < 9) {
            $(".phone-error").css("display", "block");
            return;
        }
        if ($(".address").val() == "") {
            $(".address-error").css("display", "block");
            return;
        }
        else {
            $("#uni-form").submit();
        }
    })
})(jQuery);