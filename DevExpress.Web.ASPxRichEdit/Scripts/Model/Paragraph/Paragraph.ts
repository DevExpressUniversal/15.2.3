module __aspxRichEdit {
    export class TabsInfo {
        public positions: TabPosition[];
        public defaultTabStop: number;
    }

    export enum TabAlign {
        Left,
        Center,
        Right,
        Decimal,
    }

    export class TabPosition {
        constructor(tabInfo: TabInfo = null) {
            if (tabInfo) {
                this.offset = tabInfo.position;
                this.align = tabInfo.alignment;
            }
        }

        public offset: number;
        public align: TabAlign;
    }

    export class Paragraph implements IParagraphPropertiesContainer {
        public subDocument: SubDocument;
        public startLogPosition: Position;
        public length: number;
        public paragraphStyle: ParagraphStyle;
        private mergedParagraphFormatting: ParagraphProperties;
        public maskedParagraphProperties: MaskedParagraphProperties;
        public tabs: TabProperties;
        public numberingListIndex: number = NumberingList.NumberingListNotSettedIndex;
        public listLevelIndex: number = -1;
        public layoutWidthBounds: WidthBounds; // for tables. Cached max-min layout width; When paragraphChanged - need reset this parameter.
        // Reset on every change what can change text width

        //infoParagraphFormatting can be MaskedParagraphProperties or number
        constructor(subDocument: SubDocument, startLogPosition: Position, length: number, paragraphStyle: ParagraphStyle, maskedParagraphProperties: MaskedParagraphProperties, indexInMaskedParagraphProperitesCache: number = undefined) {
            this.subDocument = subDocument;
            this.startLogPosition = startLogPosition;
            this.length = length;
            this.paragraphStyle = paragraphStyle;
            if (indexInMaskedParagraphProperitesCache === undefined) {
                if (maskedParagraphProperties)
                    this.setParagraphProperties(maskedParagraphProperties);
                else
                    this.maskedParagraphProperties = null;
            }
            else
                this.maskedParagraphProperties = this.subDocument.documentModel.cache.maskedParagraphPropertiesCache.getItem(indexInMaskedParagraphProperitesCache);
            this.mergedParagraphFormatting = null;
            this.tabs = new TabProperties();
        }

        isInList(): boolean {
            return this.getNumberingListIndex() >= 0;
        }
        isInStyleList(): boolean {
            return this.paragraphStyle && this.paragraphStyle.numberingListIndex >= 0;
        }
        getListLevelIndex(): number {
            if(this.listLevelIndex >= 0)
                return this.listLevelIndex;
            return this.paragraphStyle ? this.paragraphStyle.listLevelIndex : -1;
        }
        getListLevel(): IOverrideListLevel {
            return this.getNumberingList().levels[this.getListLevelIndex()];
        }
        getNumberingListIndex(): number {
            if(this.numberingListIndex >= 0 || this.numberingListIndex === NumberingList.NoNumberingListIndex)
                return this.numberingListIndex;
            return this.paragraphStyle ? this.paragraphStyle.getNumberingListIndex() : -1;
        }
        getNumberingList(): NumberingList {
            return this.subDocument.documentModel.numberingLists[this.getNumberingListIndex()];
        }
        getAbstractNumberingList(): AbstractNumberingList {
            var numberingList = this.getNumberingList();
            return numberingList ? numberingList.getAbstractNumberingList() : null;
        }
        getAbstractNumberingListIndex(): number {
            var numberingList = this.getNumberingList();
            return numberingList ? numberingList.abstractNumberingListIndex : -1;
        }
        getNumberingListText(): string {
            var counters = this.subDocument.documentModel.getRangeListCounters(this);
            return this.getNumberingListTextCore(counters);
        }
        getNumberingListTextCore(counters: number[]): string {
            var levels = this.getNumberingList().levels;
            var formatString = levels[this.getListLevelIndex()].getListLevelProperties().displayFormatString;
            return this.formatNumberingListText(formatString, counters, levels);
        }
        getNumberingListSeparatorChar(): string {
            var levels = this.getNumberingList().levels;
            return levels[this.getListLevelIndex()].getListLevelProperties().separator;
        }
        getNumerationCharacterProperties(): CharacterProperties {
            var merger = new CharacterPropertiesMerger();
            merger.mergeCharacterProperties(this.getNumberingList().levels[this.getListLevelIndex()].getCharacterProperties());
            merger.mergeMergedCharacterProperties(this.subDocument.getRunByPosition(this.getEndPosition() - 1).getCharacterMergedProperies());
            return merger.getMergedProperties();
        }
        private formatNumberingListText(formatString: string, args: number[], levels: IListLevel[]): string {
            var objArgs: Object[] = new Array<Object>(args.length);
            for(var i = 0; i < args.length; i++) {
                var converter = OrdinalBasedNumberConverter.createConverter(levels[i].getListLevelProperties().format);
                objArgs[i] = converter.convertNumber(args[i]);
            }
            try {
                return ASPx.Formatter.Format(formatString, objArgs);
            }
            catch(e) {
                try {
                    return <string>objArgs[0];
                }
                catch(e) {
                    return "";
                }
            }
        }

        public getInterval(): FixedInterval {
            return new FixedInterval(this.startLogPosition.value, this.length);
        }

        public getEndPosition(): number {
            return this.startLogPosition.value + this.length;
        }

        resetRunsCharacterFormatting() {
            var runs = this.subDocument.getRunsByInterval(new FixedInterval(this.startLogPosition.value, this.length));
            for(var i = 0, run: TextRun; run = runs[i]; i++) {
                run.maskedCharacterProperties.useValue = 0;
                run.onCharacterPropertiesChanged();
            }
        }

        //#region IParagraphPropertiesContainer
        setParagraphProperties(properties: MaskedParagraphProperties) {
            this.maskedParagraphProperties = this.subDocument.documentModel.cache.maskedParagraphPropertiesCache.addItemIfNonExists(properties);
        }
        onParagraphPropertiesChanged() {
            this.resetParagraphMergedProperties();
        }
        resetParagraphMergedProperties() {
            this.mergedParagraphFormatting = null;
        }
        getParagraphMergedProperies(): ParagraphProperties {
            if(!this.mergedParagraphFormatting) {
                var merger: ParagraphPropertiesMerger = new ParagraphPropertiesMerger();
                merger.mergeMaskedParagraphProperties(this.maskedParagraphProperties);
                if(this.isInList())
                    merger.mergeMaskedParagraphProperties(this.getListLevel().getParagraphProperties());
                merger.mergeParagraphStyle(this.paragraphStyle);
                merger.mergeMaskedParagraphProperties(this.subDocument.documentModel.defaultParagraphProperties);
                this.mergedParagraphFormatting = merger.getMergedProperties();
            }
            return this.mergedParagraphFormatting;
        }
        setParagraphMergedProperiesByIndexInCache(index: number) {
            this.mergedParagraphFormatting = this.subDocument.documentModel.cache.mergedParagraphPropertiesCache.getItem(index);
        }
        setParagraphMergedProperies(properties: ParagraphProperties) {
            this.mergedParagraphFormatting = this.subDocument.documentModel.cache.mergedParagraphPropertiesCache.addItemIfNonExists(properties);
        }
        hasParagraphMergedProperies(): boolean {
            return !!this.mergedParagraphFormatting;
        }
        //#endregion

        getTabs(): TabsInfo {
            var result: TabsInfo = new TabsInfo();
            result.defaultTabStop = this.subDocument.documentModel.defaultTabWidth;
            result.positions = [];

            if (this.paragraphStyle) {
                var styleTabs: TabInfo[] = this.paragraphStyle.tabs.tabsInfo;
                for (var i = 0; i < styleTabs.length; i++)
                    if (!styleTabs[i].deleted) {
                        var tabPosition: TabPosition = new TabPosition(styleTabs[i]);
                        var index = Utils.binaryIndexOf(result.positions, (t: TabPosition) => t.offset - styleTabs[i].position);
                        if (index < 0)
                            result.positions.splice(~index, 0, tabPosition);
                        else
                            result.positions[index] = tabPosition;
                    }
            }
            var paragraphTabs: TabInfo[] = this.tabs.tabsInfo;
            for (var i = 0; i < paragraphTabs.length; i++) {
                var index = Utils.binaryIndexOf(result.positions, (t: TabPosition) => t.offset - paragraphTabs[i].position);
                if (index < 0) {
                    index = ~index;
                    if (!paragraphTabs[i].deleted)
                        result.positions.splice(index, 0, new TabPosition(paragraphTabs[i]));
                }
                else {
                    if (paragraphTabs[i].deleted)
                        result.positions.splice(index, 1);
                    else
                        result.positions[index] = new TabPosition(paragraphTabs[i]);
                }
            }
            return result;
        }
    }
} 