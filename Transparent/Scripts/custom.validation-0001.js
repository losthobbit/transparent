//custom validation rule - requirestag
jQuery.validator.addMethod("requirestag",
function (value, element, param) {
    return element.value != -1;
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