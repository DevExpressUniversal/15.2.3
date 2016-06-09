/// <reference path="_references.js"/>

(function() {
var Constants = {
    MIIdSuffix: "_DXI",
    MMIdSuffix: "_DXM",
    SBIdSuffix: "_DXSB",
    SBUIdEnd: "_U",
    SBDIdEnd: "_D",
    SampleCssClassNameForImageElement: "SAMPLE_CSS_CLASS"
}

var MenuItemInfo = ASPx.CreateClass(null, {
    constructor: function(menu, indexPath) {
        var itemElement = menu.GetItemElement(indexPath);
        this.clientHeight = itemElement.clientHeight;
        this.clientWidth = itemElement.clientWidth;
        this.clientTop = ASPx.GetClientTop(itemElement);
        this.clientLeft = ASPx.GetClientLeft(itemElement);
        this.offsetHeight = itemElement.offsetHeight;
        this.offsetWidth = itemElement.offsetWidth;
        this.offsetTop = 0;
        this.offsetLeft = 0;
    }
});

var MenuCssClasses = {};
MenuCssClasses.Prefix = "dxm-";
MenuCssClasses.Menu = "dxmLite";
MenuCssClasses.BorderCorrector = "dxmBrdCor";

MenuCssClasses.Disabled = MenuCssClasses.Prefix + "disabled";
MenuCssClasses.MainMenu = MenuCssClasses.Prefix + "main";
MenuCssClasses.PopupMenu = MenuCssClasses.Prefix + "popup";

MenuCssClasses.HorizontalMenu = MenuCssClasses.Prefix + "horizontal";
MenuCssClasses.VerticalMenu = MenuCssClasses.Prefix + "vertical";
MenuCssClasses.NoWrapMenu = MenuCssClasses.Prefix + "noWrap";
MenuCssClasses.AutoWidthMenu = MenuCssClasses.Prefix + "autoWidth";

MenuCssClasses.DX = "dx";

MenuCssClasses.Separator = MenuCssClasses.Prefix + "separator";
MenuCssClasses.Spacing = MenuCssClasses.Prefix + "spacing";

MenuCssClasses.Gutter = MenuCssClasses.Prefix + "gutter";
MenuCssClasses.WithoutImages = MenuCssClasses.Prefix + "noImages";

MenuCssClasses.Item = MenuCssClasses.Prefix + "item";
MenuCssClasses.ItemHovered = MenuCssClasses.Prefix + "hovered";
MenuCssClasses.ItemSelected = MenuCssClasses.Prefix + "selected";
MenuCssClasses.ItemChecked = MenuCssClasses.Prefix + "checked";
MenuCssClasses.ItemWithoutImage = MenuCssClasses.Prefix + "noImage";
MenuCssClasses.ItemWithSubMenu = MenuCssClasses.Prefix + "subMenu";
MenuCssClasses.ItemDropDownMode = MenuCssClasses.Prefix + "dropDownMode";
MenuCssClasses.ItemWithoutSubMenu = MenuCssClasses.Prefix + "noSubMenu"; 

MenuCssClasses.AdaptiveMenuItem = MenuCssClasses.Prefix + "ami";
MenuCssClasses.AdaptiveMenuItemSpacing = MenuCssClasses.Prefix + "amis";
MenuCssClasses.AdaptiveMenu = MenuCssClasses.Prefix + "am";
MenuCssClasses.AdaptiveMenuHiddenElement = MenuCssClasses.Prefix + "amhe";

MenuCssClasses.ContentContainer = MenuCssClasses.Prefix + "content";
MenuCssClasses.Image = MenuCssClasses.Prefix + "image";
MenuCssClasses.PopOutContainer = MenuCssClasses.Prefix + "popOut";
MenuCssClasses.PopOutImage = MenuCssClasses.Prefix + "pImage";

MenuCssClasses.ImageLeft = MenuCssClasses.Prefix + "image-l";
MenuCssClasses.ImageRight = MenuCssClasses.Prefix + "image-r";
MenuCssClasses.ImageTop = MenuCssClasses.Prefix + "image-t";
MenuCssClasses.ImageBottom = MenuCssClasses.Prefix + "image-b";

MenuCssClasses.ScrollArea = MenuCssClasses.Prefix + "scrollArea";
MenuCssClasses.ScrollUpButton = MenuCssClasses.Prefix + "scrollUpBtn";
MenuCssClasses.ScrollDownButton = MenuCssClasses.Prefix + "scrollDownBtn";

MenuCssClasses.ItemClearElement = MenuCssClasses.DX + "-clear";
MenuCssClasses.ItemTextElement = MenuCssClasses.DX + "-vam";

var MenuRenderHelper = {};
// Initialization
MenuRenderHelper.InlineInitializeElements = function(menu) {
    if(!menu.isPopupMenu)
        this.InlineInitializeMainMenuElements(menu, menu.GetMainElement());

    var commonContainer = menu.GetMainElement().parentNode;
    var subMenuElements = ASPx.GetChildNodesByTagName(commonContainer, "DIV");
    for(var i = 0; i < subMenuElements.length; i++) {
        if(!menu.isPopupMenu && subMenuElements[i] == menu.GetMainElement())
            continue;
        this.InlineInitializeSubMenuElements(menu, subMenuElements[i]);
    }
};
MenuRenderHelper.InlineInitializeScrollElements = function(menu, indexPath, menuElement) {
    var scrollArea = ASPx.GetNodeByClassName(menuElement, MenuCssClasses.ScrollArea);
    if(scrollArea) scrollArea.id = menu.GetScrollAreaId(indexPath);
    var scrollUpButton = ASPx.GetNodeByClassName(menuElement, MenuCssClasses.ScrollUpButton);
    if(scrollUpButton) scrollUpButton.id = menu.GetScrollUpButtonId(indexPath);
    var scrollDownButton = ASPx.GetNodeByClassName(menuElement, MenuCssClasses.ScrollDownButton);
    if(scrollDownButton) scrollDownButton.id = menu.GetScrollDownButtonId(indexPath);
};
MenuRenderHelper.InlineInitializeMainMenuElements = function(menu, menuElement) {
    if(menu.NeedCreateItemsOnClientSide() && menu.needDropCache) {
        ASPx.CacheHelper.DropCache(menuElement);
        menu.needDropCache = false;
    }

    var contentElement = this.GetContentElement(menuElement);
    if(contentElement.className.indexOf("dxm-ti") > 1)
        menu.itemLinkMode = "TextAndImage";
    else if(contentElement.className.indexOf("dxm-t") > -1)
        menu.itemLinkMode = "TextOnly";

    var itemElements = this.GetItemElements(menuElement);
    for(var i = 0; i < itemElements.length; i++)
        this.InlineInitializeItemElement(menu, itemElements[i], "", i);
    this.InlineInitializeScrollElements(menu, "", menuElement);
};
MenuRenderHelper.InlineInitializeSubMenuElements = function(menu, parentElement) {
    parentElement.style.position = "absolute";

    var indexPath = menu.GetMenuIndexPathById(parentElement.id);

    var borderCorrectorElement = ASPx.GetNodeByClassName(parentElement, MenuCssClasses.BorderCorrector);
    if(borderCorrectorElement != null) {
        borderCorrectorElement.id = menu.GetMenuBorderCorrectorElementId(indexPath);
        borderCorrectorElement.style.position = "absolute";
        parentElement.removeChild(borderCorrectorElement);
        parentElement.parentNode.appendChild(borderCorrectorElement);
    }

    this.InlineInitializeSubMenuMenuElement(menu, parentElement);
};
MenuRenderHelper.InlineInitializeSubMenuMenuElement = function(menu, parentElement) {
    var menuElement = ASPx.GetNodeByClassName(parentElement, MenuCssClasses.PopupMenu);
    var indexPath = menu.GetMenuIndexPathById(parentElement.id);
    menuElement.id = menu.GetMenuMainElementId(indexPath);

    if(menu.NeedCreateItemsOnClientSide() && menu.needDropCache) {
        ASPx.CacheHelper.DropCache(menuElement);
        menu.needDropCache = false;
    }

    var contentElement = this.GetContentElement(menuElement);

    if(contentElement != null) {
        var itemElements = this.GetItemElements(menuElement);
        var parentIndexPath = parentElement == menu.GetMainElement() ? "" : indexPath;

        for(var i = 0; i < itemElements.length; i++) {
            var itemElementId = itemElements[i].id;
            if(itemElementId && aspxGetMenuCollection().GetMenu(itemElementId) != menu)
                continue;
            this.InlineInitializeItemElement(menu, itemElements[i], parentIndexPath, i);
        }
    }

    this.InlineInitializeScrollElements(menu, indexPath, menuElement);
};
MenuRenderHelper.HasSubMenuTemplate = function(menuElement) {
    var contentElement = this.GetContentElement(menuElement);
    return contentElement && (contentElement.tagName != "UL" || !ASPx.GetNodesByPartialClassName(contentElement, MenuCssClasses.ContentContainer).length);
};
MenuRenderHelper.InlineInitializeItemElement = function(menu, itemElement, parentIndexPath, visibleIndex) {
    function getItemIndex(visibleIndex) {
        var itemData = parentItemData[Math.max(visibleIndex, 0)];
        return itemData.constructor == Array
            ? itemData[0]
            : itemData;
    }

    var parentItemData = menu.renderData[parentIndexPath],
        prepareItemOnClick = parentItemData[visibleIndex].constructor == Array,
        indexPathPrefix = parentIndexPath + (parentIndexPath != "" ? ASPx.ItemIndexSeparator : ""),
        indexPath = indexPathPrefix + getItemIndex(visibleIndex),
        prevIndexPath = indexPathPrefix + getItemIndex(visibleIndex - 1);
    itemElement.id = menu.GetItemElementId(indexPath);
    ASPx.AssignAccessabilityEventsToChildrenLinks(itemElement);

    var separatorElement = itemElement.previousSibling;
    if(separatorElement && separatorElement.className) {
        if(ASPx.ElementContainsCssClass(separatorElement, MenuCssClasses.Spacing))
            separatorElement.id = menu.GetItemIndentElementId(indexPath);
        else if(ASPx.ElementContainsCssClass(separatorElement, MenuCssClasses.Separator))
            separatorElement.id = menu.GetItemSeparatorElementId(indexPath);
    }

    var contentElement = this.GetItemContentElement(itemElement);
    if(contentElement != null) {
        contentElement.id = menu.GetItemContentElementId(indexPath);

        var imageElement = ASPx.GetNodeByClassName(contentElement, MenuCssClasses.Image);
        if(imageElement == null) {
            var hyperLinkElement = ASPx.GetNodeByClassName(contentElement, MenuCssClasses.DX);
            if(hyperLinkElement != null)
                imageElement = ASPx.GetNodeByClassName(hyperLinkElement, MenuCssClasses.Image);
        }
        if(imageElement != null)
            imageElement.id = menu.GetItemImageId(indexPath);
    }
    else
        prepareItemOnClick = false;

    var popOutElement = this.GetItemPopOutElement(itemElement);
    if(popOutElement != null) {
        popOutElement.id = menu.GetItemPopOutElementId(indexPath);

        var popOutImageElement = ASPx.GetNodeByClassName(popOutElement, MenuCssClasses.PopOutImage);
        if(popOutImageElement != null)
            popOutImageElement.id = menu.GetItemPopOutImageId(indexPath);
    }

    if(prepareItemOnClick)
        this.InlineInitializeItemOnClick(menu, itemElement, indexPath);
};
MenuRenderHelper.InlineInitializeItemOnClick = function(menu, itemElement, indexPath) {
    var name = menu.name;
    var onclick = this.GetItemOnClick(menu, name, itemElement, indexPath);

    var assignElementOnClick = function(element, method) {
        switch(menu.itemLinkMode){
            case "ContentBounds":
                ASPx.Evt.AttachEventToElement(element, "click", method);
                break;
            case "TextOnly":
                var textElement = ASPx.GetNodeByTagName(element, "A");
                if(!textElement)
                    textElement = ASPx.GetNodeByTagName(element, "SPAN");
                if(textElement)
                    ASPx.Evt.AttachEventToElement(textElement, "click", method);
                break;
            case "TextAndImage":
                var linkElement = ASPx.GetNodeByTagName(element, "A");
                if(linkElement)
                    ASPx.Evt.AttachEventToElement(linkElement, "click", method);
                else{
                    var textElement = ASPx.GetNodeByTagName(element, "SPAN");
                    if(textElement)
                        ASPx.Evt.AttachEventToElement(textElement, "click", method);
                    var imageElement = ASPx.GetNodeByTagName(element, "IMG");
                    if(imageElement)
                        ASPx.Evt.AttachEventToElement(imageElement, "click", method);
                }
                break;
        }
    };
    if(menu.IsDropDownItem(indexPath)) {
        var contentElement = menu.GetItemContentElement(indexPath);
        var dropDownElement = menu.GetItemPopOutElement(indexPath);

        var dropDownOnclick = this.GetItemDropdownOnClick(name, itemElement, indexPath);
        assignElementOnClick(contentElement, onclick);
        assignElementOnClick(dropDownElement, dropDownOnclick);
    }
    else
        assignElementOnClick(itemElement, onclick);
};
MenuRenderHelper.GetItemOnClick = function(menu, name, itemElement, indexPath) { // B199179, B203568
    var sendPostBackHandler = function() {
        menu.SendPostBack("CLICK:" + indexPath);
    };
    var itemClickHandler = function(e) {
        ASPx.MIClick(e, name, indexPath);
    };

    var handler = menu.autoPostBack && !menu.IsClientSideEventsAssigned() && !ASPx.GetNodeByTagName(itemElement, "A")
        ? sendPostBackHandler
        : itemClickHandler;
    return function(e) {
        if(!itemElement.clientDisabled)
            handler(e);
    };
};
MenuRenderHelper.GetItemDropdownOnClick = function(name, itemElement, indexPath) {
    return function(e) {
        if(!itemElement.clientDisabled)
            ASPx.MIDDClick(e, name, indexPath);
    };
};
MenuRenderHelper.ChangeItemEnabledAttributes = function(itemElement, enabled) {
    if(itemElement) {
        itemElement.clientDisabled = !enabled;

        ASPx.Attr.ChangeStyleAttributesMethod(enabled)(itemElement, "cursor");

        var hyperLink = ASPx.GetNodeByTagName(itemElement, "A");
        if(hyperLink)
            ASPx.Attr.ChangeAttributesMethod(enabled)(hyperLink, "href");
    }
};

// Elements
MenuRenderHelper.GetContentElement = function(menuElement) {
    return ASPx.CacheHelper.GetCachedElement(this, "contentElement", 
        function() {
            var contentElement = ASPx.GetNodeByTagName(menuElement, "DIV", 0);
            if(contentElement && contentElement.className == MenuCssClasses.DX && contentElement.parentNode == menuElement) // template
                return contentElement;

            contentElement = ASPx.GetNodeByTagName(menuElement, "UL", 0);
            if(contentElement)
                return contentElement;

            return ASPx.GetNodeByTagName(menuElement, "TABLE", 0); // loading panel
        }, menuElement);
};
MenuRenderHelper.GetItemElements = function(menuElement) {
    return ASPx.CacheHelper.GetCachedElements(this, "itemElements", 
        function() {
            var contentElement = this.GetContentElement(menuElement);
            return contentElement ? ASPx.GetNodesByClassName(contentElement, MenuCssClasses.Item) : null;
        }, menuElement);
};
MenuRenderHelper.GetSpacingElements = function(menuElement) {
    return ASPx.CacheHelper.GetCachedElements(this, "spacingElements", 
        function() {
            var contentElement = this.GetContentElement(menuElement);
            return contentElement ? ASPx.GetNodesByClassName(contentElement, MenuCssClasses.Spacing) : null;
        }, menuElement);
};
MenuRenderHelper.GetSeparatorElements = function(menuElement) {
    return ASPx.CacheHelper.GetCachedElements(this, "separatorElements", 
        function() {
            var contentElement = this.GetContentElement(menuElement);
            return contentElement ? ASPx.GetNodesByClassName(contentElement, MenuCssClasses.Separator) : null;
        }, menuElement);
};
MenuRenderHelper.GetItemContentElement = function(itemElement) {
    return ASPx.CacheHelper.GetCachedElement(this, "contentElement", 
        function() {
            return ASPx.GetNodeByClassName(itemElement, MenuCssClasses.ContentContainer);
        }, itemElement);
};
MenuRenderHelper.GetItemPopOutElement = function(itemElement) {
    return ASPx.CacheHelper.GetCachedElement(this, "popOutElement", 
        function() {
            return ASPx.GetNodeByClassName(itemElement, MenuCssClasses.PopOutContainer);
        }, itemElement);
};
MenuRenderHelper.GetAdaptiveMenuItemElement = function(menuElement) {
    return ASPx.CacheHelper.GetCachedElement(this, "adaptiveMenuItemElement", 
        function() {
            var contentElement = this.GetContentElement(menuElement);
            return contentElement ? ASPx.GetNodeByClassName(contentElement, MenuCssClasses.AdaptiveMenuItem) : null;
        }, menuElement);
};
MenuRenderHelper.GetAdaptiveMenuItemSpacingElement = function(menuElement) {
    return ASPx.CacheHelper.GetCachedElement(this, "adaptiveMenuItemSpacingElement", 
        function() {
            var contentElement = this.GetContentElement(menuElement);
            return contentElement ? ASPx.GetNodeByClassName(contentElement, MenuCssClasses.AdaptiveMenuItemSpacing) : null;
        }, menuElement);
};
MenuRenderHelper.GetAdaptiveMenuElement = function(menu, menuElement) {
    return ASPx.CacheHelper.GetCachedElement(this, "adaptiveMenuElement", 
        function() {
            var adaptiveItemElement = this.GetAdaptiveMenuItemElement(menuElement);
            if(adaptiveItemElement){
                var adaptiveItemIndexPath = menu.GetIndexPathById(adaptiveItemElement.id);
                var adaptiveMenuParentElement = menu.GetMenuElement(adaptiveItemIndexPath);
                if(adaptiveMenuParentElement) 
                    return menu.GetMenuMainElement(adaptiveMenuParentElement);
            }
            return null;
        }, menuElement);
};
MenuRenderHelper.GetAdaptiveMenuContentElement = function(menu, menuElement) {
    return ASPx.CacheHelper.GetCachedElement(this, "adaptiveMenuContentElement", 
        function() {
            var adaptiveMenuElement = this.GetAdaptiveMenuElement(menu, menuElement);
            return adaptiveMenuElement ? this.GetContentElement(adaptiveMenuElement) : null;
        }, menuElement);
};

// Calculation
MenuRenderHelper.CalculateMenuControl = function(menu, menuElement, recalculate) {
    if(menuElement.offsetWidth === 0) return;

    this.PrecalculateMenuPopOuts(menuElement);

    var isVertical = menu.IsVertical("");
    var isAutoWidth = ASPx.ElementContainsCssClass(menuElement, MenuCssClasses.AutoWidthMenu);
    var isNoWrap = ASPx.ElementContainsCssClass(menuElement, MenuCssClasses.NoWrapMenu);

    var contentElement = this.GetContentElement(menuElement);
    if(menu.enableAdaptivity) 
        this.CalculateAdaptiveMainMenu(menu, menuElement, contentElement, isVertical, isAutoWidth, isNoWrap, recalculate);
    else
        this.CalculateMainMenu(menu, menuElement, contentElement, isVertical, isAutoWidth, isNoWrap, recalculate);
};
MenuRenderHelper.CalculateMainMenu = function(menu, menuElement, contentElement, isVertical, isAutoWidth, isNoWrap, recalculate) {
    var itemElements = this.GetItemElements(menuElement);
    
    this.PrecalculateMenuItems(menuElement, itemElements, recalculate);
    this.CalculateMenuItemsAutoWidth(menuElement, itemElements, isVertical, isAutoWidth);
    this.CalculateMinSize(menuElement, contentElement, itemElements, isVertical, isAutoWidth, isNoWrap, recalculate);
    this.CalculateMenuItems(menuElement, contentElement, itemElements, isVertical, recalculate);
    this.CalculateSeparatorsAndSpacers(menuElement, itemElements, contentElement, isVertical);
};
MenuRenderHelper.PrecalculateMenuPopOuts = function(menuElement) {
    if(menuElement.popOutsPreCalculated) return;

    var elements = this.GetItemElements(menuElement);
    for(var i = 0; i < elements.length; i++) {
        var popOutElement = this.GetItemPopOutElement(elements[i]);
        if(popOutElement) popOutElement.style.display = "block";
    }
    menuElement.popOutsPreCalculated = true;
};
MenuRenderHelper.PrecalculateMenuItems = function(menuElement, itemElements, recalculate) {
    if(!recalculate) return;

    for(var i = 0; i < itemElements.length; i++) {
        var itemContentElement = this.GetItemContentElement(itemElements[i]);
        if(!itemContentElement || itemContentElement.offsetWidth === 0) continue;

        ASPx.SetElementFloat(itemContentElement, "");
        ASPx.Attr.RestoreStyleAttribute(itemContentElement, "padding-left");
        ASPx.Attr.RestoreStyleAttribute(itemContentElement, "padding-right");
        
       this.ReCalculateMenuItemContent(itemElements[i], itemContentElement);
    }
};
MenuRenderHelper.ReCalculateMenuItemContent = function(itemElement, itemContentElement) {
    for(var j = 0; j < itemElement.childNodes.length; j++) {
        var child = itemElement.childNodes[j];
        if(!child.offsetWidth) continue;

        if(child !== itemContentElement) {
            if(ASPx.Browser.IE && ASPx.Browser.Version == 8)
                ASPx.Attr.RestoreStyleAttribute(child, "margin");
            else{
                ASPx.Attr.RestoreStyleAttribute(child, "margin-top");
                ASPx.Attr.RestoreStyleAttribute(child, "margin-bottom");
            }
        }
    }
};
MenuRenderHelper.CalculateMenuItemsAutoWidth = function(menuElement, itemElements, isVertical, isAutoWidth) {
    if(!isAutoWidth) return;

    for(var i = 0; i < itemElements.length; i++) 
        ASPx.Attr.RestoreStyleAttribute(itemElements[i], "width");

    if(!isVertical) {
        var autoWidthItemCount = 0;
        for(var i = 0; i < itemElements.length; i++) {
            if(ASPx.GetElementDisplay(itemElements[i]) && !ASPx.ElementHasCssClass(itemElements[i], MenuCssClasses.AdaptiveMenuItem))
                autoWidthItemCount++;
        }
        for(var i = 0; i < itemElements.length; i++) {
            if(autoWidthItemCount > 0 && !ASPx.ElementHasCssClass(itemElements[i], MenuCssClasses.AdaptiveMenuItem) && (itemElements[i].style.width === "" || itemElements[i].autoWidth)) { 
                ASPx.Attr.ChangeStyleAttribute(itemElements[i], "width", (100 / autoWidthItemCount) + "%");
                itemElements[i].autoWidth = true;
            }
        }
    }
};
MenuRenderHelper.CalculateMenuItems = function(menuElement, contentElement, itemElements, isVertical, recalculate) {
    if(contentElement.itemsCalculated && recalculate)
        contentElement.itemsCalculated = false;

    if(menuElement.offsetWidth === 0) return;

    if(contentElement.style.margin === "0px auto")
        ASPx.SetStyles(contentElement, { width: contentElement.offsetWidth, float: "none" }); // T215714

    var menuWidth = ASPx.GetCurrentStyle(menuElement).width;
    var menuRequireItemCorrection = isVertical && menuWidth;
    for(var i = 0; i < itemElements.length; i++) {
        if(!itemElements[i].style.width && !menuRequireItemCorrection) continue;

        if(ASPx.IsPercentageSize(itemElements[i].style.width) && contentElement.style.width === "")
            contentElement.style.width = "100%"; // T164516

        var itemContentElement = this.GetItemContentElement(itemElements[i]);
        if(!itemContentElement || itemContentElement.offsetWidth === 0) continue;

        if(!contentElement.itemsCalculated) {
            ASPx.Attr.RestoreStyleAttribute(itemContentElement, "padding-left");
            ASPx.Attr.RestoreStyleAttribute(itemContentElement, "padding-right");
            ASPx.SetElementFloat(itemContentElement, "none");
            var itemContentCurrentStyle = ASPx.GetCurrentStyle(itemContentElement);
            if(!isVertical || (itemContentCurrentStyle.textAlign != "center" && menuWidth)) {
                var originalPaddingLeft = parseInt(itemContentCurrentStyle.paddingLeft);
                var originalPaddingRight = parseInt(itemContentCurrentStyle.paddingRight);

                var leftChildrenWidth = 0, rightChildrenWidth = 0;
                for(var j = 0; j < itemElements[i].childNodes.length; j++) {
                    var child = itemElements[i].childNodes[j];
                    if(!child.offsetWidth) continue;

                    if(child !== itemContentElement) {
                        if(ASPx.GetElementFloat(child) === "right")
                            rightChildrenWidth += child.offsetWidth + ASPx.GetLeftRightMargins(child);
                        else if(ASPx.GetElementFloat(child) === "left")
                            leftChildrenWidth += child.offsetWidth + ASPx.GetLeftRightMargins(child);
                    }
                }
                if(leftChildrenWidth > 0 || rightChildrenWidth > 0){
                    ASPx.Attr.ChangeStyleAttribute(itemContentElement, "padding-left", (leftChildrenWidth + originalPaddingLeft) + "px");
                    ASPx.Attr.ChangeStyleAttribute(itemContentElement, "padding-right", (rightChildrenWidth + originalPaddingRight) + "px");
                }
            }
        }
        ASPx.AdjustWrappedTextInContainer(itemContentElement);
        this.CalculateMenuItemContent(itemElements[i], itemContentElement);
    }
    contentElement.itemsCalculated = true;
};
MenuRenderHelper.CalculateMenuItemContent = function(itemElement, itemContentElement) {
    var itemContentFound = false;
    for(var j = 0; j < itemElement.childNodes.length; j++) {
        var child = itemElement.childNodes[j];
        if(!child.offsetWidth) continue;

        var contentHeight = itemContentElement.offsetHeight;
        if(child !== itemContentElement) {
            if(itemContentFound){
                if(ASPx.Browser.IE && ASPx.Browser.Version == 8)
                    ASPx.Attr.ChangeStyleAttribute(child, "margin", "-" + contentHeight + "px 0 0");
                else
                    ASPx.Attr.ChangeStyleAttribute(child, "margin-top", "-" + contentHeight + "px");
            }
            else{
                if(ASPx.Browser.IE && ASPx.Browser.Version == 8)
                    ASPx.Attr.ChangeStyleAttribute(child, "margin", "0 0 -" + contentHeight + "px");
                else
                    ASPx.Attr.ChangeStyleAttribute(child, "margin-bottom", "-" + contentHeight + "px");
            }
        }
        else
            itemContentFound = true;
    }
};
MenuRenderHelper.CalculateSubMenu = function(menu, parentElement, recalculate) {
    var menuElement = menu.GetMenuMainElement(parentElement);
    var contentElement = this.GetContentElement(menuElement);
    
    if(!parentElement.isSubMenuCalculated || recalculate) {
        menuElement.style.width = "";
        menuElement.style.display = "table";
        menuElement.style.borderSpacing = "0px";
        parentElement.isSubMenuCalculated = true;
        if(contentElement.tagName === "UL") {
            if(contentElement.offsetWidth > 0) {
                if(ASPx.Browser.IE && ASPx.ElementHasCssClass(menuElement, MenuCssClasses.AdaptiveMenu))
                    menuElement.style.width = "0px";
                menuElement.style.width = contentElement.offsetWidth + "px";
                menuElement.style.display = "";
                if(ASPx.IsPercentageSize(contentElement.style.width))
                    contentElement.style.width = menuElement.style.width;
            }
            else
                parentElement.isSubMenuCalculated = false;
        }
    }
    this.CalculateSubMenuItems(menuElement, contentElement, recalculate);
};
MenuRenderHelper.CalculateSubMenuItems = function(menuElement, contentElement, recalculate) {
    var itemElements = this.GetItemElements(menuElement);
    
    this.PrecalculateMenuItems(menuElement, itemElements, recalculate);
    this.CalculateMenuItems(menuElement, contentElement, itemElements, true, recalculate);
};
MenuRenderHelper.CalculateMinSize = function(menuElement, contentElement, itemElements, isVertical, isAutoWidth, isNoWrap, recalculate) {
    if(menuElement.isMinSizeCalculated && !recalculate) return;
    
    if(isVertical) {
        menuElement.style.minWidth = "";

        ASPx.Attr.ChangeStyleAttribute(contentElement, "width", "1px");
        for(var i = 0; i < itemElements.length; i++) {
            var itemContentElement = this.GetItemContentElement(itemElements[i]);
            if(!itemContentElement || itemElements[i].offsetWidth === 0) continue;

            this.CalculateItemMinSize(itemElements[i]);
        }
        ASPx.Attr.RestoreStyleAttribute(contentElement, "width");
    }
    else {
        ASPx.RemoveClassNameFromElement(menuElement, MenuCssClasses.NoWrapMenu);
        ASPx.RemoveClassNameFromElement(menuElement, MenuCssClasses.AutoWidthMenu);

        ASPx.Attr.ChangeStyleAttribute(menuElement, "width", "1px");
        for(var i = 0; i < itemElements.length; i++) {
            var itemContentElement = this.GetItemContentElement(itemElements[i]);
            if(!itemContentElement || itemElements[i].offsetWidth === 0) continue;

            var textContainer = ASPx.GetNodeByTagName(itemContentElement, "SPAN", 0);
            if(textContainer && ASPx.GetCurrentStyle(textContainer).whiteSpace !== "nowrap")
                ASPx.AdjustWrappedTextInContainer(itemContentElement);

            this.CalculateItemMinSize(itemElements[i]);
        }

        if(isAutoWidth)
            ASPx.AddClassNameToElement(menuElement, MenuCssClasses.AutoWidthMenu);
        if(isNoWrap)
            ASPx.AddClassNameToElement(menuElement, MenuCssClasses.NoWrapMenu);

        if(isAutoWidth || isNoWrap)
            menuElement.style.minWidth = (contentElement.offsetWidth + ASPx.GetLeftRightBordersAndPaddingsSummaryValue(menuElement)) + "px";

        ASPx.Attr.RestoreStyleAttribute(menuElement, "width");
    }
    menuElement.isMinSizeCalculated = true;
};
MenuRenderHelper.CalculateItemMinSize = function(itemElement, recalculate) {
    if(itemElement.isMinSizeCalculated && !recalculate) return;

    var sizeCorrection = ASPx.Browser.HardwareAcceleration ? 1 : 0;
    itemElement.style.minWidth = "";
    var childrenWidth = 0;
    for(var j = 0; j < itemElement.childNodes.length; j++) {
        var child = itemElement.childNodes[j];
        if(!child.offsetWidth) continue;

        var float = ASPx.GetElementFloat(child);
        if(float === "none") {
            childrenWidth = child.offsetWidth;
            break;
        }
        else
            childrenWidth += child.offsetWidth;
    }
    itemElement.style.minWidth = (childrenWidth + sizeCorrection) + "px";

    itemElement.isMinSizeCalculated = true;
};
MenuRenderHelper.CalculateSeparatorsAndSpacers = function(menuElement, itemElements, contentElement, isVertical, isAutoWidth, isNoWrap) {
    var spacerElements = this.GetSpacingElements(menuElement);
    var spacerAndSeparatorElements = spacerElements.concat(this.GetSeparatorElements(menuElement));

    for(var i = 0; i < spacerAndSeparatorElements.length; i++)
        ASPx.Attr.RestoreStyleAttribute(spacerAndSeparatorElements[i], "height");

    if(!isVertical && itemElements) {
        var menuHeight = 0;
        if(!isAutoWidth && !isNoWrap) {
            for(var i=0; i < itemElements.length; i++) {
                var newHeight = itemElements[i].offsetHeight;
                if(newHeight > menuHeight)
                    menuHeight = newHeight;
            }
        }

        for(var i = 0; i < spacerAndSeparatorElements.length; i++){
            var separatorHeight = menuHeight - ASPx.GetTopBottomBordersAndPaddingsSummaryValue(spacerAndSeparatorElements[i]) - ASPx.GetTopBottomMargins(spacerAndSeparatorElements[i]);
            ASPx.Attr.ChangeStyleAttribute(spacerAndSeparatorElements[i], "height", separatorHeight + "px");
        }

        for(var i = 0; i < spacerElements.length; i++){
            if(!ASPx.ElementContainsCssClass(spacerElements[i], MenuCssClasses.AdaptiveMenuItemSpacing))
                spacerElements[i].style.minWidth = spacerElements[i].style.width; // fix width ifmenu squeezed
        }
    }
};
    
MenuRenderHelper.CalculateAdaptiveMainMenu = function(menu, menuElement, contentElement, isVertical, isAutoWidth, isNoWrap, recalculate) {
    var adaptiveItemElement = this.GetAdaptiveMenuItemElement(menuElement);
    if(!adaptiveItemElement) return;

    var adaptiveItemSpacing = this.GetAdaptiveMenuItemSpacingElement(menuElement);
    if(adaptiveItemSpacing) adaptiveItemSpacing.style.width = "";

    var adaptiveMenuElement = this.GetAdaptiveMenuElement(menu, menuElement);
    if(!adaptiveMenuElement) return;
    var adaptiveMenuContentElement = this.GetAdaptiveMenuContentElement(menu, menuElement);
    
    if(!contentElement.adaptiveInfo)
        this.InitAdaptiveInfo(contentElement);
    var wasAdaptivity = contentElement.adaptiveInfo.hasAdaptivity;
    if(wasAdaptivity)
        this.RestoreAdaptiveItems(adaptiveItemSpacing || adaptiveItemElement, contentElement, isVertical);

    if(!isVertical) {
        ASPx.SetElementDisplay(adaptiveItemElement, true);
        if(adaptiveItemSpacing) ASPx.SetElementDisplay(adaptiveItemSpacing, true);

        ASPx.RemoveClassNameFromElement(menuElement, MenuCssClasses.NoWrapMenu);
        menuElement.style.minWidth = "";

        var menuWidth = menuElement.offsetWidth - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(menuElement) - adaptiveItemElement.offsetWidth;
        if(adaptiveItemSpacing) menuWidth -= adaptiveItemSpacing.offsetWidth;

        var hasAdaptivity = this.HideAdaptiveItems(menu, menuWidth, contentElement, adaptiveMenuContentElement);
        contentElement.adaptiveInfo.hasAdaptivity = hasAdaptivity;

        ASPx.SetElementDisplay(adaptiveItemElement, hasAdaptivity);
        if(adaptiveItemSpacing) ASPx.SetElementDisplay(adaptiveItemSpacing, hasAdaptivity);
        contentElement.style.width = hasAdaptivity ? "100%" : "";
    
        if(hasAdaptivity){
            ASPx.CacheHelper.DropCache(adaptiveMenuElement);
            this.CalculateSubMenu(menu, adaptiveMenuElement, true);
            this.CalculateSeparatorsAndSpacers(adaptiveMenuElement, null, adaptiveMenuContentElement, true);
        }
        if(isNoWrap) {
            ASPx.AddClassNameToElement(menuElement, MenuCssClasses.NoWrapMenu);
            if(adaptiveItemSpacing) adaptiveItemSpacing.style.width = hasAdaptivity ? "100%" : "";
        }
    }
    else {
        ASPx.SetElementDisplay(adaptiveItemElement, false);
        if(adaptiveItemSpacing) ASPx.SetElementDisplay(adaptiveItemSpacing, false);
    }
    if(wasAdaptivity || contentElement.adaptiveInfo.hasAdaptivity)
        ASPx.CacheHelper.DropCache(menuElement);
    this.CalculateMainMenu(menu, menuElement, contentElement, isVertical, isAutoWidth, isNoWrap, wasAdaptivity || contentElement.adaptiveInfo.hasAdaptivity || recalculate);
};
MenuRenderHelper.InitAdaptiveInfo = function(contentElement) {
    if(contentElement.adaptiveInfo) return;

    contentElement.adaptiveInfo = { };
    contentElement.adaptiveInfo.elements = MenuRenderHelper.CreateAdaptiveElementsArray(contentElement);
    contentElement.adaptiveInfo.hasAdaptivity = false;
};
MenuRenderHelper.RestoreAdaptiveItems = function(previousSibling, contentElement, isVertical) {
    for(var i = 0; i < contentElement.adaptiveInfo.elements.length; i++) {
        var element = contentElement.adaptiveInfo.elements[i];
        if(ASPx.Browser.IE)
            ASPx.RemoveElement(element)
        contentElement.insertBefore(element, previousSibling);

        ASPx.Attr.RestoreStyleAttribute(element, "width");

        if(!isVertical)
            this.SetItemItemPopOutImageHorizontal(element);
    
        if(ASPx.ElementContainsCssClass(element, MenuCssClasses.Separator) || ASPx.ElementContainsCssClass(element, MenuCssClasses.Spacing))
            ASPx.RemoveClassNameFromElement(element, MenuCssClasses.AdaptiveMenuHiddenElement);
    }
};
MenuRenderHelper.CreateAdaptiveElementsArray = function(contentElement) {
    var result = [];
    var elements = ASPx.GetChildElementNodes(contentElement);
    for(var i = 0; i < elements.length; i++) {
        if(!ASPx.ElementHasCssClass(elements[i], MenuCssClasses.AdaptiveMenuItem) && !ASPx.ElementHasCssClass(elements[i], MenuCssClasses.AdaptiveMenuItemSpacing)) 
            result.push(elements[i]);
    }
    return result;
};
MenuRenderHelper.SetItemItemPopOutImageHorizontal = function(element) {
    var popOutElements = ASPx.GetNodesByPartialClassName(element, "dxWeb_mVerticalPopOut");
    for(var i = 0; i < popOutElements.length; i++)
        popOutElements[i].className = popOutElements[i].className.replace("Vertical", "Horizontal");
};
MenuRenderHelper.CheckAdaptiveItemsWidth = function(contentElement, width) {
    var itemsWidth = 0;
    var sizeCorrection = ASPx.Browser.HardwareAcceleration ? 1 : 0;
    var elements = ASPx.GetChildElementNodes(contentElement);
    for(var i = 0; i < elements.length; i++) {
        if(!ASPx.ElementHasCssClass(elements[i], MenuCssClasses.AdaptiveMenuItem) && 
            !ASPx.ElementHasCssClass(elements[i], MenuCssClasses.AdaptiveMenuItemSpacing) && elements[i].offsetWidth > 0) {
            if(elements[i].style.minWidth !== "")
                itemsWidth += parseInt(elements[i].style.minWidth) + ASPx.GetHorizontalBordersWidth(elements[i]);
            else
                itemsWidth += elements[i].offsetWidth;
        }
        if(itemsWidth > width)
            return false;
    }
    return true;
};
MenuRenderHelper.HideAdaptiveItems = function(menu, menuWidth, contentElement, adaptiveMenuContentElement) {
    if(MenuRenderHelper.CheckAdaptiveItemsWidth(contentElement, menuWidth))
        return false;

    var elementsToHide = [];
    var addToHide = function(index, itemElement, separatorElement, indentElement) {
        if(!itemElement) return;
        
        if(separatorElement && ASPx.ElementHasCssClass(separatorElement, MenuCssClasses.AdaptiveMenuItemSpacing))
            separatorElement = null;
        if(indentElement && ASPx.ElementHasCssClass(indentElement, MenuCssClasses.AdaptiveMenuItemSpacing))
            indentElement = null;

        elementsToHide[index] = { itemElement: itemElement, separatorElement: separatorElement, indentElement: indentElement };
        ASPx.Attr.ChangeStyleAttribute(itemElement, "display", "none");
        if(separatorElement)
            ASPx.Attr.ChangeStyleAttribute(separatorElement, "display", "none");
        if(indentElement)
            ASPx.Attr.ChangeStyleAttribute(indentElement, "display", "none");
    };
    for(var i = 0; i < menu.adaptiveItemsOrder.length; i++){
        var indexPath = menu.adaptiveItemsOrder[i];
        var index = parseInt(indexPath, 10);
        addToHide(index, menu.GetItemElement(indexPath), menu.GetItemSeparatorElement(indexPath), menu.GetItemIndentElement(indexPath));

        if(MenuRenderHelper.CheckAdaptiveItemsWidth(contentElement, menuWidth))
            break;
    }

    var hasImages = false;
    for(var i = 0; i < elementsToHide.length; i++) {
        if(!elementsToHide[i]) continue;

        ASPx.Attr.RestoreStyleAttribute(elementsToHide[i].itemElement, "display");
        if(elementsToHide[i].separatorElement)
            ASPx.Attr.RestoreStyleAttribute(elementsToHide[i].separatorElement, "display");
        if(elementsToHide[i].indentElement)
            ASPx.Attr.RestoreStyleAttribute(elementsToHide[i].indentElement, "display");

        if(elementsToHide[i].indentElement)
            adaptiveMenuContentElement.appendChild(elementsToHide[i].indentElement);
        if(elementsToHide[i].separatorElement)
            adaptiveMenuContentElement.appendChild(elementsToHide[i].separatorElement);
        adaptiveMenuContentElement.appendChild(elementsToHide[i].itemElement);

        ASPx.Attr.ChangeStyleAttribute(elementsToHide[i].itemElement, "width", "auto");

        if(ASPx.GetNodeByClassName(elementsToHide[i].itemElement, "dxm-image"))
            hasImages = true;
        this.SetItemPopOutImageVertical(elementsToHide[i].itemElement);
    }

    for(var i = 0; i < elementsToHide.length; i++) {
        if(!elementsToHide[i]) continue;

        if(elementsToHide[i].separatorElement) 
            ASPx.AddClassNameToElement(elementsToHide[i].separatorElement, MenuCssClasses.AdaptiveMenuHiddenElement);
        if(elementsToHide[i].indentElement) 
            ASPx.AddClassNameToElement(elementsToHide[i].indentElement, MenuCssClasses.AdaptiveMenuHiddenElement);
        break;
    }
    var elements = ASPx.GetChildElementNodes(contentElement);
    for(var i = 0; i < elements.length; i++) {
        if(ASPx.ElementContainsCssClass(elements[i], MenuCssClasses.Separator) || ASPx.ElementContainsCssClass(elements[i], MenuCssClasses.Spacing))
            ASPx.AddClassNameToElement(elements[i], MenuCssClasses.AdaptiveMenuHiddenElement);
        else
            break;
    }
    if(hasImages)
        ASPx.RemoveClassNameFromElement(adaptiveMenuContentElement, MenuCssClasses.WithoutImages);
    else
        ASPx.AddClassNameToElement(adaptiveMenuContentElement, MenuCssClasses.WithoutImages);
    return elementsToHide.length > 0;
};
MenuRenderHelper.SetItemPopOutImageVertical = function(element) {
    var popOutElements = ASPx.GetNodesByPartialClassName(element, "dxWeb_mHorizontalPopOut");
    for(var i = 0; i < popOutElements.length; i++)
        popOutElements[i].className = popOutElements[i].className.replace("Horizontal", "Vertical");
};
MenuRenderHelper.ChangeItemsPopOutImages = function(menuElement, isVertical) {
    var itemElements = this.GetItemElements(menuElement);
    for(var i = 0; i < itemElements.length; i++){
        if(isVertical)
            this.SetItemPopOutImageVertical(itemElements[i]);
        else
            this.SetItemItemPopOutImageHorizontal(itemElements[i]);
    }
};
MenuRenderHelper.ChangeOrientaion = function(isVertical, menu, menuElement) {
    var oldCssSelector = isVertical ? MenuCssClasses.HorizontalMenu : MenuCssClasses.VerticalMenu;
    var newCssSelector = isVertical ? MenuCssClasses.VerticalMenu : MenuCssClasses.HorizontalMenu;
    menuElement.className = menuElement.className.replace(oldCssSelector, newCssSelector);

    this.ChangeItemsPopOutImages(menuElement, isVertical);
    this.CalculateMenuControl(menu, menuElement, true);
    this.ChangeItemsPopOutImages(menuElement, isVertical);
};

var MenuScrollingManager = ASPx.CreateClass(ASPx.ScrollingManager, {
    constructor: function(menuScrollHelper) {
        this.constructor.prototype.constructor.call(this, menuScrollHelper, menuScrollHelper.scrollingAreaElement, [0, 1],
            function(manager, direction) {
                manager.owner.OnBeforeScrolling(direction);
            },
            function(manager, direction) {
                manager.owner.OnAfterScrolling(direction);
            }
        );
    },
    setParentNodeOverflow: function() { 
        if(ASPx.Browser.MSTouchUI) {
            this.scrollableArea.parentNode.style.overflow = "auto";
            this.scrollableArea.parentNode.style["-ms-overflow-style"] = "none";
        }  
    }
});

var MenuScrollHelper = ASPx.CreateClass(null, {
    constructor: function(menu, indexPath) {
        this.menu = menu;
        this.indexPath = indexPath;
        this.scrollingAreaElement = null;
        this.manager = null;

        this.initialized = false;
        this.visibleItems = [];
        this.itemsHeight = 0;
        this.scrollHeight = 0;
        this.scrollUpButtonHeight = 0;
        this.scrollDownButtonHeight = 0;
        this.scrollAreaHeight = null;
        this.scrollUpButtonVisible = false;
        this.scrollDownButtonVisible = false;
    },
    Initialize: function() {
        if(this.initialized) return;

        this.scrollingAreaElement = this.menu.GetScrollContentItemsContainer(this.indexPath);
        this.manager = new MenuScrollingManager(this);

        this.ShowScrollButtons();
        var scrollUpButton = this.menu.GetScrollUpButtonElement(this.indexPath);
        if(scrollUpButton) {
            this.scrollUpButtonHeight = this.GetScrollButtonHeight(scrollUpButton)
            ASPx.Selection.SetElementSelectionEnabled(scrollUpButton, false);
        }
        var scrollDownButton = this.menu.GetScrollDownButtonElement(this.indexPath);
        if(scrollDownButton) {
            this.scrollDownButtonHeight = this.GetScrollButtonHeight(scrollDownButton);
            ASPx.Selection.SetElementSelectionEnabled(scrollDownButton, false);
        }
        if(ASPx.Browser.WebKitTouchUI) {
            var preventDefault = function(event) { event.preventDefault(); };
            ASPx.Evt.AttachEventToElement(scrollUpButton, "touchstart", preventDefault);
            ASPx.Evt.AttachEventToElement(scrollDownButton, "touchstart", preventDefault);
        }
        this.HideScrollButtons();
        this.initialized = true;
    },
    GetScrollButtonHeight: function(button) {
        var style = ASPx.GetCurrentStyle(button);
        return button.offsetHeight + ASPx.PxToInt(style.marginTop) + ASPx.PxToInt(style.marginBottom);
    },
    FillVisibleItemsList: function() {
        var index = 0;
        this.visibleItems = [];
        while(true) {
            var childIndexPath = (this.indexPath != "" ? this.indexPath + ASPx.ItemIndexSeparator : "") + index;
            var itemElement = this.menu.GetItemElement(childIndexPath);
            if(itemElement == null)
                break;

            if(ASPx.GetElementDisplay(itemElement))
                this.visibleItems.push(itemElement);

            index++;
        }
    },
    CanCalculate: function() {
        return this.scrollingAreaElement && ASPx.IsElementDisplayed(this.scrollingAreaElement);
    },
    Calculate: function(scrollHeight) {
        if(!this.CanCalculate()) return;

        this.FillVisibleItemsList();

        this.itemsHeight = 0;
        this.scrollHeight = scrollHeight;
        var itemsContainer = this.menu.GetScrollContentItemsContainer(this.indexPath);
        if(itemsContainer) this.itemsHeight = itemsContainer.offsetHeight;

        this.SetPosition(0);
        this.CalculateScrollingElements(-1);
    },
    GetPosition: function() {
        return -this.manager.GetScrolledAreaPosition();
    },
    SetPosition: function(pos) {
        this.manager.SetScrolledAreaPosition(-pos);
    },
    CalculateScrollingElements: function(direction) {
        if(this.itemsHeight <= this.scrollHeight) {
            this.scrollUpButtonVisible = false;
            this.scrollDownButtonVisible = false;
            this.scrollAreaHeight = null;
            this.SetPosition(0);
        }
        else {
            var scrollTop = this.GetPosition();
            this.scrollAreaHeight = this.scrollHeight;

            if(direction > 0) {
                var showScrollUpButton = !this.scrollUpButtonVisible;
                this.scrollUpButtonVisible = true;
                this.scrollAreaHeight -= this.scrollUpButtonHeight;
                this.scrollDownButtonVisible = this.itemsHeight - this.scrollAreaHeight - scrollTop > this.scrollDownButtonHeight;
                if(this.scrollDownButtonVisible) {
                    this.scrollAreaHeight -= this.scrollDownButtonHeight;
                    if(showScrollUpButton)
                        this.SetPosition(this.GetPosition() + this.scrollUpButtonHeight);
                }
                else {
                    this.SetPosition(this.itemsHeight - this.scrollAreaHeight);
                }
            }
            else {
                this.scrollDownButtonVisible = true;
                this.scrollAreaHeight -= this.scrollDownButtonHeight;
                this.scrollUpButtonVisible = scrollTop > this.scrollUpButtonHeight;
                if(this.scrollUpButtonVisible)
                    this.scrollAreaHeight -= this.scrollUpButtonHeight;
                else
                    this.SetPosition(0);
            }
            if(this.scrollAreaHeight < 1) this.scrollAreaHeight = 1;
        }
        this.UpdateScrollingElements();
    },
    UpdateScrollingElements: function() {
        this.UpdateScrollAreaHeight();
        this.UpdateScrollButtonsVisibility();
    },
    UpdateScrollAreaHeight: function() {
        var scrollAreaElement = this.menu.GetScrollAreaElement(this.indexPath);
        if(scrollAreaElement)
            scrollAreaElement.style.height = (this.scrollAreaHeight) ? (this.scrollAreaHeight + "px") : "";
    },
    UpdateScrollButtonsVisibility: function() {
        var scrollUpButton = this.menu.GetScrollUpButtonElement(this.indexPath);
        if(scrollUpButton) ASPx.SetElementDisplay(scrollUpButton, this.scrollUpButtonVisible);
        var scrollDownButton = this.menu.GetScrollDownButtonElement(this.indexPath);
        if(scrollDownButton) ASPx.SetElementDisplay(scrollDownButton, this.scrollDownButtonVisible);
    },
    ChangeScrollButtonsVisibility: function(visible) {
        this.scrollUpButtonVisible = visible;
        this.scrollDownButtonVisible = visible;
        this.UpdateScrollButtonsVisibility();
    },
    ShowScrollButtons: function() {
        this.ChangeScrollButtonsVisibility(true);
    },
    HideScrollButtons: function() {
        this.ChangeScrollButtonsVisibility(false);
    },
    ResetScrolling: function() {
        if(!this.initialized)
            return;
        this.HideScrollButtons();
        this.SetPosition(0);
        this.scrollAreaHeight = null;
        this.UpdateScrollAreaHeight();
    },
    GetScrollAreaHeight: function() {
        var scrollAreaElement = this.menu.GetScrollAreaElement(this.indexPath);
        if(scrollAreaElement)
            return scrollAreaElement.offsetHeight;
        return 0;
    },
    OnAfterScrolling: function(direction) {
        this.CalculateScrollingElements(direction);
    },
    OnBeforeScrolling: function(direction) {
        var scrollButton = (direction > 0) ? this.menu.GetScrollDownButtonElement(this.indexPath) :
            this.menu.GetScrollUpButtonElement(this.indexPath);
        if(!scrollButton || !ASPx.GetElementDisplay(scrollButton))
            this.manager.StopScrolling();
    },
    StartScrolling: function(direction, delay, step) {
        this.manager.StartScrolling(direction, delay, step);
    },
    StopScrolling: function() {
        this.manager.StopScrolling();
    }
});
MenuScrollHelper.GetMenuByScrollButtonId = function(id) {
    var menuName = aspxGetMenuCollection().GetMenuNameBySuffixes(id, [Constants.SBIdSuffix]);
    return aspxGetMenuCollection().Get(menuName);
}
var ASPxClientMenuBase = ASPx.CreateClass(ASPxClientControl, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);

        this.createIFrames = false;
        this.renderData = null;

        this.rootMenuSample = null;

        this.sampleEmptyItemElement = null;
        this.sampleContentElement = null;
        this.sampleTextElementForItem = null;
        this.sampleImageElementForItem = null;
        this.sampleClearElement = null;
        this.samplePopOutElement = null;
        this.sampleSubMenuElement = null;
        this.sampleSpacingElement = null;
        this.sampleSeparatorElement = null;
        this.needDropCache = false;

        this.allowSelectItem = false;
        this.allowCheckItems = false;
        this.allowMultipleCallbacks = false;
        this.appearAfter = 300;
        this.slideAnimationDuration = 60;
        this.disappearAfter = 500;
        this.enableAnimation = true;
        this.enableAdaptivity = false;
        this.adaptiveItemsOrder = [];
        this.enableSubMenuFullWidth = false;
        this.checkedItems = [];
        this.isVertical = true;
        this.itemCheckedGroups = [];
        this.itemLinkMode = "ContentBounds";
        this.lockHoverEvents = false;
        this.popupToLeft = false;
        this.popupCount = 0;
        this.rootItem = null;
        this.showSubMenus = false;
        this.savedCallbackHoverItem = null;
        this.savedCallbackHoverElement = null;

        this.selectedItemIndexPath = "";
        this.checkedState = null;

        this.scrollInfo = [];
        this.scrollHelpers = {};
        this.scrollVertOffset = 1;

        this.rootSubMenuFIXOffset = 0;
        this.rootSubMenuFIYOffset = 0;
        this.rootSubMenuLIXOffset = 0;
        this.rootSubMenuLIYOffset = 0;
        this.rootSubMenuXOffset = 0;
        this.rootSubMenuYOffset = 0;
        this.subMenuFIXOffset = 0;
        this.subMenuFIYOffset = 0;
        this.subMenuLIXOffset = 0;
        this.subMenuLIYOffset = 0;
        this.subMenuXOffset = 0;
        this.subMenuYOffset = 0;
        this.maxHorizontalOverlap = -3;

        this.sizingConfig.allowSetHeight = false;
        this.ItemClick = new ASPxClientEvent();
        this.ItemMouseOver = new ASPxClientEvent();
        this.ItemMouseOut = new ASPxClientEvent();
        this.PopUp = new ASPxClientEvent();
        this.CloseUp = new ASPxClientEvent();

        aspxGetMenuCollection().Add(this);
    },

    InlineInitialize: function() {

        ASPxClientControl.prototype.InlineInitialize.call(this);
        if(!this.NeedCreateItemsOnClientSide())
            MenuRenderHelper.InlineInitializeElements(this);

        this.InitializeInternal(true);
        if(this.IsCallbacksEnabled()) {
            this.showSubMenus = this.GetLoadingPanelElement() != null;
            this.CreateCallback("DXMENUCONTENT");
        }
        else
            this.showSubMenus = true;
        this.popupToLeft = this.rtl;
    },
    InitializeInternal: function(inline) {
        if(!this.NeedCreateItemsOnClientSide()) {
            this.InitializeCheckedItems();
            this.InitializeSelectedItem();
        }
        this.InitializeEnabledAndVisible(!inline || !this.IsCallbacksEnabled());

        if(!this.IsCallbacksEnabled())
            this.InitializeScrollableMenus();
    },
    InitializeSampleMenuElement: function() {
        var wrapper = document.createElement("DIV");
        wrapper.innerHTML = this.sampleMenuHTML;
        return wrapper.childNodes[0];
    },
    InitializeMenuSamplesInternal: function(menuElement) {
        this.sampleSpacingElement = ASPx.GetNodeByClassName(menuElement, MenuCssClasses.Spacing, 0)
        this.sampleSeparatorElement = ASPx.GetNodeByClassName(menuElement, MenuCssClasses.Separator, 0);
    },
    InitializeEnabledAndVisible: function(recursive) {
        if(this.rootItem == null) return;

        for(var i = 0; i < this.rootItem.items.length; i++)
            this.rootItem.items[i].InitializeEnabledAndVisible(recursive);
    },
    InitializeScrollableMenus: function() {
        var info = eval(this.scrollInfo);

        this.scrollHelpers = {};
        for(var i = 0; i < info.length; i++)
            this.scrollHelpers[info[i]] = new MenuScrollHelper(this, info[i]);
    },
    InitializeItemSamples: function(itemsContainer) {
        this.sampleImageElementForItem =  ASPx.GetNodeByClassName(itemsContainer, MenuCssClasses.Image, 0);
        this.sampleTextElementForItem = ASPx.GetNodeByClassName(itemsContainer, MenuCssClasses.ItemTextElement, 0);
        this.sampleContentElement = ASPx.GetNodeByClassName(itemsContainer, MenuCssClasses.ContentContainer, 0);
        this.sampleContentElement.innerHTML = "";
        this.sampleClearElement  =  ASPx.GetNodeByClassName(itemsContainer, MenuCssClasses.ItemClearElement, 0);
        this.samplePopOutElement = ASPx.GetNodeByClassName(itemsContainer, MenuCssClasses.PopOutContainer, 0);
        this.sampleEmptyItemElement = ASPx.GetNodeByClassName(itemsContainer, MenuCssClasses.Item, 0);
        this.sampleEmptyItemElement.innerHTML = "";
    },
    InitializeSubMenuSample: function(subMenuElement) {
        ASPx.RemoveElement(ASPx.GetNodeByTagName(subMenuElement, "LI", 0));
        this.sampleSubMenuElement = subMenuElement;
    },
    NeedCreateItemsOnClientSide: function() {
        return false;
    },
    IsClientSideEventsAssigned: function() {
        return !this.ItemClick.IsEmpty()
            || !this.ItemMouseOver.IsEmpty()
            || !this.ItemMouseOut.IsEmpty()
            || !this.PopUp.IsEmpty()
            || !this.CloseUp.IsEmpty()
            || !this.Init.IsEmpty();
    },
    IsCallbacksEnabled: function() {
        return ASPx.IsFunction(this.callBack);
    },
    ShouldHideExistingLoadingElements: function() {
        return false;
    },
    // IDs
    GetMenuElementId: function(indexPath) {
        return this.name + Constants.MMIdSuffix + indexPath + "_";
    },
    GetMenuMainElementId: function(indexPath) {
        return this.name + "_DXME" + indexPath + "_";
    },
    GetMenuBorderCorrectorElementId: function(indexPath) {
        return this.name + "_DXMBC" + indexPath + "_";
    },
    GetMenuIFrameElementId: function(indexPath) {
        return this.name + "_DXMIF" + this.GetMenuLevel(indexPath);
    },
    GetScrollAreaId: function(indexPath) {
        return this.name + "_DXSA" + indexPath;
    },
    GetMenuTemplateContainerID: function(indexPath) {
        return this.name + "_MTCNT" + indexPath;
    },
    GetItemTemplateContainerID: function(indexPath) {
        return this.name + "_ITCNT" + indexPath;
    },
    GetScrollUpButtonId: function(indexPath) {
        return this.name + Constants.SBIdSuffix + indexPath + Constants.SBUIdEnd;
    },
    GetScrollDownButtonId: function(indexPath) {
        return this.name + Constants.SBIdSuffix + indexPath + Constants.SBDIdEnd;
    },
    GetItemElementId: function(indexPath) {
        return this.name + Constants.MIIdSuffix + indexPath + "_";
    },
    GetItemContentElementId: function(indexPath) {
        return this.GetItemElementId(indexPath) + "T";
    },
    GetItemPopOutElementId: function(indexPath) {
        return this.GetItemElementId(indexPath) + "P";
    },
    GetItemImageId: function(indexPath) {
        return this.GetItemElementId(indexPath) + "Img";
    },
    GetItemPopOutImageId: function(indexPath) {
        return this.GetItemElementId(indexPath) + "PImg";
    },
    GetItemIndentElementId: function(indexPath) {
        return this.GetItemElementId(indexPath) + "II";
    },
    GetItemSeparatorElementId: function(indexPath) {
        return this.GetItemElementId(indexPath) + "IS";
    },

    // Elements
    GetMenuElement: function (indexPath) { //T268967
        if(indexPath == "")
            return this.GetMainElement();
        return ASPx.CacheHelper.GetCachedElementById(this, this.GetMenuElementId(indexPath));
    },
    GetMenuIFrameElement: function(indexPath) {
        var elementId = this.GetMenuIFrameElementId(indexPath);
        var element = ASPx.GetElementById(elementId);
        if(!element && this.createIFrames)
            return this.CreateIFrameElement(elementId);
        return element;
    },
    CreateIFrameElement: function(elementId) {
        var element = document.createElement("IFRAME");
        ASPx.Attr.SetAttribute(element, "id", elementId);
        ASPx.Attr.SetAttribute(element, "src", "javascript:false");
        ASPx.Attr.SetAttribute(element, "scrolling", "no");
        ASPx.Attr.SetAttribute(element, "frameborder", "0");
        if(ASPx.IsExists(ASPx.AccessibilitySR.AccessibilityIFrameTitle))
            ASPx.Attr.SetAttribute(element, "title", ASPx.AccessibilitySR.AccessibilityIFrameTitle);
        element.style.position = "absolute";
        element.style.display = "none";
        element.style.zIndex = "19997";
        element.style.filter = "progid:DXImageTransform.Microsoft.Alpha(Style=0, Opacity=0)";
        ASPx.InsertElementAfter(element, this.GetMainElement());
        return element;
    },
    GetMenuBorderCorrectorElement: function(indexPath) {
        return ASPx.CacheHelper.GetCachedElementById(this, this.GetMenuBorderCorrectorElementId(indexPath));
    },
    GetMenuMainElement: function(element) {
        var indexPath = this.GetIndexPathById(element.id, true);
        return ASPx.CacheHelper.GetCachedElement(this, "menuMainElement" + indexPath, 
            function() { 
                var shadowTable = ASPx.GetElementById(this.GetMenuMainElementId(indexPath));
                return shadowTable != null ? shadowTable : element;
            });
    },
    GetScrollAreaElement: function(indexPath) {
        return ASPx.CacheHelper.GetCachedElementById(this, this.GetScrollAreaId(indexPath));
    },
    GetScrollContentItemsContainer: function(indexPath) {
        return ASPx.CacheHelper.GetCachedElement(this, "scrollContentItemsContainer" + indexPath, 
            function() { 
                return ASPx.GetNodeByTagName(this.GetScrollAreaElement(indexPath), "UL", 0);
            });
    },
    GetScrollUpButtonElement: function(indexPath) {
        return ASPx.CacheHelper.GetCachedElementById(this, this.GetScrollUpButtonId(indexPath));
    },
    GetScrollDownButtonElement: function(indexPath) {
        return ASPx.CacheHelper.GetCachedElementById(this, this.GetScrollDownButtonId(indexPath));
    },
    GetItemElement: function(indexPath) {
        return ASPx.CacheHelper.GetCachedElementById(this, this.GetItemElementId(indexPath));
    },
    GetItemTemplateElement: function(indexPath) { // Obsolete, use GetItemTextTemplateContainer instead
        return this.GetItemTextTemplateContainer(indexPath);
    },
    GetItemTemplateContainer: function(indexPath) {
        return this.GetItemElement(indexPath);
    },
    GetItemTextTemplateContainer: function(indexPath) {
        return this.GetItemContentElement(indexPath);
    },
    GetItemContentElement: function(indexPath) {
        return ASPx.CacheHelper.GetCachedElementById(this, this.GetItemContentElementId(indexPath));
    },
    GetItemPopOutElement: function(indexPath) {
        return ASPx.CacheHelper.GetCachedElementById(this, this.GetItemPopOutElementId(indexPath));
    },

    GetPopOutElements: function() {
        return ASPx.GetNodesByClassName(this.GetMainElement().parentNode, "dxm-popOut");
    },
    GetPopOutImages: function() {
        return ASPx.GetNodesByClassName(this.GetMainElement().parentNode, "dxm-pImage");
    },

    GetSubMenuXPosition: function(indexPath, isVertical) {
        var itemElement = this.GetItemElement(indexPath);
        var pos = ASPx.GetAbsoluteX(itemElement) + (isVertical ? itemElement.clientWidth + itemElement.clientLeft : 0);
        if(ASPx.Browser.WebKitFamily && !this.IsParentElementPositionStatic(indexPath))
            pos -= document.body.offsetLeft;
        return pos;
    },
    GetSubMenuYPosition: function(indexPath, isVertical) {
        var position = 0;
        var element = this.GetItemElement(indexPath);
        if(element != null) {
            if(isVertical) {
                position = ASPx.GetAbsoluteY(element); // The best solution
            }
            else {
                if(ASPx.Browser.NetscapeFamily || ASPx.Browser.Opera && ASPx.Browser.Version >= 9 || ASPx.Browser.Safari && ASPx.Browser.Version >= 3 || ASPx.Browser.Chrome || ASPx.Browser.AndroidDefaultBrowser)
                    position = ASPx.GetAbsoluteY(element) + element.offsetHeight - ASPx.GetClientTop(element);
                else if(ASPx.Browser.WebKitFamily)
                    position = ASPx.GetAbsoluteY(element) + element.offsetHeight + element.offsetTop - ASPx.GetClientTop(element);
                else
                    position = ASPx.GetAbsoluteY(element) + element.clientHeight + ASPx.GetClientTop(element);
            }
        }
        if(ASPx.Browser.WebKitFamily && !this.IsParentElementPositionStatic(indexPath))
            position -= document.body.offsetTop;
        return position;
    },
    GetClientSubMenuXPosition: function(element, x, indexPath, isVertical) {
        var itemInfo = new MenuItemInfo(this, indexPath);
        var itemWidth = itemInfo.clientWidth;
        var itemOffsetWidth = itemInfo.offsetWidth;
        var subMenuWidth = this.GetMenuMainElement(element).offsetWidth;
        var docClientWidth = ASPx.GetDocumentClientWidth();

        if(isVertical) {
            var left = x - ASPx.GetDocumentScrollLeft();
            var right = left + subMenuWidth;
            var toLeftX = x - subMenuWidth - itemWidth;
            var toLeftLeft = left - subMenuWidth - itemWidth;
            var toLeftRight = right - subMenuWidth - itemWidth;
            if(this.IsCorrectionDisableMethodRequired(indexPath)) {
                return this.GetCorrectionDisabledResult(x, toLeftX);
            }
            if(this.popupToLeft) {
                if(toLeftLeft > this.maxHorizontalOverlap) {
                    return toLeftX;
                }
                if(docClientWidth - right > this.maxHorizontalOverlap || !this.rtl) {
                    this.popupToLeft = false;
                    return x;
                }
                if(isVertical)
                    return ASPx.InvalidPosition;
                return toLeftX;
            }
            else {
                if(docClientWidth - right > this.maxHorizontalOverlap) {
                    return x;
                }
                if(toLeftLeft > this.maxHorizontalOverlap || this.rtl) {
                    this.popupToLeft = true;
                    return toLeftX;
                }
                if(isVertical)
                    return ASPx.InvalidPosition;
                return x;
            }
        }
        else {
            var left = x - ASPx.GetDocumentScrollLeft();
            var right = left + subMenuWidth;
            var toLeftX = x - subMenuWidth + itemOffsetWidth;
            var toLeftLeft = left - subMenuWidth + itemOffsetWidth;
            var toLeftRight = right - subMenuWidth + itemOffsetWidth;
            if(this.popupToLeft) {
                if(toLeftLeft < 0 && toLeftLeft < docClientWidth - right) {
                    this.popupToLeft = false;
                    return x;
                }
                else
                    return toLeftX;
            }
            else {
                if(docClientWidth - right < 0 && docClientWidth - right < toLeftLeft) {
                    this.popupToLeft = true;
                    return toLeftX;
                }
                else
                    return x;
            }
        }
    },
    GetClientSubMenuYPosition: function(element, y, indexPath, isVertical) {
        var itemInfo = new MenuItemInfo(this, indexPath);
        var itemHeight = itemInfo.offsetHeight;
        var itemOffsetHeight = itemInfo.offsetHeight;
        var subMenuHeight = this.GetMenuMainElement(element).offsetHeight;

        var menuItemTop = y - ASPx.GetDocumentScrollTop();
        var subMenuBottom = menuItemTop + subMenuHeight;
        var docClientHeight = ASPx.GetDocumentClientHeight();
        var clientSubMenuYPos = y;

        if(isVertical) {
            var notEnoughSpaceToShowDown = subMenuBottom > docClientHeight;

            var menuItemBottom = menuItemTop + itemHeight;
            if(menuItemBottom > docClientHeight) {
                menuItemBottom = docClientHeight;
                itemHeight = menuItemBottom - menuItemTop;
            }
            var notEnoughSpaceToShowUp = menuItemBottom < subMenuHeight;
            var subMenuIsFitToDisplayFrames = docClientHeight >= subMenuHeight;

            if(!subMenuIsFitToDisplayFrames) clientSubMenuYPos = y - menuItemTop;
            else if(notEnoughSpaceToShowDown) {
                if(notEnoughSpaceToShowUp) {
                    var docClientBottom = ASPx.GetDocumentScrollTop() + docClientHeight;
                    clientSubMenuYPos = docClientBottom - subMenuHeight;
                } else
                    clientSubMenuYPos = y + itemHeight - subMenuHeight;
            }
        }
        else {
            if(this.IsHorizontalSubmenuNeedInversion(subMenuBottom, docClientHeight, menuItemTop, subMenuHeight, itemHeight))
                clientSubMenuYPos = y - subMenuHeight - itemHeight;
        }
        return clientSubMenuYPos;
    },

    IsHorizontalSubmenuNeedInversion: function(subMenuBottom, docClientHeight, menuItemTop, subMenuHeight, itemHeight) {
        return subMenuBottom > docClientHeight && menuItemTop - subMenuHeight - itemHeight > docClientHeight - subMenuBottom;
    },

    IsCorrectionDisableMethodRequired: function(indexPath) {
        return false;
    },

    HasChildren: function(indexPath) {
        return (this.GetMenuElement(indexPath) != null);
    },
    IsVertical: function(indexPath) {
        return true;
    },
    IsRootItem: function(indexPath) {
        return this.GetMenuLevel(indexPath) <= 1;
    },
    IsParentElementPositionStatic: function(indexPath) {
        return this.IsRootItem(indexPath);
    },

    GetItemIndexPath: function(indexes) {
        return aspxGetMenuCollection().GetItemIndexPath(indexes);
    },
    GetItemIndexes: function(indexPath) {
        return aspxGetMenuCollection().GetItemIndexes(indexPath);
    },
    GetItemIndexPathById: function(id) {
        return aspxGetMenuCollection().GetIndexPathById(id, Constants.MIIdSuffix);
    },
    GetMenuIndexPathById: function(id) {
        return aspxGetMenuCollection().GetIndexPathById(id, Constants.MMIdSuffix);
    },
    GetScrollButtonIndexPathById: function(id) {
        return aspxGetMenuCollection().GetIndexPathById(id, Constants.SBIdSuffix);
    },
    GetIndexPathById: function(id, checkMenu) {
        var indexPath = this.GetItemIndexPathById(id);
        if(indexPath == "" && checkMenu)
            indexPath = this.GetMenuIndexPathById(id);
        return indexPath;
    },
    GetMenuLevelInternal: function(indexPath) {
        if(indexPath == "")
            return 0;
        else {
            var indexes = this.GetItemIndexes(indexPath);
            return indexes.length;
        }
    },
    GetMenuLevel: function(indexPath) {
        var level = this.GetMenuLevelInternal(indexPath);
        if(this.IsAdaptiveMenuItem(indexPath))
            level ++;
        return level;
    },
    IsAdaptiveMenuItem: function(indexPath){
        var level = this.GetMenuLevelInternal(indexPath);
        while(level > 1){
            indexPath = this.GetParentIndexPath(indexPath);
            level = this.GetMenuLevelInternal(indexPath);
        }
        var itemElement = this.GetItemElement(indexPath);
        if(itemElement && ASPx.GetParentByClassName(itemElement, MenuCssClasses.AdaptiveMenu))
            return true;
        return false;
    },
    IsAdaptiveItem: function(indexPath){
        var itemElement = this.GetItemElement(indexPath);
        if(itemElement && ASPx.ElementContainsCssClass(itemElement, MenuCssClasses.AdaptiveMenuItem))
            return true;
        return false;
    },

    GetParentIndexPath: function(indexPath) {
        var indexes = this.GetItemIndexes(indexPath);
        indexes.length--;
        return (indexes.length > 0) ? this.GetItemIndexPath(indexes) : "";
    },
    GetFirstChildIndexPath: function(indexPath) {
        var indexes = this.GetItemIndexes(indexPath);
        indexes[indexes.length] = 0;
        var newIndexPath = this.GetItemIndexPath(indexes);
        return this.GetFirstSiblingIndexPath(newIndexPath);
    },
    GetFirstSiblingIndexPath: function(indexPath) {
        var indexes = this.GetItemIndexes(indexPath);
        var i = 0;
        while(true) {
            indexes[indexes.length - 1] = i;
            var newIndexPath = this.GetItemIndexPath(indexes);
            if(!this.IsItemExist(newIndexPath))
                return null;
            if(this.IsItemVisible(newIndexPath) && this.IsItemEnabled(newIndexPath))
                return newIndexPath;
            i++;
        }
        return null;
    },
    GetLastSiblingIndexPath: function(indexPath) {
        var indexes = this.GetItemIndexes(indexPath);
        var parentItem = this.GetItemByIndexPath(this.GetParentIndexPath(indexPath));
        var i = parentItem ? parentItem.GetItemCount() - 1 : 0;
        while(true) {
            indexes[indexes.length - 1] = i;
            var newIndexPath = this.GetItemIndexPath(indexes);
            if(!this.IsItemExist(newIndexPath))
                return null;
            if(this.IsItemVisible(newIndexPath) && this.IsItemEnabled(newIndexPath))
                return newIndexPath;
            i--;
        }
        return null;
    },
    GetNextSiblingIndexPath: function(indexPath) {
        if(this.IsLastItem(indexPath)) return null;
        var indexes = this.GetItemIndexes(indexPath);
        var i = indexes[indexes.length - 1] + 1;
        while(true) {
            indexes[indexes.length - 1] = i;
            var newIndexPath = this.GetItemIndexPath(indexes);
            if(!this.IsItemExist(newIndexPath))
                return null;
            if(this.IsItemVisible(newIndexPath) && this.IsItemEnabled(newIndexPath))
                return newIndexPath;
            i++;
        }
        return null;
    },
    GetPrevSiblingIndexPath: function(indexPath) {
        if(this.IsFirstItem(indexPath)) return null;
        var indexes = this.GetItemIndexes(indexPath);
        var i = indexes[indexes.length - 1] - 1;
        while(true) {
            indexes[indexes.length - 1] = i;
            var newIndexPath = this.GetItemIndexPath(indexes);
            if(!this.IsItemExist(newIndexPath))
                return null;
            if(this.IsItemVisible(newIndexPath) && this.IsItemEnabled(newIndexPath))
                return newIndexPath;
            i--;
        }
        return null;
    },
    IsLastElement: function(element) {
        return element && (!element.nextSibling || !element.nextSibling.tagName);
    },
    IsLastItem: function(indexPath) {
        var itemElement = this.GetItemElement(indexPath);
        return this.IsLastElement(itemElement);
    },
    IsFirstElement: function(element) {
        return element && (!element.previousSibling || !element.previousSibling.tagName);
    },
    IsFirstItem: function(indexPath) {
        var itemElement = this.GetItemElement(indexPath);
        return this.IsFirstElement(itemElement);
    },
    IsItemExist: function(indexPath) {
        return !!this.GetItemByIndexPath(indexPath);
    },
    IsItemEnabled: function(indexPath) {
        var item = this.GetItemByIndexPath(indexPath);
        return item ? item.GetEnabled() : false;
    },
    IsItemVisible: function(indexPath) {
        var item = this.GetItemByIndexPath(indexPath);
        return item ? item.GetVisible() : false;
    },

    GetClientSubMenuPos: function(element, indexPath, pos, isVertical, isXPos) {
        if(!ASPx.IsValidPosition(pos)) {
            pos = isXPos ? this.GetSubMenuXPosition(indexPath, isVertical) : this.GetSubMenuYPosition(indexPath, isVertical);
        }

        var clientPos = isXPos ? this.GetClientSubMenuXPosition(element, pos, indexPath, isVertical) : this.GetClientSubMenuYPosition(element, pos, indexPath, isVertical);
        var isInverted = pos != clientPos;
        if(clientPos !== ASPx.InvalidPosition){
            var offset = isXPos ? this.GetSubMenuXOffset(indexPath) : this.GetSubMenuYOffset(indexPath);
            clientPos += isInverted ? -offset : offset;
            clientPos -= ASPx.GetPositionElementOffset(this.GetMenuElement(indexPath), isXPos);
        }
        return new ASPx.PopupPosition(clientPos, isInverted);
    },
    GetSubMenuXOffset: function(indexPath) {
        if(indexPath == "")
            return 0;
        else if(this.IsRootItem(indexPath)) {
            if(this.IsFirstItem(indexPath))
                return this.rootSubMenuFIXOffset;
            else if(this.IsLastItem(indexPath))
                return this.rootSubMenuLIXOffset;
            else
                return this.rootSubMenuXOffset;
        }
        else {
            if(this.IsFirstItem(indexPath))
                return this.subMenuFIXOffset;
            else if(this.IsLastItem(indexPath))
                return this.subMenuLIXOffset;
            else
                return this.subMenuXOffset;
        }
    },
    GetSubMenuYOffset: function(indexPath) {
        if(indexPath == "")
            return 0;
        else if(this.IsRootItem(indexPath)) {
            if(this.IsFirstItem(indexPath))
                return this.rootSubMenuFIYOffset;
            else if(this.IsLastItem(indexPath))
                return this.rootSubMenuLIYOffset;
            else
                return this.rootSubMenuYOffset;
        }
        else {
            if(this.IsFirstItem(indexPath))
                return this.subMenuFIYOffset;
            else if(this.IsLastItem(indexPath))
                return this.subMenuLIYOffset;
            else
                return this.subMenuYOffset;
        }
    },

    StartScrolling: function(buttonId, delay, step) {
        var indexPath = this.GetScrollButtonIndexPathById(buttonId);
        var level = this.GetMenuLevel(indexPath);
        aspxGetMenuCollection().DoHidePopupMenus(null, level, this.name, false, "");

        var direction = (buttonId.lastIndexOf(Constants.SBDIdEnd) == buttonId.length - Constants.SBDIdEnd.length) ? 1 : -1;
        var scrollHelper = this.scrollHelpers[indexPath];
        if(scrollHelper) scrollHelper.StartScrolling(direction, delay, step);
    },
    StopScrolling: function(buttonId) {
        var indexPath = this.GetScrollButtonIndexPathById(buttonId);
        var scrollHelper = this.scrollHelpers[indexPath];
        if(scrollHelper) scrollHelper.StopScrolling();
    },

    ClearAppearTimer: function() {
        aspxGetMenuCollection().ClearAppearTimer();
    },
    ClearDisappearTimer: function() {
        aspxGetMenuCollection().ClearDisappearTimer();
    },
    IsAppearTimerActive: function() {
        return aspxGetMenuCollection().IsAppearTimerActive();
    },
    IsDisappearTimerActive: function() {
        return aspxGetMenuCollection().IsDisappearTimerActive();
    },
    SetAppearTimer: function(indexPath, preventSubMenu) {
        aspxGetMenuCollection().SetAppearTimer(this.name, indexPath, this.appearAfter, preventSubMenu);
    },
    SetDisappearTimer: function() {
        aspxGetMenuCollection().SetDisappearTimer(this.name, this.disappearAfter);
    },

    IsDropDownItem: function(indexPath) {
        return ASPx.ElementContainsCssClass(this.GetItemElement(indexPath), MenuCssClasses.ItemDropDownMode);
    },
    DoItemClick: function(indexPath, hasItemLink, htmlEvent) {
        var processOnServer = this.RaiseItemClick(indexPath, htmlEvent);
        if(processOnServer && !hasItemLink)
            this.SendPostBack("CLICK:" + indexPath);
        else {
            this.ClearDisappearTimer();
            this.ClearAppearTimer();
            if(!this.HasChildren(indexPath) || this.IsDropDownItem(indexPath))
                aspxGetMenuCollection().DoHidePopupMenus(null, -1, this.name, false, "");
            else if(this.IsItemEnabled(indexPath) && !this.IsDropDownItem(indexPath))
                this.ShowSubMenu(indexPath);
        }
    },
    HasContent: function(mainCell) {
        for(var i = 0; i < mainCell.childNodes.length; i++)
            if(mainCell.childNodes[i].tagName)
                return true;
        return false;
    },
    DoShowPopupMenu: function(element, x, y, indexPath) {
        var parent = this.GetItemByIndexPath(indexPath);
        var menuElement = this.GetMenuMainElement(element);

        var popupMenuHasVisibleContent = menuElement && (MenuRenderHelper.HasSubMenuTemplate(menuElement) || 
            ASPx.ElementContainsCssClass(menuElement, MenuCssClasses.AdaptiveMenu)) || 
            parent && this.HasVisibleItems(parent);
        if(popupMenuHasVisibleContent === false)
            return;

        if(element && this.IsCallbacksEnabled())
            this.ShowLoadingPanelInMenu(element);

        if(ASPx.GetElementVisibility(element))
            ASPx.SetStyles(element, { left: ASPx.InvalidPosition, top: ASPx.InvalidPosition });
        ASPx.SetElementDisplay(element, true);

        if(parent) {
            for(var i = 0; i < parent.GetItemCount() ; i++) {
                var item = parent.GetItem(i);
                this.SetPopOutElementVisible(item.indexPath, this.HasVisibleItems(item));
            }
        }

        MenuRenderHelper.CalculateSubMenu(this, element, false);

        if(this.popupCount == 0) this.popupToLeft = this.rtl;
        var isVertical = this.IsVertical(indexPath);
        var horizontalPopupPosition = this.GetClientSubMenuPos(element, indexPath, x, isVertical, true);
        if(horizontalPopupPosition.position === ASPx.InvalidPosition) {
            isVertical = !isVertical;
            horizontalPopupPosition = this.GetClientSubMenuPos(element, indexPath, x, isVertical, true);
        }
        var verticalPopupPosition = this.GetClientSubMenuPos(element, indexPath, y, isVertical, false);
        var clientX = horizontalPopupPosition.position;
        var clientY = verticalPopupPosition.position;
        var toTheLeft = horizontalPopupPosition.isInverted;
        var toTheTop = verticalPopupPosition.isInverted;

        var scrollHelper = this.scrollHelpers[indexPath];
        if(scrollHelper) {
            var yClientCorrection = this.GetScrollSubMenuYCorrection(element, scrollHelper, clientY);
            if(yClientCorrection > 0) {
                clientY += yClientCorrection;
                verticalPopupPosition.position = clientY;
            }
        }

        // correct position ifscrollbars changed
        var parentElement = this.GetItemContentElement(indexPath);
        var prevParentPos = ASPx.GetAbsoluteX(parentElement);
        ASPx.SetStyles(element, {
            left: clientX, top: clientY
        });
        // the following hack forces IE8 to update scrollbar presence
        if(ASPx.Browser.IE && ASPx.IsElementRightToLeft(document.body)) {
            ASPx.SetElementDisplay(element, false);
            ASPx.SetElementDisplay(element, true);
        }
        clientX += ASPx.GetAbsoluteX(parentElement) - prevParentPos;

        if(this.enableAnimation) {
            this.StartAnimation(element, indexPath, horizontalPopupPosition, verticalPopupPosition, isVertical);
        }
        else {
            ASPx.SetStyles(element, { left: clientX, top: clientY });
            ASPx.SetElementVisibility(element, true);

            if(this.enableSubMenuFullWidth)
                this.ApplySubMenuFullWidth(element);

            this.DoShowPopupMenuIFrame(element, clientX, clientY, ASPx.InvalidDimension, ASPx.InvalidDimension, indexPath);
            this.DoShowPopupMenuBorderCorrector(element, clientX, clientY, indexPath, toTheLeft, toTheTop);
        }

        aspxGetMenuCollection().RegisterVisiblePopupMenu(this.name, element.id);
        this.popupCount++;
        ASPx.GetControlCollection().AdjustControls(element);

        this.CorrectVerticalAlignment(ASPx.AdjustHeight, this.GetPopOutElements, "PopOut");
        this.CorrectVerticalAlignment(ASPx.AdjustVerticalMargins, this.GetPopOutImages, "PopOutImg");

        this.RaisePopUp(indexPath);
    },
    ShowLoadingPanelInMenu: function(element) {
        var lpParent = this.GetMenuMainElement(element);
        if(lpParent && !this.HasContent(lpParent))
            this.CreateLoadingPanelInsideContainer(lpParent);
    },
    GetScrollSubMenuYCorrection: function(element, scrollHelper, clientY) {
        var absoluteClientY = clientY + ASPx.GetPositionElementOffset(element);

        var excessTop = this.GetScrollExcessTop(absoluteClientY);
        var excessBottom = this.GetScrollExcessBottom(element, absoluteClientY);

        var correction = 0;
        if(excessTop > 0)
            correction += excessTop + this.scrollVertOffset;

        if(excessBottom > 0 && (absoluteClientY + correction == ASPx.GetDocumentScrollTop())) {
            excessBottom += this.scrollVertOffset;
            correction += this.scrollVertOffset;
        }

        this.PrepareScrolling(element, scrollHelper, excessTop, excessBottom);
        return correction;
    },
    GetScrollExcessTop: function(clientY) {
        return ASPx.GetDocumentScrollTop() - clientY;
    },
    GetScrollExcessBottom: function(element, clientY) {
        // get rid of stray scrollbar
        ASPx.SetElementDisplay(element, false);
        var docHeight = ASPx.GetDocumentClientHeight();
        ASPx.SetElementDisplay(element, true);

        return clientY + element.offsetHeight - ASPx.GetDocumentScrollTop() - docHeight;
    },
    PrepareScrolling: function(element, scrollHelper, excessTop, excessBottom) {
        scrollHelper.Initialize();

        var corrector = element.offsetHeight - scrollHelper.GetScrollAreaHeight() + this.scrollVertOffset;
        if(excessTop > 0)
            scrollHelper.Calculate(element.offsetHeight - excessTop - corrector);
        if(excessBottom > 0)
            scrollHelper.Calculate(element.offsetHeight - excessBottom - corrector);
    },
    ApplySubMenuFullWidth: function(element) {
        ASPx.SetStyles(element, { left: 0, right: 0, width: "auto" });
        var menuElement = this.GetMenuMainElement(element);
        ASPx.SetStyles(menuElement, { width: "100%", "box-sizing": "border-box" });
        var templateElement = ASPx.GetChildByClassName(menuElement, "dx");
        if(templateElement) ASPx.SetStyles(templateElement, { width: "100%" });
    },
    DoShowPopupMenuIFrame: function(element, x, y, width, height, indexPath) {
        if(!this.renderIFrameForPopupElements) return;

        var iFrame = element.overflowElement;
        if(!iFrame) {
            iFrame = this.GetMenuIFrameElement(indexPath);
            element.overflowElement = iFrame;
        }
        if(iFrame) {
            var menuElement = this.GetMenuMainElement(element);
            if(width < 0)
                width = menuElement.offsetWidth;
            if(height < 0)
                height = menuElement.offsetHeight;
            ASPx.SetStyles(iFrame, {
                width: width, height: height,
                left: x, top: y, display: ""
            });
        }
    },
    DoShowPopupMenuBorderCorrector: function(element, x, y, indexPath, toTheLeft, toTheTop) {
        var borderCorrectorElement = this.GetMenuBorderCorrectorElement(indexPath);
        if(borderCorrectorElement) {
            var params = this.GetPopupMenuBorderCorrectorPositionAndSize(element, x, y, indexPath, toTheLeft, toTheTop);
            var itemCell = this.GetItemContentElement(indexPath);
            var popOutImageCell = this.GetItemPopOutElement(indexPath);

            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion == 9) { //B189348
                var isVertical = this.IsVertical(indexPath);
                var itemBoundCoord = itemCell.getBoundingClientRect()[isVertical ? 'bottom' : 'right'];
                var itemBorderWidth = ASPx.PxToInt(ASPx.GetCurrentStyle(itemCell)[isVertical ? 'borderBottomWidth' : 'borderRightWidth']);

                if(popOutImageCell != null) {
                    var popOutImageBoundCoord = popOutImageCell.getBoundingClientRect()[isVertical ? 'bottom' : 'right'];
                    if(popOutImageBoundCoord > itemBoundCoord) {
                        itemBoundCoord = popOutImageBoundCoord;
                        itemBorderWidth = ASPx.PxToInt(ASPx.GetCurrentStyle(popOutImageCell)[isVertical ? 'borderBottomWidth' : 'borderRightWidth']);
                    }
                }

                var menu = this.GetMainElement();

                itemBoundCoord -= Math.min(menu.getBoundingClientRect()[isVertical ? 'top' : 'left'], ASPx.GetPositionElementOffset(menu, !isVertical));

                if(isVertical) {
                    var bottomsDifference = this.GetItemElement(indexPath).getBoundingClientRect().bottom -
                        this.GetMenuElement(indexPath).getBoundingClientRect().bottom;

                    itemBoundCoord -= bottomsDifference > 0 && bottomsDifference;
                }

                var borderCorrectorBoundCoord = isVertical ? params.top + params.height : params.left + params.width;

                if(itemBoundCoord - borderCorrectorBoundCoord != itemBorderWidth) {
                    borderCorrectorBoundCoord = itemBoundCoord - itemBorderWidth;
                    if(isVertical)
                        params.height = borderCorrectorBoundCoord - params.top;
                    else
                        params.width = borderCorrectorBoundCoord - params.left;
                }
            }

            ASPx.SetStyles(borderCorrectorElement, {
                width: params.width, height: params.height,
                left: params.left, top: params.top,
                display: "", visibility: "visible"
            });

            element.borderCorrectorElement = borderCorrectorElement;
        }
    },
    GetPopupMenuBorderCorrectorPositionAndSize: function(element, x, y, indexPath, toTheLeft, toTheTop) {
        var result = {};

        var itemInfo = new MenuItemInfo(this, indexPath);
        var menuXOffset = ASPx.GetClientLeft(this.GetMenuMainElement(element));
        var menuYOffset = ASPx.GetClientTop(this.GetMenuMainElement(element));

        var menuElement = this.GetMenuMainElement(element);
        var menuClientWidth = menuElement.clientWidth;
        var menuClientHeight = menuElement.clientHeight;

        if(this.IsVertical(indexPath)) {
            var commonClientHeight = itemInfo.clientHeight < menuClientHeight
                ? itemInfo.clientHeight
                : menuClientHeight;

            result.width = menuXOffset;
            result.height = commonClientHeight + itemInfo.clientTop - menuYOffset;

            result.left = x;
            if(toTheLeft)
                result.left += menuClientWidth + menuXOffset;

            result.top = y + menuYOffset;
            if(toTheTop)
                result.top += menuClientHeight - result.height;
        }
        else {
            var itemWidth = itemInfo.clientWidth;
            if(this.IsDropDownItem(indexPath))
                itemWidth = this.GetItemContentElement(indexPath).clientWidth;

            var commonClientWidth = itemWidth < menuClientWidth
                ? itemWidth
                : menuClientWidth;

            result.width = commonClientWidth + itemInfo.clientLeft - menuXOffset;
            result.height = menuYOffset;

            result.left = x + menuXOffset;
            if(toTheLeft)
                result.left += menuClientWidth - result.width;

            result.top = y;
            if(toTheTop)
                result.top += menuClientHeight + menuYOffset;

        }

        return result;
    },
    DoHidePopupMenu: function(evt, element) {
        this.DoHidePopupMenuBorderCorrector(element);
        this.DoHidePopupMenuIFrame(element);

        var menuElement = this.GetMenuMainElement(element);
        ASPx.PopupUtils.StopAnimation(element, menuElement);

        ASPx.SetElementVisibility(element, false);
        ASPx.SetElementDisplay(element, false);

        this.CancelSubMenuItemHoverItem(element);
        aspxGetMenuCollection().UnregisterVisiblePopupMenu(this.name, element.id);
        this.popupCount--;
        var indexPath = this.GetIndexPathById(element.id, true);
        var scrollHelper = this.scrollHelpers[indexPath];
        if(scrollHelper) {
            element.style.height = "";
            scrollHelper.ResetScrolling();
        }
        this.RaiseCloseUp(indexPath);
    },
    DoHidePopupMenuIFrame: function(element) {
        if(!this.renderIFrameForPopupElements) return;

        var iFrame = element.overflowElement;
        if(iFrame)
            ASPx.SetElementDisplay(iFrame, false);
    },
    DoHidePopupMenuBorderCorrector: function(element) {
        var borderCorrectorElement = element.borderCorrectorElement;
        if(borderCorrectorElement) {
            ASPx.SetElementVisibility(borderCorrectorElement, false);
            ASPx.SetElementDisplay(borderCorrectorElement, false);
            element.borderCorrectorElement = null;
        }
    },

    SetHoverElement: function(element) {
        if(!this.IsStateControllerEnabled()) return;

        this.lockHoverEvents = true;
        ASPx.GetStateController().SetCurrentHoverElementBySrcElement(element);
        this.lockHoverEvents = false;
    },
    ApplySubMenuItemHoverItem: function(element, hoverItem, hoverElement) {
        if(!element.hoverItem && ASPx.GetElementDisplay(element)) {
            var newHoverItem = hoverItem.Clone();
            element.hoverItem = newHoverItem;
            element.hoverElement = hoverElement;
            newHoverItem.Apply(hoverElement);
        }
    },
    CancelSubMenuItemHoverItem: function(element) {
        if(element.hoverItem) {
            element.hoverItem.Cancel(element.hoverElement);
            element.hoverItem = null;
            element.hoverElement = null;
        }
    },
    ShowSubMenu: function(indexPath) {
        var element = this.GetMenuElement(indexPath);
        if(element != null) {
            var level = this.GetMenuLevel(indexPath);
            aspxGetMenuCollection().DoHidePopupMenus(null, level - 1, this.name, false, element.id);

            if(!ASPx.GetElementDisplay(element))
                this.DoShowPopupMenu(element, ASPx.InvalidPosition, ASPx.InvalidPosition, indexPath);
        }
        this.ClearAppearTimer();
    },

    SelectItem: function(indexPath) {
        if(!this.IsStateControllerEnabled()) return;

        var element = this.GetItemContentElement(indexPath);
        if(element != null)
            ASPx.GetStateController().SelectElementBySrcElement(element);
    },
    DeselectItem: function(indexPath) {
        if(!this.IsStateControllerEnabled()) return;

        var element = this.GetItemContentElement(indexPath);
        if(element != null) {
            var hoverItem = null;
            var hoverElement = null;
            var menuElement = this.GetMenuElement(indexPath);
            if(menuElement && menuElement.hoverItem) {
                hoverItem = menuElement.hoverItem;
                hoverElement = menuElement.hoverElement;
                this.CancelSubMenuItemHoverItem(menuElement);
            }
            ASPx.GetStateController().DeselectElementBySrcElement(element);
            if(menuElement != null && hoverItem != null)
                this.ApplySubMenuItemHoverItem(menuElement, hoverItem, hoverElement);
        }
    },
    InitializeSelectedItem: function() {
        if(!this.allowSelectItem) return;
        this.SelectItem(this.GetSelectedItemIndexPath());
    },
    GetSelectedItemIndexPath: function() {
        return this.selectedItemIndexPath;
    },
    SetSelectedItemInternal: function(indexPath, modifyHotTrackSelection) {
        if(modifyHotTrackSelection)
            this.SetHoverElement(null);

        this.DeselectItem(this.selectedItemIndexPath);
        this.selectedItemIndexPath = indexPath;
        var item = this.GetItemByIndexPath(indexPath);
        if(item == null || item.GetEnabled())
            this.SelectItem(this.selectedItemIndexPath);

        if(modifyHotTrackSelection) {
            var element = this.GetItemContentElement(indexPath);
            if(element != null)
                this.SetHoverElement(element);
        }
    },
    InitializeCheckedItems: function() {
        if(!this.allowCheckItems) return;

        var indexPathes = this.checkedState.split(";");
        for(var i = 0; i < indexPathes.length; i++) {
            if(indexPathes[i] != "") {
                this.checkedItems.push(indexPathes[i]);
                this.SelectItem(indexPathes[i]);
            }
        }
    },
    ChangeCheckedItem: function(indexPath) {
        this.SetHoverElement(null);

        var itemsGroup = this.GetItemsGroup(indexPath);
        if(itemsGroup != null) {
            if(itemsGroup.length > 1) {
                if(!this.IsCheckedItem(indexPath)) {
                    for(var i = 0; i < itemsGroup.length; i++) {
                        if(itemsGroup[i] == indexPath) continue;
                        if(this.IsCheckedItem(itemsGroup[i])) {
                            ASPx.Data.ArrayRemove(this.checkedItems, itemsGroup[i]);
                            this.DeselectItem(itemsGroup[i]);
                        }
                    }
                    this.SelectItem(indexPath);
                    this.checkedItems.push(indexPath);
                }
            }
            else {
                if(this.IsCheckedItem(indexPath)) {
                    ASPx.Data.ArrayRemove(this.checkedItems, indexPath);
                    this.DeselectItem(indexPath);
                }
                else {
                    this.SelectItem(indexPath);
                    this.checkedItems.push(indexPath);
                }
            }
        }

        var element = this.GetItemContentElement(indexPath);
        if(element != null)
            this.SetHoverElement(element);
    },
    GetItemsGroup: function(indexPath) {
        for(var i = 0; i < this.itemCheckedGroups.length; i++) {
            if(ASPx.Data.ArrayIndexOf(this.itemCheckedGroups[i], indexPath) > -1)
                return this.itemCheckedGroups[i];
        }
        return null;
    },
    IsCheckedItem: function(indexPath) {
        return ASPx.Data.ArrayIndexOf(this.checkedItems, indexPath) > -1;
    },

    UpdateStateObject: function(){
        this.UpdateStateObjectWithObject({ selectedItemIndexPath: this.selectedItemIndexPath, checkedState: this.GetCheckedState() });
    },
    GetCheckedState: function() {
        var state = "";
        for(var i = 0; i < this.checkedItems.length; i++) {
            state += this.checkedItems[i];
            if(i < this.checkedItems.length - 1)
                state += ";";
        }
        return state;
    },

    GetAnimationVerticalDirection: function(indexPath, popupPosition, isVertical) {
        var verticalDirection = (this.IsRootItem(indexPath) && !isVertical) ? -1 : 0;
        if(popupPosition.isInverted) verticalDirection *= -1;
        return verticalDirection;
    },
    GetAnimationHorizontalDirection: function(indexPath, popupPosition, isVertical) {
        var horizontalDirection = (this.IsRootItem(indexPath) && !isVertical) ? 0 : -1;
        if(popupPosition.isInverted) horizontalDirection *= -1;
        return horizontalDirection;
    },
    StartAnimation: function(animationDivElement, indexPath, horizontalPopupPosition, verticalPopupPosition, isVertical) {
        var element = this.GetMenuMainElement(animationDivElement);

        var clientX = horizontalPopupPosition.position;
        var clientY = verticalPopupPosition.position;
        ASPx.PopupUtils.InitAnimationDiv(animationDivElement, clientX, clientY);

        var verticalDirection = this.GetAnimationVerticalDirection(indexPath, verticalPopupPosition, isVertical);
        var horizontalDirection = this.GetAnimationHorizontalDirection(indexPath, horizontalPopupPosition, isVertical);
        var yPos = verticalDirection * element.offsetHeight;
        var xPos = horizontalDirection * element.offsetWidth;

        ASPx.SetStyles(element, { left: xPos, top: yPos });
        ASPx.SetElementVisibility(animationDivElement, true);

        if(this.enableSubMenuFullWidth)
            this.ApplySubMenuFullWidth(animationDivElement);

        this.DoShowPopupMenuIFrame(animationDivElement, clientX, clientY, 0, 0, indexPath);
        this.DoShowPopupMenuBorderCorrector(animationDivElement, clientX, clientY, indexPath,
            horizontalPopupPosition.isInverted, verticalPopupPosition.isInverted);

        ASPx.PopupUtils.StartSlideAnimation(animationDivElement, element, this.GetMenuIFrameElement(indexPath), this.slideAnimationDuration, this.enableSubMenuFullWidth, false);
    },

    OnItemClick: function(indexPath, evt) {
        var sourceElement = ASPx.Evt.GetEventSource(evt);
        var clickedLinkElement = ASPx.GetParentByTagName(sourceElement, "A");
        var isLinkClicked = (clickedLinkElement != null && clickedLinkElement.href != ASPx.AccessibilityEmptyUrl);
        var element = this.GetItemContentElement(indexPath);
        var linkElement = (element != null) ? (element.tagName === "A" ? element : ASPx.GetNodeByTagName(element, "A", 0)) : null;
        if(linkElement != null && linkElement.href == ASPx.AccessibilityEmptyUrl)
            linkElement = null;

        if(this.allowSelectItem)
            this.SetSelectedItemInternal(indexPath, true);
        if(this.allowCheckItems)
            this.ChangeCheckedItem(indexPath);
        this.DoItemClick(indexPath, isLinkClicked || (linkElement != null), evt);

        if(!isLinkClicked && linkElement != null && !(ASPx.Browser.WebKitTouchUI && this.HasChildren(indexPath)))
            ASPx.Url.NavigateByLink(linkElement);
    },
    OnItemDropDownClick: function(indexPath, evt) {
        if(this.IsItemEnabled(indexPath))
            this.ShowSubMenu(indexPath);
    },
    AfterItemOverAllowed: function(hoverItem) {
        return hoverItem.name != "" && !this.lockHoverEvents;
    },
    OnAfterItemOver: function(hoverItem, hoverElement) {
        if(!this.AfterItemOverAllowed(hoverItem)) return;

        if(!this.showSubMenus) {
            this.savedCallbackHoverItem = hoverItem;
            this.savedCallbackHoverElement = hoverElement;
            return;
        }

        this.ClearDisappearTimer();
        this.ClearAppearTimer();

        var indexPath = this.GetMenuIndexPathById(hoverItem.name);
        if(indexPath == "") {
            indexPath = this.GetItemIndexPathById(hoverItem.name);
            var canShowSubMenu = true;

            if(this.IsDropDownItem(indexPath)) {
                var popOutImageElement = this.GetItemPopOutElement(indexPath);
                if(popOutImageElement != null && popOutImageElement != hoverElement) {
                    hoverItem.needRefreshBetweenElements = true;
                    canShowSubMenu = false;
                }
            }
            var preventSubMenu = !(canShowSubMenu && hoverItem.enabled && hoverItem.kind == ASPx.HoverItemKind);
            this.SetAppearTimer(indexPath, preventSubMenu);
            this.RaiseItemMouseOver(indexPath);
        }
    },
    OnBeforeItemOver: function(hoverItem, hoverElement) {
        if(ASPx.Browser.NetscapeFamily && ASPx.IsExists(hoverElement.offsetParent) &&
                hoverElement.offsetParent.style.borderCollapse == "collapse") {
            hoverElement.offsetParent.style.borderCollapse = "separate";
            hoverElement.offsetParent.style.borderCollapse = "collapse";
        }

        var indexPath = this.GetItemIndexPathById(hoverItem.name);
        var element = this.GetMenuElement(indexPath);
        if(element) this.CancelSubMenuItemHoverItem(element);
    },
    OnItemOverTimer: function(indexPath, preventSubMenu) {
        var element = this.GetMenuElement(indexPath);
        
        if(element == null || preventSubMenu) {
            var level = this.GetMenuLevel(indexPath);
            aspxGetMenuCollection().DoHidePopupMenus(null, level - 1, this.name, false, "");
        }
        if(this.IsAppearTimerActive() && !preventSubMenu) {
            this.ClearAppearTimer();
            if(this.GetItemContentElement(indexPath) != null || this.GetItemPopOutElement(indexPath) != null) {
                this.ShowSubMenu(indexPath);
            }
        }
    },
    OnBeforeItemDisabled: function(disabledItem, disabledElement) {
        this.ClearAppearTimer();
        var indexPath = this.GetItemIndexPathById(disabledElement.id);
        if(indexPath != "") {
            var element = this.GetMenuElement(indexPath);
            if(element != null) this.DoHidePopupMenu(null, element);
        }
    },
    OnAfterItemOut: function(hoverItem, hoverElement, newHoverElement) {
        if(!this.showSubMenus) {
            this.savedCallbackHoverItem = null;
            this.savedCallbackHoverElement = null;
        }

        if(hoverItem.name == "" || this.lockHoverEvents) return;
        if(hoverItem.IsChildElement(newHoverElement)) return;

        var indexPath = this.GetItemIndexPathById(hoverItem.name);
        var element = this.GetMenuElement(indexPath);

        this.ClearDisappearTimer();
        this.ClearAppearTimer();
        if(element == null || !ASPx.GetIsParent(element, newHoverElement))
            this.SetDisappearTimer();

        if(element != null)
            this.ApplySubMenuItemHoverItem(element, hoverItem, hoverElement);

        if(indexPath != "")
            this.RaiseItemMouseOut(indexPath);
    },
    OnItemOutTimer: function() {
        if(this.IsDisappearTimerActive()) {
            this.ClearDisappearTimer();
            if(aspxGetMenuCollection().CheckFocusedElement())
                this.SetDisappearTimer();
            else
                this.OnHideByItemOut();
        }
    },
    OnHideByItemOut: function() {
        aspxGetMenuCollection().DoHidePopupMenus(null, 0, this.name, true, "");
    },
    OnFocusedItemKeyDown: function(evt, focusedItem, focusedElement) {
        var handled = false;
        var indexPath = this.GetItemIndexPathById(focusedItem.name);
        switch (evt.keyCode) {
            case ASPx.Key.Tab: {
                handled = this.FocusNextTabItem(indexPath, evt.shiftKey);
                break;
            }
            case ASPx.Key.Down: {
                if(this.IsVertical(indexPath)) {
                    this.FocusNextItem(indexPath);
                }
                else {
                    this.ShowSubMenu(indexPath);
                    this.FocusItemByIndexPath(this.GetFirstChildIndexPath(indexPath));
                }
                handled = true;
                break;
            }
            case ASPx.Key.Up: {
                if(this.IsVertical(indexPath)) {
                    this.FocusPrevItem(indexPath);
                }
                else {
                    this.ShowSubMenu(indexPath);
                    this.FocusItemByIndexPath(this.GetFirstChildIndexPath(indexPath));
                }
                handled = true;
                break;
            }
            case ASPx.Key.Left: {
                if(this.IsVertical(indexPath)) {
                    var parentIndexPath = this.GetParentIndexPath(indexPath);
                    if(this.IsVertical(parentIndexPath)) {
                        this.FocusItemByIndexPath(parentIndexPath);
                    }
                    else {
                        this.FocusPrevItem(parentIndexPath);
                    }
                }
                else {
                    this.FocusPrevItem(indexPath);
                }
                handled = true;
                break;
            }
            case ASPx.Key.Right: {
                if(this.IsVertical(indexPath)) {
                    if(this.HasChildren(indexPath)) {
                        this.ShowSubMenu(indexPath);
                        this.FocusItemByIndexPath(this.GetFirstChildIndexPath(indexPath));
                    }
                    else {
                        while(!this.IsRootItem(indexPath))
                            indexPath = this.GetParentIndexPath(indexPath);
                        this.FocusNextItem(indexPath);
                    }
                }
                else {
                    this.FocusNextItem(indexPath);
                }
                handled = true;
                break;
            }
            case ASPx.Key.Esc: {
                var parentIndexPath = this.GetParentIndexPath(indexPath);
                if(parentIndexPath === "") {
                    aspxGetMenuCollection().DoHidePopupMenus(null, -1, this.name, false, "");
                    handled = true;
                }
                else {
                    this.FocusItemByIndexPath(parentIndexPath);
                    var element = this.GetMenuElement(parentIndexPath);
                    if(element != null) {
                        this.DoHidePopupMenu(null, element);
                        handled = true;
                    }
                }
            }
        }
        if(handled)
            ASPx.Evt.PreventEventAndBubble(evt);
    },
    FocusItemByIndexPath: function(indexPath) {
        var element = this.GetItemElement(indexPath);
        var link = ASPx.GetNodeByTagName(element, "A", 0);
        if(link != null) ASPx.SetFocus(link);
    },
    FocusNextTabItem: function(indexPath, shiftKey) {
        if(this.IsRootItem(indexPath)) return false;
        while(true) {
            if(this.IsRootItem(indexPath)) {
                if(!shiftKey) {
                    if(this.GetNextSiblingIndexPath(indexPath) != null) {
                        this.FocusNextItem(indexPath);
                        return true;
                    }
                }
                else {
                    if(this.GetPrevSiblingIndexPath(indexPath) != null) {
                        this.FocusPrevItem(indexPath);
                        return true;
                    }
                }
                break;
            }
            else {
                if(!shiftKey) {
                    if(this.GetNextSiblingIndexPath(indexPath) == null)
                        indexPath = this.GetParentIndexPath(indexPath);
                    else {
                        this.FocusNextItem(indexPath);
                        return true;
                    }
                }
                else {
                    if(this.GetPrevSiblingIndexPath(indexPath) == null)
                        indexPath = this.GetParentIndexPath(indexPath);
                    else {
                        this.FocusPrevItem(indexPath);
                        return true;
                    }
                }
            }
        }
        return false;
    },
    FocusNextItem: function(indexPath) {
        var newIndexPath = this.GetNextSiblingIndexPath(indexPath);
        if(newIndexPath == null)
            newIndexPath = this.GetFirstSiblingIndexPath(indexPath);
        if(indexPath != newIndexPath)
            this.FocusItemByIndexPath(newIndexPath);
    },
    FocusPrevItem: function(indexPath) {
        var newIndexPath = this.GetPrevSiblingIndexPath(indexPath);
        if(newIndexPath == null)
            newIndexPath = this.GetLastSiblingIndexPath(indexPath);
        if(indexPath != newIndexPath)
            this.FocusItemByIndexPath(newIndexPath);
    },
    TryFocusItem: function(itemIndex) {
        var item = this.GetItem(itemIndex);
        if(item.GetVisible() && item.GetEnabled()) {
            this.FocusItemByIndexPath(item.GetIndexPath());
            return true;
        }
        return false;
    },
    Focus: function() {
        if(this.rootItem != null) { // Client-side API enabled
            for(var i = 0; i < this.GetItemCount() ; i++) {
                if(this.TryFocusItem(i))
                    return true;
            }
        }
        else
            this.FocusNextItem("-1");
    },
    FocusLastItem: function() {
        if(this.rootItem != null) { // Client-side API enabled
            for(var i = this.GetItemCount() - 1; i >= 0; i--) {
                if(this.TryFocusItem(i))
                    return true;
            }
        }
        else
            this.FocusPrevItem(this.GetItemCount() - 1);
    },
    OnCallback: function(result) {
        ASPx.InitializeScripts(); // to prevent double scripts loading and executing

        this.InitializeScrollableMenus();
        for(var indexPath in result) {
            var menuElement = this.GetMenuElement(indexPath);
            if(menuElement) {
                if(aspxGetMenuCollection().IsSubMenuVisible(menuElement.id)) {
                    this.ShowPopupSubMenuAfterCallback(menuElement, result[indexPath]);
                } else {
                    this.SetSubMenuInnerHtml(menuElement, result[indexPath]);
                }
            }
        }
        this.ClearVerticalAlignedElementsCache();

        this.CorrectVerticalAlignment(ASPx.AdjustHeight, this.GetPopOutElements, "PopOut");
        this.CorrectVerticalAlignment(ASPx.AdjustVerticalMargins, this.GetPopOutImages, "PopOutImg");

        this.InitializeInternal(false);
        if(!this.showSubMenus) {
            this.showSubMenus = true;
            if(this.savedCallbackHoverItem != null && this.savedCallbackHoverElement != null)
                this.OnAfterItemOver(this.savedCallbackHoverItem, this.savedCallbackHoverElement);
            this.savedCallbackHoverItem = null;
            this.savedCallbackHoverElement = null;
        }
    },
    SetSubMenuInnerHtml: function(menuElement, html) {
        ASPx.SetInnerHtml(this.GetMenuMainElement(menuElement), html);

        // forelements inserted on callback
        MenuRenderHelper.InlineInitializeSubMenuMenuElement(this, menuElement);
        MenuRenderHelper.CalculateSubMenu(this, menuElement, true);
    },
    ShowPopupSubMenuAfterCallback: function(element, callbackResult) {
        var indexPath = this.GetIndexPathById(element.id, true);
        var currentX = ASPx.PxToInt(element.style.left);
        var currentY = ASPx.PxToInt(element.style.top);
        var showedToTheTop = this.ShowedToTheTop(element, indexPath);
        var showedToTheLeft = this.ShowedToTheLeft(element, indexPath);

        ASPx.SetStyles(element, {
            left: ASPx.InvalidPosition, top: ASPx.InvalidPosition
        });
        this.SetSubMenuInnerHtml(element, callbackResult);

        var vertPos = this.GetClientSubMenuPos(element, indexPath, ASPx.InvalidPosition, this.IsVertical(indexPath), false);
        var clientY = vertPos.position;
        var toTheTop = vertPos.isInverted;

        if(!this.IsVertical(indexPath) && showedToTheTop != toTheTop) {
            clientY = currentY;
            toTheTop = showedToTheTop;
        }

        var scrollHelper = this.scrollHelpers[indexPath];
        if(scrollHelper) {
            var yClientCorrection = this.GetScrollSubMenuYCorrection(element, scrollHelper, clientY);
            if(yClientCorrection > 0)
                clientY += yClientCorrection;
        }
        ASPx.SetStyles(element, { left: currentX, top: clientY });

        if(this.enableSubMenuFullWidth)
            this.ApplySubMenuFullWidth(element);

        this.DoShowPopupMenuIFrame(element, currentX, clientY, ASPx.InvalidDimension, ASPx.InvalidDimension, indexPath);
        this.DoShowPopupMenuBorderCorrector(element, currentX, clientY, indexPath, showedToTheLeft, toTheTop);

        ASPx.GetControlCollection().AdjustControls(element);
    },
    ShowedToTheTop: function(element, indexPath) {
        var currentY = ASPx.PxToInt(element.style.top);
        var parentBottomY = this.GetSubMenuYPosition(indexPath, this.IsVertical(indexPath));
        return currentY < parentBottomY;
    },
    ShowedToTheLeft: function(element, indexPath) {
        var currentX = ASPx.PxToInt(element.style.left);
        var parentX = this.GetSubMenuXPosition(indexPath, this.IsVertical(indexPath));
        return currentX < parentX;
    },

    // API
    CreateItems: function(items) {
        if (items.length == 0)
            return;

        if(this.NeedCreateItemsOnClientSide())
            this.CreateClientItems(items);
        else
            this.CreateServerItems(items);
    },
    AddItem: function(item) {
        this.PreInitializeClientMenuItems();
        this.RenderItemIfRequired(this.rootItem.CreateItemInternal(item), false);
        this.InitializeClientItems();
    },
    CreateClientItems: function(items) {
        this.PreInitializeClientMenuItems();
        this.rootItem.CreateItems(items);
        this.RenderItems(this.rootItem.items);
        this.InitializeClientItems();
    },
    CreateServerItems: function(items) {
        this.CreateRootItemIfRequired();
        this.rootItem.CreateItems(items);
    },
    PreInitializeClientMenuItems: function() {
        if(!this.rootMenuSample)
            this.InitializeMenuSamples();
        this.CreateRootItemIfRequired();
        if(!this.renderData)
            this.CreateRenderData();
    },
    InitializeClientItems: function() {
        this.needDropCache = true;
        MenuRenderHelper.InlineInitializeElements(this);
        this.InitializeEnabledAndVisible(true);
    },
    CreateRootItemIfRequired: function() {
        if(!this.rootItem) {
            var itemType = this.GetClientItemType();
            this.rootItem = new itemType(this, null, 0, "");
        }
    },
    CreateRootMenuElement: function() {
        var wrapperElement = this.GetMainElement().parentNode;
        wrapperElement.innerHTML = "";
        wrapperElement.appendChild(this.rootMenuSample.cloneNode(true));
    }, 
    NeedAppendToRenderData: function(item) {
        return this.NeedCreateItemsOnClientSide() && item.visible || typeof(item.visible) == "undefined";
    },
    ClearItems: function() {
        this.PreInitializeClientMenuItems();
        this.CreateRootMenuElement();
        this.ClearRenderData();
        this.rootItem.items = [];
    },
    GetParentItem: function(rootItemIndexPath) {
        if(!rootItemIndexPath)
            return this.rootItem;
        return this.GetItemByIndexPath(rootItemIndexPath);
    },
    RenderItems: function(items) {
        for(var i=0; i < items.length; i++) {
            var item = items[i];
            this.RenderItemIfRequired(item, item.items.length > 0);
            this.RenderItems(item.items);
        }
    },
    RenderItemIfRequired: function(item, withSubitems) {
        var rootItemElement = item.parent.name ? this.GetItemElement(item.parent.indexPath) : null;
        var rootMenuElement = this.GetOrRenderRootItem(item);

        if(!this.GetItemElement(item.indexPath)) {
            if(!rootItemElement)
                this.RenderItemInternal(rootMenuElement, item, withSubitems);
            else {
                this.AddPopOutElementToItemElementIfNeeded(rootItemElement, item.parent.indexPath, true);
                this.RenderItemInternal(rootMenuElement, item, withSubitems);
            }

            var itemElementId = this.GetItemElementId(item.indexPath);
            ASPx.GetStateController().AddHoverItem(itemElementId, ["dxm-hovered"], [""], [""], null, null, false);
            ASPx.GetStateController().AddDisabledItem(itemElementId, ["dxm-disabled"], [""], [""], null, null, false);
        }
    },
    GetOrRenderRootItem: function(item) {
        if(item.parent.name) {
            var rootMenuElement = this.GetMenuElement(item.parent.indexPath);
            return rootMenuElement ? rootMenuElement : this.RenderSubMenuItem(item.parent.indexPath);
        } else
            return this.GetMenuElement("");
    },
    RenderItemInternal: function(rootItem, item, withSubitems) {
        var rootItem =  ASPx.GetNodeByTagName(rootItem, "UL", 0);
        var element = this.CreateItemElement(item, withSubitems);
        var li = ASPx.GetNodesByTagName(rootItem, "LI");

        if(li.length == 0) {
            rootItem.appendChild(element);
            return;
        }

        this.RenderSeparatorElementIfRequired(rootItem, item.index, item.beginGroup);
        this.RenderSpaceElementIfRequired(rootItem, item.index);

        if(!this.GetItemElement(item.indexPath))
            rootItem.appendChild(element);
    },
    NeedAddPopOutElement: function(rootMenuElement) {
        return ASPx.GetNodesByClassName(rootMenuElement, MenuCssClasses.PopOutContainer).length == 0
    },
    AddPopOutElementToItemElementIfNeeded: function(itemElement, indexPath, withSubitems) {
        if(this.NeedAddPopOutElement(itemElement) && withSubitems) {
            if(this.isPopupMenu || !this.IsRootItem(indexPath)) {
                itemElement.className = itemElement.className.replace(MenuCssClasses.ItemWithoutSubMenu, MenuCssClasses.ItemWithSubMenu);
                itemElement.insertBefore(this.samplePopOutElement.cloneNode(true), itemElement.childNodes[itemElement.childNodes.length - 1]);
            }
        }
    },
    RenderSeparatorElementIfRequired: function(rootItem, index, needAddSeparator) {
        if(needAddSeparator) {
            var item = this.CreateSeparatorElement(index);
            rootItem.appendChild(item);
        }
    },
    RenderSpaceElementIfRequired: function(rootItem, index) {
        if(rootItem.childNodes.length > 0) {
            var item = this.CreateSpacingElement(index);
            rootItem.appendChild(item);
        }
    },
    RenderSubMenuItem: function(indexPath) {
        var subMenuElement = this.CreateSubMenuElement(indexPath);
        this.GetMainElement().parentElement.appendChild(subMenuElement);
        return subMenuElement;
    },
    HasSeparatorOnCurrentPosition: function(itemElements, position) {
        return itemElements[position - 1 > 0 ? position - 1 : 0].className.indexOf(MenuCssClasses.Separator) > -1;
    },
    CreateItemElement: function(item, withSubitems) {
        var itemElement = this.sampleEmptyItemElement.cloneNode();
        var contentElement = this.sampleContentElement.cloneNode();
        this.AddImageToItemElementIfNeeded(item, itemElement, contentElement);
        this.AddTextElementToItemElement(contentElement, itemElement, item.text);
        this.AddPopOutElementToItemElementIfNeeded(itemElement, item.indexPath, withSubitems);
        itemElement.appendChild(this.sampleClearElement.cloneNode());
        itemElement.id = this.GetItemElementId(item.indexPath);
        return itemElement;
    },
    AddTextElementToItemElement: function(contentElement, itemElement, text) {
        var textElement = this.sampleTextElementForItem.cloneNode();
        ASPx.SetInnerHtml(textElement, text);
        contentElement.appendChild(textElement);
        itemElement.appendChild(contentElement);
    },
    AddImageToItemElementIfNeeded: function(item, itemElement, contentElement) {
        if(item.imageSrc || item.imageClassName) {
            var imageElement = this.sampleImageElementForItem.cloneNode();
            if(item.imageSrc)
                imageElement.src = item.imageSrc;
            if(item.imageClassName)
                imageElement.className += " " + item.imageClassName;

            ASPx.RemoveClassNameFromElement(imageElement, Constants.SampleCssClassNameForImageElement);
            ASPx.RemoveClassNameFromElement(itemElement, MenuCssClasses.ItemWithoutImage);
            ASPx.RemoveClassNameFromElement(ASPx.GetNodeByTagName(this.GetMenuElement(item.parent.indexPath), "UL", 0), MenuCssClasses.WithoutImages);
            contentElement.appendChild(imageElement);
        }
    },
    CreateSpacingElement: function(index) {
        var item = this.sampleSpacingElement.cloneNode();
        item.id = this.GetItemIndentElementId(index);
        return item;
    },
    CreateSeparatorElement: function(index) {
        var item = this.sampleSeparatorElement.cloneNode(true);
        item.id = this.GetItemSeparatorElementId(index);
        return item;
    },
    CreateSubMenuElement: function(indexPath) {
        var subMenu = this.sampleSubMenuElement.cloneNode(true);
        subMenu.id =  this.name + Constants.MMIdSuffix + indexPath + "_";
        return subMenu;
    },
    AppendToRenderData: function(rootItemIndexPath, index) {
        if(rootItemIndexPath) {
            if(!this.renderData[rootItemIndexPath])
                this.renderData[rootItemIndexPath] = [[index]];
            this.renderData[rootItemIndexPath][index] = [index];
        } else {
            this.renderData[""].push([[index]]);
        }
    },
    CreateRenderData: function() {
        this.renderData = {"" : []};
    },
    ClearRenderData: function() {
        this.renderData = null;
    },
    GetClientItemType: function() {
        return ASPxClientMenuItem;
    },
    GetItemByIndexPath: function(indexPath) {
        var item = this.rootItem;
        if(indexPath != "" && item != null) {
            var indexes = this.GetItemIndexes(indexPath);
            for(var i = 0; i < indexes.length; i++)
                item = item.GetItem(indexes[i]);
        }
        return item;
    },
    SetItemChecked: function(indexPath, checked) {
        var itemsGroup = this.GetItemsGroup(indexPath);
        if(itemsGroup != null) {
            if(!checked && this.IsCheckedItem(indexPath)) {
                ASPx.Data.ArrayRemove(this.checkedItems, indexPath);
                this.DeselectItem(indexPath);
            }
            else if(checked && !this.IsCheckedItem(indexPath)) {
                if(itemsGroup.length > 1) {
                    for(var i = 0; i < itemsGroup.length; i++) {
                        if(itemsGroup[i] == indexPath) continue;
                        if(this.IsCheckedItem(itemsGroup[i])) {
                            ASPx.Data.ArrayRemove(this.checkedItems, itemsGroup[i]);
                            this.DeselectItem(itemsGroup[i]);
                        }
                    }
                }
                this.SelectItem(indexPath);
                this.checkedItems.push(indexPath);
            }
        }
    },
    ChangeItemEnabledAttributes: function(indexPath, enabled) {
        MenuRenderHelper.ChangeItemEnabledAttributes(this.GetItemElement(indexPath), enabled);
    },
    IsItemEnabled: function(indexPath) {
        var item = this.GetItemByIndexPath(indexPath);
        return (item != null) ? item.GetEnabled() : true;
    },
    SetItemEnabled: function(indexPath, enabled, initialization) {
        if(indexPath == "" || !this.GetItemByIndexPath(indexPath).enabled) return;

        if(!enabled) {
            if(this.GetSelectedItemIndexPath() == indexPath)
                this.DeselectItem(indexPath);
        }

        if(!initialization || !enabled)
            this.ChangeItemEnabledStateItems(indexPath, enabled);
        this.ChangeItemEnabledAttributes(indexPath, enabled);

        if(enabled) {
            if(this.GetSelectedItemIndexPath() == indexPath)
                this.SelectItem(indexPath);
        }
    },
    ChangeItemEnabledStateItems: function(indexPath, enabled) {
        if(!this.IsStateControllerEnabled()) return;

        var element = this.GetItemElement(indexPath);
        if(element)
            ASPx.GetStateController().SetElementEnabled(element, enabled);
    },
    GetItemImageUrl: function(indexPath) {
        var image = this.GetItemImage(indexPath);
        if(image)
            return image.src;
        return "";
    },
    SetItemImageUrl: function(indexPath, url) {
        var image = this.GetItemImage(indexPath);
        if(image)
            image.src = url;
    },
    GetItemImage: function(indexPath) {
        var element = this.GetItemContentElement(indexPath);
        if(element != null) {
            var img = ASPx.GetNodeByTagName(element, "IMG", 0);
            if(img != null)
                return img;
        }
    },
    GetItemNavigateUrl: function(indexPath) {
        var element = this.GetItemContentElement(indexPath);
        if(element != null && element.tagName === "A")
            return element.href || ASPx.Attr.GetAttribute(element, "savedhref");
        if(element != null) {
            var link = ASPx.GetNodeByTagName(element, "A", 0);
            if(link != null)
	            return link.href || ASPx.Attr.GetAttribute(link, "savedhref");
        }
        return "";
    },
    SetItemNavigateUrl: function(indexPath, url) {
        var element = this.GetItemContentElement(indexPath);
	    var setUrl = function(link) {
	        if(link != null) {
	            if(ASPx.Attr.IsExistsAttribute(link, "savedhref"))
	                ASPx.Attr.SetAttribute(link, "savedhref", url);
	            else if(ASPx.Attr.IsExistsAttribute(link, "href"))
                    link.href = url;
            }
	    };
	    if(element != null) {
	        if(element.tagName === "A")
	            setUrl(element);
	        else {
	            setUrl(ASPx.GetNodeByTagName(element, "A", 0));
	            setUrl(ASPx.GetNodeByTagName(element, "A", 1));
	        }
	    }
    },
    FindTextNode: function(indexPath) {
        var element = this.GetItemContentElement(indexPath);
        if(element) {
            var link = ASPx.GetNodeByTagName(element, "A", 0); // B147802
            if(link)
                return ASPx.GetTextNode(link);
            var titleSpan = ASPx.GetNodeByTagName(element, "SPAN", 0); // same as B147802
            if(titleSpan)
                return ASPx.GetTextNode(titleSpan);

            for(var i = 0; i < element.childNodes.length; i++) { // B147763
                var child = element.childNodes[i];
                if(child.nodeValue && (ASPx.Str.Trim(child.nodeValue) != ""))
                    return child;
            }

            return ASPx.GetTextNode(element);
        }
        return null;
    },
    GetItemText: function(indexPath) {
        var textNode = this.FindTextNode(indexPath);
        return textNode
            ? ASPx.Str.Trim(textNode.nodeValue) // B210479
            : "";
    },
    SetItemText: function(indexPath, text) {
        var textNode = this.FindTextNode(indexPath);
        if(textNode) {
            textNode.nodeValue = text;

            var menuElement = this.GetMenuElement(this.GetParentIndexPath(indexPath));
            if(menuElement)
                MenuRenderHelper.CalculateSubMenu(this, menuElement, true);
            if(this.IsRootItem(indexPath) && !this.isPopupMenu) {
                var itemElement = this.GetItemElement(indexPath);
                if(itemElement)
                    MenuRenderHelper.CalculateItemMinSize(itemElement, true);
            }
        }
    },
    SetItemVisible: function(indexPath, visible, initialization) {
        if(indexPath == "" || !this.GetItemByIndexPath(indexPath).visible) return;
        if(visible && initialization) return;
        
        var element = this.GetItemElement(indexPath);
        if(element != null)
            ASPx.SetElementDisplay(element, visible);

        this.SetIndentsVisiblility(indexPath);
        this.SetSeparatorsVisiblility(indexPath);

        var parent = this.GetItemByIndexPath(indexPath).parent;
        var parentHasVisibleItems = this.HasVisibleItems(parent);
        if(this.IsRootItem(indexPath) && !this.isPopupMenu) {
            if(this.clientVisible)
                ASPx.SetElementDisplay(this.GetMainElement(), parentHasVisibleItems);
        }
        else
            this.SetPopOutElementVisible(parent.indexPath, parentHasVisibleItems);
        
        var parentIndexPath = this.GetParentIndexPath(indexPath); //T268967
        if(!this.IsRootItem(parentIndexPath) || this.isPopupMenu) {
            var menuElement = this.GetMenuElement(parentIndexPath);
            if(menuElement) 
                MenuRenderHelper.CalculateSubMenu(this, menuElement, true);
        }
        if(this.IsRootItem(indexPath) && !this.isPopupMenu) 
            MenuRenderHelper.CalculateMenuControl(this, this.GetMainElement(), true);
    },
    SetIndentsVisiblility: function(indexPath) {
        var parent = this.GetItemByIndexPath(indexPath).parent;
        for(var i = 0; i < parent.GetItemCount() ; i++) {
            var item = parent.GetItem(i);
            var separatorVisible = this.HasPrevVisibleItems(parent, i) && item.GetVisible();
            var element = this.GetItemIndentElement(item.GetIndexPath());
            if(element != null) ASPx.SetElementDisplay(element, separatorVisible);
        }
    },
    SetSeparatorsVisiblility: function(indexPath) {
        var parent = this.GetItemByIndexPath(indexPath).parent;
        for(var i = 0; i < parent.GetItemCount() ; i++) {
            var item = parent.GetItem(i);
            var separatorVisible = this.HasPrevVisibleItems(parent, i) && (item.GetVisible() || this.HasNextVisibleItemInGroup(parent, i));
            var element = this.GetItemSeparatorElement(item.GetIndexPath());
            if(element != null) ASPx.SetElementDisplay(element, separatorVisible);
        }
    },
    SetPopOutElementVisible: function(indexPath, visible) {
        var popOutElement = this.GetItemPopOutElement(indexPath);
        if(popOutElement)
            ASPx.SetElementDisplay(popOutElement, visible);
    },
    HasNextVisibleItemInGroup: function(parent, index) {
        for(var i = index + 1; i < parent.GetItemCount() ; i++) {
            var item = parent.GetItem(i);
            if(this.IsItemBeginsGroup(item))
                return false;
            if(item.GetVisible() && !this.IsAdaptiveItem(item.indexPath))
                return true;
        }
        return false;
    },
    IsItemBeginsGroup: function(item) {
        var itemSeparator = this.GetItemSeparatorElement(item.GetIndexPath());
        return itemSeparator && ASPx.ElementContainsCssClass(itemSeparator, MenuCssClasses.Separator);
    },
    HasVisibleItems: function(parent) {
        for(var i = 0; i < parent.GetItemCount() ; i++) {
            if(parent.GetItem(i).GetVisible())
                return true;
        }
        return false;
    },
    HasNextVisibleItems: function(parent, index) {
        for(var i = index + 1; i < parent.GetItemCount() ; i++) {
            if(parent.GetItem(i).GetVisible())
                return true;
        }
        return false;
    },
    HasPrevVisibleItems: function(parent, index) {
        for(var i = index - 1; i >= 0; i--) {
            if(parent.GetItem(i).GetVisible())
                return true;
        }
        return false;
    },

    GetItemIndentElement: function(indexPath) {
        return ASPx.GetElementById(this.GetItemIndentElementId(indexPath));
    },
    GetItemSeparatorElement: function(indexPath) {
        return ASPx.GetElementById(this.GetItemSeparatorElementId(indexPath));
    },

    RaiseItemClick: function(indexPath, htmlEvent) {
        var processOnServer = this.autoPostBack || this.IsServerEventAssigned("ItemClick");
        if(!this.ItemClick.IsEmpty()) {
            var item = this.GetItemByIndexPath(indexPath);
            var htmlElement = this.GetItemContentElement(indexPath);
            var args = new ASPxClientMenuItemClickEventArgs(processOnServer, item, htmlElement, htmlEvent);
            this.ItemClick.FireEvent(this, args);
            processOnServer = args.processOnServer;
        }
        return processOnServer;
    },
    RaiseItemMouseOver: function(indexPath) {
        if(!this.ItemMouseOver.IsEmpty()) {
            var item = this.GetItemByIndexPath(indexPath);
            var htmlElement = this.GetItemContentElement(indexPath);
            var args = new ASPxClientMenuItemMouseEventArgs(item, htmlElement);
            this.ItemMouseOver.FireEvent(this, args);
        }
    },
    RaiseItemMouseOut: function(indexPath) {
        if(!this.ItemMouseOut.IsEmpty()) {
            var item = this.GetItemByIndexPath(indexPath);
            var htmlElement = this.GetItemContentElement(indexPath);
            var args = new ASPxClientMenuItemMouseEventArgs(item, htmlElement);
            this.ItemMouseOut.FireEvent(this, args);
        }
    },
    RaisePopUp: function(indexPath) {
        var item = this.GetItemByIndexPath(indexPath);
        if(!this.PopUp.IsEmpty()) {
            var args = new ASPxClientMenuItemEventArgs(item);
            this.PopUp.FireEvent(this, args);
        }
    },
    RaiseCloseUp: function(indexPath) {
        var item = this.GetItemByIndexPath(indexPath);
        if(!this.CloseUp.IsEmpty()) {
            var args = new ASPxClientMenuItemEventArgs(item);
            this.CloseUp.FireEvent(this, args);
        }
    },
    SetEnabled: function(enabled) {
        for(var i = this.GetItemCount() - 1; i >= 0; i--) {
            var item = this.GetItem(i);
            item.SetEnabled(enabled);
        }
    },
    SetVisible: function(visible) {
        ASPxClientControl.prototype.SetVisible.call(this, visible);
        if(visible && !this.HasVisibleItems(this))
            ASPx.SetElementDisplay(this.GetMainElement(), false);
    },
    GetItemCount: function() {
        return (this.rootItem != null) ? this.rootItem.GetItemCount() : 0;
    },
    GetItem: function(index) {
        return (this.rootItem != null) ? this.rootItem.GetItem(index) : null;
    },
    GetItemByName: function(name) {
        return (this.rootItem != null) ? this.rootItem.GetItemByName(name) : null;
    },
    GetSelectedItem: function() {
        var indexPath = this.GetSelectedItemIndexPath();
        if(indexPath != "")
            return this.GetItemByIndexPath(indexPath);
        return null;
    },
    SetSelectedItem: function(item) {
        var indexPath = (item != null) ? item.GetIndexPath() : "";
        this.SetSelectedItemInternal(indexPath, false);
    },
    GetRootItem: function() {
        return this.rootItem;
    }
});
ASPxClientMenuBase.GetMenuCollection = function() {
    return aspxGetMenuCollection();
}
var ASPxClientMenuCollection = ASPx.CreateClass(ASPxClientControlCollection, {
    constructor: function() {
        this.constructor.prototype.constructor.call(this);

        this.appearTimerID = -1;
        this.disappearTimerID = -1;
        this.currentShowingPopupMenuName = null;
        this.visibleSubMenusMenuName = "";
        this.visibleSubMenuIds = [];
        this.overXPos = -1;
        this.overYPos = -1;
    },
    GetCollectionType: function(){
        return "Menu";
    },
    Remove: function(element) {
        if(element.name === this.visibleSubMenusMenuName) {
            this.visibleSubMenusMenuName = "";
            this.visibleSubMenuIds = [ ];
        }
        ASPxClientControlCollection.prototype.Remove.call(this, element);
    },
    RegisterVisiblePopupMenu: function(name, id) {
        this.visibleSubMenuIds.push(id);
        this.visibleSubMenusMenuName = name;
    },
    UnregisterVisiblePopupMenu: function(name, id) {
        ASPx.Data.ArrayRemove(this.visibleSubMenuIds, id);
        if(this.visibleSubMenuIds.length == 0)
            this.visibleSubMenusMenuName = "";
    },
    IsSubMenuVisible: function(subMenuId) {
        for(var i = 0; i < this.visibleSubMenuIds.length; i++) {
            if(this.visibleSubMenuIds[i] == subMenuId)
                return true;
        }
        return false;
    },

    GetMenu: function(id) {
        return this.Get(this.GetMenuName(id));
    },
    GetMenuName: function(id) {
        return this.GetMenuNameBySuffixes(id, [Constants.MMIdSuffix, Constants.MIIdSuffix]);
    },
    GetMenuNameBySuffixes: function(id, idSuffixes) {
        for(var i = 0; i < idSuffixes.length; i++) {
            var pos = id.lastIndexOf(idSuffixes[i]);
            if(pos > -1)
                return id.substring(0, pos);
        }
        return id;
    },
    ClearCurrentShowingPopupMenuName: function() {
        this.SetCurrentShowingPopupMenuName(null);
    },
    SetCurrentShowingPopupMenuName: function(value) {
        this.currentShowingPopupMenuName = value;
    },
    NowPopupMenuIsShowing: function() {
        return this.currentShowingPopupMenuName != null;
    },

    GetMenuLevelById: function(id) {
        var indexPath = this.GetIndexPathById(id, Constants.MMIdSuffix);
        var menu = this.GetMenu(id);
        return menu.GetMenuLevel(indexPath);
    },
    GetIndexPathById: function(id, idSuffix) {
        var pos = id.lastIndexOf(idSuffix);
        if(pos > -1) {
            id = id.substring(pos + idSuffix.length);
            pos = id.lastIndexOf("_");
            if(pos > -1)
                return id.substring(0, pos);
        }
        return "";
    },
    GetItemIndexPath: function(indexes) {
        var indexPath = "";
        for(var i = 0; i < indexes.length; i++) {
            indexPath += indexes[i];
            if(i < indexes.length - 1)
                indexPath += ASPx.ItemIndexSeparator;
        }
        return indexPath;
    },
    GetItemIndexes: function(indexPath) {
        var indexes = indexPath.split(ASPx.ItemIndexSeparator);
        for(var i = 0; i < indexes.length; i++)
            indexes[i] = parseInt(indexes[i]);
        return indexes;
    },
    ClearAppearTimer: function() {
        this.appearTimerID = ASPx.Timer.ClearTimer(this.appearTimerID);
    },
    ClearDisappearTimer: function() {
        this.disappearTimerID = ASPx.Timer.ClearTimer(this.disappearTimerID);
    },
    IsAppearTimerActive: function() {
        return this.appearTimerID > -1;
    },
    IsDisappearTimerActive: function() {
        return this.disappearTimerID > -1;
    },
    SetAppearTimer: function(name, indexPath, timeout, preventSubMenu) {
        this.appearTimerID = window.setTimeout(function() {
            var menu = aspxGetMenuCollection().Get(name);
            if(menu != null) menu.OnItemOverTimer(indexPath, preventSubMenu);
        }, timeout);
    },
    SetDisappearTimer: function(name, timeout) {
        this.disappearTimerID = window.setTimeout(function() {
            var menu = aspxGetMenuCollection().Get(name);
            if(menu != null)
                menu.OnItemOutTimer();
        }, timeout);
    },

    GetMouseDownMenuLevel: function(evt) {
        var srcElement = ASPx.Evt.GetEventSource(evt);
        if(this.visibleSubMenusMenuName != "") {
            var element = ASPx.GetParentById(srcElement, this.visibleSubMenusMenuName);
            if(element != null) return 1;
        }
        for(var i = 0; i < this.visibleSubMenuIds.length; i++) {
            var element = ASPx.GetParentById(srcElement, this.visibleSubMenuIds[i]);
            if(element != null)
                return this.GetMenuLevelById(this.visibleSubMenuIds[i]) + 1;
        }
        return -1;
    },
    CheckFocusedElement: function() {
        try {
            if(document.activeElement != null) {
                for(var i = 0; i < this.visibleSubMenuIds.length; i++) {
                    var menuElement = ASPx.GetElementById(this.visibleSubMenuIds[i]);
                    if(menuElement != null && ASPx.GetIsParent(menuElement, document.activeElement)) {
                        var tagName = document.activeElement.tagName;
                        if(!ASPx.Browser.IE || tagName == "INPUT" || tagName == "TEXTAREA" || tagName == "SELECT")
                            return true;
                    }
                }
            }
        } catch (e) {

        }
        return false;
    },
    DoHidePopupMenus: function(evt, level, name, leavePopups, exceptId) {
        for(var i = this.visibleSubMenuIds.length - 1; i >= 0 ; i--) {
            var menu = this.GetMenu(this.visibleSubMenuIds[i]);
            if(menu != null) {
                var menuLevel = this.GetMenuLevelById(this.visibleSubMenuIds[i]);
                if((!leavePopups || menuLevel > 0) && exceptId != this.visibleSubMenuIds[i]) {
                    if(menuLevel > level || (menu.name != name && name != "")) {
                        var element = ASPx.GetElementById(this.visibleSubMenuIds[i]);
                        if(element != null)
                            menu.DoHidePopupMenu(evt, element);
                    }
                }
            }
        }
    },
    DoShowAtCurrentPos: function(name, indexPath) {
        var pc = this.Get(name);
        var element = pc.GetMainElement();
        if(pc != null && !ASPx.GetElementDisplay(element))
            pc.DoShowPopupMenu(element, this.overXPos, this.overYPos, indexPath);
    },
    SaveCurrentMouseOverPos: function(evt, popupElement) {
        if(!this.NowPopupMenuIsShowing()) return;
        var currentShowingPopupMenu = this.Get(this.currentShowingPopupMenuName);
        if(currentShowingPopupMenu.popupElement == popupElement)
            if(!currentShowingPopupMenu.IsMenuVisible()) {
                this.overXPos = ASPx.Evt.GetEventX(evt);
                this.overYPos = ASPx.Evt.GetEventY(evt);
            }
    },

    OnMouseDown: function(evt) {
        var menuLevel = this.GetMouseDownMenuLevel(evt);
        this.DoHidePopupMenus(evt, menuLevel, "", false, "");
    },
    HideAll: function() {
        this.DoHidePopupMenus(null, -1, "", false, "");
    },
    IsAnyMenuVisible: function() {
        return this.visibleSubMenuIds.length != 0;
    }
});

