(function() {
    var KeyboardManager = ASPx.CreateClass(null, {
        constructor: function (shortcuts, inactiveShortcuts) {
	        this.shortcutCommands = [ ];
            this.shortcutCmdID = null;

            var scs = this.getActiveShortcuts(inactiveShortcuts);
            for (var i = 0; i < scs.length; i++)
                    this.AddShortcut(scs[i][1], scs[i][0]);

            if (shortcuts) {
                for(var key in shortcuts) {
                    if(shortcuts.hasOwnProperty(key) && key != "None")
                        this.AddShortcut(key, shortcuts[key]);
                }
            }
        },
        AddShortcut: function(shortcutString, commandID) {
             var shortcutCode = ASPx.ParseShortcutString(shortcutString);
             this.shortcutCommands[shortcutCode] = commandID;
        },
        getShortcutCommand: function(evt) {
            var shortcutCode = ASPx.GetShortcutCode(evt.keyCode, evt.ctrlKey, evt.shiftKey, evt.altKey);
            this.shortcutCmdID = this.shortcutCommands[shortcutCode];
            return this.shortcutCmdID;
        },
        isBrowserShortcut: function(evt) {
            if(ASPx.Browser.WebKitFamily || ASPx.Browser.IE) {
                var shortcutCode = ASPx.GetShortcutCode(evt.keyCode, evt.ctrlKey, evt.shiftKey, evt.altKey);
                return ASPx.Data.ArrayIndexOf(KeyboardManager.BrowserShortcuts, shortcutCode) > -1;
            }
        },
        getKeyDownInfo: function(evt) { // call only on keydown
            this.keyDownInfo = {
                isSystemKey: KeyboardManager.IsSystemKey(evt.keyCode),
                isDeleteOrBackSpaceKey: KeyboardManager.IsDeleteOrBackSpaceKey(evt.keyCode),
                isBackSpaceKey: KeyboardManager.IsBackSpaceKey(evt.keyCode),
                isSpaceKey: KeyboardManager.IsSpaceKey(evt.keyCode),
                isCursorMovingKey: KeyboardManager.IsCursorMovingKey(evt.keyCode),
                isControlKey: KeyboardManager.IsControlKey(evt.keyCode)
            };
            return this.keyDownInfo;
        },
        getLastShortcutID: function() {
            return this.shortcutCmdID;
        },
        clearLastShortcut: function() {
            this.shortcutCmdID = null;
        },
        getLastKeyDownInfo: function() {
            return this.keyDownInfo;
        },
        clearKeyDownInfo: function() { // call only on keyup
            this.keyDownInfo = null;
        },
        getActiveShortcuts: function (inactiveShortcuts) {
            var res = KeyboardManager.Shortcuts;
            var res = this.getDefaultShortcuts();
            if (!inactiveShortcuts)
                return res;
            for (var i = 0; i < inactiveShortcuts.length; i++)
                ASPx.Data.ArrayRemove(res, inactiveShortcuts[i]);

            return res;
        },
        isSpacing: function() {
            return this.keyDownInfo && this.keyDownInfo.isSpaceKey;
        },
        getDefaultShortcuts: function () {
            return KeyboardManager.CommonShortcuts;
        }
    });

    var DesignViewKeyboardManager = ASPx.CreateClass(KeyboardManager, {
        getDefaultShortcuts: function () {
            var res = KeyboardManager.prototype.getDefaultShortcuts.call(this);
            return res.concat(KeyboardManager.DesignViewShortcuts);
        }
    });
    var HtmlViewCMKeyboardManager = ASPx.CreateClass(KeyboardManager, {
        getDefaultShortcuts: function () {
            var res = KeyboardManager.prototype.getDefaultShortcuts.call(this);
            return res.concat(KeyboardManager.HtmlViewCMShortcuts);
        }
    });

    KeyboardManager.FINDANDREPLACE_DIALOG_COMMAND = [ASPxClientCommandConsts.FINDANDREPLACE_DIALOG_COMMAND, "CTRL+H"];
    KeyboardManager.SHOWSEARCHPANEL_COMMAND = [ASPxClientCommandConsts.SHOWSEARCHPANEL_COMMAND, "CTRL+F"];
    KeyboardManager.DesignViewShortcuts = [
        KeyboardManager.SHOWSEARCHPANEL_COMMAND,
        KeyboardManager.FINDANDREPLACE_DIALOG_COMMAND,
        [ASPxClientCommandConsts.BOLD_COMMAND, "CTRL+B"],
        [ASPxClientCommandConsts.ITALIC_COMMAND, "CTRL+I"],
        [ASPxClientCommandConsts.UNDERLINE_COMMAND, "CTRL+U"],
    
        [ASPxClientCommandConsts.JUSTIFYLEFT_COMMAND, "CTRL+L"],
        [ASPxClientCommandConsts.JUSTIFYCENTER_COMMAND, "CTRL+E"],
        [ASPxClientCommandConsts.JUSTIFYRIGHT_COMMAND, "CTRL+R"],
        [ASPxClientCommandConsts.JUSTIFYFULL_COMMAND, "CTRL+J"],
    
        [ASPxClientCommandConsts.UNDO_COMMAND, "CTRL+Z"],
        [ASPxClientCommandConsts.REDO_COMMAND, "CTRL+Y"],
    
        [ASPxClientCommandConsts.INSERTLINK_DIALOG_COMMAND, "CTRL+K"],    
        [ASPxClientCommandConsts.INSERTIMAGE_DIALOG_COMMAND, "CTRL+G"],    
        [ASPxClientCommandConsts.UNLINK_COMMAND, "CTRL+SHIFT+K"],    
    
        [ASPxClientCommandConsts.PRINT_COMMAND, "CTRL+P"],
    
        [ASPxClientCommandConsts.FULLSCREEN_COMMAND, "F11"],

        [ASPxClientCommandConsts.NEWPARAGRAPHTYPE_COMMAND, "CTRL+ENTER"],
        [ASPxClientCommandConsts.LINEBREAKETYPE_COMMAND, "SHIFT+ENTER"],
        [ASPxClientCommandConsts.ENTER_COMMAND, "ENTER"],
        [ASPxClientCommandConsts.SELECT_ALL, "CTRL+A"],

        [ASPxClientCommandConsts.KBCUT_COMMAND, "CTRL+X"],
        [ASPxClientCommandConsts.KBCUT_COMMAND, "SHIFT+DELETE"],
        [ASPxClientCommandConsts.KBCOPY_COMMAND, "CTRL+C"],
        [ASPxClientCommandConsts.KBCOPY_COMMAND, "CTRL+INSERT"],
        [ASPxClientCommandConsts.KBPASTE_COMMAND, "CTRL+V"],
        [ASPxClientCommandConsts.KBPASTE_COMMAND, "SHIFT+INSERT"]
    ];
    KeyboardManager.HtmlViewCMShortcuts = [
        KeyboardManager.SHOWSEARCHPANEL_COMMAND,
        KeyboardManager.FINDANDREPLACE_DIALOG_COMMAND,
        [ASPxClientCommandConsts.UNDO_COMMAND, "CTRL+Z"],
        [ASPxClientCommandConsts.REDO_COMMAND, "CTRL+Y"],

        [ASPxClientCommandConsts.SHOWINTELLISENSE_COMMAND, "CTRL+SPACE"],

        [ASPxClientCommandConsts.SELECT_ALL, "CTRL+A"]
    ];
    KeyboardManager.BrowserShortcuts = [
        ASPx.ParseShortcutString("CTRL+B"),
        ASPx.ParseShortcutString("CTRL+I"),
        ASPx.ParseShortcutString("CTRL+U"),
        ASPx.ParseShortcutString("CTRL+Z"),
        ASPx.ParseShortcutString("CTRL+Y")
    ];
    KeyboardManager.CommonShortcuts = [
        [ASPxClientCommandConsts.FULLSCREEN_COMMAND, "F11"]
    ];
    KeyboardManager.IsSystemKey = function(keyCode) {
        return keyCode == 0 ||
            keyCode >= ASPx.Key.F1 && keyCode <= ASPx.Key.F12 ||
            keyCode >= ASPx.Key.Backspace && keyCode <= ASPx.Key.Esc ||
            keyCode >= ASPx.Key.Space && keyCode <= ASPx.Key.Delete ||
            keyCode == ASPx.Key.ContextMenu;
    };
    KeyboardManager.IsDeleteOrBackSpaceKey = function(keyCode) {
        return keyCode == ASPx.Key.Delete || keyCode == ASPx.Key.Backspace;
    };
    KeyboardManager.IsBackSpaceKey = function(keyCode) {
        return keyCode == ASPx.Key.Backspace;
    };
    KeyboardManager.IsSpaceKey = function(keyCode) {
        return keyCode == ASPx.Key.Space;
    };
    KeyboardManager.IsCursorMovingKey = function(keyCode) {
        return keyCode >= ASPx.Key.PageUp && keyCode <= ASPx.Key.Down;
    };
    KeyboardManager.IsControlKey = function(keyCode) {
        return keyCode == ASPx.Key.Ctrl;
    };
    ASPx.HtmlEditorClasses.Managers.KeyboardManager = KeyboardManager;
    ASPx.HtmlEditorClasses.Managers.DesignViewKeyboardManager = DesignViewKeyboardManager;
    ASPx.HtmlEditorClasses.Managers.HtmlViewCMKeyboardManager = HtmlViewCMKeyboardManager;
})();