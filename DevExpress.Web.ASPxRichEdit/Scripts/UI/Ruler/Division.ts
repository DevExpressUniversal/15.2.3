module __aspxRichEdit {
    export module Ruler {
        export class DivisionInfo {
            map: number[];
            unitSize: number;
            stepSize: number;

            constructor(map: number[], unitSize: number) {
                this.map = map;
                this.unitSize = unitSize;
                this.stepSize = this.unitSize / this.map.length;
            }
        }
        export class DivisionsUnitHelper {
            currentUnitType: RichEditUnit;
            inchInfo: DivisionInfo;
            centimeterInfo: DivisionInfo;

            constructor(unitType: RichEditUnit) {
                this.currentUnitType = unitType;
                this.inchInfo = new DivisionInfo([0, 1, 1, 1, 2, 1, 1, 1], UnitConverter.inchesToPixels(1));
                this.centimeterInfo = new DivisionInfo([0, 1, 2, 1], UnitConverter.centimeterToPixel(1));
            }
            getUnitMap(): number[] {
                return this.currentUnitType == RichEditUnit.Inch ? this.inchInfo.map : this.centimeterInfo.map;
            }
            getUnitSize(): number {
                return this.currentUnitType == RichEditUnit.Inch ? this.inchInfo.unitSize : this.centimeterInfo.unitSize;
            }
            getStepSize(): number {
                return this.currentUnitType == RichEditUnit.Inch ? this.inchInfo.stepSize : this.centimeterInfo.stepSize;
            }
        }
    }
}