module __aspxRichEdit {
    export class ServerDispatcherModelChangesListener implements IDocumentModelChangesListener {
        dispatcher: ServerDispatcher;
        constructor(dispatcher: ServerDispatcher) {
            this.dispatcher = dispatcher;
        }
        NotifySimpleRunInserted(subDocument: SubDocument, position: number, length: number, characterProperties: MaskedCharacterProperties, characterStyle: CharacterStyle, type: TextRunType) {
            var text = this.dispatcher.control.model.activeSubDocument.getText(new FixedInterval(position, length));
            var characterPropertiesJSON = JSONMaskedCharacterPropertiesConverter.convertToJSON(characterProperties);
            if(this.dispatcher.canExtendPreviousTextInsertedRequest(subDocument, position, characterPropertiesJSON, characterStyle.styleName, type)) {
                this.dispatcher.lastRequestInQueue["text"] += text;
                this.dispatcher.lastRequestInQueue.serverParams["length"] += length;
            }
            else
                this.dispatcher.pushInsertTextRequest(subDocument, position, length, characterPropertiesJSON, characterStyle, type, text);
        }
        NotifyParagraphInserted(subDocument: SubDocument, position: number) {
            var paragraph = this.dispatcher.control.model.activeSubDocument.getParagraphByPosition(position);
            this.dispatcher.pushRequest(CommandType.InsertParagraph, {
                position: position,
                paragraphProperties: JSONMaskedParagraphPropertiesConverter.convertToJSON(paragraph.maskedParagraphProperties),
                styleName: paragraph.paragraphStyle.styleName,
                numberingListIndex: paragraph.numberingListIndex,
                listLevelIndex: paragraph.listLevelIndex,
                sdid: subDocument.id
            });
        }
        NotifySectionInserted(subDocument: SubDocument, position: number) {
            var section = this.dispatcher.control.model.getSectionByPosition(position);
            this.dispatcher.pushRequest(CommandType.InsertSection, {
                position: position,
                sectionProperties: JSONSectionPropertiesConverter.convertToJSON(section.sectionProperties),
                sdid: subDocument.id
            });
        }
        NotifyInlinePictureInserted(subDocument: SubDocument, position: number, id: number, scaleX: number, scaleY: number) {
            this.dispatcher.pushRequest(CommandType.InsertInlinePicture, {
                position: position,
                id: id,
                scaleX: scaleX,
                scaleY: scaleY,
                sdid: subDocument.id
            });
        }
        NotifyLoadInlinePictures(subDocument: SubDocument, imagesInfo: { guid: string; position: number; sourceUrl: string; scaleX: number; scaleY: number }[]) {
            this.dispatcher.pushRequest(CommandType.LoadInlinePictures, {
                imagesInfo: imagesInfo,
                sdid: subDocument.id
            });
        }
        NotifyInlinePicturesUpdated(subDocument: SubDocument, updatedImagesInfo: { position: number; id: number; scaleX: number; scaleY: number }[]) {
            this.dispatcher.pushRequest(CommandType.UpdateInlinePictures, {
                updatedImagesInfo: updatedImagesInfo,
                sdid: subDocument.id
            });
        }
        NotifyIntervalRemoved(subDocument: SubDocument, position: number, length: number) {
            this.dispatcher.pushRequest(CommandType.DeleteRuns, {
                position: position,
                length: length,
                sdid: subDocument.id
            });
        }
        NotifySectionMerged(subDocument: SubDocument, position: number, getPropertiesFromNext: boolean) {
            this.dispatcher.pushRequest(CommandType.MergeSections, {
                position: position,
                getPropertiesFromNext: getPropertiesFromNext,
                sdid: subDocument.id
            });
        }
        NotifyParagraphMerged(subDocument: SubDocument, position: number, getPropertiesFromNext: boolean) {
            this.dispatcher.pushRequest(CommandType.MergeParagraphs, {
                position: position,
                getPropertiesFromNext: getPropertiesFromNext,
                sdid: subDocument.id
            });
        }
        NotifyCharacterFormattingChanged(subDocument: SubDocument, property: JSONCharacterFormattingProperty, newState: HistoryItemIntervalState<HistoryItemIntervalStateObject>) {
            if(property === JSONCharacterFormattingProperty.UseValue) {
                this.dispatcher.pushRequest(CommandType.ChangeCharacterPropertiesUseValue, {
                    state: newState.toJSON(),
                    sdid: subDocument.id
                });
            }
            else {
                this.dispatcher.pushRequest(CommandType.ChangeCharacterProperties, {
                    property: property,
                    state: newState.toJSON(),
                    sdid: subDocument.id
                });
            }
        }
        NotifyTabInserted(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemTabStateObject>) {
            this.dispatcher.pushRequest(CommandType.InsertTabToParagraph, {
                state: newState.toJSON(),
                sdid: subDocument.id
            });
        }
        NotifyTabDeleted(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemTabStateObject>) {
            this.dispatcher.pushRequest(CommandType.DeleteTabAtParagraph, {
                state: newState.toJSON(),
                sdid: subDocument.id
            });
        }
        NotifyParagraphFormattingChanged(subDocument: SubDocument, property: JSONParagraphFormattingProperty, newState: HistoryItemIntervalState<HistoryItemIntervalStateObject>) {
            if(property === JSONParagraphFormattingProperty.UseValue) {
                this.dispatcher.pushRequest(CommandType.ChangeParagraphPropertiesUseValue, {
                    state: newState.toJSON(),
                    sdid: subDocument.id
                });
            }
            else {
                this.dispatcher.pushRequest(CommandType.ChangeParagraphProperties, {
                    property: property,
                    state: newState.toJSON(),
                    sdid: subDocument.id
                });
            }
        }
        NotifySectionFormattingChanged(section: Section, sectionIndex: number, property: JSONSectionProperty, newState: HistoryItemState<HistoryItemSectionStateObject>) {
            this.dispatcher.pushRequest(CommandType.ChangeSectionProperties, {
                property: property,
                state: newState.toJSON()
            });
        }
        NotifyInlineObjectRunPropertyChanged(subDocument: SubDocument, property: JSONInlineObjectProperty, position: number, newState: HistoryItemIntervalState<HistoryItemIntervalStateObject>) {
            this.dispatcher.pushRequest(CommandType.ChangeInlineObjectProperties, {
                property: property,
                state: newState.toJSON(),
                sdid: subDocument.id
            });
        }
        NotifyTextBufferChanged(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemTextBufferStateObject>) {
            var interval = newState.interval();
            this.dispatcher.pushRequest(CommandType.ChangeTextBuffer, {
                start: interval.start,
                length: interval.length,
                state: newState.toJSON(),
                sdid: subDocument.id
            });
        }
        NotifyCharacterStyleApplied(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemIntervalStyleStateObject>) {
            var interval = newState.interval();
            this.dispatcher.pushRequest(CommandType.ApplyCharacterStyle, {
                start: interval.start,
                length: interval.length,
                state: newState.toJSON(),
                sdid: subDocument.id
            });
        }
        NotifyParagraphStyleApplied(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemIntervalStyleStateObject>) {
            var interval = newState.interval();
            this.dispatcher.pushRequest(CommandType.ApplyParagraphStyle, {
                start: interval.start,
                length: interval.length,
                state: newState.toJSON(),
                sdid: subDocument.id
            });
        }

