(function() {

ASPx.SpreadsheetUploadControlUrlParametr = "DXSSUC";
var ASPxClientSpreadsheetCustomCommandExecutedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(commandName, parameter, item) {
        this.constructor.prototype.constructor.call(this);
        this.commandName = commandName;
        this.parameter = parameter;
        this.item = item;
    }
});
var ASPxClientSpreadsheetDocumentChangedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function() {
        this.constructor.prototype.constructor.call(this);
    }
});
var ASPxClientSpreadsheetSynchronizationEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function() {
        this.constructor.prototype.constructor.call(this);
    }
});
var ASPxClientSpreadsheetHyperlinkClickEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(htmlEvent, handled, targetUri, isExternal) {
        this.constructor.prototype.constructor.call(this);
        this.handled = handled;
        this.htmlEvent = htmlEvent;
        this.isExternal = isExternal;
        this.targetUri = targetUri;
    }
});

var SpreadsheetCssClasses = {
    MainSheetDiv: "dxss-md",
    
    HeaderContainer: "hcc",
    HeaderRow: "dxss-rowHeader",
    HeaderColumn: "dxss-colHeader",
    HeaderCell: "dxss-htd",

    SingleCellTextBox: "dxss-sctb",
    ComplexCellTextBox: "dxss-cctb",
    TextBoxContent: "dxss-tb",
    DrawingBox: "dxss-db",
    CellSelectedBorderElement: "dxss-csb",
    CellDynamicSelectionElement: "dxss-cdse",
    CellActiveSelectedElement: "dxss-case",
    CellRangeSelectedElement: "dxss-crse",
    TopRangeBorderElement: "dxss-trbe",
    RightRangeBorderElement: "dxss-rrbe",
    BottomRangeBorderElement: "dxss-brbe",
    LeftRangeBorderElement: "dxss-lrbe",
    SelectionMovementBorderElement: "dxss-smbe",
    TouchSelectionElement: "dxss-tse",
    TouchResizeElement: "dxss-tre",
    TextWrapper: "dxss-tw",
    AutoFilterImage: "dxss-afi",
    DropDownButtonImage: "dxss-ddb",
    PopupMessage: "dxss-pm",
    DropDownPanel: "dxss-ddp",
    InvalidDataCircle: "dxss-idc",

    DrawingBoxSelectedElement: "dxss-dbse",
    ChartElement: "dxss-chart",
    CellEditorElement: "dxss-cee",
    CellTextViewElement: "dxss-ctve", // TODO Text?

    GridLineHidden: "dxss-hideGridLines",
    GridLineHorizontal: "dxss-h",
    GridLineVertical: "dxss-v",
    TileContainer: "dxss-tc",
    SupportFrameElement: "dxss-supportFrame",
    InputTargetFrameElement: "dxss-inputTarget",
    getGridLine: function(isCol){ return isCol ? this.GridLineVertical : this.GridLineHorizontal; }
};
var ASPxClientSpreadsheet = ASPx.CreateClass(ASPxClientControl, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);

        this.sessionGuid = null;
        this.tileSize = { col: 0, row: 0 };
        this.defaultCellSize = { width: 0, height: 0 };
        this.visibleRangePadding = { col: 0, row: 0 };
        this.lastFilledCell = { col: 0, row: 0 };
        this.currentActiveCell = {col:-1, row:-1}; // TODO rename to this.currentActiveCellForSelection
        this.stateController = null;
        this.sheetIndex = -1;
        this.sheetLoaded = false;
        this.keyboardManager = null;
        this.ribbonManager = null;
        this.ribbonClientID = "";
        this.documentName = "";
        this.readOnly = false;
        this.handlerRequestCount = 0;
        this.endCallbackEventLockCount = 0;
        this.chartsInfo = {};
        this.fullScreenTempVars = {};
        this.isInFullScreenMode = false;
        this.forceUpdateDialogCollection = {};
        this.clientInstanceGuid = "00000000-0000-0000-0000-000000000000";
        this.dialogList = {};
        
        this.modified = false;
        this.updateWatcherHelper = null;

        this.showFormulaBar = true;
        this.cellTextViewElement = null;
        this.SelectionChanged = new ASPxClientEvent();
        this.CustomCommandExecuted = new ASPxClientEvent();
        this.DocumentChanged = new ASPxClientEvent();
        this.BeginSynchronization = new ASPxClientEvent();
        this.EndSynchronization = new ASPxClientEvent();
        this.HyperlinkClick = new ASPxClientEvent();
    },
    // Initialization
    InlineInitialize: function () {
        this.genarateClientInstanceGuid();
        ASPxClientControl.prototype.InlineInitialize.call(this);
    },
    Initialize: function () {
        ASPxClientControl.prototype.Initialize.call(this);
    },
    AfterInitialize: function () {
        this.initializeUpdateWatcherHelper();
        this.constructor.prototype.AfterInitialize.call(this);
        if(this.sessionIsOpened()) {
            this.loadCurrentSheet();
        }
    },
    initializeUpdateWatcherHelper: function() {
        if(this.getConfirmUpdateText())
            this.updateWatcherHelper = new SpreadsheetUpdateWatcherHelper(this);
    },
    sheetInitialize: function() {
        this.initializeHandlers();
        this.initializeShortcuts();
        this.initializeTargetInputController();
        this.getPaneManager().createTouchUIScroller();
    },
    initializeHandlers: function () {
        this.getPaneManager().attachEventToMainElement();

        this.getPaneManager().attachEventsToPaneElements();

        if(this.showFormulaBar)
            this.getPaneManager().attachEventsToFormulaBarElements();
       
        this.attachEventsToRibbon();

        this.attachEventsToTabControl();
    },  
    
    attachEventsToRibbon: function(spreadSheet) {
        var ribbon = this.GetRibbon();
        if(ribbon) {
            ribbon.CommandExecuted.AddHandler(function(s, e) { this.onRibbonCommand(e.item.name, e.parameter, e.item); }.aspxBind(this), this);
            ribbon.MinimizationStateChanged.AddHandler(function(s, e) { this.ribbonMinimizeStateChanged(e.ribbonState); }.aspxBind(this), this);
            ribbon.DialogBoxLauncherClicked.AddHandler(function(s, e) { this.onRibbonCommand(e.group.name, null, null); }.aspxBind(this), this);
        }
    },
    detachEventsFromRibbon: function() {
        var ribbon = this.GetRibbon();
        if(ribbon) {
            ribbon.CommandExecuted.removeHandlerByControlName(this.name);
            ribbon.MinimizationStateChanged.removeHandlerByControlName(this.name);
            ribbon.DialogBoxLauncherClicked.removeHandlerByControlName(this.name);
        }
    },

    // TODO relocate next group of functions to TabControl.js
    attachEventsToTabControl: function() {
        var tabControl = this.getTabControl();
        if(tabControl) {
            tabControl.ActiveTabChanged.AddHandler(function(s, e) { this.onSheetIndexChanged(e.tab.index); }.aspxBind(this), this); // TODO change index to name when collaboration mode
            tabControl.TabClick.AddHandler(this.onTabControlTabClick.aspxBind(this));
        }
    },
    onTabControlTabClick: function(s, e) {
        if(ASPx.Evt.IsRightButtonPressed(e.htmlEvent))
            this.showTabSelectorContextMenu(e.htmlEvent);
        else if(e.htmlEvent.type === "dblclick")
            this.showRenameSheetDialog();
    },
    showTabSelectorContextMenu: function(e) {
        var tabControl = this.getTabControl(),
            visibleSheetsCount = tabControl.getVisibleSheets() ? tabControl.getVisibleSheets().length : 0,
            hiddenSheetsCount = tabControl.getHiddenSheets() ? tabControl.getHiddenSheets().length : 0;

        var context = {
            isTabSelectorMenu: true,
            canHideSheet: visibleSheetsCount > 1,
            canUnhideSheet: hiddenSheetsCount > 0,
            canRemoveSheet: visibleSheetsCount > 1,
            lockedWorkbook: !!this.workbookLocked,
            mouseX: ASPx.Evt.GetEventX(e),
            mouseY: ASPx.Evt.GetEventY(e)
        };

        this.getPopupMenuHelper().showPopupMenu(context);
    },
    showRenameSheetDialog: function() {
        ASPx.SSMenuItemClick(this.name, ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("RenameSheet").id, []);
    },

    

    initializeTargetInputController: function() {
        this.ensureKeyboardManager();
        var inputController = this.getInputController();
    },
    initializeShortcuts: function() {
        var keyboardManager = this.getKeyboardManager();
        keyboardManager.InitializeShortcuts(ASPx.SpreadsheetKeyboardManager.Shortcuts);
    },
    genarateClientInstanceGuid: function() {
        this.clientInstanceGuid = ASPx.CreateGuid();
    },
    getClientInstanceGuid: function() {
        return this.clientInstanceGuid;
    },
    // Helpers
    getPaneManager: function() {
        if(!this.paneManager)
            this.paneManager = new ASPxClientSpreadsheet.PaneManager(this);
        return this.paneManager;
    },
    getRenderProvider: function() {
        if(!this.renderProvider)
            this.renderProvider = new ASPxClientSpreadsheet.RenderProvider(this);
        return this.renderProvider;
    },
    getValidationHelper: function() {
        if(!this.validationHelper)
            this.validationHelper = new ASPxClientSpreadsheet.ValidationHelper(this);
        return this.validationHelper;
    },
    getGridResizingHelper: function() {
        if(!this.gridResizingHelper)
            this.gridResizingHelper = new ASPxClientSpreadsheet.GridResizingHelper(this);
        return this.gridResizingHelper;
    },
    getKeyboardManager: function(){
        if(!this.keyboardManager)
            this.keyboardManager = new ASPx.SpreadsheetKeyboardManager();
        return this.keyboardManager;
    },
    getInputController: function() {
        if(!this.inputController)
            this.inputController = new ASPxClientSpreadsheet.InputController(this, this.getRenderProvider().getInputTargetElement());
        return this.inputController;
    },
    getRibbonManager: function() {
        if (!this.ribbonManager)
            this.ribbonManager= new ASPxClientSpreadsheet.RibbonManager(this);
        return this.ribbonManager;
    },
    getStateController: function() {
        if(!this.stateController)
            this.stateController = new ASPxClientSpreadsheet.StateController(this);
        return this.stateController;
    },
    getSelectionHelper: function() {
        if(!this.selectionHelper)
            this.selectionHelper = new ASPxClientSpreadsheet.SelectionHelper(this);
        return this.selectionHelper;
    },
    getDynamicSelectionHelper: function() {
        if(!this.dynamicSelectionHelper)
            this.dynamicSelectionHelper = new ASPxClientSpreadsheet.DynamicSelectionHelper(this);
        return this.dynamicSelectionHelper;
    },
    getEditingHelper: function() {
        if(!this.editingHelper)
            this.editingHelper = new ASPxClientSpreadsheet.EditingHelper(this);
        return this.editingHelper;
    },
    getPopupMenuHelper: function() {
        if(!this.popupMenuHelper)
            this.popupMenuHelper = new ASPxClientSpreadsheet.PopupMenuHelper(this);
        return this.popupMenuHelper;
    },
    getFormulaIntelliSenseManager: function() {
        if(!this.formulaIntelliSenseManager)
            this.formulaIntelliSenseManager = new ASPxClientSpreadsheet.FormulaIntelliSenseManager(this);
        return this.formulaIntelliSenseManager;
    },
    getDialogList: function() {
        if(ASPx.GetObjectKeys(this.dialogList).length === 0)
            this.dialogList = ASPxClientSpreadsheet.CreateDialogList();
        return this.dialogList;
    },
    updateIntelliSenseElementsPosition: function() {
        var isFormulaBarEditing = this.isFormulaBarEditMode();
        var requiredElementNearIntelliSenseElements = isFormulaBarEditing ?
            this.getRenderProvider().getFormulaBarTextBoxElement() : this.getEditingHelper().getEditorElement();
        this.getFormulaIntelliSenseManager().updateIntelliSenseElementsPosition(requiredElementNearIntelliSenseElements);
    },
    hideIntelliSenseElements: function() {
        this.getFormulaIntelliSenseManager().hideIntelliSenseElements();
    },
    attachTooltipsToEditor: function(editor) {
        this.getFormulaIntelliSenseManager().attachToEditorElement(editor);
    },
    isFunctionsListBoxDisplayed: function() {
        return this.getFormulaIntelliSenseManager().isFunctionsListBoxDisplayed();
    },
    displayCurrentFunctionArgumentsHint: function() {
        this.getFormulaIntelliSenseManager().displayCurrentFunctionArgumentsHint();
    },
    ensureKeyboardManager: function() {
        this.getKeyboardManager();
    },
    getCellTextViewElement: function() {
        if(!this.cellTextViewElement && this.showFormulaBar)
            this.cellTextViewElement = this.getEditingHelper().createCellTextViewElement();
        return this.cellTextViewElement;
    },
    isFormulaBarEditMode: function() {
        return this.getStateController().getEditMode() === ASPxClientSpreadsheet.StateController.Modes.FormulaBarEdit;
    },

    // TODO move to paneHelper
    getActiveCellModelPosition: function() {
        return this.getStateController().getActiveCellModelPosition();
    },
    GetRibbon: function() {
        var ribbonID = this.ribbonClientID || this.getRenderProvider().getChildControlsPrefix() + ASPxClientSpreadsheet.ChildControlIdPostfixes.ribbonControl;
        return ASPx.GetControlCollection().Get(ribbonID);
    },
    getPopupMenu: function() {
        return ASPx.GetControlCollection().Get(this.getRenderProvider().getChildControlsPrefix() + ASPxClientSpreadsheet.ChildControlIdPostfixes.popupMenu);
    },
    getAutoFilterPopupMenu: function() {
        return ASPx.GetControlCollection().Get(this.getRenderProvider().getChildControlsPrefix() + ASPxClientSpreadsheet.ChildControlIdPostfixes.autoFilterPopupMenu);
    },
    // IDialog interface
    GetDialogPopupControl: function() {
        return ASPx.GetControlCollection().Get(this.getRenderProvider().getChildControlsPrefix() + ASPxClientSpreadsheet.ChildControlIdPostfixes.dialogPopupControl);
    },
    getTabControl: function() {
        return ASPx.GetControlCollection().Get(this.getRenderProvider().getChildControlsPrefix() + ASPxClientSpreadsheet.ChildControlIdPostfixes.tabControl);
    },
    getFunctionsListBox: function() {
        return ASPx.GetControlCollection().Get(this.getRenderProvider().getChildControlsPrefix() + ASPxClientSpreadsheet.ChildControlIdPostfixes.functionsListBox);
    },
    getFormulaBarMenu: function() {
        return ASPx.GetControlCollection().Get(this.getRenderProvider().getChildControlsPrefix() + ASPxClientSpreadsheet.ChildControlIdPostfixes.formulaBarMenu);
    },

    // Model params
    getVisibleRangePaddings: function () {
        return this.visibleRangePadding;
    },
    getTileSize: function() {
        return this.tileSize;
    },
    getDefaultCellSize: function() {
        return this.defaultCellSize;
    },
    getLastFilledCell: function() { 
        return this.lastFilledCell; 
    },
    // Locked cell and Readonly mode
    CanChangeSheetStylesAndLayout: function() {
        return !this.readOnly && !this.sheetLocked;
    },
    CanEditDocument: function() {
        return !this.readOnly;
    },
    isActiveCellLocked: function() {
		var selection = this.getSelectionInternal();
        var cellLayoutInfo = this.getSelectionActiveCellLayoutInfoInternal(selection);
        return ASPxClientSpreadsheet.ProtectionResolver.cellLocked_ByLayoutInfo(this.sheetLocked, cellLayoutInfo);
    },
    cellLocked_ByLayoutInfo: function(cellLayoutInfo, supressMessageBox) {
        var cellLocked = ASPxClientSpreadsheet.ProtectionResolver.cellLocked_ByLayoutInfo(this.sheetLocked, cellLayoutInfo);
        if(cellLocked && !supressMessageBox)
            this.showMessageBox_EditProtectedCell();
		return cellLocked;
    },
    setSheetLocked: function(locked) {
        if(this.sheetLocked != locked) {
            this.sheetLocked = locked;
            this.onSheetLockedChanged();
        }
    },
    setWorkBookLocked: function(locked) {
        if(this.workbookLocked != locked) {
            this.workbookLocked = locked;
            this.onWorkbookLockedChanged();
        }
    },
    setReadOnlyCore: function(readOnly) { 
        readOnly =  typeof(readOnly) == "undefined" ? false : readOnly;
        if(this.readOnly !== readOnly) {
            this.readOnly = readOnly;
            this.switchReadOnlyMode();
            if (!readOnly)
                this.ribbonStateUpdate("onActiveCellChanged");
        }
    },

    // Full Screen    
    isInFullScreen: function() {
        return this.isInFullScreenMode;
    },
    setFullScreenModeFlag: function(isInFullScreen) {
        this.isInFullScreenMode = isInFullScreen;
    },
    SetFullscreenMode: function(fullscreen) {
        if(this.isInFullScreen() === fullscreen)
            return;
        this.onShortCutCommand(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FullScreen").id);
    },
    setFullScreenModeInternal: function() {
        this.setFullScreenModeFlag(!this.isInFullScreenMode);
        if(this.isInFullScreen())
            this.displayControlInFullScreenMode();
        else
            this.displayControlInPageMode();
    },
    displayControlInFullScreenMode: function() {
        this.saveControlPosition();
        this.adjustControlInFullScreenMode();

        if(this.getRenderProvider().isLoadTilesRequired(this.fullScreenTempVars.scrollContainerLeftPosition, this.fullScreenTempVars.scrollContainerTopPosition))
            if(this.isSheetLoaded()) //T313945
                this.getPaneManager().loadTilesForNewVisibleRange(this.fullScreenTempVars.cachedScrollAnchor);
    },
    saveControlPosition: function() {
        var mainElement = this.GetMainElement();
        var correctHeight = 0;
        if(this.isRibbonExternal()) {
            var ribbonManager = this.getRibbonManager();
            ribbonManager.setExternalRibbonPositionOnPageTop(this);
            correctHeight = ribbonManager.saveRibbonHeight;
        }
        ASPx.Attr.ChangeStyleAttribute(mainElement, ASPx.Browser.IE ? "borderTopWidth" : "border-top-width", "0px");
        ASPx.Attr.ChangeStyleAttribute(mainElement, ASPx.Browser.IE ? "borderLeftWidth" : "border-left-width", "0px");
        ASPx.Attr.ChangeStyleAttribute(mainElement, ASPx.Browser.IE ? "borderRightWidth" : "border-right-width", "0px");
        ASPx.Attr.ChangeStyleAttribute(mainElement, ASPx.Browser.IE ? "borderBottomWidth" : "border-bottom-width", "0px");

        this.fullScreenTempVars.savedBodyScrollTop = ASPx.GetDocumentScrollTop();
        this.fullScreenTempVars.savedBodyScrollLeft = ASPx.GetDocumentScrollLeft();

        ASPx.Attr.ChangeStyleAttribute(mainElement, "position", "fixed");
        ASPx.Attr.ChangeStyleAttribute(mainElement, "top", correctHeight + "px");
        ASPx.Attr.ChangeStyleAttribute(mainElement, "left", "0px");
            
        ASPx.Attr.ChangeStyleAttribute(mainElement, ASPx.Browser.IE ? "zIndex" : "z-index", 10001);
        this.hideBodyScroll();
            
        this.fullScreenTempVars.savedBodyMargin = document.body.style.margin;
        document.body.style.margin = 0;
            
        if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 8){
            document.documentElement.scrollTop = 0;
            document.documentElement.scrollLeft = 0;
        }
        if(ASPx.IsPercentageSize(mainElement.style.width)) {
            this.fullScreenTempVars.savedCurrentWidth = mainElement.style.width;
        } else {
            this.fullScreenTempVars.savedCurrentWidth = this.GetWidth();
        }
        if(ASPx.IsPercentageSize(mainElement.style.height)) {
            this.fullScreenTempVars.savedCurrentHeight = mainElement.style.height;
        } else {
            this.fullScreenTempVars.savedCurrentHeight = this.GetHeight();
        }
        this.fullScreenTempVars.cachedScrollAnchor = this.getPaneManager().getScrollAnchorVisiblePosition();
        this.fullScreenTempVars.scrollContainerLeftPosition = this.getRenderProvider().getScrollContainer().scrollLeft;
        this.fullScreenTempVars.scrollContainerTopPosition = this.getRenderProvider().getScrollContainer().scrollTop;
    },    
    displayControlInPageMode: function() {
       if(this.isRibbonExternal()) {
            var ribbonManager = this.getRibbonManager();
            ribbonManager.restoreExternalRibbonPositionOnPage(this);
        }
        var mainElement = this.GetMainElement();
        ASPx.Attr.RestoreStyleAttribute(mainElement, "left");
        ASPx.Attr.RestoreStyleAttribute(mainElement, "top");
        this.restoreBodyScroll();
        ASPx.Attr.RestoreStyleAttribute(mainElement, ASPx.Browser.IE ? "zIndex" : "z-index");
        document.body.style.margin = this.fullScreenTempVars.savedBodyMargin;
        ASPx.Attr.RestoreStyleAttribute(mainElement, "position");
            
        ASPx.Attr.RestoreStyleAttribute(mainElement, ASPx.Browser.IE ? "borderTopWidth" : "border-top-width");
        ASPx.Attr.RestoreStyleAttribute(mainElement, ASPx.Browser.IE ? "borderLeftWidth" : "border-left-width");
        ASPx.Attr.RestoreStyleAttribute(mainElement, ASPx.Browser.IE ? "borderRightWidth" : "border-right-width");
        ASPx.Attr.RestoreStyleAttribute(mainElement, ASPx.Browser.IE ? "borderBottomWidth" : "border-bottom-width");
            
        this.setHeightInternal(this.fullScreenTempVars.savedCurrentHeight, true);
        this.setWidthInternal(this.fullScreenTempVars.savedCurrentWidth, false);
            
        document.documentElement.scrollTop = this.fullScreenTempVars.savedBodyScrollTop;
        document.documentElement.scrollLeft = this.fullScreenTempVars.savedBodyScrollTop;
        this.adjustRootControls();
    },    
    hideBodyScroll: function() {
        ASPx.Attr.ChangeStyleAttribute(document.documentElement, "position", "static");
        ASPx.Attr.ChangeStyleAttribute(document.documentElement, "overflow", "hidden");
    },
    restoreBodyScroll: function() {
        if (!ASPx.Dialog.GetLastDialog(this)) {
            ASPx.Attr.RestoreStyleAttribute(document.documentElement, "overflow");
            ASPx.Attr.RestoreStyleAttribute(document.documentElement, "position");
        }
    },
    
    ribbonMinimizeStateChanged: function(ribbonState) {
        //TODO move adjustRootControls to RenderProvider or PaneManager
        this.adjustRootControls(false);
        this.getPaneManager().changeScrollableArea(ribbonState);        
    },
    // KBS Support
    registerFocusedElement: function(element) {
        this.focusedElement = element;
    },
    unregisterFocusedElement: function() {
        this.focusedElement = null;
    },
    onCellEditorElementFocused: function(element) {
        this.registerFocusedElement(element);
    },
    onCellEditorElementBlured: function(element) {
        if(this.focusedElement == element) {
            this.getStateController().applyCurrentEdition();
	        this.saveSelectionParams(element);
    	    this.unregisterFocusedElement();
            var inputController = this.getInputController();
            if(inputController != element) {
                inputController.captureFocus();
            }
        }
    },
    saveSelectionParams: function(input) {
        this.focusedElementSelectionTextInfo = ASPx.Selection.GetExtInfo(input);
    },
       
    // State
    UpdateStateObject: function(){
        this.UpdateStateObjectWithObject({ s: this.sessionGuid, state: this.getCallbackArgsSerialized() });
    },

    // Sessions
    sessionIsOpened: function() {
        return !!this.getSessionGuid();
    },
    getSessionGuid: function() {
        return this.sessionGuid;
    },
    setSessionGuid: function(sessionGuid) {
        this.sessionGuid = sessionGuid;
    },   
    
    // Callback
    clearOwnerControlCallback: function() {
        this.callbackOwner = null;
    },
    CreateNewDocumentCallback: function() {
        this.sendInternalServiceCallback(ASPxClientSpreadsheet.CallbackPrefixes.NewFileCallbackPrefix, "new");
    },
    SaveDocumentCallback: function() {
        this.sendInternalServiceCallback(ASPxClientSpreadsheet.CallbackPrefixes.SaveFileCallbackPrefix);
    },
    sendInternalServiceCallback: function(callbackCommandPrefix, callbackArgument, callbackOwnerControl, hideLoadingPanel) {
        if(!hideLoadingPanel)
            this.ShowLoadingElements();
        if (callbackOwnerControl)
            this.callbackOwner = callbackOwnerControl;
        this.CreateCallback(ASPx.FormatCallbackArg(callbackCommandPrefix, this.markCallbackAsInternalService(callbackArgument)));
    },
    isInternalServiceCallback: function(callbackArg) {
        return callbackArg.indexOf(ASPxClientSpreadsheet.CallbackPrefixes.InternalCallBackPostfix) > -1;
    },
    sendInternalServicePostback: function(eventArgs) {
        this.SendPostBack(eventArgs);
    },
    OnCallback: function(result) {
        if(this.callbackOwner != null)
            this.callbackOwner.OnCallback(result);
        else if(this.isDocumentLoadingCallback(result))
            this.onDocumentLoadingCallback(result);
        else if(this.isNewDocumentCallback(result))
            this.onNewDocumentCallback(result);
        else if(this.isInsertImageCallback(result))
            this.onInsertImageCallback(result);
        else if(this.isSaveDocumentCallback(result))
            this.onSaveDocumentCallback(result);
    },
    isDocumentLoadingCallback: function(result) {
        return result && result.indexOf(ASPxClientSpreadsheet.CallbackPrefixes.DocumentUpdateCallbackPrefix) == 0;
    },
    getDocumentLoadingCallbackResult: function(result) {
        return result.slice(ASPxClientSpreadsheet.CallbackPrefixes.DocumentUpdateCallbackPrefix.length);
    },
    isNewDocumentCallback: function(result) {
        return result && result.indexOf(ASPxClientSpreadsheet.CallbackPrefixes.NewFileCallbackPrefix) == 0;
    },
    isInsertImageCallback: function(result) {
        return result && result.indexOf(ASPxClientSpreadsheet.CallbackPrefixes.SaveImageCallbackPrefix) === 0;
    },
    isSaveDocumentCallback: function(result) {
        return result && result.indexOf(ASPxClientSpreadsheet.CallbackPrefixes.SaveFileCallbackPrefix) === 0;
    },
    getNewDocumentCallbackResult: function(result) {
        return result.slice(ASPxClientSpreadsheet.CallbackPrefixes.NewFileCallbackPrefix.length + 1);
    },
    getSaveDocumentCallbackResult: function(result) {
        return result.slice(ASPxClientSpreadsheet.CallbackPrefixes.SaveFileCallbackPrefix.length + 1);
    },
    onDocumentLoadingCallback: function(result) {
        this.lockEndCallbackEvent();
        var sessionGuid = this.getDocumentLoadingCallbackResult(result);
        if(this.getSessionGuid() != sessionGuid)
            this.setSessionGuid(sessionGuid);
        this.documentUpdate();
    },
    onNewDocumentCallback: function(result) {
        var newDocumentCallbackResult = this.getNewDocumentCallbackResult(result);
        this.onResponseReceivedCore(newDocumentCallbackResult);
    },
    onSaveDocumentCallback: function(result) {
        var saveDocumentCallbackResult = this.getSaveDocumentCallbackResult(result);
        this.onResponseReceivedCore(saveDocumentCallbackResult);
    },
    onInsertImageCallback: function(result) {
        var callbackResult = result.slice(ASPxClientSpreadsheet.CallbackPrefixes.SaveImageCallbackPrefix .length + 1);
        if(callbackResult.indexOf(ASPxClientSpreadsheet.CallbackPrefixes.SaveImageErrorCallbackPrefix) > -1)
            alert(callbackResult.substr(ASPxClientSpreadsheet.CallbackPrefixes.SaveImageErrorCallbackPrefix.length + 1));
        else {
            var callbackData = callbackResult.substr(ASPxClientSpreadsheet.CallbackPrefixes.SaveImageSuccessCallbackPrefix.length + 1);
            this.onResponseReceivedCore(callbackData);
        }
    },
	documentUpdate: function(){
		this.loadCurrentServerSheet();
	},
    lockEndCallbackEvent: function() {
        this.endCallbackEventLockCount ++;
    },
    unlockEndCallbackEvent: function() {
        var wasLocked = this.endCallbackEventLocked();
        if(wasLocked) {
            this.endCallbackEventLockCount --;

            var unlocks = !this.endCallbackEventLocked();
            if(unlocks)
                this.RaiseEndCallback();
        }
    },
    endCallbackEventLocked: function() {
        return this.endCallbackEventLockCount > 0;
    },
    RaiseEndCallback: function(){
        if(this.endCallbackEventLocked()) return;
            
        ASPxClientControl.prototype.RaiseEndCallback.call(this);
    },    
    markCallbackAsInternalService: function(arg) {
        return [arg, "|", ASPxClientSpreadsheet.CallbackPrefixes.InternalCallBackPostfix].join("");
    },   
    DoEndCallback: function() {
        this.constructor.prototype.DoEndCallback.call(this);
        if (this.callbackOwner != null && !this.isErrorOnCallback)
            this.callbackOwner.OnEndCallback();
        this.isErrorOnCallback = false;
    },

    // Dialogs support
    isSingleCellOrEmptyRangeSelected: function() {
        var stateController = this.getStateController();
        var selection = stateController.getSelection();
        if (selection.range.singleCell)
            return true;

        for (var i = selection.range.leftColIndex; i <= selection.range.rightColIndex; i++)
            for (var k = selection.range.topRowIndex; k <= selection.range.bottomRowIndex; k++) {
                var rawValue = this.getCellValueByVisiblePosition_Internal(i, k);
                if (rawValue != null) {
                    if (ASPx.IsNumber(rawValue))
                        return false;
                }
            }

        return true;
    },
    getCurrentSheetName: function() {
     var tabName = "";
     var tabControl = this.getTabControl();
        if (tabControl) {
            var activeTab = tabControl.tabs[tabControl.activeTabIndex];
            if(activeTab)
                tabName = activeTab.GetText();
        }
      return tabName;
    },
    getCellElementStyleByModelPosition: function(colModelIndex, rowModelIndex, attributesList) {
        var fullStyle = this.getCellFullStyle(colModelIndex, rowModelIndex);
        return this.getStyleSetByAttributes(fullStyle, attributesList);
    },
    getCellFullStyle: function(colModelIndex, rowModelIndex) {
        var cellElement = this.getRenderProvider().getCellElementByModelPosition(colModelIndex, rowModelIndex),
            style = {};
        if(cellElement) {
            var cellTextElement = this.getRenderProvider().getCellTextElementByModelPosition(colModelIndex, rowModelIndex);
            var cellTextElementStyle = ASPx.GetCurrentStyle(cellTextElement);
            var attributes = [];
            for(var attr in cellTextElementStyle) 
                attributes.push(attr);
            style = this.getStyleSetByAttributes(cellTextElementStyle, attributes);

            //TODO may be remove IE check, save on typeOf number
            if(ASPx.Browser.IE && ASPx.Browser.Version < 9 && typeof(style["fontWeight"]) == "number")
                style["fontWeight"] = style["fontWeight"].toString();
            var cellElementStyle = ASPx.GetCurrentStyle(cellElement);
            if(cellElementStyle) {
                style.backgroundColor = ASPx.Color.ColorToHexadecimal(cellElementStyle.backgroundColor) || this.getDefaultCellStyle().backgroundColor;
                style.width = cellElementStyle.width;
                style.height = cellElementStyle.height;
            }
        } else {
            style = this.getDefaultCellStyle();
            var cellLayoutInfo = this.getPaneManager().getCellLayoutInfo(colModelIndex, rowModelIndex);
            if(cellLayoutInfo && cellLayoutInfo.rect) {
                style.width = cellLayoutInfo.rect.width + "px";
                style.height = cellLayoutInfo.rect.height + "px";
            }
        }
        return style;
    },
    getStyleSetByAttributes: function(sourceStyle, attributes) {
        var style = {};
        for(var i = 0, len = attributes.length; i < len; i++) {
            var attr = attributes[i];
            style[attr] = sourceStyle[attr] || "";
        }
        return style;
    },
    getDefaultCellStyle: function() {
        return {
            fontFamily: "Calibri",
            fontSize: "15px",
            backgroundColor: "#FFFFFF",
            width: "64px",
            height: "20px",
            whiteSpace: "nowrap",
            verticalAlign: "bottom"
        };
    },

    // Send data to server
    onRibbonCommand: function(commandId, params, item) {
        var processed = this.onUICommand(commandId, params);
        var customCommand = !processed;
        if(customCommand)
            this.onRibbonCustomCommand(commandId, params, item);
    },    
    onMenuCommand: function(commandId) {
        this.onUICommand(commandId, null);
    },
    onAutoFilterClick: function(element, columnType, hasFilter) {
        var cellContainer = ASPx.GetParentByClassName(element, SpreadsheetCssClasses.SingleCellTextBox),
            cellPosition = cellContainer.id.substr(cellContainer.id.indexOf("ctb.") + 4).split("."),
            columnCaption = this.GetCellValue(cellPosition[0], cellPosition[1]);

        var columnTypeMask = {
            text: 1,
            number: 2,
            date: 4
        };

        //TODO: set checked state based on applied filter command id
        var context = {
            isFilterMenu: true,
            isFilterApplied: !!hasFilter,
            isTextColumn:   !!(columnType & columnTypeMask.text),
            isNumberColumn: !!(columnType & columnTypeMask.number),
            isDateColumn:   !!(columnType & columnTypeMask.date),
            columnCaption: columnCaption
        };

        this.getPopupMenuHelper().showAutoFilterMenu(context, element);
    },
    onShortCutCommand: function(commandId) {
        this.onUICommand(commandId, null);
    },
    onUICommand: function(commandId, params) {
        var processed = false;
        var dialog = this.getDialogList()[commandId];
        if(dialog)
            processed = this.onDialogCommand(dialog, commandId);
        else 
            processed = this.onServerCommand(commandId, params);

        return processed;
    },
    onServerCommand: function(commandId, params) {
        var processed = false;

        var isFormula = ASPxClientSpreadsheet.ServerCommands.isFormulaCommand(commandId);
        var isBorderLineStyle =  ASPxClientSpreadsheet.ServerCommands.isBorderLineStyleCommand(commandId);
        var isSetPaperKind = ASPxClientSpreadsheet.ServerCommands.isSetPaperKindCommand(commandId);

        var command = ASPxClientSpreadsheet.ServerCommands.getCommandByID(commandId);
        if(command) { 
            if (isFormula || isBorderLineStyle || isSetPaperKind)
                params = commandId;
            command(this, params);
            processed = true;
        }

        return processed;
    },
    onDialogCommand: function(dialog, commandId) {
        dialog.Execute(this, commandId);
        return true;
    },
    onRibbonCustomCommand: function(commandName, parameter, item) {
        this.RaiseCustomCommandExecuted(commandName, parameter, item);
    },
    RaiseCustomCommandExecuted: function(commandName, parameter, item) {
        if(!this.CustomCommandExecuted.IsEmpty()){
            var args = new ASPxClientSpreadsheetCustomCommandExecutedEventArgs(commandName, parameter, item);
            this.CustomCommandExecuted.FireEvent(this, args);
        }
    },
    sendCommandRequest: function(requestParams, commandID, commandParams) {
        if(this.readOnly && ASPxClientSpreadsheet.ServerCommands.isCommandDisableInReadOnlyMode(commandID))
            return; 

        if(this.workbookLocked && ASPxClientSpreadsheet.ServerCommands.isCommandDisabledForLockedWorkbook(commandID))
            return; 
        
        if(this.sheetLocked && ASPxClientSpreadsheet.ServerCommands.isCommandDisabledForLockedSheet(commandID)) {
            this.showMessageBox_EditProtectedCell();
			return;
        }

        var cellLocked = this.getIsCellLocked(commandID, commandParams);
        if(cellLocked && ASPxClientSpreadsheet.ServerCommands.isCommandDisabledForLockedCell(commandID)) {
            this.showMessageBox_EditProtectedCell();
			return;
        }

        if(!this.confirmLostChangesIfNeeded(commandID))
            return;

        if(this.commandChangesDocument(commandID))
            this.RaiseDocumentChanged();

        this.showLoadingPanelIfNeeded(commandID);
        this.sendRequest(requestParams);
    },
    getIsCellLocked: function(commandID, commandParams) {
        if(!this.sheetLocked) return false;

        var cellLocked = true;

        var isCellUpdateCommand = ASPxClientSpreadsheet.ServerCommands.isCellUpdateCommand(commandID);
        if(isCellUpdateCommand) {
            cellLocked = ASPxClientSpreadsheet.ProtectionResolver.cellLocked_ByModelIndices(
                this.getPaneManager(), this.sheetLocked, 
                commandParams.CellPositionColumn, 
                commandParams.CellPositionRow);
            
        } else {
            cellLocked = this.isActiveCellLocked();
        }

        return cellLocked;

    },
    canEditCell: function(cellLayoutInfo) { // TODO think about removing
        return !ASPxClientSpreadsheet.ProtectionResolver.cellLocked_ByLayoutInfo(this.sheetLocked, cellLayoutInfo);
    }, 
    increaseHandlerRequestCount: function() {
        this.handlerRequestCount ++;
    },
    decreaseHandlerRequestCount: function() {
        this.handlerRequestCount --;
    },
    InHandlerRequestProcessing: function() {
        return this.handlerRequestCount > 0;
    },
    InCallback: function() {
        return this.InHandlerRequestProcessing() || this.constructor.prototype.InCallback.call(this);
    },
    sendRequest: function(requestQueryString) {
        var resourceUrl = this.getResourceUrl();
        var postdata = requestQueryString + "&" + this.getCallbackArgsSerialized();

        this.createXMLHttpRequest(resourceUrl, postdata);
    },
    createXMLHttpRequest: function(resourceUrl, postdata) {
        var xmlHttp = new XMLHttpRequest();
        var async = true;
        
        this.increaseHandlerRequestCount();
        this.RaiseBeginSynchronization();

        xmlHttp.open("POST", resourceUrl, async);
        xmlHttp.setRequestHeader("Content-Type", "application/x-www-form-urlencoded; charset=utf-8"); // INFO to pass the postData
        xmlHttp.onreadystatechange = function() {
            if(xmlHttp.readyState === 4) {
                if(xmlHttp.status === 200) {
                    var response = xmlHttp.responseText;
                    this.onResponseReceived(response);
                } else if(xmlHttp.status === 404) {
                    this.throwSessionExpiredErrorMessage();
                } 
            }
        }.aspxBind(this);
        xmlHttp.onerror = function () {
        };
        xmlHttp.send(postdata);
    },
    throwSessionExpiredErrorMessage: function() {
        alert("You session has expired.\nPlease, refresh the page to continue.");
    },
    getResourceUrl: function() {
        var url = document.URL;
        if (url.indexOf("?") != -1)
            url = url.substring(0, url.indexOf("?"));
        if(/.*\.aspx$/.test(url))
            url = url.substring(0, url.lastIndexOf("/") + 1);
        else if(url.lastIndexOf("/") != url.length - 1)
            url += "/";
        url = url + "DXS.axd?s=" + this.getSessionGuid();
        return url;
    },
    sendDownloadCopyRequest: function(requestQueryString, commandID) {
        var resourceUrl = this.getResourceUrl() + "&" + requestQueryString;
        var copyFrameElement = this.getRenderProvider().getSupportFrameElement();
        copyFrameElement.document.location = resourceUrl;
    },
    sendPrintRequest: function(requestQueryString, commandID) {
        var resourceUrl = this.getResourceUrl() + "&" + requestQueryString;
        var printFrameElement = this.getRenderProvider().getSupportFrameElement();
        printFrameElement.document.location = resourceUrl;
    },
    insertSpecificFunctionToActiveEditor: function(functionName) {
        var stateController = this.getStateController();
        var selection = stateController.getSelection();
        var cellLayoutInfo = this.getPaneManager().getCellLayoutInfo(selection.activeCellColIndex, selection.activeCellRowIndex);
        stateController.insertSpecificFunction(cellLayoutInfo, functionName); 
    },
    loadCurrentSheet: function() {
        ASPxClientSpreadsheet.ServerCommands.LoadSheet(this);
    },
    loadCurrentServerSheet: function() {
		this.clearSheetIndex();
        this.loadCurrentSheet();
    },
    loadInvisibleTiles: function() {
        ASPxClientSpreadsheet.ServerCommands.LoadInvisibleTiles(this);
    },
    loadInvisibleTilesForFullScreen: function() {
        ASPxClientSpreadsheet.ServerCommands.LoadInvisibleTilesForFullScreenMode(this, {FullScreenMode: true});
    },
    getCallbackArgsSerialized: function() {
        return this.getCallbackArgs().join("&");
    },
    getCallbackArgs: function() { 
        var args = [ ];
        var mainElement = this.getRenderProvider().getWorkbookControl();
        var paneManager = this.getPaneManager();
        
        var scrollAnchor = this.getPaneManager().getScrollAnchorModelPosition();  
        var hasModelIndicesScrollAnchor = scrollAnchor.col >= 0 && scrollAnchor.row >= 0;
        if(hasModelIndicesScrollAnchor) {
            args.push("scrollAnchor.col=" + scrollAnchor.col);
            args.push("scrollAnchor.row=" + scrollAnchor.row);
        } else {
            var scrollContainer = this.getRenderProvider().getScrollContainer();
            args.push("scrollTop="  + scrollContainer.scrollTop);
            args.push("scrollLeft=" + scrollContainer.scrollLeft);
        }

        if (mainElement && mainElement.offsetWidth != 0)
            args.push("width=" + mainElement.offsetWidth);
        else  
			args.push("width=" + ASPx.GetDocumentMaxClientWidth());

        if (mainElement && mainElement.offsetHeight != 0)
            args.push("height=" + mainElement.offsetHeight);
        else 
			args.push("height=" + ASPx.GetDocumentMaxClientHeight());
        
        

        args.push("cachedTiles=" + paneManager.serializeCachedTiles());
        args.push("sheetIndex=" + this.sheetIndex);
        var selection = this.getStateController().getSelection();
        if(selection && selection.isValuable()) {
            var visibleSelection = selection.clone();
            selection = selection.getConvertedToModelIndices(paneManager);
            selection.expandToAllSheetIfRequired();

            if(selection.isAllSelected()) {
                args.push("activeCellColIndex="   + (selection.activeCellColIndex >=0 ? selection.activeCellColIndex : visibleSelection.activeCellColIndex));
                args.push("activeCellRowIndex="   + (selection.activeCellRowIndex >= 0 ? selection.activeCellRowIndex : visibleSelection.activeCellRowIndex));
                args.push("allSelected=true");
            } else if(selection.isDrawingBoxSelection()) {
				args.push("drawingBoxIndex="   + selection.drawingBoxIndex);
            }  else {
                args.push("selectLeftColIndex="   + (selection.range.leftColIndex >= 0 ? selection.range.leftColIndex : visibleSelection.range.leftColIndex));
                args.push("selectTopRowIndex="    + (selection.range.topRowIndex >= 0 ? selection.range.topRowIndex : visibleSelection.range.topRowIndex));
                args.push("selectRightColIndex="  + (selection.range.rightColIndex >= 0 ? selection.range.rightColIndex : visibleSelection.range.rightColIndex));
                args.push("selectBottomRowIndex=" + (selection.range.bottomRowIndex >= 0 ? selection.range.bottomRowIndex : visibleSelection.range.bottomRowIndex));
                args.push("activeCellColIndex="   + (selection.activeCellColIndex >=0 ? selection.activeCellColIndex : visibleSelection.activeCellColIndex));
                args.push("activeCellRowIndex="   + (selection.activeCellRowIndex >= 0 ? selection.activeCellRowIndex : visibleSelection.activeCellRowIndex));
                
                if(selection.entireColsSelected)
                    args.push("entireColsSelected=true");
                if(selection.entireRowsSelected)
                    args.push("entireRowsSelected=true");
                if(selection.multiSelection) {
                    args.push("multiSelection=true");
                    args.push("ranges=" + ASPx.Json.ToJson(selection.ranges));
                }
            }
        }

        return args;
    },
    showLoadingPanelIfNeeded: function(commandID) {
        if (ASPxClientSpreadsheet.ServerCommands.isUIBlockingCommand(commandID))
            this.ShowLoadingPanel();
    },
    confirmLostChangesIfNeeded: function(commandID) {
        if(this.getModified() && ASPxClientSpreadsheet.ServerCommands.isCommandCanRequireConfirm(commandID) && this.getConfirmUpdateText())
            return this.getUpdateWatcherHelper().ConfirmOnCustomControlEvent();
        return true;
    },
    commandChangesDocument: function(commandID) {
        return ASPxClientSpreadsheet.ServerCommands.commandChangesDocument(commandID);
    },

    // Receive data from server
    onResponseReceived: function(jsonResponse){
        this.decreaseHandlerRequestCount();
        this.onResponseReceivedCore(jsonResponse);
        this.unlockEndCallbackEvent();
        this.RaiseEndSynchronization();
    },
    onResponseReceivedCore: function(jsonResponse) {
        try {
            var response = eval("(" + jsonResponse + ")");
        } catch(e) {
            this.throwSessionExpiredErrorMessage();
        }
        this.processResponse(response);
    },
    isSheetLoaded: function() {
        return this.sheetLoaded;
    },
    setSheetLoadedFlag: function(isSheetLoaded) {
        this.sheetLoaded = isSheetLoaded;
    },
    processResponse: function(response){
        if(response.CustomResultProcessingFunction)
            this.customProcessResponse(response);
        else {
            this.processDocumentResponse(response);
            if(!this.isSheetLoaded()) {
                this.setSheetLoadedFlag(true);
                this.sheetInitialize();
            }
        }
    },
    customProcessResponse: function (response) {
        this[response.CustomResultProcessingFunction](response);
    },
    processDocumentResponse: function (response) {
        if(response.error) {
            // show alert or do something like calling this.ProcessCallbackError(errorObj, callbackId)). But we don't have the callbackId, so we can only pass -1.
            return;
        }
        if(!!response.newSessionGuid)
            this.setSessionGuid(response.newSessionGuid);

        if(this.isNewSheetLoaded(response))
            this.getPaneManager().restoreInitialState();

        this.getPaneManager().updateGridTileCache(response);
        this.setCurrentSheetIndex(response.sheetIndex);
        this.setOpenDocumentName(response.documentName);

        this.setSheetLocked(response.sheetLocked);
        this.setWorkBookLocked(response.workbookLocked);

        this.setGridLinesVisibility(!response.gridLinesHidden);

        this.setReadOnlyCore(response.readOnly);

        this.setDefaultCellSize(response.defaultCellSize);

        this.setLastFilledCell(response.lastFilledCell);

        this.getPaneManager().processDocumentResponse(response);
        
        this.getGridResizingHelper().update();
        
        this.getTabControl().processDocumentResponse(response);

        this.getRibbonManager().processDocumentResponse(response);

        this.getStateController().processDocumentResponse(response.selection);

        this.updateChartsInfo(response.charts);

        this.getEditingHelper().processDocumentResponse();

        this.setModified(response.modified);
        //Hide
        this.HideLoadingElements();

        this.updateCellTextViewElementValue();

        this.getValidationHelper().processDocumentResponse(response);

        if(response.lastMessage)
            window.alert(response.lastMessage);

        if(response.removeSheetConfirm)
            this.removeSheetWithConfirmation(response.removeSheetConfirm);
    },
    updateCellTextViewElementValue: function() {
        var cellTextViewElement = this.getCellTextViewElement();
        if(!ASPx.IsExists(cellTextViewElement))
            return;
        var cellModelPosition = this.getActiveCellModelPosition();
        // Fix TESTS TODO to Leonid - when cleared cache on client side, invisible cell value doesn't exist
        if(cellModelPosition.colIndex >= 0 && cellModelPosition.rowIndex >= 0) {
            var activeCellText = this.getPaneManager().getCellEditingText(cellModelPosition.colIndex, cellModelPosition.rowIndex);
            this.setElementValue(this.getCellTextViewElement(), activeCellText);
        }
    },
    setElementsValue: function(value) {
        this.setElementValue(this.cellEditorElement, value);
        var cellTextViewElement = this.getCellTextViewElement();
        if(ASPx.IsExists(cellTextViewElement))
            this.setElementValue(cellTextViewElement, value);
    },
    setElementValue: function(element, value) {
        value = value === null ? "" : value; //IE7,8 support
        element.value = value;
    },
    getEventControlPoint: function(evt) {
        var x = ASPx.Evt.GetEventX(evt);
        var y = ASPx.Evt.GetEventY(evt);
        var mainElement = this.getRenderProvider().getWorkbookControl();
        var controlX = ASPx.GetAbsoluteX(mainElement);
        var controlY = ASPx.GetAbsoluteY(mainElement);

        return {
            x: x - controlX,
            y: y - controlY
        };
    },
    dispatchEvent: function(evt, paneType) {
        if(ASPx.Browser.TouchUI)
            paneType = this.getPaneManager().getPaneTypeByEvent(evt);
        var stateController = this.getStateController();
        if(stateController.isCellRangeSelectionInProcess() || stateController.isSelectionMovementInProcess())
            this.dispatchCellEventByPoint(evt, paneType);
        else if(stateController.isDrawingBoxDragingOrResizingInProcess())
            this.dispatchDrawingBoxEvent(evt, -1);
        else {
            var srcElement = ASPx.Evt.GetEventSource(evt);
            var isDrawingBoxElement = ASPx.ElementHasCssClass(srcElement, SpreadsheetCssClasses.DrawingBox);
            var isDrawingBoxSelectionElement = this.getIsDrawingBoxSelectionElement(srcElement);
            if((isDrawingBoxElement || isDrawingBoxSelectionElement)) {
				var drawingBoxIndex = this.getDrawingBoxIndex(srcElement);
                this.dispatchDrawingBoxEvent(evt, drawingBoxIndex);
            } else
                this.dispatchCellEventByPoint(evt, paneType);
        }
    },
    // Internal property setters
    setCurrentSheetIndex: function(sheetIndex) {
        this.sheetIndex = sheetIndex;
    },
    setOpenDocumentName: function(documentName) {
        this.documentName = documentName;
    },
    setDefaultCellSize: function(defaultCellSize) {
        if(defaultCellSize)
            this.defaultCellSize = defaultCellSize;
    },
    setLastFilledCell: function(lastFilledCell) {
        this.lastFilledCell = lastFilledCell;
    },
    isNewSheetLoaded: function(response) {
        return !!response.loadNewSheet;
    },

    // Active control
    activateControl: function() {
        ASPxClientSpreadsheet.activeSpreadsheetControl = this;
    },
    deactivateControl: function() {
        ASPxClientSpreadsheet.activeSpreadsheetControl = null;
    },

    // Drawing boxes
    getIsDrawingBoxSelectionElement: function (element) {
        return  ASPx.ElementHasCssClass(element, SpreadsheetCssClasses.DrawingBoxSelectedElement) ||
            ASPx.ElementHasCssClass(element.parentNode, SpreadsheetCssClasses.DrawingBoxSelectedElement);
    },
    getIsChartElement: function(element) {
        return ASPx.ElementContainsCssClass(element, SpreadsheetCssClasses.ChartElement);
    },
    getIsDrawingBoxElement: function(element) {
        return ASPx.ElementHasCssClass(element, ASPx.SpreadsheetCssClasses.DrawingBox)
    },
    getDrawingBoxByIndex: function(index) {
        var tilesContainer = this.getRenderProvider().getGridTilesContainer();
        if(tilesContainer) {
            var drawingBoxes = tilesContainer.getElementsByTagName("IMG");
            for(var i = 0; i < drawingBoxes.length; i++) {
                if(this.getDrawingBoxIndex(drawingBoxes[i]) == index)
                    return drawingBoxes[i];
            }
        }
        return null;
    },
    getDrawingBoxIndex: function(drawingBoxElement) {
        return parseInt(ASPx.Attr.GetAttribute(drawingBoxElement, "data-dbi"));
    },

    // Events
    onKeyDown: function(evt, editMode) {
        var editingWasNotStarted = editMode == ASPxClientSpreadsheet.StateController.Modes.Ready;

        this.keyboardManager.onKeyDown(this, evt, editMode);
        
        if(editingWasNotStarted)
            return this.onKeyDownPreventBrowserBack(evt);
    },
    //only for webKit
    onKeyPress: function(evt, editMode) {
        this.keyboardManager.onKeyPress(this, evt, editMode);
    },
    onMouseUp: function(evt) {
        this.dispatchEvent(evt);
        this.dispatchRowHeaderEvent(evt);
        this.dispatchColHeaderEvent(evt);
    },
    onKeyDownPreventBrowserBack: function(evt) {
        var keyCode = ASPx.Evt.GetKeyCode(evt);
        if(keyCode === ASPx.Key.Backspace)
            return ASPx.Evt.PreventEvent(evt);
    },
    onSheetIndexChanged: function(visibleIndex) {
        var index = this.getModelSheetIndex(visibleIndex);
        this.setSheetIndex(index);
        this.loadCurrentSheet();
    },
    dispatchCellEventByPoint: function(evt, paneType) {
        var srcElement = ASPx.Evt.GetEventSource(evt);
        var cellLayoutInfo = null;
        var eventFromCellTextViewElement = false;
        if(ASPx.Browser.IE && ASPx.Browser.Version < 9)
            paneType = this.getPaneManager().getPaneTypeByEvent(evt);
        if(srcElement === this.getCellTextViewElement()) {
            var currentSelection = this.getStateController().getSelection();
            cellLayoutInfo = this.getPaneManager().getCellLayoutInfo(currentSelection.activeCellColIndex, currentSelection.activeCellRowIndex);
            eventFromCellTextViewElement = true;
        }
        else {
            var point = this.getEventControlPoint(evt);
            cellLayoutInfo = this.getPaneManager().getCellByCoord(point.x, point.y, paneType);
        }
        var stateController = this.getStateController();
        stateController.onCellEvent(evt, cellLayoutInfo, eventFromCellTextViewElement, paneType);
    },
    dispatchDrawingBoxEvent: function(evt, drawingBoxIndex) {
        if(this.CanChangeSheetStylesAndLayout()) {
            var srcElement = ASPx.Evt.GetEventSource(evt);
            var stateController = this.getStateController();
            stateController.onDrawingBoxEvent(evt, srcElement, drawingBoxIndex);
        }
    },
    dispatchRowHeaderEvent: function(evt, paneType) {
        paneType = paneType ? paneType : ASPxClientSpreadsheet.PaneManager.PanesType.MainPane;
        this.getStateController().onHeaderEvent(evt, false, paneType);
    },
    dispatchColHeaderEvent: function(evt, paneType) {
        paneType = paneType ? paneType : ASPxClientSpreadsheet.PaneManager.PanesType.MainPane;
        this.getStateController().onHeaderEvent(evt, true, paneType);
    },

    onVisibleTileRangeChanged: function() {
        this.applyCurrentEdition();
    },

    cancelEditing: function() {
        this.getStateController().cancelEditing();
    },
    applyCurrentEdition: function() {
        this.getStateController().applyCurrentEdition();
    },

    // Tab Control
    getModelSheetIndex: function(index) {
        var tabControl = this.getTabControl();
        if (tabControl)
            return tabControl.getModelSheetIndex(index);
        return index;
    },
    setSheetIndex: function(index) {
        this.sheetIndex = index;
    },
    clearSheetIndex: function() {
        this.setSheetIndex(-1);
    },
    removeSheetWithConfirmation: function(confirmation) {
        if(window.confirm(confirmation))
            this.onServerCommand(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("RemoveSheet").id, { ForceRemove: true });
    },
                
    // Confirm message
    getConfirmUpdateText: function() { return this.confirmUpdate; },
    getUpdateWatcherHelper: function() {
        return this.updateWatcherHelper;
    },
    getModified: function() {
        return this.modified;
    },
    setModified: function(modified) {
        this.modified = modified;
    },
    setConfirmOnCallbackEnabled: function(enabled) {
        this.getUpdateWatcherHelper().SetConfirmOnCallbackEnabled(enabled);
    },
    setConfirmOnPostbackEnabled: function(enabled) {
        this.getUpdateWatcherHelper().SetConfirmOnPostbackEnabled(enabled);
    },
    
    // Chart support
    updateChartsInfo: function(chartsInfo) {
        this.chartsInfo = eval(chartsInfo);
    },
    getChartsInfo: function() {
        return this.chartsInfo; 
    },
    getChartInfoById: function(chartId) {
        var chartsInfo = this.getChartsInfo();
        for(var index = 0; index < chartsInfo.length; index++) {
            if (chartsInfo[index].Id == chartId)
                return chartsInfo[index];
        }
        return null;
    },
    getChartAttributeById: function(chartId, attributeName) {
        var chartInfo = this.getChartInfoById(chartId);
        if (chartInfo)
            return chartInfo[attributeName];
        return "";
    },
    getChartAttribute: function(chartInfo, attributeName) {
        if (chartInfo)
            return chartInfo[attributeName];
        return "";
    },
    
    // Resizing Control
    BrowserWindowResizeSubscriber: function() {
        return this.isInFullScreenMode || ASPxClientControl.prototype.BrowserWindowResizeSubscriber.call(this);
    },
    OnBrowserWindowResize: function (evt) {
        if(this.isInFullScreenMode)
            this.adjustControlInFullScreenMode();
        else 
            this.AdjustControl();
    },
    resizeColumn: function(index, width) {
        ASPxClientSpreadsheet.ServerCommands.ResizeColumn(this, { ColumnIndex: index, ColumnWidth: width });
    },
    resizeRow: function(index, height) {
        ASPxClientSpreadsheet.ServerCommands.ResizeRow(this, { RowIndex: index, RowHeight: height });
    },
    autoFitHeaderSize: function(isCol, index) {
        ASPxClientSpreadsheet.ServerCommands.AutoFitHeaderSize(this, { Index: index, IsColumn: isCol });
    },
    
    // Adjust Control
    IsAdjustmentRequired: function() {
        if(ASPxClientControl.prototype.IsAdjustmentRequired.call(this))
            return true;
        return this.getPaneManager().restoreScrollPositionRequired();
    },
    AdjustControlCore: function() {
        this.adjustRootControls(true);
    },
    adjustRootControls: function(updateScrollPosition) {
        if(this.getPaneManager().getFrozenPaneSettings().isFrozen)
            this.getRenderProvider().adjustFreezePanes();
        else
            this.getRenderProvider().adjustControl();

        if(updateScrollPosition)
            this.getPaneManager().updateScrollPosition();

        if(this.showFormulaBar)
            this.adjustFormulaBar();
    },
    adjustFormulaBar: function() {
        var formulaBar = this.getRenderProvider().getFormulaBar();
        var formulaBarTextBoxElement = this.getRenderProvider().getFormulaBarTextBoxElement();
        var formulaBarMenuElement = this.getFormulaBarMenu().GetMainElement();
        formulaBarTextBoxElement.style.width = formulaBar.offsetWidth - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(formulaBar)
            - formulaBarMenuElement.offsetWidth + "px";
        var isFormulaBarEditing = this.isFormulaBarEditMode();
        var elementInFormulaBar = isFormulaBarEditing ? this.getEditingHelper().getEditorElement() : this.getCellTextViewElement();
        this.locateElementAboveFBPlaceholder(elementInFormulaBar);
    },
    locateElementAboveFBPlaceholder: function(element) {
        var fbEditorPlaceholder = this.getRenderProvider().getFormulaBarEditorPlaceholder();
        element.style.width = fbEditorPlaceholder.offsetWidth - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(fbEditorPlaceholder) + "px";
        element.style.height = fbEditorPlaceholder.offsetHeight - ASPx.GetTopBottomBordersAndPaddingsSummaryValue(fbEditorPlaceholder) + "px";
        ASPx.SetElementDisplay(element, true);
        ASPxClientUtils.SetAbsoluteX(element, Math.floor(ASPxClientUtils.GetAbsoluteX(fbEditorPlaceholder)));
        ASPxClientUtils.SetAbsoluteY(element, Math.floor(ASPxClientUtils.GetAbsoluteY(fbEditorPlaceholder)));
    },
    adjustControlInFullScreenMode: function() {
        var correctHeight = 0;
        if(this.isRibbonExternal()) {
            var ribbonManager = this.getRibbonManager();
            correctHeight = ribbonManager.GetRibbonHeight();
        }
        this.setWidthInternal(ASPx.GetDocumentClientWidth(), true);
        this.setHeightInternal(ASPx.GetDocumentClientHeight() - correctHeight, true);
        this.adjustRootControls();
    },
    adjustRibbonControl: function() {
        var ribbonControl = this.GetRibbon();
        if(ribbonControl)
            ribbonControl.AdjustControl();
    },
    adjustTabControl: function() {
        var tabControl = this.getTabControl();
        if(tabControl)
            tabControl.AdjustControl();
    },
    
    // Sizes
    setWidthInternal: function(width, skipAdjustControl) {
        if(!ASPx.IsNumber(width) && ASPx.IsPercentageSize(width))
            this.setSizeInPercentage("width", width);
        else
            ASPxClientControl.prototype.SetWidth.call(this, width);
        
        if(!skipAdjustControl)
            this.AdjustControl();
        this.adjustRibbonControl();
        this.adjustTabControl();
    },
    setHeightInternal: function(height, skipAdjustControl) {
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
    SetWidth: function(width) {
        ASPxClientControl.prototype.SetWidth.call(this, width);
        this.adjustRibbonControl();
        this.adjustTabControl();
        this.AdjustControl();
    },
    SetHeight: function(height, skipAdjustControl) {
        ASPxClientControl.prototype.SetHeight.call(this, height);
        this.AdjustControl();
    },
    getTabControlHeight: function() {
        var height = 0;
        var tabControl = this.getTabControl();
        if (ASPx.IsExists(tabControl))
            height = tabControl.GetHeight();
        return height;
    },
    setTabControlWidth: function(width) {
        var tabControl = this.getTabControl();
        if (ASPx.IsExists(tabControl))
            tabControl.SetWidth(width);
    },
    setRibbonWidth: function(width) {
        var ribbonControl = this.GetRibbon();
        if (ribbonControl)
            ribbonControl.SetWidth(width);
    },
    getInternalRibbonControlHeight: function() {
        if (!this.isRibbonExternal()) {
            var ribbonControl = this.GetRibbon();
            if (ribbonControl)
                return ribbonControl.GetHeight();
        }
        return 0;
    },
    getFormulaBarHeight: function() {
        var formulaBar = this.getRenderProvider().getFormulaBar();
        return  formulaBar ? formulaBar.offsetHeight : 0;
    },
    getMainElementHeightByServerValue: function() {
        var mainElement = this.GetMainElement();
        var height = this.GetHeight() - ASPx.GetTopBottomBordersAndPaddingsSummaryValue(mainElement);
        var innerControlsHeight = this.getInternalRibbonControlHeight() + this.getTabControlHeight() + this.getFormulaBarHeight();
        height = height - innerControlsHeight;
        if (height < 0)
            height =  innerControlsHeight;
        return height;
    },
    getActiveCellSize: function(isCol) {
        var currentSelection = this.getSelectionInternal(),
            cellIndex = isCol ? currentSelection.activeCellColIndex : currentSelection.activeCellRowIndex;
        return this.getPaneManager().getCellSize(cellIndex, isCol);
    },
   
    //Loading panel
    ShowLoadingPanel: function() {
        this.CreateLoadingPanelWithAbsolutePosition(this.GetMainElement(), this.GetLoadingPanelOffsetElement(this.getRenderProvider().getWorkbookControl()));
    },
    ShowLoadingDiv: function () {
        this.CreateLoadingDiv(this.getRenderProvider().getWorkbookControl().parentNode, this.getRenderProvider().getWorkbookControl());
    },

    // Messages
    showMessageBox_EditProtectedCell: function() {
        this.showLocalizedMessageBox("Msg_CellOrChartIsReadonlyShort");
    },
    showLocalizedMessageBox: function(XtraSpreadsheetStringId) {
        var localizedString = ASPxClientSpreadsheet.localizedStringHelper.getString(XtraSpreadsheetStringId);
        if(!localizedString)
            ASPxClientSpreadsheet.ServerCommands.GetLocalizedStringConstant(this, {XtraSpreadsheetStringId: XtraSpreadsheetStringId, CustomResultProcessingFunction: "showLocalizedMessageBoxFromServer"});
        else 
            alert(localizedString);
    },
    showLocalizedMessageBoxFromServer: function(response) {
        ASPxClientSpreadsheet.localizedStringHelper.setString(response["XtraSpreadsheetStringId"], response["XtraSpreadsheetString"]);
        this.showLocalizedMessageBox(response["XtraSpreadsheetStringId"]);
    },
       
    // PopupMenu support
    onKBContextMenu: function(e) {
        var selection = this.getSelectionInternal(),
            drawingBoxElement = selection.drawingBoxElement,
            xPos,
            yPos;

        if(drawingBoxElement) {
            xPos = ASPx.GetAbsolutePositionX(drawingBoxElement) + drawingBoxElement.clientWidth;
            yPos = ASPx.GetAbsolutePositionY(drawingBoxElement) + drawingBoxElement.clientHeight;
        }
        else {
            var activeCellLayoutInfo = this.getPaneManager().getCellLayoutInfo(selection.activeCellColIndex, selection.activeCellRowIndex),
                rect = activeCellLayoutInfo.rect,
                tileX = ASPx.GetAbsolutePositionX(activeCellLayoutInfo.tileInfo.htmlElement),
                tileY = ASPx.GetAbsolutePositionY(activeCellLayoutInfo.tileInfo.htmlElement);

            xPos = tileX + rect.x + rect.width;
            yPos = tileY + rect.y + rect.height;
        }

        this.showPopupMenu(e, {
            x: this.correctPopupMenuPosition(xPos, true),
            y: this.correctPopupMenuPosition(yPos, false)
        });
    },
    correctPopupMenuPosition: function(coord, isX) {
        var gridPosition = this.getPaneManager().getCellsGridAbsolutePosition(),
            lowerBound = gridPosition.top,
            upperBound = gridPosition.bottom;

        if(isX) {
            lowerBound = gridPosition.left;
            upperBound = gridPosition.right;
        }

        if(coord < lowerBound)
            coord = lowerBound;
        else if(coord > upperBound)
            coord = upperBound;

        return coord;
    },
    showPopupMenu: function(e, context) {
        var selection = this.getSelectionInternal(),
            drawingBoxElement = selection.drawingBoxElement;

        context.mouseX = context.x || ASPx.Evt.GetEventX(e);
        context.mouseY = context.y || ASPx.Evt.GetEventY(e);

        context.isRowHeader = context.isRowHeader || false;
        context.isColumnHeader = context.isColumnHeader || false;

        context.isDrawing = selection.drawingBoxIndex > -1;
        context.isChart = this.getIsChartElement(selection.drawingBoxElement);

        if(!context.isDrawing)
            context.isLink = ASPx.IsExists(ASPx.GetNodeByTagName(this.getRenderProvider().getActiveCellElement(), "a", 0));

        if(drawingBoxElement) {
            var chartIndex = ASPx.Attr.GetAttribute(drawingBoxElement, "data-dbi");
            var chartInfo = this.getChartInfoById(chartIndex);

            context.hasTitle = this.getChartAttribute(chartInfo, "Title") !== "";
            context.hasHTitle = this.getChartAttribute(chartInfo, "HAxisTitle") !== "";
            context.hasVTitle = this.getChartAttribute(chartInfo, "VAxisTitle") !== "";
        }

        context.readOnly = this.readOnly;
        context.isArrangeEnabled = this.isArrangeCommandEnabled();

        this.getPopupMenuHelper().showPopupMenu(context);
    },
    isArrangeCommandEnabled: function() {
        return ASPx.GetNodesByClassName(this.getRenderProvider().getWorkbookControl(), SpreadsheetCssClasses.DrawingBox).length > 1;
    },

    // Ribbon update support
    onRibbonFillItemsNeedToUpdate: function(selection) {
        this.getRibbonManager().onFillItemsChanged(selection.range.bottomRowIndex + selection.range.topRowIndex != 0, 
                                                        selection.range.leftColIndex + selection.range.rightColIndex != 0);
        this.updateFillGroup = selection.range.bottomRowIndex + selection.range.topRowIndex == 0 || selection.range.leftColIndex + selection.range.rightColIndex == 0;
    },
    onEditingStarted: function() {
        if(this.showFormulaBar)
            this.changeFormulaBarMenuItemsEnabled(true);
        this.ribbonStateUpdate("onEditingStarted");
    },
    onEditingStopped: function() {
        if(this.showFormulaBar)
            this.changeFormulaBarMenuItemsEnabled(false);
        this.ribbonStateUpdate("onEditingStopped");
    },
    changeFormulaBarMenuItemsEnabled: function(enabled) {
        var formulaBarMenu = this.getFormulaBarMenu();
        for(var i = 0; i < formulaBarMenu.GetItemCount(); i++)
            formulaBarMenu.GetItem(i).SetEnabled(enabled);
    },

    switchReadOnlyMode: function() {
        this.ribbonStateUpdate("onItemsEnabledChanged");
    },
    onSheetLockedChanged: function() {
        this.ribbonStateUpdate("onItemsEnabledChanged");
    },
    onWorkbookLockedChanged: function() {
        this.ribbonStateUpdate("onItemsEnabledChanged");
    },
    ribbonStateUpdate: function(methodName) {
        if(this.enabled) {
            this.getRibbonManager()[methodName]();
        }
    },
    isRibbonExternal: function() {
        return this.ribbonClientID != "";
    },
    
    // Selection
    setSelection: function(leftColIndex, topRowIndex, rightColIndex, bottomRowIndex) {
        this.getSelectionHelper().setSelectionCells(leftColIndex, topRowIndex, rightColIndex, bottomRowIndex);
    },
    onSelectionChanged: function() {
        var selection = this.getStateController().getSelection();
        this.getPaneManager().highlightHeaders(selection);
        if(this.isActiveCellChanged(selection.activeCellColIndex, selection.activeCellRowIndex))
            this.onActiveCellChanged(selection);

        if (this.updateFillGroup || (selection.range.bottomRowIndex + selection.range.topRowIndex == 0) || (selection.range.leftColIndex + selection.range.rightColIndex == 0))
            this.onRibbonFillItemsNeedToUpdate(selection);
        else
            this.updateFillGroup = false;

        this.ribbonStateUpdate("onSelectionChanged");
    },
    onActiveCellChanged: function(selection) {
        this.updateActiveCell(selection.activeCellColIndex, selection.activeCellRowIndex);
        this.getValidationHelper().showValidationElements(selection.activeCellColIndex, selection.activeCellRowIndex);

        this.updateCellTextViewElementValue();
        this.ribbonStateUpdate("onActiveCellChanged");
    },
    isActiveCellChanged: function(colIndex, rowIndex) {
        return this.currentActiveCell.col != colIndex || this.currentActiveCell.row != rowIndex;
    },
    updateActiveCell: function(colIndex, rowIndex) {
        this.currentActiveCell.col = colIndex;
        this.currentActiveCell.row = rowIndex;  
    },
    getSelectionActiveCellLayoutInfoInternal: function(selection) {
        return this.getPaneManager().getCellLayoutInfo(selection.activeCellColIndex, selection.activeCellRowIndex);
    },
    onCellselected: function(stateController) {
        this.selectedCellStateController = stateController;
    },
    onCellValueChangedWithNewSelection: function(cellPosition, newValue) {
        this.cellUpdateCore(cellPosition, newValue, true);
    },
    onCellValueChanged: function(cellPosition, newValue) {
        this.cellUpdateCore(cellPosition, newValue, false);
    },
    cellUpdateCore: function(cellPosition, newValue, reselectAfterCommand){
        newValue = this.prepareCellValueForCore(newValue); // TODO Remove when core could take only "\n" separator
        ASPxClientSpreadsheet.ServerCommands.CellUpdate(this, 
            {
                CellPositionColumn:cellPosition.Column, 
                CellPositionRow:cellPosition.Row, 
                NewValue:ASPx.Str.EncodeHtml(newValue),
                ReselectAfterCommand:reselectAfterCommand
            }
        );
    },
    prepareCellValueForCore: function(value) {
        return value.replace(/\n/g, "\r\n");
    },
    getSelectionInternal: function(){
        var stateController = this.getStateController();
        return stateController.getSelection();
    },
    getModelExpandedSelectionInternal: function(){
        var internalSelection = this.getSelectionInternal();
        
        var paneManager = this.getPaneManager();
        var internalSelectionWithModelIndices = internalSelection.getConvertedToModelIndices(paneManager);
        internalSelectionWithModelIndices.expandToAllSheetIfRequired();

        return internalSelectionWithModelIndices;
    },
    GetSelection: function() {
        return this.getClientSelection();
    },
    getClientSelection: function(internalSelection) {
        internalSelection = internalSelection || this.getSelectionInternal();
        return this.convertInternalSelectionToClientSelection(internalSelection);
    },
    convertInternalSelectionToClientSelection: function(internalSelection) {
        var paneManager = this.getPaneManager();

        var internalSelectionWithModelIndices = internalSelection.getConvertedToModelIndices(paneManager);
        internalSelectionWithModelIndices.expandToAllSheetIfRequired();

        return new ASPxClientSpreadsheet.ASPxClientSpreadsheetSelection(
            internalSelectionWithModelIndices.activeCellColIndex,
            internalSelectionWithModelIndices.activeCellRowIndex,

            internalSelectionWithModelIndices.range.leftColIndex,
            internalSelectionWithModelIndices.range.topRowIndex,
            internalSelectionWithModelIndices.range.rightColIndex,
            internalSelectionWithModelIndices.range.bottomRowIndex
        );
    },
    DoRaiseSelectionChanged: function(internalSelection) {
        var clientSelection = this.getClientSelection(internalSelection);
        this.RaiseSelectionChanged(clientSelection);
    },
    RaiseSelectionChanged: function(clientSelection) {
        if(!this.SelectionChanged.IsEmpty()){
            var args = new ASPxClientSpreadsheetSelectionChangedEventArgs(clientSelection);
            this.SelectionChanged.FireEvent(this, args);
        }
    },
    getCellValueByVisiblePosition_Internal: function(colVisibleIndex, rowVisibleIndex) {
        var paneManager = this.getPaneManager();
        var colModelIndex = paneManager.convertVisibleIndexToModelIndex(colVisibleIndex, true);
        var rowModelIndex = paneManager.convertVisibleIndexToModelIndex(rowVisibleIndex, false);
        return this.GetCellValue(colModelIndex, rowModelIndex);
    },
    setGridLinesVisibility: function(visible) {
        var mainSheetDiv = this.getRenderProvider().getWorkbookControl();
        if (mainSheetDiv) {
            var method = visible ? ASPx.RemoveClassNameFromElement : ASPx.AddClassNameToElement;
            method(mainSheetDiv, SpreadsheetCssClasses.GridLineHidden);
        }
    },

    // Scrolling and navigation
    changeSelectionAccordingToKeyboardAction: function(method) {
        var stateController = this.getStateController();
        var selectionHelper = this.getSelectionHelper();

        var selection = stateController.getSelection().clone();
        
        selectionHelper.calcActualCellSelection(selection);
        if(!selection.isDrawingBoxSelection()) {
            var changedSelection = ASPxClientSpreadsheet.SelectionKeyboardUtils[method](selection, selectionHelper);

            var inCallback = this.InCallback();
            if(!inCallback) {
                var cellScrollVisible = this.getPaneManager().navigateTo(stateController, changedSelection);
                if(cellScrollVisible)
                    stateController.setSelection(changedSelection);
            }
        }
    },
    scrollToViaServer: function(colModelIndex, rowModelIndex, selectAfterScroll) {
        ASPxClientSpreadsheet.ServerCommands.ScrollTo(this, {CellPositionColumn: colModelIndex, CellPositionRow: rowModelIndex, SelectAfterScroll: !!selectAfterScroll});
    },
    onHyperLinkClicked: function(evt, element) {
        var handled = this.RaiseHyperlinkClick(evt, element);

        if(!handled) {
            var url = ASPx.Attr.GetAttribute(element, "_loc");
            if(!ASPx.IsUrlContainsClientScript(url))
                window.open(url, '_blank');
        }
        return true;
    },
    RaiseHyperlinkClick: function(evt, element) {
        var handled = false;
        if(!this.HyperlinkClick.IsEmpty()){
            var url = ASPx.Attr.GetAttribute(element, "_loc"),
                isExternal = !!url.match(/^(https?:|ftps?:|^)/gi);
            var args = new ASPxClientSpreadsheetHyperlinkClickEventArgs(evt, handled, url, isExternal);
            this.HyperlinkClick.FireEvent(this, args);
            handled = args.handled
        }
        return handled;
    },
    // Find
    findAllFromDialog: function(dialogInstance, findWhat, matchCase, matchEntireCellContent, searchBy, lookIn) {
        this.findAllDialogInstance = dialogInstance;
        this.findAll(findWhat, matchCase, matchEntireCellContent, searchBy, lookIn);
    },
    findAll: function(findWhat, matchCase, matchEntireCellContent, searchBy, lookIn) {
        ASPxClientSpreadsheet.ServerCommands.FindAll(this, 
            {
                FindWhat:   findWhat,
                MatchCase:  !!matchCase,
                MatchEntireCellContent: !!matchEntireCellContent,
                SearchBy:   searchBy || "",
                LookIn:     lookIn || ""
            });
    },
    OnFindAllCommandResultProcessing: function(findAllCommandResponse) {
        if(findAllCommandResponse.FindAllResult) {
            if(this.findAllDialogInstance) {
                this.findAllDialogInstance.OnSearchResultsReceived(findAllCommandResponse.FindAllResult.findAllList, findAllCommandResponse.FindAllResult.findNextCellModelPosition);
                this.findAllDialogInstance = null;
            } //else
            //window.fakeOnFindAllCommandResultProcessing(findAllCommandResponse.FindAllResult);
        }
    },

    fetchAutoFilterViewModelFromDialog: function(dialogInstance) {
        this.autoFilterDialogInstance = dialogInstance;
        ASPxClientSpreadsheet.ServerCommands.FetchAutoFilterViewModel(this,
            {
                filterCommandId: dialogInstance.commandId
            });
    },
    OnAutoFilterViewModelReceived: function(response) {
        if(response.autoFilterViewModel && this.autoFilterDialogInstance)
            this.autoFilterDialogInstance.OnViewModelReceived(response.autoFilterViewModel);
        this.autoFilterDialogInstance = null;
    },

    fetchPageSetupViewModelFromDialog: function(dialogInstance) {
        this.pageSetupDialogInstance = dialogInstance;
        ASPxClientSpreadsheet.ServerCommands.FetchPageSetupViewModel(this, {});
    },
    OnPageSetupViewModelReceived: function(response) {
        if(response.pageSetupViewModel && this.pageSetupDialogInstance)
            this.pageSetupDialogInstance.OnViewModelReceived(response.pageSetupViewModel);
        this.pageSetupDialogInstance = null;
    },

    fetchDataValidationViewModelFromDialog: function(dialogInstance) {
        this.dataValidationDialogInstance = dialogInstance;
        ASPxClientSpreadsheet.ServerCommands.FetchDataValidationViewModel(this, {});
    },
    OnDataValidationViewModelReceived: function(response) {
        if(response.ViewModel && this.dataValidationDialogInstance)
            this.dataValidationDialogInstance.OnViewModelReceived(response.ViewModel);
        this.dataValidationDialogInstance = null;
    },
    fetchListAllowedValues: function() {
        ASPxClientSpreadsheet.ServerCommands.FetchListAllowedValues(this, {});
    },
    fetchMessageForCell: function() {
        ASPxClientSpreadsheet.ServerCommands.FetchMessageForCell(this, {});
    },
    OnListAllowedValuesReceived: function(response) {
        this.getValidationHelper().onAllowedValuesReceived(response);
    },
    OnMessageForCellReceived: function(response) {
        this.getValidationHelper().onMessageForCellReceived(response);
    },

    RaiseDocumentChanged: function() {
        if(!this.DocumentChanged.IsEmpty()){
            var args = new ASPxClientSpreadsheetDocumentChangedEventArgs();
            this.DocumentChanged.FireEvent(this, args);
        }
    },
    RaiseBeginSynchronization: function() {
        if(!this.BeginSynchronization.IsEmpty()){
            var args = new ASPxClientSpreadsheetSynchronizationEventArgs();
            this.BeginSynchronization.FireEvent(this, args);
        }
    },
    RaiseEndSynchronization: function() {
        if(!this.EndSynchronization.IsEmpty()){
            var args = new ASPxClientSpreadsheetSynchronizationEventArgs();
            this.EndSynchronization.FireEvent(this, args);
        }
    },
    HasUnsavedChanges: function() {
        return this.getModified();
    },
    GetCellValue: function(colModelIndex, rowModelIndex){
        return this.getPaneManager().getCellValue(colModelIndex, rowModelIndex);
    },
    GetActiveCellValue: function(){
        var paneManager = this.getPaneManager(),
            selectionInternal = this.getSelectionInternal(),
            internalSelectionWithModelIndices = selectionInternal.getConvertedToModelIndices(paneManager);

        return this.GetCellValue(internalSelectionWithModelIndices.activeCellColIndex, internalSelectionWithModelIndices.activeCellRowIndex);
    },
    PerformCallback: function (parameter) {
        this.performCallbackCore(parameter, ASPxClientSpreadsheet.CallbackPrefixes.PerformCallbackPrefix);
    },
    PerformDocumentCallback: function(parameter) {
        this.performCallbackCore(parameter, ASPxClientSpreadsheet.CallbackPrefixes.DocumentCallbackPrefix);
    },
    performCallbackCore: function(parameter, argumentPrefix) {
        parameter = ASPx.IsExists(parameter) ? parameter.toString() : "";
        var formatArg = this.formatCallbackArg(argumentPrefix, parameter);
        this.CreateCallback(formatArg);
    },
    formatCallbackArg: function(prefix, arg) { // TODO REFACTOR with ASPxGridView, replace to Callback.js
        arg = arg.toString();
        return (ASPx.IsExists(arg) ? prefix + "|" + arg.length + ';' + arg + ';' : "");
    },
    ReconnectToExternalRibbon: function() {
        if (this.isRibbonExternal()) {
            this.detachEventsFromRibbon();
            this.attachEventsToRibbon();
        }
    }
});

