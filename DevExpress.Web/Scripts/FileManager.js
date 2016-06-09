/// <reference path="_references.js"/>

(function() {
var FileManagerConsts = {};
FileManagerConsts.SplitterPostfix = "_Splitter";
FileManagerConsts.ItemsPostfix = "_Items";
FileManagerConsts.ToolbarPostfix = "_Toolbar";
FileManagerConsts.ContextMenuPostfix = "_ContextMenu";
FileManagerConsts.FoldersPostfix = "_Folders";
FileManagerConsts.BreadCrumbsPostfix = "_BreadCrumbs",
FileManagerConsts.GridPostfix = "_FilesGridView";
FileManagerConsts.UploadPostfix = "_Upload";
FileManagerConsts.UploadButtonPostfix = "_UploadButton";
FileManagerConsts.RenameFileInputPostfix = "_RFI";
FileManagerConsts.FolderBrowserPopupPostfix = "_FolderBrowserPopup";
FileManagerConsts.UploadProgressPopupPostfix = "_UploadProgressPopup";
FileManagerConsts.BreadCrumbsPopupPostfix = "_BreadCrumbsPopup";
FileManagerConsts.BreadCrumbsFolderUpButtonPostfix = "_FUB";
FileManagerConsts.BreadCrumbsHiddenItemsButtonPostfix = "_HIB";
FileManagerConsts.ItemPostfix = "_I";
FileManagerConsts.SeparatorPostfix = "S";
FileManagerConsts.ImagePostfix = "I";
FileManagerConsts.CheckboxPostfix = "_CHK";
FileManagerConsts.FolderBrowserFoldersContainerPostfix = "_FC";
FileManagerConsts.FolderBrowserFolders = "_FolderBrowserFolders";
FileManagerConsts.FolderBrowserDialogOkButtonPostfix = "_OkB";
FileManagerConsts.FolderBrowserDialogCancelButtonPostfix = "_CaB";
FileManagerConsts.UploadProgressBarPostfix = "_PB";
FileManagerConsts.UploadPopupCancelButtonPostfix = "_CB";
FileManagerConsts.ItemsPaneSplitterName = "ContentPane2";
FileManagerConsts.CreateHelperFolderPostfix = "CHF";
FileManagerConsts.VirtualScrollingEmptyPageElementPostifx = "_EP";

FileManagerConsts.ToolbarStandardButtonPrefix = "fmtsi-";

FileManagerConsts.FileClassName = "dxfm-file";
FileManagerConsts.FileContainerClassName = "dxfm-fileContainer";
FileManagerConsts.FolderContentContainerClassName = "dxtv-nd";
FileManagerConsts.UploadControlDisableClassName = "dxfm-uploadDisable";
FileManagerConsts.MultiSelectClassName = "dxfm-multiSelect";
FileManagerConsts.ItemMaskClassName = "dxfm-itemMask";
FileManagerConsts.BreadCrumbsContainerClassName = "dxfm-bcContainer";
FileManagerConsts.BreadCrumbsLineSeparatorClassName = "dxfm-bcLineSeparator";
FileManagerConsts.BreadCrumbsButtonClassName = "dxfm-bcButton";
FileManagerConsts.BreadCrumbsLastItemClassName = "dxfm-bcLastItem";
FileManagerConsts.ThumbnailCellClassName = "dxfm-fileThumb";
FileManagerConsts.GridColumnTitleCellClassName = "dxfm-fileNameCell";
FileManagerConsts.GridColumnTitleClassName = "dxfm-fileName";
FileManagerConsts.ShowAllFileAreaCheckBoxesClassName = "dxfm-faShowCheckBoxes";
FileManagerConsts.CreateHelperElementClassName = "dxfm-createHelper";

FileManagerConsts.CallbackArgumentSeparator = "|";
FileManagerConsts.CallbackLinkedArgumentSeparator = "||ls||";
FileManagerConsts.PathSeparator = "\\";
FileManagerConsts.ItemPropertyValueSeparator = "::";

FileManagerConsts.ParentFolderItemName = "..";
FileManagerConsts.CreateHelperItemName = "aspxFMCreateHelperFolder";
FileManagerConsts.BreadCrumbsHiddenItemsButtonText = "...";

FileManagerConsts.RenameInputAdditionalWidth = 20;
FileManagerConsts.DefaultRenameInputWidth = 150;

FileManagerConsts.StateField = {
    CurrentPath: "currentPath",
    ItemFilter: "item.filter",
    ItemSelected: "item.selected",
    ItemFocused: "item.focused",
    SplitterState: "splitter",
    IsFolderNodeExpanded: "isFolderNodeExpanded",
    VirtScrollItemIndexFieldName: "virtScrollItemIndex",
    VirtScrollPageItemsCountFieldName: "virtScrollPageItemsCount"
}
FileManagerConsts.ToolbarName = {
    Delete: "Delete",
    Move: "Move",
    Refresh: "Refresh",
    Rename: "Rename",
    Create: "Create",
    Download: "Download",
    Upload: "Upload",
    Copy: "Copy"
}
FileManagerConsts.CallbackCommandId = {
    GetFileList: 0,
    Refresh: 1,
    DeleteItems: 2,
    RenameItem: 3,
    ShowFolderBrowserDialog: 4,
    MoveItems: 5,
    CreateQuery: 6,
    Create: 7,
    FoldersTvCallback: 8,
    FolderBrowserFoldersTvCallback: 9,
    Download: 10,
    ServerProcessFileOpened: 11,
    GridView: 12,
    ChangeFolderTvCallback: 13,
    CopyItems: 14,
    CustomCallback: 15,
    ChangeCurrentFolderCallback: 16,
    VirtualScrollingCallback: 17,
    GridViewVirtualScrollingCallback: 18,
    ApiCommand: 19
};
FileManagerConsts.SelectedArea = {
    Folders: 0,
    Files: 1,
    None: 2
};
FileManagerConsts.SelectedAreaNames = {
    0: "Folders",
    1: "Files",
    2: "None"
};
FileManagerConsts.FileAreaItemTypes = {
    File: "File",
    Folder: "Folder",
    ParentFolder: "ParentFolder",
    CreateHelperFolder: "aspxFMCreateHelperFolder"
};
FileManagerConsts.Rights = {
    Default: 0,
    Allow: 1,
    Deny: 2
};

FileManagerConsts.ViewMode = {
    Thumbnail: 0,
    Grid: 1
};
FileManagerConsts.ModifierKey = {
    None: 0,
    Shift: 1,
    Ctrl: 2
};

FileManagerConsts.Templates = {};
FileManagerConsts.Templates.Item =
    "<div id=\"{{itemId}}\">" +
        "<div>" +
            "{{fileImage}}" +
            "<br/>" +
            "<div class=\"dxfm-itemNameContainer\">{{fileName}}</div>" +
        "</div>" +
    "</div>";

FileManagerConsts.Templates.EmptyPageElement = "<div id='{{id}}' class='dxfm-epe' style='height: {{height}}'></div>";
FileManagerConsts.Templates.HighlightedText = "{{textStart}}<span class=\"{{highlightCssClass}}\">{{textMiddle}}</span>{{textEnd}}";
var ASPxClientFileManager = ASPx.CreateClass(ASPxClientControl, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);

        this.styleSheet = ASPx.GetCurrentStyleSheet();

        this.elements = new ASPxClientFileManager.ElementsHelper(this);

        if(this.elements.filterElement)
            this.filter = new ASPxClientFileManager.FilterHelper(this);

        this.items = {};
        this.itemElementsIds = {};

        // Thumbnails
        this.noThumbnailImage = "";
        this.customThumbnails = [];

        this.selectedFolder = null;
        this.hasInitialState = undefined;

        // Upload
        this.allowUpload = true;
        this.uploadText = "";
        this.cancelUploadText = "";
        this.allowUploadToCurrentFolder = false;
        this.uploadErrorText = "";

        // Path
        this.showPath = true;
        this.showAppRelativePath = false;

        // Selection
        this.allowMultiSelect = false;
        this.focusedItem = null;

        // Edit
        this.allowRename = false;
        this.allowMove = false;
        this.allowDelete = false;
        this.allowCreate = false;
        this.renameMode = false;
        this.createMode = false;
        this.deleteConfirmText = "";
        this.downloadError = "";
        this.folderDialogCommand = null;

        this.lockLoadingDiv = false;

        // Filter
        this.filterDelay = 1200;

        // AccessRules
        this.folderRights = {};
        this.filesRules = [];

        // File Open
        this.processOpenedEventOnServer = false;

        // View
        this.viewMode = FileManagerConsts.ViewMode.Thumbnail;

        // TreeView
        this.folderExpandingNode = null;

        // Context Menu
        this.contextMenuEvt = null;

        // VirtualScrolling
        this.virtScroll = {
            itemIndex: 0,
            itemsCount: 0,
            pageSize: 0,
            pageCount: 0,
            pageItemsCount: 0,
            startPageScrollPos: null,
            lastPageScrollPos: null,
            lastPageIndexOffset: 0,
            timerId: -1,
            timeout: 10,
            jumpTimerId: -1,
            nearJumpTimeout: 100,
            farJumpTimeout: 500,
            inCallback: false
        };

        // Api command
        this.apiCommandCallback = null;
        this.apiCommandItems = null;
        this.SelectedFileChanged = new ASPxClientEvent();
        this.SelectedFileOpened = new ASPxClientEvent();
        this.FocusedItemChanged = new ASPxClientEvent();
        this.SelectionChanged = new ASPxClientEvent();
        this.CurrentFolderChanged = new ASPxClientEvent();
        this.FolderCreating = new ASPxClientEvent();
        this.FolderCreated = new ASPxClientEvent();
        this.ItemRenaming = new ASPxClientEvent();
        this.ItemRenamed = new ASPxClientEvent();
        this.ItemDeleting = new ASPxClientEvent();
        this.ItemDeleted = new ASPxClientEvent();
        this.ItemsDeleted = new ASPxClientEvent();
        this.ItemMoving = new ASPxClientEvent();
        this.ItemMoved = new ASPxClientEvent();
        this.ItemsMoved = new ASPxClientEvent();
        this.ItemCopying = new ASPxClientEvent();
        this.ItemCopied = new ASPxClientEvent();
        this.ItemsCopied = new ASPxClientEvent();
        this.ErrorOccurred = new ASPxClientEvent();
        this.ErrorAlertDisplaying = new ASPxClientEvent();
        this.CustomCommand = new ASPxClientEvent();
        this.ToolbarUpdating = new ASPxClientEvent();
        this.HighlightItemTemplate = new ASPxClientEvent();
        this.FileUploading = new ASPxClientEvent();
        this.FilesUploading = new ASPxClientEvent();
        this.FileUploaded = new ASPxClientEvent();
        this.FilesUploaded = new ASPxClientEvent();
        this.FileDownloading = new ASPxClientEvent();
    },

    Initialize: function() {
        this.constructor.prototype.Initialize.call(this);
        this.selectedArea = this.foldersHidden ? FileManagerConsts.SelectedArea.Files : FileManagerConsts.SelectedArea.Folders;
        this.GetSplitter().controlOwner = this;
        this.InitializeKbdHelper();

        if(this.allowUpload && this.enabled)
            this.InitializeUploadControl();
        this.InitializeFoldersTreeView();


        if(this.showPath && !this.showAppRelativePath)
            this.UpdatePath();

        if(!this.foldersHidden)
            this.selectedFolder = this.GetTreeView().GetSelectedNode();
        if(this.enabled) {
            this.InitializeEventHandlers();
            this.SetActiveArea(FileManagerConsts.SelectedArea.None);
        }
        if(this.breadCrumbsEnabled)
            this.breadCrumbs.refresh();
    },
    AfterInitialize: function() {
        this.constructor.prototype.AfterInitialize.call(this);
        this.SetSplitterUploadPaneSize();
        if(this.viewMode == FileManagerConsts.ViewMode.Grid)
            this.InitializeFilesGridView();
        if(this.allowMove)
            this.UpdateTollbarStandardItem(FileManagerConsts.ToolbarName.Move, this.IsMoveAvailable());
        if(this.allowCopy)
            this.UpdateTollbarStandardItem(FileManagerConsts.ToolbarName.Copy, this.IsCopyAvailable());
        if(this.inCommandProcessing)
            this.EndCommandProcessing();
        if(this.selectedItems) {
            var skipEvent = !(this.autoPostBack && this.checkHasInitialState());
            this.selectItems(this.selectedItems, skipEvent);
            this.selectedItems = null;
        }
        this.KeepClientState();
        if(this.downloadError)
            this.showError(ASPxClientFileManagerCommandConsts.Download, this.downloadError.editErrorText, this.downloadError.editErrorCode.toString());
    },
    SetSplitterUploadPaneSize: function() {
        this.SetSplitterPaneSizeByContentControl("UploadPanelPane", this.GetUploadControl());
    },
    SetSplitterPaneSizeByContentControl: function(name, control) {
        if(!control)
            return;
        var pane = this.GetSplitter().GetPaneByName(name),
            paneElement = pane.GetElement(),
            mainElement = control.GetMainElement();
        pane.SetSize(mainElement.offsetHeight + ASPx.GetTopBottomMargins(mainElement) + ASPx.GetTopBottomBordersAndPaddingsSummaryValue(paneElement.firstChild));
    },
    InitializeFoldersTreeView: function() {
        var treeView = this.GetTreeView();
        if(treeView)
            treeView.InitFileManagerCallbacks(this, false);
        this.foldersHidden = !treeView;
    },
    InitializeFilesGridView: function() {
        var gridView = this.GetFilesGridView();
        gridView.fileManager = this;
        this.AdjustGridViewSize();
        this.ensureGridViewItems();
        gridView.mainElement.className += " " + this.styles.rowSelectionActiveCssClass;
    },
    InitializeUploadControl: function() {
        var uc = this.GetUploadControl(),
            filesPaneSplitter = this.GetSplitter().GetPaneByName(FileManagerConsts.ItemsPaneSplitterName),
            filesPaneElement = filesPaneSplitter && filesPaneSplitter.GetElement();

        uc.fileManager = this;
        this.UpdateUploadPanelVisibility();
        if(this.uploadErrorText)
            this.showError(ASPxClientFileManagerCommandConsts.Upload, this.uploadErrorText);

        if(filesPaneElement)
            uc.viewManager.setInlineDropZoneAnchorElementID(filesPaneElement.id);
    },
    InitializeKbdHelper: function() {
        this.kbdHelper = new FileManagerKbdHelper(this);
        this.kbdHelper.Init();
        ASPx.KbdHelper.RegisterAccessKey(this);
    },
    InitializeEventHandlers: function() {
        var _this = this;
        this.initializeFilesAreaEventHandlers();
        if(this.enableContextMenu)
            ASPxClientUtils.AttachEventToElement(this.elements.GetItemsPaneContainer(), "contextmenu", function(evt) { _this.OnFilesAreaRMBClick(evt); });
        if(!this.foldersHidden) {
            var foldersContainer = this.elements.GetFoldersContainer();
            ASPx.Evt.AttachEventToElement(foldersContainer, "mousedown", function(evt) { _this.OnFoldersContainerClick(evt); });
            if(this.enableContextMenu)
                ASPx.Evt.AttachEventToElement(foldersContainer, "contextmenu", function(evt) { _this.OnFoldersContainerRMBClick(evt); });
            ASPx.Evt.AttachEventToElement(foldersContainer, "keydown", function(evt) {
                if(ASPx.Evt.GetKeyCode(evt) == ASPx.Key.Enter && !_this.IsEditMode())
                    _this.SetActiveArea(FileManagerConsts.SelectedArea.Folders);
            });
        }
        ASPx.Evt.AttachEventToDocument("click", function(evt) { _this.OnDocumentClick(evt); })
        if(this.allowRename || this.GetCreateHelperItem())
            this.PrepareRenameInput();
        if(this.allowMove || this.allowCopy)
            this.PrepareFolderBrowserDialog();

        if(this.allowUpload) {
            if(this.hideUploadPanel)
                ASPx.Evt.AttachEventToElement(this.elements.GetProgressPopupCancelButtonElement(), "click", function() { _this.GetUploadControl().Cancel(); });
            else
                ASPx.Evt.AttachEventToElement(this.elements.GetUploadButtonElement(), "click", function() { _this.GetUploadControl().OnButtonClick(); });
        }

        if(this.accessibilityCompliant) {
            ASPx.Evt.AttachEventToElement(this.elements.GetFocusInput(), "focus", function() {
                _this.Focus();
                if(_this.getSelectedItems().length == 0) {
                    _this.forEachItem(function(file) {
                        file.Select();
                        return true;
                    });
                }
                _this.SetActiveArea(FileManagerConsts.SelectedArea.Files);
            });
        }
    },
    initializeFilesAreaEventHandlers: function() {
        var itemsContainer = this.elements.GetItemsPaneContainer();
        if(this.viewMode == FileManagerConsts.ViewMode.Grid) {
            if(itemsContainer["dxmousedown"])
                ASPxClientUtils.DetachEventFromElement(itemsContainer, "mousedown", itemsContainer["dxmousedown"]);
            if(itemsContainer["dxscroll"])
                ASPxClientUtils.DetachEventFromElement(itemsContainer, "scroll", itemsContainer["dxscroll"]);
            itemsContainer["dxmousedown"] = null;
            this.InitializeGridViewEventHandlers();
        } else {
            var _this = this;
            itemsContainer["dxmousedown"] = function(evt) { _this.OnFilesContainerClick(evt); };
            ASPxClientUtils.AttachEventToElement(itemsContainer, "mousedown", itemsContainer["dxmousedown"]);
            if(this.isThumbnailModeVirtScrollEnabled()) {
                itemsContainer["dxscroll"] = function(evt) { _this.onItemsContainerScroll(evt); };
                ASPxClientUtils.AttachEventToElement(itemsContainer, "scroll", itemsContainer["dxscroll"]);
            }
        }
    },
    InitializeGridViewEventHandlers: function() {
        if(this.viewMode != FileManagerConsts.ViewMode.Grid)
            return;

        var grid = this.GetFilesGridView();
        grid.FocusedRowChanged.AddHandler(function(s, e) {
            this.onFilesGridViewFocusedRowChanged(s, e);
        }.aspxBind(this));
        grid.SelectionChanged.AddHandler(function(s, e) {
            this.onFilesGridViewSelectionChanged(s, e);
        }.aspxBind(this));
        grid.RowDblClick.AddHandler(function(s, e) {
            this.onFilesGridViewRowDblClick(s, e);
        }.aspxBind(this));
        ASPx.Evt.AttachEventToElement(grid.GetMainElement(), "click", function(evt) {
            this.SetActiveArea(FileManagerConsts.SelectedArea.Files);
        }.aspxBind(this));
    },
    PrepareRenameInput: function() {
        var _this = this;
        var input = this.elements.GetRenameInputElement();
        ASPx.Evt.AttachEventToElement(input, "keydown", function(evt) {
            _this.OnRenameInputKeyDown(evt);
            if(ASPx.Evt.GetKeyCode(evt) == ASPx.Key.Enter)
                return ASPx.Evt.PreventEventAndBubble(evt);
        });
        ASPx.Evt.AttachEventToElement(input, "blur", function() { _this.DoRename(); });
        ASPx.Evt.AttachEventToElement(input, "keypress", function(evt) { // B185616
            if(ASPx.Evt.GetKeyCode(evt) == ASPx.Key.Enter)
                return ASPx.Evt.PreventEventAndBubble(evt);
        });
    },
    PrepareFolderBrowserDialog: function() {
        var _this = this;
        ASPx.Evt.AttachEventToElement(this.elements.GetFolderBrowserDialogOkButton(), "click", function() { _this.FolderBrowserOkButtonClick(); });
        ASPx.Evt.AttachEventToElement(this.elements.GetFolderBrowserDialogCancelButton(), "click", function() { _this.FolderBrowserCancelButtonClick(); });
    },
    InitializeToolbars: function() {
        var toolbar = this.GetToolbar();
        if(!toolbar || this.toolbarsInitialized) {
            this.toolbarsInitialized = true;
            return;
        }
        var contentElement = ASPx.GetNodeByTagName(toolbar.GetMainElement(), "UL", 0);
        if(contentElement.offsetWidth > 0) {
            this.toolbarsInitialized = true;
            var commonWidth = 0;
            for(var i = 0; i < contentElement.childNodes.length; i++) {
                var child = contentElement.childNodes[i];
                if(child.tagName == "LI") {
                    commonWidth += child.offsetWidth;
                    var currentStyle = ASPx.GetCurrentStyle(child);
                    commonWidth += ASPx.PxToInt(currentStyle.marginLeft);
                    commonWidth += ASPx.PxToInt(currentStyle.marginRight);
                }
            }
            if(ASPx.Browser.HardwareAcceleration)
                commonWidth++;
            if(ASPx.Browser.WebKitFamily && window.devicePixelRatio !== 1) // B255486
                commonWidth += 2;
            ASPx.SetOffsetWidth(contentElement, commonWidth);
        }
    },
    UpdateFolderRights: function(str) {
        this.folderRights = {
            allowMove: str.indexOf("m") > -1,
            allowRename: str.indexOf("r") > -1,
            allowDelete: str.indexOf("d") > -1,
            allowCreate: str.indexOf("c") > -1,
            allowUpload: str.indexOf("u") > -1,
            allowCopy: str.indexOf("o") > -1
        };
    },
    NeedCollapseControlCore: function() {
        return this.GetSplitter().NeedCollapseControlCore();
    },
    CollapseControl: function() {
        this.isControlCollapsed = true;
        this.GetSplitter().CollapseControl();
    },
    ExpandControl: function() {
        this.isControlCollapsed = false;
        this.GetSplitter().ExpandControl();
    },
    AdjustControlCore: function() {
        ASPx.GetControlCollection().AdjustControls(this.GetMainElement());
        this.SetSplitterUploadPaneSize();

        var instance = this;
        window.setTimeout(function() {
            if(instance.isInitialized)
                instance.UpdateUploadPanelVisibility();

            instance.SetSplitterPaneSizeByContentControl("ToolbarPane", instance.GetToolbar());
            instance.SetSplitterUploadPaneSize();
            instance.SetSplitterPaneSizeByContentControl("BreadCrumbsPane", instance.breadCrumbs);
            instance.CorrectScroll(instance.focusedItem);
            instance.InitializeToolbars();

            if(!ASPxClientUtils.touchUI && window.devicePixelRatio !== 1 && !!instance.GetSplitter().GetPaneByName("FoldersPane")) // T252000
                instance.GetSplitter().GetPaneByName("FoldersPane").inResizing = true;
        }, 0);

        if(this.prepareCreateNode) {
            window.setTimeout(function() {
                instance.PrepareCreateNode();
            }, 0);
            this.prepareCreateNode = false;
        };
        setTimeout(function() {
            this.AdjustGridViewSize();
            this.adjustBreadCrumbsItems();
            this.updateVirtScroll();
        }.aspxBind(this), 0);
        this.itemsPaneContainerScrollHeight = this.elements.GetItemsPaneContainer().scrollTop;
    },
    AdjustGridViewSize: function() {
        if(this.viewMode != FileManagerConsts.ViewMode.Grid)
            return;
        var containerStyle = ASPx.GetCurrentStyle(this.elements.GetItemsPaneContainer());
        this.GetFilesGridView().SetHeight(ASPx.PxToInt(containerStyle.height));
    },
    OnBrowserWindowResize: function(evt) {
        setTimeout(function() { this.adjustControlOnResize(); }.aspxBind(this), 0);
        this.GetSplitter().OnBrowserWindowResize();
    },
    adjustControlOnResize: function() {
        this.updateVirtScroll();
        this.adjustBreadCrumbsItems();
        if(ASPx.IsPercentageSize(this.GetMainElement().style.width))
            this.AdjustGridViewSize();
    },
    focusMainElement: function() {
        if(ASPx.Browser.IE && ASPx.Browser.MajorVersion >= 8) {
            if(ASPx.Browser.MajorVersion == 8)
                this.focusMainElementIE8();
            else
                this.focusMainElementIE();
        }
        else
            this.GetMainElement().focus();
    },
    focusMainElementIE: function() {
        var mainElement = this.GetMainElement(),
            currentStyle = window.getComputedStyle(mainElement),
            rect = mainElement.getBoundingClientRect(),
            oldStyleWidth = mainElement.style.width,
            oldStyleHeight = mainElement.style.height;

        var placeHolder = this.getPlaceHolderElement();
            
        var outerWidth = ASPx.PxToFloat(currentStyle.width) + 
            ASPx.GetLeftRightBordersAndPaddingsSummaryValue(mainElement, currentStyle) +
            ASPx.GetLeftRightMargins(mainElement, currentStyle);

        var outerHeight = ASPx.PxToFloat(currentStyle.height) + 
            ASPx.GetTopBottomBordersAndPaddingsSummaryValue(mainElement, currentStyle) +
            ASPx.GetTopBottomMargins(mainElement, currentStyle);

        ASPx.SetStyles(mainElement, {
            width: currentStyle.width,
            height: currentStyle.height,
            position: "fixed",
            top: rect.top + "px",
            left: rect.left + "px"
        });
        
        ASPx.SetStyles(placeHolder, {
            width: outerWidth + "px",
            height: outerHeight + "px",
            display: ""
        });

        mainElement.focus();

        ASPx.SetStyles(placeHolder, {
            width: "0px",
            height: "0px",
            display: "none"
        });
        
        ASPx.SetStyles(mainElement, {
            width: oldStyleWidth,
            height: oldStyleHeight,
            position: "",
            top: "",
            left: ""
        });
    },
    focusMainElementIE8: function() {
        var scroll = document.documentElement.scrollLeft;
        this.GetMainElement().focus();
        document.documentElement.scrollLeft = scroll;
    },
    getPlaceHolderElement: function() {
        if(this.placeHolderElement === undefined) {
            var styleStr = "width:0px;height:0px;font-size:0px;line-height:0;padding:0px;margin:0px;display:none;";
            var htmlStr = "<div style='" + styleStr + "'></div>";
            this.placeHolderElement = ASPx.CreateHtmlElementFromString(htmlStr);
            ASPx.InsertElementAfter(this.placeHolderElement, this.GetMainElement());
        }
        return this.placeHolderElement;
    },

    // AccessRules
    UpdateFilesRules: function(rules) {
        this.filesRules = [];
        var ruleCount = rules.length;
        for(var i = 0; i < ruleCount; i++) {
            var ruleParts = rules[i];
            var rule = {
                pattern: ruleParts[0],
                edit: this.GetPermissionValue(ruleParts[1].charAt(0)),
                browse: this.GetPermissionValue(ruleParts[1].charAt(1))
            };
            this.filesRules.push(rule);
        }
    },
    GetPermissionValue: function(str) {
        return str == "-"
            ? FileManagerConsts.Rights.Default
            : str == "a" ? FileManagerConsts.Rights.Allow : FileManagerConsts.Rights.Deny;
    },
    EscapeRegExp: function(str) {
        return str.replace(/[-\/\\^$*+?.()|[\]{}]/g, "\\$&");
    },
    IsAppliedRule: function(rule, fileName) {
        var filePath = this.GetCurrentPath(true) + FileManagerConsts.PathSeparator + fileName;
        var regexpPattern = "^" + this.EscapeRegExp(rule.pattern).replace(/\\\*/g, ".*").replace(/\\\?/g, ".") + "$";
        var regExp = new RegExp(regexpPattern, "i");
        return regExp.test(filePath);
    },
    CheckEditAccessByFileName: function(fileName) {
        var result = true;
        var ruleCount = this.filesRules.length;
        for(var i = 0; i < ruleCount; i++) {
            var rule = this.filesRules[i];
            if(this.IsAppliedRule(rule, fileName)) {
                if(rule.edit != FileManagerConsts.Rights.Default)
                    result = rule.edit == FileManagerConsts.Rights.Allow;
                if(rule.browse != FileManagerConsts.Rights.Default)
                    result = rule.browse == FileManagerConsts.Rights.Allow;
            }
        }
        return result;
    },

    // Styles
    SetStyles: function(styles) {
        var stylesInfo = ASPxClientFileManager.StylesHelper.GetStylesInfo(
            styles.iw,
            styles.ih,
            this.elements.GetItemsContainer() || this.elements.GetItemsPaneContainer(),
            [
                [styles.fc, styles.fs],
                [styles.fsac, styles.fsas],
                [styles.fsic, styles.fsis],
                [styles.fhc, styles.fhs],
                [styles.ffc, styles.ffs],
                [styles.fafc, styles.fafs],
                [styles.fafsac, styles.fafsas],
                [styles.fafsic, styles.fafsis],
                [styles.fafhc, styles.fafhs],
                [styles.faffc, styles.faffs]
            ]
        );

        var contentWidth = "", contentHeight = "", containerPosition ="", itemElementPosition = "";
        if(stylesInfo.contentWidth > 0)
            contentWidth = "width:" + (this.isThumbnailsViewFileAreaItemTemplate ? stylesInfo.contentWidth : Math.max(stylesInfo.contentWidth, styles.tw)) + "px;";
        if(stylesInfo.contentHeight > 0)
            contentHeight = "height:" + (this.isThumbnailsViewFileAreaItemTemplate ?  stylesInfo.contentHeight : Math.max(stylesInfo.contentHeight, styles.th)) + "px;";
        if(this.allowMultiSelect || this.isThumbnailsViewFileAreaItemTemplate)
            containerPosition = "position: relative;";

        this.styles = {};
        this.styles.thumbnailWidth = styles.tw;
        this.styles.thumbnailHeight = styles.th;

        this.styles.fileCssClass = this.MergeCssClassWithStyleString(styles.fc, stylesInfo.styleStrings[0] + containerPosition);
        this.styles.fileContentCssClass = this.MergeCssClassWithStyleString(styles.fcc, styles.fcs + contentWidth + contentHeight);
        if(this.viewMode == FileManagerConsts.ViewMode.Thumbnail)
            this.styles.fileSelectionActiveCssClass = this.MergeCssClassWithStyleString(styles.fsac, stylesInfo.styleStrings[1]);
        else
            this.styles.fileSelectionActiveCssClass = styles.fsac;
        this.styles.fileSelectionInactiveCssClass = this.MergeCssClassWithStyleString(styles.fsic, stylesInfo.styleStrings[2]);
        this.styles.fileHoverCssClass = this.MergeCssClassWithStyleString(styles.fhc, stylesInfo.styleStrings[3]); // Hover should be the last registred style
        this.styles.fileFocusCssClass = this.viewMode == FileManagerConsts.ViewMode.Grid ? styles.ffc : this.MergeCssClassWithStyleString(styles.ffc, stylesInfo.styleStrings[4]);

        this.styles.fileAreaFolderCssClass = this.MergeCssClassWithStyleString(styles.fafc, stylesInfo.styleStrings[5] + containerPosition);
        this.styles.fileAreaFolderContentCssClass = this.MergeCssClassWithStyleString(styles.fafcc, styles.fafcs + contentWidth + contentHeight);
        if(this.viewMode == FileManagerConsts.ViewMode.Thumbnail)
            this.styles.fileAreaFolderSelectionActiveCssClass = this.MergeCssClassWithStyleString(styles.fafsac, stylesInfo.styleStrings[6]);
        else
            this.styles.fileAreaFolderSelectionActiveCssClass = styles.fafsac;
        this.styles.fileAreaFolderSelectionInactiveCssClass = this.MergeCssClassWithStyleString(styles.fafsic, stylesInfo.styleStrings[7]);
        this.styles.fileAreaFolderHoverCssClass = this.MergeCssClassWithStyleString(styles.fafhc, stylesInfo.styleStrings[8]); // Hover should be the last registred style
        this.styles.fileAreaFolderFocusCssClass = this.viewMode == FileManagerConsts.ViewMode.Grid ? styles.faffc : this.MergeCssClassWithStyleString(styles.faffc, stylesInfo.styleStrings[9]);

        this.styles.folderSelectionActiveCssClass = styles.fosac;
        this.styles.folderSelectionInactiveCssClass = this.MergeCssClassWithStyleString(styles.fosic, styles.fosis);

        this.styles.highlightCssClass = this.MergeCssClassWithStyleString(styles.hc, styles.hs);

        this.styles.breadCrumbsItemCssClass = this.MergeCssClassWithStyleString(styles.bc, styles.bs);
        this.styles.breadCrumbsItemHoverCssClass = this.MergeCssClassWithStyleString(styles.bhc, styles.bhs);
        this.styles.breadCrumbsUpButtonImageDisabledScriptObjects = styles.bids;
        this.styles.breadCrumbsUpButtonImageHoveredScriptObjects = styles.bihs;

        this.styles.rowSelectionActiveCssClass = "selectActive";
        this.styles.rowSelectionInactiveCssClass = "selectInactive";

        this.styles.filesGridViewDataRowCssClass = this.MergeCssClassWithStyleString(styles.gvrc, styles.gvrs);
    },
    MergeCssClassWithStyleString: function(cssClass, styleString) {
        if(!styleString)
            return cssClass;
        var styleStringCssClass = ASPx.CreateImportantStyleRule(this.styleSheet, styleString);
        return cssClass
            ? cssClass + " " + styleStringCssClass
            : styleStringCssClass;
    },

    SetHeight: function(height) {
        this.GetMainElement().style.height = height + "px";
        this.GetSplitter().SetHeight(height);
        this.AdjustGridViewSize();
    },
    SetWidth: function(width) {
        this.GetSplitter().SetWidth(width);
        this.adjustBreadCrumbsItems();
    },

    forEachItem: function(action) {
        for(var item in this.items) {
            if(this.items.hasOwnProperty(item) && action(this.items[item]))
                return;
        }
    },
    isEmptyItemsList: function() {
        for(var item in this.items) {
            if(this.items.hasOwnProperty(item))
                return false;
        }
        return true;
    },
    getSelectedItems: function() {
        var items = [];
        this.forEachItem(function(item) {
            if(item.IsSelected())
                items.push(item);
        });
        items.sort(function(item1, item2) { return item1.index - item2.index; });
        return items;
    },

    // Virtual scrolling
    isVirtScrollEnabled: function() {
        return this.virtScroll.pageSize > 0;
    },
    getItemSizes: function() {
        var sampleItem = this.virtScroll.firstItem;
        if(sampleItem) {
            var sampleElement = sampleItem.getElement();
            var cssText = sampleElement.style.cssText;
            if(cssText)
                sampleElement.style.cssText = "";
            var sampleStyle = this.getCurrentStyle(sampleElement);
            var itemWidth = 0,
                itemHeight = 0;
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 9) {
                itemWidth = sampleElement.offsetWidth;
                itemHeight = sampleElement.offsetHeight;
            }
            else {
                itemWidth = ASPx.PxToFloat(sampleStyle.width) + ASPx.GetLeftRightBordersAndPaddingsSummaryValue(sampleElement, sampleStyle);
                itemHeight = ASPx.PxToFloat(sampleStyle.height) + ASPx.GetTopBottomBordersAndPaddingsSummaryValue(sampleElement, sampleStyle);
            }
            itemWidth += ASPx.GetLeftRightMargins(sampleElement, sampleStyle);
            itemHeight += ASPx.GetTopBottomMargins(sampleElement, sampleStyle);
            if(cssText)
                sampleElement.style.cssText = cssText;
            return { 
                width: itemWidth,
                height: itemHeight
            };
        }
        return null;
    },
    getCurrentStyle: function(element) {
        if(window.getComputedStyle)
            return window.getComputedStyle(element, null);
        if(element.currentStyle)
            return element.currentStyle;
        return null;
    },
    setVirtScrollConfig: function(itemIndex, itemsCount, pageSize, pageItemsCount) {
        this.virtScroll.itemIndex = itemIndex;
        this.virtScroll.itemsCount = itemsCount;
        this.virtScroll.pageSize = pageSize;
        this.virtScroll.pageItemsCount = pageItemsCount;
    },
    createVirtScrollState: function() {
        var state = { };
        var itemsContainer = this.elements.GetItemsContainer();
        var itemsPaneContainer = this.elements.GetItemsPaneContainer();
        state.itemsAreaWidth = itemsContainer.clientWidth;
        state.itemsViewAreaHeight = itemsPaneContainer.clientHeight;
        var itemSizes = this.getItemSizes();
        if(this.virtScroll.itemsCount == 0 || itemSizes.itemWidth == 0 || itemSizes.itemHeight == 0) {
            state.itemWidth = state.itemHeight = state.itemsInRowCount = 0;
            state.isEmpty = true;
            return state;
        }
        state.itemWidth = itemSizes.width;
        state.itemHeight = itemSizes.height;
        var itemsInRowCount = Math.floor(state.itemsAreaWidth / state.itemWidth);
        state.itemsInRowCount = itemsInRowCount > 0 ? itemsInRowCount : 1;
        return state;
    },
    createVirtScrollExtendedState: function(itemIndex, pageItemsCount, itemsCount) {
        var state = this.createVirtScrollState();
        if(state.isEmpty) {
            state.pageHeight = 0;
            state.topIndentHeight = 0;
            state.bottomIndentHeight = 0;
            state.pageItemLeftOffset = 0;
            state.pageLastRowIsFull = true;
        }
        else {
            var itemLeftOffsetIndex = itemIndex % state.itemsInRowCount;
            var pageLayoutItemsCount = pageItemsCount + itemLeftOffsetIndex;
            state.pageHeight = Math.ceil((pageLayoutItemsCount) / state.itemsInRowCount) * state.itemHeight;
            var pageLastRowIsFull = (pageLayoutItemsCount % state.itemsInRowCount) == 0;
            state.pageLastRowIsFull = pageLastRowIsFull || (itemIndex + pageItemsCount >= itemsCount);
            var pageItemRowIndex = Math.floor(itemIndex / state.itemsInRowCount);
            state.topIndentHeight = pageItemRowIndex * state.itemHeight;
            var itemRowsCount = Math.ceil(itemsCount / state.itemsInRowCount);
            var itemAreaHeight = itemRowsCount * state.itemHeight;
            state.bottomIndentHeight = itemAreaHeight - state.topIndentHeight - state.pageHeight;
            state.pageItemLeftOffset = itemLeftOffsetIndex * state.itemWidth;
        }
        return state;
    },
    calculateVirtScrollState: function() {
        this.virtScroll.state = this.createVirtScrollExtendedState(this.virtScroll.itemIndex, this.virtScroll.pageItemsCount, this.virtScroll.itemsCount);
    },
    updatePageIndents: function() {
        this.elements.pageTopIndentElement.style.height = this.virtScroll.state.topIndentHeight + "px";
        this.elements.pageBottomIndentElement.style.height = this.virtScroll.state.bottomIndentHeight + "px";
    },
    updateVirtScroll: function() {
        if(!this.isThumbnailModeVirtScrollEnabled()) return;

        this.saveItemsScrollPosition();
        this.resetFirstItemIndent();
        this.updateVirtScrollCore();
        this.restoreItemsScrollPosition();
    },
    updateVirtScrollCore: function() {
        this.calculateVirtScrollState();
        this.updatePageIndents();
        this.updateFirstItemIndent();
    },
    saveNewItemsScrollPosition: function(itemIndex, pageItemsCount, scrollTop) {
        var state = this.createVirtScrollExtendedState(itemIndex, pageItemsCount, 0);
        this.virtScroll.lastPageScrollPos = this.getItemsScrollPosition(state, scrollTop);
    },
    saveItemsScrollPosition: function() {
        var scrollTop = this.elements.GetItemsPaneContainer().scrollTop;
        this.virtScroll.lastPageScrollPos = this.getItemsScrollPosition(this.virtScroll.state, scrollTop);
    },
    getItemsScrollPosition: function(state, scrollTop) {
        var pageScrollPos = null;
        if(state && !state.isEmpty) {
            var rowIndex = scrollTop / state.itemHeight;
            var itemIndex = Math.floor(rowIndex) * state.itemsInRowCount;
            var itemHeightOffset = rowIndex % 1;
            pageScrollPos = { itemIndex: itemIndex, itemHeightOffset: itemHeightOffset };
        }
        else
            pageScrollPos = this.getStartItemsScrollPos();
        return pageScrollPos;
    },
    getStartItemsScrollPos: function() {
        if(this.virtScroll.startPageScrollPos == null)
            this.virtScroll.startPageScrollPos = { itemIndex: 0, itemHeightOffset: 0 };
        return this.virtScroll.startPageScrollPos;
    },
    restoreItemsScrollPosition: function() {
        var rowIndex = Math.floor(this.virtScroll.lastPageScrollPos.itemIndex / this.virtScroll.state.itemsInRowCount);
        var scrollTop = (rowIndex + this.virtScroll.lastPageScrollPos.itemHeightOffset) * this.virtScroll.state.itemHeight;
        this.elements.GetItemsPaneContainer().scrollTop = scrollTop;
    },
    resetFirstItemIndent: function() {
        if(this.virtScroll.firstItem != null)
            this.virtScroll.firstItem.getElement().style.marginLeft = "";
    },
    updateFirstItemIndent: function() {
        if(this.virtScroll.firstItem != null) {
            var element = this.virtScroll.firstItem.getElement();
            element.style.cssText = "";
            var style = ASPx.GetCurrentStyle(element);
            var margin = style.marginLeft ? ASPx.PxToFloat(style.marginLeft) : 0;
            margin += this.virtScroll.state.pageItemLeftOffset;
            var cssText = "margin-left: " + margin + "px";
            element.style.cssText = ASPx.CreateImportantCssText(cssText);
        }
    },

    // File area items
    CreateItems: function(itemsList, skipState, isNewFileList) {
        if(this.isThumbnailModeVirtScrollEnabled())
            this.elements.ensurePageIndentsCreated();
        
        this.items = {};
        this.itemElementsIds = {};
        this.virtScroll.firstItem = null;

        for(var item, i = 0; item = itemsList[i]; i++) {
            var itemIndex = this.isVirtScrollEnabled() ? this.virtScroll.itemIndex + i : i;
            var item = this.createItem(item, itemIndex);
            if(!item.isCreateHelperFolder) {
                if(this.virtScroll.firstItem == null && !item.clientInvisibility && !item.outdated)
                    this.virtScroll.firstItem = item;
                this.items[item.id] = item;
                this.itemElementsIds[item.elementID] = item;
                this.PrepareItemState(item);
            }
        }
        if(this.enabled) {
            if(this.filter && isNewFileList)
                this.filter.UpdateEnabled(!this.isEmptyItemsList(), !this.isVirtScrollEnabled());
            this.delayedApplyState = !this.isInitialized && this.viewMode == FileManagerConsts.ViewMode.Grid;
            if(!skipState && !this.delayedApplyState)
                this.ApplyControlState(false);
        }
    },
    ClearItems: function(isNewFileList, skipUnselect) {
        if(this.internalCheckBoxCollection)
            this.internalCheckBoxCollection.Clear();
        if(isNewFileList) {
            if(!skipUnselect)
                this.unselectAllItems();
            this.DropStateField("item");
            if(this.filter)
                this.filter.resetState();
        }

        this.ClearStateControllerHoverItems();

        this.items = {};
        this.itemElementsIds = {};
        this.focusedItem = null;

        if(this.viewMode == FileManagerConsts.ViewMode.Thumbnail) {
            if(this.isVirtScrollEnabled()) {
                this.elements.dropPageIndents();
                if(isNewFileList) {
                    this.virtScroll.itemIndex = 0;
                    this.virtScroll.pageItemsCount = 0;
                }
            }
            this.elements.DropItemsContainer();
        }
    },
    createApiCommandItems: function(itemsDataList) {
        this.apiCommandItems = [];
        for(var itemData, i = 0; itemData = itemsDataList[i]; i++) {
            var item = this.createItem(itemData, i);
            if(!item.isCreateHelperFolder)
                this.apiCommandItems.push(item);
        }
    },
    createItem: function(itemData, index) {
        return itemData.it == FileManagerConsts.FileAreaItemTypes.File
            ? new ASPxClientFileManagerFile(this, index, itemData)
            : new ASPxClientFileManagerFolder(this, index, itemData);
    },
    GetVisibleItems: function() {
        var items = [];
        this.forEachItem(function(item) {
            if(item.GetVisible() && !item.isCreateHelperFolder)
                items.push(item);
        });
        items.sort(function(a, b) { return a.index - b.index });
        return items;
    },
    UpdateFileList: function(callbackCommand, needSelectItem) {
        this.DropStateField("item");
        this.SendCallback(callbackCommand, needSelectItem);
    },
    TryOpen: function() {
        return this.TryOpenItem(this.allowMultiSelect ? this.focusedItem : this.getSelectedItems()[0]);
    },
    TryOpenItem: function(item) {
        if(!item)
            return false;
        if(item.isFolder) {
            var path = this.GetCurrentPath(true);
            if(!item.isParentFolderItem)
                path += (path ? FileManagerConsts.PathSeparator : "") + item.name;
            else
                path = path.substr(0, path.lastIndexOf(FileManagerConsts.PathSeparator));
            this.SetCurrentFolderPathInternal(path, undefined, item.id, item.isParentFolderItem);   
            this.needSetActiveItemsArea = true;
        } else
            this.raiseSelectedFileOpenedEvent(item);
        return true;
    },
    ensureGridViewItems: function() {
        if(this.viewMode == FileManagerConsts.ViewMode.Grid) {
            var grid = this.GetFilesGridView();
            this.forEachItem(function(item) {
                item.index = grid.GetRowIndexByKey(item.id);
                var row = grid.GetItem(item.index);
                if(row)
                    row.title = item.tooltip;
            }.aspxBind(this));
            if(this.detailsCreateHelperItem) {
                this.detailsCreateHelperItem.index = grid.getVisibleEndIndex();
                this.elements.createDetailsCreateHelperItemElement(this.detailsCreateHelperItem);
            }
        }
        if(this.delayedApplyState) {
            this.delayedApplyState = null;
            this.ApplyControlState(false);
        }
    },

    // Bread crumbs
    createBreadCrumbs: function() {
        this.breadCrumbs = new ClientFileManagerBreadCrumbs(this);
        this.breadCrumbs.render();
    },
    adjustBreadCrumbsItems: function() {
        if(!this.breadCrumbsEnabled)
            return;
        this.GetBreadCrumbsPopup().Hide();
        this.breadCrumbs.adjustItems();
    },

    // States
    ApplyControlState: function(skipEvents) {
        if(!this.enabled) return;
        var focusFileName = this.GetItemName(this.GetStateField(FileManagerConsts.StateField.ItemFocused));
        var selectItemProperties = this.GetStateField(FileManagerConsts.StateField.ItemSelected);
        var selectItemNames = [];
        var outdatedItems = { };
        if(selectItemProperties) {
            for(var itemProperties, i = 0; itemProperties = selectItemProperties[i]; i++) {
                var itemName = this.GetItemName(itemProperties)
                selectItemNames.push(itemName);
                if(!this.items[itemName])
                    outdatedItems[itemName] = this.createOutdatedItem(itemProperties);
            }
        }
        
        if(this.viewMode == FileManagerConsts.ViewMode.Grid) {
            this.GetFilesGridView().UnselectAllRowsOnPage();
            this.GetFilesGridView().SetFocusedRowIndex(-1);
        }

        if(selectItemNames.length > 0) {
            var focused = focusFileName && ASPx.Data.ArrayIndexOf(selectItemNames, focusFileName) > -1 ? this.focusItem(focusFileName, skipEvents) : true;
            var selectedItems = this.selectItems(selectItemNames, skipEvents, !focused);
            if(selectedItems.length != selectItemNames.length) {
                for(var fileName, i = 0; fileName = selectItemNames[i]; i++) {
                    if(ASPx.Data.ArrayIndexOf(selectedItems, fileName) < 0) {
                        var item = this.items[fileName] || outdatedItems[fileName];
                        this.raiseSelectionChanged(item, false);
                    }
                }
            }
            this.saveSelectedItemsToState();
        }
        var filterValue = this.GetStateField(FileManagerConsts.StateField.ItemFilter);
        if(filterValue && this.filter)
            this.filter.SetFilterValue(filterValue, !this.isVirtScrollEnabled());
    },
    PrepareItemState: function(item) {
        if(!this.enabled || this.viewMode == FileManagerConsts.ViewMode.Grid)
            return;
        ASPx.GetStateController().AddSelectedItem(
            item.elementID,
            [item.selectionActiveCssClass],
            [""],
            null,
            null,
            null
        );
        ASPx.GetStateController().AddHoverItem(
            item.elementID,
            [item.hoverCssClass],
            [""],
            null,
            null,
            null
        );
    },
    ClearStateControllerHoverItems: function() {
        if(this.viewMode == FileManagerConsts.ViewMode.Grid)
            return;
        this.forEachItem(function(file) {
            var fileId = file.elementID;
            ASPx.GetStateController().RemoveSelectedItem(fileId);
            ASPx.GetStateController().RemoveHoverItem(fileId);
        });
    },
    GetItemName: function(itemProperties) {
        return itemProperties ? itemProperties.split(FileManagerConsts.ItemPropertyValueSeparator)[0] : "";
    },
    createOutdatedItem: function(itemProperties) {
        var propArr = itemProperties.split(FileManagerConsts.ItemPropertyValueSeparator);
        return this.createOutdatedItemCore(propArr[0], propArr[1], propArr[2] == "true")
    },
    createOutdatedItemCore: function(id, name, isFolder) {
        var itemInfo = {
            id: id,
            n: name,
            outdated: true
        };
        var item = null;
        if(!isFolder) {
            itemInfo.it = FileManagerConsts.FileAreaItemTypes.File;
            item = new ASPxClientFileManagerFile(this, 0, itemInfo);
        }
        else {
            itemInfo.it = FileManagerConsts.FileAreaItemTypes.Folder;
            item = new ASPxClientFileManagerFolder(this, 0, itemInfo);
        }
        return item;
    },

    // Events
    OnFolderClick: function(node) {
        this.SetActiveArea(FileManagerConsts.SelectedArea.Folders);
        this.UpdateTollbarStandardItem(FileManagerConsts.ToolbarName.Move, this.IsMoveAvailable());
        if(this.selectedFolder == node)
            return;
        this.selectedFolder = node;
        this.needCurrentFolderChangedRaise = true;
        this.lockLoadingDiv = true;
        this.DoRename();
        this.UpdateFileList(FileManagerConsts.CallbackCommandId.GetFileList);
        setTimeout(function() {
            this.lockLoadingDiv = false;
            if(this.inCallback)
                this.ShowLoadingDiv();
        }.aspxBind(this), 150);
    },
    OnFolderExpanding: function(treeView, e) {
        if(treeView.callBack) {
            if(!e.node.GetExpanded() && e.node.GetNodeCount() == 0)
                this.folderExpandingNode = e.node;
            if(!treeView.isFolderBrowserFolders && this.inCallback) {
                e.cancel = true;
                this.delayedExpandNode = e.node;
            }
        }
    },
    OnItemClick: function(item, modifierKey) {
        if(this.allowRename && !item.IsSelected())
            this.DoRename();
        if(this.allowMultiSelect)
            this.onItemClickInMultipleMode(item, modifierKey);
        else
            item.Select();
        this.CorrectScroll(item);
    },
    onItemClickInMultipleMode: function(item, modifierKey) {
        var visibleFiles = this.GetVisibleItems();
        var focusFile = this.focusedItem || visibleFiles[0];
        item.focus();
        if(modifierKey == FileManagerConsts.ModifierKey.None) {
            item.Select();
            this.unselectAllItems([item]);
        }
        else if(modifierKey == FileManagerConsts.ModifierKey.Ctrl) {
            if(!(this.getSelectedItems().length == 1 && item.IsSelected()))
                item.invertSelection();
        }
        else if(modifierKey == FileManagerConsts.ModifierKey.Shift) {
            if(item == focusFile)
                return;
            var select = !(item.IsSelected() && focusFile.IsSelected());
            var direction = item.index > focusFile.index;
            for(var i = focusFile.index; direction > 0 ? i <= item.index : i >= item.index; direction > 0 ? i++ : i--) {
                var nextFile = visibleFiles[i];
                if(select)
                    nextFile.Select();
                else if(nextFile != item)
                    nextFile.Unselect();
            }
        }
    },
    Focus: function() {
        if(this.kbdHelper)
            this.kbdHelper.Focus();
    },
    OnFileUploadStart: function() {
        if(!this.hideUploadPanel)
            return;
        this.GetUploadProgressPopup().Show();
        this.correctItemsAreaPopupPosition(this.GetUploadProgressPopup());
    },
    OnFilesUploadComplete: function(evt) {
        var uc = this.GetUploadControl();
        uc.OnCompleteFileUpload();
        if(!this.hideUploadPanel) {
            uc.UpdateButtonValue(false);
            this.OnUploadTextChanged("");
        }
        var result = {
            uploadErrorText: evt.errorText
        };
        if(evt.callbackData != '')
            result = eval(evt.callbackData);
        uc.ClearText();
        if(result.uploadErrorText) {
            if(result.errorCode === undefined)
                result.errorCode = "" + ASPxClientFileManagerErrorConsts.Unspecified;
            this.showError(ASPxClientFileManagerCommandConsts.Upload, result.uploadErrorText, result.errorCode);
        }

        if(result.uploadSuccess) {
            var needSelectItem = this.needSelectItems.length > 0 ? this.needSelectItems[0] : undefined;
            this.UpdateFileList(FileManagerConsts.CallbackCommandId.GetFileList, needSelectItem);
            this.raiseFileUploaded();
        }
        
        if(this.hideUploadPanel) {
            this.GetUploadProgressBar().SetPosition(0);
            this.GetUploadProgressPopup().Hide();
        }
        this.HideLoadingElements();
    },
    OnUploadingProgressChanged: function(evt) {
        if(this.hideUploadPanel)
            this.GetUploadProgressBar().SetPosition(evt.progress);
    },
    OnUploadTextChanged: function(text) {
        if(!this.hideUploadPanel)
            this.GetUploadControl().SetButtonEnable(text != "");
    },
    OnToolbarItemClick: function(itemName) {
        if(this.renameMode)
            this.DoRename();
        if(itemName.indexOf(FileManagerConsts.ToolbarStandardButtonPrefix) != 0) {
            this.raiseCustomCommand(itemName);
            return;
        }
        switch(itemName.replace(FileManagerConsts.ToolbarStandardButtonPrefix, "")) {
            case FileManagerConsts.ToolbarName.Refresh:
                this.Refresh();
                break;
            case FileManagerConsts.ToolbarName.Delete:
                this.TryDelete();
                break;
            case FileManagerConsts.ToolbarName.Rename:
                this.TryRename();
                break;
            case FileManagerConsts.ToolbarName.Move:
                this.TryMove();
                break;
            case FileManagerConsts.ToolbarName.Create:
                this.TryCreate();
                break;
            case FileManagerConsts.ToolbarName.Download:
                this.TryDownload();
                break;
            case FileManagerConsts.ToolbarName.Copy:
                this.TryCopy();
                break;
        }
    },
    OnContextMenuItemClick: function(itemName) {
        this.OnToolbarItemClick(itemName);
    },
    OnFolderBrowserDialogClosing: function() {
        this.folderDialogCommand = null;
        this.UpdateToolbars();
    },
    OnFolderBrowserDialogShown: function() {
        this.UpdateToolbars();
    },
    OnDocumentClick: function(evt) {
        if(!this.GetMainElement() || !this.isExists())
            return;
        var element = ASPx.Evt.GetEventSource(evt);
        if(element.parentNode && !ASPx.GetIsParent(this.GetMainElement(), element)) {
            if(this.createMode)
                this.DoCreate();
            this.SetActiveArea(FileManagerConsts.SelectedArea.None);
        }
    },
    OnFilesContainerClick: function(evt) {
        if(!ASPx.Evt.IsLeftButtonPressed(evt))
            return;
        if(this.createMode)
            this.DoCreate();
        else {
            var item = this.getClickedItem(evt);
            if(item && !this.elements.getCheckBoxByEvt(evt))
                this.OnItemClick(item, this.getKeyModifier(evt));
            this.SetActiveArea(FileManagerConsts.SelectedArea.Files);
        }
    },
    onItemsContainerScroll: function(evt) {
        if(this.inCallback || this.createInFileAreaMode)
            return;
        if(this.virtScroll.timerId == -1)
            this.virtScroll.timerId = window.setTimeout(this.onVirtualScrollingTimeout.aspxBind(this), this.virtScroll.timeout);
    },
    onVirtualScrollingTimeout: function() {
        this.virtScroll.timerId = -1;
        this.tryPerformVirtulScrolling();
    },
    onVirtScrollJumpTimeout: function() {
        this.virtScroll.jumpTimerId = -1;
        this.tryPerformVirtulScrolling(true);
    },
    tryPerformVirtulScrolling: function(doScroll) {
        if(this.inCallback) {
            this.virtScroll.lastPageIndexOffset = 0;
            return;
        }
        var scrollTop = this.elements.GetItemsPaneContainer().scrollTop;
        var lowerBoundary = this.virtScroll.state.topIndentHeight;
        var upperBoundary = lowerBoundary + this.virtScroll.state.pageHeight - this.virtScroll.state.itemsViewAreaHeight + 1;
        var upperBoundaryOffset = this.virtScroll.state.pageLastRowIsFull ? 0 : this.virtScroll.state.itemHeight;
        upperBoundary -= upperBoundaryOffset;
        if(upperBoundary < 0)
            upperBoundary = 0;
        var lowerBoundaryOverflow = scrollTop < lowerBoundary;
        var upperBoundaryOverflow = scrollTop > upperBoundary;
        var boundaryOverflow = lowerBoundaryOverflow || upperBoundaryOverflow;
        
        if(boundaryOverflow) {
            if(doScroll) {
                this.virtScroll.lastPageIndexOffset = 0;
                this.performVirtualScrolling(scrollTop, upperBoundaryOverflow);
                return;
            }

            var pageBottomIndexOffset = Math.floor((scrollTop - lowerBoundary) / this.virtScroll.state.pageHeight);
            var pageTopIndexOffset = Math.floor((scrollTop + this.virtScroll.state.itemsViewAreaHeight + upperBoundaryOffset - lowerBoundary) / this.virtScroll.state.pageHeight);
            var pageIndexOffset = 0;
            if(pageBottomIndexOffset !== 0)
                pageIndexOffset = pageBottomIndexOffset;
            else if(pageTopIndexOffset !== 0)
                pageIndexOffset = pageTopIndexOffset;

            if(pageIndexOffset !== this.virtScroll.lastPageIndexOffset) {
                var farJump = Math.abs(pageIndexOffset) > 1;
                var jumpTimeout = farJump ? this.virtScroll.farJumpTimeout : this.virtScroll.nearJumpTimeout;
                
                if(this.virtScroll.jumpTimerId > -1)
                    window.clearTimeout(this.virtScroll.jumpTimerId);
                this.virtScroll.jumpTimerId = window.setTimeout(this.onVirtScrollJumpTimeout.aspxBind(this), jumpTimeout);

                this.virtScroll.lastPageIndexOffset = pageIndexOffset;
            }
        }
        else {
            this.virtScroll.lastPageIndexOffset = 0;
            if(this.virtScroll.jumpTimerId > -1) {
                window.clearTimeout(this.virtScroll.jumpTimerId);
                this.virtScroll.jumpTimerId = -1;
            }
        }
    },
    performVirtualScrolling: function(scrollTop, forwardScroll) {
        var params = this.getNewVirtScrollParams(scrollTop, forwardScroll);
        this.saveNewItemsScrollPosition(params.newItemIndex, params.newPageItemsCount, scrollTop);
        this.virtScroll.inCallback = true;
        this.performVirtualScrollingCallback(params.newItemIndex, params.newPageItemsCount);
    },
    getNewVirtScrollParams: function(scrollTop, forwardScroll) {
        var state = this.createVirtScrollState();
        if(state.isEmpty)
            return { newItemIndex: 0, newPageItemsCount: this.virtScroll.pageSize };

        var prevItemsRate = 0.2,
            nextItemsRate = 1 - prevItemsRate;

        var viewAreaMaxRowCount = Math.ceil(state.itemsViewAreaHeight / state.itemHeight) + 1;
        var minPageItemsCount = Math.ceil((viewAreaMaxRowCount * state.itemsInRowCount) / nextItemsRate);
        var newPageItemsCount = this.virtScroll.pageSize >= minPageItemsCount ? this.virtScroll.pageSize : minPageItemsCount;

        var viewAreaScrollTop = forwardScroll ? scrollTop : scrollTop + state.itemsViewAreaHeight;
        var viewAreaStartItemIndex = Math.floor(viewAreaScrollTop / state.itemHeight) * state.itemsInRowCount;
        var backItemsOffset = Math.floor((forwardScroll ? prevItemsRate : nextItemsRate) * newPageItemsCount);
        var newItemIndex = viewAreaStartItemIndex - backItemsOffset;
        if(newItemIndex < 0)
            newItemIndex = 0;

        return { 
            newItemIndex: newItemIndex,
            newPageItemsCount: newPageItemsCount
        };
    },
    isThumbnailViewMode: function() {
        return this.viewMode == FileManagerConsts.ViewMode.Thumbnail;
    },
    isThumbnailModeVirtScrollEnabled: function() {
        return this.isThumbnailViewMode() && this.isVirtScrollEnabled();
    },
    isDetailsModeVirtScrollEnabled: function() {
        return !this.isThumbnailViewMode() && this.isVirtScrollEnabled();
    },
    
    OnFilesAreaRMBClick: function(evt) {
        if(ASPxClientUtils.safari && this.getKeyModifier(evt) == FileManagerConsts.ModifierKey.Ctrl) {
            ASPxClientUtils.PreventEventAndBubble(evt);
            return;
        }
        this.SetActiveArea(FileManagerConsts.SelectedArea.Files);
        var file = this.getClickedItem(evt);
        if(file) {
            if(ASPxClientUtils.ArrayIndexOf(this.getSelectedItems(), file) == -1)
                this.OnItemClick(file, this.getKeyModifier(evt));
        } else
            this.SetActiveArea(FileManagerConsts.SelectedArea.None);
        this.ShowContextMenu(evt);
    },
    onThumbnailViewFilesAreaItemDblClick: function(item, evt) {
        if(this.elements.getCheckBoxByEvt(evt))
            return;
        this.TryOpenItem(item);
    },
    onFilesGridViewRowDblClick: function(s, e) {
        if(this.createMode)
            this.DoCreate();
        else {
            var item = this.items[s.GetRowKey(e.visibleIndex)];
            this.TryOpenItem(item);
        }
    },
    onFilesGridViewFocusedRowChanged: function(s, e) {
        if(this.inCallback)
            return;
        var rowIndex = s.GetFocusedRowIndex();
        if(rowIndex == -1)
            return;
        var item = this.items[s.GetRowKey(rowIndex)];
        if(this.allowMultiSelect)
            item.focus();
        else {
            var selectedKeys = s.GetSelectedKeysOnPage();
            for(var selKey, i = 0; selKey = selectedKeys[i]; i++) {
                if(selKey != item.id)
                    s.UnselectRowOnPage(this.items[selKey].index);
            }
            item.Select();
        }
    },
    onFilesGridViewSelectionChanged: function(s, e) {
        if(!this.allowMultiSelect || this.inCallback)
            return;
        if(this.createMode)
            this.DoCreate();
        if(e.visibleIndex == -1 && e.isAllRecordsOnPage) {
            this.forEachItem(function(item) {
                if(e.isSelected)
                    item.Select();
                else
                    item.Unselect();
            });
        }
        else {
            var item = this.items[s.GetRowKey(e.visibleIndex)];
            if(item) {
                if(e.isSelected)
                    item.Select();
                else
                    item.Unselect();
            }
        }
        this.SetActiveArea(FileManagerConsts.SelectedArea.Files);
    },
    OnFoldersContainerClick: function(evt) {
        this.SetActiveArea(FileManagerConsts.SelectedArea.Folders);
    },
    OnFoldersContainerRMBClick: function(evt) {
        var sourceElement = ASPx.GetParentByTagName(ASPx.Evt.GetEventSource(evt), "DIV");
        while(!sourceElement.id)
            sourceElement = sourceElement.parentNode;
        var treeView = this.GetTreeView();
        var node = treeView.rootNode.GetNodeByContentElementID(sourceElement.id);
        if(!node)
            return;
        if(node !== treeView.GetSelectedNode()) {
            treeView.SetSelectedNode(node);
            this.OnFolderClick(node);
            this.contextMenuEvt = evt;
            ASPx.Evt.PreventEventAndBubble(evt);
        } else
            this.ShowContextMenu(evt);
    },
    OnSplitterPaneResizeCompleted: function() {
        var splitter = this.GetSplitter();
        if(this.cookieName && this.cookieName != "") {
            ASPx.Cookie.DelCookie(this.cookieName);
            ASPx.Cookie.SetCookie(this.cookieName, splitter.GetClientStateString());
        }
    },
    OnRenameInputKeyDown: function(evt) {
        switch (ASPx.Evt.GetKeyCode(evt)) {
            case ASPx.Key.Enter:
                if(this.renameMode)
                    this.DoRename();
                this.SetActiveArea(FileManagerConsts.SelectedArea.None);
                break;
            case ASPx.Key.Esc:
                this.HideRenameInput();
                this.HideCreateHelperElement();
                if(this.isDetailsModeVirtScrollEnabled())
                    this.GetFilesGridView().scrollToPageEnd();
                ASPx.Evt.PreventEvent(evt);
                this.Focus();
                break;
            case ASPx.Key.Tab:
                this.DoRename();
                this.SetActiveArea(FileManagerConsts.SelectedArea.None);
                break;
        }
    },
    getKeyModifier: function(evt) {
        if(evt.shiftKey)
            return FileManagerConsts.ModifierKey.Shift;
        if(evt.ctrlKey)
            return FileManagerConsts.ModifierKey.Ctrl;
        return FileManagerConsts.ModifierKey.None;
    },
    getClickedItem: function(evt) {
        if(this.viewMode == FileManagerConsts.ViewMode.Grid) {
            var grid = this.GetFilesGridView();
            var row = grid.getItemByHtmlEvent(evt);
            if(!row)
                return null;
            return this.items[grid.GetRowKey(grid.getItemIndex(row.id))];
        } else {
            var sourceElement = ASPx.GetParentByPartialClassName(ASPx.Evt.GetEventSource(evt), FileManagerConsts.FileClassName);
            while(!sourceElement.id)
                sourceElement = sourceElement.parentNode;
            return this.itemElementsIds[sourceElement.id];
        }
    },

    raiseEventWithArgsInternal: function(eventName, args) {
        var evt = this[eventName];
        if(!evt.IsEmpty())
            evt.FireEvent(this, args);
        return args;
    },

    raiseSelectedFileOpenedEvent: function(file) {
        var args = new ASPxClientFileManagerFileOpenedEventArgs(file);
        args.processOnServer = this.processOpenedEventOnServer;
        this.raiseEventWithArgsInternal("SelectedFileOpened", args);

        if(args.processOnServer)
            this.SendPostBack(FileManagerConsts.CallbackCommandId.ServerProcessFileOpened);
    },
    raiseFocusedItemChangedEvent: function(item) {
        if(!this.allowMultiSelect)
            return;
        var args = new ASPxClientFileManagerFocusedItemChangedEventArgs(item, item.name, item.GetFullName());
        this.raiseEventWithArgsInternal("FocusedItemChanged", args);
    },
    raiseSelectedFileChangedEvent: function(file) {
        if(!this.allowMultiSelect)
            this.raiseEventWithArgsInternal("SelectedFileChanged", new ASPxClientFileManagerFileEventArgs(file));
    },
    raiseSelectionChanged: function(item, isSelected) {
        var args = new ASPxClientFileManagerSelectionChangedEventArgs(item, item.id, item.GetFullName(), isSelected);
        this.raiseEventWithArgsInternal("SelectionChanged", args);
    },
    raiseCurrentFolderChangedEvent: function() {
        var args = new ASPxClientFileManagerCurrentFolderChangedEventArgs(this.getFolderName(), this.GetCurrentPath());
        this.raiseEventWithArgsInternal("CurrentFolderChanged", args);
    },

    raiseFileEditingEvent: function(eventName, item) {
        var arg = new ASPxClientFileManagerItemEditingEventArgs(item.GetFullName(), item.name, item.isFolder);
        arg = this.raiseEventWithArgsInternal(eventName, arg);
        return !arg.cancel;
    },
    raiseFolderEditingEvent: function(eventName) {
        var arg = new ASPxClientFileManagerItemEditingEventArgs(this.GetCurrentPath(), this.getFolderName(), true);
        arg = this.raiseEventWithArgsInternal(eventName, arg);
        return !arg.cancel;
    },
    raiseItemDeleting: function(item) {
        return this.raiseFileEditingEvent("ItemDeleting", item);
    },
    raiseFolderDeleting: function() {
        return this.raiseFolderEditingEvent("ItemDeleting");
    },
    raiseItemMoving: function(file) {
        return this.raiseFileEditingEvent("ItemMoving", file);
    },
    raiseFolderMoving: function() {
        return this.raiseFolderEditingEvent("ItemMoving");
    },
    raiseItemCopying: function(item) {
        return this.raiseFileEditingEvent("ItemCopying", item);
    },
    raiseFolderCopying: function() {
        return this.raiseFolderEditingEvent("ItemCopying");
    },
    raiseFolderCreating: function() {
        return this.raiseFolderEditingEvent("FolderCreating");
    },
    raiseFileRenaming: function(file) {
        return this.raiseFileEditingEvent("ItemRenaming", file);
    },
    raiseFolderRenaming: function(file) {
        return this.raiseFolderEditingEvent("ItemRenaming");
    },
    raiseFolderCreated: function() {
        var fullName = "",
            name = "";
        if(this.GetActiveArea() == FileManagerConsts.SelectedArea.Files) {
            var item = this.GetSelectedItem(FileManagerConsts.FileAreaItemTypes.Folder);
            if(item) {
                fullName = item.GetFullName();
                name = item.name;
            }
        }
        else {
            fullName = this.GetCurrentPath();
            name = this.getFolderName();
        }
        var arg = new ASPxClientFileManagerItemCreatedEventArgs(fullName, name, true);
        this.raiseEventWithArgsInternal("FolderCreated", arg);
    },
    raiseItemMoved: function() {
        var itemEventArgCreator = function(fullName, itemName, oldFolderFullName, itemIsFolder) {
            return new ASPxClientFileManagerItemMovedEventArgs(fullName, itemName, oldFolderFullName, itemIsFolder);
        };
        var itemsEventArgCreator = function(items, oldFolderFullName) {
            return new ASPxClientFileManagerItemsMovedEventArgs(items, oldFolderFullName);
        };
        this.raiseItemPositionChangedEvents("ItemMoved", "ItemsMoved", itemEventArgCreator, itemsEventArgCreator);
    },
    raiseItemCopied: function() {
        var itemEventArgCreator = function(fullName, itemName, oldFolderFullName, itemIsFolder) {
            return new ASPxClientFileManagerItemCopiedEventArgs(fullName, itemName, oldFolderFullName, itemIsFolder);
        };
        var itemsEventArgCreator = function(items, oldFolderFullName) {
            return new ASPxClientFileManagerItemsCopiedEventArgs(items, oldFolderFullName);
        };
        this.raiseItemPositionChangedEvents("ItemCopied", "ItemsCopied", itemEventArgCreator, itemsEventArgCreator);
    },
    raiseItemPositionChangedEvents: function(itemEventName, itemsEventName, itemEventArgCreator, itemsEventArgCreator) {
        var folderPath = this.GetCurrentPath();
        var items = [];
        var arg = null;
        for(var itemName, i = 0; itemName = this.changeItemPositionOldInfo.name[i]; i++) {
            var itemId = this.changeItemPositionOldInfo.id[i];
            var itemIsFolder = this.changeItemPositionOldInfo.isFolder[i];
            var item = this.items[itemId];
            if(!item)
                item = this.createOutdatedItemCore(itemId, itemName, itemIsFolder);
            items.push(item);
            var fullName = folderPath + FileManagerConsts.PathSeparator + itemName;
            arg = itemEventArgCreator(fullName, itemName, this.changeItemPositionOldInfo.oldFolderFullName, itemIsFolder);
            this.raiseEventWithArgsInternal(itemEventName, arg);
        }
        arg = itemsEventArgCreator(items, this.changeItemPositionOldInfo.oldFolderFullName);
        this.raiseEventWithArgsInternal(itemsEventName, arg);
    },
    raiseItemDeleted: function() {
        var doi = this.deleteOldInfo;
        var items = [];
        var arg = null;
        for(var i = 0, itemName; itemName = doi.name[i]; i++) {
            var itemIsFolder = this.deleteOldInfo.isFolder[i];
            var item = this.createOutdatedItemCore(this.deleteOldInfo.id[i], itemName, itemIsFolder);
            items.push(item);
            var arg = new ASPxClientFileManagerItemDeletedEventArgs(doi.oldFolderFullName + FileManagerConsts.PathSeparator + itemName, itemName, itemIsFolder);
            this.raiseEventWithArgsInternal("ItemDeleted", arg);
        }
        arg = new ASPxClientFileManagerItemsDeletedEventArgs(items);
        this.raiseEventWithArgsInternal("ItemsDeleted", arg);
    },
    raiseItemRenamed: function(oldName, item) {
        var args = new ASPxClientFileManagerItemRenamedEventArgs(item.GetFullName(), item.name, oldName, item.isFolder);
        this.raiseEventWithArgsInternal("ItemRenamed", args);
    },
    raiseFolderRenamed: function(oldName) {
        var args = new ASPxClientFileManagerItemRenamedEventArgs(this.GetCurrentPath(), this.getFolderName(), oldName, true);
        this.raiseEventWithArgsInternal("ItemRenamed", args);
    },
    raiseFileDownloading: function(file) {
        var arg = this.raiseEventWithArgsInternal("FileDownloading", new ASPxClientFileManagerFileDownloadingEventArgs(file));
        return !arg.cancel;
    },
    raiseErrorOccurred: function(commandName, errorText, errorCode) {
        return this.raiseEventWithArgsInternal("ErrorOccurred", new ASPxClientFileManagerErrorEventArgs(commandName, errorText, errorCode));
    },
    raiseErrorAlertDisplaying: function(commandName, errorText) {
        return this.raiseEventWithArgsInternal("ErrorAlertDisplaying", new ASPxClientFileManagerErrorAlertDisplayingEventArgs(commandName, errorText));
    },
    raiseFileUploading: function() {
        var folder = this.GetCurrentPath();
        var uploadControl = this.GetUploadControl();
        var arg = new ASPxClientFileManagerFilesUploadingEventArgs(folder, uploadControl.GetFileNameArray());
        if(!this.FilesUploading.IsEmpty())
            this.FilesUploading.FireEvent(this, arg);
        else {
            arg = new ASPxClientFileManagerFileUploadingEventArgs(folder, uploadControl.GetFileName());
            if(!this.FileUploading.IsEmpty())
                this.FileUploading.FireEvent(this, arg);
        }
        return !arg.cancel;
    },
    raiseFileUploaded: function() {
        var folder = this.GetCurrentPath();
        var fileNames = this.GetUploadControl().lastUploadedFile.split(", ");
        var arg = null;
        for(var fileName, i = 0; fileName = fileNames[i]; i++) {
            arg = new ASPxClientFileManagerFileUploadedEventArgs(folder, fileName);
            this.raiseEventWithArgsInternal("FileUploaded", arg);
        }
        arg = new ASPxClientFileManagerFilesUploadedEventArgs(folder, fileNames);
        this.raiseEventWithArgsInternal("FilesUploaded", arg);
    },
    raiseCustomCommand: function(commandName) {
        this.raiseEventWithArgsInternal("CustomCommand", new ASPxClientFileManagerCustomCommandEventArgs(commandName));
    },
    raiseToolbarUpdating: function(activeAreaName) {
        this.raiseEventWithArgsInternal("ToolbarUpdating", new ASPxClientFileManagerToolbarUpdatingEventArgs(activeAreaName));
    },
    raiseHighlightItemTemplate: function(filterValue, itemName, templateElement, highlightCssClassName) {
        this.raiseEventWithArgsInternal("HighlightItemTemplate", new ASPxClientFileManagerHighlightItemTemplateEventArgs(filterValue, itemName, templateElement, highlightCssClassName));
    },

    SetVisible: function(visible) {
        ASPxClientControl.prototype.SetVisible.call(this, visible);
        if(visible) {
            setTimeout(function() {
                this.AdjustControl();
            }.aspxBind(this), 0);
        }
    },

    // State
    GetLastKeyPart: function(key) {
        var parts = key.split(".");
        return parts[parts.length - 1];
    },
    GetStateItem: function(key) {
        var parts = key.split(".");
        var currentItem = this.getStateObject();
        for(var i = 0; i < parts.length - 1; i++) {
            var _key = parts[i];
            if(!currentItem[_key])
                currentItem[_key] = {};
            currentItem = currentItem[_key];
        }
        return currentItem;
    },
    checkHasInitialState: function() {
        if(this.hasInitialState === undefined)
            this.hasInitialState = !!this.stateObject;
        return this.hasInitialState;
    },
    getStateObject: function() {
        this.checkHasInitialState();
        if(!this.stateObject) 
            this.stateObject = { };
        return this.stateObject;
    },
    GetStateField: function(key) {
        var stateItem = this.GetStateItem(key);
        return stateItem[this.GetLastKeyPart(key)];
    },
    UpdateStateField: function(key, value) {
        if(!this.enabled) return;

        var stateItem = this.GetStateItem(key);
        stateItem[this.GetLastKeyPart(key)] = value;
    },
    DropStateField: function(key) {
        if(!this.enabled) return;

        var stateItem = this.GetStateItem(key);
        delete stateItem[this.GetLastKeyPart(key)];
    },

    // Callbacks
    SendCallback: function(commandId, args, skipScrollSaving) {
        this.inCallback = true;
        this.callbackOwner = null;

        this.KeepClientState();
        if(this.isThumbnailModeVirtScrollEnabled() && !skipScrollSaving)
            this.saveItemsScrollPosition();
        var callbackArg = this.GetArgumentsString(commandId, args);
        if(!this.autoPostBack) {
            this.ShowLoadingElements();
            if(this.viewMode == FileManagerConsts.ViewMode.Grid && this.IsCommandNeedFilesRefresh(commandId)) {
                this.savedCallbackArg = callbackArg;
                this.GetFilesGridView().Refresh();
            }
            else
                this.CreateCallback(callbackArg, commandId == FileManagerConsts.CallbackCommandId.CustomCallback ? "CUSTOMCALLBACK" : "");
        }
        else
            this.SendPostBack(callbackArg);
    },
    IsCommandNeedFilesRefresh: function(commandId) {
        return commandId != FileManagerConsts.CallbackCommandId.ShowFolderBrowserDialog && commandId != FileManagerConsts.CallbackCommandId.CustomCallback &&
               commandId != FileManagerConsts.CallbackCommandId.ApiCommand;
    },
    GetArgumentsString: function(commandId, args) {
        var callbackString = commandId;
        if(args) {
            var argumentsString = ASPx.Ident.IsArray(args)
                ? args.join(FileManagerConsts.CallbackArgumentSeparator)
                : args;
            callbackString += FileManagerConsts.CallbackArgumentSeparator + argumentsString;
        }
        return callbackString;
    },
    SendTreeViewCallback: function(treeView, callbackString) {
        this.ShowLoadingDiv();
        this.KeepClientState();
        if(this.delayedSetCurrentFolderPath !== undefined) {
            this.CreateCallback(
                FileManagerConsts.CallbackCommandId.ChangeFolderTvCallback +
                FileManagerConsts.CallbackArgumentSeparator +
                this.delayedSetCurrentFolderPath +
                FileManagerConsts.CallbackArgumentSeparator +
                callbackString);
        }
        else {
            this.callbackOwner = treeView;
            var commandId = treeView.isFolderBrowserFolders
                ? FileManagerConsts.CallbackCommandId.FolderBrowserFoldersTvCallback
                : FileManagerConsts.CallbackCommandId.FoldersTvCallback;
            var folderPath = "";
            if(this.folderExpandingNode) {
                folderPath = this.GetFoldersPath(this.folderExpandingNode, true);
                this.folderExpandingNode = null;
            }
            var args = [commandId, folderPath, callbackString].join(FileManagerConsts.CallbackArgumentSeparator);
            this.CreateCallback(args);
        }
    },
    SendGridViewCallback: function(gridView, callbackString, command) {
        this.KeepClientState();
        if(command == ASPxClientGridViewCallbackCommand.ApplyHeaderColumnFilter || command == ASPxClientGridViewCallbackCommand.ApplyFilter)
            this.needResetSelection = true;
        if(command == "FUNCTION")
            this.skipClearItems = true;
        this.ShowLoadingElements();
        if(this.savedCallbackArg != null) {
            var argPrefix = this.savedCallbackArg + FileManagerConsts.CallbackLinkedArgumentSeparator;
            this.savedCallbackArg = null;
            this.CreateCallback(argPrefix + FileManagerConsts.CallbackCommandId.GridView + FileManagerConsts.CallbackArgumentSeparator + callbackString);
        }
        else {
            var commandId = command == ASPxClientGridViewCallbackCommand.GotoPage ? FileManagerConsts.CallbackCommandId.GridViewVirtualScrollingCallback : FileManagerConsts.CallbackCommandId.GridView;
            this.virtScroll.inCallback = commandId == FileManagerConsts.CallbackCommandId.GridViewVirtualScrollingCallback;
            this.CreateCallback(commandId + FileManagerConsts.CallbackArgumentSeparator + callbackString);
        }
    },
    ClearCallbackOwner: function() {
        if(this.callbackOwner)
            this.callbackOwner.HideLoadingElements();
        this.callbackOwner = null;
    },

    OnCallback: function(result) {
        if(this.callbackOwner)
            this.callbackOwner.OnCallback(result);
        else
            this.ProcessCommandResult(result);
    },
    ProcessCommandResult: function(result) {
        this.inCommandProcessing = true;
        if(result.apiCommandResult)
            this.createApiCommandItems(result.apiCommandResult);
        var customCallbackResultExists = result.customCallbackResult != undefined;
        if(customCallbackResultExists) {
            var args = result.customCallbackResult.split(FileManagerConsts.CallbackLinkedArgumentSeparator);
            this.elements.GetItemsPaneContainer().innerHTML = args[0];
            this.viewMode = FileManagerConsts.ViewMode[args[1]];
            this.SetStyles(eval(args[2]));
        }
        if(result.selectedArea != undefined && !result.apiCommandResult)
            this.SetActiveArea(result.selectedArea ? FileManagerConsts.SelectedArea.Files : FileManagerConsts.SelectedArea.Folders);
        if(result.items && !this.skipClearItems) {
            this.needApplyState = this.getSelectedItems().length > 0 && result.isNewFileList ? 2 : 1;
            this.ClearItems(result.isNewFileList, customCallbackResultExists);
            this.needResetSelection = this.needResetSelection || result.isNewFileList;
            if(this.isVirtScrollEnabled()) {
                if(ASPx.IsExists(result.virtScrollItemIndex)) {
                    if(result.selectedItems && this.viewMode == FileManagerConsts.ViewMode.Thumbnail)
                        this.scrollToFirstSelectedItem = true;
                    this.virtScroll.itemIndex = result.virtScrollItemIndex;
                    this.virtScroll.itemsCount = result.itemsCount;
                    this.virtScroll.pageItemsCount = result.virtScrollPageItemsCount;
                    if(ASPx.IsExists(result.virtScrollResetState))
                        this.virtScroll.lastPageScrollPos = this.getStartItemsScrollPos();
                }
            }
            this.customThumbnails = result.thumbnails;
            if(result.itemsRender)
                this.elements.GetItemsContainer().innerHTML = result.itemsRender;
            this.CreateItems(result.items, true, result.isNewFileList);
            if(this.isThumbnailModeVirtScrollEnabled()) {
                if(this.filter && !result.isNewFileList)
                    this.filter.HighlightItems();
                this.updateVirtScrollCore();
                this.restoreItemsScrollPosition();
            }
            if(typeof (result.folderRights) == "string")
                this.UpdateFolderRights(result.folderRights);
            if(result.filesRules)
                this.UpdateFilesRules(result.filesRules);
            this.allowUploadToCurrentFolder = result.allowUpload && this.folderRights.allowUpload;
            this.UpdateUploadPanelVisibility();
            this.SetSplitterUploadPaneSize();
            this.UpdateToolbars();
            if(this.contextMenuEvt) {
                this.ShowContextMenu(this.contextMenuEvt);
                this.contextMenuEvt = null;
            }
        }
        if(result.gridViewResult) {
            this.gridLayoutChangeCallbackProcess = !(result.isNewFileList || this.skipClearItems);
            this.GetFilesGridView().OnCallback(result.gridViewResult);
            if(!this.isVirtScrollEnabled())
                this.updateGridFilterView();
            this.needGridViewEndCallback = true;
        }
        if(result.foldersRender)
            this.RefreshFoldersOnCallback(result.foldersRender);
        if(result.selectedItems)
            this.needSelectItems = result.selectedItems;
        if(result.treeViewResult) {
            this.GetTreeView().OnCallback(result.treeViewResult);
            this.needTreeViewEndCallback = true;
        }
        if(ASPx.IsExists(result.changeCurrentFolder)) {
            this.currentPath = result.changeCurrentFolder;
            this.KeepClientState();
        }
        this.StartCommandProcessing(result);
        if(this.showPath && !result.apiCommandResult)
            this.UpdatePath(result.path);
        if(result.currentFolderId !== undefined)
            this.currentFolderId =  result.currentFolderId;
        this.skipClearItems = null;
    },
    updateGridFilterView: function() {
        if(this.filter && this.gridLayoutChangeCallbackProcess) {
            this.GetFilesGridView().EnsureRowKeys();
            this.filter.HighlightItems();
            this.gridLayoutChangeCallbackProcess = false;
        }
    },
    StartCommandProcessing: function(result) {
        var commandName;
        switch (result.command) {
            case FileManagerConsts.CallbackCommandId.CreateQuery:
                if(result.isSuccess) {
                    this.createMode = true;
                    this.prepareCreateNode = true;
                    this.UpdateToolbars();
                }
                break;
            case FileManagerConsts.CallbackCommandId.Create:
                commandName = ASPxClientFileManagerCommandConsts.Create;
                if(result.isSuccess)
                    this.needFolderCreatedRaise = true;
                break;
            case FileManagerConsts.CallbackCommandId.Refresh:
                this.selectedArea = this.foldersHidden ? FileManagerConsts.SelectedArea.Files : FileManagerConsts.SelectedArea.Folders;
                this.UpdateToolbars();
                break;
            case FileManagerConsts.CallbackCommandId.RenameItem:
                commandName = ASPxClientFileManagerCommandConsts.Rename;
                if(result.isSuccess)
                    this.needItemRenamedRaise = true;
                break;
            case FileManagerConsts.CallbackCommandId.ShowFolderBrowserDialog:
                if(result.isSuccess) {
                    this.ShowFolderBrowserPopup(result.folderBrowserFoldersRender);
                    this.needInitFolderBrowserFolders = true;
                }
                break;
            case FileManagerConsts.CallbackCommandId.MoveItems:
                commandName = ASPxClientFileManagerCommandConsts.Move;
                if(result.isSuccess)
                    this.needItemMovedRaise = true;
                break;
            case FileManagerConsts.CallbackCommandId.CopyItems:
                commandName = ASPxClientFileManagerCommandConsts.Copy;
                if(result.isSuccess)
                    this.needItemCopiedRaise = true;
                break;
            case FileManagerConsts.CallbackCommandId.DeleteItems:
                commandName = ASPxClientFileManagerCommandConsts.Delete;
                if(result.isSuccess)
                    this.raiseItemDeleted();
                break;
            case FileManagerConsts.CallbackCommandId.ChangeFolderTvCallback:
                if(!result.isSuccess) {
                    this.delayedSetCurrentFolderPath = undefined;
                    this.delayedCallbackFunction = undefined;
                }
                else
                    this.needCurrentFolderChangedRaise = true;
                break;
        }
        if(result && !result.isSuccess) {
            this.folderDialogCommand = null;
            this.showError(commandName, result.editErrorText, result.editErrorCode);
        }
    },
    OnCallbackFinalized: function() {
        if(this.callbackOwner)
            this.callbackOwner.OnCallbackFinalized();
        else
            this.OnCallbackFinalizedCore();
        this.inCallback = false;
        this.virtScroll.inCallback = false;
        if(this.delayedExpandNode) {
            this.delayedExpandNode.SetExpanded(!this.delayedExpandNode.GetExpanded());
            this.delayedExpandNode = null;
        }
    },
    OnCallbackFinalizedCore: function() {
        if(this.isVirtScrollEnabled())
            this.updateGridFilterView();
        if(this.needGridViewEndCallback) {
            this.GetFilesGridView().OnCallbackFinalized();
            this.needGridViewEndCallback = false;
        }
        if(this.needTreeViewEndCallback) {
            this.GetTreeView().OnCallbackFinalized();
            this.needTreeViewEndCallback = false;
        }
        if(this.delayedSetCurrentFolderPath !== undefined) {
            var node = this.GetTreeView().GetNodeByName(this.delayedSetCurrentFolderPath);
            if(node) {
                this.GetTreeView().SetSelectedNode(node);
                this.selectedFolder = node;
                this.expandNodeRecursive(node);
            }
            this.delayedSetCurrentFolderPath = undefined;
        }
        if(this.createMode)
            this.PrepareCreateNode();
        this.EndCommandProcessing();
        if(this.showPath && !this.showAppRelativePath)
            this.UpdatePath();
        if(this.filter && this.filter.delayedFilter !== null)
            this.filter.applyDelayedFilter();
    },
    EndCommandProcessing: function() {
        if(!this.foldersHidden)
            this.selectedFolder = this.GetTreeView().GetSelectedNode();
        if(this.needInitFolderBrowserFolders) {
            this.GetFolderBrowserTreeView().InitFileManagerCallbacks(this, true);
            this.needInitFolderBrowserFolders = false;
        }
        if(this.needApplyState) {
            if(this.needApplyState == 2)
                this.raiseSelectedFileChangedEvent(null);
            this.ApplyControlState(true);
            this.needApplyState = null;
        }
        if(!this.needResetSelection && this.needSelectItems) {
            var selectedItems = this.selectItems(this.needSelectItems);
            if(this.scrollToFirstSelectedItem) {
                setTimeout((function() { this.scrollToItem(this.items[selectedItems[0]]); }).aspxBind(this), 0);
                this.scrollToFirstSelectedItem = false;
            }
        }
        if(this.needResetSelection) {
            this.resetSelection(this.needSelectItems);
            if(this.isThumbnailModeVirtScrollEnabled() && !this.allowMultiSelect)
                this.scrollToSelection(this.needSelectItems);
            this.needResetSelection = false;
            this.needSelectItems = null;
        }
        if(this.needFocusItemName) {
            var item = this.items[this.needFocusItemName];
            if(item)
                this.allowMultiSelect ? item.focus(true, true) : item.Select(true);
            this.needFocusItemName = "";
        }
        if(this.needSetActiveItemsArea)
            this.SetActiveArea(FileManagerConsts.SelectedArea.Files);
        if(this.needInitFolders) {
            this.InitializeFoldersTreeView();
            this.needInitFolders = false;
        }
        if(this.needItemRenamedRaise) {
            var item = this.GetSelectedItem();
            if(!this.showFoldersInFileArea || item == null)
                this.raiseFolderRenamed(this.renameOldName);
            else
                this.raiseItemRenamed(this.renameOldName, item);
            this.needItemRenamedRaise = false;
        }
        if(this.needItemMovedRaise) {
            this.raiseItemMoved();
            this.needItemMovedRaise = false;
        }
        if(this.needItemCopiedRaise) {
            this.raiseItemCopied();
            this.needItemCopiedRaise = false;
        }
        if(this.needFolderCreatedRaise) {
            this.raiseFolderCreated();
            this.needFolderCreatedRaise = false;
        }
        if(this.needCurrentFolderChangedRaise) {
            this.raiseCurrentFolderChangedEvent();
            this.needCurrentFolderChangedRaise = false;
        }
        if(this.delayedCallbackFunction !== undefined) {
            this.delayedCallbackFunction.call();
            this.delayedCallbackFunction = undefined;
        }
        if(this.apiCommandItems)
            window.setTimeout(this.onApiCommandCallback.aspxBind(this), 0);
        else {
            this.UpdateToolbars();
            this.KeepClientState();
            if(this.breadCrumbsEnabled)
                this.breadCrumbs.refresh();
        }
        this.inCommandProcessing = false;
    },
    onApiCommandCallback: function() {
        var callback = this.apiCommandCallback,
            items = this.apiCommandItems;
        this.apiCommandCallback = null;
        this.apiCommandItems = null;
        callback(items);
    },
    UpdatePath: function(path) {
        var input = this.elements.GetPathInput();
        if(input) {
            input.value = this.showAppRelativePath ? path : this.GetCurrentPath();
            if(this.createMode)
                input.value += this.newFolderName;
        }
    },
    RefreshFoldersOnCallback: function(render) {
        this.elements.GetFoldersContainer().innerHTML = render;
        if(this.allowMove)
            this.UpdateTollbarStandardItem(FileManagerConsts.ToolbarName.Move, this.IsMoveAvailable());
        this.needInitFolders = true;
    },
    AfterCustomCallback: function() {
        if(this.viewMode == FileManagerConsts.ViewMode.Grid)
            this.InitializeFilesGridView();
        if(this.enabled)
            this.initializeFilesAreaEventHandlers();
    },

    // Get inner controls
    GetSplitter: function() {
        var control = window[this.name + FileManagerConsts.SplitterPostfix];
        return control && control.isASPxClientControl ? control : null;
    },
    GetUploadControl: function() {
        var splitterPostfix = this.hideUploadPanel ? "" : FileManagerConsts.SplitterPostfix;
        var control = window[this.name + splitterPostfix + FileManagerConsts.UploadPostfix];
        return control && control.isASPxClientControl ? control : null;
    },
    GetToolbar: function() {
        var control = window[this.name + FileManagerConsts.SplitterPostfix + FileManagerConsts.ToolbarPostfix];
        return control && control.isASPxClientControl ? control : null;
    },
    GetContextMenu: function() { 
        var control = window[this.name + FileManagerConsts.ContextMenuPostfix];
        return control && control.isASPxClientControl ? control : null;
    },
    GetTreeView: function() {
        var control = window[this.name + FileManagerConsts.SplitterPostfix + FileManagerConsts.FoldersPostfix];
        return control && control.isASPxClientControl ? control : null;
    },
    GetFilesGridView: function() {
        var control = window[this.name + FileManagerConsts.SplitterPostfix + FileManagerConsts.GridPostfix];
        return control && control.isASPxClientControl ? control : null;
    },
    GetFolderBrowserPopup: function() {
        var control = window[this.name + FileManagerConsts.FolderBrowserPopupPostfix];
        return control && control.isASPxClientControl ? control : null;
    },
    GetFolderBrowserTreeView: function() {
        var control = window[this.name + FileManagerConsts.FolderBrowserPopupPostfix + FileManagerConsts.FolderBrowserFolders];
        return control && control.isASPxClientControl ? control : null;
    },
    GetUploadProgressPopup: function() {
        var control = window[this.name + FileManagerConsts.UploadProgressPopupPostfix];
        return control && control.isASPxClientControl ? control : null;
    },
    GetUploadProgressBar: function() {
        var control = window[this.name + FileManagerConsts.UploadProgressPopupPostfix + FileManagerConsts.UploadProgressBarPostfix];
        return control && control.isASPxClientControl ? control : null;
    },
    GetBreadCrumbsPopup: function() {
        var control = window[this.name + FileManagerConsts.BreadCrumbsPopupPostfix];
        return control && control.isASPxClientControl ? control : null;
    },

    // Paths utils
    getFolderName: function() {
        if(this.selectedFolder)
            return this.getFolderNameByNode(this.selectedFolder);
        else
            return this.GetCurrentPath().substr(this.GetCurrentPath().lastIndexOf(FileManagerConsts.PathSeparator) + 1);
    },
    getFolderNameByNode: function(node) {
        return this.isNewFolderNode(node) ? "" : node.GetText();
    },
    getFolderIdByNode: function(node) {
        return this.isNewFolderNode(node) ? "" : node.name;
    },
    isNewFolderNode: function(node) {
        return node.name.indexOf("[fmNewFolderNode]") > -1;
    },
    GetCurrentPath: function(skipRootFolder, separator) {
        if(!separator)
            separator = FileManagerConsts.PathSeparator;
        if(this.foldersHidden) {
            var path = skipRootFolder ? this.currentPath : (this.rootFolderName + (this.currentPath ? FileManagerConsts.PathSeparator + this.currentPath : ""));
            if(separator && separator != FileManagerConsts.PathSeparator) {
                var regExp = new RegExp("/" + separator + "/g");
                path = path.replace(regExp, separator);
            }
            return path;
        }
        return this.foldersHidden ? this.currentPath : this.GetFoldersPath(this.GetTreeView().GetSelectedNode(), skipRootFolder, separator);
    },
    getCurrentFolderParentPath: function() {
        var path = this.GetCurrentPath();
        path = path.slice(0, path.lastIndexOf(this.getFolderName()));
        return path == "" ? path : path.slice(0, path.length - 1);
    },
    GetFoldersPath: function(currentFolder, skipRootFolder, separator, useId) {
        var pathParts = [];
        while(currentFolder) {
            var pathPart = useId ? this.getFolderIdByNode(currentFolder) : this.getFolderNameByNode(currentFolder);
            pathParts.push(pathPart);
            currentFolder = currentFolder.parent;
        }
        pathParts = pathParts.reverse();
        if(skipRootFolder)
            pathParts = pathParts.slice(1);
        if(!separator)
            separator = FileManagerConsts.PathSeparator;
        return pathParts.join(separator);
    },
    // ClientState
    KeepClientState: function() {
        this.UpdateStateField(FileManagerConsts.StateField.CurrentPath, this.GetCurrentPath(true)
            + (this.GetTreeView() && this.GetTreeView().enabled ? FileManagerConsts.CallbackArgumentSeparator + this.GetTreeView().GetSelectedNode().GetExpanded() : ""));
        if(this.isVirtScrollEnabled()) {
            this.UpdateStateField(FileManagerConsts.StateField.VirtScrollItemIndexFieldName, this.virtScroll.itemIndex);
            this.UpdateStateField(FileManagerConsts.StateField.VirtScrollPageItemsCountFieldName, this.virtScroll.pageItemsCount);
        }
    },

    // Selection (Active Area)
    GetActiveArea: function() {
        return this.selectedArea;
    },
    GetActiveAreaName: function() {
        return FileManagerConsts.SelectedAreaNames[this.GetActiveArea()];
    },
    SetActiveArea: function(selectedArea) {
        if(this.folderDialogCommand != null)
            return;
        if(selectedArea != FileManagerConsts.SelectedArea.None)
            this.lastActiveItemsArea = selectedArea;
        if(this.GetActiveArea() == selectedArea) {
            this.UpdateToolbars();
            return;
        }
        this.selectedArea = selectedArea;
        this.UpdateToolbars();
        this.DoRename();
        if(selectedArea == FileManagerConsts.SelectedArea.Folders) {
            this.UpdateFileSelectState(true);
            this.UpdateFolderSelectState(false);
        }
        if(selectedArea == FileManagerConsts.SelectedArea.Files) {
            this.UpdateFileSelectState(false);
            this.UpdateFolderSelectState(true);
        }
        if(selectedArea == FileManagerConsts.SelectedArea.None) {
            this.UpdateFileSelectState(true);
            this.UpdateFolderSelectState(true);
        }
    },
    UpdateFileSelectState: function(toInactive) {
        this.forEachItem(function(file) {
            file.UpdateSelectState(toInactive);
        }.aspxBind(this));
    },
    UpdateFolderSelectState: function(toInactive) {
        var selectedNode = this.selectedFolder;
        if(selectedNode) {
            var selectedNodeEl = selectedNode.GetHtmlElement();
            if(!selectedNodeEl)
                return;
            selectedNodeEl.className = toInactive
            ? selectedNodeEl.className.replace(this.styles.folderSelectionActiveCssClass, this.styles.folderSelectionInactiveCssClass)
            : selectedNodeEl.className.replace(this.styles.folderSelectionInactiveCssClass, this.styles.folderSelectionActiveCssClass);
        }
    },
    ShowLoadingPanel: function() {
        var parentElement = this.virtScroll.inCallback ? this.elements.GetItemsPaneContainer() : this.GetMainElement();
        this.CreateLoadingPanelWithAbsolutePosition(parentElement, this.GetLoadingPanelOffsetElement(parentElement));
    },
    ShowLoadingDiv: function() {
        if(this.lockLoadingDiv) {
            this.lockLoadingDiv = false;
            return;
        }
        this.CreateLoadingDiv(this.GetMainElement());
    },
    GetLoadingPanelCallbackAnimationOffsetElement: function() {
        return this.elements.GetItemsPaneContainer();
    },
    GetCallbackAnimationElement: function() {
        return this.elements.GetItemsContainer() || this.elements.GetItemsPaneContainer();
    },

    // Toolbars
    UpdateToolbars: function(disableAll) {
        disableAll = disableAll ? disableAll : this.IsEditMode();
        var toolbar = this.GetToolbar();
        this.UpdateMenuStandardItems(toolbar, disableAll);
        this.UpdateMenuStandardItem(toolbar, FileManagerConsts.ToolbarName.Upload, !disableAll && this.allowUploadToCurrentFolder);
        this.raiseToolbarUpdating(FileManagerConsts.SelectedAreaNames[this.GetActiveArea()]);
    },
    UpdateTollbarStandardItem: function(name, enable) {
        this.UpdateToolbarItem(this.GetMenuStandardItemFullName(name), enable)
    },
    UpdateToolbarItem: function(name, enable) {
        this.UpdateMenuItem(this.GetToolbar(), name, enable);
    },
    UpdateMenuStandardItems: function(menu, disableAll) {
        this.UpdateMenuStandardItem(menu, FileManagerConsts.ToolbarName.Create, !disableAll && this.IsCreateAvailable());
        this.UpdateMenuStandardItem(menu, FileManagerConsts.ToolbarName.Move, !disableAll && this.IsMoveAvailable());
        this.UpdateMenuStandardItem(menu, FileManagerConsts.ToolbarName.Delete, !disableAll && this.IsDeleteAvailable());
        this.UpdateMenuStandardItem(menu, FileManagerConsts.ToolbarName.Rename, !disableAll && this.IsRenameAvailable());
        this.UpdateMenuStandardItem(menu, FileManagerConsts.ToolbarName.Download, !disableAll && this.IsDownloadButtonActive());
        this.UpdateMenuStandardItem(menu, FileManagerConsts.ToolbarName.Copy, !disableAll && this.IsCopyAvailable());
    },
    UpdateMenuStandardItem: function(owner, name, enable) {
        this.UpdateMenuItem(owner, this.GetMenuStandardItemFullName(name), enable);
    },
    UpdateMenuItem: function(owner, name, enable) {
        if(!owner)
            return;
        var items = owner.items || owner.rootItem.items;
        for(var item, i = 0; item = items[i] ; i++) {
            if(item.name == name)
                item.SetEnabled(enable)
            if(item.items.length > 0)
                this.UpdateMenuItem(item, name, enable);
        }
    },
    GetMenuStandardItemFullName: function(name) {
        return FileManagerConsts.ToolbarStandardButtonPrefix + name;
    },

    // Context Menu
    ShowContextMenu: function(e) {
        var menu = this.GetContextMenu();
        if(menu) {
            this.UpdateMenuStandardItems(menu);
            var isFolderArea = this.GetActiveArea() == FileManagerConsts.SelectedArea.Folders;
            this.UpdateMenuStandardItem(menu, FileManagerConsts.ToolbarName.Upload, this.allowUploadToCurrentFolder && !isFolderArea && !this.getClickedItem(e));
            menu.ShowInternal(e);
            ASPx.Evt.PreventEventAndBubble(e);
        }
    },

    // Upload
    UpdateUploadPanelVisibility: function() {
        if(!this.allowUpload) return;
        if(!this.enabled) return;
        var pane = this.elements.GetUploadPanelPane();
        var visible = this.allowUploadToCurrentFolder;
        this.GetUploadControl().SuppressFileDialog(!visible);
        if(this.hideUploadPanel)
            return;
        if(visible)
            pane.Expand();
        else
            pane.CollapseForward();
        ASPx.SetElementDisplay(this.elements.GetUploadPanelElement(), visible);
        this.GetUploadControl().SetVisible(visible);
    },

    // Edit
    IsEditOperationAvailable: function(rule, multiselect) {
        switch (this.GetActiveArea()) {
            case FileManagerConsts.SelectedArea.Folders:
                var selectedNode = this.GetTreeView().GetSelectedNode();
                return selectedNode && selectedNode.parent && this.folderRights[rule];
            case FileManagerConsts.SelectedArea.Files:
                var items = this.getSelectedItems();
                if(items.length == 0 || (!multiselect && items.length > 1))
                    return false;
                for(var item, i = 0; item = items[i]; i++) {
                    if(!item.rights[rule])
                        return false;
                }
                return true;
        }
        return false;
    },

    // Delete
    IsDeleteAvailable: function() {
        if(!this.allowDelete)
            return false;
        return this.IsEditOperationAvailable("allowDelete", true);
    },
    TryDelete: function() {
        this.UpdateToolbars(true);
        var successCommand = false;
        switch (this.GetActiveArea()) {
            case FileManagerConsts.SelectedArea.Files:
                var selectedItems = this.getSelectedItems();
                var deleteItems = [];
                if(selectedItems.length > 0) {
                    for(var item, i = 0; item = selectedItems[i]; i++) {
                        if(this.raiseItemDeleting(item))
                            deleteItems.push(item);
                    }
                }
                if(deleteItems.length > 0) {
                    if(confirm(this.deleteConfirmText.replace('{0}', this.joinItemNames(deleteItems, ", ")))) {
                        this.DoDelete(deleteItems);
                        successCommand = true;
                    }
                }
                break;
            case FileManagerConsts.SelectedArea.Folders:
                var folder = this.GetCurrentPath();
                if(this.raiseFolderDeleting() && confirm(this.deleteConfirmText.replace('{0}', folder))) {
                    this.DoDelete();
                    successCommand = true;
                }
                break;
        }
        this.UpdateToolbars();
        return successCommand;
    },
    DoDelete: function(items) {
        var isFolderAreaCommand = this.GetActiveArea() == FileManagerConsts.SelectedArea.Folders;
        this.deleteOldInfo = {
            id: [],
            name: [],
            oldFolderFullName: isFolderAreaCommand ? this.getCurrentFolderParentPath() : this.GetCurrentPath(),
            isFolder: []
        }
        this.fillEditOperationOldInfo(items, isFolderAreaCommand, this.deleteOldInfo);
        if(isFolderAreaCommand)
            this.SendCallback(FileManagerConsts.CallbackCommandId.DeleteItems, true);
        else
            this.SendCallback(FileManagerConsts.CallbackCommandId.DeleteItems, [false, this.joinItemNames(items), this.joinItemsIDs(items), this.joinItemPropertyValues(items, "isFolder")]);
    },

    // Rename
    IsRenameAvailable: function() {
        if(!this.allowRename)
            return false;
        return this.IsEditOperationAvailable("allowRename", false);
    },
    TryRename: function() {
        this.HideRenameInput();
        switch (this.GetActiveArea()) {
            case FileManagerConsts.SelectedArea.Files:
                var files = this.getSelectedItems();
                if(files.length == 1 && this.raiseFileRenaming(files[0])) {
                    this.SetVisibleRenameInputInFileArea(files[0], true);
                    return true;
                }
                break;
            case FileManagerConsts.SelectedArea.Folders:
                var folderNode = this.GetTreeView().GetSelectedNode();
                if(folderNode && this.raiseFolderRenaming())
                    return this.TrySetVisibleRenameInputInFoldersArea(folderNode, true);
                break;
        }
        return false;
    },
    HideRenameInput: function() {
        if(!this.allowRename)
            return;
        var input = this.elements.GetRenameInputElement();
        this.renameMode = false;
        this.UpdateToolbars();

        switch (this.elements.GetRenameElementArea()) {
            case FileManagerConsts.SelectedArea.Files:
                var item = this.items[input["data-fileName"]];
                if(item)
                    this.SetVisibleRenameInputInFileArea(item, false);
                break;
            case FileManagerConsts.SelectedArea.Folders:
                var node = this.GetTreeView().GetRootNode().GetNodeByContentElementID(input.parentNode.id);
                if(node)
                    this.TrySetVisibleRenameInputInFoldersArea(node, false);
                break;
        }
    },
    DoRename: function() {
        if(!this.renameMode && !this.createInFileAreaMode)
            return;
        var newName = this.elements.GetRenameInputElement().value;
        this.elements.GetRenameInputElement().value = "";
        if(newName.length > 0) {            
            if(this.createInFileAreaMode) {
                this.DoCreateCore(newName, false);
                this.createInFileAreaMode = false;
            } else if(this.renameMode) {
                switch (this.elements.GetRenameElementArea()) {
                    case FileManagerConsts.SelectedArea.Files:
                        var items = this.getSelectedItems();
                        var item = items[0];
                        if(items.length == 1 && item.name != newName) {
                            this.renameOldName = item.name;
                            this.SendCallback(FileManagerConsts.CallbackCommandId.RenameItem, [false, newName, item.id, item.isFolder]);
                        }
                        break;
                    case FileManagerConsts.SelectedArea.Folders:
                        if(this.getFolderName() != newName) {
                            this.renameOldName = this.getFolderName();
                            this.SendCallback(FileManagerConsts.CallbackCommandId.RenameItem, [true, newName]);
                        }
                        break;
                }
                this.renameMode = false;
            }

        }
        this.HideCreateHelperElement();
        this.HideRenameInput();
    },
    SetVisibleRenameInputInFileArea: function(item, enable) {
        var input = this.elements.GetRenameInputElement();
        var itemElement = item.getElement();
        ASPx.SetElementDisplay(input, enable);
        if(this.viewMode == FileManagerConsts.ViewMode.Grid) {
            var titleCellElement = ASPx.GetChildByClassName(itemElement, FileManagerConsts.GridColumnTitleCellClassName);
            if(!titleCellElement) {
                for(var cell, i = 0; cell = ASPx.GetChildNodesByTagName(itemElement, "TD")[i]; i++) {
                    if(ASPx.ElementContainsCssClass(cell, "dxgvCommandColumn"))
                        continue;
                    titleCellElement = cell;
                    if(!ASPx.ElementContainsCssClass(cell, FileManagerConsts.ThumbnailCellClassName))
                        break;
                }
            }
            var titleElement = ASPx.GetChildByClassName(titleCellElement, FileManagerConsts.GridColumnTitleClassName);
            if(enable) {
                if(titleElement && !this.nameColumnHasTemplate) {
                    titleCellElement.appendChild(input);
                    this.elements.PrepareRenameInputElement(titleElement);
                    ASPx.SetElementDisplay(titleElement, false);                    
                } else {
                    ASPx.AdjustHeight(titleCellElement);
                    this.hiddenOnRenameHtml = titleCellElement.innerHTML;
                    titleCellElement.innerHTML = "";
                    titleCellElement.appendChild(input);
                    var size = this.elements.CalculateTextSize(item.name, titleCellElement);
                    if(item.isCreateHelperFolder)
                        size.width = FileManagerConsts.DefaultRenameInputWidth;
                    this.elements.PrepareRenameInputElement(null, size);
                }
            } else {
                if(titleElement)
                    ASPx.SetElementDisplay(titleElement, true);
                else
                    titleCellElement.innerHTML = this.hiddenOnRenameHtml;
            }
        }

        if(this.viewMode == FileManagerConsts.ViewMode.Thumbnail) {
            var mask = this.elements.GetRenameMask();
            var titleElement = ASPx.GetNodeByTagName(itemElement.childNodes[0], "DIV", 0);
            if(enable) {
                if(this.isThumbnailsViewFileAreaItemTemplate) {
                    if(!item.isCreateHelperFolder) {
                        ASPx.SetElementDisplay(mask, true);
                        itemElement.appendChild(mask);
                    }
                    itemElement.appendChild(input);
                    this.elements.PrepareRenameInputElement(item.itemContentElement, this.elements.CalculateTextSize(item.name, itemElement), true);
                } else {
                    ASPx.InsertElementAfter(input, titleElement);
                    this.elements.PrepareRenameInputElement(titleElement);
                    ASPx.SetElementDisplay(titleElement, false);
                }
            } else {
                if(this.isThumbnailsViewFileAreaItemTemplate)
                    ASPx.SetElementDisplay(mask, false);
                else
                    ASPx.SetElementDisplay(titleElement, true);
            }
        }
        
        ASPx.GetStateController().ClearElementCache(input);
        if(enable) {
            if(this.createInFileAreaMode)
                input.value = "";
            else {
                input.value = item.name;
                this.renameMode = true;
            }
            ASPx.SetFocus(input);
            this.UpdateToolbars();
        }
        input["data-fileName"] = item.id;
    },

    TrySetVisibleRenameInputInFoldersArea: function(node, enable) {
        if(node == this.GetTreeView().GetNode(0))
            return false;

        var input = this.elements.GetRenameInputElement();
        var element = node.GetHtmlElement();
        var titleElement = ASPx.GetChildByClassName(element, "dxtv-ndTxt");
        ASPx.InsertElementAfter(input, titleElement);
        ASPx.SetElementDisplay(input, enable);
        ASPx.ClearHeight(input);
        this.elements.PrepareRenameInputElement(titleElement, { width: FileManagerConsts.DefaultRenameInputWidth });
        ASPx.GetStateController().ClearElementCache(input);
        if(enable) {
            input.value = node.GetText();
            ASPx.SetFocus(input);
            this.renameMode = true;
            this.UpdateToolbars();
            ASPx.RemoveClassNameFromElement(titleElement, "dx-vam");
        } else
            ASPx.AddClassNameToElement(titleElement, "dx-vam");
        ASPx.SetElementDisplay(titleElement, !enable);
        this.GetTreeView().AdjustControl();
    },
    // Move
    IsMoveAvailable: function() {
        if(!this.allowMove)
            return false;
        return this.IsEditOperationAvailable("allowMove", true);
    },
    TryMove: function() {
        this.folderDialogCommand = FileManagerConsts.CallbackCommandId.MoveItems;
        return this.TryShowFolderBrowserDialog();
    },

    // Copy
    IsCopyAvailable: function() {
        if(!this.allowCopy)
            return false;
        return this.IsEditOperationAvailable("allowCopy", true);
    },
    TryCopy: function() {
        this.folderDialogCommand = FileManagerConsts.CallbackCommandId.CopyItems;
        return this.TryShowFolderBrowserDialog();
    },

    // Change item position
    TryShowFolderBrowserDialog: function() {
        var area = this.GetActiveArea();
        var raiseEventResult = null;
        var cancel = true;
        switch (area) {
            case FileManagerConsts.SelectedArea.Files:
                var approvedItems = [];
                this.forEachItem(function(item) {
                    if(item.IsSelected() && !item.isParentFolderItem && this.GetFolderBrowserEventResult(item))
                        approvedItems.push(item);
                }.aspxBind(this));
                if(approvedItems.length > 0) {
                    this.SendCallback(FileManagerConsts.CallbackCommandId.ShowFolderBrowserDialog, "");
                    this.movedItems = approvedItems;
                    return true;
                }
                else
                    this.folderDialogCommand = null;
                break;
            case FileManagerConsts.SelectedArea.Folders:
                if(this.GetFolderBrowserEventResult()) {
                    this.SendCallback(FileManagerConsts.CallbackCommandId.ShowFolderBrowserDialog, (this.folderDialogCommand == "Copy").toString());
                    return true;
                } else
                    this.folderDialogCommand = null;
                break;
        }
        return false;
    },
    GetFolderBrowserEventResult: function(item) {
        switch(this.folderDialogCommand) {
            case FileManagerConsts.CallbackCommandId.CopyItems:
                return item ? this.raiseItemCopying(item) : this.raiseFolderCopying();
            case FileManagerConsts.CallbackCommandId.MoveItems:
                return item ? this.raiseItemMoving(item) : this.raiseFolderMoving();
        }
    },
    ChangeItemsPosition: function() {
        var node = this.GetFolderBrowserTreeView().GetSelectedNode();
        var isFolderAreaCommand = this.GetActiveArea() == FileManagerConsts.SelectedArea.Folders;
        if(this.GetActiveArea() == FileManagerConsts.SelectedArea.None || !node.enabled || (!isFolderAreaCommand && this.movedItems.length == 0))
            return;
        this.changeItemPositionOldInfo = {
            id: [],
            name: [],
            oldFolderFullName: isFolderAreaCommand ? this.getCurrentFolderParentPath() : this.GetCurrentPath(),
            isFolder: []
        }
        this.fillEditOperationOldInfo(this.movedItems, isFolderAreaCommand, this.changeItemPositionOldInfo);
        var targetPath = this.GetFoldersPath(node, true);
        var targetIdPath = this.GetFoldersPath(node, false, "\\\\", true);
        var firstArg = [isFolderAreaCommand.toString(), targetPath, targetIdPath].join(FileManagerConsts.ItemPropertyValueSeparator);
        var args = [firstArg];
        if(!isFolderAreaCommand) {
            args.push(this.joinItemNames(this.movedItems));
            args.push(this.joinItemsIDs(this.movedItems));
            args.push(this.joinItemPropertyValues(this.movedItems, "isFolder"));
        }
        this.SendCallback(this.folderDialogCommand, args);
    },
    ShowFolderBrowserPopup: function(renderFolders) {
        var popup = this.GetFolderBrowserPopup();
        var container = this.elements.GetFolderBrowserPopupFoldersContainer();
        var popupHeight = this.GetHeight() / 2;
        if(popup.GetHeight() < popupHeight)
            ASPx.SetOffsetHeight(container, popupHeight);
        popup.ShowAtElement(this.GetMainElement());
        if(renderFolders)
            container.innerHTML = renderFolders;
        popup.UpdatePosition();
    },
    HideFolderBrowserPopup: function() {
        var popup = this.GetFolderBrowserPopup();
        popup.Hide();
    },
    FolderBrowserOkButtonClick: function() {
        this.ChangeItemsPosition();
        this.HideFolderBrowserPopup();
    },
    FolderBrowserCancelButtonClick: function() {
        this.HideFolderBrowserPopup();
    },

    // Create
    IsCreateAvailable: function() {
        if(!this.allowCreate)
            return false;
        return this.folderRights.allowCreate && (this.GetActiveArea() == FileManagerConsts.SelectedArea.Folders || this.showFoldersInFileArea);
    },
    TryCreate: function() {
        if(this.raiseFolderCreating()) {
            if(this.showFoldersInFileArea && this.GetActiveArea() != FileManagerConsts.SelectedArea.Folders)
                this.ShowCreateHelperElement();
            else
                this.SendCallback(FileManagerConsts.CallbackCommandId.CreateQuery, this.GetCurrentPath(true));
            return true;
        }
        return false;
    },
    HideCreateTextbox: function() {
        if(!this.createMode)
            return;
        this.createMode = false;
        this.UpdateToolbars();
        this.elements.createNodeTextBox = null;
        var createNode = this.GetTreeView().GetSelectedNode();
        this.GetTreeView().SetSelectedNode(createNode.parent);
        this.Refresh();
    },
    PrepareCreateNode: function() {
        var tb = ASPx.GetNodeByTagName(this.GetTreeView().GetSelectedNode().GetHtmlElement(), "INPUT", 0);
        this.elements.createNodeTextbox = tb;
        var _this = this;
        ASPx.Evt.AttachEventToElement(tb, "keydown", function(evt) {
            switch (ASPx.Evt.GetKeyCode(evt)) {
                case ASPx.Key.Enter:
                    ASPx.Evt.PreventEvent(evt);
                    _this.DoCreate();
                    break;
                case ASPx.Key.Esc:
                    _this.HideCreateTextbox();
                    ASPx.Evt.PreventEvent(evt);
                    break;
            }
        });
        ASPx.Evt.AttachEventToElement(tb, "keypress", function(evt) { // B185439
            if(ASPx.Evt.GetKeyCode(evt) == ASPx.Key.Enter)
                return ASPx.Evt.PreventEventAndBubble(evt);
        });
        ASPx.Evt.AttachEventToElement(tb, "blur", function(evt) {
            _this.DoCreate();
        });
        if(!ASPxClientUtils.iOSPlatform)
            ASPx.SetFocus(tb);
    },
    ShowCreateHelperElement: function() {
        if(this.isDetailsModeVirtScrollEnabled())
            this.GetFilesGridView().lockVirtualScrolling(true);

        var item = this.GetCreateHelperItem();
        item.SetVisible(true);
        ASPx.AddClassNameToElement(item.getElement(), item.selectionActiveCssClass);
        this.createInFileAreaMode = true;
        this.SetVisibleRenameInputInFileArea(item, true);
        if(this.viewMode == FileManagerConsts.ViewMode.Grid && this.GetFilesGridView().GetEmptyDataItem())
            ASPx.SetElementDisplay(this.GetFilesGridView().GetEmptyDataItem(), false);
    },
    HideCreateHelperElement: function() {
        var item = this.GetCreateHelperItem();
        if(!item)
            return;
        item.unfocus();
        item.SetVisible(false);
        this.createInFileAreaMode = false;
        if(this.viewMode == FileManagerConsts.ViewMode.Grid && this.GetFilesGridView().GetEmptyDataItem())
            ASPx.SetElementDisplay(this.GetFilesGridView().GetEmptyDataItem(), true);
    },
    GetCreateHelperItem: function() {
        if(this.viewMode == FileManagerConsts.ViewMode.Grid)
            return this.detailsCreateHelperItem;
        else
            return this.thumbCreateHelperItem;
    },
    DoCreate: function() {
        if(!this.createMode)
            return;
        var folder = this.GetFoldersPath(this.GetTreeView().GetSelectedNode().parent, true);
        var newName = this.elements.createNodeTextbox.value;
        if(newName.length > 0) {
            this.UpdateToolbars();
            this.elements.createNodeTextbox.value = "";
            this.DoCreateCore(newName, true);
        }
        else
            this.HideCreateTextbox();
        this.createMode = false;
    },
    DoCreateCore: function(newName, selectNewFolder) {
        this.SendCallback(FileManagerConsts.CallbackCommandId.Create, [newName, selectNewFolder]);
    },

    // Download
    GetDownloadFiles: function(isNeedRaiseFileDownloading) {
        var files = this.getSelectedItems();
        var downloadFiles = [];
        for(var file, i = 0; file = files[i]; i++) {
            if(!file.rights.allowDownload)
                return [];
            if(!isNeedRaiseFileDownloading || this.raiseFileDownloading(file))
                downloadFiles.push(file);
        }
        return downloadFiles;
    },
    TryDownload: function() {
        var downloadFiles = this.GetDownloadFiles(true);
        if(downloadFiles.length == 0)
            return false;
        this.SendPostBack(this.GetArgumentsString(FileManagerConsts.CallbackCommandId.Download, [this.joinItemNames(downloadFiles), this.joinItemsIDs(downloadFiles)]));
        return true;
    },
    IsDownloadButtonActive: function() {
        return (this.GetActiveArea() == FileManagerConsts.SelectedArea.Files) && this.GetDownloadFiles(false).length > 0;
    },

    // Commons
    joinItemNames: function(items, separator) {
        return this.joinItemPropertyValues(items, "name", separator);
    },
    joinItemsIDs: function(items, separator) {
        return this.joinItemPropertyValues(items, "id", separator);
    },
    joinItemPropertyValues: function(items, propertyName, separator) {
        separator = separator ? separator : FileManagerConsts.ItemPropertyValueSeparator;
        var propertyValues = "";
        for(var item, i = 0; item = items[i]; i++) {
            if(propertyValues.length > 0)
                propertyValues += separator;
            propertyValues += item[propertyName];
        }
        return propertyValues;
    },
    IsEditMode: function() {
        return this.renameMode || this.createMode || this.folderDialogCommand != null;
    },
    showError: function(commandName, errorText, errorCode) {
        if(errorCode == undefined || errorText == undefined)
            return;
        var errorCodes = errorCode.split('|');
        var errorTexts = errorText.split('|');
        var resultErrorText = "";

        for(var i = 0; i < errorCodes.length; i++) {
            var args = this.raiseErrorOccurred(commandName, errorTexts[i], errorCodes[i]);
            if(args.showAlert) {
                if(resultErrorText.length > 0)
                    resultErrorText += "\r\n";
                resultErrorText += args.errorText;
            }
        }
        if(resultErrorText.length > 0) {
            var args = this.raiseErrorAlertDisplaying(commandName, resultErrorText);
            if(args.showAlert && args.errorText)
                alert(args.errorText);
        }
    },
    CorrectScroll: function(item) {
        if(this.viewMode == FileManagerConsts.ViewMode.Grid || !item || !item.GetVisible())
            return;

        var element = item.getElement(),
            scrollContainer = this.elements.GetItemsPaneContainer(),
            containerHeight = scrollContainer.offsetHeight,
            itemHeight = element.offsetHeight,
            selectItemOffsetTop = ASPx.GetAbsolutePositionY(element),
            scrollContainerOffsetTop = ASPx.GetAbsolutePositionY(scrollContainer);

        if(scrollContainerOffsetTop > selectItemOffsetTop)
            scrollContainer.scrollTop -= scrollContainerOffsetTop - selectItemOffsetTop;
        else if(selectItemOffsetTop + itemHeight > scrollContainerOffsetTop + containerHeight)
            scrollContainer.scrollTop += selectItemOffsetTop + itemHeight - scrollContainerOffsetTop - containerHeight;
    },
    scrollToItem: function(item) {
        var element = item.getElement(),
            scrollContainer = this.elements.GetItemsPaneContainer();
        this.elements.GetItemsPaneContainer().scrollTop += ASPx.GetAbsoluteY(element) - ASPx.GetAbsoluteY(scrollContainer);
    },
    fillEditOperationOldInfo: function(items, isFolderAreaCommand, oldInfoObject) {
        if(!isFolderAreaCommand) {
            for(var item, i = 0; item = items[i]; i++) {
                oldInfoObject.id.push(item.id);
                oldInfoObject.name.push(item.name);
                oldInfoObject.isFolder.push(item.isFolder);
            }
        }
        else {
            oldInfoObject.id.push(this.GetCurrentFolderId());
            oldInfoObject.name.push(this.getFolderName());
            oldInfoObject.isFolder.push(true);
        }
    },

    // Selection
    scrollToSelection: function(selectItemNames) {
        if(selectItemNames && selectItemNames.length > 0) {
            for(var itemName, i = 0; itemName = selectItemNames[i]; i++) {
                var item = this.items[itemName];
                if(item && !item.outdated && !item.clientInvisibility && item.GetVisible()) {
                    this.scrollToItem(item);
                    return;
                }
            }
        }
    },
    resetSelection: function(needSelectItemNames) {
        var selectedItems = [];
        var visibleFiles = this.GetVisibleItems();
        if(needSelectItemNames && needSelectItemNames.length > 0) {
            for(var itemName, i = 0; itemName = needSelectItemNames[i]; i++) {
                var item = this.items[itemName];
                if(item && item.GetVisible()) {
                    if(selectedItems.length == 0)
                        item.focus();
                    item.Select();
                    selectedItems.push(item);
                }
            }
        }
        this.unselectAllItems(selectedItems);
    },
    unselectAllItems: function(excludes) {
        var items = this.getSelectedItems();
        for(var item, i = 0; item = items[i]; i++) {
            if(!excludes || ASPx.Data.ArrayIndexOf(excludes, item) < 0)
                item.Unselect();
        }
    },
    selectAllItems: function() {
        if(!this.allowMultiSelect)
            return;
        var items = this.GetVisibleItems();
        for(var item, i = 0; item = items[i]; i++) {
            if(!item.IsSelected())
                item.Select();
        }
    },
    selectItem: function(itemName) {
        this.selectItems([itemName]);
    },
    focusItem: function(fileName, skipEvent) {
        var file = this.items[fileName];
        if(file)
            return file.focus(skipEvent);
    },
    selectItems: function(itemIDs, skipEvent, skipFocus) {
        var selected = [];
        for(var id, i = 0; id = itemIDs[i]; i++) {
            var item = this.items[id];
            if(item && item.Select(skipEvent, skipFocus))
                selected.push(id);
        }
        return selected;
    },
    saveSelectedItemsToState: function() {
        var selectedItemsInfo = [];
        this.forEachItem(function(item) {
            if(item.IsSelected())
                selectedItemsInfo.push(item.id + FileManagerConsts.ItemPropertyValueSeparator + item.name + FileManagerConsts.ItemPropertyValueSeparator + item.isFolder);
        });
        this.UpdateStateField(FileManagerConsts.StateField.ItemSelected, selectedItemsInfo);
    },
    saveFocusedItemToState: function() {
        if(!this.focusedItem)
            return;
        this.UpdateStateField(FileManagerConsts.StateField.ItemFocused, this.focusedItem.id + FileManagerConsts.ItemPropertyValueSeparator + this.focusedItem.isFolder);
    },
    correctItemsAreaPopupPosition: function(popup) {
        var popupElement = this.viewMode == FileManagerConsts.ViewMode.Grid ? this.GetFilesGridView().GetMainElement() : this.elements.GetItemsPaneContainer();
        var scrollbarWidth = ASPx.PxToInt(ASPx.GetCurrentStyle(popupElement.parentNode).width) - ASPx.PxToInt(ASPx.GetCurrentStyle(popupElement).width);
        popup.ShowAtPos(ASPx.GetAbsolutePositionX(popupElement) + popupElement.offsetWidth - scrollbarWidth - popup.GetWidth(),
                        ASPx.GetAbsolutePositionY(popupElement) + popupElement.offsetHeight - popup.GetHeight());
    },

    // Client-side API
    Refresh: function() {
        if(!this.enabled)
            return;
        this.UpdateFileList(FileManagerConsts.CallbackCommandId.Refresh);
    },
    ExecuteCommand: function(commandName) {
        if(this.GetActiveArea() == FileManagerConsts.SelectedArea.None)
            this.SetActiveArea(this.lastActiveItemsArea);
        switch(commandName) {
            case ASPxClientFileManagerCommandConsts.Delete:
                return this.TryDelete();
            case ASPxClientFileManagerCommandConsts.Rename:
                return this.TryRename();
            case ASPxClientFileManagerCommandConsts.Create:
                return this.TryCreate();
            case ASPxClientFileManagerCommandConsts.Download:
                return this.TryDownload();
            case ASPxClientFileManagerCommandConsts.Move:
                return this.TryMove();
            case ASPxClientFileManagerCommandConsts.Copy:
                return this.TryCopy();
            case ASPxClientFileManagerCommandConsts.Open:
                return this.TryOpen();
        }
    },
    GetSelectedFile: function() {
        return this.GetSelectedItem(FileManagerConsts.FileAreaItemTypes.File);
    },
    GetSelectedItem: function(type) {
        if(!this.enabled)
            return null;
        var items = this.getSelectedItems();
        if(items.length > 0 && (!type || items[0].type == type))
            return items[0];
        return null;
    },
    GetSelectedItems: function() {
        return this.getSelectedItems();
    },
    GetItems: function() {
        var files = [];
        this.forEachItem(function(file) {
            files.push(file);
        });
        return files;
    },
    GetAllItems: function(onCallback) {
        this.apiCommandCallback = onCallback;
        this.SendCallback(FileManagerConsts.CallbackCommandId.ApiCommand);
    },
    GetToolbarItemByCommandName: function(commandName) {
        return this.GetToolbar().GetItemByName(commandName);
    },
    GetContextMenuItemByCommandName: function(commandName) {
        return this.GetContextMenu().GetItemByName(commandName);
    },
    GetCurrentFolderPath: function(separator, skipRootFolder) {
        if(!this.enabled)
            return null;
        if(this.selectedFolder || this.currentPath != null)
            return this.GetCurrentPath(skipRootFolder, separator);
    },
    SetCurrentFolderPath: function(path, onCallback) {
        this.SetCurrentFolderPathInternal(path, onCallback, "", false);
    },
    GetCurrentFolderId: function () {
        return this.currentFolderId;
    },
    SetCurrentFolderPathInternal: function(path, onCallback, newFolderID, selectPreviousFolder) {
        this.cancelActiveActions();
        var currentPath = this.GetCurrentPath(true);
        if(selectPreviousFolder)
            this.needFocusItemName = currentPath;
        if(this.foldersHidden) {
            if(this.showFoldersInFileArea || ((this.showParentFolder || this.breadCrumbsEnabled) && currentPath.indexOf(path) == 0)) {
                this.needCurrentFolderChangedRaise = true;
                this.SendCallback(FileManagerConsts.CallbackCommandId.ChangeCurrentFolderCallback, [path, newFolderID]);
            }
            return;
        }
        this.SetActiveArea(FileManagerConsts.SelectedArea.Folders);

        var splittedPath = this.splitPath(path);
        var treeView = this.GetTreeView();
        var node = treeView.GetRootNode().GetNode(0);
        if(splittedPath.length > 0) {
            for(var i = 0, folder; folder = splittedPath[i]; i++) {
                if(treeView.AreChildNodesLoaded(node)) {
                    node = this.findTreeViewNodeChild(node, folder);
                    if(!node)
                        return false;
                }
                else { // Virtual Mode
                    this.delayedSetCurrentFolderPath = path.replace("/", "\\");
                    this.delayedCallbackFunction = onCallback;
                    node.SetExpanded(true);
                    return true;
                }
            }
        }
        if(this.selectedFolder != node) {
            this.GetTreeView().SetSelectedNode(node);
            this.selectedFolder = node;
            this.needCurrentFolderChangedRaise = true;
            this.delayedCallbackFunction = onCallback;
            this.expandNodeRecursive(node);
            this.UpdateFileList(FileManagerConsts.CallbackCommandId.GetFileList);
            return true;
        }
        return false;
    },
    PerformCallback: function(args) {
        if(!ASPx.IsExists(args)) args = "";
        this.SendCallback(FileManagerConsts.CallbackCommandId.CustomCallback, args);
    },
    performVirtualScrollingCallback: function(itemIndex, pageItemsCount) {
        if(this.viewMode == FileManagerConsts.ViewMode.Thumbnail)
            this.SendCallback(FileManagerConsts.CallbackCommandId.VirtualScrollingCallback, [itemIndex, pageItemsCount], true);
        else
            this.GetFilesGridView().NextPage();
    },
    expandNodeRecursive: function(node) {
        var nodeParent = node.parent;
        while(nodeParent) {
            nodeParent.SetExpanded(true);
            nodeParent = nodeParent.parent;
        }
    },
    findTreeViewNodeChild: function(parentNode, text) {
        for(var childNode, j = 0; childNode = parentNode.nodes[j]; j++) {
            if(this.getFolderNameByNode(childNode) == text)
                return childNode;
        }
    },
    splitPath: function(path) {
        if(!path) return [];
        var separator = "/";
        if(path.indexOf(separator) == -1)
            separator = "\\";
        return path.split(separator);
    },

    // Keyboard
    KeyProcessEnter: function() {
        if(this.GetActiveArea() != FileManagerConsts.SelectedArea.Files)
            return;
        if(!this.renameMode && !this.createMode) {
            if(this.allowMultiSelect) {
                if(this.focusedItem)
                    this.TryOpenItem(this.focusedItem);
            }
            else {
                var selectedFiles = this.getSelectedItems();
                if(selectedFiles.length == 1)
                    this.TryOpenItem(selectedFiles[0]);
            }
        }
    },
    KeyProcessEsc: function() {
        this.cancelActiveActions();
    },
    KeyProcessF2: function() {
        if(this.allowRename && this.IsRenameAvailable() && !this.IsEditMode())
            this.TryRename();
    },
    KeyProcessF6: function() {
        if(this.allowMove && this.IsMoveAvailable() && !this.IsEditMode())
            this.TryMove();
    },
    KeyProcessDelete: function() {
        if(this.allowDelete && this.IsDeleteAvailable() && !this.IsEditMode())
            this.TryDelete();
    },
    KeyProcessTab: function() {
        if(this.folderDialogCommand != null) {
            if(this.GetActiveArea() == FileManagerConsts.SelectedArea.Files)
                this.elements.GetAccessabilityInput().focus();
            this.SetActiveArea(FileManagerConsts.SelectedArea.None);
        }
    },
    KeyProcessF7: function() {
        if(this.allowCreate && this.IsCreateAvailable() && !this.IsEditMode())
            this.TryCreate();
    },
    KeyProcessSpace: function() {
        var focusFile = this.focusedItem;
        if(this.allowMultiSelect && focusFile) {
            if(!(this.getSelectedItems().length == 1 && focusFile.IsSelected()))
                focusFile.invertSelection();
        }
    },
    KeyProcessCtrlA: function() {
        if(this.GetActiveArea() == FileManagerConsts.SelectedArea.Files)
            this.selectAllItems();
    },
    KeyProcessHome: function() {
        this.keyProcessNavigateToEdge(0);
    },
    KeyProcessEnd: function() {
        this.keyProcessNavigateToEdge(1);
    },
    keyProcessNavigateToEdge: function(position) {
        if(this.GetActiveArea() == FileManagerConsts.SelectedArea.Files) {
            var files = this.GetVisibleItems();
            if(files.length == 0)
                return;
            var file = files[position > 0 ? files.length - 1 : 0];
            if(this.allowMultiSelect)
                file.focus();
            else
                file.Select();
        }
    },
    cancelActiveActions: function() {
        if(this.renameMode)
            this.HideRenameInput();
        else if(this.folderDialogCommand != null)
            this.HideFolderBrowserPopup();
        else if(this.createMode)
            this.HideCreateTextbox();
        else
            this.SetActiveArea(FileManagerConsts.SelectedArea.None);
    },
    isExists: function() {
        return ASPx.GetControlCollection().Get(this.name) == this;
    }
});
ASPxClientFileManager.Cast = ASPxClientControl.Cast;
var ASPxClientFileManagerItem = ASPx.CreateClass(null, {
    constructor: function(fileManager, index, itemInfo) {
        this.fileManager = fileManager;
        this.index = index;
        this.name = itemInfo.n;
        this.id = itemInfo.id;
        this.type = itemInfo.it;
        this.isFolder = this.type != FileManagerConsts.FileAreaItemTypes.File;

        this.outdated = itemInfo.outdated;
        if(this.outdated)
            return;

        this.clientInvisibility = itemInfo.cn;
        this.SetRights(itemInfo.r);
        this.imageSrc = itemInfo.i;
        this.imageIndex = itemInfo.ci;
        this.elementID = this.fileManager.elements.itemsContainerId + FileManagerConsts.ItemPostfix + this.index.toString();
        
        this.isParentFolderItem = this.type == FileManagerConsts.FileAreaItemTypes.ParentFolder;
        this.isCreateHelperFolder = this.type == FileManagerConsts.FileAreaItemTypes.CreateHelperFolder;
        var idPostfix = this.isCreateHelperFolder ? FileManagerConsts.CreateHelperFolderPostfix : this.index.toString();
        this.elementID = this.fileManager.elements.itemsContainerId + FileManagerConsts.ItemPostfix + idPostfix;

        this.cssClass = this.isFolder ? this.fileManager.styles.fileAreaFolderCssClass : this.fileManager.styles.fileCssClass;
        this.contentCssClass = this.isFolder ? this.fileManager.styles.fileAreaFolderContentCssClass : this.fileManager.styles.fileContentCssClass;
        this.selectionActiveCssClass = this.isFolder ? this.fileManager.styles.fileAreaFolderSelectionActiveCssClass : this.fileManager.styles.fileSelectionActiveCssClass;
        this.selectionInactiveCssClass = this.isFolder ? this.fileManager.styles.fileAreaFolderSelectionInactiveCssClass: this.fileManager.styles.fileSelectionInactiveCssClass;
        this.hoverCssClass = this.isFolder ? this.fileManager.styles.fileAreaFolderHoverCssClass : this.fileManager.styles.fileHoverCssClass;
        this.focusCssClass = this.isFolder ? this.fileManager.styles.fileAreaFolderFocusCssClass : this.fileManager.styles.fileFocusCssClass;

        var tooltip = this.isCreateHelperFolder ? "" : (this.name + "\r\n" + (itemInfo.t ? itemInfo.t.replace(/\|\|/g, "\r\n") : ""));
        if(this.fileManager.viewMode == FileManagerConsts.ViewMode.Thumbnail) {
            this.element = document.getElementById(this.elementID);
            if(!this.element && !this.clientInvisibility)
                this.element = this.fileManager.elements.createItemElement(this);
            if(this.element) {
                if(this.fileManager.allowMultiSelect && !this.isParentFolderItem && !this.isCreateHelperFolder) {
                    var checkBox = ASPx.CreateHtmlElementFromString(ASPxClientFileManager.PrepareTemplate( this.fileManager.thumbnailCheckBox, { itemId: this.elementID } ));
                    this.element.appendChild(checkBox);
                    this.fileManager.elements.createItemCheckBoxInstance(this);
                }
                if(ASPxClientUtils.touchUI && !ASPxClientUtils.windowsPlatform)
                    ASPx.TouchUIHelper.AttachDoubleTapEventToElement(this.element, function(evt) {  this.fileManager.onThumbnailViewFilesAreaItemDblClick(this, evt); }.aspxBind(this));                
                else
                    ASPx.Evt.AttachEventToElement(this.element, "dblclick", function(evt) { this.fileManager.onThumbnailViewFilesAreaItemDblClick(this, evt); }.aspxBind(this));
                this.itemContentElement = ASPx.GetNodeByTagName(this.element, "div", 0);
                this.fileManager.elements.PrepareItemElement(this, tooltip);
            }
            if(this.isCreateHelperFolder) {
                this.fileManager.thumbCreateHelperItem = this;
                this.fileManager.HideCreateHelperElement();
            }
        }
        else {
            this.tooltip = tooltip;
            if(this.isCreateHelperFolder)
                this.fileManager.detailsCreateHelperItem = this;
        }

        this.visible = true;
        this.isSelected = false;
    },

    SetRights: function(obj) {
        var checkPermission = function(literal) {
            return typeof (obj) == "undefined" || obj.indexOf(literal) > -1;
        };
        this.rights = {
            allowMove: checkPermission("m"),
            allowRename: checkPermission("r"),
            allowDelete: checkPermission("d"),
            allowDownload: checkPermission("l"),
            allowCopy: checkPermission("o")
        };
    },

    SetVisible: function(visible) {
        if(this.index < 0)
            return;
        if(!visible && this.IsSelected())
            this.Unselect();
        ASPx.SetElementDisplay(this.getElement(), visible);
        this.visible = visible;
    },
    GetVisible: function() {
        return this.index > -1 && this.visible && !!this.getElement();
    },
    GetVisibleIndex: function() {
        var files = this.fileManager.GetVisibleItems();
        for(var i = 0; i < files.length; i++) {
            if(files[i] == this)
                return i;
        }
        return -1;
    },
    focus: function(skipEvent, skipSelect) {
        if(!this.fileManager.allowMultiSelect)
            return;
        if(!this.GetVisible())
            return;
        if(this.isFocused())
            return;
        this.fileManager.forEachItem(function(file) {
            file.unfocus();
        });
        this.fileManager.focusedItem = this;
        if(this.fileManager.viewMode == FileManagerConsts.ViewMode.Thumbnail) {
            this.getElement().className += " " + this.focusCssClass;
            this.fileManager.CorrectScroll(this);
        }
        else {
            var grid = this.fileManager.GetFilesGridView();
            if(grid.GetFocusedRowIndex() != this.index)
                grid.SetFocusedRowIndex(this.index);
        }
        if(!skipEvent)
            this.fileManager.raiseFocusedItemChangedEvent(this);
        var gridWithMultiSelect = this.fileManager.allowMultiSelect && this.fileManager.viewMode == FileManagerConsts.ViewMode.Grid;
        if(!skipSelect && this.fileManager.getSelectedItems().length == 0 && !gridWithMultiSelect)
            this.Select(skipEvent);
        this.fileManager.saveFocusedItemToState();
        this.fileManager.UpdateToolbars();
        return true;
    },
    unfocus: function() {
        if(!this.GetVisible())
            return;
        var element = this.getElement();
        if(this.fileManager.viewMode == FileManagerConsts.ViewMode.Thumbnail) {
            var focusClass = this.focusCssClass;
            if(element.className.indexOf(focusClass) > -1)
                element.className = ASPx.Str.Trim(element.className.replace(focusClass, ""));
        } else
            this.fileManager.GetFilesGridView().SetFocusedRowIndex(-1);
        if(this.fileManager.focusedItem == this)
            this.fileManager.focusedItem = null;
        this.fileManager.UpdateToolbars();
    },
    isFocused: function() {
        return this.fileManager.focusedItem == this;
    },
    SetSelected: function(selected) {
        if(selected)
            this.Select();
        else
            this.Unselect();
    },
    Select: function(skipEvent, skipFocus) {
        if(!this.isExists() || (this.isParentFolderItem && this.fileManager.allowMultiSelect))
            return;
        if(!this.fileManager.allowMultiSelect)
            this.fileManager.unselectAllItems([this]);
        if(this.clientInvisibility) {
            this.isSelected = true;
            return;
        }
        if(!this.GetVisible())
            return;
        return this.SelectCore(skipEvent, skipFocus);
    },
    SelectCore: function(skipEvent, skipFocus) {
        if(this.IsSelected()) {
            this.UpdateSelectState(false);
            return true;
        }
        this.isSelected = true;

        if(this.fileManager.viewMode == FileManagerConsts.ViewMode.Thumbnail)
            ASPx.GetStateController().SelectElementBySrcElement(this.getElement());
        else {
            var grid = this.fileManager.GetFilesGridView();
            if(this.fileManager.allowMultiSelect) {
                if(!grid.IsRowSelectedOnPage(this.index))
                    grid.SelectRowOnPage(this.index);
            }
            else {
                if(grid.GetFocusedRowIndex() != this.index)
                    grid.SetFocusedRowIndex(this.index);
            }
        }

        this.fileManager.saveSelectedItemsToState();
        this.updateCheckBoxState(true);

        if(!skipEvent) {
            if(!this.isFolder)
                this.fileManager.raiseSelectedFileChangedEvent(this);
            this.fileManager.raiseSelectionChanged(this, true);
        }
        if(this.fileManager.allowMultiSelect && !this.fileManager.focusedItem && !skipFocus)
            this.focus(skipEvent);
        this.fileManager.SetActiveArea(FileManagerConsts.SelectedArea.Files);
        return true;
    },
    Unselect: function(skipEvent) {
        if(!this.isExists())
            return;
        if(!this.GetVisible())
            return;
        if(!this.IsSelected())
            return;
        this.UpdateSelectState(false);
        this.isSelected = false;
        if(this.fileManager.viewMode == FileManagerConsts.ViewMode.Thumbnail)
            ASPx.GetStateController().DeselectElementBySrcElement(this.getElement());
        else {
            var grid = this.fileManager.GetFilesGridView();
            if(this.fileManager.allowMultiSelect) {
                if(grid.IsRowSelectedOnPage(this.index))
                    grid.UnselectRowOnPage(this.index);
            }
            else {
                if(grid.GetFocusedRowIndex() != this.index)
                    grid.SetFocusedRowIndex(-1);
            }
        }
        this.fileManager.saveSelectedItemsToState();
        this.fileManager.UpdateToolbars();
        if(!skipEvent)
            this.fileManager.raiseSelectionChanged(this, false);
        this.updateCheckBoxState(false);
    },
    updateCheckBoxState: function(checked) {
        if(!this.fileManager.allowMultiSelect || !this.checkBox)
            return;
        this.checkBox.SetChecked(checked);
        this.fileManager.focusMainElement()
        var container = this.fileManager.elements.GetItemsContainer();
        if(this.fileManager.getSelectedItems().length > 0)
            ASPx.AddClassNameToElement(container, FileManagerConsts.ShowAllFileAreaCheckBoxesClassName);
        else
            ASPx.RemoveClassNameFromElement(container, FileManagerConsts.ShowAllFileAreaCheckBoxesClassName);        
    },
    invertSelection: function() {
        if(this.IsSelected())
            this.Unselect();
        else
            this.Select();
    },
    IsSelected: function() {
        return this.isExists() && this.isSelected;
    },
    UpdateSelectState: function(toInactive) {
        if(!this.GetVisible())
            return;
        var element = this.fileManager.elements.GetItemElement(this);
        if(!ASPx.IsExists(element))
            return;
        var className = element.className;
        if(this.IsSelected()) {
            className = toInactive
                ? className.replace(this.selectionActiveCssClass, this.selectionInactiveCssClass)
                : className.replace(this.selectionInactiveCssClass, this.selectionActiveCssClass);
            if(toInactive && className.indexOf(this.selectionInactiveCssClass) == -1)
                className += " " + this.selectionInactiveCssClass;
            else if(!toInactive && (className.indexOf("dxgvFocusedRow") > -1 && this.fileManager.allowMultiSelect))
                className = className.replace(this.selectionActiveCssClass, "");
        }
        if(this.isFocused()) {
            if(toInactive)
                className = className.replace(this.focusCssClass, "");
            else if(className.indexOf(this.focusCssClass) < 0)
                className += " " + this.focusCssClass;
        }
        element.className = ASPx.Str.Trim(className);
    },
    getElement: function() {
        if(this.element)
            return this.element;
        return this.fileManager.elements.GetItemElement(this);
    },
    GetFullName: function(separator, skipRootFolder) {
        return ASPxClientFileManager.GetItemFullName(this.name, this.fileManager, separator, skipRootFolder);
    },
    toString: function() {
        return this.name;
    },
    isExists: function() {
        return !this.outdated && this.fileManager && this.fileManager.items[this.id] == this && this.fileManager.isExists();
    }
});
var ASPxClientFileManagerFile = ASPx.CreateClass(ASPxClientFileManagerItem, {
    constructor: function(fileManager, index, fileInfo) {
        this.constructor.prototype.constructor.call(this, fileManager, index, fileInfo);
    },
    Download: function() {
        if(!this.isExists())
            return;
        this.Select();
        if(this.IsSelected())
            this.fileManager.TryDownload();
    }
});
var ASPxClientFileManagerFolder = ASPx.CreateClass(ASPxClientFileManagerItem, {
    constructor: function(fileManager, index, fileInfo) {
        this.constructor.prototype.constructor.call(this, fileManager, index, fileInfo);
    }
});

