module __aspxRichEdit {
    export class TableCellStyle extends StyleBase implements ICloneable<TableCellStyle> {
        parent: TableCellStyle;
        tableCellProperties: TableCellProperties;
        characterProperties: CharacterProperties;

        constructor(styleName: string, localizedName: string, deleted: boolean, hidden: boolean, semihidden: boolean, isDefault: boolean,
            tableCellProperties: TableCellProperties, characterProperties: CharacterProperties) {
            super(styleName, localizedName, deleted, hidden, semihidden, isDefault);
            this.tableCellProperties = tableCellProperties;
            this.characterProperties = characterProperties;
        }

        clone(): TableCellStyle {
            return new TableCellStyle(this.styleName, this.localizedName, this.deleted, this.hidden, this.semihidden, this.isDefault, this.tableCellProperties, this.characterProperties);
        }
    }
}