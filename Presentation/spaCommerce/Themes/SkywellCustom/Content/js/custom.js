/**************************************************************/
// TODO Need to make a Initial script (in Head.cshtml) acording the app settings
// like a Model.ShoppingCartEnabled in the HeaderLinks.razor and etc.
/**************************************************************/

$(document).ready(function () {
    //$('.also-purchased-products-grid a, .related-products-grid a').click(function (e) {
    //    $('html, body').animate({ scrollTop: 0 }, 'fast');
    //});
});

var GoToPageTop = function (selector) {
    //$(selector).click(function (e) {
    //    $('html, body').animate({ scrollTop: 0 }, 'fast');
    //});
};

// Infrastructure
var SaveAsFile = function (filename, bytesBase64) {

    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
};

var CookiesService = {

    Get: function (name) {
        var nameEQ = name + "=";
        var ca = document.cookie.split(';');
        for (var i = 0; i < ca.length; i++) {
            var c = ca[i];
            while (c.charAt(0) == ' ') {
                c = c.substring(1, c.length);
            }
            if (c.indexOf(nameEQ) == 0) {
                return c.substring(nameEQ.length, c.length);
            }
        }
        return null;
    },

    Set: function (cname, cvalue, exdays) {
        var d = new Date();
        d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
        var expires = "expires=" + d.toUTCString();
        document.cookie = cname + "=" + cvalue + ";" + expires + ";path=/";
    },

    Erase: function (cname) {
        this.Set(cname, "", -1);
    }
};

var displayPopupContent = function (content, title, modal, width) {
    var isModal = (modal ? true : false);
    var targetWidth = (width ? width : 550);
    var maxHeight = $(window).height() - 20;

    $('<div></div>').html(content)
        .dialog({
            modal: isModal,
            width: targetWidth,
            maxHeight: maxHeight,
            title: title,
            close: function (event, ui) {
                $(this).dialog('destroy').remove();
            }
        });
};

var DisableElementWithWaiting = function (confirmButton, confirmButtonStub){
    $(confirmButton).hide();
    $(confirmButtonStub).show();
}

var EnableElementWithWaiting = function (confirmButton, confirmButtonStub) {
    $(confirmButton).show();
    $(confirmButtonStub).hide();
}

/**************************************************************/
// _Root.razor
window.FlyoutShoppingCart = function () {
    AjaxCart.init(false, '.header-links .cart-qty', '.header-links .wishlist-qty', '#flyout-cart');
};


// behavior of the topmenu
// TopMenu.razor

var TopMenuExpander = () => {

    $('.menu-toggle').on('click', function () {
        $(this).siblings('.top-menu.mobile').slideToggle('slow');
    });

    $('.top-menu.mobile .sublist-toggle').on('click', function () {
        $(this).siblings('.sublist').slideToggle('slow');
    });

    $('.top-menu.mobile li > a').on('click', function () {
        $('.top-menu.mobile').slideUp('slow');
    });
};

/**************************************************************/
//Footer.razor
 var FooterScript = () => {

    $('.footer-block .title').click(() => {
        var e = window, a = 'inner';
        if (!('innerWidth' in window)) {
            a = 'client';
            e = document.documentElement || document.body;
        }
        var result = { width: e[a + 'Width'], height: e[a + 'Height'] };
        if (result.width < 769) {
            $(this).siblings('.list').slideToggle('slow');
        }
    });

    $('.block .title').click(() => {
        var e = window, a = 'inner';
        if (!('innerWidth' in window)) {
            a = 'client';
            e = document.documentElement || document.body;
        }
        var result = { width: e[a + 'Width'], height: e[a + 'Height'] };
        if (result.width < 1001) {
            $(this).siblings('.listbox').slideToggle('slow');
        }
    });


     $('.footer-block.information').unbind('click');
     if (window.innerWidth <= 1001) {
         $('.footer-block.information').bind('click', function (e) {
             $('.footer-block.information .list').slideToggle(100);
         });
     }

     $('.footer-block.customer-service').unbind('click');
     if (window.innerWidth <= 1001) {
         $('.footer-block.customer-service').bind('click', function (e) {
             $('.footer-block.customer-service .list').slideToggle(100);
         });
     }

     $('.footer-block.my-account').unbind('click');
     if (window.innerWidth <= 1001) {
         $('.footer-block.my-account').bind('click', function (e) {
             $('.footer-block.my-account .list').slideToggle(100);
         });
     }

    //$('.footer-block.information a, .footer-block.customer-service a, .footer-block.my-account a').click(() => {
    //    $('html, body').animate({ scrollTop: 0 }, 'fast');
    //});
};
//$(document).ready(function() {
//    $('.footer-upper a').click(function () {
//        //$('html,body').scrollTop(0);
//        $('html, body').animate({ scrollTop: 0 }, 'fast');
//    });
//});


