module __aspxRichEdit {
    export module Ruler {
        export class RulerDisplayControlBase extends DisplayControlBase implements IEventsListener, IScrollEventListener, IContainer {
            canHandleScroll: boolean = false;
            lineControl: LineDisplayControl;
            divisionsUnitHelper: DivisionsUnitHelper;
            controlElement: HTMLElement;
            commandManager: CommandManager;
            settings: RulerSettings;
            ruler: RulerControlBase;

            constructor(ruler: RulerControlBase, lineControl: LineDisplayControl, commandManager: CommandManager, settings: RulerSettings, uiUnit: RichEditUnit) {
                super();
                this.ruler = ruler;
                this.lineControl = lineControl;
                this.commandManager = commandManager;
                this.settings = settings;
                this.divisionsUnitHelper = new DivisionsUnitHelper(uiUnit);
            }

            update(props: SectionProperties): void {
            }
            adjust(): void {
            }

            getDisplayWidth(): number {
                return 0;
            }
            getDisplayHeight(): number {
                return 0;
            }
            setVisible(visible: boolean): void {
            }

            //IContainer
            addChild(child: DisplayControlBase): void {
                this.controlElement.appendChild(child.rootElement);
            }
            removeChild(child: DisplayControlBase): void {
                this.controlElement.removeChild(child.rootElement);
                child = null;
            }

            //IScrollEventListener
            onScroll(): void {
                if (this.canHandleScroll)
                    this.onScrollInternal();
            }
            onScrollInternal(): void {
            }

            //IEventsListener
            canHandle(source: HTMLElement): boolean {
                return false;
            }
            onMouseDown(source: HTMLElement, x: number): void {
            }
            onMouseMove(distance: number, source: HTMLElement): void {
            }
            onMouseUp(): void {
            }
            onDoubleClick(x: number): void {
            }
            canHandleDoubleClick(source: HTMLElement): boolean {
                return false;
            }
            onEscPress(): void {
            }
        }
        export class RulerDisplayControl extends RulerDisplayControlBase {
            private initialMarginLeftElement: number = 0;
            private positionManager: PositionManager;

            private leftMaringEnable: boolean = true;
            private rightMarginEnable: boolean = true;
            private leftIdentEnable: boolean = true;
            private rightIdentEnable: boolean = true;
            private columnsEnable: boolean = true;

            // Controls
            leftMarginDragControl: LeftMarginDragHandleControl;
            rightMarginDragControl: RightMarginDragHandleControl;
            firstLineIdentDragControl: FirstLineIdentDragHandleControl;
            rightIdentDragControl: RightIdentDragHandleControl;
            leftIdentDragControl: LeftIdentDragHandleControl;

            divisionsControl: DivisionsControl;
            tabAlignControl: TabAlignBoxControl;

            columnDragControls: ColumnDragHandleControl[] = [];
            tabDragControls: TabDragHandleControl[] = [];
            //

            viewElement: HTMLElement;

            width: number = 0;

            constructor(ruler: RulerControlBase, lineControl: LineDisplayControl, viewElement: HTMLElement, commandManager: CommandManager, settings: RulerSettings, uiUnitType: RichEditUnit) {
                super(ruler, lineControl, commandManager, settings, uiUnitType);
                this.viewElement = viewElement;
            }
            createControlHierarchy(): void {
                var maxPageWidth = UnitConverter.twipsToPixels(PaperSizeConverter.calculatePaperSize(__aspxRichEdit.PaperKind.A3).width);
                this.getHtmlRootElement().className = this.settings.styles.wrapper.className;

                this.controlElement = document.createElement("DIV");
                this.controlElement.className = this.settings.styles.control.className;

                if (ASPx.Browser.MSTouchUI)
                    this.controlElement.className += " " + ASPx.TouchUIHelper.msTouchDraggableClassName;

                this.rootElement.appendChild(this.controlElement);

                this.tabAlignControl = new TabAlignBoxControl(this.settings);
                this.rootElement.appendChild(this.tabAlignControl.rootElement);
                this.tabAlignControl.initialize();

                this.divisionsControl = new DivisionsControl(this.settings, this.divisionsUnitHelper, maxPageWidth);
                this.addChild(this.divisionsControl);
                this.divisionsControl.initialize();

                this.leftMarginDragControl = new LeftMarginDragHandleControl(this.settings, this.divisionsUnitHelper, maxPageWidth);
                this.leftMarginDragControl.initialize();
                this.addChild(this.leftMarginDragControl);

                this.rightMarginDragControl = new RightMarginDragHandleControl(this.settings, this.divisionsUnitHelper);
                this.rightMarginDragControl.initialize();
                this.addChild(this.rightMarginDragControl);

                if (this.settings.showLeftIndent) {
                    this.leftIdentDragControl = new LeftIdentDragHandleControl(this.settings, this.divisionsUnitHelper, this.divisionsControl);
                    this.addChild(this.leftIdentDragControl);
                    this.leftIdentDragControl.initialize();

                    this.firstLineIdentDragControl = new FirstLineIdentDragHandleControl(this.settings, this.divisionsUnitHelper, this.divisionsControl);
                    this.addChild(this.firstLineIdentDragControl);
                    this.firstLineIdentDragControl.initialize();
                }

                if (this.settings.showRightIndent) {
                    this.rightIdentDragControl = new RightIdentDragHandleControl(this.settings, this.divisionsUnitHelper, this.divisionsControl);
                    this.addChild(this.rightIdentDragControl);
                    this.rightIdentDragControl.initialize();
                }

                this.positionManager = new PositionManager(this);
            }
            prepareControlHierarchy(): void {
                this.controlElement.style.visibility = "visible";
                this.controlElement.style.height = this.divisionsControl.getHeight() + "px";
                if (this.settings.showLeftIndent) {
                    this.controlElement.style.paddingBottom = this.leftIdentDragControl.getHeightOfProtrudingPart() + "px",
                    this.controlElement.style.paddingTop = this.firstLineIdentDragControl.getHeightOfProtrudingPart() + "px"
                }

                var divisionOffsetTop = this.divisionsControl.getHtmlRootElement().offsetTop;
                divisionOffsetTop += this.controlElement.offsetTop;
                this.tabAlignControl.adjust(divisionOffsetTop, this.divisionsControl.getHeight());
            }

