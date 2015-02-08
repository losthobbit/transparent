var tagSelectors;

// TagSelector class

function TagSelector(element) {
    this.element = element;
    this.prevValue = element.value;
}

// TagSelectors class

function TagSelectors(container, enableAdd, ajax, tags, ticketId) {
    this.container = container;
    this.enableAdd = enableAdd;
    this.ajax = ajax;
    this.tags = tags;
    this.ticketId = ticketId;
    this.tagSelectors = [];
    tagSelectors = this;
}

TagSelectors_onChange = function () {
    tagSelectors.onChange(this);
}

TagSelectors.prototype.add = function (required) {
    var element = document.createElement("select");
    element.setAttribute("id", "TicketTagIds");
    element.setAttribute("name", "TicketTagIds");
    var selectElement = $(element);
    if (this.enableAdd)
        selectElement.change(TagSelectors_onChange);
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
        .text("Add a tag"));
    for (var i = 0; i < this.tags.length; i++) {
        var tag = this.tags[i];
        selectElement.append($(document.createElement("option"))
            .attr("value",tag.id)
            .text(repeat("\xA0\xA0", tag.indent) + tag.name));
    }
    this.tagSelectors.push(new TagSelector(element));
    this.container.append(selectElement);
}

TagSelectors.prototype.onChange = function (element) {
    if (this.ajax) {
        var tagSelector = $.grep(this.tagSelectors, function (tagSelector, i) { return tagSelector.element == element })[0];
        var request = { TicketId: this.ticketId, OldTagId: tagSelector.prevValue, NewTagId: tagSelector.element.value };
        $.ajax({
            type: "POST",
            data: JSON.stringify(request),
            url: "/TicketApi/UpdateTicketTag",
            contentType: "application/json"
        });
        tagSelector.prevValue = element.value;
    }
    this.addIfRequired();
}

TagSelectors.prototype.addIfRequired = function () {
    for (var i = 0; i < this.tagSelectors.length; i++) {
        if (this.tagSelectors[i].element.value == -1)
            return;
    }
    this.add(this.tagSelectors.length == 0);
}