/**************************************************************/
// HeaderLinks.razor
//Model.ShoppingCartEnabled
var ShoppingCartScripts = () => {
    $('.header').on('mouseenter', '#topcartlink', function () {
        $('#flyout-cart').addClass('active');
    });
    $('.header').on('mouseleave', '#topcartlink', function () {
        $('#flyout-cart').removeClass('active');
    });
    $('.header').on('mouseenter', '#flyout-cart', function () {
        $('#flyout-cart').addClass('active');
    });
    $('.header').on('mouseleave', '#flyout-cart', function () {
        $('#flyout-cart').removeClass('active');
    });
};


/**************************************************************/
// CategoryNavigation.razor
// Category menu behavior
// TODO Refactore!!!!
var CategoryNavigation = {

    SetBehavior: () => {

        //it's behavior of a dropdown arrow and a sublist
        $('#nav .dropdown-arrow').unbind('click');
        $('#nav .dropdown-arrow').bind('click', function (e) {

            if ($(this).html() === "\u25bd") $(this).html("\u25b7");
            else $(this).html("\u25bd");

            $(this).siblings('.sublist.sublist-cat').slideToggle(100);
        });

        $('#nav li > a').unbind('click');
        $('#nav li > a').bind('click', function (e) {

            if ($(this).siblings('.dropdown-arrow').html() === "\u25b7")
                $(this).siblings('.dropdown-arrow').html("\u25bd");

            $(this).siblings('.sublist.sublist-cat').slideDown(100);
        });
    },

    SetUnactive: () => {
        $('#nav li').attr('class', 'inactive');
    }
};

var AdditionNavigation = {

    SetBehavior: (element) => {

        $(element).unbind('click');
        if (window.innerWidth <= 1001)
        {
            $(element).bind('click', function (e) {
                $(element + ' .listbox').slideToggle(100);
            });
        }
    }
};

/**************************************************************/
// Yandex map for ContactUs page
var YandexMapOffice = (src, placeClass) => {

    var script = document.createElement('script');
    script.src = src;
    script.async = true;
    document.getElementsByClassName(placeClass)[0].innerHTML = '';
    document.getElementsByClassName(placeClass)[0].appendChild(script);
};

/**************************************************************/
var GoogleCaptcha = {

    googleCaptchaResponse: '',

    Set: function (config, dotnetHelper) {

        // TODO refactoring code after solving https://github.com/aspnet/AspNetCore/issues/11595
        var reCaptchaCallback = function (response) {
            this.googleCaptchaResponse = response;
            //dotnetHelper.invokeMethodAsync('SetCaptchaResponse', response);
        };

        window.gcOnloadCallback = function () {
            window.grecaptcha.render(config.id, {
                'callback': reCaptchaCallback,
                'sitekey': config.siteKey,
                'theme': config.theme
            });
        };

        var script = document.createElement('script');
        script.src = config.scriptSource;
        script.async = true;
        script.type = 'text/javascript';
        var container = document.getElementById(config.id);
        container.parentNode.insertBefore(script, container.nextSibling);
    },

    GetState: function () {
        this.googleCaptchaResponse = grecaptcha.getResponse();
        return googleCaptchaResponse;
    }
};

/**************************************************************/
var SearchBox = {

    Submit: function (message) {

        if ($("#small-searchterms").val() === "") {
            alert(JSON.stringify(message));
            $("#small-searchterms").focus();
        }
    },

    Autocomplete: function (minLength, url, showImage) {

        $('#small-searchterms').autocomplete({
            delay: 500,
            minLength: minLength,
            source: url,
            appendTo: '.search-box',
            select: function (event, ui) {
                $("#small-searchterms").val(ui.item.label);
                //setLocation(ui.item.producturl);
                return false;
            }
        })
            .data("ui-autocomplete")._renderItem = function (ul, item) {
                var t = item.label;
                //html encode
                t = htmlEncode(t);
                return $("<li></li>")
                    .data("item.autocomplete", item)
                    .append((() => {
                        if (showImage) return "<a href='" + item.producturl + "'><img src='" + item.productpictureurl + "'><span>" + t + "</span></a>";
                        else return "<a href='" + item.producturl + "><span>" + t + "</span></a >";
                    })())
                    .appendTo(ul);
            };
    }
};

