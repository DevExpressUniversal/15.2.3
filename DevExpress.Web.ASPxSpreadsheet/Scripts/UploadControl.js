(function() {
    var SpreadsheetUploadControl = ASPx.CreateClass(ASPxClientUploadControl, {
        createUploadManager: function() {
            return new SpreadsheetUploadManagerClass(this.options);
        }
    });

    var SpreadsheetUploadManagerClass = ASPx.CreateClass(ASPxClientUploadControl.UploadManagerClass, {
        GetUploadFormAction: function(form) {
            var action = ASPxClientUploadControl.UploadManagerClass.prototype.GetUploadFormAction.call(this, form);
            return this.AddQueryParamToUrl(action, ASPx.SpreadsheetUploadControlUrlParametr, ASPxClientSpreadsheet.CallbackPrefixes.InternalCallBackPostfix);
        }
    });

    ASPx.SpreadsheetUploadControl = SpreadsheetUploadControl;
})();