// Static methods
ASPxClientFileManager.GetItemFullName = function(name, fileManager, separator, skipRootFolder) {
    if(!separator)
        separator = FileManagerConsts.PathSeparator;
    var folderPath = fileManager.GetCurrentFolderPath(separator, skipRootFolder);
    return folderPath ? (folderPath + separator + name) : name;
};
ASPxClientFileManager.OnFoldersTreeViewNodeClick = function(s, e) {
    ASPxClientFileManager.GetFileManagerByInnerControl(s.name, FileManagerConsts.FoldersPostfix).OnFolderClick(e.node);
};
ASPxClientFileManager.OnFoldersTreeViewNodeExpanding = function(s, e) {
    ASPxClientFileManager.GetFileManagerByInnerControl(s.name, FileManagerConsts.FoldersPostfix).OnFolderExpanding(s, e);
};
ASPxClientFileManager.OnUploadControlFileUploadStartEventHandler = function(s, evt) {
    ASPxClientFileManager.GetFileManagerByInnerControl(s.name, FileManagerConsts.UploadPostfix, !s.IsInputsVisible()).OnFileUploadStart(evt);
};
ASPxClientFileManager.OnUploadControlUploadingProgressChanged = function(s, evt) {
    ASPxClientFileManager.GetFileManagerByInnerControl(s.name, FileManagerConsts.UploadPostfix, !s.IsInputsVisible()).OnUploadingProgressChanged(evt);
};
ASPxClientFileManager.OnUploadControlFilesUploadComplete = function(s, evt) {
    ASPxClientFileManager.GetFileManagerByInnerControl(s.name, FileManagerConsts.UploadPostfix, !s.IsInputsVisible()).OnFilesUploadComplete(evt);
};
ASPxClientFileManager.OnUploadControlTextChanged = function(s, evt) {
    ASPxClientFileManager.GetFileManagerByInnerControl(s.name, FileManagerConsts.UploadPostfix, !s.IsInputsVisible()).OnUploadTextChanged(s.GetText());
};
ASPxClientFileManager.OnToolbarMenuItemClick = function(s, evt) {
    ASPxClientFileManager.GetFileManagerByInnerControl(s.name, FileManagerConsts.ToolbarPostfix).OnToolbarItemClick(evt.item.name);
};
ASPxClientFileManager.OnContextMenuItemClick = function(s, evt) {
    ASPxClientFileManager.GetFileManagerByInnerControl(s.name, FileManagerConsts.ContextMenuPostfix, true).OnContextMenuItemClick(evt.item.name);
};
ASPxClientFileManager.OnFolderBrowserPopupShown = function(s, evt) {
    ASPxClientFileManager.GetFileManagerByInnerControl(s.name, FileManagerConsts.FolderBrowserPopupPostfix, true).OnFolderBrowserDialogShown();
};
ASPxClientFileManager.OnFolderBrowserPopupClosing = function(s, evt) {
    ASPxClientFileManager.GetFileManagerByInnerControl(s.name, FileManagerConsts.FolderBrowserPopupPostfix, true).OnFolderBrowserDialogClosing();
};
ASPxClientFileManager.OnPaneResizeCompleted = function(s, evt) {
    ASPxClientFileManager.GetFileManagerByInnerControl(s.name, FileManagerConsts.SplitterPostfix, true).OnSplitterPaneResizeCompleted();
};
ASPxClientFileManager.GetFileManagerByInnerControl = function(controlName, controlPostfix, ignoreSplitter) {
    var controlIdPostfix = ignoreSplitter ? controlPostfix : FileManagerConsts.SplitterPostfix + controlPostfix;
    var fileManagerId = controlName.substr(0, controlName.length - controlIdPostfix.length);
    return ASPx.GetControlCollection().Get(fileManagerId);
};
ASPxClientFileManager.PrepareTemplate = function(template, values) {
    var result = template;
    for(var key in values)
        result = result.replace(new RegExp("{{" + key + "}}", 'g'), values[key]);
    return result;
};
var ASPxClientFileManagerFileEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(file) {
        this.constructor.prototype.constructor.call(this);
        this.file = file;
    }
});
var ASPxClientFileManagerFileOpenedEventArgs = ASPx.CreateClass(ASPxClientFileManagerFileEventArgs, {
    constructor: function(file) {
        this.constructor.prototype.constructor.call(this, file);
        this.processOnServer = false;
    }
});
var ASPxClientFileManagerActionEventArgsBase = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(fullName, name, isFolder) {
        this.constructor.prototype.constructor.call(this);
        this.fullName = fullName;
        this.name = name;
        this.isFolder = !!isFolder;
    }
});
var ASPxClientFileManagerItemEditingEventArgs = ASPx.CreateClass(ASPxClientFileManagerActionEventArgsBase, {
    constructor: function(fullName, name, isFolder) {
        this.constructor.prototype.constructor.call(this, fullName, name, isFolder);
        this.cancel = false;
    }
});
var ASPxClientFileManagerItemRenamedEventArgs = ASPx.CreateClass(ASPxClientFileManagerActionEventArgsBase, {
    constructor: function(fullName, name, oldName, isFolder) {
        this.constructor.prototype.constructor.call(this, fullName, name, isFolder);
        this.oldName = oldName;
    }
});
var ASPxClientFileManagerItemDeletedEventArgs = ASPx.CreateClass(ASPxClientFileManagerActionEventArgsBase, {
    constructor: function(fullName, name, isFolder) {
        this.constructor.prototype.constructor.call(this, fullName, name, isFolder);
    }
});
var ASPxClientFileManagerItemsDeletedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(items) {
        this.constructor.prototype.constructor.call(this);
        this.items = items;
    }
});
var ASPxClientFileManagerItemMovedEventArgs = ASPx.CreateClass(ASPxClientFileManagerActionEventArgsBase, {
    constructor: function(fullName, name, oldFolderFullName, isFolder) {
        this.constructor.prototype.constructor.call(this, fullName, name, isFolder);
        this.oldFolderFullName = oldFolderFullName;
    }
});
var ASPxClientFileManagerItemsMovedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(items, oldFolderFullName) {
        this.constructor.prototype.constructor.call(this);
        this.items = items;
        this.oldFolderFullName = oldFolderFullName;
    }
});
var ASPxClientFileManagerItemCopiedEventArgs = ASPx.CreateClass(ASPxClientFileManagerActionEventArgsBase, {
    constructor: function(fullName, name, oldFolderFullName, isFolder) {
        this.constructor.prototype.constructor.call(this, fullName, name, isFolder);
        this.oldFolderFullName = oldFolderFullName;
    }
});
var ASPxClientFileManagerItemsCopiedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(items, oldFolderFullName) {
        this.constructor.prototype.constructor.call(this);
        this.items = items;
        this.oldFolderFullName = oldFolderFullName;
    }
});
var ASPxClientFileManagerItemCreatedEventArgs = ASPx.CreateClass(ASPxClientFileManagerActionEventArgsBase, {
    constructor: function(fullName, name, isFolder) {
        this.constructor.prototype.constructor.call(this, fullName, name, isFolder);
    }
});
var ASPxClientFileManagerErrorEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(commandName, errorText, errorCode) {
        this.constructor.prototype.constructor.call(this);
        this.commandName = commandName;
        this.errorText = errorText;
        this.showAlert = true;
        this.errorCode = errorCode;
    }
});
var ASPxClientFileManagerErrorAlertDisplayingEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(commandName, errorText, errorCode) {
        this.constructor.prototype.constructor.call(this);
        this.commandName = commandName;
        this.errorText = errorText;
        this.showAlert = true;
    }
});
var ASPxClientFileManagerFileUploadingEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(folder, fileName) {
        this.constructor.prototype.constructor.call(this);
        this.folder = folder;
        this.fileName = fileName;
        this.cancel = false;
    }
});
var ASPxClientFileManagerFilesUploadingEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(folder, fileNames) {
        this.constructor.prototype.constructor.call(this);
        this.folder = folder;
        this.fileNames = fileNames;
        this.cancel = false;
    }
});
var ASPxClientFileManagerFileUploadedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(folder, fileName) {
        this.constructor.prototype.constructor.call(this);
        this.folder = folder;
        this.fileName = fileName;
    }
});
var ASPxClientFileManagerFilesUploadedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(folder, fileNames) {
        this.constructor.prototype.constructor.call(this);
        this.folder = folder;
        this.fileNames = fileNames;
    }
});
var ASPxClientFileManagerFileDownloadingEventArgs = ASPx.CreateClass(ASPxClientFileManagerFileEventArgs, {
    constructor: function(file) {
        this.constructor.prototype.constructor.call(this, file);
        this.cancel = false;
    }
});
var ASPxClientFileManagerFocusedItemChangedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(item, name, fullName) {
        this.constructor.prototype.constructor.call(this);
        this.item = item;
        this.name = name;
        this.fullName = fullName;
    }
});
var ASPxClientFileManagerCurrentFolderChangedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(name, fullName) {
        this.constructor.prototype.constructor.call(this);
        this.name = name;
        this.fullName = fullName;
    }
});
var ASPxClientFileManagerSelectionChangedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(item, name, fullName, isSelected) {
        this.constructor.prototype.constructor.call(this);
        this.item = item;
        this.name = name;
        this.fullName = fullName;
        this.isSelected = isSelected;
    }
});
var ASPxClientFileManagerCustomCommandEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(commandName) {
        this.constructor.prototype.constructor.call(this);
        this.commandName = commandName;
    }
});
var ASPxClientFileManagerToolbarUpdatingEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(activeAreaName) {
        this.constructor.prototype.constructor.call(this);
        this.activeAreaName = activeAreaName;
    }
});
var ASPxClientFileManagerHighlightItemTemplateEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(filterValue, itemName, templateElement, highlightCssClassName) {
        this.constructor.prototype.constructor.call(this);
        this.filterValue = filterValue;
        this.itemName = itemName;
        this.templateElement = templateElement;
        this.highlightCssClassName = highlightCssClassName;
    }
});

