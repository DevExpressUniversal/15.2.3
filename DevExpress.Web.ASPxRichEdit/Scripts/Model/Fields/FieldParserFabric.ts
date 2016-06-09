module __aspxRichEdit {
    class FindFieldNameResult {
        fieldNameToParser: ParserCreatorByFieldName;
        fieldNameFirstLetterPosition: number;
    }

    class ParserCreatorByFieldName {
        name: string;
        getFieldParser: (control: IRichEditControl, subDocument: SubDocument, field: Field, modelIterator: ModelIterator,
        fieldNameFirstLetterPosition: number) => FieldCodeParser;

        constructor(name: string, getFieldParser: (control: IRichEditControl, subDocument: SubDocument, field: Field, modelIterator: ModelIterator,
            fieldNameFirstLetterPosition: number) => FieldCodeParser) {
            this.name = name;
            this.getFieldParser = getFieldParser;
        }
    }

    export class FieldParserFabric {
        public static getParser(control: IRichEditControl, subDocument: SubDocument, field: Field): FieldCodeParser {
            var modelIterator: ModelIterator = new ModelIterator(subDocument, true);
            modelIterator.setPosition(field.getCodeStartPosition());

            var findFieldNameResult: FindFieldNameResult = this.fastSearchName(modelIterator, FieldParserFabric.fieldNamesMap);
            if (findFieldNameResult)
                return findFieldNameResult.fieldNameToParser.getFieldParser(control, subDocument, field, modelIterator,
                    findFieldNameResult.fieldNameFirstLetterPosition);
            else
                return null;
        }

        private static fieldNamesMap: ParserCreatorByFieldName[] = [
            //new FieldNameToParser("CREATEDATE", (field: Field, fieldIndex: number) => new FieldCodeParser(field, fieldIndex)),
            new ParserCreatorByFieldName("DATE", (control: IRichEditControl, subDocument: SubDocument, field: Field, modelIterator: ModelIterator, fieldNameFirstLetterPosition: number) => new FieldCodeParserDate(control, subDocument, field, modelIterator, fieldNameFirstLetterPosition)),
            new ParserCreatorByFieldName("TIME", (control: IRichEditControl, subDocument: SubDocument, field: Field, modelIterator: ModelIterator, fieldNameFirstLetterPosition: number) => new FieldCodeParserTime(control, subDocument, field, modelIterator, fieldNameFirstLetterPosition)),
            new ParserCreatorByFieldName("DOCVARIABLE", (control: IRichEditControl, subDocument: SubDocument, field: Field, modelIterator: ModelIterator, fieldNameFirstLetterPosition: number) => new FieldCodeParserDocVariable(control, subDocument, field, modelIterator, fieldNameFirstLetterPosition)),
            new ParserCreatorByFieldName("HYPERLINK", (control: IRichEditControl, subDocument: SubDocument, field: Field, modelIterator: ModelIterator, fieldNameFirstLetterPosition: number) => new FieldCodeParserHyperlink(control, subDocument, field, modelIterator, fieldNameFirstLetterPosition)),
            //new FieldNameToParser("IF", new FieldCodeParser(null, 0)),
            //new FieldNameToParser("INCLUDEPICTURE", new FieldCodeParser(null, 0)),
            new ParserCreatorByFieldName("MERGEFIELD", (control: IRichEditControl, subDocument: SubDocument, field: Field, modelIterator: ModelIterator, fieldNameFirstLetterPosition: number) => new FieldCodeParserMailMerge(control, subDocument, field, modelIterator, fieldNameFirstLetterPosition)),
            new ParserCreatorByFieldName("NUMPAGES", (control: IRichEditControl, subDocument: SubDocument, field: Field, modelIterator: ModelIterator, fieldNameFirstLetterPosition: number) => new FieldCodeParserNumPages(control, subDocument, field, modelIterator, fieldNameFirstLetterPosition)),
            new ParserCreatorByFieldName("PAGE", (control: IRichEditControl, subDocument: SubDocument, field: Field, modelIterator: ModelIterator, fieldNameFirstLetterPosition: number) => new FieldCodeParserPage(control, subDocument, field, modelIterator, fieldNameFirstLetterPosition)),
            //new FieldNameToParser("SEQ", new FieldCodeParser(null, 0)),
            //new FieldNameToParser("SYMBOL", new FieldCodeParser(null, 0)),
            //new FieldNameToParser("TC", new FieldCodeParser(null, 0)),
            //new FieldNameToParser("TOC", new FieldCodeParser(null, 0)),
            //new FieldNameToParser("=", new FieldCodeParser(null, 0))
        ];

        private static fastSearchName(iterator: ModelIterator, objectList: ParserCreatorByFieldName[]): FindFieldNameResult {

            var objListBeforeIteration: ParserCreatorByFieldName[] = objectList;
            var currCharIndex: number = 0;

            while (iterator.run.type == TextRunType.TextRun && Utils.isWhitespace.test(iterator.getCurrentChar()) ||
                iterator.run.type == TextRunType.ParagraphRun ||
                iterator.run.type == TextRunType.SectionRun)
                iterator.moveToNextChar();

            var startFieldNamePos: number = iterator.getCurrectPosition();

            var result: FindFieldNameResult = new FindFieldNameResult();
            result.fieldNameFirstLetterPosition = iterator.getCurrectPosition();
            do {
                if (iterator.run.type != TextRunType.TextRun)
                    return null;
                var currIteratorChar: string = iterator.getCurrentChar().toUpperCase();
                var objListAfterIteration: ParserCreatorByFieldName[] = [];
                for (var objIndex: number = 0, obj: ParserCreatorByFieldName; obj = objListBeforeIteration[objIndex]; objIndex++) {
                    var objChar: string = obj.name[currCharIndex];
                    if (objChar && objChar == currIteratorChar)
                        objListAfterIteration.push(obj);
                }
                currCharIndex++;
                if (objListAfterIteration.length == 1) {
                    var foundName: string = objListAfterIteration[0].name;
                    for (; currCharIndex < foundName.length; currCharIndex++) {
                        if (!iterator.moveToNextChar() || iterator.run.type != TextRunType.TextRun)
                            return null;
                        if (foundName[currCharIndex] != iterator.getCurrentChar().toUpperCase())
                            return null;
                    }
                    iterator.moveToNextChar();
                    result.fieldNameToParser = objListAfterIteration[0];
                    return result;
                }
                else if (objListAfterIteration.length == 0)
                    return null;
                objListBeforeIteration = objListAfterIteration;
            } while (iterator.moveToNextChar());

            return null;// not need in it
        }
    }
}