module __aspxRichEdit {
    export class StyleBase implements ICloneable<StyleBase> {
        public styleName: string;
        public localizedName: string;

        public deleted: boolean;
        public hidden: boolean;
        public semihidden: boolean;

        public isDefault: boolean;

        constructor(styleName: string, localizedName: string, deleted: boolean, hidden: boolean, semihidden: boolean, isDefault: boolean) {
            this.styleName = styleName;
            this.deleted = deleted;
            this.localizedName = localizedName;
            this.hidden = hidden;
            this.semihidden = semihidden;
            this.isDefault = isDefault;
        }

        public clone(): StyleBase {
            throw new Error(Errors.NotImplemented);
        }

        public equalsByName(obj: StyleBase): boolean {
            return this.styleName == obj.styleName;
        }
    }
} 