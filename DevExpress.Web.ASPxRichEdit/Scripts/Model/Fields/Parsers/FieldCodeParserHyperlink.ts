module __aspxRichEdit {
    export class FieldCodeParserHyperlink extends FieldCodeParser {
        private switchInfoList: FieldSwitch[] = [];
        private parameterInfoList: FieldParameter[] = [];

        getMailMergeType(): FieldMailMergeType {
            return FieldMailMergeType.NonMailMerge;
        }

        handleSwitch(newSwitch: FieldSwitch): boolean {
            this.switchInfoList.push(newSwitch);
            return true;
        }

        handleParameter(newParameter: FieldParameter): boolean {
            this.parameterInfoList.push(newParameter);
            return true;
        }

        // always return this.parserState = FieldCodeParserState.end;
        parseCodeCurrentFieldInternal(responce: any): boolean {
            if (this.parseSwitchesAndArgs(true))
                this.fillResult();
            else
                FieldCodeParserHelper.deleteFieldResultFromModel(this.control, this.subDocument, this.getTopField());
            this.parserState = FieldCodeParserState.end;
            return true;
        }

        private fillResult(): boolean {
            var field: Field = this.getTopField();
            var text: string = this.parameterInfoList[0] ? this.parameterInfoList[0].text : "";

            var oldTip: string = field.isHyperlinkField() && field.getHyperlinkInfo().tip != "" ? field.getHyperlinkInfo().tip : "";

            var newHyperlinkInfo: HyperlinkInfo = this.updateHyperlinkInfo(field, text)
            if (!newHyperlinkInfo) {
                FieldCodeParserHelper.deleteFieldResultFromModel(this.control, this.subDocument, this.getTopField());
                return true;
            }

            var modelManipulator: ModelManipulator = this.control.modelManipulator;
            var resultInterval: FixedInterval = field.getResultInterval();
            if (resultInterval.length == 0) {
                var resultText: string = text.length > 0 ? text : "#" + newHyperlinkInfo.anchor;
                var newResultInterval: FixedInterval = new FixedInterval(resultInterval.start, resultText.length);
                var selectionEnd: number = newResultInterval.end();

                this.setInputPositionState();
                modelManipulator.insertText(this.control, resultInterval, resultText, false);

                this.control.history.addAndRedo(new ApplyFieldHyperlinkStyleHistoryItem(modelManipulator, this.subDocument, newResultInterval));
                this.control.selection.setSelection(selectionEnd + 1, selectionEnd + 1, false, -1, UpdateInputPositionProperties.No);
            }
            this.control.history.addAndRedo(new ChangeFieldHyperlinkInfoHistoryItem(modelManipulator, this.subDocument, field.index, newHyperlinkInfo));
            return true;
        }
        
        private updateHyperlinkInfo(field: Field, text: string): HyperlinkInfo {
            var newHyperlinkInfo: HyperlinkInfo = field.isHyperlinkField() ? field.getHyperlinkInfo().clone() : new HyperlinkInfo("", "", "", false);
            newHyperlinkInfo.visited = false;

            var tipSwitch: FieldSwitch;
            var bookmarkSwitch: FieldSwitch;
            for (var i: number = 0, switchInfo: FieldSwitch; switchInfo = this.switchInfoList[i]; i++)
                if (switchInfo.type == FieldSwitchType.FieldSpecific) {
                    switch (switchInfo.name.toLocaleUpperCase()) {
                        case "O": tipSwitch = switchInfo; break;
                        case "L": bookmarkSwitch = switchInfo; break;
                    }
                }

            newHyperlinkInfo.tip = tipSwitch ? tipSwitch.arg : "";

            var splitted: string[] = text.split("#");
            if (splitted.length > 2)
                return null;

            if (splitted.length == 1) {
                newHyperlinkInfo.uri = splitted[0];
                newHyperlinkInfo.anchor = bookmarkSwitch ? bookmarkSwitch.arg : "";
                if (newHyperlinkInfo.uri == "" && newHyperlinkInfo.anchor == "")
                    return null;
            }
            else {
                // ignore switch L
                newHyperlinkInfo.uri = splitted[0];
                newHyperlinkInfo.anchor = splitted[1];
            }

            return newHyperlinkInfo;
        }

    }
}