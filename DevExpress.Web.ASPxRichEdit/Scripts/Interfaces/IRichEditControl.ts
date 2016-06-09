module __aspxRichEdit {
    export interface IRichEditControl extends IFormatterController, IBatchUpdatableObject, IDisposable {
        history: IHistory;
        model: DocumentModel;
        modelManipulator: ModelManipulator;
        commandManager: CommandManager;
        selection: Selection;
        serverDispatcher: ServerDispatcher;
        options: ControlOptions;
        mailMergeOptions: MailMergeOptions;
        units: UnitConverter;

        inputPosition: InputPosition;
        bars: BarManager;
        horizontalRulerControl: IRulerControl;

        layout: DocumentLayout;

        hitTestManager: HitTestManager;

        measurer: IBoxMeasurer;

        readOnly: ReadOnlyMode;

        stringResources: StringResources;

        initialize(sessionGuid: string, fileName: string, emptyImageCacheId: number, subDocumentsCounter: number);
        afterInitialize();

        sendRequest(requestQueryString: string, viaInternalCallback: boolean);

        isResizeBoxVisible(): boolean;
        setInputTargetPosition(x: number, y: number);
        clearKeyboardTips(): void;
        captureFocus();
        canCaptureFocus(): boolean;

        showDialog(name: string, parameters: any, callback: (params: any) => void, afterClosing: () => void, isModal: boolean);

        closeDocument();
        updateDocumentLayout();
        sendDownloadRequest(downloadRequestType: DownloadRequestType, parameters?: any);

        importHtml(elements: HTMLElement[], interval: FixedInterval, modelPartCopyPasted: boolean);

        adjust();

        toggleFullScreenMode();
        isFullScreenMode(): boolean;

        getEditableDocument();
        getDocumentRenderer(): DocumentRenderer;
        renderSelectionToEditableDocument();
        selectEditableDocumentContent();
        setEditableDocumentContent(content: string);
        getEditableDocumentContent(): string;

        setCursorPointer(pointer: CursorPointer);

        setTouchBarsVisibile(visible: boolean): void;

        setWorkSession(sessionGuid: string, fileName: string, lastExecutedCommandId: number);

        getModifiedState(): IsModified;
        confirmOnLosingChanges(): boolean;
        getFileName(): string;
        hasWorkDirectory(): boolean;
        beginLoading();
        endLoading();

        waitForDblClick();

        raiseDocumentChanged();
        showPopupMenu(evtX: number, evtY: number);
        showLoupe(evt: RichMouseEvent);
        moveLoupe(evt: RichMouseEvent);
        hideLoupe();
        hidePopupMenu();
        isTouchMode(): boolean;


        getViewElement(): HTMLDivElement;
        getCanvasSizeInfo(): CanvasSizeInfo;
    }
    export enum ReadOnlyMode {
        None = 0,
        Persistent = 1,
        Temporary = 2
    }
}