(function() {
    var commentRegExp = new RegExp("<!--|-->", "i");
    var startCommentTagRegExp = new RegExp("<!--", "i");
    var endCommentTagRegExp = new RegExp("-->", "i");

    var scriptCommentRegExp = new RegExp("\\/\\*|\\*\\/", "i");
    var startScriptCommentTagRegExp = new RegExp("\\/\\*", "i");
    var endScriptCommentTagRegExp = new RegExp("\\*\\/", "i");

    ASPx.HtmlEditorClasses.Commands.HtmlView.CommentCommandBase  = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
	    Execute: function(cmdValue, wrapper) {
            var object = this.getObjectToExecuteCommand(wrapper);
            if(object) {
                object.execute(object.start, object.end, object.settings);
                return true;
            }
            return false;
	    },
	    IsLocked: function(wrapper) {
            return !this.getObjectToExecuteCommand(wrapper);
	    },
        isCommentToken: function(token) {
            return token && token.type == "comment";
        },
        isScriptTag: function(tag) {
            return tag && tag.open.tag == "script";
        },
        getObjectToExecuteCommand: function(wrapper) {
        },
        createObject: function(cm, executeFunc, startPos, endPos, settings) {
            return { execute: executeFunc.aspxBind(cm), start: startPos, end: endPos, settings: settings };
        },
        GetState: function(wrapper, selection, selectedElements) {
            return false;
        },
        canBeExecutedOnSelection: function(wrapper, curSelection) {
            return true;
        }
    });

    ASPx.HtmlEditorClasses.Commands.HtmlView.CommentCommand  = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.HtmlView.CommentCommandBase, {
        getObjectToExecuteCommand: function(wrapper) {
            var sourceEditor = wrapper.getSourceEditor(),
                selectionManager = wrapper.getSelectionManager();
            if(selectionManager.isCollapsed()) {
                var tag = selectionManager.getMatchingTag();
                if(tag && (!this.childHasCommentBlock(tag, sourceEditor) || this.isScriptTag(tag)))
                    return this.createObject(sourceEditor, sourceEditor.blockComment, tag.open.from, tag.close.to, { fullLines: false } );
                var token = sourceEditor.getTokenAt(sourceEditor.getCursor());
                if(!this.isCommentToken(token) && !this.childHasCommentBlock(selectionManager.getMatchingTag(), sourceEditor)) {
                    var pos = sourceEditor.getCursor();
                    return this.createObject(sourceEditor, sourceEditor.lineComment, pos, pos, null);
                }
            }
            else { 
                var range = selectionManager.getRange(),
                    startToken = sourceEditor.getTokenAt(range.from()),
                    endToken = sourceEditor.getTokenAt(range.to());
                if(!this.isCommentToken(startToken) && !this.isCommentToken(endToken))
                    return this.createObject(sourceEditor, sourceEditor.blockComment, range.from(), range.to(), { fullLines: false });
            }
            return null;
        },
        childHasCommentBlock: function(matchingTag, cm) {
            if(matchingTag && matchingTag.open && matchingTag.close) {
                var openTag = matchingTag.open,
                    closeTag = matchingTag.close;
                for(var line = openTag.to.line; line <= closeTag.from.line; line++) {
                    var tokens = cm.getLineTokens(line);
                    for(var i = 0; token = tokens[i]; i++) {
                        if(commentRegExp.test(token.string))
                            return CodeMirror.Pos(line, token.end);
                    }
                }
            }
            return false;
        }
    });
    ASPx.HtmlEditorClasses.Commands.HtmlView.UncommentCommand  = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.HtmlView.CommentCommandBase, {
        getObjectToExecuteCommand: function(wrapper) {
            var sourceEditor = wrapper.getSourceEditor(),
                selectionManager = wrapper.getSelectionManager(),
                tag = selectionManager.getSelectedTag(),
                isInScriptBlock = this.isScriptTag(tag);
            var regExp = new RegExp("^\\s*\\/\\/", "i");
            if(selectionManager.isCollapsed()) {
                var pos = sourceEditor.getCursor();
                var token = sourceEditor.getTokenAt(pos);
                if(this.isCommentToken(token)) {
                    if(isInScriptBlock && regExp.test(token.string))
                        return this.createObject(sourceEditor, sourceEditor.uncomment, pos, pos, null);
                    else {
                        var commentBlockPos = this.getCommentBlockPos(pos, sourceEditor, isInScriptBlock);
                        if(commentBlockPos)
                            return this.createObject(sourceEditor, sourceEditor.uncomment, commentBlockPos.startPos, commentBlockPos.endPos, null);
                    }
                }
            }
            else { 
                var range = selectionManager.getRange();
                var comparer = function(pos1, pos2) { return !pos2 || pos1.ch == pos2.ch && pos1.line == pos2.line; };
                var firstCommentBlockPos = this.getCommentBlockPos(range.to(), sourceEditor, isInScriptBlock);
                var secondCommentBlockPos = this.getCommentBlockPos(range.from(), sourceEditor, isInScriptBlock);
                if(isInScriptBlock) {
                    var startToken = this.getToken(range.from(), sourceEditor),
                        endToken = this.getToken(range.to(), sourceEditor);
                    if(this.isCommentToken(startToken) && this.isCommentToken(endToken) && regExp.test(startToken.string) && regExp.test(endToken.string))
                        return this.createObject(sourceEditor, sourceEditor.uncomment, range.from(), range.to(), null);
                    else if(firstCommentBlockPos && secondCommentBlockPos && comparer(firstCommentBlockPos.startPos, secondCommentBlockPos.startPos) && comparer(firstCommentBlockPos.endPos, secondCommentBlockPos.endPos))
                        return this.createObject(sourceEditor, sourceEditor.uncomment, firstCommentBlockPos.startPos, firstCommentBlockPos.endPos, null);
                }
                else {
                    var startToken = sourceEditor.getTokenAt(range.from()),
                        endToken = sourceEditor.getTokenAt(range.to());
                    if(this.isCommentToken(startToken) && this.isCommentToken(endToken) && firstCommentBlockPos && secondCommentBlockPos && comparer(firstCommentBlockPos.startPos, secondCommentBlockPos.startPos) && comparer(firstCommentBlockPos.endPos, secondCommentBlockPos.endPos))
                        return this.createObject(sourceEditor, sourceEditor.uncomment, firstCommentBlockPos.startPos, firstCommentBlockPos.endPos, null);
                }
            }
            return null;
        },
        getCommentBlockPos: function(pos, cm, isInScriptBlosk) {
            var token = this.getToken(pos, cm);
            if(this.isCommentToken(token)) {
                var startPos = ASPx.HtmlEditorClasses.Wrappers.HtmlViewCMWrapper.Utils.findPrevTokenPos(cm, pos, function(token) { return isInScriptBlosk ? startScriptCommentTagRegExp.test(token.string) : startCommentTagRegExp.test(token.string); });
                var endPos = ASPx.HtmlEditorClasses.Wrappers.HtmlViewCMWrapper.Utils.findNextTokenPos(cm, pos, function(token) { return isInScriptBlosk ? endScriptCommentTagRegExp.test(token.string) : endCommentTagRegExp.test(token.string); });
                return startPos && endPos ? { startPos: startPos, endPos: endPos } : null;
            }
            return null;
        },
        getToken: function(pos, cm) {
            var token = cm.getTokenAt(pos);
            if(this.isEmptyToken(token))
                token = cm.getTokenAt(CodeMirror.Pos(pos.line, token.end + 1));
            if(this.isEmptyToken(token))
                token = cm.getTokenAt(CodeMirror.Pos(pos.line, pos.start));
            return token;
        },
        isEmptyToken: function(token) {
            return !token.type && !ASPx.Str.Trim(token.string);
        }
    });
})();