module __aspxRichEdit {
    export var EMPTY_CHAR_FOR_TOUCH_UI = ".";
    export var FAKE_INPUT_CLASS_NAME = "dxre-fakeInputTarget";
    export var INPUT_CLASS_NAME = "dxre-inputTarget";

    export class InputEditorBase {
        eventManager: IEventManager;
        control: IRichEditControl;
        inputElement: HTMLElement;

        previousText: string;
        canInsertTextOnInputEvent: boolean;
        needProcessShortcutOnKeyUp: boolean;

        constructor(control: IRichEditControl, eventManager: IEventManager, parent: HTMLElement) {
            this.control = control;
            this.eventManager = eventManager;
            this.canInsertTextOnInputEvent = this.canUseInputEvent();
            this.createHierarchy(parent);
            this.initialize();
        }
        initialize(): void {
            this.initializeIfNotReadOnly();
            this.initEvents();
        }
        initializeIfNotReadOnly(): void {
            if(this.control.readOnly !== ReadOnlyMode.Persistent)
                this.initializeIfNotReadOnlyCore();
        }
        initializeIfNotReadOnlyCore(): void {
        }
        initEvents(): void {
            ASPx.Evt.AttachEventToElement(this.getEditableDocument(), "keydown", (evt: KeyboardEvent) => this.onKeyDown(evt));
            ASPx.Evt.AttachEventToElement(this.getEditableDocument(), "keyup", (evt: KeyboardEvent) => this.onKeyUp(evt));
            ASPx.Evt.AttachEventToElement(this.getEditableDocument(), "focus", () => this.onFocus());
            ASPx.Evt.AttachEventToElement(this.getEditableDocument(), "blur", () => this.onBlur());
            ASPx.Evt.AttachEventToElement(this.getEditableDocument(), "contextmenu", (evt: MouseEvent) => this.onContextMenu(evt));
            if (this.canInsertTextOnInputEvent)
                ASPx.Evt.AttachEventToElement(this.getEditableDocument(), "input", (evt: KeyboardEvent) => this.onInput(evt));
        }

        createHierarchy(parent: HTMLElement): void {
            this.inputElement = this.createInputElement();
            this.inputElement.className = INPUT_CLASS_NAME;
            parent.appendChild(this.inputElement);
        }
        createInputElement(): HTMLElement {
            return null;
        }

        onInput(evt: KeyboardEvent): void {
            if (this.canInsertTextOnInputEvent)
                this.onTextInput();
        }
        onBlur(): void {
            this.eventManager.onFocusOut();
            this.clearInputElement();
        }
        onFocus(): void {
            this.selectEditableDocumentContent();
            this.eventManager.onFocusIn();
        }
        onKeyDown(evt: KeyboardEvent) {
            evt = this.getNormalizedEvent(evt);
            var keyCode = ASPx.Evt.GetKeyCode(evt);
            this.needProcessShortcutOnKeyUp = !keyCode || keyCode == 229;
            this.canInsertTextOnInputEvent = this.canUseInputEvent();
            if (!this.needProcessShortcutOnKeyUp)
                this.onShortcut(evt);
            if (evt.altKey || evt.ctrlKey || evt.metaKey)
                this.canInsertTextOnInputEvent = false;
            else if (!this.canInsertTextOnInputEvent && !this.control.isTouchMode())
                setTimeout(() => this.onTextInput(), ASPx.Browser.Safari ? 10 : 0);
        }
        onKeyUp(evt: KeyboardEvent) {
            if (this.needProcessShortcutOnKeyUp) {
                this.onShortcut(evt);
                this.needProcessShortcutOnKeyUp = false;
            }
        }
        onContextMenu(evt: MouseEvent): void {
            ASPx.PopupUtils.PreventContextMenu(evt);
            evt.preventDefault();
            this.control.showPopupMenu(-1, -1);
        }

