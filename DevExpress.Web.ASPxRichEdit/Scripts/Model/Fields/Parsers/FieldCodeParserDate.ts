module __aspxRichEdit {
    export class FieldCodeParserDate extends FieldCodeParser {
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
            // here responce always == null
            FieldCodeParserHelper.deleteFieldResultFromModel(this.control, this.subDocument, this.getTopField());
            if (this.parseSwitchesAndArgs(true))
                this.fillResult();
            this.parserState = FieldCodeParserState.end;
            return true;
        }

        private fillResult(): boolean {
            var dateFormatter = new DateTimeFieldFormatter();
            var currDate: Date = new Date();
            var dateText: string;

            var dateFormatSwitches: FieldSwitch[] = [];
            for (var i: number = 0, switchInfo: FieldSwitch; switchInfo = this.switchInfoList[i]; i++)
                if (switchInfo.type == FieldSwitchType.DateAndTime)
                    dateFormatSwitches.push(switchInfo);

            switch (dateFormatSwitches.length) {
                case 0:
                    dateText = dateFormatter.format(currDate, this.getDefaultFormat());
                    break;
                case 1:
                    dateText = dateFormatter.format(currDate, dateFormatSwitches[0].arg);
                    break;
                default: // error
                    break;
            }

            if (dateText) {
                this.setInputPositionState();
                this.control.modelManipulator.insertText(this.control, this.getTopField().getResultInterval(), dateText, false);
            }
            return true;
        }

        getDefaultFormat(): string {
            return "M/d/yyyy";
        }
    }
}