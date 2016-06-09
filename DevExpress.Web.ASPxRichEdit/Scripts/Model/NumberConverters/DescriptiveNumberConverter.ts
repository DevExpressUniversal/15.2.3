module __aspxRichEdit {
    export enum DigitType {
        Zero,
        SingleNumeral,
        Single,
        Teen,
        Tenth,
        Hundred,
        Thousand,
        Million,
        Billion,
        Trillion,
        Quadrillion,
        Quintillion,
        Separator
    }

    export class DigitInfo {
        provider: NumericsProvider;
        value: number;
        type: DigitType;

        constructor(provider: NumericsProvider, value: number, type: DigitType) {
            this.provider = provider;
            this.value = value;
            this.type = type;
        }

        convertToString(): string {
            var numerics = this.getNumerics();
            return numerics[this.value];
        }
        getNumerics(): string[] {
            return [];
        }
    }
    export class SeparatorDigitInfo extends DigitInfo {
        constructor(provider: NumericsProvider, value: number) {
            super(provider, value, DigitType.Separator);
        }

        getNumerics(): string[] {
            return this.provider.separator;
        }
    }
    export class QuintillionDigitInfo extends DigitInfo {
        constructor(provider: NumericsProvider, value: number) {
            super(provider, value, DigitType.Quintillion);
        }

        getNumerics(): string[] {
            return this.provider.quintillion;
        }
    }
    export class QuadrillionDigitInfo extends DigitInfo {
        constructor(provider: NumericsProvider, value: number) {
            super(provider, value, DigitType.Quadrillion);
        }

        getNumerics(): string[] {
            return this.provider.quadrillion;
        }
    }
    export class TrillionDigitInfo extends DigitInfo {
        constructor(provider: NumericsProvider, value: number) {
            super(provider, value, DigitType.Trillion);
        }

        getNumerics(): string[] {
            return this.provider.trillion;
        }
    }
    export class BillionDigitInfo extends DigitInfo {
        constructor(provider: NumericsProvider, value: number) {
            super(provider, value, DigitType.Billion);
        }

        getNumerics(): string[] {
            return this.provider.billion;
        }
    }
    export class MillionDigitInfo extends DigitInfo {
        constructor(provider: NumericsProvider, value: number) {
            super(provider, value, DigitType.Million);
        }

        getNumerics(): string[] {
            return this.provider.million;
        }
    }
    export class ThousandDigitInfo extends DigitInfo {
        constructor(provider: NumericsProvider, value: number) {
            super(provider, value, DigitType.Million);
        }

        getNumerics(): string[] {
            return this.provider.thousands;
        }
    }
    export class HundredDigitInfo extends DigitInfo {
        constructor(provider: NumericsProvider, value: number) {
            super(provider, value - 1, DigitType.Hundred);
        }

        getNumerics(): string[] {
            return this.provider.hundreds;
        }
    }
    export class TenthsDigitInfo extends DigitInfo {
        constructor(provider: NumericsProvider, value: number) {
            super(provider, value - 2, DigitType.Tenth);
        }

        getNumerics(): string[] {
            return this.provider.tenths;
        }
    }
    export class TeensDigitInfo extends DigitInfo {
        constructor(provider: NumericsProvider, value: number) {
            super(provider, value, DigitType.Teen);
        }

        getNumerics(): string[] {
            return this.provider.teens;
        }
    }
    export class SingleDigitInfo extends DigitInfo {
        constructor(provider: NumericsProvider, value: number) {
            super(provider, value - 1, DigitType.Single);
        }

        getNumerics(): string[] {
            return this.provider.singles;
        }
    }
    export class ZeroDigitInfo extends DigitInfo {
        constructor(provider: NumericsProvider) {
            super(provider, 9, DigitType.Zero);
        }

        getNumerics(): string[] {
            return this.provider.singles;
        }
    }

    export class DescriptiveEnglishNumberConverter extends OrdinalBasedNumberConverter {
        constructor() {
            super();
            this.minValue = 0;
        }

        convertNumberCore(value: number): string {
            var digits = this.generateDigits(value);
            return this.convertDigitsToString(digits);
        }

        generateDigits(value: number): DigitInfo[] {
            var digits = [];
            digits = this.generateDigitsCore(digits, value);
            if(digits.length == 0)
                this.addZero(digits);
            return digits;
        }

        private generateDigitsCore(digits: DigitInfo[], value: number): DigitInfo[] {
            var currentValue = value;
            if(Math.floor(currentValue / 1000000000000000000) != 0)
                this.generateQuintillionDigits(digits, Math.floor(currentValue / 1000000000000000000));
            currentValue = currentValue % 1000000000000000000;

            if(Math.floor(currentValue / 1000000000000000) != 0)
                this.generateQuadrillionDigits(digits, Math.floor(currentValue / 1000000000000000));
            currentValue = currentValue % 1000000000000000;

            if(Math.floor(currentValue / 1000000000000) != 0)
                this.generateTrillionDigits(digits, Math.floor(currentValue / 1000000000000));
            currentValue = currentValue % 1000000000000;

            if(Math.floor(currentValue / 1000000000) != 0)
                this.generateBillionDigits(digits, Math.floor(currentValue / 1000000000));
            currentValue = currentValue % 1000000000;

            if(Math.floor(currentValue / 1000000) != 0)
                this.generateMillionDigits(digits, Math.floor(currentValue / 1000000));
            currentValue = currentValue % 1000000;

            if(Math.floor(currentValue / 1000) != 0)
                this.generateThousandDigits(digits, Math.floor(currentValue / 1000));
            currentValue = currentValue % 1000;

            if(Math.floor(currentValue / 100) != 0)
                this.generateHundredDigits(digits, Math.floor(currentValue / 100));
            currentValue = currentValue % 100;

            if(currentValue == 0)
                return digits;

            if(currentValue >= 20)
                this.generateTenthsDigits(digits, currentValue);
            else {
                if(currentValue >= 10)
                    this.generateTeensDigits(digits, currentValue % 10);
                else
                    this.generateSinglesDigits(digits, currentValue);
            }

            return digits;
        }

        private convertDigitsToString(digits: DigitInfo[]): string {
            var result = "";
            for(var i = 0; i < digits.length; i++)
                result += digits[i].convertToString();
            if(result.length > 0)
                result = result[0].toUpperCase() + result.substring(1);
            return result;
        }

        private addZero(digits: DigitInfo[]) {
            digits.push(new ZeroDigitInfo(new CardinalEnglishNumericsProvider()));
        }
        private generateQuintillionDigits(digits: DigitInfo[], value: number) {
            this.generateDigitsCore(digits, value);
            if(digits.length)
                digits.push(new SeparatorDigitInfo(new CardinalEnglishNumericsProvider(), 0));
            digits.push(new QuintillionDigitInfo(new CardinalEnglishNumericsProvider(), 0));
        }
        private generateQuadrillionDigits(digits: DigitInfo[], value: number) {
            this.generateDigitsCore(digits, value);
            if(digits.length)
                digits.push(new SeparatorDigitInfo(new CardinalEnglishNumericsProvider(), 0));
            digits.push(new QuadrillionDigitInfo(new CardinalEnglishNumericsProvider(), 0));
        }
        private generateTrillionDigits(digits: DigitInfo[], value: number) {
            this.generateDigitsCore(digits, value);
            if(digits.length)
                digits.push(new SeparatorDigitInfo(new CardinalEnglishNumericsProvider(), 0));
            digits.push(new TrillionDigitInfo(new CardinalEnglishNumericsProvider(), 0));
        }
        private generateBillionDigits(digits: DigitInfo[], value: number) {
            this.generateDigitsCore(digits, value);
            if(digits.length)
                digits.push(new SeparatorDigitInfo(new CardinalEnglishNumericsProvider(), 0));
            digits.push(new BillionDigitInfo(new CardinalEnglishNumericsProvider(), 0));
        }
        private generateMillionDigits(digits: DigitInfo[], value: number) {
            this.generateDigitsCore(digits, value);
            if(digits.length)
                digits.push(new SeparatorDigitInfo(new CardinalEnglishNumericsProvider(), 0));
            digits.push(new MillionDigitInfo(new CardinalEnglishNumericsProvider(), 0));
        }
        private generateThousandDigits(digits: DigitInfo[], value: number) {
            this.generateDigitsCore(digits, value);
            if(digits.length)
                digits.push(new SeparatorDigitInfo(new CardinalEnglishNumericsProvider(), 0));
            digits.push(new ThousandDigitInfo(new CardinalEnglishNumericsProvider(), 0));
        }
        private generateHundredDigits(digits: DigitInfo[], value: number) {
            if(digits.length)
                digits.push(new SeparatorDigitInfo(new CardinalEnglishNumericsProvider(), 0));
            digits.push(new HundredDigitInfo(new CardinalEnglishNumericsProvider(), value));
        }
        private generateTenthsDigits(digits: DigitInfo[], value: number) {
            if(digits.length)
                digits.push(new SeparatorDigitInfo(new CardinalEnglishNumericsProvider(), 0));
            digits.push(new TenthsDigitInfo(new CardinalEnglishNumericsProvider(), Math.floor(value / 10)));
            this.generateSinglesDigits(digits, value % 10);
        }
        private generateTeensDigits(digits: DigitInfo[], value: number) {
            if(digits.length)
                digits.push(new SeparatorDigitInfo(new CardinalEnglishNumericsProvider(), 0));
            digits.push(new TeensDigitInfo(new CardinalEnglishNumericsProvider(), value));
        }
        private generateSinglesDigits(digits: DigitInfo[], value: number) {
            if(value == 0)
                return;
            if(digits.length != 0) {
                if (digits[digits.length - 1].type == DigitType.Tenth)
                    digits.push(new SeparatorDigitInfo(new CardinalEnglishNumericsProvider(), 1));
                else
                    digits.push(new SeparatorDigitInfo(new CardinalEnglishNumericsProvider(), 0));
            }
            digits.push(new SingleDigitInfo(new CardinalEnglishNumericsProvider(), value));
        }
    }

    export class DescriptiveCardinalEnglishNumberConverter extends DescriptiveEnglishNumberConverter {
        constructor() {
            super();
            this.type = NumberingFormat.CardinalText;
        }
    }
    export class DescriptiveOrdinalEnglishNumberConverter extends DescriptiveEnglishNumberConverter {
        constructor() {
            super();
            this.type = NumberingFormat.OrdinalText;
        }

        generateDigits(value: number): DigitInfo[] {
            var digits = super.generateDigits(value);
            digits[digits.length - 1].provider = new OrdinalEnglishNumericsProvider();
            return digits;
        }
    }
} 