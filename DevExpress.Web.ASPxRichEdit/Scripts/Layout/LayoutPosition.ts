module __aspxRichEdit {
    export class LayoutAndModelPositions {
        layoutPosition: LayoutPosition;
        modelPosition: number;

        constructor(layoutPosition: LayoutPosition, modelPosition: number) {
            this.layoutPosition = layoutPosition;
            this.modelPosition = modelPosition;
        }
    }

    export class LayoutRowPosition {
        public row: LayoutRow = null;

        public box: LayoutBox = null;
        public boxIndex: number = -1;

        public charOffset: number = -1;
    }

    export class LayoutPositionBase extends LayoutRowPosition implements IEquatable<LayoutPositionBase> {
        public detailsLevel: DocumentLayoutDetailsLevel = DocumentLayoutDetailsLevel.None;

        public rowIndex: number = -1;

        public column: LayoutColumn = null;
        public columnIndex: number = -1;

        public pageArea: LayoutPageArea = null;
        public pageAreaIndex: number = -1; // index in position.page.areasById[position.subDocument.id];

        public page: LayoutPage = null;
        public pageIndex: number = -1; // == this.page.index !if page initialized!

        public getRelatedSubDocumentPagePosition(): number {
            return this.pageArea.subDocument.isMain() ? this.page.getStartOffsetContentOfMainSubDocument() : 0; 
        }

        public equals(obj: LayoutPositionBase): boolean {
            if (this.detailsLevel != obj.detailsLevel)
                return false;

            switch (this.detailsLevel) {
                case DocumentLayoutDetailsLevel.Character: if (this.charOffset != obj.charOffset) return false;
                case DocumentLayoutDetailsLevel.Box: if (this.boxIndex!= obj.boxIndex) return false;
                case DocumentLayoutDetailsLevel.Row: if (this.rowIndex != obj.rowIndex) return false;
                case DocumentLayoutDetailsLevel.Column: if (this.columnIndex != obj.columnIndex) return false;
                case DocumentLayoutDetailsLevel.PageArea: if (this.pageAreaIndex != obj.pageAreaIndex || this.pageArea.subDocument.id != obj.pageArea.subDocument.id) return false;
                case DocumentLayoutDetailsLevel.Page: if (this.pageIndex != obj.pageIndex) return false;
            }
            return true;
        }

        public isPositionMainSubDocument(): boolean {
            return this.pageArea.subDocument.isMain();
        }

        public getPositionRelativePage(measurer: IBoxMeasurer): LayoutPoint {
            return new LayoutPoint(this.page.index, this.getLayoutX(measurer), this.getLayoutY());
        }

        private getLayoutX(measurer: IBoxMeasurer): number {
            var x = 0;
            x += this.box.getCharOffsetXInPixels(measurer, this.charOffset);
            x += this.box.x;
            x += this.row.x;
            x += this.column.x;
            x += this.pageArea.x;
            // page.x always == 0
            return x;
        }

        private getLayoutY(): number {
            var y = 0;
            y += this.row.baseLine - this.box.getAscent() - this.row.getSpacingBefore();
            y += this.row.y;
            y += this.column.y;
            y += this.pageArea.y;
            // page.y ???
            return y;
        }

        public getPageAreaBySubDocument(subDocument: SubDocument) {
            return subDocument.isMain() ? this.page.mainSubDocumentPageAreas[this.pageAreaIndex] : this.page.otherPageAreas[subDocument.id];
        }

        public advanceToPrevRow(layout: DocumentLayout): boolean {
            this.rowIndex--;
            if (this.rowIndex < 0) {
                if (!this.pageArea.subDocument.isMain()) {
                    this.rowIndex = 0;
                    return false;
                }
                this.columnIndex--;
                if (this.columnIndex < 0) {
                    this.pageAreaIndex--;
                    if (this.pageAreaIndex < 0) {
                        this.pageIndex--;
                        if (this.pageIndex < 0) {
                            this.pageIndex = 0;
                            this.pageAreaIndex = 0;
                            this.columnIndex = 0;
                            this.rowIndex = 0;
                            return false;
                        }
                        this.page = layout.pages[this.pageIndex];
                        this.pageAreaIndex = this.page.mainSubDocumentPageAreas.length - 1;
                    }
                    this.pageArea = this.page.mainSubDocumentPageAreas[this.pageAreaIndex];
                    this.columnIndex = this.pageArea.columns.length - 1;
                }
                this.column = this.pageArea.columns[this.columnIndex];
                this.rowIndex = this.column.rows.length - 1;
            }
            this.row = this.column.rows[this.rowIndex];
            return true;
        }

        public advanceToNextRow(layout: DocumentLayout): boolean {
            this.rowIndex++;
            if (this.rowIndex >= this.column.rows.length) {
                if (!this.pageArea.subDocument.isMain()) {
                    this.rowIndex--;
                    return false;
                }
                this.rowIndex = 0;
                this.columnIndex++;
                if (this.columnIndex >= this.pageArea.columns.length) {
                    this.columnIndex = 0;
                    this.pageAreaIndex++;
                    if (this.pageAreaIndex >= this.page.mainSubDocumentPageAreas.length) {
                        this.pageAreaIndex = 0;
                        this.pageIndex++;
                        if (this.pageIndex >= layout.validPageCount) {
                            this.pageIndex--;
                            this.pageAreaIndex = this.page.mainSubDocumentPageAreas.length - 1;
                            this.columnIndex = this.pageArea.columns.length - 1;
                            this.rowIndex = this.column.rows.length - 1;
                            return false;
                        }
                        this.page = layout.pages[this.pageIndex];
                    }
                    this.pageArea = this.page.mainSubDocumentPageAreas[this.pageAreaIndex];
                }
                this.column = this.pageArea.columns[this.columnIndex];
            }
            this.row = this.column.rows[this.rowIndex];
            return true;
        }
    }

    export class LayoutPosition extends LayoutPositionBase implements ICloneable<LayoutPosition> {
        // Remember that the position is linked to a sub document as this.pageArea.subDocument

        constructor(detailsLevel: DocumentLayoutDetailsLevel) {
            super();
            this.detailsLevel = detailsLevel;
        }

        public getLogPosition(detailsLevel: DocumentLayoutDetailsLevel = null): number {
            if (!detailsLevel)
                detailsLevel = this.detailsLevel;
            var position: number = 0;
            switch (detailsLevel) {
                case DocumentLayoutDetailsLevel.Max:
                case DocumentLayoutDetailsLevel.Character: position += this.charOffset;
                case DocumentLayoutDetailsLevel.Box: position += this.box.rowOffset;
                case DocumentLayoutDetailsLevel.Row: position += this.row.columnOffset;
                case DocumentLayoutDetailsLevel.Column: position += this.column.pageAreaOffset;
                case DocumentLayoutDetailsLevel.PageArea: position += this.pageArea.pageOffset;
            }
            if (detailsLevel >= DocumentLayoutDetailsLevel.Page && this.isPositionMainSubDocument())
                position += this.page.getStartOffsetContentOfMainSubDocument();

            return position;
        }

        // only for detailsLevel == DocumentLayoutDetailsLevel.Character
        public isPositionBoxEnd(): boolean {
            return this.charOffset == this.box.getLength();
        }

        public isLastBoxInRow(): boolean {
            return this.boxIndex == this.row.boxes.length - 1;
        }

        // only for detailsLevel == DocumentLayoutDetailsLevel.Character
        public isPositionAfterLastBoxInRow(): boolean {
            return this.isLastBoxInRow() && this.isPositionBoxEnd();
        }

        // only for detailsLevel == DocumentLayoutDetailsLevel.Character
        public isPositionBeforeFirstBoxInRow(): boolean {
            return this.boxIndex == 0 && this.charOffset == 0;
        }

        public switchToEndPrevBoxInRow() {
            if (this.charOffset == 0 && this.boxIndex > 0) {
                this.boxIndex--;
                this.box = this.row.boxes[this.boxIndex];
                this.charOffset = this.box.getLength();
            }
        }

        public switchToStartNextBoxInRow() {
            if (this.charOffset == this.box.getLength() && this.boxIndex + 1 < this.row.boxes.length) {
                this.boxIndex++;
                this.box = this.row.boxes[this.boxIndex];
                this.charOffset = 0;
            }
        }

        copyFrom(source: LayoutPosition) {
            this.detailsLevel = source.detailsLevel;
            this.pageIndex = source.pageIndex;
            this.page = source.page;
            this.pageArea = source.pageArea;
            this.pageAreaIndex = source.pageAreaIndex;
            this.column = source.column;
            this.columnIndex = source.columnIndex;
            this.row = source.row;
            this.rowIndex = source.rowIndex;
            this.box = source.box;
            this.boxIndex = source.boxIndex;
            this.charOffset = source.charOffset;
        }

        public clone() : LayoutPosition {
            var clone: LayoutPosition = new LayoutPosition(this.detailsLevel);
            clone.copyFrom(this);
            return clone;
        }
    }
}