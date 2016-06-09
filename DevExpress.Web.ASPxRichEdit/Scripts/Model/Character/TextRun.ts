module __aspxRichEdit {
    export class HistoryRun {
        public type: TextRunType;
        public characterStyle: CharacterStyle;
        public offsetAtStartDocument: number;
        public characterProperties: MaskedCharacterProperties;
        public text: string;

        constructor(type: TextRunType, characterStyle: CharacterStyle, offsetAtStartDocument: number, characterProperties: MaskedCharacterProperties, text: string) {
            this.type = type;
            this.characterStyle = characterStyle;
            this.offsetAtStartDocument = offsetAtStartDocument;
            this.characterProperties = characterProperties;
            this.text = text;
        }
    }

    export class HistoryRunInlinePicture extends HistoryRun {
        id: number;
        originalWidth: number;
        originalHeight: number;
        scaleX: number;
        scaleY: number;
        lockAspectRatio: boolean;
        isLoaded: boolean;
        guid: string;
        constructor(characterStyle: CharacterStyle, offsetAtStartDocument: number, characterProperties: MaskedCharacterProperties, id: number,
            originalWidth: number, originalHeight: number, scaleX: number, scaleY: number, lockAspectRatio: boolean, isLoaded: boolean = true, guid: string = "") {
            super(TextRunType.InlinePictureRun, characterStyle, offsetAtStartDocument, characterProperties, Utils.specialCharacters.ObjectMark);
            this.id = id;
            this.originalWidth = originalWidth;
            this.originalHeight = originalHeight;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.lockAspectRatio = lockAspectRatio;
            this.isLoaded = isLoaded;
            this.guid = guid;
        }
    }

    export class HistoryRunFieldCodeStart extends HistoryRun {
        showCode: boolean;
        startPosition: number;
        separatorPosition: number;
        endPosition: number;
        hyperlinkInfo: HyperlinkInfo;

        constructor(type: TextRunType, characterStyle: CharacterStyle, offsetAtStartDocument: number, characterProperties: MaskedCharacterProperties, text: string,
            showCode: boolean, startPosition: number, separatorPosition: number, endPosition: number, hyperlinkInfo: HyperlinkInfo) {
            super(type, characterStyle, offsetAtStartDocument, characterProperties, text);
            this.showCode = showCode;
            this.startPosition = startPosition;
            this.separatorPosition = separatorPosition;
            this.endPosition = endPosition;
            this.hyperlinkInfo = hyperlinkInfo;
        }
    }

    export class HistoryRunFieldCodeEnd extends HistoryRun {
    }

    export class HistoryRunFieldResultEnd extends HistoryRun {
    }

    export class HistoryRunParagraph extends HistoryRun {
        public paragraphStyle: ParagraphStyle; // can be null (instanceof type)
        public paragraphMaskedProperties: MaskedParagraphProperties; // can be null
        public isInsertPropertiesAndStyleIndexToCurrentParagraph: boolean;
        public numbericListIndex: number;
        public listLevelIndex: number;
        public tabs: TabProperties;

        constructor(type: TextRunType, characterStyle: CharacterStyle, offsetAtStartDocument: number, characterProperties: MaskedCharacterProperties, text: string,
            paragraphStyle: ParagraphStyle, paragraphMaskedProperties: MaskedParagraphProperties, isInsertPropertiesAndStyleIndexToCurrentParagraph: boolean, numbericListIndex: number, listLevelIndex: number, tabs: TabProperties) {
            super(type, characterStyle, offsetAtStartDocument, characterProperties, text);
            this.paragraphStyle = paragraphStyle;
            this.paragraphMaskedProperties = paragraphMaskedProperties;
            this.isInsertPropertiesAndStyleIndexToCurrentParagraph = isInsertPropertiesAndStyleIndexToCurrentParagraph;
            this.numbericListIndex = numbericListIndex;
            this.listLevelIndex = listLevelIndex;
            this.tabs = tabs;
        }
    }

    export class HistoryRunSection extends HistoryRunParagraph {
        sectionProperties: SectionProperties; // can be null/undefined
        headers: SectionHeadersFooters<HeaderSubDocumentInfo>;
        footers: SectionHeadersFooters<FooterSubDocumentInfo>;

        constructor(run: TextRun, paragraph: Paragraph, section: Section, offsetAtStartDocument: number, isInsertPropertiesAndStyleIndexToCurrentParagraph: boolean) {
            super(TextRunType.SectionRun, run.characterStyle, offsetAtStartDocument, run.maskedCharacterProperties, Utils.specialCharacters.SectionMark, paragraph.paragraphStyle, paragraph.maskedParagraphProperties, isInsertPropertiesAndStyleIndexToCurrentParagraph, paragraph.numberingListIndex, paragraph.listLevelIndex, paragraph.tabs.clone());
            this.sectionProperties = section.sectionProperties;
            this.headers = section.headers.clone();
            this.footers = section.footers.clone();
        }
    }

    export class TextRun implements ICharacterPropertiesContainer, ICloneable<TextRun> {
        public startOffset: number;
        public length: number;
        public type: TextRunType;
        public paragraph: Paragraph;
        public characterStyle: CharacterStyle;
        public maskedCharacterProperties: MaskedCharacterProperties;
        private mergedCharacterProperties: CharacterProperties;

        constructor(startOffset: number, length: number, type: TextRunType, paragraph: Paragraph, characterStyle: CharacterStyle, maskedCharacterProperties: MaskedCharacterProperties) {
            this.startOffset = startOffset;
            this.length = length;
            this.type = type;
            this.paragraph = paragraph;
            this.characterStyle = characterStyle;
            this.setCharacterProperties(maskedCharacterProperties);
            this.mergedCharacterProperties = null;
        }

        public getHistoryItem(chunkOffsetAtStartDocument: number, characterProperties: MaskedCharacterProperties, text: string): HistoryRun {
            return new HistoryRun(this.type, this.characterStyle, chunkOffsetAtStartDocument + this.startOffset, characterProperties, text);
        }

        setCharacterProperties(properties: MaskedCharacterProperties) {
            this.maskedCharacterProperties = this.paragraph.subDocument.documentModel.cache.maskedCharacterPropertiesCache.addItemIfNonExists(properties);
        }
        onCharacterPropertiesChanged() {
            this.resetCharacterMergedProperties();
        }
        getCharacterMergedProperies(): CharacterProperties {
            if(!this.mergedCharacterProperties) {
                var merger: CharacterPropertiesMerger = new CharacterPropertiesMerger();
                merger.mergeCharacterProperties(this.maskedCharacterProperties);
                merger.mergeCharacterStyle(this.characterStyle);
                merger.mergeParagraphStyle(this.paragraph.paragraphStyle);
                merger.mergeCharacterProperties(this.paragraph.subDocument.documentModel.defaultCharacterProperties);
                this.mergedCharacterProperties = this.paragraph.subDocument.documentModel.cache.mergedCharacterPropertiesCache.addItemIfNonExists(merger.getMergedProperties());
            }
            return this.mergedCharacterProperties;
        }
        setCharacterMergedProperies(properties: CharacterProperties) {
            this.mergedCharacterProperties = this.paragraph.subDocument.documentModel.cache.mergedCharacterPropertiesCache.addItemIfNonExists(properties);
        }
        setCharacterMergedProperiesByIndexInCache(index: number) {
            this.mergedCharacterProperties = this.paragraph.subDocument.documentModel.cache.mergedCharacterPropertiesCache.getItem(index);
        }
        hasCharacterMergedProperies(): boolean {
            return !!this.mergedCharacterProperties;
        }
        resetCharacterMergedProperties() {
            this.mergedCharacterProperties = null;
        }

        public clone(): TextRun {
            var newRun: TextRun = new TextRun(this.startOffset, this.length, this.type, this.paragraph, this.characterStyle, this.maskedCharacterProperties);
            newRun.mergedCharacterProperties = this.mergedCharacterProperties;
            return newRun;
        }

        static create(startIndex: number, length: number, type: TextRunType, paragraph: Paragraph, characterStyle: CharacterStyle, characterProperties: MaskedCharacterProperties, specificPropertiesSource?: TextRun): TextRun {
            if(specificPropertiesSource && specificPropertiesSource.type !== type)
                throw new Error("specificPropertiesSource run should have the same type");
            switch(type) {
                case TextRunType.InlinePictureRun:
                    var pictureRun: InlinePictureRun = new InlinePictureRun(startIndex, length, type, paragraph, characterStyle, characterProperties);
                    if(specificPropertiesSource) {
                        pictureRun.id = (<InlinePictureRun>specificPropertiesSource).id;
                        pictureRun.lockAspectRatio = (<InlinePictureRun>specificPropertiesSource).lockAspectRatio;
                        pictureRun.originalHeight = (<InlinePictureRun>specificPropertiesSource).originalHeight;
                        pictureRun.originalWidth = (<InlinePictureRun>specificPropertiesSource).originalWidth;
                        pictureRun.scaleX = (<InlinePictureRun>specificPropertiesSource).scaleX;
                        pictureRun.scaleY = (<InlinePictureRun>specificPropertiesSource).scaleY;
                    }
                    return pictureRun;
                default:
                    return new TextRun(startIndex, length, type, paragraph, characterStyle, characterProperties);
            }
        }
    }

    export class InlineObjectRun extends TextRun {
        originalWidth: number;
        originalHeight: number;
        scaleX: number;
        scaleY: number;
        isLockAspectRatio(): boolean {
            return false;
        }
        getActualWidth(): number {
            return this.originalWidth * this.scaleX / 100;
        }
        getActualHeight(): number {
            return this.originalHeight * this.scaleY / 100;
        }
    }

    export class InlinePictureRun extends InlineObjectRun {
        lockAspectRatio: boolean;
        id: number;
        isLoaded: boolean = true;

        isLockAspectRatio(): boolean {
            return this.lockAspectRatio;
        }
    }

    export enum TextRunType {
        Undefined = -1,
        TextRun = 1,
        ParagraphRun = 2,
        LineNumberCommonRun = 3,
        CustomRun = 4,
        DataContainerRun = 5,
        FieldCodeStartRun = 6,
        FieldCodeEndRun = 7,
        FieldResultEndRun = 8,
        LayoutDependentTextRun = 9,
        FootNoteRun = 10,
        EndNoteRun = 11,
        InlineCustomObjectRun = 12,
        InlinePictureRun = 13,
        SectionRun = 14,
        SeparatorTextRun = 15,
        FloatingObjectAnchorRun = 16
    }
}