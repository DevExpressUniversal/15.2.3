(function() {
// TODO think about merge with ASPxHtmlEditorKeyboardManager
var SpreadsheetKeyboardManager = ASPx.CreateClass(null, {
    constructor: function() {
        this.shortcutCommands = [];
    },
    InitializeShortcuts: function(standatdSCs, customSCs) {
        for(var i = 0; i < standatdSCs.length; i++)
            this.AddShortcut(standatdSCs[i][1], standatdSCs[i][0]);
        if(customSCs) {
            for(var key in customSCs) {
                if(customSCs.hasOwnProperty(key) && key != "None")
                    this.AddShortcut(key, customSCs[key]);
            }
        }
    },
    AddShortcut: function(shortcutString, commandID) {
        var shortcutCode = ASPx.ParseShortcutString(shortcutString);
        this.shortcutCommands[shortcutCode] = commandID;
    },
    GetShortcutCommand: function(evt) {
        //only for mac and safari
        var ctrlKeyPressed = evt.ctrlKey || evt.metaKey;
        var shortcutCode = ASPx.GetShortcutCode(evt.keyCode, ctrlKeyPressed, evt.shiftKey, evt.altKey);
        return this.shortcutCommands[shortcutCode];
    },
    IsBrowserShortcut: function(evt) {
        if(ASPx.Browser.WebKitFamily || ASPx.Browser.IE) {
            var shortcutCode = ASPx.GetShortcutCode(evt.keyCode, evt.ctrlKey, evt.shiftKey, evt.altKey);
            return ASPx.Data.ArrayIndexOf(ASPxHtmlEditorKeyboardManager.BrowserShortcuts, shortcutCode) > -1;
        }
    },
    GetKeyDownInfo: function(evt) { // call only on keydown
        return {
            isSystemKey: ASPxHtmlEditorKeyboardManager.IsSystemKey(evt.keyCode),
            isDeleteOrBackSpaceKey: ASPxHtmlEditorKeyboardManager.IsDeleteOrBackSpaceKey(evt.keyCode),
            isBackSpaceKey: ASPxHtmlEditorKeyboardManager.IsBackSpaceKey(evt.keyCode),
            isSpaceKey: ASPxHtmlEditorKeyboardManager.IsSpaceKey(evt.keyCode),
            isCursorMovingKey: ASPxHtmlEditorKeyboardManager.IsCursorMovingKey(evt.keyCode)
        };
    },

    EventKeyCodeChangesTheInput: function(evt) {
        if(ASPx.IsPasteShortcut(evt))
            return true;
        else if(evt.ctrlKey && !evt.altKey)
            return false;

        var keyCode = ASPx.Evt.GetKeyCode(evt);
        var isSystemKey = ASPx.Key.Windows <= keyCode && keyCode <= ASPx.Key.ContextMenu;
        var isFKey = ASPx.Key.F1 <= keyCode && keyCode <= 127; // 127 - F16 for MAC
        return ASPx.Key.Delete <= keyCode && !isSystemKey && !isFKey || keyCode == ASPx.Key.Backspace || keyCode == ASPx.Key.Space;
    },
    onKeyDown: function(spreadsheet, evt, editMode) {
        var inputController = spreadsheet.getInputController();
        var command = this.GetShortcutCommand(evt);
        if(command) {
            inputController.setEditableDocumentContent("");
            return command(spreadsheet, evt, editMode);
        }
        else if(editMode == ASPxClientSpreadsheet.StateController.Modes.Ready) {
            var selection = spreadsheet.getStateController().getSelection();
            if(selection.activeCellExists()) {
                var typing = this.EventKeyCodeChangesTheInput(evt);
                if(typing) {
                    if(this.startTypingRequedKeyPressEvent(evt)) {
                        inputController.keyDownEventStartTyping();
                        return;
                    }
                    spreadsheet.getStateController().setEditMode(ASPxClientSpreadsheet.StateController.Modes.Enter);
                }
            }
        }
    },
    //only for WebKit family
    onKeyPress: function(spreadsheet, evt, editMode) {
        var inputController = spreadsheet.getInputController();
        if(inputController.typingInProgress()) {
            var keyCode = ASPx.Evt.GetKeyCode(evt);
            inputController.unMarkKeyDownEvent();
            inputController.setMissingKeyCode(keyCode);
            spreadsheet.getStateController().setEditMode(ASPxClientSpreadsheet.StateController.Modes.Enter);
        }
    },
    startTypingRequedKeyPressEvent: function(evt) {        
        var keyCode = ASPx.Evt.GetKeyCode(evt);
        return ASPx.Browser.WebKitFamily && ASPx.Key.Backspace != keyCode;
    }
});
SpreadsheetKeyboardManager.selection_move_core = function(spreadsheet, method, evt) {
    spreadsheet.changeSelectionAccordingToKeyboardAction(method);
    return ASPx.Evt.PreventEvent(evt);
};

SpreadsheetKeyboardManager.wrapServerCommand = function(command) {
    return function(spreadsheet, evt, editMode) {
        command(spreadsheet);
        return ASPx.Evt.PreventEvent(evt);
    }
};

SpreadsheetKeyboardManager.CommandsOnServer = {
    FormatFontBold: function(spreadsheet, evt, editMode) {
        ASPxClientSpreadsheet.ServerCommands.FormatFontBold(spreadsheet);
        return ASPx.Evt.PreventEvent(evt);
    },
    FormatFontItalic: function(spreadsheet, evt, editMode) {
        ASPxClientSpreadsheet.ServerCommands.FormatFontItalic(spreadsheet);
        return ASPx.Evt.PreventEvent(evt);
    },
    FormatFontUnderline: function(spreadsheet, evt, editMode) {
        ASPxClientSpreadsheet.ServerCommands.FormatFontUnderline(spreadsheet);
        return ASPx.Evt.PreventEvent(evt);
    },

    InsertSheet: function(spreadsheet, evt, editMode) {
        ASPxClientSpreadsheet.ServerCommands.InsertSheet(spreadsheet);
        return ASPx.Evt.PreventEvent(evt);
    },

    EditingFillDown: function(spreadsheet, evt, editMode) {
        ASPxClientSpreadsheet.ServerCommands.EditingFillDown(spreadsheet);
        return ASPx.Evt.PreventEvent(evt);
    },

    EditingFillRight: function(spreadsheet, evt, editMode) {
        ASPxClientSpreadsheet.ServerCommands.EditingFillRight(spreadsheet);
        return ASPx.Evt.PreventEvent(evt);
    }
};

SpreadsheetKeyboardManager.CommandsOnClient = {
    DOWN_COMMAND: function(spreadsheet, evt, editMode) {
        if(!ASPxClientSpreadsheet.StateController.IsEditingMode(editMode) && !spreadsheet.isFunctionsListBoxDisplayed())
            return SpreadsheetKeyboardManager.selection_move_core(spreadsheet, "moveDown", evt);
    },
    UP_COMMAND: function(spreadsheet, evt, editMode) {
        if(!ASPxClientSpreadsheet.StateController.IsEditingMode(editMode) && !spreadsheet.isFunctionsListBoxDisplayed())
            return SpreadsheetKeyboardManager.selection_move_core(spreadsheet, "moveUp", evt);
    },
    RIGHT_COMMAND: function(spreadsheet, evt, editMode) {
        if(!ASPxClientSpreadsheet.StateController.IsEditingMode(editMode))
            return SpreadsheetKeyboardManager.selection_move_core(spreadsheet, "moveRight", evt);
    },
    LEFT_COMMAND: function(spreadsheet, evt, editMode) {
        if(!ASPxClientSpreadsheet.StateController.IsEditingMode(editMode))
            return SpreadsheetKeyboardManager.selection_move_core(spreadsheet, "moveLeft", evt);
    },
    ENTER_COMMAND: function(spreadsheet, evt, editMode) {
        var retValue = SpreadsheetKeyboardManager.selection_move_core(spreadsheet, "moveActiveCellDown", evt);
        if(editMode != ASPxClientSpreadsheet.StateController.Modes.Ready)
            spreadsheet.getStateController().applyCurrentEdition();
        return retValue;
    },
    SHIFT_ENTER_COMMAND: function(spreadsheet, evt, editMode) {
        var retValue = SpreadsheetKeyboardManager.selection_move_core(spreadsheet, "moveActiveCellUp", evt);
        if(editMode != ASPxClientSpreadsheet.StateController.Modes.Ready)
            spreadsheet.getStateController().applyCurrentEdition();
        return retValue;
    },
    ALT_ENTER_COMMAND: function(spreadsheet, evt, editMode) {
        if(ASPxClientSpreadsheet.StateController.IsEditingMode(editMode) || editMode === ASPxClientSpreadsheet.StateController.Modes.Enter) {
            spreadsheet.getEditingHelper().onAltEnterEvent();
            return ASPx.Evt.PreventEventAndBubble(evt);
        }
    },

    TAB_COMMAND: function(spreadsheet, evt, editMode) {
		//TODO to think: var navigationAllowed = !spreadsheet.isFunctionsListBoxDisplayed();
        if(spreadsheet.isFunctionsListBoxDisplayed())
            return ASPx.Evt.PreventEvent(evt);

        var retValue = SpreadsheetKeyboardManager.selection_move_core(spreadsheet, "moveActiveCellRight", evt);
        if(editMode != ASPxClientSpreadsheet.StateController.Modes.Ready)
            spreadsheet.getStateController().applyCurrentEdition();
        return retValue;
    },
    SHIFT_TAB_COMMAND: function(spreadsheet, evt, editMode) {
        var retValue = SpreadsheetKeyboardManager.selection_move_core(spreadsheet, "moveActiveCellLeft", evt);
        if(editMode != ASPxClientSpreadsheet.StateController.Modes.Ready)
            spreadsheet.getStateController().applyCurrentEdition();
        return retValue;
    },

    SHIFT_LEFT_COMMAND: function(spreadsheet, evt, editMode) {
        if(ASPxClientSpreadsheet.StateController.IsEditingMode(editMode)) return;

        var retValue = SpreadsheetKeyboardManager.selection_move_core(spreadsheet, "expandLeft", evt);
        if(editMode != ASPxClientSpreadsheet.StateController.Modes.Ready)
            spreadsheet.getStateController().applyCurrentEdition();
        return retValue;
    },
    SHIFT_RIGHT_COMMAND: function(spreadsheet, evt, editMode) {
        if(ASPxClientSpreadsheet.StateController.IsEditingMode(editMode)) return;

        var retValue = SpreadsheetKeyboardManager.selection_move_core(spreadsheet, "expandRight", evt);
        if(editMode != ASPxClientSpreadsheet.StateController.Modes.Ready)
            spreadsheet.getStateController().applyCurrentEdition();
        return retValue;
    },
    SHIFT_UP_COMMAND: function(spreadsheet, evt, editMode) {
        if(ASPxClientSpreadsheet.StateController.IsEditingMode(editMode)) return;

        var retValue = SpreadsheetKeyboardManager.selection_move_core(spreadsheet, "expandUp", evt);
        if(editMode != ASPxClientSpreadsheet.StateController.Modes.Ready)
            spreadsheet.getStateController().applyCurrentEdition();
        return retValue;
    },
    SHIFT_DOWN_COMMAND: function(spreadsheet, evt, editMode) {
        if(ASPxClientSpreadsheet.StateController.IsEditingMode(editMode)) return;

        var retValue = SpreadsheetKeyboardManager.selection_move_core(spreadsheet, "expandDown", evt);
        if(editMode != ASPxClientSpreadsheet.StateController.Modes.Ready)
            spreadsheet.getStateController().applyCurrentEdition();
        return retValue;
    },

    // TODO make it a real edit mode toggle
    F2_COMMAND: function(spreadsheet, evt, editMode) {
        if(!ASPxClientSpreadsheet.StateController.IsEditingMode(editMode) && spreadsheet.CanEditDocument())
            spreadsheet.getStateController().setEditMode(ASPxClientSpreadsheet.StateController.Modes.Edit);
    },
    ESCAPE_COMMAND: function(spreadsheet, evt, editMode) {
        spreadsheet.getStateController().onEscape();
    },
    SELECT_ALL_COMMAND: function(spreadsheet, evt, editMode) {
        spreadsheet.getStateController().selectAll();
        return ASPx.Evt.PreventEvent(evt);
    },
    DELETE_COMMAND: function(spreadsheet, evt, editMode) {
        if((editMode != ASPxClientSpreadsheet.StateController.Modes.Ready) || !spreadsheet.CanEditDocument()) return;

        var selection = spreadsheet.getStateController().getSelection();
        if(selection.isDrawingBoxSelection())
            ASPxClientSpreadsheet.ServerCommands.FormatClearAll(spreadsheet);
        else
            ASPxClientSpreadsheet.ServerCommands.FormatClearContents(spreadsheet);
    },
    CTRL_C_COMMAND: function(spreadsheet, evt, editMode) {
        if(editMode == ASPxClientSpreadsheet.StateController.Modes.Ready)
            spreadsheet.getInputController().onCopyEventProcessing(1);
    },
    CTRL_V_COMMAND: function(spreadsheet, evt, editMode) {
        if((editMode == ASPxClientSpreadsheet.StateController.Modes.Ready) && spreadsheet.CanEditDocument()) {
            var inputController = spreadsheet.getInputController();
            if(inputController)
                setTimeout(function () { inputController.onPasteEventProcessing(); }, ASPx.Browser.Safari ? 10 : 0);
        }        
    },
    CTRL_X_COMMAND: function(spreadsheet, evt, editMode) {
        if((editMode == ASPxClientSpreadsheet.StateController.Modes.Ready) && spreadsheet.CanEditDocument())
            spreadsheet.getInputController().onCopyEventProcessing(0);
    },
    FILE_UNDO: function(spreadsheet, evt, editMode) {
        if(spreadsheet.getRibbonManager().history.canUndo
            && !ASPxClientSpreadsheet.StateController.IsEditingMode(editMode)
            && editMode !== ASPxClientSpreadsheet.StateController.Modes.Enter)
            ASPxClientSpreadsheet.ServerCommands.FileUndo(spreadsheet);
    },
    FILE_REDO: function(spreadsheet, evt, editMode) {
        if(spreadsheet.getRibbonManager().history.canRedo)
            ASPxClientSpreadsheet.ServerCommands.FileRedo(spreadsheet);
    },
    // TODO Keys.A, Keys.Control, SpreadsheetCommandId.SelectAll
    // TODO Keys.Space, Keys.Control | Keys.Shift, SpreadsheetCommandId.SelectAll

    // TODO Keys.PageDown, Keys.None, SpreadsheetCommandId.SelectionMovePageDown
    // TODO Keys.PageUp, Keys.None, SpreadsheetCommandId.SelectionMovePageUp
    // TODO Keys.PageDown, Keys.Shift, SpreadsheetCommandId.SelectionExpandPageDown
    // TODO Keys.PageUp, Keys.Shift, SpreadsheetCommandId.SelectionExpandPageUp

    // NON Spreadsheet's specific shortcuts
    CTRL_F_COMMAND: function(spreadsheet, evt) {
        var findAllCommandId = ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FindAll").id;
        spreadsheet.onShortCutCommand(findAllCommandId);
        return ASPx.Evt.PreventEvent(evt);
    },
    CTRL_P_COMMAND: function(spreadsheet, evt) {
        spreadsheet.onShortCutCommand(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("Print").id);
        return ASPx.Evt.PreventEvent(evt);
    },
    F11_COMMAND: function(spreadsheet, evt) {
        spreadsheet.onShortCutCommand(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FullScreen").id);
        return ASPx.Evt.PreventEvent(evt);
    },
    CONTEXTMENU_COMMAND: function(spreadsheet, evt, editMode) {
        if(editMode !== ASPxClientSpreadsheet.StateController.Modes.Ready)
            return;

        spreadsheet.onKBContextMenu(evt);

        return ASPx.Evt.PreventEvent(evt);
    }//,
};

SpreadsheetKeyboardManager.Shortcuts = [
    [SpreadsheetKeyboardManager.CommandsOnClient.DOWN_COMMAND, "DOWN"],
    [SpreadsheetKeyboardManager.CommandsOnClient.UP_COMMAND, "UP"],
    [SpreadsheetKeyboardManager.CommandsOnClient.RIGHT_COMMAND, "RIGHT"],
    [SpreadsheetKeyboardManager.CommandsOnClient.LEFT_COMMAND, "LEFT"],

    [SpreadsheetKeyboardManager.CommandsOnClient.ENTER_COMMAND, "ENTER"],
    [SpreadsheetKeyboardManager.CommandsOnClient.SHIFT_ENTER_COMMAND, "SHIFT+ENTER"],
    [SpreadsheetKeyboardManager.CommandsOnClient.ALT_ENTER_COMMAND, "ALT+ENTER"],

    [SpreadsheetKeyboardManager.CommandsOnClient.TAB_COMMAND, "TAB"],
    [SpreadsheetKeyboardManager.CommandsOnClient.SHIFT_TAB_COMMAND, "SHIFT+TAB"],

    [SpreadsheetKeyboardManager.CommandsOnClient.SHIFT_LEFT_COMMAND, "SHIFT+LEFT"],
    [SpreadsheetKeyboardManager.CommandsOnClient.SHIFT_RIGHT_COMMAND, "SHIFT+RIGHT"],
    [SpreadsheetKeyboardManager.CommandsOnClient.SHIFT_UP_COMMAND, "SHIFT+UP"],
    [SpreadsheetKeyboardManager.CommandsOnClient.SHIFT_DOWN_COMMAND, "SHIFT+DOWN"],

    [SpreadsheetKeyboardManager.CommandsOnClient.F2_COMMAND, "F2"],
    [SpreadsheetKeyboardManager.CommandsOnClient.ESCAPE_COMMAND, "ESCAPE"],

    [SpreadsheetKeyboardManager.CommandsOnClient.SELECT_ALL_COMMAND, "CTRL+A"],
    [SpreadsheetKeyboardManager.CommandsOnClient.FILE_UNDO, "CTRL+Z"],
    [SpreadsheetKeyboardManager.CommandsOnClient.FILE_UNDO, "ALT+BACK"],
    [SpreadsheetKeyboardManager.CommandsOnClient.FILE_REDO, "CTRL+Y"],
    [SpreadsheetKeyboardManager.CommandsOnClient.FILE_REDO, "ALT+SHIFT+BACK"],

    [SpreadsheetKeyboardManager.CommandsOnClient.CONTEXTMENU_COMMAND, "CONTEXT"],

// TODO File operations - think out it
// Keys.N, Keys.Control, SpreadsheetCommandId.FileNew
// Keys.O, Keys.Control, SpreadsheetCommandId.FileOpen
// Keys.S, Keys.Control, SpreadsheetCommandId.FileSave
// Keys.F12, Keys.None, SpreadsheetCommandId.FileSaveAs
// Keys.P, Keys.Control, SpreadsheetCommandId.FilePrint

    //[SpreadsheetKeyboardManager.wrapServerCommand(ASPxClientSpreadsheet.ServerCommands.FileUndo), "CTRL+Z"],
   //[ASPxClientSpreadsheet.ServerCommands.FileUndo, "CTRL+Z"],
    //[ASPxClientSpreadsheet.ServerCommands.FileRedo, "CTRL+Y"],
    //[ASPxClientSpreadsheet.ServerCommands.FileUndo, "ALT+BACK"],
    //[ASPxClientSpreadsheet.ServerCommands.FileRedo, "ALT+SHIFT+BACK"],


    [SpreadsheetKeyboardManager.wrapServerCommand(ASPxClientSpreadsheet.ServerCommands.FormatFontBold), "CTRL+B"],
    [ASPxClientSpreadsheet.ServerCommands.FormatFontBold, "CTRL+2"],
    [SpreadsheetKeyboardManager.wrapServerCommand(ASPxClientSpreadsheet.ServerCommands.FormatFontItalic), "CTRL+I"],
    [ASPxClientSpreadsheet.ServerCommands.FormatFontItalic, "CTRL+3"],
    [SpreadsheetKeyboardManager.wrapServerCommand(ASPxClientSpreadsheet.ServerCommands.FormatFontUnderline), "CTRL+U"],
    [ASPxClientSpreadsheet.ServerCommands.FormatFontUnderline, "CTRL+4"],
    [ASPxClientSpreadsheet.ServerCommands.FormatFontStrikeout, "CTRL+5"],

    [ASPxClientSpreadsheet.ServerCommands.HideRows, "CTRL+9"],
    [ASPxClientSpreadsheet.ServerCommands.HideColumns, "CTRL+0"],
    [ASPxClientSpreadsheet.ServerCommands.UnhideRows, "CTRL+SHIFT+9"],
    [ASPxClientSpreadsheet.ServerCommands.UnhideColumns, "CTRL+SHIFT+0"],
// TODO Keys.D9, Keys.Control, SpreadsheetCommandId.HideRows
// TODO Keys.D0, Keys.Control, SpreadsheetCommandId.HideColumns
// TODO Keys.D9, Keys.Control | Keys.Shift, SpreadsheetCommandId.UnhideRows
// TODO Keys.D0, Keys.Control | Keys.Shift, SpreadsheetCommandId.UnhideColumns

    [ASPxClientSpreadsheet.ServerCommands.FormatNumberPredefined4, "CTRL+SHIFT+1"],
    [ASPxClientSpreadsheet.ServerCommands.FormatNumberPredefined18, "CTRL+SHIFT+2"],
    [ASPxClientSpreadsheet.ServerCommands.FormatNumberPredefined15, "CTRL+SHIFT+3"],
    [ASPxClientSpreadsheet.ServerCommands.FormatNumberPredefined8, "CTRL+SHIFT+4"],
    [ASPxClientSpreadsheet.ServerCommands.FormatNumberPercent, "CTRL+SHIFT+5"],
    [ASPxClientSpreadsheet.ServerCommands.FormatNumberScientific, "CTRL+SHIFT+6"],
    [ASPxClientSpreadsheet.ServerCommands.FormatOutsideBorders, "CTRL+SHIFT+7"],

    [SpreadsheetKeyboardManager.CommandsOnClient.DELETE_COMMAND, "DELETE"],

    [SpreadsheetKeyboardManager.wrapServerCommand(ASPxClientSpreadsheet.ServerCommands.InsertSheet), "SHIFT+F11"],// TODO Keys.F11, Keys.Shift, SpreadsheetCommandId.InsertSheet
    [SpreadsheetKeyboardManager.wrapServerCommand(ASPxClientSpreadsheet.ServerCommands.EditingFillDown), "CTRL+D"],
    [SpreadsheetKeyboardManager.wrapServerCommand(ASPxClientSpreadsheet.ServerCommands.EditingFillRight), "CTRL+R"],


// TODO IN SECOND VERSION Keys.Left, Keys.Control, SpreadsheetCommandId.SelectionMoveLeftToDataEdge
// TODO IN SECOND VERSION Keys.Right, Keys.Control, SpreadsheetCommandId.SelectionMoveRightToDataEdge
// TODO IN SECOND VERSION Keys.Up, Keys.Control, SpreadsheetCommandId.SelectionMoveUpToDataEdge
// TODO IN SECOND VERSION Keys.Down, Keys.Control, SpreadsheetCommandId.SelectionMoveDownToDataEdge
// TODO IN SECOND VERSION Keys.Home, Keys.None, SpreadsheetCommandId.SelectionMoveToLeftColumn
// TODO IN SECOND VERSION Keys.Home, Keys.Control, SpreadsheetCommandId.SelectionMoveToTopLeftCell
// TODO IN SECOND VERSION Keys.End, Keys.Control, SpreadsheetCommandId.SelectionMoveToLastUsedCell
// TODO IN SECOND VERSION Keys.Tab, Keys.None, SpreadsheetCommandId.SelectionMoveActiveCellRight
// TODO IN SECOND VERSION Keys.Tab, Keys.Shift, SpreadsheetCommandId.SelectionMoveActiveCellLeft
// TODO IN SECOND VERSION Keys.OemPeriod, Keys.Control, SpreadsheetCommandId.SelectionMoveActiveCellToNextCorner
// TODO IN SECOND VERSION Keys.Left, Keys.Control | Keys.Alt, SpreadsheetCommandId.SelectionPreviousRange
// TODO IN SECOND VERSION Keys.Right, Keys.Control | Keys.Alt, SpreadsheetCommandId.SelectionNextRange
// TODO IN SECOND VERSION Keys.Back, Keys.Shift, SpreadsheetCommandId.SelectActiveCell
// TODO IN SECOND VERSION Keys.Space, Keys.Control, SpreadsheetCommandId.SelectActiveColumn
// TODO IN SECOND VERSION Keys.Space, Keys.Shift, SpreadsheetCommandId.SelectActiveRow
// TODO IN SECOND VERSION Keys.Left, Keys.Control | Keys.Shift, SpreadsheetCommandId.SelectionExpandLeftToDataEdge
// TODO IN SECOND VERSION Keys.Right, Keys.Control | Keys.Shift, SpreadsheetCommandId.SelectionExpandRightToDataEdge
// TODO IN SECOND VERSION Keys.Up, Keys.Control | Keys.Shift, SpreadsheetCommandId.SelectionExpandUpToDataEdge
// TODO IN SECOND VERSION Keys.Down, Keys.Control | Keys.Shift, SpreadsheetCommandId.SelectionExpandDownToDataEdge
// TODO IN SECOND VERSION Keys.Home, Keys.Shift, SpreadsheetCommandId.SelectionExpandToLeftColumn
// TODO IN SECOND VERSION Keys.Home, Keys.Control | Keys.Shift, SpreadsheetCommandId.SelectionExpandToTopLeftCell
// TODO IN SECOND VERSION Keys.End, Keys.Control | Keys.Shift, SpreadsheetCommandId.SelectionExpandToLastUsedCell

    [SpreadsheetKeyboardManager.CommandsOnClient.CTRL_C_COMMAND, "CTRL+C"],
    [SpreadsheetKeyboardManager.CommandsOnClient.CTRL_C_COMMAND, "CTRL+INSERT"],
    [SpreadsheetKeyboardManager.CommandsOnClient.CTRL_V_COMMAND, "CTRL+V"],
    [SpreadsheetKeyboardManager.CommandsOnClient.CTRL_V_COMMAND, "SHIFT+INSERT"],
    [SpreadsheetKeyboardManager.CommandsOnClient.CTRL_X_COMMAND, "CTRL+X"],
    [SpreadsheetKeyboardManager.CommandsOnClient.CTRL_X_COMMAND, "SHIFT+DELETE"],
    // TODO Keys.V, Keys.Control | Keys.Alt, SpreadsheetCommandId.ShowPasteSpecialForm

// Keys.Escape, Keys.None, SpreadsheetCommandId.ClearCopiedRange

// TODO Keys.Control, Keys.K, SpreadsheetCommandId.InsertHyperlinkContextMenuItem
// TODO Keys.Control, Keys.Shift | Keys.U, SpreadsheetCommandId.CollapseOrExpandFormulaBar
// TODO Keys.PageDown, Keys.Control, SpreadsheetCommandId.MoveToNextSheet 
// TODO Keys.PageUp, Keys.Control, SpreadsheetCommandId.MoveToPreviousSheet
// TODO Keys.Control, Keys.Shift | Keys.Enter, SpreadsheetCommandId.InsertArrayFormula
// TODO Keys.Oemtilde, Keys.Control, SpreadsheetCommandId.ViewShowFormulas
// TODO Keys.Delete, Keys.None, SpreadsheetCommandId.FormatClearAll
// TODO Keys.Back, Keys.None, SpreadsheetCommandId.FormatClearAll

// TODO IN SECOND VERSION Keys.Left, Keys.Alt, SpreadsheetCommandId.ShapeRotateLeft
// TODO IN SECOND VERSION Keys.Left, Keys.Alt | Keys.Control, SpreadsheetCommandId.ShapeRotateLeftByDegree
// TODO IN SECOND VERSION Keys.Right, Keys.Alt, SpreadsheetCommandId.ShapeRotateRight
// TODO IN SECOND VERSION Keys.Right, Keys.Alt | Keys.Control, SpreadsheetCommandId.ShapeRotateRightByDegree
// TODO IN SECOND VERSION Keys.Right, Keys.Shift, SpreadsheetCommandId.ShapeEnlargeWidth
// TODO IN SECOND VERSION Keys.Up, Keys.Shift, SpreadsheetCommandId.ShapeEnlargeHeight
// TODO IN SECOND VERSION Keys.Right, Keys.Shift | Keys.Control, SpreadsheetCommandId.ShapeBitEnlargeWidth
// TODO IN SECOND VERSION Keys.Up, Keys.Shift | Keys.Control, SpreadsheetCommandId.ShapeBitEnlargeHeight
// TODO IN SECOND VERSION Keys.Left, Keys.Shift, SpreadsheetCommandId.ShapeReduceWidth
// TODO IN SECOND VERSION Keys.Down, Keys.Shift, SpreadsheetCommandId.ShapeReduceHeight
// TODO IN SECOND VERSION Keys.Left, Keys.Shift | Keys.Control, SpreadsheetCommandId.ShapeBitReduceWidth
// TODO IN SECOND VERSION Keys.Down, Keys.Shift | Keys.Control, SpreadsheetCommandId.ShapeBitReduceHeight
// TODO IN SECOND VERSION Keys.A, Keys.Control, SpreadsheetCommandId.ShapeSelectAll
// TODO IN SECOND VERSION Keys.Tab, Keys.None, SpreadsheetCommandId.ShapeSelectNext
// TODO IN SECOND VERSION Keys.Tab, Keys.Shift, SpreadsheetCommandId.ShapeSelectPrevious

// TODO Keys.Enter, Keys.Control | Keys.Shift, SpreadsheetCommandId.InplaceEndEditEnterArrayFormula
// TODO Keys.Escape, Keys.None, SpreadsheetCommandId.InplaceCancelEdit
// TODO Keys.Enter, Keys.Control, SpreadsheetCommandId.InplaceEndEditEnterToMultipleCells

    // NON Spreadsheet's specific shortcuts
    [SpreadsheetKeyboardManager.CommandsOnClient.CTRL_F_COMMAND, "CTRL+F"],
    [SpreadsheetKeyboardManager.CommandsOnClient.CTRL_P_COMMAND, "CTRL+P"],
    [SpreadsheetKeyboardManager.CommandsOnClient.F11_COMMAND, "F11"]//,
];

ASPx.SpreadsheetKeyboardManager = SpreadsheetKeyboardManager;
})();