module __aspxRichEdit {
    export module Ruler {
        export class DisplayControlBase {
            rootElement: Node;
            constructor() {
                this.rootElement = this.createRootElement();
            }
            initialize(): void {
                this.initializeInternal();
                this.afterInitialize();
            }
            initializeInternal(): void {
                this.createControlHierarchy();
                this.prepareControlHierarchy();
            }
            afterInitialize(): void {
            }
            createControlHierarchy(): void {
            }
            prepareControlHierarchy(): void {
            }
            createRootElement(): Node {
                return document.createElement("DIV");
            }
            getHtmlRootElement(): HTMLElement {
                return <HTMLElement>this.rootElement;
            }
        }

        export class TabAlignBoxControl extends DisplayControlBase {
            align: TabAlign = TabAlign.Left;
            private settings: any;
            private innerSquareElement: HTMLElement;
            private alignElement: HTMLElement;
            private enable: boolean = false;

            constructor(settings: any) {
                super();
                this.settings = settings;
            }

            afterInitialize(): void {
                ASPx.Evt.AttachEventToElement(this.innerSquareElement, ASPx.TouchUIHelper.touchMouseDownEventName, (evt: MouseEvent) => this.onMouseDown(evt));
            }
            createControlHierarchy(): void {
                super.createControlHierarchy();
                this.innerSquareElement = document.createElement("DIV");
                this.rootElement.appendChild(this.innerSquareElement);
                this.alignElement = document.createElement("DIV");
                this.innerSquareElement.appendChild(this.alignElement);
            }
            prepareControlHierarchy(): void {
                super.prepareControlHierarchy();
                this.getHtmlRootElement().className = TAB_ALIGN_BOX_PART_HANDLE_CLASS_NAME;
                this.alignElement.className = RulerUtils.getSpriteClassName(this.align, this.settings);
            }
            adjust(offset: number, size: number): void {
                this.innerSquareElement.style.width = size + "px"
                this.innerSquareElement.style.height = size + "px"
                this.innerSquareElement.style.top = offset + "px";
                this.innerSquareElement.style.left = offset + "px";
                this.getHtmlRootElement().style.width = size + 2 * offset + "px";
                this.getHtmlRootElement().style.height = size + 2 * offset + "px";
                this.adjustAlignElement();
            }
            setEnable(enable: boolean): void {
                this.enable = enable;
            }
            private adjustAlignElement(): void {
                var size = this.innerSquareElement.offsetWidth;
                this.alignElement.style.top = (size - this.alignElement.offsetHeight) / 2 + "px";
                this.alignElement.style.left = (size - this.alignElement.offsetWidth) / 2 + "px";
                this.alignElement.title = this.settings.titles[RulerUtils.getTabTitlePropertyName(this.align)];
            }
            private onMouseDown(evt): void {
                if (!this.enable) {
                    this.align++;
                    if (this.align > TabAlign.Decimal)
                        this.align = TabAlign.Left;
                    this.alignElement.className = RulerUtils.getSpriteClassName(this.align, this.settings);
                    this.adjustAlignElement();
                    ASPx.Evt.PreventEvent(evt);
                }
            }
        }

        export class PureDragDisplayControl extends DisplayControlBase {
            settings: RulerSettings;

            constructor(settings: RulerSettings) {
                super();
                this.settings = settings;
            }

            setPosition(value: number, snapTo: SnapTo = SnapTo.LeftSide): void {
            }
            canHandle(source: HTMLElement): boolean {
                return source == this.getHandleElement();
            }
            getHandleElement(): HTMLElement {
                return this.getHtmlRootElement();
            }
        }

        export class DragDisplayControl extends PureDragDisplayControl {
            divisionsHelper: DivisionsUnitHelper;
            position: number = 0;
            shadowElements: Node[] = [];

            constructor(settings: any, divisionsHelper: DivisionsUnitHelper) {
                super(settings);
                this.divisionsHelper = divisionsHelper;
            }

            afterInitialize(): void {
                super.afterInitialize();
                if (this.hasShadow())
                    this.createShadowElements();
            }
            createShadowElements(): void {
                Utils.foreach(this.getClonedElements(), element => {
                    var cloneElement = element.cloneNode(true);
                    (<HTMLElement>cloneElement).style.display = "none";
                    ASPx.SetElementOpacity(<HTMLElement>cloneElement, 0.5);
                    this.shadowElements.push(cloneElement);
                    this.getHtmlRootElement().parentNode.appendChild(cloneElement);
                });
            }

