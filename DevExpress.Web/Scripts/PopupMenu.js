/// <reference path="_references.js"/>
/// <reference path="PopupUtils.js"/>

(function () {
var ASPxClientPopupMenu = ASPx.CreateClass(ASPxClientMenuBase, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);

        this.skipNextPEMouseOutBeforePEMouseOver = false; //Opera
        this.cursorOverPopupElement = false;
        this.isPopupMenu = true;
        this.closeAction = "OuterMouseClick";
        this.popupAction = "RightMouseClick";
        this.popupElementIDList = [];
        this.popupElementList = [];
        this.lastUsedPopupElementInfo = {};
        this.popupHorizontalOffset = 0;
        this.popupVerticalOffset = 0;
        this.popupHorizontalAlign = ASPx.PopupUtils.NotSetAlignIndicator;
        this.popupVerticalAlign = ASPx.PopupUtils.NotSetAlignIndicator;
        this.isPopupFullCorrectionOn = true;
        this.left = 0;
        this.top = 0;
    },
    Initialize: function () {
        this.RemoveAllPopupElements();
        this.PopulatePopupElements();

        ASPxClientMenuBase.prototype.Initialize.call(this);
    },
    SetPopupElementReference: function (popupElement, popupElementIndex, attach) {
        if(!ASPx.IsExistsElement(popupElement)) return;
        var setReferenceFunction = attach ? ASPx.Evt.AttachEventToElement : ASPx.Evt.DetachEventFromElement;

        if(this.closeAction == "MouseOut" || this.popupAction == "MouseOver") {
            setReferenceFunction(popupElement, "mouseover", ASPx.PopupUtils.OverControl.OnMouseOver);
            setReferenceFunction(popupElement, "mouseout", ASPx.PopupUtils.OverControl.OnMouseOut);
        }
        if(this.popupAction == "RightMouseClick")
            setReferenceFunction(popupElement, "contextmenu", aspxPMOnMouseUp);
        else if(this.popupAction == "LeftMouseClick")
            setReferenceFunction(popupElement, "mouseup", aspxPMOnMouseUp);
        if(attach) {
            popupElement.DXPopupElementControl = this;
            popupElement.DXPopupElementIndex = popupElementIndex;
        } else
            popupElement.DXPopupElementControl = popupElement.DXPopupElementIndex = undefined;

    },
    GetPopupElement: function (indexPopupElement) {
        var popupElement = this.popupElementList[indexPopupElement];
        return popupElement ? popupElement : null;
    },
    GetLastShownPopupElementIndex: function () {
        return ASPx.GetDefinedValue(this.lastUsedPopupElementInfo.shownPEIndex, 0);
    },
    SetLastShownPopupElementIndex: function (popupElementIndex) {
        this.lastUsedPopupElementInfo.shownPEIndex = popupElementIndex;
    },
    GetLastOverPopupElementIndex: function () {
        return ASPx.GetDefinedValue(this.lastUsedPopupElementInfo.overPEIndex, -1);
    },
    SetLastOverPopupElementIndex: function (popupElementIndex) {
        this.lastUsedPopupElementInfo.overPEIndex = popupElementIndex;
    },
    PopulatePopupElements: function () {
        var ids = this.popupElementIDList;
        for(var i = 0; i < ids.length; i++) {
            var popupElement = ASPx.PopupUtils.FindPopupElementById(ids[i]);
            if(popupElement)
                this.AddPopupElement(popupElement);
        }
    },
    AddPopupElement: function (popupElement) {
        var popupElementIndex = this.AddPopupElementInternal(popupElement);
        this.SetPopupElementReference(popupElement, popupElementIndex, true);
    },
    RemovePopupElement: function (popupElement) {
        this.RemovePopupElementInternal(popupElement);
        this.SetPopupElementReference(popupElement, null, false);
    },
    AddPopupElementInternal: function (element) {
        for(var i = 0; i < this.popupElementList.length; i++) {
            if(!this.popupElementList[i]) {
                this.popupElementList[i] = element;
                return i;
            }
        }
        this.popupElementList.push(element);
        return this.popupElementList.length - 1;
    },
    RemovePopupElementInternal: function (element) {
        for(var i = 0; i < this.popupElementList.length; i++) {
            if(this.popupElementList[i] == element) {
                this.popupElementList[i] = null;
                return;
            }
        }
    },
    RemoveAllPopupElements: function () {
        for(var i = 0; i < this.popupElementList.length; i++)
            this.RemovePopupElement(this.popupElementList[i]);
    },
    IsMenuVisible: function () {
        var element = this.GetMainElement();
        return (element != null) ? ASPx.GetElementDisplay(element) : false;
    },
    IsVisible: function () {
        return this.isShowing || this.IsMenuVisible();
    },
    IsParentElementPositionStatic: function (indexPath) {
        return false;
    },
    GetClientSubMenuPos: function (element, indexPath, pos, isVertical, isXPos) {
        if(indexPath == "") {
            var popupPosition = null;
            if(isXPos) {
                popupPosition = ASPx.PopupUtils.GetPopupAbsoluteX(this.GetMenuMainElement(this.GetMainElement()),
                    this.GetPopupElement(this.GetLastShownPopupElementIndex()), this.popupHorizontalAlign, this.popupHorizontalOffset, pos, this.left, this.rtl, this.isPopupFullCorrectionOn);
            }
            else {
                popupPosition = ASPx.PopupUtils.GetPopupAbsoluteY(this.GetMenuMainElement(this.GetMainElement()),
                    this.GetPopupElement(this.GetLastShownPopupElementIndex()), this.popupVerticalAlign, this.popupVerticalOffset, pos, this.top, this.isPopupFullCorrectionOn);
            }
            popupPosition.position -= ASPx.GetPositionElementOffset(element, isXPos);
            return popupPosition;
        }
        return ASPxClientMenuBase.prototype.GetClientSubMenuPos.call(this, element, indexPath, pos, isVertical, isXPos);
    },

    OnItemOverTimer: function (indexPath) {
        ASPx.GetMenuCollection().ClearCurrentShowingPopupMenuName();
        if(indexPath == "") {
            ASPx.GetMenuCollection().DoHidePopupMenus(null, -1, this.name, false, "");
            ASPx.GetMenuCollection().DoShowAtCurrentPos(this.name, indexPath);
        }
        else
            ASPxClientMenuBase.prototype.OnItemOverTimer.call(this, indexPath);
    },
    DoShow: function (x, y) {
        var element = this.GetMainElement();
        if(element != null && !ASPx.GetElementDisplay(element)) {
            ASPx.GetMenuCollection().DoHidePopupMenus(null, -1, this.name, false, "");

            if(!this.isInitialized)
                this.PopulatePopupElements();

            this.isShowing = true;
            this.DoShowPopupMenu(element, x, y, "");
            this.isShowing = false;

            var link = ASPx.GetNodeByClassName(element, ASPx.AccessibilityMarkerClass);
            if(link) this.Focus();
        }
    },
    ShowPopupSubMenuAfterCallback: function (element, callbackResult) {
        this.SetSubMenuInnerHtml(element, callbackResult);
        var indexPath = this.GetIndexPathById(element.id, true);
        var scrollHelper = this.scrollHelpers[indexPath];
        if(scrollHelper) {
            element.style.height = "";
            this.PrepareScrolling(element, scrollHelper, ASPx.PxToInt(element.style.top));
        }
        ASPx.GetControlCollection().AdjustControls(element);
    },
    WrongEventOrderOperaHack: function () {
        this.skipNextPEMouseOutBeforePEMouseOver = true;
    },
    WrongEventOrderOperaRollBack: function () {
        this.skipNextPEMouseOutBeforePEMouseOver = false;
    },
    OnAfterItemOver: function (hoverItem, hoverElement) {
        var afterItemOverAllowed = ASPxClientMenuBase.prototype.AfterItemOverAllowed(this, hoverItem);
        if(afterItemOverAllowed) {
            this.WrongEventOrderOperaHack();
            ASPxClientMenuBase.prototype.OnAfterItemOver.call(this, hoverItem, hoverElement);
        }
    },
    OnPopupElementMouseOver: function (evt, popupElement) {
        if(popupElement != null) {
            if(popupElement.DXPopupElementIndex == this.GetLastShownPopupElementIndex())
                this.cursorOverPopupElement = true
            this.WrongEventOrderOperaRollBack();

            if(this.popupAction == "MouseOver") {
                var isVisible = this.IsMenuVisible();
                if(popupElement.DXPopupElementIndex != this.GetLastOverPopupElementIndex()) {
                    this.ClearDisappearTimer();
                    this.ClearAppearTimer();
                    if(isVisible) {
                        this.Hide();
                        isVisible = false;
                    }
                }
                if(!isVisible) {
                    ASPx.GetMenuCollection().SetCurrentShowingPopupMenuName(this.name);
                    this.ShowInternal(evt, popupElement.DXPopupElementIndex);
                }
                this.SetLastOverPopupElementIndex(popupElement.DXPopupElementIndex);
            }
        }
    },
    OnPopupElementMouseOut: function (evt, popupElement) {
        if(popupElement != null) {
            ASPx.GetMenuCollection().ClearCurrentShowingPopupMenuName();
            this.cursorOverPopupElement = false;
            if(!this.IsMenuVisible())
                this.ClearAppearTimer();
            else if(!this.skipNextPEMouseOutBeforePEMouseOver)
                this.SetDisappearTimer();
        }
    },
    DoShowPopupMenuBorderCorrector: function (element, x, y, indexPath, toTheLeft, toTheTop) {
        if(indexPath != "")
            ASPxClientMenuBase.prototype.DoShowPopupMenuBorderCorrector.call(this, element, x, y, indexPath, toTheLeft, toTheTop);
    },
    ShowInternal: function (evt, popupElementIndex) {
        this.SetLastShownPopupElementIndex(popupElementIndex);
        var x = ASPx.Evt.GetEventX(evt);
        var y = ASPx.Evt.GetEventY(evt);
        if(evt.type == "mouseover")
            ASPx.GetMenuCollection().SetAppearTimer(this.name, "", this.appearAfter);
        else
            this.DoShow(x, y);
    },
    GetAnimationHorizontalDirection: function (indexPath, popupPosition) {
        if(this.GetMenuLevel(indexPath) == 0)
            return ASPx.PopupUtils.GetAnimationHorizontalDirection(popupPosition, this.popupHorizontalAlign, this.popupVerticalAlign, this.rtl);
        else
            return popupPosition.isInverted ? 1 : -1;
    },
    GetAnimationVerticalDirection: function (indexPath, popupPosition) {
        if(this.GetMenuLevel(indexPath) == 0)
            return ASPx.PopupUtils.GetAnimationVerticalDirection(popupPosition, this.popupHorizontalAlign, this.popupVerticalAlign);
        else
            return 0;
    },
    OnHideByItemOut: function () {
        if(this.closeAction == "MouseOut" && !this.cursorOverPopupElement)
            this.Hide();
        else
            ASPxClientMenuBase.prototype.OnHideByItemOut.call(this);
    },
    SetPopupElementID: function (popupElementId) {
        this.RemoveAllPopupElements();
        this.popupElementIDList = popupElementId.split(';');

        if(this.closeAction == "MouseOut") {
            this.ClearDisappearTimer();
            this.Hide();
        }

        this.PopulatePopupElements();
        this.WrongEventOrderOperaRollBack();
    },
    GetCurrentPopupElementIndex: function () {
        var popupElement = this.GetCurrentPopupElement();
        return popupElement ? popupElement.DXPopupElementIndex : -1;
    },
    GetCurrentPopupElement: function () {
        var popupElement = this.GetPopupElement(this.GetLastShownPopupElementIndex());
        if(popupElement && popupElement.DXPopupElementControl)
            return popupElement;
        return null;
    },
    RefreshPopupElementConnection: function () {
        this.RemoveAllPopupElements();
        this.PopulatePopupElements();
    },
    Hide: function () {
        ASPx.GetMenuCollection().DoHidePopupMenus(null, -1, this.name, false, "");
    },
    Show: function (popupElementIndex) {
        if(this.GetPopupElement(popupElementIndex) != null)
            this.SetLastShownPopupElementIndex(popupElementIndex);
        this.DoShow(ASPx.InvalidPosition, ASPx.InvalidPosition);
    },
    ShowAtElement: function (htmlElement) {
        this.SetLastShownPopupElementIndex(this.AddPopupElementInternal(htmlElement));
        this.DoShow(ASPx.InvalidPosition, ASPx.InvalidPosition);
        this.RemovePopupElementInternal(htmlElement);
    },
    ShowAtElementByID: function (id) {
        var htmlElement = document.getElementById(id);
        this.ShowAtElement(htmlElement);
    },
    ShowAtPos: function (x, y) {
        var lastIndexBackup = this.GetLastShownPopupElementIndex();
        this.SetLastShownPopupElementIndex(-1);
        this.DoShow(x, y);
        this.SetLastShownPopupElementIndex(lastIndexBackup);
    },

    GetVisible: function () {
        return this.IsMenuVisible();
    },
    SetVisible: function (visible) {
        if(visible && !this.IsMenuVisible())
            this.Show();
        else if(!visible && this.IsMenuVisible())
            this.Hide();
    }
});

