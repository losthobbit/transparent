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

// SCREEN SIZE

function ignoreScreenSize() {
    setCookie("ignoreScreenSize", 1, 30);
    window.location.href = window.location.href;
}

function showInvalidScreenSize() {
    $("html").css("height", "100%");
    $("body").css("height", "100%").css("display", "flex").css("align-items", "center").css("justify-content", "center")
        .html("This site has not yet been optimized for a small screen.  Please use a larger screen."+
        "  <a onclick='ignoreScreenSize()'>Continue anyway</a>");
}

$(function () {
    if (screen.width < 1000 && getCookie("ignoreScreenSize") == "") {
        showInvalidScreenSize();
    }
});