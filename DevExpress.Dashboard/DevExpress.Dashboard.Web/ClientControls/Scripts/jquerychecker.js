(function () {
    var jQueryChecker = function () {
        var message = null;
        if (!window.jQuery)
            message = "The jQuery library has not been registered on this page.\nPlease register jQuery manually or use the embedRequiredClientLibraries attribute contained within the devExpress section's settings element in the Web.config file.";
        else {
            version = window.jQuery.fn.jquery.split(".");
            if (version[0] < 1 || version[0] == 1 && version[1] < 10)// TODO: 11
                message = "The currently used jQuery library is too old. The Dashboard Viewer requires jQuery 1.11.1 or later.\nYou can allow Dashboard Viewer to automatically add and register jQuery of an appropriate version.\nTo do this, enable the embedRequiredClientLibraries attribute contained within the devExpress section's settings element in the Web.config file.";
        }
        if (message) {
            alert(message);
            if (console && console.error)   
                console.error(message);
        }
    };
    jQueryChecker();
})();