module __aspxRichEdit {
    export class ModelChangesDispatcher {
        onModelChanged: EventDispatcher<IDocumentModelChangesListener> = new EventDispatcher<IDocumentModelChangesListener>();

        notifyCharacterPropertyChanged(interval: FixedInterval, property: JSONCharacterFormattingProperty, newState: HistoryItemIntervalState<HistoryItemIntervalStateObject>, subDocument: SubDocument) {
            this.onModelChanged.raise("NotifyCharacterFormattingChanged", subDocument, property, newState);
        }
        notifyParagraphPropertyChanged(interval: FixedInterval, property: JSONParagraphFormattingProperty, newState: HistoryItemIntervalState<HistoryItemIntervalStateObject>, subDocument: SubDocument) {
            this.onModelChanged.raise("NotifyParagraphFormattingChanged", subDocument, property, newState);
        }
        notifySectionFormattingChanged(section: Section, sectionIndex: number, property: JSONSectionProperty, newState: HistoryItemState<HistoryItemSectionStateObject>) {
            this.onModelChanged.raise("NotifySectionFormattingChanged", section, sectionIndex, property, newState);
        }
        notifyInlineObjectRunPropertyChanged(interval: FixedInterval, property: JSONInlineObjectProperty, newState: HistoryItemIntervalState<HistoryItemIntervalStateObject>, subDocument: SubDocument) {
            this.onModelChanged.raise("NotifyInlineObjectRunPropertyChanged", subDocument, property, interval.start, newState);
        }
        notifyTextBufferChanged(newState: HistoryItemIntervalState<HistoryItemTextBufferStateObject>, subDocument: SubDocument) {
            this.onModelChanged.raise("NotifyTextBufferChanged", subDocument, newState);
        }
        notifyCharacterStyleApplied(newState: HistoryItemIntervalState<HistoryItemIntervalStyleStateObject>, subDocument: SubDocument) {
            this.onModelChanged.raise("NotifyCharacterStyleApplied", subDocument, newState);
        }
        notifyParagraphStyleApplied(newState: HistoryItemIntervalState<HistoryItemIntervalStyleStateObject>, subDocument: SubDocument) {
            this.onModelChanged.raise("NotifyParagraphStyleApplied", subDocument, newState);
        }
        notifySimpleRunInserted(subDocument: SubDocument, position: number, length: number, characterProperties: MaskedCharacterProperties, characterStyle: CharacterStyle, type: TextRunType) {
            this.onModelChanged.raise("NotifySimpleRunInserted", subDocument, position, length, characterProperties, characterStyle, type);
        }
        notifyParagraphInserted(subDocument: SubDocument, position: number) {
            this.onModelChanged.raise("NotifyParagraphInserted", subDocument, position);
        }
        notifySectionInserted(subDocument: SubDocument, position: number) {
            this.onModelChanged.raise("NotifySectionInserted", subDocument, position);
        }
        notifyIntervalRemoved(subDocument: SubDocument, position: number, length: number) {
            this.onModelChanged.raise("NotifyIntervalRemoved", subDocument, position, length);
        }
        notifyParagraphMerged(subDocument: SubDocument, position: number, getPropertiesFromNext: boolean) {
            this.onModelChanged.raise("NotifyParagraphMerged", subDocument, position, getPropertiesFromNext);
        }
        notifySectionMerged(subDocument: SubDocument, position: number, getPropertiesFromNext: boolean) {
            this.onModelChanged.raise("NotifySectionMerged", subDocument, position, getPropertiesFromNext);
        }
        notifyInlinePictureInserted(subDocument: SubDocument, position: number, id: number, scaleX: number, scaleY: number) {
            this.onModelChanged.raise("NotifyInlinePictureInserted", subDocument, position, id, scaleX, scaleY);
        }
        notifyParagraphNumberingListChanged(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemIntervalStateObject>, oldAbstractNumberingListIndex: number) {
            this.onModelChanged.raise("NotifyParagraphNumberingListChanged", subDocument, newState, oldAbstractNumberingListIndex);
        }
        notifyTabInserted(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemTabStateObject>) {
            this.onModelChanged.raise("NotifyTabInserted", subDocument, newState);
        }
        notifyTabDeleted(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemTabStateObject>) {
            this.onModelChanged.raise("NotifyTabDeleted", subDocument, newState);
        }
        notifyBookmarkCreated(subDocument: SubDocument, newState: HistoryItemState<HistoryItemBookmarkStateObject>) {
            this.onModelChanged.raise("NotifyBookmarkCreated", subDocument, newState);
        }
        notifyBookmarkDeleted(subDocument: SubDocument, newState: HistoryItemState<HistoryItemBookmarkStateObject>) {
            this.onModelChanged.raise("NotifyBookmarkDeleted", subDocument, newState);
        }
        notifyAbstractNumberingListAdded(index: number) {
            this.onModelChanged.raise("NotifyAbstractNumberingListAdded", index);
        }
        notifyAbstractNumberingListDeleted(index: number) {
            this.onModelChanged.raise("NotifyAbstractNumberingListDeleted", index);
        }
        notifyNumberingListAdded(index: number) {
            this.onModelChanged.raise("NotifyNumberingListAdded", index);
        }
        notifyNumberingListDeleted(index: number) {
            this.onModelChanged.raise("NotifyNumberingListDeleted", index);
        }
        notifyListLevelPropertyChanged(property: JSONListLevelProperty, newState: HistoryItemState<HistoryItemListLevelStateObject>) {
            this.onModelChanged.raise("NotifyListLevelPropertyChanged", property, newState);
        }
        notifyListLevelParagraphPropertyChanged(property: JSONParagraphFormattingProperty, newState: HistoryItemState<HistoryItemListLevelUseStateObject>) {
            this.onModelChanged.raise("NotifyListLevelParagraphPropertyChanged", property, newState);
        }
        notifyListLevelCharacterPropertyChanged(property: JSONCharacterFormattingProperty, newState: HistoryItemState<HistoryItemListLevelUseStateObject>) {
            this.onModelChanged.raise("NotifyListLevelCharacterPropertyChanged", property, newState);
        }
        notifyIOverrideListLevelChanged(property: JSONIOverrideListLevelProperty, newState: HistoryItemState<HistoryItemListLevelStateObject>) {
            this.onModelChanged.raise("NotifyIOverrideListLevelChanged", property, newState);
        }
        notifyCreateStyleLink(paragraphStyleName: string): void {
            this.onModelChanged.raise("NotifyCreateStyleLink", paragraphStyleName);
        }
        notifyDeleteStyleLink(paragraphStyleName: string): void {
            this.onModelChanged.raise("NotifyDeleteStyleLink", paragraphStyleName);
        }

