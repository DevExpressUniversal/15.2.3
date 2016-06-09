module __aspxRichEdit {
    export class GoToDocumentEndCommandBase extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        executeCore(state: SimpleCommandState, parameter: any): boolean {
            this.control.formatSync();
            var subDocument: SubDocument = this.control.model.activeSubDocument;
            var pages: LayoutPage[] = this.control.layout.pages;
            var lastPage: LayoutPage = pages[pages.length - 1];

            if (!this.extendSelection()) {
                var pageAreas: LayoutPageArea[] = lastPage.mainSubDocumentPageAreas;
                var lastPageArea: LayoutPageArea = pageAreas[pageAreas.length - 1];
                var lastColumn: LayoutColumn = lastPageArea.columns[lastPageArea.columns.length - 1];
                var lastRow: LayoutRow = lastColumn.rows[lastColumn.rows.length - 1];

                var boxIndex: number = lastRow.getLastVisibleBoxIndex();
                if (boxIndex >= 0) {
                    var box: LayoutBox = lastRow.boxes[boxIndex];
                    this.setSelection((subDocument.isMain() ? lastPage.getStartOffsetContentOfMainSubDocument() : 0) + lastPageArea.pageOffset + lastColumn.pageAreaOffset + lastRow.columnOffset + box.rowOffset + box.getLength());
                }
                else {
                    var box: LayoutBox = lastRow.boxes[0];
                    this.setSelection((subDocument.isMain() ? lastPage.getStartOffsetContentOfMainSubDocument() : 0) + lastPageArea.pageOffset + lastColumn.pageAreaOffset + lastRow.columnOffset + box.rowOffset);
                }
            }
            else
                this.setSelection(lastPage.getEndPosition(subDocument));
            return true;
        }

        setSelection(position: number) {
            throw new Error(Errors.NotImplemented);
        }

        extendSelection(): boolean {
            throw new Error(Errors.NotImplemented);
        }
        isEnabledInReadOnlyMode(): boolean {
            return true;
        }
    }

    export class GoToDocumentEndCommand extends GoToDocumentEndCommandBase {
        setSelection(position: number) {
            this.control.selection.setSelection(position, position, false, -1, UpdateInputPositionProperties.Yes);
        }
        extendSelection(): boolean {
            return false;
        }
    }

    export class ExtendGoToDocumentEndCommand extends GoToDocumentEndCommandBase {
        setSelection(position: number) {
            this.control.selection.extendLastSelection(position, false, -1, UpdateInputPositionProperties.Yes);
        }
        extendSelection(): boolean {
            return true;
        }
    }
}  