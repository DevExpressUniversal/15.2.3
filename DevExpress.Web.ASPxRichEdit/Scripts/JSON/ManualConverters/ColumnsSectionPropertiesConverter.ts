module __aspxRichEdit {
    export class JSONColumnsSectionPropertiesConverter {
        static convertFromJSON(obj: any): SectionColumnProperties[] {
            var result: SectionColumnProperties[] = [];
            for (var i = 0; i < obj.length; i++)
                result.push(new SectionColumnProperties(obj[i].width, obj[i].space));
            return result;
        }
        static convertToJSON(obj: SectionColumnProperties[]): any {
            var result = [];
            for (var i = 0; i < obj.length; i++)
                result.push({ width: obj[i].width, space: obj[i].space });
            return result;
        }
    }
}