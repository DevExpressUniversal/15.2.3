module __aspxRichEdit {
    export class DecimalEnclosedParenthesesNumberConverter extends OrdinalBasedNumberConverter {
        constructor() {
            super();
            this.type = NumberingFormat.DecimalEnclosedParenthses;
        }

        convertNumberCore(value: number): string {
            return ASPx.Formatter.Format("({0})", value);
        }
    }
}