(function() {
var MVCxClientButton = ASPx.CreateClass(ASPxClientButton, {
    InlineInitialize: function() {
        ASPxClientButton.prototype.InlineInitialize.apply(this, arguments);
        this.autoPostBack = this.autoPostBack || !!this.submitUrl;
        MVCx.validateInvisibleEditors = this.validateInvisibleEditors;
    },
    SendPostBack: function(postBackArg) {
        if(!!this.submitUrl) {
            var form = this.GetParentForm();
            if(this.useSubmitBehavior && form) {
                form.action = this.submitUrl;
                form.submit();
            }
            else {
                location.href = this.submitUrl;
            }
        }
        else
            ASPxClientButton.prototype.SendPostBack.apply(this, arguments);
    }
});

window.MVCxClientButton = MVCxClientButton;
})();