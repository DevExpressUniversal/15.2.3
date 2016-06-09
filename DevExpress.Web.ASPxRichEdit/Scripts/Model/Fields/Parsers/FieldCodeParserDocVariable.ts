module __aspxRichEdit {
    export class FieldCodeParserDocVariable extends FieldCodeParser {
        protected switchInfoList: FieldSwitch[] = [];
        protected parameterInfoList: FieldParameter[] = [];
        protected fieldID: number;

        getMailMergeType(): FieldMailMergeType {
            return FieldMailMergeType.NonMailMerge;
        }

        getServerUpdateFieldType(): ServerUpdateFieldType {
            return ServerUpdateFieldType.DocVariable;
        }

        handleSwitch(newSwitch: FieldSwitch): boolean {
            this.switchInfoList.push(newSwitch);
            return true;
        }

        handleParameter(newParameter: FieldParameter): boolean {
            this.parameterInfoList.push(newParameter);
            return true;
        }

        parseCodeCurrentFieldInternal(responce: any): boolean {
            // here responce always == null
            if(responce && this.fieldID != undefined && responce[this.fieldID]) {
                if(this.applyResponse(responce[this.fieldID]))
                    this.parserState = FieldCodeParserState.resultPartCreated;
                else
                    this.parserState = FieldCodeParserState.end;
                return true;
            }

            FieldCodeParserHelper.deleteFieldResultFromModel(this.control, this.subDocument, this.getTopField()); // delete only once
            if (!this.parseSwitchesAndArgs(true)) {
                this.parserState = FieldCodeParserState.end;
                return true;
            }
            if (this.insertDefaultText()) {
                this.parserState = FieldCodeParserState.end;
                return true;
            }
            if (!this.placeRequest()) {
                this.parserState = FieldCodeParserState.end;
                return true;
            }
            return false;
        }

        protected insertDefaultText(): boolean {
            return false;
        }

        protected placeRequest(): boolean {
            if (this.parameterInfoList.length < 1)
                return false;
            this.fieldID = this.control.serverDispatcher.getUpdateFieldID();
            this.subDocument.fieldsWaitingForUpdate.addRequest(this.getServerUpdateFieldType(), this.getRequestData(), this.fieldID);

            return true;
        }

        protected getRequestData(): any {
            var data: any = {
                name: this.parameterInfoList[0].text,
                params: [],
            };

            for (var i: number = 1, paramInfo: FieldParameter; paramInfo = this.parameterInfoList[i]; i++) {
                data.params.push({
                    intervalStart: paramInfo.interval.start,
                    intervalEnd: paramInfo.interval.end(),
                    pureText: paramInfo.text
                });
            }
            return data;
        }

        protected applyResponse(response: any): boolean {
            var fieldResultInterval: FixedInterval = this.getTopField().getResultInterval();
            this.setInputPositionState();
            var simpleText: string = response["text"];
            if (simpleText !== undefined) { // simple text
                if (simpleText !== null && simpleText != "")
                    this.control.modelManipulator.insertText(this.control, fieldResultInterval, simpleText, false);
                return true;
            }
            else { // subDocument
                var newDocumentModel: DocumentModel = new DocumentModel(this.control.model.options, 0);
                newDocumentModel.cache.fontInfoCache = this.control.model.cache.fontInfoCache;
                ServerDispatcherResponseProcessor.processNewDocumentResponse(newDocumentModel, response["document"]);
                var imageCorrespondence = response["imageCorrespondence"];

                JSONImporter.importFixedLengthText(newDocumentModel.mainSubDocument, response["document"]["mainSubDocument"]["fixedLengthFormattedText"], (run: TextRun) => {
                    if (run.type == TextRunType.InlinePictureRun)
                        (<InlinePictureRun>run).id = imageCorrespondence[(<InlinePictureRun>run).id];
                });
                ModelManipulator.insertSubDocument(this.control, this.subDocument, this.getTopField().getResultStartPosition(), newDocumentModel.activeSubDocument, new FixedInterval(0, response["docLength"]));
                return true;
            }
            return false;
        }

    }
}