var menuCollection = null;
function aspxGetMenuCollection() {
    if(menuCollection == null)
        menuCollection = new ASPxClientMenuCollection();
    return menuCollection;
}
var ASPxClientMenuItem = ASPx.CreateClass(null, {
    constructor: function(menu, parent, index, name) {
        this.menu = menu;
        this.parent = parent;
        this.index = index;
        this.name = name;
        this.indexPath = "";

        this.text = "";
        this.imageSrc = "";
        this.imageClassName = "";
        this.beginGroup = false;

        if(parent) {
            this.indexPath = this.CreateItemIndexPath(parent);
        }

        this.enabled = true;
        this.clientEnabled = true;
        this.visible = true;
        this.clientVisible = true;
        this.items = [];
    },
    CreateItemIndexPath: function(parent) {
        return parent.indexPath ? parent.indexPath + ASPx.ItemIndexSeparator + this.index.toString() : this.index.toString();
    },
    CreateItems: function(itemsProperties) {
	    var itemType = this.menu.GetClientItemType();
	    for(var i = 0; i < itemsProperties.length; i++) {
            var itemProperties = itemsProperties[i];
            var item = this.CreateItemInternal(itemProperties, i);

            if(itemProperties.items)
		        item.CreateItems(itemProperties.items);
	    }
    },
    CreateItemInternal: function(itemProperties, index) {
        var itemName = itemProperties.name || "";
        var correctIndex = index ? index : this.items.length;
        var itemType = this.menu.GetClientItemType();
        var item = new itemType(this.menu, this, correctIndex, itemName);

        if(ASPx.IsExists(itemProperties.text))
            item.text = itemProperties.text;
        if(ASPx.IsExists(itemProperties.imageSrc))
            item.imageSrc = itemProperties.imageSrc;
        if(ASPx.IsExists(itemProperties.imageClassName))
            item.imageClassName = itemProperties.imageClassName;
        if(ASPx.IsExists(itemProperties.beginGroup))
            item.beginGroup = itemProperties.beginGroup;
        if(ASPx.IsExists(itemProperties.enabled))
            item.enabled = itemProperties.enabled;
        if(ASPx.IsExists(itemProperties.clientEnabled))
            item.clientEnabled = itemProperties.clientEnabled;
        if(ASPx.IsExists(itemProperties.visible))
            item.visible = itemProperties.visible;
        if(ASPx.IsExists(itemProperties.clientVisible))
            item.clientVisible = itemProperties.clientVisible;

        if(this.menu.NeedAppendToRenderData(item))
            this.menu.AppendToRenderData(this.indexPath, correctIndex);

        this.items.push(item);
        return item;
    },
    GetIndexPath: function() {
        return this.indexPath;
    },
    GetItemCount: function() {
        return this.items.length;
    },
    GetItem: function(index) {
        return (0 <= index && index < this.items.length) ? this.items[index] : null;
    },
    GetItemByName: function(name) {
        for(var i = 0; i < this.items.length; i++)
            if(this.items[i].name == name) return this.items[i];
        for(var i = 0; i < this.items.length; i++) {
            var item = this.items[i].GetItemByName(name);
            if(item != null) return item;
        }
        return null;
    },
    GetChecked: function() {
        var indexPath = this.GetIndexPath();
        return this.menu.IsCheckedItem(indexPath);
    },
    SetChecked: function(value) {
        var indexPath = this.GetIndexPath();
        this.menu.SetItemChecked(indexPath, value);
    },
    GetEnabled: function() {
        return this.enabled && this.clientEnabled;
    },
    SetEnabled: function(value) {
        if(this.clientEnabled != value) {
            this.clientEnabled = value;
            this.menu.SetItemEnabled(this.GetIndexPath(), value, false);
        }
    },
    GetImage: function() {
        return this.menu.GetItemImage(this.GetIndexPath());
    },
    GetImageUrl: function() {
        return this.menu.GetItemImageUrl(this.GetIndexPath());
    },
    SetImageUrl: function(value) {
        var indexPath = this.GetIndexPath();
        this.menu.SetItemImageUrl(indexPath, value);
    },
    GetNavigateUrl: function() {
        var indexPath = this.GetIndexPath();
        return this.menu.GetItemNavigateUrl(indexPath);
    },
    SetNavigateUrl: function(value) {
        var indexPath = this.GetIndexPath();
        this.menu.SetItemNavigateUrl(indexPath, value);
    },
    GetText: function() {
        var indexPath = this.GetIndexPath();
        return this.menu.GetItemText(indexPath);
    },
    SetText: function(value) {
        var indexPath = this.GetIndexPath();
        this.menu.SetItemText(indexPath, value);
    },
    GetVisible: function() {
        return this.visible && this.clientVisible;
    },
    SetVisible: function(value) {
        if(this.clientVisible != value) {
            this.clientVisible = value;
            this.menu.SetItemVisible(this.GetIndexPath(), value, false);
        }
    },

    InitializeEnabledAndVisible: function(recursive) {
        this.menu.SetItemEnabled(this.GetIndexPath(), this.clientEnabled, true);
        this.menu.SetItemVisible(this.GetIndexPath(), this.clientVisible, true);
        if(recursive) {
            for(var i = 0; i < this.items.length; i++)
                this.items[i].InitializeEnabledAndVisible(recursive);
        }
    }
});
var ASPxClientMenu = ASPx.CreateClass(ASPxClientMenuBase, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.isVertical = false;
        this.orientationChanged = false;
        this.firstSubMenuDirection = "Auto";
    },

    IsVertical: function(indexPath) {
        return this.isVertical || !this.IsRootItem(indexPath) || this.IsAdaptiveMenuItem(indexPath);
    },

    IsCorrectionDisableMethodRequired: function(indexPath) {
        return (indexPath.indexOf("i") == -1) && (this.firstSubMenuDirection == "RightOrBottom" || this.firstSubMenuDirection == "LeftOrTop");
    },
    SetAdaptiveMode: function(data) {
        this.enableAdaptivity = true;

        if(ASPx.Ident.IsArray(data))
            this.adaptiveItemsOrder = data;
        else {
            for(var i = data - 1; i >= 0; i--)
                this.adaptiveItemsOrder.push(i.toString());
        }
    }, 

    OnBrowserWindowResize: function (evt) {
        this.AdjustControl();
    },
    AdjustControlCore: function() {
        this.CorrectVerticalAlignment(ASPx.ClearHeight, this.GetPopOutElements, "PopOut", true);
        this.CorrectVerticalAlignment(ASPx.ClearVerticalMargins, this.GetPopOutImages, "PopOutImg", true);

        if(this.orientationChanged){
            MenuRenderHelper.ChangeOrientaion(this.isVertical, this, this.GetMainElement());
            this.orientationChanged = false;
        }
        else
            MenuRenderHelper.CalculateMenuControl(this, this.GetMainElement());

        this.CorrectVerticalAlignment(ASPx.AdjustHeight, this.GetPopOutElements, "PopOut", true);
        this.CorrectVerticalAlignment(ASPx.AdjustVerticalMargins, this.GetPopOutImages, "PopOutImg", true);
    },

    GetCorrectionDisabledResult: function(x, toLeftX) {
        switch (this.firstSubMenuDirection) {
            case "RightOrBottom": {
                this.popupToLeft = false;
                return x;
            }
            case "LeftOrTop": {
                this.popupToLeft = true;
                return toLeftX;
            }
        }
    },

    IsHorizontalSubmenuNeedInversion: function(subMenuBottom, docClientHeight, menuItemTop, subMenuHeight, itemHeight) {
        if(this.firstSubMenuDirection == "Auto")
            return ASPxClientMenuBase.prototype.IsHorizontalSubmenuNeedInversion.call(this, subMenuBottom, docClientHeight, menuItemTop, subMenuHeight, itemHeight);
        return this.firstSubMenuDirection == "LeftOrTop"
    },
    GetOrientation: function() {
        return this.isVertical ? "Vertical" : "Horizontal";
    },
    SetOrientation: function(orientation) {
        var isVertical = orientation === "Vertical";
        if(this.isVertical !== isVertical){
            this.isVertical = isVertical;
            this.orientationChanged = true;
            this.ResetControlAdjustment();
            this.AdjustControl();
        }
    }
});

