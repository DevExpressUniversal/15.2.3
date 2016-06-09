module __aspxRichEdit {
    export class UpdateAllFieldsCommand extends CommandBase<SimpleCommandState> {
        getState(): SimpleCommandState {
            return new SimpleCommandState(this.isEnabled());
        }

        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.fields);
        }

        executeCore(state: SimpleCommandState, callbackFunc?: any): boolean {
            const modelManipulator: ModelManipulator = this.control.modelManipulator;
            const subDocument = modelManipulator.model.activeSubDocument;

            if (subDocument.fields.length == 0) {
                if (callbackFunc)
                    callbackFunc();
                return false;
            }

            const selection: Selection = this.control.selection;
            const history: IHistory = this.control.history;
            const commandManager: CommandManager = this.control.commandManager;

            Field.DEBUG_FIELDS_CHECKS(subDocument);

            var selectionStateInfo: SelectionStateInfo = selection.getSelectionState();
            var allDocumentInterval: FixedInterval = new FixedInterval(0, subDocument.getDocumentEndPosition());

            history.beginTransaction();
            history.addAndRedo(new SetSelectionHistoryItem(modelManipulator, subDocument, [allDocumentInterval], selection, UpdateInputPositionProperties.No, false));
            commandManager.getCommand(RichEditClientCommand.UpdateField).execute(() => {
                selection.restoreSelectionState(selectionStateInfo, UpdateInputPositionProperties.Yes);
                if (callbackFunc)
                    callbackFunc();
            });
            history.endTransaction();

            return true;
        }
        //executeCore(state: SimpleCommandState, callbackFunc?: any): boolean {
        //    const modelManipulator: ModelManipulator = this.control.modelManipulator;
        //    const model: DocumentModel = modelManipulator.model;
        //    const selection: Selection = this.control.selection;
        //    const history: IHistory = this.control.history;
        //    var commandManager: CommandManager = this.control.commandManager;
        //    var subDocuments: SubDocument[] = model.getSubDocumentsList();

        //    let howManySubDocsNeedUpdate: number = 0;
        //    for (let subDocument of subDocuments) {
        //        if (subDocument.fields.length > 0)
        //            howManySubDocsNeedUpdate++;
        //    }

        //    if (howManySubDocsNeedUpdate == 0) {
        //        if (callbackFunc)
        //            callbackFunc();
        //        return false;
        //    }

        //    if (howManySubDocsNeedUpdate > 1) {
        //        this.allFieldsInAllSubDocumentsAreStartUpdate = false;
        //        history.beginTransaction();
        //    }
        //    else
        //        this.allFieldsInAllSubDocumentsAreStartUpdate = true;

        //    const oldActiveSubDocument: SubDocument = model.activeSubDocument;
        //    for (let subDocument of subDocuments) {
        //        if (subDocument.fields.length == 0)
        //            continue;

        //        Field.DEBUG_FIELDS_CHECKS(subDocument);

        //        var selectionStateInfo: SelectionStateInfo = selection.getSelectionState();
        //        const allDocumentInterval: FixedInterval = new FixedInterval(0, subDocument.getDocumentEndPosition());

        //        history.beginTransaction();
        //        history.addAndRedo(new SetSelectionHistoryItem(modelManipulator, subDocument, [allDocumentInterval], selection, UpdateInputPositionProperties.No, false));
        //        model.activeSubDocument = subDocument;
        //        commandManager.getCommand(RichEditClientCommand.UpdateField).execute(() => {
        //            selection.restoreSelectionState(selectionStateInfo, UpdateInputPositionProperties.Yes);

        //            // 100% determine that func call only once and in END of update
        //            if (callbackFunc) {
        //                if ((<UpdateAllFieldsCommand>commandManager.getCommand(RichEditClientCommand.UpdateAllFields)).allFieldsInAllSubDocumentsAreStartUpdate) {
        //                    let allSubDocsUpdated: boolean = true;
        //                    for (let subDocument of subDocuments)
        //                        if (subDocument.fieldsWaitingForUpdate)
        //                            allSubDocsUpdated = false;
        //                    if (allSubDocsUpdated)
        //                        callbackFunc();
        //                }
        //            }
        //        });

        //        history.endTransaction();
        //    }

        //    if (howManySubDocsNeedUpdate > 1) {
        //        history.endTransaction();
        //        this.allFieldsInAllSubDocumentsAreStartUpdate = true;
        //    }
        //    model.activeSubDocument = oldActiveSubDocument;

        //    return true;
        //}
    }
} 