            setPosition(value: number): void {
                var elements = this.getMovableElements();
                value = this.getCorrectedValue(value);
                if (this.position != value)
                    for (var i = 0; i < elements.length; i++)
                        this.setPositionInternal(elements[i], value);
            }
            setPositionInternal(element: HTMLElement, value: number): void {
                this.position = value;
                element.style[this.getMovableStyleName()] = this.position.toString() + "px";
            }
            getCorrectedValue(value: number): number {
                return value;
            }
            getMovableElements(): HTMLElement[] {
                return [];
            }
            getMovableStyleName(): string {
                return "left";
            }

            hasShadow(): boolean {
                return false;
            }
            showShadow(): void {
                Utils.foreach(this.shadowElements, element => {
                    (<HTMLElement>element).style.display = "";
                    (<HTMLElement>element).style[this.getMovableStyleName()] = this.position.toString() + "px";
                });
            }
            hideShadow(): void {
                Utils.foreach(this.shadowElements, element => { (<HTMLElement>element).style.display = "none"; });
            }
            getClonedElements(): HTMLElement[] {
                return [this.getHtmlRootElement()];
            }
        }

        export class AdvancedDragDisplayControl extends DragDisplayControl {
            constructor(settins: any, divisionsHelper: DivisionsUnitHelper) {
                super(settins, divisionsHelper);
            }
            prepareControlHierarchy(): void {
                super.prepareControlHierarchy();
                this.setTitle();
            }
            setTitle(): void {
                this.getTitleElement().title = this.getTitle();
            }
            getTitleElement(): HTMLElement {
                return null;
            }
            getTitle(): string {
                return "";
            }
        }

        // DragHandleIdent Controls
        export class DragHandleBase extends AdvancedDragDisplayControl {
            private heightOfProtrudingPart: number = 0;
            leftCorrection: number = 0;
            divisionContol: DivisionsControl;

            constructor(settings: any, divisionHelper: DivisionsUnitHelper, divisionContol: DivisionsControl) {
                super(settings, divisionHelper);
                this.divisionContol = divisionContol;
            }
            prepareControlHierarchy(): void {
                super.prepareControlHierarchy();
                this.getHtmlRootElement().className = this.getClassName();
                this.adjustByTop();
            }
            adjustByTop(): void {
                var mainElementHeight = this.getHtmlRootElement().offsetHeight;
                var divisionsControlHeight = this.divisionContol.getHeight();
                this.getHtmlRootElement().style.marginTop = (this.getIsTopPosition() ? -(divisionsControlHeight - mainElementHeight) : divisionsControlHeight) / 2 + "px";
            }
            afterInitialize(): void {
                super.afterInitialize();
                this.leftCorrection = Math.round(this.getHtmlRootElement().offsetWidth / 2);
                this.heightOfProtrudingPart = this.getHtmlRootElement().offsetHeight - this.divisionContol.getHeight() / 2;
            }

            getTitleElement(): HTMLElement {
                return this.getHtmlRootElement();
            }
            getMovableStyleName(): string {
                return "left";
            }
            getCorrectedValue(value: number): number {
                return RULLER_NUMBER_CORRECTION + value - this.leftCorrection;
            }
            getMovableElements(): HTMLElement[] {
                return [this.getHtmlRootElement()];
            }

            getHeightOfProtrudingPart(): number {
                return this.heightOfProtrudingPart;
            }
            getIsTopPosition(): boolean {
                return true;
            }
            getClassName(): string {
                return "";
            }

            hasShadow(): boolean {
                return true;
            }
        }
        export class FirstLineIdentDragHandleControl extends DragHandleBase {
            getTitle(): string {
                return this.settings.titles.firstLineIdent;
            }
            getClassName(): string {
                return this.settings.styles.firstLineIdentImage.spriteCssClass + " " + this.settings.styles.firstLineIdent.className;
            }
        }
        export class RightIdentDragHandleControl extends DragHandleBase {
            getMovableStyleName(): string {
                return "right";
            }
            getIsTopPosition(): boolean {
                return false;
            }
            getTitle(): string {
                return this.settings.titles.rightIdent;
            }
            getClassName(): string {
                return this.settings.styles.rightIdentImage.spriteCssClass + " " + this.settings.styles.rightIdent.className;
            }
        }
        export class LeftIdentDragHandleControl extends DragHandleBase {
            private topElement: HTMLElement;
            private bodyElement: HTMLElement;

