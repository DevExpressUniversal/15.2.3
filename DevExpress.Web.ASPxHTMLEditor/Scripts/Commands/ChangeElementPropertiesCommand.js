(function() {
    ASPx.HtmlEditorClasses.Commands.ChangeElementProperties = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        // cmdValue - element properties
        Execute: function (cmdValue, wrapper) {
            var selectedElement = cmdValue.selectedElement;
            if (selectedElement && cmdValue) {               
                if(cmdValue.styles) {
                    ASPx.SetStyles(selectedElement, cmdValue.styles);
                }
                var className = cmdValue.styles.className;
                if (!className || className.replace(/\s*/g, '') == "") {
                    ASPx.Attr.RemoveAttribute(selectedElement, "class");
                }
                if(ASPx.Attr.GetAttribute(selectedElement, "style") == "") {
                    ASPx.Attr.RemoveAttribute(selectedElement, "style");
                }
                if(cmdValue.attributes) {
                    for(var key in cmdValue.attributes) {
                        if (ASPx.IsExists(cmdValue.attributes[key])) {
                            var isSelfExplaining = (key == 'disabled' || key == 'checked' || key == 'readonly');
                            if (!cmdValue.attributes[key] && isSelfExplaining)
                                selectedElement.removeAttribute(key);
                            else 
                                ASPx.Attr.SetAttribute(selectedElement, key, cmdValue.attributes[key]);
                        }
                    }
                }
            }
            return true;
        }
    });
})();