module __aspxRichEdit {
    export class ToggleMultiLevelListCommand extends NumberingListCommandBase {
        getNumberingListType(): NumberingType {
            return NumberingType.MultiLevel;
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.numberingMultiLevel);
        }
    }
} 