// Styles helper
ASPxClientFileManager.StylesHelper = {};
ASPxClientFileManager.StylesHelper.Styles = ["Border", "Padding", "Margin"];
ASPxClientFileManager.StylesHelper.Kinds = ["Left", "Right", "Top", "Bottom"];
ASPxClientFileManager.StylesHelper.GetStylesInfo = function(width, height, elementsContainer, elementsStyles) {
    this.elements = [];

    var currentStyles = [];
    for(var i = 0; i < elementsStyles.length; i++)
        currentStyles.push(this.GetCurrentStyle(elementsContainer, elementsStyles[i]));

    var result = this.GetStylesInfoCore(width, height, currentStyles);

    this.DropCurrentStyles(elementsContainer);

    for(var i = 0; i < elementsStyles.length; i++)
        result.styleStrings[i] += this.GetUnprocessedStylesString(elementsStyles[i][1]);

    return result;
};
ASPxClientFileManager.StylesHelper.GetCurrentStyle = function(elementsContainer, styles) {
    var element = ASPx.CreateHtmlElementFromString("<div class=\"" + styles[0] + "\" style=\"visibility:hidden;" + styles[1] + "\"></div>");
    elementsContainer.appendChild(element);
    this.elements.push(element);
    return ASPx.GetCurrentStyle(element);
};
ASPxClientFileManager.StylesHelper.DropCurrentStyles = function(elementsContainer) {
    // If remove elements early - current styles will dissapear
    for(var i = 0; i < this.elements.length; i++)
        elementsContainer.removeChild(this.elements[i]);
};
ASPxClientFileManager.StylesHelper.GetUnprocessedStylesString = function(stylesString) {
    var processingStyles = [];
    for(var i = 0; i < this.Styles.length; i++)
        processingStyles.push(this.Styles[i].toLowerCase());

    var result = [];
    var stylesToSkip = ["margin", "padding", "border-width"];
    var styleParts = stylesString.split(';');
    for(var i = 0; i < styleParts.length; i++) {
        var stylePart = styleParts[i];
        var skippFlag = false;
        for(var j = 0; j < stylesToSkip.length; j++) {
            if(stylePart.toLowerCase().indexOf(stylesToSkip[j]) == 0) {
                skippFlag = true;
                break;
            }
        }
        if(!skippFlag)
            result.push(stylePart);
    }
    return result.join(';');
};
ASPxClientFileManager.StylesHelper.GetStylesInfoCore = function(width, height, currentStyles) {
    var stylesTables = this.GetStylesTables(currentStyles);

    var styleStrings = [];
    for(var i = 0; i < stylesTables.length; i++)
        styleStrings.push(this.StylesTableToString(stylesTables[i]));

    var itemStylesTable = stylesTables[0];
    return {
        contentWidth: width - (itemStylesTable.Border.Left + itemStylesTable.Border.Right + itemStylesTable.Padding.Left + itemStylesTable.Padding.Right),
        contentHeight: height - (itemStylesTable.Border.Top + itemStylesTable.Border.Bottom + itemStylesTable.Padding.Top + itemStylesTable.Padding.Bottom),
        styleStrings: styleStrings
    };

};
ASPxClientFileManager.StylesHelper.GetStylesTables = function(currentStyles) {
    var itemStylesTable = this.GetStylesTable(currentStyles[0]);
    var result = [itemStylesTable];

    for(var i = 1; i < currentStyles.length; i++) {
        result.push(this.GetStylesTable(currentStyles[i]));
        for(var j = 0; j < this.Kinds.length; j++)
            this.UpdateItemStylesTable(itemStylesTable, result[i], this.Kinds[j]);
    }
    for(var i = 1; i < result.length; i++) {
        for(var j = 0; j < this.Kinds.length; j++)
            this.UpdateStateStylesTable(itemStylesTable, result[i], this.Kinds[j]);
    }

    return result;
};
ASPxClientFileManager.StylesHelper.UpdateItemStylesTable = function(baseStylesTable, stylesTable, kind) {
    baseStylesTable.Margin[kind] += Math.max(                                                   // Increase Old Margin
        stylesTable.Border[kind] + stylesTable.Padding[kind] - baseStylesTable[kind + "Sum"],   // If the New Padding+Border > Old Padding+Border+Margin
        0
    );
};
ASPxClientFileManager.StylesHelper.UpdateStateStylesTable = function(baseStylesTable, stylesTable, kind) {
    var borderChange = stylesTable.Border[kind] - baseStylesTable.Border[kind];
    var paddingChange = stylesTable.Padding[kind] - baseStylesTable.Padding[kind];
    if(stylesTable.Padding[kind] != 0)
        stylesTable.Margin[kind] = baseStylesTable.Margin[kind] - (paddingChange + borderChange);
    else {
        stylesTable.Padding[kind] = baseStylesTable.Padding[kind] - borderChange;
        stylesTable.Margin[kind] = baseStylesTable.Margin[kind];

        if(stylesTable.Padding[kind] < 0) {
            stylesTable.Margin[kind] += stylesTable.Padding[kind];
            stylesTable.Padding[kind] = 0;
        }
    }
};
ASPxClientFileManager.StylesHelper.GetBorder = function(currentStyle, borderKind) {
    var borderStyleName = "border" + borderKind + "Style";
    var borderWidthName = "border" + borderKind + "Width";

    if(currentStyle[borderStyleName] != "none")
        return ASPx.PxToInt(currentStyle[borderWidthName]);
    return 0;
};
ASPxClientFileManager.StylesHelper.GetPadding = function(currentStyle, paddingKind) {
    return ASPx.PxToInt(currentStyle["padding" + paddingKind]);
};
ASPxClientFileManager.StylesHelper.GetMargin = function(currentStyle, marginKind) {
    return ASPx.PxToInt(currentStyle["margin" + marginKind]);
};
ASPxClientFileManager.StylesHelper.BorderToString = function(kind, border) {
    return "border-" + kind.toLowerCase() + "-width:" + border + "px;";
};
ASPxClientFileManager.StylesHelper.PaddingToString = function(kind, padding) {
    return "padding-" + kind.toLowerCase() + ":" + padding + "px;";
};
ASPxClientFileManager.StylesHelper.MarginToString = function(kind, margin) {
    return "margin-" + kind.toLowerCase() + ":" + margin + "px;";
};
ASPxClientFileManager.StylesHelper.GetStylesTable = function(currentStyle) {
    var result = {};
    for(var i = 0; i < this.Styles.length; i++)
        result[this.Styles[i]] = {};
    for(var i = 0; i < this.Kinds.length; i++)
        result[this.Kinds[i] + "Sum"] = 0;

    for(var i = 0; i < this.Styles.length; i++) {
        var style = this.Styles[i];
        var getStyle = this["Get" + style];
        for(var j = 0; j < this.Kinds.length; j++) {
            var kind = this.Kinds[j];
            result[style][kind] = getStyle(currentStyle, kind);
            result[kind + "Sum"] += result[style][kind];
        }
    }

    return result;
};
ASPxClientFileManager.StylesHelper.StylesTableToString = function(stylesTable) {
    var styleString = "";
    for(var i = 0; i < this.Styles.length; i++) {
        var style = this.Styles[i];
        var toStringFunc = this[style + "ToString"]
        for(var j = 0; j < this.Kinds.length; j++) {
            var kind = this.Kinds[j];
            styleString += toStringFunc(kind, stylesTable[style][kind]);
        }
    }
    return styleString;
};