            private hideMoveCursor(): void {
                this.leftMarginDragControl.setMoveCursorVisibility(false);
                this.rightMarginDragControl.setMoveCursorVisibility(false);
            }

            canHandle(source: HTMLElement): boolean {
                return ASPx.GetIsParent(this.controlElement, source);
            }
            canHandleDoubleClick(source: HTMLElement): boolean {
                return ASPx.GetIsParent(this.divisionsControl.getHtmlRootElement(), source);
            }

            // Events
            onDoubleClick(x: number): void {
                this.getCommand(RichEditClientCommand.ShowTabsForm).execute();
            }
            onMouseDown(source: HTMLElement, x: number): void {
                var columnIndex = -1;
                var tabIndex = -1;
                var tabAction = TabAction.Move;

                var action = RulerAction.None;
                if (this.leftIdentEnable && this.firstLineIdentDragControl && this.firstLineIdentDragControl.canHandle(source))
                    action = RulerAction.FirstLineIndent;
                else if (this.leftIdentEnable && this.leftIdentDragControl && this.leftIdentDragControl.canHandleLeftIden(source))
                    action = RulerAction.LeftIdent;
                else if (this.leftIdentEnable && this.leftIdentDragControl && this.leftIdentDragControl.canHadleHangingIdent(source))
                    action = RulerAction.HangingLeftIdent;
                else if (this.rightIdentEnable && this.rightIdentDragControl && this.rightIdentDragControl.canHandle(source))
                    action = RulerAction.RightIdent;
                else if (this.leftMaringEnable && this.leftMarginDragControl.canHandle(source))
                    action = RulerAction.MarginLeft;
                else if (this.rightMarginEnable && this.rightMarginDragControl.canHandle(source))
                    action = RulerAction.MarginRight;
                if (this.columnsEnable)
                    Utils.foreach(this.columnDragControls, (column, index) => {
                        if (column.canHandle(source)) {
                            action = RulerAction.ColumntMove;
                            if (column.isSpacingHandled(source))
                                action = RulerAction.ColumnSpace;
                            else if (column.isWidthHandled(source))
                                action = RulerAction.ColumnWidth;
                            columnIndex = index;
                        }
                    });
                if (this.settings.showTabs)
                    Utils.foreach(this.tabDragControls, (tab, index) => {
                        if (tab.canHandle(source)) {
                            action = RulerAction.Tab;
                            tabIndex = index;
                        }
                    });

                if (!this.ruler.isReadOnly() && action == RulerAction.None && this.settings.showTabs) {
                    action = RulerAction.Tab;
                    tabAction = TabAction.Insert;
                    for (tabIndex = 0; tabIndex < this.tabDragControls.length; tabIndex++) {
                        if (!this.tabDragControls[tabIndex].getVisible()) {
                            this.tabDragControls[tabIndex].setVisible(true);
                            this.tabDragControls[tabIndex].changeAlign(this.tabAlignControl.align);
                            var position = x - ASPx.GetAbsolutePositionX(this.controlElement) - RULLER_NUMBER_CORRECTION;
                            this.tabDragControls[tabIndex].setPosition(position);
                            this.positionManager.appendTabInfo(position);
                            break;
                        }
                    }
                }

                this.positionManager.start(action, columnIndex, tabIndex, tabAction);
            }
            onMouseUp(): void {
                this.applyChanges();
            }
            onMouseMove(distance: number, source: HTMLElement): void {
                this.positionManager.isDeleteTab = this.positionManager.action == RulerAction.Tab && !ASPx.GetIsParent(this.getHtmlRootElement(), source);
                this.positionManager.move(distance);
            }
            onEscPress(): void {
                this.positionManager.reset();
            }
            onScrollInternal(): void {
                this.controlElement.style.left = this.initialMarginLeftElement - this.viewElement.scrollLeft + "px";
            }