        NotifyParagraphNumberingListChanged(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemIntervalStateObject>, oldAbstractNumberingListIndex: number) {
            this.dispatcher.pushRequest(CommandType.ApplyNumberingList, {
                state: newState.toJSON(),
                sdid: subDocument.id
            });
        }
        NotifyAbstractNumberingListAdded(index: number) {
            var numberingList = this.dispatcher.control.model.abstractNumberingLists[index];
            var listLevelsInfo = [];
            for(var listLevel: ListLevel, i = 0; listLevel = numberingList.levels[i]; i++) {
                listLevelsInfo.push({
                    characterProperties: JSONMaskedCharacterPropertiesConverter.convertToJSON(listLevel.getCharacterProperties()),
                    paragraphProperties: JSONMaskedParagraphPropertiesConverter.convertToJSON(listLevel.getParagraphProperties()),
                    listLevelProperties: JSONListLevelPropertiesConverter.convertToJSON(listLevel.getListLevelProperties())
                });
            }
            this.dispatcher.pushRequest(CommandType.AddAbstractNumberingList, {
                deleted: numberingList.deleted,
                levels: listLevelsInfo,
                innerId: numberingList.innerId
            });
        }
        NotifyAbstractNumberingListDeleted(index: number) {
            this.dispatcher.pushRequest(CommandType.DeleteAbstractNumberingList, {
                index: index
            });
        }
        NotifyNumberingListAdded(index: number) {
            var numberingList = this.dispatcher.control.model.numberingLists[index];
            var listLevelsInfo = [];
            for(var listLevel: IOverrideListLevel, i = 0; listLevel = numberingList.levels[i]; i++) {
                if(listLevel instanceof OverrideListLevel) {
                    listLevelsInfo.push({
                        characterProperties: JSONMaskedCharacterPropertiesConverter.convertToJSON(listLevel.getCharacterProperties()),
                        paragraphProperties: JSONMaskedParagraphPropertiesConverter.convertToJSON(listLevel.getParagraphProperties()),
                        listLevelProperties: JSONListLevelPropertiesConverter.convertToJSON(listLevel.getListLevelProperties()),
                        overrideStart: listLevel.overrideStart
                    });
                }
                else {
                    listLevelsInfo.push({
                        overrideStart: listLevel.overrideStart,
                        newStart: listLevel.getNewStart()
                    });
                }
            }
            this.dispatcher.pushRequest(CommandType.AddNumberingList, {
                abstractNumberingListIndex: numberingList.abstractNumberingListIndex,
                deleted: numberingList.deleted,
                innerId: numberingList.innerId,
                levels: listLevelsInfo
            });
        }
        NotifyNumberingListDeleted(index: number) {
            this.dispatcher.pushRequest(CommandType.DeleteNumberingList, {
                index: index
            });
        }
        NotifyListLevelPropertyChanged(property: JSONListLevelProperty, newState: HistoryItemState<HistoryItemListLevelStateObject>) {
            this.dispatcher.pushRequest(CommandType.ChangeListLevelProperties, {
                state: newState.toJSON(),
                property: property
            });
        }
        NotifyListLevelParagraphPropertyChanged(property: JSONParagraphFormattingProperty, newState: HistoryItemState<HistoryItemListLevelUseStateObject>) {
            this.dispatcher.pushRequest(CommandType.ChangeListLevelParagraphProperties, {
                state: newState.toJSON(),
                property: property
            });
        }
        NotifyListLevelCharacterPropertyChanged(property: JSONCharacterFormattingProperty, newState: HistoryItemState<HistoryItemListLevelUseStateObject>) {
            this.dispatcher.pushRequest(CommandType.ChangeListLevelCharacterProperties, {
                state: newState.toJSON(),
                property: property
            });
        }
        NotifyIOverrideListLevelChanged(property: JSONIOverrideListLevelProperty, newState: HistoryItemState<HistoryItemListLevelStateObject>) {
            this.dispatcher.pushRequest(CommandType.ChangeIOverrideListLevel, {
                state: newState.toJSON(),
                property: property
            });
        }

