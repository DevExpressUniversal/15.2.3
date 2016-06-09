module __aspxRichEdit {
    export class ParagraphPropertiesManipulator {
        align: IIntervalPropertyWithUseValueManipulator<ParagraphAlignment>;
        contextualSpacing: IIntervalPropertyWithUseValueManipulator<boolean>;
        afterAutoSpacing: IIntervalPropertyWithUseValueManipulator<boolean>;
        backColor: IIntervalPropertyWithUseValueManipulator<number>;
        beforeAutoSpacing: IIntervalPropertyWithUseValueManipulator<boolean>;
        firstLineIndent: IIntervalPropertyWithUseValueManipulator<number>;
        firstLineIndentType: IIntervalPropertyWithUseValueManipulator<ParagraphFirstLineIndent>;
        keepLinesTogether: IIntervalPropertyWithUseValueManipulator<boolean>;
        leftIndent: IIntervalPropertyWithUseValueManipulator<number>;
        lineSpacing: IIntervalPropertyWithUseValueManipulator<number>;
        lineSpacingType: IIntervalPropertyWithUseValueManipulator<ParagraphLineSpacingType>;
        outlineLevel: IIntervalPropertyWithUseValueManipulator<number>;
        pageBreakBefore: IIntervalPropertyWithUseValueManipulator<boolean>;
        rightIndent: IIntervalPropertyWithUseValueManipulator<number>;
        spacingAfter: IIntervalPropertyWithUseValueManipulator<number>;
        spacingBefore: IIntervalPropertyWithUseValueManipulator<number>;
        suppressHyphenation: IIntervalPropertyWithUseValueManipulator<boolean>;
        suppressLineNumbers: IIntervalPropertyWithUseValueManipulator<boolean>;
        widowOrphanControl: IIntervalPropertyWithUseValueManipulator<boolean>;
        useValue: IIntervalPropertyManipulator<number>;
        manipulator: ModelManipulator;

        constructor(manipulator: ModelManipulator) {
            this.align = new ParagraphPropertiesAlignManipulator(manipulator);
            this.contextualSpacing = new ParagraphPropertiesContextualSpacingManipulator(manipulator);
            this.afterAutoSpacing = new ParagraphPropertiesAfterAutoSpacingManipulator(manipulator);
            this.backColor = new ParagraphPropertiesBackColorManipulator(manipulator);
            this.beforeAutoSpacing = new ParagraphPropertiesBeforeAutoSpacingManipulator(manipulator);
            this.firstLineIndent = new ParagraphPropertiesFirstLineIndentManipulator(manipulator);
            this.keepLinesTogether = new ParagraphPropertiesKeepLinesTogetherManipulator(manipulator);
            this.firstLineIndentType = new ParagraphPropertiesFirstLineIndentTypeManipulator(manipulator);
            this.leftIndent = new ParagraphPropertiesLeftIndentManipulator(manipulator);
            this.lineSpacing = new ParagraphPropertiesLineSpacingManipulator(manipulator);
            this.lineSpacingType = new ParagraphPropertiesLineSpacingTypeManipulator(manipulator);
            this.outlineLevel = new ParagraphPropertiesOutlineLevelManipulator(manipulator);
            this.pageBreakBefore = new ParagraphPropertiesPageBreakBeforeManipulator(manipulator);
            this.rightIndent = new ParagraphPropertiesRightIndentManipulator(manipulator);
            this.spacingAfter = new ParagraphPropertiesSpacingAfterManipulator(manipulator);
            this.spacingBefore = new ParagraphPropertiesSpacingBeforeManipulator(manipulator);
            this.suppressHyphenation = new ParagraphPropertiesSuppressHyphenationManipulator(manipulator);
            this.suppressLineNumbers = new ParagraphPropertiesSuppressLineNumbersManipulator(manipulator);
            this.widowOrphanControl = new ParagraphPropertiesWidowOrphanControlManipulator(manipulator);
            this.useValue = new ParagraphPropertiesUseValueManipulator(manipulator);
            this.manipulator = manipulator;
        }

        changeAllProperties(subDocument: SubDocument, paragraphIndex: number, properties: MaskedParagraphProperties, style: ParagraphStyle, tabs: TabProperties, numberingListIndex: number, listLevelIndex: number){
            var paragraph = subDocument.paragraphs[paragraphIndex];
            paragraph.setParagraphProperties(properties);
            paragraph.paragraphStyle = style;
            paragraph.tabs = tabs.clone();
            paragraph.numberingListIndex = numberingListIndex;
            paragraph.listLevelIndex = listLevelIndex;
        }
    }

    class ParagraphPropertiesUseValueManipulator implements IIntervalPropertyManipulator<number> {
        manipulator: ModelManipulator;
        constructor(manipulator: ModelManipulator) {
            this.manipulator = manipulator;
        }
        setValue(subDocument: SubDocument, interval: FixedInterval, newValue: number): HistoryItemIntervalState<HistoryItemIntervalStateObject> {
            var oldState = new HistoryItemIntervalState<HistoryItemIntervalStateObject>();
            if(!ControlOptions.isEnabled(subDocument.documentModel.options.paragraphFormatting))
                return oldState;
            var newState = new HistoryItemIntervalState<HistoryItemIntervalStateObject>();
            var paragraphs = subDocument.getParagraphsByInterval(interval);
            for(var i = 0, paragraph: Paragraph; paragraph = paragraphs[i]; i++) {
                var properties = paragraph.maskedParagraphProperties.clone();
                oldState.register(new HistoryItemIntervalStateObject(new FixedInterval(paragraph.startLogPosition.value, paragraph.length), properties.useValue));
                newState.register(new HistoryItemIntervalStateObject(new FixedInterval(paragraph.startLogPosition.value, paragraph.length), newValue));
                properties.useValue = newValue;
                paragraph.setParagraphProperties(properties);
                paragraph.onParagraphPropertiesChanged();
            }
            this.manipulator.dispatcher.notifyParagraphPropertyChanged(newState.interval(), JSONParagraphFormattingProperty.UseValue, newState, this.manipulator.model.activeSubDocument);
            return oldState;
        }
        restoreValue(subDocument: SubDocument, state: HistoryItemIntervalState<HistoryItemIntervalStateObject>) {
            if(!ControlOptions.isEnabled(subDocument.documentModel.options.paragraphFormatting))
                return;
            for(var stateValue: HistoryItemIntervalStateObject, i = 0; stateValue = state.objects[i]; i++) {
                var paragraphs = subDocument.getParagraphsByInterval(stateValue.interval);
                for(var i = 0, paragraph: Paragraph; paragraph = paragraphs[i]; i++) {
                    var properties = paragraph.maskedParagraphProperties.clone();
                    properties.useValue = stateValue.value;
                    paragraph.setParagraphProperties(properties);
                    paragraph.onParagraphPropertiesChanged();
                }
            }
            this.manipulator.dispatcher.notifyParagraphPropertyChanged(state.interval(), JSONParagraphFormattingProperty.UseValue, state, this.manipulator.model.activeSubDocument);
        }
    }

    class MaskedParagraphPropertiesManipulator<T> {
        manipulator: ModelManipulator;
        constructor(manipulator: ModelManipulator) {
            this.manipulator = manipulator;
        }

        setValue(subDocument: SubDocument, interval: FixedInterval, newValue: T, newUse: boolean): HistoryItemIntervalState<HistoryItemIntervalUseStateObject> {
            var oldState: HistoryItemIntervalState<HistoryItemIntervalUseStateObject> = new HistoryItemIntervalState<HistoryItemIntervalUseStateObject>();
            if(!ControlOptions.isEnabled(subDocument.documentModel.options.paragraphFormatting))
                return oldState;
            var newState: HistoryItemIntervalState<HistoryItemIntervalUseStateObject> = new HistoryItemIntervalState<HistoryItemIntervalUseStateObject>();
            var paragraphs = subDocument.getParagraphsByInterval(interval);
            for(var i = 0, paragraph: Paragraph; paragraph = paragraphs[i]; i++) {
                var currentInterval = paragraph.getInterval();
                var properties = paragraph.maskedParagraphProperties.clone();

                oldState.register(new HistoryItemIntervalUseStateObject(currentInterval, this.getPropertyValue(properties), properties.getUseValue(this.getPropertyMask())));
                newState.register(new HistoryItemIntervalUseStateObject(currentInterval, newValue, newUse));

                this.setPropertyValue(properties, newValue);
                properties.setUseValue(this.getPropertyMask(), newUse);
                paragraph.setParagraphProperties(properties);
                if(paragraph.hasParagraphMergedProperies() && newUse) {
                    var mergedProperties = paragraph.getParagraphMergedProperies().clone();
                    this.setPropertyValue(mergedProperties, newValue);
                    paragraph.setParagraphMergedProperies(mergedProperties);
                }
                else
                    paragraph.onParagraphPropertiesChanged();
            }
            this.manipulator.dispatcher.notifyParagraphPropertyChanged(newState.interval(), this.getJSONParagraphFormattingProperty(), newState, this.manipulator.model.activeSubDocument);
            return oldState;
        }

        restoreValue(subDocument: SubDocument, state: HistoryItemIntervalState<HistoryItemIntervalUseStateObject>) {
            if(!ControlOptions.isEnabled(subDocument.documentModel.options.paragraphFormatting))
                return;
            if(state.isEmpty()) return;
            for(var i = 0, stateItem: HistoryItemIntervalUseStateObject; stateItem = state.objects[i]; i++) {
                var paragraphs = subDocument.getParagraphsByInterval(stateItem.interval);
                for(var j = 0, paragraph: Paragraph; paragraph = paragraphs[j]; j++) {
                    var properties = paragraph.maskedParagraphProperties.clone();
                    this.setPropertyValue(properties, stateItem.value);
                    properties.setUseValue(this.getPropertyMask(), stateItem.use);
                    paragraph.setParagraphProperties(properties);
                    if(paragraph.hasParagraphMergedProperies()) {
                        var mergedProperties = paragraph.getParagraphMergedProperies().clone();
                        this.setPropertyValue(mergedProperties, stateItem.value);
                        paragraph.setParagraphMergedProperies(mergedProperties);
                    }
                    else
                        paragraph.onParagraphPropertiesChanged();
                }
            }
            this.manipulator.dispatcher.notifyParagraphPropertyChanged(state.interval(), this.getJSONParagraphFormattingProperty(), state, this.manipulator.model.activeSubDocument);
        }

        getPropertyValue(properties: ParagraphProperties): T {
            throw new Error(Errors.NotImplemented);
        }
        setPropertyValue(properties: ParagraphProperties, value: T) {
            throw new Error(Errors.NotImplemented);
        }
        getPropertyMask(): ParagraphPropertiesMask {
            throw new Error(Errors.NotImplemented);
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            throw new Error(Errors.NotImplemented);
        }
    }

    class ParagraphPropertiesAlignManipulator extends MaskedParagraphPropertiesManipulator<ParagraphAlignment> {
        getPropertyMask(): ParagraphPropertiesMask {
            return ParagraphPropertiesMask.UseAlignment;
        }
        getPropertyValue(properties: ParagraphProperties): ParagraphAlignment {
            return properties.alignment;
        }
        setPropertyValue(properties: ParagraphProperties, value: ParagraphAlignment) {
            properties.alignment = value;
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            return JSONParagraphFormattingProperty.Alignment;
        }
    }

    class ParagraphPropertiesContextualSpacingManipulator extends MaskedParagraphPropertiesManipulator<boolean> {
        getPropertyMask(): ParagraphPropertiesMask {
            return ParagraphPropertiesMask.UseContextualSpacing;
        }
        getPropertyValue(properties: ParagraphProperties): boolean {
            return properties.contextualSpacing;
        }
        setPropertyValue(properties: ParagraphProperties, value: boolean) {
            properties.contextualSpacing = value;
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            return JSONParagraphFormattingProperty.ContextualSpacing;
        }
    }

    class ParagraphPropertiesAfterAutoSpacingManipulator extends MaskedParagraphPropertiesManipulator<boolean> {
        getPropertyMask(): ParagraphPropertiesMask {
            return ParagraphPropertiesMask.UseAfterAutoSpacing;
        }
        getPropertyValue(properties: ParagraphProperties): boolean {
            return properties.afterAutoSpacing;
        }
        setPropertyValue(properties: ParagraphProperties, value: boolean) {
            properties.afterAutoSpacing = value;
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            return JSONParagraphFormattingProperty.AfterAutoSpacing;
        }
    }

    class ParagraphPropertiesBackColorManipulator extends MaskedParagraphPropertiesManipulator<number> {
        getPropertyMask(): ParagraphPropertiesMask {
            return ParagraphPropertiesMask.UseBackColor;
        }
        getPropertyValue(properties: ParagraphProperties): number {
            return properties.backColor;
        }
        setPropertyValue(properties: ParagraphProperties, value: number) {
            properties.backColor = value;
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            return JSONParagraphFormattingProperty.BackColor;
        }
    }

    class ParagraphPropertiesBeforeAutoSpacingManipulator extends MaskedParagraphPropertiesManipulator<boolean> {
        getPropertyMask(): ParagraphPropertiesMask {
            return ParagraphPropertiesMask.UseBeforeAutoSpacing;
        }
        getPropertyValue(properties: ParagraphProperties): boolean {
            return properties.beforeAutoSpacing;
        }
        setPropertyValue(properties: ParagraphProperties, value: boolean) {
            properties.beforeAutoSpacing = value;
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            return JSONParagraphFormattingProperty.BeforeAutoSpacing;
        }
    }

    class ParagraphPropertiesFirstLineIndentManipulator extends MaskedParagraphPropertiesManipulator<number> {
        getPropertyMask(): ParagraphPropertiesMask {
            return ParagraphPropertiesMask.UseFirstLineIndent;
        }
        getPropertyValue(properties: ParagraphProperties): number {
            return properties.firstLineIndent;
        }
        setPropertyValue(properties: ParagraphProperties, value: number) {
            properties.firstLineIndent = value;
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            return JSONParagraphFormattingProperty.FirstLineIndent;
        }
    }

    class ParagraphPropertiesFirstLineIndentTypeManipulator extends MaskedParagraphPropertiesManipulator<ParagraphFirstLineIndent> {
        getPropertyMask(): ParagraphPropertiesMask {
            return ParagraphPropertiesMask.UseFirstLineIndent;
        }
        getPropertyValue(properties: ParagraphProperties): ParagraphFirstLineIndent {
            return properties.firstLineIndentType;
        }
        setPropertyValue(properties: ParagraphProperties, value: ParagraphFirstLineIndent) {
            properties.firstLineIndentType = value;
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            return JSONParagraphFormattingProperty.FirstLineIndentType;
        }
    }

    class ParagraphPropertiesKeepLinesTogetherManipulator extends MaskedParagraphPropertiesManipulator<boolean> {
        getPropertyMask(): ParagraphPropertiesMask {
            return ParagraphPropertiesMask.UseKeepLinesTogether;
        }
        getPropertyValue(properties: ParagraphProperties): boolean {
            return properties.keepLinesTogether;
        }
        setPropertyValue(properties: ParagraphProperties, value: boolean) {
            properties.keepLinesTogether = value;
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            return JSONParagraphFormattingProperty.KeepLinesTogether;
        }
    }

    class ParagraphPropertiesKeepWithNextManipulator extends MaskedParagraphPropertiesManipulator<boolean> {
        getPropertyMask(): ParagraphPropertiesMask {
            return ParagraphPropertiesMask.UseKeepWithNext;
        }
        getPropertyValue(properties: ParagraphProperties): boolean {
            return properties.keepWithNext;
        }
        setPropertyValue(properties: ParagraphProperties, value: boolean) {
            properties.keepWithNext = value;
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            return JSONParagraphFormattingProperty.KeepWithNext;
        }
    }

    class ParagraphPropertiesLeftIndentManipulator extends MaskedParagraphPropertiesManipulator<number> {
        getPropertyMask(): ParagraphPropertiesMask {
            return ParagraphPropertiesMask.UseLeftIndent;
        }
        getPropertyValue(properties: ParagraphProperties): number {
            return properties.leftIndent;
        }
        setPropertyValue(properties: ParagraphProperties, value: number) {
            properties.leftIndent = value;
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            return JSONParagraphFormattingProperty.LeftIndent;
        }
    }

    class ParagraphPropertiesLineSpacingManipulator extends MaskedParagraphPropertiesManipulator<number> {
        getPropertyMask(): ParagraphPropertiesMask {
            return ParagraphPropertiesMask.UseLineSpacing;
        }
        getPropertyValue(properties: ParagraphProperties): number {
            return properties.lineSpacing;
        }
        setPropertyValue(properties: ParagraphProperties, value: number) {
            properties.lineSpacing = value;
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            return JSONParagraphFormattingProperty.LineSpacing;
        }
    }

    class ParagraphPropertiesLineSpacingTypeManipulator extends MaskedParagraphPropertiesManipulator<ParagraphLineSpacingType> {
        getPropertyMask(): ParagraphPropertiesMask {
            return ParagraphPropertiesMask.UseLineSpacing;
        }
        getPropertyValue(properties: ParagraphProperties): ParagraphLineSpacingType {
            return properties.lineSpacingType;
        }
        setPropertyValue(properties: ParagraphProperties, value: ParagraphLineSpacingType) {
            properties.lineSpacingType = value;
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            return JSONParagraphFormattingProperty.LineSpacingType;
        }
    }

    class ParagraphPropertiesOutlineLevelManipulator extends MaskedParagraphPropertiesManipulator<number> {
        getPropertyMask(): ParagraphPropertiesMask {
            return ParagraphPropertiesMask.UseOutlineLevel;
        }
        getPropertyValue(properties: ParagraphProperties): number {
            return properties.outlineLevel;
        }
        setPropertyValue(properties: ParagraphProperties, value: number) {
            properties.outlineLevel = value;
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            return JSONParagraphFormattingProperty.OutlineLevel;
        }
    }

    class ParagraphPropertiesPageBreakBeforeManipulator extends MaskedParagraphPropertiesManipulator<boolean> {
        getPropertyMask(): ParagraphPropertiesMask {
            return ParagraphPropertiesMask.UsePageBreakBefore;
        }
        getPropertyValue(properties: ParagraphProperties): boolean {
            return properties.pageBreakBefore;
        }
        setPropertyValue(properties: ParagraphProperties, value: boolean) {
            properties.pageBreakBefore = value;
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            return JSONParagraphFormattingProperty.PageBreakBefore;
        }
    }

    class ParagraphPropertiesRightIndentManipulator extends MaskedParagraphPropertiesManipulator<number> {
        getPropertyMask(): ParagraphPropertiesMask {
            return ParagraphPropertiesMask.UseRightIndent;
        }
        getPropertyValue(properties: ParagraphProperties): number {
            return properties.rightIndent;
        }
        setPropertyValue(properties: ParagraphProperties, value: number) {
            properties.rightIndent = value;
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            return JSONParagraphFormattingProperty.RightIndent;
        }
    }

    class ParagraphPropertiesSpacingAfterManipulator extends MaskedParagraphPropertiesManipulator<number> {
        getPropertyMask(): ParagraphPropertiesMask {
            return ParagraphPropertiesMask.UseSpacingAfter;
        }
        getPropertyValue(properties: ParagraphProperties): number {
            return properties.spacingAfter;
        }
        setPropertyValue(properties: ParagraphProperties, value: number) {
            properties.spacingAfter = value;
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            return JSONParagraphFormattingProperty.SpacingAfter;
        }
    }

    class ParagraphPropertiesSpacingBeforeManipulator extends MaskedParagraphPropertiesManipulator<number> {
        getPropertyMask(): ParagraphPropertiesMask {
            return ParagraphPropertiesMask.UseSpacingBefore;
        }
        getPropertyValue(properties: ParagraphProperties): number {
            return properties.spacingBefore;
        }
        setPropertyValue(properties: ParagraphProperties, value: number) {
            properties.spacingBefore = value;
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            return JSONParagraphFormattingProperty.SpacingBefore;
        }
    }

    class ParagraphPropertiesSuppressHyphenationManipulator extends MaskedParagraphPropertiesManipulator<boolean> {
        getPropertyMask(): ParagraphPropertiesMask {
            return ParagraphPropertiesMask.UseSuppressHyphenation;
        }
        getPropertyValue(properties: ParagraphProperties): boolean {
            return properties.suppressHyphenation;
        }
        setPropertyValue(properties: ParagraphProperties, value: boolean) {
            properties.suppressHyphenation = value;
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            return JSONParagraphFormattingProperty.SuppressHyphenation;
        }
    }

    class ParagraphPropertiesSuppressLineNumbersManipulator extends MaskedParagraphPropertiesManipulator<boolean> {
        getPropertyMask(): ParagraphPropertiesMask {
            return ParagraphPropertiesMask.UseSuppressLineNumbers;
        }
        getPropertyValue(properties: ParagraphProperties): boolean {
            return properties.suppressLineNumbers;
        }
        setPropertyValue(properties: ParagraphProperties, value: boolean) {
            properties.suppressLineNumbers = value;
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            return JSONParagraphFormattingProperty.SuppressLineNumbers;
        }
    }

    class ParagraphPropertiesWidowOrphanControlManipulator extends MaskedParagraphPropertiesManipulator<boolean> {
        getPropertyMask(): ParagraphPropertiesMask {
            return ParagraphPropertiesMask.UseWidowOrphanControl;
        }
        getPropertyValue(properties: ParagraphProperties): boolean {
            return properties.widowOrphanControl;
        }
        setPropertyValue(properties: ParagraphProperties, value: boolean) {
            properties.widowOrphanControl = value;
        }
        getJSONParagraphFormattingProperty(): JSONParagraphFormattingProperty {
            return JSONParagraphFormattingProperty.WidowOrphanControl;
        }
    }
} 