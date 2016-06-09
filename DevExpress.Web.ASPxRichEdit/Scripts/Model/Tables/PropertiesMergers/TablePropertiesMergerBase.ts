module __aspxRichEdit {
    export class TablePropertiesMergerBase<PropertiesContainerType, ResultPropertyType> {
        protected result: ResultPropertyType;

        public getProperty(container: PropertiesContainerType, style: TableStyle, condStyleFormattingFlags: ConditionalTableStyleFormatting, defaultContainer: PropertiesContainerType): ResultPropertyType {
            if (this.getPropertyInternal(container))
                return this.result;
            while (style) {
                const condStyleList: ConditionalTableStyleFormatting[] = this.getCondTableStyleFormattingListForThisContainer();
                for (let cond of condStyleList)
                    if (condStyleFormattingFlags & cond) {
                        const condStyle: TableConditionalStyle = style.conditionalStyles[cond];
                        if (condStyle && this.getPropertyInternal(this.getContainerFromConditionalStyle(condStyle)))
                            return this.result;
                    }
                const baseConditionalStyleContainer: PropertiesContainerType = this.getContainerFromConditionalStyle(style.baseConditionalStyle);
                if (baseConditionalStyleContainer && this.getPropertyInternal(baseConditionalStyleContainer))
                    return this.result;
                style = style.parent;
            }

            if (this.actionBeforeDefaultValue())
                return this.result;

            return this.getPropertyFromContainer(defaultContainer);
        }

        protected getContainerFromConditionalStyle(condStyle: TableConditionalStyle): PropertiesContainerType {
            throw new Error(Errors.NotImplemented);
        }

        protected canUseValue(props: PropertiesContainerType): boolean {
            throw new Error(Errors.NotImplemented);
        }

        protected getCondTableStyleFormattingListForThisContainer(): ConditionalTableStyleFormatting[] {
            throw new Error(Errors.NotImplemented);
        }

        protected getPropertyFromContainer(container: PropertiesContainerType): ResultPropertyType {
            throw new Error(Errors.NotImplemented);
        }

        protected getPropertyMask(): number {
            throw new Error(Errors.NotImplemented);
        }

        protected actionBeforeDefaultValue(): boolean {
            return false;
        }

        private getPropertyInternal(container: PropertiesContainerType): boolean {
            if (!this.canUseValue(container))
                return false;
            this.result = this.getPropertyFromContainer(container);
            return true;
        }
    }
}