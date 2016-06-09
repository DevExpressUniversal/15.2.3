(function() {
    ASPx.HtmlEditorClasses.Commands.InsertLink = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.SelectionManipulationCommand, {
         // cmdValue = { url, text, target, title };
	    Execute: function(cmdValue, wrapper) {
            if(wrapper.tagIsFiltered("a"))
                return false;
            ASPx.HtmlEditorClasses.Commands.SelectionManipulationCommand.prototype.Execute.apply(this, arguments);
            this.ValidateCommandValue(cmdValue);
            var selection = wrapper.getSelection();
        
            this.parentElement = cmdValue.selectedElement;
        
            this.text = ASPx.HtmlEditorClasses.Commands.InsertLink.PrepareLinkText(cmdValue.text);
            this.url = cmdValue.url;
            this.target = cmdValue.target;
            this.title = cmdValue.title;
        
            var link = ASPx.GetParentByTagName(this.parentElement, "A");
            if(link)
                this.SetLinkProperties(link, this.url, this.target, this.title, this.text);
            else if(this.parentElement.tagName == "IMG" || ASPx.ElementHasCssClass(this.parentElement, ASPx.HtmlEditorClasses.PlaceholderCssClasseName))
                this.WrapElementInternal(this.parentElement, this.CreateLink());
            else if(selection.GetHtml()) {
                var elements = selection.GetElements();
                if(elements.length) {
                    var link = this.ApplyLinkFormat(elements, this.CreateLink());
                    selection.clientSelection.SelectElement(link, false);
		            if (ASPx.Browser.Opera)
                        wrapper.getDocument().body.focus();
                }
                else
                    return this.InsertLinkAtCursor(wrapper);
            }
            else
                return this.InsertLinkAtCursor(wrapper);
            return true;
	    },
        InsertLinkAtCursor: function(wrapper) {
            var htmlText = "<a href=\"" + this.url + "\"";
	        if (this.title)
	            htmlText = this.AddAttributeStringToHtml(htmlText, "title", this.title);
	        if (this.target)
	            htmlText = this.AddAttributeStringToHtml(htmlText, "target", this.target);
	            
	        htmlText += ">" + ASPx.HtmlEditorClasses.Commands.InsertLink.PrepareLinkText(this.text) + "</a>";
            return this.owner.getCommand(ASPxClientCommandConsts.PASTEHTML_COMMAND).Execute(htmlText, wrapper);
        },
        CreateLink: function() {
            var link = this.parentElement.ownerDocument.createElement('A');
            if (ASPx.Browser.WebKitFamily)
                link.href = "#";
            return link;
        },
        ApplyLinkFormat: function(elements, link) {
            var lastLink;
            for(var i = 0, el; el = elements[i]; i++) {
                if(el.tagName == "A" && el.href != this.url) {
                    this.SetLinkProperties(el, this.url, this.target, this.title);
                    this.newElements.push(el);
                    lastLink = el;
                }
                else if(ASPx.GetParentByTagName(el, "A")) {
                    var predicate = function(target) { return target.tagName != "A"; };
                    var nnElement = this.SeparateParentByPredicate(el, predicate);
                    this.SetLinkProperties(nnElement, this.url, this.target, this.title);
                    this.newElements.push(nnElement);
                    lastLink = nnElement;
                }
                else if(el.nodeType == 3 || el.tagName == "IMG" || ASPx.ElementHasCssClass(el, ASPx.HtmlEditorClasses.PlaceholderCssClasseName))
                    lastLink = this.WrapElementInternal(el, link.cloneNode(false));
                else
                    lastLink = this.ApplyLinkFormat(el.childNodes, link);
            }
            return lastLink;
        },
        WrapElementInternal: function(el, wrapper) {
            this.SetLinkProperties(wrapper, this.url, this.target, this.title);
            var link = ASPx.HtmlEditorClasses.Commands.SelectionManipulationCommand.prototype.WrapElementInternal.apply(this, arguments);
            if(this.text)
                link.innerHTML = this.text;
            return link;
        },
        CanMerge: function(el1, el2) {
            return el1.tagName == el2.tagName && el1.className == el2.className && el1.href == el2.href;
        },
        // Utils
        ValidateCommandValue: function(cmdValue) {
            cmdValue.url = this.ReplaceDoubleQuotesWithSingleQuotes(cmdValue.url);
            cmdValue.title = this.ReplaceDoubleQuotesWithSingleQuotes(cmdValue.title);
        },
        ReplaceDoubleQuotesWithSingleQuotes: function(str) {
            return str ? str.replace(new RegExp("\"", "g"), "'") : str;
        },
        AddAttributeStringToHtml: function(html, attrName, attrValue) {
            return html + " " + attrName + "=\"" + attrValue + "\"";
        },
        SetLinkProperties: function(linkElement, url, target, title, text) {
            ASPx.Attr.SetOrRemoveAttribute(linkElement, "href", url);
            ASPx.Attr.SetOrRemoveAttribute(linkElement, "target", target);
            ASPx.Attr.SetOrRemoveAttribute(linkElement, "title", title);

            if(text)
                linkElement.innerHTML = text;
        }
    });
    ASPx.HtmlEditorClasses.Commands.InsertLink.PrepareLinkText = function(text) {
        return text.replace("<", "&lt;").replace(">", "&gt;");
    };
})();