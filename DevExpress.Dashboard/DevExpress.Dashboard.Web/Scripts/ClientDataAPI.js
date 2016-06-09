// Client Data API

(function() {
var DashboardDataAxisNames = {
    DefaultAxis: DevExpress.dashboard.itemDataAxisNames.defaultAxis,
    ChartSeriesAxis: DevExpress.dashboard.itemDataAxisNames.chartSeriesAxis,
    ChartArgumentAxis: DevExpress.dashboard.itemDataAxisNames.chartArgumentAxis,
    SparklineAxis: DevExpress.dashboard.itemDataAxisNames.sparklineAxis,
    PivotColumnAxis: DevExpress.dashboard.itemDataAxisNames.pivotColumnAxis,
    PivotRowAxis: DevExpress.dashboard.itemDataAxisNames.pivotRowAxis
};
var ASPxClientDashboardItemUnderlyingData = ASPx.CreateClass(null, {
    constructor: function (data) {
        this.constructor.prototype.constructor.call(this);
        this.GetRowCount = ASPx.FunctionWrapper(data.getRowCount, data);
        this.GetRowValue = ASPx.FunctionWrapper(data.getRowValue, data);
        this.GetDataMembers = ASPx.FunctionWrapper(data.getDataMembers, data);
        this.IsDataReceived = ASPx.FunctionWrapper(data.isDataReceived, data);
        this.GetRequestDataError = ASPx.FunctionWrapper(data.getRequestDataError, data);
    }
});
var ASPxClientDashboardItemRequestUnderlyingDataParameters = ASPx.CreateClass(null, {
    constructor: function (args) {
        this.constructor.prototype.constructor.call(this);
        this.DataMembers = null;
        this.AxisPoints = null;

        //TODO
        this.ValuesByAxisName = null;

        //TODO
        this.UniqueValuesByAxisName = null;
    }
});
var ASPxClientDashboardItemClickEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function (args) {
        this.constructor.prototype.constructor.call(this);
        this.ItemName = args.itemName;
        this.GetData = ASPx.FunctionWrapper(args.getData, args, ASPxClientDashboardItemData);
        this.GetAxisPoint = ASPx.FunctionWrapper(args.getAxisPoint, args);
        this.GetMeasures = ASPx.ArrayFunctionWrapper(args.getMeasures, args, ASPxClientDashboardItemDataMeasure);
        this.GetDeltas = ASPx.ArrayFunctionWrapper(args.getDeltas, args, ASPxClientDashboardItemDataDelta);
        this.GetDimensions = ASPx.ArrayFunctionWrapper(args.getDimensions, args, ASPxClientDashboardItemDataDimension);
        this.RequestUnderlyingData = function (onCompleted, dataMembers) {
            args.requestUnderlyingData(function (data) {
                if (onCompleted)
                    onCompleted(new ASPxClientDashboardItemUnderlyingData(data));
            }, dataMembers);
        };
    }
});
var ASPxClientDashboardItemVisualInteractivityEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function (args) {
        this.constructor.prototype.constructor.call(this);
        this.ItemName = args.itemName;
        this.GetSelectionMode = ASPx.FunctionWrapper(args.getSelectionMode, args);
        this.SetSelectionMode = ASPx.FunctionWrapper(args.setSelectionMode, args);
        this.IsHighlightingEnabled = ASPx.FunctionWrapper(args.isHighlightingEnabled, args);
        this.EnableHighlighting = ASPx.FunctionWrapper(args.enableHighlighting, args);
        this.GetTargetAxes = ASPx.FunctionWrapper(args.getTargetAxes, args);
        this.SetTargetAxes = ASPx.FunctionWrapper(args.setTargetAxes, args);
        this.GetDefaultSelection = ASPx.FunctionWrapper(args.getDefaultSelection, args);
        this.SetDefaultSelection = ASPx.FunctionWrapper(args.setDefaultSelection, args);
    }
});
var ASPxClientDashboardItemSelectionChangedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function (args) {
        this.constructor.prototype.constructor.call(this);
        this.ItemName = args.itemName;
        this.GetCurrentSelection = ASPx.FunctionWrapper(args.getCurrentSelection, args);
    }
});
var ASPxClientDashboardItemElementCustomColorEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function (args) {
        this.constructor.prototype.constructor.call(this);
        this.ItemName = args.itemName;
        this.GetTargetElement = ASPx.FunctionWrapper(args.getTargetElement, args);
        this.GetColor = ASPx.FunctionWrapper(args.getColor, args);
        this.SetColor = ASPx.FunctionWrapper(args.setColor, args);
        this.GetMeasures = ASPx.ArrayFunctionWrapper(args.getMeasures, args, ASPxClientDashboardItemDataMeasure);
    }
});
var ASPxClientDashboardItemWidgetEventArgs = ASPx.CreateClass(null, {
    constructor: function (args) {
        this.constructor.prototype.constructor.call(this);
        this.ItemName = args.itemName;
        this.GetWidget = ASPx.FunctionWrapper(args.getWidget, args);
    }
});
var ASPxClientDashboardItemData = ASPx.CreateClass(null, {
    constructor: function (data) {
        this.constructor.prototype.constructor.call(this);
        this.GetAxisNames = ASPx.FunctionWrapper(data.getAxisNames, data);
        this.GetAxis = ASPx.FunctionWrapper(data.getAxis, data, ASPxClientDashboardItemDataAxis);
        this.GetDimensions = ASPx.ArrayFunctionWrapper(data.getDimensions, data, ASPxClientDashboardItemDataDimension);
        this.GetMeasures = ASPx.ArrayFunctionWrapper(data.getMeasures, data, ASPxClientDashboardItemDataMeasure);
        this.GetDeltas = ASPx.ArrayFunctionWrapper(data.getDeltas, data, ASPxClientDashboardItemDataDelta);
        this.GetSlice = ASPx.FunctionWrapper(data.getSlice, data, ASPxClientDashboardItemData);
        this.GetMeasureValue = ASPx.FunctionWrapper(data.getMeasureValue, data, ASPxClientDashboardItemDataMeasureValue);
        this.GetDeltaValue = ASPx.FunctionWrapper(data.getDeltaValue, data, ASPxClientDashboardItemDataDeltaValue);
        this.GetDataMembers = ASPx.FunctionWrapper(data.getDataMembers, data);
        this.CreateTuple = ASPx.FunctionWrapper(data.createTuple, data);
    }
});
var ASPxClientDashboardItemDataAxis = ASPx.CreateClass(null, {
    constructor: function (axis) {
        this.constructor.prototype.constructor.call(this);
        this.GetDimensions = ASPx.ArrayFunctionWrapper(axis.getDimensions, axis, ASPxClientDashboardItemDataDimension);
        this.GetRootPoint = ASPx.FunctionWrapper(axis.getRootPoint, axis);
        this.GetPoints = ASPx.FunctionWrapper(axis.getPoints, axis);
        this.GetPointsByDimension = ASPx.FunctionWrapper(axis.getPointsByDimension, axis);
        this.GetPointByUniqueValues = ASPx.FunctionWrapper(axis.getPointByUniqueValues, axis);
    }
});
var ASPxClientDashboardItemDataDimension = ASPx.CreateClass(null, {
    constructor: function (dimension) {
        this.constructor.prototype.constructor.call(this);
        this.Id = dimension.id;
        this.Name = dimension.name;
        this.DataMember = dimension.dataMember;

        //TODO create enums for group intervals
        this.DateTimeGroupInterval = dimension.dataTimeGroupInterval;
        this.TextGroupInterval = dimension.textGroupInterval;
        this.Format = ASPx.FunctionWrapper(dimension.format, dimension);
    }
});
var ASPxClientDashboardItemDataMeasure = ASPx.CreateClass(null, {
    constructor: function (measure) {
        this.constructor.prototype.constructor.call(this);
        this.Id = measure.id;
        this.Name = measure.name;
        this.DataMember = measure.dataMember;

        //TODO create enums for Summary Types
        this.SummaryType = measure.summaryType;
        this.Format = ASPx.FunctionWrapper(measure.format, measure);
    }
});
var ASPxClientDashboardItemDataDelta = ASPx.CreateClass(null, {
    constructor: function (delta) {
        this.constructor.prototype.constructor.call(this);
        this.Id = delta.id;
        this.Name = delta.name;
        this.ActualMeasureId = delta.actualMeasureId;
        this.TargetMeasureId = delta.targetMeasureId;
    }
});
var ASPxClientDashboardItemDataDimensionValue = ASPx.CreateClass(null, {
    constructor: function (value) {
        this.constructor.prototype.constructor.call(this);
        this.GetValue = ASPx.FunctionWrapper(value.getValue, value);
        this.GetUniqueValue = ASPx.FunctionWrapper(value.getUniqueValue, value);
        this.GetDisplayText = ASPx.FunctionWrapper(value.getDisplayText, value);
    }
});
var ASPxClientDashboardItemDataMeasureValue = ASPx.CreateClass(null, {
    constructor: function (value) {
        this.constructor.prototype.constructor.call(this);
        this.GetValue = ASPx.FunctionWrapper(value.getValue, value);
        this.GetDisplayText = ASPx.FunctionWrapper(value.getDisplayText, value);
    }
});
var ASPxClientDashboardItemDataDeltaValue = ASPx.CreateClass(null, {
    constructor: function (value) {
        this.constructor.prototype.constructor.call(this);
        this.GetActualValue = ASPx.FunctionWrapper(value.getActualValue, value, ASPxClientDashboardItemDataMeasureValue);
        this.GetTargetValue = ASPx.FunctionWrapper(value.getTargetValue, value, ASPxClientDashboardItemDataMeasureValue);
        this.GetAbsoluteVariation = ASPx.FunctionWrapper(value.getAbsoluteVariation, value, ASPxClientDashboardItemDataMeasureValue);
        this.GetPercentVariation = ASPx.FunctionWrapper(value.getPercentVariation, value, ASPxClientDashboardItemDataMeasureValue);
        this.GetPercentOfTarget = ASPx.FunctionWrapper(value.getPercentOfTarget, value, ASPxClientDashboardItemDataMeasureValue);
        this.GetDisplayValue = ASPx.FunctionWrapper(value.getDisplayValue, value, ASPxClientDashboardItemDataMeasureValue);
        this.GetDisplaySubValue1 = ASPx.FunctionWrapper(value.getDisplaySubValue1, value, ASPxClientDashboardItemDataMeasureValue);
        this.GetDisplaySubValue2 = ASPx.FunctionWrapper(value.getDisplaySubValue2, value, ASPxClientDashboardItemDataMeasureValue);
        this.GetIsGood = ASPx.FunctionWrapper(value.getIsGood, value, ASPxClientDashboardItemDataMeasureValue);
        this.GetIndicatorType = ASPx.FunctionWrapper(value.getIndicatorType, value, ASPxClientDashboardItemDataMeasureValue);
    }
});

