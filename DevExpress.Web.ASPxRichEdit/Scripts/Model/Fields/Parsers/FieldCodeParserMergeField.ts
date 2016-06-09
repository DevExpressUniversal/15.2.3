module __aspxRichEdit {
    export class FieldCodeParserMailMerge extends FieldCodeParserDocVariable {
        getMailMergeType(): FieldMailMergeType {
            return FieldMailMergeType.MailMerge;
        }

        getServerUpdateFieldType(): ServerUpdateFieldType {
            return ServerUpdateFieldType.MergeField;
        }

        protected insertDefaultText(): boolean {
            if(this.control.mailMergeOptions.isEnabled && this.control.mailMergeOptions.viewMergedData)
                return false;
            var defaultText: string = ASPx.Formatter.Format("<<{0}>>", this.parameterInfoList[0].text);
            this.setInputPositionState();
            this.control.modelManipulator.insertText(this.control, this.getTopField().getResultInterval(), defaultText, false);
            return true;
        }

        protected getRequestData(): any {
            return { name: this.parameterInfoList[0].text };
        }

        protected applyResponse(response: any): boolean {
            var fieldResultInterval: FixedInterval = this.getTopField().getResultInterval();
            this.setInputPositionState();
            var simpleText: string = response["text"];
            if(simpleText !== null && simpleText != "") {
                this.control.modelManipulator.insertText(this.control, fieldResultInterval, simpleText, false);
                return true;
            }
            return false;
        }
    }
}