module __aspxRichEdit {
    var constants = {
        IMAGELOADINGURL: '<%=WebResource("custom:ImageLoadingUrl")%>'
    };

    export class DocumentRendererPageCache {
        page: HTMLElement;
        areas: { [id: number]: HTMLElement };

        constructor(page: HTMLElement) {
            this.page = page;
            this.areas = {};
        }

        isConsiderRenderedAreas(): boolean {
            for (let key in this.areas) {
                if (!this.areas.hasOwnProperty(key))
                    continue;
                if (this.areas[key])
                    return true;
            }
            return false;
        }
    }

    export class DocumentRenderer {
        static CLASSNAMES = {
            PAGE: "dxre-page",
            ROW: "dxre-row",
            BOX: "dxre-box",
            START_BOOKMARK: "dxre-startbookmark",
            END_BOOKMARK: "dxre-endbookmark",
            BOX_SPACE: "dxre-boxspace",
            BOX_BG: "dxre-boxbg",
            COLUMN: "dxre-column",
            PAGE_AREA: "dxre-pagearea",
            ACTIVE: "dxre-active",
            PARAGRAPH_FRAME: "dxre-pframe",
            PICTURE: "dxre-pic",
            DRAG_CARET: "dxre-dragcaret",
            HIDDEN_BOX: "dxre-hiddenbox",
            FIELD_BOX_LEVEL1: "dxre-fieldboxlevel1",
            FIELD_BOX_LEVEL2: "dxre-fieldboxlevel2",
            FIELD_BOX_LEVEL3: "dxre-fieldboxlevel3",
            FIELD_BG: "dxre-fieldbg",
            HEADER_INFO: "dxre-headerinfo",
            FOOTER_INFO: "dxre-footerinfo",
            BACKLIGHT_MODE: "dxre-backlightmode",
            HEADERFOOTER_AREA: "dxre-headerfooterarea",

            TABLE: "dxre-table",
            TABLE_BORDER: "dxre-table-brd",
            TABLE_BG: "dxre-table-bg",

            ROWS_CONTAINER: "dxre-rows",
            PARAGRAPHFRAMES_CONTAINER: "dxre-pframes",
            TABLES_CONTAINER: "dxre-tables",
            SELECTION_CONTAINER: "dxre-selection",

            SELECTION_ROW: "dxre-sel-row",
            SELECTION_CURSOR: "dxre-sel-cursor"
        };

        canvas: HTMLDivElement;
        cache: DocumentRendererPageCache[] = [];
        handlerURI: string;
        emptyImageCacheId: number;

        private dragCaretElement: HTMLElement;
        selection: SelectionRenderer;

        constructor(canvas: HTMLDivElement, selectionCache: CanvasSelectionCache) {
            this.canvas = canvas;
            this.selection = new SelectionRenderer(this, selectionCache);
        }