            createControlHierarchy(): void {
                super.createControlHierarchy();
                this.topElement = document.createElement("DIV");
                this.topElement.className = this.settings.styles.leftIdentImage.spriteCssClass;

                this.bodyElement = document.createElement("DIV");
                this.bodyElement.className = LEFT_IDENT_DRAG_HANDLE_BODY;

                this.rootElement.appendChild(this.topElement);
                this.rootElement.appendChild(this.bodyElement);
            }
            prepareControlHierarchy(): void {
                super.prepareControlHierarchy();
                var mainElementWidth = this.topElement.offsetWidth;

                this.bodyElement.style.width = mainElementWidth + "px";

                var style = this.getHtmlRootElement().style;
                style.height = this.topElement.offsetHeight + this.bodyElement.offsetHeight + "px";
                style.width = mainElementWidth + "px";
                style.marginTop = this.divisionContol.getHeight() / 2 + "px";

                this.bodyElement.title = this.getTitleElement().title = this.settings.titles.leftIdent;
                this.topElement.title = this.getTitleElement().title = this.settings.titles.hangingIdent;
            }
            canHandleLeftIden(source: HTMLElement): boolean {
                return this.topElement == source;
            }
            canHadleHangingIdent(source: HTMLElement): boolean {
                return this.bodyElement == source;
            }
            getClassName(): string {
                return this.settings.styles.leftIdent.className;
            }
        }
        export class TabDragHandleControl extends DragHandleBase {
            private align: TabAlign = TabAlign.Left;
            private visible: boolean = true;

            constructor(settings: any, divisionHelper: DivisionsUnitHelper, divisionContol: DivisionsControl, align: TabAlign) {
                super(settings, divisionHelper, divisionContol);
                this.align = align;
            }
            afterInitialize(): void {
                super.afterInitialize();
                this.setCorrection();
            }
            setCorrection(): void {
                switch (this.align) {
                    case TabAlign.Left:
                        this.leftCorrection = 0;
                        break;
                    case TabAlign.Right:
                        this.leftCorrection = this.getHtmlRootElement().offsetWidth;
                        break;
                    case TabAlign.Center:
                    case TabAlign.Decimal:
                        this.leftCorrection = Math.round(this.getHtmlRootElement().offsetWidth / 2);
                        break;
                }
            }
            adjustByTop(): void {
                this.getHtmlRootElement().style.marginTop = (this.divisionContol.getHeight() - this.getHtmlRootElement().offsetHeight) + "px";
            }
            getClassName(): string {
                return this.settings.styles.tab.className + " " + RulerUtils.getSpriteClassName(this.align, this.settings);
            }
            getTitle(): string {
                return this.settings.titles[RulerUtils.getTabTitlePropertyName(this.align)];
            }
            changeAlign(align: TabAlign): void {
                if (this.align != align) {
                    this.align = align;

                    (<HTMLElement>this.shadowElements[0]).className = this.getClassName();
                    this.getHtmlRootElement().className = this.getClassName();

                    this.setTitle();
                    this.setCorrection();
                }
            }
            setVisible(visible: boolean): void {
                this.visible = visible;
                this.getHtmlRootElement().style.display = visible ? "" : "none";
            }
            getVisible(): boolean {
                return this.visible;
            }
        }

        export class MarginControlBase extends AdvancedDragDisplayControl {
            marginPanelElement: HTMLElement;
            handlePanelElement: HTMLElement;

            createControlHierarchy(): void {
                super.createControlHierarchy();
                this.marginPanelElement = document.createElement("DIV");
                this.handlePanelElement = document.createElement("DIV");
                this.rootElement.appendChild(this.marginPanelElement);
                this.rootElement.appendChild(this.handlePanelElement);
            }
            prepareControlHierarchy(): void {
                super.prepareControlHierarchy();
                this.marginPanelElement.className = this.getMarginElementClassName();
                this.handlePanelElement.className = this.getHandlePanelElementClassName();
            }
            createRootElement(): Node {
                return document.createDocumentFragment();
            }
            getHandleElement(): HTMLElement {
                return this.handlePanelElement;
            }
            getTitleElement(): HTMLElement {
                return this.handlePanelElement;
            }
            getMovableElements(): HTMLElement[] {
                return [this.marginPanelElement, this.handlePanelElement];
            }
            getMarginElementClassName(): string {
                return "";
            }
            getHandlePanelElementClassName(): string {
                return "";
            }
            setMoveCursorVisibility(visible: boolean): void {
                this.handlePanelElement.style.cursor = visible ? "" : "default";
            }
        }
        export class LeftMarginDragHandleControl extends MarginControlBase {
            private initialLeft: number = 0;
            private maxPageWidth: number = 0;

