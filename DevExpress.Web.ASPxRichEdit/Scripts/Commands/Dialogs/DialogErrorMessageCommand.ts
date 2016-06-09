module __aspxRichEdit {
    export class DialogErrorMessageCommand extends ShowDialogCommandBase {
        createParameters(): ErrorMessageDialogParameters {
            var parameters = new ErrorMessageDialogParameters();
            parameters.errorTextId = this.getErrorTextId();
            return parameters;
        }
        getDialogName() {
            return "ErrorMessage";
        }
        getErrorTextId(): ErrorMessageText {
            throw new Error(Errors.NotImplemented);
        }
        isEnabledInReadOnlyMode(): boolean {
            return true;
        }
    }

    export class ShowErrorModelIsChangedMessageCommand extends DialogErrorMessageCommand {
        getErrorTextId(): ErrorMessageText {
            return ErrorMessageText.ModelIsChanged;
        }
        afterClosing() {
            this.control.commandManager.getCommand(RichEditClientCommand.ReloadDocument).execute();
        }
    }

    export class ShowErrorSessionHasExpiredMessageCommand extends DialogErrorMessageCommand {
        getErrorTextId(): ErrorMessageText {
            return ErrorMessageText.SessionHasExpired;
        }
    }

    export class ShowErrorOpeningAndOverstoreImpossibleMessageCommand extends DialogErrorMessageCommand {
        getErrorTextId(): ErrorMessageText {
            return ErrorMessageText.OpeningAndOverstoreImpossible;
        }
    }

    export class ShowErrorClipboardAccessDeniedMessageCommand extends DialogErrorMessageCommand {
        getErrorTextId(): ErrorMessageText {
            return ErrorMessageText.ClipboardAccessDenied;
        }
    }

    export class ShowErrorInnerExceptionMessageCommand extends DialogErrorMessageCommand {
        getErrorTextId(): ErrorMessageText {
            return ErrorMessageText.InnerException;
        }
        afterClosing() {
            this.control.commandManager.getCommand(RichEditClientCommand.ReloadDocument).execute();
        }
    }

    export class ShowErrorAuthExceptionMessageCommand extends DialogErrorMessageCommand {
        getErrorTextId(): ErrorMessageText {
            return ErrorMessageText.AuthException;
        }
        afterClosing() {
            this.control.commandManager.getCommand(RichEditClientCommand.ReloadDocument).execute();
        }
    }

    export class ShowErrorCantOpenDocument extends DialogErrorMessageCommand {
        getErrorTextId(): ErrorMessageText {
            return ErrorMessageText.CantOpenFile;
        }
    }

    export class ShowErrorCantSaveDocument extends DialogErrorMessageCommand {
        getErrorTextId(): ErrorMessageText {
            return ErrorMessageText.CantSaveFile;
        }
    }

    export class ShowErrorPathTooLong extends DialogErrorMessageCommand {
        getErrorTextId(): ErrorMessageText {
            return ErrorMessageText.PathTooLongException;
        }
    }

    export class ShowErrorDocVariableExceptionCommand extends DialogErrorMessageCommand {
        getErrorTextId(): ErrorMessageText {
            return ErrorMessageText.DocVariableException;
        }
    }

    export class ErrorMessageDialogParameters extends DialogParametersBase {
        errorTextId: ErrorMessageText;

        copyFrom(obj: ErrorMessageDialogParameters) {
            this.errorTextId = obj.errorTextId;
        }

        getNewInstance(): DialogParametersBase {
            return new ErrorMessageDialogParameters();
        }

        toAnotherMeasuringSystem(converterFunc: (val: any) => any) {
        }
    }


    export enum ErrorMessageText {
        ModelIsChanged = 0,
        SessionHasExpired = 1,
        OpeningAndOverstoreImpossible = 2,
        ClipboardAccessDenied = 3,
        InnerException = 4,
        AuthException = 5,
        CantOpenFile = 6,
        CantSaveFile = 7,
        DocVariableException = 8,
        PathTooLongException = 9
    }
}