/// <reference path="_references.js"/>

(function () {
var PagerIDSuffix = {
    PageSizeBox: "PSB",
    PageSizeButton: "DDB",
    PageSizePopup: "PSP"
};

var ASPxClientPager = ASPx.CreateClass(ASPxClientControl, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);

        this.hasOwnerControl = false;
        this.originalWidth = null;
        this.containerOffsetWidth = 0;
        this.droppedDown = false;
        this.pageSizeItems = [];
        this.pageSizeSelectedItem = null;
        this.enableAdaptivity = false;

        this.pageSizeChangedHandler = new ASPxClientEvent();
        this.requireInlineLayout = false;
    },

    InlineInitialize: function () {
        this.originalWidth = this.GetMainElement().style.width;
        ASPxClientControl.prototype.InlineInitialize.call(this);
    },
    Initialize: function() {
        ASPxClientControl.prototype.Initialize.call(this);
        aspxGetPagersCollection().Add(this);
        if(this.requireInlineLayout) {
            var mainElement = this.GetMainElement();
            mainElement.style.display = "inline-block";
            mainElement.style.float = "none";
        }
    },

    BrowserWindowResizeSubscriber: function () {
        return ASPxClientControl.prototype.BrowserWindowResizeSubscriber.call(this) || this.hasOwnerControl;
    },
    OnBrowserWindowResize: function (evt) {
        this.AdjustControl();
    },
    GetAdjustedSizes: function () {
        if(this.hasOwnerControl) {
            var mainElement = this.GetMainElement();
            if(mainElement)
                return { width: mainElement.parentNode.offsetWidth, height: mainElement.parentNode.offsetHeight };
        }
        return ASPxClientControl.prototype.GetAdjustedSizes.call(this);
    },
    AdjustControlCore: function() {
        this.CorrectVerticalAlignment(ASPx.ClearHeight, this.GetPageSizeButtonElement, "PSB");
        this.CorrectVerticalAlignment(ASPx.ClearVerticalMargins, this.GetPageSizeButtonImage, "PSBImg");
        this.CorrectVerticalAlignment(ASPx.ClearHeight, this.GetButtonElements, "Btns");
        this.CorrectVerticalAlignment(ASPx.ClearVerticalMargins, this.GetButtonImages, "BtnImgs");
        this.CorrectVerticalAlignment(ASPx.ClearVerticalMargins, this.GetSeparatorElements, "Seps");

        this.containerOffsetWidth = this.GetContainerWidth();
        var savedDroppedDown = false;
        if(this.droppedDown && this.GetPageSizePopupMenu()) {
            this.HidePageSizeDropDown();
            savedDroppedDown = true;
        }
        if(ASPx.IsPercentageSize(this.originalWidth))
            this.AdjustControlItems();
        else if(this.hasOwnerControl)
            this.AdjustControlMinWidth();
        if(savedDroppedDown)
            this.ShowPageSizeDropDown();

        this.CorrectVerticalAlignment(ASPx.AdjustHeight, this.GetPageSizeButtonElement, "PSB");
        this.CorrectVerticalAlignment(ASPx.AdjustVerticalMargins, this.GetPageSizeButtonImage, "PSBImg");
        this.CorrectVerticalAlignment(ASPx.AdjustHeight, this.GetButtonElements, "Btns");
        this.CorrectVerticalAlignment(ASPx.AdjustVerticalMargins, this.GetButtonImages, "BtnImgs");
        this.CorrectVerticalAlignment(ASPx.AdjustVerticalMargins, this.GetSeparatorElements, "Seps");
    },
    AdjustControlMinWidth: function() {
        var mainElement = this.GetMainElement();
        if(!mainElement) return;  
            
        if(this.enableAdaptivity) {
            this.SetElementsDisplay(mainElement, "dxp-num", true);
            this.SetElementsDisplay(mainElement, "dxp-ellip", true);
            this.SetElementsDisplay(mainElement, "dxp-summary", true);

            if(this.GetAdaptiveWidth(mainElement) < this.GetMinWidth(mainElement)){
                this.SetElementsDisplay(mainElement, "dxp-num", false);
                this.SetElementsDisplay(mainElement, "dxp-ellip", false);
            }
            if(this.GetAdaptiveWidth(mainElement) < this.GetMinWidth(mainElement)){
                this.SetElementsDisplay(mainElement, "dxp-summary", false);
            }
        }
        else {
            var minWidth = this.GetMinWidth(mainElement);
            mainElement.style.minWidth = minWidth + "px";
        }
    },
    GetAdaptiveWidth: function(element){
        if(ASPx.IsPercentageSize(this.originalWidth)) 
            return element.offsetWidth;
        else if(this.hasOwnerControl)
            return element.parentNode.offsetWidth;
        else
            return 10000;
    },
    GetMinWidth: function(element){
        return this.GetItemsWidth(element) + ASPx.GetLeftRightPaddings(element) + (ASPx.Browser.HardwareAcceleration ? 1 : 0);
    },
    SetElementsDisplay: function(element, cssClass, value) {
        var elements = ASPx.GetNodesByPartialClassName(element, cssClass);
        for(var i = 0; i < elements.length; i++) 
            ASPx.SetElementDisplay(elements[i], value);
    },
    AdjustControlItems: function () {
        var mainElement = this.GetMainElement();
        mainElement.style.width = this.originalWidth;

        var spacers = [];
        for(var i = 0; i < mainElement.childNodes.length; i++) {
            var itemElement = mainElement.childNodes[i];
            if(!itemElement.tagName) continue;

            if(itemElement.className === "dxp-spacer") {
                spacers.push(itemElement);
                itemElement.style.width = "0px";
            }
        }
        this.AdjustControlMinWidth();

        if(spacers.length > 0) {
            var clientWidth = mainElement.clientWidth - ASPx.GetLeftRightPaddings(mainElement);
            var spacerWidth = Math.floor((clientWidth - this.GetItemsWidth(mainElement)) / spacers.length);
            var makeItemsFloatRight = false;
            var rightItems = [];
            for(var i = 0; i < mainElement.childNodes.length; i++) {
                var itemElement = mainElement.childNodes[i];
                if(!itemElement.tagName) continue;

                if(itemElement.className === "dxp-spacer") {
                    if(itemElement == spacers[spacers.length - 1])
                        makeItemsFloatRight = true;
                    else
                        itemElement.style.width = spacerWidth + "px";
                }
                else if(makeItemsFloatRight) {
                    if(!this.IsAdjusted())
                        rightItems.push(itemElement);
                }
            }
            this.AdjustRightFloatItems(rightItems, ASPx.GetLeftRightPaddings(mainElement));
            this.AdjustControlMinWidth();
        }
    },
    AdjustRightFloatItems: function (items, rightMargin) {
        for(var i = 0; i < items.length; i++) {
            if(i > 0)
                items[i].parentNode.insertBefore(items[i], items[i - 1]);
            items[i].className += " dxp-right";
        }
    },
    GetItemsWidth: function (mainElement) {
        var width = 0;
        for(var i = 0; i < mainElement.childNodes.length; i++)
            width += this.GetItemWidth(mainElement.childNodes[i]);
        return width;
    },
    GetItemWidth: function (item) {
        if(!item || !item.tagName || !ASPx.GetElementDisplay(item))
            return 0;
        var style = ASPx.GetCurrentStyle(item);
        var margins = ASPx.PxToInt(style.marginLeft) + ASPx.PxToInt(style.marginRight);
        if(ASPx.Browser.IE) {
            if(ASPx.Browser.Version > 8)
                return ASPx.PxToFloat(window.getComputedStyle(item, null).width) + ASPx.GetLeftRightBordersAndPaddingsSummaryValue(item) + margins;
            return item.offsetWidth + margins;
        }
        return ASPx.PxToFloat(style.width) + ASPx.GetLeftRightBordersAndPaddingsSummaryValue(item) + margins;
    },

    // Get/Set
    GetContainerWidth: function () {
        var mainElement = this.GetMainElement();
        if(mainElement && mainElement.parentNode)
            return mainElement.parentNode.offsetWidth;
        return 0;
    },
    GetPageSizeBoxID: function () {
        return this.name + "_" + PagerIDSuffix.PageSizeBox;
    },
    GetPageSizeButtonID: function () {
        return this.name + "_" + PagerIDSuffix.PageSizeButton;
    },
    GetPageSizePopupMenuID: function () {
        return this.name + "_" + PagerIDSuffix.PageSizePopup;
    },
    GetPageSizeBoxElement: function () {
        return ASPx.GetElementById(this.GetPageSizeBoxID());
    },
    GetPageSizeButtonElement: function () {
        return ASPx.GetElementById(this.GetPageSizeButtonID());
    },
    GetPageSizeButtonImage: function () {
        return ASPx.GetNodeByTagName(this.GetPageSizeButtonElement(), "IMG", 0);
    },
    GetPageSizeInputElement: function () {
        return ASPx.GetNodeByTagName(this.GetPageSizeBoxElement(), "INPUT", 0);
    },
    GetPageSizePopupMenu: function () {
        return ASPx.GetControlCollection().Get(this.GetPageSizePopupMenuID());
    },
    GetButtonElements: function () {
        return ASPx.GetNodesByClassName(this.GetMainElement(), "dxp-button");
    },
    GetButtonImages: function () {
        var images = [];
        var buttons = this.GetButtonElements();
        for(var i = 0; i < buttons.length; i++) {
            var img = ASPx.GetNodeByTagName(buttons[i], "IMG", 0);
            if(img) images.push(img);
        }
        return images;
    },
    GetSeparatorElements: function () {
        return ASPx.GetNodesByClassName(this.GetMainElement(), "dxp-sep");
    },

    TogglePageSizeDropDown: function () {
        if(!this.droppedDown)
            this.ShowPageSizeDropDown();
        else
            this.HidePageSizeDropDown();
    },
    ShowPageSizeDropDown: function () {
        this.GetPageSizePopupMenu().Show();
        this.droppedDown = true;
    },
    HidePageSizeDropDown: function () {
        this.GetPageSizePopupMenu().Hide();
        this.droppedDown = false;
    },
    ChangePageSizeInput: function (isNext) {
        var input = this.GetPageSizeInputElement();
        var index = this.GetPageSizeIndexByText(input.value);
        var count = this.pageSizeItems.length;
        if(isNext)
            index = (index < count - 1) ? (index + 1) : 0;
        else
            index = (index > 0) ? (index - 1) : (count - 1);
        input.value = this.pageSizeItems[index].text;
    },
    ChangePageSizeValue: function (value) {
        this.GetPageSizeInputElement().value = this.GetPageSizeTextByValue(value);
    },
    IsPageSizeValueChanged: function () {
        var newValue = this.GetPageSizeValueByText(this.GetPageSizeInputElement().value);
        return newValue != this.pageSizeSelectedItem.value;
    },

    // Events

    OnDocumentOnClick: function (evt) {
        var srcElement = ASPx.Evt.GetEventSource(evt);
        if(srcElement != this.GetPageSizeBoxElement() && ASPx.GetParentById(srcElement, this.GetPageSizeBoxID()) == null) {
            this.droppedDown = false;
        }
    },
    OnPageSizeClick: function (evt) {
        var self = this;
        window.setTimeout(function () {
            self.TogglePageSizeDropDown();
        }, 0);
        ASPx.SetFocus(this.GetPageSizeInputElement());
    },
    OnPageSizePopupItemClick: function (value) {
        this.ChangePageSizeValue(value);
        if(this.IsPageSizeValueChanged())
            this.OnPageSizeValueChanged();
    },
    OnPageSizeBlur: function (evt) {
        if(this.IsPageSizeValueChanged())
            this.OnPageSizeValueChanged();
    },
    OnPageSizeKeyDown: function (evt) {
        var keyCode = ASPx.Evt.GetKeyCode(evt);
        if(keyCode == ASPx.Key.Down || keyCode == ASPx.Key.Up) {
            if(evt.altKey)
                this.TogglePageSizeDropDown();
            else
                this.ChangePageSizeInput(keyCode == ASPx.Key.Down);

            if(this.droppedDown) {
                var popupMenu = this.GetPageSizePopupMenu();
                var value = this.GetPageSizeValueByText(this.GetPageSizeInputElement().value);
                var item = popupMenu.GetItemByName(value);
                popupMenu.SetSelectedItem(item);
                ASPx.Evt.PreventEvent(evt);
            }
        }
        else if(keyCode == ASPx.Key.Enter) {
            if(this.IsPageSizeValueChanged())
                this.OnPageSizeValueChanged();
            else
                this.HidePageSizeDropDown();
            return ASPx.Evt.PreventEventAndBubble(evt);
        }
        else if(keyCode == ASPx.Key.Tab) {
            this.HidePageSizeDropDown();
        }
        else if(keyCode == ASPx.Key.Esc) {
            this.HidePageSizeDropDown();
            this.GetPageSizeInputElement().value = this.pageSizeSelectedItem.text;
        }
        return true;
    },
    UpdatePageSizeSelectedItem: function() {
        this.pageSizeSelectedItem.text = this.GetPageSizeInputElement().value;
        this.pageSizeSelectedItem.value = this.GetPageSizeValueByText(this.pageSizeSelectedItem.text);
    },
    OnPageSizeValueChanged: function () {
        this.UpdatePageSizeSelectedItem();
        if(!this.pageSizeChangedHandler.IsEmpty()) {
            var popupMenu = this.GetPageSizePopupMenu();
            var menuItem = popupMenu.GetItemByName(this.pageSizeSelectedItem.value);
            var menuItemElement = popupMenu.GetItemElement(menuItem.index);
            var command = PagerIDSuffix.PageSizePopup + this.pageSizeSelectedItem.value;
            var args = new ASPxClientPagerPageSizeChangedEventArgs(menuItemElement, command);
            this.pageSizeChangedHandler.FireEvent(this, args);
        }
    },

    // Utils

    GetPageSizeIndexByText: function (text) {
        var count = this.pageSizeItems.length;
        for(var i = 0; i < count; i++) {
            if(text == this.pageSizeItems[i].text)
                return i;
        }
        return -1;
    },
    GetPageSizeTextByValue: function (value) {
        var count = this.pageSizeItems.length;
        for(var i = 0; i < count; i++) {
            if(value == this.pageSizeItems[i].value)
                return this.pageSizeItems[i].text;
        }
        return value.toString();
    },
    GetPageSizeValueByText: function (text) {
        var count = this.pageSizeItems.length;
        for(var i = 0; i < count; i++) {
            if(text == this.pageSizeItems[i].text)
                return this.pageSizeItems[i].value;
        }
        return this.pageSizeSelectedItem.value;
    }
});

var ASPxClientPagerPageSizeChangedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function (element, value) {
        this.constructor.prototype.constructor.call(this);
        this.element = element;
        this.value = value;
    }
});

var pagersCollection = null;
function aspxGetPagersCollection() {
    if(pagersCollection == null)
        pagersCollection = new ASPxClientPagersCollection();
    return pagersCollection;
}
var ASPxClientPagersCollection = ASPx.CreateClass(ASPxClientControlCollection, {
    constructor: function () {
        this.constructor.prototype.constructor.call(this);
    },
    GetCollectionType: function(){
        return "Pager";
    },
    OnDocumentOnClick: function (evt) {
        this.ForEachControl(function (control) {
            control.OnDocumentOnClick(evt);
        });
    }
});

ASPx.Evt.AttachEventToDocument("click", aspxPagerDocumentOnClick);
function aspxPagerDocumentOnClick(evt) {
    return aspxGetPagersCollection().OnDocumentOnClick(evt);
}

// SEO
function _aspxPGNavCore(element) {
    if(element != null) {
        if(element.tagName != "A") {
            var linkElement = ASPx.GetNodeByTagName(element, "A", 0);
            if(linkElement != null)
                ASPx.Url.NavigateByLink(linkElement);
        }
    }
}
ASPx.PGNav = function(evt) {
    var element = ASPx.Evt.GetEventSource(evt);
    _aspxPGNavCore(element);
    if(!ASPx.Browser.NetscapeFamily)
        evt.cancelBubble = true;
}