            constructor(settins: any, divisionHelper: DivisionsUnitHelper, maxPageWidth: number) {
                super(settins, divisionHelper);
                this.maxPageWidth = maxPageWidth;
            }

            prepareControlHierarchy(): void {
                super.prepareControlHierarchy();
                var width = this.maxPageWidth;
                this.marginPanelElement.style.width = width + "px";
                this.handlePanelElement.style.width = width + "px";
            }
            afterInitialize(): void {
                super.afterInitialize();
                this.initialLeft = -(Math.ceil(this.maxPageWidth / this.divisionsHelper.getUnitSize() * this.divisionsHelper.getUnitSize()) - RULLER_NUMBER_CORRECTION);
            }
            getCorrectedValue(value: number): number {
                return this.initialLeft + value;
            }
            getMovableStyleName(): string {
                return "left";
            }
            getTitle(): string {
                return this.settings.titles.marginLeft;
            }
            getMarginElementClassName(): string {
                return DIVISION_MARGIN_LEFT_CLASS_NAME;
            }
            getHandlePanelElementClassName(): string {
                return DIVISION_MARGIN_LEFT_CURSOR_CLASS_NAME;
            }
        }
        export class RightMarginDragHandleControl extends MarginControlBase {
            getCorrectedValue(value: number): number {
                return RULLER_NUMBER_CORRECTION + value
            }
            getMovableStyleName(): string {
                return "width";
            }
            getTitle(): string {
                return this.settings.titles.marginRight;
            }
            getMarginElementClassName(): string {
                return DIVISION_MARGIN_RIGHT_CLASS_NAME;
            }
            getHandlePanelElementClassName(): string {
                return DIVISION_MARGIN_RIGHT_CURSOR_CLASS_NAME;
            }
        }

        export class DivisionsControl extends DragDisplayControl {
            private height: number = 0;
            private initialLeft: number = 0;
            private unitCount: number = 0;
            private divisionLeftPosition: number = 0;
            private maxPageWidth: number = 0;

            constructor(settings: any, divisionHeler: DivisionsUnitHelper, maxPageWidth: number) {
                super(settings, divisionHeler);
                this.maxPageWidth = maxPageWidth;
            }

            afterInitialize(): void {
                this.initialLeft = -(this.unitCount * this.divisionsHelper.getUnitSize() - RULLER_NUMBER_CORRECTION);
            }
            createControlHierarchy(): void {
                this.unitCount = Math.ceil(this.maxPageWidth / this.divisionsHelper.getUnitSize());
                this.getHtmlRootElement().className = DIVISION_CONTAINER_CLASS_NAME;
                this.getHtmlRootElement().style.width = this.divisionsHelper.getUnitSize() * (this.unitCount * 2 + 1) + "px";

                if (ASPx.Browser.IE && ASPx.Browser.MajorVersion <= 9) //T182655
                    this.getHtmlRootElement().offsetParent;

                this.height = this.getHtmlRootElement().offsetHeight;
                this.createDivisionElements();
            }
            createDivisionElements(): void {
                for (var i = this.unitCount; i > 0; i--)
                    this.rootElement.appendChild(this.createDivision(i));
                for (var j = 0; j <= this.unitCount; j++)
                    this.rootElement.appendChild(this.createDivision(j));
            }
            createDivision(numb: number): DocumentFragment {
                var fragment = document.createDocumentFragment();
                var map = this.divisionsHelper.getUnitMap();
                for (var i = 0; i < map.length; i++) {
                    switch (map[i]) {
                        case DivisionType.Number:
                            fragment.appendChild(this.createNumberDivision(DIVISION_NUMBER_CLASS_NAME, numb));
                            break;
                        case DivisionType.Major:
                            fragment.appendChild(this.createSimpleDivision(DIVISION_MAJOR_CLASS_NAME, MAJOR_TOP_AND_BOTTOM_MARGIN));
                            break;
                        case DivisionType.Minor:
                            fragment.appendChild(this.createSimpleDivision(DIVISION_MINOR_CLASS_NAME, MINOR_TOP_AND_BOTTOM_MARGIN));
                            break;
                    }
                }
                return fragment;
            }
            createSimpleDivision(className: string, topAndBottomMargin: number): HTMLElement {
                if (!topAndBottomMargin)
                    topAndBottomMargin = 0;
                var stepSize = this.divisionsHelper.getStepSize();
                var element = document.createElement("DIV");
                element.style.left = this.divisionLeftPosition + "px";
                element.style.width = stepSize + "px";
                element.style.height = this.height - topAndBottomMargin * 2 + "px";
                element.style.marginTop = topAndBottomMargin + "px";
                element.style.marginBottom = topAndBottomMargin + "px";
                element.className = className;
                this.divisionLeftPosition += stepSize;

                return element;
            }
            createNumberDivision(className: string, numb: number): HTMLElement {
                var element = this.createSimpleDivision(className, null);
                if (numb != 0) {
                    var numberElement = document.createElement("DIV");
                    numberElement.innerHTML = numb.toString();
                    element.appendChild(numberElement);
                }
                return element
            }
            getMovableStyleName(): string {
                return "left";
            }
            getMovableElements(): HTMLElement[] {
                return [this.getHtmlRootElement()];
            }
            getCorrectedValue(value): number {
                return this.initialLeft + value;
            }
            getHeight(): number {
                return this.height;
            }
        }

