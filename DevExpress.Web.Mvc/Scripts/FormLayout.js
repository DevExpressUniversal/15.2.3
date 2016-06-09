(function() {
var MVCxClientFormLayout = ASPx.CreateClass(ASPxClientFormLayout, {
    CreateRootItem: function(){
        return new MVCxClientLayoutItem(this, "", "", null);
    }
});

var MVCxClientLayoutItem = ASPx.CreateClass(ASPxClientLayoutItem, {
    CreateItemInstance: function (name, path) {
        return new MVCxClientLayoutItem(this.formLayout, name, path, this);
    },
    ConfigureByProperties: function(itemProperties){
        ASPxClientLayoutItem.prototype.ConfigureByProperties.call(this, itemProperties);
        if(ASPx.IsExists(itemProperties[6]))
            this.EnsureLabelAssociatedElementId(itemProperties[6]);
    },
    EnsureLabelAssociatedElementId: function(associatedControlName) {
        var labelElement = this.GetLabelElement();
        if(!labelElement) return;

        var associatedControl = ASPx.GetControlCollection().GetByName(associatedControlName);
        var inputElement = associatedControl && associatedControl.GetInputElement && associatedControl.GetInputElement() || ASPx.GetElementById(associatedControlName);
        if(inputElement)
            $(labelElement).attr("for", inputElement.id);
    },
    GetLabelElement: function(){
        var itemElement = this.formLayout.GetHTMLElementByItem(this);
        if (!itemElement)
            return;
        var captionCells = ASPx.GetNodesByClassName(itemElement, ASPx.FormLayoutConsts.CAPTION_CELL_SYSTEM_CLASS_NAME);
        if(!captionCells || captionCells.length == 0)
            return;

        return ASPx.GetNodeByTagName(captionCells[0], "label", 0);
    }
});

window.MVCxClientFormLayout = MVCxClientFormLayout;
window.MVCxClientLayoutItem = MVCxClientLayoutItem;
})();