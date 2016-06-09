module __aspxRichEdit {
    export enum JSONTabInfoProperty {
        Alignment = 0,
        LeaderType = 1,
        Position = 2,
        Deleted = 3,
        IsDefault = 4
    }

    export class JSONTabConverter {
        static convertFromJSON(obj: any): TabInfo {
            return new TabInfo(obj[JSONTabInfoProperty.Position],
                obj[JSONTabInfoProperty.Alignment],
                obj[JSONTabInfoProperty.LeaderType],
                obj[JSONTabInfoProperty.Deleted],
                obj[JSONTabInfoProperty.IsDefault]);
        }

        static convertToJSON(source: TabInfo): any {
            var result = {};
            result[JSONTabInfoProperty.Position] = source.position;
            result[JSONTabInfoProperty.Alignment] = source.alignment;
            result[JSONTabInfoProperty.LeaderType] = source.leader;
            result[JSONTabInfoProperty.Deleted] = source.deleted;
            result[JSONTabInfoProperty.IsDefault] = source.isDefault;
            return result;
        }
    }
} 