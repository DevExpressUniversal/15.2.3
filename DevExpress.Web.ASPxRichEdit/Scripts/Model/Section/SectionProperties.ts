module __aspxRichEdit {
    export enum SectionStartType {
        NextPage = 0,
        OddPage = 1,
        EvenPage = 2,
        Continuous = 3,
        Column = 4
    }
    export enum HeaderFooterType {
        First = 0,
        Odd = 1,
        Even = 2,
    }
    export class SectionProperties implements ICloneable<SectionProperties>, ISupportCopyFrom<SectionProperties> {
        static fieldsInfoToCompare: InputPositionCompareInfo[] = [
            new InputPositionCompareInfo("marginLeft", Utils.exactlyEqual, 1440),
            new InputPositionCompareInfo("marginTop", Utils.exactlyEqual, 1440),
            new InputPositionCompareInfo("marginRight", Utils.exactlyEqual, 1440),
            new InputPositionCompareInfo("marginBottom", Utils.exactlyEqual, 1440),
            new InputPositionCompareInfo("columnCount", Utils.exactlyEqual, 1),
            new InputPositionCompareInfo("space", Utils.exactlyEqual, 720),
            new InputPositionCompareInfo("equalWidthColumns", Utils.exactlyEqual, true),
            new InputPositionCompareInfo("columnsInfo", SectionProperties.equalsColumnsInfoBinary, []),
            new InputPositionCompareInfo("pageWidth", Utils.exactlyEqual, 11906),
            new InputPositionCompareInfo("pageHeight", Utils.exactlyEqual, 16838),
            new InputPositionCompareInfo("startType", Utils.exactlyEqual, SectionStartType.NextPage),
            new InputPositionCompareInfo("landscape", Utils.exactlyEqual, false),
            new InputPositionCompareInfo("differentFirstPage", Utils.exactlyEqual, false),
            new InputPositionCompareInfo("headerOffset", Utils.exactlyEqual, 720),
            new InputPositionCompareInfo("footerOffset", Utils.exactlyEqual, 720)
        ];

        marginLeft: number = 1440;
        marginTop: number = 1440;
        marginRight: number = 1440;
        marginBottom: number = 1440;
        headerOffset: number = 720;
        footerOffset: number = 720;
        columnCount: number = 1;
        space: number = 720;  // between columns (in case equalWidthColumns==true)
        equalWidthColumns: boolean = true;
        columnsInfo: SectionColumnProperties[] = [];
        pageWidth: number = 11906;//twips
        pageHeight: number = 16838;//twips
        startType: SectionStartType = SectionStartType.NextPage;
        landscape: boolean = false;
        differentFirstPage: boolean = false;

        public copyFrom(obj: SectionProperties) {
            this.columnCount = obj.columnCount;
            
            if (obj.columnsInfo) {
                this.columnsInfo = [];
                for (var i = 0; i < obj.columnsInfo.length; i++)
                    this.columnsInfo.push(new SectionColumnProperties(obj.columnsInfo[i].width, obj.columnsInfo[i].space));
            }
            else
                this.columnsInfo = obj.columnsInfo;
            this.equalWidthColumns = obj.equalWidthColumns;
            this.marginBottom = obj.marginBottom;
            this.marginLeft = obj.marginLeft;
            this.marginRight = obj.marginRight;
            this.marginTop = obj.marginTop;
            this.pageHeight = obj.pageHeight;
            this.pageWidth = obj.pageWidth;
            this.space = obj.space;
            this.startType = obj.startType;
            this.landscape = obj.landscape;
            this.differentFirstPage = obj.differentFirstPage;
            this.headerOffset = obj.headerOffset;
            this.footerOffset = obj.footerOffset;
        }

        public clone(): SectionProperties {
            var obj = new SectionProperties();
            obj.copyFrom(this);
            return obj;
        }

        public equals(obj: SectionProperties): boolean {
            if (!obj)
                return false;
            return this.marginLeft == obj.marginLeft &&
                this.marginTop == obj.marginTop &&
                this.marginRight == obj.marginRight &&
                this.marginBottom == obj.marginBottom &&
                this.headerOffset == obj.headerOffset &&
                this.footerOffset == obj.footerOffset &&
                this.columnCount == obj.columnCount &&
                this.space == obj.space &&
                this.equalWidthColumns == obj.equalWidthColumns &&
                this.pageWidth == obj.pageWidth &&
                this.pageHeight == obj.pageHeight &&
                this.startType == obj.startType &&
                this.landscape == obj.landscape &&
                this.differentFirstPage == obj.differentFirstPage &&
                SectionProperties.equalsColumnsInfoBinary(this.columnsInfo, obj.columnsInfo);
        }

        public static equalsColumnsInfoBinary(columnsInfoA: SectionColumnProperties[], columnsInfoB: SectionColumnProperties[]) {
            if (!columnsInfoA || !columnsInfoB || columnsInfoA.length != columnsInfoB.length)
                return false;
            for (var i: number = 0; i < columnsInfoA.length; i++)
                if (!columnsInfoA[i].equals(columnsInfoB[i]))
                    return false;
            return true;
        }
    }

    export class SectionColumnProperties implements ICloneable< SectionColumnProperties>, ISupportCopyFrom<SectionColumnProperties> {
        width: number = 0;
        space: number = 0;

        constructor(width: number, space: number) {
            this.width = width;
            this.space = space;
        }

        public equals(obj: SectionColumnProperties): boolean {
            if(!obj)
                return false;
            return this.width == obj.width &&
                this.space == obj.space;
        }

        public copyFrom(obj: SectionColumnProperties) {
            this.width = obj.width;
            this.space = obj.space;
        }

        public clone(): SectionColumnProperties {
            return new SectionColumnProperties(this.width, this.space);
        }

        public applyConverter(unitConverter: IUnitConverter) {
            this.width = unitConverter.toPixels(this.width);
            this.space = unitConverter.toPixels(this.space);
        }
    }
} 