        export class LineDisplayControl extends PureDragDisplayControl {
            private borderWidth: number = 0;
            private rulerControlLeft: number = 0;
            private rulerControlWidth: number = 0;
            private viewElementLeft: number = 0;
            private viewElement: HTMLElement;
            rulerControlElement: HTMLElement;

            constructor(viewElement: HTMLElement, settings: any) {
                super(settings);
                this.viewElement = viewElement;
            }


            createControlHierarchy(): void {
                this.getHtmlRootElement().className = this.settings.styles.line.className;
            }
            afterInitialize(): void {
                this.borderWidth = ASPx.GetHorizontalBordersWidth(this.getHtmlRootElement());
            }

            setPosition(value: number, snapTo: SnapTo): void {
                if (snapTo == SnapTo.RightSide)
                    value = this.rulerControlLeft + this.rulerControlWidth - RULLER_NUMBER_CORRECTION - value;
                else if (snapTo == SnapTo.LeftSide)
                    value = this.rulerControlLeft + RULLER_NUMBER_CORRECTION + value - this.borderWidth;
                this.getHtmlRootElement().style.left = this.viewElementLeft + value + "px";
            }
            show(): void {
                this.rulerControlLeft = this.rulerControlElement.offsetLeft;
                this.rulerControlWidth = this.rulerControlElement.offsetWidth;
                this.viewElementLeft = this.viewElement.offsetLeft;

                var style = this.getHtmlRootElement().style;
                style.top = this.viewElement.offsetTop + "px";
                style.height = this.viewElement.clientHeight + "px";
                style.display = "block";
            }
            hide(): void {
                this.getHtmlRootElement().style.display = "";
            }
        }

        export class ColumnDragHandleControl extends PureDragDisplayControl {
            private leftElement: HTMLElement;
            private rightElement: HTMLElement;

            createControlHierarchy(): void {
                super.createControlHierarchy();
                this.leftElement = document.createElement("DIV");
                this.rightElement = document.createElement("DIV");

                this.rootElement.appendChild(this.leftElement);
                this.rootElement.appendChild(this.rightElement);
            }
            prepareControlHierarchy(): void {
                super.prepareControlHierarchy();
                this.getHtmlRootElement().className = COLUMN_HANDLE_CLASS_NAME;
                this.leftElement.className = COLUMN_LEFT_PART_HANDLE_CLASS_NAME;
                this.rightElement.className = COLUMN_RIGHT_PART_HANDLE_CLASS_NAME;
            }

            setWidth(value: number): void {
                this.getHtmlRootElement().style.left = RULLER_NUMBER_CORRECTION + value + "px";
            }
            setSpacing(value: number): void {
                this.getHtmlRootElement().style.width = value + "px";
            }

            isWidthHandled(source: HTMLElement): boolean {
                return source == this.leftElement;
            }
            isSpacingHandled(source: HTMLElement): boolean {
                return source == this.rightElement;
            }
            canHandle(source: HTMLElement): boolean {
                return source == this.getHandleElement() || this.isWidthHandled(source) || this.isSpacingHandled(source);
            }
        }
    }
} 