// Filter helper
ASPxClientFileManager.FilterHelper = ASPx.CreateClass(null, {
    constructor: function(fileManager) {
        this.fileManager = fileManager;
        this.input = this.fileManager.elements.filterElement;
        this.lastValue = "";
        var _this = this;
        this.delayedFilter = null;
        ASPx.Evt.AttachEventToElement(this.input, "keyup", function() { _this.OnInputValueChanged(); });
    },
    OnInputValueChanged: function() {
        var _this = this;
        var value = this.input.value;
        if(this.filterTimerID > -1)
            ASPx.Timer.ClearTimer(this.filterTimerID);
        if(this.delayedFilter !== null)
            this.delayedFilter = value;
        this.filterTimerID = window.setTimeout(function() {
            _this.Filter(value);
        }, this.fileManager.filterDelay);
    },
    Filter: function(value) {
        if(this.delayedFilter !== null)
            return;
        if(this.filterTimerID)
            ASPx.Timer.ClearTimer(this.filterTimerID);

        if(this.lastValue != value) {
            this.lastValue = value;
            this.fileManager.UpdateStateField(FileManagerConsts.StateField.ItemFilter, value);

            if(this.fileManager.isThumbnailViewMode() && !this.fileManager.isVirtScrollEnabled()) {
                this.fileManager.forEachItem(function(item) {
                    if(item.isParentFolderItem|| item.isCreateHelperFolder)
                        return;
                    var isSatisfy = item.name.toLowerCase().indexOf(value.toLowerCase()) != -1;
                    item.SetVisible(isSatisfy);
                    this.HighlightItem(item, value);
                }.aspxBind(this));
                this.fileManager.resetSelection();
            }
            else {
                this.delayedFilter = value;
                if(this.fileManager.isThumbnailViewMode())
                    this.fileManager.performVirtualScrolling(0, true, 0);
                else {
                    var gridFilter = "[Name] = '" + FileManagerConsts.CreateHelperItemName + "' Or [Name] = '" + FileManagerConsts.ParentFolderItemName + "' Or [Name] LIKE '%" + value + "%'";
                    this.fileManager.GetFilesGridView().ApplyFilter(gridFilter);
                }
            }
        }
    },
    applyDelayedFilter: function() {
        this.HighlightItems();
        var delayedFilter = this.delayedFilter;
        this.delayedFilter = null;

        if(this.lastValue !== delayedFilter)
            this.Filter(delayedFilter);
    },
    HighlightItems: function() {
        this.fileManager.forEachItem(function(item) {
            this.HighlightItem(item, this.lastValue);
        }.aspxBind(this));
    },
    HighlightItem: function(item, value) {
        if(!item.GetVisible() || item.isParentFolderItem)
            return;
        if(this.fileManager.viewMode == FileManagerConsts.ViewMode.Thumbnail && this.fileManager.isThumbnailsViewFileAreaItemTemplate) {
            this.fileManager.raiseHighlightItemTemplate(this.GetCurrentValue(), item.name, item.itemContentElement, this.fileManager.styles.highlightCssClass);
            return;
        }
        if(this.fileManager.viewMode == FileManagerConsts.ViewMode.Grid && this.fileManager.nameColumnHasTemplate) {
            var templateContainer = ASPx.GetChildByClassName(item.getElement(), FileManagerConsts.GridColumnTitleCellClassName);
            if(!templateContainer)
                return;
            this.fileManager.raiseHighlightItemTemplate(this.GetCurrentValue(), item.name, templateContainer, this.fileManager.styles.highlightCssClass);
            return;
        }

        var titleElement = this.fileManager.elements.GetItemTitleElement(item);
        if(!titleElement)
            return;
        if(value.length > 0) {
            var name = item.name;
            var startIndex = name.toLowerCase().indexOf(value.toLowerCase());
        
            titleElement.innerHTML = ASPxClientFileManager.PrepareTemplate(
                FileManagerConsts.Templates.HighlightedText,
                {
                    textStart: name.substr(0, startIndex),
                    textMiddle: name.substr(startIndex, value.length),
                    textEnd: name.substr(startIndex + value.length),
                    highlightCssClass: this.fileManager.styles.highlightCssClass
                }
            );
        }
        else
            titleElement.innerHTML = item.name;
    },
    resetState: function() {
        this.lastValue = "";
    },
    GetCurrentValue: function() {
        return this.input.value;
    },
    UpdateEnabled: function(enabled, resetFilter) {
        if(this.delayedFilter === null)
            this.input.value = "";
        this.input.disabled = !enabled;
        if(enabled && resetFilter)
            this.Filter("");
    },
    SetFilterValue: function(value, reapplyFilter) {
        if(this.delayedFilter === null)
            this.input.value = value;
        if(reapplyFilter)
            this.Filter(value);
    }
});

