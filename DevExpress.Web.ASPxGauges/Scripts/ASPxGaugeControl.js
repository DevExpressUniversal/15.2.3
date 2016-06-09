

(function() {
var ASPxClientGaugeControl = ASPx.CreateClass(ASPxClientControl, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
    },
    OnCallback: function(result) {
        if(result.url) {
            var gaugeImage = this.GetMainElement();
            if(ASPx.IsExists(gaugeImage))
                gaugeImage.setAttribute("src", result.url);
        }
        this.UpdateStateObjectWithObject(result.stateObject);
    },
    ShowLoadingPanel: function() {
        this.CreateLoadingPanelWithAbsolutePosition(this.GetMainElement().parentNode, this.GetMainElement());
    },
    GetCallbackAnimationElement: function() {
        return this.GetMainElement();
    },
    PerformCallback: function(parameter) {
        this.ShowLoadingElements();    
        this.CreateCallback(parameter);
    }
}); 
ASPxClientGaugeControl.Cast = ASPxClientControl.Cast;

window.ASPxClientGaugeControl = ASPxClientGaugeControl;
})();