module __aspxRichEdit {
    export enum LogSource {
        Main = 1,
        LayoutFormatter,
        LayoutFormatterNotifier,
        LayoutFormatterInvalidator,
        CanvasManager,
        DocumentRenderer,
    }

    export class LogSourceParams {
        private name: string;
        private logSource: LogSource;
        public enabled: boolean;

        constructor(enabled: boolean, logSource: LogSource) {
            this.enabled = enabled;
            this.logSource = logSource;
            this.name = LogSource[logSource];
        }

        public getName() {
            return this.name;
        }

        public isEnabled(functionName: string) {
            return this.enabled;
        }
    }

    export type LogSourceParamsMap = { [sourceId: number]: LogSourceParams };

    export class Log {
        public static splitBy: string = ", ";
        public static SOLID_BLOCK = String.fromCharCode(0x2588);
        public static DOUBLE_SOLID_BLOCK = Log.SOLID_BLOCK + Log.SOLID_BLOCK;
        public static TRIPLE_SOLID_BLOCK = Log.DOUBLE_SOLID_BLOCK + Log.SOLID_BLOCK;

        public static logSourceParams: LogSourceParamsMap = Log.getMap();
        public static isEnabled: boolean = false;

        private static getMap(): LogSourceParamsMap {
            const map: LogSourceParamsMap = {};
            for (let key in LogSource) {
                const keyNum: number = parseInt(key);
                if (!isNaN(keyNum))
                    map[keyNum] = new LogSourceParams(false, keyNum);
            }
            return map;
        }

        static print(logSource: LogSource, functionName: string, ...params: any[][]) {
            if (Log.isEnabled)
                return;

            const logSourceParams: LogSourceParams = Log.logSourceParams[logSource];
            if (logSourceParams) {
                if (logSourceParams.isEnabled(functionName)) {
                    const result: string[] = [];
                    for (let par of params) {
                        const paramName: string = par[0];
                        const paramValue: any = par[1];
                        result.push(`${paramName}: ${typeof paramValue == "function" ? paramValue() : paramValue}`);
                    }
                    console.log(`${Log.DOUBLE_SOLID_BLOCK} ${logSourceParams.getName() }. ${functionName}. ${result.join(Log.splitBy) }`);
                }
            }
            else
                console.log(`Log.print ${LogSource[logSource]} not defined`);
        }
        
        public static w0(func): any {
            return () => func.apply(func, []);
        }

        public static w1(func): any {
            return (p) => {
                return func.apply(func, [p]);
            }
        }

        public static w2(func): any {
            return (p1) => {
                return (p2) => {
                    return func.apply(func, [p1, p2]);
                }
            }
        }

        public static w3(func): any {
            return (p1) => {
                return (p2) => {
                    return (p3) => {
                        return func.apply(func, [p1, p2, p3]);
                    }
                }
            }
        }

        public static join<T>(sep: string, list: T[]): string {
            return list.join(sep);
        }

        public static map<TInp, TTes>(func: (p: TInp, index: number) => TTes, list: TInp[]): TTes[] {
            const result: TTes[] = [];
            for (let elemIndex: number = 0, elem: TInp; elem = elem = list[elemIndex]; elemIndex++)
                result.push(func(elem, elemIndex));
            return result;
        }
        
        public static mask(objEnum: any, mask: number): string {
            let res: string[] = [];
            for (let key in objEnum) {
                if (!objEnum.hasOwnProperty(key))
                    continue;
                const keyNum: number = parseInt(key);
                if (!isNaN(keyNum) && (keyNum & mask) == keyNum)
                    res.push(objEnum[key]);
            }
            return res.join(Log.splitBy);
        }
    }

    export class LogObjToStr {
        public static layoutChangeBase(obj: LayoutChangeBase<any>): string {
            return `type: ${LayoutChangeType[obj.changeType]}, index: ${obj.index}`;
        }

        public static canvasPage(page: LayoutPage): string {
            return `isComplete: ${page.isNeedFullyRender}, index: ${page.index}`;
        }

        static layoutPosition(lp: LayoutPosition): string {
            return LogObjToStr.layoutPositionShort(lp) + `, boxInd: ${lp.boxIndex}, charOffset:${lp.charOffset}`;
        }

