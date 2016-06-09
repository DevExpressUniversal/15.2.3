module __aspxRichEdit {
    export class ColumnsCalculator {
        unitConverter: IUnitConverter;
        constructor(unitConverter: IUnitConverter) {
            this.unitConverter = unitConverter;
        }

        public generateSectionColumns(properties: SectionProperties): Rectangle[]{
            var availablePageWidth: number = properties.pageWidth - (properties.marginLeft + properties.marginRight + properties.space * (properties.columnCount - 1));
            var availablePageHeight: number = properties.pageHeight - (properties.marginTop + properties.marginBottom);
            var columnOffsetX = properties.marginLeft;
            var sectionColumns: Rectangle[] = [];
            for(var i = 0; i < properties.columnCount; i++) {
                var columnWidth = Math.max(properties.equalWidthColumns ? Math.floor(availablePageWidth / (properties.columnCount - i)) : properties.columnsInfo[i].width, 1);
                var column = new Rectangle().init(
                    this.unitConverter.toPixels(columnOffsetX),
                    this.unitConverter.toPixels(properties.marginTop),
                    Math.max(this.unitConverter.toPixels(columnWidth), 1),
                    this.unitConverter.toPixels(availablePageHeight)
                );
                sectionColumns.push(column);
                columnOffsetX += columnWidth + (properties.equalWidthColumns ? properties.space : properties.columnsInfo[i].space);
                availablePageWidth -= columnWidth;
            }
            return sectionColumns;
        }

        public findMinimalColumnSize(properties: SectionProperties): Size {
            var columnRects: Rectangle[] = this.generateSectionColumns(properties);
            var size: Size = new Size(columnRects[0].width, columnRects[0].height);
            for (var i = 0, columnRect: Rectangle; columnRect = columnRects[i]; i++) {
                if (columnRect.width < size.width) {
                    size.width = columnRect.width;
                    size.height = columnRect.height;
                }
            }
            return size;
        }
    }
} 