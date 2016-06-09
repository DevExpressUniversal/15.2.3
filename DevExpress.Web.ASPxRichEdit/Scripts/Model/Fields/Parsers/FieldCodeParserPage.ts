module __aspxRichEdit {
    export class FieldCodeParserPage extends FieldCodeParser {
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
            var field: Field = this.getTopField();
            var pos: number = field.showCode ? field.getCodeStartPosition() : field.getResultStartPosition();

            this.setInputPositionState();
            if(this.subDocument.isMain()) {
                var layoutPosition: LayoutPosition = LayoutPositionMainSubDocumentCreator.ensureLayoutPosition(this.control, this.control.layout, this.subDocument, pos,
                    DocumentLayoutDetailsLevel.Page, new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(true));
                var pageText = (layoutPosition.pageIndex + 1).toString();
                this.control.modelManipulator.insertText(this.control, this.getTopField().getResultInterval(), pageText, false);
            }
            else
                this.control.modelManipulator.insertLayoutDependentText(this.control, this.getTopField().getResultInterval());

            return true;
        }

    }
}