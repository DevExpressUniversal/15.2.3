/// <reference path="_references.js"/>
/// <reference path="HtmlEditorConstants.js"/>
/// <reference path="HtmlEditorCore.js"/>
/// <reference path="EventManager.js"/>
(function() {
    var popupMenuIDSuffix = "_PPM";
    ASPx.HtmlEditorClasses.Controls.ContextMenuManager = ASPx.CreateClass(null, {
	    constructor: function(htmlEditor) {
	        this.htmlEditor = htmlEditor;
            this.core = htmlEditor.core;
            this.eventManager = null;
        },
        initializeEventManager: function() {
            if(this.isContextMenuAllowed() && (this.isInternalContextMenuMode() || !this.isBrowserContextMenuMode()))
                this.eventManager = new ASPx.HtmlEditorClasses.Managers.ContextMenuEventManager(this);
        },
        getContextMenuControl: function() {
            return ASPx.GetControlCollection().Get(this.htmlEditor.name + popupMenuIDSuffix);
        },
        updateContextMenu: function() {
            if(this.htmlEditor.GetEnabled() && this.isContextMenuAllowed() && this.isInternalContextMenuMode() && this.getContextMenuControl().GetVisible())
                this.getContextMenuControl().SetVisible(false);
        },
        showContextMenu: function(evt, wrapper) {
            if(this.htmlEditor.GetEnabled() && this.isContextMenuAllowed() && this.isInternalContextMenuMode()) {
                // TODO
                if(wrapper.saveSelectionForPopup)
                    wrapper.saveSelectionForPopup();
                if(!this.getContextMenuControl().GetVisible()) {
                    this.updateContextMenuItemsState();
                    this.htmlEditor.RaiseContextMenuShowing();
                    var isDesignView = wrapper.getName() == ASPx.HtmlEditorClasses.View.Design;
                    var x = isDesignView ? ASPx.Evt.GetEventX(evt) + ASPx.GetAbsoluteX(wrapper.getElement()) - ASPx.GetDocumentScrollLeft() : ASPx.Evt.GetEventX(evt); 
                    var y = isDesignView ? ASPx.Evt.GetEventY(evt) + ASPx.GetAbsoluteY(wrapper.getElement()) - ASPx.GetDocumentScrollTop() : ASPx.Evt.GetEventY(evt);
                    this.getContextMenuControl().ShowAtPos(x + 8, y + 8);
                    if(ASPx.Browser.IE && wrapper.eventManager)
                        wrapper.eventManager.isPreventKeyPressOnShowContextMenu = true;
                }
            }
        },
        updateContextMenuItemsState: function() {
            this.hideContextMenuItems(ASPx.HtmlEditorClasses.DefaultCommands.getDesignViewCommands());
            this.hideContextMenuItems(ASPx.HtmlEditorClasses.DefaultCommands.getHtmlViewCMCommands());
            this.updateContextMenu(this.htmlEditor.getActiveWrapper());
        },
        updateContextMenu: function(wrapper) {
            if(wrapper) {
                var selection;
                var popupMenu = this.getContextMenuControl();
                if(wrapper.getName() == ASPx.HtmlEditorClasses.View.Design)
                    selection = wrapper.getSelection();

                for(var i = 0; i < popupMenu.GetItemCount(); i++) {
                    var item = popupMenu.GetItem(i);
                    var command = wrapper.commandManager.getCommand(item.name);
                    if(command)
                        item.SetVisible(command.isEnabled(wrapper, selection));
                }
            }
        },
        hideContextMenuItems: function(defaultCommands) {
            var popupMenu = this.getContextMenuControl();
            for(var i = 0, command; command = defaultCommands[i]; i++) {
                var item = popupMenu.GetItemByName(command.cmdName);
                if(item)
                    item.SetVisible(false);
            }
        },

        isContextMenuAllowed: function() {
            return (this.htmlEditor.isDesignViewAllowed() || this.htmlEditor.isHtmlViewAllowed() && !this.htmlEditor.isSimpleHtmlEditingMode());
        },
        isInternalContextMenuMode: function() {
            return this.htmlEditor.allowContextMenu == "true";
        },
        isBrowserContextMenuMode: function() {
            return this.htmlEditor.allowContextMenu == "default";
        }
    });
})();