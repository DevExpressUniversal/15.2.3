module __aspxRichEdit {
    export class CommandManager implements ISelectionChangesListener {
        commands: { [key: number]: ICommand; } = {};
        shortcuts: { [code: number]: RichEditClientCommand } = {};
        control: IRichEditControl;
        knownNonCommandShortcuts: { [code: number]: boolean } = {};
        private lastCommandsChain: ICommand[] = [];
        private executingCommandsChain: ICommand[] = [];
        private executingCommandCounter: number = 0;

        constructor(control: IRichEditControl) {
            this.control = control;
            this.createCommands();
            this.knownNonCommandShortcuts[KeyCode.Escape] = true;
        }

        public processShortcut(code: number): boolean {
            var commandKey = this.shortcuts[code];
            if(commandKey) {
                var command = this.getCommand(commandKey);
                if(command) {
                    this.executeCommand(command, this.isClipboardCommand(command) ? true : null);
                    return true;
                }
            }
            return false;
        }
        public isKnownShortcut(code: number): boolean {
            return !!this.shortcuts[code] || this.knownNonCommandShortcuts[code];
        }
        public isClipboardCommandShortcut(code: number): boolean {
            var key = this.shortcuts[code];
            if(key)
                return this.isClipboardCommand(this.commands[key]);
            return false;
        }

        public getCommand(key: RichEditClientCommand) {
            return this.commands[key];
        }

        public beforeExecuting(command: ICommand) {
            this.executingCommandsChain.push(command);
            this.executingCommandCounter++;
        }
        public afterExecuting() {
            this.executingCommandCounter--;
            if(this.executingCommandCounter === 0) {
                this.lastCommandsChain = this.executingCommandsChain;
                this.executingCommandsChain = [];
            }
        }
        public assertLastExecutedCommandsChain(checkLength: boolean, ...types: Function[]): boolean {
            if(checkLength && this.lastCommandsChain.length !== types.length)
                return false;
            for(var i = 0, type: Function; type = types[i]; i++) {
                if(!this.lastCommandsChain[i] || !(this.lastCommandsChain[i] instanceof type))
                    return false;
            }
            return true;
        }
        private isClipboardCommand(command: ICommand): boolean {
            return command instanceof ClipboardCommand;
        }

        NotifySelectionChanged(selection: Selection): void {
            this.lastCommandsChain = [];
        }
        NotifyFocusChanged(inFocus: boolean): void { }

        private createCommands() {
            this.createCommand(RichEditClientCommand.AddSpacingAfterParagraph, AddSpacingAfterParagraphCommand);
            this.createCommand(RichEditClientCommand.AddSpacingBeforeParagraph, AddSpacingBeforeParagraphCommand);
            this.createCommand(RichEditClientCommand.CapitalizeEachWordTextCase, CapitalizeEachWordCaseCommand);
            this.createCommand(RichEditClientCommand.ChangeFontBackColor, ChangeFontBackColorCommand);
            this.createCommand(RichEditClientCommand.ChangeFontForeColor, ChangeFontColorCommand);
            this.createCommand(RichEditClientCommand.ChangeFontName, ChangeFontNameCommand);
            this.createCommand(RichEditClientCommand.ChangeFontSize, ChangeFontSizeCommand);
            this.createCommand(RichEditClientCommand.ChangeInlinePictureScale, ChangeInlinePictureScaleCommand);
            this.createCommand(RichEditClientCommand.ChangePageColor, ChangePageColorCommand);
            this.createCommand(RichEditClientCommand.ChangeParagraphBackColor, ChangeParagraphBackColorCommand);
            this.createCommand(RichEditClientCommand.ChangeStyle, ApplyStyleCommand);
            this.createCommand(RichEditClientCommand.ClearFormatting, ClearFormattingCommand, KeyModifiers.Ctrl | KeyCode.Space);
            this.createCommand(RichEditClientCommand.ContinueNumberingList, ContinueNumberingListCommand);
            this.createCommand(RichEditClientCommand.CopySelection, CopySelectionCommand, KeyModifiers.Ctrl | KeyCode.Key_c, KeyModifiers.Meta | KeyCode.Key_c);
            this.createCommand(RichEditClientCommand.CreateField, CreateFieldCommand, KeyModifiers.Ctrl | KeyCode.F9);
            this.createCommand(RichEditClientCommand.CutSelection, CutSelectionCommand, KeyModifiers.Ctrl | KeyCode.Key_x, KeyModifiers.Meta | KeyCode.Key_x);
            this.createCommand(RichEditClientCommand.DecreaseFontSize, DecreaseFontSizeCommand);
            this.createCommand(RichEditClientCommand.DecreaseIndent, DecrementIndentCommand, KeyModifiers.Ctrl | KeyModifiers.Shift | KeyCode.Key_m, KeyModifiers.Meta | KeyModifiers.Shift | KeyCode.Key_m);
            this.createCommand(RichEditClientCommand.DecrementNumberingIndent, DecrementNumberingIndentCommand);
            this.createCommand(RichEditClientCommand.DecrementParagraphIndentFromFirstRow, DecrementParagraphIndentFromFirstRowCommand);
            this.createCommand(RichEditClientCommand.DecrementParagraphLeftIndent, DecrementParagraphLeftIndentCommand);
            this.createCommand(RichEditClientCommand.DeleteNumerationFromParagraphs, DeleteNumerationFromParagraphsCommand);
            this.createCommand(RichEditClientCommand.DeleteTabRuler, DeleteTabAtParagraphCommand);
            this.createCommand(RichEditClientCommand.DocumentEnd, GoToDocumentEndCommand, KeyModifiers.Ctrl | KeyCode.End, KeyModifiers.Meta | KeyCode.Down);
            this.createCommand(RichEditClientCommand.DocumentStart, GoToDocumentStartCommand, KeyModifiers.Ctrl | KeyCode.Home, KeyModifiers.Meta | KeyCode.Up);
            this.createCommand(RichEditClientCommand.DragCopyContent, DragCopyContentCommand);
            this.createCommand(RichEditClientCommand.DragMoveContent, DragMoveContentCommand);
            this.createCommand(RichEditClientCommand.ExtendDocumentEnd, ExtendGoToDocumentEndCommand, KeyModifiers.Ctrl | KeyModifiers.Shift | KeyCode.End, KeyModifiers.Meta | KeyModifiers.Shift | KeyCode.Down);
            this.createCommand(RichEditClientCommand.ExtendDocumentStart, ExtendGoToDocumentStartCommand, KeyModifiers.Ctrl | KeyModifiers.Shift | KeyCode.Home, KeyModifiers.Meta | KeyModifiers.Shift | KeyCode.Up);
            this.createCommand(RichEditClientCommand.ExtendGoToEndParagraph, ExtendGoToParagraphEndCommand, KeyModifiers.Ctrl | KeyModifiers.Shift | KeyCode.Down);
            this.createCommand(RichEditClientCommand.ExtendGoToNextWord, ExtendGoToNextWordCommand, KeyModifiers.Ctrl | KeyModifiers.Shift | KeyCode.Right);
            this.createCommand(RichEditClientCommand.ExtendGoToPrevWord, ExtendGoToPrevWordCommand, KeyModifiers.Ctrl | KeyModifiers.Shift | KeyCode.Left);
            this.createCommand(RichEditClientCommand.ExtendGoToStartParagraph, ExtendGoToParagraphStartCommand, KeyModifiers.Ctrl | KeyModifiers.Shift | KeyCode.Up);
            this.createCommand(RichEditClientCommand.ExtendLineDown, ExtendLineDownCommand, KeyModifiers.Shift | KeyCode.Down);
            this.createCommand(RichEditClientCommand.ExtendLineEnd, ExtendGoToLineEndCommand, KeyModifiers.Shift | KeyCode.End);
            this.createCommand(RichEditClientCommand.ExtendLineStart, ExtendGoToLineStartCommand, KeyModifiers.Shift | KeyCode.Home);
            this.createCommand(RichEditClientCommand.ExtendLineUp, ExtendGoToLineAboveCommand, KeyModifiers.Shift | KeyCode.Up);
            this.createCommand(RichEditClientCommand.ExtendNextCharacter, ExtendGoToNextCharacterCommand, KeyModifiers.Shift | KeyCode.Right);
            this.createCommand(RichEditClientCommand.ExtendNextPage, ExtendGoToNextPageCommand, KeyModifiers.Shift | KeyCode.PageDown);
            this.createCommand(RichEditClientCommand.ExtendPreviousCharacter, ExtendGoToPrevCharacterCommand, KeyModifiers.Shift | KeyCode.Left);
            this.createCommand(RichEditClientCommand.ExtendPreviousPage, ExtendGoToPrevPageCommand, KeyModifiers.Shift | KeyCode.PageUp);
            this.createCommand(RichEditClientCommand.ExtendSelectLine, ExtendSelectLineCommand);
            this.createCommand(RichEditClientCommand.ExtendSelectLineNoUpdateControlState, ExtendSelectLineCommandNoUpdateControlState);
            this.createCommand(RichEditClientCommand.FileNew, NewDocumentCommand, KeyModifiers.Ctrl | KeyCode.Key_n, KeyModifiers.Meta | KeyCode.Key_n);
            this.createCommand(RichEditClientCommand.FileOpen, DialogOpenFileCommand, KeyModifiers.Ctrl | KeyCode.Key_o, KeyModifiers.Meta | KeyCode.Key_o, KeyModifiers.Ctrl | KeyCode.F12, KeyModifiers.Meta | KeyCode.F12);
            this.createCommand(RichEditClientCommand.FilePrint, PrintDocumentCommand, KeyModifiers.Ctrl | KeyCode.Key_p, KeyModifiers.Meta | KeyCode.Key_p, KeyModifiers.Ctrl | KeyModifiers.Shift | KeyCode.F12, KeyModifiers.Meta | KeyModifiers.Shift | KeyCode.F12);
            this.createCommand(RichEditClientCommand.FileSave, SaveDocumentCommand, KeyModifiers.Ctrl | KeyCode.Key_s, KeyModifiers.Meta | KeyCode.Key_s);
            this.createCommand(RichEditClientCommand.FileSaveAs, DialogSaveFileCommand);
            this.createCommand(RichEditClientCommand.FullScreen, ToggleFullScreenCommand, KeyCode.F11, KeyModifiers.Ctrl | KeyCode.F10, KeyModifiers.Meta | KeyCode.F10);
            this.createCommand(RichEditClientCommand.GoToEndParagraph, GoToParagraphEndCommand, KeyModifiers.Ctrl | KeyCode.Down);
            this.createCommand(RichEditClientCommand.GoToNextWord, GoToNextWordCommand, KeyModifiers.Ctrl | KeyCode.Right);
            this.createCommand(RichEditClientCommand.GoToPrevWord, GoToPrevWordCommand, KeyModifiers.Ctrl | KeyCode.Left);
            this.createCommand(RichEditClientCommand.GoToFirstDataRecord, GoToFirstDataRecordCommand);
            this.createCommand(RichEditClientCommand.GoToPreviousDataRecord, GoToPreviousDataRecordCommand);
            this.createCommand(RichEditClientCommand.GoToNextDataRecord, GoToNextDataRecordCommand);
            this.createCommand(RichEditClientCommand.GoToLastDataRecord, GoToLastDataRecordCommand);
            this.createCommand(RichEditClientCommand.GoToStartParagraph, GoToParagraphStartCommand, KeyModifiers.Ctrl | KeyCode.Up);
            this.createCommand(RichEditClientCommand.IncreaseFontSize, IncreaseFontSizeCommand);
            this.createCommand(RichEditClientCommand.IncreaseIndent, IncrementIndentCommand, KeyModifiers.Ctrl | KeyCode.Key_m, KeyModifiers.Meta | KeyCode.Key_m);
            this.createCommand(RichEditClientCommand.IncrementNumberingIndent, IncrementNumberingIndentCommand);
            this.createCommand(RichEditClientCommand.IncrementParagraphIndentFromFirstRow, IncrementParagraphIndentFromFirstRowCommand);
            this.createCommand(RichEditClientCommand.IncrementParagraphLeftIndent, IncrementParagraphLeftIndentCommand);
            this.createCommand(RichEditClientCommand.InsertColumnBreak, InsertColumnBreakCommand, KeyModifiers.Ctrl | KeyModifiers.Shift | KeyCode.Enter, KeyModifiers.Meta | KeyModifiers.Shift | KeyCode.Enter);
            this.createCommand(RichEditClientCommand.InsertLineBreak, InsertLineBreakCommand, KeyModifiers.Shift | KeyCode.Enter);
            this.createCommand(RichEditClientCommand.InsertNumerationToParagraphs, InsertNumerationToParagraphsCommand);
            this.createCommand(RichEditClientCommand.InsertPageBreak, InsertPageBreakCommand, KeyModifiers.Ctrl | KeyCode.Enter, KeyModifiers.Meta | KeyCode.Enter);
            this.createCommand(RichEditClientCommand.InsertParagraph, InsertParagraphCommand, KeyCode.Enter);
            this.createCommand(RichEditClientCommand.InsertPicture, DialogInsertImageCommand);
            this.createCommand(RichEditClientCommand.InsertSectionBreakEvenPage, InsertSectionBreakEvenPageCommand);
            this.createCommand(RichEditClientCommand.InsertSectionBreakNextPage, InsertSectionBreakNextPageCommand);
            this.createCommand(RichEditClientCommand.InsertSectionBreakOddPage, InsertSectionBreakOddPageCommand);
            this.createCommand(RichEditClientCommand.InsertShiftTabMark, InsertShiftTabCommand, KeyModifiers.Shift | KeyCode.Tab);
            this.createCommand(RichEditClientCommand.InsertSpace, InsertSpaceCommand, KeyCode.Space, KeyModifiers.Shift | KeyCode.Space);
            this.createCommand(RichEditClientCommand.InsertTabMark, InsertTabCommand, KeyCode.Tab);
            this.createCommand(RichEditClientCommand.InsertTabRuler, InsertTabToParagraphCommand);
            this.createCommand(RichEditClientCommand.InsertText, InsertTextCommand);
            this.createCommand(RichEditClientCommand.LineDown, LineDownCommand, KeyCode.Down);
            this.createCommand(RichEditClientCommand.LineEnd, GoToLineEndCommand, KeyCode.End);
            this.createCommand(RichEditClientCommand.LineStart, GoToLineStartCommand, KeyCode.Home);
            this.createCommand(RichEditClientCommand.LineUp, GoToLineAboveCommand, KeyCode.Up);
            this.createCommand(RichEditClientCommand.MakeTextLowerCase, MakeTextLowerCaseCommand);
            this.createCommand(RichEditClientCommand.MakeTextUpperCase, MakeTextUpperCaseCommand);
            this.createCommand(RichEditClientCommand.MoveTabRuler, MoveTabRulerInParagraphCommand);
            this.createCommand(RichEditClientCommand.NextCharacter, GoToNextCharacterCommand, KeyCode.Right);
            this.createCommand(RichEditClientCommand.NextPage, GoToNextPageCommand, KeyCode.PageDown);
            this.createCommand(RichEditClientCommand.OpenHyperlink, OpenHyperlinkCommand);
            this.createCommand(RichEditClientCommand.PasteSelection, PasteSelectionCommand, KeyModifiers.Ctrl | KeyCode.Key_v, KeyModifiers.Meta | KeyCode.Key_v);
            this.createCommand(RichEditClientCommand.PreviousCharacter, GoToPrevCharacterCommand, KeyCode.Left);
            this.createCommand(RichEditClientCommand.PreviousPage, GoToPrevPageCommand, KeyCode.PageUp);
            this.createCommand(RichEditClientCommand.Redo, RedoCommand, KeyModifiers.Ctrl | KeyCode.Key_y, KeyModifiers.Meta | KeyCode.Key_y);
            this.createCommand(RichEditClientCommand.ReloadDocument, ReloadDocumentCommand);
            this.createCommand(RichEditClientCommand.RemoveHyperlink, RemoveHyperlinkCommand);
            this.createCommand(RichEditClientCommand.RemoveSpacingAfterParagraph, RemoveSpacingAfterParagraphCommand);
            this.createCommand(RichEditClientCommand.RemoveSpacingBeforeParagraph, RemoveSpacingBeforeParagraphCommand);
            this.createCommand(RichEditClientCommand.RestartNumberingList, RestartNumberingListCommand);
            this.createCommand(RichEditClientCommand.RulerParagraphLeftIndents, RulerParagraphLeftIndentsCommand);
            this.createCommand(RichEditClientCommand.RulerParagraphRightIndent, RulerParagraphRightIndentCommand);
            this.createCommand(RichEditClientCommand.RulerSectionColumnsSettings, RulerSectionColumnsSettingsCommand);
            this.createCommand(RichEditClientCommand.RulerSectionMarginLeft, RulerSectionMarginLeftCommand);
            this.createCommand(RichEditClientCommand.RulerSectionMarginRight, RulerSectionMarginRightCommand);
            this.createCommand(RichEditClientCommand.SelectAll, SelectAllDocumentCommand, KeyModifiers.Ctrl | KeyCode.Key_a, KeyModifiers.Meta | KeyCode.Key_a);
            this.createCommand(RichEditClientCommand.SelectLine, SelectLineCommand);
            this.createCommand(RichEditClientCommand.SelectLineNoUpdateControlState, SelectLineCommandNoUpdateControlState);
            this.createCommand(RichEditClientCommand.AddSelectedLineCommandNoUpdateControlState, AddSelectedLineCommandNoUpdateControlState);
            this.createCommand(RichEditClientCommand.SelectParagraph, SelectParagraphCommand);
            this.createCommand(RichEditClientCommand.SetDoubleParagraphSpacing, SetDoubleParagraphSpacingCommand, KeyModifiers.Ctrl | KeyCode.Key_2, KeyModifiers.Meta | KeyCode.Key_2);
            this.createCommand(RichEditClientCommand.SetLandscapePageOrientation, SetLandscapePageOrientationCommand);
            this.createCommand(RichEditClientCommand.SetModerateSectionPageMargins, SetModerateSectionPageMarginsCommand);
            this.createCommand(RichEditClientCommand.SetNarrowSectionPageMargins, SetNarrowSectionPageMarginsCommand);
            this.createCommand(RichEditClientCommand.SetNormalSectionPageMargins, SetNormalSectionPageMarginsCommand);
            this.createCommand(RichEditClientCommand.SetPortraitPageOrientation, SetPortraitPageOrientationCommand);
            this.createCommand(RichEditClientCommand.SetSectionA4PaperKind, SetSectionA4PaperKindCommand);
            this.createCommand(RichEditClientCommand.SetSectionA5PaperKind, SetSectionA5PaperKindCommand);
            this.createCommand(RichEditClientCommand.SetSectionA6PaperKind, SetSectionA6PaperKindCommand);
            this.createCommand(RichEditClientCommand.SetSectionB5PaperKind, SetSectionB5PaperKindCommand);
            this.createCommand(RichEditClientCommand.SetSectionExecutivePaperKind, SetSectionExecutivePaperKindCommand);
            this.createCommand(RichEditClientCommand.SetSectionFolioPaperKind, SetSectionFolioPaperKindCommand);
            this.createCommand(RichEditClientCommand.SetSectionLegalPaperKind, SetSectionLegalPaperKindCommand);
            this.createCommand(RichEditClientCommand.SetSectionLetterPaperKind, SetSectionLetterPaperKindCommand);
            this.createCommand(RichEditClientCommand.SetSectionOneColumn, SetSectionOneColumnCommand);
            this.createCommand(RichEditClientCommand.SetSectionThreeColumns, SetSectionThreeColumnsCommand);
            this.createCommand(RichEditClientCommand.SetSectionTwoColumns, SetSectionTwoColumnsCommand);
            this.createCommand(RichEditClientCommand.SetSesquialteralParagraphSpacing, SetSesquialteralParagraphSpacingCommand, KeyModifiers.Ctrl | KeyCode.Key_5, KeyModifiers.Meta | KeyCode.Key_5);
            this.createCommand(RichEditClientCommand.SetSingleParagraphSpacing, SetSingleParagraphSpacingCommand, KeyModifiers.Ctrl | KeyCode.Key_1, KeyModifiers.Meta | KeyCode.Key_1);
            this.createCommand(RichEditClientCommand.SetWideSectionPageMargins, SetWideSectionPageMarginsCommand);
            this.createCommand(RichEditClientCommand.ShowAllFieldCodes, ShowAllFieldCodesCommand);
            this.createCommand(RichEditClientCommand.ShowAllFieldResults, ShowAllFieldResultCommand);
            this.createCommand(RichEditClientCommand.ShowBookmarkForm, DialogBookmarksCommand);
            this.createCommand(RichEditClientCommand.ShowColumnsSetupForm, DialogColumnsCommand);
            this.createCommand(RichEditClientCommand.ShowCustomNumberingListForm, DialogCustomNumberingListCommand);
            this.createCommand(RichEditClientCommand.ShowEditHyperlinkForm, DialogEditHyperlinkCommand);
            this.createCommand(RichEditClientCommand.ShowErrorAuthExceptionMessageCommand, ShowErrorAuthExceptionMessageCommand);
            this.createCommand(RichEditClientCommand.ShowErrorClipboardAccessDeniedMessageCommand, ShowErrorClipboardAccessDeniedMessageCommand);
            this.createCommand(RichEditClientCommand.ShowErrorDocVariableErrorCommand, ShowErrorDocVariableExceptionCommand);
            this.createCommand(RichEditClientCommand.ShowErrorInnerExceptionMessageCommand, ShowErrorInnerExceptionMessageCommand);
            this.createCommand(RichEditClientCommand.ShowErrorModelIsChangedMessageCommand, ShowErrorModelIsChangedMessageCommand);
            this.createCommand(RichEditClientCommand.ShowErrorOpeningAndOverstoreImpossibleMessageCommand, ShowErrorOpeningAndOverstoreImpossibleMessageCommand);
            this.createCommand(RichEditClientCommand.ShowErrorOpeningMessageCommand, ShowErrorCantOpenDocument);
            this.createCommand(RichEditClientCommand.ShowErrorSavingMessageCommand, ShowErrorCantSaveDocument);
            this.createCommand(RichEditClientCommand.ShowErrorPathTooLongCommand, ShowErrorPathTooLong);
            this.createCommand(RichEditClientCommand.ShowErrorSessionHasExpiredMessageCommand, ShowErrorSessionHasExpiredMessageCommand);
            this.createCommand(RichEditClientCommand.ShowFinishAndMergeForm, DialogFinishAndMergeCommand);
            this.createCommand(RichEditClientCommand.ShowFontForm, DialogFontCommand, KeyModifiers.Ctrl | KeyCode.Key_d, KeyModifiers.Meta | KeyCode.Key_d);
            this.createCommand(RichEditClientCommand.ShowHyperlinkForm, DialogCreateOrEditHyperlinkCommand, KeyModifiers.Ctrl | KeyCode.Key_k);
            this.createCommand(RichEditClientCommand.ShowInsertMergeFieldForm, DialogInsertMergeFieldCommand);
            this.createCommand(RichEditClientCommand.ShowInsertTableForm, DialogInsertTableCommand);
            this.createCommand(RichEditClientCommand.ShowNumberingListForm, DialogNumberingListCommand);
            this.createCommand(RichEditClientCommand.ShowPageMarginsSetupForm, DialogPageSetupCommand);
            this.createCommand(RichEditClientCommand.ShowPagePaperSetupForm, ShowPagePaperSetupFormCommand);
            this.createCommand(RichEditClientCommand.ShowPageSetupForm, DialogPageSetupCommand);
            this.createCommand(RichEditClientCommand.ShowParagraphForm, DialogParagraphPropertiesCommand);
            this.createCommand(RichEditClientCommand.ShowSaveMergedDocumentForm, DialogSaveMergedDocumentCommand);
            this.createCommand(RichEditClientCommand.ShowServiceFontForm, DialogServiceFontCommand);
            this.createCommand(RichEditClientCommand.ShowServiceSymbolsForm, DialogServiceSymbolsCommand);
            this.createCommand(RichEditClientCommand.ShowSymbolForm, DialogSymbolsCommand);
            this.createCommand(RichEditClientCommand.ShowTablePropertiesForm, DialogTablePropertiesCommand);
            this.createCommand(RichEditClientCommand.ShowCellOptionsForm, DialogCellPropertiesCommand);
            this.createCommand(RichEditClientCommand.ShowBorderShadingForm, DialogBorderShadingCommand);
            this.createCommand(RichEditClientCommand.ShowTabsForm, DialogTabsCommand);
            this.createCommand(RichEditClientCommand.ToggleAllFields, ToggleAllFieldsCommand, KeyModifiers.Alt | KeyCode.F9);
            this.createCommand(RichEditClientCommand.ToggleBackspaceKey, BackspaceCommand, KeyCode.Backspace);
            this.createCommand(RichEditClientCommand.ToggleBulletedListItem, ToggleBulletedListCommand);
            this.createCommand(RichEditClientCommand.ToggleDeleteKey, DeleteCommand, KeyCode.Delete);
            this.createCommand(RichEditClientCommand.ToggleFieldCodes, ToggleFieldCodesCommand, KeyModifiers.Shift | KeyCode.F9);
            this.createCommand(RichEditClientCommand.ToggleFontDoubleUnderline, ToggleFontDoubleUnderlineCommand);
            this.createCommand(RichEditClientCommand.ToggleFontItalic, ToggleFontItalicCommand, KeyModifiers.Ctrl | KeyCode.Key_i, KeyModifiers.Meta | KeyCode.Key_i);
            this.createCommand(RichEditClientCommand.ToggleFontStrikeout, ToggleFontStrikeoutCommand);
            this.createCommand(RichEditClientCommand.ToggleFontSubscript, ToggleFontSubscriptCommand, KeyModifiers.Ctrl | KeyCode.Equals, KeyModifiers.Ctrl | KeyModifiers.Meta | KeyCode.Dash);
            this.createCommand(RichEditClientCommand.ToggleFontSuperscript, ToggleFontSuperscriptCommand, KeyModifiers.Ctrl | KeyModifiers.Shift | KeyCode.Equals, KeyModifiers.Ctrl | KeyModifiers.Meta | KeyCode.Equals);
            this.createCommand(RichEditClientCommand.ToggleFontUnderline, ToggleFontSingleUnderlineCommand, KeyModifiers.Ctrl | KeyCode.Key_u, KeyModifiers.Meta | KeyCode.Key_u);
            this.createCommand(RichEditClientCommand.ToggleMultilevelListItem, ToggleMultiLevelListCommand);
            this.createCommand(RichEditClientCommand.ToggleNumberingListItem, ToggleNumberingListCommand);
            this.createCommand(RichEditClientCommand.ToggleParagraphAlignmentCenter, ToggleParagraphAlignmentCenterCommand, KeyModifiers.Ctrl | KeyCode.Key_e, KeyModifiers.Meta | KeyCode.BackSlash);
            this.createCommand(RichEditClientCommand.ToggleParagraphAlignmentJustify, ToggleParagraphAlignmentJustifyCommand, KeyModifiers.Ctrl | KeyCode.Key_j, KeyModifiers.Alt | KeyModifiers.Meta | KeyCode.BackSlash);
            this.createCommand(RichEditClientCommand.ToggleParagraphAlignmentLeft, ToggleParagraphAlignmentLeftCommand, KeyModifiers.Ctrl | KeyCode.Key_l, KeyModifiers.Meta | KeyCode.OpenBracket);
            this.createCommand(RichEditClientCommand.ToggleParagraphAlignmentRight, ToggleParagraphAlignmentRightCommand, KeyModifiers.Ctrl | KeyCode.Key_r, KeyModifiers.Meta | KeyCode.CloseBracket);
            this.createCommand(RichEditClientCommand.ToggleShowHorizontalRuler, ToggleShowHorizontalRulerCommand);
            this.createCommand(RichEditClientCommand.ToggleShowWhitespace, ToggleShowHiddenSymbolsCommand, KeyModifiers.Ctrl | KeyModifiers.Shift | KeyCode.Key_8);
            this.createCommand(RichEditClientCommand.ToggleTextCase, ToggleTextCaseCommand);
            this.createCommand(RichEditClientCommand.ToggleViewMergedData, ToggleViewMergedDataCommand);
            this.createCommand(RichEditClientCommand.Undo, UndoCommand, KeyModifiers.Ctrl | KeyCode.Key_z, KeyModifiers.Meta | KeyCode.Key_z);
            this.createCommand(RichEditClientCommand.UpdateAllFields, UpdateAllFieldsCommand);
            this.createCommand(RichEditClientCommand.UpdateField, UpdateFieldCommand, KeyCode.F9);
            this.createCommand(RichEditClientCommand.ToggleFontBold, ToggleFontBoldCommand, KeyModifiers.Ctrl | KeyCode.Key_b, KeyModifiers.Meta | KeyCode.Key_b);
            this.createCommand(RichEditClientCommand.InsertNonBreakingSpace, InsertNonBreakingSpaceCommand, KeyModifiers.Ctrl | KeyModifiers.Shift | KeyCode.Space);
            this.createCommand(RichEditClientCommand.RemoveHyperlinks, RemoveHyperlinksCommand, KeyModifiers.Ctrl | KeyModifiers.Shift | KeyCode.F9);

            this.createCommand(RichEditClientCommand.CreateDateField, CreateDateFieldCommand, KeyModifiers.Alt | KeyModifiers.Shift | KeyCode.Key_d);
            this.createCommand(RichEditClientCommand.CreatePageField, CreatePageFieldCommand, KeyModifiers.Alt | KeyModifiers.Shift | KeyCode.Key_p);
            this.createCommand(RichEditClientCommand.CreateTimeField, CreateTimeFieldCommand, KeyModifiers.Alt | KeyModifiers.Shift | KeyCode.Key_t);
            this.createCommand(RichEditClientCommand.CreateMergeField, CreateMergeFieldCommand);
            this.createCommand(RichEditClientCommand.ShowCreateHyperlinkForm, DialogCreateHyperlinkCommand);
            this.createCommand(RichEditClientCommand.SentenceCase, SentenceCaseCommand);
            this.createCommand(RichEditClientCommand.SwitchTextCase, SwitchTextCaseCommand, KeyModifiers.Shift | KeyCode.F3);

            this.createCommand(RichEditClientCommand.ChangeActiveSubDocument, ChangeActiveSubDocumentCommand);

            this.createCommand(RichEditClientCommand.InsertHeader, InsertHeaderCommand);
            this.createCommand(RichEditClientCommand.InsertFooter, InsertFooterCommand);
            this.createCommand(RichEditClientCommand.LinkHeader, LinkHeaderCommand);
            this.createCommand(RichEditClientCommand.LinkFooter, LinkFooterCommand);
            this.createCommand(RichEditClientCommand.LinkHeaderFooterToPrevious, LinkHeaderFooterToPreviousCommand);

            this.createCommand(RichEditClientCommand.CreateBookmark, CreateBookmarkCommand);
            this.createCommand(RichEditClientCommand.DeleteBookmarks, DeleteBookmarksCommand);
            this.createCommand(RichEditClientCommand.GoToBookmark, GoToBookmarkCommand);

            this.createCommand(RichEditClientCommand.ContextItem_HeadersFooters, ContextItemHeadersFooters);
            this.createCommand(RichEditClientCommand.ClosePageHeaderFooter, CloseHeaderFooterCommand);
            this.createCommand(RichEditClientCommand.GoToPageHeader, GoToHeaderCommand);
            this.createCommand(RichEditClientCommand.GoToPageFooter, GoToFooterCommand);
            this.createCommand(RichEditClientCommand.GoToNextPageHeaderFooter, GoToNextHeaderFooterCommand);
            this.createCommand(RichEditClientCommand.GoToPreviousPageHeaderFooter, GoToPreviousHeaderFooterCommand);

            this.createCommand(RichEditClientCommand.ToggleDifferentFirstPage, DifferentFirstPageHeaderFooterCommand);
            this.createCommand(RichEditClientCommand.ToggleDifferentOddAndEvenPages, DifferentOddEvenHeaderFooterCommand);

            this.createCommand(RichEditClientCommand.InsertPageNumberField, CreatePageFieldCommand);
            this.createCommand(RichEditClientCommand.InsertPageCountField, CreatePageCountFieldCommand);

            this.createCommand(RichEditClientCommand.InsertTableCore, InsertTableCoreCommand);
            this.createCommand(RichEditClientCommand.ContextItem_Tables, ContextItemTables);

            this.createCommand(RichEditClientCommand.InsertTableColumnToTheLeft, InsertTableColumnToTheLeftCommand);
            this.createCommand(RichEditClientCommand.InsertTableColumnToTheRight, InsertTableColumnToTheRightCommand);

            this.createCommand(RichEditClientCommand.InsertTableRowAbove, InsertTableRowAboveCommand);
            this.createCommand(RichEditClientCommand.InsertTableRowBelow, InsertTableRowBelowCommand);
            this.createCommand(RichEditClientCommand.DeleteTableRows, DeleteTableRowsCommand);
            this.createCommand(RichEditClientCommand.DeleteTableColumns, DeleteTableColumnsCommand);
            this.createCommand(RichEditClientCommand.InsertTableCellWithShiftToTheLeft, InsertTableCellWithShiftToTheLeftCommand);
            this.createCommand(RichEditClientCommand.DeleteTableCellsWithShiftToTheHorizontally, DeleteTableCellsWithShiftToTheHorizontallyCommand);
            this.createCommand(RichEditClientCommand.DeleteTable, DeleteTableCommand);

            this.createCommand(RichEditClientCommand.ShowInsertTableCellsForm, DialogInsertTableCellsCommand);
            this.createCommand(RichEditClientCommand.ShowDeleteTableCellsForm, DialogDeleteTableCellsCommand);
            this.createCommand(RichEditClientCommand.MergeTableCells, MergeTableCellsCommand);
            this.createCommand(RichEditClientCommand.ShowSplitTableCellsForm, DialogSplitTableCellsCommand);

            this.createCommand(RichEditClientCommand.SplitTableCellsCommand, SplitTableCellsCommand);
            this.createCommand(RichEditClientCommand.InsertTableCellsWithShiftToTheVertically, InsertTableCellsWithShiftToTheVerticallyCommand);
            this.createCommand(RichEditClientCommand.DeleteTableCellsWithShiftToTheVertically, DeleteTableCellsWithShiftToTheVerticallyCommand);

            this.createCommand(RichEditClientCommand.TableCellAlignBottomCenter, ChangeTableCellBottomCenterAlignmentCommand);
            this.createCommand(RichEditClientCommand.TableCellAlignBottomLeft, ChangeTableCellBottomLeftAlignmentCommand);
            this.createCommand(RichEditClientCommand.TableCellAlignBottomRight, ChangeTableCellBottomRightAlignmentCommand);
            this.createCommand(RichEditClientCommand.TableCellAlignMiddleCenter, ChangeTableCellMiddleCenterAlignmentCommand);
            this.createCommand(RichEditClientCommand.TableCellAlignMiddleLeft, ChangeTableCellMiddleLeftAlignmentCommand);
            this.createCommand(RichEditClientCommand.TableCellAlignMiddleRight, ChangeTableCellMiddleRightAlignmentCommand);
            this.createCommand(RichEditClientCommand.TableCellAlignTopCenter, ChangeTableCellTopCenterAlignmentCommand);
            this.createCommand(RichEditClientCommand.TableCellAlignTopLeft, ChangeTableCellTopLeftAlignmentCommand);
            this.createCommand(RichEditClientCommand.TableCellAlignTopRight, ChangeTableCellTopRightAlignmentCommand);

            this.createCommand(RichEditClientCommand.ApplyTableStyle, ApplyTableStyleCommand);

            this.createCommand(RichEditClientCommand.ToggleTableCellAllBorders, ToggleTableCellAllBordersCommand);
            this.createCommand(RichEditClientCommand.ToggleTableCellInsideBorders, ToggleTableCellInsideBordersCommand);
            this.createCommand(RichEditClientCommand.ToggleTableCellInsideHorizontalBorders, ToggleTableCellInsideHorizontalBordersCommand);
            this.createCommand(RichEditClientCommand.ToggleTableCellInsideVerticalBorders, ToggleTableCellInsideVerticalBordersCommand);
            this.createCommand(RichEditClientCommand.ToggleTableCellNoBorder, ToggleTableCellNoBorderCommand);
            this.createCommand(RichEditClientCommand.ToggleTableCellOutsideBorders, ToggleTableCellOutsideBordersCommand);
            this.createCommand(RichEditClientCommand.ToggleTableCellsBottomBorder, ToggleTableCellsBottomBorderCommand);
            this.createCommand(RichEditClientCommand.ToggleTableCellsLeftBorder, ToggleTableCellsLeftBorderCommand);
            this.createCommand(RichEditClientCommand.ToggleTableCellsRightBorder, ToggleTableCellsRightBorderCommand);
            this.createCommand(RichEditClientCommand.ToggleTableCellsTopBorder, ToggleTableCellsTopBorderCommand);

            this.createCommand(RichEditClientCommand.ToggleFirstRow, ToggleFirstRowCommand);
            this.createCommand(RichEditClientCommand.ToggleLastRow, ToggleLastRowCommand);
            this.createCommand(RichEditClientCommand.ToggleFirstColumn, ToggleFirstColumnCommand);
            this.createCommand(RichEditClientCommand.ToggleLastColumn, ToggleLastColumnCommand);
            this.createCommand(RichEditClientCommand.ToggleBandedRows, ToggleBandedRowsCommand);
            this.createCommand(RichEditClientCommand.ToggleBandedColumn, ToggleBandedColumnCommand);

            this.createCommand(RichEditClientCommand.SelectTableCell, SelectTableCellCommand);
            this.createCommand(RichEditClientCommand.SelectTableColumn, SelectTableColumnCommand);
            this.createCommand(RichEditClientCommand.SelectTableRow, SelectTableRowCommand);
            this.createCommand(RichEditClientCommand.SelectTable, SelectTableCommand);

            this.createCommand(RichEditClientCommand.ChangeTableBorderColorRepositoryItem, ChangeTableBorderColorRepositoryItemCommand);
            this.createCommand(RichEditClientCommand.ChangeTableBorderStyleRepositoryItem, ChangeTableBorderStyleRepositoryItemCommand);
            this.createCommand(RichEditClientCommand.ChangeTableBorderWidthRepositoryItem, ChangeTableBorderWidthRepositoryItemCommand);

            this.createCommand(RichEditClientCommand.ChangeTableCellShading, ChangeTableCellShadingCommand);
            this.createCommand(RichEditClientCommand.ToggleShowTableGridLines, ToggleShowTableGridLinesCommand);

            this.createCommand(RichEditClientCommand.ExtendSelectTableCell, ExtendSelectTableCellCommand);
            this.createCommand(RichEditClientCommand.ExtendSelectTableColumn, ExtendSelectTableColumnCommand);
            this.createCommand(RichEditClientCommand.ExtendSelectTableRow, ExtendSelectTableRowCommand);

            this.createCommand(RichEditClientCommand.SelectTableCellsRange, SelectTableCellsRangeCommand);
        }
        private createCommand(key: RichEditClientCommand, commandType: new (control: IRichEditControl) => ICommand, ...shortcuts: number[]) {
            this.commands[key] = new commandType(this.control);
            for(var i = 0; i < shortcuts.length; i++) {
                this.shortcuts[shortcuts[i]] = key;
            }
        }

        public executeCommand(command: ICommand, parameter: any = null): boolean {
            return command.execute(parameter);
        }
    }
}