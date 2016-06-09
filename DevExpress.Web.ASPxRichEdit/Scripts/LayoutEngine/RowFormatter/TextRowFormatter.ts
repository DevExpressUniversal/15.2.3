module __aspxRichEdit {
    // deleted next functionality: http://screencast.com/t/Z5QhxGdY. About overflow row when tab.x > paragraph.endX

    export interface ITextRowFormatter {
        init(isFirstRowInParagraph: boolean, paragraphStartXPosition: number, paragraphEndXPosition: number);
        formatRow();
        applyRowHorizontalAlign();
        applyLineSpacing();
        setIterator(iterator: ITextBoxIterator);
        pieceTableNumberingListCountersManager: PieceTableNumberingListCountersManager;
        row: LayoutRow;
        sectionIndex: number;
        paragraphIndex: number;
    }

    export enum TextRowFormatterState {
        Base = 0,
        WithTextOnly = 1,
        EndedWithText = 2,
        EndedWithPageBreak = 3,
        EndedWithParagraphMark = 4,
    }
    
    export class TextRowFormatter implements ITextRowFormatter {
        private stateMap: { [stateType: number]: RowBaseFormatterState };
        private static addBoxFunctionMap: { [boxType: number]: (box: LayoutBox) => void };

        private unitConverter: IUnitConverter;
        private iterator: ITextBoxIterator;
        private tabPositions: TabPosition[];
        private defaultTabStop: number;
        public pieceTableNumberingListCountersManager: PieceTableNumberingListCountersManager;

        public row: LayoutRow;
        public sectionIndex: number;
        public paragraphIndex: number;
        
        private currentState: RowBaseFormatterState;
        private availableRowWidth: number;
        private lastTabBoxIndex: number;
        private lastTabPosition: TabPosition;
        private wordStartBoxIndex: number;
        private wordStartPosition: number;
        private paragraphEndXPosition: number;
        private startParagraphProperties: ParagraphProperties;

        private rowFormatting = true;
        private currBox: LayoutBox;

        constructor(iterator: ITextBoxIterator, unitConverter: IUnitConverter) {
            this.iterator = iterator;
            this.unitConverter = unitConverter;
            this.pieceTableNumberingListCountersManager = new PieceTableNumberingListCountersManager(iterator.subDocument);
            
            this.stateMap = {};
            this.stateMap[TextRowFormatterState.Base] = new RowBaseFormatterState(this);
            this.stateMap[TextRowFormatterState.WithTextOnly] = new RowWithTextOnlyFormatterState(this);
            this.stateMap[TextRowFormatterState.EndedWithText] = new RowEndedWithTextFormatterState(this);
            this.stateMap[TextRowFormatterState.EndedWithPageBreak] = new RowEndedWithPageBreakState(this);
            this.stateMap[TextRowFormatterState.EndedWithParagraphMark] = new RowEndedWithParagraphMarkFormatterState(this);

            if (TextRowFormatter.addBoxFunctionMap)
                return;
            TextRowFormatter.addBoxFunctionMap = {};
            TextRowFormatter.addBoxFunctionMap[LayoutBoxType.Space] = function (box: LayoutBox) { this.currentState.addSpaceBox(box) };
            TextRowFormatter.addBoxFunctionMap[LayoutBoxType.Dash] = function (box: LayoutBox) { this.currentState.addDashBox(box) };
            TextRowFormatter.addBoxFunctionMap[LayoutBoxType.Text] = function (box: LayoutBox) { this.currentState.addTextBox(box) };
            TextRowFormatter.addBoxFunctionMap[LayoutBoxType.Picture] = function (box: LayoutBox) { this.currentState.addPictureBox(box) };
            TextRowFormatter.addBoxFunctionMap[LayoutBoxType.ParagraphMark] = function (box: LayoutBox) { this.currentState.addParagraphBox(box) };
            TextRowFormatter.addBoxFunctionMap[LayoutBoxType.LineBreak] = function (box: LayoutBox) { this.currentState.addLineBreakBox(box) };
            TextRowFormatter.addBoxFunctionMap[LayoutBoxType.SectionMark] = function (box: LayoutBox) { this.currentState.addSectionBox(box) };
            TextRowFormatter.addBoxFunctionMap[LayoutBoxType.PageBreak] = function (box: LayoutBox) { this.currentState.addPageBreakBox(box) };
            TextRowFormatter.addBoxFunctionMap[LayoutBoxType.ColumnBreak] = function (box: LayoutBox) { this.currentState.addColumnBreakBox(box) };
            TextRowFormatter.addBoxFunctionMap[LayoutBoxType.TabSpace] = function (box: LayoutBox) { this.currentState.addTabulationBox(box) };
            TextRowFormatter.addBoxFunctionMap[LayoutBoxType.FieldCodeStart] = function (box: LayoutBox) { this.currentState.addTextBox(box) };
            TextRowFormatter.addBoxFunctionMap[LayoutBoxType.FieldCodeEnd] = function (box: LayoutBox) { this.currentState.addTextBox(box) };
            TextRowFormatter.addBoxFunctionMap[LayoutBoxType.LayoutDependent] = function (box: LayoutBox) { this.currentState.addTextBox(box) };
        }

        // prevRowParagraphIndex - in document. not in column
        public init(isFirstRowInParagraph: boolean, paragraphStartXPosition: number, paragraphEndXPosition: number) {
            this.setState(TextRowFormatterState.WithTextOnly);
            this.rowFormatting = true;
            this.row = new LayoutRow();

            // get first box to get actual paragraphIndex
            this.currBox = this.iterator.getNextBox();
            this.paragraphIndex = this.iterator.getParagraphIndex();
            let paragraph: Paragraph = this.iterator.subDocument.paragraphs[this.paragraphIndex];
            if (this.currBox && (this.currBox.getType() == LayoutBoxType.ParagraphMark ||
                this.currBox.getType() == LayoutBoxType.SectionMark) && this.paragraphIndex > 0) { // if first box in row - paragraph end, then we get wrong paragraph
                this.paragraphIndex--;
                paragraph = this.iterator.subDocument.paragraphs[this.paragraphIndex];
            }

            this.sectionIndex = this.iterator.getSectionIndex();
            let section: Section = this.iterator.subDocument.documentModel.sections[this.sectionIndex];
            if (this.currBox && this.currBox.getType() == LayoutBoxType.SectionMark)
                this.sectionIndex--;

            // init tabs info
            this.startParagraphProperties = paragraph.getParagraphMergedProperies();
            const tabsInfo: TabsInfo = paragraph.getTabs();
            this.defaultTabStop = this.unitConverter.toPixels(tabsInfo.defaultTabStop);
            this.tabPositions = tabsInfo.positions;
            for (let tabPosition of this.tabPositions)
                tabPosition.offset = this.unitConverter.toPixels(tabPosition.offset);
            this.resetPreviousTabInfo();

            // init row
            
            this.row.isContentValid = true;
            this.row.x = paragraphStartXPosition;
            this.row.flags = LayoutRowStateFlags.NormallyEnd;

            const parProps: ParagraphProperties = paragraph.getParagraphMergedProperies();
            this.row.width = isFirstRowInParagraph ?
                this.unitConverter.toPixels(parProps.getLeftIndentForFirstRow()) :
                this.unitConverter.toPixels(parProps.getLeftIndentForOtherRow());

            this.paragraphEndXPosition = paragraphEndXPosition - this.unitConverter.toPixels(parProps.rightIndent);
            this.availableRowWidth = this.paragraphEndXPosition - this.row.getRightBoundPosition();
            
            this.wordStartPosition = this.iterator.getPosition();
            this.wordStartBoxIndex = 0;

            if (isFirstRowInParagraph && paragraph.isInList()) {
                this.addNumberingListBox(new LayoutNumberingListBox(
                    paragraph.getNumerationCharacterProperties(),
                    paragraph.getNumberingListTextCore(this.pieceTableNumberingListCountersManager.calculateCounters(this.paragraphIndex)),
                    paragraph.getNumberingListSeparatorChar()),
                    parProps);
            }
        }
        
        public formatRow() {
            const rowStartPosition: number = this.currBox.rowOffset;
            while (true) {
                if (!this.currBox) {
                    if (!this.iterator.isLastBoxGiven()) { // we format faster than chunks coming
                        this.iterator.setPosition(rowStartPosition, false);
                        this.row.flags |= LayoutRowStateFlags.NotEnoughChunks;
                        return;
                    }

                    this.row.flags |= this.iterator.getEndDocumentFlag();
                    this.finishRow();
                    return;
                }
                TextRowFormatter.addBoxFunctionMap[this.currBox.getType()].call(this, this.currBox);
                if (!this.rowFormatting)
                    break;

                this.currBox = this.iterator.getNextBox();
            }
        }

        public applyRowHorizontalAlign() {
            BoxAligner.align(this.row, this.startParagraphProperties.alignment, this.paragraphEndXPosition);
        }

        public applyLineSpacing() {
            let maxAscent: number = 0;
            let maxDescent: number = 0;
            let maxPictureBoxHeight: number = 0;
            let maxBoxHeight: number = 0;
            for (let box of this.row.boxes) {
                switch (box.getType()) {
                    case LayoutBoxType.Text:
                    case LayoutBoxType.FieldCodeStart: // { as text
                    case LayoutBoxType.FieldCodeEnd: // } as text
                    case LayoutBoxType.Dash:
                        maxBoxHeight = Math.max(maxBoxHeight, box.height);
                        maxAscent = Math.max(maxAscent, box.getAscent());
                        maxDescent = Math.max(maxDescent, box.getDescent());
                        break;
                    case LayoutBoxType.Picture:
                        maxBoxHeight = Math.max(maxBoxHeight, box.height);
                        maxPictureBoxHeight = Math.max(maxPictureBoxHeight, box.height);
                }
            }
            
            if (maxBoxHeight == 0) {
                const lastBox: LayoutBox = this.row.boxes[this.row.boxes.length - 1];
                maxBoxHeight = lastBox.height;
                maxAscent = lastBox.getAscent();
                maxDescent = lastBox.getDescent();
            }

            const lineSpacing: number = this.startParagraphProperties.lineSpacing;
            const lineSpacingType: ParagraphLineSpacingType = this.startParagraphProperties.lineSpacingType;
            const lineSpacingCalculator: LineSpacingCalculator = LineSpacingCalculator.create(lineSpacing, lineSpacingType, this.unitConverter);

            this.row.height = lineSpacingCalculator.calculate(maxBoxHeight, maxAscent, maxDescent, maxPictureBoxHeight);
            switch (lineSpacingType) {
                case ParagraphLineSpacingType.AtLeast:
                case ParagraphLineSpacingType.Exactly:
                    this.row.baseLine = this.row.height - maxDescent;
                    break;
                default:
                    this.row.baseLine = Math.max(maxAscent, maxPictureBoxHeight);
                    break;
            }
            
            this.row.width = this.paragraphEndXPosition - this.row.x;
        }

        public setState(state: TextRowFormatterState) {
            this.currentState = this.stateMap[state];
        }

        //paragraphEnd: boolean, pageEnd: boolean, sectionEnd: boolean, columnEnd: boolean
        public finishRow() {
            this.shiftBoxesAfterLastTab();
            this.rowFormatting = false;
        }
        
        public addBox(box: LayoutBox) {
            box.x = this.row.width;
            this.row.boxes.push(box);
            this.row.width += box.width;
            this.availableRowWidth -= box.width;
        }

        public startNewWord() {
            this.wordStartBoxIndex = this.row.boxes.length;
            this.wordStartPosition = this.currBox.rowOffset;
        }

        public isPossibleAddFullBox(box: LayoutBox): boolean {
            return box.width <= this.availableRowWidth;
        }
        
        public resetToBoxStart() {
            this.iterator.setPosition(this.currBox.rowOffset, false);
        }

        public addBoxByChars(box: LayoutBox): boolean {
            const { isCurrBoxWidthOk, nextBox } = box.splitByWidth(this.iterator.getMeasurer(), this.availableRowWidth, this.row.isEmpty());

            if (isCurrBoxWidthOk) {
                if (nextBox)
                    this.iterator.replaceCurrentBoxByTwoBoxes(nextBox);
                this.addBox(box);
                return true;
            }

            this.iterator.setPosition(this.iterator.getPosition() - box.getLength(), false); // if isCurrBoxWidthOk==false, mean that nextBox == null
            return false;
        }

        public addTabBox(box: LayoutBox): boolean {
            this.shiftBoxesAfterLastTab();
            const tabPosition: TabPosition = this.getNextCustomTabPosition();
            const tabXPositionInRow: number = tabPosition ? tabPosition.offset : this.getNextDefaultTabPosition();
            box.width = !tabPosition || tabPosition.align == TabAlign.Left ? tabXPositionInRow - this.row.width : 0;

            if (!this.row.isEmpty() && !this.isPossibleAddFullBox(box))
                return false;

            this.lastTabPosition = tabPosition;
            this.lastTabBoxIndex = this.row.boxes.length;

            this.addBox(box);
            return true;
        }
        
        public resetToWordStart() {
            this.iterator.setPosition(this.wordStartPosition, false);
            this.row.boxes = this.row.boxes.slice(0, this.wordStartBoxIndex);
            this.availableRowWidth = this.paragraphEndXPosition - this.row.getRightBoundPosition();
        }
        
        private addNumberingListBox(numberingListBox: LayoutNumberingListBox, parProps: ParagraphProperties) {
            LayoutBox.initializeWithMeasurer([numberingListBox], this.iterator.getMeasurer(), this.iterator.subDocument.documentModel.showHiddenSymbols);

            const textBox: LayoutTextBox = numberingListBox.textBox;
            const separatorBox: LayoutBox = numberingListBox.separatorBox;

            textBox.x = this.row.width;
            numberingListBox.x = textBox.x;
            numberingListBox.height = textBox.height;
            numberingListBox.width = textBox.width;

            this.row.numberingListBox = numberingListBox;
            this.row.width += textBox.width;
            this.availableRowWidth -= numberingListBox.width;

            if (separatorBox) {
                separatorBox.x = this.row.width;
                if (separatorBox.getType() == LayoutBoxType.TabSpace)
                    separatorBox.width = (parProps.firstLineIndentType == ParagraphFirstLineIndent.Hanging &&
                        this.unitConverter.toPixels(parProps.firstLineIndent) >= textBox.width ?
                        this.unitConverter.toPixels(parProps.leftIndent) :
                        this.getNextDefaultTabPosition()) - this.row.width;
                this.row.width += separatorBox.width;
                numberingListBox.width += separatorBox.width;
                if (separatorBox.width > 0)
                    numberingListBox.height = Math.max(textBox.height, separatorBox.height);
                this.availableRowWidth -= separatorBox.width;
            }
        }

        private getNextDefaultTabPosition(): number {
            return this.defaultTabStop * (Math.floor(this.row.width / this.defaultTabStop) + 1);
        }
        
        private getNextCustomTabPosition(): TabPosition {
            const rowRightPos: number = this.row.width;
            for (let tabPos of this.tabPositions)
                if (rowRightPos < tabPos.offset)
                    return tabPos;
            return null;
        }

        private shiftBoxesAfterLastTab() {
            if (this.lastTabBoxIndex < 0 || !this.lastTabPosition || this.lastTabPosition.align == TabAlign.Left)
                return;

            const prevTabBox: LayoutBox = this.row.boxes[this.lastTabBoxIndex];
            let prevTabNewWidth: number = this.calculateActualTabWidth(prevTabBox);
            if (prevTabNewWidth > 0) {
                prevTabBox.width = prevTabNewWidth;
                this.row.width += prevTabNewWidth;
                const boxes: LayoutBox[] = this.row.boxes;
                for (let i = this.lastTabBoxIndex + 1, box: LayoutBox; box = boxes[i]; i++)
                    box.x += prevTabNewWidth;
            }
            this.resetPreviousTabInfo();
        }

        private calculateActualTabWidth(prevTabBox: LayoutBox): number {
            const prevTabBoxXPos: number = prevTabBox.x; // here prevTabBox.width == 0
            switch (this.lastTabPosition.align) {
                case TabAlign.Decimal: 
                    const decimalSeparatorChar: string = Utils.getDecimalSeparator();
                    for (let i: number = this.lastTabBoxIndex + 1, box: LayoutBox; box = this.row.boxes[i]; i++) {
                        const charIndex: number = box.getCharIndex(decimalSeparatorChar)
                        if (charIndex >= 0) {
                            const charXOffset: number = box.getCharOffsetXInPixels(this.iterator.getMeasurer(), charIndex);
                            return this.getFinalCustomTabWidth(prevTabBoxXPos, box.x + charXOffset - prevTabBoxXPos);
                        }
                    }
                    return 0;
                case TabAlign.Right:
                    return this.getFinalCustomTabWidth(prevTabBoxXPos, 0);
                case TabAlign.Center:
                    const lastVisibleBox: LayoutBox = this.row.boxes[Math.max(0, BoxAligner.findLastVisibleBoxIndex(this.row.boxes))];
                    const lastTextBoxRightBound: number = lastVisibleBox.getRightBoundPosition();
                    return this.getFinalCustomTabWidth(prevTabBoxXPos, Math.ceil((lastTextBoxRightBound - prevTabBoxXPos) / 2));
            }
        }

        private getFinalCustomTabWidth(prevTabBoxXPos: number, textLengthBetweenTabBoxAndTabMark: number): number {
            // mean tab.width must be > 0 and < this.availableRowWidth
            return Math.max(0, Math.min(this.availableRowWidth, this.lastTabPosition.offset - prevTabBoxXPos - textLengthBetweenTabBoxAndTabMark));
        }

        private resetPreviousTabInfo() {
            this.lastTabPosition = null;
            this.lastTabBoxIndex = -1;
        }

        public setIterator(iterator: ITextBoxIterator) {
            this.iterator = iterator;
        }
    }

    
}