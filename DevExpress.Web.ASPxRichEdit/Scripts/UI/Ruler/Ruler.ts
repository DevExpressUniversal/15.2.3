module __aspxRichEdit {
    export module Ruler {
        export interface IEventsListener {
            canHandle(source: HTMLElement): boolean;
            onMouseDown(source: HTMLElement, x: number): void;
            onMouseMove(distance: number, source: HTMLElement): void;
            onMouseUp(): void;
            onDoubleClick(x: number): void;
            onEscPress(): void;
            canHandleDoubleClick(source: HTMLElement): boolean;
        }
        export interface IScrollEventListener {
            onScroll(): void;
        }
        export interface IContainer {
            addChild(child: DisplayControlBase): void;
        }

        export class RulerControlBase extends BatchUpdatableObject implements IRulerControl, ISelectionChangesListener, IContainer {
            private richEditMainElement: HTMLElement;
            private richEditViewElement: HTMLElement;

            private lineControl: LineDisplayControl;
            private rulerDisplayControl: RulerDisplayControlBase;

            private settings: RulerSettings;
            private inputPosition: InputPosition;
            private commandManager: CommandManager;
            private uiType: UnitConverter;
            private visible: boolean = true;
            private enable: boolean = true;
            private initialized: boolean = false;
            private core: RichEditCore;

            constructor(core: RichEditCore, settings: RulerSettings, richEditMainElement: HTMLElement, viewElement: HTMLElement) {
                super();
                this.settings = settings;
                this.richEditMainElement = richEditMainElement;
                this.richEditViewElement = viewElement;
                this.core = core;
                this.uiType = core.units;
            }
            initialize(testMode: boolean): void {
                if (testMode) return;
                this.inputPosition = this.core.inputPosition;
                if (!this.initialized) {
                    this.initialized = true;
                    this.commandManager = this.core.commandManager;

                    this.lineControl = this.createLineControl(this.richEditViewElement, this.settings);
                    this.addChild(this.lineControl);
                    this.lineControl.initialize();

                    this.rulerDisplayControl = this.createDisplayControl(this.lineControl, this.richEditViewElement, this.commandManager, this.settings, this.uiType.ui);
                    this.addChild(this.rulerDisplayControl);
                    this.rulerDisplayControl.initialize();

                    this.lineControl.rulerControlElement = this.rulerDisplayControl.controlElement;

                    MouseEventsManager.addListener(this.rulerDisplayControl);
                    ViewElementScrollManager.addListener(this.rulerDisplayControl, this.richEditViewElement);
                }
                this.setVisible(this.settings.visibility != RulerVisibility.Hidden);
            }

            isReadOnly(): boolean {
                return this.core.readOnly != ReadOnlyMode.None;
            }

            //ISelectionChangesListener
            NotifySelectionChanged(selection: Selection): void {
                this.update();
            }
            NotifyFocusChanged(inFocus: boolean): void {
            }

            addChild(child: DisplayControlBase): void {
                this.richEditMainElement.insertBefore(child.rootElement, this.richEditViewElement);
            }
            update(): void {
                if (this.rulerDisplayControl && !this.isUpdateLocked()) {
                    this.rulerDisplayControl.update(this.inputPosition.getMergedSectionPropertiesRaw());
                    //this.rulerDisplayControl.update(this.inputPosition.getMergedSectionPropertiesFull()); //TODO
                }
            }
            adjust(): void {
                if (this.rulerDisplayControl) //TODO
                    this.rulerDisplayControl.adjust();
            }

            createDisplayControl(lineControl: LineDisplayControl, viewElement: HTMLElement, commandManager: CommandManager, settings: any, uiType: RichEditUnit): RulerDisplayControl {
                return null
            }
            createLineControl(viewElement: HTMLElement, settings: any): LineDisplayControl {
                return null;
            }

            setEnable(enable: boolean): void {
                this.enable = enable;
                MouseEventsManager.canHandle = enable;
            }
            setVisible(visible: boolean): void {
                if (this.rulerDisplayControl && this.visible != visible) {
                    this.visible = visible;
                    this.rulerDisplayControl.setVisible(this.visible);
                    MouseEventsManager.canHandle = visible;
                    ViewElementScrollManager.canHandle = visible;
                }
            }
            getVisible(): boolean {
                return this.visible;
            }
            getHeight(): number {
                if (this.rulerDisplayControl)
                    return this.rulerDisplayControl.getDisplayHeight();
                return 0;
            }
            getWidth(): number {
                if (this.rulerDisplayControl)
                    return this.rulerDisplayControl.getDisplayWidth();
                return 0;
            }

            getTabPositions(): TabPosition[]{
                var info: { paragraphs: Paragraph[]; intervals: FixedInterval[] } = Utils.getSelectedParagraphs(this.core.selection, this.core.model.activeSubDocument);
                var tabPositions: TabPosition[] = [];

                Utils.foreach(info.paragraphs[0].getTabs().positions, tabPosition => {
                    var newTabPosition = new TabPosition();
                    newTabPosition.offset = UnitConverter.twipsToPixelsF(tabPosition.offset);
                    newTabPosition.align = tabPosition.align;
                    tabPositions.push(newTabPosition);
                });

                return tabPositions;
            }
        }

        export class HorizontalRulerControl extends RulerControlBase {
            constructor(core: RichEditCore, settings: RulerSettings, richEditMainElement: HTMLElement, viewElement: HTMLElement) {
                super(core, settings, richEditMainElement, viewElement);
            }
            createLineControl(viewElement: HTMLElement, settings: RulerSettings): LineDisplayControl {
                return new LineDisplayControl(viewElement, settings);
            }
            createDisplayControl(lineControl: LineDisplayControl, viewElement: HTMLElement, commandManager: CommandManager, settings: RulerSettings, uiType: RichEditUnit): RulerDisplayControl {
                return new RulerDisplayControl(this, lineControl, viewElement, commandManager, settings, uiType);
            }
        }
        export class VerticalRulerControl extends RulerControlBase {
            constructor(core: RichEditCore, settings: RulerSettings, richEditMainElement: HTMLElement, viewElement: HTMLElement) {
                super(core, settings, richEditMainElement, viewElement);
            }
        }

        export class RulerSettings {
            visibility: RulerVisibility;
            showLeftIndent: boolean;
            showRightIndent: boolean;
            showTabs: boolean;

            titles: RulerTitles;
            styles: RulerStyles;
        }
        export class RulerTitles {
            firstLineIdent: string;
            hangingIdent: string;
            leftIdent: string;
            rightIdent: string;
            marginLeft: string;
            marginRight: string;
            tabLeft: string;
            tabRight: string;
            tabCenter: string;
            tabDecimal: string;
        }
        export class RulerStyles {
            firstLineIdentImage: RulerSpriteInfo;
            leftIdentImage: RulerSpriteInfo;
            rightIdentImage: RulerSpriteInfo;

            firstLineIdent: RulerStyleInfo;
            leftIdent: RulerStyleInfo;
            rightIdent: RulerStyleInfo;

            tab: RulerStyleInfo;
            line: RulerStyleInfo;
            control: RulerStyleInfo;
            wrapper: RulerStyleInfo;

            tabImages: RulerTabImages;
        }
        export class RulerTabImages {
            left: RulerSpriteInfo;
            right: RulerSpriteInfo;
            center: RulerSpriteInfo;
            decimal: RulerSpriteInfo
        }
        export class RulerStyleInfo {
            className: string;
            style: string;
        }
        export class RulerSpriteInfo {
            spriteCssClass: string;
            style: string;
        }
    }
}