module __aspxRichEdit {
    export class ToggleAllFieldsCommand extends CommandBase<IntervalCommandState> {
        getState(): IntervalCommandState {
            return new IntervalCommandState(this.isEnabled(), this.control.selection.getLastSelectedInterval());
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.fields);
        }

        executeCore(state: IntervalCommandState, parameter: any): boolean {
            var subDocument: SubDocument = this.control.model.activeSubDocument;
            Field.DEBUG_FIELDS_CHECKS(subDocument);
            var subDocumentsList: SubDocument[] = [subDocument];
            var atLeastExistOneField: boolean = false;
            for (var subDocumentIndex: number = 0, currSubDoc: SubDocument; currSubDoc = subDocumentsList[subDocumentIndex]; subDocumentIndex++) {
                var fields: Field[] = currSubDoc.fields;
                if (fields.length > 0)
                    atLeastExistOneField = true;
                for (var fieldIndex: number = 0, field: Field; field = fields[fieldIndex]; fieldIndex++)
                    if (field.showCode) {
                        this.control.commandManager.getCommand(RichEditClientCommand.ShowAllFieldResults).execute(null);
                        return true;
                    }
            }
            if (atLeastExistOneField) {
                this.control.commandManager.getCommand(RichEditClientCommand.ShowAllFieldCodes).execute(null);
                return true;
            }
            return false;
        }

        isEnabledInReadOnlyMode(): boolean {
            return true;
        }
    }
} 