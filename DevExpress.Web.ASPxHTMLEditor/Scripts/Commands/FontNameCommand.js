(function() {
    var defaultFontNamesArray = ["Times New Roman", "Tahoma", "Verdana", "Arial", "MS Sans Serif", "Courier", "Helvetica", "Segoe UI"];
    ASPx.HtmlEditorClasses.Commands.Browser.FontName  = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Browser.WrappedCommand, {
        ExecuteCore: function(cmdValue, wrapper) {
            if(wrapper.attributeIsFiltered("style") || wrapper.styleAttributeIsFiltered("font-family"))
                return false;
            if(cmdValue)
                this.cmdValue = cmdValue;
            return this.ExecuteInternal(cmdValue, wrapper);
        },
        GetStyleAttributeConstValue: function() {
            return this.cmdValue;
        },
        IsTagFontStyle: function(element) {
            return true;
        },
        GetValueCore: function(wrapper, selection, selectedParent, selectedElements) {
            try {
                var attrValue = this.getFontNameValue(wrapper, selection, selectedParent, selectedElements);
                return attrValue ? this.getDefaulFontNameValue(attrValue) : "";
	        }
	        catch(e) {}
	        return "";
        },
        getFontNameValue: function(wrapper, selection, selectedParent, selectedElements) {
            try {
                if(this.owner.typeCommandProcessor.hasApplyStyleCommand(this.GetCommandID()))
                    return this.cmdValue;
                return !selection || this.canCheckSelectedElementStyle(selection, selectedParent, selectedElements) ?
                    this.GetCurrentStyleAttributeValue(selection ? selectedParent : wrapper.getBody()) :
                    this.GetCommonStyleAttributeValue(!selectedElements ? selection.GetElements(true) : selectedElements);
	        }
	        catch(e) {}
	        return "";
        },
        getDefaulFontNameValue: function(attrValue) {
            if(attrValue) {
                var comparer = function(el1, el2) { return this.getProcessingValue(el1) == this.getProcessingValue(el2); }.aspxBind(this);
                var index = ASPx.Data.ArrayIndexOf(defaultFontNamesArray, attrValue, comparer);
                if(index > -1)
                    return defaultFontNamesArray[index];
                var atteValueArray = attrValue.split(',');
                for(var i = 0, value; value = atteValueArray[i]; i++) {
                    if(ASPx.Data.ArrayIndexOf(defaultFontNamesArray, value, comparer) > -1)
                        return value;
                }
            } 
            return "";
        },
        getProcessingValue: function(value) {
            value = value.replace(/'|"/gi, "");
            var splitValue = value.split(",");
            if(splitValue.length > 0) {
                value = "";
                for(var i = 0, item; item = splitValue[i]; i++) {
                    if(!item)
                        continue;
                    if(value)
                        value += ",";
                    value += ASPx.Str.Trim(item);
                }
            }
            else
                value = ASPx.Str.Trim(value);
            return value;
        },
        NeedUseCss: function() {
            return !(ASPx.Browser.Firefox && ASPx.Browser.Version >= 4);
        },
        GetStyleAttribute: function() {
            return "fontFamily";
        },
        GetAttribute: function() {
            return "face";
        },
        getDefaultFontNames: function() {
            return defaultFontNamesArray;
        },
        setDefaultFontNames: function(fontNamesArray) {
            defaultFontNamesArray = fontNamesArray;
        }
    });
})();