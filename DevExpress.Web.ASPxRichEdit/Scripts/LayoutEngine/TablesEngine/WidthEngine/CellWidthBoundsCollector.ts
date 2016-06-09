//module __aspxRichEdit {
//    export class CellWidthBounds extends WidthBounds implements ICloneable<CellWidthBounds> {
//        public firstColumnIndex: number;
//        public endColumnIndex: number;

//        constructor(firstColumnIndex: number, endColumnIndex: number, min: number, max: number) {
//            super(min, max);
//            this.firstColumnIndex = firstColumnIndex;
//            this.endColumnIndex = endColumnIndex;
//        }

//        public clone(): CellWidthBounds {
//            return new CellWidthBounds(this.firstColumnIndex, this.endColumnIndex, this.min, this.max);
//        }
//    }

//    export class CellWidthBoundsCollector {
//        public static getCollector(table: Table): CellWidthBoundsCollector {
//            return table.rows.length < 20 ? new CellWidthBoundsCollectorEffectiveWhenLittleCells() : new CellWidthBoundsCollectorEffectiveWhenManyCells();
//        }

//        public addInfo(info: CellWidthBounds) {
//            throw new Error(Errors.NotImplemented);
//        }

//        public getList(): CellWidthBounds[] {
//            throw new Error(Errors.NotImplemented);
//        }

//        public calculateTableWidth(): WidthBounds {
//            const cellInfos: CellWidthBounds[] = this.getList();
//            this.sortList(cellInfos);
//            const summaryTableWidthsForColumn: WidthBounds[] = [new WidthBounds(0, 0)];
//            let summaryTableWidthForColumnLength: number = 1;
//            for (let cellInfo of cellInfos) {
//                const tableWidthsForCurrentCellStartColumn: WidthBounds = summaryTableWidthsForColumn[cellInfo.firstColumnIndex];
//                const cellPreferredWidthMin: number = tableWidthsForCurrentCellStartColumn.min + cellInfo.min;
//                const cellPreferredWidthMax: number = tableWidthsForCurrentCellStartColumn.max + cellInfo.max;
//                const summInfo: WidthBounds = summaryTableWidthsForColumn[summaryTableWidthForColumnLength - 1];
//                while (summaryTableWidthForColumnLength <= cellInfo.endColumnIndex)
//                    summaryTableWidthForColumnLength = summaryTableWidthsForColumn.push(summInfo.clone());

//                for (let i: number = cellInfo.endColumnIndex, wInfo: WidthBounds; wInfo = summaryTableWidthsForColumn[i]; i++) {
//                    if (wInfo.min < cellPreferredWidthMin)
//                        wInfo.min = cellPreferredWidthMin;
//                    if (wInfo.max < cellPreferredWidthMax)
//                        wInfo.max = cellPreferredWidthMax;
//                }
//            }
//            return summaryTableWidthsForColumn[summaryTableWidthForColumnLength - 1];
//        }

//        private sortList(cellInfos: CellWidthBounds[]) {
//            cellInfos.sort((a: CellWidthBounds, b: CellWidthBounds) => {
//                const startColIndexdiff = a.firstColumnIndex - b.firstColumnIndex;
//                if (startColIndexdiff)
//                    return startColIndexdiff;
//                return a.endColumnIndex - b.endColumnIndex;
//            });
//        }
//    }

//    class CellWidthBoundsCollectorEffectiveWhenLittleCells extends CellWidthBoundsCollector {
//        private cellInfos: CellWidthBounds[] = [];

//        public addInfo(info: CellWidthBounds) {
//            this.cellInfos.push(info);
//        }

//        public getList(): CellWidthBounds[] {
//            return this.cellInfos;
//        }
//    }

//    class CellWidthBoundsCollectorEffectiveWhenManyCells extends CellWidthBoundsCollector {
//        private cellDictionary: { [hash: number]: CellWidthBounds } = {};

//        public addInfo(info: CellWidthBounds) {
//            const hash: number = CellWidthBoundsCollectorEffectiveWhenManyCells.getHash(info);
//            const currVal: CellWidthBounds = this.cellDictionary[hash];
//            if (currVal) {
//                if (currVal.min < info.min)
//                    currVal.min = info.min;
//                if (currVal.max < info.max)
//                    currVal.max = info.max;
//            }
//            else
//                this.cellDictionary[hash] = info;
//        }

//        public getList(): CellWidthBounds[] {
//            const result: CellWidthBounds[] = [];
//            for (let key in result) {
//                if (!this.cellDictionary.hasOwnProperty(key))
//                    continue;
//                result.push(result[key]);
//            }
//            return result;
//        }

//        private static getHash(info: CellWidthBounds): number {
//            return (info.firstColumnIndex << 16) + info.endColumnIndex;
//        }
//    }    
//}