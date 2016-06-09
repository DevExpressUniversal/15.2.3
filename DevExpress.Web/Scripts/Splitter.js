/// <reference path="_references.js"/>

(function () {
var SplitterHelper = ASPx.CreateClass(null, {
    // Constructor
    constructor: function (splitter) {
        this.splitter = splitter;
    },

    // Resizing
    GetMoveMaxDeltaSize: function (deltaSize) {
        if(deltaSize == 0)
            return 0;
        var splitter = this.splitter,
            leftPane = splitter.moveLeftPane,
            rightPane = splitter.moveRightPane;

        if(splitter.isHeavyUpdate) {
            var parent = leftPane.parent;
            var totalSize = 0, minSize = 0;
            for(var i = 0; i < parent.panes.length; i++) {
                var pane = parent.panes[i];
                if(pane.isSizePx)
                    continue;
                if(pane.collapsed) {
                    var collapsedSize = pane.GetSizeDiff(pane.isVertical);
                    totalSize += collapsedSize;
                    minSize += collapsedSize;
                }
                else {
                    totalSize += pane.GetOffsetSize();
                    minSize += pane.GetMinSize();
                }
            }

            var rightPanePx = rightPane.isSizePx;
            if(rightPanePx)
                deltaSize = this.GetPaneMaxDeltaSize(rightPane, -deltaSize);
            deltaSize = this.GetMaxDeltaSize(totalSize, minSize, Number.MAX_VALUE, -deltaSize);
            if(!rightPanePx)
                deltaSize = this.GetPaneMaxDeltaSize(leftPane, -deltaSize);
        }
        else {
            var parent = leftPane.parent,
                rightPaneAutoSize = rightPane.IsAutoSize(parent.isVertical),
                leftPaneAutoSize = leftPane.IsAutoSize(parent.isVertical);
            if(!rightPaneAutoSize)
                deltaSize = -this.GetPaneMaxDeltaSize(rightPane, -1 * deltaSize);
            if(!leftPaneAutoSize)
                deltaSize = this.GetPaneMaxDeltaSize(leftPane, deltaSize);
        }
        return deltaSize;
    },
    GetPaneMaxDeltaSize: function (pane, deltaSize) {
        return this.GetMaxDeltaSize(pane.GetOffsetSize(), pane.GetMinSize(), pane.maxSize, deltaSize);
    },
    GetMaxDeltaSize: function (size, min, max, deltaSize) {
        var minDeltaSize = Math.floor(min - size);
        var maxDeltaSize = Math.floor(max - size);
        if(deltaSize < minDeltaSize)
            return (size < min) ? 0 : minDeltaSize;
        else if(deltaSize > maxDeltaSize)
            return (size > max) ? 0 : maxDeltaSize;
        return deltaSize;
    },
    GetCurrentPos: function () {
        return this.splitter.moveIsVertical
            ? ASPxClientSplitter.CurrentYPos
            : ASPxClientSplitter.CurrentXPos;
    },

    // Resizing panel
    SetResizingPanelVisibility: function (visible, cursor) {
        var resizingPanel = ASPx.CacheHelper.GetCachedValue(this, "resizingPanel", function () {
            var resizingPanel = document.createElement("DIV");
            resizingPanel.style.overflow = "hidden";
            resizingPanel.style.position = "absolute";

            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 10) {
                resizingPanel.style.backgroundColor = "White";
                resizingPanel.style.filter = "alpha(opacity=1)";
            }

            resizingPanel.isVisible = false;

            return resizingPanel;
        });

        if(resizingPanel.isVisible != visible) {
            if(visible) {
                var mainElement = this.splitter.GetMainElement();
                ASPx.SetStyles(resizingPanel, {
                    width: mainElement.offsetWidth,
                    height: mainElement.offsetHeight
                });
                if(cursor)
                    resizingPanel.style.cursor = cursor;

                mainElement.parentNode.appendChild(resizingPanel);
                ASPx.SetAbsoluteX(resizingPanel, ASPx.GetAbsoluteX(mainElement));
                ASPx.SetAbsoluteY(resizingPanel, ASPx.GetAbsoluteY(mainElement));
            }
            else
                resizingPanel.parentNode.removeChild(resizingPanel);

            resizingPanel.isVisible = visible;
        }
    }
});
SplitterHelper.Resize = function (pane1, pane2, deltaSize) {
    if(pane1.isSizePx || pane2.isSizePx) {
        var parent = pane1.parent;
        if(pane1.isSizePx && !pane1.IsAutoSize(parent.isVertical))
            pane1.size += deltaSize;
        if(pane2.isSizePx && !pane2.IsAutoSize(parent.isVertical))
            pane2.size -= deltaSize;
    }
    else {
        var c = (pane1.size + pane2.size) / (pane1.GetOffsetSize() + pane2.GetOffsetSize());
        pane1.size = c * (pane1.GetOffsetSize() + deltaSize);
        pane2.size = c * (pane2.GetOffsetSize() - deltaSize);
    }
};
SplitterHelper.IsAllowResize = function (pane1, pane2) {
    if(!pane1 || !pane2)
        return false;
    if(!pane1.splitter.enabled)
        return false;

    var bothAutoSizeOrPercent = pane1.isVertical
        ? pane1.autoHeight && pane2.autoHeight || pane1.autoHeight && !pane2.isSizePx || !pane1.isSizePx && pane2.autoHeight
        : pane1.autoWidth && pane2.autoWidth || pane1.autoWidth && !pane2.isSizePx || !pane1.isSizePx && pane2.autoWidth;
    if(bothAutoSizeOrPercent)
        return false;

    return pane1.splitter.allowResize && pane1.allowResize && pane2.allowResize;
};

var ASPxSplitterPaneHelper = ASPx.CreateClass(null, {
    // Constructor
    constructor: function (pane) {
        this.pane = pane;

        this.indexPath = this.GetIndexPath();
        var paneIdPostfix = this.pane.isRootPane ? "" : "_" + this.indexPath;
        var separatorIdPostfix = paneIdPostfix + "_S";

        this.postfixes = {
            pane: paneIdPostfix,
            separator: separatorIdPostfix,
            table: paneIdPostfix + "_T",
            contentContainer: paneIdPostfix + "_CC",
            collapseForwardButton: separatorIdPostfix + "_CF",
            collapseBackwardButton: separatorIdPostfix + "_CB",
            collapseButtonsSeparator: separatorIdPostfix + "_CS"
        };

        this.buttonsTableExists = !!this.GetCollapseBackwardButton();
        this.separatorImageExists = !!this.GetCollapseButtonsSeparatorImage();
        this.buttonsExists = this.buttonsTableExists || this.separatorImageExists;
    },

    // Caching
    GetCachedValue: function (name, func) {
        return ASPx.CacheHelper.GetCachedValue(this, name, func);
    },
    DropCachedValue: function (name) {
        ASPx.CacheHelper.DropCachedValue(this, name);
    },

    // Element IDs
    GetIndexPath: function () {
        if(this.pane.isRootPane)
            return "";
        var parentPane = this.pane.parent;
        if(parentPane.isRootPane)
            return "" + this.pane.index;
        return parentPane.helper.indexPath + ASPx.ItemIndexSeparator + this.pane.index;
    },

    // Elements
    GetChildElement: function (idPostfix) {
        return this.pane.splitter.GetChildElement(idPostfix);
    },
    GetPaneElement: function () {
        return this.GetChildElement(this.postfixes.pane);
    },
    GetTableElement: function () {
        return this.GetChildElement(this.postfixes.table);
    },
    GetContentContainerElement: function () {
        return this.GetCachedValue(this.postfixes.contentContainer, 
            function () {
                return this.pane.splitter.GetChildElement(this.postfixes.contentContainer);
            });
    },
    DropContentContainerElementFromCache: function () {
        this.DropCachedValue(this.postfixes.contentContainer);
    },
    GetSeparatorElementId: function () {
        return this.pane.splitter.name + this.postfixes.separator;
    },
    GetSeparatorElement: function () {
        return this.GetChildElement(this.postfixes.separator);
    },
    GetSeparatorDivElement: function () {
        return this.GetCachedValue("separatorDivElement", function () {
            var separatorElement = this.GetSeparatorElement();
            return separatorElement ? separatorElement.childNodes[0] : null;
        });
    },
    GetCollapseBackwardButton: function () {
        return this.GetChildElement(this.postfixes.collapseBackwardButton);
    },
    GetCollapseForwardButton: function () {
        return this.GetChildElement(this.postfixes.collapseForwardButton);
    },
    GetCollapseButtonsSeparator: function () {
        return this.GetChildElement(this.postfixes.collapseButtonsSeparator);
    },
    GetCollapseButtonsTable: function () {
        return this.GetCachedValue("collapseButtonsTable", function () {
            return this.buttonsTableExists ? ASPx.GetParentByTagName(this.GetCollapseForwardButton(), "TABLE") : null;
        });
    },
    GetCollapseButtonsSeparatorImage: function () {
        return this.GetCachedValue("collapseButtonsSeparatorImage", function () {
            var separator = this.GetCollapseButtonsSeparator();
            if(!separator) {
                if(!this.buttonsTableExists)
                    separator = this.GetSeparatorElement();
                else
                    return null;
            }
            return ASPx.GetNodeByTagName(separator, "IMG", 0);
        });
    },
    GetButtonUpdateElement: function (buttonElement) {
        return !this.pane.isVertical ? buttonElement.parentNode : buttonElement;
    },

    ClearElementSizeProperty: function (property) {
        var element = this.GetPaneElement(),
            isVertical = property === "width";
        this.pane.savedSizeProperty = element.style[property];
        element.style[property] = "";
        if(!this.pane.IsAutoSize(isVertical)) {
            var contentContainerElement = this.GetContentContainerElement();
            this.pane.savedContentSizeProperty = contentContainerElement.style[property];
            contentContainerElement.style[property] = (this.pane.GetMinSize() - (isVertical ? this.pane.contentContainerWidthDiff : this.pane.contentContainerHeightDiff)) + "px";
        }
    },
    RestoreElementSizeProperty: function (property) {
        if(this.pane.savedSizeProperty) {
            this.GetPaneElement().style[property] = this.pane.savedSizeProperty;
            this.pane.savedSizeProperty = null;
        }
        if(!this.pane.IsAutoSize(property === "width")) {
            this.GetContentContainerElement().style[property] = this.pane.savedContentSizeProperty;
            this.pane.savedContentSizeProperty = null;
        }
    },

    // Common
    SetEmptyDivVisible: function (visible) {
        var emptyDiv = this.GetCachedValue("emptyDiv", function () {
            var emptyDiv = document.createElement("DIV");
            emptyDiv.style.cssText = "overflow: hidden; width: 0px; height: 0px";
            emptyDiv.isVisible = false;
            return emptyDiv;
        });
        if(visible != emptyDiv.isVisible) {
            if(visible)
                this.GetPaneElement().appendChild(emptyDiv);
            else
                this.GetPaneElement().removeChild(emptyDiv);
            emptyDiv.isVisible = visible;
        }
    },

    HasCollapsedParent: function () {
        var parent = this.pane.parent;
        if(parent)
            return parent.collapsed || parent.helper.HasCollapsedParent();
        return false;
    },

    HasVisibleAutoSizeChildren: function (isVertical) {
        var result = false;
        if(!ASPx.IsExists(isVertical))
            isVertical = this.pane.isVertical;
        for(var i = 0; i < this.pane.panes.length; i++) {
            var pane = this.pane.panes[i];
            result = result || !pane.collapsed && pane.IsAutoSize(isVertical) && (!pane.panes.length || pane.helper.HasVisibleAutoSizeChildren(isVertical));
        }
        return result;
    }
});

var ASPxSplitterResizingPointer = ASPx.CreateClass(null, {
    constructor: function (elementId) {
        this.elementId = elementId;
        this.element = ASPx.GetElementById(this.elementId);
        this.x = 0;
        this.y = 0;
    },
    SetCursor: function (cursor) {
        this.element.style.cursor = cursor;
    },
    SetPosition: function (x, y) {
        this.x = x;
        this.y = y;
        ASPx.SetAbsoluteY(this.element, this.y);
        ASPx.SetAbsoluteX(this.element, this.x);
    },
    SetVisibility: function (isVisible) {
        ASPx.SetElementDisplay(this.element, isVisible);
    },
    Move: function (delta, isX) {
        if(isX)
            this.x += delta;
        else
            this.y += delta;
        this.SetPosition(this.x, this.y);
    },
    AttachToElement: function (element, isShow) {
        ASPx.SetStyles(this.element, {
            width: element.offsetWidth, height: element.offsetHeight
        });
        this.SetVisibility(true);
        this.SetPosition(ASPx.GetAbsoluteX(element), ASPx.GetAbsoluteY(element));
    }
});
var ASPxClientSplitter = ASPx.CreateClass(ASPxClientControl, {
    // Constructor
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);

        this.leadingAfterInitCall = ASPxClientControl.LeadingAfterInitCallConsts.Reverse;

        this.emptyUrls = [
            "javascript:false",
            "about:blank",
            "#"
        ];

        this.width = "100%";
        this.height = "200px";

        this.helper = new SplitterHelper(this);
        this.resizingPointer = new ASPxSplitterResizingPointer(this.name + "_RP");
        this.rootPane = new ASPxClientSplitterPane(this, null, 0, 0, {});

        this.liveResizing = false;
        this.allowResize = true;
        this.defaultMinSize = 5;
        //
        this.showSeparatorImage = true;
        this.showCollapseBackwardButton = false;
        this.showCollapseForwardButton = false;
        //
        this.fullScreen = false;

        this.prepared = false;
        this.PaneResizing = new ASPxClientEvent();
        this.PaneResized = new ASPxClientEvent();
        this.PaneCollapsing = new ASPxClientEvent();
        this.PaneCollapsed = new ASPxClientEvent();
        this.PaneExpanding = new ASPxClientEvent();
        this.PaneExpanded = new ASPxClientEvent();
        this.PaneResizeCompleted = new ASPxClientEvent();
        this.PaneContentUrlLoaded = new ASPxClientEvent();

        this.autoHeightPanes = [];
        this.autoWidthPanes = [];
    },

    // Panes creation
    CreatePanes: function (panesInfo) {
        this.CreatePanesInternal(this.rootPane, panesInfo);
        this.rootPane.ForEach("UpdateSize");
        this.rootPane.ForEach("UpdateAutoSize");
        this.state = this.GetStateObj(panesInfo);
    },
    CreatePanesInternal: function (parent, panesInfo) {
        var prevPane = null,
            visibleIndex = 0;
        for(var i = 0; i < panesInfo.length; i++) {
            var paneInfo = panesInfo[i];
            if(!paneInfo.v) continue; // server-side Visible == false

            var pane = new ASPxClientSplitterPane(this, parent, visibleIndex++, i, paneInfo);
            updatePrevNext(pane);
            updateAutoSize(parent, pane.autoWidth, pane.autoHeight);
            parent.panes.push(pane);

            if(ASPx.IsExists(paneInfo["i"]))
                this.CreatePanesInternal(pane, paneInfo["i"]);
        }

        function updatePrevNext(pane) {
            pane.prevPane = prevPane;
            if(prevPane != null)
                prevPane.nextPane = pane;
            prevPane = pane;
        }
        function updateAutoSize(pane, autoWidth, autoHeight) {
            if(pane && (autoWidth || autoHeight)) {
                if(autoWidth)
                    pane.autoWidth = true;
                if(autoHeight)
                    pane.autoHeight = true;
                if(!pane.splitter.hasAutoSizePane)
                    pane.splitter.hasAutoSizePane = true;
                updateAutoSize(pane.parent, autoWidth, autoHeight);
            }
        }
    },
    GetStateObj: function (panesInfo) {
        var result = [];
        for(var i = 0; i < panesInfo.length; i++) {
            var paneState = {};
            if(panesInfo[i].st) {
                paneState.st = panesInfo[i].st;
                paneState.s = panesInfo[i].s;
            }
            if(panesInfo[i].c)
                paneState.c = panesInfo[i].c;
            if(panesInfo[i]["i"])
                paneState["i"] = this.GetStateObj(panesInfo[i]["i"]);
            result.push(paneState);
        }
        return result;
    },
    GetClientStateString: function () {
        return ASPx.Json.ToJson(this.GetClientStateObject());
    },
    GetClientStateObject: function () {
        return this.RefreshState(this.state, this.rootPane.panes);
    },
    RefreshState: function (state, panes) {
        for(var i = 0; i < panes.length; i++) {
            var pane = panes[i];
            var paneState = state[pane._index];

            paneState.s = Math.round(pane.size * 1000) / 1000;
            paneState.st = pane.sizeType;
            paneState.c = pane.collapsed ? 1 : 0;

            if(pane.panes.length == 0) {
                paneState.spt = Math.round(pane.scrollTop);
                paneState.spl = Math.round(pane.scrollLeft);
            }

            if(pane.panes.length > 0)
                this.RefreshState(paneState["i"], pane.panes);
        }
        return state;
    },

    // Initialization
    InlineInitialize: function () {
        this.EnsureFullscreenMode();
        this.rootPane.ForEach("Initialize");

        this.constructor.prototype.InlineInitialize.call(this);
    },
    EnsureFullscreenMode: function () {
        if(this.fullScreen) {
            var overflowProperty = "overflow",
                oldIEOverflowAutoProperty = null,
                autoWidth = this.rootPane.autoWidth,
                autoHeight = this.rootPane.autoHeight;

            if(autoWidth && autoHeight) {
                overflowProperty = null;
                oldIEOverflowAutoProperty = "overflow";
            }
            else if(autoWidth) {
                overflowProperty = "overflowY";
                oldIEOverflowAutoProperty = "overflowX";
            }
            else if(autoHeight) {
                overflowProperty = "overflowX";
                oldIEOverflowAutoProperty = "overflowY";
            }

            var element = this.GetMainElement().parentNode;
            while(element && element.tagName) {
                var tagName = element.tagName.toLowerCase();

                // T291096
                if(tagName != "tbody") {
                    element.style.height = "100%";

                    if(tagName == "form" || tagName == "body" || tagName == "html") {
                        element.style.margin = "0px";
                        element.style.padding = "0px";

                        if(overflowProperty)
                            element.style[overflowProperty] = "hidden";
                        if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 9 && tagName == "form" && oldIEOverflowAutoProperty)
                            element.style[oldIEOverflowAutoProperty] = "auto";
                        if((autoHeight != autoWidth || (ASPx.Browser.IE && ASPx.Browser.MajorVersion < 9)) && (tagName == "body" || tagName == "html"))
                            element.style.overflow = "hidden";
                    }

                    if(tagName == "html")
                        break;
                }
                element = element.parentNode;
            }
        }
    },
    Initialize: function () {
        this.constructor.prototype.Initialize.call(this);
        this.rootPane.ForEach("CreateContentUrlIFrame", true);
    },
    AfterInitialize: function () {
        this.constructor.prototype.AfterInitialize.call(this);
        this.rootPane.ForEach("RaiseResizedEvent", true);
    },
    IsPrepared: function () {
        return this.prepared;
    },
    Prepare: function () {
        if(this.IsPrepared() || !this.IsDisplayed())
            return;
        this.rootPane.ForEach("Prepare", true);
        ASPxClientSplitter.Instances.Add(this);
        this.prepared = true;
    },

    UpdateAdjustmentFlags: function () {
        var mainElement = this.GetMainElement();
        if(mainElement) {
            var mainElementStyle = ASPx.GetCurrentStyle(mainElement);
            this.UpdatePercentSizeConfig([mainElementStyle.width, this.width], [mainElementStyle.height, this.height]);
        }
    },
    AdjustControlCore: function () {
        this.Prepare();
        this.UpdateControlSizes(false, this.isInsideHierarchyAdjustment);
    },
    NeedCollapseControlCore: function () {
        return true;
    },
    NeedUpdateControlSizes: function () {
        return ASPx.IsPercentageSize(this.width) || ASPx.IsPercentageSize(this.height) || !this.sizeUpdatedOnce;
    },
    UpdateControlSizes: function (forceUpdate, disableControlsAdjustment) {
        if(!(forceUpdate || this.NeedUpdateControlSizes()) || !this.IsDisplayed())
            return;

        var element = this.GetMainElement(),
            autoHeightSpacer;
        if(this.rootPane.autoHeight && !ASPx.Browser.Chrome) {
            autoHeightSpacer = ASPx.CreateHtmlElementFromString("<div style='float: left; width: 0px; height: " + element.offsetHeight + "px'></div>");
            element.parentNode.insertBefore(autoHeightSpacer, element);
        }
        element.style.width = this.width;
        element.style.height = this.height;

        var focusedElement = ASPx.GetFocusedElement(); // B154375, B156237

        if(ASPx.Browser.IE && ASPx.Browser.Version === 9) {  //Q435288
            ASPx.Attr.ChangeStyleAttribute(this.GetMainElement(), "display", "none");
            this.UpdatePanesVisible(ASPx.Attr.ChangeStyleAttribute);
            ASPx.Attr.RestoreStyleAttribute(this.GetMainElement(), "display");
        }
        else
            this.UpdatePanesVisible(ASPx.Attr.ChangeStyleAttribute);
        if(ASPx.Browser.WebKitFamily)
            this.CreateWebkitSpecialElement();
        var newWidth = ASPx.GetClearClientWidth(element);
        var newHeight = ASPx.GetClearClientHeight(element);
        this.UpdatePanesVisible(ASPx.Attr.RestoreStyleAttribute);
        if(autoHeightSpacer)
            element.parentNode.removeChild(autoHeightSpacer);
        if((this.rootPane.offsetWidth != newWidth) || (this.rootPane.offsetHeight != newHeight)) {

            this.rootPane.offsetWidth = Math.max(newWidth, this.defaultMinSize);
            this.rootPane.offsetHeight = Math.max(newHeight, this.defaultMinSize);

            this.rootPane.UpdatePanes(true);
        }

        try { // Focused element can be unaccessible (B185659)
            if(focusedElement &&  // B154375, B156237
                !ASPx.Browser.AndroidMobilePlatform && //Q425154
                !(ASPx.Browser.MacOSMobilePlatform && ASPx.Browser.Version >= 6) && // Q437440
                ASPx.GetIsParent(element, focusedElement) && // We shouldn't play with focused elements placed outside the splitter (B149308)
                !(focusedElement.tagName && focusedElement.tagName == "IFRAME")) { // B157503
                focusedElement.blur();
                focusedElement.focus(); // This line should be try..catched because of B156539
            }
        }
        catch (e) { }

        this.rootPane.ForEach("ApplyScrollPosition", true);
        if(this.isInitialized && !disableControlsAdjustment)
            this.rootPane.ForEach("AdjustControls", true);

        if(this.IsPrepared())
            this.sizeUpdatedOnce = true;

        this.UpdateCookie();
    },

    // Auto size
    UpdateAutoSizePanes: function (forced) {
        if(this.hasAutoSizePane) {
            var heightChanged = this.UpdateAutoHeightPanes(forced),
                widthChanged = this.UpdateAutoWidthPanes(forced);
            if(forced || heightChanged || widthChanged)
                this.rootPane.ForEach("UpdateChildrenSize");
        }
    },
    UpdateAutoHeightPanes: function (forced) {
        var changed = false;
        for(var i = 0; i < this.autoHeightPanes.length; i++)
            changed = this.autoHeightPanes[i].IsContentHeightChanged() || changed;
        if(forced || changed)
            this.UpdateAutoSizePanesSizes(false);
        return changed;
    },
    UpdateAutoWidthPanes: function (forced) {
        var changed = false;
        for(var i = 0; i < this.autoWidthPanes.length; i++)
            changed = this.autoWidthPanes[i].IsContentWidthChanged() || changed;
        if(forced || changed)
            this.UpdateAutoSizePanesSizes(true);
        return changed;
    },
    UpdateAutoSizePanesSizes: function (isVertical) {
        var autoSizePanes = isVertical
            ? this.autoWidthPanes
            : this.autoHeightPanes,
            property = isVertical ? "width" : "height",
            percentPanes = [];
        for(var i = 0; i < autoSizePanes.length; i++) {
            var pane = autoSizePanes[i];
            if(!pane.helper.HasCollapsedParent()) {
                pane.helper.ClearElementSizeProperty(property);
                for(var child = pane.panes[0]; child; child = child.nextPane)
                    if(!child.isSizePx) {
                        percentPanes.push(child);
                        child.helper.ClearElementSizeProperty(property);
                    }
            }
        }
        for(var i = 0; i < autoSizePanes.length; i++)
            autoSizePanes[i].UpdateOffsetSize(isVertical);
        for(var i = 0; i < autoSizePanes.length; i++) {
            var pane = autoSizePanes[i];
            if(!pane.helper.HasCollapsedParent())
                pane.helper.RestoreElementSizeProperty(property);
        }
        for(var i = 0; i < percentPanes.length; i++)
            percentPanes[i].helper.RestoreElementSizeProperty(property);
    },

    // Common
    UpdatePanesVisible: function (func) {
        var firstTD = this.rootPane.panes[0].helper.GetPaneElement();
        func(firstTD, "width", "1px");
        func(firstTD, "height", "1px");

        func(this.rootPane.panes[0].helper.GetContentContainerElement(), "display", "none");
        for(var i = 1; i < this.rootPane.panes.length; i++) {
            var pane = this.rootPane.panes[i];
            func(pane.helper.GetPaneElement(), "display", "none");
            var separator = pane.helper.GetSeparatorElement();
            if(separator)
                func(separator, "display", "none");
        }
    },
    UpdateCookie: function () {
        if(this.cookieName && this.cookieName != "") {
            ASPx.Cookie.DelCookie(this.cookieName);
            ASPx.Cookie.SetCookie(this.cookieName, this.GetClientStateString());
        }
    },
    UpdateStateObject: function(){
        this.UpdateStateObjectWithObject({ state: this.GetClientStateObject() });
    },

    GetPaneByPath: function (panePath, parentPane) {
        var pane = parentPane || this.rootPane;
        for(var i = 0; i < panePath.length; i++)
            pane = pane.panes[panePath[i]];
        return pane;
    },
    GetPaneByStringPath: function (paneStringPath, paneIndexSeparator) {
        if(!paneIndexSeparator)
            paneIndexSeparator = ASPx.ItemIndexSeparator;
        return this.GetPaneByPath(paneStringPath.split(paneIndexSeparator));
    },

    IsDocumentWidthChanged: function () {
        var documentWidth = this.GetDocumentWidth();
        if(!ASPx.IsExists(this.lastDocumentWidth) || documentWidth != this.lastDocumentWidth) {
            this.lastDocumentWidth = documentWidth;
            return true;
        }
        return false;
    },

    GetDocumentWidth: function () {
        if(this.fullScreen && (this.rootPane.autoHeight || this.rootPane.autoWidth))
            return this.GetDocumentWidthFullscreen();
        return ASPx.GetDocumentWidth();
    },

    GetDocumentWidthFullscreen: function () {
        var sizeElement = ASPx.CacheHelper.GetCachedValue(this, "fullscreenWidthElement", function () {
            var element = ASPx.CreateHtmlElementFromString("<div style='width: 100%; height: 0px'></div>");
            this.GetMainElement().parentNode.insertBefore(element, this.GetMainElement());
            return element;
        });
        return sizeElement.offsetWidth;
    },

    CreateWebkitSpecialElement: function () {
        var webkitSpecialElement = document.createElement("DIV"),
                element = this.GetMainElement();
        element.parentNode.appendChild(webkitSpecialElement);
        var offsetHeight = element.offsetHeight;
        element.parentNode.removeChild(webkitSpecialElement);
    },

    // Global events
    OnBrowserWindowResize: function () {
        this.UpdateControlSizes(false, true);
        this.lastDocumentWidth = this.GetDocumentWidth();
    },
    OnSeparatorMouseDown: function (moveRightPanePath) {
        var pane = this.GetPaneByStringPath(moveRightPanePath);
        var invert = this.rtl && !pane.isVertical;
        this.moveRightPane = invert ? pane.prevPane : pane;
        this.moveLeftPane = invert ? pane : pane.prevPane;
        this.moveIsVertical = pane.isVertical;
        this.moveStartPos = this.helper.GetCurrentPos();
        this.moveLastPos = this.moveStartPos;
        this.isHeavyUpdate = this.moveLeftPane.isSizePx != this.moveRightPane.isSizePx
            && !this.moveLeftPane.parent.IsAutoSize();

        if(!SplitterHelper.IsAllowResize(this.moveLeftPane, this.moveRightPane))
            return false;

        if(this.moveLeftPane.collapsed || this.moveRightPane.collapsed)
            return false;

        if(this.RaiseCancelEvent("PaneResizing", this.moveRightPane) || this.RaiseCancelEvent("PaneResizing", this.moveLeftPane))
            return false;

        var cursor = this.moveIsVertical ? "n-resize" : "w-resize";
        if(!this.liveResizing) {
            this.resizingPointer.SetCursor(cursor);
            this.resizingPointer.AttachToElement(pane.helper.GetSeparatorElement(), true);
        }
        else
            this.isInLiveResizing = true;

        this.helper.SetResizingPanelVisibility(true, cursor);

        return true;
    },
    OnSeparatorMouseUp: function () {
        this.helper.SetResizingPanelVisibility(false);

        if(!this.liveResizing || !this.isHeavyUpdate) {
            var deltaSize = this.moveLastPos - this.moveStartPos;
            if(!this.moveLeftPane.IsAutoSize(!this.moveLeftPane.isVertical)) {
                this.moveLeftPane.SetOffsetSize(this.moveLeftPane.GetOffsetSize() - deltaSize);
                this.moveLeftPane.inResizing = true;
            }

            if(!this.moveRightPane.IsAutoSize(!this.moveRightPane.isVertical)) {
                this.moveRightPane.SetOffsetSize(this.moveRightPane.GetOffsetSize() + deltaSize);
                this.moveRightPane.inResizing = true;
            }
            if(!this.liveResizing || !this.hasAutoSizePane)
                SplitterHelper.Resize(this.moveLeftPane, this.moveRightPane, deltaSize);

            this.moveLeftPane.parent.ForEach("UpdateChildrenSize");
        }

        if(!this.liveResizing)
            this.resizingPointer.SetVisibility(false);
        else
            this.isInLiveResizing = null;
        this.UpdateAutoSizePanes(true);
        this.moveLeftPane.parent.ForEach("AdjustControls");
        if(!this.liveResizing && (this.rootPane.autoHeight || this.rootPane.autoWidth) && this.IsDocumentWidthChanged())
            this.UpdateControlSizes();

        this.moveLeftPane.inResizing = null;
        this.moveRightPane.inResizing = null;

        this.UpdateCookie();
        this.RaiseEvent("PaneResizeCompleted", this.moveLeftPane);
        this.RaiseEvent("PaneResizeCompleted", this.moveRightPane);
    },
    OnMouseMove: function () {
        var deltaSize = this.helper.GetMoveMaxDeltaSize(this.helper.GetCurrentPos() - this.moveLastPos);
        if(deltaSize == 0) return;

        if(!this.moveLeftPane.IsAutoSize(!this.moveLeftPane.isVertical) || this.liveResizing)
            this.moveLeftPane.SetOffsetSize(this.moveLeftPane.GetOffsetSize() + deltaSize);
        if(!this.moveRightPane.IsAutoSize(!this.moveRightPane.isVertical) || this.liveResizing)
            this.moveRightPane.SetOffsetSize(this.moveRightPane.GetOffsetSize() - deltaSize);
        if(this.liveResizing) {
            var changePaneSize = function (pane, deltaSize) {
                pane.SetContentVisible(false);
                if(pane.ApplyElementSize()) {
                    pane.ForEach("UpdateChildrenSize");
                    pane.SetContentVisible(true);
                    pane.RaiseResizedEvent();
                }
            }
            if(this.isHeavyUpdate || this.moveLeftPane.parent.autoHeight || this.moveLeftPane.parent.autoWidth) {
                SplitterHelper.Resize(this.moveLeftPane, this.moveRightPane, this.moveLeftPane.isSizePx || this.moveRightPane.isSizePx ? deltaSize : 0);
                this.moveLeftPane.parent.ForEach("UpdateChildrenSize");
            }
            else {
                changePaneSize(this.moveLeftPane, deltaSize, this.helper);
                changePaneSize(this.moveRightPane, -deltaSize, this.helper);
            }
            this.UpdateAutoSizePanes(this.liveResizing);
        }
        else
            this.resizingPointer.Move(deltaSize, !this.moveIsVertical);
        this.moveLastPos += deltaSize;
    },
    OnCollapseButtonClick: function (panePath, forwardDirection) {
        var rightPane = this.GetPaneByStringPath(panePath);

        var pane1 = forwardDirection ? rightPane.prevPane : rightPane;
        var pane2 = forwardDirection ? rightPane : rightPane.prevPane;
        if(pane1.collapsed && pane1.maximizedPane == pane2) {
            if(!this.RaiseCancelEvent("PaneExpanding", pane1)) {
                pane1.Expand();
                if(this.savedSize) {
                    var rootPaneChildren = this.rootPane.panes;
                    for(var i = 0; i < rootPaneChildren.length; i++) {
                        if(rootPaneChildren[i].IsCollapsed())
                            return;
                    }
                    for(var sizeProperty in this.savedSize)
                        this.GetMainElement().style[sizeProperty] = this.savedSize[sizeProperty];
                    this.savedSize = null;
                }
            }
        }
        else {
            if(!this.RaiseCancelEvent("PaneCollapsing", pane2)) {
                if(pane2.NeedResetSplitterSizeOnCollapsing(pane1)) {
                    if(!this.savedSize)
                        this.savedSize = {};
                    var sizeProperty = pane1.isVertical ? "height" : "width";
                    if(!this.savedSize[sizeProperty]) {
                        this.savedSize[sizeProperty] = this.GetMainElement().style[sizeProperty];
                        this.GetMainElement().style[sizeProperty] = "";
                    }
                }
                pane2.Collapse(pane1);
            }
        }
    },

    IsEmptyUrl: function (url) {
        for(var i = 0; i < this.emptyUrls.length; i++)
            if(url == this.emptyUrls[i])
                return true;
        return false;
    },

    // Events
    RaiseEvent: function (eventName, pane) {
        if(this.isInitialized)
            this[eventName].FireEvent(this, new ASPxClientSplitterPaneEventArgs(pane));
    },
    RaiseCancelEvent: function (eventName, pane) {
        var args = new ASPxClientSplitterPaneCancelEventArgs(pane);
        this[eventName].FireEvent(this, args);
        return args.cancel;
    },
    GetPaneCount: function () {
        return this.rootPane.GetPaneCount();
    },
    GetPane: function (index) {
        return this.rootPane.GetPane(index);
    },
    GetPaneByName: function (name) {
        return this.rootPane.GetPaneByName(name);
    },
    SetAllowResize: function (allowResize) {
        if(this.allowResize == allowResize)
            return;
        this.allowResize = allowResize;
        this.rootPane.ForEach("UpdateSeparatorStyle", true);
    },
    GetLayoutData: function() {
        return this.GetClientStateString();
    },

    SetWidth: function (width) {
        this.width = width + "px";
        if(this.IsPrepared())
            this.UpdateControlSizes(true);
    },
    SetHeight: function (height) {
        this.height = height + "px";
        if(this.IsPrepared())
            this.UpdateControlSizes(true);
    }
});
ASPxClientSplitter.Cast = ASPxClientControl.Cast;
var ASPxClientSplitterPane = ASPx.CreateClass(null, {
    // Constructor
    constructor: function (splitter, parent, visibleIndex, index, paneInfo) {
        this.splitter = splitter;
        this.parent = parent;
        this.index = visibleIndex;
        this._index = index;
        this.name = paneInfo.n || "";

        this.isRootPane = (this.parent == null);
        this.helper = new ASPxSplitterPaneHelper(this);

        this.prevPane = null;
        this.nextPane = null;
        this.panes = [];
        this.isVertical = this.isRootPane ? false : !parent.isVertical;
        this.hasSeparator = (this.index > 0);

        this.collapsed = ASPx.IsExists(paneInfo.c);
        this.size = ASPx.IsExists(paneInfo.s) ? paneInfo.s : 0;
        this.sizeType = ASPx.IsExists(paneInfo.st) ? paneInfo.st : null;
        this.autoWidth = ASPx.IsExists(paneInfo.aw);
        this.autoHeight = ASPx.IsExists(paneInfo.ah);
        this.maxSize = ASPx.IsExists(paneInfo.smax) ? paneInfo.smax : Number.MAX_VALUE;
        this.minSize = ASPx.IsExists(paneInfo.smin) ? paneInfo.smin : this.splitter.defaultMinSize;
        this.allowResize = !ASPx.IsExists(paneInfo.nar);
        this.showCollapseBackwardButton = ASPx.IsExists(paneInfo.scbb);
        this.showCollapseForwardButton = ASPx.IsExists(paneInfo.scfb);

        this.iframe = {};
        if(paneInfo.iframe) {
            this.iframe = {
                src: paneInfo.iframe[0],
                scrolling: paneInfo.iframe[1]
            };
            if(paneInfo.iframe[2] != "")
                this.iframe.name = paneInfo.iframe[2];
            if(paneInfo.iframe[3] != "")
                this.iframe.title = paneInfo.iframe[3];
            this.isContentUrl = true;
        }

        this.scrollTop = paneInfo.spt || 0;
        this.scrollLeft = paneInfo.spl || 0;

        this.isSizePx = (this.sizeType == "px");

        this.maximizedPane = null;
        this.dragPrevented = false;
        this.offsetWidth = 0;
        this.offsetHeight = 0;
        this.widthDiff = 0;
        this.heightDiff = 0;
        this.contentContainerWidthDiff = 0;
        this.contentContainerHeightDiff = 0;
        this.collapsedWidthDiff = 0;
        this.collapsedHeightDiff = 0;

        this.isASPxClientSplitterPane = true;
    },

    // Creation
    UpdateSize: function () {
        if(!this.panes.length) return;

        var prcSum = 0,
            emptyPanesCount = 0;
        for(var pane = this.panes[0]; pane; pane = pane.nextPane) {
            if(!pane.sizeType)
                emptyPanesCount++;
            else if(pane.sizeType == "%")
                prcSum += pane.size;
        }

        if(emptyPanesCount) {
            var emptyPaneSize = Math.max(100 - prcSum, 0) / emptyPanesCount;
            for(var pane = this.panes[0]; pane; pane = pane.nextPane) {
                if(!pane.sizeType) {
                    pane.sizeType = "%";
                    pane.size = emptyPaneSize;
                }
            }
        }
        if(prcSum && (!emptyPanesCount && prcSum != 100 || prcSum > 100)) {
            for(var pane = this.panes[0]; pane; pane = pane.nextPane) {
                if(pane.sizeType == "%")
                    pane.size = 100 * pane.size / prcSum;
            }
        }
    },

    // Auto size
    UpdateAutoSize: function () {
        if(this.panes.length) {
            var propertyAll = this.isVertical ? "autoHeight" : "autoWidth",
                propertyOne = this.isVertical ? "autoWidth" : "autoHeight";
            if(this[propertyAll]) {
                for(var pane = this.panes[0]; pane; pane = pane.nextPane) {
                    pane[propertyAll] = true;
                }
            }
            if(this[propertyOne]) {
                var selected;
                for(var pane = this.panes[0]; pane; pane = pane.nextPane) {
                    if(pane[propertyOne] || !pane.isSizePx || pane.isSizePx && !selected && !pane.nextPane)
                        selected = pane;
                    if(pane[propertyOne])
                        break;
                }
                selected[propertyOne] = true;
            }

            for(var pane = this.panes[0]; pane; pane = pane.nextPane) {
                if(pane.isSizePx)
                    continue;
                if(pane[propertyOne]) {
                    pane.size = pane.GetMinSize();
                    pane.sizeType = "px";
                    pane.isSizePx = true;
                }
            }
        }
        if(!this.isRootPane) {
            if(this.autoHeight)
                this.splitter.autoHeightPanes.push(this);
            if(this.autoWidth)
                this.splitter.autoWidthPanes.push(this);
        }
    },
    IsAutoSize: function (isVertical) {
        if(isVertical == null)
            isVertical = this.isVertical;
        return isVertical ? this.autoWidth : this.autoHeight;
    },
    IsContentHeightChanged: function () {
        var contentHeight = this.helper.GetContentContainerElement().offsetHeight;

        if(!ASPx.IsExists(this.lastContentHeight) || contentHeight != this.lastContentHeight) {
            this.lastContentHeight = contentHeight;
            return true;
        }
        return false;
    },

    IsContentWidthChanged: function () {
        var contentWidth = this.helper.GetContentContainerElement().offsetWidth;

        if(!ASPx.IsExists(this.lastContentWidth) || contentWidth != this.lastContentWidth) {
            this.lastContentWidth = contentWidth;
            return true;
        }
        return false;
    },

    UpdateOffsetSize: function (isVertical) {
        var hasPanes = !!this.panes.length,
            contentContainerSizeDiff = hasPanes
                ? 0
                : isVertical
                    ? this.widthDiff
                    : this.heightDiff,
                contentSize = 0;
        if(this.isContentUrl && !hasPanes) {
            var element = this.helper.GetContentContainerElement();
            element.style.display = "none";
        }
        var contentSize = this.GetContentMinSize(isVertical);
        this.SetOffsetSize(Math.max(this.GetMinSize(!isVertical), contentSize), !isVertical);
        if(this.isContentUrl && !hasPanes) {
            element.style[isVertical ? "width" : "height"] = this.isVertical || isVertical
                ? "100%"
                : this.helper.GetPaneElement().offsetHeight - contentContainerSizeDiff + "px";
            element.style.display = "";
        }
    },

    GetContentMinSize: function (isVertical) {
        if(!this.panes.length) {
            var contentContainerElement = this.helper.GetContentContainerElement(),
                contentContainerSizeDiff = isVertical
                    ? this.widthDiff
                    : this.heightDiff;
            return (isVertical ? contentContainerElement.offsetWidth : contentContainerElement.offsetHeight) + contentContainerSizeDiff;
        }
        var contentSize = 0;
        if(this.isVertical != isVertical)
            for(var pane = this.panes[0]; pane; pane = pane.nextPane)
                contentSize = Math.max(contentSize, pane.GetContentMinSize(isVertical));
        else {
            for(var pane = this.panes[0]; pane; pane = pane.nextPane)
                contentSize += pane.GetContentMinSize(isVertical);
            contentSize += this.GetTotalSeparatorsSize(!this.isVertical);
        }
        return contentSize;
    },

    // Initialization
    Initialize: function () {
        this.InitializePreventDragging();

        if(this.isRootPane)
            return;
        if(this.collapsed) {
            if(this.IsFirstPane())
                this.maximizedPane = this.GetNextPane();
            else if(this.IsLastPane())
                this.maximizedPane = this.GetPrevPane();
            else if(this.prevPane.maximizedPane && this.IsFirstPane())
                this.maximizedPane = this.prevPane;
            else
                this.maximizedPane = this.nextPane;

            if(this.maximizedPane == null)
                this.collapsed = false;
        }
    },
    Prepare: function () {
        var EvaluateWidthDiff = function (element) {
            return element.offsetWidth - element.clientWidth;
        };
        var EvaluateHeightDiff = function (element) {
            var elementClientHeight = ((ASPx.Browser.Safari && (ASPx.Browser.Version < 4)) || (ASPx.Browser.Chrome && (ASPx.Browser.Version < 2))) ? (element.offsetHeight - element.clientTop * 2) : element.clientHeight;
            return element.offsetHeight - elementClientHeight;
        };

        this.GetSeparatorSize();

        var element = this.helper.GetPaneElement();

        if(ASPx.Browser.IE && ASPx.Browser.MajorVersion == 9) { // B203253
            var b203253_TestWidth = EvaluateWidthDiff(element);
            if(b203253_TestWidth > 10000) { // We assume that nobody would like to get more than 10000px of width
                ASPx.Attr.ChangeStyleAttribute(document.body, "width", "1px");
                var b203253_BodyWidthChanged = true;
            }
        }

        this.widthDiff = EvaluateWidthDiff(element);
        this.heightDiff = EvaluateHeightDiff(element); // used in XtraReports.Web (B253837)

        if(this.panes.length == 0) {
            var contentContainerElement = this.helper.GetContentContainerElement();
            ASPx.SetScrollBarVisibility(contentContainerElement, false);
            ASPx.SetStyles(contentContainerElement, { width: 1, height: 1 });

            this.contentContainerWidthDiff = contentContainerElement.offsetWidth - 1;
            this.contentContainerHeightDiff = contentContainerElement.offsetHeight - 1;
            if(this.autoWidth) {
                contentContainerElement.style.width = "";
                var minWidthValue = this.splitter.defaultMinSize - this.contentContainerWidthDiff;
                if(minWidthValue > -1)
                    contentContainerElement.style.minWidth = minWidthValue + "px";
            }
            if(this.autoHeight) {
                contentContainerElement.style.height = "";
                var minHeightValue = this.splitter.defaultMinSize - this.contentContainerWidthDiff;
                if(minHeightValue > -1)
                    contentContainerElement.style.minHeight = minHeightValue + "px";
            }
            ASPx.SetScrollBarVisibility(contentContainerElement, true);

            if(!this.scrollEventAttached) {
                var _this = this;
                ASPx.Evt.AttachEventToElement(contentContainerElement, "scroll", function () {
                    if(contentContainerElement.scrollTop >= 0)
                        _this.scrollTop = contentContainerElement.scrollTop;
                    if(contentContainerElement.scrollLeft >= 0)
                        _this.scrollLeft = contentContainerElement.scrollLeft;
                    _this.splitter.UpdateCookie();
                });
                this.scrollEventAttached = true;
            }
        }

        this.UpdateStyle(element, true);
        this.collapsedWidthDiff = EvaluateWidthDiff(element);
        this.collapsedHeightDiff = EvaluateHeightDiff(element);
        this.UpdateStyle(element, false);

        if(ASPx.Browser.IE && ASPx.Browser.MajorVersion == 9 && b203253_BodyWidthChanged) // B203253
            ASPx.Attr.RestoreStyleAttribute(document.body, "width");

        var separator = this.helper.GetSeparatorElement();
        if(separator) {
            ASPx.SetElementDisplay(this.helper.GetSeparatorDivElement(), false);
            if(!this.isVertical)
                this.separatorSizeDiff = separator.offsetWidth - separator.clientWidth;
            else
                this.separatorSizeDiff = separator.offsetHeight - separator.clientHeight;
            ASPx.SetElementDisplay(this.helper.GetSeparatorDivElement(), true);
        }
        else
            this.separatorSizeDiff = 0;

        this.PrepareSeparatorButtons();

        if(ASPx.Browser.WebKitTouchUI) {
            var contentContainer = this.helper.GetContentContainerElement();
            var scrollbarVisible = contentContainer.style.overflow == "auto" || contentContainer.style.overflow == "scroll";
            var hScrollbarVisible = scrollbarVisible || contentContainer.style.overflowX == "scroll";
            var vScrollbarVisible = scrollbarVisible || contentContainer.style.overflowY == "scroll";
            if(hScrollbarVisible || vScrollbarVisible) {
                ASPx.TouchUIHelper.MakeScrollable(contentContainer, { showHorizontalScrollbar: hScrollbarVisible, showVerticalScrollbar: vScrollbarVisible });
            }
        }

        if(!this.isRootPane) {
            if(this.autoHeight)
                this.offsetHeight = this.GetMinSize(true);
            if(this.autoWidth)
                this.offsetWidth = this.GetMinSize(false);
        }
    },
    PrepareSeparatorButtons: function () {
        if(!(this.hasSeparator && this.helper.buttonsExists))
            return;

        var sizeProperty = this.isVertical ? "offsetWidth" : "offsetHeight";

        if(this.helper.buttonsTableExists) {
            this.collapseBackwardButtonSize = this.helper.GetButtonUpdateElement(this.helper.GetCollapseBackwardButton())[sizeProperty];
            this.collapseForwardButtonSize = this.helper.GetButtonUpdateElement(this.helper.GetCollapseForwardButton())[sizeProperty];

            this.buttonsTableDiffSize = this.helper.GetCollapseButtonsTable()[sizeProperty] - this.collapseBackwardButtonSize - this.collapseForwardButtonSize;

            if(this.helper.separatorImageExists) {
                this.collapseButtonsSeparatorSize = this.helper.GetButtonUpdateElement(this.helper.GetCollapseButtonsSeparator())[sizeProperty];
                this.buttonsTableDiffSize -= this.collapseButtonsSeparatorSize;
            }
        }
        else
            this.collapseButtonsSeparatorSize = this.helper.GetCollapseButtonsSeparatorImage()[sizeProperty];
    },
    InitializePreventDragging: function () {
        if(!this.dragPrevented && this.helper.separatorImageExists) {
            ASPx.Evt.PreventElementDrag(this.helper.GetCollapseButtonsSeparatorImage());
            this.dragPrevented = true;
        }
    },
    ApplyScrollPosition: function () {
        if(this.panes.length == 0) {
            this.SetScrollTop(this.scrollTop);
            this.SetScrollLeft(this.scrollLeft);
        }
    },

    // Common
    ForEach: function (funcName, skippSelf) {
        if(!skippSelf)
            this[funcName]();
        for(var i = 0; i < this.panes.length; i++)
            this.panes[i].ForEach(funcName);
    },
    SetContentVisible: function (visible) {
        ASPx.SetElementDisplay(this.helper.GetContentContainerElement(), visible);

        // This is a hack for"empty td"-"borders gone" IE problem
        // "empty-cells: show;" isn't supported by IE6/IE7
        if(ASPx.Browser.IE)
            this.helper.SetEmptyDivVisible(!visible);
    },

    // Elements updating
    AdjustControls: function () {
        if(this.panes.length == 0 && !this.collapsed && !this.isContentUrl)
            ASPx.GetControlCollection().AdjustControls(this.helper.GetContentContainerElement(), true);
    },
    UpdatePanes: function (forceAutoSizeUpdate) {
        this.ForEach("UpdateVisualElements", true);
        this.ForEach("UpdateChildrenSize");
        this.splitter.UpdateAutoSizePanes(forceAutoSizeUpdate);
    },
    UpdateVisualElements: function () {
        this.UpdateButtonsVisibility();
        this.UpdateSeparatorStyle();
        this.UpdatePaneStyle();
    },
    IsBackwardButtonVisible: function () {
        return ASPx.CacheHelper.GetCachedValue(this, "isBackwardButtonVisible", function () {
            if(!this.helper.buttonsTableExists)
                return false;

            if(this.collapsed && (this.maximizedPane == this.prevPane))
                return true;
            if(this.nextPane && this.collapsed && this.nextPane.collapsed)
                return true;
            if(this.prevPane.collapsed)
                return false;
            return this.showCollapseBackwardButton;
        }, this.helper);
    },
    IsForwardButtonVisible: function () {
        return ASPx.CacheHelper.GetCachedValue(this, "isForwardButtonVisible", function () {
            if(!this.helper.buttonsTableExists)
                return false;

            if(this.prevPane.collapsed && (this.prevPane.maximizedPane == this))
                return true;
            if(this.prevPane.collapsed && !this.collapsed)
                return true;
            if(this.collapsed)
                return false;
            return this.showCollapseForwardButton;
        }, this.helper);
    },
    DropCachedButtonsVisible: function () {
        ASPx.CacheHelper.DropCachedValue(this.helper, "isBackwardButtonVisible");
        ASPx.CacheHelper.DropCachedValue(this.helper, "isForwardButtonVisible");
    },
    UpdateSeparatorStyle: function () {
        var separator = this.helper.GetSeparatorElement();
        if(!separator) return;

        var prevPane = this.prevPane,
            isCollapsed = this.collapsed || prevPane && prevPane.collapsed,
            resizingEnabled = SplitterHelper.IsAllowResize(this, prevPane);
        if(this.splitter.IsStateControllerEnabled())
            ASPx.GetStateController().SetMouseStateItemsEnabled(this.helper.GetSeparatorElementId(), null, !isCollapsed && resizingEnabled);

        this.UpdateStyle(separator, isCollapsed);
    },
    UpdatePaneStyle: function () {
        this.UpdateStyle(this.helper.GetPaneElement(),
            this.collapsed && !this.NeedResetSplitterSizeOnCollapsing(this.maximizedPane) && !this.NeedKeepOffsetSizeOnCollapsing()
        );
    },
    UpdateStyle: function (element, isSelect) {
        if(!this.splitter.IsStateControllerEnabled()) return;

        if(isSelect)
            ASPx.GetStateController().SelectElementBySrcElement(element);
        else
            ASPx.GetStateController().DeselectElementBySrcElement(element);
    },
    UpdateButtonsVisibility: function () {
        if(!(this.hasSeparator && this.helper.buttonsExists))
            return;

        var separatorSize = this.GetOffsetSize(!this.isVertical) - this.separatorSizeDiff;
        if(this.helper.buttonsTableExists) {
            var buttonsSize = this.buttonsTableDiffSize;
            if(this.IsBackwardButtonVisible())
                buttonsSize += this.collapseBackwardButtonSize;
            if(this.IsForwardButtonVisible())
                buttonsSize += this.collapseForwardButtonSize;

            var buttonsVisible = (buttonsSize <= separatorSize);
            var backwardButtonVisible = buttonsVisible && this.IsBackwardButtonVisible();
            var forwardButtonVisible = buttonsVisible && this.IsForwardButtonVisible();

            ASPx.SetElementDisplay(this.helper.GetButtonUpdateElement(this.helper.GetCollapseBackwardButton()), backwardButtonVisible);
            ASPx.SetElementDisplay(this.helper.GetButtonUpdateElement(this.helper.GetCollapseForwardButton()), forwardButtonVisible);

            if(this.helper.separatorImageExists) {
                if(!buttonsVisible)
                    buttonsSize = this.buttonsTableDiffSize;
                buttonsSize += this.collapseButtonsSeparatorSize;

                var separatorImageVisible = this.splitter.showSeparatorImage && (backwardButtonVisible === forwardButtonVisible) && (buttonsSize <= separatorSize);
                ASPx.SetElementDisplay(this.helper.GetButtonUpdateElement(this.helper.GetCollapseButtonsSeparator()), separatorImageVisible);
            }
        }
        else {
            var separatorImageVisible = this.splitter.showSeparatorImage && (this.collapseButtonsSeparatorSize <= separatorSize);
            ASPx.SetElementDisplay(this.helper.GetCollapseButtonsSeparatorImage(), separatorImageVisible);
        }
    },

    // Size evaluation common
    GetSeparatorSize: function () {
        return ASPx.CacheHelper.GetCachedValue(this, "SeparatorSize", function () {
            var separator = this.helper.GetSeparatorElement();
            return separator ? (this.isVertical ? separator.offsetHeight : separator.offsetWidth) : 0;
        }, this.helper);
    },
    GetTotalSeparatorsSize: function (isVertical) {
        if(!ASPx.IsExists(isVertical) || (isVertical == this.isVertical))
            return 0;
        var cacheKey = (isVertical ? "v" : "h") + "TotalSeparatorsSize";
        return ASPx.CacheHelper.GetCachedValue(this, cacheKey, function () {
            var result = 0;
            for(var i = 0; i < this.panes.length; i++)
                result += this.panes[i].GetSeparatorSize();
            return result;
        }, this.helper);
    },
    GetMinSize: function (isVertical) {
        if(!ASPx.IsExists(isVertical))
            isVertical = this.isVertical;
        var cacheKey = (isVertical ? "v" : "h") + "ItemMinSize";
        return ASPx.CacheHelper.GetCachedValue(this, cacheKey, function () {
            var result = 0;
            for(var i = 0; i < this.panes.length; i++)
                if(isVertical != this.isVertical)
                    result += this.panes[i].GetMinSize(isVertical);
                else
                    result = Math.max(result, this.panes[i].GetMinSize(isVertical));

            result += this.GetTotalSeparatorsSize(isVertical);

            var minSize = (isVertical == this.isVertical) ? this.minSize : this.splitter.defaultMinSize;
            result = Math.max(result, Math.max(minSize, this.GetSizeDiff(isVertical)));

            return result;
        }, this.helper);
    },
    DropCachedSizes: function () {
        ASPx.CacheHelper.DropCachedValue(this.helper, "SeparatorSize");
        ASPx.CacheHelper.DropCachedValue(this.helper, "vTotalSeparatorsSize");
        ASPx.CacheHelper.DropCachedValue(this.helper, "hTotalSeparatorsSize");
        ASPx.CacheHelper.DropCachedValue(this.helper, "ItemMinSize");
    },
    IsMaxSizeSpecified: function () {
        return this.maxSize != Number.MAX_VALUE;
    },
    GetMaxSize: function () {
        return Math.max(this.maxSize, this.GetSizeDiff(this.isVertical));
    },

    // Size evaluation core
    PrepareUpdateInfo: function () {
        var updateInfo = {};

        var prepareUpdateInfoPart = function () {
            return {
                panes: [],
                sum: 0,
                sumMin: 0,
                sumMax: 0,
                addPane: function () {
                    this.panes.push(pane);
                    if(pane.collapsed) {
                        var sizeDiff = pane.GetSizeDiff(pane.isVertical);
                        this.sum += sizeDiff;
                        this.sumMin += sizeDiff;
                    }
                    else {
                        this.sum += pane.size;
                        this.sumMin += pane.GetMinSize();
                    }
                    this.sumMax += pane.GetMaxSize();
                },
                IsIgnoreMaxSize: function () {
                    return this.sumMax < this.sum;
                }
            };
        };

        updateInfo.px = prepareUpdateInfoPart();
        updateInfo.prc = prepareUpdateInfoPart();
        updateInfo.collapsed = prepareUpdateInfoPart();
        updateInfo.autoSize = prepareUpdateInfoPart();

        updateInfo.onlyPxPanes = true; // TODO: evaluate it once on page load
        updateInfo.hasPxPanesShown = false;
        updateInfo.hasPrcPanesShown = false;
        for(var i = 0; i < this.panes.length; i++) {
            var pane = this.panes[i];

            if(pane.collapsed)
                updateInfo.collapsed.addPane(pane);
            else if(pane.IsAutoSize(this.isVertical) && pane.GetOffsetSize()) {
                updateInfo.autoSize.addPane(pane)
            }
            else if(pane.isSizePx) {
                updateInfo.px.addPane(pane);
                updateInfo.hasPxPanesShown = true;
            }
            else {
                updateInfo.prc.addPane(pane);
                updateInfo.hasPrcPanesShown = true;
            }

            if(!pane.isSizePx)
                updateInfo.onlyPxPanes = false;
        }

        updateInfo.px.isIgnoreMaxSize = (!updateInfo.hasPrcPanesShown && (updateInfo.px.sumMax < updateInfo.px.sum));
        updateInfo.prc.isIgnoreMaxSize = (updateInfo.prc.sumMax < updateInfo.prc.sum);

        return updateInfo;
    },
    SetChildrenSecondSize: function () {
        var orientation = this.isVertical;
        var size = this.GetClientSize(orientation);
        if(this.isRootPane)
            for(var pane = this.panes[0]; pane; pane = pane.nextPane) {
                if(pane.IsAutoSize(!this.isVertical))
                    size = Math.max(size, pane.GetOffsetSize(this.isVertical));
            }
        for(var i = 0; i < this.panes.length; i++)
            this.panes[i].SetOffsetSize(size, orientation);
    },
    GetChildrenTotalSize: function () {
        return this.GetClientSize(!this.isVertical) - this.GetTotalSeparatorsSize(!this.isVertical);
    },
    UpdateChildrenSize: function () {
        if(this.collapsed || (this.panes.length == 0))
            return;

        var updateInfo = this.PrepareUpdateInfo();
        var childrenTotalSize = this.GetChildrenTotalSize();

        var asTotalSize = 0;
        for(var i = 0; i < updateInfo.autoSize.panes.length; i++) {
            var pane = updateInfo.autoSize.panes[i];
            pane.size = pane.GetOffsetSize();
            asTotalSize += pane.size;
        }
        if(!updateInfo.hasPxPanesShown && !updateInfo.hasPrcPanesShown) {
            var asMaxSize = childrenTotalSize - (updateInfo.px.sumMin + updateInfo.prc.sumMin + updateInfo.collapsed.sumMin);
            asTotalSize = this.NormalizePanesSizes(updateInfo.autoSize.panes, asTotalSize, asMaxSize);
        }
        else {
            var pxMaxSize = childrenTotalSize - (updateInfo.prc.sumMin + updateInfo.collapsed.sumMin) - asTotalSize,
                isOutOfParentSize = !!(pxMaxSize < 0 && updateInfo.autoSize.panes.length);
            if(isOutOfParentSize)
                pxMaxSize = updateInfo.px.sum;

            var pxTotalSize = 0;
            if(updateInfo.hasPxPanesShown) {
                var c = !updateInfo.hasPrcPanesShown && !isOutOfParentSize && !updateInfo.autoSize.panes.length
                    ? (pxMaxSize / (updateInfo.px.sum + updateInfo.autoSize.sum))
                    : 1;
                for(var i = 0; i < updateInfo.px.panes.length; i++) {
                    var pane = updateInfo.px.panes[i];
                    var newSize = pxMaxSize > 0
                        ? Math.max(Math.round(pane.size * c), pane.GetMinSize())
                        : pane.GetMinSize();
                    if(!updateInfo.px.isIgnoreMaxSize)
                        newSize = Math.min(newSize, pane.GetMaxSize());
                    pane.SetOffsetSize(newSize);
                    pxTotalSize += newSize;
                }

                if(pxMaxSize > 0 && (!updateInfo.hasPrcPanesShown || (pxTotalSize > pxMaxSize))) {
                    pxTotalSize = this.NormalizePanesSizes(updateInfo.autoSize.panes, pxTotalSize, pxMaxSize);
                    pxTotalSize = this.NormalizePanesSizes(updateInfo.px.panes, pxTotalSize, pxMaxSize);
                }
                if(updateInfo.onlyPxPanes && !(this.IsAutoSize(this.isVertical) && !updateInfo.autoSize.panes.length)) {
                    for(var i = 0; i < updateInfo.px.panes.length; i++) {
                        var pane = updateInfo.px.panes[i];
                        pane.size = pane.GetOffsetSize();
                    }
                }
            }

            var prcMaxSize = pxMaxSize - pxTotalSize + updateInfo.prc.sumMin;
            var prcTotalSize = 0;
            if((prcMaxSize > 0) && updateInfo.hasPrcPanesShown) {
                var c = 1 / updateInfo.prc.sum;

                for(var i = 0; i < updateInfo.prc.panes.length; i++) {
                    var pane = updateInfo.prc.panes[i];
                    var newSize = Math.max(Math.round(pane.size * c * (childrenTotalSize - pxTotalSize - asTotalSize)), pane.GetMinSize());
                    if(!updateInfo.prc.isIgnoreMaxSize)
                        newSize = Math.min(newSize, pane.GetMaxSize());
                    pane.SetOffsetSize(newSize);
                    prcTotalSize += newSize;
                }

                if(prcTotalSize != prcMaxSize)
                    prcTotalSize = this.NormalizePanesSizes(updateInfo.prc.panes, prcTotalSize, prcMaxSize);
            }
        }
        for(var i = 0; i < updateInfo.collapsed.panes.length; i++) {
            var pane = updateInfo.collapsed.panes[i],
                collapsedSize = pane.GetSizeDiff(pane.isVertical);
            if(!(ASPx.Browser.IE && pane.NeedKeepOffsetSizeOnCollapsing()))
                pane.SetOffsetSize(collapsedSize);
        }
        if(ASPx.Browser.WebKitFamily && updateInfo.collapsed.panes.length && this.IsAutoSize(this.IsVertical))
            this.splitter.CreateWebkitSpecialElement();
        this.SetChildrenSecondSize();

        for(var i = 0; i < this.panes.length; i++) {
            var pane = this.panes[i];
            if(pane.collapsed)
                pane.SetContentVisible(false);
            else
                pane.SetContentVisible(true);
            if(pane.ApplyElementSize())
                pane.RaiseResizedEvent();
        }

        if(ASPx.Browser.Chrome) { //T166315
            for(var i = 0; i < this.panes.length; i++)
                this.panes[i].doLessWidth();
            this.splitter.GetMainElement().offsetWidth;
            for(var i = 0; i < this.panes.length; i++)
                this.panes[i].restoreWidth();
        }

        this.ForEach("UpdateButtonsVisibility", true);
    },
    GetPossibleUp: function () {
        if(this.inResizing)
            return -1;
        return this.GetMaxSize() - this.GetOffsetSize();
    },
    GetPossibleDown: function () {
        if(this.IsAutoSize(!this.isVertical) && !(this.panes.length && !this.helper.HasVisibleAutoSizeChildren(!this.isVertical)))
            return -1;
        if(this.inResizing)
            return -1;
        return this.GetOffsetSize() - this.GetMinSize();
    },
    NormalizePanesSizes: function (panes, size, maxSize) {
        var insufficientSize = maxSize - size;
        var changeStep = (insufficientSize > 0) ? 1 : -1;
        var possibleChangeFunction = (insufficientSize > 0) ? "GetPossibleUp" : "GetPossibleDown";

        var changed = true;
        while((Math.floor(insufficientSize) != 0) && changed) {
            changed = false;
            for(var i = 0; i < panes.length; i++) {
                var pane = panes[i];
                if(pane[possibleChangeFunction]() > 0) {
                    pane.SetOffsetSize(pane.GetOffsetSize() + changeStep);
                    insufficientSize -= changeStep;
                    changed = true;

                    if(insufficientSize == 0)
                        break;
                }
            }
        }
        return maxSize - insufficientSize;
    },

    // Sizes
    GetOffsetSize: function (isVertical) {
        if(!ASPx.IsExists(isVertical))
            isVertical = this.isVertical;
        return isVertical ? this.offsetHeight : this.offsetWidth;
    },
    GetClientSize: function (isVertical) {
        return isVertical ? this.GetClientHeightInternal(true) : this.GetClientWidthInternal(true);
    },
    SetOffsetSize: function (value, isVertical) {
        if(!ASPx.IsExists(isVertical))
            isVertical = this.isVertical;
        if(isVertical)
            this.offsetHeight = value;
        else
            this.offsetWidth = value;
    },
    GetSizeDiff: function (isVertical) {
        return isVertical ? this.GetHeightDiff(true) : this.GetWidthDiff(true);
    },
    GetWidthDiff: function (isContainer) {
        if(this.collapsed)
            return this.collapsedWidthDiff;
        return this.widthDiff + (isContainer ? this.contentContainerWidthDiff : 0);
    },
    GetHeightDiff: function (isContainer) {
        if(this.collapsed)
            return this.collapsedHeightDiff;
        return this.heightDiff + (isContainer ? this.contentContainerHeightDiff : 0);
    },
    GetClientWidthInternal: function (isContainer) {
        if(ASPx.Browser.Firefox && this.autoWidth)
            return this.offsetWidth;
        return this.offsetWidth - this.GetWidthDiff(isContainer);
    },
    GetClientHeightInternal: function (isContainer) {
        if(ASPx.Browser.Firefox && this.autoHeight)
            return this.offsetHeight - (isContainer ? 0 : ASPx.GetVerticalBordersWidth(this.GetElement()));
        return this.offsetHeight - this.GetHeightDiff(isContainer);
    },

    // Size updating
    ApplyElementSize: function () {
        if(this.IsSizeChanged()) {
            this.ApplyElementSizeCore();
            var contentContainerElement = this.helper.GetContentContainerElement();

            if(ASPx.Browser.Chrome && ASPx.Browser.MajorVersion >= 3
                    || ASPx.Browser.Safari && ASPx.Browser.MajorVersion >= 5) {
                var marginRight = ASPx.PxToInt(contentContainerElement.style.marginRight);
                marginRight -= ASPx.PxToInt(ASPx.GetCurrentStyle(contentContainerElement).marginRight);
                contentContainerElement.style.marginRight = marginRight + "px";
            }

            if(ASPx.Browser.WebKitFamily) {
                this.splitter.CreateWebkitSpecialElement(); //Q424762
                var updated = ASPx.SetScrollBarVisibilityCore(contentContainerElement, "overflowY", this.GetClientWidthInternal(true) > ASPx.GetVerticalScrollBarWidth());
                if(updated && this.isContentUrl)
                    this.RefreshContentUrl();
            }

            return true;
        }
        return false;
    },
    ApplyElementSizeCore: function () {
        var paneWidth = this.GetClientWidthInternal(false);
        var paneHeight = this.GetClientHeightInternal(false);
        var contentContainerWidth = this.GetClientWidthInternal(true);
        var contentContainerHeight = this.GetClientHeightInternal(true);
        if(contentContainerWidth < 0) {
            paneWidth -= contentContainerWidth;
            contentContainerWidth = 0;
        }
        if(contentContainerHeight < 0) {
            paneHeight -= contentContainerHeight;
            contentContainerHeight = 0;
        }

        var paneElement = this.helper.GetPaneElement(),
            contentContainerElement = this.helper.GetContentContainerElement();
        if(!isNaN(paneWidth) && !(paneWidth === 0 && !this.collapsed))
            paneElement.style.width = paneWidth + "px";
        if(!isNaN(paneHeight) && !(paneHeight === 0 && !this.collapsed))
            paneElement.style.height = paneHeight + "px";
        if(!this.autoWidth && !isNaN(contentContainerWidth))
            contentContainerElement.style.width = contentContainerWidth + "px";
        if(!this.autoHeight && !isNaN(contentContainerHeight))
            contentContainerElement.style.height = contentContainerHeight + "px";
    },
    IsSizeChanged: function () {
        if(!ASPx.IsExists(this.lastWidth) || !ASPx.IsExists(this.lastHeight) ||
            (this.offsetWidth != this.lastWidth) || (this.offsetHeight != this.lastHeight)) {
            this.lastWidth = this.offsetWidth;
            this.lastHeight = this.offsetHeight;
            return true;
        }
        return false;
    },

    doLessWidth: function() {
        var scrollBarWidth = 17;
        var contentContainerElement = this.helper.GetContentContainerElement();
        if(!this.collapsed && !this.autoWidth && contentContainerElement) {
            contentContainerElement.dxWidth = contentContainerElement.style.width;
            contentContainerElement.style.width = ASPx.PxToInt(contentContainerElement.style.width) - scrollBarWidth + "px";
        }
    },
    restoreWidth: function() {
        var contentContainerElement = this.helper.GetContentContainerElement();
        if(!this.collapsed && !this.autoWidth && contentContainerElement) {
            contentContainerElement.style.width = contentContainerElement.dxWidth;
            contentContainerElement.dxWidth = undefined;
        }
    },

    // Client-side API
    GetSplitter: function () {
        return this.splitter;
    },
    GetParentPane: function () {
        return this.parent;
    },
    GetPrevPane: function () {
        return this.prevPane;
    },
    GetNextPane: function () {
        return this.nextPane;
    },
    IsFirstPane: function () {
        return (this.prevPane == null);
    },
    IsLastPane: function () {
        return (this.nextPane == null);
    },
    IsVertical: function () {
        return this.isVertical;
    },
    GetPaneCount: function () {
        return this.panes.length;
    },
    GetPane: function (index) {
        return (0 <= index && index < this.panes.length) ? this.panes[index] : null;
    },
    GetPaneByName: function (name) {
        for(var i = 0; i < this.panes.length; i++)
            if(this.panes[i].name == name) return this.panes[i];
        for(var i = 0; i < this.panes.length; i++) {
            var pane = this.panes[i].GetPaneByName(name);
            if(pane != null) return pane;
        }
        return null;
    },
    GetClientWidth: function () {
        var clientWidth = this.GetClientWidthInternal(true);
        if(!this.IsContentUrlPane()) {
            var contentContainer = this.helper.GetContentContainerElement();
            if((contentContainer.style.overflow == "auto" && contentContainer.scrollHeight > contentContainer.clientHeight)
                    || contentContainer.style.overflow == "scroll"
                    || contentContainer.style.overflowY == "scroll") {
                clientWidth = clientWidth - ASPx.GetVerticalScrollBarWidth();
            }
        }
        return clientWidth;
    },
    GetClientHeight: function () {
        return this.GetClientHeightInternal(true);
    },
    Collapse: function (maximizedPane) {
        if(!this.splitter.IsPrepared())
            return false;
        if(this.collapsed)
            return false;
        if(!ASPx.IsExists(maximizedPane) || !maximizedPane.isASPxClientSplitterPane)
            return false;

        return this.CollapseExpandCore(true, maximizedPane, "PaneCollapsed");
    },
    CollapseForward: function () {
        return this.Collapse(this.prevPane);
    },
    CollapseBackward: function () {
        return this.Collapse(this.nextPane);
    },
    Expand: function () {
        if(!this.splitter.IsPrepared())
            return false;
        if(!this.collapsed)
            return false;

        return this.CollapseExpandCore(false, null, "PaneExpanded");
    },
    CollapseExpandCore: function (collapsed, maximizedPane, eventName) {
        this.collapsed = collapsed;
        this.maximizedPane = maximizedPane;

        this.DropCachedButtonsVisible();
        if(this.nextPane != null)
            this.nextPane.DropCachedButtonsVisible();
        this.GetParentPane().UpdatePanes(!collapsed);
        this.GetParentPane().ForEach("AdjustControls");

        this.splitter.RaiseEvent(eventName, this);

        this.splitter.UpdateCookie();

        return true;
    },
    IsCollapsed: function () {
        return this.collapsed;
    },
    NeedResetSplitterSizeOnCollapsing: function (maximizedPane) {
        return maximizedPane.IsMaxSizeSpecified() && maximizedPane.GetParentPane().isRootPane;
    },
    NeedKeepOffsetSizeOnCollapsing: function () {
        return this.maximizedPane.IsMaxSizeSpecified() && !this.GetParentPane().isRootPane && this.GetParentPane().GetPaneCount() == 2;
    },
    IsContentUrlPane: function () {
        return this.isContentUrl;
    },
    GetContentUrl: function () {
        return this.isContentUrl
            ? this.iframeObj.GetContentUrl()
            : "";
    },
    SetContentUrl: function (url, preventBrowserCaching) {
        if(!this.isContentUrl)
            return;
        this.iframeObj.SetContentUrl(url, preventBrowserCaching);
    },
    RefreshContentUrl: function () {
        if(!this.isContentUrl)
            return;
        this.iframeObj.RefreshContentUrl();
    },
    GetContentIFrame: function () {
        return this.isContentUrl
            ? this.helper.GetContentContainerElement()
            : null;
    },
    CreateContentUrlIFrame: function () {
        if(!this.isContentUrl)
            return;

        var contentContainer = this.helper.GetContentContainerElement();
        contentContainer.parentNode.removeChild(contentContainer);

        this.iframeObj = new ASPx.IFrameHelper({
            id: contentContainer.id,
            name: this.iframe.name,
            title: this.iframe.title,
            scrolling: this.iframe.scrolling,
            src: this.iframe.src,
            onCreate: function (containerElement, element) {
                element.className = "dxsplIF";
                this.helper.GetPaneElement().appendChild(containerElement);
                this.helper.DropContentContainerElementFromCache();
                this.ApplyElementSizeCore();
                if(this.autoHeight && this.isVertical)
                    containerElement.style.height = "100%";
            }.aspxBind(this),
            onLoad: function () {
                this.splitter.RaiseEvent("PaneContentUrlLoaded", this);
            }.aspxBind(this)
        });
    },
    SetAllowResize: function (allowResize) {
        this.allowResize = allowResize;
        this.UpdateSeparatorStyle();
        if(!this.IsLastPane())
            this.nextPane.UpdateSeparatorStyle();
    },
    RaiseResizedEvent: function () {
        this.splitter.RaiseEvent("PaneResized", this);
    },
    GetElement: function () {
        return this.helper.GetPaneElement();
    },
    SetSize: function (size) {
        if(!this.splitter.IsPrepared())
            return;
        if(this.SetSizeCore(size)) {
            this.parent.ForEach("UpdateChildrenSize");
            this.splitter.UpdateAutoSizePanes();
            this.parent.ForEach("AdjustControls");

            this.splitter.UpdateCookie();
        }
    },
    GetSize: function () {
        return this.size + this.sizeType;
    },

    SetSizeCore: function (size) {
        if(!ASPx.IsExists(size))
            return false;
        if(this.IsAutoSize(!this.isVertical))
            return false;
        if(typeof (size) == "string") {
            var parsedSize = parseInt(size);
            if(isNaN(parsedSize))
                return false;

            this.size = parsedSize;
            this.sizeType = ASPx.IsPercentageSize(size) ? "%" : "px";
        }
        else if(typeof (size) == "number") {
            this.size = size;
            this.sizeType = "px";
        }
        else
            return false;
        this.isSizePx = this.sizeType == "px";
        return true;
    },
    GetScrollTop: function () {
        return this.scrollTop;
    },
    SetScrollTop: function (value) {
        this.helper.GetContentContainerElement().scrollTop = value;
    },
    GetScrollLeft: function () {
        return this.scrollLeft;
    },
    SetScrollLeft: function (value) {
        this.helper.GetContentContainerElement().scrollLeft = value;
    }
});

