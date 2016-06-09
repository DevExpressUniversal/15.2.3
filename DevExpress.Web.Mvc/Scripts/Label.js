(function() {
var MVCxClientLabel = ASPx.CreateClass(ASPxClientLabel, {
    Initialize: function() {
        if(this.associatedControlName){
            var associatedControl = ASPx.GetControlCollection().GetByName(this.associatedControlName);
            var inputElement = associatedControl && associatedControl.GetInputElement ? associatedControl.GetInputElement() : null;
            var inputName = inputElement ? inputElement.id : this.associatedControlName + "_I";
            $(this.GetMainElement()).attr("for", inputName);
        }

        ASPxClientLabel.prototype.Initialize.call(this);
    }
});

window.MVCxClientLabel = MVCxClientLabel;
})();