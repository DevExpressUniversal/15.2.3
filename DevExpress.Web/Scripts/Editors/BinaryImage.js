/// <reference path="..\_references.js"/>

(function() {
        var CONST = {
        POSTFIX: {
            BUTTON_PANEL: "DXButtonPanel",
            BUTTONS_CONTAINER: "DXButtonsContainer",
            BUTTONS_SHADER: "DXButtonsShader",
            DISABLED_COVER: "DXDisabledCover",
            CANCEL_BUTTON: "DXCancelButton",
            DELETE_BUTTON: "DXDeleteButton",
            DROP_ZONE: "DXDropZone",
            EMPTY_VALUE_PLACEHOLDER: "EVP",
            EMPTY_VALUE_IMAGE: "EVI",
            IMAGE_CONTAINER: "DXImageContainer",
            OPEN_DIALOG_BUTTON: "DXOpenDialogButton",          
            PREVIEW_CONTROL: "DXPreview",
            PROGRESS_BAR: "DXProgressBar",
            PROGRESS_PANEL: "DXProgressPanel",
            UPLOAD_CONTROL: "DXUploadEditor",                      
            VALUE_KEY_INPUT: "DXValueKeyInput"
        },
        CALLBACK_PREFIX: {
            CUSTOM_CALLBACK: "BICC",
        }
    };
    /* Enums */
    var ButtonPanelVisibilityEnum = {
        None: 0,
        Faded: 1,
        OnMouseOver: 2,
        Always: 3
    };

    var getChildElement = function(editor, postfix) {
        var name = editor ? editor.name || editor.id : '';
        var id = name + '_' + postfix;
        return document.getElementById(id);
    };
    var getChildControl = function(editor, postfix) {
        var name = editor ? editor.name : '';
        return ASPx.GetControlCollection().Get(editor.name + '_' + postfix);
    };
    var formatCallbackArg = function(prefix, arg) { //TODO rm duplicate
        arg = arg ? arg.toString() : '';
        return prefix + "|" + arg.length + ';' + arg + ';';
    };
    var ASPxClientBinaryImage = ASPx.CreateClass(ASPxClientEdit, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.Click = new ASPxClientEvent();

            this.imageWidth = "";
            this.imageHeight = "";

            this.buttonPanelVisibility = null;
            this.enableEditing = false;
            this.imageContainerSizeIsValid = false;
            this.emptyImageContainerSizeIsValid = false;
            this.showLoadingImage = false;
            this.loadingImageData = null;
            this.isASPxClientBinaryImage = true;
            //events
            this.onDeleteButtonClick = this.OnDeleteButtonClick.aspxBind(this);
            this.onPreviewContainerTouchUp = this.OnPreviewContainerTouchUp.aspxBind(this);
            this.onCancelButtonClick = this.OnCancelButtonClick.aspxBind(this);
        },
        Initialize: function() {
            ASPxClientEdit.prototype.Initialize.call(this);
            if(this.enableEditing) {
                this.ShowValueDisplayElement();
                this.InitializeButtonPanel();
                this.InitializeProgressPanel();
                this.InitializeUploadControl();
            }
        },
        InitializeButtonPanel: function() {
            var buttonPanel = this.GetButtonPanel();
            if(!buttonPanel)
                return;
            if(this.buttonPanelVisibility != ButtonPanelVisibilityEnum.Always) {
                this.UpdateButtonPanelVisibility();
                this.InitializePreviewContainerEvents();
            }
        },
        InitializePreviewContainerEvents: function() {
            if(ASPx.Browser.TouchUI) 
                this.InitializePreviewContainerTouchEvents();
            else
                this.InitializePreviewContainerMouseEvents();
        },
        InitializePreviewContainerMouseEvents: function() {//TODO ren
            ASPx.Evt.AttachMouseEnterToElement(this.GetMainElement(), this.OnPreviewContainerMouseOver.aspxBind(this), this.OnPreviewContainerMouseOut.aspxBind(this));
        },
        InitializePreviewContainerTouchEvents: function() {
            ASPx.Evt.AttachEventToElement(this.GetMainElement(),ASPx.TouchUIHelper.touchMouseUpEventName, this.onPreviewContainerTouchUp);
            // TODO Touch events or show command panel by click
        },
        InitializeProgressPanel: function() {
            this.InitializeProgressPanelEvents();
        },
        InitializeProgressPanelEvents: function() {
            cancelButton = this.GetCancelButton();
            if(cancelButton)
                ASPx.Evt.AttachEventToElement(cancelButton, "click", this.onCancelButtonClick);
        },
        InitializeUploadControl: function() {
            var uploadControl = this.GetUploadControl();
            if(this.allowDropOnPreview) {
                uploadControl.DropZoneEnter.AddHandler(function() {
                    if(!this.GetEnabled())
                        return;
                    var buttonPanel = this.GetButtonPanel();
                    if(buttonPanel)
                        ASPx.SetElementDisplay(buttonPanel, false);
                    ASPx.SetElementDisplay(this.GetImageContainer(), false);
                    ASPx.SetElementDisplay(this.GetEmptyValuePlaceholder(), false);
                    ASPx.SetElementDisplay(this.GetDropZone(), true);
                }.aspxBind(this));
                uploadControl.DropZoneLeave.AddHandler(function() {
                    if(!this.GetEnabled())
                        return;
                    var buttonPanel = this.GetButtonPanel();
                    if(buttonPanel)
                        ASPx.SetElementDisplay(buttonPanel, true);
                    this.ShowValueDisplayElement();
                    ASPx.SetElementDisplay(this.GetDropZone(), false);
                }.aspxBind(this));
            }
            uploadControl.FileUploadComplete.AddHandler(function(s, e) {
                this.OnEndUpload(e.callbackData);
            }.aspxBind(this));
            uploadControl.UploadingProgressChanged.AddHandler(function(s, e) {
                this.OnUploadProgressChanged(e);
            }.aspxBind(this));
            uploadControl.FilesUploadStart.AddHandler(function() {
                this.OnStartUpload();
            }.aspxBind(this));
        },
        AdjustControl: function() {
            ASPxClientEdit.prototype.AdjustControl.call(this);
            if(this.enableEditing) 
                this.ShowValueDisplayElement();
        },
        OnClick: function(htmlEvent) {
            this.RaiseClick(this.GetMainElement(), htmlEvent);
        },
        RaiseClick: function(htmlElement, htmlEvent){
            if(!this.Click.IsEmpty()){
                var args = new ASPxClientEditClickEventArgs(htmlElement, htmlEvent);
                this.Click.FireEvent(this, args);
            }
        },    
        ChangeEnabledAttributes: function(enabled) {
            if(!this.enableEditing)
                return;
            this.ChangeElementsEnabledAttributes(ASPx.Attr.ChangeAttributesMethod(enabled));            
            this.ChangeEnabledEventsAttributes(ASPx.Attr.ChangeEventsMethod(enabled));
            this.ChangeDialogTriggerID(enabled);
            this.ChangeDropZoneID(enabled);
            ASPx.SetElementDisplay(this.GetDisabledCover(), !enabled);
        },
        ChangeElementsEnabledAttributes: function(method){
            method(this.GetImageElement(), "onclick");
            method(this.GetEmptyValuePlaceholder(), "onclick");
        },
        ChangeEnabledEventsAttributes: function(method){
            var deleteButton = this.GetDeleteButton();
            if (deleteButton)
                method(deleteButton, "click", this.onDeleteButtonClick);
        },
        ChangeDialogTriggerID: function(enabled){
            var openButton = this.GetOpenDialogButton();
            if (openButton) {
                var id = enabled ? openButton.id : "";
                this.GetUploadControl().SetDialogTriggerID(id);
            }
        },
        ChangeDropZoneID: function(enabled){
            var id = enabled ? this.GetMainElement().id : "";
            this.GetUploadControl().SetExternalDropZoneID(id);
        },
        GetWidth: function(){
            return this.GetSize(true);
        },
        GetHeight: function(){
            return this.GetSize(false);
        },
        SetWidth: function(width) {
            this.SetSize(width, this.GetHeight());
        },
        SetHeight: function(height) {
            this.SetSize(this.GetWidth(), height);
        },
        SetSize: function(width, height){
            this.imageWidth = width + "px";
            this.imageHeight = height + "px";
            var mainElement = this.GetMainElement();
            if(ASPx.IsExistsElement(mainElement))
                ASPx.ImageUtils.SetSize(mainElement, width, height);
            if(this.enableEditing) {
                this.imageContainerSizeIsValid = false;
                this.emptyImageContainerSizeIsValid = false;
                this.ShowValueDisplayElement();
            }
        },
        GetSize: function(isWidth){
            var image = this.GetMainElement();
            if(ASPx.IsExistsElement(image))
                return ASPx.ImageUtils.GetSize(image, isWidth);
            return -1;
        },
        FadeButtonPanelOpacity: function(show) {
            if(!this.IsAnimationEnabled() || !this.GetButtonPanel())
                return;

            var element = this.GetButtonPanelAnimatedElement();
            ASPx.AnimationHelper.fadeTo(element, {
                from: show ? this.GetButtonPanelMinOpacity() : 1, 
                to: show ? 1 : this.GetButtonPanelMinOpacity(),
                duration: 100
            });

            this.FadeButtonsOpacity(show);
        },
        FadeButtonsOpacity: function(show) {
            var maxButtonOpacity = 0.75;
            var options = {
                animationEngine: "js",                 
                property: "opacity",
                duration: 100,
                from: show ? this.GetButtonPanelMinOpacity() : maxButtonOpacity,
                to: show ? maxButtonOpacity : this.GetButtonPanelMinOpacity()
            };

            this.FadeButtonOpacity(this.GetDeleteButton(), options);
            this.FadeButtonOpacity(this.GetOpenDialogButton(), options);
        },
        FadeButtonOpacity: function (element, options) {
            if(!ASPx.IsExistsElement(element))
                return;
            var transition = ASPx.AnimationHelper.createAnimationTransition(element, options);
            transition.Start(options.from, options.to);
        },
        IsAnimationEnabled: function() {
            return this.GetEnabled();
        },
        OnCancelButtonClick: function() {
            var uploadControl = this.GetUploadControl();
            if(uploadControl && uploadControl.isInCallback)
                uploadControl.Cancel();
            this.UpdateOnUploading();
        },
        OnDeleteButtonClick: function() {
            this.Clear();
        },
        OnPreviewContainerMouseOver: function() {
            this.FadeButtonPanelOpacity(true);
        },
        OnPreviewContainerMouseOut: function() {
            this.FadeButtonPanelOpacity();
        },
        OnPreviewContainerTouchUp: function() {
            var show = ASPx.GetElementOpacity(this.GetButtonPanelAnimatedElement()) == this.GetButtonPanelMinOpacity();
            this.FadeButtonPanelOpacity(show);
        },
        GetButtonPanel: function() {
            return getChildElement(this, CONST.POSTFIX.BUTTON_PANEL);
        },
        GetButtonPanelAnimatedElement: function() {
            return this.buttonPanelVisibility == ButtonPanelVisibilityEnum.Faded ? this.GetButtonsShader(): this.GetButtonPanel();
        },
        GetButtonsContainer: function() {
            return getChildElement(this.GetButtonPanel(), CONST.POSTFIX.BUTTONS_CONTAINER);
        },
        GetButtonsShader: function() {
            return getChildElement(this.GetButtonPanel(), CONST.POSTFIX.BUTTONS_SHADER);
        },
        GetButtonPanelMinOpacity: function() {
            return this.buttonPanelVisibility == ButtonPanelVisibilityEnum.Faded ? 0.6 : 0;
        },
        GetCancelButton: function() {
            return getChildElement(this, CONST.POSTFIX.CANCEL_BUTTON);
        },
        GetDeleteButton: function() {
            return getChildElement(this.GetButtonPanel(), CONST.POSTFIX.DELETE_BUTTON);
        },
        GetDisabledCover: function() {
            return getChildElement(this, CONST.POSTFIX.DISABLED_COVER);
        },        
        GetDropZone: function() {
            return getChildElement(this, CONST.POSTFIX.DROP_ZONE);
        },
        GetImageContainer: function() {
            return getChildElement(this, CONST.POSTFIX.IMAGE_CONTAINER);
        },
        GetImageElement: function() {
            return getChildElement(this, CONST.POSTFIX.PREVIEW_CONTROL);
        },
        GetEmptyValuePlaceholder: function() {
            return getChildElement(this, CONST.POSTFIX.EMPTY_VALUE_PLACEHOLDER);
        },
        GetEmptyValueImage: function() {
            return getChildElement(this, CONST.POSTFIX.EMPTY_VALUE_IMAGE);
        },
        GetOpenDialogButton: function() {
            return getChildElement(this.GetButtonPanel(), CONST.POSTFIX.OPEN_DIALOG_BUTTON);            
        },
        GetProgressBar: function() { 
            return getChildControl(this, CONST.POSTFIX.PROGRESS_BAR); 
        },
        GetProgressPanel: function() { 
            return getChildElement(this, CONST.POSTFIX.PROGRESS_PANEL);            
        },  
        GetValueHiddenField: function() {
            return getChildElement(this, CONST.POSTFIX.VALUE_KEY_INPUT);
        },
        GetStateInput: function() {
            return this.GetValueHiddenField();
        },
        GetUploadControl: function() {
            return getChildControl(this, CONST.POSTFIX.UPLOAD_CONTROL);
        },
        GetValue: function() {
            return this.GetValueKey();
        },
        GetValueKey: function() {
            var dataKeyElem = this.GetValueHiddenField();
            if(ASPx.IsExistsElement(dataKeyElem))
                return dataKeyElem.value;
            return "";
        },
        GetText: function() {
            return this.GetValue();
        },
        SetProgressPanelVisibility: function(show) {
            var progressPanel = this.GetProgressPanel();
            var progressBar = this.GetProgressBar();
            if(progressPanel){
                ASPx.SetElementDisplay(progressPanel, show);
            }
            if(progressBar && show){
                progressBar.AdjustControlCore();
            }
        },
        SetDataKeyValue: function(value, forceValueChanged) {            
            var dataKeyElem = this.GetValueHiddenField();
            dataKeyElem.value = value;
            if(forceValueChanged)
                this.OnValueChanged();
        },
        SetProgressBarPosition: function(pos) {
            var progressBar = this.GetProgressBar();
            if(progressBar)
                progressBar.SetPosition(pos);
        },
        SetValue: function(value) {
            this.SetValueKey(value);
        },
        SetValueKey: function(key) {
            if(this.enableEditing)
                this.ApplyDataKey(key);
        },
        ApplyDataKey: function(key) {
            this.SetDataKeyValue(key);
            this.UpdatePreview();
        },
        UpdateButtonPanelVisibility: function() {
            ASPx.SetElementDisplay(this.GetDeleteButton(), this.GetValue());
        },
        UpdateImage: function(info) {
            var key = info ? info.Key : "";
            var imageUrl = info ? info.ImageUrl : null;

            this.SetDataKeyValue(key, true);
            var image = this.GetImageElement();
            image.onload = function() {
                this.UpdateOnUploading();
                this.ShowValueDisplayElement();
                this.UpdateButtonPanelVisibility();
            }.aspxBind(this);
            ASPx.ImageUtils.SetImageSrc(image, imageUrl || ASPx.EmptyImageUrl);
        },
        UpdatePreview: function() {
            this.PerformCallbackInternal();
        },
        UpdateOnUploading: function(start){
            ASPx.SetElementDisplay(this.GetButtonPanel(), !start);
            this.SetProgressPanelVisibility(start);
            ASPx.SetElementDisplay(this.GetEmptyValuePlaceholder(), !start && !this.GetValue());
        },
        Clear: function() {
            this.UpdateImage(null);
        },
        PerformCallback: function(parameter) {
            if (!ASPx.IsExists(parameter)) parameter = "";
            parameter = formatCallbackArg(CONST.CALLBACK_PREFIX.CUSTOM_CALLBACK, parameter);
            this.PerformCallbackInternal(parameter);
        },
        PerformCallbackInternal: function(parameter) {
            this.CreateCallback(parameter);
        },
        OnCallback: function(result) {
            this.UpdateImage(result);
        },
        OnStartUpload: function() {
            this.SetProgressBarPosition(0);
            this.UpdateOnUploading(true);
        },
        OnEndUpload: function(data) {
            if(this.showLoadingImage) {
                this.RemoveImageElement();
                this.CreateImageElement();
            }
            var obj = eval(data);
            this.UpdateImage(obj.info);
        },
        RemoveImageElement: function() {
            if(this.enableEditing) {
                ASPx.RemoveElement(this.GetImageElement());
            }
        },
        CreateImageElement: function() {
            if(!this.enableEditing)
                return;
            var container = this.GetImageContainer();
            var img = document.createElement('IMG');
            img.id = this.name + '_' + CONST.POSTFIX.PREVIEW_CONTROL;
            img.style.height = 'auto';
            img.style.width = 'auto';
            if(this.showLoadingImage) {
                for(var i = 0; i < this.loadingImageData.cssClasses.length; i++) {
                    var cssClass = this.loadingImageData.cssClasses[i];
                    img.className += ' ' + cssClass;
                }
                if(this.loadingImageData.loadingImage) {
                    img.style.backgroundImage =  this.loadingImageData.loadingImage.replace(/"/g, "'");
                }
                var isOldBrowser = ASPx.Browser.IE && ASPx.Browser.Version < 9;
                var loadingImage = this.loadingImageData.loadingImage;
                var backgroundImageUrl = this.loadingImageData.backgroundImageUrl;
                var onLoad = function() {
                    ASPx.ASPxImageLoad.OnLoad(img, loadingImage, isOldBrowser, backgroundImageUrl);
                }
                ASPx.Evt.AttachEventToElement(img, "load", onLoad);
                ASPx.Evt.AttachEventToElement(img, "onabort", onLoad);
                ASPx.Evt.AttachEventToElement(img, "onerror", onLoad);
            }
            container.appendChild(img);
            this.CalculateImageElementSizes();
        },
        OnUploadProgressChanged: function(args) {
            this.SetProgressBarPosition(args.progress);
        },
        ShowValueDisplayElement: function() {
            if(!ASPx.IsElementVisible(this.GetMainElement()))
                return;
            if(this.GetValue()) {
                this.ShowPreview();
            } else {
                this.ShowPlaceholder();
            }
        },
        ShowPreview: function() {
            ASPx.SetElementDisplay(this.GetEmptyValuePlaceholder(), false);
            var imageContainer = this.GetImageContainer();
            if(!this.imageContainerSizeIsValid) 
                this.CalculateImageElementSizes();
            ASPx.SetElementDisplay(imageContainer, true);
        },
        CalculateImageElementSizes: function() {
            var imageContainer = this.GetImageContainer();
            var imageElement = this.GetImageElement();
            ASPx.SetElementDisplay(imageElement, false);
            ASPx.SetElementDisplay(imageContainer, false);
            imageContainer.style.height = this.GetMainElementHeightForInnerContainer();
            imageContainer.style.width = this.GetMainElementWidthForInnerContainer();
            ASPx.SetElementDisplay(imageContainer, true);
            imageContainer.style['line-height'] = this.GetMainElementHeightForInnerContainer();
            ASPx.SetElementDisplay(imageElement, true);
            this.imageContainerSizeIsValid = true;
        },
        ShowPlaceholder: function() {
            var emptyImage = this.GetEmptyValueImage();
            if(emptyImage && ASPx.GetElementDisplay(emptyImage) && !this.emptyImageContainerSizeIsValid){
                var emptyImageContainer = emptyImage.parentElement;
                emptyImageContainer.style.height = this.GetMainElementHeightForInnerContainer();
                emptyImageContainer.style.width = this.GetMainElementWidthForInnerContainer();            
                this.emptyImageContainerSizeIsValid = true;
            }
            ASPx.SetElementDisplay(this.GetImageContainer(), false);
            ASPx.SetElementDisplay(this.GetEmptyValuePlaceholder(), true);
        },
        GetMainElementHeightForInnerContainer: function() {
            var mainElement = this.GetMainElement();
            if(window.getComputedStyle)
                return window.getComputedStyle(mainElement).height;
            return mainElement.clientHeight;
        },
        GetMainElementWidthForInnerContainer: function() {
            var mainElement = this.GetMainElement();
            if(window.getComputedStyle)
                return window.getComputedStyle(mainElement).width;
            return mainElement.clientWidth;
        }
    });
    window.ASPxClientBinaryImage = ASPxClientBinaryImage;
    ASPxClientBinaryImage.Cast = ASPxClientControl.Cast;

    ASPx.Ident.IsStaticASPxClientBinaryImage = function(obj) {
        return !!obj.isASPxClientBinaryImage && !obj.enableEditing;
    };
})();