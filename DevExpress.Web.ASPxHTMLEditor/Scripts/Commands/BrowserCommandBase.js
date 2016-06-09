(function() {
    ASPx.HtmlEditorClasses.Commands.Browser = { };

    ASPx.HEBlockElements = { div:1, address:1, blockquote:1, center:1, table:1, h1:1, h2:1, h3:1, h4:1, h5:1, h6:1, p:1, pre:1, ol:1, ul:1, dl:1, hr:1 };
    ASPx.HEPathBlockElements = { address:1, blockquote:1, dl:1, h1:1, h2:1, h3:1, h4:1, h5:1, h6:1, p:1, pre:1, li:1, dt:1, de:1 };
    ASPx.HEPathBlockLimitElements = { html:1, body: 1, form: 1, div:1, table:1, tbody:1, tr:1, td:1, th:1, li:1, caption:1 };
    ASPx.HEBogusSymbol = "\u200B";

    ASPx.IsEmptyHtml = function(html){
        var html = ASPx.Str.Trim(html);
        return html == "" || html == "&nbsp;" || html == "<P>&nbsp;</P>";
    }

    ASPx.HtmlEditorClasses.Commands.Browser.Command = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
	    Execute: function(cmdValue, wrapper) {
            ASPx.HtmlEditorClasses.Commands.Command.prototype.Execute.apply(this, arguments);
		    var contentAreaDoc = wrapper.getDocument();
		    if (!this.NeedUseCss())
       	        contentAreaDoc.execCommand(this.GetUseCSSCommandName(), false, false);
		    if(!wrapper.isSelectionRestored()) {
		        if(ASPx.Browser.Opera) // B219088
		            wrapper.focus();
                else
		            wrapper.restoreSelection();
		    }
       	    var isSuccessfully = contentAreaDoc.execCommand(this.GetCommandName(), false, this.GetCorrectedValue(cmdValue));
		    if (!this.NeedUseCss())
       	        contentAreaDoc.execCommand(this.GetUseCSSCommandName(), false, true);
            if(isSuccessfully) {
                wrapper.saveSelection();
                if((ASPx.Browser.WebKitTouchUI || ASPx.Browser.MSTouchUI) && !wrapper.isInFocus)
                    wrapper.eventManager.setLockUpdate(true);
                setTimeout(function() {
                    if(!wrapper.isSelectionRestored())
                        wrapper.restoreSelection();
                }.aspxBind(this), 300);
            }
		    return isSuccessfully;
	    },
	    GetCommandName: function() {
	        return this.commandID;
	    },	
	    GetCorrectedValue: function(value) {
	        return value;	    
	    },
	    GetState: function(wrapper, selection, selectedElements) {
	        var ret = true;
	        if (!this.IsAlwaysEnabledCommand(this.commandID)) {	        	        
                try {
	                ret = this.TryGetState(wrapper);
	            }
	            catch(ex) { ret = false; }
	        }
            return ret;
	    },
	    GetValue: function(wrapper, selection, selectedElements) {	    
	        var ret = null;
            try {
	            ret = this.TryGetValue(wrapper);
	        }
	        catch(e) {}
	        return !ret ? null : ret;
	    },
	    GetUseCSSCommandName: function(doc) {
	        return ASPx.Browser.NetscapeFamily ? "styleWithCSS" : "useCSS";
	    },
	    IsLocked: function(wrapper) {
	        var ret = this.TryGetIsLocked(wrapper);
	        if (ASPx.Browser.Opera && ret) {// Opera bug. When text underline (bold and etc.), then queryCommandEnabled == false
                try { 
                    ret = !this.TryGetState(wrapper); 
                }
	            catch(ex) { ret = false; }
	        }
	        return ret;
	    },
	    TryGetState: function(wrapper) {
            return wrapper.getDocument().queryCommandState(this.GetCommandName());	
	    },
	    TryGetValue: function(wrapper) {
	        return wrapper.getDocument().queryCommandValue(this.GetCommandName());
	    },
	    TryGetIsLocked: function(wrapper) {
	        try {
	            return !wrapper.getDocument().queryCommandEnabled(this.GetCommandName());
	        } catch(e) {}
	        return true;
	    },
	
	    IsAlwaysEnabledCommand: function(commandID) {
	        return this.commandID == ASPxClientCommandConsts.FONTSIZE_COMMAND || this.commandID == ASPxClientCommandConsts.FONTNAME_COMMAND;
	    },
	    NeedUseCss: function() {
	        return true;
	    }
    });

    ASPx.HtmlEditorClasses.Commands.SelectionManipulationCommand = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
         // cmdValue = { tagName, cssClass };
        Execute: function(cmdValue, wrapper) {
            this.newElements = [];

            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion > 8)
                wrapper.restoreSelection();
        },
        WrapElementInternal: function(target, wrapper) {
            var inline = ASPx.HtmlEditorClasses.Commands.Utils.IsInlineTextElement(wrapper);
            if((inline || this.IsNewElement(target.previousSibling)) && target.previousSibling && this.CanMerge(target.previousSibling, wrapper)) {
                target.previousSibling.appendChild(target);
                return target.previousSibling;
            }
            else if ((inline || this.IsNewElement(target.nextSibling)) && target.nextSibling && this.CanMerge(target.nextSibling, wrapper)) {
                target.nextSibling.insertBefore(target, target.nextSibling.firstChild);
                return target.nextSibling;
            }
            else {
                target.parentNode.insertBefore(wrapper, target);
                wrapper.appendChild(target);
                this.newElements.push(wrapper);
                return wrapper;
            }
        },

        IsNewElement: function(target) {
            for(var i = 0, el; el = this.newElements[i]; i++) {
                if(target == el)
                    return true;
            }
            return false;
        },
        CanMerge: function(el1, el2) {
            return el1.tagName == el2.tagName && el1.className == el2.className && !this.isBlockElement(el1) && !this.isBlockElement(el2);
        },
        SeparateParentByPredicate: function(target, predicate) {
            while(target && predicate(target) && this.SeparateParent(target, target.parentNode.className, false)) {
                target = target.parentNode;
            }
            return target;
        },
        SeparateParent: function(target, className, tryMerge) { // 1
            var parent = target.parentNode;
            if(!parent || parent.tagName == "BODY")
                return false;
            if(target.previousSibling) {
                var prevParent = parent.previousSibling;
                if(!prevParent || prevParent.tagName != parent.tagName || prevParent.className != parent.className || this.isBlockElement(prevParent)) {
                    prevParent = parent.cloneNode(false);
                    parent.parentNode.insertBefore(prevParent, parent);
                }
                for(var child = parent.firstChild; child && child != target; child = parent.firstChild) {
                    prevParent.appendChild(child);
                }
            }
            if(target.nextSibling) {
                var nextParent = parent.nextSibling;
                if(!nextParent || nextParent.tagName != parent.tagName || nextParent.className != parent.className || this.isBlockElement(nextParent)) {
                    nextParent = parent.cloneNode(false);
                    ASPx.InsertElementAfter(nextParent, parent);
                }
                for(var child = parent.lastChild; child && child != target; child = parent.lastChild) {
                    nextParent.insertBefore(child, nextParent.firstChild);
                }
            }
            this.SetClassName(parent, className);
            if(tryMerge && parent.previousSibling && this.CanMerge(parent, parent.previousSibling)) {
                parent.previousSibling.appendChild(target);
                ASPx.RemoveElement(parent);
            }
            else if(tryMerge && parent.nextSibling && this.CanMerge(parent, parent.nextSibling)) {
                parent.nextSibling.insertBefore(target, parent.nextSibling.firstChild);
                ASPx.RemoveElement(parent);
            }
            return true;
        },
        SetClassName: function(target, className) {
            if(!className) 
                ASPx.Attr.RemoveAttribute(target, !ASPx.Browser.IE || ASPx.Browser.Version >= 8 ? "class" : "className");
            else
                target.className = className;
        },
        isBlockElement: function(element) {
            return element.nodeType == 1 && element.innerHTML && ASPx.GetCurrentStyle(element)["display"] == "block";
        }
    });

})();