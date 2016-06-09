

(function() {
    var ASPxClientCommandConsts = {
        SHOWSEARCHPANEL_COMMAND: "showsearchpanel",
        FINDANDREPLACE_DIALOG_COMMAND: "findandreplacedialog",
	    BOLD_COMMAND : "bold",
        ITALIC_COMMAND: "italic",
        UNDERLINE_COMMAND: "underline",
        STRIKETHROUGH_COMMAND: "strikethrough",
        SUPERSCRIPT_COMMAND: "superscript",
        SUBSCRIPT_COMMAND: "subscript",
        JUSTIFYCENTER_COMMAND: "justifycenter",
        JUSTIFYLEFT_COMMAND: "justifyleft",
        INDENT_COMMAND: "indent",
        OUTDENT_COMMAND: "outdent",
        JUSTIFYRIGHT_COMMAND: "justifyright",
        JUSTIFYFULL_COMMAND: "justifyfull",
	    FONTSIZE_COMMAND : "fontsize",
	    FONTNAME_COMMAND : "fontname",
	    FONTCOLOR_COMMAND: "forecolor",
	    BACKCOLOR_COMMAND: "backcolor",
	    FORMATBLOCK_COMMAND: "formatblock",
	    APPLYCSS_COMMAND: "applycss",
	    REMOVEFORMAT_COMMAND: "removeformat",
	    UNDO_COMMAND: "undo",
	    REDO_COMMAND: "redo",
	    COPY_COMMAND: "copy",
	    KBCOPY_COMMAND: "kbcopy",
	    PASTE_COMMAND: "paste",
	    KBPASTE_COMMAND: "kbpaste",
	    PASTEFROMWORD_COMMAND: "pastefromword",
	    PASTEFROMWORDDIALOG_COMMAND: "pastefromworddialog",
	    CUT_COMMAND: "cut",
	    KBCUT_COMMAND: "kbcut",
	    SELECT_ALL: "selectall",
	    DELETE_COMMAND: "delete",	
	    KBDELETE_COMMAND: "kbdelete",
	
	    TEXTTYPE_COMMAND: "texttype",
	    NEWPARAGRAPHTYPE_COMMAND: "newparagraphtype",
	    LINEBREAKETYPE_COMMAND: "linebreaktype",
	
	    ENTER_COMMAND: "enter",
	    PASTEHTML_COMMAND: "pastehtml",
        RESIZEOBJECT_COMMAND: "resizeobject",
        DRAGDROPOBJECT_COMMAND: "dragdropobject",
        DROPOBJECTFROMEXTERNAL_COMMAND: "dropobjectfromexternal",
	    INSERTORDEREDLIST_COMMAND: "insertorderedlist",
	    INSERTUNORDEREDLIST_COMMAND: "insertunorderedlist",
	    RESTARTORDEREDLIST_COMMAND: "restartorderedlist",
	    CONTINUEORDEREDLIST_COMMAND: "continueorderedlist",
	    UNLINK_COMMAND: "unlink",
	    INSERTLINK_COMMAND: "insertlink",
	    INSERTIMAGE_COMMAND: "insertimage",
        CHANGEIMAGE_COMMAND: "changeimage",
        CHECKSPELLING_COMMAND: "checkspelling",
        INSERTIMAGE_DIALOG_COMMAND: "insertimagedialog",
        CHANGEIMAGE_DIALOG_COMMAND: "changeimagedialog",
	    INSERTLINK_DIALOG_COMMAND: "insertlinkdialog",
	    CHANGELINK_DIALOG_COMMAND: "changelinkdialog",
	    INSERTTABLE_DIALOG_COMMAND: "inserttabledialog",
	    TABLEPROPERTIES_DIALOG_COMMAND: "tablepropertiesdialog",
	    TABLECELLPROPERTIES_DIALOG_COMMAND: "tablecellpropertiesdialog",
	    TABLECOLUMNPROPERTIES_DIALOG_COMMAND: "tablecolumnpropertiesdialog",
	    TABLEROWPROPERTIES_DIALOG_COMMAND: "tablerowpropertiesdialog",
	    PRINT_COMMAND: "print",
	    FULLSCREEN_COMMAND: "fullscreen",
	

	    /* Table command */
	    INSERTTABLE_COMMAND: "inserttable",
	    CHANGETABLE_COMMAND: "changetable",
	    CHANGETABLECELL_COMMAND: "changetablecell",
	    CHANGETABLEROW_COMMAND: "changetablerow",
	    CHANGETABLECOLUMN_COMMAND: "changetablecolumn",
	    DELETETABLE_COMMAND: "deletetable",
	    DELETETABLEROW_COMMAND: "deletetablerow",
	    DELETETABLECOLUMN_COMMAND: "deletetablecolumn",
	    INSERTTABLECOLUMNTOLEFT_COMMAND: "inserttablecolumntoleft",
	    INSERTTABLECOLUMNTORIGHT_COMMAND: "inserttablecolumntoright",
	    INSERTTABLEROWBELOW_COMMAND: "inserttablerowbelow",
	    INSERTTABLEROWABOVE_COMMAND: "inserttablerowabove",
	    SPLITTABLECELLHORIZONTALLY_COMMAND: "splittablecellhorizontally",
	    SPLITTABLECELLVERTICALLY_COMMAND: "splittablecellvertically",
	    MERGETABLECELLRIGHT_COMMAND: "mergetablecellright",
	    MERGETABLECELLDOWN_COMMAND: "mergetablecelldown",
		
	    CheckSpellingCore_COMMAND: "checkspellingcore",
	    Start_COMMAND: "start",
        CUSTOMDIALOG_COMMAND: "customdialog",
        EXPORT_COMMAND: "export",

        SAVESTATEUNDOREDOSTACK_COMMAND: "savestateundoredostack",
        INSERTAUDIO_COMMAND: "insertaudio",
        INSERTVIDEO_COMMAND: "insertvideo",
        INSERTFLASH_COMMAND: "insertflash",
        INSERTYOUTUBEVIDEO_COMMAND: "insertyoutubevideo",
        CHANGEAUDIO_COMMAND: "changeaudio",
        CHANGEVIDEO_COMMAND: "changevideo",
        CHANGEFLASH_COMMAND: "changeflash",
        CHANGEYOUTUBEVIDEO_COMMAND: "changeyoutubevideo",
        INSERTAUDIO_DIALOG_COMMAND: "insertaudiodialog",
        INSERTVIDEO_DIALOG_COMMAND: "insertvideodialog",
        INSERTFLASH_DIALOG_COMMAND: "insertflashdialog",
        INSERTYOUTUBEVIDEO_DIALOG_COMMAND: "insertyoutubevideodialog",
        CHANGEAUDIO_DIALOG_COMMAND: "changeaudiodialog",
        CHANGEVIDEO_DIALOG_COMMAND: "changevideodialog",
        CHANGEFLASH_DIALOG_COMMAND: "changeflashdialog",
        CHANGEYOUTUBEVIDEO_DIALOG_COMMAND: "changeyoutubevideodialog",
        PASTEHTMLSOURCEFORMATTING_COMMAND: "pastehtmlsourceformatting",
        PASTEHTMLPLAINTEXT_COMMAND: "pastehtmlplaintext",
        PASTEHTMLMERGEFORMATTING_COMMAND: "pastehtmlmergeformatting",
        INSERTPLACEHOLDER_COMMAND: "insertplaceholder",
        CHANGEPLACEHOLDER_COMMAND: "changeplaceholder",
        INSERTPLACEHOLDER_DIALOG_COMMAND: "insertplaceholderdialog",
        CHANGEPLACEHOLDER_DIALOG_COMMAND: "changeplaceholderdialog",
        UPDATEDOCUMENT_COMMAND: "updatedocument",
        CHANGEELEMENTPROPERTIES_COMMAND: "changeelementproperties",
        CHANGEELEMENTPROPERTIES_DIALOG_COMMAND: "changeelementpropertiesdialog",
        COMMENT_COMMAND: "comment",
        UNCOMMENT_COMMAND: "uncomment",
        FORMATDOCUMENT_COMMAND: "formatdocument",
        INDENTLINE_COMMAND: "indentline",
        OUTDENTLINE_COMMAND: "outdentline",
        COLLAPSETAG_COMMAND: "collapsetag",
        EXPANDTAG_COMMAND: "expandtag",
        SHOWINTELLISENSE_COMMAND: "showintellisense",

        // private commands below:
	    DELETEELEMENT_COMMAND: "deleteelement",
        MERGETABLECELLLEFT_COMMAND: "mergetablecellleft",
        DELETEPLACEHOLDER_COMMAND: "deleteplaceholder",
        TABLETOOLSSTATE_COMMAND: "hetabletools"
    };

    var TypeCommandProcessor = ASPx.CreateClass(null,{
        constructor: function() {
            this.applyStyleCommands = [];
            this.removeStyleCommands = [];
        },
        hasApplyStyleCommand: function(commandName) {
            return this.hasCommand(commandName, this.applyStyleCommands);
        },
        hasRemoveStyleCommand: function(commandName) {
            return this.hasCommand(commandName, this.removeStyleCommands);
        },
        hasCommand: function(commandName, executeStyleCommands) {
            return commandName? ASPx.Data.ArrayIndexOf(executeStyleCommands, commandName) > -1 : executeStyleCommands.length;
        },
        hasStyleCommand: function() {
            return this.hasApplyStyleCommand() || this.hasRemoveStyleCommand();
        },
        changeApplyStyleCommand: function(command) {
            this.changeExecuteStyleCommand(command, this.applyStyleCommands);
        },
        changeRemoveStyleCommand: function(command) {
            this.changeExecuteStyleCommand(command, this.removeStyleCommands);
        },
        changeExecuteStyleCommand: function(command, executeStyleCommands) {
            var commandName = command.GetCommandName();
            var index = ASPx.Data.ArrayIndexOf(executeStyleCommands, commandName);
            if(index > -1)
                executeStyleCommands.splice(index, 1);
            if(index < 0 || !command.canBeRemoved())
                executeStyleCommands.push(commandName); 
        },
        clearCommands: function() {
            this.applyStyleCommands = [];
            this.removeStyleCommands = [];
        }
    });

    var emptySelection = "empty";


    ASPx.HtmlEditorClasses.Managers.CommandManagerBase = ASPx.CreateClass(null, {
        constructor: function(wrapper) {
            this.wrapper = wrapper;
            this.commandList = {};
            this.lastExecutedCommand = null;
            this.createCommands();
        },
        addCommand: function(cmdName, cmdClass) {
            this.commandList[cmdName] = new cmdClass(cmdName, this);
        },
        getDefaultCommands: function() {
            return ASPx.HtmlEditorClasses.DefaultCommands.getCommonCommands();
        },
        createCommands: function() {
            var defaultCommads = this.getDefaultCommands();
            for(var i = 0, commad; commad = defaultCommads[i]; i++)
                this.addCommand(commad.cmdName, commad.cmdClass);
        },
        executeCommand: function(cmdID, cmdValue) {
            this.lastExecutedCommand = cmdID;
            return this.getCommand(cmdID).Execute(cmdValue, this.wrapper);
        },
        getLastExecutedCommand: function() {
            return this.lastExecutedCommand;
        },
        clearLastExecutedCommand: function() {
            this.lastExecutedCommand = null;
        },
        getCommandList: function() {
            return this.commandList;
        },
        getCommand: function(name) {
            return this.getCommandList()[name];
        },
        isDefaultActionCommand: function(cmdID) {
            var cmd = this.getCommand(cmdID);
            return cmd && cmd.IsDefaultAction(this);
        }
    });

    ASPx.HtmlEditorClasses.Managers.HtmlViewCMCommandManager = ASPx.CreateClass(ASPx.HtmlEditorClasses.Managers.CommandManagerBase, {
        executeCommand: function(cmdID, cmdValue) {
            this.wrapper.eventManager.detachHtmlChangedEventToEditor();
            ASPx.HtmlEditorClasses.Managers.CommandManagerBase.prototype.executeCommand.call(this, cmdID, cmdValue);
            this.wrapper.eventManager.attachHtmlChangedEventToEditor();
        },
        getDefaultCommands: function() {
            return ASPx.HtmlEditorClasses.DefaultCommands.getHtmlViewCMCommands();
        }
    });

    ASPx.HtmlEditorClasses.Managers.HtmlViewMemoCommandManager = ASPx.CreateClass(ASPx.HtmlEditorClasses.Managers.CommandManagerBase, {
        getDefaultCommands: function() {
            return ASPx.HtmlEditorClasses.DefaultCommands.getHtmlViewMemoCommands();
        }
    });

    ASPx.HtmlEditorClasses.Managers.DesignCommandManager = ASPx.CreateClass(ASPx.HtmlEditorClasses.Managers.CommandManagerBase, {
        constructor: function(wrapper) {
            this.constructor.prototype.constructor.call(this, wrapper);
            this.lastPasteFormattingCommandObject = { commandName: "", html: "" };

            this.commandIdArray = [ ];
            this.currentCmdIDIndex = -1;

            // For Undo/Redo
            this.lastRestoreSelection = null;
            this.restoreHtmlArray = [ ];
            this.undoSelectionArray = [ ];
            this.redoSelectionArray = [ ];

            // executing command's state
            this.typeCommandProcessor = new TypeCommandProcessor(this);
            this.executeCommand(ASPxClientCommandConsts.Start_COMMAND, "null", true);
        },
        initDefaultCommandValues: function(barDocControl) {
            this.initFontNameCommand(barDocControl.getItemTemplateValuesByName(ASPxClientCommandConsts.FONTNAME_COMMAND));
            this.initCustomCssValueHashTable(barDocControl.getItemTemplateValuesByName(ASPxClientCommandConsts.APPLYCSS_COMMAND));
            var itemValues = barDocControl.getItemTemplateValuesByName(ASPxClientCommandConsts.FORMATBLOCK_COMMAND);
            this.getCommand(ASPxClientCommandConsts.FORMATBLOCK_COMMAND).setDefaultFormatBlockValues(itemValues);
        },
        initFontNameCommand: function(itemValues) {            
            var fontNameCommand = this.getCommand(ASPxClientCommandConsts.FONTNAME_COMMAND);
            var defaultFontNames = fontNameCommand.getDefaultFontNames();
            var compare = function(itemValue, value) {
                return itemValue.toLowerCase() == value.toLowerCase();
            };
            for(var i = 0, value; value = itemValues[i]; i++) {
                if(ASPx.Data.ArrayIndexOf(defaultFontNames, value, compare) == -1)
                    defaultFontNames.push(value);
            }
            fontNameCommand.setDefaultFontNames(defaultFontNames);
        },
        initCustomCssValueHashTable: function(itemValues) {
            var customCssValueHashTable = {};
            for(var i = 0, itemValue; itemValue = itemValues[i]; i++) {
                var valueArray = itemValue.split("|");
                customCssValueHashTable[itemValue] = { tagName: valueArray[0], cssClass: valueArray[1] };
            }
            this.getCommand(ASPxClientCommandConsts.APPLYCSS_COMMAND).setCustomCssValueHashTable(customCssValueHashTable);
        },
        executeCommand: function(cmdID, cmdValue, addToUndoHistory) {
            this.lastExecutedCommand = cmdID;
            var isSuccessfully = false;
            var command = this.getCommand(cmdID);
            if (!command.IsHtmlChangeable() || !addToUndoHistory)
                isSuccessfully = command.Execute(cmdValue, this.wrapper);
            else {
                this.onCommandExecuting(cmdID);
                if(cmdID != ASPxClientCommandConsts.Start_COMMAND) {
                    var selectedElement = this.wrapper.getSelection().GetSelectedElement();
                    this.wrapper.placeholderManager.setAllowEditFilds(true);
                    if(ASPx.ElementHasCssClass(selectedElement, ASPx.HtmlEditorClasses.SelectedContainerCssClasseName))
                        this.wrapper.getSelection().clientSelection.applySpecialSelectionToElement(selectedElement, true);
                }
                isSuccessfully = command.Execute(cmdValue, this.wrapper);
                if(cmdID != ASPxClientCommandConsts.Start_COMMAND) {
                    var selectedElement = this.wrapper.getSelection().GetSelectedElement();
                    if(ASPx.HtmlEditorClasses.Utils.needToUseSpecialSelection(selectedElement) && cmdID != ASPxClientCommandConsts.CUT_COMMAND && cmdID != ASPxClientCommandConsts.KBCUT_COMMAND) {
                        if(selectedElement.firstChild && selectedElement.firstChild.nodeType == 1 && selectedElement.firstChild.className != ASPx.HtmlEditorClasses.PlaceholderStartMarkCssClasseName) {
                            var firstChild = selectedElement.firstChild;
                            selectedElement.parentNode.insertBefore(firstChild, selectedElement);
                            selectedElement.innerHTML = firstChild.innerHTML;
                            firstChild.innerHTML = "";
                            firstChild.appendChild(selectedElement);
                        }
                        var func = function() { this.wrapper.getSelection().clientSelection.applySpecialSelectionToElement(selectedElement); }.aspxBind(this);
                        if(ASPx.Browser.NetscapeFamily)
                            setTimeout(function() { func(); }, 100);
                        else
                            func();
                    }
                    this.wrapper.placeholderManager.updateFildsArray();
                    if(cmdID != ASPxClientCommandConsts.CUT_COMMAND && cmdID != ASPxClientCommandConsts.KBCUT_COMMAND)
                        this.wrapper.placeholderManager.setAllowEditFilds(false);
                }
                // IsReversable == false only for Undo/Redo
                var needAddToStack = isSuccessfully && command.IsReversable();
                if(needAddToStack) {
                    this.clearActionsToRedo();
                    this.currentCmdIDIndex = this.commandIdArray.length;
                    this.commandIdArray.push(cmdID);
                }
                this.onCommandExecuted(needAddToStack, this.wrapper);
            }
            return isSuccessfully;
        },
        undo: function(depth) {
            if(this.isUndoAvailable()) {
                this.typeCommandProcessor.clearCommands();
                depth = depth || 1;
                depth = Math.min(depth, this.commandIdArray.length);
                var actionCount = this.commandIdArray.length;
                depth = Math.min(depth, this.commandIdArray.length);
                while ((depth > 0) && (this.currentCmdIDIndex > 0) && (this.currentCmdIDIndex < actionCount)) {
                    this.wrapper.setInnerHtmlToBody(this.getRestoreText(this.currentCmdIDIndex - 1));
                    this.getUndoSelection(this.currentCmdIDIndex - 1).Restore();
                    this.currentCmdIDIndex--;
                    depth--;
                }
                return true;
            }
            return false;
        },
        redo: function(depth) {
            if(this.isRedoAvailable()) {
                this.typeCommandProcessor.clearCommands();
                depth = depth || 1;
                depth = Math.min(depth, this.commandIdArray.length);
                var actionIndex = this.currentCmdIDIndex + 1;
                while (depth > 0 && this.commandIdArray.length >= actionIndex) {
                    this.wrapper.setInnerHtmlToBody(this.getRestoreText(actionIndex));
                    this.getRedoSelection(actionIndex).Restore();
                    this.currentCmdIDIndex = actionIndex;
                    actionIndex++;
                    depth--;
                }
                return true;
            }
            return false;
        },
        // Execute operation for previous command
        onCommandExecuting: function(cmdID) {
            if(this.isUndoRedoCommand(cmdID) &&
                ASPx.IsExists(this.commandIdArray[this.currentCmdIDIndex + 1]))
                return;
            if(ASPx.IsExists(this.commandIdArray[this.currentCmdIDIndex])) {
                var prevActionUndoSelection = this.wrapper.saveLastSelection();
                if (this.isLastCommandImmediateExecute()) {//prev command
                    this.addNewItemToArray(this.restoreHtmlArray, this.currentCmdIDIndex, this.lastHTML);
                    this.updateOrAddNewItemToArray(this.undoSelectionArray, this.currentCmdIDIndex, prevActionUndoSelection);
                    this.addNewItemToArray(this.redoSelectionArray, this.currentCmdIDIndex,
                            ASPx.CloneObject(this.lastRestoreSelection));
                }
                else
                // undo selection for previous command
                    this.undoSelectionArray[this.currentCmdIDIndex] = prevActionUndoSelection;
            }
        },
        onCommandExecuted: function(needAddToStack) {
            var lastCmdID = this.getLastCommandID();
            if (needAddToStack && !this.getCommand(lastCmdID).IsImmediateExecution()) {
                this.addNewItemToArray(this.restoreHtmlArray, this.currentCmdIDIndex, this.getEditorHtml());
                this.addNewItemToArray(this.undoSelectionArray, this.currentCmdIDIndex, emptySelection);
                try { // B192803
                    var selection = lastCmdID == ASPxClientCommandConsts.Start_COMMAND ? emptySelection : this.wrapper.saveLastSelection();
                } catch(e) {
                    var selection = emptySelection;
                }
                this.addNewItemToArray(this.redoSelectionArray, this.currentCmdIDIndex, selection);
            }
        },

        // Undo/redo opearations
        clearActionsToRedo: function() {
            if (this.isRedoAvailable()) {
                this.lastRestoreSelection = null;

                var startIndex = this.currentCmdIDIndex + 1;
                var length = this.commandIdArray.length - this.currentCmdIDIndex;

                this.commandIdArray.splice(startIndex, length);
                this.restoreHtmlArray.splice(startIndex, length);
                this.undoSelectionArray.splice(startIndex, length);
                this.redoSelectionArray.splice(startIndex, length);
            }
        },
        clearUndoHistory: function() {
            this.lastRestoreSelection = null;

            this.currentCmdIDIndex = -1;
            this.commandIdArray.length = 0;
            this.commandIdArray.length = 0;
            this.restoreHtmlArray.length = 0;
            this.undoSelectionArray.length = 0;
            this.redoSelectionArray.length = 0;
            this.executeCommand(ASPxClientCommandConsts.Start_COMMAND, "null", true);
            this.clearPasteOptions(true);
        },
        cleanEmptyRestoreHtml: function() {
            if (this.getEditorHtml() == this.lastHTML) {
                ASPx.Data.ArrayRemoveAt(this.commandIdArray, this.currentCmdIDIndex);
                this.currentCmdIDIndex = this.commandIdArray.length - 1;
                return true;
            }
            return false;
        },
        getEditorHtml: function() {
            return this.wrapper.getRawHtml();
        },
        getRestoreText: function(index) {
            return this.restoreHtmlArray[index];
        },
        getRedoSelection: function(index) {
            return this.redoSelectionArray[index];
        },
        getUndoSelection: function(index) {
            return this.undoSelectionArray[index];
        },
        isRedoAvailable: function() {
            return (this.commandIdArray.length - 1 > this.currentCmdIDIndex);
        },
        isUndoAvailable: function() {
            return this.currentCmdIDIndex > 0;
        },
        updateLastRestoreSelectionAndHTML: function() {
            if(this.lastRestoreSelection == null)
                this.lastRestoreSelection = ASPxClientHtmlEditorSelection.Create(this.wrapper.getWindow());
            this.lastRestoreSelection.Save();
            this.updateLastRestoreHtml();
        },
        updateLastRestoreHtml: function() {
            this.lastHTML = this.getEditorHtml();
        },
        updateLastItemInRestoreHtmlArray: function() {
            this.restoreHtmlArray[this.restoreHtmlArray.length - 1] = this.getEditorHtml();
        },
    
        // Utils
        isLastCommandImmediateExecute: function() {
            var lastCmdID = this.getLastCommandID();
            var isImmediateExecution = this.getCommand(lastCmdID).IsImmediateExecution();
            return (lastCmdID != null) && isImmediateExecution &&
	                (this.currentCmdIDIndex == this.commandIdArray.length - 1);
        },
        isUndoRedoCommand: function(cmdID) {
            return (cmdID == ASPxClientCommandConsts.REDO_COMMAND) ||
	                (cmdID == ASPxClientCommandConsts.UNDO_COMMAND);
        },
        getLastCommandID: function() {
            var curAction = this.commandIdArray[this.currentCmdIDIndex];
            return ASPx.IsExists(curAction) ? curAction : null;
        },
        addNewItemToArray: function(array, index, value) {
            if (!ASPx.IsExists(array[index]))
                array.push(value);
        },
        updateOrAddNewItemToArray: function(array, index, value) {
            if (!ASPx.IsExists(array[index]))
                array.push(value);
            else if (array[index] == emptySelection)
                array[index] = value;
        },
        isHtmlChangeableCommand: function(cmdID) {
            var cmd = this.getCommand(cmdID);
            return cmd && cmd.IsHtmlChangeable();
        },
        isTextTyping: function() {
            var lastKeyDown = this.wrapper.getLastKeyDownInfo();
            return this.getLastExecutedCommand() == ASPxClientCommandConsts.TEXTTYPE_COMMAND && lastKeyDown && (!lastKeyDown.isSystemKey || lastKeyDown.isSpaceKey);
        },
        isDeleting: function() {
            var lastKeyDown = this.wrapper.getLastKeyDownInfo();            
            var lastCmdID = this.getLastCommandID();
            return (lastCmdID != null) && (lastCmdID == ASPxClientCommandConsts.KBDELETE_COMMAND) &&
	                (this.currentCmdIDIndex == this.commandIdArray.length - 1) && (lastKeyDown && lastKeyDown.isDeleteOrBackSpaceKey);
        },
        getLastPasteFormattingCommandName: function() {
            return this.lastPasteFormattingCommandObject.commandName;
        },
        getLastPasteFormattingHtml: function() {
            return this.lastPasteFormattingCommandObject.html;
        },
        clearPasteOptions: function(hidePasteOptionsBar) {
            this.getCommand(ASPxClientCommandConsts.PASTEHTMLSOURCEFORMATTING_COMMAND).initPasteContent(this.wrapper, null);
            this.getCommand(ASPxClientCommandConsts.PASTEHTMLPLAINTEXT_COMMAND).initPasteContent(this.wrapper, null);
            this.lastPasteFormattingCommandObject = { commandName: "", html: "" };
            if(hidePasteOptionsBar)
                this.wrapper.hidePasteOptionsBar();
        },
        getDefaultCommands: function() {
            return ASPx.HtmlEditorClasses.DefaultCommands.getDesignViewCommands();
        }
    });

    ASPx.HtmlEditorClasses.DefaultCommands = { 
        addToList: function(list, cmdName, cmdClass) {
            list.push({ cmdName: cmdName, cmdClass: cmdClass });
        },
        getCommonCommands: function() {
            var result = [];
            this.addToList(result, ASPxClientCommandConsts.FULLSCREEN_COMMAND, ASPx.HtmlEditorClasses.Commands.Fullscreen);
            return result;
        },
        getHtmlViewCMCommands: function() {
            var result = this.getCommonCommands();
            this.addToList(result, ASPxClientCommandConsts.UNDO_COMMAND, ASPx.HtmlEditorClasses.Commands.HtmlView.Undo);
            this.addToList(result, ASPxClientCommandConsts.REDO_COMMAND, ASPx.HtmlEditorClasses.Commands.HtmlView.Redo);

            this.addToList(result, ASPxClientCommandConsts.INDENTLINE_COMMAND, ASPx.HtmlEditorClasses.Commands.HtmlView.IndentLine);
            this.addToList(result, ASPxClientCommandConsts.OUTDENTLINE_COMMAND, ASPx.HtmlEditorClasses.Commands.HtmlView.IndentLine);
            this.addToList(result, ASPxClientCommandConsts.FORMATDOCUMENT_COMMAND, ASPx.HtmlEditorClasses.Commands.HtmlView.FormatDocumentCommand);

            this.addToList(result, ASPxClientCommandConsts.SELECT_ALL, ASPx.HtmlEditorClasses.Commands.HtmlView.SelectAll);
            this.addToList(result, ASPxClientCommandConsts.PASTEHTML_COMMAND, ASPx.HtmlEditorClasses.Commands.HtmlView.PasteHtml);

            this.addToList(result, ASPxClientCommandConsts.COMMENT_COMMAND, ASPx.HtmlEditorClasses.Commands.HtmlView.CommentCommand);
            this.addToList(result, ASPxClientCommandConsts.UNCOMMENT_COMMAND, ASPx.HtmlEditorClasses.Commands.HtmlView.UncommentCommand);

            this.addToList(result, ASPxClientCommandConsts.COLLAPSETAG_COMMAND, ASPx.HtmlEditorClasses.Commands.HtmlView.CollapseTagCommand);
            this.addToList(result, ASPxClientCommandConsts.EXPANDTAG_COMMAND, ASPx.HtmlEditorClasses.Commands.HtmlView.ExpandTagCommand);

            this.addToList(result, ASPxClientCommandConsts.SHOWINTELLISENSE_COMMAND,  ASPx.HtmlEditorClasses.Commands.HtmlView.ShowIntelliSenseWindowCommand);
            
            this.addToList(result, ASPxClientCommandConsts.SHOWSEARCHPANEL_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.QuickSearch);
            this.addToList(result, ASPxClientCommandConsts.FINDANDREPLACE_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.FindAndReplaceDialog);
            return result;
        },
        getHtmlViewMemoCommands: function() {
            var result = this.getCommonCommands();
            this.addToList(result, ASPxClientCommandConsts.FORMATDOCUMENT_COMMAND, ASPx.HtmlEditorClasses.Commands.HtmlView.FormatDocumentCommand);
            return result;
        },
        getDesignViewCommands: function() {
            var result = this.getCommonCommands();
            this.addToList(result, ASPxClientCommandConsts.BOLD_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.FontStyle);
            this.addToList(result, ASPxClientCommandConsts.ITALIC_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.FontStyle);
            this.addToList(result, ASPxClientCommandConsts.UNDERLINE_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.FontStyle);
            this.addToList(result, ASPxClientCommandConsts.STRIKETHROUGH_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.FontStyle);
            this.addToList(result, ASPxClientCommandConsts.JUSTIFYCENTER_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.Justify);
            this.addToList(result, ASPxClientCommandConsts.JUSTIFYLEFT_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.Justify);
            this.addToList(result, ASPxClientCommandConsts.JUSTIFYRIGHT_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.Justify);
            this.addToList(result, ASPxClientCommandConsts.JUSTIFYFULL_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.Justify);
            this.addToList(result, ASPxClientCommandConsts.SUPERSCRIPT_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.Command);
            this.addToList(result, ASPxClientCommandConsts.SUBSCRIPT_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.Command);
            this.addToList(result, ASPxClientCommandConsts.INDENT_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.Indent);
            this.addToList(result, ASPxClientCommandConsts.OUTDENT_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.Indent);

            this.addToList(result, ASPxClientCommandConsts.SHOWSEARCHPANEL_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.QuickSearch);
            this.addToList(result, ASPxClientCommandConsts.FINDANDREPLACE_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.FindAndReplaceDialog);

            this.addToList(result, ASPxClientCommandConsts.INSERTORDEREDLIST_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.InsertList);
            this.addToList(result, ASPxClientCommandConsts.INSERTUNORDEREDLIST_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.InsertList);
            this.addToList(result, ASPxClientCommandConsts.RESTARTORDEREDLIST_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.RestartOrderedList);
            this.addToList(result, ASPxClientCommandConsts.CONTINUEORDEREDLIST_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.ContinueOrderedList);

            this.addToList(result, ASPxClientCommandConsts.SELECT_ALL, ASPx.HtmlEditorClasses.Commands.Browser.SelectAll);

            this.addToList(result, ASPxClientCommandConsts.PASTE_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.Clipboard);
            this.addToList(result, ASPxClientCommandConsts.PASTEFROMWORD_COMMAND, ASPx.HtmlEditorClasses.Commands.PasteFromWord);
            this.addToList(result, ASPxClientCommandConsts.CUT_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.Clipboard);
            this.addToList(result, ASPxClientCommandConsts.COPY_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.Clipboard);
            this.addToList(result, ASPxClientCommandConsts.KBPASTE_COMMAND, ASPx.HtmlEditorClasses.Commands.KbPaste);
            this.addToList(result, ASPxClientCommandConsts.KBCUT_COMMAND, ASPx.HtmlEditorClasses.Commands.KbCut);
            this.addToList(result, ASPxClientCommandConsts.KBCOPY_COMMAND, ASPx.HtmlEditorClasses.Commands.KbCopy);

            this.addToList(result, ASPxClientCommandConsts.FONTSIZE_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.FontSize);
            this.addToList(result, ASPxClientCommandConsts.FONTNAME_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.FontName);
            this.addToList(result, ASPxClientCommandConsts.FONTCOLOR_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.FontColor);
            this.addToList(result, ASPxClientCommandConsts.BACKCOLOR_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.BgColor);

            this.addToList(result, ASPxClientCommandConsts.APPLYCSS_COMMAND, ASPx.HtmlEditorClasses.Commands.ApplyCss);
            this.addToList(result, ASPxClientCommandConsts.FORMATBLOCK_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.FormatBlock);
            this.addToList(result, ASPxClientCommandConsts.REMOVEFORMAT_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.RemoveFormat);

            this.addToList(result, ASPxClientCommandConsts.UNDO_COMMAND, ASPx.HtmlEditorClasses.Commands.Undo);
            this.addToList(result, ASPxClientCommandConsts.REDO_COMMAND, ASPx.HtmlEditorClasses.Commands.Redo);

            this.addToList(result, ASPxClientCommandConsts.LINEBREAKETYPE_COMMAND, ASPx.HtmlEditorClasses.Commands.LineBreakType);
            this.addToList(result, ASPxClientCommandConsts.NEWPARAGRAPHTYPE_COMMAND, ASPx.HtmlEditorClasses.Commands.NewParagraphType);
            this.addToList(result, ASPxClientCommandConsts.TEXTTYPE_COMMAND, ASPx.HtmlEditorClasses.Commands.TextType);
            this.addToList(result, ASPxClientCommandConsts.RESIZEOBJECT_COMMAND, ASPx.HtmlEditorClasses.Commands.TextType);
            this.addToList(result, ASPxClientCommandConsts.DRAGDROPOBJECT_COMMAND, ASPx.HtmlEditorClasses.Commands.TextType);
            this.addToList(result, ASPxClientCommandConsts.DROPOBJECTFROMEXTERNAL_COMMAND, ASPx.HtmlEditorClasses.Commands.TextType);
            this.addToList(result, ASPxClientCommandConsts.DELETE_COMMAND, ASPx.HtmlEditorClasses.Commands.Delete);
            this.addToList(result, ASPxClientCommandConsts.KBDELETE_COMMAND, ASPx.HtmlEditorClasses.Commands.DeleteWithoutSelection);
            this.addToList(result, ASPxClientCommandConsts.ENTER_COMMAND, ASPx.HtmlEditorClasses.Commands.Enter);

            this.addToList(result, ASPxClientCommandConsts.PASTEHTML_COMMAND, ASPx.HtmlEditorClasses.Commands.PasteHtml);

            this.addToList(result, ASPxClientCommandConsts.INSERTLINK_COMMAND, ASPx.HtmlEditorClasses.Commands.InsertLink);
            this.addToList(result, ASPxClientCommandConsts.UNLINK_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.Unlink);

            this.addToList(result, ASPxClientCommandConsts.CHANGEIMAGE_COMMAND, ASPx.HtmlEditorClasses.Commands.ChangeImage);
            this.addToList(result, ASPxClientCommandConsts.INSERTIMAGE_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.InsertImage);

            this.addToList(result, ASPxClientCommandConsts.CHECKSPELLING_COMMAND, ASPx.HtmlEditorClasses.Commands.CheckSpelling);
            this.addToList(result, ASPxClientCommandConsts.CheckSpellingCore_COMMAND, ASPx.HtmlEditorClasses.Commands.CheckSpellingCore);

            this.addToList(result, ASPxClientCommandConsts.PRINT_COMMAND, ASPx.HtmlEditorClasses.Commands.Print);

            this.addToList(result, ASPxClientCommandConsts.DELETEELEMENT_COMMAND, ASPx.HtmlEditorClasses.Commands.DeleteElement);
            // *** Table commands ***
            this.addToList(result, ASPxClientCommandConsts.INSERTTABLE_COMMAND, ASPx.HtmlEditorClasses.Commands.Tables.Table.Insert);
            this.addToList(result, ASPxClientCommandConsts.CHANGETABLE_COMMAND, ASPx.HtmlEditorClasses.Commands.Tables.Table.Change);
            this.addToList(result, ASPxClientCommandConsts.DELETETABLE_COMMAND, ASPx.HtmlEditorClasses.Commands.Tables.Table.Delete);

            this.addToList(result, ASPxClientCommandConsts.INSERTTABLECOLUMNTOLEFT_COMMAND, ASPx.HtmlEditorClasses.Commands.Tables.Column.Insert);
            this.addToList(result, ASPxClientCommandConsts.INSERTTABLECOLUMNTORIGHT_COMMAND, ASPx.HtmlEditorClasses.Commands.Tables.Column.Insert);
            this.addToList(result, ASPxClientCommandConsts.CHANGETABLECOLUMN_COMMAND, ASPx.HtmlEditorClasses.Commands.Tables.Column.Change);
            this.addToList(result, ASPxClientCommandConsts.DELETETABLECOLUMN_COMMAND, ASPx.HtmlEditorClasses.Commands.Tables.Column.Delete);

            this.addToList(result, ASPxClientCommandConsts.INSERTTABLEROWBELOW_COMMAND, ASPx.HtmlEditorClasses.Commands.Tables.Row.Insert);
            this.addToList(result, ASPxClientCommandConsts.INSERTTABLEROWABOVE_COMMAND, ASPx.HtmlEditorClasses.Commands.Tables.Row.Insert);
            this.addToList(result, ASPxClientCommandConsts.CHANGETABLEROW_COMMAND, ASPx.HtmlEditorClasses.Commands.Tables.Row.Change);
            this.addToList(result, ASPxClientCommandConsts.DELETETABLEROW_COMMAND, ASPx.HtmlEditorClasses.Commands.Tables.Row.Delete);

            this.addToList(result, ASPxClientCommandConsts.CHANGETABLECELL_COMMAND, ASPx.HtmlEditorClasses.Commands.Tables.Cell.Change);
            this.addToList(result, ASPxClientCommandConsts.SPLITTABLECELLHORIZONTALLY_COMMAND, ASPx.HtmlEditorClasses.Commands.Tables.Cell.SplitHorizontally);
            this.addToList(result, ASPxClientCommandConsts.SPLITTABLECELLVERTICALLY_COMMAND, ASPx.HtmlEditorClasses.Commands.Tables.Cell.SplitVertically);
            this.addToList(result, ASPxClientCommandConsts.MERGETABLECELLRIGHT_COMMAND, ASPx.HtmlEditorClasses.Commands.Tables.Cell.MergeRight);
            this.addToList(result, ASPxClientCommandConsts.MERGETABLECELLLEFT_COMMAND, ASPx.HtmlEditorClasses.Commands.Tables.Cell.MergeLeft);
            this.addToList(result, ASPxClientCommandConsts.MERGETABLECELLDOWN_COMMAND, ASPx.HtmlEditorClasses.Commands.Tables.Cell.MergeDown);

            // *** Dialog Commands *** 
            this.addToList(result, ASPxClientCommandConsts.PASTEFROMWORDDIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.PasteFromWord);
            this.addToList(result, ASPxClientCommandConsts.CHANGEIMAGE_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.ChangeImage);
            this.addToList(result, ASPxClientCommandConsts.INSERTIMAGE_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.InsertImage);
            this.addToList(result, ASPxClientCommandConsts.INSERTLINK_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.InsertLink);
            this.addToList(result, ASPxClientCommandConsts.CHANGELINK_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.ChangeLink);
            this.addToList(result, ASPxClientCommandConsts.TABLEPROPERTIES_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.ChangeTable);
            this.addToList(result, ASPxClientCommandConsts.INSERTTABLE_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.DialogCommand);

            this.addToList(result, ASPxClientCommandConsts.TABLECELLPROPERTIES_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.TableCellProperties);
            this.addToList(result, ASPxClientCommandConsts.TABLECOLUMNPROPERTIES_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.TableColumnProperties);
            this.addToList(result, ASPxClientCommandConsts.TABLEROWPROPERTIES_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.TableRowProperties);

            this.addToList(result, ASPxClientCommandConsts.CUSTOMDIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.CustomDialog);


            this.addToList(result, ASPxClientCommandConsts.Start_COMMAND, ASPx.HtmlEditorClasses.Commands.Command);

            this.addToList(result, ASPxClientCommandConsts.SAVESTATEUNDOREDOSTACK_COMMAND, ASPx.HtmlEditorClasses.Commands.Command);

            this.addToList(result, ASPxClientCommandConsts.EXPORT_COMMAND, ASPx.HtmlEditorClasses.Commands.SaveAs);
            
            // media commands
            this.addToList(result, ASPxClientCommandConsts.INSERTAUDIO_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.InsertAudio);
            this.addToList(result, ASPxClientCommandConsts.INSERTVIDEO_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.InsertVideo);
            this.addToList(result, ASPxClientCommandConsts.INSERTFLASH_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.InsertFlash);
            this.addToList(result, ASPxClientCommandConsts.INSERTYOUTUBEVIDEO_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.InsertYoutubeVideo);

            this.addToList(result, ASPxClientCommandConsts.CHANGEAUDIO_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.ChangeAudio);
            this.addToList(result, ASPxClientCommandConsts.CHANGEVIDEO_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.ChangeVideo);
            this.addToList(result, ASPxClientCommandConsts.CHANGEFLASH_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.ChangeFlash);
            this.addToList(result, ASPxClientCommandConsts.CHANGEYOUTUBEVIDEO_COMMAND, ASPx.HtmlEditorClasses.Commands.Browser.ChangeYoutubeVideo);

            this.addToList(result, ASPxClientCommandConsts.INSERTAUDIO_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.InsertAudio);
            this.addToList(result, ASPxClientCommandConsts.INSERTVIDEO_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.InsertVideo);
            this.addToList(result, ASPxClientCommandConsts.INSERTFLASH_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.InsertFlash);
            this.addToList(result, ASPxClientCommandConsts.INSERTYOUTUBEVIDEO_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.InsertYoutubeVideo);

            this.addToList(result, ASPxClientCommandConsts.CHANGEAUDIO_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.ChangeAudio);
            this.addToList(result, ASPxClientCommandConsts.CHANGEVIDEO_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.ChangeVideo);
            this.addToList(result, ASPxClientCommandConsts.CHANGEFLASH_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.ChangeFlash);
            this.addToList(result, ASPxClientCommandConsts.CHANGEYOUTUBEVIDEO_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.ChangeYoutubeVideo);

            this.addToList(result, ASPxClientCommandConsts.PASTEHTMLSOURCEFORMATTING_COMMAND, ASPx.HtmlEditorClasses.Commands.PasteHtmlSourceFormatting);
            this.addToList(result, ASPxClientCommandConsts.PASTEHTMLPLAINTEXT_COMMAND, ASPx.HtmlEditorClasses.Commands.PasteHtmlPlainText);
            this.addToList(result, ASPxClientCommandConsts.PASTEHTMLMERGEFORMATTING_COMMAND, ASPx.HtmlEditorClasses.Commands.PasteHtmlMergeFormatting);

            this.addToList(result, ASPxClientCommandConsts.CHANGEELEMENTPROPERTIES_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.ChangeElementProperties);
            this.addToList(result, ASPxClientCommandConsts.CHANGEELEMENTPROPERTIES_COMMAND, ASPx.HtmlEditorClasses.Commands.ChangeElementProperties);
            // Placeholder commands
            this.addToList(result, ASPxClientCommandConsts.INSERTPLACEHOLDER_COMMAND, ASPx.HtmlEditorClasses.Commands.InsertPlaceholder);
            this.addToList(result, ASPxClientCommandConsts.CHANGEPLACEHOLDER_COMMAND, ASPx.HtmlEditorClasses.Commands.ChangePlaceholder);
            this.addToList(result, ASPxClientCommandConsts.INSERTPLACEHOLDER_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.InsertPlaceholder);
            this.addToList(result, ASPxClientCommandConsts.CHANGEPLACEHOLDER_DIALOG_COMMAND, ASPx.HtmlEditorClasses.Commands.Dialogs.ChangePlaceholder);
            this.addToList(result, ASPxClientCommandConsts.DELETEPLACEHOLDER_COMMAND, ASPx.HtmlEditorClasses.Commands.DeletePlaceholder);

            this.addToList(result, ASPxClientCommandConsts.UPDATEDOCUMENT_COMMAND, ASPx.HtmlEditorClasses.Commands.UpdateDocument);
            this.addToList(result, ASPxClientCommandConsts.TABLETOOLSSTATE_COMMAND, ASPx.HtmlEditorClasses.Commands.TableToolsState);
            return result;
        },
        getPreviewCommands: function() {
            return this.getCommonCommands();
        }
    };

    window.ASPxClientCommandConsts = ASPxClientCommandConsts;
})();