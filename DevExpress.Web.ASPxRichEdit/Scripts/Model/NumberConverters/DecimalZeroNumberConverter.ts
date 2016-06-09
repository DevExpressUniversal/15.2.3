module __aspxRichEdit {
    export class DecimalZeroNumberConverter extends OrdinalBasedNumberConverter {
        constructor() {
            super();
            this.type = NumberingFormat.DecimalZero;
        }

        convertNumberCore(value: number) : string {
            if(value < 10)
                return ASPx.Formatter.Format("0{0}", value);
            else
                return value.toString();
        }
    }
} 