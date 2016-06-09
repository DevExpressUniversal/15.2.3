module __aspxRichEdit {
    var waitingTimer = 10000;
    var pendingTimer = 5000;

    export class ServerDispatcher implements ITextBoxIteratorRequestsListener {
        control: IRichEditControl;
        waitingTimerID: number = -1;
        pendingTimerID: number = -1;
        lastSavedHistoryItemId: number = -1;
        wasModifiedOnServer: boolean = false;
        saveRequestsCounter = 0;
        isWaiting: boolean = false;
        lockQueue: boolean = false;
        private testMode: boolean;
        dontSyncChangesWithServer: boolean = false;
        private updateFieldID: number = 0; // every update one field get difference id

        queue: { [key: number]: QueueItem } = {};

        requestsCounter: number = 0;
        editRequestsCounter: number = 0;

        lastRequestInQueue: ClientServerCommandRequest;

        responseProcessor: ServerDispatcherResponseProcessor; 

        constructor(control: IRichEditControl) {
            this.control = control;
            this.responseProcessor = new ServerDispatcherResponseProcessor(this);
        }

        initialize(testMode: boolean) {
            this.testMode = testMode;
        }
        //ITextBoxIteratorRequestsListener
        NotifyNextChunkRequired(subDocument: SubDocument, chunkIndex: number) {
            var prevChunk = this.control.model.activeSubDocument.chunks[chunkIndex - 1];
            var startPosition = prevChunk.startLogPosition.value + prevChunk.textBuffer.length;
            this.pushRequest(CommandType.FixedLengthText, {
                "start": startPosition,
                "sdid": subDocument.id
            }, undefined, RequestParams.immediate());
        }

        pushRequest(commandType: CommandType, serverParams: { [key: string]: any; } = {}, clientParams?: { [key: string]: any; }, requestParams: RequestParams = new RequestParams()): ClientServerCommandRequest {
            var request = new ClientServerCommandRequest(commandType, this.getNextRequestId(), serverParams, clientParams);
            return this.pushRequestCore(request, requestParams);
        }
        pushInsertTextRequest(subDocument: SubDocument, position: number, length: number, characterPropertiesJSON: any, characterStyle: CharacterStyle, type: TextRunType, text: string) {
            var request = new ClientServerTextCommandRequest(CommandType.InsertSimpleRun, this.getNextRequestId(), {
                position: position,
                length: length,
                type: type,
                characterProperties: characterPropertiesJSON,
                characterStyleName: characterStyle.styleName,
                sdid: subDocument.id
            }, text);
            this.pushRequestCore(request);
        }
        getRequestJSON(): string { // Used by RichEdit.js
			this.lastRequestInQueue = undefined;
            var request = this.getRequestList(true);
            if(request.length && !this.lockQueue)
                return JSON.stringify(request);
            return "";
        }
        reset() {
            this.clearTimers();
            this.saveRequestsCounter = 0;
            this.isWaiting = false;
            this.lockQueue = false;
            this.queue = {};
        }
        forceSendingRequest() {
            this.clearTimers();
            this.sendRequestCore();
        }
        hasQueue(): boolean {
            return !!this.queue[this.requestsCounter];
        }
        saveInProgress(): boolean {
            return this.saveRequestsCounter > 0;
        }
        processSaveResponse(queueItem: QueueItem) {
            this.decSaveCoun();
            this.lastSavedHistoryItemId = queueItem.request.clientParams["historyId"];
            this.wasModifiedOnServer = false;
            this.control.bars.updateItemsState();
        }
        onGetResponse(obj: any) {
            if(this.waitingTimerID > -1)
                clearTimeout(this.waitingTimerID);
            this.isWaiting = false;
            if(obj["errorCode"]) {
                if(obj["results"])
                    this.onGetCommandsResponse(obj["results"]);
                this.reset();
                switch(obj.errorCode) {
                    case ResponseError.ModelIsChanged:
                        this.control.commandManager.getCommand(RichEditClientCommand.ShowErrorModelIsChangedMessageCommand).execute();
                        break;
                    case ResponseError.AuthException:
                        this.control.commandManager.getCommand(RichEditClientCommand.ShowErrorAuthExceptionMessageCommand).execute();
                        break;
                    case ResponseError.InnerException:
                        this.control.commandManager.getCommand(RichEditClientCommand.ShowErrorInnerExceptionMessageCommand).execute();
                        break;
                    case ResponseError.CantSaveToAlreadyOpenedFile:
                        this.control.commandManager.getCommand(RichEditClientCommand.ShowErrorOpeningAndOverstoreImpossibleMessageCommand).execute();
                        break;
                    case ResponseError.CantSaveDocument:
                        this.control.commandManager.getCommand(RichEditClientCommand.ShowErrorSavingMessageCommand).execute();
                        break;
                    case ResponseError.CantOpenDocument:
                        this.control.commandManager.getCommand(RichEditClientCommand.ShowErrorOpeningMessageCommand).execute();
                        break;
                    case ResponseError.PathTooLongException:
                        this.control.commandManager.getCommand(RichEditClientCommand.ShowErrorPathTooLongCommand).execute();
                        break;
                    case ResponseError.CalculateDocumentVariableException:
                        this.control.commandManager.getCommand(RichEditClientCommand.ShowErrorDocVariableErrorCommand).execute();
                        if(this.control.model.activeSubDocument.fieldsWaitingForUpdate)
                            this.control.model.activeSubDocument.fieldsWaitingForUpdate.endAction();
                        break;
                    default:
                        throw "Undefined server error";
                        break;
                }
            }
            else {
                this.onGetCommandsResponse(obj.commands);
                if(!this.isWaiting) {
                    if(!this.sendRequestCore())
                        this.lockQueue = false;
                }
            }
        }
        getUpdateFieldID(): number {
            return this.updateFieldID++;
        }
        canExtendPreviousTextInsertedRequest(subDocument: SubDocument, position: number, characterPropertiesJSON: any, characterStyleName: string, type: TextRunType): boolean {
            var previousRequest = this.lastRequestInQueue;
            if(!previousRequest || !(previousRequest instanceof ClientServerTextCommandRequest) || previousRequest.type !== CommandType.InsertSimpleRun)
                return false;
            if(previousRequest.serverParams["position"] + previousRequest.serverParams["length"] !== position)
                return false;
            if(previousRequest.serverParams["type"] !== type)
                return false;
            if(JSON.stringify(previousRequest.serverParams["characterProperties"]) !== JSON.stringify(characterPropertiesJSON))
                return false;
            if(characterStyleName !== previousRequest.serverParams["characterStyleName"])
                return false;
            return true;
        }
        private clearTimers() {
            if(this.waitingTimerID > 0) {
                clearTimeout(this.waitingTimerID);
                this.waitingTimerID = -1;
            }
            if(this.pendingTimerID > 0) {
                clearTimeout(this.pendingTimerID);
                this.pendingTimerID = -1;
            }
        }
        private getNextRequestId(): number {
            this.requestsCounter++;
            return this.requestsCounter;
        }
        private pushRequestCore(request: ClientServerCommandRequest, requestParams: RequestParams = new RequestParams()): ClientServerCommandRequest {
            if(request.type < 0)
                this.control.raiseDocumentChanged();
            if(request.type < 0 && this.dontSyncChangesWithServer)
                return;
            if(request.type < 0)
                request.editIncId = ++this.editRequestsCounter;
            else
                request.fiIndex = this.control.model.cache.fontInfoCache.length;
            this.lastRequestInQueue = request;
            var requests = [request];
            if(request.type < 0 && !this.control.model.isLoaded()) {
                var fixedLengthTextRequest = new ClientServerCommandRequest(CommandType.FixedLengthText, this.getNextRequestId(), { "start": this.control.model.getCurrentLength() });
                fixedLengthTextRequest.fiIndex = this.control.model.cache.fontInfoCache.length;
                requestParams.immediateSend = true;
            }
            this.sendRequest(requests, requestParams);
            return request;
        }
        private sendRequest(requests: ClientServerCommandRequest[], requestParams: RequestParams = new RequestParams()) {
            if(this.lockQueue)
                return;
            if(this.testMode)
                this.queue = {};
            this.lockQueue = !!requestParams.lockQueue;
            this.placeRequestsInQueue(requests, requestParams.processOnCallback);
            this.prepareSendRequests(requestParams);
        }
        private placeRequestsInQueue(requests: ClientServerCommandRequest[], processOnCallback: boolean) {
            for(var i = 0, request: ClientServerCommandRequest; request = requests[i]; i++) {
                if(request.type < 0)
                    this.removeModelRequests();
                if(request.type === CommandType.SaveDocument || request.type === CommandType.SaveAsDocument)
                    this.incSaveCount();
                this.queue[request.incId] = new QueueItem(request, processOnCallback);
            }
        }
        private incSaveCount() {
            this.saveRequestsCounter++;
        }
        private decSaveCoun() {
            this.saveRequestsCounter--;
            if (this.saveRequestsCounter < 0)
                throw new Error(Errors.InternalException);
        }
        private prepareSendRequests(requestParams: RequestParams) {
            if(this.testMode)
                this.sendRequestCore();
            else if(requestParams.immediateSend)
                this.forceSendingRequest();
            else if(!this.isWaiting && this.pendingTimerID < 0)
                this.pendingTimerID = setTimeout(() => this.onPendingTimerExpired(), pendingTimer);
        }
        private sendRequestCore(): boolean {
            if (this.hasQueue()) {
                this.isWaiting = true;
                this.lastRequestInQueue = undefined;
                var sendViaCallback = this.shouldSendRequestsWithCallback();
                var request = this.getRequestList(sendViaCallback);
                this.control.sendRequest(JSON.stringify(request), sendViaCallback);
                if(!this.lockQueue)
                    this.waitingTimerID = setTimeout(() => this.onWaitingTimerExpired(), waitingTimer);
                return true;
            }
            return false;
        }
        private shouldSendRequestsWithCallback(): boolean {
            for (var key in this.queue) {
                if (this.queue.hasOwnProperty(key)) {
                    if (this.queue[key].callbackRequired)
                        return true;
                }
            }
            return false;
        }
        private getRequestList(withPostData: boolean): any[] {
            var request = [];
            for (var key in this.queue) {
                if(this.queue.hasOwnProperty(key))
                    request.push(this.queue[key].request.getJsonObject(withPostData));
            }
            return request;
        }
        private removeModelRequests() {
            var keysForDeleting: number[] = [];
            for (var key in this.queue) {
                if (!this.queue[key].isModelChanging())
                    keysForDeleting.push(key);
            }
            for (var i = keysForDeleting.length - 1; i >= 0; i--)
                delete this.queue[keysForDeleting[i]];
        }
        private onGetCommandsResponse(commands: any[]) {
            for(var i = 0, response: any; response = commands[i]; i++) {
                if(response.incId > 0 && !this.queue.hasOwnProperty(response["incId"]))
                    continue;
                var queueItem = this.queue[response.incId];
                delete this.queue[response.incId];
                this.responseProcessor.process(response, queueItem);
            }
        }
        private onPendingTimerExpired() {
            this.sendRequestCore();
            this.pendingTimerID = -1;
        }
        private onWaitingTimerExpired() {
            this.sendRequestCore();
            this.waitingTimerID = -1;
        }
        static prepareTextForRequest(text: string): string {
            return text
                .replace(/%/g, '%25')
                .replace(/&/g, '%26amp;')
                .replace(/</g, '%26lt;')
                .replace(/>/g, '%26gt;')
                .replace(/"/g, '%26quot;');
        }
    }

    export class QueueItem {
        constructor(request: ClientServerCommandRequest, callbackRequired: boolean) {
            this.request = request;
            this.callbackRequired = callbackRequired;
        }
        request: ClientServerCommandRequest;
        callbackRequired: boolean;
        isModelChanging(): boolean {
            return this.request.type < 0;
        }
    }

    export class ClientServerCommandRequest {
        type: CommandType;
        incId: number;
        editIncId: number;
        fiIndex: number;
        serverParams: { [key: string]: any; } = {};
        clientParams: { [key: string]: any; } = {};

        constructor(type: CommandType, incId: number, serverParams: { [key: string]: any; }, clientParams: { [key: string]: any;} = undefined ) {
            this.type = type;
            this.incId = incId;
            this.serverParams = serverParams;
            this.clientParams = clientParams;
        }

        getJsonObject(withPostData: boolean): any {
            var obj = {
                type: this.type,
                incId: this.incId,
                editIncId: this.editIncId,
                fiIndex: this.fiIndex,
                serverParams: this.serverParams
            };
            if (this.clientParams)
                obj["clientParams"] = this.clientParams;
            return obj;
        }
    }
    class ClientServerTextCommandRequest extends ClientServerCommandRequest {
        constructor(type: CommandType, incId: number, serverParams: { [key: string]: any; }, text: string) {
            super(type, incId, serverParams, undefined);
            this.text = text;
        }

        text: string;
        getJsonObject(withPostData: boolean): any {
            var obj = super.getJsonObject(withPostData);
            obj.serverParams["text"] = !withPostData ? ServerDispatcher.prepareTextForRequest(this.text) : this.text;
            return obj;
        }
    }

    export class RequestParams {
        lockQueue: boolean = false;
        immediateSend: boolean = false;
        processOnCallback = false;

        static immediate(): RequestParams {
            var params = new RequestParams();
            params.immediateSend = true;
            return params;
        }
    }
} 