/**************************************************************/
var _CreateOrUpdateAddress = {

    GetStatesByCountryId: function (elementCountry, elementStateProvince, url) {

        $(elementCountry).on('change', function () {
            var selectedItem = $(this).val();
            var ddlStates = $(elementStateProvince);
            var statesProgress = $("#states-loading-progress");
            statesProgress.show();
            $.ajax({
                cache: false,
                type: "GET",
                url: url,
                data: { "countryId": selectedItem, "addSelectStateItem": "true" },
                success: function (data) {
                    ddlStates.html('');
                    $.each(data, function (id, option) {
                        ddlStates.append($('<option></option>').val(option.id).html(option.name));
                    });
                    statesProgress.hide();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert('Failed to retrieve states.');
                    statesProgress.hide();
                }
            });
        });
    }
};


/**************************************************************/
var EuCookieLaw = (acceptUrl) => {

    $('#eu-cookie-bar-notification').show();

    $('#eu-cookie-ok').on('click', function () {
        $.ajax({
            cache: false,
            type: 'POST',
            url: acceptUrl,
            dataType: 'json',
            success: function (data) {
                $('#eu-cookie-bar-notification').hide();
            },
            failure: function () {
                alert('Cannot store value');
            }
        });
    });
};

/**************************************************************/
// TopicDetails.Razor
// todo
var TopicDetailsScript = function (id, url) {
    $('#button-password').on('click', function () {
        var postData = {
            id: $("#topic-" + id).val(),
            password: $('#password').val()
        };

        $.ajax({
            cache: false,
            type: 'POST',
            url: url,
            data: postData,
            dataType: 'json',
            success: function (data) {
                if (data.Authenticated) {
                    $('#ph-topic #ph-title h1').html(data.Title);
                    if ($('#ph-topic #ph-title h1').text().length === 0) {
                        $('#ph-title').hide();
                    }
                    $('#ph-topic .page-body').html(data.Body);
                    $('#ph-password').hide();
                    $('#ph-topic').show();
                    //we need to re-run the validation plugin after the content is loaded after successful authentication
                    $.validator.unobtrusive.parse('#ph-topic');
                }
                else {
                    $('#password-error').text(data.Error).fadeIn("slow");
                    $('#ph-password #password').select().focus();
                }
            }
        });
        return false;
    });
};

$(document).ready(function () {
    $('#ph-topic').hide();
    $('#ph-password #password').select().focus();
});

/**************************************************************/
//NewsletterBox.razor

var NewsLetterSurscribe = {

    IsChecked: function () {
        if ($('#newsletter_subscribe').is(':checked')) return true;
        else return false;
    },

    SubscribeProgressShow: function () {
        $("#subscribe-loading-progress").show();
    },

    SubscribeProgressHide: function () {
        $("#subscribe-loading-progress").hide();
    },

    Success: function (data) {
        $("#subscribe-loading-progress").hide();
        $("#newsletter-result-block").html(data.result);
        if (data.success) {
            $('#newsletter-subscribe-block').hide();
            $('#newsletter-result-block').show();
        } else {
            $('#newsletter-result-block').fadeIn("slow").delay(2000).fadeOut("slow");
        }
    },

    Error: function (thrownError) {
        alert('Failed to subscribe.' + thrownError);
        $("#subscribe-loading-progress").hide();
    }
};

/**************************************************************/
// _AddToCart.razor
//when a customer clicks 'Enter' button we submit the "add to cart" button (if visible)
var AddToCartScript = function (quantityElement, productId) {
    $(quantityElement).on("keydown", function (event) {
        if (event.keyCode === 13) {
            $("#add-to-cart-button-" + productId).trigger("click");
            return false;
        }
    });
};

/**************************************************************/
// _BackInStockSubscription.razor
var BackInStockSubscriptionScript = function (id, url, local) {
    $("#back-in-stock-subscribe-" + id).on('click', function () {
        displayPopupContentFromUrl(url, local);
    });
};