ASPx.FunctionWrapper = function (fn, context, resultWrapperClass) {
    return function () {
        var ctx = context || this,
            result = fn.apply(ctx, Array.prototype.slice.call(arguments));
        if (resultWrapperClass && result)
            result = new resultWrapperClass(result);
        return result;
    };
};

ASPx.ArrayFunctionWrapper = function (fn, context, resultItemWrapperClass) {
    return ASPx.FunctionWrapper(fn, context, function (array) {
        var resArray = [];
        $.each(array, function (_, item) {
            resArray.push(new resultItemWrapperClass(item));
        });
        return resArray;
    });
};

var axisPointPrototype = DevExpress.dashboard.data.itemDataAxisPoint.prototype;
axisPointPrototype.GetAxisName = axisPointPrototype.getAxisName;
axisPointPrototype.GetDimension = ASPx.FunctionWrapper(axisPointPrototype.getDimension, null, ASPxClientDashboardItemDataDimension);
axisPointPrototype.GetDimensions = ASPx.ArrayFunctionWrapper(axisPointPrototype.getDimensions, null, ASPxClientDashboardItemDataDimension);
axisPointPrototype.GetValue = axisPointPrototype.getValue;
axisPointPrototype.GetDisplayText = axisPointPrototype.getDisplayText;
axisPointPrototype.GetUniqueValue = axisPointPrototype.getUniqueValue;
axisPointPrototype.GetDimensionValue = ASPx.FunctionWrapper(axisPointPrototype.getDimensionValue, null, ASPxClientDashboardItemDataDimensionValue);
axisPointPrototype.GetChildren = axisPointPrototype.getChildren;
axisPointPrototype.GetParent = axisPointPrototype.getParent;