        NotifyCreateStyleLink(paragraphStyleName: string): void {
            this.dispatcher.pushRequest(CommandType.CreateStyleLink, {
                styleName: paragraphStyleName
            });
        }
        NotifyDeleteStyleLink(paragraphStyleName: string): void {
            this.dispatcher.pushRequest(CommandType.DeleteStyleLink, {
                styleName: paragraphStyleName
            });
        }

        NotifyFieldInserted(subDocument: SubDocument, startPosition: number, separatorPosition: number, endPosition: number) {
            this.dispatcher.pushRequest(CommandType.InsertField, {
                start: startPosition,
                separator: separatorPosition,
                end: endPosition,
                sdid: subDocument.id
            });
        }

        NotifyFieldDeleted(subDocument: SubDocument, endPosition: number) {
            this.dispatcher.pushRequest(CommandType.DeleteField, {
                end: endPosition,
                sdid: subDocument.id
            });
        }

        NotifyHyperlinkInfoChanged(subDocument: SubDocument, fieldResultInterval: FixedInterval, newHyperlinkInfo: HyperlinkInfo) {
            if(newHyperlinkInfo) {
                this.dispatcher.pushRequest(CommandType.HyperlinkInfoChanged, {
                    end: fieldResultInterval.end() + 1,
                    uri: ServerDispatcher.prepareTextForRequest(newHyperlinkInfo.uri),
                    anchor: ServerDispatcher.prepareTextForRequest(newHyperlinkInfo.anchor),
                    tip: ServerDispatcher.prepareTextForRequest(newHyperlinkInfo.tip),
                    visited: newHyperlinkInfo.visited,
                    sdid: subDocument.id
                });
            }
            else {
                this.dispatcher.pushRequest(CommandType.HyperlinkInfoChanged, {
                    end: fieldResultInterval.end() + 1,
                    noInfo: 1,
                    sdid: subDocument.id
                });
            }
        }
        NotifyDefaultTabWidthChanged(defaultTabWidth: number) {
            this.dispatcher.pushRequest(CommandType.ChangeDefaultTabWidth, {
                defaultTabWidth: defaultTabWidth,
                sdid: 0 // TODO
            });
        }
        NotifyPageColorChanged(pageColor: number) {
            this.dispatcher.pushRequest(CommandType.ChangePageColor, {
                pageColor: pageColor
            });
        }
        NotifyDifferentOddAndEvenPagesChanged(newValue: boolean) {
            this.dispatcher.pushRequest(CommandType.ChangeDifferentOddAndEvenPages, {
                newValue: newValue
            });
        }
        NotifyHeaderFooterCreated(isHeader: boolean, type: HeaderFooterType, subDocumentInfo: HeaderFooterSubDocumentInfoBase) {
            this.dispatcher.pushRequest(isHeader ? CommandType.CreateHeader : CommandType.CreateFooter, {
                type: type
            });
        }
        NotifyHeaderFooterIndexChanged(sectionIndex: number, isHeader: boolean, type: HeaderFooterType, newIndex: number) {
            this.dispatcher.pushRequest(isHeader ? CommandType.ChangeHeaderIndex : CommandType.ChangeFooterIndex, {
                sectionIndex: sectionIndex,
                type: type,
                newObjectIndex: newIndex
            });
        }
        NotifyBookmarkCreated(subDocument: SubDocument, newState: HistoryItemState<HistoryItemBookmarkStateObject>) {
            this.dispatcher.pushRequest(CommandType.CreateBookmark, {
                state: newState.toJSON(),
                sdid: subDocument.id
            });
        }
        NotifyBookmarkDeleted(subDocument: SubDocument, newState: HistoryItemState<HistoryItemBookmarkStateObject>) {
            this.dispatcher.pushRequest(CommandType.DeleteBookmark, {
                state: newState.toJSON(),
                sdid: subDocument.id
            });
        }
        NotifyTableCreated(subDocument: SubDocument, table: Table) {
            this.dispatcher.pushRequest(CommandType.CreateTable, {
                position: table.getStartPosition(),
                properties: JSONTablePropertiesConverter.convertToJSON(table.properties),
                styleName: table.style ? table.style.styleName : null,
                preferredWidth: JSONTableWidthUnitConverter.convertToJSON(table.preferredWidth),
                lookTypes: table.lookTypes,
                rows: this.rowsToJSON(table),
                sdid: subDocument.id
            });
        }

