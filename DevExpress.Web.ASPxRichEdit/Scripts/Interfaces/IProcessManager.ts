module __aspxRichEdit {
    export interface IFormatterController {
        formatterOnIntervalChanged(interval: FixedInterval, subDocument: SubDocument);
        formatterResetAllLayout();
        runFormattingAsync();
        runFormatting(pageIndex: number);
        formatSync();
        stopFormatting();
        forceFormatPage(pageIndex: number): LayoutPage;
        forceFormatFromPage(pageIndex: number, modelPosition: number);
    }

    export interface IProcessManager {
    }
} 