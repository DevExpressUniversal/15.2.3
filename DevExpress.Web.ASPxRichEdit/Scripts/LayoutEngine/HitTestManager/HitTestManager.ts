module __aspxRichEdit {
    // http://workservices01/OpenWiki/ow.asp?p=ASPxRichEdit_HitTestManager
    const TABLECELL_TOPOFFSET = 3;

    export class HitTestManager {
        private documentLayout: DocumentLayout;
        private measurer: IBoxMeasurer;

        private result: HitTestResult;
        private subDocument: SubDocument;

        // Please. Always set subDocument. it's work faster
        // subDocument. If subDocument set - then search only position in this subDoc.
        constructor(documentLayout: DocumentLayout, measurer: IBoxMeasurer) {
            this.documentLayout = documentLayout;
            this.measurer = measurer;
            this.result = null;
        }

        public calculate(point: LayoutPoint, requestDetailsLevel: DocumentLayoutDetailsLevel, subDocument: SubDocument): HitTestResult {
            if(!point || point.isEmpty())
                return;
            this.subDocument = subDocument;
            this.result = new HitTestResult(subDocument);
            this.result.detailsLevel = requestDetailsLevel;

            this.calcPage(point);
            return this.result;
        }

        private calcPage(point: LayoutPoint) {
            const page: LayoutPage = this.documentLayout.pages[point.pageIndex];
            this.result.pageIndex = point.pageIndex;
            this.result.page = page;
            const pageDeviation: HitTestDeviation = HitTestManager.getDeviation(point.pageX, point.pageY, 0, 0, page.width, page.height);
            this.result.deviations[DocumentLayoutDetailsLevel.Page] = pageDeviation;
            if (pageDeviation == HitTestDeviation.None)
                this.result.exactlyDetailLevel = DocumentLayoutDetailsLevel.Page;
            if (this.result.detailsLevel > DocumentLayoutDetailsLevel.Page)
                this.calcPageArea(point.pageX, point.pageY);
        }

        private calcPageArea(pointX: number, pointY: number) {
            let pageArea: LayoutPageArea;
            let pageAreaIndex: number;
            if (this.subDocument) {
                if (this.subDocument.isMain()) {
                    const pageAreas: LayoutPageArea[] = this.result.page.mainSubDocumentPageAreas;
                    pageAreaIndex = HitTestManager.findNearest(pointY, pageAreas, (pa: LayoutPageArea) => pa.y, (pa: LayoutPageArea) => pa.y + pa.height);
                    pageArea = pageAreas[pageAreaIndex];
                }
                else {
                    pageArea = this.result.page.otherPageAreas[this.subDocument.id];
                    pageAreaIndex = 0;
                    if (!pageArea)
                        return;
                }
            }
            else {
                // collect all pageAreas
                let pageAreas: LayoutPageArea[] = this.result.page.mainSubDocumentPageAreas.slice(); // [mainSubDocPageAreas, otherPageAreas]
                const otherPageAreas: { [subDocumentId: number]: LayoutPageArea } = this.result.page.otherPageAreas; 
                for (let subDocId in otherPageAreas) {
                    if (!otherPageAreas.hasOwnProperty(subDocId))
                        continue;
                    pageAreas.push(otherPageAreas[subDocId]);
                }

                // determine closest pageArea
                pageAreas.sort((a: LayoutPageArea, b: LayoutPageArea) => a.y - b.y);
                let getMinSquareDistance = (pageArea: LayoutPageArea): number => Math.min(Math.pow(pageArea.y - pointY, 2), Math.pow(pageArea.y + pageArea.height - pointY, 2));
                let pageAreaMinDistance: number = getMinSquareDistance(pageAreas[0]);
                let pageAreaIndexWithMinDistance: number = 0;
                for (let currPageAreaIndex: number = 1, currPageArea: LayoutPageArea; currPageArea = pageAreas[currPageAreaIndex]; currPageAreaIndex++) {
                    const currSqDist: number = getMinSquareDistance(currPageArea);
                    const pageAreaWithMinDistance = pageAreas[pageAreaIndexWithMinDistance];
                    if (currSqDist < pageAreaMinDistance || currSqDist == pageAreaMinDistance && pointY >= pageAreaWithMinDistance.y + pageAreaWithMinDistance.height) {
                        pageAreaMinDistance = currSqDist;
                        pageAreaIndexWithMinDistance = currPageAreaIndex;
                    }
                }

                pageAreaIndex = pageAreaIndexWithMinDistance < this.result.page.mainSubDocumentPageAreas.length ? pageAreaIndexWithMinDistance : 0;
                pageArea = pageAreas[pageAreaIndexWithMinDistance];
            }

            this.result.pageArea = pageArea;
            this.result.pageAreaIndex = pageAreaIndex;

            const pageAreaDeviation: HitTestDeviation = HitTestManager.getDeviation(pointX, pointY, pageArea.x, pageArea.y, pageArea.x + pageArea.width, pageArea.y + pageArea.height)
                | this.result.deviations[DocumentLayoutDetailsLevel.Page];
            this.result.deviations[DocumentLayoutDetailsLevel.PageArea] = pageAreaDeviation;
            if (pageAreaDeviation == HitTestDeviation.None)
                this.result.exactlyDetailLevel = DocumentLayoutDetailsLevel.PageArea;
            if (this.result.detailsLevel > DocumentLayoutDetailsLevel.PageArea)
                this.calcColumn(pointX - pageArea.x, pointY - pageArea.y);
        }

        private calcColumn(pointX: number, pointY: number) {
            let columns: LayoutColumn[] = this.result.pageArea.columns;
            let columnIndex: number = HitTestManager.findNearest(pointX, columns, (col: LayoutColumn) => col.x, (col: LayoutColumn) => col.x + col.width);
            let column: LayoutColumn = columns[columnIndex];

            this.result.columnIndex = columnIndex;
            this.result.column = column;

            const columnDeviation: HitTestDeviation = HitTestManager.getDeviation(pointX, pointY, column.x, column.y, column.x + column.width, column.y + column.height)
                | this.result.deviations[DocumentLayoutDetailsLevel.PageArea];
            this.result.deviations[DocumentLayoutDetailsLevel.Column] = columnDeviation;
            if (columnDeviation == HitTestDeviation.None)
                this.result.exactlyDetailLevel = DocumentLayoutDetailsLevel.Column;
            if (this.result.detailsLevel > DocumentLayoutDetailsLevel.Column)
                this.calcRow(pointX - column.x, pointY - column.y);
        }

        private calcRow(pointX: number, pointY: number) {
            const rows: LayoutRow[] = this.result.column.rows;
            let rowIndex: number;
            let row: LayoutRow;
            const tableColumnInfos: LayoutTableColumnInfo[] = this.result.column.tablesInfo;
            let targetTableColumnInfo: LayoutTableColumnInfo;
            if(tableColumnInfos.length > 0) {
                let deviationTableColumnInfo: LayoutTableColumnInfo = null;
                for (let tableColumnInfoIndex: number = tableColumnInfos.length - 1, currTableColumnInfo: LayoutTableColumnInfo; currTableColumnInfo = tableColumnInfos[tableColumnInfoIndex]; tableColumnInfoIndex--) {
                    if(currTableColumnInfo.containsPoint(pointX, pointY)) {
                        targetTableColumnInfo = currTableColumnInfo;
                        break;
                    }
                    else if(pointY >= currTableColumnInfo.y && pointY <= currTableColumnInfo.getBottomBoundPosition())
                        deviationTableColumnInfo = currTableColumnInfo;
                    else {
                        let topDelta = currTableColumnInfo.y - pointY;
                        if((topDelta < TABLECELL_TOPOFFSET && topDelta >= 0) || (currTableColumnInfo.y === 0 && pointY < 0))
                            deviationTableColumnInfo = currTableColumnInfo;
                    }
                }
                targetTableColumnInfo = targetTableColumnInfo || deviationTableColumnInfo;
            }

            if (targetTableColumnInfo) {
                let tableCellInfo: LayoutTableCellInfo;
                let exactRowInfoIndex = Math.max(0, Utils.normedBinaryIndexOf(targetTableColumnInfo.tableRows, (r: LayoutTableRowInfo) => r.bound.y - pointY));
                let foundExactCell = false;
                for(let tableRowInfoIndex = exactRowInfoIndex; tableRowInfoIndex >= 0; tableRowInfoIndex--) {
                    const tableRowInfo: LayoutTableRowInfo = targetTableColumnInfo.tableRows[tableRowInfoIndex];
                    const tableCellInfoIndex: number = Math.max(0, Utils.normedBinaryIndexOf(tableRowInfo.rowCells, (c: LayoutTableCellInfo) => c.bound.x - pointX));
                    tableCellInfo = tableRowInfo.rowCells[tableCellInfoIndex];
                    if(tableCellInfo.bound.containsPoint(pointX, pointY)) {
                        foundExactCell = true;
                        break;
                    }
                }
                if(!foundExactCell) {
                    let tableRowInfo = targetTableColumnInfo.tableRows[exactRowInfoIndex];
                    if(pointX < tableRowInfo.rowCells[0].bound.x) {
                        this.result.deviations[DocumentLayoutDetailsLevel.TableCell] = HitTestDeviation.Left;
                        tableCellInfo = tableRowInfo.rowCells[0];
                    }
                    else if(exactRowInfoIndex === 0 && tableRowInfo.rowIndex === 0 && pointY < tableRowInfo.bound.y) {
                        this.result.deviations[DocumentLayoutDetailsLevel.TableCell] = HitTestDeviation.Top;
                        for(let i = 0, tableRowInfo: LayoutTableRowInfo; tableRowInfo = targetTableColumnInfo.tableRows[i]; i++) {
                            for(let cellInfoIndex = 0, btmTableCellInfo: LayoutTableCellInfo; btmTableCellInfo = tableRowInfo.rowCells[cellInfoIndex]; cellInfoIndex++) {
                                if(pointX >= btmTableCellInfo.bound.x && pointX <= btmTableCellInfo.bound.getRightBoundPosition()) {
                                    tableCellInfo = btmTableCellInfo;
                                    foundExactCell = true;
                                    break;
                                }
                            }
                            if(foundExactCell)
                                break;
                        }
                        this.result.deviations[DocumentLayoutDetailsLevel.TableCell] = HitTestDeviation.Top;
                        tableCellInfo = tableCellInfo || tableRowInfo.rowCells[0];
                    }
                    else {
                        this.result.deviations[DocumentLayoutDetailsLevel.TableCell] = HitTestDeviation.Right;
                        tableCellInfo = tableRowInfo.rowCells[tableRowInfo.rowCells.length - 1];
                    }
                }

                const layoutRowAndIndex_Index: number = Math.max(0, Utils.normedBinaryIndexOf(tableCellInfo.layoutRows, (r: LayoutRowWithIndex) => r.y - pointY));

                rowIndex = tableCellInfo.layoutRows[layoutRowAndIndex_Index].indexInColumn;
                row = rows[rowIndex];
            }
            else {
                rowIndex = Math.max(0, Utils.normedBinaryIndexOf(rows, (r: LayoutRow) => r.y - pointY));
                row = rows[rowIndex];
            }

            this.result.rowIndex = rowIndex;
            this.result.row = row;

            const rowDeviation: HitTestDeviation = HitTestManager.getDeviation(pointX, pointY, row.x, row.y, row.x + row.width, row.y + row.height)
                | this.result.deviations[DocumentLayoutDetailsLevel.Column];
            this.result.deviations[DocumentLayoutDetailsLevel.Row] = rowDeviation;
            if (rowDeviation == HitTestDeviation.None)
                this.result.exactlyDetailLevel = DocumentLayoutDetailsLevel.Row;
            if (this.result.detailsLevel > DocumentLayoutDetailsLevel.Row)
                this.calcBox(pointX - row.x, pointY - row.y);
        }

        private calcBox(pointX: number, pointY: number) {
            const boxes: LayoutBox[] = this.result.row.boxes;
            const boxIndex: number = Math.max(0, Utils.normedBinaryIndexOf(boxes, (b: LayoutBox) => b.x - pointX));
            const box: LayoutBox = boxes[boxIndex];

            const boxLeftBorder: number = box.x;
            const boxRightBorder: number = boxLeftBorder + box.width;
            const boxTopBorder: number = this.result.row.baseLine - box.getAscent();
            let boxBottomBorder: number = box.height + boxTopBorder;
            if (boxBottomBorder > this.result.row.height)
                boxBottomBorder = this.result.row.height;

            this.result.boxIndex = boxIndex;
            this.result.box = this.result.row.boxes[boxIndex];

            const boxDeviation: HitTestDeviation = HitTestManager.getDeviation(pointX, pointY, boxLeftBorder, boxTopBorder, boxRightBorder, boxBottomBorder)
                | this.result.deviations[DocumentLayoutDetailsLevel.Row];
            this.result.deviations[DocumentLayoutDetailsLevel.Box] = boxDeviation;
            if (boxDeviation == HitTestDeviation.None)
                this.result.exactlyDetailLevel = DocumentLayoutDetailsLevel.Box;
            if (this.result.detailsLevel > DocumentLayoutDetailsLevel.Box)
                this.calcCharacter(pointX - boxLeftBorder, pointY - boxTopBorder);
        }

        private calcCharacter(pointX: number, pointY: number) {
            const boxDeviation: HitTestDeviation = this.result.deviations[DocumentLayoutDetailsLevel.Box];
            let boxOffset: number = -1;
            if (boxDeviation & HitTestDeviation.Left)
                boxOffset = 0;
            else if (boxDeviation & HitTestDeviation.Right)
                boxOffset = this.result.box.getLength();
            else
                boxOffset = this.result.box.calculateCharOffsetByPointX(this.measurer, pointX);

            this.result.charOffset = boxOffset;
        }

        private static getDeviation(x: number, y: number, leftBorder: number, topBorder: number, rightBorder: number, bottomBorder: number): HitTestDeviation {
            let deviation: HitTestDeviation = HitTestDeviation.None;
            if (x < leftBorder)
                deviation |= HitTestDeviation.Left;
            else
                if (x > rightBorder)
                    deviation |= HitTestDeviation.Right;

            if (y < topBorder)
                deviation |= HitTestDeviation.Top;
            else
                if (y > bottomBorder)
                    deviation |= HitTestDeviation.Bottom;
            return deviation;
        }

        // return index
        private static findNearest<T>(point: number, objects: T[], minBound: (a: T) => number, maxBound: (a: T) => number): number {
            let currObj: T = objects[0];
            let nextObjIndex: number = 1;
            for (let nextObj: T; nextObj = objects[nextObjIndex]; nextObjIndex++) {
                if (point - maxBound(currObj) <= minBound(nextObj) - point)
                    break;
                currObj = nextObj;
            }
            return nextObjIndex - 1;
        }
    }
}