// Elements helper
ASPxClientFileManager.ElementsHelper = ASPx.CreateClass(null, {
    constructor: function(fileManager) {
        this.fileManager = fileManager;

        // Public access
        this.filterElementId = this.fileManager.name + FileManagerConsts.SplitterPostfix + "_Filter";
        this.filterElement = document.getElementById(this.filterElementId);
        this.itemsContainerId = this.fileManager.name + FileManagerConsts.SplitterPostfix + FileManagerConsts.ItemsPostfix;
        this.focusId = this.fileManager.name + "_Focus";

        // Private access (Get methods should be created)
        this._sbPane = this.GetSplitterPane("UploadPanelPane");
        if(this._sbPane)
            this._sbPaneElement = this._sbPane.helper.GetContentContainerElement();
        this._bcPane = this.GetSplitterPane("BreadCrumbsPane");
        if(this._bcPane)
            this._bcPaneElement = this._bcPane.helper.GetContentContainerElement();
        this._itemsPaneElement = this.GetSplitterPane("ItemsPane").helper.GetContentContainerElement();
        this._tbPane = this.GetSplitterPane("ToolbarPane");
        if(this._tbPane)
            this._tbPaneElement = this._tbPane.helper.GetContentContainerElement();
        this._itemsContainer = null;

        var foldersPane = this.GetSplitterPane("FoldersPane");
        if(foldersPane)
            this._foldersContainer = foldersPane.helper.GetContentContainerElement();
        this._renameInput = document.getElementById(this.fileManager.name + FileManagerConsts.RenameFileInputPostfix);
        this.emptyPageElementsInfo = [];
    },
    GetSplitterPane: function(name) {
        return this.fileManager.GetSplitter().GetPaneByName(name)
    },
    GetItemsContainer: function() {
        if(this.fileManager.viewMode == FileManagerConsts.ViewMode.Grid)
            return null;
        if(!ASPx.IsValidElement(this._itemsContainer))
            this._itemsContainer = document.getElementById(this.itemsContainerId);
        if(!this._itemsContainer) {
            this._itemsContainer = ASPx.CreateHtmlElementFromString("<div id=\"" + this.itemsContainerId + "\"></div>");
            this._itemsPaneElement.appendChild(this._itemsContainer);
        }
        if(!this.itemsContainerPrepared)
            this.PrepareItemsContainer();
        return this._itemsContainer;
    },
    PrepareItemsContainer: function() {
        this._itemsContainer.className = FileManagerConsts.FileContainerClassName;
        if(this.fileManager.allowMultiSelect)
            ASPx.AddClassNameToElement(this._itemsContainer, FileManagerConsts.MultiSelectClassName);
        this.itemsContainerPrepared = true;
    },
    GetItemsPaneContainer: function() {
        return this._itemsPaneElement;
    },
    DropItemsContainer: function() {
        var filesContainer = this.GetItemsContainer();
        if(filesContainer)
            filesContainer.parentNode.removeChild(this._itemsContainer);
        this._itemsContainer = null;
        this.itemsContainerPrepared = false;
        this.emptyPageElementsInfo = [];
    },
    GetUploadPanelContainer: function() {
        return this._sbPaneElement;
    },
    GetUploadPanelPane: function() {
        return this._sbPane;
    },
    GetBreadCrumbsPaneContainer: function() {
        return this._bcPaneElement;
    },
    GetToolBarContainer: function() {
        return this._tbPaneElement;
    },
    GetFoldersContainer: function() {
        return this._foldersContainer;
    },
    GetFocusInput: function() {
        if(!this._focusInput)
            this._focusInput = this.PrepareFocusInput(this.GetItemsPaneContainer(), this.focusId);
        return this._focusInput;
    },
    GetAccessabilityInput: function() {
        if(!this._accessabilityInput)
            this._accessabilityInput = this.PrepareFocusInput(this.GetItemsPaneContainer(), "");
        return this._accessabilityInput;
    },
    PrepareFocusInput: function(parent, id) {
        var input = ASPx.CreateHtmlElementFromString("<input type=\"text\" id=\"" + id + "\"></div>");
        ASPx.SetStyles(input, {
            left: ASPx.AbsoluteLeftPosition,
            top: ASPx.InvalidPosition,
            position: "absolute"
        });
        parent.appendChild(input);
        return input;
    },

    // Item Operations
    ensurePageIndentsCreated: function() {
        if(this.pageIndentsCreated) return;

        var indentHtml = "<div style='height: 0px; padding: 0; margin: 0; border: 0px none; clear: both;'></div>";
        this.pageTopIndentElement = ASPx.CreateHtmlElementFromString(indentHtml);
        this.pageBottomIndentElement = ASPx.CreateHtmlElementFromString(indentHtml);
        var itemsContainer = this.GetItemsContainer();
        if(this.fileManager.isThumbnailsViewFileAreaItemTemplate)
            itemsContainer.insertBefore(this.pageTopIndentElement, itemsContainer.firstChild);
        else
            itemsContainer.appendChild(this.pageTopIndentElement);
        itemsContainer.appendChild(this.pageBottomIndentElement);
        this.pageIndentsCreated = true;
    },
    dropPageIndents: function() {
        this.pageTopIndentElement = null;
        this.pageBottomIndentElement = null;
        this.pageIndentsCreated = false;
    },
    addItemElementToItemsArea: function(element) {
        var itemsContainer = this.GetItemsContainer();
        if(this.fileManager.isVirtScrollEnabled())
            itemsContainer.insertBefore(element, this.pageBottomIndentElement);
        else
            itemsContainer.appendChild(element);
    },
    createItemElement: function(item) {
        var wrap = ASPx.CreateHtmlElementFromString("<div></div>");
        var fileImage = item.type == "File" ? this.fileManager.noThumbnailImage : this.fileManager.folderThumbnailImageRender;
        if(item.imageIndex > -1 && this.fileManager.customThumbnails[item.imageIndex]) {
            wrap.innerHTML = this.fileManager.customThumbnails[item.imageIndex];
            var img = ASPx.GetNodeByTagName(wrap, "IMG", 0);
            if(img) {
                ASPx.ImageUtils.SetImageSrc(img, ASPx.ImageUtils.GetImageSrc(img));
                ASPx.ImageUtils.SetSize(img, this.fileManager.styles.thumbnailWidth, this.fileManager.styles.thumbnailHeight);
            }
            fileImage = wrap.innerHTML;
        }
        else if(item.imageSrc) {
            var img = document.createElement("img");
            ASPx.ImageUtils.SetImageSrc(img, item.imageSrc);
            ASPx.ImageUtils.SetSize(img, this.fileManager.styles.thumbnailWidth, this.fileManager.styles.thumbnailHeight);
            wrap.appendChild(img);
            fileImage = wrap.innerHTML;
        }
        var element = ASPx.CreateHtmlElementFromString(
            ASPxClientFileManager.PrepareTemplate(
                FileManagerConsts.Templates.Item,
                {
                    itemId: item.elementID,
                    fileImage: fileImage,
                    fileName: this.fileManager.isThumbnailsViewFileAreaItemTemplate ? "" : item.name.replace(/&/g, "&amp;")
                }
            )
        );
        if(item.isCreateHelperFolder)
            ASPx.AddClassNameToElement(element, FileManagerConsts.CreateHelperElementClassName);
        
        this.addItemElementToItemsArea(element);
        return element;
    },
    createItemCheckBoxInstance: function(item) {
        var checkboxInput = document.getElementById(item.elementID + FileManagerConsts.CheckboxPostfix);
        if(!checkboxInput)
            return;
        if(!this.fileManager.internalCheckBoxCollection)
            this.fileManager.internalCheckBoxCollection = new ASPx.CheckBoxInternalCollection(this.fileManager.checkBoxImageProperties);
        var internalCheckBox = this.fileManager.internalCheckBoxCollection.Add(checkboxInput.id, checkboxInput);
        internalCheckBox.CreateFocusDecoration(this.fileManager.icbFocusedStyle);
        internalCheckBox.CheckedChanged.AddHandler(function(s, e) { item.SetSelected(s.GetChecked()); }.aspxBind(item));
        item.checkBox = internalCheckBox;
    },
    getCheckBoxByEvt: function(evt) {
        return ASPx.GetParentByPartialClassName(ASPx.Evt.GetEventSource(evt), "dxichSys");
    },
    PrepareItemElement: function(item, tooltip) {
        ASPx.Attr.SetAttribute(item.element, "title", tooltip.replace(/&/g, "&amp;"));
        ASPx.AddClassNameToElement(item.element, item.cssClass);
        ASPx.AddClassNameToElement(item.itemContentElement, item.contentCssClass);
    },
    createDetailsCreateHelperItemElement: function(item) {
        if(ASPx.IsExists(item.element))
            ASPx.RemoveElement(item.element);
        var grid = this.fileManager.GetFilesGridView();
        var tbody = ASPx.GetChildByTagName(grid.GetTableHelper().GetHeaderTable(), "TBODY");
        var element = ASPx.GetChildByTagName(tbody, "TR").cloneNode(true);
        ASPx.GetChildByTagName(grid.GetMainTable(), "TBODY").appendChild(element);
        item.element = element;
        this.fileManager.HideCreateHelperElement();
        setTimeout(function() { this.prepareDetailsCreateHelperElement(element); }.aspxBind(this), 0);
    },
    prepareDetailsCreateHelperElement: function(element) {
        ASPx.AddClassNameToElement(element, this.fileManager.styles.filesGridViewDataRowCssClass);
        ASPx.AddClassNameToElement(element, FileManagerConsts.CreateHelperElementClassName);
        var grid = this.fileManager.GetFilesGridView();
        var cells = ASPx.GetChildNodesByTagName(element, "TD");
        var hMatrix = grid.GetHeaderMatrix();
        hMatrix.EnsureMatrix();

        for(var i = 0, cell; cell = cells[i]; i++) {
            ASPx.AddClassNameToElement(cell, "dxgv");
            var columnIndex = hMatrix.matrix[0][i];
            if(grid.GetColumnByField("Name") && columnIndex == grid.GetColumnByField("Name").index)
                ASPx.AddClassNameToElement(cell, FileManagerConsts.GridColumnTitleCellClassName);
            if(grid.GetColumnByField("ThumbnailUrl") && columnIndex == grid.GetColumnByField("ThumbnailUrl").index) {
                var thumbnail = ASPx.CreateHtmlElementFromString(this.fileManager.folderThumbnailImageRender);
                cell.appendChild(thumbnail);
                ASPx.AddClassNameToElement(cell, FileManagerConsts.ThumbnailCellClassName);
            }
        }
    },
    GetItemElement: function(item) {
        if(this.fileManager.viewMode == FileManagerConsts.ViewMode.Thumbnail)
            return document.getElementById(item.elementID);
        else {
            var grid = this.fileManager.GetFilesGridView();
            var index = grid.GetRowIndexByKey(item.id);
            return grid.GetItem(index);
        }
    },
    GetItemTitleElement: function(item) {
        var fileElement = item.getElement();
        if(this.fileManager.viewMode == FileManagerConsts.ViewMode.Thumbnail)
            return ASPx.GetNodeByTagName(fileElement.childNodes[0], "DIV", 0);
        var fileNameCell = ASPx.GetNodeByClassName(fileElement, FileManagerConsts.GridColumnTitleCellClassName);
        return fileNameCell != null ? fileNameCell.firstChild : null;
    },

    // Rename input
    PrepareRenameInputElement: function(neighborElement, size, absolutePosition) {
        var input = this.GetRenameInputElement();
        var width = size ? size.width : null;
        input.style.width = "";
        if(neighborElement) {
            if(absolutePosition) {
                var parent = input.parentNode;
                input.style.position = "absolute";
                input.style.bottom = ASPx.GetAbsolutePositionY(neighborElement) - ASPx.GetAbsolutePositionY(parent) + "px";
                if(input.offsetWidth > ASPx.GetClearClientWidth(parent))
                     ASPx.SetOffsetWidth(input, ASPx.GetClearClientWidth(parent));
                input.style.left = (parent.offsetWidth - ASPx.GetHorizontalBordersWidth(parent) - width) / 2 + "px";
            } else {
                input.style.position = "";
                ASPx.SetOffsetHeight(input, neighborElement.offsetHeight);
                width = width ? width : neighborElement.offsetWidth;
            }
        }
        if(size && size.height)
            ASPx.SetOffsetHeight(input, size.height);
        if(ASPx.Browser.IE) {
            width = width ? width - 4 : width;
        	input.style.lineHeight = input.style.height;
        }
        ASPx.SetOffsetWidth(input, width);
    },
    GetRenameInputElement: function() {
        return this._renameInput;
    },
    GetRenameElementArea: function() {
        var input = this.GetRenameInputElement(this);
        if(!ASPx.GetElementDisplay(input) || !input.parentNode)
            return FileManagerConsts.SelectedArea.None;
        var parent = input.parentNode;

        if(ASPx.GetParentByPartialClassName(input, FileManagerConsts.FileClassName))
            return FileManagerConsts.SelectedArea.Files;
        if(parent.className.indexOf(FileManagerConsts.FolderContentContainerClassName) > -1)
            return FileManagerConsts.SelectedArea.Folders;
    },
    GetRenameMask: function() {
        if(!this._renameMask)
            this._renameMask = ASPx.CreateHtmlElementFromString("<div class='" + FileManagerConsts.ItemMaskClassName + "'></div>");
        return this._renameMask;
    },
    CalculateTextSize: function(text, parentNode) {
        var parentNodeSizes = { width: ASPx.GetClearClientWidth(parentNode), height: ASPx.GetClearClientHeight(parentNode) };
        var calculateSizeHelper = document.createElement("span");        
        calculateSizeHelper.innerHTML = text;
        parentNode.appendChild(calculateSizeHelper);

        var width = calculateSizeHelper.offsetWidth + FileManagerConsts.RenameInputAdditionalWidth;
        if(width < FileManagerConsts.DefaultRenameInputWidth)
            width = FileManagerConsts.DefaultRenameInputWidth;
        else if(width > parentNodeSizes.width)
            width = parentNodeSizes.width;

        var height = calculateSizeHelper.offsetHeight + 4;
        if(height > parentNodeSizes.height)
            height = parentNodeSizes.height;

        calculateSizeHelper.parentNode.removeChild(calculateSizeHelper);
        return { width: width, height: height };        
    },
    // Move elements
    GetFolderBrowserPopupFoldersContainer: function() {
        return document.getElementById(this.fileManager.name + FileManagerConsts.FolderBrowserPopupPostfix + FileManagerConsts.FolderBrowserFoldersContainerPostfix);
    },
    GetFolderBrowserDialogOkButton: function() {
        return document.getElementById(this.fileManager.name + FileManagerConsts.FolderBrowserPopupPostfix + FileManagerConsts.FolderBrowserDialogOkButtonPostfix);
    },
    GetFolderBrowserDialogCancelButton: function() {
        return document.getElementById(this.fileManager.name + FileManagerConsts.FolderBrowserPopupPostfix + FileManagerConsts.FolderBrowserDialogCancelButtonPostfix);
    },
    // Upload elements
    GetUploadPanelElement: function() {
        return ASPx.GetChildByTagName(this.GetUploadPanelContainer(), "TABLE");
    },
    GetUploadButtonElement: function() {
        return document.getElementById(this.fileManager.name + FileManagerConsts.SplitterPostfix + FileManagerConsts.UploadButtonPostfix);
    },
    GetProgressPopupCancelButtonElement: function() {
        return document.getElementById(this.fileManager.name + FileManagerConsts.UploadProgressPopupPostfix + FileManagerConsts.UploadPopupCancelButtonPostfix);
    },

    // Path elements
    GetPathInput: function() {
        var toolbar = this.fileManager.GetToolbar();
        if(!toolbar)
            return null;
        var pathItem = toolbar.GetItemByName("Path");
        if(!pathItem)
            return null;
        var indexPath = toolbar.GetItemByName("Path").indexPath;
        var pathTemplateElement = toolbar.GetItemTemplateContainer(indexPath);
        if(!pathTemplateElement)
            pathTemplateElement = toolbar.GetItemTextTemplateContainer(indexPath);
        return ASPx.GetNodeByTagName(pathTemplateElement, "INPUT", 0);
    }
});