ASPxClientSpreadsheet.ChildControlIdPostfixes = {
    inputTargetElement: "_IT",
    ribbonControl: "SSRC",
    popupMenu: "SSPUM",
    autoFilterPopupMenu: "SSAFPM",
    dialogPopupControl: "SSDC",
    tabControl: "SSTC",
    workbookPostfix: "WBC",
    columnHeaderDivPostfix: "ColumnHeader",
    rowHeaderDivPostfix: "RowHeader",
    gridContainerDivPostfix: "Grid",
    scrollContainerDivPostfix: "ScrollContainer",
    scrollContentDivPostfix: "ScrollContent",
    supportFrame: "DXSpreadsheetSupporter",
    formulaBar: "SSFB",
    formulaBarTextBox: "SSFBTB",
    formulaBarMenu: "SSFBM",
    functionsListBox: "SSFLB",
    frozenPanePostfix: "FP",
    bottomLeftPanePostfix: "BL",
    topRightPanePostfix: "TR",
    resizeGrip: "RG"
};

ASPxClientSpreadsheet.CallbackPrefixes = {
    DocumentUpdateCallbackPrefix: "SSDU",

    PerformCallbackPrefix: "SSPC",
    DocumentCallbackPrefix: "SSDC",
    
    InternalCallBackPostfix: "%Spread%Sheet",
    FileManagerCallbackPrefix: "SSFM",

    OpenFileCallbackPrefix : "SSOF",
    SaveFileCallbackPrefix : "SSSF",
    NewFileCallbackPrefix  : "SSNF",

    SaveImageCallbackPrefix : "SSITS",
    SaveImageSuccessCallbackPrefix : "ISU",
    SaveImageErrorCallbackPrefix : "ISE"
};

