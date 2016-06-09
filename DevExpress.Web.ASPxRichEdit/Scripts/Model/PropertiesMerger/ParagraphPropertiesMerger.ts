module __aspxRichEdit {
    export class ParagraphPropertiesMerger extends PropertiesMergerBase<ParagraphPropertiesMask> {

        constructor() {
            super(new MaskedParagraphProperties());
        }

        public mergeMaskedParagraphProperties(maskedParagraphProperties: MaskedParagraphProperties) {
            this.merge(maskedParagraphProperties);
        }

        public mergeParagraphStyle(paragraphStyle: ParagraphStyle) {
            var currentParagraphStyle: ParagraphStyle = paragraphStyle;
            while (currentParagraphStyle) {
                this.merge(currentParagraphStyle.maskedParagraphProperties);
                currentParagraphStyle = currentParagraphStyle.parent;
            }
        }

        public getMergedProperties(): ParagraphProperties {
            delete this.innerProperties["useValue"]; // to not to be confuse call side
            return <ParagraphProperties>(<MaskedParagraphProperties>this.innerProperties);
        }

        private merge(properties: MaskedParagraphProperties) {
            if (!properties)
                return;
            var innerProperty = <MaskedParagraphProperties>this.innerProperties; // TODO: UseBorders
            this.mergeInternal(properties, ParagraphPropertiesMask.UseAlignment, () => innerProperty.alignment = properties.alignment);
            this.mergeInternal(properties, ParagraphPropertiesMask.UseBackColor, () => innerProperty.backColor = properties.backColor);
            this.mergeInternal(properties, ParagraphPropertiesMask.UseLeftIndent, () => innerProperty.leftIndent = properties.leftIndent);
            this.mergeInternal(properties, ParagraphPropertiesMask.UseRightIndent, () => innerProperty.rightIndent = properties.rightIndent);
            this.mergeInternal(properties, ParagraphPropertiesMask.UseTopBorder, () => innerProperty.topBorder.copyFrom(properties.topBorder));
            this.mergeInternal(properties, ParagraphPropertiesMask.UseKeepWithNext, () => innerProperty.keepWithNext = properties.keepWithNext);
            this.mergeInternal(properties, ParagraphPropertiesMask.UseOutlineLevel, () => innerProperty.outlineLevel = properties.outlineLevel);
            this.mergeInternal(properties, ParagraphPropertiesMask.UseSpacingAfter, () => innerProperty.spacingAfter = properties.spacingAfter);
            this.mergeInternal(properties, ParagraphPropertiesMask.UseLeftBorder, () => innerProperty.leftBorder.copyFrom(properties.leftBorder));
            this.mergeInternal(properties, ParagraphPropertiesMask.UseSpacingBefore, () => innerProperty.spacingBefore = properties.spacingBefore);
            this.mergeInternal(properties, ParagraphPropertiesMask.UseRightBorder, () => innerProperty.rightBorder.copyFrom(properties.rightBorder));
            this.mergeInternal(properties, ParagraphPropertiesMask.UseBottomBorder, () => innerProperty.bottomBorder.copyFrom(properties.bottomBorder));
            this.mergeInternal(properties, ParagraphPropertiesMask.UsePageBreakBefore, () => innerProperty.pageBreakBefore = properties.pageBreakBefore);
            this.mergeInternal(properties, ParagraphPropertiesMask.UseAfterAutoSpacing, () => innerProperty.afterAutoSpacing = properties.afterAutoSpacing);
            this.mergeInternal(properties, ParagraphPropertiesMask.UseKeepLinesTogether, () => innerProperty.keepLinesTogether = properties.keepLinesTogether);
            this.mergeInternal(properties, ParagraphPropertiesMask.UseBeforeAutoSpacing, () => innerProperty.beforeAutoSpacing = properties.beforeAutoSpacing);
            this.mergeInternal(properties, ParagraphPropertiesMask.UseContextualSpacing, () => innerProperty.contextualSpacing = properties.contextualSpacing);
            this.mergeInternal(properties, ParagraphPropertiesMask.UseWidowOrphanControl, () => innerProperty.widowOrphanControl = properties.widowOrphanControl);
            this.mergeInternal(properties, ParagraphPropertiesMask.UseSuppressHyphenation, () => innerProperty.suppressHyphenation = properties.suppressHyphenation);
            this.mergeInternal(properties, ParagraphPropertiesMask.UseSuppressLineNumbers, () => innerProperty.suppressLineNumbers = properties.suppressLineNumbers);
            this.mergeInternal(properties, ParagraphPropertiesMask.UseFirstLineIndent, () => {
                innerProperty.firstLineIndent = properties.firstLineIndent;
                innerProperty.firstLineIndentType = properties.firstLineIndentType;
            });
            this.mergeInternal(properties, ParagraphPropertiesMask.UseLineSpacing, () => {
                innerProperty.lineSpacing = properties.lineSpacing;
                innerProperty.lineSpacingType = properties.lineSpacingType;
            });
        }

        

    }
} 