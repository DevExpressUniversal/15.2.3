(function() {
    ASPx.HtmlEditorClasses.Commands.Browser.InsertImage = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.Command, {
        // cmdValue = { url, width, height, align, alt };
        Execute: function(cmdValue, wrapper) {
            if(wrapper.tagIsFiltered("img"))
                return false;
            var newImageElement = this.InsertImage(cmdValue.src, wrapper);
            ASPx.HtmlEditorClasses.Commands.ChangeImage.SetImageProperties(newImageElement, cmdValue);
            if(ASPx.Browser.IE)
                ASPx.HtmlEditorClasses.HtmlProcessor.restoreUrlsInDOM(wrapper.getDocument());
            return true;
        },
        InsertImage: function(source, wrapper) {
            var markerID = ASPx.HtmlEditorClasses.Selection.CreateUniqueID();
            var htmlText = '<img src="' + source + '" id="' + markerID + '" />';
            this.owner.getCommand(ASPxClientCommandConsts.PASTEHTML_COMMAND).Execute(htmlText, wrapper);
            var imageElement = ASPx.GetElementByIdInDocument(wrapper.getDocument(), markerID);
            ASPx.Attr.RemoveAttribute(imageElement, "id");
            return imageElement;
        }
    });
})();