module __aspxRichEdit {
    export class RichEditCore implements IRichEditControl, IProcessManager {
        history: IHistory;
        model: DocumentModel;
        modelManipulator: ModelManipulator;
        commandManager: CommandManager;
        selection: Selection;
        serverDispatcher: ServerDispatcher;
        inputPosition: InputPosition; 
        bars: BarManager;
        layout: DocumentLayout;
        hitTestManager: HitTestManager;
        options: ControlOptions;
        mailMergeOptions: MailMergeOptions;
        measurer: IBoxMeasurer;
        units: UnitConverter;
        horizontalRulerControl: IRulerControl;
        readOnly: ReadOnlyMode = ReadOnlyMode.None;
        stringResources: StringResources;

        private layoutFormatterInvalidator: LayoutFormatterInvalidator;
        private layoutFormatterMain: LayoutFormatterMain;

        private sessionGuid: string;
        private fileName: string;
        private emptyImageCacheId: number;
        private clientGuid: string;
        private owner: IControlOwner;
        private formatterProcessID: number;
        private eventManager: EventManager;
        private canvasManager: CanvasManager;
        private inputController: InputController;
        private selectionFormatter: SelectionFormatter;
        private bookmarksFormatter: BookmarksFormatter;
        private loupe: Loupe;
        private bookmarksSettings: any;
        
        private resizeBoxVisualizer: ResizeBoxVisualizer;

        constructor(owner: IControlOwner, clientGuid: string, readOnly: ReadOnlyMode, elements: { mainElement: HTMLElement; viewElement: HTMLDivElement; measurerContainer: HTMLElement }, unitsType: RichEditUnit, rulerSettings: any, bookmarksSettings: any, mailMergeOptions: MailMergeOptions, bars: { bars: IBar[] }) {
            this.stringResources = new StringResources();
            this.owner = owner;
            this.clientGuid = clientGuid;
            this.readOnly = readOnly;
            this.options = new ControlOptions();
            this.mailMergeOptions = mailMergeOptions;
            this.bookmarksSettings = bookmarksSettings;

            this.measurer = new Measurer(elements.measurerContainer);
            var dragCaretVisualizer: DragCaretVisualizer = new DragCaretVisualizer(this);
            this.resizeBoxVisualizer = new ResizeBoxVisualizer(this);
            this.eventManager = new EventManager(this, dragCaretVisualizer, this.resizeBoxVisualizer);

            this.units = new UnitConverter(unitsType);
            this.horizontalRulerControl = new Ruler.HorizontalRulerControl(this, rulerSettings, elements.mainElement, elements.viewElement);
            this.canvasManager = new CanvasManager(null, elements.viewElement, this, this.eventManager, this.stringResources, this.horizontalRulerControl);
            this.inputController = new InputController(this, this.eventManager, elements.mainElement);

            if (this.isTouchMode())
                this.loupe = new Loupe(elements.mainElement);

            this.commandManager = new CommandManager(this);

            this.bars = new BarManager(this, bars.bars);
            this.serverDispatcher = new ServerDispatcher(this);
            dragCaretVisualizer.onChanged.add(this.canvasManager);
            this.resizeBoxVisualizer.onChanged.add(this.canvasManager);
        }