/**************************************************************/
//_ProductReviewHelpfulness.razor

var ProductReviewHelpfulness = function (data) {

    $("#helpfulness-vote-yes-"+ data.ProductReviewId).html(data.TotalYes);
    $("#helpfulness-vote-no-"+ data.ProductReviewId).html(data.TotalNo);
    $("#helpfulness-vote-result-" + data.ProductReviewId).html(data.Result);

    $('#helpfulness-vote-result-'+ data.ProductReviewId).fadeIn("slow").delay(1000).fadeOut("slow");
}

/**************************************************************/
var _DeliveryInfo = function () {
    $(document).on("product_attributes_changed", function (data) {
        if (data.changedData.isFreeShipping) {
            $(".free-shipping").addClass("visible");
        } else {
            $(".free-shipping").removeClass("visible");
        }
    });
};

/**************************************************************/
var _ProductDetailsPictures = {

    MagnificPopup: function (id, tPrev, tNext, tCounter, tClose, tLoading) {
        $('#main-product-img-lightbox-anchor-' + id).magnificPopup(
            {
                type: 'image',
                removalDelay: 300,

                gallery: {
                    enabled: true,
                    tPrev: tPrev,
                    tNext: tNext,
                    tCounter: tCounter
                },

                tClose: tClose,
                tLoading: tLoading
            });
    },

    ProductDetailsPicturesScript: function (id) {

        $('.thumb-item img').on('click', function () {
            $('#main-product-img-' + id).attr('src', $(this).attr('data-defaultsize'));
            $('#main-product-img-' + id).attr('title', $(this).attr('title'));
            $('#main-product-img-' + id).attr('alt', $(this).attr('alt'));
            //$('#main-product-img-lightbox-anchor-' + id).attr('href', $(this).attr('data-fullsize'));
            $('#main-product-img-lightbox-anchor-' + id).attr('data-mfp-src', $(this).attr('data-fullsize'));
            $('#main-product-img-lightbox-anchor-' + id).attr('title', $(this).attr('title'));
        });
    }
};

/**************************************************************/
// _RentalInfo.razor

window.StartDateControl = function (startDateControlId, datePickerFormat) {
    $(startDateControlId).datepicker({ dateFormat: datePickerFormat, onSelect: onRentalDatePickerSelect });
};

window.endDateControl = function (endDateControlId, datePickerFormat) {
    $(endDateControlId).datepicker({ dateFormat: datePickerFormat, onSelect: onRentalDatePickerSelect });
};

function onRentalDatePickerSelect() {
    //    @{
    //        //almost the same implementation is used in the \Views\Product\_ProductAttributes.cshtml file
    //        var productId = Model.Id;
    //        if(catalogSettings.AjaxProcessAttributeChange)
    //                    {
    //        <text>
    //            $.ajax({
    //                cache: false,
    //                            url: '@Html.Raw(Url.Action("productdetails_attributechange", "shoppingcart", new {productId = productId, validateAttributeConditions = false, loadPicture = false }))',
    //            data: $('#product-details-form').serialize(),
    //            type: 'post',
    //                            success: function(data) {
    //                                if (data.sku) {
    //                $('#sku-@productId').text(data.sku);
    //            }
    //                                if (data.mpn) {
    //                $('#mpn-@productId').text(data.mpn);
    //            }
    //                                if (data.gtin) {
    //                $('#gtin-@productId').text(data.gtin);
    //            }
    //                                if (data.price) {
    //                $('.price-value-@productId').text(data.price);
    //            }
    //        }
    //    });
    //                        </text>
    //    }
    //}
};


/**************************************************************/
var _CheckUsernameAvailability = function (url, locale) {

    $('#check-availability-button').before('<span id="username-availabilty"></span>');
    $('#Username').on({
        keydown: function () {
            $('#username-availabilty').text('');
        }
    });
    $('#check-availability-button').on('click', function () {
        $('#username-availabilty').text('');
        if ($("#Username").val().length > 0) {
            $('#availability-check-progress').show();
            $('#check-availability-button').prop('disabled', true);

            var postData = {
                Username: $('#Username').val()
            };
            addAntiForgeryToken(postData);

            $.ajax({
                cache: false,
                type: 'POST',
                url: url,
                data: postData,
                dataType: 'json',
                success: function (data) {
                    $('#check-availability-button').prop('disabled', false);
                    $('#availability-check-progress').hide();
                    $('#username-availabilty').removeAttr('class').attr('class', data.Available ? 'username-available-status' : 'username-not-available-status');
                    $('#username-availabilty').text(data.Text);
                },
                failure: function () {
                    $('#check-availability-button').prop('disabled', false);
                    $('#availability-check-progress').hide();
                }
            });
        } else {
            $('#username-availabilty').attr('class', 'username-not-available-status');
            $('#username-availabilty').text(locale);
        }
        return false;
    });
};


