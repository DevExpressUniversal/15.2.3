(function() {
    var IDConstants = {};
    IDConstants.PageControl = "_PageControl";
    IDConstants.GalleryFileManager = IDConstants.PageControl + "_FileManager";
    IDConstants.UploadFormLayout = IDConstants.PageControl + "_UploadFormLayout";
    IDConstants.UploadControl = IDConstants.UploadFormLayout + "_UploadControl";
    IDConstants.UploadPreviewPanel = IDConstants.UploadFormLayout + "_UploadPreviewPanel";
    IDConstants.UploadCancelButton = IDConstants.UploadFormLayout + "_UploadCancelButton";
    IDConstants.UrlFormLayout = IDConstants.PageControl + "_UrlFormLayout";
    IDConstants.UrlTextBox = IDConstants.UrlFormLayout + "_UrlTextBox";
    IDConstants.UrlCheckBox = IDConstants.UrlFormLayout + "_UrlCheckBox";
    IDConstants.UrlPreviewPanel = IDConstants.UrlFormLayout + "_UrlPreviewPanel";
    
    var CssClasses = {
        InUpload: "dxic-inUpload"    
    };
    
    var PreviewType = {
        NotSpecified: -1,
        Audio: 1,
        Object: 2,  
        Image: 3,
        Video: 4
    };

    var PageName = {
        FromUrl: "FromURL",
        Upload: "UploadTab",
        FromGallery: "FromGallery"
    };

    var FilePreview = ASPx.CreateClass(null, {
        constructor: function(options) {
            this.previewPanel = options.previewPanel;
            this.callbackFunc = options.onReadyCallback;
            this.currentElement = null;
        },
        collapse: function() {
            if(this.currentElement)
                this.currentElement.style.display = "none";
        },
        expand: function() {
            if(this.currentElement) {
                var contentWrapper = this.previewPanel.getContentElement();
                var size =  { 
                    width: contentWrapper.offsetWidth, 
                    height: contentWrapper.offsetHeight
                };
                this.setElementSize(this.currentElement, size);
                this.currentElement.style.display = "";
            }
        },
        updatePreviewStyles: function(styleSettings) {
            if(!this.currentElement)
                return;
            if(styleSettings.width && styleSettings.height)
            this.rescaleElementSize(this.currentElement, styleSettings.width, styleSettings.height);
            ASPx.SetStyles(this.currentElement, {
                borderColor: styleSettings.borderColor || "",
                borderWidth: styleSettings.borderWidth || "",
                borderStyle: styleSettings.borderStyle || ""
            });
        },
        rescaleElementSize: function(element, newWidth, newHeight) {
        },
        releaseResources: function() {
            if(!this.currentElement)
                return;
            var parent = this.currentElement.parentNode;
            while(parent.childNodes.length)
                parent.removeChild(parent.firstChild);
            this.restorePreviewPanel();
        },
        restorePreviewPanel: function() {
            this.currentElement = null;
            if(this.previewPanel.cachedHtml)
                this.previewPanel.SetContentHtml(this.previewPanel.cachedHtml);    
        },
        setURL: function(url) {
            if(url) {
                var element = document.createElement(this.getTagName());
                this.prepareElement(element, url);
                if(!this.previewPanel.cachedHtml)
                    this.previewPanel.cachedHtml = this.previewPanel.GetContentHtml();
                var contentWrapper = this.previewPanel.getContentElement();
                contentWrapper.innerHTML = "";
                var size =  { 
                    width: contentWrapper.offsetWidth, 
                    height: contentWrapper.offsetHeight
                };
                contentWrapper.appendChild(element);
                this.setElementSize(element, size);
            } else 
                this.restorePreviewPanel();
        },
        prepareElement: function(element, url) {
            this.currentElement = element;
            if(this.callbackFunc)
                ASPx.Evt.AttachEventToElement(element, "load", this.callbackFunc);
            ASPx.Attr.SetAttribute(element, this.getSrcAttribute(), url);
            ASPx.SetStyles(element, {
                boxSizing: "border-box",
                border: 0,
                borderColor: "transparent",
                borderStyle: "none"
            });    
        },
        setElementSize: function(element, size) {
            ASPx.SetStyles(element, size);
        },
        getTagName: function() {
            return "IFRAME";    
        },
        getSrcAttribute: function() {
            return "src";    
        }
    });
    var ImagePreview = ASPx.CreateClass(FilePreview, {
        collapse: function() {
            this.releaseResources();
        },
        expand: function() {
            this.setURL(this.src);
        },
        setElementSize: function (element, options) {
            this.src = element.src;
            options.canUseCanvas = false;
            ASPx.ImageControlUtils.ResizeImage(element, options); //T249223
            options.onEndResize = function (element) {
                this.currentElement = element;
            }.aspxBind(this);
            if(!ASPx.ImageControlUtils.IsImageLoaded(element)) {
                ASPx.Evt.AttachEventToElement(element, "load", function(evt) {
                    this.resizeImage(element, options);
                }.aspxBind(this));
            } else
                this.resizeImage(element, options);
        },
        getTagName: function() {
            return "IMG";    
        },
        resizeImage: function(element, options) {
            ASPx.ImageControlUtils.ResizeImage(element, options);
            if(ASPx.PxToInt(element.style.marginTop) > 0)
                element.style.marginTop = "";
            if(ASPx.PxToInt(element.style.marginBottom) > 0)
                element.style.marginBottom = "";
        }
    });
    var HTML5MediaPreview = ASPx.CreateClass(FilePreview, {
        prepareElement: function(element, url) {
            FilePreview.prototype.prepareElement.call(this, element, url);
            ASPx.Attr.SetAttribute(element, "controls", "controls");
        }
    });
    var AudioPreview = ASPx.CreateClass(HTML5MediaPreview, {
        getTagName: function() {
            return "AUDIO";    
        }
    });
    var VideoPreview = ASPx.CreateClass(HTML5MediaPreview, {
        getTagName: function() {
            return "VIDEO";    
        }
    });
    var ObjectPreview = ASPx.CreateClass(FilePreview, {
        getTagName: function() {
            return "OBJECT";    
        },
        getSrcAttribute: function() {
            return "data";    
        }
    });
    FilePreview.Create = function(options) {
        switch(options.type) {
            case PreviewType.Audio:
                return new AudioPreview(options);
            case PreviewType.Video:
                return new VideoPreview(options);
            case PreviewType.Image:
                return new ImagePreview(options);
            case PreviewType.Object:
                return new ObjectPreview(options);
            case PreviewType.NotSpecified:
                return new FilePreview(options);
        }
    };

    var MediaFileSelector = ASPx.CreateClass(ASPxClientControl, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.cache = {};
            this.isInUpload = false;
            this.currentURL = null;
            this.previewType = -1;

            this.RequestInsert = new ASPxClientEvent();
            this.BeforePreviewUpdate = new ASPxClientEvent();
            this.PreviewElementLoaded = new ASPxClientEvent();
            this.CurrentURLChanged = new ASPxClientEvent();
        },
        AdjustControl: function() {
            this.beginAdjust();
            var mainElement = this.GetMainElement();
            var fileManager = this.getFileManager();
            if(fileManager) {
                var width = this.getClearWidth(fileManager.GetMainElement(), mainElement, 0);
                fileManager.SetWidth(width);
            }
            ASPx.GetControlCollection().AdjustControlsCore(mainElement, true);
            this.endAdjust();
        },
        getClearWidth: function(element, finalNode, widthDelta) {
            if(element === finalNode)
                return element.offsetWidth - widthDelta;
            return this.getClearWidth(element.parentNode, finalNode, widthDelta + ASPx.GetLeftRightBordersAndPaddingsSummaryValue(element));
        },
        beginAdjust: function() {
            if(this.hasCurrentPreview())
                this.getCurrentPreview().collapse();   
        },
        endAdjust: function() {
            if(this.hasCurrentPreview())
                this.getCurrentPreview().expand();    
        },
        InlineInitialize: function() {
            ASPxClientControl.prototype.InlineInitialize.call(this);
            var pageControl = this.getPageControl();
            pageControl.ActiveTabChanged.AddHandler(function(s, e) {
                this.SetURL(this.currentURL);
                this.clearErrors();
            }.aspxBind(this));
            pageControl.ActiveTabChanging.AddHandler(this.ReleaseResources.aspxBind(this));
            this.initializeFromUrlSection(!!this.getTabPage(PageName.FromUrl));
            this.initializeUploadSection(!!this.getTabPage(PageName.Upload));
            this.initializeFromGallerySection(!!this.getTabPage(PageName.FromGallery));
        },
        GetURLTextBox: function() {
            return this.getInnerControl(IDConstants.UrlTextBox);    
        },
        GetURL: function() {
            return this.getURLInternal();
        },
        getURLInternal: function() {
            switch(this.getActiveTabName()) {
                case PageName.FromGallery:
                    return this.getFileManagerSelecteFileURL();
                case PageName.FromUrl:
                    return this.currentURL;
                case PageName.Upload:
                    return this.currentURL;
            }
        },
        SetURL: function(url, switchToURLTab) {
            this.currentURL = url;
            switch(this.getActiveTabName()) {
                case PageName.FromUrl:
                    this.updatePreview(url);
                    this.GetURLTextBox().SetText(url);
                    break;
                case PageName.Upload:
                    this.updatePreview(url);
                    if(url)
                        this.getInnerControl(IDConstants.UploadControl).SetCustomText(url);
                    else
                        this.getInnerControl(IDConstants.UploadControl).ClearText();
                    break;
            }
        },
        UpdatePreviewStyles: function(styleSettings) {
            if(this.hasCurrentPreview())
                this.getCurrentPreview().updatePreviewStyles(styleSettings);
        },
        IsInUpload: function() {
            return this.isInUpload;    
        },
        TryCancelUpload: function() {
            if(!this.IsInUpload())
                return;
            switch(this.getActiveTabName()) {
                case PageName.Upload:
                    this.getInnerControl(IDConstants.UploadControl).Cancel();        
                    break;
                case PageName.FromGallery:
                    this.getFileManager().GetUploadControl().Cancel();
                    break;
            }
            this.onUpload(true);
        },
        NeedSaveToServer: function() {
            if(this.getActiveTabName() != PageName.FromUrl)
                return false;
            var checkBox = this.getInnerControl(IDConstants.UrlCheckBox);
            return !!checkBox && checkBox.GetChecked();
        },
        ReleaseResources: function() {
            if(this.hasCurrentPreview()) {
                this.getCurrentPreview().releaseResources();
                this.setCurrentPreview(null);
            }
        },
        IsValid: function() {
            switch(this.getActiveTabName()) {
                case PageName.Upload:
                    var uploadControl = this.getInnerControl(IDConstants.UploadControl);
                    var isValid = !!this.currentURL;
                    uploadControl.viewManager.errorView.UpdateCommonErrorDiv(isValid ? "" : uploadControl.generalErrorText);
                    return isValid;
                case PageName.FromGallery:
                    var fileManager = this.getFileManager();
                    var isValid = !!fileManager.GetSelectedFile();
                    if(!isValid)
                        alert(fileManager.cpRequiredErrorText);
                    return isValid;
                case PageName.FromUrl:
                    var textBox = this.GetURLTextBox();
                    textBox.Validate();
                    return textBox.GetIsValid();
                default:
                    return false;
            }
        },
        getFileManagerSelecteFileURL: function() {
            var fileManager = this.getFileManager();
            var file = fileManager.GetSelectedFile();
            return file ? (fileManager.cp_RootFolderRelativePath + fileManager.GetSelectedFile().GetFullName("\/", true)) : null;
        },
        clearErrors: function() {
            switch(this.getActiveTabName()) {
                case PageName.Upload:
                    this.getInnerControl(IDConstants.UploadControl).viewManager.errorView.UpdateCommonErrorDiv("");
                    break;
                case PageName.FromGallery:
                    break;
                case PageName.FromUrl:
                    this.GetURLTextBox().SetIsValid(true);
                    break;
            }        
        },
        initializeFromUrlSection: function(tabExists) {
            if(!tabExists)
                return;
            var textBox = this.GetURLTextBox();
            var inputElement = textBox.GetInputElement();
            ASPx.Evt.AttachEventToElement(inputElement, "paste", function(evt) {
                setTimeout(function() {
                    textBox.SyncRawValue();
                    this.onUrlTextBoxValueChanged(textBox);
                }.aspxBind(this), 0);
                return evt;
            }.aspxBind(this));
            textBox.KeyUp.AddHandler(function(s, e) {
                this.onUrlTextBoxValueChanged(s);
            }.aspxBind(this));
        },
        onUrlTextBoxValueChanged: function(textBox) {
            var url = textBox.GetText();
            if(url != this.currentURL) {
                var oldIsValidValue = textBox.GetIsValid();
                textBox.ValidateWithPatterns();
                var checkBox = this.getInnerControl(IDConstants.UrlCheckBox);
                if(checkBox) {
                    var isExternalUrl = MediaFileSelector.isExternalURL(url, this.appDomainPath, window.location.hostname);
                    checkBox.SetEnabled(isExternalUrl);
                    checkBox.SetChecked(isExternalUrl && checkBox.GetChecked());
                }
                if(textBox.GetIsValid() || !url) {
                    this.setCurrentURL(url);
                    this.updatePreview(url);
                }
                textBox.SetIsValid(oldIsValidValue);
            }   
        },
        initializeFromGallerySection: function(tabExists) {
            if(!tabExists)
                return;
            var fileManager = this.getFileManager();
            fileManager.FilesUploading.AddHandler(function(s, e) {
                this.onUpload(false);    
            }.aspxBind(this));
            fileManager.FilesUploaded.AddHandler(function(s, e) {
                this.onUpload(true);    
            }.aspxBind(this));
            fileManager.SelectedFileOpened.AddHandler(function(s, e) {
                if(!this.RequestInsert.IsEmpty())
                    this.RequestInsert.FireEvent(this, null);
            }.aspxBind(this));
            fileManager.SelectedFileChanged.AddHandler(function(s, e) {
                this.setCurrentURL(this.getFileManagerSelecteFileURL());
            }.aspxBind(this));
            this.initializeUploadControl(fileManager.GetUploadControl());
        },
        initializeUploadSection: function(tabExists) {
            if(!tabExists)
                return;
            var uploadControl = this.getInnerControl(IDConstants.UploadControl);
            if(this.previewType != PreviewType.Image && uploadControl.dialogTriggerIDList && uploadControl.dialogTriggerIDList.length) {
                this.BeforePreviewUpdate.AddHandler(function(s, e) {
                    if(uploadControl.IsVisible())
                        uploadControl.SetDialogTriggerID(e.url ? "" : uploadControl.dialogTriggerIDList[0]);
                });
            }
            this.initializeUploadControl(uploadControl, true);
            uploadControl.FilesUploadStart.AddHandler(function(s, e) {
                this.getInnerControl(IDConstants.UploadCancelButton).SetVisible(true);
                this.onUpload(false);
            }.aspxBind(this));
            uploadControl.FileUploadComplete.AddHandler(function(s, e) {
                s.recentUploadedURL = e.callbackData;
                this.setCurrentURL(e.callbackData);
                this.updatePreview(e.callbackData);
            }.aspxBind(this));
        },
        initializeUploadControl: function(uploadControl, isUploadSection) {
            if(!uploadControl)
                return;
            uploadControl.FilesUploadComplete.AddHandler(function(s, e) {
                if(isUploadSection) {
                    if(s.recentUploadedURL)
                        s.SetCustomText(s.recentUploadedURL);
                    this.getInnerControl(IDConstants.UploadCancelButton).SetVisible(false);
                }
                this.onUpload(true);    
            }.aspxBind(this));
        },
        onUpload: function(completed) {
            this.getPageControl().SetEnabled(completed);
            this.isInUpload = !completed;    
        },
        updatePreview: function(url) {
            var panel = this.getCurrentPreviewPanel();
            if(!panel)
                return;
            var args = { 
                url: url,
                createOptions: {
                    previewPanel: panel,
                    onReadyCallback: null,
                    type: this.previewType
                }
            };
            if(!this.BeforePreviewUpdate.IsEmpty())
                this.BeforePreviewUpdate.FireEvent(this, args);
            var filePreview = FilePreview.Create(args.createOptions);
            filePreview.setURL(args.url);
            this.setCurrentPreview(filePreview);
        },
        setCurrentURL: function(url) {
            this.currentURL = url;
            if(!this.CurrentURLChanged.IsEmpty())
                this.CurrentURLChanged.FireEvent(this);
        },
        setCurrentPreview: function(preview) {
            this.cache[this.getActiveTabName()] = preview;    
        },
        getCurrentPreview: function() {
            return this.cache[this.getActiveTabName()];
        },
        hasCurrentPreview: function() {
            return !!this.getCurrentPreview();    
        },
        getCurrentPreviewPanel: function() {
            switch(this.getActiveTabName()) {
                case PageName.FromUrl:
                    return this.getInnerControl(IDConstants.UrlPreviewPanel);
                case PageName.Upload:
                    return this.getInnerControl(IDConstants.UploadPreviewPanel);
            }
        },
        getActiveTabName: function() {
            return this.getPageControl().GetActiveTab().name;    
        },
        getTabPage: function(name) {
            return this.getPageControl().GetTabByName(name);
        },
        getPageControl: function() {
            return this.getInnerControl(IDConstants.PageControl);
        },
        getFileManager: function() {
            return this.getInnerControl(IDConstants.GalleryFileManager);    
        },
        getInnerControl: function(suffix) {
            if(!this.cache[suffix])
                this.cache[suffix] = ASPx.GetControlCollection().Get(this.name + suffix);
            return this.cache[suffix];
        }
    });
    MediaFileSelector.isExternalURL = function(url, appPath, host) { 
        host = host || window.location.hostname;
        if(url) {
            var regexpTest = [
                { exp: "^data:[a-zA-Z0-9]+\\/[a-zA-Z0-9]+;base64,.*", ret: false},
                { exp: "^(https?:|ftps?:|^)\\/\\/(?!"+ (host + appPath).split("/").join("\\/") +").*", ret: true }
            ];
            for(var i = 0; i < regexpTest.length; i++) {
                if((new RegExp(regexpTest[i].exp)).test(url))
                    return regexpTest[i].ret;
            }
        }
        return false;
    };

    ASPx.MediaFileSelector = MediaFileSelector;
})();