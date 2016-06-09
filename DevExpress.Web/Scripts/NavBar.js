/// <reference path="_references.js"/>

(function () {
var ASPxClientNavBar = ASPx.CreateClass(ASPxClientControl, {
    GROUP_TEXT_CLASSNAME: 'dxnb-ghtext',

    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);

        this.animationDuration = 200;
        this.autoCollapse = false;
        this.allowExpanding = true;
        this.allowSelectItem = false;
        this.cookieName = "";
        this.groupCount = 0;
        this.enableAnimation = false;
        this.groups = [];
        this.groupsExpanding = [];
        this.groupsExpandingRequest = [];
        this.selectedItemIndexPath = "";

        this.mouseOverActionDelay = 300;
        this.mouseOverActionTimerID = -1;
        this.ItemClick = new ASPxClientEvent();
        this.ExpandedChanged = new ASPxClientEvent();
        this.ExpandedChanging = new ASPxClientEvent();
        this.HeaderClick = new ASPxClientEvent();
    },
    InlineInitialize: function () {
        ASPxClientControl.prototype.InlineInitialize.call(this);

        this.AssignControlElementAttributes();

        this.InitializeSelectedItem();
        this.InitializeEnabledAndVisible();
    },
    InitializeEnabledAndVisible: function () {
        for(var i = 0; i < this.groups.length; i++) {
            this.SetGroupVisible(i, this.groups[i].clientVisible, true);
            this.InitializeGroupItemsEnabledAndVisible(i);
        }
    },
    InitializeGroupItemsEnabledAndVisible: function (groupIndex) {
        var group = this.groups[groupIndex];
        for(var j = 0; j < group.items.length; j++) {
            this.SetItemEnabled(groupIndex, j, group.items[j].clientEnabled, true);
            this.SetItemVisible(groupIndex, j, group.items[j].clientVisible, true);
        }
    },
    InitializeSelectedItem: function () {
        if(!this.allowSelectItem) return;
        this.SelectItem(this.GetSelectedItemIndexPath());
    },
    InitializeCallBackData: function () {
        for(var i = 0; i < this.GetGroupCountCore() ; i++) {
            if(this.groupsExpanding[i]) {
                var element = this.GetGroupContentElement(i);
                if(element != null) element.loaded = true;
            }
        }
    },
    AdjustControlCore: function () {
        this.CorrectGroupHeaderText();
        this.ShrinkWrappedText(this.GetGroupHeaderElements, "Headers");
        this.ShrinkWrappedText(this.GetItemLinkElements, "Items");
        this.CorrectWrappedText(this.GetGroupHeaderElements, "Headers");
        this.CorrectWrappedText(this.GetItemLinkElements, "Items");
        this.CorrectVerticalAlignment(ASPx.AdjustVerticalMarginsInContainer, this.GetGroupHeaderElements, "Headers", true);
    },
    CorrectGroupHeaderText: function () {
        var elements = ASPx.CacheHelper.GetCachedElements(this, "headerElementsCache", this.GetGroupHeaderElements);
        var hasHeaderButtonOnLeft;
        var textElementMargin;
        for(var i = 0; i < elements.length; i++) {
            if(elements[i].textCorrected || elements[i].offsetWidth == 0) continue;

            this.CorrectGroupHeaderTextElement(elements[i]);
            if(!elements[i].needExpandButtonReplacement) {
                elements[i].textCorrected = true;
                if(!textElementMargin) {
                    var btnElement = this.GetGroupHeaderButton(elements[i]);
                    if(btnElement) {
                        hasHeaderButtonOnLeft = ASPx.GetElementFloat(btnElement) == "left";
                        textElementMargin = this.GetGroupHeaderTextElementMargin(btnElement);
                    }
                }
            }
        }
        for(var i = 0; i < elements.length; i++) {
            if(!elements[i].textCorrected && elements[i].needExpandButtonReplacement) {
                if(textElementMargin) {
                    var textElementStyle = {};
                    textElementStyle[hasHeaderButtonOnLeft ? "marginLeft" : "marginRight"] = textElementMargin;
                    this.CorrectGroupHeaderTextElementStyle(elements[i], textElementStyle);
                    elements[i].textCorrected = true;
                }
            }
        }
    },
    CorrectGroupHeaderTextElement: function (element) {
        var btnElement = this.GetGroupHeaderButton(element);
        if(btnElement) {
            if(!ASPx.IsElementRightToLeft(this.GetMainElement()))
                this.CorrectGroupHeaderTextElementStyle(element, { marginRight: this.GetGroupHeaderTextElementMargin(btnElement) });
        }
        else
            element.needExpandButtonReplacement = true;
    },
    CorrectGroupHeaderTextElementStyle: function (element, textElementStyle) {
        var textElements = ASPx.GetChildNodesByClassName(element, "dxnb-ghtext");
        if(textElements.length > 0)
            ASPx.SetStyles(textElements[0], textElementStyle, true);
    },
    GetGroupHeaderButton: function (element) {
        var btnElements = ASPx.GetChildNodesByClassName(element, "dxnb-btn");
        if(btnElements.length == 0)
            btnElements = ASPx.GetChildNodesByClassName(element, "dxnb-btnLeft");
        return btnElements.length > 0 ? btnElements[0] : null;
    },
    GetGroupHeaderTextElementMargin: function (btnElement) {
        return btnElement ? btnElement.offsetWidth + ASPx.GetLeftRightMargins(btnElement) : 0;
    },

    //ID's forliteRender
    AssignControlElementAttributes: function () {
        var disabledCssClass = "dxnbLiteDisabled";
        var mainElement = this.GetMainElement();
        //B188844
        if(mainElement.style.width && !ASPx.IsPercentageSize(mainElement.style.width)) {
            mainElement.style.width = ASPx.PxToInt(mainElement.style.width) -
                ASPx.GetLeftRightBordersAndPaddingsSummaryValue(mainElement) + 'px';
        }
        if(ASPx.ElementContainsCssClass(mainElement, disabledCssClass)) return;
        var groups = ASPx.GetChildElementNodes(mainElement);
        var groupIndex = 0;
        for(var i = 0; i < groups.length; i++) {
            while(!this.groups[groupIndex].visible)
                groupIndex++;

            this.AssignGroupAttributes(groups[i], groupIndex);
            groupIndex++;
        }
    },
    AssignGroupAttributes: function (group, groupIndex) {
        var headers = ASPx.GetNodesByPartialClassName(group, "dxnb-header");
        for(var i = 0; i < headers.length; i++) {
            if(headers[i].id)
                continue;
            headers[i].id = this.PrepareElementID(this.GetGroupHeaderElementID(groupIndex, headers[i].className.indexOf("Collapsed") == -1));
            if(this.IsStateControllerEnabled())
                ASPx.AssignAccessabilityEventsToChildrenLinks(headers[i]);
        }
        this.AssignGroupContentAttributes(ASPx.GetNodesByPartialClassName(group, "dxnb-content")[0], groupIndex);
    },
    AssignGroupContentAttributes: function (content, groupIndex) {
        if(!content) return;
        content.id = this.PrepareElementID(this.GetGroupContentElementID(groupIndex));
        if(content.tagName == "UL") {
            var items = ASPx.GetChildElementNodes(content);
            var itemIndex = 0;
            var group = this.groups[groupIndex];
            for(var i = 0; i < items.length; i++) {
                while(!group.items[itemIndex].visible)
                    itemIndex++;
                this.AssignItemAttributes(items[i], this.GetIndexPath(groupIndex, itemIndex));
                itemIndex++;
            }
        }
    },
    AssignItemAttributes: function (item, indexPath) {
        item.id = this.PrepareElementID(this.GetItemElementID(indexPath));
        if(this.IsStateControllerEnabled())
            ASPx.AssignAccessabilityEventsToChildrenLinks(item);

        if(ASPx.ElementContainsCssClass(item, "dxnb-tmpl")) return;

        var itemLink = ASPx.GetNodesByPartialClassName(item, "dxnb-link")[0];
        if(itemLink)
            itemLink.id = this.PrepareElementID(this.GetItemLinkID(indexPath));
        var itemImg = ASPx.GetNodesByPartialClassName(item, "dxnb-img")[0];
        if(itemImg)
            itemImg.id = this.PrepareElementID(this.GetItemImageID(indexPath));
    },
    PrepareElementID: function (id) {
        return this.name + id;
    },
    GetGroupHeaderElementID: function (index, expanded) {
        return "_GH" + (expanded ? "E" : "C") + index;
    },
    GetClickableGroupHeaderElement: function (index) {
        var isDisplayed = false;
        var element = this.GetGroupHeaderExpandedElement(index);
        if(element != null) isDisplayed = !ASPx.GetElementDisplay(element) || !this.allowExpanding;

        if(!isDisplayed)
            element = this.GetGroupHeaderCollapsedElement(index);
        return element;
    },
    GetGroupContentElementID: function (index) {
        return "_GC" + index;
    },
    GetItemElementID: function (indexPath) {
        return "_I" + indexPath + "_";
    },
    GetItemLinkID: function (indexPath) {
        return this.GetItemElementID(indexPath) + "T";
    },
    GetItemImageID: function (indexPath) {
        return this.GetItemElementID(indexPath) + "Img";
    },
    GetGroupHeaderExpandedElement: function (index) {
        return this.GetChildElement(this.GetGroupHeaderElementID(index, true));
    },
    GetGroupHeaderCollapsedElement: function (index) {
        return this.GetChildElement(this.GetGroupHeaderElementID(index, false));
    },
    GetGroupHeaderElements: function () {
        var elements = [];
        elements = elements.concat(ASPx.GetNodesByClassName(this.GetMainElement(), "dxnb-header"));
        elements = elements.concat(ASPx.GetNodesByClassName(this.GetMainElement(), "dxnb-headerCollapsed"));
        return elements;
    },
    GetItemLinkElements: function () {
        return ASPx.GetNodesByClassName(this.GetMainElement(), "dxnb-link");
    },
    GetGroupContentElement: function (index) {
        return this.GetChildElement(this.GetGroupContentElementID(index));
    },
    GetGroupContentAnimationElement: function (index) {
        return this.GetGroupContentElement(index);
    },
    GetItemElement: function (groupIndex, itemIndex) {
        return this.GetItemElementByIndexPath(this.GetIndexPath(groupIndex, itemIndex));
    },
    GetItemSeparatorElement: function (groupIndex, itemIndex) {
        return this.GetChildElement("I" + this.GetIndexPath(groupIndex, itemIndex) + "S");
    },
    GetItemElementByIndexPath: function (indexPath) {
        return this.GetChildElement(this.GetItemElementID(indexPath));
    },
    GetItemTextElementByIndexPath: function (indexPath) {
        return this.GetChildElement(this.GetItemElementID(indexPath) + "T");
    },
    GetItemImageElementByIndexPath: function (indexPath) {
        return this.GetChildElement(this.GetItemElementID(indexPath) + "I");
    },
    GetGroupRow: function (groupIndex) {
        return ASPx.GetChildElementNodes(this.GetMainElement())[groupIndex];
    },
    GetGroupSeparatorRow: function (groupIndex) {
        return this.GetChildElement("GSR" + groupIndex);
    },

    DoItemClick: function (groupIndex, itemIndex, hasItemLink, htmlEvent) {
        var processOnServer = this.RaiseItemClick(groupIndex, itemIndex, htmlEvent);
        if(processOnServer && !hasItemLink)
            this.SendPostBack("CLICK:" + this.GetIndexPath(groupIndex, itemIndex));
    },
    GetAutoCollapseCoGroupIndex: function (groupIndex) {
        if(this.autoCollapse) {
            for(var i = 0; i < this.GetGroupCountCore() ; i++) {
                if(!this.groups[i].GetVisible()) continue;

                if(i != groupIndex && this.groupsExpanding[i])
                    return i;
            }
        }
        return -1;
    },
    SetExpandedInternal: function (groupIndex, expanded) {
        if(expanded == this.groupsExpanding[groupIndex] || !this.GetChildElement(this.GetGroupHeaderElementID(groupIndex, expanded)))
            return;

        var processingMode = this.RaiseExpandedChanging(groupIndex);
        if(processingMode == "Client") {
            if(expanded || !this.autoCollapse)
                this.ChangeExpanding(groupIndex, expanded);
        }
        else if(processingMode == "Server")
            this.SendPostBack("EXPAND:" + groupIndex);
    },
    ChangeExpanding: function (groupIndex, expanded) {
        var element = this.GetGroupContentElement(groupIndex);
        var autoCollapseCoGroupIndex = this.GetAutoCollapseCoGroupIndex(groupIndex);
        if(expanded && ASPx.IsFunction(this.callBack) && element != null && !element.loaded) {
            this.DoChangeExpanding(groupIndex, autoCollapseCoGroupIndex, expanded, false, false, true);
            if(!element.loading) {
                element.loading = true;
                this.groupsExpandingRequest.push(groupIndex);

                this.ShowLoadingPanelInGroup(groupIndex);
                this.CreateCallback(groupIndex);
            }
        }
        else
            this.DoChangeExpanding(groupIndex, autoCollapseCoGroupIndex, expanded, this.enableAnimation, true, true);
    },
    DoChangeExpanding: function (groupIndex, autoCollapseCoGroupIndex, expanded, doAnimation, raiseChangedEvent, raiseCoGroupChangedEvent) {
        this.ChangeGroupExpandState(groupIndex, expanded);
        if(autoCollapseCoGroupIndex > -1)
            this.ChangeGroupExpandState(autoCollapseCoGroupIndex, !expanded);
        if(doAnimation)
            this.DoChangeExpandingWithAnimation(groupIndex, autoCollapseCoGroupIndex, expanded);
        else
            this.DoChangeExpandingWithoutAnimation(groupIndex, autoCollapseCoGroupIndex, expanded, raiseChangedEvent, raiseCoGroupChangedEvent);
    },
    DoChangeExpandingWithoutAnimation: function (groupIndex, autoCollapseCoGroupIndex, expanded, raiseChangedEvent, raiseCoGroupChangedEvent) {
        this.ChangeGroupElementsExpandState(groupIndex, expanded);
        if(autoCollapseCoGroupIndex > -1)
            this.ChangeGroupElementsExpandState(autoCollapseCoGroupIndex, !expanded);

        if(raiseCoGroupChangedEvent && autoCollapseCoGroupIndex > -1)
            this.RaiseExpandedChanged(autoCollapseCoGroupIndex);
        if(raiseChangedEvent)
            this.RaiseExpandedChanged(groupIndex);
    },
    DoChangeExpandingWithAnimation: function (groupIndex, autoCollapseCoGroupIndex, expanded) {
        var controlHeight = this.GetMainElement().offsetHeight, endHeight = 0;
        var element = this.GetGroupContentAnimationElement(groupIndex);
        if(element != null) {
            element.expanding = expanded;

            endHeight = this.GetGroupContentEndHeight(element, expanded);
            this.PrepareGroupElementsForAnimation(groupIndex, expanded, 0);
        }
        if(autoCollapseCoGroupIndex > -1)
            this.PrepareGroupElementsForAnimation(autoCollapseCoGroupIndex, !expanded, controlHeight - this.GetMainElement().offsetHeight);

        if(element != null)
            this.StartAnimation(element, this.GetGroupContentStartHeight(element, expanded), endHeight, function (element) { this.OnAnimationComplete(groupIndex); }.aspxBind(this));
        if(autoCollapseCoGroupIndex > -1) {
            var groupContentElement = this.GetGroupContentAnimationElement(autoCollapseCoGroupIndex);
            if(groupContentElement)
                this.StartAnimation(groupContentElement, groupContentElement.offsetHeight, 0, function (element) { this.OnAnimationCompleteAutoCollapseGroup(autoCollapseCoGroupIndex); }.aspxBind(this));
        }
    },
    StartAnimation: function (element, from, to, onComplete) {
        var transition = ASPx.AnimationHelper.createAnimationTransition(element, {
            property: "height", unit: "px",
            duration: this.animationDuration,
            onComplete: onComplete
        });
        transition.Start(from, to);
    },
    OnAnimationCompleteAutoCollapseGroup: function (groupIndex) {
        this.ChangeGroupElementsExpandState(groupIndex, false);
        var element = this.GetGroupContentAnimationElement(groupIndex);
        if(element)
            ASPx.SetStyles(element, { height: "" })
        this.RaiseExpandedChanged(groupIndex);
    },
    OnAnimationComplete: function (groupIndex) {
        var element = this.GetGroupContentAnimationElement(groupIndex);
        if(element) {
            this.ChangeGroupElementsExpandState(groupIndex, element.expanding);
            ASPx.SetStyles(element, { overflow: "", height: "", width: "" });
            this.RaiseExpandedChanged(groupIndex);
        }
    },
    PrepareGroupElementsForAnimation: function (groupIndex, expanding, heightCorrection) {
        var element = this.GetGroupContentAnimationElement(groupIndex);
        var contentElement = this.GetGroupContentElement(groupIndex);
        if(!element || !contentElement)
            return;
        element.style.overflow = "hidden";
        if(expanding) {
            element.style.height = "0px";
            this.SetGroupElementDisplay(contentElement, true);
        }
        else {
            var height = element.offsetHeight + heightCorrection;
            if(height >= 0)
                ASPx.SetOffsetHeight(element, height);
        }
    },
    GetGroupContentEndHeight: function (element, expanding) {
        if(!expanding)
            return 0;
        var container = element;
        var dispaly = container.style.display;
        container.style.display = "";
        var height = ASPx.GetClearClientHeight(element);
        container.style.display = dispaly;
        return height;
    },
    GetGroupContentStartHeight: function (element, expanding) {
        return expanding ? 0 : element.offsetHeight
    },
    ChangeGroupExpandState: function (groupIndex, expanded) {
        this.groupsExpanding[groupIndex] = expanded;
        this.UpdateGroupStateCookie();
    },
    ChangeGroupElementsExpandState: function (groupIndex, expanded) {
        this.SetGroupElementDisplay(this.GetGroupContentElement(groupIndex), expanded);
        this.SetGroupElementDisplay(this.GetGroupHeaderExpandedElement(groupIndex), expanded);
        this.SetGroupElementDisplay(this.GetGroupHeaderCollapsedElement(groupIndex), !expanded);
        if(expanded)
            ASPx.GetControlCollection().AdjustControls(this.GetGroupContentElement(groupIndex), true);

        this.CorrectGroupHeaderText();
        this.ShrinkWrappedText(this.GetGroupHeaderElements, "Headers");
        this.ShrinkWrappedText(this.GetItemLinkElements, "Items");
        this.CorrectWrappedText(this.GetGroupHeaderElements, "Headers");
        this.CorrectWrappedText(this.GetItemLinkElements, "Items");
        this.CorrectVerticalAlignment(ASPx.AdjustVerticalMarginsInContainer, this.GetGroupHeaderElements, "Headers");
    },
    SetGroupElementDisplay: function (groupElement, value) {
        if(groupElement != null)
            ASPx.SetElementDisplay(groupElement, value);
    },
    GetGroupCountCore: function () {
        return (this.groups.length > 0) ? this.groups.length : this.groupCount;
    },
    GetIndexPath: function (groupIndex, itemIndex) {
        return (groupIndex != -1 && itemIndex != -1) ? groupIndex + ASPx.ItemIndexSeparator + itemIndex : "";
    },
    GetGroupIndex: function (indexPath) {
        var indexes = indexPath.split(ASPx.ItemIndexSeparator);
        return (indexes.length > 0) ? indexes[0] : -1;
    },
    GetItemIndex: function (indexPath) {
        var indexes = indexPath.split(ASPx.ItemIndexSeparator);
        return (indexes.length > 1) ? indexes[1] : -1;
    },

    GetGroupState: function () {
        var state = "";
        for(var i = 0; i < this.GetGroupCountCore() ; i++) {
            state += this.groupsExpanding[i] ? "1" : "0";
            if(i < this.GetGroupCountCore() - 1) state += ";";
        }
        return state;
    },
    UpdateGroupStateCookie: function () {
        if(this.cookieName == "") return;

        ASPx.Cookie.DelCookie(this.cookieName);
        ASPx.Cookie.SetCookie(this.cookieName, this.GetGroupState());
    },
    UpdateStateObject: function(){
        this.UpdateStateObjectWithObject({ selectedItemIndexPath: this.selectedItemIndexPath, groupsExpanding: this.GetGroupState() });
    },

    SelectItem: function (indexPath) {
        if(!this.IsStateControllerEnabled()) return;

        var element = this.GetSelectingItemElement(indexPath);
        if(element != null) ASPx.GetStateController().SelectElementBySrcElement(element);
    },
    DeselectItem: function (indexPath) {
        if(!this.IsStateControllerEnabled()) return;

        var element = this.GetSelectingItemElement(indexPath);
        if(element != null) ASPx.GetStateController().DeselectElementBySrcElement(element);
    },
    GetSelectingItemElement: function (indexPath) {
        return this.GetItemElementByIndexPath(indexPath);
    },
    GetSelectedItemIndexPath: function () {
        return this.selectedItemIndexPath;
    },
    SetSelectedItemInternal: function (groupIndex, itemIndex) {
        var indexPath = this.GetIndexPath(groupIndex, itemIndex);
        if(this.allowSelectItem) {
            this.DeselectItem(this.selectedItemIndexPath);
            this.selectedItemIndexPath = indexPath;
            var group = this.GetGroup(groupIndex);
            var item = group ? group.GetItem(itemIndex) : null;
            if(item == null || item.GetEnabled())
                this.SelectItem(this.selectedItemIndexPath);
        }
    },
    OnHeaderClick: function (groupIndex, evt) {
        this.ClearMouseMoverTimer();
        var processingMode = this.RaiseHeaderClick(groupIndex, evt);

        var linkElement = (evt != null) ? ASPx.GetParentByTagName(ASPx.Evt.GetEventSource(evt), "A") : null;
        if(linkElement == null || linkElement.href == ASPx.AccessibilityEmptyUrl) {
            if(processingMode == "Client" && this.allowExpanding) {
                var expanded = this.groupsExpanding[groupIndex];
                this.SetExpandedInternal(groupIndex, !expanded);
                if(evt != null && this.IsStateControllerEnabled())
                    ASPx.UpdateHoverState(evt);
            }
            else if(processingMode == "Server")
                this.SendPostBack("HEADERCLICK:" + groupIndex);
        }
    },
    OnHeaderMouseMove: function (groupIndex, evt) {
        if(MouseOverActiveNavBar != this || MouseOverActiveGroupIndex != groupIndex) {
            MouseOverActiveNavBar = this;
            MouseOverActiveGroupIndex = groupIndex;
            this.ClearMouseMoverTimer();
            this.mouseOverActionTimerID = window.setTimeout(function() {
                this.OnHeaderMouseMoveTimer(groupIndex);
            }.aspxBind(this), this.mouseOverActionDelay);
        }
    },
    OnHeaderMouseMoveTimer: function (groupIndex) {
        this.ClearMouseMoverTimer();
        if(MouseOverActiveNavBar == this || MouseOverActiveGroupIndex == groupIndex)
            this.OnHeaderClick(groupIndex, null)
    },
    ClearMouseMoverTimer: function () {
        if(this.mouseOverActionTimerID > -1) {
            ASPx.Timer.ClearTimer(this.mouseOverActionTimerID);
            this.mouseOverActionTimerID = -1;
        }
    },
    OnItemClick: function (groupIndex, itemIndex, evt) {
        var element = this.GetItemElement(groupIndex, itemIndex);
        var clickedLinkElement = ASPx.GetParentByTagName(ASPx.Evt.GetEventSource(evt), "A");
        var isLinkClicked = (clickedLinkElement != null && clickedLinkElement.href != ASPx.AccessibilityEmptyUrl);
        var linkElement = (element != null) ? (element.tagName === "A" ? element : ASPx.GetNodeByTagName(element, "A", 0)) : null;
        if(linkElement != null && linkElement.href == ASPx.AccessibilityEmptyUrl)
            linkElement = null;

        if(this.IsStateControllerEnabled())
            ASPx.ClearHoverState();
        this.SetSelectedItemInternal(groupIndex, itemIndex);
        if(this.IsStateControllerEnabled())
            ASPx.UpdateHoverState(evt);

        this.DoItemClick(groupIndex, itemIndex, isLinkClicked || (linkElement != null), evt);

        if(!isLinkClicked && linkElement != null)
            ASPx.Url.NavigateByLink(linkElement);
    },

    OnCallback: function (result) {
        this.OnCallbackInternal(result.html, result.index, false);
    },
    OnCallbackError: function (result, data) {
        this.OnCallbackInternal(result, data, true);
    },
    OnCallbackInternal: function (html, index, isError) {
        this.SetCallbackContent(html, index, isError);
        ASPx.Data.ArrayRemoveAt(this.groupsExpandingRequest, 0);

        if(this.enableCallbackAnimation)
            ASPx.AnimationHelper.fadeIn(this.GetGroupContentElement(index), function () { this.OnCallbackFinish(index, isError); }.aspxBind(this));
        else
            this.OnCallbackFinish(index, isError);
    },
    OnCallbackFinish: function (index, isError) {
        if(!isError) {
            this.InitializeGroupItemsEnabledAndVisible(index);

            this.ClearWrappedTextContainersCache();
            this.ShrinkWrappedText(this.GetItemLinkElements, "Items");
            this.CorrectWrappedText(this.GetItemLinkElements, "Items");

            this.RaiseExpandedChanged(index);
        }
    },
    OnCallbackGeneralError: function (result) {
        var callbackGroupIndex = (this.groupsExpandingRequest.length > 0) ? this.groupsExpandingRequest[0] : 0;
        this.SetCallbackContent(result, callbackGroupIndex, true);
        ASPx.Data.ArrayRemoveAt(this.groupsExpandingRequest, 0);
    },
    ShowLoadingPanelInGroup: function (groupIndex) {
        if(this.lpDelay > 0)
            window.setTimeout(function () { this.ShowLoadingPanelInGroupCore(groupIndex); }.aspxBind(this), this.lpDelay);
        else
            this.ShowLoadingPanelInGroupCore(groupIndex);
    },
    ShowLoadingPanelInGroupCore: function (groupIndex) {
        if(ASPx.Data.ArrayIndexOf(this.groupsExpandingRequest, groupIndex) < 0) return;

        var element = this.GetGroupContentElement(groupIndex);
        this.CreateLoadingPanelInsideContainer(element);
    },
    ShouldHideExistingLoadingElements: function () {
        return false;
    },
    SetCallbackContent: function (html, index, isError) {
        var replaceGroupElement = !isError;
        if(replaceGroupElement) {
            var groupElement = ASPx.GetChildElementNodes(this.GetMainElement())[index];
            ASPx.RemoveElement(this.GetGroupContentElement(index));
            ASPx.SetInnerHtml(groupElement, groupElement.innerHTML + html);
            this.AssignGroupContentAttributes(this.GetGroupContentElement(index), index);
        }
        var element = this.GetGroupContentElement(index);
        if(element != null) {
            if(!replaceGroupElement)
                ASPx.SetInnerHtml(element, html);
            if(!isError)
                element.loaded = true;
            element.loading = false;
        }
    },
    // API
    CreateGroups: function (groupsProperties) {
        for(var i = 0; i < groupsProperties.length; i++) {
            var groupName = groupsProperties[i][0] || "";
            var group = new ASPxClientNavBarGroup(this, i, groupName);
            if(ASPx.IsExists(groupsProperties[i][1]))
                group.enabled = groupsProperties[i][1];
            if(ASPx.IsExists(groupsProperties[i][2]))
                group.visible = groupsProperties[i][2];
            if(ASPx.IsExists(groupsProperties[i][3]))
                group.clientVisible = groupsProperties[i][3];
            this.groups.push(group);
            group.CreateItems(groupsProperties[i][4]);
        }
    },
    RaiseItemClick: function (groupIndex, itemIndex, htmlEvent) {
        var processOnServer = this.autoPostBack || this.IsServerEventAssigned("ItemClick");
        if(!this.ItemClick.IsEmpty()) {
            var htmlElement = this.GetItemElement(groupIndex, itemIndex);
            var args = new ASPxClientNavBarItemEventArgs(processOnServer, this.GetGroup(groupIndex).GetItem(itemIndex), htmlElement, htmlEvent);
            this.ItemClick.FireEvent(this, args);
            processOnServer = args.processOnServer;
        }
        return processOnServer;
    },
    RaiseExpandedChanged: function (groupIndex) {
        if(!this.ExpandedChanged.IsEmpty()) {
            var args = new ASPxClientNavBarGroupEventArgs(this.GetGroup(groupIndex));
            this.ExpandedChanged.FireEvent(this, args);
        }
    },
    RaiseExpandedChanging: function (groupIndex) {
        var processingMode = this.autoPostBack ? "Server" : "Client";
        if(!this.ExpandedChanging.IsEmpty()) {
            var args = new ASPxClientNavBarGroupCancelEventArgs(processingMode == "Server", this.GetGroup(groupIndex));
            this.ExpandedChanging.FireEvent(this, args);
            if(args.cancel)
                processingMode = "Handled";
            else
                processingMode = args.processOnServer ? "Server" : "Client";
        }
        return processingMode;
    },
    RaiseHeaderClick: function (groupIndex, htmlEvent) {
        var processingMode = this.autoPostBack || this.IsServerEventAssigned("HeaderClick") ? "Server" : "Client";
        if(!this.HeaderClick.IsEmpty()) {
            var htmlElement = this.GetClickableGroupHeaderElement(groupIndex);
            var args = new ASPxClientNavBarGroupClickEventArgs(processingMode == "Server", this.GetGroup(groupIndex), htmlElement, htmlEvent);
            this.HeaderClick.FireEvent(this, args);
            if(args.cancel)
                processingMode = "Handled";
            else
                processingMode = args.processOnServer ? "Server" : "Client";
        }
        return processingMode;
    },
    SetEnabled: function (enabled) {
        for(var i = this.GetGroupCount() - 1; i >= 0; i--) {
            var group = this.GetGroup(i);
            for(var j = group.GetItemCount() - 1; j >= 0; j--) {
                var item = group.GetItem(j);
                item.SetEnabled(enabled);
            }
        }
    },
    GetGroupCount: function () {
        return this.groups.length;
    },
    GetGroup: function (index) {
        return (0 <= index && index < this.groups.length) ? this.groups[index] : null;
    },
    GetGroupByName: function (name) {
        for(var i = 0; i < this.groups.length; i++)
            if(this.groups[i].name == name) return this.groups[i];
        return null;
    },
    GetActiveGroup: function () {
        if(this.autoCollapse) {
            for(var i = 0; i < this.groups.length; i++) {
                if(this.groups[i].GetVisible() && this.groups[i].GetExpanded())
                    return this.groups[i];
            }
        }
        return null;
    },
    SetActiveGroup: function (group) {
        if(this.autoCollapse && group != null && group.GetVisible())
            group.SetExpanded(true);
    },
    GetItemByName: function (name) {
        for(var i = 0; i < this.groups.length; i++) {
            var item = this.groups[i].GetItemByName(name);
            if(item != null) return item;
        }
        return null;
    },
    GetSelectedItem: function () {
        var indexPath = this.GetSelectedItemIndexPath();
        if(indexPath != "") {
            var groupIndex = this.GetGroupIndex(indexPath);
            var itemIndex = this.GetItemIndex(indexPath);
            if(groupIndex > -1 && itemIndex > -1)
                return this.GetGroup(groupIndex).GetItem(itemIndex);
        }
        return null;
    },
    SetSelectedItem: function (item) {
        var groupIndex = (item != null) ? item.group.index : -1;
        var itemIndex = (item != null) ? item.index : -1;
        if(this.IsStateControllerEnabled())
            ASPx.ClearHoverState();
        this.SetSelectedItemInternal(groupIndex, itemIndex);
    },
    CollapseAll: function () {
        for(var i = 0; i < this.groupsExpanding.length; i++) {
            if(this.groupsExpanding[i])
                this.SetExpandedInternal(i, false);
        }
    },
    ExpandAll: function () {
        for(var i = 0; i < this.groupsExpanding.length; i++) {
            if(!this.groupsExpanding[i])
                this.SetExpandedInternal(i, true);
        }
    },

    ChangeItemElementsEnabledAttributes: function (groupIndex, itemIndex, method, styleMethod) {
        var indexPath = this.GetIndexPath(groupIndex, itemIndex);
        var imageElement = this.GetItemImageElementByIndexPath(indexPath);
        if(imageElement) {
            method(imageElement, "onclick");
            styleMethod(imageElement, "cursor");
            var link = this.GetInternalHyperlinkElement(imageElement, 0);
            if(link != null) {
                method(link, "href");
                styleMethod(link, "cursor");
            }
        }
        var textElement = this.GetItemTextElementByIndexPath(indexPath);
        if(textElement) {
            method(textElement, "onclick");
            styleMethod(textElement, "cursor");
            if(textElement.tagName === "A")
                method(textElement, "href");
            var link = this.GetInternalHyperlinkElement(textElement, 0);
            if(link != null) {
                method(link, "href");
                styleMethod(link, "cursor");
                link = this.GetInternalHyperlinkElement(textElement, 1);
                if(link != null) {
                    method(link, "href");
                    styleMethod(link, "cursor");
                }
            }
        }
        var itemElement = this.GetItemElement(groupIndex, itemIndex);
        if(itemElement) {
            method(itemElement, "onclick");
            if(imageElement == null && textElement == null) {
                styleMethod(itemElement, "cursor");
                var link = this.GetInternalHyperlinkElement(itemElement, 0);
                if(link != null) {
                    method(link, "href");
                    styleMethod(link, "cursor");
                }
            }
        }
    },
    SetItemEnabled: function (groupIndex, itemIndex, enabled, initialization) {
        if(!this.groups[groupIndex].items[itemIndex].enabled) return;

        var indexPath = this.GetIndexPath(groupIndex, itemIndex);
        if(!enabled) {
            if(this.GetSelectedItemIndexPath() == indexPath)
                this.DeselectItem(indexPath);
        }

        if(!initialization || !enabled)
            this.ChangeItemEnabledStateItems(groupIndex, itemIndex, enabled);
        this.ChangeItemEnabledAttributes(groupIndex, itemIndex, enabled);

        if(enabled) {
            if(this.GetSelectedItemIndexPath() == indexPath)
                this.SelectItem(indexPath);
        }
    },
    ChangeItemEnabledAttributes: function (groupIndex, itemIndex, enabled) {
        this.ChangeItemElementsEnabledAttributes(groupIndex, itemIndex, ASPx.Attr.ChangeAttributesMethod(enabled),
            ASPx.Attr.ChangeStyleAttributesMethod(enabled));
    },
    ChangeItemEnabledStateItems: function (groupIndex, itemIndex, enabled) {
        if(!this.IsStateControllerEnabled()) return;

        var indexPath = this.GetIndexPath(groupIndex, itemIndex);
        var element = this.GetItemTextElementByIndexPath(indexPath);
        if(element == null)
            element = this.GetItemImageElementByIndexPath(indexPath);
        if(element == null)
            element = this.GetItemElement(groupIndex, itemIndex);
        if(element != null)
            ASPx.GetStateController().SetElementEnabled(element, enabled);
    },
    GetItemImageUrl: function (groupIndex, itemIndex) {
        var indexPath = this.GetIndexPath(groupIndex, itemIndex);
        var element = this.GetItemImageContainer(indexPath);
        if(element != null) {
            var img = ASPx.GetNodeByTagName(element, "IMG", 0);
            if(img != null)
                return img.src;
        }
        element = this.GetItemTextElementByIndexPath(indexPath);
        if(element != null) {
            var img = ASPx.GetNodeByTagName(element, "IMG", 0);
            if(img != null)
                return img.src;
        }
        return "";
    },
    SetItemImageUrl: function (groupIndex, itemIndex, url) {
        var indexPath = this.GetIndexPath(groupIndex, itemIndex);
        var element = this.GetItemImageContainer(indexPath);
        if(element != null) {
            var img = ASPx.GetNodeByTagName(element, "IMG", 0);
            if(img != null)
                img.src = url;
        }
        element = this.GetItemTextElementByIndexPath(indexPath);
        if(element != null) {
            var itemImageElementID = this.name + this.GetItemImageID(indexPath);
            var img = ASPx.GetChildById(element, itemImageElementID);
            if(img != null)
                img.src = url;
        }
    },
    GetItemImageContainer: function (indexPath) {
        return this.GetItemElementByIndexPath(indexPath);
    },
    GetItemNavigateUrl: function (groupIndex, itemIndex) {
        var element = this.GetItemElement(groupIndex, itemIndex);
        if(element != null) {
            var link = ASPx.GetNodeByTagName(element, "A", 0);
            if(link != null)
                return link.href || ASPx.Attr.GetAttribute(link, "savedhref");
        }
        return "";
    },
    SetItemNavigateUrl: function (groupIndex, itemIndex, url) {
        var element = this.GetItemElement(groupIndex, itemIndex);
        if(element != null) {
            var setUrl = function(link) {
                if(link != null) {
                    if(ASPx.Attr.IsExistsAttribute(link, "savedhref"))
                        ASPx.Attr.SetAttribute(link, "savedhref", url);
                    else if(ASPx.Attr.IsExistsAttribute(link, "href"))
                        link.href = url;
                }
            };
            setUrl(ASPx.GetNodeByTagName(element, "A", 0));
            setUrl(ASPx.GetNodeByTagName(element, "A", 1));
        }
    },
    GetItemText: function (groupIndex, itemIndex) {
        var indexPath = this.GetIndexPath(groupIndex, itemIndex);
        var element = this.GetItemTextElementByIndexPath(indexPath);
        if(element == null)
            element = this.GetItemElement(groupIndex, itemIndex);
        if(element != null) {
            var textNode = ASPx.GetTextNode(element);
            if(textNode != null)
                return textNode.nodeValue;
        }
        return "";
    },
    SetItemText: function (groupIndex, itemIndex, text) {
        var indexPath = this.GetIndexPath(groupIndex, itemIndex);
        var element = this.GetItemTextElementByIndexPath(indexPath);
        if(element == null)
            element = this.GetItemElement(groupIndex, itemIndex);
        if(element != null) {
            var textNode = ASPx.GetTextNode(element);
            if(textNode != null)
                textNode.nodeValue = text;
        }
    },
    SetItemVisible: function (groupIndex, itemIndex, visible, initialization) {
        if(!this.groups[groupIndex].items[itemIndex].visible) return;
        if(visible && initialization) return;

        var element = this.GetItemElement(groupIndex, itemIndex);
        if(element != null) ASPx.SetElementDisplay(element, visible);

        this.SetItemSeparatorsVisiblility(groupIndex);
    },
    SetItemSeparatorsVisiblility: function (groupIndex) {
        var group = this.groups[groupIndex];
        for(var i = 0; i < group.items.length; i++) {
            var separatorVisible = group.items[i].GetVisible() && this.HasNextVisibleItems(group, i);
            var separatorElement = this.GetItemSeparatorElement(groupIndex, i);
            if(separatorElement != null) ASPx.SetElementDisplay(separatorElement, separatorVisible);
        }
    },
    HasNextVisibleItems: function (group, index) {
        for(var i = index + 1; i < group.items.length; i++) {
            if(group.items[i].GetVisible())
                return true;
        }
        return false;
    },
    SetGroupVisible: function (groupIndex, visible, initialization) {
        if(!this.groups[groupIndex].visible) return;
        if(visible && initialization) return;

        var element = this.GetGroupRow(groupIndex);
        if(element != null) ASPx.SetElementDisplay(element, visible);

        this.SetGroupSeparatorsVisiblility();
    },
    GetGroupText: function (groupIndex) {
        var groupRow = this.GetGroupRow(groupIndex);

        if(!groupRow)
            return;

        var groupTextElement = ASPx.GetNodeByClassName(groupRow, this.GROUP_TEXT_CLASSNAME);

        if(!groupTextElement)
            return;

        return ASPx.GetInnerText(groupTextElement);
    },
    SetGroupText: function (groupIndex, text) {
        var groupRow = this.GetGroupRow(groupIndex);

        if(!groupRow)
            return;

        var groupTextElements = ASPx.GetNodesByClassName(groupRow, this.GROUP_TEXT_CLASSNAME);

        for(var i = 0; i < groupTextElements.length; i++) {
            var textNode = ASPx.GetTextNode(groupTextElements[i]);
            if(textNode != null)
                textNode.nodeValue = text;
        }
    },
    SetGroupSeparatorsVisiblility: function () {
        for(var i = 0; i < this.groups.length; i++) {
            var separatorVisible = this.groups[i].GetVisible() && this.HasNextVisibleGroups(i);
            var separatorElement = this.GetGroupSeparatorRow(i);
            if(separatorElement != null) ASPx.SetElementDisplay(separatorElement, separatorVisible);
        }
    },
    HasNextVisibleGroups: function (index) {
        for(var i = index + 1; i < this.groups.length; i++) {
            if(this.groups[i].GetVisible())
                return true;
        }
        return false;
    }
});
ASPxClientNavBar.Cast = ASPxClientControl.Cast;
var ASPxClientNavBarGroup = ASPx.CreateClass(null, {
    constructor: function (navBar, index, name) {
        this.navBar = navBar;
        this.index = index;
        this.name = name;

        this.enabled = true;
        this.visible = true;
        this.clientVisible = true;
        this.items = [];
    },
    CreateItems: function (itemsProperties) {
        for(var i = 0; i < itemsProperties.length; i++) {
            var itemName = itemsProperties[i][0] || "";
            var item = new ASPxClientNavBarItem(this.navBar, this, i, itemName);
            if(ASPx.IsExists(itemsProperties[i][1]))
                item.enabled = itemsProperties[i][1];
            if(ASPx.IsExists(itemsProperties[i][2]))
                item.clientEnabled = itemsProperties[i][2];
            if(ASPx.IsExists(itemsProperties[i][3]))
                item.visible = itemsProperties[i][3];
            if(ASPx.IsExists(itemsProperties[i][4]))
                item.clientVisible = itemsProperties[i][4];
            this.items.push(item);
        }
    },
    GetEnabled: function () {
        return this.enabled;
    },
    GetExpanded: function () {
        return this.navBar.groupsExpanding[this.index];
    },
    SetExpanded: function (value) {
        this.navBar.SetExpandedInternal(this.index, value);
    },
    GetVisible: function () {
        return this.visible && this.clientVisible;
    },
    GetText: function () {
        return this.navBar.GetGroupText(this.index);
    },
    SetText: function (text) {
        this.navBar.SetGroupText(this.index, text);
    },
    SetVisible: function (value) {
        if(this.clientVisible != value) {
            this.clientVisible = value;
            this.navBar.SetGroupVisible(this.index, value, false);
        }
    },
    GetItemCount: function (groupIndex) {
        return this.items.length;
    },
    GetItem: function (index) {
        return (0 <= index && index < this.items.length) ? this.items[index] : null;
    },
    GetItemByName: function (name) {
        for(var i = 0; i < this.items.length; i++)
            if(this.items[i].name == name) return this.items[i];
        return null;
    }
});
var ASPxClientNavBarItem = ASPx.CreateClass(null, {
    constructor: function (navBar, group, index, name) {
        this.navBar = navBar;
        this.group = group;
        this.index = index;
        this.name = name;

        this.enabled = true;
        this.clientEnabled = true;
        this.visible = true;
        this.clientVisible = true;
    },
    GetEnabled: function () {
        return this.enabled && this.clientEnabled;
    },
    SetEnabled: function (value) {
        if(this.clientEnabled != value) {
            this.clientEnabled = value;
            this.navBar.SetItemEnabled(this.group.index, this.index, value, false);
        }
    },
    GetImageUrl: function () {
        return this.navBar.GetItemImageUrl(this.group.index, this.index);
    },
    SetImageUrl: function (value) {
        this.navBar.SetItemImageUrl(this.group.index, this.index, value);
    },
    GetNavigateUrl: function () {
        return this.navBar.GetItemNavigateUrl(this.group.index, this.index);
    },
    SetNavigateUrl: function (value) {
        this.navBar.SetItemNavigateUrl(this.group.index, this.index, value);
    },
    GetText: function () {
        return this.navBar.GetItemText(this.group.index, this.index);
    },
    SetText: function (value) {
        this.navBar.SetItemText(this.group.index, this.index, value);
    },
    GetVisible: function () {
        return this.visible && this.clientVisible;
    },
    SetVisible: function (value) {
        if(this.clientVisible != value) {
            this.clientVisible = value;
            this.navBar.SetItemVisible(this.group.index, this.index, value, false);
        }
    }
});
var ASPxClientNavBarItemEventArgs = ASPx.CreateClass(ASPxClientProcessingModeEventArgs, {
    constructor: function (processOnServer, item, htmlElement, htmlEvent) {
        this.constructor.prototype.constructor.call(this, processOnServer);
        this.item = item;
        this.htmlElement = htmlElement;
        this.htmlEvent = htmlEvent;
    }
});
var ASPxClientNavBarGroupEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function (group) {
        this.group = group;
    }
});
var ASPxClientNavBarGroupCancelEventArgs = ASPx.CreateClass(ASPxClientProcessingModeCancelEventArgs, {
    constructor: function (processOnServer, group) {
        this.constructor.prototype.constructor.call(this, processOnServer);
        this.group = group;
    }
});
var ASPxClientNavBarGroupClickEventArgs = ASPx.CreateClass(ASPxClientNavBarGroupCancelEventArgs, {
    constructor: function (processOnServer, group, htmlElement, htmlEvent) {
        this.constructor.prototype.constructor.call(this, processOnServer, group);
        this.htmlElement = htmlElement;
        this.htmlEvent = htmlEvent;
    }
});