        onShortcut(evt: KeyboardEvent): void {
            var shortcutCode = this.getShortcutCode(evt);
            if (this.eventManager.isShortcut(shortcutCode))
                this.onShortcutCore(evt, shortcutCode);
        }
        onShortcutCore(evt: KeyboardEvent, shortcutCode: number): void {
            this.clearInputElement();
            this.selectEditableDocumentContent();
            if (!this.eventManager.isClipboardCommandShortcut(shortcutCode))
                ASPx.Evt.PreventEvent(evt);
            this.eventManager.onShortcut(shortcutCode);
        }
        onText(text: string, currentText: string, isUpdated: boolean) {
            if (!this.canInsertTextOnInputEvent)
                this.needProcessShortcutOnKeyUp = false;
            this.eventManager.onText(text, isUpdated);
            this.previousText = currentText;
        }
        onTextInput() {
            var text = this.getEditableDocumentText();
            if (text) {
                if (this.previousText) {
                    var previousText = this.previousText;
                    var previousTextLastIndex = previousText.length - 1;
                    if (text[previousTextLastIndex] && text[previousTextLastIndex] != previousText[previousTextLastIndex])
                        this.onText(text[previousTextLastIndex], text, true);
                    var insertedCharacterCount = text.length - previousText.length;
                    if (insertedCharacterCount > 0) {
                        for (var i = text.length - insertedCharacterCount; i < text.length; i++)
                            this.onText(text[i], text, false);
                    }
                }
                else
                    this.onText(text, text, false);
            }
        }

        clearKeyboardTips(): void {
        }

        captureFocus() {
        }
        canUseInputEvent(): boolean {
            return ASPx.Browser.Firefox && ASPx.Browser.MajorVersion >= 14 || ASPx.Browser.WebKitTouchUI;
        }

        getEditableDocumentText(): string {
            return ASPx.GetInnerText(this.getEditableTextOwner()).replace(/(\r\n|\n|\r)/gm, "");
        }
        getEditableTextOwner(): HTMLElement {
            return null;
        }

        setPosition(left: number, top: number): void {
            this.inputElement.style.left = left + "px";
            this.inputElement.style.top = top + "px";
        }

        clearInputElement(): void {
            this.previousText = "";
        }
        setEditableDocumentContent(content: string): void {
            this.previousText = "";
        }
        selectEditableDocumentContent() {
            this.control.bars.updateItemsState([RichEditClientCommand.CopySelection, RichEditClientCommand.PasteSelection, RichEditClientCommand.CutSelection]);
        }
        getEditableDocumentContent(): string {
            return "";
        }
        getEditableDocument(): Node {
            return null;
        }
        getNormalizedEvent(evt: Event): any {
            if (ASPx.Browser.IE && ASPx.Browser.MajorVersion < 9) {
                var eventCopy = {};
                for (var i in evt)
                    eventCopy[i] = evt[i];
                return eventCopy;
            }
            return evt;
        }

        private getShortcutCode(evt: KeyboardEvent): number {
            var keyCode = ASPx.Evt.GetKeyCode(evt);
            var modifiers = 0;
            if (evt.altKey)
                modifiers |= KeyModifiers.Alt;
            if (evt.ctrlKey)
                modifiers |= KeyModifiers.Ctrl;
            if (evt.shiftKey)
                modifiers |= KeyModifiers.Shift;
            if (evt.metaKey && ASPx.Browser.MacOSPlatform)
                modifiers |= KeyModifiers.Meta;
            return modifiers | (keyCode << 8);
        }
    }

    export class DivInputEditor extends InputEditorBase {
        private canSkipInputEvent: boolean;
        private canSkipFocusAndBlur: boolean;
        private fakeInput: HTMLElement;

        constructor(control: IRichEditControl, eventManager: IEventManager, parent: HTMLElement) {
            super(control, eventManager, parent);
        }

        initializeIfNotReadOnlyCore(): void {
            this.inputElement.dir = "rtl";
            this.inputElement.contentEditable = "true";
            this.clearInputElement();
        }

        private canUseKeyboardTips(): boolean {
            return ASPx.Browser.Safari && ASPx.Browser.MajorVersion > 7;
        }
        private createFakeInput(): HTMLElement {
            var input = document.createElement("DIV");
            input.className = FAKE_INPUT_CLASS_NAME;
            input.contentEditable = "true";
            this.inputElement.parentNode.appendChild(input);
            return input;
        }
        private setCursorOnSecondPosition(): void {
            var selection = window.getSelection();
            selection.removeAllRanges();
            var range = document.createRange();
            range.setStart(this.inputElement.firstChild, 1);
            range.setEnd(this.inputElement.firstChild, 1);
            selection.addRange(range);
        }

        createHierarchy(parent: HTMLElement): void {
            super.createHierarchy(parent);
            if(this.canUseKeyboardTips())
                this.fakeInput = this.createFakeInput();
        }
        createInputElement(): HTMLElement {
            return document.createElement("DIV");
        }

