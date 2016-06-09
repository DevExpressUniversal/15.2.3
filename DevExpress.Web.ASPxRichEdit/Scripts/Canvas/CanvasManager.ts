module __aspxRichEdit {
    const VISIBLE_PAGES_RANGE = 2;
    const VISIBLE_PAGES_MAXCOUNT = 16;
    const VISIBLE_PAGES_RANGE_TOUCH = 0;
    const VISIBLE_PAGES_MAXCOUNT_TOUCH = 1;
    const HEADERFOOTER_SCROLLOFFSET = 5;
    const SCROLL_INTERVAL_MS = 50;
    const CSSCLASS_FOCUSED = "dxre-inFocus";
    const CSSCLASS_HEADERFOOTERINFOPANEL = "dxre-headerFooter-info";
    const AUTOSCROLL_AREA_SIZE = 10;
    const AUTOSCROLL_STEP = 10;
    const MSTOUCH_MOVE_SENSITIVITY = 5;

    export class CanvasManager extends BatchUpdatableObject
        implements ILayoutChangesListener, ISelectionLayoutChangesListener, IDragCaretVisualizerChangesListener, IResizeBoxVisualizerChangesListener {
        public canvas: HTMLDivElement;
        public layout: DocumentLayout;
        private formatterController: IFormatterController;
        private eventManager: IEventManager;
        private resizeBox: CanvasResizeBoxHelper;
        private scrollMeasurer: HTMLDivElement;
        private scrollIntervalID: number = 0;
        private lastMousePosition = { x: -1, y: -1 };
        private canvasPosition = { x: -1, y: -1 };

        private pointer: CursorPointer = CursorPointer.Auto;
        private blockNotPointerEvents: boolean = false;
        private lastPointerPosition = { x: -1, y: -1 };
        private lastActivePageIndex: number;

        private selectionCache: CanvasSelectionCache = new CanvasSelectionCache();
        
        public renderer: DocumentRenderer;
        private scroll: CanvasScrollInfo;

        private visiblePages: LayoutPage[] = [];
        private firstSelectionItem: LayoutSelectionItem;
        private lastSelectionItem: LayoutSelectionItem;
        public sizes: CanvasSizeInfo = new CanvasSizeInfo(); // do it private
        private updateScrollTimeoutId = -1;
        private waitForDblClickTimeoutId = -1;
        private horizontalRuler: IRulerControl;

        private activeSubDocumentInfo: SubDocumentInfoBase;
        private stringResources: StringResources;

        constructor(layout: DocumentLayout, canvas: HTMLDivElement, formatterController: IFormatterController, eventManager: IEventManager, stringResources: StringResources, horizontalRuler: IRulerControl) {
            super();
            this.layout = layout;
            this.stringResources = stringResources;
            this.canvas = canvas;
            this.formatterController = formatterController;
            this.eventManager = eventManager;
            this.renderer = new DocumentRenderer(canvas, this.selectionCache);
            this.initCommonEvents();
            this.horizontalRuler = horizontalRuler;
            this.scroll = new CanvasScrollInfo(canvas, this.sizes);
            if(!ASPx.Browser.WebKitTouchUI)
                this.initMouseEvents();
            if(ASPx.Browser.TouchUI)
                this.initTouchEvents();
            if(ASPx.Browser.MSTouchUI)
                if(ASPx.Browser.MajorVersion > 10)
                    this.initPointerEvents();
                else
                    this.initMSPointerEvents();
            this.resizeBox = new CanvasResizeBoxHelper(this.renderer);
        }
        public adjust(force: boolean) {
            if (force || this.sizes.topSpacing < 0) {
                const pageCache: DocumentRendererPageCache = this.renderer.cache[0];
                if(!pageCache)
                    return;
                const pageElementStyle: CSSStyleDeclaration = ASPx.GetCurrentStyle(pageCache.page);
                this.sizes.topSpacing = ASPx.PxToInt(pageElementStyle.borderTopWidth) + ASPx.PxToInt(pageElementStyle.marginTop);
                this.sizes.betweenPageSpacing = ASPx.PxToInt(pageElementStyle.borderTopWidth) + ASPx.PxToInt(pageElementStyle.borderBottomWidth) + Math.max(ASPx.PxToInt(pageElementStyle.marginTop), ASPx.PxToInt(pageElementStyle.marginBottom));
                this.sizes.setVisibleAreaSize(this.canvas.offsetWidth, this.canvas.offsetHeight);
                this.updateScrollVisibility();
                this.onCanvasScroll();
            }
        }
        setCursorPointer(pointer: CursorPointer) {
            if(this.pointer === pointer)
                return;
            if (this.pointer !== CursorPointer.Auto)
                ASPx.RemoveClassNameFromElement(this.canvas, CanvasManager.getCursorClassName(this.pointer));
            const newClassName: string = CanvasManager.getCursorClassName(pointer);
            if(newClassName)
                ASPx.AddClassNameToElement(this.canvas, newClassName);
            this.pointer = pointer;
        }
        setParameters(handlerUri: string, emptyImageCacheId: number) {
            this.renderer.handlerURI = handlerUri;
            this.renderer.emptyImageCacheId = emptyImageCacheId;
        }
        getContextMenuAbsPoint(): Point {
            if(!this.firstSelectionItem)
                return;
            let firstSelectionItem = this.firstSelectionItem;
            const layoutPage: LayoutPage = this.layout.pages[this.firstSelectionItem.pageIndex];
            const pageArea: LayoutPageArea = layoutPage.mainSubDocumentPageAreas[firstSelectionItem.pageAreaIndex];
            const pageColumn: LayoutColumn = pageArea.columns[firstSelectionItem.columnIndex];
            const x: number = pageArea.x + pageColumn.x + firstSelectionItem.x;
            const y: number = pageArea.y + pageColumn.y + firstSelectionItem.y + firstSelectionItem.height;
            if(this.isVisiblePosition(firstSelectionItem.pageIndex, x, y))
                return new Point(ASPx.GetAbsolutePositionX(this.renderer.cache[firstSelectionItem.pageIndex].page) + x, ASPx.GetAbsolutePositionY(this.renderer.cache[firstSelectionItem.pageIndex].page) + y);
            return new Point(ASPx.GetAbsolutePositionX(this.canvas), ASPx.GetAbsolutePositionY(this.canvas));
        }
        private static getCursorClassName(pointer: CursorPointer): string {
            switch(pointer) {
                case CursorPointer.Copy:
                    return "dxre-cursor-copy";
                case CursorPointer.NoDrop:
                    return "dxre-cursor-nodrop";
                case CursorPointer.EResize:
                    return "dxre-cursor-eresize";
                case CursorPointer.NResize:
                    return "dxre-cursor-nresize";
                case CursorPointer.SResize:
                    return "dxre-cursor-sresize";
                case CursorPointer.WResize:
                    return "dxre-cursor-wresize";
                case CursorPointer.SEResize:
                    return "dxre-cursor-seresize";
                case CursorPointer.SWResize:
                    return "dxre-cursor-swresize";
                case CursorPointer.NWResize:
                    return "dxre-cursor-nwresize";
                case CursorPointer.NEResize:
                    return "dxre-cursor-eeresize";
                case CursorPointer.Move:
                case CursorPointer.Default:
                    return "dxre-cursor-default";
            }
        }
        closeDocument() {
            this.scroll = new CanvasScrollInfo(this.canvas, this.sizes);
            this.renderer.close();
            this.visiblePages = [];
            this.selectionCache.close();
        }
        waitForDblClick() {
            this.waitForDblClickTimeoutId = setTimeout(() => {
                this.waitForDblClickTimeoutId = -1;
            }, MouseHandler.WAIT_FOR_DBLCLICK_INTERVAL);
        }

        // events processing
        onCanvasScroll() {
            this.updateVisiblePages();
        }
        private onCanvasMouseWheel(evt: WheelEvent) {
            const point: LayoutPoint = this.getLayoutPoint(evt, false);
            point.pageY += evt.deltaY;

            this.eventManager.onMouseMove(new RichMouseEvent(evt, point, this.getMouseEventSource(ASPx.Evt.GetEventSource(evt)), this.scroll.lastScrollTop, this.scroll.lastScrollLeft)); // maybe here false
            this.updateVisiblePages();
        }
        private onCanvasMouseDown(evt: MouseEvent): boolean {
            if(!this.blockNotPointerEvents)
                this.onCanvasMouseDownInternal(evt);
            return ASPx.Evt.PreventEventAndBubble(evt);
        }
        private onCanvasMouseDownInternal(evt: MouseEvent) {
            const point: LayoutPoint = this.getLayoutPoint(evt, true);
            this.eventManager.onMouseDown(new RichMouseEvent(evt, point, this.getMouseEventSource(ASPx.Evt.GetEventSource(evt)), this.scroll.lastScrollTop, this.scroll.lastScrollLeft));
            this.saveMousePosition(evt);
            this.resetScrollInterval();
            this.canvasPosition.x = ASPx.GetAbsolutePositionX(this.canvas);
            this.canvasPosition.y = ASPx.GetAbsolutePositionY(this.canvas);
            if(!point.isEmpty()) {
                this.scrollIntervalID = setInterval(() => {
                    this.onScrollIntervalTick();
                }, SCROLL_INTERVAL_MS);
            }
        }
        private onCanvasMouseUp(evt: MouseEvent) {
            if(!this.blockNotPointerEvents)
                this.onCanvasMouseUpInternal(evt);
        }
        private onCanvasMouseUpInternal(evt: MouseEvent) {
            this.eventManager.onMouseUp(new RichMouseEvent(evt, this.getLayoutPoint(evt, false), this.getMouseEventSource(ASPx.Evt.GetEventSource(evt)), this.scroll.lastScrollTop, this.scroll.lastScrollLeft));
            this.resetScrollInterval();
        }
        private onCanvasMouseMove(evt: MouseEvent) {
            if(!this.blockNotPointerEvents)
                this.onCanvasMouseMoveInternal(evt);
        }
        private onCanvasMouseMoveInternal(evt: MouseEvent) {
            this.eventManager.onMouseMove(new RichMouseEvent(evt, this.getLayoutPoint(evt, false), this.getMouseEventSource(ASPx.Evt.GetEventSource(evt)), this.scroll.lastScrollTop, this.scroll.lastScrollLeft));
        }
        private onCanvasMouseDblClick(evt: MouseEvent) {
            this.eventManager.onMouseDblClick(new RichMouseEvent(evt, this.getLayoutPoint(evt, true), this.getMouseEventSource(ASPx.Evt.GetEventSource(evt)), this.scroll.lastScrollTop, this.scroll.lastScrollLeft));
            return ASPx.Evt.PreventEventAndBubble(evt);
        }
        private onCanvasTouchStart(evt: MouseEvent) {
            if(!this.blockNotPointerEvents)
                this.onCanvasTouchStartInternal(evt);
            return true;
        }
        private onCanvasTouchStartInternal(evt: MouseEvent) {
            this.saveMousePosition(evt);
            this.eventManager.onTouchStart(new RichMouseEvent(evt, this.getLayoutPoint(evt, true), this.getMouseEventSource(ASPx.Evt.GetEventSource(evt)), this.scroll.lastScrollTop, this.scroll.lastScrollLeft));
        }
        private onCanvasTouchEnd(evt: MouseEvent): boolean {
            if(!this.blockNotPointerEvents)
                this.onCanvasTouchEndInternal(evt);
            return ASPx.Evt.PreventEventAndBubble(evt);
        }
        private onCanvasTouchEndInternal(evt: MouseEvent): boolean {
            return this.eventManager.onTouchEnd(new RichMouseEvent(evt, this.getLayoutPoint(evt, false), this.getMouseEventSource(ASPx.Evt.GetEventSource(evt)), this.scroll.lastScrollTop, this.scroll.lastScrollLeft));
        }
        private onCanvasTouchMove(evt: MouseEvent): boolean {
            if(!this.blockNotPointerEvents)
                return this.onCanvasTouchMoveInternal(evt);
            return true;
        }
        private onCanvasTouchMoveInternal(evt: MouseEvent): boolean {
            if(!this.eventManager.onTouchMove(new RichMouseEvent(evt, this.getLayoutPoint(evt, false), this.getMouseEventSource(ASPx.Evt.GetEventSource(evt)), this.scroll.lastScrollTop, this.scroll.lastScrollLeft)))
                return ASPx.Evt.PreventEventAndBubble(evt);
            return true;
        }
        private onCanvasPointerDown(evt: PointerEvent) {
            if(evt.pointerType == "mouse")
                this.onCanvasMouseDownInternal(evt);
            else if(evt.pointerType == "touch")
                this.onCanvasTouchStartInternal(evt);
            this.blockNotPointerEvents = true;

            this.lastPointerPosition.x = evt.x;
            this.lastPointerPosition.y = evt.y;
        }
        private onCanvasPointerMove(evt: PointerEvent): boolean {
            if(evt.x - this.lastPointerPosition.x > MSTOUCH_MOVE_SENSITIVITY || evt.y - this.lastPointerPosition.y > MSTOUCH_MOVE_SENSITIVITY) { //T254211
                if(evt.pointerType == "mouse")
                    this.onCanvasMouseMoveInternal(evt);
                else if(evt.pointerType == "touch")
                    return this.onCanvasTouchMoveInternal(evt);
                return ASPx.Evt.PreventEventAndBubble(evt);
            }
        }
        private onCanvasPointerUp(evt: PointerEvent): boolean {
            if(evt.pointerType == "mouse")
                this.onCanvasMouseUpInternal(evt);
            else if(evt.pointerType == "touch")
                this.onCanvasTouchEndInternal(evt);
            setTimeout(() => { this.blockNotPointerEvents = false; }, 0);
            return ASPx.Evt.PreventEventAndBubble(evt);
        }
        private onCanvasGestureStart(evt: MouseEvent) {
            this.eventManager.onGestureStart(evt);
        }
        private onDocumentMouseUp(evt: MouseEvent) {
            if(ASPx.GetIsParent(this.canvas, ASPx.Evt.GetEventSource(evt))) {
                if(!ASPx.Evt.IsLeftButtonPressed(evt))
                    ASPx.PopupUtils.PreventContextMenu(evt);
                this.onCanvasMouseUp(evt);
            }
            else {
                this.eventManager.onMouseUp(new RichMouseEvent(evt, null, MouseEventSource.Undefined, this.scroll.lastScrollTop, this.scroll.lastScrollLeft));
                this.resetScrollInterval();
            }
        }
        private onDocumentContextMenu(evt: MouseEvent) {
            if(ASPx.GetIsParent(this.canvas.parentNode, ASPx.Evt.GetEventSource(evt))) {
                ASPx.PopupUtils.PreventContextMenu(evt);
                return ASPx.Evt.CancelBubble(evt);
            }
        }
        private onDocumentMouseMove(evt: MouseEvent) {
            this.saveMousePosition(evt);
        }
        private onDocumentTouchEnd(evt: MouseEvent) {
            if(ASPx.GetIsParent(this.canvas, ASPx.Evt.GetEventSource(evt)))
                return;
            this.eventManager.onTouchEnd(new RichMouseEvent(evt, null, MouseEventSource.Undefined, this.scroll.lastScrollTop, this.scroll.lastScrollLeft));
            this.resetScrollInterval();
        }
        private onDocumentTouchMove(evt: MouseEvent) {
            this.saveMousePosition(evt);
        }

        private getMouseEventSource(source: HTMLElement): MouseEventSource {
            return CanvasResizeBoxHelper.getMouseEventSource(source.nodeType === 1 ? source : <HTMLElement>source.parentNode);
        }
        
        private updateScrollVisibility() {
            //Logging.print(LogSource.CanvasManager, "updateScrollVisibility.");
            if(!this.scrollMeasurer) {
                this.scrollMeasurer = document.createElement("div");
                this.scrollMeasurer.style.position = "absolute";
                this.scrollMeasurer.style.top = "0";
                this.scrollMeasurer.style.bottom = "0";
                this.scrollMeasurer.style.right = "0";
                this.scrollMeasurer.style.left = "0";
            }
            this.canvas.appendChild(this.scrollMeasurer);
            const prevScrollYVisibility: boolean = this.sizes.scrollYVisible;
            this.sizes.updateScrollVisibility(this.scrollMeasurer.offsetWidth, this.scrollMeasurer.offsetHeight);
            this.canvas.removeChild(this.scrollMeasurer);
            if(prevScrollYVisibility !== this.sizes.scrollYVisible)
                this.adjustRuler();
        }
        private adjustRuler() {
            if(this.horizontalRuler)
                this.horizontalRuler.adjust();
        }
        private getLayoutPoint(evt, checkScroll: boolean) {
            if (!this.layout)
                return LayoutPoint.Empty();

            const clientX: number = ASPx.Evt.GetEventX(evt);
            const clientY: number = ASPx.Evt.GetEventY(evt);

            const canvasY: number = ASPx.GetAbsolutePositionY(this.canvas);
            const canvasX: number = ASPx.GetAbsolutePositionX(this.canvas);

            const offsetY: number = clientY - canvasY + this.canvas.scrollTop;
            const pageIndex: number = this.sizes.findPageIndexByOffsetY(this.layout.pages, offsetY);

            if(checkScroll) {
                if(this.sizes.scrollYVisible && canvasX + this.sizes.getVisibleAreaWidth(false) - clientX < 0)
                    return LayoutPoint.Empty();
                if(this.sizes.scrollXVisible && canvasY + this.sizes.getVisibleAreaHeight(false) - clientY < 0)
                    return LayoutPoint.Empty();
            }
            const layoutPage: LayoutPage = this.layout.pages[pageIndex];
            const renderPageCacheElem: DocumentRendererPageCache = this.renderer.cache[pageIndex];
            if (!layoutPage || !renderPageCacheElem)
                return LayoutPoint.Empty();
            return new LayoutPoint(pageIndex,
                clientX - canvasX + this.canvas.scrollLeft - renderPageCacheElem.page.offsetLeft,
                offsetY - this.sizes.getPageOffsetY(layoutPage));
        }

        // ILayoutChangesListener
        NotifyPagesInvalidated(firstInvalidPageIndex: number) {
            //Logging.print(LogSource.CanvasManager, "NotifyPagesInvalidated.");
            this.formatterController.runFormatting(firstInvalidPageIndex);
        }
        NotifyPageReady(pageChange: PageChange) {
            switch (pageChange.changeType) {
                case LayoutChangeType.Added: this.onPageAdded(pageChange.layoutElement); break;
                case LayoutChangeType.Deleted: this.onPageDeleted(pageChange.index); break;
                case LayoutChangeType.Updated: this.onPageUpdated(pageChange); break;
                case LayoutChangeType.Replaced: this.onPageReplaced(pageChange.layoutElement); break;
                default: throw new Error(Errors.NotImplemented);
            }
        }
        private onPageAdded(layoutPage: LayoutPage) {
            layoutPage.isNeedFullyRender = false;
            this.renderer.insertPage(layoutPage, this.layout.pageColor);
            this.adjust(false);
            this.updateVisiblePages();
            this.restoreSelection(layoutPage.index);
            this.updateScrollVisibility();
        }
        private onPageDeleted(index: number) {
            this.renderer.removePage(index);

            this.selectionCache.removeItemsByPageIndex(index);
            const lastPageIndex: number = this.selectionCache.findLastPageIndex();
            for (let canvasPageIndex: number = index; canvasPageIndex < lastPageIndex; canvasPageIndex++)
                this.selectionCache.moveItemsToPage(canvasPageIndex + 1, canvasPageIndex);
            
            this.updateVisiblePages();
            this.updateScrollVisibility();
        }
        private onPageUpdated(pageChange: PageChange) {
            const layoutPage: LayoutPage = pageChange.layoutElement;
            if (layoutPage.isNeedFullyRender)
                this.renderer.applyPageChanges(pageChange, this.layout.isShowTableGridLines);
            else
                this.updateVisiblePages();

            if (this.activeSubDocumentInfo && this.activeSubDocumentInfo.isHeaderFooter && this.lastActivePageIndex === layoutPage.index)
                this.renderHeaderFooter(layoutPage, this.activeSubDocumentInfo.subDocumentId);
        }
        private onPageReplaced(layoutPage: LayoutPage) {
            layoutPage.isNeedFullyRender = false;
            this.renderer.renderPage(layoutPage, false, this.layout.pageColor, this.layout.isShowTableGridLines);
            this.updateVisiblePages();

            this.restoreSelection(layoutPage.index);
            if (this.activeSubDocumentInfo && this.activeSubDocumentInfo.isHeaderFooter && this.lastActivePageIndex === layoutPage.index)
                this.renderHeaderFooter(layoutPage, this.activeSubDocumentInfo.subDocumentId);

            this.updateScrollVisibility();
        }
        
        private restoreSelection(pageIndex: number) {
            const selectionItems: LayoutSelectionItem[] = this.selectionCache.items[pageIndex];
            if (!selectionItems)
                return;
            for (let item of selectionItems)
                this.renderer.selection.renderItem(item);
            if (ASPx.Browser.TouchUI) {
                this.renderer.selection.renderTouchBars(this.firstSelectionItem, this.lastSelectionItem,
                    this.firstSelectionItem && this.firstSelectionItem.pageIndex === pageIndex,
                    this.lastSelectionItem && this.lastSelectionItem.pageIndex === pageIndex);
            }
        }

        // ISelectionLayoutChangesListener
        NotifySelectionLayoutChanged(layoutSelection: LayoutSelection) {
            if (this.activeSubDocumentInfo !== layoutSelection.subDocumentInfo ||
                    (!layoutSelection.subDocumentInfo.isMain && this.lastActivePageIndex !== layoutSelection.pageIndex))
                this.switchActiveSubDocumentInfo(layoutSelection);
            else if(layoutSelection.subDocumentInfo.isHeaderFooter) {
                const pageIndex: number = layoutSelection.pageIndex;
                const isHeader: boolean = layoutSelection.subDocumentInfo.isHeader;
                const expectedScrollTop: number = this.sizes.getPageOffsetY(this.layout.pages[pageIndex]) +
                    (isHeader ? -HEADERFOOTER_SCROLLOFFSET : this.layout.pages[pageIndex].height - this.sizes.getVisibleAreaHeight(false) + HEADERFOOTER_SCROLLOFFSET);
                if(this.canvas.scrollTop !== expectedScrollTop)
                    this.scrollToHeaderFooter(isHeader, pageIndex);
            }

            this.firstSelectionItem = layoutSelection.firstItem;
            this.lastSelectionItem = layoutSelection.lastItem;
            this.lastActivePageIndex = layoutSelection.pageIndex;

            this.selectionCache.forEachPageIndex(pi => {
                if(!layoutSelection.pages[pi])
                    this.renderer.selection.removeItemsByPage(pi, true);
            });
            for (let key in layoutSelection.pages) {
                const pageIndex: number = parseInt(key);
                this.updatePageSelection(pageIndex, layoutSelection.pages[pageIndex]);
            }
            this.updateTouchBarsIfRequired(layoutSelection);
            this.updateScrollPosition(layoutSelection.scrollPosition);
        }
        NotifyPageSelectionLayoutChanged(pageIndex: number, layoutSelection: LayoutSelection) {
            this.updateTouchBarsIfRequired(layoutSelection);
            if(!layoutSelection || !layoutSelection.pages[pageIndex])
                this.renderer.selection.removeItemsByPage(pageIndex, true);
            else {
                if(this.activeSubDocumentInfo !== layoutSelection.subDocumentInfo ||
                    (!layoutSelection.subDocumentInfo.isMain && this.lastActivePageIndex !== layoutSelection.pageIndex))
                    this.switchActiveSubDocumentInfo(layoutSelection);
                else if(layoutSelection.subDocumentInfo.isHeaderFooter) {
                    const pageIndex: number = layoutSelection.pageIndex;
                    const isHeader: boolean = layoutSelection.subDocumentInfo.isHeader;
                    const expectedScrollTop: number = this.sizes.getPageOffsetY(this.layout.pages[pageIndex]) +
                        (isHeader ? -HEADERFOOTER_SCROLLOFFSET : this.layout.pages[pageIndex].height - this.sizes.getVisibleAreaHeight(false) + HEADERFOOTER_SCROLLOFFSET);
                    if (this.canvas.scrollTop !== expectedScrollTop)
                        this.scrollToHeaderFooter(isHeader, pageIndex);
                }

                this.firstSelectionItem = layoutSelection.firstItem || this.firstSelectionItem;
                this.lastSelectionItem = layoutSelection.lastItem || this.lastSelectionItem;
                this.updatePageSelection(pageIndex, layoutSelection.pages[pageIndex]);
                this.updateScrollPosition(layoutSelection.scrollPosition);
            }
        }
        NotifyFocusChanged(inFocus) {
            if(inFocus)
                ASPx.AddClassNameToElement(this.canvas, CSSCLASS_FOCUSED);
            else
                ASPx.RemoveClassNameFromElement(this.canvas, CSSCLASS_FOCUSED);
        }

        // IResizeBoxVisualizerChangesListener
        NotifyResizeBoxShown(pageIndex: number, layoutItem: LayoutItemBase, hyperlinkTip: string) {
            this.resizeBox.show(pageIndex, layoutItem, hyperlinkTip);
        }
        NotifyResizeBoxHidden() {
            this.resizeBox.hide();
        }

        // IDragCaretVisualizerChangesListener
        NotifyDragCaretShown(layoutDragCaret: LayoutDragCaret) {
            this.renderer.drawDragCaret(layoutDragCaret);
        }
        NotifyDragCaretHidden() {
            this.renderer.hideDragCaret();
        }
        switchActiveSubDocumentInfo(layoutSelection: LayoutSelection) {
            if(layoutSelection.subDocumentInfo.isMain && this.activeSubDocumentInfo && this.activeSubDocumentInfo.isHeaderFooter) {
                this.setScrollEnabled(true);
                let pageArea = this.layout.pages[this.lastActivePageIndex].otherPageAreas[this.activeSubDocumentInfo.subDocumentId];
                this.renderer.deactivateOtherPageArea(this.lastActivePageIndex, pageArea);
                this.renderer.hideHeaderFooterInfo(pageArea);
            }
            if(layoutSelection.subDocumentInfo.isHeaderFooter) {
                this.scrollToHeaderFooter(layoutSelection.subDocumentInfo.isHeader, layoutSelection.pageIndex);
                this.renderHeaderFooter(this.layout.pages[layoutSelection.pageIndex], layoutSelection.subDocumentInfo.subDocumentId);
            }
            this.activeSubDocumentInfo = layoutSelection.subDocumentInfo;
        }
        private renderHeaderFooter(layoutPage: LayoutPage, headerFooterSubDocumentId: number) {
            const layoutArea: LayoutPageArea = layoutPage.otherPageAreas[headerFooterSubDocumentId];
            if (layoutArea) {
                const pageIndex: number = layoutPage.index;
                this.renderer.activateOtherPageArea(pageIndex, layoutArea);
                const infoTexts: string[] = this.getHeaderFooterInfoTexts(layoutPage, layoutArea);
                this.renderer.showHeaderFooterInfo(pageIndex, layoutArea, infoTexts[0], infoTexts[1]);
            }
        }
        getHeaderFooterInfoTexts(layoutPage: LayoutPage, layoutArea: LayoutPageArea): string[] {
            let subDocument = layoutArea.subDocument;
            let sectionIndex = HeaderFooterCommandBase.getSectionIndex(layoutPage, subDocument.documentModel);
            let section = subDocument.documentModel.sections[sectionIndex];
            let result = [];
            if(section.sectionProperties.differentFirstPage && HeaderFooterCommandBase.isFirstPageInSection(layoutPage, section))
                result.push(subDocument.isHeader() ? this.stringResources.firstPageHeader : this.stringResources.firstPageFooter);
            else if(subDocument.documentModel.differentOddAndEvenPages) {
                if(Utils.isEven(layoutPage.index))
                    result.push(subDocument.isHeader() ? this.stringResources.evenPageHeader : this.stringResources.evenPageFooter);
                else
                    result.push(subDocument.isHeader() ? this.stringResources.oddPageHeader : this.stringResources.oddPageFooter);
            }
            else
                result.push(subDocument.isHeader() ? this.stringResources.header : this.stringResources.footer);

            let type = (<HeaderFooterSubDocumentInfoBase>subDocument.info).headerFooterType;
            if((subDocument.isHeader() && section.headers.isLinkedToPrevious(type)) || (subDocument.isFooter() && section.footers.isLinkedToPrevious(type)))
                result.push(this.stringResources.sameAsPrevious);
            return result;
        }

        private updateVisiblePages() {
            const pages: LayoutPage[] = this.layout.pages;
            this.scroll.updatePageIndexesInfo(pages);
            const renderInterval: FixedInterval = FixedInterval.fromPositions(this.scroll.getStartRenderPageIndex(), this.scroll.getEndRenderPageIndex());
            const renderInnerContentMap: { [pageIndex: number]: boolean } = {}; // false - mean need delete inner content. true - need render inner content
            for (let page of this.visiblePages)
                if (!renderInterval.contains(page.index)) {
                    renderInnerContentMap[page.index] = false;
                    page.isNeedFullyRender = false;
                }

            this.visiblePages = [];
            const endPageIndex: number = Math.min(renderInterval.end(), pages.length - 1);
            
            for (let pageIndex: number = renderInterval.start; pageIndex <= endPageIndex; pageIndex++) {
                if (!this.renderer.cache[pageIndex])
                    continue;
                const page: LayoutPage = pages[pageIndex];
                if (!page.isNeedFullyRender)
                    renderInnerContentMap[page.index] = true;
                page.isNeedFullyRender = true;
                this.visiblePages.push(page);
            }

            for (let key in renderInnerContentMap) {
                const pageIndex: number = key;
                this.renderer.renderPage(pages[pageIndex], renderInnerContentMap[pageIndex], this.layout.pageColor, this.layout.isShowTableGridLines);
                if(renderInnerContentMap[pageIndex] && this.lastActivePageIndex == pageIndex) {
                    if(this.activeSubDocumentInfo && this.activeSubDocumentInfo.isHeaderFooter) {
                        //TODO - move to renderer!
                        if(!ASPx.GetChildNodesByClassName(this.renderer.cache[pageIndex].page, DocumentRenderer.CLASSNAMES.ACTIVE).length)
                            this.renderHeaderFooter(pages[pageIndex], this.activeSubDocumentInfo.subDocumentId);
                    }
                }
            }
        }

        private isVisiblePosition(pageIndex: number, offsetX: number, offsetY: number): boolean {
            const pages: LayoutPage[] = this.layout.pages;
            this.scroll.updatePageIndexesInfo(pages);
            if(pageIndex < this.scroll.startVisiblePageIndex || pageIndex > this.scroll.endVisiblePageIndex)
                return false;
            const pageY: number = this.sizes.getPageOffsetY(pages[pageIndex]);
            const pageX: number = this.renderer.cache[pageIndex].page.offsetLeft;
            const x: number = pageX + offsetX;
            const y: number = pageY + offsetY;
            const isVisiblePosition: boolean = x >= this.scroll.lastScrollLeft && x <= this.sizes.getVisibleAreaWidth(false) + this.scroll.lastScrollLeft &&
                y >= this.scroll.lastScrollTop && y <= this.sizes.getVisibleAreaHeight(false) + this.scroll.lastScrollTop;
            return isVisiblePosition;
        }

        private initCommonEvents() {
            ASPx.Evt.AttachEventToElement(this.canvas, "scroll", () => this.onCanvasScroll());
        }
        private initMouseEvents() {
            ASPx.Evt.AttachEventToElement(this.canvas, "mousedown", (evt: MouseEvent) => this.onCanvasMouseDown(evt));
            ASPx.Evt.AttachEventToElement(this.canvas, "mousemove", (evt: MouseEvent) => this.onCanvasMouseMove(evt));
            ASPx.Evt.AttachEventToElement(this.canvas, "dblclick", (evt: MouseEvent) => this.onCanvasMouseDblClick(evt));
            ASPx.Evt.AttachEventToElement(this.canvas, "mousewheel", (evt: WheelEvent) => this.onCanvasMouseWheel(evt));
            ASPx.Evt.AttachEventToDocument("mouseup", (evt: MouseEvent) => this.onDocumentMouseUp(evt));
            ASPx.Evt.AttachEventToDocument("mousemove", (evt: MouseEvent) => this.onDocumentMouseMove(evt));
            ASPx.Evt.AttachEventToDocument("contextmenu", (evt: MouseEvent) => this.onDocumentContextMenu(evt));
        }
        private initTouchEvents() {
            ASPx.Evt.AttachEventToElement(this.canvas, "touchstart", (evt: MouseEvent) => this.onCanvasTouchStart(evt));
            ASPx.Evt.AttachEventToElement(this.canvas, "touchend", (evt: MouseEvent) => this.onCanvasTouchEnd(evt));
            ASPx.Evt.AttachEventToElement(this.canvas, "touchmove", (evt: MouseEvent) => this.onCanvasTouchMove(evt));
            ASPx.Evt.AttachEventToElement(this.canvas, "gesturestart", (evt: MouseEvent) => this.onCanvasGestureStart(evt));

            ASPx.Evt.AttachEventToDocument("touchend", (evt: MouseEvent) => this.onDocumentTouchEnd(evt));
            ASPx.Evt.AttachEventToDocument("touchmove", (evt: MouseEvent) => this.onDocumentTouchMove(evt));
        }
        private initPointerEvents() {
            ASPx.Evt.AttachEventToElement(this.canvas, "pointerdown", (evt: PointerEvent) => this.onCanvasPointerDown(evt));
            ASPx.Evt.AttachEventToElement(this.canvas, "pointermove", (evt: PointerEvent) => this.onCanvasPointerMove(evt));
            ASPx.Evt.AttachEventToElement(this.canvas, "pointerup", (evt: PointerEvent) => this.onCanvasPointerUp(evt));
        }
        private initMSPointerEvents() {
            ASPx.Evt.AttachEventToElement(this.canvas, "mspointerdown", (evt: PointerEvent) => this.onCanvasPointerDown(evt));
            ASPx.Evt.AttachEventToElement(this.canvas, "mspointermove", (evt: PointerEvent) => this.onCanvasPointerMove(evt));
            ASPx.Evt.AttachEventToElement(this.canvas, "mspointerup", (evt: PointerEvent) => this.onCanvasPointerUp(evt));
        }

        // selection
        private updatePageSelection(pageIndex: number, items: LayoutSelectionItem[]) {
            const existItems = this.selectionCache.items[pageIndex];
            const page: LayoutPage = this.layout.pages[pageIndex];
            if (page && page.isNeedFullyRender) {
                const newItemsSet: { [hash: string]: boolean } = {};
                for(let i = 0, item: LayoutSelectionItem; item = items[i]; i++)
                    newItemsSet[item.getHash()] = true;
                const outdatedItems: LayoutSelectionItem[] = [];
                if(existItems) {
                    for(let i = 0, item: LayoutSelectionItem; item = existItems[i]; i++) {
                        let hash = item.getHash();
                        if(newItemsSet[hash])
                            delete newItemsSet[hash];
                        else
                            outdatedItems.push(item);
                    }
                }
                for(let item of items) {
                    if(newItemsSet[item.getHash()])
                        this.renderer.selection.replaceOrAddItem(item, outdatedItems.pop());
                }
                if(outdatedItems.length)
                    this.renderer.selection.removeItems(pageIndex, outdatedItems);
            }
            this.selectionCache.items[pageIndex] = items;
        }

        private updateScrollPosition(position: LayoutPosition) {
            if (!position)
                return;
            const page: LayoutPage = this.layout.pages[position.pageIndex];
            if (!page || !page.isNeedFullyRender)
                return;
            if(this.updateScrollTimeoutId > 0) {
                clearTimeout(this.updateScrollTimeoutId);
                this.updateScrollTimeoutId = -1;
            }
            if(this.waitForDblClickTimeoutId > 0) {
                this.updateScrollTimeoutId = setTimeout(() => {
                    this.scrollToX(position, RelativePosition.Inside);
                }, MouseHandler.WAIT_FOR_DBLCLICK_INTERVAL);
            }
            else
                this.scrollToX(position, RelativePosition.Inside);
        }
        private updateTouchBarsIfRequired(layoutSelection: LayoutSelection): void {
            if(!ASPx.Browser.TouchUI || !layoutSelection) // TODO
                return;
            let firstItem = layoutSelection.firstItem;
            let lastItem = layoutSelection.lastItem;
            const pages: LayoutPage[] = this.layout.pages;
            this.renderer.selection.renderTouchBars(firstItem, lastItem, firstItem && pages[firstItem.pageIndex].isNeedFullyRender, lastItem && pages[lastItem.pageIndex].isNeedFullyRender);
        }

        private scrollToHeaderFooter(isHeader: boolean, pageIndex: number) {
            this.setScrollEnabled(false);
            let scrollPosition = new LayoutPosition(DocumentLayoutDetailsLevel.Page);
            scrollPosition.pageIndex = pageIndex;
            scrollPosition.page = this.layout.pages[pageIndex];
            this.scrollToX(scrollPosition, isHeader ? RelativePosition.Top : RelativePosition.Bottom, isHeader ? -HEADERFOOTER_SCROLLOFFSET : HEADERFOOTER_SCROLLOFFSET);
        }
        private scrollToX(position: LayoutPosition, relativePosition: RelativePosition, offsetY: number = 0) {
            let pageIndex = position.pageIndex;
            let y = this.sizes.getPageOffsetY(this.layout.pages[pageIndex]);
            let height = 0;
            if(position.detailsLevel >= DocumentLayoutDetailsLevel.PageArea) {
                y += position.pageArea.y;
                if(position.detailsLevel >= DocumentLayoutDetailsLevel.Column) {
                    y += position.column.y;
                    if(position.detailsLevel >= DocumentLayoutDetailsLevel.Row) {
                        y += position.row.y;
                        if(relativePosition === RelativePosition.Bottom)
                            y += position.row.height;
                        else if(relativePosition === RelativePosition.Inside)
                            height = position.row.height;
                    }
                    else if(relativePosition === RelativePosition.Bottom)
                        y += position.column.height;
                    else if(relativePosition === RelativePosition.Inside)
                        height = position.column.height;
                }
                else if(relativePosition === RelativePosition.Bottom)
                    y += position.pageArea.height;
                else if(relativePosition === RelativePosition.Inside)
                    height = position.pageArea.height;
            }
            else if(relativePosition === RelativePosition.Bottom)
                y += position.page.height;
            else if(relativePosition === RelativePosition.Inside)
                height = position.page.height;
            if(relativePosition === RelativePosition.Bottom)
                y -= this.sizes.getVisibleAreaHeight(false);
            y += offsetY;
            if(relativePosition === RelativePosition.Inside) {
                let scrollTop = this.canvas.scrollTop;
                const scrollVisibleAreaHeight = this.sizes.getVisibleAreaHeight(false);
                if(y < scrollTop)
                    this.canvas.scrollTop = y;
                else if(y + height > scrollVisibleAreaHeight + scrollTop)
                    this.canvas.scrollTop = y + height - scrollVisibleAreaHeight
            }
            else
                this.canvas.scrollTop = y;
        }
        private setScrollEnabled(enabled: boolean) {
            if(enabled)
                this.canvas.style.overflow = "";
            else
                this.canvas.style.overflow = "hidden";
            this.updateScrollVisibility();
        }
        private resetScrollInterval() {
            if(this.scrollIntervalID) {
                clearInterval(this.scrollIntervalID);
                this.scrollIntervalID = 0;
            }
        }
        private saveMousePosition(evt: MouseEvent) {
            this.lastMousePosition.x = ASPx.Evt.GetEventX(evt);
            this.lastMousePosition.y = ASPx.Evt.GetEventY(evt);
        }
        private onScrollIntervalTick() {
            const evtX = this.lastMousePosition.x;
            const evtY = this.lastMousePosition.y;
            const inHorizontalArea = evtX >= this.canvasPosition.x && evtX <= this.canvasPosition.x + this.sizes.getVisibleAreaWidth(true);
            const inVerticalArea = evtY >= this.canvasPosition.y && evtY <= this.canvasPosition.y + this.sizes.getVisibleAreaHeight(true);

            if(!inHorizontalArea && !inVerticalArea)
                return;

            if(inHorizontalArea && evtY - this.canvasPosition.y <= AUTOSCROLL_AREA_SIZE)
                this.canvas.scrollTop -= AUTOSCROLL_STEP;
            else if(inHorizontalArea && this.canvasPosition.y + this.sizes.getVisibleAreaHeight(true) - evtY <= AUTOSCROLL_AREA_SIZE)
                this.canvas.scrollTop += AUTOSCROLL_STEP;
            if(inVerticalArea && evtX - this.canvasPosition.x <= AUTOSCROLL_AREA_SIZE)
                this.canvas.scrollLeft -= AUTOSCROLL_STEP;
            else if(inVerticalArea && this.canvasPosition.x + this.sizes.getVisibleAreaWidth(true) - evtX <= AUTOSCROLL_AREA_SIZE)
                this.canvas.scrollLeft += AUTOSCROLL_STEP;
        }
        private static insertAfter(element: Node, refNode: Node) {
            if(refNode.nextSibling)
                refNode.parentNode.insertBefore(element, refNode.nextSibling);
            else
                refNode.parentNode.appendChild(element);
        }

        onUpdateUnlocked(occurredEvents: number) {
            this.updateVisiblePages();
        }
    }
    
    export class CanvasSelectionCache {
        public items: { [pageIndex: number]: LayoutSelectionItem[] } = {};
        public elements: { [key: string]: HTMLElement } = {};

        public findLastPageIndex(): number {
            let lastPageIndex: number = 0;
            for (let key in this.items) {
                if (!this.items.hasOwnProperty(key))
                    continue;

                const pageIndex: number = parseInt(key);
                if (pageIndex > lastPageIndex)
                    lastPageIndex = pageIndex;
            }
            return lastPageIndex;
        }

        public removeItemsByPageIndex(pageIndex: number) {
            const items: LayoutSelectionItem[] = this.items[pageIndex];
            if(!items)
                return;
            for(let item of items)
                delete this.elements[item.getHash()];
            delete this.items[pageIndex];
        }
        public removeItemsByPageArea(pageIndex: number, subDocumentId: number, pageAreaIndex: number) {
            this.removeItemsByExpression(pageIndex, item => item.pageAreaIndex === pageAreaIndex && item.subDocumentID === subDocumentId);
        }
        public removeItemsByColumn(pageIndex: number, subDocumentId: number, pageAreaIndex: number, columnIndex: number) {
            this.removeItemsByExpression(pageIndex, item => item.pageAreaIndex === pageAreaIndex && item.subDocumentID === subDocumentId && item.columnIndex === columnIndex);
        }
        public moveItemsToPage(fromPageIndex: number, toPageIndex: number) {
            if(this.items[fromPageIndex]) {
                this.items[toPageIndex] = this.items[fromPageIndex];
                delete this.items[fromPageIndex];
            }
        }
        public forEachPageIndex(func: (pageIndex: number) => void) {
            for(let pageIndex in this.items) {
                if(!this.items.hasOwnProperty(pageIndex))
                    continue;
                func(pageIndex);
            }
        }
        public removeItemsByExpression(pageIndex: number, exprFunc: (item: LayoutSelectionItem) => boolean) {
            const itemsByPage: LayoutSelectionItem[] = this.items[pageIndex];
            if (!itemsByPage)
                return;
            for (let itemIndex = 0, item: LayoutSelectionItem; item = itemsByPage[itemIndex]; itemIndex++) {
                if (exprFunc(item)) {
                    delete this.elements[item.getHash()];
                    itemsByPage.splice(itemIndex, 1);
                    itemIndex--;
                }
            }
            if (itemsByPage.length === 0)
                delete this.items[pageIndex];
        }
        public close() {
            this.elements = {};
            this.items = {};
        }
    }

    class CanvasScrollInfo {
        private canvas: HTMLDivElement;
        private sizes: CanvasSizeInfo;
        private renderPagesOffset: number;
        private visiblePagesMaxCount: number; 

        lastScrollTop: number = -1;
        lastScrollLeft: number = -1;

        startVisiblePageIndex: number = 0;
        endVisiblePageIndex: number = 0;

        constructor(canvas: HTMLDivElement, sizes: CanvasSizeInfo) {
            this.canvas = canvas;
            this.sizes = sizes;
            this.renderPagesOffset = ASPx.Browser.TouchUI ? VISIBLE_PAGES_RANGE_TOUCH : VISIBLE_PAGES_RANGE;
            this.visiblePagesMaxCount = ASPx.Browser.TouchUI ? VISIBLE_PAGES_MAXCOUNT_TOUCH : VISIBLE_PAGES_MAXCOUNT;
        }

        public getStartRenderPageIndex(): number {
            return Math.max(0, this.startVisiblePageIndex - this.renderPagesOffset);
        }

        public getEndRenderPageIndex(): number {
            return this.endVisiblePageIndex + this.renderPagesOffset;
        }

        public updatePageIndexesInfo(pages: LayoutPage[]) {
            if (!pages.length)
                return;

            const scrollTop: number = this.canvas.scrollTop;
            this.lastScrollLeft = this.canvas.scrollLeft; // maybe need move below.
            if (this.startVisiblePageIndex >= 0 && scrollTop == this.lastScrollTop)
                return;

            this.startVisiblePageIndex = this.sizes.findPageIndexByOffsetY(pages, this.canvas.scrollTop);
            this.endVisiblePageIndex = this.sizes.findPageIndexByOffsetY(pages, this.canvas.scrollTop + this.sizes.getVisibleAreaHeight(false));

            this.lastScrollTop = scrollTop;
        }

        public setPageVisability(page: LayoutPage) {
            const renderInterval: FixedInterval = FixedInterval.fromPositions(this.getStartRenderPageIndex(), this.getEndRenderPageIndex());
            page.isNeedFullyRender = renderInterval.contains(page.index);
        }
    }

    export class CanvasSizeInfo {
        public topSpacing = -1;
        public betweenPageSpacing = 1;
        public scrollXVisible: boolean;
        public scrollYVisible: boolean;

        private visibleAreaSize: Size = new Size(-1, -1);
        private scrollWidth = -1;

        constructor() {
            this.scrollWidth = ASPx.GetVerticalScrollBarWidth();
        }

        public findPageIndexByOffsetY(pages: LayoutPage[], offsetY: number) {
            return Math.max(0, Utils.normedBinaryIndexOf(pages, (p: LayoutPage) => {
                return this.getPageOffsetY(p) - offsetY;
            }));
        }
        public getPageOffsetY(layoutPage: LayoutPage): number {
            return layoutPage.y + (this.topSpacing + layoutPage.index * this.betweenPageSpacing);
        }

        public setVisibleAreaSize(width: number, height: number) {
            this.visibleAreaSize.width = width;
            this.visibleAreaSize.height = height;
        }

        public getVisibleAreaWidth(includeScrollBars: boolean): number { // default = false
            if (includeScrollBars)
                return this.visibleAreaSize.width;
            return this.scrollYVisible ? this.visibleAreaSize.width - this.scrollWidth : this.visibleAreaSize.width;
        }
        public getVisibleAreaHeight(includeScrollBars: boolean): number { // default - false
            if (includeScrollBars)
                return this.visibleAreaSize.height;
            return this.scrollXVisible ? this.visibleAreaSize.height - this.scrollWidth : this.visibleAreaSize.height;
        }

        public updateScrollVisibility(measurerWidth: number, measurerHeight: number) {
            this.scrollXVisible = measurerHeight < this.visibleAreaSize.height;
            this.scrollYVisible = measurerWidth < this.visibleAreaSize.width;
        }
    }

    enum RelativePosition {
        Top,
        Bottom,
        Inside
    }
}