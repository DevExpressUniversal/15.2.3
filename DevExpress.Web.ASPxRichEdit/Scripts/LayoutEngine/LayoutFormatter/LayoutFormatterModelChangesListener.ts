module __aspxRichEdit {
    export class LayoutFormatterModelChangesListener implements IDocumentModelChangesListener {
        private invalidator: LayoutFormatterInvalidator;

        constructor(invalidator: LayoutFormatterInvalidator) {
            this.invalidator = invalidator;
        }

        // IDocumentModelChangesListener
        // text
        public NotifySimpleRunInserted(subDocument: SubDocument, logPosition: number, length: number, characterProperties: MaskedCharacterProperties, characterStyle: CharacterStyle, type: TextRunType) {
            ////Logging.print(LogSource.LayoutFormatterNotifier, `NotifySimpleRunInserted. subDocumentId:${subDocument.id}, logPosition:${logPosition}, length:${length} type:${type}`);
            this.invalidator.onContentInserted(subDocument, logPosition, length, false);
        }
        public NotifyParagraphInserted(subDocument: SubDocument, position: number) {
            ////Logging.print(LogSource.LayoutFormatterNotifier, `NotifyParagraphInserted. subDocumentId:${subDocument.id}, position:${position}`);
            this.invalidator.onContentInserted(subDocument, position, 1, true);
        }
        public NotifySectionInserted(subDocument: SubDocument, position: number) {
            ////Logging.print(LogSource.LayoutFormatterNotifier, `NotifySectionInserted. subDocumentId:${subDocument.id}, position:${position}`);

            const sections: Section[] = subDocument.documentModel.sections;
            const sectionIndex: number = Utils.normedBinaryIndexOf(sections, (s: Section) => s.startLogPosition.value - position);

            this.invalidator.onChangedSection(sections[sectionIndex], sectionIndex);
        }
        public NotifyInlinePictureInserted(subDocument: SubDocument, position: number, id: number, scaleX: number, scaleY: number) {
            ////Logging.print(LogSource.LayoutFormatterNotifier, `NotifyInlinePictureInserted. subDocumentId:${subDocument.id}, position:${position}, id:${id}, id:${scaleX}, id:${scaleY}`);
            this.invalidator.onContentInserted(subDocument, position, 1, false);
        }
        public NotifyLoadInlinePictures(subDocument: SubDocument, imagesInfo: { guid: string; position: number; sourceUrl: string; scaleX: number; scaleY: number }[]) {
            ////Logging.print(LogSource.LayoutFormatterNotifier, `NotifyLoadInlinePictures. subDocumentId:${subDocument.id}, imagesInfo:`, imagesInfo);
        }
        public NotifyInlinePicturesUpdated(subDocument: SubDocument, updatedImagesInfo: { position: number; id: number; scaleX: number; scaleY: number }[]) {
            ////Logging.print(LogSource.LayoutFormatterNotifier, `NotifyInlinePicturesUpdated. subDocumentId:${subDocument.id}, updatedImagesInfo:`, updatedImagesInfo);
            for (let imageInfo of updatedImagesInfo)
                this.invalidator.onIntervalChanged(subDocument, new FixedInterval(imageInfo.position, 1));
        }
        public NotifyIntervalRemoved(subDocument: SubDocument, position: number, length: number) {
            ////Logging.print(LogSource.LayoutFormatterNotifier, `NotifyIntervalRemoved. subDocumentId:${subDocument.id}, position:${position}, length:${length}`);
            this.invalidator.onContentInserted(subDocument, position, -length, false);
        }
        public NotifyInlineObjectRunPropertyChanged(subDocument: SubDocument, property: JSONInlineObjectProperty, position: number, newState: HistoryItemIntervalState<HistoryItemIntervalStateObject>) {
            const interval: FixedInterval = newState.interval();
            ////Logging.print(LogSource.LayoutFormatterNotifier, `NotifyInlineObjectRunPropertyChanged. subDocumentId:${subDocument.id}, property:${property}, position:${position}, newState:`, newState, `, interval:`, interval);
            this.invalidator.onIntervalChanged(subDocument, interval);
        }
        public NotifySectionMerged(subDocument: SubDocument, position: number, getPropertiesFromNext: boolean) {
            ////Logging.print(LogSource.LayoutFormatterNotifier, `NotifySectionMerged. subDocumentId:${subDocument.id}, position:${position}, getPropertiesFromNext:${getPropertiesFromNext}`);

            const sections: Section[] = subDocument.documentModel.sections;
            const sectionIndex: number = Utils.normedBinaryIndexOf(sections, (s: Section) => s.startLogPosition.value - position);

            this.invalidator.onChangedSection(sections[sectionIndex], sectionIndex);
        }
        public NotifyParagraphMerged(subDocument: SubDocument, position: number, getPropertiesFromNext: boolean) {
            ////Logging.print(LogSource.LayoutFormatterNotifier, `NotifyParagraphMerged. subDocumentId:${subDocument.id}, position:${position}, getPropertiesFromNext:${getPropertiesFromNext}`);
            this.invalidator.onContentInserted(subDocument, position, -1, true);
        }
        public NotifyTextBufferChanged(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemTextBufferStateObject>) {
            ////Logging.print(LogSource.LayoutFormatterNotifier, `NotifyTextBufferChanged. subDocumentId:${subDocument.id}, newState:`, newState);
            this.invalidator.onIntervalChanged(subDocument, newState.interval());
        }
        // main formatting
        public NotifyCharacterFormattingChanged(subDocument: SubDocument, property: JSONCharacterFormattingProperty, newState: HistoryItemIntervalState<HistoryItemIntervalStateObject>) {
            ////Logging.print(LogSource.LayoutFormatterNotifier, `NotifyCharacterFormattingChanged. subDocumentId:${subDocument.id}, property:${property}, newState:`, newState);
            this.invalidator.onIntervalChanged(subDocument, newState.interval());
        }
        public NotifyParagraphFormattingChanged(subDocument: SubDocument, property: JSONParagraphFormattingProperty, newState: HistoryItemIntervalState<HistoryItemIntervalStateObject>) {
            ////Logging.print(LogSource.LayoutFormatterNotifier, `NotifyParagraphFormattingChanged. subDocumentId:${subDocument.id}, property:${property}, newState:`, newState);
            this.invalidator.onIntervalChanged(subDocument, newState.interval());
        }
        public NotifySectionFormattingChanged(section: Section, sectionIndex: number, property: JSONSectionProperty, newState: HistoryItemState<HistoryItemSectionStateObject>) {
            ////Logging.print(LogSource.LayoutFormatterNotifier, `NotifySectionFormattingChanged. property:${property}, newState:`, newState);
            this.invalidator.onChangedSection(section, sectionIndex);
        }
        // tabs
        public NotifyTabInserted(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemTabStateObject>) {
            ////Logging.print(LogSource.LayoutFormatterNotifier, `NotifyTabInserted. subDocumentId:${subDocument.id}, newState:`, newState);
            this.invalidator.onIntervalChanged(subDocument, newState.interval());
        }
        public NotifyTabDeleted(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemTabStateObject>) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyTabDeleted. subDocumentId:${subDocument.id}, newState:`, newState);
            this.invalidator.onIntervalChanged(subDocument, newState.interval());
        }
        // bookmarks
        public NotifyBookmarkCreated(subDocument: SubDocument, newState: HistoryItemState<HistoryItemBookmarkStateObject>) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyBookmarkCreated. subDocumentId:${subDocument.id}, newState:`, newState);
            var lenght = newState.objects[0].end - newState.objects[0].start;
            var interval = new FixedInterval(newState.objects[0].start, lenght);
            this.invalidator.onIntervalChanged(subDocument, interval);
        }
        public NotifyBookmarkDeleted(subDocument: SubDocument, newState: HistoryItemState<HistoryItemBookmarkStateObject>) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyBookmarkDeleted. subDocumentId:${subDocument.id}, newState:`, newState);
            var lenght = newState.objects[0].end - newState.objects[0].start;
            var interval = new FixedInterval(newState.objects[0].start, lenght);
            this.invalidator.onIntervalChanged(subDocument, interval);
        }

        // style
        public NotifyCreateStyleLink(paragraphStyleName: string): void {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyCreateStyleLink. paragraphStyleName:${paragraphStyleName}`);
        }
        public NotifyDeleteStyleLink(paragraphStyleName: string): void {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyDeleteStyleLink. paragraphStyleName:${paragraphStyleName}`);
        }
        public NotifyCharacterStyleApplied(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemIntervalStyleStateObject>) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyCharacterStyleApplied. subDocumentId:${subDocument.id}, newState:`, newState);
            this.invalidator.onIntervalChanged(subDocument, newState.interval());
        }
        public NotifyParagraphStyleApplied(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemIntervalStyleStateObject>) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyParagraphStyleApplied. subDocumentId:${subDocument.id}, newState:`, newState);
            this.invalidator.onIntervalChanged(subDocument, newState.interval());
        }
        // numbering list
        public NotifyParagraphNumberingListChanged(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemIntervalStateObject>, oldAbstractNumberingListIndex: number) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyParagraphNumberingListChanged. subDocumentId:${subDocument.id}, newState:`, newState, `, oldAbstractNumberingListIndex:`, oldAbstractNumberingListIndex);
            this.invalidator.onIntervalChanged(subDocument, newState.interval()); // for paragraph when change
            const intervalStart: number = newState.interval().start;
            const paragraphIndex: number = Utils.normedBinaryIndexOf(subDocument.paragraphs, p => p.startLogPosition.value - intervalStart);
            const newAbstractNumberingListIndex: number = subDocument.paragraphs[paragraphIndex].getAbstractNumberingListIndex();
            for (let i = paragraphIndex + 1, paragraph: Paragraph; paragraph = subDocument.paragraphs[i]; i++) {
                const parAbstractNumberingListIndex: number = paragraph.getAbstractNumberingListIndex();
                if (parAbstractNumberingListIndex === oldAbstractNumberingListIndex || parAbstractNumberingListIndex === newAbstractNumberingListIndex)
                    this.invalidator.onIntervalChanged(subDocument, paragraph.getInterval());
            }
        }
        public NotifyAbstractNumberingListAdded(index: number) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyAbstractNumberingListAdded. index:${index}`);
        }
        public NotifyAbstractNumberingListDeleted(index: number) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyAbstractNumberingListDeleted. index:${index}`);
        }
        public NotifyNumberingListAdded(index: number) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyNumberingListAdded. index:${index}`);
        }
        public NotifyNumberingListDeleted(index: number) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyNumberingListDeleted. index:${index}`);
        }
        // list level
        public NotifyListLevelPropertyChanged(property: JSONListLevelProperty, newState: HistoryItemState<HistoryItemListLevelStateObject>) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyListLevelPropertyChanged. property:${property}, newState:`, newState);
            this.invalidator.onListLevelChanged(newState);
        }
        public NotifyListLevelParagraphPropertyChanged(property: JSONParagraphFormattingProperty, newState: HistoryItemState<HistoryItemListLevelUseStateObject>) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyListLevelParagraphPropertyChanged. property:${property}, newState:`, newState);
            this.invalidator.onListLevelChanged(newState);
        }
        public NotifyListLevelCharacterPropertyChanged(property: JSONCharacterFormattingProperty, newState: HistoryItemState<HistoryItemListLevelUseStateObject>) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyListLevelCharacterPropertyChanged. property:${property}, newState:`, newState);
            this.invalidator.onListLevelChanged(newState);
        }
        public NotifyIOverrideListLevelChanged(property: JSONIOverrideListLevelProperty, newState: HistoryItemState<HistoryItemListLevelStateObject>) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyIOverrideListLevelChanged. property:${property}, newState:`, newState);
            this.invalidator.onListLevelChanged(newState);
        }
        // fields
        public NotifyFieldInserted(subDocument: SubDocument, startPosition: number, separatorPosition: number, endPosition: number) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyFieldInserted. subDocumentId:${subDocument.id}, startPosition:${startPosition}, separatorPosition:${separatorPosition}, endPosition:${endPosition}`);
        }
        public NotifyFieldDeleted(subDocument: SubDocument, endPosition: number) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyFieldDeleted. subDocumentId:${subDocument.id}, endPosition:${endPosition}`);
        }
        public NotifyHyperlinkInfoChanged(subDocument: SubDocument, fieldResultInterval: FixedInterval, newHyperlinkInfo: HyperlinkInfo) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyHyperlinkInfoChanged. subDocumentId:${subDocument.id}, fieldResultInterval:`, fieldResultInterval, `, newHyperlinkInfo:`, newHyperlinkInfo);
            this.invalidator.onIntervalChanged(subDocument, fieldResultInterval);
        }
        // global settings
        public NotifyDefaultTabWidthChanged(defaultTabWidth: number) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyDefaultTabWidthChanged. defaultTabWidth:${defaultTabWidth}`);
            this.invalidator.onChangedAllLayout();
        }
        public NotifyPageColorChanged(pageColor: number) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyPageColorChanged. pageColor:${pageColor}`);
            this.invalidator.onChangedAllLayout();
        }
        public NotifyDifferentOddAndEvenPagesChanged(newValue: boolean) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyDifferentOddAndEvenPagesChanged. newValue:${newValue}`);
            this.invalidator.onChangedAllLayout();
        }
        NotifyHeaderFooterCreated(isHeader: boolean, type: HeaderFooterType, subDocumentInfo: HeaderFooterSubDocumentInfoBase) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyHeaderFooterCreated. isHeader:${isHeader}, type:${type}, subDocumentInfo:`, subDocumentInfo);
        }
        NotifyHeaderFooterIndexChanged(sectionIndex: number, isHeader: boolean, type: HeaderFooterType, newIndex: number) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyHeaderFooterIndexChanged. sectionIndex:${sectionIndex}, isHeader:${isHeader}, type:${type}, newIndex:${newIndex}`);
            this.invalidator.onHeaderFooterIndexChanged(sectionIndex, isHeader, type, newIndex);
        }

        // TABLES
        NotifyTableCreated(subDocument: SubDocument, table: Table) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyTableCreated. subDocumentId:${subDocument.id}, table:`, table);
            this.invalidator.onIntervalChanged(subDocument, table.getTopLevelParent().getInterval(), false);
        }
        NotifyTableRemoved(subDocument: SubDocument, startPosition: number, endPosition: number, nestedLevel: number) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyTableRemoved. subDocumentId:${subDocument.id}, startPosition:${startPosition}, endPosition:${endPosition}, nestedLevel:${nestedLevel}`);
            this.invalidator.onIntervalChanged(subDocument, FixedInterval.fromPositions(startPosition, endPosition), true);
        }
        NotifyTableStartPositionShifted(subDocument: SubDocument, table: Table, oldPosition: number, newPosition: number) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyTableStartPositionShifted. subDocumentId:${subDocument.id}, oldPosition:${oldPosition}, newPosition:${newPosition}, table:`, table);
            this.invalidator.onIntervalChanged(subDocument, table.getTopLevelParent().getInterval(), false);
        }
        NotifyTableCellPropertyChanged(subDocument: SubDocument, property: JSONTableCellProperty, newState: HistoryItemState<HistoryItemTableCellStateObject>) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyTableCellPropertyChanged. subDocumentId:${subDocument.id}, property:${property}, newState:`, newState);
            for (let state of newState.objects)
                this.invalidator.onIntervalChanged(subDocument, subDocument.tables[state.tableIndex].getTopLevelParent().getInterval(), false);
        }
        NotifyTablePropertyChanged(subDocument: SubDocument, property: JSONTableProperty, newState: HistoryItemState<HistoryItemTableStateObject>) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyTablePropertyChanged. subDocumentId:${subDocument.id}, property:${property}, newState:`, newState);
            for (let state of newState.objects)
                this.invalidator.onIntervalChanged(subDocument, subDocument.tables[state.tableIndex].getTopLevelParent().getInterval(), false);
        }
        NotifyTableRowPropertyChanged(subDocument: SubDocument, property: JSONTableRowProperty, newState: HistoryItemState<HistoryItemTableRowStateObject>) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyTableRowPropertyChanged. subDocumentId:${subDocument.id}, property:${property}, newState:`, newState);
            for (let state of newState.objects)
                this.invalidator.onIntervalChanged(subDocument, subDocument.tables[state.tableIndex].getTopLevelParent().getInterval(), false);
        }
        NotifyTableCellSplittedHorizontally(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number, rightDirection: boolean) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyTableCellSplittedHorizontally. subDocumentId:${subDocument.id}, rowIndex:${rowIndex}, cellIndex:${cellIndex}, rightDirection:${rightDirection}, table:`, table);
            this.invalidator.onIntervalChanged(subDocument, table.getTopLevelParent().getInterval(), false);
        }
        NotifyTableCellMergedHorizontally(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number, rightDirection: boolean) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyTableCellMergedHorizontally. subDocumentId:${subDocument.id}, rowIndex:${rowIndex}, cellIndex:${cellIndex}, rightDirection:${rightDirection}, table:`, table);
            this.invalidator.onIntervalChanged(subDocument, table.getTopLevelParent().getInterval(), false);
        }
        NotifyTableRowInserted(subDocument: SubDocument, table: Table, rowIndex: number) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyTableRowInserted. subDocumentId:${subDocument.id}, rowIndex:${rowIndex}`);
            this.invalidator.onIntervalChanged(subDocument, table.getTopLevelParent().getInterval(), false);
        }
        NotifyTableRowRemoved(subDocument: SubDocument, table: Table, rowIndex: number) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyTableRowRemoved. subDocumentId:${subDocument.id}, rowIndex:${rowIndex}`);
            this.invalidator.onIntervalChanged(subDocument, table.getTopLevelParent().getInterval(), false);
        }
        NotifyTableCellRemoved(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyTableCellRemoved. subDocumentId:${subDocument.id}, rowIndex:${rowIndex}, cellIndex:${cellIndex}`);
            this.invalidator.onIntervalChanged(subDocument, table.getTopLevelParent().getInterval(), false);
        }
        NotifyTableCellInserted(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyTableCellInserted. subDocumentId:${subDocument.id}, rowIndex:${rowIndex}, cellIndex:${cellIndex}`);
            this.invalidator.onIntervalChanged(subDocument, table.getTopLevelParent().getInterval(), false);
        }
        NotifyTableStyleChanged(subDocument: SubDocument, table: Table, newStyle: TableStyle) {
            //Logging.print(LogSource.LayoutFormatterNotifier, `NotifyTableStyleChanged. subDocumentId:${subDocument.id}, newStyle:`, newStyle);
            this.invalidator.onIntervalChanged(subDocument, table.getTopLevelParent().getInterval(), false);
        }
    }
} 