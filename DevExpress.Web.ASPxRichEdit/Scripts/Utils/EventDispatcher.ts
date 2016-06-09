module __aspxRichEdit {
    export interface IEventListener { }
    export class EventDispatcher<T extends IEventListener> {
        listeners: IEventListener[] = [];

        public add(listener: T): void {
            if (!listener)
                throw new Error(Errors.NotImplemented);
            if (!this.hasEventListener(listener))
                this.listeners.push(listener);
        }
        public remove(listener: T): void {
            for (var i = 0, currentListener; currentListener = this.listeners[i]; i++) {
                if (currentListener === listener) {
                    this.listeners.splice(i, 1);
                    break;
                }
            }
        }
        public raise(funcName: string, ...args: any[]): void {
            for (var i = 0, listener: IEventListener; listener = this.listeners[i]; i++) {
                listener[funcName].apply(listener, args);
            }
        }

        hasEventListener(listener: IEventListener): boolean {
            for (var i = 0, l = this.listeners.length; i < l; i++)
                if (this.listeners[i] === listener)
                    return true;
            return false;
        }
    }
} 