module __aspxRichEdit {
    export class InsertSectionBreakCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.sections);
        }

        // parameter must be null or undefined
        executeCore(state: SimpleCommandState, parameter: any): boolean {
            ModelManipulator.insertSectionAndSetStartType(this.control, this.control.selection.getLastSelectedInterval().start, this.getStartType());
            return true;
        }

        getStartType(): SectionStartType {
            throw new Error(Errors.NotImplemented);
        }
    }

    export class InsertSectionBreakNextPageCommand extends InsertSectionBreakCommand {
        getStartType(): SectionStartType {
            return SectionStartType.NextPage;
        }
    }

    export class InsertSectionBreakEvenPageCommand extends InsertSectionBreakCommand {
        getStartType(): SectionStartType {
            return SectionStartType.EvenPage;
        }
    }

    export class InsertSectionBreakOddPageCommand extends InsertSectionBreakCommand {
        getStartType(): SectionStartType {
            return SectionStartType.OddPage;
        }
    }
} 