            private getCommand(commandName: RichEditClientCommand): ICommand {
                return this.commandManager.getCommand(commandName);
            }
            private getCommandSate(commandName: RichEditClientCommand): any {
                var command = this.commandManager.getCommand(commandName);
                if (command)
                    return command.getState();
                return null;
            }
            private applyChanges(): void {
                var info = this.positionManager.getInfo();
                switch (info.action) {
                    case RulerAction.MarginLeft:
                        if (info.marginLeftChanged) {
                            var command = this.getCommand(__aspxRichEdit.RichEditClientCommand.RulerSectionMarginLeft);
                            if (command) this.commandManager.executeCommand(command, info.marginLeft);
                        }
                        break;
                    case RulerAction.MarginRight:
                        if (info.marginRightChanged) {
                            command = this.getCommand(__aspxRichEdit.RichEditClientCommand.RulerSectionMarginRight);
                            if (command) this.commandManager.executeCommand(command, info.marginRight);
                        }
                        break;
                    case RulerAction.FirstLineIndent:
                    case RulerAction.LeftIdent:
                    case RulerAction.HangingLeftIdent:
                        if (info.leftIndentChanged) {
                            command = this.getCommand(RichEditClientCommand.RulerParagraphLeftIndents);
                            this.commandManager.executeCommand(command, {
                                firstLine: info.firstLineIndent,
                                hanging: info.leftIndent
                            });
                        }
                        break;
                    case RulerAction.RightIdent:
                        if (info.rightIndentChanged) {
                            command = this.getCommand(RichEditClientCommand.RulerParagraphRightIndent);
                            this.commandManager.executeCommand(command, info.rightIndent);
                        }
                        break;
                    case RulerAction.Tab:
                        this.applyTabChanges(info);
                        break;
                    case RulerAction.ColumnSpace:
                    case RulerAction.ColumnWidth:
                    case RulerAction.ColumntMove:
                        command = this.getCommand(RichEditClientCommand.RulerSectionColumnsSettings);
                        this.commandManager.executeCommand(command, info.columns);
                        break;
                }
            }
            private applyTabChanges(info: PositionsInfoChanged): void {
                switch (info.tabAction) {
                    case TabAction.Delete:
                        if (info.oldTabPosition == -1) // if just created the tab and removed
                            this.initializeDragHandles();
                        else {
                            var command = this.getCommand(RichEditClientCommand.DeleteTabRuler);
                            if (command) this.commandManager.executeCommand(command, info.oldTabPosition);
                        }
                        break;
                    case TabAction.Insert:
                        var command = this.getCommand(RichEditClientCommand.InsertTabRuler);
                        if (command) this.commandManager.executeCommand(command, { position: info.newTabPosition, align: this.tabAlignControl.align });
                        break;
                    case TabAction.Move:
                        var command = this.getCommand(RichEditClientCommand.MoveTabRuler);
                        if (command) this.commandManager.executeCommand(command, { start: info.oldTabPosition, end: info.newTabPosition });
                        break;
                }
            }

