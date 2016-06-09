module __aspxRichEdit {
    export enum JSONTableStyleProperty {
        Name = 0,
        Deleted = 1,
        Hidden = 2,
        Semihidden = 3,
        IsDefault = 4,
        ParentName = 5,
        BaseConditionalStyle = 6,
        ConditionalStyles = 7,
        LocalizedName = 8
    }

    export class JSONTableStyleConverter {
        static convertFromJSON(obj: any,
            model: DocumentModel,
            tempTablePropertiesCache: TablePropertiesCache,
            tempTableRowPropertiesCache: TableRowPropertiesCache,
            tempTableCellPropertiesCache: TableCellPropertiesCache
            ): TableStyle {

            var conditionalStyles: { [typeId: number]: TableConditionalStyle } = {};
            var rawConditionalStyles: any = obj[JSONTableStyleProperty.ConditionalStyles];
            for (var rawStyleType in rawConditionalStyles)
                if (rawConditionalStyles.hasOwnProperty(rawStyleType))
                    conditionalStyles[rawStyleType] =
                    JSONTableConditionalStyleConverter.convertFromJSON(rawConditionalStyles[rawStyleType], model,
                        tempTablePropertiesCache, tempTableRowPropertiesCache, tempTableCellPropertiesCache);

            return new TableStyle(
                obj[JSONTableStyleProperty.Name],
                obj[JSONTableStyleProperty.LocalizedName],
                obj[JSONTableStyleProperty.Deleted],
                obj[JSONTableStyleProperty.Hidden],
                obj[JSONTableStyleProperty.Semihidden],
                obj[JSONTableStyleProperty.IsDefault],
                JSONTableConditionalStyleConverter.convertFromJSON(obj[JSONTableStyleProperty.BaseConditionalStyle], model,
                    tempTablePropertiesCache, tempTableRowPropertiesCache, tempTableCellPropertiesCache),
                conditionalStyles);
        }
    }
} 