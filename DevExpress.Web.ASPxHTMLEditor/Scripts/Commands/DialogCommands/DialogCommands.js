(function() {
    ASPx.HtmlEditorClasses.Commands.DialogCommand = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        Execute: function(cmdValue, wrapper) {
            var dialog = ASPx.HtmlEditorDialogList[this.getDialogCmdID(wrapper, cmdValue)];
            if (dialog != null)
                wrapper.executeDialog(dialog, cmdValue);
            else
                alert('Dialog is not found');
            return true;
        },
        CanChangeSelection: function() {
            return false;    
        },
        IsHtmlChangeable: function() {
            return false;
        },
        GetState: function(core, selection, selectedElements) {
            return false;
        },
        IsLocked: function(core) {
            return false;
        },
        getDialogCmdID: function(wrapper, cmdValue) {
            return this.commandID;
        }
    });

    var MediaDialogBase = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.DialogCommand, {
        getCssClassMarker: function() {
            return [""];
        }
    });
    var InsertMediaDialogBase = ASPx.CreateClass(MediaDialogBase, {
        canBeExecutedOnSelectedElement: function(selectedElement) {
            return !selectedElement || (selectedElement && ASPx.Data.ArrayIndexOf(this.getCssClassMarker(), selectedElement.className, function(classMarker, className) { return className.indexOf(classMarker) > -1; }) > -1);
        },
        getDialogCmdID: function(wrapper) {
            var selectedElement = wrapper.getSelection().GetSelectedElement();
            if(selectedElement && ASPx.Data.ArrayIndexOf(this.getCssClassMarker(), selectedElement.className, function(classMarker, className) { return className.indexOf(classMarker) > -1; }) > -1)
                return this.getChangeCommandID();
            else
                return this.commandID;
        },
        getChangeCommandID: function() {
            return "";    
        }
    });
    var ChangeMediaDialogBase = ASPx.CreateClass(MediaDialogBase, {
        canBeExecutedOnSelectedElement: function(selectedElement) {
            return selectedElement && ASPx.Data.ArrayIndexOf(this.getCssClassMarker(), selectedElement.className, function(classMarker, className) { return className.indexOf(classMarker) > -1; }) > -1;
        }
    });

    var isSimpleImage = function(selectedElement) {
        return selectedElement && !(selectedElement.className.indexOf(ASPx.HtmlEditorClasses.MediaCssClasses.Audio) > -1 || 
                                selectedElement.className.indexOf(ASPx.HtmlEditorClasses.MediaCssClasses.Video) > -1 || 
                                selectedElement.className.indexOf(ASPx.HtmlEditorClasses.MediaCssClasses.Flash) > -1|| 
                                selectedElement.className.indexOf(ASPx.HtmlEditorClasses.MediaCssClasses.YouTube) > -1 ||
                                selectedElement.className.indexOf(ASPx.HtmlEditorClasses.MediaCssClasses.NotSupported) > -1);    
    };
    ASPx.HtmlEditorClasses.Commands.Dialogs = {
        PasteFromWord: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.DialogCommand, {
            Execute: function(cmdValue, wrapper) {
                if(!ASPx.Browser.WebKitTouchUI)
                    ASPx.HtmlEditorClasses.Commands.DialogCommand.prototype.Execute.call(this, cmdValue, wrapper);
                else
                    alert(ASPx.HtmlEditorClasses.Commands.Browser.Clipboard.NotAllowedMessage[ASPxClientCommandConsts.PASTE_COMMAND]);
            },
            IsLocked: function(wrapper) {
                return false;
            },
            canBeExecutedOnSelection: function(wrapper) {
                return true;
            }
        }),
        InsertImage: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.DialogCommand, {
            canBeExecutedOnSelectedElement: function(selectedElement) {
                return !selectedElement || isSimpleImage(selectedElement);
            }
        }),
        ChangeImage: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.DialogCommand, {
            canBeExecutedOnSelectedElement: function(selectedElement) {
                return isSimpleImage(selectedElement);
            }
        }),
        InsertLink: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.DialogCommand, {
            canBeExecutedOnSelectedElement: function(selectedElement) {
                return isSimpleImage(selectedElement) || !selectedElement;
            }
        }),
        ChangeLink: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.DialogCommand, {
            IsLocked: function (wrapper) {
                return !ASPx.HtmlEditorClasses.Utils.IsLinkSelected(wrapper);
            },
            canBeExecutedOnSelectedElement: function(selectedElement) {
                return isSimpleImage(selectedElement) || !selectedElement;
            }
        }),
        TableCellProperties: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.DialogCommand, {
            IsLocked: function(wrapper) {
                return !ASPx.HtmlEditorClasses.Commands.Tables.Cell.IsSelected(wrapper);
            }
        }),
        TableRowProperties: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.DialogCommand, {
            IsLocked: function(wrapper) {
                return !ASPx.HtmlEditorClasses.Commands.Tables.Cell.IsSelected(wrapper);
            }
        }),
        TableColumnProperties: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.DialogCommand, {
            IsLocked: function(wrapper) {
                return !ASPx.HtmlEditorClasses.Commands.Tables.Cell.IsSelected(wrapper);
            }
        }),
        ChangeTable: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.DialogCommand, {
            IsLocked: function(wrapper) {
                return !ASPx.HtmlEditorClasses.Commands.Tables.Table.IsSelected(wrapper);
            }
        }),
        CustomDialog: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.DialogCommand, {
            constructor: function(cmdID) {
		        this.constructor.prototype.constructor.call(this, cmdID);
                this.dialogs = {};
            },
            Execute: function(cmdValue, wrapper) {
                var customDialogName = "cd_" + cmdValue;

                if(!this.dialogs[customDialogName])
                    this.dialogs[customDialogName] = this.createCustomDialog(customDialogName, cmdValue);

                var dialog = this.dialogs[customDialogName];
                wrapper.executeDialog(dialog);
        
                return true;
            },
            DialogNotFound: function(dialogName) {
                delete this.dialogs[dialogName];
            },
            createCustomDialog: function(customDialogName, cmdValue) {
                return new ASPx.HtmlEditorClasses.Dialogs.ASPxCustomDialog(customDialogName, cmdValue);
            }
        }),
        // mdeia dialogs
        InsertAudio: ASPx.CreateClass(InsertMediaDialogBase, {
            getCssClassMarker: function() {
                return [ASPx.HtmlEditorClasses.MediaCssClasses.Audio];
            },
            IsLocked: function(wrapper) {
                return !wrapper.settings.allowHTML5MediaElements;
            },
            getChangeCommandID: function() {
                return ASPxClientCommandConsts.CHANGEAUDIO_DIALOG_COMMAND;    
            }
        }),
        InsertVideo: ASPx.CreateClass(InsertMediaDialogBase, {
            getCssClassMarker: function() {
                return [ASPx.HtmlEditorClasses.MediaCssClasses.Video];
            },
            IsLocked: function(wrapper) {
                return !wrapper.settings.allowHTML5MediaElements;
            },
            getChangeCommandID: function() {
                return ASPxClientCommandConsts.CHANGEVIDEO_DIALOG_COMMAND;    
            }
        }),
        InsertFlash: ASPx.CreateClass(InsertMediaDialogBase, {
            getCssClassMarker: function() {
                return [ASPx.HtmlEditorClasses.MediaCssClasses.Flash, ASPx.HtmlEditorClasses.MediaCssClasses.NotSupported];
            },
            getChangeCommandID: function() {
                return ASPxClientCommandConsts.CHANGEFLASH_DIALOG_COMMAND;    
            },
            IsLocked: function(wrapper) {
                return !wrapper.settings.allowObjectAndEmbedElements;
            }
        }),
        InsertYoutubeVideo: ASPx.CreateClass(InsertMediaDialogBase, {
            getCssClassMarker: function() {
                return [ASPx.HtmlEditorClasses.MediaCssClasses.YouTube];
            },
            getChangeCommandID: function() {
                return ASPxClientCommandConsts.CHANGEYOUTUBEVIDEO_DIALOG_COMMAND;    
            },
            IsLocked: function(wrapper) {
                return !wrapper.settings.allowYouTubeVideoIFrames;
            }
        }),
        ChangeAudio: ASPx.CreateClass(ChangeMediaDialogBase, {
            getCssClassMarker: function() {
                return [ASPx.HtmlEditorClasses.MediaCssClasses.Audio];
            },
            IsLocked: function(wrapper) {
                return !wrapper.settings.allowHTML5MediaElements;
            }
        }),
        ChangeVideo: ASPx.CreateClass(ChangeMediaDialogBase, {
            getCssClassMarker: function() {
                return [ASPx.HtmlEditorClasses.MediaCssClasses.Video];
            },
            IsLocked: function(wrapper) {
                return !wrapper.settings.allowHTML5MediaElements;
            }
        }),
        ChangeFlash: ASPx.CreateClass(ChangeMediaDialogBase, {
            getCssClassMarker: function() {
                return [ASPx.HtmlEditorClasses.MediaCssClasses.Flash, ASPx.HtmlEditorClasses.MediaCssClasses.NotSupported];
            },
            IsLocked: function(wrapper) {
                return !wrapper.settings.allowObjectAndEmbedElements;
            }
        }),
        ChangeYoutubeVideo: ASPx.CreateClass(ChangeMediaDialogBase, {
            getCssClassMarker: function() {
                return [ASPx.HtmlEditorClasses.MediaCssClasses.YouTube];
            },
            IsLocked: function(wrapper) {
                return !wrapper.settings.allowYouTubeVideoIFrames;
            }
        }),
        InsertPlaceholder: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.DialogCommand, {
            canBeExecutedOnSelection: function(wrapper, curSelection) {
                if(!curSelection) return true;
                var containerElement = curSelection.GetSelectedElement();
                return containerElement && containerElement.nodeName != "IMG";
            }
        }),
        ChangePlaceholder: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.DialogCommand, {
            canBeExecutedOnSelection: function(wrapper, curSelection) {
                if(!curSelection) return true;
                var containerElement = curSelection.GetSelectedElement();
                return ASPx.ElementHasCssClass(containerElement, ASPx.HtmlEditorClasses.PlaceholderCssClasseName);
            }
        }),
        ChangeElementProperties: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.DialogCommand, {
            canBeExecutedOnSelection: function (wrapper, curSelection) {
                if ((curSelection.selectedElement.nodeName == "BODY") || (this.getDialogCmdID(wrapper) != ASPxClientCommandConsts.CHANGEELEMENTPROPERTIES_DIALOG_COMMAND))
                    return false;
                return curSelection && !!curSelection.GetSelectedElement();
            },
            getDialogCmdID: function(wrapper, cmdValue) {
                var selectedElement = (cmdValue && cmdValue.selectedElement) ? cmdValue.selectedElement : wrapper.getSelection().GetSelectedElement();
                if(selectedElement) {
                    switch(selectedElement.tagName) {
                        case "TD":
                        case "TH":
                            return ASPxClientCommandConsts.TABLECELLPROPERTIES_DIALOG_COMMAND;
                        case "TR":
                            return ASPxClientCommandConsts.TABLEROWPROPERTIES_DIALOG_COMMAND;
                        case "TABLE":
                            return ASPxClientCommandConsts.TABLEPROPERTIES_DIALOG_COMMAND;
                        case "A":
                            return ASPxClientCommandConsts.CHANGELINK_DIALOG_COMMAND;
                        case "IMG":
                            if(selectedElement.className.indexOf(ASPx.HtmlEditorClasses.MediaCssClasses.YouTube) > -1)
                                return ASPxClientCommandConsts.CHANGEYOUTUBEVIDEO_DIALOG_COMMAND;
                            else if(selectedElement.className.indexOf(ASPx.HtmlEditorClasses.MediaCssClasses.Audio) > -1)
                                return ASPxClientCommandConsts.CHANGEAUDIO_DIALOG_COMMAND;
                            else if(selectedElement.className.indexOf(ASPx.HtmlEditorClasses.MediaCssClasses.Video) > -1)
                                return ASPxClientCommandConsts.CHANGEVIDEO_DIALOG_COMMAND;
                            else if(selectedElement.className.indexOf(ASPx.HtmlEditorClasses.MediaCssClasses.Flash) > -1 || 
                                selectedElement.className.indexOf(ASPx.HtmlEditorClasses.MediaCssClasses.NotSupported) > -1)
                                    return ASPxClientCommandConsts.CHANGEFLASH_DIALOG_COMMAND;
                            return ASPxClientCommandConsts.CHANGEIMAGE_DIALOG_COMMAND;
                    }
                }
                return this.commandID;
            }
        })
    };
})();