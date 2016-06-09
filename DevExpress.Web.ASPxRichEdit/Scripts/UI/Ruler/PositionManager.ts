module __aspxRichEdit {
    export module Ruler {
        export class PositionManager {
            action: RulerAction;
            private marginLeftValue: number = 0;
            private marginRightValue: number = 0;
            private firstLineIdentValue: number = 0;
            private rightIdentValue: number = 0;
            private leftIdentValue: number = 0;
            private divisionPositionValue: number = 0;

            private prevMarginLeftValue: number = 0;
            private prevMarginRightValue: number = 0;
            private prevFirstLineIdentValue: number = 0;
            private prevRightIdentValue: number = 0;
            private prevLeftIdentValue: number = 0;
            private prevDivisionPositionValue: number = 0;

            private tabValues: TabValues[] = [];
            private dragTabIndex: number = 0;
            tabAction: TabAction = TabAction.None;
            isDeleteTab: boolean = false;

            private columnValues: ColumnValues[] = [];
            private equalWidth: boolean = true;
            private activeIndex: number = 0;
            private dragColumnIndex: number = 0;

            private linePositionValue: number = 0;
            private snapToValue: SnapTo = SnapTo.LeftSide;

            private ruler: RulerDisplayControl;

            // for columns
            private leftIndentRelativeValue: number = 0;
            private firstLineIndentRelativeValue: number = 0;
            private rightIndentRelativeValue: number = 0;

            private isRightDirection: boolean = true;
            private unitSize: number = 0;

            constructor(ruler: RulerDisplayControl) {
                this.ruler = ruler;
                this.unitSize = this.ruler.divisionsUnitHelper.getStepSize();
            }

            start(action: RulerAction, dragColumnIndex: number, dragTabIndex: number, tabAction: TabAction): void {
                if (action == RulerAction.None)
                    return;
                this.action = action;
                this.tabAction = tabAction;
                this.prevFirstLineIdentValue = this.firstLineIdentValue;
                this.prevLeftIdentValue = this.leftIdentValue;
                this.prevRightIdentValue = this.rightIdentValue;
                this.prevMarginLeftValue = this.marginLeftValue;
                this.prevMarginRightValue = this.marginRightValue;
                this.prevDivisionPositionValue = this.divisionPositionValue;

                this.dragColumnIndex = dragColumnIndex;

                Utils.foreach(this.columnValues, column => {
                    column.prevWidth = column.width;
                    column.prevSpace = column.space;
                });

                this.dragTabIndex = dragTabIndex;
                Utils.foreach(this.tabValues, tab => { tab.prevValue = tab.value; });

                switch (this.action) {
                    case RulerAction.MarginLeft:
                        this.linePositionValue = this.marginLeftValue;
                        this.snapToValue = SnapTo.LeftSide;
                        break;
                    case RulerAction.MarginRight:
                        this.linePositionValue = this.marginRightValue;
                        this.snapToValue = SnapTo.RightSide;
                        break;
                    case RulerAction.FirstLineIndent:
                        this.linePositionValue = this.firstLineIdentValue;
                        this.snapToValue = SnapTo.LeftSide;
                        this.ruler.firstLineIdentDragControl.showShadow();
                        break;
                    case RulerAction.LeftIdent:
                        this.linePositionValue = this.leftIdentValue;
                        this.snapToValue = SnapTo.LeftSide;
                        this.ruler.leftIdentDragControl.showShadow();
                        break;
                    case RulerAction.HangingLeftIdent:
                        this.linePositionValue = this.leftIdentValue;
                        this.snapToValue = SnapTo.LeftSide;
                        this.ruler.firstLineIdentDragControl.showShadow();
                        this.ruler.leftIdentDragControl.showShadow();
                        break;
                    case RulerAction.RightIdent:
                        this.linePositionValue = this.rightIdentValue;
                        this.snapToValue = SnapTo.RightSide;
                        this.ruler.rightIdentDragControl.showShadow();
                        break;
                    case RulerAction.ColumntMove:
                        if (!this.equalWidth)
                            this.linePositionValue = this.getColumnWidth() + this.getColumnSpace() / 2;
                        break;
                    case RulerAction.ColumnWidth:
                        this.snapToValue = SnapTo.LeftSide;
                        this.linePositionValue = this.getColumnWidth();
                        break;
                    case RulerAction.ColumnSpace:
                        this.snapToValue = SnapTo.LeftSide;
                        this.linePositionValue = this.getColumnWidth() + this.getColumnSpace();
                        break;
                    case RulerAction.Tab:
                        this.snapToValue = SnapTo.LeftSide;
                        this.linePositionValue = this.tabValues[this.dragTabIndex].value;
                        this.ruler.tabDragControls[this.dragTabIndex].showShadow();
                        break;
                }
                if (!this.equalWidth || this.action != RulerAction.ColumntMove)
                    this.ruler.lineControl.show();

                this.ruler.leftMarginDragControl.setMoveCursorVisibility(true);
                this.ruler.rightMarginDragControl.setMoveCursorVisibility(true);

                this.updateDisplayControls(true);
            }
            move(distance: number): void {
                this.isRightDirection = distance > -1;
                switch (this.action) {
                    case RulerAction.MarginLeft:
                        this.moveMarginLeft(distance);
                        break;
                    case RulerAction.MarginRight:
                        this.moveMarginRight(distance);
                        break;
                    case RulerAction.FirstLineIndent:
                        this.moveFirstLineIndent(distance);
                        break;
                    case RulerAction.LeftIdent:
                        this.moveLeftIdent(distance);
                        break;
                    case RulerAction.HangingLeftIdent:
                        this.moveHangingLeftIdent(distance);
                        break;
                    case RulerAction.RightIdent:
                        this.moveRightIdent(distance);
                        break;
                    case RulerAction.Tab:
                        this.moveTab(distance);
                        break;
                    case RulerAction.ColumntMove:
                        this.moveColumnMove(distance);
                        break;
                    case RulerAction.ColumnWidth:
                        this.moveColumnWidth(distance);
                        break;
                    case RulerAction.ColumnSpace:
                        this.moveColumnSpace(distance);
                        break;
                }
                this.updateDisplayControls();
            }
            reset(): void {
                switch (this.action) {
                    case RulerAction.MarginLeft:
                        this.marginLeftValue = this.prevMarginLeftValue;
                        this.divisionPositionValue = this.prevDivisionPositionValue;
                        this.leftIdentValue = this.prevLeftIdentValue;
                        this.firstLineIdentValue = this.prevFirstLineIdentValue;
                        Utils.foreach(this.tabValues, tab => { tab.value = tab.prevValue; });
                    case RulerAction.Tab:
                        Utils.foreach(this.tabValues, tab => { tab.value = tab.prevValue; });
                        break;
                    case RulerAction.MarginRight:
                        this.marginRightValue = this.prevMarginRightValue;
                        this.rightIdentValue = this.prevRightIdentValue;
                        break;
                    case RulerAction.FirstLineIndent:
                        this.firstLineIdentValue = this.prevFirstLineIdentValue;
                        break;
                    case RulerAction.LeftIdent:
                        this.leftIdentValue = this.prevLeftIdentValue;
                        break;
                    case RulerAction.HangingLeftIdent:
                        this.leftIdentValue = this.prevLeftIdentValue;
                        this.firstLineIdentValue = this.prevFirstLineIdentValue;
                        break;
                    case RulerAction.RightIdent:
                        this.marginRightValue = this.prevMarginRightValue;
                        break;
                }
                this.updateDisplayControls();
                this.hideControls();
                this.action = RulerAction.None;
            }

            applyInfo(info: PositionsInfo): void {
                this.marginLeftValue = info.marginLeft;
                this.marginRightValue = info.marginRight;

                this.equalWidth = info.equalWidth;
                this.activeIndex = info.columnActiveIndex;
                this.columnValues = [];
                if (info.columns.length > 0)
                    Utils.foreach(info.columns, (column, index) => {
                        var width = (index == 0) ? this.marginLeftValue + column.width : this.columnValues[index - 1].width + this.columnValues[index - 1].space + column.width;
                        this.columnValues.push(new ColumnValues(width, column.space));
                    });

                this.divisionPositionValue = info.marginLeft;
                this.leftIdentValue = info.marginLeft + info.leftIndent;
                this.firstLineIdentValue = info.marginLeft + info.firstLineIndent;
                this.rightIdentValue = info.marginRight + info.rightIndent;

                this.leftIndentRelativeValue = info.leftIndent;
                this.firstLineIndentRelativeValue = info.firstLineIndent;
                this.rightIndentRelativeValue = info.rightIndent;

                this.correctDragHandlesRelativeColumns();

                this.isDeleteTab = false;
                this.tabValues = [];
                if (info.tabs.length > 0)
                    Utils.foreach(info.tabs, position => { this.appendTabInfo(this.divisionPositionValue + position); });

                this.updateDisplayControls();
            }

            private isExistTab(): boolean {
                var currentTabValue = this.tabValues[this.dragTabIndex].value;
                var isExist = false;
                Utils.foreach(this.tabValues, (obj: any, index: number) => {
                    if (index != this.dragTabIndex && obj.value == currentTabValue)
                        isExist = true;
                });

                return isExist;
            }

            getInfo(): PositionsInfoChanged {
                var info = new PositionsInfoChanged();
                info.action = this.action;
                info.columns = [];

                switch (this.action) {
                    case RulerAction.MarginLeft:
                        info.marginLeftChanged = this.prevMarginLeftValue != this.marginLeftValue;
                        if (info.marginLeftChanged)
                            info.marginLeft = this.marginLeftValue;
                        break;
                    case RulerAction.MarginRight:
                        info.marginRightChanged = this.prevMarginRightValue != this.marginRightValue;
                        if (info.marginRightChanged)
                            info.marginRight = this.marginRightValue;
                        break;
                    case RulerAction.FirstLineIndent:
                    case RulerAction.LeftIdent:
                    case RulerAction.HangingLeftIdent:
                        info.leftIndentChanged = this.prevLeftIdentValue != this.leftIdentValue || this.prevFirstLineIdentValue != this.firstLineIdentValue;
                        var currentColumnValue = null;
                        if (info.leftIndentChanged) {
                            if (this.columnValues.length && this.activeIndex != 0) {
                                var startPos = this.getColumnWidth(this.activeIndex - 1) + this.getColumnSpace(this.activeIndex - 1);
                                info.firstLineIndent = this.firstLineIdentValue - startPos;
                                info.leftIndent = this.leftIdentValue - startPos;
                            }
                            else {
                                info.firstLineIndent = this.firstLineIdentValue - this.marginLeftValue;
                                info.leftIndent = this.leftIdentValue - this.marginLeftValue;
                            }
                        }
                        break;
                    case RulerAction.RightIdent:
                        info.rightIndentChanged = this.prevRightIdentValue != this.rightIdentValue;
                        if (info.rightIndentChanged)
                            info.rightIndent = this.activeIndex != this.columnValues.length ? this.getColumnWidth(this.activeIndex) - (this.ruler.width - this.rightIdentValue) : this.rightIdentValue - this.marginRightValue;
                        break;
                    case RulerAction.Tab:
                        info.newTabPosition = this.tabValues[this.dragTabIndex].value - this.divisionPositionValue;
                        info.oldTabPosition = this.tabValues[this.dragTabIndex].prevValue - this.divisionPositionValue;
                        if (this.isExistTab())
                            this.isDeleteTab = true;
                        if (this.isDeleteTab && this.tabAction == TabAction.Insert)
                            info.oldTabPosition = -1;
                        info.tabAction = this.isDeleteTab ? TabAction.Delete : this.tabAction;
                        break;
                    case RulerAction.ColumntMove:
                    case RulerAction.ColumnWidth:
                    case RulerAction.ColumnSpace:
                        var columnsAllDistance = 0;
                        Utils.foreach(this.columnValues, (column, index) => {
                            var width = (index == 0) ? column.width - this.marginLeftValue : column.width - (this.getColumnWidth(index - 1) + this.getColumnSpace(index - 1));
                            info.columns.push(new ColumnSectionProperties(width, column.space));
                            columnsAllDistance += width + column.space;
                        });
                        info.columns.push(new ColumnSectionProperties(this.ruler.width - this.marginLeftValue - this.marginRightValue - columnsAllDistance, 0));
                        break;
                }
                this.hideControls();
                this.action = RulerAction.None;

                return info;
            }
            appendTabInfo(tabPosition): void {
                this.tabValues.push(new TabValues(tabPosition));
            }

            private hideControls(): void {
                this.ruler.lineControl.hide();
                this.ruler.leftMarginDragControl.setMoveCursorVisibility(true);
                this.ruler.rightMarginDragControl.setMoveCursorVisibility(true);

                if (this.action == RulerAction.LeftIdent || this.action == RulerAction.HangingLeftIdent)
                    this.ruler.leftIdentDragControl.hideShadow();
                if (this.action == RulerAction.FirstLineIndent || this.action == RulerAction.HangingLeftIdent)
                    this.ruler.firstLineIdentDragControl.hideShadow();
                if (this.action == RulerAction.RightIdent)
                    this.ruler.rightIdentDragControl.hideShadow();
                if (this.action == RulerAction.Tab)
                    this.ruler.tabDragControls[this.dragTabIndex].hideShadow();
            }
            private moveMarginLeft(distance: number): void {
                distance = this.getCorrectedMarginLeftDistance(distance);
                this.marginLeftValue = this.prevMarginLeftValue + distance;
                this.divisionPositionValue = this.marginLeftValue;
                this.leftIdentValue = this.prevLeftIdentValue + distance;
                this.firstLineIdentValue = this.prevFirstLineIdentValue + distance;
                this.correctEqualWidthColumns();
                this.correctTabs();
                this.linePositionValue = this.marginLeftValue;
            }
            private moveMarginRight(distance: number): void {
                distance = this.getCorrectedMarginRightDistance(distance);
                this.marginRightValue = this.prevMarginRightValue - distance;
                this.rightIdentValue = this.prevRightIdentValue - distance;
                this.correctEqualWidthColumns();
                this.linePositionValue = this.marginRightValue;
            }
            private moveFirstLineIndent(distance: number): void {
                distance = this.getCorrectedFirstLineIndentDistance(distance);
                this.firstLineIdentValue = this.prevFirstLineIdentValue + distance;
                this.linePositionValue = this.firstLineIdentValue;
            }
            private moveLeftIdent(distance: number): void {
                distance = this.getCorrectedLeftIndentDistance(distance);
                this.leftIdentValue = this.prevLeftIdentValue + distance;
                this.linePositionValue = this.leftIdentValue;
            }
            private moveHangingLeftIdent(distance: number): void {
                distance = this.getCorrectedHangingLeftIdentDistance(distance);
                this.leftIdentValue = this.prevLeftIdentValue + distance;
                this.firstLineIdentValue = this.prevFirstLineIdentValue + distance;
                this.linePositionValue = this.leftIdentValue;
            }
            private moveRightIdent(distance: number): void {
                distance = this.getCorrectedRightIdentIdentDistance(distance);
                this.rightIdentValue = this.prevRightIdentValue - distance;
                this.linePositionValue = this.rightIdentValue;
            }
            private moveTab(distance: number): void {
                distance = this.getCorrectedTabDistance(distance);
                this.tabValues[this.dragTabIndex].value = this.tabValues[this.dragTabIndex].prevValue + distance;
                this.linePositionValue = this.tabValues[this.dragTabIndex].value;
            }
            private moveColumnMove(distance: number): void {
                var minColumnPosition = 0;
                var maxColumnPosition = 0;
                if (!this.equalWidth) {
                    this.columnValues[this.dragColumnIndex].width = this.getColumnPrevWidth() + distance;

                    if (this.dragColumnIndex != 0 && this.dragColumnIndex < this.columnValues.length - 1) {
                        minColumnPosition = this.getColumnWidth(this.dragColumnIndex - 1) + this.getColumnSpace(this.dragColumnIndex - 1) + MINIMUN_DISTANCE_BETWEEN_COLUMNS;
                        maxColumnPosition = this.getColumnWidth(this.dragColumnIndex + 1) - MINIMUN_DISTANCE_BETWEEN_COLUMNS;
                    }
                    else if (this.dragColumnIndex == 0) {
                        minColumnPosition = this.prevMarginLeftValue + MINIMUN_DISTANCE_BETWEEN_COLUMNS;
                        maxColumnPosition = this.columnValues.length > 1 ? this.getColumnWidth(this.dragColumnIndex + 1) : this.ruler.width - this.prevMarginRightValue;
                        maxColumnPosition -= MINIMUN_DISTANCE_BETWEEN_COLUMNS;
                    }
                    else if (this.dragColumnIndex == this.columnValues.length - 1) {
                        minColumnPosition = this.getColumnWidth(this.dragColumnIndex - 1) + this.getColumnSpace(this.dragColumnIndex - 1) + MINIMUN_DISTANCE_BETWEEN_COLUMNS;
                        maxColumnPosition = this.ruler.width - this.prevMarginRightValue - MINIMUN_DISTANCE_BETWEEN_COLUMNS;
                    }

                    if (this.columnValues[this.dragColumnIndex].width < minColumnPosition)
                        this.columnValues[this.dragColumnIndex].width = minColumnPosition;
                    else if ((this.columnValues[this.dragColumnIndex].width + this.columnValues[this.dragColumnIndex].space) > maxColumnPosition)
                        this.columnValues[this.dragColumnIndex].width = maxColumnPosition - this.getColumnSpace();

                    this.correctDragHandlesRelativeColumns();
                    this.linePositionValue = this.getColumnWidth() + this.getColumnSpace() / 2;
                }
            }
            private moveColumnWidth(distance: number): void {
                var minColumnPosition = 0;
                var maxColumnPosition = 0;

                if (this.equalWidth)
                    this.moveColumnsWithEqualWidth(distance);
                else {
                    this.columnValues[this.dragColumnIndex].width = this.getColumnPrevWidth() + distance;
                    maxColumnPosition = this.columnValues[this.dragColumnIndex].prevWidth + this.columnValues[this.dragColumnIndex].prevSpace - MINIMUN_DISTANCE_BETWEEN_COLUMNS;

                    if (this.dragColumnIndex != 0 && this.dragColumnIndex < this.columnValues.length - 1)
                        minColumnPosition = this.getColumnWidth(this.dragColumnIndex - 1) + this.getColumnSpace(this.dragColumnIndex - 1) + MINIMUN_DISTANCE_BETWEEN_COLUMNS;
                    else if (this.dragColumnIndex == 0)
                        minColumnPosition = this.prevMarginLeftValue + MINIMUN_DISTANCE_BETWEEN_COLUMNS;
                    else if (this.dragColumnIndex == this.columnValues.length - 1)
                        minColumnPosition = this.getColumnWidth(this.dragColumnIndex - 1) + this.getColumnSpace(this.dragColumnIndex - 1) + MINIMUN_DISTANCE_BETWEEN_COLUMNS;

                    if (this.columnValues[this.dragColumnIndex].width < minColumnPosition)
                        this.columnValues[this.dragColumnIndex].width = minColumnPosition;
                    else if (this.columnValues[this.dragColumnIndex].width > maxColumnPosition)
                        this.columnValues[this.dragColumnIndex].width = maxColumnPosition;

                    var prevSpacePosition = this.getColumnPrevWidth() + this.getColumnPrevSpace();
                    this.columnValues[this.dragColumnIndex].space = prevSpacePosition - this.getColumnWidth();
                }

                this.correctDragHandlesRelativeColumns();
                this.linePositionValue = this.getColumnWidth();
            }
            private moveColumnSpace(distance: number): void {
                var minColumnPosition = 0;
                var maxColumnPosition = 0;

                if (this.equalWidth)
                    this.moveColumnsWithEqualWidth(distance);
                else {
                    this.columnValues[this.dragColumnIndex].space = this.getColumnPrevSpace() + distance;
                    var spacePosition = this.getColumnWidth() + this.getColumnSpace();
                    minColumnPosition = this.getColumnWidth() + MINIMUN_DISTANCE_BETWEEN_COLUMNS;

                    if (this.dragColumnIndex != 0 && this.dragColumnIndex < this.columnValues.length - 1)
                        maxColumnPosition = this.getColumnWidth(this.dragColumnIndex + 1);
                    else if (this.dragColumnIndex == 0)
                        maxColumnPosition = this.columnValues.length > 1 ? this.getColumnWidth(this.dragColumnIndex + 1) : this.ruler.width - this.prevMarginRightValue;
                    else if (this.dragColumnIndex == this.columnValues.length - 1)
                        maxColumnPosition = this.ruler.width - this.prevMarginRightValue;
                    maxColumnPosition -= MINIMUN_DISTANCE_BETWEEN_COLUMNS;

                    if (spacePosition < minColumnPosition)
                        this.columnValues[this.dragColumnIndex].space = MINIMUN_DISTANCE_BETWEEN_COLUMNS;
                    else if (spacePosition > maxColumnPosition)
                        this.columnValues[this.dragColumnIndex].space = maxColumnPosition - this.getColumnPrevWidth();
                }

                this.correctDragHandlesRelativeColumns();
                this.linePositionValue = this.getColumnWidth() + this.getColumnSpace();
            }

            private getColumnWidth(index: number = -1): number {
                if (index == -1)
                    index = this.dragColumnIndex;
                return this.columnValues[index].width;

            }
            private getColumnSpace(index: number = -1): number {
                if (index == -1)
                    index = this.dragColumnIndex;
                return this.columnValues[index].space;
            }
            private getColumnPrevWidth(index: number = -1): number {
                if (index == -1)
                    index = this.dragColumnIndex;
                return this.columnValues[index].prevWidth;

            }
            private getColumnPrevSpace(index: number = -1): number {
                if (index == -1)
                    index = this.dragColumnIndex;
                return this.columnValues[index].prevSpace;
            }

            private correctTabs(): void {
                var difference = this.marginLeftValue - this.prevMarginLeftValue;
                Utils.foreach(this.tabValues, tab => { tab.value = tab.prevValue + difference; });
            }
            private correctEqualWidthColumns(): void {
                if (this.columnValues.length) {
                    var freeDistance = this.ruler.width - this.marginLeftValue - this.marginRightValue - this.getColumnSpace(0) * this.columnValues.length;
                    var columnWidth = freeDistance / (this.columnValues.length + 1);

                    Utils.foreach(this.columnValues, (column, index) => {
                        var width = (index == 0) ? this.marginLeftValue + columnWidth : this.columnValues[index - 1].width + this.columnValues[0].space + columnWidth;
                        column.width = width;
                    });
                }
                this.correctDragHandlesRelativeColumns();
            }
            private correctDragHandlesRelativeColumns(): void {
                if (this.columnValues.length) {
                    var currentColumnValue = null;
                    if (this.activeIndex != 0) {
                        currentColumnValue = this.columnValues[this.activeIndex - 1];
                        this.divisionPositionValue = currentColumnValue.width + currentColumnValue.space;
                        this.leftIdentValue = currentColumnValue.width + currentColumnValue.space + this.leftIndentRelativeValue;
                        this.firstLineIdentValue = currentColumnValue.width + currentColumnValue.space + this.firstLineIndentRelativeValue;
                    }
                    if (this.activeIndex != this.columnValues.length) {
                        currentColumnValue = this.columnValues[this.activeIndex];
                        this.rightIdentValue = this.ruler.width - currentColumnValue.width + this.rightIndentRelativeValue;
                    }
                }
            }

            private moveColumnsWithEqualWidth(distance: number): void {
                var width = this.getColumnPrevWidth(0) - this.marginLeftValue;
                var space = this.getColumnPrevSpace(0);

                var spaceCount = this.columnValues.length;
                var widthCount = this.columnValues.length + 1;

                var visibleWidth = this.ruler.width - this.marginLeftValue - this.marginRightValue;
                var indexDragColumn = this.dragColumnIndex + 1;
                var mousePosition = 0;

                if (this.action == RulerAction.ColumnWidth) {
                    if (this.dragColumnIndex == 0) {
                        width = width + distance;
                        space = (visibleWidth - width * widthCount) / spaceCount;
                    } else {
                        var mousePosition = indexDragColumn * (width + space) - space + distance;
                        width = (spaceCount * mousePosition - (indexDragColumn - 1) * visibleWidth) / (spaceCount - indexDragColumn + 1);
                        space = (mousePosition - width) / (indexDragColumn - 1) - width;
                    }
                }
                else {
                    mousePosition = indexDragColumn * (width + space) + distance;
                    width = visibleWidth - (mousePosition / indexDragColumn) * this.columnValues.length;
                    space = mousePosition / indexDragColumn - width;
                }

                var maxSpace = visibleWidth / spaceCount - MINIMUN_DISTANCE_BETWEEN_COLUMNS;
                if (space < MINIMUN_DISTANCE_BETWEEN_COLUMNS)
                    space = MINIMUN_DISTANCE_BETWEEN_COLUMNS;
                else if (space > maxSpace)
                    space = maxSpace;
                width = (visibleWidth - space * spaceCount) / widthCount;

                Utils.foreach(this.columnValues, (column, index) => {
                    column.space = space;
                    column.width = (index == 0) ? this.marginLeftValue + width : this.columnValues[index - 1].width + space + width;
                });
            }

            private updateDisplayControls(skipHandles: boolean = false) {
                if (!skipHandles) {
                    this.ruler.leftMarginDragControl.setPosition(this.marginLeftValue);
                    this.ruler.rightMarginDragControl.setPosition(this.marginRightValue);
                    this.ruler.divisionsControl.setPosition(this.divisionPositionValue);
                    if (this.ruler.leftIdentDragControl) {
                        this.ruler.firstLineIdentDragControl.setPosition(this.firstLineIdentValue);
                        this.ruler.leftIdentDragControl.setPosition(this.leftIdentValue);
                    }
                    if (this.ruler.rightIdentDragControl)
                        this.ruler.rightIdentDragControl.setPosition(this.rightIdentValue);

                    Utils.foreach(this.ruler.columnDragControls, (columnDragHandle, index) => {
                        columnDragHandle.setWidth(this.columnValues[index].width);
                        columnDragHandle.setSpacing(this.columnValues[index].space);
                    });

                    if (this.action == RulerAction.Tab)
                        this.ruler.tabDragControls[this.dragTabIndex].setVisible(!this.isDeleteTab);

                    Utils.foreach(this.ruler.tabDragControls, (tabDragHandle, index) => {
                        if (tabDragHandle.getVisible())
                            tabDragHandle.setPosition(this.tabValues[index].value);
                    });
                }
                if (!(this.action == RulerAction.Tab && this.isDeleteTab))
                    this.ruler.lineControl.setPosition(this.linePositionValue, this.snapToValue);
            }

            private getCalculatedDistance(distance: number, prevValue: number, startPointValue: number = 0): number {
                var currentValue = prevValue - startPointValue + distance;
                currentValue = this.getConvertedByStepValue(currentValue, distance);
                return (currentValue + startPointValue) - prevValue;
            }
            private getCalculatedRevertedDistance(distance: number, prevValue: number): number {
                var currentValue = (this.ruler.width - prevValue) - this.marginLeftValue + distance;
                currentValue = this.ruler.width - (this.getConvertedByStepValue(currentValue, distance) + this.marginLeftValue);
                distance = -(currentValue - prevValue);

                if (this.isRightDirection && (prevValue - distance) > prevValue)
                    distance = 0;
                else if (!this.isRightDirection && (prevValue - distance) < prevValue)
                    distance = 0;
                return distance;
            }
            private getConvertedByStepValue(value: number, distance: number): number {
                var count = value / this.unitSize;
                count = distance > 0 ? Math.floor(count) : Math.ceil(count);
                return count * this.unitSize;
            }

            private getCorrectedMarginLeftDistance(distance: number): number {
                distance = this.getCalculatedDistance(distance, this.prevMarginLeftValue);
                var leftSideMaxValue = Math.max(this.prevMarginLeftValue + distance, this.prevLeftIdentValue + distance, this.prevFirstLineIdentValue + distance);
                var rightSideMaxValue = Math.max(this.marginRightValue, this.rightIdentValue);

                var currentMarginLeftValue = this.prevMarginLeftValue + distance;
                if (this.isRightDirection && currentMarginLeftValue <= this.prevMarginLeftValue)
                    distance = 0;
                if (!this.isRightDirection && currentMarginLeftValue > this.prevMarginLeftValue)
                    distance = 0;

                if (this.prevMarginLeftValue + distance < 0)
                    distance = -this.prevMarginLeftValue;
                else if (this.ruler.width - leftSideMaxValue < rightSideMaxValue + MINIMUN_DISTANCE_BETWEEN_HANDLE)
                    distance -= rightSideMaxValue + leftSideMaxValue + MINIMUN_DISTANCE_BETWEEN_HANDLE - this.ruler.width;
                return distance;
            }
            private getCorrectedMarginRightDistance(distance: number): number {
                distance = this.getCalculatedRevertedDistance(distance, this.prevMarginRightValue);
                var leftSideMaxValue = Math.max(this.marginLeftValue, this.leftIdentValue, this.firstLineIdentValue);
                var rightSideMaxValue = Math.max(this.prevMarginRightValue - distance, this.prevRightIdentValue - distance);

                if (this.prevMarginRightValue - distance < 0)
                    distance = this.prevMarginRightValue;
                else if (this.ruler.width - leftSideMaxValue < rightSideMaxValue + MINIMUN_DISTANCE_BETWEEN_HANDLE)
                    distance += rightSideMaxValue + leftSideMaxValue + MINIMUN_DISTANCE_BETWEEN_HANDLE - this.ruler.width;
                return distance;
            }
            private getCorrectedRightIdentIdentDistance(distance: number): number {
                var leftSideMaxValue = Math.max(this.marginLeftValue, this.leftIdentValue, this.firstLineIdentValue);
                distance = this.getCalculatedRevertedDistance(distance, this.prevRightIdentValue);
                if (this.prevRightIdentValue - distance < 0)
                    distance = this.prevRightIdentValue;
                else if (this.ruler.width - leftSideMaxValue < this.prevRightIdentValue - distance + MINIMUN_DISTANCE_BETWEEN_HANDLE)
                    distance += this.prevRightIdentValue - distance + leftSideMaxValue + MINIMUN_DISTANCE_BETWEEN_HANDLE - this.ruler.width;
                return distance;
            }
            private getCorrectedFirstLineIndentDistance(distance: number): number {
                var rightSideMaxValue = Math.max(this.marginRightValue, this.rightIdentValue);
                distance = this.getCalculatedDistance(distance, this.prevFirstLineIdentValue, this.marginLeftValue);
                if (this.prevFirstLineIdentValue + distance < 0)
                    distance = -this.prevFirstLineIdentValue;
                else if (this.ruler.width - (this.prevFirstLineIdentValue + distance) < rightSideMaxValue + MINIMUN_DISTANCE_BETWEEN_HANDLE)
                    distance -= rightSideMaxValue + this.prevFirstLineIdentValue + distance + MINIMUN_DISTANCE_BETWEEN_HANDLE - this.ruler.width;
                return distance;
            }
            private getCorrectedLeftIndentDistance(distance: number): number {
                var rightSideMaxValue = Math.max(this.marginRightValue, this.rightIdentValue);
                distance = this.getCalculatedDistance(distance, this.prevLeftIdentValue, this.marginLeftValue);
                if (this.prevLeftIdentValue + distance < 0)
                    distance = -this.prevLeftIdentValue;
                else if (this.ruler.width - (this.prevLeftIdentValue + distance) < rightSideMaxValue + MINIMUN_DISTANCE_BETWEEN_HANDLE)
                    distance -= rightSideMaxValue + this.prevLeftIdentValue + distance + MINIMUN_DISTANCE_BETWEEN_HANDLE - this.ruler.width;
                return distance;
            }
            private getCorrectedHangingLeftIdentDistance(distance: number): number {
                distance = this.getCalculatedDistance(distance, this.prevLeftIdentValue, this.marginLeftValue);
                var leftSideMaxValue = Math.max(this.prevLeftIdentValue + distance, this.prevFirstLineIdentValue + distance);
                var rightSideMaxValue = Math.max(this.marginRightValue, this.rightIdentValue);
                var leftSideMinValue = Math.min(this.prevLeftIdentValue + distance, this.prevFirstLineIdentValue + distance);

                if (leftSideMinValue < 0)
                    distance = -Math.min(this.prevLeftIdentValue, this.prevFirstLineIdentValue);
                else if (this.ruler.width - leftSideMaxValue < rightSideMaxValue + MINIMUN_DISTANCE_BETWEEN_HANDLE)
                    distance -= rightSideMaxValue + leftSideMaxValue + MINIMUN_DISTANCE_BETWEEN_HANDLE - this.ruler.width;
                return distance;
            }
            private getCorrectedTabDistance(distance: number): number {
                var currentTabValue = this.tabValues[this.dragTabIndex].prevValue - this.marginLeftValue + distance;
                var leftSideMaxValue = Math.max(this.marginLeftValue, this.leftIdentValue, this.firstLineIdentValue);
                var rightSideMaxValue = Math.max(this.marginRightValue, this.rightIdentValue);
                distance = this.getCalculatedDistance(distance, this.tabValues[this.dragTabIndex].prevValue, this.marginLeftValue);
                currentTabValue = this.tabValues[this.dragTabIndex].prevValue + distance;

                if (this.isRightDirection && currentTabValue <= this.tabValues[this.dragTabIndex].prevValue)
                    distance = 0;
                if (!this.isRightDirection && currentTabValue > this.tabValues[this.dragTabIndex].prevValue)
                    distance = 0;

                if (currentTabValue < leftSideMaxValue)
                    distance = leftSideMaxValue - this.tabValues[this.dragTabIndex].prevValue;
                else if (currentTabValue > this.ruler.width - rightSideMaxValue)
                    distance = this.ruler.width - rightSideMaxValue - this.tabValues[this.dragTabIndex].prevValue;
                return distance;
            }
        }

        export class PositionsInfo {
            marginLeft: number;
            marginRight: number;
            leftIndent: number;
            firstLineIndent: number;
            rightIndent: number;

            tabs: number[] = [];

            columns: ColumnSectionProperties[] = [];
            columnActiveIndex: number;
            equalWidth: boolean;
        }

        export class PositionsInfoChanged extends PositionsInfo {
            oldTabPosition: number;
            newTabPosition: number;
            tabAction: TabAction;

            marginLeftChanged: boolean = false;
            marginRightChanged: boolean = false;
            leftIndentChanged: boolean = false;
            rightIndentChanged: boolean = false;

            action: RulerAction = RulerAction.None;
        }

        export class ColumnSectionProperties {
            width: number;
            space: number;
            constructor(width: number, space: number) {
                this.width = width;
                this.space = space;
            }
        }
        export class ColumnValues {
            constructor(width: number, space: number) {
                this.width = width;
                this.space = space;
            }
            width: number = 0;
            space: number = 0;
            prevWidth: number = 0;
            prevSpace: number = 0
        }
        export class TabValues {
            constructor(value: number) {
                this.value = value;
            }
            value: number = 0;
            prevValue: number = 0
        }
    }
}