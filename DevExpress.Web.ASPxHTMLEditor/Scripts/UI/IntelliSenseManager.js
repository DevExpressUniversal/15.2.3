(function () {
    var IntelliSenseManager = ASPx.CreateClass(null, {
	    constructor: function(wrapper) {
            this.wrapper = wrapper;
            this.dtdElementDeclaration = new ASPx.HtmlEditorClasses.Utils.DtdElementDeclaration(this.wrapper.settings.documentType);
            this.bufferItems = null;
            this.currentToken = null;
        },
        getItems: function() {
            if(this.isCurrentTokenChanged())
                this.bufferItems = this.getItemsInternal();
            return this.bufferItems;
        },
        isCurrentTokenChanged: function() {
            if(!this.currentToken)
                return true;
            var token = this.wrapper.selectionManager.getCurrentToken();
            return token && (this.currentToken.string != token.string || this.currentToken.start != token.start || this.currentToken.end != token.end);
        },
        saveCurrentToken: function(token) {
            this.currentToken = token;
        },
        getItemsInternal: function() {
            var sourceEditor = this.wrapper.getSourceEditor(),
                selectionManager = this.wrapper.getSelectionManager();

            if(!selectionManager.isCollapsed()) 
                return [];

            var cur = selectionManager.getCursorPos(),
                token = sourceEditor.getTokenAt(cur),
                state = CodeMirror.innerMode(sourceEditor.getMode(), token.state).state,
                posState = selectionManager.getPositionState();
            this.saveCurrentToken(token);
            if(this.isTagState(posState))
                return this.getTags(state, token, cur, posState);
            else if(this.isAttState(posState))
                return this.getAttrs(state, token, cur);
            else if(this.isAttValueState(posState))
                return this.getAttrValueList(state, token, cur);
            return [];
        },
        isTagState: function(posState) {
            return posState == ASPx.HtmlEditorClasses.TokenType.Text || posState == ASPx.HtmlEditorClasses.TokenType.OpenTagBracket || posState == ASPx.HtmlEditorClasses.TokenType.CloseTagBracket || posState == ASPx.HtmlEditorClasses.TokenType.TagName;
        },
        isAttState: function(posState) {
            return posState == ASPx.HtmlEditorClasses.TokenType.AttributeState || posState == ASPx.HtmlEditorClasses.TokenType.AttributeName;
        },
        isAttValueState: function(posState) {
            return posState == ASPx.HtmlEditorClasses.TokenType.AttributeValue;
        },
        getTags: function(state, token, pos, posState) {
            var result = [],
                elements = [],
                sourceEditor = this.wrapper.getSourceEditor();

            if(state.context && state.context.tagName && !this.closingTagExists(sourceEditor, state.context.tagName, pos, state))
                elements.push("/" + state.context.tagName + ">");
            else if(posState == ASPx.HtmlEditorClasses.TokenType.OpenTagBracket) {
                var nextToken = sourceEditor.getTokenAt(CodeMirror.Pos(pos.line, token.end + 1));
                if(nextToken && nextToken.type == "tag")
                    token = nextToken;
            }
            var selectedTag = this.wrapper.getSelectionManager().getSelectedTag();
            var tagName = selectedTag ? selectedTag.open.tag : (this.wrapper.settings.allowEditFullDocument ? "" : "body");

            elements = elements.concat(this.dtdElementDeclaration.getContentElements(tagName));
            if(!elements || elements.length == 0)
                return result;
            if(posState == ASPx.HtmlEditorClasses.TokenType.Text && ASPx.Str.Trim(token.string))
                token = this.getTextToken(token, pos);
            for(var i = 0, element; element = elements[i]; i++) {
                if(/^(<|\s*|>)$/gi.test(token.string) || element.indexOf(token.string) > -1)
                    result.push({ name: element, iconType: ASPx.HtmlEditorClasses.IconType.XmlItem });
            }
            return result;
        },
        getAttrs: function(state, token, pos) {
            var result = [],
                sourceEditor = this.wrapper.getSourceEditor(),
                attrList = this.dtdElementDeclaration.getAttrListByElementName(state.tagName),
                curAttrList = this.getCurrentTagAttrList(pos, sourceEditor);

            for(var i = 0, attr; attr = attrList[i]; i++) {
                if((!ASPx.Data.ArrayContains(curAttrList, attr.name) || attr.name == token.string) && (/\s+/gi.test(token.string) || attr.name.indexOf(token.string) > -1))
                    result.push({ name: attr.name, iconType: attr.iconType });
            }
            return result;
        },
        getAttrValueList: function(state, token, pos) {
            var result = [],
                sourceEditor = this.wrapper.getSourceEditor(),
                attrPos = this.getAttrPos(pos, sourceEditor);

            if(attrPos) {
                var attrName = sourceEditor.getTokenAt(attrPos).string,
                    attr = this.dtdElementDeclaration.getAttr(state.tagName, attrName);
                if(attr && attr.valueList) {
                    var curAttrValue = token.string.replace(/'|"/gi, "");
                    for(var i = 0, value; value = attr.valueList[i]; i++) {
                        if(value.indexOf(curAttrValue) > -1 || !curAttrValue)
                            result.push({ name: value, iconType: ASPx.HtmlEditorClasses.IconType.Enum });
                    }
                }
            }
            return result;
        },
        getTextToken: function(token, pos) {
            var chPos = (token.end - token.start) - (token.end - pos.ch);
            var startPos = 0,
                endPos = token.string.length;
            if(endPos > 0) {
                for(var i = chPos - 1; i >= 0; i--) {
                    if(!ASPx.Str.Trim(token.string[i])) {
                        startPos = i + 1;
                        break;
                    }
                }
                for(var i = chPos - 1; i < token.string.length; i++) {
                    if(!ASPx.Str.Trim(token.string[i])) {
                        endPos = i;
                        break;
                    }
                }
            }
            return { start: token.start + startPos, end: token.start + endPos, string: token.string.substring(startPos, endPos) };
        },
        closingTagExists: function(cm, tagName, pos, state) {
            if (!CodeMirror.scanForClosingTag) 
                return false;
            var end = Math.min(cm.lastLine() + 1, 
                pos.line + 500);
            var nextClose = CodeMirror.scanForClosingTag(cm, pos, null, end);
            if (!nextClose || nextClose.tag != tagName) 
                return false;
            var cx = state.context;
            for (var onCx = 0; cx && cx.tagName == tagName; cx = cx.prev) 
                ++onCx;
            pos = nextClose.to;
            for (var i = 1; i < onCx; i++) {
                var next = CodeMirror.scanForClosingTag(cm, pos, null, end);
                if (!next || next.tag != tagName) return false;
                pos = next.to;
            }
            return true;
        },
        getStartTagPos: function(pos, cm) {
            var isFoundToken = function(token) { return token.string != ">"; }
            return this.getPos(pos, cm, "tag", isFoundToken);
        },
        getAttrPos: function(pos, cm) {
            var isFoundToken = function(token) { return token.type == "attribute"; }
            return this.getPos(pos, cm, "attribute", isFoundToken);
        },
        getPos: function(pos, cm, tokenType, isFoundToken) {
            var token = cm.getTokenAt(pos),
                line = pos.line,
                ch = token.start;
            while(token && token.type != tokenType && line >= 0 && !/<|>/gi.test(token.string)) {
                if(token.start == 0) {
                    line--;
                    ch = cm.getLine(line).length;
                }
                token = cm.getTokenAt(CodeMirror.Pos(line, ch));
                ch = token.start;
            }
            return isFoundToken ? CodeMirror.Pos(line, ch + 1) : null;
        },
        getCurrentTagAttrList: function(pos, cm) {
            var pos = this.getStartTagPos(pos, cm),
                result = [];
            if(pos) {
                var token = cm.getTokenAt(pos),
                    line = pos.line,
                    ch = token.end,
                    lineLength = cm.getLine(line).length,
                    lineCount = cm.lineCount();
                while(token && !/<|>/gi.test(token.string) && line < lineCount) {
                    if(ch >= lineLength) {
                        line++;
                        ch = 0;
                        if(line >= lineCount)
                            return result;
                        lineLength = cm.getLine(line).length;
                    }
                    token = cm.getTokenAt(CodeMirror.Pos(line, ch));
                    if(token.type == "attribute")
                        result.push(token.string);
                    ch = token.end + 1;
                }
            }
            return result;
        },
        insertItem: function(cmdValue) {
            var sourceEditor = this.wrapper.getSourceEditor(),
                cur = sourceEditor.getCursor(),
                token = sourceEditor.getTokenAt(cur),
                selectionManager = this.wrapper.getSelectionManager();
            var posState = selectionManager.getPositionState();
            if(posState == ASPx.HtmlEditorClasses.TokenType.OpenTagBracket || posState == ASPx.HtmlEditorClasses.TokenType.TagName || posState == ASPx.HtmlEditorClasses.TokenType.CloseTagBracket || posState == ASPx.HtmlEditorClasses.TokenType.Text)
                this.insertTag(this.wrapper, cmdValue, token, cur, posState);
            else if(posState == ASPx.HtmlEditorClasses.TokenType.AttributeValue && token.end != cur.ch)
                this.insertAttrValue(this.wrapper, cmdValue, token, cur);
            else if(posState == ASPx.HtmlEditorClasses.TokenType.AttributeState || posState == ASPx.HtmlEditorClasses.TokenType.AttributeName)
                this.insertAttr(this.wrapper, cmdValue, token, cur);
	    },
        insertTag: function(wrapper, tagName, token, pos, posState) {
            var sourceEditor = wrapper.getSourceEditor();
            if(posState == ASPx.HtmlEditorClasses.TokenType.CloseTagBracket || posState == ASPx.HtmlEditorClasses.TokenType.Text)
                tagName = "<" + tagName;
            else if(posState == ASPx.HtmlEditorClasses.TokenType.OpenTagBracket) {
                var nextToken = sourceEditor.getTokenAt(CodeMirror.Pos(pos.line, token.end + 1));
                if(nextToken && nextToken.type == "tag") {
                    token = nextToken;
                    posState = ASPx.HtmlEditorClasses.TokenType.TagName
                }
            }
            if(posState == ASPx.HtmlEditorClasses.TokenType.Text)
                token = this.getTextToken(token, pos);
            if(/^(<|>|\s*)$/gi.test(token.string))
                sourceEditor.replaceSelection(tagName);
            else
                sourceEditor.replaceRange(tagName, CodeMirror.Pos(pos.line, token.start), CodeMirror.Pos(pos.line, token.end));
        },
        insertAttr: function(wrapper, attrName, token, pos) {
            var sourceEditor = wrapper.getSourceEditor();
            var isAttrEqState = this.isAttrEqState(CodeMirror.Pos(pos.line, token.end + 1), sourceEditor);
            if(attrName == "=" && !isAttrEqState) {
                sourceEditor.replaceSelection("=\"\"");
                this.showIntelliSenseWindowToAttrValue(wrapper);
            }
            else if(/\s+/gi.test(token.string)) {
                if(!isAttrEqState) {
                    sourceEditor.replaceSelection(attrName + "=\"\"");
                    this.showIntelliSenseWindowToAttrValue(wrapper);
                }
                else
                    sourceEditor.replaceSelection(attrName);
            }
            else {
                if(!isAttrEqState)
                    attrName += "=\"\"";
                sourceEditor.replaceRange(attrName, CodeMirror.Pos(pos.line, token.start), CodeMirror.Pos(pos.line, token.end));
                if(!isAttrEqState)
                    this.showIntelliSenseWindowToAttrValue(wrapper);
            }
        },
        insertAttrValue: function(wrapper, attrValue, token, pos) {
            wrapper.getSourceEditor().replaceRange("\"" + attrValue + "\"", CodeMirror.Pos(pos.line, token.start), CodeMirror.Pos(pos.line, token.end));
        },
        isAttrEqState: function(pos, cm) {
            var token = cm.getTokenAt(pos),
                line = pos.line,
                ch = token.end,
                lineLength = cm.getLine(line).length,
                lineCount = cm.lineCount();
            while(token && token.string != ">" && line < lineCount) {
                if(ch >= lineLength) {
                    line++;
                    ch = 0;
                    if(line >= lineCount)
                        return false;
                    lineLength = cm.getLine(line).length;
                }
                token = cm.getTokenAt(CodeMirror.Pos(line, ch));
                if(!/^\s+$/gi.test(token.string))
                    return token.string == "="
                ch = token.end + 1;
            }
            return false;
        },
        showIntelliSenseWindowToAttrValue: function(wrapper) {
            wrapper.getSourceEditor().execCommand("goCharLeft");
            setTimeout(function() {
                wrapper.commandManager.getCommand(ASPxClientCommandConsts.SHOWINTELLISENSE_COMMAND).Execute(null, wrapper);
            }, 0);
        }
    });

    ASPx.HtmlEditorClasses.Controls.IntelliSenseManager = IntelliSenseManager;
})();