(function() {
    var HtmlEditorClasses = {
        Commands: {},
        Managers: {},
        Controls: {},
        Utils: {},
        Wrappers: {},
        Dialogs: {}
    };
    HtmlEditorClasses.MediaCssClasses = {
        Audio: "dxheAudio",
        Video: "dxheVideo",
        Flash: "dxheFlash",
        YouTube: "dxheYoutube",
        NotSupported: "dxheNotSupported"
    };
    HtmlEditorClasses.MediaPreloadMode = {
        None: "none",
        Auto: "auto",
        Metadata: "metadata"
    };
    HtmlEditorClasses.EnterMode = {
        Default: "Default",
        BR: "BR",
        P: "P"
    };
    HtmlEditorClasses.ResourcePathMode = {
        Absolute: 0, 
        Relative: 1, 
        RootRelative: 2, 
        NotSet: 3    
    };
    HtmlEditorClasses.GetPreparedUrl = function(htmlEditor, url) {
        if(url) {
            switch(htmlEditor.resourcePathMode) {
                case ASPx.HtmlEditorClasses.ResourcePathMode.Absolute:
                    return ASPx.Url.GetAbsoluteUrl(url);
                case ASPx.HtmlEditorClasses.ResourcePathMode.Relative:
                    return ASPx.Url.getPathRelativeUrl(url);
                case ASPx.HtmlEditorClasses.ResourcePathMode.RootRelative:
                case ASPx.HtmlEditorClasses.ResourcePathMode.NotSet:
                    return ASPx.Url.getRootRelativeUrl(url);
            }
        }
        return url;
    };

    HtmlEditorClasses.PasteMode = {
        SourceFormatting: "SourceFormatting",
        PlainText: "PlainText",
        MergeFormatting: "MergeFormatting"
    };
    HtmlEditorClasses.FilterMode = {
        BlackList: "BlackList",
        WhiteList: "WhiteList"
    };
    HtmlEditorClasses.HtmlEditingMode = {
        Simple: "Simple",
        Advanced:  "Advanced"
    };
    HtmlEditorClasses.DocumentType = { 
        XHTML: "XHTML", 
        HTML5: "HTML5",
        Both: "Both"
    };
    HtmlEditorClasses.IconType = { 
        XmlItem: "XmlItem",
        Field: "Field", 
        Enum: "Enum",
        Event: "Event"
    };
    HtmlEditorClasses.TokenType = {
        OpenTagBracket: "openTagBracket",
        CloseTagBracket: "closeTagBracket",
        TagName: "tagName",
        AttributeState: "attrState",
        AttributeName: "attrName",
        AttributeEqual: "attrName",
        AttributeValue: "attrValue",
        Script: "script",
        Css: "css",
        Text: "text"
    }

    HtmlEditorClasses.SwitchToDesignViewCallbackPrefix = "ProcessHtml_Design";
    HtmlEditorClasses.SwitchToHtmlViewCallbackPrefix = "ProcessHtml_Html";
    HtmlEditorClasses.SwitchToPreviewCallbackPrefix = "ProcessHtml_Preview";

    HtmlEditorClasses.SelectedContainerCssClasseName = "dxheSelected";
    HtmlEditorClasses.SelectedFormElementAttrName = "data-aspx-selectedFormElement";
    HtmlEditorClasses.PlaceholderCssClasseName = "dxhePlaceholder";
    HtmlEditorClasses.PlaceholderStartMarkCssClasseName = "dxhePlaceholderStartMark";
    HtmlEditorClasses.PlaceholderEndMarkCssClasseName = "dxhePlaceholderEndMark";
    HtmlEditorClasses.PlaceholderContentClasseName = "dxhePlaceholderContent";
    HtmlEditorClasses.SelectedHiddenContainerID = "dx_temp_selectedHiddenContainer";
    HtmlEditorClasses.StartSelectionPosMarkID = "dxheStartSelectionPos";
    HtmlEditorClasses.EndSelectionPosMarkID = "dxheEndSelectionPos";
    HtmlEditorClasses.StartSelectionPosMarkName = "[" + HtmlEditorClasses.StartSelectionPosMarkID + "]";
    HtmlEditorClasses.EndSelectionPosMarkName = "[" + HtmlEditorClasses.EndSelectionPosMarkID + "]";

    HtmlEditorClasses.FileManagerCallbackPrefix = "FileManager";
    HtmlEditorClasses.SpellCheckingCallbackPrefix = "SpellCheck";
    HtmlEditorClasses.SpellCheckingLoadControlCallbackPrefix = "SpellCheckLoadControl";
    HtmlEditorClasses.SpellCheckerOptionsCallbackPrefix = "SpellCheckerOptions";
    
    HtmlEditorClasses.DefaultFontSizes = ["8pt", "10pt", "12pt", "14pt", "18pt", "24pt", "36pt"];
    HtmlEditorClasses.ContentEditableAttributeNameRegExp = "contenteditable";
    HtmlEditorClasses.JSEventAttributeNameRegExp = "on[a-zA-Z][a-z]+";
    HtmlEditorClasses.TagsWithAttributeRegExpPattern = "(<[a-zA-z][a-z\\d]*(?![^>]*class\\s*=\\s*['\"]" + HtmlEditorClasses.PlaceholderCssClasseName + "['\"][^>]*))(?:\\s+[a-zA-Z][a-z]*\\s*=\\s*(?:(?:\"[^\"]*\")|(?:'[^']*')))*";
    
    HtmlEditorClasses.BrTagsRegExpPattern = "<br\\/\\>|<br \\/\\>|<br>";
    HtmlEditorClasses.EmptyHtmlRegExpPattern = "^(" + HtmlEditorClasses.BrTagsRegExpPattern + "|&nbsp;)?$";
    HtmlEditorClasses.YouTubeSrcWithoutIDRegExpPattern = "((youtube(-nocookie)?\\.com\\/((watch\\?v\\=|embed\\/|v\\/)))|(youtu\\.be\\/))";
    HtmlEditorClasses.YouTubeSrcRegExpPattern = HtmlEditorClasses.YouTubeSrcWithoutIDRegExpPattern + "([^\?^\&.]+)";

    HtmlEditorClasses.SpecialImageAttributeName = "data-aspx-elementcode";
    HtmlEditorClasses.DefaultHeightAttributeName = "data-aspx-defaultheight";
    HtmlEditorClasses.DefaultWidthAttributeName = "data-aspx-defaultwidth";
    HtmlEditorClasses.DefaultHeightAttributeValue = 150;
    HtmlEditorClasses.DefaultWidthAttributeValue = 300;
    HtmlEditorClasses.DefaultAudioHeightAttributeValue = 60;

    HtmlEditorClasses.PasteCatcherID = "dx_temp_pasteCatcher";
    HtmlEditorClasses.preservedAttributeNamePrefix = "aspx-preserved-";

    HtmlEditorClasses.SavedSrcAttributeName = "data-aspx-saved-src";

    HtmlEditorClasses.ListIdAttributeName = "data-aspx-list-id";
    HtmlEditorClasses.HeadListAttributeName = "data-aspx-head-list";

    HtmlEditorClasses.DefaultSaveSelectionTimeoutValue = 300;

    HtmlEditorClasses.View = {
        Design: "D",
        Html: "H",
        Preview: "P"
    };
    var tryEncode = function(str, encoder, decoder) {
        try {
            str = decoder(str); // '%' single char throws 'URI malformed'
        } catch(e) {
        } finally {
            str = encoder(str);
        }
        return str;
    };
    HtmlEditorClasses.encodeURIPath = function(url) {
        if(!url)
            return url;
        var pathParts = url.split('/');
        var lastPart = pathParts[pathParts.length - 1];
        var tmp = lastPart.split('?');
        var queryParts = [tryEncode(tmp[0], encodeURIComponent, decodeURIComponent)];
        if(tmp.length > 1)
            queryParts.push(tryEncode(tmp.splice(1).join('?'), encodeURI, decodeURI));
        lastPart = queryParts.join('?');
        pathParts[pathParts.length - 1] = lastPart;
        return pathParts.join('/');
    };

    HtmlEditorClasses.getReplacingElementsOptionsArray = function() {
        var array = [];
        array.push({ tagName: "object", regExp: "", imageHeight: HtmlEditorClasses.DefaultHeightAttributeValue, imageWidth: HtmlEditorClasses.DefaultWidthAttributeValue });
        array.push({ tagName: "embed", regExp: HtmlEditorClasses.Utils.getElementRegExp("embed", ""), imageHeight: HtmlEditorClasses.DefaultHeightAttributeValue, imageWidth: HtmlEditorClasses.DefaultWidthAttributeValue });
        array.push({ tagName: "video", regExp: HtmlEditorClasses.Utils.getElementRegExp("video", ""), imageHeight: HtmlEditorClasses.DefaultHeightAttributeValue, imageWidth: HtmlEditorClasses.DefaultWidthAttributeValue });
        array.push({ tagName: "audio", regExp: HtmlEditorClasses.Utils.getElementRegExp("audio", ""), imageHeight: HtmlEditorClasses.DefaultAudioHeightAttributeValue, imageWidth: HtmlEditorClasses.DefaultWidthAttributeValue });
        array.push({ tagName: "iframe", regExp: HtmlEditorClasses.Utils.getElementRegExp("iframe", "[^>]*src\\s*=\\s*['\"][^>]*" + HtmlEditorClasses.YouTubeSrcWithoutIDRegExpPattern + "[^>]*['\"]"), imageHeight: HtmlEditorClasses.DefaultHeightAttributeValue, imageWidth: HtmlEditorClasses.DefaultWidthAttributeValue });
        return array;
    };
    HtmlEditorClasses.getReplacingElementsOptionsByTagName = function(tagName) {
        var optionsArray = HtmlEditorClasses.getReplacingElementsOptionsArray();
        for(var i = 0, options; options = optionsArray[i]; i++) {
            if(options.tagName == tagName)
                return options;
        }
        return { imageHeight: HtmlEditorClasses.DefaultHeightAttributeValue, imageWidth: HtmlEditorClasses.DefaultWidthAttributeValue };
    };

    HtmlEditorClasses.IsDocumentDragOver = false;

    HtmlEditorClasses.ItemPickerImageMode = {
        ShowDropDown:               0,
        ExecuteAction:              1,
        ExecuteSelectedItemAction:  2
    };

    HtmlEditorUnforcedFunctions = {};

    HtmlEditorClasses.Utils.Throttle = function (func, delay) {
        var isCallAllowed = true,
          savedArgs,
          context;
        return function wrapper() {
            if (!isCallAllowed) {
                savedArgs = arguments;
                context = this;
                return;
            }

            func.apply(this, arguments);
            isCallAllowed = false;

            setTimeout(function () {
                isCallAllowed = true;
                if (savedArgs) {
                    wrapper.apply(context, savedArgs);
                    savedArgs = context = null;
                }
            }, delay);
        };
    };
    UnforcedFunctionCall = function(func, key, timeout, resetTimer) {
        if(resetTimer && HasUnforcedFunction(key))
            ClearUnforcedFunctionByKey(key);
        if(HtmlEditorUnforcedFunctions[key] === undefined) {
            HtmlEditorUnforcedFunctions[key] = setTimeout(function() {
                func();
                HtmlEditorUnforcedFunctions[key] = undefined;
            }, timeout);
        }
    };
    HasUnforcedFunction = function(key) {
        return !!HtmlEditorUnforcedFunctions[key];
    };
    ClearUnforcedFunctionByKey = function(key) {
        clearTimeout(HtmlEditorUnforcedFunctions[key]);
        HtmlEditorUnforcedFunctions[key] = undefined;
    };
    var isMediaType = function (img, type) {
        return img.className.indexOf(ASPx.HtmlEditorClasses.MediaCssClasses[type]) > -1;
    };
    HtmlEditorClasses.Utils.isMediaType = isMediaType;
    var getTagName = function (source) {
        if (isMediaType(source, 'YouTube'))
            return 'iframe';
        else if (isMediaType(source, 'Audio'))
            return 'audio';
        else if (isMediaType(source, 'Video'))
            return 'video';
        else if (isMediaType(source, 'Flash') || isMediaType(source, 'NotSupported'))
            return 'object';
        return source.tagName ? source.tagName.toLowerCase() : "TEXT NODE";
    };
    HtmlEditorClasses.Utils.getTagName = getTagName;
    HtmlEditorClasses.Utils.isSimpleImage = function (img) {
        return getTagName(img) == 'img';
    };
    HtmlEditorClasses.Utils.UnforcedFunctionCall = UnforcedFunctionCall;
    HtmlEditorClasses.Utils.HasUnforcedFunction = HasUnforcedFunction;
    HtmlEditorClasses.Utils.ClearUnforcedFunctionByKey = ClearUnforcedFunctionByKey;
    HtmlEditorClasses.Utils.getElementRegExp = function(tagName, ext) {
        return new RegExp("<" + tagName + ext + "[^>]*((>[\\s\\S]*?</" + tagName + ">)|(>))", "gi");
    };

    var hasValuableSiblings = function(element) {
        var hasNonEmptyElements = false;
        var parentNode = element.parentNode;
        for(var i = 0; i < parentNode.childNodes.length; i++) {
            var node = parentNode.childNodes[i];
            hasNonEmptyElements = !(node == element || node.nodeType == 3 || !node.nodeValue);
            if(hasNonEmptyElements)
                break;
        }
        return hasNonEmptyElements;
    };
    var setElementProperty = function(element, propName, propValue) {
        if(propValue != null && propValue != "")
            ASPx.Attr.SetAttribute(element, propName, propValue);
        else
            ASPx.Attr.RemoveAttribute(element, propName);
    };
    HtmlEditorClasses.Utils.AppendStyleSettings = function (element, settings, attributeMapping) {
        if(!element || !settings)
            return;
        var attributeMapping = attributeMapping || [
            {from: "width", to: "width"}, 
            {from: "height", to: "height"}, 
            {from: "borderWidth", to: "border-width"}, 
            {from: "marginTop", to: "margin-top"}, 
            {from: "marginBottom", to: "margin-bottom"},
            {from: "marginLeft", to: "margin-left"},
            {from: "marginRight", to: "margin-right"},
            {from: "borderColor", to: "border-color"},
            {from: "borderStyle", to: "border-style"}
        ];
        if (settings.hasOwnProperty("className") || settings.hasOwnProperty("cssClass"))
            setElementProperty(element, "class", settings.className || settings.cssClass);
        for (var i = 0; i < attributeMapping.length; i++) {
            var route = attributeMapping[i];
            if (settings.hasOwnProperty(route.from))
                setElementProperty(element.style, route.to, settings[route.from]);
        }
    };

    HtmlEditorClasses.Utils.getElementPosition = function(element) {
        var align = "";
        if(element) {
            var parentNode = element.parentNode;
            if (!hasValuableSiblings(element) && parentNode.tagName != "BODY")
                align = parentNode.style.textAlign || parentNode.align;

            if (!align) {
                if (ASPx.Attr.IsExistsAttribute(element, "align"))
                    align = ASPx.Attr.GetAttribute(element, "align");
                else if(ASPx.IsExists(element.style.cssFloat) || ASPx.IsExists(element.style.styleFloat)) {
                    var float = ASPx.GetElementFloat(element);
                    align = float == "none" ? "" : float;
                }
            }
        }
        return align;
    };

    function processHorAlignWrapper(wrapElem) {
        if(wrapElem != null && wrapElem.style.textAlign == "center")
            wrapElem.style.textAlign = "";           
    };

    HtmlEditorClasses.Utils.setHorizontalAlign = function(element, align, tempId) {
        var documentObj = ASPx.GetElementDocument(element); // B34006 - fix
        var sourceId = element.id;
        element.id = tempId;

        var parentNode = element.parentNode;
        var wrapElem = null;
        
        if(parentNode.tagName == "DIV" && !hasValuableSiblings(element))
            wrapElem = parentNode;

        ASPx.Attr.RemoveAttribute(element, "align");
        ASPx.SetElementFloat(element, "");

        var alignValue = (align) ? align.toLowerCase() : null;
        if(!alignValue || alignValue == "left" || alignValue == "right") {
            processHorAlignWrapper(wrapElem);
            element = ASPx.GetElementByIdInDocument(documentObj, tempId);
            
            if(alignValue)
                ASPx.SetElementFloat(element, align);
            else
                ASPx.SetElementFloat(element, "");
        }
        else { // align center
            if(wrapElem == null)
                wrapElem = ASPx.WrapElementInNewElement(element, "DIV");
            wrapElem.style.textAlign = "center";
            element = ASPx.GetElementByIdInDocument(documentObj, tempId);
        }
        sourceId != "" ? element.id = sourceId : ASPx.Attr.RemoveAttribute(element, "id");
        return element;
    };

    HtmlEditorClasses.Utils.createHiddenContainer = function(doc, tagName, id, content) {
        var container = doc.createElement(tagName);
        if(content)
            container.innerHTML = content;
        container.id = id;
        if(!ASPx.Browser.Safari) {
            var styleObject = HtmlEditorClasses.Utils.PasteContainerStyleObject;
            for(var key in styleObject) { 
                if (styleObject.hasOwnProperty(key))
                    container.style[key] = styleObject[key];
            }
        }
        return container;
    };

    HtmlEditorClasses.Utils.GetSelectedElementComputedStyle = function (selectedElement, styleInfoObject) {
        var selectedElementComputedStyle = ASPx.GetCurrentStyle(selectedElement);
        for (var j in styleInfoObject) {
            styleValue = selectedElementComputedStyle[j];
            if (ASPx.IsExists(styleValue))
                switch (styleInfoObject[j]) {
                    case "int":
                        styleInfoObject[j] = (isNaN(styleValue.replace('px', '')) ? "" : parseInt(styleValue.replace('px', '')));
                        break;
                    default:
                        styleInfoObject[j] = styleValue;
                        break;
                }
        }
        return styleInfoObject;
    };

    HtmlEditorClasses.Utils.RemoveStylesDuplicates = function (styles, stylesStorageDocument) {
        if (!styles.className)
            return styles;

        var styleSheetRules = ASPx.GetStyleSheetRules(styles.className.toLowerCase(), stylesStorageDocument);

        if (!styleSheetRules)
            return styles;

        var cssClassesStyles = styleSheetRules.style;

        for (var i in styles) {
            if ((cssClassesStyles[i] == styles[i]) ||
                        (parseInt(cssClassesStyles[i]) == styles[i]))
                delete styles[i];
        }
        return styles;
    };

    HtmlEditorClasses.Utils.IsLinkSelected = function (wrapper) {
        var curSelection = ASPxClientHtmlEditorSelection.Create(wrapper.getWindow());
        return HtmlEditorClasses.Utils.IsLink(curSelection.GetParentElement());
    };

    HtmlEditorClasses.Utils.IsLink = function (element) {
        return !!ASPx.GetParentByTagName(element, "A");
    };

    HtmlEditorClasses.Utils.clearPasteContainerStyle = function(element) {
        var styleObject = HtmlEditorClasses.Utils.PasteContainerStyleObject;
        for(var key in styleObject) { 
            if (styleObject.hasOwnProperty(key))
                element.style[key] = "";
        }
    };
    
    HtmlEditorClasses.Utils.PasteContainerStyleObject = { position: "absolute", left: "-1000px", width: 0, height: 0, overflow: "hidden" };

    HtmlEditorClasses.Utils.createPasteContainer = function(wrapper) {
        var doc = wrapper.getDocument();
        var selection = wrapper.getSelection();
        var selectedElement = selection.GetSelectedElement();
        if(ASPx.HtmlEditorClasses.Utils.needToUseSpecialSelection(selectedElement))
            ASPx.HtmlEditorClasses.Utils.removePlaceholderElement(selectedElement, wrapper, true);
        var clientSelection = selection.clientSelection;
        wrapper.restoreFocus = clientSelection.GetExtendedBookmark();
        
        var start = doc.createElement("span"),
            end = doc.createElement("span");
        start.id = HtmlEditorClasses.Selection.CreateUniqueID();
        end.id = HtmlEditorClasses.Selection.CreateUniqueID();
        var hiddenContainer = HtmlEditorClasses.Utils.createHiddenContainer(doc, "div", HtmlEditorClasses.PasteCatcherID);
        
        var content = ASPx.Browser.WebKitTouchUI || ASPx.Browser.WebKitFamily ? "&nbsp;" : null;
        if(content) {
            hiddenContainer.innerHTML = content;
            hiddenContainer.insertBefore(start, hiddenContainer.firstChild);
        }
        else 
            hiddenContainer.appendChild(start);
        hiddenContainer.appendChild(end);

        var parent = HtmlEditorClasses.Utils.getParent(doc.getElementById(wrapper.restoreFocus.startMarkerID));
        parent.parentNode.insertBefore(hiddenContainer, parent);

        clientSelection.SelectExtendedBookmark({ "startMarkerID": !content ? end.id : start.id, "endMarkerID": !content ? start.id : end.id });

        if(ASPx.Browser.IE) {
            wrapper.eventManager.setPreventEventFlag();
            ASPx.HtmlEditorClasses.Utils.UnforcedFunctionCall(function() {
                ASPx.RemoveElement(doc.getElementById(ASPx.HtmlEditorClasses.PasteCatcherID));
                wrapper.getSelection().clientSelection.SelectExtendedBookmark(wrapper.restoreFocus);
                wrapper.commandManager.updateLastRestoreSelectionAndHTML();
            }.aspxBind(this), "RemovePasteContainer", 200, true);
        }
    };

    HtmlEditorClasses.Utils.isExternalUrl = function(url, appPath, host) {  
        if(url) {
            var regexpTest = [
                { exp: "^data:[a-zA-Z0-9]+\\/[a-zA-Z0-9]+;base64,.*", ret: false},
                { exp: "^(https?:|ftps?:|^)\\/\\/(?!"+ (host + appPath).split("/").join("\\/") +").*", ret: true }
            ];
            for(var i = 0; i < regexpTest.length; i++) {
                if((new RegExp(regexpTest[i].exp)).test(url))
                    return regexpTest[i].ret;
            }
        }
        return false;
    };

    HtmlEditorClasses.Utils.getChildsByPredicate = function(element, predicate, withTextNodes) {  
        var result = [];
        if((withTextNodes || element.nodeType == 1) && predicate(element))
            result.push(element);
        for(var i = 0; i < element.childNodes.length; i ++)
            result = result.concat(HtmlEditorClasses.Utils.getChildsByPredicate(element.childNodes[i], predicate, withTextNodes));
        return result;
    };

    HtmlEditorClasses.Utils.elementAttributesContains = function(elem) {
        var attrs = elem.attributes;
        for(var i = 0, attr; attr = attrs[i]; i++) {
	        if(attr.specified) {
	            var attrName = attr.nodeName;
	            var attrValue = elem.getAttribute(attrName, 2);
	            if(attrValue || attr.nodeValue)
	                return true;
		    }
	    }
        return !!elem.style.cssText;
    };
    HtmlEditorClasses.Utils.insetContentInHiddenContainer = function(wrapper, content, select) {
        var doc = wrapper.getDocument();
        var array = content.match(/<(div|p|table|ol|ul|dl|h[1-6]|address|blockquote|center|pre)(?![^>]*(display\s*:\s*inline|data-aspx-elementcode))[^>]*/gi) || content.match(/<img[^>]*data-aspx-elementcode[^>]*/gi);
        var hiddenChild = HtmlEditorClasses.Utils.createHiddenContainer(doc, array && array.length > 0 ? "div" : "span", "hiddenChild");
        wrapper.commandManager.executeCommand(ASPxClientCommandConsts.PASTEHTML_COMMAND, hiddenChild.outerHTML);
        hiddenChild = doc.getElementById(hiddenChild.id);
        var parent = HtmlEditorClasses.Utils.getParent(hiddenChild);
        
        var hiddenContainer = HtmlEditorClasses.Utils.createHiddenContainer(doc, "div", "hiddenContainer");
        hiddenContainer.appendChild(parent.cloneNode(true));
        parent.parentNode.insertBefore(hiddenContainer, parent);
        
        var start = doc.createElement("span"),
            end = doc.createElement("span");
        start.id = HtmlEditorClasses.Selection.CreateUniqueID();
        end.id = HtmlEditorClasses.Selection.CreateUniqueID();
        hiddenChild.parentNode.insertBefore(end, hiddenChild);
        hiddenChild.parentNode.insertBefore(start, hiddenChild);
        hiddenChild.parentNode.removeChild(hiddenChild);
        hiddenChild = doc.getElementById(hiddenChild.id);
        if(hiddenChild) {
            if(select) {
                var bm = { "startMarkerID": HtmlEditorClasses.Selection.CreateUniqueID(), "endMarkerID": HtmlEditorClasses.Selection.CreateUniqueID() };
                hiddenChild.innerHTML = "<span id=\"" + bm.startMarkerID +"\"></span>" + content + "<span id=\"" + bm.endMarkerID +"\"></span>";
                wrapper.getSelection().clientSelection.SelectExtendedBookmark(bm);
            }
            else
                hiddenChild.innerHTML = content;
        }
        return { bookmark: { "startMarkerID": start.id, "endMarkerID": end.id }, hiddenParent: hiddenContainer, hiddenChild: hiddenChild  };
    };

    HtmlEditorClasses.Utils.removePlaceholderElement = function(element, wrapper, setFocus) {
        var isSelected = element.className.indexOf(HtmlEditorClasses.SelectedContainerCssClasseName);
        var doc = wrapper.getDocument();
        var hiddenElement = doc.getElementById(HtmlEditorClasses.SelectedHiddenContainerID);
        if(hiddenElement)
            hiddenElement.parentNode.removeChild(hiddenElement);
        if(isSelected || setFocus) {
            var start = HtmlEditorClasses.Selection.CreateElementWithUniqueID(doc);
            var end = HtmlEditorClasses.Selection.CreateElementWithUniqueID(doc);
            element.parentNode.insertBefore(end, element);
            element.parentNode.insertBefore(start, element);
            element.parentNode.removeChild(element);
            wrapper.getSelection().clientSelection.SelectExtendedBookmark({ "startMarkerID": start.id, "endMarkerID": end.id});
        }
        else
            element.parentNode.removeChild(element);
    };

    HtmlEditorClasses.getSpecialSelectionInfo = function() {
        var result = [];
        result.push({ tagName: "SPAN", className: HtmlEditorClasses.PlaceholderCssClasseName });
        result.push({ tagName: "SPAN", className: HtmlEditorClasses.PlaceholderStartMarkCssClasseName });
        result.push({ tagName: "SPAN", className: HtmlEditorClasses.PlaceholderEndMarkCssClasseName });
        result.push({ tagName: "SPAN", className: HtmlEditorClasses.PlaceholderContentClasseName });
        result.push({ tagName: "INPUT", className: null });
        result.push({ tagName: "TEXTAREA", className: null });
        result.push({ tagName: "SELECT", className: null });
        result.push({ tagName: "OPTION", className: null });
        result.push({ tagName: "BUTTON", className: null });
        return result;
    };
    HtmlEditorClasses.Utils.needToUseSpecialSelection = function(element) {
        if(element) {
            var specialSelectionInfo = HtmlEditorClasses.getSpecialSelectionInfo();
            for(var i = 0, item; item = specialSelectionInfo[i]; i++) {
                if(item.tagName == element.nodeName && (item.className == null || ASPx.ElementHasCssClass(element, item.className)))
                    return true;
            }
        }
        return false;
    };

    HtmlEditorClasses.Utils.getParent = function(element) {
        var parent = element.parentNode;
        var prevElement = element;
        while(!/^body$/i.test(parent.nodeName)) {
            prevElement = parent;
            parent = parent.parentNode;
        }
        return prevElement;
    }

    function nextElementSibling(el) {
        do { el = el.nextSibling } while ( el && el.nodeType !== 1 );
        return el;
    };
    function previousElementSibling(el) {
        do { el = el.previousSibling } while ( el && el.nodeType !== 1 );
        return el;
    };

    HtmlEditorClasses.Utils.getNextElementSibling = function(element) {
        return element && (element.nextElementSibling || nextElementSibling(element));
    };
    HtmlEditorClasses.Utils.getPreviousElementSibling = function(element) {
        return element && (element.previousElementSibling || previousElementSibling(element));
    };

    HtmlEditorClasses.Utils.splitElementByChildNodeID = function(element, nodeID) {
        var getFirstChild = function(element) { return element.firstChild; };
        var getLastChild = function(element) { return element.lastChild; };
        var getPrevElement = function(element) { return element.previousSibling; };
        var getNextElement = function(element) { return element.nextSibling; };
        var removeElements = function(parent, id, getChild, getElement) {
            var child = getChild(parent);
            if(child.id && child.id == id) {
                child.parentNode.removeChild(child);
                return;
            }
            var prevElement;
            for(var i = 0, element = child; element; element = getElement(element)) {
                if(prevElement)
                    prevElement.parentNode.removeChild(prevElement);
                if(element.id && element.id == id) {
                    element.parentNode.removeChild(element);
                    return;
                }
                else {
                    var list = [];    
                    ASPx.GetNodesByPartialId(element, id, list)
                    if(list.length > 0)
                        return removeElements(element, id, getChild, getElement);
                }
                prevElement = element;
            }
        }
        element.parentNode.insertBefore(element.cloneNode(true), element);
        removeElements(element, nodeID, getFirstChild, getNextElement);
        previousCloneElement = element.previousSibling;
        removeElements(previousCloneElement, nodeID, getLastChild, getPrevElement);
        return [previousCloneElement, element];
    }

    HtmlEditorClasses.Utils.findInlineElementBeforeBlockParent = function(element) {
        if(ASPx.HtmlEditorClasses.Commands.Utils.IsBlockElement(element.parentNode) || element.parentNode.nodeName == "BODY")
            return element;
        else
            return ASPx.HtmlEditorClasses.Utils.findInlineElementBeforeBlockParent(element.parentNode);
    }
    HtmlEditorClasses.Utils.splitParentElement = function(parent, startNode, endNode) {
        var startMarkElement = ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID(parent.ownerDocument);
        var endMarkElement = ASPx.HtmlEditorClasses.Selection.CreateElementWithUniqueID(parent.ownerDocument);
        startNode.parentNode.insertBefore(startMarkElement, startNode);
        ASPx.InsertElementAfter(endMarkElement, endNode);
        var array = ASPx.HtmlEditorClasses.Utils.splitElementByChildNodeID(parent, startMarkElement.id);
        if(!ASPx.GetInnerText(array[0]))
            array[0].parentNode.removeChild(array[0]);
        array = ASPx.HtmlEditorClasses.Utils.splitElementByChildNodeID(array[1], endMarkElement.id);
        if(!ASPx.GetInnerText(array[1]))
            array[1].parentNode.removeChild(array[1]);
        return array[0];
    }
    HtmlEditorClasses.Utils.getBeforeBlockParentElements = function(elements) {
        var childs = [];
        var objectArray = [];
        var oldParentElement;
        for(var i = 0, element; element = elements[i]; i++) {
            if(!ASPx.HtmlEditorClasses.Commands.Utils.IsBlockElement(element)) {
                var parent = ASPx.HtmlEditorClasses.Utils.findInlineElementBeforeBlockParent(element);
                if(parent.nodeType != 3) {
                    if(oldParentElement && oldParentElement != parent) {
                        objectArray.push({ parent: oldParentElement, childs: childs });
                        oldParentElement = null;
                        childs = [];
                    }
                    if(!oldParentElement)
                        oldParentElement = parent;
                    childs.push(element);
                }
            }
            else {
                if(childs.length > 0)
                    objectArray.push({ parent: oldParentElement, childs: childs });
                objectArray.push({ parent: element, childs: [element] });
                oldParentElement = null;
                childs = [];
            }
        }
        if(childs.length > 0)
            objectArray.push({ parent: oldParentElement, childs: childs });
        var result = [];
        for(var i = 0, object; object = objectArray[i]; i++) {
            if(object.childs.length == 1 && object.childs[0].nodeType != 3)
                result.push(object.parent);
            else {
                var firstChild = object.childs[0];
                var lastChild = object.childs[object.childs.length - 1];
                result.push(ASPx.HtmlEditorClasses.Utils.splitParentElement(object.parent, firstChild, lastChild));
            }
        }
        return result;
    }
    HtmlEditorClasses.Utils.getTableRows = function (table) {
        var result = [];
        var children = ASPx.GetChildElementNodes(table); 
        for(var i = 0; i < children.length; i++) {
            var child = children[i];
            if(child.tagName == "TR")
                result.push(child);
            else
                result = ASPx.Data.CollectionsUnionToArray(result, ASPx.GetChildNodesByTagName(child, "TR"));
        }
        return result;
    };
    HtmlEditorClasses.Utils.getBodyByElement = function(element) { 
        for(var parent = element.parentNode; parent; parent = parent.parentNode) {
            if(parent.nodeName == "BODY")
                return parent;
            else if(!parent.parentNode)
                return null;
        }
        return null;
    }

    HtmlEditorClasses.getElementInfo = function(element) {
        var styleAttrList;
        var attrList = [];
        for(var i = 0, attr; attr = element.attributes[i]; i++) {
            if(attr.name == "style")
                styleAttrList = HtmlEditorClasses.getStyleAttrList(attr.value);
            else
                attrList.push({ name: attr.name, value: attr.value });
        }
        return {
            tagName: element.nodeName,
            attrList: attrList,
            styleAttrList: styleAttrList
        }
    };
    HtmlEditorClasses.getStyleAttrList = function(styleAttrValue) {
        var result = [];
        var styleAttrList = styleAttrValue.split(/;\s*/);
        for(var i = 0, styleAttr; styleAttr = styleAttrList[i]; i++) {
            var array = styleAttr.split(/:\s*/);
            result.push({ name: array[0], value: array[1] });
        }
        return result;
    };
    HtmlEditorClasses.getConditionalTagFilterSettings = function() {
        var result = [];
        result.push({ tagName: "SPAN", isNotFiltered: function(attrObject) { return attrObject.name == "id" && /^dx_temp_/i.test(attrObject.value) || attrObject.name == "class" && /^dxhe/i.test(attrObject.value); } });
        result.push({ tagName: "IMG", isNotFiltered: function(attrObject) { return attrObject.name == "class" && /^dxhe/i.test(attrObject.value); } });
        return result;
    };
    HtmlEditorClasses.setDocumentInnerHTML = function (workDocument, html) {
        if (ASPx.Browser.IE && ASPx.Browser.Version < 9) {
            HtmlEditorClasses.documentWrite(workDocument, html);
        }
        else
            if (ASPx.Browser.IE && ASPx.Browser.Version == 9) {              
                workDocument = new ActiveXObject("htmlfile");
                ASPx.HtmlEditorClasses.documentWrite(workDocument, html);
            }
            else {
                workDocument.documentElement.innerHTML = html;
            }
    };
    HtmlEditorClasses.documentWrite = function (workDocument, html) {
        workDocument.open();
        workDocument.write(html);
        workDocument.close();
    };
    HtmlEditorClasses.createHtmlDocument = function () {
        var workDocument;
        if (document.implementation && document.implementation.createHTMLDocument) {
            workDocument = document.implementation.createHTMLDocument("title");
        }
        else {
            workDocument = new ActiveXObject("htmlfile");
            ASPx.HtmlEditorClasses.documentWrite(workDocument, "<html><head><title>title</title></head><body></body></html>");
        }
        return workDocument;
    };
    HtmlEditorClasses.getMetaHttpEquivTagsRegExp = function (htmlText) {
        var metaTags = htmlText.match(/<meta(.*?)>/gi);
        var metaHttpEquivTags = [];
        if (metaTags)
            for (var i = 0; i < metaTags.length; i++) {
                var hasContentAttr = metaTags[i].match(/content\s*=\s*(["'])(.*?)\1/gi);
                var hasHttpEquivAttr = metaTags[i].match(/http-equiv\s*=\s*(["'])(.*?)\1/gi);
                var isHttpEquivMetaTag = hasContentAttr && hasHttpEquivAttr;
                if (isHttpEquivMetaTag) {
                    metaHttpEquivTags.push(metaTags[i]);
                }
            }
        return metaHttpEquivTags;
    },
    HtmlEditorClasses.replaceMetaHttpEquivIE = function (html, actualHttpEqiuvMetaTag) {
        var actualHttpEqiuvMetaTag = actualHttpEqiuvMetaTag || "";
        var metaTags = html.match(/<meta(.*?)>/gi);
        var metaHttpEquivTags = HtmlEditorClasses.getMetaHttpEquivTagsRegExp(html);

        if (metaHttpEquivTags.length == 1) {
            html = html.replace(metaHttpEquivTags[0], actualHttpEqiuvMetaTag);
        }
        else
            if (metaHttpEquivTags.length > 1) {
                var lastMetaTagIndex = metaHttpEquivTags.length - 1;
                for (var i = 0; i < lastMetaTagIndex; i++) {
                    html = html.replace(metaHttpEquivTags[i], "");
                }
                html = html.replace(metaHttpEquivTags[lastMetaTagIndex], actualHttpEqiuvMetaTag);
            }
        return html;
    };
    
    var DisplayPositon = {
        Top: "top",
        Bottom: "bottom",
        Center: "center"
    };

    var scrollHelper = {
        getScrollInfo: function (element, padding) {
            var container = element.ownerDocument.body;

            return {
                padding: padding,
                container: container,
                direction: ASPx.GetCurrentStyle(container).direction,
                elementPosition: element.getBoundingClientRect(),
                topBorder: container.clientTop,
                bottomBorder: container.clientHeight,
                docClientHeight: container.clientHeight,
                docClientWidth: container.clientWidth,
                leftBorder: container.clientLeft,
                rightBorder: container.clientWidth,
                curScrollTop: container.scrollTop,
                curScrollLeft: container.scrollLeft
            }
        },
        getScrollWindow: function (container) {
            return container.ownerDocument.defaultView || container.ownerDocument.parentWindow;
        },
        isVerticalScrollRequired: function (info) {
            var topCondition = (info.topBorder + info.padding) < info.elementPosition.top;
            var bottomCondition = info.elementPosition.bottom < (info.bottomBorder - info.padding);
            return !topCondition || !bottomCondition;
        },
        isHorisontalScrollRequired: function (info) {
            var leftCondition = (info.leftBorder + info.padding) < info.elementPosition.left;
            var rightCondition = info.elementPosition.right < (info.rightBorder - info.padding);
            return !leftCondition || !rightCondition;
        },
        scrollToInternal: function (element, padding, displayPosition) {
            var info = this.getScrollInfo(element, padding);
            if (this.isVerticalScrollRequired(info) || this.isHorisontalScrollRequired(info)) {
                var coords = this.getPositionedCoords(this.getNewScrollLeft(info), this.getNewScrollTop(info), displayPosition);
                this.getScrollWindow(info.container).scrollTo(coords.x, coords.y);
            }
        },
        getNewScrollLeft: function (info) {
            var newScrollLeft = info.curScrollLeft;
            var elemDisatance = info.elementPosition.left - info.padding;
            if (this.isHorisontalScrollRequired(info)) {
                if (ASPx.Browser.IE && info.direction === "rtl") {
                    var invertedDistance = 0 - elemDisatance;
                    newScrollLeft += invertedDistance;
                }
                else {
                    newScrollLeft += elemDisatance;
                }
            }
            return newScrollLeft;
        },
        getNewScrollTop: function (info) {
            var newScrollTop = info.curScrollTop;
            if (this.isVerticalScrollRequired(info)) {
                newScrollTop += info.elementPosition.top - info.padding;
            }
            return newScrollTop;
        },
        getPositionedCoords: function (newScrollLeft, newScrollTop, displayPositon) {
            var displayPositon = displayPositon || DisplayPositon.Top;
            switch (displayPositon) {
                case (displayPositon.Center):
                    newScrollTop -= info.docClientHeight / 2;
                    newScrollLeft -= info.docClientWidth / 2;
                    break;
                case (displayPositon.Bottom):
                    newScrollTop -= info.docClientHeight + info.elementPosition.Height;
                    newScrollLeft -= info.docClientWidth + info.elementPosition.Width;
                    break;
                default:
                    break;
            }
            return {
                x: newScrollLeft,
                y: newScrollTop
            };
        },
        scrollTo: function (element, padding, displayPosition) {
            this.scrollToInternal(element, padding, displayPosition);
        }
    };

    HtmlEditorClasses.scrollHelper = scrollHelper;
    HtmlEditorClasses.DisplayPositon = DisplayPositon;

    ASPx.HtmlEditorClasses = HtmlEditorClasses;
})();