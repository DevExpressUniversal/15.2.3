module __aspxRichEdit {
    export class LineSpacingCalculator {
        public static create(lineSpacing: number, lineSpacingType: ParagraphLineSpacingType, unitConverter: IUnitConverter): LineSpacingCalculator {
            switch (lineSpacingType) {
                case ParagraphLineSpacingType.AtLeast:
                    return new AtLeastSpacingCalculator(Math.max(1, unitConverter.toPixels(lineSpacing)));
                case ParagraphLineSpacingType.Double:
                    return new DoubleSpacingCalculator();
                case ParagraphLineSpacingType.Exactly:
                    return new ExactlySpacingCalculator(Math.max(1, unitConverter.toPixels(lineSpacing)));
                case ParagraphLineSpacingType.Multiple:
                    return new MultipleSpacingCalculator(lineSpacing !== 0 ? lineSpacing : 1);
                case ParagraphLineSpacingType.Sesquialteral:
                    return new SesquialteralSpacingCalculator();
                case ParagraphLineSpacingType.Single:
                default:
                    return new SingleSpacingCalculator();
            }
        }

        public calculate(rowHeight: number, maxAscent: number, maxDescent: number, maxPictureHeight: number): number {
            var maxTextHeight = maxAscent + maxDescent;
            if (!maxTextHeight || maxTextHeight == 0)
                return rowHeight;
            var rowTextSpacing = this.calculateSpacing(maxTextHeight);
            if (!maxPictureHeight || maxAscent > maxPictureHeight)
                return rowTextSpacing;
            else {
                if (maxDescent == 0) // only picture or tabs (and possible paragraph mark) in row
                    return rowHeight;
                else
                    return this.calculateSpacingInlineObjectCase(rowHeight, maxTextHeight, rowTextSpacing, maxPictureHeight, maxDescent);
            }
        }
        calculateSpacing(maxTextHeight: number): number {
            throw new Error(Errors.NotImplemented);
        }
        calculateSpacingInlineObjectCase(rowHeight: number, maxTextHeight: number, rowTextSpacing: number, maxPictureHeight: number, maxDescent: number): number {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class MultipleSpacingCalculator extends LineSpacingCalculator {
        multiplier: number;

        public constructor(multiplier: number) {
            super();
            if (multiplier <= 0)
                throw new Error(Errors.ArgumentException("multiplier", multiplier));
            this.multiplier = multiplier;
        }

        calculateSpacing(maxTextHeight: number): number {
            return maxTextHeight * this.multiplier;
        }
        calculateSpacingInlineObjectCase(rowHeight: number, maxTextHeight: number, rowTextSpacing: number, maxPictureHeight: number, maxDescent: number): number {
            return maxPictureHeight + maxDescent + (rowTextSpacing - maxTextHeight);
        }
    }
    export class SingleSpacingCalculator extends LineSpacingCalculator {
        calculateSpacing(maxTextHeight: number): number {
            return maxTextHeight;
        }
        calculateSpacingInlineObjectCase(rowHeight: number, maxTextHeight: number, rowTextSpacing: number, maxPictureHeight: number, maxDescent: number): number {
            return maxPictureHeight + maxDescent;
        }
    }
    export class DoubleSpacingCalculator extends LineSpacingCalculator {
        calculateSpacing(maxTextHeight: number): number {
            return 2 * maxTextHeight;
        }
        calculateSpacingInlineObjectCase(rowHeight: number, maxTextHeight: number, rowTextSpacing: number, maxPictureHeight: number, maxDescent: number): number {
            return maxPictureHeight + maxDescent + (rowTextSpacing - maxTextHeight);
        }
    }
    export class SesquialteralSpacingCalculator extends LineSpacingCalculator {
        calculateSpacing(maxTextHeight: number): number {
            return 3 * maxTextHeight / 2;
        }
        calculateSpacingInlineObjectCase(rowHeight: number, maxTextHeight: number, rowTextSpacing: number, maxPictureHeight: number, maxDescent: number): number {
            return maxPictureHeight + maxDescent + (rowTextSpacing - maxTextHeight);
        }
    }

    export class ExactlySpacingCalculator extends LineSpacingCalculator {
        lineSpacing: number;

        public constructor(lineSpacing: number) {
            super();
            this.lineSpacing = lineSpacing;
        }

        public calculate(rowHeight: number, maxAscent: number, maxDescent: number, maxPictureHeight: number) {
            return this.lineSpacing;
        }
    }

    export class AtLeastSpacingCalculator extends SingleSpacingCalculator {
        lineSpacing: number;

        public constructor(lineSpacing: number) {
            super();
            if (lineSpacing <= 0)
                throw new Error(Errors.ArgumentException("lineSpacing", lineSpacing));
            this.lineSpacing = lineSpacing;
        }

        public calculate(rowHeight: number, maxAscent: number, maxDescent: number, maxPictureHeight: number) {
            var result = super.calculate(rowHeight, maxAscent, maxDescent, maxPictureHeight);
            return Math.max(result, this.lineSpacing);
        }
    }
}