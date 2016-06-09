module __aspxRichEdit {

    export class DialogHyperlinkCommandBase extends ShowDialogCommandBase {
        getState(): SimpleCommandState {
            var state = new SimpleCommandState(this.isEnabled());
            state.value = this.getStateValue();
            state.visible = this.isVisible()
            return state;
        }
        getStateValue(): Field {
            return this.getSelectedField();
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.hyperlinks) && this.control.selection.intervals.length === 1;
        }
        isVisible(): boolean {
            return true;
        }
        createParameters(): DialogHyperlinkParameters {
            var parameters: DialogHyperlinkParameters = new DialogHyperlinkParameters();
            parameters.canChangeDisplayText = this.canChangeDisplayText();
            var field: Field = this.getState().value
            if (field) {
                var hyperlinkInfo: HyperlinkInfo = field.getHyperlinkInfo();
                //parameters.url = hyperlinkInfo.uri + (hyperlinkInfo.anchor != "" ? "#" + hyperlinkInfo.anchor : ""); //TODO
                parameters.url = hyperlinkInfo.uri;
                parameters.anchor = hyperlinkInfo.anchor;
                parameters.tooltip = hyperlinkInfo.tip;
                parameters.text = FieldContextMenuHelper.getHyperlinkResultText(this.control.model.activeSubDocument, field);
            } else
                parameters.text = this.control.model.activeSubDocument.getText(this.control.selection.intervals[0]);

            for (let b of this.control.model.activeSubDocument.bookmarks)
                parameters.bookmarkNames.push(b.name);

            return parameters;
        }
        canChangeDisplayText(): boolean {
            var iterator: ModelIterator = new ModelIterator(this.control.model.activeSubDocument, false);
            iterator.setPosition(this.control.selection.intervals[0].start);
            do {
                if (iterator.getCurrectPosition() >= this.control.selection.intervals[0].end())
                    break;
                if (iterator.run.type == TextRunType.InlinePictureRun)
                    return false;
            } while(iterator.moveToNextRun())
            return true;
        }
        applyParameters(state: IntervalCommandState, newParams: DialogHyperlinkParameters) {
            var initParams: DialogHyperlinkParameters = <DialogHyperlinkParameters>this.initParams;
            if (newParams.tooltip == initParams.tooltip && newParams.url == initParams.url && newParams.anchor == initParams.anchor && newParams.text == initParams.text)
                return;

            var hyperlinkInfo: HyperlinkInfo = new HyperlinkInfo(newParams.url, newParams.anchor, newParams.tooltip, false);
            if (hyperlinkInfo.anchor == "" && hyperlinkInfo.uri == "")
                return;

            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            var selection: Selection = this.control.selection;
            var subDocument: SubDocument = this.control.model.activeSubDocument;
            var history: IHistory = this.control.history;
            var oldSelection: FixedInterval = selection.getLastSelectedInterval();
            if (oldSelection.end() == subDocument.getDocumentEndPosition()) {
                oldSelection.length--;
                if (oldSelection.length < 0)
                    return;
            }

            history.beginTransaction();
            this.control.beginUpdate();

            var field: Field = this.getField(state.value);

            history.addAndRedo(new ChangeFieldHyperlinkInfoHistoryItem(modelManipulator, subDocument, field.index, hyperlinkInfo));

            if (newParams.tooltip != initParams.tooltip || newParams.url != initParams.url || newParams.anchor != initParams.anchor) {
                selection.setSelection(field.getCodeStartPosition(), field.getSeparatorPosition(), false, selection.keepX, UpdateInputPositionProperties.No, false);
                this.control.commandManager.getCommand(RichEditClientCommand.InsertText).execute(this.getNewCodeText(hyperlinkInfo));
            }

            if (initParams.canChangeDisplayText && newParams.text != initParams.text || field.getResultInterval().length == 0) {
                selection.setSelection(field.getResultStartPosition(), field.getResultEndPosition(), false, selection.keepX, UpdateInputPositionProperties.No, false);
                this.control.commandManager.getCommand(RichEditClientCommand.InsertText).execute(!newParams.text || newParams.text == "" ? hyperlinkInfo.getUriPlusAnchor() : newParams.text);
            }

            history.addAndRedo(new ApplyFieldHyperlinkStyleHistoryItem(modelManipulator, this.control.model.activeSubDocument, field.getResultInterval()));
            selection.setSelection(field.getFieldEndPosition(), field.getFieldEndPosition(), false, selection.keepX, UpdateInputPositionProperties.No, false);

            this.control.endUpdate();
            history.endTransaction();
        }
        private getNewCodeText(hyperlinkInfo: HyperlinkInfo): string {
            return [
                " HYPERLINK \"",
                hyperlinkInfo.uri,
                "\" ",
                hyperlinkInfo.tip == "" ? "" : "\\o \"" + hyperlinkInfo.tip + "\" ",
                hyperlinkInfo.anchor == "" ? "" : "\\l \"" + hyperlinkInfo.anchor + "\" "
            ].join("")
        }
        getField(stateValue: any): Field {
            if (stateValue) {
                var field: Field = <Field>stateValue;
                field.showCode = false; // for good here need insert SetFieldShowCodeHistoryItem to false...
            } else {
                var selection: Selection = this.control.selection;
                var subDocument: SubDocument = this.control.model.activeSubDocument;
                var initSelectionInterval: FixedInterval = selection.getLastSelectedInterval();
                this.control.history.addAndRedo(new FieldInsertHistoryItem(this.control, this.control.modelManipulator, subDocument, initSelectionInterval.start, 0, initSelectionInterval.length, false));
                var fieldIndex: number = Field.normedBinaryIndexOf(subDocument.fields, initSelectionInterval.start + 1);
                var field: Field = subDocument.fields[fieldIndex];
            }
            return field;
        }
        showCreateHyperlinkItem(): boolean {
            return FieldContextMenuHelper.showCreateHyperlinkItem(this.control.model.activeSubDocument.fields, this.control.selection.getLastSelectedInterval())
        }
        getSelectedField(): Field {
            return FieldContextMenuHelper.showHyperlinkItems(this.control.model.activeSubDocument.fields, this.control.selection.getLastSelectedInterval())
        }
        hasOneSelectedHyperlink(): boolean {
            return !!this.getSelectedField();
        }
        getDialogName() {
            return "Hyperlink";
        }
    }

    export class DialogCreateOrEditHyperlinkCommand extends DialogHyperlinkCommandBase {
        isEnabled(): boolean {
            return super.isEnabled() && (this.showCreateHyperlinkItem() || this.hasOneSelectedHyperlink());
        }
    }

    export class DialogCreateHyperlinkCommand extends DialogHyperlinkCommandBase {
        isVisible(): boolean {
            return this.showCreateHyperlinkItem();
        }
    }

    export class DialogEditHyperlinkCommand extends DialogHyperlinkCommandBase {
        isVisible(): boolean {
            return this.hasOneSelectedHyperlink();
        }
    }

    export class DialogHyperlinkParameters extends DialogParametersBase {
        url: string = "";
        text: string = "";
        tooltip: string = "";
        anchor: string = "";

        bookmarkNames: string[] = [];
        canChangeDisplayText: boolean = true;

        copyFrom(obj: DialogHyperlinkParameters) {
            this.url = obj.url;
            this.text = obj.text;
            this.tooltip = obj.tooltip;
            this.canChangeDisplayText = obj.canChangeDisplayText;
            this.bookmarkNames = obj.bookmarkNames;
        }

        getNewInstance(): DialogParametersBase {
            return new DialogHyperlinkParameters();
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
        }
    }
}