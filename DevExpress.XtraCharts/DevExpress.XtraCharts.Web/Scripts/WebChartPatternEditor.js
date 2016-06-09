

(function() {
var _argumentPattern = "A";
var _valuePattern = "V";
var _seriesNamePattern = "S";
var _stackedGroupPattern = "G";
var _value1Pattern = "V1";
var _value2Pattern = "V2";
var _valueDurationPattern = "VD";
var _weightPattern = "W";
var _highValuePattern = "HV";
var _lowValuePattern = "LV";
var _openValuePattern = "OV";
var _closeValuePattern = "CV";
var _percentValuePattern = "VP";
var _pointHintPattern = "HINT";

var PointDataToStringConverter = ASPx.CreateClass(null, {
    constructor: function () {
        this.allowArgument = ASPx.IsExists(this.allowArgument) ? this.allowArgument : true;
        this.allowValue = ASPx.IsExists(this.allowValue) ? this.allowValue : true;
    },
    GetHintText: function (seriesPoint) {
        return seriesPoint.hint != null ? seriesPoint.hint : "";
    },
    GetArgumentText: function (seriesPoint, series, format) {
        if (seriesPoint.argument == null)
            return "";
        if (series.argumentScaleType == ASPxClientScaleType.Qualitative)
            return seriesPoint.argument;
        else {
            if (series.argumentScaleType == ASPxClientScaleType.DateTime && format == "q") {
                var monthNum = Math.floor(seriesPoint.argument.getMonth() / 3) + 1;
                return "Q" + monthNum + " " + seriesPoint.argument.getFullYear();
            }
            else
                return ASPx.Formatter.Format("{0:" + format + "}", [seriesPoint.argument]);
        }
    },
    GetValueTextByIndex: function(valueIndex, seriesPoint, format) {
        var formattedValue = ASPx.Formatter.Format("{0:" + format + "}", seriesPoint.values[valueIndex]);
        return valueIndex >= seriesPoint.values.length ? "" : formattedValue;
    }
});

var ValueToStringConverter = ASPx.CreateClass(PointDataToStringConverter, {
    constructor: function() {
        this.constructor.prototype.constructor.call(this);
    },
    GetValueText: function(seriesPoint, format) {
        return this.GetValueTextByIndex(0, seriesPoint, format);
    }
});

var RangeValueToStringConverter = ASPx.CreateClass(PointDataToStringConverter, {
    constructor: function() {
        this.constructor.prototype.constructor.call(this);
    },
    GetValue1Text: function(seriesPoint, format) {
        return this.GetValueTextByIndex(0, seriesPoint, format);
    },
    GetValue2Text: function(seriesPoint, format) {
        return this.GetValueTextByIndex(1, seriesPoint, format);
    },
    GetValueDurationText: function(seriesPoint, format) {
        var valueDuration = Math.abs(seriesPoint.values[1] - seriesPoint.values[0]);
        var formattedValue = ASPx.Formatter.Format("{0:" + format + "}", valueDuration);
        return seriesPoint.values.length < 2 ? "" : formattedValue;
    }
});

var BubbleValueToStringConverter = ASPx.CreateClass(ValueToStringConverter, {
    constructor: function() {
        this.constructor.prototype.constructor.call(this);
    },
    GetWeightText: function(seriesPoint, format) {
        return this.GetValueTextByIndex(1, seriesPoint, format);
    }
});

var FinancialValueToStringConverter = ASPx.CreateClass(PointDataToStringConverter, {
    constructor: function() {
        this.constructor.prototype.constructor.call(this);
    },
    GetLowValueText: function(seriesPoint, format) {
        return this.GetValueTextByIndex(0, seriesPoint, format);
    },
    GetHighValueText: function(seriesPoint, format) {
        return this.GetValueTextByIndex(1, seriesPoint, format);
    },
    GetOpenValueText: function(seriesPoint, format) {
        return this.GetValueTextByIndex(2, seriesPoint, format);
    },
    GetCloseValueText: function(seriesPoint, format) {
        return this.GetValueTextByIndex(3, seriesPoint, format);
    }
});

var PercentValueToStringConverter = ASPx.CreateClass(ValueToStringConverter, {
    constructor: function() {
        this.constructor.prototype.constructor.call(this);
    },
    GetPercentValueText: function(seriesPoint, format) {
        if (seriesPoint.percentValue == null)
            return format;
        return ASPx.Formatter.Format("{0:" + format + "}", seriesPoint.percentValue);
    }
});

var CrosshairGroupHeaderValueToStringConverter = ASPx.CreateClass(ValueToStringConverter, {
    constructor: function(allowArgument, allowValue) {
        this.allowValue = allowValue;
        this.allowArgument = allowArgument;
        this.constructor.prototype.constructor.call(this);
    }
});

var ToolTipPatternHelper = {
    ReplacePatternToValue: function(fragment, series, seriesPoint, converter) {
        var preparedFragment = this.PrepareFragment(fragment);
        if (preparedFragment == null)
            return fragment;
        var format = preparedFragment[0];
        var pattern = preparedFragment[1];
        var allowAllPatterns = converter != null ? (converter.allowArgument && converter.allowValue) : true;
        switch (pattern) {
            case _seriesNamePattern:
                return allowAllPatterns ? series.name : fragment;
            case _stackedGroupPattern:
                return series.stackedGroup != null && allowAllPatterns ? series.stackedGroup : fragment;
        }
        if (seriesPoint != null) {
            var result;
            switch (pattern) {
                case _argumentPattern:
                    result = converter.allowArgument ? converter.GetArgumentText(seriesPoint, series, format) : fragment;
                    break;
                case _valuePattern:
                    result = (converter instanceof ValueToStringConverter) && converter.allowValue ? converter.GetValueText(seriesPoint, format) : fragment;
                    break;
                case _value1Pattern:
                    result = (converter instanceof RangeValueToStringConverter) && allowAllPatterns ? converter.GetValue1Text(seriesPoint, format) : fragment;
                    break;
                case _value2Pattern:
                    result = (converter instanceof RangeValueToStringConverter) && allowAllPatterns ? converter.GetValue2Text(seriesPoint, format) : fragment;
                    break;
                case _valueDurationPattern:
                    result = (converter instanceof RangeValueToStringConverter) && allowAllPatterns ? converter.GetValueDurationText(seriesPoint, format) : fragment;
                    break;
                case _weightPattern:
                    result = (converter instanceof BubbleValueToStringConverter) && allowAllPatterns ? converter.GetWeightText(seriesPoint, format) : fragment;
                    break;
                case _highValuePattern:
                    result = (converter instanceof FinancialValueToStringConverter) && allowAllPatterns ? converter.GetHighValueText(seriesPoint, format) : fragment;
                    break;
                case _lowValuePattern:
                    result = (converter instanceof FinancialValueToStringConverter) && allowAllPatterns ? converter.GetLowValueText(seriesPoint, format) : fragment;
                    break;
                case _openValuePattern:
                    result = (converter instanceof FinancialValueToStringConverter) && allowAllPatterns ? converter.GetOpenValueText(seriesPoint, format) : fragment;
                    break;
                case _closeValuePattern:
                    result = (converter instanceof FinancialValueToStringConverter) && allowAllPatterns ? converter.GetCloseValueText(seriesPoint, format) : fragment;
                    break;
                case _percentValuePattern:
                    result = (converter instanceof PercentValueToStringConverter) && allowAllPatterns ? converter.GetPercentValueText(seriesPoint, format) : fragment;
                    break;
                case _pointHintPattern:
                    result = allowAllPatterns ? converter.GetHintText(seriesPoint) : fragment;
                    break;
                default:
                    result = fragment;
                    break;
            }
            return result;
        }
        else
            return fragment;

    },
    PrepareFragment: function(fragment) {
        var pattern = "";
        var format = "";
        if (!(fragment.charAt(0) == "{" && fragment.charAt(fragment.length - 1) == "}"))
            return null;
        pattern = fragment.substring(1, fragment.length - 1);
        var formatIndex = pattern.indexOf(":");
        if (formatIndex >= 0) {
            format = pattern.substring(formatIndex + 1).replace(/^\s*/, "").replace(/\s*$/, "");
            pattern = pattern.substring(0, formatIndex);
        }
        pattern = pattern.replace(/^\s*/, "").replace(/\s*$/, "").toUpperCase();
        return [format, pattern];
    },
    SplitString: function(splitingString, leftSeparator, rightSeparator) {
        var substrings = [];
        var leftStringIndex = 0;
        var rightStringIndex = 0;
        var currentIndex = 0;
        if (splitingString != null && splitingString != "") {
            for (var i = 0; i < splitingString.length; i++) {
                var charElement = splitingString.charAt(i);
                if (charElement == leftSeparator) {
                    leftStringIndex = currentIndex;
                }
                else {
                    if (charElement == rightSeparator) {
                        rightStringIndex = currentIndex;
                        substrings.push(splitingString.substring(leftStringIndex, rightStringIndex + 1));
                    }
                }
                currentIndex++;
            }
            return substrings;
        }
        else
            return null;
    },
    GetPointToolTipText: function(seriesPointPattern, seriesPoint, series) {
        return this.GetPointToolTipTextByConverter(seriesPointPattern, seriesPoint, series, this.GetPatternConverter(series));
    },
    GetPointToolTipTextByConverter: function(seriesPointPattern, seriesPoint, series, converter) {
        if (converter == null)
            return "";
        parsedPattern = this.SplitString(seriesPointPattern, "{", "}");
        var result = seriesPointPattern;
        if (parsedPattern != null)
            for (var i = 0; i < parsedPattern.length; i++) {
                var fragment = parsedPattern[i];
                var formattedFragment = this.ReplacePatternToValue(fragment, series, seriesPoint, converter);
                result = result.replace(fragment, formattedFragment);
            }
        return result;
    },
    GetSeriesToolTipText: function(seriesPattern, series) {
        if (seriesPattern == null || seriesPattern == "")
            return "";
        parsedPattern = this.SplitString(seriesPattern, "{", "}");
        var result = seriesPattern;
        if (parsedPattern != null)
            for (var i = 0; i < parsedPattern.length; i++) {
                var fragment = parsedPattern[i];
                result = result.replace(fragment, this.ReplacePatternToValue(fragment, series, null, null));
            }
        return result;
    },
    GetPatternConverter: function(series) {
        var viewType = series.viewType;
        if (viewType == "Pie" || viewType == "Funnel" || viewType == "Doughnut" || viewType == "NestedDoughnut")
            return new PercentValueToStringConverter();
        if (viewType == "FullStackedArea" || viewType == "FullStackedBar" || viewType == "SideBySideFullStackedBar" || viewType == "FullStackedLine" || viewType == "FullStackedSplineArea")
            return new PercentValueToStringConverter();
        if (viewType == "CandleStick" || viewType == "Stock")
            return new FinancialValueToStringConverter();
        if (viewType == "Bubble")
            return new BubbleValueToStringConverter();
        if (viewType == "RangeArea" || viewType == "RangeBar" || viewType == "SideBySideRangeBar" || viewType == "SideBySideRangeBar" || viewType == "Gantt" || viewType == "SideBySideGantt")
            return new RangeValueToStringConverter();
        if (viewType.indexOf("3D") >= 0 || viewType.indexOf("Manhattan") >= 0)
            return null;
        return new ValueToStringConverter();
    },
    GetAxisLabelText: function(axis, isValueAxis, value) {
        parsedPattern = this.SplitString(axis.crosshairAxisLabelOptions.pattern, "{", "}");
        if (parsedPattern != null) {
            var result = axis.crosshairAxisLabelOptions.pattern;
            for (var i = 0; i < parsedPattern.length; i++) {
                var fragment = parsedPattern[i];
                var preparedFragment = this.PrepareFragment(fragment);
                if (preparedFragment == null)
                    return fragment;
                var format = preparedFragment[0];
                var formattedFragment = fragment;
                var pattern = preparedFragment[1];
                if (pattern == _argumentPattern && !isValueAxis || isValueAxis && pattern == _valuePattern) {
                    var isDateTime = axis.scale instanceof ASPxClientDateTimeMap;
                    if (isDateTime || axis.scale instanceof ASPxClientNumericalMap) {
                        formattedFragment = ASPx.Formatter.Format("{0:" + format + "}", [value]);
                    }
                    else {
                        formattedFragment = value;
                    }
                }
                result = result.replace(new RegExp(fragment, "g"), formattedFragment);
            }
            return result;
        }
        return value;
    }
};

ASPx.CrosshairGroupHeaderValueToStringConverter = CrosshairGroupHeaderValueToStringConverter;
ASPx.ToolTipPatternHelper = ToolTipPatternHelper;
})();