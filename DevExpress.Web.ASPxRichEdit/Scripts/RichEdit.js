/// <reference path="compiled.js"/>
/// <reference path="..\..\DevExpress.Web\Scripts\Utils.js"/>
/// <reference path="..\..\DevExpress.Web\Scripts\Ribbon.js"/>

(function() {
    var constants = {
        ViewID: "_View",
        MeasureID: "_Measure",
        DialogControlID: "_REDC",
        PopupMenuControlID: "_REPUM",
        AutoCorrectBarID: "_AC",
        RibbonID: "_R",
        HelperFrameID: "_HF",
        LoadingPanel: "_LPC",
        Bar: "_Bar",

        MeasurerContainerCssClass: "dxre-measurer",
        HelperFrameCssClass: "dxre-helperFrame",
        InternalCallBackPostfix: "%Rich%Edit",
        PerformCallbackPrefix: "REPC",
        CommandsCallbackPrefix: "REC",
        InternalCallbackPrefix: "IC-"
    };
    var ASPxClientRichEditCustomCommandExecutedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(commandName, parameter) {
            this.constructor.prototype.constructor.call(this);
            this.commandName = commandName;
            this.parameter = parameter;
        }
    });
    var ASPxClientRichEdit = ASPx.CreateClass(ASPxClientControl, { // : IRichEditControl, IProcessManager
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.rulerSettings = null;
            this.bookmarksSettings = null;
            this.mailMergeOptions = null;
            this.core = null;
            this.units = 1;
            this.clientGuid = null;
            this.readOnly = false;
            this.pendingCallbacks = [];

            this.isInternalServiceCallback = false;
            this.CustomCommandExecuted = new ASPxClientEvent();
            this.BeginSynchronization = new ASPxClientEvent();
            this.EndSynchronization = new ASPxClientEvent();
            this.DocumentChanged = new ASPxClientEvent();
        },

        InlineInitialize: function() {
            ASPxClientControl.prototype.InlineInitialize.call(this);
            if(!(ASPx.Browser.IE && ASPx.Browser.Version < 9))
                this.InitializeRichEditCore();
        },

        Initialize: function() {
            this.constructor.prototype.Initialize.call(this);
            if(ASPx.Browser.IE && ASPx.Browser.Version < 9)
                this.InitializeRichEditCore();

            // Init FullScreen vars
            this.fullScreenTempVars = {};
            this.isInFullScreenMode = false;

            if(this.getConfirmUpdateText())
                this.updateWatcherHelper = new RichEditUpdateWatcherHelper(this);

            if(!ASPx.Browser.TouchUI)
                this.initializeRefocus();
        },

        AfterInitialize: function() {
            this.core.serverDispatcher.onGetResponse(eval(this.startResponse));
            this.constructor.prototype.AfterInitialize.call(this);
            this.AdjustControlCore();
            if(this.workSessionIsLost)
                this.core.commandManager.getCommand(__aspxRichEdit.RichEditClientCommand.ShowErrorSessionHasExpiredMessageCommand).execute();
            this.core.afterInitialize();
        },

        InitializeRichEditCore: function() {
            var unitsType = this.units == 0 ? __aspxRichEdit.RichEditUnit.Centimeter : __aspxRichEdit.RichEditUnit.Inch;
            var viewElement = document.getElementById(this.name + constants.ViewID);
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 9)
                ASPx.Evt.PreventElementDragAndSelect(viewElement);

            var measureContainer = document.createElement("div");
            measureContainer.id = this.name + constants.MeasureID;
            measureContainer.className = constants.MeasurerContainerCssClass;
            document.body.appendChild(measureContainer);

            var core = new __aspxRichEdit.RichEditCore(this, this.clientGuid, this.readOnly ? __aspxRichEdit.ReadOnlyMode.Persistent : __aspxRichEdit.ReadOnlyMode.None, { 
                mainElement: this.GetMainElement(),
                viewElement: viewElement, 
                measurerContainer: measureContainer
            }, unitsType, this.rulerSettings, this.bookmarksSettings, this.mailMergeOptions, { bars: this.getBars() });
            this.core = core;
        },

        getBars: function() {
            var bars = [];
            bars.push(new ContextMenuBar(this.getPopupMenu(), this));
            var ribbon = this.GetRibbon();
            if(ribbon)
                bars.push(new RibbonBar(ribbon, this))
            return bars;
        },

        getAutoCorrectBars: function() {
            var bars = {};
            bars[__aspxRichEdit.AutoCorrectBarType.InsertList] = new AutoCorrectBarControl(this, __aspxRichEdit.AutoCorrectBarType.InsertList);
            return bars;
        },
        getEditableDocument: function() {
            return this.core.getEditableDocument();
        },
        getModifiedState: function() {
            return this.core.getModifiedState();
        },
        initializeRefocus: function() {
            var refocusFunction = function() {
                var activeElement = ASPx.GetActiveElement();
                if(activeElement && !ASPx.IsInteractiveControl(activeElement))
                    this.Focus();
            }.aspxBind(this);

            var refocusOnClick = function(element) {
                ASPx.Evt.AttachEventToElement(element, "mouseup", function(evt){ window.setTimeout(refocusFunction, 120); });
            }

            var statusBar = this.getStatusBar();
            if(statusBar)
                refocusOnClick(statusBar);
            var ribbon = this.GetRibbon();
            if(ribbon)
                refocusOnClick(ribbon.GetMainElement());
        },

        /* Dialogs beginning */
        showDialog: function(commandID, parameters, callback, afterClosing, isModal) {
            var dialog = ASPxClientRichEdit.ASPxRichEditDialogList[commandID]
            if(dialog) {
                if(this.hasActiveDialog()) {
                    var activeDialog = ASPx.Dialog.GetLastDialog(this);
                    activeDialog.HideDialog();
                }
                dialog.Execute(this, parameters, callback, afterClosing, isModal);
            } else
                alert("Dialog is not found");
        },
        canCaptureFocus: function() {
            return !ASPx.Browser.TouchUI ? !this.hasActiveModalDialog() && !this.getPopupMenu().IsVisible() : !this.hasActiveModalDialog();
        },
        hasActiveDialog: function() {
            return !!this.stateObject.currentDialog;
        },
        hasActiveModalDialog: function() {
            return this.hasActiveDialog() && ASPx.Dialog.GetLastDialog(this).isModal;
        },
        /* Dialogs end */

        /* Callbacks */
        sendInternalServiceCallback: function(callbackPrefix, arg, callbackOwnerControl) {
            this.isInternalServiceCallback = true;
            arg = ASPx.FormatCallbackArg(constants.InternalCallbackPrefix + callbackPrefix, arg);
            this.createCallbackCore(arg, callbackOwnerControl);
        },
        sendCustomCallback: function(callbackPrefix, arg) {
            arg = ASPx.FormatCallbackArg(callbackPrefix, arg);
            this.createCallbackCore(arg);
        },
        createCallbackCore: function(arg, callbackOwnerControl) {
            if(this.InCallback())
                this.pendingCallbacks.push([arg, callbackOwnerControl]);
            else {
                if(callbackOwnerControl)
                    this.setOwnerControlCallback(callbackOwnerControl);
                this.CreateCallback(arg);
            }
        },
        DoEndCallback: function() {
            this.constructor.prototype.DoEndCallback.call(this);
            if (this.callbackOwner != null && !this.isErrorOnCallback)
                this.callbackOwner.OnEndCallback();
            this.isErrorOnCallback = false;
            if(this.pendingCallbacks.length) {
                var callbackInfo = this.pendingCallbacks.shift();
                if(callbackInfo[1])
                    this.setOwnerControlCallback(callbackInfo[1]);
                this.CreateCallback(callbackInfo[0]);
            }
        },
        clearOwnerControlCallback: function() {
            this.callbackOwner = null;
        },
        setOwnerControlCallback: function(owner) {
            this.callbackOwner = owner;
        },
        /* Callbacks end */

        /* Requests */
        sendRequest: function(requestQueryString, viaInternalCallback) {
            var resourceUrl = this.getResourceUrl();
            if(viaInternalCallback)
                this.sendInternalServiceCallback(constants.CommandsCallbackPrefix, requestQueryString);
            else {
                var postdata = "commands=" + requestQueryString;
                this.createXMLHttpRequest(resourceUrl, postdata);
            }
        },
        sendDownloadRequest: function(downloadRequestType, parameters) {
            var resourceUrl = this.getResourceUrl() + "&downloadRequestType=" + downloadRequestType;
            if(parameters)
                resourceUrl += "&parameters=" + parameters;
            var helperFrame = this.getHelperFrame();
            helperFrame.document.location = resourceUrl;
        },
        getResourceUrl: function() {
            var url = document.URL;
            if (url.indexOf("?") != -1)
                url = url.substring(0, url.indexOf("?"));
            if(/.*\.aspx$/.test(url))
                url = url.substring(0, url.lastIndexOf("/") + 1);
            else if(url.lastIndexOf("/") != url.length - 1)
                url += "/";
            url += "DXS.axd?s=" + this.core.sessionGuid + "&c=" + this.clientGuid;
            return url;
        },
        createXMLHttpRequest: function(resourceUrl, postdata) {
            var xmlHttp = new XMLHttpRequest();
            var async = true;
            this.increaseHandlerRequestCount();
            xmlHttp.open("POST", resourceUrl, async);
            xmlHttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=utf-8"); // INFO to pass the postData
            xmlHttp.onreadystatechange = function () {
                if (xmlHttp.readyState === 4) {
                    if (xmlHttp.status === 200) {
                        var response = xmlHttp.responseText;
                        this.onResponseReceived(response);
                    } else if (xmlHttp.status === 404) {
                        this.core.serverDispatcher.reset();
                        this.core.commandManager.getCommand(__aspxRichEdit.RichEditClientCommand.ShowErrorSessionHasExpiredMessageCommand).execute();
                    }
                }
            }.aspxBind(this);
            xmlHttp.onerror = function () {
            };
            xmlHttp.send(postdata);
            this.setLoadingPanelVisible(true);
        },
        onResponseReceived: function (jsonResponse) {
            var response = eval("(" + jsonResponse + ")");
            this.decreaseHandlerRequestCount();
            this.core.serverDispatcher.onGetResponse(response);
            this.unlockEndCallbackEvent();
            this.setLoadingPanelVisible(false);
            this.raiseEndSynchronization();
        },
        /* Requests end */

        AdjustControlCore: function() {
            this.adjustRibbon();
            this.adjustMainView();
        },
        adjustControlInFullScreenMode: function() {
            this.SetWidth(ASPx.GetDocumentClientWidth());
            this.SetHeight(ASPx.GetDocumentClientHeight() - this.getExternalRibbonControlHeight());
            this.AdjustControlCore();
            this.adjustPlaceholderDiv();
        },
        adjustMainView: function() {
            var mainView = this.getMainView();
            if (mainView && this.core) {
                var innerControlsHeight = this.getInternalRibbonControlHeight() + this.getStatusBarHeight() + this.getRulerHeight();
                var height = mainView.parentNode.offsetHeight - ASPx.GetTopBottomBordersAndPaddingsSummaryValue(mainView.parentNode) 
                    - innerControlsHeight - ASPx.GetTopBottomBordersAndPaddingsSummaryValue(mainView);
                if (height < 0)
                    height =  innerControlsHeight;
                mainView.style.height = height + "px";
                if(this.core.canvasManager)
                    this.core.canvasManager.adjust(true);
                if(this.core.horizontalRulerControl)
                    this.core.horizontalRulerControl.adjust();
            }
        },
        adjustRibbon: function() {
            var ribbonControl = this.GetRibbon();
            if(ribbonControl)
                ribbonControl.AdjustControl();
        },
        adjustPlaceholderDiv: function() {
            var ribbonControl = this.GetRibbon();
            if(ribbonControl) {
                ASPx.SetStyles(this.placeholderDiv, {
                    "width": ribbonControl.GetWidth(),
                    "height": ribbonControl.GetHeight()
                });
            }
        },
        ribbonStateChanged: function() {
            if(this.isInFullScreenMode && this.isRibbonExternal()) {
                ASPx.Attr.ChangeStyleAttribute(this.GetMainElement(), "top", this.getExternalRibbonControlHeight() + "px");
                this.adjustControlInFullScreenMode();
            } else
                this.adjustMainView();
        },
        getRibbonControlHeight: function() {
            var ribbonControl = this.GetRibbon();
            return ribbonControl ? ribbonControl.GetHeight() : 0;
        },
        getInternalRibbonControlHeight: function() {
            return this.isRibbonExternal() ? 0 : this.getRibbonControlHeight();
        },
        getExternalRibbonControlHeight: function() {
            return this.isRibbonExternal() ? this.getRibbonControlHeight() : 0;
        },
        getStatusBarHeight: function() {
            var statusBar = this.getStatusBar();
            return statusBar ? statusBar.offsetHeight : 0;
        },
        getRulerHeight: function () {
            if(this.core.horizontalRulerControl)
                return this.core.horizontalRulerControl.getHeight();
            return 0;
        },
        SetFullscreenMode: function(fullscreen) {
            if(this.isInFullScreenMode === fullscreen)
                return;
            this.core.commandManager.getCommand(__aspxRichEdit.RichEditClientCommand.FullScreen).execute();
        },
        toggleFullScreenMode: function() {
            this.isInFullScreenMode = !this.isInFullScreenMode;
            if(this.isInFullScreenMode)
                this.setFullScreenMode();
            else
                this.setNormalMode();
        },
        setFullScreenMode: function() {
            this.prepareFullScreenMode();
            this.togglePlaceholderDiv();
            if(this.isRibbonExternal())
                this.setExternalRibbonPositionOnPageTop();
            this.adjustControlInFullScreenMode();      
        },
        setNormalMode: function() {
            if(this.isRibbonExternal())
                this.restoreExternalRibbonPositionOnPage(this);
            var mainElement = this.GetMainElement();
            ASPx.Attr.RestoreStyleAttribute(mainElement, "left");
            ASPx.Attr.RestoreStyleAttribute(mainElement, "top");
            if(!this.getLastDialog())
                this.restoreBodyScroll();
            ASPx.Attr.RestoreStyleAttribute(mainElement, ASPx.Browser.IE ? "zIndex" : "z-index");
            document.body.style.margin = this.fullScreenTempVars.savedBodyMargin;
            ASPx.Attr.RestoreStyleAttribute(mainElement, "position");
            
            ASPx.Attr.RestoreStyleAttribute(mainElement, ASPx.Browser.IE ? "borderTopWidth" : "border-top-width");
            ASPx.Attr.RestoreStyleAttribute(mainElement, ASPx.Browser.IE ? "borderLeftWidth" : "border-left-width");
            ASPx.Attr.RestoreStyleAttribute(mainElement, ASPx.Browser.IE ? "borderRightWidth" : "border-right-width");
            ASPx.Attr.RestoreStyleAttribute(mainElement, ASPx.Browser.IE ? "borderBottomWidth" : "border-bottom-width");
            
            this.SetHeight(this.fullScreenTempVars.savedCurrentHeight);
            this.SetWidth(this.fullScreenTempVars.savedCurrentWidth);

            document.documentElement.scrollTop = this.fullScreenTempVars.savedBodyScrollTop;
            document.documentElement.scrollLeft = this.fullScreenTempVars.savedBodyScrollTop;

            this.togglePlaceholderDiv();

            this.AdjustControlCore();
        },
        prepareFullScreenMode: function() {
            var mainElement = this.GetMainElement();
            ASPx.Attr.ChangeStyleAttribute(mainElement, ASPx.Browser.IE ? "borderTopWidth" : "border-top-width", "0px");
            ASPx.Attr.ChangeStyleAttribute(mainElement, ASPx.Browser.IE ? "borderLeftWidth" : "border-left-width", "0px");
            ASPx.Attr.ChangeStyleAttribute(mainElement, ASPx.Browser.IE ? "borderRightWidth" : "border-right-width", "0px");
            ASPx.Attr.ChangeStyleAttribute(mainElement, ASPx.Browser.IE ? "borderBottomWidth" : "border-bottom-width", "0px");

            this.fullScreenTempVars.savedBodyScrollTop = ASPx.GetDocumentScrollTop();
            this.fullScreenTempVars.savedBodyScrollLeft = ASPx.GetDocumentScrollLeft();

            ASPx.Attr.ChangeStyleAttribute(mainElement, "position", "fixed");
            ASPx.Attr.ChangeStyleAttribute(mainElement, "top", this.getExternalRibbonControlHeight() + "px");
            ASPx.Attr.ChangeStyleAttribute(mainElement, "left", "0px");
            
            ASPx.Attr.ChangeStyleAttribute(mainElement, ASPx.Browser.IE ? "zIndex" : "z-index", 10001);
            this.hideBodyScroll();

            this.fullScreenTempVars.savedBodyMargin = document.body.style.margin;
            document.body.style.margin = 0;
            
            if(ASPx.IsPercentageSize(mainElement.style.width))
                this.fullScreenTempVars.savedCurrentWidth = mainElement.style.width;
            else
                this.fullScreenTempVars.savedCurrentWidth = this.GetWidth();
            if(ASPx.IsPercentageSize(mainElement.style.height))
                this.fullScreenTempVars.savedCurrentHeight = mainElement.style.height;
            else
                this.fullScreenTempVars.savedCurrentHeight = this.GetHeight();
        },
        hideBodyScroll: function() {
            ASPx.Attr.ChangeStyleAttribute(document.documentElement, "position", "static");
            ASPx.Attr.ChangeStyleAttribute(document.documentElement, "overflow", "hidden");
        },
        restoreBodyScroll: function() {
            ASPx.Attr.RestoreStyleAttribute(document.documentElement, "overflow");
            ASPx.Attr.RestoreStyleAttribute(document.documentElement, "position");
        },
        togglePlaceholderDiv: function() {
            if(!this.placeholderDiv) {
                this.placeholderDiv = ASPx.CreateHtmlElement("div");
                ASPx.InsertElementAfter(this.placeholderDiv, this.GetMainElement());
                ASPx.SetStyles(this.placeholderDiv, {
                    "background-color": "white",
                    "position": "fixed",
                    "top": 0,
                    "left": 0
                });
            }
            ASPx.SetElementDisplay(this.placeholderDiv, this.isInFullScreenMode);
        },
        setExternalRibbonPositionOnPageTop: function() {
            var ribbonMainElement = this.GetRibbon().GetMainElement();
            if(ribbonMainElement.style.position == "fixed")
                return;
            ASPx.Attr.ChangeStyleAttribute(ribbonMainElement, "position", "fixed");
            ASPx.Attr.ChangeStyleAttribute(ribbonMainElement, "top", "0px");
            ASPx.Attr.ChangeStyleAttribute(ribbonMainElement, "left", "0px");
            ASPx.Attr.ChangeStyleAttribute(ribbonMainElement, ASPx.Browser.IE ? "zIndex" : "z-index", 10002);

            if(ASPx.IsPercentageSize(ribbonMainElement.style.width))
                this.fullScreenTempVars.saveRibbonWidth = ribbonMainElement.style.width;
            else
                this.fullScreenTempVars.saveRibbonWidth = this.GetRibbon().GetWidth();
            ribbonMainElement.style.width = "100%";
        },
        restoreExternalRibbonPositionOnPage: function() {
            var ribbonMainElement = this.GetRibbon().GetMainElement();
            if(!ribbonMainElement.style.position)
                return;
            ASPx.Attr.RestoreStyleAttribute(ribbonMainElement, "left");
            ASPx.Attr.RestoreStyleAttribute(ribbonMainElement, "top");
            ASPx.Attr.RestoreStyleAttribute(ribbonMainElement, "position");
            ASPx.Attr.RestoreStyleAttribute(ribbonMainElement, ASPx.Browser.IE ? "zIndex" : "z-index");
            if(this.fullScreenTempVars.saveRibbonWidth) {
                if(!ASPx.IsNumber(this.fullScreenTempVars.saveRibbonWidth) && ASPx.IsPercentageSize(this.fullScreenTempVars.saveRibbonWidth))
                    ribbonMainElement.style.width = this.fullScreenTempVars.saveRibbonWidth;
                else
                    ribbonMainElement.style.width = this.fullScreenTempVars.saveRibbonWidth + "px";
                this.fullScreenTempVars.saveRibbonWidth = undefined;
            }
        },
        SetWidth: function(width, skipAdjustControl) {
            if(!ASPx.IsNumber(width) && ASPx.IsPercentageSize(width))
                this.setSizeInPercentage("width", width);
            else
                ASPxClientControl.prototype.SetWidth.call(this, width);
            if(!skipAdjustControl)
                this.AdjustControl();
        },
        SetHeight: function(height, skipAdjustControl) {
            if(!ASPx.IsNumber(height) && ASPx.IsPercentageSize(height))
                this.setSizeInPercentage("height", height);
            else
                ASPxClientControl.prototype.SetHeight.call(this, height);

            if(!skipAdjustControl)
                this.AdjustControl();
        },
        setSizeInPercentage: function(sizePropertyName, size){
            this.GetMainElement().style[sizePropertyName] = size;
            this.UpdateAdjustmentFlags();
        },
        isRibbonExternal: function() {
            return !!this.ribbonClientID;
        },
        getRibbonBar: function() {
            var bars = this.core.bars.bars;
            for(var i = 0, bar; bar = bars[i]; i++) {
                if(bar instanceof RibbonBar)
                    return bar;
            }
            return null;
        },
        GetRibbon: function() {
            if(!this.ribbon) {
                var ribbonID = this.ribbonClientID || this.name + constants.RibbonID;
                this.ribbon = ASPx.GetControlCollection().Get(ribbonID);
            }
            return this.ribbon;
        },
        getStatusBar: function() {
            return this.GetChildElement(constants.Bar);
        },
	    getMainView: function() {
            return this.GetChildElement(constants.ViewID);
        },
        getLoadingPanel: function() {
            if(!this.loadingPanel) {
                var loadingPanelDiv = this.GetChildElement(constants.LoadingPanel)
                this.loadingPanel = ASPx.GetNodeByTagName(loadingPanelDiv, "table", 0);
            }
            return this.loadingPanel;
        },
        GetDialogPopupControl: function() {
            return ASPx.GetControlCollection().Get(this.name + constants.DialogControlID);
        },
        getLastDialog: function () {
            return ASPx.Dialog.GetLastDialog(this);
        },
        getPopupMenu: function() {
            return ASPx.GetControlCollection().Get(this.name + constants.PopupMenuControlID);
        },
        getHelperFrame: function() {
            if(ASPx.Browser.Firefox)
                this.removeHelperFrame();
            if(!this.helperFrame)
                this.helperFrame = this.createHelperFrame();
            return this.helperFrame;
        },
        createHelperFrame: function() {
            var helperFrame = document.createElement("iframe");
            var frameSize = ASPx.Browser.Safari ? "1px" : "0px";
            helperFrame.style.width = frameSize;
            helperFrame.style.height = frameSize;
            helperFrame.name = this.name + constants.HelperFrameID;
            helperFrame.id = this.name + constants.HelperFrameID;
            helperFrame.className = constants.HelperFrameCssClass;
            this.GetMainElement().appendChild(helperFrame);
            return window.frames[helperFrame.name];
        },
        removeHelperFrame: function() {
            var frameElement = this.GetChildElement(constants.HelperFrameID);
            if(frameElement)
                this.GetMainElement().removeChild(frameElement);

            try {
                delete window.frames[this.name + constants.HelperFrameID];
            } catch(e) { }
        
            if(this.helperFrame)
                this.helperFrame = null;
        },

        increaseHandlerRequestCount: function () {
            this.handlerRequestCount++;
        },
        decreaseHandlerRequestCount: function () {
            this.handlerRequestCount--;
        },
        InHandlerRequestProcessing: function () {
            return this.handlerRequestCount > 0;
        },
        InCallback: function () {
            return this.InHandlerRequestProcessing() || this.constructor.prototype.InCallback.call(this);
        },
        lockEndCallbackEvent: function () {
            this.endCallbackEventLockCount++;
        },
        unlockEndCallbackEvent: function () {
            var wasLocked = this.endCallbackEventLocked();
            if (wasLocked) {
                this.endCallbackEventLockCount--;

                var unlocks = !this.endCallbackEventLocked();
                if (unlocks)
                    this.RaiseEndCallback();
            }
        },
        endCallbackEventLocked: function () {
            return this.endCallbackEventLockCount > 0;
        },
        RaiseEndCallback: function () {
            if (this.endCallbackEventLocked()) return;

            ASPxClientControl.prototype.RaiseEndCallback.call(this);
        },
        OnCallback: function (result) {
            if (this.callbackOwner != null)
                this.callbackOwner.OnCallback(result);
            else if(typeof result === "string" && result.indexOf(constants.CommandsCallbackPrefix + "|") === 0) {
                result = eval(result.substr(constants.CommandsCallbackPrefix.length + 1));
                this.core.serverDispatcher.onGetResponse(result);
                this.raiseEndSynchronization();
            }
            else {
                this.core.initialize(result.sessionGuid, result.fileName);
                this.core.serverDispatcher.onGetResponse(eval(result.startResponse));
            }
        },
        OnCallbackError: function(errorMessage, data) {
            ASPxClientControl.prototype.OnCallbackError.call(this, errorMessage, data);
            var lastDialog = this.getLastDialog();
            if(lastDialog != null)
                lastDialog.OnCallbackError();
        },
        BrowserWindowResizeSubscriber: function () {
            return this.isInFullScreenMode || ASPxClientControl.prototype.BrowserWindowResizeSubscriber.call(this);
        },
        OnBrowserWindowResize: function(evt) {
            if(this.isInFullScreenMode)
                this.adjustControlInFullScreenMode();
            else
                this.AdjustControl();
        },
        setLoadingPanelVisible: function(visible) {
            var loadingPanel = this.getLoadingPanel();
            if(loadingPanel)
                loadingPanel.style.display = visible ? "" : "none";
        },
        getConfirmUpdateText: function() {
            return this.confirmUpdate;
        },
        getUpdateWatcherHelper: function() {
            return this.updateWatcherHelper;
        },
        setConfirmOnCallbackEnabled: function(enabled) {
            this.getUpdateWatcherHelper().SetConfirmOnCallbackEnabled(enabled);
        },
        setConfirmOnPostbackEnabled: function(enabled) {
            this.getUpdateWatcherHelper().SetConfirmOnPostbackEnabled(enabled);
        },
        confirmOnLosingChanges: function() {
            return this.getUpdateWatcherHelper() ? this.getUpdateWatcherHelper().ConfirmOnCustomControlEvent() : true;
        },
        syncSessionGuid: function(sessionGuid) {
            this.UpdateStateObjectWithObject({ s: sessionGuid });
        },

        Focus: function() {
            this.core.captureFocus();
        },
        showPopupMenu: function(x, y) {
            var popupMenu = this.getPopupMenu();
            if(popupMenu)
                popupMenu.ShowAtPos(x, y);
        },
        hidePopupMenu: function() {
            var popupMenu = this.getPopupMenu();
            if(popupMenu)
                popupMenu.Hide();
        },
        RaiseCustomCommandExecuted: function(commandName, parameter) {
            if(!this.CustomCommandExecuted.IsEmpty()) {
                var args = new ASPxClientRichEditCustomCommandExecutedEventArgs(commandName, parameter);
                this.CustomCommandExecuted.FireEvent(this, args);
            }
        },
        raiseBeginSynchronization: function() {
            if(!this.BeginSynchronization.IsEmpty()) {
                var args = new ASPxClientEventArgs();
                this.BeginSynchronization.FireEvent(this, args);
            }
        },
        raiseEndSynchronization: function() {
            if(!this.EndSynchronization.IsEmpty()) {
                var args = new ASPxClientEventArgs();
                this.EndSynchronization.FireEvent(this, args);
            }
        },
        isTouchMode: function() {
            return ASPx.Browser.TouchUI && !ASPx.Browser.MSTouchUI;
        },

        // State
        UpdateStateObject: function() {
            this.UpdateStateObjectWithObject({ scmds: this.core.serverDispatcher.getRequestJSON(), cguid: this.clientGuid, isc: this.isInternalServiceCallback });
            this.isInternalServiceCallback = false;
        },
        PerformCallback: function (parameter) {
            this.sendCustomCallback(constants.PerformCallbackPrefix, parameter);
            this.core.closeDocument();
        },
        HasUnsavedChanges: function() {
            return this.core.getModifiedState() != __aspxRichEdit.IsModified.False;
        },
        ReconnectToExternalRibbon: function() {
            if(this.isRibbonExternal()) {
                var ribbonBar = this.getRibbonBar();
                if(ribbonBar) {
                    ribbonBar.detachEvents();
                    this.ribbon = ASPx.GetControlCollection().Get(this.ribbonClientID);
                    ribbonBar.control = this.ribbon;
                    ribbonBar.attachEvents();
                }
            }
        },
        raiseDocumentChanged: function() {
            if(!this.DocumentChanged.IsEmpty()){
                var args = new ASPxClientEventArgs();
                this.DocumentChanged.FireEvent(this, args);
            }
        },
        OnDispose: function() {
            this.constructor.prototype.OnDispose.call(this);
            this.core.dispose();
        }
    });

    ASPxClientRichEdit.Constants = {
        PerformCallbackPrefix: constants.PerformCallbackPrefix
    };

    var BarBase = ASPx.CreateClass(null, { /* implements IBar */
        constructor: function(control, owner) {
            this.control = control;
            this.owner = owner;
            this.onChanged = new __aspxRichEdit.EventDispatcher(); // onChanged: EventDispatcher<IBarListener>;
        },
        getCommandKeys: function() { // getCommandKeys(): RichEditClientCommand[];
        },
        setItemValue: function(key, value) { // setItemValue(key: RichEditClientCommand, value: any);
        },
        setItemEnabled: function(key, enabled) { // setItemEnabled(key: RichEditClientCommand, enabled: boolean);
        },
        setItemVisible: function(key, visible) { // setItemVisible(key: RichEditClientCommand, visible: boolean);
        },
        setItemSubItems: function (key, subItems) { //  setItemSubItems(key: RichEditClientCommand, subItems: any[]);
        },
        setEnabled: function(enabled) { // setEnabled(enabled: boolean);
        },
        isVisible: function() { // isVisible(): boolean;
            return true;
        },
        isContextMenu: function() { // isContextMenu(): boolean;
            return false;
        },
        hasContextItems: function() {
            return false;
        },
        getContextKeys: function() {
            return [];
        },
        setContextItemVisible: function(key, visible) {
        },
        activateContextItem: function(key){
        },
        raiseBarCommandExecuted: function(item, parameter) {
            var key = parseInt(item.name);
            if(!isNaN(key) && __aspxRichEdit.RichEditClientCommand[key] !== undefined)
                this.onChanged.raise("NotifyBarCommandExecuted", key, parameter);
            else
                this.owner.RaiseCustomCommandExecuted(item.name, parameter);
        },
        raiseBarUpdateRequested: function() {
            this.onChanged.raise("NotifyBarUpdateRequested");
        }
    });

    var RibbonBar = ASPx.CreateClass(BarBase, {
        constructor: function(control, owner) {
            this.constructor.prototype.constructor.call(this, control, owner);
            this.cache = {};
            this.visibility = ASPxClientRibbonState.Normal;
            this.attachEvents();
            this.contextTabs = null;
        },
        getCommandKeys: function() { // getCommandKeys(): RichEditClientCommand[];
            var itemsCache = this.getItemsCache(this.control.GetActiveTab().index);
            var result = [];
            for(var itemKey in itemsCache) {
                if(!itemsCache.hasOwnProperty(itemKey)) continue;
                result.push(itemKey);
            }
            return result;
        },
        setItemSubItems: function (key, subItems) {
            var itemsCache = this.getItemsCache(this.control.GetActiveTab().index);
            var obj = [];
            if(parseInt(key) === __aspxRichEdit.RichEditClientCommand.ChangeStyle) {
                obj.push({ text: "Paragraph Styles", items: [] });
                obj.push({ text: "Character Styles", items: [] });
                for(var i = 0, style; style = subItems[0][i]; i++) {
                    obj[0].items.push({ text: style.text, value: style.value, image: { className: this.owner.paragraphRibbonStyleCssClass } });
                }
                for(var i = 0, style; style = subItems[1][i]; i++) {
                    obj[1].items.push({ text: style.text, value: style.value, image: { className: this.owner.characterRibbonStyleCssClass } });
                }
            }
            if(parseInt(key) === __aspxRichEdit.RichEditClientCommand.ApplyTableStyle) {
                obj.push({ text: "Table Styles", items: [] });
                for(var i = 0, style; style = subItems[i]; i++) {
                    obj[0].items.push({ text: style.text, value: style.value, image: { className: this.owner.tableRibbonStyleCssClass } });
                }
            }
            if(obj.length) {
                for(var i = 0, item; item = itemsCache[key][i]; i++) {
                    var prevValue = item["aspxricheditsubitems"];
                    if(!prevValue || this.areSubItemsChanged(prevValue, obj)) {
                        item["aspxricheditsubitems"] = obj;
                        item.setItems(obj);
                    }
                }
            }
        },
        areSubItemsChanged: function(oldItems, newItems) {
            if(oldItems.length !== newItems.length)
                return true;
            for(var i = oldItems.length - 1; i >= 0; i--) {
                var oldGroup = oldItems[i],
                    newGroup = newItems[i];
                if(oldGroup.text !== newGroup.text)
                    return true;
                if(oldGroup.items.length !== newGroup.items.length)
                    return true;
                for(var j = newGroup.items.length - 1; j >= 0; j--) {
                    var oldItem = oldGroup.items[j];
                    var newItem = newGroup.items[j];
                    if(oldItem.value !== newItem.value)
                        return true;
                    if(oldItem.text !== newItem.text)
                        return true;
                }
            }
            return false;
        },

        setItemValue: function(key, value) {
            var itemsCache = this.getItemsCache(this.control.GetActiveTab().index);
            for(var i = 0, item; item = itemsCache[key][i]; i++)
                item.SetValue(value);
        },
        setItemEnabled: function(key, enabled) {
            var itemsCache = this.getItemsCache(this.control.GetActiveTab().index);
            for(var i = 0, item; item = itemsCache[key][i]; i++)
                item.SetEnabled(enabled);
        },
        setEnabled: function(enabled) {
            this.control.SetEnabled(enabled);
        },
        isVisible: function() {
            return this.visibility != ASPxClientRibbonState.Minimized;
        },
        getItemsCache: function(tabIndex) {
            if(!this.control["aspxricheditlink"]) {
                this.cache = {};
                this.control["aspxricheditlink"] = true;
            }
            if(!this.cache[tabIndex]) {
                this.cache[tabIndex] = {};
                var tab = this.control.GetTab(tabIndex);
                for(var i = 0, group; group = tab.groups[i]; i++) {
                    this.ensureSubItemsCache(tabIndex, group);
                }
            }
            return this.cache[tabIndex];
        },
        ensureSubItemsCache: function(tabIndex, parentItem) {
            for(var i = 0, item; item = parentItem.items[i]; i++) {
                var key = parseInt(item.name);
                if(!isNaN(key) && __aspxRichEdit.RichEditClientCommand[key] !== undefined) {
                    if(!this.cache[tabIndex][key])
                        this.cache[tabIndex][key] = [];
                    this.cache[tabIndex][key].push(item);
                }
                if(item.items && item.items.length)
                    this.ensureSubItemsCache(tabIndex, item);
            }
        },
        attachEvents: function() {
            this.control.CommandExecuted.AddHandler(function(s, e) { this.raiseBarCommandExecuted(e.item, e.parameter); }.aspxBind(this), this.owner);
            this.control.MinimizationStateChanged.AddHandler(function(s, e) {
                this.visibility = e.ribbonState;
                if(e.ribbonState == ASPxClientRibbonState.TemporaryShown)
                    this.raiseBarUpdateRequested();
                this.owner.ribbonStateChanged(); 
            }.aspxBind(this), this.owner);

            this.control.DialogBoxLauncherClicked.AddHandler(function(s, e) { this.raiseBarCommandExecuted(e.group, null) }.aspxBind(this), this.owner);
            this.control.ActiveTabChanged.AddHandler(function(s, e) { this.raiseBarUpdateRequested(); }.aspxBind(this), this.owner);
        },
        detachEvents: function() {
            this.control.CommandExecuted.removeHandlerByControlName(this.owner.name);
            this.control.MinimizationStateChanged.removeHandlerByControlName(this.owner.name);
            this.control.DialogBoxLauncherClicked.removeHandlerByControlName(this.owner.name);
            this.control.ActiveTabChanged.removeHandlerByControlName(this.owner.name);
        },
        hasContextItems: function() {
            return true;
        },
        getContextKeys: function() {
            if(!this.contextTabs) {
                this.contextTabs = [];
                for(var tab, i = 0; tab = this.control.tabs[i]; i++) {
                    if(tab.isContext && tab.categoryName)
                        this.contextTabs.push(parseInt(tab.categoryName));
                }
            }
            return this.contextTabs;
        },
        setContextItemVisible: function(key, visible) {
            this.control.SetContextTabCategoryVisible(key, visible);
        },
        activateContextItem: function(key) {
            for(var tab, i = 0; tab = this.control.tabs[i]; i++) {
                if(tab.isContext && tab.categoryName == key) {
                    this.control.SetActiveTab(tab);
                    break;
                }
            }
        }
    });
    var ContextMenuBar = ASPx.CreateClass(BarBase, {
        constructor: function(control, owner) {
            this.cache = {};

            control.ItemClick.AddHandler(function(s, e) {
                this.raiseBarCommandExecuted(e.item, null); 
                if(owner.isTouchMode())
                    this.owner.Focus();
            }.aspxBind(this));

            control.CloseUp.AddHandler(function() {
                if(!ASPx.Browser.TouchUI)
                    this.owner.Focus();
            }.aspxBind(this));

            control.PopUp.AddHandler(function(s, e) {
                this.raiseBarUpdateRequested(); 
            }.aspxBind(this));

            this.constructor.prototype.constructor.call(this, control, owner);
        },
        isVisible: function() {
            return this.control.IsVisible();
        },
        getCommandKeys: function() { // getCommandKeys(): RichEditClientCommand[];
            var itemsCache = this.getItemsCache();
            var result = [];
            for(var itemKey in itemsCache) {
                if(!itemsCache.hasOwnProperty(itemKey)) continue;
                result.push(parseInt(itemKey));
            }
            return result;
        },
        setItemValue: function(key, value) {
            var itemsCache = this.getItemsCache();
            for(var i = 0, item; item = itemsCache[key][i]; i++)
                item.SetChecked(!!value);
        },
        setItemEnabled: function(key, enabled) {
            var itemsCache = this.getItemsCache();
            for(var i = 0, item; item = itemsCache[key][i]; i++)
                item.SetEnabled(enabled);
        },
        setItemVisible: function(key, visible) {
            var itemsCache = this.getItemsCache();
            for(var i = 0, item; item = itemsCache[key][i]; i++)
                item.SetVisible(visible);
        },
        setEnabled: function(enabled) {
            this.control.SetEnabled(enabled);
        },
        isContextMenu: function() {
            return true;
        }, 
        getItemsCache: function() {
            if(!this.control["aspxricheditlink"]) {
                this.cache = null;
                this.control["aspxricheditlink"] = true;
            }
            if(!this.cache) {
                this.cache = {};
                this.ensureSubItemsCache(this.control);
            }
            return this.cache;
        },
        ensureSubItemsCache: function(parentItem) {
            var length = parentItem.GetItemCount();
            for(var i = 0; i < length; i++) {
                var item = parentItem.GetItem(i);
                var key = parseInt(item.name);
                if(!isNaN(key) && __aspxRichEdit.RichEditClientCommand[key] !== undefined) {
                    if(!this.cache[key])
                        this.cache[key] = [];
                    var item = parentItem.GetItem(i);
                    this.cache[key].push(item);
                }
                this.ensureSubItemsCache(item);
            }
        }
    });

    var RichEditUpdateWatcherHelper = ASPx.CreateClass(ASPx.UpdateWatcherHelper, {
        constructor: function(owner) {
            this.constructor.prototype.constructor.call(this, owner);
            this.richedit = owner;
            this.confirmOnCallback = false;
            this.confirmOnPostback = false;
        },
        HasChanges: function() {
            return this.richedit.getModifiedState();
        },
        NeedConfirmOnCallback: function(dxCallbackOwner, arg) {
            return this.confirmOnCallback && arg.indexOf(constants.InternalCallbackPrefix) !== 0;
        },
        NeedConfirmOnPostback: function() {
            return this.confirmOnPostback;
        },
        GetConfirmUpdateText: function() {
            return this.richedit.getConfirmUpdateText();
        },

        SetConfirmOnCallbackEnabled: function(enabled) {
            this.confirmOnCallback = enabled;
        },
        SetConfirmOnPostbackEnabled: function(enabled) {
            this.confirmOnPostback = enabled;
        }
    });

    function findRichEditControl(subControlName, subControlID) {
        var richEditID = subControlName.substr(0, subControlName.length - subControlID.length);
        return ASPx.GetControlCollection().Get(richEditID);
    }

    window.ASPxClientRichEdit = ASPxClientRichEdit;
    window.ASPxClientRichEditCustomCommandExecutedEventArgs = ASPxClientRichEditCustomCommandExecutedEventArgs;
})();