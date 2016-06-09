module __aspxRichEdit {
    export class DialogColumnsCommand extends ShowDialogCommandBase {
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.sections) && this.control.model.activeSubDocument.isMain();
        }
        createParameters(): ColumnsDialogParameters {
            var columnsInfo: ColumnsInfoUI = new ColumnsInfoUI();
            var secProps: SectionProperties = this.control.inputPosition.getMergedSectionPropertiesRaw();
            columnsInfo.equalColumnWidth = secProps.equalWidthColumns;
            if (secProps.pageWidth != undefined && secProps.marginLeft != undefined && secProps.marginRight != undefined)
                columnsInfo.pageWidth = secProps.pageWidth - secProps.marginLeft - secProps.marginRight;
            else
                columnsInfo.pageWidth = ColumnsInfoUI.minColumnWidth;
            var columnCount: number = secProps.columnCount == undefined ? 0 : secProps.columnCount;
            columnsInfo.changeColumnCount(columnCount);
            for(var i = 0, info; info = secProps.columnsInfo[i]; i++) {
                columnsInfo.columns[i].width = info.width;
                columnsInfo.columns[i].spacing = info.space;
            }

            var parameters: ColumnsDialogParameters = new ColumnsDialogParameters();
            parameters.columnsInfo = columnsInfo;
            parameters.unitConverter = this.control.units;
            return parameters;
        }
        applyParameters(state: IntervalCommandStateEx, newParams: ColumnsDialogParameters) {
            var interval = this.getInterval(newParams.columnsInfo.applyType);
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            var columnsInfo = newParams.columnsInfo;

            this.control.history.beginTransaction();
            var columns: SectionColumnProperties[] = [];
            for (var i = 0, columnInfo: ColumnInfoUI; columnInfo = columnsInfo.columns[i]; i++) {
                var column = new SectionColumnProperties(columnInfo.width, columnInfo.spacing);
                columns.push(column);
            }
            this.control.history.addAndRedo(new SectionColumnsInfoHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, columns));
            this.control.history.addAndRedo(new SectionColumnCountHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, columnsInfo.columnCount));
            this.control.history.addAndRedo(new SectionEqualWidthColumnsHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, columnsInfo.equalColumnWidth));
            this.control.history.endTransaction();
        }
        getInterval(applyTo: SectionPropertiesApplyType): FixedInterval {
            if(applyTo == SectionPropertiesApplyType.WholeDocument)
                return new FixedInterval(0, this.control.model.mainSubDocument.getDocumentEndPosition() - 1);
            var selectedSections = this.control.model.getSectionsByInterval(this.control.selection.getLastSelectedInterval());
            if(applyTo == SectionPropertiesApplyType.SelectedSections) {
                var lastSection = selectedSections[selectedSections.length - 1];
                return FixedInterval.fromPositions(selectedSections[0].startLogPosition.value, lastSection.startLogPosition.value + lastSection.getLength() - 1);
            }
            if(applyTo == SectionPropertiesApplyType.ThisPointForward)
                return FixedInterval.fromPositions(selectedSections[0].startLogPosition.value, this.control.model.mainSubDocument.getDocumentEndPosition());
            return new FixedInterval(selectedSections[0].startLogPosition.value, selectedSections[0].getLength() - 1);
        }
        getDialogName() {
            return "Columns";
        }
        lockInputPositionUpdating(prevModifiedState: IsModified): boolean {
            return false;
        }
    }

    export class ColumnsDialogParameters extends DialogParametersBase {
        columnsInfo: ColumnsInfoUI;
        unitConverter: UnitConverter;

        copyFrom(obj: ColumnsDialogParameters) {
            var columnsInfo = new ColumnsInfoUI();
            columnsInfo.copyFrom(obj.columnsInfo);
            this.columnsInfo = columnsInfo;
            this.unitConverter = obj.unitConverter;
        }

        getNewInstance(): DialogParametersBase {
            return new ColumnsDialogParameters();
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
        }
    }

    export class ColumnsInfoUI implements ICloneable<ColumnsInfoUI>, ISupportCopyFrom<ColumnsInfoUI> {
        static minColumnWidth: number = 720;
        static minSpacingWidth: number = 0;

        columns: ColumnInfoUI[] = [];
        columnCount: number;
        equalColumnWidth: boolean = false;
        pageWidth: number;
        applyType: SectionPropertiesApplyType = SectionPropertiesApplyType.WholeDocument;

        getMaxColumnCount(): number {
            return Math.ceil(this.pageWidth / (ColumnsInfoUI.minColumnWidth + ColumnsInfoUI.minSpacingWidth));
        }
        hasColumnsNull(): boolean {
            if(this.columnCount > this.columns.length)
                return true;
            for(var i = 0; i < this.columnCount; i++) {
                if(!this.columns[i].width)
                    return true;
                if(!this.columns[i].spacing && this.columns[i].spacing != 0)
                    return true;
            }
            return false;
        }
        hasColumnsInfoUINull(): boolean {
            if(!this.columnCount)
                return true;
            return this.hasColumnsNull();
        }

        changeColumnCount(count: number) {
            if(count < 0)
                return;
            count = Math.min(count, this.getMaxColumnCount());

            var previousCount: number = this.columns.length;
            var hasColumnInfoUINull: boolean = this.hasColumnsInfoUINull();

            for(var i: number = this.columns.length; i < count; i++)
                this.columns[i] = new ColumnInfoUI(i + 1);
            this.columns.splice(count);

            this.columnCount = count;

            if(hasColumnInfoUINull) {
                this.calculateEqualColumnsOnChangeCount();
                return;
            }
            if(!this.equalColumnWidth && previousCount > 0)
                this.calculateNotEqualColumnsOnChangeCount(previousCount);
            else
                this.calculateEqualColumnsOnChangeCount();
        }
        calculateEqualColumnsOnChangeCount() {
            if(this.columns.length <= 0)
                return;
            var spacingValue: number;
            if(this.columns[0].spacing && this.columns[0].spacing != 0) // has value
                spacingValue = this.columns[0].spacing;
            else
                spacingValue = ColumnsInfoUI.minColumnWidth;

            this.calculateUniformColumnsByColumnSpacing(spacingValue);
        }
        calculateNotEqualColumnsOnChangeCount(previousCount: number) {
            if(this.columns.length <= 0)
                return;
            if(this.columns.length == 1)
                this.columns[0].width = this.pageWidth;

            var calculateCount: number = Math.min(previousCount, this.columns.length);

            for(var i = 0; i < calculateCount; i++)
                this.columns[i].width = Math.max(ColumnsInfoUI.minColumnWidth, this.columns[i].width * previousCount / this.columns.length);
            for(var i = 0; i < calculateCount - 1; i++)
                this.columns[i].spacing = Math.max(ColumnsInfoUI.minSpacingWidth, this.columns[i].spacing * (previousCount - 1) / (this.columns.length - 1));
            if(calculateCount > 0)
                for(var i = calculateCount; i < this.columns.length; i++)
                    this.columns[i].width = this.columns[calculateCount - 1].width;
            if(calculateCount > 1)
                for(var i = calculateCount - 1; i < this.columns.length - 1; i++)
                    this.columns[i].spacing = this.columns[calculateCount - 2].spacing;

            this.disableTheLastSpacing();
            this.correctColumns();
        }
        correctColumns() {
            if(!this.columnCount || this.columnCount <= 0)
                return;
            var difference: number = -this.calculateAvailableSpace();
            var calculatorWidth: ColumnsDistributionCalculator = new ColumnsDistributionWidthPriorityCalculator(this.columns);
            var calculatorSpacing: ColumnsDistributionCalculator = new ColumnsDistributionSpacingPriorityCalculator(this.columns);
            var sumWidth: number = calculatorWidth.calculateTotal(0, this.columns.length - 1);
            var sumSpacing: number = calculatorSpacing.calculateTotal(0, this.columns.length - 1);
            var partWidth: number = sumWidth / (sumWidth + sumSpacing); //double
            var differenceWidth: number = Math.ceil(difference * partWidth);
            var differenceSpacing: number = difference - differenceWidth;
            calculatorWidth.distributeSpace(0, this.columns.length - 1, differenceWidth);
            calculatorSpacing.distributeSpace(0, this.columns.length - 2, differenceSpacing);
        }
        disableTheLastSpacing() {
            this.columns[this.columns.length - 1].spacing = 0;
        }
        recalculateColumnsByWidthAfterIndex(index: number) {
            if(this.hasColumnsInfoUINull())
                return;
            if(this.equalColumnWidth)
                this.calculateColumnWidthForUniformColumns();
            else
                this.changeColumnsNotEqualByWidthAfterIndex(index);
        }
        recalculateColumnsBySpacingAfterIndex(index: number) {
            if(this.hasColumnsInfoUINull())
                return;
            if(this.equalColumnWidth)
                this.calculateColumnSpacingForUniformColumns();
            else
                this.changeColumnsNotEqualBySpacingAfterIndex(index);
        }

        calculateUniformColumnsCore(columnWidth: number, columnSpacing: number, restWidth: number, restSpacing: number) {
            var calculatorWidth: ColumnsDistributionCalculator = new ColumnsDistributionWidthPriorityCalculator(this.columns);
            var calculatorSpacing: ColumnsDistributionCalculator = new ColumnsDistributionSpacingPriorityCalculator(this.columns);

            calculatorWidth.setAllValues(columnWidth, restWidth);
            calculatorSpacing.setAllValues(columnSpacing, restSpacing);

            this.disableTheLastSpacing();
        }
        calculateColumnWidthForUniformColumns() {
            var columnWidth: number = (this.columns[0].width) ? this.columns[0].width : ColumnsInfoUI.minColumnWidth; // HasValue
            this.calculateUniformColumnsByColumnWidth(columnWidth);
        }
        calculateUniformColumnsByColumnWidth(columnWidth: number) {
            if(!this.columnCount || this.columnCount <= 0)
                return;
            if(this.columnCount <= 1)
                columnWidth = this.pageWidth;

            if(columnWidth * this.columnCount > this.pageWidth)
                columnWidth = this.pageWidth / this.columnCount;

            columnWidth = Math.max(columnWidth, ColumnsInfoUI.minColumnWidth);

            var dividend = this.pageWidth - columnWidth * this.columnCount;
            var divider = Math.max(1, this.columnCount - 1);
            var restSpacing = dividend % divider;
            var columnSpacing = dividend / divider;

            this.calculateUniformColumnsCore(columnWidth, columnSpacing, 0, restSpacing);
        }
        calculateColumnSpacingForUniformColumns() {
            if(this.hasColumnsInfoUINull())
                return;
            var columnSpacing = (this.columns[0].spacing) ? this.columns[0].spacing : ColumnsInfoUI.minSpacingWidth;
            this.calculateUniformColumnsByColumnSpacing(columnSpacing);
        }
        calculateUniformColumnsByColumnSpacing(columnSpacing: number) {
            if(!this.columnCount || this.columnCount <= 0)
                return;
            columnSpacing = Math.max(columnSpacing, ColumnsInfoUI.minSpacingWidth);

            if(columnSpacing * (this.columnCount - 1) > this.pageWidth - ColumnsInfoUI.minColumnWidth * this.columnCount)
                columnSpacing = Math.ceil((this.pageWidth - ColumnsInfoUI.minColumnWidth * this.columnCount) / (this.columnCount - 1));

            if(this.columnCount == 1)
                columnSpacing = 0;

            var dividend: number = Math.ceil(this.pageWidth - columnSpacing * (this.columnCount - 1));
            var restWidth: number = Math.ceil(dividend % this.columnCount);
            var columnWidth: number = Math.ceil(dividend / this.columnCount);

            this.calculateUniformColumnsCore(columnWidth, columnSpacing, restWidth, 0);
        }
        calculateAvailableSpace(): number {
            var usedSpace = 0;
            for(var i = 0; i < this.columnCount; i++)
                usedSpace += ((this.columns[i].width) ? this.columns[i].width : 0) + ((this.columns[i].spacing) ? this.columns[i].spacing : 0);
            return this.pageWidth - usedSpace;
        }
        changeColumnsNotEqualByWidthAfterIndex(index: number) {
            if(!this.columnCount || this.columnCount <= 0 || index >= this.columnCount)
                return;

            var calculatorWidth: ColumnsDistributionCalculator = new ColumnsDistributionWidthPriorityCalculator(this.columns);
            var calculatorSpacing: ColumnsDistributionCalculator = new ColumnsDistributionSpacingPriorityCalculator(this.columns);
            calculatorWidth.correctValue(index);
            var difference = -this.calculateAvailableSpace();
            difference = calculatorWidth.distributeSpace(index + 1, this.columnCount - 1, difference);
            difference = calculatorWidth.distributeSpace(0, index - 1, difference);
            difference = calculatorSpacing.distributeSpace(0, this.columnCount - 2, difference);
            this.columns[index].width -= difference;
            this.disableTheLastSpacing();
        }
        changeColumnsNotEqualBySpacingAfterIndex(index: number) {
            if(!this.columnCount || this.columnCount <= 0 || index >= this.columnCount)
                return;
            var calculatorWidth: ColumnsDistributionCalculator = new ColumnsDistributionWidthPriorityCalculator(this.columns);
            var calculatorSpacing: ColumnsDistributionCalculator = new ColumnsDistributionSpacingPriorityCalculator(this.columns);
            calculatorSpacing.correctValue(index);
            var difference = -this.calculateAvailableSpace();
            difference = calculatorWidth.distributeSpace(index + 1, this.columnCount - 1, difference);
            difference = calculatorWidth.distributeSpace(0, index, difference);
            difference = calculatorSpacing.distributeSpace(0, index - 1, difference);
            difference = calculatorSpacing.distributeSpace(index + 1, this.columnCount - 2, difference);
            this.columns[index].spacing -= difference;
            this.disableTheLastSpacing();
        }
        clone(): ColumnsInfoUI {
            var obj = new ColumnsInfoUI();
            obj.copyFrom(this);
            return obj;
        }
        copyFrom(info: ColumnsInfoUI) {
            this.applyType = info.applyType;
            this.equalColumnWidth = info.equalColumnWidth;
            this.pageWidth = info.pageWidth;
            this.changeColumnCount(info.columns.length);
            for(var i = 0; i < this.columns.length; i++) {
                this.columns[i].width = info.columns[i].width;
                this.columns[i].spacing = info.columns[i].spacing;
            }
        }
    }

    export class ColumnInfoUI {
        num: number;
        width: number;
        spacing: number;

        constructor(num: number) {
            this.num = num;
        }
    }

    export class ColumnsDistributionCalculator {
        private columns: ColumnInfoUI[] = [];

        constructor(columns: ColumnInfoUI[]) {
            this.columns = columns;
        }

        calculateTotal(from: number, to: number): number {
            var result: number = 0;
            for(var i = from; i <= to; i++)
                result += this.getValue(this.columns[i]);
            return result;
        }
        hasEnoughSpaceForDistribution(from: number, to: number, space: number): boolean {
            var total: number = this.calculateTotal(from, to);
            return space < total - this.getMinValue() * (to - from + 1);
        }
        setMinValues(from: number, to: number, space: number) {
            for(var i = from; i <= to; i++) {
                space -= this.getValue(this.columns[i]) - this.getMinValue();
                this.setValue(this.columns[i], this.getMinValue());
            }
            return space;
        }
        correctValue(index: number) {
            if(index >= this.columns.length)
                return;
            if(this.getValue(this.columns[index]) < this.getMinValue())
                this.setValue(this.columns[index], this.getMinValue());
        }
        distributeRemainder(from: number, to: number, remainder: number): number {
            var correction: number = (remainder > 0) ? 1 : -1;
            while (remainder != 0) {
                for(var i = from; i <= to && (remainder != 0); i++) {
                    var newValue = this.getValue(this.columns[i]) - correction;
                    if(newValue > this.getMinValue()) {
                        this.setValue(this.columns[i], newValue);
                        remainder -= correction;
                    }
                }
            }
            return 0;
        }
        distributeSpaceCore(from: number, to: number, space: number): number {
            var remainder: number = Math.round(space % (to - from + 1));
            var difference: number = Math.round(space / (to - from + 1));

            for(var i = from; i <= to; i++) {
                var newValue: number = this.getValue(this.columns[i]) - difference;
                if(newValue >= this.getMinValue())
                    this.setValue(this.columns[i], newValue);
                else {
                    this.setValue(this.columns[i], this.getMinValue());
                    remainder += (this.getMinValue() - newValue);
                }
            }
            this.distributeRemainder(from, to, remainder);
            return 0;
        }
        distributeSpace(from: number, to: number, space: number): number {
            if(from > to)
                return space;
            if(this.hasEnoughSpaceForDistribution(from, to, space))
                return this.distributeSpaceCore(from, to, space);
            else
                return this.setMinValues(from, to, space);
        }
        setAllValues(value: number, rest: number) {
            var count: number = this.columns.length;
            for(var i: number = 0; i < count; i++)
                this.setValue(this.columns[i], value);
            this.distributeSpace(0, count - 1, -rest);
        }

        getMinValue(): number { return null; }
        getValue(column: ColumnInfoUI): number { return null; }
        setValue(column: ColumnInfoUI, value: number) { }
    }
    export class ColumnsDistributionWidthPriorityCalculator extends ColumnsDistributionCalculator {
        getMinValue(): number {
            return 720;
        }
        getValue(column: ColumnInfoUI): number {
            return (column.width) ? column.width : 0;
        }
        setValue(column: ColumnInfoUI, value: number) {
            column.width = value;
        }
    }
    export class ColumnsDistributionSpacingPriorityCalculator extends ColumnsDistributionCalculator {
        getMinValue(): number {
            return 0;
        }
        getValue(column: ColumnInfoUI): number {
            return (column.spacing) ? column.spacing : 0;
        }
        setValue(column: ColumnInfoUI, value: number) {
            column.spacing = value;
        }
    }

    export class ColumnsEditorController {
        columnsInfo: ColumnsInfoUI;
        unitConverter: UnitConverter;
        presets: Array<ColumnsInfoPreset> = [];

        constructor(parameters: ColumnsDialogParameters) {
            this.columnsInfo = parameters.columnsInfo;
            this.unitConverter = parameters.unitConverter;
            
            this.presets.push(new SingleColumnsInfoPreset());
            this.presets.push(new TwoColumnsInfoPreset());
            this.presets.push(new ThreeColumnsInfoPreset());
            this.presets.push(new LeftNarrowColumnsInfoPreset());
            this.presets.push(new RightNarrowColumnsInfoPreset());
        }

        changeColumnCount(count: number) {
            this.columnsInfo.changeColumnCount(count);
        }
        setEqualColumnWidth(value: boolean) {
            this.columnsInfo.equalColumnWidth = value;
            if(value)
                this.columnsInfo.calculateColumnSpacingForUniformColumns();
        }
        applyPreset(index: number) {
            this.presets[index].applyTo(this.columnsInfo);
        }
        matchPreset(index: number): boolean {
            return this.presets[index].matchTo(this.columnsInfo);
        }
        getWidth(index: number): number {
            var width = this.columnsInfo.columns[index].width;
            return this.unitConverter.twipsToUI(width);
        }
        getSpacing(index: number): number {
            var spacing = this.columnsInfo.columns[index].spacing;
            return this.unitConverter.twipsToUI(spacing);
        }
        setWidth(index: number, value: number) {
            var width = this.unitConverter.UIToTwips(value);
            this.columnsInfo.columns[index].width = width;
            this.columnsInfo.recalculateColumnsByWidthAfterIndex(index);
        }
        setSpacing(index: number, value: number) {
            var spacing = this.unitConverter.UIToTwips(value);
            this.columnsInfo.columns[index].spacing = spacing;
            this.columnsInfo.recalculateColumnsBySpacingAfterIndex(index);
        }
    }

    export class ColumnsInfoPreset {
        getSpacing(): number { return 1800; }

        applyTo(columnsInfo: ColumnsInfoUI) { }
        matchTo(columnsInfo: ColumnsInfoUI): boolean { return false; }
    }
    export class UniformColumnsInfoPreset extends ColumnsInfoPreset {
        getColumnCount(): number { return null; }
        matchTo(columnsInfo: ColumnsInfoUI): boolean {
            if(!columnsInfo.equalColumnWidth)
                return false;
            if(!columnsInfo.columnCount)
                return false;
            return columnsInfo.columnCount == this.getColumnCount();
        }
        applyTo(columnsInfo: ColumnsInfoUI) {
            columnsInfo.equalColumnWidth = true;
            if(columnsInfo.columns.length > 0)
                columnsInfo.columns[0].spacing = this.getSpacing();
            columnsInfo.changeColumnCount(this.getColumnCount());
        }
    }
    export class SingleColumnsInfoPreset extends UniformColumnsInfoPreset {
        getColumnCount(): number { return 1; }
    }
    export class TwoColumnsInfoPreset extends UniformColumnsInfoPreset {
        getColumnCount(): number { return 2; }
    }
    export class ThreeColumnsInfoPreset extends UniformColumnsInfoPreset {
        getColumnCount(): number { return 3; }
    }
    export class TwoNonUniformColumnsInfoPreset extends ColumnsInfoPreset {
        getFirstColumnRelativeWidth(): number { return null; }

        matchTo(columnsInfo: ColumnsInfoUI): boolean {
            if(columnsInfo.equalColumnWidth)
                return false;
            if(columnsInfo.columnCount != 2)
                return false;
            if(columnsInfo.columns.length != 2)
                return false;

            if(!columnsInfo.columns[0].width)
                return false;
            if(!columnsInfo.columns[0].spacing && columnsInfo.columns[1].spacing != 0)
                return false;
            if(!columnsInfo.columns[1].width)
                return false;
            if(!columnsInfo.columns[1].spacing && columnsInfo.columns[1].spacing != 0)
                return false;

            var totalWidth = columnsInfo.pageWidth - this.getSpacing();
            if(columnsInfo.columns[0].width != Math.round(totalWidth * this.getFirstColumnRelativeWidth()))
                return false;
            if(columnsInfo.columns[0].spacing != this.getSpacing())
                return false;
            if(columnsInfo.columns[1].width != Math.round(totalWidth - columnsInfo.columns[0].width))
                return false;

            return columnsInfo.columns[1].spacing == 0;
        }
        applyTo(columnsInfo: ColumnsInfoUI) {
            columnsInfo.equalColumnWidth = false;
            columnsInfo.changeColumnCount(2);

            var totalWidth = columnsInfo.pageWidth - this.getSpacing();
            columnsInfo.columns[0].width = Math.round(totalWidth * this.getFirstColumnRelativeWidth());
            columnsInfo.columns[0].spacing = this.getSpacing();
            columnsInfo.columns[1].width = Math.round(totalWidth - columnsInfo.columns[0].width);
            columnsInfo.columns[1].spacing = 0;
        }
    }
    export class LeftNarrowColumnsInfoPreset extends TwoNonUniformColumnsInfoPreset {
        getFirstColumnRelativeWidth(): number { return 0.292; }
    }
    export class RightNarrowColumnsInfoPreset extends TwoNonUniformColumnsInfoPreset {
        getFirstColumnRelativeWidth(): number { return 0.708; }
    }
}