// Keyboard
var FileManagerKbdHelper = ASPx.CreateClass(ASPx.KbdHelper, {
    HandleKeyDown: function(e) {
        var modifier = this.control.getKeyModifier(e);
        var fm = this.control;

        var keyCode_A = 65;

        switch (ASPx.Evt.GetKeyCode(e)) {
            case ASPx.Key.Left:
                this.TryMoveFocusLeft(modifier);
                return true;
            case ASPx.Key.Right:
                this.TryMoveFocusRight(modifier);
                return true;
            case ASPx.Key.Up:
                this.TryMoveFocusUp(modifier);
                return true;
            case ASPx.Key.Down:
                this.TryMoveFocusDown(modifier);
                return true;
            case ASPx.Key.Enter:
                fm.KeyProcessEnter();
                return true;
            case ASPx.Key.Esc:
                fm.KeyProcessEsc();
                return true;
            case ASPx.Key.F2:
                fm.KeyProcessF2();
                return true;
            case ASPx.Key.F6:
                fm.KeyProcessF6();
                return true;
            case ASPx.Key.Delete:
                fm.KeyProcessDelete();
                return true;
            case ASPx.Key.Tab:
                fm.KeyProcessTab();
                return false;
            case ASPx.Key.F7:
                fm.KeyProcessF7();
                return true;
            case ASPx.Key.Space:
                fm.KeyProcessSpace();
                return true;
            case ASPx.Key.Home:
                fm.KeyProcessHome();
                return true;
            case ASPx.Key.End:
                fm.KeyProcessEnd();
                return true;
            case keyCode_A:
                if(modifier == FileManagerConsts.ModifierKey.Ctrl)
                    fm.KeyProcessCtrlA();
                return true;
        }
        return false;
    },
    TryMoveFocusLeft: function(modifier) {
        this.TryMoveCore(-1, this.GetLeftRightIndex, modifier);
    },
    TryMoveFocusRight: function(modifier) {
        this.TryMoveCore(1, this.GetLeftRightIndex, modifier);
    },
    TryMoveFocusUp: function(modifier) {
        this.TryMoveCore(-1, this.GetTopLeftIndex, modifier);
    },
    TryMoveFocusDown: function(modifier) {
        this.TryMoveCore(1, this.GetTopLeftIndex, modifier);
    },
    TryMoveCore: function(direction, getNewIndex, modifier) {
        var fm = this.control;
        if(fm.GetActiveArea() != FileManagerConsts.SelectedArea.Files || this.folderDialogCommand != null)
            return;
        if(fm.viewMode == FileManagerConsts.ViewMode.Grid)
            return;

        var visibleFiles = fm.GetVisibleItems();
        if(visibleFiles.length == 0)
            return;

        var focusFile = (this.control.allowMultiSelect ? this.control.focusedItem : this.control.GetSelectedItem()) || visibleFiles[0];
        var newIndex = this.Bound(getNewIndex.call(this, focusFile, direction), 0, visibleFiles.length - 1);
        var file = visibleFiles[newIndex];
        if(!file)
            file = direction == -1 ? visibleFiles[0] : visibleFiles[visibleFiles.length - 1];

        if(this.control.allowMultiSelect) {
            file.focus(false, true);
            if(modifier == FileManagerConsts.ModifierKey.Ctrl) {
                if(focusFile.IsSelected() && !file.IsSelected())
                    file.Select();
                else if(!focusFile.IsSelected())
                    focusFile.Select();
                else if(!(focusFile.IsSelected() && this.control.getSelectedItems().length == 1))
                    focusFile.Unselect();
            }
            else if(modifier == FileManagerConsts.ModifierKey.Shift) {
                if(file == focusFile)
                    return;
                var select = !(file.IsSelected() && focusFile.IsSelected());
                for(var i = focusFile.index; direction > 0 ? i <= file.index : i >= file.index; direction > 0 ? i++ : i--) {
                    var nextFile = visibleFiles[i];
                    if(select)
                        nextFile.Select();
                    else if(nextFile != file)
                        nextFile.Unselect();
                }
            }
        }
        else {
            file.Select();
            fm.CorrectScroll(file);
        }
    },
    GetLeftRightIndex: function(selectedFile, direction) {
        return selectedFile.GetVisibleIndex() + direction;
    },
    GetTopLeftIndex: function(selectedFile, direction) {
        var fm = this.control;

        var fileContainerWidth = fm.elements.GetItemsContainer().offsetWidth;
        var fileElement = selectedFile.getElement();
        var fileElementCurrentStyle = ASPx.GetCurrentStyle(fileElement);
        var fileWidth = fileElement.offsetWidth + ASPx.PxToInt(fileElementCurrentStyle.marginLeft) + ASPx.PxToInt(fileElementCurrentStyle.marginRight);

        var itemsPerLine = Math.floor(fileContainerWidth / fileWidth);

        return selectedFile.GetVisibleIndex() + itemsPerLine * direction;
    },
    Bound: function(value, min, max) {
        return Math.min(Math.max(value, min), max);
    },
    Focus: function() {
        this.control.focusMainElement();
    }
});

