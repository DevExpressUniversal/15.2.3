module __aspxRichEdit {
    export interface IHistoryItemStateObject {
        value: any;
        toJSON(): any;
        canMerge(obj: IHistoryItemStateObject);
        merge(obj: IHistoryItemStateObject);
    }

    export class HistoryItemIntervalStateObject implements IHistoryItemStateObject {
        value: any;
        interval: FixedInterval;
        constructor(interval: FixedInterval, value: any) {
            this.interval = interval.clone();
            this.value = value;
        }
        merge(object: HistoryItemIntervalStateObject) {
            this.interval.length += object.interval.length;
        }
        canMerge(object: HistoryItemIntervalStateObject): boolean {
            return this.interval.end() === object.interval.start && this.isEqualValue(object);
        }
        isEqualValue(object: HistoryItemIntervalStateObject): boolean {
            return this.value === object.value;
        }
        toJSON(): any[]{
            return [this.interval.start, this.interval.length, this.getPropertyValueForJSON(this.value)];
        }
        getPropertyValueForJSON(value: any): any {
            if(value instanceof FontInfo)
                return (<FontInfo>value).name;
            return value;
        }
    }

    export class HistoryItemTabStateObject extends HistoryItemIntervalStateObject {
        constructor(interval: FixedInterval, tabInfo: TabInfo) {
            super(interval, tabInfo);
        }
        isEqualValue(object: HistoryItemTabStateObject): boolean {
            return (<TabInfo>this.value).position == (<TabInfo>object.value).position;
        }
        getPropertyValueForJSON(value: any): any {
            return JSONTabConverter.convertToJSON(value);
        }
    }

    export class HistoryItemTextBufferStateObject extends HistoryItemIntervalStateObject {
        constructor(startPosition: number, text: string) {
            super(new FixedInterval(startPosition, text.length), text);
        }
        canMerge(stateValue: HistoryItemIntervalStateObject): boolean {
            return false;
        }
    }

    export class HistoryItemIntervalStyleStateObject extends HistoryItemIntervalStateObject {
        constructor(interval: FixedInterval, style: StyleBase) {
            super(interval, style);
        }
        getPropertyValueForJSON(value: any): any {
            return (<StyleBase>value).styleName;
        }
    }

    export class HistoryItemIntervalUseStateObject extends HistoryItemIntervalStateObject {
        use: boolean;
        constructor(interval: FixedInterval, value: any, use: boolean) {
            super(interval, value);
            this.use = use;
        }
        canMerge(stateValue: HistoryItemIntervalUseStateObject): boolean {
            return super.canMerge(stateValue) && this.use === stateValue.use;
        }
        toJSON(): any[] {
            return super.toJSON().concat([this.use ? 1 : 0]);
        }
    }

    export class HistoryItemSectionStateObject implements IHistoryItemStateObject {
        value: any;
        sectionIndex: number;

        constructor(sectionIndex: number, value: any) {
            this.value = value;
            this.sectionIndex = sectionIndex;
        }

        toJSON(): any[] {
            return [this.sectionIndex, this.value];
        }
        canMerge(obj: IHistoryItemStateObject) { return false; }
        merge(obj: IHistoryItemStateObject) { }
    }

    /* List Level */
    export class HistoryItemListLevelStateObject implements IHistoryItemStateObject {
        value: any;

        listLevelIndex: number;
        numberingListIndex: number;
        isAbstractNumberingList: boolean;

        constructor(isAbstractNumberingList: boolean, numberingListIndex: number, listLevelIndex: number, value: any) {
            this.isAbstractNumberingList = isAbstractNumberingList;
            this.numberingListIndex = numberingListIndex;
            this.listLevelIndex = listLevelIndex;
            this.value = value;
        }

        toJSON(): any {
            return [this.isAbstractNumberingList ? 1 : 0, this.numberingListIndex, this.listLevelIndex, this.getPropertyValueForJSON(this.value)];
        }
        canMerge(obj: IHistoryItemStateObject) {
            return false;
        }
        merge(obj: IHistoryItemStateObject) {
            throw new Error(Errors.NotImplemented);
        }
        getPropertyValueForJSON(value: any): any {
            if(value instanceof FontInfo)
                return (<FontInfo>value).name;
            return value;
        }
    }

    export class HistoryItemListLevelUseStateObject extends HistoryItemListLevelStateObject {
        use: boolean;
        constructor(isAbstractNumberingList: boolean, numberingListIndex: number, listLevelIndex: number, value: any, use: boolean) {
            super(isAbstractNumberingList, numberingListIndex, listLevelIndex, value);
            this.use = use;
        }

        toJSON(): any {
            return super.toJSON().concat([this.use ? 1 : 0]);
        }
    }

    /* Bookmarks */
    export class HistoryItemBookmarkStateObject implements IHistoryItemStateObject {
        value: any;
        name: string;
        start: number;
        end: number;
        deleted: boolean;

        constructor(name: string, start: number, end: number, deleted?: boolean) {
            this.name = name;
            this.start = start;
            this.end = end;
            this.deleted = deleted;
        }

        toJSON(): any {
            if (this.deleted)
                return [this.name];
            return [this.name, this.start, this.end];
        }
        canMerge(obj: IHistoryItemStateObject) {
            return false;
        }
        merge(obj: IHistoryItemStateObject) {
            throw new Error(Errors.NotImplemented);
        }
    }

    /* Table Properties */
    export class HistoryItemTableStateObject implements IHistoryItemStateObject {
        value: any;
        tableIndex: number;

        tableStartPosition: number;
        tableNestedLevel: number;

        constructor(tableStartPosition: number, tableNestedLevel: number, tableIndex: number, value: any) {
            this.tableIndex = tableIndex;
            this.value = value;
            this.tableStartPosition = tableStartPosition;
            this.tableNestedLevel = tableNestedLevel;
        }

        toJSON(): any {
            return [this.tableStartPosition, this.tableNestedLevel, this.getPropertyValueForJSON(this.value)];
        }
        canMerge(obj: IHistoryItemStateObject) {
            return false;
        }
        merge(obj: IHistoryItemStateObject) {
            throw new Error(Errors.NotImplemented);
        }
        getPropertyValueForJSON(value: any): any {
            if(value instanceof TableWidthUnit)
                return [(<TableWidthUnit>value).type, (<TableWidthUnit>value).value];
            if(value instanceof TableHeightUnit)
                return [(<TableHeightUnit>value).type, (<TableHeightUnit>value).value];
            if(value instanceof BorderInfo)
                return [(<BorderInfo>value).color, (<BorderInfo>value).frame, (<BorderInfo>value).offset, (<BorderInfo>value).shadow, (<BorderInfo>value).style, (<BorderInfo>value).width];
            return value;
        }
    }

    export class HistoryItemTableUseStateObject extends HistoryItemTableStateObject {
        use: boolean;
        constructor(tableStartPosition: number, tableNestedLevel: number, tableIndex: number, value: any, use: boolean) {
            super(tableStartPosition, tableNestedLevel, tableIndex, value);
            this.use = use;
        }
        toJSON(): any[] {
            return super.toJSON().concat([this.use ? 1 : 0]);
        }
    }

    export class HistoryItemTableComplexUseStateObject extends HistoryItemTableStateObject {
        uses: boolean[];
        constructor(tableStartPosition: number, tableNestedLevel: number, tableIndex: number, value: any[], uses: boolean[]) {
            super(tableStartPosition, tableNestedLevel, tableIndex, value);
            this.uses = uses;
        }
        toJSON(): any[] {
            let uses = [];
            let usesLength = this.uses.length;
            for(let i = 0; i < usesLength; i++)
                uses.push(this.uses[i] ? 1 : 0);
            return super.toJSON().concat([uses]);
        }
        getPropertyValueForJSON(value: any): any {
            let result = [];
            let length = value.length;
            for(let i = 0; i < length; i++)
                result.push(super.getPropertyValueForJSON(value[i]));
            return result;
        }
    }

    /* Table Cell Properties */
    export class HistoryItemTableCellStateObject implements IHistoryItemStateObject {
        value: any;

        tableIndex: number;
        rowIndex: number;
        cellIndex: number;
        tableStartPosition: number;
        tableNestedLevel: number;

        constructor(tableStartPosition: number, tableNestedLevel: number, tableIndex: number, rowIndex: number, cellIndex: number, value: any) {
            this.tableIndex = tableIndex;
            this.rowIndex = rowIndex;
            this.cellIndex = cellIndex;
            this.value = value;
            this.tableStartPosition = tableStartPosition;
            this.tableNestedLevel = tableNestedLevel;
        }

        toJSON(): any {
            return [this.tableStartPosition, this.tableNestedLevel, this.rowIndex, this.cellIndex, this.getPropertyValueForJSON(this.value)];
        }
        canMerge(obj: IHistoryItemStateObject) {
            return false;
        }
        merge(obj: IHistoryItemStateObject) {
            throw new Error(Errors.NotImplemented);
        }
        getPropertyValueForJSON(value: any): any {
            if(value instanceof TableWidthUnit)
                return [(<TableWidthUnit>value).type, (<TableWidthUnit>value).value];
            if(value instanceof TableHeightUnit)
                return [(<TableHeightUnit>value).type, (<TableHeightUnit>value).value];
            if(value instanceof BorderInfo)
                return [(<BorderInfo>value).color, (<BorderInfo>value).frame, (<BorderInfo>value).offset, (<BorderInfo>value).shadow, (<BorderInfo>value).style, (<BorderInfo>value).width];
            return value;
        }
    }
    export class HistoryItemTableCellUseStateObject extends HistoryItemTableCellStateObject {
        use: boolean;
        constructor(tableStartPosition: number, tableNestedLevel: number, tableIndex: number, rowIndex: number, cellIndex: number, value: any, use: boolean) {
            super(tableStartPosition, tableNestedLevel, tableIndex, rowIndex, cellIndex, value);
            this.use = use;
        }
        toJSON(): any[] {
            return super.toJSON().concat([this.use ? 1 : 0]);
        }
    }
    export class HistoryItemTableCellComplexUseStateObject extends HistoryItemTableCellStateObject {
        uses: boolean[];
        constructor(tableStartPosition: number, tableNestedLevel: number, tableIndex: number, rowIndex: number, cellIndex: number, value: any[], uses: boolean[]) {
            super(tableStartPosition, tableNestedLevel, tableIndex, rowIndex, cellIndex, value);
            this.uses = uses;
        }
        toJSON(): any[] {
            let uses = [];
            let usesLength = this.uses.length;
            for(let i = 0; i < usesLength; i++) {
                uses.push(this.uses[i] ? 1 : 0);
            }
            return super.toJSON().concat([uses]);
        }
        getPropertyValueForJSON(value: any): any {
            let result = [];
            let length = value.length;
            for(let i = 0; i < length; i++)
                result.push(super.getPropertyValueForJSON(value[i]));
            return result;
        }
    }
    export class HistoryItemTableRowStateObject implements IHistoryItemStateObject {
        value: any;

        tableIndex: number;
        rowIndex: number;
        tableStartPosition: number;
        tableNestedLevel: number;

        constructor(tableStartPosition: number, tableNestedLevel: number, tableIndex: number, rowIndex: number, value: any) {
            this.tableIndex = tableIndex;
            this.rowIndex = rowIndex;
            this.value = value;
            this.tableStartPosition = tableStartPosition;
            this.tableNestedLevel = tableNestedLevel;
        }

        toJSON(): any {
            return [this.tableStartPosition, this.tableNestedLevel, this.rowIndex, this.getPropertyValueForJSON(this.value)];
        }
        canMerge(obj: IHistoryItemStateObject) {
            return false;
        }
        merge(obj: IHistoryItemStateObject) {
            throw new Error(Errors.NotImplemented);
        }
        getPropertyValueForJSON(value: any): any {
            if(value instanceof TableWidthUnit)
                return [(<TableWidthUnit>value).type, (<TableWidthUnit>value).value];
            if(value instanceof TableHeightUnit)
                return [(<TableHeightUnit>value).type, (<TableHeightUnit>value).value];
            return value;
        }
    } 
    export class HistoryItemTableRowUseStateObject extends HistoryItemTableRowStateObject {
        use: boolean;
        constructor(tableStartPosition: number, tableNestedLevel: number, tableIndex: number, rowIndex: number, value: any, use: boolean) {
            super(tableStartPosition, tableNestedLevel, tableIndex, rowIndex, value);
            this.use = use;
        }
        toJSON(): any[] {
            return super.toJSON().concat([this.use ? 1 : 0]);
        }
    }
} 