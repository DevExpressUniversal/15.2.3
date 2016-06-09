module __aspxRichEdit {
    export class HtmlExporter {
        control: IRichEditControl;
        rangeCopy: RangeCopy = null;

        constructor(control: IRichEditControl) {
            this.control = control;
        }

        public getHtmlElementsByInterval(interval: FixedInterval): string {
            if(interval.length === 0)
                return "";

            var model = this.control.model;
            this.rangeCopy = ModelManipulator.createRangeCopy(model.activeSubDocument, [interval]);
            var unitConverter = this.control.units;
            var unitTypeToString = unitConverter.ui == RichEditUnit.Centimeter ? "cm" : "in";
            var iterator = model.activeSubDocument.getConstRunIterator(interval);
            var remainLength = interval.length;
            var currentPosition = interval.start;
            var renderer: DocumentRenderer = this.control.getDocumentRenderer();
            var result = "";

            var paragraphsInInterval = model.activeSubDocument.getParagraphsByInterval(interval);
            var paragraphs: Paragraph[] = [];
            for(var i = 0, paragraphInInterval: Paragraph; paragraphInInterval = paragraphsInInterval[i]; i++) {
                if(interval.contains(paragraphInInterval.getEndPosition()))
                    paragraphs.push(paragraphInInterval);
            }

            var listsInInterval: { numberingListIndex: number; listLevelIndex: number; start: number; end: number }[] = [];
            for(var i = 0, paragraph: Paragraph; paragraph = paragraphs[i]; i++) {
                if(paragraph.isInList()) {
                    var paragraphNumberingListIndex = paragraph.getNumberingListIndex();
                    var paragraphListLevelIndex = paragraph.getListLevelIndex();
                    var paragraphStart = paragraph.startLogPosition.value;
                    var paragraphEnd = paragraph.getEndPosition();
                    var existingItem = null;
                    for(var j = 0; j < listsInInterval.length; j++) {
                        if(listsInInterval[j].numberingListIndex == paragraphNumberingListIndex && listsInInterval[j].listLevelIndex == paragraphListLevelIndex)
                            existingItem = listsInInterval[j];
                    }
                    if(existingItem && (paragraphListLevelIndex == 0 || existingItem.end == paragraphStart
                        || listsInInterval[listsInInterval.length - 1].listLevelIndex > paragraphListLevelIndex)) {
                        existingItem.end = paragraphEnd;
                    } else {
                        listsInInterval.push({
                            numberingListIndex: paragraphNumberingListIndex, listLevelIndex: paragraphListLevelIndex,
                            start: paragraphStart, end: paragraphEnd
                        });
                    }
                    var listLevelIndex = paragraphListLevelIndex;
                    while(listLevelIndex > 0) {
                        var parentItem = null;
                        for(var j = 0; j < listsInInterval.length; j++) {
                            if(listsInInterval[j].listLevelIndex == listLevelIndex - 1)
                                parentItem = listsInInterval[j];
                        }
                        if(parentItem)
                            parentItem.end = paragraphEnd;
                        listLevelIndex--;
                    }
                }
            }

            var isInsideFieldCode = false;
            var hyperlinkInfo = null;
            while(iterator.moveNext()) {
                let tableCell = Table.getTableCellByPosition(model.activeSubDocument.tables, iterator.currentInterval().start);
                let isContinueMergingCell = tableCell && tableCell.verticalMerging === TableCellMergingState.Continue;
                var listToStartIndex = -1;
                var listsToEndIndices: number[] = [];

                if(!tableCell) {
                    if(listsInInterval.length) {
                        let currentPosition = iterator.currentInterval().start;
                        for(var i = 0; i < listsInInterval.length; i++) {
                            if(listsInInterval[i].start == currentPosition)
                                listToStartIndex = i;
                            if(listsInInterval[i].end == iterator.currentInterval().end())
                                listsToEndIndices.push(i);
                        }

                        if(listToStartIndex < 0 && currentPosition == interval.start) {
                            var firstParagraph = model.activeSubDocument.getParagraphByPosition(currentPosition);
                            if(firstParagraph.getNumberingListIndex() == listsInInterval[0].numberingListIndex)
                                listToStartIndex = 0;
                        }
                    }

                    if(listToStartIndex > -1) {
                        var numberingList = model.numberingLists[listsInInterval[listToStartIndex].numberingListIndex];
                        var listFormatType = "";
                        switch(numberingList.levels[listsInInterval[listToStartIndex].listLevelIndex].getListLevelProperties().format) {
                            case NumberingFormat.Bullet:
                                listFormatType = "disc";
                                break;
                            case NumberingFormat.Decimal:
                                listFormatType = "decimal";
                                break;
                            case NumberingFormat.LowerLetter:
                                listFormatType = "lower-alpha";
                                break;
                            case NumberingFormat.UpperLetter:
                                listFormatType = "upper-alpha";
                                break;
                            case NumberingFormat.LowerRoman:
                                listFormatType = "lower-roman";
                                break;
                            case NumberingFormat.UpperRoman:
                                listFormatType = "upper-roman";
                                break;
                            default:
                                break;
                        }
                        result += "<" + (numberingList.getListType() != NumberingType.Bullet ? "ol" : "ul") + " style=\"list-style-type:" + listFormatType + "\">";
                    }
                }

                var run = iterator.currentRun;
                var isRunInEmptyParagraph = run.paragraph.length === 1;
                if(paragraphs.length && (run.type != TextRunType.ParagraphRun || isRunInEmptyParagraph)) {
                    var paragraphToStartIndex = Utils.normedBinaryIndexOf(paragraphs,
                        (p: Paragraph) => p.startLogPosition.value - currentPosition);
                    if(paragraphToStartIndex > -1) {
                        var currentParagraph = paragraphs[paragraphToStartIndex];
                        paragraphs.splice(paragraphToStartIndex, 1);

                        if(tableCell) {
                            let parentRow = tableCell.parentRow;
                            let parentTable = parentRow.parentTable;
                            let paragraphStartPosition = currentParagraph.startLogPosition.value;
                            if(parentTable.getStartPosition() == paragraphStartPosition) {
                                //TODO
                                result += "<table style=\"border-spacing: 0px; border-collapse: collapse;\"><tbody>";
                            }
                            if(parentRow.getStartPosition() == paragraphStartPosition)
                                result += "<tr>";
                            if(tableCell.startParagraphPosition.value == paragraphStartPosition && !isContinueMergingCell) {
                                let rowSpan = 1;
                                if(tableCell.verticalMerging === TableCellMergingState.Restart) {
                                    let rowIndex = parentTable.rows.indexOf(parentRow);
                                    let cellIndex = parentRow.cells.indexOf(tableCell);
                                    for(let i = rowIndex + 1, row: TableRow; row = parentTable.rows[i]; i++) {
                                        let nextRowCellIndex = cellIndex;
                                        if(row.cells.length != parentRow.cells.length) {
                                            let extraCellsCount = 0;
                                            let isNextRowLonger = row.cells.length > parentRow.cells.length;
                                            let shorterRow = isNextRowLonger ? parentRow : row;
                                            for(let j = 0; (j < cellIndex) && (j < shorterRow.cells.length); j++) {
                                                extraCellsCount += shorterRow.cells[j].columnSpan - 1;
                                                if(!isNextRowLonger)
                                                    extraCellsCount -= parentRow.cells[j].columnSpan - 1;
                                            }
                                            nextRowCellIndex += (isNextRowLonger ? 1 : -1) * extraCellsCount;
                                        }
                                        let nextRowCell = row.cells[nextRowCellIndex];
                                        if(nextRowCell && nextRowCell.verticalMerging === TableCellMergingState.Continue)
                                            rowSpan++;
                                        else
                                            break;
                                    }
                                }
                                result += "<td style=\"border: solid 1px; width: " + UnitConverter.twipsToPixels(tableCell.preferredWidth.value) + "px;\"" + (rowSpan > 1 ? " rowspan=\"" + rowSpan + "\"" : "") +
                                    (tableCell.columnSpan > 1 ? " colspan=\"" + tableCell.columnSpan + "\"" : "") + ">";
                            }
                        }

                        if(!isContinueMergingCell) {
                            var maskedParagraphProperties: ParagraphProperties = currentParagraph.getParagraphMergedProperies();
                            var paragraphStyle = "";
                            var firstLineIndentType = maskedParagraphProperties.firstLineIndentType;
                            if(firstLineIndentType != ParagraphFirstLineIndent.None) {
                                paragraphStyle += "text-indent: " + (firstLineIndentType == ParagraphFirstLineIndent.Hanging ? "-" : "") +
                                    unitConverter.twipsToUI(maskedParagraphProperties.firstLineIndent) + unitTypeToString + ";";
                            }
                            if(maskedParagraphProperties.alignment != ParagraphAlignment.Unspecified) {
                                paragraphStyle += "text-align: ";
                                switch(maskedParagraphProperties.alignment) {
                                    case ParagraphAlignment.Left:
                                        paragraphStyle += "left;";
                                        break;
                                    case ParagraphAlignment.Right:
                                        paragraphStyle += "right;"
                                        break;
                                    case ParagraphAlignment.Justify:
                                        paragraphStyle += "justify;"
                                        break;
                                    case ParagraphAlignment.Center:
                                        paragraphStyle += "center;";
                                        break;
                                    default:
                                        break;
                                }
                            }
                            if(maskedParagraphProperties.lineSpacingType != ParagraphLineSpacingType.Single) {
                                var lineSpacingInUI = unitConverter.twipsToUI(maskedParagraphProperties.lineSpacing) + unitTypeToString + ";"
                                paragraphStyle += "line-height: ";
                                switch(maskedParagraphProperties.lineSpacingType) {
                                    case ParagraphLineSpacingType.AtLeast:
                                        paragraphStyle += lineSpacingInUI;
                                        break;
                                    case ParagraphLineSpacingType.Double:
                                        paragraphStyle += "2;";
                                        break;
                                    case ParagraphLineSpacingType.Exactly:
                                        paragraphStyle += lineSpacingInUI + "mso-line-height-rule: exactly;";
                                        break;
                                    case ParagraphLineSpacingType.Multiple:
                                        paragraphStyle += maskedParagraphProperties.lineSpacing + ";";
                                        break;
                                    case ParagraphLineSpacingType.Sesquialteral:
                                        paragraphStyle += "1.5;";
                                        break;
                                    default:
                                        break;
                                }
                            }
                            if(ColorHelper.getAlpha(maskedParagraphProperties.backColor) > 0)
                                paragraphStyle += "background: " + ColorHelper.getCssStringInternal(maskedParagraphProperties.backColor) + ";";
                            if(maskedParagraphProperties.leftIndent)
                                paragraphStyle += "margin-left: " + unitConverter.twipsToUI(maskedParagraphProperties.leftIndent) + unitTypeToString + ";";
                            if(maskedParagraphProperties.rightIndent)
                                paragraphStyle += "margin-right: " + unitConverter.twipsToUI(maskedParagraphProperties.rightIndent) + unitTypeToString + ";";
                            paragraphStyle += "margin-top: " + unitConverter.twipsToUI(maskedParagraphProperties.spacingBefore) + unitTypeToString + ";";
                            paragraphStyle += "margin-bottom: " + unitConverter.twipsToUI(maskedParagraphProperties.spacingAfter) + unitTypeToString + ";";
                            var topBorderStyle = this.getBorderCssString(maskedParagraphProperties.topBorder);
                            if(topBorderStyle)
                                paragraphStyle += "border-top:" + topBorderStyle + ";";
                            var leftBorderStyle = this.getBorderCssString(maskedParagraphProperties.leftBorder);
                            if(leftBorderStyle)
                                paragraphStyle += "border-left:" + leftBorderStyle + ";";
                            var bottomBorderStyle = this.getBorderCssString(maskedParagraphProperties.bottomBorder);
                            if(bottomBorderStyle)
                                paragraphStyle += "border-bottom:" + bottomBorderStyle + ";";
                            var rightBorderStyle = this.getBorderCssString(maskedParagraphProperties.rightBorder);
                            if(rightBorderStyle)
                                paragraphStyle += "border-right:" + rightBorderStyle + ";";
                            if(isRunInEmptyParagraph)
                                paragraphStyle += HtmlConverter.getCssRules(run.getCharacterMergedProperies(), false, false).join(";");

                            result += (currentParagraph.isInList() && !tableCell ? "<li>" : "") + "<p" + (paragraphStyle ? " style=\"" + paragraphStyle + "\"" : "") + ">";
                            if(isRunInEmptyParagraph)
                                result += "<br>";
                        }
                    }
                }

                var html = "";
                var innerHtml = "";
                var type = LayoutBoxType.Text;
                var length = Math.min(remainLength, iterator.currentInterval().length);
                switch(run.type) {
                    case TextRunType.ParagraphRun:
                        if(!isContinueMergingCell) {
                            html = "</p>";
                            let paragraphEndPosition = run.paragraph.getEndPosition();
                            if(tableCell) {
                                let parentRow = tableCell.parentRow;
                                let parentTable = parentRow.parentTable;
                                if(tableCell.endParagrapPosition.value == paragraphEndPosition)
                                    html += "</td>";
                                if(parentRow.getEndPosition() == paragraphEndPosition)
                                    html += "</tr>";
                                if(parentTable.getEndPosition() == paragraphEndPosition)
                                    html += "</tbody></table>";
                            }
                        }
                        break;
                    case TextRunType.InlinePictureRun:
                        var picRun: InlinePictureRun = <InlinePictureRun>run;
                        var pictureBox: LayoutPictureBox = new LayoutPictureBox(run.getCharacterMergedProperies(), picRun.id,
                                                                                UnitConverter.twipsToPixels(picRun.getActualWidth()),
                                                                                UnitConverter.twipsToPixels(picRun.getActualHeight()));
                        innerHtml = renderer.renderPicture(pictureBox);
                        type = LayoutBoxType.Picture;
                        break;
                    case TextRunType.FieldCodeStartRun:
                        isInsideFieldCode = true;
                        var fieldIndex: number = Field.normedBinaryIndexOf(model.activeSubDocument.fields, currentPosition + 1);
                        var field: Field = model.activeSubDocument.fields[fieldIndex];
                        if(field.isHyperlinkField())
                            hyperlinkInfo = field.getHyperlinkInfo();
                        break;
                    case TextRunType.FieldCodeEndRun:
                        isInsideFieldCode = false;
                        break;
                    case TextRunType.FieldResultEndRun:
                        break;
                    default:
                        if(!isInsideFieldCode) {
                            var text = model.activeSubDocument.getText(new FixedInterval(currentPosition, length));
                            innerHtml = text.length == 1 && text.charAt(0) == model.specChars.PageBreak ? "<br style=\"page-break-before:always\">" : Utils.encodeHtml(text);
                        }
                        break;
                }
                
                if(!html && innerHtml) {
                    var characterProperties = run.getCharacterMergedProperies();
                    var boxStyle = "white-space:pre;" + HtmlConverter.getCssRules(run.getCharacterMergedProperies(), run.type == TextRunType.TextRun, false).join(";");
                    html = "<span style=\"" + boxStyle + "\">" + innerHtml + "</span>";
                    if(hyperlinkInfo) {
                        var url = hyperlinkInfo.uri + (hyperlinkInfo.anchor != "" ? "#" + hyperlinkInfo.anchor : "");
                        var tooltip = hyperlinkInfo.tip;
                        html = "<a href=\"" + url + "\" title=\"" + tooltip + "\">" + html + "</a>";
                        hyperlinkInfo = null;
                    } else {
                        if(characterProperties.fontUnderlineType != UnderlineType.None && !characterProperties.underlineWordsOnly) {
                            var underlineColor = characterProperties.underlineColor;
                            var cssColorValue = (underlineColor != ColorHelper.AUTOMATIC_COLOR) ? ColorHelper.getCssStringInternal(underlineColor) : "initial"; // maybe repace initial?
                            html = "<span style=\"text-decoration: underline; color: " + cssColorValue + "\">" + html + "</span>";
                        }
                        if(characterProperties.script !== CharacterFormattingScript.Normal)
                            html = "<span style=\"font-size: " + characterProperties.fontSize + "px;\">" + html + "</span>";
                        if(ColorHelper.getAlpha(characterProperties.backColor) > 0)
                            html = "<span style=\"background: " + ColorHelper.getCssStringInternal(characterProperties.backColor) + "\">" + html + "</span>";
                    }
                }

                result += html;
                if(listsToEndIndices.length) {
                    for(var i = listsToEndIndices.length - 1; i >= 0; i--)
                        result += "</" + (model.numberingLists[listsInInterval[listsToEndIndices[i]].numberingListIndex].getListType() != NumberingType.Bullet ? "ol" : "ul") + ">";
                }

                currentPosition += length;
                remainLength -= length;
            }
            if(/^<td[^>]*>/gi.test(result))
                result = "<tr>" + result;
            if(/<\/td>$/gi.test(result))
                result += "</tr>";
            if(/^<tr[^>]*>/gi.test(result))
                result = "<table style=\"border-spacing: 0px; border-collapse: collapse;\"><tbody>" + result;
            if(/<\/tr>$/gi.test(result))
                result += "</tbody></table>";
            return result ? "<i data-re-modelpartcopy=\"true\" style=\"font-style:normal; display:inline-block\">" + result + "</i>" : "";
        }

        private getBorderCssString(borderInfo: BorderInfo): string {
            var borderStyle = "";
            if(borderInfo) {
                if(borderInfo.width)
                    borderStyle += " " + UnitConverter.twipsToPixels(borderInfo.width);
                switch(borderInfo.style) {
                    case BorderLineStyle.Dashed:
                        borderStyle += " dashed";
                        break;
                    case BorderLineStyle.Dotted:
                        borderStyle += " dotted";
                        break;
                    case BorderLineStyle.Double:
                        borderStyle += " double";
                        break;
                    case BorderLineStyle.Inset:
                        borderStyle += " inset";
                        break;
                    case BorderLineStyle.None:
                        borderStyle += " none";
                        break;
                    case BorderLineStyle.Outset:
                        borderStyle += " outset";
                        break;
                    default:
                        break;
                }
                if (ColorHelper.getAlpha(borderInfo.color) > 0)
                    borderStyle += " " + ColorHelper.getCssStringInternal(borderInfo.color);
            }
            return borderStyle;
        }
    }
} 