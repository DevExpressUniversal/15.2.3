// API

(function() {
MVCxClientDashboardViewer = ASPx.CreateClass(ASPxClientDashboardViewer, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);
        this.callbackUrl = "";
        this.exportUrl = "";
    },
    Initialize: function () {
        var self = this;
        if(self.callbackUrl != "") {
            self.callBack = function (arg) {
                MVCx.PerformControlCallback(self.name, self.callbackUrl, arg, null, null)
            };
        }
        ASPxClientDashboardViewer.prototype.Initialize.call(self);
    },
    CreateCallbackByInfo: function (arg, command, callbackInfo) {
        this.CreateCallbackInternal(arg, command, true, callbackInfo);
    },
    SendPostBack: function (postBackArg) {
        var form = ASPx.GetParentByTagName(this.GetMainElement(), "form");
        if (form) {
            var hiddenField = ASPx.CreateHiddenField("DXMVCDashboardExportArgument"),
                sourceFormAction = form.action,
                sourceFormMethod = form.method;
            hiddenField.value = postBackArg;
            form.action = this.exportUrl;
            form.method = "post";
            form.appendChild(hiddenField);
            form.submit();
            form.removeChild(hiddenField);
            form.action = sourceFormAction;
            form.method = sourceFormMethod;
        }
    }
});

window.MVCxClientDashboardViewer = MVCxClientDashboardViewer;
})();