module __aspxRichEdit {
    export interface ISelectionLayoutChangesListener {
        NotifyPageSelectionLayoutChanged(pageIndex: number, layoutSelection: LayoutSelection);
        NotifySelectionLayoutChanged(layoutSelection: LayoutSelection);
        NotifyFocusChanged(inFocus: boolean);
    }
}