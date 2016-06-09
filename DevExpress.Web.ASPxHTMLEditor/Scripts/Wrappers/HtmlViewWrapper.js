(function() {
    ASPx.HtmlEditorClasses.Wrappers.HtmlViewCMWrapper = ASPx.CreateClass(ASPx.HtmlEditorClasses.Wrappers.BaseWrapper,{
        constructor: function(id, settings, callbacks) {
            this.constructor.prototype.constructor.call(this, id, settings, callbacks);
            this.sourceEditor = null;
            this.isIntelliSenseWindowShown = false;
        },
        initialize: function(inlineInit) {
            this.sourceEditor = CodeMirror(document.getElementById(this.id), {
                mode: "htmlmixed",
                lineWrapping: true
            });
            this.applySettings(this.settings);
            this.selectionManager = new ASPx.HtmlEditorClasses.Managers.HtmlViewCMSelectionManager(this);
        },
        performLockedAction: function(action) {
            this.sourceEditor.operation(action);    
        },
        onLockRelease: function() {
            this.updateToolbar(true);
            this.eventManager.onHtmlChanged();    
        },
        applySettings: function(settings) {
            this.sourceEditor.setOption("indentUnit", 4);
            this.sourceEditor.setOption("dragDrop", false);
            this.sourceEditor.setOption("lineNumbers", settings.showLineNumbers);
            this.sourceEditor.setOption("styleActiveLine", settings.highlightActiveLine);
            this.sourceEditor.setOption("autoCloseTags", settings.enableTagAutoClosing);
            this.sourceEditor.setOption("matchTags", (settings.highlightMatchingTags ? { bothTags: true } : false));
            this.sourceEditor.setOption("foldGutter", settings.showCollapseTagButtons);
            this.sourceEditor.setOption("maxHighlightLength", 600000);
            if(settings.showCollapseTagButtons)
                this.sourceEditor.setOption("gutters", ["CodeMirror-linenumbers", "CodeMirror-foldgutter"]);
            this.sourceEditor.options.foldOptions = this.getFoldOptions();
        },
        initializeManagers: function() {
            ASPx.HtmlEditorClasses.Wrappers.BaseWrapper.prototype.initializeManagers.call(this);
            this.eventManager = new ASPx.HtmlEditorClasses.Managers.HtmlViewCMEventManager(this);
        },
        createCommandManager: function() {
            return new ASPx.HtmlEditorClasses.Managers.HtmlViewCMCommandManager(this);
        },
        createKeyboardManager: function (shortcuts, disabledShortcuts) {
            return new ASPx.HtmlEditorClasses.Managers.HtmlViewCMKeyboardManager(shortcuts, disabledShortcuts);
        },
        getName: function() {
            return ASPx.HtmlEditorClasses.View.Html;
        },
        focus: function() {
            this.sourceEditor.focus();
            this.raiseFocus(this);
            ASPx.HtmlEditorClasses.Wrappers.BaseWrapper.prototype.focus.call(this);
        },
        getMainElement: function() {
            return this.sourceEditor.getWrapperElement();
        },
        getRawHtml: function() {
            return this.sourceEditor.getValue();
        },
        getSourceEditor: function() {
            return this.sourceEditor;
        },
        setHtml: function(html) {
            this.insertHtml(this.processHtmlBodyBeforeInsert(html));
            this.sourceEditor.clearHistory();
        },
        processHtmlBodyBeforeInsert: function(html) {
            html = ASPx.HtmlEditorClasses.Wrappers.BaseWrapper.prototype.processHtmlBodyBeforeInsert.call(this, html);
            html = this.convertToEmptyHtml(html);
            return ASPx.HtmlEditorClasses.HtmlProcessor.encodeTextContent(html);
        },
        insertHtml: function(html) {
            this.sourceEditor.setValue(html);
        },
        pasteHtml: function(html) {
            this.selectionManager.setHtml(html);
        },
        adjustControl: function() {
            this.sourceEditor.refresh();
        },
        setSpellCheckAttributeValue: function(value) {
            //var element = this.getInputElement();
            //element.spellcheck = value;
        },
        getSourceEditorState: function() {
            return this.sourceEditor.state;
        },
        getLineByNumber: function(value) {
            return this.sourceEditor.getLine(value);
        },
        getFoldOptions: function() {
          return {
                widget: "",
                minFoldSize: 1
            };
        },
        getSearchSelectedText: function() {
            return this.getSelectedHtml();
        },
        getSearchContainer: function() {
            return document.getElementById(this.id);
        },

        // selection
        getSelectionManager: function() {
            return this.selectionManager;
        },
        getSelectedHtml: function() {
            return this.selectionManager.getHtml();
        },
        setSelection: function(from, to) {
            this.selectionManager.setSelection(from, to);
        },
        saveSelectionByBookmark: function() {
            return this.selectionManager.saveSelectionByBookmark();
        },
        restoreSelectionByBookmark: function() {
            this.selectionManager.restoreSelectionByBookmark(ASPx.HtmlEditorClasses.StartSelectionPosMarkName, ASPx.HtmlEditorClasses.EndSelectionPosMarkName);
        },
        restoreBookmark: function(saveSelectionObjectByBookmark, html) {
            return this.selectionManager.restoreBookmark(saveSelectionObjectByBookmark, html);
        },

        /* Events */
        showIntelliSenseWindow: function() {
            if(this.settings.enableAutoCompletion)
                this.tryInvoke(this.callbacks.onShowIntelliSenseWindow);
        }
    });

    ASPx.HtmlEditorClasses.Wrappers.HtmlViewCMWrapper.Utils = {
        getTokenType: function(state, style, value) {
            if(value == "<" || value == "</")
                return ASPx.HtmlEditorClasses.TokenType.OpenTagBracket;
            else if(value == ">" || value == "/>")
                return ASPx.HtmlEditorClasses.TokenType.CloseTagBracket;
            if(style == "tag")
                return ASPx.HtmlEditorClasses.TokenType.TagName;
            else if(!style && state.tagName && !ASPx.Str.Trim(value))
                return ASPx.HtmlEditorClasses.TokenType.AttributeState;
            else if(style == "attribute")
                return ASPx.HtmlEditorClasses.TokenType.AttributeName;
            else if(!style && state.tagName && value == "=")
                return ASPx.HtmlEditorClasses.TokenType.AttributeEqual;
            else if(style == "string" && state.tagName && /['"]/gi.test(value))
                return ASPx.HtmlEditorClasses.TokenType.AttributeValue;
            else if(style == "script")
                return ASPx.HtmlEditorClasses.TokenType.Script;
            else if(style == "css")
                return ASPx.HtmlEditorClasses.TokenType.Css;
            return ASPx.HtmlEditorClasses.TokenType.Text;
        },
        isAttrTokenType: function(type) {
            return type == ASPx.HtmlEditorClasses.TokenType.AttributeState || type == ASPx.HtmlEditorClasses.TokenType.AttributeName || type == ASPx.HtmlEditorClasses.TokenType.AttributeEqual || type == ASPx.HtmlEditorClasses.TokenType.AttributeValue;
        },
        isCommentToken: function(token, pos, cm) {
            var isComment = function(value) { return value == "comment"; };
            if(token) {
                if(isComment(token.type))
                    return true;
                else {
                    var isNotEmptyToken = function(token) { return token && ASPx.Str.Trim(token.string); };
                    var prevTokenPos = ASPx.HtmlEditorClasses.Wrappers.HtmlViewCMWrapper.Utils.findPrevTokenPos(cm, pos, isNotEmptyToken);
                    var nextTokenPos = ASPx.HtmlEditorClasses.Wrappers.HtmlViewCMWrapper.Utils.findNextTokenPos(cm, pos, isNotEmptyToken);
                    if(prevTokenPos && nextTokenPos) {
                        prevTokenPos.ch++;
                        return isComment(cm.getTokenAt(prevTokenPos).type) && isComment(cm.getTokenAt(nextTokenPos).type);
                    }
                }
            }
            return false;
        },
        getNextToken: function(stream, outer, state) {
           var missingAttrValue = "",
               missingSpaces = "",
               curString = "";
            while (!stream.eol()) {
                var inner = CodeMirror.innerMode(outer, state),
                    style = outer.token(stream, state), 
                    curString = stream.current();

                var tokenType = this.getTokenType(inner.state, style, curString);
                if(this.isAttrTokenType(tokenType)) {
                    stream.start = stream.pos;
                    missingAttrValue += curString;
                }
                else if(!ASPx.Str.Trim(curString))
                    missingSpaces += curString;
                else {
                    stream.start = stream.pos;
                    return { value: curString.replace(missingAttrValue, "").replace(missingSpaces, ""), missingSpaces: missingSpaces, missingAttrValue: missingAttrValue };
                }
            }
            return { value: curString.replace(missingAttrValue, "").replace(missingSpaces, ""), missingSpaces: missingSpaces, missingAttrValue: missingAttrValue };;
        },
        restoreMarkPositionBySafeHtml: function(safeHtml, html) {
            if(!html)
                return ASPx.HtmlEditorClasses.StartSelectionPosMarkName + ASPx.HtmlEditorClasses.EndSelectionPosMarkName;
            var currentSafeHtml = ASPx.HtmlEditorClasses.HtmlProcessor.safeHtml(ASPx.HtmlEditorClasses.HtmlProcessor.encodeTextContent(html));
            var startIndex = safeHtml.html.indexOf(ASPx.HtmlEditorClasses.StartSelectionPosMarkName);
            var endIndex = safeHtml.html.indexOf(ASPx.HtmlEditorClasses.EndSelectionPosMarkName);
            if(safeHtml.html.substring(0, startIndex) == currentSafeHtml.html.substring(0, startIndex)) {
                currentSafeHtml.html = currentSafeHtml.html.substring(0, startIndex) + ASPx.HtmlEditorClasses.StartSelectionPosMarkName + currentSafeHtml.html.substring(startIndex, currentSafeHtml.html.length);
                if(safeHtml.html.substring(startIndex, endIndex) == currentSafeHtml.html.substring(startIndex, endIndex)) {
                    currentSafeHtml.html = currentSafeHtml.html.substring(0, endIndex) + ASPx.HtmlEditorClasses.EndSelectionPosMarkName + currentSafeHtml.html.substring(endIndex, currentSafeHtml.html.length);
                    html = ASPx.HtmlEditorClasses.HtmlProcessor.restoreHtml(currentSafeHtml);
                }
            }
            return html;
        },
        correctSelection: function(wrapper) {
            var eq = function(pos1, pos2) { return pos1.ch == pos2.ch && pos1.line == pos2.line; };
            var correctSelectionInternal = function(func) {
                var oldRange,
                    newRange = wrapper.selectionManager.getRange();
                do {
                    oldRange = newRange;
                    func();
                    newRange = wrapper.selectionManager.getRange();
                }
                while(!eq(oldRange.from(), newRange.from()) || !eq(oldRange.to(), newRange.to()));
            }.aspxBind(this);
            correctSelectionInternal(
                function() {
                    this.correctSelectionPos(wrapper, 
                        function(range) { return range.from(); },
                        function(cm, pos, compare) { return this.findNextTokenPos(cm, pos, compare); }.aspxBind(this),
                        function(tag, newPos, range) { return { from: (tag && /^(body)$/i.test(tag.open.tag) ? tag.open.to : newPos), to: range.to() }; }
                    )
                }.aspxBind(this)
            )
            correctSelectionInternal(
                function() {
                    this.correctSelectionPos(wrapper, 
                        function(range) { return range.to(); },
                        function(cm, pos, compare) { return this.findPrevTokenPos(cm, pos, compare); }.aspxBind(this),
                        function(tag, newPos, range) { return { from: range.from(), to: (tag && tag.close && /^(body)$/i.test(tag.close.tag) ? tag.close.from : newPos) }; }
                    )
                }.aspxBind(this)
            )
        },
        correctSelectionPos: function(wrapper, getPos, findTokenPos, getNewRange) {
            var regExp = /^(script|html|head|title)$/i;
            var range = wrapper.selectionManager.getRange();
            var tag = wrapper.selectionManager.getMatchingTag(getPos(range)) || CodeMirror.findEnclosingTag(wrapper.getSourceEditor(), getPos(range));
            var newPos;
            if(!tag && wrapper.settings.allowEditFullDocument || tag && regExp.test(tag.open.tag)) {
                if(tag && /^(script)$/i.test(tag.open.tag)) {
                    var tempTag = CodeMirror.findEnclosingTag(wrapper.getSourceEditor(), tag.open.from);
                    if(!tempTag || !regExp.test(tempTag.open.tag))
                        newPos = tag.open.from;
                }
                if(!newPos)
                    newPos = findTokenPos(wrapper.getSourceEditor(), getPos(range), function(token) { return token.type == "tag" && /^body$/i.test(token.string); });
            }
            if(newPos) {
                var tag = wrapper.selectionManager.getMatchingTag(newPos);
                var newRange = getNewRange(tag, newPos, range);
                wrapper.selectionManager.setSelection(newRange.from, newRange.to);
            }
        },
        findNextTokenPos: function(cm, pos, compare) {
            var startToken = cm.getTokenAt(pos),
                line = pos.line,
                lineCount = cm.lineCount();
            for(var line = pos.line; line < lineCount; line++) {
                var tokens = cm.getLineTokens(line),
                    startIndex = startToken ? this.tokensIndexOf(tokens, startToken) : 0;
                for(var i = startIndex; token = tokens[i]; i++) {
                    if(compare(token))
                        return CodeMirror.Pos(line, token.end);
                }
                startToken = null;
            }
            return null;
        },
        findPrevTokenPos: function(cm, pos, compare) {
            var startToken = cm.getTokenAt(pos),
                line = pos.line;
            for(var line = pos.line; line >= 0; line--) {
                var tokens = cm.getLineTokens(line),
                    startIndex = startToken ? ASPx.HtmlEditorClasses.Wrappers.HtmlViewCMWrapper.Utils.tokensIndexOf(tokens, startToken) : tokens.length - 1;
                for(var i = startIndex; token = tokens[i]; i--) {
                    if(compare(token))
                        return CodeMirror.Pos(line, token.start);
                }
                startToken = null;
            }
            return null;
        },
        tokensIndexOf: function(tokens, token) {
            var comparer = function(el1, el2) { return el1.start == el2.start && el1.end == el2.end && el1.string == el2.string; };
            return ASPx.Data.ArrayIndexOf(tokens, token, comparer);
        },
        getPositionToBookmarks: function(wrapper) {
            var startPos, endPos;
            var startTagName, endTagName;

            var sel = wrapper.selectionManager.getRange();
            var startMatchingTag = wrapper.selectionManager.getMatchingTag(sel.from());
            if(!startMatchingTag) {
                var token = wrapper.sourceEditor.getTokenAt(sel.from());
                if(ASPx.HtmlEditorClasses.Wrappers.HtmlViewCMWrapper.Utils.isCommentToken(token, sel.from(), wrapper.sourceEditor))
                    startPos = ASPx.HtmlEditorClasses.Wrappers.HtmlViewCMWrapper.Utils.findPrevTokenPos(wrapper.sourceEditor, sel.from(), function(token) { return /<!--/.test(token.string); });
                else
                    startPos = sel.from();
            }
            else {
                if(startMatchingTag.open.tag == "body")
                    startPos = startMatchingTag.open.to;
                else
                    startPos = startMatchingTag.at == "open" ? startMatchingTag.open.from : startMatchingTag.close.from;
                startTagName = startMatchingTag.open.tag;
            }

            var pos = sel.to();
            var endMatchingTag = wrapper.selectionManager.getMatchingTag(pos);
            if(!endMatchingTag || endMatchingTag.open.from.line == pos.line && endMatchingTag.open.from.ch == pos.ch) {
                if(!endMatchingTag) {
                    var token = wrapper.sourceEditor.getTokenAt(pos);
                    if(ASPx.HtmlEditorClasses.Wrappers.HtmlViewCMWrapper.Utils.isCommentToken(token, pos, wrapper.sourceEditor))
                        endPos = ASPx.HtmlEditorClasses.Wrappers.HtmlViewCMWrapper.Utils.findPrevTokenPos(wrapper.sourceEditor, pos, function(token) { return /<!--/.test(token.string); });
                }
                if(!endPos)
                    endPos = pos;
            }
            else if(endMatchingTag.close)
                endPos = endMatchingTag.at == "open" && endMatchingTag.open.tag != "body" ? endMatchingTag.close.to : endMatchingTag.close.from;
            else
                endPos = endMatchingTag.open.to;
            endTagName = endMatchingTag ? endMatchingTag.open.tag : null;
            if(!wrapper.selectionManager.isCollapsed() && startMatchingTag && startMatchingTag.close && (startPos.line == endPos.line && startMatchingTag.close.to.ch > endPos.ch || startMatchingTag.close.to.line > endPos.line)) {
                endPos = startMatchingTag.close.to;
                endTagName = startMatchingTag.close.tag;
            }
            else if(!endPos && startPos && wrapper.selectionManager.isCollapsed())
                endPos = CodeMirror.Pos(startPos.line, startPos.ch);

            return { start: { pos: startPos, tagName: startTagName }, end: { pos: CodeMirror.Pos(endPos.line, endPos.ch), tagName: endTagName} };
        }
    }
})();