ASPx.NBHClick = function(evt, name, groupIndex) {
    var nb = ASPx.GetControlCollection().Get(name);
    if(nb != null) nb.OnHeaderClick(groupIndex, evt);
    if(!ASPx.Browser.NetscapeFamily)
        evt.cancelBubble = true;
}
ASPx.NBHMMove = function(evt, name, groupIndex) {
    var nb = ASPx.GetControlCollection().Get(name);
    if(nb != null) nb.OnHeaderMouseMove(groupIndex, evt);
}
ASPx.NBIClick = function(evt, name, groupIndex, itemIndex) {
    var nb = ASPx.GetControlCollection().Get(name);
    if(nb != null) nb.OnItemClick(groupIndex, itemIndex, evt);
    if(!ASPx.Browser.NetscapeFamily)
        evt.cancelBubble = true;
}

var MouseOverActiveNavBar = null;
var MouseOverActiveGroupIndex = -1;
var DocMouseMoveHandler = function (evt) {
    if(MouseOverActiveNavBar != null && MouseOverActiveGroupIndex != -1) {
        var srcElement = ASPx.Evt.GetEventSource(evt);
        var headerElement = MouseOverActiveNavBar.GetGroupHeaderExpandedElement(MouseOverActiveGroupIndex);
        if(headerElement == null || (srcElement != headerElement && !ASPx.GetIsParent(headerElement, srcElement))) {
            headerElement = MouseOverActiveNavBar.GetGroupHeaderCollapsedElement(MouseOverActiveGroupIndex);
            if(headerElement == null || (srcElement != headerElement && !ASPx.GetIsParent(headerElement, srcElement))) {
                MouseOverActiveNavBar = null;
                MouseOverActiveGroupIndex = -1;
            }
        }
    }
};
ASPx.Evt.AttachEventToDocument("mousemove", DocMouseMoveHandler);

window.ASPxClientNavBar = ASPxClientNavBar;
window.ASPxClientNavBarGroup = ASPxClientNavBarGroup;
window.ASPxClientNavBarItem = ASPxClientNavBarItem;
window.ASPxClientNavBarItemEventArgs = ASPxClientNavBarItemEventArgs;
window.ASPxClientNavBarGroupEventArgs = ASPxClientNavBarGroupEventArgs;
window.ASPxClientNavBarGroupCancelEventArgs = ASPxClientNavBarGroupCancelEventArgs;
window.ASPxClientNavBarGroupClickEventArgs = ASPxClientNavBarGroupClickEventArgs;
})();