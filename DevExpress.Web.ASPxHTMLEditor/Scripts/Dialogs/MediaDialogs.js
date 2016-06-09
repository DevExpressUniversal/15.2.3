(function() {
    var InsertDialogBase = ASPx.HtmlEditorClasses.Dialogs.InsertDialogBase;
    var mergeFieldMaps = ASPx.HtmlEditorClasses.Utils.mergeFieldMaps;
    var executeIfExists = ASPx.HtmlEditorClasses.Utils.executeIfExists;

    var saveCallbackPrefixes = {
        image: {
            callback: "ImageToServer",
            uploaded: "ISU",
            error: "ISE",
            width: "TNIW",
            height: "TNIH",
            thumbnailName: "TNIF"
        },
        flash: {
            callback: "FSC",
            uploaded: "FSU",
            error: "FSE"
        },
        audio: {
            callback: "ASC",
            uploaded: "ASU",
            error: "ASE"
        },
        video: {
            callback: "VSC",
            uploaded: "VSU",
            error: "VSE"
        }
    };

    var InsertMediaDialogBase = ASPx.CreateClass(InsertDialogBase, {
        constructor: function(cmdName) {
            this.constructor.prototype.constructor.call(this, cmdName);
            this.elementId = null;
            this.htmlBeforeChange = null;
        },
        getResourceUrlPropertyName: function() {
            return "src";    
        },
        SaveEditorsState: function() {
            executeIfExists(this.getControl("MoreOptions"), function(element) {
                this.saveValue("moreOptionsCheckbox", element.GetChecked());
            }.aspxBind(this));
        },
        RestoreEditorsState: function() {
            executeIfExists(this.getControl("MoreOptions"), function(element) {
                element.SetChecked(this.loadValue("moreOptionsCheckbox"));
                this.onMoreOptionsCheckedChanged(element);
            }.aspxBind(this));
        },
        getSelectContentControl: function() {
            return this.getControl("SourceSelectControl");    
        },
        getCallbackPrefix: function() {
            return this.getCallbackPrefixes().callback;    
        },
        getCallbackUploadedPrefix: function() {
            return this.getCallbackPrefixes().uploaded;    
        },
        getCallbackErrorPrefix: function() {
            return this.getCallbackPrefixes().error;    
        },
        getSelectedElement: function() {
            var selectedElement = this.selectionInfo.selectedElement;
            if(selectedElement.tagName == "IMG" && this.hasCssClassMarker(selectedElement))
                return selectedElement;
            return null;
        },
        hasCssClassMarker: function(el) {
            var markerArray = this.getMediaObjectCssClassMarker();
            if(markerArray.length == 0 )
                return true;
            return el && ASPx.Data.ArrayIndexOf(markerArray, el.className, function(marker, className) { return className.indexOf(marker) > -1; }) > -1;     
        },
        getMediaObjectCssClassMarker: function() {
            return [];
        },
        getDialogPropertiesMap: function() {
            return mergeFieldMaps(this.getDialogStylePropertiesMap(), {
                "position": "Position",
                "className": "CssClassName"
            });    
        },
        getDialogStylePropertiesMap: function() {
            return mergeFieldMaps(this.getPreviewDependentStylePropertiesMap(), {
                "marginTop": "TopMargin",
                "marginBottom": "BottomMargin",
                "marginLeft": "LeftMargin",
                "marginRight": "RightMargin"
            });       
        },
        getPreviewDependentStylePropertiesMap: function() {
            return {
                "width": "Width",
                "height": "Height",
                "borderWidth": "BorderWidth",
                "borderStyle": "BorderStyle",
                "borderColor": "BorderColor"
            };    
        },
        updatePreview: function() {
            var styleSettings = {};
            this.iterateDialogEditors(styleSettings, function(editor, settings, settingName) {
                settings[settingName] = (editor.GetChecked || editor.GetValue).call(editor);        
            }, this.getPreviewDependentStylePropertiesMap());
            executeIfExists(this.getSelectContentControl(), function (mediaFileSelector) {
                mediaFileSelector.UpdatePreviewStyles(styleSettings);
            }.aspxBind(this));
        },
        getObjectSettings: function(settings) {
            settings = InsertDialogBase.prototype.getObjectSettings.call(this, settings);
            settings.id = this.elementId;
            settings.htmlBeforeChange = this.htmlBeforeChange;
            settings.src = ASPx.HtmlEditorClasses.encodeURIPath(this.getSelectContentControl().GetURL());
            return settings;
        },
        extractObjectSettings: function(element, settings) {
            settings = settings || {};
            if(element) {
                this.elementId = element.id;
                this.htmlBeforeChange = element.outerHTML;

                settings.className = ASPx.Attr.GetAttribute(element, "class");
                settings.src = ASPx.Attr.GetAttribute(element, "src");
                settings.position = ASPx.HtmlEditorClasses.Utils.getElementPosition(this.getSelectedElement());
                settings.width = ASPx.Attr.GetAttribute(element, "width");
                settings.height = ASPx.Attr.GetAttribute(element, "height");
                
                var styleAttributes = this.getDialogStylePropertiesMap();
                for(var p in styleAttributes) {
                    var value = element.style[p];
                    if(ASPx.IsExists(value) && value != "")
                        settings[p] = value;
                }            
            }
            return settings;
        },
        releaseResources: function() {
            this.getSelectContentControl().ReleaseResources();
        },
        isDialogValid: function(skipInnerValidation) {
            if(skipInnerValidation)
                return !!this.getSelectContentControl().GetURL();
            return this.getSelectContentControl().IsValid() && ASPxClientEdit.ValidateGroup("mediaDialogValidationGroup", false);    
        },
        onCancelButtonClick: function() {
            this.getSelectContentControl().TryCancelUpload();        
        },
        InitializeDialogFields: function(settings) {
            InsertDialogBase.prototype.InitializeDialogFields.call(this, settings);
            if(settings) {
                this.getSelectContentControl().SetURL(settings.src, true);
                this.RaiseValueChanged();
                this.updatePreview();
            }
        },
        preparePreviewInfo: function (s, e) {
            e.url = ASPx.HtmlEditorClasses.encodeURIPath(e.url);
        },
        attachEvents: function () {
            InsertDialogBase.prototype.attachEvents.call(this);
            var selectContentControl = this.getSelectContentControl();
            selectContentControl.RequestInsert.AddHandler(function(s) {
                ASPx.DialogComplete(1, this.getObjectSettings());        
            }.aspxBind(this));
            selectContentControl.CurrentURLChanged.AddHandler(function(s, e) {
                this.RaiseValueChanged();    
            }.aspxBind(this));
            var fileManager = selectContentControl.getFileManager();
            if(fileManager)
                fileManager.setOwner(this.htmlEditor);
            executeIfExists(this.getControl("BorderStyle"), function (element) {
                element.ValueChanged.AddHandler(this.updatePreview.aspxBind(this));
            }.aspxBind(this));
            executeIfExists(this.getControl("BorderColor"), function(element) {
                element.ValueChanged.AddHandler(this.updatePreview.aspxBind(this));
            }.aspxBind(this));
            executeIfExists(this.getControl("BorderWidth"), function(element) {
                element.ValueChanged.AddHandler(this.updatePreview.aspxBind(this));
            }.aspxBind(this));
            executeIfExists(this.getControl("MoreOptions"), function(element) {
                element.CheckedChanged.AddHandler(this.onMoreOptionsCheckedChanged.aspxBind(this));
            }.aspxBind(this));
            selectContentControl.BeforePreviewUpdate.AddHandler(this.preparePreviewInfo.aspxBind(this));
        },
        onMoreOptionsCheckedChanged: function(s, e) {
            this.getControl("MainFormLayout").GetItemByName("settingsGroup").SetVisible(s.GetChecked());  
            this.getControl("SourceSelectControl").AdjustControl();    
        },
        GetInitInfoObject: function() {
            return this.isChangeDialog() ? this.extractObjectSettings(this.getSelectedElement()) : null;
        },
        getCallbackPrefixes: function() {      
        },
        OnCallback: function(result) {
            if(!!this.getCallbackPrefixes() && result.indexOf(this.getCallbackPrefix()) == 0) {
                var callbackData = result.substring(this.getCallbackPrefix().length + 1, result.length);
                if(callbackData.indexOf(this.getCallbackErrorPrefix()) > -1) {
                    var message = callbackData.substring(this.getCallbackErrorPrefix().length + 1, callbackData.length);
                    var textBox = this.getSelectContentControl().GetURLTextBox();
                    textBox.SetIsValid(false);
                    textBox.SetErrorText(message);
                    textBox.UpdateErrorFrameAndFocus(false, true);
                } else {
                    this.insertParams.src = callbackData.substr(this.getCallbackUploadedPrefix().length + 1, callbackData.length);
                    InsertDialogBase.prototype.OnComplete.call(this, 1, this.insertParams);
                }
            }
            else
                ASPx.Dialog.prototype.OnCallback.call(this, result);
        },
        OnComplete: function(result, params) {
            this.insertParams = params;
            if(result) {
                if(this.getSelectContentControl().NeedSaveToServer())
                    return this.SendCallback(ASPx.FormatCallbackArg(this.getCallbackPrefix(), params.src));
            }
            InsertDialogBase.prototype.OnComplete.call(this, result, params);
        },
        OnClosing: function (args) {
            executeIfExists(this.getSelectContentControl(), function (sourceSelectControl) {
                if (args.closeReason == ASPxClientPopupControlCloseReason.Escape && sourceSelectControl.IsInUpload()) {
                    args.cancel = true; 
                    sourceSelectControl.TryCancelUpload();
                }
            }.aspxBind(this));
        }
    });

    var InsertMediaDialog = ASPx.CreateClass(InsertMediaDialogBase, {
        GetInitInfoObject: function() {
            if(this.isChangeDialog())
                return this.extractObjectSettings(ASPx.HtmlEditorClasses.HtmlProcessor.restoreDomElementBySpecialImageElement(document, this.getSelectedElement()));
            return null;
        }    
    });

    var InsertImageDialog = ASPx.CreateClass(InsertMediaDialogBase, {
        constructor: function(cmdName) {
            this.constructor.prototype.constructor.call(this, cmdName);
            this.constrainProportions = true;
            this.constrainProportionsCoef_WH = 1;
            this.isSizeDirty = false;
        },
        createCommandArgument: function(params) {
            var args = this.isChangeDialog() ? 
                new ASPxClientHtmlEditorChangeImageCommandArguments(this.htmlEditor, this.getSelectedElement()) : 
                new ASPxClientHtmlEditorInsertImageCommandArguments(this.htmlEditor);

            args.src = params.src;
            args.alt = params.alt;
            args.align = params.align;
            args.useFloat = params.useFloat;

            args.styleSettings.className = params.cssClass;

            args.styleSettings.borderWidth = params.borderWidth;
            args.styleSettings.borderColor = params.borderColor;
            args.styleSettings.borderStyle = params.borderStyle;
            
            args.styleSettings.marginTop = params.marginTop;
            args.styleSettings.marginRight = params.marginRight;
            args.styleSettings.marginBottom = params.marginBottom;
            args.styleSettings.marginLeft = params.marginLeft;
            
            args.styleSettings.width = params.width;
            args.styleSettings.height = params.height;

            return args;
        },
        OnShown: function (args) {
            InsertMediaDialogBase.prototype.OnShown.call(this, args);
            this.updatePreview();
        },
        InitializeDialogFields: function (settings) {
            if(settings && settings.isCustomSize) {
                this.isSizeDirty = true;
                executeIfExists(this.getControl("MainSettings"), function(formLayout) {
                    this.getControl("Sizes").SetValue("custom");
                }.aspxBind(this));
                this.changeSizeSettingsVisibility(true);
            }
            InsertMediaDialogBase.prototype.InitializeDialogFields.call(this, settings);
        },
        changeSizeSettingsVisibility: function(visibility) {
            executeIfExists(this.getControl("MainSettings"), function(formLayout) {
                formLayout.GetItemByName("EmptyItem").SetVisible(!visibility);
                formLayout.GetItemByName("SizeGroup").SetVisible(visibility);
                var thumbnail = formLayout.GetItemByName("ThumbnailName");
                if(thumbnail)
                    thumbnail.SetVisible(this.isCreateThumbnail());
            }.aspxBind(this));
        },
        getCacheKey: function() {
            return "image";    
        },
        isChangeDialog: function() {
            return !!this.getSelectedElement();    
        },
        getCommandName: function() {
            return this.isChangeDialog() ? ASPxClientCommandConsts.CHANGEIMAGE_COMMAND : ASPxClientCommandConsts.INSERTIMAGE_COMMAND;    
        },
        extractObjectSettings: function(element, settings) {
            settings = ASPx.HtmlEditorClasses.Commands.ChangeImage.GetImageProperties(element);
            settings = InsertMediaDialogBase.prototype.extractObjectSettings.call(this, element, settings);
            settings.width = settings.width && (ASPx.IsNumber(settings.width) ? settings.width : ASPx.PxToInt(settings.width));
            settings.height = settings.height && (ASPx.IsNumber(settings.height) ? settings.height : ASPx.PxToInt(settings.height));
            return settings;
        },
        getDialogPropertiesMap: function() {
            return mergeFieldMaps(InsertMediaDialogBase.prototype.getDialogStylePropertiesMap.call(this), {
                "cssClass": "CssClassName",
                "useFloat": "Wrap",
                "alt": "Description",
                "align": "Position"
            });    
        },
        GetDialogCaptionText: function() {
            return this.isChangeDialog() ? ASPx.HtmlEditorDialogSR.ChangeImage : ASPx.HtmlEditorDialogSR.InsertImage;    
        },
        getCallbackPrefixes: function() {
            return saveCallbackPrefixes.image;    
        },
        isCreateThumbnail: function() {
            return executeIfExists(this.getControl("CreateThumbnail"), function(el) { 
                return el.IsVisible() && el.GetChecked();
            }, false);    
        },
        saveThumbnailImageToServerViaCallback: function(thumbnailWidth, thumbnailHeight) {
            var src = this.getSelectContentControl().GetURL();
            var newImageFileName = this.getControl("ThumbnailName").GetText();
            var callbackPrefixes = this.getCallbackPrefixes();
            this.SendCallback(ASPx.FormatCallbackArgs([
                [ callbackPrefixes.callback, src ],
                [ callbackPrefixes.width, thumbnailWidth ],
                [ callbackPrefixes.height, thumbnailHeight ],
                [ callbackPrefixes.thumbnailName, newImageFileName ]
            ]));
        },
        resetSize: function() { 
            if(this.isSizeDirty) {
                this.isSizeDirty = false;
                this.updateSizeFields(this.initialWidth, this.initialHeight);
            }
        },
        attachEvents: function() {
            InsertMediaDialogBase.prototype.attachEvents.call(this);
            executeIfExists(this.getControl("MainSettings"), function(formLayout) {
                ASPx.Evt.AttachEventToElement(document.getElementById("dxheMediaDialogResetImage"), "click", this.resetSize.aspxBind(this));
                this.getControl("Sizes").SelectedIndexChanged.AddHandler(function(s, e) {
                    this.changeSizeSettingsVisibility(s.GetValue() == "custom");
                }.aspxBind(this));
                var isCreateThumbnailExists = executeIfExists(this.getControl("CreateThumbnail"), function(cb) {
                    cb.ValueChanged.AddHandler(function(s, e) {
                        formLayout.GetItemByName("ThumbnailName").SetVisible(s.GetChecked());    
                    });
                    return true;
                }.aspxBind(this), false);
                this.getSelectContentControl().BeforePreviewUpdate.AddHandler(function(s, e) {
                    e.createOptions.onReadyCallback = function(evt) {
                        var image = ASPx.Evt.GetEventSource(evt);
                        this.updateSizeFields(image.naturalWidth || image.width, image.naturalHeight || image.height);
                    }.aspxBind(this);
                    if (isCreateThumbnailExists) {
                        var fileName = ASPx.HtmlEditorDialog.ClientPath.GetFileNameWithoutExtension(e.url);
                        if(fileName)
                            fileName += "Thumbnail";
                        this.getControl("ThumbnailName").SetText(fileName);
                    }
                }.aspxBind(this));
                this.attachSizeSpinEditsHandlers(this.getControl("Width"), "width");
                this.attachSizeSpinEditsHandlers(this.getControl("Height"), "height");

            }.aspxBind(this));
        },
        attachSizeSpinEditsHandlers: function(spinEdit, sizeType) {
            spinEdit.NumberChanged.AddHandler(function(s, e) {
                this.isSizeDirty = true;
                if(this.constrainProportions)
                    this.updateSizeSpinEditsWithConstrainProportions(sizeType);    
            }.aspxBind(this));
            spinEdit.KeyUp.AddHandler(function(s, e) {
                this.isSizeDirty = true;
                if(this.constrainProportions) {
                    var keyCode = ASPx.Evt.GetKeyCode(e.htmlEvent);        
                    if(keyCode != ASPx.Key.Tab && keyCode != ASPx.Key.Shift)
                        this.updateSizeSpinEditsWithConstrainProportions(sizeType);
                }    
            }.aspxBind(this));
        },
        onConstrainSizeClick: function(event) {
            var imageToDisappear = ASPx.Evt.GetEventSource(event);
            var imageToAppear = imageToDisappear.previousSibling || imageToDisappear.nextSibling;
            ASPx.SetElementDisplay(imageToDisappear, false);
            ASPx.SetElementDisplay(imageToAppear, true);
            
            this.constrainProportions = !this.constrainProportions;
            if(this.constrainProportions)
                this.updateSizeFields(this.initialWidth, this.initialHeight);
        },
        updateSizeSpinEditsWithConstrainProportions: function(sizeType) {
            var widthVal = this.getControl("Width").GetText();
            var heightVal = this.getControl("Height").GetText();

            if(widthVal === "" || heightVal === "") {
                if(widthVal == heightVal)
                    this.updateSizeFields(this.initialWidth, this.initialHeight);
                return;
            }
            if(sizeType == "width")
                this.updateSizeEditorValues(this.getControl("Width"), this.getControl("Height"), (1/this.constrainProportionsCoef_WH));
            else 
                this.updateSizeEditorValues(this.getControl("Height"), this.getControl("Width"), this.constrainProportionsCoef_WH);
        },
        updateSizeEditorValues: function(editor1, edtitor2, coef) {
            editor1.SaveSelectionStartAndEndPosition();
            edtitor2.SetValue(Math.floor(editor1.GetParsedNumber()*coef));
            editor1.RestoreSelectionStartAndEndPosition();    
        },
        updateSizeFields: function(imageWidth, imageHeight, imageStyleWidth, imageStyleHeight) {
            var imageWidth = imageWidth || 0;
            var imageHeight = imageHeight || 0;
            if(!this.isSizeDirty) {
                this.getControl("Width").SetValue(imageStyleWidth ? ASPx.PxToInt(imageStyleWidth) : imageWidth);
                this.getControl("Height").SetValue(imageStyleHeight ? ASPx.PxToInt(imageStyleHeight) : imageHeight);
            }
            this.updateConstrainProportionsCoef(imageWidth, imageHeight);
        },
        OnComplete: function (result, params) {
            this.insertParams = params;
            if (result) {
                this.prepareParams(params);
                if (this.isCreateThumbnail())
                    return this.saveThumbnailImageToServerViaCallback(params.width, params.height);
                else
                    if (!this.htmlEditor.allowInsertDirectImageUrls || this.getSelectContentControl().NeedSaveToServer())
                        return this.SendCallback(ASPx.FormatCallbackArg(this.getCallbackPrefix(), params.src));
            }
            InsertDialogBase.prototype.OnComplete.call(this, result, params);
        },
        prepareParams: function (params) {
            params.imageElement = this.getSelectedElement();
            executeIfExists(this.getControl("MainSettings"), function() {
                if(this.getControl("Sizes").GetValue() != "custom") {
                    delete params.width;
                    delete params.height;
                }
            }.aspxBind(this));
        },
        updateConstrainProportionsCoef: function(width, height) {
            if((width == height) || (height == 0))
                this.constrainProportionsCoef_WH = 1;
            else
                this.constrainProportionsCoef_WH = width/height;  
            this.initialWidth = width;
            this.initialHeight = height;
        }
    });
    var InsertFlashDialog = ASPx.CreateClass(InsertMediaDialog, {
        getCacheKey: function() {
            return "flash";    
        },
        getCallbackPrefixes: function() {
            return saveCallbackPrefixes.flash;        
        },
        getMediaObjectCssClassMarker: function() {
            return  [ASPx.HtmlEditorClasses.MediaCssClasses.Flash, ASPx.HtmlEditorClasses.MediaCssClasses.NotSupported];
        },
        GetDialogCaptionText: function() {
            return this.isChangeDialog() ? ASPx.HtmlEditorDialogSR.ChangeFlash : ASPx.HtmlEditorDialogSR.InsertFlash;    
        },
        getDialogPropertiesMap: function() {
            var standartMap = InsertMediaDialog.prototype.getDialogPropertiesMap.call(this);
            return mergeFieldMaps(standartMap, this.getParamProperties());
        },
        getParamProperties: function() {
            return {
                "loop": "Loop",
                "allowFullScreen": "AllowFullscreen",
                "quality": "Quality",
                "play": "AutoPlay",
                "menu": "EnableFlashMenu",
                "movie": "",
                "sound": "",
                "image": "",
                "url": "",
                "src": ""
            };
        },
        extractObjectSettings: function(element) {
            var settings = InsertMediaDialog.prototype.extractObjectSettings.call(this, element);
            this.tryToExtractParamProperties(element.childNodes, settings);
            settings.src = ASPx.Attr.GetAttribute(element, "data") || settings.src;
            return settings;
        },
        tryToExtractParamProperties: function(childNodes, resultContainer) {
            var paramProps = this.getParamProperties();
            for(var i = 0; i < childNodes.length; i++) {
                var child = childNodes[i];
                var name = ASPx.Attr.GetAttribute(child, "name");
                for(var p in paramProps) {
                    if(p == name) {
                        resultContainer[p] = this.fixParamValue(ASPx.Attr.GetAttribute(child, "value"));
                        delete paramProps[p];
                        break;
                    }
                }
            }
            resultContainer.src = resultContainer.src || resultContainer.url || resultContainer.movie || resultContainer.sound || resultContainer.image;
        },
        fixParamValue: function(value) {
            switch(value.toLowerCase()) {
                case "true":
                    return true;
                case "false":
                    return false;
                default:
                    return value;
            }
            return value;
        }
    });

    var InsertYouTubeDialog = ASPx.CreateClass(InsertMediaDialog, {
        getResourceUrlPropertyName: function() {
            return null;    
        },
        getCacheKey: function() {
            return "youtube";    
        },
        GetDialogCaptionText: function() {
            return this.isChangeDialog() ? ASPx.HtmlEditorDialogSR.ChangeYouTubeVideo : ASPx.HtmlEditorDialogSR.InsertYouTubeVideo;    
        },
        getDialogPropertiesMap: function() {
            return mergeFieldMaps(InsertMediaDialog.prototype.getDialogPropertiesMap.call(this), {
                "showControls": "ShowControls",
                "highSecureMode": "ConfidentMode",
                "showSameVideos": "ShowSameVideos",
                "showVideoName": "ShowVideoName"
            });
        },
        getMediaObjectCssClassMarker: function() {
            return  [ASPx.HtmlEditorClasses.MediaCssClasses.YouTube];
        },
        extractObjectSettings: function(element) {
            var settings = ASPx.HtmlEditorClasses.YoutubeObject.createSettings(ASPx.Attr.GetAttribute(element, "src"));
            return InsertMediaDialog.prototype.extractObjectSettings.call(this, element, settings);
        },
        preparePreviewInfo: function (s, e) {
            if(e.url) {
                e.url = ASPx.HtmlEditorClasses.YoutubeObject.createURL({
                    youtubeVideoId: ASPx.HtmlEditorClasses.YoutubeObject.getYoutubeIdFromURL(e.url)
                });
            }
        }
    });
    var InsertHTML5MediaDialog = ASPx.CreateClass(InsertMediaDialog, {
        getDialogPropertiesMap: function() {
            return mergeFieldMaps(InsertMediaDialog.prototype.getDialogPropertiesMap.call(this), this.getObjectProperties());
        },
        getObjectProperties: function() {
            return { 
                "autoPlay": "AutoPlay",
                "showControls": "ShowControls",
                "repeat": "Loop",
                "preload": "Preload"
            };
        },
        extractObjectSettings: function(element) {
            var result = InsertMediaDialog.prototype.extractObjectSettings.call(this, element);
            result.autoPlay = !!ASPx.Attr.GetAttribute(element, ASPx.HtmlEditorClasses.preservedAttributeNamePrefix + "autoplay");
            result.showControls = !!ASPx.Attr.GetAttribute(element, "controls");
            result.repeat = !!ASPx.Attr.GetAttribute(element, "loop");
            result.preload = ASPx.Attr.GetAttribute(element, "preload");
            return result;
        }
    });
    var InsertAudioDialog = ASPx.CreateClass(InsertHTML5MediaDialog, {
        getCacheKey: function() {
            return "audio";    
        },
        getCallbackPrefixes: function() {
            return saveCallbackPrefixes.audio;    
        },
        GetDialogCaptionText: function() {
            return this.isChangeDialog() ? ASPx.HtmlEditorDialogSR.ChangeAudio : ASPx.HtmlEditorDialogSR.InsertAudio;    
        },
        getMediaObjectCssClassMarker: function() {
            return  [ASPx.HtmlEditorClasses.MediaCssClasses.Audio];
        }
    });

    
    var InsertVideoDialog = ASPx.CreateClass(InsertHTML5MediaDialog, {
        getCacheKey: function() {
            return "video";    
        },
        getCallbackPrefixes: function() {
            return saveCallbackPrefixes.video;    
        },
        GetDialogCaptionText: function() {
            return this.isChangeDialog() ? ASPx.HtmlEditorDialogSR.ChangeVideo : ASPx.HtmlEditorDialogSR.InsertVideo;    
        },
        getDialogPropertiesMap: function() {
            return mergeFieldMaps(InsertHTML5MediaDialog.prototype.getDialogPropertiesMap.call(this), {
                "poster": "Poster"
            });
        },
        getMediaObjectCssClassMarker: function() {
            return  [ASPx.HtmlEditorClasses.MediaCssClasses.Video];
        },
        extractObjectSettings: function(element) {
            var result = InsertHTML5MediaDialog.prototype.extractObjectSettings.call(this, element);
            result.poster = ASPx.Attr.GetAttribute(element, "poster");
            return result;
        }
    });
    
    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.INSERTIMAGE_DIALOG_COMMAND] = new InsertImageDialog(ASPxClientCommandConsts.INSERTIMAGE_DIALOG_COMMAND);
    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.CHANGEIMAGE_DIALOG_COMMAND] = new InsertImageDialog(ASPxClientCommandConsts.INSERTIMAGE_DIALOG_COMMAND);
    
    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.INSERTFLASH_DIALOG_COMMAND] = new InsertFlashDialog(ASPxClientCommandConsts.INSERTFLASH_DIALOG_COMMAND);
    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.CHANGEFLASH_DIALOG_COMMAND] = new InsertFlashDialog(ASPxClientCommandConsts.CHANGEFLASH_DIALOG_COMMAND);
    
    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.INSERTVIDEO_DIALOG_COMMAND] = new InsertVideoDialog(ASPxClientCommandConsts.INSERTVIDEO_DIALOG_COMMAND);
    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.CHANGEVIDEO_DIALOG_COMMAND] = new InsertVideoDialog(ASPxClientCommandConsts.CHANGEVIDEO_DIALOG_COMMAND);
    
    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.INSERTAUDIO_DIALOG_COMMAND] = new InsertAudioDialog(ASPxClientCommandConsts.INSERTAUDIO_DIALOG_COMMAND);
    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.CHANGEAUDIO_DIALOG_COMMAND] = new InsertAudioDialog(ASPxClientCommandConsts.CHANGEAUDIO_DIALOG_COMMAND);
    
    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.INSERTYOUTUBEVIDEO_DIALOG_COMMAND] = new InsertYouTubeDialog(ASPxClientCommandConsts.INSERTYOUTUBEVIDEO_DIALOG_COMMAND);
    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.CHANGEYOUTUBEVIDEO_DIALOG_COMMAND] = new InsertYouTubeDialog(ASPxClientCommandConsts.CHANGEYOUTUBEVIDEO_DIALOG_COMMAND);
})();