        notifyLoadInlinePictures(subDocument: SubDocument, imagesToLoad: { guid: string; position: number; sourceUrl: string; scaleX: number; scaleY: number }[]) {
            this.onModelChanged.raise("NotifyLoadInlinePictures", subDocument, imagesToLoad);
        }
        notifyInlinePicturesUpdated(subDocument: SubDocument, updatedImagesInfo: { position: number; id: number; scaleX: number; scaleY: number }[]) {
            this.onModelChanged.raise("NotifyInlinePicturesUpdated", subDocument, updatedImagesInfo);
        }
        notifyFieldInserted(subDocument: SubDocument, startPosition: number, separatorPosition: number, endPosition: number) {
            this.onModelChanged.raise("NotifyFieldInserted", subDocument, startPosition, separatorPosition, endPosition);
        }
        notifyFieldDeleted(subDocument: SubDocument, endPosition: number) {
            this.onModelChanged.raise("NotifyFieldDeleted", subDocument, endPosition);
        }
        notifyHyperlinkInfoChanged(subDocument: SubDocument, fieldResultInterval: FixedInterval, newHyperlinkInfo: HyperlinkInfo) {
            this.onModelChanged.raise("NotifyHyperlinkInfoChanged", subDocument, fieldResultInterval, newHyperlinkInfo);
        }
        notifyDefaultTabWidthChanged(defaultTabWidth: number) {
            this.onModelChanged.raise("NotifyDefaultTabWidthChanged", defaultTabWidth);
        }
        notifyPageColorChanged(pageColor: number) {
            this.onModelChanged.raise("NotifyPageColorChanged", pageColor);
        }
        notifyHeaderFooterCreated(isHeader: boolean, type: HeaderFooterType, subDocumentInfo: HeaderFooterSubDocumentInfoBase) {
            this.onModelChanged.raise("NotifyHeaderFooterCreated", isHeader, type, subDocumentInfo);
        }
        notifyHeaderFooterIndexChanged(sectionIndex: number, isHeader: boolean, type: HeaderFooterType, newIndex: number) {
            this.onModelChanged.raise("NotifyHeaderFooterIndexChanged", sectionIndex, isHeader, type, newIndex);
        }
        notifyDifferentOddAndEvenPagesChanged(newValue: boolean) {
            this.onModelChanged.raise("NotifyDifferentOddAndEvenPagesChanged", newValue);
        }
        /* Table */
        notifyTableCreated(subDocument: SubDocument, table: Table) {
            this.onModelChanged.raise("NotifyTableCreated", subDocument, table);
        }
        notifyTableRemoved(subDocument: SubDocument, startPosition: number, endPosition: number, nestedLevel: number) {
            this.onModelChanged.raise("NotifyTableRemoved", subDocument, startPosition, endPosition, nestedLevel);
        }
        notifyTableStartPositionShifted(subDocument: SubDocument, table: Table, oldPosition: number, newPosition: number) {
            this.onModelChanged.raise("NotifyTableStartPositionShifted", subDocument, table, oldPosition, newPosition);
        }
        notifyTableCellPropertyChanged(subDocument: SubDocument, property: JSONTableCellProperty, newState: HistoryItemState<HistoryItemTableCellStateObject>) {
            this.onModelChanged.raise("NotifyTableCellPropertyChanged", subDocument, property, newState);
        }
        notifyTablePropertyChanged(subDocument: SubDocument, property: JSONTableProperty, newState: HistoryItemState<HistoryItemTableStateObject>) {
            this.onModelChanged.raise("NotifyTablePropertyChanged", subDocument, property, newState);
        }
        notifyTableRowPropertyChanged(subDocument: SubDocument, property: JSONTableRowProperty, newState: HistoryItemState<HistoryItemTableRowStateObject>) {
            this.onModelChanged.raise("NotifyTableRowPropertyChanged", subDocument, property, newState);
        }
        notifyTableCellSplittedHorizontally(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number, rightDirection: boolean) {
            this.onModelChanged.raise("NotifyTableCellSplittedHorizontally", subDocument, table, rowIndex, cellIndex, rightDirection);
        }
        notifyTableCellMergedHorizontally(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number, rightDirection: boolean) {
            this.onModelChanged.raise("NotifyTableCellMergedHorizontally", subDocument, table, rowIndex, cellIndex, rightDirection);
        }
        notifyTableRowInserted(subDocument: SubDocument, table: Table, rowIndex: number) {
            this.onModelChanged.raise("NotifyTableRowInserted", subDocument, table, rowIndex);
        }
        notifyTableRowRemoved(subDocument: SubDocument, table: Table, rowIndex: number, direction: boolean) {
            this.onModelChanged.raise("NotifyTableRowRemoved", subDocument, table, rowIndex);
        }
        notifyTableCellRemoved(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number) {
            this.onModelChanged.raise("NotifyTableCellRemoved", subDocument, table, rowIndex, cellIndex);
        }
        notifyTableCellInserted(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number) {
            this.onModelChanged.raise("NotifyTableCellInserted", subDocument, table, rowIndex, cellIndex);
        }
        notifyTableStyleChanged(subDocument: SubDocument, table: Table, newStyle: TableStyle) {
            this.onModelChanged.raise("NotifyTableStyleChanged", subDocument, table, newStyle);
        }
    }
}