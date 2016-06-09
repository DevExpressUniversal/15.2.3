(function() {
    var constants = {
        CallbackPrefix: "REFM"
    };

    var ASPxClientRichEditFolderManager = ASPx.CreateClass(ASPxClientFileManager, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.callbackCount = 0;
        },
        InitializeKbdHelper: function() {

        },
        CreateCallback: function(callbackString) {
            this.richedit.sendInternalServiceCallback(constants.CallbackPrefix, callbackString, this);
            this.callbackCount++;
        },
        OnEndCallback: function() {
            if(this.callbackCount > 0)
                this.callbackCount--;
            if(this.callbackCount <= 0)
                this.richedit.clearOwnerControlCallback();
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

    ASPxClientRichEditFolderManager.SetOwner = function(container, owner) {
        var fileManager = null;
        ASPx.GetControlCollection().ProcessControlsInContainer(
            container,
            function(control) {
                if(control instanceof ASPxClientRichEditFolderManager)
                    fileManager = control;
            }
        );
        if(fileManager)
            fileManager.richedit = owner;
    };

    window.ASPxClientRichEditFolderManager = ASPxClientRichEditFolderManager;
})();