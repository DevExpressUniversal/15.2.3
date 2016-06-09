module __aspxRichEdit {
    export interface ITextBoxIterator {
        setPosition(position: number, forceResetBoxInfos: boolean); // [box][box][box][lastGettedBox] >< <<<here position
        getNextBox(): LayoutBox;
        nextVisibleBoxStartPositionEqualWith(nextBoxStartPos: number): boolean; // need for try reuse
        getPosition(): number;
        getParagraphIndex(): number;
        getSectionIndex(): number;
        getTablePositions(): TablePosition[];
        checkTableLevelsInfo(): any;
        setGenerateBoxCount(count: number);
        getGenerateBoxCount(): number;
        isLastBoxGiven(): boolean;
        getMeasurer(): IBoxMeasurer;
        replaceCurrentBoxByTwoBoxes(nextBox: LayoutBox);
        setTableCellParagraphBoxes(boxes: LayoutBox[], parIndex: number);
        getEndDocumentFlag(): LayoutRowStateFlags;

        subDocument: SubDocument;
        onNextChunkRequested: EventDispatcher<ITextBoxIteratorRequestsListener>;
    }
}