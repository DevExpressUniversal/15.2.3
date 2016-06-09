(function() {

    function EditorInfo() {
        this.assign = function(editorText, editorCaretPosition) {
            this.text = editorText;
            this.caretPosition = editorCaretPosition;
        };
    }

    var EditorTextParser = {
        lettersDigitsDotsPattern: "[^~!@#\$%\^&\*\(\)\\-\+=\{\[\\]\}\|:;\"',/\?<>\\\\ \r\n]+",
        digitsDotsAtTheBeginningPattern: "^[0-9\.]+",
        getLettersDigitsDotsAtTheEndPattern: function() {
            return EditorTextParser.lettersDigitsDotsPattern + "$";
        },
        getLettersDigitsDotsAtTheBeginningPattern: function() {
            return "^" + EditorTextParser.lettersDigitsDotsPattern;
        },
        getTextBeforeCaret: function(editorInfo) {
            return editorInfo.text.substring(0, editorInfo.caretPosition);
        },
        getTextAfterCaret: function(editorInfo) {
            return editorInfo.text.substring(editorInfo.caretPosition);
        },
        getTextBeforePartialFunctionNameBeforeCaret: function(editorInfo) {
            var partialFunctionNameBeforeCaret = EditorTextParser.getPartialFunctionNameBeforeCaret(editorInfo);
            return editorInfo.text.substring(0, editorInfo.caretPosition - partialFunctionNameBeforeCaret.length);
        },
        editorTextContainsFormula: function(editorInfo) {
            return EditorTextParser.getCharByIndex(editorInfo.text, 0) === "=";
        },
        getCharByIndex: function(string, index) {
            return string.substring(index, index + 1); // TODO - substring is used only for web server tests - to replace with text[i] after Jasmine test implementation
        },
        getPartialFunctionNameBeforeCaret: function(editorInfo) {
            if(!EditorTextParser.editorTextContainsFormula(editorInfo))
                return "";
            var textBeforeCaret = EditorTextParser.getTextBeforeCaret(editorInfo);
            var lettersDigitsDotsBeforeCaretMatch = new RegExp(EditorTextParser.getLettersDigitsDotsAtTheEndPattern()).exec(textBeforeCaret);
            if(!ASPx.IsExists(lettersDigitsDotsBeforeCaretMatch))
                return "";
            return lettersDigitsDotsBeforeCaretMatch[0].replace(new RegExp(EditorTextParser.digitsDotsAtTheBeginningPattern), "");
        },
        getPartialFunctionNameAfterCaret: function(editorInfo) {
            if(!EditorTextParser.editorTextContainsFormula(editorInfo))
                return "";
            var textAfterCaret = EditorTextParser.getTextAfterCaret(editorInfo);
            var lettersDigitsDotsAfterCaretMatch = new RegExp(EditorTextParser.getLettersDigitsDotsAtTheBeginningPattern()).exec(textAfterCaret);
            return ASPx.IsExists(lettersDigitsDotsAfterCaretMatch) ? lettersDigitsDotsAfterCaretMatch[0] : "";
        },
        removeOpenBracketFromBeginning: function(text) {
            return text.substring(0, 1) === "(" ? text.substring(1) : text;
        },
        modifyEditorInfoByFunctionNameInserting: function(editorInfo, functionName) {
            var textBeforePartialFunctionNameBeforeCaret = EditorTextParser.getTextBeforePartialFunctionNameBeforeCaret(editorInfo);
            var textAfterCaret = EditorTextParser.getTextAfterCaret(editorInfo);
            var requiredTextAfterFunction = EditorTextParser.removeOpenBracketFromBeginning(textAfterCaret);
            var newText = textBeforePartialFunctionNameBeforeCaret + functionName + "(" + requiredTextAfterFunction;
            var newCaretPosition = newText.length - requiredTextAfterFunction.length;
            editorInfo.assign(newText, newCaretPosition);
        },
        getOuterOpenBracketAtLeftPosition: function(editorInfo) {
            var textBeforeCaret = EditorTextParser.getTextBeforeCaret(editorInfo);
            var openBracketsAtLeftCounter = 0;
            var closeBracketsAtLeftCounter = 0;
            for(var i = textBeforeCaret.length - 1; i >= 0; i--) {
                if(EditorTextParser.getCharByIndex(textBeforeCaret, i) === "(")
                    openBracketsAtLeftCounter++;
                if(EditorTextParser.getCharByIndex(textBeforeCaret, i) === ")")
                    closeBracketsAtLeftCounter++;
                if(openBracketsAtLeftCounter > closeBracketsAtLeftCounter)
                    return i;
            }
            return -1;
        },
        getPartialFunctionNameWithOpenBracketAroundCaret: function(editorInfo) {
            var partialFunctionNameBeforeCaret = EditorTextParser.getPartialFunctionNameBeforeCaret(editorInfo);
            if(partialFunctionNameBeforeCaret === "")
                return "";
            var partialFunctionNameAfterCaret = EditorTextParser.getPartialFunctionNameAfterCaret(editorInfo);
            var positionAfterPartialFunctionName = editorInfo.caretPosition + partialFunctionNameAfterCaret.length;
            var isOpenBracketAfterFunctionName = EditorTextParser.getCharByIndex(editorInfo.text, positionAfterPartialFunctionName) === "(";
            return isOpenBracketAfterFunctionName ? partialFunctionNameBeforeCaret + partialFunctionNameAfterCaret : "";
        },
        getPartialFunctionNameBeforeOuterOpenBracket: function(editorInfo) {
            var outerOpenBracketAtLeftPosition = this.getOuterOpenBracketAtLeftPosition(editorInfo);
            if(outerOpenBracketAtLeftPosition !== -1) {
                var newEditorInfo = new EditorInfo();
                newEditorInfo.assign(editorInfo.text, outerOpenBracketAtLeftPosition);
                return EditorTextParser.getPartialFunctionNameBeforeCaret(newEditorInfo);
            }
            return EditorTextParser.getPartialFunctionNameWithOpenBracketAroundCaret(editorInfo);
        }
    };

    var HintManager = {
        hintClassName: "dxss-Hint",
        hintIdPostfix: "_Hint",
        GetHintId: function(hintIdPrefix) {
            return hintIdPrefix + HintManager.hintIdPostfix;
        },
        FindHint: function(hintParent, hintIdPrefix) {
            return ASPx.GetChildById(hintParent, HintManager.GetHintId(hintIdPrefix));
        },
        FindOrCreateHint: function(hintParent, hintIdPrefix, hintStyle) {
            var hint = HintManager.FindHint(hintParent, hintIdPrefix);
            if(!ASPx.IsExists(hint)) {
                hint = document.createElement("DIV");
                hint.id = HintManager.GetHintId(hintIdPrefix);
                ASPx.AddClassNameToElement(hint, HintManager.hintClassName);
                ASPx.AddClassNameToElement(hint, hintStyle[0]);
                if(ASPx.IsExists(hintStyle[1]))
                    ASPx.AddClassNameToElement(hint, ASPx.CreateImportantStyleRule(ASPx.GetCurrentStyleSheet(), hintStyle[1]));
                hintParent.appendChild(hint);
            }
            return hint;
        },
        ShowHint: function(hintParent, hintIdPrefix, text, posX, posY, hintStyle, getOffsetToLeftWhenPlaceIsNotEnough) {
            var hint = HintManager.FindOrCreateHint(hintParent, hintIdPrefix, hintStyle);
            ASPx.SetInnerHtml(hint, text);
            ASPx.SetAbsoluteX(hint, posX);
            ASPx.SetAbsoluteY(hint, posY);
            var hintRightPosition = ASPx.GetAbsoluteX(hint) + hint.offsetWidth + ASPx.GetLeftRightMargins(hint);
            var isPlaceAtRightIsNotEnoughForHint = hintRightPosition > ASPx.GetDocumentClientWidth();
            if(isPlaceAtRightIsNotEnoughForHint && getOffsetToLeftWhenPlaceIsNotEnough) {
                var correctedPosX = posX - getOffsetToLeftWhenPlaceIsNotEnough(hint);
                if(correctedPosX >= 0)
                    ASPx.SetAbsoluteX(hint, correctedPosX);
            }
        },
        HideHint: function(hintParent, hintIdPrefix) {
            var hint = HintManager.FindHint(hintParent, hintIdPrefix);
            if(ASPx.IsExists(hint))
                ASPx.RemoveElement(hint);
        }
    };

    ASPxClientSpreadsheet.FormulaIntelliSenseManager = function(spreadsheet) {
        var functionsListBox = spreadsheet.getFunctionsListBox(); // TODO
        var functionsListBoxMainElement = functionsListBox.GetMainElement();

        if(ASPx.Browser.WebKitTouchUI)
            ASPx.TouchUIHelper.AttachDoubleTapEventToElement(functionsListBoxMainElement, function(e) {
                this.onFunctionsListBoxItemDoubleClick();
            }.aspxBind(this));
        else
            ASPx.Evt.AttachEventToElement(functionsListBoxMainElement, "dblclick", function(evt) {
                this.onFunctionsListBoxItemDoubleClick();
            }.aspxBind(this));

        functionsListBox.Hide();

        this.attachToEditorElement = function(editor) {
            if(ASPx.IsExists(this.editor))
                return;
            this.editor = editor;
            ASPx.Evt.AttachEventToElement(editor, "click", function(evt) { this.onEditorClick(); }.aspxBind(this));
            ASPx.Evt.AttachEventToElement(editor, "keydown", function(evt) { this.onEditorKeyDown(evt); }.aspxBind(this));
            ASPx.Evt.AttachEventToElement(editor, "keyup", function(evt) { this.onEditorKeyUp(evt); }.aspxBind(this));
            ASPx.Evt.AttachEventToElement(editor, "input", function(evt) { this.displayPossibleFunctionNamesInListBox(); }.aspxBind(this));
            if(ASPx.Browser.IE && ASPx.Browser.Version < 9)
                ASPx.Evt.AttachEventToElement(editor, "propertychange", function(evt) {
                    if(ASPx.GetElementDisplay(editor))
                        this.displayPossibleFunctionNamesInListBox();
                }.aspxBind(this));
        };
        this.onFunctionsListBoxItemDoubleClick = function() {
            this.insertSelectedFunctionNameToEditor();
            this.displayCurrentFunctionArgumentsHint();
        }
        this.updateIntelliSenseElementsPosition = function(requiredElementNearIntelliSenseElements) {
            this.intelliSenseElementPositionX = ASPx.GetAbsoluteX(requiredElementNearIntelliSenseElements);
            this.intelliSenseElementPositionY = ASPx.GetAbsoluteY(requiredElementNearIntelliSenseElements)
                + requiredElementNearIntelliSenseElements.offsetHeight;
            if(this.isFunctionsListBoxDisplayed())
                this.showFunctionsListBoxAtActualPosition();
        };
        this.onEditorClick = function() {
            this.hideFunctionsListBox();
            this.displayCurrentFunctionArgumentsHint();
        };
        this.isNavigationInListBoxKeyCode = function(evt) {
            var keyCode = ASPx.Evt.GetKeyCode(evt);
            return keyCode === ASPx.Key.Up || keyCode === ASPx.Key.Down
                || keyCode === ASPx.Key.PageUp || keyCode === ASPx.Key.PageDown;
        },
        this.isInsertFunctionKeyCode = function(evt) {
            return ASPx.Evt.GetKeyCode(evt) == ASPx.Key.Tab;
        };
        this.onEditorKeyDown = function(evt) {
            this.editorValueOnKeyDown = this.editor.value;
            if(this.isFunctionsListBoxDisplayed()) {
                if(this.isNavigationInListBoxKeyCode(evt))
                    this.performNavigationInListBox(evt);
                if(this.isInsertFunctionKeyCode(evt))
                    ASPx.Evt.PreventEventAndBubble(evt);
            }
        };
        this.onEditorKeyUp = function(evt) {
            if(this.isFunctionsListBoxDisplayed() && this.isInsertFunctionKeyCode(evt))
                this.insertSelectedFunctionNameToEditor();
            else if(!this.isNavigationInListBoxKeyCode(evt) && this.editor.value === this.editorValueOnKeyDown)
                this.hideFunctionsListBox();
            setTimeout(function() {
                if(!this.isFunctionsListBoxDisplayed())
                    this.displayCurrentFunctionArgumentsHint();
            }.aspxBind(this), 0);
        };
        this.getEditorInfo = function() {
            if(!ASPx.IsExists(this.editorInfo))
                this.editorInfo = new EditorInfo();
            this.editorInfo.assign(this.editor.value, ASPx.Selection.GetCaretPosition(this.editor));
            return this.editorInfo;
        };
        this.getFunctionByName = function(name) {
            if(name === "")
                return null;
            var nameForFiltering = ASPx.Str.PrepareStringForFilter(name);
            var functionIndex = ASPx.Data.ArrayIndexOf(ASPxClientSpreadsheet.Functions, nameForFiltering,
                function(currentFunction) {
                    var functionNameForFiltering = ASPx.Str.PrepareStringForFilter(currentFunction.name);
                    return functionNameForFiltering === nameForFiltering;
                });
            return functionIndex !== -1 ? ASPxClientSpreadsheet.Functions[functionIndex] : null;
        };
        this.displayCurrentFunctionArgumentsHint = function() {
            var editorInfo = this.getEditorInfo();
            var partialFunctionNameBeforeOuterOpenBracket = EditorTextParser.getPartialFunctionNameBeforeOuterOpenBracket(editorInfo);
            var spreadsheetFunction = this.getFunctionByName(partialFunctionNameBeforeOuterOpenBracket);
            if(ASPx.IsExists(spreadsheetFunction))
                this.showFunctionArgumentsHint(spreadsheetFunction);
            else
                this.hideFunctionArgumentsHint();
        };
        this.getFunctionArgumentsHintText = function(spreadsheetFunction) {
            var argumentsString = "";
            var argumentsWithDescriptionsString = "";
            for(var i = 0; i < spreadsheetFunction.arguments.length; i++) {
                var argument = spreadsheetFunction.arguments[i];
                argumentsString += argument.name;
                if(i !== spreadsheetFunction.arguments.length - 1)
                    argumentsString += ", ";
                argumentsWithDescriptionsString += "\r\n\t<b>" + argument.name + ": </b>" + argument.description;
            }
            var result = spreadsheetFunction.name + "(" + argumentsString;
            if(spreadsheetFunction.hasUnlimitedParametersCount)
                result += "...";
            result += ")" + argumentsWithDescriptionsString;
            return result;
        };
        this.insertSelectedFunctionNameToEditor = function() {
            var listBoxSelectedItem = functionsListBox.GetSelectedItem();
            if(!ASPx.IsExists(listBoxSelectedItem))
                return;
            var functionName = listBoxSelectedItem.text;
            var editorInfo = this.getEditorInfo();
            EditorTextParser.modifyEditorInfoByFunctionNameInserting(editorInfo, functionName);
            spreadsheet.setElementsValue(editorInfo.text);
            this.hideFunctionsListBoxOnFunctionInserting();
            spreadsheet.getEditingHelper().updateInplaceEditingCellElementSize();
            ASPx.Selection.SetCaretPosition(this.editor, editorInfo.caretPosition);
        };
        this.hideFunctionsListBoxOnFunctionInserting = function() {
            if(ASPx.Browser.IE && ASPx.Browser.Version < 9)
                setTimeout(function() { this.hideFunctionsListBox(); }.aspxBind(this), 0);
            else
                this.hideFunctionsListBox();
        };
        this.displayPossibleFunctionNamesInListBox = function() {
            if(ASPx.Browser.Safari)
                setTimeout(function() { this.displayPossibleFunctionNamesInListBoxCore(); }.aspxBind(this), 0);
            else
                this.displayPossibleFunctionNamesInListBoxCore();
        };
        this.displayPossibleFunctionNamesInListBoxCore = function() {
            ASPx.Attr.ChangeStyleAttribute(functionsListBoxMainElement, "display", "table");
            var editorInfo = this.getEditorInfo();
            var partialFunctionNameBeforeCaret = EditorTextParser.getPartialFunctionNameBeforeCaret(editorInfo);
            this.recreateListBoxItems(partialFunctionNameBeforeCaret);
            if(functionsListBox.GetItemCount() > 0) {
                this.hideFunctionArgumentsHint();
                this.showFunctionsListBoxAtActualPosition();
                functionsListBox.SetSelectedIndex(0);
            }
            else
                this.hideFunctionsListBox();
        };
        this.recreateListBoxItems = function(partialFunctionName) {
            functionsListBox.ClearItems();
            if(partialFunctionName)
                this.addListBoxItems(partialFunctionName);
        };
        this.addListBoxItems = function(partialFunctionName) {
            var partialFunctionNameForFiltering = ASPx.Str.PrepareStringForFilter(partialFunctionName);
            functionsListBox.AddFunctionItemsByFilter(function(currentFunction) {
                var functionNameForFiltering = ASPx.Str.PrepareStringForFilter(currentFunction.name);
                return functionNameForFiltering.indexOf(partialFunctionNameForFiltering) === 0;
            });
        };
        this.performNavigationInListBox = function(evt) {
            var keyCode = ASPx.Evt.GetKeyCode(evt);
            if(keyCode === ASPx.Key.Up)
                functionsListBox.OnArrowUp();
            if(keyCode === ASPx.Key.Down)
                functionsListBox.OnArrowDown();
            if(keyCode === ASPx.Key.PageUp)
                functionsListBox.OnPageUp();
            if(keyCode === ASPx.Key.PageDown)
                functionsListBox.OnPageDown();
            /*return*/ ASPx.Evt.PreventEventAndBubble(evt); // TODO
        };
        this.showFunctionArgumentsHint = function(spreadsheetFunction) {
            var argumentsHintText = this.getFunctionArgumentsHintText(spreadsheetFunction);
            HintManager.ShowHint(spreadsheet.GetMainElement(), spreadsheet.name, argumentsHintText,
                this.intelliSenseElementPositionX, this.intelliSenseElementPositionY,
                spreadsheet.functionArgumentsHintStyle, function(hint) {
                    return hint.offsetWidth > this.editor.offsetWidth ? hint.offsetWidth - this.editor.offsetWidth : 0;
                }.aspxBind(this));
        };
        this.hideFunctionArgumentsHint = function() {
            HintManager.HideHint(spreadsheet.GetMainElement(), spreadsheet.name);
        };
        this.showFunctionsListBoxAtActualPosition = function() {
            functionsListBox.ShowAtPos(this.intelliSenseElementPositionX, this.intelliSenseElementPositionY);
        };
        this.hideFunctionsListBox = function() {
            functionsListBox.Hide();
        };
        this.hideIntelliSenseElements = function() {
            this.hideFunctionsListBox();
            this.hideFunctionArgumentsHint();
        };
        this.isFunctionsListBoxDisplayed = function() {
            return ASPx.GetElementVisibility(functionsListBoxMainElement);
        };
    };

    ASPxClientSpreadsheet.EditorTextParser = EditorTextParser;
    ASPxClientSpreadsheet.EditorInfo = EditorInfo;
    ASPxClientSpreadsheet.HintManager = HintManager;

})();