module __aspxRichEdit {
    export class ListLevelParagraphPropertiesManipulator {
        align: IListLevelPropertyWithUseManipulator<ParagraphAlignment>;
        contextualSpacing: IListLevelPropertyWithUseManipulator<boolean>;
        afterAutoSpacing: IListLevelPropertyWithUseManipulator<boolean>;
        backColor: IListLevelPropertyWithUseManipulator<number>;
        beforeAutoSpacing: IListLevelPropertyWithUseManipulator<boolean>;
        firstLineIndent: IListLevelPropertyWithUseManipulator<number>;
        firstLineIndentType: IListLevelPropertyWithUseManipulator<ParagraphFirstLineIndent>;
        keepLinesTogether: IListLevelPropertyWithUseManipulator<boolean>;
        leftIndent: IListLevelPropertyWithUseManipulator<number>;
        lineSpacing: IListLevelPropertyWithUseManipulator<number>;
        lineSpacingType: IListLevelPropertyWithUseManipulator<ParagraphLineSpacingType>;
        outlineLevel: IListLevelPropertyWithUseManipulator<number>;
        pageBreakBefore: IListLevelPropertyWithUseManipulator<boolean>;
        rightIndent: IListLevelPropertyWithUseManipulator<number>;
        spacingAfter: IListLevelPropertyWithUseManipulator<number>;
        spacingBefore: IListLevelPropertyWithUseManipulator<number>;
        suppressHyphenation: IListLevelPropertyWithUseManipulator<boolean>;
        suppressLineNumbers: IListLevelPropertyWithUseManipulator<boolean>;
        widowOrphanControl: IListLevelPropertyWithUseManipulator<boolean>;

