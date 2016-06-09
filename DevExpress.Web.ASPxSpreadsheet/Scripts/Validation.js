(function() {
    ASPxClientSpreadsheet.ValidationHelper = ASPx.CreateClass(null, {
        constructor: function(spreadsheet) {
            this.spreadsheet = spreadsheet;
            this.listValidations = [];
            this.inputMessages = [];
            this.messagesCache = [];
        },
        processDocumentResponse: function(response) {
            var validation = response.validation;
            if(response.clearGridCache || response.loadNewSheet) {
                this.listValidations = [];
                this.inputMessages = [];
                this.messagesCache = [];
            }
            if(validation) {
                this.addListValidations(validation.listValidations);
                this.addInputMessages(validation.inputMessages);
                if(validation.invalidDataCircles)
                    this.showInvalidDataCircles(validation.invalidDataCircles);
            }
            if(response.validationConfirm)
                this.showValidationConfirm(response.validationConfirm);
            this.showValidationElementsByModelPosition(response.selection.activeCol, response.selection.activeRow);
        },
        addListValidations: function(listValidations) {
            this.addNewItemsToLTRBArray(this.listValidations, listValidations);
        },
        addInputMessages: function(inputMessages) {
            this.addNewItemsToLTRBArray(this.inputMessages, inputMessages);
        },
        removeValidations: function(range) {
            for(var i = range.leftColIndex; i <= range.rightColIndex; i++) {
                for(var j = range.topRowIndex; j <= range.bottomRowIndex; j++) {
                    this.removeItemFromLTRBByCell(this.listValidations, i, j);
                    this.removeItemFromLTRBByCell(this.inputMessages, i, j);
                }
            }
        },

        showInvalidDataCircles: function(circles) {
            var colIndex = 0, rowIndex = 1;
            for(var i = 0; i < circles.length; i++)
                this.showCircle(circles[i][colIndex], circles[i][rowIndex]);
        },
        showCircle: function(col, row) {
            var cellLayoutInfo = this.getCellLayoutInfo_ByModelIndices(col, row);
            if(!cellLayoutInfo || !cellLayoutInfo.tileInfo) return;
            var rect = this.getCircleRect(cellLayoutInfo);
            if(!this.circleExists(rect, cellLayoutInfo)) {
                var circle = this.createCircle();
                ASPx.SetStyles(circle, {
                    left: rect.left,
                    top: rect.top,
                    width: rect.width,
                    height: rect.height
                });
                ASPxClientSpreadsheet.ElementPlacementHelper.attachElementToTile(circle, cellLayoutInfo.tileInfo.htmlElement);
            }
        },
        createCircle: function() {
            var circle = document.createElement("DIV");
            circle.className = ASPx.SpreadsheetCssClasses.InvalidDataCircle;
            return circle;
        },
        getCircleRect: function(cellLayoutInfo) {
            return {
                left: cellLayoutInfo.rect.x - 5,
                top: cellLayoutInfo.rect.y - 2,
                width: cellLayoutInfo.rect.width + 10,
                height: cellLayoutInfo.rect.height + 4
            };
        },
        circleExists: function(rect, cellLayoutInfo) {
            var circles = ASPx.GetChildNodesByClassName(cellLayoutInfo.tileInfo.htmlElement, ASPx.SpreadsheetCssClasses.InvalidDataCircle);
            for(var i = 0; i < circles.length; i++) {
                var circle = circles[i];
                if(rect.left === ASPx.PxToInt(circle.style.left) && rect.top === ASPx.PxToInt(circle.style.top) && 
                    rect.width === ASPx.PxToInt(circle.style.width) && rect.height === ASPx.PxToInt(circle.style.height))
                    return true;
            }
            return false;
        },

        showValidationElements: function(col, row) {
            var modelCol = this.getPaneManager().convertVisibleIndexToModelIndex(col, true),
                modelRow = this.getPaneManager().convertVisibleIndexToModelIndex(row, false);
            this.showValidationElementsByModelPosition(modelCol, modelRow);
        },
        showValidationElementsByModelPosition: function(modelCol, modelRow) {
            this.showDropDownButton(modelCol, modelRow);
            this.tryToShowPopupMessage(modelCol, modelRow);
            this.setCellPosition(modelCol, modelRow);
        },
        setCellPosition: function(col, row) {
            this.cellPosition = { col: col, row: row };
        },

        showDropDownButton: function(col, row) {
            var dropDownButton = this.getDropDownButton();
            ASPx.SetElementDisplay(this.getDropDownPanel(), false);
            if(this.allowDropDownForCell(col, row)) {
                var cellLayoutInfo = this.getCellLayoutInfo_ByModelIndices(col, row);
                var dropDownButtonHeight = 17, borderCorrection = 1;

                this.placeElementToWorkbook(dropDownButton, cellLayoutInfo.rect.x + cellLayoutInfo.rect.width + borderCorrection, cellLayoutInfo.rect.y + cellLayoutInfo.rect.height - dropDownButtonHeight - borderCorrection, cellLayoutInfo.tileInfo.htmlElement);

                if(this.isCellOutOfGridVisibleRange(cellLayoutInfo))
                    ASPx.SetElementDisplay(dropDownButton, false);
            } else
                ASPx.SetElementDisplay(dropDownButton, false);
        },
        isCellOutOfGridVisibleRange: function(cellLayoutInfo) {
            var activePane = this.getPaneManager().getPaneByType(cellLayoutInfo.paneType),
                visibleCellRange = this.getPaneManager().getDisplayedCellVisibleRange(activePane);
            return !visibleCellRange.isCellInRange(cellLayoutInfo.colIndex, cellLayoutInfo.rowIndex);
        },

        tryToShowPopupMessage: function(col, row) {
            if(this.allowPopupForCell(col, row))
                this.showPopupMessage(col, row);
            else
                ASPx.SetElementDisplay(this.getPopupMessageElement(), false);
        },
        showPopupMessage: function(col, row) {
            var message = this.getCellMessage(col, row);
            if(!message) return;
            var popup = this.getPopupMessageElement();
            this.setPopupMessageTitle(message.title);
            this.setPopupMessageText(message.text);
            
            var cellLayoutInfo = this.getCellLayoutInfo_ByModelIndices(col, row);
            var verticalOffset = 5;

            this.placeElementToWorkbook(popup, cellLayoutInfo.rect.x + cellLayoutInfo.rect.width / 2, cellLayoutInfo.rect.y + cellLayoutInfo.rect.height + verticalOffset, cellLayoutInfo.tileInfo.htmlElement);
            if(this.isCellOutOfGridVisibleRange(cellLayoutInfo))
                ASPx.SetElementDisplay(popup, false);
        },

        showValidationConfirm: function(validationConfirm) {
            var dialog = this.spreadsheet.getDialogList()["validationconfirmdialog"];
            dialog.setValidationConfirm(validationConfirm);
            dialog.Execute(this.spreadsheet);
        },

        showDropDownPanel: function(col, row, values) {
            var panel = this.getDropDownPanel(),
                button = this.getDropDownButton(),
                borderCorrection = 1,
                cellLayoutInfo = this.getCellLayoutInfo_ByModelIndices(col, row);
            panel.innerHTML = "";
            panel.appendChild(this.createValuesTable(values));
            
            this.placeElementToWorkbook(panel, cellLayoutInfo.rect.x, cellLayoutInfo.rect.y + cellLayoutInfo.rect.height, cellLayoutInfo.tileInfo.htmlElement);
            ASPx.SetStyles(panel, {
                minWidth: cellLayoutInfo.rect.width + button.offsetWidth - borderCorrection
            });
        },
        placeElementToWorkbook: function(element, x, y, offsetElement) {
            var workbook = this.spreadsheet.getRenderProvider().getWorkbookControl(),
                rect = { x: x, y: y };
            ASPxClientSpreadsheet.ElementPlacementHelper.appendChildWithCheck(element, workbook);
            ASPx.SetElementDisplay(element, true);
            ASPxClientSpreadsheet.RectHelper.setElementRectWithElementOffset(element, rect, offsetElement);
        },
        toggleDropDownPanel: function() {
            var panel = this.getDropDownPanel();
            if(ASPx.GetElementDisplay(panel))
                ASPx.SetElementDisplay(panel, false);
            else
                this.spreadsheet.fetchListAllowedValues();
        },

        allowDropDownForCell: function(col, row) {
            return this.getCellIndexInLTRBArray(this.listValidations, col, row) > -1;
        },
        allowPopupForCell: function(col, row) {
            return this.getCellIndexInLTRBArray(this.inputMessages, col, row) > -1;
        },
        getCellIndexInLTRBArray: function(array, col, row) {
            var ltrb = {
                left: 0,
                top: 1,
                right: 2,
                bottom: 3
            };
            for(var i = 0; i < array.length; i++) {
                var validation = array[i];
                if(validation[ltrb.left] <= col && validation[ltrb.right] >= col &&
                    validation[ltrb.top] <= row && validation[ltrb.bottom] >= row)
                    return i;
            }
            return -1;
        },
        removeItemFromLTRBByCell: function(array, col, row) {
            var index = this.getCellIndexInLTRBArray(array, col, row);
            if(index > -1)
                array.splice(index, 1);
        },
        addNewItemsToLTRBArray: function(array, items) {
            if(!items) return;
            for(var i = 0; i < items.length; i++) {
                var item = items[i];
                if(this.getItemIndexInLTRBArray(array, item) < 0)
                    array.push(item);
            }
        },
        getItemIndexInLTRBArray: function(array, item) {
            for(var i = 0; i < array.length; i++) {
                if(array[i].toString() === item.toString())
                    return i;
            }
            return -1;
        },

        getPaneManager: function() {
            return this.spreadsheet.getPaneManager();
        },
        getCellLayoutInfo_ByModelIndices: function(col, row) {
            return this.getPaneManager().getCellLayoutInfo_ByModelIndices(col, row);
        },

        setPopupMessageText: function(text) {
            var childDivs = this.getPopupMessageElement().getElementsByTagName("DIV");
            childDivs[1].innerHTML = text;
        },
        setPopupMessageTitle: function(title) {
            var childDivs = this.getPopupMessageElement().getElementsByTagName("DIV");
            childDivs[0].innerHTML = title;
        },
        getCellMessage: function(col, row) {
            var index = this.getCellIndexInLTRBArray(this.inputMessages, col, row);
            if(this.messagesCache[index])
                return this.messagesCache[index];
            else
                this.spreadsheet.fetchMessageForCell();
            return null;
        },
        setCellMessage: function(col, row, title, text) {
            var index = this.getCellIndexInLTRBArray(this.inputMessages, col, row);
            this.messagesCache[index] = { title: title, text: text };
        },

        getDropDownButton: function() {
            if(!this.spreadsheet.dropDownButtonElement)
                this.spreadsheet.dropDownButtonElement = this.createDropDownButton();
            return this.spreadsheet.dropDownButtonElement;
        },
        getPopupMessageElement: function() {
            if(!this.spreadsheet.popupMessage)
                this.spreadsheet.popupMessage = this.createPopupMessage();
            return this.spreadsheet.popupMessage;
        },
        getDropDownPanel: function() {
            if(!this.spreadsheet.dropDownPanelElement)
                this.spreadsheet.dropDownPanelElement = this.createDropDownPanel();
            return this.spreadsheet.dropDownPanelElement;
        },

        createDropDownButton: function() {
            var button = document.createElement("DIV");
            button.className = ASPx.SpreadsheetCssClasses.DropDownButtonImage + " " + this.spreadsheet.autoFilterImagesClassNames["DropDown"];
            return button;
        },
        createPopupMessage: function() {
            var popup = document.createElement("DIV"),
                title = document.createElement("DIV"),
                text = document.createElement("DIV");
            popup.className = ASPx.SpreadsheetCssClasses.PopupMessage;
            popup.appendChild(title);
            popup.appendChild(text);
            return popup;
        },
        createDropDownPanel: function() {
            var div = document.createElement("DIV");
            div.className = ASPx.SpreadsheetCssClasses.DropDownPanel;
            ASPx.SetElementDisplay(div, false);
            return div;
        },
        createValuesTable: function(values) {
            var table = document.createElement("TABLE");
            var tbody = document.createElement("TBODY");
            table.appendChild(tbody);
            for(var i = 0; i < values.length; i++) {
                var tr = document.createElement("TR"),
                    td = document.createElement("TD");
                var text = values[i];
                tbody.appendChild(tr);
                tr.appendChild(td);
                td.innerHTML = ASPx.Str.EncodeHtml(text);
            }
            return table;
        },

        onScroll: function() {
            var selection = this.getPaneManager().getStateController().getSelection();
            this.showValidationElements(selection.activeCellColIndex, selection.activeCellRowIndex)
        },
        onDropDownButtonMouseUp: function() {
            this.toggleDropDownPanel();
        },
        onDropDownPanelMouseUp: function(srcElement) {
            if(srcElement.tagName === "TD") {
                var value = ASPx.Str.DecodeHtml(srcElement.innerHTML);
                this.spreadsheet.onCellValueChangedWithNewSelection({ Column: this.cellPosition.col, Row: this.cellPosition.row }, value);
            }
        },
        onAllowedValuesReceived: function(response) {
            this.showDropDownPanel(this.cellPosition.col, this.cellPosition.row, response.allowedValues);
        },
        onMessageForCellReceived: function(response) {
            this.setCellMessage(this.cellPosition.col, this.cellPosition.row, response.title, response.text);
            this.showPopupMessage(this.cellPosition.col, this.cellPosition.row);
        }
    });
})();