(function() {
    var SpreadsheetFileManagerUploadControl = ASPx.CreateClass(ASPx.FileManagerUploadControl, {
        createUploadManager: function() {
            return new ASPxSpreadsheetFileManagerUploadManagerClass(this.options);
        }
    });

    var ASPxSpreadsheetFileManagerUploadManagerClass = ASPx.CreateClass(ASPxClientUploadControl.UploadManagerClass, {
        GetUploadFormAction: function(form) {
            var action = ASPxClientUploadControl.UploadManagerClass.prototype.GetUploadFormAction.call(this, form);
            return this.AddQueryParamToUrl(action, ASPx.SpreadsheetUploadControlUrlParametr, ASPxClientSpreadsheet.CallbackPrefixes.InternalCallBackPostfix);
        }
    });

    ASPx.SpreadsheetFileManagerUploadControl = SpreadsheetFileManagerUploadControl;
})();