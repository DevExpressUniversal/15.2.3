module __aspxRichEdit {
    export class TableStyle extends StyleBase implements ICloneable<TableStyle> {
        static SIMPLE_STYLENAME = "Table Simple 1";
        static DEFAULT_STYLENAME = "Normal Table";

        parent: TableStyle;
        baseConditionalStyle: TableConditionalStyle; // todo. DELETE THAT
        conditionalStyles: { [typeId: number]: TableConditionalStyle } = {};

        constructor(styleName: string, localizedName: string, deleted: boolean, hidden: boolean, semihidden: boolean, isDefault: boolean,
            baseConditionalStyle: TableConditionalStyle, conditionalStyles: { [typeId: number]: TableConditionalStyle }) {
            super(styleName, localizedName, deleted, hidden, semihidden, isDefault);
            this.conditionalStyles = conditionalStyles;
            this.baseConditionalStyle = baseConditionalStyle;
        }

        clone(): TableStyle {
            var newStyle = new TableStyle(this.styleName, this.localizedName, this.deleted, this.hidden, this.semihidden, this.isDefault, this.baseConditionalStyle, this.conditionalStyles);
            return newStyle;
        }
    }
}