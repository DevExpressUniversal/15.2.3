module __aspxRichEdit {
    export class InputPositionCompareInfo {
        name: string;
        comparer: any; // here must be binary function
        defaultValue: any;

        constructor(name: string, comparer: any, defaultValue: any) {
            this.name = name;
            this.comparer = comparer;
            this.defaultValue = defaultValue;
        }
    }

    export class Utils {
        static sortAndDisctinctNumbers(array: number[]): number[] {
            array = array.sort();
            var prevValue = Number.NaN;
            for(let i = array.length - 1; i >= 0; i--) {
                if(prevValue === array[i])
                    array.splice(i, 1);
                prevValue = array[i];
            }
            return array;
        }

        static exactlyEqual(a: any, b: any): boolean {
            return a === b;
        }

        static getSymbolFromEnd(text: string, posFromEnd: number): string {
            return text[text.length - posFromEnd];
        }

        static isHTMLElementNode(node: Node) {
            return node.nodeType == Node.ELEMENT_NODE;
        }

        static stringInLowerCase(str: string): boolean {
            return str.toLowerCase() == str;
        }

        static stringInUpperCase(str: string): boolean {
            return str.toUpperCase() == str;
        }

        static inStringAtLeastOneSymbolInUpperCase(str: string): boolean {
            for (var i: number = 0, char: string; char = str[i]; i++)
                if (Utils.stringInUpperCase(char) && !Utils.stringInLowerCase(char))
                    return true;
            return false;
        }

        static isTextNode(node: Node) {
            return node.nodeType == Node.TEXT_NODE;
        }

        static stringTrim(str: string): string {
            return str.replace(/(^\s*)|(\s*)$/g, '');
        }

        static boolToInt(value: boolean): number {
            return value ? 1 : 0;
        }

        // inputStr = \"\'\\ => result = "'\
        static hideEscapes(inputStr: string): string {
            var res: string = "";
            return eval("res='" + inputStr.replace(/'/g, "\'") + "'");
        }
        public static isEven(num: number): boolean {
            return (num % 2) != 0;
        }
        public static isOdd(num: number): boolean {
            return !Utils.isEven(num);
        }

        public static isLatinLetter: RegExp = /\w/;
        public static isWhitespace: RegExp = /\s/; // space, tab, caret break
        public static isAlphanumeric: RegExp = /[\u00C0-\u1FFF\u2C00-\uD7FF\w]/;
        public static predefinedFontSizes: number[] = [8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72];
        public static specialCharacters: SpecialCharacters = new SpecialCharacters();

        //http://workservices01/OpenWiki/ow.asp?ASPxRichEdit_BinarySearch#preview
        // BINARY INDEX 
        // Input array [4, 8, 10]
        // find       binaryIndexOf normedBinaryIndexOf
        // (-inf, 3]     -1            -1
        // 4              0             0
        // [5, 7]        -2             0
        // 8              1             1
        // 9             -3             1
        // 10             2             2
        // [11, +inf)    -4             2
        // case array.length == 0, then return -1
        // don't touch default value  = -2! In some case binaryIndexOf call as ([], ()=>.., 0, [].length - 1)
        public static binaryIndexOf<T>(array: T[], comparer: (a: T) => number, minIndex = 0, maxIndex = -2): number {
            const findFromZeroPosition: boolean = minIndex == 0;
            if (maxIndex == -2)
                maxIndex = array.length - 1;
            while (minIndex <= maxIndex) {
                const currentIndex: number = (minIndex + ((maxIndex - minIndex) >> 1));
                const compare: number = comparer(array[currentIndex]);
                if (compare < 0)
                    minIndex = currentIndex + 1;
                else if (compare > 0)
                    maxIndex = currentIndex - 1;
                else
                    return currentIndex;
            }
            return findFromZeroPosition ? ~minIndex : -1;
        }

        // see description above
        public static normedBinaryIndexOf<T>(array: T[], comparer: (a: T) => number, minIndex: number = 0, maxIndex: number = -2): number {
            const index: number = Utils.binaryIndexOf(array, comparer, minIndex, maxIndex);
            return Utils.binaryIndexNormalizator(index);
        }

        public static binaryIndexNormalizator(index: number): number {
            return index < 0 ? ~index - 1 : index;
        }

        public static strCompare(a: string, b: string): number {
            return ((a == b) ? 0 : ((a > b) ? 1 : -1));
        }

        public static getDecimalSeparator(): string {
            return (1.1).toLocaleString().substr(1, 1);
        }

        public static getHashCode(str: string): number {
            var strLength: number = str.length;
            var hash: number = 0;
            for (var charIndex: number = 0; charIndex < strLength; charIndex++) {
                var unicodeValue: number = str.charCodeAt(charIndex);
                hash = ((hash << 5) - hash) + unicodeValue;
                hash |= 0; // Convert to 32bit integer
            }
            return hash;
        }

        // + + + + + + + + + + +
        // 1   5 2 1 2 5 3 4 4 3 - intervals
        // [   [ [ ] ] ] [ [ ] ] 
        //          TO
        // [           ] [     ]
        public static getMergedIntervals(intervals: FixedInterval[], needSort: boolean): FixedInterval[] {
            if (intervals.length < 2)
                return intervals.length > 0 ? [intervals[0]] : [];

            var sortedIntervals: FixedInterval[];
            if (needSort)
                sortedIntervals = [].concat(intervals).sort(function (a: FixedInterval, b: FixedInterval) { return a.start - b.start; }); // copy array and sort
            else
                sortedIntervals = intervals;

            var result: FixedInterval[] = [];
            for (var i: number = 0, interval: FixedInterval; interval = sortedIntervals[i];) {
                var minBound: number = interval.start;
                var maxBound: number = interval.end();
                for (++i; (interval = sortedIntervals[i]) != undefined && (interval.start <= maxBound); i++)
                    if (interval.end() > maxBound)
                        maxBound = interval.end();
                result.push(FixedInterval.fromPositions(minBound, maxBound));
            }
            return result;
        }

        public static indexOf(array: any[], element: any): number {
            if (Array.prototype.indexOf)
                return array.indexOf(element);
            else {
                for (var i = 0, el; el = array[i]; i++)
                    if (el === element)
                        return i;
                return -1;
            }
        }

        // mergeStringNTimes("&nbsp;", 1) = "&nbsp;"
        // mergeStringNTimes("&nbsp;", 3) = "&nbsp;&nbsp;&nbsp;"
        public static mergeStringNTimes(str: string, times: number): string {
            return new Array(times <= 0 ? 0 : times + 1).join(str);
        }

        public static getKeyModifiers(evt: any): number {
            var result = 0;
            if (evt.altKey)
                result |= KeyModifiers.Alt;
            if (evt.ctrlKey)
                result |= KeyModifiers.Ctrl;
            if (evt.shiftKey)
                result |= KeyModifiers.Shift;
            return result;
        }

        public static encodeHtml(text: string): string {
            return text.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
        }

        public static getNextPredefinedFontSize(current: number): number {
            var index = Utils.normedBinaryIndexOf(Utils.predefinedFontSizes, (a) => a - current);
            return Utils.predefinedFontSizes[index + 1] !== undefined ? Utils.predefinedFontSizes[index + 1] : (Math.floor(current / 10) * 10 + 10);
        }

        public static getPreviousPredefinedFontSize(current: number): number {
            var index = Utils.binaryIndexOf(Utils.predefinedFontSizes, (a) => a - current);
            if (index > 0)
                return Utils.predefinedFontSizes[index - 1];
            if (index < 0)
                index = ~index;
            if (index === 0)
                return Math.max(1, current - 1);
            var predefinedFontsCount = Utils.predefinedFontSizes.length;
            if (index < predefinedFontsCount)
                return Utils.predefinedFontSizes[index - 1];
            var newValue = current % 10 > 0 ? (Math.floor(current / 10) * 10) : (Math.floor(current / 10) * 10 - 10);
            if (newValue >= Utils.predefinedFontSizes[predefinedFontsCount - 1])
                return newValue;
            return Utils.predefinedFontSizes[predefinedFontsCount - 1];
        }

        public static getRandomInt(min: number, max: number) {
            return Math.floor(Math.random() * (max - min + 1)) + min;
        }

        public static getSelectedParagraphs(selection: Selection, activeSubDocument: SubDocument): { paragraphs: Paragraph[]; intervals: FixedInterval[] } {
            var intervals: FixedInterval[] = Utils.getMergedIntervals(selection.intervals, true);

            // collect paragraphs
            var selectedParagraphs: Paragraph[] = [];
            for (var i: number = 0, interval: FixedInterval; interval = intervals[i]; i++)
                selectedParagraphs = selectedParagraphs.concat(activeSubDocument.getParagraphsByInterval(interval));


            // delete dublicates
            selectedParagraphs = selectedParagraphs.sort(function (a: Paragraph, b: Paragraph) { return a.startLogPosition.value < b.startLogPosition.value ? -1 : 1; });
            var newSelectedParagraphs: Paragraph[] = [selectedParagraphs[0]];
            var prevLogPos: number = newSelectedParagraphs[0].startLogPosition.value;
            for (var i: number = 1, paragraph: Paragraph; paragraph = selectedParagraphs[i]; i++)
                if (paragraph.startLogPosition.value != prevLogPos) {
                    newSelectedParagraphs.push(paragraph);
                    prevLogPos = paragraph.startLogPosition.value;
                }

            return { paragraphs: newSelectedParagraphs, intervals: intervals };
        }

        public static foreach<T>(collection: T[], callback: (value: T, index: number) => void): void {
            for (var colIndex = 0; colIndex < collection.length; colIndex++)
                callback(collection[colIndex], colIndex);
        }
    }

    
} 