(function() {
    ASPx.HtmlEditorClasses.Commands.PasteFromWord = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        // cmdValue = { html, stripFontFamily };
	    Execute: function(cmdValue, wrapper) {
            if(!ASPx.Browser.WebKitTouchUI) {
                var inlineNode;
                var doc = wrapper.getDocument();
                if(ASPx.Browser.NetscapeFamily) {
                
                    if(doc.body.childNodes.length == 0 || (doc.body.childNodes.length == 1 && doc.body.firstChild.nodeName == "BR"))
                        inlineNode = doc.createTextNode("\xA0");
                }
                ASPx.HtmlEditorClasses.Commands.Command.prototype.Execute.apply(this, arguments);
                var isSuccessfully = this.owner.getCommand(ASPxClientCommandConsts.PASTEHTML_COMMAND).Execute(
	                ASPx.HtmlEditorClasses.Commands.PasteFromWord.ClearWordFormatting(cmdValue.html, cmdValue.stripFontFamily, doc, true), wrapper);	
                if(inlineNode)
                    doc.body.appendChild(inlineNode);
                return isSuccessfully;
            }
            else
                alert(ASPx.HtmlEditorClasses.Commands.Browser.Clipboard.NotAllowedMessage[ASPxClientCommandConsts.PASTE_COMMAND]);
	    },
        canBeExecutedOnSelection: function(wrapper) {
            return true;
        }
    });

    ASPx.HtmlEditorClasses.Commands.PasteFromWord.ClearWordFormatting = function(html, stripFontFamily, doc, stripClassName) {
        // remove word attributes
        html = ASPx.HtmlEditorClasses.Commands.PasteFromWord.ClearWordAttributes(html);
        // remove special Word tags
        html = html.replace(/<\!--\[if\s*supportFields\]>[\s\S]*?<\!\[endif\]-->/g, '');
	    html = html.replace(/<\/?\w+:[^>]*>/gi, '');
	    html = html.replace(/<STYLE[^>]*>[\s\S]*?<\/STYLE[^>]*>/gi, '');
	    html = html.replace(/<(?:META|LINK)[^>]*>\s*/gi, '');
	    html = html.replace(/<\\?\?xml[^>]*>/gi, '');
	    html = html.replace(/<o:[pP][^>]*>\s*<\/o:[pP]>/gi, '');
	    html = html.replace(/<o:[pP][^>]*>.*?<\/o:[pP]>/gi, '&nbsp;');
	    html = html.replace(/<st1:.*?>/gi, '');
	    html = html.replace(/<\!-*[\s\S]*?-*>/g, '');
        html = html.replace(/<xml>[\s\S]*?<\/xml>/gi, '');
		
	    // remove empty attributes
	    html =  html.replace(/\s*style="\s*"/gi, '');
	    html = html.replace(/style=""/ig, "");
	    html = html.replace(/style=''/ig, "");

        // clean style attributes (B208534)
        var stRegExp = new RegExp('(?:style=\\")([^\\"]*)(?:\\")', 'gi');
        html = html.replace(stRegExp, function(str) {
            str = str.replace(/&quot;/gi, "'")
            str = str.replace(/&#xA;/gi, " ")
            return str;
        });
	
        // replace ugly Word markup
        html = html.replace(/^\s|\s$/gi, '');
	    html = html.replace(/<p>&nbsp;<\/p>/gi, '<br /><br />');
	
	    html = html.replace(/<font\s*>([^<>]+)<\/font>/gi, '$1');
	    html = html.replace(/<span\s*><span\s*>([^<>]+)<\/span><\/span>/ig, '$1');
	    html = html.replace(/<span>([^<>]+)<\/span>/gi, '$1');
	
	    if (stripFontFamily) {
		    html = html.replace(/\s*face="[^"]*"/gi, '');
		    html = html.replace(/\s*face=[^ >]*/gi, '');
		    html = html.replace(/\s*FONT-FAMILY:[^;"]*;?/gi, '');
	    }
	
	    // Remove nested empty tags
        // safe empty td
        html = html.replace( /<td([^>]*)>\s*<\/td>/gi, '<td$1>&nbsp;</td>' );

        var getEmptyTags = function(html) {
            var matchRes = html.match(/<([^\s>]+)(\s[^>]*)?><\/\1>/gi),
                result = [];
            if(matchRes) {
                for(var i = 0, item; item = matchRes[i]; i++) {
                    if(!/^<(iframe|input|textarea|select|option|button)/gi.test(item))
                        result.push(item);
                }
            }
            return result;
        } 
        var emptyTags = getEmptyTags(html);
        while(emptyTags.length > 0) {
            for(var i = 0, item; item = emptyTags[i]; i++)
                html = html.replace(item, '');
            emptyTags = getEmptyTags(html);
        }

        re = /<([^\s>]+)(\s[^>]*)?>\s+<\/\1>/g;
        while(html != html.replace(re, ' ' ))
            html = html.replace( re, ' ' );
        html = ASPx.HtmlEditorClasses.Commands.PasteFromWord.MergerFontFamilyAttributes(html);
        html = html.replace(/^\n|\n$/gi, '');
        html = html.replace(/\n/gi, ' ');
    
	    html = ASPx.HtmlEditorClasses.Commands.PasteFromWord.RestoreBrokenLists(html, doc);
        html = html.replace(/\s*mso-list\s*=\s*"[^"]+"/gi, '');
        html = html.replace(/\s*mso-[^:]+:[^;"']+;?/gi, '');
        if (stripClassName)
            html = html.replace(/<(\w[^>]*) class=([^ |>]*)([^>]*)/gi, "<$1$3");

        html = html.replace(/\s*class\s*=\s*["']MsoNormal[^"']*["']/gi, '');
        html = ASPx.HtmlEditorClasses.Commands.PasteFromWord.ClearEmptySpans(html);

        return html;
    };

    ASPx.HtmlEditorClasses.Commands.PasteFromWord.ClearEmptySpans = function (html) {
        var container = document.createElement("div");
        container.innerHTML = html;
        var spans = container.getElementsByTagName("span");

        for (var i = spans.length - 1; i >= 0 ; i--) {
            var target = spans[i];
            if (!ASPx.HtmlEditorClasses.Utils.elementAttributesContains(target)) {
                var content = target.innerHTML,
                parent = target.parentNode;
                target.insertAdjacentHTML('beforeBegin', content)
                parent.removeChild(target);
            }
        }

        return container.innerHTML;
    };

    ASPx.HtmlEditorClasses.Commands.PasteFromWord.ClearWordAttributes = function(html) {
	    html = html.replace(/<(\w[^>]*) lang=([^ |>]*)([^>]*)/gi, "<$1$3");

        html = html.replace(/\s*mso-bidi-font-family/gi, "font-family");
        html = html.replace(/\s*mso-ascii-font-family/gi, "font-family");

        html = html.replace(/\s*mso-hansi-font-family:\s*[^;]*;/gi, "");
        html = html.replace(/\s*mso-fareast-font-family:\s*[^;]*;/gi, "");

	    html = html.replace(/\s*MARGIN: 0cm 0cm 0pt\s*;/gi, '');
	    html = html.replace(/\s*MARGIN: 0cm 0cm 0pt\s*"/gi, "\"");

	    html = html.replace(/\s*TEXT-INDENT: 0cm\s*;/gi, '');
	    html = html.replace(/\s*TEXT-INDENT: 0cm\s*"/gi, "\"");

	    html = html.replace(/\s*PAGE-BREAK-BEFORE: [^\s;]+;?"/gi, "\"");

	    html = html.replace(/\s*FONT-VARIANT: [^\s;]+;?"/gi, "\"") ;

	    html = html.replace(/\s*tab-stops:[^;"]*;?/gi, '') ;
	    html = html.replace(/\s*tab-stops:[^"]*/gi, '') ;
	    return html;
    };
    ASPx.HtmlEditorClasses.Commands.PasteFromWord.RestoreBrokenLists = function(html, doc) {
        var brokenListHtmlArray = html.match(/<p[^>]*(class=[\"']*MsoListParagraphCxSpFirst[\"']*[\s\S]*?class=[\"']*MsoListParagraphCxSpLast[\"']*|class=[\"']*MsoListParagraph[\"']*)[\s\S]*?<\/p>/gi);
        if(brokenListHtmlArray && brokenListHtmlArray.length > 0)
            html = ASPx.HtmlEditorClasses.Commands.PasteFromWord.RestoreBrokenListsHtmlArray(html, brokenListHtmlArray, doc);
        brokenListHtmlArray = html.match(/<p[^>]*class\s*=\s*[\"']*MsoNormal[\"']*[^>]*mso-list:\s*\w*\s*level[^>]*>[\s\S]*?<\/p>/gi);
        if(brokenListHtmlArray && brokenListHtmlArray.length > 0)
            html = ASPx.HtmlEditorClasses.Commands.PasteFromWord.RestoreBrokenListsHtmlArray(html, brokenListHtmlArray, doc);
        if(ASPx.Browser.IE) {
            brokenListHtmlArray = html.match(/<(?:(?:ol)|(?:ul))>\s*<font[^>]*><font[^>]*>/);
            if(brokenListHtmlArray && brokenListHtmlArray.length > 0) {
                html = html.replace(/(?:<font[^>]*>)*(?:<span[^>]*>)*\s*<p[^>]*>(<li>.*?<\/li>)\s*(?:<\/span>)*(?:<\/font>)*/gi,"$1");
                html = html.replace(/(?:<font[^>]*>)*(?:<span[^>]*>)*\s*(<(?:(?:ul)|(?:ol))>)/gi,"$1");
                html = html.replace(/(<(?:(?:ul)|(?:ol))>)\s*(?:<\/span>)*(?:<\/font>)*/gi,"$1");
                var tempDivElement = doc.createElement("DIV");
                tempDivElement.innerHTML = html;
                for(var i = 0, childElement; childElement = tempDivElement.childNodes[i]; i++) {
                    if(childElement.nodeName == "OL" || childElement.nodeName == "UL") {
                        var listItemsArray = ASPx.GetNodesByTagName(childElement, "LI");
                        for(var j = 0, listItem; listItem = listItemsArray[j]; j++) {
                            if(listItem.nextSibling && (listItem.nextSibling.nodeName == "UL" || listItem.nextSibling.nodeName == "OL"))
                                listItem.appendChild(listItem.nextSibling);
                        }
                    }
                }
                html = tempDivElement.innerHTML;
            }
        }
        var hasList = /<(ol|ul)/gi.test(html);
        var numberingListArray = html.match(/<ol/gi);
        if(numberingListArray && numberingListArray.length > 1 || hasList) {
            var tempElement = doc.createElement("DIV");
            tempElement.innerHTML = html;
            if(numberingListArray && numberingListArray.length > 1)
                ASPx.HtmlEditorClasses.Commands.Browser.InsertList.UpdateListNumbering(tempElement, function(el) { return el.innerHTML.replace(/[\s\S]*<[^>]*mso-list\s*(:|=)\s*['"]*([A-Za-z]*\d*)['"]*[\s\S]*/gi,'$2'); });
            ASPx.HtmlEditorClasses.Commands.Browser.InsertList.MergeLists(tempElement);
            html = tempElement.innerHTML;
        }
        html = html.replace(/<li[^>]*>/gi,"<li>");
	    return html;
    };
    ASPx.HtmlEditorClasses.Commands.PasteFromWord.RestoreBrokenListsHtmlArray = function(html, brokenListHtmlArray, doc) {
        var getIgnorElement = function(element) {
            var parentElement = element;
            while(element.firstChild.nodeType == 1)
                element = element.firstChild;
            return element == parentElement || element.nextSibling ? element : element.parentElement;
        }
        var convertTypeAttrToListStyleType = function(typeAttrValue) {
            if(/^(IX|IV|V?I{0,3})$/.test(typeAttrValue))
                return "upper-roman";
            else if(/^(ix|iv|v?i{0,3})$/.test(typeAttrValue))
                return "lower-roman";
            else if(/[0-9]/.test(typeAttrValue))
                return "decimal";
            else if(typeAttrValue == "o")
                return "circle";
            else  if(/[a-z]/.test(typeAttrValue))
                return "lower-alpha";
            else if(/[A-Z]/.test(typeAttrValue))
                return "upper-alpha";
            var typeAttrValue = escape(typeAttrValue);
            if(typeAttrValue == "%B7")
                return "disc";
            else if(typeAttrValue == "%A7")
                return "square";
        }
        for(var i = 0, brokenListHtml; brokenListHtml = brokenListHtmlArray[i]; i++) {
            var listItemsArray = [];
            var lastListNumber = -1;
            var parentItemIndex = -1;
            var listHtml = "";
            var startAttrValue = null;
            var attributes = [];
            var brokenListItemHtmlArray = brokenListHtml.match(/<p[\s\S]*?>[\s\S]*?<\/p>/gi);
            for(var j = 0, brokenListItemHtml; brokenListItemHtml = brokenListItemHtmlArray[j]; j++) {
                if(!/<p[^>]*mso-list:[^>]*>/.test(brokenListItemHtml))
                    continue;
                var msoListAttr = brokenListItemHtml.match(/mso-list:\s*\w*\s*level[^ ]/gi);
                if(!msoListAttr || msoListAttr.length == 0) {
                    if(listItemsArray.length > 0) {
                        listHtml += ASPx.HtmlEditorClasses.Commands.PasteFromWord.CreateListByListItems(listItemsArray, startAttrValue, attributes).outerHTML;
                        attributes = [];
                        listItemsArray = [];
                    }
                    listHtml += brokenListItemHtml;
                    continue;
                }
                var currentListNumber = parseInt(msoListAttr[0].replace(/mso-list:\s*[A-Za-z]*(\d*)[\s\S]*/gi,'$1'));
                var currentLevel = parseInt(msoListAttr[0].replace(/mso-list:\s*\w*\s*level/gi,'')) - 1;
                var tempElement = doc.createElement("DIV");
                tempElement.innerHTML = brokenListItemHtml;
                tempElement = tempElement.firstChild;
                var listItemElement = doc.createElement("LI");
                ASPx.Attr.SetAttribute(listItemElement, "mso-list", msoListAttr[0].replace(/mso-list:\s*([A-Za-z]*\d*)[\s\S]*/gi,'$1'));

                var ignorElement = getIgnorElement(tempElement);
                var textContent = ignorElement != tempElement ? ASPx.Str.Trim(ASPx.GetInnerText(ignorElement)) : ignorElement.firstChild.nodeValue;
                var listStyleTypeValue = convertTypeAttrToListStyleType(textContent.indexOf(".") > -1 ? textContent.substring(0, textContent.length - 1) : textContent);
                if(ignorElement != tempElement)
                    ignorElement.parentNode.removeChild(ignorElement);
                else if(tempElement.childNodes.length > 2) {
                    tempElement.removeChild(tempElement.firstChild);
                    tempElement.removeChild(tempElement.firstChild);
                }
                listItemElement.innerHTML = tempElement.innerHTML;
                if(attributes.length == 0 && /<[^>]*dir\s*=\s*['"]RTL['"][^>]*>/gi.test(tempElement.outerHTML))
                    attributes.push({ name: "dir", value: "RTL" });

                var parentItemIndex = -1;
                var isNewList = listItemsArray.length == 0 || currentListNumber != lastListNumber && currentLevel == 0;
                if(isNewList || listItemsArray[listItemsArray.length - 1].level < currentLevel)
                    parentItemIndex = isNewList ? -1 : listItemsArray.length - 1;
                else if(currentLevel == 0)
                    listStyleTypeValue = listItemsArray[0].listStyleTypeValue;
                else {
                    for(var k = j - 1; listItem = listItemsArray[k]; k--) {
                        if(listItem.level <= currentLevel) {
                            listStyleTypeValue = listItemsArray[k].listStyleTypeValue;
                            parentItemIndex = listItemsArray[k].parentItemIndex;
                        }
                    }
                }
                if(listItemsArray.length > 0 && isNewList) {
                    listHtml += ASPx.HtmlEditorClasses.Commands.PasteFromWord.CreateListByListItems(listItemsArray, startAttrValue, attributes).outerHTML;
                    attributes = [];
                    listItemsArray = [];
                }
                lastListNumber = currentListNumber;
                listItemsArray.push({ 'level': currentLevel, 'node' : listItemElement, 'parentItemIndex' : parentItemIndex, 'parentListType' : /square|disc|circle/.test(listStyleTypeValue) ? "UL" : "OL", 'listStyleTypeValue' : listStyleTypeValue});
            }
            if(listItemsArray.length > 0)
                listHtml += ASPx.HtmlEditorClasses.Commands.PasteFromWord.CreateListByListItems(listItemsArray, startAttrValue, attributes).outerHTML;
            if(listHtml)
                html = html.replace(brokenListHtml, listHtml);
        }
        return html;
    };
    ASPx.HtmlEditorClasses.Commands.PasteFromWord.CreateListByListItems = function(listItemsArray, startAttrValue, attributes) {
        var listElement = ASPx.HtmlEditorClasses.Commands.Browser.Indent.GenerateListByItemArray(listItemsArray, null, startAttrValue);
        for(var i = 0, attr; attr = attributes[i]; i++)
            ASPx.Attr.SetAttribute(listElement, attr.name, attr.value);
        return listElement;
    };
    ASPx.HtmlEditorClasses.Commands.PasteFromWord.MergerFontFamilyAttributes = function(html) {
        var array = html.match(/<[^>]*style\s*=\s*[^>]*>/gi);
        if(array && array.length > 0) {
            for(var i = 0, elementHtml; elementHtml = array[i]; i++) {
                var fontFamilyArray = elementHtml.match(/\s*font-family\s*:\s*([^;]*)(([\"']>)|;| )/gi);
                if(fontFamilyArray && fontFamilyArray.length > 1) {
                    var commonValue = "";
                    for(var j = 0, fontFamily; fontFamily = fontFamilyArray[j]; j++) {
                        fontFamily = fontFamily.replace(/[\"']>$/i, "");
                        commonValue += commonValue ? "," : "";
                        commonValue += fontFamily.replace(/font-family\s*:\s*([^;]*[\"']*)[; ]*/gi, "$1");
                    }
                    var newElementHtml = elementHtml.replace(fontFamilyArray[0], "font-family: " + commonValue + ";");
                    html = html.replace(elementHtml, newElementHtml);
                    for(var j = 1, fontFamily; fontFamily = fontFamilyArray[j]; j++) {
                        fontFamily = fontFamily.replace(/[\"']>$/i, "");
                        var replacement = newElementHtml.replace(fontFamily, "");
                        html = html.replace(newElementHtml, replacement);
                        newElementHtml = replacement;
                    }
                }
            }
        }
        return html;
    };
})();