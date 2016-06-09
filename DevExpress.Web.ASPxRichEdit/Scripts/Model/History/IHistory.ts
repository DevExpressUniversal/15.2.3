module __aspxRichEdit {
    export interface IHistory {
        modelManipulator: ModelManipulator;

        undo: () => void;
        redo: () => void;
        canUndo: () => boolean;
        canRedo: () => boolean;
        clear: () => void;
        add: (historyItem: HistoryItem) => void;
        addAndRedo: (historyItem: HistoryItem) => void;
        beginTransaction: () => void;
        endTransaction: () => void; 
        isModified: () => boolean;
        resetModified: () => void;
        getCurrentItemId: () => number;
    }
} 