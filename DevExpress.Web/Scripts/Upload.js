/// <reference path="_references.js"/>

(function() {
    var Constants = {
        QueryParamNames: {
            PROGRESS_INFO: "DXProgressInfo",
            UPLOADING_CALLBACK: "DXUploadingCallback",
            HELPER_UPLOADING_CALLBACK: "DXHelperUploadingCallback",
            CANCEL_UPLOAD: "DXFakeQueryParam",
            PROGRESS_HANDLER: "DXProgressHandlerKey"
        },
        DEFAULT_PACKET_SIZE: 200000,
        ERROR_TEXT_RESPONSE_PREFIX: "DXER:",
        INPUT_ZINDEX: 29999
    };

    var IdSuffixes = {
        Input: {
            FileInput: "_Input",
            FileInputRow: "_FI",
            FileFakeInput: "_FakeInput",
            FakeFocusInput: "_FFI",
            UploadInputsTable: "_UploadInputs",
            TextBoxCell: "_TextBox",

            AddButtonsSeparator: "_AddUploadR",
            AddUploadButtonPanelRow: "_AddUploadPanelR",

            ButtonCell: {
                Add: "_Add",
                Upload: "_Upload",
                Browse: "_Browse",
                Remove: "_Remove",
                Cancel: "_Cancel",
                Clear: "_ClearBox",
                ClearImg: "Img"
            },

            FileList: {
                List: "_FileList",
                Row: "_FileR",
                RemoveRowButton: "_RemoveRow",
                ProgressControl: "_FL_Progress"
            }
        },
        SL: {
            UploadHelper: "_SLUploadHelper",
            UploadHost: "_SLUploadHost",
            ErrorTextResponsePrefix: "DXER:"
        },
        Progress: {
            Panel: "_ProgressPanel",
            Control: "_UCProgress"
        },
        Error: {
            Row: "_ErrR",
            RowTemplate: "_ErrRRT",
            Div: "_CErr",
            PlatformErrorTable: "_PlatformErrorPanel"
        },
        Upload: {
            IFrame: "_UploadIframe"
        }
    };

    var CSSClasses = {
        ErrorTextResponsePrefix: "DXER:",
        BrowseButtonCell: "dxBB",
        ClearButtonCell: "dxCB",
        RemoveButtonCell: "dxRB",
        BrowseButtonFocus: "dxbf",
        FITextBoxHoverDocument: "_dxFITextBoxHover",
        FIButtonHoverDocument: "_dxFIButtonHover",
        SeparatorRow: "dxucSR",

        HiddenUI: "dxucHidden",

        Textbox: "dxTB",
        TextboxInput: "dxTI",
        TextboxFakeInput: "dxTF",

        FileList: {
            List: "dxucFileList",
            NameCell: "dxucNameCell",
            RemoveButtonCell: "dxRB",
            ProgressBar: "dxucFL-Progress",
            ProgressBarCell: "dxucBarCell",
            State: {
                Pending: "dxuc-pending",
                Uploading: "dxuc-uploading",
                Complete: "dxuc-complete"
            }
        },

        DropZone: {
            Inline: "dxucInlineDropZoneSys",
            InlineHidden: "dxucIZ-hidden",
            HasExternalAnchor: "hasExternalAnchor"
        }
    };

    var ASPxEmptyRefreshArgs = {
        fileInfos: [],
        forceClear: true
    };

    var ASPxUploadErrorTypes = {
        Common: 0,
        Platform: 1,
        Validation: 2,
        InputRowError: 3
    };
    var ASPxClientUploadControl = ASPx.CreateClass(ASPxClientControl, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.settingsID = "";
            this.signature = "";
            this.validationSettings = this.validationSettings || {};
            this.isInCallback = false;
            this.advancedModeEnabled = false;
            this.clientEnabled = true;

            this.fileInfosCache = new ASPxFileInfoCache();

            this.templateDisabledRemoveItem = null;

            this.dropZoneAnimationType = "Fade";

            aspxGetUploadControlCollection().Add(this);
            this.FileUploadComplete = new ASPxClientEvent();
            this.FilesUploadComplete = new ASPxClientEvent();
            this.FileUploadStart = new ASPxClientEvent();
            this.FilesUploadStart = new ASPxClientEvent();
            this.TextChanged = new ASPxClientEvent();
            this.UploadingProgressChanged = new ASPxClientEvent();
            this.FileInputCountChanged = new ASPxClientEvent();
            this.DropZoneEnter = new ASPxClientEvent();
            this.DropZoneLeave = new ASPxClientEvent();
            // find if add remove buttons exist in template
        },
        Initialize: function() {
            if(!this.GetMainElement())
                return;

            this.InitializeHandlers();
            this.AdjustMainElementWidth();
            this.initializeForm();
            this.viewManager.Initialize();
            this.initializeUploadManagerHandlers();

            this.SetEnabledInternal(this.clientEnabled, true);
        },
        InlineInitialize: function() {
            ASPxClientControl.prototype.InlineInitialize.call(this);

            this.initializeDomHelper();
            this.initializeOptions();
            this.initializeDragAndDrop();
            this.initializeFileValidator();
            this.initializeUploadManager();
            this.initializeViewManager();
        },
        initializeFileValidator: function() {
            this.fileValidator = new ASPxFileValidator(this.options);
        },
        initializeForm: function () {
            var form = this.GetParentForm();
            if(form)
                form.enctype = form.encoding = "multipart/form-data";
        },
        initializeDomHelper: function() {
            var domHelper = new ASPxDOMHelper({name: this.name, stateObject: this.stateObject});
            domHelper.GetChildElement = this.GetChildElement.aspxBind(this);
            domHelper.GetMainElement = this.GetMainElement.aspxBind(this);
            domHelper.IsDisplayed = this.IsDisplayed.aspxBind(this);
            this.domHelper = domHelper;
        },
        initializeOptions: function() {
            var options = this.options = {};
            options.advancedModeEnabled = this.advancedModeEnabled;
            options.autoModeEnabled = this.autoModeEnabled || false;
            options.autoStart = this.autoStart;

            options.maxFileCount = this.validationSettings.maxFileCount || 0;

            options.enableMultiSelect = (ASPxClientUploadControl.IsFileApiAvailable() || this.IsSLPluginAvailable()) && this.enableMultiSelect;

            options.domHelper = this.domHelper;
            options.templateDisabledRemoveItem = this.templateDisabledRemoveItem;
            options.fileInputCount = this.domHelper.GetFileInputCountInternal();
            options.showAddRemoveButtons = !!this.domHelper.GetChildElement(IdSuffixes.Input.ButtonCell.Add);
            options.multiFileInputEnabled = options.fileInputCount > 1 || options.showAddRemoveButtons;

            options.enableDragAndDrop = this.enableDragAndDrop && ASPxClientUploadControl.IsDragAndDropAvailable() && !ASPx.Browser.WebKitTouchUI && !options.multiFileInputEnabled;
            options.externalDropZoneIDList = this.externalDropZoneIDList || [];
            options.inlineDropZoneAnchorElementID = this.inlineDropZoneAnchorElementID;
            options.disableInlineDropZone = this.disableInlineDropZone;

            options.dropZoneAnimationType = this.dropZoneAnimationType;

            this.enableDragAndDrop = options.enableDragAndDrop;

            options.enableFileList = options.enableMultiSelect && this.enableFileList && !options.multiFileInputEnabled;
            options.enableProgressPanel = !!this.GetProgressPanel();

            options.IsRightToLeft = this.IsRightToLeft.aspxBind(this);
            options.name = this.name;
            options.isFileApiAvailable = ASPxClientUploadControl.IsFileApiAvailable();

            options.isSLEnabled = this.IsSLPluginAvailable();
            options.isNative = this.isNative || false;
            options.selectedSeveralFilesText = this.selectedSeveralFilesText;

            options.nullText = this.getNullText();

            options.nullTextItem = this.nullTextItem;

            options.showProgressPanel = !!this.domHelper.GetChildElement(IdSuffixes.Progress.Panel);
            options.progressHandlerPage = this.progressHandlerPage;
            options.uploadingKey = this.uploadingKey;
            options.packetSize = this.packetSize || Constants.DEFAULT_PACKET_SIZE;
            options.uploadProcessingEnabled = this.uploadProcessingEnabled || false;
            options.slUploadHelperUrl = this.slUploadHelperUrl;
            options.unspecifiedErrorText = this.unspecifiedErrorText || "";
            options.uploadWasCanceledErrorText = this.uploadWasCanceledErrorText;
            options.generalErrorText = this.generalErrorText;
            options.dragAndDropMoreThanOneFileError = this.dragAndDropMoreThanOneFileError;

            options.settingsID = this.settingsID;
            options.signature = this.signature;

            options.validationSettings = this.validationSettings;

            options.fileInputSpacing = this.fileInputSpacing || "";

            options.dialogTriggerIDList = this.dialogTriggerIDList;
            options.accessibilityCompliant = this.accessibilityCompliant;

            options.templates = {
                DisabledTextBoxItem: this.templateDisabledTextBoxItem,
                DisabledClearBoxItem: this.templateDisabledClearBoxItem,
                HoveredBrowseItem: this.templateHoveredBrowseItem,
                PressedBrowseItem: this.templatePressedBrowseItem,
                DisabledRemoveItem: this.templateDisabledRemoveItem,
                DisabledBrowseItem: this.templateDisabledBrowseItem
            }
        },
        initializeViewManager: function() {
            this.viewManager = new ASPxViewManager(this.options);

            this.viewManager.UploadCancelled.AddHandler(function() {
                this.uploadManager.CancelUploading(true);
            }.aspxBind(this));
            this.viewManager.UploadStarted.AddHandler(function() {
                this.Upload();
            }.aspxBind(this));

            this.viewManager.FileInputCountChangedInternal.AddHandler(this.raiseFileInputCountChanged.aspxBind(this));

            this.viewManager.DropZoneEnter.AddHandler(this.raiseDropZoneEnter.aspxBind(this));
            this.viewManager.DropZoneLeave.AddHandler(this.raiseDropZoneLeave.aspxBind(this));

            this.fileValidator.ValidationErrorInternal.AddHandler(this.OnErrorInternal.aspxBind(this));

            this.viewManager.InlineInitialize();
        },
        initializeUploadManager: function() {
            if(this.options.advancedModeEnabled) {
                if(this.options.isFileApiAvailable)
                    this.options.uploadHelper = new ASPxUploadHelperHTML5(this.options);
                else
                    this.options.uploadHelper = new ASPxUploadHelperSL(this.options);

                this.uploadManager = this.createUploadManager(); //ASPxUploadManagerAdvancedStrategy(this.options);
            }
            else {
                this.options.uploadHelper = new ASPxUploadHelperStandardStrategy(this.options);
                this.uploadManager = this.createUploadManager(); // ASPxUploadManagerStandardStrategy(this.options);
            }

            //TODO:
            this.uploadManager.GetParentForm = this.GetParentForm.aspxBind(this);
        },
        initializeDragAndDrop: function() {
            if(this.options.enableDragAndDrop)
                aspxGetUploadControlCollection().initializeEvents();
        },
        createUploadManager: function() {
            return new ASPxLegacyUploadManager(this.options);
        },
        initializeUploadManagerHandlers: function() {
            this.uploadManager.FileUploadCompleteInternal.AddHandler(this.OnFileUploadComplete.aspxBind(this));
            this.uploadManager.FilesUploadCompleteInternal.AddHandler(this.OnFilesUploadComplete.aspxBind(this));
            this.uploadManager.FileUploadStartInternal.AddHandler(this.OnFileUploadStart.aspxBind(this));

            this.uploadManager.UploadInitiatedInternal.AddHandler(this.viewManager.OnUploadInitiated.aspxBind(this.viewManager));
            this.uploadManager.BeginProcessUploadingInternal.AddHandler(this.viewManager.OnBeginProcessUploading.aspxBind(this.viewManager));
            this.uploadManager.UploadingProgressChangedInternal.AddHandler(this.OnUploadingProgressChangedInternal.aspxBind(this));

            this.uploadManager.InCallbackChangedInternal.AddHandler(this.OnInCallbackChanged.aspxBind(this));
            this.uploadManager.NeedSetJSProperties.AddHandler(this.setJSProperties.aspxBind(this));

            this.uploadManager.InternalError.AddHandler(this.OnErrorInternal.aspxBind(this));
            this.uploadManager.FileUploadErrorInternal.AddHandler(this.OnErrorInternal.aspxBind(this));
        },
        getNullText: function() {
            var result = null;

            if(this.nullText)
                result = this.nullText;
            else if(this.enableDragAndDrop)
                result = this.dropZoneText;

            return result;
        },
        OnInCallbackChanged: function(isInCallback) {
            this.isInCallback = isInCallback;
            this.viewManager.InCallbackChanged(isInCallback);
        },
        OnFilesUploadComplete: function(args) {
            if(!args.uploadCancelled || this.options.autoStart)
                this.viewManager.Clear();

            this.viewManager.OnUploadFilesComplete(args);

            var fileUploadCompleteArgs = new ASPxClientUploadControlFilesUploadCompleteEventArgs(args.commonErrorText, args.commonCallbackData);
            this.FilesUploadComplete.FireEvent(this, fileUploadCompleteArgs);
        },
        OnFileUploadComplete: function(args) {
            this.FileUploadComplete.FireEvent(this, args);
        },
        OnFileUploadStart: function(args) {
            if(!this.FilesUploadStart.IsEmpty())
                this.FilesUploadStart.FireEvent(this, args);
            else if(!this.FileUploadStart.IsEmpty())
                this.FileUploadStart.FireEvent(this, args);
        },
        OnUploadingProgressChangedInternal: function(args) {
            this.RaiseUploadingProgressChanged(args);
            this.viewManager.UpdateProgress(args);
        },
        RaiseUploadingProgressChanged: function(args) {
            this.UploadingProgressChanged.FireEvent(this, args);
        },
        raiseDropZoneEnter: function(args) {
            this.DropZoneEnter.FireEvent(this, args);
        },
        raiseDropZoneLeave: function(args) {
            this.DropZoneLeave.FireEvent(this, args);
        },
        GetProgressPanel: function () {
            return this.domHelper.GetChildElement(IdSuffixes.Progress.Panel);
        },
        GetProgressControl: function() {
            return this.viewManager.GetProgressControl();
        },
        InitializeHandlers: function() {
            this.viewManager.StateChanged.AddHandler(function(args) {
                this.OnViewManagerStateChange(args);
            }.aspxBind(this));

            this.fileInfosCache.FileListChanged.AddHandler(function(args) {
                this.OnFileListChanged(args);
            }.aspxBind(this))
        },
        AdjustMainElementWidth: function() {
            var element = this.GetMainElement();
            if(this.IsDisplayed() && element.style.width == "") {
                if(ASPx.Browser.IE)
                    element.style.width = ASPx.GetClearClientWidth(element);
                else
                    element.style.width = ASPx.GetCurrentStyle(element).width;
            }
        },
        initializeForm: function() {
            var form = this.GetParentForm();
            if(form)
                form.enctype = form.encoding = "multipart/form-data";
        },
        OnViewManagerStateChange: function(args) {
            args.fileInfos = this.Validate(args.fileInfos);
            this.fileInfosCache.Update(args);
        },
        Validate: function(fileInfos) {
            return this.fileValidator.validate(fileInfos);
        },
        UpdateErrorMessageCell: function(index, errorText, isValid) {
            this.viewManager.UpdateErrorMessageCell(index, errorText, isValid);
        },
        OnFileListChanged: function(args) {
            args.isStateChanged = true;
            this.viewManager.RefreshViews(args);

            if(args.fileCountChanged || !this.options.enableMultiSelect)
                this.RaiseTextChanged(args.inputIndex);

            if(this.options.autoStart)
                this.Upload();
        },
        OnPluginLoaded: function(index) {
            this.viewManager.SetFileInputRowEnabled(true, index);
        },
        OnDocumentMouseUp: function() {
            this.viewManager.OnDocumentMouseUp();
        },
        InvokeTextChangedInternal: function(index) {
            this.viewManager.InvokeTextChangedInternal(index);
        },
        OnErrorInternal: function(args) {
            this.viewManager.HandleError(args);
        },
        setJSProperties: function(JSProperties) {
            for(var property in JSProperties)
                this[property] = JSProperties[property];
        },
        GetFileInputCountInternal: function() {
            return this.domHelper.GetFileInputCountInternal();
        },
        RaiseTextChanged: function(inputIndex) {
            if(!this.TextChanged.IsEmpty()) {
                var args = new ASPxClientUploadControlTextChangedEventArgs(inputIndex);
                this.TextChanged.FireEvent(this, args);
            }
        },
        raiseFileInputCountChanged: function() {
            if(!this.FileInputCountChanged.IsEmpty()) {
                var args = new ASPxClientEventArgs();
                this.FileInputCountChanged.FireEvent(this, args);
            }
        },
        IsRightToLeft: function() {
            return ASPx.IsElementRightToLeft(this.GetMainElement());
        },
        IsSLPluginInstalled: function () {
            if(!this.isSilverlightInstalled) {
                try {
                    if(typeof (ActiveXObject) != 'undefined') {
                        var slControl = new ActiveXObject('AgControl.AgControl');
                        if(slControl != null)
                            this.isSilverlightInstalled = true;
                    }
                    else if(navigator.plugins["Silverlight Plug-In"])
                        this.isSilverlightInstalled = true;
                } catch (e) { }
            }
            return this.isSilverlightInstalled;
        },
        IsSLPluginSupported: function () {
            return !(ASPx.Browser.Safari && ASPx.Browser.MajorVersion == 5);
        },
        IsSLPluginAvailable: function () {
            return this.IsSLPluginInstalled() && this.IsSLPluginSupported();
        },
        IsShowPlatformErrorElement: function () {
            return this.advancedModeEnabled && !ASPxClientUploadControl.IsFileApiAvailable() && !this.IsSLPluginAvailable() && !this.autoModeEnabled;
        },
        IsHelperElementReady: function(index) {
            return this.options.uploadHelper.IsHelperElementReady(index);
        },
        IsAdvancedModeEnabled: function() {
            return this.options.advancedModeEnabled;
        },
        UploadFileFromUser: function() {
            this.Upload();
        },
        UploadFile: function() {
            this.Upload();
        },
        AddFileInput: function() {
            this.viewManager.AddFileInput();
        },
        RemoveFileInput: function(index) {
            this.viewManager.RemoveFileInput(index);
        },
        RemoveFileFromSelection: function(fileIndex) {
            if(this.advancedModeEnabled && !this.isInCallback)
                this.fileInfosCache.RemoveFile(fileIndex);
        },
        GetText: function(index) {
            return this.viewManager.GetText(index);
        },
        SetCustomText: function(text, index) {
            this.viewManager.setText(text, index || 0);
        },
        SetCustomTooltip: function(tooltip, index) {
            this.viewManager.setTooltip(tooltip, index || 0);
        },
        GetFileInputCount: function () {
            return this.GetFileInputCountInternal();
        },
        SetFileInputCount: function(count) {
            this.viewManager.SetFileInputCount(count);
        },
        SetEnabled: function(enabled) {
            this.SetEnabledInternal(enabled);
        },
        GetEnabled: function() {
            return this.clientEnabled;
        },
        SetEnabledInternal: function(enabled, initialization) {
            if(this.clientEnabled !== enabled || initialization) {
                this.clientEnabled = enabled;
                this.viewManager.SetEnabled(enabled);
            }
        },
        Upload: function() {
            var fileInfos = this.fileInfosCache.Get(),
                filesCount = fileInfos.length;

            if(filesCount) {
                var uploadStarted = this.uploadManager.UploadFileFromUser(fileInfos);
                if(uploadStarted)
                    this.viewManager.SetViewsEnabled(false);
            }
        },
        Cancel: function () {
            this.uploadManager.CancelUploading(true);
        },
        ClearText: function() {
            this.fileInfosCache.clear();
        },
        SetAddButtonText: function (text) {
            this.viewManager.SetAddButtonText(text);
        },
        SetUploadButtonText: function (text) {
            this.viewManager.SetUploadButtonText(text);
        },
        GetAddButtonText: function () {
            return this.viewManager.GetAddButtonText();
        },
        GetUploadButtonText: function () {
            return this.viewManager.GetUploadButtonText();
        },
        SetExternalDropZoneID: function(ids) {
            this.viewManager.SetExternalDropZoneID(ids);
        },
        SetDialogTriggerID: function(ids) {
            this.viewManager.SetDialogTriggerID(ids);
        },
        OnDragEnter: function(args) {
            this.viewManager.OnDragEnter(args);
        },
        OnDragLeave: function(args) {
            this.viewManager.OnDragLeave(args);
        },
        OnDrop: function(args) {
            this.viewManager.OnDrop(args);
        },
        AdjustControlCore: function() {
            ASPxClientControl.prototype.AdjustControlCore.call(this);

            this.viewManager.AdjustSize();
        },
        ShowTooManyFilesError: function () {
            window.alert(this.tooManyFilesErrorText);
        },
        IsInputsVisible: function() {
            return this.viewManager.isInputsVisible();
        },
        SuppressFileDialog: function(suppress) {
            this.viewManager.suppressFileDialog(suppress);
        }
    });

    var ASPxFileValidator = ASPx.CreateClass(null, {
        constructor: function(options) {
            this.validationSettings = options.validationSettings;
            this.options = options;
            this.validators = this.CreateFileValidators();

            this.ValidationErrorInternal = new ASPxClientEvent();
        },
        clearValidationResult: function() {
            this.result = {
                validFileInfos: [],
                invalidFileNames: [],
                isValid: true,
                exceedCount: 0,
                initialFilesCount: 0,
                errorText: ""
            };
        },
        getFileExtension: function (fileName) {
            return fileName.replace(/.*?(\.[^.\\\/:*?\"<>|]+$)/, "$1");
        },
        CreateFileValidators: function () {
            var that = this;
            var validators = {
                fileName: {
                    value: true,
                    errorText: that.validationSettings.invalidWindowsFileNameErrorText,
                    isValid: function(fileInfo) {
                        var fileName = ASPx.Str.Trim(fileInfo.fileName),
                            forbiddenCharsRegExp = /^[^\\/:\*\?"<>\|]+$/,
                            forbiddenNamesRegExp = /^(nul|prn|con|lpt[0-9]|com[0-9])(\.|$)/i;

                        return forbiddenCharsRegExp.test(fileName) && !forbiddenNamesRegExp.test(fileName);
                    },
                    getErrorText: function() {
                        return this.errorText;
                    }
                },
                fileSize: {
                    value: this.validationSettings.maxFileSize,
                    errorText: this.validationSettings.maxFileSizeErrorText,
                    isValid: function (fileInfo) {
                        return fileInfo.fileSize < this.value;
                    },
                    getErrorText: function () {
                        return this.errorText.replace("{0}", this.value);
                    }
                },
                fileExtensions: {
                    value: this.validationSettings.allowedFileExtensions,
                    errorText: this.validationSettings.notAllowedFileExtensionErrorText,
                    isValid: function (fileInfo) {
                        var fileExtension = that.getFileExtension(fileInfo.fileName).toLowerCase();
                        return ASPx.Data.ArrayIndexOf(this.value, fileExtension) != -1;
                    },
                    getErrorText: function () {
                        return this.errorText;
                    }
                }
            };

            return this.options.advancedModeEnabled ? validators : { fileName: validators["fileName"] };
        },
        validateFileCore: function(fileInfo) {
            for(var validatorName in this.validators) {
                var validator = this.validators[validatorName];

                if(validator != null && validator.value) {
                    if(!validator.isValid(fileInfo)) {
                        this.result.commonErrorText = validator.getErrorText();
                        return false;
                    }
                }
            }
            return true;
        },
        validate: function(fileInfos) {
            this.clearValidationResult();

            this.validateFiles(fileInfos);

            if(!this.result.isValid) {
                this.raiseValidationError(this.result.errorText);
            }

            return this.result.validFileInfos;
        },
        validateFiles: function(fileInfos) {
            this.result.initialFilesCount = ASPxFileInfoCache.getPlainArray(fileInfos).length;
            this.result.validFileInfos = fileInfos;

            if(this.validationSettings.maxFileCount > 0 && this.options.enableMultiSelect)
                this.validateFilesCount(fileInfos);

            this.validateFilesProperties();
        },
        validateFilesProperties: function() {
            var fileInfos = this.result.validFileInfos;

            for(var i = 0; i < fileInfos.length; i++) {
                var validFileInfos = [];
                ASPx.Data.ForEach(fileInfos[i].reverse(), function(fileInfo) {
                    var isValid = this.validateFileCore(fileInfo);

                    if(isValid)
                        validFileInfos.push(fileInfo);
                    else {
                        this.result.invalidFileNames.push(fileInfo.fileName);
                        fileInfo.dispose();
                    }
                }.aspxBind(this));

                fileInfos[i] = validFileInfos.slice();

                fileInfos[i].reverse();
            }

            var isValid = !this.result.invalidFileNames.length;
            if(!isValid)
                this.result.errorText = this.prepareResultErrorText();

            this.result.isValid = this.result.isValid && isValid;
            this.result.validFileInfos = fileInfos;
        },
        validateFilesCount: function() {
            var fileInfosArray = ASPxFileInfoCache.getPlainArray(this.result.validFileInfos),
                invalidFilesCount,
                validFileInfos;

            invalidFilesCount = fileInfosArray.length - this.validationSettings.maxFileCount;
            validFileInfos = fileInfosArray.splice(0, this.validationSettings.maxFileCount);
            if(invalidFilesCount < 0)
                invalidFilesCount = 0;

            ASPx.Data.ForEach(fileInfosArray.reverse(), function(fileInfo) {
                if(fileInfo.fileName)
                    fileInfo.dispose();
            });

            this.result.exceedCount = invalidFilesCount;
            this.result.validFileInfos = [validFileInfos];
            this.result.errorText = this.prepareMaxFileCountErrorText();
            this.result.isValid = invalidFilesCount === 0;
        },
        prepareMaxFileCountErrorText: function() {
            var errorText = "";
            if(this.result.exceedCount > 0) {
                errorText = this.validationSettings.maxFileCountErrorText
                    .replace("{0}", this.result.exceedCount)
                    .replace("{1}", this.validationSettings.maxFileCount);
            }

            return errorText;
        },
        prepareResultErrorText: function() {
            var errorText,
                multipleFilesSelected = this.result.initialFilesCount > 1 && this.options.enableMultiSelect;

            if(multipleFilesSelected) {
                errorText = this.validationSettings.multiSelectionErrorText
                    .replace("{0}", this.result.invalidFileNames.length)
                    .replace("{1}", this.validators.fileSize.value)
                    .replace("{2}", this.result.invalidFileNames.join(', '));
            } else
                errorText = this.result.commonErrorText;

            if(this.result.errorText.length)
                errorText += "\n\n" + this.result.errorText;

            return errorText;
        },
        raiseValidationError: function(errorText) {
            var args = {
                text: errorText,
                type: ASPxUploadErrorTypes.Validation
            };

            this.ValidationErrorInternal.FireEvent(args);
        }
    });

    var ASPxFileInfo = ASPx.CreateClass(null, {
        constructor: function(file, inputIndex, input) {
            this.file = file;
            this.fileName = file.name || file.fileName;
            this.fileSize = file.size || file.fileSize || 0;
            this.fileType = file.type || file.fileType;
            this.fullName = "C:\\fakepath\\" + this.fileName;
            this.inputIndex = inputIndex;

            this.OnDispose = new ASPxClientEvent();
            this.OnUploadStart = new ASPxClientEvent();
            this.OnUploadComplete = new ASPxClientEvent();
        },
        dispose: function() {
            this.OnDispose.FireEvent(this);
        }
    });

    // view
    var ASPxViewManager = ASPx.CreateClass(null, {
        constructor: function(options) {
            this.options = options;
            this.domHelper = options.domHelper;
            this.options.parentNode = this.GetUploadInputsTable();
            this.progressPanelView = null;

            this.InternalError = new ASPxClientEvent();
            this.StateChanged = new ASPxClientEvent();
            this.UploadCancelled = new ASPxClientEvent();
            this.UploadStarted = new ASPxClientEvent();
            this.FileInputCountChangedInternal = new ASPxClientEvent();
            this.DropZoneEnter = new ASPxClientEvent();
            this.DropZoneLeave = new ASPxClientEvent();
            this.DropZoneDropInternal = new ASPxClientEvent();

            this.InternalError.AddHandler(this.HandleError.aspxBind(this));
        },
        InlineInitialize: function() {
            this.initializeViewCollection(this.options);

            this.applyToViewCollection("InlineInitialize");
        },
        Initialize: function() {
            this.initializeViews();
            //errors, buttons(cancel, upload)
            this.isInCallback = false;

            this.initializeHandlers();
        },
        initializeViewCollection: function(options) {
            this.viewCollection = [];
            var inputView = this.CreateFileInputView(options);

            if(inputView)
                this.viewCollection.push(inputView);

            if(options.enableFileList)
                this.viewCollection.push(new ASPxFileListView(options));

            if(options.enableDragAndDrop) {
                var dropZoneView = new ASPxDropZoneView(options);
                dropZoneView.DropZoneEnterInternal.AddHandler(this.raiseDropZoneEnter.aspxBind(this));
                dropZoneView.DropZoneLeaveInternal.AddHandler(this.raiseDropZoneLeave.aspxBind(this));
                dropZoneView.DropZoneDropInternal.AddHandler(this.raiseDropZoneDrop.aspxBind(this));
                this.viewCollection.push(dropZoneView);
            }

            if(options.enableProgressPanel) {
                this.progressPanelView = new ASPxProgressPanelView(options);
                this.viewCollection.push(this.progressPanelView);
            }

            this.uploadButton = new ASPxButtonView(options, IdSuffixes.Input.ButtonCell.Upload, this.uploadButtonHandler.aspxBind(this));
            this.cancelButton = new ASPxButtonView(options, IdSuffixes.Input.ButtonCell.Cancel, this.cancelButtonHandler.aspxBind(this));

            this.errorView = new ASPxErrorView(options);

            ASPx.Data.ForEach(this.viewCollection, function(view) {
                view.ErrorOccurred.AddHandler(this.HandleError.aspxBind(this));
            }.aspxBind(this));
        },
        initializeViews: function() {
            this.applyToViewCollection("Initialize");
        },
        uploadButtonHandler: function() {
            this.UploadStarted.FireEvent();
        },
        cancelButtonHandler: function() {
            this.UploadCancelled.FireEvent();
        },
        GetUploadInputsTable: function() {
            return this.domHelper.GetChildElement(IdSuffixes.Input.UploadInputsTable);
        },
        initializeHandlers: function() {
            for(var i = 0; i < this.viewCollection.length; i++) {
                var view = this.viewCollection[i];

                view.StateChangedInternal.AddHandler(
                    function(view, args) {
                        this.OnViewStateChanged(view, args);
                    }.aspxBind(this));
            }
        },
        CreateFileInputView: function(options) {
            var fileInputStrategy,
                fileInputView;

            if(options.isNative)
                fileInputView = ASPxNativeInputView;
            else if(options.advancedModeEnabled || options.autoModeEnabled) {
                if(options.isFileApiAvailable)
                    fileInputView = ASPxHTML5InputView;
                else if(options.isSLEnabled)
                    fileInputView = ASPxSLInputView;
            } else
                fileInputView = ASPxStandardInputView;

            // fallback when autoMode is set
            if(!fileInputView) {
                if(options.autoModeEnabled)
                    fileInputView = ASPxStandardInputView;
                else if(options.advancedModeEnabled)
                    this.InternalError.FireEvent({ type: ASPxUploadErrorTypes.Platform });
            }

            if(fileInputView) {
                if(this.isInputsVisible()) {
                    fileInputStrategy = new ASPxMultiFileInputView(options, fileInputView);
                    fileInputStrategy.FileInputCountChangedInternal.AddHandler(this.raiseFileInputCountChanged.aspxBind(this));

                    this.addButton = new ASPxButtonView(options, IdSuffixes.Input.ButtonCell.Add, function() {
                        fileInputStrategy.addFileInput();
                    }.aspxBind(fileInputStrategy));
                    this.addButton.SetEnabled(true);

                    this.domHelper.isMultiFileInput = true;

                    if(fileInputStrategy)
                        fileInputStrategy.FocusNeedResetInternal.AddHandler(this.resetFocus.aspxBind(this));
                }
                else
                    fileInputStrategy = new ASPxInvisibleFileInputDecorator(options, fileInputView);
            }

            return fileInputStrategy;
        },
        applyToViewCollection: function(method, args) {
            ASPx.Data.ForEach(this.viewCollection, function(view) {
                if(view[method])
                    view[method].apply(view, args || []);
            });
        },
        resetFocus: function(view, args) {
            var element = view.GetNextFocusElement(args);

            if(!args.backward) {
                if(!element)
                    element = this.addButton && this.addButton.GetLink();
                if(!element)
                    element = this.uploadButton && this.uploadButton.GetLink();
            }

            if(element) {
                element.focus();
                ASPx.Evt.PreventEvent(args.event);
            }
        },
        showFileInputError: function(error) {
            this.applyToViewCollection("showError", [error]);
        },
        showPlatformError: function() {
            ASPx.SetElementDisplay(this.GetUploadInputsTable(), false);
            ASPx.SetElementDisplay(this.getPlatformErrorPanel(), true);
        },
        getPlatformErrorPanel: function() {
            return this.domHelper.GetChildElement(IdSuffixes.Error.PlatformErrorTable);
        },
        showValidationError: function(error) {
            window.alert(error.text);
        },
        raiseFileInputCountChanged: function() {
            var args = new ASPxClientEventArgs();
            this.FileInputCountChangedInternal.FireEvent(this, args);
        },
        raiseDropZoneEnter: function(args) {
            this.DropZoneEnter.FireEvent(args);
        },
        raiseDropZoneLeave: function(args) {
            this.DropZoneLeave.FireEvent(args);
        },
        raiseDropZoneDrop: function(args) {
            this.DropZoneDropInternal.FireEvent(args);
        },
        OnViewStateChanged: function(view, args) {
            if(args.uploadCancelled)
                this.UploadCancelled.FireEvent();
            else
                this.StateChanged.FireEvent(args);
        },
        OnUploadInitiated: function() {
            this.errorView.Clear();
            this.applyToViewCollection("clearErrors");
            this.applyToViewCollection("OnUploadInitiated");
        },
        OnBeginProcessUploading: function() {
            this.applyToViewCollection("OnBeginProcessUploading");
            this.SetViewsEnabled(false);
        },
        OnUploadFilesComplete: function(args) {
            if(!this.lock) {
                this.lock = true;
                this.ShowCommonError(args);
                this.SetViewsEnabled(true);

                if(args.uploadCancelled)
                    this.uploadButton.SetEnabled(true);

                this.applyToViewCollection("OnUploadFilesComplete", [args]);

                this.lock = false;
            }
        },
        OnDocumentMouseUp: function() {
            this.applyToViewCollection("OnDocumentMouseUp");
        },
        OnDragEnter: function(args) {
            this.applyToViewCollection("OnDragEnter", args);
        },
        OnDragLeave: function(args) {
            this.applyToViewCollection("OnDragLeave", args);
        },
        OnDrop: function(args) {
            this.applyToViewCollection("OnDrop", args);
        },
        HandleError: function(error) {
            switch (error.type) {
                case ASPxUploadErrorTypes.Common:
                    this.errorView.UpdateCommonErrorDiv(error.text);
                    break;
                case ASPxUploadErrorTypes.InputRowError:
                    this.showFileInputError(error);
                    break;
                case ASPxUploadErrorTypes.Platform:
                    this.showPlatformError();
                    break;
                case ASPxUploadErrorTypes.Validation:
                    this.showValidationError(error);
                    break;
            }
        },
        SetFileInputRowEnabled: function(enabled, index) {
            this.applyToViewCollection("SetFileInputRowEnabled", [enabled, index]);
        },
        RefreshViews: function(args) {
            if(!this.lock) {
                this.lock = true;
                this.errorView.Clear();
                args.skipRefreshInput = this.options.enableFileList && this.options.enableDragAndDrop;

                ASPx.Data.ForEach(this.viewCollection, function(view) {
                    view.Refresh(args);
                });

                this.lock = false;
            }

            this.uploadButton.SetEnabled(!!args.fileInfosCount);
        },
        GetText: function(index) {
            var text = "";

            ASPx.Data.ForEach(this.viewCollection, function(view) {
                if(view.GetText)
                    text = view.GetText(index || 0)
            });

            return text || "";
        },
        setText: function(text, index) {
            this.applyToViewCollection("setText", [text, index]);
        },
        setTooltip: function(text, index) {
            this.applyToViewCollection("setTooltip", [text, index]);
        },
        AddFileInput: function() {
            if(!this.options.enableDragAndDrop && !this.options.enableFileList)
                this.applyToViewCollection("addFileInput");
        },
        RemoveFileInput: function(index) {
            this.applyToViewCollection("removeFileInput", [index]);
        },
        SetFileInputCount: function(count) {
            this.applyToViewCollection("setFileInputCount", [count]);
        },
        GetFileInputElement: function(index) {
            var element;

            ASPx.Data.ForEach(this.viewCollection, function(view) {
                if(view.GetFileInputElement)
                    element = view.GetFileInputElement(index)
            });

            return element;
        },
        Clear: function(args) {
            if(!this.lock) {
                this.lock = true;
                this.applyToViewCollection("Refresh", [ASPxEmptyRefreshArgs]);
                this.lock = false;
            }
        },
        ShowCommonError: function(args) {
            this.errorView.Refresh(args);
        },
        UpdateProgress: function(args) {
            this.applyToViewCollection("UpdateProgress", [args]);
        },
        InCallbackChanged: function(isInCallback) {
            this.applyToViewCollection("setInCallback", [isInCallback]);
        },
        SetViewsEnabled: function(enabled) {
            this.applyToViewCollection("SetEnabled", [enabled]);
            if(!enabled)
                this.uploadButton.SetEnabled(false);

            if(this.addButton)
                this.addButton.SetEnabled(enabled);
        },
        InvokeTextChangedInternal: function(index) {
            if(!this.lock)
                this.applyToViewCollection("InvokeTextChangedInternal", [index]);
        },
        getButtonLinkById: function(idPrefix) {
            var button = this.domHelper.GetChildElement(idPrefix),
                link = ASPx.GetNodeByTagName(button, "A", 0);

            return link || {};
        },
        SetAddButtonText: function(text) {
            this.getButtonLinkById(IdSuffixes.Input.ButtonCell.Add).innerHTML = text;
        },
        SetUploadButtonText: function(text) {
            this.getButtonLinkById(IdSuffixes.Input.ButtonCell.Upload).innerHTML = text;
        },
        GetAddButtonText: function() {
            return this.getButtonLinkById(IdSuffixes.Input.ButtonCell.Add).innerHTML || null;
        },
        GetUploadButtonText: function() {
            return this.getButtonLinkById(IdSuffixes.Input.ButtonCell.Upload).innerHTML || null;
        },
        SetEnabled: function(enabled) {
            this.SetViewsEnabled(enabled);
        },
        SetExternalDropZoneID: function(ids) {
            this.applyToViewCollection("SetExternalDropZoneID", [ids.split(";")]);
        },
        SetDialogTriggerID: function(ids) {
            this.applyToViewCollection("setDialogTriggerID", [ids]);
        },
        setInlineDropZoneAnchorElementID: function(id) {
            this.applyToViewCollection("SetInlineDropZoneAnchorElementID", [id]);
        },
        GetProgressControl: function() {
            if(this.options.enableProgressPanel)
                return this.progressPanelView.GetProgressControl();
            return null;
        },
        UpdateErrorMessageCell: function(index, errorText, isValid) {
            var args = {
                index: index,
                errorText: errorText,
                isValid: isValid
            };
            this.errorView.UpdateErrorMessageCell(args);
        },
        AdjustSize: function() {
            this.adjustMainElementWidth();
            this.applyToViewCollection("AdjustSize");
        },
        adjustMainElementWidth: function () {
            var element = this.domHelper.GetMainElement();
            if(this.domHelper.IsDisplayed() && element.style.width == "") {
                if(ASPx.Browser.IE)
                    element.style.width = ASPx.GetClearClientWidth(element);
                else
                    element.style.width = ASPx.GetCurrentStyle(element).width;
            }
        },
        isInputsVisible: function() {
            return !ASPx.ElementHasCssClass(this.domHelper.GetMainElement(), CSSClasses.HiddenUI);
        },
        suppressFileDialog: function(suppress) {
            this.applyToViewCollection("suppressFileDialog", [suppress]);
        }
    });

    var ASPxBaseView = ASPx.CreateClass(null, {
        constructor: function(options) {
            options = options || [];
            this.baseName = options.name;

            this.IsRightToLeft = options.IsRightToLeft;
            this.options = options;
            this.domHelper = options.domHelper;
            this.id = options.id || 0;
            this.containerNode = options.containerNode;
            this.supressEvents = false;
            this.cache = {};
            this.buttonEventHandlers = {};

            this.StateChangedInternal = new ASPxClientEvent();
            this.ErrorOccurred = new ASPxClientEvent();
        },
        Initialize: function() { },
        InlineInitialize: function() { },
        GetContainerNode: function() {
            return this.containerNode || this.domHelper.GetChildElement(IdSuffixes.Input.UploadInputsTable);
        },
        SetButtonEnabled: function(element, enabled) {
            this.ChangeButtonEnabledState(element, enabled);
            this.ChangeButtonEnabledAttributes(element, ASPx.Attr.ChangeAttributesMethod(enabled), enabled);
        },
        ChangeButtonEnabledState: function(element, enabled) {
            if(element)
                ASPx.GetStateController().SetElementEnabled(element, enabled);
        },
        ChangeButtonEnabledAttributes: function(element, method, enabled) {
            if(element) {
                var link = ASPx.GetNodeByTagName(element, "A", 0);
                if(link) {
                    var isBrowseButton = element.className.indexOf(CSSClasses.BrowseButtonCell) != -1;
                    if(!isBrowseButton)
                        ASPx.Attr.SetOrRemoveAttribute(link, "tabindex", !enabled ? "-1" : "0");
                    ASPx.Attr.SetOrRemoveAttribute(link, "unselectable", !enabled ? "on" : null);
                    if(ASPx.Browser.NetscapeFamily || ASPx.Browser.WebKitFamily) {
                        method = this.ChangeEventsMethod(!enabled);
                        method(link, "mousedown", function(e) {
                            e.preventDefault();
                            return false;
                        }, true);
                    }
                }
            }
        },
        attachButtonHandler: function(element, enabled) {
            var method = this.ChangeEventsMethod(enabled);
            method(element, "click", this.buttonEventHandlers[element.id]);
        },
        GetElementFromCacheByClassName: function(className) {
            if(!this.cache[className])
                this.cache[className] = ASPx.GetNodesByPartialClassName(this.GetRenderResult(), className)[0];

            return this.cache[className];
        },
        GetID: function() {
            return this.id;
        },
        GetFullID: function() {
            return this.GetName() + this.GetInputRowPrefix() + this.GetID();
        },
        GetName: function() {
            return this.baseName;
        },
        GetUploadInputsTable: function() {
            return this.domHelper.GetChildElement(IdSuffixes.Input.UploadInputsTable);
        },
        GetInputRow: function(id) {
            return this.domHelper.GetChildElement(this.GetInputRowId(id));
        },
        GetInputRowPrefix: function() {
            return IdSuffixes.Input.FileInputRow;
        },
        GetInputRowId: function(id) {
            return this.GetInputRowPrefix() + (id || this.GetID());
        },
        GetRowTemplate: function() {
            return this.GetInputRow("T");
        },
        GetFileInfos: function() {
            return this.fileInfos;
        },
        getFileInfos: function() {
            return this.fileInfos;
        },
        GetFileNames: function(isShortName) {
            var fileInfos = this.getFileInfos(),
                files = [];

            for(var i in fileInfos) {
                if(fileInfos[i])
                    files.push(isShortName ? fileInfos[i].fileName : fileInfos[i].fullName);
            }

            return files;
        },
        OnUploadFilesComplete: function() { },
        OnBeginProcessUploading: function() { },
        setInCallback: function(isInCallback) {
            this.isInCallback = isInCallback;
        },
        IsSlModeEnabled: function() {
            return this.options.isSLEnabled && !this.options.isFileApiAvailable && (this.options.advancedModeEnabled || this.options.autoModeEnabled);
        },
        RaiseStateChangedInternal: function(view) {
            var args = view.prepareInternalStateChangedArgs(view);

            if(!this.supressEvents && this.StateChangedInternal)
                this.StateChangedInternal.FireEvent(this, args);
        },
        raiseError: function(errorText) {
            var args = {
                type: ASPxUploadErrorTypes.Validation,
                text: errorText
            };

            this.ErrorOccurred.FireEvent(args);
        },
        StopEventPropagation: function(evt) {
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion <= 8)
                evt.cancelBubble = true;    // B236108
        },
        prepareInternalStateChangedArgs: function(view) {
            var fileInfos = view.GetFileInfos.call(view),
                inputIndex = view.GetID.call(view);

            return new ASPxViewStateChangedInternalArgs(fileInfos, inputIndex);
        },
        AttachEventForElement: function(element, eventName, func, detachOldEvent) {
            if(detachOldEvent && element["dx" + eventName])
                ASPx.Evt.DetachEventFromElement(element, eventName, element["dx" + eventName]);
            element["dx" + eventName] = func;
            ASPx.Evt.AttachEventToElement(element, eventName, element["dx" + eventName]);
        },
        DetachEventForElement: function(element, eventName) {
            if(element["dx" + eventName]) {
                ASPx.Evt.DetachEventFromElement(element, eventName, element["dx" + eventName]);
                element["dx" + eventName] = null;
            }
        },
        ChangeEventsMethod: function(attach) {
            return attach ? this.AttachEventForElement : this.DetachEventForElement;
        },
        UpdateIndex: function() {},
        Clear: function() {
            this.refreshBase(ASPxEmptyRefreshArgs);
            this.RaiseStateChangedInternal(this);
        },
        Dispose: function() {
            this.dropElementCache();
            ASPx.RemoveElement(this.GetRenderResult());
        },
        SetEnabled: function(enabled) { },
        EnsureRender: function() {
            if(!this.GetRenderResult()) {
                this.Render();
                this.AfterRender();
            }
        },
        Render: function() { },
        AfterRender: function() { },
        Refresh: function(args) {
            this.supressEvents = true;
            this.refreshBase(args);
            this.supressEvents = false;
        },
        refreshBase: function(args) {
            this.updateFileInfos(args);
        },
        updateFileInfos: function(args) {
            this.fileInfos = args.fileInfos;
        },
        GetRenderResult: function() {
            if(!this.renderResult)
                this.renderResult = this.domHelper.GetChildElement(this.GetInputRowId());

            return this.renderResult;
        },
        getOwnerControl: function () {
            return aspxGetUploadControlCollection().Get(this.baseName);
        },
        dropElementCache: function(id) {
            ASPx.CacheHelper.DropCachedValue(this.getOwnerControl(), id || this.GetFullID());
        }
    });

    var ASPxButtonView = ASPx.CreateClass(ASPxBaseView, {
        constructor: function(options, idSuffix, handler, disabledTemplate) {
            this.constructor.prototype.constructor.call(this, options);
            this.disabledItemTemplate = disabledTemplate;

            this.SetId(idSuffix);
            this.SetHandler(handler);
            this.CreateDisabledState();
            this.SetEnabled(false);
        },
        CreateDisabledState: function() {
            if(this.disabledItemTemplate) {
                ASPx.GetStateController().AddDisabledItem(this.GetName() + this.GetID(),
                    this.disabledItemTemplate.className, this.disabledItemTemplate.cssText,
                    this.disabledItemTemplate.postfixes, this.disabledItemTemplate.imageObjs,
                    this.disabledItemTemplate.imagePostfixes);
            }
        },
        GetRenderResult: function() {
            return this.domHelper.GetChildElement(this.GetID());
        },
        GetLink: function() {
            return ASPx.GetNodeByTagName(this.GetRenderResult(), "A", 0);
        },
        SetId: function(id) {
            this.dropElementCache(this.GetName() + this.GetID());

            this.id = id;
        },
        SetHandler: function(handler) {
            this.handler = handler;
        },
        SetEnabled: function(enabled) {
            var method = this.ChangeEventsMethod(enabled),
                markup = this.GetRenderResult();

            if(markup) {
                method(this.GetRenderResult(), "click", this.handler, true);
                this.SetButtonEnabled(markup, enabled);
            }
        }
    });

    var ASPxCompositeView = ASPx.CreateClass(ASPxBaseView, {
        constructor: function(options) {
            this.constructor.prototype.constructor.call(this, options);
            this.initialized = false;
            this.views = [];
            this.removeButtons = [];
            this.options.containerNode = this.GetContainerNode();
            this.internalCount = this.internalCount || 0;
            this.internalIndex = this.internalIndex || 0;
            this.viewPrototype = options.viewPrototype || this.viewPrototype || undefined;

            this.templates = options.templates;

            this.createViews();
        },
        createViews: function() {
            for(var i = 0; i < this.internalCount; i++)
                this.addView();
        },
        Initialize: function() {
            ASPx.Data.ForEach(this.views, function(view) {
                view.Initialize();
                view.Refresh(ASPxEmptyRefreshArgs);
            });

            this.initialized = true;
        },
        InlineInitialize: function() {
            ASPx.Data.ForEach(this.views, function(view) {
                view.InlineInitialize();
            });
        },
        addView: function(fileInfo) {
            var index = this.internalIndex;

            this.options.id = index;
            
            var view = new this.viewPrototype(this.options),
                removeButton = new ASPxButtonView(this.options, this.getRemoveButtonPostfix(index), function() {
                    this.onRemoveButtonClick(index);
                }.aspxBind(this), this.templates.DisabledRemoveItem);

            view.EnsureRender(fileInfo);

            if(this.initialized) {
                view.Initialize();
                view.Refresh(ASPxEmptyRefreshArgs);
            }
            removeButton.SetEnabled(true);

            this.views[index] = view;
            this.removeButtons[index] = removeButton;

            view.StateChangedInternal.AddHandler(this.onInternalStateChanged, this);

            this.internalIndex++;
        },
        removeView: function(index) {
            if(this.views[index]) {
                this.removeButtons[index].SetEnabled(false);

                this.views[index].Dispose();
                this.views[index] = undefined;

                for(var viewIndex = index + 1; viewIndex < this.internalCount; viewIndex++) {
                    var newIndex = viewIndex - 1;

                    this.changeRemoveHandler(viewIndex, newIndex);

                    this.views[viewIndex].UpdateIndex.call(this.views[viewIndex], newIndex);
                    this.views[newIndex] = this.views[viewIndex];
                    this.removeButtons[newIndex].SetEnabled(true);
                }

                this.internalCount--;
                this.internalIndex = this.internalCount;

                // array shifted
                this.views.splice(this.internalCount, 1);

                this.RaiseStateChangedInternal(this)
            }
        },
        getRemoveButtonPostfix: function(index) {
            return  IdSuffixes.Input.ButtonCell.Remove + index;
        },
        onRemoveButtonClick: function(index) {
            this.disposeFileInfo(index);
            this.removeView(index);
        },
        changeRemoveHandler: function(oldIndex, newIndex) {
            this.removeButtons[oldIndex].SetEnabled(false);
            this.removeButtons[newIndex] = this.removeButtons[oldIndex];
            this.removeButtons[newIndex].SetId(this.getRemoveButtonPostfix(newIndex));
            this.removeButtons[newIndex].SetHandler(function() {
                this.onRemoveButtonClick(newIndex);
            }.aspxBind(this));
        },
        refreshView: function(view, commonArgs) {
            var args = this.getViewRefreshArgs(view, commonArgs);
            view.Refresh(args);
        },
        getViewRefreshArgs: function(view, commonArgs) {
            return {
                fileInfos: this.fileInfos[view.GetID()],
                isStateChanged: commonArgs.isStateChanged
            };
        },
        GetView: function(index) {
            return this.views[index];
        },
        clearViews: function() {
            ASPx.Data.ForEach(this.views, this.clearView.aspxBind(this));
        },
        clearView: function(view, index) {
            view.Clear();
        },
        FilterViews: function() {
            /*this.views = this.views.filter(function(view) {
                return view !== undefined;
            });*/
        },
        GetContainerNode: function() {
            var inputsTable = this.domHelper.GetChildElement(IdSuffixes.Input.UploadInputsTable);
            return ASPx.GetChildByTagName(inputsTable, "TBODY");
        },
        onInternalStateChanged: function() {
            this.RaiseStateChangedInternal();
        },
        RaiseStateChangedInternal: function() {
            this.FilterViews();

            ASPxBaseView.prototype.RaiseStateChangedInternal.call(this, this);
        },
        SetFileInputRowEnabled: function(enabled, index) {
            if(this.views[index]) {
                this.views[index].SetFileInputRowEnabled(enabled);
                this.removeButtons[index].SetEnabled(enabled);
            }
        },
        GetRemoveButtonCell: function(index) {
            var renderResult = this.views[index].GetRenderResult();

            return ASPx.GetNodesByPartialClassName(renderResult, CSSClasses.RemoveButtonCell)[0];
        },
        OnUploadFilesComplete: function(args) {
            ASPx.Data.ForEach(this.views, function(view) {
                view.OnUploadFilesComplete.call(view, args);
            });
        },
        SetEnabled: function(enabled) {
            ASPx.Data.ForEach(this.views, function(view, index) {
                view.SetEnabled(enabled);
                this.removeButtons[index].SetEnabled(enabled);
            }.aspxBind(this));
        },
        ResetState: function() {
            ASPx.Data.ForEach(this.views, function(view) {
                view.ResetState();
            })
        },
        GetFileInfos: function() {
            var fileInfos = [];

            ASPx.Data.ForEach(this.views, function(view) {
                fileInfos.push(view.GetFileInfos());
            });

            return fileInfos;
        },
        refreshBase: function(args) {
            ASPxBaseView.prototype.refreshBase.call(this, args);

            ASPx.Data.ForEach(this.views, function(view) {
                this.refreshView(view, args);
            }.aspxBind(this));
        },
        Clear: function() {
            this.supressEvents = true;
            this.clearViews();
            this.supressEvents = false;
        }
    });

    var ASPxFileListItem = ASPx.CreateClass(ASPxBaseView, {
        constructor: function(options) {
            this.constructor.prototype.constructor.call(this, options);
            this.templateDisabledRemoveItem = this.options.templateDisabledRemoveItem;
        },
        Render: function() {
            var template = this.GetRowTemplate().cloneNode(true);

            template.id = this.GetFullID();
            this.GetContainerNode().appendChild(template);
            this.progressControl = this.cloneProgressControl(template);

            ASPx.SetElementDisplay(template, true);

            this.correctNameSpanWidth();

            this.RedefineAttributes();
            this.SetState(CSSClasses.FileList.State.Pending);
        },
        cloneProgressControl: function(template) {
            var progressBarTemplateName = this.baseName + IdSuffixes.Input.FileList.ProgressControl,
                templateProgressControl = ASPx.GetControlCollection().Get(progressBarTemplateName),
                clonedBarMarkup = ASPx.GetNodesByPartialClassName(template, CSSClasses.FileList.ProgressBar)[0],
                indicatorClassName = templateProgressControl.GetIndicatorDiv().className,
                clonedIndicatorDiv = ASPx.GetNodeByClassName(template, indicatorClassName),
                progressControl;

            this.progressBarBaseId = templateProgressControl.name;

            clonedIndicatorDiv.id = this.progressBarBaseId + this.GetID() + "_DI";
            clonedBarMarkup.id = this.progressBarBaseId + this.GetID();

            progressControl = new ASPxClientProgressBarBase(this.progressBarBaseId + this.GetID());
            progressControl.mainElement = clonedBarMarkup;
            progressControl.displayMode = templateProgressControl.displayMode;
            progressControl.customDisplayFormat = templateProgressControl.customDisplayFormat;
            progressControl.position = templateProgressControl.position;
            progressControl.minimum = templateProgressControl.minimum;
            progressControl.maximum = templateProgressControl.maximum;
            progressControl.onePercentValue = templateProgressControl.onePercentValue;
            progressControl.AfterCreate();

            return progressControl;
        },
        correctNameSpanWidth: function() {
            var progressBarShare = 0.283,
                fileNameShare = 1 - progressBarShare;
            var controlWidth = this.domHelper.GetMainElement().offsetWidth,
                fileRow = this.GetRenderResult(),
                fileNameCell = this.GetNameCell(fileRow),
                progressBarCell = this.getProgressBarCell(),
                stateDiv = ASPx.GetChildNodesByTagName(fileNameCell, "DIV");

            var fileNameWidth = fileNameShare * controlWidth - 1;
            var progressBarWidth = progressBarShare * controlWidth - 1;

            for(var i = 0; i < stateDiv.length; i++) {
                var fileNameLabel = ASPx.GetChildByTagName(stateDiv[i], "SPAN");
                fileNameLabel.style.maxWidth = (fileNameWidth - ASPx.GetLeftRightPaddings(fileNameLabel)) + "px";
                progressBarCell.style.width = progressBarWidth + "px";
            }
        },
        Dispose: function() {
            this.dropElementCache();
            this.GetRenderResult().parentNode.removeChild(this.GetRenderResult());
        },
        refreshBase: function(args) {
            ASPxBaseView.prototype.refreshBase.call(this, args);
            var fileRow = this.GetRenderResult(),
                fileNameCell = this.GetNameCell(fileRow),
                fileInfo = args.fileInfos;

            var stateDiv = ASPx.GetChildNodesByTagName(fileNameCell, "DIV");
            for(var i = 0; i < stateDiv.length; i++) {
                var fileNameLabel = ASPx.GetChildByTagName(stateDiv[i], "SPAN");
                fileNameLabel.innerHTML = fileInfo.fileName || "";
                fileNameLabel.title = fileInfo.fileName || "";
            }


            fileRow.aspxFileInfos = fileInfo;
        },
        GetFileInfos: function() {
            return ASPxBaseView.prototype.GetFileInfos.call(this);
        },
        RedefineAttributes: function(oldIndex) {
            this.dropElementCache();
            this.dropElementCache(this.GetRemoveButtonCell().id);
            
            this.GetRenderResult().id = this.GetFullID();
            this.GetRemoveButtonCell().id = this.GetRemoveRowButtonId();
            this.redefineProgressBarAttributes();
        },
        redefineProgressBarAttributes: function() {
            var progressbarMarkup = this.progressControl.GetMainElement(),
                progressBarIndicatorMarkup = this.progressControl.GetIndicatorDiv();

            this.dropElementCache(progressbarMarkup.id);
            this.dropElementCache(progressBarIndicatorMarkup.id);

            progressbarMarkup.id = this.progressBarBaseId + this.GetID();
            progressBarIndicatorMarkup.id = this.progressBarBaseId + this.GetID() + "_DI";
        },
        setId: function(newId) {
            this.id = newId;
        },
        SetState: function(newState) {
            var stateDiv = ASPx.GetChildNodesByTagName(this.GetNameCell(), "DIV")[0];

            if(!ASPx.ElementHasCssClass(stateDiv, newState)) {
                var stateClasses = CSSClasses.FileList.State;
                for(var state in stateClasses)
                    ASPx.RemoveClassNameFromElement(stateDiv, stateClasses[state]);

                ASPx.AddClassNameToElement(stateDiv, newState);
            }
        },
        ResetState: function() {
            this.SetState(CSSClasses.FileList.State.Pending);
        },
        SetEnabled: function(enabled) {

        },
        GetRemoveRowButtonId: function() {
            return this.GetName() + IdSuffixes.Input.FileList.RemoveRowButton + this.GetID();
        },
        GetInputRowPrefix: function() {
            return IdSuffixes.Input.FileList.Row;
        },
        UpdateIndex: function(newIndex) {
            var oldIndex = this.GetID();

            this.setId(newIndex);
            this.RedefineAttributes(oldIndex);
        },
        GetNameCell: function() {
            return ASPx.GetChildByClassName(this.GetRenderResult(), CSSClasses.FileList.NameCell);
        },
        GetRemoveButtonCell: function() {
            return this.GetElementFromCacheByClassName(CSSClasses.FileList.RemoveButtonCell);
        },
        RaiseStateChangedInternal: function(args) {
            ASPxBaseView.prototype.RaiseStateChangedInternal.call(this, args);
        },
        OnFileItemUploadStart: function() {
            this.UpdateProgress({ currentFileProgress: 0 });
            this.SetState(CSSClasses.FileList.State.Uploading);
            this.progressControl.AdjustControl();
        },
        OnFileItemUploadComplete: function() {
            this.UpdateProgress({ currentFileProgress: 100 });
            this.SetState(CSSClasses.FileList.State.Complete);
        },
        OnUploadInitiated: function() {
            ASPx.SetElementDisplay(this.progressControl.GetMainElement(), true);
            ASPx.SetElementDisplay(this.getProgressBarCell(), true);
            ASPx.SetElementDisplay(this.GetRemoveButtonCell(), false);

            this.progressControl.AdjustControl(true);
        },
        OnUploadFilesComplete: function() {
            this.SetState(CSSClasses.FileList.State.Pending);
            ASPx.SetElementDisplay(this.getProgressBarCell(), false);
            this.UpdateProgress({ currentFileProgress: 0 });
        },
        UpdateProgress: function(args) {
            this.progressControl.SetPosition(parseInt(args.currentFileProgress));
        },
        getProgressBarCell: function() {
            return ASPx.GetNodesByPartialClassName(this.GetRenderResult(), CSSClasses.FileList.ProgressBarCell)[0];
        }
    });

    var ASPxFileListView = ASPx.CreateClass(ASPxCompositeView, {
        viewPrototype: ASPxFileListItem,
        constructor: function(options, id) {
            this.constructor.prototype.constructor.call(this, options);
            this.name = options.name + IdSuffixes.Input.FileList.List;
            this.progressControl = undefined;
            this.currentView = null;
            this.showList(false);
        },
        // TODO: maybe options.showProgressPanel?
        UpdateProgress: function(args) {
            if(this.currentView)
                this.currentView.UpdateProgress(args);
        },
        addView: function(fileInfo) {
            ASPxCompositeView.prototype.addView.call(this, fileInfo);

            var index = this.internalIndex - 1,
                view = this.views[index];

            fileInfo.OnUploadStart.ClearHandlers();
            fileInfo.OnUploadComplete.ClearHandlers();

            fileInfo.OnUploadStart.AddHandler(function() {
                view.OnFileItemUploadStart();
                this.currentView = view;
            }.aspxBind(this));

            fileInfo.OnUploadComplete.AddHandler(function() { view.OnFileItemUploadComplete(); }.aspxBind(this));
        },
        removeView: function(index) {
            ASPxCompositeView.prototype.removeView.call(this, index);

            if(this.internalCount === 0)
                this.showList(false);
        },
        disposeFileInfo: function(index) {
            if(this.fileInfos[index])
                this.fileInfos[index].dispose();
        },
        getRemoveButtonPostfix: function(index) {
            return IdSuffixes.Input.FileList.RemoveRowButton + index;
        },
        showList: function(show) {
            ASPx.SetElementDisplay(this.GetRenderResult(), show);
        },
        GetContainerNode: function() {
            return this.domHelper.GetChildElement(IdSuffixes.Input.FileList.List);
        },
        ResetState: function() {
            ASPxBaseInputView.prototype.ResetState.call(this);

            ASPx.Data.ForEach(this.views, function(view) {
                view.ResetState();
            });
        },
        OnUploadFilesComplete: function(args) {
            ASPx.Data.ForEach(this.views, function(view, index) {
                view.OnUploadFilesComplete();
                ASPx.SetElementDisplay(this.GetRemoveButtonCell(index), true);
            }.aspxBind(this));

            this.currentView = null;
        },
        refreshBase: function(args) {
            this.fileInfos = args.fileInfos[0] || [];

            if(args.forceClear || this.fileInfos.length < this.internalCount)
                this.Clear();

            if(args.forceClear)
                return;

            if(this.fileInfos.length)
                this.showList(true);

            var currentCount = this.internalCount,
                endIndex = this.fileInfos.length;

            for(var i = currentCount; i < endIndex; i++) {
                this.addView(this.fileInfos[i]);
                this.internalCount++;
            }

            ASPxCompositeView.prototype.refreshBase.call(this, args);
        },
        SetEnabled: function(enabled) {
            ASPxCompositeView.prototype.SetEnabled.call(this, enabled);
        },
        clearViews: function() {
            for(var i = this.views.length; i >= 0; --i)
                this.clearView(null, i)
        },
        clearView: function(view, index) {
            this.removeView(index);
        },
        OnUploadInitiated: function() {
            ASPx.Data.ForEach(this.views, function(view) {
                view.OnUploadInitiated();
            });
        },
        GetRenderResult: function() {
            if(!this.renderResult)
                this.renderResult = ASPx.GetNodesByPartialClassName(this.domHelper.GetMainElement(), CSSClasses.FileList.List)[0];

            return this.renderResult;
        },
        GetFileInfos: function() {
            return [ASPxCompositeView.prototype.GetFileInfos.call(this)];
        },
        updateFileInfos: function(args) {
            this.fileInfos = args.fileInfos[0];
        }
    });

    var ASPxBaseInputView = ASPx.CreateClass(ASPxBaseView, {
        constructor: function(options) {
            this.constructor.prototype.constructor.call(this, options);

            this.nullText = options.nullText;
            this.templates = options.templates;
            this.enabled = true;
            this.triggerCursorsList = {};

            this.FocusNeedResetInternal = new ASPxClientEvent();
        },
        Initialize: function() {
            this.prepareFileInputRowTemplate();
            this.InitializeFakeFocusInputElement();
            this.InitializeTemplates();
            this.initializeFileSelector();

            this.ChangeEventsToFileInput(true);
        },
        InitializeFakeFocusInputElement: function() {
            if(this.IsFocusNeedReset()) {
                var mainCell = this.GetUploadInputsTable().parentNode;
                var div = ASPx.CreateHtmlElementFromString("<div class='dxucFFIHolder'></div>");
                mainCell.appendChild(div);

                var fakeFocusInput = ASPx.CreateHtmlElementFromString("<input readonly='readonly' class='dxucFFI'></input>");
                fakeFocusInput.id = this.GetFakeFocusInputElementID();

                div.appendChild(fakeFocusInput);
            }
        },
        prepareFileInputRowTemplate: function() {
            this.fileInputRowTemplate = this.GetFileInputRowTemplate();
            this.fileInputRowTemplateNode = this.fileInputRowTemplate.cloneNode(true);
            ASPx.SetElementDisplay(this.fileInputRowTemplateNode, true);
        },
        initializeFileSelector: function() {
            var fileSelector = this.GetFileSelectorElement();

            if(fileSelector &&  fileSelector.accept)
                fileSelector.accept = fileSelector.accept.replace(/\/\*/g, "");
        },
        Render: function() {
            var errorRowTemplate = this.GetErrorRowTemplate(),
                separatorRow = this.GetFileInputSeparatorRowTemplate().cloneNode(true),
                addButtonSeparator = this.domHelper.GetChildElement(IdSuffixes.Input.AddButtonsSeparator),
                errorRow;

            if(errorRowTemplate) {
                errorRow = errorRowTemplate.cloneNode(true);
                this.errorRow = errorRow;
            }

            var inputRow = this.CreateInputRow();

            this.GetContainerNode().insertBefore(separatorRow, addButtonSeparator);
            this.GetContainerNode().insertBefore(inputRow, addButtonSeparator);

            if(errorRow) {
                this.GetContainerNode().insertBefore(errorRow, addButtonSeparator);
                ASPx.SetElementDisplay(errorRow, false);
            }

            ASPx.SetElementDisplay(separatorRow, true);
            ASPx.SetElementDisplay(inputRow, true);

            this.registerStates();
        },
        AfterRender: function() {
            this.ChangeEventsToFileInput(true);
        },
        Dispose: function() {
            ASPx.RemoveElement(this.GetErrorRow());
            ASPxBaseView.prototype.Dispose.call(this);
        },
        UpdateIndex: function(newIndex) {
            var oldIndex = this.GetID();

            this.dropElementCache();
            this.ChangeEventsToFileInput(false);
            this.setId(newIndex);
            this.RedefineAttributes(oldIndex);
            this.ChangeEventsToFileInput(true);
        },
        replaceFileInputElement: function() {
            var inputElement = this.getFileInputTemplate().cloneNode(),
                aspxFileInfos = this.GetFileInputElement().aspxFileInfos;

            this.GetFileInputElement().parentNode.replaceChild(inputElement, this.GetFileInputElement());

            this.cache[CSSClasses.TextboxInput] = null;

            this.RedefineInputAttributes();

            this.ChangeEventsToFileInput(false);
            this.ChangeEventsToFileInput(true);

            this.GetFileInputElement().aspxFileInfos = aspxFileInfos;
        },
        setId: function(newId) {
            this.id = newId;
        },
        CreateInputRow: function() {
            var inputRow = this.GetRowTemplate().cloneNode(true);
            this.renderResult = inputRow;

            this.RedefineAttributes("T");

            return inputRow;
        },
        refreshBase: function(args) {
            ASPxBaseView.prototype.refreshBase.call(this, args);

            if(args.isStateChanged)
                this.clearErrors();

            if(this.fileInfos && !this.fileInfos.length)
                this.Clear();
        },
        updateFileInfos: function(args) {
            this.fileInfos = args.fileInfos[this.GetID()];
        },
        Clear: function() {
            this.clearFileInputValue();

            this.fileInfos = [];

            this.replaceFileInputElementIfNeeded();

            ASPxBaseView.prototype.Clear.call(this);
        },
        clearFileInputValue: function() {
            this.supressInputEvent = true;
            this.GetFileInputElement().value = "";
            this.supressInputEvent = false;
        },
        replaceFileInputElementIfNeeded: function() {
            if(this.GetFileInputElement().value) {
                this.replaceFileInputElement();
                this.Refresh(ASPxEmptyRefreshArgs);
            }
        },
        clearErrors: function() {
            var errorCell = this.GetErrorCell();

            if(errorCell) {
                errorCell.innerHTML = "";
                ASPx.SetElementDisplay(this.GetErrorRow(), false);
            }
        },
        showError: function(error) {
            var errorCell = this.GetErrorCell();
            if(errorCell) {
                var currentErrors = errorCell.innerHTML;
                errorCell.innerHTML = currentErrors + error.text + "<br />";
                ASPx.SetElementDisplay(this.GetErrorRow(), true);
            }
        },
        Validate: function() { },
        GetRemoveRowButtonId: function() {
            return this.GetName() + IdSuffixes.Input.ButtonCell.Remove + this.GetID();
        },
        RedefineAttributes: function(oldIndex) {
            this.RedefineInputRowAttributes(this.GetRenderResult());
            if(this.errorRow)
                this.RedefineErrorRowAttributes(oldIndex);
            this.RedefineRemoveAttributes();
        },
        RedefineRemoveAttributes: function() {
            var removeButtonCell = ASPx.GetNodesByPartialClassName(this.GetRenderResult(), CSSClasses.RemoveButtonCell)[0];

            if(removeButtonCell)
                removeButtonCell.id = this.GetRemoveButtonId();
        },
        GetRemoveButtonId: function() {
            return this.GetName() + IdSuffixes.Input.ButtonCell.Remove + this.GetID();
        },
        RedefineErrorRowAttributes: function(oldIndex) {
            this.GetErrorRow(oldIndex).id = this.GetName() + this.GetErrorRowId();
        },
        RedefineInputRowAttributes: function() {
            this.GetRenderResult().id = this.GetFullID();
            this.RedefineTextBoxAttributes();
        },
        RedefineTextBoxAttributes: function() {
            var textBoxCell = this.GetTextBoxCell();

            if(textBoxCell) {
                textBoxCell.id = this.GetTextBoxCellID();
                this.RedefineInputAttributes();
            }
        },
        RedefineInputAttributes: function() {
            var input = this.GetFileInputElement();
            var newInputID = this.GetFileInputElementId();          
            input.id = newInputID;
            input.name = newInputID;
        },
        GetFileInputElementId: function() {
            return this.GetTextBoxCellID() + IdSuffixes.Input.FileInput;
        },
        ChangeTextBoxEnabledAttributes: function(element, method, enabled) {
            this.enabled = enabled;
            if(element) {
                var inputs = ASPx.GetNodesByTagName(element, "INPUT");
                for(var i = 0; i < inputs.length; i++)
                    inputs[i].disabled = !enabled;
            }
        },
        ChangeClearBoxEnabledAttributes: function(element, method, enabled) {
            if(element) {
                var link = ASPx.GetNodeByTagName(element, "A", 0);
                this.ChangeButtonEnabledAttributes(link, method, enabled);
            }
        },
        SetFileInputRowEnabled: function(enabled) {
            this.SetTextBoxEnabled(this.GetTextBoxCell(), enabled);
        },
        SetTextBoxEnabled: function(element, enabled) {
            this.ChangeTextBoxEnabledState(element, enabled);
            this.ChangeTextBoxEnabledAttributes(element, ASPx.Attr.ChangeAttributesMethod(enabled), enabled);
        },
        ChangeTextBoxEnabledState: function(element, enabled) {
            if(element) {
                ASPx.GetStateController().SetElementEnabled(element, enabled);
                var editArea = ASPx.GetNodeByTagName(element, "INPUT", 1);
                if(editArea)
                    ASPx.GetStateController().SetElementEnabled(editArea, enabled);
            }
        },
        SetClearBoxEnabled: function(element, enabled) {
            this.ChangeClearBoxEnabledState(element, enabled);
            this.ChangeClearBoxEnabledAttributes(element, ASPx.Attr.ChangeAttributesMethod(enabled), enabled);
        },
        ChangeClearBoxEnabledState: function(element, enabled) {
            if(element)
                ASPx.GetStateController().SetElementEnabled(element, enabled);
        },
        SetEnabled: function(enabled) {
            this.SetFileInputRowEnabled(enabled);
        },
        registerStates: function() {
            this.CreateTextBoxDisabledState();
            this.CreateClearBoxDisabledState();
            this.CreateBrowseHoveredState();
            this.CreateBrowsePressedState();
            this.CreateBrowseDisabledState();
        },
        CreateTextBoxDisabledState: function () {
            if(this.templates.DisabledTextBoxItem) {
                ASPx.GetStateController().AddDisabledItem(this.GetTextBoxCellID(),
                    this.templates.DisabledTextBoxItem.className, this.templates.DisabledTextBoxItem.cssText,
                    this.templates.DisabledTextBoxItem.postfixes, this.templates.DisabledTextBoxItem.imageUrls,
                    this.templates.DisabledTextBoxItem.imagePostfixes);
            }
        },
        CreateClearBoxDisabledState: function () {
            if(this.templates.DisabledClearBoxItem) {
                ASPx.GetStateController().AddDisabledItem(this.GetClearBoxCellId(),
                    this.templates.DisabledClearBoxItem.className, this.templates.DisabledClearBoxItem.cssText,
                    this.templates.DisabledClearBoxItem.postfixes, this.templates.DisabledClearBoxItem.imageObjs,
                    this.templates.DisabledClearBoxItem.imagePostfixes);
            }
        },
        CreateBrowseHoveredState: function () {
            if(this.templates.HoveredBrowseItem) {
                ASPx.GetStateController().AddHoverItem(this.GetBrowseButtonCellId(),
                    this.templates.HoveredBrowseItem.className, this.templates.HoveredBrowseItem.cssText,
                    this.templates.HoveredBrowseItem.postfixes, this.templates.HoveredBrowseItem.imageObjs,
                    this.templates.HoveredBrowseItem.imagePostfixes);
            }
        },
        CreateBrowsePressedState: function () {
            if(this.templates.PressedBrowseItem) {
                ASPx.GetStateController().AddPressedItem(this.GetBrowseButtonCellId(),
                    this.templates.PressedBrowseItem.className, this.templates.PressedBrowseItem.cssText,
                    this.templates.PressedBrowseItem.postfixes, this.templates.PressedBrowseItem.imageObjs,
                    this.templates.PressedBrowseItem.imagePostfixes);
            }
        },
        CreateBrowseDisabledState: function () {
            if(this.templates.DisabledBrowseItem) {
                ASPx.GetStateController().AddDisabledItem(this.GetBrowseButtonCellId(),
                    this.templates.DisabledBrowseItem.className, this.templates.DisabledBrowseItem.cssText,
                    this.templates.DisabledBrowseItem.postfixes, this.templates.DisabledBrowseItem.imageObjs,
                    this.templates.DisabledBrowseItem.imagePostfixes);
            }
        },
        getBaseFileName: function(filePath) {
            if(!ASPxClientUploadControl.IsValidWindowsFileName(filePath))
                return filePath;
            var windowsFileNameRegExp = new RegExp(windowsFileNameRegExpTemplate, "gi");
            return filePath.replace(windowsFileNameRegExp, '$2').replace('\\', '');
        },
        GetFileInfos: function() {
            var fileInfos = this.getFilesFromCache(),
                fileList = this.getFileList(),
                index = this.GetID(),
                fileCount;

            fileCount = fileList && fileList.length;

            if(fileCount && !this.options.enableMultiSelect)
                fileInfos = [];

            ASPx.Data.ForEach(fileList, function(fileInfo) {
                fileInfos.push(new ASPxFileInfo(fileInfo, index));
            }.aspxBind(this));

            fileInfos = this.ensureFileInputIndex(fileInfos);
            this.subsribeFileInfos(fileInfos);

            return fileInfos;
        },
        getFilesFromCache: function() {
            return this.fileInfos && this.fileInfos.length && this.fileInfos.slice() || [];
        },
        getFileList: function() {
            var fileInputElement = this.GetFileInputElement(),
                fileList = [];

            if(fileInputElement.value) {
                var fileName = this.getBaseFileName(fileInputElement.value);
                fileList.push({ name: fileName });
            }

            return fileList;
        },
        ensureFileInputIndex: function(fileInfos) {
            if(fileInfos) {
                ASPx.Data.ForEach(fileInfos, function(fileInfo) {
                    fileInfo.inputIndex = this.GetID();
                }.aspxBind(this))
            }

            return fileInfos;
        },
        subsribeFileInfos: function(fileInfos) {
            ASPx.Data.ForEach(fileInfos, function(fileInfo) {
                fileInfo.OnDispose.ClearHandlers();
                fileInfo.OnDispose.AddHandler(function() {
                    this.Refresh(ASPxEmptyRefreshArgs);
                }.aspxBind(this));
            }.aspxBind(this));
        },
        GetText: function(index) {
            return this.GetValue(index);
        },
        setText: function() { },
        setTooltip: function() { },
        GetValue: function(isShortName) {
            var value = this.GetFileNames(isShortName).join(', ');
            return value != '' ? value : null;
        },
        IsInputEmpty: function(index) {
            var value = this.GetFileNames(index || 0);
            return !value.length;
        },
        GetFileSelectorElement: function() {
            return this.GetFileInputElement();
        },
        GetErrorRow: function(id) {
            if(!this.errorRow)
                this.errorRow = this.domHelper.GetChildElement(this.GetErrorRowId(id));

            return this.errorRow;
        },
        GetErrorCell: function(row) {
            var row = this.GetErrorRow(),
                errorCell = null;
            if(row) {
                errorCell = ASPx.GetNodesByTagName(row, "TD")[0];
            }

            return errorCell;
        },
        GetErrorRowPrefix: function() {
            return IdSuffixes.Error.Row;
        },
        GetErrorRowId: function(id) {
            return this.GetErrorRowPrefix() + (id || this.GetID());
        },
        GetFileInputElement: function() {
            return this.GetElementFromCacheByClassName(CSSClasses.TextboxInput);
        },
        getFileInputTemplate: function() {
            return ASPx.GetNodesByPartialClassName(this.GetFileInputRowTemplate(), CSSClasses.TextboxInput)[0];
        },
        GetErrorRowTemplate: function() {
            return this.GetErrorRow("RT");
        },
        GetTextBoxCell: function() {
            return this.GetElementFromCacheByClassName(CSSClasses.Textbox);
        },
        GetTextBoxCellID: function(id) {
            var id = ASPx.IsExists(id) ? id : this.GetID();
            return this.GetName() + IdSuffixes.Input.TextBoxCell + id;
        },
        GetFileFakeInputElement: function() {
            return this.GetElementFromCacheByClassName(CSSClasses.TextboxFakeInput);
        },
        GetFileInputRowTemplate: function() {
            if(!this.fileInputRowTemplate) {
                var inputTemplate = this.GetInputRow("T");
                this.fileInputRowTemplate = ASPx.GetParentByTagName(inputTemplate, "TR");
            }
            return this.fileInputRowTemplate;
        },
        GetFileInputSeparatorRowTemplate: function() {
            if(this.options.fileInputSpacing === "")
                return null;

            return ASPx.GetNodesByPartialClassName(this.GetUploadInputsTable(), CSSClasses.SeparatorRow)[0];
        },
        GetBrowseButtonCell: function() {
            return this.GetElementFromCacheByClassName(CSSClasses.BrowseButtonCell);
        },
        GetFakeFocusInputElementID: function () {
            return this.GetName() + IdSuffixes.Input.FakeFocusInput;
        },
        GetFakeFocusInputElement: function () {
            return ASPx.GetInputElementById(this.GetFakeFocusInputElementID());
        },
        setDialogTriggerID: function(ids) {
            this.triggerElements = this.triggerElements || [];

            if(this.triggerElements.length)
                aspxGetUploadControlCollection().SubscribeDialogTriggers(this, this.GetName(), this.triggerElements, this.getDialogTriggerHandlers(), false);

            this.ensureTriggerElementsCache(ids.split(";"));

            if(this.triggerElements.length)
                aspxGetUploadControlCollection().SubscribeDialogTriggers(this, this.GetName(), this.triggerElements, this.getDialogTriggerHandlers(), true);

            this.ensureTriggerCursors();
        },
        ensureTriggerCursors: function() {
            ASPx.Data.ForEach(this.triggerElements, function(trigger) {
                this.triggerCursorsList[trigger.id] = this.getElementCursor(trigger);
            }.aspxBind(this));
        },
        ensureTriggerElementsCache: function(triggerIdList) {
            this.triggerElements = [];

            if(triggerIdList && triggerIdList.length && this.GetID() === 0) {
                ASPx.Data.ForEach(triggerIdList, function(triggerId) {
                    var triggerElement = document.getElementById(triggerId);
                    if(triggerElement && ASPx.Data.ArrayIndexOf(this.triggerElements, triggerElement) === -1)
                        this.triggerElements.push(triggerElement);
                }.aspxBind(this));
            }
        },
        getTriggerElements: function() {
            this.triggerElements = this.triggerElements || [];

            if(!this.triggerElements.length)
                this.ensureTriggerElementsCache(this.options.dialogTriggerIDList);

            return this.triggerElements;
        },
        InitializeTemplates: function() {
            if(this.options.fileInputSpacing != "") {
                this.fileInputSeparatorTemplateNode = this.GetFileInputSeparatorRowTemplate().cloneNode(true);
                ASPx.SetElementDisplay(this.fileInputSeparatorTemplateNode, true);
            }
        },
        IsFocusNeedReset: function() {
            return this.IsSlModeEnabled() ? !ASPx.Browser.IE : (ASPx.Browser.IE || ASPx.Browser.Opera);
        },
        ChangeEventsToFileInput: function(attach) {
            var method = this.ChangeEventsMethod(attach),
                index = this.GetID(),
                fileInput = this.GetRenderResult();

            this.AttachOnChangeHandler(fileInput, method, attach);
        },
        AttachOnChangeHandlerCore: function(fileInput, method, attach) {
            this.attachOnChangeHandlerForInput(method);
            aspxGetUploadControlCollection().SubscribeDialogTriggers(this, this.GetName(), this.getTriggerElements(), this.getDialogTriggerHandlers(), attach);
            this.ensureTriggerCursors();
        },
        attachOnChangeHandlerForInput: function(method) {
            method(this.GetFileInputElement(), "change", function() {
                if(!this.supressInputEvent)
                    this.RaiseStateChangedInternal(this);
            }.aspxBind(this));
        },
        getDialogTriggerHandlers: function() {
            var triggerHandlers = {};

            triggerHandlers.click = [this.createTriggerHandler(this.onTriggerClick)];

            if(!this.isSupportsInputClick()) {
                triggerHandlers.mousemove = [
                    this.createTriggerHandler(this.onTriggerMouseMove),
                    this.createTriggerHandler(this.onFileSelectorMouseMove, this.GetFileSelectorElement())
                ];
                triggerHandlers.mouseout = [
                    this.createTriggerHandler(this.OnFileInputMouseOut),
                    this.createTriggerHandler(this.onFileSelectorMouseOut, this.GetFileSelectorElement())
                ];
                triggerHandlers.mousedown = [this.createTriggerHandler(this.onFileSelectorMouseDown, this.GetFileSelectorElement())];
            }

            return triggerHandlers;
        },
        createTriggerHandler: function(handler, target) {
            return {
                handler: handler,
                target: target
            };
        },
        setFileInputPosition: function(e) {
            this.setFileInputPositionCore(e)
        },
        setFileInputPositionCore: function(e) {
            var space = 10,
                xPos = ASPx.Evt.GetEventX(e),
                yPos = ASPx.Evt.GetEventY(e),
                fileSelector = this.GetFileSelectorElement(),
                width = fileSelector.offsetWidth,
                height = fileSelector.offsetHeight;

            xPos -= this.IsRightToLeft() ? space : (width - space);
            yPos -= height / 2;

            ASPx.SetAbsoluteY(fileSelector, yPos);
            ASPx.SetAbsoluteX(fileSelector, xPos);
        },
        onFileSelectorMouseMove: function(e, trigger) {
            if(this.domHelper.IsMouseOverElement(e, trigger)) {
                ASPx.Evt.CancelBubble(e);
                this.reraiseEvent(e, "onmousemove", trigger);

                ASPx.RemoveClassNameFromElement(this.GetFileSelectorElement(), this.GetFileInputOnTextBoxHoverClassName());
                this.ensureCursorStyle(this.triggerCursorsList[trigger.id]);
            }
        },
        onFileSelectorMouseDown: function(e, trigger) {
            if(this.domHelper.IsMouseOverElement(e, trigger)) {
                ASPx.Evt.CancelBubble(e);
                this.reraiseEvent(e, "onmousedown", trigger);
            }
        },
        onFileSelectorMouseOut: function(e) {
            this.ensureCursorStyle("");
        },
        getElementCursor: function(element) {
            return ASPx.GetCurrentStyle(element).cursor;
        },
        ensureCursorStyle: function(cursor) {
            if(this.getElementCursor(this.GetFileSelectorElement()) !== cursor) {
                ASPx.SetStyles(this.GetFileSelectorElement(), {
                    cursor: cursor
                });
            }
        },
        onTriggerMouseMove: function(e) {
            if(this.enabled)
                this.setFileInputPosition(e, false, true);
        },
        isSupportsInputClick: function() {
            return !(ASPx.Browser.IE && ASPx.Browser.Version < 11);
        },
        onTriggerClick: function(e) {
            this.GetFileSelectorElement().click();
        },
        showFileChooserDialog: function() {
            var fileInput = this.GetFileSelectorElement();
            if(fileInput.click)
                fileInput.click();
        },
        OnUploadFilesComplete: function(args) {
            if(!args.uploadCancelled || this.options.autoStart)
                this.Clear();
        },
        OnDocumentMouseUp: function() {
            this.browseButtonPressed = false;
        },
        reraiseEvent: function(e, eventType, newTarget) {
            var evt;
            if(document.createEvent) {
                evt = document.createEvent("MouseEvents");
                evt.initMouseEvent(e.type, e.bubbles, e.cancelable, window, e.detail, e.screenX, e.screenY,
                    e.clientX, e.clientY, e.ctrlKey, e.altKey, e.shiftKey, e.metaKey, e.button, e.relatedTarget);
                evt.target = newTarget;

                newTarget.dispatchEvent(evt);
            } else if(document.createEventObject) {
                evt = document.createEventObject(window.event);
                evt.type = e.type;
                evt.bubbles = e.bubbles;
                evt.cancelable = e.cancelable;
                evt.view = window;
                evt.detail = e.detail;
                evt.screenX = e.screenX;
                evt.screenY = e.screenY;
                evt.clientX = e.clientX;
                evt.clientY = e.clientY;
                evt.ctrlKey = e.ctrlKey;
                evt.altKey = e.altKey;
                evt.shiftKey = e.shiftKey;
                evt.metaKey = e.metaKey;
                evt.button = e.button;
                evt.relatedTarget = e.relatedTarget;

                newTarget.fireEvent(eventType, evt);
            }
        },
        suppressFileDialog: function(suppress) {
            this.ChangeEventsToFileInput(!suppress);
        }
    });

    var ASPxInvisibleFileInputDecorator = ASPx.CreateClass(ASPxBaseView, {
        constructor: function(options, view) {
            this.view = new view(options);
            this.options = options;

            this.initializeEvents();
            this.replaceFunctions();
        },
        initializeEvents: function() {
            this.StateChangedInternal = new ASPxClientEvent();
            this.ErrorOccurred = new ASPxClientEvent();

            this.view.StateChangedInternal.AddHandler(function(view, args) {
                this.StateChangedInternal.FireEvent(this.view, args);
            }.aspxBind(this));
            this.view.ErrorOccurred.AddHandler(function(args) {
                this.ErrorOccurred.FireEvent(args);
            }.aspxBind(this));
        },
        replaceFunctions: function() {
            this.view.getFileInputTemplate = this.getFileInputTemplate;
            this.view.GetFileInputElement = this.GetFileInputElement;
            this.view.baseGetFileInfos = this.view.GetFileInfos.aspxBind(this.view);
            this.view.GetFileInfos = this.GetFileInfos.aspxBind(this.view);
            this.view.SetFileInputTooltip = this.SetFileInputTooltip.aspxBind(this.view);
            this.view.SetFileInputRowEnabled = this.SetFileInputRowEnabled.aspxBind(this);
            this.view.GetRenderResult = this.GetRenderResult.aspxBind(this);
            this.view.GetUploadInputsTable = this.GetUploadInputsTable.aspxBind(this);

            this.view.prepareFileInputRowTemplate = this.noop;
            this.view.UpdateNullText = this.noop;
            this.view.InitializeFakeFocusInputElement = this.noop;
            this.view.InitializeTemplates = this.noop;
            this.view.InitializeFileInputStyles = this.noop;
            this.view.FileInputGotFocus = this.noop;
            this.view.FileInputLostFocus = this.noop;
            this.view.changeTooltip = this.noop;

            this.view.GetBrowseButtonCell = this.returnNull;
            this.view.GetTextBoxCell = this.returnNull;
        },
        // view API
        SetFileInputRowEnabled: function() {
            this.view.enabled = true;
        },
        InvokeTextChangedInternal: function() {
            this.view.InvokeTextChangedInternal();
        },
        GetRenderResult: function() {
            return this.options.domHelper.GetMainElement();
        },
        Initialize: function() {
            this.view.Initialize.call(this.view);
        },
        InlineInitialize: function() {
            this.view.InlineInitialize.call(this.view);
        },
        Clear: function() {
            this.view.Clear.call(this.view);
        },
        GetText: function() {
            return this.view.GetText.call(this.view);
        },
        OnBeginProcessUploading: function() {
            this.view.OnBeginProcessUploading.call(this.view);
        },
        OnUploadFilesComplete: function(args) {
            this.view.OnUploadFilesComplete.call(this.view, args);
        },
        Refresh: function(args) {
            this.view.Refresh.call(this.view, args);
        },
        setDialogTriggerID: function(ids) {
            this.view.setDialogTriggerID.call(this.view, ids);
        },
        SetEnabled: function(enabled) {
            this.view.SetEnabled.call(this.view, enabled);
        },
        setInCallback: function(isInCallback) {
            this.view.setInCallback.call(this.view, isInCallback);
        },
        GetNextFocusElement: function() {
            return null;
        },
        // Internal
        noop: function() { },
        returnNull: function() {
            return null;
        },
        getFileInputTemplate: function() {
            return ASPx.GetNodesByPartialClassName(this.GetRenderResult(), CSSClasses.TextboxInput)[0];
        },
        GetFileInputElement: function() {
            return ASPx.GetNodesByPartialClassName(this.GetRenderResult(), CSSClasses.TextboxInput)[1];
        },
        GetFileInfos: function() {
            return [this.baseGetFileInfos()];
        },
        SetFileInputTooltip: function(text) {
            var handler = (text != '') ? ASPx.Attr.SetAttribute : ASPx.Attr.RemoveAttribute;

            ASPx.Data.ForEach(this.getTriggerElements(), function(trigger) {
                handler(trigger, "title", text);
            });
            handler(this.GetFileSelectorElement(), "title", text);
        },
        GetUploadInputsTable: function() {
            return this.GetRenderResult();
        },
        suppressFileDialog: function(suppress) {
            this.view.suppressFileDialog.call(this.view,  suppress);
        }
    });

    var ASPxStandardInputView = ASPx.CreateClass(ASPxBaseInputView, {
        constructor: function(options) {
            this.constructor.prototype.constructor.call(this, options);
            this.fileInputIsHidden = true;
            this.selectedSeveralFilesText = options.selectedSeveralFilesText;
            this.nullTextItem = options.nullTextItem;
            this.accessibilityCompliant = options.accessibilityCompliant;
        },
        InlineInitialize: function(options) {
            ASPxBaseView.prototype.InlineInitialize.call(this, options);
            this.UpdateNullText();
        },
        Initialize: function() {
            ASPxBaseInputView.prototype.Initialize.call(this, this.options);
            this.InitializeFileInputStyles();
        },
        InitializeFileInputStyles: function() {
            var styleSheet = ASPx.GetCurrentStyleSheet();
            ASPx.AddStyleSheetRule(styleSheet,
                                    " ." + this.GetFileInputOnTextBoxHoverClassName(),
                                    "cursor: " + ASPx.GetCurrentStyle(this.GetTextBoxCell())["cursor"] + ";");
            ASPx.AddStyleSheetRule(styleSheet,
                                    " ." + this.GetFileInputOnBrowseButtonHoverClassName(),
                                    "cursor: " + ASPx.GetCurrentStyle(this.GetBrowseButtonCell())["cursor"] + ";");
        },
        GetFileInputOnTextBoxHoverClassName: function() {
            return this.GetName() + CSSClasses.FITextBoxHoverDocument;
        },
        GetFileInputOnBrowseButtonHoverClassName: function() {
            return this.GetName() + CSSClasses.FIButtonHoverDocument;
        },
        GetClearBoxCell: function() {
            return this.GetElementFromCacheByClassName(CSSClasses.ClearButtonCell);
        },
        RedefineInputRowAttributes: function() {
            ASPxBaseInputView.prototype.RedefineInputRowAttributes.call(this);

            this.redefineClearBoxAttributes();
            this.GetBrowseButtonCell().id = this.GetBrowseButtonCellId();
        },
        redefineClearBoxAttributes: function() {
            var clearBox = this.GetClearBoxCell(),
                clearBoxImg = ASPx.GetNodeByTagName(clearBox, "IMG", 0),
                clearBoxId = this.GetClearBoxCellId(),
                clearBoxImgId = clearBoxId + IdSuffixes.Input.ButtonCell.ClearImg;

            if(clearBox) {
                clearBox.id = clearBoxId;
                
                if(clearBoxImg)
                    clearBoxImg.id = clearBoxImgId;
            }
        },
        RedefineInputAttributes: function() {
            ASPxBaseInputView.prototype.RedefineInputAttributes.call(this);

            var fakeInputElement = this.GetFileFakeInputElement();
            if(fakeInputElement)
                fakeInputElement.id = this.GetFileFakeInputElementId();
        },
        GetClearBoxCellId: function() {
            return this.GetName() + IdSuffixes.Input.ButtonCell.Clear + this.GetID();
        },
        GetBrowseButtonCellId: function() {
            return this.GetName() + IdSuffixes.Input.ButtonCell.Browse + this.GetID();
        },
        SetFileInputRowEnabled: function(enabled) {
            ASPxBaseInputView.prototype.SetFileInputRowEnabled.call(this, enabled);

            this.SetClearBoxEnabled(this.GetClearBoxCell(), enabled);
            this.SetButtonEnabled(this.GetBrowseButtonCell(), enabled);
        },
        GetFileFakeInputElementId: function() {
            return this.GetTextBoxCellID() + IdSuffixes.Input.FileFakeInput;
        },
        SetFileInputTooltip: function(text) {
            var handler = (text != '') ? ASPx.Attr.SetAttribute : ASPx.Attr.RemoveAttribute,
                isTextBoxHidden = this.isTextBoxHidden(),
                setTitleForSelector = ASPx.Browser.Firefox && !this.IsSlModeEnabled() || ASPx.Browser.WebKitFamily && isTextBoxHidden,
                element = isTextBoxHidden ? this.GetBrowseButtonCell() : this.GetTextBoxCell();

            handler(element, "title", text);
            if(setTitleForSelector && !this.accessibilityCompliant)
                handler(this.GetFileSelectorElement(), "title", text);
        },
        getFileInputTooltipText: function(files) {
            var value = '';

            if((typeof files == "object") && (files instanceof Array)) {
                var isNewlineSupported = ASPx.Browser.IE || ASPx.Browser.WebKitFamily || (ASPx.Browser.Firefox && !this.IsSlModeEnabled());
                if(isNewlineSupported && files.length > 1) {
                    var i = 0;
                    while(i < files.length) {
                        if(i > 0)
                            value += '\n';
                        value += ASPx.Str.Trim(files[i++] || "");
                    }
                }
                else
                    value = files.join(', ');
            }

            return value;
        },
        ShowClearButton: function(show) {
            var clearBoxCell = this.GetClearBoxCell();
            if(clearBoxCell) {
                var link = ASPx.GetNodeByTagName(clearBoxCell, "A", 0);
                var func = show ? ASPx.Attr.RemoveAttribute : ASPx.Attr.SetAttribute;
                func(link.style, "visibility", "hidden");
            }
        },
        SetNullTextEnabled: function (enabled) {
            if(this.nullText != null) {
                if(enabled)
                    this.SetFileFakeInputElementValue(this.nullText);

                this.ChangeTextBoxNullTextState(this.GetTextBoxCell(), enabled);
                this.ChangeClearBoxNullTextState(this.GetClearBoxCell(), enabled);
            }
        },
        ChangeTextBoxNullTextState: function (element, enabled) {
            if(element && (this.nullText || this.customText) && this.nullTextItem) {
                var restore = !enabled,
                    styleAttrName = 'style';

                ASPx.Attr.ChangeAttributesMethod(restore)(element, 'class');
                ASPx.Attr.ChangeAttributesMethod(restore)(element, styleAttrName);

                var inputRow = null;
                if(this.nullTextItem.inputRow) {
                    inputRow = this.GetInputRow();
                    ASPx.Attr.ChangeAttributesMethod(restore)(inputRow, styleAttrName);
                }

                var editArea = this.GetFileFakeInputElement();
                if(editArea)
                    ASPx.Attr.ChangeAttributesMethod(restore)(editArea, styleAttrName);

                if(enabled) {
                    element.className = this.nullTextItem.textBox.className;
                    element.style.cssText = this.nullTextItem.textBox.cssText;
                    if(editArea)
                        editArea.style.cssText = this.nullTextItem.editArea.cssText;

                    if(this.nullTextItem.inputRow)
                        inputRow.style.cssText = this.nullTextItem.inputRow.cssText;
                }
            }
        },
        ChangeClearBoxNullTextState: function (element, enabled) {
            if(element && this.nullText != null && this.nullTextItem) {
                var restore = !enabled;

                ASPx.Attr.ChangeAttributesMethod(restore)(element, 'style');
                ASPx.Attr.ChangeAttributesMethod(restore)(element, 'class');

                if(enabled) {
                    element.className = this.nullTextItem.clearBox.className;
                    element.style.cssText = this.nullTextItem.clearBox.cssText;
                }
            }
        },
        CreateSelectedSeveralFilesText: function(files) {
            var text = "",
                filteredFiles = [];

            if(files.length) {
                ASPx.Data.ForEach(files, function(file) {
                    if(file !== undefined) {
                        filteredFiles.push(file);
                    }
                });
                files = filteredFiles;

                if(files.length > 1)
                    text = this.selectedSeveralFilesText.replace("{0}", files.length);
                else if(files.length === 1)
                    text = files[0];
            }

            return text;
        },
        SetFileFakeInputElementValue: function(value) {
            var element = this.GetFileFakeInputElement();
            if(element)
                element.value = value;
        },
        refreshBase: function(args) {
            ASPxBaseInputView.prototype.refreshBase.call(this, args);
            this.RefreshInput(args.skipRefreshInput);
        },
        RefreshInput: function(skipRefreshInput) {
            var files = this.GetFileNames(true),
                fakeInputValue = this.CreateSelectedSeveralFilesText(files),
                inputTooltip = skipRefreshInput ? fakeInputValue : this.getFileInputTooltipText(files);

            this.SetFileInputTooltip(inputTooltip);

            if(!skipRefreshInput) {
                this.SetFileFakeInputElementValue(fakeInputValue);
                this.UpdateNullText();
                this.ShowClearButton(fakeInputValue && fakeInputValue !== "");
            }
        },
        UpdateNullText: function() {
            var isEmpty = this.IsInputEmpty();
            if(this.nullText != null)
                this.SetNullTextEnabled(isEmpty);
            else if(isEmpty)
                this.SetFileFakeInputElementValue("");
        },
        AttachOnChangeHandler: function(fileInput, method, attach) {
            var textBoxCell = this.GetTextBoxCell(),
                clearBoxCell = this.GetClearBoxCell(),
                browseButton = this.GetBrowseButtonCell(),
                fileSelectorElement = this.GetFileSelectorElement();

            if(textBoxCell) {
                method(textBoxCell, "mousemove", this.OnFileInputMouseMove.aspxBind(this));
                method(textBoxCell, "mouseout", this.OnFileInputMouseOut.aspxBind(this));
                method(textBoxCell, "click", this.OnBrowseButtonClick.aspxBind(this));
            }

            method(fileInput, "mouseout", this.OnBrowseButtonMouseOut.aspxBind(this));
            method(fileInput, "mousemove", this.OnFileInputMouseMove.aspxBind(this));
            method(fileInput, "mouseout", this.OnFileInputMouseOut.aspxBind(this));
            method(fileInput, "mousedown", this.OnFileInputMouseDown.aspxBind(this));

            method(fileSelectorElement, "focus", this.FileInputGotFocus.aspxBind(this));
            method(fileSelectorElement, "blur", this.FileInputLostFocus.aspxBind(this));

            if(this.IsFocusNeedReset())
                method(fileSelectorElement, "keydown", this.raiseFocusNeedResetInternal.aspxBind(this));

            method(fileSelectorElement, "mousemove", this.OnFileInputMouseMove.aspxBind(this));

            if(browseButton) {
                this.buttonEventHandlers[browseButton.id] = this.OnBrowseButtonClick.aspxBind(this);
                this.attachButtonHandler(browseButton, attach);
            }

            if(clearBoxCell) {
                this.buttonEventHandlers[clearBoxCell.id] = function() {
                    if(this.isInCallback) return;

                    this.Clear();
                    this.clearErrors();
                }.aspxBind(this);

                this.attachButtonHandler(clearBoxCell, attach);
            }

            this.AttachOnChangeHandlerCore(fileInput, method, attach);
        },
        raiseFocusNeedResetInternal: function(e) {
            if(ASPx.Evt.GetKeyCode(e) === ASPx.Key.Tab) {
                var args = {
                    backward: e.shiftKey,
                    index: this.GetID(),
                    event: e
                };

                this.FocusNeedResetInternal.FireEvent(this, args);
            }
        },
        GetNextFocusElement: function(args) {
            var element = null;
            if(this.GetText() && !args.backward)
                element = this.GetClearBoxCell() && this.GetClearBoxCell().childNodes[0];

            return element;
        },
        IsMouseOverTextBox: function(evt) {
            return this.domHelper.IsMouseOverElement(evt, this.GetTextBoxCell());
        },
        IsMouseOverBrowseButton: function(evt) {
            return this.domHelper.IsMouseOverElement(evt, this.GetBrowseButtonCell());
        },
        isOverTriggerElement: function(evt) {
            var triggerElements = this.getTriggerElements();

            for(var i = 0; i < triggerElements.length; i++)
                if(this.domHelper.IsMouseOverElement(evt, triggerElements[i]))
                    return true;

            return false;
        },
        isTextBoxHidden: function() {
            return this.GetFileFakeInputElement() == null;
        },
        FileInputGotFocus: function(evt) {
            var button = this.GetBrowseButtonCell();
            var focusedClassName = " " + CSSClasses.BrowseButtonFocus;
            button.className += focusedClassName;
            if(ASPx.Browser.Opera) {
                if(this._operaFocusedFlag)
                    this._operaFocusedFlag = false;
                else {
                    this._operaFocusedFlag = true;
                    this.GetFakeFocusInputElement().focus();

                    var _this = this;
                    window.setTimeout(function() {
                        _this.GetFileInputElement().focus();
                    }, 100);
                }
            }
        },
        NeedMouseClickCorrection: function() {
            return !ASPx.Browser.TouchUI && this.fileInputIsHidden;
        },
        FileInputLostFocus: function(evt) {
            var button = this.GetBrowseButtonCell();
            var focusedClassName = " " + CSSClasses.BrowseButtonFocus;
            var className = button.className;
            while(className.indexOf(focusedClassName) != -1)
                className = className.replace(focusedClassName, "");
            button.className = className;
        },
        OnBrowseButtonClick: function(evt) {
            this.OnClickInFakeElement(evt);
        },
        OnBrowseButtonMouseOut: function(e) {
            this.OnFileInputMouseOut(e);
            this.ChangeButtonHoveredState(this.GetBrowseButtonCell(), false);
        },
        OnFileInputMouseMove: function(evt) {
            this.OnMouseMoveInFileInputElement(evt);
        },
        OnFileInputMouseOut: function(e) {
            if(!(this.IsMouseOverBrowseButton(e) || this.IsMouseOverTextBox(e) || this.isOverTriggerElement(e))) {
                this.ResetFileInputPosition();
            }
            this.StopEventPropagation(e);
        },
        OnFileInputMouseDown: function(evt) {
            var isOverBrowseButton = this.IsMouseOverBrowseButton(evt);
            this.browseButtonPressed = true;
            this.ChangeButtonPressedState(this.GetBrowseButtonCell(), isOverBrowseButton);
        },
        OnMouseMoveInFakeElement: function(evt) {
            if(this.enabled) {
                var isOverBrowseButton = this.IsMouseOverBrowseButton(evt);
                this.setFileInputPosition(evt, isOverBrowseButton);
                var browseButtonCell = this.GetBrowseButtonCell();
                if(this.browseButtonPressed)
                    this.ChangeButtonPressedState(browseButtonCell, isOverBrowseButton);
                else
                    this.ChangeButtonHoveredState(browseButtonCell, isOverBrowseButton);
            }
        },
        OnMouseMoveInFileInputElement: function(evt) {
            if(this.enabled) {
                var isOverBrowseButton = this.IsMouseOverBrowseButton(evt),
                    isOverTextBox = this.IsMouseOverTextBox(evt),
                    isOverTriggerElement = this.isOverTriggerElement(evt),
                    browseButtonCell = this.GetBrowseButtonCell();

                if(isOverTextBox || isOverBrowseButton || isOverTriggerElement) {
                    this.setFileInputPosition(evt, isOverBrowseButton, isOverTriggerElement);

                    if(this.browseButtonPressed)
                        this.ChangeButtonPressedState(browseButtonCell, isOverBrowseButton);
                    else if(!isOverTriggerElement)
                        this.ChangeButtonHoveredState(browseButtonCell, isOverBrowseButton);

                    this.changeTooltip(isOverTextBox);
                }
                else
                    this.ResetFileInputPosition();
            }
        },
        OnClickInFakeElement: function(evt) {
            if(!this.NeedMouseClickCorrection()) return;

            this.OnMouseMoveInFakeElement(evt);
            this.showFileChooserDialog();
        },
        ChangeButtonHoveredState: function(element, enabled) {
            if(element && !this.browseButtonPressed) {
                element = enabled ? ASPx.GetStateController().GetHoverElement(element) : null;
                ASPx.GetStateController().SetCurrentHoverElement(element);
            }
        },
        ChangeButtonPressedState: function(element, enabled) {
            if(element) {
                var controller = ASPx.GetStateController();
                var pressedElement = controller.GetPressedElement(element);
                controller.SetPressedElement(enabled ? pressedElement : null);
            }
        },
        changeTooltip: function() { },
        setFileInputPosition: function(e, isChooseButton, isOverTrigger) {
            this.setFileInputPositionCore(e);

            this.SetFileInputCursor(isChooseButton, isOverTrigger);
            this.fileInputIsHidden = false;
        },
        SetFileInputCursor: function(isChooseButton, isOverTrigger) {
            var fileSelectorElement = this.GetFileSelectorElement(),
                initialClassName = fileSelectorElement.className,
                textboxHoverClass = this.GetFileInputOnTextBoxHoverClassName(),
                browseHoverClass = this.GetFileInputOnBrowseButtonHoverClassName(),
                hoverClassName = isChooseButton ? browseHoverClass : textboxHoverClass;

			if(isOverTrigger)
                hoverClassName = "";

            var newClassName = initialClassName
				.replace(textboxHoverClass, "")
                .replace(browseHoverClass, "")
                .replace(/^\s+|\s+$/g, '')     //trim
                .concat(" ", hoverClassName);

            if(initialClassName !== newClassName)
                fileSelectorElement.className = newClassName;
        },
        ResetFileInputPosition: function() {
            this.GetFileSelectorElement().style.top = '-5000px';
            this.fileInputIsHidden = true;
        },
        setText: function(text) {
            this.customText = text;
            this.SetFileFakeInputElementValue(text);
            this.ChangeTextBoxNullTextState(this.GetTextBoxCell(), false);
        },
        setTooltip: function(text) {
            this.SetFileInputTooltip(text);
        }
    });

    var ASPxMultiFileInputView = ASPx.CreateClass(ASPxCompositeView, {
        constructor: function(options, viewPrototype) {
            this.internalCount = options.fileInputCount;
            this.viewPrototype = viewPrototype;
            this.activeInputIndex = -1;

            this.constructor.prototype.constructor.call(this, options);
            this.FileInputCountChangedInternal = new ASPxClientEvent();
            this.FocusNeedResetInternal = new ASPxClientEvent();
        },
        GetRenderResult: function() {
            return this.GetUploadInputsTable();
        },
        raiseFileInputCountChanged: function() {
            var args = new ASPxClientEventArgs();
            this.FileInputCountChangedInternal.FireEvent(this, args);
        },
        Clear: function(index) {
            ASPx.Data.ForEach(this.views, function(view) {
                if(index === view.GetID() || index === undefined)
                    view.Clear();
            });
        },
        AdjustSize: function() {
            ASPx.Data.ForEach(this.views, function(view) {
                if(view.AdjustSize)
                    view.AdjustSize();
            });
        },
        getViewRefreshArgs: function(view, commonArgs) {
            var args = ASPxEmptyRefreshArgs,
                fileInfos = [],
                viewIndex = view.GetID(),
                isStateChanged = commonArgs.isStateChanged && commonArgs.inputIndex === viewIndex;

            fileInfos[viewIndex] = this.fileInfos[viewIndex] || [];

            if(fileInfos.length) {
                args = {
                    fileInfos: fileInfos,
                    isStateChanged: isStateChanged,
                    skipRefreshInput: commonArgs.skipRefreshInput
                };
            }

            return args;
        },
        addView: function(fileInfo) {
            ASPxCompositeView.prototype.addView.call(this, fileInfo);

            this.views[this.internalIndex - 1].FocusNeedResetInternal.AddHandler(this.raiseFocusNeedResetInternal, this);
        },
        onInternalStateChanged: function(_, args) {
            this.activeInputIndex = args.inputIndex;

            ASPxCompositeView.prototype.onInternalStateChanged.call(this);
        },
        prepareInternalStateChangedArgs: function(view) {
            var args = ASPxCompositeView.prototype.prepareInternalStateChangedArgs.call(this, view);
            args.inputIndex = this.activeInputIndex;
            this.activeInputIndex = -1;

            return args;
        },
        addFileInput: function(supressCountChanged) {
            if(this.options.maxFileCount && this.internalCount >= this.options.maxFileCount)
                return;

            this.addView();

            this.internalCount++;
            this.updateInternalInputCountField(this.internalCount);

            if(this.internalCount === 1)
                this.showSeparatorRow(true);

            if(!supressCountChanged)
                this.raiseFileInputCountChanged();
        },
        raiseFocusNeedResetInternal: function(view, args) {
            this.FocusNeedResetInternal.FireEvent(this, args);
        },
        GetNextFocusElement: function(args) {
            var element = this.views[args.index].GetNextFocusElement(args),
                inputIndex = args.index + (args.backward ? -1 : 1),
                removeButtonIndex = args.backward ? inputIndex : args.index;

            if(this.IsSlModeEnabled()) {
                element = this.views[inputIndex] && this.views[inputIndex].GetFileSelectorElement();
                if(!element && inputIndex > this.internalCount - 1)
                    element = this.views[args.index].GetFakeFocusInputElement();
            }

            if(!element) {
                element = this.removeButtons[removeButtonIndex] && this.removeButtons[removeButtonIndex].GetLink();
                if(!element)
                    element = this.views[inputIndex] && this.views[inputIndex].GetFileSelectorElement();
            }

            return element;
        },
        updateInternalInputCountField: function(count) {
            this.domHelper.stateObject.inputCount = count;
        },
        removeFileInput: function(index) {
            this.removeView(index);
        },
        removeView: function(index) {
            if(this.views[index]) {
                var separatorsCount = ASPx.GetNodesByPartialClassName(this.domHelper.GetMainElement(), CSSClasses.SeparatorRow).length;
                if(separatorsCount > 1)
                    ASPx.RemoveElement(ASPx.GetPreviousSibling(this.views[index].GetRenderResult())); // separator

                ASPxCompositeView.prototype.removeView.call(this, index);

                this.updateInternalInputCountField(this.internalCount);
                this.raiseFileInputCountChanged();
            }

            if(this.internalCount === 0)
                this.showSeparatorRow(false);
        },
        disposeFileInfo: function(index) {
            if(this.fileInfos && this.fileInfos[index] && this.fileInfos[index].length) {
                ASPx.Data.ForEach(this.fileInfos[index], function(fileInfo) {
                    fileInfo.dispose();
                });
            }
        },
        showSeparatorRow: function(show) {
            var separatorRow = this.domHelper.GetAddUploadButtonsSeparatorRow();
            separatorRow && ASPx.SetElementDisplay(separatorRow, show);
        },
        setFileInputCount: function(count) {
            var currentCount = this.internalCount;
            if(count > currentCount) {
                for(var i = currentCount; i < count; i++)
                    this.addFileInput(true);
            } else {
                for(var i = currentCount; i >= count; i--)
                    this.removeFileInput(i);
            }
        },
        setDialogTriggerID: function(ids) {
            if(this.views && this.views[0])
                this.views[0].setDialogTriggerID(ids);
        },
        setInCallback: function(isInCallback) {
            ASPxCompositeView.prototype.setInCallback.call(this, isInCallback);

            ASPx.Data.ForEach(this.views, function(view) {
                view.setInCallback(isInCallback);
            });
        },
        showError: function(error) {
            for(var i = 0; i < this.internalCount; i++) {
                if(i === error.inputIndex)
                    this.views[i].showError(error);
            }
        },
        GetText: function(index) {
            return this.views[index].GetText();
        },
        setText: function(text, index) {
            if(this.views && this.views[index])
                this.views[index].setText(text);
        },
        setTooltip: function(text, index) {
            if(this.views && this.views[index])
                this.views[index].setTooltip(text);
        },
        GetFileInputElement: function(index) {
            var view = this.views[index || 0] || {};
            return view.GetFileInputElement.call(view);
        },
        GetFileInputRowTemplate: function(index) {
            var view = this.views[index || 0] || {};
            return view.GetFileInputRowTemplate.call(view);
        },
        GetErrorRow: function(index) {
            var view = this.views[index || 0] || {};
            return view.GetErrorRow.call(view, index);
        },
        GetErrorRowTemplate: function() {
            var view = this.views[0] || {};
            return view.GetErrorRowTemplate.call(view);
        },
        GetInputRow: function(index) {
            var view = this.views[index || 0] || {};
            return view.GetInputRow.call(view);
        },
        GetErrorCell: function(index) {
            var view = this.views[index || 0] || {};
            return view.GetErrorCell.call(view);
        },
        GetAddUploadButtonsSeparatorRow: function() {
            return this.GetChildElement(IdSuffixes.Input.AddButtonsSeparator);
        },
        GetAddUploadButtonsPanelRow: function() {
            return this.GetChildElement(IdSuffixes.Input.AddUploadButtonPanelRow);
        },
        InvokeTextChangedInternal: function(index) {
            if(this.views[index]) {
                this.views[index].InvokeTextChangedInternal();
            }
        },
        OnDocumentMouseUp: function() {
            ASPx.Data.ForEach(this.views, function(view) {
                view.OnDocumentMouseUp();
            });
        },
        suppressFileDialog: function(suppress) {
            ASPx.Data.ForEach(this.views, function(view) {
                view.suppressFileDialog(suppress);
            })
        }
    });

    var ASPxAdvancedInputView = ASPx.CreateClass(ASPxStandardInputView, {
        constructor: function(options) {
            this.constructor.prototype.constructor.call(this, options);
        },
        Initialize: function() {
            ASPxStandardInputView.prototype.Initialize.call(this, this.options);

            if(ASPx.Browser.IE)
                this.clearInputValue();
        },
        RaiseStateChangedInternal: function(view) {
            ASPxStandardInputView.prototype.RaiseStateChangedInternal.call(this, view);
            this.clearInputValue();
        },
        refreshBase: function(args) {
            ASPxStandardInputView.prototype.refreshBase.call(this, args);
            this.clearInputValue();
        },
        clearInputValue: function() {
            this.supressInputEvent = true;

            if(ASPx.Browser.IE)
                this.replaceFileInputElement();
            else
                this.GetFileInputElement(0).value = "";

            this.supressInputEvent = false;
        },
        getFilesFromCache: function() {
            var files = [];
            if(this.options.enableMultiSelect || !this.GetFileInputElement().files.length)
                files = ASPxStandardInputView.prototype.getFilesFromCache.call(this);

            return files;
        },
        getFileList: function() {
            return this.GetFileInputElement().files || [];
        },
        subsribeFileInfos: function() {}
    });

    var ASPxSLInputView = ASPx.CreateClass(ASPxAdvancedInputView, {
        constructor: function(options) {
            this.constructor.prototype.constructor.call(this, options);
            this.slUploadHelperUrl = options.slUploadHelperUrl;
            this.domHelper.GetSlUploadHelperElement = this.GetSlUploadHelperElement.aspxBind(this);
        },
        Initialize: function(options) {
            this.initSlObject();
            ASPxAdvancedInputView.prototype.Initialize.call(this, options);
            this.SetEnabled(false);
        },
        initSlObject: function() {
            if(!this.SlInitialized && (!this.domHelper.IsSlObjectLoaded() || !this.GetRenderResult())) {
                this.createSlHost();
                this.initInput();
                this.SlInitialized = true;
            }
        },
        Render: function() {
            ASPxAdvancedInputView.prototype.Render.call(this);
            this.initSlObject();
            this.SetEnabled(false);
        },
        Dispose: function() {
            ASPx.RemoveElement(this.GetFileSelectorElement());

            ASPxAdvancedInputView.prototype.Dispose.call(this);
        },
        UpdateIndex: function(newIndex) {
            this.cachedSlHelperElement = this.GetSlUploadHelperElement();

            ASPxAdvancedInputView.prototype.UpdateIndex.call(this, newIndex);
        },
        SetEnabled: function(enabled) {
            this.SetFileInputRowEnabled(enabled);
        },
        initInput: function() {
            var slHelper = this.CreateSlObject(this.slUploadHelperUrl);

            if(this.GetUploadHostElement())
                this.GetUploadHostElement().appendChild(slHelper);
            else
                this.GetFileInputElement().parentNode.insertBefore(slHelper, this.GetFileInputElement());

            this.GetFileInputElement().parentNode.removeChild(this.GetFileInputElement());

            ASPx.SetStyles(slHelper, {
                "zIndex": Constants.INPUT_ZINDEX
            });
        },
        createSlHost: function() {
            if(!this.GetUploadHostElement()) {
                var slHost = document.createElement("DIV");
                var mainCell = this.GetUploadInputsTable().parentNode;

                ASPx.Attr.SetAttribute(slHost, "id", this.GetHostElementId());

                ASPx.Attr.SetAttribute(slHost.style, "position", "absolute");
                ASPx.Attr.SetAttribute(slHost.style, "width", "0px");
                ASPx.Attr.SetAttribute(slHost.style, "height", "0px");
                ASPx.Attr.SetAttribute(slHost.style, "border-width", "0px");

                mainCell.appendChild(slHost);
            }
        },
        RedefineInputAttributes: function() {
            ASPxAdvancedInputView.prototype.RedefineInputAttributes.call(this);

            this.redefineSlObjectAttributes(this.cachedSlHelperElement);
        },
        redefineSlObjectAttributes: function(slElement) {
            if(this.GetUploadHostElement())
                this.RedefineSlObjectAttributesInHostElement(slElement);
            else
                this.RedefineSlObjectAttributes(slElement);
        },
        RedefineSlObjectAttributesInHostElement: function (slElement) {
            var slElement = slElement || this.GetSlUploadHelperElement();
            if(slElement) {
                slElement.id = this.GetSlUploadHelperElementID();
                if(this.domHelper.IsSlObjectLoaded(this.GetID()))
                    slElement.content.sl.RedefineAttributes(this.baseName, this.GetID());
            }
        },
        clearInputValue: function() { },
        changeTooltip: function(isOverTextBox) {
            var tooltipElement = isOverTextBox ? this.GetTextBoxCell() : this.GetBrowseButtonCell();
            var tooltip = ASPx.Attr.GetAttribute(tooltipElement, "title");
            ASPx.Attr.SetAttribute(this.GetFileSelectorElement(), "title", tooltip ? tooltip : "");
        },
        renewFileInfosSubscribtion: function(fileInfos) {
            if(this.domHelper.IsSlObjectLoaded(this.GetID())) {
                var slElement = this.GetSlUploadHelperElement(),
                    fileInfosArray = fileInfos || this.fileInfos || [];

                ASPx.Data.ForEach(fileInfosArray, function(fileInfo, index) {
                    fileInfo.OnDispose.ClearHandlers();
                    fileInfo.OnDispose.AddHandler(function() {
                        slElement.content.sl.DisposeFileInfo(index);
                    });
                }.aspxBind(this));
            }
        },
        GetNextFocusElement: function() {
            return null;
        },
        RedefineSlObjectAttributes: function (slElement) {
            var slElement = slElement || ASPx.GetNodeByTagName(this.GetTextBoxCell(), "OBJECT", 0),
                inputIndex = this.GetID();

            if(slElement) {
                var slObjectId = this.GetSlUploadHelperElementID(inputIndex);
                var controlName = this.baseName;
                slElement.id = slObjectId;

                if(this.domHelper.IsSlObjectLoaded(inputIndex))
                    slElement.content.sl.RedefineAttributes(controlName, inputIndex);
            }
        },
        GetUploadHostElement: function() {
            if(!this.slUploadHostElement)
                this.slUploadHostElement = document.getElementById(this.GetSlUploadHostElementID());

            return this.slUploadHostElement;
        },
        GetSlUploadHostElementID: function () {
            return this.GetName() + IdSuffixes.SL.UploadHost;
        },
        InlineInitialize: function(options) {
            ASPxAdvancedInputView.prototype.InlineInitialize.call(this, options);
            if(!ASPx.Browser.Opera)
                this.SetFileInputRowEnabled(false);
        },
        CreateSlObject: function(source) {
            var inputIndex = this.GetID();
            var slObjectId = this.GetUploadHelperElementID(inputIndex);
            var controlName = this.GetName();

            var properties = { width: '70px', height: '22px' };
            var events = {};
            events.onLoad = 'slOnLoad_' + slObjectId;
            window[events.onLoad] = function() {
                ASPx.SLOnLoad(this.GetName(), inputIndex);
            }.aspxBind(this);

            events.onError = 'slOnError_' + slObjectId;
            window[events.onError] = function() {
                ASPx.SLOnError(this.GetName(), inputIndex);
            }.aspxBind(this);


            var parentElement = document.createElement("DIV");
            parentElement.innerHTML = this.BuildHTML(source, slObjectId, controlName, inputIndex, properties, events);

            return parentElement.firstChild;
        },
        BuildHTML: function (source, id, controlName, inputIndex, properties, events) {
            var sb = [];
            sb.push('<object type="application/x-silverlight-2" data="data:application/x-silverlight-2,"');
            sb.push(' id="' + id + '"');
            if(properties.width != null) sb.push(' width="' + properties.width + '"');
            if(properties.height != null) sb.push(' height="' + properties.height + '"');

            var opacityStyle = "";
            if(!ASPx.Browser.IE)
                opacityStyle = "opacity: 0.01;";
            sb.push(' style="position: absolute; background-color: transparent; top: -5000px; ' + opacityStyle + '"');
            sb.push('>');

            sb.push('<param name="source" value="' + source + '" />');
            sb.push('<param name="background" value="Transparent" />');
            sb.push('<param name="windowless" value="true" />');
            sb.push('<param name="minRuntimeVersion" value="3.0.40818.0" />');

            var init = '<param name="initParams" value="';
            init += 'controlName=' + controlName + ', ';
            init += 'inputIndex=' + inputIndex + ', ';
            init += 'multiselect=' + this.options.enableMultiSelect + ', ';
            init += 'allowedMaxFileSize=' + this.options.validationSettings.maxFileSize;
            var allowedFileExtensions = this.options.validationSettings.allowedFileExtensions;
            if(allowedFileExtensions != null) {
                init += ', allowedFileExtensions=' + allowedFileExtensions.join(';') + ', ';
                var fileMasks = [];
                for(var i = 0; i < allowedFileExtensions.length; i++)
                    fileMasks.push("*" + allowedFileExtensions[i]);
                init += 'filter=' + fileMasks.join(';');
            }
            init += '" />';

            sb.push(init);

            if(events.onLoad)
                sb.push('<param name="onLoad" value="' + events.onLoad + '" />');
            if(events.onError)
                sb.push('<param name="onError" value="' + events.onError + '" />');

            sb.push("</object>");

            return sb.join("");
        },
        InvokeTextChangedInternal: function() {
            this.changed = true;
            this.RaiseStateChangedInternal(this);
            this.changed = false;
        },
        GetFileInfos: function() {
            var slElement,
                slFileInfos,
                fileInfo,
                inputIndex = this.GetID(),
                currentFilesLength = this.fileInfos && this.fileInfos.length || 0;

            if(this.domHelper.IsSlObjectLoaded(inputIndex)) {
                slElement = this.GetSlUploadHelperElement();

                if(!this.options.enableMultiSelect && currentFilesLength && this.changed) {
                    this.fileInfos[0].dispose();
                    this.fileInfos.splice(0, 1);
                }

                var fileInfos = this.fileInfos && this.fileInfos.slice() || [];
                currentFilesLength = fileInfos.length;

                slFileInfos = eval(slElement.content.sl.FileInfos);

                ASPx.Data.ForEach(slFileInfos, function(file, index) {
                    if(index >= currentFilesLength) {
                        file.fileType = "";
                        fileInfo = new ASPxFileInfo(file, this.GetID());
                        fileInfos.push(fileInfo);
                    }
                }.aspxBind(this));
            }
            this.renewFileInfosSubscribtion(fileInfos);

            fileInfos = this.ensureFileInputIndex(fileInfos);

            return fileInfos || [];
        },
        GetUploadHelperElementID: function() {
            return this.GetTextBoxCellID() + IdSuffixes.SL.UploadHelper;
        },
        GetFileSelectorElement: function() {
            return this.GetSlUploadHelperElement();
        },
        GetSlUploadHelperElement: function(id) {
            return document.getElementById(this.GetSlUploadHelperElementID(id));
        },
        GetSlUploadHelperElementID: function(id) {
            return this.GetTextBoxCellID(id) + IdSuffixes.SL.UploadHelper;
        },
        SetFileInputCursor: function(isChooseButton, isOverTrigger) {
            ASPxStandardInputView.prototype.SetFileInputCursor.call(this, isChooseButton, isOverTrigger);

            var fileSelectorElement = this.GetFileSelectorElement();
            this.SetCursorStyle(ASPx.GetCurrentStyle(fileSelectorElement)["cursor"]);
        },
        GetHostElementId: function() {
            return this.baseName + IdSuffixes.SL.UploadHost;
        },
        SetCursorStyle: function (cursorStyle) {
            var inputIndex = this.GetID();
            if(this.domHelper.IsSlObjectLoaded(inputIndex)) {
                var slElement = this.domHelper.GetSlUploadHelperElement(inputIndex);
                slElement.content.sl.SetCursorStyle(cursorStyle);
            }
        },
        Clear: function() {
            this.options.uploadHelper.ClearFileInfos(this.GetID());
            ASPxAdvancedInputView.prototype.Clear.call(this);
        },
        clearFileInputValue: function() { },
        replaceFileInputElementIfNeeded: function() { },
        refreshBase: function(args) {
            ASPxAdvancedInputView.prototype.refreshBase.call(this, args);
            this.renewFileInfosSubscribtion();
        },
        attachOnChangeHandlerForInput: function() { },
        getDialogTriggerHandlers: function() {
            var triggerHandlers = ASPxAdvancedInputView.prototype.getDialogTriggerHandlers.call(this);

            triggerHandlers.mousemove = triggerHandlers.mousemove || [];
            triggerHandlers.mousemove.push(this.createTriggerHandler(this.OnFileInputMouseMove));

            return triggerHandlers;
        }
    });

    var ASPxHTML5InputView = ASPx.CreateClass(ASPxAdvancedInputView, {
        constructor: function(options) {
            this.constructor.prototype.constructor.call(this, options);
        },
        Initialize: function() {
            ASPxAdvancedInputView.prototype.Initialize.call(this, this.options);

            if(this.options.enableMultiSelect) {
                this.GetFileSelectorElement().multiple = true;
                this.initializeTemplateInput();
            }
        },
        initializeTemplateInput: function() {
            this.getFileInputTemplate().multiple = true;
        }
    });

    var ASPxNativeInputView = ASPx.CreateClass(ASPxBaseInputView, {
        constructor: function(options) {
            this.constructor.prototype.constructor.call(this, options);
        },
        Initialize: function() {
            ASPxBaseInputView.prototype.Initialize.call(this);

            if(ASPx.Browser.Firefox)
                this.correctFileInputSize();
        },
        EnsureRender: function() {
            ASPxBaseInputView.prototype.EnsureRender.call(this);

            if(ASPx.Browser.Firefox)
                this.correctFileInputSize();
        },
        AdjustSize: function() {
            this.correctFileInputSize();
        },
        correctFileInputSize: function() {
            if(!this.domHelper.GetMainElement())
                return;

            var width = this.GetFileInputElement().clientWidth,
                fontSize = ASPx.GetCurrentStyle(this.GetFileInputElement()).fontSize,
                size = this.findInputSize(width, fontSize);

            this.GetFileInputElement().size = size;
        },
        findInputSize: function (width, fontSize) {
            var spanInput = document.createElement("SPAN");
            document.body.appendChild(spanInput);

            var fakeInput = document.createElement("INPUT");
            fakeInput.type = "file";
            fakeInput.size = 1;
            fakeInput.style.fontSize = fontSize;
            spanInput.appendChild(fakeInput);

            var stepSize = 1;
            while(true) {
                var previousInputWidth = spanInput.offsetWidth;

                fakeInput.size += stepSize;

                if(previousInputWidth == spanInput.offsetWidth) {
                    fakeInput.size = 1;
                    break;
                }

                if(spanInput.offsetWidth == width)
                    break;
                else if(spanInput.offsetWidth > width) {
                    if(stepSize > 1) {
                        fakeInput.size -= stepSize;
                        stepSize = 1;
                    } else {
                        fakeInput.size -= 1;
                        break;
                    }
                }
                else
                    stepSize *= 2;
            }

            var inputSize = fakeInput.size;
            ASPx.RemoveElement(fakeInput);
            ASPx.RemoveElement(spanInput);

            return inputSize;
        },
        AttachOnChangeHandler: function(fileInput, method) {
            this.AttachOnChangeHandlerCore(fileInput, method);
        },
        Clear: function() {
            ASPxBaseInputView.prototype.Clear.call(this);

            this.supressInputEvent = true;
            this.GetFileInputElement().value = "";
            this.supressInputEvent = false;
        }
    });

    var ASPxErrorView = ASPx.CreateClass(ASPxBaseView, {
        constructor: function(options) {
            this.constructor.prototype.constructor.call(this, options);
        },
        Initialize: function() {
            if(this.options.advancedModeEnabled && !this.options.isFileApiAvailable && !this.options.isSLEnabled && !this.options.autoModeEnabled)
                this.SetVisiblePlatformErrorElement(true);

            var errorRowTemplate = this.GetErrorRowTemplate();
            if(errorRowTemplate)
                this.errorRowTemplateNode = errorRowTemplate.cloneNode(true);
        },
        GetErrorRowTemplate: function() {
            return this.domHelper.GetChildElement(IdSuffixes.Error.RowTemplate);
        },
        GetErrorRow: function(index) {
            return this.domHelper.GetChildElement(IdSuffixes.Error.Row + index);
        },
        GetErrorCell: function(index) {
            return ASPx.GetNodesByTagName(this.GetErrorRow(index), "td")[0];
        },
        Refresh: function(args) {

        },
        Clear: function() {
            this.UpdateCommonErrorDiv("");
        },
        UpdateErrorMessageCell: function(args) {
            var index = args.index,
                errorText = args.errorText,
                isValid = args.isValid;

            if(this.GetErrorRow(index)) {
                var errorCell = this.GetErrorCell(index);

                if(errorText instanceof Array) {
                    var errorTexts = [];
                    for(var i = 0; i < errorText.length; i++)
                        if(!isValid[i] && errorText[i] != "")
                            errorTexts.push(errorText[i]);

                    errorText = errorTexts.join("<br />");
                    ASPx.SetElementDisplay(this.GetErrorRow(index), true);
                }
                else
                    ASPx.SetElementDisplay(this.GetErrorRow(index), !isValid);

                if(errorText != "")
                    errorCell.innerHTML = errorText;
            }
        },
        UpdateCommonErrorDiv: function(text) {
            var commonErrorDiv = this.getCommonErrorDivElement();
            if(commonErrorDiv) 
                commonErrorDiv.innerHTML = text;

            ASPx.SetElementDisplay(commonErrorDiv, !!text.length);
        },
        getCommonErrorDivElement: function () {
            return this.domHelper.GetChildElement(IdSuffixes.Error.Div);
        }
    });

    var ASPxProgressPanelView = ASPx.CreateClass(ASPxBaseView, {
        constructor: function(options) {
            this.constructor.prototype.constructor.call(this, options);
            this.isInCallback = false;
        },
        Initialize: function(options) {
            ASPxBaseView.prototype.Initialize.call(this, options);

            var cancelButton = this.getCancelButton();
            if(cancelButton) {
                this.AttachEventForElement(this.getCancelButton(), "click", function() {
                    this.RaiseStateChangedInternal(this);
                }.aspxBind(this));
            }
        },
        prepareInternalStateChangedArgs: function() {
            return {
                uploadCancelled: true
            };
        },
        OnBeginProcessUploading: function() {
            this.ShowProgressPanel(true);
        },
        ShowProgressPanel: function() {
            window.setTimeout(function() {
                if(this.isInCallback)
                    this.ShowProgressInfoPanel(true);
            }.aspxBind(this), 600);
            this.CleanUploadingInfoPanel();
        },
        ShowProgressInfoPanel: function (show) {
            var inputsTable = this.GetUploadInputsTable();
            ASPx.SetStyles(this.GetProgressPanel(), {
                width: inputsTable.clientWidth,
                height: inputsTable.clientHeight
            });

            ASPx.SetElementDisplay(inputsTable, !show);
            ASPx.SetElementDisplay(this.GetProgressPanel(), show);
            if(!show && ASPx.Browser.Chrome) { // B221522
                var _inputsTable = inputsTable;
                window.setTimeout(function () {
                    ASPx.SetElementVisibility(_inputsTable, true);
                }, 100);
            }

            if(show) {
                var progressControl = this.GetProgressControl();
                if(progressControl != null)
                    progressControl.AdjustControl();
            }
            this.SetButtonEnabled(this.getCancelButton(), true);
        },
        CleanUploadingInfoPanel: function () {
            this.UpdateProgress(0);
        },
        getCancelButton: function() {
            return this.domHelper.GetChildElement(IdSuffixes.Input.ButtonCell.Cancel);
        },
        OnUploadFilesComplete: function() {
            this.UpdateProgress(100);
            this.ShowProgressInfoPanel(false);
        },
        UpdateProgress: function (args) {
            var percent = args.progress;

            if(!(percent > 0 && percent <= 100))
                percent = percent > 0 ? 100 : 0;

            var element = this.GetProgressControl();
            if(element != null)
                element.SetPosition(percent);
        },
        GetProgressControl: function () {
            if(!this.progressControl) {
                var name = this.GetName() + IdSuffixes.Progress.Control;
                this.progressControl = ASPx.GetControlCollection().Get(name);
            }
            return this.progressControl;
        },
        GetProgressPanel: function () {
            return this.domHelper.GetChildElement(IdSuffixes.Progress.Panel);
        }
    });

    var ASPxDropZoneView = ASPx.CreateClass(ASPxBaseView, {
        constructor: function(options) {
            this.constructor.prototype.constructor.call(this, options);

            this.enabled = true;
            this.externalDropZoneIDList = [];
            this.savedExternalDropZoneIDList = null;
            this.inlineDropZone = null;
            this.inlineDropZoneAnchorElementID = options.inlineDropZoneAnchorElementID;

            this.animationStrategy = this.animationStrategies[options.dropZoneAnimationType];

            this.DropZoneEnterInternal = new ASPxClientEvent();
            this.DropZoneLeaveInternal = new ASPxClientEvent();
            this.DropZoneDropInternal = new ASPxClientEvent();

            this.fileSystemHelper = new DragAndDropFileSystemHelper();

            this.fileSystemHelper.ProcessingComplete.ClearHandlers();
            this.fileSystemHelper.ProcessingComplete.AddHandler(this.onDropEventProcessed.aspxBind(this));
        },
        InlineInitialize: function() {
            if(!this.options.disableInlineDropZone) {
                this.InitializeInlineDropZone();

                if(this.inlineDropZone)
                    aspxGetUploadControlCollection().RegisterDropZone(this.GetName(), this.inlineDropZone.id, true);


                this.AdjustInlineDropZone();
            }
        },
        Initialize: function() {
            this.SetExternalDropZoneID(this.options.externalDropZoneIDList);
            aspxGetUploadControlCollection().RegisterAnchorElement(this.GetName(), this.getAnchorElement());
        },
        SetEnabled: function(enabled) {
            if(this.enabled === enabled)
                return;

            this.setInlineDropZoneEnabled(enabled);
            this.setExternalDropZonesEnabled(enabled);

            this.enabled = enabled;
        },
        setInlineDropZoneEnabled: function(enabled) {
            if(!this.inlineDropZone)
                return;

            if(enabled) {
                aspxGetUploadControlCollection().RegisterAnchorElement(this.GetName(), this.getAnchorElement());
                aspxGetUploadControlCollection().RegisterDropZone(this.GetName(), this.inlineDropZone.id, true);
            }
            else {
                aspxGetUploadControlCollection().DeregisterAnchorElement(this.getAnchorElement());
                aspxGetUploadControlCollection().DeregisterDropZones(this.GetName(), [this.inlineDropZone.id], true);
            }
        },
        setExternalDropZonesEnabled: function(enabled) {
            if(enabled) {
                this.SetExternalDropZoneID(this.savedExternalDropZoneIDList || this.externalDropZoneIDList);
                this.savedExternalDropZoneIDList = null;
            }
            else {
                this.savedExternalDropZoneIDList = this.externalDropZoneIDList;
                this.SetExternalDropZoneID();
            }
        },
        SetExternalDropZoneID: function(externalZoneIDList) {
            aspxGetUploadControlCollection().DeregisterDropZones(this.GetName(), this.externalDropZoneIDList);
            this.externalDropZoneIDList = [];

            if(externalZoneIDList && externalZoneIDList.length) {
                ASPx.Data.ForEach(externalZoneIDList, function(zoneId) {
                    aspxGetUploadControlCollection().RegisterDropZone(this.GetName(), zoneId, false);
                    this.externalDropZoneIDList.push(zoneId);
                }.aspxBind(this));
            }
        },
        AdjustInlineDropZone: function() {
            if(this.options.disableInlineDropZone || !this.inlineDropZone)
                return;

            var dropZone = this.inlineDropZone,
                anchorElement = this.getAnchorElement(),
                anchorStyle = ASPx.GetCurrentStyle(anchorElement),
                anchorRect = anchorElement.getBoundingClientRect();

            ASPx.Attr.SetAttribute(dropZone.style, "height", anchorElement.offsetHeight + "px");
            ASPx.Attr.SetAttribute(dropZone.style, "width", anchorElement.offsetWidth + "px");
            ASPx.Attr.SetAttribute(dropZone.style, "top", anchorRect.top + "px");
            ASPx.Attr.SetAttribute(dropZone.style, "left", anchorRect.left + "px");
            ASPx.Attr.SetAttribute(dropZone.style, "padding", anchorStyle.padding);
        },
        AdjustSize: function() {
            this.AdjustInlineDropZone();
        },
        InitializeInlineDropZone: function() {
            this.inlineDropZone = this.domHelper.GetMainElement().previousElementSibling;
        },
        getAnchorElement: function() {
            if(!this.anchorElement) {
                if(this.inlineDropZoneAnchorElementID)
                    this.anchorElement = document.getElementById(this.inlineDropZoneAnchorElementID);
                else
                    this.anchorElement = this.domHelper.GetMainElement();
            }
            return this.anchorElement;
        },
        SetInlineDropZoneAnchorElementID: function(id) {
            aspxGetUploadControlCollection().DeregisterAnchorElement(this.getAnchorElement());
            this.anchorElement = null;
            this.inlineDropZoneAnchorElementID = id;
            aspxGetUploadControlCollection().RegisterAnchorElement(this.GetName(), this.getAnchorElement());

            this.moveInlineDropZoneToAnchor();
        },
        moveInlineDropZoneToAnchor: function() {
            this.anchorElement.insertBefore(this.inlineDropZone, this.anchorElement.firstChild);
            ASPx.AddClassNameToElement(this.inlineDropZone, CSSClasses.DropZone.HasExternalAnchor);
        },
        refreshBase: function(args) {
            ASPxBaseView.prototype.refreshBase.call(this, args);

            this.AdjustInlineDropZone();
        },
        updateFileInfos: function(args) {
            this.fileInfos = args.fileInfos[0];
        },
        onDropEventProcessed: function(files) {
            this.rawFiles = files;

            if(!this.options.enableMultiSelect && this.rawFiles.length > 1)
                this.raiseError(this.options.dragAndDropMoreThanOneFileError);
            else
                this.RaiseStateChangedInternal(this);
        },
        OnDrop: function(e, zoneId) {
            this.rawFiles = [];

            this.RaiseDropZoneDropInternal(e, zoneId);
            this.RaiseDropZoneLeave(e, zoneId);

            this.fileSystemHelper.processDataTransfer(e.dataTransfer);

            if(zoneId === this.inlineDropZone.id)
                this.animationStrategy.hide(this.inlineDropZone);
        },
        OnDragLeave: function(e, zoneId) {
            if(this.inlineDropZone && zoneId === this.inlineDropZone.id)
                this.animationStrategy.hide(this.inlineDropZone);

            this.RaiseDropZoneLeave(e, zoneId);
        },
        OnDragEnter: function(e, zoneId) {
            if(zoneId === this.inlineDropZone.id)
                this.onDragEnterInlineZone();

            this.RaiseDropZoneEnter(e, zoneId);
        },
        onDragEnterInlineZone: function() {
            this.AdjustInlineDropZone();
            this.animationStrategy.show(this.inlineDropZone);
        },
        RaiseDropZoneEnter: function(e, dropZoneId) {
            if(!this.DropZoneEnterInternal.IsEmpty()) {
                var dropZone = document.getElementById(dropZoneId),
                    args = new ASPxClientUploadControlDropZoneEnterEventArgs(dropZone);

                this.DropZoneEnterInternal.FireEvent(args);
            }
        },
        RaiseDropZoneLeave: function(e, dropZoneId) {
            if(!this.DropZoneLeaveInternal.IsEmpty()) {
                var dropZone = document.getElementById(dropZoneId),
                    args = new ASPxClientUploadControlDropZoneLeaveEventArgs(dropZone);

                this.DropZoneLeaveInternal.FireEvent(args);
            }
        },
        RaiseDropZoneDropInternal: function(e, dropZoneId) {
            if(!this.DropZoneEnterInternal.IsEmpty()) {
                var dropZone = document.getElementById(dropZoneId),
                    args = new ASPxClientUploadControlDropZoneDropEventArgs(dropZone);
                this.DropZoneDropInternal.FireEvent(args);
            }
        },
        GetFileInfos: function() {
            this.fileInfos = this.fileInfos || [];

            if(!this.options.enableMultiSelect)
                this.fileInfos = [];

            ASPx.Data.ForEach(this.rawFiles, function(file) {
                this.fileInfos.push(new ASPxFileInfo(file, 0))
            }.aspxBind(this));

            return [this.fileInfos];
        },
        animationStrategies: {
            None: {
                show: function(dropZone) {
                    ASPx.SetElementDisplay(dropZone, true);
                },
                hide: function(dropZone) {
                    ASPx.SetElementDisplay(dropZone, false);
                }
            },
            Fade: {
                animationDuration: 250,
                show: function(dropZone) {
                    ASPx.AnimationHelper.setOpacity(dropZone, 0);
                    ASPx.SetElementDisplay(dropZone, true);
                    ASPx.AnimationHelper.fadeIn(dropZone, null, this.animationDuration);
                },
                hide: function(dropZone) {
                    ASPx.AnimationHelper.setOpacity(dropZone, 1);
                    ASPx.AnimationHelper.fadeOut(dropZone, function() {
                        ASPx.SetElementDisplay(dropZone, false);
                    }, this.animationDuration);
                }
            }
        }
    });

        var DragAndDropFileSystemHelper = ASPx.CreateClass(null, {
            constructor: function() {
                this.ProcessingComplete = new ASPxClientEvent();
                this.ValidationComplete = new ASPxClientEvent();
            },
            processDataTransfer: function(dataTransfer) {
                this.files = [];

                if(dataTransfer.items)
                    this.processWebkitItems(dataTransfer.items);
                else if(dataTransfer.files)
                    this.finalizeProcessing(dataTransfer.files);
            },
            processWebkitItems: function(items) {
                this.callbackCount = 0;

                ASPx.Data.ForEach(items, function(item) {
                    var entries = [item.webkitGetAsEntry()];
                    this.processEntries(entries);
                }.aspxBind(this));
            },
            processEntries: function(entries) {
                ASPx.Data.ForEach(entries, function(entry) {
                    if (entry.isDirectory)
                        this.processDirectory(entry.createReader(), this.processEntries);
                    else {
                        this.callbackCount++;

                        entry.file(this.appendFile.aspxBind(this));
                    }
                }.aspxBind(this));
            },
            appendFile: function(file) {
                this.callbackCount--;

                this.files.push(file);

                if(this.callbackCount === 0)
                    this.finalizeProcessing(this.files);
            },
            processDirectory: function(directoryReader, callback) {
                var entries = [],
                    that = this;

                var readEntries = function() {
                    that.callbackCount++;

                    directoryReader.readEntries(function(results) {
                        that.callbackCount--;

                        if(results.length) {
                            entries = entries.concat(results.slice(0));
                            readEntries();
                        } else
                            callback.call(that, entries);
                    });
                };

                readEntries();
            },
            finalizeProcessing: function(files) {
                if(this.ValidationComplete.IsEmpty()) {
                    this.ValidationComplete.AddHandler(function() {
                        this.ProcessingComplete.FireEvent(this.validFiles);
                    }.aspxBind(this));
                }

                this.validateFiles(files);
            },
            validateFiles: function(files) {
                this.validFiles = files;
                this.callbackCount = 0;

                for(var i = 0; i < files.length; i++)
                    this.validateFile(files[i]);

                if(this.callbackCount === 0)
                    this.ValidationComplete.FireEvent();
            },
            validateFile: function(file) {
                if(!file.type)
                    this.examineFile(file);
            },
            examineFile: function(file) {
                var that = this;

                try {
                    var reader = new FileReader();

                    reader.onerror = that.onReaderError.aspxBind(that);
                    reader.onload = that.onReaderLoad.aspxBind(that);

                    this.callbackCount++;
                    reader.readAsDataURL(file);
                }
                catch(e) {
                    this.onReaderError();
                }
            },
            onReaderLoad: function() {
                if(--this.callbackCount === 0)
                    this.ValidationComplete.FireEvent();
            },
            onReaderError: function() {
                this.validFiles = [];
                this.ValidationComplete.FireEvent();
            }
        });

    ASPxClientUploadControl.isValidDragAndDropEvent = function(e) {
        var isValid = false,
            forbiddenTypes = ["Text", "text/plain"],
            type;

        for(var i = 0; i < e.dataTransfer.types.length; i++) {
            type = e.dataTransfer.types[i];

            if(forbiddenTypes.indexOf(type) > -1) {
                isValid = false;
                break;
            }

            if(type === "Files")
                isValid = true;
        }

        return isValid;
    };


    var ASPxFileInfoCache = ASPx.CreateClass(null, {
        constructor: function() {
            this.fileInfos = [];
            this.FileListChanged = new ASPxClientEvent();

            ASPxFileInfoCache.getPlainArray = this.getPlainArray;
        },
        Update: function(args) {
            this.UpdateFileInfos(args);

            var listChangedArgs = new ASPxFileListChangedInternalArgs(this.fileInfos, args.inputIndex, args.fileCountChanged);
            this.RaiseFileListChanged(listChangedArgs);
        },
        UpdateFileInfos: function(args) {
            var currentLength = this.getPlainArray(this.fileInfos).length,
                newLength = this.getPlainArray(args.fileInfos).length;

            args.fileCountChanged = currentLength !== newLength;
            this.fileInfos = args.fileInfos
        },
        Get: function() {
            return this.getPlainArray(this.fileInfos);
        },
        clear: function() {
            ASPx.Data.ForEach(this.fileInfos, function(fileInfosArr) {
                fileInfosArr && fileInfosArr.reverse();
                ASPx.Data.ForEach(fileInfosArr, function(fileInfo) {
                    fileInfo.dispose();
                }.aspxBind(this));
            }.aspxBind(this));

            this.Update(ASPxEmptyRefreshArgs);
        },
        getPlainArray: function(complexArray) {
            var result = [];

            ASPx.Data.ForEach(complexArray, function(inputFileInfos) {
                ASPx.Data.ForEach(inputFileInfos, function(fileInfo, fileIndex) {
                    fileInfo.fileIndex = fileIndex;
                    result.push(fileInfo);
                });
            });

            return result;
        },
        getFileInfosCount: function() {
            var count = 0;
            ASPx.Data.ForEach(this.fileInfos, function(fileInfos) {
                count += fileInfos.length;
            }.aspxBind(this));

            return count;
        },
        RemoveFile: function(index) {
            var absoluteIndex = 0;

            for(var i = 0; i < this.fileInfos.length; i++) {
                var current = this.fileInfos[i],
                    remained = [];

                ASPx.Data.ForEach(current, function(fileInfo) {
                    if(absoluteIndex++ === index)
                        fileInfo.dispose();
                    else
                        remained.push(fileInfo);
                });

                this.fileInfos[i] = remained.slice();
            }

            var args = {
                fileInfos: this.fileInfos,
                inputIndex: undefined
            };

            this.Update(args);
        },
        GetFileIndexesCount: function(fileInputCount) {
            var count = 0;
            for(var inputIndex = 0; inputIndex < fileInputCount; inputIndex++) {
                var fileInfos = this.GetFileInfos(inputIndex);
                count += fileInfos.length > 0 ? fileInfos.length : 1;
            }
            return count;
        },
        RaiseFileListChanged: function(args) {
            args.fileInfosCount = this.getFileInfosCount();
            this.FileListChanged.FireEvent(args);
        }
    });

    var ASPxLegacyUploadManager = ASPx.CreateClass(null, {
        constructor: function(options) {
            this.options = options;
            this.isInCallback = false;
            this.isNative = false;
            this.domHelper = options.domHelper;
            this.uploadHelper = options.uploadHelper;
            this.progressHandlerPage = options.progressHandlerPage;
            this.uploadingKey = options.uploadingKey;
            this.name = options.name;
            this.packetSize = options.packetSize;
            this.unspecifiedErrorText = options.unspecifiedErrorText;
            this.validationSettings = options.validationSettings;

            this.InitializeIframe();

            this.FileUploadErrorInternal = new ASPxClientEvent();

            this.UploadInitiatedInternal = new ASPxClientEvent();

            this.InternalError = new ASPxClientEvent();

            this.FileUploadCompleteInternal = new ASPxClientEvent();
            this.FilesUploadCompleteInternal = new ASPxClientEvent();
            this.FileUploadStartInternal = new ASPxClientEvent();
            this.UploadingProgressChangedInternal = new ASPxClientEvent();
            this.InCallbackChangedInternal = new ASPxClientEvent();
            this.NeedSetJSProperties = new ASPxClientEvent();

            this.BeginProcessUploadingInternal = new ASPxClientEvent();
        },
        UploadFileFromUser: function(fileInfos) {
            this.fileInfos = fileInfos;

            this.currentFileIndex = 0;

            this.UploadInitiatedInternal.FireEvent();

            this.isAborted = false;
            this.isCancel = false;
            this.uploadProcessingErrorText = "";

            var validateObj = {
                commonErrorText: "",
                commonCallbackData: ""
            };

            if(!this.isInCallback) {
                if(this.IsFileUploadCanceled(validateObj)) {
                    this.isCancel = true;

                    this.RaiseFilesUploadCompleteInternal(validateObj);
                    return false;
                }

                var isSuccessful = true;
                if(this.IsAdvancedModeEnabled())
                    this.BeginProcessUploading();
                else {
                    isSuccessful = this.UploadForm();
                    if(this.IsUploadProcessingEnabled())
                        this.BeginProcessUploading();
                }

                return true;
            }
        },
        GetUploadInputsTable: function() {
            return this.domHelper.GetChildElement(IdSuffixes.Input.UploadInputsTable);
        },
        IsAdvancedModeEnabled: function() {
            return this.options.advancedModeEnabled && (this.options.isFileApiAvailable || this.options.isSLEnabled);
        },
        CancelUploading: function(isUI) {
            this.cancelUploadingProcess(isUI);
            window.setTimeout(this.UploadAsyncCancelProcessing.aspxBind(this), 100)
        },
        cancelUploadingProcess: function (isUI) {
            if(this.isInCallback) {
                if(isUI)
                    this.isCancel = true;
                else
                    this.isAborted = true;
                var iframeUrl = ASPx.SSLSecureBlankUrl;
                if(ASPx.Browser.Opera)
                    this.SetIFrameUrl(iframeUrl + "&" + Constants.QueryParamNames.CANCEL_UPLOAD + "=" + (new Date()).valueOf());
                this.SetIFrameUrl(iframeUrl);
                this.EndProcessUploading();
            }
        },
        CancelUploadingFileFromHelper: function () {
            this.cancelUploadingProcess();
        },
        IsRightToLeft: function () {
            return ASPx.IsElementRightToLeft(this.GetMainElement());
        },
        IsUploadProcessingEnabled: function () {
            return this.options.uploadProcessingEnabled;
        },
        CreateXmlHttpRequestObject: function () {
            if(!this.xmlHttpRequest) {
                if(typeof (XMLHttpRequest) != 'undefined')
                    this.xmlHttpRequest = new XMLHttpRequest();
                else if(typeof (ActiveXObject) != 'undefined')
                    this.xmlHttpRequest = new ActiveXObject('Microsoft.XMLHTTP');
                this.xmlHttpRequest.onreadystatechange = function() { this.UploadAsyncXmlHttpResponse(this.xmlHttpRequest); }.aspxBind(this);
            }
            return this.xmlHttpRequest;
        },
        BeginProcessUploading: function() {
            this.TotalFilesLength = this.GetTotalLength();
            this.BeginProcessUploadingInternal.FireEvent();
            this.uploadingTimerID = window.setInterval(function() { this.UploadProcessing(); }.aspxBind(this), 1000);
        },
        EndProcessUploading: function () {
            this.uploadingInfo = null;
            if(this.uploadingTimerID != null)
                this.uploadingTimerID = ASPx.Timer.ClearInterval(this.uploadingTimerID);

            if(this.IsAdvancedModeEnabled() && !this.isCancel && !this.isAborted)
                this.UploadForm();
        },
        GetFileIndexesCount: function() {
            return this.fileInfos.length;
        },
        GetHelperRequestData: function () {
            this.currentFileIndex = this.currentFileIndex || 0;
            this.isLastChunk = false;
            var fileInfo = this.fileInfos[this.currentFileIndex] || {},
                currentIndex = this.currentFileIndex,
                startPos = 0,
                requestData = { data: "", isNewFile: !fileInfo.uploadedLength },
                isNewUploading = !(currentIndex || !fileInfo || fileInfo.uploadedLength),
                chunkLength;

            if(fileInfo) {
                fileInfo.uploadedLength = fileInfo.uploadedLength || 0;
                startPos = fileInfo.uploadedLength;
                chunkLength = fileInfo.fileSize - fileInfo.uploadedLength;
                if(chunkLength > this.packetSize) {
                    chunkLength = this.packetSize;
                } else {
                    this.currentFileIndex++;
                    if(this.currentFileIndex === this.fileInfos.length) {
                        this.isLastChunk = true;
                    }
                }

                fileInfo.uploadedLength += chunkLength;
            }

            var fileData = this.uploadHelper.ReadFileData(fileInfo.file, startPos, chunkLength, fileInfo.inputIndex, fileInfo.fileIndex);
            if(fileData.errorText)
                requestData.errorText = fileData.errorText;
            else {
                requestData.data = this.uploadHelper.BuildChunkRequest(
                    isNewUploading,
                    this.options.settingsID,
                    this.TotalFilesLength,
                    fileInfo.inputIndex,
                    currentIndex,
                    fileInfo.fileSize,
                    fileInfo.fileType,
                    chunkLength,
                    fileInfo.fileName,
                    this.options.signature,
                    fileData.data);
            }

            return requestData;
        },
        //TODO: get rid of this
        GetUploadingInfo: function () {
            if(!this.uploadingInfo) {
                this.uploadingInfo = {
                    isUploadingStart: false,
                    isComplete: false,
                    currentFileName: "",
                    currentFileContentLength: 0,
                    currentFileUploadedContentLength: 0,
                    currentFileProgress: 0,
                    currentContentType: "",
                    totalUploadedSize: 0,
                    totalLength: 0,
                    progress: 0,
                    errorText: ""
                };
            }
            return this.uploadingInfo;
        },
        GetTotalLength: function() {
            var totalFileLength = 0;
            ASPx.Data.ForEach(this.fileInfos, function(file) {
                totalFileLength += parseInt(file.fileSize);
            });

            return totalFileLength;
        },
        UpdateUploadingInfo: function (responseXML) {
            var info = this.GetUploadingInfo();

            if(responseXML == null || this.GetXmlAttribute(responseXML, 'empty') == 'true') {
                if(info.isUploadingStart) {
                    info.isUploadingStart = false;
                    info.isComplete = true;
                    info.progress = 100;
                    info.totalUploadedSize = info.totalLength;
                }
                return;
            }
            info.isUploadingStart = true;
            info.errorText = this.GetXmlAttribute(responseXML, 'errorText');
            info.currentFileName = this.GetXmlAttribute(responseXML, 'fileName');

            info.currentFileContentLength = this.GetXmlAttribute(responseXML, 'fileSize');
            info.currentFileUploadedContentLength = this.GetXmlAttribute(responseXML, 'fileUploadedSize');
            info.currentFileProgress = this.GetXmlAttribute(responseXML, 'fileProgress');

            info.currentContentType = this.GetXmlAttribute(responseXML, 'contentType');
            info.totalUploadedSize = parseInt(this.GetXmlAttribute(responseXML, 'totalUploadedSize'));
            info.totalLength = parseInt(this.GetXmlAttribute(responseXML, 'totalSize'));
            info.progress = parseInt(this.GetXmlAttribute(responseXML, 'progress'));
        },
        UploadProcessing: function () {
            if(this.isProgressWaiting || this.isResponseWaiting) return;
            this.isProgressWaiting = true;

            var xmlHttp = this.CreateXmlHttpRequestObject();
            if(xmlHttp == null) {
                this.isProgressWaiting = false;
                this.EndProcessUploading();
                return;
            }

            if(!this.GetUploadingInfo().isComplete) {
                var url = this.progressHandlerPage + '?' + Constants.QueryParamNames.PROGRESS_HANDLER + '=' + this.GetProgressInfoKey();

                var httpMethod = "GET";
                var requestData = { data: "" };
                if(this.IsAdvancedModeEnabled()) {
                    url += "&" + Constants.QueryParamNames.HELPER_UPLOADING_CALLBACK + "=" + this.name;
                    httpMethod = "POST";

                    var previousFileIndex = this.currentFileIndex;
                    requestData = this.GetHelperRequestData();

                    if(requestData.isNewFile) {
                        this.fileInfos[previousFileIndex].OnUploadStart.FireEvent();
                    }

                    if(requestData.errorText) {
                        this.isProgressWaiting = false;
                        this.uploadProcessingErrorText = requestData.errorText;
                        this.CancelUploadingFileFromHelper();
                        return;
                    }
                }

                xmlHttp.open(httpMethod, url, true);
                xmlHttp.send(requestData.data);
                this.isResponseWaiting = true;
            }
            else {
                this.fileInfos[this.currentFileIndex].OnUploadComplete.FireEvent();
                this.EndProcessUploading();
            }

            this.isProgressWaiting = false;
        },
        UploadAsyncXmlHttpResponse: function (xmlHttp) {
            if(xmlHttp && xmlHttp.readyState == 4) {
                var successful = false;
                if(xmlHttp.status == 200) {
                    var xmlDoc = this.GetResponseXml(xmlHttp);
                    this.UpdateUploadingInfo(xmlDoc);
                    var info = this.GetUploadingInfo();
                    successful = !info.errorText;
                }

                if(successful) {
                    if(info.isUploadingStart || info.isComplete) {
                        this.OnUploadingProgressChanged(this.fileInfos.length, info);
                    }
                } else {
                    if(this.IsAdvancedModeEnabled()) {
                        this.uploadProcessingErrorText = (info && info.errorText != "") ? info.errorText : xmlHttp.statusText;
                        this.isLastChunk = true;
                        this.CancelUploadingFileFromHelper();
                    }
                }

                var isEndProcessUploading = !(this.GetUploadingInfo().isUploadingStart || this.isInCallback) ||
                    this.IsAdvancedModeEnabled() && this.isLastChunk || this.uploadingTimerID == -1;

                if(isEndProcessUploading)
                    this.EndProcessUploading();
                else if(this.IsAdvancedModeEnabled())
                    window.setTimeout(function() { this.UploadProcessing(); }.aspxBind(this), 0);

                this.isResponseWaiting = false;
            }
        },
        UploadAsyncCancelProcessing: function () {
            if(this.isResponseWaiting)
                window.setTimeout(this.UploadAsyncCancelProcessing.aspxBind(this), 100);
            else {
                var xmlHttp = this.CreateXmlHttpRequestObject();
                if(xmlHttp && this.IsAdvancedModeEnabled()) {
                    var url = this.progressHandlerPage + '?' + Constants.QueryParamNames.PROGRESS_HANDLER + '=' + this.GetProgressInfoKey();
                    url += "&" + Constants.QueryParamNames.HELPER_UPLOADING_CALLBACK + "=" + this.name;
                    var cancelRequest = this.uploadHelper.BuildCancelRequest(this.options.settingsID, this.options.signature);
                    xmlHttp.open("POST", url, false);
                    xmlHttp.send(cancelRequest);
                }
            }
        },
        GetResponseXml: function(xmlHttp) {
            var xmlDoc = xmlHttp.responseXML;
            if(this.IsInvalidXmlDocument(xmlDoc)) {
                var responseContent = this.GetContentFromString(xmlHttp.responseText, "<", ">");
                try {
                    xmlDoc = ASPx.Xml.Parse(responseContent);
                }
                catch(ex) {
                    xmlDoc = null;
                }
            }
            return xmlDoc;
        },
        IsInvalidXmlDocument: function(xmlDoc) {
            var ret = xmlDoc == null || xmlDoc.documentElement == null;
            if(!ret) {
                var xmlStr = xmlDoc.documentElement.outerHTML;
                ret = xmlStr && xmlStr.toLowerCase().indexOf("parsererror") > -1;
            }
            return ret;
        },
        GetContentFromString: function(str, startSubstr, endSubstr) {
            var startIndex = str.indexOf(startSubstr),
                endIndex = str.lastIndexOf(endSubstr);
            return str.substring(startIndex, endIndex + 1);
        },
        GetXmlAttribute: function (xmlDoc, attrName) {
            return xmlDoc.documentElement.getAttribute(attrName);
        },
        WriteResponseString: function (responseString) {
            try {
                this.GetFakeIframeDocument().body.innerHTML = responseString;
            }
            catch (e) { }
        },
        restoreProtectedWhitespaceSeries: function (text) {
            return text.replace(/&nbsp;/g, ' ').replace(/&nbspx;/g, '&nbsp;');
        },
        raiseFileUploadErrorInternal: function(errorText, errorType, index) {
            this.FileUploadErrorInternal.FireEvent({
                text: errorText,
                type: errorType,
                inputIndex: index
            });
        },
        OnCompleteFileUpload: function () {
            var commonErrorText = '';
            var responseObj = this.GetResponseObject();

            if(responseObj) {
                if(responseObj.customJSProperties)
                    this.NeedSetJSProperties.FireEvent(responseObj.customJSProperties);

                var indexTable = [];
                ASPx.Data.ForEach(this.fileInfos, function(fileInfo) {
                    var index = fileInfo.inputIndex;
                    if(!indexTable[index])
                        indexTable[index] = 1;
                    else
                        indexTable[index]++;
                });

                var fileInputCount = this.domHelper.GetFileInputCountInternal();

                for(var inputIndex = 0, responseFileIndex = -1; inputIndex < fileInputCount; inputIndex++) {
                    var fileCount = indexTable[inputIndex] || 0;
                    if(this.options.enableMultiSelect) {
                        var errorTexts = [];
                        for(var j = 0; j < fileCount; j++) {
                            var eventInputIndex = fileInputCount > 1 ? inputIndex : j;
                            responseFileIndex++;

                            this.raiseFileUploadCompleteInternal(responseFileIndex, eventInputIndex, responseObj);

                            if(!responseObj.isValidArray[responseFileIndex] && responseObj.errorTexts[responseFileIndex])
                                errorTexts.push(responseObj.errorTexts[responseFileIndex]);
                        }

                        if(errorTexts.length)
                            this.raiseFileUploadErrorInternal(errorTexts.join("<br />"), ASPxUploadErrorTypes.InputRowError, inputIndex);
                    }
                    else {
                        responseFileIndex++;
                        if(fileCount == 1) {
                            if(!responseObj.isValidArray[responseFileIndex] && responseObj.errorTexts[responseFileIndex])
                                this.raiseFileUploadErrorInternal(responseObj.errorTexts[responseFileIndex], ASPxUploadErrorTypes.InputRowError, inputIndex);

                            this.raiseFileUploadCompleteInternal(responseFileIndex, inputIndex, responseObj);
                        }
                    }
                }
            }

            if(!this.isCancel) {
                if(responseObj)
                    commonErrorText = responseObj.commonErrorText;
                else if(this.uploadProcessingErrorText != '')
                    commonErrorText = this.uploadProcessingErrorText;
                else
                    commonErrorText = this.unspecifiedErrorText;
            } else {
                var currentFileInfo = this.fileInfos[this.currentFileIndex] || this.fileInfos[this.currentFileIndex - 1];
                currentFileInfo.OnUploadComplete.FireEvent();
            }

            if(commonErrorText) {
                this.raiseFileUploadErrorInternal(commonErrorText, ASPxUploadErrorTypes.Common);
            }

            this.RaiseInCallbackChange(false);

            if(responseObj)
                this.RaiseFilesUploadCompleteInternal(responseObj);
            else
                this.RaiseFilesUploadCompleteInternal({
                    commonErrorText: commonErrorText,
                    commonCallbackData: ""
                });

            // Bugfix B94907
            if(ASPx.Browser.IE) {
                try {
                    this.GetFakeIframeDocument().write("");
                    this.GetFakeIframeDocument().close();
                }
                catch (e) { }
            }
        },
        InitializeIframe: function() {
            if(ASPx.Browser.Opera && !frames[this.GetFakeIframeName()])
                this.ReinitializeIFrame(this.GetFakeIframe());

            this.GetIFrameUrl();

            ASPx.Evt.AttachEventToElement(ASPx.Browser.IE ? this.GetFakeIframeElement() : this.GetFakeIframe(), "load", function(){
                if(this.isInCallback)
                    this.OnCompleteFileUpload();
            }.aspxBind(this));
        },
        ReinitializeIFrame: function(iframe) {
            var divElem = document.createElement("DIV");
            ASPx.SetElementDisplay(divElem, false);

            var parentIframe = iframe.parentNode;
            parentIframe.appendChild(divElem);
            divElem.appendChild(iframe);
        },
        GetFakeIframe: function() {
            var name = this.GetFakeIframeName();
            return ASPx.Browser.IE ? frames[name] : document.getElementById(name);
        },
        GetFakeIframeElement: function () {
            return this.GetFakeIframe().frameElement;
        },
        GetFakeIframeResponseString: function () {
            var html = ASPx.Str.DecodeHtml(this.GetFakeIframeDocument().body.innerHTML);
            if(ASPx.Browser.IE && ASPx.Browser.Version == 8) // B208078
                html = this.restoreProtectedWhitespaceSeries(html);
            return html;
        },
        GetFakeIframeDocument: function () {
            return ASPx.Browser.IE ? this.GetFakeIframe().document : this.GetFakeIframe().contentDocument;
        },
        GetIFrameUrl: function() {
            if(!this.iframeUrl) {
                var iframe = ASPx.Browser.IE ? this.GetFakeIframeElement() : this.GetFakeIframe();
                var iframeSrc = ASPx.Attr.GetAttribute(iframe, "src");
                this.iframeUrl = (iframeSrc) ? iframeSrc : "";
            }
            return this.iframeUrl;
        },
        SetIFrameUrl: function (url) {
            var iframe = ASPx.Browser.IE ? this.GetFakeIframeElement() : this.GetFakeIframe();
            ASPx.Attr.SetAttribute(iframe, "src", url);
        },
        GetFakeIframeName: function() {
            return this.name + IdSuffixes.Upload.IFrame;
        },
        OnUploadingProgressChanged: function (fileCount, info) {
            this.RaiseUploadingProgressChanged(fileCount, info.currentFileName, info.currentFileContentLength,
                info.currentFileUploadedContentLength, info.currentFileProgress, info.totalLength, info.totalUploadedSize, info.progress);

            if(info.currentFileProgress === "100" && !(this.isCancel || this.isAborted) && this.currentFileIndex)
                this.fileInfos[this.currentFileIndex - 1].OnUploadComplete.FireEvent();
        },
        GetProgressInfoKey: function () {
            return this.uploadingKey;
        },
        GetResponseObject: function () {
            var ret = null,
                responseString = "";
            try {
                responseString = this.GetFakeIframeResponseString();
                ret = window.eval(responseString);
            }
            catch(ex) {
                if(ASPx.Browser.IE)
                    this.GetFakeIframe().window.location = this.GetIFrameUrl(); // hack B35365
            }
            return this.GetCorrectedResponseObject(ret, responseString);
        },
        GetCorrectedResponseObject: function(responseObj, responseString) {
            if(responseObj != null && !responseObj.hasOwnProperty("commonCallbackData")) {
                var responseContent = this.GetContentFromString(responseString, "(", ")");
                try {
                    responseObj = window.eval(responseContent);
                }
                catch(ex) { }
            }
            return responseObj;
        },
        GetUploadFormAction: function (form) {
            var action = form.action;

            // ProgressQueryParam
            if(this.IsAdvancedModeEnabled())
                action = this.AddQueryParamToUrl(action, Constants.QueryParamNames.PROGRESS_HANDLER, this.GetProgressInfoKey());
            else if(this.IsUploadProcessingEnabled())
                action = this.AddQueryParamToUrl(action, Constants.QueryParamNames.PROGRESS_INFO, this.GetProgressInfoKey());

            // UploadCallbackControlID
            if(this.IsAdvancedModeEnabled())
                action = this.AddQueryParamToUrl(action, Constants.QueryParamNames.HELPER_UPLOADING_CALLBACK, this.name);
            else
                action = this.AddQueryParamToUrl(action, Constants.QueryParamNames.UPLOADING_CALLBACK, this.name);

            return action;
        },
        AddQueryParamToUrl: function (url, paramName, paramValue) {
            var prefix = url.indexOf("?") >= 0 ? "&" : "?";
            var paramQueryString = prefix + paramName + "=" + paramValue;
            var anchorStart = url.indexOf("#");
            return anchorStart >= 0
                ? url.substring(0, anchorStart) + paramQueryString + url.substring(anchorStart)
                : url + paramQueryString;
        },
        GetUploadFormTarget: function (form) {
            return this.GetFakeIframe().name;
        },
        UploadForm: function () {
            // -- Prepare for uploading
            var form = this.GetParentForm();
            if(!form) return;

            var sourceTarget = form.target;
            var soureActionString = form.action;
            var sourceMethod = form.method;

            form.action = this.GetUploadFormAction(form);
            form.target = this.GetUploadFormTarget(form);
            form.method = "post";

            var isInternalErrorOccurred = false;
            try {
                form.submit();
            }
            catch (e) {
                isInternalErrorOccurred = true;
                this.WriteResponseString(Constants.ERROR_TEXT_RESPONSE_PREFIX + this.options.generalErrorText);
                this.OnCompleteFileUpload();
            }

            form.target = sourceTarget;
            form.action = soureActionString;
            form.method = sourceMethod;

            return !isInternalErrorOccurred;
        },
        IsFileUploadCanceled: function (validateObj) {
            var isCancel = this.RaiseFileUploadStartInternal();
            if(!isCancel)
                this.RaiseInCallbackChange(true);
            else
                validateObj.commonErrorText = this.options.uploadWasCanceledErrorText;
            return isCancel;
        },
        RaiseInCallbackChange: function(isInCallback) {
            this.isInCallback = isInCallback;
            this.InCallbackChangedInternal.FireEvent(isInCallback);
        },
        RaiseUploadingProgressChanged: function (fileCount, currentFileName, currentFileContentLength,
                                                 currentFileUploadedContentLength, currentFileProgress, totalContentLength, uploadedContentLength, progress) {
            if(!this.UploadingProgressChangedInternal.IsEmpty()) {
                var args = new ASPxClientUploadControlUploadingProgressChangedEventArgs(fileCount, currentFileName, currentFileContentLength,
                    currentFileUploadedContentLength, currentFileProgress, totalContentLength, uploadedContentLength, progress);
                this.UploadingProgressChangedInternal.FireEvent(args);
            }
        },
        raiseFileUploadCompleteInternal: function(index, inputIndex, responseObj) {
            if(!this.FileUploadCompleteInternal.IsEmpty()) {
                var args = new ASPxClientUploadControlFileUploadCompleteEventArgs(inputIndex, responseObj.isValidArray[index],
                    responseObj.errorTexts[index], responseObj.callbackDataArray[index]);
                this.FileUploadCompleteInternal.FireEvent(args);
            }
        },
        RaiseFilesUploadCompleteInternal: function (responseObj) {
            ASPx.Data.ForEach(this.fileInfos, function(fileInfo) {
                fileInfo.uploadedLength = 0;
            });

            var args = {
                commonErrorText: responseObj.commonErrorText,
                commonCallbackData: responseObj.commonCallbackData,
                uploadCancelled: this.isCancel || this.isAborted
            };

            this.FilesUploadCompleteInternal.FireEvent(args);
        },
        RaiseFileUploadStartInternal: function () {
            var ret = false;
            var args = new ASPxClientUploadControlFilesUploadStartEventArgs(false);
            this.FileUploadStartInternal.FireEvent(args);
            ret = args.cancel;

            return ret;
        }
    });

    var ASPxUploadHelperStandardStrategy = ASPx.CreateClass(null, {
        constructor: function(options) {
            this.domHelper = options.domHelper;
        },
        IsHelperElementReady: function() {
            return true;
        }
    });

    var ASPxUploadHelperHTML5 = ASPx.CreateClass(ASPxUploadHelperStandardStrategy, {
        constructor: function(options) {
            this.constructor.prototype.constructor.call(this, options);
        },
        FileSlice: function(file, startPos, endPos) {
            if(file.slice)
                return file.slice(startPos, endPos);
            if(ASPx.Browser.WebKitFamily && file.webkitSlice)
                return file.webkitSlice(startPos, endPos);
            if(ASPx.Browser.NetscapeFamily && file.mozSlice)
                return file.mozSlice(startPos, endPos);
            throw "'File.slice()' method is not implemented";
        },
        ReadFileData: function(file, startPos, chunkLength) {
            var fileData = {};
            if(!chunkLength)
                return fileData;

            try {
                fileData.data = this.FileSlice(file, startPos, startPos + chunkLength);
            }
            catch(ex) {
                fileData.errorText = "" + ex;
            }
            return fileData;
        },
        RemoveFileInfo: function(inputIndex, fileIndex) {
            var fileInfos = this.GetFileInfos(inputIndex);
            ASPx.Data.ArrayRemoveAt(fileInfos, fileIndex);
        },
        BuildChunkRequest: function (isNewUploading, settingsID, totalSize, inputIndex, fileIndex, fileSize, fileType, chunkSize, fileName, signature, fileData) {
            var index = fileIndex;
            var formData = new FormData();
            formData.append("IsNewUploading", isNewUploading ? "true" : "false");
            formData.append("SettingsID", settingsID);
            formData.append("TotalSize", totalSize);
            formData.append("FileIndex", index);
            formData.append("FileSize", fileSize);
            formData.append("FileType", fileType);
            formData.append("ChunkSize", chunkSize);
            formData.append("FileName", fileName);
            formData.append("Signature", signature);
            if(chunkSize)
                formData.append("Data", fileData);

            return formData;
        },
        BuildCancelRequest: function(settingsID, signature) {
            var formData = new FormData();
            formData.append("IsCancel", "true");
            formData.append("SettingsID", settingsID);
            formData.append("Signature", signature);
            return formData;
        },
        UpdateFileInfos: function(inputIndex) {
            var fileInputElement = this.uploadControl.GetFileInputElement(inputIndex);
        }
    });

    var ASPxUploadHelperSL = ASPx.CreateClass(ASPxUploadHelperStandardStrategy, {
        constructor: function(options) {
            this.constructor.prototype.constructor.call(this, options);
        },
        SetCursorStyle: function(inputIndex, cursorStyle) {
            if(this.IsHelperElementReady(inputIndex)) {
                var slElement = this.domHelper.GetSlUploadHelperElement(inputIndex);
                slElement.content.sl.SetCursorStyle(cursorStyle);
            }
        },
        ClearFileInfos: function(inputIndex) {
            if(this.IsHelperElementReady(inputIndex)) {
                var slElement = this.domHelper.GetSlUploadHelperElement(inputIndex);
                return slElement.content.sl.ClearFileInfos();
            }
        },
        GetErrorText: function(stringData) {
            var index = stringData.indexOf(Constants.ERROR_TEXT_RESPONSE_PREFIX) + Constants.ERROR_TEXT_RESPONSE_PREFIX.length;
            return stringData.substr(index);
        },
        GetFileInfos: function(inputIndex) {
            if(this.IsHelperElementReady(inputIndex)) {
                var slElement = this.domHelper.GetSlUploadHelperElement(inputIndex);
                var fileInfos = eval(slElement.content.sl.FileInfos);
                for(var i = 0, count = fileInfos.length; i < count; i++)
                    fileInfos[i].fileType = "";
                return fileInfos;
            }
            return [];
        },
        ReadBase64StringData: function(fileIndex, startPos, length, inputIndex) {
            if(this.IsHelperElementReady(inputIndex)) {
                var slElement = this.domHelper.GetSlUploadHelperElement(inputIndex);

                return slElement.content.sl.ReadBase64StringData(fileIndex, startPos, length);
            }
            return null;
        },
        ReadFileData: function(file, startPos, chunkLength, inputIndex, fileIndex) {
            var fileData = {};
            var encodedData = this.ReadBase64StringData(fileIndex, startPos, chunkLength, inputIndex);

            if(this.IsErrorOccurred(encodedData))
                fileData.errorText = this.GetErrorText(encodedData);
            else
                fileData.data = encodedData;
            return fileData;
        },
        IsErrorOccurred: function (stringData) {
            return stringData !== null ? stringData.indexOf(IdSuffixes.SL.ErrorTextResponsePrefix) != -1 : true;
        },
        RemoveFileInfo: function(inputIndex, fileIndex) {
            var slElement = this.domHelper.GetSlUploadHelperElement(inputIndex);
            return slElement.content.sl.RemoveFileInfo(fileIndex);
        },
        BuildChunkRequest: function(isNewUploading, settingsID, totalSize, inputIndex, fileIndex, fileSize, fileType, chunkSize, fileName, signature, fileData) {
            var index = fileIndex; // this.GetAbsoluteFileIndex(inputIndex, fileIndex);

            var request = "";
            request += "IsNewUploading:" + (isNewUploading ? "true" : "false") + "\r\n";
            request += "SettingsID:" + settingsID + "\r\n";
            request += "TotalSize:" + totalSize + "\r\n";
            request += "FileIndex:" + index + "\r\n";
            request += "FileSize:" + fileSize + "\r\n";
            request += "FileType:" + fileType + "\r\n";
            request += "ChunkSize:" + chunkSize + "\r\n";
            request += "FileName:" + fileName + "\r\n";
            request += "Signature:" + signature + "\r\n";
            request += "EncodingData:" + fileData;
            return request;
        },
        BuildCancelRequest: function(settingsID, signature) {
            var request = "";
            request += "IsCancel:true" + "\r\n";
            request += "SettingsID:" + settingsID + "\r\n";
            request += "Signature:" + signature;
            return request;
        },
        IsHelperElementReady: function(index) {
            return this.domHelper.IsSlObjectLoaded(index);
        }
    });

    var ASPxDOMHelper = ASPx.CreateClass(null, {
        constructor: function(options) {
            this.name = options.name;
            this.stateObject = options.stateObject;
            this.options = options;
            this.isMultiFileInput = false;
        },
        IsMouseOverElement: function(mouseEvt, element) {
            if(!element) return false;

            var x = ASPx.GetAbsoluteX(element);
            var y = ASPx.GetAbsoluteY(element);
            var w = element.offsetWidth;
            var h = element.offsetHeight;

            var eventX = ASPx.Evt.GetEventX(mouseEvt);
            var eventY = ASPx.Evt.GetEventY(mouseEvt);

            return (eventX >= x && eventX < (x + w) && eventY >= y && eventY < (y + h));
        },
        GetFileInputCountInternal: function() {
            return this.stateObject.inputCount;
        },
        IsSlObjectLoaded: function (inputIndex) {
            var slElement = this.GetSlUploadHelperElement(inputIndex);

            try {
                if(slElement) {
                    if(slElement.content) {
                        return !!slElement.content.sl;
                    }
                    else if(slElement.Content) {
                        return slElement.Content.sl;
                    }
                }
            }
            catch(e) { }

            return false;
        },
        GetFileInputSeparatorRow: function (index) {
            if(this.options.fileInputSpacing == "" || this.GetFileInputCountInternal() == 1)
                return null;

            return ASPx.GetNodesByPartialClassName(this.GetMainElement(), CSSClasses.SeparatorRow)[index || 0];
        },
        GetAddUploadButtonsSeparatorRow: function() {
            return this.GetChildElement(IdSuffixes.Input.AddButtonsSeparator);
        },
        GetAddUploadButtonsPanelRow: function() {
            return this.GetChildElement(IdSuffixes.Input.AddUploadButtonPanelRow);
        }
    });
    var ASPxClientUploadControlFilesUploadStartEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(cancel) {
            this.constructor.prototype.constructor.call(this);
            this.cancel = cancel;
        }
    });
    var ASPxClientUploadControlFileUploadCompleteEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(inputIndex, isValid, errorText, callbackData) {
            this.constructor.prototype.constructor.call(this);
            this.inputIndex = inputIndex;
            this.isValid = isValid;
            this.errorText = errorText;
            this.callbackData = callbackData;
        }
    });
    var ASPxClientUploadControlFilesUploadCompleteEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(errorText, callbackData) {
            this.constructor.prototype.constructor.call(this);
            this.errorText = errorText;
            this.callbackData = callbackData;
        }
    });
    var ASPxClientUploadControlTextChangedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(inputIndex) {
            this.constructor.prototype.constructor.call(this);
            this.inputIndex = inputIndex;
        }
    });
    var ASPxClientUploadControlUploadingProgressChangedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(fileCount, currentFileName, currentFileContentLength,
            currentFileUploadedContentLength, currentFileProgress, totalContentLength, uploadedContentLength, progress) {
            this.constructor.prototype.constructor.call(this);
            this.fileCount = fileCount;
            this.currentFileName = currentFileName;
            this.currentFileContentLength = currentFileContentLength;
            this.currentFileUploadedContentLength = currentFileUploadedContentLength;
            this.currentFileProgress = currentFileProgress;
            this.totalContentLength = totalContentLength;
            this.uploadedContentLength = uploadedContentLength;
            this.progress = progress;
        }
    });
    var ASPxClientUploadControlDropZoneEnterEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(dropZone) {
            this.constructor.prototype.constructor.call(this);
            this.dropZone = dropZone;
        }
    });
    var ASPxClientUploadControlDropZoneLeaveEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(dropZone) {
            this.constructor.prototype.constructor.call(this);
            this.dropZone = dropZone;
        }
    });
    var ASPxClientUploadControlDropZoneDropEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(dropZone) {
            this.constructor.prototype.constructor.call(this);
            this.dropZone = dropZone;
        }
    });

    var windowsFileNameRegExpTemplate = "^([a-zA-Z]\\:|\\\\\\\\[^\\/\\\\:*?\\\"<>|]+\\\\[^\\/\\\\:*?\\\"<>|]+)(\\\\[^\\/\\\\:*?\\\"<>|]+)+(\\.[^\\/\\\\:*?\\\"<>|]+)?$";
    var windowsRootDirectoryNameRegExpTemplate = "[a-zA-Z]\\:";
    ASPxClientUploadControl.IsValidWindowsFileName = function (fileName) {
        var windowsRootDirectoryNameRegExp = new RegExp(windowsRootDirectoryNameRegExpTemplate, "gi");
        var windowsFileNameRegExp = new RegExp(windowsFileNameRegExpTemplate, "gi");
        return (fileName == "" || windowsFileNameRegExp.test(fileName)) || (fileName.length == 3 && windowsRootDirectoryNameRegExp.test(fileName));
    };
    ASPxClientUploadControl.UploadManagerClass = ASPxLegacyUploadManager;
    ASPxClientUploadControl.IsDragAndDropAvailable = function() {
        var unsupportedIE = ASPx.Browser.IE && ASPx.Browser.MajorVersion < 10;
        return 'draggable' in document.createElement('span') && ASPxClientUploadControl.IsFileApiAvailable() && !unsupportedIE;
    };
    ASPxClientUploadControl.IsFileApiAvailable = function() {
        if(ASPxClientUploadControl.fileApiAvailable === undefined) {
            var input = document.createElement("input"),
                isBuggyAndroid = ASPx.Browser.AndroidDefaultBrowser && "webkitSlice" in window.File.prototype;

            ASPxClientUploadControl.fileApiAvailable = "multiple" in input && "File" in window && "FormData" in window;
            ASPxClientUploadControl.fileApiAvailable = ASPxClientUploadControl.fileApiAvailable && !isBuggyAndroid && ("slice" in window.File.prototype ||    // B232194
                ASPx.Browser.WebKitFamily && "webkitSlice" in window.File.prototype ||      // B239016
                ASPx.Browser.NetscapeFamily && "mozSlice" in window.File.prototype);
        }
        return ASPxClientUploadControl.fileApiAvailable;
    };
    ASPxClientUploadControl.OnTooManyFilesError = function(name) {
        var uploader = ASPx.GetControlCollection().Get(name);
        if(uploader != null)
            uploader.ShowTooManyFilesError();    // Q476327
    };
    ASPxClientUploadControl.Cast = ASPxClientControl.Cast;

    ASPx.SLOnLoad = function(name, index) {
        var uploader = ASPx.GetControlCollection().Get(name);
        if(uploader != null)
            uploader.OnPluginLoaded(index);
    }
    ASPx.SLOnError = function(name, index) {
        var uploader = ASPx.GetControlCollection().Get(name);
        if(uploader != null)
            uploader.OnPluginError(index);
    }
    ASPx.SLOnFileSelectionChanged = function(name, index) {
        var uploader = ASPx.GetControlCollection().Get(name);
        if(uploader != null)
            uploader.InvokeTextChangedInternal(index);
    };

    var ASPxClientUploadControlCollection = ASPx.CreateClass(ASPxClientControlCollection, {
        constructor: function () {
            this.constructor.prototype.constructor.call(this);

            this.dropZoneList = [];
            this.dropZoneRegistry = {};
            this.controlDropZonesRegistry = {};
            this.inlineZoneRegistry = {};
            this.anchorElements = [];
            this.anchorElementRegistry = {};
            this.zoneHandlersList = {};

            this.triggersSubscriptions = {};

            this.activeZoneId = null;
            this.dropEffect = "none";
            this.domHelper = new ASPxDOMHelper({ name: this.name });

            this.lastEventTimeStamp = 0;
            this.lastEventName = null;

            this.eventsInitialized = false;
            this.hasActiveDragFiles = false;
        },
        GetCollectionType: function(){
            return "Upload";
        },
        Remove: function(element) {
            this.unsubscribeControl(element.name);
            ASPxClientControlCollection.prototype.Remove.call(this, element);
        },
        RegisterDropZone: function(controlName, zoneId, isInline) {
            if(zoneId) {
                if(this.isRegistered(zoneId))
                    this.unsubscribeDropZone(zoneId, controlName);

                this.dropZoneList.push(zoneId);

                this.dropZoneRegistry[zoneId] = controlName;
                this.ensureDropZoneRegistered(controlName, zoneId);
                if(isInline)
                    this.inlineZoneRegistry[controlName] = zoneId;

                this.subscribeDropZone(zoneId, controlName);
            }
        },
        DeregisterDropZones: function(controlName, zoneIDs) {
            if(!(zoneIDs && zoneIDs.length)) return;

            ASPx.Data.ForEach(zoneIDs, function(zoneId) {
                this.unsubscribeDropZone(zoneId, controlName);
                var pos = this.controlDropZonesRegistry[controlName].indexOf(zoneId);
                if(pos > -1)
                    this.controlDropZonesRegistry[controlName].splice(pos, 1);
            }.aspxBind(this));
        },
        ensureDropZoneRegistered: function(controlName, zoneId) {
            if(!this.controlDropZonesRegistry[controlName])
                this.controlDropZonesRegistry[controlName] = [];

            if(this.controlDropZonesRegistry[controlName].indexOf(zoneId) === -1)
                this.controlDropZonesRegistry[controlName].push(zoneId);
        },
        isRegistered: function(zoneId) {
            return this.dropZoneList.indexOf(zoneId) !== -1;
        },
        RegisterAnchorElement: function(controlName, anchor) {
            this.anchorElements.push(anchor.id);
            this.anchorElementRegistry[anchor.id] = controlName;

            this.subscribeAnchorElement(anchor);
        },
        DeregisterAnchorElement: function(anchor) {
            if(anchor) {
                if(anchor.id) {
                    this.anchorElements.splice(this.anchorElements.indexOf(anchor.id), 1);
                    delete this.anchorElementRegistry[anchor.id];
                }

                ASPx.Evt.DetachEventFromElement(anchor, "dragleave");
            }
        },
        subscribeAnchorElement: function(anchor) {
            var controlName = this.anchorElementRegistry[anchor.id],
                inlineZoneId = this.inlineZoneRegistry[controlName];

            ASPx.Evt.AttachEventToElement(anchor, "dragleave", function(e) {
                this.onDropZoneLeave(e, inlineZoneId);
            }.aspxBind(this));
        },
        subscribeDropZone: function(zoneId) {
            var dropZone = document.getElementById(zoneId),
                handlerIndex = zoneId,
                that = this;

            if(dropZone) {
                var dropHandler = function(e) {
                    that.onDrop(e, zoneId);
                };
                var dragLeaveHandler = function(e) {
                    that.onDropZoneLeave(e, zoneId);
                };

                this.zoneHandlersList[handlerIndex] = {
                    drop: dropHandler,
                    dragleave: dragLeaveHandler
                };

                ASPx.Evt.AttachEventToElement(dropZone, "drop", this.zoneHandlersList[handlerIndex].drop);
                ASPx.Evt.AttachEventToElement(dropZone, "dragleave", this.zoneHandlersList[handlerIndex].dragleave);
            }
        },
        unsubscribeDropZone: function(zoneId) {
            var dropZone = document.getElementById(zoneId),
                handlerIndex = zoneId;

            if(dropZone) {
                ASPx.Evt.DetachEventFromElement(dropZone, "drop", this.zoneHandlersList[handlerIndex].drop);
                ASPx.Evt.DetachEventFromElement(dropZone, "dragleave", this.zoneHandlersList[handlerIndex].dragleave);

                this.dropZoneList.splice(this.dropZoneList.indexOf(zoneId), 1);
                delete this.zoneHandlersList[handlerIndex];
            }
        },
        setDropEffect: function(e, effect) {
            e.dataTransfer.dropEffect = effect;
        },
        isMouseOverElement: function(e, element) {
            if(!element)
                return false;

            var x = this.getElementAbsoluteX(element),
                w = element.offsetWidth,
                eventX = ASPx.Evt.GetEventX(e);

            if(eventX < x || eventX >= (x + w))
                return false;

            var y = this.getElementAbsoluteY(element),
                h = element.offsetHeight,
                eventY = ASPx.Evt.GetEventY(e);

            return (eventY >= y && eventY < (y + h));
        },
        getElementAbsoluteX: function(element) {
            if(ASPx.Browser.WebKitFamily)
                return Math.round(element.getBoundingClientRect().left + ASPx.GetDocumentScrollLeft());
            else
                return ASPx.GetAbsoluteX(element);
        },
        getElementAbsoluteY: function(element) {
            if(ASPx.Browser.WebKitFamily)
                return Math.round(element.getBoundingClientRect().top + ASPx.GetDocumentScrollTop());
            else
                return ASPx.GetAbsoluteY(element);
        },
        isActiveZoneChanged: function(e, zoneId) {
            var zoneChanged = this.activeZoneId === null || this.activeZoneId !== zoneId;
            if(this.activeZoneId !== null) {
                zoneChanged = this.isMouseLeftActiveZone(e);
            }

            return zoneChanged;
        },
        isMouseLeftActiveZone: function(e) {
            var activeZone = document.getElementById(this.activeZoneId);

            return !this.isMouseOverElement(e, activeZone);
        },
        isInstanceAlive: function(controlName) {
            return this.elements[controlName] && this.elements[controlName].GetMainElement();
        },
        getElementUnderPointerFromList: function(e, list) {
            var ret = null;

            list = list.filter(function (id) {
                return document.getElementById(id) !== null;
            });

            for(var i = 0; i < list.length; i++) {
                var element = document.getElementById(list[i]);
                if(this.isMouseOverElement(e, element)) {
                    ret = element;

                    break;
                }
            }

            return ret;
        },
        OnDocumentMouseUp: function (e) {
            this.ForEachControl(function (control) {
                if(control.IsDOMInitialized())
                    control.OnDocumentMouseUp();
            });
        },
        unsubscribeControl: function(controlName) {
            this.DeregisterDropZones(controlName, this.controlDropZonesRegistry[controlName]);
            delete this.controlDropZonesRegistry[controlName];

            this.detachTriggersFromControl(controlName);
        },
        proxyEvent: function(controlName, handler) {
            if(this.isInstanceAlive(controlName))
                handler.call(this);
            else
                this.unsubscribeControl(controlName);
        },
        proxyTriggerEvent: function(controlName, handler) {
            if(!this.hasActiveDragFiles)
                this.proxyEvent(controlName, handler);
        },
        OnDocumentDragEnter: function(e) {
            ASPx.Evt.PreventEvent(e);

            if(ASPxClientUploadControl.isValidDragAndDropEvent(e)) {
                var anchorElement = this.getElementUnderPointerFromList(e, this.anchorElements),
                    zone,
                    zoneId,
                    controlName;

                if(anchorElement) {
                    controlName = this.anchorElementRegistry[anchorElement.id];
                    zoneId = this.inlineZoneRegistry[controlName];
                }
                else {
                    zone = this.getElementUnderPointerFromList(e, this.dropZoneList);
                    if(zone) {
                        zoneId = zone.id;
                        controlName = this.dropZoneRegistry[zoneId];
                    }
                }

                if(controlName && zoneId)
                    this.onDragEnter(e, controlName, zoneId);

            } else
                this.dropEffect = "none";
        },
        onDragEnter: function(e, controlName, zoneId) {
            this.lastEventName = "dragenter";

            if(this.isActiveZoneChanged(e, zoneId)) {
                if(this.activeZoneId)
                    this.onDropZoneLeave(e, this.activeZoneId);

                if(this.activeZoneId !== zoneId) {
                    this.proxyEvent(controlName, function() {
                        this.passDragEnterToControl(e, controlName, zoneId);
                    });
                }
            }
        },
        passDragEnterToControl: function(e, controlName, zoneId) {
            if(!this.elements[controlName].isInCallback) {
                this.elements[controlName].OnDragEnter([e, zoneId]);
                this.activeZoneId = zoneId;
                this.dropEffect = "copy";
            }
            else
                this.dropEffect = "none";
        },
        onDropZoneLeave: function(e, zoneId) {
            var zone = document.getElementById(zoneId),
                controlName = this.dropZoneRegistry[zoneId];

            this.hasActiveDragFiles = false;

            if(this.shouldRaiseDragLeave(e, zoneId, zone)) {
                this.proxyEvent(controlName, function() {
                    this.elements[controlName].OnDragLeave([e, zoneId]);
                });

                this.activeZoneId = null;
                this.dropEffect = "none";
            }

            ASPx.Evt.PreventEvent(e);
        },
        shouldRaiseDragLeave: function(e, zoneId, zone) {
            return this.activeZoneId === zoneId && (this.isActiveZoneLeft(e, zone) || this.dragCancelledByEscKey());
        },
        isActiveZoneLeft: function(e, zone) {
            return !this.isMouseOverElement(e, zone);
        },
        dragCancelledByEscKey: function() {
            // When the 'esc' key was hit, the "dragenter" event would not be fired right before the "dragleave"
            return this.lastEventName !== "dragenter";
        },
        onDrop: function(e, zoneId) {
            var controlName = this.dropZoneRegistry[zoneId];
            this.hasActiveDragFiles = false;

            ASPx.Evt.PreventEvent(e);

            if(ASPxClientUploadControl.isValidDragAndDropEvent(e) && this.lastEventTimeStamp !== e.timeStamp) {
                this.proxyEvent(controlName, function() {
                    this.elements[controlName].OnDrop([e, zoneId]);
                });

                this.activeZoneId = null;

                this.lastEventTimeStamp = e.timeStamp;
            }
        },
        OnDocumentDragOver: function(e) {
            this.lastEventName = "dragover";
            this.setDropEffect(e, this.dropEffect);

            ASPx.Evt.PreventEvent(e);
        },
        initializeEvents: function() {
            var that = this;

            if(!this.eventsInitialized) {
                ASPx.Evt.AttachEventToDocument("dragenter", function(e) {
                    that.hasActiveDragFiles = true;

                    that.OnDocumentDragEnter(e);
                });
                ASPx.Evt.AttachEventToDocument("dragover", function(e) {
                    that.OnDocumentDragOver(e);
                });

                this.eventsInitialized = true;
            }
        },
        SubscribeDialogTriggers: function(context, controlName, dialogTriggersList, triggerHandlers, attach) {
            var config = {
                controlName: controlName,
                dialogTriggersList: dialogTriggersList,
                triggerHandlers: triggerHandlers,
                context: context
            };

            this.detachTriggersFromControl(config.controlName);
            if(attach)
                this.attachTriggersToControl(config);
        },
        attachTriggersToControl: function(config) {
            this.triggersSubscriptions[config.controlName] = {};

            ASPx.Data.ForEach(config.dialogTriggersList, function(trigger) {
                this.subscribeTrigger(config.controlName, trigger, config.triggerHandlers, config.context);
            }.aspxBind(this));
        },
        detachTriggersFromControl: function(controlName) {
            var controlSubscriptions = this.triggersSubscriptions[controlName],
                triggerIDs = ASPx.GetObjectKeys(controlSubscriptions);

            ASPx.Data.ForEach(triggerIDs, function(triggerID) {
                var triggerSubscriptions = controlSubscriptions[triggerID];
                if(triggerSubscriptions) {
                    var eventNames = ASPx.GetObjectKeys(triggerSubscriptions);
                    ASPx.Data.ForEach(eventNames, function(eventName) {
                        ASPx.Data.ForEach(triggerSubscriptions[eventName], function(subscription) {
                            ASPx.Evt.DetachEventFromElement(subscription.target, eventName, subscription.handler);
                        });
                        triggerSubscriptions[eventName] = {};
                    });
                }
            });
        },
        subscribeTrigger: function(controlName, trigger, triggerHandlers, context) {
            var triggerSubscriptions =  this.triggersSubscriptions[controlName][trigger.id] = {},
                eventNames = ASPx.GetObjectKeys(triggerHandlers),
                that = this;

            ASPx.Data.ForEach(eventNames, function(eventName) {
                triggerSubscriptions[eventName] = triggerSubscriptions[eventName] || [];

                ASPx.Data.ForEach(triggerHandlers[eventName], function(config) {
                    var handler = function(e) {
                        that.proxyTriggerEvent(controlName, function() {
                            config.handler.call(context, e, trigger);
                        });
                    };
                    
                    triggerSubscriptions[eventName].push({
                        handler: handler,
                        target: config.target || trigger
                    });
                });

                ASPx.Data.ForEach(triggerSubscriptions[eventName], function(subscription) {
                    ASPx.Evt.AttachEventToElement(subscription.target, eventName, subscription.handler);
                });
            }.aspxBind(this));
        }
    });

    // Document event handlers
    var uploadControlCollection = null;
    function aspxGetUploadControlCollection() {
        if(uploadControlCollection == null)
            uploadControlCollection = new ASPxClientUploadControlCollection();
        return uploadControlCollection;
    }

    ASPx.Evt.AttachEventToDocument("mouseup", function(e) {
        aspxGetUploadControlCollection().OnDocumentMouseUp(e);
    });

    var ASPxFileListChangedInternalArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(fileInfos, inputIndex, fileCountChanged) {
            this.fileInfos = fileInfos;
            this.inputIndex = inputIndex;
            this.fileCountChanged = fileCountChanged;
        }
    });

    var ASPxViewStateChangedInternalArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(fileInfos, inputIndex) {
            this.constructor.prototype.constructor.call(this);
            this.fileInfos = fileInfos;
            this.inputIndex = inputIndex;
        }
    });

    window.ASPxClientUploadControl = ASPxClientUploadControl;
    window.ASPxClientUploadControlCollection = ASPxClientUploadControlCollection;
    window.ASPxClientUploadControlFilesUploadStartEventArgs = ASPxClientUploadControlFilesUploadStartEventArgs;
    window.ASPxClientUploadControlFileUploadCompleteEventArgs = ASPxClientUploadControlFileUploadCompleteEventArgs;
    window.ASPxClientUploadControlFilesUploadCompleteEventArgs = ASPxClientUploadControlFilesUploadCompleteEventArgs;
    window.ASPxClientUploadControlTextChangedEventArgs = ASPxClientUploadControlTextChangedEventArgs;
    window.ASPxClientUploadControlUploadingProgressChangedEventArgs = ASPxClientUploadControlUploadingProgressChangedEventArgs;
})();