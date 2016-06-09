module __aspxRichEdit {
    export class SelectionFormatter extends BatchUpdatableObject implements ILayoutChangesListener, ISelectionChangesListener {
        layout: DocumentLayout;
        layoutSelection: LayoutSelection;
        measurer: IBoxMeasurer;
        selection: Selection;
        onSelectionLayoutChanged: EventDispatcher<ISelectionLayoutChangesListener> = new EventDispatcher<ISelectionLayoutChangesListener>();
        private control: IRichEditControl;
        private occuredPageChanges: PageChange[] = [];

        constructor(selection: Selection, layout: DocumentLayout, measurer: IBoxMeasurer, control: IRichEditControl) {
            super();
            this.selection = selection;
            this.layout = layout;
            this.measurer = measurer;
            this.control = control;
            this.layoutSelection = new LayoutSelection(control.model.activeSubDocument.info);
        }

        // ISelectionChangesListener
        NotifySelectionChanged(selection: Selection) {
            if(!this.isUpdateLocked())
                this.onSelectionChanged(selection);
            else
                this.registerOccurredEvent(SelectionFormatterEvents.SelectionChanged);
        }

        NotifyFocusChanged(inFocus: boolean) {
            this.raiseFocusChanged(inFocus);
        }

        onSelectionChanged(selection: Selection) {
            var subDocument: SubDocument = this.selection.model.activeSubDocument;
            var lastSelectedInterval = selection.getLastSelectedInterval();
            var wholeDocumentIsSelected = this.selection.intervals.length === 1 && lastSelectedInterval.start === 0 && lastSelectedInterval.end() === subDocument.getDocumentEndPosition() && subDocument.isMain();
            var layoutSelection: LayoutSelection = this.createSelectionLayout(selection.intervals, !wholeDocumentIsSelected, true, true);
            if (!layoutSelection) // when page will be ready, then onSelectionChanged will be called once again
                return;
            this.raiseSelectionLayoutChaged(layoutSelection);
            this.layoutSelection = layoutSelection;
        }

        // ILayoutChangesListener
        NotifyPagesInvalidated(firstInvalidPageIndex: number) { }

        NotifyPageReady(pageChange: PageChange) {
            if(this.isUpdateLocked()) {
                this.registerOccurredEvent(SelectionFormatterEvents.PageReady);
                this.occuredPageChanges.push(pageChange);
            }
            else
                this.onPageReady(pageChange);
        }

        private onPageReady(pageChange: PageChange) {
            if(pageChange.changeType == LayoutChangeType.Deleted) {
                this.applySelectionLayoutChange(pageChange.index, null);
                return;
            }
            const page: LayoutPage = pageChange.layoutElement;
            var layoutSelection: LayoutSelection = null;
            var subDocument = this.control.model.activeSubDocument;
            if(subDocument.isMain()) {
                let pageEndPosition: number = page.getEndPosition(page.mainSubDocumentPageAreas[0].subDocument); // place main subDocument
                let pageInterval: FixedInterval = FixedInterval.fromPositions(page.contentIntervals[0].start, pageEndPosition);
                let selectionIntervals = this.control.selection.intervals;
                let pageSelectionIntersections = this.getIntersections(selectionIntervals, pageInterval);
                if(pageSelectionIntersections.length)
                    layoutSelection = this.createSelectionLayout(pageSelectionIntersections, false, pageInterval.start === selectionIntervals[0].start, pageInterval.end() === selectionIntervals[selectionIntervals.length - 1].end());
            }
            else if(page.otherPageAreas[subDocument.id] && page.index == this.selection.pageIndex) {
                let selectionIntervals = this.getIntersections(this.control.selection.intervals, FixedInterval.fromPositions(0, page.getEndPosition(subDocument)));
                if(selectionIntervals.length)
                    layoutSelection = this.createSelectionLayout(selectionIntervals, false, true, true);
            }

            this.applySelectionLayoutChange(pageChange.index, layoutSelection);
        }

        private getIntersections(orderedIntervals: FixedInterval[], target: FixedInterval): FixedInterval[]{
            var result: FixedInterval[] = [];
            for(let i = 0, interval: FixedInterval; interval = orderedIntervals[i]; i++) {
                let intersection = FixedInterval.getIntersection(interval, target);
                if(intersection)
                    result.push(intersection);
            }
            return result;
        }

        private applySelectionLayoutChange(pageIndex: number, selectionLayout: LayoutSelection) {
            this.raisePageSelectionLayoutChanged(pageIndex, selectionLayout);
            if(!selectionLayout || !selectionLayout.pages[pageIndex])
                delete this.layoutSelection.pages[pageIndex];
            else
                this.layoutSelection.pages[pageIndex] = selectionLayout.pages[pageIndex];
        }

        private createSelectionLayout(selectionIntervals: FixedInterval[], detectScrollPosition: boolean, containsFirstPoint: boolean, containsLastPoint: boolean): LayoutSelection {
            var subDocument: SubDocument = this.selection.model.activeSubDocument;
            var selectionLayout: LayoutSelection = new LayoutSelection(this.selection.model.activeSubDocument.info);
            selectionLayout.pageIndex = this.selection.pageIndex;
            if (this.selection.isCollapsed()) {
                var cursorPos: LayoutPosition = subDocument.isMain()
                    ? new LayoutPositionMainSubDocumentCreator(this.layout, subDocument, selectionIntervals[0].start, DocumentLayoutDetailsLevel.Character)
                        .create(new LayoutPositionCreatorConflictFlags().setDefault(this.selection.endOfLine), new LayoutPositionCreatorConflictFlags().setDefault(false))
                    : new LayoutPositionOtherSubDocumentCreator(this.layout, subDocument, selectionIntervals[0].start, this.selection.pageIndex, DocumentLayoutDetailsLevel.Character)
                        .create(new LayoutPositionCreatorConflictFlags().setDefault(this.selection.endOfLine), new LayoutPositionCreatorConflictFlags().setDefault(false));
                if (!cursorPos)
                    return null;
                if(cursorPos.boxIndex > 0 && cursorPos.charOffset === 0) {
                    for(var i = cursorPos.boxIndex - 1; i >= 0; i--) {
                        if(cursorPos.row.boxes[i].isVisible()) {
                            cursorPos.boxIndex = i;
                            cursorPos.box = cursorPos.row.boxes[cursorPos.boxIndex];
                            cursorPos.charOffset = cursorPos.box.getLength();
                            break;
                        }
                    }
                }

                var selectionCursor: LayoutSelectionCursor = new LayoutSelectionCursor(cursorPos.pageArea.subDocument.id, cursorPos.pageIndex, cursorPos.pageAreaIndex, cursorPos.columnIndex);
                selectionCursor.width = 1;
                selectionCursor.height = cursorPos.box.height;
                selectionCursor.y = cursorPos.row.y + cursorPos.row.getSpacingBefore() + cursorPos.row.baseLine - cursorPos.box.getAscent() - cursorPos.row.getSpacingBefore();

                if(cursorPos.box.characterProperties.script === CharacterFormattingScript.Subscript) {
                    var multiplier = cursorPos.box.characterProperties.fontInfo.scriptMultiplier;
                    selectionCursor.y += UnitConverter.pointsToPixels(cursorPos.box.characterProperties.fontSize) * (cursorPos.box.characterProperties.fontInfo.subScriptOffset * multiplier - multiplier + 1);
                }

                selectionCursor.x = cursorPos.row.x + cursorPos.box.x;
                if (cursorPos.box.isVisible())
                    selectionCursor.x += cursorPos.box.getCharOffsetXInPixels(this.measurer, cursorPos.charOffset);

                selectionLayout.registerItem(selectionCursor);
                selectionLayout.scrollPosition = cursorPos;
                //this.control.selection.pageIndex = cursorPos.pageIndex;
                selectionLayout.firstItem = selectionCursor;
                selectionLayout.lastItem = selectionCursor;
                return selectionLayout;
            }

            for(let i = 0, interval: FixedInterval; interval = selectionIntervals[i]; i++) {
                let iterator: LayoutBoxIteratorBase = subDocument.isMain() ? new LayoutBoxIteratorMainSubDocument(subDocument, this.layout, interval.start, interval.end()) : new LayoutBoxIteratorOtherSubDocument(subDocument, this.layout, interval.start, interval.end(), this.selection.pageIndex);
                if(!iterator.isInitialized())
                    return null;

                var currentLayoutRow: LayoutRow;
                var selectionRow: LayoutSelectionItem;
                var isFirstBox: boolean = true;
                while(iterator.moveNext(new LayoutPositionCreatorConflictFlags().setDefault(!isFirstBox), new LayoutPositionCreatorConflictFlags().setDefault(true))) {
                    var currPosition: LayoutPosition = iterator.position;

                    if(isFirstBox && !this.selection.forwardDirection) {
                        if(detectScrollPosition)
                            selectionLayout.scrollPosition = currPosition.clone();
                        //this.control.selection.pageIndex = iterator.position.pageIndex;
                    }

                    var isWhollySelectedCell = false;
                    if(currPosition.row.tableCellInfo) {
                        var cell = Table.getTableCellByPosition(subDocument.tables, currPosition.getLogPosition());
                        isWhollySelectedCell = cell && cell.startParagraphPosition.value >= interval.start && cell.endParagrapPosition.value <= interval.end();
                    }

                    if(isWhollySelectedCell) {
                        if(currPosition.row !== currentLayoutRow) {
                            currentLayoutRow = currPosition.row;
                            selectionRow = new LayoutSelectionItem(currPosition.pageArea.subDocument.id, currPosition.pageIndex, currPosition.pageAreaIndex, currPosition.columnIndex);
                            selectionRow.x = currentLayoutRow.tableCellInfo.bound.x;
                            selectionRow.y = currentLayoutRow.y - currentLayoutRow.getSpacingBefore();
                            selectionRow.width = currentLayoutRow.tableCellInfo.bound.width;
                            selectionRow.height = currentLayoutRow.height;
                            selectionLayout.registerItem(selectionRow);
                        }
                    }
                    else {
                        var rightCharOffset: number = Math.min(interval.end() - currPosition.getLogPosition(DocumentLayoutDetailsLevel.Box), currPosition.box.getLength());
                        var currentBoxLeftOffsetX: number = currPosition.box.getCharOffsetXInPixels(this.measurer, currPosition.charOffset);
                        var currentBoxRightOffsetX: number = currPosition.box.getCharOffsetXInPixels(this.measurer, rightCharOffset);

                        if(currPosition.row !== currentLayoutRow || (selectionRow && selectionRow.x + selectionRow.width !== currPosition.box.x + currPosition.row.x + currentBoxLeftOffsetX)) {
                            currentLayoutRow = currPosition.row;
                            selectionRow = new LayoutSelectionItem(currPosition.pageArea.subDocument.id, currPosition.pageIndex, currPosition.pageAreaIndex, currPosition.columnIndex);
                            selectionRow.x = currentLayoutRow.x + currPosition.box.x + currentBoxLeftOffsetX;
                            selectionRow.y = currentLayoutRow.y;
                            selectionRow.width = 0;
                            selectionRow.height = currentLayoutRow.height;
                            selectionLayout.registerItem(selectionRow);
                        }
                        selectionRow.width += currentBoxRightOffsetX - currentBoxLeftOffsetX;
                    }

                    if(isFirstBox && containsFirstPoint)
                        selectionLayout.firstItem = selectionRow;
                    if(containsLastPoint)
                        selectionLayout.lastItem = selectionRow;
                    isFirstBox = false;
                }
                if(this.selection.forwardDirection) {
                    if(detectScrollPosition)
                        selectionLayout.scrollPosition = iterator.position.clone();
                }
            }
            return selectionLayout;
        }

        onUpdateUnlocked(occurredEvents: number) {
            if(occurredEvents & SelectionFormatterEvents.SelectionChanged)
                this.onSelectionChanged(this.control.selection);
            else if(occurredEvents & SelectionFormatterEvents.PageReady) {
                for(let i = 0, pageChange: PageChange; pageChange = this.occuredPageChanges[i]; i++) {
                    this.onPageReady(pageChange);
                }
                this.occuredPageChanges = [];
            }
        }
        onUpdateLocked() {
            this.occuredPageChanges = [];
        }

        private raiseSelectionLayoutChaged(selectionLayout: LayoutSelection) {
            this.onSelectionLayoutChanged.raise("NotifySelectionLayoutChanged", selectionLayout);
        }

        private raisePageSelectionLayoutChanged(pageIndex: number, selectionLayout: LayoutSelection) {
            this.onSelectionLayoutChanged.raise("NotifyPageSelectionLayoutChanged", pageIndex, selectionLayout);
        }

        private raiseFocusChanged(inFocus: boolean) {
            this.onSelectionLayoutChanged.raise("NotifyFocusChanged", inFocus);
        }
    }

    enum SelectionFormatterEvents {
        SelectionChanged = 1 << 0,
        PageReady = 1 << 1
    }
}