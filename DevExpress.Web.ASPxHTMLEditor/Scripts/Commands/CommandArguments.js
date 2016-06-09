

(function() {
    var ASPxClientHtmlEditorCommandStyleSettings = ASPx.CreateClass(null, {
        constructor: function(element) {
            this.resetProperties();
            if(element)
                this.initializeProperties(element);
        },
        resetProperties: function() {
            this.className = "";
            this.width = "";
            this.height = "";

            this.borderWidth = "";
            this.borderColor = "";
            this.borderStyle = "";
            
            this.marginTop = "";
            this.marginRight = "";
            this.marginBottom = "";
            this.marginLeft = "";
        },
        initializeProperties: function(element) {
            this.className = element.className;
            this.width = element.style.width;
            this.height = element.style.height;
            this.borderWidth = element.style.borderWidth;
            this.borderColor = element.style.borderColor;
            this.borderStyle = element.style.borderStyle;
            this.marginTop = element.style.marginTop;
            this.marginRight = element.style.marginRight;
            this.marginBottom = element.style.marginBottom;
            this.marginLeft = element.style.marginLeft;
        }
    });
    var ASPxClientHtmlEditorCommandArguments = ASPx.CreateClass(null, {
        constructor: function(htmlEditor, selectedElement) {
            this.selectedElement = selectedElement;
            if(!this.selectedElement && htmlEditor)
                this.selectedElement = htmlEditor.GetSelection().GetSelectedElement();
            if(this.selectedElement)
                this.initializeProperties(this.selectedElement);
        },
        getSelectedElement: function() {
            return this.selectedElement;    
        },
        initializeProperties: function(element) { }
    });
    var ASPxClientHtmlEditorInsertImageCommandArguments = ASPx.CreateClass(ASPxClientHtmlEditorCommandArguments, {
        constructor: function(htmlEditor, selectedElement) {
            this.src = "";
            this.alt = "";
            this.useFloat = false;
            this.align = "";
            
            this.constructor.prototype.constructor.call(this, htmlEditor, selectedElement);
            this.styleSettings = new ASPxClientHtmlEditorCommandStyleSettings(this.getElementToChange());
        },
        getElementToChange: function() {
            return null;    
        }
    });
    var ASPxClientHtmlEditorChangeImageCommandArguments = ASPx.CreateClass(ASPxClientHtmlEditorInsertImageCommandArguments, {
        getElementToChange: function() {
            return this.getSelectedElement();    
        },
        initializeProperties: function(element ){
            this.src = element.src;
            this.alt = element.alt;
        }
    });
    var ASPxClientHtmlEditorInsertLinkCommandArguments = ASPx.CreateClass(ASPxClientHtmlEditorCommandArguments, {
        constructor: function(htmlEditor, selectedElement) {
            this.url = "";
            this.text = "";
            this.target = "";
            this.title = "";

            this.constructor.prototype.constructor.call(this, htmlEditor, selectedElement);
        }
    });

    window.ASPxClientHtmlEditorInsertImageCommandArguments = ASPxClientHtmlEditorInsertImageCommandArguments;
    window.ASPxClientHtmlEditorChangeImageCommandArguments = ASPxClientHtmlEditorChangeImageCommandArguments;
    window.ASPxClientHtmlEditorInsertLinkCommandArguments = ASPxClientHtmlEditorInsertLinkCommandArguments;
})();