var ASPxClientMenuExt = ASPx.CreateClass(ASPxClientMenu, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);

        this.INDEX_ROOTMENU_ITEM = 0;
        this.INDEX_MAINMENU_ELEMENT = 1;
        this.INDEX_SUBMENU_ELEMENT = 2;
        
    },
    InitializeMenuSamples: function() {
        var menu = this.InitializeSampleMenuElement();
        this.InitializeRootItemSample(menu);
        this.InitializeItemSamples(this.GetMainMenuElementSample(menu));
        this.InitializeMenuSamplesInternal(menu);
        this.InitializeSubMenuSample(this.GetSubMenuElementSample(menu));
    },
    GetMainMenuElementSample: function(menu) {
        return menu.childNodes[this.INDEX_MAINMENU_ELEMENT];
    },
    GetSubMenuElementSample: function(menu) {
        return menu.childNodes[this.INDEX_SUBMENU_ELEMENT];
    },
    InitializeRootItemSample: function(sample) {
        this.rootMenuSample = this.GetRootMenuItemSample(sample).cloneNode(true);
        ASPx.RemoveElement(ASPx.GetNodeByTagName(this.rootMenuSample, "LI", 0));
        sample.removeChild(this.GetRootMenuItemSample(sample));
    },
    GetRootMenuItemSample: function(menu) {
        return menu.childNodes[this.INDEX_ROOTMENU_ITEM];
    },
    NeedCreateItemsOnClientSide: function() {
        return true;
    }
});
ASPxClientMenu.Cast = ASPxClientControl.Cast;
var ASPxClientMenuItemEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(item) {
        this.constructor.prototype.constructor.call(this);
        this.item = item;
    }
});
var ASPxClientMenuItemMouseEventArgs = ASPx.CreateClass(ASPxClientMenuItemEventArgs, {
    constructor: function(item, htmlElement) {
        this.constructor.prototype.constructor.call(this, item);
        this.htmlElement = htmlElement;
    }
});
var ASPxClientMenuItemClickEventArgs = ASPx.CreateClass(ASPxClientProcessingModeEventArgs, {
    constructor: function(processOnServer, item, htmlElement, htmlEvent) {
        this.constructor.prototype.constructor.call(this, processOnServer);
        this.item = item;
        this.htmlElement = htmlElement;
        this.htmlEvent = htmlEvent;
    }
});

