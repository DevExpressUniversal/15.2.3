module __aspxRichEdit {
    export class Size {
        constructor(width: number, height: number) {
            this.width = width;
            this.height = height;
        }
        width: number;
        height: number;
    }

    export class Point {
        constructor(x: number, y: number) {
            this.x = x;
            this.y = y;
        }
        x: number;
        y: number;
    }
    
    export class Rectangle implements IEquatable<Rectangle>, ICloneable<Rectangle>, ISupportCopyFrom<Rectangle> {
        x: number;
        y: number;
        width: number;
        height: number;
        
        init(x: number, y: number, width: number, height: number): Rectangle {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            return this;
        }
        equals(obj: Rectangle): boolean {
            return obj &&
                Rectangle.equalsBinary(this, obj);
        }

        static equalsBinary(A: Rectangle, B: Rectangle): boolean {
            return A.x === B.x &&
                A.y === B.y &&
                A.width === B.width &&
                A.height === B.height;
        }

        isEqualsWidthAndHeight(width: number, height: number): boolean {
            return this.width == width && this.height == height;
        }
        clone(): Rectangle {
            var rect: Rectangle = new Rectangle();
            rect.copyFrom(this);
            return rect;
        }
        copyFrom(obj: Rectangle) {
            this.x = obj.x;
            this.y = obj.y;
            this.width = obj.width;
            this.height = obj.height;
        }
        containsPoint(x: number, y: number) {
            return x >= this.x && y >= this.y && this.x + this.width >= x && this.y + this.height >= y;
        }
        getRightBoundPosition(): number {
            return this.x + this.width;
        }

        getBottomBoundPosition(): number {
            return this.y + this.height;
        }

        applyLeftInternalOffset(offset: number) {
            offset = Math.min(offset, this.width);
            this.x += offset;
            this.width -= offset;
        }

        applyRightInternalOffset(offset: number) {
            this.width -= Math.min(offset, this.width);
        }
    }

    export enum RichEditUnit {
        Centimeter = 0,
        Inch = 1
    }

    export class UnitConverter {
        ui: RichEditUnit;

        constructor(ui: RichEditUnit) {
            this.ui = ui;
            switch(ui) {
                case RichEditUnit.Centimeter:
                    this.twipsToUI = UnitConverter.twipsToCentimeters;
                    this.UIToTwips = UnitConverter.centimetersToTwips;
                    break;
                case RichEditUnit.Inch:
                    this.twipsToUI = UnitConverter.twipsToInches;
                    this.UIToTwips = UnitConverter.inchesToTwips;
                    break;
            }
        }
        twipsToUI(value: number): number {
            throw new Error(Errors.NotImplemented);
        }
        UIToTwips(value: number): number {
            throw new Error(Errors.NotImplemented);
        }

        static DPI: number = 96;
        static CENTIMETERS_PER_INCH: number = 2.54;
        static PICAS_PER_INCH: number = 6;

        // LAYOUT - PIXELS
        // MODEL  - TWIPS
        // FORMS  - CENTIMETERS / INCHES
        // FONT   - POINTS

        // TO TWIPS (round and no round version)
        static pixelsToTwips(value: number): number {
            return Math.round(UnitConverter.pixelsToTwipsF(value));
        }
        static inchesToTwips(value: number): number {
            return Math.round(UnitConverter.inchesToTwipsF(value));
        }
        static pointsToTwips(value: number): number {
            return Math.round(UnitConverter.pointsToTwipsF(value));
        }
        static picasToTwips(value: number): number {
            return Math.round(value * 1440 / UnitConverter.PICAS_PER_INCH);
        }
        static centimetersToTwips(value: number): number {
            return Math.round(UnitConverter.centimetersToTwipsF(value));
        }

        static pixelsToTwipsF(value: number): number {
            return value * 1440 / UnitConverter.DPI;
        }
        static inchesToTwipsF(value: number): number {
            return value * 1440;
        }
        static pointsToTwipsF(value: number): number {
            return value * 20;
        }
        static centimetersToTwipsF(value: number): number {
            return value * 1440 / UnitConverter.CENTIMETERS_PER_INCH;
        }

        // TO PIXELS (round)
        static twipsToPixels(value: number): number {
            return Math.round(UnitConverter.twipsToPixelsF(value));
        }
        static inchesToPixels(value: number): number {
            return Math.round(UnitConverter.DPI * value);
        }
        static centimeterToPixel(value: number): number {
            return Math.round(value / (UnitConverter.CENTIMETERS_PER_INCH / UnitConverter.DPI));
        }
        static pointsToPixels(value: number): number {
            return Math.round(value * UnitConverter.DPI / 72);
        }

        static twipsToPixelsF(value: number): number {
            return value * UnitConverter.DPI / 1440;
        }

        // TO POINTS (round)
        static pixelsToPoints(value: number): number { // call in HtmlImport (fontSize to points)
            return Math.round(value * 72 / UnitConverter.DPI);
        }
        static twipsToPoints(value: number): number {
            return Math.round(this.twipsToPointsF(value));
        }
        // TO POINTS (no round)
        static twipsToPointsF(value: number): number {
            return value / 20;
        }

        // TO INCHES (no round)
        static twipsToInches(value: number): number {
            return value / 1440;
        }
        static pixelsToInches(value: number): number {
            return value / UnitConverter.DPI;
        }

        // TO CENTIMETERS (no round)
        static twipsToCentimeters(value: number): number {
            return value * UnitConverter.CENTIMETERS_PER_INCH / 1440;
        }
        static pixelToCentimeters(value: number): number {
            return value * UnitConverter.CENTIMETERS_PER_INCH / UnitConverter.DPI;
        }
    }

    export interface IUnitConverter {
        toPixels(value: number): number;
    }
    export class TwipsUnitConverter implements IUnitConverter {
        toPixels(value: number) {
            return UnitConverter.twipsToPixels(value);
        }
    }
    export class PaperSizeConverter {
        static paperSizeTable: { [kind: number]: Size } = PaperSizeConverter.createPaperSizeTable();
        static createPaperSizeTable(): { [kind: number]: Size } {
            var result: { [kind: number]: Size } = {};
            result[PaperKind.Letter] = new Size(12240, 15840); // Letter 8 1/2 x 11 in
            result[PaperKind.LetterSmall] = new Size(12240, 15840); // Letter Small 8 1/2 x 11 in
            result[PaperKind.Tabloid] = new Size(15840, 24480); // Tabloid 11 x 17 in
            result[PaperKind.Ledger] = new Size(24480, 15840); // Ledger 17 x 11 in
            result[PaperKind.Legal] = new Size(12240, 20160);  // Legal 8 1/2 x 14 in
            result[PaperKind.Statement] = new Size(7920, 12240); // Statement 5 1/2 x 8 1/2 in
            result[PaperKind.Executive] = new Size(10440, 15120); // Executive 7 1/4 x 10 1/2 in
            result[PaperKind.A3] = new Size(16839, 23814); // A3 297 x 420 mm
            result[PaperKind.A4] = new Size(11907, 16839); // A4 210 x 297 mm
            result[PaperKind.A4Small] = new Size(11907, 16839); // A4 Small 210 x 297 mm
            result[PaperKind.A5] = new Size(8391, 11907); // A5 148 x 210 mm
            result[PaperKind.B4] = new Size(14572, 20639); // B4 (JIS) 250 x 354
            result[PaperKind.B5] = new Size(10319, 14571); // B5 (JIS) 182 x 257 mm
            result[PaperKind.Folio] = new Size(12240, 18720); // Folio 8 1/2 x 13 in
            result[PaperKind.Quarto] = new Size(12189, 15591); // Quarto 215 x 275 mm
            result[PaperKind.Standard10x14] = new Size(14400, 20160); // 10x14 in
            result[PaperKind.Standard11x17] = new Size(15840, 24480); // 11x17·in
            result[PaperKind.Note] = new Size(12240, 15840); // Note 8 1/2 x 11 in
            result[PaperKind.Number9Envelope] = new Size(5580, 12780); // Envelope #9 3 7/8 x 8 7/8
            result[PaperKind.Number10Envelope] = new Size(5940, 13680); // Envelope #10 4 1/8 x 9 1/2
            result[PaperKind.Number11Envelope] = new Size(6480, 14940); // Envelope #11 4 1/2 x 10 3/8
            result[PaperKind.Number12Envelope] = new Size(6840, 15840); // Envelope #12 4 3/4 x 11
            result[PaperKind.Number14Envelope] = new Size(7200, 16560); // Envelope #14 5 x 11 1/2
            result[PaperKind.CSheet] = new Size(24480, 31680); // C size sheet
            result[PaperKind.DSheet] = new Size(31680, 48960); // D size sheet
            result[PaperKind.ESheet] = new Size(48960, 63360); // E size sheet
            result[PaperKind.DLEnvelope] = new Size(6237, 12474); // Envelope DL 110 x 220mm
            result[PaperKind.C5Envelope] = new Size(9185, 12984); // Envelope C5 162 x 229 mm
            result[PaperKind.C3Envelope] = new Size(18369, 25965); // Envelope C3  324 x 458 mm
            result[PaperKind.C4Envelope] = new Size(12983, 18369); // Envelope C4  229 x 324 mm
            result[PaperKind.C6Envelope] = new Size(6463, 9184); // Envelope C6  114 x 162 mm
            result[PaperKind.C65Envelope] = new Size(6463, 12983); // Envelope C65 114 x 229 mm
            result[PaperKind.B4Envelope] = new Size(14173, 20013); // Envelope B4  250 x 353 mm
            result[PaperKind.B5Envelope] = new Size(9978, 14173); // Envelope B5  176 x 250 mm
            result[PaperKind.B6Envelope] = new Size(9978, 7087); // Envelope B6  176 x 125 mm
            result[PaperKind.ItalyEnvelope] = new Size(6236, 13039); // Envelope 110 x 230 mm
            result[PaperKind.MonarchEnvelope] = new Size(5580, 10800); // Envelope Monarch 3.875 x 7.5 in
            result[PaperKind.PersonalEnvelope] = new Size(5220, 9360); // 6 3/4 Envelope 3 5/8 x 6 1/2 in
            result[PaperKind.USStandardFanfold] = new Size(21420, 15840); // US Std Fanfold 14 7/8 x 11 in
            result[PaperKind.GermanStandardFanfold] = new Size(12240, 17280); // German Std Fanfold 8 1/2 x 12 in
            result[PaperKind.GermanLegalFanfold] = new Size(12240, 18720); // German Legal Fanfold 8 1/2 x 13 in
            result[PaperKind.IsoB4] = new Size(14173, 20013); // B4 (ISO) 250 x 353 mm
            result[PaperKind.JapanesePostcard] = new Size(5669, 8391); // Japanese Postcard 100 x 148 mm
            result[PaperKind.Standard9x11] = new Size(12960, 15840); // 9 x 11 in
            result[PaperKind.Standard10x11] = new Size(14400, 15840); // 10 x 11 in
            result[PaperKind.Standard15x11] = new Size(21600, 15840); // 15 x 11 in
            result[PaperKind.InviteEnvelope] = new Size(12472, 12472); // Envelope Invite 220 x 220 mm
            result[PaperKind.LetterExtra] = new Size(13680, 17280); // Letter Extra 9 1/2 x 12 in
            result[PaperKind.LegalExtra] = new Size(13680, 21600); // Legal Extra 9 1/2 x 15 in
            result[PaperKind.TabloidExtra] = new Size(16834, 25920); // Tabloid Extra 11.69 x 18 in
            result[PaperKind.A4Extra] = new Size(13349, 18274); // A4 Extra 9.27 x 12.69 in
            result[PaperKind.LetterTransverse] = new Size(12240, 15840); // Letter Transverse 8 1/2 x 11 in
            result[PaperKind.A4Transverse] = new Size(11907, 16839); // A4 Transverse 210 x 297 mm
            result[PaperKind.LetterExtraTransverse] = new Size(13680, 17280); // Letter Extra Transverse 9 1/2 x 12 in
            result[PaperKind.APlus] = new Size(12869, 20183); // SuperA/SuperA/A4 227 x 356 mm
            result[PaperKind.BPlus] = new Size(17291, 27609); // SuperB/SuperB/A3 305 x 487 mm
            result[PaperKind.LetterPlus] = new Size(12240, 18274); // Letter Plus 8.5 x 12.69 in
            result[PaperKind.A4Plus] = new Size(11907, 18709); // A4 Plus 210 x 330 mm
            result[PaperKind.A5Transverse] = new Size(8391, 11907); // A5 Transverse 148 x 210 mm
            result[PaperKind.B5Transverse] = new Size(10319, 14571); // B5 (JIS) Transverse 182 x 257 mm
            result[PaperKind.A3Extra] = new Size(18255, 25228); // A3 Extra 322 x 445 mm
            result[PaperKind.A5Extra] = new Size(9865, 13323); // A5 Extra 174 x 235 mm
            result[PaperKind.B5Extra] = new Size(11395, 15647); // B5 (ISO) Extra 201 x 276 mm
            result[PaperKind.A2] = new Size(23811, 33676); // A2 420 x 594 mm
            result[PaperKind.A3Transverse] = new Size(16839, 23814); // A3 Transverse 297 x 420 mm
            result[PaperKind.A3ExtraTransverse] = new Size(18255, 25228); // A3 Extra Transverse 322 x 445 mm
            result[PaperKind.JapaneseDoublePostcard] = new Size(11339, 8391); // Japanese Double Postcard 200 x 148 mm
            result[PaperKind.A6] = new Size(5953, 8391); // A6 105 x 148 mm
            result[PaperKind.JapaneseEnvelopeKakuNumber2] = new Size(12240, 15840); // Japanese Envelope Kaku #2
            result[PaperKind.JapaneseEnvelopeKakuNumber3] = new Size(12240, 15840); // Japanese Envelope Kaku #3
            result[PaperKind.JapaneseEnvelopeChouNumber3] = new Size(12240, 15840); // Japanese Envelope Chou #3
            result[PaperKind.JapaneseEnvelopeChouNumber4] = new Size(12240, 15840); // Japanese Envelope Chou #4
            result[PaperKind.LetterRotated] = new Size(15840, 12240); // Letter Rotated 11 x 8 1/2 11 in
            result[PaperKind.A3Rotated] = new Size(23814, 16839); // A3 Rotated 420 x 297 mm
            result[PaperKind.A4Rotated] = new Size(16839, 11907); // A4 Rotated 297 x 210 mm
            result[PaperKind.A5Rotated] = new Size(11907, 8391); // A5 Rotated 210 x 148 mm
            result[PaperKind.B4JisRotated] = new Size(20636, 14570); // B4 (JIS) Rotated 364 x 257 mm
            result[PaperKind.B5JisRotated] = new Size(14570, 10318); // B5 (JIS) Rotated 257 x 182 mm
            result[PaperKind.JapanesePostcardRotated] = new Size(8391, 5669); // Japanese Postcard Rotated 148 x 100 mm
            result[PaperKind.JapaneseDoublePostcardRotated] = new Size(8391, 11339); // Double Japanese Postcard Rotated 148 x 200 mm
            result[PaperKind.A6Rotated] = new Size(8391, 5953); // A6 Rotated 148 x 105 mm
            result[PaperKind.JapaneseEnvelopeKakuNumber2Rotated] = new Size(12240, 15840); // Japanese Envelope Kaku #2 Rotated
            result[PaperKind.JapaneseEnvelopeKakuNumber3Rotated] = new Size(12240, 15840); // Japanese Envelope Kaku #3 Rotated
            result[PaperKind.JapaneseEnvelopeChouNumber3Rotated] = new Size(12240, 15840); // Japanese Envelope Chou #3 Rotated
            result[PaperKind.JapaneseEnvelopeChouNumber4Rotated] = new Size(12240, 15840); // Japanese Envelope Chou #4 Rotated
            result[PaperKind.B6Jis] = new Size(7257, 10318); // B6 (JIS) 128 x 182 mm
            result[PaperKind.B6JisRotated] = new Size(10318, 7257); // B6 (JIS) Rotated 182 x 128 mm
            result[PaperKind.Standard12x11] = new Size(17280, 15840); // 12 x 11 in
            result[PaperKind.JapaneseEnvelopeYouNumber4] = new Size(12240, 15840); // Japanese Envelope You #4
            result[PaperKind.JapaneseEnvelopeYouNumber4Rotated] = new Size(15840, 12240); // Japanese Envelope You #4 Rotated
            result[PaperKind.Prc16K] = new Size(8277, 12189); // PRC 16K 146 x 215 mm
            result[PaperKind.Prc32K] = new Size(5499, 8561); // PRC 32K 97 x 151 mm
            result[PaperKind.Prc32KBig] = new Size(5499, 8561); // PRC 32K(Big) 97 x 151 mm
            result[PaperKind.PrcEnvelopeNumber1] = new Size(5783, 9354); // PRC Envelope #1 102 x 165 mm
            result[PaperKind.PrcEnvelopeNumber2] = new Size(5783, 9978); // PRC Envelope #2 102 x 176 mm
            result[PaperKind.PrcEnvelopeNumber3] = new Size(7087, 9978); // PRC Envelope #3 125 x 176 mm
            result[PaperKind.PrcEnvelopeNumber4] = new Size(6236, 11792); // PRC Envelope #4 110 x 208 mm
            result[PaperKind.PrcEnvelopeNumber5] = new Size(6236, 12472); // PRC Envelope #5 110 x 220 mm
            result[PaperKind.PrcEnvelopeNumber6] = new Size(6803, 13039); // PRC Envelope #6 120 x 230 mm
            result[PaperKind.PrcEnvelopeNumber7] = new Size(9071, 13039); // PRC Envelope #7 160 x 230 mm
            result[PaperKind.PrcEnvelopeNumber8] = new Size(6803, 17518); // PRC Envelope #8 120 x 309 mm
            result[PaperKind.PrcEnvelopeNumber9] = new Size(12983, 18369); // PRC Envelope #9 229 x 324 mm
            result[PaperKind.PrcEnvelopeNumber10] = new Size(18369, 25965); // PRC Envelope #10 324 x 458 mm
            result[PaperKind.Prc16KRotated] = new Size(12189, 8277); // PRC 16K Rotated
            result[PaperKind.Prc32KRotated] = new Size(8561, 5499); // PRC 32K Rotated
            result[PaperKind.Prc32KBigRotated] = new Size(8561, 5499); // PRC 32K(Big) Rotated
            result[PaperKind.PrcEnvelopeNumber1Rotated] = new Size(9354, 5783); // PRC Envelope #1 Rotated 165 x 102 mm
            result[PaperKind.PrcEnvelopeNumber2Rotated] = new Size(9978, 5783); // PRC Envelope #2 Rotated 176 x 102 mm
            result[PaperKind.PrcEnvelopeNumber3Rotated] = new Size(9978, 7087); // PRC Envelope #3 Rotated 176 x 125 mm
            result[PaperKind.PrcEnvelopeNumber4Rotated] = new Size(11792, 6236); // PRC Envelope #4 Rotated 208 x 110 mm
            result[PaperKind.PrcEnvelopeNumber5Rotated] = new Size(12472, 6236); // PRC Envelope #5 Rotated 220 x 110 mm
            result[PaperKind.PrcEnvelopeNumber6Rotated] = new Size(13039, 6803); // PRC Envelope #6 Rotated 230 x 120 mm
            result[PaperKind.PrcEnvelopeNumber7Rotated] = new Size(13039, 9071); // PRC Envelope #7 Rotated 230 x 160 mm
            result[PaperKind.PrcEnvelopeNumber8Rotated] = new Size(17518, 6803); // PRC Envelope #8 Rotated 309 x 120 mm
            result[PaperKind.PrcEnvelopeNumber9Rotated] = new Size(18369, 12983); // PRC Envelope #9 Rotated 324 x 229 mm
            result[PaperKind.PrcEnvelopeNumber10Rotated] = new Size(25965, 18369); // PRC Envelope #10 Rotated 458 x 324 mm
            return result;
        }

        static calculatePaperSize(paperKind: PaperKind): Size {
            var result = PaperSizeConverter.paperSizeTable[paperKind];
            return result ? result : PaperSizeConverter.paperSizeTable[PaperKind.Letter];
        }
        static calculatePaperKind(size: Size, defaultValue: PaperKind, tolerance: number = 0): PaperKind {
            if(size.width == 0 || size.height == 0)
                return defaultValue;
            var entSize: Size;
            for(var paperKind in PaperSizeConverter.paperSizeTable) {
                if(!PaperSizeConverter.paperSizeTable.hasOwnProperty(paperKind))
                    continue;
                entSize = PaperSizeConverter.paperSizeTable[paperKind];
                if(Math.abs(size.width - entSize.width) <= tolerance && Math.abs(size.height - entSize.height) <= tolerance)
                    return parseInt(paperKind);
            }
            return defaultValue;
        }
    }
}