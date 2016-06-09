(function() {
    var heMarkID = "dxMID";
    
    ASPx.HtmlEditorClasses.Commands.ChangeImage = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
         // cmdValue = { imageElement, width, height, align, alt };
	    Execute: function(cmdValue, wrapper) {
    	    ASPx.HtmlEditorClasses.Commands.ChangeImage.SetImageProperties(cmdValue.selectedElement, cmdValue);
    	    return true;
        }
    });
    ASPx.HtmlEditorClasses.Commands.ChangeImage.GetUnitAttributeValue = function(element, attrName) {
        var attrValue = ASPx.Attr.GetAttribute(element, attrName);
        if(attrValue && attrValue.replace)
            attrValue = attrValue.replace("px", "");
        return attrValue;
    };
    ASPx.HtmlEditorClasses.Commands.ChangeImage.GetImageProperties = function(imageElement) {
        var GetUnitAttributeValue = ASPx.HtmlEditorClasses.Commands.ChangeImage.GetUnitAttributeValue;
        var getAttrValue = function(element, attrName) {
            var attrValue = GetUnitAttributeValue(element, attrName);
            var styleAttrValue = GetUnitAttributeValue(element.style, attrName);
            return styleAttrValue ? styleAttrValue : attrValue;
        }
        var imageInfoObject = {
            isCustomSize: false,
            src: ASPx.Browser.IE ? imageElement.getAttribute("src", 2) : ASPx.Attr.GetAttribute(imageElement, "src"),
            width: imageElement.width,
            height: imageElement.height,
            styleWidth: getAttrValue(imageElement, "width"),
            styleHeight: getAttrValue(imageElement, "height"),
            align: "",
            alt: "",
            useFloat: false,
            alt: ASPx.Attr.GetAttribute(imageElement, "alt"),
            cssClass: imageElement.className,
            borderStyle: GetUnitAttributeValue(imageElement.style, "border-style"),
            borderWidth: GetUnitAttributeValue(imageElement.style, "border-width"),
            borderColor: ASPx.Color.ColorToHexadecimal(ASPx.Attr.GetAttribute(imageElement.style, "border-color")),
            marginTop: GetUnitAttributeValue(imageElement.style, "margin-top"),
            marginLeft: GetUnitAttributeValue(imageElement.style, "margin-left"),
            marginRight: GetUnitAttributeValue(imageElement.style, "margin-right"),
            marginBottom: GetUnitAttributeValue(imageElement.style, "margin-bottom")
        };

        imageInfoObject.isCustomSize = ASPx.HtmlEditorClasses.Commands.ChangeImage.IsExistImageAttribute(imageElement, "width") ||
                                       ASPx.HtmlEditorClasses.Commands.ChangeImage.IsExistImageAttribute(imageElement, "height");

        var parentNode = imageElement.parentNode;
        if (parentNode.childNodes.length == 1 && parentNode.tagName != "BODY" && !ASPx.Attr.IsExistsAttribute(imageElement, "align")) {
            imageInfoObject.align = parentNode.style.textAlign;
            if (!imageInfoObject.align)
                imageInfoObject.align = parentNode.align;
        }

        if (!imageInfoObject.align) {
            if (ASPx.Attr.IsExistsAttribute(imageElement, "align"))
                imageInfoObject.align = ASPx.Attr.GetAttribute(imageElement, "align");
            else {
                if (ASPx.Browser.IE && ASPx.IsExists(imageElement.style.styleFloat)) {
                    imageInfoObject.align = imageElement.style.styleFloat;
                    imageInfoObject.useFloat = true;
                }
                if (!ASPx.Browser.IE && ASPx.IsExists(imageElement.style.cssFloat)) {
                    imageInfoObject.align = imageElement.style.cssFloat;
                    imageInfoObject.useFloat = true;
                }
            }
        }

        return imageInfoObject;
    };
    ASPx.HtmlEditorClasses.Commands.ChangeImage.IsExistImageAttribute = function(image, attrName) {
        var styleAttr = ASPx.Attr.GetAttribute(image.style, attrName);
        return ((styleAttr != "") && (styleAttr != null)) ||
               (!ASPx.Browser.NetscapeFamily && (image.outerHTML.toLowerCase().indexOf(attrName + "=") > -1));
    };
    ASPx.HtmlEditorClasses.Commands.ChangeImage.SetImageHorizontalAlign = function(imageElement, align, useFloat) {
        var documentObj = ASPx.GetElementDocument(imageElement); // B34006 - fix
        var sourceId = imageElement.id;
        imageElement.id = heMarkID;
        ASPx.Attr.RemoveAttribute(imageElement, "align");
        ASPx.SetElementFloat(imageElement, "");

        var alignValue = (align) ? align.toLowerCase() : null;
        if(!alignValue || alignValue == "left" || alignValue == "right") {
            if(!/^(body|li|td|a)$/gi.test(imageElement.parentNode.nodeName) && !ASPx.Str.Trim(ASPx.GetInnerText(imageElement.parentNode))) {
                var parent = imageElement.parentNode;
                parent.parentNode.insertBefore(imageElement, parent);
                parent.parentNode.removeChild(parent);
            }
            if(alignValue) {
                if(useFloat) {
                    if(ASPx.Browser.IE)
                        imageElement.style.styleFloat = align;
                    else
                        imageElement.style.cssFloat = align;
                }
                else
                    ASPx.Attr.SetAttribute(imageElement, "align", align);
            }
        }
        else { // align center
            var wrapElem, array;
            var compare = function(el) {
                var expr = /^(H[1-6]|DIV|P|CENTER|ADDRES|PRE|BODY)$/;
                return expr.test(el.nodeName);
            };
            var parentNode = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GetParent(imageElement.parentNode, compare);
            if(parentNode.nodeName != "BODY") {
                if(!ASPx.Str.Trim(ASPx.GetInnerText(parentNode)))
                    wrapElem = parentNode;
                else {
                    var tempElement = ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID(documentObj);
                    imageElement.parentNode.insertBefore(tempElement, imageElement);
                    var array = ASPx.HtmlEditorClasses.Utils.splitElementByChildNodeID(parentNode, tempElement.id);
                    wrapElem = array[1].cloneNode(false);
                    array[1].parentNode.insertBefore(wrapElem, array[1]);
                    imageElement = ASPx.GetElementByIdInDocument(documentObj, heMarkID);
                    wrapElem.appendChild(imageElement);
                    if(!ASPx.Str.Trim(ASPx.GetInnerText(array[0])))
                        array[0].parentNode.removeChild(array[0]);
                    if(!ASPx.Str.Trim(ASPx.GetInnerText(array[1])))
                        array[1].parentNode.removeChild(array[1]);
                }
            }
            if(wrapElem == null)
                wrapElem = ASPx.WrapElementInNewElement(imageElement, "DIV");
            wrapElem.style.textAlign = "center";
            imageElement = ASPx.GetElementByIdInDocument(documentObj, heMarkID);
        }
        sourceId != "" ? imageElement.id = sourceId : ASPx.Attr.RemoveAttribute(imageElement, "id");
    };
    ASPx.HtmlEditorClasses.Commands.ChangeImage.SetImageProperties = function (imageElement, params) {
        if (params.src)
            ASPx.Attr.SetAttribute(imageElement, "src", params.src);
        ASPx.Attr.SetAttribute(imageElement, "alt", params.alt || "");
        if (params.styleSettings.width)
            ASPx.Attr.RemoveAttribute(imageElement, "width");
        if (params.styleSettings.height)
            ASPx.Attr.RemoveAttribute(imageElement, "height");
        ASPx.HtmlEditorClasses.Utils.AppendStyleSettings(imageElement, params.styleSettings);
        ASPx.HtmlEditorClasses.Commands.ChangeImage.SetImageHorizontalAlign(imageElement, params.align, params.useFloat);
        return imageElement;
    };
})();