var ASPxClientPopupMenuExt = ASPx.CreateClass(ASPxClientPopupMenu, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);

        this.INDEX_SUB_MENU_ELEMENT = 1;
        this.INDEX_MENU_ROOT_ELEMENT = 0;
    },
    InitializeMenuSamples: function() {
        var menuWrapper = this.InitializeSampleMenuElement();
        var menu = this.GetMenuRootElementSample(menuWrapper);
        this.InitializeRootItemSample(menu);
        this.InitializeMenuSamplesInternal(menu);
        this.InitializeItemSamples(menu);
        this.InitializeSubMenuSample(this.GetSubMenuElementSample(menuWrapper));
    },
    InitializeRootItemSample: function(sample) {
        this.rootMenuSample = sample.cloneNode(true);
        this.rootMenuSample.getElementsByTagName("UL")[0].innerHTML = "";
    },
    GetSubMenuElementSample: function(wrapper) {
        return wrapper.childNodes[this.INDEX_SUB_MENU_ELEMENT];
    },
    GetMenuRootElementSample: function(wrapper) {
        return wrapper.childNodes[this.INDEX_MENU_ROOT_ELEMENT];
    },
    NeedCreateItemsOnClientSide: function() {
        return true;
    }
});
ASPxClientPopupMenu.Cast = ASPxClientControl.Cast;

