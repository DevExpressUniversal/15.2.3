module __aspxRichEdit {
    export enum JSONTableConditionalStyleProperty {
        TablePropertiesIndex = 0,
        TableRowPropertiesIndex = 1,
        TableCellPropertiesIndex = 2,
        MaskedParagraphPropertiesCacheIndex = 3,
        MaskedCharacterPropertiesCacheIndex = 4,
        Tabs = 5
    }

    export class JSONTableConditionalStyleConverter {
        static convertFromJSON(obj: any, model: DocumentModel,
            tempTablePropertiesCache: TablePropertiesCache,
            tempTableRowPropertiesCache: TableRowPropertiesCache,
            tempTableCellPropertiesCache: TableCellPropertiesCache): TableConditionalStyle {

            var tabs: TabProperties = new TabProperties;
            for (var tabIndex = 0, rawTab; rawTab = obj[JSONTableConditionalStyleProperty.Tabs][tabIndex]; tabIndex++)
                tabs.tabsInfo.push(JSONTabConverter.convertFromJSON(rawTab));

            return new TableConditionalStyle(
                tempTablePropertiesCache.getItem(obj[JSONTableConditionalStyleProperty.TablePropertiesIndex]),
                model.cache.tableRowPropertiesCache.addItemIfNonExists(tempTableRowPropertiesCache.getItem(obj[JSONTableConditionalStyleProperty.TableRowPropertiesIndex])),
                model.cache.tableCellPropertiesCache.addItemIfNonExists(tempTableCellPropertiesCache.getItem(obj[JSONTableConditionalStyleProperty.TableCellPropertiesIndex])),
                model.cache.maskedParagraphPropertiesCache.getItem(obj[JSONTableConditionalStyleProperty.MaskedParagraphPropertiesCacheIndex]),
                model.cache.maskedCharacterPropertiesCache.getItem(obj[JSONTableConditionalStyleProperty.MaskedCharacterPropertiesCacheIndex]),
                tabs
            );
        }
    }
} 