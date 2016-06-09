module __aspxRichEdit {
    export class SetPortraitPageOrientationCommand extends ChangeSectionPropertiesCommandBase<boolean> {
        executeCore(state: IntervalCommandStateEx, parameter: boolean): boolean {
            this.setNewValue(state.intervals, false);
            return true;
        }
        getHistoryItem(interval: FixedInterval, newValue: boolean): HistoryItem {
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            return new SectionLandscapeHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newValue);
        }
        setSectionPropertiesValue(properties: SectionProperties, value: boolean) {
            properties.landscape = value;
        }
        getSectionPropertiesValue(properties: SectionProperties): boolean {
            return properties.landscape === false;
        }
    }
}