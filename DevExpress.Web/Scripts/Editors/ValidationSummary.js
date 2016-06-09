/// <reference path="..\_references.js"/>

(function() {
var ASPxValidationSummaryRenderMode = {
    Table: "t",
    List: "l"
};

var ValidationSummaryDomHelper = ASPx.CreateClass(null, {
    constructor: function(validationSummary) {
        this.validationSummary = validationSummary;
        this.editorNameErrorContainerMap = { };
        this.errorCount = 0;
    },
    
    // Initialization
    
    CreateErrors: function(invalidEditorNames) {
        var controlCollection = ASPx.GetControlCollection();
        for(var i = 0; i < invalidEditorNames.length; i++) {
            var editorName = invalidEditorNames[i];
            var editor = controlCollection.Get(editorName);
            if(editor) {
            	if(editor.GetIsValid() || editor.validationGroup != this.validationSummary.validationGroup)
                	continue;
            	var errorText = editor.HtmlEncode(editor.GetErrorText());

            	var errorContainer = this.SetError(editorName, errorText, true /* forceNewErrorsAccepting */);
            	this.editorNameErrorContainerMap[editorName] = errorContainer;
            }
        }
    },
    
    // General operations
    
    SetError: function(editorName, errorText, forceNewErrorsAccepting) {
        var errorContainer = this.GetOrCreateErrorContainer(editorName, this.validationSummary.acceptNewErrors || forceNewErrorsAccepting);
        if(errorContainer) {
            this.InsertErrorTextAndLinkIntoErrorContainer(editorName, errorContainer, errorText);
            this.validationSummary.UpdateVisibility();
        }
        return errorContainer;
    },
    RemoveError: function(editorName) {
        var errorContainer = this.editorNameErrorContainerMap[editorName];
        if(ASPx.IsExistsElement(errorContainer)) {
            delete this.editorNameErrorContainerMap[editorName];
            errorContainer.parentNode.removeChild(errorContainer);
            this.errorCount--;
            this.validationSummary.UpdateVisibility();
        }
    },
    HasErrors: function() {
        return this.errorCount > 0;
    },
    GetInvalidEditorNames: function() {
        var names = [ ];
        for(var editorName in this.editorNameErrorContainerMap) {
            if(typeof(editorName) == "string") {
                var editor = ASPx.GetControlCollection().Get(editorName);
                if(ASPx.Ident.IsASPxClientEdit(editor))
                    names.push(editorName);
            }
        }
        return names;
    },
    
    // Low-level DOM operations
    
    GetOrCreateErrorContainer: function(editorName, summaryAcceptsNewErrors) {
        var errorContainer = this.editorNameErrorContainerMap[editorName];
        if(!errorContainer && summaryAcceptsNewErrors) {
            errorContainer = this.CreateErrorContainer();
            this.editorNameErrorContainerMap[editorName] = errorContainer;
            var errorsContainer = this.GetErrorsContainer();
            this.AppendError(errorsContainer, errorContainer);
            this.errorCount++;
        }
        return errorContainer;
    },
    GetErrorsContainer: function() {
        var rootTable = this.validationSummary.GetMainElement();
        if(rootTable) {
            var rootCell = rootTable.rows[0].cells[0];
            return ASPx.GetChildElementNodes(rootCell)[this.validationSummary.hasHeader ? 1 : 0];
        }
    },
    GetEffectiveErrorsContainer: function(errorsContainer) {
        if(!errorsContainer)
            errorsContainer = this.GetErrorsContainer();
        if(errorsContainer && errorsContainer.tagName == "TABLE") {
            var tbody = ASPx.GetNodeByTagName(errorsContainer, "TBODY", 0);
            if(!tbody) {
                tbody = document.createElement("TBODY");
                errorsContainer.appendChild(tbody);
            }
            return tbody;
        } else
            return errorsContainer;
    },
    CreateErrorContainer: function() {
        var sample = this.GetSampleErrorContainerNode();
        return sample.cloneNode(true /* deep */);
    },
    InsertErrorTextAndLinkIntoErrorContainer: function(editorName, errorContainer, errorText) {
        var errorTextContainer;

        if(this.validationSummary.showErrorAsLink)
            errorTextContainer = ASPx.GetNodeByTagName(errorContainer, "A", 0);
        else if(this.validationSummary.renderMode == ASPxValidationSummaryRenderMode.Table)
            errorTextContainer = ASPx.GetNodeByTagName(errorContainer, "TD", 0);
        else
            errorTextContainer = errorContainer;
            
        errorTextContainer.innerHTML = errorText;
        
        if(this.validationSummary.showErrorAsLink)
            errorTextContainer.href = "javascript:ASPx.VSOnErrorClick('" + editorName + "');";
    },
    AppendError: function(errorsContainer, errorContainer) {
        var effectiveErrorsContainer = this.GetEffectiveErrorsContainer(errorsContainer);
        if (effectiveErrorsContainer)
            effectiveErrorsContainer.appendChild(errorContainer);
    },

    // Sample error container

    GetSampleErrorContainerNode: function() {
        if(!this.sampleErrorContainerNode)
            this.sampleErrorContainerNode = this.CreateSampleErrorContainerNode();
        return this.sampleErrorContainerNode;
    },
    CreateSampleErrorContainerNode: function() {
        var errorsContainer = this.GetErrorsContainer();
        var errorsContainerTagName = errorsContainer.tagName;
        var html = "<" + errorsContainerTagName + ">" + this.validationSummary.sampleErrorContainer + "</" + errorsContainerTagName + ">";
        var div = document.createElement("DIV");
        div.innerHTML = html;
        var effectiveTemporaryErrorsContainer = this.GetEffectiveErrorsContainer(ASPx.GetChildElementNodes(div)[0]);
        var sample = ASPx.GetChildElementNodes(effectiveTemporaryErrorsContainer)[0];
        sample.parentNode.removeChild(sample);
        return sample;
    }
});
var ASPxClientValidationSummary = ASPx.CreateClass(ASPxClientControl, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        
        this.isASPxClientValidationSummary = true;
        
        this.validationGroup = "";
        this.invalidEditorNames = [ ];
        this.renderMode = ASPxValidationSummaryRenderMode.Table;
        this.showErrorAsLink = true;
        this.hasHeader = false;
        
        this.domHelper = new ValidationSummaryDomHelper(this);
        this.acceptNewErrors = false;
        this.VisibilityChanged = new ASPxClientEvent();
    },
    
    // Initialization

    InlineInitialize: function() {
        var summaryCollection = aspxGetClientValidationSummaryCollection();
        summaryCollection.RegisterSummary(this);
        
        this.RemoveFakeItem();
        this.UpdateVisibility(this.invalidEditorNames.length > 0 /* hasErrors */, true /* initializing */);
        
        ASPxClientControl.prototype.InlineInitialize.call(this);
    },
    Initialize: function() {
        this.domHelper.CreateErrors(this.invalidEditorNames);
        this.UpdateVisibility();
    },
    
    // Public

    AllowNewErrorsAccepting: function() {
        this.acceptNewErrors = true;
    },
    ForbidNewErrorsAccepting: function() {
        this.acceptNewErrors = false;
    },
    
    // Synchronization

    UpdateStateObject: function(){
        if(!this.IsDOMDisposed())
            this.UpdateStateObjectWithObject({ invalidEditors: this.domHelper.GetInvalidEditorNames() });
    },

    // Main operations

    SetError: function (editorName, errorText, forceNewErrorsAccepting) {
        if(!this.IsDOMDisposed())
            this.domHelper.SetError(editorName, errorText, forceNewErrorsAccepting);
    },
    RemoveError: function(editorName) {
        this.domHelper.RemoveError(editorName);
    },
    UpdateVisibility: function(hasErrors, initializing) {
        if(typeof(hasErrors) == "undefined")
            hasErrors = this.domHelper.HasErrors();
        this.SetVisible(hasErrors, initializing);
    },
    SetVisible: function(visible, initializing) {
        var visibilityChanged = this.GetVisible() != visible;

        if(visibilityChanged)
            ASPxClientControl.prototype.SetVisible.call(this, visible);

        if(visibilityChanged || initializing) {
            var args = new ASPxClientValidationSummaryVisibilityChangedEventArgs(visible);
            this.VisibilityChanged.FireEvent(this, args);
        }
    },
    
    // Misc

    RemoveFakeItem: function() {
        var fakeItem = this.GetChildElement(ASPxClientValidationSummary.FakeItemIDSuffix);
        ASPx.RemoveElement(fakeItem);
    }
});

