(function() {
    var State = {
            Initial: 'Initial',
            Collapsed: 'Collapsed',
            Adjusted: 'Adjusted'
        };
    var PivotTableWrapper = ASPx.CreateClass(null, {
        constructor: function (utils) {
            this.constructor.prototype.constructor.call(this);
            this.utils = utils;
        },
        Initialize: function(pivot, pivotTable, onPagingRequest, opts) {
            var that = this,
                elements = this.parseDomElements(pivotTable);
            that.pivot = pivot;
            that.pivotTable = pivotTable;
            that._domElements = elements;
            that.onPagingRequest = onPagingRequest;
            that.patchOpts(opts);
            that.opts = opts;

            var spacerDiv = document.createElement('div');
            spacerDiv.style.width = '0px';
            spacerDiv.style.height = '0px';
            that.pivotTable.parentNode.appendChild(spacerDiv);
            elements.spacerDiv = spacerDiv;

            ASPx.Attr.ChangeStyleAttribute(document.body, "padding-right", "");
            that.prepareElements();
            that.prepareScrollingContainers();
            that.state = State.Initial;
            that._scrollPos = {
                horz: null,
                vert: null
            };
            that._scrollableSizes = {};
        },

        CalculateSizes: function(newOpts, getScrollableSizesOnCollapse) {
            this.opts = newOpts;
            var that = this,
                horzOpts = that.opts.Horz,
                vertOpts = that.opts.Vert,
                scrollBarModes = {
                    horzMode: horzOpts.ScrollBarMode,
                    vertMode: vertOpts.ScrollBarMode
                },
                scrollingOpts = that.createConfiguration(horzOpts.Enabled, vertOpts.Enabled);

            that.toInitialState();
            var allSizes = that.measureAllSizes(scrollBarModes);
            that.prepareScrollableSizes(allSizes, getScrollableSizesOnCollapse, scrollingOpts, scrollBarModes);
        },


        MergeBeforeCollapse: function(src) {
            var that = this,
                elems = that._domElements,
                rowHeaderCell = elems.rowHeaderCells[0],
                srcElems = src._domElements,
                srcRowHeaderCell = srcElems.rowHeaderCells[0];

            ASPx.SetOffsetHeight(rowHeaderCell, srcRowHeaderCell.offsetHeight);
            ASPx.SetOffsetWidth(rowHeaderCell, srcRowHeaderCell.offsetWidth);
            that._scrollPos = src._scrollPos;
        },

        Adjust: function(opts, isPartialRender) {
            var that = this,
                horz = opts.Horz,
                vert = opts.Vert,
                elems = that._domElements,
                sizes = that._sizes,
                sizeInfos = that._sizeInfos,
                virtualWidth = sizeInfos.column.fullSize,
                virtualHeight = sizeInfos.row.fullSize,
                vertEnabled = vert.Enabled,
                horzEnabled = horz.Enabled;

            that.opts = opts;

            if(!isPartialRender)
                that.ejectPivotTable();

            if(elems.columnHeadersContainer)
                elems.columnHeadersContainer.style.display = '';
            elems.spacerDiv.style.width = '0px';


            if(!vertEnabled) that._scrollableSizes.scrollableHeight = virtualHeight;
            if(!horzEnabled) that._scrollableSizes.scrollableWidth = virtualWidth;

            that.preparePivotTable("fixed",
                {
                    rowAreaColWidths: sizes.rowHeaderWidths.length > 1 ? sizes.rowHeaderWidths : sizes.rowAreaColWidths,
                    dataWidth: that._scrollableSizes.scrollableWidth
                },
                that._sbConf);

            var columnSize = {
                    viewPort: {
                        width: that._scrollableSizes.scrollableWidth,
                        height: sizes.rowHeaderHeight
                    },
                    scrollable: {
                        width: sizes.columnCellsTotalWidth,
                        height: sizes.rowHeaderHeight
                    },
                    virtual: {
                        width: virtualWidth,
                        height: sizes.rowHeaderHeight
                    }
                },
                rowSize = {
                    viewPort: {
                        width: sizes.rowTotalWidth,
                        height: that._scrollableSizes.scrollableHeight
                    },
                    scrollable: {
                        width: sizes.rowTotalWidth,
                        height: sizes.rowCellsTotalHeight
                    },
                    virtual: {
                        width: sizes.rowTotalWidth,
                        height: virtualHeight
                    }
                },
                dataSize = {
                    viewPort: {
                        width: that._scrollableSizes.scrollableWidth,
                        height: that._scrollableSizes.scrollableHeight
                    },
                    scrollable: {
                        width: sizes.dataCellsTotalWidth,
                        height: sizes.dataCellsTotalHeight
                    },
                    virtual: {
                        width: virtualWidth,
                        height: virtualHeight
                    }
                },
                columnOpts = {
                    colWidths: sizes.dataCellWidths,
                    rowHeights: sizes.columnHeights
                },
                rowOpts = {
                    colWidths: sizes.rowAreaColWidths,
                    rowHeights: sizes.dataCellHeights
                },
                dataOpts = {
                    colWidths: sizes.dataCellWidths,
                    rowHeights: sizes.dataCellHeights
                };

            //shield cells width and height styles
            that.setPivotCellsStyles('auto');

            that.prepareScrollableContainerCell(elems.columnContainer, columnSize, columnOpts, elems.columnCellRows);
            that.prepareScrollableContainerCell(elems.rowContainer, rowSize, rowOpts, elems.rowCellRows);
            that.prepareScrollableContainerCell(elems.dataContainer, dataSize, dataOpts, elems.dataCellRows);


            that._scrollSizes = {
                column: columnSize,
                row: rowSize,
                data: dataSize
            };

            if(!isPartialRender)
                that.adjectPivotTable();

            that.prepareScrolling(horz, vert, isPartialRender);

            that.state = State.Adjusted;
        },

        ResetScrollPos: function(horz, vert) {
            var that = this,
                scrollPos = that._scrollPos;
            this._scrollPos = {
                horz: horz ? null : scrollPos.horz,
                vert: vert ? null : scrollPos.vert
            };
        },

        Merge: function(newWrapper) {
            this.inRequest = false;
            var that = this;

            // merge new pivot table content
            that._domElements.rowCellRows = newWrapper._domElements.rowCellRows;
            that._domElements.columnCellRows = newWrapper._domElements.columnCellRows;
            that._domElements.dataCellRows = newWrapper._domElements.dataCellRows;
            that._domElements.rowCellsMatrix = newWrapper._domElements.rowCellsMatrix;
            that._domElements.columnCellsMatrix = newWrapper._domElements.columnCellsMatrix;
            that._sizes = newWrapper._sizes;
            that._scrollableSizes = newWrapper._scrollableSizes;
            that._pagingOptions = newWrapper._pagingOptions;
            that._sizeInfos = newWrapper._sizeInfos;
            that._sbConf = newWrapper._sbConf;
        },

        GetCallbackAnimationElement: function() {
            var that = this,
                elems = that._domElements;
            return elems.dataContainer.rootDiv;
        },

        patchOpts: function(opts) {
            opts.Vert.ScrollBarMode = ASPx.Browser.WebKitTouchUI ? 'Hidden' : opts.Vert.ScrollBarMode;
            opts.Horz.ScrollBarMode = ASPx.Browser.WebKitTouchUI ? 'Hidden' : opts.Horz.ScrollBarMode;
        },

        prepareScrollingContainers: function() {
            var that = this,
                elems = that._domElements,
                horzOnlyConf = that.createConfiguration(true, false),
                vertOnlyConf = that.createConfiguration(false, true),
                bothConf = that.createConfiguration(true, true);

            that.prepareContainerScrolling(elems.dataContainer, bothConf, true);
            that.prepareContainerScrolling(elems.rowContainer, vertOnlyConf, false);
            that.prepareContainerScrolling(elems.columnContainer, horzOnlyConf, false);
            if(that.opts.Vert.ScrollBarMode != 'Hidden')
                that.prepareContainerScrolling(elems.vertScrollBarContainer, vertOnlyConf, false);
            if(that.opts.Horz.ScrollBarMode != 'Hidden')
                that.prepareContainerScrolling(elems.horzScrollBarContainer, horzOnlyConf, false);
        },

        prepareContainerScrolling: function(container, conf, showScrollbars) {
            var that = this;
            ASPx.Evt.AttachEventToElement(container.viewPortDiv, 'scroll', function () {
                that.onScroll(container, conf);
            });
            ASPx.Evt.AttachEventToElement(container.viewPortDiv, that.getMouseWheelEventName(), function(eventArgs) {
                that.onMouseWheel(container, conf, eventArgs);
            });
            if(ASPx.Browser.WebKitTouchUI) {
                container.viewPortDiv.className += 'dxpgTouchScrollBars' + (showScrollbars ? 'Visible' : 'Hidden');
                container.touchScroller = new ASPx.TouchUIHelper.ScrollExtender(container.viewPortDiv,
                    { forceCustomScroll: true, touchEventHandlersElement: container.viewPortDiv });
            }
            else if (ASPx.Browser.MSTouchUI)
                container.touchUIScroller = ASPx.MouseScroller.Create(
                    function () { return container.viewPortDiv; }.aspxBind(that),
                    function () { return conf.horz ? container.viewPortDiv : null; }.aspxBind(that),
                    function () { return conf.vert ? container.viewPortDiv : null; }.aspxBind(that),
                    function () { return false; },
                    true
                );
        },

        prepareElements: function() {
            var that = this,
                elems = that._domElements,

                columnContainer = elems.columnContainer,
                rowContainer = elems.rowContainer,
                dataContainer = elems.dataContainer,

                columnCellsMatrix = elems.columnCellsMatrix,
                rowCellsMatrix = elems.rowCellsMatrix,
                dataCellRows = elems.dataCellRows,

                horzSbContainer = elems.horzScrollBarContainer,
                vertSbContainer = elems.vertScrollBarContainer;

            that.pivotTable.style.display = '';

            if(columnCellsMatrix.getColumnCount() > 0 && columnCellsMatrix.getRowCount() > 0) {
                var leftTopColumnCellStyle = ASPx.GetCurrentStyle(columnCellsMatrix.getLeftTopElement()),
                    rightBottomColumnCellStyle = ASPx.GetCurrentStyle(columnCellsMatrix.getRightBottomElement());

                that.prepareValuesContainerStyle(columnContainer, leftTopColumnCellStyle, 'Left');
                that.prepareValuesContainerStyle(columnContainer, leftTopColumnCellStyle, 'Top');
                that.prepareValuesContainerStyle(columnContainer, rightBottomColumnCellStyle, 'Right');
                that.prepareValuesContainerStyle(columnContainer, rightBottomColumnCellStyle, 'Bottom');
            }

            if(rowCellsMatrix.getColumnCount() > 0 && rowCellsMatrix.getRowCount() > 0) {
                var leftTopRowCellStyle = ASPx.GetCurrentStyle(rowCellsMatrix.getLeftTopElement()),
                    rightBottomRowCellStyle = ASPx.GetCurrentStyle(rowCellsMatrix.getRightBottomElement());

                that.prepareValuesContainerStyle(rowContainer, leftTopRowCellStyle, 'Left');
                that.prepareValuesContainerStyle(rowContainer, leftTopRowCellStyle, 'Top');
                that.prepareValuesContainerStyle(rowContainer, rightBottomRowCellStyle, 'Right');
                that.prepareValuesContainerStyle(rowContainer, rightBottomRowCellStyle, 'Bottom');
            }

            if(dataCellRows.length > 0) {
            var leftTopDataCellStyle = ASPx.GetCurrentStyle(dataCellRows[0].cells[0]),
                rightBottomDataCellStyle = ASPx.GetCurrentStyle(dataCellRows[dataCellRows.length-1].cells[dataCellRows[0].cells.length-1]),

                scrollBarEdgeCellStyle = ASPx.GetCurrentStyle(elems.scrollBarEdgeCell);

                that.prepareDataContainerStyle(dataContainer, leftTopColumnCellStyle, leftTopDataCellStyle, 'Left');
                that.prepareDataContainerStyle(dataContainer, leftTopRowCellStyle, leftTopDataCellStyle, 'Top');
                that.prepareDataContainerStyle(dataContainer, rightBottomColumnCellStyle, rightBottomDataCellStyle, 'Right');
                that.prepareDataContainerStyle(dataContainer, rightBottomRowCellStyle, rightBottomDataCellStyle, 'Bottom');

                that.prepareScrollBarContainer(horzSbContainer, leftTopDataCellStyle, 'Left', true);
                that.prepareScrollBarContainer(horzSbContainer, scrollBarEdgeCellStyle, 'Top', true);
                that.prepareScrollBarContainer(horzSbContainer, rightBottomDataCellStyle, 'Right', true);
                that.prepareScrollBarContainer(horzSbContainer, scrollBarEdgeCellStyle, 'Bottom', true);

                that.prepareScrollBarContainer(vertSbContainer, scrollBarEdgeCellStyle, 'Left', false);
                that.prepareScrollBarContainer(vertSbContainer, leftTopDataCellStyle, 'Top', false);
                that.prepareScrollBarContainer(vertSbContainer, scrollBarEdgeCellStyle, 'Right', false);
                that.prepareScrollBarContainer(vertSbContainer, rightBottomDataCellStyle, 'Bottom', false);
            }

            that.setScrollBarContainerSizes(horzSbContainer, true);
            that.setScrollBarContainerSizes(vertSbContainer, false);
        },

        prepareValuesContainerStyle: function(container, style, side) {
            var that = this;
            that.mergeBorderStyle(container.decoratorDiv, style, side);
            that.mergeBorderWidth(container.viewPortDiv, style, 'margin', side, '-');
        },

        prepareDataContainerStyle: function(container, valueCellStyle, dataCellStyle, side) {
            var that = this;
            that.mergeBorderStyle(container.decoratorDiv, dataCellStyle, side);
            var styleName = "border" + side + "Style";
            if(dataCellStyle[styleName] == "none" && (side == 'Left' || side == 'Top')) {
                that.mergeBorderWidth(container.decoratorDiv, valueCellStyle, 'padding', side);
            }
            that.mergeBorderWidth(container.viewPortDiv, valueCellStyle, 'margin', side, '-');
        },

        prepareScrollBarContainer: function(container, style, side, isHorz) {
            var that = this;
            that.mergeBorderStyle(container.decoratorDiv, style, side);
            if((isHorz && (side == 'Left' || side == 'Right')) || (!isHorz && (side == 'Top' || side == 'Bottom'))) {
                that.mergeBorderWidth(container.viewPortDiv, style, 'margin', side, '-');
            }
        },

        setScrollBarContainerSizes: function(sbContainer, isHorz) {
            var sbWidth = ASPx.GetVerticalScrollBarWidth() + 1,
                side = isHorz ? "Top" : "Left",
                size = isHorz ? "height" : "width";

            sbContainer.viewPortDiv.style[size] = sbWidth + "px";
            sbContainer.scrollableDiv.style[size] = sbWidth + "px";
            sbContainer.viewPortDiv.style["margin" + side] = "-1px";
        },

        mergeBorderStyle: function(el, style, side) {
            var that = this,
                borderName = 'border' + side,
                styleName = borderName + 'Style',
                widthName = borderName + 'Width',
                colorName = borderName + 'Color',
                optsList = [
                    { same: styleName },
                    { same: widthName },
                    { same: colorName }
                ];
            that.each(optsList, function(opts) {
                that.mergeStyle(el, style, opts);
            });
        },

        mergeBorderWidth: function (el, style, property, side, prefix) {
            var that = this,
                borderWidthName = 'border' + side + 'Width',
                propertyName = property + side;
            if(style['border' + side + 'Style'] != 'none') {
                that.mergeStyle(el, style, {
                    from: borderWidthName,
                    to: propertyName,
                    prefix: prefix
                });
            }

        },

        mergeStyle: function(el, style, opts) {
            var to = opts.same || opts.to,
                from = opts.same || opts.from,
                prefix = opts.prefix || '';
            el.style[to] = prefix +  style[from];
        },

        parseDomElements: function(pivotTable) {
            var that = this,
                columnCellRows= [],
                dataCellRows = [],
                rowCellRows = [],
                rowHeaderCells = [],
                rowContainerCell = undefined,
                columnContainerCell = undefined,
                dataContainerCell = undefined,
                columnHeadersCell = undefined,
                dataHeadersCell = undefined,
                horzScrollBarRowAreaCell = undefined,
                horzScrollBarContainerCell = undefined,
                vertScrollBarColumnAreaCell = undefined,
                vertScrollBarContainerCell = undefined,
                scrollBarEdgeCell = undefined,
                columnHeadersContainer = undefined,
                columnFieldValuesRowsStarted = false;

            that.each(pivotTable.rows, function(row) {
                var columnCells = [],
                    rowCells = [],
                    dataCells = [];
                that.each(row.cells, function(cell) {
                    if(cell.className.indexOf('dxpgColumnFieldValue') !== -1)
                        columnCells.push(cell);
                    if (cell.className.indexOf('dxpgRowFieldValue') !== -1)
                        rowCells.push(cell);
                    if (cell.className.indexOf('dxpgCell') !== -1)
                        dataCells.push(cell);
                    if (cell.className.indexOf('dxpgRowArea') !== -1)
                        rowHeaderCells.push(cell);
                    if (cell.className.indexOf('dxpgColumnArea') !== -1)
                        columnHeadersCell = cell;
                    if (cell.className.indexOf('dxpgDataArea') !== -1)
                        dataHeadersCell = cell;
                    if(cell.id) {
                        if (that.endsWith(cell.id, '_RVSCell'))
                            rowContainerCell = cell;
                        if (that.endsWith(cell.id, '_CVSCell'))
                            columnContainerCell = cell;
                        if (that.endsWith(cell.id, '_DCSCell'))
                            dataContainerCell = cell;
                        if(that.endsWith(cell.id, '_HSBRACell'))
                            horzScrollBarRowAreaCell = cell;
                        if(that.endsWith(cell.id, '_HSBCCell'))
                            horzScrollBarContainerCell = cell;
                        if(that.endsWith(cell.id, '_VSBCACell'))
                            vertScrollBarColumnAreaCell = cell;
                        if(that.endsWith(cell.id, '_VSBCCell'))
                            vertScrollBarContainerCell = cell;
                        if(that.endsWith(cell.id, '_SBECell'))
                            scrollBarEdgeCell = cell;
                    }
                });
                var inColumnFieldValuesRows = columnFieldValuesRowsStarted && !rowContainerCell;
                if (columnCells.length > 0 || inColumnFieldValuesRows) {
                    columnFieldValuesRowsStarted = true;
                    columnCellRows.push(that.createCellsRow(row, columnCells));
                }
                if (rowCells.length > 0)
                    rowCellRows.push(that.createCellsRow(row, rowCells));
                if (dataCells.length > 0)
                    dataCellRows.push(that.createCellsRow(row, dataCells));
            });

            var rowCellsMatrix = that.createCellsMatrix(rowCellRows),
                columnCellsMatrix = that.createCellsMatrix(columnCellRows);

            if(columnHeadersCell)
                columnHeadersContainer = that.findChildByIdSuffix(columnHeadersCell, 'ColumnArea');


            var rowContainer = that.createContainer(rowContainerCell),
                columnContainer = that.createContainer(columnContainerCell),
                dataContainer = that.createContainer(dataContainerCell),
                horzScrollBarContainer = that.createContainer(horzScrollBarContainerCell),
                vertScrollBarContainer = that.createContainer(vertScrollBarContainerCell),
                scrollableContainers = {};
            scrollableContainers['horz'] = [dataContainer, columnContainer, horzScrollBarContainer];
            scrollableContainers['vert'] = [dataContainer, rowContainer, vertScrollBarContainer];

            return {
                columnCellRows : columnCellRows,
                rowCellRows: rowCellRows,
                dataCellRows: dataCellRows,
                rowHeaderCells: rowHeaderCells,
                columnHeadersCell: columnHeadersCell,
                dataHeadersCell: dataHeadersCell,
                columnHeadersContainer: columnHeadersContainer,
                rowContainer: rowContainer,
                columnContainer: columnContainer,
                dataContainer: dataContainer,
                horzScrollBarContainer: horzScrollBarContainer,
                vertScrollBarContainer: vertScrollBarContainer,
                horzScrollBarRowAreaCell: horzScrollBarRowAreaCell,
                scrollableContainers: scrollableContainers,
                vertScrollBarColumnAreaCell: vertScrollBarColumnAreaCell,
                scrollBarEdgeCell: scrollBarEdgeCell,
                rowCellsMatrix: rowCellsMatrix,
                columnCellsMatrix: columnCellsMatrix
            };
        },

        createCellsRow: function(row, cells) {
            return {
                row: row,
                cells: cells
            };
        },

        toInitialState: function() {
            var that = this,
                state = that.state;
            if(state != State.Adjusted && state != State.Initial) {
                throw new Error('Incorrect state');
            }
            if(state == State.Adjusted) {
                that.ejectPivotTable();
                that.adjustedToInitial();
                that.adjectPivotTable();
            }
            ASPx.Attr.ChangeStyleAttribute(document.body, "padding-right", "");
            that.state = State.Initial;
        },

        getScrollBarsConf: function(sizeInfos, scrollableSizes, scrolling, scrollBarModes) {
            var that = this,
                calculationError = that.getCalculationError(),
                horz = scrolling.horz ? sizeInfos.column.fullSize > scrollableSizes.scrollableWidth + calculationError : false,
                vert = scrolling.vert ? sizeInfos.row.fullSize > scrollableSizes.scrollableHeight + calculationError : false;

            //scrollBarModes logic
            if(scrollBarModes.horzMode == 'Visible') horz = true;
            if(scrollBarModes.vertMode == 'Visible') vert = true;

            return that.createConfiguration(horz, vert);
        },

        getCalculationError: function() {
            return 0.1;
        },

        prepareScrollableSizes: function(allSizes, getScrollableSizesOnCollapse, scrollingConf, scrollBarModes) {
            var that = this,
                elems = that._domElements,
                opts = that.opts,
                scrollableSizes = undefined,
                sizes = undefined,
                availableConfs = that.getAvailableConfigurations(scrollBarModes),
                sbConf = undefined,
                sizeInfos = undefined,
                isVerticalScrollExistsInInitialState,
                isVerticalScrollExistsInCollapsedState;

            that.each(availableConfs, function(availableConf) {
                sizes = allSizes[availableConf.toKey()];
                isVerticalScrollExistsInInitialState = ASPx.PopupUtils.IsVerticalScrollExists();
                that.collapseInitial(sizes, scrollingConf, availableConf);
                isVerticalScrollExistsInCollapsedState = ASPx.PopupUtils.IsVerticalScrollExists();
                if(isVerticalScrollExistsInInitialState && !isVerticalScrollExistsInCollapsedState) {
                    ASPx.Attr.ChangeStyleAttribute(document.body, "padding-right", ASPx.GetVerticalScrollBarWidth() + "px");
                }
                scrollableSizes = getScrollableSizesOnCollapse();
                if(scrollingConf.horz && ASPx.Browser.WebKitFamily) {//Webkit issue: 68004
                    scrollableSizes.scrollableWidth += 1;
                }
                sbConf = availableConf;
                sizeInfos = {
                    row: that.createSizeInfo(opts.Vert, sizes.rowCellsTotalHeight, elems.rowCellsMatrix.getLastLevel(false), sizes.rowAreaRowHeights),
                    column: that.createSizeInfo(opts.Horz, sizes.columnCellsTotalWidth, elems.columnCellsMatrix.getLastLevel(true), sizes.columnAreaColumnWidths)
                };
                return that.getScrollBarsConf(sizeInfos, scrollableSizes, scrollingConf, scrollBarModes).equal(availableConf);
            });
            that._sizes = sizes;
            that._sbConf = sbConf;
            that._scrollableSizes = scrollableSizes;
            that._sizeInfos = sizeInfos;
        },

        createConfiguration: function(horz, vert) {
            return {
                horz: horz,
                vert: vert,
                toKey: function() {
                    return [horz, vert];
                },
                equal: function(conf) {
                    return conf.horz == horz && conf.vert == vert;
                },
                inArray: function(array) {
                    var that = this,
                        i = array.length;
                    while (i--) {
                        if (that.equal(array[i])) {
                            return true;
                        }
                    }
                    return false;
                }
            };
        },

        getAvailableConfigurations: function(scrollBarModes) {
            var that = this,
                rules = {
                    'Hidden': [false],
                    'Visible': [true],
                    'Auto': [true, false]
                },
                horzStates = rules[scrollBarModes.horzMode],
                vertStates = rules[scrollBarModes.vertMode],
                confs = [];

            that.each(horzStates, function(horzState) {
                that.each(vertStates, function(vertState) {
                   confs.push(that.createConfiguration(horzState, vertState));
                });
            });
            return confs;
        },

        ejectPivotTable: function() {
            this.pivotTable.style.display = 'none';
        },

        adjectPivotTable: function() {
            this.pivotTable.style.display = '';
        },

        measureAllSizes: function(scrollBarsModes) {
            var that = this,
                allSizes = {};
            that.each(that.getAvailableConfigurations(scrollBarsModes), function(conf) {
                allSizes[conf.toKey()] = that.measureSizes(conf);
            });
            that.measureSizes(that.createConfiguration(false, false));
            return allSizes;
        },

        measureSizes: function(conf) {
            var that = this;
            that.preparePivotTableConf(conf);
            return that.measureElements();
        },

        preparePivotTableConf: function(conf) {
            var that = this,
                scrollBarWidth = ASPx.GetVerticalScrollBarWidth() + 1; //magic number
            that.pivotTable.style.paddingBottom = (conf.horz ? scrollBarWidth : 0) + "px";
            that.pivotTable.style.paddingRight = (conf.vert ? scrollBarWidth : 0) + "px";
        },

        measureElements: function() {
            var that = this,
                elems = that._domElements,
                dataCellWidths = [],
                dataCellHeights = [],
                columnHeights = [],
                rowAreaColWidths = [],
                rowHeaderWidths = [],
                rowHeaderHeight = 0,
                dataCellsTotalWidth = 0,
                dataCellsTotalHeight = 0,
                rowCellsTotalHeight = 0,
                columnCellsTotalWidth = 0,
                rowTotalWidth = 0,
                columnHeadersWidth = 0,
                columnHeadersHeight = 0;

            if(elems.dataCellRows[0]) {
                that.each(elems.dataCellRows[0].cells, function(cell) {
                    var width = that.getCellWidth(cell);
                    dataCellWidths.push(width);
                    dataCellsTotalWidth += width;
                });
                that.each(elems.dataCellRows, function(row) {
                    var height = that.getCellHeight(row.cells[0]);
                    dataCellHeights.push(height);
                    dataCellsTotalHeight += height;
                });
            }

            that.each(that.getRowHeights(elems.rowCellRows), function(height) {
                rowCellsTotalHeight += height;
            });

            if(elems.columnCellRows[0]) {
                that.each(elems.columnCellRows[0].cells, function(cell) {
                    var width = that.getCellWidth(cell);
                    columnCellsTotalWidth += width;
                });
            }

            columnHeights = that.getRowHeights(elems.columnCellRows);

            this.each(elems.rowHeaderCells, function(cell) {
                var width = that.getCellWidth(cell);
                rowHeaderWidths.push(width);
                rowTotalWidth += width;
            });

            rowAreaColWidths = that.measureRowAreaColWidths(elems, rowHeaderWidths);

            var rowAreaRowHeights = [];
            that.each(elems.rowCellsMatrix.getLastLevel(false), function(cell) {
                rowAreaRowHeights.push(that.getCellHeight(cell));
            });

            var columnAreaColumnWidths = [];
            that.each(elems.columnCellsMatrix.getLastLevel(true), function(cell) {
                columnAreaColumnWidths.push(that.getCellWidth(cell));
            });

            rowHeaderHeight = that.getCellHeight(elems.rowHeaderCells[0]);

            if(elems.columnHeadersCell)
                columnHeadersHeight = that.getCellHeight(elems.columnHeadersCell);

            if(elems.columnHeadersContainer) {
                columnHeadersWidth = that.utils.getRect(elems.columnHeadersContainer).width;
            }

            return {
                dataCellWidths: dataCellWidths,
                dataCellHeights: dataCellHeights,
                columnHeights: columnHeights,
                rowAreaColWidths: rowAreaColWidths,
                rowHeaderWidths: rowHeaderWidths,
                rowHeaderHeight: rowHeaderHeight,
                dataCellsTotalWidth: dataCellsTotalWidth,
                dataCellsTotalHeight: dataCellsTotalHeight,
                rowTotalWidth: rowTotalWidth,
                columnHeadersWidth: columnHeadersWidth,
                columnHeadersHeight: columnHeadersHeight,
                rowCellsTotalHeight: rowCellsTotalHeight,
                columnCellsTotalWidth: columnCellsTotalWidth,
                rowAreaRowHeights: rowAreaRowHeights,
                columnAreaColumnWidths: columnAreaColumnWidths
            };
        },

        measureRowAreaColWidths: function(elems, rowHeaderWidths) {
            var that = this,
                rowAreaColWidths = [],
                index = 0,
                cellsMatrix = elems.rowCellsMatrix,
                rowCount = cellsMatrix.getRowCount(),
                columnCount = cellsMatrix.getColumnCount();

            if(rowCount > 0 && columnCount > 0) {
                while(rowAreaColWidths.length == 0) {
                    for(var rowIndex = 0; rowIndex < rowCount; rowIndex++) {
                        var isRowWithDifferentCells = true;
                        for (var columnIndex = 1; columnIndex < (columnCount - index); columnIndex++) {
                            if (cellsMatrix.getElement(rowIndex, columnIndex) === cellsMatrix.getElement(rowIndex, columnIndex - 1)) {
                                isRowWithDifferentCells = false;
                                break;
                            }
                        }
                        if (isRowWithDifferentCells) {
                            that.each(cellsMatrix.getRow(rowIndex), function (cell) {
                                rowAreaColWidths.push(that.getCellWidth(cell) / cell.colSpan);
                            });
                            break;
                        }
                    }
                    index++;
                }
            }

            if(rowAreaColWidths.length == 0) {
                rowAreaColWidths = rowHeaderWidths.length > 0 ? rowHeaderWidths :
                    [ elems.dataHeadersCell ? that.getCellWidth(elems.dataHeadersCell) : 0 ];
            }
            return rowAreaColWidths;
        },

        createCellsMatrix: function(cellRows) {
            var that = this,
                matrix = {};

            var rowCount = 0,
                columnCount = 0,
                rowIndex = 0,
                columnIndex;
            that.each(cellRows, function (row) {
                columnIndex = 0;
                that.each(row.cells, function(cell) {
                    while(matrix[[rowIndex, columnIndex]]) {
                        columnIndex++;
                    }
                    for(var rowSpanIndex = 0; rowSpanIndex < cell.rowSpan; rowSpanIndex++) {
                        for(var colSpanIndex = 0; colSpanIndex < cell.colSpan; colSpanIndex++) {
                            matrix[[rowIndex + rowSpanIndex, columnIndex + colSpanIndex]] = cell;
                        }
                    }
                    rowCount = Math.max(rowCount, rowIndex + cell.rowSpan);
                    columnCount = Math.max(columnCount, columnIndex + cell.colSpan);
                    columnIndex = columnIndex + cell.colSpan;
                });
                rowIndex++;
            });

            function getRowCount() {
                return rowCount;
            }

            function getColumnCount() {
                return columnCount;
            }

            function getElement(rowIndex, columnIndex) {
                return matrix[[rowIndex, columnIndex]]
            }

            function getLeftTopElement() {
                return getElement(0, 0);
            }

            function getRightBottomElement() {
                return getElement(getRowCount()-1, getColumnCount()-1);
            }

            function getRow(rowIndex) {
                var cells = [];
                for(var i=0; i<getColumnCount(); i++) {
                    cells.push(getElement(rowIndex, i));
                }
                return cells;
            }

            function getColumn(columnIndex) {
                var cells = [];
                for(var i=0; i<getRowCount(); i++) {
                    cells.push(getElement(i, columnIndex));
                }
                return cells;
            }

            function getLastLevel(isHorz) {
                return isHorz ? getRow(getRowCount()-1) : getColumn(getColumnCount()-1);
            }

            return {
                getRowCount: getRowCount,
                getColumnCount: getColumnCount,
                getElement: getElement,
                getLeftTopElement: getLeftTopElement,
                getRightBottomElement: getRightBottomElement,
                getRow: getRow,
                getColumn: getColumn,
                getLastLevel: getLastLevel
            };
        },

        collapseInitial: function(sizes, sOpts, sbConf) {
            var that = this,
                elems = that._domElements,
                columnRootDiv = elems.columnContainer.rootDiv,
                rowRootDiv = elems.rowContainer.rootDiv,
                dataRootDiv = elems.dataContainer.rootDiv,
                horzScrollBarRootDiv = elems.horzScrollBarContainer.rootDiv,
                vertScrollBarRootDiv = elems.vertScrollBarContainer.rootDiv,
                vert = sOpts.vert,
                horz = sOpts.horz;

            that.createPivotTableColGroup();
            that.preparePivotTable("fixed",
                {
                    rowAreaColWidths: sizes.rowHeaderWidths.length > 1 ? sizes.rowHeaderWidths : sizes.rowAreaColWidths,
                    dataWidth: horz ? 0 : sizes.dataCellsTotalWidth
                },
                sbConf);

            // fix row headers widths
            this.each(elems.rowHeaderCells, function(cell, i) {
                that.setCellWidth(cell, sizes.rowHeaderWidths[i]);
            });

            rowRootDiv.style.width = 'auto'; // IE issue: T174031
            rowRootDiv.style.height = (vert ? 0 : sizes.dataCellsTotalHeight ) + 'px';
            columnRootDiv.style.width = (horz ? 0 : sizes.dataCellsTotalWidth) + 'px';
            columnRootDiv.style.height = sizes.rowHeaderHeight + 'px';
            dataRootDiv.style.width = (horz ? 0 : sizes.dataCellsTotalWidth) + 'px';
            dataRootDiv.style.height = (vert ? 0 : sizes.dataCellsTotalHeight) + 'px';

            horzScrollBarRootDiv.style.width = (horz ? 0 : sizes.dataCellsTotalWidth) + 'px';
            vertScrollBarRootDiv.style.height = (vert ? 0 : sizes.dataCellsTotalHeight ) + 'px';

            rowRootDiv.style.overflow = 'hidden';
            dataRootDiv.style.overflow = 'hidden';
            columnRootDiv.style.overflow = 'hidden';
            horzScrollBarRootDiv.style.overflow = 'hidden';
            vertScrollBarRootDiv.style.overflow = 'hidden';

            if(elems.columnHeadersCell)
                that.setRowHeight(elems.columnHeadersCell.parentNode, sizes.columnHeadersHeight);

            // remove cells
            that.removeCells();

            // show hidden scrollable table cells
            elems.columnContainer.cell.style.display = '';
            elems.rowContainer.cell.style.display = '';
            elems.dataContainer.cell.style.display = '';

            if(elems.columnHeadersContainer)
                elems.columnHeadersContainer.style.display = 'none';
            elems.spacerDiv.style.width = sizes.rowTotalWidth + sizes.columnHeadersWidth + 'px';
        },

        setScrollBarsVisiblity: function(sbConf) {
            var that = this,
                horz = sbConf.horz,
                vert = sbConf.vert,
                elems = that._domElements;

            elems.vertScrollBarContainer.viewPortDiv.style.height = '0px';
            elems.vertScrollBarContainer.scrollableDiv.style.height = '0px';
            elems.horzScrollBarContainer.viewPortDiv.style.width = '0px';
            elems.horzScrollBarContainer.scrollableDiv.style.width = '0px';
            elems.horzScrollBarContainer.viewPortDiv.style.display = '';
            elems.vertScrollBarContainer.viewPortDiv.style.display = '';

            elems.vertScrollBarColumnAreaCell.style.display = vert ? '' : 'none';
            elems.vertScrollBarContainer.cell.style.display = vert ? '' : 'none';
            elems.horzScrollBarRowAreaCell.style.display = horz ? '' : 'none';
            elems.horzScrollBarContainer.cell.style.display = horz ? '' : 'none';
            elems.scrollBarEdgeCell.style.display = vert && horz ? '' : 'none';


            if(elems.columnHeadersCell)
                elems.columnHeadersCell.colSpan = vert ? 2 : 1;
        },

        adjustedToInitial: function() {
            var that = this,
                elems = that._domElements;
            that.removeCells();
            that.setPivotCellsStyles('');
            that.clearScrollableContainer(elems.columnContainer);
            that.clearScrollableContainer(elems.rowContainer);
            that.clearScrollableContainer(elems.dataContainer);
            that.clearScrollBars();

            that.preparePivotTable("auto",
                {
                    rowAreaColWidths: [],
                    dataWidth: []
                },
                that.createConfiguration(false, false));

            this.each(elems.rowHeaderCells, function(cell) {
                cell.style.width = '';
            });

            if(elems.columnHeadersCell)
                elems.columnHeadersCell.parentNode.style.height = 'auto';

            if(elems.columnHeadersCell && elems.dataCellRows[0])
                elems.columnHeadersCell.colSpan = elems.dataCellRows[0].cells.length;
            that.insertCellsToPivotTable();
        },


        createSizeInfo: function(opts, contentSize, lastLevelCells, sizes) {
            var that = this,
                pagingOpts = opts.PagingOptions,
                virtualPaging = opts.VirtualPagingEnabled,
                fullSize = contentSize,
                pageSize = contentSize,
                approxRowSize = undefined,
                areas = undefined;

            if(opts.Enabled) {
                approxRowSize = contentSize / pagingOpts.RowsCount;
                if(virtualPaging)
                    fullSize = approxRowSize * pagingOpts.TotalRowsCount;
                pageSize = 0;
                var startId = that.pivot.name + '_' + pagingOpts.StartPageCellId,
                    endId = that.pivot.name + '_' + pagingOpts.EndPageCellId,
                    startIndex = that.getCellIndexById(lastLevelCells, startId),
                    endIndex = that.getCellIndexById(lastLevelCells, endId),
                    pageStartIndex = pagingOpts.PageIndex * pagingOpts.PageSize,
                    knownAreaStartIndex = pageStartIndex - startIndex,
                    beforeAreaSize = virtualPaging ? knownAreaStartIndex * approxRowSize : 0,
                    knownAreaStartOffset = beforeAreaSize,
                    knownAreaEndIndex = knownAreaStartIndex + pagingOpts.RowsCount - 1,
                    afterAreaStartOffset = knownAreaStartOffset + contentSize,
                    afterAreaSize = virtualPaging ? (pagingOpts.TotalRowsCount - (knownAreaEndIndex + 1)) * approxRowSize : 0;
                for (var i = startIndex; i <= endIndex; i++) {
                    pageSize += sizes[i];
                }
                areas = [
                    {// before
                        startOffset: 0,
                        size: beforeAreaSize,
                        itemSize: approxRowSize,
                        startIndex: 0,
                        count: knownAreaStartIndex
                    },
                    {// known
                        startOffset: knownAreaStartOffset,
                        size: contentSize,
                        startIndex: knownAreaStartIndex,
                        count: pagingOpts.RowsCount,
                        itemSizes: sizes
                    },
                    {// after
                        startOffset: afterAreaStartOffset,
                        size: afterAreaSize,
                        itemSize: approxRowSize,
                        startIndex: knownAreaEndIndex + 1,
                        count: pagingOpts.TotalRowsCount - (knownAreaEndIndex + 1)
                    }
                ];
            }
            return {
                fullSize: fullSize,
                pageSize: pageSize,
                approxRowSize: approxRowSize,
                areas: areas
            };
        },

        getScrollLocation: function(sizeInfo, offset, size) {
            var that = this;
            return {
                start: that.getAreaLocation(sizeInfo, offset),
                end: that.getAreaLocation(sizeInfo, offset + size)
            }
        },

        getScrollOffset: function(sizeInfo, scrollLocation) {
            var that = this,
                itemIndex = scrollLocation.index,
                currentArea = undefined,
                areas = sizeInfo.areas,
                lastArea = areas[areas.length - 1],
                startIndex = areas[0].startIndex,
                endIndex = lastArea.startIndex + lastArea.count - 1;
            itemIndex = itemIndex < startIndex ? startIndex : itemIndex;
            itemIndex = itemIndex > endIndex ? endIndex : itemIndex;

            that.each(areas, function(area) {
                if(that.insideInterval(itemIndex, area.startIndex, area.count - 1)) {
                    currentArea = area;
                    return true;
                }
            });

            var itemOffset = currentArea.startOffset;
            if(currentArea.itemSizes) {
                that.each(currentArea.itemSizes, function(size, index) {
                    var currentIndex = currentArea.startIndex + index;
                    if(currentIndex == itemIndex) {
                        return true;
                    }
                    itemOffset += size;
                });
            }
            else {
                var offsetInArea = (itemIndex - currentArea.startIndex) * currentArea.itemSize;
                itemOffset += offsetInArea;
            }
            return itemOffset + scrollLocation.offset;
        },

        getScrollPos: function() {
            var that = this,
                elems = that._domElements,
                viewPortDiv = elems.dataContainer.viewPortDiv,
                opts = that.opts,
                horzOpts = opts.Horz,
                vertOpts = opts.Vert,
                sizeInfos = that._sizeInfos,
                colSizeInfo = sizeInfos.column,
                rowSizeInfo = sizeInfos.row,
                scrollLeft = elems.columnContainer.viewPortDiv.scrollLeft,
                scrollTop = elems.rowContainer.viewPortDiv.scrollTop,
                viewPortWidth = viewPortDiv.offsetWidth,
                viewPortHeight = viewPortDiv.offsetHeight,
                horz = horzOpts.PagingOptions ? that.getScrollLocation(colSizeInfo, scrollLeft, viewPortWidth) : null,
                vert = vertOpts.PagingOptions ? that.getScrollLocation(rowSizeInfo, scrollTop, viewPortHeight) : null;
            return {
                horz: horz,
                vert: vert
            };
        },

        getAreaLocation: function(sizeInfo, offset) {
            var that = this,
                currentArea = undefined,
                itemIndex = -1,
                locationOffset = 0,
                fullSize = sizeInfo.fullSize;
            offset = offset <= fullSize ? offset : fullSize;
            that.each(sizeInfo.areas, function(area) {
                if(that.insideInterval(offset, area.startOffset, area.size)) {
                    currentArea = area;
                    return true;
                }
            });
            var areaOffset = currentArea.startOffset;
            if(currentArea.itemSizes) {
                var itemOffset = areaOffset;
                that.each(currentArea.itemSizes, function(size, index) {
                    if(index == currentArea.itemSizes.length - 1) { // T214500
                        itemIndex = index;
                        return true;
                    }
                    if(that.insideInterval(offset, itemOffset, size)) {
                        itemIndex = index;
                        locationOffset = offset - itemOffset;
                        return true;
                    }
                    itemOffset += size;
                });
            }
            else {
                var offsetInArea = offset - areaOffset;
                itemIndex = Math.floor(offsetInArea / currentArea.itemSize);
                locationOffset = offsetInArea - itemIndex * currentArea.itemSize;
            }

            itemIndex += currentArea.startIndex;

            return {
                index: itemIndex,
                offset: locationOffset
            }
        },

        insideInterval: function(value, start, length) {
            return value >= start && value <= start + length;
        },

        getCellIndexById: function(cells, id) {
            var that = this,
                foundIndex = -1;
            that.each(cells, function(cell, i) {
                if (cell.id === id)
                    foundIndex = i;
            });
            return foundIndex;
        },

        prepareScrollableContainerCell: function(container, scrollSize, cellSizes, cellRows) {
            var that = this,
                rootDiv = container.rootDiv,
                viewPortDiv = container.viewPortDiv,
                scrollableDiv = container.scrollableDiv,
                table = container.table;
            that.clearScrollableContainer(container);
            container.cell.style.display = '';
            viewPortDiv.style.overflow = 'hidden';
            viewPortDiv.style.height = scrollSize.viewPort.height + 'px';
            scrollableDiv.style.width = scrollSize.virtual.width + 'px';
            that.setSize(false, scrollableDiv, scrollSize.virtual.height);

            rootDiv.style.overflow = 'auto';
            rootDiv.style.width = 'auto';
            rootDiv.style.height = 'auto';
            table.style.top = '';
            table.style.left = '';
            that.prepareScrollableTable(table, scrollSize.scrollable, cellSizes, cellRows);
        },

        prepareScrollableTable: function(table, totalSizes, cellSizes, cellRows) {
            var that = this;
            if(cellSizes.colWidths.length > 0) {
                that.prepareTableColGroup(table, cellSizes.colWidths);
                table.style['table-layout'] = 'fixed';
            }
            that.each(cellRows, function (row, i) {
                var tr = table.insertRow(i);
                that.setRowHeight(tr, cellSizes.rowHeights[i]);
                that.each(row.cells, function (cell) {
                    tr.appendChild(cell);
                });
            });
            table.style.width = totalSizes.width +'px';
            table.style.height = totalSizes.height + 'px';
        },

        prepareTableColGroup: function (table, widths) {
            var colGroup = this.findChildByIdSuffix(table, '_CG');
            this.prepareColGroup(colGroup, widths);
        },

        prepareColGroup: function(colGroup, widths) {
            var that = this;
            that.each(widths, function (width) {
                var col = document.createElement('col');
                col.style.width = that.getPreparedSizeValue(width) + 'px';
                colGroup.appendChild(col);
            });
        },

        setColGroupWidths: function(colGroup, widths) {
            for(var i = colGroup.childNodes.length - 1; i>=0; i--) {
                colGroup.removeChild(colGroup.childNodes[i]);
            }
            this.prepareColGroup(colGroup, widths);
        },

        createPivotTableColGroup: function() {
            var that = this;
            if(!that.pivotTableColGroup) {
                var tableColGroup = document.createElement('colgroup');
                that.pivotTable.insertBefore(tableColGroup, that.pivotTable.children[0]);
                that.pivotTableColGroup = tableColGroup;
            }
        },

        setPivotTableWidth: function(colWidths) {
            var tableWidth = 0;
            this.each(colWidths, function(width) {
                tableWidth += width;
            });
            this.pivotTable.style.width = (tableWidth > 0) ? tableWidth + "px" : '100%';
        },

        getPivotTableColGroupWidths: function(rowAreaColWidths, dataWidth, vert) {
            var colWidths = rowAreaColWidths.concat(dataWidth);
            if(vert) colWidths = colWidths.concat(ASPx.GetVerticalScrollBarWidth() + 1);
            return colWidths;
        },

        setPivotTableWidthWithColGroup: function(rowAreaColWidths, dataWidth, vert) {
            var colWidths = this.getPivotTableColGroupWidths(rowAreaColWidths, dataWidth, vert);
            this.setColGroupWidths(this.pivotTableColGroup, colWidths);
            this.setPivotTableWidth(colWidths);
        },

        preparePivotTable: function(tableLayout, widths, sbConf) {
            this.setPivotTableWidthWithColGroup(widths.rowAreaColWidths, widths.dataWidth, sbConf.vert);
            this.pivotTable.style.tableLayout = tableLayout;
            this.setScrollBarsVisiblity(sbConf);
        },

        prepareScrolling: function(horz, vert) {
            var that = this,
                elems = that._domElements,
                scrollPos = that._scrollPos;

            if(horz.Enabled)
                that.prepareContainersScrolling(true, horz, elems.columnContainer, elems.dataContainer,
                    elems.horzScrollBarContainer, scrollPos);
            if(vert.Enabled)
                that.prepareContainersScrolling(false, vert, elems.rowContainer, elems.dataContainer,
                    elems.vertScrollBarContainer, scrollPos);
            that.requestCheckPaging();
        },

        prepareContainersScrolling: function(isHorz, opts, targetContainer, dataContainer, scrollBarContainer, scrollPos) {
            var that = this,
                dataScrollSize = that._scrollSizes.data,
                sizeInfos = that._sizeInfos,
                sizeInfo = isHorz ? sizeInfos.column: sizeInfos.row,
                areas = sizeInfo.areas,
                scrollPos = isHorz ? scrollPos.horz : scrollPos.vert,
                scrollPos = scrollPos || {
                        start: {
                            index: opts.PagingOptions.PageIndex * opts.PagingOptions.PageSize,
                            offset: 0
                        }
                    },
                scrollLocation = scrollPos.start,
                scrollOffset = that.getScrollOffset(sizeInfo, scrollLocation);

            var offsetProperty = isHorz ? 'left' : 'top',
                sizeProperty = isHorz ? 'width' : 'height',
                scrollProperty = isHorz ? 'scrollLeft' : 'scrollTop';
            targetContainer.table.style[offsetProperty] = areas[1].startOffset + 'px'; //known area
            dataContainer.table.style[offsetProperty] = areas[1].startOffset + 'px'; //known area

            that.prepareScrollBar(isHorz, opts.ScrollBarMode, dataScrollSize.viewPort[sizeProperty], dataScrollSize.virtual[sizeProperty], scrollBarContainer,
                [targetContainer, dataContainer]);
            targetContainer.viewPortDiv[scrollProperty] = scrollOffset;
            that.onScroll(targetContainer, that.createConfiguration(isHorz, !isHorz));
        },

        prepareScrollBar: function(isHorz, mode, outerSize, innerSize, scrollBar) {
            var that = this;
            that.inTimeout = false;
            if(mode) {
                scrollBar.rootDiv.style.overflow = 'auto';
                scrollBar.rootDiv.style.width = 'auto';
                scrollBar.rootDiv.style.height = 'auto';

				that.setSize(isHorz, scrollBar.viewPortDiv, outerSize);
                that.setSize(isHorz, scrollBar.scrollableDiv, innerSize);
            }
        },

        onScroll: function(container, conf) {
            var that = this,
                elems = that._domElements,
                containers = elems.scrollableContainers,
                scrollProps = that.getProps(conf),
                calculationError = that.getCalculationError();
            that.each(scrollProps, function(prop) {
                var value = container.viewPortDiv[prop.scrollProp],
                    maxValue = that.getMaxScrollValue(prop.sizeProp, container),
                    delta = value - maxValue;
                value = delta < calculationError ? value : maxValue;
                that.each(containers[prop.direction], function(cont) {
                    cont.viewPortDiv[prop.scrollProp] = value;
                });
            });
            that._scrollPos = that.getScrollPos();
            that.requestCheckPaging();
        },

        onMouseWheel: function(container, conf, eventArgs) {
            var that = this,
                scrollProps = that.getProps(conf),
                deltas = that.getMouseWheelDeltas(eventArgs),
                changed = false;
            that.each(scrollProps, function(prop) {
                var value = container.viewPortDiv[prop.scrollProp],
                    delta = deltas[prop.direction],
                    newValue = value + delta,
                    maxValue = that.getMaxScrollValue(prop.sizeProp, container);
                container.viewPortDiv[prop.scrollProp] = newValue;
                if(delta != 0 && newValue >= 0 && newValue <= maxValue)
                    changed = true;
            });
            if(!changed)
                return;
            return ASPx.Evt.PreventEvent(eventArgs);
        },

        getMouseWheelDeltas: function(e) {
            var deltas = {};
            deltas["horz"] = 0;
            deltas["vert"] = -e.wheelDelta || e.deltaY * 60; //wheelDelta in px, deltaY for FF in rowCount
            return deltas;
        },

        getMouseWheelEventName: function() {
            return document.onmousewheel !== undefined ? "mousewheel" : "wheel";
        },

        getProps: function(conf) {
            var props = [];
            if(conf.horz)
                props.push({
                        direction: 'horz',
                        scrollProp: 'scrollLeft',
                        sizeProp: 'width'
                    }
                );
            if(conf.vert)
                props.push({
                        direction: 'vert',
                        scrollProp: 'scrollTop',
                        sizeProp: 'height'
                    }
                );
            return props;
        },
        requestCheckPaging: function() {
            var that = this;
            if(!that.inTimeout) {
                window.setTimeout(function () {
                    that.inTimeout = false;
                    if(!that.inRequest) {
                        that.checkPaging();
                    }
                }, 300);
                that.inTimeout = true;
            }
        },

        checkPaging: function () {
            var that = this,
                vertPagingEnabled = that.opts.Vert.VirtualPagingEnabled,
                horzPagingEnabled = that.opts.Horz.VirtualPagingEnabled;
            if(!vertPagingEnabled && !horzPagingEnabled)
                return;
            var elems = that._domElements,
                sizeInfos = that._sizeInfos,
                scrollTop = elems.rowContainer.viewPortDiv.scrollTop,
                scrollLeft = elems.columnContainer.viewPortDiv.scrollLeft,
                viewPort = elems.dataContainer.viewPortDiv,
                viewPortHeight = that.utils.getRect(viewPort).height,
                viewPortWidth = that.utils.getRect(viewPort).width,
                vertOpts = vertPagingEnabled ? that.opts.Vert.PagingOptions : null,
                horzOpts = horzPagingEnabled ? that.opts.Horz.PagingOptions : null,
                vertPageIndex = vertOpts ? vertOpts.PageIndex : -1,
                vertPageSize = vertOpts ? vertOpts.PageSize : -1,
                horzPageIndex = horzOpts ? horzOpts.PageIndex : -1,
                horzPageSize = horzOpts ? horzOpts.PageSize : -1;

            var newVertPageIndex = vertOpts ? vertOpts.PageIndex : -1,
                newHorzPageIndex = horzOpts ? horzOpts.PageIndex : -1;

            var newHorzPageSize = horzOpts ? that.getPageSize(horzOpts, sizeInfos.column, viewPortWidth) : horzPageSize;
            var newVertPageSize = vertOpts ? that.getPageSize(vertOpts, sizeInfos.row, viewPortHeight) : vertPageSize;

            var pageSizesChanged = newHorzPageSize != horzPageSize || newVertPageSize != vertPageSize,
                pageIndicesChanged = false;

            if(!pageSizesChanged) {
                var horzScrollPos = horzOpts ? that.getScrollLocation(sizeInfos.column, scrollLeft, viewPortWidth) : null;
                var vertScrollPos = vertOpts ? that.getScrollLocation(sizeInfos.row, scrollTop, viewPortHeight) : null;

                if (vertOpts)
                    newVertPageIndex = that.getPageIndex(vertScrollPos.start.index, vertScrollPos.end.index, vertOpts);
                if (horzOpts)
                    newHorzPageIndex = that.getPageIndex(horzScrollPos.start.index, horzScrollPos.end.index, horzOpts);
                pageIndicesChanged = vertPageIndex != newVertPageIndex || horzPageIndex != newHorzPageIndex;
            }

            // send request
            if(pageSizesChanged || pageIndicesChanged) {
                that.onPagingRequest(newVertPageIndex, newVertPageSize, newHorzPageIndex, newHorzPageSize);
                that.inRequest = true;
            }
        },

        getPageSize: function(opts, sizeInfo, viewPortSize) {
            var newPageSize = opts.PageSize;
            if(opts.PageIndex != opts.PageCount - 1) {
                if(sizeInfo.pageSize < viewPortSize) {
                    var approxPageSizeRatio = 1.25;
                    newPageSize = Math.ceil((viewPortSize / sizeInfo.approxRowSize) * approxPageSizeRatio);
                    if(newPageSize < opts.PageSize) {
                        newPageSize = opts.PageSize * 2;//dichotomy
                    }
                }
            }
            return newPageSize;
        },


        getPageIndex: function(startPos, endPos, opts) {
            var pageSize = opts.PageSize,
                medianPos = (startPos + endPos) / 2,
                pageIndex = Math.floor(medianPos / pageSize),
                startPageIndex = Math.floor(startPos / pageSize),
                endPageIndex = Math.floor(endPos / pageSize);

            if(endPageIndex == opts.PageCount - 1) { // special logic for last page
                //var lastPageRemain = opts.TotalRowsCount - endPos - 1;
                //pageIndex = nextPageItemsCount > lastPageRemain ? endPageIndex : startPageIndex;
                pageIndex = endPageIndex;
            }
            return pageIndex;
        },

        getMaxScrollValue: function(sizeProp, container) {
            var that = this,
                outerRect = that.utils.getRect(container.viewPortDiv),
                innerRect = that.utils.getRect(container.scrollableDiv),
                outerValue = outerRect[sizeProp],
                innerValue = innerRect[sizeProp];
            return innerValue - outerValue;
        },

        setSize: function (isHorz, el, value) {
            if (isHorz)
                el.style.width = value + 'px';
            else {
                this.prepareVirtualScrollMarginDiv(el, value);
            }
        },

        prepareVirtualScrollMarginDiv: function (div, height) {
            if (!div) return;

            var maxPieceHeight = 1100000;
            if (height <= maxPieceHeight) {
                div.style.height = height + "px";
            } else {
                div.style.height = "";
                div.innerHTML = "";
                while (height > 0) {
                    var pieceHeight = height >= maxPieceHeight ? maxPieceHeight : height;
                    height -= maxPieceHeight;
                    var pieceDiv = document.createElement("DIV");
                    pieceDiv.style.height = pieceHeight + "px";
                    div.appendChild(pieceDiv);
                }
            }
        },

        findChildByIdSuffix: function(el, idSuffix, deep) {
            deep = !!deep;
            var that = this,
                foundChild = undefined;
            that.each(el.childNodes, function(node) {
                if(deep) {
                    foundChild = that.findChildByIdSuffix(node, idSuffix, deep);
                    if(foundChild)
                        return true;
                }
                if(node.id && that.endsWith(node.id, idSuffix)) {
                    foundChild = node;
                    return true;
                }
                return false;
            });
            return foundChild;
        },

        removeCells: function() {
            var that = this,
                elems = that._domElements;
            that.removeElements(elems.columnCellRows, false);
            that.removeElements(elems.rowCellRows, true);
            that.removeElements(elems.dataCellRows, true);
        },

        setCellStyles: function(cellRows, sizeStyle) {
            var that = this;
            that.each(cellRows, function(row){
                that.each(row.cells, function(cell) {
                    cell.style.width = sizeStyle;
                    cell.style.height = sizeStyle;
                });
            });
        },

        setPivotCellsStyles: function(sizeStyle) {
            var that = this,
                elems = that._domElements;
            that.setCellStyles(elems.columnCellRows, sizeStyle);
            that.setCellStyles(elems.rowCellRows, sizeStyle);
            that.setCellStyles(elems.dataCellRows, sizeStyle);
        },

        clearScrollableContainer: function(container) {
            var that = this,
                viewPort = container.viewPortDiv,
                scrollableDiv = container.scrollableDiv,
                table = container.table,
                tableColGroup = that.findChildByIdSuffix(table, '_CG');

            for(var i = tableColGroup.childNodes.length - 1; i>=0; i--) {
                tableColGroup.removeChild(tableColGroup.childNodes[i]);
            }
            for(var i = table.rows.length - 1; i>=0; i--) {
                table.deleteRow(i);
            }
            table.style['table-layout'] = 'auto';
            table.style.width = '';
            table.style.height = '';
            viewPort.style.width = '';
            viewPort.style.height = '';
            scrollableDiv.style.width = '';
            scrollableDiv.style.height = '';
            container.cell.style.display = 'none';
            container.cell.style.width = '';
            container.cell.style.height = '';
        },

        clearScrollBars: function() {
            var that = this,
                elems = that._domElements,
                scrollBar = elems.horzScrollBarContainer;
            scrollBar.viewPortDiv.style.width = '';
            scrollBar.viewPortDiv.style.display = 'none';
            scrollBar.scrollableDiv.style.width = '';
        },

        insertCellsToPivotTable: function () {
            var that = this,
                elems = that._domElements,
                columnBeforeCell = elems.columnContainer.cell,
                pivotTable = that.pivotTable,
                columnTableRow = columnBeforeCell.parentNode,
                rowAndDataBeforeCell = elems.rowContainer.cell,
                rowAndDataTableRow = rowAndDataBeforeCell.parentNode;
            that.each(elems.columnCellRows, function (row, i) {
                that.each(row.cells, function (cell) {
                    if (i == 0) {
                        columnTableRow.insertBefore(cell, columnBeforeCell);
                    }
                    else {
                        columnTableRow.appendChild(cell);
                    }
                });
                row.row = columnTableRow;
                columnTableRow = columnTableRow.nextSibling;
            });

            that.each(elems.rowCellRows, function (row, i) {
                var dataCellRow = elems.dataCellRows[i];
                rowAndDataTableRow = pivotTable.insertRow(rowAndDataTableRow.rowIndex + 1);
                row.row = rowAndDataTableRow;
                that.each(row.cells, function (cell) {
                    rowAndDataTableRow.appendChild(cell);
                });
                if(dataCellRow) {
                    dataCellRow.row = rowAndDataTableRow;
                    that.each(dataCellRow.cells, function (cell) {
                        rowAndDataTableRow.appendChild(cell);
                    });
                }
            });
        },

        createContainer: function(cell) {
            var that = this,
                rootDiv = that.findChildByIdSuffix(cell, '_SCRootDiv', true),
                decoratorDiv = that.findChildByIdSuffix(rootDiv, '_SCDecorDiv'),
                viewPortDiv =  that.findChildByIdSuffix(decoratorDiv, '_SCVPDiv'),
                scrollableDiv = that.findChildByIdSuffix(viewPortDiv, '_SCSDiv'),
                table = that.findChildByIdSuffix(viewPortDiv, '_SCDTable');
            return {
                cell: cell,
                rootDiv: rootDiv,
                decoratorDiv: decoratorDiv,
                viewPortDiv: viewPortDiv,
                scrollableDiv: scrollableDiv,
                table: table
            };
        },

        removeElements: function(rows, removeRows) {
            var that = this;
            that.each(rows, function (row, i) {
                that.each(row.cells, function (cell, j) {
                    var rowEl = cell.parentNode;
                    if(rowEl) {
                        if (j == 0) {
                            that.setElementStyle(rowEl, 'height', '');
                            if(rowEl.parentNode && removeRows) {
                                rowEl.parentNode.removeChild(rowEl);
                            }
                        }
                        rowEl.removeChild(cell);
                    }
                });
            });
        },

        setElementStyle: function(el, prop, value) {
            if(!ASPx.IsExists(el.style)) {
                el.cssText += prop + ': ' + value + ';';
            }
            else {
                el.style[prop] = value;
            }
        },

        //move to BaseScripts
        getPreparedSizeValue: function(sizeValue) {
            if(ASPx.Browser.WebKitFamily && sizeValue == 0) //Webkit issue: 68004
                sizeValue = 1;
            if(!ASPx.Browser.IE || ASPx.Browser.MajorVersion < 9)
                return sizeValue;
            return Math.ceil(sizeValue*100) / 100;
        },

        each: function(list, process, reverse) {
            reverse = !!reverse;
            var listSize = list.length,
                startIndex = reverse ? listSize - 1 : 0,
                increment = reverse ? -1 : 1,
                i = startIndex;
            while(i <= listSize - 1 && i>= 0) {
                if(process(list[i], i) === true)
                    break;
                i += increment;
            };
        },

        endsWith: function(str, suffix) {
            return str.indexOf(suffix, str.length - suffix.length) !== -1;
        },

        setCellWidth: function(el, width) {
            ASPx.SetOffsetWidth(el, width);
        },
        setRowHeight: function(el, height) {
            el.style.height = (height == null) ? '' : height + 'px';
        },
        getRowHeights: function(rows) {
            var that = this,
                heights = [];

            that.each(rows, function(row, index) {
                var isLastRow = index == rows.length - 1,
                    minCell = that.getMinHeightCell(row),
                    height = null;
                if(minCell) {
                    height = that.getCellHeight(minCell);
                    if (!isLastRow && minCell.rowSpan && minCell.rowSpan > 1) //T234714
                        height = null;
                }
                heights.push(height);
            });
            return heights;
        },
        getMinHeightCell: function(row) {
            var that = this,
                minCell = null,
                minHeight = undefined;
            that.each(row.cells, function (cell) {
                var height = that.getCellHeight(cell);
                if (minHeight === undefined || height < minHeight) {
                    minHeight = height;
                    minCell = cell;
                }
            });
            return minCell;
        },
        getCellWidth: function(cell) {
            return this.utils.getRect(cell).width;
        },
        getCellHeight: function(cell) {
            return this.utils.getRect(cell).height;
        }
    });
    ASPx.PivotTableWrapper = PivotTableWrapper;
})();