        updateLastTimeMoveCursor(): void {
            this.selection.updateLastTimeMoveCursor();
        }
        /* Page */
        public updatePageSize(page: LayoutPage) {
            const pageCache: DocumentRendererPageCache = this.cache[page.index];
            this.updatePageSizeInner(page, pageCache.page);
        }
        public applyPageChanges(pageChange: PageChange, isShowGridlines: boolean) {
            const pageIndex: number = pageChange.index;
            //Logging.print(LogSource.DocumentRenderer, "applyPageChanges", ["pageIndex", pageIndex]);
            for (let pageAreaChanges of pageChange.pageAreaChanges) {
                const pageAreaSubDocId: number = pageAreaChanges.layoutElement.subDocument.id;
                switch (pageAreaChanges.changeType) {
                    case LayoutChangeType.Deleted: this.removePageArea(pageIndex, pageAreaSubDocId, pageAreaChanges.index); break;
                    case LayoutChangeType.Added: this.insertPageArea(pageIndex, pageAreaSubDocId, pageAreaChanges.index, pageAreaChanges.layoutElement, isShowGridlines); break;
                    case LayoutChangeType.Replaced: this.replacePageArea(pageIndex, pageAreaSubDocId, pageAreaChanges.index, pageAreaChanges.layoutElement, isShowGridlines); break;
                    case LayoutChangeType.Updated: this.updatePageAreaContent(pageIndex, pageAreaSubDocId, pageAreaChanges.index, pageAreaChanges.columnChanges, isShowGridlines); break;
                    default: throw new Error(Errors.InternalException);
                }
            }
        }
        // full render
        public renderPage(layoutPage: LayoutPage, renderInnerContent: boolean, pageColor: number, isShowGridlines: boolean) {
            const pageIndex: number = layoutPage.index;
            const pageCache: DocumentRendererPageCache = this.cache[pageIndex];
            if (pageCache) {
                this.updatePageSizeInner(layoutPage, pageCache.page);
                if (renderInnerContent) {
                    if (pageCache.page.childNodes.length)
                        this.removePageContent(pageIndex);
                    this.innerRenderPageContent(layoutPage, pageIndex, isShowGridlines);
                }
                else
                    this.removePageContent(pageIndex);
            }
            else {
                this.insertPage(layoutPage, pageColor);
                if (renderInnerContent)
                    this.innerRenderPageContent(layoutPage, pageIndex, isShowGridlines);
            }
        }
        insertPage(page: LayoutPage, pageColor: number) {
            const pageElement: HTMLElement = document.createElement("DIV");
            pageElement.className = DocumentRenderer.CLASSNAMES.PAGE;
            if (!ColorHelper.isEmptyBgColor(pageColor))
                pageElement.style.backgroundColor = ColorHelper.colorToHash(pageColor);
            this.updatePageSizeInner(page, pageElement);
            const nextPageCache: DocumentRendererPageCache = this.cache[page.index];
            if(nextPageCache)
                this.canvas.insertBefore(pageElement, nextPageCache.page);
            else
                this.canvas.appendChild(pageElement);
            this.cache.splice(page.index, 0, new DocumentRendererPageCache(pageElement));
        }
        removePage(pageIndex: number) {
            const pageCache: DocumentRendererPageCache = this.cache[pageIndex];
            if(pageCache) {
                pageCache.page.parentNode.removeChild(pageCache.page);
                this.cache.splice(pageIndex, 1);
            }
        }
        public removePageContent(pageIndex: number) {
            const pageCache: DocumentRendererPageCache = this.cache[pageIndex];
            if(pageCache) {
                pageCache.page.innerHTML = "";
                pageCache.areas = {};
            }
            this.selection.removeItemsByPage(pageIndex, false);
        }
        private removePageArea(pageIndex: number, subDocumentId: number, pageAreaIndex: number): void {
            const pageCache: DocumentRendererPageCache = this.cache[pageIndex];
            const id: number = DocumentRenderer.getAreaCacheId(subDocumentId, pageAreaIndex);
            pageCache.page.removeChild(pageCache.areas[id]);
            delete pageCache.areas[id];
            this.selection.removeItemsFromCacheByPageArea(pageIndex, subDocumentId, pageAreaIndex);
        }
        private insertPageArea(pageIndex: number, subDocumentId: number, pageAreaIndex: number, pageArea: LayoutPageArea, isShowGridlines: boolean): void {
            const pageCache: DocumentRendererPageCache = this.cache[pageIndex];
            const id: number = DocumentRenderer.getAreaCacheId(subDocumentId, pageAreaIndex);
            const areaElement: HTMLElement = this.renderPageArea(pageArea, isShowGridlines);
            pageCache.page.appendChild(areaElement);
            pageCache.areas[id] = areaElement;
        }
        private replacePageArea(pageIndex: number, subDocumentId: number, pageAreaIndex: number, pageArea: LayoutPageArea, isShowGridlines: boolean): void {
            const pageCache: DocumentRendererPageCache = this.cache[pageIndex];
            const id: number = DocumentRenderer.getAreaCacheId(subDocumentId, pageAreaIndex);
            let areaElement = this.renderPageArea(pageArea, isShowGridlines);
            pageCache.page.replaceChild(areaElement, pageCache.areas[id]);
            pageCache.areas[id] = areaElement;
            this.selection.removeItemsFromCacheByPageArea(pageIndex, subDocumentId, pageAreaIndex);
        }
        private updatePageAreaContent(pageIndex: number, subDocumentId: number, pageAreaIndex: number, columnChanges: ColumnChange[], isShowGridlines: boolean) {
            const pageCache: DocumentRendererPageCache = this.cache[pageIndex];
            const id: number = DocumentRenderer.getAreaCacheId(subDocumentId, pageAreaIndex);
            const pageAreaElement: HTMLElement = pageCache.areas[id];
            for (let columnChange of columnChanges) {
                switch (columnChange.changeType) {
                    case LayoutChangeType.Deleted: this.removeColumn(pageAreaElement, pageIndex, subDocumentId, pageAreaIndex, columnChange.index); break;
                    case LayoutChangeType.Added: this.insertColumn(pageAreaElement, columnChange.index, columnChange.layoutElement, isShowGridlines); break;
                    case LayoutChangeType.Replaced: this.replaceColumn(pageAreaElement, pageIndex, subDocumentId, pageAreaIndex, columnChange.index, columnChange.layoutElement, isShowGridlines); break;
                    case LayoutChangeType.Updated: this.updateColumnContent(pageAreaElement, columnChange.index, columnChange.layoutElement, columnChange, isShowGridlines); break;
                    default: throw new Error(Errors.InternalException);
                }
            }
        }
        private static getAreaCacheId(subDocumentId: number, pageAreaIndex: number): number {
            return subDocumentId === SubDocument.MAIN_SUBDOCUMENT_ID ? -pageAreaIndex : subDocumentId;
        }
        private removeColumn(pageAreaElement: HTMLElement, pageIndex: number, subDocumentId: number, pageAreaIndex: number, columnIndex: number): void {
            this.selection.removeItemsFromCacheByColumn(pageIndex, subDocumentId, pageAreaIndex, columnIndex);
            pageAreaElement.removeChild(pageAreaElement.childNodes[columnIndex]);
        }
        private insertColumn(pageAreaElement: HTMLElement, columnIndex: number, column: LayoutColumn, isShowGridlines: boolean): void {
            pageAreaElement.insertBefore(this.renderColumn(column, columnIndex, isShowGridlines), pageAreaElement.childNodes[columnIndex + 1]);
        }
        private replaceColumn(pageAreaElement: HTMLElement, pageIndex: number, subDocumentId: number, pageAreaIndex: number, columnIndex: number, column: LayoutColumn, isShowGridlines: boolean): void {
            this.selection.removeItemsFromCacheByColumn(pageIndex, subDocumentId, pageAreaIndex, columnIndex);
            pageAreaElement.replaceChild(this.renderColumn(column, columnIndex, isShowGridlines), pageAreaElement.childNodes[columnIndex]);
        }
        private updateColumnContent(pageAreaElement: HTMLElement, columnIndex: number, column: LayoutColumn, columnChange: ColumnChange, isShowGridlines: boolean) {
            const columnElement: HTMLElement = <HTMLElement>(pageAreaElement.childNodes[columnIndex]);
            this.updateParagraphFrames(column, columnElement); // TODO
            for (let rowChange of columnChange.rowChanges) {
                switch(rowChange.changeType) {
                    case LayoutChangeType.Deleted: this.removeRow(columnElement, rowChange.index); break;
                    case LayoutChangeType.Replaced: this.replaceRow(columnElement, rowChange.index, rowChange.layoutElement); break;
                    case LayoutChangeType.Inserted: this.insertRow(columnElement, rowChange.index, rowChange.layoutElement); break;
                    case LayoutChangeType.Added: this.appendRow(columnElement, rowChange.layoutElement); break;
                    default: throw new Error(Errors.InternalException);
                }
            }
            
            for (let tableChange of columnChange.tableChanges) {
                switch (tableChange.changeType) {
                    case LayoutChangeType.Deleted: this.removeTable(columnElement, tableChange.index); break;
                    case LayoutChangeType.Replaced: this.replaceTable(columnElement, tableChange.index, tableChange.layoutElement, isShowGridlines); break;
                    case LayoutChangeType.Inserted: this.insertTable(columnElement, tableChange.index, tableChange.layoutElement, isShowGridlines); break;
                    case LayoutChangeType.Added: this.appendTable(columnElement, tableChange.index, tableChange.layoutElement, isShowGridlines); break;
                    default: throw new Error(Errors.InternalException);
                }
            }
        }
        private removeTable(columnElement: HTMLElement, tableIndex: number) {
            const container: Node = this.getTablesContainerCore(columnElement);
            //Logging.print(LogSource.DocumentRenderer, "removeTable", ["container.childNodes.length", container.childNodes.length], ["tableIndex", tableIndex]);
            container.removeChild(container.childNodes[tableIndex]);
        }
        private replaceTable(columnElement: HTMLElement, tableIndex: number, columnInfo: LayoutTableColumnInfo, isShowGridlines: boolean) {
            const container: Node = this.getTablesContainerCore(columnElement);
            //Logging.print(LogSource.DocumentRenderer, "replaceTable", ["container.childNodes.length", container.childNodes.length], ["tableIndex", tableIndex]);
            container.replaceChild(this.renderTable(columnInfo, isShowGridlines), container.childNodes[tableIndex]);
        }
        private insertTable(columnElement: HTMLElement, tableIndex: number, columnInfo: LayoutTableColumnInfo, isShowGridlines: boolean) {
            const container: Node = this.getTablesContainerCore(columnElement);
            //Logging.print(LogSource.DocumentRenderer, "insertTable", ["container.childNodes.length", container.childNodes.length], ["tableIndex", tableIndex]);
            container.insertBefore(this.renderTable(columnInfo, isShowGridlines), container.childNodes[tableIndex]);
        }
        private appendTable(columnElement: HTMLElement, tableIndex: number, columnInfo: LayoutTableColumnInfo, isShowGridlines: boolean) {
            const container: Node = this.getTablesContainerCore(columnElement);
            //Logging.print(LogSource.DocumentRenderer, "appendTable", ["container.childNodes.length", container.childNodes.length]);
            container.appendChild(this.renderTable(columnInfo, isShowGridlines));
        }
        private removeRow(columnElement: HTMLElement, rowIndex: number): void {
            const container: Node = this.getRowsContainerCore(columnElement);
            //Logging.print(LogSource.DocumentRenderer, "removeRow", ["container.childNodes.length", container.childNodes.length], ["rowIndex", rowIndex]);
            container.removeChild(container.childNodes[rowIndex]);
        }
        private appendRow(columnElement: HTMLElement, row: LayoutRow): void {
            const container: Node = this.getRowsContainerCore(columnElement);
            //Logging.print(LogSource.DocumentRenderer, "appendRow", ["container.childNodes.length", container.childNodes.length]);
            container.appendChild(this.renderRow(row));
        }
        private insertRow(columnElement: HTMLElement, rowIndex: number, row: LayoutRow): void {
            const container: Node = this.getRowsContainerCore(columnElement);
            //Logging.print(LogSource.DocumentRenderer, "insertRow", ["container.childNodes.length", container.childNodes.length], ["rowIndex", rowIndex]);
            container.insertBefore(this.renderRow(row), container.childNodes[rowIndex]);
        }
        private replaceRow(columnElement: HTMLElement, rowIndex: number, row: LayoutRow): void {
            const container: Node = this.getRowsContainerCore(columnElement);
            //Logging.print(LogSource.DocumentRenderer, "replaceRow", ["container.childNodes.length", container.childNodes.length], ["rowIndex", rowIndex]);
            container.replaceChild(this.renderRow(row), container.childNodes[rowIndex]);
        }
        private updatePageSizeInner(page: LayoutPage, pageElement: HTMLElement) {
            pageElement.style.height = page.height + "px";
            pageElement.style.width = page.width + "px";
        }
        private innerRenderPageContent(page: LayoutPage, pageIndex: number, isShowGridlines: boolean) {
            const pageCache: DocumentRendererPageCache = this.cache[pageIndex];
            for (let pageAreaIndex = 0, area: LayoutPageArea; area = page.mainSubDocumentPageAreas[pageAreaIndex]; pageAreaIndex++) {
                const pageAreaElement: HTMLElement = this.renderPageArea(area, isShowGridlines);
                pageCache.areas[-pageAreaIndex] = pageAreaElement;
                pageCache.page.appendChild(pageAreaElement);
            }
            for(let subDocId in page.otherPageAreas) {
                if (!page.otherPageAreas.hasOwnProperty(subDocId))
                    continue;
                const area: LayoutPageArea = page.otherPageAreas[subDocId];
                const pageAreaElement: HTMLElement = this.renderPageArea(area, isShowGridlines);
                pageCache.areas[subDocId] = pageAreaElement;
                pageCache.page.appendChild(pageAreaElement);
            }
        }
        private renderPageArea(area: LayoutPageArea, isShowGridlines: boolean): HTMLElement {
            var element = document.createElement("DIV");
            let className = DocumentRenderer.CLASSNAMES.PAGE_AREA;
            if(area.subDocument.isHeaderFooter())
                className += " " + DocumentRenderer.CLASSNAMES.HEADERFOOTER_AREA;
            element.className = className;
            element.style.height = area.height + "px";
            element.style.width = area.width + "px";
            element.style.left = area.x + "px";
            element.style.top = area.y + "px";
            for(var i = 0, column; column = area.columns[i]; i++)
                element.appendChild(this.renderColumn(column, i, isShowGridlines));
            return element;
        }
        private renderRowsContainer(): HTMLElement {
            let element = document.createElement("DIV");
            element.className = DocumentRenderer.CLASSNAMES.ROWS_CONTAINER;
            return element;
        }
        private renderTablesContainer(): HTMLElement {
            let element = document.createElement("DIV");
            element.className = DocumentRenderer.CLASSNAMES.TABLES_CONTAINER;
            return element;
        }
        private renderSelectionContainer(): HTMLElement {
            let element = document.createElement("DIV");
            element.className = DocumentRenderer.CLASSNAMES.SELECTION_CONTAINER;
            return element;
        }
        private renderParagraphFramesContainer(): HTMLElement {
            let element = document.createElement("DIV");
            element.className = DocumentRenderer.CLASSNAMES.PARAGRAPHFRAMES_CONTAINER;
            return element;
        }
        private renderColumn(column: LayoutColumn, index: number, isShowGridlines: boolean): HTMLElement {
            const element = document.createElement("DIV");
            element.className = DocumentRenderer.CLASSNAMES.COLUMN;
            element.style.height = column.height + "px";
            element.style.width = column.width + "px";
            element.style.left = column.x + "px";
            element.style.top = column.y + "px";

            let rowsContainer = element.appendChild(this.renderRowsContainer());
            let paragraphFramesContainer = element.appendChild(this.renderParagraphFramesContainer());
            let tablesContainer = element.appendChild(this.renderTablesContainer());
            let selectionContainer = element.appendChild(this.renderSelectionContainer());

            this.renderRowsInContainer(rowsContainer, column);
            this.renderParagraphFramesInContainer(paragraphFramesContainer, column);
            this.renderTablesInContainer(tablesContainer, column, isShowGridlines);

            return element;
        }
        private renderRowsInContainer(container: Node, column: LayoutColumn) {
            for(let i = 0, row: LayoutRow; row = column.rows[i]; i++)
                container.appendChild(this.renderRow(row));
        }
        private renderParagraphFramesInContainer(container: Node, column: LayoutColumn) {
            for(let i = 0, frame: ParagraphFrame; frame = column.paragraphFrames[i]; i++)
                container.appendChild(this.createParagraphFrameElement(frame));
        }
        private renderTablesInContainer(container: Node, column: LayoutColumn, isShowGridlines: boolean) {
            for(let i = 0, table: LayoutTableColumnInfo; table = column.tablesInfo[i]; i++)
                container.appendChild(this.renderTable(table, isShowGridlines));
        }