// PageSize event handlers
ASPx.POnPageSizeChanged = function(s, e) {
    s.SendPostBack(e.value);
}
ASPx.POnSeoPageSizeChanged = function(s, e) {
    _aspxPGNavCore(e.element);
}
ASPx.POnPageSizeBlur = function(name, evt) {
    var pager = ASPx.GetControlCollection().Get(name);
    if(pager != null)
        pager.OnPageSizeBlur(evt);
    return true;
}
ASPx.POnPageSizeKeyDown = function(name, evt) {
    var pager = ASPx.GetControlCollection().Get(name);
    if(pager != null)
        return pager.OnPageSizeKeyDown(evt);
    return true;
}
ASPx.POnPageSizeClick = function(name, evt) {
    var pager = ASPx.GetControlCollection().Get(name);
    if(pager != null)
        pager.OnPageSizeClick(evt);
}
ASPx.POnPageSizePopupItemClick = function(name, item) {
    var pager = ASPx.GetControlCollection().Get(name);
    if(pager != null) {
        pager.OnPageSizePopupItemClick(item.name);
    }
}

window.ASPxClientPager = ASPxClientPager;
window.ASPxClientPagerPageSizeChangedEventArgs = ASPxClientPagerPageSizeChangedEventArgs;

ASPx.GetPagersCollection = aspxGetPagersCollection;
})();