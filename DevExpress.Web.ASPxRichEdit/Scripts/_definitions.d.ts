declare module ASPx {
    module Evt {
        function AttachEventToElement(element: Node, eventName: string, handler: () => void);
        function AttachEventToElement(element: Node, eventName: string, handler: (evt: Event) => void);
        function AttachEventToDocument(eventName: string, handler: (evt: Event) => void);
        function PreventEvent(evt: Event);
        function GetKeyCode(evt: KeyboardEvent): number;
        function GetEventSource(evt: Event): HTMLElement;
        function IsLeftButtonPressed(evt: Event): boolean;
        function PreventEventAndBubble(evt: Event): boolean;
        function GetEventX(evt): number;
        function GetEventY(evt): number;
        function CancelBubble(evt);
    }


    export class Key {
        static Esc: number;
    }

    module PopupUtils {
        function PreventContextMenu(evt);
    }

    module Attr {
        function SetAttribute(obj: any, attrName: string, value: any);
        function GetAttribute(obj: any, attrName: string): any;
    }

    module Str {
        function TrimStart(str: string): string;
        function TrimEnd(str: string): string;
        function Trim(str: string): string;
    }

    export class Browser {
        static Edge: boolean;
        static IE: boolean;
        static Chrome: boolean;
        static Opera: boolean;
        static Firefox: boolean;
        static Safari: boolean;
        static MajorVersion: number;
        static WebKitFamily: boolean;
        static TouchUI: boolean;
        static WebKitTouchUI: boolean;
        static MacOSMobilePlatform: boolean;
        static MacOSPlatform: boolean;
        static MSTouchUI: boolean;
    }

    export class TouchUIHelper {
        static touchMouseDownEventName: string;
        static touchMouseUpEventName: string;
        static touchMouseMoveEventName: string;
        static msTouchDraggableClassName: string;
    }

    export class Formatter {
        static Format(...args: any[]): string;
    }

    export class DateFormatter {
        SetFormatString(formatString: string);
        Format(date: Date): string;
    }

    export class CultureInfo {
        static abbrDayNames: string[];
        static dayNames: string[];
        static abbrMonthNames: string[];
        static monthNames: string[];
        static genMonthNames: string[];
        static am: string;
        static pm: string;
    }

    var EmptyImageUrl: string;

    function GetFocusedElement(): HTMLElement;
    function SetFocus(element: HTMLElement, selectAction ?: string);
    function GetIsParent(parent: Node, element: HTMLElement): boolean;
    function AddClassNameToElement(element: HTMLElement, className: string);
    function RemoveClassNameFromElement(element: HTMLElement, className: string);
    function GetAbsolutePositionY(element: HTMLElement): number;
    function GetAbsolutePositionX(element: HTMLElement): number;
    function GetCurrentStyle(element: HTMLElement): CSSStyleDeclaration;
    function IsPercentageSize(size: string): boolean;
    function PxToInt(px: string): number;
    function GetChildNodes(element: HTMLElement, predicate: (e: HTMLElement) => boolean): HTMLElement[];
    function GetChildNodesByClassName(element: Node, className: string): HTMLElement[];
    function GetNodesByClassName(element: Node, className: string): HTMLElement[];
    function GetNodes(element: HTMLElement, predicate: (e: HTMLElement) => boolean): HTMLElement[];
    function GetParentByClassName(element: HTMLElement, className: string): HTMLElement;
    function GetParentByTagName(element: HTMLElement, tagName: string): HTMLElement;
    function GetInnerText(container: HTMLElement): string;
    function GetVerticalScrollBarWidth(): number;
    function GetHorizontalBordersWidth(element: HTMLElement): number;
    function SetElementOpacity(element: HTMLElement, value: number): void;
    function CreateGuid(): string;
    function IsUrlContainsClientScript(url: string): boolean;
}