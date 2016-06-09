function XafHeightAdjuster(containerEl) {
    var containerElement = $(containerEl);

    this.Adjust = function () {
        if (IsEnabled()) {
            AdjustCore();
        }
        else {
            containerElement.css("min-height", 0);
        }
    }
    function IsEnabled() {
        return $(window).width() > 1000;
    }
    function AdjustCore() {
        var difference = $(window).height() - $(document.body).height();
        if (difference != 0) {
            var minHeight = containerElement.height() + difference;
            containerElement.css("min-height", minHeight);
        }
    }

    this.Adjust();
}