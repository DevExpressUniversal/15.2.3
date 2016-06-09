module __aspxRichEdit {
    const CURSOR_NO_BLINK_CLASS_NAME = "dxre-sel-noblink";
    const BAR_CLASS_NAME = "dxre-sel-bar";

    const CIRCLE_TOP_OFFSET = 2;
    const CHANGE_BLINK_CURSOR_DELAY = 350;

    export class SelectionRenderer {
        private lastTimeMoveCursor: number; // cursor move commands update this
        private processIdChangeCursorToBlink;

        private selectionCache: CanvasSelectionCache;

        private startBarElement: BarElement;
        private endBarElement: BarElement;
        private startCircleHash: string = "";
        private endCircleHash: string = "";

        documentRenderer: DocumentRenderer;

        constructor(documentRenderer: DocumentRenderer, selectionCache: CanvasSelectionCache) {
            this.documentRenderer = documentRenderer;
            this.processIdChangeCursorToBlink = null;
            this.updateLastTimeMoveCursor();
            this.selectionCache = selectionCache;
        }

        updateLastTimeMoveCursor(): void {
            this.lastTimeMoveCursor = (new Date()).getTime();
        }

        /* TouchUI */
        removeTouchBars(): void {
            if (this.startBarElement)
                this.startBarElement.setParent(null);
            if (this.endBarElement)
                this.endBarElement.setParent(null);
            this.startCircleHash = "";
            this.endCircleHash = "";
        }

        renderTouchBars(firstRow: LayoutSelectionItem, lastRow: LayoutSelectionItem, needRenderStartBar: boolean, needRenderEndBar: boolean) {
            if(needRenderStartBar) {
                this.startBarElement = this.startBarElement || new BarElement(true);
                this.startBarElement.setParent(this.documentRenderer.getSelectionContainer(firstRow.subDocumentID, firstRow.pageIndex, firstRow.pageAreaIndex, firstRow.columnIndex));

                if(!(firstRow instanceof LayoutSelectionCursor)) {
                    this.startBarElement.setTop(firstRow.y + firstRow.height);
                    this.startBarElement.setLeft(firstRow.x);
                }
            }

            if(needRenderEndBar) {
                this.endBarElement = this.endBarElement || new BarElement(false);
                this.endBarElement.setParent(this.documentRenderer.getSelectionContainer(lastRow.subDocumentID, lastRow.pageIndex, lastRow.pageAreaIndex, lastRow.columnIndex));

                this.endBarElement.setTop(lastRow.y + lastRow.height);
                this.endBarElement.setLeft(lastRow.x + lastRow.width);

                if(lastRow instanceof LayoutSelectionCursor) {
                    this.startBarElement.setTop(lastRow.y + lastRow.height);
                    this.startBarElement.setLeft(lastRow.x + lastRow.width);
                }
            }

            this.startBarElement.setVisible(needRenderStartBar);
            this.endBarElement.setVisible(needRenderEndBar);
        }

        setVisibilityTouchBars(visible: boolean): void {
            if (this.startBarElement)
                this.startBarElement.setVisible(visible)
            if (this.endBarElement)
                this.endBarElement.setVisible(visible)
        }
        removeItemsFromCacheByPageArea(pageIndex: number, subDocumentId: number, pageAreaIndex: number) {
            this.selectionCache.removeItemsByPageArea(pageIndex, subDocumentId, pageAreaIndex);
        }
        removeItemsFromCacheByColumn(pageIndex: number, subDocumentId: number, pageAreaIndex: number, columnIndex: number) {
            this.selectionCache.removeItemsByColumn(pageIndex, subDocumentId, pageAreaIndex, columnIndex);
        }

        removeItems(pageIndex: number, items: LayoutSelectionItem[]) {
            this.selectionCache.removeItemsByExpression(pageIndex, (item => {
                if(Utils.indexOf(items, item) > -1) {
                    let element = this.selectionCache.elements[item.getHash()];
                    if(element)
                        element.parentElement.removeChild(element);
                    return true;
                }
                return false;
            }));
        }
        removeItemsByPage(pageIndex: number, removeElements: boolean) {
            const itemsOnPage: LayoutSelectionItem[] = this.selectionCache.items[pageIndex];
            if(!itemsOnPage)
                return;
            if(removeElements) {
                for (let item of itemsOnPage) {
                    const element: HTMLElement = this.selectionCache.elements[item.getHash()];
                    if(element)
                        element.parentNode.removeChild(element);
                }
            }
            this.selectionCache.removeItemsByPageIndex(pageIndex);
        }

        renderItem(item: LayoutSelectionItem) {
            let hash = item.getHash();
            let selectionRowElement = document.createElement("div");
            selectionRowElement.className = DocumentRenderer.CLASSNAMES.SELECTION_ROW;
            this.applyStyleToSelectionItemElement(item, selectionRowElement);
            this.selectionCache.elements[hash] = selectionRowElement;
            let selectionContainer = this.documentRenderer.getSelectionContainer(item.subDocumentID, item.pageIndex, item.pageAreaIndex, item.columnIndex);
            selectionContainer.appendChild(selectionRowElement);
        }

        replaceOrAddItem(newItem: LayoutSelectionItem, oldItem: LayoutSelectionItem) {
            if(oldItem) {
                let oldItemHash = oldItem.getHash();
                let oldItemElement = this.selectionCache.elements[oldItemHash];
                if(oldItem.columnIndex !== newItem.columnIndex || oldItem.pageAreaIndex !== newItem.pageAreaIndex || oldItem.subDocumentID !== newItem.subDocumentID)
                    this.documentRenderer.getSelectionContainer(newItem.subDocumentID, newItem.pageIndex, newItem.pageAreaIndex, newItem.columnIndex).appendChild(oldItemElement);
                delete this.selectionCache.elements[oldItemHash];
                this.applyStyleToSelectionItemElement(newItem, oldItemElement);
                this.selectionCache.elements[newItem.getHash()] = oldItemElement;
            }
            else
                this.renderItem(newItem);
        }

        private applyStyleToSelectionItemElement(item: LayoutSelectionItem, element: HTMLElement) {
            element.style.width = item.width + "px";
            element.style.height = item.height + "px";
            element.style.top = item.y + "px";
            element.style.left = item.x + "px";

            if(item instanceof LayoutSelectionCursor)
                element.className = [DocumentRenderer.CLASSNAMES.SELECTION_ROW, CURSOR_NO_BLINK_CLASS_NAME, DocumentRenderer.CLASSNAMES.SELECTION_CURSOR].join(" ");
            else if(element.className !== DocumentRenderer.CLASSNAMES.SELECTION_ROW)
                element.className = DocumentRenderer.CLASSNAMES.SELECTION_ROW;
            if(this.processIdChangeCursorToBlink) {
                clearTimeout(this.processIdChangeCursorToBlink);
                this.processIdChangeCursorToBlink = null;
            }
            if(item instanceof LayoutSelectionCursor) {
                this.processIdChangeCursorToBlink = setTimeout(() => {
                    element.className = DocumentRenderer.CLASSNAMES.SELECTION_ROW + " " + DocumentRenderer.CLASSNAMES.SELECTION_CURSOR;
                }, CHANGE_BLINK_CURSOR_DELAY);
            }
        }
    }

    export class BarElement {
        private element: HTMLElement = null;
        private isLeftBar: boolean = false;
        private radius: number = 0;
        private visible: boolean = true;

        constructor(isStartEdge: boolean) {
            this.isLeftBar = isStartEdge;
        }

        private create(parent: Node): void {
            this.element = document.createElement("DIV");
            parent.appendChild(this.element);
        }
        private prepare(): void {
            this.element.className = BAR_CLASS_NAME;
            this.radius = this.element.offsetWidth / 2
        }
        private changeParent(parent: Node) {
            if (this.element.parentNode)
                this.element.parentNode.removeChild(this.element);
            if (parent)
                parent.appendChild(this.element);
        }

        setTop(value: number): void {
            this.element.style.top = value + "px";
        }
        setLeft(value: number): void {
            this.element.style.left = value - this.radius + "px";
        }
        setParent(parent: Node = null): void {
            if (this.element)
                this.changeParent(parent);
            else {
                this.create(parent);
                this.prepare();
            }
        }
        setVisible(visible: boolean): void {
            if (this.visible != visible) {
                this.visible = visible;
                this.element.style.display = this.visible ? "" : "none";
            }
        }
    }
}