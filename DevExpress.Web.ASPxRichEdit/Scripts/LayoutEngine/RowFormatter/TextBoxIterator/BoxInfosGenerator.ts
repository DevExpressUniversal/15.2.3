module __aspxRichEdit {
    export class BoxInfosGenerator {
        private textBoxIterator: TextBoxIterator;
        public maxNumNewBoxes: number = TextBoxIterator.SIZE_PAGE;
        public maxBoxesInList: number = TextBoxIterator.SIZE_PAGE * 4;
        public isClearList: boolean;

        private modelCharToLayoutBoxConverter: any = {};

        private chunks: Chunk[];
        private chunk: Chunk;
        private run: TextRun;
        private runText: string;
        private offsetCharAtStartRun: number;
        private offsetStartWordAtStartRun: number; // here word mean WORD or PART WORD. "word /t word"      "word /t word"
        //                                   5 words              6 words if last 2 symbols have another formatting
        private currBoxInfoType: LayoutBoxType;
        private isNeedResetPosition: boolean;

        // vars below need copy back to textBoIterator
        private newLayoutBoxes: LayoutBox[]; // collect only for measuring

        private chunkIndex: number;
        private runIndex: number;

        public isDocumentEnd: boolean;

        constructor(textBoxIterator: TextBoxIterator) {
            this.textBoxIterator = textBoxIterator;

            this.modelCharToLayoutBoxConverter[Utils.specialCharacters.LineBreak] = function (charProp: CharacterProperties): LayoutBox { return new LayoutLineBreakBox(charProp); };
            this.modelCharToLayoutBoxConverter[Utils.specialCharacters.Space] = function (charProp: CharacterProperties): LayoutBox { return new LayoutSpaceBox(charProp); };
            this.modelCharToLayoutBoxConverter[Utils.specialCharacters.TabMark] = function (charProp: CharacterProperties): LayoutBox { return new LayoutTabSpaceBox(charProp); };
            this.modelCharToLayoutBoxConverter[Utils.specialCharacters.PageBreak] = function (charProp: CharacterProperties): LayoutBox { return new LayoutPageBreakBox(charProp); };
            this.modelCharToLayoutBoxConverter[Utils.specialCharacters.ColumnBreak] = function (charProp: CharacterProperties): LayoutBox { return new LayoutColumnBreakBox(charProp); };

            this.isNeedResetPosition = true;
            this.isDocumentEnd = false;
        }

        public setPosition() {
            this.isNeedResetPosition = true;
        }

        // set needed vars
        public generate(): boolean {
            if (this.isNeedResetPosition) {
                this.chunks = this.textBoxIterator.subDocument.chunks;
                this.chunkIndex = Utils.normedBinaryIndexOf(this.chunks, (chunk: Chunk) => chunk.startLogPosition.value - this.textBoxIterator.position);
                this.chunk = this.chunks[this.chunkIndex];
                if (this.textBoxIterator.position >= this.chunk.getEndPosition()) {
                    if (!this.textBoxIterator.subDocument.getLastChunk().isLast)
                        this.textBoxIterator.onNextChunkRequested.raise("NotifyNextChunkRequired", this.textBoxIterator.subDocument, this.textBoxIterator.subDocument.chunks.length);
                    return false;
                }
                const runOffsetAtStartChunk: number = this.textBoxIterator.position - this.chunk.startLogPosition.value;
                this.runIndex = Utils.normedBinaryIndexOf(this.chunk.textRuns, (run: TextRun) => run.startOffset - runOffsetAtStartChunk);
                this.run = this.chunk.textRuns[this.runIndex];
                this.runText = this.chunk.getRunText(this.run);
                this.isNeedResetPosition = false;
            }
            
            this.isDocumentEnd = false;
            this.newLayoutBoxes = [];
            this.offsetCharAtStartRun = this.textBoxIterator.position - this.chunk.startLogPosition.value - this.run.startOffset;
            this.offsetStartWordAtStartRun = this.offsetCharAtStartRun;
            this.currBoxInfoType = LayoutBoxType.Text;

            this.createNewBoxes();
            LayoutBox.initializeWithMeasurer(this.newLayoutBoxes, this.textBoxIterator.measurer, this.textBoxIterator.subDocument.documentModel.showHiddenSymbols);

            this.setNewBoxes();

            
            return this.newLayoutBoxes.length > 0;
        }

        private setNewBoxes() {
            if (this.isClearList) {
                this.textBoxIterator.boxes = this.newLayoutBoxes;
                this.textBoxIterator.boxIndex = 0;
                return;
            }

            this.textBoxIterator.boxes = this.textBoxIterator.boxes.concat(this.newLayoutBoxes);
            const excessLen: number = this.textBoxIterator.boxes.length - this.maxBoxesInList;
            if (excessLen > 0) {
                this.textBoxIterator.boxes.splice(0, excessLen);
                this.textBoxIterator.boxIndex = Math.max(0, this.textBoxIterator.boxIndex - excessLen);
            }
        }

        private addNewBoxInfo(layoutBox: LayoutBox) {
            layoutBox.rowOffset = this.getAbsolutePosition() - layoutBox.getLength();
            this.newLayoutBoxes.push(layoutBox);
        }

        private getAbsolutePosition(): number {
            return this.chunk.startLogPosition.value + this.run.startOffset + this.offsetCharAtStartRun;
        }

        // main work
        private createNewBoxes() {
            while (this.newLayoutBoxes.length < this.maxNumNewBoxes) {
                if (this.offsetCharAtStartRun >= this.run.length)
                    if (!this.getNextRun()) {
                        this.isDocumentEnd = this.chunk.isLast;
                        if (!this.isDocumentEnd)
                            this.textBoxIterator.onNextChunkRequested.raise("NotifyNextChunkRequired", this.textBoxIterator.subDocument, this.textBoxIterator.subDocument.chunks.length);
                        return;
                    }
                var currChar: string = this.runText.charAt(this.offsetCharAtStartRun);

                switch (currChar) {
                    case Utils.specialCharacters.Space:
                    case Utils.specialCharacters.TabMark:
                    case Utils.specialCharacters.LineBreak:
                    case Utils.specialCharacters.PageBreak:
                    case Utils.specialCharacters.ColumnBreak:
                        this.currWordToBox();
                        this.offsetCharAtStartRun++;
                        this.addNewBoxInfo(this.modelCharToLayoutBoxConverter[currChar](this.run.getCharacterMergedProperies()));
                        this.offsetStartWordAtStartRun = this.offsetCharAtStartRun;
                        break;
                    case Utils.specialCharacters.Dash:
                    case Utils.specialCharacters.EmDash:
                    case Utils.specialCharacters.EnDash:
                        if (this.currBoxInfoType != LayoutBoxType.Dash) {
                            this.currWordToBox();
                            this.currBoxInfoType = LayoutBoxType.Dash;
                            this.offsetStartWordAtStartRun = this.offsetCharAtStartRun;
                        }
                        this.offsetCharAtStartRun++;
                        break;
                    default: {
                        switch (this.run.type) {
                            case TextRunType.ParagraphRun:
                            case TextRunType.SectionRun:
                                this.offsetCharAtStartRun++;
                                this.addNewBoxInfo(this.run.type == TextRunType.ParagraphRun ?
                                    new LayoutParagraphMarkBox(this.run.getCharacterMergedProperies()) :
                                    new LayoutSectionMarkBox(this.run.getCharacterMergedProperies()));
                                this.offsetStartWordAtStartRun = this.offsetCharAtStartRun;
                                break;
                            case TextRunType.InlinePictureRun:
                                var picRun: InlinePictureRun = <InlinePictureRun>this.run;
                                this.offsetCharAtStartRun++;
                                this.addNewBoxInfo(new LayoutPictureBox(picRun.getCharacterMergedProperies(), picRun.id,
                                    UnitConverter.twipsToPixels(picRun.getActualWidth()),
                                    UnitConverter.twipsToPixels(picRun.getActualHeight()),
                                    picRun.isLoaded));
                                this.offsetStartWordAtStartRun = this.offsetCharAtStartRun;
                                break;
                            case TextRunType.FieldCodeStartRun:
                                this.offsetCharAtStartRun++;
                                this.addNewBoxInfo(new LayoutFieldCodeStartBox(this.run.getCharacterMergedProperies()));
                                this.offsetStartWordAtStartRun = this.offsetCharAtStartRun;
                                break;
                            case TextRunType.FieldCodeEndRun:
                                this.offsetCharAtStartRun++;
                                this.addNewBoxInfo(new LayoutFieldCodeEndBox(this.run.getCharacterMergedProperies()));
                                this.offsetStartWordAtStartRun = this.offsetCharAtStartRun;
                                break;
                            case TextRunType.FieldResultEndRun:
                                this.offsetCharAtStartRun++;
                                this.addNewBoxInfo(new LayoutFieldResultEndBox(this.run.getCharacterMergedProperies())); // never used in layout
                                this.offsetStartWordAtStartRun = this.offsetCharAtStartRun;
                                break;
                            case TextRunType.LayoutDependentTextRun:
                                this.offsetCharAtStartRun++;
                                this.addNewBoxInfo(new LayoutDependentTextBox(this.run.getCharacterMergedProperies(), this.runText));
                                this.offsetStartWordAtStartRun = this.offsetCharAtStartRun;
                                break;
                            default: {
                                if (this.currBoxInfoType == LayoutBoxType.Dash) {
                                    this.currWordToBox(); // last dashes to box
                                    this.offsetStartWordAtStartRun = this.offsetCharAtStartRun;
                                }
                                this.offsetCharAtStartRun++;
                            }
                        } //switch (this.run.type)
                    } //default:
                } //switch (currChar)
            } //while (this.newBoxInfos.length < this.maxNumNewBoxes)
        }

        private currWordToBox() {
            if (this.offsetCharAtStartRun > this.offsetStartWordAtStartRun) {
                var word: string = this.runText.substring(this.offsetStartWordAtStartRun, this.offsetCharAtStartRun);
                if (this.currBoxInfoType == LayoutBoxType.Text)
                    this.addNewBoxInfo(new LayoutTextBox(this.run.getCharacterMergedProperies(), word));
                else
                    this.addNewBoxInfo(new LayoutDashBox(this.run.getCharacterMergedProperies(), word));
            }
            this.currBoxInfoType = LayoutBoxType.Text;
        }

        private getNextRun(): boolean {
            this.currWordToBox();
            this.runIndex++;
            if (this.runIndex >= this.chunk.textRuns.length) {
                this.chunkIndex++;
                if (this.chunkIndex >= this.chunks.length)
                    return false;
                this.chunk = this.chunks[this.chunkIndex];
                this.runIndex = 0;
            }
            this.run = this.chunk.textRuns[this.runIndex];
            this.runText = this.chunk.getRunText(this.run);
            this.offsetCharAtStartRun = 0;
            this.offsetStartWordAtStartRun = 0;

            return true;
        }
    }
}