module __aspxRichEdit {
    export class FieldCodeParserTime extends FieldCodeParserDate {
        getDefaultFormat(): string {
            return "H:mm";
        }
    }
}