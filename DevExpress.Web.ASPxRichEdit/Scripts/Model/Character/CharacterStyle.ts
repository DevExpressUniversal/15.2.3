module __aspxRichEdit {
    export class CharacterStyle extends StyleBase implements ICloneable<CharacterStyle> {
        public parent: CharacterStyle;
        public linkedStyle: ParagraphStyle;
        public maskedCharacterProperties: MaskedCharacterProperties;

        constructor(styleName: string, localizedName: string, deleted: boolean, hidden: boolean, semihidden: boolean, isDefault: boolean, maskedCharacterProperties: MaskedCharacterProperties) {
            super(styleName, localizedName, deleted, hidden, semihidden, isDefault);
            this.maskedCharacterProperties = maskedCharacterProperties;
        }

        public clone(): CharacterStyle {
            return new CharacterStyle(this.styleName, this.localizedName, this.deleted, this.hidden, this.semihidden, this.isDefault, this.maskedCharacterProperties);
        }
    }
}  