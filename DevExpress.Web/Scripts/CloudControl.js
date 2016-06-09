/// <reference path="_references.js"/>

(function () {
var ASPxClientCloudControl = ASPx.CreateClass(ASPxClientControl, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);
        this.ItemClick = new ASPxClientEvent();
    },
    RaiseItemClick: function (itemElement, htmlEvent) {
        var processOnServer = this.autoPostBack || this.IsServerEventAssigned("ItemClick");
        if(!this.ItemClick.IsEmpty()) {
            var name = this.getItemName(itemElement);
            var args = new ASPxClientCloudControlItemEventArgs(processOnServer, name, itemElement, htmlEvent);
            this.ItemClick.FireEvent(this, args);
            processOnServer = args.processOnServer;
        }
        return processOnServer;
    },
    OnControlClick: function (clickedElement, htmlEvent) {
        var itemElement = ASPx.GetParentByPartialClassName(clickedElement, "dxccLink");
        if(itemElement) {
            var processOnServer = this.RaiseItemClick(itemElement, htmlEvent);

            var hasItemLink = this.GetLinkElement(itemElement) != null;
            if(processOnServer && !hasItemLink) {
                var name = this.getItemName(itemElement);
                this.SendPostBack("CLICK:" + name);
            }
        }
    },
    getItemName: function(element) {
        return  element.id ? this.GetItemElementName(element) : this.GetItemElementName(element.parentElement);
    }
});
ASPxClientCloudControl.Cast = ASPxClientControl.Cast;
var ASPxClientCloudControlItemEventArgs = ASPx.CreateClass(ASPxClientProcessingModeEventArgs, {
    constructor: function (processOnServer, name, htmlElement, htmlEvent) {
        this.constructor.prototype.constructor.call(this, processOnServer);
        this.name = name;
        this.htmlElement = htmlElement;
        this.htmlEvent = htmlEvent;
    }
});

window.ASPxClientCloudControl = ASPxClientCloudControl;
window.ASPxClientCloudControlItemEventArgs = ASPxClientCloudControlItemEventArgs;
})();