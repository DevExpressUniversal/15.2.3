//module __aspxRichEdit {
//    export class AutofitTableLayoutCalculator {
//        public static compressTableGrid(grid: TableGridColumn[], startIndex: number, endIndex: number, newTableWidth: number) {
//            const deltas: number[] = [];
//            let deltasTotalWidth: number = 0;
//            let initialTableWidth: number = 0;
//            for (let columnIndex = startIndex; columnIndex <= endIndex; columnIndex++) {
//                const gridColumn: TableGridColumn = grid[columnIndex];
//                initialTableWidth += gridColumn.width;
//                const delta: number = Math.max(gridColumn.width - gridColumn.minWidth, 0);
//                deltas.push(delta);
//                deltasTotalWidth += delta;
//            }

//            const deltaTableWidth = initialTableWidth - newTableWidth;
//            if (deltasTotalWidth > deltaTableWidth)
//                AutofitTableLayoutCalculator.compressProportionallyWidthCore(grid, deltas, deltasTotalWidth, deltaTableWidth);
//            else {
//                for (let columnIndex = startIndex; columnIndex <= endIndex; columnIndex++)
//                    grid[columnIndex].width -= deltas[columnIndex];
//                AutofitTableLayoutCalculator.changeColumnsProportionally(grid, startIndex, endIndex, initialTableWidth - deltasTotalWidth, newTableWidth);
//            }
//        }
            
//        public static enlargeProportionallyAverageWidth(grid: TableGridColumn[], startIndex: number, endIndex: number, newWidth: number) {
//            const averageWidths: number[] = [];
//            let totalWidth: number = 0;
//            for (let columnIndex = startIndex; columnIndex <= endIndex; columnIndex++) {
//                const gridColumn: TableGridColumn = grid[columnIndex];
//                const averageWidth: number = gridColumn.maxWidth + gridColumn.minWidth;
//                averageWidths.push(averageWidth);
//                totalWidth += averageWidth;
//            }

//            let rest: number = newWidth;
//            for (let columnIndex = startIndex; columnIndex <= endIndex; columnIndex++) {
//                const gridColumn: TableGridColumn = grid[columnIndex];
//                const width: number = (columnIndex != endIndex) ? averageWidths[columnIndex] * newWidth / totalWidth : rest;
//                if (width > gridColumn.maxWidth)
//                    gridColumn.maxWidth = gridColumn.minWidth = gridColumn.width = width;
//                else
//                    gridColumn.minWidth = width;
//                rest -= width;
//            }
//        }
//        private static changeColumnsProportionally(grid: TableGridColumn[], startIndex: number, endIndex: number, initialWidth: number, newWidth: number) {
//            const deltaTableWidth: number = Math.abs(newWidth - initialWidth);
//            let rest: number = deltaTableWidth;
//            for (let columnIndex = startIndex; columnIndex <= endIndex; columnIndex++) {
//                const gridColumn: TableGridColumn = grid[columnIndex];
//                const delta: number = (columnIndex != endIndex) ? gridColumn.width * deltaTableWidth / initialWidth : rest;
//                if (initialWidth > newWidth)
//                    gridColumn.width = Math.max(1, gridColumn.width - delta);
//                else
//                    gridColumn.width += delta;
//                rest -= delta;
//            }
//        }

//        private static compressProportionallyWidthCore(grid: TableGridColumn[], deltas: number[], totalItemsWidth: number, deltaTableWidth: number) {
//            const colCount: number = grid.length;
//            if (totalItemsWidth == 0) {
//                let rest: number = deltaTableWidth;
//                for (let columnIndex: number = 0, gridColumn: TableGridColumn; gridColumn = grid[columnIndex]; columnIndex++) {
//                    const delta: number = (columnIndex != colCount - 1) ? deltaTableWidth / colCount : rest;
//                    gridColumn.width = Math.max(gridColumn.width - delta, 0);
//                    rest -= delta;
//                }
//                return;
//            }

//            for (let columnIndex: number = 0, gridColumn: TableGridColumn; gridColumn = grid[columnIndex]; columnIndex++) {
//                const item: number = deltas[columnIndex];
//                const delta: number = item * deltaTableWidth / totalItemsWidth;
//                gridColumn.width -= delta;
//                deltaTableWidth -= delta;
//                totalItemsWidth -= item;
//                if (totalItemsWidth == 0)
//                    break;
//            }
//        }
//    }
//}