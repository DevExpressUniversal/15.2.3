module __aspxRichEdit {
    export class LayoutItemBase extends Rectangle implements ICloneable<LayoutItemBase>, ISupportCopyFrom<LayoutItemBase> {
        public isContentValid: boolean = false;//false - the item must be recalculated, but inner items can be reused. true - item can be reused.

        public clone(): LayoutItemBase {
            const newObj: LayoutItemBase = new LayoutItemBase();
            newObj.copyFrom(this);
            return newObj;
        }

        public copyFrom(obj: LayoutItemBase) {
            super.copyFrom(obj);
            this.isContentValid = obj.isContentValid;
        }

        // don't realize here equal method!
    }
} 