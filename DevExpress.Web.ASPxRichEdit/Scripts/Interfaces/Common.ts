module __aspxRichEdit {
    export interface ICloneable<T> {
        clone(): T;
    }
    export interface ISupportCopyFrom<T> {
        copyFrom(obj: T): void;
    }
    export interface IIndexedCacheType<T> extends ISupportCopyFrom<T>, IEquatable<T> {

    }
    export interface IDisposable {
        dispose();
    }
    export interface IEquatable<T> {
        equals(obj: T): boolean;
    }
    export interface ISupportTranslateToAnotherMeasuringSystem {
        toAnotherMeasuringSystem(converterFunc: (val: any) => any): void;
    }
} 