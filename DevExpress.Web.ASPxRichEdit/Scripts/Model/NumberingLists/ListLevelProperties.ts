module __aspxRichEdit {
    export class ListLevelProperties implements IIndexedCacheType<ListLevelProperties>, ICloneable<ListLevelProperties> {
        start: number = 1;
        format: NumberingFormat = NumberingFormat.Decimal;
        alignment: ListNumberAlignment = ListNumberAlignment.Left;
        convertPreviousLevelNumberingToDecimal: boolean = false;
        separator: string = Utils.specialCharacters.TabMark;
        suppressRestart: boolean = false;
        suppressBulletResize: boolean = false;
        displayFormatString: string = "{0}.";
        relativeRestartLevel: number = 0;
        templateCode: number = 0;
        originalLeftIndent: number = 0;
        legacy: boolean = false;
        legacySpace: number = 0;
        legacyIndent: number = 0;

        equals(obj: ListLevelProperties): boolean {
            return this.alignment === obj.alignment &&
                this.convertPreviousLevelNumberingToDecimal === obj.convertPreviousLevelNumberingToDecimal &&
                this.displayFormatString === obj.displayFormatString &&
                this.format === obj.format &&
                this.legacy === obj.legacy &&
                this.legacyIndent === obj.legacyIndent &&
                this.legacySpace === obj.legacySpace &&
                this.originalLeftIndent === obj.originalLeftIndent &&
                this.relativeRestartLevel === obj.relativeRestartLevel &&
                this.separator === obj.separator &&
                this.start === obj.start &&
                this.suppressBulletResize === obj.suppressBulletResize &&
                this.suppressRestart === obj.suppressRestart &&
                this.templateCode === obj.templateCode;
        }

        copyFrom(obj: any) {
            this.alignment = obj.alignment;
            this.convertPreviousLevelNumberingToDecimal = obj.convertPreviousLevelNumberingToDecimal;
            this.displayFormatString = obj.displayFormatString;
            this.format = obj.format;
            this.legacy = obj.legacy;
            this.legacyIndent = obj.legacyIndent;
            this.legacySpace = obj.legacySpace;
            this.originalLeftIndent = obj.originalLeftIndent;
            this.relativeRestartLevel = obj.relativeRestartLevel;
            this.separator = obj.separator;
            this.start = obj.start;
            this.suppressBulletResize = obj.suppressBulletResize;
            this.suppressRestart = obj.suppressRestart;
            this.templateCode = obj.templateCode;
        }

        clone(): ListLevelProperties {
            var clone = new ListLevelProperties();
            clone.copyFrom(this);
            return clone;
        }
    }

    export enum NumberingFormat {
        Decimal = 0,
        AIUEOHiragana = 1,
        AIUEOFullWidthHiragana = 2,
        ArabicAbjad = 3,
        ArabicAlpha = 4,
        Bullet = 5,
        CardinalText = 6,
        Chicago = 7,
        ChineseCounting = 8,
        ChineseCountingThousand = 9,
        ChineseLegalSimplified = 10,
        Chosung = 11,
        DecimalEnclosedCircle = 12,
        DecimalEnclosedCircleChinese = 13,
        DecimalEnclosedFullstop = 14,
        DecimalEnclosedParenthses = 15,
        DecimalFullWidth = 16,
        DecimalFullWidth2 = 17,
        DecimalHalfWidth = 18,
        DecimalZero = 19,
        Ganada = 20,
        Hebrew1 = 21,
        Hebrew2 = 22,
        Hex = 23,
        HindiConsonants = 24,
        HindiDescriptive = 25,
        HindiNumbers = 26,
        HindiVowels = 27,
        IdeographDigital = 28,
        IdeographEnclosedCircle = 29,
        IdeographLegalTraditional = 30,
        IdeographTraditional = 31,
        IdeographZodiac = 32,
        IdeographZodiacTraditional = 33,
        Iroha = 34,
        IrohaFullWidth = 35,
        JapaneseCounting = 36,
        JapaneseDigitalTenThousand = 37,
        JapaneseLegal = 38,
        KoreanCounting = 39,
        KoreanDigital = 40,
        KoreanDigital2 = 41,
        KoreanLegal = 42,
        LowerLetter = 43,
        LowerRoman = 44,
        None = 45,
        NumberInDash = 46,
        Ordinal = 47,
        OrdinalText = 48,
        RussianLower = 49,
        RussianUpper = 50,
        TaiwaneseCounting = 51,
        TaiwaneseCountingThousand = 52,
        TaiwaneseDigital = 53,
        ThaiDescriptive = 54,
        ThaiLetters = 55,
        ThaiNumbers = 56,
        UpperLetter = 57,
        UpperRoman = 58,
        VietnameseDescriptive = 59,
    }

    export enum ListNumberAlignment {
        Left = 0,
        Center = 1,
        Right = 2
    }
} 