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

function repeat(stringToRepeat, count)
{
    var result = "";
    for(var i = 0; i < count; i++) {
        result += stringToRepeat;
    }
    return result;
}

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