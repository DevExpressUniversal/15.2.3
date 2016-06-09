module __aspxRichEdit {
    export class SimpleCommandState implements ICommandState {
        enabled: boolean;
        value: any;
        visible: boolean = true;
        denyUpdateValue: boolean = false;
        items: any[];
        constructor(enabled: boolean, value?: any) {
            this.enabled = enabled;
            this.value = value;
        }
    }

    export class IntervalCommandState extends SimpleCommandState {
        public interval: FixedInterval;
        constructor(enabled: boolean, interval: FixedInterval, value?: any) {
            super(enabled, value);
            this.interval = interval;
        }
    }

    export class IntervalCommandStateEx extends SimpleCommandState {
        public intervals: FixedInterval[];
        constructor(enabled: boolean, intervals: FixedInterval[], value?: any) {
            super(enabled, value);
            this.intervals = intervals;
        }
    }
}  