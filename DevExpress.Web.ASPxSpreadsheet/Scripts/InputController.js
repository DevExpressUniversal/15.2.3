(function() {
    ASPxClientSpreadsheet.InputController = ASPx.CreateClass(null, {
        constructor: function(spreadsheetControl, targetInput) {
            this.spreadsheetControl = spreadsheetControl;
            this.targetInput = targetInput;
            this.editableDocument = this.targetInput.contentDocument || this.targetInput.contentWindow.document;
            this.canInsertTextOnInputEvent = ASPx.Browser.Firefox && ASPx.Browser.MajorVersion >= 14;
            this.initialize();
            this.pastedHTML = "";
            this.currentColIndex = -1;
            this.currentRowIndex = -1;
        },
        convertVisibleCellPositionToModel: function(colIndex, rowIndex) {
            var paneManager = this.spreadsheetControl.getPaneManager();
            return {
                col: paneManager.convertVisibleIndexToModelIndex(colIndex, true),
                row: paneManager.convertVisibleIndexToModelIndex(rowIndex, false)
            };
        },
        initialize: function() {
            var frameHtml = "<!DOCTYPE html>";
            frameHtml += "<html>";
            frameHtml += "<head>";
            frameHtml += "</head>";
            frameHtml += "<body style=\"padding: 0; margin: 0; overflow: hidden;\">";
            frameHtml += "</body>";
            frameHtml += "</html>";
            this.editableDocument.designMode = "on";
            this.editableDocument.open();
            this.editableDocument.write(frameHtml);
            this.editableDocument.close();
            this.initEvents();
        },
        initEvents: function() {
            ASPx.Evt.AttachEventToElement(this.editableDocument.body, "keydown", function(evt) {
                evt = this.normalizeEvent(evt);
                return this.spreadsheetControl.onKeyDown(evt, this.spreadsheetControl.getStateController().getEditMode());
            }.aspxBind(this));

            if(ASPx.Browser.WebKitFamily || ASPx.Browser.MSTouchUI)
                ASPx.Evt.AttachEventToElement(this.editableDocument.body, "keypress", function(evt) {
                    return this.spreadsheetControl.onKeyPress(evt, this.spreadsheetControl.getStateController().getEditMode());
                }.aspxBind(this));

            ASPx.Evt.AttachEventToElement(this.editableDocument.body, "focus", function() {
                this.selectEditableDocumentContent();
            }.aspxBind(this));

            ASPx.Evt.AttachEventToElement(this.editableDocument.body, "blur", function() {
                this.setEditableDocumentContent("");
            }.aspxBind(this));

            ASPx.Evt.AttachEventToElement(this.editableDocument.body, "paste", function(evt) {
                var e = evt;
                if(e && e.clipboardData && e.clipboardData.items) {
                    this.savePastedHtml(e.clipboardData.getData('text/html'));
                    var items = e.clipboardData.items, blob = null;

                    for(var i = 0; i < items.length; i++) {
                        if(ASPx.Browser.WebKitFamily && items[i].type.indexOf("text/rtf") === 0)
                            break;
                        if(items[i].type.indexOf("image") === 0)
                            blob = items[i].getAsFile();
                    }

                    if(blob) {
                        ASPx.Evt.PreventEvent(evt);
                        var reader = new FileReader();
                        var image = this.editableDocument.createElement("IMG");
                        image.alt = "pastedByCallback";
                        var spreadsheet = this.spreadsheetControl;
                        reader.onload = function(ev) {
                            image.src = ev.target.result;
                            spreadsheet.sendInternalServiceCallback(ASPxClientSpreadsheet.CallbackPrefixes.SaveImageCallbackPrefix, ev.target.result);
                        };
                        reader.readAsDataURL(blob);
                        this.editableDocument.body.appendChild(image);
                    }
                } else {
                    this.savePastedHtml("");
                }
            }.aspxBind(this));

            ASPx.Evt.AttachEventToElement(this.editableDocument, "contextmenu", ASPx.PopupUtils.PreventContextMenu);

            this.initRefocusingHandler();
        },
        initRefocusingHandler: function() {
            var focusHandlerCore = function() {
                var spreadsheet = this.spreadsheetControl;
                if(!spreadsheet.focusedElement || spreadsheet.focusedElement != spreadsheet.getEditingHelper().getEditorElement()) {
                    this.captureFocus();
                }
            }.aspxBind(this);

            var focusHandler = function(evt) {
                var eventSourceElement = ASPx.Evt.GetEventSource(evt);
                var shouldPreserveCustomFocus = eventSourceElement && this.isInExcludedHtmlElements(eventSourceElement.tagName);
                if(!shouldPreserveCustomFocus)
                    window.setTimeout(focusHandlerCore, 100);
            }.aspxBind(this);

            var refocusOnClick = function(element) {
                ASPx.Evt.AttachEventToElement(element, "mousedown", focusHandler);
            };

            var mainSheetDiv = this.spreadsheetControl.getRenderProvider().getWorkbookControl();
            if(mainSheetDiv)
                refocusOnClick(mainSheetDiv);

            var ribbon = this.spreadsheetControl.GetRibbon();
            if(ribbon)
                refocusOnClick(ribbon.GetMainElement());

            var tabControl = this.spreadsheetControl.getTabControl();
            if(tabControl)
                refocusOnClick(tabControl.GetMainElement());
        },

        isInExcludedHtmlElements: function(elementTagName) {
            var excludedItems = ["INPUT"];
            if(ASPx.Browser.IE && ASPx.Browser.Version < 9)
                return ASPx.Data.ArrayContains(excludedItems, elementTagName);
            return excludedItems.indexOf(elementTagName) >= 0;
        },

        captureFocus: function() {
            if(ASPx.Browser.TouchUI) return;

            if(ASPx.Browser.Opera || ASPx.Browser.Safari)
                this.targetInput.contentWindow.focus();
            else
                ASPx.Browser.IE || ASPx.Browser.Edge ? this.editableDocument.body.focus() : this.targetInput.focus();
        },

        selectEditableDocumentContent: function() {
            if(this.editableDocument.body.childNodes.length) {
                var firstChildNode = this.editableDocument.body.childNodes[0];
                if(!firstChildNode.childNodes.length)
                    return;
            }
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 9) {
                var range = this.editableDocument.body.createTextRange();
                range.moveToElementText(this.editableDocument.body);
                range.select();
            } else {
                var selection = this.editableDocument.getSelection();
                selection.removeAllRanges();
                selection.selectAllChildren(firstChildNode || this.editableDocument.body);
            }
        },

        setEditableDocumentContent: function(content) {
            this.editableDocument.body.innerHTML = content;
        },

        normalizeEvent: function(evt) {
            if(ASPx.Browser.IE && ASPx.Browser.MajorVersion < 9) {
                var eventCopy = {};
                for(var i in evt)
                    eventCopy[i] = evt[i];
                return eventCopy;
            }
            return evt;
        },

        keyDownEventStartTyping: function() {
            this.startTyping = true;
        },
        unMarkKeyDownEvent: function() {
            this.startTyping = false;
        },
        typingInProgress: function() {
            return this.startTyping;
        },
        setMissingKeyCode: function(code) {
            this.missingKeyCode = code;
        },
        getMissingKeyCode: function() {
            return this.missingKeyCode;
        },
        editingStartedFromKeyPressEvent: function() {
            return ASPx.Browser.WebKitFamily && this.getMissingKeyCode() > 0;
        },
        savePastedHtml: function(html) {
            this.pastedHTML = html;
        },
        getPastedHtml: function() {
            return this.pastedHTML;
        },

        // Copy Event
        onCopyEventProcessing: function(commandId) {
            this.setEditableDocumentContent("");
            this.selectEditableDocumentContent();
            this.renderSelectionToEditableDocument();
            this.selectEditableDocumentContent();
            if(commandId)
                ASPxClientSpreadsheet.ServerCommands.CopySelection(this.spreadsheetControl);
            else
                ASPxClientSpreadsheet.ServerCommands.CutSelection(this.spreadsheetControl);
            
            if(ASPx.Browser.Firefox)
                setTimeout(function () { this.setEditableDocumentContent(""); }.aspxBind(this), 10);
        },
        renderSelectionToEditableDocument: function() {
            var selection = this.spreadsheetControl.getStateController().getSelection();
            selection.expandToAllSheetIfRequired();
            if(selection.isDrawingBoxSelection())
                this.renderDrawingBoxToEditableDocument(selection.drawingBoxElement);
            else
                this.renderTableToEditableDocument(selection);
        },
        renderDrawingBoxToEditableDocument: function(drawingBoxElement) {
            var span = this.editableDocument.createElement("SPAN"),
                image = drawingBoxElement.cloneNode();
            image.className = "";
            span.appendChild(image);
            this.editableDocument.body.appendChild(span);
        },
        renderTableToEditableDocument: function(selection) {
            var table = this.editableDocument.createElement("TABLE"),
                defaultTableStyles = {
                    "border-collapse": "collapse",
                    "border-spacing": 0
                };
            ASPx.SetStyles(table, defaultTableStyles);
            table.cellPadding = 0;
            // create marker
            var marker = this.editableDocument.createElement("I");
            ASPx.Attr.SetAttribute(marker, "cbId", this.spreadsheetControl.getClientInstanceGuid());

            var paneManager = this.spreadsheetControl.getPaneManager(),
                // TODO if freeze panes and selectAll, document have 2+ visible area
                copyModelRange = selection.isAllSelected() ? paneManager.getVisibleModelCellRange() : {
                    top: selection.range.topRowIndex,
                    bottom: selection.range.bottomRowIndex,
                    left: selection.range.leftColIndex,
                    right: selection.range.rightColIndex
                };
            for(var row = copyModelRange.top; row <= copyModelRange.bottom; row++) {
                var tr = table.insertRow(),
                    rowModelIndex = paneManager.convertVisibleIndexToModelIndex(row, false),
                    rowHeight = this.spreadsheetControl.getCellElementStyleByModelPosition(selection.range.leftColIndex, rowModelIndex, ["height"]).height;
                ASPx.SetStyles(tr, { height: rowHeight });
                for(var col = copyModelRange.left; col <= copyModelRange.right; col++) {
                    var selectionHelper = this.spreadsheetControl.getSelectionHelper(),
                        editingRange = new ASPxClientSpreadsheet.Range(col, row, col, row),
                        expandedRange = selectionHelper.ExpandRangeToMergedCellSize(editingRange),
                        colModelIndex = paneManager.convertVisibleIndexToModelIndex(col, true),
                        shouldCreateTd = true,
                        colSpan = 0,
                        rowSpan = 0;

                    if(expandedRange.leftColIndex !== expandedRange.rightColIndex) {
                        colSpan = expandedRange.rightColIndex - expandedRange.leftColIndex + 1;
                        if(col !== expandedRange.leftColIndex) shouldCreateTd = false;
                    }
                    if(expandedRange.topRowIndex !== expandedRange.bottomRowIndex) {
                        rowSpan = expandedRange.bottomRowIndex - expandedRange.topRowIndex + 1;
                        if(row !== expandedRange.topRowIndex) shouldCreateTd = false;
                    }

                    if(shouldCreateTd) {
                        var td = tr.insertCell(),
                            cellStyle = this.getCellStyleForClipboard(colModelIndex, rowModelIndex, row === selection.range.topRowIndex),
                            rawValue = this.spreadsheetControl.getPaneManager().getCellValue(colModelIndex, rowModelIndex),
                            cell = this.spreadsheetControl.getRenderProvider().getCellTextElementByModelPosition(colModelIndex, rowModelIndex),
                            link = ASPx.GetNodeByTagName(cell, "a", 0);
                        ASPx.SetStyles(td, cellStyle);
                        if(colSpan)
                            td.colSpan = colSpan;
                        if(rowSpan)
                            td.rowSpan = rowSpan;

                        td.appendChild(link ? link.cloneNode(true) : this.editableDocument.createTextNode(rawValue ? rawValue : ""));
                    }
                }
            }
            marker.appendChild(table);
            this.editableDocument.body.appendChild(marker);
        },
        getCellStyleForClipboard: function(col, row, isFistRow) {
            var defaultCellStyle = {
                color: "#000000",
                backgroundColor: "#FFFFFF",
                textAlign: "left",
                textDecoration: "none",
                fontStyle: "normal",
                fontWeight: "normal",
                padding: 0
            };
            var cellStyle = this.spreadsheetControl.getCellElementStyleByModelPosition(col, row, this.getCellStyleAttributes(isFistRow));
            var defaultStyleAttributes = ASPx.GetObjectKeys(defaultCellStyle);
            for(var i = 0, len = defaultStyleAttributes.length; i < len; i++) {
                var attr = defaultStyleAttributes[i];
                if(cellStyle[attr] === defaultCellStyle[attr])
                    delete cellStyle[attr];
            }
            return cellStyle;
        },
        getCellStyleAttributes: function(isCellInFistRow) {
            var attributes = [
                "color", "backgroundColor", "verticalAlign", "whiteSpace",
                "fontFamily", "fontSize", "fontWeight", "fontStyle",
                "textAlign", "textDecoration", "textAlign"
            ];
            if(isCellInFistRow) attributes.push("width");
            return attributes;
        },

        // Paste Event
        onPasteEventProcessing: function() {
            this.onBeforePasteEvent();

            this.onPasteEvent();

            this.onAfterPasteEvent();
        },
        onBeforePasteEvent: function() {
            // Prepare html content
            var innerText = this.processPastedHtml(this.editableDocument.body.innerHTML);
            this.editableDocument.body.innerHTML = innerText;
            // Save current position of selection
            var selection = this.spreadsheetControl.getSelectionInternal(),
                modelCellIndex = this.convertVisibleCellPositionToModel(selection.range.leftColIndex, selection.range.topRowIndex);

            this.currentColIndex = modelCellIndex.col;
            this.currentRowIndex = modelCellIndex.row;

            this.clipboardData = [];
            // Prepare cache
            this.initializeStyleSheetSection();
        },
        onPasteEvent: function() {
            this.getClipboardDataFromInnerHtml(ASPx.GetChildNodes(this.editableDocument.body, function(e) {
                return true;
            }), false);
            var clipboardData = this.convertClipboardData();
            if(clipboardData && clipboardData.length > 0)
                ASPxClientSpreadsheet.ServerCommands.Paste(this.spreadsheetControl, { PasteValue: ASPx.Json.ToJson(clipboardData), BufferId: this.spreadsheetControl.getClientInstanceGuid() });
        },
        onAfterPasteEvent: function() {
            this.currentColIndex = -1;
            this.currentRowIndex = -1;
            this.cachedStyles = null;
            this.clipboardData = null;
            if(ASPx.Browser.Firefox)
               setTimeout(function () { this.setEditableDocumentContent(""); }.aspxBind(this), 10);
        },
        processPastedHtml: function(html) {
            // clear word formatting
            // remove Word attributes
            html = html.replace(/<(\w[^>]*) lang=([^ |>]*)([^>]*)/gi, "<$1$3");
            html = html.replace(/\s*mso-bidi-font-family/gi, "font-family");
            html = html.replace(/\s*MARGIN: 0cm 0cm 0pt\s*;/gi, '');
            html = html.replace(/\s*MARGIN: 0cm 0cm 0pt\s*"/gi, "\"");
            html = html.replace(/\s*TEXT-INDENT: 0cm\s*;/gi, '');
            html = html.replace(/\s*TEXT-INDENT: 0cm\s*"/gi, "\"");
            html = html.replace(/\s*FONT-VARIANT: [^\s;]+;?"/gi, "\"");
            html = html.replace(/\s*tab-stops:[^;"]*;?/gi, '');
            html = html.replace(/\s*tab-stops:[^"]*/gi, '');

            // remove special Word tags
            html = html.replace(/<\w+:imagedata/gi, '<img');
            html = html.replace(/<\/?\w+:[^>]*>/gi, '');
            html = html.replace(/<STYLE[^>]*>[\s\S]*?<\/STYLE[^>]*>/gi, '');
            html = html.replace(/<(?:META|LINK)[^>]*>\s*/gi, '');
            html = html.replace(/<\\?\?xml[^>]*>/gi, '');
            html = html.replace(/<o:[pP][^>]*>\s*<\/o:[pP]>/gi, '');
            html = html.replace(/<o:[pP][^>]*>.*?<\/o:[pP]>/gi, '&nbsp;');
            html = html.replace(/<st1:.*?>/gi, '');
            html = html.replace(/<\!--[\s\S]*?-->/g, '');

            // remove empty attributes
            html = html.replace(/\s*style="\s*"/gi, '');
            html = html.replace(/style=""/ig, "");
            html = html.replace(/style=''/ig, "");

            // clean style attributes
            var stRegExp = new RegExp('(?:style=\\")([^\\"]*)(?:\\")', 'gi');
            html = html.replace(stRegExp, function(str) {
                str = str.replace(/&quot;/gi, "'");
                str = str.replace(/&#xA;/gi, " ");
                return str;
            });

            // replace ugly Word markup
            html = html.replace(/^\s|\s$/gi, '');
            html = html.replace(/<p>&nbsp;<\/p>/gi, '<br /><br />');
            html = html.replace(/<font\s*>([^<>]+)<\/font>/gi, '$1');
            html = html.replace(/<span\s*><span\s*>([^<>]+)<\/span><\/span>/ig, '$1');
            html = html.replace(/<span>([^<>]+)<\/span>/gi, '$1');

            // Remove nested empty tags
            // safe empty td
            html = html.replace(/<td([^>]*)>\s*<\/td>/gi, '<td$1>&nbsp;</td>');

            var re = /<([^\s>]+)(\s[^>]*)?><\/\1>/g;
            while(html != html.replace(re, ''))
                html = html.replace(re, '');

            re = /<([^\s>]+)(\s[^>]*)?>\s+<\/\1>/g;
            while(html != html.replace(re, ' '))
                html = html.replace(re, ' ');

            // merge font family attributes
            var array = html.match(/<[^>]*style\s*=\s*[^>]*>/gi);
            if(array && array.length > 0) {
                for(var i = 0, elementHtml; elementHtml = array[i]; i++) {
                    var fontFamilyArray = elementHtml.match(/\s*font-family\s*:\s*([^;]*)[\"'; ]/gi);
                    if(fontFamilyArray && fontFamilyArray.length > 0) {
                        var commonValue = "";
                        for(var j = 0, fontFamily; fontFamily = fontFamilyArray[j]; j++) {
                            commonValue += commonValue ? "," : "";
                            commonValue += fontFamily.replace(/font-family\s*:\s*([^;]*)[\"'; ]/gi, "$1");
                        }
                        html = html.replace(elementHtml, elementHtml.replace(fontFamilyArray[0], "font-family: " + commonValue + ";"));
                    }
                }
            }
            html = html.replace(/^\n|\n$/gi, '');
            html = html.replace(/\n/gi, ' ');

            //clear particular tags
            //html = html.replace(/<u>([\s\S]*?)<\/u>/gi, '<span style="text-decoration: underline">$1</span>');
            //html = html.replace(/<s>([\s\S]*?)<\/s>/gi, '<span style="text-decoration: line-through">$1</span>');
            var re = /<span style=\"mso-bookmark:\s*?OLE_LINK\d*?;?\"\>([\s\S]*?)<\/span>/gi;
            while(html != html.replace(re, "$1"))
                html = html.replace(re, "$1");
            re = /<a name=\"OLE_LINK\d*?\"\>([\s\S]*?)<\/a>/gi;
            while(html != html.replace(re, "$1"))
                html = html.replace(re, "$1");

            html = html.replace(/<\/([^\s>]+)(\s[^>]*)?><br><\/([^\s>]+)(\s[^>]*)?>/gi, '');
            return html;
        },
        getClipboardDataFromInnerHtml: function(elements) {
            for(var i = 0, element; element = elements[i]; i++) {
                var childElements = ASPx.GetChildNodes(element, function(e) {
                    return true;
                });
                //Transfer styles from parent to children
                if(childElements.length && element.tagName != "TABLE") {
                    for(var j = 0, childElement; childElement = childElements[j]; j++) {
                        if(childElement.style) {
                            for(var prop in childElement.style) {
                                if(childElement.style[prop] === "" && !(prop == "cssText" || prop == "listStyleType") && element.style[prop] !== "") {
                                    childElement.style[prop] = element.style[prop];
                                }
                            }
                        }
                        // copy marker to table
                        if(element.tagName === "I") {
                            var bufferId = ASPx.Attr.GetAttribute(element, "cbId");
                            if(bufferId) {
                                ASPx.Attr.SetAttribute(childElement, "cbId", bufferId);
                            }
                        }
                    }

                    var isParagraph = element.tagName == "DIV" || element.tagName == "P";
                    if(isParagraph)
                        this.clipboardData.push({ TagName: "P", Element: element, Mark: "BP" });

                    this.getClipboardDataFromInnerHtml(childElements);

                    if(isParagraph)
                        this.clipboardData.push({ TagName: "P", Mark: "EP" });

                } else {
                    switch(element.tagName) {
                        case "TABLE":
                            var bufferId = ASPx.Attr.GetAttribute(element, "cbId");
                            if(bufferId)
                                this.clipboardData.unshift({ TagName: "Important", Data: { ServerBufferId: bufferId } });
                            this.clipboardData.push({ TagName: element.tagName, Element: element });
                            break;
                        case "IMG":
                            if(element.alt !== "pastedByCallback")
                                this.clipboardData.push({ TagName: element.tagName, Element: element });
                            break;
                        case "DIV":
                            this.clipboardData.push({ TagName: "P", Element: element, Mark: "BP" });
                            this.clipboardData.push({ TagName: "", Data: ASPx.GetInnerText(element) });
                            this.clipboardData.push({ TagName: "P", Mark: "EP" });
                            break;
                        case "BR":
                            this.clipboardData.push({ TagName: element.tagName, Element: element });
                            break;
                        default:
                            this.clipboardData.push({ TagName: "", Data: element.nodeValue || ASPx.GetInnerText(element) });
                            break;
                    }
                }
            }
        },
        convertClipboardData: function() {
            var clipboardData = [],
                lastParagraph = null,
                paragraphText = [],
                paragraphFlag = false;

            for(var i = 0, element; element = this.clipboardData[i]; i++) {
                switch(element.TagName) {
                    case "Important":
                        clipboardData.push(element.Data);
                        break;
                    case "TABLE":
                        clipboardData = clipboardData.concat(this.parseTable(element.Element));
                        break;
                    case "IMG":
                        if(paragraphFlag && paragraphText.join("") != "") {
                            clipboardData.push(this.parseParagraph(lastParagraph, paragraphText));
                            this.currentRowIndex += 1;
                            paragraphText = [];
                        }

                        clipboardData.push(this.parseImage(element.Element));
                        break;
                    case "P":
                        if(element.Mark === "BP") {
                            paragraphText = [];
                            paragraphFlag = true;
                            if(element.Element) {
                                lastParagraph = element.Element;
                            }
                        } else if(element.Mark === "EP") {
                            paragraphFlag = false;
                            clipboardData.push(this.parseParagraph(lastParagraph, paragraphText));
                            this.currentRowIndex += 1;
                            paragraphText = [];
                        }
                        break;
                    case "BR":
                        if(paragraphFlag) {
                            paragraphText.push("\r\n");
                        } else {
                            clipboardData.push(this.parseText(paragraphText));
                            paragraphText = [];
                            this.currentRowIndex += 1;
                        }
                        break;
                    default:
                        paragraphText.push(element.Data);
                        break;
                }
            }
            if(paragraphText.length > 0)
                clipboardData.push(this.parseText(paragraphText));
            return clipboardData;
        },
        parseText: function(text) {
            var cellStyle = new this.getElementDefaultStyle();
            cellStyle.Value = text.join("");
            cellStyle.Wrap = cellStyle.Wrap || cellStyle.Value.indexOf("\r\n") > 0;
            cellStyle.RowIndex = this.currentRowIndex;
            cellStyle.ColIndex = this.currentColIndex;

            cellStyle.ColSpan = 1;
            cellStyle.RowSpan = 1;

            return cellStyle;
        },
        parseImage: function(element) {
            var cellStyle = new this.getElementDefaultStyle();
            cellStyle.Value = "";
            cellStyle.RowIndex = this.currentRowIndex;
            cellStyle.ColIndex = this.currentColIndex;
            cellStyle.ImgUrl = ASPx.Attr.GetAttribute(element, "src");
            cellStyle.ColSpan = 1;
            cellStyle.RowSpan = 1;

            return cellStyle;
        },
        parseParagraph: function(element, paragraphText) {
            var cellStyle = this.getElementStyle(element);
            cellStyle.Value = paragraphText.join("");
            cellStyle.Wrap = cellStyle.Wrap || cellStyle.Value.indexOf("\r\n") > 0;
            cellStyle.RowIndex = this.currentRowIndex;
            cellStyle.ColIndex = this.currentColIndex;

            cellStyle.ColSpan = 1;
            cellStyle.RowSpan = 1;

            return cellStyle;
        },
        parseTable: function(element) {
            var tableColumnCount = this.getTableColumnCount(element),
                rowSpanCorrection = this.initializeArray(tableColumnCount),
                tableContent = [];

            for(var i = 0, row, colSpanCorrection = 0; row = element.rows[i]; i++) {
                for(var j = 0; j < tableColumnCount; j++) {
                    if(rowSpanCorrection[j] > 0) {
                        colSpanCorrection = colSpanCorrection + 1;
                        rowSpanCorrection[j] = rowSpanCorrection[j] - 1;
                    } else {
                        var cell = row.cells[j - colSpanCorrection];
                        if(cell) {
                            var cellStyle = this.getElementStyle(cell);

                            cellStyle.Value = ASPx.GetInnerText(cell);
                            cellStyle.Wrap = cellStyle.Wrap || cellStyle.Value.indexOf("\r\n") > 0;
                            cellStyle.RowIndex = this.currentRowIndex + i;
                            cellStyle.ColIndex = this.currentColIndex + j;

                            cellStyle.ColSpan = cell.colSpan;
                            cellStyle.RowSpan = cell.rowSpan;

                            //TODO: to Zhuravlev in one cell can be more than 1 images
                            var innerImages = cell.getElementsByTagName("img");
                            if(innerImages && innerImages.length > 0)
                                cellStyle.ImgUrl = ASPx.Attr.GetAttribute(innerImages[0], "src");

                            var innerHyperLinks = cell.getElementsByTagName("a");
                            if(innerHyperLinks && innerHyperLinks.length > 0) {
                                var linkSource = ASPx.Attr.GetAttribute(innerHyperLinks[0], "href"),
                                    linkTitle = ASPx.Attr.GetAttribute(innerHyperLinks[0], "title");
                                cellStyle.Link.Url = linkSource ? linkSource : "";
                                cellStyle.Link.Alt = linkTitle ? linkTitle : "";
                            }

                            //vertical correction
                            if(cellStyle.RowSpan > 1) {
                                for(var k = j; k < j + cellStyle.ColSpan; k++)
                                    rowSpanCorrection[k] = cellStyle.RowSpan - 1;
                            }
                            //horizontal correction
                            if(cellStyle.ColSpan > 1) {
                                colSpanCorrection = colSpanCorrection + cellStyle.ColSpan - 1;
                                j = j + cellStyle.ColSpan - 1;
                            }

                            tableContent.push(cellStyle);
                        }
                    }
                }
                colSpanCorrection = 0;
            }
            this.currentRowIndex = this.currentRowIndex + element.rows.length;
            return tableContent;
        },

        getElementStyle: function(element) {
            var elementInlineStyle = this.getElementBaseStyle(element),
                elementCssStyle = this.getElementStyleByCssRules(element.className),
                elementStyleByTagName = this.getElementStyleByTagName(element.tagName);

            return this.mergeElementStyles(elementInlineStyle, elementCssStyle, elementStyleByTagName);
        },

        getElementBaseStyle: function(element) {
            if(element.tagName === "TD") {
                return this.analyzeCellContainer(element);
            } else {
                if(element.tagName === "P") {
                    return this.analyzeParagraph(element);
                }
            }

            return this.getElementInlineStyle(element.style);
        },
        analyzeCellContainer: function(cellContainer) {
            var cellBaseStyle = this.getElementInlineStyle(cellContainer.style);

            if(cellContainer.getElementsByTagName("SPAN").length && cellContainer.getElementsByTagName("SPAN").length === 1) {
                cellBaseStyle = this.mergeElementStyles(cellBaseStyle, this.getElementInlineStyle(cellContainer.getElementsByTagName("SPAN")[0].style), new this.getElementDefaultStyle());

                if(cellContainer.getElementsByTagName("B").length || cellContainer.getElementsByTagName("STRONG").length)
                    cellBaseStyle.Font.Bold = true;

                if(cellContainer.getElementsByTagName("U").length)
                    cellBaseStyle.Font.Decoration += " underline";

                if(cellContainer.getElementsByTagName("S").length)
                    cellBaseStyle.Font.Decoration += " line-through";

                if(cellContainer.getElementsByTagName("I").length || cellContainer.getElementsByTagName("EM").length)
                    cellBaseStyle.Font.Style = "italic";
            }

            if(ASPx.Browser.IE && cellContainer.getElementsByTagName("FONT").length && cellContainer.getElementsByTagName("FONT").length === 1) {
                var fontContainer = cellContainer.getElementsByTagName("FONT")[0];
                cellBaseStyle.Font.Name = cellBaseStyle.Font.Name || this.normalizeFontName(fontContainer.face);
                cellBaseStyle.Font.Size = cellBaseStyle.Font.Size || this.getFontSize(this.convertFontSizeFromCUToPoint(fontContainer.size));
                cellBaseStyle.Font.Color = cellBaseStyle.Font.Color || ASPxClientSpreadsheet.ParseColor(fontContainer.color)
            }
            return cellBaseStyle;
        },
        analyzeParagraph: function(paragraphElement) {
            var paragraphBaseStyle = this.getElementInlineStyle(paragraphElement.style);

            if(paragraphElement.getElementsByTagName("SPAN").length && paragraphElement.getElementsByTagName("SPAN").length === 1) {
                paragraphBaseStyle = this.mergeElementStyles(paragraphBaseStyle, this.getElementInlineStyle(paragraphElement.getElementsByTagName("SPAN")[0].style), new this.getElementDefaultStyle());

                if(paragraphElement.getElementsByTagName("B").length || paragraphElement.getElementsByTagName("STRONG").length)
                    paragraphBaseStyle.Font.Bold = true;

                if(paragraphElement.getElementsByTagName("U").length)
                    paragraphBaseStyle.Font.Decoration += " underline";

                if(paragraphElement.getElementsByTagName("S").length)
                    paragraphBaseStyle.Font.Decoration += " line-through";

                if(paragraphElement.getElementsByTagName("I").length || paragraphElement.getElementsByTagName("EM").length)
                    paragraphBaseStyle.Font.Style = "italic";
            }

            if(ASPx.Browser.IE && paragraphElement.getElementsByTagName("FONT").length && paragraphElement.getElementsByTagName("FONT").length === 1) {
                var fontContainer = paragraphElement.getElementsByTagName("FONT")[0];
                paragraphBaseStyle.Font.Name = paragraphBaseStyle.Font.Name || this.normalizeFontName(fontContainer.face);
                paragraphBaseStyle.Font.Size = paragraphBaseStyle.Font.Size || this.getFontSize(this.convertFontSizeFromCUToPoint(fontContainer.size));
                paragraphBaseStyle.Font.Color = paragraphBaseStyle.Font.Color || ASPxClientSpreadsheet.ParseColor(fontContainer.color);
            }

            var innerHyperLinks = paragraphElement.getElementsByTagName("a");
            if(innerHyperLinks && innerHyperLinks.length > 0) {
                var linkSource = ASPx.Attr.GetAttribute(innerHyperLinks[0], "href"),
                    linkTitle = ASPx.Attr.GetAttribute(innerHyperLinks[0], "title");
                paragraphBaseStyle.Link.Url = linkSource ? linkSource : "";
                paragraphBaseStyle.Link.Alt = linkTitle ? linkTitle : "";

            }

            return paragraphBaseStyle;
        },
        getElementInlineStyle: function(cellStyle) {
            var elementStyle = new this.getElementDefaultStyle();
            elementStyle.Font.Name = this.normalizeFontName(cellStyle.fontFamily);
            elementStyle.Font.Size = this.getFontSize(cellStyle.fontSize);
            elementStyle.Font.Color = ASPxClientSpreadsheet.ParseColor(cellStyle.color);
            elementStyle.Font.Bold = this.checkFontBold(cellStyle.fontWeight);
            elementStyle.Font.Decoration = cellStyle.textDecoration;
            elementStyle.Font.Style = cellStyle.fontStyle;

            elementStyle.VAlign = this.getVerticalAlign(cellStyle.verticalAlign);
            elementStyle.HAlign = this.getHorizontalAlign(cellStyle.textAlign);

            elementStyle.BackgroundColor = ASPxClientSpreadsheet.ParseColor(cellStyle.backgroundColor);
            elementStyle.Wrap = !(cellStyle.whiteSpace === "nowrap" || cellStyle.whiteSpace === "");

            elementStyle.Border.Left.Color = ASPxClientSpreadsheet.ParseColor(cellStyle.borderLeftColor);
            elementStyle.Border.Left.Style = this.getCoreLineStyle(cellStyle.borderLeftWidth, cellStyle.borderLeftStyle);

            elementStyle.Border.Right.Color = ASPxClientSpreadsheet.ParseColor(cellStyle.borderRightColor);
            elementStyle.Border.Right.Style = this.getCoreLineStyle(cellStyle.borderRightWidth, cellStyle.borderRightStyle);

            elementStyle.Border.Top.Color = ASPxClientSpreadsheet.ParseColor(cellStyle.borderTopColor);
            elementStyle.Border.Top.Style = this.getCoreLineStyle(cellStyle.borderTopWidth, cellStyle.borderTopStyle);

            elementStyle.Border.Bottom.Color = ASPxClientSpreadsheet.ParseColor(cellStyle.borderBottomColor);
            elementStyle.Border.Bottom.Style = this.getCoreLineStyle(cellStyle.borderBottomWidth, cellStyle.borderBottomStyle);
            return elementStyle;
        },
        getElementStyleByCssRules: function(className) {
            return this.getStyleRulesByTagOrClassName(className);
        },
        getElementStyleByTagName: function(tagName) {
            return this.getStyleRulesByTagOrClassName(tagName);
        },
        getStyleRulesByTagOrClassName: function(tagOrClassName) {
            var elementStyle = new this.getElementDefaultStyle();
            var styleSection = this.getStyleSectionFromCache();
            if(styleSection && tagOrClassName && tagOrClassName !== "") {
                var styleTextValue = styleSection.match(new RegExp(tagOrClassName + "[\\s\\S]*?\\}", "gi"));
                if(styleTextValue && styleTextValue.length > 0) {
                    var headElement = ASPx.GetNodeByTagName(this.editableDocument, "HEAD", 0);
                    var style = this.editableDocument.createElement('style');
                    style.type = 'text/css';
                    style.innerHTML = styleTextValue[0];
                    headElement.appendChild(style);
                    if(this.editableDocument.styleSheets.length > 0 && this.editableDocument.styleSheets[0].cssRules.length > 0)
                        elementStyle = this.getElementInlineStyle(this.editableDocument.styleSheets[0].cssRules[0].style);
                    headElement.removeChild(style);
                }
            }
            return elementStyle;
        },

        getElementDefaultStyle: function() {
            var cellInfo = {
                Value: "",
                ImgUrl: "",
                Link: { Url: "", Alt: "" },
                Font: { Name: "", Color: "", Size: "", Bold: "", Style: "", Decoration: false },
                Border: { Left: { Color: "", Style: "" }, Right: { Color: "", Style: "" }, Top: { Color: "", Style: "" }, Bottom: { Color: "", Style: "" } },
                BackgroundColor: "",
                VAlign: "",
                HAlighn: "",
                Wrap: "",
                ColIndex: -1,
                RowIndex: -1,
                ColSpan: 1,
                RowSpan: 1
            };
            return cellInfo;
        },
        getCoreLineStyle: function(lineWidth, lineStyle) {
            var result = "";
            if(lineStyle !== "" && lineStyle !== "none") {
                if(lineWidth.indexOf("pt") > 0)
                    lineWidth = this.convertMSPointsToPixels(lineWidth);
                else
                    lineWidth = ASPx.PxToInt(lineWidth);
                if(lineStyle === "solid") {
                    switch(lineWidth) {
                        case 1: result = "Thin";
                            break;
                        case 2: result = "Medium";
                            break;
                        default: result = "Thick";
                            break;
                    }
                } else if(lineStyle === "dotted" || lineStyle === "dashed") {
                    switch(lineWidth) {
                        case 2: result = "Medium" + lineStyle.charAt(0).toUpperCase() + lineStyle.slice(1);
                            break;
                        default: result = lineStyle.charAt(0).toUpperCase() + lineStyle.slice(1);
                            break;
                    }

                } else {
                    result = "Thin";
                }
            }
            return result
        },
        getFontSize: function(fontSize) {// copied from ribbon manager
            var size = 0;
            if(fontSize.indexOf('px') > -1)
                size = Math.round(ASPx.PixelToPoint(fontSize, false));
            else {
                if(fontSize.indexOf('p') > -1)
                    size = fontSize.substr(0, fontSize.indexOf('p'));
                else size = fontSize;
            }
            return size;
        },
        convertFontSizeFromCUToPoint: function(fontSizeInCU) {
            if(fontSizeInCU === "")
                return "";
            var pixelFactor = [0.625, 0.80, 1.0, 1.125, 1.5, 2.0, 3.0],
                baseFontSize = 12;
            return Math.round(pixelFactor[parseInt(fontSizeInCU) - 1] * baseFontSize) + "pt";
        },
        getVerticalAlign: function(verticalAlign) {
            var result = "";
            switch(verticalAlign) {
                case "":
                    result = "";
                    break;
                case "top":
                    result = "Top";
                    break;
                case "bottom":
                    result = "Bottom";
                    break;
                case "center":
                    result = "Center";
                    break;
                default:
                    result = "Bottom";
                    break;
            }
            return result;
        },
        getHorizontalAlign: function(horizontalAlign) {
            var result = "";
            switch(horizontalAlign) {
                case "":
                    result = "";
                    break;
                case "left":
                    result = "Left";
                    break;
                case "center":
                    result = "Center";
                    break;
                case "right":
                    result = "Right";
                    break;
                default:
                    result = "General";
                    break;
            }
            return result;
        },
        checkFontBold: function(fontWeight) {
            var fontIsBold = fontWeight === "bold";
            if(!fontIsBold)
                fontIsBold = parseInt(fontWeight) >= 700;
            return fontIsBold;
        },
        convertMSPointsToPixels: function(points) {
            var result = 1;
            try {
                var indexOfPt = points.toLowerCase().indexOf("pt");
                if(indexOfPt > -1)
                    points = points.substr(0, indexOfPt);
                result = parseFloat(points) / 0.5;
            } catch(e) { }
            return result;
        },
        normalizeFontName: function(fontName) {
            if(fontName !== "") {
                var commaIndex = fontName.indexOf(",");
                if(commaIndex > 0)
                    fontName = fontName.substr(0, commaIndex);
                fontName = fontName.replace(/["']/g, "");
            }
            return fontName;
        },
        // Cache
        initializeStyleSheetSection: function() {
            var importCache = {
                styleSection: null,
                tagNameStyles: {}
            };
            var fullHtml = this.getPastedHtml();
            if(fullHtml != "") {
                var headHtml = fullHtml.match(/<style[\s\S]*?<\/style>/gi);
                if(headHtml && headHtml.length > 0) {
                    importCache.styleSection = headHtml[0];
                }
            }
            this.cachedStyles = importCache;
        },
        getStyleSectionFromCache: function() {
            return this.cachedStyles.styleSection;
        },
        // Table helpers
        getTableColumnCount: function(tableElement) {
            var i = 0,
                colCount = 0;
            while(tableElement.rows[0].cells[i]) {
                colCount += tableElement.rows[0].cells[i].colSpan;
                i++;
            }
            return colCount;
        },
        initializeArray: function(itemCount) {
            var initialArray = [];
            for(var i = 0; i < itemCount; i++)
                initialArray.push(0);
            return initialArray;
        },
        mergeElementStyles: function(elementInlineStyle, elementCssStyle, elementStyleByTagName) {
            elementInlineStyle.Font.Name = elementInlineStyle.Font.Name || elementCssStyle.Font.Name || elementStyleByTagName.Font.Name;
            elementInlineStyle.Font.Size = elementInlineStyle.Font.Size || elementCssStyle.Font.Size || elementStyleByTagName.Font.Size;
            elementInlineStyle.Font.Color = elementInlineStyle.Font.Color || elementCssStyle.Font.Color || elementStyleByTagName.Font.Color;
            elementInlineStyle.Font.Bold = elementInlineStyle.Font.Bold || elementCssStyle.Font.Bold || elementStyleByTagName.Font.Bold;
            elementInlineStyle.Font.Decoration = elementInlineStyle.Font.Decoration || elementCssStyle.Font.Decoration || elementStyleByTagName.Font.Decoration;
            elementInlineStyle.Font.Style = elementInlineStyle.Font.Style || elementCssStyle.Font.Style || elementStyleByTagName.Font.Style;
            elementInlineStyle.VAlign = elementInlineStyle.VAlign || elementCssStyle.VAlign || elementStyleByTagName.VAlign;
            elementInlineStyle.HAlign = elementInlineStyle.HAlign || elementCssStyle.HAlign || elementStyleByTagName.HAlign;
            elementInlineStyle.BackgroundColor = elementInlineStyle.BackgroundColor || elementCssStyle.BackgroundColor || elementStyleByTagName.BackgroundColor;
            elementInlineStyle.Wrap = elementInlineStyle.Wrap || elementCssStyle.Wrap || elementStyleByTagName.Wrap;
            elementInlineStyle.Border.Left.Color = elementInlineStyle.Border.Left.Color || elementCssStyle.Border.Left.Color || elementStyleByTagName.Border.Left.Color;
            elementInlineStyle.Border.Left.Style = elementInlineStyle.Border.Left.Style || elementCssStyle.Border.Left.Style || elementStyleByTagName.Border.Left.Style;
            elementInlineStyle.Border.Right.Color = elementInlineStyle.Border.Right.Color || elementCssStyle.Border.Right.Color || elementStyleByTagName.Border.Right.Color;
            elementInlineStyle.Border.Right.Style = elementInlineStyle.Border.Right.Style || elementCssStyle.Border.Right.Style || elementStyleByTagName.Border.Right.Style;
            elementInlineStyle.Border.Top.Color = elementInlineStyle.Border.Top.Color || elementCssStyle.Border.Top.Color || elementStyleByTagName.Border.Top.Color;
            elementInlineStyle.Border.Top.Style = elementInlineStyle.Border.Top.Style || elementCssStyle.Border.Top.Style || elementStyleByTagName.Border.Top.Style;
            elementInlineStyle.Border.Bottom.Color = elementInlineStyle.Border.Bottom.Color || elementCssStyle.Border.Bottom.Color || elementStyleByTagName.Border.Bottom.Color;
            elementInlineStyle.Border.Bottom.Style = elementInlineStyle.Border.Bottom.Style || elementCssStyle.Border.Bottom.Style || elementStyleByTagName.Border.Bottom.Style;
            return elementInlineStyle;
        }
    });
})();

(function() {
    var colorNameArray = {
        aliceblue: 'f0f8ff',
        antiquewhite: 'faebd7',
        aqua: '00ffff',
        aquamarine: '7fffd4',
        azure: 'f0ffff',
        beige: 'f5f5dc',
        bisque: 'ffe4c4',
        black: '000000',
        blanchedalmond: 'ffebcd',
        blue: '0000ff',
        blueviolet: '8a2be2',
        brown: 'a52a2a',
        burlywood: 'deb887',
        cadetblue: '5f9ea0',
        chartreuse: '7fff00',
        chocolate: 'd2691e',
        coral: 'ff7f50',
        cornflowerblue: '6495ed',
        cornsilk: 'fff8dc',
        crimson: 'dc143c',
        cyan: '00ffff',
        darkblue: '00008b',
        darkcyan: '008b8b',
        darkgoldenrod: 'b8860b',
        darkgray: 'a9a9a9',
        darkgreen: '006400',
        darkkhaki: 'bdb76b',
        darkmagenta: '8b008b',
        darkolivegreen: '556b2f',
        darkorange: 'ff8c00',
        darkorchid: '9932cc',
        darkred: '8b0000',
        darksalmon: 'e9967a',
        darkseagreen: '8fbc8f',
        darkslateblue: '483d8b',
        darkslategray: '2f4f4f',
        darkturquoise: '00ced1',
        darkviolet: '9400d3',
        deeppink: 'ff1493',
        deepskyblue: '00bfff',
        dimgray: '696969',
        dodgerblue: '1e90ff',
        feldspar: 'd19275',
        firebrick: 'b22222',
        floralwhite: 'fffaf0',
        forestgreen: '228b22',
        fuchsia: 'ff00ff',
        gainsboro: 'dcdcdc',
        ghostwhite: 'f8f8ff',
        gold: 'ffd700',
        goldenrod: 'daa520',
        gray: '808080',
        green: '008000',
        greenyellow: 'adff2f',
        honeydew: 'f0fff0',
        hotpink: 'ff69b4',
        indianred: 'cd5c5c',
        indigo: '4b0082',
        ivory: 'fffff0',
        khaki: 'f0e68c',
        lavender: 'e6e6fa',
        lavenderblush: 'fff0f5',
        lawngreen: '7cfc00',
        lemonchiffon: 'fffacd',
        lightblue: 'add8e6',
        lightcoral: 'f08080',
        lightcyan: 'e0ffff',
        lightgoldenrodyellow: 'fafad2',
        lightgrey: 'd3d3d3',
        lightgreen: '90ee90',
        lightpink: 'ffb6c1',
        lightsalmon: 'ffa07a',
        lightseagreen: '20b2aa',
        lightskyblue: '87cefa',
        lightslateblue: '8470ff',
        lightslategray: '778899',
        lightsteelblue: 'b0c4de',
        lightyellow: 'ffffe0',
        lime: '00ff00',
        limegreen: '32cd32',
        linen: 'faf0e6',
        magenta: 'ff00ff',
        maroon: '800000',
        mediumaquamarine: '66cdaa',
        mediumblue: '0000cd',
        mediumorchid: 'ba55d3',
        mediumpurple: '9370d8',
        mediumseagreen: '3cb371',
        mediumslateblue: '7b68ee',
        mediumspringgreen: '00fa9a',
        mediumturquoise: '48d1cc',
        mediumvioletred: 'c71585',
        midnightblue: '191970',
        mintcream: 'f5fffa',
        mistyrose: 'ffe4e1',
        moccasin: 'ffe4b5',
        navajowhite: 'ffdead',
        navy: '000080',
        oldlace: 'fdf5e6',
        olive: '808000',
        olivedrab: '6b8e23',
        orange: 'ffa500',
        orangered: 'ff4500',
        orchid: 'da70d6',
        palegoldenrod: 'eee8aa',
        palegreen: '98fb98',
        paleturquoise: 'afeeee',
        palevioletred: 'd87093',
        papayawhip: 'ffefd5',
        peachpuff: 'ffdab9',
        peru: 'cd853f',
        pink: 'ffc0cb',
        plum: 'dda0dd',
        powderblue: 'b0e0e6',
        purple: '800080',
        red: 'ff0000',
        rosybrown: 'bc8f8f',
        royalblue: '4169e1',
        saddlebrown: '8b4513',
        salmon: 'fa8072',
        sandybrown: 'f4a460',
        seagreen: '2e8b57',
        seashell: 'fff5ee',
        sienna: 'a0522d',
        silver: 'c0c0c0',
        skyblue: '87ceeb',
        slateblue: '6a5acd',
        slategray: '708090',
        snow: 'fffafa',
        springgreen: '00ff7f',
        steelblue: '4682b4',
        tan: 'd2b48c',
        teal: '008080',
        thistle: 'd8bfd8',
        tomato: 'ff6347',
        turquoise: '40e0d0',
        violet: 'ee82ee',
        violetred: 'd02090',
        wheat: 'f5deb3',
        white: 'ffffff',
        whitesmoke: 'f5f5f5',
        yellow: 'ffff00',
        yellowgreen: '9acd32',
        windowtext: '000000'
    };

    ASPxClientSpreadsheet.ParseColor = function(colorString) {
        if(colorString && colorString !== "") {
            var regExp = new RegExp("^#?([a-f]|[A-F]|[0-9]){3}(([a-f]|[A-F]|[0-9]){3})?$");
            var color = colorNameArray[colorString.toLowerCase()];
            if(!color) {
                if(regExp.test(colorString))
                    colorString = _aspxGetFullHexColor(colorString);
                color = ASPx.Color.ColorToHexadecimal(colorString);
            }
            else
                color = "#" + color;
            return regExp.test(color) ? color : "";
        }
        return "";
    };

    // #XXXXXX
    function _aspxGetFullHexColor(colorString) {
        if(colorString == "")
            return null;
        var color = colorString.replace("#", "");
        if(color.length == 3) {
            var newColor = "";
            for(var i = 0 ; i < 3; i++)
                newColor += color.charAt(i) + color.charAt(i);

            color = newColor;
        }
        return "#" + color;
    }
})();