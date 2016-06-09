module __aspxRichEdit {
    export class DialogSaveFileCommand extends ShowDialogCommandBase {
        getState(): ICommandState {
            var state = new SimpleCommandState(this.isEnabled());
            state.visible = this.control.options.saveAs !== DocumentCapability.Hidden;
            return state;
        }
        createParameters(parameters?: any): SaveFileDialogParameters {
            var saveFileDialogParameters = new SaveFileDialogParameters();
            saveFileDialogParameters.fileName = this.getFileName();
            saveFileDialogParameters.fileExtension = this.getFileExtension();
            saveFileDialogParameters.folderPath = "";
            saveFileDialogParameters.fileSavedToServer = true;
            return saveFileDialogParameters;
        }
        executeCore(state: ICommandState, parameter?: any): boolean {
            if(this.control.hasWorkDirectory())
                return super.executeCore(state, parameter);
            else
                return this.executeShowErrorMessageCommand();
        }
        applyParameters(state: IntervalCommandState, params: SaveFileDialogParameters) {
            if(params.fileSavedToServer) {
                var requestParams = new RequestParams();
                requestParams.lockQueue = true;
                requestParams.immediateSend = true;
                requestParams.processOnCallback = true;
                this.control.serverDispatcher.pushRequest(CommandType.SaveAsDocument,
                    {
                        "fileName": params.fileName,
                        "folderPath": params.folderPath,
                        "fileExtension": params.fileExtension,
                    },
                    {
                        "historyId": this.control.history.getCurrentItemId()
                    },
                    requestParams);
            } else
                this.control.sendDownloadRequest(DownloadRequestType.DownloadCurrentDocument, {
                    "fileExtension": params.fileExtension
                });
        }
        getDialogName() {
            return "FileSaveAs";
        }
        getFileName(): string {
            var fileNameWithExtension = this.control.getFileName();
            var fileName = fileNameWithExtension.substr(0, fileNameWithExtension.indexOf("."));
            return fileName !== "" ? fileName : "document1";
        }
        getFileExtension(): string {
            var fileNameWithExtension = this.control.getFileName();
            var fileExtension = fileNameWithExtension.substr(fileNameWithExtension.indexOf("."));
            return fileExtension !== "" ? fileExtension : ".docx";
        }
        executeShowErrorMessageCommand(): boolean {
            return this.control.commandManager.getCommand(RichEditClientCommand.ShowErrorOpeningAndOverstoreImpossibleMessageCommand).execute();
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.saveAs);
        }
    }

    export class SaveFileDialogParameters extends DialogParametersBase {
        fileName: string = "";
        folderPath: string = "";
        fileExtension: string = "";
        fileSavedToServer: boolean = true;

        getNewInstance(): DialogParametersBase {
            return new SaveFileDialogParameters();
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
        }

        copyFrom(obj: SaveFileDialogParameters) {
            this.fileName = obj.fileName;
            this.folderPath = obj.folderPath;
            this.fileExtension = obj.fileExtension;
            this.fileSavedToServer = obj.fileSavedToServer;
        }
    }

    export class DialogSaveMergedDocumentCommand extends DialogSaveFileCommand {
        createParameters(parameters: FinishAndMergeDialogParameters): SaveMergedDocumentDialogParameters {
            var saveFileDialogParameters = new SaveMergedDocumentDialogParameters();
            saveFileDialogParameters.fileName = "result";
            saveFileDialogParameters.fileExtension = ".docx";
            saveFileDialogParameters.folderPath = "";
            saveFileDialogParameters.fileSavedToServer = true;

            saveFileDialogParameters.mergeMode = parameters.mergeMode;
            switch(parameters.range) {
                case MailMergeExportRange.AllRecord:
                    saveFileDialogParameters.firstRecordIndex = 0;
                    saveFileDialogParameters.lastRecordIndex = this.control.mailMergeOptions.recordCount - 1;
                    break;
                case MailMergeExportRange.CurrentRecord:
                    saveFileDialogParameters.firstRecordIndex = this.control.mailMergeOptions.activeRecordIndex;
                    saveFileDialogParameters.lastRecordIndex = this.control.mailMergeOptions.activeRecordIndex;
                    break;
                case MailMergeExportRange.Range:
                    saveFileDialogParameters.firstRecordIndex = parameters.exportFrom - 1;
                    saveFileDialogParameters.lastRecordIndex = parameters.exportFrom + parameters.exportRecordsCount - 2;
                    break;
            }
            return saveFileDialogParameters;
        }
        applyParameters(state: IntervalCommandState, params: SaveMergedDocumentDialogParameters) {
            if(params.fileSavedToServer) {
                var requestParams = new RequestParams();
                requestParams.lockQueue = true;
                requestParams.immediateSend = true;
                requestParams.processOnCallback = true;
                this.control.serverDispatcher.pushRequest(CommandType.SaveMergedDocument,
                    {
                        "fileName": params.fileName,
                        "folderPath": params.folderPath,
                        "fileExtension": params.fileExtension,
                        "firstRecordIndex": params.firstRecordIndex,
                        "lastRecordIndex": params.lastRecordIndex,
                        "mergeMode": params.mergeMode
                    },
                    {
                        "historyId": this.control.history.getCurrentItemId()
                    },
                    requestParams);
            } else
                this.control.sendDownloadRequest(DownloadRequestType.DownloadMergedDocument, {
                    "fileExtension": params.fileExtension,
                    "firstRecordIndex": params.firstRecordIndex,
                    "lastRecordIndex": params.lastRecordIndex,
                    "mergeMode": params.mergeMode
                });
        }
    }

    export class SaveMergedDocumentDialogParameters extends SaveFileDialogParameters {
        firstRecordIndex: number;
        lastRecordIndex: number;
        mergeMode: MergeMode;

        getNewInstance(): DialogParametersBase {
            return new SaveMergedDocumentDialogParameters();
        }

        copyFrom(obj: SaveMergedDocumentDialogParameters) {
            this.fileName = obj.fileName;
            this.folderPath = obj.folderPath;
            this.fileExtension = obj.fileExtension;
            this.fileSavedToServer = obj.fileSavedToServer;

            this.firstRecordIndex = obj.firstRecordIndex;
            this.lastRecordIndex = obj.lastRecordIndex;
            this.mergeMode = obj.mergeMode;
        }
    }
}