        initialize(sessionGuid: string, fileName: string, emptyImageCacheId: number, subDocumentsCounter: number, testMode: boolean = false, documentModel?: DocumentModel) {
            if(!documentModel)
                documentModel = new DocumentModel(this.options, subDocumentsCounter);
            this.setWorkSession(sessionGuid, fileName, emptyImageCacheId);

            this.model = documentModel;
            this.layout = new DocumentLayout();
            this.canvasManager.layout = this.layout;
            this.selection = new Selection(this.model, this.layout);

            this.hitTestManager = new HitTestManager(this.layout, this.measurer);
            this.serverDispatcher.initialize(testMode);

            const iterator: TextBoxIterator = new TextBoxIterator(this.measurer, this.model.activeSubDocument);
            iterator.onNextChunkRequested.add(this.serverDispatcher);
            
            // formatters
            this.layoutFormatterMain = new LayoutFormatterMain(this.layout, iterator, new TwipsUnitConverter());

            this.bookmarksFormatter = new BookmarksFormatter(this.layout, this.measurer, this.bookmarksSettings);
            this.layoutFormatterMain.addLayoutChangedListener(this.bookmarksFormatter);

            this.layoutFormatterMain.addLayoutChangedListener(this.canvasManager);
            this.layoutFormatterInvalidator = new LayoutFormatterInvalidator(this.layout, this.model, this.selection, this.layoutFormatterMain);

            this.selectionFormatter = new SelectionFormatter(this.selection, this.layout, this.measurer, this);
            this.selectionFormatter.onSelectionLayoutChanged.add(this.canvasManager);
            this.selection.onChanged.add(this.selectionFormatter);
            this.layoutFormatterMain.addLayoutChangedListener(this.selectionFormatter);
            this.layoutFormatterMain.addLayoutChangedListener(this.resizeBoxVisualizer);


            this.modelManipulator = new ModelManipulator(this.model);
            this.history = new History(this.modelManipulator, this.options);

            this.modelManipulator.dispatcher.onModelChanged.add(new LayoutFormatterModelChangesListener(this.layoutFormatterInvalidator));
            this.modelManipulator.dispatcher.onModelChanged.add(new ServerDispatcherModelChangesListener(this.serverDispatcher));

            this.inputPosition = new InputPosition(this.selection, this.modelManipulator.model);
            this.selection.inputPosition = this.inputPosition;
            this.selection.onChanged.add(this.bars);
            this.selection.onChanged.add(this.resizeBoxVisualizer);
            this.bars.setEnabled(false);

            this.horizontalRulerControl.initialize(testMode);
            this.horizontalRulerControl.setEnable(false);
            this.selection.onChanged.add(this.horizontalRulerControl);
        }

        afterInitialize() {
            if(this.mailMergeOptions.isEnabled && this.mailMergeOptions.viewMergedData)
                this.commandManager.getCommand(RichEditClientCommand.UpdateAllFields).execute();
        }

        setWorkSession(sessionGuid: string, fileName: string, emptyImageCacheId: number) {
            this.sessionGuid = sessionGuid;
            this.fileName = fileName;
            this.canvasManager.setParameters(this.getHandlerUri(), emptyImageCacheId);
            this.emptyImageCacheId = emptyImageCacheId;
            if(this.owner)
                this.owner.syncSessionGuid(sessionGuid);
        }

        sendRequest(requestQueryString: string, viaInternalCallback: boolean) {
            this.owner.raiseBeginSynchronization();
            this.owner.sendRequest(requestQueryString, viaInternalCallback);
        }
        beginUpdate() {
            this.layoutFormatterMain.beginUpdate();
            this.bars.beginUpdate();
            this.horizontalRulerControl.beginUpdate();
            this.selectionFormatter.beginUpdate();
            this.resizeBoxVisualizer.beginUpdate();
            this.canvasManager.beginUpdate();
        }
        endUpdate() {
            this.layoutFormatterMain.endUpdate();
            this.bars.endUpdate();
            this.horizontalRulerControl.endUpdate();
            this.selectionFormatter.endUpdate();
            this.resizeBoxVisualizer.endUpdate();
            this.canvasManager.endUpdate();
            this.runFormattingAsync();
        }

        formatterResetAllLayout() {
            this.layoutFormatterMain.restartFormatingAllLayout();
        }

        formatterOnIntervalChanged(interval: FixedInterval, subDocument: SubDocument) {
            this.layoutFormatterInvalidator.onIntervalChanged(subDocument, interval);
        }

