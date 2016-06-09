/// <reference path="ReportDocumentMap.js"/>
/// <reference path="ReportParametersPanel.js"/>
/// <reference path="ReportToolbar.js"/>
/// <reference path="ReportViewer.js"/>

(function(window) {
    var ASPxClientDocumentViewer = ASPx.CreateClass(ASPxClientControl, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.ToolbarItemValueChanged = new ASPxClientEvent();
            this.ToolbarItemClick = new ASPxClientEvent();
            this.PageLoad = new ASPxClientEvent();

            this.forceDocumentMapCollapsed = false;
            this.ignoreItems = {};
            this.enabledItems = [];
            this.exportToPrefix = 'dxxrdvExportTo';
            this.searchItemName = 'dxxrdvBtnSearch';
            this.saveToDiskItemName = 'dxxrdvBtnSave';
            this.saveToWindowItemName = 'dxxrdvBtnSaveWindow';
            this.parametersPanelToggleCommandName = 'dxxrdvBtnParameters';
            this.documentMapToggleCommandName = 'dxxrdvBtnDocumentMap';
        },
        InlineInitialize: function() {
            ASPxClientControl.prototype.InlineInitialize.call(this);

            var controlCollection = ASPx.GetControlCollection();

            this.splitter = controlCollection.Get(this.name + '_Splitter');
            var splitterName = this.splitter.name;

            this.sideCenterPane = this.splitter.GetPaneByName('SideCenterPane');
            this.parametersPanelPane = this.splitter.GetPaneByName('ParametersPanelPane');
            this.documentMapPane = this.splitter.GetPaneByName('DocumentMapPane');
            this.viewerPane = this.splitter.GetPaneByName('ViewerPane');
            this.toolbarPane = this.splitter.GetPaneByName('ToolbarPane');

            this.viewer = controlCollection.Get(splitterName + '_Viewer');

            var toolbar = controlCollection.Get(splitterName + '_Toolbar');
            if(toolbar && toolbar.isValid()) {
                this.toolbar = toolbar;
            }
            this.parametersPanel = ASPxClientDocumentViewer.findValidControl(splitterName + '_ParametersPanel');
            if(this.externalRibbonID) {
                this.ribbonToolbar = ASPxClientDocumentViewer.findValidControl(this.externalRibbonID);
            } else {
                this.ribbonToolbar = ASPxClientDocumentViewer.findValidControl(splitterName + '_RibbonToolbar');
            }
            this.initializeToogleButtons();
            this.subscribeSplitterCollapsing();
        },
        Initialize: function() {
            ASPxClientControl.prototype.Initialize.call(this);
            var self = this;

            if(this.viewer) {
                this.viewer.createCallbackCoreEvent.AddHandler(function(s, e) { self.executeCallback(e.callbackString); });
                this.viewer.PageLoad.AddHandler(function(s, e) { self.PageLoad.FireEvent(self, e); });
                this.lastSaveFormat = this.viewer.DefaultSaveFormat;
            }
            if(this.parametersPanel) {
                this.parametersPanel.cascadeLookupValuesRequest.AddHandler(function(_, e) { self.execCascadeLookupCallback(e); });
            }
            if(this.ribbonToolbar) {
                this.ribbonToolbar.CommandExecuted.AddHandler(function(s, a) { self.handleButton(a.item, a.parameter); });
                this.viewer.PageLoad.AddHandler(function(s, a) { self.pageLoadRibbonEventHandler(s, a); });
                this.viewer.BeginCallback.AddHandler(function() { self.beginCallbackRibbonEventHanler(); });
                this.viewer.EndCallback.AddHandler(function() { self.endCallbackRibbonEventHanler(); });
                this.prepareRibbonItems();
            }
            this.assignDocumentMap();

            if(this.documentMapPane && this.documentMapPane.autoHeight) {
                this.splitter.PaneResized.AddHandler(function(s, e) {
                    if(e.pane === self.documentMapPane) {
                        self.correctDocumentMapHeight();
                    }
                });
            }
            if(!this.parametersPanel && this.documentMap) {
                this.stretchDocumentMap();
            }
        },
        GetSplitter: function() {
            return this.splitter;
        },
        GetViewer: function() {
            return this.viewer;
        },
        GetToolbar: function() {
            return this.toolbar;
        },
        GetRibbonToolbar: function() {
            return this.ribbonToolbar;
        },
        GetParametersPanel: function() {
            return this.parametersPanel;
        },
        GetDocumentMap: function() {
            return this.documentMap;
        },
        GotoBookmark: function(pageIndex, bookmarkPath) {
            return this.viewer.GotoBookmark(pageIndex, bookmarkPath);
        },
        Refresh: function() {
            return this.viewer.Refresh();
        },
        AdjustControlCore: function () {
            if(this.splitter) {
                this.splitter.AdjustControlCore();
                this.splitter.UpdateAdjustedSizes();
            }
        },
        Print: function(pageIndex) {
            this.viewer.Print(pageIndex);
        },
        GotoPage: function(pageIndex) {
            this.viewer.GotoPage(pageIndex);
        },
        Search: function() {
            this.viewer.Search();
        },
        IsSearchAllowed: function() {
            return this.viewer.IsSearchAllowed();
        },
        SaveToWindow: function(format) {
            this.viewer.SaveToWindow(format);
        },
        SaveToDisk: function(format) {
            this.viewer.SaveToDisk(format);
        },
        SetWidth: function(width) {
            this.splitter.SetWidth(width);
        },
        SetHeight: function(height) {
            this.splitter.SetHeight(height);
        },
        SetVisible: function(value) {
            this.splitter.SetVisible(value);
        },
        GetMainElement: function() {
            return this.splitter
                ? this.splitter.GetMainElement()
                : ASPxClientControl.prototype.GetMainElement.call(this);
        },
        initializeToogleButtons: function() {
            if(this.ribbonToolbar) {
                this.parametersPanelToggle = this.ribbonToolbar.GetItemByName(this.parametersPanelToggleCommandName);
                this.documentMapToggle = this.ribbonToolbar.GetItemByName(this.documentMapToggleCommandName);
                this.setToggleValues(this.parametersPanelToggle, (this.parametersPanelPane && !this.parametersPanelPane.IsCollapsed()) != null);
                this.setToggleValues(this.documentMapToggle, null, false);

                if(!this.sideCenterPane) {
                    this.setToggleValues(this.documentMapToggle, false, false);
                    this.setToggleValues(this.parametersPanelToggle, false, false);
                } else if(!this.documentMapPane) {
                    this.setToggleValues(this.documentMapToggle, false, false);
                } else if(!this.parametersPanelPane) {
                    this.setToggleValues(this.parametersPanelToggle, false, false);
                }
            }
        },
        assignDocumentMap: function() {
            var documentMap = ASPx.GetControlCollection().Get(this.splitter.name + '_DocumentMap');
            if(documentMap && documentMap.isValid()) {
                this.documentMap = documentMap;
            }
            this.refreshDocumentMapPaneLayout();
        },
        execCascadeLookupCallback: function(pathsWithCallbackArguments) {
            var json = ASPx.Json.ToJson(pathsWithCallbackArguments);
            var encodedJson = encodeURIComponent(json);
            var callbackString = 'cascadeLookups=' + json;
            this.executeCallback(callbackString);
        },
        getCommandName: function(callbackString) {
            var parts = callbackString.split('=');
            return parts.length > 0
                ? parts[0]
                : null;
        },
        executeCallback: function(callbackString) {
            var command = this.getCommandName(callbackString);
            if(command === 'submitParameters') {
                this.clearRemoteDocumentInformation();
                if(this.documentMap) {
                    this.documentMap.onBeginCallback();
                }
            }
            this.CreateCallback(callbackString);
        },
        clearRemoteDocumentInformation: function() {
            var value = this.viewer.stateObject.remote;
            if(value) {
                value = value.split(';', 1);
            }
            this.viewer.stateObject.remote = value;
        },
        OnCallback: function(result) {
            if (result.viewer) {
                this.viewer.DoBeginCallback();
                this.viewer.OnCallback(result.viewer);
                this.viewer.DoEndCallback();
            }
            this.documentMapExisted = this.documentMap && this.documentMap.hasTreeView();
            if(result.documentMap && this.documentMap) {
                this.documentMap.onEndCallback();
                this.documentMapPane.helper.GetContentContainerElement().innerHTML = result.documentMap;
                var controlCollection = ASPx.GetControlCollection();
                var documentMapInitialized;
                documentMapInitialized = function() {
                    controlCollection.ControlsInitialized.RemoveHandler(documentMapInitialized);
                    this.assignDocumentMap();
                }.aspxBind(this);
                controlCollection.ControlsInitialized.AddHandler(documentMapInitialized);
            }
            if(result.cascadeLookups && this.parametersPanel) {
                this.parametersPanel.onCascadeLookupsCallbackCore(result.cascadeLookups);
            }
        },
        correctDocumentMapHeight: function() {
            var documentMapPaneElement = this.documentMapPane.GetElement();
            var div = documentMapPaneElement.childNodes[0];
            if(!ASPx.IsExists(div)) {
                return;
            }
            ASPx.SetElementDisplay(div, false);
            var offsetHeight = documentMapPaneElement.offsetHeight - ASPx.GetTopBottomBordersAndPaddingsSummaryValue(div) - this.documentMapPane.heightDiff;
            ASPx.SetStyles(div, {
                display: "",
                height: offsetHeight
            });
        },
        stretchDocumentMap: function() {
            var paneElement = this.documentMapPane.GetElement();
            var child = paneElement.childNodes[0];
            child.style.overflowX = "auto";
        },
        refreshDocumentMapPaneLayout: function() {
            if(this.documentMap && this.documentMap.hasTreeView()) {
                this.setToggleValues(this.documentMapToggle, null, true);
            }
            var hideDocumentMap = false;
            if(!this.documentMap || !this.documentMap.hasTreeView()) {
                hideDocumentMap = true;
            } else if(this.forceDocumentMapCollapsed) {
                hideDocumentMap = true;
                this.forceDocumentMapCollapsed = null;
            } else if(!this.documentMapExisted) {
                if(this.sideCenterPane && this.sideCenterPane.IsCollapsed()) {
                    this.sideCenterPane.Expand();
                }
                if(this.documentMapPane) {
                    this.documentMapPane.Expand();
                }
            }
            if(hideDocumentMap) {
                this.ensurePaneCollapsing(this.documentMapPane);
                if(!this.parametersPanelPane || this.parametersPanelPane.IsCollapsed())
                    this.ensurePaneCollapsing(this.sideCenterPane);
            }
            var documentMapToggleValue = this.sideCenterPane && !this.sideCenterPane.IsCollapsed() && this.documentMapPane && !this.documentMapPane.IsCollapsed();
            this.setToggleValues(this.documentMapToggle, documentMapToggleValue != null);
            this.updateDocumentMapPaneVisible();
        },
        updateDocumentMapPaneVisible: function() {
            var hideDocumentMap = !this.documentMap || !this.documentMap.hasTreeView();
            var hideSidePane = !this.parametersPanel && hideDocumentMap;
            var separatorElement = this.viewerPane.helper.GetSeparatorElement() || (this.sideCenterPane && this.sideCenterPane.helper.GetSeparatorElement());
            var someCollapsedPaneElement = this.sideCenterPane && this.sideCenterPane.GetElement();
            var documentViewerSeparatorDivElement = this.documentMapPane && this.documentMapPane.helper.GetSeparatorDivElement();
            var documentMapStyleDisplay = hideDocumentMap ? "none" : "";
            var sidePaneStyleDisplay = hideSidePane ? "none" : "";
            if(separatorElement)
                separatorElement.style.display = sidePaneStyleDisplay;
            if(someCollapsedPaneElement)
                someCollapsedPaneElement.style.display = sidePaneStyleDisplay;
            if(documentViewerSeparatorDivElement)
                documentViewerSeparatorDivElement.style.display = documentMapStyleDisplay;
            if(!hideDocumentMap && !hideSidePane)
                this.correctDocumentMapHeight();
        },
        pageLoadRibbonEventHandler: function(s, a) {
            this.pageIndex = a.PageIndex;
            this.pageCount = a.PageCount;
            if(!this.viewer.useIFrame && a.PageCount > 0) {
                for(var i = 0; i < this.enabledItems.length; i++)
                    this.enabledItems[i].SetEnabled(true);
            }
            this.updatePageIndexes();
            this.updateElements();
        },
        updatePageIndexes: function() {
            if(!this.ribbonToolbar) {
                return;
            }
            var pageCountLabelText = '';
            if(this.pageCountLabelElement) {
                pageCountLabelText = this.pageCountText + ' ' + this.pageCount;
                if(ASPx.Browser.Firefox) {
                    this.pageCountLabelElement.textContent = pageCountLabelText;
                } else {
                    this.pageCountLabelElement.innerText = pageCountLabelText;
                }
            }
            if(!this.pageNumberEditor)
                return;
            if(this.pageNumberEditor.GetItemCount() == this.pageCount) {
                this.updatePageIndexes_selectPageIndex(this.pageNumberEditor);
                return;
            }
            this.pageNumberEditor.BeginUpdate();
            this.pageNumberEditor.ClearItems();
            this.runLoopInPortionsAsync(
                { to: this.pageCount },
                function(i) {
                    var text = (i + 1).toString();
                    this.pageNumberEditor.AddItem(text, text);
                    if(i === this.pageIndex) {
                        this.updatePageIndexes_selectPageIndex(this.pageNumberEditor);
                    }
                }.aspxBind(this),
                function() { this.pageNumberEditor.EndUpdate(); }.aspxBind(this));
        },
        runLoopInPortionsAsync: function runLoopInPortionsAsync(args, actionCallback, completeCallback) {
            var from = args.from || 0;
            var step = args.step || 100;
            if(from > args.to) {
                throw new Error("Argument 'args.from' can not be greater than 'args.to'.");
            }
            if(step <= 0) {
                throw new Error("Argument 'args.step' can not be less than or equal to zero.");
            }
            var nextBound = from + step;
            if(nextBound > args.to) {
                nextBound = args.to;
            }
            var current;
            for(current = from; current < nextBound; current++) {
                actionCallback(current);
            }
            if(current < args.to) {
                var newArgs = { from: current, to: args.to, step: args.step, timeout: args.timeout };
                var timeout = args.timeout || 50;
                setTimeout(function() { runLoopInPortionsAsync(newArgs, actionCallback, completeCallback); }, timeout);
            } else {
                completeCallback();
            }
        },
        updatePageIndexes_selectPageIndex: function(cbx) {
            cbx.SetSelectedIndex(this.pageIndex);
            //temporary workaround
            if(ASPx.Browser.WebKitFamily)
                cbx.GetInputElement().value = (this.pageIndex + 1).toString();
        },
        beginCallbackRibbonEventHanler: function() {
            this.disableRibbonItems();
        },
        endCallbackRibbonEventHanler: function() {
            for(var i = 0; i < this.enabledItems.length; i++)
                this.enabledItems[i].SetEnabled(true);
            this.updateElements();
        },
        prepareRibbonItems: function() {
            var i;
            var currentChild;
            var pageInfoTemplate = this.ribbonToolbar.GetItemByName("dxxrdvPageNumbers");
            var pageNumberElementId;
            if(pageInfoTemplate && pageInfoTemplate.element.children) {
                if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 10) {
                    for(i = 0; i < pageInfoTemplate.element.children.length; i++) {
                        currentChild = pageInfoTemplate.element.children[i];
                        if(!currentChild || !currentChild.className)
                            continue;
                        if(currentChild.className.indexOf("dxxrdvrPageNumberComboBox") >= 0)
                            pageNumberElementId = currentChild.id;
                        if(currentChild.className.indexOf("dxxrdvrPageCountLabel") >= 0)
                            this.pageCountLabelElement = currentChild;
                    }
                } else {
                    for(i = 0; i < pageInfoTemplate.element.childElementCount; i++) {
                        currentChild = pageInfoTemplate.element.children[i];
                        if(!currentChild || !currentChild.classList || !currentChild.classList.length)
                            continue;
                        for(var j = 0; j < currentChild.classList.length; j++) {
                            if(currentChild.classList[j] === "dxxrdvrPageNumberComboBox")
                                pageNumberElementId = currentChild.id;
                            if(currentChild.classList[j] === "dxxrdvrPageCountLabel")
                                this.pageCountLabelElement = currentChild;
                        }
                    }
                }
            }
            this.pageNumberEditor = ASPx.GetControlCollection().Get(pageNumberElementId);
            if(this.pageNumberEditor) {
                this.pageNumberEditor.ValueChanged.AddHandler(function(s) {
                    if(pageInfoTemplate && s && s.GetValue)
                        this.ribbonToolbar.onExecCommand(pageInfoTemplate, s.GetValue());
                }.aspxBind(this));
            }
            this.fillIgnoredItems();
            this.disableRibbonItems();
        },
        updateElements: function() {
            var val = this.pageIndex > 0;
            this.setRibbonItemEnabled("dxxrdvBtnFirstPage", val);
            this.setRibbonItemEnabled("dxxrdvBtnPrevPage", val);

            val = this.pageCount > 0 && this.pageIndex < this.pageCount - 1;
            this.setRibbonItemEnabled("dxxrdvBtnNextPage", val);
            this.setRibbonItemEnabled("dxxrdvBtnLastPage", val);
            this.setRibbonItemEnabled(this.searchItemName, this.viewer.IsSearchAllowed());
        },
        setToggleValues: function(toggle, value, enabled) {
            if(!toggle)
                return;
            if(value != null)
                toggle.SetValue(value);
            if(enabled != null)
                toggle.SetEnabled(enabled);
        },
        setRibbonItemEnabled: function(name, enabled) {
            if(this.ignoreItems[name])
                return;
            var item = this.ribbonToolbar.GetItemByName(name);
            if(item)
                item.SetEnabled(enabled);
        },
        disableRibbonItems: function() {
            if(!this.enabledItems)
                this.enabledItems = [];
            else if(this.enabledItems.length > 0)
                return;
            var tabCount = this.ribbonToolbar.GetTabCount();
            for(var tabIndex = 0; tabIndex < tabCount; tabIndex++) {
                var currentRibbonTab = this.ribbonToolbar.GetTab(tabIndex);
                if(!currentRibbonTab)
                    continue;
                for(var i = 0; i < currentRibbonTab.groups.length; i++) {
                    var currentGroup = currentRibbonTab.groups[i];
                    if(!currentGroup.items)
                        continue;
                    for(var j = 0; j < currentGroup.items.length; j++) {
                        var itemElement = currentGroup.items[j];
                        if(itemElement.GetEnabled() && itemElement.name.substring(0, 6) === "dxxrdv"
                            && !(itemElement.name === this.parametersPanelToggleCommandName || itemElement.name === this.documentMapToggleCommandName)) {
                            this.enabledItems[this.enabledItems.length] = itemElement;
                            itemElement.SetEnabled(false);
                        }
                    }
                }
            }
        },
        fillIgnoredItems: function() {
            var tabCount = this.ribbonToolbar.GetTabCount();
            for(var tabIndex = 0; tabIndex < tabCount; tabIndex++) {
                var currentRibbonTab = this.ribbonToolbar.GetTab(tabIndex);
                if(!currentRibbonTab)
                    continue;
                for(var i = 0; i < currentRibbonTab.groups.length; i++) {
                    var currentGroup = currentRibbonTab.groups[i];
                    if(!currentGroup.items)
                        continue;
                    for(var j = 0; j < currentGroup.items.length; j++) {
                        var itemElement = currentGroup.items[j];
                        if(!itemElement.GetEnabled() && itemElement.name.substring(0, 6) === "dxxrdv") {
                            this.ignoreItems[itemElement.name] = itemElement;
                        }
                    }
                }
            }
        },
        fireToolbarItemValueChanged: function(sender, eventArgs) {
            var item = this.toolbar.getItemByEditor(sender);
            this.ToolbarItemValueChanged.FireEvent(this, new ASPxClientToolbarItemValueChangedEventArgs(eventArgs.processOnServer, item, sender));
        },
        fireToolbarItemClick: function(sender, eventArgs) {
            this.ToolbarItemClick.FireEvent(this, eventArgs);
        },
        handleButton: function(item, parameter) {
            var btnId = item.name;
            if(!ASPx.IsExists(this.viewer))
                return;
            if(btnId === this.searchItemName) {
                this.viewer.Search();
                return;
            }
            if(btnId === this.parametersPanelToggleCommandName || btnId === this.documentMapToggleCommandName) {
                this.handleSidePanelControlsToggle(btnId === this.parametersPanelToggleCommandName, parameter);
                return;
            }
            if(btnId === "dxxrdvBtnPrint") {
                this.viewer.Print();
                return;
            }
            if(btnId === "dxxrdvBtnPrintPage") {
                this.viewer.Print(this.pageIndex);
                return;
            }
            if(item.parent) {
                var saveToDisk = item.parent.name === this.saveToDiskItemName;
                var saveToWindow = item.parent.name === this.saveToWindowItemName;
                if (btnId.indexOf("dxxrdv") === 0 && (saveToDisk || saveToWindow)) {
                    var format = btnId.indexOf(this.exportToPrefix) === 0 ? btnId.substring(this.exportToPrefix.length) : btnId;
                    this.lastSaveFormat = (format === "Img") ? "Png" : format;
                    saveToDisk ? this.viewer.SaveToDisk(this.lastSaveFormat) : this.viewer.SaveToWindow(this.lastSaveFormat);
                    return;
                }
            }
            if(btnId === this.saveToWindowItemName) {
                this.viewer.SaveToWindow(this.lastSaveFormat);
                return;
            }
            if(btnId === this.saveToDiskItemName) {
                this.viewer.SaveToDisk(this.lastSaveFormat);
                return;
            }
            if(this.pageIndex < 0 || this.pageCount <= 0)
                return;
            var index = this.pageIndex;
            if(btnId === "dxxrdvBtnFirstPage") {
                index = 0;
            } else if(btnId === "dxxrdvBtnPrevPage") {
                index = this.pageIndex - 1;
            } else if(btnId === "dxxrdvBtnNextPage") {
                index = this.pageIndex + 1;
            } else if(btnId === "dxxrdvBtnLastPage") {
                index = this.pageCount - 1;
            } else if(btnId === "dxxrdvPageNumbers") {
                index = parameter - 1;
            }
            this.viewer.GotoPage(index);
        },
        handleSidePanelControlsToggle: function(isParametersPanelToggle, toggleChecked) {
            this.unsubscribeSplitterCollapsing();
            var otherPane = isParametersPanelToggle ? this.documentMapPane : this.parametersPanelPane;
            var currentPane = isParametersPanelToggle ? this.parametersPanelPane : this.documentMapPane;

            if(!currentPane || (!toggleChecked && currentPane.IsCollapsed()))
                return;
            if(toggleChecked) {
                currentPane.Expand();
                if(this.sideCenterPane.IsCollapsed()) {
                    this.ensurePaneCollapsing(otherPane);
                    this.sideCenterPane.Expand();
                }
            } else if(!this.sideCenterPane.IsCollapsed()) {
                if(!otherPane || otherPane.IsCollapsed()) {
                    this.ensurePaneCollapsing(this.sideCenterPane);
                } else {
                    if(isParametersPanelToggle && !this.documentMap.hasTreeView()) {
                        this.ensurePaneCollapsing(this.sideCenterPane);
                        this.ensurePaneCollapsing(otherPane);
                    } else
                        this.ensurePaneCollapsing(currentPane);
                }
            }
            this.subscribeSplitterCollapsing();
        },
        ensurePaneCollapsing: function(pane) {
            if(pane && !pane.CollapseForward())
                pane.CollapseBackward();
        },
        splitterPaneCollapseStateChanged: function(s, e) {
            var pane = e.pane;
            if(!pane)
                return;
            if(pane.IsCollapsed())
                this.splitterPaneCollapsed(pane);
            else
                this.splitterPaneExpanded(pane);
        },
        splitterPaneCollapsed: function(pane) {
            if(this.sideCenterPane && pane.name === this.sideCenterPane.name) {
                this.setToggleValues(this.parametersPanelToggle, false);
                this.setToggleValues(this.documentMapToggle, false);
            }
            if(this.parametersPanelPane && pane.name === this.parametersPanelPane.name)
                this.setToggleValues(this.parametersPanelToggle, false);
            if(this.documentMapPane && pane.name === this.documentMapPane.name)
                this.setToggleValues(this.documentMapToggle, false);
        },
        splitterPaneExpanded: function(pane) {
            if(pane.name === this.sideCenterPane.name) {
                if(this.documentMapPane) {
                    if(this.parametersPanelPane) {
                        this.documentMapPane.IsCollapsed() && this.parametersPanelPane.Expand();
                    } else {
                        this.documentMapPane.Expand();
                    }
                } else {
                    this.parametersPanelPane && this.parametersPanelPane.Expand();
                }
            }
            if(this.documentMapPane && (pane.name === this.documentMapPane.name || pane.name === this.sideCenterPane.name))
                this.correctDocumentMapHeight();
            if(this.parametersPanelPane && !this.parametersPanelPane.IsCollapsed())
                this.setToggleValues(this.parametersPanelToggle, true);
            if(this.documentMapPane && !this.documentMapPane.IsCollapsed())
                this.setToggleValues(this.documentMapToggle, true);
        },
        subscribeSplitterCollapsing: function() {
            if(this.splitter) {
                this.splitter.PaneCollapsed.AddHandler(this.splitterPaneCollapseStateChanged, this);
                this.splitter.PaneExpanded.AddHandler(this.splitterPaneCollapseStateChanged, this);
            }
        },
        unsubscribeSplitterCollapsing: function() {
            if(this.splitter) {
                this.splitter.PaneCollapsed.RemoveHandler(this.splitterPaneCollapseStateChanged, this);
                this.splitter.PaneExpanded.RemoveHandler(this.splitterPaneCollapseStateChanged, this);
            }
        },
        getControlIntValue: function(name) {
            var val = this.getControlValue(name);
            return val ? parseInt(val, 10) : -1;
        },
        getControlValue: function(name) {
            return this.ribbonToolbar.GetItemValueByName(name);
        }
    });
    ASPxClientDocumentViewer.findValidControl = function(name) {
        var control = ASPx.GetControlCollection().Get(name);
        return control && ASPx.IsExistsElement(control.GetMainElement())
            ? control
            : null;
    };
    ASPxClientDocumentViewer.Cast = ASPxClientControl.Cast;
    var ASPxClientToolbarItemValueChangedEventArgs = ASPx.CreateClass(ASPxClientProcessingModeEventArgs, {
        constructor: function(processOnServer, item, editor) {
            this.constructor.prototype.constructor.call(this, processOnServer);
            this.item = item;
            this.editor = editor;
        }
    });

    var ASPxClientCreateCallbackEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(callbackString, command) {
            this.constructor.prototype.constructor.call(this);
            this.callbackString = callbackString;
            this.command = command;
        }
    });

    var ASPxClientDocumentViewerReportViewer = ASPx.CreateClass(ASPxClientReportViewer, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.createCallbackCoreEvent = new ASPxClientEvent();
        },
        CreateCallback: function(callbackString, command) {
            this.RaiseBeginCallbackInternal();
            this.createCallbackCoreEvent.FireEvent(this, new ASPxClientCreateCallbackEventArgs(callbackString, command));
        },
        setFormHelper: function(formHelper) {
            this.formHelper = formHelper;
        },
        removeContentPaddings: function() {
            var viewerContent = this.getContentElement();
            if(viewerContent)
                viewerContent.style.padding = "0px";
        },
        IsSearchAllowed: function() {
            return this.shouldDisableSearchButton ? false : ASPxClientReportViewer.prototype.IsSearchAllowed.call(this);
        },
        subscribeToAspForm: function() {
            // pass
        }
    });

    var ASPxClientDocumentViewerReportDocumentMap = ASPx.CreateClass(ASPxClientReportDocumentMap, {
        subscribeReportViewerEvents: function() {
            // pass
        },
        onBeginCallback: function() {
            this.callbackPanel.ShowLoadingElements();
        },
        onEndCallback: function() {
            this.callbackPanel.HideLoadingElements();
        }
    });

    window.ASPxClientDocumentViewer = ASPxClientDocumentViewer;
    window.ASPxClientToolbarItemValueChangedEventArgs = ASPxClientToolbarItemValueChangedEventArgs;
    window.ASPxClientCreateCallbackEventArgs = ASPxClientCreateCallbackEventArgs;
    window.ASPxClientDocumentViewerReportViewer = ASPxClientDocumentViewerReportViewer;
    window.ASPxClientDocumentViewerReportDocumentMap = ASPxClientDocumentViewerReportDocumentMap;
})(window);