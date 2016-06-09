module __aspxRichEdit {
    export class FieldCodeParserNumPages extends FieldCodeParser {
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
            this.setInputPositionState();

            if(this.subDocument.isMain()) {
                var numPagesText: string = this.control.layout.lastMaxNumPages.toString();
                this.control.modelManipulator.insertText(this.control, this.getTopField().getResultInterval(), numPagesText, false);
            }
            else
                this.control.modelManipulator.insertLayoutDependentText(this.control, this.getTopField().getResultInterval());
            return true;
        }

    }
}