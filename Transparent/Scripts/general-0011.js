// COOKIES

function setCookie(cname, cvalue, exdays) {
    var d = new Date();
    d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
    var expires = "expires=" + d.toUTCString();
    document.cookie = cname + "=" + cvalue + "; " + expires;
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1);
        if (c.indexOf(name) == 0) return c.substring(name.length, c.length);
    }
    return "";
}

// GENERAL

function repeat(stringToRepeat, count)
{
    var result = "";
    for(var i = 0; i < count; i++) {
        result += stringToRepeat;
    }
    return result;
}

var ajaxAuthorizationAttempted = function (result) {
    if (result.unauthorized) {
        window.location.href = "/Account/Login?returnUrl=" + location.pathname;
    }
}

String.prototype.replaceAll = function (find, replace) {
    var str = this;
    return str.split(find).join(replace);
};

function navigateTo(controller, action) {
    window.location.href = "/" + controller + "/" + action;
}

// DISPLAY TEXT

var texts = [];

function textById(id) {
    for (var i in texts) {
        if (id == texts[i].id) {
            return texts[i].text;
        }
    }
    return null;
}

if (!String.linkify) {
    String.prototype.linkify = function () {

        // http://, https://, ftp://
        var urlPattern = /\b(?:https?|ftp):\/\/[a-z0-9-+&@#\/%?=~_|!:,.;]*[a-z0-9-+&@#\/%=~_|]/gim;

        // www. sans http:// or https://
        var pseudoUrlPattern = /(^|[^\/])(www\.[\S]+(\b|$))/gim;

        // Email addresses
        var emailAddressPattern = /[\w.]+@[a-zA-Z_-]+?(?:\.[a-zA-Z]{2,6})+/gim;

        return this
            .replace(urlPattern, '<a href="$&" target=\"_blank\">$&</a>')
            .replace(pseudoUrlPattern, '$1<a href="http://$2" target=\"_blank\">$2</a>')
            .replace(emailAddressPattern, '<a href="mailto:$&">$&</a>');
    };
}

function textToHtml(text) {
    return text.replaceAll("\n", "<br/>").replace("\r", "").linkify();
}

function displayOrWrite(text, element) {
    text = textToHtml(text);
    if (element === undefined)
        document.write(text);
    else
        element.innerHTML = text;
}

function displayText(elementId, text, maxLength) {
    if (text == null)
        text = textById(elementId);
    if(textById(elementId) == null)
        texts.push({ "id": elementId, "text": text });
    var element = document.getElementById(elementId);
    if (maxLength === undefined || maxLength >= text.length)
        displayOrWrite(text, element);
    else 
        displayOrWrite(text.substring(0, maxLength - 3) + "<a href='javascript:displayText(\"" + elementId + "\")'>...</a>", element);
}

// Screen Size

function checkScreenSize() {
    var width = $(window).width();
    if (width < 400) {
        $(".small, .medium").addClass("xsmall").removeClass("small medium");
    }
    else if (width < 992) {
        $(".xsmall, .medium").addClass("small").removeClass("xsmall medium");
    }
    else {
        $(".xsmall, .small").addClass("medium").removeClass("xsmall small");
    }
}

$(function () {
    checkScreenSize();
    $(window).resize(function () {
        checkScreenSize();
    });
});

// Facebook

function facebookLoginCallback(response) {
    if (response.status === 'connected') {
        $("#FacebookToken").val(response.authResponse.accessToken);
        $("#loginForm").children()[0].submit();
    };
}

function facebookLoginAttempted() {
    FB.getLoginStatus(function (response) {
        facebookLoginCallback(response);
    });
}

function checkFacebookLoginCallback(response) {
    if (response.status === 'connected') {
        $("#FacebookToken").val(response.authResponse.accessToken);
        FB.api('/me', 'get', { access_token: response.authResponse.accessToken, fields: 'email' }, function(response) {
            $("#Email").val(response.email);
        });       
    };
}

function getFacebookToken() {
    FB.getLoginStatus(function (response) {
        checkFacebookLoginCallback(response);
    });
}

window.fbAsyncInit = function () {
    FB.init({
        appId: '1508253132827931',
        cookie: false,  
        xfbml: true, 
        version: 'v2.4'
    });

    if ($("#GetFacebookToken").length && $("#GetFacebookToken")[0].value == "True") {
        getFacebookToken();
    }
};

// Bootstrap

$(function () {
    $('[data-toggle="tooltip"]').tooltip();
});