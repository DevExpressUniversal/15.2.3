

(function() {
    ASPxClientSpreadsheet.RibbonManager = function(spreadsheetControl) {
        this.spreadsheetControl = spreadsheetControl;

        this.history = { canRedo: undefined, canUndo: undefined };
        this.calculationMode = "";
        this.pageOrientation = "Default";
        this.paperKind = "";
        this.pageMargins = "custome"; // TODO fix typo

        this.containingTable = null;
        this.intersectingTable = null;
        this.isSelectionWithinAnyTable = false;
        this.isPictureSelected = false;
        this.isChartSelected = false;

        var tablePropertiesMask = {
            FILTERED_OR_SORTED  : 1,
            HEADER_ROW          : 2,
            TOTAL_ROW           : 4,
            BANDED_COLUMNS      : 8,
            BANDED_ROWS         : 16,
            FIRST_COLUMN        : 32,
            LAST_COLUMN         : 64,
            FILTER_ENABLED      : 128
        };

        // Active cell
        function getRibbonControl() {
            return spreadsheetControl.GetRibbon();
        }
        function activeCellChanged() {
            applyCellStyleToRibbonControl();
        }
        function applyCellStyleToRibbonControl() {
            var ribbon = getRibbonControl();
            var selectedRangeStyles = getActiveCellStyle();
            if(ribbon) { 
                var item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FormatFontBold").id);
                if(item)
                    item.SetValue(selectedRangeStyles.bold);

                item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FormatFontItalic").id);
                if(item)
                    item.SetValue(selectedRangeStyles.italic);

                item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FormatFontUnderline").id);
                if(item)
                    item.SetValue(selectedRangeStyles.underline);

                item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FormatFontStrikeout").id);
                if(item)
                    item.SetValue(selectedRangeStyles.strike);

                item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FormatFontSize").id);
                if(item)
                    item.SetValue(selectedRangeStyles.fontSize);

                item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FormatFontName").id);
                if(item)
                    item.SetValue(selectedRangeStyles.fontName);

                item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FormatFontColor").id);
                if(item)
                    item.SetValue(selectedRangeStyles.fontColor);

                item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FormatFillColor").id);
                if(item)
                    item.SetValue(selectedRangeStyles.backgroundColor);

                item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FormatWrapText").id);
                if(item)
                    item.SetValue(selectedRangeStyles.textWrap);

                applyVerticalAlign(ribbon, selectedRangeStyles.verticalAlign);
                applyHorizontalAlign(ribbon, selectedRangeStyles.horizontalAlign);

                applyHyperlink(ribbon, selectedRangeStyles.containsHyperlink);
            }
        }
        function applyHyperlink(ribbon, isCellContainsLink) {
            var item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FormatClearHyperlinks").id);
            if(item)
                item.SetEnabled(isCellContainsLink);

            item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FormatRemoveHyperlinks").id);
            if(item)
                item.SetEnabled(isCellContainsLink);
        }


        function selectionRangeChanged() {
            var ribbon = getRibbonControl(),
                ribbonManager = spreadsheetControl.getRibbonManager();

            updateActiveTable();
            updateActiveDrawing();

            updateContextTabsVisibility(ribbon);

            applyTable(ribbon);
            if(ribbonManager.isChartSelected)
                applyChart(ribbon);
            if(ribbonManager.isPictureSelected)
                applyPicture(ribbon);
        }
        function updateContextTabsVisibility(ribbon) {
            var ribbonManager = spreadsheetControl.getRibbonManager();

            ribbon.SetContextTabCategoryVisible("TableTools", ribbonManager.isSelectionWithinAnyTable);
            ribbon.SetContextTabCategoryVisible("PictureTools", ribbonManager.isPictureSelected);
            ribbon.SetContextTabCategoryVisible("ChartTools", ribbonManager.isChartSelected);
        }

        function updateActiveTable() {
            var ribbonManager = spreadsheetControl.getRibbonManager(),
                tableInfos = ribbonManager.tableInfos,
                selectionRange = getSelectionRange();

            ribbonManager.intersectingTable = null;

            ribbonManager.containingTable = findContainingTable(tableInfos, selectionRange);
            if(!ribbonManager.containingTable)
                ribbonManager.intersectingTable = findIntersectingTable(tableInfos, selectionRange);

            ribbonManager.isSelectionWithinAnyTable = !!ribbonManager.containingTable;
        }

        function applyTable(ribbon) {
            var ribbonManager = spreadsheetControl.getRibbonManager(),
                selectionContext = getTableSelectionContext(ribbonManager),
                tableCommandsIDs = ASPxClientSpreadsheet.ServerCommands.getTableCommandsIDs();

            updateContextItemsEnabledState(ribbon, selectionContext, tableCommandsIDs);
            updateContextItemsCheckedState(ribbon, selectionContext, tableCommandsIDs);

            if(selectionContext.contextTabShown)
                updateTableNameTextBox(ribbon, ribbonManager.containingTable.name);
        }

        function updateActiveDrawing() {
            var ribbonManager = spreadsheetControl.getRibbonManager(),
                selection = spreadsheetControl.getStateController().getSelection();

            ribbonManager.isPictureSelected = false;
            ribbonManager.isChartSelected = false;

            if(selection.drawingBoxIndex > -1) {
                ribbonManager.isPictureSelected = !spreadsheetControl.getIsChartElement(selection.drawingBoxElement);
                ribbonManager.isChartSelected = !ribbonManager.isPictureSelected;
            }
        }

        function applyPicture(ribbon) {
            var ribbonManager = spreadsheetControl.getRibbonManager(),
                selectionContext = getDrawingSelectionContext(ribbonManager),
                pictureCommandsIDs = ASPxClientSpreadsheet.ServerCommands.getPictureCommandsIDs();

            updateContextItemsEnabledState(ribbon, selectionContext, pictureCommandsIDs);
        }

        function getDrawingSelectionContext(ribbonManager) {
            return {
                isArrangeEnabled: spreadsheetControl.isArrangeCommandEnabled()
            };
        }

        function applyChart(ribbon) {
            var ribbonManager = spreadsheetControl.getRibbonManager(),
                selectionContext = getDrawingSelectionContext(ribbonManager),
                chartCommandsIDs = ASPxClientSpreadsheet.ServerCommands.getChartCommandsIDs();

            updateContextItemsEnabledState(ribbon, selectionContext, chartCommandsIDs);
        }

        function getTableSelectionContext(ribbonManager) {
            var isSelectionIntersectsTable = !!ribbonManager.intersectingTable;

            var canInsertTable = !(ribbonManager.isSelectionWithinAnyTable || isSelectionIntersectsTable),
                canSort = ribbonManager.isSelectionWithinAnyTable || !isSelectionIntersectsTable;

            var context = {
                lockedSheet: !!spreadsheetControl.sheetLocked,
                readonlyMode: !!spreadsheetControl.readOnly,
                lockedWorkbook: !!spreadsheetControl.workbookLocked,
                canInsertTable: canInsertTable,
                canSort: canSort,
                canToggleFilter: !isSelectionIntersectsTable,
                canFormatAsTable: !isSelectionIntersectsTable,
                isFilterEnabled: isAutoFilterEnabled(ribbonManager),
                isFilterApplied: isAutoFilterApplied(ribbonManager)
            };

            if(ribbonManager.isSelectionWithinAnyTable) {
                var properties = ribbonManager.containingTable.properties;

                context.showHeaderRow     = getTableBoolProperty(properties, tablePropertiesMask.HEADER_ROW);
                context.showTotalRow      = getTableBoolProperty(properties, tablePropertiesMask.TOTAL_ROW);
                context.showBandedColumns = getTableBoolProperty(properties, tablePropertiesMask.BANDED_COLUMNS);
                context.showBandedRows    = getTableBoolProperty(properties, tablePropertiesMask.BANDED_ROWS);
                context.showFirstColumn   = getTableBoolProperty(properties, tablePropertiesMask.FIRST_COLUMN);
                context.showLastColumn    = getTableBoolProperty(properties, tablePropertiesMask.LAST_COLUMN);
                context.contextTabShown   = true;
            }

            return context;
        }

        function isAutoFilterEnabled(ribbonManager) {
            if(!!ribbonManager.intersectingTable)
                return false;

            var insideTable = !!ribbonManager.containingTable,
                isTableFilterEnabled = ribbonManager.containingTable && getTableBoolProperty(ribbonManager.containingTable.properties, tablePropertiesMask.FILTER_ENABLED),
                isSheetFilterEnabled = ribbonManager.sheetFilterState.enabled;

            return insideTable ? isTableFilterEnabled : isSheetFilterEnabled;
        }
        function isAutoFilterApplied(ribbonManager) {
            if(!!ribbonManager.intersectingTable)
                return false;

            var insideTable = !!ribbonManager.containingTable,
                isTableFilterApplied = ribbonManager.containingTable && getTableBoolProperty(ribbonManager.containingTable.properties, tablePropertiesMask.FILTERED_OR_SORTED),
                isSheetFilterApplied = ribbonManager.sheetFilterState.applied;

            return insideTable ? isTableFilterApplied : isSheetFilterApplied;
        }

        function updateContextItemsEnabledState(ribbon, context, commandsIDs) {
            var changeEnabledStateDelegate = function(commandName, commandConfig) {
                updateRibbonItemEnabledState(ribbon, commandName, getIsCommandEnabled(commandConfig, context));
            };

            updateContextItemsState(changeEnabledStateDelegate, commandsIDs);
        }
        function updateContextItemsCheckedState(ribbon, context, commandsIDs) {
            var changeCheckedStateDelegate = function(commandName, commandConfig) {
                var isCommandVisible = !commandConfig.isContextCommand || commandConfig.isContextCommand === context.contextTabShown;

                if(commandConfig.checked && isCommandVisible)
                    updateRibbonItemCheckedState(ribbon, commandName, getIsCommandChecked(commandConfig, context));
            };

            updateContextItemsState(changeCheckedStateDelegate, commandsIDs);
        }

        function updateContextItemsState(changeStateDelegate, commandsIDs) {
            for(var commandID in commandsIDs) {
                if(!commandsIDs.hasOwnProperty(commandID))
                    continue;

                var commandConfig = ASPxClientSpreadsheet.ServerCommands.getCommandConfigByID(commandID);

                changeStateDelegate(commandsIDs[commandID], commandConfig);
            }
        }

        function getIsCommandEnabled(commandConfig, context) {
            var lockProperties = ["lockedSheet", "readonlyMode", "lockedWorkbook"];
            var enabledConfig = commandConfig.enabled;

            for(var i = 0; i < lockProperties.length; i++) { // TODO: refactor this part
                var property = lockProperties[i];
                if(context[property] && (enabledConfig && !enabledConfig.hasOwnProperty(property)))
                    return false;
            }

            return getCommandStateCore(commandConfig.enabled, context);
        }

        function getIsCommandChecked(commandConfig, context) {
            return getCommandStateCore(commandConfig.checked, context);
        }

        function getCommandStateCore(stateConfig, context) {
            if(!stateConfig)
                return true;

            for(var property in stateConfig) {
                if(!(stateConfig.hasOwnProperty(property) && context.hasOwnProperty(property)))
                    continue;

                if(context[property] !== stateConfig[property])
                    return false;
            }
            return true;
        }

        function updateRibbonItemEnabledState(ribbon, commandName, enabled) {
            var item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(commandName).id);
            if(item && item.GetEnabled() !== enabled)
                item.SetEnabled(enabled);
        }
        function updateRibbonItemCheckedState(ribbon, commandName, checked) {
            var item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName(commandName).id);
            if(item && item.GetValue() !== checked)
                item.SetValue(checked);
        }
        function updateTableNameTextBox(ribbon, tableName) {
            var item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("TableToolsRenameTable").id);
            item.SetValue(tableName);
        }
        function findContainingTable(tableInfos, selectionRange) {
            return findTableCore(tableInfos, selectionRange, "containsRange");
        }
        function findIntersectingTable(tableInfos, selectionRange) {
            return findTableCore(tableInfos, selectionRange, "intersectsRange");
        }
        function findTableCore(tableInfos, selectionRange, compareFn) {
            for(var i = 0; i < tableInfos.length; i++) {
                if(tableInfos[i].range[compareFn](selectionRange))
                    return tableInfos[i];
            }
            return null;
        }
        function getTableBoolProperty(properties, propertyMask) {
            return !!(properties & propertyMask);
        }

        function getSelectionRange() {
            var selection = spreadsheetControl.getStateController().getSelection().range;
            return convertSelectionToRange(selection);
        }
        function convertSelectionToRange(selection) {
            var paneManager = spreadsheetControl.getPaneManager();

            return new ASPxClientSpreadsheet.Range(
                paneManager.convertVisibleIndexToModelIndex(selection.leftColIndex, true),
                paneManager.convertVisibleIndexToModelIndex(selection.topRowIndex),
                paneManager.convertVisibleIndexToModelIndex(selection.rightColIndex, true),
                paneManager.convertVisibleIndexToModelIndex(selection.bottomRowIndex)
            );
        }

        function doesCellContainsHyperLink(element) {
            if(element) {
                var link = ASPx.GetNodeByTagName(element, "a", 0);
                return ASPx.IsExists(link);
            }
            return false;
        }
        function applyVerticalAlign(ribbon, verticalAlign) {
            var itemTop = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FormatAlignmentTop").id);
            var itemBottom = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FormatAlignmentBottom").id);
            var itemMiddle = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FormatAlignmentMiddle").id);
            uncheckButtons(itemTop, itemMiddle, itemBottom);
            switch(verticalAlign) {
                case 'auto':
                case 'baseline':
                case 'top':
                    if(itemTop)
                        itemTop.SetValue(true);
                    break;
                case 'middle':
                    if(itemMiddle)
                        itemMiddle.SetValue(true);
                    break;
                case 'bottom':
                    if(itemBottom)
                        itemBottom.SetValue(true);
                    break;
            }
        }
        function applyHorizontalAlign(ribbon, horizontalAlign) {
            var itemLeft = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FormatAlignmentLeft").id);
            var itemRight = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FormatAlignmentRight").id);
            var itemCenter = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FormatAlignmentCenter").id);
            uncheckButtons(itemLeft, itemCenter, itemRight);
            switch(horizontalAlign) {
                case 'right':
                    if(itemRight)
                        itemRight.SetValue(true);
                    break;
                case 'center':
                    if(itemCenter)
                        itemCenter.SetValue(true);
                    break;
                case 'left':
                    if(itemLeft)
                        itemLeft.SetValue(true);
                    break;
            }
        }
        // Editing
        function onEditModeChanged(inEditMode) {
            switchRibbonEditingMode(!inEditMode);
        }
        function switchRibbonEditingMode(enabled) {
            var ribbon = getRibbonControl();
            if(ribbon) {
                var disabledCommands = ASPxClientSpreadsheet.ServerCommands.getEditModeDisabledCommandsIDs();
                for(var commandID in disabledCommands) {
                    var item = ribbon.GetItemByName(commandID);
                    if(item)
                        if(item.GetEnabled(false) != enabled)
                            item.SetEnabled(enabled);
                }
            }
        }
        function restoreDisabledItems(ribbonManager) {
            var ribbon = getRibbonControl();
            if(ribbon) {
                applyHyperlink(ribbon, doesCellContainsHyperLink(spreadsheetControl.getRenderProvider().getActiveCellElement()));
                applyHistoryNavigation(ribbonManager.history);
                applyTabParams();
                ribbonManager.onDrawingBoxSelected();
            }
        }
        function applyCalculationMode(calcMode) {
            var ribbon = getRibbonControl();
            var item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FormulasCalculationModeAutomatic").id);
            if(item)
                item.SetValue(calcMode == "Automatic");

            item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FormulasCalculationModeManual").id);
            if(item)
                item.SetValue(calcMode == "Manual");
        }
        function applyPageOrientation(pageOrientation) {
            var ribbon = getRibbonControl();
            var item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("PageSetupOrientationPortrait").id);
            if(item)
                item.SetValue(pageOrientation == "Portrait");

            item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("PageSetupOrientationLandscape").id);
            if(item)
                item.SetValue(pageOrientation == "Landscape");
        }
        function applyPageMargins(pageMargins) {
            var ribbon = getRibbonControl();
            var item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("PageSetupMarginsNormal").id);
            if(item)
                item.SetValue(pageMargins == "normal");

            item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("PageSetupMarginsNarrow").id);
            if(item)
                item.SetValue(pageMargins == "narrow");

            item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("PageSetupMarginsWide").id);
            if(item)
                item.SetValue(pageMargins == "wide");
        }
        function applyPaperKind(paperKind) {
            var pageFormat = ["Letter", "Legal", "Folio", "A4", "B5", "Executive", "A5", "A6"];
            var ribbon = getRibbonControl();
            for(var index in pageFormat) {
                var item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("SetPaperKind").id + ";#" + pageFormat[index]);
                if(item)
                    item.SetValue(paperKind == pageFormat[index]);                
            }
        }
        function applyTabParams() {
            var ribbon = getRibbonControl();
            var tabControl = spreadsheetControl.getTabControl();

            var isHideCommandEnable = tabControl.getVisibleSheets() && tabControl.getVisibleSheets().length > 1;
            var isUnhideCommandEnable = tabControl.getHiddenSheets() && tabControl.getHiddenSheets().length > 0;

            var item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("UnhideSheet").id);
            if(item)
                item.SetEnabled(isUnhideCommandEnable);

            item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("HideSheet").id);
            if(item)
                item.SetEnabled(isHideCommandEnable);

            item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("RemoveSheet").id);
            if(item)
                item.SetEnabled(isHideCommandEnable);
        }
        function applyHistoryNavigation(history) {
            var ribbon = getRibbonControl();
            if(history) {
                var item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FileUndo").id);
                if(item)
                    item.SetEnabled(history.canUndo);

                item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FileRedo").id);
                if(item)
                    item.SetEnabled(history.canRedo);
            }
        }
        // Cell style
        function getDefaultCellStyle() {
            return  {   bold: false,
                        italic: false,
                        underline: false,
                        strike: false,

                        fontName: "Calibri",
                        fontSize: "11",
                        fontColor: "",

                        textWrap: false,

                        backgroundColor: "",

                        verticalAlign: "",
                        horizontalAlign: "",

                        containsHyperlink: false }
        }
        function getActiveCellStyle() {
            var styles = getDefaultCellStyle();
            var activeCellElement = spreadsheetControl.getRenderProvider().getActiveCellElement();
            if(activeCellElement) {
                var textBoxElement = ASPx.GetNodeByClassName(activeCellElement, ASPx.SpreadsheetCssClasses.TextBoxContent);
                if(ASPx.IsExists(textBoxElement)) {
                    var currentCellStyle = ASPx.GetCurrentStyle(textBoxElement);
                    if(currentCellStyle) {
                        styles.bold = checkFontBold(currentCellStyle.fontWeight);
                        styles.italic = currentCellStyle.fontStyle == 'italic';
                        styles.underline = currentCellStyle.textDecoration.indexOf('underline') == 0;
                        styles.strike = currentCellStyle.textDecoration.indexOf('line-through') == 0;
                        styles.fontSize = getFontSize(currentCellStyle.fontSize);
                        styles.fontName = getFontName(currentCellStyle.fontFamily);
                        styles.fontColor = ASPx.Color.ColorToHexadecimal(currentCellStyle.color);

                        styles.verticalAlign = currentCellStyle.verticalAlign.toLowerCase();
                        styles.horizontalAlign = currentCellStyle.textAlign.toLowerCase();

                        styles.containsHyperlink = doesCellContainsHyperLink(activeCellElement);
                        currentCellStyle = ASPx.GetCurrentStyle(activeCellElement);
                        if(currentCellStyle)
                            styles.backgroundColor = ASPx.Color.ColorToHexadecimal(currentCellStyle.backgroundColor);
                    }
                }
                styles.bold = styles.bold || ASPx.ElementContainsCssClass(activeCellElement.childNodes[0], "bold");
                styles.italic = styles.italic || ASPx.ElementContainsCssClass(activeCellElement.childNodes[0], "italic");
                styles.underline = styles.underline || ASPx.ElementContainsCssClass(activeCellElement.childNodes[0], "underline");
                styles.strike = styles.strike || ASPx.ElementContainsCssClass(activeCellElement.childNodes[0], "strike");
                styles.textWrap = ASPx.ElementContainsCssClass(activeCellElement.childNodes[0], "wrap");
            }
            return styles;
        }
        function getFontName(fontName) {
            if((fontName.indexOf("'") > -1) || (fontName.indexOf('"') > -1) || (fontName.indexOf('`') > -1))
                fontName = fontName.substr(1, fontName.length - 2);
            return fontName;
        }
        function checkFontBold(fontWeight) {
            var fontIsBold = false;
            if(ASPx.Browser.Chrome || ASPx.Browser.Opera)
                fontIsBold = fontWeight == 'bold';
            else fontIsBold = parseInt(fontWeight) >= 700;
            return fontIsBold;
        }
        function getFontSize(fontSize) {
            var size = 0;
            if(fontSize.indexOf('px') > -1)
                size = Math.round(ASPx.PixelToPoint(fontSize, false));
            else {
                if(fontSize.indexOf('p') > -1)
                    size = fontSize.substr(0, fontSize.indexOf('p'));
                else size = fontSize;
            }
            return size;
        }
        function uncheckButtons(itemTopOrLeft, itemMiddleOrCenter, itemBottomOrRight) {
            if(itemTopOrLeft)
                itemTopOrLeft.SetValue(false);
            if(itemMiddleOrCenter)
                itemMiddleOrCenter.SetValue(false);
            if(itemBottomOrRight)
                itemBottomOrRight.SetValue(false);
        }
        // Fill items
        function updateFillGroup(fillDownDisabled, fillRightDisabled) {
            var ribbon = getRibbonControl();
            var item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("EditingFillDown").id);
            if(item)
                item.SetEnabled(fillDownDisabled);

            item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("EditingFillRight").id);
            if(item)
                item.SetEnabled(fillRightDisabled);
        }
        // Readonly || LockedCell || LockedSheet
        function setItemsEnable(items, allEnabled, isCommandEnabledDelegate) {
            if(items) {
                var subItem = null;
                var enabledCommand = false;

                for(var itemID in items) {
                    var item = items[itemID];

                    if(allEnabled)
                        item.SetEnabled(allEnabled);
                    else {
                        enabledCommand = !!isCommandEnabledDelegate(item.name);
                        item.SetEnabled(enabledCommand);
                    }
                    if(item.items)
                        setItemsEnable(item.items, allEnabled, isCommandEnabledDelegate);
                }
                return subItem;
            }
        }
        // Ribbon state update
        function isHistoryChanged(ribbonManager, history) {
            return (ribbonManager.history.canUndo != history.canUndo) || (ribbonManager.history.canRedo != history.canRedo);
        }
        function updateHistory(ribbonManager, history) {        
            ribbonManager.history.canRedo = history.canRedo;
            ribbonManager.history.canUndo = history.canUndo;
            applyHistoryNavigation(history);
        }
        function updateCalculationMode(ribbonManager, calcMode) {
            ribbonManager.calculationMode = calcMode;
            applyCalculationMode(calcMode);
        }
        function updatePageOrientation(ribbonManager, pageOrientation) {
            ribbonManager.pageOrientation = pageOrientation;
            applyPageOrientation(pageOrientation);
        }
        function updatePaperKind(ribbonManager, paperKind) {
            ribbonManager.paperKind = paperKind;
            applyPaperKind(paperKind);
        }
        function updatePageMargins(ribbonManager, pageMargins) {
            ribbonManager.pageMargins = pageMargins;
            applyPageMargins(pageMargins);
        }
        function updatePageLayoutPrintOptions(showGridLines, showHeading) {
            var ribbon = getRibbonControl();
            var item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("PageSetupPrintGridlines").id);
            if(item)
                item.SetValue(showGridLines);

            item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("PageSetupPrintHeadings").id);
            if(item)
                item.SetValue(showHeading);
        }
        function updateShowOptions(visible) {
            var ribbon = getRibbonControl();
            var item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("ViewShowGridlines").id);
            if(item)
                item.SetValue(visible);
        }
        function updateFreezePanes(isFrozen) {
            var ribbon = getRibbonControl();
            var item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FreezePanes").id);
            if (item)
                item.SetEnabled(!isFrozen);

            item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("ViewUnfreezePanes").id);
            if (item)
                item.SetEnabled(isFrozen);
        }
        function updateSaveButtonState(modified) {
            var ribbon = getRibbonControl();
            var item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("FileSave").id);
            if(item)
                item.SetEnabled(modified);
        }
        function updateTableInfos(ribbonManager, tableInfos) {
            var NAME_INDEX          = 0,
                PROPERTIES_INDEX    = 1,
                RANGE_INDEX         = 2;

            ribbonManager.tableInfos = [];

            if(!tableInfos)
                return;

            ASPx.Data.ForEach(tableInfos, function(rawTableInfo) {
                ribbonManager.tableInfos.push({
                        name: rawTableInfo[NAME_INDEX],
                        properties: rawTableInfo[PROPERTIES_INDEX],
                        range: createTableRange(rawTableInfo[RANGE_INDEX])
                    });
            });

        }
        function createTableRange(range) {
            return new ASPxClientSpreadsheet.Range(range[0], range[1], range[2], range[3]);
        }

        //Drawing box (atm don't used)
        function drawingBoxSelected(enable) {
            var ribbon = getRibbonControl();
            var item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("ArrangeBringForwardCommandGroup").id);
            if(item)
                item.SetEnabled(enable);
            item = ribbon.GetItemByName(ASPxClientSpreadsheet.ServerCommands.getCommandIDByName("ArrangeSendBackwardCommandGroup").id);
            if(item)
                item.SetEnabled(enable);
        }
        
        // Fill items
        this.onFillItemsChanged = function(fillDownDisabled, fillRightDisabled) {
            if(this.isRibbonExist()) {                
                updateFillGroup(fillDownDisabled, fillRightDisabled);
            }
        };
        
        // Active cell
        this.onActiveCellChanged = function() {
            if(this.isRibbonExist())
                activeCellChanged();
        };

        this.onSelectionChanged = function() {
            if(this.isRibbonExist())
                selectionRangeChanged();
        };

        // Editing
        this.onEditingStarted = function() {
            if(this.canEdit() && this.isRibbonExist())
                onEditModeChanged(true);
        };
        this.onEditingStopped = function() {
            if(this.canEdit() && this.isRibbonExist()) {
                onEditModeChanged(false);
                restoreDisabledItems(this);
            }
        };
        
        // ReadOnly || LockedCell || LockedSheet
        this.onItemsEnabledChanged = function() {
            if(this.isRibbonExist()) {
                var ribbon = getRibbonControl();
                if(ribbon.items) {
                    var allEnabled = !spreadsheetControl.readOnly && !spreadsheetControl.sheetLocked && !spreadsheetControl.workbookLocked;

                    var readOnlyEnabledCommands = ASPxClientSpreadsheet.ServerCommands.getReadOnlyModeEnabledCommandsIDs();
                    var lockedSheetEnabledCommands = ASPxClientSpreadsheet.ServerCommands.getLockedSheetEnabledCommandsIDs();
                    var lockedWorkbookEnabledCommands = ASPxClientSpreadsheet.ServerCommands.getLockedWorkbookEnabledCommandsIDs();

                    function isCommandEnabledDelegate(commandName) {
                        if(!ASPxClientSpreadsheet.ServerCommands.getCommandByID(commandName))
                            return true;

                        if(spreadsheetControl.readOnly && !readOnlyEnabledCommands[commandName])
                            return false;

                        if(spreadsheetControl.sheetLocked && !lockedSheetEnabledCommands[commandName])
                            return false;

                        if(spreadsheetControl.workbookLocked && !lockedWorkbookEnabledCommands[commandName])
                            return false;

                        return true;
                    }
                    setItemsEnable(ribbon.items, allEnabled, isCommandEnabledDelegate);
                }
            }
        };
        
        // Full screen mode
        this.setExternalRibbonPositionOnPageTop = function() {
            var externalRibbon = getRibbonControl();
            var ribbonMainElement = externalRibbon.GetMainElement();
            if(ribbonMainElement.style.position == "fixed")
                return;
            ASPx.Attr.ChangeStyleAttribute(ribbonMainElement, "position", "fixed");
            ASPx.Attr.ChangeStyleAttribute(ribbonMainElement, "top", "0px");
            ASPx.Attr.ChangeStyleAttribute(ribbonMainElement, "left", "0px");
            ASPx.Attr.ChangeStyleAttribute(ribbonMainElement, ASPx.Browser.IE ? "zIndex" : "z-index", 10002);

            if(ASPx.IsPercentageSize(ribbonMainElement.style.width))
                this.saveRibbonWidth = ribbonMainElement.style.width;
            else
                this.saveRibbonWidth = externalRibbon.GetWidth();

            ribbonMainElement.style.width = "100%";
            externalRibbon.AdjustControl();
            this.SetRibbonHeight(ribbonMainElement.offsetHeight);
        };
        this.restoreExternalRibbonPositionOnPage = function() {
            var externalRibbon = getRibbonControl();
            var ribbonMainElement = externalRibbon.GetMainElement();
            if(!ribbonMainElement.style.position)
                return;
            ASPx.Attr.RestoreStyleAttribute(ribbonMainElement, "left");
            ASPx.Attr.RestoreStyleAttribute(ribbonMainElement, "top");
            ASPx.Attr.RestoreStyleAttribute(ribbonMainElement, "position");
            ASPx.Attr.RestoreStyleAttribute(ribbonMainElement, ASPx.Browser.IE ? "zIndex" : "z-index");
            if(this.saveRibbonWidth) {
                if(!ASPx.IsNumber(this.saveRibbonWidth) && ASPx.IsPercentageSize(this.saveRibbonWidth))
                    ribbonMainElement.style.width = this.saveRibbonWidth;
                else
                    ribbonMainElement.style.width = this.saveRibbonWidth + "px";
                this.saveRibbonWidth = undefined;
            }
            externalRibbon.AdjustControl();
        };
        this.SetRibbonHeight = function(ribbonHeight) {
            this.saveRibbonHeight = ribbonHeight;
        };
        this.GetRibbonHeight = function() {
            return this.saveRibbonHeight;
        };

        // Drawing box
        this.onDrawingBoxSelected = function() {
            if(this.isRibbonExist()) {
                var enabled = this.spreadsheetControl.getStateController().getSelection().drawingBoxIndex >= 0 && 
                    ASPx.GetNodesByClassName(this.spreadsheetControl.getRenderProvider().getGridContainer(), ASPx.SpreadsheetCssClasses.DrawingBox).length > 1;
                drawingBoxSelected(enabled);
            }
        };
        
        // Ribbon state update
        this.processDocumentResponse = function(response) {
            if(this.isRibbonExist()) {
                if(response.tabControlInfo)
                    applyTabParams();

                if(response.updateRibbonControl)
                    this.onActiveCellChanged();
        
                if(isHistoryChanged(this, response.history) && this.canEdit())
                    updateHistory(this, response.history);

                if(response.calculation && !!response.calculation.updateCalcMode)
                    updateCalculationMode(this, response.calculation.calcMode);
        
                if(!!response.pageOrientation)
                    updatePageOrientation(this, response.pageOrientation);

                if(!!response.paperKind)
                    updatePaperKind(this, response.paperKind);

                if(!!response.pageMargins)
                    updatePageMargins(this, response.pageMargins);

                if(!!response.printOptions)
                    updatePageLayoutPrintOptions(response.printOptions.showGridlines, response.printOptions.showHeadings);

                updateSaveButtonState(!!response.modified);

                updateShowOptions(!response.gridLinesHidden);

                updateFreezePanes(!!response.fp);

                updateTableInfos(this, response.tableInfos);

                this.sheetFilterState = response.sheetFilterState;
            }
        };
              
        // Helpers
        this.isRibbonExist = function() {
            var ribbon = getRibbonControl();
            return !(typeof(ribbon) == "undefined");
        };
        this.canEdit = function() {
            return !spreadsheetControl.readOnly && !spreadsheetControl.sheetLocked;
        };

        // Update Client Command State
        this.onFullScreenCommandExecuted = function(commandId) {
            var ribbon = getRibbonControl();
            var item = ribbon.GetItemByName(commandId);
            if(item)
                item.SetValue(!spreadsheetControl.isInFullScreenMode)
        };
    };
})();