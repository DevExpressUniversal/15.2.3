(function() {
    var validationIDSuffix = {
        ButtonImageIdPostfix: "Img"
    };
    ASPx.HtmlEditorClasses.Managers.ValidationManager = ASPx.CreateClass(null, {
	    constructor: function(htmlEditor) {
	        this.htmlEditor = htmlEditor;
            this.templateHoverErrorFrameCloseButton = null;
            this.isValid = true; // TODO

            this.clientValidationEnabled = false;
            this.validationPatterns = [];
            this.initialErrorText = "";
            this.errorText = "";
            this.notifyValidationSummariesToAcceptNewError = false;
        },
        initialize: function() {
            if(this.htmlEditor.enabled)
                this.initializeErrorFrames();
            this.updateClientValidationState();
            this.updateValidationSummaries(null /* validationType */, true /* initializing */);
        },
        initializeErrorFrames: function() {
            this.initialErrorText = this.errorText;
            this.createErrorFrames();
        },
        createErrorFrames: function() {
            if(!this.htmlEditor.getTemplateErrorFrame())
                return;
            this.createAndInsertErrorFrame(this.htmlEditor.getDesignViewCell(), ASPx.HtmlEditorClasses.View.Design);
            this.createAndInsertErrorFrame(this.htmlEditor.getHtmlViewEditCell(), ASPx.HtmlEditorClasses.View.Html);
            this.createAndInsertErrorFrame(this.htmlEditor.getPreviewCell(), ASPx.HtmlEditorClasses.View.Preview);
        },
        createAndInsertErrorFrame: function(cell, view) {
            if(!cell)
                return;
            var errorFrame = this.createErrorFrame(
                this.htmlEditor.getErrorFrameID(view), 
                this.htmlEditor.getErrorTextCellID(view), 
                this.htmlEditor.getErrorFrameCloseButtonCellID(view));
            if(view == ASPx.HtmlEditorClasses.View.Design && this.htmlEditor.enableTagInspector)
                ASPx.InsertElementAfter(errorFrame, cell.firstChild);
            else
                cell.insertBefore(errorFrame, cell.firstChild);
        },
        createErrorFrame: function(errorFrameId, errorTextCellId, errorFrameCloseButtonCellId) {
            var errorFrame = this.htmlEditor.getTemplateErrorFrame().cloneNode(true);
            errorFrame.id = errorFrameId;
            errorTable = ASPx.GetNodeByTagName(errorFrame, "TABLE", 0);
            var errorTextCell = ASPx.GetNodeByTagName(errorTable, "TD", 0);
            errorTextCell.id = errorTextCellId;
            var errorFrameCloseButtonCell = ASPx.GetNodeByTagName(errorTable, "TD", 2);
            errorFrameCloseButtonCell.id = errorFrameCloseButtonCellId;
            var errorFrameCloseButtonImage = ASPx.GetNodeByTagName(errorFrameCloseButtonCell, "IMG", 0);
            errorFrameCloseButtonImage.id = errorFrameCloseButtonCellId + validationIDSuffix.ButtonImageIdPostfix;

            this.createErrorFrameCloseButtonHoverState(errorFrameCloseButtonCell.id);

            return errorFrame;
        },
        createErrorFrameCloseButtonHoverState: function(buttonID) {
            if(this.templateHoverErrorFrameCloseButton) {
                ASPx.GetStateController().AddHoverItem(buttonID,
                    this.templateHoverErrorFrameCloseButton.className, this.templateHoverErrorFrameCloseButton.cssText,
                    this.templateHoverErrorFrameCloseButton.postfixes, this.templateHoverErrorFrameCloseButton.imageUrls,
                    this.templateHoverErrorFrameCloseButton.imagePostfixes);
            }
        },
        getIsValid: function(){
            return this.isValid;
        },
        getErrorText: function(){
            return this.errorText;
        },
        setIsValid: function(isValid, validating){
            if(this.clientValidationEnabled) {
                this.isValid = isValid;
                this.updateErrorFrame();
                this.updateClientValidationState();
                if(!validating)
                    this.updateValidationSummaries(ASPx.ValidationType.PersonalViaScript);
            }
        },
        setErrorText: function(errorText, validating){
            if(this.clientValidationEnabled) {
                this.errorText = errorText;
                this.updateErrorFrame();
                this.updateClientValidationState();
                if(!validating)
                    this.htmlEditor.updateValidationSummaries(ASPx.ValidationType.PersonalViaScript);
            }
        },
        validate: function() {
            this.onValidation(ASPx.ValidationType.PersonalViaScript);
        },
        beginErrorFrameUpdate: function() {
            if(!this.errorFrameUpdateLocked)
                this.errorFrameUpdateLocked = true;
        },
        endErrorFrameUpdate: function() {
            this.errorFrameUpdateLocked = false;
            this.updateErrorFrame();
        },
        onValidation: function(validationType) {
            if(this.clientValidationEnabled && this.htmlEditor.isInitialized) {
                this.beginErrorFrameUpdate();

                this.setIsValid(true, true);
                this.setErrorText(this.initialErrorText, true);

                this.isValid = this.validateWithPatterns();
                if(this.isValid) {
                    var currentHtml = this.htmlEditor.GetHtml();
                    var args = this.htmlEditor.RaiseValidation(currentHtml, this.isValid, this.errorText);
                    if(currentHtml != args.html)
                        this.htmlEditor.SetHtml(args.html);
                    this.setIsValid(args.isValid, true);
                    this.setErrorText(args.errorText, true);
                }
                this.endErrorFrameUpdate();

                this.updateClientValidationState();
                this.updateValidationSummaries(validationType);

                return this.isValid;
            }
            else
                return true;
        },
        validateWithPatterns: function() {
            if(this.validationPatterns.length > 0) {
                var html = this.htmlEditor.GetHtml();
                for(var i = 0; i < this.validationPatterns.length; i++) {
                    var validator = this.validationPatterns[i];
                    if(!validator.EvaluateIsValid(html)) {
                        this.setIsValid(false, true);
                        this.setErrorText(validator.errorText, true);
                        return false;
                    }
                }
            }
            return true;
        },
        updateErrorFrame: function() {
            if(this.errorFrameUpdateLocked)
                return;
            if(!this.htmlEditor.IsIFrameReady()) {
                this.needUpdateErrorFrame = true;
                return;
            }
            this.needUpdateErrorFrame = false;
            this.htmlEditor.layoutManager.saveCurrentSize(false, true);
            var errorTextCell = this.htmlEditor.getErrorTextCell();
            if(errorTextCell != null)
                errorTextCell.innerHTML = this.htmlEditor.HtmlEncode(this.errorText);
            if(!this.isValid)
                this.showErrorFrame();
            else
                this.hideErrorFrame();
            this.htmlEditor.layoutManager.restoreHeight();
        },
        showErrorFrame: function() {
            this.setErrorFrameVisibility(true);
        },
        hideErrorFrame: function() {
            this.setErrorFrameVisibility(false);
        },
        setErrorFrameVisibility: function(visible) {
            var errorFrame = this.htmlEditor.getErrorFrame();
            if(errorFrame != null)
                ASPx.SetElementDisplay(errorFrame, visible);
        },
        updateClientValidationState: function() {
            var validationState = !this.isValid ? this.errorText : "";
            this.htmlEditor.UpdateStateObjectWithObject({ validationState: validationState });
        },
        updateValidationSummaries: function(validationType, initializing) {
            if(typeof(ASPxClientValidationSummary) != "undefined") {
                var summaryCollection = ASPx.GetClientValidationSummaryCollection();
                if(summaryCollection != null)
                    summaryCollection.OnEditorIsValidStateChanged(this.htmlEditor, validationType, initializing && this.htmlEditor.notifyValidationSummariesToAcceptNewError);
            }
        },
        isStandardValidationEnabled: function() {
            return !!window.ValidatorHookupControl;
        },
        redirectStandardValidators: function() {
            var contentToValidate = this.htmlEditor.getStandardValidationHiddenField();
            if(contentToValidate.Validators) {
                for(var i = 0; i < contentToValidate.Validators.length; i++)
                    contentToValidate.Validators[i].controltovalidate = contentToValidate.id;
            }
        }
    });
})();