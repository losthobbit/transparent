//custom validation rule - requirestag
jQuery.validator.addMethod("requirestag",
function (value, element, param) {
    return jQuery.grep($(element).siblings().andSelf(), function (select) {
        return select.value != -1;
    }).length > 0;
});

jQuery.validator.unobtrusive.adapters.add("requirestag", ["param"], function (options) {
    options.rules["requirestag"] = options.params.param;
    options.messages["requirestag"] = options.message;
});

function validateDynamicContent(element) {
    var currForm = element.closest("form");
    currForm.removeData("validator");
    currForm.removeData("unobtrusiveValidation");
    $.validator.unobtrusive.parse(currForm);
    currForm.validate();
}