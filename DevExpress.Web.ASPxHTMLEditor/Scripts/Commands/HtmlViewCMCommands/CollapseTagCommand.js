(function() {
    ASPx.HtmlEditorClasses.Commands.HtmlView.CollapseTagCommandBase  = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
	    Execute: function(cmdValue, wrapper) {
            var isSuccessfully = false;
            var sourceEditor = wrapper.getSourceEditor();
            sourceEditor.foldCode(sourceEditor.getCursor(), this.getOption(), "fold");
            return true;
	    },
        getOption: function() {
            return {
                rangeFinder: CodeMirror.fold.auto,
                widget: "",
                minFoldSize: 1,
                scanUp: true
            }
        },
        GetState: function(wrapper, selection, selectedElements) {
            return false;
        },
	    IsLocked: function(wrapper) {
            return wrapper.getSourceEditor().somethingSelected();
	    },
        canBeExecutedOnSelection: function(wrapper, curSelection) {
            return true;
        },
        getFoldMarker: function(sourceEditor, line) {
            var markedSpans = sourceEditor.getLineHandle(line).markedSpans;
            if(markedSpans) {
                for(var i = 0, markedSpan; markedSpan = markedSpans[i]; i++) {
                    var marker = markedSpan.marker;
                    if(marker.__isFold) 
                        return marker;
                }
            }
            return null;
        },
        getFoldedStartLineNumber: function(sourceEditor, line) {
            var marker = this.getFoldMarker(sourceEditor, line);
            if(marker)
                return sourceEditor.getLineNumber(marker.lines[0]);
            return null;
        }
    });
    ASPx.HtmlEditorClasses.Commands.HtmlView.CollapseTagCommand  = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.HtmlView.CollapseTagCommandBase, {
	    Execute: function(cmdValue, wrapper) {
            var sourceEditor = wrapper.getSourceEditor();
            var openTagPos = this.getOpenTagPos(sourceEditor);
            if(!openTagPos)
                return false;
            sourceEditor.foldCode(openTagPos, this.getOption(), "fold");
            return true;
	    },
	    IsLocked: function(wrapper) {
            var result = ASPx.HtmlEditorClasses.Commands.HtmlView.CollapseTagCommandBase.prototype.IsLocked.call(this, wrapper);
            if(result)
                return result;
            var sourceEditor = wrapper.getSourceEditor();
            var openTagPos = this.getOpenTagPos(sourceEditor);
            if(openTagPos)
                return !!this.getFoldMarker(sourceEditor, openTagPos.line);
            return true;
	    },
        getOpenTagPos: function(sourceEditor) {
            var cursorPos = sourceEditor.getCursor();
            var tag = CodeMirror.findMatchingTag(sourceEditor, cursorPos);
            if(!tag)
                tag = CodeMirror.findEnclosingTag(sourceEditor, cursorPos);
            return tag ? tag.open.from : null;
        }
    });
    ASPx.HtmlEditorClasses.Commands.HtmlView.ExpandTagCommand  = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.HtmlView.CollapseTagCommandBase, {
	    Execute: function(cmdValue, wrapper) {
            var sourceEditor = wrapper.getSourceEditor();
            var cursorPos = sourceEditor.getCursor();
            var lineNumber = this.getFoldedStartLineNumber(sourceEditor, cursorPos.line);
            if(lineNumber == null)
                return false;
            sourceEditor.foldCode(CodeMirror.Pos(lineNumber, 0), this.getOption(), "unfold");
            return true;
	    },
	    IsLocked: function(wrapper) {
            var result = ASPx.HtmlEditorClasses.Commands.HtmlView.CollapseTagCommandBase.prototype.IsLocked.call(this, wrapper);
            if(result)
                return result;
            var sourceEditor = wrapper.getSourceEditor();
            return !this.getFoldMarker(sourceEditor, sourceEditor.getCursor().line);
	    }
    });
})();