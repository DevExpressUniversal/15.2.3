(function() {
var ASPxClientTabControlWithClientTabAPI = ASPx.CreateClass(ASPxClientTabControl, {

    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);
        this.APILockCount = 0;
        this.handlersList = {
            click: [],
            contextmenu: [],
            dblclick: []
        };
        this.prepareTemplateTab();
    },
    prepareTemplateTab: function() {
        var strip = this.getTabSampleStrip();
        this.unsubscribeElements(strip, ASPxClientSpreadsheetTabControl.CssClasses.PassiveTabCss, ["click"]);
    },
    // SampleTabs
    getTabSampleStripID: function () {
        return "_ETC";
    },
    getTabSampleStrip: function () {
        return this.GetChildElement(this.getTabSampleStripID());
    },
    getActiveSampleTab: function () {
        return ASPx.GetChildByClassName(this.getTabSampleStrip(), ASPxClientSpreadsheetTabControl.CssClasses.ActiveTabCss);
    },
    getPassiveSampleTab: function () {
        return ASPx.GetChildByClassName(this.getTabSampleStrip(), ASPxClientSpreadsheetTabControl.CssClasses.PassiveTabCss);
    },
    getSpaceSampleTab: function () {
        return ASPx.GetChildByClassName(this.getTabSampleStrip(), ASPxClientSpreadsheetTabControl.CssClasses.SpaceCss);
    },

    getTabChildElement: function (tabElement, insideClassName) {
        if (!tabElement)
            return null;
        return ASPx.GetNodeByClassName(tabElement, insideClassName);
    },
    getBeforeNodeInScrollingMode: function (index) {
        if (index >= this.GetTabCount()) {
            var strip = this.GetTabsCell();
            if (strip) {
                var spacers = ASPx.GetChildNodesByClassName(strip, "dxtc-spacer");
                if (spacers && spacers.length > 0)
                    return spacers[spacers.length - 1];
                else
                    if (strip.childNodes.length > 0)
                        return strip.childNodes[0];
            }
        }
        return this.GetTabElement(index, false);
    },
    getBeforeNodeInNoneScrollingMode: function (index) {
        if (index >= this.GetTabCount())
            return this.GetRightIndentLite();
        else
            return this.GetTabElement(index, false);
    },
    /*Client Tab API*/
    ClearTabs: function () {
        var strip = this.GetTabsCell();
        if (ASPx.IsExists(strip)) {
            this.BeginUpdate();

            for (var index = this.GetTabCount() - 1; index >= 0; index--)
                this.RemoveTab(index);
            this.ClearClientCollection();
            this.EndUpdate();
        }
    },
    RemoveTab: function (index) {
        var strip = this.GetTabsCell();
        if (ASPx.IsExists(strip)) {
            this.RemoveActiveTabAtPosition(strip, index);
            this.RemovePassiveTabAtPosition(strip, index);
            this.RemoveSpaceTabAtPosition(strip, index);

            this.UpdateTabStripIds();

            this.RemoveItemFromClientCollections(index);
            this.CorrectActiveTabIndex();

            this.UpdateTabStripSize();
        }
    },
    AddTab: function (text, isActive, toolTip, navigateUrl) {
        this.InsertTab(this.GetTabCount(), text, isActive, toolTip, navigateUrl)
    },
    InsertTab: function (index, text, isActive, toolTip, navigateUrl) {
        var strip = this.GetTabsCell();
        if (ASPx.IsExists(strip)) {
            isActive = ASPx.IsExists(isActive) ? isActive : false;
            var isLast = index >= this.GetTabCount();
            this.InsertSpace(strip, index, !isLast || (this.GetTabCount() == 0));
            this.InsertPassiveTab(strip, index, text, isActive, toolTip, navigateUrl);
            this.InsertActiveTab(strip, index, text, isActive, toolTip, navigateUrl);
            this.InsertSpace(strip, index, isLast);

            this.InsertItemToClientCollections(index, isActive);

            this.UpdateTabStripIds();

            this.AddTabStyle(this.GetTabElement(index, true), this.getActiveSampleTab());
            this.AddTabStyle(this.GetTabElement(index, false), this.getPassiveSampleTab());

            this.UpdateTabStripSize();
        }
    },

    APILock: function () {
        this.APILockCount++;
    },
    APIUnlock: function () {
        this.APILockCount--;
    },
    IsAPILocked: function () {
        return this.APILockCount > 0;
    },
    BeginUpdate: function () {
        this.APILock();
    },
    EndUpdate: function () {
        this.APIUnlock();
        this.UpdateTabStripSize();
    },
    UpdateTabStripSize: function () {
        if (!this.IsAPILocked()) {
            this.ResizeTabControl();
        }
    },
    // Prepare Sample Elements
    PrepareTabStrip: function () {
        ASPxClientTabControl.prototype.PrepareTabStrip.call(this);
        this.PrepareSampleTabStrip();
    },
    PrepareSampleTabStrip: function () {
        var sampleStrip = this.getTabSampleStrip();
        if (sampleStrip) {
            this.PrepareElements(sampleStrip, ASPxClientSpreadsheetTabControl.CssClasses.PassiveTabCss,
                function (index) {
                    var tabIndex = -1;
                    return this.name + this.GetTabElementID(tabIndex, false);
                }.aspxBind(this));
            this.PrepareElements(sampleStrip, ASPxClientSpreadsheetTabControl.CssClasses.ActiveTabCss,
                function (index) {
                    var tabIndex = -1;
                    return this.name + this.GetTabElementID(tabIndex, true);
                }.aspxBind(this));
            this.PrepareElements(sampleStrip, ASPxClientSpreadsheetTabControl.CssClasses.SpaceCss,
                function (index) {
                    var separatorIndex = -1;
                    return this.name + this.GetSeparatorElementID(separatorIndex);
                }.aspxBind(this));
        }
    },
    // Remove
    RemoveActiveTabAtPosition: function (parentStrip, index) {
        var tabElement = this.GetTabElement(index, true);

        if(tabElement)
            parentStrip.removeChild(tabElement);
    },
    RemovePassiveTabAtPosition: function (parentStrip, index) {
        var tabElement = this.GetTabElement(index, false);
        if(tabElement)
            parentStrip.removeChild(tabElement);
    },
    RemoveSpaceTabAtPosition: function (parentStrip, index) {
        var tabElement = this.GetSeparatorElement(index);
        if(tabElement)
            parentStrip.removeChild(tabElement);
    },
    RemoveItemFromClientCollections: function (index) {
        ASPx.Data.ArrayRemoveAt(this.tabs, index);
        this.UpdateTabsIndex(index);
    },
    ClearClientCollection: function () {
        this.activeTabIndex = -1;
        this.tabs = [];
    },
    CorrectActiveTabIndex: function () {
        if (!this.IsAPILocked()) {
            if (this.GetTabCount() > 0) {
                if (this.activeTabIndex >= this.GetTabCount()) {
                    this.activeTabIndex = this.GetTabCount() - 1;
                }
            } else this.activeTabIndex = -1;
        }
    },
    //Insert
    InsertPassiveTab: function (strip, index, text, isActive, toolTip, navigateUrl) {
        var beforeNode = this.FindBeforeNode(index);
        var sampleTab = this.getPassiveSampleTab();
        if (sampleTab) {
            var cloneTab = sampleTab.cloneNode(true);
            strip.insertBefore(cloneTab, beforeNode);

            this.SetTabText(cloneTab, text);
            this.SetTabToolTip(cloneTab, toolTip);
            this.SetTabNavigateUrl(cloneTab, navigateUrl);

            if (isActive)
                ASPx.Attr.SetAttribute(cloneTab.style, "display", 'none');

            if (index === 0)
                ASPx.AddClassNameToElement(cloneTab, ASPxClientSpreadsheetTabControl.CssClasses.LeadTabCss);
        }
    },
    InsertActiveTab: function (strip, index, text, isActive, toolTip, navigateUrl) {
        var beforeNode = this.FindBeforeNode(index);
        var sampleTab = this.getActiveSampleTab();
        if (sampleTab) {
            var cloneTab = sampleTab.cloneNode(true);
            strip.insertBefore(cloneTab, beforeNode);

            this.SetTabText(cloneTab, text);
            this.SetTabToolTip(cloneTab, toolTip);
            this.SetTabNavigateUrl(cloneTab, navigateUrl);

            if (!isActive)
                ASPx.Attr.SetAttribute(cloneTab.style, "display", 'none');

            if (index === 0)
                ASPx.AddClassNameToElement(cloneTab, ASPxClientSpreadsheetTabControl.CssClasses.LeadTabCss);
        }
    },
    InsertSpace: function (strip, index, isLast) {
        if (isLast) return;

        var beforeNode = this.FindBeforeNode(index);
        var sampleTab = this.getSpaceSampleTab();
        if (sampleTab) {
            var cloneTab = sampleTab.cloneNode(true);
            strip.insertBefore(cloneTab, beforeNode);
        }
    },
    FindBeforeNode: function (index) {
        if (this.enableScrolling)
            return this.getBeforeNodeInScrollingMode(index);
        else
            return this.getBeforeNodeInNoneScrollingMode(index);
    },
    SetTabText: function (tabElement, text) {
        var container = this.getTabChildElement(tabElement, ASPxClientSpreadsheetTabControl.CssClasses.TabLinkCss);
        if (container != null) {
            var textNode = ASPx.GetTextNode(container);
            if (textNode != null)
                textNode.nodeValue = text;
        }
    },
    SetTabToolTip: function (tabElement, toolTip) {
        if (ASPx.IsExists(toolTip) && (toolTip != "")) {
            ASPx.Attr.SetAttribute(tabElement, "title", toolTip);
            var linkElement = this.getTabChildElement(tabElement, ASPxClientSpreadsheetTabControl.CssClasses.TabLinkCss);
            if (linkElement)
                ASPx.Attr.SetAttribute(linkElement, "title", toolTip);
        }
    },
    SetTabNavigateUrl: function (tabElement, navigateUrl) {
        if (ASPx.IsExists(navigateUrl) && (navigateUrl != "")) {
            var linkElement = this.getTabChildElement(tabElement, ASPxClientSpreadsheetTabControl.CssClasses.TabLinkCss);
            if (linkElement)
                ASPx.Attr.SetAttribute(linkElement, "href", navigateUrl);
        }
    },
    InsertItemToClientCollections: function (index, isActive) {
        var tab = this.CreateClientTab(index, '', null, null, null, null);
        ASPx.Data.ArrayInsert(this.tabs, tab, index);
        if (isActive)
            this.activeTabIndex = index;
        this.UpdateTabsIndex(index);
    },
    CreateClientTab: function (index, name, enable, clientEnable, visible, clientVisible) {
        var property = this.CreateProperty(name, enable, clientEnable, visible, clientVisible);
        var tabName = property[0] || "";
        var tab = new ASPxClientTab(this, index, tabName);
        this.CreateTabProperties(tab, property);
        return tab;
    },
    CreateProperty: function (name, enable, clientEnable, visible, clientVisible) {
        var property = [];
        property.push(name);//name
        property.push(enable);//tab.Enabled
        property.push(clientEnable);//tab.ClientEnabled
        property.push(visible);//tab.Visible
        property.push(clientVisible);//tab.ClientVisible
        return property;
    },
    // Update Child Elements
    UpdateTabStripIds: function () {
        var strip = this.GetTabsCell();
        if (strip) {
            var indexCorrection = (ASPx.GetChildNodesByClassName(strip, ASPxClientSpreadsheetTabControl.CssClasses.FileTabCss).length > 0) ? 1 : 0;

            this.UpdateElementsId(strip, ASPxClientSpreadsheetTabControl.CssClasses.PassiveTabCss,
                function (index) {
                    return this.name + this.GetTabElementID(index, false);
                }.aspxBind(this));
            this.unsubscribeElements(strip, ASPxClientSpreadsheetTabControl.CssClasses.PassiveTabCss, ["click", "dblclick", "contextmenu"]);
            this.subscribeElements(strip, ASPxClientSpreadsheetTabControl.CssClasses.PassiveTabCss, ["click", "dblclick", "contextmenu"], indexCorrection);

            this.UpdateElementsId(strip, ASPxClientSpreadsheetTabControl.CssClasses.ActiveTabCss,
                function (index) {
                    return this.name + this.GetTabElementID(index + indexCorrection, true);
                }.aspxBind(this));
            this.unsubscribeElements(strip, ASPxClientSpreadsheetTabControl.CssClasses.ActiveTabCss, ["dblclick", "contextmenu"]);
            this.subscribeElements(strip, ASPxClientSpreadsheetTabControl.CssClasses.ActiveTabCss, ["dblclick", "contextmenu"], indexCorrection);

            this.UpdateElementsId(strip, ASPxClientSpreadsheetTabControl.CssClasses.SpaceCss,
                function (index) {
                    if (this.enableScrolling && (index == this.tabs.length - 1)) return;
                    return this.name + this.GetSeparatorElementID(index);
                }.aspxBind(this));

            this.UpdateElementsId(strip, ASPxClientSpreadsheetTabControl.CssClasses.TabLinkCss, function (index, el) { return el.parentNode.id + "T"; }.aspxBind(this));
            this.UpdateElementsId(strip, ASPxClientSpreadsheetTabControl.CssClasses.TabImageCss, function (index, el) { return el.parentNode.parentNode.id + "Img"; }.aspxBind(this));
        }
    },
    UpdateElementsId: function (container, className, getId) {
        var elements = this.getElementsByClassName(container, className);
        for (var i = 0; i < elements.length; i++) {
            var id = getId(i, elements[i]);
            if (id) {
                elements[i].id = id;
            }
        }
    },
    getElementsByClassName: function(container, className) {
        return (className !== "") ? ASPx.GetNodesByClassName(container, className) : ASPx.GetChildNodes(container, function (el) { return !!el.tagName; });
    },
    unsubscribeElements: function(container, className, events) {
        this.changeElementsSubscription(container, className, events, 0, false);
    },
    subscribeElements: function(container, className, events, indexCorrection) {
        this.changeElementsSubscription(container, className, events, indexCorrection, true);
    },
    changeElementsSubscription: function(container, className, events, indexCorrection, subscribe) {
        var changeSubscriptionFn = subscribe ? this.subscribeElement : this.unsubscribeElement,
            elements = this.getElementsByClassName(container, className);

        for(var i = 0; i < elements.length; i++) {
            for(var j = 0; j < events.length; j++)
                changeSubscriptionFn.apply(this, [elements[i], events[j], i + indexCorrection]);
        }
    },
    unsubscribeElement: function(element, eventName) {
        var existingHandler = this.handlersList[eventName][element.id];
        if(existingHandler) {
            ASPx.Evt.DetachEventFromElement(element, eventName, existingHandler);
            delete this.handlersList[eventName][element.id];
        }
        if(eventName === "click")
            this.removeInlineOnClickHandler(element);
    },
    subscribeElement: function(element, eventName, index) {
        var handler = this.getHandlers(this.name, index)[eventName];

        ASPx.Evt.AttachEventToElement(element, eventName, handler);
        this.handlersList[eventName][element.id] = handler;
    },
    removeInlineOnClickHandler: function(element) {
        if(ASPx.Browser.Edge || (ASPx.Browser.IE && ASPx.Browser.MajorVersion > 10))
            element.onclick = null;
        else
            ASPx.Attr.RemoveAttribute(element, "onclick");
    },
    getHandlers: function(name, index) {
        return {
            click: function(e) {
                ASPx.TCTClick(e, name, index);
            },
            dblclick: function(e) {
                ASPx.TCTClick(e, name, index);
            },
            contextmenu: function(e) {
                ASPx.Evt.PreventEvent(e);
                ASPx.TCTClick(e, name, index);
            }
        };
    },
    UpdateTabsIndex: function (updatePosition) {
        for (var index = updatePosition; index < this.tabs.length; index++)
            this.tabs[index].index = index;
    },
    // Resizing
    ResizeTabControl: function () {
        if (this.enableScrolling) {
            this.CalculateSizes();
            this.PrepareParams();
            this.RecalculateTabStripWidthLite();
            this.AdjustTabScrollingCore(true, false);
        } else
            this.AdjustTabControlSizeLite();
    },
    PrepareParams: function() {
        if(!this.adjustmentVars.indentsSizes.left)
            this.adjustmentVars.indentsSizes.left = 0;
        if(!this.adjustmentVars.indentsSizes.right)
            this.adjustmentVars.indentsSizes.right = 0;
    },
    // Styles
    AddTabStyle: function (tabElement, copyFromElement) {
        var styleController = ASPx.GetStateController();
        if (styleController) {
            ASPx.AddHoverItems(this.name,
                           this.CreateStyleClasses(tabElement.id.replace(this.name + '_', ''),
                               styleController.GetHoverElement(copyFromElement), ASPx.HoverItemKind), true);

            ASPx.AddDisabledItems(this.name,
                            this.CreateStyleClasses(tabElement.id.replace(this.name + '_', ''),
                                styleController.GetDisabledElement(copyFromElement), ASPx.DisabledItemKind), true);
        }
    },
    CreateStyleClasses: function (id, item, kind) {
        var classes = [];
        if (item && item[kind]) {
            classes[0] = [];
            classes[0][0] = item[kind].classNames;
            classes[0][1] = item[kind].cssTexts;
            classes[0][2] = [];
            classes[0][2][0] = id;
        }
        return classes;
    },
    NeedCollapseControlCore: function () {
        return false;
    }
});
var ASPxClientSpreadsheetTabControl = ASPx.CreateClass(ASPxClientTabControlWithClientTabAPI, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);
        this.visibleSheets = [];
        this.hiddenSheets = [];
    },
    processDocumentResponse: function (response) {
        var responseTabInfo = response.tabControlInfo;
        if(responseTabInfo) {
            var activeTabIndex = response.sheetIndex;
            var visibleSheets = eval(responseTabInfo.visibleSheets);
            var hiddenSheets = eval(responseTabInfo.hiddenSheets);

            this.setVisibleSheets(visibleSheets);
            this.setHiddenSheets(hiddenSheets);
            this.UpdateTabsOnClient(activeTabIndex, visibleSheets);
        }
    },
    UpdateTabsOnClient: function (newActiveTabIndex, tabNames) {
        var strip = this.GetTabsCell();
        if ((tabNames.length > 0) && ASPx.IsExists(strip)) {
            this.BeginUpdate();
            this.ClearTabs();
            for (var index = 0; index < tabNames.length; index++) {
                this.AddTab(tabNames[index].Name, tabNames[index].Id == newActiveTabIndex, tabNames[index].Name);
            }
            this.EndUpdate();
        }
    },
    setVisibleSheets: function (tabsInfo) {
        this.visibleSheets = tabsInfo
    },
    setHiddenSheets: function (hiddenSheets) {
        this.hiddenSheets = hiddenSheets;
    },
    getVisibleSheets: function () {
        return this.visibleSheets;
    },
    getHiddenSheets: function () {
        return this.hiddenSheets;
    },
    getModelSheetIndex: function (index) {
        var visibleSheets = this.getVisibleSheets();
        if (visibleSheets) {
            var tab = visibleSheets[index];
            if (tab) {
                return tab.Id;
            }
        }
        return index;
    }
});

ASPxClientSpreadsheetTabControl.CssClasses = {
    ActiveTabCss: "dxtc-activeTab",
    PassiveTabCss: "dxtc-tab",
    LeadTabCss: "dxtc-lead",
    SpaceCss: "dxtc-spacer",
    FileTabCss: "dxr-fileTab",
    LineBreakCss: "dxtc-lineBreak",

    TabLinkCss: "dxtc-link",
    TabImageCss: "dxtc-img"
};

window.ASPxClientTabControlWithClientTabAPI = ASPxClientTabControlWithClientTabAPI;
window.ASPxClientSpreadsheetTabControl = ASPxClientSpreadsheetTabControl;
})();