        static layoutPositionShort(lp: LayoutPosition): string {
            return `DetLevel: ${DocumentLayoutDetailsLevel[lp.detailsLevel]}, pageIndex: ${lp.pageIndex }, pageAreaInd: ${lp.pageAreaIndex }, columnInd: ${lp.columnIndex }, rowInd: ${lp.rowIndex }`;
        }

        static fixedInterval(interval: FixedInterval): string {
            return `[${interval.start}, ${interval.end() }]`;
        }

        //static pageChange(pageChange: PageChange): string {
        //    return `type: ${LayoutChangeType[pageChange.changeType]}, pageIndex: ${pageChange.index}, ${Log.SOLID_BLOCK} pageAreaChanges: ${
        //        LogObjToStr.list(pageChange.pageAreaChanges, LogObjToStr.pageAreaChange, 1)
        //        }`;
        //}

        //static pageAreaChange(pageAreaChange: PageAreaChange): string {
        //    return `${LogObjToStr.layoutChangeBase(pageAreaChange) }, ${Log.SOLID_BLOCK} columnChanges: ${LogObjToStr.list(pageAreaChange.columnChanges, LogObjToStr.columnChange, 1) }`;
        //}

        //static columnChange(columnChange: ColumnChange): string {
        //    return `${LogObjToStr.layoutChangeBase(columnChange) }, ${Log.SOLID_BLOCK} rowChanges: ${
        //        LogObjToStr.list(columnChange.rowChanges, LogObjToStr.rowChange, 1) }, ${Log.SOLID_BLOCK} paragraphFrameChanges: ${
        //        LogObjToStr.list(columnChange.paragraphFrameChanges, LogObjToStr.paragraphFrameChange, 1) }, ${Log.SOLID_BLOCK} tableChanges: ${
        //        LogObjToStr.list(columnChange.tableChanges, LogObjToStr.tableChange, 1) }`;
        //}

        static rowChange(rowChange: RowChange): string {
            return `${LogObjToStr.layoutChangeBase(rowChange) }`;
        }

        static paragraphFrameChange(paragraphFrameChange: ParagraphFrameChange): string {
            return `${LogObjToStr.layoutChangeBase(paragraphFrameChange) }, paragraphColor: ${paragraphFrameChange.layoutElement.paragraphColor}`;
        }

        static tableChange(tableChange: TableChange): string {
            return `${LogObjToStr.layoutChangeBase(tableChange) }`;
        }

        //static test1(): any {
        //    const list: number[][] = [[1, 2, 3],
        //        [4, 5, 6],
        //        [7, 8, 9],
        //    ];

        //    let add = TEST.wrap1(TEST.addOne);
        //    let map = TEST.wrap2(TEST.map);
        //    let join = TEST.wrap2(TEST.join);
        //    let f = (r) => join("-")(map(add)(r));
        //    return join("|")(map(f)(list));

        //    return (list) => TEST.wrap2(TEST.join)("|")(TEST.wrap2(TEST.map)((innerList) => TEST.wrap2(TEST.join)("-")(TEST.wrap2(TEST.map)(TEST.wrap1(TEST.addOne))(innerList)))(list));
        //}
    }

    export class LogListHelper {
        private static wMap = Log.w2(Log.map);
        private static wJoin = Log.w2(Log.join);
        private static addSuffix = Log.w3((handler, val, index) => `[${index}]=${handler(val) }`);

        private static addIndex(index: number, str: string): string {
            return `[${index}]${str}`;
        }

        public static level_1<T>(handler: (p: T) => string, list: T[]): string {
            return LogListHelper.wJoin(Log.splitBy)(LogListHelper.wMap(LogListHelper.addSuffix(handler))(list));
        }

        public static level_2<T>(handler: (p: T) => string, list: T[][]): string {
            let levelOneHandler = Log.w2(LogListHelper.level_1)(handler);
            return LogListHelper.wJoin(Log.splitBy)(LogListHelper.wMap(LogListHelper.addSuffix(levelOneHandler))(list));
        }

        public static customLevel_2<T1, T2>(handlerLevel_1: (p: T1) => string): string {
            //let levelOneHandler = Log.w2(LogListHelper.level_1)(handler);
            //return LogListHelper.wJoin(Log.splitBy)(LogListHelper.wMap(levelOneHandler)(list));
            return "";
        }
    }
}