module __aspxRichEdit {
    export class NumberInDashNumberConverter extends OrdinalBasedNumberConverter {
        constructor() {
            super();
            this.type = NumberingFormat.NumberInDash;
        }

        convertNumberCore(value: number): string {
            return ASPx.Formatter.Format("- {0} -", value);
        }
    }
} 