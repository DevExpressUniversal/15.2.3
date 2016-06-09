module __aspxRichEdit {
    export class TableCustomUnit<ValueTypeVariants> implements IEquatable<TableCustomUnit<ValueTypeVariants>> {
        value: number;
        type: ValueTypeVariants;

        public equals(obj: TableCustomUnit<ValueTypeVariants>): boolean {
            if (!obj)
                return false;
            return this.value == obj.value && this.type == obj.type;
        }

        public copyFrom(obj: any) {
            this.value = obj.value;
            this.type = obj.type;
        }
    }
    export class TableWidthUnit extends TableCustomUnit<TableWidthUnitType> implements ICloneable<TableWidthUnit> {
        public static MAX_PERCENT_WIDTH: number = 5000;

        static createDefault(): TableWidthUnit {
            return new TableWidthUnit().init(0, TableWidthUnitType.Nil);
        }
        static create(value: number, type: TableWidthUnitType): TableWidthUnit {
            return new TableWidthUnit().init(value, type);
        }

        init(value: number, type: TableWidthUnitType): TableWidthUnit {
            this.value = value;
            this.type = type;
            return this;
        }

        public clone(): TableWidthUnit {
            return new TableWidthUnit().init(this.value, this.type);
        }

        public asNumberNoPercentType(unitConverter: IUnitConverter): number {
            switch (this.type) {
                case TableWidthUnitType.Nil:
                    return 0;
                case TableWidthUnitType.Auto:
                case TableWidthUnitType.ModelUnits:
                    return unitConverter.toPixels(this.value);
                case TableWidthUnitType.FiftiethsOfPercent:
                    throw new Error(Errors.InternalException);
            }
        }
        public asNumber(unitConverter: IUnitConverter, avaliableWidth: number): number {
            switch (this.type) {
                case TableWidthUnitType.Nil:
                    return 0;
                case TableWidthUnitType.Auto:
                case TableWidthUnitType.ModelUnits:
                    return unitConverter.toPixels(this.value);
                case TableWidthUnitType.FiftiethsOfPercent:
                    return avaliableWidth * this.value / TableWidthUnit.MAX_PERCENT_WIDTH;
            }
        }

        public divide(n: number) {
            if (n > 1)
                this.value = this.value / n;
        }
    }

    export class TableHeightUnit extends TableCustomUnit<TableHeightUnitType> implements ICloneable<TableHeightUnit> {
        init(value: number, type: TableHeightUnitType): TableHeightUnit {
            this.value = value;
            this.type = type;
            return this;
        }

        public clone(): TableHeightUnit {
            return new TableHeightUnit().init(this.value, this.type);
        }

        static create(value: number, type: TableHeightUnitType): TableHeightUnit {
            return new TableHeightUnit().init(value, type);
        }

        static createDefault(): TableHeightUnit {
            return new TableHeightUnit().init(0, TableHeightUnitType.Auto);
        }
    }

    export enum TableHeightUnitType {
        Minimum,
        Auto,
        Exact
    }

    export enum TableWidthUnitType {
        Nil = 0, //No Width (=0)
        Auto = 1,
        FiftiethsOfPercent = 2, //Width in Fiftieths of a Percent
        ModelUnits = 3
    }
} 