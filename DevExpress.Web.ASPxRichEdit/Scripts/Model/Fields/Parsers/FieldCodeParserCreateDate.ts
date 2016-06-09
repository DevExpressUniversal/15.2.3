module __aspxRichEdit {
    export class FieldCodeParserCreateDate extends FieldCodeParser {
        getMailMergeType(): FieldMailMergeType {
            return FieldMailMergeType.MailMerge;
        }
    }
}