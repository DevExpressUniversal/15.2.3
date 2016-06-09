(function() {
    var SearchConstants = {
        EntryContextRadius: 15,
        Ellipsis: "...",
        VirtualSearchAreaSize: 1000,
        MaxEntryCount: 500,
        ScrollPadding: 10,
        DisplayPosition: ASPx.HtmlEditorClasses.DisplayPositon.Top,
        ReplaceBatchSize: 100,
        ReplaceBatchDelay: 5,
        MaxSilentActionDuration: 1000
    };

    var CssClasses = {
        CurrentEntry: "dxhe-searchCurrent",
        SearchContainer: "dxhe-searchContainer",
        Highlight: "dxhe-searchHI",
        CMHighlight: "dxhe-searchCMHI"
    };

    var SearchDirection = {
        Downward: 0,
        Upward: 1
    };

    function addTextNode(nodeCollection, doc, str, startPos, endPos) {
        var text = str.substring(startPos, endPos);
        var newNode = null;
        if(text) {
            var newNode = doc.createTextNode(text);
            nodeCollection.push(newNode);
        }
        return newNode;
    };

    function iterateCollection(collection, callback) {
        for(var i = 0; i < collection.length; i++)
            callback(collection[i]);
    };

    function getTime() {
        return (new Date()).getTime();    
    }

    function getPrevNode(node) {
        return node.previousSibling || node.parentNode.previousSibling;
    };

    function getNextNode(node) {
        return node.nextSibling || node.parentNode.nextSibling;
    };

    function getNodeText(node) {
        var text = node.textContent || node.innerText || node.data || "";
        return text === "\n" ? " " : text;
    };

    var spaceSymbol = String.fromCharCode(32);

    function getNodeTrimmedText(node) {
        return getTrimmedText(getNodeText(node) || "");
    };

    function getTrimmedText(text) {
        if(text) {
            text = text.replace(/(\r\n|\n|\r)/gm, "");
            text = text.replace(/\s{2,}/gm, spaceSymbol);
        }
        return text;        
    };

    function isAllowedNode(node) {
        return node.nodeType == 1 || node.nodeType == 3;    
    };

    function isWordPart(node) {
        return isTextNode(node) || isInlineElement(node);
    };

    function isEditor(node) {
        switch(node.tagName) {
            case "INPUT":
            case "TEXTAREA":
            case "SELECT":
            case "BUTTON":
            case "OUTPUT":
            case "OPTION":
            case "OPTGROUP":
            case "DATALIST":
            case "KEYGEN":
            case "PROGRESS":
                return true;
            default:
                return false;
        }
    };

    function isTextNode(node) {
        return node.nodeType == 3;    
    };

    function isInlineElement(node) {
        if(node.nodeType != 1)
            return false;
        switch(node.tagName.toLowerCase()) {
            case "b":
            case "big":
            case "i":
            case "small":
            case "tt":
            case "abbr":
            case "acronym":
            case "cite":
            case "em":
            case "strong":
            case "a":
            case "q":
            case "span":
            case "label":
                return true;
            default:
                return false;
        }
    };

    function getClientRect(clientRects) {
        var clientRect = {
            top: null,
            right: null,
            bottom: null,
            left: null,
            width: null,
            height: null
        };
        iterateCollection(clientRects, function(rect) {
            clientRect.top = Math.ceil(clientRect.top === null ? rect.top : Math.min(rect.top, clientRect.top));
            clientRect.left = Math.ceil(clientRect.left === null ? rect.left : Math.min(rect.left, clientRect.left));
            clientRect.right = Math.ceil(clientRect.right === null ? rect.right : Math.max(rect.right, clientRect.right));
            clientRect.bottom = Math.ceil(clientRect.bottom === null ? rect.bottom : Math.max(rect.bottom, clientRect.bottom));
        });
        clientRect.width = Math.ceil(clientRect.right - clientRect.left);
        clientRect.height = Math.ceil(clientRect.bottom - clientRect.top);
        return clientRect;
    };

    function getWordEntries(text, word) {
        var startIndex = -1;
        var endIndex = 0;
        var ranges = [];
        while ((startIndex = text.indexOf(word, endIndex)) != -1) {
            endIndex = startIndex + word.length;
            var range = { start: startIndex, end: endIndex };
            ranges.push(range);
        }
        return ranges;
    };

    function stripHighlightElements(container) {
        if(container) {
            var highlightElements = ASPx.GetNodesByClassName(container, CssClasses.Highlight);
            while(highlightElements.length) {
                var elementToDelete = highlightElements.pop();
                while(elementToDelete.firstChild) {
                    if(isTextNode(elementToDelete.firstChild))
                        ASPx.InsertElementAfter(elementToDelete.firstChild, elementToDelete);
                    else
                        ASPx.RemoveElement(elementToDelete.firstChild);
                }
                ASPx.RemoveElement(elementToDelete);
            }
        }   
    };

    var Part = ASPx.CreateClass(null, {
        constructor: function(text, element) {
            this.text = text;
            this.element = element;

            this.wrapper = null;
            this.backgroundElement = null;
        },
        initialize: function() {
            this.createWrapper();
        },
        getLength: function() {
            return this.text.length;
        },
        setText: function(text) {
            if(!text) {
                ASPx.RemoveElement(this.getOuterElement());
                this.wrapper = null;
                this.backgroundElement = null;
            }
            else
                this.element.data = text;
        },
        insertAfter: function(text) {
            var textNode = this.element.ownerDocument.createTextNode(text);
            var leftSibling = this.getOuterElement();
            ASPx.InsertElementAfter(textNode, leftSibling);
        },
        getOuterElement: function(element) {
            element = element || this.element;
            var parentNode = element.parentNode;
            for(var i = 0; i < parentNode.childNodes.length; i++) {
                var child = parentNode.childNodes[i];
                var isEmptyTextNode = child.nodeType == 3 && !child.data;
                if(child != element && !isEmptyTextNode && child != this.wrapper && child != this.backgroundElement)
                    return element;
            }
            return this.getOuterElement(parentNode);
        },
        getClientRect: function() {
            return getClientRect([this.wrapper.getBoundingClientRect()]);
        },
        createWrapper: function() {
            var textNode = this.element;
            var doc = textNode.parentNode.ownerDocument;
            var wrapper = doc.createElement("SPAN");
            ASPx.InsertElementAfter(wrapper, textNode);
            wrapper.outerHTML = '<span class="' + CssClasses.Highlight + '">' + this.text + '<span></span></span>';

            this.wrapper = textNode.nextSibling;
            this.element = this.wrapper.childNodes[0];
            this.backgroundElement = this.wrapper.childNodes[1];
            ASPx.RemoveElement(textNode);
        },
        prepareBackground: function(wordClientRect) {
            var partClientRect = this.getClientRect();
            this.backgroundElement.style.height = wordClientRect.height + "px";
            this.backgroundElement.style.top = (wordClientRect.top - partClientRect.top);
        },
        highlightPart: function(isActive) {
            if(isActive)
                ASPx.AddClassNameToElement(this.wrapper, CssClasses.CurrentEntry);
            else
                ASPx.RemoveClassNameFromElement(this.wrapper, CssClasses.CurrentEntry);
        },
        restoreHierarchy: function() {
            if(!this.wrapper)
                return;
            ASPx.RemoveElement(this.backgroundElement);
            if (this.wrapper.parentNode)
                while (this.wrapper.firstChild)
                    this.wrapper.parentNode.insertBefore(this.wrapper.firstChild, this.wrapper);
            ASPx.RemoveElement(this.wrapper);
        }
    });

    var EntryBase = ASPx.CreateClass(null, {
        constructor: function(owner) {
            this.owner = owner;   
            this.contextHtml = this.createContextHtml(); 
        },
        restoreHighlight: function() {
            this.highlightWord(false);
        },
        show: function() {
            this.highlightWord(true);
            if (!ASPx.Browser.IE)
                this.selectWord(true);
            this.scrollToWord();
        },
        replace: function(text) {
            if(!this.hasBeenReplaced) {
                this.owner.lockAndPerformAction(function() {
                    this.replaceInternal(text || "");        
                }.aspxBind(this));
                this.hasBeenReplaced = true;
            }
        },
        getContextHtml: function() {
            return this.contextHtml;
        },
        createContextHtml: function(leftRadius, rightRadius) {
            var state = {
                hasLeftEllipsis: false,
                hasRightEllipsis: false
            };

            if(!ASPx.IsExists(leftRadius))
                leftRadius = SearchConstants.EntryContextRadius;
            var leftPart = this.getLeftSideText(state, leftRadius);

            if(!ASPx.IsExists(rightRadius))
                rightRadius = leftRadius * 2 - leftPart.length;
            var rightPart = this.getRightSideText(state, rightRadius);

            var result = getTrimmedText(ASPx.Str.EncodeHtml(leftPart) + this.getContextWordHtml() + ASPx.Str.EncodeHtml(rightPart));
            if(state.hasLeftEllipsis)
                result = SearchConstants.Ellipsis + result;
            if(state.hasRightEllipsis)
                result += SearchConstants.Ellipsis;
            return result;
        },
        highlightWord: function (isCurrent) { },
        selectWord: function (isCurrent) { },
        scrollToWord: function() { },
        replaceInternal: function(text) { },
        getContextWordHtml: function() { },
        getLeftSideText: function(state, radius) { },
        getRightSideText: function(state, radius) { }
    });

    var Entry = ASPx.CreateClass(EntryBase, {
        constructor: function(parts, owner) {
            this.parts = parts;
            this.constructor.prototype.constructor.call(this, owner);
            this.initialize();
        },
        initialize: function() {
            iterateCollection(this.parts, function(part) { part.initialize(); });
        },
        getContextWordHtml: function() {
            var text = "";
            iterateCollection(this.parts, function(part) { text += part.text; });
            return "<b>" +  ASPx.Str.EncodeHtml(text) + "</b>";
        },
        getLeftSideText: function(state, radius) {
            return this.appendText(getPrevNode(this.parts[0].element), "", true, getPrevNode, state, radius);
        },
        getRightSideText: function(state, radius) {
            return this.appendText(getNextNode(this.parts[this.parts.length - 1].element), "", false, getNextNode, state, radius);
        },
        appendText: function(node, str, isPrev, nodeGetter, state, radius) {
            if(!node || !isWordPart(node))
                return str;
            str = isPrev ? (getNodeTrimmedText(node) + str) : (str + getNodeTrimmedText(node));
            if(str.length > radius) {
                var startIndex = isPrev ? (str.length - radius) : 0;
                state.hasLeftEllipsis = isPrev || state.hasLeftEllipsis;
                state.hasRightEllipsis = !isPrev || state.hasRightEllipsis;
                return str.substring(startIndex, startIndex + radius);
            } else if(!((isPrev ? node.previousSibling : node.nextSibling) || isWordPart(node.parentNode)))
                return str; 
            return this.appendText(nodeGetter(node), str, isPrev, nodeGetter, state, radius);   
        },
        replaceInternal: function(replaceText) {
            var index = 0;
            iterateCollection(this.parts, function(part) {
                var nextIndex = index + part.getLength();
                part.setText(replaceText.substring(index, nextIndex));
                index = nextIndex;
            });
            var text = replaceText.substring(index, replaceText.length);
            if(text)
                this.parts[this.parts.length - 1].insertAfter(text);
        },
        replaceWithRichText: function() {
                
        },
        getClientRect: function() {
            var clientRects = [];
            iterateCollection(this.parts, function(part) { clientRects.push(part.getClientRect()); });
            return getClientRect(clientRects);
        },
        createHighlighing: function() {
            if(this.parts.length > 1) {
                var clientRect = this.getClientRect();
                iterateCollection(this.parts, function(part) { part.prepareBackground(clientRect); });
            }
        },
        restoreHierarchy: function() {
            iterateCollection(this.parts, function(part) { part.restoreHierarchy(); });
        },
        selectByBookmarks: function(bookmarks) {
            var selection = this.owner.selectionManager.selection;
            if (selection) {
                selection.clientSelection.SelectExtendedBookmark(bookmarks);
            }
        },
        getSelectionBookmarks: function() {
            var isEntryDeleted = this.parts[0].wrapper.parentNode;
            if (isEntryDeleted) {
                return ASPx.HtmlEditorClasses.Selection.getBookmarkByElements(this.owner.getDocument(), this.parts[0].wrapper, this.parts[this.parts.length - 1].wrapper);
            }
            return null;
        },
        selectWord: function(isActive) {
            if (isActive && !this.owner.isInFocus) {
                var bookmarks = this.getSelectionBookmarks();
                if (bookmarks) {
                    this.selectByBookmarks(bookmarks);
                    this.owner.raiseSelectionChanged(this.owner);
                }
            }
        },
        highlightWord: function(isActive) {
            iterateCollection(this.parts, function(part) { part.highlightPart(isActive); });
        },
        scrollToWord: function () {
            ASPx.HtmlEditorClasses.scrollHelper.scrollTo(this.parts[0].wrapper, SearchConstants.ScrollPadding, SearchConstants.DisplayPosition);
            raiseScrollEvent();
        }
    });

    var ReplaceEntry = ASPx.CreateClass(Entry, {
        constructor: function(parts, owner, replaceMachine) {
            this.constructor.prototype.constructor.call(this, parts, owner);
            this.replaceMachine = replaceMachine;
            this.isReplaced = false;
        },
        initialize: function() { },
        createContextHtml: function() { },
        replaceInternal: function(text) {
            Entry.prototype.replaceInternal.call(this, text);
            this.replaceMachine.onEntryReplaced();
        }
    });

    var CMEntry = ASPx.CreateClass(EntryBase, {
        constructor: function(codeMirror, position, owner, line) {
            this.codeMirror = codeMirror;
            this.line = line;
            this.from = CodeMirror.Pos(position.line, position.from);
            this.to = CodeMirror.Pos(position.line, position.to);
            this.constructor.prototype.constructor.call(this, owner);
        },
        getLeftSideText: function(state, radius) {
            var result = this.line.substring(this.from.ch - radius, this.from.ch);
            state.hasLeftEllipsis = this.from.ch > radius;
            return result;
        },
        getRightSideText: function(state, radius) {
            var result = this.line.substring(this.to.ch, this.to.ch + radius);
            state.hasRightEllipsis = this.line.length - this.to.ch > + radius;
            return result;
        },
        getContextWordHtml: function() {
            return "<b>" +  ASPx.Str.EncodeHtml(this.line.substring(this.from.ch, this.to.ch)) + "</b>";
        },
        scrollToWord: function() {
            this.codeMirror.scrollIntoView({ from: this.from, to: this.to }, SearchConstants.ScrollPadding);    
        },
        createHighlighing: function() {
            this.highlightWord(false);    
        },
        selectWord: function(isActive) {
            if (isActive && !this.owner.isInFocus) {
                this.owner.selectionManager.setSelection(this.from, this.to);
                this.owner.raiseSelectionChanged(this.owner);
            }
        },
        highlightWord: function(isActive) {
            var className = CssClasses.CMHighlight;
            if(isActive)
                className += " " + CssClasses.CurrentEntry;
            this.createMark(className);
        },
        createMark: function(className) {
            this.removeMark();
            this.mark = this.codeMirror.markText(this.from, this.to, { className: className });     
        },
        removeMark: function() {
            if(this.mark) {
                this.mark.className = "";
                this.mark.changed();
                this.mark.clear();
                this.mark = null;    
            }
        },
        restoreHierarchy: function() {
            this.removeMark();
            if(this.codeMirror.doc.lineOffsets)
                this.codeMirror.doc.lineOffsets = null;
        },
        replaceInternal: function(text) {
            this.restoreHighlight();
            this.updateOffset(text);
            this.codeMirror.doc.replaceRange(text, this.from, this.to);
        },
        updateOffset: function(text) {
            var doc = this.codeMirror.doc;
            var offset = doc.lineOffsets ? doc.lineOffsets[this.from.line] || 0 : 0;
            this.from.ch += offset;
            this.to.ch += offset;
            (doc.lineOffsets || (doc.lineOffsets = []))[this.from.line] = offset + (text.length - (this.to.ch - this.from.ch));    
        }
    });

    var CMSearchMachine = ASPx.CreateClass(null, {
        constructor: function(owner, stringModifier) {
            this.owner = owner;
            this.codeMirror = owner.sourceEditor;
            this.stringModifier = stringModifier;
        },
        findAll: function(searchWord) {
            var entries = [];
            searchWord = this.getStr(searchWord);
            this.codeMirror.operation(function() {
                var doc = this.codeMirror.doc;
                var lineCount = doc.lineCount();
                for(var i = 0; i < lineCount; i++) {
                    var lineText = this.getStr(doc.getLine(i));
                    var entryRanges = getWordEntries(lineText, searchWord);
                    for(var j = 0; j < entryRanges.length; j++) {
                        var entryRange = entryRanges[j];
                        entries.push(new CMEntry(this.codeMirror, { line: i, from: entryRange.start, to: entryRange.end }, this.owner, lineText));
                    }
                }
            }.aspxBind(this));
            return entries; 
        },
        getStr: function(str) {
            return this.stringModifier(str);
        }
    });

    var SearchMachineBase = ASPx.CreateClass(null, {
        constructor: function() {
            this.wordPartsArray = [];
            this.searchAreasCollection = [];
            this.foundEntries = [];
            this.nodeSplitInfo = {};
            this.wordCount = 0;
        },
        addWordParts: function(wordIndex, node, text) {
            var wordContainer = (this.wordPartsArray[wordIndex] || (this.wordPartsArray[wordIndex] = []));
            wordContainer.push(new Part(text, node));
        },
        findAll: function(containers, searchWord) {
            while(containers.length)
                this.processContainer(containers.shift());
            this.performSearch(searchWord);
            iterateCollection(this.wordPartsArray, function(parts) { this.foundEntries.push(this.createEntry(parts)); }.aspxBind(this));
            return this.foundEntries;
        },
        getFoundEntryRanges: function (searchAreas, searchWord) {
            var fullText = this.prepareStringBeforeSearch(this.getSearchAreasFullText(searchAreas));
            searchWord = this.prepareStringBeforeSearch(searchWord);

            var startIndex = -1;
            var endIndex = -1;
            var foundEntryRanges = [];
            while ((startIndex = fullText.indexOf(searchWord, endIndex + 1)) != -1) {
                endIndex = startIndex + searchWord.length - 1;
                foundEntryRanges.push({ id: this.wordCount++, start: startIndex, end: endIndex });
            }
            return foundEntryRanges;
        },
        performSearch: function(searchWord) {
            iterateCollection(this.searchAreasCollection, function(searchAreas) {
                var foundEntryRanges = this.getFoundEntryRanges(searchAreas, searchWord);
                var offset = 0;
                iterateCollection(searchAreas, function(searchArea) {
                    var nodeEndIndex = offset + searchArea.text.length - 1;
                    var nodeInnerRanges = [];

                    iterateCollection(foundEntryRanges, function(range) {
                        if((range.start >= offset && range.start <= nodeEndIndex) || (range.end >= offset && range.end <= nodeEndIndex) || (offset >= range.start && offset <= range.end))
                            nodeInnerRanges.push(range);
                    });

                    if (nodeInnerRanges.length && searchArea.text) {
                        var newNodes = [];
                        var tmpText = searchArea.text;
                        var headIndex = 0;
                        var doc = searchArea.node.ownerDocument;

                        iterateCollection(nodeInnerRanges, function(nodeInnerRange) {
                            var start = Math.max(nodeInnerRange.start, offset) - offset;
                            var end = Math.min(nodeInnerRange.end, nodeEndIndex) - offset + 1;
                            addTextNode(newNodes, doc, tmpText, headIndex, start);
                            var nodeText = tmpText.substring(start, end);
                            var newNode = doc.createTextNode(nodeText);
                            newNodes.push(newNode);
                            this.addWordParts(nodeInnerRange.id, newNode, nodeText);
                            headIndex = end;

                        }.aspxBind(this));

                        addTextNode(newNodes, doc, tmpText, headIndex, tmpText.length);
                        iterateCollection(newNodes, function(newNode) {
                            searchArea.node.parentNode.insertBefore(newNode, searchArea.node)
                        });
                        ASPx.RemoveElement(searchArea.node);
                    }
                    offset += searchArea.text.length;
                }.aspxBind(this));

            }.aspxBind(this));
        },
        getSearchAreasFullText: function(searchAreas) {
            var fullText = "";
            iterateCollection(searchAreas, function(searchArea) { fullText += searchArea.text; });
            return fullText;
        },
        createSearchArea: function() {
            var searchAreas = [];
            this.searchAreasCollection.push(searchAreas);
        },
        getSearchArea: function() {
            if(!this.searchAreasCollection.length)
                this.createSearchArea();
            return this.searchAreasCollection[this.searchAreasCollection.length - 1];
        },
        processContainer: function(node) {
            node.normalize();
            this.getSearchArea().push({ node: node, text: getNodeText(node) });
        },
        createEntry: function(parts) {
            return { parts: parts };    
        },
        prepareStringBeforeSearch: function(str) {
            return str;    
        }
    });

    var SearchMachine = ASPx.CreateClass(SearchMachineBase, {
        constructor: function(stringModifier, owner) {
            this.owner = owner;
            this.constructor.prototype.constructor.call(this);
            this.stringModifier = stringModifier;
        },
        prepareStringBeforeSearch: function(str) {
            return this.stringModifier(str);
        },
        createEntry: function(parts) {
            return new Entry(parts, this.owner);
        },
        processContainer: function(node) {
            if(isEditor(node))
                return;
            if(!isTextNode(node)) {
                var isWordBreaker = !isWordPart(node);
                if(isWordBreaker)
                    this.createSearchArea();
                iterateCollection(node.childNodes, function(childNode) { this.processContainer(childNode); }.aspxBind(this));
                if(isWordBreaker)
                    this.createSearchArea();
            } else
                SearchMachineBase.prototype.processContainer.call(this, node);
        }
    });
    var ReplaceMachine = ASPx.CreateClass(SearchMachine, {
        constructor: function(stringModifier, owner) {
            this.constructor.prototype.constructor.call(this, stringModifier, owner);
            this.replaceContainers = [];
            this.entryCount = 0;
        },
        onEntryReplaced: function() {
            this.entryCount--;
            if(this.entryCount == 0) {
                while(this.replaceContainers.length)
                    this.replaceContainers.pop().normalize();
            }
        },
        createEntry: function(parts) {
            this.entryCount++;
            return new ReplaceEntry(parts, this.owner, this);
        },
        findAll: function(containers, searchWord) {
            iterateCollection(containers, function(c) { 
                this.replaceContainers.push(c);
            }.aspxBind(this));
            return SearchMachine.prototype.findAll.call(this, containers, searchWord);
        }
    });

    function getOriginalString(str) {
        return str || "";
    }
    function getLowerCaseString(str) {
        return str ? str.toLowerCase() : "";
    }
    
    var CMSearchStrategy = ASPx.CreateClass(null, {
        constructor: function(stringModifier, params) {
            this.owner = params.wrapper;
            this.stringModifier = stringModifier;
        },
        findAll: function(text) {
            var search = new CMSearchMachine(this.owner, this.stringModifier);
            return search.findAll(text);
        }
    });

    var SimpleSearchStrategy = ASPx.CreateClass(null, {
        constructor: function(stringModifier, params) {
            this.owner = params.wrapper;
            this.stringModifier = stringModifier;
            this.container = params.container;
            this.skipSelectedElement = params.skipSelectedElement;
            this.selectedElement = this.getSelectedElement(params);
        },
        getSelectedElement: function(params) {
            var selectedElement = params.selectedElement;
            var container = params.container;
            if(selectedElement == container || !selectedElement || (selectedElement && selectedElement.ownerDocument != container.ownerDocument))
                selectedElement = container.firstChild;
            return selectedElement;
        },
        findAll: function(text) {
            if(!this.selectedElement)
                return [];
            stripHighlightElements(this.container);
            return this.findAllInternal(text);
        },
        findAllInternal: function(text) {
            return this.findInContainers(text);
        },
        findInContainers: function(text) {
            var containers = this.getSearchContainers();
            if(!containers)
                return [];
            var searchMachine = new SearchMachine(this.getStringModifier(), this.owner);
            return searchMachine.findAll(this.getSearchContainers(), text);
        },
        getSearchContainers: function() {
            return [this.container];
        },
        getStringModifier: function() {
            return this.stringModifier;
        }
    });
    var DirectionSearchStrategy = ASPx.CreateClass(SimpleSearchStrategy, {
        getSearchContainers: function() {
            return this.getSearchContainersInternal();
        },
        getSearchContainersInternal: function() {
            var result = [];
            if(!this.skipSelectedElement)
                result.push(this.selectedElement);
            var tmpNode;

            var hierarchy = [this.selectedElement];
            tmpNode = this.selectedElement.parentNode;
            while(tmpNode && tmpNode != this.container) {
                hierarchy.push(tmpNode);
                tmpNode = tmpNode.parentNode;
            }
            iterateCollection(hierarchy, function(element) {
                tmpNode = this.getSibling(element);
                while(tmpNode) {
                    this.addSibling(tmpNode, result);
                    tmpNode = this.getSibling(tmpNode);
                }
            }.aspxBind(this));
            return result;
        },
        getAdditionalEntries: function(text) {
            var searchStrategyClass = this.getAdditionalEntriesSearchStrategy();
            var strategy = new searchStrategyClass(this.getStringModifier(), {
                container: this.container,
                selectedElement: this.selectedElement,
                skipSelectedElement: true,
                wrapper: this.owner
            });
            return strategy.findInContainers(text);
        },
        getAdditionalEntriesSearchStrategy: function() { },
        getSibling: function(element) { },
        addSibling: function(element, container) { }
    });
    var DownwardSearchStrategy = ASPx.CreateClass(DirectionSearchStrategy, {
        findAllInternal: function(text) {
            var belowEntries = DirectionSearchStrategy.prototype.findInContainers.call(this, text);
            iterateCollection(this.getAdditionalEntries(text), function(entry) { belowEntries.push(entry); });
            return belowEntries;
        },
        getAdditionalEntriesSearchStrategy: function() {
            return UpwardSearchStrategy;
        },
        getSibling: function(element) {
            return element.nextSibling;
        },
        addSibling: function(element, container) {
            container.push(element);
        }
    });
    var UpwardSearchStrategy = ASPx.CreateClass(DirectionSearchStrategy, {
        findAllInternal: function(text) {
            var aboveEntries = DirectionSearchStrategy.prototype.findInContainers.call(this, text);
            aboveEntries.reverse();
            var belowEntries = this.getAdditionalEntries(text);
            while(belowEntries.length)
                aboveEntries.push(belowEntries.pop());
            return aboveEntries;
        },
        getAdditionalEntriesSearchStrategy: function() {
            return DownwardSearchStrategy;
        },
        getSibling: function(element) {
            return element.previousSibling;
        },
        addSibling: function(element, container) {
            container.unshift(element);
        }
    });

    var MultipleContainersSearchStrategy = ASPx.CreateClass(null, {
        constructor: function(stringModifier, params) {
            this.stringModifier = stringModifier;
            this.owner = params.wrapper;
            this.containers = params.containers;
        },
        findAll: function(text) {
            if(!text || !this.containers || !this.containers.length) 
                return [];
            var searchMachine = this.createSearchMachine();
            return searchMachine.findAll(this.containers, text);
        },
        createSearchMachine: function() {
            return new SearchMachine(this.stringModifier, this.owner);
        }
    });

    var ReplaceSearchStrategy = ASPx.CreateClass(MultipleContainersSearchStrategy, {
        createSearchMachine: function() {
            return new ReplaceMachine(this.stringModifier, this.owner);
        }
    });


    function attachDocEventsIfNeeded(doc) {
        if(!doc.hasSearchEvents) {
            ASPx.Evt.AttachEventToElement(doc, "scroll", function(evt) {
                scrollEvent.FireEvent(doc, evt);                
            });
            doc.hasSearchEvents = true;
        }
    };
    function raiseScrollEvent() {
        if(!scrollEvent.IsEmpty())
            scrollEvent.FireEvent();
    };

    var scrollEvent = new ASPxClientEvent();

    var onDocScroll = function(doc, evt) {
        this.onDocScroll(doc, evt);    
    };

    function isElementVisible (el) {
        var rect = el.getBoundingClientRect();
        var doc = el.ownerDocument;
        var win = doc.defaultView;

        return (
            rect.top >= 0 &&
            rect.left >= 0 &&
            rect.bottom <= (win.innerHeight || doc.documentElement.clientHeight) && 
            rect.right <= (win.innerWidth || doc.documentElement.clientWidth)
        );
    };

    var SplittedArea = ASPx.CreateClass(null, {
        constructor: function(containers, entryCount, stringModifier, searchText, owner, index) {
            this.containers = containers;
            this.entryCount = entryCount;
            this.stringModifier = stringModifier;
            this.searchText = searchText;
            this.owner = owner;
            this.entries = null;
            this.replaceEntries = null;
            this.isDisposed = false;
            this.index = index;
        },
        initialize: function() {
            attachDocEventsIfNeeded(this.containers[0].ownerDocument);
            scrollEvent.AddHandler(onDocScroll, this);
            this.createRealEntriesIfAreaIsVisible();
        },
        dispose: function() {
            if(!this.isDisposed) {
                scrollEvent.RemoveHandler(onDocScroll, this);
                this.isDisposed = true;
            }
        },
        onDocScroll: function(doc, evt) {
            this.createRealEntriesIfAreaIsVisible();
        },
        createRealEntriesIfAreaIsVisible: function() {
            for(var i = 0; i < this.containers.length; i++) {
                if(this.containers[i].nodeType === 1 && isElementVisible(this.containers[i])) {
                    this.getRealEntries();
                    return;
                }
            }
        },
        getRealEntry: function(dontHighlight, dontCreate, index) {
            if(dontCreate && !this.entries)
                return null;
            return this.getRealEntries(dontHighlight)[index];
        },
        getRealEntries: function(dontHighlight) {
            if(!this.entries) {
                this.entries = this.findEntriesWithinStrategy(MultipleContainersSearchStrategy);
                if(!dontHighlight)
                    setTimeout(iterateCollection, 300, this.entries, function(entry) { entry.createHighlighing(); });
                this.dispose();
            }
            return this.entries;
        },
        getReplaceEntry: function(index) {
            var entry = this.getRealEntry(true, true, index);
            if(!entry && !this.replaceEntries)
                this.replaceEntries = this.findEntriesWithinStrategy(ReplaceSearchStrategy);
            return entry || this.replaceEntries[index];
        },
        findEntriesWithinStrategy: function(strategyClass) {
            var strategy = new strategyClass(this.stringModifier, {
                wrapper: this.owner,
                containers: this.containers
            });
            return strategy.findAll(this.searchText);
        },
        getEntryCount: function() {
            return this.entryCount;
        }
    });

    var VirtualEntry = ASPx.CreateClass(null, {
        constructor: function(searchArea, entryIndex) {
            this.searchArea = searchArea;
            this.entryIndex = entryIndex;
        },
        getEntry: function(dontHighlight, dontCreate) {
            return this.searchArea.getRealEntry(dontHighlight, dontCreate, this.entryIndex);
        },
        initialize: function() { },
        createHighlighing: function() { },
        getContextHtml: function() {
            return this.getEntry().getContextHtml();
        },
        restoreHierarchy: function() {
            this.searchArea.dispose();
            var entry = this.getEntry(true, true);
            if(entry)
                entry.restoreHierarchy();
        },
        restoreHighlight: function() {
            this.getEntry().restoreHighlight();
        },
        replace: function(text) {
            this.searchArea.getReplaceEntry(this.entryIndex).replace(text);
        },
        replaceInternal: function(text) {
            this.searchArea.getReplaceEntry(this.entryIndex).replaceInternal(text);
        },

        show: function() {
            this.getEntry().show();
        },
        selectWord: function() {
            this.getEntry().selectWord();
        }
    });

    var VirtualSearchStrategy = ASPx.CreateClass(null, {
        constructor: function(stringModifier, params) {
            this.stringModifier = stringModifier;
            this.owner = params.wrapper;
            this.container = params.container;
        },
        getMaxCharPerArea: function() {
            return SearchConstants.VirtualSearchAreaSize;    
        },
        findAll: function(text) {
            var result = [];
            if(text) {
                stripHighlightElements(this.container);
                var searchAreas = this.splitContainer(text);
                if(searchAreas && searchAreas.length) {
                    var offset = 0;
                    for(var i = 0; i < searchAreas.length; i++) {
                        var searchArea = searchAreas[i];
                        var entryCount = searchArea.getEntryCount();
                        searchArea.offset = offset;
                        for(var j = 0; j < entryCount; j++)
                            result.push(new VirtualEntry(searchArea, j));
                        offset += entryCount;
                        searchArea.initialize();
                    }
                }
            }
            return result;
        },
        splitContainer: function(text) {
            this.areaBuffer = [];
            this.nodeBuffer = [];
            this.currLength = 0;
            this.currText = "";
            this.maxLength = this.getMaxCharPerArea();
            this.searchText = text;

            this.processContainer(this.container);
            this.createArea();
            return this.areaBuffer;
        },
        processContainer: function(container) {
            var text = getNodeText(container);
            var textLength = text.length;
            this.checkNode(container, text, textLength);

            if(this.canSkip)
                return;
            if((this.needNewArea || !this.canBeAddedToCurrent) && this.isBreaksWord)
                this.createArea();
            if(this.needNewArea && this.currLength == 0) {
                for(var i = 0; i < container.childNodes.length; i++)
                    this.processContainer(container.childNodes[i]);
            } else
                this.addToNodeBuffer(container, text, textLength);
        },
        checkNode: function(node, nodeText, nodeLength) {
            this.canBeAddedToCurrent = nodeLength + this.currLength <= this.maxLength;
            this.needNewArea = nodeLength > this.maxLength;
            this.isWordPart = isWordPart(node);
            this.isBreaksWord = this.canBreaksWord(node, nodeText, nodeLength);
            this.canSkip = !nodeText || (!this.isWordPart && this.indexOf(nodeText, this.searchText) == -1) || !isAllowedNode(node);
        },
        canBreaksWord: function(node, nodeText, nodeLength) {
            if(!this.isWordPart || this.searchText.length == 1)
                return true;

            var testStr = 
                this.currText.substring(this.currText.length - this.searchText.length + 1, this.currText.length) + 
                nodeText.substring(0, this.searchText.length - 1);
            return this.indexOf(testStr, this.searchText) == -1;
        },
        addToNodeBuffer: function(node, nodeText, length) {
            this.nodeBuffer.push(node);
            this.currLength += length;
            this.currText += nodeText;
        },
        createArea: function() {
            if(this.currLength) {
                this.areaBuffer.push(new SplittedArea(this.nodeBuffer,
                    getWordEntries(this.getStr(this.currText), this.getStr(this.searchText)).length,
                    this.stringModifier,
                    this.searchText,
                    this.owner, 
                    this.areaBuffer.length));
            }
            this.nodeBuffer = [];
            this.currText = "";
            this.currLength = 0;        
        },
        indexOf: function(str, substr) {
            return this.getStr(str).indexOf(this.getStr(substr));    
        },
        getStr: function(str) {
            return this.stringModifier(str);        
        }
    });

    function getSearchStrategy(options, params) {
        options = options || Search.DefaultSearchOptions;

        var stringModifier = options.matchCase ? getOriginalString : getLowerCaseString;

        if(params && params.wrapper && params.wrapper.sourceEditor)
            return new CMSearchStrategy(stringModifier, params);

        if(getNodeText(params.container).length > SearchConstants.VirtualSearchAreaSize)
            return new VirtualSearchStrategy(stringModifier, params);

        switch(options.direction) {
            case SearchDirection.Downward:
                return new DownwardSearchStrategy(stringModifier, params);
            case SearchDirection.Upward:
                return new UpwardSearchStrategy(stringModifier, params);
            default:
                return new SimpleSearchStrategy(stringModifier, params);
        }
    };

    function removeWrappersByClass(html, cssClass) {
        var workDoc = ASPx.HtmlEditorClasses.createHtmlDocument();
        ASPx.SetInnerHtml(workDoc.getElementsByTagName("body")[0], html);

        var wrappers = ASPx.GetNodesByClassName(workDoc, cssClass);
            
        while (wrappers.length > 0) {
            if (!ASPx.GetNodesByClassName(workDoc, cssClass)[0])
                break;
            ASPx.GetNodesByClassName(workDoc, cssClass)[0].outerHTML = ASPx.GetNodesByClassName(workDoc, cssClass)[0].innerHTML;
        }

        return workDoc.getElementsByTagName("body")[0].innerHTML;
    };

    function removeSearchInfo(html) {
        html = removeWrappersByClass(html, CssClasses.Highlight);
        html = removeWrappersByClass(html, CssClasses.Background);
        return html;
    };

    var Search = ASPx.CreateClass(null, {
        constructor: function(settings) {
            this.wrapper = settings.wrapper;
            this.text = settings.text;
            this.options = settings.options;

            this.entries = null;
            this.currentEntry = null;
            this.currentIndex = -1;
        },
        getEntryCount: function() {
            return this.entries.length;
        },
        findNext: function() {
            return this.findEntry(+1);
        },
        findPrev: function() {
            return this.findEntry(-1);
        },
        findEntry: function(incValue) {
            if(this.currentIndex == -1)
                this.currentIndex = 0;
            else {
                this.currentIndex += incValue;
                if(this.currentIndex == -1)
                    this.currentIndex = this.entries.length - 1;
                else if(this.currentIndex == this.entries.length)
                    this.currentIndex = 0;
            }
            this.findByIndex(this.currentIndex);
            return this.currentIndex;
        },
        findByIndex: function(index) {
            if(index < 0 || index >= this.entries.length)
                return;
            this.currentIndex = index;
            if(this.currentEntry)
                this.currentEntry.restoreHighlight();
            this.currentEntry = this.entries[this.currentIndex];
            if(this.currentEntry)
                this.currentEntry.show();
        },
        beginSearch: function() {
            this.entries = this.findAll(this.text, this.options, this.getSearchSelectedElement());
            this.prepareEntries();
            var searchContainer = this.getSearchContainer();
            if(searchContainer)
                ASPx.AddClassNameToElement(searchContainer, CssClasses.SearchContainer);        
        },
        prepareEntries: function() {
            iterateCollection(this.entries, function(entry) { entry.createHighlighing(); });
            this.currentEntry = null;
        },
        createBookmarksForCurrentEntry: function() {
            if (this.currentEntry && this.currentEntry.getSelectionBookmarks) {
                return this.currentEntry.getSelectionBookmarks();
            }
            return null;
        },
        setSelectionAccordingCurrentEntry: function () {
            if (this.entries && this.currentEntry) {
                this.currentEntry.selectWord(true);
            }
        },
        endSearch: function (saveSelection) {
            var saveSelection = (saveSelection === undefined) ? true : saveSelection;
            if (saveSelection) {
                var bookmarks = this.createBookmarksForCurrentEntry();
            }
            iterateCollection(this.entries, function (entry) { entry.restoreHierarchy(); });
            var searchContainer = this.getSearchContainer();
            if(searchContainer)
                ASPx.RemoveClassNameFromElement(searchContainer, CssClasses.SearchContainer); 

            if (bookmarks) {
                this.wrapper.getSelection().clientSelection.SelectExtendedBookmark(bookmarks, true);
                this.wrapper.selectionManager.saveSelection();
            }
        },
        getSearchContainer: function() {
            return this.wrapper.getSearchContainer();
        },
        getSearchSelectedElement: function() {
            return this.wrapper.getSearchSelectedElement();    
        },
        findAll: function(text, options, selectedElement) {
            if(!text)
                return [];
            var searchStrategy = getSearchStrategy(options, {
                container: this.getSearchContainer(),
                selectedElement: selectedElement,
                wrapper: this.wrapper
            });
            return searchStrategy.findAll(text);
        },
        replaceAll: function(replaceText, callbacks) {
            if(callbacks)
                this.batchReplace(0, replaceText, callbacks, getTime());
            else {
                this.wrapper.lockAndPerformAction(function() { 
                    iterateCollection(this.entries, function(entry) { entry.replace(replaceText); });
                }.aspxBind(this));
            }
        },
        batchReplace: function(startIndex, replaceText, callbacks, lastTime) {
            var nextStartIndex = startIndex + SearchConstants.ReplaceBatchSize;
            var length = Math.min(this.entries.length, nextStartIndex);
            for(var i = startIndex; i < length; i++) {
                this.entries[i].replaceInternal(replaceText);
                this.entries[i].restoreHierarchy();
            }
            if(getTime() - lastTime > SearchConstants.MaxSilentActionDuration)
                callbacks.onBatchEnd(nextStartIndex);

            if(this.entries.length > nextStartIndex) {
                setTimeout(function() {
                    this.batchReplace(nextStartIndex, replaceText, callbacks, lastTime);
                }.aspxBind(this), SearchConstants.ReplaceBatchDelay);
            } else
                this.wrapper.lockAndPerformAction(function() { callbacks.onEnd(); });
        }
    });

    Search.DefaultSearchOptions = {
        matchCase: false,
        direction: SearchDirection.Downward
    };

    ASPx.HtmlEditorClasses.MultipleContainersSearchStrategy = MultipleContainersSearchStrategy;
    ASPx.HtmlEditorClasses.VirtualSearchStrategy = VirtualSearchStrategy;

    ASPx.HtmlEditorClasses.Search = Search;
    ASPx.HtmlEditorClasses.SearchDirection = SearchDirection;
    ASPx.HtmlEditorClasses.Search.Entry = Entry;
    ASPx.HtmlEditorClasses.Search.Part = Part;
    ASPx.HtmlEditorClasses.Search.SearchMachineBase = SearchMachineBase;
    ASPx.HtmlEditorClasses.Search.RemoveSearchInfo = removeSearchInfo;
    ASPx.HtmlEditorClasses.Search.getTrimmedText = getTrimmedText;
    ASPx.HtmlEditorClasses.Search.SearchConstants = SearchConstants;
    ASPx.HtmlEditorClasses.Search.CssClasses = CssClasses;
})();