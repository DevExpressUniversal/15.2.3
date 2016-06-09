module __aspxRichEdit {
    export module Ruler {
        export class ListenerManagerBase {
            canHandle: boolean = true;
        }

        export class MouseEventsManagerClass extends ListenerManagerBase {
            private canMouseMoveHandle: boolean = false;
            private startX: number;
            private touchID: number = -1;
            private listeners: IEventsListener[] = [];
            private listener: IEventsListener;

            constructor() {
                super();
                ASPx.Evt.AttachEventToElement(document.documentElement, ASPx.TouchUIHelper.touchMouseDownEventName, (evt: MouseEvent) => this.onMouseDown(evt));
                ASPx.Evt.AttachEventToElement(document.documentElement, ASPx.TouchUIHelper.touchMouseMoveEventName, (evt: MouseEvent) => this.onMouseMove(evt));
                ASPx.Evt.AttachEventToElement(document.documentElement, ASPx.TouchUIHelper.touchMouseUpEventName, (evt: MouseEvent) => this.onMouseUp(evt));
                ASPx.Evt.AttachEventToElement(document.documentElement, "dblclick", (evt: MouseEvent) => this.onDoubleClick(evt));
                if (!ASPx.Browser.TouchUI)
                    ASPx.Evt.AttachEventToElement(document.documentElement, "keydown", (evt: KeyboardEvent) => this.onKeyBoardEvent(evt));
            }

            private onDoubleClick(evt: MouseEvent): void {
                this.listener = this.getCurrentListener(evt);
                if (this.listener && this.listener.canHandleDoubleClick(ASPx.Evt.GetEventSource(evt)))
                    this.listener.onDoubleClick(ASPx.Evt.GetEventX(evt));
                this.listener = null;
            }

            private onKeyBoardEvent(evt: KeyboardEvent): void {
                if (this.canMouseMoveHandle && ASPx.Evt.GetKeyCode(evt) == ASPx.Key.Esc) {
                    this.listener.onEscPress();
                    this.reset();
                }
            }
            private onMouseDown(evt: MouseEvent): void {
                if (!this.canHandle) return;
                if (ASPx.Browser.TouchUI)
                    this.touchID = this.getChangedTouchesIdentifier(evt);

                this.startX = ASPx.Evt.GetEventX(evt);
                this.listener = this.getCurrentListener(evt);
                if (ASPx.Evt.IsLeftButtonPressed(evt) && this.listener) {
                    this.canMouseMoveHandle = true;
                    this.listener.onMouseDown(ASPx.Evt.GetEventSource(evt), this.startX);
                    ASPx.Evt.PreventEventAndBubble(evt);
                }
            }
            private onMouseMove(evt: MouseEvent): void {
                if (ASPx.Browser.TouchUI && this.touchID != this.getChangedTouchesIdentifier(evt))
                    return;

                if (this.canMouseMoveHandle) {
                    var distance = ASPx.Evt.GetEventX(evt) - this.startX;
                    this.listener.onMouseMove(ASPx.Evt.GetEventX(evt) - this.startX, ASPx.Evt.GetEventSource(evt));
                    ASPx.Evt.PreventEventAndBubble(evt);
                }
            }
            private onMouseUp(evt: MouseEvent): void {
                if (this.listener) {
                    this.listener.onMouseUp();
                    this.reset();
                }
            }
            private reset(): void {
                this.listener = null;
                this.canMouseMoveHandle = false;
                this.touchID = -1;
            }
            private getCurrentListener(evt: MouseEvent): IEventsListener {
                for (var i = 0; i < this.listeners.length; i++)
                    if (this.listeners[i].canHandle(ASPx.Evt.GetEventSource(evt)))
                        return this.listeners[i];
                return null;
            }
            public addListener(rulerControl: IEventsListener): void {
                this.listeners.push(rulerControl);
            }
            private getChangedTouchesIdentifier(evt: MouseEvent): number {
                return !ASPx.Browser.MSTouchUI ? (<any>evt).changedTouches[0].identifier : this.touchID;
            }
        }

        export class ListenerScrollStruct {
            view: HTMLElement;
            listeners: IScrollEventListener[] = [];
            constructor(view: HTMLElement) {
                this.view = view;
            }
        }

        export class ViewElementScrollManagerClass extends ListenerManagerBase {
            controls: ListenerScrollStruct[] = [];

            private onScroll(index: number): void {
                if (!this.canHandle) return;

                var listeners = this.controls[index].listeners;
                for (var i = 0; i < listeners.length; i++)
                    listeners[i].onScroll();
            }
            addListener(owner: IScrollEventListener, viewElement: HTMLElement): void {
                var listenerObj = null;
                for (var i = 0; i < this.controls.length; i++)
                    if (this.controls[i].view == viewElement)
                        listenerObj = this.controls[i];
                if (!listenerObj) {
                    listenerObj = new ListenerScrollStruct(viewElement);
                    this.controls.push(listenerObj);
                    ASPx.Evt.AttachEventToElement(listenerObj.view, "scroll", (evt: MouseEvent) => this.onScroll(this.controls.length - 1));
                }
                listenerObj.listeners.push(owner);
            }
        }

        export var MouseEventsManager: MouseEventsManagerClass = new MouseEventsManagerClass();
        export var ViewElementScrollManager: ViewElementScrollManagerClass = new ViewElementScrollManagerClass();
    }
} 