ASPx.Evt.AttachEventToDocument(ASPx.TouchUIHelper.touchMouseDownEventName, function(evt) {
    return aspxGetMenuCollection().OnMouseDown(evt);
});

function aspxAMIMOver(source, args) {
    var menu = aspxGetMenuCollection().GetMenu(args.item.name);
    if(menu != null) menu.OnAfterItemOver(args.item, args.element);
}
function aspxBMIMOver(source, args) {
    var menu = aspxGetMenuCollection().GetMenu(args.item.name);
    if(menu != null) menu.OnBeforeItemOver(args.item, args.element);
}
function aspxAMIMOut(source, args) {
    var menu = aspxGetMenuCollection().GetMenu(args.item.name);
    if(menu != null) menu.OnAfterItemOut(args.item, args.element, args.toElement);
}
function aspxMSBOver(source, args) {
    var menu = MenuScrollHelper.GetMenuByScrollButtonId(args.element.id)
    if(menu != null) menu.ClearDisappearTimer();
}

ASPx.AddAfterSetFocusedState(aspxAMIMOver);
ASPx.AddAfterClearFocusedState(aspxAMIMOut);
ASPx.AddAfterSetHoverState(aspxAMIMOver);
ASPx.AddAfterClearHoverState(aspxAMIMOut);
ASPx.AddBeforeSetFocusedState(aspxBMIMOver);
ASPx.AddBeforeSetHoverState(aspxBMIMOver);
ASPx.AddAfterSetHoverState(aspxMSBOver);
ASPx.AddAfterSetPressedState(aspxMSBOver);

