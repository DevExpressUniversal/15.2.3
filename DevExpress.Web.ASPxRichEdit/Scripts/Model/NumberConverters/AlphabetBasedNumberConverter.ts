module __aspxRichEdit {
    export class AlphabetBasedNumberConverter extends OrdinalBasedNumberConverter {
        alphabet: string[];
        alphabetSize: number;

        constructor() {
            super();
            this.minValue = 0;
            this.maxValue = 780;
            this.alphabetSize = this.alphabet.length;
        }

        convertNumberCore(value: number): string {
            if(value == 0)
                return "";
            value--;
            var count = Math.floor(value / this.alphabetSize + 1);
            var symbol = this.alphabet[value % this.alphabetSize];
            return Array(count + 1).join(symbol);
        }
    }
}