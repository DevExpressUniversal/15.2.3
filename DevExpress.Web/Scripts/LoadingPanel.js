/// <reference path="_references.js"/>

(function () {
var ASPxClientLoadingPanel = ASPx.CreateClass(ASPxClientControl, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);

        this.containerElementID = "";
        this.containerElement = null;
        this.horizontalOffset = 0;
        this.verticalOffset = 0;

        this.isTextEmpty = false;
        this.showImage = true;

        this.shown = false;
        this.currentoffsetElement = null;
        this.currentX = null;
        this.currentY = null;
    },

    Initialize: function () {
        if(this.containerElementID != "")
            this.containerElement = ASPx.GetElementById(this.containerElementID);
        this.constructor.prototype.Initialize.call(this);
    },
    SetCurrentShowArguments: function (offsetElement, x, y) {
        if(offsetElement == null)
            offsetElement = this.containerElement;
        if(offsetElement && !ASPx.IsValidElement(offsetElement))
            offsetElement = ASPx.GetElementById(offsetElement.id);
        if(offsetElement == null)
            offsetElement = document.body;

        this.currentoffsetElement = offsetElement;
        this.currentX = x;
        this.currentY = y;
    },
    ResetCurrentShowArguments: function () {
        this.currentoffsetElement = null;
        this.currentX = null;
        this.currentY = null;
    },
    SetLoadingPanelPosAndSize: function () {
        var element = this.GetMainElement();
        this.SetLoadingPanelLocation(this.currentoffsetElement, element, this.currentX, this.currentY, this.horizontalOffset, this.verticalOffset);
    },
    SetLoadingDivPosAndSize: function () {
        var element = this.GetLoadingDiv();
        if(element != null) {
            ASPx.SetElementDisplay(element, true);
            this.SetLoadingDivBounds(this.currentoffsetElement, element);
        }
    },
    ShowInternal: function (offsetElement, x, y) {
        this.SetCurrentShowArguments(offsetElement, x, y);

        var element = this.GetMainElement();
        ASPx.SetElementDisplay(element, true);
        this.SetLoadingPanelPosAndSize();
        this.SetLoadingDivPosAndSize();
        ASPx.GetControlCollection().AdjustControls(element);
        this.shown = true;
    },
    Show: function () {
        this.ShowInternal(null);
    },
    ShowInElement: function (htmlElement) {
        if(htmlElement)
            this.ShowInternal(htmlElement);
    },
    ShowInElementByID: function (id) {
        var htmlElement = ASPx.GetElementById(id);
        this.ShowInElement(htmlElement);
    },
    ShowAtPos: function (x, y) {
        this.ShowInternal(null, x, y);
    },
    SetText: function (text) {
        this.isTextEmpty = (text == null || text == "");
        var textLabel = this.GetTextLabel();
        if(textLabel)
            textLabel.innerHTML = this.isTextEmpty ? "&nbsp;" : text;
    },
    GetText: function () {
        return this.isTextEmpty ? "" : this.GetTextLabel().innerHTML;
    },
    Hide: function () {
        var element = this.GetMainElement();
        ASPx.SetElementDisplay(element, false);
        element = this.GetLoadingDiv();
        if(element != null) {
            ASPx.SetStyles(element, { width: 1, height: 1 });// B150515
            ASPx.SetElementDisplay(element, false);
        }
        this.ResetCurrentShowArguments();
        this.shown = false;
    },

    GetTextLabel: function () {
        return this.GetChildElement("TL");
    },
    GetVisible: function () {
        return ASPx.GetElementDisplay(this.GetMainElement());
    },
    SetVisible: function (visible) {
        if(visible && !this.IsVisible())
            this.Show();
        else if(!visible && this.IsVisible())
            this.Hide();
    },
    BrowserWindowResizeSubscriber: function () {
        return true;
    },
    OnBrowserWindowResize: function () {
        if(this.shown) {
            this.SetLoadingPanelPosAndSize();
            this.SetLoadingDivPosAndSize();
        }
    }
});
ASPxClientLoadingPanel.Cast = ASPxClientControl.Cast;

window.ASPxClientLoadingPanel = ASPxClientLoadingPanel;
})();