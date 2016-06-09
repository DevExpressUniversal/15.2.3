/// <reference path="_references.js"/>

(function() {
    var constants = {
        TABCONTROL_POSTFIX: "_TC",
        GROUPCOLLAPSEPOPUP_POSTFIX: "_GPC",
        TABMINIMIZEPOPUP_POSTFIX: "_MPC",
        TAB_CONTENTCONTROL_POSTFIX: "_T",
        TABSCONTAINER_POSTFIX: "_RTC",
        GROUP_POSTFIX: "G",
        GROUPDIALOGBOXLAUNCHER_POSTFIX: "_DBL",
        
        POPUP_MENU_POSTFIX: "_PM",
        POPUP_CONTROL_POSTFIX: "_IPC",

        COMBOBOX_POSTFIX: "_CMB",
        CHECKBOX_POSTFIX: "_CB",
        SPINEDIT_POSTFIX: "_SE",
        TEXTBOX_POSTFIX: "_TB",
        DATEEDIT_POSTFIX: "_DE",
        POPUP_GALLERY_POSTFIX: "_PG",

        MINIMIZEBUTTON_POSTFIX: "_TPTCL_MinBtn",
        
        INACTIVETAB_NAME: "DXR_INACTIVE",
        FILETAB_NAME: "DXR_FILE",
        
        COLORTABLE_POSTFIX: "_IPC_CT",
        COLORINDICATOR_POSTFIX: "_CI",

        MENU_INDEXPATH_SEPARATOR: "i",
        COLORITEM_CLIENTSTATE_SEPARATOR: "|",

        CLASSNAMES: {
            LARGEITEM: "dxr-largeSize",
            ITEMLABEL: "dxr-label",
            ITEMPOPOUT: "dxr-popOut",
            ITEMDDIMAGECONTAINER: "dxr-ddImageContainer",
            GROUPLIST: "dxr-groupList",
            BLOCK: "dxr-block",
            TAB_HOVER: "dxtc-tabHover", //T244124

            BLOCK_REGULAR_ITEMS: "dxr-blRegItems",
            BLOCK_REGULAR_ITEMS_REDUCED: "dxr-blRegItems dxr-blReduced",
            BLOCK_LARGE_ITEMS: "dxr-blLrgItems",
            BLOCK_SEPARATE_ITEMS: "dxr-blSepItems",
            BLOCK_HORIZONTAL_ITEMS: "dxr-blHorItems",
            BLOCK_HORIZONTAL_ITEMS_REDUCED: "dxr-blHorItems dxr-blReduced",
            BLOCK_HORIZONTAL_ITEMS_HIDE: "dxr-blHorItems dxr-blHide",
            BlOCK_HIDE: "dxr-blHide",

            GROUP_COLLAPSED: "dxr-group dxr-grCollapsed",
            GROUP_EXPAND: "dxr-grExpBtn",
            GROUP_DIALOG_BOX: "dxr-grDialogBoxLauncher",
            GROUP: "dxr-group",
            ONE_LINE_MODE_GROUP_EXPAND: "dxr-olmGrExpBtn",
            ONE_LINE_MODE_GROUP_EXPAND_VISIBLE: "dxr-olmGrExpBtn dxr-olmGrExpBtnVisible",

            GROUP_LABEL: "dxr-groupLabel",
            GROUP_CONTENT: "dxr-groupContent",

            BUTTONITEM: "dxr-buttonItem",
            ITEM: "dxr-item",

            IMAGE16: "dxr-img16",
            IMAGE32: "dxr-img32",

            LABELTEXT: "dxr-lblText",
            LABELCONTENT: "dxr-lblContent",

            TABWRAPPER: "dxr-tabWrapper",

            GALLERY_ITEM: "dxr-glrItem",
            GALLERY_MAIN_DIV: "dxr-glrMainDiv",
            GALLERY_BAR_CONTAINER: "dxr-glrBarContainer",
            GALLERY_POPOUT: "dxr-glrPopout",
            GALLERY_BAR_DIV: "dxr-glrBarDiv",
            GALLERY_GROUP_DIV: "dxr-glrGroup",
            GALLERY_ITEM_CONTENT: "dxr-glrItemContent",
            GALLERY_ITEM_TEXT: "dxr-glrItemText"
        },

        ITEMTYPES: {
            BUTTON: 0,
            TEMPLATE: 1,
            DROPDOWNSPLIT: 2,
            DROPDOWNMENU: 3,
            TOGGLE: 4,
            OPTION: 5,
            SPINEDIT: 6,
            COLOR: 7,
            TEXTBOX: 8,
            DATEEDIT: 9,
            COMBOBOX: 10,
            CHECKBOX: 11,
            DROPDOWNTOGGLEBUTTON: 12,
            GALLERYDROPDOWN: 13,
            GALLERYBAR: 14
        }
    };

    ASPxClientRibbonCollection = {
        ribbons: {},
        register: function(ribbon) {
            ASPxClientRibbonCollection.ribbons[ribbon.name] = ribbon;
        },
        find: function(id) {
            var ribbons = ASPxClientRibbonCollection.ribbons;
            for(var name in ribbons) {
                if(!ribbons.hasOwnProperty(name)) continue;
                if(id.indexOf(name) == 0)
                    return ASPxClientRibbonCollection.get(name);
            }
            return null;
        },
        get: function(name) {
            return ASPxClientRibbonCollection.ribbons[name];
        },
        onMouseDown: function(evt) {
            for(var name in ASPxClientRibbonCollection.ribbons) {
                if(!ASPxClientRibbonCollection.ribbons.hasOwnProperty(name)) continue;
                var ribbon = ASPxClientRibbonCollection.get(name);
                if(ASPx.IsExists(ribbon.GetMainElement()))
                    ribbon.onMouseDown(evt);
            }
        }
    };

    ASPx.Evt.AttachEventToDocument(ASPx.TouchUIHelper.touchMouseDownEventName, ASPxClientRibbonCollection.onMouseDown);
    var ASPxClientRibbon = ASPx.CreateClass(ASPxClientControl, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.activeTab = null;
            this.activeTabIndex = 0;
            this.tabs = [];
            this.items = {};
            this.CommandExecuted = new ASPxClientEvent();
            this.ActiveTabChanged = new ASPxClientEvent();
            this.MinimizationStateChanged = new ASPxClientEvent();
            this.FileTabClicked = new ASPxClientEvent();
            this.DialogBoxLauncherClicked = new ASPxClientEvent();

            this.currentDropDownItem = null;
            this.currentGroupInPopup = null;

            ASPxClientRibbonCollection.register(this);

            this.adjustLocked = false;

            this.minimized = false;
            this.showFileTab = true;
            this.allowMinimize = true;
            this.showTabs = true;
            this.oneLineMode = false;

            this.tabContentCollapsed = false;
            this.lastTabContentWidth = "";

            this.tabControl = null;
        },
        SetEnabled: function(enabled) {
            if(!this.enabled) return;
            this.setEnabledCore(enabled, false);
        },
        GetTab: function(index) {
            return this.tabs[index] || null;
        },
        GetTabByName: function(name) {
            for(var tab, i = 0; tab = this.tabs[i]; i++) {
                if(tab.name == name)
                    return tab;
            }
            return null;
        },
        GetTabCount: function() {
            return this.tabs.length;
        },
        GetActiveTab: function() {
            return this.activeTab;
        },
        SetActiveTab: function(tab) {
            this.setActiveTabControlTab(tab.index);
        },
        SetActiveTabIndex: function(index) {
            this.setActiveTabControlTab(index);
        },
        GetItemByName: function(name) {
            var items = this.items;
            var subItem = null;
            for(var itemID in items) {
                if(!items.hasOwnProperty(itemID)) continue;
                if(items[itemID].name == name)
                    return items[itemID];
                if(items[itemID].getItemByName && !subItem)
                    subItem = items[itemID].getItemByName(name);
            }
            return subItem;
        },
        GetItemValueByName: function(name) {
            var item = this.GetItemByName(name);
            if(item)
                return item.GetValue();
        },
        SetItemValueByName: function(name, value) {
            var item = this.GetItemByName(name);
            if(item)
                item.SetValue(value);
        },
        SetMinimized: function(minimized) {
            this.setMininizedCore(minimized, true);
        },
        GetMinimized: function() {
            return this.minimized;
        },
        SetContextTabCategoryVisible: function(categoryName, visible) {
            for(var tab, i = 0; tab = this.tabs[i]; i++) {
                if(tab.isContext && tab.categoryName == categoryName)
                    tab.setVisible(visible);
            }
        },

        InlineInitialize: function() {
            ASPxClientControl.prototype.InlineInitialize.call(this);
            var mainElement = this.GetMainElement(),
                controlWidth = mainElement.offsetWidth;
            this.setActiveTabContainer(this.activeTabIndex);
            this.setEnabledCore(this.enabled && this.clientEnabled, true);
            if(this.showTabs && !this.minimized)
                this.setActiveTabControlTab(this.activeTabIndex);
            if(this.minimized) {
                this.switchInactiveTab(false);
                ASPx.GetStateController().SelectElementBySrcElement(this.getMinimizeButton());
            }
            var minimizeButton = this.getMinimizeButton();
            if(minimizeButton)
                ASPx.Evt.AttachEventToElement(minimizeButton, "click", this.onMinimizeButtonClick.aspxBind(this));
            if(this.widthValueSetInPercentage)
                this.initWidth = mainElement.style.width;
            if(controlWidth > 0) {
                var innerWidth = controlWidth - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(mainElement);
                ASPx.SetOffsetWidth(this.activeTab.getTabContentElement(), innerWidth);
            }
            this.collapseTabContent();
        },

        Initialize: function() {
            this.constructor.prototype.Initialize.call(this);
            this.activeTab.initialize();
        },
        AfterInitialize: function() {
            this.constructor.prototype.AfterInitialize.call(this);
            this.updatePercentageWidthCore();
            setTimeout(function () { this.activeTab.applyModification(); }.aspxBind(this), 100);            
        },
        AdjustControlCore: function() {
            this.updateLayout(true);
            if(this.showTabs)
                this.getTabControl().AdjustControl(true);
        },
        OnBrowserWindowResize: function(evt) {
            if(this.minimized)
                this.hideTabMinimizePopup();
            this.updateLayout();
        },
        NeedCollapseControlCore: function() {
            return !this.minimized;
        },
        CollapseControl: function() {
            this.isControlCollapsed = true;
            this.collapseTabContent();
            if(this.showTabs)
                this.getTabControl().CollapseControl();
        },
        ExpandControl: function() {
            this.isControlCollapsed = false;

            if(!this.tabContentCollapsed) {
                var tabContent = this.activeTab.getTabContentElement(),
                    tabWrapper = this.activeTab.getWrapperElement();

                tabContent.style.width = this.lastTabContentWidth;
                tabWrapper.style.position = "static";
                tabContent.style.position = "static";
            }

            if(this.showTabs)
                this.getTabControl().ExpandControl();
        },
        collapseTabContent: function() {
            if(!this.tabContentCollapsed) {
                var tabContent = this.activeTab.getTabContentElement(),
                    tabWrapper = this.activeTab.getWrapperElement();

                this.lastTabContentWidth = tabContent.style.width;

                tabContent.style.position = "relative";
                tabWrapper.style.position = "absolute";
                tabContent.style.width = "auto";

                this.tabContentCollapsed = true;
            }
        },
        SetWidth: function(width) {
            this.constructor.prototype.SetWidth.call(this, width);
            this.updateLayout(false, true);
            if(this.showTabs)
                this.getTabControl().AdjustControl(true);
        },
        updateLayout: function(adjustItems, forceUpdating) {
            if(forceUpdating || !this.adjustLocked) {
                if(forceUpdating || this.widthValueSetInPercentage)
                    this.updatePercentageWidth();
                this.hideAllPopups(true, true);
                if(forceUpdating || !this.widthValueSetInPercentage)
                    this.activeTab.applyModification();
                if(adjustItems)
                    this.activeTab.adjustItems();
            }
        },
        updatePercentageWidth: function() {
            if(this.resizingTimer === undefined) {
                this.collapseTabContent();
                this.resizingTimer = setTimeout(function() { this.updatePercentageWidthCore() }.aspxBind(this), 100);
            }
        },
        updatePercentageWidthCore: function() {
            var tabContent = this.activeTab.getTabContentElement();
            var tabWrapper = this.activeTab.getWrapperElement();
            if(this.showTabs)
                this.getTabControl().CollapseControl();
            if(tabContent.offsetWidth > 0)
                ASPx.SetOffsetWidth(tabContent, tabContent.offsetWidth);
            tabWrapper.style.position = "static";
            tabContent.style.position = "static";
            this.tabContentCollapsed = false;
            this.activeTab.applyModification();
            if (this.showTabs)                
                this.getTabControl().AdjustControl();            
            this.resizingTimer = undefined;
        },
        applyModifications: function() {
            if(!this.modificationsTimer) {
                this.modificationsTimer = window.setTimeout(function() {
                    this.activeTab.applyModification();
                    this.modificationsTimer = undefined;
                }.aspxBind(this), 200);
            }
        },

        loadItems: function(tabsInfo) {
            for(var i = 0, tabInfo; tabInfo = tabsInfo[i]; i++) {
                var tab = new ASPxClientRibbonTab(this, i, tabInfo);
                this.tabs.push(tab);
                var tabServerVisible = !!tab.getTabContentElement();
                if(!tabServerVisible)
                    continue;
                for(var j = 0, groupInfo; groupInfo = tabInfo.g[j]; j++) {
                    var group = new ASPxClientRibbonGroup(this, tab, j, groupInfo.n);
                    tab.groups.push(group);
                    if(!group.GetVisible())
                        continue;
                    for(var k = 0, itemInfo; itemInfo = groupInfo.i[k]; k++) {
                        var item = ASPxClientRibbonItem.create(this, group, k, itemInfo.c, itemInfo.t, itemInfo.sg, itemInfo);
                        group.items.push(item);
                        this.items[getItemID(item, true)] = item;
                    }
                }
            }
        },

        hideAllPopups: function(lockAdjust, skipTempTab) {
            lockAdjust = lockAdjust === undefined ? true : lockAdjust;
            if(lockAdjust)
                this.adjustLocked = true;
            this.hideGroupCollapsePopup(false, false);
            if(this.lastShownPopup && this.lastShownPopup.IsVisible())
                this.lastShownPopup.Hide();
            if(lockAdjust)
                this.adjustLocked = false;
            if(!skipTempTab)
                this.hideTabMinimizePopup();
        },

        /* Outer controls event handlers */
        onTabChanged: function(tcTab) {
            if(tcTab.name == constants.INACTIVETAB_NAME) return;
            var actTabIndex = this.activeTab.index;
            var newTabIndex = this.getRibbonTabIndex(tcTab.index);
            this.setActiveTabContainer(newTabIndex, actTabIndex != newTabIndex);
            if(this.widthValueSetInPercentage)
                this.updatePercentageWidth();
        },
        onTabClick: function(tcTabIndex, evt) {
            if(evt.tab.name == constants.FILETAB_NAME) {
                evt.cancel = true;
                this.raiseEvent("FileTabClicked", new ASPxClientEventArgs());
            }
            var tabIndex = this.getRibbonTabIndex(tcTabIndex);
            if(this.minimized && tabIndex > -1){
                evt.cancel = true;
                if (!evt.htmlElement.HoverStateItem)
                    ASPx.AddClassNameToElement(evt.htmlElement, constants.CLASSNAMES.TAB_HOVER);
            }
        },
        onTabElementMouseDown: function(tab) {
            if(this.minimized && tab.GetEnabled()) {
                setTimeout(function() {
                    if(this.currentTemporaryTabPageIndex == tab.index)
                        this.hideTabMinimizePopup();
                    else
                        this.showTabMinimizePopup(tab.index);
                }.aspxBind(this), 0);
            }
        },
        onTabElementDblClick: function(tab) {
            if(!this.GetEnabled()) return;
            this.setMininizedCore(!this.minimized);
            if(this.minimized)
                ASPx.GetStateController().SetCurrentHoverElementBySrcElement(this.getTabElement(tab.index, false));
        },
        onEditorValueChanged: function(itemID) {
            this.items[itemID].onValueChanged();
        },
        onPopupMenuItemClick: function(itemID, menuItem) {
            this.items[itemID].onMenuItemClick(menuItem);
        },
        onPopupMenuPopUp: function(itemID, popup) {
            this.onItemPopupPopUp(itemID, popup);
        },
        onPopupMenuCloseUp: function(itemID, popup) {
            this.onItemPopupCloseUp(itemID, popup);
        },
        onItemPopupPopUp: function(itemID, popup) {
            this.currentDropDownItem = this.items[itemID];
            this.lastShownPopup = popup;
        },
        onItemPopupCloseUp: function(itemID, popup) {
            var element = this.currentDropDownItem.getElement();
            if(!!this.currentDropDownItem.hoverItem) {
                this.currentDropDownItem.hoverItem.Cancel(element);
                this.currentDropDownItem.hoverItem = null;
            }
            this.currentDropDownItem = null;
            setTimeout(function() {
                if(this.lastShownPopup == popup)
                    this.lastShownPopup = undefined;
            }.aspxBind(this), 100);
        },
        onMinimizeButtonClick: function() {
            if(this.GetEnabled())
                this.setMininizedCore(!this.minimized);
        },
        onColorButtonPopupPopUp: function(itemID, popup) {
            this.items[itemID].onPopupPopUp();
            this.onItemPopupPopUp(itemID, popup);
        },
        onGalleryPopupPopUp: function(itemID, popup) {
            this.items[itemID].onPopupPopUp();
            this.onItemPopupPopUp(itemID, popup);
        },
        onGalleryPopupCloseUp: function(itemID, popup) {
            this.onItemPopupCloseUp(itemID, popup);
        },
        onColorTableColorChanged: function(itemID) {
            this.items[itemID].onColorChanged();
        },
        onCNCCustomColorTableUpdated: function(itemID) {
            this.items[itemID].updateClientState();
        },
        onCNCShouldBeClosed: function(itemID) {
            this.items[itemID].onCNCShouldBeClosed();
        },
        onMouseDown: function(evt) {
            if(!this.enabled) return;
            var source = ASPx.Evt.GetEventSource(evt);
            if(!ASPx.GetIsParent(this.GetMainElement(), source))
                this.hideAllPopups();
            else if(this.currentTemporaryTabPageIndex > -1 && !ASPx.GetIsParent(this.activeTab.getTabContentElement(), source)) {
                if((!this.currentGroupInPopup || this.currentGroupInPopup.tab != this.activeTab) && (!this.currentDropDownItem || this.currentDropDownItem.group.tab != this.activeTab))
                    this.hideTabMinimizePopup();
            }
            if(this.currentGroupInPopup && !ASPx.GetIsParent(this.getGroupCollapsePopup().GetContentContainer(-1), source)) {
                var isExpandButtonClick = ASPx.GetIsParent(this.currentGroupInPopup.getExpandButtonElement(), source);
                var isInGroupPopupItemClick = this.currentDropDownItem && ASPx.GetIsParent(this.currentDropDownItem.getPopupElement(), source);
                if(!isExpandButtonClick && !isInGroupPopupItemClick)
                    this.hideGroupCollapsePopup();
            }
        },

        onExecCommand: function(item, parameter) {
            if(!item.getEnabledCore()) return;
            this.hideGroupCollapsePopup();
            this.hideTabMinimizePopup();
            var processOnServer = this.IsServerEventAssigned("CommandExecuted");
            var args = new ASPxClientRibbonCommandExecutedEventArgs(item, parameter, processOnServer);
            this.raiseEvent("CommandExecuted", args);
            if(args.processOnServer)
                this.SendPostBack("COMMANDEXECUTED" + ASPx.CallbackSeparator + item.getFullIndexPath() + ASPx.CallbackSeparator + parameter);
        },

        raiseEvent: function(eventName, args) {
            var evt = this[eventName];
            if(!evt.IsEmpty())
                evt.FireEvent(this, args);
            return args;
        },

        setEnabledCore: function(enabled, initialization) {
            if(!initialization)
                this.constructor.prototype.SetEnabled.call(this, enabled);
            var clientEnabled = this.clientEnabled;
            for(var itemID in this.items) {
                if(!this.items.hasOwnProperty(itemID)) continue;
                var item = this.items[itemID];
                item.setEnabledCore(clientEnabled, initialization);
            }
            for(var group, i = 0; group = this.activeTab.groups[i]; i++) {
                var eb = group.getExpandButtonElement();
                if(eb)
                    ASPx.GetStateController().SetElementEnabled(eb, enabled);
                var dbl = group.getDialogBoxLauncherElement();
                if(dbl)
                    ASPx.GetStateController().SetElementEnabled(dbl, clientEnabled);
            }
            var minimizeButton = this.getMinimizeButton();
            if(minimizeButton)
                ASPx.GetStateController().SetElementEnabled(minimizeButton, clientEnabled);
        },

        setMininizedCore: function(minimized, skipEvent) {
            if(!this.allowMinimize || this.minimized == minimized) return;
            this.hideTabMinimizePopup();
            this.minimized = minimized;

            this.adjustLocked = true;
            if(this.minimized) {
                this.activeTab.hideContainer();
                this.switchInactiveTab(false);
                ASPx.GetStateController().SelectElementBySrcElement(this.getMinimizeButton());
            }
            else {
                this.activeTab.showContainer();
                this.switchInactiveTab(true);
                ASPx.GetStateController().DeselectElementBySrcElement(this.getMinimizeButton());
            }
            this.adjustLocked = false;
            this.AdjustControlCore();
            if(!skipEvent)
                this.raiseEvent("MinimizationStateChanged", new ASPxClientRibbonMinimizationStateEventArgs(minimized ? ASPxClientRibbonState.Minimized : ASPxClientRibbonState.Normal));
        },
        showTabMinimizePopup: function(index) {
            if(!this.minimized) return;
            if(this.lastShownTemporaryTabPageIndex == index) return;

            this.currentTemporaryTabPageIndex = index;
            this.lastShownTemporaryTabPageIndex = index;

            var pc = this.getTabMinimizePopup();
            var cc = pc.GetContentContainer(-1);
            var tab = this.tabs[index];

            if(cc.childNodes.length)
                cc.innerHTML = "";

            if(!tab.initialized)
                tab.initialize(true);
            
            var tcMainElement = this.getTabControl().GetMainElement();
            var tabContainer = tab.getTabContentElement();
            cc.appendChild(tabContainer);

            var tcMainElementOffsetWidth = tcMainElement.offsetWidth;
            pc.SetHeight(0);
            pc.SetWidth(tcMainElementOffsetWidth);
            ASPx.SetOffsetWidth(tabContainer, tcMainElementOffsetWidth);

            tab.showContainer();
            if(this.widthValueSetInPercentage)
                this.updatePercentageWidth();
            pc.ShowAtElement(tcMainElement);
            var tabChanged = this.activeTab !== tab;
            this.activeTab = tab;
            this.raiseEvent("MinimizationStateChanged", new ASPxClientRibbonMinimizationStateEventArgs(ASPxClientRibbonState.TemporaryShown));
            if(tabChanged)
                this.raiseEvent("ActiveTabChanged", new ASPxClientRibbonTabEventArgs(this.activeTab));
        },

        hideTabMinimizePopup: function() {
            if(this.getTabMinimizePopup().IsVisible() || this.activeTab.getTabContentElement().parentNode != this.getTabsContainer())
                this.hideTabMinimizePopupCore();
        },
        hideTabMinimizePopupCore: function() {
            this.getTabMinimizePopup().Hide();
            this.activeTab.hideContainer();
            this.getTabsContainer().appendChild(this.activeTab.getTabContentElement());
            if(this.minimized)
                this.switchInactiveTab(false);

            var tabIndex = this.currentTemporaryTabPageIndex;
            if(tabIndex > -1) {
                var tabElement = this.getTabElement(tabIndex);
                if(tabElement.hoverItem) {
                    tabElement.hoverItem.Cancel(tabElement);
                    tabElement.hoverItem = null;
                } else {
                    ASPx.RemoveClassNameFromElement(tabElement, constants.CLASSNAMES.TAB_HOVER);
                }
                this.currentTemporaryTabPageIndex = -1;
                setTimeout(function() {
                    if(this.lastShownTemporaryTabPageIndex == tabIndex)
                        this.lastShownTemporaryTabPageIndex = -1;
                }.aspxBind(this), 100);
            }

            this.raiseEvent("MinimizationStateChanged", new ASPxClientRibbonMinimizationStateEventArgs(ASPxClientRibbonState.Minimized));
        },

        setActiveTabContainer: function(index, raiseEvent) {
            if(index >= this.GetTabCount()) {
                this.setActiveTabContainer(this.GetTabCount() - 1, raiseEvent);
                index = this.GetTabCount() - 1;
            }
            if(index < 0)
                index = 0;

            var tab = this.tabs[index];

            if(tab.isContext && !tab.visible) {
                index = 0;
                tab = this.tabs[index];
            }

            if(!this.minimized) {
                if(this.activeTab)
                    this.activeTab.hideContainer();
                if(!this.minimized)
                    tab.showContainer();
            }
            this.activeTab = tab;
            this.activeTabIndex = index;
            this.updateClientStateField("ActiveTabIndex", this.activeTabIndex);
            window.setTimeout(function() { ASPx.GetControlCollection().AdjustControlsCore(tab.getWrapperElement(), true); }, 0);            
            if(raiseEvent)
                this.raiseEvent("ActiveTabChanged", new ASPxClientRibbonTabEventArgs(this.activeTab));
        },
        showGroupCollapsePopup: function(group) {
            this.adjustLocked = true;
            if(this.currentGroupInPopup) {
                if(this.currentGroupInPopup != group)
                    this.hideGroupCollapsePopup();
                else {
                    this.adjustLocked = false;
                    return;
                }
            }
            var pc = this.getGroupCollapsePopup();
            var cc = pc.GetContentContainer(-1);

            if(cc.childNodes.length > 0)
                cc.innerHTML = "";

            if(this.oneLineMode) {
                var groupSubElements = group.getOneLineModeSubElements(true);
                this.activeTab.rollbackOneLineModeGroupModifications(group);

                for(var se, i = 0; se = groupSubElements[i]; i++) {
                    se.parent = se.parentNode;
                    cc.appendChild(se);
                }
            }
            else {
                var groupSubElements = group.getSubElements();
                for(var se, i = 0; se = groupSubElements[i]; i++) {
                    se.parent = se.parentNode;
                    cc.appendChild(se);
                    se.style.width = (group.initWidth + 1) + "px";
                }

                this.activeTab.rollbackGroupModifications(group);
            }

            setTimeout(function() {
                pc.SetWidth(1);
                pc.SetHeight(1);
                pc.ShowAtElement(group.getExpandButtonElement());
            }.aspxBind(this), 0);
            this.currentGroupInPopup = group;

            this.adjustLocked = false;
        },
        hideGroupCollapsePopup: function(skipHide, lockAdjust) {
            lockAdjust = lockAdjust === undefined ? true : lockAdjust;
            if(lockAdjust)
                this.adjustLocked = true;
            var pc = this.getGroupCollapsePopup();
            if(!pc) 
                return;
            if(this.enabled && !skipHide)
                pc.Hide();

            if(!this.currentGroupInPopup) {
                if(lockAdjust)
                    this.adjustLocked = false;
                return;
            }

            var element = this.currentGroupInPopup.getExpandButtonElement();
            if(element.hoverItem) {
                element.hoverItem.Cancel(element);
                element.hoverItem = null;
            }

            if(this.oneLineMode)
                this.activeTab.restoreOneLineModeGroupModifications();
            else
                this.activeTab.restoreGroupModifications();

            var groupSubElements = this.oneLineMode ? this.currentGroupInPopup.getOneLineModeSubElements() : this.currentGroupInPopup.getSubElements();
            for(var i = 0, se; se = groupSubElements[i]; i++) {
                se.style.width = "";
                var isBlock = ASPx.ElementContainsCssClass(se, constants.CLASSNAMES.BLOCK);

                if(this.oneLineMode && isBlock)
                    se.parent.insertBefore(se, this.currentGroupInPopup.getExpandButtonElement());
                else
                    se.parent.appendChild(se);
            }
            if(lockAdjust)
                this.adjustLocked = false;
            this.currentGroupInPopup = undefined;
        },
        switchInactiveTab: function(activate) {
            this.setActiveTabControlTab(activate ? this.activeTab.index : -1);
        },
        getTabControl: function() {
            if(!this.tabControl)
                this.tabControl = ASPx.GetControlCollection().Get(this.name + constants.TABCONTROL_POSTFIX);
            return this.tabControl;
        },
        getGroupCollapsePopup: function() {
            return ASPx.GetControlCollection().Get(this.name + constants.GROUPCOLLAPSEPOPUP_POSTFIX);
        },
        getTabMinimizePopup: function() {
            return ASPx.GetControlCollection().Get(this.name + constants.TABMINIMIZEPOPUP_POSTFIX);
        },
        getTabsContainer: function() {
            return document.getElementById(this.name + constants.TABSCONTAINER_POSTFIX);
        },
        getTabElement: function(index, active) {
            return this.getTabControl().GetTabElement(this.getTabControlTabIndex(index), active);
        },
        getMinimizeButton: function() {
            return document.getElementById(this.name + constants.TABCONTROL_POSTFIX + constants.MINIMIZEBUTTON_POSTFIX);
        },

        updateClientStateField: function(key, value) {
            this.stateObject[key] = value;
            this.updateCookie();
        },
        clearClientStateField: function(key) {
            delete this.stateObject[key];
            this.updateCookie();
        },
        updateCookie: function() {
            if(this.cookieName && this.cookieName != "") {
                ASPx.Cookie.DelCookie(this.cookieName);
                ASPx.Cookie.SetCookie(this.cookieName, ASPx.Json.ToJson(this.stateObject));
            }
        },

        /*Tab control */
        getTabControlTab: function(index) {
            var tcTabIndex = this.getTabControlTabIndex(index);
            return this.getTabControl().GetTab(tcTabIndex);
        },
        setActiveTabControlTab: function(index) {
            if(this.showTabs) {
                if(this.minimized && index > -1) {
                    var tc = this.getTabControl();
                    if(this.currentTemporaryTabPageIndex == index) {
                        this.hideTabMinimizePopup();
                        tc.scrollToActiveTab = false;
                        tc.SetActiveTab(tc.GetTabByName(constants.INACTIVETAB_NAME));
                    }
                    else {
                        this.showTabMinimizePopup(index);
                        tc.scrollToActiveTab = true;
                        tc.SetActiveTabIndex(this.getTabControlTabIndex(index));
                    }
                }
                else {
                    var tc = this.getTabControl();
                    if (index === -1) {
                        tc.scrollToActiveTab = false;
                        tc.SetActiveTab(tc.GetTabByName(constants.INACTIVETAB_NAME));
                    }
                    else {
                        tc.scrollToActiveTab = true;
                        tc.SetActiveTabIndex(this.getTabControlTabIndex(index));
                    }    
                }
            }
            else if(index >= 0)
                this.setActiveTabContainer(index, index != this.activeTab.index);
        },
        getActiveTabControlTab: function() {
            return this.getTabControl().GetActiveTab();
        },
        getTabControlTabIndex: function(rTabIndex) {
            return this.showFileTab ? (rTabIndex + 1) : rTabIndex;
        },
        getRibbonTabIndex: function(tcTabIndex) {
            return this.showFileTab ? (tcTabIndex - 1) : tcTabIndex;
        }
    });
    ASPxClientRibbon.Cast = ASPxClientControl.Cast;
    var ASPxClientRibbonTab = ASPx.CreateClass(null, {
        constructor: function(ribbon, index, tabInfo) {
            this.ribbon = ribbon;
            this.index = index;
            this.name = tabInfo.n || "";
            this.visible = tabInfo.v ? true : false;
            this.groups = [];

            this.isContext = tabInfo.c || false;
            this.categoryName = tabInfo.cn || "";

            this.tabElement = null;
            this.listElement = null;

            this.listWidth = -1;
            this.width = -1;
            this.height = -1;

            this.initialized = false;

            this.modifications = [];
            this.activeModificationIndex = -1;

            this.initializeHandlers();

            this.initializeSpacers();
        },
        GetText: function() {
            return this.ribbon.showTabs ? this.getTabControlTab().GetText() : "";
        },
        SetEnabled: function(enabled) {
            if(this.ribbon.showTabs)
                this.getTabControlTab().SetEnabled(enabled);
        },
        GetEnabled: function() {
            if(this.ribbon.showTabs)
                return this.getTabControlTab().GetEnabled();
            return true;
        },
        GetVisible: function() {
            if(this.isContext)
                return this.visible;
            else
                return !!this.getTabContentElement();
        },

        initialize: function(forced) {
            if(this.initialized) return;
            var tabContentElement = this.getTabContentElement();
            var parent = tabContentElement.parentNode;
            if(tabContentElement.offsetWidth == 0 && forced) {
                ASPx.Attr.ChangeStyleAttribute(tabContentElement, "position", "absolute");
                ASPx.Attr.ChangeStyleAttribute(tabContentElement, "top", "-10000px");
                ASPx.Attr.ChangeStyleAttribute(tabContentElement, "left", "-10000px");
                ASPx.Attr.ChangeStyleAttribute(tabContentElement, "display", "block");
                this.ribbon.GetMainElement().appendChild(tabContentElement);
            }
            if(tabContentElement.offsetWidth > 0) {
                var groups = this.groups;
                for(var group, i = groups.length - 1; group = groups[i]; i--) {
                    group.initialize();
                }

                this.calculateModifications();
                this.initialized = true;
                this.applyModification();

                this.height = this.getListElement().offsetHeight;
                this.listWidth = this.getListElement().offsetWidth;
            }
            if(tabContentElement.parentNode != parent) {
                ASPx.Attr.RestoreStyleAttribute(tabContentElement, "display");
                ASPx.Attr.RestoreStyleAttribute(tabContentElement, "position");
                ASPx.Attr.RestoreStyleAttribute(tabContentElement, "top");
                ASPx.Attr.RestoreStyleAttribute(tabContentElement, "left");
                parent.appendChild(tabContentElement);
                tabContentElement.style.display = "";
            }
        },
        initializeSpacers: function () {
            if (this.isContext) {
                var tabElement = this.ribbon.getTabElement(this.index, this.ribbon.enabled);
                if (tabElement) {
                    var spacer = tabElement.nextSibling;
                    if (spacer && ASPx.ElementContainsCssClass(spacer, "dxtc-spacer"))
                        spacer.style.width = "0px";
                }
            }
        },
        initializeHandlers: function() {
            if(this.ribbon.showTabs) {
                var tabControl = this.ribbon.getTabControl();
                var tcTab = this.getTabControlTab();
                if (this.ribbon.enabled && (this.isContext || this.GetVisible())) {
                    ASPx.Evt.AttachEventToElement(tabControl.GetTabElement(tcTab.index, true), 'dblclick', this.onTabElementDblClick.aspxBind(this));
                    ASPx.Evt.AttachEventToElement(tabControl.GetTabElement(tcTab.index, false), 'dblclick', this.onTabElementDblClick.aspxBind(this));

                    ASPx.Evt.AttachEventToElement(tabControl.GetTabElement(tcTab.index, true), 'mousedown', this.onTabElementMouseDown.aspxBind(this));
                    ASPx.Evt.AttachEventToElement(tabControl.GetTabElement(tcTab.index, false), 'mousedown', this.onTabElementMouseDown.aspxBind(this));
                }
            }
        },
        deleteModifications: function() {
            for(var i = 0, group; group = this.groups[i]; i++) {
                if(!group.GetVisible())
                    continue;
                group.getElement().className = constants.CLASSNAMES.GROUP;
                var blocks = ASPx.GetNodesByClassName(group.getElement(), constants.CLASSNAMES.BLOCK);
                for(var j = 0, block; block = blocks[j]; j++) {
                    block.className = block.originalClassName;
                }
                for (var j = 0, item; item = group.items[j]; j++)
                    if (item.setBaseWidth)
                        item.setBaseWidth();
            }
            this.modifications = [];
        },
        recalculateModifications: function() {
            this.calculateModifications();
            this.activeModificationIndex = -1;
            if (!this.ribbon.minimized)
                this.listWidth = -1;
            this.applyModification();
        },
        
        calculateModifications: function() {
            var groupInfos = [],
                blockInfos;

            for(var i = 0, group; group = this.groups[i]; i++) {
                if(!group.GetVisible())
                    continue;
                blockInfos = group.getBlocksInfos(i);
                groupInfos.push({
                    group: group,
                    width: group.getElement().offsetWidth,
                    collapseWidth: getCollapsedGroupWidth(group),
                    blockInfos: blockInfos,
                    expandButtonWidth: getGroupExpandButtonWidth(group)
                });
            }

            while(true) {
                var mod = findNextModification(groupInfos);
                if(mod.indexPath[0] == -1) break;
                var groupInfo = groupInfos[mod.indexPath[0]];

                if(this.ribbon.oneLineMode)
                    this.pushOneLineModification(mod, groupInfo);
                else
                    this.pushModification(mod, groupInfo);
            }

            for(var i = 0, groupInfo; groupInfo = groupInfos[i]; i++) {
                for(var j = 0, blockInfo; blockInfo = groupInfo.blockInfos[j]; j++)
                    blockInfo.activeMod = 0;
            }
        },

        pushModification: function(mod, groupInfo) {
            if(mod.indexPath[1] > -1) {
                var blockInfo = groupInfo.blockInfos[mod.indexPath[1]];
                var newClassName = blockInfo.mods[blockInfo.activeMod + 1].className;
                var prevClassName = blockInfo.mods[blockInfo.activeMod].className;

                this.modifications.push(new Modification(mod.delta, blockInfo.block, newClassName, prevClassName, groupInfo.group, true));
                blockInfo.activeMod++;
                groupInfo.width -= blockInfo.mods[blockInfo.activeMod].width;
            }
            else {
                this.modifications.push(new Modification(mod.delta, groupInfo.group.getElement(), constants.CLASSNAMES.GROUP_COLLAPSED, constants.CLASSNAMES.GROUP, groupInfo.group, false));
                groupInfo.width = -1;
            }
        },

        pushOneLineModification: function(mod, groupInfo) {
            if(mod.indexPath[1] > -1) {
                var blockInfo = groupInfo.blockInfos[mod.indexPath[1]];
                var newClassName = blockInfo.mods[blockInfo.activeMod + 1].className;
                var prevClassName = blockInfo.mods[blockInfo.activeMod].className;

                var needToCollapse = groupInfo.width > groupInfo.expandButtonWidth;
                var needToShowExpandBtn = needToCollapse && newClassName.indexOf(constants.CLASSNAMES.BLOCK_HORIZONTAL_ITEMS_HIDE) != -1 && !groupInfo.expandBtnMod;

                if(needToShowExpandBtn) {
                    this.modifications.push(new Modification(-groupInfo.expandButtonWidth, groupInfo.group.getExpandButtonElement(), constants.CLASSNAMES.ONE_LINE_MODE_GROUP_EXPAND_VISIBLE, constants.CLASSNAMES.ONE_LINE_MODE_GROUP_EXPAND, groupInfo.group, false));
                    groupInfo.expandBtnMod = true;
                    groupInfo.width += groupInfo.expandButtonWidth;
                }
                
                if(needToCollapse)
                    this.modifications.push(new Modification(mod.delta, blockInfo.block, newClassName, prevClassName, groupInfo.group, true));
                blockInfo.activeMod++;
                groupInfo.width -= blockInfo.mods[blockInfo.activeMod].width;
            }
            else {
                groupInfo.width = -1;
            }
        },

        applyModification: function() {
            if(!this.initialized)
                this.initialize();
            else if(!this.rollbackGroup) {
                var newWidth = this.ribbon.widthValueSetInPercentage ? ASPx.PxToInt(this.getTabContentElement().style.width) : this.getTabContentElement().offsetWidth;
                if(newWidth == 0)
                    return;
                
                this.listWidth = this.getListElement().offsetWidth;

                var listWidth = this.listWidth;
                if(listWidth == 0) return;

                var deltaWidth = newWidth - listWidth;
                if(deltaWidth == 0) return;

                var modificator = deltaWidth < 0 ? 1 : -1;
                while(deltaWidth != 0) {
                    var newIndex = deltaWidth < 0 ? (this.activeModificationIndex + modificator) : this.activeModificationIndex;
                    var modification = this.modifications[newIndex];
                    if(!modification)
                        return;
                    var modDelta = modification.delta;
                    if(deltaWidth > 0 && deltaWidth - modDelta < 0)
                        return;
                    this.applyModificationCore(modification, modificator);
                    if(deltaWidth < 0 && deltaWidth - modDelta > 0 && modDelta > 0)
                        return;
                    if(modificator > 0)
                        deltaWidth += modDelta;
                    else
                        deltaWidth -= modDelta;
                }
            }
        },
        applyModificationCore: function(modification, modificator) {
            var modifyItemAttributes = function() {
                if(modification.newClassName == constants.CLASSNAMES.BLOCK_HORIZONTAL_ITEMS) {
                    forEachItemInModificationBlock(function(item) {
                        if(!item.navigateUrl || !(item instanceof ASPxClientRibbonDropDownMenuItem))
                            return;
                        var itemLabelTextElement = item.getLabelTextElement();
                        modificator > 0 ? ASPx.Attr.SetAttribute(itemLabelTextElement, "href", item.navigateUrl) : ASPx.Attr.RemoveAttribute(itemLabelTextElement, "href");
                    });
                }

                if(modification.newClassName == constants.CLASSNAMES.BLOCK_HORIZONTAL_ITEMS_REDUCED) {
                    forEachItemInModificationBlock(function(item) {
                        var itemElement = item.getElement();
                        if(modificator > 0) {
                            if(!ASPx.Attr.IsExistsAttribute(itemElement, "title") && item.text) {
                                ASPx.Attr.SetAttribute(itemElement, "title", item.text);
                                item.isTextTitleSet = true;
                            }
                        } else {
                            if(item.isTextTitleSet) {
                                ASPx.Attr.RemoveAttribute(itemElement, "title");
                                item.isTextTitleSet = undefined;
                            }
                        }
                    });
                }
            }
            var forEachItemInModificationBlock = function(func) {
                for(var item, i = 0; item = modification.group.items[i]; i++) {
                    if(!item.GetVisible() || item.getElement().parentNode != modification.element)
                        continue;
                    func(item);
                }
            }
            var updateItems = function() {
                forEachItemInModificationBlock(function(item) {
                    if(item.onAfterModification)
                        item.onAfterModification();
                });
            }

            if(modificator > 0) {
                modification.element.className = modification.element.className.replace(modification.prevClassName, modification.newClassName);
            }
            else {
                modification.element.className = modification.element.className.replace(modification.newClassName, modification.prevClassName);
            }

            modifyItemAttributes();
            updateItems();
            this.activeModificationIndex = this.activeModificationIndex + modificator;
            this.listWidth = this.getListElement().offsetWidth;
        },
        rollbackOneLineModeGroupModifications: function(group) {
            var containsItem = function(modification) {
                for(var i = 0, element; element = modification.group.subElements[i]; i++)
                    if(element == modification.element)
                        return true;
                return false;
            };
            this.rollbackGroup = group;
            for(var mod, i = this.activeModificationIndex; mod = this.modifications[i]; i--) {
                if(mod.group == group && mod.isBlock && containsItem(mod))
                    this.applyModificationCore(mod, -1);
            }
        },
        rollbackGroupModifications: function(group) {
            this.rollbackGroup = group;
            for(var mod, i = this.activeModificationIndex; mod = this.modifications[i]; i--) {
                if(mod.group == group && mod.isBlock)
                    this.applyModificationCore(mod, -1);
            }
        },
        restoreOneLineModeGroupModifications: function() {
            var containsItem = function(modification) {
                for(var i = 0, element; element = modification.group.subElements[i]; i++)
                    if(element == modification.element)
                        return true;
                return false;
            };
            if(this.rollbackGroup) {
                var group = this.rollbackGroup;
                for(var mod, i = 0; mod = this.modifications[i]; i++) {
                    var mod = this.modifications[i];
                    if(mod.group == group && mod.isBlock && containsItem(mod))
                        this.applyModificationCore(mod, 1);
                }
                this.rollbackGroup = null;
            }
        },
        restoreGroupModifications: function() {
            if(this.rollbackGroup) {
                var group = this.rollbackGroup;
                for(var i = 0; i <= this.activeModificationIndex; i++) {
                    var mod = this.modifications[i];
                    if(mod.group == group && mod.isBlock)
                        this.applyModificationCore(mod, 1);
                }
                this.rollbackGroup = null;
            }
        },
        onTabElementDblClick: function() {
            this.ribbon.onTabElementDblClick(this);
        },
        onTabElementMouseDown: function() {
            this.ribbon.onTabElementMouseDown(this);
        },

        adjustItems: function() {
            for(var i = 0, group; group = this.groups[i]; i++) {
                for(var j = 0, item; item = group.items[j]; j++) {
                    item.adjust();
                }
            }
        },

        getTabControlTab: function() {
            return this.ribbon.getTabControlTab(this.index);
        },
        getTabContentElement: function() {
            if(!this.tabElement)
                this.tabElement = document.getElementById(this.ribbon.name + constants.TAB_CONTENTCONTROL_POSTFIX + this.index);
            return this.tabElement;
        },
        getWrapperElement: function() {
            if(this.wrapperElement === undefined)
                this.wrapperElement = ASPx.GetNodeByClassName(this.getTabContentElement(), constants.CLASSNAMES.TABWRAPPER);
            return this.wrapperElement;
        },
        getListElement: function() {
            if(!this.listElement)
                this.listElement = ASPx.GetNodeByClassName(this.getTabContentElement(), constants.CLASSNAMES.GROUPLIST);
            return this.listElement;
        },
        showContainer: function() {
            this.getTabContentElement().style.display = "block";
            this.initialize();
            setTimeout(function() {
                this.ribbon.AdjustControl();
            }.aspxBind(this), 0);
        },
        hideContainer: function() {
            this.getTabContentElement().style.display = "";
        },
        setVisible: function(visible) {
            if(this.visible == visible)
                return;
            if(!visible && this.ribbon.activeTab.index == this.index) {
                if(this.ribbon.minimized)
                    this.ribbon.setActiveTabContainer(0, true);
                else
                    this.ribbon.SetActiveTabIndex(0);
            }
            this.getTabControlTab().SetVisible(visible);
            this.ribbon.updateClientStateField("T" + this.index, visible);
            this.visible = visible;
        }
    });
    var ASPxClientRibbonGroup = ASPx.CreateClass(null, {
        constructor: function(ribbon, tab, index, name) {
            this.ribbon = ribbon;
            this.tab = tab;
            this.index = index;
            this.name = name;
            this.items = [];

            this.initWidth = 0;

            this.subElements = [];
        },
        GetVisible: function() {
            return !!this.getElement();
        },

        initialize: function() {
            if(!this.getElement())
                return;
            var expandButton = this.getExpandButtonElement();
            if(expandButton)
                ASPx.Evt.AttachEventToElement(expandButton, "mousedown", this.onExpandButtonClick.aspxBind(this));
            
            var dialogBoxLauncher = this.getDialogBoxLauncherElement();
            if(dialogBoxLauncher) {
                var _this = this;
                ASPx.Evt.AttachEventToElement(dialogBoxLauncher, "click", function() {
                    var processOnServer = _this.ribbon.IsServerEventAssigned("DialogBoxLauncherClicked");
                    var args = new ASPxClientRibbonDialogBoxLauncherClickedEventArgs(_this, processOnServer);
                    _this.ribbon.raiseEvent("DialogBoxLauncherClicked", args);
                    if(args.processOnServer)
                        _this.ribbon.SendPostBack("DIALOGBOXLAUNCHERCLICKED" + ASPx.CallbackSeparator + _this.getIndexPath());
                });
            }

            var items = this.items;
            for(var item, i = 0; item = items[i]; i++)
                item.initialize();
        },
        onExpandButtonClick: function() {
            if(this.ribbon.GetEnabled()) {
                if(this.ribbon.currentGroupInPopup == this) {
                    setTimeout(function() {
                        this.ribbon.hideGroupCollapsePopup();
                    }.aspxBind(this), 0);
                }
                else {
                    setTimeout(function() {
                        this.ribbon.showGroupCollapsePopup(this);
                    }.aspxBind(this), 0);
                }
            }
        },
        getElement: function() {
            if(!this.element)
                this.element = document.getElementById(this.ribbon.name + constants.TAB_CONTENTCONTROL_POSTFIX +
                    this.tab.index + constants.GROUP_POSTFIX + this.index);
            return this.element;
        },
        getSubElements: function() {
            if(this.subElements.length == 0) {
                var element = this.getElement();
                this.subElements.push(ASPx.GetNodeByClassName(element, constants.CLASSNAMES.GROUP_CONTENT));
                var groupLabel = ASPx.GetNodeByClassName(element, constants.CLASSNAMES.GROUP_LABEL);
                if(!!groupLabel)
                    this.subElements.push(groupLabel);
            }
            return this.subElements;
        },
        getOneLineModeSubElements: function(reGet) {
            if(reGet) {
                var element = this.getElement();
                this.subElements = ASPx.GetNodesByClassName(element, constants.CLASSNAMES.BlOCK_HIDE);
                var groupLabel = ASPx.GetNodeByClassName(element, constants.CLASSNAMES.GROUP_LABEL);
                if(!!groupLabel) {
                    this.subElements.push(ASPx.GetPreviousSibling(groupLabel));
                    this.subElements.push(groupLabel);
                }
            }
            return this.subElements;
        },
        getExpandButtonElement: function() {
            if(!this.getElement())
                return null;
            if(this.expandButtonElement === undefined)
                this.expandButtonElement = this.ribbon.oneLineMode ? ASPx.GetNodeByClassName(this.getElement(), constants.CLASSNAMES.ONE_LINE_MODE_GROUP_EXPAND)
                    : ASPx.GetNodeByClassName(this.getElement(), constants.CLASSNAMES.GROUP_EXPAND);
            return this.expandButtonElement;
        },
        getExpandButtonLabelTextElement: function() {
            if(this.expandButtonLabelTextElement === undefined)
                 this.expandButtonLabelTextElement = ASPx.GetNodeByClassName(this.getExpandButtonElement(), constants.CLASSNAMES.LABELTEXT);
            return this.expandButtonLabelTextElement;
        },
        getExpandButtonLabelContentElement: function() {
            if(this.expandButtonLabelContentElement === undefined)
                 this.expandButtonLabelContentElement = ASPx.GetNodeByClassName(this.getExpandButtonElement(), constants.CLASSNAMES.LABELCONTENT);
            return this.expandButtonLabelContentElement;
        },
        getExpandButtonPopOutElement: function() {
            if(this.expandButtonPopOutElement === undefined)
                 this.expandButtonPopOutElement = ASPx.GetNodeByClassName(this.getExpandButtonElement(), constants.CLASSNAMES.ITEMPOPOUT);
            return this.expandButtonPopOutElement;
        },
        getDialogBoxLauncherElement: function() {
            var element = this.getElement();
            if(!element)
                return null;
            return document.getElementById(element.id + constants.GROUPDIALOGBOXLAUNCHER_POSTFIX);
        },

        getBlockItems: function(block) {
            var items = [];
            for(var item, i = 0; item = this.items[i]; i++) {
                if(!item.GetVisible())
                    continue;
                if(item.getElement().parentNode == block)
                    items.push(item);
                else if(items.length > 0)
                    break;
            }
            return items;
        },
        getIndexPath: function() {
            return this.tab.index + ASPx.ItemIndexSeparator + this.index;
        },

        getRegularBlockSizeRanges: function(block) {
            var items = this.getBlockItems(block);
            var lastItemGroupName = "";
            var itemSizeInfos = [];
            var commonWidth = 0;
            var getRightBoundInfo = function(el) {
                var sibling,
                    node = el,
                    width = 0;
                while(el.nextSibling) {
                    sibling = el.nextSibling;
                    if(sibling.className && ASPx.ElementHasCssClass(sibling, constants.CLASSNAMES.ITEM))
                        break;
                    if(sibling.nodeType == 1) {
                        var style = ASPx.GetCurrentStyle(sibling);
                        width += sibling.offsetWidth + ASPx.PxToInt(style.marginLeft) + ASPx.PxToInt(style.marginRight);
                        node = sibling;
                    }
                    el = sibling;
                }
                return { width: width, node: node };
            };

            for(var item, i = 0; item = items[i]; i++) {
                var element = item.getElement();
                var style = ASPx.GetCurrentStyle(element);
                var width = element.offsetWidth + ASPx.PxToInt(style.marginLeft) + ASPx.PxToInt(style.marginRight);
                var rightBoundInfo = getRightBoundInfo(element);
                width += rightBoundInfo.width;

                if(i > 0 && item.groupName && item.groupName == items[i - 1].groupName) {
                    itemSizeInfos[itemSizeInfos.length - 1].width += width;
                    itemSizeInfos[itemSizeInfos.length - 1].node = rightBoundInfo.node;
                }
                else
                    itemSizeInfos.push({ width: width, node: rightBoundInfo.node });

                commonWidth += width;
            }
            if(this.ribbon.oneLineMode)
                return [
                    this.calculateRegularItemsBlock(1, itemSizeInfos, commonWidth)
                ]
            else
                return [
                    this.calculateRegularItemsBlock(2, itemSizeInfos, commonWidth),
                    this.calculateRegularItemsBlock(3, itemSizeInfos, commonWidth)
                ];
        },

        getBlocksInfos: function(groupIndex) {
            var blockMods = [];
            var groupElement = this.getElement();
            var blocks = ASPx.GetNodesByClassName(groupElement, constants.CLASSNAMES.BLOCK);

            for(var block, i = 0; block = blocks[i]; i++) {
                if(!block.originalClassName)
                    block.originalClassName = block.className;
                var info = this.getBlockInfo(block, groupElement, groupIndex * 100 + i + 1);
                blockMods.push(info);
            }
            this.initWidth = groupElement.offsetWidth;
            return blockMods;
        },
        getBlockInfo: function(block, groupElement, index) {
            var canReduceHorizontalBlock = function(items) {
                var itemsWidth = [];
                for(var i = 0, item; item = items[i]; i++) {
                    var element = item.getElement();
                    if(ASPx.GetNodesByClassName(element, constants.CLASSNAMES.IMAGE16).length == 0)
                        return false;
                    itemsWidth.push(element.offsetWidth);
                }
                ASPx.AddClassNameToElement(groupElement, constants.CLASSNAMES.BLOCK_HORIZONTAL_ITEMS_REDUCED);
                for(var i = 0, item; item = items[i]; i++) {
                    if(item.getElement().offsetWidth == itemsWidth[i]) {
                        ASPx.RemoveClassNameFromElement(groupElement, constants.CLASSNAMES.BLOCK_HORIZONTAL_ITEMS_REDUCED);
                        return false;
                    }
                }
                ASPx.RemoveClassNameFromElement(groupElement, constants.CLASSNAMES.BLOCK_HORIZONTAL_ITEMS_REDUCED);
                return true;
            };
            var canReduceLargeBlock = function(items) {
                for(var i = 0, item; item = items[i]; i++) {
                    var element = item.getElement();
                    if(ASPx.GetNodesByClassName(element, constants.CLASSNAMES.LABELTEXT).length == 0 && 
                        ASPx.GetNodesByClassName(element, constants.CLASSNAMES.IMAGE16).length == 0)
                        return false;
                }
                return true;
            }
            if(!groupElement)
                groupElement = this.getElement();

            var info = { block: block, activeMod: 0, mods: [], group: this };
            var initWidth = 0;
            var items = this.getBlockItems(block);

            if(ASPx.ElementHasCssClass(block, constants.CLASSNAMES.BLOCK_HORIZONTAL_ITEMS)) {
                initWidth = groupElement.offsetWidth;
                info.mods.push(new BlockMod(constants.CLASSNAMES.BLOCK_HORIZONTAL_ITEMS, 0, -1));
                if(canReduceHorizontalBlock(items))
                    info.mods.push(new BlockMod(constants.CLASSNAMES.BLOCK_HORIZONTAL_ITEMS_REDUCED, -1, 0));
                if(this.ribbon.oneLineMode) {
                    var priotiry = -1 + (index / 10000);
                    for(var i = 0, item; item = items[i]; i++) {
                        info.mods.push(new BlockMod(constants.CLASSNAMES.BLOCK_HORIZONTAL_ITEMS_HIDE, -1, priotiry));
                    }
                }
            }
            else if(ASPx.ElementHasCssClass(block, constants.CLASSNAMES.BLOCK_LARGE_ITEMS)) {
                initWidth = groupElement.offsetWidth;
                info.mods.push(new BlockMod(constants.CLASSNAMES.BLOCK_LARGE_ITEMS, 0, -1));
                if(canReduceLargeBlock(items)) {
                    if(ASPx.GetNodesByClassName(block, constants.CLASSNAMES.BUTTONITEM).length > 1) {
                        info.mods.push(new BlockMod(constants.CLASSNAMES.BLOCK_HORIZONTAL_ITEMS, -1, 1));

                        ASPx.RemoveClassNameFromElement(block, constants.CLASSNAMES.BLOCK_LARGE_ITEMS);
                        ASPx.AddClassNameToElement(block, constants.CLASSNAMES.BLOCK_HORIZONTAL_ITEMS);
                        if(canReduceHorizontalBlock(items))
                            info.mods.push(new BlockMod(constants.CLASSNAMES.BLOCK_HORIZONTAL_ITEMS_REDUCED, -1, 0));
                        ASPx.RemoveClassNameFromElement(block, constants.CLASSNAMES.BLOCK_HORIZONTAL_ITEMS);
                        ASPx.AddClassNameToElement(block, constants.CLASSNAMES.BLOCK_LARGE_ITEMS);
                    }
                }
            }
            else if(ASPx.ElementHasCssClass(block, constants.CLASSNAMES.BLOCK_REGULAR_ITEMS)) {
                var ranges = this.getRegularBlockSizeRanges(block);
                var classNames = [constants.CLASSNAMES.BLOCK_REGULAR_ITEMS, constants.CLASSNAMES.BLOCK_REGULAR_ITEMS_REDUCED];

                for(var j = 0, range; range = ranges[j]; j++) {
                    var classPostfix = ASPx.CreateImportantStyleRule(null, "width: " + ranges[j].width + "px;");
                    var className = classNames[j] + " " + classPostfix;

                    for(var k = 0; k < range.endNodes.length - 1; k++) {
                        var rowEndNode = range.endNodes[k];
                        var clearNode = document.createElement("b");
                        clearNode.className = "dx-clear dxr-regClear-" + j;
                        ASPx.InsertElementAfter(clearNode, rowEndNode);
                    }
                    if(j == 0) {
                        block.className = block.className.replace(classNames[j], className);
                        initWidth = groupElement.offsetWidth;
                        info.mods.push(new BlockMod(className, 0, 2));
                    }
                    else
                        info.mods.push(new BlockMod(className, -1, 2));
                }
            } else if(ASPx.ElementHasCssClass(block, constants.CLASSNAMES.BLOCK_SEPARATE_ITEMS)) {
                var gallery = items[0];
                if(gallery) {
                    gallery.setBaseWidth();
                    block.originClassName = block.className;
                    var elementWidth = gallery.maxElementFullWidth;
                    var blockWidth = block.offsetWidth;
                    var baseClassName = constants.CLASSNAMES.BLOCK_SEPARATE_ITEMS;
                    initWidth = groupElement.offsetWidth;
                    var classPostfix = ASPx.CreateImportantStyleRule(null, "width: " + blockWidth + "px;");
                    ASPx.AddClassNameToElement(block, classPostfix);
                    info.mods.push(new BlockMod(baseClassName + " " + classPostfix, 0, 3));
                    gallery.setBaseWidth(true);

                    var colMax = gallery.getColMax();

                    for(var j = 1; colMax - j >= gallery.colMin; j++) {
                        var priority = 3;
                        if((colMax - gallery.colMin) / 2 < j)
                            priority = 2;
                        var width = blockWidth - (elementWidth * j);
                        classPostfix = ASPx.CreateImportantStyleRule(null, "width: " + width + "px;");
                        var className = baseClassName + " " + classPostfix;
                        info.mods.push(new BlockMod(className, -1, priority));
                    }
                }
            }
            if(info.mods.length > 1) { // precalculate mods
                var calculateMod = function(currentMod, newMod) {
                    block.className = block.className.replace(currentMod.className, newMod.className);
                    if(newMod.width < 0) {
                        newMod.width = initWidth - groupElement.offsetWidth;
                        initWidth -= newMod.width;
                    }
                };

                for(var j = 1, mod; mod = info.mods[j]; j++)
                    calculateMod(info.mods[j - 1], mod);
                calculateMod(info.mods[info.mods.length - 1], info.mods[0]);
                for(var j = 1, mod; mod = info.mods[j]; j++) {
                    if(mod.width < 0) {
                        info.mods.splice(j, 1);
                        var nextMod = info.mods[j + 1];
                        if(nextMod)
                            nextMod += mod.width;
                        j--;
                    }
                }
            }
            return info;
        },
        calculateRegularItemsBlock: function(rowsAmount, itemInfos, commonWidth) {
            var maxRowIndex = 0,
                rowsInfo = [],
                results = { endNodes: [itemInfos[itemInfos.length - 1].node], width: (commonWidth + 2) },
                width = commonWidth,
                offsetWidth = 2;

            if(itemInfos.length < 2)
                return results;

            for(var i = 0; i < rowsAmount; i++)
                rowsInfo.push({ width: 0, items: [] });

            rowsInfo[0].width = commonWidth;
            for(var i = 0; i < itemInfos.length; i++)
                rowsInfo[0].items.push(itemInfos[i]);

            var getLongestRowIndex = function() {
                var res = 0;
                for(var i = 0; i < rowsAmount; i++) {
                    if(rowsInfo[i].width > rowsInfo[res].width)
                        res = i;
                }
                return res;
            };

            var moveItem = function(index) {
                var item = rowsInfo[index].items.pop();
                rowsInfo[index].width -= item.width;
                rowsInfo[index + 1].items.unshift(item);
                rowsInfo[index + 1].width += item.width;
            };

            var updateResults = function(width) {
                results.width = width + offsetWidth;
                results.endNodes = [];
                for(var i = 0; i < rowsAmount; i++) {
                    var ri = rowsInfo[i];
                    if(ri.items.length === 0) return;
                    results.endNodes.push(ri.items[ri.items.length - 1].node);
                }
            };

            while(true) {
                maxRowIndex = getLongestRowIndex();
                width = rowsInfo[maxRowIndex].width;
                if(width != results.width - offsetWidth)
                    updateResults(width);

                if(maxRowIndex == rowsAmount - 1) break;

                moveItem(maxRowIndex);
                for(var i = maxRowIndex + 1; i < rowsAmount; i++) {
                    if(rowsInfo[i].width > width) {
                        if(i == rowsAmount - 1)
                            return results;
                        moveItem(i);
                    }
                    else
                        break;
                }
            }

            return results;
        }
    });

    /* Ribbon ITEMS */
    var ASPxClientRibbonItem = ASPx.CreateClass(null, {
        constructor: function(ribbon, group, index, name, type, groupName, clientDisabled, text) {
            this.group = group;
            this.index = index;
            this.name = name || "";
            this.ribbon = ribbon;

            this.groupName = groupName;

            this.type = type;
            this.text = text;
            this.id = getItemID(this);
            this.enabled = !clientDisabled;
            this.clientDisabledLocked = clientDisabled;
        },
        GetEnabled: function() {
            return this.getEnabledCore();
        },
        SetEnabled: function(enabled) {
            if(!this.ribbon.clientEnabled) return;
            this.clientDisabledLocked = !enabled;
            this.setEnabledCore(enabled, false);
        },
        GetValue: function() {
            return this.getValueCore();
        },
        SetValue: function(value) {
            this.setValueCore(value);
        },
        GetVisible: function() {
            return !!this.getElement();
        },


        initialize: function() { },
        onClick: function(evt) { },
        setEnabledCore: function(enabled, initialization) {
            enabled = enabled && (!initialization || this.enabled);
            if(enabled && this.clientDisabledLocked)
                return;
            this.enabled = enabled;
            ASPx.GetStateController().SetElementEnabled(this.getElement(), enabled);
        },
        getEnabledCore: function() {
            return this.enabled;
        },
        getValueCore: function() {
            return null;
        },
        setValueCore: function() { },

        getElement: function() {
            if(!this.element)
                this.element = document.getElementById(this.id);
            return this.element;
        },
        getLabelElement: function() {
            if(!this.labelElement)
                this.labelElement = ASPx.GetNodeByClassName(this.getElement(), constants.CLASSNAMES.ITEMLABEL);
            return this.labelElement;
        },
        getLabelContentElement: function() {
            if(!this.labelContentElement) {
                var labelElement = this.getLabelElement();
                if(labelElement)
                    this.labelContentElement = ASPx.GetNodeByClassName(labelElement, constants.CLASSNAMES.LABELCONTENT);
            }
            return this.labelContentElement;
        },
        getLabelTextElement: function() {
            if(!this.labelTextElement) {
                var labelElement = this.getLabelElement();
                if(labelElement)
                    this.labelTextElement = ASPx.GetNodeByClassName(labelElement, constants.CLASSNAMES.LABELTEXT);
            }
            return this.labelTextElement;
        },
        getBlockElement: function() {
            if(!this.blockElement)
                this.blockElement = this.getElement().parentNode;
            return this.blockElement;
        },
        getGroupIndexPath: function() {
            return this.group.getIndexPath();
        },
        getFullIndexPath: function() {
            return this.getGroupIndexPath() + ASPx.ItemIndexSeparator + this.index;
        },
        adjust: function() { }
    });

    var ASPxClientRibbonButtonItem = ASPx.CreateClass(ASPxClientRibbonItem, {
        constructor: function(ribbon, group, index, name, type, groupName, clientDisabled, text, navigateUrl) {
            this.constructor.prototype.constructor.call(this, ribbon, group, index, name, type, groupName, clientDisabled, text);
            this.navigateUrl = navigateUrl;
        },
        initialize: function() {
            if(this.ribbon.enabled) {
                ASPx.Evt.AttachEventToElement(this.getElement(), "click", function(evt) {
                    this.onClick(evt);
                }.aspxBind(this));
            }

            var element = this.getElement();
            var label = this.getLabelTextElement();
            if(label && ASPx.ElementHasCssClass(this.getBlockElement(), constants.CLASSNAMES.BLOCK_LARGE_ITEMS) && !element.style.width) {
                var className = ASPx.CreateImportantStyleRule(null, "width: " + this.getLargeItemWidth() + "px;", undefined, "." + constants.CLASSNAMES.BLOCK_LARGE_ITEMS);
                ASPx.AddClassNameToElement(element, className);
            }
        },
        getLargeItemWidth: function() {
            return getLargeItemWidth(this.getLabelTextElement(), ASPx.GetLeftRightMargins(this.getLabelContentElement()));
        },
        onClick: function(evt) {
            if(!this.getEnabledCore()) return;
            this.execute();
        },
        execute: function() {
            this.ribbon.onExecCommand(this, null);
        },
        setEnabledCore: function(enabled, initialization) {
            this.constructor.prototype.setEnabledCore.call(this, enabled, initialization);
            var element = this.getElement();
            if (!element)
                return;
            if(this.navigateUrl) {
                if(!this.enabled && element.href) {
                    ASPx.Attr.SetAttribute (element, "savedhref", element.href);
                    ASPx.Attr.RemoveAttribute (element, "href");
                }
                else if(this.enabled && ASPx.Attr.GetAttribute (element, "savedhref")) {
                    ASPx.Attr.SetAttribute (element, "href", ASPx.Attr.GetAttribute (element, "savedhref"));
                    ASPx.Attr.RemoveAttribute (element, "savedhref");
                }
            }
        },
        adjust: function() {
            var block = this.getBlockElement();
            var label = this.getLabelElement();
            var image = this.getImageElement(constants.CLASSNAMES.IMAGE16);
            if(label && !image && ASPx.ElementHasCssClass(block, constants.CLASSNAMES.BLOCK_HORIZONTAL_ITEMS) && !ASPx.ElementHasCssClass(block, constants.CLASSNAMES.BLOCK_HORIZONTAL_ITEMS_REDUCED)) {
                this.ribbon.CorrectVerticalAlignment(ASPx.AdjustVerticalMargins, function() { return label; });
            }
        },
        getImageElement: function(className) {
            if(this.imageElement === undefined)
                this.imageElement = ASPx.GetNodeByClassName(this.getElement(), className) || null;
            return this.imageElement;
        }
    });
    var ASPxClientRibbonDropDownMenuItem = ASPx.CreateClass(ASPxClientRibbonButtonItem, {
        constructor: function(ribbon, group, index, name, type, groupName, clientDisabled, text, items, navigateUrl) {
            this.constructor.prototype.constructor.call(this, ribbon, group, index, name, type, groupName, clientDisabled, text, navigateUrl);
            this.parent = null;
            if(items !== undefined) {
                this.items = [];
                for(var i = 0, itemInfo; itemInfo = items[i]; i++) {
                    var subItem = ASPxClientRibbonItem.create(ribbon, group, i, itemInfo.c, itemInfo.t, itemInfo.sg, itemInfo);
                    subItem.parent = this;
                    subItem.id = getItemID(subItem);
                    this.items.push(subItem);
                }
            }
        },
        getLargeItemWidth: function() {
            var popOut = this.getPopOutElement();
            return getLargeItemWidth(this.getLabelTextElement(), ASPx.GetLeftRightMargins(this.getLabelContentElement()), popOut.offsetWidth + ASPx.GetLeftRightMargins(popOut));
        },
        getPopup: function() {
            return this.parent ? this.parent.getPopup() : ASPx.GetControlCollection().Get(getItemID(this) + constants.POPUP_MENU_POSTFIX);
        },
        getPopupElement: function() {
            var popup = this.getPopup();
            if(popup)
                return popup.GetMainElement();
        },
        getPopOutElement: function() {
            if(!this.popOutElement)
                this.popOutElement = ASPx.GetNodeByClassName(this.getLabelElement(), constants.CLASSNAMES.ITEMPOPOUT);
            return this.popOutElement;
        },
        onClick: function(evt) {
            if(!this.getEnabledCore()) return;
            setTimeout(function() { this.showPopup(); }.aspxBind(this), 0);
        },
        showPopup: function() {
            if(!this.getEnabledCore()) return;
            var pm = this.getPopup();
            if(pm) {
                if(pm.IsVisible())
                    pm.Hide();
                else if(this.ribbon.lastShownPopup != pm)
                    pm.ShowAtElement(this.getElement());
            }
        },
        hidePopup: function() {
            var pm = this.getPopup();
            if(pm)
                pm.Hide();
        },
        onMenuItemClick: function(menuItem) {
            var indexPath = menuItem.indexPath.split(constants.MENU_INDEXPATH_SEPARATOR);
            var item = this.findItem(indexPath);
            if(item.type != constants.ITEMTYPES.DROPDOWNMENU)
                item.execute();
        },
        findItem: function(indexPath) {
            var item = this.items[indexPath.shift()];
            return indexPath.length ? item.findItem(indexPath) : item;
        },
        setEnabledCore: function(enabled, initialization) {
            if(!this.parent)
                ASPxClientRibbonButtonItem.prototype.setEnabledCore.call(this, enabled, initialization);
            else {
                var indexPath = this.getIndexPath();
                this.getPopup().GetItemByIndexPath(indexPath).SetEnabled(enabled);
                ASPxClientRibbonButtonItem.prototype.setEnabledCore.call(this, enabled, initialization);
            }
            if(this.items) {
                for(var item, i = 0; item = this.items[i]; i++)
                    item.setEnabledCore(item.enabled, initialization);
            }
        },
        getEnabledCore: function() {
            if(!this.parent)
                return ASPxClientRibbonButtonItem.prototype.getEnabledCore.call(this);
            else {
                var indexPath = this.getIndexPath();
                return this.getPopup().GetItemByIndexPath(indexPath).GetEnabled();
            }
        },
        getIndexPath: function(includeLastParent) {
            if(!this.parent) return includeLastParent ? this.index : "";
            var parentPath = this.parent.getIndexPath(includeLastParent);
            return "" + (parentPath === "" ? this.index : (parentPath + constants.MENU_INDEXPATH_SEPARATOR + this.index));
        },
        getFullIndexPath: function() {
            return this.getGroupIndexPath() + ASPx.ItemIndexSeparator + this.getIndexPath(true);
        },
        getItemByName: function(name) {
            if(this.items) {
                var subItem = null;
                for(var i = 0; i < this.items.length; i++) {
                    var item = this.items[i];
                    if(item.name == name)
                        return item;
                    if(!subItem && item.getItemByName)
                        subItem = item.getItemByName(name);
                }
                return subItem;
            }
            return null;
        },
        adjust: function() { }
    });
    var ASPxClientRibbonDropDownSplitItem = ASPx.CreateClass(ASPxClientRibbonDropDownMenuItem, {
        initialize: function() {
            ASPxClientRibbonDropDownMenuItem.prototype.initialize.call(this);
            this.correctDropDownElementPosition();
        },
        onClick: function(evt) {
            if(!this.getEnabledCore()) return;
            var pm = this.getPopup();
            var source = ASPx.Evt.GetEventSource(evt);
            var itemElement = this.getElement();

            var isLargeItem = ASPx.ElementHasCssClass(itemElement.parentNode, constants.CLASSNAMES.BLOCK_LARGE_ITEMS);
            var popoutContainer = isLargeItem ? this.getLabelElement() : this.getPopOutElement();
            var imgContainer = ASPx.GetNodeByClassName(itemElement, constants.CLASSNAMES.ITEMDDIMAGECONTAINER);
            var itemPopupContainer = ASPx.GetNodeByClassName(itemElement, constants.CLASSNAMES.ITEMPOPOUT);

            if(ASPx.GetIsParent(popoutContainer, source) || this.isClickByPopout(isLargeItem, imgContainer, itemPopupContainer, evt))
                setTimeout(function() { this.showPopup(); }.aspxBind(this), 0);
            else if(!pm || !ASPx.GetIsParent(pm.GetMainElement(), source))
                this.execute();
        },
        isClickByPopout: function(isLargeItem, imgContainer, itemPopupContainer, evt) {
            if(isLargeItem) {
                var offsetY = evt.offsetY == undefined ? evt.layerY : evt.offsetY;
                return imgContainer && offsetY > imgContainer.offsetHeight + ASPx.GetTopBottomBordersAndPaddingsSummaryValue(imgContainer)
                    + ASPx.GetTopBottomMargins(imgContainer);
            } else {
                var offsetX = evt.offsetX == undefined ? evt.layerX : evt.offsetX;
                return itemPopupContainer && offsetX > itemPopupContainer.offsetLeft;
            }
        },
        getPopOutElement: function() {
            var itemElement = this.getElement();
            return ASPx.GetNodeByClassName(itemElement, constants.CLASSNAMES.ITEMPOPOUT);
        },
        correctDropDownElementPosition: function() {
            var itemElement = this.getElement();
            var textElement = this.getLabelTextElement();
            var dropDownElement = ASPx.GetNodeByClassName(itemElement, constants.CLASSNAMES.ITEMPOPOUT);
            var imgContainer = ASPx.GetNodeByClassName(itemElement, constants.CLASSNAMES.ITEMDDIMAGECONTAINER);

            var getElementRightCoordinate = function(element) {
                return ASPx.GetAbsolutePositionX(element) + element.offsetWidth + ASPx.PxToInt(ASPx.GetCurrentStyle(element).marginRight);
            }
            var getElementFullWidth = function(element) {
                if(element)
                    return element.offsetWidth + ASPx.GetLeftRightMargins(element);
                return 0;
            }

            var difference = getElementRightCoordinate(itemElement) - getElementRightCoordinate(this.getBlockElement());
            if(difference > 0)
                itemElement.style.maxWidth = ASPx.GetClearClientWidth(itemElement) - difference + "px";
            var width = ASPx.GetClearClientWidth(itemElement) - getElementFullWidth(imgContainer) - getElementFullWidth(textElement) - getElementFullWidth(dropDownElement);
            if(width <= 0)
                return;
            var correctionElement = textElement || imgContainer;
            correctionElement.style.marginRight = ASPx.PxToInt(correctionElement.style.marginRight) + width + "px";
        }
    });
    var ASPxClientRibbonDropDownToggleButtonItem = ASPx.CreateClass(ASPxClientRibbonDropDownSplitItem, {
        constructor: function (ribbon, group, index, name, type, groupName, clientDisabled, text, items, navigateUrl, checked) {
            this.constructor.prototype.constructor.call(this, ribbon, group, index, name, type, groupName, clientDisabled, text, items, navigateUrl, clientDisabled);
            this.checked = !!checked;
        },
        initialize: function () {
            ASPxClientRibbonDropDownSplitItem.prototype.initialize.call(this);
            if (this.checked) {                
                ASPx.GetStateController().SelectElementBySrcElement(this.getElement());
            }
        },
        getValueCore: function () {
            return !!this.checked;
        },
        setValueCore: function (value) {
            value = !!value;
            if (this.getValueCore() == value)
                return;
            this.checked = value;
            if (!this.parent) {
                var element = this.getElement();                
                if (this.checked)
                    ASPx.GetStateController().SelectElementBySrcElement(element);
                else
                    ASPx.GetStateController().DeselectElementBySrcElement(element);
            } else {
                var indexPath = this.getIndexPath();
                this.getPopup().GetItemByIndexPath(indexPath).SetChecked(value);
            }            
            this.setCheckedState();
        },        
        execute: function () {            
            this.setValueCore(!this.checked);
            this.ribbon.onExecCommand(this, this.checked);
        },
        setCheckedState: function () {
            this.ribbon.updateClientStateField(getItemID(this, true), this.checked);
        }
    });
    var ASPxClientRibbonGalleryDropDownItem = ASPx.CreateClass(ASPxClientRibbonDropDownMenuItem, {
        constructor: function(ribbon, group, index, name, type, groupName, clientDisabled, text, items, navigateUrl, properties) {
            this.constructor.prototype.constructor.call(this, ribbon, group, index, name, type, groupName, clientDisabled, text, navigateUrl);
            this.col = properties.col;
            this.row = properties.row;
            this.containerHeight = 0;
            this.maxElementWidth = 0;
            this.maxElementHeight = 0;
            this.maxElementFullWidth = 0;
            this.maxElementFullHeight = 0;
            this.minPopupWidth = 0;
            this.minPopupHeight = 0;
            this.selectedValue = properties.val;
            this.selectedItem = null;
            this.allowSelectItem = properties.asi;
            this.imageWidth = properties.iw;
            this.imageHeight = properties.ih;
            this.showText = properties.st;
            this.imagePosition = properties.ip;
            this.maxTextWidth = properties.tw;
            this.showGroupText = null;
            this.initialized = false;
            if(items !== undefined) {
                this.galleryGroups = [];
                for(var i = 0, groupInfo; groupInfo = items[i]; i++) {
                    var galleryItems = [];
                    groupInfo.index = i;
                    for(var j = 0, itemInfo; itemInfo = groupInfo[j]; j++) {
                        var galleryItem = [];
                        galleryItem.group = groupInfo;
                        galleryItem.index = itemInfo.indx;
                        galleryItem.value = itemInfo.val;
                        galleryItem.id = this.getItemID(galleryItem);
                        galleryItem.element = ASPx.GetElementById(galleryItem.id);
                        galleryItem.selected = false;
                        galleryItems.push(galleryItem);
                    }
                    this.galleryGroups.push(galleryItems);
                }
            }
            if(this.ribbon.enabled)
                this.setContainerSize(1, 1);
        },
        initialize: function() {
            ASPxClientRibbonDropDownMenuItem.prototype.initialize.call(this);
            this.setValueCore(this.selectedValue);
            if (this.ribbon.enabled) {
                this.initializeHandlers();
                if (ASPx.Browser.MSTouchUI) {
                    var styleAttributeName = ASPx.Browser.IE ? "-ms-touch-action" : "touch-action";
                    this.getGalleryDiv().parentNode.style[styleAttributeName] = "pan-y";
                }
            }
        },
        initializeHandlers: function() {
            if(this.ribbon.enabled) {
                ASPx.Evt.AttachEventToElement(this.getGalleryDiv(), "click", function(evt) {
                    this.onGalleryDropDownClick(evt);
                }.aspxBind(this));
            }
        },
        initializeSize: function() {
            if(!this.ribbon.enabled)
                return
            var elements = [];
            this.eachItem(function(item) {
                elements.push(item.element);
            });
            this.initMaxElementSize(elements);
            ASPx.Data.ForEach(elements, function(element) {
                if(!element)
                    return;
                this.setElementSize(element, this.maxElementWidth, this.maxElementHeight);
            }.aspxBind(this));
            if (this.getShowGroupText()) {
                var maxCount = this.getMaxItemsCountInGroup();
                this.col = this.col > maxCount ? maxCount : this.col;
            }
            this.setContainerSize(this.maxElementFullWidth * this.col, this.maxElementFullHeight * this.row);
        },
        setItems: function(items) {
            this.clearItems();
            var gallery = this.getGalleryDiv();
            this.setItemsInternal(gallery, items, true);

            for(var i = 0, groupInfo; groupInfo = items[i]; i++) {
                var galleryItems = [];
                groupInfo.index = i;
                for(var j = 0, itemInfo; itemInfo = groupInfo.items[j]; j++) {
                    var galleryItem = [];
                    galleryItem.group = groupInfo;
                    galleryItem.index = j;
                    galleryItem.value = itemInfo.value;
                    galleryItem.id = this.getItemID(galleryItem);
                    galleryItem.element = ASPx.GetElementById(galleryItem.id);
                    galleryItem.selected = false;
                    galleryItems.push(galleryItem);
                }
                this.galleryGroups.push(galleryItems);
            }
        },
        setItemsInternal: function(gallery, items, showGroups, itemIdGetter) {
            for(var i = 0, groupInfo; groupInfo = items[i]; i++) {
                if(showGroups) {
                    var groupElement = document.createElement("div");
                    groupElement.setAttribute("class", constants.CLASSNAMES.GALLERY_GROUP_DIV);
                    groupElement.innerHTML = groupInfo.text;
                    gallery.appendChild(groupElement);
                }

                for(var j = 0, itemInfo; itemInfo = groupInfo.items[j]; j++) {
                    this.addItemInternal(gallery, itemInfo, i, j, itemIdGetter);
                }
            }
        },
        addItemInternal: function(gallery, itemInfo, groupIndex, itemIndex, itemIdGetter) {
            var itemId = itemIdGetter ? itemIdGetter.call(this, groupIndex, itemIndex) : this.getItemIDInternal(groupIndex, itemIndex);

            var glrItemElement = document.createElement("div");
            glrItemElement.setAttribute("class", constants.CLASSNAMES.GALLERY_ITEM)
            glrItemElement.setAttribute("id", itemId);

            var glrItemContentElement = document.createElement("div");
            glrItemContentElement.setAttribute("class", constants.CLASSNAMES.GALLERY_ITEM_CONTENT);
            glrItemElement.appendChild(glrItemContentElement);

            if(this.showText && (this.imagePosition == 'Bottom' || this.imagePosition == 'Right')) {
                this.addItemTextElement(glrItemContentElement, itemInfo);
            }

            var glrItemImage = document.createElement("img");
            glrItemImage.setAttribute("style", "width:" + this.imageWidth + ";height:" + this.imageHeight);
            glrItemImage.setAttribute("src", itemInfo.image.url ? itemInfo.image.url : ASPx.EmptyImageUrl);
            if(itemInfo.image.className)
                glrItemImage.setAttribute("class", itemInfo.image.className);
            glrItemContentElement.appendChild(glrItemImage);

            if(this.showText && (this.imagePosition == 'Top' || this.imagePosition == 'Left')) {
                this.addItemTextElement(glrItemContentElement, itemInfo);
            }

            gallery.appendChild(glrItemElement);

            ASPx.GetStateController().AddHoverItem(itemId,
                ["dxr-itemHover"],
                [""],
                null,
                null,
                null);

            ASPx.GetStateController().AddSelectedItem(itemId,
                ["dxr-itemChecked"],
                [""],
                null,
                null,
                null);
        },
        addItemTextElement: function(parent, itemInfo) {
            var glrItemTextElement = document.createElement("div");
            glrItemTextElement.setAttribute("class", constants.CLASSNAMES.GALLERY_ITEM_TEXT);
            glrItemTextElement.innerHTML = itemInfo.text;
            if(this.maxTextWidth)
                glrItemTextElement.style.width = this.maxTextWidth;
            parent.appendChild(glrItemTextElement);
        },
        clearItems: function() {
            this.selectedItem = null;
            this.selectedValue = null;
            this.galleryGroups = [];
            var gallery = this.getGalleryDiv();
            gallery.innerHTML = '';
            this.eachItem(function(item) {
                ASPx.GetStateController().RemoveSelectedItem(item.id);
                ASPx.GetStateController().RemoveHoverItem(item.id);
            });
            this.minPopupWidth = 0;
            this.minPopupHeight = 0;
            this.maxElementWidth = 0;
            this.maxElementHeight = 0;
            this.initialized = false;
        },
        onGalleryDropDownClick: function(evt) {
            if(!this.getEnabledCore()) return;
            var source = ASPx.Evt.GetEventSource(evt);
            var element = ASPx.GetParentByClassName(source, constants.CLASSNAMES.GALLERY_ITEM);
            if(element) {
                var item = this.getItemByElement(element);
                this.selectItem(item);
                this.execute();
            }
            var popup = this.getPopup();
            if(popup) {
                if(popup.IsVisible())
                    popup.Hide();
            }
        },
        execute: function() {
            this.ribbon.onExecCommand(this, this.getValueCore());
        },
        setValueCore: function(value) {
            if(!value) {
                this.selectedValue = null;
                this.selectItem(null);
                return;
            }
            this.eachItem(function(item) {
                if(item.value == value) {
                    if(!item.selected) {
                        this.selectItem(item);
                    }
                }
            });
        },
        selectItem: function(item, elementSelector) {
            if(!elementSelector)
                elementSelector = function(i) {
                    return i.element;
                }
            if(item) {
                this.selectedValue = item.value;
                if(!this.allowSelectItem)
                    return;
                this.selectedItem = item;
                ASPx.GetStateController().SelectElementBySrcElement(elementSelector(item));
            }
            this.updateClientState();
            this.eachItem(function(i) {
                if (!item || item.id != i.id)
                    ASPx.GetStateController().DeselectElementBySrcElement(elementSelector(i));
            });
        },
        updateClientState: function() {
            this.ribbon.updateClientStateField(getItemID(this, true), this.selectedValue);
        },
        getValueCore: function() {
            return this.selectedValue;
        },
        getPopup: function() {
            return ASPx.GetControlCollection().Get(getItemID(this) + constants.POPUP_GALLERY_POSTFIX);
        },
        getGalleryDiv: function() {
            var popup = this.getPopup();
            if(!popup)
                return;
            var dropDownGalleryElement = ASPx.GetNodeByClassName(popup.GetWindowElement(-1), constants.CLASSNAMES.GALLERY_MAIN_DIV);
            return dropDownGalleryElement;
        },
        getItemByElement: function(element) {
            var result;
            this.eachItem(function(item) {
                if(item.id == element.id ||
                    item.id.replace(constants.POPUP_GALLERY_POSTFIX, "") == element.id) {
                    result = item;
                    return false;
                }
            });
            return result;
        },
        showPopup: function() {
            if(!this.getEnabledCore()) return;
            var popup = this.getPopup();
            if(popup) {
                if(popup.IsVisible())
                    popup.Hide();
                else if(this.ribbon.lastShownPopup != popup) {
                    this.setPopupOffset(popup);
                    popup.ShowAtElement(this.getElement());
                    this.scrollPopUpToSelectedItem(popup);
                }
            }
        },
        setPopupOffset: function(popup) {
            popup.SetPopupVerticalOffset(this.element.offsetHeight);
        },
        onPopupPopUp: function() {
            var popup = this.getPopup();
            this.initializeSize();
            this.initContainerHeight();
            if(!this.minPopupWidth || !this.minPopupHeight)
                this.eachItem(function(item) {
                    this.minPopupWidth = item.element.offsetLeft + item.element.offsetWidth + ASPx.GetVerticalScrollBarWidth();
                    this.minPopupHeight = item.element.offsetTop + item.element.offsetHeight + popup.GetWindowFooterHeightLite(-1);
                    return false;
                });
            if (!this.initialized)
                this.initTooltips(function(item) { return item.element; })
            this.initialized = true;
            this.setPopupSize(popup, this.getPopupWidth(), this.containerHeight);
        },
        initTooltips: function(elementGetter) {
            if(!this.showText)
                return;
            var maxTextWidth = ASPx.PxToInt(this.maxTextWidth);
            this.eachItem(function(item) {
                var element = elementGetter(item);
                var textElement = ASPx.GetNodesByClassName(element, constants.CLASSNAMES.GALLERY_ITEM_TEXT)[0];
                if(textElement.scrollWidth > maxTextWidth) {
                    var text = textElement.textContent;
                    if(text)
                        element.title = text.trim();
                }
            });
        },
        setPopupSize: function(popup, popupWidth, popupHeight) {
            this.setContainerSize(popupWidth, popupHeight);
            popup.minWidth = 0;
            popup.minHeight = 0;
            popup.SetSize(0, 0);
            var width = popup.GetWidth();
            var height = popup.GetHeight();
            popup.SetWidth(width);
            popup.SetHeight(height);
            popup.minWidth = width;
            popup.minHeight = this.minPopupHeight;
            this.setContainerSize();
        },
        setContainerSize: function(width, height) {
            this.setElementSize(this.getGalleryDiv(), width, height);
        },
        setElementSize: function(element, width, height) {
            var clearWidth = width - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(element);
            var clearHeight = height - ASPx.GetTopBottomBordersAndPaddingsSummaryValue(element);
            var widthPx = width ? clearWidth + "px" : "";
            var heightPx = height ? clearHeight + "px" : "";
            element.style.width = widthPx;
            element.style.height = heightPx;
        },
        eachItem: function(callback) {
            for(var i = 0, group; group = this.galleryGroups[i]; i++) {
                for(var j = 0, item; item = group[j]; j++) {
                    if((callback.aspxBind(this)(item)) == false)
                        return;
                }
            }
        },
        getPopupWidth: function() {
            var popupWidth = this.col * this.maxElementFullWidth;
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion == 8)
                popupWidth += ASPx.GetVerticalScrollBarWidth();
            return popupWidth;
        },
        getLastItem: function() {
            for(var i = this.galleryGroups.length - 1, group; group = this.galleryGroups[i]; i--) {
                for(var j = group.length - 1, item; item = group[j]; j--) {
                    return item;
                }
            }
        },
        getItemsCount: function() {
            var count = 0;
            this.eachItem(function() {
                count++;
            });
            return count;
        },
        getMaxItemsCountInGroup: function() {
            var maxLength = 0;
            for(var i = 0, group; group = this.galleryGroups[i]; i++) {
                if(group.length > maxLength)
                    maxLength = group.length;
            }
            return maxLength;
        },
        getItemID: function(item) {
            return this.getItemIDInternal(item.group.index, item.index);
        },
        getItemIDInternal: function(groupIndex, itemIndex) {
            return getItemID(this) + constants.POPUP_GALLERY_POSTFIX + "_" + groupIndex + "i" + itemIndex;
        },
        initMaxElementSize: function(elements) {
            if(this.maxElementWidth || this.maxElementHeight || !elements || !elements.length)
                return;
            var size = this.getMaxSize(elements);
            this.maxElementWidth = size.width;
            this.maxElementHeight = size.height;
            this.maxElementFullWidth = this.maxElementWidth + ASPx.GetLeftRightMargins(elements[0]);
            this.maxElementFullHeight = this.maxElementHeight + ASPx.GetTopBottomMargins(elements[0]);
        },
        getMaxSize: function(elements) {
            var maxWidth = 0;
            var maxHeight = 0;
            ASPx.Data.ForEach(elements, function(element) {
                if(!element)
                    return;
                var width = element.offsetWidth;
                var height = element.offsetHeight;
                maxWidth = maxWidth < width ? width : maxWidth;
                maxHeight = maxHeight < height ? height : maxHeight;
            });
            var size = [];
            size.height = maxHeight;
            size.width = maxWidth;
            return size;
        },
        initContainerHeight: function() {
            var row = 1, col = 0;
            var lastItem;

            var top = -1;
            var element = null;
            this.eachItem(function(item) {
                if(top == -1)
                    top = item.element.offsetTop;
                var nextItemTop = item.element.offsetTop;
                if(nextItemTop != top) {
                    row++;
                    top = item.element.offsetTop;
                }
                if(row >= this.row) {
                    element = item.element;
                    return false;
                }
            });
            if(!element)
                element = this.getLastItem().element;
            if (element)
                this.containerHeight = element.offsetTop + element.offsetHeight - element.parentNode.offsetTop + 1;
        },
        getShowGroupText: function () {
            if (!this.showGroupText) {
                var groups = ASPx.GetChildNodesByClassName(this.getGalleryDiv(), "dxr-glrGroup");
                this.showGroupText = (groups.length != 0 && groups[0].textContent) ? true : false;
            }
            return this.showGroupText;
        },
        getSelectedItemRowNumber: function () {
            for (var i = 0, itemCount = 0, rowCount = 0; i < this.selectedItem.group.index; i++) {
                itemCount += this.galleryGroups[i].length;
                rowCount += Math.ceil(this.galleryGroups[i].length / this.col);
            }
            return this.showGroupText ? rowCount + Math.ceil((this.selectedItem.index + 1) / this.col) : Math.ceil((itemCount + this.selectedItem.index + 1) / this.col);
        },
        scrollPopUpToSelectedItem: function (popup) {            
            if (this.selectedItem) {
                var content = popup.GetWindowContentElement(-1);
                var scrollPosistion = this.selectedItem.element.offsetHeight * (this.getSelectedItemRowNumber() - 1);
                var groupOffset = 0;
                if (this.showGroupText) {
                    var firstGroupRow = Math.ceil((this.selectedItem.index + 1) / this.col) == 1 ? true : false;
                    var groupLabelHeigth = ASPx.GetChildNodesByClassName(this.getGalleryDiv(), constants.CLASSNAMES.GALLERY_GROUP_DIV)[0].offsetHeight;
                    var groupOffset = firstGroupRow ? groupLabelHeigth * this.selectedItem.group.index : groupLabelHeigth * (this.selectedItem.group.index + 1);
                }
                content.scrollTop = scrollPosistion + groupOffset;
            }
        }
    });
    var ASPxClientRibbonGalleryBarItem = ASPx.CreateClass(ASPxClientRibbonGalleryDropDownItem, {
        constructor: function(ribbon, group, index, name, type, groupName, clientDisabled, text, items, navigateUrl, properties) {
            this.constructor.prototype.constructor.call(this, ribbon, group, index, name, type, groupName, clientDisabled, text, items, navigateUrl, properties);
            this.eachItem(function(item) {
                item.barItemId = this.getBarItemID(item);
                item.barElement = ASPx.GetElementById(item.barItemId);
            });
            this.colMax = properties.colMax;
            this.colMin = properties.colMin;
            this.rowBar = properties.rowBar;
            this.scrollManager = null;
            this.scrollOffset = 0;
            this.needScollToSelectedItem = true;
            this.galleryWidth = 0;
            this.adjusted = true;
        },
        initialize: function() {
            ASPxClientRibbonGalleryDropDownItem.prototype.initialize.call(this);
            this.initializeCore();
        },
        initializeCore: function() {
            var noItems = true;
            this.eachItem(function() {
                noItems = false;
                return false;
            });
            if(noItems)
                return;
            this.initializeSize();
            this.initializeScrolling();
            this.initTooltips(function(item) { return item.barElement; })
            setTimeout(function() { this.setValueCore(this.selectedValue); }.aspxBind(this), 100);
        },
        initializeHandlers: function() {
            ASPxClientRibbonGalleryDropDownItem.prototype.initializeHandlers.call(this);
            if(this.ribbon.enabled) {
                ASPx.Evt.AttachEventToElement(this.getGalleryBarDiv(), "click", function(evt) {
                    this.onGalleryBarClick(evt);
                }.aspxBind(this));
            }
        },
        initializeScrolling: function() {
            this.scrollManager = this.scrollManager ? this.scrollManager : new ASPx.ScrollingManager(this, this.getGalleryBarDiv(), [0, 1], this.onBeforeScrolling, this.onAfterScrolling, true);
            this.scrollManager.scrollSessionInterval = 100;
            this.scrollManager.animationAcceleration = 2;
            this.scrollManager.animationOffset = 10;
            this.initializeScrollButton(this.getUpButtonElement(), -1);
            this.initializeScrollButton(this.getDownButtonElement(), 1);
            this.updateScrollButtonsEnabled();
            this.initializeScrollOffset();
            this.getElement().style.overflow = "hidden";
            if(ASPx.Browser.MSTouchUI) {
                this.getGalleryBarContainer().style.marginRight = "0px";
                this.getGalleryBarContainer().style["-ms-overflow-style"] = "none";
            }
        },
        initializeScrollOffset: function() {
            var galleryElement = this.getGalleryBarDiv();
            var width = galleryElement.style.width;
            galleryElement.style.width = this.maxElementFullWidth + "px";

            var rowsCount = 0;
            var offsetTop = 0;
            var firstElementOffset = 0;
            this.eachItem(function(item) {
                if(offsetTop == 0) {
                    offsetTop = item.barElement.offsetTop;
                    firstElementOffset = offsetTop;
                }
                var elementOffsetTop = item.barElement.offsetTop;
                if(offsetTop != elementOffsetTop) {
                    rowsCount++;
                    offsetTop = elementOffsetTop;
                }
                if(rowsCount >= this.rowBar)
                    return false;

            });
            if(rowsCount >= this.rowBar)
                this.scrollOffset = offsetTop - firstElementOffset;
            else
                this.scrollOffset = this.maxElementFullHeight * this.rowBar;
            galleryElement.style.width = width;
        },
        initializeScrollButton: function(button, dir) {
            if(!button || !button.id) return;
            ASPx.Selection.SetElementSelectionEnabled(button, false);
            var manager = this.scrollManager;
            if(this.enabled) {
                ASPx.Evt.AttachEventToElement(button, ASPx.TouchUIHelper.touchMouseDownEventName, function(e) { manager.StartScrolling(dir, 10, 5); ASPx.Evt.PreventEvent(e); });
                ASPx.Evt.AttachEventToElement(button, ASPx.TouchUIHelper.touchMouseUpEventName, function(e) { manager.StopScrolling(); });
                if(ASPx.Browser.IE) {
                    ASPx.Evt.AttachEventToElement(button, "dblclick", function(e) { manager.StartScrolling(dir, 10, 5); manager.StopScrolling(); });
                }
            }
        },
        initializeSize: function() {
            if(!this.maxElementWidth || !this.maxElementHeight) {
                var elements = [];
                this.eachItem(function(item) {
                    elements.push(item.barElement);
                });
                this.initMaxElementSize(elements);
                var margin = this.calculateItemMargin();
                var height = this.maxElementHeight + margin * 2;
                var needChromeFix = (ASPx.Browser.WebKitFamily && height % Math.floor(height) >= 0.5); //Chrome bug. Issue 270926
                if(needChromeFix)
                    height = Math.floor(height);
                ASPx.Data.ForEach(elements, function(element) {
                    if(!element)
                        return;
                    if(needChromeFix) {
                        element.style.marginBottom = "1px";
                    }
                    this.setElementSize(element, this.maxElementWidth, height);
                    element.children[0].style.marginTop = margin + "px";
                }.aspxBind(this));
                var colMax = this.getColMax();
                this.galleryWidth = this.maxElementFullWidth * colMax;
                this.setBaseWidth();
            }
            ASPxClientRibbonGalleryDropDownItem.prototype.initializeSize.call(this);
        },
        getColMax: function() {
            var itemsCount = this.getItemsCount();
            var colMax = this.colMax > itemsCount ? itemsCount : this.colMax;
            return colMax;
        },
        setItems: function(items) {
            ASPxClientRibbonGalleryDropDownItem.prototype.setItems.call(this, items);
            this.setItemsInternal(this.getGalleryBarDiv(), items, false, this.getBarItemIDInternal);
            this.eachItem(function(item) {
                item.barItemId = this.getBarItemID(item);
                item.barElement = ASPx.GetElementById(item.barItemId);
            });
            if (this.group.tab.tabElement.offsetParent === null) {
                this.adjusted = false;
                return;
            }
            this.group.tab.deleteModifications();
            this.initializeCore();
            this.group.tab.recalculateModifications();
            this.adjusted = true;
        },
        clearItems: function() {
            var gallery = this.getGalleryBarDiv();
            gallery.innerHTML = "";
            this.getGalleryBarDiv().style.top = "0px";
            this.eachItem(function(item) {
                ASPx.GetStateController().RemoveSelectedItem(item.barItemId);
                ASPx.GetStateController().RemoveHoverItem(item.barItemId);
            });

            ASPxClientRibbonGalleryDropDownItem.prototype.clearItems.call(this);
        },
        adjust: function () {
            if (this.adjusted)
                return;
            this.group.tab.deleteModifications();
            this.initializeCore();
            this.group.tab.recalculateModifications();
            this.adjusted = true;
        },
        setBaseWidth: function(reset) {
            var galleryElement = this.getGalleryBarDiv();
            if(reset)
                galleryElement.style.width = "";
            else
                galleryElement.style.width = this.galleryWidth + "px";
        },
        onGalleryBarClick: function(evt) {
            this.needScollToSelectedItem = false;
            this.onGalleryDropDownClick(evt);
        },
        selectItem: function(item) {
            ASPxClientRibbonGalleryDropDownItem.prototype.selectItem.call(this, item);
            ASPxClientRibbonGalleryDropDownItem.prototype.selectItem.call(this, item, function(i) {
                return i.barElement;
            });
            if(this.needScollToSelectedItem)
                this.scrollToItem(item);
            this.needScollToSelectedItem = true;
        },
        calculateItemMargin: function() {
            var rowsHeight = this.maxElementFullHeight * this.rowBar;
            var margin = ASPx.GetClearClientHeight(this.getGalleryBarContainer()) - rowsHeight;
            if(margin < 0)
                return 0;
            var marginTop = margin / (this.rowBar * 2);
            return marginTop;
        },
        onBeforeScrolling: function(manager, direction) {
            var gallery = manager.owner;
            if(!gallery.getEnabledCore() || (gallery.isFullyScrolledToBottom() && direction > 0 || gallery.isFullyScrolledToTop() && direction < 0)) {
                manager.StopScrolling();
                return;
            }
            var offset = gallery.scrollOffset;
            var top =  ASPx.PxToInt(gallery.getGalleryBarDiv().style.top);
            if(direction < 0) {
                var newTop = top + gallery.scrollOffset;
                if(newTop > 0)
                    offset = offset - newTop;
            }
            if(direction > 0) {
                var newTop = top - gallery.scrollOffset;
                var maxTop = -(gallery.getGalleryBarDiv().offsetHeight - offset);
                if(newTop < maxTop)
                    offset = offset - (maxTop - newTop);
            }
                manager.animationOffset = offset;
        },
        onAfterScrolling: function(manager, direction) {
            manager.owner.updateScrollButtonsEnabled();
        },
        scrollToItem: function(item) {
            if(!item || !this.scrollManager || !this.getEnabledCore())
                return;
            var galleryBarDiv = this.getGalleryBarDiv();
            galleryBarDiv.style.top = 0;
            var itemOffsetTop = item.barElement.offsetTop;
            while(ASPx.PxToInt(galleryBarDiv.style.top) - this.scrollOffset >= -itemOffsetTop) {
                this.scrollManager.StartScrolling(1, 10, itemOffsetTop);
                this.scrollManager.StopScrolling();
            }
            this.updateScrollButtonsEnabled();
        },
        updateScrollButtonsEnabled: function() {
            ASPx.GetStateController().SetElementEnabled(this.getUpButtonElement(), !this.isFullyScrolledToTop());
            ASPx.GetStateController().SetElementEnabled(this.getDownButtonElement(), !this.isFullyScrolledToBottom());
        },
        isFullyScrolledToBottom: function() {
            return ASPx.PxToInt(this.getGalleryBarDiv().style.top) <= -(this.getGalleryBarDiv().offsetHeight - this.getElement().clientHeight);
        },
        isFullyScrolledToTop: function() {
            return ASPx.PxToInt(this.getGalleryBarDiv().style.top) >= 0;
        },
        onPopupPopUp: function() {
            ASPxClientRibbonGalleryDropDownItem.prototype.onPopupPopUp.call(this);
            this.col = this.getCurrentBarColumnsCount();
            this.setPopupSize(this.getPopup(), this.getPopupWidth(), this.containerHeight);
        },
        onAfterModification: function() {
            if(this.selectedItem)
                this.scrollToItem(this.selectedItem);
            else {
                var maxTop = -(this.getGalleryBarDiv().offsetHeight - this.getElement().clientHeight);
                var currentTop = ASPx.PxToInt(this.getGalleryBarDiv().style.top);
                if(maxTop > currentTop)
                    var lastItem = this.getLastItem();
                    this.scrollToItem(lastItem);
            }
            this.updateScrollButtonsEnabled();
        },
        getCurrentBarColumnsCount: function() {
            var galleryBarElement = this.getGalleryBarDiv();
            if(!galleryBarElement)
                return;
            var columnsCount = Math.floor(galleryBarElement.offsetWidth / this.maxElementFullWidth);
            return columnsCount;
        },
        getGalleryBarDiv: function() {
            return ASPx.GetNodeByClassName(this.getElement(), constants.CLASSNAMES.GALLERY_MAIN_DIV);
        },
        getGalleryBarContainer: function() {
            return ASPx.GetNodeByClassName(this.getElement(), constants.CLASSNAMES.GALLERY_BAR_CONTAINER);
        },
        getPopOutElement: function() {
            var itemElement = this.getElement();
            return ASPx.GetChildById(itemElement, getItemID(this) + "_PB");
        },
        getUpButtonElement: function() {
            var itemElement = this.getElement();
            return ASPx.GetChildById(itemElement, getItemID(this) + "_UB");
        },
        getDownButtonElement: function() {
            var itemElement = this.getElement();
            return ASPx.GetChildById(itemElement, getItemID(this) + "_DB");
        },
        getBarItemID: function(item) {
            return this.getBarItemIDInternal(item.group.index, item.index);
        },
        getBarItemIDInternal: function(groupIndex, itemIndex) {
            return getItemID(this) + "_" + groupIndex + "i" + itemIndex;
        },
        setPopupOffset: function() {
            return;
        },
        setEnabledCore: function(enabled, initialization) {
            this.constructor.prototype.setEnabledCore.call(this, enabled, initialization);
            this.eachItem(function(item) {
                ASPx.GetStateController().SetElementEnabled(item.barElement, enabled);
            });
            if(enabled)
                this.updateScrollButtonsEnabled();
            else {
                ASPx.GetStateController().SetElementEnabled(this.getUpButtonElement(), enabled);
                ASPx.GetStateController().SetElementEnabled(this.getDownButtonElement(), enabled);
            }
            ASPx.GetStateController().SetElementEnabled(this.getPopOutElement(), enabled);
            
        },
        onClick: function(evt) {
            if(!this.getEnabledCore()) return;
            var source = ASPx.Evt.GetEventSource(evt);
            var popout = this.getPopOutElement();
            if(ASPx.GetIsParent(popout, source)) {
                ASPxClientRibbonGalleryDropDownItem.prototype.onClick.call(this, evt);
            }
        }
    });
    var ASPxClientRibbonColorButtonItem = ASPx.CreateClass(ASPxClientRibbonDropDownSplitItem, {
        constructor: function(ribbon, group, index, name, type, groupName, clientDisabled, text, color, isAutomaticColor) {
            this.constructor.prototype.constructor.call(this, ribbon, group, index, name, type, groupName, clientDisabled, text);
            if(ribbon.enabled) {
                if(color || isAutomaticColor)
                    this.setValueCore(color, isAutomaticColor);
                this.updateIndicator();
            }
        },
        onColorChanged: function() {
            this.hidePopup();
            this.updateIndicator();
            this.updateClientState();
            this.execute();
        },
        onCNCShouldBeClosed: function() {
            this.getPopup().Hide();
            this.execute();
        },
        onPopupPopUp: function () {
            this.getColorTable().SetOwner(this.getPopup());
            this.getColorTable().SetOwnerElementId(this.element.id);
            this.getColorTable().SetColorSelectorDisplay(false);
            this.getColorTable().SetColorTablesDisplay(true);

            this.getPopup().SetWidth(0);
            this.getPopup().SetHeight(0);
            this.getPopup().UpdatePositionAtElement(this.element);
        },
        execute: function() {
            this.ribbon.onExecCommand(this, this.getValueCore());
        },
        getPopup: function() {
            return ASPx.GetControlCollection().Get(getItemID(this) + constants.POPUP_CONTROL_POSTFIX);
        },
        getPopupElement: function() {
            var popup = this.getPopup();
            if(popup)
                return popup.GetContentContainer(-1);
        },
        getColorTable: function() {
            return ASPx.GetControlCollection().Get(this.id + constants.COLORTABLE_POSTFIX);
        },
        getValueCore: function() {
            return this.getColorTable().GetValue();
        },
        getColor: function() {
            return this.getColorTable().GetColor();
        },
        setValueCore: function(color, isAutomaticColor) {
            this.getColorTable().SetColor(color, isAutomaticColor);
            this.updateIndicator();
        },
        getIndicatorElement: function() {
            if(!this.indicatorElement)
                this.indicatorElement = document.getElementById(this.id + constants.COLORINDICATOR_POSTFIX);
            return this.indicatorElement;
        },
        updateIndicator: function() {
            var indicator = this.getIndicatorElement();
            indicator.style.backgroundColor = this.getColor();
        },
        getState: function() {
            var colorTable = this.getColorTable();
            return (colorTable.GetCustomColorTableControl() ? colorTable.GetState() : "") + constants.COLORITEM_CLIENTSTATE_SEPARATOR +  this.getColor() + 
                constants.COLORITEM_CLIENTSTATE_SEPARATOR + this.getColorTable().IsAutomaticColorSelected();
        },
        updateClientState: function() {
            this.ribbon.updateClientStateField(getItemID(this, true), this.getState());
        }
    });

    var ASPxClientRibbonTemplateItem = ASPx.CreateClass(ASPxClientRibbonItem, {
        setEnabledCore: function(enabled, initialization) {
            this.constructor.prototype.setEnabledCore.call(this, enabled, initialization);
            enabled = this.GetEnabled();
            if(!initialization || !enabled) {
                ASPx.GetControlCollection().ProcessControlsInContainer(this.getElement(), function(control) {
                    if(ASPx.IsFunction(control.SetEnabled))
                        control.SetEnabled(enabled);
                });
            }
        }
    });

    var ASPxClientRibbonToggleItem = ASPx.CreateClass(ASPxClientRibbonButtonItem, {
        constructor: function(ribbon, group, index, name, type, groupName, clientDisabled, text, checked) {
            this.constructor.prototype.constructor.call(this, ribbon, group, index, name, type, groupName, clientDisabled, text);
            this.checked = !!checked;
        },
        initialize: function() {
            ASPxClientRibbonButtonItem.prototype.initialize.call(this);
            if(this.checked)
                ASPx.GetStateController().SelectElementBySrcElement(this.getElement());
        },
        getValueCore: function() {
            return !!this.checked;
        },
        setValueCore: function(value) {
            value = !!value;
            if(this.getValueCore() == value)
                return;
            var element = this.getElement();
            this.checked = value;
            if(this.checked)
                ASPx.GetStateController().SelectElementBySrcElement(element);
            else
                ASPx.GetStateController().DeselectElementBySrcElement(element);
            this.setCheckedState();
        },
        onClick: function(evt) {
            if(!this.getEnabledCore()) return;
            this.setValueCore(!this.checked);
            this.ribbon.onExecCommand(this, this.checked);
        },
        setCheckedState: function() {
            this.ribbon.updateClientStateField(getItemID(this, true), this.checked);
        }
    });

    var ASPxClientRibbonOptionItem = ASPx.CreateClass(ASPxClientRibbonToggleItem, {
        constructor: function(ribbon, group, index, name, type, groupName, clientDisabled, text, checked, optionGroup) {
            this.constructor.prototype.constructor.call(this, ribbon, group, index, name, type, groupName, clientDisabled, text, checked);
            this.optionGroup = optionGroup;
        },
        setValueCore: function(value) {
            ASPxClientRibbonToggleItem.prototype.setValueCore.call(this, value);
            if(this.checked && this.optionGroup) {
                for(var i = 0, tab; tab = this.ribbon.tabs[i]; i++) {
                    for(var j = 0, group; group = tab.groups[j]; j++) {
                        for(var k = 0, item; item = group.items[k]; k++) {
                            if(item != this && item instanceof ASPxClientRibbonOptionItem && item.optionGroup == this.optionGroup)
                                item.setValueCore(false);
                        }
                    }
                }
            }
        },
        setCheckedState: function() {
            if(this.checked)
                this.ribbon.updateClientStateField(getItemID(this, true), this.checked);
            else
                this.ribbon.clearClientStateField(getItemID(this, true));
        }
    });

    var ASPxClientRibbonEditorItem = ASPx.CreateClass(ASPxClientRibbonItem, {
        onValueChanged: function() {
            this.ribbon.onExecCommand(this, this.getEditor().GetValue());
        },
        getValueCore: function () {
            if (this.ribbon.enabled)
                return this.getEditor().GetValue();
        },
        setValueCore: function (value) {
            if (this.ribbon.enabled)
                this.getEditor().SetValue(value);
        },
        setEnabledCore: function(enabled, initialization) {
            this.constructor.prototype.setEnabledCore.call(this, enabled, initialization);
            if(this.ribbon.enabled)
                this.getEditor().SetEnabled(this.enabled);
        },
        getEditor: function() {
            return ASPx.GetControlCollection().Get(getItemID(this) + this.getEditorPostfix());
        },
        getEditorPostfix: function() {

        }
    });
    var ASPxClientRibbonCheckBoxItem = ASPx.CreateClass(ASPxClientRibbonEditorItem, {
        getEditorPostfix: function() {
            return constants.CHECKBOX_POSTFIX;
        }
    });
    var ASPxClientRibbonComboBoxItem = ASPx.CreateClass(ASPxClientRibbonEditorItem, {
        getEditorPostfix: function() {
            return constants.COMBOBOX_POSTFIX;
        }
    });
    var ASPxClientRibbonDateEditItem = ASPx.CreateClass(ASPxClientRibbonEditorItem, {
        getEditorPostfix: function() {
            return constants.DATEEDIT_POSTFIX;
        }
    });
    var ASPxClientRibbonSpinEditItem = ASPx.CreateClass(ASPxClientRibbonEditorItem, {
        getEditorPostfix: function() {
            return constants.SPINEDIT_POSTFIX;
        }
    });
    var ASPxClientRibbonTextBoxItem = ASPx.CreateClass(ASPxClientRibbonEditorItem, {
        getEditorPostfix: function() {
            return constants.TEXTBOX_POSTFIX;
        }
    });

    ASPxClientRibbonItem.create = function(ribbon, group, index, name, type, subGroup, info) {
        switch(type) {
            case constants.ITEMTYPES.BUTTON:
                return new ASPxClientRibbonButtonItem(ribbon, group, index, name, type, subGroup, info.cdi, info.txt, info.nu);
            case constants.ITEMTYPES.DROPDOWNMENU:
                return new ASPxClientRibbonDropDownMenuItem(ribbon, group, index, name, type, subGroup, info.cdi, info.txt, info.i, info.nu);
            case constants.ITEMTYPES.DROPDOWNSPLIT:
                return new ASPxClientRibbonDropDownSplitItem(ribbon, group, index, name, type, subGroup, info.cdi, info.txt, info.i, info.nu);
            case constants.ITEMTYPES.DROPDOWNTOGGLEBUTTON:
                return new ASPxClientRibbonDropDownToggleButtonItem(ribbon, group, index, name, type, subGroup, info.cdi, info.txt, info.i, info.nu, info.chk);
            case constants.ITEMTYPES.CHECKBOX:
                return new ASPxClientRibbonCheckBoxItem(ribbon, group, index, name, type, subGroup, info.cdi, info.txt);
            case constants.ITEMTYPES.COMBOBOX:
                return new ASPxClientRibbonComboBoxItem(ribbon, group, index, name, type, subGroup, info.cdi, info.txt);
            case constants.ITEMTYPES.TEXTBOX:
                return new ASPxClientRibbonTextBoxItem(ribbon, group, index, name, type, subGroup, info.cdi, info.txt);
            case constants.ITEMTYPES.SPINEDIT:
                return new ASPxClientRibbonSpinEditItem(ribbon, group, index, name, type, subGroup, info.cdi, info.txt);
            case constants.ITEMTYPES.DATEEDIT:
                return new ASPxClientRibbonDateEditItem(ribbon, group, index, name, type, subGroup, info.cdi, info.txt);
            case constants.ITEMTYPES.TOGGLE:
                return new ASPxClientRibbonToggleItem(ribbon, group, index, name, type, subGroup, info.cdi, info.txt, info.chk);
            case constants.ITEMTYPES.COLOR:
                return new ASPxClientRibbonColorButtonItem(ribbon, group, index, name, type, subGroup, info.cdi, info.txt, info.color, info.iac);
            case constants.ITEMTYPES.TEMPLATE:
                return new ASPxClientRibbonTemplateItem(ribbon, group, index, name, type, subGroup, info.cdi, info.txt);
            case constants.ITEMTYPES.OPTION:
                return new ASPxClientRibbonOptionItem(ribbon, group, index, name, type, subGroup, info.cdi, info.txt, info.chk, info.og);
            case constants.ITEMTYPES.GALLERYDROPDOWN:
                return new ASPxClientRibbonGalleryDropDownItem(ribbon, group, index, name, type, subGroup, info.cdi, info.txt, info.i, info.nu, info.pr);
            case constants.ITEMTYPES.GALLERYBAR:
                return new ASPxClientRibbonGalleryBarItem(ribbon, group, index, name, type, subGroup, info.cdi, info.txt, info.i, info.nu, info.pr);
            default:
                return new ASPxClientRibbonItem(ribbon, group, index, name, type, subGroup, info.cdi, info.txt);
        }
    };

    ASPxClientRibbon.onTabChanged = function(s, e) {
        return getRibbonControl(s.name, constants.TABCONTROL_POSTFIX).onTabChanged(e.tab);
    };
    ASPxClientRibbon.onComboBoxValueChanged = function(s, e) {
        return getRibbonControlByItem(s.name, constants.COMBOBOX_POSTFIX, s.cpRibbonItemID).onEditorValueChanged(s.cpRibbonItemID);
    };
    ASPxClientRibbon.onDateEditValueChanged = function(s, e) {
        return getRibbonControlByItem(s.name, constants.DATEEDIT_POSTFIX, s.cpRibbonItemID).onEditorValueChanged(s.cpRibbonItemID);
    };
    ASPxClientRibbon.onSpinEditValueChanged = function(s, e) {
        return getRibbonControlByItem(s.name, constants.SPINEDIT_POSTFIX, s.cpRibbonItemID).onEditorValueChanged(s.cpRibbonItemID);
    };
    ASPxClientRibbon.onTextBoxValueChanged = function(s, e) {
        return getRibbonControlByItem(s.name, constants.TEXTBOX_POSTFIX, s.cpRibbonItemID).onEditorValueChanged(s.cpRibbonItemID);
    };
    ASPxClientRibbon.onCheckBoxCheckedChanged = function(s, e) {
        return getRibbonControlByItem(s.name, constants.CHECKBOX_POSTFIX, s.cpRibbonItemID).onEditorValueChanged(s.cpRibbonItemID);
    };

    ASPxClientRibbon.onMenuItemClick = function(s, e) {
        var itemID = s.cpRibbonItemID;
        return getRibbonControlByItem(s.name, constants.POPUP_MENU_POSTFIX, itemID).onPopupMenuItemClick(itemID, e.item);
    };
    ASPxClientRibbon.onMenuPopUp = function(s, e) {
        if(e.item.parent) return;
        var itemID = s.cpRibbonItemID;
        return getRibbonControlByItem(s.name, constants.POPUP_MENU_POSTFIX, itemID).onPopupMenuPopUp(itemID, s);
    };
    ASPxClientRibbon.onMenuCloseUp = function(s, e) {
        if(e.item.parent) return;
        var itemID = s.cpRibbonItemID;
        return getRibbonControlByItem(s.name, constants.POPUP_MENU_POSTFIX, itemID).onPopupMenuCloseUp(itemID, s);
    };
    ASPxClientRibbon.onItemPopupPopUp = function(s, e) {
        var itemID = s.cpRibbonItemID;
        return getRibbonControlByItem(s.name, constants.POPUP_CONTROL_POSTFIX, itemID).onItemPopupPopUp(itemID, s);
    };
    ASPxClientRibbon.onItemPopupCloseUp = function(s, e) {
        var itemID = s.cpRibbonItemID;
        return getRibbonControlByItem(s.name, constants.POPUP_CONTROL_POSTFIX, itemID).onItemPopupCloseUp(itemID, s);
    };

    ASPxClientRibbon.onTabClick = function(s, e) {
        return getRibbonControl(s.name, constants.TABCONTROL_POSTFIX).onTabClick(e.tab.index, e);
    };
    ASPxClientRibbon.onColorButtonPopupPopUp = function(s, e) {
        var itemID = s.cpRibbonItemID;
        return getRibbonControlByItem(s.name, constants.POPUP_CONTROL_POSTFIX, itemID).onColorButtonPopupPopUp(itemID, s);
    };
    ASPxClientRibbon.onColorTableColorChanged = function(s, e) {
        var itemID = s.cpRibbonItemID;
        return getRibbonControlByItem(s.name, constants.COLORTABLE_POSTFIX, itemID).onColorTableColorChanged(itemID);
    };
    ASPxClientRibbon.OnCNCCustomColorTableUpdated = function(s, e) {
        var itemID = s.cpRibbonItemID;
        return getRibbonControlByItem(s.name, constants.COLORTABLE_POSTFIX, itemID).onCNCCustomColorTableUpdated(itemID, s);
    };
    ASPxClientRibbon.onCNCShouldBeClosed = function(s, e) {
        var itemID = s.cpRibbonItemID;
        return getRibbonControlByItem(s.name, constants.COLORTABLE_POSTFIX, itemID).onCNCShouldBeClosed(itemID);
    };
    ASPxClientRibbon.onPopupGalleryPopUp = function(s, e) {
        var itemID = s.cpRibbonItemID;
        return getRibbonControlByItem(s.name, constants.POPUP_GALLERY_POSTFIX, itemID).onGalleryPopupPopUp(itemID, s);
    };
    ASPxClientRibbon.onPopupGalleryCloseUp = function(s, e) {
        var itemID = s.cpRibbonItemID;
        return getRibbonControlByItem(s.name, constants.POPUP_GALLERY_POSTFIX, itemID).onGalleryPopupCloseUp(itemID, s);
    };


    function getLargeItemWidth(textContainer, marginWidth, popOutWidth) {
        popOutWidth = popOutWidth === undefined ? 0 : popOutWidth;
        marginWidth = marginWidth === undefined ? 0 : marginWidth;
        
        var html = textContainer.innerHTML;
            
        var words = ASPx.GetInnerText(textContainer).split(' ');
        var wordsWidths = [];
        textContainer.style.visibility = "hidden";
        textContainer.innerHTML = "&nbsp;";
        var spaceWidth = getElementOffsetWidth(textContainer);
        var commonWidth = spaceWidth;
        for(var i = 0; i < words.length; i++) {
            var word = words[i];
            textContainer.innerHTML = word;
            var width = getElementOffsetWidth(textContainer);
            commonWidth += width;
            if(i > 0)
                commonWidth += spaceWidth;
            wordsWidths.push(width);
        }
                
        var labelWidth = getLargeItemLabelWidth(commonWidth, popOutWidth, spaceWidth, wordsWidths);
        textContainer.innerHTML = html;
        textContainer.style.visibility = "";
        return labelWidth + marginWidth;
    }
    function getLargeItemLabelWidth(firstRowWidth, secondRowWidth, spaceWidth, wordsWidths) {
        var result = Math.max(firstRowWidth, secondRowWidth);
        for(var i = wordsWidths.length - 1; i > 0; i--) {
            if(firstRowWidth - secondRowWidth > 0) {
                secondRowWidth += (secondRowWidth ? spaceWidth : 0) + wordsWidths[i];
                firstRowWidth -= wordsWidths[i] + spaceWidth;
                if(Math.max(firstRowWidth, secondRowWidth) >= result)
                    return result;
                result = Math.ceil(Math.max(firstRowWidth, secondRowWidth));
            }
        }
        return result;
    }
    function getElementOffsetWidth(element) {
        return (ASPx.Browser.IE && ASPx.Browser.MajorVersion >= 9) || ASPx.Browser.Edge ? element.getBoundingClientRect().width : element.offsetWidth;
    }
    function getCollapsedGroupWidth(group) {
        var groupElement = group.getElement();
        groupElement.className = groupElement.className.replace(constants.CLASSNAMES.GROUP, constants.CLASSNAMES.GROUP_COLLAPSED);
                
        var expandButton = group.getExpandButtonElement();
        if(!expandButton.style.width && !group.ribbon.oneLineMode) {
            var popout = group.getExpandButtonPopOutElement();
            var itemWidth = getLargeItemWidth(group.getExpandButtonLabelTextElement(), ASPx.GetLeftRightMargins(group.getExpandButtonLabelContentElement()), popout.offsetWidth + ASPx.GetLeftRightMargins(popout));
            expandButton.style.width = itemWidth + "px";
        }

        var width = groupElement.offsetWidth;
        groupElement.className = groupElement.className.replace(constants.CLASSNAMES.GROUP_COLLAPSED, constants.CLASSNAMES.GROUP);
        return width;
    }
    function getGroupExpandButtonWidth(group) {
        if(!group.ribbon.oneLineMode)
            return 0;
        var groupElement = group.getElement();
        var width = groupElement.offsetWidth;

        var expandButton = group.getExpandButtonElement();
        expandButton.className = expandButton.className.replace(constants.CLASSNAMES.ONE_LINE_MODE_GROUP_EXPAND, constants.CLASSNAMES.ONE_LINE_MODE_GROUP_EXPAND_VISIBLE);

        var deltaWidth = groupElement.offsetWidth - width;

        expandButton.className = expandButton.className.replace(constants.CLASSNAMES.ONE_LINE_MODE_GROUP_EXPAND_VISIBLE, constants.CLASSNAMES.ONE_LINE_MODE_GROUP_EXPAND);
        return deltaWidth;
    }
    function findNextModification(groupInfos) {
        var currentDelta,
            delta = Number.MAX_VALUE,
            priority = -1,
            indexPath = [-1, -1];
        for(var i = 0, groupInfo; groupInfo = groupInfos[i]; i++) {
            if(groupInfo.width < 0) continue;
            for(var j = 0, blockInfo; blockInfo = groupInfo.blockInfos[j]; j++) {
                if((blockInfo.activeMod + 1) >= blockInfo.mods.length) continue;
                var blockMod = blockInfo.mods[blockInfo.activeMod + 1];
                currentDelta = blockMod.width;

                if(blockMod.priority < priority) 
                    continue;
                if(blockMod.priority > priority || currentDelta <= delta) {
                    delta = currentDelta;
                    indexPath = [i, j];
                    priority = blockMod.priority;
                }
            }
        }
        if(indexPath[0] == -1) {
            for(var i = 0, groupInfo; groupInfo = groupInfos[i]; i++) {
                if(groupInfo.collapseWidth == -1 && groupInfo.width)
                    continue;
                currentDelta = groupInfo.width - groupInfo.collapseWidth;
                if(currentDelta > 0 && currentDelta < delta) {
                    delta = currentDelta;
                    indexPath = [i, -1];
                }
            }
        }
        return { delta: delta, indexPath: indexPath };
    };

    function getRibbonControl(controlName, controlPostfix) {
        var ribbonID = controlName.substr(0, controlName.length - controlPostfix.length);
        return ASPx.GetControlCollection().Get(ribbonID);
    }
    function getRibbonControlByItem(controlName, controlPostfix, itemID) {
        return getRibbonControl(controlName, "_" + itemID + controlPostfix);
    }
    function getItemID(item, excludeRibbon) {
        var id = "T" + item.group.tab.index + "G" + item.group.index + "I";;
        id = excludeRibbon ? id : (item.ribbon.name + "_" + id);
        if (item.type == constants.ITEMTYPES.DROPDOWNTOGGLEBUTTON || item.type == constants.ITEMTYPES.DROPDOWNSPLIT) {
            id += item.getIndexPath(true);
        } else {
            id += item.index;
        }
        return id;
    }
    var BlockMod = function(className, width, priority) {
        this.className = className;
        this.width = width;
        this.priority = priority;
    };
    var Modification = function(delta, element, newClassName, prevClassName, group, isBlock) {
        this.delta = delta;
        this.element = element;
        this.newClassName = newClassName;
        this.prevClassName = prevClassName;
        this.group = group;
        this.isBlock = isBlock;
    };
    var ASPxClientRibbonCommandExecutedEventArgs = ASPx.CreateClass(ASPxClientProcessingModeEventArgs, {
        constructor: function(item, parameter, processOnServer) {
            this.constructor.prototype.constructor.call(this, processOnServer);
            this.item = item;
            this.parameter = parameter;
        }
    });
    var ASPxClientRibbonTabEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(tab) {
            this.constructor.prototype.constructor.call(this);
            this.tab = tab;
        }
    });
    var ASPxClientRibbonMinimizationStateEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(ribbonState) {
            this.constructor.prototype.constructor.call(this);
            this.ribbonState = ribbonState;
        }
    });
    var ASPxClientRibbonDialogBoxLauncherClickedEventArgs = ASPx.CreateClass(ASPxClientProcessingModeEventArgs, {
        constructor: function(group, processOnServer) {
            this.constructor.prototype.constructor.call(this, processOnServer);
            this.group = group;
        }
    });
    var ASPxClientRibbonState = {
        Normal: 0,
        Minimized: 1,
        TemporaryShown: 2
    };

    function scItemOut(source, args) {
        var ribbon = ASPxClientRibbonCollection.find(args.item.name);
        if(ribbon && ribbon.currentDropDownItem) {
            if(args.item.name == ribbon.currentDropDownItem.id) {
                var element = ribbon.currentDropDownItem.getElement();
                var newHoverItem = args.item.Clone();
                ribbon.currentDropDownItem.hoverItem = newHoverItem;
                newHoverItem.Apply(element);
            }
        }
        else if(ribbon && ribbon.currentGroupInPopup) {
            var expButton = ribbon.currentGroupInPopup.getExpandButtonElement();
            if(args.item.name == expButton.id) {
                var newHoverItem = args.item.Clone();
                expButton.hoverItem = newHoverItem;
                newHoverItem.Apply(expButton);
            }
        }
        if(ribbon && ribbon.showTabs && ribbon.minimized && ribbon.currentTemporaryTabPageIndex > -1) {
            var tcTabIndex = ribbon.getTabControlTabIndex(ribbon.currentTemporaryTabPageIndex);
            var tabElement = ribbon.getTabControl().GetTabElement(tcTabIndex, false);
            if(args.item.name == tabElement.id) {
                var newHoverItem = args.item.Clone();
                tabElement.hoverItem = newHoverItem;
                newHoverItem.Apply(tabElement);
            }
        }
    }

    ASPx.AddAfterClearFocusedState(scItemOut);
    ASPx.AddAfterClearHoverState(scItemOut);

    window.ASPxClientRibbon = ASPxClientRibbon;
    window.ASPxClientRibbonItem = ASPxClientRibbonItem;
    window.ASPxClientRibbonCommandExecutedEventArgs = ASPxClientRibbonCommandExecutedEventArgs;
    window.ASPxClientRibbonMinimizationStateEventArgs = ASPxClientRibbonMinimizationStateEventArgs;
    window.ASPxClientRibbonDialogBoxLauncherClickedEventArgs = ASPxClientRibbonDialogBoxLauncherClickedEventArgs;
    window.ASPxClientRibbonState = ASPxClientRibbonState;
})();