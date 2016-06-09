(function() {

    ASPx.HtmlEditorClasses.HtmlEditorCore = ASPx.CreateClass(null, {
        constructor: function(wrappersArray) {
            this.wrappersArray = wrappersArray;
            
            this.htmlBackup = "";
            this.enabled = true;
            this.activeWrapper = wrappersArray.length > 0 ? wrappersArray[0] : null;
        },
        forEachWrapper: function(action) {
            for(var i = 0, wrapper; wrapper = this.wrappersArray[i]; i++)
                action(wrapper);
        },
        initializeWrapperEventManagers: function() {
            this.forEachWrapper(function(wrapper) { wrapper.initializeManagers(); });
        },
        initializeManagers: function() {
            this.initializeWrapperEventManagers();
            if(this.isDesignViewAllowed() && this.htmlToDelayedSet) {
                this.setHtml(this.htmlToDelayedSet.html, this.htmlToDelayedSet.clearUndoHistory);
                this.htmlToDelayedSet = null;
            }
        },
        initAreas: function(inlineInit) {
            this.forEachWrapper(function(wrapper){ wrapper.initialize(inlineInit); });
            if(this.htmlBackup != "") {
                var wrapper = this.getActiveWrapper();
                if(!this.isHtmlView() && !wrapper.getBody())
                    ASPx.Evt.AttachEventToElement(wrapper.getElement(), "load", this.restoreHtml.aspxBind(this));
                else
                    this.restoreHtml();
            }
        },
        reInitAreasIfNeeded: function() {
            this.forEachWrapper(function(wrapper) { wrapper.initializeIfNeeded(); });
        },
        isNeedReinitIFrame: function() {
            var wrapper = this.getActiveWrapper();
            return wrapper.isIFrameLoaded ? !wrapper.isIFrameLoaded() : false;
        },
        saveHtmlToBackup: function(value) {
            this.htmlBackup = ASPx.IsExists(value) ? value : this.getHtml();
        },
        setHtml: function(html) {
            this.saveHtmlToBackup(html);
            this.getActiveWrapper().setHtml(html);
        },
        getHtml: function() {
            return ASPx.HtmlEditorClasses.HtmlProcessor.encodeTextContent(this.getActiveWrapper().getHtml());
        },
        restoreHtml: function() {
            this.setHtml(this.htmlBackup);
        },
        getBackupHtml: function() {
            return this.htmlBackup;
        },
        setAllowScripts: function(value) {
            this.forEachWrapper(function(wrapper){ 
                if(ASPx.IsExists(wrapper.settings.allowScripts))
                    wrapper.settings.allowScripts = value;
            });
        },
        setSpellCheckAttributeValue: function(value) {
            this.forEachWrapper(function(wrapper){ 
                wrapper.settings.disableBrowserSpellCheck = !value;
                wrapper.setSpellCheckAttributeValue(value);
            });
        },
        setEnabled: function(value) {
            this.getActiveWrapper().enabled = value;
        },
        getActiveWrapper: function() {
            return this.activeWrapper;
        },
        setActiveWrapper: function(wrapper) {
            this.activeWrapper = wrapper;
        },
        isDesignViewAllowed: function() {
            return !!this.getWrapperByName(ASPx.HtmlEditorClasses.View.Design);
        },
        isHtmlViewAllowed: function() {
            return !!this.getWrapperByName(ASPx.HtmlEditorClasses.View.Html);
        },
        isPreviewAllowed: function() {
            return !!this.getWrapperByName(ASPx.HtmlEditorClasses.View.Preview);
        },
        isDesignView: function(view) {
            return (view || this.getActiveWrapper().getName()) == ASPx.HtmlEditorClasses.View.Design;
        },
        isHtmlView: function(view) {
            return (view || this.getActiveWrapper().getName()) == ASPx.HtmlEditorClasses.View.Html;
        },
        isPreview: function(view) {
            return (view || this.getActiveWrapper().getName()) == ASPx.HtmlEditorClasses.View.Preview;
        },
        setActiveWrapperByName: function(value) {
            var currantActiveWrapper = this.getActiveWrapper();
            if((currantActiveWrapper.settings.allowHTML5MediaElements || currantActiveWrapper.settings.allowObjectAndEmbedElements || currantActiveWrapper.settings.allowYouTubeVideoIFrames) && this.isPreview(currantActiveWrapper.getName()))
                currantActiveWrapper.reinitIFrame();
            this.setActiveWrapper(this.getWrapperByName(value));
        },
        getWrapperByName: function(name) {
            for(var i = 0, wrapper; wrapper = this.wrappersArray[i]; i++) {
                if(wrapper.getName() == name)
                    return wrapper;
            }
            return null;
        }, 
        onHtmlChangedCore: function(saveSelectionAndHtml, preventEvent) {
            var wrapper = this.getActiveWrapper();
            if(this.isDesignView(wrapper.getName())) {
                ASPx.HtmlEditorClasses.Utils.UnforcedFunctionCall(function() {
                    this.htmlBackup = saveSelectionAndHtml && wrapper.commandManager.lastHTML != undefined ?  wrapper.commandManager.lastHTML :  wrapper.getHtml();
                }.aspxBind(this), "htmlBackup", 400, true);
            }
        }
    });
})();