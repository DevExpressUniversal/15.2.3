(function(window) {
    /*class DivSearcher*/
    function dx_DivSearcher(mainContentElement, text, searchUp, mcase, wholeWord) {
        this.mainContentElement = mainContentElement;
        this.doc = ASPx.GetElementDocument(mainContentElement);
        this.win = this.doc.defaultView || this.doc.parentWindow;
        this.text = text;
        this.up = searchUp;
        this.mcase = mcase;
        this.wholeWord = wholeWord;
        this.currentElementIndex;
        this.currentElementCharOffset;
        this.lastTriedCheckPointIndex;
        this.lastTriedCheckPointCharOffset;
        this.currentElement;
        this.currentElementTextContent;

        this.search = function() {
            var nobrs = this.mainContentElement.getElementsByTagName("nobr");
            if(!mcase)
                this.text = this.text.toLowerCase();
            var matched = this.searchCore(nobrs);
            if(matched) {
                var currentContainer = this.getTextNodeFromElement(nobrs[this.lastTriedCheckPointIndex]);
                var otherContainer = this.getTextNodeFromElement(nobrs[this.currentElementIndex]);
                var currentOffset = this.lastTriedCheckPointCharOffset;
                var otherOffset = this.currentElementCharOffset;
                return { "startContainer": (this.up ? otherContainer : currentContainer), "startOffset": (this.up ? otherOffset : currentOffset), "endContainer": (this.up ? currentContainer : otherContainer), "endOffset": (this.up ? currentOffset : otherOffset) + 1 };
            }
            return null;
        };
        this.getIndexOf = function(cortege, element) {
            for(var index in cortege) {
                if(cortege[index] === element.parentNode)
                    return index;
            }
            return null;
        };
        this.decreaseOrNull = function(value) {
            return value == 0 ? 0 : (value - 1);
        };
        this.searchCore = function(elements) {
            if(elements.length == 0 || this.text.length == 0) {
                return false;
            }
            var textLength = this.text.length;
            var selection = this.win.getSelection();
            var range;
            var resetArrayElementIndex = function(array, up) {
                return up ? array.length - 1 : 0;
            };
            if(selection.rangeCount != 0 && (range = selection.getRangeAt(0)) && this.mainContentElement.contains(range.startContainer) && range.startContainer !== this.mainContentElement) {
                this.currentElementIndex = this.getIndexOf(elements, this.up ? range.endContainer : range.startContainer);
                if(!this.currentElementIndex)
                    return false;
                this.currentElementCharOffset = this.up ? this.decreaseOrNull(range.startOffset) : range.endOffset;
                selection.removeAllRanges();
            } else {
                this.currentElementIndex = resetArrayElementIndex(elements, this.up);
                this.currentElementCharOffset = this.up ? this.getLastIndex(elements[this.currentElementIndex].textContent) : 0;
            }
            this.currentElement = elements[this.currentElementIndex];
            this.currentElementTextContent = this.currentElement ? this.currentElement.textContent : "";
            if(this.up ? this.currentElementCharOffset == 0 : this.currentElementCharOffset >= this.currentElementTextContent.length) {
                this.currentElementCharOffset = this.up ? 0 : this.decreaseOrNull(this.currentElementTextContent.length);
                if(!this.nextSearchPointStep(elements, this.up))
                    return false;
                else {
                    this.currentElement = elements[this.currentElementIndex];
                    this.currentElementTextContent = this.currentElement ? this.currentElement.textContent : "";
                }
            }

            for(var searchTextCharIndex = resetArrayElementIndex(this.text, this.up) ; this.up ? searchTextCharIndex >= 0 : searchTextCharIndex < textLength;) {
                if(this.up ? searchTextCharIndex == (textLength - 1) : (searchTextCharIndex == 0)) {
                    this.lastTriedCheckPointIndex = this.currentElementIndex;
                    this.lastTriedCheckPointCharOffset = this.currentElementCharOffset;
                }
                var char = this.currentElementTextContent[this.currentElementCharOffset];
                var equal = !this.isWhiteSpace(this.text[searchTextCharIndex]) ? this.text[searchTextCharIndex] === (this.mcase ? char : char.toLowerCase()) : this.isWhiteSpace(this.currentElementTextContent[this.currentElementCharOffset]);
                if(equal) {
                    if(this.isLastItem(searchTextCharIndex, this.text, this.up))
                        return true;
                    else if(this.isLastItem(this.currentElementCharOffset, this.currentElementTextContent, this.up)) {
                        equal = this.isWhiteSpace(this.text[this.up ? --searchTextCharIndex : ++searchTextCharIndex]);
                        if(equal && this.isLastItem(searchTextCharIndex, this.text, this.up))
                            return true;
                    }
                }
                if(equal) {
                    if(!this.nextSearchPointStep(elements, this.up))
                        return false;
                    this.up ? --searchTextCharIndex : ++searchTextCharIndex;
                } else {
                    searchTextCharIndex = resetArrayElementIndex(this.text, this.up);
                    this.currentElementCharOffset = this.lastTriedCheckPointCharOffset;
                    this.currentElementIndex = this.lastTriedCheckPointIndex;
                    if(!this.nextSearchPointStep(elements, this.up))
                        return false;
                }
            }
        };
        this.getLastIndex = function(array) {
            return (!array || array.length === 0) ? 0 : array.length - 1;
        };
        this.getTextNodeFromElement = function(element) {
            for(var i = 0; i < element.childNodes.length; i++) {
                var curNode = element.childNodes[i];
                if(curNode.nodeType == 3)
                    return curNode;
            }
        };
        this.nextSearchPointStep = function(elements, up) {
            if(this.isLastItem(this.currentElementCharOffset, this.currentElementTextContent, up)) {
                if(this.isLastItem(this.currentElementIndex, elements, up))
                    return false;
                else {
                    this.currentElement = elements[up ? --this.currentElementIndex : ++this.currentElementIndex];
                    this.currentElementTextContent = this.currentElement ? this.currentElement.textContent : "";
                    this.currentElementCharOffset = up ? this.getLastIndex(elements[this.currentElementIndex].textContent) : 0;
                }
            } else {
                up ? this.currentElementCharOffset-- : this.currentElementCharOffset++;
            }
            return true;
        };
        this.isLastItem = function(offset, array, up) {
            return !isNaN(parseInt(offset)) && array && ((parseInt(offset, 10) + 1 === array.length) && !up || up && (parseInt(offset, 10) === 0));
        };
        this.isWhiteSpace = function(stringChar) {
            return !/\S/.test(stringChar);
        };
    }

    /* class Range*/
    function dx_Range(element, searchUp, useDivContent) {
        this.element = element;
        this.doc = ASPx.GetElementDocument(element);
        this.win = this.doc.defaultView || this.doc.parentWindow;
        this.storedSelections = [];
        this.useDivContent = useDivContent;

        this.empty = function() {
            if(!ASPx.Browser.MacOSMobilePlatform)
                return;
            this.storeCurrentSelection();
            for(var index = 0; index < this.storedSelections.length; index++) {
                var selection = this.win.getSelection();
                if(selection) {
                    selection.removeAllRanges();
                    var rangeValues = this.storedSelections.pop();
                    var range = this.doc.createRange();
                    range.setStart(rangeValues.startContainer, rangeValues.startOffset);
                    range.setEnd(rangeValues.endContainer, rangeValues.endOffset);
                    selection.addRange(range);
                    this.hilightSelected("transparent");
                }
            }
            this.restoreLastStoredSelection();
            this.storedSelections = [];
            this.lastStoredSelection = null;
        };
        this.clearLastStoredSelection = function() {
            this.lastStoredSelection = null;
        };
        this.restoreLastStoredSelection = function(up) {
            if(this.lastStoredSelection) {
                var restoredSelection = this.setSelection(this.lastStoredSelection, up);
                var range;
                if(restoredSelection && restoredSelection.rangeCount !== 0) {
                    range = restoredSelection.getRangeAt(0);
                    restoredSelection.removeAllRanges();
                    restoredSelection.addRange(range);
                    range.collapse(up);
                }
                return range;
            }
        };
        this.storeCurrentSelection = function(lastStoredSelection) {
            if(lastStoredSelection) {
                this.lastStoredSelection = lastStoredSelection;
                return;
            }
            var selection = this.win.getSelection();
            if(selection.rangeCount !== 0) {
                var range = selection.getRangeAt(0);
                this.lastStoredSelection = { "startContainer": range.startContainer, "startOffset": range.startOffset, "endContainer": range.endContainer, "endOffset": range.endOffset };
            }
        };
        this.setSelection = function(rangeValues) {
            var selection = this.win.getSelection();
            selection.removeAllRanges();
            var range = this.doc.createRange();
            range.setStart(rangeValues.startContainer, rangeValues.startOffset);
            range.setEnd(rangeValues.endContainer, rangeValues.endOffset);
            selection.addRange(range);
            return selection;
        };
        this.hilightSelected = function(color) {
            this.doc.designMode = "on";
            if(!this.doc.execCommand("HiliteColor", false, color))
                this.doc.execCommand("BackColor", false, color);
            this.doc.designMode = "off";
        };
        this.findText = function(text, mword, mcase, up, isServerCallback) {
            var selection, range;
            if(isServerCallback) {
                if(up) {
                    this.empty();
                    range = this.doc.createRange();
                    range.selectNodeContents(this.element);
                    range.collapse(false);
                    selection = this.win.getSelection();
                    selection.removeAllRanges();
                    selection.addRange(range);
                }
            } else {
                this.empty();
            }
            var searchResult;
            if(this.useDivContent) {
                var searcher = new dx_DivSearcher(this.element, text, up, mcase, mword);
                isServerCallback ? this.clearLastStoredSelection() : this.restoreLastStoredSelection(up);
                if(searchResult = searcher.search()) {
                    this.setSelection(searchResult, up);
                    this.scrollTo(searchResult.startContainer.parentElement);
                    this.storeCurrentSelection(searchResult);
                } else {
                    this.clearLastStoredSelection();
                }
            } else {
                searchResult = this.win.find(text, mcase, up, false, false);
                if(searchResult && ASPx.Browser.MacOSMobilePlatform) {
                    this.hilightSelected("yellow");
                    selection = this.win.getSelection();
                    range = selection.getRangeAt(0);
                    var currentRangeInfo = { "startContainer": range.startContainer, "startOffset": range.startOffset, "endContainer": range.endContainer, "endOffset": range.endOffset };
                    this.storedSelections.push(currentRangeInfo);
                    range.collapse(up);
                    selection.removeAllRanges();
                    selection.addRange(range);
                }
            }
            return searchResult;
        };
        this.scrollTo = function(element) {
            if(element && element.scrollIntoView)
                element.scrollIntoView(false);
        };
    }

    /* class TextRange */
    function dx_TextRange(element, searchUp) {
        this.selText = "";
        this.element = element;
        this.range = ASPx.GetElementDocument(element).body.createTextRange();
        this.range.moveToElementText(element);
        this.originalRange = this.range.duplicate();
        //if(searchUp) this.range.moveStart("textedit", 1);

        this.empty = function() {
            if(this.element.document && this.element.document.selection)
                this.element.document.selection.empty();
        };
        this.select = function(text) {
            this.range.select();
            this.selText = text;
        };
        this.findText = function(text, mword, mcase, up, isServerCallback) {
            if(!text || text.length == 0)
                return true;
            var fl = this.getFlags(mword, mcase);
            var val = up ? this.findUp(text, fl) : this.findDown(text, fl);
            if(val) this.select(text);
            return val;
        };
        this.getFlags = function(mword, mcase) {
            var fl = 0;
            if(mword) fl += 2;
            if(mcase) fl += 4;
            return fl;
        };
        this.findUp = function(text, fl) {
            this.range.moveEnd("character", -this.selText.length);
            var val = this.range.findText(text, -1000, fl) && this.originalRange.inRange(this.range);
            if(!val) this.range.moveEnd("character", this.selText.length);
            return val;
        };
        this.findDown = function(text, fl) {
            this.range.moveStart("character", this.selText.length);
            var val = this.range.findText(text, 1000, fl) && this.originalRange.inRange(this.range);
            if(!val) this.range.moveStart("character", -this.selText.length);
            return val;
        };
    }

    ASPx.dx_Range = dx_Range;
    ASPx.dx_TextRange = dx_TextRange;
})(window);