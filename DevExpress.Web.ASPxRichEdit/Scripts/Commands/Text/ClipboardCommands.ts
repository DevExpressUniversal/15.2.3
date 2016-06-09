module __aspxRichEdit {
    export class ClipboardCommand extends CommandBase<SimpleCommandState> {
        queryCommandId: string;
        static builtInClipboard: BuiltInClipboard;

        constructor(control: IRichEditControl, queryCommandId: string) {
            super(control);
            this.queryCommandId = queryCommandId;
            if (this.canUseBuiltInClipboard())
               ClipboardCommand.builtInClipboard = new BuiltInClipboard(this.control);
        }

        getState(): SimpleCommandState {
            var state = new SimpleCommandState(this.isEnabled());
            state.visible = this.isVisible();
            return state;
        }
        canUseBuiltInClipboard() {
            return this.control.isTouchMode();
        }
        execute(parameter: boolean = false): boolean {
            if(!this.canUseBuiltInClipboard() && !parameter && !ASPx.Browser.IE)
                return this.executeShowErrorMessageCommand();
            var prepareAndExecute = () => {
                this.beforeExecute();
                this.executeCore(this.getState(), parameter);
            };
            this.control.bars.beginUpdate();
            if(parameter)
                prepareAndExecute();
            else
                setTimeout(prepareAndExecute, 0);
            return true;
        }
        executeCore(state: SimpleCommandState, parameter: boolean): boolean {
            if(!state.enabled)
                return;
            var prevModifiedState = this.control.getModifiedState();
            if (!this.canUseBuiltInClipboard()) {
                if(!parameter)
                    this.control.getEditableDocument().execCommand(this.queryCommandId, false, null);
                setTimeout(() => {
                    this.changeModel();
                    var editableDocument = this.control.getEditableDocument();
                    if (ASPx.Browser.TouchUI) {
                        window.getSelection().removeAllRanges();
                        editableDocument.innerHTML = "";
                    } else {
                        var selection = editableDocument.getSelection ? editableDocument.getSelection() : editableDocument.selection;
                        if(selection.removeAllRanges)
                            selection.removeAllRanges();
                        else if(selection.empty)
                            selection.empty();
                        editableDocument.body.innerHTML = "";
                    }

                    if (ASPx.Browser.TouchUI)
                        window.getSelection().selectAllChildren(editableDocument);
                    else
                        this.control.selectEditableDocumentContent();

                    this.control.bars.endUpdate();
                    this.updateControlState(prevModifiedState);
                }, 0);
            } else {
                setTimeout(() => {
                    this.control.bars.endUpdate();
                    this.updateControlState(prevModifiedState);
                }, 0);
            }
            return true;
        }
        executeShowErrorMessageCommand(): boolean {
            return this.control.commandManager.getCommand(RichEditClientCommand.ShowErrorClipboardAccessDeniedMessageCommand).execute();
        }

        isEnabled(): boolean {
            var isEnabled;
            try {
                isEnabled = !ASPx.Browser.IE || ASPx.Browser.MSTouchUI || this.control.getEditableDocument().queryCommandEnabled(this.queryCommandId);
            }
            catch(e) {
                isEnabled = false;
            }
            return super.isEnabled() && isEnabled;
        }
        isVisible(): boolean {
            return true;
        }
        changeModel() {

        }
        beforeExecute() {
            if(!ASPx.Browser.TouchUI)
                this.control.captureFocus();
        }
    }
    export class CopySelectionCommand extends ClipboardCommand {
        constructor(control: IRichEditControl) {
            super(control, "copy");
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.copy) && !this.control.selection.isCollapsed();
        }
        isVisible(): boolean {
            return this.control.options.copy !== DocumentCapability.Hidden;
        }
        beforeExecute() {
            super.beforeExecute();
            if (!this.canUseBuiltInClipboard()) {
                this.control.renderSelectionToEditableDocument();
            } else
                ClipboardCommand.builtInClipboard.execute(BuiltInClipboardAction.Copy);
        }
        isEnabledInReadOnlyMode(): boolean {
            return true;
        }
    }
    export class CutSelectionCommand extends ClipboardCommand {
        constructor(control: IRichEditControl) {
            super(control, "cut");
        }
        changeModel() {
            this.control.history.beginTransaction();
            let intervals = this.control.selection.getIntervalsClone();
            for(let i = intervals.length - 1; i >= 0; i--) {
                ModelManipulator.removeInterval(this.control, this.control.model.activeSubDocument, intervals[i], true);
            }
            ModelManipulator.addToHistorySelectionHistoryItem(this.control, new FixedInterval(intervals[0].start, 0), UpdateInputPositionProperties.Yes, this.control.selection.endOfLine);
            this.control.history.endTransaction();
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.cut) && !this.control.selection.isCollapsed();
        }
        isVisible(): boolean {
            return this.control.options.cut !== DocumentCapability.Hidden;
        }
        beforeExecute() {
            super.beforeExecute();
            if (!this.canUseBuiltInClipboard()) {
                this.control.renderSelectionToEditableDocument();
            } else 
                ClipboardCommand.builtInClipboard.execute(BuiltInClipboardAction.Cut);
        }
    }
    export class PasteSelectionCommand extends ClipboardCommand {
        constructor(control: IRichEditControl) {
            super(control, "paste");
        }
        changeModel() {
            this.control.setEditableDocumentContent(this.processPastedHtml(this.control.getEditableDocumentContent()));
            var editableElement = this.control.getEditableDocument();
            var elementsContainer = editableElement.body || editableElement;
            var elements = <HTMLElement[]>(<any>elementsContainer.childNodes);
            var modelPartCopyPasted = elements.length == 1 && elements[0].nodeType == 1 && elements[0].hasAttribute("data-re-modelpartcopy");
            this.control.importHtml(elements, this.control.selection.getLastSelectedInterval().clone(), modelPartCopyPasted);
        }
        isEnabled(): boolean {
            return super.isEnabled() && ControlOptions.isEnabled(this.control.options.paste);
        }
        isVisible(): boolean {
            return this.control.options.paste !== DocumentCapability.Hidden || this.canUseBuiltInClipboard();
        }
        beforeExecute() {
            super.beforeExecute();
            if (!this.canUseBuiltInClipboard()) {
                var selection = ASPx.Browser.TouchUI ? window.getSelection() : this.control.getEditableDocument().getSelection();
                selection.removeAllRanges();
                var editableElement = this.control.getEditableDocument();
                selection.selectAllChildren(editableElement.body || editableElement);
            } else 
                ClipboardCommand.builtInClipboard.execute(BuiltInClipboardAction.Paste);
        }

        private processPastedHtml(html: string): string {
            // clear word formatting
            // remove Word attributes
            html = html.replace(/<(\w[^>]*) lang=([^ |>]*)([^>]*)/gi, "<$1$3");
            html = html.replace(/\s*mso-bidi-font-family/gi, "font-family");
            html = html.replace(/\s*MARGIN: 0cm 0cm 0pt\s*;/gi, '');
            html = html.replace(/\s*MARGIN: 0cm 0cm 0pt\s*"/gi, "\"");
            html = html.replace(/\s*TEXT-INDENT: 0cm\s*;/gi, '');
            html = html.replace(/\s*TEXT-INDENT: 0cm\s*"/gi, "\"");
            html = html.replace(/\s*FONT-VARIANT: [^\s;]+;?"/gi, "\"");
            html = html.replace(/\s*tab-stops:[^;"]*;?/gi, '');
            html = html.replace(/\s*tab-stops:[^"]*/gi, '');
            // remove special Word tags
            html = html.replace(/<\w+:imagedata/gi, '<img');
            html = html.replace(/<\/?\w+:[^>]*>/gi, '');
            html = html.replace(/<STYLE[^>]*>[\s\S]*?<\/STYLE[^>]*>/gi, '');
            html = html.replace(/<(?:META|LINK)[^>]*>\s*/gi, '');
            html = html.replace(/<\\?\?xml[^>]*>/gi, '');
            html = html.replace(/<o:[pP][^>]*>\s*<\/o:[pP]>/gi, '');
            html = html.replace(/<o:[pP][^>]*>.*?<\/o:[pP]>/gi, '&nbsp;');
            html = html.replace(/<st1:.*?>/gi, '');
            html = html.replace(/<\!--[\s\S]*?-->/g, '');
            // remove empty attributes
            html = html.replace(/\s*style="\s*"/gi, '');
            html = html.replace(/style=""/ig, "");
            html = html.replace(/style=''/ig, "");
            // clean style attributes
            var stRegExp = new RegExp('(?:style=\\")([^\\"]*)(?:\\")', 'gi');
            html = html.replace(stRegExp, (str) => {
                str = str.replace(/&quot;/gi, "'");
                str = str.replace(/&#xA;/gi, " ");
                return str;
            });
            // replace ugly Word markup
            html = html.replace(/^\s|\s$/gi, '');
            html = html.replace(/<font\s*>([^<>]+)<\/font>/gi, '$1');
            html = html.replace(/<span\s*><span\s*>([^<>]+)<\/span><\/span>/ig, '$1');
            html = html.replace(/<span>([^<>]+)<\/span>/gi, '$1');

            // Remove nested empty tags
            // safe empty td
            html = html.replace(/<td([^>]*)>\s*<\/td>/gi, '<td$1>&nbsp;</td>');

            var re = /<([^\s>]+)(\s[^>]*)?><\/\1>/g;
            while (html != html.replace(re, ''))
                html = html.replace(re, '');

            re = /<([^\s>]+)(\s[^>]*)?>\s+<\/\1>/g;
            while (html != html.replace(re, ' '))
                html = html.replace(re, ' ');
            // merge font family attributes
            var array = html.match(/<[^>]*style\s*=\s*[^>]*>/gi);
            if(array && array.length > 0) {
                for(var i = 0, elementHtml; elementHtml = array[i]; i++) {
                    var fontFamilyArray = elementHtml.match(/\s*font-family\s*:\s*([^;]*)[\"'; ]/gi);
                    if(fontFamilyArray && fontFamilyArray.length > 1) {
                        var commonValue = "";
                        for(var j = 0, fontFamily; fontFamily = fontFamilyArray[j]; j++) {
                            commonValue += commonValue ? "," : "";
                            commonValue += fontFamily.replace(/font-family\s*:\s*([^;]*)[\"'; ]/gi, "$1");
                        }
                        html = html.replace(elementHtml, elementHtml.replace(fontFamilyArray[0], "font-family: " + commonValue + ";"));
                    }
                }
            }
            html = html.replace(/^\n|\n$/gi, '');
            html = html.replace(/(\n+(<br>)|(<\/p>|<br>)\n+)/gi, '$2$3');
            html = html.replace(/\n/gi, ' '); 
            html = html.replace(/>[\s]*</g, "><");

            html = html.replace(/<span(\s(?!data-re-modelpartcopy="true")[^>]*)?><a(\s[^>]*)>([\s\S]*?)<\/a><\/span>/gi, '<a$2><span$1>$3</span></a>');

            //clear particular tags
            html = html.replace(/<script(\s[^>]*)?>[\s\S]*?<\/script>/gi, '');
            html = html.replace(/<u>([\s\S]*?)<\/u>/gi, '<span style="text-decoration: underline">$1</span>');
            html = html.replace(/<s>([\s\S]*?)<\/s>/gi, '<span style="text-decoration: line-through">$1</span>');
            html = html.replace(/<([^\s>]+)(\s[^>]*)?><br><\/\1>/gi, '');
            html = html.replace(/<\/([^\s>]+)(\s[^>]*)?><br><\/([^\s>]+)(\s[^>]*)?>/gi, '');
            return html;
        }
    }

    export class BuiltInClipboard {
        private clipboard: any;
        private control: IRichEditControl;
        private textManipulator: TextManipulator;

        constructor(control: IRichEditControl) {
            this.control = control;
        }
        private executeCopy() {
            this.clipboard = ModelManipulator.createRangeCopy(this.control.modelManipulator.model.activeSubDocument, this.control.selection.getIntervalsClone());
        }
        private executePaste() {
            if(this.clipboard)
                ModelManipulator.pasteRangeCopy(this.control, this.control.model.activeSubDocument, this.control.selection.getLastSelectedInterval().clone(), this.clipboard);
        }
        private executeCut() {
            this.control.history.beginTransaction();
            this.clipboard = ModelManipulator.createRangeCopy(this.control.modelManipulator.model.activeSubDocument, this.control.selection.getIntervalsClone());
            let intervals = this.control.selection.getIntervalsClone();
            for(let i = intervals.length - 1; i >= 0; i--) {
                ModelManipulator.removeInterval(this.control, this.control.model.activeSubDocument, intervals[i], true);
            }
            ModelManipulator.addToHistorySelectionHistoryItem(this.control, new FixedInterval(intervals[0].start, 0), UpdateInputPositionProperties.Yes, this.control.selection.endOfLine);
            this.control.history.endTransaction();
        }
        private createTextManipulatorIfNeeded() {
            if(!this.textManipulator)
                this.textManipulator = new TextManipulator(this.control.modelManipulator);
        }
        execute(action: BuiltInClipboardAction) {
            this.createTextManipulatorIfNeeded();
            switch (action) {
                case BuiltInClipboardAction.Copy:
                    this.executeCopy();
                    break;
                case BuiltInClipboardAction.Paste:
                    this.executePaste();
                    break;
                case BuiltInClipboardAction.Cut:
                    this.executeCut();
                    break;
            }
        }
    }

    export enum BuiltInClipboardAction {
        Copy = 0,
        Paste = 1,
        Cut = 2
    }
}