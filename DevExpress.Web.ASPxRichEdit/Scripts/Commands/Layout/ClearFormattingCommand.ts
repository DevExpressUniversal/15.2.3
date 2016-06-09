module __aspxRichEdit {
    export class ClearFormattingCommand extends CommandBase<IntervalCommandStateEx> {
        getActualIntervals(): FixedInterval[] {
            if(this.control.selection.isCollapsed())
                return [this.control.model.activeSubDocument.getWholeWordInterval(this.control.selection.intervals[0].start)];
            return this.control.selection.getIntervalsClone();
        }

        getState(): IntervalCommandStateEx {
            return new IntervalCommandStateEx(this.isEnabled(), this.getActualIntervals());
        }

        executeCore(state: IntervalCommandStateEx, parameter: string): boolean {
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            var model: DocumentModel = modelManipulator.model;
            var subDocument: SubDocument = model.activeSubDocument;

            this.control.history.beginTransaction();
            var defaultCharProperties: MaskedCharacterProperties = model.defaultCharacterProperties;
            if(state.intervals[0].length > 0 || state.intervals.length > 0) {
                for(let i = 0, interval: FixedInterval; interval = state.intervals[i]; i++) {
                    this.control.history.addAndRedo(new ApplyCharacterStyleHistoryItem(modelManipulator, subDocument, interval, model.getDefaultCharacterStyle()));
                    this.control.history.addAndRedo(new FontBoldHistoryItem(modelManipulator, subDocument, interval, defaultCharProperties.fontBold, false));
                    this.control.history.addAndRedo(new FontCapsHistoryItem(modelManipulator, subDocument, interval, defaultCharProperties.allCaps, false));
                    this.control.history.addAndRedo(new FontUnderlineTypeHistoryItem(modelManipulator, subDocument, interval, defaultCharProperties.fontUnderlineType, false));
                    this.control.history.addAndRedo(new FontForeColorHistoryItem(modelManipulator, subDocument, interval, defaultCharProperties.foreColor, false));
                    this.control.history.addAndRedo(new FontBackColorHistoryItem(modelManipulator, subDocument, interval, defaultCharProperties.backColor, false));
                    this.control.history.addAndRedo(new FontHiddenHistoryItem(modelManipulator, subDocument, interval, defaultCharProperties.hidden, false));
                    this.control.history.addAndRedo(new FontItalicHistoryItem(modelManipulator, subDocument, interval, defaultCharProperties.fontItalic, false));
                    this.control.history.addAndRedo(new FontNameHistoryItem(modelManipulator, subDocument, interval, defaultCharProperties.fontInfo, false));
                    this.control.history.addAndRedo(new FontScriptHistoryItem(modelManipulator, subDocument, interval, defaultCharProperties.script, false));
                    this.control.history.addAndRedo(new FontSizeHistoryItem(modelManipulator, subDocument, interval, defaultCharProperties.fontSize, false));
                    this.control.history.addAndRedo(new FontStrikeoutTypeHistoryItem(modelManipulator, subDocument, interval, defaultCharProperties.fontStrikeoutType, false));
                    this.control.history.addAndRedo(new FontUnderlineColorHistoryItem(modelManipulator, subDocument, interval, defaultCharProperties.underlineColor, false));
                    this.control.history.addAndRedo(new FontUnderlineWordsOnlyHistoryItem(modelManipulator, subDocument, interval, defaultCharProperties.underlineWordsOnly, false));
                    this.control.history.addAndRedo(new FontStrikeoutWordsOnlyHistoryItem(modelManipulator, subDocument, interval, defaultCharProperties.strikeoutWordsOnly, false));
                    this.control.history.addAndRedo(new FontStrikeoutColorHistoryItem(modelManipulator, subDocument, interval, defaultCharProperties.strikeoutColor, false));
                    this.control.history.addAndRedo(new FontNoProofHistoryItem(modelManipulator, subDocument, interval, defaultCharProperties.noProof, false));
                }
            }

            var paragraphIndices: number[] = model.activeSubDocument.getParagraphIndicesByIntervals(state.intervals);
            for(let i = paragraphIndices.length - 1; i >= 0; i--) {
                let paragraph = subDocument.paragraphs[paragraphIndices[i]];
                let interval = paragraph.getInterval();
                paragraph.onParagraphPropertiesChanged();
                this.control.history.addAndRedo(new ApplyParagraphStyleHistoryItem(modelManipulator, subDocument, interval, model.getDefaultParagraphStyle()));
                var defaultParProperties: MaskedParagraphProperties = model.defaultParagraphProperties;
                this.control.history.addAndRedo(new ParagraphAlignmentHistoryItem(modelManipulator, subDocument, interval, defaultParProperties.alignment, false));
                this.control.history.addAndRedo(new ParagraphContextualSpacingHistoryItem(modelManipulator, subDocument, interval, defaultParProperties.contextualSpacing, false));
                this.control.history.addAndRedo(new ParagraphAfterAutoSpacingHistoryItem(modelManipulator, subDocument, interval, defaultParProperties.afterAutoSpacing, false));
                this.control.history.addAndRedo(new ParagraphBackColorHistoryItem(modelManipulator, subDocument, interval, defaultParProperties.backColor, false));
                this.control.history.addAndRedo(new ParagraphBeforeAutoSpacingHistoryItem(modelManipulator, subDocument, interval, defaultParProperties.beforeAutoSpacing, false));
                this.control.history.addAndRedo(new ParagraphFirstLineIndentHistoryItem(modelManipulator, subDocument, interval, defaultParProperties.firstLineIndent, false));
                this.control.history.addAndRedo(new ParagraphFirstLineIndentTypeHistoryItem(modelManipulator, subDocument, interval, defaultParProperties.firstLineIndentType, false));
                this.control.history.addAndRedo(new ParagraphKeepLinesTogetherHistoryItem(modelManipulator, subDocument, interval, defaultParProperties.keepLinesTogether, false));
                this.control.history.addAndRedo(new ParagraphLeftIndentHistoryItem(modelManipulator, subDocument, interval, defaultParProperties.leftIndent, false));
                this.control.history.addAndRedo(new ParagraphLineSpacingHistoryItem(modelManipulator, subDocument, interval, defaultParProperties.lineSpacing, false));
                this.control.history.addAndRedo(new ParagraphLineSpacingTypeHistoryItem(modelManipulator, subDocument, interval, defaultParProperties.lineSpacingType, false));
                this.control.history.addAndRedo(new ParagraphOutlineLevelHistoryItem(modelManipulator, subDocument, interval, defaultParProperties.outlineLevel, false));
                this.control.history.addAndRedo(new ParagraphPageBreakBeforeHistoryItem(modelManipulator, subDocument, interval, defaultParProperties.pageBreakBefore, false));
                this.control.history.addAndRedo(new ParagraphRightIndentHistoryItem(modelManipulator, subDocument, interval, defaultParProperties.rightIndent, false));
                this.control.history.addAndRedo(new ParagraphSpacingAfterHistoryItem(modelManipulator, subDocument, interval, defaultParProperties.spacingAfter, false));
                this.control.history.addAndRedo(new ParagraphSpacingBeforeHistoryItem(modelManipulator, subDocument, interval, defaultParProperties.spacingBefore, false));
                this.control.history.addAndRedo(new ParagraphSuppressHyphenationHistoryItem(modelManipulator, subDocument, interval, defaultParProperties.suppressHyphenation, false));
                this.control.history.addAndRedo(new ParagraphSuppressLineNumbersHistoryItem(modelManipulator, subDocument, interval, defaultParProperties.suppressLineNumbers, false));
                this.control.history.addAndRedo(new ParagraphWidowOrphanControlHistoryItem(modelManipulator, subDocument, interval, defaultParProperties.widowOrphanControl, false));
            }
            this.control.history.endTransaction();
            return true;
        }

        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.characterFormatting);
        }
    }
} 