        constructor(manipulator: ModelManipulator) {
            this.align = new ParagraphPropertiesManipulator<ParagraphAlignment>(manipulator,
                JSONParagraphFormattingProperty.Alignment,
                ParagraphPropertiesMask.UseAlignment,
                (properties: ParagraphProperties, value: ParagraphAlignment) => properties.alignment = value,
                properties => properties.alignment);
            this.contextualSpacing = new ParagraphPropertiesManipulator<boolean>(manipulator,
                JSONParagraphFormattingProperty.ContextualSpacing,
                ParagraphPropertiesMask.UseContextualSpacing,
                (properties: ParagraphProperties, value: boolean) => properties.contextualSpacing = value,
                properties => properties.contextualSpacing);
            this.afterAutoSpacing = new ParagraphPropertiesManipulator<boolean>(manipulator,
                JSONParagraphFormattingProperty.AfterAutoSpacing,
                ParagraphPropertiesMask.UseAfterAutoSpacing,
                (properties: ParagraphProperties, value: boolean) => properties.afterAutoSpacing = value,
                properties => properties.afterAutoSpacing);
            this.backColor = new ParagraphPropertiesManipulator<number>(manipulator,
                JSONParagraphFormattingProperty.BackColor,
                ParagraphPropertiesMask.UseBackColor,
                (properties: ParagraphProperties, value: number) => properties.backColor = value,
                properties => properties.backColor);
            this.beforeAutoSpacing = new ParagraphPropertiesManipulator<boolean>(manipulator,
                JSONParagraphFormattingProperty.BeforeAutoSpacing,
                ParagraphPropertiesMask.UseBeforeAutoSpacing,
                (properties: ParagraphProperties, value: boolean) => properties.beforeAutoSpacing = value,
                properties => properties.beforeAutoSpacing);
            this.firstLineIndent = new ParagraphPropertiesManipulator<number>(manipulator,
                JSONParagraphFormattingProperty.FirstLineIndent,
                ParagraphPropertiesMask.UseFirstLineIndent,
                (properties: ParagraphProperties, value: number) => properties.firstLineIndent = value,
                properties => properties.firstLineIndent);
            this.keepLinesTogether = new ParagraphPropertiesManipulator<boolean>(manipulator,
                JSONParagraphFormattingProperty.KeepLinesTogether,
                ParagraphPropertiesMask.UseKeepLinesTogether,
                (properties: ParagraphProperties, value: boolean) => properties.keepLinesTogether = value,
                properties => properties.keepLinesTogether);
            this.firstLineIndentType = new ParagraphPropertiesManipulator<ParagraphFirstLineIndent>(manipulator,
                JSONParagraphFormattingProperty.FirstLineIndentType,
                ParagraphPropertiesMask.UseFirstLineIndent,
                (properties: ParagraphProperties, value: ParagraphFirstLineIndent) => properties.firstLineIndentType = value,
                properties => properties.firstLineIndentType);
            this.leftIndent = new ParagraphPropertiesManipulator<number>(manipulator,
                JSONParagraphFormattingProperty.LeftIndent,
                ParagraphPropertiesMask.UseLeftIndent,
                (properties: ParagraphProperties, value: number) => properties.leftIndent = value,
                properties => properties.leftIndent);
            this.lineSpacing = new ParagraphPropertiesManipulator<number>(manipulator,
                JSONParagraphFormattingProperty.LineSpacing,
                ParagraphPropertiesMask.UseLineSpacing,
                (properties: ParagraphProperties, value: number) => properties.lineSpacing = value,
                properties => properties.lineSpacing);
            this.lineSpacingType = new ParagraphPropertiesManipulator<ParagraphLineSpacingType>(manipulator,
                JSONParagraphFormattingProperty.LineSpacingType,
                ParagraphPropertiesMask.UseLineSpacing,
                (properties: ParagraphProperties, value: ParagraphLineSpacingType) => properties.lineSpacingType = value,
                properties => properties.lineSpacingType);
            this.outlineLevel = new ParagraphPropertiesManipulator<number>(manipulator,
                JSONParagraphFormattingProperty.OutlineLevel,
                ParagraphPropertiesMask.UseOutlineLevel,
                (properties: ParagraphProperties, value: number) => properties.outlineLevel = value,
                properties => properties.outlineLevel);
            this.pageBreakBefore = new ParagraphPropertiesManipulator<boolean>(manipulator,
                JSONParagraphFormattingProperty.PageBreakBefore,
                ParagraphPropertiesMask.UsePageBreakBefore,
                (properties: ParagraphProperties, value: boolean) => properties.pageBreakBefore = value,
                properties => properties.pageBreakBefore);
            this.rightIndent = new ParagraphPropertiesManipulator<number>(manipulator,
                JSONParagraphFormattingProperty.RightIndent,
                ParagraphPropertiesMask.UseRightIndent,
                (properties: ParagraphProperties, value: number) => properties.rightIndent = value,
                properties => properties.rightIndent);
            this.spacingAfter = new ParagraphPropertiesManipulator<number>(manipulator,
                JSONParagraphFormattingProperty.SpacingAfter,
                ParagraphPropertiesMask.UseSpacingAfter,
                (properties: ParagraphProperties, value: number) => properties.spacingAfter = value,
                properties => properties.spacingAfter);
            this.spacingBefore = new ParagraphPropertiesManipulator<number>(manipulator,
                JSONParagraphFormattingProperty.SpacingBefore,
                ParagraphPropertiesMask.UseSpacingBefore,
                (properties: ParagraphProperties, value: number) => properties.spacingBefore = value,
                properties => properties.spacingBefore);
            this.suppressHyphenation = new ParagraphPropertiesManipulator<boolean>(manipulator,
                JSONParagraphFormattingProperty.SuppressHyphenation,
                ParagraphPropertiesMask.UseSuppressHyphenation,
                (properties: ParagraphProperties, value: boolean) => properties.suppressHyphenation = value,
                properties => properties.suppressHyphenation);
            this.suppressLineNumbers = new ParagraphPropertiesManipulator<boolean>(manipulator,
                JSONParagraphFormattingProperty.SuppressLineNumbers,
                ParagraphPropertiesMask.UseSuppressLineNumbers,
                (properties: ParagraphProperties, value: boolean) => properties.suppressLineNumbers = value,
                properties => properties.suppressLineNumbers);
            this.widowOrphanControl = new ParagraphPropertiesManipulator<boolean>(manipulator,
                JSONParagraphFormattingProperty.WidowOrphanControl,
                ParagraphPropertiesMask.UseWidowOrphanControl,
                (properties: ParagraphProperties, value: boolean) => properties.widowOrphanControl = value,
                properties => properties.widowOrphanControl);
        }
    }

