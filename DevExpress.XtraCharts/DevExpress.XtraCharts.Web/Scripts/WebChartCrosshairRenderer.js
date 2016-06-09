(function() {
ASPx.crosshairVDivID = "_CHV-";
ASPx.crosshairHDivID = "_CHH-";
ASPx.crosshairLabelID = "_CHL-";
ASPx.crosshairVLabelID = "_CHVL-";
ASPx.crosshairHLabelID = "_CHHL-";
ASPx.chartToolTipID = "_CTT-";

var chartToolTipClassName = "dxchartsuiTooltip";
var chartCrosshairVLineClassName = "dxchartsuiCrosshairVLine";
var chartCrosshairHLineClassName = "dxchartsuiCrosshairHLine";
var chartCrosshairLabelClassName = "dxchartsuiCrosshairLabel";

function __aspxCrosshairValueItemsComparer(arrayElement, index, value) {
    if (arrayElement[index].value == value)
        return 0;
    else
        return arrayElement[index].value < value ? -1 : 1;
};
function __aspxSortPointInfoByArgument(p1, p2) {
    if (p1.argument < p2.argument)
        return 1;
    if (p1.argument > p2.argument)
        return -1;
    return p1.value > p2.value ? -1 : 1;
};
function __aspxSortPointInfoByValue(p1, p2) {
    if (p1.value < p2.value)
        return 1;
    if (p1.value > p2.value)
        return -1;
    return p1.argument > p2.argument ? -1 : 1;
};
function __aspxSortCrosshairElementByValue(p1, p2) {
    if (p1.Point.value < p2.Point.value)
        return 1;
    if (p1.Point.value > p2.Point.value)
        return -1;
    return p1.Point.argument > p2.Point.argument ? -1 : 1;
};
function __aspxSortCrosshairElementByArgument(p1, p2) {
    if (p1.Point.argument < p2.Point.argument)
        return 1;
    if (p1.Point.argument > p2.Point.argument)
        return -1;
    return p1.Point.value > p2.Point.value ? -1 : 1;
};
function __aspxChartGetHorizontalTable(cell1, cell2) {
    return "<table><tr><td>" + cell1 + "</td><td>" + cell2 + "</td></tr></table>";
};
function __aspxChartGetVerticalTable(row1, row2, row3) {
    return "<table><tr><td>" + row1 + "</td></tr><tr><td>" + row2 + "</td></tr><tr><td>" + row3 + "</td></tr></table>";
};
function __aspxGetToolTipX(dockPosition, dockTargetWidth, width, dockTargetX, offset) {
    if (dockPosition.indexOf("Right") > -1) {
        return dockTargetX + dockTargetWidth - width - offset;
    }
    else {
        return dockTargetX + offset;
    }
};
function __aspxGetToolTipY(dockPosition, dockTargetHeight, height, dockTargetY, offset) {
    if (dockPosition.indexOf("Bottom") > -1) {
        return dockTargetY + dockTargetHeight - height - offset;
    }
    else {
        return dockTargetY + offset;
    }
};
function __aspxSetCssClassName(object, chartControl, className) {
    if (chartControl.chart.cssPostfix != "") {
        className += "_" + chartControl.chart.cssPostfix;
    }
    object.className = className;
};
function __aspxCreateChartDiv(chartControl, prefix, index) {
    var div = document.createElement("div");
    div.onselectstart = function() { return false; };
    div.id = __aspxGetChartDivId(chartControl, prefix, index);
    div.style.textAlign = chartControl.rtl ? "right" : "left";
    chartControl.mainElement.appendChild(div);
    return div;
};
function __aspxGetChartDivId(chartControl, prefix, index) {
    return chartControl.uniqueID + prefix + index;
};
function __aspxCreateChartToolTip(chartControl, prefix, index) {
    var div = __aspxCreateChartDiv(chartControl, prefix, index);
    __aspxSetCssClassName(div, chartControl, chartToolTipClassName);
    return div;
};
function __aspxRemoveChartDiv(chartControl, prefix, index) {
    var id = __aspxGetChartDivId(chartControl, prefix, index);
    var mainElement = chartControl.GetMainDOMElement();
    var element = ASPx.GetChildById(chartControl.GetMainDOMElement(), id);
    mainElement.removeChild(element);
};
function __aspxChartSetDivPosition(div, x, y) {
    ASPx.SetAbsoluteX(div, x);
    ASPx.SetAbsoluteY(div, y);
};
function __aspxShowInFreePosition(div, chart, position, htmlElementX, htmlElementY) {
    var left;
    var top;
    if (ASPx.IsExists(position.paneID) && chart.diagram instanceof ASPxClientXYDiagram2D) {
        var pane = chart.diagram.FindPaneByID(position.paneID);
        if (ASPx.IsExists(pane)) {
            left = __aspxGetToolTipX(position.dockPosition, pane.boundsWidth, div.offsetWidth, htmlElementX + pane.boundsLeft, position.offsetX);
            top = __aspxGetToolTipY(position.dockPosition, pane.boundsHeight, div.offsetHeight, htmlElementY + pane.boundsTop, position.offsetY);
        }
    }
    else {
        left = __aspxGetToolTipX(position.dockPosition, chart.chartControl.GetWidth(), div.offsetWidth, htmlElementX, position.offsetX);
        top = __aspxGetToolTipY(position.dockPosition, chart.chartControl.GetHeight(), div.offsetHeight, htmlElementY, position.offsetY);
    }
    __aspxChartSetDivPosition(div, left, top);
}
var CrosshairElementsGroup = ASPx.CreateClass(null, {
    constructor: function(seriesPoint, snapsToArgument) {
        this.headerText="";
        this.anchorValue = snapsToArgument ? seriesPoint.argument : seriesPoint.values[0];
        this.crosshairElements = [];
        this.crosshairGroupHeaderElement = null;
    }
});
var CrosshairRenderer = ASPx.CreateClass(ASPxClientControl, {
    constructor: function (chartControl, name) {
        this.constructor.prototype.constructor.call(this, name);
        this.chart = chartControl;
        this.hDivs = [];
        this.vDivs = [];
        this.labelDivs = [];
        this.hLabelDivs = [];
        this.vLabelDivs = [];
        this.diagram = chartControl.chart.diagram;
        this.snapToNearestArgument = this.chart.chart.crosshairOptions.snapMode == "NearestArgument";
        this.isHorizontal = this.snapToNearestArgument ^ this.diagram.rotated;
        this.indent = 2;
    },
    CreateVerticalLineDiv: function (index) {
        var div = __aspxCreateChartDiv(this.chart, ASPx.crosshairVDivID, index);
        __aspxSetCssClassName(div, this.chart, chartCrosshairVLineClassName);
        this.vDivs.push(div);
        return div;
    },
    CreateHorizontalLineDiv: function (index) {
        var div = __aspxCreateChartDiv(this.chart, ASPx.crosshairHDivID, index);
        __aspxSetCssClassName(div, this.chart, chartCrosshairHLineClassName);
        this.hDivs.push(div);
        return div;
    },
    CreateVerticalLabelDiv: function (index) {
        var div = __aspxCreateChartDiv(this.chart, ASPx.crosshairVLabelID, index);
        __aspxSetCssClassName(div, this.chart, chartCrosshairLabelClassName);
        this.vLabelDivs.push(div);
        return div;
    },
    CreateHorizontalLabelDiv: function (index) {
        var div = __aspxCreateChartDiv(this.chart, ASPx.crosshairHLabelID, index);
        __aspxSetCssClassName(div, this.chart, chartCrosshairLabelClassName);
        this.hLabelDivs.push(div);
        return div;
    },
    CreateMainLabelDiv: function (index) {
        var div = __aspxCreateChartToolTip(this.chart, ASPx.crosshairLabelID, index);
        this.labelDivs.push(div);
        return div;
    },
    GetPane: function (x, y) {
        if (this.diagram != null && this.diagram instanceof ASPxClientXYDiagram2D){
            if (this.diagram.defaultPane.visible && this.diagram.defaultPane.InPane(x, y))
                return this.diagram.defaultPane;
            for (var i = 0; i < this.diagram.panes.length; i++) {
                if (this.diagram.panes[i].InPane(x, y))
                    return this.diagram.panes[i];
            }
            return null;
        }
        return null;
    },
    ClearDivArray: function (divArray, prefix, index) {
        var length = divArray.length;
        if (index < length) {
            for (var i = index; i < length; i++) {
                divArray.pop();
                __aspxRemoveChartDiv(this.chart, prefix, i);
            }
        }
    },
    Hide: function () {
        this.ClearDivs(0, 0, 0, 0, 0);
    },
    ClearDivs: function (hLineIndex, vLineIndex, hLabelIndex, vLabelIndex, labelIndex) {
        this.ClearDivArray(this.hDivs, ASPx.crosshairHDivID, hLineIndex);
        this.ClearDivArray(this.vDivs, ASPx.crosshairVDivID, vLineIndex);
        this.ClearDivArray(this.hLabelDivs, ASPx.crosshairHLabelID, hLabelIndex);
        this.ClearDivArray(this.vLabelDivs, ASPx.crosshairVLabelID, vLabelIndex);
        this.ClearDivArray(this.labelDivs, ASPx.crosshairLabelID, labelIndex);
    },
    GetVerticalDiv: function (index) {
        if (index < this.vDivs.length)
            return this.vDivs[index];
        return this.CreateVerticalLineDiv(index);
    },
    GetHorizontalDiv: function (index) {
        if (index < this.hDivs.length)
            return this.hDivs[index];
        return this.CreateHorizontalLineDiv(index);
    },
    GetVerticalLabelDiv: function (index) {
        if (index < this.vLabelDivs.length)
            return this.vLabelDivs[index];
        return this.CreateVerticalLabelDiv(index);
    },
    GetHorizontalLabelDiv: function (index) {
        if (index < this.hLabelDivs.length)
            return this.hLabelDivs[index];
        return this.CreateHorizontalLabelDiv(index);
    },
    GetLabelDiv: function (index) {
        if (index < this.labelDivs.length)
            return this.labelDivs[index];
        return this.CreateMainLabelDiv(index);
    },
    GetCrosshairSeries: function (pane) {
        var crosshairSeries = [];
        for (var i = 0; i < this.chart.chart.series.length; i++) {
            var series = this.chart.chart.series[i];
            if (series.paneID == pane.paneID && series.visible)
                crosshairSeries.push(series);
        }
        return crosshairSeries;
    },
    ClampCoord: function (coord, start, length) {
        if (coord < start)
            coord = start;
        else {
            if (coord > start + length)
                coord = start + length;
        }
        return coord;
    },
    GetAxisByID: function (id, isXAxis) {
        if (isXAxis) {
            return this.diagram.FindAxisXByID(id);
        }
        return this.diagram.FindAxisYByID(id);
    },
    GetDiff: function (value1, value2) {
        return value1 > value2 ? value1 - value2 : value2 - value1;
    },
    GetValueItemsByValue: function (series, index) {
        var items = [];
        var crosshairValueItem = series.crosshairValueItems[index];
        items.push(crosshairValueItem);
        for (var i = index - 1; i >= 0; i--) {
            if (series.crosshairValueItems[i].value == crosshairValueItem.value) {
                items.push(series.crosshairValueItems[i]);
            }
            else {
                break;
            }
        }
        for (var i = index + 1; i < series.crosshairValueItems.length; i++) {
            if (series.crosshairValueItems[i].value == crosshairValueItem.value) {
                items.push(series.crosshairValueItems[i]);
            }
            else {
                break;
            }
        }
        return items;
    },
    GetAxisValue: function (axisValues, axis) {
        for (var i = 0; i < axisValues.length; i++) {
            if (axisValues[i].axis == axis) {
                return axisValues[i].internalValue;
            }
        }
        return null;
    },
    FindNearestPoint: function (pane, series, valueItems, axisValues, primaryAxis, secondaryAxis) {
        var axisValue = this.GetAxisValue(axisValues, secondaryAxis);
        var value = null;
        var valueItem = null;
        for (var i = 0; i < valueItems.length; i++) {
            var point = series.points[valueItems[i].pointIndex];
            if (ASPx.IsExists(point.isEmpty))
                if (point.isEmpty)
                    continue;
            if (this.snapToNearestArgument) {
                for (var j = 0; j < point.values.length; j++) {
                    var pointValue = point.crosshairValues[j];
                    var isVisible = this.diagram.IsAxisValueVisible(pane, secondaryAxis, pointValue);
                    if (isVisible && (value == null || this.GetDiff(axisValue, pointValue) < this.GetDiff(axisValue, value))) {
                        value = pointValue;
                        valueItem = valueItems[i];
                    }
                }
            }
            else {
                var pointValue = secondaryAxis.GetInternalArgument(point.argument);
                var isVisible = this.diagram.IsAxisValueVisible(pane, secondaryAxis, pointValue);
                if (isVisible && (value == null || this.GetDiff(axisValue, pointValue) < this.GetDiff(axisValue, value))) {
                    value = pointValue;
                    valueItem = valueItems[i];
                }
            }
        }
        if (value == null || valueItem == null) {
            return null;
        }
        var point = series.points[valueItem.pointIndex];
        var argument = this.snapToNearestArgument ? valueItem.value : value;
        if (ASPx.IsExists(point.offset)) {
            argument += point.offset;
        }
        value = this.snapToNearestArgument ? value : valueItem.value;

        var argumentAxis = this.snapToNearestArgument ? primaryAxis : secondaryAxis;
        var valueAxis = this.snapToNearestArgument ? secondaryAxis : primaryAxis;

        var x = this.diagram.MapInternalToPoint(pane, argumentAxis, argument);
        var y = this.diagram.MapInternalToPoint(pane, valueAxis, value);
        if (ASPx.IsExists(point.fixedOffset)) {
            x += point.fixedOffset;
        }
        var ax = this.diagram.rotated ? y : x;
        var ay = this.diagram.rotated ? x : y;
        return { point: point, x: ax, y: ay, argument: argument, value: value };
    },
    AddCursorCrosshairItems: function (paneCrosshairInfo, value, isArgument, isHorizontal, axis) {
        paneCrosshairInfo.AddCursorCrosshairLine(value, isHorizontal, this.snapToNearestArgument ^ isArgument);
        paneCrosshairInfo.AddCursorCrosshairLabel(value, isHorizontal);
    },
    AddCrosshairItems: function (paneCrosshairInfo, point, value, isArgument, isHorizontal, axis) {
        point.axisValue = value;
        paneCrosshairInfo.AddPoint(point);
        paneCrosshairInfo.AddCrosshairLine(point, value, isHorizontal, this.snapToNearestArgument ^ isArgument);
        paneCrosshairInfo.AddCrosshairLabel(point, value, isHorizontal);
    },
    GetRange: function (pane, percent, maxValue) {
        var range = this.isHorizontal ? pane.boundsWidth : pane.boundsHeight;
        range *= percent;
        if (maxValue > 0 && range > maxValue)
            range = maxValue;
        return range;
    },
    AddCrosshairPointItems: function (paneCrosshairInfo, secondaryItem) {
        var value = this.isHorizontal ? secondaryItem.pointInfo.y : secondaryItem.pointInfo.x;
        var internalValue = this.snapToNearestArgument ? secondaryItem.pointInfo.value : secondaryItem.pointInfo.argument;
        this.AddCrosshairItems(paneCrosshairInfo, secondaryItem.pointInfo, new AxisValuePair(secondaryItem.secondaryAxis, internalValue, value), !this.snapToNearestArgument, this.isHorizontal, secondaryItem.secondaryAxis);
    },
    GetCrosshairValueItemIndex: function (series, axisValue) {
        var index = ASPx.Data.ArrayBinarySearch(series.crosshairValueItems, axisValue, __aspxCrosshairValueItemsComparer, 0, series.crosshairValueItems.length);
        if (index < 0) {
            index = -1 * index - 1;
            if (index >= series.crosshairValueItems.length)
                index = series.crosshairValueItems.length - 1;
            else {
                if (index > 0) {
                    var prev = this.GetDiff(series.crosshairValueItems[index - 1].value, axisValue);
                    var next = this.GetDiff(series.crosshairValueItems[index].value, axisValue);
                    if (!ASPx.IsExists(series.invertedStep)) {
                        if (prev < next)
                            index--;
                    }
                    else
                        if (!series.invertedStep && index > 0)
                            index--;
                }
            }
        }
        return index;
    },
    IsBarSeries: function (series) {
        if (series.viewType == "Bar" ||
            series.viewType == "StackedBar" ||
            series.viewType == "FullStackedBar" ||
            series.viewType == "SideBySideStackedBar" ||
            series.viewType == "SideBySideFullStackedBar" ||
            series.viewType == "SideBySideRangeBar" ||
            series.viewType == "RangeBar" ||
            series.viewType == "SideBySideGantt" ||
            series.viewType == "Gantt") {
            return true;
        }
        return false;
    },
    IsContinuousSeries: function (series) {
        if (series.viewType == "Bar" ||
            series.viewType == "Line" ||
            series.viewType == "StackedLine" ||
            series.viewType == "FullStackedLine" ||
            series.viewType == "StepLine" ||
            series.viewType == "Spline" ||
            series.viewType == "ScatterLine" ||
            series.viewType == "SwiftPlot" ||
            series.viewType == "Area" ||
            series.viewType == "StepArea" ||
            series.viewType == "SplineArea" ||
            series.viewType == "StackedArea" ||
            series.viewType == "StackedSplineArea" ||
            series.viewType == "FullStackedArea" ||
            series.viewType == "FullStackedSplineArea" ||
            series.viewType == "RangeArea") {
            return true;
        }
        return false;
    },
    GetBarRange: function (pane, series, point, smallRange) {
        var barRange = smallRange;
        if (this.snapToNearestArgument) {
            if (ASPx.IsExists(point.point.barWidth)) {
                var pointArgument = point.argument;
                var barWidth = point.point.barWidth;
                var pointValue = pointArgument;
                var axisX = this.GetAxisByID(series.axisXID, true);
                var barSidePoint = this.diagram.MapInternalToPoint(pane, axisX, pointValue + barWidth * 0.5);
                if (ASPx.IsExists(point.point.fixedOffset))
                    barSidePoint += point.point.fixedOffset;
                barRange = this.isHorizontal ? barSidePoint - point.x : barSidePoint - point.y;
                barRange = barRange > 0 ? barRange : -barRange;
                barRange = barRange < smallRange ? smallRange : barRange;
            }
        }
        return barRange;
    },
    GetContinuousSeriesRange: function (pane, series, point, crosshairPosition, smallRange, maxRange, primaryAxis) {
        var continuousRange = maxRange;
        if (point.argument == series.crosshairValueItems[0].value) {
            if (this.isHorizontal) {
                if (!primaryAxis.reverse && crosshairPosition < point.x || primaryAxis.reverse && crosshairPosition > point.x)
                    continuousRange = smallRange;
            }
            else
                if (!primaryAxis.reverse && crosshairPosition > point.y || primaryAxis.reverse && crosshairPosition < point.y)
                    continuousRange = smallRange;
        }
        else
            if (point.argument == series.crosshairValueItems[series.crosshairValueItems.length - 1].value) {
                if (this.isHorizontal) {
                    if (!primaryAxis.reverse && crosshairPosition > point.x || primaryAxis.reverse && crosshairPosition < point.x)
                        continuousRange = smallRange;
                }
                else
                    if (!primaryAxis.reverse && crosshairPosition < point.y || primaryAxis.reverse && crosshairPosition > point.y)
                        continuousRange = smallRange;

            }
        return continuousRange;
    },
    AreArgumentsEqual: function (argument1, argument2) {
        if (argument1 instanceof Date && argument2 instanceof Date) {
            return argument1.toString() == argument2.toString();
        }
        else
            return argument1 == argument2;
    },
    FindConnectedPoint: function (series, nearestPoint, pane) {
        var connectedPoint;
        var secondaryAxisID = this.snapToNearestArgument ? series.axisYID : series.axisXID;
        var secondaryAxis = this.GetAxisByID(secondaryAxisID, !this.snapToNearestArgument);
        if (this.snapToNearestArgument) {

            var pointInfo = nearestPoint.pointInfo;
            var offset = ASPx.IsExists(pointInfo.point.offset) ? pointInfo.point.offset : 0;
            var index = ASPx.Data.ArrayBinarySearch(series.crosshairValueItems, pointInfo.argument - offset, __aspxCrosshairValueItemsComparer, 0, series.crosshairValueItems.length);
            if (index < 0)
                index = -1 * index - 1;
            if (index >= series.crosshairValueItems.length)
                index = series.crosshairValueItems.length - 1;
            if (index >= 0) {
                var seriesPoint = series.points[series.crosshairValueItems[index].pointIndex];
                if (ASPx.IsExists(seriesPoint.isEmpty))
                    if (seriesPoint.isEmpty)

                        return null;
                var pointVisible = false;
                for (var j = 0; j < seriesPoint.crosshairValues.length; j++) {
                    var pointValue = seriesPoint.crosshairValues[j];
                    var isVisible = this.diagram.IsAxisValueVisible(pane, secondaryAxis, pointValue);
                    if (isVisible)
                        pointVisible = true;
                }
                if (!pointVisible)
                    return null;
                if (this.AreArgumentsEqual(pointInfo.point.argument, seriesPoint.argument))
                    connectedPoint = seriesPoint;
            }
        }
        return connectedPoint;
    },
    CreatePointInfo: function(point, pane, series, axisValues, secondaryAxis) {
        var primaryAxisID = this.snapToNearestArgument ? series.axisXID : series.axisYID;
        var primaryAxis = this.GetAxisByID(primaryAxisID, this.snapToNearestArgument);
        var primaryAxisValue = this.GetAxisValue(axisValues, primaryAxis);
        var primaryScreenAxisValue = this.diagram.MapInternalToPoint(pane, primaryAxis, primaryAxisValue);
        if (!series.actualCrosshairEnabled || series.crosshairValueItems.length == 0)
            return;
        var index = this.GetCrosshairValueItemIndex(series, primaryAxisValue);
        var valueItems = this.GetValueItemsByValue(series, index);

        var axisValue = this.GetAxisValue(axisValues, secondaryAxis);
        var value = null;
        var valueItem = null;
        for (var i = 0; i < valueItems.length; i++) {
            if (point == series.points[valueItems[i].pointIndex]) {
                if (this.snapToNearestArgument) {
                    for (var j = 0; j < point.values.length; j++) {
                        var pointValue = point.crosshairValues[j];
                        var isVisible = this.diagram.IsAxisValueVisible(pane, secondaryAxis, pointValue);
                        if (isVisible) {
                            value = pointValue;
                            valueItem = valueItems[i];
                        }
                    }
                }
                else {
                    value = secondaryAxis.GetInternalArgument(point.argument);
                    valueItem = valueItems[i];
                }
            }
        }
        if (value == null || valueItem == null) {
            return null;
        }
        var point = series.points[valueItem.pointIndex];
        var argument = this.snapToNearestArgument ? valueItem.value : value;
        if (ASPx.IsExists(point.offset)) {
            argument += point.offset;
        }
        value = this.snapToNearestArgument ? value : valueItem.value;
        var x = this.diagram.MapInternalToPoint(pane, this.snapToNearestArgument ? primaryAxis : secondaryAxis, argument);
        var y = this.diagram.MapInternalToPoint(pane, this.snapToNearestArgument ? secondaryAxis : primaryAxis, value);
        if (ASPx.IsExists(point.fixedOffset)) {
            x += point.fixedOffset;
        }
        var ax = this.diagram.rotated ? y : x;
        var ay = this.diagram.rotated ? x : y;
        return { point: point, x: ax, y: ay, argument: argument, value: value };
    },
    AddSortedCrosshairPointItems: function (paneCrosshairInfo, pane, x, y, crosshairSeries, secondaryItems) {
        for (var i = 0; i < crosshairSeries.length; i++) {
            var series = crosshairSeries[i];
            for (var j = 0; j < secondaryItems.length; j++) {
                if (secondaryItems[j].series == series) {
                    this.AddCrosshairPointItems(paneCrosshairInfo, secondaryItems[j]);
                    break;
                }
            }
        }
        return paneCrosshairInfo;
    },
    FindConnectedPointItems: function (nearestPoint, crosshairSeries, pane, axisValues) {
        var connectedItems = [];
        for (var i = 0; i < crosshairSeries.length; i++) {
            var series = crosshairSeries[i];
            if (series != nearestPoint.series) {
                var seriesConnectedPoint = this.FindConnectedPoint(series, nearestPoint, pane);
                if (seriesConnectedPoint != null) {
                    var secondaryAxisID = this.snapToNearestArgument ? series.axisYID : series.axisXID;
                    var secondaryAxis = this.GetAxisByID(secondaryAxisID, !this.snapToNearestArgument);

                    var pointInfo = this.CreatePointInfo(seriesConnectedPoint, pane, series, axisValues, secondaryAxis);
                    if (pointInfo != null)
                        connectedItems.push({ pointInfo: pointInfo, secondaryAxis: secondaryAxis, series: series });
                }
            }
        }
        return connectedItems;
    },
    CalculatePaneCrosshairInfo: function (pane, x, y) {
        x = this.ClampCoord(x, pane.boundsLeft, pane.boundsWidth);
        y = this.ClampCoord(y, pane.boundsTop, pane.boundsHeight);
        var paneCrosshairInfo = new PaneCrosshairInfo(pane, x, y);
        var crosshairSeries = this.GetCrosshairSeries(pane);
        var axisValues = this.diagram.MapPointToInternal(pane, x, y);
        var smallRange = this.GetRange(pane, 0.08, 20);
        var secondaryItems = [];
        var nearestPoint = null;
        var minDiff = -1;
        var primaryAxisID = this.snapToNearestArgument ? pane.primaryAxisXID : pane.primaryAxisYID;
        var primaryAxis = this.GetAxisByID(primaryAxisID, this.snapToNearestArgument);
        var primaryAxisValue = this.GetAxisValue(axisValues, primaryAxis);
        var primaryScreenAxisValue = this.diagram.MapInternalToPoint(pane, primaryAxis, primaryAxisValue);
        this.AddCursorCrosshairItems(paneCrosshairInfo, new AxisValuePair(primaryAxis, primaryAxisValue, primaryScreenAxisValue), this.snapToNearestArgument, !this.isHorizontal, primaryAxis);
        for (var i = 0; i < crosshairSeries.length; i++) {
            var series = crosshairSeries[i];
            if (!series.actualCrosshairEnabled || series.crosshairValueItems.length == 0)
                continue;
            var index = this.GetCrosshairValueItemIndex(series, primaryAxisValue);
            var valueItems = this.GetValueItemsByValue(series, index);
            var secondaryAxisID = this.snapToNearestArgument ? series.axisYID : series.axisXID;
            var secondaryAxis = this.GetAxisByID(secondaryAxisID, !this.snapToNearestArgument);
            var pointInfo = this.FindNearestPoint(pane, series, valueItems, axisValues, primaryAxis, secondaryAxis);
            if (pointInfo != null && pane.InPane(pointInfo.x, pointInfo.y)) {
                var diff = this.GetDiff(this.isHorizontal ? x : y, this.isHorizontal ? pointInfo.x : pointInfo.y);
                var correctedRange = smallRange;
                if (this.IsBarSeries(series)) {
                    correctedRange = this.GetBarRange(pane, series, pointInfo, smallRange);
                }
                else {
                    if (this.IsContinuousSeries(series)) {
                        correctedRange = this.GetContinuousSeriesRange(pane, series, pointInfo, this.isHorizontal ? x : y, correctedRange, this.isHorizontal ? pane.boundsWidth : pane.boundsHeight, primaryAxis);
                    }
                }
                if (diff <= correctedRange) {
                    if (this.chart.chart.crosshairOptions.crosshairLabelMode != "ShowForNearestSeries" && (series.argumentScaleType == "Qualitative" || this.IsBarSeries(series))) {
                        if (minDiff == -1 || (minDiff != -1 && diff < minDiff)) {
                            minDiff = diff;
                            nearestPoint = { pointInfo: pointInfo, secondaryAxis: secondaryAxis, series: series };
                        }
                    }
                    else
                        if (this.chart.chart.crosshairOptions.crosshairLabelMode == "ShowForNearestSeries" && secondaryItems.length > 0) {
                            var pointDiff = this.CalcSquareLength(x, y, pointInfo.x, pointInfo.y);
                            for (var secondaryItemIndex = 0; secondaryItemIndex < secondaryItems.length; secondaryItemIndex++) {
                                var secondaryItem = secondaryItems[secondaryItemIndex];
                                var itemDiff = this.CalcSquareLength(x, y, secondaryItem.pointInfo.x, secondaryItem.pointInfo.y);
                                if (pointDiff < itemDiff) {
                                    secondaryItems = [];
                                    secondaryItems.push({ pointInfo: pointInfo, secondaryAxis: secondaryAxis, series: series });
                                }
                            }
                        }
                        else
                            secondaryItems.push({ pointInfo: pointInfo, secondaryAxis: secondaryAxis, series: series });
                }
            }
        }
        if (nearestPoint != null) {
            secondaryItems.push(nearestPoint);
            var connectedPoints = this.FindConnectedPointItems(nearestPoint, crosshairSeries, pane, axisValues);
            for (var i = 0; i < connectedPoints.length; i++)
                secondaryItems.push(connectedPoints[i]);
        }
        paneCrosshairInfo = this.AddSortedCrosshairPointItems(paneCrosshairInfo, pane, x, y, crosshairSeries, secondaryItems);
        return paneCrosshairInfo;
    },
    AddUniqueValue: function (uniqueValues, value) {
        for (var i = 0; i < uniqueValues.length; i++) {
            if (uniqueValues[i] == value)
                return false;
        }
        uniqueValues.push(value);
        return true;
    },
    FindAxisLabelBounds: function(pane, axis) {
        if (ASPx.IsExists(pane.axisLabelBounds)) {
            for (var i = 0; i < pane.axisLabelBounds.length; i++) {
                if (pane.axisLabelBounds[i].GetAxis() == axis)
                    return pane.axisLabelBounds[i];
            }
        }
        return null;
    },
    GetLineLabelHtml: function (axisValuePair, isValueAxis) {
        var value = axisValuePair.axis.GetNativeArgument(axisValuePair.internalValue);
        return ASPx.ToolTipPatternHelper.GetAxisLabelText(axisValuePair.axis, isValueAxis, value);
    },
    GetLineThickness: function (horizontal) {
        horizontal = horizontal ^ this.diagram.rotated;
        if (horizontal) {
            lineStyle = this.chart.chart.crosshairOptions.valueLineStyle;
        }
        else {
            lineStyle = this.chart.chart.crosshairOptions.argumentLineStyle;
        }
        if (lineStyle != null) {
            return lineStyle.thickness;
        }
        else {
            return 0;
        }
    },
    DrawCrosshairLineElements: function(crosshairDrawInfo) {
        var chartImage = this.chart.GetImageElement();
        var chartX = ASPx.GetAbsoluteX(chartImage);
        var chartY = ASPx.GetAbsoluteY(chartImage);
        for (var i = 0; i < crosshairDrawInfo.CrosshairElements.length; i++) {
            if (crosshairDrawInfo.CrosshairElements[i].visible) {
                var crosshairLine = crosshairDrawInfo.CrosshairElements[i].Point.crosshairLine;
                var crosshairLineElement = crosshairDrawInfo.CrosshairElements[i].LineElement;
                this.DrawCrosshairLine(crosshairLine, crosshairDrawInfo.pane, chartX, chartY, crosshairLineElement);
            }
        }
        if (!ASPx.IsExists(crosshairDrawInfo.CursorCrosshairLineDrawInfo))
            return;
        var cursorCrosshairLine = crosshairDrawInfo.CursorCrosshairLineDrawInfo.crosshairLine;
        var crosshairLineElement = crosshairDrawInfo.CursorCrosshairLineDrawInfo.cursorCrosshairLineElement;
        this.DrawCrosshairLine(cursorCrosshairLine, crosshairDrawInfo.pane, chartX, chartY, crosshairLineElement);
    },
    SetLineStyle: function (lineDiv, crosshairLineElement) {
        var lineStyle = crosshairLineElement.lineStyle;
        lineDiv.style.borderRight = "0px";
        lineDiv.style.borderLeft = "0px";
        lineDiv.style.borderTop = "0px";
        lineDiv.style.borderBottom = "0px";
        if (lineStyle != null) {
            lineDiv.style.borderTop = lineStyle.thickness + "px";
            lineDiv.style.borderLeft = lineStyle.thickness + "px";
        }
        else {
            lineDiv.style.borderTop = "1px";
            lineDiv.style.borderLeft = "1px";
        }
        if (crosshairLineElement.color != null) {
            lineDiv.style.borderColor = "#" + crosshairLineElement.color;
        }
        var dashStyle;
        if (lineStyle != null) {
            switch (lineStyle.dashStyle) {
                case "Empty":
                    dashStyle = "none";
                    break;
                case "Solid":
                    dashStyle = "solid";
                    break;
                case "Dash":
                    dashStyle = "dashed";
                    break;
                default:
                    dashStyle = "dotted";
            }
        }
        else {
            dashStyle = "solid";
        }
        lineDiv.style.borderStyle = dashStyle;
    },
    DrawCrosshairLine: function (crosshairLine, pane, chartX, chartY, crosshairLineElement) {
        var axisValuePair = crosshairLine.axisValuePair;
        if (!crosshairLineElement.visible)
            return;
        if (crosshairLine.isHorizontal) {
            var lineDiv = this.GetHorizontalDiv(this.hLineIndex);
            var offsetY = Math.floor(this.GetLineThickness(true) / 2);
            __aspxChartSetDivPosition(lineDiv, chartX + pane.boundsLeft, chartY + axisValuePair.screenValue - offsetY);
            lineDiv.style.width = pane.boundsWidth + "px";
            this.SetLineStyle(lineDiv, crosshairLineElement);
            this.hLineIndex++;
        }
        else {
            var lineDiv = this.GetVerticalDiv(this.vLineIndex);
            var offsetX = Math.floor(this.GetLineThickness(false) / 2);
            __aspxChartSetDivPosition(lineDiv, chartX + axisValuePair.screenValue - offsetX, chartY + pane.boundsTop);
            lineDiv.style.height = pane.boundsHeight + "px";
            this.SetLineStyle(lineDiv, crosshairLineElement);
            this.vLineIndex++;
        }
    },
    DrawCrosshairAxisLabelElements: function(crosshairDrawInfo) {
        var chartImage = this.chart.GetImageElement();
        var chartX = ASPx.GetAbsoluteX(chartImage);
        var chartY = ASPx.GetAbsoluteY(chartImage);
        for (var i = 0; i < crosshairDrawInfo.CrosshairElements.length; i++) {
            if (crosshairDrawInfo.CrosshairElements[i].visible) {
                var crosshairAxisLabel = crosshairDrawInfo.CrosshairElements[i].Point.crosshairLabel;
                var crosshairAxisLabelElement = crosshairDrawInfo.CrosshairElements[i].AxisLabelElement;
                this.DrawCrosshairAxisLabel(crosshairAxisLabel, crosshairDrawInfo.pane, chartX, chartY, crosshairAxisLabelElement);
            }
        }
        for (var i = 0; i < crosshairDrawInfo.CursorCrosshairAxisLabelDrawInfos.length; i++) {
            var crosshairAxisLabel = crosshairDrawInfo.CursorCrosshairAxisLabelDrawInfos[i].crosshairAxisLabel;
            var crosshairAxisLabelElement = crosshairDrawInfo.CursorCrosshairAxisLabelDrawInfos[i].cursorCrosshairAxisLabelElement;
            this.DrawCrosshairAxisLabel(crosshairAxisLabel, crosshairDrawInfo.pane, chartX, chartY, crosshairAxisLabelElement);
        }
    },
    SetCrosshairAxisLabelOptions: function (labelDiv, crosshairAxisLabelElement) {
        if (crosshairAxisLabelElement.backColor != null) {
            labelDiv.style.background = "#" + crosshairAxisLabelElement.backColor;
        }
        if (crosshairAxisLabelElement.textColor != null) {
            labelDiv.style.color = "#" + crosshairAxisLabelElement.textColor;
        }
        if (crosshairAxisLabelElement.font != null) {
            labelDiv.style.font = crosshairAxisLabelElement.font.fontSize + "pt" + " " + crosshairAxisLabelElement.font.fontFamily;
        }
    },
    DrawCrosshairAxisLabel: function (crosshairAxisLabel, pane, chartX, chartY, crosshairAxisLabelElement) {
        var axisValuePair = crosshairAxisLabel.axisValuePair;
        if (!crosshairAxisLabelElement.visible)
            return;
        if (crosshairAxisLabel.isHorizontal) {
            var axisLabelBounds = this.FindAxisLabelBounds(pane, axisValuePair.axis);
            if (axisLabelBounds != null) {
                var labelDiv = this.GetHorizontalLabelDiv(this.hLabelIndex);
                this.SetCrosshairAxisLabelOptions(labelDiv, crosshairAxisLabelElement);
                labelDiv.innerHTML = crosshairAxisLabelElement.text; //this.GetLineLabelHtml(axisValuePair, !this.diagram.rotated);
                var offset = axisLabelBounds.left;
                if (axisLabelBounds.left < pane.boundsLeft)
                    offset += axisLabelBounds.width - labelDiv.offsetWidth;
                __aspxChartSetDivPosition(labelDiv, chartX + offset, chartY + axisValuePair.screenValue - labelDiv.offsetHeight / 2);
                this.hLabelIndex++;
            }
        }
        else {
            var axisLabelBounds = this.FindAxisLabelBounds(pane, axisValuePair.axis);
            if (axisLabelBounds != null) {
                var labelDiv = this.GetVerticalLabelDiv(this.vLabelIndex);
                this.SetCrosshairAxisLabelOptions(labelDiv, crosshairAxisLabelElement);
                labelDiv.innerHTML = crosshairAxisLabelElement.text; //this.GetLineLabelHtml(axisValuePair, this.diagram.rotated);
                var offset = axisLabelBounds.top;
                if (axisLabelBounds.top < pane.boundsTop)
                    offset += axisLabelBounds.height - labelDiv.offsetHeight;
                __aspxChartSetDivPosition(labelDiv, chartX + axisValuePair.screenValue - labelDiv.offsetWidth / 2, chartY + offset);
                this.vLabelIndex++;
            }
        }
    },
    DrawCrosshairSeriesLabel: function (pane, labelsBounds, pointsInfo, pointInfo, seriesLabelElement, isSingle) {
        if (!seriesLabelElement.visible)
            return;
        var labelDiv = this.GetLabelDiv(this.labelIndex);
        var marker = "";
        var text = "";
        if (seriesLabelElement.textVisible)
            text = seriesLabelElement.text;
        if (seriesLabelElement.markerVisible)
            marker = seriesLabelElement.marker;
        var textContent = __aspxChartGetHorizontalTable(marker, text);
        var content = __aspxChartGetVerticalTable(seriesLabelElement.headerText, textContent, seriesLabelElement.footerText);
        labelDiv.innerHTML = content;
        labelDiv.style.font = seriesLabelElement.font.fontSize + "pt" + " " + seriesLabelElement.font.fontFamily + " ";
        labelDiv.style.color = seriesLabelElement.textColor;
        var x = pointInfo.x - pane.boundsLeft;
        var y = pointInfo.y - pane.boundsTop;
        var labelBounds = { x: x, y: y, width: labelDiv.offsetWidth, height: labelDiv.offsetHeight };
        var direction = { x: 1, y: -1 };

        direction = this.CheckCrosshairSeriesLabelDirection(direction, x, y, labelDiv);

        this.SetDirection(labelBounds, x, y, direction.x, direction.y);

        if (!isSingle) {
            direction = this.CorrectCrosshairSeriesLabelBounds(labelsBounds, pane, labelBounds, pointsInfo, x, y, direction.x, direction.y);
        }
        var verticalPrefix = direction.y > 0 ? "Top" : "Bottom";
        var horizontalPrefix = direction.x > 0 ? "Left" : "Right";
        labelsBounds.push(labelBounds);
        var chartImage = this.chart.GetImageElement();
        var offsetX = ASPx.GetAbsoluteX(chartImage) + pane.boundsLeft;
        var offsetY = ASPx.GetAbsoluteY(chartImage) + pane.boundsTop;
        __aspxSetCssClassName(labelDiv, this.chart, chartToolTipClassName + "_" + verticalPrefix + horizontalPrefix);
        __aspxChartSetDivPosition(labelDiv, offsetX + labelBounds.x, offsetY + labelBounds.y);
        this.labelIndex++;
    },
    GetLabelHTML: function (point) {
        var color = ASPx.IsExists(point.color) ? point.color : point.series.color;
        var marker = "<div style='width:15px; height:12px; margin-top:5px; background-color:#" + color + "' />";
        var text = ASPx.ToolTipPatternHelper.GetPointToolTipText(point.series.crosshairLabelPattern, point, point.series);
        return __aspxChartGetHorizontalTable(marker, text);
    },
    CalcSquareLength: function (x1, y1, x2, y2) {
        return (x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2);
    },
    DrawCrosshairSeriesLabels: function (crosshairDrawInfo, paneCrosshairInfo) {
        if (this.diagram.rotated) {
            crosshairDrawInfo.CrosshairElements.sort(__aspxSortCrosshairElementByValue);
        }
        else {
            crosshairDrawInfo.CrosshairElements.sort(__aspxSortCrosshairElementByArgument);
        }
        if (crosshairDrawInfo.CrosshairElements.length < 1)
            return;
        var pointsInfo = [];
        for (var i = 0; i < crosshairDrawInfo.CrosshairElements.length; i++) {
            pointsInfo.push(crosshairDrawInfo.CrosshairElements[i].Point);
        }
        switch (this.chart.chart.crosshairOptions.crosshairLabelMode) {
            case "ShowForEachSeries":
                for (var i = 0; i < crosshairDrawInfo.CrosshairElements.length; i++) {
                    if (crosshairDrawInfo.CrosshairElements[i].visible) {
                        var pointInfo = crosshairDrawInfo.CrosshairElements[i].Point;
                        var seriesLabelElement = crosshairDrawInfo.CrosshairElements[i].LabelElement;
                        this.DrawCrosshairSeriesLabel(crosshairDrawInfo.pane, crosshairDrawInfo.labelsBounds, pointsInfo, pointInfo, seriesLabelElement, false);
                    }
                }
                break;
            case "ShowForNearestSeries":
                var min = this.CalcSquareLength(paneCrosshairInfo.x, paneCrosshairInfo.y, pointsInfo[0].x, pointsInfo[0].y);
                var index = 0;
                for (var i = 1; i < pointsInfo.length; i++) {
                    var distance = this.CalcSquareLength(paneCrosshairInfo.x, paneCrosshairInfo.y, pointsInfo[i].x, pointsInfo[i].y);
                    if (distance < min) {
                        min = distance;
                        index = i;
                    }
                }
                if (crosshairDrawInfo.CrosshairElements[index].visible) {
                    var seriesLabelElement = crosshairDrawInfo.CrosshairElements[index].LabelElement;
                    this.DrawCrosshairSeriesLabel(crosshairDrawInfo.pane, crosshairDrawInfo.labelsBounds, pointsInfo, pointsInfo[index], seriesLabelElement, true);
                }
                break;
        }
    },
    DrawCommonSeriesPointLabel: function (crosshairElementGroups, paneCrosshairInfoList) {
        var content = this.GetCommonSeriesLabelContent(crosshairElementGroups);
        if (!ASPx.IsExists(content))
            return;
        var chartImage = this.chart.GetImageElement();
        var chartX = ASPx.GetAbsoluteX(chartImage);
        var chartY = ASPx.GetAbsoluteY(chartImage);
        var labelDiv = this.GetLabelDiv(this.labelIndex);
        labelDiv.innerHTML = content;
        if (this.chart.chart.crosshairOptions.crosshairLabelPosition instanceof ASPxClientCrosshairFreePosition) {
            __aspxShowInFreePosition(labelDiv, this.chart.chart, this.chart.chart.crosshairOptions.crosshairLabelPosition, chartX, chartY);
        }
        else {
            var paneCrosshairInfo;
            for (var i = 0; i < paneCrosshairInfoList.length; i++) {
                if (paneCrosshairInfoList[i].pane == this.focusedPane) {
                    paneCrosshairInfo = paneCrosshairInfoList[i];
                    break;
                }
            }

            var offsetX = this.chart.chart.crosshairOptions.crosshairLabelPosition.offsetX;
            var offsetY = this.chart.chart.crosshairOptions.crosshairLabelPosition.offsetY;

            var x = paneCrosshairInfo.x + offsetX;
            var chartWidth = this.chart.GetMainElement().scrollWidth;
            if (x + labelDiv.offsetWidth > chartWidth) {
                x -= labelDiv.offsetWidth + 2 * offsetX;
            }
            var y = paneCrosshairInfo.y - labelDiv.offsetHeight - offsetY;
            if (y < 0) {
                y += labelDiv.offsetHeight + 2 * offsetY;
            }
            __aspxChartSetDivPosition(labelDiv, chartX + x, chartY + y);
        }
        this.labelIndex++;
    },
    GetCommonSeriesLabelContent: function (crosshairElementGroups) {
        var content = "";
        for (var j = 0; j < crosshairElementGroups.length; j++) {
            var group = crosshairElementGroups[j];
            var groupHeaderElement = group.HeaderElement;
            if (groupHeaderElement != null && groupHeaderElement.text != "" && groupHeaderElement.visible)
                content += this.GetGroupHeaderContent(groupHeaderElement);
            for (var pointIndex = 0; pointIndex < group.CrosshairElements.length; pointIndex++) {
                content += this.GetSingleLabelContent(group.CrosshairElements[pointIndex]);
            }
        }
        return content != "" ? "<table>" + content + "</table>" : null;
    },
    GetSingleLabelContent: function (crosshairElement) {
        var content = "";
        var seriesLabelElement = crosshairElement.LabelElement;
        if (!crosshairElement.visible || !seriesLabelElement.visible)
            return "";
        var style = "style = 'color:" + seriesLabelElement.textColor + ";" + "font:" + seriesLabelElement.font.fontSize + "pt" + " " + seriesLabelElement.font.fontFamily + ";'";
        if (seriesLabelElement.headerText != "")
            content += "<tr><td colspan='2' " + style + ">" + seriesLabelElement.headerText + "</td></tr>"
        if (seriesLabelElement.markerVisible) {
            var markerStyle;
            if (seriesLabelElement.marker != seriesLabelElement.defaultMarker)
                markerStyle = seriesLabelElement.marker;
            else
                markerStyle = "<div style='width:19px; height:14px; text-align:center'><div style='width:15px; height:12px; margin-top:5px; background-color:" + seriesLabelElement.markerColor + "'/></div>"
            content += "<tr><td style='vertical-align:top'>" + markerStyle + "</td>";
        }
        else
            content += "<tr><td></td>";
        if (seriesLabelElement.textVisible)
            content += "<td " + style + ">" + seriesLabelElement.text + "</td></tr>";
        else
            content += "<td></td></tr>";
        if (seriesLabelElement.footerText != "")
            content += "<tr><td colspan='2' " + style + ">" + seriesLabelElement.footerText + "</td></tr>";
        return content;
    },
    GetGroupHeaderContent: function (crosshairGroupHeaderElement) {
        var style = "style = 'color:" + crosshairGroupHeaderElement.textColor + ";font:" + crosshairGroupHeaderElement.font.fontSize + "pt" + " " + crosshairGroupHeaderElement.font.fontFamily + ";'";
        return "<tr><td colspan='2' " + style + ">" + crosshairGroupHeaderElement.text + "</td></tr>"
    },
    PointBelongsToGroup: function (point, group, snapsToArgument) {
        if (snapsToArgument)
            return point.argument.toString() == group.anchorValue.toString();
        else
            return point.values.length > 1 ? false : point.values[0].toString() == group.anchorValue.toString();
    },
    HasIntersection: function (v1, l1, v2, l2) {
        v1 -= this.indent;
        l1 += 2 * this.indent;
        var v12 = v1 + l1;
        var v22 = v2 + l2;
        return !(v1 <= v2 && v1 <= v22 && v12 <= v2 && v12 <= v22 || v1 >= v2 && v1 >= v22 && v12 >= v2 && v12 >= v22);
    },
    CalcIntersectionArea: function (bounds1, bounds2) {
        var x1 = bounds1.x > bounds2.x ? bounds1.x : bounds2.x;
        var right1 = bounds1.x + bounds1.width;
        var right2 = bounds2.x + bounds2.width;
        var x2 = right1 < right2 ? right1 : right2;
        var y1 = bounds1.y > bounds2.y ? bounds1.y : bounds2.y;
        var bottom1 = bounds1.y + bounds1.height;
        var bottom2 = bounds2.y + bounds2.height;
        var y2 = bottom1 < bottom2 ? bottom1 : bottom2;
        var width = x2 - x1;
        var height = y2 - y1;
        return width * height;
    },
    CheckWithLabelIntersection: function (labelsBounds, bounds) {
        var intersectionAreas = [];
        for (var i = 0; i < labelsBounds.length; i++) {
            var hasXIntersection = this.HasIntersection(bounds.x, bounds.width, labelsBounds[i].x, labelsBounds[i].width);
            var hasYIntersection = this.HasIntersection(bounds.y, bounds.height, labelsBounds[i].y, labelsBounds[i].height);
            if (hasXIntersection && hasYIntersection) {
                intersectionAreas.push(this.CalcIntersectionArea(bounds, labelsBounds[i]));
            }
            else {
                intersectionAreas.push(0);
            }
        }
        var sum = 0;
        for (var i = 0; i < intersectionAreas.length; i++) {
            sum += intersectionAreas[i];
        }
        return sum;
    },
    CheckWithPointIntersection: function (pane, bounds, pointsInfo) {
        for (var i = 0; i < pointsInfo.length; i++) {
            var x = pointsInfo[i].x - pane.boundsLeft;
            var y = pointsInfo[i].y - pane.boundsTop;
            if (x > bounds.x && x < bounds.x + bounds.width && y > bounds.y && y < bounds.y + bounds.height) {
                return false;
            }
        }
        return true;
    },
    SetDirection: function (bounds, x, y, dirX, dirY) {
        bounds.x = x;
        bounds.x += dirX > 0 ? -10 : 10 - bounds.width;
        bounds.y = y;
        bounds.y += dirY > 0 ? 20 : -20 - bounds.height;
    },
    CheckCrosshairSeriesLabelDirection: function (direction, x, y, labelDiv) {
        if (y - labelDiv.offsetHeight < 0)
            direction = { x: direction.x, y: -direction.y };

        if (x + labelDiv.offsetWidth > this.chart.GetWidth())
            direction = { x: -direction.x, y: direction.y };
        return direction;
    },
    CorrectCrosshairSeriesLabelBounds: function (labelsBounds, pane, bounds, pointsInfo, x, y, dirX, dirY) {
        var directions = [];
        directions.push({ x: dirX, y: dirY });
        directions.push({ x: dirX, y: -dirY });
        directions.push({ x: -dirX, y: dirY });
        directions.push({ x: -dirX, y: -dirY });
        var intersections = [];
        for (var i = 0; i < directions.length; i++) {
            this.SetDirection(bounds, x, y, directions[i].x, directions[i].y);
            var intersection = this.CheckWithLabelIntersection(labelsBounds, bounds);
            if (intersection == 0 && this.CheckWithPointIntersection(pane, bounds, pointsInfo)) {
                return directions[i];
            }
            else {
                intersections.push(intersection);
            }
        }
        var min = intersections[0];
        var index = 0;
        for (var i = 1; i < intersections.length; i++) {
            if (intersections[i] < min) {
                min = intersections[i];
                index = i;
            }
        }
        this.SetDirection(bounds, x, y, directions[index].x, directions[index].y);
        return directions[index];
    },
    ExtractGroups: function (crosshairDrawInfoList) {
        var options = this.chart.chart.crosshairOptions;
        var snapsToArgument = options.snapMode == "NearestArgument";
        var totalCrosshairElements = [];
        for (var i = 0; i < crosshairDrawInfoList.length; i++)
            for (var j = 0; j < crosshairDrawInfoList[i].CrosshairElements.length; j++)
                totalCrosshairElements.push(crosshairDrawInfoList[i].CrosshairElements[j]);
        var groups = [];
        for (var elementIndex = 0; elementIndex < totalCrosshairElements.length; elementIndex++) {
            var crosshairElement = totalCrosshairElements[elementIndex];
            var point = crosshairElement.Point.point;
            var isInGroup = false;
            if (options.showGroupHeaders) {
                for (var groupIndex = 0; groupIndex < groups.length; groupIndex++) {
                    var group = groups[groupIndex];
                    if (this.PointBelongsToGroup(point, group, snapsToArgument)) {
                        isInGroup = true;
                        group.crosshairElements.push(crosshairElement);
                        break;
                    }
                }
            }
            if (!isInGroup) {
                var group = new CrosshairElementsGroup(point, snapsToArgument);
                group.crosshairElements.push(crosshairElement);
                groups.push(group);
            }
        }
        var converter = new ASPx.CrosshairGroupHeaderValueToStringConverter(snapsToArgument, !snapsToArgument);
        var hasHeader = (groups.length != totalCrosshairElements.length) && options.showGroupHeaders;
        for (var groupIndex = 0; groupIndex < groups.length; groupIndex++) {
            var group = groups[groupIndex];
            var basePoint = group.crosshairElements[0].Point.point;
            group.headerText = hasHeader ? ASPx.ToolTipPatternHelper.GetPointToolTipTextByConverter(options.groupHeaderPattern, basePoint, basePoint.series, converter) : "";
            var groupHeaderElement = options.showGroupHeaders ? new ASPxClientCrosshairGroupHeaderElement(group) : null;
            group.crosshairGroupHeaderElement = groupHeaderElement;
            for (var elementIndex = 0; elementIndex < group.crosshairElements.length; elementIndex++) {
                var crosshairElement = group.crosshairElements[elementIndex];
                var point = crosshairElement.Point.point;
                var pattern = hasHeader ? point.series.groupedElementsPattern : point.series.crosshairLabelPattern;
                var seriesLabelElement = crosshairElement.LabelElement;
                seriesLabelElement.text = ASPx.ToolTipPatternHelper.GetPointToolTipText(pattern, point, point.series);
                if (groupHeaderElement != null)
                    groupHeaderElement.seriesPoints.push(crosshairElement.Point);
            }
        }
        return groups;
    },
    UpdateCrosshair: function (x, y, chartControl) {
        this.focusedPane = this.GetPane(x, y);
        if (this.focusedPane == null) {
            this.Hide();
            return;
        }
        var panes = [];
        if (!this.chart.chart.crosshairOptions.showOnlyInFocusedPane &&
            ((this.diagram.paneLayoutDirection == "Vertical" && this.isHorizontal) ||
            (this.diagram.paneLayoutDirection == "Horizontal" && !this.isHorizontal))) {
            panes.push(this.diagram.defaultPane);
            for (var i = 0; i < this.diagram.panes.length; i++) {
                panes.push(this.diagram.panes[i]);
            }
        }
        else {
            panes.push(this.focusedPane);
        }
        var paneCrosshairInfoList = [];
        var crosshairDrawInfoList = [];
        for (var i = 0; i < panes.length; i++) {
            if (!panes[i].visible)
                continue;
            var paneCrosshairInfo = this.CalculatePaneCrosshairInfo(panes[i], x, y)
            paneCrosshairInfoList.push(paneCrosshairInfo);
            crosshairDrawInfo = new ASPxClientCrosshairDrawInfo(this.chart);
            crosshairDrawInfo.AddPaneCrosshairInfo(paneCrosshairInfo, chartControl.chart.diagram.rotated);
            crosshairDrawInfo.pane = panes[i];
            crosshairDrawInfoList.push(crosshairDrawInfo);
        }
        var crosshairGroups = [];
        if (this.chart.chart.crosshairOptions.crosshairLabelMode == "ShowCommonForAllSeries")
            crosshairGroups = this.ExtractGroups(crosshairDrawInfoList);
        if (ASPx.IsExists(chartControl.RaiseCustomDrawCrosshair)) {
            var crosshairDrawInfo = new ASPxClientCrosshairDrawInfoList(crosshairDrawInfoList, crosshairGroups);
            chartControl.RaiseCustomDrawCrosshair(crosshairDrawInfo.CursorCrosshairAxisLabelElements, crosshairDrawInfo.CursorCrosshairLineElement, crosshairDrawInfo.CrosshairElementGroups);
        }
        this.hLineIndex = 0;
        this.hLabelIndex = 0;
        this.vLineIndex = 0;
        this.vLabelIndex = 0;
        this.labelIndex = 0;
        for (var i = 0; i < paneCrosshairInfoList.length; i++) {
            this.DrawCrosshairLineElements(crosshairDrawInfoList[i]);
            this.DrawCrosshairAxisLabelElements(crosshairDrawInfoList[i]);
            if (this.chart.chart.crosshairOptions.crosshairLabelMode != "ShowCommonForAllSeries") {
                this.DrawCrosshairSeriesLabels(crosshairDrawInfoList[i], paneCrosshairInfoList[i]);
            }
        }
        if (this.chart.chart.crosshairOptions.crosshairLabelMode == "ShowCommonForAllSeries") {
            this.DrawCommonSeriesPointLabel(crosshairDrawInfo.CrosshairElementGroups, paneCrosshairInfoList);
        }
        this.ClearDivs(this.hLineIndex, this.vLineIndex, this.hLabelIndex, this.vLabelIndex, this.labelIndex);
    }
});

var PaneCrosshairInfo = ASPx.CreateClass(null, {
    constructor: function(pane, x, y) {
        this.pane = pane;
        this.horizontalLines = [];
        this.verticalLines = [];
        this.horizontalLabels = [];
        this.verticalLabels = [];
        this.pointsInfo = [];
        this.labelsBounds = [];
        this.x = x;
        this.y = y;
        this.cursorLabel = [];
    },
    AddCursorCrosshairLine: function(axisValuePair, isHorizontal, isSingle) {
        this.cursorLine = this.GetCrosshairLine(axisValuePair, isHorizontal);
    },
    AddCursorCrosshairLabel: function(axisValuePair, isHorizontal) {
        this.cursorLabel.push({ axisValuePair: axisValuePair, isHorizontal: isHorizontal });
    },
    AddCrosshairLine: function(point, axisValuePair, isHorizontal, isSingle) {
        if (this.Contains(this.pointsInfo, point)) {
            point.crosshairLine = this.GetCrosshairLine(axisValuePair, isHorizontal);
        }
    },
    AddCrosshairLabel: function(point, axisValuePair, isHorizontal) {
        if (this.Contains(this.pointsInfo, point)) {
            point.crosshairLabel = { axisValuePair: axisValuePair, isHorizontal: isHorizontal };
        }
    },
    GetCrosshairLine: function(axisValuePair, isHorizontal) {
        return { axisValuePair: axisValuePair, isHorizontal: isHorizontal };
    },
    AddPoint: function(point) {
        if (!this.Contains(this.pointsInfo, point)) {
            this.pointsInfo.push(point);
        }
    },
    Contains: function(array, crosshairPoint) {
        for (var i = 0; i < array.length; i++) {
            if (array[i].axisValue.axis == crosshairPoint.axisValue.axis && array[i].axisValue.internalValue == crosshairPoint.axisValue.internalValue
                && array[i].point.series.name == crosshairPoint.point.series.name) {
                return true;
            }
        }
        return false;
    }
});

var AxisValuePair = ASPx.CreateClass(null, {
    constructor: function(axis, internalValue, screenValue) {
        this.axis = axis;
        this.internalValue = internalValue;
        this.screenValue = screenValue;
    }
});

ASPx.ChartGetHorizontalTable = __aspxChartGetHorizontalTable;
ASPx.GetChartDivId = __aspxGetChartDivId;
ASPx.CreateChartToolTip = __aspxCreateChartToolTip;
ASPx.RemoveChartDiv = __aspxRemoveChartDiv;
ASPx.ChartSetDivPosition = __aspxChartSetDivPosition;
ASPx.ShowInFreePosition = __aspxShowInFreePosition;

ASPx.CrosshairRenderer = CrosshairRenderer;
ASPx.AxisValuePair = AxisValuePair;
})();