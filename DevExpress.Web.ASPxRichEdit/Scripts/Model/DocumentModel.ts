module __aspxRichEdit {
    export class DocumentModel {
        cache: DocumentCache;
        public mainSubDocument: SubDocument;
        public activeSubDocument: SubDocument;

        public specChars: SpecialCharacters = new SpecialCharacters();

        public defaultTabWidth: number; // this DefaultTabStop
        public differentOddAndEvenPages: boolean;
        public displayBackgroundShape: boolean;
        public pageBackColor: number;
        public sections: Section[] = [];
        public showHiddenSymbols: boolean;
        public showTableGridLines: boolean;
        public headers: HeaderSubDocumentInfo[] = [];
        public footers: FooterSubDocumentInfo[] = [];

        public characterStyles: CharacterStyle[] = [];
        public paragraphStyles: ParagraphStyle[] = [];
        public numberingListStyles: NumberingListStyle[] = [];
        public tableStyles: TableStyle[] = [];
        public tableCellStyles: TableCellStyle[] = [];
        
        subDocuments: { [id: number]: SubDocument } = {};

        stylesManager: StylesManager;

        defaultCharacterProperties: MaskedCharacterProperties;
        defaultParagraphProperties: MaskedParagraphProperties;

        defaultTableProperties: TableProperties;
        defaultTableRowProperties: TableRowProperties;
        defaultTableCellProperties: TableCellProperties;

        abstractNumberingListTemplates: AbstractNumberingList[] = [];

        // Lists
        abstractNumberingLists: AbstractNumberingList[] = [];
        numberingLists: NumberingList[] = [];
        abstractNumberingListsIdProvider: AbstractNumberingListIdProvider = new AbstractNumberingListIdProvider(this);
        numberingListsIdProvider: NumberingListIdProvider = new NumberingListIdProvider(this);

        options: ControlOptions;

        repositoryBorderItem: BorderInfo = new BorderInfo();

        private subDocumentsIdCounter = -1;

        private loaded: boolean = false;

        constructor(options: ControlOptions, subDocumentsIdCounter: number) {
            this.cache = new DocumentCache();
            this.mainSubDocument = this.createSubDocument(SubDocumentInfoType.Main, new MainSubDocumentInfo(0));
            this.activeSubDocument = this.mainSubDocument;
            this.stylesManager = new StylesManager(this);
            this.options = options;
            this.subDocumentsIdCounter = subDocumentsIdCounter;
            this.showHiddenSymbols = false;
            this.showTableGridLines = false;
            this.initRepositoryBorderItem();
        }

        private initRepositoryBorderItem() {
            this.repositoryBorderItem.color = ColorHelper.BLACK_COLOR;
            this.repositoryBorderItem.style = BorderLineStyle.Single;
            this.repositoryBorderItem.width = UnitConverter.pixelsToTwipsF(1);
        }

        public getCharacterStyleByName(name: string): CharacterStyle {
            return this.stylesManager.getCharacterStyleByName(name);
        }
        public getParagraphStyleByName(name: string): ParagraphStyle {
            return this.stylesManager.getParagraphStyleByName(name);
        }
        public getNumberingListStyleByName(name: string): NumberingListStyle {
            return this.stylesManager.getNumberingListStyleByName(name);
        }
        public getTableStyleByName(name: string): TableStyle {
            return this.stylesManager.getTableStyleByName(name);
        }
        public getTableCellStyleByName(name: string): TableCellStyle {
            return this.stylesManager.getTableCellStyleByName(name);
        }
        public getDefaultCharacterStyle(): CharacterStyle {
            return this.stylesManager.getDefaultCharacterStyle();
        }
        public getDefaultParagraphStyle(): ParagraphStyle {
            return this.stylesManager.getDefaultParagraphStyle();
        }

        public setDefaultCharacterProperties(obj: any) {
            this.defaultCharacterProperties = JSONMaskedCharacterPropertiesConverter.convertFromJSON(obj);
            this.cache.maskedCharacterPropertiesCache.mergeItemCore(this.defaultCharacterProperties, obj);
            this.cache.maskedCharacterPropertiesCache.addItemIfNonExists(this.defaultCharacterProperties);
        }
        public setDefaultParagraphProperties(obj: any) {
            this.defaultParagraphProperties = JSONMaskedParagraphPropertiesConverter.convertFromJSON(obj);
            this.cache.maskedParagraphPropertiesCache.mergeItemCore(this.defaultParagraphProperties, obj);
            this.cache.maskedParagraphPropertiesCache.addItemIfNonExists(this.defaultParagraphProperties);
        }

        public getSectionsByInterval(interval: FixedInterval): Section[] {
            var result = [],
                section: Section;
            var endPosition = interval.end();
            var sectionIndex = Utils.normedBinaryIndexOf(this.sections, (s: Section) => s.startLogPosition.value - interval.start);
            for(; section = this.sections[sectionIndex]; sectionIndex++) {
                if(section.startLogPosition.value > endPosition)
                    break;
                result.push(section);
            }
            return result;
        }
        public getSectionIndicesByIntervals(intervals: FixedInterval[]): number[] {
            let result: number[] = [];
            for(var i = 0, interval: FixedInterval; interval = intervals[i]; i++) {
                let sectionIndex: number = Utils.normedBinaryIndexOf(this.sections, (s: Section) => s.startLogPosition.value - interval.start);
                let intervalEnd: number = interval.end();
                result.push(sectionIndex++);
                for(let section: Section; section = this.sections[sectionIndex]; sectionIndex++) {
                    if(section.startLogPosition.value < intervalEnd)
                        result.push(sectionIndex);
                    else
                        break;
                }
            }
            return Utils.sortAndDisctinctNumbers(result);
        }
        public getSectionByPosition(position: number): Section {
            var sectionIndex = Utils.normedBinaryIndexOf(this.sections, (s: Section) => s.startLogPosition.value - position);
            return this.sections[sectionIndex];
        }
        public getCurrentLength(): number {
            var lastChunk = this.mainSubDocument.getLastChunk();
            var lastRun = lastChunk.textRuns[lastChunk.textRuns.length - 1];
            return lastChunk.startLogPosition.value + lastRun.startOffset + lastRun.length;
        }
        public isLoaded(): boolean {
            if(this.loaded)
                return true;
            this.loaded = this.mainSubDocument.getLastChunk().isLast;
            return this.loaded;
        }

        getNumberingListIndexById(id: number): number {
            for(var i = 0, numberingList: NumberingList; numberingList = this.numberingLists[i]; i++) {
                if(numberingList.getId() === id)
                    return i;
            }
            return -1;
        }
        getAbstractNumberingListIndexById(id: number): number {
            for(var i = 0, abstractNumberingList: AbstractNumberingList; abstractNumberingList = this.abstractNumberingLists[i]; i++) {
                if(abstractNumberingList.getId() === id)
                    return i;
            }
            return -1;
        }
        getRangeListCounters(paragraph: Paragraph): number[] {
            var calculator = new NumberingListCountersCalculator(paragraph.getAbstractNumberingList());
            return calculator.calculateCounters(paragraph);
        }
        resetMergedFormattingCache(type: ResetFormattingCacheType) {
            this.mainSubDocument.resetMergedFormattingCache(type);
        }
        getPreviousSection(section: Section): Section {
            var sectionIndex = Utils.normedBinaryIndexOf(this.sections, s => s.startLogPosition.value - section.startLogPosition.value);
            return this.sections[sectionIndex - 1];
        }
        getNextSection(section: Section): Section {
            var sectionIndex = Utils.normedBinaryIndexOf(this.sections, s => s.startLogPosition.value - section.startLogPosition.value);
            return this.sections[sectionIndex + 1];
        }
        createSubDocument(subDocumentInfoType: SubDocumentInfoType, info?: SubDocumentInfoBase): SubDocument {
            const isRuntimeCreated = !info;
            const id = isRuntimeCreated ? this.subDocumentsIdCounter++ : info.subDocumentId;
            if(isRuntimeCreated)
                info = SubDocumentInfoBase.create(subDocumentInfoType, id);
            if(this.subDocuments[id])
                throw new Error("SubDocument with this ID already exists");
            if(info.getType() !== subDocumentInfoType)
                throw new Error("SubDocument.type doesn't equal to info.type");
            let subDocument = new SubDocument(this, info);
            if(isRuntimeCreated)
                this.initNewSubDocument(subDocument);
            this.subDocuments[id] = subDocument;
            return subDocument;
        }

        private initNewSubDocument(subDocument: SubDocument) {
            subDocument.chunks = [new Chunk(subDocument.positionManager.registerPosition(0), "", true)];
            subDocument.paragraphs.push(new Paragraph(subDocument, subDocument.positionManager.registerPosition(0), 1, this.getDefaultParagraphStyle(), this.defaultParagraphProperties));
            subDocument.chunks[0].textRuns.push(TextRun.create(0, 1, TextRunType.ParagraphRun, subDocument.paragraphs[0], this.getDefaultCharacterStyle(), this.defaultCharacterProperties));
            subDocument.chunks[0].textBuffer = Utils.specialCharacters.ParagraphMark;
        }

        public getSubDocumentsList(): SubDocument[] {
            var subDocumentsList: SubDocument[] = [];
            const subDocuments: { [id: number]: SubDocument } = this.subDocuments;
            for (let subDocumentId in subDocuments) {
                if (!subDocuments.hasOwnProperty(subDocumentId))
                    continue;
                subDocumentsList.push(subDocuments[subDocumentId]);
            }
            return subDocumentsList;
        }
    }

    export enum ResetFormattingCacheType {
        Character = 1,
        Paragraph = 2,
        All = 0x7FFFFFFF
    }
}