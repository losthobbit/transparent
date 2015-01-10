var links = [];
var hoverLink = "";
var boxHeight = 35;
var boxVerticalPadding = 17;
var boxHorizontalPadding = 20;
var boxWidth = 150;
var arrowLength = 80;

CanvasRenderingContext2D.prototype.roundRect = function (x, y, w, h, r) {
    if (w < 2 * r) r = w / 2;
    if (h < 2 * r) r = h / 2;
    this.beginPath();
    this.moveTo(x + r, y);
    this.arcTo(x + w, y, x + w, y + h, r);
    this.arcTo(x + w, y + h, x, y + h, r);
    this.arcTo(x, y + h, x, y, r);
    this.arcTo(x, y, x + w, y, r);
    this.closePath();
    return this;
}

function calculateBoxTop(row, boxesInColumn, maxBoxes) {
    var verticalPadding = (maxBoxes * (boxHeight + boxVerticalPadding) - (boxesInColumn * (boxHeight + boxVerticalPadding))) / 2;
    return verticalPadding + boxVerticalPadding + row * (boxHeight + boxVerticalPadding);
}

function drawTag(ctx, tag, row, column, boxesInColumn, maxBoxes, selected, actionUrl) {
    var boxLeft = boxHorizontalPadding + column * (boxHorizontalPadding + boxWidth + arrowLength);
    var boxTop = calculateBoxTop(row, boxesInColumn, maxBoxes);
    ctx.fillStyle = "#FFFFFF";
    ctx.roundRect(boxLeft, boxTop, 150, boxHeight, 5).fill()
    ctx.font = "12px Arial";
    ctx.fillStyle = selected ? "#FF0000" : "#000000";
    if (selected)
        ctx.fillText(tag.name, boxLeft + 10, boxTop + 20);
    else
        drawLink(ctx, boxLeft + 10, boxTop + 20, actionUrl + "/" + tag.id, tag.name);
}

function drawArrow(ctx, row, column, boxesInColumn, maxBoxes) {
    ctx.strokeStyle = "#FFFFFF";
    var x1 = column == 0 ? boxHorizontalPadding * 1.5 + boxWidth : boxHorizontalPadding * 2.5 + boxWidth * 2 + arrowLength * 2;
    var x2 = column == 0 ? x1 + arrowLength : x1 - arrowLength;
    ctx.moveTo(x1, calculateBoxTop(row, boxesInColumn, maxBoxes) + boxHeight / 2);
    ctx.lineTo(x2, calculateBoxTop(0, 1, maxBoxes) + boxHeight / 2);
    ctx.stroke();
}

function drawLink(ctx, x, y, href, title) {
    var width = ctx.measureText(title).width,
        height = parseInt(ctx.font);

    ctx.fillText(title, x, y);

    links.push({ "x": x, "y": y - 10, "width": width, "height": height, "href": href });
}

// Link hover
function canvas_mousemove(ev) {
    var x, y;

    // Get the mouse position relative to the canvas element
    if (ev.layerX || ev.layerX == 0) { // For Firefox
        x = ev.layerX;
        y = ev.layerY;
    }

    // Link hover
    for (var i = links.length - 1; i >= 0; i--) {
        var params = new Array();

        var link = links[i];

        // Check if cursor is in the link area
        if (x >= link.x && x <= (link.x + link.width) && y >= link.y && y <= (link.y + link.height)) {
            document.body.style.cursor = "pointer";
            hoverLink = link.href;
            break;
        }
        else {
            document.body.style.cursor = "";
            hoverLink = "";
        }
    };
}

function canvas_click(e) {
    if (hoverLink) {
        window.location = hoverLink;
    }
}

function drawTags(model, actionUrl) {
    var c = $("#tagDiagram")[0];
    var ctx = c.getContext("2d");
    var verticalBoxes = Math.max(1, model.parents.length, model.children.length);
    c.height = boxVerticalPadding + (boxHeight + boxVerticalPadding) * verticalBoxes;
    c.width = boxHorizontalPadding * 4 + boxWidth * 3 + arrowLength * 2;
    for (i = 0; i < model.parents.length; i++) {
        drawTag(ctx, model.parents[i], i, 0, model.parents.length, verticalBoxes, false, actionUrl);
        drawArrow(ctx, i, 0, model.parents.length, verticalBoxes);
    }
    drawTag(ctx, model, 0, 1, 1, verticalBoxes, true);
    for (i = 0; i < model.children.length; i++) {
        drawTag(ctx, model.children[i], i, 2, model.children.length, verticalBoxes, false, actionUrl);
        drawArrow(ctx, i, 2, model.children.length, verticalBoxes);
    }
    c.addEventListener("mousemove", canvas_mousemove, false);
    c.addEventListener("click", canvas_click, false);
}