function aspxPMOnMouseUp(evt, element) {
    var element = ASPx.PopupUtils.FindEventSourceParentByTestFunc(evt, aspxTestPopupMenuElement);
    if(element == null || !element.DXPopupElementControl.isPopupMenu)
        return;
    var elementPopupAction = element.DXPopupElementControl.popupAction;
    switch (elementPopupAction) {
        case "LeftMouseClick":
            if(!ASPx.Evt.IsLeftButtonPressed(evt)) return;
            break;
        case "RightMouseClick":
            ASPx.PopupUtils.PreventContextMenu(evt);
    }
    ASPx.GetMenuCollection().ClearDisappearTimer();
    element.DXPopupElementControl.ShowInternal(evt, element.DXPopupElementIndex);
    return ASPx.Evt.CancelBubble(evt);
}

function aspxTestPopupMenuElement(element) {
    return !!element.DXPopupElementControl;
}
ASPx.Evt.AttachEventToDocument("mousemove", aspxPopupMenuDocumentMouseMove);
function aspxPopupMenuDocumentMouseMove(evt) {
    var element = ASPx.PopupUtils.FindEventSourceParentByTestFunc(evt, aspxTestPopupMenuElement);
    if(element != null)
        ASPx.GetMenuCollection().SaveCurrentMouseOverPos(evt, element);
}

window.ASPxClientPopupMenu = ASPxClientPopupMenu;
window.ASPxClientPopupMenuExt = ASPxClientPopupMenuExt;
})();