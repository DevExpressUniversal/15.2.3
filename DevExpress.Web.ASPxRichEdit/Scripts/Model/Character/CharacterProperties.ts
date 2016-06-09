module __aspxRichEdit {
    export enum CharacterFormattingScript {
        Normal = 0,
        Subscript = 1,
        Superscript = 2,
    }
    export enum CharacterPropertiesMask {
        UseNone = 0x00000000,
        UseFontName = 0x00000001,
        UseDoubleFontSize = 0x00000002,
        UseFontBold = 0x00000004,
        UseFontItalic = 0x00000008,
        UseFontStrikeoutType = 0x00000010,
        UseFontUnderlineType = 0x00000020,
        UseAllCaps = 0x00000040,
        UseForeColor = 0x00000080,
        UseBackColor = 0x00000100,
        UseUnderlineColor = 0x00000200,
        UseStrikeoutColor = 0x00000400,
        UseUnderlineWordsOnly = 0x00000800,
        UseStrikeoutWordsOnly = 0x00001000,
        UseScript = 0x00002000,
        UseHidden = 0x00004000,
        UseLangInfo = 0x00008000,
        UseNoProof = 0x00010000,
        UseFontInfo = 0x00020000,
        UseAll = 0x7FFFFFFF
    }
    export enum StrikeoutType {
        None = 0,
        Single = 1,
        Double = 2,
    }
    export enum UnderlineType {
        None = 0,
        Single = 1,
        Dotted = 2,
        Dashed = 3,
        DashDotted = 4,
        DashDotDotted = 5,
        Double = 6,
        HeavyWave = 7,
        LongDashed = 8,
        ThickSingle = 9,
        ThickDotted = 10,
        ThickDashed = 11,
        ThickDashDotted = 12,
        ThickDashDotDotted = 13,
        ThickLongDashed = 14,
        DoubleWave = 15,
        Wave = 16,
        DashSmallGap = 17,
    }

    export class CharacterProperties implements IEquatable<CharacterProperties>, ICloneable<CharacterProperties> {
        // IMPORTANT, when add new property, add it in PropertiesWhatNeedSetWhenCreateHyperlinkField

        static fieldsInfoToCompare: InputPositionCompareInfo[] = [
            new InputPositionCompareInfo("fontSize", Utils.exactlyEqual, 11), 
            new InputPositionCompareInfo("fontBold", Utils.exactlyEqual, false),
            new InputPositionCompareInfo("fontItalic", Utils.exactlyEqual, false),
            new InputPositionCompareInfo("fontInfo", FontInfo.equalsBinary, undefined),
            new InputPositionCompareInfo("script", Utils.exactlyEqual, CharacterFormattingScript.Normal),
            new InputPositionCompareInfo("fontStrikeoutType", Utils.exactlyEqual, StrikeoutType.None),
            new InputPositionCompareInfo("fontUnderlineType", Utils.exactlyEqual, UnderlineType.None),
            new InputPositionCompareInfo("allCaps", Utils.exactlyEqual, false),
            new InputPositionCompareInfo("underlineWordsOnly", Utils.exactlyEqual, false),
            new InputPositionCompareInfo("strikeoutWordsOnly", Utils.exactlyEqual, false),
            new InputPositionCompareInfo("noProof", Utils.exactlyEqual, false),
            new InputPositionCompareInfo("hidden", Utils.exactlyEqual, false),
            new InputPositionCompareInfo("foreColor", Utils.exactlyEqual, 0),
            new InputPositionCompareInfo("backColor", Utils.exactlyEqual, 0),
            new InputPositionCompareInfo("strikeoutColor", Utils.exactlyEqual, 0),
            new InputPositionCompareInfo("underlineColor", Utils.exactlyEqual, 0),
        ];

        public fontSize: number = 11; //todo: to pixels
        public fontBold: boolean = false;
        public fontItalic: boolean = false;
        public fontInfo: FontInfo;
        public script: CharacterFormattingScript = CharacterFormattingScript.Normal;
        public fontStrikeoutType: StrikeoutType = StrikeoutType.None;
        public fontUnderlineType: UnderlineType = UnderlineType.None;
        public allCaps: boolean = false;
        public underlineWordsOnly = false;
        public strikeoutWordsOnly = false;
        public noProof: boolean = false;
        public hidden: boolean = false;
        public foreColor: number = 0;
        public backColor: number = 0;
        public strikeoutColor: number = 0;
        public underlineColor: number = 0;

        public equals(obj: CharacterProperties): boolean {
            if (!obj)
                return false;
            return this.fontBold == obj.fontBold &&
                this.fontItalic == obj.fontItalic &&
                (this.fontInfo && obj.fontInfo && this.fontInfo.equals(obj.fontInfo)) &&
                this.fontSize == obj.fontSize &&
                this.script == obj.script &&
                this.fontStrikeoutType == obj.fontStrikeoutType &&
                this.fontUnderlineType == obj.fontUnderlineType &&
                this.allCaps == obj.allCaps &&
                this.underlineWordsOnly == obj.underlineWordsOnly &&
                this.strikeoutWordsOnly == obj.strikeoutWordsOnly &&
                this.noProof == obj.noProof &&
                this.hidden == obj.hidden &&
                this.foreColor == obj.foreColor &&
                this.backColor == obj.backColor &&
                this.strikeoutColor == obj.strikeoutColor &&
                this.underlineColor == obj.underlineColor;
        }
        public clone(): CharacterProperties {
            var result = new CharacterProperties();
            result.copyFrom(this);
            return result;
        }
        public copyFrom(obj: any) {
            this.fontInfo = obj.fontInfo || null;
            this.fontSize = obj.fontSize;
            this.fontBold = obj.fontBold;
            this.fontItalic = obj.fontItalic;
            this.script = obj.script;
            this.fontStrikeoutType = obj.fontStrikeoutType;
            this.fontUnderlineType = obj.fontUnderlineType;
            this.allCaps = obj.allCaps;
            this.underlineWordsOnly = obj.underlineWordsOnly;
            this.strikeoutWordsOnly = obj.strikeoutWordsOnly;
            this.noProof = obj.noProof;
            this.hidden = obj.hidden;
            this.foreColor = obj.foreColor;
            this.backColor = obj.backColor;
            this.strikeoutColor = obj.strikeoutColor;
            this.underlineColor = obj.underlineColor;
        }
    }
    export class MaskedCharacterProperties extends CharacterProperties implements IMaskedProperties<CharacterPropertiesMask> {
        useValue: CharacterPropertiesMask = CharacterPropertiesMask.UseNone;

        public getUseValue(value: CharacterPropertiesMask): boolean {
            return (this.useValue & value) != 0;
        }
        public setUseValue(mask: CharacterPropertiesMask, value: boolean) {
            if (value)
                this.useValue |= mask;
            else
                this.useValue &= ~mask;
        }
        public clone(): MaskedCharacterProperties {
            var result = new MaskedCharacterProperties();
            result.copyFrom(this);
            return result;
        }
        public copyFrom(obj: any) {
            CharacterProperties.prototype.copyFrom.call(this, obj);
            this.useValue = obj.useValue;
        }
        public equals(obj: MaskedCharacterProperties): boolean {
            return CharacterProperties.prototype.equals.call(this, obj)
                && this.useValue == obj.useValue;
        }
    }
} 