var itemDataTuplePrototype = DevExpress.dashboard.data.itemDataTuple.prototype;
itemDataTuplePrototype.GetAxisPoint = itemDataTuplePrototype.getAxisPoint;

window.DashboardDataAxisNames = DashboardDataAxisNames;
window.ASPxClientDashboardItemUnderlyingData = ASPxClientDashboardItemUnderlyingData;

window.ASPxClientDashboardItemRequestUnderlyingDataParameters = ASPxClientDashboardItemRequestUnderlyingDataParameters;
window.ASPxClientDashboardItemClickEventArgs = ASPxClientDashboardItemClickEventArgs;
window.ASPxClientDashboardItemVisualInteractivityEventArgs = ASPxClientDashboardItemVisualInteractivityEventArgs;
window.ASPxClientDashboardItemSelectionChangedEventArgs = ASPxClientDashboardItemSelectionChangedEventArgs;
window.ASPxClientDashboardItemWidgetEventArgs = ASPxClientDashboardItemWidgetEventArgs;
window.ASPxClientDashboardItemData = ASPxClientDashboardItemData;
window.ASPxClientDashboardItemDataAxis = ASPxClientDashboardItemDataAxis;
window.ASPxClientDashboardItemDataDimension = ASPxClientDashboardItemDataDimension;
window.ASPxClientDashboardItemDataMeasure = ASPxClientDashboardItemDataMeasure;
window.ASPxClientDashboardItemDataDelta = ASPxClientDashboardItemDataDelta;
window.ASPxClientDashboardItemDataDimensionValue = ASPxClientDashboardItemDataDimensionValue;
window.ASPxClientDashboardItemDataMeasureValue = ASPxClientDashboardItemDataMeasureValue;
window.ASPxClientDashboardItemDataDeltaValue = ASPxClientDashboardItemDataDeltaValue;
window.ASPxClientDashboardItemDataAxisPoint = DevExpress.dashboard.data.itemDataAxisPoint;
window.ASPxClientDashboardItemDataTuple = DevExpress.dashboard.data.itemDataTuple;
window.ASPxClientDashboardItemElementCustomColorEventArgs = ASPxClientDashboardItemElementCustomColorEventArgs;
})();