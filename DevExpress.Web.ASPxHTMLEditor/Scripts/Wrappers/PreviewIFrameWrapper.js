(function() {

    function evalScripts(scriptElements, window, doc) {
        if(scriptElements.length) {
            var scriptElement = scriptElements.shift();
            if(scriptElement.src) {
                var newScriptElement = doc.createElement("SCRIPT");
                ASPx.Evt.AttachEventToElement(newScriptElement, "load", function() {
                    evalScripts(scriptElements, window, doc);            
                });
                ASPx.Evt.AttachEventToElement(newScriptElement, "error", function(e) {
                    evalScripts(scriptElements, window, doc);            
                });
                newScriptElement.src = scriptElement.src;
                newScriptElement.charset = scriptElement.charset;
                newScriptElement.crossOrigin = scriptElement.crossOrigin;
                newScriptElement.type = scriptElement.type;
                newScriptElement.text = scriptElement.text;
                ASPx.InsertElementAfter(newScriptElement, scriptElement);
                ASPx.RemoveElement(scriptElement);
            } else if(scriptElement.text) {
                window.eval(scriptElement.text);
                evalScripts(scriptElements, window, doc);
            }
        }        
    }

    ASPx.HtmlEditorClasses.Wrappers.PreviewIFrameWrapper = ASPx.CreateClass(ASPx.HtmlEditorClasses.Wrappers.IFrameWrapper, {
        constructor: function(id, settings, callbacks) {
            this.constructor.prototype.constructor.call(this, id, settings, callbacks);
            this.html = "";
        },
        initializeManagers: function() {
            ASPx.HtmlEditorClasses.Wrappers.IFrameWrapper.prototype.initializeManagers.call(this);
            this.eventManager = new ASPx.HtmlEditorClasses.Managers.PreviewEventManager(this);
        },
        getName: function() {
            return ASPx.HtmlEditorClasses.View.Preview;
        },
        focus: function() {
            this.getWindow().focus();
            ASPx.HtmlEditorClasses.Wrappers.IFrameWrapper.prototype.focus.call(this);
        },
        getViewAreaStyleCssText: function() {
            return ASPx.IsExists(this.settings.viewAreaStyleCssText) ? this.settings.viewAreaStyleCssText : "";
        },
        getDocumentClassName: function() {
            return "dxhePreviewDoc";
        },
        runScripts: function () {
            if (this.settings.isScriptExecutionAllowed) {
                var doc = this.getDocument();
                if (doc) {
                    var wrapperWindow = this.getWindow();
                    var scripts = doc.getElementsByTagName("SCRIPT");
                    setTimeout(function () {  // T271633
                        evalScripts(ASPx.Data.CollectionToArray(scripts), wrapperWindow, doc);
                    }, 0);
                }
            }
        },
        getRawHtml: function() {
            return this.html;
        },
        canDepreserveScriptTags: function() {
            return !this.settings.isScriptExecutionAllowed;
        },
        insertHtml: function(html) {
            this.setInnerHtml(this.getBody(), html);
            this.html = this.getBody().innerHTML;
            this.replaceLinkTargetWithBlank(this.getBody());
            this.processingFormElements(this.getBody());
            this.runScripts()
        },
        reinitIFrame: function() {
            this.initialize();
            if(this.eventManager)
                this.eventManager.attachEvents();
        },
        setHtml: function(html) {
            if(this.settings.isScriptExecutionAllowed)
                this.reinitIFrame();
            ASPx.HtmlEditorClasses.Wrappers.IFrameWrapper.prototype.setHtml.call(this, html);
        },
        processHtmlBodyBeforeInsert: function(html) {
            html = ASPx.HtmlEditorClasses.Wrappers.IFrameWrapper.prototype.processHtmlBodyBeforeInsert.call(this, html);
            html = this.depreserveAttribute(html, "autoplay");
            return html;
        }
    });
})();