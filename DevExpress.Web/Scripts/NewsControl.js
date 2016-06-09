/// <reference path="_references.js"/>

(function () {
var ASPxClientNewsControl = ASPx.CreateClass(ASPxClientDataView, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);

        this.sizingConfig.allowSetHeight = false;
        this.TailClick = new ASPxClientEvent();
    },
    DoTailClick: function (itemName, evt) {
        var clickedElement = ASPx.Evt.GetEventSource(evt);
        this.OnTailClick(clickedElement, itemName, evt);
    },
    OnTailClick: function (clickedElement, itemName, htmlEvent) {
        var itemElement = clickedElement;
        if(itemElement) {
            var processOnServer = this.RaiseTailClick(itemElement, itemName, htmlEvent);

            var hasItemLink = this.GetLinkElement(itemElement) != null;
            if(processOnServer && !hasItemLink) {
                this.SendPostBack("CLICK:" + itemName);
            }
        }
    },
    RaiseTailClick: function (tailElement, itemName, htmlEvent) {
        var processOnServer = this.autoPostBack || this.IsServerEventAssigned("TailClick");
        if(!this.TailClick.IsEmpty()) {
            var args = new ASPxClientNewsControlItemEventArgs(processOnServer, itemName, tailElement, htmlEvent);
            this.TailClick.FireEvent(this, args);
            processOnServer = args.processOnServer;
        }
        return processOnServer;
    }
});
ASPxClientNewsControl.Cast = ASPxClientControl.Cast;
var ASPxClientNewsControlItemEventArgs = ASPx.CreateClass(ASPxClientProcessingModeEventArgs, {
    constructor: function (processOnServer, name, htmlElement, htmlEvent) {
        this.constructor.prototype.constructor.call(this, processOnServer);
        this.name = name;
        this.htmlElement = htmlElement;
        this.htmlEvent = htmlEvent;
    }
});

ASPx.HLTClick = function(evt, name, itemName) {
    var control = ASPx.GetControlCollection().Get(name);
    if(control != null) control.DoTailClick(itemName, evt);
}
    
window.ASPxClientNewsControl = ASPxClientNewsControl;
window.ASPxClientNewsControlItemEventArgs = ASPxClientNewsControlItemEventArgs;
})();