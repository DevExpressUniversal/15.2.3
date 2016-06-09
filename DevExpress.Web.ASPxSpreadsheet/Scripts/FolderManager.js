(function() {
    var SpreadsheetFolderManager = ASPx.CreateClass(ASPxClientFileManager, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.callbackCount = 0;
        },
        InitializeKbdHelper: function() {

        },
        CreateCallback: function(callbackString) {
            this.spreadsheet.sendInternalServiceCallback(ASPxClientSpreadsheet.CallbackPrefixes.FileManagerCallbackPrefix, callbackString, this, true);
            this.callbackCount++;
        },
        OnEndCallback: function() {
            if(this.callbackCount > 0)
                this.callbackCount--;
            if(this.callbackCount <= 0)
                this.spreadsheet.clearOwnerControlCallback();
            ASPxClientFileManager.prototype.DoEndCallback.apply(this, arguments);
        },
        PrepareFileStates: function() {
            ASPxClientFileManager.prototype.PrepareFileStates();
            this.forEachFile(function(file) {
                var fileId = file.id;
                ASPx.GetStateController().RemoveSelectedItem(fileId);
            }.aspxBind(this));
        },
        RaiseEndCallback: function() {
            this.RaiseEndCallbackInternal();
        }
    });

    SpreadsheetFolderManager.SetOwner = function(container, owner) {
        var fileManager = null;
        ASPx.GetControlCollection().ProcessControlsInContainer(
            container,
            function(control) {
                if(control instanceof SpreadsheetFolderManager)
                    fileManager = control;
            }
        );
        if(fileManager)
            fileManager.spreadsheet = owner;
    };

    ASPx.SpreadsheetFolderManager = SpreadsheetFolderManager;
})();