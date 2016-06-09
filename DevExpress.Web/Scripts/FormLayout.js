/// <reference path="_references.js"/>

(function () {
    // Constant objects
    FormLayoutConsts = {
        FORM_LAYOUT_PARTIAL_CLASS_NAME: "dxflFormLayout",
        ITEM_SYSTEM_CLASS_NAME: "dxflItemSys",
        CAPTION_CELL_SYSTEM_CLASS_NAME: "dxflCaptionCellSys",
        ALIGNED_GROUP_SYSTEM_CLASS_NAME: "dxflAGSys",
        FULL_HEIGHT_CELL_SYSTEM_CLASS_NAME: "dxflFullHeightItemCellSys",
        HEADING_LINE_GROUP_BOX_WITH_CAPTION_SYSTEM_CLASS_NAME: "dxflWithCaptionSys",
        NESTED_CONTROL_CELL_PARTIAL_CLASS_NAME: "dxflNestedControlCell",
        HEADING_LINE_GROUP_BOX_SYSTEM_CLASS_NAME: "dxflHeadingLineGroupBoxSys",
        TABBED_GROUP_PAGE_CONTROL_SYSTEM_CLASS_NAME: "dxflPCSys",
        PAGE_CONTROL_ID_PREFIX: "PC_",
        ITEM_PATH_SEPARATOR: "_",
        NOT_FLOATED_ELEMENT_SYSTEM_CLASS_NAME: "dxflNotFloatedElSys",
        ELEMENT_CONTAINER_SYSTEM_CLASS_NAME: "dxflElConSys",
        ELEMENT_IN_ADAPTIVE_VIEW: "dxflElInAdaptiveView",
        GROUP_CHILD_ITEM_IN_FIRST_ROW_SYSTEM_CLASS_NAME: "dxflChildInFirstRowSys",
        GROUP_FIRST_CHILD_ITEM_SYSTEM_CLASS_NAME: "dxflFirstChildSys",
        GROUP_CHILD_ITEM_IN_LAST_ROW_SYSTEM_CLASS_NAME: "dxflChildInLastRowSys",
        GROUP_LAST_CHILD_ITEM_SYSTEM_CLASS_NAME: "dxflLastChildSys",
        GROUP_SYSTEM_CLASS_NAME: "dxflGroupSys",
        GROUP_BOX_SYSTEM_CLASS_NAME: "dxflGroupBoxSys",
        TABBED_GROUP_SYSTEM_CLASS_NAME: "dxflPCContainerSys",
        EMPTY_ITEM_CLASS_NAME: "dxflEmptyItem",
		GROUP_LAST_CHILD_IN_ROW_SYSTEM_CLASS_NAME: "dxflLastChildInRowSys",
		GROUP_WITHOUT_PADDINGS_SYSTEM_CLASS_NAME: "dxflNoDefaultPaddings",
		GROUP_CELL_PARTIAL_CLASS_NAME: "dxflGroupCell",
		LAYOUT_ITEM_CAPTION_PARTIAL_CLASS_NAME: "dxflCaption",
		GROUP_BOX_CAPTION_PARTIAL_CLASS_NAME: "dxflGroupBoxCaption",
        VIEW_FORM_LAYOUT_SYSTEM_CLASS_NAME: "dxflViewFormLayoutSys",
        EDIT_FORM_ITEM_SYSTEM_CLASS_NAME: "dxflEditFormItemSys",
        TEXT_ITEM_SYSTEM_CLASS_NAME: "dxflTextItemSys"
    };
    FormLayoutHorizontalCaptionsCssClasses = [ "dxflCLLSys", "dxflCLRSys" ];
    FormLayoutVerticalCaptionsCssClasses = [ "dxflCLTSys", "dxflCLBSys" ];
    FormLayoutHorizontalHelpTextsCssClasses = [ "dxflLHelpTextSys", "dxflRHelpTextSys" ];
    FormLayoutVerticalHelpTextsCssClasses = [ "dxflTHelpTextSys", "dxflBHelpTextSys" ];

    var NestedControlToItemClassNamesMap = [
        { controlClassName: "dxeTextBoxSys", itemClassName: "dxflTextEditItemSys" },
        { controlClassName: "dxeButtonEditSys", itemClassName: "dxflTextEditItemSys" },
        { controlClassName: "dxeMemoSys", itemClassName: "dxflMemoItemSys" },
        { controlClassName: "dxeBase", itemClassName: "dxflCheckBoxItemSys" }
    ];
    ASPxClientLayoutItem = ASPx.CreateClass(null, {
        constructor: function (formLayout, name, path, parent) {
            this.formLayout = formLayout;
            this.name = name;

            this.path = path;
            this.parent = parent;

            this.visible = true;
            this.clientVisible = true;
            this.isTabbedGroup = false;
            this.needAdjustContentOnShowing = true;
            this.items = [];
        },

        CreateItems: function (itemsProperties) {
            for(var i = 0; i < itemsProperties.length; i++){
                var item = this.CreateItemInstance(itemsProperties[i][0], itemsProperties[i][1]);
                item.ConfigureByProperties(itemsProperties[i]);
                this.items.push(item);
            }
        },
        CreateItemInstance: function (name, path) {
            return new ASPxClientLayoutItem(this.formLayout, name, path, this);
        },
        ConfigureByProperties: function(itemProperties){
            if(ASPx.IsExists(itemProperties[2]))
                this.visible = itemProperties[2];
            if(ASPx.IsExists(itemProperties[3]))
                this.clientVisible = itemProperties[3];
            if(ASPx.IsExists(itemProperties[4]))
                this.isTabbedGroup = itemProperties[4];
            if(ASPx.IsExists(itemProperties[5]))
                this.CreateItems(itemProperties[5]);
        },
        GetItemByName: function (name) {
            for(var i = 0; i < this.items.length; i ++)
                if(this.items[i].name == name) return this.items[i];
            for(var i = 0; i < this.items.length; i ++){
                var item = this.items[i].GetItemByName(name);
                if(item != null) return item;
            }
            return null;
        },

        GetItemByPath: function (path) {
            var pathIndexes = path.split(FormLayoutConsts.ITEM_PATH_SEPARATOR);
            var currentIndex = pathIndexes[0];
            pathIndexes.shift();
            if(currentIndex > this.items.length - 1)
                return null;
            if(pathIndexes.length > 0 && this.items[currentIndex].items.length > 0) {
                var newPath = pathIndexes.join(FormLayoutConsts.ITEM_PATH_SEPARATOR);
                var result = this.items[currentIndex].GetItemByPath(newPath);
                if(result != null)
                    return result;
            }
            else
                return pathIndexes.length > 0 ? null : this.items[currentIndex];
            return null;
        },
        GetVisible: function () {
	        return this.visible && this.clientVisible;
	    },
        SetVisible: function (value) {
            if(this.clientVisible != value) {
                this.clientVisible = value;
	            this.formLayout.SetItemVisible(this, value, false);
	        }
		},
		SetCaption: function(caption) {
            if(!this.visible)
                return;
            this.formLayout.SetItemCaption(this, caption);
		},
		GetCaption: function() {
		    if(this.visible)
		        return this.formLayout.GetItemCaption(this);
		    return "";
		},

        InitializeNeedAdjustContentOnShowing: function () {
            if(this.visible) {
                this.needAdjustContentOnShowing = !this.IsVisibleOnClient();
	            for(var i = 0; i < this.items.length; i++)
	                this.items[i].InitializeNeedAdjustContentOnShowing();
            }
	    },

        IsVisibleOnClient: function () {
            var currentItem = this;
            while(currentItem != null) {
                if(!currentItem.clientVisible)
                    return false;
                currentItem = currentItem.parent;
            }
            return true;
        },

        ResetNeedAdjustContentOnShowing: function () {
            this.needAdjustContentOnShowing = false;
        }
    });
    var ASPxClientFormLayout = ASPx.CreateClass(ASPxClientControl, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);
            this.rootItem = null;

            // Server-Provided Fields
            this.alignItemCaptionsInAllGroups = false;
            this.leftAndRightCaptionsWidth = 0;
            this.adaptivityMode = "Off";
            this.switchToSingleColumnAtWindowInnerWidth = 0;
            this.showItemCaptionColon = true;
        },

        InlineInitialize: function () {
            ASPxClientControl.prototype.InlineInitialize.call(this);

            this.InitializeNeedAdjustItemsContentOnShowing();
            this.UpdateHeadingLineGroupBoxElements();
            if(this.enabled)
                this.addHandlersToActiveTabChangedEvents();
            if(this.IsFlowRender()) {
                if(!this.IsOldIE())
                    this.CreateAdaptivityCssRules();
                var currentDocumentWidth = this.GetCurrentDocumentWidth();
                this.EnsureLayoutItemsCssClasses(currentDocumentWidth);
                this.needAlignCaptionsOnResize = currentDocumentWidth <= this.switchToSingleColumnAtWindowInnerWidth;
                this.prevDocumentWidth = currentDocumentWidth;
            }
        },

        GetScrollIsNotHidden: function() {
            if(!ASPx.IsExists(this.scrollIsNotHidden))
                this.scrollIsNotHidden = ASPx.GetCurrentStyle(document.body).overflowY !== "hidden"
                    && ASPx.GetCurrentStyle(document.documentElement).overflowY !== "hidden";
            return this.scrollIsNotHidden;
        },

        GetCurrentDocumentWidth: function() {
            var result = ASPx.GetDocumentClientWidth();
            if(!ASPx.Browser.Safari && this.GetScrollIsNotHidden() && ASPx.GetDocumentHeight() > ASPx.GetDocumentClientHeight())
                result += ASPx.GetVerticalScrollBarWidth();
            return result;
        },

        IsOldIE: function() {
            return ASPx.Browser.IE && ASPx.Browser.Version < 9;
        },

        IsFlowRender: function() {
            return this.adaptivityMode !== "Off";
        },

        CreateAdaptivityCssRules: function() {
            var styleSheet = ASPx.GetCurrentStyleSheet();
            if(!styleSheet || !styleSheet.insertRule) return;
            var rule = "@media all and (max-width: " + this.switchToSingleColumnAtWindowInnerWidth + "px) { " +
                "#" + this.name + " ." + FormLayoutConsts.ELEMENT_CONTAINER_SYSTEM_CLASS_NAME
                    + " > div { width: 100%!important; float: left; }" +
                "#" + this.name + " ." + FormLayoutConsts.CAPTION_CELL_SYSTEM_CLASS_NAME + "{ height: auto !important; }" +
                "}";
            styleSheet.insertRule(rule, styleSheet.cssRules.length);
        },

        OnBrowserWindowResize: function(evt) {
            if(this.IsFlowRender()) {
                var currentDocumentWidth = this.GetCurrentDocumentWidth();
                this.EnsureLayoutItemsCssClasses(currentDocumentWidth);
                this.EnsureCaptionsAlignedAfterResize(currentDocumentWidth);
                this.prevDocumentWidth = currentDocumentWidth;
            }
        },

        BrowserWindowResizeSubscriber: function() {
            return this.IsFlowRender();
        },

        GetLayoutItemElementsWithHorizontalCaptions: function() {
            if(!this.itemsWithHorizontalCaptions)
                this.itemsWithHorizontalCaptions = this.getFormLayoutElementNodesByPredicate(this.GetMainElement(),
                    function(element) { return this.itemHasHorizontalCaption(element); }.aspxBind(this));
            return this.itemsWithHorizontalCaptions;
        },

        GetHorizontalHelpTextElements: function() {
            if(!this.horizontalHelpTextElements)
                this.horizontalHelpTextElements = this.getFormLayoutElementNodesByPredicate(this.GetMainElement(),
                    function(element) { return this.isHorizontalHelpTextElement(element); }.aspxBind(this));
            return this.horizontalHelpTextElements;
        },

        GetGroupChildElementsInFirstRowExceptFirst: function() {
            if(!this.groupChildElementsInFirstRow)
                this.groupChildElementsInFirstRow = this.getFormLayoutElementNodesByPredicate(this.GetMainElement(),
                    function(element) {
                        return ASPx.ElementContainsCssClass(element, FormLayoutConsts.GROUP_CHILD_ITEM_IN_FIRST_ROW_SYSTEM_CLASS_NAME)
                        && !ASPx.ElementContainsCssClass(element, FormLayoutConsts.GROUP_FIRST_CHILD_ITEM_SYSTEM_CLASS_NAME);
                    });
            return this.groupChildElementsInFirstRow;
        },

        GetGroupChildElementsInLastRowExceptLast: function() {
            if(!this.groupChildElementsInLastRow)
                this.groupChildElementsInLastRow = this.getFormLayoutElementNodesByPredicate(this.GetMainElement(),
                    function(element) { 
                        return ASPx.ElementContainsCssClass(element, FormLayoutConsts.GROUP_CHILD_ITEM_IN_LAST_ROW_SYSTEM_CLASS_NAME)
                        && !ASPx.ElementContainsCssClass(element, FormLayoutConsts.GROUP_LAST_CHILD_ITEM_SYSTEM_CLASS_NAME);
                    });
            return this.groupChildElementsInLastRow;
        },

        GetGroupsWithoutPaddings: function() {
            if(!this.groupsWithoutPaddings)
                this.groupsWithoutPaddings = this.getFormLayoutElementNodesByClassName(this.GetMainElement(),
                    FormLayoutConsts.GROUP_WITHOUT_PADDINGS_SYSTEM_CLASS_NAME);
            return this.groupsWithoutPaddings;
        },

        GetGroupChildNotFirstInRowElements: function() {
            if(!this.groupChildNotFirstInRowElements) {
                this.groupChildNotFirstInRowElements = [];
                var groupsWithoutPaddings = this.GetGroupsWithoutPaddings();
                for(var i = 0; i < groupsWithoutPaddings.length; i++) {
                    for(var j = 0; j < groupsWithoutPaddings[i].childNodes.length; j++) {
                        var currentGroupChild = groupsWithoutPaddings[i].childNodes[j];
                        if(j !== 0 && !ASPx.ElementContainsCssClass(currentGroupChild, FormLayoutConsts.NOT_FLOATED_ELEMENT_SYSTEM_CLASS_NAME))
                            this.groupChildNotFirstInRowElements.push(currentGroupChild);
                    }
                }
            }
            return this.groupChildNotFirstInRowElements;
        },

        GetGroupChildNotLastInRowElements: function() {
            if(!this.groupChildNotLastInRowElements) {
                this.groupChildNotLastInRowElements = [];
                var groupsWithoutPaddings = this.GetGroupsWithoutPaddings();
                for(var i = 0; i < groupsWithoutPaddings.length; i++) {
                    for(var j = 0; j < groupsWithoutPaddings[i].childNodes.length; j++) {
                        var currentGroupChild = groupsWithoutPaddings[i].childNodes[j];
                        if(!ASPx.ElementContainsCssClass(currentGroupChild, FormLayoutConsts.GROUP_LAST_CHILD_IN_ROW_SYSTEM_CLASS_NAME))
                            this.groupChildNotLastInRowElements.push(currentGroupChild);
                    }
                }
            }
            return this.groupChildNotLastInRowElements;
        },

        GetElementsThatChangeWidthAndFloating: function() {
            if(!this.elementsThatChangeWidthAndFloating) {
                this.elementsThatChangeWidthAndFloating = [];
                var elementContainers = this.getFormLayoutElementNodesByClassName(this.GetMainElement(),
                    FormLayoutConsts.ELEMENT_CONTAINER_SYSTEM_CLASS_NAME);
                for(var i = 0; i < elementContainers.length; i++) {
                    var childDivs = ASPx.GetChildNodesByTagName(elementContainers[i], "DIV");
                    for(var j = 0; j < childDivs.length; j++)
                        this.elementsThatChangeWidthAndFloating.push(childDivs[j]);
                }
            }
            return this.elementsThatChangeWidthAndFloating;
        },

        EnsureLayoutItemsCssClasses: function(currentDocumentWidth) {
            if(!this.prevDocumentWidth && currentDocumentWidth <= this.switchToSingleColumnAtWindowInnerWidth) {
                this.UpdateItemsCssClasses(true);
                return;
            }
            if(this.IsAdaptivityWidthExceeded(currentDocumentWidth))
                this.UpdateItemsCssClasses(false);
            if(this.IsWidthReducedToAdaptivityWidth(currentDocumentWidth))
                this.UpdateItemsCssClasses(true);
        },

        EnsureCaptionsAlignedAfterResize: function(currentDocumentWidth) {
            if(this.needAlignCaptionsOnResize && this.IsAdaptivityWidthExceeded(currentDocumentWidth)) {
                this.alignGroupsInContainer(this.GetMainElement());
                this.needAlignCaptionsOnResize = false;
            }
        },

        IsAdaptivityWidthExceeded: function(currentDocumentWidth) {
            return this.prevDocumentWidth <= this.switchToSingleColumnAtWindowInnerWidth
                && currentDocumentWidth > this.switchToSingleColumnAtWindowInnerWidth;
        },

        IsWidthReducedToAdaptivityWidth: function(currentDocumentWidth) {
            return this.prevDocumentWidth > this.switchToSingleColumnAtWindowInnerWidth
                && currentDocumentWidth <= this.switchToSingleColumnAtWindowInnerWidth;
        },

        UpdateItemsCssClasses: function(fromHorizontalToVertical) {
            this.ReplaceElementsCssClasses(this.GetLayoutItemElementsWithHorizontalCaptions(),
                fromHorizontalToVertical ? FormLayoutHorizontalCaptionsCssClasses : FormLayoutVerticalCaptionsCssClasses,
                fromHorizontalToVertical ? FormLayoutVerticalCaptionsCssClasses : FormLayoutHorizontalCaptionsCssClasses);
            
            this.ReplaceElementsCssClasses(this.GetHorizontalHelpTextElements(),
                fromHorizontalToVertical ? FormLayoutHorizontalHelpTextsCssClasses : FormLayoutVerticalHelpTextsCssClasses,
                fromHorizontalToVertical ? FormLayoutVerticalHelpTextsCssClasses : FormLayoutHorizontalHelpTextsCssClasses);

            if(fromHorizontalToVertical) {
                this.RemoveCssClassFromElements(this.GetGroupChildElementsInFirstRowExceptFirst(),
                    FormLayoutConsts.GROUP_CHILD_ITEM_IN_FIRST_ROW_SYSTEM_CLASS_NAME);
                this.RemoveCssClassFromElements(this.GetGroupChildElementsInLastRowExceptLast(),
                    FormLayoutConsts.GROUP_CHILD_ITEM_IN_LAST_ROW_SYSTEM_CLASS_NAME);
                this.AddCssClassToElements(this.GetGroupChildNotFirstInRowElements(),
                    FormLayoutConsts.NOT_FLOATED_ELEMENT_SYSTEM_CLASS_NAME);
                this.AddCssClassToElements(this.GetGroupChildNotLastInRowElements(),
                    FormLayoutConsts.GROUP_LAST_CHILD_IN_ROW_SYSTEM_CLASS_NAME);
            }
            else {
                this.AddCssClassToElements(this.GetGroupChildElementsInFirstRowExceptFirst(),
                    FormLayoutConsts.GROUP_CHILD_ITEM_IN_FIRST_ROW_SYSTEM_CLASS_NAME);
                this.AddCssClassToElements(this.GetGroupChildElementsInLastRowExceptLast(),
                    FormLayoutConsts.GROUP_CHILD_ITEM_IN_LAST_ROW_SYSTEM_CLASS_NAME);
                this.RemoveCssClassFromElements(this.GetGroupChildNotFirstInRowElements(),
                    FormLayoutConsts.NOT_FLOATED_ELEMENT_SYSTEM_CLASS_NAME);
                this.RemoveCssClassFromElements(this.GetGroupChildNotLastInRowElements(),
                    FormLayoutConsts.GROUP_LAST_CHILD_IN_ROW_SYSTEM_CLASS_NAME);
            }

            if(this.IsOldIE()) {
                if(fromHorizontalToVertical)
                    this.AddCssClassToElements(this.GetElementsThatChangeWidthAndFloating(), FormLayoutConsts.ELEMENT_IN_ADAPTIVE_VIEW);
                else
                    this.RemoveCssClassFromElements(this.GetElementsThatChangeWidthAndFloating(), FormLayoutConsts.ELEMENT_IN_ADAPTIVE_VIEW);
            }
        },

        ReplaceElementsCssClasses: function(elements, oldCssClasses, newCssClasses) {
            for(var i = 0; i < elements.length; i++) {
                for(var j = 0; j < oldCssClasses.length; j++)
                    elements[i].className = elements[i].className.replace(oldCssClasses[j], newCssClasses[j]);
            }
        },

        AddCssClassToElements: function(elements, className) {
            for(var i = 0; i < elements.length; i++)
                ASPx.AddClassNameToElement(elements[i], className);
        },

        RemoveCssClassFromElements: function(elements, className) {
            for(var i = 0; i < elements.length; i++)
                ASPx.RemoveClassNameFromElement(elements[i], className);
        },

        InitializeNeedAdjustItemsContentOnShowing: function () {
	        if(this.rootItem == null) return;
            for(var i = 0; i < this.rootItem.items.length; i++)
                this.rootItem.items[i].InitializeNeedAdjustContentOnShowing();
        },

        CreateItems: function (itemsProperties) {
            this.rootItem = this.CreateRootItem();
            this.rootItem.CreateItems(itemsProperties);
        },
        CreateRootItem: function(){
            return new ASPxClientLayoutItem(this, "", "", null);
        },
        GetItemByName: function (name) {
            return this.rootItem != null ? this.rootItem.GetItemByName(name) : null;
        },

        GetItemByPath: function (path) {
            return this.rootItem != null ? this.rootItem.GetItemByPath(path) : null;
        },

        getElementToAlignGroups: function(elementToChangeState, item) {
            if(this.alignItemCaptionsInAllGroups)
                return this.GetMainElement();
            else {
                if(item.items.length > 0)
                    return elementToChangeState;
                else
                    return this.GetHTMLElementByItem(item.parent);
            }
        },

        SetItemVisible: function (item, visible, initialization) {
            if(visible && initialization) return;
            if(!item.visible) return;
            var tab = this.findItemParentTab(item);
            if(tab)
                tab.SetVisible(visible);
            else {
                var element = this.GetHTMLElementByItem(item);
                if(element) {
                    ASPx.SetElementDisplay(element, visible);
                    this.alignGroupsAfterItemStateChanged(element, item);
                    if(visible && item.needAdjustContentOnShowing) {
                        ASPx.GetControlCollection().AdjustControls(element);
                        item.ResetNeedAdjustContentOnShowing();
                    }
                }
            }
            if (this.itemsWithFullHeightExist())
                this.SetFullHeightForItems();
        },

        GetNestedControlCell: function(item) {
            var itemElementContainer = this.getItemElementContainer(item);
            return this.getFormLayoutElementNodesByPartialClassName(itemElementContainer, FormLayoutConsts.NESTED_CONTROL_CELL_PARTIAL_CLASS_NAME)[0];
        },

        getItemCaptionElement: function(item, itemParentTab) {
            var itemElementContainer = this.getItemElementContainer(item, itemParentTab);
            return this.getCaptionElementByItemContainer(itemElementContainer);
        },
        getItemCaptionInternal: function(captionElement) {
            var captionText = ASPx.GetTextNode(captionElement).nodeValue;
            if(captionText !== "" && captionText[captionText.length - 1] == ":")
                captionText = captionText.substring(0, captionText.length - 1);
            return captionText;
        },
        GetItemCaption: function(item) {
            var itemParentTab = this.findItemParentTab(item);
            if(itemParentTab)
                return itemParentTab.GetText();
            var captionElement = this.getItemCaptionElement(item);
            if(captionElement)
                return this.getItemCaptionInternal(captionElement);
            return "";
        },

        SetItemCaption: function(item, caption) {
            var itemParentTab = this.findItemParentTab(item);
            var captionElement = this.getItemCaptionElement(item, itemParentTab);
            if(itemParentTab)
                itemParentTab.SetText(caption);
            if(captionElement)
                this.setItemCaptionInternal(captionElement, caption);
            if(this.isLayoutItemCaptionElement(captionElement)) {
                this.alignGroupsAfterItemStateChanged(captionElement, item);
            }

        },

        isLayoutItemCaptionElement: function(captionElement) {
            return ASPx.ElementContainsCssClass(captionElement, FormLayoutConsts.LAYOUT_ITEM_CAPTION_PARTIAL_CLASS_NAME);
        },

        alignGroupsAfterItemStateChanged: function(element, item) {
            var elementToAlignGroups = this.getElementToAlignGroups(element, item);
            if(elementToAlignGroups)
                this.alignGroupsInContainer(elementToAlignGroups);
        },
        needToAddItemCaptionColon: function(captionElement, caption) {
			if(caption === "")
				return false;
            var captionEndsWithColon = caption[caption.length - 1] == ":";
            return this.showItemCaptionColon && this.isLayoutItemCaptionElement(captionElement) && !captionEndsWithColon;
        },
        setItemCaptionInternal: function(captionElement, caption) {
            caption = ASPx.Str.Trim(caption);
            var captionTextNode = ASPx.GetTextNode(captionElement);
            if(this.needToAddItemCaptionColon(captionElement, caption))
                caption += ":";
            captionTextNode.nodeValue = caption;
        },

        findItemParentTab: function(item) {
			if(!item.parent.isTabbedGroup)
				return null;
            var pageControlObject = this.getTabbedGroupPageControlObject(item.parent);
            if(pageControlObject)
                return pageControlObject.GetTabByName(item.path);
            return null;
        },

        getItemElementContainer: function(item, itemParentTab) {
            if(itemParentTab)
                return itemParentTab.tabControl.GetContentHolder(itemParentTab.index);
            var container = this.GetHTMLElementByItem(item);
            if(!this.IsFlowRender())
				return container;
            var groupCellElements = ASPx.GetChildNodesByPartialClassName(container, FormLayoutConsts.GROUP_CELL_PARTIAL_CLASS_NAME);
            return groupCellElements ? groupCellElements[0] : null;
        },

        getTabbedGroupPageControlObject: function(tabbedGroup) {
            var pageControlName = this.GetPageControlName(tabbedGroup);
            return ASPx.GetControlCollection().Get(pageControlName);
        },

        getCaptionElementByItemContainer: function(container) {
            var captionElement = null;
            var layoutItemElement = ASPx.GetChildByClassName(container, FormLayoutConsts.ITEM_SYSTEM_CLASS_NAME);
            if(layoutItemElement)
                captionElement = this.getLayoutItemCaptionElement(layoutItemElement);
            else {
                var groupBoxElement = ASPx.GetChildByClassName(container, FormLayoutConsts.GROUP_BOX_SYSTEM_CLASS_NAME);
                if(groupBoxElement)
                    captionElement = this.getGroupBoxCaptionElement(groupBoxElement);
            }

            return captionElement;
        },

        getLayoutItemCaptionElement: function(itemElement) {
            var captionCell = this.getFormLayoutElementNodesByClassName(itemElement,
                FormLayoutConsts.CAPTION_CELL_SYSTEM_CLASS_NAME)[0];
			if(!captionCell)
				return null;
            var captionElements = ASPx.GetChildNodesByPartialClassName(captionCell, FormLayoutConsts.LAYOUT_ITEM_CAPTION_PARTIAL_CLASS_NAME);
            return captionElements ? captionElements[0] : null;
        },

        getGroupBoxCaptionElement: function(groupBoxElement) {
            var captionElements = ASPx.GetChildNodesByPartialClassName(groupBoxElement, FormLayoutConsts.GROUP_BOX_CAPTION_PARTIAL_CLASS_NAME);
            return captionElements ? captionElements[0] : null;
        },

        GetPageControlName: function (tabbedGroup) {
            return this.GetItemElementIDPrefix(tabbedGroup) + FormLayoutConsts.PAGE_CONTROL_ID_PREFIX + tabbedGroup.path;
        },

        GetHTMLElementByItem: function(item) {
            if(item === this.rootItem)
                return this.GetMainElement();
            var itemParentTab = this.findItemParentTab(item);
            if(itemParentTab)
                return itemParentTab.tabControl.GetContentHolder(itemParentTab.index);
            return ASPx.GetElementById(this.GetItemElementID(item));
        },

        GetItemElementID: function (item) {
            return this.GetItemElementIDPrefix(item) + item.path;
        },

        GetItemElementIDPrefix: function (item) {
            var result = "";
            var currentParent = item.parent;
            while(currentParent != null) {
                if(currentParent.isTabbedGroup)
                    result = FormLayoutConsts.PAGE_CONTROL_ID_PREFIX + currentParent.path + "_" + result;
                currentParent = currentParent.parent;
            }
            return this.name + "_" + result;
        },

        UpdateHeadingLineGroupBoxElements: function () {
            var mainElement = this.GetMainElement();
            if(ASPx.IsExistsElement(mainElement)) {
                var groupBoxElements = this.getGroupBoxElements(mainElement);
                for(var i = 0; i < groupBoxElements.length; i++) {
                    var groupBoxElement = groupBoxElements[i];
                    var isHeadingLineGroupBox = groupBoxElement.className.indexOf(FormLayoutConsts.HEADING_LINE_GROUP_BOX_SYSTEM_CLASS_NAME) !== -1;
                    if(isHeadingLineGroupBox) {
                        var hasGroupBoxCaption = ASPx.GetChildNodesByTagName(groupBoxElement, "SPAN").length === 1;
                        if(hasGroupBoxCaption)
                            groupBoxElement.className += " " + FormLayoutConsts.HEADING_LINE_GROUP_BOX_WITH_CAPTION_SYSTEM_CLASS_NAME;
                    }
                }
            }
        },

        /* Adjust */
        AdjustControlCore: function () {
            this.alignGroupsInContainer(this.GetMainElement());
            this.SetFullHeightForItems();
            this.isAdjustmentPerformedAtFirst = true;
        },

        IsAdjustmentRequired: function() {
            return !this.isAdjustmentPerformedAtFirst;
        },

        SetFullHeightForItems: function() {
            var fullHeightCells = this.getItemsWithFullHeight();
            var layoutItemsWithFullHeight = [];
            for(var i = 0; i < fullHeightCells.length; i++) {
                var cellHelper = new FullHeightCellHelper(this, fullHeightCells[i]);
                cellHelper.setCellContentHeight();
                var cellContent = cellHelper.getCellContent();
                if(ASPx.ElementContainsCssClass(cellContent, FormLayoutConsts.ITEM_SYSTEM_CLASS_NAME) && !this.IsFlowRender())
                    layoutItemsWithFullHeight.push(cellContent);
                var pageControl = ASPx.GetChildByClassName(cellContent, FormLayoutConsts.TABBED_GROUP_PAGE_CONTROL_SYSTEM_CLASS_NAME);
                if(pageControl)
                    pageControl.style.height = cellHelper.getContentRequiredHeight() + "px";
            }
            if(!this.IsFlowRender())
                this.PrepareLayoutItemsWithFullHeight(layoutItemsWithFullHeight);
        },

        PrepareLayoutItemsWithFullHeight: function(layoutItemsWithFullHeight) {
            setTimeout(function() {
                for(var i = 0; i < layoutItemsWithFullHeight.length; i++) {
                    var captionCell = this.getFormLayoutElementNodesByClassName(layoutItemsWithFullHeight[i],
                        FormLayoutConsts.CAPTION_CELL_SYSTEM_CLASS_NAME)[0];
                    var nestedControlCell = this.getFormLayoutElementNodesByPartialClassName(layoutItemsWithFullHeight[i],
                        FormLayoutConsts.NESTED_CONTROL_CELL_PARTIAL_CLASS_NAME)[0];
                    if(captionCell && nestedControlCell) {
                        captionCell.style.height = "0px";
                        nestedControlCell.style.height = "100%";
                    }
                }
            }.aspxBind(this), 0);
        },

        itemsWithFullHeightExist: function () {
            return this.getItemsWithFullHeight().length > 0;
        },

        getItemsWithFullHeight: function () {
            if(!ASPx.IsExists(this.itemsWithFullHeight))
                this.itemsWithFullHeight = this.getFormLayoutElementNodesByClassName(this.GetMainElement(),
                FormLayoutConsts.FULL_HEIGHT_CELL_SYSTEM_CLASS_NAME);
            return this.itemsWithFullHeight;
        },

        addHandlersToActiveTabChangedEvents: function () {
            var pageControls = this.getFormLayoutElementNodesByClassName(this.GetMainElement(),
                FormLayoutConsts.TABBED_GROUP_PAGE_CONTROL_SYSTEM_CLASS_NAME);
            for(var i = 0; i < pageControls.length; i++) {
                var pageControlObject = ASPx.GetControlCollection().Get(pageControls[i].id);
                if(pageControlObject)
                    pageControlObject.ActiveTabChanged.AddHandler(function(s, e) {
                        this.alignGroupsInContainer(s.GetContentElement(s.GetActiveTabIndex()));
                    }.aspxBind(this));
            }
        },

        alignGroupsInContainer: function (groupContainer) {
            if(ASPx.IsExistsElement(groupContainer)) {
                if(this.leftAndRightCaptionsWidth === 0) {
                    var groupElements = this.getGroupElements(groupContainer);
                    this.alignItemCaptionsInAllGroups ? this.alignGroupsTogether(groupElements) : this.alignGroupsSeparately(groupElements);
                }
                else
                    this.alignLeftAndRightCaptionsIdentically(groupContainer);
            }
        },

        alignLeftAndRightCaptionsIdentically : function (groupContainer) {
            var captionCells = this.getFormLayoutElementNodesByClassName(groupContainer,
                FormLayoutConsts.CAPTION_CELL_SYSTEM_CLASS_NAME);
            for(var i = 0; i < captionCells.length; i++)
                if(this.isLeftOrRigthCaption(captionCells[i]))
                    this.setCaptionCellWidth(captionCells[i], this.leftAndRightCaptionsWidth);
        },

        isLeftOrRigthCaption: function (captionCell) {
            var itemElement = ASPx.GetParentByClassName(captionCell, FormLayoutConsts.ITEM_SYSTEM_CLASS_NAME);
            return this.itemHasHorizontalCaption(itemElement);
        },

        itemHasHorizontalCaption: function(itemElement) {
            for(var i = 0; i < FormLayoutHorizontalCaptionsCssClasses.length; i++)
                if(ASPx.ElementContainsCssClass(itemElement, FormLayoutHorizontalCaptionsCssClasses[i]))
                    return true;
            return false;
        },

        isHorizontalHelpTextElement: function(element) {
            for(var i = 0; i < FormLayoutHorizontalHelpTextsCssClasses.length; i++)
                if(ASPx.ElementContainsCssClass(element, FormLayoutHorizontalHelpTextsCssClasses[i]))
                    return true;
            return false;
        },

        alignGroupsTogether: function (groupElements) {
            var captionWidths = [];
            for(var i = 0; i < groupElements.length; i++)
                captionWidths = this.getGroupCaptionWidths(groupElements[i], captionWidths);
            for(var i = 0; i < groupElements.length; i++)
                this.setGroupCaptionWidths(groupElements[i], captionWidths);
        },

        alignGroupsSeparately: function (groupElements) {
            for(var i = 0; i < groupElements.length; i++)
                this.alignGroup(groupElements[i]);
        },

        getGroupCaptionWidths: function (groupElement, captionWidths) {
            var cellMatrix = this.getGroupCellMatrix(groupElement);
            var colCount = this.getCellMatrixColCount(cellMatrix);
            for(var i = 0; i < colCount; i++) {
                var cells = this.getGroupCellOnSimularLevel(cellMatrix, i);
                var maxWidth = this.getGroupCellsMaxWidth(cells);
                if(captionWidths.length <= i)
                    captionWidths.push(0);
                if(captionWidths[i] < maxWidth)
                    captionWidths[i] = maxWidth;
            }
            return captionWidths;
        },

        setGroupCaptionWidths: function(groupElement, captionWidths) {
            var cellMatrix = this.getGroupCellMatrix(groupElement);
            var colCount = this.getCellMatrixColCount(cellMatrix);
            for(var i = 0; i < colCount; i++) {
                var cells = this.getGroupCellOnSimularLevel(cellMatrix, i);
                this.setGroupCellsWidth(cells, captionWidths[i]);
            }
        },

        alignGroup: function (groupElement) {
            var cellMatrix = this.getGroupCellMatrix(groupElement);
            this.alignCaptionCellsByCellMatrix(cellMatrix);
        },

        getGroupCellMatrix: function (groupElement) {
            var cellMatrix = [];
            var groupRows = this.getRows(groupElement);

            for(var i = 0; i < groupRows.length; i++)
                cellMatrix.push([]);
            for(var i = 0; i < groupRows.length; i++)
                this.addRowToMatrix(cellMatrix, groupRows[i], i);
            return cellMatrix;
        },

        isTopLeftItemCell: function (cellMatrix, i, j) {
            if(!ASPx.IsExists(cellMatrix[i][j]))
                return false;

            var isLeft = j === 0,
                isTop = i === 0;

            isLeft = isLeft || cellMatrix[i][j] !== cellMatrix[i][j - 1];
            isTop = isTop || cellMatrix[i][j] !== cellMatrix[i - 1][j];

            return isLeft && isTop;
        },

        removeItemFromMatrix: function (cellMatrix, rowIndex, colIndex) {
            var cell = cellMatrix[rowIndex][colIndex];
            for(var i = 0; i < cellMatrix.length; i++) {
                for(var j = 0; j < cellMatrix[i].length; j++) {
                    if(cellMatrix[i][j] === cell)
                        cellMatrix[i][j] = null;
                }
            }
        },

        getItemCaptionCell: function(groupCell) {
            return this.getFormLayoutElementNodesByClassName(groupCell,
                FormLayoutConsts.CAPTION_CELL_SYSTEM_CLASS_NAME)[0];
        },

        getGroupCellOnSimularLevel: function (cellMatrix, colIndex) {
            var cells = [];
            for(var i = 0; i < cellMatrix.length; i++) {
                if(this.isTopLeftItemCell(cellMatrix, i, colIndex)) {
                    var captionCell = this.getItemCaptionCell(cellMatrix[i][colIndex]);
                    if(captionCell && this.isLeftOrRigthCaption(captionCell))
                        cells.push(cellMatrix[i][colIndex]);
                    this.removeItemFromMatrix(cellMatrix, i, colIndex);
                }
            }
            return cells;
        },

        alignCellOnSimularLevel: function (cellMatrix, colIndex) {
            var cells = this.getGroupCellOnSimularLevel(cellMatrix, colIndex);
            this.alignGroupColumnCaptionCellWidths(cells);
        },

        getCellMatrixColCount: function(cellMatrix) {
            var colCount = 0;
            for(var i = 0; i < cellMatrix.length; i++) {
                if(colCount < cellMatrix[i].length)
                    colCount = cellMatrix[i].length;
            }
            return colCount;
        },

        alignCaptionCellsByCellMatrix: function (cellMatrix) {
            for(var i = 0; i < this.getCellMatrixColCount(cellMatrix); i++)
                this.alignCellOnSimularLevel(cellMatrix, i);
        },

        addRowToMatrix: function (cellMatrix, row, rowIndex) {
            var cells = this.getRowCells(row);
            for(var i = 0; i < cells.length; i++)
                this.addCellToMatrix(cellMatrix, cells[i], rowIndex);
        },

        addCellToMatrix: function (cellMatrix, cell, rowIndex) {
            var cellRowSpan = this.getCellRowSpan(cell);
            var cellColSpan = this.getCellColSpan(cell);

            var colIndex = cellMatrix[rowIndex].length;
            for(var i = 0; i < cellMatrix[rowIndex].length; i++) {
                if(!cellMatrix[rowIndex][i]) {
                    colIndex = i;
                    break;
                }
            }

            for(var i = 0; i < cellColSpan; i++) {
                for(var j = 0; j < cellRowSpan; j++) {
                    if(j < cellMatrix.length) {
                        if(ASPx.IsElementDisplayed(cell))
                            cellMatrix[j + rowIndex][i + colIndex] = cell;
                        else
                            cellMatrix[j + rowIndex][i + colIndex] = null;
                    }
                }
            }
        },

        getGroupCellsMaxWidth: function(cellElements) {
            var maxCaptionCellWidth = 0;
            for(var i = 0; i < cellElements.length; i++) {
                var currentCellWidth = this.getGroupCellCaptionElementWindth(cellElements[i]);
                if(maxCaptionCellWidth < currentCellWidth)
                    maxCaptionCellWidth = currentCellWidth;
            }
            return maxCaptionCellWidth;
        },

        setGroupCellsWidth: function (cellElements, captionCellWidth) {
            for(var i = 0; i < cellElements.length; i++)
                this.setGroupCellCaptionElementWidth(cellElements[i], captionCellWidth);
        },

        alignGroupColumnCaptionCellWidths: function (cellElements) {
            var maxCaptionCellWidth = this.getGroupCellsMaxWidth(cellElements);
            this.setGroupCellsWidth(cellElements, maxCaptionCellWidth);
        },

        /* Utils */

        getCellColSpan: function (cellElement) {
            var result = ASPx.Attr.GetAttribute(cellElement, "colSpan");
            return result ? parseInt(result) : 1;
        },

        getCellRowSpan: function (cellElement) {
            var result = ASPx.Attr.GetAttribute(cellElement, "rowSpan");
            return result ? parseInt(result) : 1;
        },

        getGroupCellCaptionElement: function(groupCellElement) {
            var itemElements = ASPx.GetChildNodes(groupCellElement, function(child) {
                return child.className && child.className.indexOf(FormLayoutConsts.ITEM_SYSTEM_CLASS_NAME) != -1;
            });

            if(itemElements.length > 0) {
                var captionCellElements = this.getFormLayoutElementNodesByClassName(itemElements[0],
                    FormLayoutConsts.CAPTION_CELL_SYSTEM_CLASS_NAME);
                if(captionCellElements.length > 0)
                    return captionCellElements[0];
            }
            return null;
        },

        getGroupCellCaptionElementWindth: function (groupCellElement) {
            if(groupCellElement) {
                var captionCellElement = this.getGroupCellCaptionElement(groupCellElement);
                if(captionCellElement) {
                    var result = 0;
                    for(var i = 0; i < captionCellElement.childNodes.length; i++)
                        if(captionCellElement.childNodes[i].offsetWidth)
                            result += captionCellElement.childNodes[i].offsetWidth;
                    if(result)
                        return result;
                }
            }
            return null;
        },

        getGroupElements: function (parent) {
            return this.getFormLayoutElementNodesByClassName(parent,
                FormLayoutConsts.ALIGNED_GROUP_SYSTEM_CLASS_NAME);
        },

        isElementContainedInNestedFormLayout: function(element) {
            var parentFormLayout = ASPx.GetParentByPartialClassName(element, FormLayoutConsts.FORM_LAYOUT_PARTIAL_CLASS_NAME);
            return parentFormLayout !== this.GetMainElement();
        },

        getFormLayoutElementNodesByPredicate: function(parent, predicate) {
            return ASPx.GetNodes(parent, function(element) {
                return !this.isElementContainedInNestedFormLayout(element) && (!predicate || predicate(element));
            }.aspxBind(this));
        },
        getFormLayoutElementNodesByClassName: function(parent, className) {
            var result = [];
            ASPx.Data.ForEach(ASPx.GetNodesByClassName(parent, className), function(element) {
                if(!this.isElementContainedInNestedFormLayout(element))
                    result.push(element);
            }.aspxBind(this));
            return result;
        },
        getFormLayoutElementNodesByPartialClassName: function(parent, className) {
            var result = [];
            ASPx.Data.ForEach(ASPx.GetNodesByPartialClassName(parent, className), function(element) {
                if(!this.isElementContainedInNestedFormLayout(element))
                    result.push(element);
            }.aspxBind(this));
            return result;
        },
        getGroupBoxElements: function(parent) {
            return this.getFormLayoutElementNodesByClassName(parent,
                FormLayoutConsts.HEADING_LINE_GROUP_BOX_SYSTEM_CLASS_NAME);
        },

        getRowCells: function (rowElement) {
            if(this.IsFlowRender())
                return rowElement;
            return ASPx.GetChildNodesByTagName(rowElement, "TD");
        },

        getRows: function (groupElement) {
            if(this.IsFlowRender()) {
                var rows = [];
                var childDivs = ASPx.GetChildNodesByTagName(groupElement, "DIV");
                for(var i = 0; i < childDivs.length; i++) {
                    if(i === 0 || ASPx.ElementHasCssClass(childDivs[i], FormLayoutConsts.NOT_FLOATED_ELEMENT_SYSTEM_CLASS_NAME))
                        rows.push([]);
                    var groupCellElement = ASPx.GetNodeByTagName(childDivs[i], "DIV");
                    rows[rows.length - 1].push(groupCellElement);
                }
                return rows;
            }
            else {
                var tbody = ASPx.GetNodeByTagName(groupElement, "TBODY");
                return ASPx.GetChildNodesByTagName(tbody, "TR");
            }
        },

        setGroupCellCaptionElementWidth: function (groupCellElement, width) {
            if(groupCellElement) {
                var captionCellElement = this.getGroupCellCaptionElement(groupCellElement);
                if(captionCellElement)
                    this.setCaptionCellWidth(captionCellElement, width);
            }
        },

        setCaptionCellWidth: function (captionCellElement, width) {
            var captionCellPaddings = ASPx.GetLeftRightBordersAndPaddingsSummaryValue(captionCellElement);
            ASPx.SetOffsetWidth(captionCellElement, width + captionCellPaddings);
        },


        // Grid adaptive layout

        UpdateVisibleLayout: function() { // works only with root item - plain structure, table layout
            if(this.rootItem.items.length === 0 || !this.HasAnyHiddenItem())
                return;
            this.InitializeColCount();
            this.MoveItemsToTempContainer();
            this.CreateVisibleLayout();
        },

        CreateVisibleLayout: function() {
            var rootTable = ASPx.GetChildByClassName(this.GetMainElement(), FormLayoutConsts.GROUP_SYSTEM_CLASS_NAME);
            var tBody = ASPx.GetChildByTagName(rootTable, "TBODY");

            while(tBody.firstChild)
                ASPx.RemoveElement(tBody.firstChild);

            var row = null;
            var cellIndex = 0;
            for(var i = 0; i < this.rootItem.items.length; i++) {
                var item = this.rootItem.items[i];
                if(!item.GetVisible()) continue;

                if(cellIndex === 0) {
                    row = document.createElement("TR");
                    tBody.appendChild(row);
                }

                var itemCell = this.GetHTMLElementByItem(item);
                row.appendChild(itemCell);

                cellIndex++;
                if(cellIndex === this.colCount)
                    cellIndex = 0;
            }

            while(row && row.cells.length !== this.colCount)
                row.appendChild(document.createElement("TD"));
        },

        MoveItemsToTempContainer: function() {
            var container = this.GetItemsTempContainer();
            for(var i = 0; i < this.rootItem.items.length; i++) {
                var itemCell = this.GetHTMLElementByItem(this.rootItem.items[i]);
                if(itemCell.parentNode !== container)
                    container.appendChild(itemCell);
            }
        },

        HasAnyHiddenItem: function() {
            for(var i = 0; i < this.rootItem.items.length; i++) {
                if(!this.rootItem.items[i].GetVisible())
                    return true;
            }
            return this.GetItemsTempContainer().cells.length > 0;
        },
        GetItemsTempContainer: function() {
            if(!this.itemsTempContainer) {
                var table = document.createElement("TABLE");
                var tbody = document.createElement("TBODY");
                var tr = document.createElement("TR");
                ASPx.SetElementDisplay(table, false);

                table.appendChild(tbody);
                tbody.appendChild(tr);
                this.GetMainElement().appendChild(table);
                this.itemsTempContainer = tr;
            }
            return this.itemsTempContainer;
        },
        InitializeColCount: function() {
            if(ASPx.IsExists(this.colCount)) return;
            var firstItemCell = this.GetHTMLElementByItem(this.rootItem.items[0]);
            this.colCount = firstItemCell.parentNode.cells.length;
        }

    });

    ASPxClientFormLayout.UpdateNestedControlTypeClassName = function(itemElement, isEditingItem) {
        var getActualEditorTypeClassName = function(itemElement) {
            for(var i = 0; i < NestedControlToItemClassNamesMap.length; i++)
                if(ASPx.ElementHasCssClass(itemElement, NestedControlToItemClassNamesMap[i].itemClassName))
                    return NestedControlToItemClassNamesMap[i].itemClassName;            
            return "";
        };

        var getRequiredEditorTypeClassName = function(itemElement) {
            var nestedControlElement = getNestedControlElement(itemElement);
            if(ASPx.IsExists(nestedControlElement)) {
                for(var i = 0; i < NestedControlToItemClassNamesMap.length; i++)
                    if(ASPx.ElementContainsCssClass(nestedControlElement, NestedControlToItemClassNamesMap[i].controlClassName))
                        return NestedControlToItemClassNamesMap[i].itemClassName;                
            }
            return "";
        };

        var getNestedControlElement = function(itemElement) {
            var nestedControlCell = ASPx.GetNodesByPartialClassName(itemElement, FormLayoutConsts.NESTED_CONTROL_CELL_PARTIAL_CLASS_NAME)[0];
            if(!nestedControlCell) return null; // TODO check ShowCaption=false

            var nestedControlElements = ASPx.GetNodes(nestedControlCell, function(element) {
                var nestedControlObject = element.id && ASPx.GetControlCollection().Get(element.id);
                return !!nestedControlObject;
            });
            return nestedControlElements.length > 0 ? nestedControlElements[0] : null;
        };


        if(isEditingItem) {
            var editorClassName = getRequiredEditorTypeClassName(itemElement);
            if(editorClassName) {
                ASPx.AddClassNameToElement(itemElement, ASPx.FormLayoutConsts.EDIT_FORM_ITEM_SYSTEM_CLASS_NAME);

                ASPx.RemoveClassNameFromElement(itemElement, FormLayoutConsts.TEXT_ITEM_SYSTEM_CLASS_NAME);
                ASPx.AddClassNameToElement(itemElement, editorClassName);
            }
        }
        else {
            ASPx.RemoveClassNameFromElement(itemElement, ASPx.FormLayoutConsts.EDIT_FORM_ITEM_SYSTEM_CLASS_NAME);

            ASPx.RemoveClassNameFromElement(itemElement, getActualEditorTypeClassName(itemElement));
            ASPx.AddClassNameToElement(itemElement, FormLayoutConsts.TEXT_ITEM_SYSTEM_CLASS_NAME);
        }
    };

    FullHeightCellHelper = ASPx.CreateClass(null, {
        constructor: function (formLayout, cell) {
            this.row = null;
            this.cell = cell;
            this.content = null;
            this.parentGroupRows = null;
            this.formLayout = formLayout;
        },

        setCellContentHeight: function () {
            var content = this.getCellContent();
            content.style.height = "";
            var requiredHeight = this.getContentRequiredHeight();
            if(requiredHeight == 0)
                return;
            var isItem = ASPx.ElementContainsCssClass(this.getCellContent(), FormLayoutConsts.ITEM_SYSTEM_CLASS_NAME);
            var isGroupBox = ASPx.ElementContainsCssClass(content, FormLayoutConsts.GROUP_BOX_SYSTEM_CLASS_NAME);
            if(isItem) {
                var captionCell = this.formLayout.getFormLayoutElementNodesByClassName(content,
                    FormLayoutConsts.CAPTION_CELL_SYSTEM_CLASS_NAME)[0];
                if(captionCell)
                    ASPx.SetOffsetHeight(captionCell, captionCell.offsetHeight);
            }
            if(isGroupBox)
                content.style.height = requiredHeight + "px";
            else
                ASPx.SetOffsetHeight(content, requiredHeight);
            if(isItem && this.formLayout.IsFlowRender())
                this.setInternalContainerHeight();
        },       

        getVerticalHelpTextCell: function () {
            var result = this.formLayout.getFormLayoutElementNodesByPredicate(this.getCellContent(), function(element) {
                return ASPx.ElementContainsCssClass(element, FormLayoutVerticalHelpTextsCssClasses[0])
                    || ASPx.ElementContainsCssClass(element, FormLayoutVerticalHelpTextsCssClasses[1]);
            })[0];
            return result;
        },
        
        correctNestedControlCellHeight: function (captionCell) {
            var nestedControlCell = this.formLayout.getFormLayoutElementNodesByPartialClassName(this.getCellContent(), "dxflNestedControlCell")[0];
            var nestedControlCellRequiredHeight = ASPx.GetClearClientHeight(this.getCellContent()) - captionCell.offsetHeight;
            ASPx.SetOffsetHeight(nestedControlCell, nestedControlCellRequiredHeight);
        },

        correctControlCellHeight: function (verticalHelpTextCell) {
            var internalTable = this.formLayout.getFormLayoutElementNodesByPartialClassName(this.getCellContent(),"dxflInternalEditorTable")[0];
            var controlCell = ASPx.RetrieveByPredicate(ASPx.GetChildNodesByTagName(internalTable, "DIV"),
                function (element) {
                    return element != verticalHelpTextCell;
                })[0];
            var controlCellRequiredHeight = ASPx.GetClearClientHeight(internalTable) - verticalHelpTextCell.offsetHeight;
            ASPx.SetOffsetHeight(controlCell, controlCellRequiredHeight);
        },

        setInternalContainerHeight: function() {
            var captionCell = this.formLayout.getFormLayoutElementNodesByClassName(this.getCellContent(),
                FormLayoutConsts.CAPTION_CELL_SYSTEM_CLASS_NAME)[0];
            if(captionCell)
                this.correctNestedControlCellHeight(captionCell);
            var verticalHelpTextCell = this.getVerticalHelpTextCell();
            if(verticalHelpTextCell) 
                this.correctControlCellHeight(verticalHelpTextCell);
        },

        getOtherGroupRowsHeight: function () {
            var height = 0;
            var otherRows = ASPx.RetrieveByPredicate(this.getParentGroupRows(), function (item) {
                return ASPx.Data.ArrayIndexOf(item, this.cell) == -1;
            }.aspxBind(this));
            for(var i = 0; i < otherRows.length; i++) {
                height += this.getRowMaxOffsetHeight(otherRows[i]);
            }
            return height;
        },

        getParentGroupRows: function () {
            if(this.parentGroupRows == null) {
                var parentGroup = ASPx.GetParentByClassName(this.cell, FormLayoutConsts.GROUP_SYSTEM_CLASS_NAME);
                this.parentGroupRows = this.formLayout.getRows(parentGroup);
            }
            return this.parentGroupRows;
        },

        getRow: function () {
            if(this.row == null) {
                this.row = ASPx.RetrieveByPredicate(this.getParentGroupRows(), function (item) {
                    return ASPx.Data.ArrayIndexOf(item, this.cell) != -1;
                }.aspxBind(this))[0];
            }
            return this.row;
        },

        oneInRow: function () {
            return this.getRow().length == 1;
        },

        isFormLayoutElement: function (element) {
            return ASPx.ElementHasCssClass(element, FormLayoutConsts.TABBED_GROUP_SYSTEM_CLASS_NAME) ||
                ASPx.ElementHasCssClass(element, FormLayoutConsts.GROUP_BOX_SYSTEM_CLASS_NAME) ||
                ASPx.ElementHasCssClass(element, FormLayoutConsts.ITEM_SYSTEM_CLASS_NAME) ||
                ASPx.ElementHasCssClass(element, FormLayoutConsts.GROUP_SYSTEM_CLASS_NAME) ||
                ASPx.ElementContainsCssClass(element, FormLayoutConsts.EMPTY_ITEM_CLASS_NAME);
        },

        getCellContent: function () {
            if(this.content == null)
                this.content = ASPx.GetChildNodes(this.cell, this.isFormLayoutElement)[0];
            return this.content;
        },

        getContentParentHeight: function () {
            var height = 0;
            if(this.formLayout.IsFlowRender())
                height = this.getRowMaxOffsetHeight(this.getRow());
            else
                height = this.cell.offsetHeight;
            return height;
        },

        getPaddingsAndMargins: function (container) {
            return ASPx.GetTopBottomMargins(this.getCellContent())
                + ASPx.GetTopBottomBordersAndPaddingsSummaryValue(container);
        },

        calculateContentRequiredHeight: function () {
            var requiredHeight = 0;
            var parentContainer = null;
            if(this.formLayout.IsFlowRender() && this.oneInRow()) {
                parentContainer = ASPx.GetParentByClassName(this.cell, FormLayoutConsts.GROUP_SYSTEM_CLASS_NAME);
                var parentHeight = parentContainer.offsetHeight;
                var otherItemsHeight = this.getOtherGroupRowsHeight();
                requiredHeight = parentHeight - otherItemsHeight;
            } else {
                requiredHeight = this.getContentParentHeight();
                parentContainer = this.cell;
            }
            requiredHeight -= this.getPaddingsAndMargins(parentContainer);
            this.requiredHeight = requiredHeight;
        },

        getContentRequiredHeight: function () {
            if(!ASPx.IsExists(this.requiredHeight))
                this.calculateContentRequiredHeight();
            return this.requiredHeight;
        },

        getRowMaxOffsetHeight: function (row) {
            var maxOffsetHeight = 0;
            for(var i = 0; i < row.length; i++)
                if(row[i].offsetHeight > maxOffsetHeight)
                    maxOffsetHeight = row[i].offsetHeight;
            return maxOffsetHeight;
        }
    });

ASPx.FormLayoutConsts = FormLayoutConsts;

window.ASPxClientLayoutItem = ASPxClientLayoutItem;
window.ASPxClientFormLayout = ASPxClientFormLayout;
})();