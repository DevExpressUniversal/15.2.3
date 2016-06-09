(function() {
    var HtmlEditorFileManager = ASPx.CreateClass(ASPxClientFileManager, {
        InitializeKbdHelper: function() {
            this.kbdHelper = new HtmlEditorFileManagerKbdHelper(this);
            this.kbdHelper.Init();
            ASPx.KbdHelper.RegisterAccessKey(this);
        },
        CreateCallback: function (callbackString) {
            this.htmlEditor.sendCallbackViaQueue(ASPx.HtmlEditorClasses.FileManagerCallbackPrefix, callbackString, false, this);
        },
        OnEndCallback: function () {
            ASPxClientFileManager.prototype.DoEndCallback.apply(this, arguments);
        },
        RaiseEndCallback: function() {
            this.RaiseEndCallbackInternal();
        },
        setOwner: function (owner) {
            this.htmlEditor = owner;
        }
    });
    HtmlEditorFileManager.SetOwner = function(container, owner) {
        var fileManager = null;
        ASPx.GetControlCollection().ProcessControlsInContainer(
            container,
            function(control) {
                if (control instanceof HtmlEditorFileManager)
                    fileManager = control;
            }
        );
        if(fileManager)
            fileManager.htmlEditor = owner;
    };

    var HtmlEditorFileManagerKbdHelper = ASPx.CreateClass(ASPx.FileManagerKbdHelper, {
        ProcessEsc: function() {
            if(!this.control.IsEditMode() && this.control.browsePopup) {
                var _this = this;
                window.setTimeout(function() { _this.control.browsePopup.Hide(); }, 0);
            }
            else
                ASPx.FileManagerKbdHelper.prototype.ProcessEsc.apply(this);
        }
    });

    ASPx.HtmlEditorFileManager = HtmlEditorFileManager;
})();