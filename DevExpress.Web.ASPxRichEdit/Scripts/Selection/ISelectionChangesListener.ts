module __aspxRichEdit {
    export interface ISelectionChangesListener {
        NotifySelectionChanged(selection: Selection): void;
        NotifyFocusChanged(inFocus: boolean): void;
    }
} 