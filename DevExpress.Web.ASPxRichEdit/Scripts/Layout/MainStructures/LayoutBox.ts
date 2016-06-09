module __aspxRichEdit {
    export enum LayoutBoxType {
        Text, // visible
        Space, // visible
        Dash, // visible
        TabSpace, // visible
        LineBreak, // invisible
        PageBreak, // invisible
        ColumnBreak, // invisible
        SectionMark, // invisible
        ParagraphMark, // invisible
        Picture, // visible
        NumberingList,
        FieldCodeStart, // visible
        FieldCodeEnd, // visible
        FieldResultEnd, // never used in layout. But we need in this type
        BookmarkStartBox, // visible
        BookmarkEndBox, // visible
        LayoutDependent //visible
    }

    export class LayoutBox extends LayoutItemBase implements ICloneable<LayoutBox>, ISupportCopyFrom<LayoutBox> {
        //LayoutItemBase.width - box width. Not matter what text in box
        public characterProperties: CharacterProperties;
        public rowOffset: number;

        // please. Don't initialize that two members. After using this field need delete that.
        public hyperlinkTip: string; 
        public fieldLevel: number; // not defined or [1, 2...]. If defined then mean that box located in some field code part. Include code part borders.
         
        constructor(characterProperties: CharacterProperties) {
            super();
            this.characterProperties = characterProperties;
        }

        public clone(): LayoutBox {
            return this.virtualClone();
        }
        
        public equals(obj: LayoutBox): boolean {
            return super.equals(obj) &&
                this.rowOffset == obj.rowOffset &&
                this.hyperlinkTip == obj.hyperlinkTip &&
                this.fieldLevel == this.fieldLevel;
        }

        public copyFrom(obj: LayoutBox) {
            super.copyFrom(obj);
            //this.characterProperties = obj.characterProperties; // not need. set in constructor;
            this.rowOffset = obj.rowOffset;
            if (obj.hyperlinkTip)
                this.hyperlinkTip = obj.hyperlinkTip;
            if (obj.fieldLevel)
                this.fieldLevel = obj.fieldLevel;
        }

        protected virtualClone(): LayoutBox {
            throw new Error(Errors.NotImplemented);
        }

        static initializeWithMeasurer(initBoxes: LayoutBox[], measurer: IBoxMeasurer, showHiddenSymbols: boolean) {
            var widthHeightInfo: MeasureInfo[] = [];
            for (var i: number = 0, box: LayoutBox; box = initBoxes[i]; i++)
                box.pushInfoForMeasure(widthHeightInfo, showHiddenSymbols);

            measurer.measure(widthHeightInfo);

            for (var i: number = initBoxes.length - 1, box: LayoutBox; box = initBoxes[i]; i--)
                box.popInfoForMeasure(widthHeightInfo, showHiddenSymbols);

            if (widthHeightInfo.length != 0)
                throw new Error("In initializeWithMeasurer widthHeightInfo.length != 0" + widthHeightInfo.length);
        }

        // next 2 methods use for get some info from measure
        public pushInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) {
            throw new Error(Errors.NotImplemented);
        }

        public popInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) {
            throw new Error(Errors.NotImplemented);
        }

        public getEndPosition(): number {
            return this.rowOffset + this.getLength();
        }

        public getAscent(): number {
            return this.characterProperties.fontInfo.getAscent(this.height);
        }

        public getDescent(): number {
            return this.characterProperties.fontInfo.getDescent(this.height);
        }

        public getType(): LayoutBoxType {
            throw new Error(Errors.NotImplemented);
        }

        public isVisibleForRowAlign(): boolean {
            return false;
        }

        public isVisible(): boolean {
            return false;
        }

        public isWhitespace(): boolean {
            throw new Error(Errors.NotImplemented);
        }

        public getCharOffsetXInPixels(measurer: IBoxMeasurer, charOffset: number): number {
            return charOffset <= 0 ? 0 : this.width;
        }

        public getLength(): number {
            return 1;
        }

        public calculateCharOffsetByPointX(measurer: IBoxMeasurer, pointX: number): number {
            return pointX / this.width > 0.6 ? 1 : 0;
        }

        // this box length decrease. Return new(next) box.
        public splitByWidth(measurer: IBoxMeasurer, maxWidth: number, leaveAtLeastOneChar: boolean): { isCurrBoxWidthOk: boolean; nextBox: LayoutBox } {
            if (this.width <= maxWidth || leaveAtLeastOneChar)
                return { isCurrBoxWidthOk: true, nextBox: null };
            return { isCurrBoxWidthOk: false, nextBox: null };
        }

        public getCharIndex(char: string): number {
            return -1;
        }

        public splitBoxByPosition(measurer: IBoxMeasurer, offsetAtStartBox: number): LayoutBox {
            return null;
        }

        public renderGetContent(constants: any, handlerURI: string, emptyImageCacheId: number): string {
            throw new Error(Errors.NotImplemented);
        }

        public renderNoStrikeoutAndNoUnderlineIfBoxInEndRow() {
            return false;
        }

        public renderIsWordBox(): boolean {
            return false;
        }

        public renderGetCharacterProperties(): CharacterProperties {
            return this.characterProperties;
        }
    }

    export class LayoutTextBox extends LayoutBox implements ISupportCopyFrom<LayoutTextBox> {
        text: string;

        constructor(characterProperties: CharacterProperties, text: string) {
            super(characterProperties);
            this.text = text;
        }

        public equals(obj: LayoutTextBox): boolean {
            return super.equals(obj) &&
                this.text == obj.text;
        }

        protected virtualClone(): LayoutBox {
            const newObj: LayoutTextBox = new LayoutTextBox(this.characterProperties, this.text);
            newObj.copyFrom(this);
            return newObj;
        }

        public copyFrom(obj: LayoutTextBox) {
            super.copyFrom(obj);
        }

        public getType(): LayoutBoxType {
            return LayoutBoxType.Text;
        }

        public pushInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) {
            info.push(new MeasureInfo(this.text, this.characterProperties, true));
        }

        public popInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) {
            var elem: MeasureInfo = info.pop();
            this.width = elem.resultWidth;
            this.height = elem.resultHeight;
        }

        public isVisible(): boolean {
            return true;
        }

        public isVisibleForRowAlign(): boolean {
            return true;
        }

        public getLength(): number {
            return this.text.length;
        }

        public getCharOffsetXInPixels(measurer: IBoxMeasurer, charOffset: number): number {
            if (charOffset == 0)
                return 0;
            else if (charOffset === this.getLength())
                return this.width;
            else {
                var info: MeasureInfo = new MeasureInfo(this.text.substr(0, charOffset), this.characterProperties, true);
                measurer.measure([info]);
                return info.resultWidth;
            }
        }

        public calculateCharOffsetByPointX(measurer: IBoxMeasurer, pointX: number): number {
            const boxRanges: MeasureInfo[] = this.getBoxRanges(measurer);
            const boxRangesLength: number = boxRanges.length;

            for (var i: number = 0; i < boxRangesLength; i++) {
                if (boxRanges[i].resultWidth > pointX) {
                    var letterWidth: number = i > 0 ? boxRanges[i].resultWidth - boxRanges[i - 1].resultWidth : boxRanges[0].resultWidth;
                    var clickPointOffset: number = pointX - (i > 0 ? boxRanges[i - 1].resultWidth : 0);
                    if (clickPointOffset / letterWidth > 0.6)
                        i++;
                    break;
                }
            }
            return i;
        }

        public splitByWidth(measurer: IBoxMeasurer, maxWidth: number, leaveAtLeastOneChar: boolean): { isCurrBoxWidthOk: boolean; nextBox: LayoutBox } {
            if (this.getLength() > 1) {
                var boxRanges: MeasureInfo[] = this.getBoxRanges(measurer);
                for (let i: number = boxRanges.length - 1, info: MeasureInfo; info = boxRanges[i]; i--)
                    if (info.resultWidth <= maxWidth || i == 0 && leaveAtLeastOneChar) {
                        const nextBox: LayoutBox = this.createBoxThisType(this.text.substr(info.text.length));
                        this.text = this.text.substr(0, info.text.length);
                        LayoutBox.initializeWithMeasurer([this, nextBox], measurer, undefined);
                        return { isCurrBoxWidthOk: true, nextBox: nextBox };
                    }
                return { isCurrBoxWidthOk: false, nextBox: null };
            }
            else
                return { isCurrBoxWidthOk: this.width <= maxWidth || leaveAtLeastOneChar, nextBox: null };
            
        }

        public splitBoxByPosition(measurer: IBoxMeasurer, offsetAtStartBox: number): LayoutBox {
            const nextBox: LayoutBox = this.createBoxThisType(this.text.substr(offsetAtStartBox));
            this.text = this.text.substr(0, offsetAtStartBox);
            nextBox.rowOffset = this.getEndPosition();
            LayoutBox.initializeWithMeasurer([this, nextBox], measurer, undefined);
            return nextBox;
        }

        public getCharIndex(char: string): number {
            return this.text.indexOf(char);
        }

        public renderGetContent(constants: any, handlerURI: string, emptyImageCacheId: number): string {
            return Utils.encodeHtml(this.text);
        }

        public renderIsWordBox(): boolean {
            return true;
        }

        public isWhitespace(): boolean {
            return false;
        }

        // private and only for for text and dash
        public createBoxThisType(text: string): LayoutBox {
            return new LayoutTextBox(this.characterProperties, text);
        }

        private getBoxRanges(measurer: IBoxMeasurer): MeasureInfo[] {
            var measureInfos: MeasureInfo[] = [];
            var textLength = this.text.length;
            for (var i: number = 1; i <= textLength; i++)
                measureInfos.push(new MeasureInfo(this.text.substr(0, i), this.characterProperties, true));
            measurer.measure(measureInfos);
            return measureInfos;
        }
    }

    export class LayoutDashBox extends LayoutTextBox implements ISupportCopyFrom<LayoutDashBox> {
        public getType(): LayoutBoxType {
            return LayoutBoxType.Dash;
        }

        protected virtualClone(): LayoutBox {
            const newObj: LayoutDashBox = new LayoutDashBox(this.characterProperties, this.text);
            newObj.copyFrom(this);
            return newObj;
        }

        public copyFrom(obj: LayoutDashBox) {
            super.copyFrom(obj);
        }

        public renderGetContent(constants: any, handlerURI: string, emptyImageCacheId: number): string {
            return this.text;
        }

        // private and only for for text and dash
        public createBoxThisType(text: string): LayoutBox {
            return new LayoutDashBox(this.characterProperties, text);
        }

        public isWhitespace(): boolean {
            return false;
        }
    }

    export class LayoutDependentTextBox extends LayoutTextBox implements ISupportCopyFrom<LayoutDependentTextBox> {
        getText: (layoutFormatter: LayoutFormatterBase) => string;

        protected virtualClone(): LayoutBox {
            const newObj: LayoutDependentTextBox = new LayoutDependentTextBox(this.characterProperties, this.text);
            newObj.copyFrom(this);
            return newObj;
        }
        
        public copyFrom(obj: LayoutDependentTextBox) {
            super.copyFrom(obj);
            this.getText = obj.getText;
        }

        public getType(): LayoutBoxType {
            return LayoutBoxType.LayoutDependent;
        }

        public calculateText(layoutFormatter: LayoutFormatterBase) {
            if(this.getText)
                this.text = this.getText(layoutFormatter);
        }
    }

    export class LayoutPictureBox extends LayoutBox implements ISupportCopyFrom<LayoutPictureBox> {
        id: any;
        isLoaded: boolean;

        constructor(characterProperties: CharacterProperties, id: any, width: number, height: number, isLoaded: boolean = true) {
            super(characterProperties);
            this.id = id;
            this.width = width;
            this.height = height;
            this.isLoaded = isLoaded;
        }

        public equals(obj: LayoutPictureBox): boolean {
            return super.equals(obj) &&
                this.id == obj.id &&
                this.isLoaded == obj.isLoaded;
        }

        protected virtualClone(): LayoutBox {
            const newObj: LayoutPictureBox = new LayoutPictureBox(this.characterProperties, this.id, this.width, this.height, this.isLoaded);
            newObj.copyFrom(this);
            return newObj;
        }

        public copyFrom(obj: LayoutPictureBox) {
            super.copyFrom(obj);
        }

        public getType(): LayoutBoxType {
            return LayoutBoxType.Picture;
        }

        public pushInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) { }

        public popInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) { }

        public getAscent(): number {
            return this.height;
        }

        public getDescent(): number {
            return 0;
        }

        public isVisible(): boolean {
            return true;
        }

        public isVisibleForRowAlign(): boolean {
            return true;
        }

        // the same code in DocumentRenderer.renderPicture
        public renderGetContent(constants: any, handlerURI: string, emptyImageCacheId: number): string {
            var style = "height: " + this.height + "px; width: " + this.width + "px;";
            if(this.id == emptyImageCacheId) {
                var innerContent = "";
                style += "display: inline-block; box-sizing: border-box; -moz-box-sizing: border-box; -webkit-box-sizing: border-box; border: 1px dashed";
                if(this.isLoaded) {
                    style += " red; font: 10px Arial; color: red; text-align: center; line-height: " + this.height + "px;";
                    innerContent = "Error"; //TODO
                }
                else 
                    style += " gray; background: url('" + constants.IMAGELOADINGURL + "') no-repeat center;";
                return "<span style=\"" + style +"\">" + innerContent + "</span>";
            }
            style += " vertical-align: baseline";
            return "<img src=\"" + handlerURI + "&img=" + this.id + "\" style=\"" + style + "\" class=\"" + DocumentRenderer.CLASSNAMES.PICTURE + "\" />";
        }

        public isWhitespace(): boolean {
            return false;
        }
    }

    export class LayoutParagraphMarkBox extends LayoutBox implements ISupportCopyFrom<LayoutParagraphMarkBox> {
        paragraphMarkSymbol: string;
        //this.width - width Utils.specialCharacters.HiddenParagraphMark

        public equals(obj: LayoutParagraphMarkBox): boolean {
            return super.equals(obj) &&
                this.paragraphMarkSymbol == obj.paragraphMarkSymbol;
        }

        protected virtualClone(): LayoutBox {
            const newObj: LayoutParagraphMarkBox = new LayoutParagraphMarkBox(this.characterProperties);
            newObj.copyFrom(this);
            return newObj;
        }

        public copyFrom(obj: LayoutParagraphMarkBox) {
            super.copyFrom(obj);
            this.paragraphMarkSymbol = obj.paragraphMarkSymbol;
        }

        public getType(): LayoutBoxType {
            return LayoutBoxType.ParagraphMark;
        }

        public pushInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) {
            info.push(new MeasureInfo(Utils.specialCharacters.HiddenParagraphMark, this.characterProperties, false));
        }

        public popInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) {
            if (showHiddenSymbols)
                this.paragraphMarkSymbol = Utils.specialCharacters.HiddenParagraphMark;
            else
                this.paragraphMarkSymbol = "&nbsp;";

            var elem: MeasureInfo = info.pop();
            this.width = elem.resultWidth;
            this.height = elem.resultHeight;
        }

        public renderGetContent(constants: any, handlerURI: string, emptyImageCacheId: number): string {
            return this.paragraphMarkSymbol;
        }

        public renderNoStrikeoutAndNoUnderlineIfBoxInEndRow() {
            return true;
        }

        public isWhitespace(): boolean {
            return false;
        }
    }
    
    export class LayoutSpaceBox extends LayoutBox implements ISupportCopyFrom<LayoutSpaceBox> {
        spaceWidth: number; // width  "&nbsp;"
        hiddenSpaceWidth: number; // width Utils.specialCharacters.HiddenSpace can be = 0 in case showHiddenSymbols = false

        public equals(obj: LayoutSpaceBox): boolean {
            return super.equals(obj) &&
                this.spaceWidth == obj.spaceWidth &&
                this.hiddenSpaceWidth == obj.hiddenSpaceWidth;
        }

        protected virtualClone(): LayoutBox {
            const newObj: LayoutSpaceBox = new LayoutSpaceBox(this.characterProperties);
            newObj.copyFrom(this);
            return newObj;
        }

        public copyFrom(obj: LayoutSpaceBox) {
            super.copyFrom(obj);
            this.spaceWidth = obj.spaceWidth;
            this.hiddenSpaceWidth = obj.hiddenSpaceWidth;
        }

        public getType(): LayoutBoxType {
            return LayoutBoxType.Space;
        }

        public pushInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) {
            if (showHiddenSymbols) {
                info.push(new MeasureInfo(Utils.specialCharacters.HiddenSpace, this.characterProperties, false));
                info.push(new MeasureInfo("&nbsp;", this.characterProperties, false));
            }
            else
                info.push(new MeasureInfo("&nbsp;", this.characterProperties, false));
        }

        public popInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) {
            if (showHiddenSymbols) {
                var elem: MeasureInfo = info.pop();
                this.width = elem.resultWidth;
                this.height = elem.resultHeight;
                this.spaceWidth = elem.resultWidth;

                elem = info.pop();
                this.hiddenSpaceWidth = elem.resultWidth;
            }
            else {
                var elem: MeasureInfo = info.pop();
                this.width = elem.resultWidth;
                this.height = elem.resultHeight;

                this.spaceWidth = elem.resultWidth;
                this.hiddenSpaceWidth = 0;
            }
        }

        public isVisible(): boolean {
            return true;
        }

        public renderGetContent(constants: any, handlerURI: string, emptyImageCacheId: number): string {
            if (this.hiddenSpaceWidth > 0) { // show hiddenSymbols
                var numNbsps: number = Math.ceil((this.width - this.hiddenSpaceWidth) / this.spaceWidth);
                return Utils.specialCharacters.HiddenSpace + Utils.mergeStringNTimes("&nbsp;", numNbsps);
            }
            else {
                var numNbsps: number = Math.ceil(this.width / this.spaceWidth);
                return Utils.mergeStringNTimes("&nbsp;", numNbsps);
            }
        }

        public renderNoStrikeoutAndNoUnderlineIfBoxInEndRow() {
            return true;
        }

        public isWhitespace(): boolean {
            return true;
        }
    }

    export class LayoutTabSpaceBox extends LayoutBox implements ISupportCopyFrom<LayoutTabSpaceBox> {
        spaceWidth: number; // width  "&nbsp;"
        hiddenTabWidth: number; // width Utils.specialCharacters.HiddenTabSpace can be = 0 in case showHiddenSymbols = false

        public equals(obj: LayoutTabSpaceBox): boolean {
            return super.equals(obj) &&
                this.spaceWidth == obj.spaceWidth &&
                this.hiddenTabWidth == obj.hiddenTabWidth;
        }

        protected virtualClone(): LayoutBox {
            const newObj: LayoutTabSpaceBox = new LayoutTabSpaceBox(this.characterProperties);
            newObj.copyFrom(this);
            return newObj;
        }

        public copyFrom(obj: LayoutTabSpaceBox) {
            super.copyFrom(obj);
            this.spaceWidth = obj.spaceWidth;
            this.hiddenTabWidth = obj.hiddenTabWidth;
        }

        public getType(): LayoutBoxType {
            return LayoutBoxType.TabSpace;
        }

        public pushInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) {
            // for formatter not matter height, width, etc, it only need for renderer
            if (showHiddenSymbols) {
                var charProps: CharacterProperties = this.renderGetCharacterProperties();
                info.push(new MeasureInfo(Utils.specialCharacters.HiddenTabSpace, charProps, false));
                info.push(new MeasureInfo("&nbsp;", charProps, false));
            }
            else
                info.push(new MeasureInfo("&nbsp;", this.characterProperties, false));
        }

        public popInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) {
            if (showHiddenSymbols) {
                var elem: MeasureInfo = info.pop();
                this.spaceWidth = elem.resultWidth;

                elem = info.pop();
                this.hiddenTabWidth = elem.resultWidth;

                this.width = elem.resultWidth;
                this.height = elem.resultHeight;
            }
            else {
                var elem: MeasureInfo = info.pop();

                this.spaceWidth = elem.resultWidth;
                this.hiddenTabWidth = 0;

                this.width = elem.resultWidth;
                this.height = elem.resultHeight;
            }
        }

        public isVisible(): boolean {
            return true;
        }

        public renderGetContent(constants: any, handlerURI: string, emptyImageCacheId: number): string {
            if (this.hiddenTabWidth > 0) { // show hiddenSymbols
                var nbspsSpace: number = this.width - this.hiddenTabWidth;

                var nbspsSpaceLeftPart: number = nbspsSpace < 0 ? 0 : nbspsSpace / 2;
                var numNbspsLeftPart: number = Math.floor(nbspsSpaceLeftPart / this.spaceWidth);

                var nbspsSpaceRightPart: number = this.width - (numNbspsLeftPart * this.spaceWidth + this.hiddenTabWidth);
                var numNbspsRightPart: number = nbspsSpaceRightPart < 0 ? 0 : Math.ceil(nbspsSpaceRightPart / this.spaceWidth);

                return Utils.mergeStringNTimes("&nbsp;", numNbspsLeftPart) + Utils.specialCharacters.HiddenTabSpace + Utils.mergeStringNTimes("&nbsp;", numNbspsRightPart);
            }
            else {
                var numNbsps: number = Math.ceil(this.width / this.spaceWidth);
                return Utils.mergeStringNTimes("&nbsp;", numNbsps);
            }
        }

        public renderGetCharacterProperties(): CharacterProperties {
            if (this.characterProperties.fontBold || this.characterProperties.fontItalic || this.characterProperties.foreColor) {
                var renderCharProps: CharacterProperties = this.characterProperties.clone();
                renderCharProps.fontBold = false;
                renderCharProps.fontItalic = false;
                if (renderCharProps.foreColor) { // if not set underline|strikeout color, then he have color = foreColor
                    if (!renderCharProps.underlineColor)
                        renderCharProps.underlineColor = renderCharProps.foreColor;
                    if (!renderCharProps.strikeoutColor)
                        renderCharProps.strikeoutColor = renderCharProps.foreColor;
                    renderCharProps.foreColor = 0;
                }
                return renderCharProps;
            }
            else
                return this.characterProperties;
        }

        public isWhitespace(): boolean {
            return true;
        }
    }

    export class LayoutLineBreakBox extends LayoutBox implements ISupportCopyFrom<LayoutLineBreakBox> {
        lineBreakSymbol: string; //"&nbsp;" or Utils.specialCharacters.HiddenLineBreak, this.width corresponding

        public equals(obj: LayoutLineBreakBox): boolean {
            return super.equals(obj) &&
                this.lineBreakSymbol == obj.lineBreakSymbol;
        }

        static renderCharacterProperties: CharacterProperties;

        constructor(characterProperties: CharacterProperties) {
            super(characterProperties);
            if (!LayoutLineBreakBox.renderCharacterProperties && characterProperties) { // && characterProperties for tests
                LayoutLineBreakBox.renderCharacterProperties = new CharacterProperties();
                LayoutLineBreakBox.renderCharacterProperties.fontSize = 14;

                LayoutLineBreakBox.renderCharacterProperties.fontInfo = this.characterProperties.fontInfo.clone();
                LayoutLineBreakBox.renderCharacterProperties.fontInfo.measurer = this.characterProperties.fontInfo.measurer;
                LayoutLineBreakBox.renderCharacterProperties.fontInfo.name = "Arial";
                LayoutLineBreakBox.renderCharacterProperties.fontInfo.cssString = "Arial";
                LayoutLineBreakBox.renderCharacterProperties.fontInfo.scriptMultiplier = 0.65;
            }
        }

        protected virtualClone(): LayoutBox {
            const newObj: LayoutLineBreakBox = new LayoutLineBreakBox(this.characterProperties);
            newObj.copyFrom(this);
            return newObj;
        }

        public copyFrom(obj: LayoutLineBreakBox) {
            super.copyFrom(obj);
            this.lineBreakSymbol = obj.lineBreakSymbol;
        }

        public getType(): LayoutBoxType {
            return LayoutBoxType.LineBreak;
        }

        public pushInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) {
            info.push(new MeasureInfo(Utils.specialCharacters.HiddenLineBreak, this.renderGetCharacterProperties(), false));
        }

        public popInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) {
            if (showHiddenSymbols)
                this.lineBreakSymbol = Utils.specialCharacters.HiddenLineBreak;
            else
                this.lineBreakSymbol = "&nbsp;";

            var elem: MeasureInfo = info.pop();
            this.width = elem.resultWidth;
            this.height = elem.resultHeight;
        }

        public renderGetContent(constants: any, handlerURI: string, emptyImageCacheId: number): string {
            return this.lineBreakSymbol;
        }

        public renderNoStrikeoutAndNoUnderlineIfBoxInEndRow() {
            return true;
        }

        public renderGetCharacterProperties(): CharacterProperties {
            LayoutLineBreakBox.renderCharacterProperties.hidden = this.characterProperties.hidden;
            return LayoutLineBreakBox.renderCharacterProperties;
        }
    }

    export class LayoutPageBreakBox extends LayoutBox implements ISupportCopyFrom<LayoutPageBreakBox> {
        // width - special hidden symbol, or &nbsp;
        // width in case show hidden symbols not depend at charProps
        text: string;
        static renderCharacterProperties: CharacterProperties;

        public equals(obj: LayoutPageBreakBox): boolean {
            return super.equals(obj) &&
                this.text == obj.text;
        }

        constructor(characterProperties: CharacterProperties) {
            super(characterProperties);
            if (!LayoutPageBreakBox.renderCharacterProperties && characterProperties) { // && characterProperties for tests
                LayoutPageBreakBox.renderCharacterProperties = new CharacterProperties();
                LayoutPageBreakBox.renderCharacterProperties.fontSize = 10;

                LayoutPageBreakBox.renderCharacterProperties.fontInfo = this.characterProperties.fontInfo.clone();
                LayoutPageBreakBox.renderCharacterProperties.fontInfo.measurer = this.characterProperties.fontInfo.measurer;
                LayoutPageBreakBox.renderCharacterProperties.fontInfo.name = "Arial";
                LayoutPageBreakBox.renderCharacterProperties.fontInfo.cssString = "Arial";
                LayoutPageBreakBox.renderCharacterProperties.fontInfo.scriptMultiplier = 0.65;
            }
        }

        protected virtualClone(): LayoutBox {
            const newObj: LayoutPageBreakBox = new LayoutPageBreakBox(this.characterProperties);
            newObj.copyFrom(this);
            return newObj;
        }

        public copyFrom(obj: LayoutPageBreakBox) {
            super.copyFrom(obj);
            this.text = obj.text;
        }

        public getType(): LayoutBoxType {
            return LayoutBoxType.PageBreak;
        }

        public pushInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) {
            info.push(new MeasureInfo(this.getHiddenText(), this.renderGetCharacterProperties(), true)); // better here must be constant value
        }

        public popInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) {
            var elem: MeasureInfo = info.pop();

            if (showHiddenSymbols)
                this.text = elem.text;
            else
                this.text = "&nbsp;";
            
            this.width = elem.resultWidth;
            this.height = elem.resultHeight;
        }

        public renderGetContent(constants: any, handlerURI: string, emptyImageCacheId: number): string {
            return this.text;
        }

        public renderNoStrikeoutAndNoUnderlineIfBoxInEndRow() {
            return true;
        }

        public renderGetCharacterProperties(): CharacterProperties {
            LayoutPageBreakBox.renderCharacterProperties.hidden = this.characterProperties.hidden;
            return LayoutPageBreakBox.renderCharacterProperties;
        }

        // only for interval use
        public getHiddenText(): string {
            return "........Page Break........";
        }
    }

    export class LayoutColumnBreakBox extends LayoutPageBreakBox implements ISupportCopyFrom<LayoutColumnBreakBox> {
        protected virtualClone(): LayoutBox {
            const newObj: LayoutColumnBreakBox = new LayoutColumnBreakBox(this.characterProperties);
            newObj.copyFrom(this);
            return newObj;
        }

        public copyFrom(obj: LayoutColumnBreakBox) {
            super.copyFrom(obj);
        }

        public getType(): LayoutBoxType {
            return LayoutBoxType.ColumnBreak;
        }

        // only for interval use
        public getHiddenText(): string {
            return "........Column Break........";
        }
    }

    export class LayoutSectionMarkBox extends LayoutPageBreakBox implements ISupportCopyFrom<LayoutSectionMarkBox>{
        protected virtualClone(): LayoutBox {
            const newObj: LayoutSectionMarkBox = new LayoutSectionMarkBox(this.characterProperties);
            newObj.copyFrom(this);
            return newObj;
        }

        public copyFrom(obj: LayoutSectionMarkBox) {
            super.copyFrom(obj);
        }

        public getType(): LayoutBoxType {
            return LayoutBoxType.SectionMark;
        }

        // only for interval use
        public getHiddenText(): string {
            return "::::::::Section Break::::::::";
        }
    }

    export class LayoutNumberingListBox extends LayoutBox implements ISupportCopyFrom<LayoutNumberingListBox> {
        textBox: LayoutTextBox;
        separatorBox: LayoutBox;

        public equals(obj: LayoutNumberingListBox): boolean {
            return super.equals(obj) &&
                this.textBox.equals(obj.textBox) &&
                (!this.separatorBox && !obj.separatorBox || this.separatorBox && obj.separatorBox && this.separatorBox.equals(obj.separatorBox));
        }

        constructor(characterProperties: CharacterProperties, text: string, separatorChar: string) {
            super(characterProperties);
            this.textBox = new LayoutTextBox(characterProperties, text);
            if(separatorChar != '\u0000') {
                const separatorCharacterProperties = characterProperties.clone();
                separatorCharacterProperties.fontInfo = characterProperties.fontInfo.clone();
                separatorCharacterProperties.fontInfo.measurer = characterProperties.fontInfo.measurer;
                separatorCharacterProperties.fontInfo.name = "Arial";
                separatorCharacterProperties.fontInfo.cssString = "Arial";
                switch(separatorChar) {
                    case Utils.specialCharacters.TabMark:
                        this.separatorBox = new LayoutTabSpaceBox(separatorCharacterProperties);
                        break;
                    case Utils.specialCharacters.Space:
                    case Utils.specialCharacters.EmSpace:
                    case Utils.specialCharacters.EnSpace:
                        this.separatorBox = new LayoutSpaceBox(separatorCharacterProperties);
                        break;
                    default:
                        break;
                }
            }
        }

        protected virtualClone(): LayoutBox {
            const newObj: LayoutNumberingListBox = new LayoutNumberingListBox(this.characterProperties, "0", '\u0000');
            newObj.copyFrom(this);
            return newObj;
        }

        public copyFrom(obj: LayoutNumberingListBox) {
            super.copyFrom(obj);
            this.textBox = <LayoutTextBox>(obj.textBox.clone());
            this.separatorBox = obj.separatorBox.clone();
        }

        public getType(): LayoutBoxType {
            return LayoutBoxType.NumberingList;
        }

        public pushInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) {
            this.textBox.pushInfoForMeasure(info, showHiddenSymbols);
            if (this.separatorBox)
                this.separatorBox.pushInfoForMeasure(info, showHiddenSymbols);
        }
        public popInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) {
            if (this.separatorBox)
                this.separatorBox.popInfoForMeasure(info, showHiddenSymbols);
            this.textBox.popInfoForMeasure(info, showHiddenSymbols);
        }
        public isWhitespace(): boolean {
            return false;
        }
    }

    export class LayoutFieldCodeStartBox extends LayoutBox implements ISupportCopyFrom<LayoutFieldCodeStartBox>{
        protected virtualClone(): LayoutBox {
            const newObj: LayoutFieldCodeStartBox = new LayoutFieldCodeStartBox(this.characterProperties);
            newObj.copyFrom(this);
            return newObj;
        }

        public copyFrom(obj: LayoutFieldCodeStartBox) {
            super.copyFrom(obj);
        }

        public getType(): LayoutBoxType {
            return LayoutBoxType.FieldCodeStart;
        }

        public getBoxChar(): string {
            return "{";
        }

        public pushInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) {
            info.push(new MeasureInfo(this.getBoxChar(), this.characterProperties, true));
        }

        public popInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) {
            var elem: MeasureInfo = info.pop();
            this.width = elem.resultWidth;
            this.height = elem.resultHeight;
        }

        public isVisibleForRowAlign(): boolean {
            return true;
        }

        public isVisible(): boolean {
            return true;
        }

        public renderGetContent(constants: any, handlerURI: string, emptyImageCacheId: number): string {
            return Utils.encodeHtml(this.getBoxChar());
        }
    }

    export class LayoutFieldCodeEndBox extends LayoutFieldCodeStartBox implements ISupportCopyFrom<LayoutFieldCodeEndBox> {
        protected virtualClone(): LayoutBox {
            const newObj: LayoutFieldCodeEndBox = new LayoutFieldCodeEndBox(this.characterProperties);
            newObj.copyFrom(this);
            return newObj;
        }

        public copyFrom(obj: LayoutFieldCodeEndBox) {
            super.copyFrom(obj);
        }

        public getType(): LayoutBoxType {
            return LayoutBoxType.FieldCodeEnd;
        }

        public getBoxChar(): string {
            return "}";
        }
    }

    export class LayoutFieldResultEndBox extends LayoutBox implements ISupportCopyFrom<LayoutFieldResultEndBox> {
        protected virtualClone(): LayoutBox {
            const newObj: LayoutFieldResultEndBox = new LayoutFieldResultEndBox(this.characterProperties);
            newObj.copyFrom(this);
            return newObj;
        }

        public copyFrom(obj: LayoutFieldResultEndBox) {
            super.copyFrom(obj);
        }

        public getType(): LayoutBoxType {
            return LayoutBoxType.FieldResultEnd;
        }

        public pushInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) { }

        public popInfoForMeasure(info: MeasureInfo[], showHiddenSymbols: boolean) { }
    }

    export class BookmarkBox extends LayoutItemBase {
        public boxType: LayoutBoxType = LayoutBoxType.BookmarkStartBox;
        public color: string = "";

        static DEFAULT_WIDTH: number = 3;
        static DEFAULT_BORDER_WIDTH: number = 2;

        constructor(boxType: LayoutBoxType) {
            super();
            this.boxType = boxType;
        }
    }
}