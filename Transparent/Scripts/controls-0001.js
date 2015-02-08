var tagSelectors;

function TagSelectors(container, enableAdd, ajax, tags) {
    this.container = container;
    this.enableAdd = enableAdd;
    this.ajax = ajax;
    this.tags = tags;
    tagSelectors = this;
}

TagSelectors.prototype.add = function (required) {
    var element = document.createElement("select");
    element.setAttribute("id", "TicketTagIds");
    element.setAttribute("name", "TicketTagIds");
    var selectElement = $(element);
    if (this.enableAdd)
        selectElement.change(this.onChange);
    if (required)
        selectElement.attr({
            "data-val": "true",
            "data-val-requirestag": "A tag is required.",
            "data-val-requirestag-param": "whatever"
        });
    selectElement.addClass("tags");
    selectElement.append($(document.createElement("option"))
        .attr("value", -1)
        .attr("selected", "selected")
        .text("Not selected"));
    for (var i = 0; i < this.tags.length; i++) {
        var tag = this.tags[i];
        selectElement.append($(document.createElement("option"))
            .attr("value",tag.id)
            .text(repeat("\xA0\xA0", tag.indent) + tag.name));
    }
    this.container.append(selectElement);
}

TagSelectors.prototype.onChange = function (e) {
    tagSelectors.addIfRequired();
}

TagSelectors.prototype.addIfRequired = function () {
    var selectElements = this.container.find("select");
    for (var i = 0; i < selectElements.length; i++) {
        if (selectElements[i].value == -1)
            return;
    }
    this.add(selectElements.length == 0);
}