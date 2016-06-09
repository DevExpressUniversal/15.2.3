(function() {
    /* RegExp */
    var jsEventHandlersRegExp = new RegExp("(" + ASPx.HtmlEditorClasses.TagsWithAttributeRegExpPattern + ")(?:\\s+" + ASPx.HtmlEditorClasses.JSEventAttributeNameRegExp + "\\s*=\\s*(?:(?:\"[^\"]*\")|(?:'[^']*')))", "gi"); 
    var jsTagsRegExpPattern = "<(script)([^>]*)>[\\s\\S]*?</(script)([^>]*)>";
    var emptyTagsRegExpPattern = "^<(?:(p|div|address|h\\d|center|strong)(?=[ >]))(?![\\s\\S]*id\\s*=\\s*[\"|'][\\s\\S]+[\"|'][^>]*>)[^>]*>(?:\\s*|&nbsp;|" + ASPx.HtmlEditorClasses.BrTagsRegExpPattern + ")(<\\/\\1>)?$";

    var protectUrlsARegExpPattern = /<a(?=\s).*?\shref=((?:"[^">]+")|(?:'[^'>]+')|(?:[^"' >]+))/gi;
    var protectUrlsImgRegExpPattern = /<img(?=\s).*?\ssrc=((?:('|")[^>]+?(?:\2))|(?:[^"' >]+))/gi;
    var protectUrlsAreaRegExpPattern = /<area(?=\s).*?\shref=((?:(?:'|")[^"'>]+(?:'|"))|(?:[^"' >]+))/gi;
    var removeSavedUrlsRegExpPattern = /\ssavedurl=((?:"[^">]+")|(?:'[^'>]+')|(?:[^"' >]+))/gi;

    var PreservedTagNamePrefix = "ASPxPreservedTag_";

    // HtmlProcessing Utils
    var styleAttributeRegEx = new RegExp("style=\"[^\"]*\"|'[^']*'", "ig");
    var rgbColorCssAttrRegExp = new RegExp("rgb\\((\\d+)\\,(\\s)?(\\d+)\\,(\\s)?(\\d+)\\)", "ig");

    var defaultSafariFontSizes = ["x-small", "small", "medium", "large", "x-large", "xx-large", "-webkit-xxx-large"];

    var defaultSafariFontSizesHashTable = null;
    function _aspxGetDefaultSafariFontSizesHashTable() {
        if(!defaultSafariFontSizesHashTable)
            defaultSafariFontSizesHashTable = ASPx.Data.CreateIndexHashTableFromArray(defaultSafariFontSizes);
        return defaultSafariFontSizesHashTable;
    }

    ASPx.HtmlEditorClasses.HtmlProcessor = ASPx.CreateClass(null, {
        convertToEmptyHtml: function(html, body) {
            var emptyTagsRegExp = new RegExp(emptyTagsRegExpPattern, "ig");
            var emptyHtmlRegExp = new RegExp(ASPx.HtmlEditorClasses.EmptyHtmlRegExpPattern, "ig");
            var brTagsRegExp = new RegExp(ASPx.HtmlEditorClasses.BrTagsRegExpPattern, "ig");
            var processedHtml = ASPx.Str.Trim(html);

            if(emptyTagsRegExp.test(processedHtml) || emptyHtmlRegExp.test(processedHtml))
                html = "";
            else if(ASPx.Browser.IE && ASPx.Browser.MajorVersion > 10 && !html.replace(brTagsRegExp, ""))
                html = this.removeBrElements(html, body);
            return html;
        },
        removeBrElements: function(html, body) {
            return html;
        },
        maskComments: function(html) {
            var comments = [];
            var match = null;
            var i = 0;

            while ((match = html.match("<!--(.*?)-->")) !== null) {
                var comment = match[0];
                comments.push(comment);

                var mask = "<dxHideComment" + i + " />";
                html = html.replace(comment, mask);
                i++;
            }
            return { html: html, comments: comments };
        },
        unMaskComments: function(html, comments) {
            for(var i = 0, l = comments.length; i < l; i++) {
                var mask = "<dxHideComment" + i + " />";
                html = html.replace(mask, comments[i]);
            }
            return html;
        },
        cleanHtmlScripts: function(html) {
            var ret = this.removeScriptTags(html);
            return this.removeScriptEventHandlers(ret);
        },
        removeScriptTags: function(html) {
            var ret = html;
	        var rx = new RegExp(jsTagsRegExpPattern, "gi");
	        if(ret != "")
	            ret = ret.replace(rx, "");
	        return ret;
        },
        removeScriptEventHandlers: function(html) {
            return ASPx.Str.CompleteReplace(html, jsEventHandlersRegExp, "$1");
        },

        preserveTagsByName: function(tagName, html) {
            var openReg = new RegExp("<" + tagName, "gi");
            var closeReg = new RegExp("<" + "/" + tagName + ">", "gi");
            return ASPx.Str.ApplyReplacement(html, [
                [openReg, "<" + "!--" + PreservedTagNamePrefix + tagName],
                [closeReg, "</" + PreservedTagNamePrefix + tagName + "--" + ">"]
            ]); 
        },
        depreserveTagsByName: function(tagName, html) {
            var openReg = new RegExp("<!--" + PreservedTagNamePrefix + tagName, "gi");
            var closeReg = new RegExp("</" + PreservedTagNamePrefix + tagName + "--" + ">", "gi");
            return ASPx.Str.ApplyReplacement(html, [
                [openReg, "<" + tagName],
                [closeReg, "<" + "/" + tagName + ">"]
            ]); 
        },
        processHtmlByBrowser: function(html) {
            var container = document.createElement("DIV");
            ASPx.SetInnerHtml(container, html);
            return container.innerHTML;
        },
        depreserveButtonTags: function(content) {
            if(!ASPx.Browser.WebKitFamily && !ASPx.Browser.Firefox) 
                return content;
            var container = content;
            if(typeof content === "string") {
                var container = document.createElement("DIV");
                container.innerHTML = content;
            }

            for(var child = container.firstChild; child; child = child.nextSibling) {
                if(child.nodeType != 1) continue;
                if(child.tagName == "INPUT" && child.name.indexOf("dx_savedButton:") === 0) {
                    var node = document.createElement("BUTTON");
                    ASPx.Attr.CopyAllAttributes(child, node);
                    try {
                        var savedData = eval("(" + child.name.substr("dx_savedButton:".length) + ")");
                        node.innerHTML = savedData.dx_savedContent;
                        node.value = savedData.dx_savedValue;
                        node.type = savedData.dx_savedType;
                        node.name = savedData.dx_savedName;
                    } catch (e) { 
                        node.innerHTML = child.value;
                    }

                    child.parentNode.replaceChild(node, child);
                    child = node;
                }
                else if(child.childNodes.length > 0)
                    this.depreserveButtonTags(child);
            }
            return container.innerHTML;
        },
        depreserveAttribute: function(html, attributeNameRegExp) {
            return ASPx.Str.CompleteReplace(html, this.getAttributeDepreserveRegExp(attributeNameRegExp), "$1$3$4");
        },
        getAttributeDepreserveRegExp: function(attributeNameRegExp) {
            return new RegExp("(" + ASPx.HtmlEditorClasses.TagsWithAttributeRegExpPattern + "\\s+)" + ASPx.HtmlEditorClasses.preservedAttributeNamePrefix + "(" + attributeNameRegExp + ")(\\s*=)*", "gi");
        },
        removeEmptyBorderClassName: function(html) {
            var reg = new RegExp("\\s*\\b" + ASPx.HtmlEditorTableHelper.EmptyBorderTableClassName + "\\b", "ig"); // B206203
            html = html.replace(reg, ""); 
            return html.replace(/class=["']\s*["']\s*/ig, ""); // remove empty class
        },
        // Font -> Span
        replaceFontWithSpanTag: function(html) {
            html = ASPx.Str.ApplyReplacement(html, [
                [/<font/ig, "<span"],
                [/<\/font>/ig, "</span>"]
            ]);
            var containerElement = document.createElement("DIV");
            this.setInnerHtml(containerElement, html);
            var spans = ASPx.GetNodesByTagName(containerElement, "SPAN");

            for(var i = 0; i < spans.length; i++) {
                var curSpan = spans[i];
                // face            
                var curSpanFace = ASPx.Attr.GetAttribute(curSpan, "face");
                if(ASPx.IsExists(curSpanFace)) { // hack
                    if(curSpanFace != "null")
                        if(!ASPx.Browser.Opera)
                            curSpan.style.fontFamily = curSpanFace;
                        else
                            this.setStylePropForOpera(curSpan, "font-family", curSpanFace);
                    ASPx.Attr.RemoveAttribute(curSpan, "face");
                }

                // size
                var size = 0;
                if(!isNaN(size = parseInt(ASPx.Attr.GetAttribute(curSpan, "size")))) {
                    try {
                        curSpan.style.fontSize = ASPx.HtmlEditorClasses.DefaultFontSizes[size - 1];
                    }
                    catch (ex) { }
                }
                else {
                    if(ASPx.Browser.WebKitFamily) {
                        var index = _aspxGetDefaultSafariFontSizesHashTable()[curSpan.style.fontSize.toLowerCase()]; ;
                        if(index > -1)
                            curSpan.style.fontSize = ASPx.HtmlEditorClasses.DefaultFontSizes[index];
                    }
                }
                ASPx.Attr.RemoveAttribute(curSpan, "size");

                // color
                if(this.isExistAttribute(curSpan, "color")) {
                    var correctColor = this.correctColorValue(ASPx.Attr.GetAttribute(curSpan, "color"));
                    if(!ASPx.Browser.Opera)
                        curSpan.style.color = correctColor;
                    else
                        this.setStylePropForOpera(curSpan, "color", correctColor);
                    ASPx.Attr.RemoveAttribute(curSpan, "color");
                }
            }
            var retHtml = containerElement.innerHTML;
            containerElement = null;

            return retHtml;
        },
        setStylePropForOpera: function(element, name, value) { // B223811
            var oldStyle = element.getAttribute("style");
            var newStyle = oldStyle + "; " + name + ": " + value;
            element.setAttribute("style", newStyle);
        },
        correctColorValue: function(colorValue) {
            if(!colorValue) return null;

            var retColorValue = colorValue;
            if(typeof (colorValue) == "number" || colorValue.substr(0, 3) == "rgb")
                retColorValue = ASPx.Color.ColorToHexadecimal(colorValue);
            return retColorValue;
        },
        closeTags: function(html) {
            var unclosedTagNames = ["img", "br", "input", "hr", "area", "colgroup", "col"];
            for(var i = 0, tagName; tagName = unclosedTagNames[i]; i++) {
                var re = new RegExp("<" + tagName + "(.*?)\/*>", "gi");
                html = html.replace(re, "<"+ tagName +"$1/>");
            }
            return html;
        },
        setInnerHtml: function(element, html) {
            if(ASPx.Browser.IE)
                html = ASPx.HtmlEditorClasses.HtmlProcessor.protectUrlsInHTML(html);
            ASPx.SetInnerHtml(element, html);
            if(ASPx.Browser.IE)
                ASPx.HtmlEditorClasses.HtmlProcessor.restoreUrlsInDOM(element);
        },
        replaceLinkTargetWithBlank: function(parentElem) {
            var linkArray = ASPx.GetNodesByTagName(parentElem, "A");
            for(var i = 0; i < linkArray.length; i++)
                ASPx.Attr.SetAttribute(linkArray[i], "target", "_blank");
        },
        replaceRGBToHex: function(html) {
            return html.replace(styleAttributeRegEx,
                                    function(cssStyleString) {
                                        return cssStyleString.replace(rgbColorCssAttrRegExp, ASPx.Color.ColorToHexadecimal);
                                    }
                                );
        },
        filterHtmlToGetHtml: function(html) {
            if(!ASPx.Browser.IE || ASPx.Browser.MajorVersion > 8)
                html = this.replaceRGBToHex(html);
            return html;
        },
        isExistAttribute: function(element, attrName) {
            var attrObj = ASPx.Attr.GetAttribute(element, attrName);
            return ASPx.IsExists(attrObj) && attrObj != "null";
        },
        replaceNbspWithEmptyText: function(element) {
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 11 && element && element.nodeType == 1) {
                var textNodes = [];
                ASPx.GetTextNodes(element, textNodes);
                for(var i = 0, node; node = textNodes[i]; i++) {
                    var parent = node.parentNode;
                    if(parent.innerHTML == "&nbsp;" && parent.nodeName != "TD")
                        parent.innerText = "";
                }
            }
        },
        removeEmptyInlineElements: function(element) {
            if(!element || element.nodeType == 3) return;
            var expr = /IFRAME|IMG|SCRIPT|AREA|MAP|INPUT|SELECT|OPTION|OPTGROUP|TEXTAREA|DATALIST|EMBED|VIDEO|AUDIO|OBJECT/;
            for(var i = 0, childNode; childNode = element.childNodes[i]; i++) {
                if(childNode.nodeType != 1 || expr.test(childNode.nodeName) || childNode.nodeName == "BR") continue;
                if(ASPx.GetCurrentStyle(childNode)["display"] == "inline") {
                    if(!ASPx.GetInnerText(childNode) && !expr.test(childNode.innerHTML.toUpperCase()) && (!ASPx.HtmlEditorClasses.Utils.elementAttributesContains(childNode) || childNode.nodeName == "SPAN" && !childNode.id && childNode.parentNode.nodeName == "P")) {
                        var brElements = ASPx.GetNodesByTagName(childNode, "BR");
                        if(brElements.length > 0) {
                            for(var i = 0, brElement; brElement = brElements[i]; i++)
                                childNode.parentNode.insertBefore(brElement.cloneNode(false), childNode);
                        }
                        ASPx.RemoveElement(childNode);
                    }
                }
                else { 
                    this.removeEmptyInlineElements(childNode);
                    if(childNode.childNodes.length == 0 && (childNode.nodeName == "P" || parent.nadeName == "DIV")) {
                        if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 11) {
                            childNode.innerHTML = "&nbsp;";
                            childNode.innerText = "";
                        }
                        else 
                            childNode.appendChild(childNode.ownerDocument.createElement("BR"));
                    }
                } 
            }
        },
        processingEmptyElements: function(element) {
            if(!element)
                element = this.getDOMElement();
            this.replaceNbspWithEmptyText(element);
            this.removeEmptyInlineElements(element);
        },
        processingFormElements: function(element) {
            var predicate = function(el) { return (el.nodeName == "INPUT" || el.nodeName == "BUTTON") && el.type.toLowerCase() == "submit" };
            var childElements = ASPx.HtmlEditorClasses.Utils.getChildsByPredicate(element, predicate);
            for(var i = 0, child; child = childElements[i]; i++)
                ASPx.Evt.AttachEventToElement(child, "click", function(evt) { evt.preventDefault(); });
        },
        getDOMElement: function() {
            return null;
        },
        clearFakeBrElements: function(element, html) {
            var outerHTML = element.ownerDocument.createElement("BR").outerHTML;
            var replacedHtml = (html + outerHTML).replace(/<br[^>]*>(?!(<br[^>]*>)*<br[^>]*>$)/gi, "");
            var existingBrElementsCount = 0;
            var childElement = element.lastChild;
            while(childElement && childElement.nodeName == "BR") {
                childElement = childElement.previousSibling;
                existingBrElementsCount++;
            }
            var brElements = replacedHtml.match(/<br[^>]*>/gi);
            if(brElements && brElements.length > existingBrElementsCount)
                html = html.substring(0, html.length - outerHTML.length * (brElements.length - existingBrElementsCount));
            return html;
        },
        setTextInputUnselectable: function(html) {
            return html.replace(/(<input(?:(?![^>]*type[^>]*>)|(?=[^>]*type\s*=\s*['|\"](?:text)*['|\"][^>]*>)).*?)(?:\sstyle\s*=\s*(?:(?:\"([^\"]*)\")|(?:'([^']*)')))*([^>]*)/gi,"$1 style=\"-moz-user-select:none; $2$3\"$4");
        },
        removeMozUserSelectStyleAttribute: function(html) {
            return html.replace(/(<[a-zA-z][a-z\\d]*.*?)-moz-user-select:\s*none;\s*([^>]*)/gi,"$1$2");
        }
    });

    ASPx.HtmlEditorClasses.HtmlProcessor.getAttributeValueByName = function(html, attrName, isStyleAttr) {
        var regExp = isStyleAttr ? new RegExp("[ '\"]" + attrName + "\\s*:\\s*([^\"'; ]*)[\"'; ]", "gi") : new RegExp("\\s" + attrName + "\\s*=\\s*[\"']([^\"']*)[\"']", "gi");
        var array = html.match(regExp)
        if(array)
            return array[0].replace(regExp, "$1");
        return "";
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.addAttrValueToStyleValue = function(attrName, value, styleAttrValue) {
        if(value) {
            if(styleAttrValue.toLowerCase().indexOf(attrName) < 0)
                styleAttrValue += attrName + ":" + value + "px;";
            else
                styleAttrValue = styleAttrValue.replace(new RegExp("((^|\\s|;)" + attrName + "\\s*:\\s*)[^;]*(px|em|pt|%);", "gi"), "$1" + value + "$3;");
        }
        return styleAttrValue;
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.mergeStyleAttrValue = function(styleAttValue, elementHtml, attrName) {
        var attValue = ASPx.HtmlEditorClasses.HtmlProcessor.getAttributeValueByName(elementHtml, attrName);
        if(attValue && styleAttValue.toLowerCase().indexOf(attrName) < 0)
            styleAttValue += attrName + ":" + attValue + "px;";
        return styleAttValue;
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.getSizeAttrValue = function(specialImageHtml, attrName) {
        var widthAttValue = ASPx.HtmlEditorClasses.HtmlProcessor.getAttributeValueByName(specialImageHtml, attrName, true);
        if(!widthAttValue)
            widthAttValue = ASPx.HtmlEditorClasses.HtmlProcessor.getAttributeValueByName(specialImageHtml, attrName);
        return (Math.round(parseFloat(widthAttValue))).toString();
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.getExtension = function(src) {
        return src ? src.replace(/[\s\S]*?.(\w*)\s*$/gi, "$1") : "";
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.getMediaClassNameByExtension = function(extension) {
        if(extension) {
            if(extension.toLowerCase() == "swf")
                return ASPx.HtmlEditorClasses.MediaCssClasses.Flash;
            else if(/avi|wmv|mpeg|mpg|mpe|mp4|ogg|mkv/i.test(extension))
                return ASPx.HtmlEditorClasses.MediaCssClasses.Video;
            else if(/wav|wma|mp3|mid|midi|snd/.test(extension))
                return ASPx.HtmlEditorClasses.MediaCssClasses.Audio;
        }
        return "";
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.getMediaClassNameByTypeAttr = function(typeAttrValue) {
        if(typeAttrValue && typeAttrValue.toLowerCase().indexOf("flash") > -1)
            return ASPx.HtmlEditorClasses.MediaCssClasses.Flash;
        return "";
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.getMediaClassNameByTagName = function(tagName) {
        if(/video/i.test(tagName))
            return ASPx.HtmlEditorClasses.MediaCssClasses.Video;
        else if(/audio/i.test(tagName))
            return ASPx.HtmlEditorClasses.MediaCssClasses.Audio;
        else if(/embed/i.test(tagName) || /object/i.test(tagName))
            return ASPx.HtmlEditorClasses.MediaCssClasses.Flash;
        return "";
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.getMediaClassName = function(elementHtml, tagName) {
        var className;
        var value = ASPx.HtmlEditorClasses.HtmlProcessor.getAttributeValueByName(elementHtml, "data");
        if(!value)
            value = ASPx.HtmlEditorClasses.HtmlProcessor.getAttributeValueByName(elementHtml, "src");
        if(value && ASPx.HtmlEditorClasses.YoutubeObject.getYoutubeIdFromURL(value))
            return /iframe/i.test(tagName) ? ASPx.HtmlEditorClasses.MediaCssClasses.YouTube : ASPx.HtmlEditorClasses.MediaCssClasses.NotSupported;
        else {
            var extension = ASPx.HtmlEditorClasses.HtmlProcessor.getExtension(value);
            className = ASPx.HtmlEditorClasses.HtmlProcessor.getMediaClassNameByExtension(extension);
            if(className)
                return className != ASPx.HtmlEditorClasses.MediaCssClasses.Flash && (/embed/i.test(tagName) || /object/i.test(tagName)) ? ASPx.HtmlEditorClasses.MediaCssClasses.NotSupported : className;
        }
        value = ASPx.HtmlEditorClasses.HtmlProcessor.getAttributeValueByName(elementHtml, "type");
        className = ASPx.HtmlEditorClasses.HtmlProcessor.getMediaClassNameByTypeAttr(value);
        return className ? className : ASPx.HtmlEditorClasses.HtmlProcessor.getMediaClassNameByTagName(tagName);
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.createSpecialImageHtml = function(elementHtml, id, className, styleAttValue, imageUrl, defaultSize) {
        var createAttr = function(name, value) { return value ?  name + "=\"" + value + "\" " : ""; };
        var fakeAttrs = "";
        if(defaultSize.height)
            fakeAttrs += ASPx.HtmlEditorClasses.DefaultHeightAttributeName + "=\"true\" ";
        if(defaultSize.width)
            fakeAttrs += ASPx.HtmlEditorClasses.DefaultWidthAttributeName + "=\"true\" ";
        return "<img " + ASPx.HtmlEditorClasses.SpecialImageAttributeName + "=\"" + ASPx.HtmlEditorClasses.HtmlProcessor.encodeURIComponent(elementHtml) + "\" " + createAttr("id", id) + createAttr("class", className) + fakeAttrs + createAttr("style", styleAttValue) + "src=\"" + imageUrl + "\" />";
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.replaceElementsToSpecialImage = function(doc, html, optionsArray) {
        for(var j = 0, options; options = optionsArray[j]; j++) {
            if(!options.regExp)
                html = ASPx.HtmlEditorClasses.HtmlProcessor.replaceObjectElementToSpecialImage(doc, html, options);
            else {
                var elementHtmlArray = html.match(options.regExp);
                if(elementHtmlArray && elementHtmlArray.length > 0) {
                    for(var i = 0, elementHtml; elementHtml = elementHtmlArray[i]; i++) {
                        var mediaClassName = ASPx.HtmlEditorClasses.HtmlProcessor.getMediaClassName(elementHtml, options.tagName);
                        var classNameAttrValue = ASPx.HtmlEditorClasses.HtmlProcessor.getAttributeValueByName(elementHtml, "class");
                        if(classNameAttrValue)
                            mediaClassName += " " + classNameAttrValue;
                        var styleAttrValue = ASPx.HtmlEditorClasses.HtmlProcessor.getAttributeValueByName(elementHtml, "style");
                        styleAttrValue = ASPx.HtmlEditorClasses.HtmlProcessor.mergeStyleAttrValue(styleAttrValue, elementHtml, "width");
                        styleAttrValue = ASPx.HtmlEditorClasses.HtmlProcessor.mergeStyleAttrValue(styleAttrValue, elementHtml, "height");
                        var defaultSize = { height: false, width: false };
                        if(!/height/gi.test(styleAttrValue)) {
                            styleAttrValue += "height: " + options.imageHeight + "px;";
                            defaultSize.height = true;
                        }
                        if(!/width/gi.test(styleAttrValue)) {
                            styleAttrValue += "width: " + options.imageWidth + "px;";
                            defaultSize.width = true;
                        }
                        var id = ASPx.HtmlEditorClasses.HtmlProcessor.getAttributeValueByName(elementHtml, "id");
                        html = html.replace(elementHtml, ASPx.HtmlEditorClasses.HtmlProcessor.createSpecialImageHtml(elementHtml, id, mediaClassName, styleAttrValue, ASPx.EmptyImageUrl, defaultSize));
                    }
                }
            }
        }
        return html;
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.restoreElementBySpecialImage = function(html) {
        var specialImageHtmlArray = html.match(new RegExp("<img[^>]*" + ASPx.HtmlEditorClasses.SpecialImageAttributeName + "[^>]*\>", "gi"));
        if(specialImageHtmlArray && specialImageHtmlArray.length > 0) {
            for(var i = 0, specialImageHtml; specialImageHtml = specialImageHtmlArray[i]; i++) {
                var elementHtml = ASPx.HtmlEditorClasses.HtmlProcessor.getAttributeValueByName(specialImageHtml, ASPx.HtmlEditorClasses.SpecialImageAttributeName);
                elementHtml = ASPx.HtmlEditorClasses.HtmlProcessor.decodeURIComponent(elementHtml);
                var widthAttValue = ASPx.HtmlEditorClasses.HtmlProcessor.getSizeAttrValue(specialImageHtml, "width");
                var heightAttValue = ASPx.HtmlEditorClasses.HtmlProcessor.getSizeAttrValue(specialImageHtml, "height");
                var styleAttValue = ASPx.HtmlEditorClasses.HtmlProcessor.getAttributeValueByName(elementHtml, "style");
                var options = ASPx.HtmlEditorClasses.getReplacingElementsOptionsByTagName(elementHtml.replace(/^<([^\s]*)[\s\S]*/gi, "$1"));
                if(!isNaN(widthAttValue) && (parseInt(widthAttValue) != options.imageWidth || !ASPx.HtmlEditorClasses.HtmlProcessor.getAttributeValueByName(specialImageHtml, ASPx.HtmlEditorClasses.DefaultWidthAttributeName)))
                    styleAttValue = ASPx.HtmlEditorClasses.HtmlProcessor.addAttrValueToStyleValue("width", widthAttValue, styleAttValue);
                var defaultWidthAttributeValue = ASPx.HtmlEditorClasses.HtmlProcessor.getAttributeValueByName(specialImageHtml, ASPx.HtmlEditorClasses.DefaultWidthAttributeName);
                if(!isNaN(heightAttValue) && (parseInt(heightAttValue) != options.imageHeight || !ASPx.HtmlEditorClasses.HtmlProcessor.getAttributeValueByName(specialImageHtml, ASPx.HtmlEditorClasses.DefaultHeightAttributeName)))
                    styleAttValue = ASPx.HtmlEditorClasses.HtmlProcessor.addAttrValueToStyleValue("height", heightAttValue, styleAttValue);
                if(styleAttValue) {
                    if(elementHtml.toLowerCase().indexOf("style") > -1)
                        elementHtml = elementHtml.replace(new RegExp("(^\\s*<[\\s\\S]*style\\s*=\\s*[\"'])[^\"']*([\"'][^>]*>{1})", "gi"), "$1" + styleAttValue + "$2");
                    else
                        elementHtml = elementHtml.replace(new RegExp("(^\\s*<[\\s\\S]*?)(/?>{1})", "gi"), "$1 style=\"" + styleAttValue + "\"$2");
                }
                html = html.replace(specialImageHtml, elementHtml);
            }
        }
        return html;
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.decodeURIComponent = function(html) {
        html = decodeURIComponent(html);
        html = html.replace(/%27/gi, "'");
        html = html.replace(/%22/gi, "\"");
        return html;
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.encodeURIComponent = function(html) {
        html = encodeURIComponent(html);
        html = html.replace(/'/gi, "%27");
        html = html.replace(/"/gi, "%22");
        return html;
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.replaceDomElementToSpecialImageElement = function(doc, element) {
        var elementHtml = element.outerHTML;
        var html = ASPx.HtmlEditorClasses.HtmlProcessor.replaceElementsToSpecialImage(doc, elementHtml, ASPx.HtmlEditorClasses.getReplacingElementsOptionsArray());
        var divElement = doc.createElement("DIV");
        divElement.innerHTML = html;
        var imageElement = divElement.childNodes[0];
        element.parentNode.insertBefore(imageElement, element);
        element.parentNode.removeChild(element);
        return imageElement;
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.restoreDomElementBySpecialImageElement = function(doc, element) {
        var html = ASPx.HtmlEditorClasses.HtmlProcessor.restoreElementBySpecialImage(element.outerHTML);
        if(html) {
            var divElement = doc.createElement("DIV");
            divElement.innerHTML = html;
            return divElement.childNodes[0];
        }
        return null;
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.replaceObjectElementToSpecialImage = function(doc, html, options) {
        if(html.toLowerCase().indexOf("<object") < 0)
            return html;
        var divElement = doc.createElement("DIV");
        divElement.innerHTML = html;
        var objectElements = ASPx.GetNodesByTagName(divElement, options.tagName);
        for(var i = 0, element; element = objectElements[i]; i++) {
            var imgElement = doc.createElement("IMG");
            var src;
            if(element.data)
                src = element.data;
            else {
                for(var j = 0, child; child = element.childNodes[j]; j++) {
                    if(child.name && (child.name.toLowerCase() == "movie" || child.name.toLowerCase() == "url")) {
                        src = child.value;
                        break;
                    }
                }
            }
            var className;
            var extension = ASPx.HtmlEditorClasses.HtmlProcessor.getExtension(src);
            className = ASPx.HtmlEditorClasses.HtmlProcessor.getMediaClassNameByExtension(extension) || ASPx.HtmlEditorClasses.YoutubeObject.getYoutubeIdFromURL(src);
            if(!className)
                className = ASPx.HtmlEditorClasses.HtmlProcessor.getMediaClassNameByTypeAttr(element.type);
            ASPx.Attr.SetAttribute(imgElement, ASPx.HtmlEditorClasses.SpecialImageAttributeName, ASPx.HtmlEditorClasses.HtmlProcessor.encodeURIComponent(element.outerHTML));
            if(element.id)
                imgElement.id = element.id;
            imgElement.className = className == ASPx.HtmlEditorClasses.MediaCssClasses.Flash || !className ? ASPx.HtmlEditorClasses.MediaCssClasses.Flash : ASPx.HtmlEditorClasses.MediaCssClasses.NotSupported;
            if(element.className)
                imgElement.className += " " + element.className;
            imgElement.style.cssText = element.style.cssText;
            if(element.width)
                imgElement.width = element.width;
            else if(!ASPx.Attr.GetAttribute(element.style, "width")) {
                ASPx.Attr.SetAttribute(imgElement.style, "width", options.imageWidth + "px");
                if(!ASPx.Browser.NetscapeFamily)
                    ASPx.Attr.SetAttribute(imgElement, ASPx.HtmlEditorClasses.DefaultWidthAttributeName, "true");
            }
            if(element.height)
                imgElement.height = element.height;
            else if(!ASPx.Attr.GetAttribute(element.style, "height")) {
                ASPx.Attr.SetAttribute(imgElement.style, "height", options.imageHeight + "px");
                if(!ASPx.Browser.NetscapeFamily)
                    ASPx.Attr.SetAttribute(imgElement, ASPx.HtmlEditorClasses.DefaultHeightAttributeName, "true");
            }
            imgElement.src = ASPx.EmptyImageUrl;
            element.parentNode.insertBefore(imgElement, element);
        }
        while(objectElements.length > 0)
            objectElements[0].parentNode.removeChild(objectElements[0]);
        return divElement.innerHTML;
    };

    ASPx.HtmlEditorClasses.HtmlProcessor.protectUrlsInHTML = function(html) {
        html = ASPx.HtmlEditorClasses.HtmlProcessor.removeSavedUrlsInHTML(html);
        html = html.replace(protectUrlsARegExpPattern, '$& savedurl=$1');
        html = html.replace(protectUrlsImgRegExpPattern, '$& savedurl=$1');
        html = html.replace(protectUrlsAreaRegExpPattern, '$& savedurl=$1');
        return html;
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.removeSavedUrlsInHTML = function(html) {
        return html.replace(removeSavedUrlsRegExpPattern, '');
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.preserveButtonTags = function(container) {
        if(!ASPx.Browser.WebKitFamily && !ASPx.Browser.Firefox) return;
        for(var child = container.firstChild; child; child = child.nextSibling) {
            if(child.nodeType != 1) continue;
            if(child.tagName == "BUTTON") {
                var safeNode = document.createElement("INPUT");
                ASPx.Attr.CopyAllAttributes(child, safeNode);
                var savedData = "dx_savedButton:" + JSON.stringify({ dx_savedContent: child.innerHTML || "", dx_savedValue: child.value, dx_savedType: child.type, dx_savedName: child.name });
                safeNode.value = ASPx.GetInnerText(child);
                safeNode.type = "button";
                safeNode.name = savedData;
                child.parentNode.replaceChild(safeNode, child);
                child = safeNode;
            }
            else if(child.childNodes.length > 0)
                ASPx.HtmlEditorClasses.HtmlProcessor.preserveButtonTags(child);
        }
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.removeInternalDocsElements = function(container) {
        var internalDocsElements = ASPx.HtmlEditorClasses.Utils.getChildsByPredicate(container, function(el) { return el.nodeType == 1 && /^docs-internal-guid-/i.test(el.id); });
        for(var i = 0, element; element = internalDocsElements[i]; i++) {
            for(var j = 0, child; child = element.childNodes[j]; j++)
                element.parentNode.insertBefore(child.cloneNode(true), element);
            element.parentNode.removeChild(element);
        }
    }
    ASPx.HtmlEditorClasses.HtmlProcessor.processInnerHtml = function(container) {
        ASPx.HtmlEditorClasses.HtmlProcessor.removeInternalDocsElements(container);
        ASPx.HtmlEditorClasses.HtmlProcessor.preserveButtonTags(container);
        ASPx.HtmlEditorClasses.HtmlProcessor.addEmptyBorderClassInTables(container);
    };
    // If Table doesn't have a borderSize, we should add special css class
    ASPx.HtmlEditorClasses.HtmlProcessor.addEmptyBorderClassInTables = function(parentElem) {
        var tables = ASPx.GetNodesByTagName(parentElem, "TABLE");
        for(var i = 0; i < tables.length; i++) {
            if(ASPx.HtmlEditorTableHelper.IsEmptyBorder(tables[i]))
                ASPx.HtmlEditorTableHelper.AppendEmptyBorderClassName(tables[i]);
        }
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.restoreUrlsInDOM = function(element) {
        var restoreElementsUrlsInDOM = function(elements, tagName) {
            var attributeName = tagName == 'IMG' ? 'src' : 'href';
            var savedAttributeName = 'savedurl';
            for(var i = 0; i < elements.length; i++) {
                var url = ASPx.Attr.GetAttribute(elements[i], savedAttributeName);
                if(url != null) {
                    var savedInnerHTML = elements[i].innerHTML;
                    ASPx.Attr.SetAttribute(elements[i], attributeName, url);

                    if(ASPx.Browser.IE && tagName == 'A' && elements[i].innerHTML != savedInnerHTML) // B148191
                        elements[i].innerHTML = savedInnerHTML;
                }
            }
        }
        restoreElementsUrlsInDOM(ASPx.GetNodesByTagName(element, 'A'), 'A');
        restoreElementsUrlsInDOM(ASPx.GetNodesByTagName(element, 'IMG'), 'IMG');
        restoreElementsUrlsInDOM(ASPx.GetNodesByTagName(element, 'AREA'), 'AREA');
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.preserveAttribute = function(html, attributeNameRegExp) {
        return ASPx.Str.CompleteReplace(html, ASPx.HtmlEditorClasses.HtmlProcessor.getAttributePreserveRegExp(attributeNameRegExp), "$1" + ASPx.HtmlEditorClasses.preservedAttributeNamePrefix + "$3$4");
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.getAttributePreserveRegExp = function(attributeNameRegExp) {
        return new RegExp("(" + ASPx.HtmlEditorClasses.TagsWithAttributeRegExpPattern + "\\s+)(" + attributeNameRegExp + ")(\\s*=)*", "gi");
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.clearExcessStyleToElement = function(parent, doc) {
        getParentBackgroundColorValue = function(element) {
            var result = "", parent = element;
            while(!result.replace(/rgba\(\d*,\s*\d*,\s*\d*,\s*0\)/gi, "") && parent.nodeName != "BODY") {
                parent = element.parentNode;
                element = parent;
                result = ASPx.GetCurrentStyle(parent)["background-color"];
            }
            return result;
        }
        for(var i = 0, child; child = parent.childNodes[i]; i++) {
            if(child.nodeType == 1) {
                if(child.style.cssText) {
                    var cssText = child.style.cssText;
                    var attrArray = cssText.match(/[^;]*;/gi);
                    var childStyle = ASPx.GetCurrentStyle(child);
                    var tempElement = child.cloneNode(true);

                    tempElement.style.cssText = "";
                    parent.appendChild(tempElement);
                    var tempElementStyle = ASPx.GetCurrentStyle(tempElement);
                    for(var j = 0, attr; attr = attrArray[j]; j++) {
                        var attrName = ASPx.Str.Trim(attr.replace(/([^:]*)[\s\S]*/gi, "$1"));
                        if(/^(width|height)$/.test(attrName))
                            continue;
                        if(tempElementStyle[attrName] == childStyle[attrName] || attrName == "background-color" && childStyle[attrName] == getParentBackgroundColorValue(parent))
                            child.style[attrName] = "";
                    }
                    tempElement.parentNode.removeChild(tempElement);
                }
                if(child.childNodes.length > 0)
                    ASPx.HtmlEditorClasses.HtmlProcessor.clearExcessStyleToElement(child, doc);
            }
        }
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.clearExcessStyle = function(wrapper, html) {
        var obj = ASPx.HtmlEditorClasses.Utils.insetContentInHiddenContainer(wrapper, html, false);
        var doc = wrapper.getDocument();
        var container = obj.hiddenChild;

        ASPx.HtmlEditorClasses.HtmlProcessor.clearExcessStyleToElement(container, doc);
        html = container.innerHTML;
        obj.hiddenParent.parentNode.removeChild(obj.hiddenParent);
        var selection = wrapper.getSelection();

        selection.clientSelection.SelectExtendedBookmark(obj.bookmark);
        return html;
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.convertStyleElementToInlineStyleElementByHtml = function(headHtml, bodyHtml, doc, needClearExcessStyle) {
        if(headHtml) {
            var iframe = doc.createElement("IFRAME");
            iframe.style.display = "none";
            doc.body.appendChild(iframe);
            iframe.contentDocument.head.innerHTML = headHtml;
            iframe.contentDocument.body.innerHTML = bodyHtml;
            ASPx.HtmlEditorClasses.HtmlProcessor.convertStyleElementToInlineStyleElement(iframe.contentDocument.body);
            if(needClearExcessStyle)
                ASPx.HtmlEditorClasses.HtmlProcessor.clearExcessStyleToElement(iframe.contentDocument.body, doc);
            bodyHtml = iframe.contentDocument.body.innerHTML;
            iframe.parentNode.removeChild(iframe);
        }
        return bodyHtml;
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.convertStyleElementToInlineStyleElement = function(parent, doc) {
        var styleAttributes = ["color", "background-color", "font-size", "font-family", "font-weight", "font-style", "margin", "margin-bottom", "margin-top", "margin-right", "margin-left"];
        for(var i = 0, child; child = parent.childNodes[i]; i++) {
            if(child.nodeType == 1) {
                var childStyle = ASPx.GetCurrentStyle(child);
                if(!/^<[^>]*mso-list[^>]*>/gi.test(child.outerHTML)) {
                    for(var j = 0, attrName; attrName = styleAttributes[j]; j++) {
                        if(!child.style[attrName])
                            child.style[attrName] = childStyle[attrName];
                    }
                }
                if(child.childNodes.length > 0)
                    ASPx.HtmlEditorClasses.HtmlProcessor.convertStyleElementToInlineStyleElement(child, doc);
            }
        }
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.removeSelectedHiddenContainer = function(html) {
        return html.replace(new RegExp("<[a-zA-z][a-z\\d]*(?:[^>]*id\\s*=\\s*['\"]" + ASPx.HtmlEditorClasses.SelectedHiddenContainerID + "['\"][^>]*)[^>]*>[\\s\\S]*?</[^>]*>", "gi"), "");
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.restoreSrcAttr = function(html) {
        html = html.replace(/(<(img|a|iframe)[^>]*)\s+savedurl\s*=\s*['\"][^'\"]*['\"]([^>]*>)/gi, "$1$3");
        var array = html.match(/<(img|a|iframe)[^>]*data-aspx-saved-src\s*=\s*['\"][^'\"]*['\"][^>]*>/gi);
        if(array) {
            for(var i = 0, itemHtml; itemHtml = array[i]; i++) {
                var savedSrc = itemHtml.replace(/<(img|a|iframe)[^>]*data-aspx-saved-src\s*=\s*['\"]([^'\"]*)['\"][^>]*>/gi, "$2");
                var newItemHtml = itemHtml.replace(/(<(img|a|iframe)[^>]*[^>]*)\s+data-aspx-saved-src\s*=\s*['\"][^'\"]*['\"]([^>]*>)/gi, "$1$3");
                newItemHtml = newItemHtml.replace(/(<(img|a|iframe)[^>]*(src|href)\s*=\s*['\"])[^'\"]*(['\"][^>]*>)/gi, "$1" + savedSrc + "$4");
                html = html.replace(itemHtml, newItemHtml);
            }
        }
        return html;
    };

    ASPx.HtmlEditorClasses.HtmlProcessor.tagFilteringByHtml = function(html, tagFilterSettrings) {
        if(!html || !tagFilterSettrings || tagFilterSettrings.list.length == 0 && tagFilterSettrings.filterMode == ASPx.HtmlEditorClasses.FilterMode.BlackList)
            return html;
        if(tagFilterSettrings.filterMode == ASPx.HtmlEditorClasses.FilterMode.BlackList) {
            for(var i = 0, tagName; tagName = tagFilterSettrings.list[i]; i++) {
                html = html.replace(new RegExp("<" + tagName + "[^>]*>", "gi"), '');
                html = html.replace(new RegExp("</" + tagName + ">", "gi"), '');
            }
        }
        else {
            if(tagFilterSettrings.list.length == 0)
                html = html.replace(new RegExp("<[^>]*>", "gi"), '');
            else {
                var tagsRegExp = tagFilterSettrings.list[0];
                for(var i = 1, tagName; tagName = tagFilterSettrings.list[i]; i++)
                    tagsRegExp += "|" + tagName;
                html = html.replace(new RegExp("<(?!((" + tagsRegExp + ")|/(" + tagsRegExp + ")))[^>]*>", "gi"), '');
            }
        }
        return html;
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.getAttributeByName = function(elementHtml, attrName) {
        var regExp = new RegExp("\\s+" + attrName + "\s*=\s*['\"]", "gi");
        if(!regExp.test(elementHtml))
            return "";
        var mark = elementHtml.replace(new RegExp("[\\s\\S*]*\\s+" + attrName + "\\s*=\\s*(['\"])[\\s\\S*]*", "gi"), "$1");
        return elementHtml.replace(new RegExp("[\\s\\S*]*(\\s+" + attrName + "\\s*=\\s*['\"][^\\" + mark + "]*\\" + mark + ")[\\s\\S*]*", "gi"), "$1");
    }
    ASPx.HtmlEditorClasses.HtmlProcessor.attrFilteringByHtml = function(html, attrFilterSettings, styleAttrFilterSettings) {
        if(!html || !attrFilterSettings && !styleAttrFilterSettings)
            return html;
        else if(!attrFilterSettings && styleAttrFilterSettings)
            attrFilterSettings = { list: [],  filterMode: ASPx.HtmlEditorClasses.FilterMode.BlackList };

        var array = html.match(/<[a-zA-z][a-z\d]*[^>]*>/gi);
        if(array) {
            var styleAttrRegExp = /\s*style\s*=\s*['\"]([\s\S]*)['\"]/gi;
            for(var i = 0, itemHtml; itemHtml = array[i]; i++) {
                var attrsHtml = itemHtml.replace(/<[a-zA-z][a-z\d]*([^>]*)>/gi, "$1");
                if(!ASPx.Str.Trim(attrsHtml))
                    continue;
                var replacement = "";
                if(attrFilterSettings.filterMode == ASPx.HtmlEditorClasses.FilterMode.BlackList) {
                    replacement = attrsHtml;
                    for(var j = 0, attrName; attrName = attrFilterSettings.list[j]; j++)
                        replacement = replacement.replace(ASPx.HtmlEditorClasses.HtmlProcessor.getAttributeByName(attrsHtml, attrName), "");
                    if(/\s*style\s*=\s*['\"]/.test(replacement)) {
                        var styleAttr = ASPx.HtmlEditorClasses.HtmlProcessor.getAttributeByName(attrsHtml, "style");
                        var styleAttrValue = styleAttr.replace(styleAttrRegExp, "$1");
                        var processedStyleAttrValue = ASPx.HtmlEditorClasses.HtmlProcessor.styleAttrFilteringByHtml(styleAttrValue, styleAttrFilterSettings);
                        if(processedStyleAttrValue)
                            replacement = replacement.replace(styleAttrValue, processedStyleAttrValue);
                        else
                            replacement = replacement.replace(new RegExp("\\s+style\\s*=\\s*['\"]" + styleAttrValue + "['\"]", "gi"), "");
                    }
                }
                else {
                    var savedAttrs = [];
                    for(var j = 0, attrName; attrName = attrFilterSettings.list[j]; j++) {
                        var regExp = new RegExp(attrName + "\\s*=\\s*['\"]", "gi");
                        if(regExp.test(attrsHtml)) {
                            var attrHtml = ASPx.HtmlEditorClasses.HtmlProcessor.getAttributeByName(attrsHtml, attrName);
                                
                            if(/style/i.test(attrName)) {
                                var styleAttrValue = attrHtml.replace(styleAttrRegExp, "$1");
                                var processedStyleAttrValue = ASPx.HtmlEditorClasses.HtmlProcessor.styleAttrFilteringByHtml(styleAttrValue, styleAttrFilterSettings);
                                attrHtml = processedStyleAttrValue ? attrHtml.replace(styleAttrValue, processedStyleAttrValue) : "";
                            }
                            if(attrHtml)
                                savedAttrs.push(attrHtml);
                        }
                    }
                    for(var j = 0, savedAttr; savedAttr = savedAttrs[j]; j++)
                        replacement += savedAttr;
                }
                replacement = ASPx.Str.Trim(replacement);
                var processedItemHtml = itemHtml.replace(attrsHtml, (!replacement ? "" : " ") + replacement);
                html = html.replace(itemHtml, processedItemHtml);
            }
        }
        return html;
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.styleAttrFilteringByHtml = function(styleValue, filterSettings) {
        var result = "";
        if(!filterSettings || !styleValue)
            return styleValue;
        else if(filterSettings.list.length == 0)
            return filterSettings.filterMode == ASPx.HtmlEditorClasses.FilterMode.BlackList ? styleValue : result;

        if(filterSettings.filterMode == ASPx.HtmlEditorClasses.FilterMode.BlackList) {
            result = styleValue;
            for(var j = 0, styleAttrName; styleAttrName = filterSettings.list[j]; j++) {
                var styleAttrValueArray = styleValue.match(new RegExp("(\\s|;|^)" + styleAttrName + "\\s*:\\s*[^;]*;*", "gi"));
                if(styleAttrValueArray)
                    result = result.replace(styleAttrValueArray[0], "");
            }
        }
        else {
            var savedStyleAttrs = [];
            for(var j = 0, styleAttrName; styleAttrName = filterSettings.list[j]; j++) {
                var regExp = new RegExp("(\\s|;|^)" + styleAttrName + "\\s*:\\s*", "gi");
                if(regExp.test(styleValue)) {
                    var styleAttrValueArray = styleValue.match(new RegExp(styleAttrName + "\\s*:\\s*[^;]*;*", "gi"));
                    if(styleAttrValueArray)
                        savedStyleAttrs.push(styleAttrValueArray[0]);
                }
            }
            for(var j = 0, savedStyleAttr; savedStyleAttr = savedStyleAttrs[j]; j++)
                result += savedStyleAttr;
        }
        return ASPx.Str.Trim(result);
    };
    ASPx.HtmlEditorClasses.HtmlProcessor.tagFilteringByDomElement = function(element, wrapper) {
        var filterSettings = wrapper.settings.tagFilter;
        var tagsRegExp;
        if(!element || element.nodeType == 3 || !filterSettings || filterSettings.list.length == 0 && filterSettings.filterMode == ASPx.HtmlEditorClasses.FilterMode.BlackList)
            return;
        else if(filterSettings.list.length == 0)
            tagsRegExp = /[\s\S]*/i;
        else {
            tagsRegExp = "^" + filterSettings.list[0] + "$";
            for(var i = 1, tagName; tagName = filterSettings.list[i]; i++)
                tagsRegExp += "|^" + tagName + "$";
            tagsRegExp = new RegExp(tagsRegExp, "i");
        }
        var predicate = filterSettings.filterMode == ASPx.HtmlEditorClasses.FilterMode.BlackList || filterSettings.list.length == 0 ? 
            function(element) { return element.nodeType == 1 && element.nodeName != "BODY" && tagsRegExp.test(element.nodeName); } :
            function(element) { return element.nodeType == 1 && element.nodeName != "BODY" && !tagsRegExp.test(element.nodeName); };
        var elements = ASPx.HtmlEditorClasses.Utils.getChildsByPredicate(element, predicate);
        if(elements.length > 0) {
            for(var i = elements.length - 1, el; el = elements[i]; i--) {
                if(wrapper.tagIsFilteredByConditional(el.nodeName, el)) {
                    for(var j = 0, child; child = el.childNodes[j]; j++)
                        el.parentNode.insertBefore(child.cloneNode(true), el);
                    el.parentNode.removeChild(el);
                }
            }
        }
    }
    ASPx.HtmlEditorClasses.HtmlProcessor.attrFilteringByDomElement = function(element, attrFilterSettings, styleAttrSettings) {
        if(!attrFilterSettings && !styleAttrSettings)
            return;
        else if(!attrFilterSettings && styleAttrSettings)
            attrFilterSettings = { list: [],  filterMode: ASPx.HtmlEditorClasses.FilterMode.BlackList };
        ASPx.HtmlEditorClasses.HtmlProcessor.attrFilteringByDomElementCore(element, attrFilterSettings, styleAttrSettings);
    }
    ASPx.HtmlEditorClasses.HtmlProcessor.attrFilteringByDomElementCore = function(element, attrFilterSettings, styleAttrFilterSettings) {
        if(!element || element.nodeType == 3)
            return;
        if(element.nodeName != "BODY") {
            if(attrFilterSettings.filterMode == ASPx.HtmlEditorClasses.FilterMode.BlackList) {
                for(var i = 0, attrName; attrName = attrFilterSettings.list[i]; i++)
                    ASPx.Attr.RemoveAttribute(element, attrName);
            }
            else {
                var attrs = element.attributes;
                for(var i = 0, attrName; attrName = attrs[i]; i++) {
                    if(!ASPx.Data.ArrayContains(attrFilterSettings.list, attrName))
                        ASPx.Attr.RemoveAttribute(element, attrName);
                }
            }
            if(styleAttrFilterSettings && element.style.cssText)
                element.style.cssText = ASPx.HtmlEditorClasses.HtmlProcessor.styleAttrFilteringByHtml(element.style.cssText, styleAttrFilterSettings);
        }
        for(var i = 0, child; child = element.childNodes[i]; i++)
            ASPx.HtmlEditorClasses.HtmlProcessor.attrFilteringByDomElementCore(child, attrFilterSettings, styleAttrFilterSettings);
    }
    ASPx.HtmlEditorClasses.HtmlProcessor.filteringByHtml = function(html, tagFilterSettings, attrFilterSettings, styleAttrFilterSettings) {
        html = ASPx.HtmlEditorClasses.HtmlProcessor.tagFilteringByHtml(html, tagFilterSettings);
        return ASPx.HtmlEditorClasses.HtmlProcessor.attrFilteringByHtml(html, attrFilterSettings, styleAttrFilterSettings);
    }
    ASPx.HtmlEditorClasses.HtmlProcessor.filteringByDomElement = function(element, wrapper) {
        ASPx.HtmlEditorClasses.HtmlProcessor.tagFilteringByDomElement(element, wrapper);
        ASPx.HtmlEditorClasses.HtmlProcessor.attrFilteringByDomElement(element, wrapper.settings.attrFilter, wrapper.settings.styleAttrFilter);
    }
    ASPx.HtmlEditorClasses.HtmlProcessor.getBodyContent = function(html) {
        var match = html.match(/<body[^>]*>[\s\S]*?<\/body>/gi);
        return match ? match[0].replace(/<body[^>]*>|<\/body>/gi, "") : html;
    }
    ASPx.HtmlEditorClasses.HtmlProcessor.replaceBodyContent = function(html, bodyContent) {
        return html.replace(/(<body[^>]*>)[\s\S]*?(<\/body>)/gi,"$1" + bodyContent + "$2")
    }
    ASPx.HtmlEditorClasses.HtmlProcessor.clearDXElements = function(html) {
        return html.replace(/<span[^>]*id\s*=\s*['"]dxhe[^>]*><\/span>/gi, "");
    }
    ASPx.HtmlEditorClasses.HtmlProcessor.encodeTextContent = function(html) {
        if(!html)
            return html;
        var encodeSymbols = [ 
            { search: "\"", replacement: "&quot;" },
            { search: "'", replacement: "&#39;" }
        ];
        var hasBodyTag = function(html) { return /<body[^>]*>/gi.test(html); };
        var bodyHtml = hasBodyTag(html) ? ASPx.HtmlEditorClasses.HtmlProcessor.getBodyContent(html) : html;
        if(!bodyHtml)
            return html;

        var safeHtmlObject = ASPx.HtmlEditorClasses.HtmlProcessor.safeHtml(bodyHtml);

        for(var i = 0, item; item = encodeSymbols[i]; i++)
            safeHtmlObject.html = safeHtmlObject.html.replace(new RegExp(item.search, "gi"), item.replacement);

        bodyHtml = ASPx.HtmlEditorClasses.HtmlProcessor.restoreHtml(safeHtmlObject);
        return hasBodyTag(html) ? ASPx.HtmlEditorClasses.HtmlProcessor.replaceBodyContent(html, bodyHtml) : bodyHtml;
    }
    ASPx.HtmlEditorClasses.HtmlProcessor.safeHtml = function(html) {
        var safeHtmlRegExp = ["<script[^>]*>[\\s\\S]*?<\\/script>", "<!--[\\s\\S]*?-->", "<[^>]*>"];
        var defaultId = "!dxhesafehtml";
        var matches;
        for(var i = 0; regExp = safeHtmlRegExp[i]; i++) {
            regExp = new RegExp(regExp, "gi");
            var matchRes = html.match(regExp);
            if(!matchRes)
                continue;
            matches = !matches ? matchRes : matches.concat(matchRes);
        }
        var safeHtmlArray = [];
        if(matches) {
            for(var i = 0; matcheHtml = matches[i]; i++) {
                var contentId = defaultId + "[" + i + "]";
                safeHtmlArray.push({ html: matcheHtml, contentId: contentId });
                html = html.replace(matcheHtml, contentId);
            }
        }
        return { html: html, array: safeHtmlArray };
    }
    ASPx.HtmlEditorClasses.HtmlProcessor.restoreHtml = function(safeHtmlObject) {
        var html = safeHtmlObject.html;
        for(var i = 0, item; item = safeHtmlObject.array[i]; i++)
            html = html.replace(item.contentId, item.html);
        return html;
    }
})();