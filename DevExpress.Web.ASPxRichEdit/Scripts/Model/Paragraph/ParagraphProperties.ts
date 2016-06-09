module __aspxRichEdit {
    export enum ParagraphPropertiesMask {
        UseNone = 0x00000000,
        UseAlignment = 0x00000001,
        UseLeftIndent = 0x00000002,
        UseRightIndent = 0x00000004,
        UseSpacingBefore = 0x00000008,
        UseSpacingAfter = 0x00000010,
        UseLineSpacing = 0x00000020,
        UseFirstLineIndent = 0x00000040,
        UseSuppressHyphenation = 0x00000080,
        UseSuppressLineNumbers = 0x00000100,
        UseContextualSpacing = 0x00000200,
        UsePageBreakBefore = 0x00000400,
        UseBeforeAutoSpacing = 0x00000800,
        UseAfterAutoSpacing = 0x00001000,
        UseKeepWithNext = 0x00002000,
        UseKeepLinesTogether = 0x00004000,
        UseWidowOrphanControl = 0x00008000,
        UseOutlineLevel = 0x00010000,
        UseBackColor = 0x00020000,
        UseLeftBorder = 0x00040000,
        UseRightBorder = 0x00080000,
        UseTopBorder = 0x00100000,
        UseBottomBorder = 0x00200000,
        UseBorders = 0x003C0000,
        UseAll = 0x7FFFFFFF
    }

    export enum ParagraphAlignment {
        Left = 0,
        Right = 1,
        Center = 2,
        Justify = 3,
        Unspecified = 4
    }

    export enum ParagraphLineSpacingType {
        Single = 0,
        Sesquialteral = 1,
        Double = 2,
        Multiple = 3,
        Exactly = 4,
        AtLeast = 5
    }
    export enum ParagraphFirstLineIndent {
        None = 0,
        Indented = 1,
        Hanging = 2
    }

    export class ParagraphProperties implements IEquatable<ParagraphProperties>, ICloneable<ParagraphProperties> {
        static fieldsInfoToCompare: InputPositionCompareInfo[] = [
            new InputPositionCompareInfo("firstLineIndent", Utils.exactlyEqual, 0),
            new InputPositionCompareInfo("widowOrphanControl", Utils.exactlyEqual, true),
            new InputPositionCompareInfo("firstLineIndentType", Utils.exactlyEqual, ParagraphFirstLineIndent.None),
            new InputPositionCompareInfo("afterAutoSpacing", Utils.exactlyEqual, false),
            new InputPositionCompareInfo("outlineLevel", Utils.exactlyEqual, 0),
            new InputPositionCompareInfo("beforeAutoSpacing", Utils.exactlyEqual, false),
            new InputPositionCompareInfo("pageBreakBefore", Utils.exactlyEqual, false),
            new InputPositionCompareInfo("rightIndent", Utils.exactlyEqual, 0),
            new InputPositionCompareInfo("suppressHyphenation", Utils.exactlyEqual, false),
            new InputPositionCompareInfo("lineSpacing", Utils.exactlyEqual, 0),
            new InputPositionCompareInfo("suppressLineNumbers", Utils.exactlyEqual, false),
            new InputPositionCompareInfo("keepLinesTogether", Utils.exactlyEqual, false),
            new InputPositionCompareInfo("keepWithNext", Utils.exactlyEqual, false),
            new InputPositionCompareInfo("backColor", Utils.exactlyEqual, 0),
            new InputPositionCompareInfo("leftIndent", Utils.exactlyEqual, 0),
            new InputPositionCompareInfo("lineSpacingType", Utils.exactlyEqual, ParagraphLineSpacingType.Single),
            new InputPositionCompareInfo("alignment", Utils.exactlyEqual, ParagraphAlignment.Left),
            new InputPositionCompareInfo("contextualSpacing", Utils.exactlyEqual, false),
            new InputPositionCompareInfo("spacingBefore", Utils.exactlyEqual, 0),
            new InputPositionCompareInfo("spacingAfter", Utils.exactlyEqual, 0),
            new InputPositionCompareInfo("leftBorder", BorderInfo.equalsBinary, new BorderInfo()), // IMPORTANT!!. All default created paragraphs get ONE object. Do clone!
            new InputPositionCompareInfo("rightBorder", BorderInfo.equalsBinary, new BorderInfo()),
            new InputPositionCompareInfo("topBorder", BorderInfo.equalsBinary, new BorderInfo()),
            new InputPositionCompareInfo("bottomBorder", BorderInfo.equalsBinary, new BorderInfo()),            
        ];

        firstLineIndent: number = 0;
        widowOrphanControl: boolean = true; // Do not leave the first line of the paragraph in the previous column. Migrate to next
        firstLineIndentType: ParagraphFirstLineIndent = ParagraphFirstLineIndent.None;
        afterAutoSpacing: boolean = false;
        outlineLevel: number = 0;
        beforeAutoSpacing: boolean = false;
        pageBreakBefore: boolean = false;
        rightIndent: number = 0;
        suppressHyphenation: boolean = false;
        lineSpacing: number = 0;
        suppressLineNumbers: boolean = false;
        keepLinesTogether: boolean = false;
        keepWithNext: boolean = false;
        backColor: number = 0;
        // distance from left ident column to start SECOND row in paragraph.
        // It mean that firstLineIndentType == None && firstLineIndent == 10 && leftIndent == 20  =>  distance(left ident column, start first row) == 20
        //                                                                                        =>  distance(left ident column, start second row) == 20
        //              firstLineIndentType == Indented && firstLineIndent == 10 && leftIndent == 20  =>  distance(left ident column, start first row) == 30
        //                                                                                            =>  distance(left ident column, start second row) == 20
        //              firstLineIndentType == Hanging && firstLineIndent == 10 && leftIndent == 20   =>  distance(left ident column, start first row) == 10
        //                                                                                            =>  distance(left ident column, start second row) == 20
        leftIndent: number = 0; 

        lineSpacingType: ParagraphLineSpacingType = ParagraphLineSpacingType.Single;
        alignment: ParagraphAlignment = ParagraphAlignment.Left;
        contextualSpacing: boolean = false;
        spacingBefore: number = 0;
        spacingAfter: number = 0;
        leftBorder: BorderInfo = new BorderInfo();
        rightBorder: BorderInfo = new BorderInfo();
        topBorder: BorderInfo = new BorderInfo();
        bottomBorder: BorderInfo = new BorderInfo();

        public copyFrom(obj: any) {
            this.alignment = obj.alignment;
            this.leftIndent = obj.leftIndent;
            this.rightIndent = obj.rightIndent;
            this.spacingBefore = obj.spacingBefore;
            this.spacingAfter = obj.spacingAfter;
            this.lineSpacing = obj.lineSpacing;
            this.firstLineIndent = obj.firstLineIndent;
            this.suppressHyphenation = obj.suppressHyphenation;
            this.suppressLineNumbers = obj.suppressLineNumbers;
            this.contextualSpacing = obj.contextualSpacing;
            this.pageBreakBefore = obj.pageBreakBefore;
            this.beforeAutoSpacing = obj.beforeAutoSpacing;
            this.afterAutoSpacing = obj.afterAutoSpacing;
            this.keepWithNext = obj.keepWithNext;
            this.keepLinesTogether = obj.keepLinesTogether;
            this.widowOrphanControl = obj.widowOrphanControl;
            this.outlineLevel = obj.outlineLevel;
            this.firstLineIndentType = obj.firstLineIndentType;
            this.backColor = obj.backColor;
            this.lineSpacingType = obj.lineSpacingType;
            if (obj.leftBorder)
                this.leftBorder.copyFrom(obj.leftBorder);
            else
                this.leftBorder = obj.leftBorder;
            if (obj.rightBorder)
                this.rightBorder.copyFrom(obj.rightBorder);
            else
                this.rightBorder = obj.rightBorder;
            if (obj.topBorder)
                this.topBorder.copyFrom(obj.topBorder);
            else
                this.topBorder = obj.topBorder;
            if (obj.bottomBorder)
                this.bottomBorder.copyFrom(obj.bottomBorder);
            else
                this.bottomBorder = obj.bottomBorder;
        }

        public clone(): ParagraphProperties {
            var result = new ParagraphProperties();
            result.copyFrom(this);
            return result;
        }

        public equals(obj: ParagraphProperties): boolean {
            if (!obj)
                return false;
            return this.firstLineIndent == obj.firstLineIndent &&
                this.firstLineIndentType == obj.firstLineIndentType &&
                this.afterAutoSpacing == obj.afterAutoSpacing &&
                this.outlineLevel == obj.outlineLevel &&
                this.beforeAutoSpacing == obj.beforeAutoSpacing &&
                this.pageBreakBefore == obj.pageBreakBefore &&
                this.rightIndent == obj.rightIndent &&
                this.suppressHyphenation == obj.suppressHyphenation &&
                this.lineSpacing == obj.lineSpacing &&
                this.suppressLineNumbers == obj.suppressLineNumbers &&
                this.keepLinesTogether == obj.keepLinesTogether &&
                this.keepWithNext == obj.keepWithNext &&
                this.backColor == obj.backColor &&
                this.leftIndent == obj.leftIndent &&
                this.lineSpacingType == obj.lineSpacingType &&
                this.alignment == obj.alignment &&
                this.contextualSpacing == obj.contextualSpacing &&
                this.spacingBefore == obj.spacingBefore &&
                this.spacingAfter == obj.spacingAfter &&
                this.leftBorder.equals(obj.leftBorder) &&
                this.rightBorder.equals(obj.rightBorder) &&
                this.topBorder.equals(obj.topBorder) &&
                this.bottomBorder.equals(obj.bottomBorder);
        }

        public getLeftIndentForFirstRow() {
            let indent: number = this.leftIndent;
            switch (this.firstLineIndentType) {
                case ParagraphFirstLineIndent.None:
                case ParagraphFirstLineIndent.Indented:
                    indent += this.firstLineIndent;
                    break;
                case ParagraphFirstLineIndent.Hanging:
                    indent -= this.firstLineIndent;
            }
            return indent;
        }

        public getLeftIndentForOtherRow() {
            return this.leftIndent;
        }

        public getLeftIndentForParagraphFrame(unitConverter: IUnitConverter) {
            let indent: number = this.leftIndent;
            if (this.firstLineIndentType == ParagraphFirstLineIndent.Hanging)
                indent -= this.firstLineIndent;
            return unitConverter.toPixels(indent);
        }
    }

    export class MaskedParagraphProperties extends ParagraphProperties implements IMaskedProperties<ParagraphPropertiesMask> {
        useValue: ParagraphPropertiesMask;
        public getUseValue(value: ParagraphPropertiesMask): boolean {
            return (this.useValue & value) != 0;
        }
        public setUseValue(mask: ParagraphPropertiesMask, value: boolean) {
            if (value)
                this.useValue |= mask;
            else
                this.useValue &= ~mask;
        }
        public copyFrom(obj: any) {
            ParagraphProperties.prototype.copyFrom.call(this, obj);
            this.useValue = obj.useValue;
        }
        public equals(obj: MaskedParagraphProperties): boolean {
            return ParagraphProperties.prototype.equals.call(this, obj)
                && this.useValue == obj.useValue;
        }
        public clone(): MaskedParagraphProperties {
            var result = new MaskedParagraphProperties();
            result.copyFrom(this);
            return result;
        }
    }
} 