module __aspxRichEdit {
    export class ChangeTextCaseCommandBaseBase extends CommandBase<IntervalCommandStateEx> {
        getActualIntervals(): FixedInterval[] {
            if (this.control.selection.isCollapsed())
                return [this.control.model.activeSubDocument.getWholeWordInterval(this.control.selection.intervals[0].start, false, true)];
            return this.control.selection.getIntervalsClone();
        }

        getState(): IntervalCommandStateEx {
            return new IntervalCommandStateEx(this.isEnabled(), this.getActualIntervals());
        }

        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.characterFormatting);
        }
    }


    export class ChangeTextCaseCommandBase extends ChangeTextCaseCommandBaseBase {
        getActualInterval(): FixedInterval {
            if (this.control.selection.isCollapsed())
                return this.control.model.activeSubDocument.getWholeWordInterval(this.control.selection.intervals[0].start, false, true);
            return this.control.selection.getLastSelectedInterval().clone();
        }

        getState(): IntervalCommandStateEx {
            return new IntervalCommandStateEx(this.isEnabled(), this.getActualIntervals());
        }

        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.characterFormatting);
        }

        executeCore(state: IntervalCommandStateEx): boolean {
            if(state.intervals.length === 0 || state.intervals[0].length === 0)
                return false;
            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            var subDocument: SubDocument = modelManipulator.model.activeSubDocument;
            var layout: DocumentLayout = this.control.layout;

            this.control.history.beginTransaction();
            for(let i = 0, interval: FixedInterval; interval = state.intervals[i]; i++) {
                this.control.history.addAndRedo(new (this.getHistoryItemName())(modelManipulator, subDocument, layout, interval, this.control));
                this.control.history.addAndRedo(new FontCapsHistoryItem(modelManipulator, subDocument, interval, false, true));
            }
            this.control.history.endTransaction();
            return true;
        }

        getHistoryItemName(): new (modelManipulator: ModelManipulator, boundSubDocument: SubDocument, layout: DocumentLayout, interval: FixedInterval, control: IRichEditControl) => ChangeCaseHistoryItemBase {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class CapitalizeEachWordCaseCommand extends ChangeTextCaseCommandBase {
        getHistoryItemName(): new (modelManipulator: ModelManipulator, boundSubDocument: SubDocument, layout: DocumentLayout, interval: FixedInterval, control: IRichEditControl) => ChangeCaseHistoryItemBase {
            return CapitalizeEachWordCaseHistoryItem;
        }
    }

    export class MakeTextLowerCaseCommand extends ChangeTextCaseCommandBase {
        getHistoryItemName(): new (modelManipulator: ModelManipulator, boundSubDocument: SubDocument, layout: DocumentLayout, interval: FixedInterval, control: IRichEditControl) => ChangeCaseHistoryItemBase {
            return LowerCaseHistoryItem;
        }
    }

    export class MakeTextUpperCaseCommand extends ChangeTextCaseCommandBase {
        getHistoryItemName(): new (modelManipulator: ModelManipulator, boundSubDocument: SubDocument, layout: DocumentLayout, interval: FixedInterval, control: IRichEditControl) => ChangeCaseHistoryItemBase {
            return UpperCaseHistoryItem;
        }
    }

    export class ToggleTextCaseCommand extends ChangeTextCaseCommandBase {
        getHistoryItemName(): new (modelManipulator: ModelManipulator, boundSubDocument: SubDocument, layout: DocumentLayout, interval: FixedInterval, control: IRichEditControl) => ChangeCaseHistoryItemBase {
            return ToggleCaseHistoryItem;
        }
    }

    export class SentenceCaseCommand extends ChangeTextCaseCommandBase {
        getHistoryItemName(): new (modelManipulator: ModelManipulator, boundSubDocument: SubDocument, layout: DocumentLayout, interval: FixedInterval, control: IRichEditControl) => ChangeCaseHistoryItemBase {
            return SentenceCaseHistoryItem;
        }
    }

    export class SwitchTextCaseCommand extends ChangeTextCaseCommandBaseBase {
        executeCore(state: IntervalCommandStateEx): boolean {
            var executed = false;
            this.control.history.beginTransaction();
            for(let i = 0, interval: FixedInterval; interval = state.intervals[i]; i++) {
                let commandId: RichEditClientCommand = this.getCommand(interval);
                let command: ChangeTextCaseCommandBase = <ChangeTextCaseCommandBase>(this.control.commandManager.getCommand(commandId));
                executed = command.executeCore(state) || executed;
            }
            this.control.history.endTransaction();
            return executed;
        }

        private getCommand(interval: FixedInterval): RichEditClientCommand {
            var sentences: Sentence[] = SentenceStructureBuilder.getBuilder(this.control, this.control.model.activeSubDocument, this.control.layout, interval, true).sentences;

            var atLeastOneCharInNotInFirstPositionOnSentenceInUpperCase: boolean = false;
            var atLeastOneSentenceFullSelected: boolean = false;
            var atLeastOneFirstCharInSentenceInLowerCase: boolean = false;
            var atLeastOneFirstCharInWordInLowerCase: boolean = false;
            var allNotFirstCharsInWordsIsLowerCase: boolean = true;
            var allCharsInUpperCase: boolean = true;
            var allFirstCharsInWordsInUpperCase = true;
             
            externalLoop:
            for (var sentenceIndex: number = 0, sentence: Sentence; sentence = sentences[sentenceIndex]; sentenceIndex++) {
                if (sentence.words[0].parts[0].position >= interval.start && sentence.getLastWord().getLastWordPart().getEndPosition() <= interval.end())
                    atLeastOneSentenceFullSelected = true;

                for (var wordIndex: number = 0, wordInfo: SentenceWord; wordInfo = sentence.words[wordIndex]; wordIndex++) {
                    for (var wordPartIndex: number = 0, wordPart: WordPart; wordPart = wordInfo.parts[wordPartIndex]; wordPartIndex++) {
                        if (wordPart.position < interval.start)
                            continue;
                        if (wordPart.position >= interval.end())
                            break externalLoop;

                        if (wordPart.type == LayoutBoxType.Text) {
                            var firstWordChar: string = wordPart.text[0];
                            var otherWordChars: string = wordPart.text.substr(1);

                            if (wordIndex == 0 && wordPartIndex == 0) {
                                atLeastOneFirstCharInSentenceInLowerCase = atLeastOneFirstCharInSentenceInLowerCase || (Utils.stringInLowerCase(firstWordChar) && !Utils.stringInUpperCase(firstWordChar));
                                atLeastOneCharInNotInFirstPositionOnSentenceInUpperCase = atLeastOneCharInNotInFirstPositionOnSentenceInUpperCase || Utils.inStringAtLeastOneSymbolInUpperCase(otherWordChars);
                            }
                            else
                                atLeastOneCharInNotInFirstPositionOnSentenceInUpperCase = atLeastOneCharInNotInFirstPositionOnSentenceInUpperCase || Utils.inStringAtLeastOneSymbolInUpperCase(wordPart.text);

                            if (wordPartIndex == 0) {
                                atLeastOneFirstCharInWordInLowerCase = atLeastOneFirstCharInWordInLowerCase || (Utils.stringInLowerCase(firstWordChar) && !Utils.stringInUpperCase(firstWordChar));
                                allFirstCharsInWordsInUpperCase = allFirstCharsInWordsInUpperCase && Utils.stringInUpperCase(firstWordChar);
                            }

                            allNotFirstCharsInWordsIsLowerCase = allNotFirstCharsInWordsIsLowerCase && Utils.stringInLowerCase(otherWordChars);
                            allCharsInUpperCase = allCharsInUpperCase && Utils.stringInUpperCase(wordPart.text);
                        }
                    }
                }
            }

            if (allCharsInUpperCase)
                return RichEditClientCommand.MakeTextLowerCase;

            if (allFirstCharsInWordsInUpperCase && allNotFirstCharsInWordsIsLowerCase)
                return RichEditClientCommand.MakeTextUpperCase;

            if (atLeastOneSentenceFullSelected && (atLeastOneFirstCharInSentenceInLowerCase || atLeastOneCharInNotInFirstPositionOnSentenceInUpperCase))
                return RichEditClientCommand.SentenceCase;

            if (atLeastOneFirstCharInWordInLowerCase && allNotFirstCharsInWordsIsLowerCase)
                return RichEditClientCommand.CapitalizeEachWordTextCase;

            return RichEditClientCommand.MakeTextUpperCase;
        }
    }
} 