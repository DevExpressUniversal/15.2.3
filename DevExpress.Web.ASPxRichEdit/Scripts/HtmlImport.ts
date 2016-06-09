module __aspxRichEdit {
    export class ImportedRunInfo {
        runType: TextRunType;
        runLength: number;

        constructor(runType: TextRunType, runLength: number) {
            this.runType = runType;
            this.runLength = runLength;
        }
    }

    export class ImportedTextRunInfo extends ImportedRunInfo {
        text: string;
        maskedCharacterProperties: MaskedCharacterProperties;

        constructor(text: string, maskedCharacterProperties: MaskedCharacterProperties) {
            super(TextRunType.TextRun, text.length);
            this.text = text;
            this.maskedCharacterProperties = maskedCharacterProperties;
        }
    }

    export class ImportedParagraphListInfo {
        listIndex: number;
        listLevel: number;
        listFormat: NumberingFormat;
        listType: NumberingType;
        displayFormatString: string;
        maskedCharacterProperties: MaskedCharacterProperties;

        constructor(listIndex: number, listLevel: number, listFormat: NumberingFormat, listType: NumberingType, displayFormatString: string, maskedCharacterProperties: MaskedCharacterProperties) {
            this.listIndex = listIndex;
            this.listLevel = listLevel;
            this.listFormat = listFormat;
            this.listType = listType;
            this.displayFormatString = displayFormatString;
            this.maskedCharacterProperties = maskedCharacterProperties;
        }
    }

    export class ImportedParagraphRunInfo extends ImportedRunInfo {
        tableInfo: ImportedTableInfo;
        listInfo: ImportedParagraphListInfo;
        maskedParagraphProperties: MaskedParagraphProperties;

        constructor(listInfo: ImportedParagraphListInfo, tableInfo: ImportedTableInfo, maskedParagraphProperties: MaskedParagraphProperties) {
            super(TextRunType.ParagraphRun, Utils.specialCharacters.ParagraphMark.length);
            this.listInfo = listInfo;
            this.tableInfo = tableInfo;
            this.maskedParagraphProperties = maskedParagraphProperties;
        }
    }

    export class ImportedInlinePictureRunInfo extends ImportedRunInfo {
        id: number;
        originalWidth: number;
        originalHeight: number;
        scaleX: number;
        scaleY: number;
        lockAspectRatio: boolean;
        sourceUrl: string;

        constructor(id: number, originalWidth: number, originalHeight: number, scaleX: number, scaleY: number, lockAspectRatio: boolean, sourceUrl: string = null) {
            super(TextRunType.InlinePictureRun, 1);
            this.id = id;
            this.originalWidth = originalWidth;
            this.originalHeight = originalHeight;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.lockAspectRatio = lockAspectRatio;
            this.sourceUrl = sourceUrl;
        }
    }

    export class ImportedFieldCodeStartRunInfo extends ImportedRunInfo {
        hyperlinkInfo: HyperlinkInfo;

        constructor(hyperlinkInfo: HyperlinkInfo) {
            super(TextRunType.FieldCodeStartRun, Utils.specialCharacters.FieldCodeStartRun.length);
            this.hyperlinkInfo = hyperlinkInfo;
        }
    }

    export class ImportedFieldCodeEndRunInfo extends ImportedRunInfo {
        constructor() {
            super(TextRunType.FieldCodeEndRun, Utils.specialCharacters.FieldCodeEndRun.length);
        }
    }

    export class ImportedFieldResultEndRunInfo extends ImportedRunInfo {
        constructor() {
            super(TextRunType.FieldResultEndRun, Utils.specialCharacters.FieldResultEndRun.length);
        }
    }

    export class ImportedTableInfo {
        rowCount: number;
        cellCount: number;
        properties: TableProperties; //TODO
        rows: ImportedTableRowInfo[];
        width: number;

        constructor(rowCount: number, cellCount: number, width: number) {
            this.rowCount = rowCount;
            this.cellCount = cellCount;
            this.properties = new TableProperties();
            this.rows = [];
            this.width = width;
        }
    }

    export class ImportedTableRowInfo {
        properties: TableRowProperties; //TODO
        cells: ImportedTableCellInfo[];

        constructor() {
            this.properties = new TableRowProperties();
            this.cells = [];
        }
    }

    export class ImportedTableCellInfo {
        startParagraphIndex: number;
        endParagraphIndex: number;
        columnSpan: number = 1;
        rowSpan: number = 1;
        properties: TableCellProperties; //TODO

        constructor(startParagraphIndex: number, endParagraphIndex: number, columnSpan: number, rowSpan: number) {
            this.startParagraphIndex = startParagraphIndex;
            this.endParagraphIndex = endParagraphIndex;
            this.columnSpan = columnSpan;
            this.rowSpan = rowSpan;
            this.properties = new TableCellProperties();
        }
    }

    export class HtmlImporter {
        control: IRichEditControl;
        allowedWidth: number;
        allowedHeight: number;
        columnsCalculator: ColumnsCalculator;
        emptyImageCacheId: number;
        importedRunsInfo: ImportedRunInfo[];
        importedTablesInfo: ImportedTableInfo[];

        constructor(control: IRichEditControl, emptyImageCacheId: number) {
            this.control = control;
            this.emptyImageCacheId = emptyImageCacheId;
            this.importedRunsInfo = [];
            this.importedTablesInfo = [];
            this.columnsCalculator = new ColumnsCalculator(new TwipsUnitConverter());
        }

        public static importPlainText(text: string, control: IRichEditControl, interval: FixedInterval) {
            control.modelManipulator.insertText(control, interval, text, false);
        }
        public elementsImport(elements: HTMLElement[], interval: FixedInterval) {
            var section = this.control.model.getSectionByPosition(interval.start);
            var minimalSize = this.columnsCalculator.findMinimalColumnSize(section.sectionProperties);
            this.allowedWidth = UnitConverter.pixelsToTwips(minimalSize.width);
            this.allowedHeight = UnitConverter.pixelsToTwips(minimalSize.height);
            this.elementsImportCore(elements);
            if(this.importedRunsInfo.length)
                ModelManipulator.pasteSelection(this.control, interval, this.importedRunsInfo);
        }

        private getScaleDimensions(originalWidth: number, originalHeight: number): { x: number; y: number } {
            var scaleX: number = 100;
            var scaleY: number = 100;
            if(originalWidth > this.allowedWidth)
                scaleX = (this.allowedWidth / originalWidth) * 100;
            if(originalHeight > this.allowedHeight)
                scaleY = (this.allowedHeight / originalHeight) * 100;
            return { x: Math.min(scaleX, scaleY), y: Math.min(scaleX, scaleY) };
        }
        private elementsImportCore(elements: HTMLElement[]) {
            var model = this.control.model;
            for(var i = 0, element: HTMLElement; element = elements[i]; i++) {
                if(element.style && element.style.display == "none")
                    continue;
                var childElements = <HTMLElement[]>(<any>element.childNodes);
                if(childElements.length) {
                    for(var j = 0, childElement: HTMLElement; childElement = childElements[j]; j++) {
                        if(childElement.style) {
                            for(var prop in childElement.style) {
                                //TODO
                                if(prop.indexOf("border") === 0 && (element.tagName.toUpperCase() === "TD" || element.tagName.toUpperCase() === "TH"))
                                    continue; 
                                var isShorthandProperty = false;
                                switch(prop) {
                                    case "background":
                                    case "border":
                                    case "borderImage":
                                    case "borderTop":
                                    case "borderRight":
                                    case "borderBottom":
                                    case "borderLeft":
                                    case "borderWidth":
                                    case "borderColor":
                                    case "borderStyle":
                                    case "borderRadius":
                                    case "font":
                                    case "fontVariant":
                                    case "listStyle":
                                    case "margin":
                                    case "padding":
                                    case "transition":
                                    case "transform":
                                        isShorthandProperty = true;
                                        break;
                                    default:
                                        break;
                                } 
                                if(childElement.style[prop] === "" && !(prop == "cssText" || prop == "listStyleType" || isShorthandProperty)  && element.style[prop] !== "")
                                    childElement.style[prop] = element.style[prop];
                            }
                        }
                    }
                    switch(element.tagName) {
                        case "H1":
                        case "H2":
                        case "H3":
                        case "H4":
                        case "H5":
                        case "H6":
                        case "P":
                            let emptyParagraphMatches = element.outerHTML.match(/<p([^>]*)>&nbsp;<\/p>/gi);
                            if(!(emptyParagraphMatches && emptyParagraphMatches.length)) {
                                var listInfo = this.getParagraphListInfo(element);
                                if(listInfo)
                                    childElements = ASPx.GetChildNodes(element, (e) => { return true; });
                                this.elementsImportCore(childElements);
                            }
                            this.importedRunsInfo.push(new ImportedParagraphRunInfo(listInfo, this.importedTablesInfo.pop(), this.getMaskedParagraphProperties(element)));
                            break;
                        case "A":
                            var hyperlinkElement = <HTMLLinkElement>element;
                            var uriParts = hyperlinkElement.href.split("#");
                            var hyperlinkInfo = new HyperlinkInfo(uriParts[0], uriParts.length > 1 ? uriParts[1] : "", hyperlinkElement.title, false);
                            this.importedRunsInfo.push(new ImportedFieldCodeStartRunInfo(hyperlinkInfo));
                            this.importedRunsInfo.push(new ImportedFieldCodeEndRunInfo());
                            this.elementsImportCore(childElements);
                            this.importedRunsInfo.push(new ImportedFieldResultEndRunInfo());
                            break;
                        case "TABLE":
                            let tableRows = ASPx.GetNodes(element, (e) => { return e.tagName && e.tagName.toUpperCase() == "TR"; });
                            let rowCount = tableRows.length;
                            if(rowCount) {
                                let paragraphElements = ASPx.GetNodes(element, (e) => { return e.tagName && e.tagName.toUpperCase() == "P"; });
                                let cellCount = 0;
                                let firstRowCells = ASPx.GetChildNodes(tableRows[0], (e) => { return e.tagName && (e.tagName.toUpperCase() == "TD" || e.tagName.toUpperCase() == "TH"); });
                                for(let i = 0, firstRowCell: HTMLTableCellElement; firstRowCell = <HTMLTableCellElement>firstRowCells[i]; i++)
                                    cellCount += firstRowCell.colSpan;
                                let tableInfo = new ImportedTableInfo(rowCount, cellCount, this.allowedWidth);
                                for(let i = 0, row: HTMLTableRowElement; row = <HTMLTableRowElement>tableRows[i]; i++) {
                                    let rowInfo = new ImportedTableRowInfo();
                                    let rowCells = ASPx.GetChildNodes(row, (e) => { return e.tagName && (e.tagName.toUpperCase() == "TD" || e.tagName.toUpperCase() == "TH"); });
                                    for(let j = 0, cell: HTMLTableCellElement; cell = <HTMLTableCellElement>rowCells[j]; j++) {
                                        //TODO!
                                        let startParagraphIndex = 0;
                                        let endParagraphIndex = 0;
                                        if(paragraphElements.length) {
                                            let cellParagraphs = ASPx.GetChildNodes(cell, (e) => { return e.tagName && e.tagName.toUpperCase() == "P"; });
                                            if(cellParagraphs.length) {
                                                startParagraphIndex = paragraphElements.indexOf(cellParagraphs[0]);
                                                endParagraphIndex = paragraphElements.indexOf(cellParagraphs[cellParagraphs.length - 1]);
                                            }
                                        }
                                        let cellInfo = new ImportedTableCellInfo(startParagraphIndex, endParagraphIndex, cell.colSpan, cell.rowSpan);
                                        rowInfo.cells.push(cellInfo);
                                    }
                                    tableInfo.rows.push(rowInfo);
                                }
                                this.importedTablesInfo.push(tableInfo);
                            }
                        default:
                            this.elementsImportCore(childElements);
                            break;
                    }
                }
                else {
                    switch(element.tagName) {
                        case "IMG":
                            var imageUrl: string = ASPx.Attr.GetAttribute(element, "src");
                            var imageWidth = UnitConverter.pixelsToTwips(element.offsetWidth || 32);
                            var imageHeight = UnitConverter.pixelsToTwips(element.offsetHeight || 32);

                            var cachedImageMatches = imageUrl.match(/DXS\.axd.+\&img=(.*)$/);
                            if(cachedImageMatches) {
                                var imageCacheId = parseInt(cachedImageMatches[1]);
                                var scaleDimensions = this.getScaleDimensions(imageWidth, imageHeight);
                                this.importedRunsInfo.push(new ImportedInlinePictureRunInfo(imageCacheId, imageWidth, imageHeight, scaleDimensions.x, scaleDimensions.y, false));
                            }
                            else {
                                if(!imageUrl.match(/^file\:\/\//gi)) {
                                    this.importedRunsInfo.push(new ImportedInlinePictureRunInfo(this.emptyImageCacheId, 1, 1,
                                        Math.min(imageWidth, this.allowedWidth) * 100, Math.min(imageHeight, this.allowedHeight) * 100, false, encodeURIComponent(imageUrl)));
                                }
                            }
                            break;
                        case "BR":
                            var breakChar = element.style.pageBreakBefore != "always" ? model.specChars.LineBreak : model.specChars.PageBreak;
                            this.importedRunsInfo.push(new ImportedTextRunInfo(breakChar, this.getMaskedCharacterProperties(element)));
                            break;
                        default:
                            var text = element.nodeValue || ASPx.GetInnerText(element);
                            if(text)
                                this.importedRunsInfo.push(new ImportedTextRunInfo(text, this.getMaskedCharacterProperties(element)));
                            break;
                    }
                }
            }
        }

        private getColor(color: string): number {
            var foreColorMatchesRGB = this.getRGBA(color);
            var hashColor = "";
            if(foreColorMatchesRGB) {
                return ((foreColorMatchesRGB[0] & 255) << 16) | ((foreColorMatchesRGB[1] & 255) << 8) | (foreColorMatchesRGB[2] & 255) |
                    (foreColorMatchesRGB.length > 3 ? ((foreColorMatchesRGB[3] & 255) << 24) : ((255 & 255) << 24));
            }
            else {
                if(/^#([0-9a-f]{6})$/.test(color) || /^#([0-9a-f]{3})$/.test(color))
                    hashColor = color;
                else if (/^[a-z]+$/.test(color))
                    hashColor = ColorHelper.colorNames[color.toLowerCase()];
            }
            if(hashColor)
                return ColorHelper.hashToColor(hashColor);
            return null;
        }
        private getRGBA(color: string) : number[] {
            var matchesRGBA = color.replace(/ +/g, '').match(/(rgba?)|(\d+(\.\d+)?%?)|(\.\d+)/g);
            if(matchesRGBA && matchesRGBA.length > 3) {
                var i = 0, itm;
                var result: number[] = [];
                while(i < matchesRGBA.length - 1) {
                    itm = matchesRGBA[++i];
                    if(itm.indexOf('%') != -1)
                        itm = Math.round(parseFloat(itm) * 2.55);
                    else
                        itm = parseInt(itm);
                    if(itm < 0 || itm > 255)
                        return null;
                    result.push(itm);
                }
                if(color.indexOf('rgba') === 0) {
                    if(result[3] == NaN || result[3] < 0 || result[3] > 1)
                        return null;
                }
                else if(result[3])
                    return null;
                return result;
            }
            return null;
        }
        private getValueInTwips(stringValue: string): number {
            if (!stringValue)
                return null;

            var unitTypeMatches = stringValue.match(/(px|em|ex|%|in|cm|mm|pt|pc)/g);
            if (!unitTypeMatches)
                return null;

            var unitType = unitTypeMatches[0];
            var value = parseFloat(stringValue.replace(unitType, ''));
            switch(unitType) {
                case "px":
                    return UnitConverter.pixelsToTwips(value);
                case "in":
                    return UnitConverter.inchesToTwips(value);
                case "cm":
                    return UnitConverter.centimetersToTwips(value);
                case "mm":
                    return UnitConverter.centimetersToTwips(value) * 10;
                case "pt":
                    return UnitConverter.pointsToTwips(value);
                case "pc":
                    return UnitConverter.picasToTwips(value);
                default:
                    return 0;
            }
        }
        private getFontInfo(fontFamily: string): FontInfo {
            var fonts = fontFamily.split(",");
            var fontCount = fonts.length;
            var genericFontFamily = ASPx.Str.Trim(fonts[fontCount - 1]).toLowerCase();
            var defaultFont = "";
            switch(genericFontFamily) {
                case "serif":
                    defaultFont = "'Times New Roman'";
                    break;
                case "sans-serif":
                    defaultFont = "Arial";
                    break;
                case "cursive":
                case "fantasy":
                    defaultFont = "'Comic Sans MS'";
                    break;
                case "monospace":
                    defaultFont = "'Courier New'";
                    break;
                default:
                    genericFontFamily = "";
                    break;
            }
            if(genericFontFamily && fontCount > 1)
                fontCount--;
            var fontInfo = this.control.model.cache.fontInfoCache.findItem((f: FontInfo) => {
                for(var i = 0; i < fontCount; i++) {
                    var cssString = f.cssString;
                    var font = ASPx.Str.TrimStart(fonts[i]);
                    if(font == genericFontFamily)
                        font = defaultFont;
                    var fontPosition = cssString.indexOf(font);
                    if((fontPosition == 0 || fontPosition == 1 && cssString[0] == "'") &&
                        (!genericFontFamily || cssString.substr(cssString.length - genericFontFamily.length) === genericFontFamily))
                        return true;
                }
                return false;
            });
            return fontInfo;
        }
        private getFontSize(font: string, parentFont: string): number {
            var getFontSizeCore = (font: string) => {
                if(font) {
                    if(font.indexOf("px") > 0)
                        return UnitConverter.pixelsToPoints(parseInt(font));
                    if(font.indexOf("pt") > 0)
                        return parseInt(font);
                }
                return null;
            };

            var fontSize = getFontSizeCore(font);
            if(fontSize !== null)
                return fontSize;

            var parentFontSize = getFontSizeCore(parentFont);
            var currentFontSize = parentFontSize !== null ? parentFontSize : 12;
            if(font.indexOf("em") > 0)
                return parseInt(font) * currentFontSize;
            if(font.indexOf("%") > 0)
                return parseInt(font) * currentFontSize / 100;

            return null;    
        }
        private getMaskedCharacterProperties(element: HTMLElement): MaskedCharacterProperties {
            var result = this.control.inputPosition.getMaskedCharacterProperties().clone();
            var styledElement = element.nodeType == 1 ? element : (element.parentElement || <HTMLElement>element.parentNode);
            var currentStyle = ASPx.GetCurrentStyle(styledElement);
            if(currentStyle) {
                if(currentStyle.fontWeight == "bold" || currentStyle.fontWeight == "700") {
                    result.fontBold = true;
                    result.setUseValue(CharacterPropertiesMask.UseFontBold, true);
                }
                if(currentStyle.fontStyle == "italic") {
                    result.fontItalic = true;
                    result.setUseValue(CharacterPropertiesMask.UseFontItalic, true);
                }
                if(currentStyle.textTransform == "uppercase") {
                    result.allCaps = true;
                    result.setUseValue(CharacterPropertiesMask.UseAllCaps, true);
                }

                var textDecoration = currentStyle.textDecoration;
                switch(textDecoration) {
                    case "line-through":
                        result.fontStrikeoutType = StrikeoutType.Single;
                        break;
                    case "underline":
                        result.fontUnderlineType = UnderlineType.Single;
                        break;
                    default:
                        break;
                }
                if(result.fontUnderlineType == UnderlineType.None) {
                    var bottomBorderStyle = currentStyle.borderBottomStyle;
                    switch(bottomBorderStyle) {
                        case "solid":
                            result.fontUnderlineType = UnderlineType.Single;
                            break;
                        case "dashed":
                            result.fontUnderlineType = UnderlineType.Dashed;
                            break;
                        case "dotted":
                            result.fontUnderlineType = UnderlineType.Dotted;
                            break;
                        case "double":
                            result.fontUnderlineType = UnderlineType.Double;
                            break;
                        default:
                            break;
                    }
                }
                if(result.fontStrikeoutType != StrikeoutType.None)
                    result.setUseValue(CharacterPropertiesMask.UseFontStrikeoutType, true);
                if(result.fontUnderlineType != UnderlineType.None)
                    result.setUseValue(CharacterPropertiesMask.UseFontUnderlineType, true);

                var getIsPropertyDefined = (propName: string) => {
                    return styledElement.style[propName] !== "";
                };

                if(getIsPropertyDefined("color")) {
                    var foreColor = this.getColor(currentStyle.color);
                    if(foreColor != null) {
                        result.foreColor = foreColor;
                        result.setUseValue(CharacterPropertiesMask.UseForeColor, true);
                    }
                }

                if(getIsPropertyDefined("background-color")) {
                    var backColor = this.getColor(currentStyle.backgroundColor);
                    if(backColor) {
                        result.backColor = backColor;
                        result.setUseValue(CharacterPropertiesMask.UseBackColor, true);
                    }
                }

                if(getIsPropertyDefined("font-family")) {
                    var fontInfo = this.getFontInfo(currentStyle.fontFamily.replace(/"/g, "'").replace(/,(\S)/g, ", $1"));
                    result.fontInfo = fontInfo || this.control.model.defaultCharacterProperties.fontInfo;
                    if (fontInfo)
                        result.setUseValue(CharacterPropertiesMask.UseFontName, true);
                }

                if(getIsPropertyDefined("font-size")) {
                    var parentCurrentStyle = ASPx.GetCurrentStyle(styledElement.parentElement || <HTMLElement>styledElement.parentNode);
                    var fontSize = this.getFontSize(currentStyle.fontSize, parentCurrentStyle ? parentCurrentStyle.fontSize : null);
                    if(fontSize) {
                        result.fontSize = fontSize;
                        result.setUseValue(CharacterPropertiesMask.UseDoubleFontSize, true);
                    }
                }

                if(currentStyle.display == "none")
                    result.hidden = true;
                if(currentStyle.verticalAlign == "sub")
                    result.script = CharacterFormattingScript.Subscript;
            }
            return result;
        }

        private getBorderInfo(borderWidth: string, borderStyle: string, borderColor: string): BorderInfo {
            var border = new BorderInfo();
            var width = this.getValueInTwips(borderWidth);
            if(width != null)
                border.width = width;
            switch(borderStyle) {
                case "dashed":
                    border.style = BorderLineStyle.Dashed;
                    break;
                case "dotted":
                    border.style = BorderLineStyle.Dotted;
                    break;
                case "double":
                    border.style = BorderLineStyle.Double;
                    break;
                case "inset":
                    border.style = BorderLineStyle.Inset;
                    break;
                case "outset":
                    border.style = BorderLineStyle.Outset;
                    break;
                default:
                    border.style = BorderLineStyle.None;
                    break;
            }
            var color = this.getColor(borderColor);
            if(color != null)
                border.color = color == ColorHelper.BLACK_COLOR ? 0 : color; // TODO why???
            return border;
        }
        private getMaskedParagraphProperties(element: HTMLElement): MaskedParagraphProperties {
            var elementStyle = element.style;
            var result = new MaskedParagraphProperties();

            var paragraphAlignment = ParagraphAlignment.Left;
            switch(elementStyle.textAlign) {
                case "right":
                    paragraphAlignment = ParagraphAlignment.Right;
                    break;
                case "justify":
                    paragraphAlignment = ParagraphAlignment.Justify;
                    break;
                case "center":
                    paragraphAlignment = ParagraphAlignment.Center;
                    break;
                default:
                    break;
            }
            result.alignment = paragraphAlignment;
            result.setUseValue(ParagraphPropertiesMask.UseAlignment, result.alignment != ParagraphAlignment.Left);
            
            var firstLineIndentType = ParagraphFirstLineIndent.None;
            var firstLineIndent = this.getValueInTwips(elementStyle.textIndent);
            if(firstLineIndent != null) {
                if(firstLineIndent)
                    firstLineIndentType = firstLineIndent > 0 ? ParagraphFirstLineIndent.Indented : ParagraphFirstLineIndent.Hanging;
                result.firstLineIndent = Math.abs(firstLineIndent);
            }
            result.firstLineIndentType = firstLineIndentType;
            result.setUseValue(ParagraphPropertiesMask.UseFirstLineIndent, result.firstLineIndentType != ParagraphFirstLineIndent.None);

            var leftIndent = this.getValueInTwips(elementStyle.marginLeft);
            if(leftIndent != null) {
                result.leftIndent = leftIndent;
                result.setUseValue(ParagraphPropertiesMask.UseLeftIndent, leftIndent != 0);
            }
            var rightIndent = this.getValueInTwips(elementStyle.marginRight);
            if(rightIndent != null) {
                result.rightIndent = rightIndent;
                result.setUseValue(ParagraphPropertiesMask.UseRightIndent, rightIndent != 0);
            }

            var spacingBefore = this.getValueInTwips(elementStyle.marginTop);
            if(spacingBefore != null && spacingBefore >= 0) {
                result.spacingBefore = spacingBefore;
                result.setUseValue(ParagraphPropertiesMask.UseSpacingBefore, true);
            }
            var spacingAfter = this.getValueInTwips(elementStyle.marginBottom);
            if(spacingAfter != null && spacingAfter >= 0) {
                result.spacingAfter = spacingAfter;
                result.setUseValue(ParagraphPropertiesMask.UseSpacingAfter, true);
            }

            var multipleValue = NaN;
            var stringLineHeight = elementStyle.lineHeight;
            if(ASPx.IsPercentageSize(stringLineHeight))
                multipleValue = parseFloat(stringLineHeight.replace("%", "")) / 100;
            else {
                var lineSpacing = this.getValueInTwips(stringLineHeight);
                if(lineSpacing == null)
                    multipleValue = parseFloat(stringLineHeight);
                else if(lineSpacing) {
                    result.lineSpacing = lineSpacing;
                    result.lineSpacingType = elementStyle["mso-line-height-rule"] == "exactly" ? ParagraphLineSpacingType.Exactly
                        : ParagraphLineSpacingType.AtLeast;
                    result.setUseValue(ParagraphPropertiesMask.UseLineSpacing, true);
                }
            }
            if(multipleValue && multipleValue != NaN) {
                switch(multipleValue) {
                    case 1.5:
                        result.lineSpacingType = ParagraphLineSpacingType.Sesquialteral;
                        break;
                    case 2:
                        result.lineSpacingType = ParagraphLineSpacingType.Double;
                        break;
                    default:
                        result.lineSpacing = multipleValue;
                        result.lineSpacingType = ParagraphLineSpacingType.Multiple;
                }
                result.setUseValue(ParagraphPropertiesMask.UseLineSpacing, true);
            }

            var backColor = this.getColor(element.style.backgroundColor);
            if(backColor) {
                result.backColor = backColor == ColorHelper.BLACK_COLOR ? 0 : backColor; // TODO why???
                result.setUseValue(ParagraphPropertiesMask.UseBackColor, result.backColor != 0);
            }
            result.setUseValue(ParagraphPropertiesMask.UseLineSpacing, true);
            result.setUseValue(ParagraphPropertiesMask.UseBeforeAutoSpacing, true);
            result.setUseValue(ParagraphPropertiesMask.UseAfterAutoSpacing, true);

            var calculatedStyle = ASPx.GetCurrentStyle(element);
            if(calculatedStyle.borderTop) {
                result.topBorder.copyFrom(this.getBorderInfo(calculatedStyle.borderTopWidth, calculatedStyle.borderTopStyle, calculatedStyle.borderTopColor));
                result.setUseValue(ParagraphPropertiesMask.UseTopBorder, result.topBorder.style != BorderLineStyle.None);
            }
            if(calculatedStyle.borderLeft) {
                result.leftBorder.copyFrom(this.getBorderInfo(calculatedStyle.borderLeftWidth, calculatedStyle.borderLeftStyle, calculatedStyle.borderLeftColor));
                result.setUseValue(ParagraphPropertiesMask.UseLeftBorder, result.leftBorder.style != BorderLineStyle.None);
            }
            if(calculatedStyle.borderBottom) {
                result.bottomBorder.copyFrom(this.getBorderInfo(calculatedStyle.borderBottomWidth, calculatedStyle.borderBottomStyle, calculatedStyle.borderBottomColor));
                result.setUseValue(ParagraphPropertiesMask.UseBottomBorder, result.bottomBorder.style != BorderLineStyle.None);
            }
            if(calculatedStyle.borderRight) {
                result.rightBorder.copyFrom(this.getBorderInfo(calculatedStyle.borderRightWidth, calculatedStyle.borderRightStyle, calculatedStyle.borderRightColor));
                result.setUseValue(ParagraphPropertiesMask.UseRightBorder, result.rightBorder.style != BorderLineStyle.None);
            }
            return result;
        }
        private getParagraphListInfo(element: HTMLElement): ImportedParagraphListInfo {
            var listItemElement = ASPx.GetParentByTagName(element, "LI");
            var msoListAttr = element.outerHTML.match(/mso-list:\s*\w*\s*level[^ ]/gi);
            if(listItemElement || msoListAttr) {
                var listFormat = NumberingFormat.None;
                var displayFormatString = "";
                var maskedCharacterProperties = null;
                var listIndex = 0;
                var listLevelIndex = 0;
                var getParentListElement = (childElement: HTMLElement): HTMLElement => {
                    return ASPx.GetParentByTagName(childElement, "UL") || ASPx.GetParentByTagName(childElement, "OL");
                };
                var parentListElement = listItemElement ? getParentListElement(listItemElement) : null;

                if(msoListAttr && msoListAttr.length) {
                    listIndex = parseInt(msoListAttr[0].replace(/mso-list:\s*[A-Za-z]*(\d*)[\s\S]*/gi, '$1'));
                    listLevelIndex = parseInt(msoListAttr[0].replace(/mso-list:\s*\w*\s*level/gi, '')) - 1;
                } else {
                    var mainParentListElement = null;
                    var listElement = parentListElement ? parentListElement : null;
                    while(listElement) {
                        mainParentListElement = listElement;
                        listElement = getParentListElement(<HTMLElement>listElement.parentNode);
                    }
                    if(mainParentListElement) {
                        var lists = ASPx.GetChildNodes(<HTMLElement>mainParentListElement.parentNode, (e: HTMLElement) => { return e.tagName == "UL" || e.tagName == "OL" });
                        for(var i = 0; i < lists.length; i++) {
                            if(lists[i] == mainParentListElement)
                                listIndex = i;
                        }
                    }
                    var parentListItemElement = parentListElement ? ASPx.GetParentByTagName(parentListElement, "LI") : null;
                    while(parentListItemElement) {
                        listLevelIndex++;
                        parentListItemElement = ASPx.GetParentByTagName(<HTMLElement>parentListItemElement.parentNode, "LI");
                    }
                }  
                  
                if(parentListElement) {
                    var getListType = (listElement: HTMLElement): NumberingFormat => {
                        switch(ASPx.GetCurrentStyle(listElement).listStyleType) {
                            case "decimal":
                                return NumberingFormat.Decimal;
                            case "circle":
                            case "disc":
                            case "square":
                                return NumberingFormat.Bullet;
                            case "lower-alpha":
                            case "lower-latin":
                                return NumberingFormat.LowerLetter;
                            case "upper-alpha":
                            case "upper-latin":
                                return NumberingFormat.UpperLetter;
                            case "lower-roman":
                                return NumberingFormat.LowerRoman;
                            case "upper-roman":
                                return NumberingFormat.UpperRoman;
                            default:
                                return NumberingFormat.None;
                        }
                    };
                    listFormat = getListType(parentListElement);
                    var listElementParentList = getParentListElement(<HTMLElement>parentListElement.parentNode);
                    if(listElementParentList) {
                        var parentListFormat = getListType(listElementParentList);
                        if(listFormat != parentListFormat)
                            displayFormatString = "{" + listLevelIndex + "}";
                    }
                }
                else {
                    var ignoreElements = ASPx.GetNodes(element, (e: HTMLElement) => { return e.outerHTML.match(/mso-list:Ignore/gi) != null; });
                    var listTextElement = ignoreElements.length ? ignoreElements[0] : null;
                    if(!listTextElement) {
                        var whiteSpacesOnlyTextElements = ASPx.GetNodes(element, (e: HTMLElement) => {
                            return e.nodeType == 3 && e.nodeValue && !ASPx.Str.Trim(e.nodeValue) || e.textContent && !ASPx.Str.Trim(e.textContent)
                                || e.innerText && !ASPx.Str.Trim(e.innerText);
                        });
                        listTextElement = whiteSpacesOnlyTextElements.length ? <HTMLElement>whiteSpacesOnlyTextElements[0].previousSibling : <any>(element.firstChild);
                    }
                    var listText = listTextElement ? ASPx.Str.Trim((listTextElement.nodeValue || ASPx.GetInnerText(listTextElement)).split(" ")[0]) : "";
                    if(listText) {
                        if(/^(IX|IV|V?I{1,3})/.test(listText))
                            listFormat = NumberingFormat.UpperRoman;
                        else if(/^(ix|iv|v?i{1,3})/.test(listText))
                            listFormat = NumberingFormat.LowerRoman;
                        else if(/[0-9]/.test(listText))
                            listFormat = NumberingFormat.Decimal;
                        else if(/[a-z]/.test(listText))
                            listFormat = NumberingFormat.LowerLetter;
                        else if(/[A-Z]/.test(listText))
                            listFormat = NumberingFormat.UpperLetter;
                        var encodedIndexText = encodeURI(listText);
                        if(listText == "o" || encodedIndexText.indexOf("%B7") > -1 || encodedIndexText.indexOf("%A7") > -1)
                            listFormat = NumberingFormat.Bullet;

                        if(listFormat != NumberingFormat.Bullet) {
                            if(listText.indexOf(".") > -1) {
                                var splittedByDot = listText.split(".");
                                for(var i = 0; i < splittedByDot.length; i++) {
                                    if(splittedByDot[i])
                                        displayFormatString += "{" + i + "}.";
                                }
                            } else {
                                var matches = listText.match(/^(\W?)(\w+)(\W?)/);
                                if(matches && matches.length > 2)
                                    displayFormatString = matches[1] + "{" + listLevelIndex + "}" + matches[3];
                            }
                        }

                        maskedCharacterProperties = this.getMaskedCharacterProperties(listTextElement);

                        if(element.childNodes.length == 1)
                            element.innerHTML = ASPx.Str.TrimStart(element.innerHTML.replace(listText, ''));
                        else {
                            var listTextElementParent = <HTMLHtmlElement>listTextElement.parentNode;
                            listTextElementParent.removeChild(listTextElement);
                            listTextElementParent.innerHTML = listTextElementParent.innerHTML.replace(/^<([^\s>]+)(\s[^>]*)?>(\s|&nbsp;){2,}<\/\1>/g, '');
                            if(listTextElement.parentNode != element)
                                element.innerHTML = element.innerHTML.replace(/<([^\s>]+)(\s[^>]*)?><\/\1>/g, '');
                        }
                    }
                }
                return new ImportedParagraphListInfo(listIndex, listLevelIndex, listFormat, listFormat == NumberingFormat.Bullet ? NumberingType.Bullet : NumberingType.MultiLevel,
                    displayFormatString, maskedCharacterProperties);
            }
            return null;
        }
    }
} 