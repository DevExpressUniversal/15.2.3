module __aspxRichEdit {
    export class ListLevelPropertiesManipulator {
        start: IListLevelPropertyManipulator<number>;
        format: IListLevelPropertyManipulator<NumberingFormat>;
        alignment: IListLevelPropertyManipulator<ListNumberAlignment>;
        convertPreviousLevelNumberingToDecimal: IListLevelPropertyManipulator<boolean>;
        separator: IListLevelPropertyManipulator<string>;
        suppressRestart: IListLevelPropertyManipulator<boolean>;
        suppressBulletResize: IListLevelPropertyManipulator<boolean>;
        displayFormatString: IListLevelPropertyManipulator<string>;
        relativeRestartLevel: IListLevelPropertyManipulator<number>;
        templateCode: IListLevelPropertyManipulator<number>;
        originalLeftIndent: IListLevelPropertyManipulator<number>;
        legacy: IListLevelPropertyManipulator<boolean>;
        legacySpace: IListLevelPropertyManipulator<number>;
        legacyIndent: IListLevelPropertyManipulator<number>;

        constructor(manipulator: ModelManipulator) {
            this.start = new StartListLevelPropertiesManipulator(manipulator);
            this.format = new FormatListLevelPropertiesManipulator(manipulator);
            this.alignment = new AlignmentListLevelPropertiesManipulator(manipulator);
            this.convertPreviousLevelNumberingToDecimal = new ConvertPreviousLevelNumberingToDecimalListLevelPropertiesManipulator(manipulator);
            this.separator = new SeparatorListLevelPropertiesManipulator(manipulator);
            this.suppressRestart = new SuppressRestartListLevelPropertiesManipulator(manipulator);
            this.suppressBulletResize = new SuppressBulletResizeListLevelPropertiesManipulator(manipulator);
            this.displayFormatString = new DisplayFormatStringListLevelPropertiesManipulator(manipulator);
            this.relativeRestartLevel = new RelativeRestartLevelListLevelPropertiesManipulator(manipulator);
            this.templateCode = new TemplateCodeListLevelPropertiesManipulator(manipulator);
            this.originalLeftIndent = new OriginalLeftIndentListLevelPropertiesManipulator(manipulator);
            this.legacy = new LegacyListLevelPropertiesManipulator(manipulator);
            this.legacySpace = new LegacySpaceListLevelPropertiesManipulator(manipulator);
            this.legacyIndent = new LegacyIndentListLevelPropertiesManipulator(manipulator);
        }
    }

    class ListLevelPropertiesManipulatorBase<T> implements IListLevelPropertyManipulator<T> {
        private manipulator: ModelManipulator;
        constructor(dispatcher: ModelManipulator) {
            this.manipulator = dispatcher;
        }

        setValue(model: DocumentModel, isAbstractList: boolean, listIndex: number, listLevelIndex: number, newValue: T): HistoryItemState<HistoryItemListLevelStateObject> {
            var oldState = new HistoryItemState<HistoryItemListLevelStateObject>();
            var newState = new HistoryItemState<HistoryItemListLevelStateObject>();

            var numberingList: NumberingListBase<IListLevel> = isAbstractList ? model.abstractNumberingLists[listIndex] : model.numberingLists[listIndex];
            var listLevel: IListLevel = numberingList.levels[listLevelIndex];

            if(listLevel instanceof NumberingListReferenceLevel) {
                var abstractNumberingListIndex = (<NumberingList>numberingList).abstractNumberingListIndex;
                oldState.register(new HistoryItemListLevelStateObject(true, abstractNumberingListIndex, listLevelIndex, this.getPropertyValue(listLevel.getListLevelProperties())));
                this.setValueCore(listLevel, newValue);
                newState.register(new HistoryItemListLevelStateObject(true, abstractNumberingListIndex, listLevelIndex, newValue));
            }
            else {
                oldState.register(new HistoryItemListLevelStateObject(isAbstractList, listIndex, listLevelIndex, this.getPropertyValue(listLevel.getListLevelProperties())));
                this.setValueCore(listLevel, newValue);
                newState.register(new HistoryItemListLevelStateObject(isAbstractList, listIndex, listLevelIndex, newValue));
            }
            this.manipulator.dispatcher.notifyListLevelPropertyChanged(this.getJSONListLevelProperty(), newState);
            return oldState;
        }

        restoreValue(model: DocumentModel, state: HistoryItemState<HistoryItemListLevelStateObject>) {
            var stateObject = state.objects[0];
            var numberingList: NumberingListBase<IListLevel> = stateObject.isAbstractNumberingList ? model.abstractNumberingLists[stateObject.numberingListIndex] : model.numberingLists[stateObject.numberingListIndex];
            var listLevel: IListLevel = numberingList.levels[stateObject.listLevelIndex];
            this.setValueCore(listLevel, stateObject.value);
            this.manipulator.dispatcher.notifyListLevelPropertyChanged(this.getJSONListLevelProperty(), state);
        }


        setPropertyValue(properties: ListLevelProperties, newValue: T) {
            throw new Error(Errors.NotImplemented);
        }
        getPropertyValue(properties: ListLevelProperties): T {
            throw new Error(Errors.NotImplemented);
        }
        getJSONListLevelProperty(): JSONListLevelProperty {
            throw new Error(Errors.NotImplemented);
        }

        private setValueCore(level: IListLevel, newValue: T) {
            var properties = level.getListLevelProperties().clone();
            this.setPropertyValue(properties, newValue);
            level.setListLevelProperties(properties);
        }
    }

    class StartListLevelPropertiesManipulator extends ListLevelPropertiesManipulatorBase<number> {
        setPropertyValue(properties: ListLevelProperties, newValue: number) {
            properties.start = newValue;
        }
        getPropertyValue(properties: ListLevelProperties): number {
            return properties.start;
        }
        getJSONListLevelProperty(): JSONListLevelProperty {
            return JSONListLevelProperty.Start;
        }
    }

    class FormatListLevelPropertiesManipulator extends ListLevelPropertiesManipulatorBase<NumberingFormat> {
        setPropertyValue(properties: ListLevelProperties, newValue: NumberingFormat) {
            properties.format = newValue;
        }
        getPropertyValue(properties: ListLevelProperties): NumberingFormat {
            return properties.format;
        }
        getJSONListLevelProperty(): JSONListLevelProperty {
            return JSONListLevelProperty.Format;
        }
    }

    class AlignmentListLevelPropertiesManipulator extends ListLevelPropertiesManipulatorBase<ListNumberAlignment> {
        setPropertyValue(properties: ListLevelProperties, newValue: ListNumberAlignment) {
            properties.alignment = newValue;
        }
        getPropertyValue(properties: ListLevelProperties): ListNumberAlignment {
            return properties.alignment;
        }
        getJSONListLevelProperty(): JSONListLevelProperty {
            return JSONListLevelProperty.Alignment;
        }
    }

    class ConvertPreviousLevelNumberingToDecimalListLevelPropertiesManipulator extends ListLevelPropertiesManipulatorBase<boolean> {
        setPropertyValue(properties: ListLevelProperties, newValue: boolean) {
            properties.convertPreviousLevelNumberingToDecimal = newValue;
        }
        getPropertyValue(properties: ListLevelProperties): boolean {
            return properties.convertPreviousLevelNumberingToDecimal;
        }
        getJSONListLevelProperty(): JSONListLevelProperty {
            return JSONListLevelProperty.ConvertPreviousLevelNumberingToDecimal;
        }
    }

    class SeparatorListLevelPropertiesManipulator extends ListLevelPropertiesManipulatorBase<string> {
        setPropertyValue(properties: ListLevelProperties, newValue: string) {
            properties.separator = newValue;
        }
        getPropertyValue(properties: ListLevelProperties): string {
            return properties.separator;
        }
        getJSONListLevelProperty(): JSONListLevelProperty {
            return JSONListLevelProperty.Separator;
        }
    }

    class SuppressRestartListLevelPropertiesManipulator extends ListLevelPropertiesManipulatorBase<boolean> {
        setPropertyValue(properties: ListLevelProperties, newValue: boolean) {
            properties.suppressRestart = newValue;
        }
        getPropertyValue(properties: ListLevelProperties): boolean {
            return properties.suppressRestart;
        }
        getJSONListLevelProperty(): JSONListLevelProperty {
            return JSONListLevelProperty.SuppressRestart;
        }
    }

    class SuppressBulletResizeListLevelPropertiesManipulator extends ListLevelPropertiesManipulatorBase<boolean> {
        setPropertyValue(properties: ListLevelProperties, newValue: boolean) {
            properties.suppressBulletResize = newValue;
        }
        getPropertyValue(properties: ListLevelProperties): boolean {
            return properties.suppressBulletResize;
        }
        getJSONListLevelProperty(): JSONListLevelProperty {
            return JSONListLevelProperty.SuppressBulletResize;
        }
    }

    class DisplayFormatStringListLevelPropertiesManipulator extends ListLevelPropertiesManipulatorBase<string> {
        setPropertyValue(properties: ListLevelProperties, newValue: string) {
            properties.displayFormatString = newValue;
        }
        getPropertyValue(properties: ListLevelProperties): string {
            return properties.displayFormatString;
        }
        getJSONListLevelProperty(): JSONListLevelProperty {
            return JSONListLevelProperty.DisplayFormatString;
        }
    }

    class RelativeRestartLevelListLevelPropertiesManipulator extends ListLevelPropertiesManipulatorBase<number> {
        setPropertyValue(properties: ListLevelProperties, newValue: number) {
            properties.relativeRestartLevel = newValue;
        }
        getPropertyValue(properties: ListLevelProperties): number {
            return properties.relativeRestartLevel;
        }
        getJSONListLevelProperty(): JSONListLevelProperty {
            return JSONListLevelProperty.RelativeRestartLevel;
        }
    }

    class TemplateCodeListLevelPropertiesManipulator extends ListLevelPropertiesManipulatorBase<number> {
        setPropertyValue(properties: ListLevelProperties, newValue: number) {
            properties.templateCode = newValue;
        }
        getPropertyValue(properties: ListLevelProperties): number {
            return properties.templateCode;
        }
        getJSONListLevelProperty(): JSONListLevelProperty {
            return JSONListLevelProperty.TemplateCode;
        }
    }

    class OriginalLeftIndentListLevelPropertiesManipulator extends ListLevelPropertiesManipulatorBase<number> {
        setPropertyValue(properties: ListLevelProperties, newValue: number) {
            properties.originalLeftIndent = newValue;
        }
        getPropertyValue(properties: ListLevelProperties): number {
            return properties.originalLeftIndent;
        }
        getJSONListLevelProperty(): JSONListLevelProperty {
            return JSONListLevelProperty.OriginalLeftIndent;
        }
    }

    class LegacyListLevelPropertiesManipulator extends ListLevelPropertiesManipulatorBase<boolean> {
        setPropertyValue(properties: ListLevelProperties, newValue: boolean) {
            properties.legacy = newValue;
        }
        getPropertyValue(properties: ListLevelProperties): boolean {
            return properties.legacy;
        }
        getJSONListLevelProperty(): JSONListLevelProperty {
            return JSONListLevelProperty.Legacy;
        }
    }

    class LegacySpaceListLevelPropertiesManipulator extends ListLevelPropertiesManipulatorBase<number> {
        setPropertyValue(properties: ListLevelProperties, newValue: number) {
            properties.legacySpace = newValue;
        }
        getPropertyValue(properties: ListLevelProperties): number {
            return properties.legacySpace;
        }
        getJSONListLevelProperty(): JSONListLevelProperty {
            return JSONListLevelProperty.LegacySpace;
        }
    }

    class LegacyIndentListLevelPropertiesManipulator extends ListLevelPropertiesManipulatorBase<number> {
        setPropertyValue(properties: ListLevelProperties, newValue: number) {
            properties.legacyIndent = newValue;
        }
        getPropertyValue(properties: ListLevelProperties): number {
            return properties.legacyIndent;
        }
        getJSONListLevelProperty(): JSONListLevelProperty {
            return JSONListLevelProperty.LegacyIndent;
        }
    }
} 