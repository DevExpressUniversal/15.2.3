module __aspxRichEdit {
    //module Loupe {
        export class Loupe {
            private LOUPE_WIDTH = 100;
            private LOUPE_HEIGHT = 100;

            private loupeWrapper: LoupeWrapper = null;
            private loupe: LoupeElement = null;

            constructor(parent: Node) {
                this.loupeWrapper = new LoupeWrapper(parent, this.LOUPE_WIDTH, this.LOUPE_HEIGHT);
                this.loupe = new LoupeElement(this.loupeWrapper, this.LOUPE_WIDTH, this.LOUPE_HEIGHT);
            }

            private setPosition(evt: RichMouseEvent) {
                this.loupeWrapper.setPosition(evt.evtX, evt.evtY);
                this.loupe.setPosition(evt.evtX, evt.evtY);
            }
            show(evt: RichMouseEvent) {
                this.setPosition(evt);
                this.loupeWrapper.show();
            }
            move(evt: RichMouseEvent) {
                this.setPosition(evt);
            }
            hide() {
                this.loupeWrapper.hide();
            }
        }

        class LoupeWrapper {
            private LOUPE_WRAPPER_CLASS_NAME = "dxre-loupeWrapper";
            private LOUPE_MARGIN_TO_CURSOR = 20;

            private loupeWrapper: HTMLElement = null;
            private height: number;
            private width: number;

            constructor(parent: Node, width: number, height: number) {
                this.width = width;
                this.height = height;
                this.initialize(parent);
            }

            private initialize(parent: Node) {
                this.createWrapper(parent);
                this.prepareWrapper();
            }
            private createWrapper(parent: Node): void {
                this.loupeWrapper = document.createElement("DIV");
                parent.appendChild(this.loupeWrapper);
            }
            private prepareWrapper(): void {
                this.loupeWrapper.className = this.LOUPE_WRAPPER_CLASS_NAME;
                this.loupeWrapper.style.width = this.width + "px";
                this.loupeWrapper.style.height = this.height + "px";
            }
            setPosition(x: number, y: number) {
                this.loupeWrapper.style.top = y - this.height - this.LOUPE_MARGIN_TO_CURSOR + "px";
                this.loupeWrapper.style.left = x - (this.width / 2) + "px";
            }
            appendToWrapper(content: Node) { //this method does not need - refactoring and remove  it!
                this.loupeWrapper.appendChild(content);
            }
            show() {
                this.loupeWrapper.style.visibility = "visible";
            }
            hide() {
                //this.loupeWrapper.style.visibility = "hidden";
            }
        }

        class LoupeElement {
            private LOUPE_ELEMENT_CLASS_NAME = "dxre-loupe";
            private LOUPE_SCALE = 1;

            private wrapper: LoupeWrapper = null;
            private loupe: SVGSVGElement = null;
            private foreignObject: Node = null;
            private loupeContent: Node = null;
            private height: number;
            private width: number;

            constructor(wrapper: LoupeWrapper, width: number, height: number) {
                this.wrapper = wrapper;
                this.width = width;
                this.height = height;
                this.initialize();
            }

            private initialize() {
                this.createLoupe();
                this.prepareLoupe();
            }
            private createLoupe() {
                this.loupe = <SVGSVGElement>document.createElementNS('http://www.w3.org/2000/svg', 'svg');
                this.wrapper.appendToWrapper(this.loupe);
                this.createForeignObject();
                this.prepareForeignObject();
            }
            private prepareLoupe() {
                this.loupe.setAttribute("xmlns", "http://www.w3.org/2000/svg");
                this.loupe.setAttribute("class", this.LOUPE_ELEMENT_CLASS_NAME);
                this.loupe.setAttribute("width",(this.width / 2) + "px");
                this.loupe.setAttribute("height",(this.height / 2) + "px");
                this.setScale();
            }
            private createForeignObject() {
                this.foreignObject = document.createElementNS('http://www.w3.org/2000/svg', 'foreignObject');
                this.loupe.appendChild(this.foreignObject);
                this.createLoupeContentElement();
            }
            private prepareForeignObject() {
                (<HTMLElement>this.foreignObject).setAttribute("x", "0");
                (<HTMLElement>this.foreignObject).setAttribute("y", "-" + (this.height / 4)); //TODO =)))
                (<HTMLElement>this.foreignObject).setAttribute("width", this.width + "px");
                (<HTMLElement>this.foreignObject).setAttribute("height",(this.height / 2) + "px");
            }
            private createLoupeContentElement() {
                this.loupeContent = document.createElementNS("http://www.w3.org/1999/xhtml", "div");
                (<HTMLElement>this.loupeContent).setAttribute("xmlns", "http://www.w3.org/1999/xhtml");
                this.foreignObject.appendChild(this.loupeContent);
            }
            private setScale() {
                this.loupe.setAttribute("style", this.getTransform() + ": scale(" + this.LOUPE_SCALE + ".0);");
            }
            private getTransform(): string {
                if (ASPx.Browser.WebKitFamily)
                    return "-webkit-transform";
                else if (ASPx.Browser.Firefox)
                    return "-moz-transform";
                else if (ASPx.Browser.IE && ASPx.Browser.MajorVersion < 10)
                    return "-ms-transform";
                return "transform";
            }
            private clearLoupeContent() {
                this.foreignObject.removeChild(this.foreignObject.childNodes[0]);
                this.createLoupeContentElement();
            }
            private setLoupeContentPosition(x: number, y: number) {

                //(<SVGSVGElement>this.loupe).style.top = this.loupeWrapper.clientTop + "px";
                //(<SVGSVGElement>this.loupe).style.left = this.loupeWrapper.clientLeft + "px";
            }
            private render(x: number, y: number) {
                this.clearLoupeContent();
                var words: HTMLElement[][] = this.getElementsFromPoint(x, y);
                for (var i = 0; i < words.length; i++) {
                    if (words[i]) {
                        for (var k = 0; k < words[i].length; k++) {
                            var node: HTMLElement = <HTMLElement>words[i][k].cloneNode();
                            node.innerText = (<HTMLElement>words[i][k]).innerText;
                            this.loupeContent.appendChild(node);
                        }
                        //var br = document.createElement("BR");
                        //this.loupeContent.appendChild(br);
                    }
                }
            }
            private getElementsFromPoint(x: number, y: number): HTMLElement[][] {
                var elem = document.elementFromPoint(x, y);
                if (elem && elem.tagName == "SPAN")
                    return this.getLineUnderCursor(elem);
                else
                    return [];
            }
            private getLineUnderCursor(elemUnderCursor): HTMLElement[][] {
                var elem = elemUnderCursor.parentNode;
                var prevElem = this.clearStyles(elem.previousElementSibling);
                var nextElem = this.clearStyles(elem.nextElementSibling);
                var data = [prevElem, this.clearStyles(elem), nextElem];

                return [this.clearStyles(elem)]; //TODO: DEBUG
                return data;
            }
            private clearStyles(htmlHeap): HTMLElement[] {
                if (htmlHeap) {
                    var elements = htmlHeap.childNodes;
                    for (var i = 0; i < elements.length; i++) {
                        elements[i].style.top = "";
                        elements[i].style.left = "";
                        elements[i].position = "static";
                        elements[i].removeAttribute("class");
                    }
                    return elements;
                }
            }
            setPosition(x: number, y: number) {
                this.render(x, y);
                this.setLoupeContentPosition(x, y);
            }
        }
   // }
}