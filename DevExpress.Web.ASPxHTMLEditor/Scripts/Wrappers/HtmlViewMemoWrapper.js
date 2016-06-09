(function() {
    ASPx.HtmlEditorClasses.Wrappers.HtmlViewMemoWrapper = ASPx.CreateClass(ASPx.HtmlEditorClasses.Wrappers.BaseWrapper,{
        constructor: function(id, settings, callbacks) {
            this.constructor.prototype.constructor.call(this, id, settings, callbacks);
        },
        initialize: function(inlineInit) {
            this.getHtmlViewEditControl().heightCorrectionRequired = false;
            if(ASPx.Browser.WebKitFamily)
                this.getInputElement().style.resize = "none"; // disable native textarea resizing
        },
        initializeManagers: function() {
            ASPx.HtmlEditorClasses.Wrappers.BaseWrapper.prototype.initializeManagers.call(this);
            this.eventManager = new ASPx.HtmlEditorClasses.Managers.HtmlViewMemoEventManager(this);
        },
        createCommandManager: function() {
            return new ASPx.HtmlEditorClasses.Managers.HtmlViewMemoCommandManager(this);
        },
        getName: function() {
            return ASPx.HtmlEditorClasses.View.Html;
        },
        getHtmlViewEditControl: function() {
            return ASPx.GetControlCollection().Get(this.id);
        },
        focus: function() {
            var htmlViewInput = this.getInputElement();
            htmlViewInput.focus();
            ASPx.Selection.SetCaretPosition(htmlViewInput, 0);
            this.raiseFocus(this);
            ASPx.HtmlEditorClasses.Wrappers.BaseWrapper.prototype.focus.call(this);
        },
        getElement: function() {
            return this.getInputElement();
        },
        getInputElement: function() {
            var htmlViewEditControl = this.getHtmlViewEditControl();
            if(htmlViewEditControl)
                return htmlViewEditControl.GetInputElement();
        },
        getMainElement: function() {
            var htmlViewEditControl = this.getHtmlViewEditControl();
            if(htmlViewEditControl)
                return htmlViewEditControl.GetMainElement();
        },
        getRawHtml: function() {
            var htmlViewEditControl = this.getHtmlViewEditControl();
            if(htmlViewEditControl)
                return htmlViewEditControl.GetText();
        },
        setHtml: function(html) {
            this.insertHtml(this.processHtmlBodyBeforeInsert(html));
        },
        processHtmlBodyBeforeInsert: function(html) {
            html = ASPx.HtmlEditorClasses.Wrappers.BaseWrapper.prototype.processHtmlBodyBeforeInsert.call(this, html);
            html = this.convertToEmptyHtml(html);
            return ASPx.HtmlEditorClasses.HtmlProcessor.encodeTextContent(html);
        },
        insertHtml: function(html) {
            var htmlViewEditControl = this.getHtmlViewEditControl();
            if(htmlViewEditControl)
                return htmlViewEditControl.SetText(html);
        },
        setSpellCheckAttributeValue: function(value) {
            var element = this.getInputElement();
            element.spellcheck = value;
        }
    });
})();