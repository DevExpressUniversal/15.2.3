module __aspxRichEdit {
    export class UpperLatinLetterNumberConverter extends AlphabetBasedNumberConverter {
        constructor() {
            this.type = NumberingFormat.UpperLetter;
            this.alphabet = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'];
            super();
        }
    }
    export class LowerLatinLetterNumberConverter extends AlphabetBasedNumberConverter {
        constructor() {
            this.type = NumberingFormat.LowerLetter;
            this.alphabet = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'];
            super();
        }
    }
} 