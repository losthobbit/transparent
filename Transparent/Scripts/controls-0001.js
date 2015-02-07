function addTagSelector(div, tags, enableAdd, required) {
    var selectElement = "<select id=\"TicketTagIds\" name=\"TicketTagIds\"";
    if (enableAdd)
        selectElement += " onchange=\"addTagSelectorIfRequired()\"";
    if (required)
        selectElement += "data-val=\"true\" data-val-requirestag=\"A tag is required.\" data-val-requirestag-param=\"whatever\"";
    selectElement += " class=\"tags\"><option value=\"-1\" selected>Not selected</option>";
    for (var i = 0; i < tags.length; i++) {
        var tag = tags[i];
        selectElement += "<option value=\"" + tag.id + "\">" + repeat("&nbsp;&nbsp;", tag.indent) + tag.name + "</option>";
    }
    selectElement += "</select>"
    div.append(selectElement);
}

function addTagSelectorIfRequired() {
    var div = $("#tagSelectors");
    var selectElements = div.find("select");
    for (var i = 0; i < selectElements.length; i++) {
        if (selectElements[i].value == -1)
            return;
    }
    addTagSelector(div, tags, enableAdd, selectElements.length == 0);
}