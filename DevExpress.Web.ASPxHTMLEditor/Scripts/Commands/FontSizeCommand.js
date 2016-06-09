(function() {
    var defaultSafariFontSizesHashTable = null;
    var defaultSafariFontSizesInPixelHashTable = null;
    var defaultFontSizesHashTable = null;
    
    ASPx.HtmlEditorClasses.Commands.Browser.FontSize  = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.WrappedCommand, {
        ExecuteCore: function(cmdValue, wrapper) {
            if(wrapper.attributeIsFiltered("style") || wrapper.styleAttributeIsFiltered("font-size"))
                return false;
            if(cmdValue) {
                var value = parseInt(cmdValue);
                var unitMeasure = typeof(cmdValue) == "string" && cmdValue.indexOf("%") > -1 && cmdValue.indexOf("px") > -1 && cmdValue.indexOf("pt") > -1;
                if(!unitMeasure && (value > 0 && value < 8))
                    this.cmdValue = ASPx.HtmlEditorClasses.DefaultFontSizes[value - 1];
                else
                    this.cmdValue = cmdValue;
            }
            return this.ExecuteInternal(cmdValue, wrapper);
        },
        getFontSizeValue: function(wrapper, selection, selectedElements) {
            var styleAttrValue;
            if(wrapper.isInFocus && (ASPx.Browser.WebKitTouchUI || ASPx.Browser.MSTouchUI))
                styleAttrValue = this.TryGetValue(wrapper);
            else {
                if(this.owner.typeCommandProcessor.hasApplyStyleCommand(this.GetCommandID()))
                    return this.cmdValue;
                var selectedParent;
                if(wrapper.isInFocus) {
                    selection = !selection ? wrapper.getSelection() : selection;
                    selectedParent = this.GetSelectedElement(wrapper, selection, selectedElements);
                    if(selectedElements && selectedElements.length == 1 && selectedElements[0].nodeType != 3 && selectedElements[0].childNodes.length == 0)
                        selectedElements = [];
                }
                if(!selectedParent || this.canCheckSelectedElementStyle(selection, selectedParent, selectedElements))
                    return this.GetCurrentStyleAttributeValue(selectedParent ? selectedParent : wrapper.getBody());
                selectedElements = !selectedElements ? selection.GetElements(true) : selectedElements;
                if(selectedElements.length > 0)
                    styleAttrValue = this.GetCommonStyleAttributeValue(selectedElements);
                if(!styleAttrValue)
                    return "";
            }
            return styleAttrValue;
        },
        GetValue: function(wrapper, selection, selectedElements) {
            var result = this.getFontSizeValue(wrapper, selection, selectedElements);
            return result ? this.getDefaultFontSizeValue(result) : "";
        },
        GetStyleAttributeConstValue: function() {
            return this.cmdValue;
        },
        GetAttributeValue: function(element, attr) {
            var attrValue = "" + ASPx.Attr.GetAttribute(element.nodeType == 1 ? element : element.parentNode, attr);
            if(parseInt(attrValue) > 0)
                return attrValue;
            return "";
        },
        IsTagFontStyle: function(element) {
            return this.GetCurrentStyleAttributeValue(element) == this.cmdValue;
        },
        getDefaultFontSizeValue: function(styleAttrValue) {
            if(styleAttrValue && typeof(styleAttrValue) == 'string') {
                var unitMeasure = styleAttrValue.indexOf("px") > -1 ? "px"
                                : styleAttrValue.indexOf("pt") > -1 ? "pt"
                                : undefined;
                if(unitMeasure) {
                    var value = (Math.round(parseFloat(styleAttrValue))).toString();
                    if(isNaN(value))
                        return "";
                    styleAttrValue = value + unitMeasure;
                    if(unitMeasure == "px") {
                        var index = this.getDefaultSafariFontSizesInPixelHashTable()[styleAttrValue];
                        return index > -1 ? index + 1 : "";
                    }
                    if(unitMeasure ==  "pt") {
                        var index = this.getDefaultSafariFontSizesHashTable()[styleAttrValue];
                        if(!index || index == -1)
                            index = this.getDefaultFontSizesHashTable()[styleAttrValue];
                        return index > -1 ? index + 1 : "";
                    }
                }
            }
            return styleAttrValue;
        },
        GetStyleAttribute: function() {
            return "fontSize";
        },
        GetAttribute: function() {
            return "size";
        },
        getDefaultSafariFontSizesHashTable: function() {
            if(!defaultSafariFontSizesHashTable)
                defaultSafariFontSizesHashTable = ASPx.Data.CreateIndexHashTableFromArray(this.getDefaultSafariFontSizesArray());
            return defaultSafariFontSizesHashTable;
        },
        getDefaultSafariFontSizesInPixelHashTable: function() {
            if(!defaultSafariFontSizesInPixelHashTable)
                defaultSafariFontSizesInPixelHashTable = ASPx.Data.CreateIndexHashTableFromArray(this.getDefaultSafariFontSizesArrayInPixel());
            return defaultSafariFontSizesInPixelHashTable;
        },
        getDefaultFontSizesHashTable: function() {
            if(!defaultFontSizesHashTable)
                defaultFontSizesHashTable = ASPx.Data.CreateIndexHashTableFromArray(this.getDefaultFontSizesArray());
            return defaultFontSizesHashTable;
        },
        getDefaultSafariFontSizesArray: function() {
            return ["x-small", "small", "medium", "large", "x-large", "xx-large", "-webkit-xxx-large"];
        },
        getDefaultSafariFontSizesArrayInPixel: function() {
            return ["11px", "13px", "16px", "19px", "24px", "32px", "48px"];
        },
        getDefaultFontSizesArray: function() {
            return ASPx.HtmlEditorClasses.DefaultFontSizes;
        }
    });
})();