        private getRowsContainerCore(columnElement: Node): Node {
            return columnElement.firstChild;
        }
        private getParagraphFramesContainerCore(columnElement: Node): Node {
            return ASPx.Browser.Chrome ? columnElement.firstChild.nextSibling : columnElement.childNodes[1];
        }
        private getTablesContainerCore(columnElement: Node): Node {
            return ASPx.Browser.Chrome ? columnElement.lastChild.previousSibling : columnElement.childNodes[2];
        }
        private getSelectionContainerCore(columnElement: Node): Node {
            return columnElement.lastChild;
        }

        getSelectionContainer(subDocumentId: number, pageIndex: number, pageAreaIndex: number, columnIndex: number): Node {
            let pageCache = this.cache[pageIndex];
            let pageAreaElement = pageCache.areas[DocumentRenderer.getAreaCacheId(subDocumentId, pageAreaIndex)];
            return this.getSelectionContainerCore(pageAreaElement.childNodes[columnIndex]);
        }

        // Table
        renderTable(tableColumnInfo: LayoutTableColumnInfo, isShowGridlines: boolean): HTMLElement {
            const tblXPos: number = tableColumnInfo.x;
            const tblYPos: number = tableColumnInfo.y;
            const tableElement = document.createElement("DIV");
            tableElement.style.left = tblXPos + "px";
            tableElement.style.top = tblYPos + "px";
            tableElement.className = DocumentRenderer.CLASSNAMES.TABLE;

            for (let border of tableColumnInfo.horizontalBorders)
                this.renderHorizontalBorder(tableElement, border, isShowGridlines);

            for (let border of tableColumnInfo.verticalBorders)
                this.renderVerticalBorder(tableElement, border, isShowGridlines);

            this.populateBgFrames(tableColumnInfo, tableElement, tblXPos, tblYPos);

            return tableElement;
        }

