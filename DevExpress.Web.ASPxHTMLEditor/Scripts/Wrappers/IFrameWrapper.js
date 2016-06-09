(function () {
    var RegExprIframeHelper = ASPx.CreateClass(null, {
        constructor: function (owner) {
            this.wrapper = owner;
        },
        getDocumentElementAttrs: function (htmlOpenTag) {
            var attrsPairs = htmlOpenTag.match(/[\s\S]*?=(['"])[\s\S]*?\1/g);
            var attrs = [];

            if (attrsPairs) {
                for (var i = 0; i < attrsPairs.length; i++) {
                    var parts = attrsPairs[i].match(/([\S]*?)\s*=\s*(["'])(.*?)\2/);
                    attrs.push({
                        'name': parts[1],
                        'value': parts[3]
                    });
                }
            }
            return attrs;
        },
        getHtmlOpenTag: function (html) {
            var openTag = html.match(/<html(\s*\w+?=(["'])(.*?)\2)*>/);
            return openTag ? openTag[0] : "";
        },
        appendDocumentElementAttrs: function (html, doc) {
            if (!this.getHtmlOpenTag(html))
                return;

            var doc = doc || this.wrapper.getDocument();
            var htmlOpenTag = this.getHtmlOpenTag(html);
            var attrs = this.getDocumentElementAttrs(htmlOpenTag);
            var htmlTag = doc.documentElement;

            for (var i = 0; i < htmlTag.attributes.length; i++) {
                htmlTag.removeAttribute(htmlTag.attributes[i].name);
            }
            for (var i = 0; i < attrs.length; i++) {
                htmlTag.setAttribute(attrs[i].name, attrs[i].value);
            }
        }
    });

    var IframeStateController = ASPx.CreateClass(null, {
        constructor: function (owner) {
            this.wrapper = owner;
            this.ClearState();
        },
        ClearState: function () {
            this.state = {
                headChildNodes: [], //{outerHTML: , savedstyleSheets} //ie8 cloneNode errors
                bodyAttributes: [],
                bodyClassList: []
            };
            this.collistionInfo = {
                bodyAttributes: []
            };
        },
        SaveState: function () {
            if (!(this.wrapper.getDocument() && this.wrapper.getBody() && this.wrapper.getHead()))
                return;
            this.ClearState();
            this.saveHeadChildNodes();
            this.saveBodyAttributes();
            this.saveBodyClassList();
        },
        saveHeadChildNodes: function () {
            if (!this.wrapper.getHead())
                return;
            var childNodes = this.wrapper.getHead().childNodes;
            for (var i = 0, node; node = childNodes[i]; i++) {
                if (node.nodeName != "TITLE") {
                    this.state.headChildNodes.push({
                        'nodeOuterHTML': node.outerHTML,
                        'savedStyleSheets': function (node) {
                            if (node.nodeName == "STYLE" && node.sheet && node.sheet.cssRules.length > 0)
                                return node.sheet.cssRules;
                            if (ASPx.Browser.IE && ASPx.Browser.Version < 9) {
                                if (node.nodeName == "STYLE" && node.styleSheet && node.styleSheet.rules.length > 0) {
                                    return node.styleSheet.cssText;
                                }
                            }
                            return;
                        }.call(this, node)
                    });
                }
            }
        },
        saveBodyAttributes: function () {
            if (!this.wrapper.getBody())
                return;
            var attributes = this.wrapper.getBody().attributes;
            for (var i = 0, attr; attr = attributes[i]; i++) {
                if (attr.name != "class") {
                    this.state.bodyAttributes.push({
                        "name": attr.name,
                        "value": attr.value
                    });
                }
            }
        },
        saveBodyClassList: function () {
            if (!this.wrapper.getBody())
                return;
            var classes = this.wrapper.getBody().className.split(" ");
            for (var i = 0, className; className = classes[i]; i++) {
                this.state.bodyClassList.push(className);
            }
        },
        SaveCollisions: function(html) {
            if (!this.wrapper.getBody())
                return;

            var body = this.wrapper.getBody();
            this.collistionInfo.bodyAttributes = [];

            for (var i = 0, attr; attr = this.state.bodyAttributes[i]; i++) {
                if (ASPx.Attr.IsExistsAttribute(body, attr.name))
                    this.collistionInfo.bodyAttributes.push({
                        'name': attr.name,
                        'value': body.getAttribute(attr.name)
                    });
            }
            if (ASPx.Browser.IE) {
                var httpEqiuvMetaTags = ASPx.HtmlEditorClasses.getMetaHttpEquivTagsRegExp(html);
                if (httpEqiuvMetaTags.length >= 1) {
                    this.collistionInfo.userHttpEqiuvMetaTag = httpEqiuvMetaTags[0];
                }
                else {
                    this.collistionInfo.userHttpEqiuvMetaTag = "\n";
                }
            }
        },
        AddCollisionsInfo: function (doc) {
            var doc = doc || this.getDocument();
            var body = doc.body || this.getBody();

            for (var i = 0, attr; attr = this.collistionInfo.bodyAttributes[i]; i++) {
                body.setAttribute(attr.name, attr.value);
            }
        },
        removeIframeHeadChildNodes: function (doc) {
            var doc = doc || this.getDocument();
            var head = doc.getElementsByTagName('head')[0];

            var headDomNodes = head.childNodes;
            var domNode;
            for (var i = 0, node; node = this.state.headChildNodes[i]; i++) {
                var j = 0;
                while (headDomNodes.length > 0 && j < headDomNodes.length) {
                    domNode = headDomNodes[j];
                    var oldIEStyleNode = false;
                    if (ASPx.Browser.IE && ASPx.Browser.Version < 9) {
                        if (domNode.styleSheet && domNode.styleSheet.cssText == node.savedStyleSheets) {
                            oldIEStyleNode = true;
                        }
                    }
                    if (domNode.outerHTML == node.nodeOuterHTML || oldIEStyleNode) {
                        head.removeChild(domNode);
                        break;
                    }
                    j++;
                }
            }
        },
        removeIframeBodyAttributes: function (doc) {
            var doc = doc || this.getDocument();
            var body = doc.body || this.getBody();

            for (var i = 0, attr; attr = this.state.bodyAttributes[i]; i++) {
                ASPx.Attr.RemoveAttribute(body, attr.name);
            }
        },
        removeIframeBodyClasses: function (doc) {
            var doc = doc || this.getDocument();
            var body = doc.body || this.getBody();

            for (var i = 0, className; className = this.state.bodyClassList[i]; i++) {
                ASPx.RemoveClassNameFromElement(body, className);
            }
            if (body.className === "") {
                ASPx.Attr.RemoveAttribute(body, "class");
            }
        },
        RemoveIframeInfo: function (doc) {
            this.removeIframeHeadChildNodes(doc);
            this.removeIframeBodyAttributes(doc);
            this.removeIframeBodyClasses(doc);
        },
        RemoveBrowserInfo: function(bufDocHtml) {
            if (ASPx.Browser.IE) {
                bufDocHtml = ASPx.HtmlEditorClasses.replaceMetaHttpEquivIE(bufDocHtml, this.collistionInfo.userHttpEqiuvMetaTag);
            }
            return bufDocHtml;
        },
        RestoreState: function () {
            if (ASPx.Browser.IE && ASPx.Browser.Version < 9) {
                this.restoreHeadIE();
            }
            else {
                this.restoreHeadChildNodes();
            }
            this.restoreBodyAttributes();
            this.restoreBodyClasses();
        },
        restoreHeadIE: function () {
            var html = this.wrapper.getDocument().documentElement.outerHTML;
            var savedHeadhtml = this.wrapper.getHead().innerHTML, headhtml = savedHeadhtml;
            for (var i = 0, node; node = this.state.headChildNodes[i]; i++) {
                headhtml += node.nodeOuterHTML;
            }
            html = html.replace(savedHeadhtml, headhtml);
            this.wrapper.getDocument().open();
            this.wrapper.getDocument().write(html);
            this.wrapper.getDocument().close();
        },
        restoreHeadChildNodes: function () {
            var head = this.wrapper.getHead();
            for (var i = 0, node; node = this.state.headChildNodes[i]; i++) {
                if (node.savedStyleSheets) {
                    var style = this.wrapper.getDocument().createElement('style');
                    head.appendChild(style);
                    this.restoreCssRules(style, node.savedStyleSheets);
                }
                else {
                    if (node.nodeOuterHTML) {
                        if (ASPx.Browser.IE && ASPx.Browser.Version < 10) {
                            var temp = document.createElement('div');
                            temp.innerHTML = node.nodeOuterHTML;
                            head.appendChild(temp.firstChild); //head.innerHTML readOnly
                        }
                        else
                            head.innerHTML += node.nodeOuterHTML;
                    }
                }
            }
        },
        restoreCssRules: function (style, savedStyleSheets) {
            if (ASPx.Browser.IE && ASPx.Browser.Version < 9) {
                style.styleSheet.cssText = savedStyleSheets;
            }
            else {
                for (var i = 0, rule; rule = savedStyleSheets[i]; i++) {
                    ASPx.AddStyleSheetRule(style.sheet, rule.selectorText, rule.style.cssText);
                }
            }
        },
        restoreBodyAttributes: function () {
            var body = this.wrapper.getBody();
            for (var i = 0, attr; attr = this.state.bodyAttributes[i]; i++) {
                ASPx.Attr.SetAttribute(body, attr.name, attr.value);
            }
        },
        restoreBodyClasses: function () {
            var body = this.wrapper.getBody();
            for (var i = 0, className; className = this.state.bodyClassList[i]; i++) {
                if (!ASPx.ElementHasCssClass(body, className))
                    ASPx.AddClassNameToElement(body, className);
            }
        }
    });



    ASPx.HtmlEditorClasses.Wrappers.IFrameWrapper = ASPx.CreateClass(ASPx.HtmlEditorClasses.Wrappers.BaseWrapper, {
        constructor: function(id, settings, callbacks) {
            this.constructor.prototype.constructor.call(this, id, settings, callbacks);
            this.iframeStateController = new IframeStateController(this);
            this.regExprIframeHelper = new RegExprIframeHelper(this);
            this.bodySpellchecking = null;
        },
        initializeIfNeeded: function() {
            if (this.getDocument().readyState === "loading") {
                this.initialize(false);
            }
        },
        initialize: function (inlineInit, bodyContentHtml) {
            this.initIFrame(bodyContentHtml || "");
            this.initIFrameStyle();
            this.iframeStateController.SaveState();
        },
        
        getWindow: function() {
            return ASPx.IFrameHelper.GetWindow(this.id);
        },
        getDocument: function() {
            return ASPx.IFrameHelper.GetDocument(this.id);
        },
        getSearchContainer: function() {
            return this.getBody();    
        },
        getBody: function() {
            return ASPx.IFrameHelper.GetDocumentBody(this.id);
        },
        getHead: function () {
            return ASPx.IFrameHelper.GetDocumentHead(this.id);
        },
        getElement: function() {
            return ASPx.IFrameHelper.GetElement(this.id);
        },
        getDOMElement: function () {
            return this.getBody();
        },
        setWholeHtml: function (html) {
            var doc = this.getDocument();
            var savedBodyHtml;
            var bufDoc = ASPx.HtmlEditorClasses.createHtmlDocument();

            ASPx.HtmlEditorClasses.setDocumentInnerHTML(bufDoc, html);
            savedBodyHtml = bufDoc.body.innerHTML;
            bufDoc.body.innerHTML = "";
            ASPx.HtmlEditorClasses.setDocumentInnerHTML(this.getDocument(), bufDoc.documentElement.innerHTML);

            this.regExprIframeHelper.appendDocumentElementAttrs(html);
            this.iframeStateController.SaveCollisions(html);
            this.iframeStateController.RestoreState();
            this.setBodyHtmlWithoutRattle(savedBodyHtml);
        },
        setBodyHtmlWithoutRattle: function (savedBodyHtml) {
            this.getBody().style.opacity = 0;
            this.insertHtml(this.processHtmlBodyBeforeInsert(savedBodyHtml));
            setTimeout(function () {
                ASPx.Attr.RemoveStyleAttribute(this.getBody(), "opacity");
            }.aspxBind(this), 0);
        },
        getWholeHtml: function () {
            var bufDoc = ASPx.HtmlEditorClasses.createHtmlDocument();
            var wholeHtml;

            ASPx.HtmlEditorClasses.setDocumentInnerHTML(bufDoc, this.getDocument().documentElement.innerHTML);
            this.regExprIframeHelper.appendDocumentElementAttrs(this.getDocument().documentElement.outerHTML, bufDoc);
            this.iframeStateController.RemoveIframeInfo(bufDoc);
            this.iframeStateController.AddCollisionsInfo(bufDoc);

            bufDoc.body.innerHTML = this.getProcessedHtml(this.getBody().innerHTML);
            
            wholeHtml = bufDoc.documentElement.outerHTML;
            var innerHtmlWithIndents = "\n" + bufDoc.documentElement.innerHTML + "\n";
            wholeHtml = wholeHtml.replace(bufDoc.documentElement.innerHTML, innerHtmlWithIndents);
            wholeHtml = this.iframeStateController.RemoveBrowserInfo(wholeHtml);

            if (ASPx.Browser.IE && ASPx.Browser.Version < 11)
                wholeHtml = wholeHtml.replace(/&nbsp;/ig, "\n \n");

            return wholeHtml;
        },
        initIFrame: function (bodyContentHtml) {
            var doc = this.getDocument();
            var element = this.getElement();

            if (doc != null && element != null) {
                this.initIFrameDocument(doc, bodyContentHtml);
                this.getElement().style.height = "100%";
            }
        },
        initIFrameDocumentBody: function (doc) {
            var elementClassList = this.getElement().className.split(" ");

            if (ASPx.Browser.IE)
                doc.body.style.paddingBottom = 2;
            if (ASPx.Browser.Opera)
                doc.body.style.height = "100%";
            for (var i = 0; i < elementClassList.length; i++) {
                if (elementClassList[i])
                    ASPx.AddClassNameToElement(doc.body, elementClassList[i]);
            }
            if (this.settings.rtl)
                doc.body.dir = "rtl";
            if (this.settings.docStyleCssText)
                doc.body.style.cssText = this.settings.docStyleCssText;
            doc.body.style.borderWidth = 0;
        },
        initIFrameStyleSheets: function (doc) {
            if (!this.settings.cssFileLinkArray)
                return;
            var linkArray = this.settings.cssFileLinkArray;

            for (var i = 0; i < linkArray.length; i++) {
                ASPx.AddStyleSheetLinkToDocument(doc, linkArray[i]);
            }
        },
        initIFrameDocument: function (doc, bodyContentHtml) {
            doc.open();
            doc.write(this.getIFrameDocumentHtml(bodyContentHtml));
            doc.close();
            this.initIFrameDocumentBody(doc);
            this.initIFrameStyleSheets(doc);
        },
        initIFrameStyle: function () {
            if (this.getViewAreaStyleCssText() == "")
                return;

            var viewAreaStyleCssText = this.getViewAreaStyleCssText();
            var documentStyleSheet = ASPx.CreateStyleSheetInDocument(this.getDocument());

            ASPx.AddStyleSheetRule(documentStyleSheet,
                        "body." + this.getDocumentClassName(), viewAreaStyleCssText);
            ASPx.AddClassNameToElement(this.getBody(), this.getDocumentClassName());
        },
        isIFrameLoaded: function() {
            var doc = this.getDocument();
            return !!doc && !!doc.body.className;
        },
        isExistsWindow: function() {
            try {
                return !!this.getWindow();
            }
            catch (e) {
                return false;
            }
        },
        getIFrameDocumentHtml: function(bodyContentHtml) {
            var html = "";
            //html += "<!DOCTYPE html>";
            html += "<html>"
            html += "<head>";
            if(ASPx.Browser.IE)
                html += "  <scr" + "ipt type=\"text/javascript\">" +
                    "    window.document.onkeydown = function() {" +
                    "       if(event.keyCode == 121) {" +
                    "           window.parent.FocusActiveEditorToolbar();" +
                    "           event.keyCode = 0;" +
                    "           return false;" +
                    "       }" +
                    "   }" +
                    "  </scr" + "ipt>";
            html += "<style></style><title>title</title></head><body>";
            if(bodyContentHtml)
                html += bodyContentHtml;
            html += "</body>";
            html += "</html>";
            return html;
        },
        getDocumentStyleCssText: function() {
            return "";
        },
        getDocumentClassName: function() {
            return "";
        },
        setInnerHtmlToBody: function(html) {
            var body = this.getBody();
            this.setInnerHtml(body, html);
            this.processingEmptyElements(body);
            if(ASPx.Browser.IE) // hack. InIE ifset markup with font element, then attribute size=+0 appears
                this.cleanWrongSizeAttribute(body);
        },
        getProcessedHtml: function(html) {
            html = ASPx.HtmlEditorClasses.HtmlProcessor.preserveAttribute(html, "autoplay");
            html = this.convertToEmptyHtml(this.depreserveTagsByName("noscript", html), this.getBody());
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion > 10)
                html = this.clearFakeBrElements(this.getBody(), html);
            if(this.settings.allowScripts && this.canDepreserveScriptTags()) {
                html = this.depreserveTagsByName("script", html);
                html = this.depreserveAttribute(html, ASPx.HtmlEditorClasses.JSEventAttributeNameRegExp);
            }
            html = ASPx.HtmlEditorClasses.Wrappers.BaseWrapper.prototype.getProcessedHtml.call(this, html);
            html = this.depreserveAttribute(html, "autoplay");
            return html;
        },
        canDepreserveScriptTags: function() {
            return true;
        },
        removeDataLinkEscapeSymbol: function (html) {
            var DATA_LINK_ESCAPE_CODE = 10;
            if (html && html.charCodeAt(html.length - 1) == DATA_LINK_ESCAPE_CODE)
                html = html.substr(0, html.length - 1);
            return html;
        },
        processHtmlBodyBeforeInsert: function(html) {
            html = ASPx.HtmlEditorClasses.Wrappers.BaseWrapper.prototype.processHtmlBodyBeforeInsert.call(this, html);
            html = this.removeDataLinkEscapeSymbol(html);
            html = ASPx.HtmlEditorClasses.HtmlProcessor.preserveAttribute(html, "autoplay");
            html = this.processHtmlByBrowser(html);
            html = this.preserveTagsByName("noscript", html);
            if (!this.settings.allowScripts)
                html = this.cleanHtmlScripts(html);
            else {
                var masked = this.maskComments(html); // B221221
                if(this.canDepreserveScriptTags()) {
                    html = this.preserveTagsByName("script", masked.html);
                    html = ASPx.HtmlEditorClasses.HtmlProcessor.preserveAttribute(html, ASPx.HtmlEditorClasses.JSEventAttributeNameRegExp);
                }
                html = this.unMaskComments(html, masked.comments);
            }
            return html;
        },
        cleanWrongSizeAttribute: function() {
            var fonts = ASPx.GetNodesByTagName(this.getBody(), "font");
            for(var i = 0; i < fonts.length; i++)
                if(fonts[i].size == "+0")
                ASPx.Attr.RemoveAttribute(fonts[i], "size");
        },
        setSpellCheckAttributeValue: function (value) {
            this.bodySpellchecking = value;
            this.getBody().spellcheck = value;
            if (this.iframeStateController) {
                this.iframeStateController.state.bodyAttributes.push({ 'name': 'spellcheck', 'value': value });
            }
        },
        restoreSpellChecking: function () {
            this.getBody().spellcheck = this.bodySpellchecking;
        },
        turnOffSpellChecking: function () {
            this.getBody().spellcheck = false;
        }
    });
})();