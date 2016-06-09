(function() {

    ASPx.HtmlEditorClasses.Managers.LayoutManager = ASPx.CreateClass(null, {
        constructor: function(htmlEditor) {
            this.htmlEditor = htmlEditor;
             // fullscreen
            this.isFullscreenMode = false;
            this.fullscreenTempVars = {};
            this.layoutCalculator = ASPx.GetHtmlEditorLayoutCalculator();
            this.isWidthDefinedInPercent = false;
            this.initialMainElementWidth = -1;
            // resizing
            this.minWidth = 0;
            this.minHeight = 0;
            this.maxWidth = 0;
            this.maxHeight = 0;
            this.currentHeight = 0;
            this.currentWidth = 0;
            this.resizeTempVars = {};

            this.inAdjusting = false;
            this.eventManager = null;
        },
        initializeEventManager: function() {
            this.eventManager = new ASPx.HtmlEditorClasses.Managers.LayoutEventManager(this);
        },
        isInFullscreen: function() {
            return this.isFullscreenMode;
        },
        getMainElement: function() {
            return this.htmlEditor.GetMainElement();
        },
        isInAdjusting: function() {
            return this.inAdjusting;
        },
        updateLayout: function(activeView, isInitializing) {
            if(this.getMainElement().offsetWidth == 0)
                return;
            this.inAdjusting = true;
            isInitializing = !!isInitializing;
            if(this.isFullscreenMode)
                this.adjustSizeInFullscreen();
            this.layoutCalculator.updateLayout(this.htmlEditor, activeView, isInitializing);
            this.inAdjusting = false;
        },
        adjustSizeInPercent: function() {
            this.inAdjusting = true;
            var mainElement = this.getMainElement();
            var mainCell = this.htmlEditor.getMainCell();

            if(this.currentHeight)
                this.percentSizeDiv.style.height = this.currentHeight + "px";

            this.removeChildrenFromFlow(mainCell, true); //Q353829
            var newOffsetWidth = this.percentSizeDiv.offsetWidth;
            this.removeChildrenFromFlow(mainCell, false);
        
            this.percentSizeDiv.style.height = "0px";

            if(this.adjustResizeTimerID){
                ASPx.Timer.ClearTimer(this.adjustResizeTimerID);
                delete this.adjustResizeTimerID;
            }
        
            if(this.oldOffsetWidth != newOffsetWidth || newOffsetWidth != mainElement.offsetWidth) {
                if(!this.percentResizeStarted || this.resizeTempVars.startMainWidth === undefined) {
                    this.saveStartSize();
                    this.percentResizeStarted = true;
                }
                this.setDeltaWidthInternal(newOffsetWidth - this.resizeTempVars.startMainWidth, true);
            }
            this.inAdjusting = false;
            this.oldOffsetWidth = newOffsetWidth;
        },
        removeChildrenFromFlow: function(parent, remove) {
            var elements = ASPx.GetChildElementNodes(parent);
            for(var i = 0, l = elements.length; i < l; i++) {
                if(ASPx.Browser.IE && ASPx.Browser.MajorVersion >= 9)
                    elements[i].style.position = remove ? "absolute" : "";
                else
                    ASPx.SetElementDisplay(elements[i], !remove);
            }
        },
        setDeltaSize: function() {
            this.setDeltaWidthInternal(this.rtl 
                ? this.resizeTempVars.startSizeGripXPos - this.resizeTempVars.currentSizeGripXPos 
                : this.resizeTempVars.currentSizeGripXPos - this.resizeTempVars.startSizeGripXPos
            );
            this.setDeltaHeightInternal(this.resizeTempVars.currentSizeGripYPos - this.resizeTempVars.startSizeGripYPos);
        },
        setDeltaWidthInternal: function(delta, isCheckRanges) {
            isCheckRanges = ASPx.IsExists(isCheckRanges) ? isCheckRanges : true;
            var lastMainElementHeight = this.getMainElement().offsetHeight;
            if(isCheckRanges)
                var width = this.getSizeInRanges(this.minWidth, this.maxWidth, this.resizeTempVars.startMainWidth, this.resizeTempVars.startResizeElWidth, delta);
            else
                var width = this.resizeTempVars.startResizeElWidth + delta;
        
            this.getMainElement().style.width = "";
            this.getCurrentEditElement().style.width = "100%";
            this.htmlEditor.getMainCell().firstChild.style.width = Math.max(width, 1) + "px";

            this.adjustInnerControls();
            var mainElementDeltaHeight = this.getMainElement().offsetHeight - lastMainElementHeight;
            if(mainElementDeltaHeight != 0){
                this.setDeltaHeightInternal(-mainElementDeltaHeight);
                this.resizeTempVars.startResizeElHeight -= mainElementDeltaHeight;
            }
        },
        setDeltaHeightInternal: function(delta, isCheckRanges) {
            isCheckRanges = ASPx.IsExists(isCheckRanges) ? isCheckRanges : true;
            if(isCheckRanges)
                var height = this.getSizeInRanges(this.minHeight, this.maxHeight, this.resizeTempVars.startMainHeight, this.resizeTempVars.startResizeElHeight, delta);
            else
                var height = this.resizeTempVars.startResizeElHeight + delta;
            this.getMainElement().style.height = "";
            this.htmlEditor.getEditAreaCell().style.height = "";
            this.setEditElementHeight(this.getCurrentEditElement(), Math.max(height, 5));
            if(!this.resizeTempVars.isInResize)
                this.adjustInnerControls();
        },
        adjustInnerControls: function() {
            if(this.htmlEditor.isHtmlView() && !this.htmlEditor.isSimpleHtmlEditingMode())
                this.htmlEditor.getHtmlViewWrapper().adjustControl();
            var tabControl = this.htmlEditor.statusBarManager.getTabControl();
            if(tabControl && !tabControl.GetMainElement())
                tabControl = null;

            var ribbon = this.htmlEditor.barDockManager.getRibbon();
            if(ribbon && !ribbon.GetMainElement())
                ribbon = null;

            if(tabControl && ribbon) {
                tabControl.CollapseControl();
                ribbon.AdjustControlCore();
                tabControl.ExpandControl();
                tabControl.AdjustControlCore();
                var ribbonMarkerSize = ribbon.GetControlPercentMarkerSize(true);
                if(ribbon.adjustedSizes.width < ribbonMarkerSize.width)
                    ribbon.AdjustControlCore();
                ribbon.ResetControlPercentMarkerSize();
            }
            else if(tabControl)
                tabControl.AdjustControlCore();
            else if(ribbon)
                ribbon.AdjustControlCore();
            
            if(this.htmlEditor.tagInspector)
                this.htmlEditor.tagInspector.adjustControl();
        },
        getSizeInRanges: function(minSize, maxSize, startMainElementSize, startResizeElementSize, delta) {
            var size = startResizeElementSize + delta;
            if(maxSize > 0 && delta > 0 && startMainElementSize + delta > maxSize){
                size = (maxSize - startMainElementSize + startResizeElementSize);
            }
            if(minSize > 0 && delta < 0 && startMainElementSize + delta < minSize){
                size = (minSize - startMainElementSize + startResizeElementSize);
            }
            return size;
        },
        saveCurrentSizeGripPosition: function(evt, isSaveStart) {
            this.resizeTempVars.currentSizeGripXPos = ASPx.Evt.GetEventX(evt);
            this.resizeTempVars.currentSizeGripYPos = ASPx.Evt.GetEventY(evt);
        
            if(isSaveStart) {
                this.resizeTempVars.startSizeGripXPos = this.resizeTempVars.currentSizeGripXPos;
                this.resizeTempVars.startSizeGripYPos = this.resizeTempVars.currentSizeGripYPos;
            }
        },
        saveStartSize: function() {
            this.resizeTempVars.startResizeElWidth = ASPx.GetClearClientWidth(this.htmlEditor.getMainCell().firstChild);
            var resizeElStyleHeight = ASPx.PxToInt(this.getCurrentEditElement().style.height);
            var resizeElClientHeight = this.getCurrentEditElement().clientHeight;
            this.resizeTempVars.startResizeElHeight = (resizeElStyleHeight < 1 && resizeElClientHeight >= 0) ? resizeElClientHeight : resizeElStyleHeight;
            this.resizeTempVars.startMainWidth = this.getMainElement().offsetWidth;
            this.resizeTempVars.startMainHeight = this.getMainElement().offsetHeight;
        },
        setResizingPanelVisibility: function(visible) {
            if(!this.resizingPanel) {
                var html = "<div style='overflow:hidden; position: fixed; ";
                if(ASPx.Browser.IE ){
                    html += "background-color: White; "
                    html += ASPx.Browser.MajorVersion < 10 ? "filter: alpha(opacity=0.1);" : "opacity: 0.01";
                }
                html += "'></div>";
                this.resizingPanel = ASPx.CreateHtmlElementFromString(html);
                this.htmlEditor.getStatusBarCell().appendChild(this.resizingPanel);
            }
            if(visible) {
                ASPx.SetAbsoluteX(this.resizingPanel, 0);
                ASPx.SetAbsoluteY(this.resizingPanel, 0);
                ASPx.SetStyles(this.resizingPanel, {
                    width: ASPx.GetDocumentWidth(),
                    height: ASPx.GetDocumentHeight()
                });
            }
            ASPx.SetElementDisplay(this.resizingPanel, visible);
        },
        setEditElementHeight: function(element, value) {
            element.style.height = value + "px";
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion >= 11 && (this.htmlEditor.isDesignView() || this.htmlEditor.isPreview()))
                ASPx.SetOffsetHeight(this.htmlEditor.core.getActiveWrapper().getWindow().frameElement, value);
        },
        setResizeRanges: function(minWidth, minHeight, maxWidth, maxHeight) {
            this.minWidth = minWidth;
            this.minHeight = minHeight;
            this.maxWidth = maxWidth;
            this.maxHeight = maxHeight;
        },
        setHeight: function(height) {
            if(this.isInAdjusting()) return;
            if(this.isInFullscreen())
                this.fullscreenTempVars.savedCurrentHeight = height;
            else {
                this.setHeightInternal(height, true);
                this.resizeTempVars = {};
            }
        },
        setWidth: function(width) {
            this.isWidthDefinedInPercent = false;
            if(this.isInAdjusting()) return;
            if(this.isInFullscreen())
                this.fullscreenTempVars.savedCurrentWidth = width;
            else {
                this.setWidthInternal(width, true);
                this.resizeTempVars = {};
            }
        },
        setHeightInternal: function(height, isCheckRanges, isSaveSize) {
            var currentEditElementHeight = this.htmlEditor.enableTagInspector ? 25 + this.htmlEditor.getTagInspectorWrapperElement().offsetHeight : 25;
            if(this.getCurrentEditElement().offsetHeight < currentEditElementHeight)
                this.setEditElementHeight(this.getCurrentEditElement(), currentEditElementHeight);
            isSaveSize = ASPx.IsExists(isSaveSize) ? isSaveSize : true;
            this.saveStartSize();
            if(this.resizeTempVars.startMainHeight == 0 && !this.postponedHeight)
                this.postponedHeight = height;
            this.setDeltaHeightInternal(height - this.resizeTempVars.startMainHeight, isCheckRanges);
            if(isSaveSize)
                this.saveCurrentSize(false, true, true);
        },
        setWidthInternal: function(width, isCheckRanges, isSaveSize) {
            isSaveSize = ASPx.IsExists(isSaveSize) ? isSaveSize : true;
            this.saveStartSize();
            if(this.resizeTempVars.startMainWidth == 0 && !this.postponedWidth)
                this.postponedWidth = width;
            this.setDeltaWidthInternal(width -  this.resizeTempVars.startMainWidth, isCheckRanges);
            if(isSaveSize)
                this.saveCurrentSize(true, false, true);
        },
        restoreHeight: function(value) {
            if(ASPx.IsExists(value))
                this.currentHeight = value;
            this.setHeightInternal(this.currentHeight);
        },
        restoreWidth: function(value) {
            if(ASPx.IsExists(value))
                this.currentWidth = value;
            this.setWidthInternal(this.currentWidth);
        },
        saveCurrentSize: function(saveWidth, saveHeight, updateClientState, saveToCookie) {
            var mainElement = this.getMainElement();
            if(saveWidth){
                var currentWidth = mainElement.offsetWidth;
                if(currentWidth > 0){
                    this.currentWidth = currentWidth;
                    if(updateClientState){
                        this.htmlEditor.SetClientStateFieldValue("CurrentWidth", this.isWidthDefinedInPercent 
                            ? Math.round(ASPx.PercentageToFloat(this.initialMainElementWidth)*100) 
                            : this.currentWidth, saveToCookie);
                        this.htmlEditor.SetClientStateFieldValue("IsPercentWidth", this.isWidthDefinedInPercent ? 1 : 0, saveToCookie);
                    }
                }
            }
            if(saveHeight){
                var currentHeight = mainElement.offsetHeight;
                if(currentHeight > 0){
                    this.currentHeight = currentHeight;
                    if(updateClientState){
                        this.htmlEditor.SetClientStateFieldValue("CurrentHeight", this.currentHeight, saveToCookie);
                    }
                }
            }
        },
        setFullscreenMode: function(enable) {
            enable = ASPx.IsExists(enable) ? enable : !this.isFullscreenMode;
            var sizeGrip = this.htmlEditor.getSizeGrip();
            this.isFullscreenMode = enable;
            var mainElement = this.getMainElement();
            if(enable) {
                //ASPx.SetStyles(element, { borderTopWidth: 0}); TODO
                ASPx.Attr.ChangeStyleAttribute(mainElement, ASPx.Browser.IE ? "borderTopWidth" : "border-top-width", "0px");
                ASPx.Attr.ChangeStyleAttribute(mainElement, ASPx.Browser.IE ? "borderLeftWidth" : "border-left-width", "0px");
                ASPx.Attr.ChangeStyleAttribute(mainElement, ASPx.Browser.IE ? "borderRightWidth" : "border-right-width", "0px");
                ASPx.Attr.ChangeStyleAttribute(mainElement, ASPx.Browser.IE ? "borderBottomWidth" : "border-bottom-width", "0px");
                this.fullscreenTempVars.savedBodyScrollTop = ASPx.GetDocumentScrollTop();
                this.fullscreenTempVars.savedBodyScrollLeft = ASPx.GetDocumentScrollLeft();
                this.showPlaceholderDiv(true);
                ASPx.Attr.ChangeStyleAttribute(mainElement, "position", "fixed");
                ASPx.Attr.ChangeStyleAttribute(mainElement, "top", "0px");
                ASPx.Attr.ChangeStyleAttribute(mainElement, "left", "0px");
            
                ASPx.Attr.ChangeStyleAttribute(mainElement, ASPx.Browser.IE ? "zIndex" : "z-index", 10001);
                this.hideBodyScroll();
            
                this.fullscreenTempVars.savedBodyMargin = document.body.style.margin;
                document.body.style.margin = 0;
            
                if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 8){
                    document.documentElement.scrollTop = 0;
                    document.documentElement.scrollLeft = 0;
                }
            
                this.fullscreenTempVars.savedCurrentWidth = this.currentWidth;
                this.fullscreenTempVars.savedCurrentHeight = this.currentHeight;
            
                if(sizeGrip)
                    sizeGrip.style.visibility = "hidden";
                this.showCoverDiv(true);
                this.htmlEditor.barDockManager.setExternalRibbonPositionOnPageTop();
                
                this.adjustSizeInFullscreen();
            }
            else {
                this.showCoverDiv(false);
                ASPx.Attr.RestoreStyleAttribute(mainElement, "left");
                ASPx.Attr.RestoreStyleAttribute(mainElement, "top");
                this.restoreBodyScroll();
                ASPx.Attr.RestoreStyleAttribute(mainElement, ASPx.Browser.IE ? "zIndex" : "z-index");
                document.body.style.margin = this.fullscreenTempVars.savedBodyMargin;
                ASPx.Attr.RestoreStyleAttribute(mainElement, "position");
            
                ASPx.Attr.RestoreStyleAttribute(mainElement, ASPx.Browser.IE ? "borderTopWidth" : "border-top-width");
                ASPx.Attr.RestoreStyleAttribute(mainElement, ASPx.Browser.IE ? "borderLeftWidth" : "border-left-width");
                ASPx.Attr.RestoreStyleAttribute(mainElement, ASPx.Browser.IE ? "borderRightWidth" : "border-right-width");
                ASPx.Attr.RestoreStyleAttribute(mainElement, ASPx.Browser.IE ? "borderBottomWidth" : "border-bottom-width");
            
                this.setHeightInternal(this.fullscreenTempVars.savedCurrentHeight, false, false);
                this.setWidthInternal(this.fullscreenTempVars.savedCurrentWidth, false, false);
                this.currentWidth = this.fullscreenTempVars.savedCurrentWidth;
                this.currentHeight = this.fullscreenTempVars.savedCurrentHeight;
                document.body.style.margin = this.fullscreenTempVars.savedBodyMargin;
            
                document.documentElement.scrollTop = this.fullscreenTempVars.savedBodyScrollTop;
                document.documentElement.scrollLeft = this.fullscreenTempVars.savedBodyScrollTop;
                this.showPlaceholderDiv(false);
                this.htmlEditor.barDockManager.restoreExternalRibbonPositionOnPage();
                if(sizeGrip)
                    sizeGrip.style.visibility = "";
            }
            this.htmlEditor.removeFocus(true);
            this.htmlEditor.Focus();
            return enable;
        },
        hideBodyScroll: function() {
            ASPx.Attr.ChangeStyleAttribute(document.documentElement, "position", "static");
            ASPx.Attr.ChangeStyleAttribute(document.documentElement, "overflow", "hidden");
        },
        restoreBodyScroll: function() {
            if (!ASPx.Dialog.GetLastDialog(this.htmlEditor)) {
                ASPx.Attr.RestoreStyleAttribute(document.documentElement, "overflow");
                ASPx.Attr.RestoreStyleAttribute(document.documentElement, "position");
            }
        },
        adjustSizeInFullscreen: function() {
            if(!this.isInFullscreen())
                return false;
            this.setWidthInternal(ASPx.GetDocumentClientWidth(), false, false);
            var height = 0;
            var barDockManager = this.htmlEditor.barDockManager;
            var ribbon = barDockManager.getRibbon(true);
            if(ribbon && this.isFullscreenMode && this.htmlEditor.isDesignView())
                height = ribbon.GetMainElement().offsetHeight;
            this.setHeightInternal(ASPx.GetDocumentClientHeight() - height, false, false);
            return true;
        },
        showCoverDiv: function(isShow) {
            if(!this.coverDiv) {
                this.coverDiv = ASPx.CreateHtmlElementFromString("<div class='dxhe-coverDiv'></div>");
                this.getMainElement().parentNode.insertBefore(this.coverDiv, this.getMainElement());
            }
            ASPx.SetElementDisplay(this.coverDiv, isShow);
        },
        showPlaceholderDiv: function(isShow) {
            if(!this.placeholderDiv) {
                this.placeholderDiv = ASPx.CreateHtmlElementFromString("<div style='background-color: White; display:none;'></div>");
                ASPx.InsertElementAfter(this.placeholderDiv, this.getMainElement());
                ASPx.SetStyles(this.placeholderDiv, {
                    width: this.htmlEditor.GetWidth(),
                    height: this.htmlEditor.GetHeight()
                });
            }
            ASPx.SetElementDisplay(this.placeholderDiv, isShow);
        },
        getCurrentEditElement: function() {
            if(this.htmlEditor.isDesignView())
                return this.htmlEditor.getDesignViewCell();
            if(this.htmlEditor.isHtmlView())
                return this.getHtmlViewEditElement();
            return this.htmlEditor.getPreviewCell();
        },
        collapseViewAreas: function() {
            var designViewCell = this.htmlEditor.getDesignViewCell();
            var previewCell = this.htmlEditor.getPreviewCell();
        
            if(designViewCell)
                this.setEditElementHeight(designViewCell, 0);
            if(this.htmlEditor.isHtmlViewAllowed())
                this.setEditElementHeight(this.getHtmlViewEditElement(), 0);
            if(previewCell)
                this.setEditElementHeight(previewCell, 0);
            if(ASPx.Browser.Edge)
                this.getMainElement().offsetHeight;
        },
        showHideViewAreas: function(showDesignView, showHtmlView, showPreview) {
            var designViewTable = this.htmlEditor.getDesignViewTable();
            var htmlViewTable = this.htmlEditor.getHtmlViewTable();
            var previewTable = this.htmlEditor.getPreviewTable();
        
            if(designViewTable)
                ASPx.SetElementDisplay(designViewTable, showDesignView);
            if(htmlViewTable)
                ASPx.SetElementDisplay(htmlViewTable, showHtmlView);
            if(previewTable)
                ASPx.SetElementDisplay(previewTable, showPreview);
        },
        getHtmlViewEditElement: function() {
            var wrapper = this.htmlEditor.getHtmlViewWrapper();
            return this.htmlEditor.isSimpleHtmlEditingMode() ? wrapper.getInputElement() : wrapper.getMainElement();
        },
        hideViewAreas: function() {
            this.showHideViewAreas(false, false, false);
        },
        showHideViewAreasDependingOnActiveView: function(activeView) {
            this.showHideViewAreas(this.htmlEditor.isDesignView(activeView), this.htmlEditor.isHtmlView(activeView), this.htmlEditor.isPreview(activeView));
        },
        correctSizeOnSwitchToView: function(view) {
            ASPx.SetStyles(this.htmlEditor.getEditAreaCell(), { height: "100%", width: "100%" });
            this.restoreWidth();
            this.restoreHeight();
        },
        setPostponedSize: function() {
            this.setPostponedWidth();
            this.setPostponedHeight();
        },
        setPostponedWidth: function() {
            if(this.postponedWidth) {
                this.setWidth(this.postponedWidth);
                this.postponedWidth = undefined;
            }
        },
        setPostponedHeight: function() {
            if(this.postponedHeight) {
                this.setHeight(this.postponedHeight);
                this.postponedHeight = undefined;
            }
        }
    });
})();