        private renderTableBackgroundElement(tableElement: HTMLElement, bound: Rectangle, tblXPos: number, tblYPos: number, color: number) {
            if (ColorHelper.isEmptyBgColor(color))
                return;

            const elem: HTMLElement = document.createElement("DIV");
            elem.className = DocumentRenderer.CLASSNAMES.TABLE_BG;
            elem.style.left = bound.x - tblXPos + "px";
            elem.style.top = bound.y - tblYPos + "px";
            elem.style.width = bound.width + "px";
            elem.style.height = bound.height + "px";
            elem.style.backgroundColor = ColorHelper.colorToHash(color);
            tableElement.appendChild(elem);
        }
        private populateBgFrames(table: LayoutTableColumnInfo, tableElement: HTMLElement, xTablePos: number, yTablePos: number) {
            for (let i = 0, tblRow: LayoutTableRowInfo; tblRow = table.tableRows[i]; i++) {
                this.renderTableBackgroundElement(tableElement, tblRow.bound, xTablePos, yTablePos, table.logicInfo.properties.backgroundColor);
                for (let cellBgInfo of tblRow.backgroundInfos)
                    this.renderTableBackgroundElement(tableElement, cellBgInfo.bound, 0, 0, cellBgInfo.color);
            }
        }

        private renderVerticalBorder(tableElement: HTMLElement, border: LayoutTableBorder, isShowGridlines: boolean) {
            const borderInfo: BorderInfo = border.borderInfo;
            if (!borderInfo || borderInfo.style == BorderLineStyle.None || borderInfo.style == BorderLineStyle.Nil || borderInfo.width == 0) {
                if (isShowGridlines) {
                    const element = this.createBorderElement(ColorHelper.LIGHT_COLOR);
                    element.style.top = border.yPos + "px";
                    element.style.left = border.xPos + "px";
                    element.style.height = border.length + "px";
                    element.style.width = "0px";
                    element.style.borderStyle = "None";
                    element.style.borderRightStyle = "dashed";
                    element.style.borderRightWidth = "1px";
                    tableElement.appendChild(element);
                }
            }
            else {
                if (!borderInfo)
                    return;
                const element = this.createBorderElement(borderInfo.color);
                element.style.top = border.yPos + "px";
                element.style.left = border.xPos + "px";
                element.style.height = border.length + "px";
                element.style.width = borderInfo.width + "px";
                tableElement.appendChild(element);
            }
        }
        private renderHorizontalBorder(tableElement: HTMLElement, border: LayoutTableBorder, isShowGridlines: boolean) {
            const borderInfo: BorderInfo = border.borderInfo;
            if (!borderInfo || borderInfo.style == BorderLineStyle.None || borderInfo.style == BorderLineStyle.Nil || borderInfo.width == 0) {
                if (!isShowGridlines)
                    return;
                const element = this.createBorderElement(ColorHelper.LIGHT_COLOR);
                element.style.top = border.yPos + "px";
                element.style.left = border.xPos + "px";
                element.style.width = border.length + "px"
                element.style.height = "0px";

                element.style.borderStyle = "None";
                element.style.borderBottomStyle = "dashed";
                element.style.borderBottomWidth = "1px";
                tableElement.appendChild(element);
            }
            else {
                if (!borderInfo)
                    return;
                const element = this.createBorderElement(borderInfo.color);
                element.style.top = border.yPos + "px";
                element.style.left = border.xPos + "px";
                element.style.width = border.length + "px"
                element.style.height = borderInfo.width + "px";
                tableElement.appendChild(element);
            }
        }
        private createBorderElement(color: number): HTMLElement {
            var element = document.createElement("DIV");
            element.style.backgroundColor = ColorHelper.colorToHash(color);
            element.className = DocumentRenderer.CLASSNAMES.TABLE_BORDER;
            return element;
        }
        
