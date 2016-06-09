/// <reference path="_references.js"/>
(function() {
var ControlResizeManager = {
    GetControlCollection: function() {
        if(!ASPx.IsExists(this.controls)) {
            this.controls = [];

            ASPx.Evt.AttachEventToElement(window, "resize", function() { ControlResizeManager.OnWindowResizing(); });
            this.resizeTimerId = window.setTimeout(this.OnTimeOut, 1000);
         }
        return this.controls;
    },
    OnTimeOut: function() {
        ControlResizeManager.CheckResize();
    },
    Add: function(control) {
        if (!ASPx.IsExists(control.GetCurrentSize))
            return;        
        control.lastCheckedSize = control.GetCurrentSize();        
        this.GetControlCollection().push(control);
    },    
    SafeAdd: function (control) {
        if (!ASPx.IsExists(control.GetCurrentSize))
            return;
        this.RemoveElementControls(this.GetMainElement(control))
        control.lastCheckedSize = control.GetCurrentSize();
        this.GetControlCollection().push(control);
    },
    RemoveElementControls: function (element) {
        var collection = this.GetControlCollection();
        var count = collection.length;
        for (var i = count - 1; i >= 0; i--) {
            var control = collection[i];
            if (this.GetMainElement(control) == element) {
                this.Remove(control);
                continue;
            }
        }
    },
    GetMainElement: function (control) {
        if (ASPx.IsExists(control.GetMainElement))
            return control.GetMainElement();
        return null;
    },
    Remove: function (control) {
        ASPx.Data.ArrayRemove(this.GetControlCollection(), control);
    },
    Clear: function() {
        this.controls = null;
        window.clearTimeout(this.resizeTimerId);
    },    
    CheckResize: function() {
        window.clearTimeout(this.resizeTimerId);
        var collection = this.GetControlCollection();
        var count = collection.length;
        var hasLiveElements = false;
        for(var i = count - 1; i >= 0; i--) {
            var control = collection[i];
            var size = control.GetCurrentSize();
            if(size == null || (control.GetMainElement && !control.GetMainElement())) {
                this.Remove(control);
                continue;
            }
            var lastCheckedSize = control.lastCheckedSize;
            if(lastCheckedSize[0] != size[0]) {
                control.lastCheckedSize = size;
                control.OnWindowResized();
            }
            control.OnSizeChecked();
            hasLiveElements = true;
        }
        if(hasLiveElements)
            this.resizeTimerId = window.setTimeout(this.OnTimeOut, 1000);
        else
            this.Clear();
    },
    OnWindowResizing: function() {
        window.clearTimeout(this.resizeTimerId);
        this.resizeTimerId = window.setTimeout(this.OnTimeOut, 1000);
    }
};

ASPx.ControlResizeManager = ControlResizeManager;
})();