ASPx.AddBeforeDisabled(function(source, args) {
    var menu = aspxGetMenuCollection().GetMenu(args.item.name);
    if(menu != null)
        menu.OnBeforeItemDisabled(args.item, args.element);
});
ASPx.AddFocusedItemKeyDown(function(source, args) {
    var menu = aspxGetMenuCollection().GetMenu(args.item.name);
    if(menu != null)
        menu.OnFocusedItemKeyDown(args.htmlEvent, args.item, args.element);
});
ASPx.AddAfterClearHoverState(function(source, args) {
    var menu = MenuScrollHelper.GetMenuByScrollButtonId(args.element.id)
    if(menu != null) menu.SetDisappearTimer();
});
ASPx.AddAfterSetPressedState(function(source, args) {
    var menu = MenuScrollHelper.GetMenuByScrollButtonId(args.element.id);
    if(menu) menu.StartScrolling(args.element.id, 1, 4);
});
ASPx.AddAfterClearPressedState(function(source, args) {
    var menu = MenuScrollHelper.GetMenuByScrollButtonId(args.element.id);
    if(menu) menu.StopScrolling(args.element.id);
});

if(!ASPx.Browser.TouchUI) {
    ASPx.AddAfterSetHoverState(function(source, args) {
        var menu = MenuScrollHelper.GetMenuByScrollButtonId(args.element.id);
        if(menu) menu.StartScrolling(args.element.id, 15, 1);
    });
    ASPx.AddAfterClearHoverState(function(source, args) {
        var menu = MenuScrollHelper.GetMenuByScrollButtonId(args.element.id);
        if(menu) menu.StopScrolling(args.element.id);
    });
}