        /* Paragraph Frames */
        private updateParagraphFrames(column: LayoutColumn, columnElement: HTMLElement) {
            let container = this.getParagraphFramesContainerCore(columnElement);
            for(let i = container.childNodes.length - column.paragraphFrames.length; i > 0; i--)
                container.removeChild(container.lastChild);
            let existFramesCount = container.childNodes.length;
            for(var i = 0, frame: ParagraphFrame; frame = column.paragraphFrames[i]; i++) {
                if(i < existFramesCount)
                    this.updateParagraphFrame(frame, <HTMLElement>container.childNodes[i]);
                else
                    container.appendChild(this.createParagraphFrameElement(frame));
            }
        }
        private createParagraphFrameElement(paragraphFrame: ParagraphFrame) {
            var element: HTMLElement = document.createElement("DIV");
            element.className = DocumentRenderer.CLASSNAMES.PARAGRAPH_FRAME;
            this.updateParagraphFrame(paragraphFrame, element);
            return element;
        }
        private updateParagraphFrame(frame: ParagraphFrame, frameElement: HTMLElement) {
            frameElement.style.left = frame.x + "px";
            frameElement.style.top = frame.y + "px";
            frameElement.style.width = frame.width + "px";
            frameElement.style.height = frame.height + "px";
            frameElement.style.background = ColorHelper.getCssStringInternal(frame.paragraphColor);
        }