        onKeyDown(evt: KeyboardEvent) {
            super.onKeyDown(evt);
            this.canSkipInputEvent = true;
        }
        onKeyUp(evt: KeyboardEvent): void {
            super.onKeyUp(evt);
            this.control.hidePopupMenu();
            this.onTextInput();
        }
        onInput(evt: KeyboardEvent): void {
            if (!this.canSkipInputEvent)
                super.onInput(evt);
            this.control.hidePopupMenu();
            this.canSkipInputEvent = false;
        }
        onFocus(): void {
            if (!this.canSkipFocusAndBlur)
                super.onFocus();
            if(this.canUseKeyboardTips())
                this.setCursorOnSecondPosition();
            this.canSkipFocusAndBlur = false;
        }
        onBlur(): void {
            if (!this.canSkipFocusAndBlur)
                super.onBlur();
        }
        onShortcutCore(evt: KeyboardEvent, shortcutCode: number): void {
            super.onShortcutCore(evt, shortcutCode);
            this.clearKeyboardTips();
        }

        getEditableDocumentText(): string {
            var text = super.getEditableDocumentText();
            return this.canUseKeyboardTips() ? text.substr(1, text.length - 1) : text;
        }
        getEditableTextOwner(): HTMLElement {
            return this.inputElement;
        }
        captureFakeInputFocus(): void {
            this.fakeInput.style.left = this.inputElement.style.left;
            this.fakeInput.style.top = this.inputElement.style.top;
            this.fakeInput.style.display = "block";
            this.fakeInput.focus();
        }

        captureFocus() {
            this.inputElement.focus();
        }
        getEditableDocument(): Node {
            return this.inputElement;
        }
        clearInputElement(): void {
            super.clearInputElement();
            this.inputElement.innerHTML = this.canUseKeyboardTips() ? EMPTY_CHAR_FOR_TOUCH_UI : "";
        }
        clearKeyboardTips(): void {
            if (this.canUseKeyboardTips()) {
                this.clearInputElement(); //TODO
                this.canSkipFocusAndBlur = true;
                this.captureFakeInputFocus();
                this.captureFocus();
                this.fakeInput.style.display = "";
            }
        }
        getEditableDocumentContent(): string {
            return this.inputElement.innerHTML;
        }
        selectEditableDocumentContent() {
            var selection = window.getSelection();
            var firstChildNode = null;
            if (this.inputElement.childNodes.length) {
                firstChildNode = this.inputElement.childNodes[0];
                if (!firstChildNode.childNodes.length)
                    return;
            }
            selection.removeAllRanges();
            selection.selectAllChildren(this.inputElement);
            super.selectEditableDocumentContent();
        }
    }

    export class IFrameInputEditor extends InputEditorBase {
        inputElement: HTMLIFrameElement;
        editableDocument: Document;

        constructor(control: IRichEditControl, eventManager: IEventManager, parent: HTMLElement) {
            super(control, eventManager, parent);
        }

        createHierarchy(parent: HTMLElement): void {
            super.createHierarchy(parent);
            var frameHtml = "<!DOCTYPE html>";
            frameHtml += "<html>";
            frameHtml += "<head>";
            frameHtml += "</head>";
            frameHtml += "<body style=\"padding: 0px; margin: 0px; overflow: hidden;\">";
            frameHtml += "</body>";
            frameHtml += "</html>";
            this.editableDocument = this.inputElement.contentDocument || this.inputElement.contentWindow.document;
            this.editableDocument.open();
            this.editableDocument.write(frameHtml);
            this.editableDocument.close();
        }

        initializeIfNotReadOnlyCore(): void {
            this.editableDocument.designMode = "on";
        }

        createInputElement(): HTMLElement {
            var element = document.createElement("IFRAME");
            (<HTMLIFrameElement>element).src = "javascript:false;"
	        return element;
        }

