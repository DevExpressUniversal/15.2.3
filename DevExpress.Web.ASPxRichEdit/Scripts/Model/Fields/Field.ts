module __aspxRichEdit {
    export enum FieldNameType {
        None = 0,
        CreateDate = 1,
        Date = 2,
        DocVariable = 3,
        Hyperlink = 4,
        If = 5,
        IncludePicture = 6,
        MergeField = 7,
        NumPages = 8,
        Page = 9,
        Seq = 10,
        Symbol = 11,
        TC = 12,
        TOC = 13,
        Formula = 14 // =
    }

    export class HyperlinkInfo implements ICloneable<HyperlinkInfo> {
        uri: string;
        tip: string;
        anchor: string; // - bookmark or anchor on page with uri
        visited: boolean;

        constructor(uri: string, anchor: string, tip: string, visited: boolean) {
            this.uri = uri ? uri : "";
            this.anchor = anchor ? anchor : "";
            this.tip = tip ? tip : "";
            this.visited = visited ? visited : false;
        }

        clone(): HyperlinkInfo {
            return new HyperlinkInfo(this.uri, this.anchor, this.tip, this.visited);
        }

        getUriPlusAnchor(): string {
            return this.uri + (this.anchor == "" ? "" : "#" + this.anchor);
        }
    }

    export class Field {
        // IMPORTANT!!! Use getCodeStartPosition when search by fieldsList
        private codeStartPosition: Position;
        private resultStartPosition: Position;
        private fieldEndPosition: Position;
        private hyperlinkInfo: HyperlinkInfo; // not undefined only if this field - hyperlink

        parent: Field; // field satisfies parent.startPosition < this.startPosition and parent.endPosition > this.endPosition (not matter where located daughter field - in code or result section)
        showCode: boolean; // true - code, false - result
        index: number; // index in subDocument.fields. IMPORTANT! Need correct it manually

        // after constructor need call setParent or findParent
        constructor(positionManager: PositionManager, index: number, startFieldPosition: number, separatorPosition: number, endFieldPosition: number, showCode: boolean, hyperlinkInfo: HyperlinkInfo) {
            this.codeStartPosition = positionManager.registerPosition(startFieldPosition + 1);  // this need to be that!
            this.resultStartPosition = positionManager.registerPosition(separatorPosition + 1);  // this need to be that!
            this.fieldEndPosition = positionManager.registerPosition(endFieldPosition);
            this.index = index;
            this.showCode = showCode;
            this.parent = undefined;
            if (hyperlinkInfo !== undefined)
                this.hyperlinkInfo = hyperlinkInfo;
        }

        destructor(positionManager: PositionManager) {
            positionManager.unregisterPosition(this.codeStartPosition);
            positionManager.unregisterPosition(this.resultStartPosition);
            positionManager.unregisterPosition(this.fieldEndPosition);
        }

        static addField(fields: Field[], newField: Field) {
            var field: Field;
            var fieldIndex: number;

            fields.splice(newField.index, 0, newField);
            for (fieldIndex = newField.index + 1; field = fields[fieldIndex]; fieldIndex++)
                field.index++;

            // reset parents
            for (fieldIndex = newField.index - 1; field = fields[fieldIndex]; fieldIndex--) {
                if (field.getFieldEndPosition() <= newField.getFieldStartPosition())
                    break;
            }
            var resetParentFrom: number = Math.max(0, fieldIndex);

            for (fieldIndex = newField.index + 1; field = fields[fieldIndex]; fieldIndex++) {
                if (field.getFieldStartPosition() >= newField.getFieldEndPosition())
                    break;
            }
            var resetParentToIndex: number = Math.min(fields.length - 1, fieldIndex);

            for (fieldIndex = resetParentFrom; fieldIndex <= resetParentToIndex; fieldIndex++)
                fields[fieldIndex].initParent(fields);
        }

        static deleteFieldByIndex(subDocument: SubDocument, delFieldIndex: number, modelManipulator: ModelManipulator) {
            var fields: Field[] = subDocument.fields;
            var delField: Field = fields[delFieldIndex];
            for (var fieldIndex: number = delFieldIndex + 1, field: Field; field = fields[fieldIndex]; fieldIndex++)
                field.index--;
            
            var delFieldEndPos: number = delField.getFieldEndPosition();
            for (var i: number = delFieldIndex + 1, currField: Field; currField = fields[i]; i++) {
                if (currField.parent == delField)
                    currField.parent = delField.parent;
                if (currField.getFieldStartPosition() >= delFieldEndPos)
                    break;
            }

            fields.splice(delField.index, 1);

            delField.destructor(subDocument.positionManager);
            modelManipulator.dispatcher.notifyFieldDeleted(subDocument, delFieldEndPos);
        }

        isHyperlinkField(): boolean {
            return this.hyperlinkInfo !== undefined;
        }

        setNewHyperlinkInfo(hyperlinkInfo: HyperlinkInfo) {
            this.hyperlinkInfo = hyperlinkInfo;
        }

        getHyperlinkInfo(): HyperlinkInfo {
            return this.hyperlinkInfo;
        }

        getFieldStartPosition(): number {
            return this.codeStartPosition.value - 1;
        }

        getCodeStartPosition() {
            return this.codeStartPosition.value
        }

        getSeparatorPosition(): number {
            return this.resultStartPosition.value - 1;
        }

        getResultStartPosition(): number {
            return this.resultStartPosition.value;
        }

        getResultEndPosition(): number {
            return this.fieldEndPosition.value - 1;
        }

        getFieldEndPosition(): number {
            return this.fieldEndPosition.value;
        }

        //isResultEmpty(): boolean {
        //    return this.resultStartPosition.value + 1 == this.fieldEndPosition.value;
        //}

        //isCodeEmpty(): boolean {
        //    return this.codeStartPosition.value + 1 == this.resultStartPosition.value;
        //}

        setParent(parent: Field) {
            if (parent !== null && (parent.getFieldStartPosition() >= this.getFieldStartPosition() || parent.getFieldEndPosition() <= this.getFieldEndPosition()))
                throw new Error("Incorrect field parent");
            this.parent = parent;
        }

        // IMPORTANT - need at first calculate all parents fields with indexes < this.index
        initParent(fieldList: Field[]) {
            for (var i: number = this.index - 1, possibleParent: Field; possibleParent = fieldList[i]; i--) {
                if (possibleParent.getFieldEndPosition() > this.getFieldEndPosition()) {
                    this.parent = possibleParent;
                    return;
                }
                if (possibleParent.parent == null)
                    break;
            }
            this.parent = null;
        }

        // IMPORTANT!!! Use getCodeStartPosition when try search by fieldsList. NOT the start field index
        static normedBinaryIndexOf(fields: Field[], position: number) {
            return Utils.normedBinaryIndexOf(fields, (f: Field) => f.getCodeStartPosition() - position);
        }

        static binaryIndexOf(fields: Field[], position: number) {
            return Utils.normedBinaryIndexOf(fields, (f: Field) => f.getCodeStartPosition() - position);
        }

        getAbsolutelyTopLevelField(fields: Field[]): Field {
            var field: Field = this;
            for (; field.parent; field = field.parent);
            return field;
        }

        getCodeInterval(): FixedInterval {
            return FixedInterval.fromPositions(this.getCodeStartPosition(), this.getSeparatorPosition());
        }

        getCodeIntervalWithBorders(): FixedInterval {
            return FixedInterval.fromPositions(this.getFieldStartPosition(), this.getResultStartPosition());
        }

        getResultInterval(): FixedInterval {
            return FixedInterval.fromPositions(this.getResultStartPosition(), this.getResultEndPosition());
        }

        getResultIntervalWithBorders(): FixedInterval {
            return FixedInterval.fromPositions(this.getResultStartPosition(), this.getFieldEndPosition());
        }

        getAllFieldInterval(): FixedInterval {
            return FixedInterval.fromPositions(this.getFieldStartPosition(), this.getFieldEndPosition());
        }

        getAllFieldIntervalWithoutBorders(): FixedInterval {
            return FixedInterval.fromPositions(this.getCodeStartPosition(), this.getResultEndPosition());
        }

        isPlacedInCodeAreaTopLevelField(topLevelField: Field): boolean {
            return !!FixedInterval.getIntersection(this.getAllFieldInterval(), topLevelField.getCodeInterval());
        }

        // can extend interval if some fields borders intersect
        // return result used for select result in hyperlink field when result consider only image
        static correctIntervalDueToFields(layout: DocumentLayout, subDocument: SubDocument, newInterval: FixedInterval, pageIndex?: number): Field {
            var fields: Field[] = subDocument.fields;
            if (fields.length == 0)
                return null;

            var startInterval: FixedInterval = newInterval.clone();
            if (newInterval.length == 0) {
                // need selection out from invisible field part
                var position: number = newInterval.start;

                var visabilityInfo: FieldVisabilityInfo[] = FieldVisabilityInfo.getRelativeVisabilityInfo(position, fields);
                for (var i: number = visabilityInfo.length - 1, fieldInfo: FieldVisabilityInfo; fieldInfo = visabilityInfo[i]; i--) {
                    var field: Field = fieldInfo.field;
                    if (field.getCodeInterval().contains(position)) {
                        if (fieldInfo.showCode)
                            break;
                        else
                            position = field.getFieldStartPosition();
                    }
                    else {
                        var fieldResultInterval: FixedInterval = field.getResultInterval();
                        if (fieldResultInterval.contains(position)) {
                            if (fieldInfo.showResult) {
                                if (position == fieldResultInterval.start)
                                    position = field.getFieldStartPosition();
                                else if (position == fieldResultInterval.end())
                                    position = field.getFieldEndPosition();
                                break;
                            }
                            else
                                position = field.getFieldEndPosition();
                        }
                    }
                }

                newInterval.start = position;
                return null;
            }

            var fieldIndexesInfo: { startIndex: number; endIndex: number } = Field.getIndexesRangeFieldsThatIntersectIntervals(newInterval, fields);
            if (fieldIndexesInfo.startIndex == fieldIndexesInfo.endIndex) { // when selected all result part
                var field: Field = fields[Math.max(0, Field.normedBinaryIndexOf(fields, newInterval.start))];
                if (!field.showCode && field.getResultStartPosition() == newInterval.start && field.getResultEndPosition() == newInterval.end()) {
                    var fieldInterval: FixedInterval = field.getAllFieldInterval();
                    newInterval.start = fieldInterval.start;
                    newInterval.length = fieldInterval.length;
                    return field;
                }
            }

            var maxBoundByLeft: number = newInterval.start;
            var minBoundByRight: number = newInterval.end();
            for (var fieldIndex: number = fieldIndexesInfo.startIndex; fieldIndex <= fieldIndexesInfo.endIndex; fieldIndex++) {
                var field: Field = fields[fieldIndex];
                var intersection: FixedInterval = FixedInterval.getIntersection(field.getAllFieldIntervalWithoutBorders(), newInterval);
                // not correct when 
                if (intersection) {
                    if (field.getFieldStartPosition() < maxBoundByLeft)
                        maxBoundByLeft = field.getFieldStartPosition();
                    if (field.getFieldEndPosition() > minBoundByRight)
                        minBoundByRight = field.getFieldEndPosition();
                    newInterval.expandInterval(field.getAllFieldInterval());
                }
            }
            
            
            var layoutPositionStartInterval: LayoutPosition = (subDocument.isMain() ? new LayoutPositionMainSubDocumentCreator(layout, subDocument, newInterval.start, DocumentLayoutDetailsLevel.Character)
                : new LayoutPositionOtherSubDocumentCreator(layout, subDocument, newInterval.start, pageIndex, DocumentLayoutDetailsLevel.Character))
                .create(new LayoutPositionCreatorConflictFlags().setDefault(false), new LayoutPositionCreatorConflictFlags().setDefault(false));
            if (!layoutPositionStartInterval) { // Think that interval correct.
                newInterval.start = startInterval.start;
                newInterval.length = startInterval.length;
                return null;
            }
            layoutPositionStartInterval.switchToStartNextBoxInRow();
            
            var layoutPositionEndInterval: LayoutPosition = (subDocument.isMain() ? new LayoutPositionMainSubDocumentCreator(layout, subDocument, newInterval.end(), DocumentLayoutDetailsLevel.Character)
                : new LayoutPositionOtherSubDocumentCreator(layout, subDocument, newInterval.end(), pageIndex, DocumentLayoutDetailsLevel.Character))
                .create(new LayoutPositionCreatorConflictFlags().setDefault(true), new LayoutPositionCreatorConflictFlags().setDefault(true));
            if(!layoutPositionEndInterval) { // Think again. That interval is correct :)
                newInterval.start = startInterval.start;
                newInterval.length = startInterval.length;
                return null;
            }
            layoutPositionEndInterval.switchToEndPrevBoxInRow();

            newInterval.start = Math.min(maxBoundByLeft, layoutPositionStartInterval.getLogPosition());
            newInterval.length = Math.max(layoutPositionEndInterval.getLogPosition(), minBoundByRight) - newInterval.start;
            return null;
        }

        // result index range collect all fields, what intersect interval, or consider in interval
        // [n [n] [y[y] [y [y] [y] ]] ] n - no, y - yes
        //          [               ]
        // no use result this function if startIndex===endIndex
        static getIndexesRangeFieldsThatIntersectIntervals(interval: FixedInterval, fields: Field[]): { startIndex: number; endIndex: number} {
            if (fields.length == 0)
                return {startIndex: 0, endIndex: 0};

            var intervalEnd: number = interval.end();
            var fieldIndex: number = Math.max(0, Field.normedBinaryIndexOf(fields, interval.start + 1)); // max for case field [2, 5, 9] and selection [1, 8]

            var field: Field = fields[fieldIndex];

            while (field.parent) {
                var parent: Field = field.parent;
                if (parent.showCode ?
                    interval.start <= parent.getFieldStartPosition() && intervalEnd <= parent.getSeparatorPosition() ||
                        interval.start > parent.getFieldStartPosition() && intervalEnd > parent.getSeparatorPosition() :
                    interval.start <= parent.getSeparatorPosition() && intervalEnd < parent.getFieldEndPosition() ||
                        interval.start > parent.getSeparatorPosition() && intervalEnd >= parent.getFieldEndPosition()) {
                    field = parent;
                }
                else
                    break;
            }

            var currStartIndex: number = field.index;
            if (field.getResultEndPosition() < interval.start)
                currStartIndex++;
            
            for(let i = currStartIndex; field = fields[i]; i++) {
                fieldIndex = i;
                if (field.showCode ? intervalEnd <= field.getFieldStartPosition() : intervalEnd < field.getResultStartPosition())
                    break;

                // mean field consider all interval
                if ((field.showCode ?
                    field.getFieldStartPosition() < interval.start && field.getResultStartPosition() > intervalEnd :
                    field.getResultStartPosition() <= interval.start && field.getResultEndPosition() >= intervalEnd)) {
                    currStartIndex++;
                    continue;
                }
            }

            return { startIndex: currStartIndex, endIndex: fieldIndex };
        }

        public static correctWhenPositionInStartCode(fields: Field[], position: number): number {
            if (fields.length < 1)
                return position
            var field: Field = fields[Math.max(0, Field.normedBinaryIndexOf(fields, position))];
            if (field.getResultStartPosition() == position)
                return field.parent ? Field.correctWhenPositionInStartCode(fields, field.getFieldStartPosition()) : field.getFieldStartPosition();
            return position;
        }

        public static jumpThroughFieldToRight(fields: Field[], selection: Selection) {
            if (fields.length == 0 || selection.forwardDirection)
                return;

            var interval: FixedInterval = selection.getLastSelectedInterval();
            var position: number = interval.start;
            var field: Field = fields[Math.max(0, Field.normedBinaryIndexOf(fields, position + 1))];
            if (field.getFieldStartPosition() == position)
                selection.extendLastSelection(field.getFieldEndPosition(), false, -1, UpdateInputPositionProperties.Yes);
        }

        public static jumpThroughFieldToLeft(fields: Field[], selection: Selection) {
            if (fields.length == 0 || !selection.forwardDirection)
                return;

            var interval: FixedInterval = selection.getLastSelectedInterval();
            var position: number = interval.end();
            var field: Field = fields[Math.max(0, Field.normedBinaryIndexOf(fields, position))];
            if (field.getFieldEndPosition() == position)
                selection.extendLastSelection(field.getFieldStartPosition(), false, -1, UpdateInputPositionProperties.Yes);
        }

        //forceCheck - for tests
        public static DEBUG_FIELDS_CHECKS(subDocument: SubDocument, forceCheck: boolean = false) {
            var disableDegugTest: boolean = true; // MUST BE true for release
            if (!forceCheck && disableDegugTest)
                return;

            var fields: Field[] = subDocument.fields;
            for (var fieldIndex: number = 0, field: Field; field = fields[fieldIndex]; fieldIndex++) {
                if (field.index != fieldIndex)
                    throw new Error("DEBUG_FIELDS_CHECKS incorrect index " + field.index + " must be " + fieldIndex);

                var chunkAndRunInfoStartCode: FullChunkAndRunInfo = subDocument.getRunAndIndexesByPosition(field.getFieldStartPosition());
                if (chunkAndRunInfoStartCode.run.type != TextRunType.FieldCodeStartRun)
                    throw new Error("DEBUG_FIELDS_CHECKS incorrect run type");

                var chunkAndRunInfoSeparator: FullChunkAndRunInfo = subDocument.getRunAndIndexesByPosition(field.getSeparatorPosition());
                if (chunkAndRunInfoSeparator.run.type != TextRunType.FieldCodeEndRun)
                    throw new Error("DEBUG_FIELDS_CHECKS incorrect run type");

                var chunkAndRunInfoEndField: FullChunkAndRunInfo = subDocument.getRunAndIndexesByPosition(field.getResultEndPosition());
                if (chunkAndRunInfoEndField.run.type != TextRunType.FieldResultEndRun)
                    throw new Error("DEBUG_FIELDS_CHECKS incorrect run type");

                if (chunkAndRunInfoStartCode.getAbsoluteRunPosition() >= chunkAndRunInfoSeparator.getAbsoluteRunPosition() ||
                    chunkAndRunInfoStartCode.getAbsoluteRunPosition() >= chunkAndRunInfoEndField.getAbsoluteRunPosition() ||
                    chunkAndRunInfoSeparator.getAbsoluteRunPosition() >= chunkAndRunInfoEndField.getAbsoluteRunPosition())
                    throw new Error("DEBUG_FIELDS_CHECKS incorrect some of main positions");

                if (field.parent) {
                    var fieldInterval: FixedInterval = field.getAllFieldInterval();
                    if (!(field.parent.getCodeInterval().containsInterval(fieldInterval.start, fieldInterval.end()) ||
                        field.parent.getResultInterval().containsInterval(fieldInterval.start, fieldInterval.end())))
                        throw new Error("DEBUG_FIELDS_CHECKS error with intervals");

                    if (field.parent.index >= field.index)
                        throw new Error("DEBUG_FIELDS_CHECKS error with parent and current indexes");
                }
            }
            if (!forceCheck)
                console.log("Field check correct. Please disable DEBUG_FIELDS_CHECKS if it release.");
        }
    }

    export class FieldVisabilityInfo implements ICloneable<FieldVisabilityInfo>, ISupportCopyFrom<FieldVisabilityInfo> {
        // show code and show resulc can be both false, but can't be both true
        showCode: boolean;
        showResult: boolean;
        field: Field;

        constructor(showCode: boolean, showResult: boolean, field: Field) {
            this.showCode = showCode;
            this.showResult = showResult;
            this.field = field;
        }

        static getRelativeVisabilityInfo(position: number, fields: Field[]): FieldVisabilityInfo[] {
            var visabilityInfo: FieldVisabilityInfo[] = [];

            var currFieldIndex: number = Field.normedBinaryIndexOf(fields, position + 1);
            if (currFieldIndex < 0)
                return [];

            var currField: Field = fields[currFieldIndex];
            do {
                if(currField.getAllFieldIntervalWithoutBorders().contains(position))
                    visabilityInfo.unshift(new FieldVisabilityInfo(currField.showCode, !currField.showCode, currField));
            } while (currField = currField.parent);

            var topLevelFieldInfo: FieldVisabilityInfo = visabilityInfo[0];
            for (var i: number = 1, fieldInfo: FieldVisabilityInfo; fieldInfo = visabilityInfo[i]; i++) {
                FieldVisabilityInfo.applyTopLevelFieldInfoVisabilityToThisFieldInfo(topLevelFieldInfo, fieldInfo);
                topLevelFieldInfo = fieldInfo;
            }
            return visabilityInfo;
        }

        static applyTopLevelFieldInfoVisabilityToThisFieldInfo(topLevelFieldInfo: FieldVisabilityInfo, lowLevelFieldInfo: FieldVisabilityInfo) {
            var topLevelFieldAllowShowThisField: boolean = lowLevelFieldInfo.field.isPlacedInCodeAreaTopLevelField(topLevelFieldInfo.field) ? topLevelFieldInfo.showCode : topLevelFieldInfo.showResult;

            lowLevelFieldInfo.showCode = lowLevelFieldInfo.showCode && topLevelFieldAllowShowThisField;
            lowLevelFieldInfo.showResult = lowLevelFieldInfo.showResult && topLevelFieldAllowShowThisField;
        }

        clone(): FieldVisabilityInfo {
            return new FieldVisabilityInfo(this.showCode, this.showResult, this.field);
        }

        copyFrom(obj: FieldVisabilityInfo) {
            this.field = obj.field;
            this.showCode = obj.showCode;
            this.showResult = obj.showResult;
        }

    }
}