ASPxClientSpreadsheet.Constants = {
    RibbonHeightInRows: 7,
    FormulaBarMenuEnterItemName: "Enter",
    FormulaBarMenuCancelItemName: "Cancel"
};

ASPxClientSpreadsheet.localizedStringHelper = (function(){
    var stringHash = {};
    return { 
        getString: function(stringID) {
            return stringHash[stringID];
        }, 
        setString: function(stringID, value) {
            stringHash[stringID] = value;
        }
    };
})();

ASPx.SSMenuItemClick = function(name, commandId, params) {
    if(params.length == 0) {
        var spreadsheet = ASPx.GetControlCollection().Get(name);
        spreadsheet.onMenuCommand(commandId);
    }
};

ASPx.SSFormulaBarMenuItemClick = function(name, menuItemName) {
    var spreadsheet = ASPx.GetControlCollection().Get(name);
    if(menuItemName === ASPxClientSpreadsheet.Constants.FormulaBarMenuEnterItemName)
        spreadsheet.getStateController().applyCurrentEdition();
    if(menuItemName === ASPxClientSpreadsheet.Constants.FormulaBarMenuCancelItemName)
        spreadsheet.getStateController().onEscape();
};

ASPx.SSOnHyperLinkClicked = function(evt, element) {
    var cellContainer = ASPx.GetParentByClassName(element, SpreadsheetCssClasses.TextBoxContent);
    if(cellContainer) {
        var spreadsheetControlName = cellContainer.id.substr(0, cellContainer.id.indexOf("tb.") - 1),
            spreadsheet = ASPx.GetControlCollection().Get(spreadsheetControlName);
        if(spreadsheet) {
            return spreadsheet.onHyperLinkClicked(evt, element);
        }
    }
    return true;
};