    class ParagraphPropertiesManipulator<T> implements IListLevelPropertyWithUseManipulator<T> {
        manipulator: ModelManipulator;
        jsonParagraphFormattingProperty: JSONParagraphFormattingProperty;
        paragraphPropertiesMask: ParagraphPropertiesMask;
        setProperty: (properties: MaskedParagraphProperties, value: T) => void;
        getProperty: (properties: MaskedParagraphProperties) => T;

        constructor(manipulator: ModelManipulator, jsonParagraphFormattingProperty: JSONParagraphFormattingProperty, paragraphPropertiesMask: ParagraphPropertiesMask, setProperty: (properties: MaskedParagraphProperties, value: T) => void, getProperty: (properties: MaskedParagraphProperties) => T) {
            this.manipulator = manipulator;
            this.paragraphPropertiesMask = paragraphPropertiesMask;
            this.jsonParagraphFormattingProperty = jsonParagraphFormattingProperty;
            this.setProperty = setProperty;
            this.getProperty = getProperty;
        }

        setValue(model: DocumentModel, isAbstractList: boolean, listIndex: number, listLevelIndex: number, newValue: T, newUse: boolean): HistoryItemState<HistoryItemListLevelUseStateObject> {
            var newState = new HistoryItemState<HistoryItemListLevelUseStateObject>();
            var oldState = new HistoryItemState<HistoryItemListLevelUseStateObject>();

            var numberingList: NumberingListBase<IListLevel> = isAbstractList ? model.abstractNumberingLists[listIndex] : model.numberingLists[listIndex];
            var listLevel: IListLevel = numberingList.levels[listLevelIndex];

            var properties = listLevel.getParagraphProperties();

            if(listLevel instanceof NumberingListReferenceLevel) {
                var abstractNumberingListIndex = (<NumberingList>numberingList).abstractNumberingListIndex;
                oldState.register(new HistoryItemListLevelUseStateObject(true, abstractNumberingListIndex, listLevelIndex, this.getProperty(properties), properties.getUseValue(this.paragraphPropertiesMask)));
                this.setValueCore(listLevel, newValue, newUse);
                newState.register(new HistoryItemListLevelUseStateObject(true, abstractNumberingListIndex, listLevelIndex, newValue, newUse));
            }
            else {
                oldState.register(new HistoryItemListLevelUseStateObject(isAbstractList, listIndex, listLevelIndex, this.getProperty(properties), properties.getUseValue(this.paragraphPropertiesMask)));
                this.setValueCore(listLevel, newValue, newUse);
                newState.register(new HistoryItemListLevelUseStateObject(isAbstractList, listIndex, listLevelIndex, newValue, newUse));
            }
            this.manipulator.model.resetMergedFormattingCache(ResetFormattingCacheType.Paragraph);
            this.manipulator.dispatcher.notifyListLevelParagraphPropertyChanged(this.jsonParagraphFormattingProperty, newState);
            return oldState;
        }
        restoreValue(model: DocumentModel, state: HistoryItemState<HistoryItemListLevelUseStateObject>) {
            var stateObject = state.objects[0];
            var numberingList: NumberingListBase<IListLevel> = stateObject.isAbstractNumberingList ? model.abstractNumberingLists[stateObject.numberingListIndex] : model.numberingLists[stateObject.numberingListIndex];
            var listLevel: IListLevel = numberingList.levels[stateObject.listLevelIndex];
            this.setValueCore(listLevel, stateObject.value, stateObject.use);
            this.manipulator.model.resetMergedFormattingCache(ResetFormattingCacheType.Paragraph);
            this.manipulator.dispatcher.notifyListLevelParagraphPropertyChanged(this.jsonParagraphFormattingProperty, state);
        }

        private setValueCore(level: IListLevel, newValue: T, newUse: boolean) {
            var properties = level.getParagraphProperties().clone();
            this.setProperty(properties, newValue);
            properties.setUseValue(this.paragraphPropertiesMask, newUse);
            level.setParagraphProperties(properties);
            level.onParagraphPropertiesChanged();
        }
    }


}