module __aspxRichEdit {
    export class StylesManager {
        documentModel: DocumentModel;
        characterStyleNameToIndex: { [name: string]: number } = {};
        paragraphStyleNameToIndex: { [name: string]: number } = {};
        numberingListStyleNameToIndex: { [name: string]: number } = {};
        tableStyleNameToIndex: { [name: string]: number } = {};
        tableCellStyleNameToIndex: { [name: string]: number } = {};

        private defaultCharacterStyle: CharacterStyle;
        private defaultParagraphStyle: ParagraphStyle;

        constructor(documentModel: DocumentModel) {
            this.documentModel = documentModel;
        }

        registerLink(characterStyle: CharacterStyle, paragraphStyle: ParagraphStyle) {
            characterStyle.linkedStyle = paragraphStyle;
            paragraphStyle.linkedStyle = characterStyle;
        }
        unregisterLink(characterStyle: CharacterStyle, paragraphStyle: ParagraphStyle) { //TODO
            characterStyle.linkedStyle = null;
            paragraphStyle.linkedStyle = null;
        }

        getCharacterStyleByName(styleName: string): CharacterStyle {
            return <CharacterStyle>this.getStyleByNameCore(styleName, this.documentModel.characterStyles, this.characterStyleNameToIndex);
        }
        getParagraphStyleByName(styleName: string): ParagraphStyle {
            return <ParagraphStyle>this.getStyleByNameCore(styleName, this.documentModel.paragraphStyles, this.paragraphStyleNameToIndex);
        }
        getNumberingListStyleByName(styleName: string): NumberingListStyle {
            return <NumberingListStyle>this.getStyleByNameCore(styleName, this.documentModel.numberingListStyles, this.numberingListStyleNameToIndex);
        }
        getTableStyleByName(styleName: string): TableStyle {
            return <TableStyle>this.getStyleByNameCore(styleName, this.documentModel.tableStyles, this.tableStyleNameToIndex);
        }
        getTableCellStyleByName(styleName: string): TableCellStyle {
            return <TableCellStyle>this.getStyleByNameCore(styleName, this.documentModel.tableCellStyles, this.tableCellStyleNameToIndex);
        }

        getDefaultCharacterStyle(): CharacterStyle {
            return this.defaultCharacterStyle || this.getDefaultStyleCore<CharacterStyle>(this.documentModel.characterStyles, (style) => {
                this.defaultCharacterStyle = style;
            });
        }

        getDefaultParagraphStyle(): ParagraphStyle {
            return this.defaultParagraphStyle || this.getDefaultStyleCore<ParagraphStyle>(this.documentModel.paragraphStyles, (style) => {
                this.defaultParagraphStyle = style;
            });
        }

        addCharacterStyle(style: CharacterStyle): CharacterStyle {
            return style ? this.getCharacterStyleByName(style.styleName) || this.addCharacterStyleCore(style) : null;
        }
        removeLastStyle(): void {
            var style = this.documentModel.characterStyles.pop();
            delete this.characterStyleNameToIndex[style.styleName];
        }

        addParagraphStyle(style: ParagraphStyle): ParagraphStyle {
            return style ? this.getParagraphStyleByName(style.styleName) || this.addParagraphStyleCore(style) : null;
        }

        addTableStyle(style: TableStyle): TableStyle {
            return style ? this.getTableStyleByName(style.styleName) || this.addTableStyleCore(style) : null;
        }

        addTableCellStyle(style: TableCellStyle): TableCellStyle {
            return style ? this.getTableCellStyleByName(style.styleName) || this.addTableCellStyleCore(style) : null;
        }

        private addTableStyleCore(oldStyle: TableStyle): TableStyle {
            var newStyle = oldStyle.clone();
            this.tableStyleNameToIndex[newStyle.styleName] = this.documentModel.tableStyles.push(newStyle) - 1;
            newStyle.baseConditionalStyle = this.cloneTableConditionalStyle(oldStyle.baseConditionalStyle);
            for(let type in oldStyle.conditionalStyles) {
                if(!oldStyle.conditionalStyles.hasOwnProperty(type)) continue;
                newStyle.conditionalStyles[type] = this.cloneTableConditionalStyle(oldStyle.conditionalStyles[type]);
            }
            return newStyle;
        }

        private addTableCellStyleCore(oldStyle: TableCellStyle): TableCellStyle {
            var newStyle = oldStyle.clone();
            newStyle.characterProperties = this.documentModel.cache.mergedCharacterPropertiesCache.addItemIfNonExists(oldStyle.characterProperties);
            newStyle.tableCellProperties = this.documentModel.cache.tableCellPropertiesCache.addItemIfNonExists(oldStyle.tableCellProperties);
            return newStyle;
        }

        private cloneTableConditionalStyle(style: TableConditionalStyle): TableConditionalStyle {
            return new TableConditionalStyle(
                style.tableProperties.clone(),
                this.documentModel.cache.tableRowPropertiesCache.addItemIfNonExists(style.tableRowProperties),
                this.documentModel.cache.tableCellPropertiesCache.addItemIfNonExists(style.tableCellProperties),
                this.documentModel.cache.maskedParagraphPropertiesCache.addItemIfNonExists(style.maskedParagraphProperties),
                this.documentModel.cache.maskedCharacterPropertiesCache.addItemIfNonExists(style.maskedCharacterProperties),
                style.tabs.clone()
            );
        }

        private addCharacterStyleCore(oldStyle: CharacterStyle): CharacterStyle {
            var newStyle: CharacterStyle = oldStyle.clone();
            this.characterStyleNameToIndex[newStyle.styleName] = this.documentModel.characterStyles.push(newStyle) - 1;
            newStyle.maskedCharacterProperties = this.documentModel.cache.maskedCharacterPropertiesCache.addItemIfNonExists(oldStyle.maskedCharacterProperties);
            newStyle.parent = this.addCharacterStyle(oldStyle.parent);
            newStyle.linkedStyle = this.addParagraphStyle(oldStyle.linkedStyle);
            return newStyle;
        }
        private addParagraphStyleCore(oldStyle: ParagraphStyle): ParagraphStyle {
            var newStyle: ParagraphStyle = oldStyle.clone();
            this.paragraphStyleNameToIndex[newStyle.styleName] = this.documentModel.paragraphStyles.push(newStyle) - 1;
            newStyle.maskedCharacterProperties = this.documentModel.cache.maskedCharacterPropertiesCache.addItemIfNonExists(oldStyle.maskedCharacterProperties);
            newStyle.maskedParagraphProperties = this.documentModel.cache.maskedParagraphPropertiesCache.addItemIfNonExists(oldStyle.maskedParagraphProperties);
            newStyle.linkedStyle = this.addCharacterStyle(oldStyle.linkedStyle);
            newStyle.parent = this.addParagraphStyle(oldStyle.parent);
            return newStyle;
        }
        private getDefaultStyleCore<T extends StyleBase>(styles: T[], updateCache: (style: T) => void) {
            for(var i = 0, style: T; style = styles[i]; i++) {
                if(style.isDefault) {
                    updateCache(style);
                    return style;
                }
            }
        }
        private getStyleByNameCore(styleName: string, styles: StyleBase[], cache: { [name: string]: number }): StyleBase {
            var styleIndex: number = cache[styleName];
            if(styleIndex === undefined) {
                for(var i = 0, style: StyleBase; style = styles[i]; i++) {
                    if(cache[style.styleName] === undefined)
                        cache[style.styleName] = i;
                    if(style.styleName === styleName)
                        return style;
                }
                return null;
            }
            else
                return styles[styleIndex];
        }
    }
}