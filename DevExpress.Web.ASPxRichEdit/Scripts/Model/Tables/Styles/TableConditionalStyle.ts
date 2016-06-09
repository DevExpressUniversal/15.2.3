module __aspxRichEdit {
    export class TableConditionalStyle {
        tableProperties: TableProperties;
        tableRowProperties: TableRowProperties;
        tableCellProperties: TableCellProperties;

        maskedParagraphProperties: MaskedParagraphProperties;
        maskedCharacterProperties: MaskedCharacterProperties;
        
        tabs: TabProperties;

        constructor(tableProperties: TableProperties, tableRowProperties: TableRowProperties, tableCellProperties: TableCellProperties, maskedParagraphProperties: MaskedParagraphProperties,
            maskedCharacterProperties: MaskedCharacterProperties, tabs: TabProperties) {
            this.tableProperties = tableProperties;
            this.tableRowProperties = tableRowProperties;
            this.tableCellProperties = tableCellProperties;
            this.maskedParagraphProperties = maskedParagraphProperties;
            this.maskedCharacterProperties = maskedCharacterProperties;
            this.tabs = tabs;
        }

        public static addConditionalStyle(conditionalStyles: { [typeId: number]: TableConditionalStyle }, type: ConditionalTableStyleFormatting, conditionalStyle: TableConditionalStyle) {
            conditionalStyles[type] = conditionalStyle;
        }
    }
}