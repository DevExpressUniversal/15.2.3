module __aspxRichEdit {
    export enum FieldCodeParserState {
        start = 0,
        addedParsersCodePart = 1,
        updatedParsersCodePart = 2,
        resultPartCreated = 3,
        addedParsersResultPart = 4,
        end = 5
    }

    export enum FieldSwitchType {
        Error = 0,
        DateAndTime = 1, // @
        Numeric = 2, // #
        General = 3, // *
        FieldSpecific = 4, // ! or 1 or 2 latin letters
    }

    export enum FieldMailMergeType {
        NonMailMerge = 1,
        MailMerge = 2,
        Mixed = 3
    }

    export class FieldSwitch {
        name: string;
        type: FieldSwitchType;
        arg: string;

        constructor(type: FieldSwitchType, name: string, arg: string) {
            this.name = name;
            this.type = type;
            this.arg = arg;
        }
    }

    export class FieldParameter {
        interval: FixedInterval;
        text: string;

        constructor(interval: FixedInterval, textRepresentation: string) {
            this.text = textRepresentation;
            this.interval = interval;
        }
    }

    // static funcs
    export class FieldCodeParserHelper {
        public static deleteFieldResultFromModel(control: IRichEditControl, subDocument: SubDocument, field: Field) {
            var resultInterval: FixedInterval = field.getResultInterval();
            if(resultInterval.length > 0) {
                control.history.beginTransaction();
                ModelManipulator.removeInterval(control, subDocument, resultInterval, true);
                ModelManipulator.addToHistorySelectionHistoryItem(control, new FixedInterval(resultInterval.start, 0), UpdateInputPositionProperties.Yes, control.selection.endOfLine);
                control.history.endTransaction();
            }
        }

        public static isWhitespaceAndTextRunType(char: string, type: TextRunType): boolean {
            return Utils.isWhitespace.test(char) && type == TextRunType.TextRun;
        }

        public static isBackslesh(char: string) {
            return char == "\\";
        }

        public static isQuote(char: string) {
            return char == "\"";
        }
    }

    export class FieldCodeParser {
        // IMPORTANT
        // Here most functions return boolean value, where TRUE mean that all entity this functions updated, FALSE mean that need update one more time
        // FALSE NOT mean that update incorrect, it mean ONLY that no need call update one more time

        control: IRichEditControl;
        subDocument: SubDocument;
        modelIterator: ModelIterator;
        lowLevelParsers: FieldCodeParser[];
        parserState: FieldCodeParserState;
        fieldsStack: Field[]; // need for iterate at code part top level field and result part low level fields

        // need for get character properties for result
        fieldNameFirstLetterPosition: Position;

        constructor(control: IRichEditControl, subDocument: SubDocument, field: Field, modelIterator: ModelIterator, fieldNameFirstLetterPosition: number) {
            this.control = control;
            this.subDocument = subDocument;
            this.fieldsStack = [field];
            this.modelIterator = modelIterator;
            this.lowLevelParsers = [];
            this.parserState = FieldCodeParserState.start;
            this.fieldNameFirstLetterPosition = subDocument.positionManager.registerPosition(fieldNameFirstLetterPosition);
        }

        // call for each updated or not field
        public static finalAction(field: Field, control: IRichEditControl, subDocument: SubDocument) {
            field.showCode = false;
            control.formatterOnIntervalChanged(field.getAllFieldInterval(), subDocument);
        }

        public destructor() {
            this.subDocument.positionManager.unregisterPosition(this.fieldNameFirstLetterPosition);
            this.fieldNameFirstLetterPosition = null;
            FieldCodeParser.finalAction(this.getTopField(), this.control, this.subDocument);
        }

        public getMailMergeType(): FieldMailMergeType {
            throw new Error(Errors.NotImplemented);
        }

        public handleSwitch(newSwitch: FieldSwitch): boolean {
            throw new Error(Errors.NotImplemented);
        }

        public handleParameter(newParameter: FieldParameter): boolean {
            throw new Error(Errors.NotImplemented);
        }

        // must set state resultCreated or allLowLevelFieldsInResultPartUpdated_endState or allLowLevelFieldsInCodePartUpdated
        // return value - see above
        public parseCodeCurrentFieldInternal(responce: any): boolean {
            throw new Error(Errors.NotImplemented);
        }

        // used when need insert text in result with formatting first run. Don't call when have mergeformat switch
        public setInputPositionState() {
            this.control.inputPosition.setPropertiesFromPosition(this.subDocument, this.fieldNameFirstLetterPosition.value);
        }

        public getTopField(): Field {
            return this.fieldsStack[0];
        }

        public update(responce: any): boolean {
            if (this.parserState == FieldCodeParserState.end)
                throw new Error("Excess call updated field");

            switch (this.parserState) {
                case FieldCodeParserState.start:
                    var field: Field = this.getTopField();
                    if (this.collectAndUpdateLowLevelFields(field.index + 1, field.getCodeStartPosition(), field.getSeparatorPosition())) {
                        this.parserState = FieldCodeParserState.updatedParsersCodePart;
                        return this.parseCodeCurrentField(null); // here can't be responce
                    }
                    else {
                        this.parserState = FieldCodeParserState.addedParsersCodePart;
                        return false;
                    }
                case FieldCodeParserState.addedParsersCodePart:
                    if (this.updateLowLevelFields(responce)) {
                        this.parserState = FieldCodeParserState.updatedParsersCodePart;
                        return this.parseCodeCurrentField(responce);
                    }
                    else
                        return false;
                case FieldCodeParserState.updatedParsersCodePart:
                    return this.parseCodeCurrentField(responce);
                case FieldCodeParserState.addedParsersResultPart:
                    return this.updateFieldsInResult(responce);
                case FieldCodeParserState.end:
                    throw new Error("Wrong way");
            }
        }

        // not change any states
        private collectAndUpdateLowLevelFields(fieldIndex: number, startPosition: number, endPosition: number): boolean {
            var fields: Field[] = this.subDocument.fields;
            for (var field: Field; field = fields[fieldIndex]; fieldIndex++) {
                if (field.getFieldStartPosition() > endPosition)
                    break;

                if (field.parent != this.getTopField() || field.getFieldEndPosition() <= startPosition) {
                    fieldIndex++;
                    continue;
                }

                var fieldParser: FieldCodeParser = FieldParserFabric.getParser(this.control, this.subDocument, field);
                if (fieldParser) {
                    if (!fieldParser.update(null))
                        this.lowLevelParsers.push(fieldParser);
                    else
                        fieldParser.destructor();
                }
                else
                    FieldCodeParserHelper.deleteFieldResultFromModel(this.control, this.subDocument, field);
            }
            return this.lowLevelParsers.length == 0;
        }

        // not change any states
        private updateLowLevelFields(responce: any): boolean {
            for (var parserIndex: number = 0, parser: FieldCodeParser; parser = this.lowLevelParsers[parserIndex]; parserIndex++) {
                if (parser.update(responce)) {
                    parser.destructor();
                    this.lowLevelParsers.splice(parserIndex, 1);
                    parserIndex--;
                }
            }
            return this.lowLevelParsers.length == 0;
        }

        // call when all internal fields in code part updated
        private parseCodeCurrentField(responce: any): boolean {
            if (this.parseCodeCurrentFieldInternal(responce)) {
                switch (this.parserState) {
                    case FieldCodeParserState.resultPartCreated:
                        return this.updateFieldsInResult(responce);
                    case FieldCodeParserState.end:
                        return true;
                    default: 
                        throw new Error("wrong way");
                }
            }
            else {
                if (this.parserState == FieldCodeParserState.updatedParsersCodePart)
                    return false;
                else
                    throw new Error("wrong way");
            }
        }

        // must set state resultPartCreatedAndParsersCreated or allLowLevelFieldsInResultPartUpdated_endState
        private updateFieldsInResult(responce: any): boolean {
            switch (this.parserState) {
                case FieldCodeParserState.resultPartCreated:
                    var fieldIndex: number = Field.normedBinaryIndexOf(this.subDocument.fields, this.getTopField().getResultStartPosition() + 1);
                    var field: Field = this.getTopField();
                    if (this.collectAndUpdateLowLevelFields(fieldIndex, field.getResultStartPosition(), field.getResultEndPosition())) {
                        this.parserState = FieldCodeParserState.end;
                        return true;
                    }
                    else {
                        this.parserState = FieldCodeParserState.addedParsersResultPart;
                        return false;
                    }
                case FieldCodeParserState.addedParsersResultPart:
                    if (this.updateLowLevelFields(responce)) {
                        this.parserState = FieldCodeParserState.end;
                        return true;
                    }
                    else
                        return false;
            }
        }

        // this func unlike this.modelIterator.moveToNextChar see run type and skip field codes
        // true - we still in field, false - we outside top level field
        private moveIteratorToNextChar(): boolean {
            if (this.modelIterator.run.type == TextRunType.FieldCodeEndRun)
                return false;

            if (!this.modelIterator.moveToNextChar())
                throw new Error("wrong way");

            while (true) {
                switch (this.modelIterator.run.type) {
                    case TextRunType.FieldCodeStartRun:
                        var fieldIndex: number = Field.normedBinaryIndexOf(this.subDocument.fields, this.modelIterator.getCurrectPosition() + 1);
                        var lowLevelField: Field = this.subDocument.fields[fieldIndex];
                        this.fieldsStack.push(lowLevelField);
                        this.modelIterator.setPosition(lowLevelField.getResultStartPosition());
                        break;
                    case TextRunType.FieldResultEndRun:
                    case TextRunType.FieldCodeEndRun:
                        var lowLevelField: Field = this.fieldsStack.pop();
                        if (this.fieldsStack.length == 0) {
                            this.fieldsStack.push(lowLevelField);// need always keep at least one field
                            return false;
                        }
                        this.modelIterator.setPosition(lowLevelField.getFieldEndPosition());
                        break;
                    default:
                        return true;
                }
            }
            return true;
        }

        public parseSwitchesAndArgs(needAtLestOneSpaceAfterFieldName: boolean): boolean {
            var letDoFirstIteration: boolean = false;
            if (needAtLestOneSpaceAfterFieldName) {
                var prevPos: number = this.modelIterator.getCurrectPosition();
                if (this.skipWhitespaces())
                    this.modelIterator.setPosition(prevPos);
                else
                    return this.modelIterator.run.type == TextRunType.FieldCodeEndRun;
            }

            while (this.skipWhitespaces() && this.modelIterator.run.type != TextRunType.FieldCodeEndRun) {
                var currChar: string = this.modelIterator.getCurrentChar();
                if (FieldCodeParserHelper.isBackslesh(currChar)) { // switch
                    var switchInfo: FieldSwitch = this.getSwitchInfo();
                    if (switchInfo.type == FieldSwitchType.Error || !this.handleSwitch(switchInfo))
                        return false;
                }
                else { // argument
                    var paramInfo: FieldParameter = this.getFieldParameterInfo();
                    if (!paramInfo || !this.handleParameter(paramInfo))
                        return false;
                }
            }
            return this.modelIterator.run.type == TextRunType.FieldCodeEndRun;
        }

        private skipWhitespaces(): boolean {
            var isFindWhitespace: boolean = false;
            do {
                if (FieldCodeParserHelper.isWhitespaceAndTextRunType(this.modelIterator.getCurrentChar(), this.modelIterator.run.type) ||
                    this.modelIterator.run.type == TextRunType.ParagraphRun ||
                    this.modelIterator.run.type == TextRunType.SectionRun)
                    isFindWhitespace = true;
                else
                    break;
            } while (this.moveIteratorToNextChar());
            return isFindWhitespace;
        }

        private getFieldParameterInfo(): FieldParameter {
            var startPosition: number = this.modelIterator.getCurrectPosition();
            var parseResult: { argListChars: string[]; quoted: boolean } = this.parseSwitchOrFieldArgument();
            if (!parseResult)
                return null;

            var argInterval: FixedInterval = parseResult.quoted ?
                FixedInterval.fromPositions(startPosition + 1, this.modelIterator.getCurrectPosition() - 1) :
                FixedInterval.fromPositions(startPosition, this.modelIterator.getCurrectPosition())

            return new FieldParameter(argInterval, parseResult.argListChars.join(""));
        }

        // call here when we stand after "\\". SwitchType.None mean error
        private getSwitchInfo(): FieldSwitch {
            if (!this.moveIteratorToNextChar() || this.modelIterator.run.type != TextRunType.TextRun)
                return new FieldSwitch(FieldSwitchType.Error, "", "");

            var currChar: string = this.modelIterator.getCurrentChar();
            switch (currChar) {
                case "*": return this.makeSwitchInfo(FieldSwitchType.General, currChar, true);
                case "@": return this.makeSwitchInfo(FieldSwitchType.DateAndTime, currChar, true);
                case "#": return this.makeSwitchInfo(FieldSwitchType.Numeric, currChar, true);
                default:
                    if (currChar == "!")
                        return this.makeSwitchInfo(FieldSwitchType.FieldSpecific, currChar, true);

                    if (!Utils.isLatinLetter.test(currChar))
                        return new FieldSwitch(FieldSwitchType.Error, "", "");

                    var switchName: string = currChar;
                    var lastPos: number = this.modelIterator.getCurrectPosition();
                    if (this.moveIteratorToNextChar()) {
                        currChar = this.modelIterator.getCurrentChar();
                        if (Utils.isLatinLetter.test(currChar))
                            switchName += currChar;
                        else
                            this.modelIterator.setPosition(lastPos);
                    }
                    else
                        this.modelIterator.setPosition(lastPos);
                    return this.makeSwitchInfo(FieldSwitchType.FieldSpecific, switchName, false);
            }
        }

        private makeSwitchInfo(switchType: FieldSwitchType, switchName: string, needArgument: boolean): FieldSwitch {
            var switchArg: string = this.getSwitchArgument(needArgument);
            if (switchArg === null || needArgument && switchArg.length == 0)
                return new FieldSwitch(FieldSwitchType.Error, "", "");
            return new FieldSwitch(switchType, switchName, switchArg);
        }

        // call when you in position "\ * MERGEFORMAT"
        //                             ^
        private getSwitchArgument(needArgument: boolean): string {
            if (!this.moveIteratorToNextChar())
                return needArgument ? null : "";

            if (!this.skipWhitespaces())
                return null;

            var parseResult: { argListChars: string[]; quoted: boolean } = this.parseSwitchOrFieldArgument();
            if (!parseResult)
                return null;

            var resArg: string = parseResult.argListChars.join("");
            if (resArg.length == 0)
                return null;

            return resArg;
        }

        private parseSwitchOrFieldArgument(): { argListChars: string[]; quoted: boolean } {
            if (this.modelIterator.run.type == TextRunType.FieldCodeEndRun)
                return null;

            // parameter must start and end in ONE field
            var resList: string[] = [];
            var startFieldStackSize: number = this.fieldsStack.length;
            var lastFieldStackLength: number = startFieldStackSize;

            var currChar: string = this.modelIterator.getCurrentChar();
            var needSearchNextQuote: boolean = FieldCodeParserHelper.isQuote(currChar);
            if (needSearchNextQuote)
                if (!this.moveIteratorToNextChar())
                    return null;

            var lastSymbolIsQuote: boolean = !needSearchNextQuote;

            do {
                currChar = this.modelIterator.getCurrentChar();
                if (needSearchNextQuote) {
                    if (FieldCodeParserHelper.isQuote(currChar)) {
                        var prevChar: string = resList[resList.length - 1];
                        if (!(prevChar && FieldCodeParserHelper.isBackslesh(prevChar))) {
                            lastSymbolIsQuote = true;
                            lastFieldStackLength = this.fieldsStack.length;
                            this.moveIteratorToNextChar();
                            break;
                        }
                    }
                }
                else
                    if (FieldCodeParserHelper.isWhitespaceAndTextRunType(currChar, this.modelIterator.run.type))
                        break;

                resList.push(currChar);
                lastFieldStackLength = this.fieldsStack.length;
            } while (this.moveIteratorToNextChar());

            if (startFieldStackSize != lastFieldStackLength || !lastSymbolIsQuote)
                return null;

            return { argListChars: resList, quoted: needSearchNextQuote }
        }
    }
}