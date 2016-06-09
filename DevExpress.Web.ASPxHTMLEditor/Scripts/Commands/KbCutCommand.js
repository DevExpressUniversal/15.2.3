(function() {
    ASPx.HtmlEditorClasses.Commands.KbCut = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.TextType, {
        Execute: function(cmdValue, wrapper) {
            if(wrapper.settings.placeholders && wrapper.settings.placeholders.length > 0 &&  wrapper.placeholderManager.getFildsCount() > 0) {
                setTimeout(function() {
                    wrapper.placeholderManager.updatePlaceholdersStyle();
                    wrapper.placeholderManager.setAllowEditFilds(false);
                }.aspxBind(this), 0);
            }
            setTimeout(function() {
                ASPx.HtmlEditorClasses.Commands.Browser.InsertList.UpdateListNumbering(wrapper.getBody());
            }, 0);
            return true;
        },
        canBeExecutedOnSelection: function(wrapper) {
            return true;
        }
    });
})();