        NotifyTableRemoved(subDocument: SubDocument, startPosition: number, endPosition: number, nestedLevel: number) {
            this.dispatcher.pushRequest(CommandType.RemoveTable, {
                tablePosition: this.tablePositionToJSONCore(startPosition, nestedLevel),
                sdid: subDocument.id
            });
        }
        NotifyTableStartPositionShifted(subDocument: SubDocument, table: Table, oldPosition: number, newPosition: number) {
            this.dispatcher.pushRequest(CommandType.ShiftTableStartPosition, {
                tablePosition: this.tablePositionToJSONCore(oldPosition, table.nestedLevel),
                sdid: subDocument.id,
                newPosition: newPosition
            });
        }
        NotifyTableCellPropertyChanged(subDocument: SubDocument, property: JSONTableCellProperty, newState: HistoryItemState<HistoryItemTableCellStateObject>) {
            switch(property) {
                case JSONTableCellProperty.ColumnSpan:
                case JSONTableCellProperty.PreferredWidth:
                case JSONTableCellProperty.VerticalMerging:
                    this.dispatcher.pushRequest(CommandType.ChangeTableCell, {
                        property: property,
                        state: newState.toJSON(),
                        sdid: subDocument.id
                    });
                    break;
                default:
                    this.dispatcher.pushRequest(CommandType.ChangeTableCellProperty, {
                        property: property,
                        state: newState.toJSON(),
                        sdid: subDocument.id
                    });
                    break;
            }
        }
        NotifyTablePropertyChanged(subDocument: SubDocument, property: JSONTableProperty, newState: HistoryItemState<HistoryItemTableStateObject>) {
            switch(property) {
                case JSONTableProperty.PreferredWidth:
                case JSONTableProperty.TableLookTypes:
                    this.dispatcher.pushRequest(CommandType.ChangeTable, {
                        property: property,
                        state: newState.toJSON(),
                        sdid: subDocument.id
                    });
                    break;
                default:
                    this.dispatcher.pushRequest(CommandType.ChangeTableProperty, {
                        property: property,
                        state: newState.toJSON(),
                        sdid: subDocument.id
                    });
                    break;
            }
        }
        NotifyTableRowPropertyChanged(subDocument: SubDocument, property: JSONTableRowProperty, newState: HistoryItemState<HistoryItemTableRowStateObject>) {
            switch(property) {
                case JSONTableRowProperty.GridAfter:
                case JSONTableRowProperty.GridBefore:
                case JSONTableRowProperty.Height:
                case JSONTableRowProperty.WidthAfter:
                case JSONTableRowProperty.WidthBefore:
                    this.dispatcher.pushRequest(CommandType.ChangeTableRow, {
                        property: property,
                        state: newState.toJSON(),
                        sdid: subDocument.id
                    });
                    break;
                default:
                    this.dispatcher.pushRequest(CommandType.ChangeTableRowProperty, {
                        property: property,
                        state: newState.toJSON(),
                        sdid: subDocument.id
                    });
                    break;
            }
        }
        NotifyTableCellSplittedHorizontally(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number, rightDirection: boolean) {
            var newCell = table.rows[rowIndex].cells[rightDirection ? (cellIndex + 1) : (cellIndex - 1)];
            this.dispatcher.pushRequest(CommandType.SplitTableCellHorizontally, {
                tablePosition: this.tablePositionToJSON(table),
                rowIndex: rowIndex,
                cellIndex: rightDirection ? cellIndex : (cellIndex - 1),
                rightDirection: rightDirection,
                newCellInfo: this.cellInfoToJSON(newCell),
                sdid: subDocument.id
            });
        }
        NotifyTableCellMergedHorizontally(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number, rightDirection: boolean) {
            this.dispatcher.pushRequest(CommandType.MergeTableCellHorizontally, {
                tablePosition: this.tablePositionToJSON(table),
                rowIndex: rowIndex,
                cellIndex: rightDirection ? cellIndex : (cellIndex + 1),
                rightDirection: rightDirection,
                sdid: subDocument.id
            });
        }
        NotifyTableRowInserted(subDocument: SubDocument, table: Table, rowIndex: number) {
            var row = table.rows[rowIndex];
            this.dispatcher.pushRequest(CommandType.InsertTableRow, {
                tablePosition: this.tablePositionToJSON(table),
                newRowIndex: rowIndex,
                newRowInfo: this.rowInfoToJSON(row),
                newRowCells: this.cellsToJSON(row),
                sdid: subDocument.id
            });
        }
        NotifyTableRowRemoved(subDocument: SubDocument, table: Table, rowIndex: number) {
            this.dispatcher.pushRequest(CommandType.RemoveTableRow, {
                tablePosition: this.tablePositionToJSON(table),
                rowIndex: rowIndex,
                sdid: subDocument.id
            });
        }
        NotifyTableCellRemoved(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number) {
            this.dispatcher.pushRequest(CommandType.RemoveTableCell, {
                tablePosition: this.tablePositionToJSON(table),
                rowIndex: rowIndex,
                cellIndex: cellIndex,
                sdid: subDocument.id
            });
        }
        NotifyTableCellInserted(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number) {
            var cell = table.rows[rowIndex].cells[cellIndex];
            this.dispatcher.pushRequest(CommandType.InsertTableCell, {
                tablePosition: this.tablePositionToJSON(table),
                rowIndex: rowIndex,
                newCellIndex: cellIndex,
                newCellInfo: this.cellInfoToJSON(cell),
                newCellStartPosition: cell.startParagraphPosition.value,
                newCellEndPosition: cell.endParagrapPosition.value,
                sdid: subDocument.id
            });
        }
        NotifyTableStyleChanged(subDocument: SubDocument, table: Table, newStyle: TableStyle) {
            this.dispatcher.pushRequest(CommandType.ApplyTableStyle, {
                tablePosition: this.tablePositionToJSON(table),
                styleName: newStyle ? newStyle.styleName : null,
                sdid: subDocument.id
            });
        }