ASPxClientSplitter.Instances = {
    items: {},
    Add: function (instance) {
        this.items[instance.name] = instance;
        if(instance.hasAutoSizePane)
            ASPxClientSplitter.AutoSizePanesUpdater.Start();
    },
    Get: function (name) {
        var instance = this.items[name];
        if(instance) {
            if(instance.GetMainElement())
                return instance;
            delete this.items[name];
        }
        return null;
    },
    Each: function (cb) {
        var hasInstances = false;
        for(var name in this.items) {
            var instance = this.Get(name);
            if(instance) {
                hasInstances = true;
                cb.call(instance);
            }
        }
        return hasInstances;
    }
};
ASPxClientSplitter.AutoSizePanesUpdater = {
    timeoutId: -1,
    Start: function () {
        var updater = ASPxClientSplitter.AutoSizePanesUpdater;
        if(updater.timeoutId > -1)
            return;
        updater.timeoutId = window.setTimeout(updater.OnTimeout, 300);
    },
    Stop: function () {
        var updater = ASPxClientSplitter.AutoSizePanesUpdater;
        updater.timeoutId = ASPx.Timer.ClearTimer(updater.timeoutId);
    },
    OnTimeout: function () {
        var updater = ASPxClientSplitter.AutoSizePanesUpdater;
        updater.Stop();
        if(ASPxClientSplitter.Instances.Each(function () {
            this.UpdateAutoSizePanes();
            if(!this.isInLiveResizing && this.hasAutoSizePane && this.IsDocumentWidthChanged())
                this.UpdateControlSizes();
        }))
            updater.Start();
    }
};
ASPxClientSplitter.timerInterval = 0;
ASPxClientSplitter.GetRegEx = function (idPostfix) {
    if(!this.regExs)
        this.regExs = {};
    if(!this.regExs[idPostfix])
        this.regExs[idPostfix] = "_\\d+(" + ASPx.ItemIndexSeparator + "\\d+)*_" + idPostfix + "$";
    return this.regExs[idPostfix];
};

