module __aspxRichEdit {
    export class PropertiesMergerBase<TMask> {
        innerProperties: IMaskedProperties<TMask>; // use for collect all properties and return to call side in getMergedProperties

        constructor(initialialProperties: IMaskedProperties<TMask>) {
            this.innerProperties = initialialProperties;
        }

        mergeInternal(properties: IMaskedProperties<TMask>, mask: TMask, setValue: ISetPropertiesDelegate) {
            if (!this.innerProperties.getUseValue(mask) && properties.getUseValue(mask)) {
                setValue();
                this.innerProperties.setUseValue(mask, true);
            }
        }

        public getMergedProperties(): any {
            throw new Error(Errors.NotImplemented);
        }
    }

    export interface ISetPropertiesDelegate {
        (): void;
    }
}