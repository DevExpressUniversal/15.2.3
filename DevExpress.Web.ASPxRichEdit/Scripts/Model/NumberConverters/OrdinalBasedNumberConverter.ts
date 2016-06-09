module __aspxRichEdit {
    export interface INumberConverter {
        minValue: number;
        maxValue: number;
        type: NumberingFormat;
        convertNumber(value: number): string;
    }

    export class OrdinalBasedNumberConverter implements INumberConverter {
        minValue: number;
        maxValue: number;
        type: NumberingFormat;

        constructor() {
            this.minValue = Number.MIN_VALUE;
            this.maxValue = Number.MAX_VALUE;
        }

        convertNumber(value: number): string {
            if(value < this.minValue || value > this.maxValue) {
                throw new Error("InvalidNumberConverterValue");
            }
            return this.convertNumberCore(value);
        }

        convertNumberCore(value: number): string {
            throw new Error(Errors.NotImplemented);
        }

        static createConverter(type: NumberingFormat): OrdinalBasedNumberConverter {
            switch(type) {
                case NumberingFormat.UpperRoman:
                    return new UpperRomanNumberConverterClassic();
                case NumberingFormat.LowerRoman:
                    return new LowerRomanNumberConverterClassic();
                case NumberingFormat.Ordinal:
                    return new OrdinalEnglishNumberConverter();
                case NumberingFormat.OrdinalText:
                    return new DescriptiveOrdinalEnglishNumberConverter();
                case NumberingFormat.CardinalText:
                    return new DescriptiveCardinalEnglishNumberConverter();
                case NumberingFormat.UpperLetter:
                    return new UpperLatinLetterNumberConverter();
                case NumberingFormat.LowerLetter:
                    return new LowerLatinLetterNumberConverter();
                case NumberingFormat.NumberInDash:
                    return new NumberInDashNumberConverter();
                case NumberingFormat.Bullet:
                    return new BulletNumberConverter();
                case NumberingFormat.DecimalZero:
                    return new DecimalZeroNumberConverter();
                case NumberingFormat.DecimalEnclosedParenthses:
                    return new DecimalEnclosedParenthesesNumberConverter();
                case NumberingFormat.Decimal:
                    return new DecimalNumberConverter();
                default:
                    return new DecimalNumberConverter();
            }
        }
    }

    export class OrdinalEnglishNumberConverter extends OrdinalBasedNumberConverter {
        private ending: string[] = ["st", "nd", "rd", "th"];

        constructor() {
            super();
            this.type = NumberingFormat.Ordinal;
        }

        convertNumberCore(value: number): string {
            var temp = value % 100;
            if(temp < 21) {
                switch(temp) {
                    case 1:
                        return ASPx.Formatter.Format("{0}{1}", value, this.ending[0]);
                    case 2: 
                        return ASPx.Formatter.Format("{0}{1}", value, this.ending[1]);
                    case 3:
                        return ASPx.Formatter.Format("{0}{1}", value, this.ending[2]);
                    default:
                        return ASPx.Formatter.Format("{0}{1}", value, this.ending[3]);
                }
            }
            value--;
            temp = value % 10;
            if(temp < 3)
                return ASPx.Formatter.Format("{0}{1}", value + 1, this.ending[temp % 3]);
            return ASPx.Formatter.Format("{0}{1}", value + 1, this.ending[3]);
        }
    }
} 