ASPxClientSplitter.SaveCurrentPos = function (evt) {
    evt = ASPx.Evt.GetEvent(evt);
    ASPxClientSplitter.CurrentXPos = ASPx.Evt.GetEventX(evt);
    ASPxClientSplitter.CurrentYPos = ASPx.Evt.GetEventY(evt);
};
ASPxClientSplitter.FindParentCell = function (element) {
    if(element.tagName != "TD")
        element = ASPx.GetParentByTagName(element, "TD");
    return element;
};
ASPxClientSplitter.FindSplitterInfo = function (evt, regex, suffixLength) {
    var element = ASPxClientSplitter.FindParentCell(ASPx.Evt.GetEventSource(evt));
    if(element) {
        var matchResult = element.id.match(regex);

        if(matchResult) {
            var name = element.id.substring(0, matchResult.index);
            var splitter = ASPxClientSplitter.Instances.Get(name);
            if(splitter != null) {
                var panePath = element.id.substring(matchResult.index + 1, element.id.length - suffixLength);
                return { "splitter": splitter, "panePath": panePath };
            }
        }
    }
    return null;
};
ASPxClientSplitter.OnMouseClick = function (evt) {
    var info = ASPxClientSplitter.FindSplitterInfo(evt, ASPxClientSplitter.GetRegEx("S_CF"), 5);
    if(info) {
        if(info.splitter.enabled)
            info.splitter.OnCollapseButtonClick(info.panePath, true);
    }
    else {
        info = ASPxClientSplitter.FindSplitterInfo(evt, ASPxClientSplitter.GetRegEx("S_CB"), 5);
        if(info && info.splitter.enabled)
            info.splitter.OnCollapseButtonClick(info.panePath, false);
    }
};
ASPxClientSplitter.OnMouseDown = function (evt) {
    var info = ASPxClientSplitter.FindSplitterInfo(evt, ASPxClientSplitter.GetRegEx("S"), 2);
    if(!info)
        info = ASPxClientSplitter.FindSplitterInfo(evt, ASPxClientSplitter.GetRegEx("S_CS"), 5);
    if(info && info.splitter) {
        ASPxClientSplitter.current = info.splitter;
        ASPxClientSplitter.SaveCurrentPos(evt);
        ASPx.Selection.SetElementSelectionEnabled(ASPxClientSplitter.current.GetMainElement(), false);
        ASPxClientSplitter.isInMove = info.splitter.OnSeparatorMouseDown(info.panePath);
        ASPx.Evt.PreventEvent(evt);
    }
};
ASPxClientSplitter.OnMouseUp = function () {
    if(ASPxClientSplitter.isInMove) {
        ASPxClientSplitter.isInMove = false;
        ASPx.Selection.SetElementSelectionEnabled(ASPxClientSplitter.current.GetMainElement(), true);
        ASPxClientSplitter.current.OnSeparatorMouseUp();
    }
};
ASPxClientSplitter.mouseMoveTimeoutId = -1;
ASPxClientSplitter.SuspendedMouseMove = function () {
    if(ASPxClientSplitter.isInMove)
        ASPxClientSplitter.current.OnMouseMove();
    ASPxClientSplitter.mouseMoveTimeoutId = ASPx.Timer.ClearTimer(ASPxClientSplitter.mouseMoveTimeoutId);
};
ASPxClientSplitter.OnMouseMove = function (evt) {
    if(ASPx.Browser.WebKitTouchUI && ASPx.TouchUIHelper.isGesture)
        return;
    if(!ASPxClientSplitter.isInMove)
        return;
    if(ASPx.Browser.IE && !ASPx.Evt.IsLeftButtonPressed(evt)) {
        ASPxClientSplitter.OnMouseUp(evt);
        return;
    }
    ASPxClientSplitter.SaveCurrentPos(evt);
    if(ASPxClientSplitter.mouseMoveTimeoutId == -1)
        ASPxClientSplitter.mouseMoveTimeoutId = window.setTimeout(ASPxClientSplitter.SuspendedMouseMove, ASPxClientSplitter.timerInterval);
    if(ASPx.Browser.WebKitTouchUI)
        evt.preventDefault();
};
ASPx.Evt.AttachEventToDocument("click", ASPxClientSplitter.OnMouseClick);
ASPx.Evt.AttachEventToDocument(ASPx.TouchUIHelper.touchMouseDownEventName, ASPxClientSplitter.OnMouseDown);
ASPx.Evt.AttachEventToDocument(ASPx.TouchUIHelper.touchMouseUpEventName, ASPxClientSplitter.OnMouseUp);
ASPx.Evt.AttachEventToDocument(ASPx.TouchUIHelper.touchMouseMoveEventName, ASPxClientSplitter.OnMouseMove);
var ASPxClientSplitterPaneEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function (pane) {
        this.constructor.prototype.constructor.call(this, pane);
        this.pane = pane;
    }
});
var ASPxClientSplitterPaneCancelEventArgs = ASPx.CreateClass(ASPxClientSplitterPaneEventArgs, {
    constructor: function (pane) {
        this.constructor.prototype.constructor.call(this, pane);
        this.cancel = false;
    }
});

window.ASPxClientSplitter = ASPxClientSplitter;
window.ASPxClientSplitterPane = ASPxClientSplitterPane;
window.ASPxClientSplitterPaneEventArgs = ASPxClientSplitterPaneEventArgs;
window.ASPxClientSplitterPaneCancelEventArgs = ASPxClientSplitterPaneCancelEventArgs;
})();