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

// This is called with the results from from FB.getLoginStatus().
function statusChangeCallback(response) {
    // The response object is returned with a status field that lets the
    // app know the current login status of the person.
    // Full docs on the response object can be found in the documentation
    // for FB.getLoginStatus().
    if (response.status === 'connected') {
        // Logged into your app and Facebook.
        //response.authResponse.accessToken
        //{
        //    status: 'connected',
        //    authResponse: {
        //        accessToken: '...',
        //        expiresIn:'...',
        //        signedRequest:'...',
        //        userID:'...'
        //    }
        //}
        FB.api('/me', function (response) {
            //response.name
        });
    };
}

// This function is called when someone finishes with the Login
// Button.  See the onlogin handler attached to it in the sample
// code below.
function checkLoginState() {
    FB.getLoginStatus(function (response) {
        statusChangeCallback(response);
    });
}

window.fbAsyncInit = function () {
    FB.init({
        appId: '1508253132827931',
        cookie: true,  // enable cookies to allow the server to access 
        // the session
        xfbml: true,  // parse social plugins on this page
        version: 'v2.4' // use version 2.2
    });

    // Now that we've initialized the JavaScript SDK, we call 
    // FB.getLoginStatus().  This function gets the state of the
    // person visiting this page and can return one of three states to
    // the callback you provide.  They can be:
    //
    // 1. Logged into your app ('connected')
    // 2. Logged into Facebook, but not your app ('not_authorized')
    // 3. Not logged into Facebook and can't tell if they are logged into
    //    your app or not.
    //
    // These three cases are handled in the callback function.

    FB.getLoginStatus(function (response) {
        statusChangeCallback(response);
    });

};
