module __aspxRichEdit {
    export class DocumentCache {
        mergedCharacterPropertiesCache: CharacterPropertiesCache = new CharacterPropertiesCache(this);
        mergedParagraphPropertiesCache: ParagraphPropertiesCache = new ParagraphPropertiesCache();

        maskedCharacterPropertiesCache: MaskedCharacterPropertiesCache = new MaskedCharacterPropertiesCache(this);
        maskedParagraphPropertiesCache: MaskedParagraphPropertiesCache = new MaskedParagraphPropertiesCache();

        tableRowPropertiesCache: TableRowPropertiesCache = new TableRowPropertiesCache();
        tableCellPropertiesCache: TableCellPropertiesCache = new TableCellPropertiesCache();

        listLevelPropertiesCache: ListLevelPropertiesCache = new ListLevelPropertiesCache();

        fontInfoCache: IndexedCache<FontInfo> = new FontInfoCache();
    }
    export class IndexedCache<T extends IIndexedCacheType<any>> {
        nextKey: number = 1;
        innerCache: { [cacheIndex: number]: T; } = {};
        length: number = 0;

        public registerItem(cacheIndex: number, item: T) {
            var result = this.innerCache[cacheIndex];
            if(result == null) {
                this.appendItem(item, cacheIndex);
                this.processNewItem(item, cacheIndex);
            }
        }
        public addItemIfNonExists(item: T): T {
            for (var key in this.innerCache) {
                if (this.isEquals(this.innerCache[key], item))
                    return this.innerCache[key];
            }
            this.appendItem(item, -this.nextKey);
            this.nextKey++;
            return item;
        }
        private appendItem(item: T, cacheIndex: number) {
            this.innerCache[cacheIndex] = item;
            this.length++;
            this.processNewItem(item, cacheIndex);
        }
        public merge(properties: any, convertFromJSON: { (obj: any): T }) {
            for (var key in properties) {
                if (!properties.hasOwnProperty(key)) continue;
                var property = properties[key];
                var modelProperties = convertFromJSON(property);
                var intKey = parseInt(key);
                this.registerItem(intKey, modelProperties);
                this.mergeItemCore(modelProperties, property);
                this.processNewItem(modelProperties, intKey);
            }
        }
        mergeItemCore(property: T, obj: any) { }
        processNewItem(item: T, key: number) { }
        isEquals(a: T, b: T): boolean {
            if (this.isEqualsCore(a, b))
                return true;
            return a === b;
        }
        isEqualsCore(a: T, b: T) {
            if (!a)
                return false;
            return a.equals(b);
        }
        public getItem(cacheIndex: number): T {
            return this.innerCache[cacheIndex];
        }
        findItem(predicate: (item: T) => boolean): T {
            for (var key in this.innerCache) {
                if (!this.innerCache.hasOwnProperty(key)) continue;
                if (predicate(this.innerCache[key]))
                    return this.innerCache[key];
            }
            return null;
        }
    }
    export class CharacterPropertiesCache extends IndexedCache<CharacterProperties> {
        documentCache: DocumentCache;
        constructor(documentCache: DocumentCache) {
            super();
            this.documentCache = documentCache;
        }
        mergeItemCore(property: CharacterProperties, obj: any) {
            property.fontInfo = this.documentCache.fontInfoCache.getItem(obj[JSONCharacterFormattingProperty.FontInfoIndex]);
        }
    }
    export class MaskedCharacterPropertiesCache extends IndexedCache<MaskedCharacterProperties> {
        documentCache: DocumentCache;
        constructor(documentCache: DocumentCache) {
            super();
            this.documentCache = documentCache;
        }
        mergeItemCore(property: MaskedCharacterProperties, obj: any) {
            property.fontInfo = this.documentCache.fontInfoCache.getItem(obj[JSONCharacterFormattingProperty.FontInfoIndex]);
        }
    }
    export class ParagraphPropertiesCache extends IndexedCache<ParagraphProperties> {        
    }
    export class MaskedParagraphPropertiesCache extends IndexedCache<MaskedParagraphProperties> {
    }
    export class TablePropertiesCache extends IndexedCache<TableProperties> {
    }
    export class TableRowPropertiesCache extends IndexedCache<TableRowProperties> {
    }
    export class TableCellPropertiesCache extends IndexedCache<TableCellProperties> {
    }

    export class FontInfoCache extends IndexedCache<FontInfo> {
        fontMeasurer: FontMeasurer;
        constructor() {
            super();
            this.fontMeasurer = new FontMeasurer();
        }
        processNewItem(property: FontInfo, key: number) {
            property.index = key;
            property.measurer = this.fontMeasurer;
        }
    }

    export class ListLevelPropertiesCache extends IndexedCache<ListLevelProperties> {

    }
}