module __aspxRichEdit {
    export interface IRulerControl extends ISelectionChangesListener, IBatchUpdatableObject {
        update(): void;
        setVisible(visible: boolean);
        getVisible(): boolean;
        setEnable(enable: boolean);
        getWidth(): number;
        getHeight(): number;
        initialize(testMode: boolean): void;
        adjust(): void;
    }
}