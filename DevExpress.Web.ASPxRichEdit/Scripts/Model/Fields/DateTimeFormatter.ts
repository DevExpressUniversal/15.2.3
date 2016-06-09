module __aspxRichEdit {
    export class DateTimeFieldFormatter {
        private date: Date;
        private formatString: string;
        private result: string;
        private AMPMKeyword = "am/pm";

        format(date: Date, formatString: string): string {
            this.date = date;
            this.formatString = formatString;
            this.result = "";

            var index = 0;
            var formatLength = this.formatString.length;
            while(index < formatLength) {
                index += this.formatNext(index);
            }
            return this.result;
        }

        private formatNext(index: number): number {
            var ch = this.formatString[index];

            var formattingItem: DateTimeFormattingItem = this.tryCreateFormattingItem(ch);
            if(formattingItem)
                return this.processAsFormattingItem(index, formattingItem);
            if(this.isKeyword(this.AMPMKeyword, index))
                return this.processAsAMPMKeyword();
            if(ch == '\'')
                return this.processAsEmbedText(index);
            return this.processAsSingleCharacter(index);
        }

        private isKeyword(keyword: string, index: number): boolean {
            if(keyword.length > (this.formatString.length - index))
                return false;
            var substring = this.formatString.substr(index, keyword.length);
            return keyword.toLowerCase() === substring.toLowerCase();
        }

        private processAsAMPMKeyword(): number {
            var result = (this.date.getHours() - 12) >= 0 ? ASPx.CultureInfo.pm : ASPx.CultureInfo.am;
            this.result += result;
            return this.AMPMKeyword.length;
        }

        private processAsEmbedText(index: number): number {
            var startTextIndex = index + 1;
            if(startTextIndex >= (this.formatString.length - 1))
                return 1;

            var textLength = this.getCharacterSequenceLength(this.formatString[index], startTextIndex, this.charsAreNotEqual);
            if((textLength + startTextIndex) == this.formatString.length) {
                this.result += "'";
                return 1;
            }
            this.result += this.formatString.substr(startTextIndex, textLength);
            return textLength + 2;
        }

        private processAsSingleCharacter(index: number): number {
            this.result += this.formatString[index];
            return 1;
        }

        private processAsFormattingItem(index: number, formattingItem: DateTimeFormattingItem): number {
            var sequenceLength = this.getCharacterSequenceLength(this.formatString[index], index, this.charsAreEqual);
            var patternLength = formattingItem.getAvailablePatternLength(sequenceLength);
            var result = formattingItem.format(this.date, patternLength);
            this.result += result;
            return Math.min(sequenceLength, patternLength);
        }

        private getCharacterSequenceLength(ch: string, index: number, predicate: (ch1: string, ch2: string) => boolean) {
            var length = this.formatString.length;
            var nextCharIndex = index + 1;
            while(nextCharIndex < length && predicate(ch, this.formatString[nextCharIndex]))
                nextCharIndex++;
            return nextCharIndex - index;
        }

        private tryCreateFormattingItem(formattingChar: string): DateTimeFormattingItem {
            switch(formattingChar) {
                case 'h':
                    return new Hour12FormattingItem();
                case 'H':
                    return new Hour24FormattingItem();
                case 'm':
                    return new MinuteFormattingItem();
                case 'S':
                case 's':
                    return new SecondFormattingItem();
                case 'Y':
                case 'y':
                    return new YearFormattingItem();
                case 'M':
                    return new MonthFormattingItem();
                case 'D':
                case 'd':
                    return new DayFormattingItem();
            }
            return null;
        }

        private charsAreEqual(ch1: string, ch2: string): boolean {
            return ch1 === ch2;
        }
        private charsAreNotEqual(ch1: string, ch2: string): boolean {
            return ch1 !== ch2;
        }
    }

    class DateTimeFormattingItem { // abstract
        patternsLength: number[];
        format(date: Date, patternLength: number): string {
            throw new Error(Errors.NotImplemented);
        }
        getAvailablePatternLength(patternLength: number) {
            var count = this.patternsLength.length;
            for(var i = 0; i < count; i++) {
                if(this.patternsLength[i] >= patternLength)
                    return this.patternsLength[i];
            }
            return this.patternsLength[count - 1];
        }
    }

    class NumericFormattingItem extends DateTimeFormattingItem { // abstract
        patternsLength: number[] = [1, 2];

        formatCore(value: number, patternLength: number): string {
            var result = "" + value;
            if(patternLength === 2 && result.length === 1)
                return "0" + result;
            return result;
        }
    }

    class CombinedFormattingItem extends NumericFormattingItem { // abstract
        patternsLength: number[] = [1, 2, 3, 4];

        format(date: Date, patternLength: number) {
            if(patternLength <= 2)
                return this.formatCore(this.getNumericValue(date), patternLength);
            if(patternLength === 3)
                return this.getAbbreviatedName(date);
            return this.getFullName(date);
        }

        getNumericValue(date: Date): number {
            throw new Error(Errors.NotImplemented);
        }
        getAbbreviatedName(date: Date): string {
            throw new Error(Errors.NotImplemented);
        }
        getFullName(date: Date): string {
            throw new Error(Errors.NotImplemented);
        }
    }

    class Hour24FormattingItem extends NumericFormattingItem {
        format(date: Date, patternLength: number): string {
            return this.formatCore(date.getHours(), patternLength);
        }
    }

    class Hour12FormattingItem extends NumericFormattingItem {
        format(date: Date, patternLength: number): string {
            var hour = date.getHours() % 12;
            if(hour == 0)
                hour = 12;
            return this.formatCore(hour, patternLength);
        }
    }

    class MinuteFormattingItem extends NumericFormattingItem {
        format(date: Date, patternLength: number): string {
            return this.formatCore(date.getMinutes(), patternLength);
        }
    }

    class SecondFormattingItem extends NumericFormattingItem {
        format(date: Date, patternLength: number): string {
            return this.formatCore(date.getSeconds(), patternLength);
        }
    }

    class DayFormattingItem extends CombinedFormattingItem {
        getAbbreviatedName(date: Date) {
            return ASPx.CultureInfo.abbrDayNames[this.getDayOfWeek(date)];
        }
        getFullName(date: Date): string {
            return ASPx.CultureInfo.dayNames[this.getDayOfWeek(date)];
        }
        getNumericValue(date: Date) {
            return date.getDate();
        }
        getDayOfWeek(date: Date): number {
            return date.getDay();
        }
    }

    class MonthFormattingItem extends CombinedFormattingItem {
        getAbbreviatedName(date: Date) {
            return ASPx.CultureInfo.abbrMonthNames[date.getMonth()];
        }
        getFullName(date: Date): string {
            return ASPx.CultureInfo.monthNames[date.getMonth()];
        }
        getNumericValue(date: Date) {
            return date.getMonth() + 1;
        }
    }

    class YearFormattingItem extends DateTimeFormattingItem {
        patternsLength: number[] = [2, 4];
        format(date: Date, patternLength: number): string {
            var year = date.getFullYear();
            if(patternLength == 2 && year > 99) {
                var shortYear = year % 100;
                var result = "" + shortYear;
                if(result.length === 1)
                    return "0" + result;
                return result;
            }
            return "" + year;
        }
    }
}