        runFormattingAsync() {
            if(this.formatterProcessID || this.layoutFormatterMain.isUpdateLocked())
                return;
            var asyncCalculating = () => {
                if (this.layoutFormatterMain.formatNext())
                    this.formatterProcessID = setTimeout(asyncCalculating, 0);
                else
                    this.formatterProcessID = null;
            };
            this.formatterProcessID = setTimeout(asyncCalculating, 0);
        }
        runFormatting(pageIndex: number) {
            if(this.layoutFormatterMain.isUpdateLocked()) return;
            this.layoutFormatterMain.formatPage(pageIndex);
            this.runFormattingAsync();
        }
        formatSync() {
            this.layoutFormatterMain.suspendUpdate();
            while (this.layoutFormatterMain.formatNext()) { }
            this.layoutFormatterMain.continueUpdate();
        }
        stopFormatting() {
            if(this.formatterProcessID) {
                clearTimeout(this.formatterProcessID);
                this.formatterProcessID = null;
            }
        }
        forceFormatPage(pageIndex: number): LayoutPage {
            this.layoutFormatterMain.suspendUpdate();
            var page = this.layoutFormatterMain.formatPage(pageIndex);
            this.layoutFormatterMain.continueUpdate();
            return page;
        }
        forceFormatFromPage(pageIndex: number, modelPosition: number) {
            this.layoutFormatterMain.suspendUpdate();
            this.layoutFormatterMain.restartFromPage(pageIndex, modelPosition);
            this.layoutFormatterMain.continueUpdate();
        }

        setInputTargetPosition(x: number, y: number) {
            this.inputController.setPosition(x, y);
        }
        clearKeyboardTips(): void {
            this.inputController.clearKeyboardTips();
        }
        renderSelectionToEditableDocument() {
            this.inputController.renderSelectionToEditableDocument();
        }
        selectEditableDocumentContent() {
            this.inputController.selectEditableDocumentContent();
        }
        setEditableDocumentContent(content: string) {
            this.inputController.setEditableDocumentContent(content);
        }
        getEditableDocumentContent(): string {
            return this.inputController.getEditableDocumentContent();
        }
        canCaptureFocus(): boolean {
            return this.owner.canCaptureFocus();
        }
        captureFocus() {
            if(this.canCaptureFocus()) {
                if(!ASPx.Browser.MacOSMobilePlatform || this.owner.isInitialized)
                    this.inputController.captureFocus();
                this.eventManager.onFocusIn();
            }
        }
        isResizeBoxVisible(): boolean {
            return this.resizeBoxVisualizer.isResizeBoxVisible();
        }

        beginLoading() {
            if(this.readOnly === ReadOnlyMode.None) {
                this.readOnly = ReadOnlyMode.Temporary;
                this.bars.suspendUpdate();
                this.bars.updateItemsState();
                this.bars.continueUpdate();
                this.owner.setLoadingPanelVisible(true);
            }
        }
        endLoading() {
            if(this.readOnly === ReadOnlyMode.Temporary) {
                this.readOnly = ReadOnlyMode.None;
                this.bars.suspendUpdate();
                this.bars.updateItemsState();
                this.bars.continueUpdate();
                this.owner.setLoadingPanelVisible(false);
            }
        }

        adjust() {
            if (this.owner && this.owner.AdjustControlCore)
                this.owner.AdjustControlCore();
        }

        showDialog(name: string, parameters: any, callback: (params: any) => void, afterClosing: () => void, isModal: boolean) {
            this.owner.showDialog(name, parameters, callback, afterClosing, isModal);
        }

        closeDocument() {
            this.bars.setEnabled(false);
            if (this.horizontalRulerControl)
                this.horizontalRulerControl.setEnable(false);
            this.stopFormatting();
            this.canvasManager.closeDocument();
            this.serverDispatcher.reset();
        }
        updateDocumentLayout() {
            this.layout.pageColor = this.model.pageBackColor;
        }
        toggleFullScreenMode() {
            if(this.owner)
                this.owner.toggleFullScreenMode();
        }
        isFullScreenMode(): boolean {
            return this.owner && this.owner.isInFullScreenMode;
        }
        setCursorPointer(cursor: CursorPointer) {
            this.canvasManager.setCursorPointer(cursor);
        }
        sendDownloadRequest(downloadRequestType: DownloadRequestType, parameters?: any) {
            this.owner.sendDownloadRequest(downloadRequestType, JSON.stringify(parameters));
        }