        private rowsToJSON(table: Table): any[] {
            var result = [];
            for(let rowIndex = 0, row: TableRow; row = table.rows[rowIndex]; rowIndex++) {
                result.push({
                    rowInfo: this.rowInfoToJSON(row),
                    cells: this.cellsToJSON(row)
                });
            }
            return result;
        }
        private cellsToJSON(row: TableRow): any[] {
            var result = [];
            for(let cellIndex = 0, cell: TableCell; cell = row.cells[cellIndex]; cellIndex++)
                result.push(this.cellInfoToJSON(cell));
            return result;
        }
        private rowInfoToJSON(row: TableRow): any {
            return {
                gridAfter: row.gridAfter,
                gridBefore: row.gridBefore,
                height: JSONTableHeightUnitConverter.convertToJSON(row.height),
                properties: JSONTableRowPropertiesConverter.convertToJSON(row.properties),
                tablePropertiesException: JSONTablePropertiesConverter.convertToJSON(row.tablePropertiesException),
                widthAfter: JSONTableWidthUnitConverter.convertToJSON(row.widthAfter),
                widthBefore: JSONTableWidthUnitConverter.convertToJSON(row.widthBefore)
            }
        }
        private cellInfoToJSON(cell: TableCell): any {
            return {
                columnSpan: cell.columnSpan,
                conditionalFormatting: cell.conditionalFormatting,
                startPosition: cell.startParagraphPosition.value,
                endPosition: cell.endParagrapPosition.value,
                preferredWidth: JSONTableWidthUnitConverter.convertToJSON(cell.preferredWidth),
                properties: JSONTableCellPropertiesConverter.convertToJSON(cell.properties),
                styleName: cell.style ? cell.style.styleName : undefined,
                verticalMerging: cell.verticalMerging
            };
        }
        private tablePositionToJSON(table: Table): number[]{
            return this.tablePositionToJSONCore(table.getStartPosition(), table.nestedLevel);
        }
        private tablePositionToJSONCore(startPosition: number, nestedLevel: number): number[] {
            return [startPosition, nestedLevel];
        }
    }
}