/**************************************************************/
var ShareButton = function (content) {

    //var script = document.createElement('script');
    //script.src = "https://s7.addthis.com/js/250/addthis_widget.js#pubid=nopsolutions";
    //script.async = true;
    //script.type = 'text/javascript';

    //$(".product-share-button").html(
    //    '<div class="addthis_toolbox addthis_default_style ">'+
    //        '<a class= "addthis_button_preferred_1" />'+
    //        '<a class="addthis_button_preferred_2" />'+
    //        '<a class="addthis_button_preferred_3" />'+
    //        '<a class="addthis_button_preferred_4" />'+
    //        '<a class="addthis_button_compact" />'+
    //        '<a class="addthis_counter addthis_bubble_style" />'+
    //    '</div>');

    //$(".product-share-button").append(script);
    $(".product-share-button").html(content);
};

var _DiscountBox = function () {
    $('#discountcouponcode').on('keydown', function (event) {
        if (event.keyCode == 13) {
            $('#applydiscountcouponcode').trigger("click");
            return false;
        }
    });
};

var _GiftCardBox = function () {
    $('#giftcardcouponcode').on('keydown', function (event) {
        if (event.keyCode == 13) {
            $('#applygiftcardcouponcode').trigger("click");
            return false;
        }
    });
};


/**************************************************************/
var Avatar = {

    UploadAvatar: function (dotnetHelper, inputRef, url) {

        inputRef.addEventListener('change', function (e) {
            var formData = new FormData();
            formData.append('files', inputRef.files[0])

            $.ajax({

                url: url,
                type: "POST",
                data: formData,
                processData: false,
                contentType: false,

                beforeSend: function () {
                    displayAjaxLoading(true);
                },

                success: function (data) {
                    dotnetHelper.invokeMethodAsync("UploadAvatar", data)
                        .then(result => { displayAjaxLoading(false); });
                },

                error: function (xhr, ajaxOptions, thrownError) {

                    dotnetHelper.invokeMethodAsync("UploadAvatar", {

                        result: "error",
                        message: thrownError + "\r\n" + xhr.statusText + "\r\n" + xhr.responseText

                    }).then(result => { displayAjaxLoading(false); });
                }
            });
        });
    }
};

/**************************************************************/
var CheckGiftCardBalance = function () {
    $('#giftcardcouponcode').keydown(function (event) {
        if (event.keyCode == 13) {
            $('#checkbalancegiftcard').click();
            return false;
        }
    });
};

/**************************************************************/
var UserAgreement = function () {
    $("#agreement-agree").on('click', toggleContinueButton);
    toggleContinueButton();
};

function toggleContinueButton() {
    if ($('#agreement-agree').is(':checked')) {
        $('#continue-downloading').prop('disabled', false);
    } else {
        $('#continue-downloading').prop('disabled', true);
    }
};

/**************************************************************/
var GdprTools = function (message) {
    $('#delete-account').on('click', function () {
        return confirm(message);
    });
};

/**************************************************************/
var Details = {

    GoogleApi: function (element, data) {

        var script = document.createElement('script');
        script.src = "https://apis.google.com/js/platform.js?onload=renderOptIn";
        script.async = true;
        script.type = 'text/javascript';
        script.defer = true;
        var container = document.getElementById(element);
        container.parentNode.insertBefore(script, container.nextSibling);

        window.renderOptIn = function () {

            window.gapi.load('surveyoption', function () {

                window.gapi.surveyoptin.render(
                    {
                        // REQUIRED FIELDS
                        "merchant_id": data.merchant_id,
                        "order_id": data.order_id,
                        "email": data.email,
                        "delivery_country": data.delivery_country,
                        "estimated_delivery_date": data.estimated_delivery_date,
                        "products": data.products
                    });
            });
        };
    }
};

