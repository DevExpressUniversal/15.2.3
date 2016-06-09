(function() {
    ASPx.HtmlEditorClasses.Commands.Browser.Unlink = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.Command, {
        Execute: function (cmdValue, wrapper) {
            var bookmark;
            var isSuccessfully = true;
            var selection = wrapper.getSelection();
            if(ASPx.Browser.IE || !selection.IsCollapsed()) {
                if(ASPx.Browser.NetscapeFamily && !selection.IsCollapsed())
                    bookmark = selection.clientSelection.GetExtendedBookmark();
                isSuccessfully = ASPx.HtmlEditorClasses.Commands.Browser.Command.prototype.Execute.call(this, cmdValue, wrapper);
            }
            else {
                var linkElement = ASPx.GetParentByTagName(selection.GetSelectedElement(), "A");
                if(!linkElement)
                    return false;
                bookmark = selection.clientSelection.GetExtendedBookmark();
                for(var i = 0, childNode; childNode = linkElement.childNodes[i]; i++)
                    linkElement.parentNode.insertBefore(childNode.cloneNode(true), linkElement);
                ASPx.RemoveElement(linkElement);
            }
            if(bookmark)
                selection.clientSelection.SelectExtendedBookmark(bookmark);
            return isSuccessfully;
        },
        TryGetIsLocked: function (wrapper) {
            if(!wrapper.isInFocus)
                return true;
            if(!ASPx.Browser.Firefox && !ASPx.Browser.WebKitFamily)
                return ASPx.HtmlEditorClasses.Commands.Browser.Command.prototype.TryGetIsLocked.apply(this, arguments);
            return false;
        },
        canBeExecutedOnSelection: function(wrapper, curSelection) {
            var selection = wrapper.getSelection();
            var html = selection.GetHtml();
            if(html && html.indexOf("<\/a>") > -1) {
                var html = html.replace(/<a.*?<\/a>/i, "");
                if(ASPx.Browser.Firefox && !html) {
                    var bookmark = selection.clientSelection.GetExtendedBookmark();
                    var doc = wrapper.getDocument();
                    var startMarkerElement = doc.getElementById(bookmark.startMarkerID);
                    var endMarkerElement = doc.getElementById(bookmark.endMarkerID);
                    startMarkerParentElement = startMarkerElement.parentNode;
                    doc.body.normalize();
                    if(!startMarkerElement.previousSibling && startMarkerParentElement.nodeName == "A" && startMarkerParentElement == endMarkerElement.previousSibling)
                        startMarkerParentElement.appendChild(endMarkerElement);
                    selection.clientSelection.SelectExtendedBookmark(bookmark);
                }
                else if(!ASPx.Str.Trim(html))
                    return true;
            }
            var parent = ASPx.GetParentByTagName(wrapper.getSelection().GetSelectedElement(), "A");
            if(parent && parent.nodeName == "A")
                return true;
            return false;
        }
    });
})();