ASPx.MIClick = function(evt, name, indexPath) {
    if(ASPx.TouchUIHelper.isMouseEventFromScrolling) return;

    var menu = aspxGetMenuCollection().Get(name);
    if(menu != null) menu.OnItemClick(indexPath, evt);
    if(!ASPx.Browser.NetscapeFamily)
        evt.cancelBubble = true;
}
ASPx.MIDDClick = function(evt, name, indexPath) {
    var menu = aspxGetMenuCollection().Get(name);
    if(menu != null) menu.OnItemDropDownClick(indexPath, evt);
    if(!ASPx.Browser.NetscapeFamily)
        evt.cancelBubble = true;
}

ASPx.GetMenuCollection = aspxGetMenuCollection;

window.ASPxClientMenuBase = ASPxClientMenuBase;
window.ASPxClientMenuCollection = ASPxClientMenuCollection;
window.ASPxClientMenuItem = ASPxClientMenuItem;
window.ASPxClientMenu = ASPxClientMenu;
window.ASPxClientMenuExt = ASPxClientMenuExt;

window.ASPxClientMenuItemEventArgs = ASPxClientMenuItemEventArgs;
window.ASPxClientMenuItemMouseEventArgs = ASPxClientMenuItemMouseEventArgs;
window.ASPxClientMenuItemClickEventArgs = ASPxClientMenuItemClickEventArgs;
})();