/**************************************************************/
var ShippingAddress = {

    Map: function (pickUpElement, src, pickupPoints) {

        var markers = new Map();
        var googleMap = null;
        var interval = [];

        $.getScript(src, function (data, textStatus, jqxhr) {

            google.maps.visualRefresh = true;
            googleMap = new google.maps.Map(document.getElementById("map"), {
                zoom: 15,
                mapTypeId: google.maps.MapTypeId.ROADMAP
            });

            var geocoder = new google.maps.Geocoder();
            var infowindow = new google.maps.InfoWindow();

            for (var i = 0; i < pickupPoints.Count; i++) {

                var point = pickupPoints[i];
                var value = point.Id + "___" + point.ProviderSystemName;
                var pickupPointInfo = "<div class='pickup-point-info'><ul><li><strong>" + point.Name + "</strong></li><li>" + point.OpeningHours + "</li><li>" + point.PickupFee + "</li></ul></div>";

                if (point.Latitude != null && point.Longitude != null) {
                    (function () {
                        var marker = new google.maps.Marker({
                            map: googleMap,
                            title: point.Name,
                            position: new google.maps.LatLng(point.Latitude, point.Longitude),
                            icon: "https://maps.google.com/mapfiles/ms/icons/blue-dot.png"
                        });
                        markers.set(value, marker);
                        google.maps.event.addListener(marker, 'click', function () {
                            $('#pickup-points-select').val(value);
                            googleMap.setCenter(this.getPosition());
                            infowindow.setContent(pickupPointInfo);
                            infowindow.open(googleMap, marker);
                        });
                        if (i == 0) {
                            googleMap.setCenter(marker.getPosition());
                        }
                    })();
                }
                else {

                    var address = point.Address + " " + point.City + " " + point.ZipPostalCode + " " + point.CountryName;

                    interval[i] = setInterval(function () {

                        geocoder.geocode({ 'address': address }, function (results, status) {

                            if (status === google.maps.GeocoderStatus.OK) {

                                var marker = new google.maps.Marker({
                                    map: googleMap,
                                    title: point.Name,
                                    position: results[0].geometry.location,
                                    icon: "https://maps.google.com/mapfiles/ms/icons/blue-dot.png"
                                });

                                markers.set(value, marker);

                                google.maps.event.addListener(marker, 'click', function () {
                                    $('#pickup-points-select').val(value);
                                    googleMap.setCenter(this.getPosition());
                                    infowindow.setContent(pickupPointInfo);
                                    infowindow.open(googleMap, marker);
                                });

                                if (i == 0) {

                                    googleMap.setCenter(marker.getPosition());
                                }

                                clearInterval(interval[i]);
                            }
                        });
                    }, 250);
                }
            }

            $('#pickup-points-select').on('change', function () {
                new google.maps.event.trigger(markers.get(this.value), 'click');
            });

            $(pickUpElement).on('change', function () {

                if ($(pickUpElement).is(':checked')) {

                    var center = googleMap.getCenter();
                    google.maps.event.trigger(googleMap, 'resize');
                    googleMap.setCenter(center);
                }
            });
        });    
    }
};

