module __aspxRichEdit {
    const ELEMENT_CLASSNAME = "dxre-rb";
    const CORNER_CLASSNAME_PREFIX = "dxre-rb-c";
    const CORNERTOUCH_CLASSNAME_POSTFIX = "Touch";
    const DIRECTION_ATTRIBUTE = "resizeDirection";

    export class CanvasResizeBoxHelper {
        private renderer: DocumentRenderer;
        private isMouseCaptured: boolean = false;
        private direction: string;
        private startX: number;
        private startY: number;
        private element: HTMLElement;

        private static directionToSource: { [direction: string]: MouseEventSource } = {
            "nw": MouseEventSource.ResizeBox_NW,
            "ne": MouseEventSource.ResizeBox_NE,
            "se": MouseEventSource.ResizeBox_SE,
            "sw": MouseEventSource.ResizeBox_SW,
            "n": MouseEventSource.ResizeBox_N,
            "e": MouseEventSource.ResizeBox_E,
            "s": MouseEventSource.ResizeBox_S,
            "w": MouseEventSource.ResizeBox_W
        };

        constructor(renderer: DocumentRenderer) {
            this.renderer = renderer;

            this.element = document.createElement("DIV");
            this.element.className = ELEMENT_CLASSNAME;
            this.element.appendChild(this.createCornerElement("nw"));
            this.element.appendChild(this.createCornerElement("ne"));
            this.element.appendChild(this.createCornerElement("se"));
            this.element.appendChild(this.createCornerElement("sw"));
            this.element.appendChild(this.createCornerElement("n"));
            this.element.appendChild(this.createCornerElement("e"));
            this.element.appendChild(this.createCornerElement("s"));
            this.element.appendChild(this.createCornerElement("w"));
        }

        public show(pageIndex: number, layoutItem: LayoutItemBase, hyperlinkTip: string) {
            this.element.style.left = layoutItem.x + "px";
            this.element.style.top = layoutItem.y + "px";
            this.element.style.width = layoutItem.width + "px";
            this.element.style.height = layoutItem.height + "px";
            this.element.title = hyperlinkTip === undefined ? "" : HtmlConverter.buildHyperlinkTipString(hyperlinkTip);

            this.renderer.cache[pageIndex].page.appendChild(this.element);
        }

        public hide() {
            var parentNode: Node = this.element.parentNode;
            if (parentNode)
                parentNode.removeChild(this.element);
        }

        public static getMouseEventSource(source: HTMLElement): MouseEventSource {
            var className: string = source.className;
            if (className.indexOf(CORNER_CLASSNAME_PREFIX) != 0)
                return MouseEventSource.Undefined;

            var direction: string = className.split(" ")[1];
            return CanvasResizeBoxHelper.directionToSource[direction];
        }

        private createCornerElement(direction: string): HTMLElement {
            var corner: HTMLElement = document.createElement("DIV");
            corner.className = CORNER_CLASSNAME_PREFIX + (ASPx.Browser.TouchUI ? CORNERTOUCH_CLASSNAME_POSTFIX : "") + " " + direction;
            return corner;
        }
    }
} 