var FileManagerGridKbdHelper = ASPx.CreateClass(ASPx.GridViewKbdHelper, {
    HandleKeyDown: function(e) {
        if(ASPx.GridViewKbdHelper.prototype.HandleKeyDown.call(this, e))
            return true;

        var fm = this.control.fileManager;
        if(!fm)
            return false;

        var modifier = fm.getKeyModifier(e);
        var keyCode_A = 65;

        switch (ASPx.Evt.GetKeyCode(e)) {
            case ASPx.Key.Enter:
                fm.KeyProcessEnter();
                return true;
            case ASPx.Key.Esc:
                fm.KeyProcessEsc();
                return true;
            case ASPx.Key.F2:
                fm.KeyProcessF2();
                return true;
            case ASPx.Key.F6:
                fm.KeyProcessF6();
                return true;
            case ASPx.Key.Delete:
                fm.KeyProcessDelete();
                return true;
            case ASPx.Key.Tab:
                fm.KeyProcessTab();
                return false;
            case ASPx.Key.F7:
                fm.KeyProcessF7();
                return true;
            case ASPx.Key.Space:
                fm.KeyProcessSpace();
                return true;
            case keyCode_A:
                if(modifier == FileManagerConsts.ModifierKey.Ctrl)
                    fm.KeyProcessCtrlA();
                return true;
            case ASPx.Key.Home:
                fm.KeyProcessHome();
                return true;
            case ASPx.Key.End:
                fm.KeyProcessEnd();
                return true;
        }
        return false;
    }
});