/**************************************************************/
var ProductAttributes = {

    FineUploader: function (controlId, dropFiles, upload, processing, cancel, retry, del, url, allowedExtensions, file) {

        if (!$("#" + controlId).length) {
            $("#product-attribute-container").append("<script type=\"text/template\" id=\"" + controlId + "-qq-template\"></script>");
        }

        /* fine uploader template(keep it synchronized to \Content\fineuploader\templates\default.html) */
        $("'#" + controlId + "'").html("<div class=\"qq-uploader-selector qq-uploader\">"
            + "<div class=\"qq-upload-drop-area-selector qq-upload-drop-area\" qq-hide1-dropzone><span>" + dropFiles + "</span></div>"
                + "<div class=\"qq-upload-button-selector qq-upload-button\"><div>" + upload + "</div></div>"
                + "<span class=\"qq-drop-processing-selector qq-drop-processing\"><span>" + processing + "</span>"
                + "<span class=\"qq-drop-processing-spinner-selector qq-drop-processing-spinner\"></span></span>"
                + "<ul class=\"qq-upload-list-selector qq-upload-list\">"
                    + "<li>"
                        + "<div class=\"qq-progress-bar-container-selector\">"
                            + "<div class=\"qq-progress-bar-selector qq-progress-bar\"></div>"
                        + "</div>"
                        + "<span class=\"qq-upload-spinner-selector qq-upload-spinner\"></span>"
                        + "<span class=\"qq-edit-filename-icon-selector qq-edit-filename-icon\"></span>"
                        + "<span class=\"qq-upload-file-selector qq-upload-file\"></span>"
                        + "<input class=\"qq-edit-filename-selector qq-edit-filename\" tabindex=\"0\" type=\"text\">"
                        + "<span class=\"qq-upload-size-selector qq-upload-size\"></span>"
                        + "<a class=\"qq-upload-cancel-selector qq-upload-cancel\" href=\"#\">" + cancel + "</a>"
                        + "<a class=\"qq-upload-retry-selector qq-upload-retry\" href=\"#\">" + retry + "</a>"
                        + "<a class=\"qq-upload-delete-selector qq-upload-delete\" href=\"#\">" + del + "</a>"
                        + "<span class=\"qq-upload-status-text-selector qq-upload-status-text\"></span>"
                    + "</li>"
                + "</ul>"
            + "</div>");

        $("#" + controlId + "uploader").fineUploader({
            request: {
                endpoint: url
            },
            template: controlId + "-qq-template",
            multiple: false,
            validation: {
                allowedExtensions: [allowedExtensions]
            }
        }).on("complete", function (event, id, name, responseJSON, xhr) {
            $("#" + controlId).val(responseJSON.downloadGuid);
            if (responseJSON.success) {
                $(controlId + "downloadurl").html("<a href='" + responseJSON.downloadUrl + "'>" + file + "</a>");
                $(controlId + "remove").show();
            }
            if (responseJSON.message) {
                alert(responseJSON.message);
            }
        });

        $(controlId + "remove").on('click', function (e) {
            $(controlId + "downloadurl").html("");
            $(controlId).val('');
            $(this).hide();
        });
    }
};

/**************************************************************/
//var Confirm = {

//    TermOfServiceWarningBox: function (element) {
//        $(element).dialog({ closeText:"" });
//    }
//};

/**************************************************************/
// Info.razor

//function removeexternalassociation(itemId) {
//    if (confirm('@T("Common.AreYouSure")')) {
//        var postData = {
//            id: itemId
//        };
//        addAntiForgeryToken(postData);
//        $.ajax({
//            cache: false,
//            type: 'POST',
//            url: '@Url.Action("RemoveExternalAssociation", "Customer")',
//            data: postData,
//            dataType: 'json',
//            success: function (data) {
//                location.href = data.redirect;
//            },
//            error: function (xhr, ajaxOptions, thrownError) {
//                alert('Failed to delete');
//            }
//        });
//    }
//    return false;
//}

/**************************************************************/
// _ProductReviewHelpfulness.razor

//window.SetProductReviewHelpfulness = function (productReviewId) {
//    $('#vote-yes-'+ productReviewId).on('click', function () {
//        setProductReviewHelpfulness@(Model.ProductReviewId)('true');
//    });
//    $('#vote-no-@(Model.ProductReviewId)').on('click', function () {
//        setProductReviewHelpfulness@(Model.ProductReviewId)('false');
//    });
//});

//function setProductReviewHelpfulness@(Model.ProductReviewId)(wasHelpful) {
//    $.ajax({
//        cache: false,
//        type: "POST",
//        url: "@(Url.RouteUrl("SetProductReviewHelpfulness"))",
//        data: { "productReviewId": @(Model.ProductReviewId), "washelpful": wasHelpful
//},
//success: function (data) {
//    $("#helpfulness-vote-yes-@(Model.ProductReviewId)").html(data.TotalYes);
//    $("#helpfulness-vote-no-@(Model.ProductReviewId)").html(data.TotalNo);
//    $("#helpfulness-vote-result-@(Model.ProductReviewId)").html(data.Result);

//    $('#helpfulness-vote-result-@(Model.ProductReviewId)').fadeIn("slow").delay(1000).fadeOut("slow");
//},
//error: function (xhr, ajaxOptions, thrownError) {
//    alert('Failed to vote. Please refresh the page and try one more time.');
//}  
//            });
//        }

/**************************************************************/
/**************************************************************/
/**************************************************************/
/**************************************************************/
/**************************************************************/
/**************************************************************/
/**************************************************************/
/**************************************************************/
/**************************************************************/
/**************************************************************/