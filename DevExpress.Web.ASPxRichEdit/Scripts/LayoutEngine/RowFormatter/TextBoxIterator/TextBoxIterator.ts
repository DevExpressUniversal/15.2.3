module __aspxRichEdit {
    export class TextBoxIterator implements ITextBoxIterator {
        public static SIZE_FIND_NEXT_VISIBLE_BOX = 5;
        public static SIZE_ROW = 30;
        public static SIZE_PAGE = 800;

        public measurer: IBoxMeasurer;
        public subDocument: SubDocument;
        protected boxInfosGenerator: BoxInfosGenerator;
        protected tablePositionsManipulator: BoxIteratorTablePositionsManipulator;
        
        protected fieldVisabilityInfoList: FieldVisabilityInfo[] = [];
        public tablePositions: TablePosition[] = [];
        public currTableIndexInLevels: number[] = []; // prevTableIndexInLevel[1] mean that first level prev index in subDocument.tablesByLevels[1] was prevTableIndexInLevel[1]
        
        public boxes: LayoutBox[] = [];
        public boxIndex: number = 0;
        public position: number = 0;
        public sectionIndex: number = 0;
        public paragraphIndex: number = 0;

        public onNextChunkRequested: EventDispatcher<ITextBoxIteratorRequestsListener> = new EventDispatcher<ITextBoxIteratorRequestsListener>();

        constructor(measurer: IBoxMeasurer, subDocument: SubDocument) {
            this.measurer = measurer;
            this.subDocument = subDocument;
            this.boxInfosGenerator = new BoxInfosGenerator(this);
            this.tablePositionsManipulator = new BoxIteratorTablePositionsManipulator(this);
        }

        // call if getNextBox return null
        public isLastBoxGiven(): boolean {
            return this.boxInfosGenerator.isDocumentEnd;
        }

        public getTablePositions(): TablePosition[] {
            return this.tablePositions;
        }

        public checkTableLevelsInfo() {
            //this.tablePositionsManipulator.checkTableLevelsInfo();
        }

        public getSectionIndex(): number {
            return this.sectionIndex;
        }

        public getMeasurer(): IBoxMeasurer {
            return this.measurer;
        }

        public getPosition() {
            return this.position;
        }

        public getParagraphIndex(): number {
            return this.paragraphIndex;
        }

        public setGenerateBoxCount(count: number) {
            this.boxInfosGenerator.maxNumNewBoxes = count;
        }

        public getGenerateBoxCount(): number {
            return this.boxInfosGenerator.maxNumNewBoxes;
        }

        public setPosition(position: number, forceResetBoxInfos: boolean) {
            if (position == this.position) {
                if (forceResetBoxInfos)
                    this.resetInternal();
                return;
            }

            this.position = position;
            this.paragraphIndex = Utils.normedBinaryIndexOf(this.subDocument.paragraphs, (p: Paragraph) => p.startLogPosition.value - position);
            this.sectionIndex = this.subDocument.isMain() ? Math.max(0,
                Utils.normedBinaryIndexOf(this.subDocument.documentModel.sections, (s: Section) => s.startLogPosition.value - position)) : -1;
            this.fieldVisabilityInfoList = FieldVisabilityInfo.getRelativeVisabilityInfo(this.position, this.subDocument.fields);
            //this.tablePositionsManipulator.setTableIndexes();

            const lastBox: LayoutBox = this.boxes[this.boxes.length - 1];
            const interval: FixedInterval = lastBox ? FixedInterval.fromPositions(this.boxes[0].rowOffset, lastBox.getEndPosition()) : new FixedInterval(0, 0);

            if (!forceResetBoxInfos && interval.containsPositionWithoutIntervalEnd(this.position)) {
                this.boxIndex = Utils.normedBinaryIndexOf(this.boxes, (box: LayoutBox) => box.rowOffset - this.position);
                const box: LayoutBox = this.boxes[this.boxIndex];
                const offsetAtStartBox: number = this.position - box.rowOffset;
                if (offsetAtStartBox > 0)
                    this.splitBoxByPosition(box, this.boxIndex, offsetAtStartBox);
            }
            else
                this.resetInternal();
        }

        public nextVisibleBoxStartPositionEqualWith(nextBoxStartPos: number): boolean {
            const oldBoxIndex: number = this.boxIndex;
            const oldPosition: number = this.position;
            const oldBoxInfos: LayoutBox[] = this.boxes;

            const oldFieldLevelInfo: FieldVisabilityInfo[] = [];
            for (let visInfo of this.fieldVisabilityInfoList)
                oldFieldLevelInfo.push(visInfo.clone());

            const oldMaxNumNewBoxes: number = this.boxInfosGenerator.maxNumNewBoxes;
            this.boxInfosGenerator.maxNumNewBoxes = TextBoxIterator.SIZE_FIND_NEXT_VISIBLE_BOX;
            const box: LayoutBox = this.getNextBoxInternal(nextBoxStartPos);
            this.boxInfosGenerator.maxNumNewBoxes = oldMaxNumNewBoxes;

            this.boxIndex = oldBoxIndex;
            this.position = oldPosition;
            this.boxes = oldBoxInfos;
            this.fieldVisabilityInfoList = oldFieldLevelInfo;

            this.boxInfosGenerator.setPosition();

            return box ? (box.rowOffset == nextBoxStartPos ? true : false) : false;
        }

        public getNextBox(): LayoutBox {
            const box: LayoutBox = this.getNextBoxInternal(-1);
            if (!box)
                return null;

            if (this.position >= this.subDocument.paragraphs[this.paragraphIndex].getEndPosition())
                this.paragraphIndex++;
            if (this.subDocument.isMain() && this.sectionIndex >= 0 && this.position >= this.subDocument.documentModel.sections[this.sectionIndex].getEndPosition())
                this.sectionIndex++;

            return box;
        }

        private getNextBoxInternal(maxPosition: number): LayoutBox {
            // if you change this method, then change please getNextBoxStartPosition too
            let box: LayoutBox;
            let isNeedSkipBox: boolean;
            do {
                box = this.boxes[this.boxIndex];
                if (!box) {
                    if (this.boxInfosGenerator.generate())
                        box = this.boxes[this.boxIndex];
                    else
                        return null;
                }

                this.boxIndex++;
                this.position += box.getLength();

                const isIfItFieldBoxIsVisible: boolean = this.determineVisabilityFieldElement(box);
                //this.tablePositionsManipulator.advanceTableIndexes(box);

                isNeedSkipBox = !this.subDocument.documentModel.showHiddenSymbols && box.characterProperties.hidden || !isIfItFieldBoxIsVisible;
                if (maxPosition >= 0) {
                    if (box.rowOffset == maxPosition)
                        return isNeedSkipBox ? null : box;
                    if (box.rowOffset > maxPosition)
                        return null;
                }
            } while (isNeedSkipBox);

            return box;
        }

        // true - visible field
        private determineVisabilityFieldElement(boxInfo: LayoutBox): boolean {
            switch (boxInfo.getType()) {
                case LayoutBoxType.FieldCodeStart:
                    const fields: Field[] = this.subDocument.fields;
                    const nextFieldIndex: number = Field.normedBinaryIndexOf(fields, this.position - boxInfo.getLength() + 1);
                    const nextField: Field = fields[nextFieldIndex];

                    const topLevelFieldInfo: FieldVisabilityInfo = this.fieldVisabilityInfoList[this.fieldVisabilityInfoList.length - 1];
                    const lowLevelFieldInfo: FieldVisabilityInfo = new FieldVisabilityInfo(nextField.showCode, !nextField.showCode, nextField);
                    this.fieldVisabilityInfoList.push(lowLevelFieldInfo);
                    if (topLevelFieldInfo)
                        FieldVisabilityInfo.applyTopLevelFieldInfoVisabilityToThisFieldInfo(topLevelFieldInfo, lowLevelFieldInfo);
                    boxInfo.fieldLevel = this.fieldVisabilityInfoList.length;
                    return lowLevelFieldInfo.showCode;
                case LayoutBoxType.FieldCodeEnd:
                    boxInfo.fieldLevel = this.fieldVisabilityInfoList.length;
                    return this.fieldVisabilityInfoList[this.fieldVisabilityInfoList.length - 1].showCode;
                case LayoutBoxType.FieldResultEnd:
                    this.fieldVisabilityInfoList.pop();
                    return false;
                case LayoutBoxType.LayoutDependent:
                    const fieldInfo = this.fieldVisabilityInfoList[this.fieldVisabilityInfoList.length - 1];
                    if(fieldInfo) {
                        let fieldCodeText = ASPx.Str.Trim(this.subDocument.getText(fieldInfo.field.getCodeInterval()).split("\\")[0]); //TODO
                        let layoutDependentBox = <LayoutDependentTextBox>boxInfo;
                        switch(fieldCodeText) {
                            case "PAGE":
                                layoutDependentBox.getText = (layoutFormatter: LayoutFormatterBase) => {
                                    return (layoutFormatter.layoutPosition.pageIndex + 1).toString();
                                };
                                break;
                            case "NUMPAGES":
                                layoutDependentBox.getText = (layoutFormatter: LayoutFormatterBase) => {
                                    if(!layoutFormatter.iterator.subDocument.isMain()) {
                                        let pageIndex = layoutFormatter.layoutPosition.pageIndex;
                                        if(!layoutFormatter.layoutDependentOtherPageAreasCache.hasOwnProperty(pageIndex.toString()))
                                            layoutFormatter.layoutDependentOtherPageAreasCache[pageIndex] = [];
                                        let subDocumentId = layoutFormatter.iterator.subDocument.id;
                                        if(layoutFormatter.layoutDependentOtherPageAreasCache[pageIndex].indexOf(subDocumentId) < 0)
                                            layoutFormatter.layoutDependentOtherPageAreasCache[pageIndex].push(subDocumentId);
                                    }
                                    return layoutFormatter.pagesCount.toString();
                                };
                                break;
                            default:
                                break;
                        }
                    }
                    return true;
                default:
                    if (this.fieldVisabilityInfoList.length > 0) {
                        var thisBoxInterval: FixedInterval = FixedInterval.fromPositions(this.position - boxInfo.getLength(), this.position);
                        var currLowLevelFieldInfo: FieldVisabilityInfo = this.fieldVisabilityInfoList[this.fieldVisabilityInfoList.length - 1];

                        if (currLowLevelFieldInfo.field.isHyperlinkField() && thisBoxInterval.start >= currLowLevelFieldInfo.field.getResultStartPosition()) {
                            if(this.subDocument.documentModel.activeSubDocument === this.subDocument) {
                                var hyperlinkInfo: HyperlinkInfo = currLowLevelFieldInfo.field.getHyperlinkInfo();
                                if(hyperlinkInfo.tip != "")
                                    boxInfo.hyperlinkTip = hyperlinkInfo.tip;
                                else
                                    boxInfo.hyperlinkTip = hyperlinkInfo.uri + (hyperlinkInfo.anchor == "" ? "" : "#" + hyperlinkInfo.anchor);
                            }
                        }
                        else {
                            for (var i: number = 0, info: FieldVisabilityInfo; info = this.fieldVisabilityInfoList[i]; i++) {
                                if (info.showCode && thisBoxInterval.start < info.field.getSeparatorPosition()) {
                                    boxInfo.fieldLevel = this.fieldVisabilityInfoList.length;
                                    break;
                                }
                            }
                        }

                        var isPlacedInCodeSection: boolean = !!FixedInterval.getIntersection(thisBoxInterval, currLowLevelFieldInfo.field.getCodeInterval());
                        return isPlacedInCodeSection && currLowLevelFieldInfo.showCode || !isPlacedInCodeSection && currLowLevelFieldInfo.showResult;
                    }
            }
            return true;
        }
        
        private splitBoxByPosition(box: LayoutBox, boxIndex: number, offsetAtStartBox: number) {
            const nextBox: LayoutBox = box.splitBoxByPosition(this.measurer, offsetAtStartBox);
            if (!nextBox)
                return;
            this.boxIndex++;
            this.boxes.splice(this.boxIndex, 0, nextBox);
        }

        private resetInternal() {
            this.boxes = [];
            this.boxIndex = 0;
            this.boxInfosGenerator.setPosition();
        }

        public replaceCurrentBoxByTwoBoxes(nextBox: LayoutBox) {
            nextBox.rowOffset = this.position - nextBox.getLength();
            this.boxes.splice(this.boxIndex, 0, nextBox);
            this.setPosition(nextBox.rowOffset, false);
        }

        setTableCellParagraphBoxes(boxes: LayoutBox[], parIndex: number) {
        }

        public getEndDocumentFlag(): LayoutRowStateFlags {
            return LayoutRowStateFlags.DocumentEnd;
        }
    }

    export class TableBoxIterator extends TextBoxIterator {
        collectorInfo: TableTextBoxCollectorTableInfo[];

        constructor(measurer: IBoxMeasurer, subDocument: SubDocument, collectorInfo: TableTextBoxCollectorTableInfo[], sectionIndex: number) {
            super(measurer, subDocument);
            this.sectionIndex = sectionIndex;
            this.collectorInfo = collectorInfo;

            // just in case
            this.boxInfosGenerator = null;
            this.tablePositionsManipulator = null;
            this.fieldVisabilityInfoList = null;
        }

        setTableCellParagraphBoxes(boxes: LayoutBox[], parIndex: number) {
            this.boxes = boxes;
            this.position = boxes[0].rowOffset;
            this.paragraphIndex = parIndex;
            this.boxIndex = 0;
        }

        getNextBox(): LayoutBox {
            if (this.boxIndex == this.boxes.length)
                return null;
            const box: LayoutBox = this.boxes[this.boxIndex++];
            this.position = box.getEndPosition();
            return box;
        }

        setPosition(position: number, forceResetBoxInfos: boolean) {
            if (this.position == position)
                return;
            this.boxIndex = Utils.normedBinaryIndexOf(this.boxes, (b: LayoutBox) => b.rowOffset - position);
            if (this.boxIndex < 0) {
                this.boxIndex = 0;
                this.position = this.boxes[0].rowOffset;
                return;
            }
            const box: LayoutBox = this.boxes[this.boxIndex];
            if (box.rowOffset == position) {
                this.position = position;
                return;
            }
            const boxEndPos: number = box.getEndPosition();
            if (position >= boxEndPos) {
                const box: LayoutBox = this.boxes[++this.boxIndex];
                this.position = box ? box.rowOffset : boxEndPos;
                return;
            }
            this.boxes.splice(this.boxIndex + 1, 0, box.splitBoxByPosition(this.measurer, position - box.rowOffset));
        }
        
        nextVisibleBoxStartPositionEqualWith(nextBoxStartPos: number): boolean {
            return false;
        }
        isLastBoxGiven(): boolean {
            return this.boxIndex >= this.boxes.length;
        }
        replaceCurrentBoxByTwoBoxes(nextBox: LayoutBox) {
            nextBox.rowOffset = this.position - nextBox.getLength();
            this.boxes.splice(this.boxIndex, 0, nextBox);
            this.position = nextBox.rowOffset;
        }
        public getEndDocumentFlag(): LayoutRowStateFlags {
            return LayoutRowStateFlags.CellTableEnd;
        }
        
    }
}