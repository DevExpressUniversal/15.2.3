module __aspxRichEdit {
    export class CharacterPropertiesMerger extends PropertiesMergerBase<CharacterPropertiesMask> {

        constructor() {
            super(new MaskedCharacterProperties());
        }

        public mergeCharacterProperties(maskedCharacterProperties: MaskedCharacterProperties) {
            this.merge(maskedCharacterProperties);
        }
        public mergeMergedCharacterProperties(mergedCharacterProperties: CharacterProperties) {
            var maskedCharacterProperties = new MaskedCharacterProperties();
            maskedCharacterProperties.copyFrom(mergedCharacterProperties);
            maskedCharacterProperties.useValue = CharacterPropertiesMask.UseAll;
            this.merge(maskedCharacterProperties);
        }

        public mergeCharacterStyle(characterStyle: CharacterStyle) {
            var currentCharacterStyle: CharacterStyle = characterStyle;
            while (currentCharacterStyle) {
                this.merge(currentCharacterStyle.maskedCharacterProperties);
                currentCharacterStyle = currentCharacterStyle.parent;
            }
        }

        public mergeParagraphStyle(paragraphStyle: ParagraphStyle) {
            var currentParagraphStyle: ParagraphStyle = paragraphStyle;
            while (currentParagraphStyle) {
                this.merge(currentParagraphStyle.maskedCharacterProperties);
                currentParagraphStyle = currentParagraphStyle.parent;
            }
        }

        public getMergedProperties(): CharacterProperties {
            delete this.innerProperties["useValue"]; // to not to be confuse call side
            return <CharacterProperties>(<MaskedCharacterProperties>this.innerProperties);
        }

        private merge(properties: MaskedCharacterProperties) {
            if (!properties)
                return;
            var innerProperty: MaskedCharacterProperties = <MaskedCharacterProperties>this.innerProperties;
            //this.mergeInternal(properties, CharacterPropertiesMask.UseLangInfo, () => innerProperty.?? = properties.??);// not yet
            this.mergeInternal(properties, CharacterPropertiesMask.UseHidden, () => innerProperty.hidden = properties.hidden);
            this.mergeInternal(properties, CharacterPropertiesMask.UseScript, () => innerProperty.script = properties.script);
            this.mergeInternal(properties, CharacterPropertiesMask.UseAllCaps, () => innerProperty.allCaps = properties.allCaps);
            this.mergeInternal(properties, CharacterPropertiesMask.UseNoProof, () => innerProperty.noProof = properties.noProof);
            this.mergeInternal(properties, CharacterPropertiesMask.UseFontBold, () => innerProperty.fontBold = properties.fontBold);
            this.mergeInternal(properties, CharacterPropertiesMask.UseFontName, () => innerProperty.fontInfo = properties.fontInfo);
            this.mergeInternal(properties, CharacterPropertiesMask.UseBackColor, () => innerProperty.backColor = properties.backColor);
            this.mergeInternal(properties, CharacterPropertiesMask.UseForeColor, () => innerProperty.foreColor = properties.foreColor);
            this.mergeInternal(properties, CharacterPropertiesMask.UseDoubleFontSize, () => innerProperty.fontSize = properties.fontSize);
            this.mergeInternal(properties, CharacterPropertiesMask.UseFontItalic, () => innerProperty.fontItalic = properties.fontItalic);
            this.mergeInternal(properties, CharacterPropertiesMask.UseStrikeoutColor, () => innerProperty.strikeoutColor = properties.strikeoutColor);
            this.mergeInternal(properties, CharacterPropertiesMask.UseUnderlineColor, () => innerProperty.underlineColor = properties.underlineColor);
            this.mergeInternal(properties, CharacterPropertiesMask.UseFontStrikeoutType, () => innerProperty.fontStrikeoutType = properties.fontStrikeoutType);
            this.mergeInternal(properties, CharacterPropertiesMask.UseFontUnderlineType, () => innerProperty.fontUnderlineType = properties.fontUnderlineType);
            this.mergeInternal(properties, CharacterPropertiesMask.UseStrikeoutWordsOnly, () => innerProperty.strikeoutWordsOnly = properties.strikeoutWordsOnly);
            this.mergeInternal(properties, CharacterPropertiesMask.UseUnderlineWordsOnly, () => innerProperty.underlineWordsOnly = properties.underlineWordsOnly);
        }
    }
} 