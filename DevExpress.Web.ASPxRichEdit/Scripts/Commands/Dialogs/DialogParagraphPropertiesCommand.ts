module __aspxRichEdit {
    export class DialogParagraphPropertiesCommand extends ShowDialogCommandBase {
        getState(): ICommandState {
            return new IntervalCommandStateEx(this.isEnabled(), this.control.selection.getIntervalsClone());
        }

        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.paragraphFormatting);
        }

        createParameters(): ParagraphDialogParameters {
            var parameters: ParagraphDialogParameters = new ParagraphDialogParameters();
            parameters.init(this.control.inputPosition.getMergedParagraphPropertiesRaw(), this.control.inputPosition.getMergedSectionPropertiesRaw());
            return parameters;
        }

        applyParameters(state: IntervalCommandStateEx, newParams: ParagraphDialogParameters) {
            var initParams: ParagraphDialogParameters = <ParagraphDialogParameters>this.initParams;
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            var history: IHistory = this.control.history;
            this.control.inputPosition.resetParagraphMergedProperties();

            history.beginTransaction();
            var paragraphIndices = this.control.model.activeSubDocument.getParagraphIndicesByIntervals(state.intervals);
            for(var i = paragraphIndices.length - 1; i >= 0; i--) {
                var paragraph = this.control.model.activeSubDocument.paragraphs[paragraphIndices[i]];
                var interval = paragraph.getInterval();
                if(newParams.alignment !== initParams.alignment)
                    history.addAndRedo(new ParagraphAlignmentHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newParams.alignment, true));
                if(newParams.contextualSpacing !== initParams.contextualSpacing)
                    history.addAndRedo(new ParagraphContextualSpacingHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newParams.contextualSpacing, true));
                if(newParams.firstLineIndent !== initParams.firstLineIndent)
                    history.addAndRedo(new ParagraphFirstLineIndentHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newParams.firstLineIndent, true));
                if(newParams.firstLineIndentType !== initParams.firstLineIndentType)
                    history.addAndRedo(new ParagraphFirstLineIndentTypeHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newParams.firstLineIndentType, true));
                if(newParams.leftIndent !== initParams.leftIndent || newParams.firstLineIndentType !== initParams.firstLineIndentType) {
                    var leftIndent: number = newParams.firstLineIndentType === ParagraphFirstLineIndent.Hanging ? newParams.leftIndent + newParams.firstLineIndent : newParams.leftIndent;
                    history.addAndRedo(new ParagraphLeftIndentHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, leftIndent, true));
                }

                if(newParams.keepLinesTogether !== initParams.keepLinesTogether)
                    history.addAndRedo(new ParagraphKeepLinesTogetherHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newParams.keepLinesTogether, true));
                var lineSpacingTypeChanged = false;
                if(newParams.lineSpacingType !== initParams.lineSpacingType) {
                    history.addAndRedo(new ParagraphLineSpacingTypeHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newParams.lineSpacingType, true));
                    lineSpacingTypeChanged = true;
                }
                if((newParams.lineSpacingType === ParagraphLineSpacingType.AtLeast || newParams.lineSpacingType === ParagraphLineSpacingType.Exactly) &&
                    (lineSpacingTypeChanged || newParams.lineSpacing !== initParams.lineSpacing))
                    history.addAndRedo(new ParagraphLineSpacingHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newParams.lineSpacing, true));
                else if(newParams.lineSpacingType === ParagraphLineSpacingType.Multiple && (lineSpacingTypeChanged || newParams.lineSpacingMultiple !== initParams.lineSpacingMultiple))
                    history.addAndRedo(new ParagraphLineSpacingHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newParams.lineSpacingMultiple, true));
                if(newParams.outlineLevel !== initParams.outlineLevel)
                    history.addAndRedo(new ParagraphOutlineLevelHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newParams.outlineLevel, true));
                if(newParams.pageBreakBefore !== initParams.pageBreakBefore)
                    history.addAndRedo(new ParagraphPageBreakBeforeHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newParams.pageBreakBefore, true));
                if(newParams.rightIndent !== initParams.rightIndent)
                    history.addAndRedo(new ParagraphRightIndentHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newParams.rightIndent, true));
                if(newParams.spacingAfter !== initParams.spacingAfter)
                    history.addAndRedo(new ParagraphSpacingAfterHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newParams.spacingAfter, true));
                if(newParams.spacingBefore !== initParams.spacingBefore)
                    history.addAndRedo(new ParagraphSpacingBeforeHistoryItem(modelManipulator, modelManipulator.model.activeSubDocument, interval, newParams.spacingBefore, true));
            }
            history.endTransaction();
        }
        getDialogName() {
            return "EditParagraph";
        }
    }

    export class ParagraphDialogParameters extends DialogParametersBase {
        alignment: ParagraphAlignment;
        outlineLevel: number;
        rightIndent: number;
        spacingBefore: number;
        spacingAfter: number;
        lineSpacingType: ParagraphLineSpacingType;
        firstLineIndentType: ParagraphFirstLineIndent;
        firstLineIndent: number;
        contextualSpacing: boolean;
        keepLinesTogether: boolean;
        pageBreakBefore: boolean;
        pageWidth: number;
        leftIndent: number;
        lineSpacing: number;
        lineSpacingMultiple: number;

        init(parProps: ParagraphProperties, secProps: SectionProperties) {
            this.alignment = parProps.alignment;
            this.outlineLevel = parProps.outlineLevel;
            this.rightIndent = parProps.rightIndent;
            this.spacingAfter = parProps.spacingAfter;
            this.spacingBefore = parProps.spacingBefore;
            this.lineSpacingType = parProps.lineSpacingType;
            this.firstLineIndentType = parProps.firstLineIndentType;
            this.firstLineIndent = parProps.firstLineIndent;
            this.contextualSpacing = parProps.contextualSpacing;
            this.keepLinesTogether = parProps.keepLinesTogether;
            this.pageBreakBefore = parProps.pageBreakBefore;
            this.pageWidth = secProps.pageWidth - secProps.marginLeft - secProps.marginRight;
            this.leftIndent = parProps.firstLineIndentType === ParagraphFirstLineIndent.Hanging ? parProps.leftIndent - parProps.firstLineIndent : parProps.leftIndent;
            switch (parProps.lineSpacingType) {
                case ParagraphLineSpacingType.AtLeast:
                case ParagraphLineSpacingType.Exactly:
                    this.lineSpacing = parProps.lineSpacing;
                    this.lineSpacingMultiple = 3;
                    break;
                case ParagraphLineSpacingType.Multiple:
                    this.lineSpacing = 240;
                    this.lineSpacingMultiple = parProps.lineSpacing;
                    break;
                default:
                    this.lineSpacing = 240;
                    this.lineSpacingMultiple = 3;
                    break;
            }
        }

        getNewInstance(): DialogParametersBase {
            return new ParagraphDialogParameters();
        }

        copyFrom(obj: ParagraphDialogParameters) {
            this.alignment = obj.alignment;
            this.outlineLevel = obj.outlineLevel;
            this.rightIndent = obj.rightIndent;
            this.spacingBefore = obj.spacingBefore;
            this.spacingAfter = obj.spacingAfter;
            this.lineSpacingType = obj.lineSpacingType;
            this.firstLineIndentType = obj.firstLineIndentType;
            this.firstLineIndent = obj.firstLineIndent;
            this.contextualSpacing = obj.contextualSpacing;
            this.keepLinesTogether = obj.keepLinesTogether;
            this.pageBreakBefore = obj.pageBreakBefore;
            this.pageWidth = obj.pageWidth;
            this.leftIndent = obj.leftIndent;
            this.lineSpacing = obj.lineSpacing;
            this.lineSpacingMultiple = obj.lineSpacingMultiple;
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
            if (this.pageWidth) this.pageWidth = converterFunc(this.pageWidth);
            if (this.firstLineIndent) this.firstLineIndent = converterFunc(this.firstLineIndent);
            if (this.leftIndent) this.leftIndent = converterFunc(this.leftIndent);
            if (this.lineSpacing) this.lineSpacing = converterFunc(this.lineSpacing);
            if (this.rightIndent) this.rightIndent = converterFunc(this.rightIndent);
            if (this.spacingAfter) this.spacingAfter = converterFunc(this.spacingAfter);
            if (this.spacingBefore) this.spacingBefore = converterFunc(this.spacingBefore);
        }
    }
} 