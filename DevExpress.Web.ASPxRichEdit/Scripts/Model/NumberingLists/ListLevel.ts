module __aspxRichEdit {
    export interface IListLevel extends ICharacterPropertiesContainer, IParagraphPropertiesContainer, IListLevelPropertiesContainer, IEquatable<IListLevel>, ISupportCopyFrom<IListLevel> {
        documentModel: DocumentModel;
        getCharacterProperties(): MaskedCharacterProperties;
        getParagraphProperties(): MaskedParagraphProperties;
        externallyEquals(obj: IListLevel): boolean;
    }

    export interface IOverrideListLevel extends IListLevel {
        overrideStart: boolean;
        getNewStart(): number;
        setNewStart(newStart: number);
    }

    export class ListLevel implements IListLevel {
        documentModel: DocumentModel;

        constructor(documentModel: DocumentModel, maskedCharacterProperties: MaskedCharacterProperties, maskedParagraphProperties: MaskedParagraphProperties, listLevelProperties: ListLevelProperties) {
            this.documentModel = documentModel;
            this.setCharacterProperties(maskedCharacterProperties);
            this.setParagraphProperties(maskedParagraphProperties);
            this.setListLevelProperties(listLevelProperties);
        }

        private mergedCharacterProperties: CharacterProperties;
        private maskedCharacterProperties: MaskedCharacterProperties;
        private mergedParagraphProperties: ParagraphProperties;
        private maskedParagraphProperties: MaskedParagraphProperties;
        private listLevelProperties: ListLevelProperties;

        //#region IListLevel
        getListLevelProperties(): ListLevelProperties {
            return this.listLevelProperties;
        }
        setListLevelProperties(properties: ListLevelProperties) {
            this.listLevelProperties = this.documentModel.cache.listLevelPropertiesCache.addItemIfNonExists(properties);
        }
        getCharacterProperties(): MaskedCharacterProperties {
            return this.maskedCharacterProperties;
        }
        getParagraphProperties(): MaskedParagraphProperties {
            return this.maskedParagraphProperties;
        }
        //#endregion

        //#region IParagraphPropertiesContainer
        setParagraphProperties(properties: MaskedParagraphProperties) {
            this.maskedParagraphProperties = this.documentModel.cache.maskedParagraphPropertiesCache.addItemIfNonExists(properties);
        }
        onParagraphPropertiesChanged() {
            this.resetParagraphMergedProperties();
        }
        resetParagraphMergedProperties() {
            this.mergedParagraphProperties = null;
        }
        getParagraphMergedProperies(): ParagraphProperties {
            if(!this.hasParagraphMergedProperies()) {
                var merger = new ParagraphPropertiesMerger();
                merger.mergeMaskedParagraphProperties(this.maskedParagraphProperties);
                merger.mergeMaskedParagraphProperties(this.documentModel.defaultParagraphProperties);
                this.mergedParagraphProperties = merger.getMergedProperties();
            }
            return this.mergedParagraphProperties;
        }
        setParagraphMergedProperies(properties: ParagraphProperties) {
            this.mergedParagraphProperties = this.documentModel.cache.mergedParagraphPropertiesCache.addItemIfNonExists(properties);
        }
        setParagraphMergedProperiesByIndexInCache(index: number) {
            this.mergedParagraphProperties = this.documentModel.cache.mergedParagraphPropertiesCache.getItem(index);
        }
        hasParagraphMergedProperies(): boolean {
            return !!this.mergedParagraphProperties;
        }
        //#endregion

        //#region ICharacterPropertiesContainer
        setCharacterProperties(properties: MaskedCharacterProperties) {
            this.maskedCharacterProperties = this.documentModel.cache.maskedCharacterPropertiesCache.addItemIfNonExists(properties);
        }
        onCharacterPropertiesChanged() {
            this.resetCharacterMergedProperties();
        }
        resetCharacterMergedProperties() {
            this.mergedCharacterProperties = null;
        }
        getCharacterMergedProperies(): CharacterProperties {
            if(!this.hasCharacterMergedProperies()) {
                var merger = new CharacterPropertiesMerger();
                merger.mergeCharacterProperties(this.maskedCharacterProperties);
                merger.mergeCharacterProperties(this.documentModel.defaultCharacterProperties);
                this.mergedCharacterProperties = merger.getMergedProperties();
            }
            return this.mergedCharacterProperties;
        }
        setCharacterMergedProperies(properties: CharacterProperties) {
            this.mergedCharacterProperties = this.documentModel.cache.mergedCharacterPropertiesCache.addItemIfNonExists(properties);
        }
        setCharacterMergedProperiesByIndexInCache(index: number) {
            this.mergedCharacterProperties = this.documentModel.cache.mergedCharacterPropertiesCache.getItem(index);
        }
        hasCharacterMergedProperies(): boolean {
            return !!this.mergedCharacterProperties;
        }
        //#endregion

        equals(obj: ListLevel): boolean {
            if(obj === this)
                return true;
            if(!(<ListLevel>obj).getCharacterProperties().equals(this.getCharacterProperties()))
                return false;
            if(!(<ListLevel>obj).getParagraphProperties().equals(this.getParagraphProperties()))
                return false;
            if(!(<ListLevel>obj).getListLevelProperties().equals(this.getListLevelProperties()))
                return false;
            return true;
        }
        externallyEquals(obj: ListLevel): boolean {
            if(obj === this)
                return true;
            var result: boolean = obj.getListLevelProperties().displayFormatString == this.getListLevelProperties().displayFormatString &&
                obj.getListLevelProperties().format == this.getListLevelProperties().format &&
                obj.getListLevelProperties().start == this.getListLevelProperties().start &&

                obj.getCharacterProperties().fontBold == this.getCharacterProperties().fontBold &&
                obj.getCharacterProperties().fontItalic == this.getCharacterProperties().fontItalic &&
                obj.getCharacterProperties().fontSize == this.getCharacterProperties().fontSize &&
                obj.getCharacterProperties().foreColor == this.getCharacterProperties().foreColor &&
                obj.getCharacterProperties().fontInfo.equals(this.getCharacterProperties().fontInfo);
            return result;
        }
        copyFrom(obj: IListLevel) {
            this.setListLevelProperties(obj.getListLevelProperties());
            this.setCharacterProperties(obj.getCharacterProperties());
            this.setParagraphProperties(obj.getParagraphProperties());
            this.onCharacterPropertiesChanged();
            this.onParagraphPropertiesChanged();
        }
    }

    export class NumberingListReferenceLevel implements IOverrideListLevel {
        private owner: NumberingList;
        level: number;

        documentModel: DocumentModel;

        constructor(owner: NumberingList, level: number) {
            this.owner = owner;
            this.level = level;
            this.documentModel = owner.documentModel;
        }

        //#region IListLevel
        getListLevelProperties(): ListLevelProperties {
            return this.getOwnerLevel().getListLevelProperties();
        }
        setListLevelProperties(properties: ListLevelProperties) {
            this.getOwnerLevel().setListLevelProperties(properties);
        }
        getCharacterProperties(): MaskedCharacterProperties {
            return this.getOwnerLevel().getCharacterProperties();
        }
        getParagraphProperties(): MaskedParagraphProperties {
            return this.getOwnerLevel().getParagraphProperties();
        }
        //#endregion

        //#region IParagraphPropertiesContainer
        setParagraphProperties(properties: MaskedParagraphProperties) {
            this.getOwnerLevel().setParagraphProperties(properties);
        }
        onParagraphPropertiesChanged() {
            this.getOwnerLevel().onParagraphPropertiesChanged();
        }
        getParagraphMergedProperies(): ParagraphProperties {
            return this.getOwnerLevel().getParagraphMergedProperies();
        }
        setParagraphMergedProperies(properties: ParagraphProperties) {
            this.getOwnerLevel().setParagraphMergedProperies(properties);
        }
        setParagraphMergedProperiesByIndexInCache(index: number) {
            this.getOwnerLevel().setParagraphMergedProperiesByIndexInCache(index);
        }
        hasParagraphMergedProperies(): boolean {
            return this.getOwnerLevel().hasParagraphMergedProperies();
        }
        resetParagraphMergedProperties() {
            this.getOwnerLevel().resetParagraphMergedProperties();
        }
        //#endregion

        //#region ICharacterPropertiesContainer
        setCharacterProperties(properties: MaskedCharacterProperties) {
            this.getOwnerLevel().setCharacterProperties(properties);
        }
        onCharacterPropertiesChanged() {
            this.getOwnerLevel().onCharacterPropertiesChanged();
        }
        getCharacterMergedProperies(): CharacterProperties {
            return this.getOwnerLevel().getCharacterMergedProperies();
        }
        setCharacterMergedProperies(properties: CharacterProperties) {
            this.getOwnerLevel().setCharacterMergedProperies(properties);
        }
        setCharacterMergedProperiesByIndexInCache(index: number) {
            this.getOwnerLevel().setCharacterMergedProperiesByIndexInCache(index);
        }
        hasCharacterMergedProperies(): boolean {
            return this.getOwnerLevel().hasCharacterMergedProperies();
        }
        resetCharacterMergedProperties() {
            this.getOwnerLevel().resetCharacterMergedProperties();
        }
        //#endregion

        getNewStart(): number {
            return this.newStart;
        }
        setNewStart(newStart: number) {
            this.newStart = newStart;
        }

        overrideStart: boolean = false;
        private newStart: number = 1;

        private getOwnerLevel(): ListLevel {
            return <ListLevel>this.owner.getAbstractNumberingList().levels[this.level];
        }

        equals(obj: NumberingListReferenceLevel): boolean {
            return this.getOwnerLevel().equals(obj.getOwnerLevel());
        }
        externallyEquals(obj: NumberingListReferenceLevel): boolean {
            return this.getOwnerLevel().externallyEquals(obj.getOwnerLevel());
        }

        copyFrom(obj: IListLevel) {
            if(!(obj instanceof NumberingListReferenceLevel))
                throw new Error("Source level should have equal type");
            this.newStart = (<NumberingListReferenceLevel>obj).newStart;
            this.overrideStart = (<NumberingListReferenceLevel>obj).overrideStart;
        }
    }

    export class OverrideListLevel extends ListLevel implements IOverrideListLevel {
        overrideStart: boolean = false;
        getNewStart(): number {
            return this.getListLevelProperties().start;
        }
        setNewStart(newStart: number) {
            var properties = this.getListLevelProperties().clone();
            properties.start = newStart;
            this.setListLevelProperties(properties);
        }
        copyFrom(obj: IListLevel) {
            if(!(obj instanceof OverrideListLevel))
                throw new Error("Source level should have equal type");
            super.copyFrom(obj);
            this.overrideStart = (<OverrideListLevel>obj).overrideStart;
        }
    }
}