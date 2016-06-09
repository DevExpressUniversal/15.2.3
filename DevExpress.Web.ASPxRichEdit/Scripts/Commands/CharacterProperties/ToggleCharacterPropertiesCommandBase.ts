module __aspxRichEdit {
    export class ToggleCharacterPropertiesCommandBase extends ChangeCharacterPropertiesCommandBase<boolean> {
        getActualValue(parameter: any, currentValue: boolean): boolean {
            return !currentValue;
        }
    }
}  