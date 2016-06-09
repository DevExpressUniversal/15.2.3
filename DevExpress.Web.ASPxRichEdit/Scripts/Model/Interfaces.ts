module __aspxRichEdit {
    export interface IListLevelPropertiesContainer {
        setListLevelProperties(properties: ListLevelProperties);
        getListLevelProperties(): ListLevelProperties;
    }

    export interface IMaskedProperties<T> {
        setUseValue(mask: T, value: boolean);
        getUseValue(mask: T): boolean;
    }

    export interface IParagraphPropertiesContainer {
        setParagraphProperties(properties: MaskedParagraphProperties);
        onParagraphPropertiesChanged();
        resetParagraphMergedProperties();
        getParagraphMergedProperies(): ParagraphProperties;
        setParagraphMergedProperies(properties: ParagraphProperties);
        setParagraphMergedProperiesByIndexInCache(index: number);
        hasParagraphMergedProperies(): boolean;
    }

    export interface ICharacterPropertiesContainer {
        setCharacterProperties(properties: MaskedCharacterProperties);
        onCharacterPropertiesChanged();
        resetCharacterMergedProperties();
        getCharacterMergedProperies(): CharacterProperties;
        setCharacterMergedProperies(properties: CharacterProperties);
        setCharacterMergedProperiesByIndexInCache(index: number);
        hasCharacterMergedProperies(): boolean;
    }

    export interface IPositionManager {
        registerPosition: (position: number) => Position;
        // Use when you do not need the position
        unregisterPosition: (position: Position) => void;
        // 1) length >= 0  =>  move all positions on the right of the current (including current) at length
        // 2) length < 0   =>  move all positions on the right of the current (including or excluding current in some case) at length
        advance: (position: number, delta: number) => void;
        //remove all positions
        reset: () => void;
    }
}