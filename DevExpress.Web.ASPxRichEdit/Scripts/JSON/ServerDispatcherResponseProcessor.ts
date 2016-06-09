module __aspxRichEdit {
    export class ServerDispatcherResponseProcessor {
        dispatcher: ServerDispatcher;
        control: IRichEditControl;
        constructor(dispatcher: ServerDispatcher) {
            this.dispatcher = dispatcher;
            this.control = dispatcher.control;
        }

        process(obj: any, queueItem: QueueItem) {
            if(obj["isNewWorkSession"])
                this.processNewWorkSessionResponse(obj);
            if(obj["fontInfoCache"])
                this.control.model.cache.fontInfoCache.merge(obj["fontInfoCache"], JSONFontInfoConverter.convertFromJSON);
            switch(obj["type"]) {
                case CommandType.Start:
                    JSONImporter.importOptions(this.control.model.options, obj["options"]);
                    JSONImporter.importStringResources(this.control.stringResources, obj["stringResources"]);
                    break;
                case CommandType.FixedLengthText:
                    this.processFixedLengthTextResponse(this.control.model, obj, false);
                    break;
                case CommandType.SaveDocument:
                    this.dispatcher.processSaveResponse(queueItem);
                    break;
                case CommandType.SaveAsDocument:
                    this.dispatcher.processSaveResponse(queueItem);
                    break;
                case CommandType.DelayedPrint:
                    this.control.sendDownloadRequest(DownloadRequestType.PrintCurrentDocument);
                    break;
                case CommandType.LoadInlinePictures:
                    this.control.modelManipulator.text.applyLoadedImages(this.control.model.activeSubDocument, obj["loadedImagesInfo"]);
                    break;
                case CommandType.FieldUpdate:
                    this.control.modelManipulator.fieldsManipulator.continueUpdateFields(this.control, obj);
                    break;
            }
            if(obj["isNewDocument"])
                ServerDispatcherResponseProcessor.processNewDocumentResponse(this.control.model, obj["document"]);
            if(obj["subDocuments"])
                JSONImporter.importSubDocuments(this.control.model, obj["subDocuments"]);
            if(obj["isNewDocument"]) {
                JSONImporter.finishImportSections(this.control.model, obj["document"]["sections"]);
                this.processFixedLengthTextResponse(this.control.model, obj["document"]["mainSubDocument"]["fixedLengthFormattedText"], true);
            }
        }

        processNewWorkSessionResponse(obj: any) {
            this.dispatcher.editRequestsCounter = obj["lastExecutedEditCommandId"];
            if(obj["isNewDocument"])
                this.control.initialize(obj["sessionGuid"], obj["fileName"], obj["emptyImageCacheID"], obj["subDocumentsCounter"]);
            else
                this.control.setWorkSession(obj["sessionGuid"], obj["fileName"], obj["lastExecutedCommandId"]);
            this.dispatcher.wasModifiedOnServer = obj["isModified"];
        }

        static processNewDocumentResponse(documentModel: DocumentModel, obj: any) {
            JSONImporter.importStyles(documentModel, obj["styles"]);
            JSONImporter.importDocumentProperties(documentModel, obj["documentProperties"]);
            JSONImporter.importHeadersFooters(documentModel, obj["headers"], obj["footers"]);
            JSONImporter.importSections(documentModel, obj["sections"]);
            JSONImporter.importNumberingLists(documentModel, obj["lists"]);
            JSONImporter.importSubDocumentProperties(documentModel.mainSubDocument, obj["mainSubDocument"]);
        }

        processFixedLengthTextResponse(documentModel: DocumentModel, obj: any, isNewDocument: boolean) {
            JSONImporter.importFixedLengthText(documentModel.mainSubDocument, obj);
            if(isNewDocument) {
                this.control.updateDocumentLayout();
                this.control.runFormatting(0); // this.control.layout.validPageCount??
                this.control.bars.setEnabled(true);
                this.control.horizontalRulerControl.setEnable(true);
                this.control.selection.setSelection(0, 0, false, -1, UpdateInputPositionProperties.Yes);
                this.control.inputPosition.reset();
                this.control.selection.raiseSelectionChanged();
            }
            this.control.runFormattingAsync();
            if(!this.control.model.isLoaded())
                this.dispatcher.pushRequest(CommandType.FixedLengthText, { "start": this.control.model.getCurrentLength() }, undefined, RequestParams.immediate());
        }
    }
}