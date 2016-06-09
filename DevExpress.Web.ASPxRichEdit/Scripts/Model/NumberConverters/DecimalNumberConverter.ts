module __aspxRichEdit {
    export class DecimalNumberConverter extends OrdinalBasedNumberConverter {
        constructor() {
            super();
            this.type = NumberingFormat.Decimal;
        }

        convertNumberCore(value: number): string {
            return value.toString();
        }
    }
} 