// Upload control
var FileManagerUploadControl = ASPx.CreateClass(ASPxClientUploadControl, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.lastUploadedFile = "";
        this.fileManager = null;
        this.inProgress = false;

        this.TextChanged.AddHandler(this.onTextChanged.aspxBind(this));
        this.FilesUploadStart.AddHandler(this.onFileUploadStart.aspxBind(this));
    },
    initializeViewManager: function() {
        ASPxClientUploadControl.prototype.initializeViewManager.call(this);

        this.viewManager.DropZoneDropInternal.AddHandler(function() {
            this.forceUpload = true;
        }.aspxBind(this));
    },
    onFileUploadStart: function(s, e) {
        if(!this.autoStart) return;

        e.cancel = !this.fileManager.raiseFileUploading();
        if(!e.cancel)
            this.changeUploadState(true);
    },
    onTextChanged: function(s, e) {
        if(this.autoStart) return;

        if(this.forceUpload && this.fileManager.raiseFileUploading()) {
            this.changeUploadState(true);
            this.Upload();
        }
        this.forceUpload = false;
    },
    changeUploadState: function(start) {
        this.fileManager.KeepClientState();
        this.inProgress = start;
        this.lastUploadedFile = this.GetFileName();
        if(!this.fileManager.hideUploadPanel)
            this.UpdateButtonValue();
    },
    OnCompleteFileUpload: function() {
        this.inProgress = false;
        if(!this.fileManager.hideUploadPanel) {
            this.UpdateButtonValue();
            this.SetButtonEnable(true);
        }
        this.fileManager.HideLoadingElements();
        this.fileManager.needSelectItems = [];
        for(var fileName, i = 0; fileName = this.lastUploadedFile.split(", ")[i]; i++)
            this.fileManager.needSelectItems.push(this.fileManager.GetCurrentPath(true) + FileManagerConsts.PathSeparator + fileName);
    },
    SetVisible: function(visible) {
        ASPxClientUploadControl.prototype.SetVisible.call(this, visible);
        if(this.IsShowPlatformErrorElement()) {
            var pane = this.fileManager.elements.GetUploadPanelPane().helper.GetContentContainerElement();
            var uploadButton = this.fileManager.elements.GetUploadButtonElement();
            ASPx.Attr.SetAttribute(pane.style, "text-align", "center");
            ASPx.SetElementDisplay(uploadButton, false);
        }
    },
    CreateFileValidators: function() {
        var validators = ASPxClientUploadControl.prototype.CreateFileValidators.apply(this);
        validators.filePermissions = {
            value: this.fileManager.filesRules,
            errorText: this.validationSettings.notAllowedFileExtensionErrorText,
            isValid: function(fileInfo) {
                return this.fileManager.CheckEditAccessByFileName(fileInfo.fileName);
            }.aspxBind(this),
            getErrorText: function() {
                return this.errorText;
            }
        };
        return validators;
    },
    ShowMultiselectionErrorText: function(errorText) {
        this.fileManager.showError(ASPxClientFileManagerCommandConsts.Upload, errorText, "" + ASPxClientFileManagerErrorConsts.WrongExtension);
    },
    // FileManager
    OnButtonClick: function() {
        if(!this.GetButtonEnabled())
            return;
        if(!this.inProgress && !this.fileManager.raiseFileUploading())
            return;
        this.changeUploadState(!this.inProgress)
        if(!this.inProgress)
            this.Cancel();
        else if(this.UploadFileFromUser())
            this.fileManager.ShowLoadingDivAndPanel();
    },
    UpdateButtonValue: function(inProgress) {
        if(typeof (inProgress) != 'undefined')
            this.inProgress = inProgress;
        var button = this.fileManager.elements.GetUploadButtonElement();
        button.innerHTML = this.inProgress ? this.fileManager.cancelUploadText : this.fileManager.uploadText;
    },
    SetButtonEnable: function(enable) {
        var button = this.fileManager.elements.GetUploadButtonElement();
        if(!enable && button.className.indexOf(FileManagerConsts.UploadControlDisableClassName) == -1)
            button.className += " " + FileManagerConsts.UploadControlDisableClassName;
        else if(enable && button.className.indexOf(FileManagerConsts.UploadControlDisableClassName) > -1)
            button.className = button.className.replace(FileManagerConsts.UploadControlDisableClassName, "");
    },
    GetButtonEnabled: function() {
        var button = this.fileManager.elements.GetUploadButtonElement();
        return button.className.indexOf(FileManagerConsts.UploadControlDisableClassName) == -1;
    },

    GetFileNameArray: function() {
        var files = this.GetText().split(', ');
        for(var i = 0; i < files.length; i++) {
            var fn = files[i];
            files[i] = fn.substring(fn.lastIndexOf("\\") + 1);
        }
        return files;
    },

    GetFileName: function() {
        return this.GetFileNameArray().join(", ");
    }
});

// TreeView
var FileManagerTreeView = ASPx.CreateClass(ASPxClientTreeView, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.fileManager = null;
        this.callbackCount = 0;
        this.isFolderBrowserFolders = false;
    },
    CreateCallback: function(callbackString) {
        this.fileManager.SendTreeViewCallback(this, callbackString);
        this.callbackCount++;
    },
    OnCallbackFinalized: function() {
        if(this.callbackCount > 0)
            this.callbackCount--;
        if(this.callbackCount <= 0)
            this.fileManager.ClearCallbackOwner();
        ASPxClientTreeView.prototype.OnCallbackFinalized.apply(this, arguments);
    },
    InitFileManagerCallbacks: function(fm, isFolderBrowserFolders) {
        this.fileManager = fm;
        this.isFolderBrowserFolders = isFolderBrowserFolders;
        if(isFolderBrowserFolders)
            this.ExpandedChanging.AddHandler(fm.OnFolderExpanding.aspxBind(fm));
    }
});

// GridView
var FileManagerGridView = ASPx.CreateClass(ASPxClientGridView, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.fileManager = null;
    },
    CreateCallback: function(callbackString, command) {
        this.fileManager.SendGridViewCallback(this, callbackString, command);
    },
    OnCallback: function(resultObj) {
        ASPxClientGridView.prototype.OnCallback.apply(this, arguments);
    },
    OnCallbackFinalized: function() {
        this.fileManager.AdjustGridViewSize();
        ASPxClientGridView.prototype.OnCallbackFinalized.apply(this);
    },
    GetRowIndexByKey: function(key) {
        for(i = 0; i < this.keys.length; i++) {
            if(this.keys[i] == key)
                return this.visibleStartIndex + i;
        }
        return -1;
    },
    getVisibleEndIndex: function() {
        return this.visibleStartIndex + this.GetDataItemCountOnPage();
    },
    EnsureRowKeys: function() {
        ASPxClientGridView.prototype.EnsureRowKeys.apply(this);
        if(this.fileManager)
            this.fileManager.ensureGridViewItems();
    },
    ProcessTableClick: function(evt, fromCheckBox) {
	    var row = this.getItemByHtmlEvent(evt);
        if(!row)
            return;
        var rowIndex = this.getItemIndex(row.id);
        if(this.fileManager.items[this.GetRowKey(rowIndex)].isParentFolderItem)
            this.SetFocusedRowIndex(rowIndex);
        else
            ASPxClientGridView.prototype.ProcessTableClick.call(this, evt, fromCheckBox);
    },
    lockVirtualScrolling: function(lock) {
        this.GetScrollHelper().lockVirtualScrolling = lock;
    },
    scrollToPageEnd: function() {
        var scrollHelper = this.GetScrollHelper(),
            mainTable = this.GetMainTable(),
            scrollableElement = scrollHelper.GetVertScrollableControl();
        var childElements = ASPx.GetChildElementNodes(scrollableElement);
        var startHeight = childElements[0].tagName == "DIV" ? childElements[0].offsetHeight : 0;
        scrollableElement.scrollTop = startHeight + mainTable.offsetHeight - scrollableElement.offsetHeight - 1;
        this.lockVirtualScrolling(false);
    }
});

// Bread Crumbs
var ClientFileManagerBreadCrumbs = ASPx.CreateClass(null, {
    constructor: function(fileManager) {
        this.fileManager = fileManager;
        this.showFolderUpButton = this.fileManager.showBreadCrumbsFolderUpButton;
        this.mainElementID = this.fileManager.name + FileManagerConsts.BreadCrumbsPostfix;
        this.items = [];
    },

    render: function() {
        this.fileManager.elements.GetBreadCrumbsPaneContainer().innerHTML = "";
        this.mainElement = document.createElement("div");
        this.mainElement.id = this.mainElementID;
        this.mainElement.className = FileManagerConsts.BreadCrumbsContainerClassName;

        if(this.showFolderUpButton) {
            this.folderUpElement = this.createFolderUpElement();
            this.mainElement.appendChild(this.folderUpElement);
            this.mainElement.appendChild(this.createLineSeparatorElement());
        }

        this.hiddenItemsButton = this.createHiddenItemsButtonElement();
        this.mainElement.appendChild(this.hiddenItemsButton);
        this.mainElement.appendChild(this.createItemSeparatorElement());

        this.fileManager.elements.GetBreadCrumbsPaneContainer().appendChild(this.mainElement);

        this.calculateItemsSizes();
    },
    createElementBase: function(className, innerHtml) {
        var element = document.createElement("span");
        if(className)
            element.className = className;
        element.innerHTML = innerHtml;
        return element;
    },
    createElement: function(innerHtml, id, hasImage, disableHover) {
        var element = this.createElementBase(this.fileManager.styles.breadCrumbsItemCssClass, innerHtml);
        element.id = id;
        if(hasImage)
            ASPxClientUtils.GetChildByTagName(element, "IMG", 0).id = element.id + FileManagerConsts.ImagePostfix;
        if(disableHover)
            return element;

        ASPx.GetStateController().AddHoverItem(
            element.id,
            [this.fileManager.styles.breadCrumbsItemHoverCssClass],
            [""],
            null,
            hasImage ? [this.fileManager.styles.breadCrumbsUpButtonImageHoveredScriptObjects] : null,
            hasImage ? [FileManagerConsts.ImagePostfix] : null,
            null
        );
        return element;
    },
    createFolderUpElement: function() {
        var element = this.createElement(this.fileManager.breadCrumbsUpButtonImage, this.mainElementID + FileManagerConsts.BreadCrumbsFolderUpButtonPostfix, true);
        ASPx.Evt.AttachEventToElement(element, "mouseup", function() { this.onFolderUpElementClick(); }.aspxBind(this));
        ASPx.AddClassNameToElement(element, FileManagerConsts.BreadCrumbsButtonClassName);
        ASPx.GetStateController().AddDisabledItem(
            element.id,
            [""],
            [""],
            null,
            [this.fileManager.styles.breadCrumbsUpButtonImageDisabledScriptObjects],
            [FileManagerConsts.ImagePostfix],
            null
        );
        return element;
    },
    createHiddenItemsButtonElement: function() {
        var element = this.createElement(FileManagerConsts.BreadCrumbsHiddenItemsButtonText, this.mainElementID + FileManagerConsts.BreadCrumbsHiddenItemsButtonPostfix, false);
        ASPx.AddClassNameToElement(element, FileManagerConsts.BreadCrumbsButtonClassName);
        ASPx.Evt.AttachEventToElement(element, "mouseup", function() { this.onHiddenItemsButtonClick(); }.aspxBind(this));
        return element;
    },
    createItemElement: function(item) {
        var element = this.createElement(item.text, item.getElementID(), false, item.isLastItem);
        if(item.isLastItem)
            ASPx.AddClassNameToElement(element, FileManagerConsts.BreadCrumbsLastItemClassName);
        else
            ASPx.Evt.AttachEventToElement(element, "mouseup", function() { this.onItemClick(item); }.aspxBind(this));
        return element;
    },
    createItemSeparatorElement: function() {
        return this.createElementBase("", this.fileManager.breadCrumbsSeparatorImage);
    },
    createLineSeparatorElement: function() {
        return this.createElementBase(FileManagerConsts.BreadCrumbsLineSeparatorClassName, "");
    },
    createItems: function() {
        for(var item, i = 0; item = this.items[i]; i++)
            item.removeElement();
        this.items = [];
        this.getPopup().GetContentContainer(-1).innerHTML = "";

        var path = "";
        var currentPath = this.fileManager.GetCurrentPath();
        if(currentPath[currentPath.length - 1] == FileManagerConsts.PathSeparator)
            currentPath = currentPath.substring(0, currentPath.length - 1);
        var pathParts = currentPath.split(FileManagerConsts.PathSeparator);
        for(var part, i = 0; part = pathParts[i]; i++) {
            if(i > 0)
                path += (path ? FileManagerConsts.PathSeparator : "") + part;
            var isLastItem = i == pathParts.length - 1;
            var item = new ASPxClientFileManagerBreadCrumbsItem(this, i, path, part, isLastItem);
            this.items.push(item);
            if(!isLastItem)
                this.placeItemElementToPopup(item);
            else
                this.mainElement.appendChild(item.getElement());
        }
        if(this.items.length == 1)
            this.hideHiddenItemsButton();
        this.adjustItems();
    },

    onFolderUpElementClick: function() {
        if(this.upFolderPath != null)
            this.fileManager.SetCurrentFolderPath(this.upFolderPath);
    },
    onHiddenItemsButtonClick: function() {
        this.getPopup().ShowAtElement(this.hiddenItemsButton);
    },
    onItemClick: function(item) {
        this.hidePopup();
        this.fileManager.SetCurrentFolderPath(item.targetPath);            
    },

    refresh: function() {
        if(this.showFolderUpButton)
            this.updateFolderUpElementState();
        this.createItems();

    },    
    updateFolderUpElementState: function() {
        var path = this.fileManager.GetCurrentPath(true);
        this.upFolderPath = path ? path.substr(0, path.lastIndexOf(FileManagerConsts.PathSeparator)) : null;
        ASPx.GetStateController().SetElementEnabled(this.folderUpElement, !!path);
    },

    calculateItemsSizes: function() {
        var itemSeparator = this.createItemSeparatorElement();
        this.mainElement.appendChild(itemSeparator);

        var simpleItem = new ASPxClientFileManagerBreadCrumbsItem(this, 0, "_", "_");
        var element = simpleItem.getElement();
        this.mainElement.appendChild(element);

        this.itemSeparatorWidth = itemSeparator.offsetWidth;
        this.itemElementSymbolWidth = ASPx.GetClearClientWidth(element);
        this.itemElementOuterWidth = element.offsetWidth + ASPx.GetLeftRightMargins(element) - this.itemElementSymbolWidth;

        ASPx.RemoveElement(itemSeparator);
        ASPx.RemoveElement(element);
    },
    getItemWithSeparatorWidth: function(item) {
        return this.itemSeparatorWidth + this.itemElementOuterWidth + item.text.length * this.itemElementSymbolWidth;
    },
    adjustItems: function() {
        if(this.items.length == 0)
            return;
        var modificationRange = this.getModificationRange();
        if(!modificationRange)
            return;

        if(modificationRange < 0) {
            for(var i = 0; i < this.items.length - 1; i++) {
                var item = this.items[i];
                if(!item.inPopup) {
                    this.placeItemElementToPopup(item);
                    modificationRange += this.getItemWithSeparatorWidth(item);
                    if(modificationRange >= 0)
                        break;
                }
            }
            return;
        }

        for(var item, i = this.items.length - 2; item = this.items[i]; i--) {
            if(item.inPopup) {
                var itemWidth = this.getItemWithSeparatorWidth(item);
                if(itemWidth <= modificationRange) {
                    this.placeItemToMainElement(item);
                    modificationRange -= itemWidth;
                } else
                    break;
            }
        }
    },
    getModificationRange: function() {
        var lastElement = this.items[this.items.length - 1].getElement();
        return ASPx.GetAbsoluteX(this.GetMainElement()) + this.GetMainElement().offsetWidth - ASPx.GetAbsoluteX(lastElement) - lastElement.offsetWidth;
    },
    placeItemElementToPopup: function(item) {
        var itemElement = item.getElement();
        var popupContainer = this.getPopup().GetContentContainer(-1);
        if(itemElement.nextSibling)
            ASPx.RemoveElement(itemElement.nextSibling);
        popupContainer.insertBefore(itemElement, popupContainer.firstChild);
        item.inPopup = true;
        this.getPopup().SetSize(0, 0);
        if(item.index == 0)
            this.showHiddenItemsButton();
    },
    placeItemToMainElement: function(item) {
        var lastItem = this.items[this.items.length - 1];
        for(var curItem, i = lastItem.index - 1; curItem = this.items[i]; i--) {
            if(!curItem.inPopup)
                lastItem = curItem;
            else
                break;
        }
        var separator = this.createItemSeparatorElement();
        this.GetMainElement().insertBefore(separator, lastItem.getElement());
        this.GetMainElement().insertBefore(item.getElement(), separator);
        item.inPopup = false;
        if(item.index == 0)
            this.hideHiddenItemsButton();
    },

    GetMainElement: function() {
        return this.mainElement;
    },
    getPopup: function() {
        return this.fileManager.GetBreadCrumbsPopup();
    },

    hidePopup: function() {
        this.getPopup().Hide();
    },
    hideHiddenItemsButton: function() {
        ASPx.SetElementDisplay(this.hiddenItemsButton.nextSibling, false);
        ASPx.SetElementDisplay(this.hiddenItemsButton, false);
    },
    showHiddenItemsButton: function() {
        ASPx.SetElementDisplay(this.hiddenItemsButton, true);
        ASPx.SetElementDisplay(this.hiddenItemsButton.nextSibling, true);
    }
});

var ASPxClientFileManagerBreadCrumbsItem = ASPx.CreateClass(null, {
    constructor: function(breadCrumbs, index, targetPath, text, isLastItem) {
        this.breadCrumbs = breadCrumbs;
        this.index = index;
        this.targetPath = targetPath;
        this.text = text;
        this.isLastItem = isLastItem;
    },
    getElement: function() {
        if(!this.element)
            this.element = this.breadCrumbs.createItemElement(this);
        return this.element;
    },
    removeElement: function() {
        if(!this.element)
            return;
        ASPx.GetStateController().RemoveHoverItem(this.element.id, null);
        if(this.element.nextSibling)
            ASPx.RemoveElement(this.element.nextSibling);
        ASPx.RemoveElement(this.element);
    },
    getElementID: function() {
        return this.breadCrumbs.mainElementID + FileManagerConsts.ItemPostfix + this.index;
    }
});
var ASPxClientFileManagerCommandConsts = {
    Rename: "rename",
    Move: "move",
    Delete: "delete",
    Create: "create",
    Upload: "upload",
    Download: "download",
    Copy: "copy",
    Open: "open"
};
var ASPxClientFileManagerErrorConsts = {
    FileNotFound: 0,
    FolderNotFound: 1,
    AccessDenied: 2,
    UnspecifiedIO: 3,
    Unspecified: 4,
    EmptyName: 5,
    CanceledOperation: 6,
    InvalidSymbols: 7,
    WrongExtension: 8,
    UsedByAnotherProcess: 9,
    AlreadyExists: 10
};

ASPx.FileManagerConsts = FileManagerConsts;
ASPx.FileManagerGridKbdHelper = FileManagerGridKbdHelper;
ASPx.FileManagerKbdHelper = FileManagerKbdHelper;

ASPx.FileManagerTreeView = FileManagerTreeView;
ASPx.FileManagerGridView = FileManagerGridView;
ASPx.FileManagerUploadControl = FileManagerUploadControl;

window.ASPxClientFileManager = ASPxClientFileManager;
window.ASPxClientFileManagerFile = ASPxClientFileManagerFile;
window.ASPxClientFileManagerFolder = ASPxClientFileManagerFolder;

window.ASPxClientFileManagerSelectionChangedEventArgs = ASPxClientFileManagerSelectionChangedEventArgs;
window.ASPxClientFileManagerCurrentFolderChangedEventArgs = ASPxClientFileManagerCurrentFolderChangedEventArgs;
window.ASPxClientFileManagerFocusedItemChangedEventArgs = ASPxClientFileManagerFocusedItemChangedEventArgs;
window.ASPxClientFileManagerFileDownloadingEventArgs = ASPxClientFileManagerFileDownloadingEventArgs;
window.ASPxClientFileManagerFileUploadedEventArgs = ASPxClientFileManagerFileUploadedEventArgs;
window.ASPxClientFileManagerFileUploadingEventArgs = ASPxClientFileManagerFileUploadingEventArgs;
window.ASPxClientFileManagerFilesUploadingEventArgs = ASPxClientFileManagerFilesUploadingEventArgs;
window.ASPxClientFileManagerErrorAlertDisplayingEventArgs = ASPxClientFileManagerErrorAlertDisplayingEventArgs;
window.ASPxClientFileManagerErrorEventArgs = ASPxClientFileManagerErrorEventArgs;
window.ASPxClientFileManagerItemCreatedEventArgs = ASPxClientFileManagerItemCreatedEventArgs;
window.ASPxClientFileManagerItemCopiedEventArgs = ASPxClientFileManagerItemCopiedEventArgs;
window.ASPxClientFileManagerItemMovedEventArgs = ASPxClientFileManagerItemMovedEventArgs;
window.ASPxClientFileManagerItemDeletedEventArgs = ASPxClientFileManagerItemDeletedEventArgs;
window.ASPxClientFileManagerItemRenamedEventArgs = ASPxClientFileManagerItemRenamedEventArgs;
window.ASPxClientFileManagerItemEditingEventArgs = ASPxClientFileManagerItemEditingEventArgs;
window.ASPxClientFileManagerActionEventArgsBase = ASPxClientFileManagerActionEventArgsBase;
window.ASPxClientFileManagerFileOpenedEventArgs = ASPxClientFileManagerFileOpenedEventArgs;
window.ASPxClientFileManagerFileEventArgs = ASPxClientFileManagerFileEventArgs;

window.ASPxClientFileManagerCommandConsts = ASPxClientFileManagerCommandConsts;
window.ASPxClientFileManagerErrorConsts = ASPxClientFileManagerErrorConsts;
})();