        /* Rows */
        renderRow(row: LayoutRow): HTMLElement {
            //console.log("\t\t\trenderRow: " + index);
            var element = document.createElement("DIV");
            element.className = DocumentRenderer.CLASSNAMES.ROW;

            element.style.height = (row.height - row.getSpacingBefore()) + "px";
            element.style.width = row.width + "px";
            element.style.left = row.x + "px";
            element.style.top = row.y + row.getSpacingBefore() + "px";
            //element.style.paddingTop = row.getSpacingBefore() + "px";

            var box: LayoutBox;
            var lastBoxIndexWhatCanStrikeoutAndUnderline: number = row.boxes.length - 1;
            for(; box = row.boxes[lastBoxIndexWhatCanStrikeoutAndUnderline]; lastBoxIndexWhatCanStrikeoutAndUnderline--)
                if(!box.renderNoStrikeoutAndNoUnderlineIfBoxInEndRow())
                    break;

            var content = "";
            if(row.numberingListBox) {
                content += this.renderBox(row, row.numberingListBox.textBox, 0, lastBoxIndexWhatCanStrikeoutAndUnderline);
                if(row.numberingListBox.separatorBox)
                    content += this.renderBox(row, row.numberingListBox.separatorBox, 0, lastBoxIndexWhatCanStrikeoutAndUnderline);
            }
            for(var i = 0; box = row.boxes[i]; i++)
                content += this.renderBox(row, box, i, lastBoxIndexWhatCanStrikeoutAndUnderline);

            var bookmarkBox: BookmarkBox;
            for(var i = 0; bookmarkBox = row.bookmarkBoxes[i]; i++)
                content += this.renderBookmark(row, bookmarkBox);

            element.innerHTML = content;
            return element;
        }

