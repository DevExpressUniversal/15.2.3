module __aspxRichEdit {
    // Please CARE with fieldUpdateOperation. Here used Position class and he manually operated!!!

    export enum ServerUpdateFieldType {
        DocVariable = 1,
        MergeField = 2
    }

    class FieldParsersAndIntervals {
        interval: LinkedInterval;
        parsers: FieldCodeParser[];
        updated: boolean;

        constructor(interval: LinkedInterval) {
            this.interval = interval;
            this.parsers = [];
            this.updated = false;
        }

        destructor(manager: PositionManager) {
            this.interval.destructor(manager);
        }
    }

    class RequestInfo {
        type: ServerUpdateFieldType;
        data: any;
        fieldID: number;

        constructor(type: ServerUpdateFieldType, data: any, fieldID: number) {
            this.type = type;
            this.data = data;
            this.fieldID = fieldID;
        }
    }

    export class FieldsWaitingForUpdate {
        callbackFunc: any;
        control: IRichEditControl;
        subDocument: SubDocument;
        fields: Field[];
        

        private infoForFutureUpdate: FieldParsersAndIntervals[] = [];
        private requests: RequestInfo[] = [];
        private needCallEndUpdate: boolean;

        // callbackFunc can be null
        constructor(control: IRichEditControl, callbackFunc: any) {
            this.callbackFunc = callbackFunc;
            this.control = control;
            this.subDocument = control.model.activeSubDocument;
            this.fields = this.subDocument.fields;
            this.subDocument.fieldsWaitingForUpdate = this;
            var fixedIntervals: FixedInterval[] = control.selection.intervals;
            this.needCallEndUpdate = false;

            for (var intervalIndex: number = 0, interval: FixedInterval; interval = fixedIntervals[intervalIndex]; intervalIndex++)
                this.infoForFutureUpdate.push(new FieldParsersAndIntervals(new LinkedInterval(this.subDocument.positionManager, interval.start, interval.end())));
        }

        // calls from parsers
        public addRequest(type: ServerUpdateFieldType, data: any, fieldID: number) {
            this.requests.push(new RequestInfo(type, data, fieldID));
        }

        // update must be null for first call
        public update(responce: any) {
            this.updateChesks(responce);
            this.requests = [];

            this.needCallEndUpdate = !!responce;
            if (this.needCallEndUpdate)
                this.control.beginUpdate();
            else
                this.startAction();

            var countUpdatedInfos: number = 0;
            for (var infoIndex: number = 0, info: FieldParsersAndIntervals; info = this.infoForFutureUpdate[infoIndex]; infoIndex++) {
                if (info.updated) {
                    countUpdatedInfos++;
                    continue;
                }

                if (info.parsers.length > 0) {
                    if (this.continueUpdateCurrentInterval(info.parsers, responce)) {
                        info.updated = true;
                        countUpdatedInfos++;
                    }
                    continue;
                }

                var fieldIndex: number = Math.max(0, Field.normedBinaryIndexOf(this.fields, info.interval.start.value + 1));
                var field: Field = this.fields[fieldIndex];

                while (!field.getAllFieldInterval().contains(info.interval.start.value) && field.parent)
                    field = field.parent;

                var startParent: Field = field.parent;

                var someFieldInCurrentInfoNotUpdated: boolean = false;
                for (fieldIndex = field.index; field = this.fields[fieldIndex]; fieldIndex++) {
                    if (field.getFieldStartPosition() >= info.interval.end.value)
                        break;
                    if (field.getFieldEndPosition() <= info.interval.start.value || (field.parent != null && field.parent != startParent))
                        continue;

                    var fieldParser: FieldCodeParser = FieldParserFabric.getParser(this.control, this.subDocument, field);
                    if (fieldParser) {
                        if (!fieldParser.update(responce)) {
                            someFieldInCurrentInfoNotUpdated = true;
                            info.parsers.push(fieldParser);
                        }
                        else
                            fieldParser.destructor();
                    }
                    else {
                        var resultInterval: FixedInterval = field.getResultInterval();
                        if(resultInterval.length > 0) {
                            this.control.history.beginTransaction();
                            ModelManipulator.removeInterval(this.control, this.subDocument, resultInterval, true);
                            ModelManipulator.addToHistorySelectionHistoryItem(this.control, new FixedInterval(resultInterval.start, 0), UpdateInputPositionProperties.Yes, this.control.selection.endOfLine);
                            this.control.history.endTransaction();
                        }
                        FieldCodeParser.finalAction(field, this.control, this.subDocument);
                    }
                }

                if (!someFieldInCurrentInfoNotUpdated) {
                    info.updated = true;
                    countUpdatedInfos++;
                }
            }

            if (this.needCallEndUpdate) {
                this.control.endUpdate();
                this.needCallEndUpdate = false;
            }

            if (this.infoForFutureUpdate.length != countUpdatedInfos)
                this.sentRequest();
            else
                this.endAction();
        }

        private updateChesks(responce: any) {
            if (this.requests.length > 1 && !responce ||
                this.requests.length == 0 && responce)
                throw new Error("Wrong way");

            for (var i: number = 0, requestInfo: RequestInfo; requestInfo = this.requests[i]; i++) {
                if (!responce[requestInfo.fieldID])
                    throw new Error("Wrong way");
            }
        }

        private startAction() {
            this.control.beginLoading();
            this.control.history.beginTransaction();
        }

        // stop update. All changes are saved
        endAction() {
            if (this.needCallEndUpdate) {
                this.control.endUpdate();
                this.needCallEndUpdate = false;
            }
            var selection: Selection = this.control.selection;
            for (var i: number = 0, info: FieldParsersAndIntervals; info = this.infoForFutureUpdate[i]; i++) {
                if (i == 0)
                    selection.setSelection(info.interval.start.value, info.interval.end.value, false, -1, UpdateInputPositionProperties.Yes, true);
                else
                    selection.addSelection(info.interval.start.value, info.interval.end.value, false, -1);
                info.destructor(this.subDocument.positionManager);
            }
            this.infoForFutureUpdate = [];

            this.control.history.endTransaction();
            this.control.endLoading();
            this.subDocument.fieldsWaitingForUpdate = null;
            if (this.callbackFunc)
                this.callbackFunc();
            Field.DEBUG_FIELDS_CHECKS(this.subDocument);
        }

        private continueUpdateCurrentInterval(fieldParsers: FieldCodeParser[], responce: any): boolean {
            var allFieldUpdated: boolean = true;
            for (var parserIndex: number = 0, parser: FieldCodeParser; parser = fieldParsers[parserIndex]; parserIndex++) {
                if (parser.update(responce)) {
                    parser.destructor();
                    fieldParsers.splice(parserIndex, 1);
                    parserIndex--;
                }
                else
                    allFieldUpdated = false;
            }
            return allFieldUpdated;
        }

        private sentRequest() {
            if (!this.requests)
                throw new Error("Wrong way");

            var sendData: any = {
                subDocID: this.subDocument.id,
                activeRecord: this.control.mailMergeOptions.activeRecordIndex,
                info: {}
            };
            for (var i: number = 0, requestInfo: RequestInfo; requestInfo = this.requests[i]; i++)
                sendData.info[requestInfo.fieldID] = {
                    type: requestInfo.type,
                    data: requestInfo.data
                };

            var reqParams: RequestParams = new RequestParams();
            reqParams.processOnCallback = true;
            reqParams.immediateSend = true;

            this.control.serverDispatcher.pushRequest(CommandType.FieldUpdate, sendData, undefined, reqParams);
        }
    }
}