module __aspxRichEdit {
    class TableBorderInfo {
        compoundArray: number[]; // int even numbers
        drawingCompoundArray: number[] = []; // float
        widthDivider: number;
        widthMultiplier: number;
        lineCount: number;

        constructor(compoundArray: number[], widthDivider: number) {
            this.compoundArray = compoundArray;
            this.widthDivider = widthDivider;
            this.lineCount = Math.floor(compoundArray.length / 2);
            this.widthMultiplier = this.compoundArray[this.compoundArray.length - 1];
            for (let num of this.compoundArray)
                this.drawingCompoundArray.push(num / this.widthMultiplier);
        }

        public getActualWidth(borderWidth: number): number {
            return borderWidth * this.widthMultiplier / this.widthDivider;
        }
    }


    export class TableBorderCalculator {
        private static lineStyleInfo: { [borderLineStyle: number]: TableBorderInfo };

        constructor() {
            TableBorderCalculator.initStaticMembers();
        }

        private static initStaticMembers() {
            if (!!TableBorderCalculator.lineStyleInfo)
                return;
            TableBorderCalculator.lineStyleInfo = {};
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.Single] = new TableBorderInfo([0, 1], 1);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.Thick] = new TableBorderInfo([0, 1], 1);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.Double] = new TableBorderInfo([0, 1, 2, 3], 1);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.Dotted] = new TableBorderInfo([0, 1], 1);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.Dashed] = new TableBorderInfo([0, 1], 1);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.DotDash] = new TableBorderInfo([0, 1], 1);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.DotDotDash] = new TableBorderInfo([0, 1], 1);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.Triple] = new TableBorderInfo([0, 1, 2, 3, 4, 5], 1);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.ThinThickSmallGap] = new TableBorderInfo([0, 1, 2, 10], 8);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.ThickThinSmallGap] = new TableBorderInfo([0, 8, 9, 10], 8);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.ThinThickThinSmallGap] = new TableBorderInfo([0, 1, 2, 10, 11, 12], 8);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.ThinThickMediumGap] = new TableBorderInfo([0, 1, 2, 4], 2);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.ThickThinMediumGap] = new TableBorderInfo([0, 2, 3, 4], 2);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.ThinThickThinMediumGap] = new TableBorderInfo([0, 1, 2, 4, 5, 6], 2);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.ThinThickLargeGap] = new TableBorderInfo([0, 1, 9, 11], 8);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.ThickThinLargeGap] = new TableBorderInfo([0, 2, 10, 11], 8);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.ThinThickThinLargeGap] = new TableBorderInfo([0, 1, 9, 11, 19, 20], 8);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.Wave] = new TableBorderInfo([0, 1], 1);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.DoubleWave] = new TableBorderInfo([0, 1, 1, 2], 1);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.DashSmallGap] = new TableBorderInfo([0, 1], 1);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.DashDotStroked] = new TableBorderInfo([0, 1], 1);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.ThreeDEmboss] = new TableBorderInfo([0, 1, 1, 5, 6], 4);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.ThreeDEngrave] = new TableBorderInfo([0, 1, 1, 5, 6], 4);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.Outset] = new TableBorderInfo([0, 1], 1);
            TableBorderCalculator.lineStyleInfo[BorderLineStyle.Inset] = new TableBorderInfo([0, 1], 1);
        }

        public static getPowerfulBorder(aBorder: BorderInfo, bBorder: BorderInfo): BorderInfo {
            this.initStaticMembers();
            if (!aBorder) return bBorder;
            if (!bBorder) return aBorder;

            const aBorderWeight: number = TableBorderCalculator.getWeight(aBorder);
            const bBorderWeight: number = TableBorderCalculator.getWeight(bBorder);
            if (aBorderWeight > bBorderWeight) return aBorder;
            if (bBorderWeight > aBorderWeight) return bBorder;

            const aBorderStyleWeight: BorderLineStyle = aBorder.style;
            const bBorderStyleWeight: BorderLineStyle = bBorder.style;
            if (aBorderStyleWeight > bBorderStyleWeight) return aBorder;
            if (bBorderStyleWeight > aBorderStyleWeight) return bBorder;

            const aBorderColor: number = aBorder.color;
            const bBorderColor: number = bBorder.color;
            let aBorderBrightness: number = TableBorderCalculator.getBrightnessLevelOne(aBorderColor);
            let bBorderBrightness: number = TableBorderCalculator.getBrightnessLevelOne(bBorderColor);
            if (aBorderBrightness == bBorderBrightness) {
                aBorderBrightness = TableBorderCalculator.getBrightnessLevelTwo(aBorderColor);
                bBorderBrightness = TableBorderCalculator.getBrightnessLevelTwo(bBorderColor);
                if (aBorderBrightness == bBorderBrightness) {
                    aBorderBrightness = TableBorderCalculator.getBrightnessLevelThree(aBorderColor);
                    bBorderBrightness = TableBorderCalculator.getBrightnessLevelThree(bBorderColor);
                }
            }
            if (aBorderBrightness < bBorderBrightness) return aBorder;
            if (bBorderBrightness < aBorderBrightness) return bBorder;
            return aBorder;
        }

        public static getActualWidth(borderInfo: BorderInfo): number {
            const {actualLineStyle, info} = TableBorderCalculator.getActualBorderLineStyle(borderInfo.style);
            return info ? info.getActualWidth(borderInfo.width) : 0;
        }

        private static getBrightnessLevelOne(color: number ): number {
            return ColorHelper.getRed(color) + TableBorderCalculator.getBrightnessLevelTwo(color);
        }

        private static getBrightnessLevelTwo(color: number): number {
            return ColorHelper.getBlue(color) + 2 * ColorHelper.getGreen(color);
        }

        private static getBrightnessLevelThree(color: number): number {
            return ColorHelper.getGreen(color);
        }

        private static getWeight(borderInfo: BorderInfo): number {
            const borderStyle: BorderLineStyle = borderInfo.style;
            const { actualLineStyle, info } = TableBorderCalculator.getActualBorderLineStyle(borderStyle);
            return info ? (info.lineCount * borderStyle) : (borderStyle == BorderLineStyle.Disabled ? Number.MAX_VALUE : 0);
        }

        private static getActualBorderLineStyle(borderLineStyle: BorderLineStyle): { actualLineStyle: BorderLineStyle; info: TableBorderInfo} {
            if (borderLineStyle == BorderLineStyle.None || borderLineStyle == BorderLineStyle.Nil || borderLineStyle == BorderLineStyle.Disabled)
                return { actualLineStyle: borderLineStyle, info: null };

            const info: TableBorderInfo = TableBorderCalculator.lineStyleInfo[borderLineStyle];
            if (info)
                return { actualLineStyle: borderLineStyle, info: info};

            return { actualLineStyle: BorderLineStyle.Single, info: null };
        }
    }
}