        /* Box */
        renderBookmark(row: LayoutRow, box: BookmarkBox): string {
            var className = box.boxType == LayoutBoxType.BookmarkStartBox ? DocumentRenderer.CLASSNAMES.START_BOOKMARK : DocumentRenderer.CLASSNAMES.END_BOOKMARK;
            return `<div class="${className}" style="width: ${box.width}px; height: ${box.height}px; left: ${box.x}px; top: ${box.y}px; border-color: ${box.color}"></div>`;
        }
        renderBox(row: LayoutRow, box: LayoutBox, boxIndex: number, lastBoxIndexWhatCanStrikeoutAndUnderline: number): string {
            var content: string = box.renderGetContent(constants, this.handlerURI, this.emptyImageCacheId);

            var top = row.baseLine - box.getAscent() - row.getSpacingBefore();
            var left = box.x;

            if(box.characterProperties.script === CharacterFormattingScript.Subscript) { // offset is calculated empirically
                var multiplier = box.characterProperties.fontInfo.scriptMultiplier;
                top += UnitConverter.pointsToPixels(box.characterProperties.fontSize) * (box.characterProperties.fontInfo.subScriptOffset * multiplier - multiplier + 1);
            }

            var boxStyles: string[] = [];
            boxStyles.push("left: " + left + "px");
            boxStyles.push("top: " + top + "px");
            boxStyles.push("width: " + box.width + "px");
            boxStyles.push("height: " + box.height + "px");

            var noNeedUnderlineAndStrikeout: boolean = boxIndex > lastBoxIndexWhatCanStrikeoutAndUnderline; // and backColor
            var charProps: CharacterProperties = box.renderGetCharacterProperties();
            var underlineColor: number = charProps.underlineColor;
            var strikeoutColor: number = charProps.strikeoutColor;
            var foreColor: number = charProps.foreColor;

            var needUnderline: boolean = !noNeedUnderlineAndStrikeout && (charProps.fontUnderlineType != UnderlineType.None) && (box.getType() != LayoutBoxType.Space || !charProps.underlineWordsOnly);
            var needStrikeout: boolean = !noNeedUnderlineAndStrikeout && (charProps.fontStrikeoutType != StrikeoutType.None) && (box.getType() != LayoutBoxType.Space || !charProps.strikeoutWordsOnly);
            if((needStrikeout && needUnderline) || (needStrikeout && strikeoutColor != ColorHelper.AUTOMATIC_COLOR) || (needUnderline && underlineColor != ColorHelper.AUTOMATIC_COLOR)) {
                boxStyles = boxStyles.concat(HtmlConverter.getSizeSignificantRules(charProps));
                if(foreColor != ColorHelper.AUTOMATIC_COLOR && strikeoutColor == ColorHelper.AUTOMATIC_COLOR && underlineColor == ColorHelper.AUTOMATIC_COLOR)
                    boxStyles.push("color: " + ColorHelper.getCssStringInternal(foreColor));

                var needColor: boolean = strikeoutColor != ColorHelper.AUTOMATIC_COLOR ||
                    underlineColor != ColorHelper.AUTOMATIC_COLOR;
                if(needColor && (!needStrikeout || strikeoutColor != ColorHelper.AUTOMATIC_COLOR))
                    content = '<span style="color: ' + ColorHelper.getCssString(foreColor, true) + ';">' + content + '</span>';
                if(needStrikeout) {
                    var strikeoutColorStyle = needColor ? ("color: " + ColorHelper.getCssString(strikeoutColor == ColorHelper.AUTOMATIC_COLOR ? foreColor : strikeoutColor, true)) : "";
                    content = '<span style="text-decoration: line-through;' + strikeoutColorStyle + '">' + content + '</span>';
                }
                if(needUnderline) {
                    var underlineColorStyle = needColor ? ("color: " + ColorHelper.getCssString(underlineColor == ColorHelper.AUTOMATIC_COLOR ? foreColor : underlineColor, true)) : "";
                    content = '<span style="text-decoration: underline;' + underlineColorStyle + '">' + content + '</span>';
                }
            }
            else
                boxStyles = boxStyles.concat(HtmlConverter.getCssRules(charProps, box.renderIsWordBox(), noNeedUnderlineAndStrikeout));

            var boxClass: string = "";
            switch(box.getType()) {
                case LayoutBoxType.Text:
                case LayoutBoxType.LayoutDependent:
                case LayoutBoxType.FieldCodeEnd:
                case LayoutBoxType.FieldCodeStart:
                case LayoutBoxType.ColumnBreak:
                case LayoutBoxType.LineBreak:
                case LayoutBoxType.PageBreak:
                case LayoutBoxType.ParagraphMark:
                case LayoutBoxType.SectionMark:
                    boxClass = DocumentRenderer.CLASSNAMES.BOX;
                    break;
                default:
                    boxClass = DocumentRenderer.CLASSNAMES.BOX_SPACE;
            }

            if(charProps.hidden)
                boxClass += ' ' + DocumentRenderer.CLASSNAMES.HIDDEN_BOX;

            var html: string = '<span style="' + boxStyles.join(";") + '" class="' + boxClass + '">' + content + '</span>';
            var backColor: number = charProps.backColor;

            if(box.fieldLevel && (backColor == ColorHelper.AUTOMATIC_COLOR || backColor == ColorHelper.NO_COLOR)) {
                var fieldBgClass = DocumentRenderer.CLASSNAMES.FIELD_BG;
                switch(box.fieldLevel) {
                    case 1: fieldBgClass += ' ' + DocumentRenderer.CLASSNAMES.FIELD_BOX_LEVEL1; break;
                    case 2: fieldBgClass += ' ' + DocumentRenderer.CLASSNAMES.FIELD_BOX_LEVEL2; break;
                    default: fieldBgClass += ' ' + DocumentRenderer.CLASSNAMES.FIELD_BOX_LEVEL3;
                }
                html += '<span class="' + fieldBgClass + '" style="top: ' + top + 'px; left: ' + left + 'px; width: ' + box.width + 'px; height: ' + box.height + 'px"></span>';
            }

            if(ColorHelper.getAlpha(backColor) > 0 && !noNeedUnderlineAndStrikeout) {
                var height: number = row.height - row.getSpacingAfter();
                var bgBoxStyle: string = "top: 0px; left: " + Math.floor(box.x) + "px; width: " + Math.ceil(box.width) + "px; height: " + height + "px; background: " +
                    ColorHelper.getCssStringInternal(backColor) + ";";
                html += '<span class="' + DocumentRenderer.CLASSNAMES.BOX_BG + '" style="' + bgBoxStyle + '"></span>';
            }

            if(box.hyperlinkTip)
                html = '<span title="' + HtmlConverter.buildHyperlinkTipString(box.hyperlinkTip) + '">' + html + "</span>";

            return html;
        }

        // the same code in LayoutPictureBox.renderGetContent
        renderPicture(box: LayoutPictureBox): string {
            var style = "height: " + box.height + "px; width: " + box.width + "px;";
            if(box.id == this.emptyImageCacheId) {
                var innerContent = "";
                style += "display: inline-block; box-sizing: border-box; -moz-box-sizing: border-box; -webkit-box-sizing: border-box; border: 1px dashed";
                if(box.isLoaded) {
                    style += " red; font: 10px Arial; color: red; text-align: center; line-height: " + box.height + "px;";
                    innerContent = "Error"; //TODO
                }
                else
                    style += " gray; background: url('" + constants.IMAGELOADINGURL + "') no-repeat center;";
                return "<span style=\"" + style + "\">" + innerContent + "</span>";
            }
            style += " vertical-align: baseline";
            return "<img src=\"" + this.handlerURI + "&img=" + box.id + "\" style=\"" + style + "\" class=\"" + DocumentRenderer.CLASSNAMES.PICTURE + "\" />";
        }

