(function() {
    ASPx.HtmlEditorClasses.Commands.PasteHtml = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        Execute: function(htmlText, wrapper) {
            ASPx.HtmlEditorClasses.Commands.Command.prototype.Execute.apply(this, arguments);
            if(ASPx.Browser.IE) {
                var obj = ASPx.HtmlEditorClasses.Commands.PasteHtml.safePreLineBreaks(htmlText); // B218369
                htmlText = obj.html.replace(/(\r\n|\n|\r)/gm, " "); // Q348173, B207558
                htmlText = htmlText.replace(/>[\s]*</g, "><");
                htmlText = ASPx.HtmlEditorClasses.Commands.PasteHtml.restorePreLineBreaks(htmlText, obj.safePreHtmlElements);
            }
            htmlText = ASPx.HtmlEditorClasses.HtmlProcessor.replaceElementsToSpecialImage(wrapper.getDocument(), htmlText, ASPx.HtmlEditorClasses.getReplacingElementsOptionsArray());
            htmlText = ASPx.HtmlEditorClasses.HtmlProcessor.preserveAttribute(htmlText, ASPx.HtmlEditorClasses.ContentEditableAttributeNameRegExp);
            htmlText = ASPx.HtmlEditorClasses.HtmlProcessor.preserveAttribute(htmlText, "autoplay");
            htmlText = ASPx.HtmlEditorClasses.HtmlProcessor.restoreSrcAttr(htmlText);
            if(wrapper.settings.placeholders && wrapper.settings.placeholders.length > 0)
                htmlText = wrapper.placeholderManager.replaceTextLabelToPlaceholderElement(htmlText, wrapper.settings.placeholders);
            var selection = wrapper.getSelection();
            selection.SetHtmlInternal(htmlText);
            selection.getSpecialSelection().removeHiddenContainer();
            wrapper.placeholderManager.updatePlaceholdersStyle();
            ASPx.HtmlEditorClasses.Commands.Browser.InsertList.UpdateListNumbering(wrapper.getBody());
            return true;
        },
        canBeExecutedOnSelection: function(wrapper) {
            return true;
        }
    });
    ASPx.HtmlEditorClasses.Commands.PasteHtml.safePreLineBreaks = function(html) {
        var matches = html.match(/<pre[^>]*>[\S\s]*?<\/pre>/gi);
        var result = [];
        for(var i = 0; matches && i < matches.length; i++) {
            result.push(matches[i]);
            html = html.replace(matches[i], "!dxpre"+ i +"!");
        }
        return { html: html, safePreHtmlElements: result };
    };
    ASPx.HtmlEditorClasses.Commands.PasteHtml.restorePreLineBreaks = function(html, safePreHtmlElements) {
        for(var i = 0, safeHtml; safeHtml = safePreHtmlElements[i]; i++)
            html = html.replace("!dxpre"+ i +"!", safeHtml);
        return html;
    };

    ASPx.HtmlEditorClasses.Commands.HtmlView.PasteHtml = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        Execute: function(htmlText, wrapper) {
            ASPx.HtmlEditorClasses.Commands.Command.prototype.Execute.apply(this, arguments);
            wrapper.pasteHtml(htmlText);
            return true;
        },
        canBeExecutedOnSelection: function(wrapper) {
            return true;
        }
    });
})();