ASPxClientValidationSummary.AllowNewErrorsAccepting = function(validationGroup) {
    aspxGetClientValidationSummaryCollection().AllowNewErrorsAccepting(validationGroup);
};
ASPxClientValidationSummary.ForbidNewErrorsAccepting = function(validationGroup) {
    aspxGetClientValidationSummaryCollection().ForbidNewErrorsAccepting(validationGroup);
};

ASPxClientValidationSummary.FakeItemIDSuffix = "_FI";

ASPx.Ident.IsASPxClientValidationSummary = function(obj) {
    return obj && obj.isASPxClientValidationSummary;
};
var ASPxClientValidationSummaryVisibilityChangedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(visible) {
        this.constructor.prototype.constructor.call(this);
        this.visible = visible;
    }
});

var ASPxClientValidationSummaryCollection = ASPx.CreateClass(null, {
    constructor: function() {
        this.summaries = { };
    },
    
    // External operations
    
    AllowNewErrorsAccepting: function(validationGroup) {
        this.ProcessValidationGroupSummaries(validationGroup, function(summary) {
            summary.AllowNewErrorsAccepting();
        });
    },
    ForbidNewErrorsAccepting: function(validationGroup) {
        this.ProcessValidationGroupSummaries(validationGroup, function(summary) {
            summary.ForbidNewErrorsAccepting();
        });
    },
    OnEditorIsValidStateChanged: function(editor, validationType, notifyValidationSummariesToAcceptNewError) {
        if(validationType != ASPx.ValidationType.PersonalOnValueChanged){
            if(editor.GetIsValid())
                this.RemoveError(editor);
            else
                this.SetError(editor, notifyValidationSummariesToAcceptNewError);
        }
    },

    // Summary Operations
    AddSummaryToGroupSummaries: function(groupSummaries, summary) {
        for(var i = 0; i < groupSummaries.length; i++) {
            if(groupSummaries[i].name == summary.name) {
                groupSummaries[i] = summary;
                return;
            }
        }
        groupSummaries.push(summary);
    },
    RegisterSummary: function(summary) {
        var groupSummaries = this.GetValidationGroupSummaries(summary.validationGroup);
        this.AddSummaryToGroupSummaries(groupSummaries, summary);
    },
    SetError: function(editor, forceNewErrorsAccepting) {
        this.ProcessValidationGroupSummaries(editor.validationGroup, function(summary, editorName, errorText, _forceNewErrorsAccepting) {
            summary.SetError(editorName, errorText, _forceNewErrorsAccepting);
        }, [ editor.name, editor.HtmlEncode(editor.GetErrorText()), forceNewErrorsAccepting ]);
    },
    RemoveError: function(editor) {
        this.ProcessValidationGroupSummaries(editor.validationGroup, function(summary, editorName) {
            summary.RemoveError(editorName);
        }, editor.name);
    },
    
    // Utils
    
    /* processingProc = function(summary, arg1, arg2, ...) { ... } */
    ProcessValidationGroupSummaries: function(validationGroup, processingProc, args) {
        if(!args)
            args = [ ];
        else if(args.constructor != Array)
            args = [ args ];
        var groupSummaries = this.GetValidationGroupSummaries(validationGroup);
        for(var i = 0; i < groupSummaries.length; i++)
            processingProc.apply(null, [groupSummaries[i]].concat(args));
    },
    GetValidationGroupSummaries: function(validationGroup) {
        var groupSummaries = [ ];

        if(!ASPx.IsExists(validationGroup)) {
            for(var groupName in this.summaries) {
                var summaries = this.summaries[groupName];
                if(ASPx.Ident.IsArray(summaries)) {
                    for(var i = 0; i < summaries.length; i++) {
                        var summary = summaries[i];
                        if(ASPx.Ident.IsASPxClientValidationSummary(summary))
                            this.AddSummaryToGroupSummaries(groupSummaries, summary);
                    }
                }
            }
        } else {
            groupSummaries = this.summaries[validationGroup];
            if(!groupSummaries) {
                groupSummaries = [ ];
                this.summaries[validationGroup] = groupSummaries;
            }
        }
        
        return groupSummaries;
    }
});

var validationSummaryCollection = null;
function aspxGetClientValidationSummaryCollection() {
    if(!validationSummaryCollection)
        validationSummaryCollection = new ASPxClientValidationSummaryCollection();
    return validationSummaryCollection;
}

ASPx.VSOnErrorClick = function(editorName) {
    var editor = ASPx.GetControlCollection().Get(editorName);
    if(editor && ASPx.IsFunction(editor.SetFocus))
        editor.SetFocus();
}

ASPx.ValidationSummaryDomHelper = ValidationSummaryDomHelper;
ASPx.GetClientValidationSummaryCollection = aspxGetClientValidationSummaryCollection;

window.ASPxClientValidationSummary = ASPxClientValidationSummary;
window.ASPxClientValidationSummaryVisibilityChangedEventArgs = ASPxClientValidationSummaryVisibilityChangedEventArgs;
})();