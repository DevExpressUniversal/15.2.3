module __aspxRichEdit {
    export class FontInfo implements ISupportCopyFrom<FontInfo>, IEquatable<FontInfo>, ICloneable<FontInfo> {
        name: string;
        scriptMultiplier: number;
        cssString: string;
        index: number;
        canBeSet: boolean;
        measurer: IFontMeasurer;
        subScriptOffset: number;

        private baseLine: number;

        constructor(name?: string) {
            this.name = name;
            this.cssString = name;
        }

        copyFrom(obj: FontInfo) {
            this.name = obj.name;
            this.scriptMultiplier = obj.scriptMultiplier;
            this.canBeSet = obj.canBeSet;
            this.cssString = obj.cssString;
            this.subScriptOffset = obj.subScriptOffset;
        }

        equals(obj: FontInfo) {
            return obj && this.name == obj.name &&
                this.scriptMultiplier == obj.scriptMultiplier;
        }

        static equalsBinary(fontInfoA: FontInfo, fontInfoB: FontInfo) {
            return fontInfoA && fontInfoB &&
                fontInfoA.name == fontInfoB.name &&
                fontInfoA.scriptMultiplier == fontInfoB.scriptMultiplier;
        }

        clone(): FontInfo {
            var obj = new FontInfo(null);
            obj.copyFrom(this);
            return obj;
        }

        getBaseLine(): number {
            if(this.baseLine === undefined)
                this.baseLine = this.measurer.getBaseLine(this);
            return this.baseLine;
        }
        getAscent(boxHeight: number): number {
            return this.getBaseLine() * boxHeight;
        }
        getDescent(boxHeight: number): number {
            return boxHeight - this.getAscent(boxHeight);
        }
    }

    export interface IFontMeasurer {
        getBaseLine(font: FontInfo): number;
    }

    export class FontMeasurer implements IFontMeasurer {
        private container: HTMLDivElement;

        getBaseLine(font: FontInfo): number {
            this.beginMeasuring(font);
            var baseLine = (<HTMLSpanElement>this.container.firstChild).offsetTop / (<HTMLSpanElement>this.container.lastChild).offsetHeight;
            this.endMeasuring();
            return baseLine;
        }

        private beginMeasuring(font: FontInfo): HTMLDivElement {
            if(!this.container) {
                this.container = document.createElement("div");
                this.container.style.position = "absolute";
                this.container.style.top = "-10000px";
                this.container.style.left = "-10000px";
                this.container.style.opacity = "0";
                this.container.style.fontSize = "0px";
            }
            this.container.innerHTML = '<span style="font-size:0; font-family: ' + font.cssString + '; display: inline-block;">A</span><span style="font-size:288pt; font-family: ' + font.cssString + '; display: inline-block;">A</span>';
            document.body.appendChild(this.container);
            return this.container;
        }
        private endMeasuring() {
            if(this.container && this.container.parentNode)
                this.container.parentNode.removeChild(this.container);
        }
    }
}