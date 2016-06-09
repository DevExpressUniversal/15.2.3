/// <reference path="PrintHelper.js"/>
/// <reference path="Searcher.js"/>

(function(window) {
    var ASPxClientReportViewer = ASPx.CreateClass(ASPxClientControl, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.searchWindow = null;
            this.textRange = null;
            this.pageCount = 0;
            this.exportWindow = null;
            this.pageByPage = true;
            this.printUsingAdobePlugIn = true;
            this.searchPopupControl = null;
            this.useIFrame = true;
            this.loadPage = true;
            this.DefaultSaveFormat = "pdf";

            var me = this;
            ASPx.Evt.AttachEventToElement(window, "beforeunload", function() { me.onunload(); });

            this.printHelper = new ASPx.dx_PrintHelper();
            this.PageLoad = new ASPxClientEvent();

            this.postBackStarted = false;
            this.ChangedParameterKeyName = "$changed";
            this.submitParametersComplete = new ASPxClientEvent();
            this.refreshRising = new ASPxClientEvent();
        },
        InlineInitialize: function() {
            ASPxClientControl.prototype.InlineInitialize.call(this);

            this.formHelper = this.createFormHelper();
        },
        AfterInitialize: function() {
            if(this.loadPage && this.useIFrame)
                this.LoadInitialPage();
            this.constructor.prototype.AfterInitialize.call(this);
            if(!this.loadPage || !this.useIFrame) {
                this.RaisePageLoadEvent();
            }
            this.subscribeToAspForm();
        },
        subscribeToAspForm: function() {
            if(typeof (theForm) !== 'undefined') {
                var obj = this;
                this.addBeforeFormSubmit(theForm, function() {
                    obj.postBackStarted = true;
                });
            }
        },
        addBeforeFormSubmit: function(form, beforeSubmitFunction) {
            var original = form.submit;
            form.submit = function() {
                beforeSubmitFunction();
                var callee = arguments.callee;
                this.submit = original;
                var submitResult = this.submit();
                this.submit = callee;
                return submitResult;
            };
        },
        LoadInitialPage: function() {
            this.gotoPageInternal(this.getCurrentPageIndex());
        },

        // virtual
        createFormHelper: function() {
            return ASPxClientReportViewer.__useMobileSpecificExport
                ? new dx_FormHelperMobile(this)
                : new dx_FormHelper();
        },
        onunload: function() {
            this.closeWindow(this.searchWindow);
        },
        closeWindow: function(win) {
            if(win != null && !win.closed) {
                win.close();
            }
        },
        OnCallbackInternal: function(result, isError) {
            if(result == "" || isError) {
                this.ShowCallbackError(result == "" ? "unknown report error" : result);
                return;
            }
            var callbackType = this.getResultParam(result);
            if(callbackType != null) {
                var hiddenFieldAssignmentScript = this.getResultParam(callbackType.remainder);
                var callbackScript = this.getResultParam(hiddenFieldAssignmentScript.remainder);
                var resultRemainder = callbackScript.remainder;
                eval(this.protectEscapes(hiddenFieldAssignmentScript.param));
                switch(callbackType.param) {
                    case "submitParameters":
                        this.clearParametersChanged();
                        this.submitParametersComplete.FireEvent(this);
                    case "page":
                        this.OnPageChanged(resultRemainder);
                        break;
                    case "print":
                        this.OnPrint(resultRemainder);
                        break;
                    case "search":
                        this.OnSearch(resultRemainder);
                        break;
                    case "searchControl":
                        this.OnSearchControl(resultRemainder);
                        break;
                    case "saveToWindow":
                        this.OnSaveToWindow(resultRemainder);
                        break;
                    case "saveToDisk":
                        this.OnSaveToDisk(resultRemainder);
                        break;
                    case "bookmark":
                        this.OnBookmark(resultRemainder);
                        break;
                }
                eval(this.protectEscapes(callbackScript.param));
            }
        },
        DoEndCallback: function() {
            this.constructor.prototype.DoEndCallback.call(this);
            if(this.pendingBookmark) {
                this.GotoBookmarkCore(this.pendingBookmark.pageIndex, this.pendingBookmark.bookmarkPath);
                delete this.pendingBookmark;
            }
        },
        ShowCallbackError: function(result) {
            alert(result);
        },
        protectEscapes: function (str) {
            return decodeURIComponent(str.replace(/\\/g, "\\\\").replace(/\\\\"/g, '\\\\\\"'));
        },
        clearParametersChanged: function() {
            var parameters = this.stateObject.parameters;
            delete parameters[this.ChangedParameterKeyName];
        },
        alert: function(x) {
            alert(x);
        },
        gotoPageInternal: function(pageIndex) {
            this.execContentChangingCallback(pageIndex, "page");
        },
        getHiddenFields: function() {
            return [this.GetStateHiddenField()];
        },
        getResultParam: function(result) {
            var pos = result.indexOf("|");
            if(pos > -1) {
                var len = parseInt(result.substr(0, pos), 10);
                return { param: result.substr(pos + 1, len), remainder: result.substr(len + pos + 1) };
            }
            return null;
        },
        execExport: function(exportKind, params, win) {
            this.execPostbackInWindow((win != null ? win : this.getFrameForExport()), exportKind, params);
        },
        execPrintPdf: function(pageIndex) {
            if(this.printHelper.pdfExists())
                this.execExport("saveToDisk", { format: "pdf", showPrintDialog: "true", idx: pageIndex });
            else
                this.execCallbackPrint(pageIndex);
        },
        execCallbackPrint: function(pageIndex) {
            this.execCallback("print", { idx: pageIndex });
        },
        execCallback: function(command, params) {
            if(!this.postBackStarted && !this.InCallback()) {
                this.ShowLoadingElements();
                this.CreateCallback(this.createEventArgument(command, params));
            }
        },
        execPostbackInWindow: function(win, command, params) {
            ASPx.RaisePostHandlerOnPost();
            this.formHelper.sendPostbackInWindow(win, this.uniqueID, this.createEventArgument(command, params));
        },
        createEventArgument: function(command, params) {
            var formatter = new dx_ParamFormatter();
            formatter.params = params;
            return command + "=" + formatter.getValue();
        },
        execContentChangingCallback: function(pageIndex, command, params) {
            this.setCurrentPageIndex(pageIndex);
            this.execCallback(command, params);
        },
        ShowLoadingPanel: function() {
            this.CreateLoadingPanelWithAbsolutePosition(this.GetMainElement());
        },
        OnCallback: function(result) {
            this.OnCallbackInternal(result, false);
        },
        OnCallbackError: function(result, data) {
            this.OnCallbackInternal(result, true);
        },
        OnPageChanged: function(result) {
            var element = this.getContentElement();
            if(element != null) {
                ASPx.SetInnerHtml(element, result);
            }
            this.assignPageBackColor();
        },
        OnSearch: function(result) {
            this.OnPageChanged(result);
        },
        OnSearchControl: function(result) {
            var div = document.createElement("DIV");
            var element = this.getOuterContentElement();
            element.parentNode.appendChild(div);
            div.outerHTML = result;
            /*this.setOuterHTML(div, result);*/
        },
        /*    setOuterHTML: function(someElement, txt) {
        if(someElement.outerHTML) {
        someElement.outerHTML = txt;
        }
        else {
        var range = document.createRange();
        range.setStartBefore(someElement);
        var docFrag = range.createContextualFragment(txt);
        someElement.parentNode.replaceChild(docFrag, someElement);
        }
        },
        */
        OnPrint: function(result) {
            this.printHelper.print(result, this.getContentElement().className);
        },
        OnSaveToWindow: function(result) {
            if(this.exportWindow != null && !this.exportWindow.closed) {
                this.setWindowLocation(this.exportWindow, result);
            }
        },
        OnSaveToDisk: function(result) {
            this.printHelper.getFrame().location = result;
        },
        OnBookmark: function(result) {
            this.OnPageChanged(result);
        },
        showSearchWindow: function() {
            if(this.searchPopupControl == null) {
                this.execCallback("searchControl", {});
                return;
            }
            this.closeTextRange();
            this.searchPopupControl.ShowAndFocus();
        },
        findText: function(s) {
            var f = new dx_ParamFormatter();
            f.parse(s);
            this.findTextCore(f.params, true);
        },
        findTextCore: function(params, isServerCallback) {
            var isTrue = function(val) {
                return val == true || val === "true";
            }
            var text = params["txt"];
            var mword = isTrue(params["word"]);
            var mcase = isTrue(params["case"]);
            var up = isTrue(params["up"]);
            if(text === "")
                return;
            var range = this.getTextRange(up);
            if(!range.findText(text, mword, mcase, up, isServerCallback) && !isServerCallback)
                this.execCallback("search", params);
        },
        getTextRange: function(up) {
            if(!ASPx.IsExists(this.textRange)) {
                var f = this.getContentElement();
                this.textRange = ASPx.Browser.IE ? new ASPx.dx_TextRange(f, up) : new ASPx.dx_Range(f, up, !this.useIFrame);
            }
            return this.textRange;
        },
        closeTextRange: function() {
            if(ASPx.IsExists(this.textRange))
                this.textRange.empty();
            this.textRange = null;
        },
        getContentElementDiv: function() {
            return this.getElement("Div");
        },
        getContentElementIFrame: function() {
            var frameDocument = this.getContentDocument(this.getContentFrameElement());
            var divElement = frameDocument.getElementById("report_div");
            if(!ASPx.IsExists(divElement)) {
                frameDocument.open("text/html", "replace");
                //frameDocument.write('<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html><body style="margin:0px;overflow:hidden;"><div id="report_div" style="width:100%;height:100%;overflow:hidden;"><div></body></html>');
                frameDocument.write('<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"><html><body style="margin:0px;"><div id="report_div" style="width:100%;height:100%;overflow:hidden;"><div></body></html>');
                frameDocument.close();
                divElement = frameDocument.getElementById("report_div");
                var paddingString = this.padding.join(' ').replace(/^\s+|\s+$/g, ''); // IE8
                divElement.style.padding = paddingString;

                var frameWindow = this.getContentWindow(this.contentFrame);
                if(!frameWindow.ASPx)
                    frameWindow.ASPx = {};
                frameWindow.ASPx.xr_NavigateUrl = function(url, target) {
                    window.ASPx.xr_NavigateUrl(url, target);
                }
                frameWindow.ASPx.RVGotoBM = function(name, pageIndex, bookmarkIndices) {
                    window.ASPx.RVGotoBM(name, pageIndex, bookmarkIndices);
                }
                frameWindow.ASPx.xr_NavigateDrillDown = function(viewerName, drillDownKey) {
                    window.ASPx.xr_NavigateDrillDown(viewerName, drillDownKey);
                };
                frameWindow.DXReportViewerWindow = window;
            }
            return divElement;
        },
        getContentFrameElement: function() {
            if(!ASPx.IsExists(this.contentFrame))
                this.createContent();
            return this.contentFrame;
        },
        createContent: function() {
            this.container = this.getElement("Div");

            this.contentFrame = document.createElement("iframe");
            this.contentFrame.id = this.getElementName("ContentFrame");
            this.contentFrame.name = this.contentFrame.id;
            this.contentFrame.frameBorder = 0;
            if(ASPx.Browser.IE)
                this.contentFrame.allowTransparency = true;
            this.container.appendChild(this.contentFrame);
            if(!this.autosize) {
                this.contentFrame.style.width = this.container.style.width;
                this.contentFrame.style.height = this.container.style.height;
            }
        },
        GetMainElement: function() {
            return this.getContentElementDiv();
        },
        getOuterContentElement: function() {
            if(!this.useIFrame)
                return this.getContentElementDiv();
            return this.getContentFrameElement();
        },
        getContentElement: function() {
            if(!this.useIFrame)
                return this.getContentElementDiv();
            return this.getContentElementIFrame();
        },
        getContentDocument: function(frameElement) {
            return frameElement.contentDocument || frameElement.contentWindow.document;
        },
        getContentWindow: function(frameElement) {
            return frameElement.contentWindow;
        },
        getElement: function(elementNameSuffix) {
            return ASPx.GetElementById(this.getElementName(elementNameSuffix));
        },
        getElementName: function(elementNameSuffix) {
            return this.name + "_" + elementNameSuffix;
        },
        setViewSize: function(w, h) {//TODO: have to call this after the OnPageChanged func
            var el = this.getContentElement();
            if(!ASPx.IsExists(el)) {
                return;
            }
            var width = 0,
                height = 0;
            if(!ASPx.IsExists(w) && !ASPx.IsExists(h)) {
                var children = el.childNodes;
                for(var i = 0; i < children.length; i++) {
                    var child = children[i];
                    if(child.tagName != "DIV" && child.tagName != "TABLE") {
                        continue;
                    }
                    width = Math.max(child.clientWidth, width);
                    height += child.clientHeight;
                }
                if(height === 0) { // T150337 - IE 10 fix
                    setTimeout(function() { this.setViewSize(w, h); }.aspxBind(this), 100);
                    return;
                }
            } else {
                width = w;
                height = h;
            }

            el.style.width = getValueWithPxUnitEx(width);
            el.style.height = getValueWithPxUnitEx(height);
            if(!this.useIFrame) {
                return;
            }
            var fel = this.getContentFrameElement();
            if(fel == null) {
                return;
            }
            if(this.autosize) {
                fel.style.width = fel.parentNode.style.width = getValueWithPxUnitEx(this.getFrameSize(el.clientWidth, width, this.padding[1], this.padding[3]));
                fel.style.height = fel.parentNode.style.height = getValueWithPxUnitEx(this.getFrameSize(el.clientHeight, height, this.padding[0], this.padding[2]));
            }
        },
        getFrameSize: function(clientSize, size, nearPadding, farPadding) {
            return clientSize > 0
                ? clientSize
                : getValueWithoutPxUnit(size) + getValueWithoutPxUnit(nearPadding) + getValueWithoutPxUnit(farPadding);
        },
        onPageLoad: function(pageCount) {
            this.textRange = null;
            if(ASPx.IsExists(this.bookmarkHighlighter))
                this.bookmarkHighlighter.Reset();
            this.pageCount = pageCount;
            if(this.isInitialized)
                this.RaisePageLoadEvent();
        },
        assignPageBackColor: function() {
            var contentElement = this.getContentElement();
            for(var child in contentElement.childNodes)
                if(contentElement.childNodes[child].className === 'page-background-color-holder') {
                    contentElement.style.backgroundColor = contentElement.childNodes[child].style.backgroundColor;
                    break;
                }
        },
        getCurrentPageIndex: function() {
            return this.stateObject.currentPageIndex;
        },
        setCurrentPageIndex: function(pageIndex) {
            this.stateObject.currentPageIndex = parseInt(pageIndex, 10);
        },
        setStateObjectKey: function(key, value) {
            this.stateObject[key] = value;
        },
        RaisePageLoadEvent: function() {
            var args = new ASPxClientReportViewerPageLoadEventArgs(this.getCurrentPageIndex(), this.pageCount);
            this.PageLoad.FireEvent(this, args);
        },
        GotoBookmark: function(pageIndex, bookmarkPath) {
            if(!this.pageByPage) return;
            if(this.InCallback())
                this.pendingBookmark = { pageIndex: pageIndex, bookmarkPath: bookmarkPath };
            else
                this.GotoBookmarkCore(pageIndex, bookmarkPath);
        },
        GotoBookmarkCore: function(pageIndex, bookmarkPath) {
            if(pageIndex != this.getCurrentPageIndex()) {
                this.execContentChangingCallback(pageIndex, "bookmark", { path: bookmarkPath });
            } else {
                this.HighlightBookmark(bookmarkPath);
            }
        },
        HighlightBookmark: function(bookmarkPath) {
            if(!ASPx.IsExists(this.bookmarkHighlighter))
                this.bookmarkHighlighter = new dx_BookmarkHighlighter(this.getContentElement(), this.name);
            this.bookmarkHighlighter.Highlight(this.getCurrentPageIndex(), bookmarkPath);
        },
        setWindowLocation: function(win, loc) {
            if(ASPx.Browser.IE) { //Bug B30005
                win.document.open("text/html", "replace");
                win.document.write("<frameset><frame src='" + loc + "'></frame></frameset>");
                win.document.close();
            } else
                win.location = loc;
        },
        getFrameForExport: function() {
            if(ASPx.Browser.Firefox) {
                var win = this.printHelper.getFrame();
                try {
                    var doc = win.document;
                } catch(e) {
                    return this.printHelper.getFrameRecreated();
                }
                return win;
            }
            return (ASPx.Browser.IE || ASPx.Browser.Chrome || ASPx.Browser.Safari) ? this.printHelper.getFrameRecreated() : this.printHelper.getFrame();
        },
        SubmitParameters: function(parameters) {
            this.stateObject.drillDown = {};
            this.submitParametersCore(0, parameters);
        },
        navigateDrillDown: function(drillDownKey) {
            var drillDown = this.stateObject.drillDown;
            if(drillDown.hasOwnProperty(drillDownKey)) {
                drillDown[drillDownKey] = !drillDown[drillDownKey];
            } else {
                drillDown[drillDownKey] = true;
            }
            this.submitParametersCore(this.getCurrentPageIndex());
        },
        submitParametersCore: function(pageIndex, parameters) {
            parameters = parameters || {};
            parameters[this.ChangedParameterKeyName] = true;
            for(var i in parameters) {
                if(!parameters.hasOwnProperty(i)) {
                    continue;
                }
                var value = parameters[i];
                this.stateObject.parameters[i] = value;
            }
            this.execContentChangingCallback(pageIndex, 'submitParameters');
        }
    });

    ASPxClientReportViewer.__useMobileSpecificExport = ASPx.Browser.MacOSMobilePlatform || ASPx.Browser.AndroidMobilePlatform;

    function getValueWithPxUnit(value) {
        return value.toString() + 'px';
    }

    function getValueWithPxUnitEx(value) {
        var str = value.toString();
        return (str.length < 3 || str.substr(str.length - 2) != 'px') ? getValueWithPxUnit(value) : str;
    }

    function getValueWithoutPxUnit(value) {
        if(value === '') {
            return 0;
        }
        var str = value.toString();
        if(str.length >= 3 && str.substr(str.length - 2) === 'px') {
            str = str.substr(0, str.length - 2);
        }
        return parseInt(str, 10);
    }

    function rvsdLoaded(reportViewerID, popupControlID, textEditID, buttonFindID, checkWordID, checkCaseID, radioUpID, radioDownID) {
        var controlCollection = ASPx.GetControlCollection();
        var popupControl = controlCollection.Get(popupControlID);
        popupControl.reportViewer = controlCollection.Get(reportViewerID);
        popupControl.buttonFind = controlCollection.Get(buttonFindID);
        popupControl.textEdit = controlCollection.Get(textEditID);
        popupControl.checkWord = controlCollection.Get(checkWordID);
        popupControl.checkCase = controlCollection.Get(checkCaseID);
        popupControl.radioUp = controlCollection.Get(radioUpID);
        popupControl.radioDown = controlCollection.Get(radioDownID);

        var textEdit = popupControl.textEdit;
        var inputElement = textEdit.GetInputElement();
        inputElement.onpropertychange = function() {
            if(popupControl.buttonFind)
                popupControl.buttonFind.SetEnabled(inputElement.value != "" && ASPx.Browser.IE);
        };
        if(popupControl.reportViewer) {
            popupControl.reportViewer.searchPopupControl = popupControl;
            if(ASPx.Browser.MacOSMobilePlatform && popupControl.reportViewer.searchPopupControl && popupControl.reportViewer.searchPopupControl.Closing) {
                popupControl.reportViewer.searchPopupControl.Closing.AddHandler(function() {
                    try {
                        if(this.textRange)
                            this.textRange.empty();
                    } catch(e) { }
                }, popupControl.reportViewer);
            }
        }

        popupControl.ShowAndFocus = function() {
            this.Show();
            this.buttonFind.Focus();
            this.textEdit.SelectAll();
        };
        popupControl.ShowAndFocus();
    }

    function rvsdFind(popupControlID) {
        var popupControl = ASPx.GetControlCollection().Get(popupControlID);
        if(popupControl) {
            var params = {
                "txt": popupControl.textEdit.GetText(),
                "case": popupControl.checkCase.GetChecked(),
                "word": popupControl.checkWord.GetChecked(),
                "up": popupControl.radioUp.GetChecked()
            };
            popupControl.reportViewer.findTextCore(params);
        }
    }

    function rvsdClose(popupControlID) {
        var popupControl = ASPx.GetControlCollection().Get(popupControlID);
        if(popupControl)
            popupControl.Hide();
    }

    function rvGotoBM(name, pageIndex, bookmarkIndices) {
        var reportViewer = ASPx.GetControlCollection().Get(name);
        if(reportViewer) {
            reportViewer.GotoBookmark(pageIndex, bookmarkIndices);
        }
    }

    /* class ParamFormatter */
    function dx_ParamFormatter() {
        this.params = [];
        this.addParam = function(name, val) {
            this.params[name] = val;
        }
        this.getValue = function() {
            var val = "";
            for(var i in this.params)
                val += this.format(i, this.params[i]);
            return val;
        }
        this.format = function(name, val) {
            if(!ASPx.IsExists(val)) {
                return name + ":;";
            }
            var value = val.toString().replace(/&/g, "&a")
                .replace(/:/g, "&c")
                .replace(/;/g, "&s");
            return name + ":" + value + ";";
        }
        this.parse = function(s) {
            var ss = s.split(";");
            for(var i = 0; i < ss.length; i++) {
                var sss = ss[i].split(":");
                if(sss.length === 2) {
                    var sss1 = sss[1].replace(/&c/g, ":")
                        .replace(/&s/g, ";")
                        .replace(/&a/g, "&");
                    this.addParam(sss[0], sss[1]);
                }
            }
        }
    }

    /* class BookmarkHighlighter */
    function dx_BookmarkHighlighter(contentElement, ownerName) {
        this.contentElement = contentElement;
        this.bookmarkElement = null;
        this.selectionTemplate = ASPx.GetElementById(ownerName + "_Bookmark");

        this.Highlight = function(pageIndex, bookmarkPath) {
            if(ASPx.IsExists(this.bookmarkElement)) {
                try {
                    this.contentElement.removeChild(this.bookmarkElement)
                } catch(e) { }
                this.bookmarkElement = null;
            }
            bookmarkPath = pageIndex.toString() + "_" + bookmarkPath;
            var bookmarkElements = this.getBookmarkElements(ASPx.GetNodesByTagName(this.contentElement, "A"), bookmarkPath);
            if(bookmarkElements.length === 0)
                return;
            var bounds = this.getBookmarkBounds(bookmarkElements);
            this.bookmarkElement = this.addBookmarkElement(bounds);
        }
        this.Reset = function() {
            this.bookmarkElement = null;
        }
        this.getBorderWidth = function() {
            return parseInt(this.selectionTemplate.style.borderWidth, 10);
        }
        this.getBookmarkElements = function(elements, name) {
            var bookmarkElements = [];
            for(var i = 0; i < elements.length; i++) {
                if(elements[i].name === name)
                    bookmarkElements.push(elements[i].parentNode);
            }
            return bookmarkElements;
        }
        this.getBookmarkBounds = function(bookmarkElements) {
            var x = this.getLeft(bookmarkElements[0]);
            var y = this.getTop(bookmarkElements[0]);
            var right = this.getRight(bookmarkElements[0]);
            var bottom = this.getBottom(bookmarkElements[0]);

            for(var i = 1; i < bookmarkElements.length; i++) {
                x = Math.min(x, this.getLeft(bookmarkElements[i]));
                y = Math.min(y, this.getTop(bookmarkElements[i]));
                right = Math.max(right, this.getRight(bookmarkElements[i]));
                bottom = Math.max(bottom, this.getBottom(bookmarkElements[i]));
            }
            var width = right - x - 2 * this.getBorderWidth();
            var height = bottom - y - 2 * this.getBorderWidth();

            return { 'left': x, 'top': y, 'width': width, 'height': height };
        }
        this.getLeft = function(el) {
            return ASPx.GetAbsoluteX(el) - ASPx.GetAbsoluteX(this.contentElement);
        };
        this.getTop = function(el) {
            return ASPx.GetAbsoluteY(el) - ASPx.GetAbsoluteY(this.contentElement);
        };
        this.getWidth = function(el) {
            return el.offsetWidth;
        };
        this.getHeight = function(el) {
            return el.offsetHeight;
        };
        this.getRight = function(el) {
            return this.getLeft(el) + this.getWidth(el);
        };
        this.getBottom = function(el) {
            return this.getTop(el) + this.getHeight(el);
        };
        this.addBookmarkElement = function(bounds) {
            var newEl = this.cloneElement(this.selectionTemplate, this.contentElement);
            newEl.style.display = 'block';
            newEl.style.backgroundColor = '';

            this.contentElement.appendChild(newEl);
            bounds.left -= this.getLeft(newEl);
            bounds.top -= this.getTop(newEl);

            newEl.style.width = getValueWithPxUnit(bounds.width);
            newEl.style.height = getValueWithPxUnit(bounds.height);
            newEl.style.position = 'relative';
            newEl.style.left = getValueWithPxUnit(bounds.left);
            newEl.style.top = getValueWithPxUnit(bounds.top);

            if(ASPx.Browser.Opera) {
                this.contentElement.appendChild(newEl);
            }

            return newEl;
        }
        this.cloneElement = function(element, elementFromTargetDocument) {
            return element.cloneNode(false);
        }
    }

    var dx_FormHelper = ASPx.CreateClass(null, {
        constructor: function() {
            this.targetFieldName = '__EVENTTARGET';
            this.argumentFieldName = '__EVENTARGUMENT';
        },
        sendPostbackInWindow: function(win, eventTarget, eventArgument) {
            this.eventTarget = eventTarget;
            this.eventArgument = eventArgument;
            var theForm = this.getTheForm();
            var formAttributes = { name: theForm.name, method: theForm.method, action: this.getCorectedAction(theForm.action), id: theForm.id };
            this.inputValues = {};
            this.inputUniqueIdCounter = 1;
            var content = "<html><body><div style='overflow:hidden;width:0px;height:0px'>"
                + this.buildTag("form", formAttributes, this.getInputsAndInitInputValues() + "<input type='submit' value='submit'/>")
                + '</div></body></html>';
            this.submitForm(win, content);
        },
        submitForm: function(win, content) {
            var submitFormCore = function() {
                this.writeContent(win, content);
                this.applyInputValues(win);
                var theForm = this.getTheForm();
                var form = win.document.getElementById(theForm.id);
                if(ASPx.Browser.Chrome && !ASPx.IsExists(form.submit)) { // B135437 workaround for Chrome bug 10493
                    var fakeInput = win.document.createElement("input");
                    fakeInput.setAttribute("type", "submit");
                    win.document.forms[0].appendChild(fakeInput);
                    fakeInput.click();
                } else
                    form.submit();
            }.aspxBind(this);
            if(typeof (win.document) === 'object') {
                submitFormCore();
                return;
            }
            var frameElement = win.frameElement;
            if(frameElement) {
                ASPx.Evt.AttachEventToElement(frameElement, "readystatechange", function submitFormCoreEventHandler() {
                    if(frameElement.readyState === "complete") {
                        ASPx.Evt.DetachEventFromElement(frameElement, "readystatechange", submitFormCoreEventHandler);
                        submitFormCore();
                    }
                });
                win.location = "about:blank";
            }
        },
        getCorectedAction: function(action) {
            var formAction = action;
            if(ASPx.Browser.Chrome || ASPx.Browser.Safari) {
                if(formAction.indexOf('?') < 0)
                    formAction = formAction + "?dxrep_fake=";
                else
                    formAction = formAction.replace('?', "?dxrep_fake=&");
            }
            return formAction;
        },
        writeContent: function(win, content) {
            var doc = win.document;
            doc.open("text/html", "replace");
            doc.write(content);
            doc.close();
        },
        applyInputValues: function(win) {
            for(var inputId in this.inputValues) {
                try {
                    win.document.getElementById(inputId).value = this.inputValues[inputId];
                } catch(e) {
                }
            }
        },
        getInputsAndInitInputValues: function() {
            var formElements = this.getTheForm().elements;
            var count = formElements.length;
            var result = '';

            for(var i = 0; i < count; i++) {
                var element = formElements[i];
                var tagName = element.tagName.toLowerCase();
                if(tagName === 'input') {
                    var type = element.type;
                    if((type === 'checkbox' || type === 'radio') && element.checked)
                        result += this.buildInput(element.type, this.getElementName(element.name), this.getElementName(element.id), this.getElementValue(element), element.checked);
                    else if(type === 'text' || type === 'hidden' || type === 'password')
                        result += this.buildInput(element.type, this.getElementName(element.name), this.getElementName(element.id), this.getElementValue(element));
                }
                else if(tagName === 'select') {
                    var selectCount = element.options.length;
                    for(var j = 0; j < selectCount; j++) {
                        var selectChild = element.options[j];
                        if(selectChild.selected === true) {
                            result += this.buildInput('hidden', this.getElementName(element.name), this.getElementName(element.id), selectChild.value);
                        }
                    }
                }
                else if(tagName === 'textarea') {
                    result += this.buildTextArea(this.getElementName(element.name), this.getElementName(element.id), element.value);
                }
            }
            return result;
        },
        getTheForm: function() {
            return theForm;
        },
        getElementName: function(name) {
            return name;
        },
        getElementValue: function(element) {
            if(element.id === this.targetFieldName)
                return this.eventTarget;
            if(element.id === this.argumentFieldName)
                return this.eventArgument;
            return element.value;
        },
        saveValue: function(id, value) {
            if(id === '' || this.inputValues[id] !== undefined)
                id = 'dx_fh_uniqueId' + this.inputUniqueIdCounter++;
            this.inputValues[id] = value;
            return id;
        },
        buildInput: function(type, name, id, value, checked) {
            id = this.saveValue(id, value);
            var params = { type: type, name: name, id: id };
            if(checked)
                params.checked = checked;
            return this.buildTag("input", params);
        },
        buildTextArea: function(name, id, value) {
            id = this.saveValue(id, value);
            return this.buildTag("textarea", { name: name, id: id }, '');
        },
        buildTag: function(tag, attributes, content) {
            var result = '<' + tag + ' ';
            for(var attrName in attributes)
                result += attrName + '="' + attributes[attrName].toString().replace(/"/g, '&quot;') + '" ';
            result += content != null
                ? '>' + content + '</' + tag + '>\n'
                : '/>\n';
            return result;
        }
    });

    var dx_FormHelperMobile = ASPx.CreateClass(dx_FormHelper, {
        constructor: function(owner) {
            this.owner = owner;
            this.constructor.prototype.constructor.call(this);
        },
        sendPostbackInWindow: function(win, eventTarget, eventArgument) {
            this.owner.SendPostBack(eventArgument);
        }
    });

    function xr_NavigateUrlForIE(url, target) {
        if(url == null)
            return;
        var a = document.createElement("a");
        a.setAttribute('href', url);
        if(target != null)
            a.setAttribute('target', target);
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
    }

    function xr_NavigateUrl(url, target) {
        if(ASPx.Browser.IE)
            xr_NavigateUrlForIE(url, target);
        else
            ASPx.Url.Navigate(url, target);
    }

    function xr_NavigateDrillDown(viewerName, drillDownKey) {
        var viewer = ASPx.GetControlCollection().Get(viewerName);
        if(viewer) {
            viewer.navigateDrillDown(drillDownKey);
        }
    }
    ASPxClientReportViewer.Cast = ASPxClientControl.Cast;
    ASPxClientReportViewer.prototype.Print = function(pageIndex) {
        if(pageIndex == null)
            pageIndex = "";
        if(this.printUsingAdobePlugIn)
            this.execPrintPdf(pageIndex);
        else
            this.execCallbackPrint(pageIndex);
    };
    ASPxClientReportViewer.prototype.GotoPage = function(pageIndex) {
        pageIndex = Math.max(0, Math.min(pageIndex, this.pageCount - 1));
        if(pageIndex !== this.getCurrentPageIndex()) {
            this.gotoPageInternal(pageIndex);
        }
    };
    ASPxClientReportViewer.prototype.Refresh = function() {
        this.gotoPageInternal(0);
        this.refreshRising.FireEvent(this);
    };
    ASPxClientReportViewer.prototype.Search = function() {
        if(this.IsSearchAllowed())
            this.showSearchWindow();
    };
    ASPxClientReportViewer.prototype.SaveToWindow = function(format) {
        this.exportWindow = window.open('', '_blank', 'toolbars=no, resizable=yes, scrollbars=yes');
        this.execExport("saveToWindow", { format: format }, this.exportWindow);
    };
    ASPxClientReportViewer.prototype.SaveToDisk = function(format) {
        this.execExport("saveToDisk", { format: format });
    };
    ASPxClientReportViewer.prototype.IsSearchAllowed = function() {
        if(!(this.pageCount > 0)) {
            return false;
        }
        try {
            if(ASPx.Browser.IE) {
                var body = document.body;
                return body.createTextRange && body.createTextRange();
            } else {
                return window.find;
            }
        } catch(e) {
            return false;
        }
    };
    var ASPxClientReportViewerPageLoadEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(pageIndex, pageCount) {
            this.constructor.prototype.constructor.call(this);
            this.PageIndex = pageIndex;
            this.PageCount = pageCount;
        },
        IsFirstPage: function() {
            return this.PageIndex === 0;
        },
        IsLastPage: function() {
            return this.PageIndex === this.PageCount - 1;
        }
    });

    window.ASPxClientReportViewer = ASPxClientReportViewer;
    window.ASPxClientReportViewerPageLoadEventArgs = ASPxClientReportViewerPageLoadEventArgs;

    ASPx.RVSDLoaded = rvsdLoaded;
    ASPx.RVSDFind = rvsdFind;
    ASPx.RVSDClose = rvsdClose;
    ASPx.RVGotoBM = rvGotoBM;
    ASPx.dx_FormHelper = dx_FormHelper;
    ASPx.dx_FormHelperMobile = dx_FormHelperMobile;
    ASPx.xr_NavigateUrl = xr_NavigateUrl;
    ASPx.xr_NavigateDrillDown = xr_NavigateDrillDown;
})(window);