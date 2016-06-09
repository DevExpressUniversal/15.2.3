module __aspxRichEdit {
    export class SetLandscapePageOrientationCommand extends ChangeSectionPropertiesCommandBase<boolean> {
        executeCore(state: IntervalCommandStateEx, parameter: boolean): boolean {
            this.setNewValue(state.intervals, true);
            return true;
        }
        getHistoryItem(interval: FixedInterval, newValue: boolean): HistoryItem {
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            return new SectionLandscapeHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newValue);
        }
        setSectionPropertiesValue(properties: SectionProperties, value: boolean) {
            if (properties.landscape === value) return;
            properties.landscape = value;
            var temp = properties.pageWidth;
            properties.pageWidth = properties.pageHeight;
            properties.pageHeight = temp;
        }
        getSectionPropertiesValue(properties: SectionProperties): boolean {
            return properties.landscape === true;
        }
    }
} 