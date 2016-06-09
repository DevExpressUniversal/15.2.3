module __aspxRichEdit {
    export class SectionPropertiesManipulator {
        landscape: ISectionPropertyManipulator<boolean>;
        marginLeft: ISectionPropertyManipulator<number>;
        marginTop: ISectionPropertyManipulator<number>;
        marginRight: ISectionPropertyManipulator<number>;
        marginBottom: ISectionPropertyManipulator<number>;
        columnCount: ISectionPropertyManipulator<number>;
        space: ISectionPropertyManipulator<number>;
        equalWidthColumns: ISectionPropertyManipulator<boolean>;
        columnsInfo: ISectionPropertyManipulator<SectionColumnProperties[]>;
        pageWidth: ISectionPropertyManipulator<number>;
        pageHeight: ISectionPropertyManipulator<number>;
        startType: ISectionPropertyManipulator<SectionStartType>;
        differentFirstPage: ISectionPropertyManipulator<boolean>;
        dispatcher: ModelManipulator;

        constructor(manipulator: ModelManipulator) {
            this.landscape = new SectionPropertiesLandscapeManipulator(manipulator);
            this.marginBottom = new SectionPropertiesMarginBottomManipulator(manipulator);
            this.marginLeft = new SectionPropertiesMarginLeftManipulator(manipulator);
            this.marginRight = new SectionPropertiesMarginRightManipulator(manipulator);
            this.marginTop = new SectionPropertiesMarginTopManipulator(manipulator);
            this.columnCount = new SectionPropertiesColumnCountManipulator(manipulator);
            this.columnsInfo = new SectionPropertiesColumnsInfoManipulator(manipulator);
            this.equalWidthColumns = new SectionPropertiesEqualWidthColumnsManipulator(manipulator);
            this.pageHeight = new SectionPropertiesPageHeightManipulator(manipulator);
            this.pageWidth = new SectionPropertiesPageWidthManipulator(manipulator);
            this.space = new SectionPropertiesSpaceManipulator(manipulator);
            this.startType = new SectionPropertiesStartTypeManipulator(manipulator);
            this.differentFirstPage = new SectionPropertiesDifferentFirstPageManipulator(manipulator);
        }
    }

    class SectionPropertiesManipulatorBase<T> implements ISectionPropertyManipulator<T> {
        manipulator: ModelManipulator;
        constructor(manipulator: ModelManipulator) {
            this.manipulator = manipulator;
        }

        setValue(subDocument: SubDocument, interval: FixedInterval, newValue: T): HistoryItemState<HistoryItemSectionStateObject> {
            var oldState = new HistoryItemState<HistoryItemSectionStateObject>();
            if(!ControlOptions.isEnabled(subDocument.documentModel.options.sections))
                return oldState;
            var newState = new HistoryItemState<HistoryItemSectionStateObject>();

            var endPosition = Math.max(interval.start, interval.end() - 1);
            var sections = subDocument.documentModel.sections;

            var startSectionIndex = Utils.normedBinaryIndexOf(sections, section => section.startLogPosition.value - interval.start);
            var endSectionIndex = interval.length ? Utils.normedBinaryIndexOf(sections, section => section.startLogPosition.value - endPosition) : startSectionIndex;

            for(var i = startSectionIndex; i <= endSectionIndex; i++) {
                var section = sections[i];
                oldState.register(new HistoryItemSectionStateObject(i, this.getPropertyValue(section.sectionProperties)));
                newState.register(new HistoryItemSectionStateObject(i, newValue));
                this.setPropertyValue(section.sectionProperties, newValue);
            }
            this.manipulator.dispatcher.notifySectionFormattingChanged(sections[startSectionIndex], startSectionIndex, this.getJSONSectionProperty(), newState);
            return oldState;
        }

        restoreValue(subDocument: SubDocument, state: HistoryItemState<HistoryItemSectionStateObject>) {
            if(!ControlOptions.isEnabled(subDocument.documentModel.options.sections))
                return;
            if(state.isEmpty()) return;
            var sections = subDocument.documentModel.sections;

            for(var i = 0, stateObject: HistoryItemSectionStateObject; stateObject = state.objects[i]; i++) {
                var section = sections[stateObject.sectionIndex];
                this.setPropertyValue(section.sectionProperties, stateObject.value);
            }

            const sectionIndex: number = state.objects[0].sectionIndex;
            this.manipulator.dispatcher.notifySectionFormattingChanged(sections[sectionIndex], sectionIndex, this.getJSONSectionProperty(), state);
        }

        // must return copy of value
        getPropertyValue(properties: SectionProperties): T {
            throw new Error(Errors.NotImplemented);
        }

        setPropertyValue(properties: SectionProperties, value: T) {
            throw new Error(Errors.NotImplemented);
        }

        getJSONSectionProperty(): JSONSectionProperty{
            throw new Error(Errors.NotImplemented);
        }
    }

    class SectionPropertiesLandscapeManipulator extends SectionPropertiesManipulatorBase<boolean> {
        getPropertyValue(properties: SectionProperties): boolean {
            return properties.landscape;
        }
        setPropertyValue(properties: SectionProperties, value: boolean) {
            if (properties.landscape === value)
                return;
            properties.landscape = value;
            var temp = properties.pageWidth;
            properties.pageWidth = properties.pageHeight;
            properties.pageHeight = temp;
        }
        getJSONSectionProperty(): JSONSectionProperty {
            return JSONSectionProperty.Landscape;
        }
    }

    class SectionPropertiesMarginLeftManipulator extends SectionPropertiesManipulatorBase<number> {
        getPropertyValue(properties: SectionProperties): number {
            return properties.marginLeft;
        }
        setPropertyValue(properties: SectionProperties, value: number) {
            properties.marginLeft = value;
        }
        getJSONSectionProperty(): JSONSectionProperty {
            return JSONSectionProperty.MarginLeft;
        }
    }

    class SectionPropertiesMarginRightManipulator extends SectionPropertiesManipulatorBase<number> {
        getPropertyValue(properties: SectionProperties): number {
            return properties.marginRight;
        }
        setPropertyValue(properties: SectionProperties, value: number) {
            properties.marginRight = value;
        }
        getJSONSectionProperty(): JSONSectionProperty {
            return JSONSectionProperty.MarginRight;
        }
    }

    class SectionPropertiesMarginTopManipulator extends SectionPropertiesManipulatorBase<number> {
        getPropertyValue(properties: SectionProperties): number {
            return properties.marginTop;
        }
        setPropertyValue(properties: SectionProperties, value: number) {
            properties.marginTop = value;
        }
        getJSONSectionProperty(): JSONSectionProperty {
            return JSONSectionProperty.MarginTop;
        }
    }

    class SectionPropertiesMarginBottomManipulator extends SectionPropertiesManipulatorBase<number> {
        getPropertyValue(properties: SectionProperties): number {
            return properties.marginBottom;
        }
        setPropertyValue(properties: SectionProperties, value: number) {
            properties.marginBottom = value;
        }
        getJSONSectionProperty(): JSONSectionProperty {
            return JSONSectionProperty.MarginBottom;
        }
    }

    class SectionPropertiesColumnCountManipulator extends SectionPropertiesManipulatorBase<number> {
        getPropertyValue(properties: SectionProperties): number {
            return properties.columnCount;
        }
        setPropertyValue(properties: SectionProperties, value: number) {
            properties.columnCount = value;
        }
        getJSONSectionProperty(): JSONSectionProperty {
            return JSONSectionProperty.ColumnCount;
        }
    }

    class SectionPropertiesSpaceManipulator extends SectionPropertiesManipulatorBase<number> {
        getPropertyValue(properties: SectionProperties): number {
            return properties.space;
        }
        setPropertyValue(properties: SectionProperties, value: number) {
            properties.space = value;
        }
        getJSONSectionProperty(): JSONSectionProperty {
            return JSONSectionProperty.Space;
        }
    }

    class SectionPropertiesEqualWidthColumnsManipulator extends SectionPropertiesManipulatorBase<boolean> {
        getPropertyValue(properties: SectionProperties): boolean {
            return properties.equalWidthColumns;
        }
        setPropertyValue(properties: SectionProperties, value: boolean) {
            properties.equalWidthColumns = value;
        }
        getJSONSectionProperty(): JSONSectionProperty {
            return JSONSectionProperty.EqualWidthColumns;
        }
    }
    class SectionPropertiesColumnsInfoManipulator extends SectionPropertiesManipulatorBase<SectionColumnProperties[]> {
        getPropertyValue(properties: SectionProperties): SectionColumnProperties[] {
            return properties.columnsInfo;
        }
        setPropertyValue(properties: SectionProperties, value: SectionColumnProperties[]) {
            properties.columnsInfo = value;
        }
        getJSONSectionProperty(): JSONSectionProperty {
            return JSONSectionProperty.ColumnsInfo;
        }
    }

    class SectionPropertiesPageWidthManipulator extends SectionPropertiesManipulatorBase<number> {
        getPropertyValue(properties: SectionProperties): number {
            return properties.pageWidth;
        }
        setPropertyValue(properties: SectionProperties, value: number) {
            properties.pageWidth = value;
        }
        getJSONSectionProperty(): JSONSectionProperty {
            return JSONSectionProperty.PageWidth;
        }
    }

    class SectionPropertiesPageHeightManipulator extends SectionPropertiesManipulatorBase<number> {
        getPropertyValue(properties: SectionProperties): number {
            return properties.pageHeight;
        }
        setPropertyValue(properties: SectionProperties, value: number) {
            properties.pageHeight = value;
        }
        getJSONSectionProperty(): JSONSectionProperty {
            return JSONSectionProperty.PageHeight;
        }
    }

    class SectionPropertiesStartTypeManipulator extends SectionPropertiesManipulatorBase<SectionStartType> {
        getPropertyValue(properties: SectionProperties): SectionStartType {
            return properties.startType;
        }
        setPropertyValue(properties: SectionProperties, value: SectionStartType) {
            properties.startType = value;
        }
        getJSONSectionProperty(): JSONSectionProperty {
            return JSONSectionProperty.StartType;
        }
    }

    class SectionPropertiesDifferentFirstPageManipulator extends SectionPropertiesManipulatorBase<boolean> {
        getPropertyValue(properties: SectionProperties): boolean {
            return properties.differentFirstPage;
        }
        setPropertyValue(properties: SectionProperties, value: boolean) {
            properties.differentFirstPage = value;
        }
        getJSONSectionProperty(): JSONSectionProperty {
            return JSONSectionProperty.DifferentFirstPage;
        }
    }
} 