        hideDragCaret() {
            var element = this.getDragCaretElement();
            var parentNode: Node = element.parentNode;
            if(parentNode)
                parentNode.removeChild(element);
        }
        drawDragCaret(layoutDragCaret: LayoutDragCaret) {
            var element = this.getDragCaretElement();
            var pageElement = this.cache[layoutDragCaret.pageIndex].page;
            element.style.left = layoutDragCaret.x + "px";
            element.style.top = layoutDragCaret.y + "px";
            element.style.height = layoutDragCaret.height + "px";
            pageElement.appendChild(element);
        }
        private getDragCaretElement() {
            if(!this.dragCaretElement) {
                this.dragCaretElement = document.createElement("div");
                this.dragCaretElement.className = DocumentRenderer.CLASSNAMES.DRAG_CARET;
            }
            return this.dragCaretElement;
        }

        // Header Footer
        public activateOtherPageArea(pageIndex: number, pageArea: LayoutPageArea) {
            const pageCache: DocumentRendererPageCache = this.cache[pageIndex];
            const activePageAreaId: number = DocumentRenderer.getAreaCacheId(pageArea.subDocument.id, -1);

            ASPx.AddClassNameToElement(this.canvas, DocumentRenderer.CLASSNAMES.BACKLIGHT_MODE);
            ASPx.AddClassNameToElement(pageCache.page, DocumentRenderer.CLASSNAMES.ACTIVE);
            for(let pageAreaId in pageCache.areas) {
                if (!pageCache.areas.hasOwnProperty(pageAreaId) || pageAreaId != activePageAreaId)
                    continue;
                const pageAreaElement: HTMLElement = pageCache.areas[pageAreaId];
                ASPx.AddClassNameToElement(pageAreaElement, DocumentRenderer.CLASSNAMES.ACTIVE);
                if(pageArea.subDocument.isHeaderFooter()) {
                    const pageBackColor: number = pageArea.subDocument.documentModel.pageBackColor;
                    pageAreaElement.style.backgroundColor = ColorHelper.colorToHash(ColorHelper.isEmptyBgColor(pageBackColor) ? ColorHelper.LIGHT_COLOR : pageBackColor);
                }
            }
        }
        deactivateOtherPageArea(pageIndex: number, pageArea: LayoutPageArea) {
            ASPx.RemoveClassNameFromElement(this.canvas, DocumentRenderer.CLASSNAMES.BACKLIGHT_MODE);
            let pageCache = this.cache[pageIndex];
            ASPx.RemoveClassNameFromElement(pageCache.page, DocumentRenderer.CLASSNAMES.ACTIVE);
            for(let pageAreaId in pageCache.areas) {
                if(!pageCache.areas.hasOwnProperty(pageAreaId)) continue;
                let pageAreaElement = pageCache.areas[pageAreaId];
                if(pageAreaElement.className.indexOf(DocumentRenderer.CLASSNAMES.ACTIVE) >= 0) {
                    ASPx.RemoveClassNameFromElement(pageAreaElement, DocumentRenderer.CLASSNAMES.ACTIVE);
                    if(pageArea.subDocument.isHeaderFooter())
                        pageAreaElement.style.backgroundColor = "";
                }
            }
        }
        showHeaderFooterInfo(pageIndex: number, pageArea: LayoutPageArea, infoText: string, addInfoText: string) {
            var infoElement = pageArea.subDocument.isHeader() ? this.getHeaderInfoElement() : this.getFooterInfoElement();
            this.prepareHeaderFooterInfoElement(pageIndex, pageArea, infoElement, infoText, addInfoText);
        }
        hideHeaderFooterInfo(pageArea: LayoutPageArea) {
            var infoElement = pageArea.subDocument.isHeader() ? this.getHeaderInfoElement() : this.getFooterInfoElement();
            infoElement.parentNode.removeChild(infoElement);
        }

        headerInfoElement: HTMLElement;
        footerInfoElement: HTMLElement;
        private getHeaderInfoElement(): HTMLElement {
            if(!this.headerInfoElement) {
                this.headerInfoElement = document.createElement("div");
                this.headerInfoElement.className = DocumentRenderer.CLASSNAMES.HEADER_INFO;
            }
            return this.headerInfoElement;
        }
        private getFooterInfoElement(): HTMLElement {
            if(!this.footerInfoElement) {
                this.footerInfoElement = document.createElement("div");
                this.footerInfoElement.className = DocumentRenderer.CLASSNAMES.FOOTER_INFO;
            }
            return this.footerInfoElement;
        }
        private prepareHeaderFooterInfoElement(pageIndex: number, pageArea: LayoutPageArea, infoElement: HTMLElement, infoText: string, addInfoText) {
            let infoHtml = "<b>" + infoText + "</b>";
            if(addInfoText)
                infoHtml += "<b>" + addInfoText + "</b>";

            infoElement.innerHTML = infoHtml;
            this.cache[pageIndex].page.appendChild(infoElement);
            if(pageArea.subDocument.isHeader())
                infoElement.style.top = (pageArea.y + pageArea.height + 1) + "px";
            else
                infoElement.style.top = (pageArea.y - infoElement.offsetHeight - 1) + "px";
        }

        close() {
            this.canvas.innerHTML = "";
            this.cache = [];
        }
    }
}