            private initializeDragHandles(): void {
                var info = new PositionsInfo();
                this.tabAlignControl.setEnable(this.ruler.isReadOnly());

                var state = this.getCommandSate(RichEditClientCommand.RulerSectionMarginLeft);
                if (state) {
                    this.leftMaringEnable = state.enabled;
                    info.marginLeft = state.value;
                }

                state = this.getCommandSate(RichEditClientCommand.RulerSectionMarginRight);
                if (state) {
                    this.rightMarginEnable = state.enabled;
                    info.marginRight = state.value;
                }

                state = this.getCommandSate(RichEditClientCommand.RulerParagraphLeftIndents);
                if (state) {
                    this.leftIdentEnable = state.enabled;
                    info.leftIndent = state.value.hanging;
                    info.firstLineIndent = state.value.firstLine;
                }

                state = this.getCommandSate(RichEditClientCommand.RulerParagraphRightIndent);
                if (state) {
                    this.rightIdentEnable = state.enabled;
                    info.rightIndent = state.value.hanging;
                }

                state = <RulerSectionColumnsSettingsState>this.getCommandSate(RichEditClientCommand.RulerSectionColumnsSettings);
                if (state) {
                    this.columnsEnable = state.enabled;
                    this.initializeColumnDragHandles(state, info);
                }
                
                this.initializeTabDragHanles(info);

                this.positionManager.applyInfo(info);
            }
            private initializeTabDragHanles(info: PositionsInfo): void {
                var tabPositions = [];
                this.settings.showTabs = this.getCommandSate(RichEditClientCommand.InsertTabRuler).enabled;
                if (this.settings.showTabs)
                    tabPositions = this.ruler.getTabPositions();
                

                var difference = this.tabDragControls.length - tabPositions.length;
                if (difference > 0)
                    for (var i = 0; i < difference; i++)
                        this.tabDragControls[this.tabDragControls.length - 1 - i].setVisible(false);


                Utils.foreach(tabPositions, (tabPosition, index) => {
                    info.tabs.push(tabPosition.offset);

                    if (this.tabDragControls[index]) {
                        this.tabDragControls[index].setVisible(true);
                        this.tabDragControls[index].changeAlign(tabPosition.align);
                    }
                    else
                        this.tabDragControls.push(this.createTabDragHandle(tabPosition.align));
                });

                if (difference <= 0) //for insert new Tab
                    this.tabDragControls.push(this.createTabDragHandle(TabAlign.Left, false));
            }
            private createTabDragHandle(align: TabAlign, visible: boolean = true): TabDragHandleControl {
                var tabControl = new TabDragHandleControl(this.settings, this.divisionsUnitHelper, this.divisionsControl, align);
                this.addChild(tabControl);
                tabControl.initialize();
                tabControl.setVisible(visible);
                return tabControl;
            }

            private initializeColumnDragHandles(state: RulerSectionColumnsSettingsState, info: PositionsInfo): void {
                var i = 0;
                var columnProperties = <SectionColumnProperties[]>state.value;
                columnProperties.pop();

                info.columns = [];
                info.equalWidth = state.equalWidth;
                info.columnActiveIndex = state.activeIndex;

                var difference = this.columnDragControls.length - columnProperties.length;
                if (difference > 0) {
                    for (i = 0; i < difference; i++)
                        this.removeChild(this.columnDragControls.pop());
                }
                else if (difference < 0) {
                    difference = Math.abs(difference);
                    for (i = 0; i < difference; i++)
                        this.columnDragControls.push(this.createColumnDragHandleControl());
                }
                if (columnProperties.length > 0)
                    Utils.foreach(columnProperties, (column) => { info.columns.push(new ColumnSectionProperties(column.width, column.space)); });
            }
            private createColumnDragHandleControl(): ColumnDragHandleControl {
                var column = new ColumnDragHandleControl(this.settings);
                column.initialize();
                this.addChild(column);
                return column;
            }

            update(props: SectionProperties): void {
                this.initializeDragHandles();
                this.setWidth(__aspxRichEdit.UnitConverter.twipsToPixels(props.pageWidth));
            }
            adjust(): void {
                var viewWidth = this.viewElement.clientWidth;
                if (viewWidth > this.width)
                    this.initialMarginLeftElement = (viewWidth - this.width - RULLER_NUMBER_CORRECTION * 2) / 2;
                else {
                    var paddingLeft = ASPx.PxToInt(ASPx.GetCurrentStyle(this.viewElement).paddingLeft);
                    var pageAreaBorderWidth = (this.viewElement.scrollWidth - paddingLeft - this.width) / 2;
                    this.initialMarginLeftElement = paddingLeft + pageAreaBorderWidth - RULLER_NUMBER_CORRECTION;
                }
                this.controlElement.style.left = Math.round(this.initialMarginLeftElement) + "px";

                this.canHandleScroll = this.viewElement.scrollWidth > this.viewElement.offsetWidth;
            }
            setVisible(visible: boolean): void {
                this.getHtmlRootElement().style.display = visible ? "block" : "none";
            }
            setWidth(value: number): void {
                if (this.width != value) {
                    this.width = value;
                    this.controlElement.style.width = this.width + RULLER_NUMBER_CORRECTION * 2 + "px";
                    this.adjust();
                }
            }
            getDisplayWidth(): number {
                return this.getHtmlRootElement().offsetWidth;
            }
            getDisplayHeight(): number {
                return this.getHtmlRootElement().offsetHeight;
            }
        }
    }
}