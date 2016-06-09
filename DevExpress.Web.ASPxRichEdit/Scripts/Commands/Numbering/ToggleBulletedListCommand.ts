module __aspxRichEdit {
    export class ToggleBulletedListCommand extends ToggleNumberingListCommand {
        getNumberingListType(): NumberingType {
            return NumberingType.Bullet;
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.numberingBulleted);
        }
    }
}  