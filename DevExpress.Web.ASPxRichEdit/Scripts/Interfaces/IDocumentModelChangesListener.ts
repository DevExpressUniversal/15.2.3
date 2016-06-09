module __aspxRichEdit {
    export interface IDocumentModelChangesListener {
        NotifySimpleRunInserted(subDocument: SubDocument, position: number, length: number, characterProperties: MaskedCharacterProperties, characterStyle: CharacterStyle, type: TextRunType);
        NotifyParagraphInserted(subDocument: SubDocument, position: number);
        NotifySectionInserted(subDocument: SubDocument, position: number);
        NotifyInlinePictureInserted(subDocument: SubDocument, position: number, id: number, scaleX: number, scaleY: number);
        NotifyLoadInlinePictures(subDocument: SubDocument, imagesInfo: { guid: string; position: number; sourceUrl: string; scaleX: number; scaleY: number }[]);
        NotifyInlinePicturesUpdated(subDocument: SubDocument, updatedImagesInfo: { position: number; id: number; scaleX: number; scaleY: number }[]);

        NotifyIntervalRemoved(subDocument: SubDocument, position: number, length: number);
        NotifySectionMerged(subDocument: SubDocument, position: number, getPropertiesFromNext: boolean);
        NotifyParagraphMerged(subDocument: SubDocument, position: number, getPropertiesFromNext: boolean);

        NotifyCharacterFormattingChanged(subDocument: SubDocument, property: JSONCharacterFormattingProperty, newState: HistoryItemIntervalState<HistoryItemIntervalStateObject>);
        NotifyParagraphFormattingChanged(subDocument: SubDocument, property: JSONParagraphFormattingProperty, newState: HistoryItemIntervalState<HistoryItemIntervalStateObject>);
        NotifySectionFormattingChanged(section: Section, sectionIndex: number, property: JSONSectionProperty, newState: HistoryItemState<HistoryItemSectionStateObject>);
        NotifyInlineObjectRunPropertyChanged(subDocument: SubDocument, property: JSONInlineObjectProperty, position: number, newState: HistoryItemIntervalState<HistoryItemIntervalStateObject>);

        NotifyTabInserted(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemTabStateObject>);
        NotifyTabDeleted(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemTabStateObject>);

        NotifyCreateStyleLink(paragraphStyleName: string);
        NotifyDeleteStyleLink(paragraphStyleName: string);

        NotifyCharacterStyleApplied(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemIntervalStyleStateObject>);
        NotifyParagraphStyleApplied(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemIntervalStyleStateObject>);

        NotifyTextBufferChanged(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemTextBufferStateObject>);

        // Numbering
        NotifyParagraphNumberingListChanged(subDocument: SubDocument, newState: HistoryItemIntervalState<HistoryItemIntervalStateObject>, oldAbstractNumberingListIndex: number);
        NotifyAbstractNumberingListAdded(index: number);
        NotifyAbstractNumberingListDeleted(index: number);
        NotifyNumberingListAdded(index: number);
        NotifyNumberingListDeleted(index: number);
        NotifyListLevelPropertyChanged(property: JSONListLevelProperty, newState: HistoryItemState<HistoryItemListLevelStateObject>);
        NotifyListLevelParagraphPropertyChanged(property: JSONParagraphFormattingProperty, newState: HistoryItemState<HistoryItemListLevelUseStateObject>);
        NotifyListLevelCharacterPropertyChanged(property: JSONCharacterFormattingProperty, newState: HistoryItemState<HistoryItemListLevelUseStateObject>);
        NotifyIOverrideListLevelChanged(property: JSONIOverrideListLevelProperty, newState: HistoryItemState<HistoryItemListLevelStateObject>);

        // Fields
        NotifyFieldInserted(subDocument: SubDocument, startPosition: number, separatorPosition: number, endPosition: number);
        NotifyFieldDeleted(subDocument: SubDocument, endPosition: number);
        NotifyHyperlinkInfoChanged(subDocument: SubDocument, fieldResultInterval: FixedInterval, newHyperlinkInfo: HyperlinkInfo);

        NotifyDefaultTabWidthChanged(defaultTabWidth: number);
        NotifyPageColorChanged(pageColor: number);
        NotifyDifferentOddAndEvenPagesChanged(newValue: boolean);

        NotifyHeaderFooterCreated(isHeader: boolean, type: HeaderFooterType, subDocumentInfo: HeaderFooterSubDocumentInfoBase);
        NotifyHeaderFooterIndexChanged(sectionIndex: number, isHeader: boolean, type: HeaderFooterType, newIndex: number);

        // Bookmarks
        NotifyBookmarkCreated(subDocument: SubDocument, newState: HistoryItemState<HistoryItemBookmarkStateObject>);
        NotifyBookmarkDeleted(subDocument: SubDocument, newState: HistoryItemState<HistoryItemBookmarkStateObject>);

        // Tables
        NotifyTableCreated(subDocument: SubDocument, table: Table);
        NotifyTableRemoved(subDocument: SubDocument, startPosition: number, endPosition: number, nestedLevel: number);
        NotifyTableStartPositionShifted(subDocument: SubDocument, table: Table, oldPosition: number, newPosition: number);
        NotifyTableCellPropertyChanged(subDocument: SubDocument, property: JSONTableCellProperty, newState: HistoryItemState<HistoryItemTableCellStateObject>);
        NotifyTablePropertyChanged(subDocument: SubDocument, property: JSONTableProperty, newState: HistoryItemState<HistoryItemTableStateObject>);
        NotifyTableRowPropertyChanged(subDocument: SubDocument, property: JSONTableRowProperty, newState: HistoryItemState<HistoryItemTableRowStateObject>);
        NotifyTableCellSplittedHorizontally(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number, rightDirection: boolean);
        NotifyTableCellMergedHorizontally(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number, rightDirection: boolean);
        NotifyTableRowInserted(subDocument: SubDocument, table: Table, rowIndex: number);
        NotifyTableRowRemoved(subDocument: SubDocument, table: Table, rowIndex: number);
        NotifyTableCellRemoved(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number);
        NotifyTableCellInserted(subDocument: SubDocument, table: Table, rowIndex: number, cellIndex: number);
        NotifyTableStyleChanged(subDocument: SubDocument, table: Table, newStyle: TableStyle);
    }
}