        initEvents() {
            super.initEvents();
            ASPx.Evt.AttachEventToElement(this.getEditableDocument(), "paste", (evt: Event) => {  //TODO need refact
                var e = <any>evt;
                if (e && e.clipboardData && e.clipboardData.items) {
                    var items = e.clipboardData.items,
                        blob: Blob = null;

                    for (var i = 0; i < items.length; i++)
                        if (items[i].type.indexOf("image") === 0)
                            blob = items[i].getAsFile();

                    if (blob) {
                        ASPx.Evt.PreventEvent(evt);
                        var reader: FileReader = new FileReader();
                        var image: HTMLImageElement = <HTMLImageElement>document.createElement("IMG");

                        reader.onload = (ev) => {
                            image.src = <string>(<any>ev.target).result; //TODO need refact
                            image.src = <string>(<any>ev.target).result;
                        };
                        reader.readAsDataURL(blob);
                        this.getEditableDocument().appendChild(image);
                    }
                }
            });
        }
        captureFocus() {
            if(ASPx.Browser.Opera || ASPx.Browser.Chrome && this.inputElement === document.activeElement)
                this.inputElement.contentWindow.focus();
            else
                ASPx.SetFocus(ASPx.Browser.Edge || ASPx.Browser.IE || ASPx.Browser.Safari ? this.editableDocument.body : this.inputElement);
        }
        setPosition(left: number, top: number) {
            super.setPosition(left, top);
            if(left && top)
                this.selectEditableDocumentContent();
        }
        clearInputElement(): void {
            super.clearInputElement();
            this.editableDocument.body.innerHTML = "";
        }
        setEditableDocumentContent(content: string) {
            super.setEditableDocumentContent(content);
            this.editableDocument.body.innerHTML = content;
        }
        getEditableDocumentContent(): string {
            return this.editableDocument.body.innerHTML;
        }
        selectEditableDocumentContent() {
            if(!this.control.selection.isCollapsed() && !this.getEditableDocumentContent().length) {
                this.setEditableDocumentContent(ASPx.Formatter.Format("<i style=\"font-style:normal;\"{0}>&nbsp;</i>",
                    ASPx.Browser.IE && ASPx.Browser.MajorVersion < 9 ? "" : " contenteditable=\"false\""));
            }
            var firstChildNode = this.editableDocument.body.childNodes[0];
            if(firstChildNode && !firstChildNode.childNodes.length && !(firstChildNode.nodeType === 3 && firstChildNode.nodeValue === ""))
                return;

            var selection: any = this.editableDocument.getSelection ? this.editableDocument.getSelection() : this.editableDocument["selection"];

            if(selection.removeAllRanges)
                selection.removeAllRanges();
            else if(selection.empty)
                selection.empty();

            if(selection.selectAllChildren)
                selection.selectAllChildren(this.editableDocument.body);
            else if(selection.createRange) {
                try {
                    var range = selection.createRange();
                    range.moveToElementText(this.editableDocument.body);
                }
                catch(e) { }
                range.select();
            }

            super.selectEditableDocumentContent();
        }
        getEditableDocument(): Node {
            return this.editableDocument;
        }
        getEditableTextOwner(): HTMLElement {
            return this.editableDocument.body;
        }

        onShortcutCore(evt: KeyboardEvent, shortcutCode: number): void {
            var prevSelectedInterval = this.control.selection.getLastSelectedInterval();
            super.onShortcutCore(evt, shortcutCode);
            if(this.control.selection.getLastSelectedInterval() != prevSelectedInterval)
                this.selectEditableDocumentContent();
        }
        onTextInput() {
            if(ASPx.Str.Trim(this.getEditableDocumentText()))
                super.onTextInput();
        }
    }

    export class InputController {
        control: IRichEditControl;
        inputEditor: InputEditorBase;

        private exporter: HtmlExporter;

        constructor(control: IRichEditControl, eventManager: IEventManager, parent: HTMLElement) {
            this.control = control;
            this.inputEditor = this.createInputEditor(parent, eventManager);
            this.exporter = new HtmlExporter(this.control);
        }

        private createInputEditor(parent: HTMLElement, eventManager: IEventManager): InputEditorBase {
            if (this.control.isTouchMode())
                return new DivInputEditor(this.control, eventManager, parent);
            return new IFrameInputEditor(this.control, eventManager, parent);
        }

        getEditableDocument() {
            return this.inputEditor.getEditableDocument();
        }
        getExportedRangeCopy() {
            return this.exporter.rangeCopy;
        }
        captureFocus() {
            this.inputEditor.captureFocus();
        }
        setPosition(left: number, top: number) {
            this.inputEditor.setPosition(left, top);
        }
        renderSelectionToEditableDocument() {
            let html = "";
            for(let i = 0, interval: FixedInterval; interval = this.control.selection.intervals[i]; i++)
                html += this.exporter.getHtmlElementsByInterval(interval.clone());
            this.setEditableDocumentContent(html);
            this.selectEditableDocumentContent();
        }
        setEditableDocumentContent(content: string) {
            this.inputEditor.setEditableDocumentContent(content);
        }
        getEditableDocumentContent(): string {
            return this.inputEditor.getEditableDocumentContent();
        }
        selectEditableDocumentContent(): void {
            this.inputEditor.selectEditableDocumentContent();
        }
        clearKeyboardTips(): void {
            this.inputEditor.clearKeyboardTips();
        }
    }
} 