        importHtml(elements: HTMLElement[], interval: FixedInterval, modelPartCopyPasted: boolean) {
            this.beginUpdate();
            var exportedRangeCopy = this.inputController.getExportedRangeCopy();
            if(modelPartCopyPasted && exportedRangeCopy)
                ModelManipulator.pasteRangeCopy(this, this.model.activeSubDocument, interval, exportedRangeCopy);
            else {
                var htmlImporter = new HtmlImporter(this, this.emptyImageCacheId);
                htmlImporter.elementsImport(elements, interval);
            }
            this.endUpdate();
        }

        getEditableDocument() {
            return this.inputController.getEditableDocument();
        }
        getDocumentRenderer(): DocumentRenderer {
            return this.canvasManager.renderer;
        }
        getModifiedState(): IsModified {
            if(this.serverDispatcher.saveInProgress())
                return IsModified.SaveInProgress;
            else if(!this.history)
                return IsModified.False;
            else if(this.serverDispatcher.wasModifiedOnServer)
                return IsModified.True;
            else if(this.serverDispatcher.lastSavedHistoryItemId != this.history.getCurrentItemId())
                return IsModified.True;
            return IsModified.False;
        }
        confirmOnLosingChanges(): boolean {
            return this.owner.confirmOnLosingChanges();
        }
        getFileName(): string {
            return this.fileName;
        }
        hasWorkDirectory(): boolean {
            return this.owner.hasWorkDirectory;
        }
        raiseDocumentChanged() {
            this.owner.raiseDocumentChanged();
        }

        setTouchBarsVisibile(visible: boolean): void {
            this.canvasManager.renderer.selection.setVisibilityTouchBars(visible);
        }

        private getHandlerUri(): string {
            var handlerURI = document.URL;
            if(handlerURI.indexOf("?") != -1)
                handlerURI = handlerURI.substring(0, handlerURI.indexOf("?"));
            if(/.*\.aspx$/.test(handlerURI))
                handlerURI = handlerURI.substring(0, handlerURI.lastIndexOf("/") + 1);
            else if(handlerURI.lastIndexOf("/") != handlerURI.length - 1)
                handlerURI += "/";
            handlerURI += "DXS.axd?s=" + this.sessionGuid + "&c=" + this.clientGuid;
            return handlerURI;
        }
        showPopupMenu(x: number, y: number) {
            this.bars.updateContextMenu();
            if(x < 0 && y < 0) {
                var selectionPoint = this.canvasManager.getContextMenuAbsPoint();
                this.owner.showPopupMenu(selectionPoint.x, selectionPoint.y);
            }
            else
                this.owner.showPopupMenu(x, y);
        }
        hidePopupMenu() {
            this.owner.hidePopupMenu();
        }
        showLoupe(evt: RichMouseEvent) {
            this.loupe.show(evt);
        }
        moveLoupe(evt: RichMouseEvent) {
            this.loupe.move(evt);
        }
        hideLoupe() {
            this.loupe.hide();
        }
        isTouchMode(): boolean {
            return this.owner.isTouchMode();
        }
        dispose() {
            this.serverDispatcher.reset();
        }
        waitForDblClick() {
            this.canvasManager.waitForDblClick();
        }

        getViewElement(): HTMLDivElement {
            return this.canvasManager.canvas;
        }
        getCanvasSizeInfo(): CanvasSizeInfo {
            return this.canvasManager.sizes;
        }
    }

    export interface IControlOwner {
        syncSessionGuid(sessionGuid: string);
        raiseBeginSynchronization();
        sendRequest(content: string, viaInternalCallback: boolean);
        canCaptureFocus(): boolean;
        isInitialized: boolean;
        AdjustControlCore();
        toggleFullScreenMode();
        isInFullScreenMode: boolean;
        sendDownloadRequest(downloadRequestType: DownloadRequestType, parameters?: string);
        confirmOnLosingChanges(): boolean;
        hasWorkDirectory: boolean;
        showDialog(name: string, parameters: any, callback: (params: any) => void, afterClosing: () => void, isModal: boolean);
        raiseDocumentChanged();
        showPopupMenu(x: number, y: number);
        hidePopupMenu();
        showLoupe(evt: RichMouseEvent);
        moveLoupe(evt: RichMouseEvent);
        hideLoupe();
        setLoadingPanelVisible(visible: boolean);
        isTouchMode(): boolean;
    }
} 