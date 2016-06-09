

(function() {
    var queryParamName = "DXMVC";
    var MVCxClientUploadControl = ASPx.CreateClass(ASPxClientUploadControl, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.callbackUrl = "";
        },
        InlineInitialize: function() {
            var element = this.GetMainElement();
            if(element)
                element.appendChild(ASPx.CreateHiddenField(this.name + "_DXMVCUploadControl"));
            ASPxClientUploadControl.prototype.InlineInitialize.call(this);
        },
        initializeOptions: function() {
            ASPxClientUploadControl.prototype.initializeOptions.call(this);
            this.options.callbackUrl = this.callbackUrl;
        },
        createUploadManager: function() {
            return new MVCxLegacyUploadManager(this.options);
        }
    });

    var MVCxLegacyUploadManager = ASPx.CreateClass(ASPxClientUploadControl.UploadManagerClass, {
        constructor: function(options) {
            this.constructor.prototype.constructor.call(this, options);
        },
        GetUploadFormAction: function(form) {
            if(this.options.callbackUrl != "")
                form.action = this.options.callbackUrl;

            var action = ASPxClientUploadControl.UploadManagerClass.prototype.GetUploadFormAction.call(this, form);
            return this.AddQueryParamToUrl(action, queryParamName, "true");
        }
    });
    MVCxClientUploadControl.Cast = ASPxClientControl.Cast;

    window.MVCxClientUploadControl = MVCxClientUploadControl;

})();