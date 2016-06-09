module __aspxRichEdit {
    export class MeasureInfo {
        text: string;
        charProp: CharacterProperties;
        isText: boolean;

        resultWidth: number;
        resultHeight: number;

        // isText set true when box.type == LayoutBoxType.Text
        constructor(text: string, charProp: CharacterProperties, isText: boolean) {
            this.text = text;
            this.charProp = charProp;
            this.isText = isText;
        }
    }

    export class Measurer implements IBoxMeasurer {
        private measureContainer: HTMLElement;
        private cache: { [key: string]: CacheValue } = {};

        public constructor(measureContainer: HTMLElement) {
            this.measureContainer = measureContainer;
        }

        public measure(measureInfos: MeasureInfo[]) {
            var info: MeasureInfo,
                htmlParts: string[] = [],
                i: number,
                extCache: { [key: string]: number } = {},
                sbInfo: MeasureInfo,
                sbInfoCacheKey: string,
                sbCacheValue: CacheValue,
                cacheKeys: string[] = [];
            
            for (i = 0; info = measureInfos[i]; i++) {
                var cacheKey: string = this.generateCacheKey(info);
                cacheKeys.push(cacheKey);
                if (!this.tryApplyCachedValue(cacheKey, info)) {
                    this.registerLayoutBoxInfo(cacheKey, extCache, info, htmlParts);
                    if (info.charProp.script !== CharacterFormattingScript.Normal) {
                        sbInfo = this.createSpaceMeasureInfo(info);
                        sbInfoCacheKey = this.generateCacheKey(sbInfo);
                        if (this.cache[sbInfoCacheKey] === undefined && extCache[sbInfoCacheKey] === undefined)
                            this.registerLayoutBoxInfo(sbInfoCacheKey, extCache, sbInfo, htmlParts);
                    }
                }
            }

            if (!htmlParts.length)
                return;
            this.measureContainer.innerHTML = htmlParts.join("");
            var nodes: NodeList = this.measureContainer.childNodes;
            for (i = 0; info = measureInfos[i]; i++) {
                if (info.resultWidth !== undefined && info.resultHeight !== undefined)
                    continue;
                var cacheKey: string = cacheKeys[i];
                var cacheValue: CacheValue = this.cache[cacheKey];
                if (cacheValue) {
                    info.resultWidth = cacheValue.width;
                    info.resultHeight = cacheValue.height;
                    continue;
                }

                var node = <HTMLElement>nodes[extCache[cacheKey]];
                if (info.charProp.script === CharacterFormattingScript.Normal)
                    this.applyOffsetSizes(cacheKey, info, node, undefined);
                else {
                    sbInfo = this.createSpaceMeasureInfo(info);
                    sbInfoCacheKey = this.generateCacheKey(sbInfo);
                    sbCacheValue = this.cache[sbInfoCacheKey];
                    if (!sbCacheValue)
                        this.applyOffsetSizes(sbInfoCacheKey, sbInfo, <HTMLElement>nodes[extCache[sbInfoCacheKey]], undefined);
                    this.applyOffsetSizes(cacheKey, info, node, sbCacheValue ? sbCacheValue.height : sbInfo.resultHeight);
                }
            }
        }

        private generateCacheKey(info: MeasureInfo): string {
            return info.text + "&&&" + HtmlConverter.getSizeSignificantCssString(info.charProp);
        }
        private tryApplyCachedValue(cacheKey: string, info: MeasureInfo): boolean {
            var cacheValue: CacheValue = this.cache[cacheKey];
            if (cacheValue) {
                info.resultWidth = cacheValue.width;
                info.resultHeight = cacheValue.height;
                return true;
            }
            return false;
        }
        private registerLayoutBoxInfo(cacheKey: string, extCache: { [key: string]: number }, info: MeasureInfo, htmlParts: string[]) {
            if (extCache[cacheKey] !== undefined)
                return;
            extCache[cacheKey] = htmlParts.length;
            htmlParts.push(this.createMeasureNodeHTML(info));
        }
        // public only for tests
        public createMeasureNodeHTML(info: MeasureInfo): string {
            var text: string = info.isText ? Utils.encodeHtml(info.text) : info.text;
            text = text.replace(/\&nbsp;/gi, " ");
            return "<pre style=\"" + HtmlConverter.getSizeSignificantCssString(info.charProp) + "\">" + text + "</pre>";
        }
        private createSpaceMeasureInfo(info: MeasureInfo): MeasureInfo {
            var newCharProp: CharacterProperties = info.charProp.clone();
            newCharProp.script = CharacterFormattingScript.Normal;
            return new MeasureInfo(" ", newCharProp, true);
        }
        // public only for tests
        public applyOffsetSizes(cacheKey: string, info: MeasureInfo, node: HTMLElement, precalculatedHeight: number) {
            if (ASPx.Browser.IE && ASPx.Browser.MajorVersion < 9) {
                info.resultWidth = node.offsetWidth;
                info.resultHeight = precalculatedHeight === undefined ? node.offsetHeight : precalculatedHeight;
            }
            else {
                var style = document.defaultView.getComputedStyle(node, null);
                info.resultWidth = parseFloat(style.width);
                info.resultHeight = precalculatedHeight === undefined ? parseFloat(style.height) : precalculatedHeight;
            }
            this.cache[cacheKey] = new CacheValue(info.resultWidth, info.resultHeight);
        }
    }

    export class CacheValue {
        width: number;
        height: number;
        constructor(width: number, height: number) {
            this.width = width;
            this.height = height;
        }
    }
}