ASPx.SSMenuCloseUp = function(name) {
    var spreadsheet = ASPx.GetControlCollection().Get(name);
    var inputController = spreadsheet.getInputController();
    if(inputController)
        inputController.captureFocus();
};

ASPx.Evt.AttachEventToDocument(ASPx.TouchUIHelper.touchMouseUpEventName, function(e) {
    if(ASPxClientSpreadsheet.activeSpreadsheetControl) {
        ASPxClientSpreadsheet.activeSpreadsheetControl.onMouseUp(e);
    }
    if(ASPxClientSpreadsheet.activeResizingHelper != null) {
        ASPxClientSpreadsheet.activeResizingHelper.onMouseUp(e);
        return true;
    }
});
if(ASPx.Browser.WebKitTouchUI) {
    ASPx.TouchUIHelper.AttachDoubleTapEventToElement(document, function(e) {
        var activeControl = ASPxClientSpreadsheet.activeSpreadsheetControl;
        if(activeControl && ASPx.GetIsParent(activeControl.getRenderProvider().getWorkbookControl(), e.target))
            activeControl.dispatchEvent(e);
    });
}

var SpreadsheetUpdateWatcherHelper = ASPx.CreateClass(ASPx.UpdateWatcherHelper, {
    constructor: function(owner) {
        this.constructor.prototype.constructor.call(this, owner);
        this.spreadsheet = owner;
        this.confirmOnCallback = false;
        this.confirmOnPostback = false;
    },
    HasChanges: function() {
        return this.spreadsheet.getModified();
    },
    NeedConfirmOnCallback: function(dxCallbackOwner, arg) {
        return this.confirmOnCallback && !this.spreadsheet.isInternalServiceCallback(arg);
    },
    NeedConfirmOnPostback: function() {
        return this.confirmOnPostback;
    },
    GetConfirmUpdateText: function() {
        return this.spreadsheet.getConfirmUpdateText();
    },

    SetConfirmOnCallbackEnabled: function(enabled) {
        this.confirmOnCallback = enabled;
    },
    SetConfirmOnPostbackEnabled: function(enabled) {
        this.confirmOnPostback = enabled;
    }
});

ASPx.SpreadsheetCssClasses = SpreadsheetCssClasses;

window.ASPxClientSpreadsheet = ASPxClientSpreadsheet;
window.ASPxClientSpreadsheetCustomCommandExecutedEventArgs = ASPxClientSpreadsheetCustomCommandExecutedEventArgs;

})();