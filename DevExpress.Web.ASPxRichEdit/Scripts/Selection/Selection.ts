module __aspxRichEdit {
    export class SelectionStateInfo {
        keepX: number;
        forwardDirection: boolean;
        intervals: LinkedInterval[];
        endOfLine: boolean;
        layoutPageIndex: number; // == -1 in case selection in main sub document, >= 0 in case change header\footer. this set mouseHandler or ribbon when create new header\footer
        private manager: PositionManager;

        constructor(manager: PositionManager, intervals: FixedInterval[], forwardDirection: boolean, keepX: number, endOfLine: boolean) {
            this.manager = manager;
            this.keepX = keepX;
            this.forwardDirection = forwardDirection;
            this.endOfLine = endOfLine;
            this.intervals = [];

            for (var i: number = 0, interval: FixedInterval; interval = intervals[i]; i++)
                this.intervals.push(new LinkedInterval(manager, interval.start, interval.end()));
        }

        destructor() {
            for (var i: number = 0, interval: LinkedInterval; interval = this.intervals[i]; i++)
                interval.destructor(this.manager);
            this.intervals = [];
        }
    }

    export class Selection {
        // http://workservices01/OpenWiki/ow.asp?p=ASPxRichEdit_Selection
        public model: DocumentModel;
        //public subDocument: SubDocument;
        public layout: DocumentLayout;
        private inFocus: boolean = false; // editor in focus
        public keepX: number = -1; // stored x position in pixels. Only moveCaretUp and down set it to some value, rest selection command set it to -1.
        //public state: SelectionState;
        public selectedInlinePictureRunPosition: number = -1;
        public forwardDirection: boolean = true; // store for last selection
        public endOfLine: boolean = false; // store for last selection. Useless if end of last interval != end line or start line
        public intervals: FixedInterval[] = [new FixedInterval(0, 0)];
        public pageIndex: number = -1; // >= 0 in case write in NOT main sub document;

        public onChanged: EventDispatcher<ISelectionChangesListener> = new EventDispatcher<ISelectionChangesListener>();
        public tableNestedLevel: number = -1;

        public inputPosition: InputPosition; // need set it manually!
        private fieldResultPartFullySelected: Field = null; // when field - it mean that we have field {code}image> and selected all field. Need right select image with borders
        private lastSelectedIntervalIndex: number = -1;
        private selectedCells: TableCell[][];

        constructor(model: DocumentModel, layout: DocumentLayout) {
            this.model = model;
            this.layout = layout;
        }

        // to get mergedIntervals use Utils.getMergedIntervals

        // NEED CALL DESTRUCTOR of SelectionStateInfo or call restoreSelectionState
        public getSelectionState(): SelectionStateInfo {
            return new SelectionStateInfo(this.model.activeSubDocument.positionManager, this.intervals, this.forwardDirection, this.keepX, this.endOfLine);
        }

        public restoreSelectionState(selectionState: SelectionStateInfo, upd: UpdateInputPositionProperties) {
            this.keepX = selectionState.keepX;
            this.intervals = [];

            for (var i: number = 0, interval: LinkedInterval; interval = selectionState.intervals[i]; i++) {
                var newInterval: FixedInterval = interval.getFixedInterval();
                // here no correct interval due to fields. Because we think that previous interval - correct.
                this.addIntervalToSelection(newInterval, selectionState.forwardDirection, selectionState.endOfLine, upd);
            }

            selectionState.destructor();
        }

        public setFocusState(focused: boolean) {
            if (this.inFocus !== focused) {
                this.inFocus = focused;
                this.raiseFocusChanged();
            }
        }

        public isInFocus(): boolean {
            return this.inFocus;
        }

        public getIntervalsClone(): FixedInterval[]{
            var intervals: FixedInterval[] = [];
            for (var i: number = 0, interval: FixedInterval; interval = this.intervals[i]; i++)
                intervals.push(interval.clone());
            return intervals;
        }

        public isCollapsed(): boolean {
            return this.intervals.length == 1 && this.intervals[0].length == 0;
        }

        public getLastSelectedInterval(): FixedInterval {
            return this.lastSelectedIntervalIndex > -1 ? this.intervals[this.lastSelectedIntervalIndex] : this.intervals[0];
        }

        public setSelection(firstPosition: number, secondPosition: number, endOfLine: boolean, keepX: number, upd: UpdateInputPositionProperties, correctIntervalDueToFields: boolean = true,
            correctIntervalDueToTables: boolean = true) {
            this.keepX = keepX;
            var newInterval: FixedInterval = new FixedInterval(Math.min(firstPosition, secondPosition), Math.abs(firstPosition - secondPosition));
            if (this.endOfLine != endOfLine || this.intervals.length != 1 || !this.intervals[0].equals(newInterval)) {
                this.intervals = [];
                this.fieldResultPartFullySelected = null;
                if(correctIntervalDueToTables)
                    this.correctIntervalDueToTables(this.model.activeSubDocument, newInterval);
                if(correctIntervalDueToFields)
                    this.fieldResultPartFullySelected = Field.correctIntervalDueToFields(this.layout, this.model.activeSubDocument, newInterval, this.pageIndex);
                this.addIntervalToSelection(newInterval, secondPosition >= firstPosition, endOfLine, upd);
            }
        }

        // add new selection to existing
        public addSelection(anchor: number, end: number, endOfLine: boolean, keepX: number, correctIntervalDueToTables: boolean = true) {
            this.keepX = keepX;
            var forwardDirection: boolean = (end >= anchor);
            var newInterval: FixedInterval = new FixedInterval(Math.min(anchor, end), Math.abs(anchor - end));
            if(this.forwardDirection != forwardDirection || this.endOfLine != endOfLine || !this.getLastSelectedInterval().equals(newInterval)) {
                if(this.isCollapsed())
                    this.intervals = [];
                if(correctIntervalDueToTables)
                    this.correctIntervalDueToTables(this.model.activeSubDocument, newInterval);
                this.fieldResultPartFullySelected = Field.correctIntervalDueToFields(this.layout, this.model.activeSubDocument, newInterval, this.pageIndex);
                this.addIntervalToSelection(newInterval, forwardDirection, endOfLine, UpdateInputPositionProperties.Yes);
            }
        }

        // delete here endOfLine parameter
        public extendLastSelection(end: number, endOfLine: boolean, keepX: number, upd: UpdateInputPositionProperties = UpdateInputPositionProperties.Yes, callInCaseIntervalsEqual: any = null) {
            this.keepX = keepX;
            var lastInterval: FixedInterval = this.getLastSelectedInterval();
            var currInterval: FixedInterval = lastInterval.clone();
            var oldAnchor: number = this.forwardDirection ? lastInterval.start : lastInterval.end();

            var forwardDirection: boolean = (end >= oldAnchor);
            var newInterval: FixedInterval = new FixedInterval(Math.min(oldAnchor, end), Math.abs(oldAnchor - end));
            if(this.endOfLine != endOfLine || this.forwardDirection != forwardDirection || !lastInterval.equals(newInterval)) {
                this.correctIntervalDueToTables(this.model.activeSubDocument, newInterval);
                this.fieldResultPartFullySelected = Field.correctIntervalDueToFields(this.layout, this.model.activeSubDocument, newInterval, this.pageIndex);
                if (callInCaseIntervalsEqual && currInterval.equals(newInterval))
                    callInCaseIntervalsEqual(this.model.activeSubDocument.fields, this);
                else {
                    this.intervals.splice(this.lastSelectedIntervalIndex, 1);
                    this.addIntervalToSelection(newInterval, forwardDirection, endOfLine, upd);
                }
            }
        }

        public extendLastSelectionOnOneSide(end: number, endOfLine: boolean, isDragLeftEdge: boolean): void {
            var lastInterval: FixedInterval = this.getLastSelectedInterval();
            var newInterval: FixedInterval = null;
            var forwardDirection = this.getCalculatedForwardDirection(end);

            if (isDragLeftEdge && end < lastInterval.end())
                newInterval = new FixedInterval(end, lastInterval.length - (end - lastInterval.start));
            else if (!isDragLeftEdge && ((end - lastInterval.start) >= 1))
                newInterval = new FixedInterval(lastInterval.start, end - lastInterval.start);

            if (newInterval && (this.endOfLine != endOfLine || !lastInterval.equals(newInterval))) {
                this.intervals.splice(this.lastSelectedIntervalIndex, 1);                
                this.correctIntervalDueToTables(this.model.activeSubDocument, newInterval);
                this.fieldResultPartFullySelected = Field.correctIntervalDueToFields(this.layout, this.model.activeSubDocument, newInterval, this.pageIndex);
                this.addIntervalToSelection(newInterval, forwardDirection, endOfLine, UpdateInputPositionProperties.No);
            }
        }
        public getSelectedCells(): TableCell[][] {
            if(this.selectedCells)
                return this.selectedCells;
            let firstCell = Table.getTableCellByPosition(this.model.activeSubDocument.tables, this.intervals[0].start);
            if(!firstCell)
                return this.selectedCells = [];
            let lastCell = Table.getTableCellByPosition(this.model.activeSubDocument.tables, Math.max(this.intervals[0].start, this.intervals[this.intervals.length - 1].end() - 1));
            if(!lastCell)
                return this.selectedCells = [];
            return this.selectedCells = this.getSelectedCellsCore(firstCell, lastCell);
        }
        public isSelectedEntireTable(): boolean {
            let selectedCells = this.getSelectedCells();
            if(!selectedCells.length)
                return false;
            let table = selectedCells[0][0].parentRow.parentTable;
            if(table.rows.length !== selectedCells.length)
                return false;
            for(let rowIndex = 0, row: TableRow; row = table.rows[rowIndex]; rowIndex++) {
                if(row.cells.length !== selectedCells[rowIndex].length)
                    return false;
            }
            return true;
        }
        // *****************************************************************************************************************************
        // PRIVATE
        private getSelectedCellsCore(firstCell: TableCell, lastCell: TableCell): TableCell[][] {
            let normalizedTable = this.normalizeSelectedTable(firstCell, lastCell);
            if(!normalizedTable)
                return [];
            let selectionIntervalIndex = 0;
            let selectionInterval: FixedInterval = this.intervals[selectionIntervalIndex].clone();
            let result: TableCell[][] = [];
            let prevParentRow: TableRow;
            for(let i = 0, row: TableRow; row = normalizedTable.rows[i]; i++) {
                for(let j = 0, cell: TableCell; cell = row.cells[j]; j++) {
                    let cellInterval = cell.getInterval();
                    if(selectionInterval.start >= cellInterval.start && selectionInterval.start < cellInterval.end()) {
                        if(cell.parentRow !== prevParentRow)
                            result.push([cell]);
                        else
                            result[result.length - 1].push(cell);
                        prevParentRow = cell.parentRow;
                        selectionInterval = FixedInterval.fromPositions(Math.min(selectionInterval.end(), cellInterval.end()), selectionInterval.end());
                        if(selectionInterval.length === 0)
                            selectionInterval = this.intervals[++selectionIntervalIndex];
                    }
                    if(!selectionInterval)
                        return result;
                }
            }
            return result;
        }
        private normalizeSelectedTable(firstCell: TableCell, lastCell: TableCell): Table {
            let sameLevelCells = TableCellUtils.getSameTableCells(firstCell, lastCell);
            if(!sameLevelCells)
                return null;
            let table = sameLevelCells.firstCell.parentRow.parentTable;
            while(this.tableNestedLevel >= 0 && table.nestedLevel > this.tableNestedLevel)
                table = table.parentCell.parentRow.parentTable;
            return table;
        }
        private correctIntervalDueToTables(subDocument: SubDocument, newInterval: FixedInterval): void {
            if(newInterval.length == 0 || subDocument.tables.length == 0)
                return;
            var startCell: TableCell = Table.getTableCellByPosition(subDocument.tables, newInterval.start);
            var endCell: TableCell = Table.getTableCellByPosition(subDocument.tables, newInterval.end());
            if(startCell == null && endCell == null)
                return;

            if(startCell == null && endCell != null) {
                if(newInterval.end() != endCell.parentRow.getStartPosition())
                    newInterval.expandInterval(new FixedInterval(newInterval.end(), endCell.parentRow.getEndPosition() - newInterval.end()));
                return;
            }
            if((startCell != null && endCell == null) || this.firstCellIsParentCellForSecondCellsTable(endCell, startCell)) {
                if(newInterval.end() != startCell.parentRow.parentTable.getEndPosition())
                    newInterval.expandInterval(new FixedInterval(startCell.parentRow.getStartPosition(), newInterval.start - startCell.parentRow.getStartPosition()));
                return;
            }

            var startTable: Table = Table.getTableByPosition(subDocument.tables, newInterval.start, false);
            var endTable: Table = Table.getTableByPosition(subDocument.tables, newInterval.end(), false);
            if(startTable != endTable) {
                newInterval.expandInterval(new FixedInterval(startCell.parentRow.getStartPosition(), newInterval.start - startCell.parentRow.getStartPosition()));
                if(newInterval.end() != endCell.parentRow.getStartPosition())
                    newInterval.expandInterval(new FixedInterval(newInterval.end(), endCell.parentRow.getEndPosition() - newInterval.end()));
                return;
            }
            if(startCell != null && newInterval.end() == startCell.endParagrapPosition.value) {
                newInterval.expandInterval(startCell.getInterval());
                return;
            }
            if(startCell != null && endCell != null && startCell != endCell && startCell.parentRow.parentTable == endCell.parentRow.parentTable) {
                newInterval.expandInterval(startCell.getInterval());
                if(newInterval.end() != endCell.startParagraphPosition.value)
                    newInterval.expandInterval(endCell.getInterval());
                return;
            }
        }
        private firstCellIsParentCellForSecondCellsTable(firstCell: TableCell, secondCell: TableCell): boolean {
            if(firstCell == null || secondCell == null || secondCell.parentRow.parentTable.parentCell == null)
                return false;

            var parentTable: Table = secondCell.parentRow.parentTable;
            while(parentTable.parentCell != null) {
                if(parentTable.parentCell == firstCell)
                    return true;
                parentTable = parentTable.parentCell.parentRow.parentTable;
            }
            return false;
        }
        private getCalculatedForwardDirection(end: number): boolean {
            var lastInterval: FixedInterval = this.getLastSelectedInterval();
            var oldAnchor: number = this.forwardDirection ? lastInterval.start : lastInterval.end();
            return end >= oldAnchor
        }
        private addIntervalToSelection(newInterval: FixedInterval, forwardDirection: boolean, endOfLine: boolean, upd: UpdateInputPositionProperties) {
            if (this.model.activeSubDocument.getDocumentEndPosition() == newInterval.start && newInterval.length == 0)
                throw new Error("Impossible set selection with length == 0 after last paragraph run.");
            this.insertNewInterval(newInterval);
            this.forwardDirection = forwardDirection;
            this.endOfLine = endOfLine;
            this.updateState();
            if (upd == UpdateInputPositionProperties.Yes)
                this.inputPosition.reset();
            this.raiseSelectionChanged();
        }

        private insertNewInterval(interval: FixedInterval) {
            this.selectedCells = undefined;
            this.tableNestedLevel = -1;
            interval = interval.clone();
            let intervalEnd = interval.end();
            if(this.intervals.length) {
                let prevSiblingIntervalIndex = Utils.normedBinaryIndexOf(this.intervals, int => int.start - interval.start);
                let prevSiblingInterval = this.intervals[prevSiblingIntervalIndex];
                let nextSiblingInterval = this.intervals[prevSiblingIntervalIndex + 1];
                if(prevSiblingInterval && interval.start <= prevSiblingInterval.end()) {
                    if(intervalEnd > prevSiblingInterval.end()) {
                        if(!nextSiblingInterval || nextSiblingInterval.start > intervalEnd)
                            prevSiblingInterval.length += interval.end() - prevSiblingInterval.end();
                        else {
                            prevSiblingInterval.length = nextSiblingInterval.end() - prevSiblingInterval.start;
                            this.intervals.splice(prevSiblingIntervalIndex + 1, 1);
                            if(intervalEnd > nextSiblingInterval.end())
                                this.insertNewInterval(new FixedInterval(nextSiblingInterval.start, intervalEnd - nextSiblingInterval.start));
                        }
                    }
                    this.lastSelectedIntervalIndex = prevSiblingIntervalIndex;
                }
                else {
                    if(!nextSiblingInterval || nextSiblingInterval.start > intervalEnd)
                        this.intervals.splice(prevSiblingIntervalIndex + 1, 0, interval);
                    else {
                        nextSiblingInterval.length += nextSiblingInterval.start - interval.start;
                        nextSiblingInterval.start = interval.start;
                        if(intervalEnd > nextSiblingInterval.end())
                            this.insertNewInterval(new FixedInterval(nextSiblingInterval.end(), interval.end() - nextSiblingInterval.end()));
                    }
                    this.lastSelectedIntervalIndex = prevSiblingIntervalIndex + 1;
                }
            }
            else {
                this.intervals.push(interval);
                this.lastSelectedIntervalIndex = 0;
            }
        }
        private updateState() {
            this.selectedInlinePictureRunPosition = -1;
            if (this.fieldResultPartFullySelected) {
                var resultInterval: FixedInterval = this.fieldResultPartFullySelected.getResultInterval();
                this.fieldResultPartFullySelected = null;

                if (resultInterval.length == 1) {
                    var run: TextRun = this.model.activeSubDocument.getRunByPosition(resultInterval.start);
                    if (run.type == TextRunType.InlinePictureRun)
                        this.selectedInlinePictureRunPosition = resultInterval.start;
                }
                return;
            }

            if (this.intervals.length === 1 && this.intervals[0].length === 1) {
                var firstSelectionIntervalStart: number = this.intervals[0].start;
                var runInfo: { chunkIndex: number; runIndex: number; chunk: Chunk; run: TextRun } = this.model.activeSubDocument.getRunAndIndexesByPosition(firstSelectionIntervalStart);
                var absoluteRunPosition: number = runInfo.chunk.startLogPosition.value + runInfo.run.startOffset;
                if (runInfo.run.type === TextRunType.InlinePictureRun && firstSelectionIntervalStart == absoluteRunPosition) {
                    this.selectedInlinePictureRunPosition = absoluteRunPosition;
                    return;
                }
            }
        }

        raiseSelectionChanged() {
            this.onChanged.raise("NotifySelectionChanged", this);
        }

        private raiseFocusChanged() {
            this.onChanged.raise("NotifyFocusChanged", this.inFocus);
        }
    }
     
    export enum UpdateInputPositionProperties {
        Yes,
        No
    }
} 