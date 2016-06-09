/*! 
* DevExpress Analytics Dashboard
* Version: 15.2.3
* Build date: Dec 1, 2015
*
* Copyright (c) 2012 - 2015 Developer Express Inc. ALL RIGHTS RESERVED
* EULA: http://www.devexpress.com/Support/EULAs/NetComponents.xml
*/

"use strict";
if (!window.DevExpress || !DevExpress.MOD_DASHBOARD) {
    if (!window.DevExpress || !DevExpress.MOD_WIDGETS_WEB)
        throw Error('Required module is not referenced: widgets-web');
    if (!window.DevExpress || !DevExpress.MOD_VIZ_CHARTS)
        throw Error('Required module is not referenced: viz-charts');
    if (!window.DevExpress || !DevExpress.MOD_VIZ_GAUGES)
        throw Error('Required module is not referenced: viz-gauges');
    if (!window.DevExpress || !DevExpress.MOD_VIZ_RANGESELECTOR)
        throw Error('Required module is not referenced: viz-rangeselector');
    if (!window.DevExpress || !DevExpress.MOD_VIZ_VECTORMAP)
        throw Error('Required module is not referenced: viz-vectormap');
    if (!window.DevExpress || !DevExpress.MOD_VIZ_SPARKLINES)
        throw Error('Required module is not referenced: viz-sparklines');
    /*! Module dashboard, file dashboard.js */
    (function(DevExpress) {
        DevExpress.dashboard = {
            viewerItems: {},
            data: {}
        }
    })(DevExpress);
    /*! Module dashboard, file utils.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard;
        dashboard.utils = {
            specialValues: {
                nullValueGuid: 'D86D8A6C-0D87-4CA4-9C15-3356A83699B5',
                othersValueGuid: '5821CCA5-303B-425D-909F-B8373FB7FAE3',
                olapNullValueGuid: '764E2930-72BE-4464-ACB6-4ADB205BD414',
                errorValueGuid: 'D7BB8881-C9F3-45E3-B370-2EA8E836FC5D'
            },
            KpiValueMode: {
                Measure: 'Measure',
                Delta: 'Delta'
            },
            pivotArea: {
                column: 'column',
                row: 'row',
                data: 'data'
            },
            gaugeViewType: {
                CircularFull: 'CircularFull',
                CircularHalf: 'CircularHalf',
                CircularQuarterRight: 'CircularQuarterRight',
                CircularQuarterLeft: 'CircularQuarterLeft',
                CircularThreeFourth: 'CircularThreeFourth',
                LinearHorizontal: 'LinearHorizontal',
                LinearVertical: 'LinearVertical'
            },
            tooltipContainerSelector: '.dx-viewport',
            toColor: function(numericColorValue) {
                var number = numericColorValue >>> 0;
                var b = number & 0xFF,
                    g = (number & 0xFF00) >>> 8,
                    r = (number & 0xFF0000) >>> 16;
                return this.getRGBColor(r, g, b)
            },
            getRGBColor: function(r, g, b) {
                return "rgb(" + [r, g, b].join(",") + ")"
            },
            allowSelectValue: function(values) {
                var result = true;
                if (values)
                    $.each(values, function(_, value) {
                        result = result && value !== dashboard.utils.specialValues.othersValueGuid && value !== dashboard.utils.specialValues.errorValueGuid
                    });
                return result
            },
            cleanNullValues: function(values) {
                if (values) {
                    var i = values.length - 1;
                    while (dashboard.utils.specialValues.olapNullValueGuid == values[i] && i >= 0) {
                        values.splice(i, 1);
                        i--
                    }
                }
            },
            encodeHtml: function(str) {
                return String(str).replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/'/g, '&#39;').replace(/</g, '&lt;').replace(/>/g, '&gt;')
            },
            decodeHtml: function(value) {
                return String(value).replace(/&quot;/g, '"').replace(/&#39;/g, "'").replace(/&lt;/g, '<').replace(/&gt;/g, '>').replace(/&amp;/g, '&')
            },
            moveContent: function($source, $dest, clearSource) {
                $source.contents().appendTo($dest);
                if (clearSource)
                    $source.html('')
            },
            checkValuesAreEqual: function(value1, value2) {
                if (!value1 || !value2)
                    return false;
                var list1 = $.isArray(value1) ? value1 : [value1],
                    list2 = $.isArray(value2) ? value2 : [value2];
                if (list1.length !== list2.length || list1.length === 0)
                    return false;
                else {
                    for (var i = 0; i < list1.length; i++)
                        if (DX.data.utils.toComparable(list1[i], false) !== DX.data.utils.toComparable(list2[i], false))
                            return false;
                    return true
                }
            },
            checkTuplesAreEqual: function(tuple1, tuple2) {
                if (!tuple1 || !tuple2)
                    return false;
                var containsCount = 0;
                $.each(tuple1, function(_, tuple1AxisValue) {
                    var value = $.grep(tuple2, function(tuple2AxisValue) {
                            return tuple2AxisValue.AxisName == tuple1AxisValue.AxisName
                        })[0].Value;
                    if (dashboard.utils.checkValuesAreEqual(tuple1AxisValue.Value, value))
                        containsCount = containsCount + 1
                });
                return containsCount == tuple1.length
            },
            checkArrayContainsTuple: function(array, tuple) {
                var that = this,
                    contains,
                    currentIndex;
                $.each(array, function(index, aTuple) {
                    contains = that.checkTuplesAreEqual(aTuple, tuple);
                    if (contains)
                        currentIndex = index;
                    return !contains
                });
                return currentIndex
            },
            getAxisPointValue: function(tuple, axisName) {
                var axisPoints = $.grep(tuple, function(axisValue) {
                        return axisValue.AxisName == axisName
                    });
                return axisPoints.length > 0 ? axisPoints[0].Value : null
            },
            getTagValue: function(tag) {
                var axisPoint = tag.axisPoint;
                return axisPoint ? axisPoint.getUniquePath() : tag
            },
            getValueIndex: function(matrix, vector) {
                if (matrix && vector)
                    for (var i = 0; i < matrix.length; i++)
                        if (this.checkValuesAreEqual(matrix[i], vector))
                            return i;
                return -1
            },
            treeWalker: function(rootNode, childrenFunc) {
                return {
                        walk: function(func) {
                            this._walkInternal(rootNode, null, func, function() {
                                return true
                            })
                        },
                        walkLeaf: function(func) {
                            this._walkInternal(rootNode, null, func, function(node, parent, isLeaf) {
                                return isLeaf
                            })
                        },
                        _walkInternal: function(node, parent, func, callPredicate) {
                            var that = this,
                                children = childrenFunc(node),
                                isLeaf = !children || children.length === 0;
                            if (callPredicate(node, parent, isLeaf))
                                func(node, parent, isLeaf);
                            if (!isLeaf)
                                $.each(children, function(i, branch) {
                                    that._walkInternal(branch, node, func, callPredicate)
                                })
                        }
                    }
            },
            getParentClasses: function($obj) {
                var parents = [$obj.attr('class')];
                $.each($obj.parents(), function(_, parent) {
                    var name = $(parent).attr('class');
                    if (name)
                        parents.push(name)
                });
                return parents.reverse()
            },
            createFakeObjects: function(classNames, cssOptions) {
                var firstElement,
                    prevElement,
                    currElement;
                $.each(classNames, function(_, name) {
                    currElement = $('<div>', {css: $.extend({
                            position: 'absolute',
                            top: 0,
                            left: 0,
                            visibility: 'hidden',
                            overflow: 'hidden'
                        }, cssOptions)});
                    currElement.appendTo(prevElement ? prevElement : $('body'));
                    currElement.addClass(name);
                    prevElement = currElement;
                    if (!firstElement)
                        firstElement = currElement
                });
                return {
                        firstElement: firstElement,
                        lastElement: currElement,
                        remove: function() {
                            firstElement.remove()
                        }
                    }
            },
            getBorderSizeByClasses: function(classNames) {
                if (classNames && classNames.length > 0) {
                    var fakeObjs = this.createFakeObjects(classNames, {
                            width: 100,
                            height: 100
                        });
                    try {
                        return DX.utils.renderHelper.getActualBorder(fakeObjs.lastElement)
                    }
                    finally {
                        fakeObjs.remove()
                    }
                }
                else
                    return {
                            width: 0,
                            height: 0
                        }
            },
            wrapHash: function(valuesArray) {
                var hash = {};
                if (valuesArray)
                    $.each(valuesArray, function(_, value) {
                        hash[value] = true
                    });
                return hash
            },
            areNotOrderedListsEqual: function(list1, list2) {
                if (list1.length != list2.length)
                    return false;
                list1 = list1.slice();
                list2 = list2.slice();
                list1.sort();
                list2.sort();
                for (var i = 0; i < list1.length; i++)
                    if (list1[i] !== list2[i])
                        return false;
                return true
            },
            pxToNumber: function(px) {
                var result = 0;
                if (px != null && px != "")
                    try {
                        var indexOfPx = px.indexOf("px");
                        if (indexOfPx > -1)
                            result = parseInt(px.substr(0, indexOfPx))
                    }
                    catch(e) {}
                return result
            }
        }
    })(jQuery, DevExpress);
    /*! Module dashboard, file localizer.js */
    (function($, DX, undefined) {
        var formatHelper = DX.require("/utils/utils.formatHelper"),
            commonUtils = DX.require("/utils/utils.common"),
            dashboard = DX.dashboard,
            data = dashboard.data;
        dashboard.localizationId = {
            FilterElementShowAllItem: 'FilterElementShowAllItem',
            DateTimeQuarterFormat: 'DateTimeQuarterFormat',
            buttonNames: {
                ClearMasterFilter: 'ClearMasterFilter',
                ClearSelection: 'ClearSelection',
                ElementSelection: 'ElementSelection',
                DrillUp: 'DrillUp',
                ExportTo: 'ExportTo',
                ExportToPdf: 'ExportToPdf',
                ExportToImage: 'ExportToImage',
                ExportToExcel: 'ExportToExcel',
                ExportTemplate: 'ExportTemplate',
                AllowMultiselection: 'AllowMultiselection',
                ButtonCancel: 'ButtonCancel',
                ButtonOK: 'ButtonOK',
                ButtonReset: 'ButtonReset',
                ButtonSubmit: 'ButtonSubmit',
                ButtonExport: 'ButtonExport',
                ParametersFormCaption: 'ParametersFormCaption',
                InitialExtent: 'InitialExtent',
                GridResetColumnWidths: 'GridResetColumnWidths',
                GridSortAscending: 'GridSortAscending',
                GridSortDescending: 'GridSortDescending',
                GridClearSorting: 'GridClearSorting'
            },
            labelName: {
                PageLayout: 'PageLayout',
                PageLayoutAuto: 'PageLayoutAuto',
                PageLayoutPortrait: 'PageLayoutPortrait',
                PageLayoutLandscape: 'PageLayoutLandscape',
                PaperKind: 'PaperKind',
                PaperKindLetter: 'PaperKindLetter',
                PaperKindLegal: 'PaperKindLegal',
                PaperKindExecutive: 'PaperKindExecutive',
                PaperKindA5: 'PaperKindA5',
                PaperKindA4: 'PaperKindA4',
                PaperKindA3: 'PaperKindA3',
                ScaleMode: 'ScaleMode',
                ScaleModeNone: 'ScaleModeNone',
                ScaleModeUseScaleFactor: 'ScaleModeUseScaleFactor',
                ScaleModeAutoFitToPageWidth: 'ScaleModeAutoFitToPageWidth',
                AutoFitPageCount: 'AutoFitPageCount',
                ScaleFactor: 'ScaleFactor',
                PrintHeadersOnEveryPage: 'PrintHeadersOnEveryPage',
                FitToPageWidth: 'FitToPageWidth',
                SizeMode: 'SizeMode',
                SizeModeNone: 'SizeModeNone',
                SizeModeStretch: 'SizeModeStretch',
                SizeModeZoom: 'SizeModeZoom',
                AutoArrangeContent: 'AutoArrangeContent',
                ImageFormat: 'ImageFormat',
                ExcelFormat: 'ExcelFormat',
                CsvValueSeparator: 'CsvValueSeparator',
                Resolution: 'Resolution',
                ShowTitle: 'ShowTitle',
                Title: 'Title',
                FileName: 'FileName',
                FilterStatePresentation: 'FilterStatePresentation',
                FilterStatePresentationNone: 'FilterStatePresentationNone',
                FilterStatePresentationAfter: 'FilterStatePresentationAfter',
                FilterStatePresentationAfterAndSplitPage: 'FilterStatePresentationAfterAndSplitPage'
            },
            sparkline: {
                TooltipStartValue: 'SparklineTooltipStartValue',
                TooltipEndValue: 'SparklineTooltipEndValue',
                TooltipMinValue: 'SparklineTooltipMinValue',
                TooltipMaxValue: 'SparklineTooltipMaxValue'
            },
            MessageGridHasNoData: 'MessageGridHasNoData',
            MessagePivotHasNoData: 'MessagePivotHasNoData',
            PivotGridGrandTotal: 'PivotGridGrandTotal',
            PivotGridTotal: 'PivotGridTotal',
            ChartTotalValue: 'ChartTotalValue',
            ParametersSelectorText: 'ParametersSelectorText',
            OpenCaption: 'OpenCaption',
            HighCaption: 'HighCaption',
            LowCaption: 'LowCaption',
            CloseCaption: 'CloseCaption',
            NumericFormatUnitSymbolThousands: 'NumericFormatUnitSymbolThousands',
            NumericFormatUnitSymbolMillions: 'NumericFormatUnitSymbolMillions',
            NumericFormatUnitSymbolBillions: 'NumericFormatUnitSymbolBillions'
        };
        data.localizer = {
            _localizationStrings: undefined,
            initialize: function(localizationStrings) {
                this._localizationStrings = localizationStrings;
                var quarterFormat = this.getStringCore(dashboard.localizationId.DateTimeQuarterFormat);
                if (quarterFormat)
                    formatHelper.defaultQuarterFormat = quarterFormat;
                var unitSymbolThousands = this.getStringCore(dashboard.localizationId.NumericFormatUnitSymbolThousands);
                if (unitSymbolThousands)
                    formatHelper.defaultLargeNumberFormatPostfixes[1] = unitSymbolThousands;
                var unitSymbolMillions = this.getStringCore(dashboard.localizationId.NumericFormatUnitSymbolMillions);
                if (unitSymbolMillions)
                    formatHelper.defaultLargeNumberFormatPostfixes[2] = unitSymbolMillions;
                var unitSymbolBillions = this.getStringCore(dashboard.localizationId.NumericFormatUnitSymbolBillions);
                if (unitSymbolBillions)
                    formatHelper.defaultLargeNumberFormatPostfixes[3] = unitSymbolBillions;
                var allElementText = this.getStringCore(dashboard.localizationId.FilterElementShowAllItem);
                if (allElementText)
                    data.ALL_ELEMENT.text = allElementText
            },
            getString: function(key) {
                return this.getStringCore(key, key)
            },
            getPredefinedString: function(value) {
                switch (value) {
                    case dashboard.utils.specialValues.nullValueGuid:
                    case dashboard.utils.specialValues.olapNullValueGuid:
                        return this.getString(dashboard.specialValueNames.NullValue);
                    case dashboard.utils.specialValues.othersValueGuid:
                        return this.getString(dashboard.specialValueNames.OthersValue);
                    case dashboard.utils.specialValues.errorValueGuid:
                        return this.getString(dashboard.specialValueNames.ErrorValue);
                    default:
                        return undefined
                }
            },
            getStringCore: function(key, defaultValue) {
                var value = undefined;
                if (this._localizationStrings) {
                    value = this._localizationStrings[key];
                    return commonUtils.isDefined(value) ? value : defaultValue
                }
                else {
                    value = Globalize.localize(key);
                    return commonUtils.isDefined(value) ? value : defaultValue
                }
            }
        }
    })(jQuery, DevExpress);
    /*! Module dashboard, file formatter.js */
    (function($, DX, undefined) {
        var data = DX.dashboard.data,
            localizer = data.localizer,
            formatHelper = DX.require("/utils/utils.formatHelper"),
            commonUtils = DX.require("/utils/utils.common");
        data.formatter = {
            defaultNumericFormat: {
                format: 'fixedPoint',
                unitPower: 'auto',
                precision: 0,
                significantDigits: 3
            },
            defaultPercentFormat: {
                format: 'percent',
                unitPower: 0,
                precision: 2,
                significantDigits: 0,
                showTrailingZeros: false
            },
            _types: {
                Abbreviated: 'abbr',
                Full: 'full',
                Long: 'long',
                Numeric: 'num',
                Short: 'short',
                TimeOnly: 'timeOnly'
            },
            format: function(value, formatViewModel) {
                var str = this.getPredefinedString(value);
                if (!commonUtils.isDefined(str)) {
                    var numericFormat = formatViewModel.NumericFormat,
                        dateTimeFormat = formatViewModel.DateTimeFormat;
                    if (numericFormat)
                        str = this.formatNumeric(value, numericFormat);
                    else
                        str = !dateTimeFormat ? this.formatObject(value) : this.formatDateTime(value, dateTimeFormat)
                }
                return str
            },
            formatNumeric: function(value, numericFormatViewModel) {
                if (!numericFormatViewModel)
                    return value.toString();
                else {
                    var format = this._convertToNumberFormat(numericFormatViewModel);
                    return formatHelper.format(value, format)
                }
            },
            formatDateTime: function(value, dateFormatViewModel) {
                var format = this._convertToDateFormat(dateFormatViewModel);
                return formatHelper.format(value, format)
            },
            formatObject: function(value) {
                return value == null ? '' : value.toString()
            },
            formatAxisValue: function(value, axisMin, axisMax, isPercentAxis) {
                return formatHelper.format(value, this.getAxisFormat(axisMin, axisMax, isPercentAxis))
            },
            getAxisFormat: function(axisMin, axisMax, isPercentAxis) {
                if (isPercentAxis)
                    return this.defaultPercentFormat;
                return {
                        format: 'fixedPoint',
                        unitPower: this.calculateUnitPower(axisMin, axisMax),
                        precision: 2,
                        significantDigits: 0,
                        showTrailingZeros: false
                    }
            },
            calculateUnitPower: function(axisMin, axisMax) {
                var range = axisMax - axisMin;
                if (range >= 1000000000)
                    return 3;
                if (range >= 1000000)
                    return 2;
                if (range >= 1000)
                    return 1;
                return 0
            },
            getPredefinedString: function(value) {
                return localizer.getPredefinedString(value)
            },
            convertToFormat: function(formatViewModel) {
                var format = null;
                if (formatViewModel) {
                    var numericFormat = formatViewModel.NumericFormat,
                        dateTimeFormat = formatViewModel.DateTimeFormat;
                    if (numericFormat)
                        format = this._convertToNumberFormat(numericFormat);
                    else
                        format = !dateTimeFormat ? null : this._convertToDateFormat(dateTimeFormat)
                }
                return format
            },
            _convertToNumberFormat: function(numericFormatViewModel) {
                var formatInfo = null,
                    formatType = numericFormatViewModel ? numericFormatViewModel.FormatType : undefined,
                    unit = numericFormatViewModel ? numericFormatViewModel.Unit : undefined;
                if (formatType !== 'General') {
                    formatInfo = {};
                    formatInfo.format = this._convertNumericFormat(formatType);
                    formatInfo.currencyCulture = numericFormatViewModel.CurrencyCulture;
                    if (numericFormatViewModel.IncludeGroupSeparator)
                        formatInfo.includeGroupSeparator = numericFormatViewModel.IncludeGroupSeparator;
                    if (numericFormatViewModel.ForcePlusSign)
                        formatInfo.plus = numericFormatViewModel.ForcePlusSign;
                    formatInfo.precision = numericFormatViewModel.Precision;
                    if (unit && unit === "Auto")
                        formatInfo.significantDigits = numericFormatViewModel.SignificantDigits;
                    if (unit && formatType === 'Number' || formatType === 'Currency')
                        formatInfo.unitPower = this._convertNumericUnit(unit)
                }
                return formatInfo
            },
            _convertToDateFormat: function(dateFormatViewModel) {
                if (dateFormatViewModel)
                    switch (this._getSyntheticDateTimeGroupInterval(dateFormatViewModel.GroupInterval, dateFormatViewModel.ExactDateFormat)) {
                        case'MonthYear':
                            return {
                                    format: "monthYear",
                                    dateType: this._types.Full
                                };
                        case'QuarterYear':
                            return {
                                    format: "quarterYear",
                                    dateType: this._types.Full
                                };
                        case'DayMonthYear':
                            return {
                                    format: "dayMonthYear",
                                    dateType: dateFormatViewModel.DateFormat === 'Long' ? this._types.Long : this._types.Short
                                };
                        case'DateHour':
                            if (dateFormatViewModel.DateHourFormat === 'Long')
                                return {
                                        format: "dateHour",
                                        dateType: this._types.Long
                                    };
                            else
                                return {
                                        format: "dateHour",
                                        dateType: dateFormatViewModel.DateHourFormat === 'Short' ? this._types.Short : this._types.TimeOnly
                                    };
                        case'DateHourMinute':
                            if (dateFormatViewModel.DateHourMinuteFormat === 'Long')
                                return {
                                        format: "dateHourMinute",
                                        dateType: this._types.Long
                                    };
                            else
                                return {
                                        format: "dateHourMinute",
                                        dateType: dateFormatViewModel.DateHourMinuteFormat === 'Short' ? this._types.Short : this._types.TimeOnly
                                    };
                        case'DateHourMinuteSecond':
                            if (dateFormatViewModel.DateTimeFormat === 'Long')
                                return {
                                        format: "dateHourMinuteSecond",
                                        dateType: this._types.Long
                                    };
                            else
                                return {
                                        format: "dateHourMinuteSecond",
                                        dateType: dateFormatViewModel.DateTimeFormat === 'Short' ? this._types.Short : this._types.TimeOnly
                                    };
                        case'Year':
                            return {
                                    format: "year",
                                    dateType: dateFormatViewModel.YearFormat === 'Abbreviated' ? this._types.Abbreviated : this._types.Full
                                };
                        case'DateYear':
                            return {
                                    format: "dateYear",
                                    dateType: dateFormatViewModel.YearFormat === 'Abbreviated' ? this._types.Abbreviated : this._types.Full
                                };
                        case'Quarter':
                            if (dateFormatViewModel.QuarterFormat === 'Numeric')
                                return {
                                        format: "quarter",
                                        dateType: this._types.Numeric
                                    };
                            else
                                return {
                                        format: "quarter",
                                        dateType: this._types.Full
                                    };
                        case'Month':
                            if (dateFormatViewModel.MonthFormat === 'Numeric')
                                return {
                                        format: "month",
                                        dateType: this._types.Numeric
                                    };
                            else
                                return {
                                        format: "month",
                                        dateType: dateFormatViewModel.MonthFormat === 'Abbreviated' ? this._types.Abbreviated : this._types.Full
                                    };
                        case'Hour':
                            return {
                                    format: "hour",
                                    dateType: dateFormatViewModel.HourFormat === 'Long' ? this._types.Long : this._types.Short
                                };
                        case'DayOfWeek':
                            if (dateFormatViewModel.DayOfWeekFormat === 'Numeric')
                                return {
                                        format: "dayOfWeek",
                                        dateType: this._types.Numeric
                                    };
                            else
                                return {
                                        format: "dayOfWeek",
                                        dateType: dateFormatViewModel.DayOfWeekFormat === 'Abbreviated' ? this._types.Abbreviated : this._types.Full
                                    };
                        default:
                            return {
                                    format: dateFormatViewModel.GroupInterval.toString(),
                                    dateType: this._types.Numeric
                                }
                    }
                else
                    return null
            },
            _getSyntheticDateTimeGroupInterval: function(groupInterval, exactDateFormat) {
                if (groupInterval != 'None')
                    return groupInterval;
                switch (exactDateFormat) {
                    case'Year':
                        return 'DateYear';
                    case'Quarter':
                        return 'QuarterYear';
                    case'Month':
                        return 'MonthYear';
                    case'Day':
                        return 'DayMonthYear';
                    case'Hour':
                        return 'DateHour';
                    case'Minute':
                        return 'DateHourMinute';
                    case'Second':
                        return 'DateHourMinuteSecond';
                    default:
                        return null
                }
                return null
            },
            _convertNumericFormat: function(formatType) {
                switch (formatType) {
                    case'Number':
                        return 'fixedPoint';
                    case'Currency':
                        return 'currency';
                    case'Scientific':
                        return 'exponential';
                    case'Percent':
                        return 'percent';
                    default:
                        return undefined
                }
            },
            _convertNumericUnit: function(numericUnit) {
                switch (numericUnit) {
                    case'Auto':
                        return "auto";
                    case'Thousands':
                        return 1;
                    case'Millions':
                        return 2;
                    case'Billions':
                        return 3;
                    default:
                        return 0
                }
            }
        }
    })(jQuery, DevExpress);
    /*! Module dashboard, file tagValuesProvider.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data;
        data.tagValuesProvider = {
            getTag: function(listSource, tagDataMembers, rowIndex) {
                var values = null;
                if (tagDataMembers === null)
                    return values;
                values = [];
                for (var i = 0; i < tagDataMembers.length; i++)
                    values.push(listSource.getRowValue(rowIndex, tagDataMembers[i]));
                return this.getTagByValues(values)
            },
            getTagByValues: function(values) {
                if (!values || values.length === 0)
                    return null;
                return values
            }
        }
    })(jQuery, DevExpress);
    /*! Module dashboard, file listSource.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            formatter = dashboard.data.formatter,
            Class = DevExpress.require("/class"),
            data = dashboard.data;
        data.listSource = Class.inherit({
            ctor: function ctor(dataSource, dataMembers) {
                this.dataSource = this._wrapIfRequired(dataSource, dataMembers);
                this.dataMembers = dataMembers;
                if (this.dataSource && this.dataMembers) {
                    this.rowCount = this.dataSource.length;
                    this.columnCount = this.dataMembers.length
                }
                else {
                    this.dataSource = [];
                    this.dataMembers = [];
                    this.rowCount = 0;
                    this.columnCount = 0
                }
            },
            _wrapIfRequired: function(dataSource, dataMembers) {
                var dataRow,
                    dataSourceWrapper = [],
                    isWrapRequired = dataSource && dataSource.length > 0 && $.isArray(dataSource) && $.isArray(dataSource[0]);
                if (isWrapRequired)
                    for (var i = 0; i < dataSource.length; i++) {
                        dataRow = dataSource[i];
                        if (dataMembers && dataRow && dataRow.length === dataMembers.length) {
                            dataSourceWrapper[i] = {};
                            for (var j = 0; j < dataRow.length; j++)
                                dataSourceWrapper[i][dataMembers[j]] = dataRow[j]
                        }
                        else {
                            isWrapRequired = false;
                            break
                        }
                    }
                return isWrapRequired ? dataSourceWrapper : dataSource
            },
            getRowValue: function(rowIndex, dataMember) {
                return this.dataSource[rowIndex][dataMember]
            },
            getFormattedRowValue: function(rowIndex, dataMember, formatInfo) {
                var value = this.getRowValue(rowIndex, dataMember);
                return formatter.formatNumeric(value, formatInfo)
            },
            getFormattedArgumentRowValue: function(rowIndex, dataMember, formatInfo) {
                var value = this.getRowValue(rowIndex, dataMember);
                return formatter.format(value, formatInfo)
            },
            getColumnIndex: function(dataMember) {
                return $.inArray(dataMember, this.dataMembers)
            },
            getRowCount: function() {
                return this.rowCount
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file chartHelper.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data;
        data.chartHelper = {
            SelectionMode: {
                Argument: 'Argument',
                Series: 'Series',
                Points: 'Points'
            },
            ChartLegendInsidePosition: {
                TopLeftVertical: 'TopLeftVertical',
                TopLeftHorizontal: 'TopLeftHorizontal',
                TopCenterVertical: 'TopCenterVertical',
                TopCenterHorizontal: 'TopCenterHorizontal',
                TopRightVertical: 'TopRightVertical',
                TopRightHorizontal: 'TopRightHorizontal',
                BottomLeftVertical: 'BottomLeftVertical',
                BottomLeftHorizontal: 'BottomLeftHorizontal',
                BottomCenterVertical: 'BottomCenterVertical',
                BottomCenterHorizontal: 'BottomCenterHorizontal',
                BottomRightVertical: 'BottomRightVertical',
                BottomRightHorizontal: 'BottomRightHorizontal'
            },
            ChartLegendOutsidePosition: {
                TopLeftVertical: 'TopLeftVertical',
                TopLeftHorizontal: 'TopLeftHorizontal',
                TopCenterHorizontal: 'TopCenterHorizontal',
                TopRightVertical: 'TopRightVertical',
                TopRightHorizontal: 'TopRightHorizontal',
                BottomLeftVertical: 'BottomLeftVertical',
                BottomLeftHorizontal: 'BottomLeftHorizontal',
                BottomCenterHorizontal: 'BottomCenterHorizontal',
                BottomRightVertical: 'BottomRightVertical',
                BottomRightHorizontal: 'BottomRightHorizontal'
            },
            convertSeriesType: function(viewSeriesType) {
                switch (viewSeriesType) {
                    case'Bar':
                        return 'bar';
                    case'StackedBar':
                        return 'stackedbar';
                    case'FullStackedBar':
                        return 'fullstackedbar';
                    case'Point':
                        return 'scatter';
                    case'Line':
                        return 'line';
                    case'StackedLine':
                        return 'stackedline';
                    case'FullStackedLine':
                        return 'fullstackedline';
                    case'StepLine':
                        return 'stepline';
                    case'Spline':
                        return 'spline';
                    case'Area':
                        return 'area';
                    case'StackedArea':
                        return 'stackedarea';
                    case'FullStackedArea':
                        return 'fullstackedarea';
                    case'StepArea':
                        return 'steparea';
                    case'SplineArea':
                        return 'splinearea';
                    case'StackedSplineArea':
                        return 'stackedSplineArea';
                    case'FullStackedSplineArea':
                        return 'fullStackedSplineArea';
                    case'SideBySideRangeBar':
                        return 'rangebar';
                    case'RangeArea':
                        return 'rangearea';
                    case'CandleStick':
                        return 'candlestick';
                    case'Stock':
                        return 'stock';
                    case'Donut':
                        return 'doughnut';
                    case'Pie':
                        return 'pie';
                    case'HighLowClose':
                        return 'stock';
                    case'Weighted':
                        return 'bubble';
                    default:
                        return 'area'
                }
            },
            convertPresentationUnit: function(argumentViewModel) {
                if (argumentViewModel && argumentViewModel.DataType === 'DateTime')
                    switch (argumentViewModel.DateTimePresentationUnit) {
                        case'Second':
                            return 'second';
                        case'Minute':
                            return 'minute';
                        case'Hour':
                            return 'hour';
                        case'Day':
                            return 'day';
                        case'Month':
                            return 'month';
                        case'Quarter':
                            return 'quarter';
                        default:
                            return null
                    }
                return null
            },
            convertLegendInsidePosition: function(position) {
                var legendPosition = this.ChartLegendInsidePosition;
                switch (position) {
                    case legendPosition.TopLeftVertical:
                        return {
                                verticalAlignment: 'top',
                                horizontalAlignment: 'left',
                                orientation: 'vertical'
                            };
                    case legendPosition.TopLeftHorizontal:
                        return {
                                verticalAlignment: 'top',
                                horizontalAlignment: 'left',
                                orientation: 'horizontal'
                            };
                    case legendPosition.TopCenterVertical:
                        return {
                                verticalAlignment: 'top',
                                horizontalAlignment: 'center',
                                orientation: 'vertical'
                            };
                    case legendPosition.TopCenterHorizontal:
                        return {
                                verticalAlignment: 'top',
                                horizontalAlignment: 'center',
                                orientation: 'horizontal'
                            };
                    case legendPosition.TopRightVertical:
                        return {
                                verticalAlignment: 'top',
                                horizontalAlignment: 'right',
                                orientation: 'vertical'
                            };
                    case legendPosition.TopRightHorizontal:
                        return {
                                verticalAlignment: 'top',
                                horizontalAlignment: 'right',
                                orientation: 'horizontal'
                            };
                    case legendPosition.BottomLeftVertical:
                        return {
                                verticalAlignment: 'bottom',
                                horizontalAlignment: 'left',
                                orientation: 'vertical'
                            };
                    case legendPosition.BottomLeftHorizontal:
                        return {
                                verticalAlignment: 'bottom',
                                horizontalAlignment: 'left',
                                orientation: 'horizontal'
                            };
                    case legendPosition.BottomCenterVertical:
                        return {
                                verticalAlignment: 'bottom',
                                horizontalAlignment: 'center',
                                orientation: 'vertical'
                            };
                    case legendPosition.BottomCenterHorizontal:
                        return {
                                verticalAlignment: 'bottom',
                                horizontalAlignment: 'center',
                                orientation: 'horizontal'
                            };
                    case legendPosition.BottomRightVertical:
                        return {
                                verticalAlignment: 'bottom',
                                horizontalAlignment: 'right',
                                orientation: 'vertical'
                            };
                    case legendPosition.BottomRightHorizontal:
                        return {
                                verticalAlignment: 'bottom',
                                horizontalAlignment: 'right',
                                orientation: 'horizontal'
                            };
                    default:
                        return {
                                verticalAlignment: 'top',
                                horizontalAlignment: 'center',
                                orientation: 'horizontal'
                            }
                }
            },
            convertLegendOutsidePosition: function(position) {
                var legendPosition = this.ChartLegendOutsidePosition;
                switch (position) {
                    case legendPosition.TopLeftVertical:
                        return {
                                verticalAlignment: 'top',
                                horizontalAlignment: 'left',
                                orientation: 'vertical'
                            };
                    case legendPosition.TopLeftHorizontal:
                        return {
                                verticalAlignment: 'top',
                                horizontalAlignment: 'left',
                                orientation: 'horizontal'
                            };
                    case legendPosition.TopCenterHorizontal:
                        return {
                                verticalAlignment: 'top',
                                horizontalAlignment: 'center',
                                orientation: 'horizontal'
                            };
                    case legendPosition.TopRightVertical:
                        return {
                                verticalAlignment: 'top',
                                horizontalAlignment: 'right',
                                orientation: 'vertical'
                            };
                    case legendPosition.TopRightHorizontal:
                        return {
                                verticalAlignment: 'top',
                                horizontalAlignment: 'right',
                                orientation: 'horizontal'
                            };
                    case legendPosition.BottomLeftVertical:
                        return {
                                verticalAlignment: 'bottom',
                                horizontalAlignment: 'left',
                                orientation: 'vertical'
                            };
                    case legendPosition.BottomLeftHorizontal:
                        return {
                                verticalAlignment: 'bottom',
                                horizontalAlignment: 'left',
                                orientation: 'horizontal'
                            };
                    case legendPosition.BottomCenterHorizontal:
                        return {
                                verticalAlignment: 'bottom',
                                horizontalAlignment: 'center',
                                orientation: 'horizontal'
                            };
                    case legendPosition.BottomRightVertical:
                        return {
                                verticalAlignment: 'bottom',
                                horizontalAlignment: 'right',
                                orientation: 'vertical'
                            };
                    case legendPosition.BottomRightHorizontal:
                        return {
                                verticalAlignment: 'bottom',
                                horizontalAlignment: 'right',
                                orientation: 'horizontal'
                            };
                    default:
                        return {
                                verticalAlignment: 'top',
                                horizontalAlignment: 'center',
                                orientation: 'horizontal'
                            }
                }
            },
            convertPointLabelRotationAngle: function(orientation) {
                switch (orientation) {
                    case"RotateRight":
                        return 90;
                    case"RotateLeft":
                        return 270;
                    default:
                        return 0
                }
            },
            convertPointLabelPosition: function(position) {
                if (position == "Inside")
                    return "inside";
                else
                    return "outside"
            },
            allowArgumentAxisMargins: function(panes) {
                var seriesType = undefined,
                    seriesEqual = true,
                    marginsArgsEnabled;
                if (panes.length == 1) {
                    $.each(panes[0].SeriesTemplates, function(index, seriesTemplate) {
                        seriesType = seriesType == undefined ? seriesTemplate.SeriesType : seriesType;
                        seriesEqual = seriesEqual && seriesType === seriesTemplate.SeriesType
                    });
                    marginsArgsEnabled = !(seriesEqual && $.inArray(seriesType, ['Area', 'StackedArea', 'FullStackedArea', 'StepArea', 'SplineArea', 'StackedSplineArea', 'RangeArea', 'FullStackedSplineArea']) != -1)
                }
                else
                    marginsArgsEnabled = true;
                return marginsArgsEnabled
            },
            isFinancialType: function(type) {
                switch (type) {
                    case'candlestick':
                    case'stock':
                        return true;
                    default:
                        return false
                }
            },
            isTransparentColorType: function(type) {
                switch (type) {
                    case'area':
                    case'steparea':
                    case'splinearea':
                    case'rangearea':
                    case'stackedarea':
                    case'fullstackedarea':
                    case'bubble':
                        return true;
                    default:
                        return false
                }
            },
            isSeriesColorSupported: function(type) {
                switch (type) {
                    case'line':
                    case'stackedLine':
                    case'fullstackedLine':
                    case'stepline':
                    case'spline':
                    case'area':
                    case'fullstackedarea':
                    case'spline':
                    case'stackedarea':
                    case'stackedsplineArea':
                    case'steparea':
                    case'rangearea':
                        return true;
                    default:
                        return false
                }
            }
        }
    })(jQuery, DevExpress);
    /*! Module dashboard, file selectionHelper.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data;
        data.selectionHelper = {
            setSelectedArguments: function(widget, values, state) {
                if (!values)
                    return;
                for (var valueIndex = 0; valueIndex < values.length; valueIndex++)
                    this._selectArgument(widget, values[valueIndex], state)
            },
            setSelectedSeries: function(widget, values, state) {
                if (!values)
                    return;
                for (var valueIndex = 0; valueIndex < values.length; valueIndex++)
                    this._selectSeries(widget, values[valueIndex], state)
            },
            setSelectedPoint: function(widget, seriesValue, argumentValue, state) {
                var that = this;
                if (seriesValue != null && argumentValue == null)
                    that._selectSeries(widget, seriesValue, state);
                if (seriesValue == null && argumentValue != null)
                    that._selectArgument(widget, argumentValue, state);
                if (seriesValue != null && argumentValue != null) {
                    var seriesList = widget.getAllSeries();
                    for (var i = 0; i < seriesList.length; i++)
                        if (that._checkWidgetCorrespondsToValue(seriesList[i], seriesValue))
                            this._selectSeriesPoints(seriesList[i], argumentValue, state)
                }
            },
            setSelectedWidgetViewer: function(widget, values, state) {
                if (!values)
                    return;
                for (var valueIndex = 0; valueIndex < values.length; valueIndex++)
                    this._selectValue(widget, values[valueIndex], state)
            },
            selectWholePie: function(widgetViewer, state) {
                var seriesList = widgetViewer.getAllSeries();
                for (var i = 0; i < seriesList.length; i++)
                    this._selectWidget(seriesList[i], state)
            },
            _selectSeries: function(widget, seriesValue, state) {
                var seriesList = widget.getAllSeries();
                for (var i = 0; i < seriesList.length; i++)
                    this._selectValue(seriesList[i], seriesValue, state)
            },
            _selectArgument: function(widget, argumentValue, state) {
                var seriesList = widget.getAllSeries();
                for (var i = 0; i < seriesList.length; i++)
                    this._selectSeriesPoints(seriesList[i], argumentValue, state)
            },
            _selectSeriesPoints: function(series, argumentValue, state) {
                var points = series.getAllPoints();
                for (var j = 0; j < points.length; j++)
                    this._selectValue(points[j], argumentValue, state)
            },
            _selectValue: function(widget, value, state) {
                if (this._checkWidgetCorrespondsToValue(widget, value))
                    this._selectWidget(widget, state)
            },
            _selectWidget: function(widget, state) {
                if (state)
                    widget.select();
                else
                    widget.clearSelection()
            },
            _checkWidgetCorrespondsToValue: function(widget, value) {
                var tag = widget.tag;
                if (!tag || !value)
                    return false;
                if (tag)
                    tag = dashboard.utils.getTagValue(tag);
                if (tag && !$.isArray(tag) && !$.isArray(value))
                    throw Error("Internal Error: incorrect values for selection");
                return dashboard.utils.checkValuesAreEqual(tag, value)
            }
        }
    })(jQuery, DevExpress);
    /*! Module dashboard, file tuple.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            Class = DevExpress.require("/class"),
            data = dashboard.data;
        data.tuple = Class.inherit({
            ctor: function ctor(axisPoints) {
                var that = this;
                that.axisPoints = axisPoints
            },
            getAxisPoint: function(axisName) {
                var that = this;
                return $.grep(that.axisPoints, function(axisPoint) {
                        return axisPoint.AxisName == axisName
                    })[0]
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file parameters.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            Class = DevExpress.require("/class"),
            data = dashboard.data;
        data.parameter = Class.inherit({
            ctor: function ctor(parameterViewModel) {
                var that = this;
                that._name = parameterViewModel.Name;
                that._value = parameterViewModel.DefaultValue;
                that._defaultValue = parameterViewModel.DefaultValue;
                that._description = parameterViewModel.Description;
                that._type = parameterViewModel.Type;
                that._visible = parameterViewModel.Visible;
                that._allowNull = parameterViewModel.AllowNull;
                that._allowmultiselect = parameterViewModel.AllowMultiselect;
                that._values = [];
                if (parameterViewModel.Values)
                    $.each(parameterViewModel.Values, function(index, value) {
                        that._values.push(new dashboard.data.parameterValue(value.Value, value.DisplayText, parameterViewModel.ContainsDisplayMember))
                    });
                this.parameterChanged = new $.Callbacks
            },
            getName: function() {
                return this._name
            },
            getAllowNull: function() {
                return this._allowNull
            },
            getAllowMultiselect: function() {
                return this._allowmultiselect
            },
            getValue: function() {
                return this._value
            },
            setValue: function(value) {
                this._value = value;
                this.parameterChanged.fire()
            },
            getDefaultValue: function() {
                return this._defaultValue
            },
            getDescription: function() {
                return this._description
            },
            getType: function() {
                return this._type
            },
            getValues: function() {
                return this._values
            },
            isVisible: function() {
                return this._visible
            }
        });
        data.parameterValue = Class.inherit({
            ctor: function ctor(value, displayText, containsDisplayMember) {
                var that = this;
                that._value = value;
                that._displayText = containsDisplayMember ? displayText : value
            },
            getValue: function() {
                return this._value
            },
            getDisplayText: function() {
                return this._displayText
            }
        });
        data.parametersCollection = Class.inherit({
            ctor: function ctor(parametersViewModel) {
                var that = this;
                that._parameters = [];
                if (parametersViewModel)
                    $.each(parametersViewModel, function(index, parameterViewModel) {
                        var parameter = new dashboard.data.parameter(parameterViewModel);
                        parameter.parameterChanged.add(function() {
                            that.collectionChanged.fire()
                        });
                        that._parameters.push(parameter)
                    });
                this.collectionChanged = new $.Callbacks
            },
            setParameters: function(newParameters) {
                var that = this;
                $.each(newParameters, function(index, newParameter) {
                    var parameter = that.getParameterByName(newParameter.Name);
                    parameter.setValue(newParameter.Value)
                })
            },
            getParameterValues: function() {
                var that = this,
                    parameterValues = [];
                $.each(that._parameters, function(index, parameter) {
                    parameterValues.push({
                        Name: parameter.getName(),
                        Value: parameter.getValue()
                    })
                });
                return parameterValues
            },
            getParameterDefaultValue: function(name) {
                return this.getParameterByName(name).getDefaultValue()
            },
            getParameterValue: function(name) {
                return this.getParameterByName(name).getValue()
            },
            setParameterValue: function(name, value) {
                var parameter = this.getParameterByName(name);
                parameter.setValue(value)
            },
            getVisibleParameters: function() {
                return $.grep(this._parameters, function(parameter, index) {
                        return parameter.isVisible()
                    })
            },
            getParameters: function() {
                return this._parameters
            },
            getParameterByName: function(name) {
                var that = this;
                return $.grep(that._parameters, function(parameter) {
                        return parameter.getName() == name
                    })[0]
            },
            getParameterByIndex: function(index) {
                return this._parameters[index]
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file factory.js */
    (function($, DX, undefined) {
        var ui = DX.ui,
            dashboard = DX.dashboard,
            data = dashboard.data,
            viewerItems = dashboard.viewerItems;
        viewerItems.types = {
            group: 'GROUP',
            rangeFilter: 'RANGEFILTER',
            chart: 'CHART',
            scatter: 'SCATTERCHART',
            pie: 'PIE',
            card: 'CARD',
            grid: 'GRID',
            pivot: 'PIVOT',
            gauge: 'GAUGE',
            text: 'TEXT',
            image: 'IMAGE',
            map: 'MAP',
            choroplethMap: 'CHOROPLETHMAP',
            geoPointMap: 'GEOPOINTMAP',
            bubbleMap: 'BUBBLEMAP',
            pieMap: 'PIEMAP',
            comboBox: 'COMBOBOX',
            listBox: 'LISTBOX',
            treeView: 'TREEVIEW'
        };
        data.factory = function() {
            var createItem = function($container, options) {
                    switch (options.Type) {
                        case viewerItems.types.group:
                            return new viewerItems.groupItem($container, options);
                        case viewerItems.types.rangeFilter:
                            return new viewerItems.rangeSelectorItem($container, options);
                        case viewerItems.types.chart:
                        case viewerItems.types.scatter:
                            return new viewerItems.chartItem($container, options);
                        case viewerItems.types.pie:
                            return new viewerItems.pieItem($container, options);
                        case viewerItems.types.card:
                            return new viewerItems.cardsItem($container, options);
                        case viewerItems.types.grid:
                            return new viewerItems.dataGridItem($container, options);
                        case viewerItems.types.pivot:
                            return new viewerItems.pivotGridItem($container, options);
                        case viewerItems.types.gauge:
                            return new viewerItems.gaugesItem($container, options);
                        case viewerItems.types.text:
                            return new viewerItems.textItem($container, options);
                        case viewerItems.types.image:
                            return new viewerItems.imageItem($container, options);
                        case viewerItems.types.map:
                        case viewerItems.types.choroplethMap:
                            return new viewerItems.choroplethMapItem($container, options);
                        case viewerItems.types.geoPointMap:
                            return new viewerItems.geoPointMapItem($container, options);
                        case viewerItems.types.bubbleMap:
                            return new viewerItems.bubbleMapItem($container, options);
                        case viewerItems.types.pieMap:
                            return new viewerItems.pieMapItem($container, options);
                        case viewerItems.types.comboBox:
                            return new viewerItems.comboBoxFilterElement($container, options);
                        case viewerItems.types.listBox:
                            return new viewerItems.listFilterElement($container, options);
                        case viewerItems.types.treeView:
                            return new viewerItems.treeViewFilterElement($container, options);
                        default:
                            return {}
                    }
                };
            var createDataController = function(type, options) {
                    switch (type) {
                        case viewerItems.types.rangeFilter:
                            return new data.rangeFilterDataController(options);
                        case viewerItems.types.chart:
                            return new data.chartDataController(options);
                        case viewerItems.types.scatter:
                            return new data.scatterChartDataController(options);
                        case viewerItems.types.pie:
                            return new data.pieDataController(options);
                        case viewerItems.types.pivot:
                            return new data.pivotDataController(options);
                        case viewerItems.types.choroplethMap:
                            return new data.choroplethMapDataController(options);
                        case viewerItems.types.grid:
                            return new data.gridDataController(options);
                        case viewerItems.types.card:
                            return new data.cardDataController(options);
                        case viewerItems.types.gauge:
                            return new data.gaugeDataController(options);
                        case viewerItems.types.geoPointMap:
                            return new data.geoPointMapDataController(options);
                        case viewerItems.types.bubbleMap:
                            return new data.bubbleMapDataController(options);
                        case viewerItems.types.pieMap:
                            return new data.pieMapDataController(options);
                        case viewerItems.types.comboBox:
                            return new data.comboBoxDataController(options);
                        case viewerItems.types.listBox:
                            return new data.listBoxDataController(options);
                        case viewerItems.types.treeView:
                            return new data.treeViewDataController(options);
                        default:
                            return undefined
                    }
                };
            return {
                    createItem: createItem,
                    createDataController: createDataController
                }
        }()
    })(jQuery, DevExpress);
    /*! Module dashboard, file gridBarCalculator.js */
    (function($, DX, undefined) {
        var Class = DevExpress.require("/class"),
            dashboard = DX.dashboard,
            data = dashboard.data,
            startPercent = 0.15;
        data.gridBarCalculator = Class.inherit({
            ctor: function ctor(options) {
                this._valueItems = [];
                this._alwaysShowZeroLevel = options.showZeroLevel
            },
            addValue: function(valueItem) {
                this._valueItems.push(valueItem);
                this._initialized = false;
                this._invalidate()
            },
            getNormalizedValue: function(index) {
                if (!this._initialized)
                    this._initialize();
                if (this._normalizedValues[index])
                    return this._normalizedValues[index];
                var normalizedValue = this._normalizeValue(this._valueItems[index].getValue());
                this._normalizedValues[index] = normalizedValue;
                return normalizedValue
            },
            getZeroPosition: function() {
                if (!this._initialized)
                    this._initialize();
                return this._zeroPosition
            },
            _invalidate: function() {
                this._normalizedValues = [];
                this._range = null;
                this._min = null;
                this._max = null;
                this._zeroPosition = null;
                this._normalizationData = null;
                this._initialized = false
            },
            _normalizeValue: function(value) {
                var showZero = this._normalizationData.showZero,
                    minimum = this._normalizationData.minimum,
                    ratio = this._normalizationData.ratio,
                    range = this._normalizationData.range,
                    sign = value >= 0 ? 1 : -1;
                return showZero || ratio === 0 ? value / range : sign * (startPercent + ratio * (Math.abs(value) - minimum))
            },
            _calcMinMax: function() {
                var values = [];
                $.each(this._valueItems, function(_, item) {
                    values.push(item.getValue())
                });
                this._min = Math.min.apply(Math, values);
                this._max = Math.max.apply(Math, values)
            },
            _calcRange: function() {
                var min = this._min,
                    minAbs = Math.abs(min),
                    max = this._max,
                    maxAbs = Math.abs(max);
                this._range = Math.max(max - min, minAbs, maxAbs)
            },
            _calcZeroPosition: function() {
                var min = this._min,
                    minAbs = Math.abs(min),
                    max = this._max,
                    range = this._range;
                if (min < 0)
                    if (max < 0)
                        this._zeroPosition = 1;
                    else
                        this._zeroPosition = range !== 0 ? minAbs / range : 0;
                else
                    this._zeroPosition = 0
            },
            _calcNormalizationData: function() {
                var range = this._range || 1,
                    min = this._min,
                    max = this._max,
                    equalSign = min < 0 && max < 0 || min >= 0 && max >= 0,
                    minAbs = Math.abs(min),
                    maxAbs = Math.abs(max),
                    minimum = Math.min(minAbs, maxAbs),
                    maximum = Math.max(minAbs, maxAbs),
                    delta = maximum - minimum,
                    ratio = delta !== 0 ? (1 - startPercent) / delta : 0,
                    showZero = this._alwaysShowZeroLevel || equalSign && minimum / maximum <= startPercent;
                this._normalizationData = {
                    showZero: showZero,
                    minimum: minimum,
                    ratio: ratio,
                    range: range
                }
            },
            _initialize: function() {
                this._calcMinMax();
                this._calcRange();
                this._calcZeroPosition();
                this._calcNormalizationData();
                this._initialized = true
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file gaugeRangeCalculator.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data,
            utils = dashboard.utils,
            Class = DevExpress.require("/class");
        data.gaugeRangeCalculator = Class.inherit({
            ctor: function ctor(options) {
                this._values = options.values;
                this._gaugeViewType = options.gaugeModel.Type;
                this._customMin = options.gaugeModel.MinValue;
                this._customMax = options.gaugeModel.MaxValue;
                this._minDefined = !!this._customMin;
                this._maxDefined = !!this._customMax;
                this._minTickCount = undefined;
                this._maxTickCount = undefined;
                this._min = undefined;
                this._max = undefined;
                this._equalSign = undefined;
                this._defineMinMaxTicks()
            },
            getGaugeRange: function() {
                this._defineMinMax();
                this._setRangeStart();
                this._extendRange();
                var left = Math.min(this._min, this._max),
                    right = Math.max(this._min, this._max),
                    rangeLength = right - left,
                    scaleReversed = this._min > this._max,
                    majorTickCount,
                    minorTickCount,
                    stepCount,
                    step,
                    delta,
                    fit,
                    currentStep,
                    currentDelta,
                    currentFit;
                if (rangeLength === 0) {
                    majorTickCount = 1;
                    minorTickCount = 0
                }
                else {
                    stepCount = this._minTickCount - 1;
                    step = this._chooseMultiplier(rangeLength / stepCount);
                    delta = step * stepCount - rangeLength;
                    fit = this._isFit(left, right, step, stepCount);
                    for (var i = stepCount + 1; i < this._maxTickCount; i++) {
                        currentStep = this._chooseMultiplier(rangeLength / i);
                        currentDelta = currentStep * i - rangeLength;
                        currentFit = this._isFit(left, right, currentStep, i);
                        if (currentFit && (currentDelta < delta || !fit)) {
                            delta = currentDelta;
                            step = currentStep;
                            fit = currentFit;
                            stepCount = i
                        }
                    }
                    left = this._getLeft(left, step);
                    right = this._getRight(right, step);
                    this._min = !scaleReversed ? left : right;
                    this._max = !scaleReversed ? right : left;
                    majorTickCount = stepCount + 1;
                    if (step % 5 === 0)
                        minorTickCount = 4;
                    else if (step % 3 === 0)
                        minorTickCount = 2;
                    else
                        minorTickCount = 3;
                    return {
                            minorTickCount: minorTickCount,
                            majorTickCount: majorTickCount,
                            min: this._min,
                            max: this._max
                        }
                }
            },
            _getLeft: function(left, step) {
                var sign = left > 0 ? 1 : -1;
                if (this._equalSign && left > 0)
                    return Math.floor(Math.abs(left) / step) * step * sign;
                else
                    return Math.ceil(Math.abs(left) / step) * step * sign
            },
            _getRight: function(right, step) {
                var sign = right > 0 ? 1 : -1;
                if (this._equalSign && right < 0)
                    return Math.floor(Math.abs(right) / step) * step * sign;
                else
                    return Math.ceil(Math.abs(right) / step) * step * sign
            },
            _isFit: function(left, right, step, tickCount) {
                var leftAbs = Math.abs(left),
                    rigthAbs = Math.abs(right),
                    isFit = false;
                if (!this._signsEqual(left, right))
                    isFit = Math.ceil(leftAbs / step) + Math.ceil(rigthAbs / step) <= tickCount;
                else {
                    var minAbs = Math.min(leftAbs, rigthAbs),
                        maxAbs = Math.max(leftAbs, rigthAbs);
                    isFit = Math.ceil(maxAbs / step) - Math.floor(minAbs / step) <= tickCount
                }
                return isFit
            },
            _extendRange: function() {
                var that = this,
                    extendMin = function(coef) {
                        if (!that._minDefined)
                            that._min *= coef
                    },
                    extendMax = function(coef) {
                        if (!that._maxDefined)
                            that._max *= coef
                    };
                if (this._equalSign)
                    if (Math.abs(this._min) < Math.abs(this._max)) {
                        extendMin(0.95);
                        extendMax(1.05)
                    }
                    else {
                        extendMin(1.05);
                        extendMax(0.95)
                    }
                else {
                    extendMin(1.05);
                    extendMax(1.05)
                }
                if (this._min === this._max)
                    if (this._min !== 0)
                        this._max *= 1.4;
                    else
                        this._max = 1
            },
            _setRangeStart: function() {
                if (this._equalSign) {
                    if (this._min === this._max)
                        if (this._min > 0) {
                            if (!this._minDefined)
                                this._min = 0
                        }
                        else if (!this._maxDefined)
                            this._max = 0;
                    if (Math.abs(this._min) <= Math.abs(this._max)) {
                        if (!this._minDefined)
                            this._min = 0
                    }
                    else if (!this._maxDefined)
                        this._max = 0
                }
            },
            _defineMinMax: function() {
                if (this._minDefined)
                    this._min = this._customMin;
                else
                    this._min = this._values.length > 0 ? Math.min.apply(Math, this._values) : 0;
                if (this._maxDefined)
                    this._max = this._customMax;
                else
                    this._max = this._values.length > 0 ? Math.max.apply(Math, this._values) : 1;
                this._equalSign = this._signsEqual(this._min, this._max)
            },
            _signsEqual: function(number1, number2) {
                return number1 >= 0 && number2 >= 0 || number1 < 0 && number2 < 0
            },
            _defineMinMaxTicks: function() {
                switch (this._gaugeViewType) {
                    case utils.gaugeViewType.CircularFull:
                        this._minTickCount = 6;
                        this._maxTickCount = 9;
                        break;
                    case utils.gaugeViewType.CircularHalf:
                    case utils.gaugeViewType.CircularThreeFourth:
                        this._minTickCount = 4;
                        this._maxTickCount = 6;
                        break;
                    case utils.gaugeViewType.LinearHorizontal:
                        this._minTickCount = 3;
                        this._maxTickCount = 3;
                        break;
                    default:
                        this._minTickCount = 4;
                        this._maxTickCount = 5;
                        break
                }
            },
            _chooseMultiplier: function(delta) {
                var multipliers = [1, 2, 3, 5],
                    result,
                    exp,
                    scale,
                    normDelta,
                    newResult,
                    i;
                if (delta > 1)
                    for (var factor = 1; ; factor *= 10)
                        for (i = 0; i < multipliers.length; i++) {
                            result = multipliers[i] * factor;
                            if (delta <= result)
                                return result
                        }
                else {
                    result = 10;
                    exp = Math.floor(Math.log(Math.abs(delta)) / Math.LN10);
                    scale = Math.pow(10, -exp);
                    normDelta = delta * scale;
                    for (i = multipliers.length - 1; i >= 0; i--) {
                        newResult = multipliers[i];
                        if (normDelta > newResult)
                            break;
                        result = newResult
                    }
                    return result / scale
                }
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file dataControllerBase.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            Class = DevExpress.require("/class"),
            data = dashboard.data,
            localizer = data.localizer,
            dxUtils = DX.utils;
        data.dataController = {DATA_POSTFIX: '_Data'};
        data.dataControllerBase = Class.inherit({
            ctor: function ctor(options) {
                this.deltaIndicatorTypes = ["none", "up", "down", "warning"];
                this.multiData = options ? options.multiData : undefined;
                this.viewModel = options ? options.viewModel : undefined;
                this.cfModel = options ? options.cfModel : undefined;
                this.drillDownState = options ? options.drillDownState : undefined
            },
            isMultiselectable: function() {
                return false
            },
            getTitle: function(axisPoint, separator) {
                var DEFAULT_SUBTITLE_SEPARATOR = ' - ',
                    axisName = axisPoint.getAxisName();
                if (this.drillDownState[axisName])
                    return axisPoint.getDisplayText();
                else
                    return axisPoint.getDisplayPath().reverse().join(separator ? separator : DEFAULT_SUBTITLE_SEPARATOR)
            },
            _getMeasureValueByAxisPoints: function(axisPoints, cfMeasureId) {
                var slice = this._getSlice(axisPoints);
                return slice.getConditionalFormattingMeasureValue(cfMeasureId)
            },
            _getSlice: function(axisPoints) {
                var slice = this.multiData;
                $.each(axisPoints, function(_, axisPoint) {
                    slice = slice.getSlice(axisPoint)
                });
                return slice
            },
            _getZeroPosition: function(zeroPositionMeasureId, columnAxisName, rowAxisName) {
                var that = this,
                    currentZeroPosition,
                    zeroPosition,
                    columnRootPoint,
                    rowRootPoint;
                columnRootPoint = that.multiData.getAxis(columnAxisName).getRootPoint();
                rowRootPoint = that.multiData.getAxis(rowAxisName).getRootPoint();
                currentZeroPosition = that._getMeasureValueByAxisPoints([columnRootPoint, rowRootPoint], zeroPositionMeasureId);
                if (currentZeroPosition !== undefined && currentZeroPosition !== null)
                    zeroPosition = currentZeroPosition;
                return zeroPosition
            },
            _getStyleSettingsInfoCore: function(cellInfo, rules, columnAxisName, rowAxisName) {
                var that = this,
                    currentStyleIndexes = [],
                    uniqueIndexes = [],
                    styleAndRuleMappingTable = {},
                    ruleIndex,
                    currentNormalizedValue,
                    normalizedValue,
                    zeroPosition,
                    styleSettingsInfo,
                    points = [];
                if (rules.length > 0)
                    $.each(rules, function(_, rule) {
                        currentStyleIndexes = that._getStyleIndexes(rule, cellInfo, points);
                        if (currentStyleIndexes.length > 0) {
                            if (!Array.prototype.indexOf)
                                Array.prototype.indexOf = function(obj, start) {
                                    for (var i = start || 0, j = this.length; i < j; i++)
                                        if (this[i] === obj)
                                            return i;
                                    return -1
                                };
                            ruleIndex = that.cfModel.RuleModels.indexOf(rule);
                            $.each(currentStyleIndexes, function(_, styleIndex) {
                                if (uniqueIndexes[styleIndex] === undefined) {
                                    uniqueIndexes.push(styleIndex);
                                    styleAndRuleMappingTable[styleIndex] = ruleIndex
                                }
                            })
                        }
                        currentNormalizedValue = that._getMeasureValueByAxisPoints(points, rule.NormalizedValueMeasureId);
                        if (currentNormalizedValue !== undefined && currentNormalizedValue !== null) {
                            normalizedValue = currentNormalizedValue;
                            zeroPosition = that._getZeroPosition(rule.ZeroPositionMeasureId, columnAxisName, rowAxisName)
                        }
                    });
                styleSettingsInfo = {
                    styleIndexes: uniqueIndexes,
                    styleAndRuleMappingTable: styleAndRuleMappingTable
                };
                if (normalizedValue !== undefined && zeroPosition !== undefined) {
                    styleSettingsInfo.normalizedValue = normalizedValue;
                    styleSettingsInfo.zeroPosition = zeroPosition
                }
                return styleSettingsInfo
            },
            _generateSparklineOptions: function(data, options, format) {
                return {
                        dataSource: data,
                        type: options.ViewType.toLowerCase(),
                        onIncidentOccurred: dxUtils.renderHelper.widgetIncidentOccurred,
                        showMinMax: options.HighlightMinMaxPoints,
                        showFirstLast: options.HighlightStartEndPoints,
                        tooltip: {
                            _justify: true,
                            container: dashboard.utils.tooltipContainerSelector,
                            customizeTooltip: function() {
                                var startText = localizer.getString(dashboard.localizationId.sparkline.TooltipStartValue),
                                    endText = localizer.getString(dashboard.localizationId.sparkline.TooltipEndValue),
                                    minText = localizer.getString(dashboard.localizationId.sparkline.TooltipMinValue),
                                    maxText = localizer.getString(dashboard.localizationId.sparkline.TooltipMaxValue),
                                    html = "<table style='border-spacing:0px;'>",
                                    template = "</td><td style='width: 15px'></td><td style='text-align: right'>";
                                html += "<tr><td>" + startText + template + format(this.originalFirstValue) + "</td></tr>";
                                html += "<tr><td>" + endText + template + format(this.originalLastValue) + "</td></tr>";
                                html += "<tr><td>" + minText + template + format(this.originalMinValue) + "</td></tr>";
                                html += "<tr><td>" + maxText + template + format(this.originalMaxValue) + "</td></tr>";
                                html += "</table>";
                                return {html: html}
                            },
                            zIndex: 100
                        }
                    }
            },
            _convertIndicatorType: function(type) {
                return this.deltaIndicatorTypes[type]
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file chartDataControllerBase.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data,
            localizer = data.localizer,
            commonUtils = DX.require("/utils/utils.common"),
            Color = DX.require("/color");
        data.chartDataControllerBase = data.dataControllerBase.inherit({
            ctor: function ctor(options) {
                this.callBase(options);
                this.elementCustomColor = options.elementCustomColor
            },
            getArgument: function(argumentAxisPoint) {
                if (this.isQualitativeArgument())
                    return this.getTitle(argumentAxisPoint, '\n');
                else if (this.isDiscreteArgument())
                    if (argumentAxisPoint.getParent() != null)
                        return argumentAxisPoint.getDisplayText();
                    else
                        return localizer.getString(dashboard.localizationId.ChartTotalValue);
                else
                    return argumentAxisPoint.getValue()
            },
            getArgumentAxisPoints: function(argumentId) {
                if (this.viewModel) {
                    var id = argumentId ? argumentId : this.viewModel.Argument.SummaryArgumentMember,
                        argumentAxis = this._getArgumentAxis();
                    return argumentAxis ? argumentAxis.getPointsByDimension(id, true) : []
                }
                else
                    return []
            },
            getSeriesAxisPoints: function(seriesId) {
                var seriesAxis = this._getSeriesAxis(),
                    id = seriesId ? seriesId : this.viewModel.SummarySeriesMember;
                return seriesAxis ? seriesAxis.getPointsByDimension(id, true) : []
            },
            getArgumentAxisDimensionFormat: function(index) {
                var argumentDimension = this._getArgumentDimension(index);
                return argumentDimension ? argumentDimension.getFormat() : undefined
            },
            getColor: function(argumentAxisPoint, seriesAxisPoint, measuesIds, colorMeasureId) {
                var that = this,
                    color = that._getColorFromData(argumentAxisPoint, seriesAxisPoint, colorMeasureId);
                return that._getElementCustomColor(argumentAxisPoint, seriesAxisPoint, measuesIds, color)
            },
            isDiscreteArgument: function() {
                return this.viewModel && this.viewModel.Argument.DataType === 'String'
            },
            isQualitativeArgument: function() {
                return this._getArgumentAxisDimensions().length > 1
            },
            isSingleArgument: function() {
                return this._getArgumentAxisDimensions().length == 1
            },
            hasSeriesPoints: function() {
                return this.viewModel && !!this.viewModel.SummarySeriesMember
            },
            _getElementCustomColor: function(argumentAxisPoint, seriesAxisPoint, measuesIds, color) {
                var that = this,
                    dxColor = new Color(color),
                    newColor;
                if (that.elementCustomColor && color) {
                    var customElementColorEventArgs = {
                            targetElement: [argumentAxisPoint, seriesAxisPoint],
                            measureIds: measuesIds,
                            color: dxColor.toHex()
                        };
                    that.elementCustomColor(customElementColorEventArgs);
                    newColor = new Color(customElementColorEventArgs.color);
                    if (!newColor.colorIsInvalid)
                        return customElementColorEventArgs.color
                }
                return color
            },
            _getColorFromData: function(argumentAxisPoint, seriesAxisPoint, colorMeasureId) {
                var that = this,
                    colorArgumentAxisPoint = argumentAxisPoint ? argumentAxisPoint.getParentByDimensionId(that.viewModel.ArgumentColorDimension) : undefined,
                    colorSeriesAxisPoint = seriesAxisPoint.getParentByDimensionId(that.viewModel.SeriesColorDimension);
                if (commonUtils.isDefined(colorMeasureId))
                    try {
                        var colorValue = that._getCrossSlice(colorArgumentAxisPoint, colorSeriesAxisPoint).getColorMeasureValue(colorMeasureId);
                        return colorValue !== null ? dashboard.utils.toColor(colorValue) : undefined
                    }
                    catch(e) {
                        return undefined
                    }
                return undefined
            },
            _getCrossSlice: function(argumentAxisPoint, seriesAxisPoint) {
                var slice = this.multiData.getSlice(seriesAxisPoint);
                return argumentAxisPoint ? slice.getSlice(argumentAxisPoint) : slice
            },
            _getArgumentAxis: function() {
                return this.multiData ? this.multiData.getAxis(dashboard.itemDataAxisNames.chartArgumentAxis) : undefined
            },
            _getSeriesAxis: function() {
                return this.multiData ? this.multiData.getAxis(dashboard.itemDataAxisNames.chartSeriesAxis) : undefined
            },
            _getArgumentAxisDimensions: function() {
                var argumentAxis = this._getArgumentAxis(),
                    dimensions = argumentAxis ? argumentAxis.getDimensions() : [];
                return dimensions ? dimensions : []
            },
            _getArgumentDimension: function(index) {
                var argumentDimensions = this._getArgumentAxisDimensions();
                if (index >= 0 && index < argumentDimensions.length)
                    return argumentDimensions[index];
                else
                    return undefined
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file pivotDataController.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data,
            GT_UNIQUE_PATH = 'GT';
        data.pivotDataController = data.dataControllerBase.inherit({
            ctor: function ctor(options) {
                this.callBase(options);
                this._columnPointsHash = {};
                this._rowPointsHash = {};
                this._measureIds = this._getMeasureIds();
                this._collapseStateCache = {};
                this._conditionalFormattingInfoCache = []
            },
            _createAreaFields: function(list, area) {
                var result = [];
                if (list && list.length > 0)
                    $.each(list, function(i, elem) {
                        result.push({
                            caption: elem.Caption,
                            area: area
                        })
                    });
                return result
            },
            _getFields: function() {
                var viewModel = this.viewModel;
                if (!viewModel)
                    return undefined;
                return this._createAreaFields(viewModel.Columns, "column").concat(this._createAreaFields(viewModel.Rows, "row")).concat(this._createAreaFields(viewModel.Values, "data"))
            },
            getDataSource: function(isColumn, path) {
                var columnRoot = path && isColumn ? this._getColumnAxis().getPointByUniqueValues(path) : this._getColumnAxis().getRootPoint(),
                    rowRoot = path && !isColumn ? this._getRowAxis().getPointByUniqueValues(path) : this._getRowAxis().getRootPoint(),
                    columnHeaders = [],
                    rowHeaders = [],
                    columnIndexHash = [],
                    rowIndexHash = [],
                    columnHash = {},
                    rowHash = {};
                this._prepareHierarchy(columnRoot, columnHeaders, columnIndexHash, columnHash, path && isColumn);
                this._prepareHierarchy(rowRoot, rowHeaders, rowIndexHash, rowHash, path && !isColumn);
                this._columnPointsHash = columnHash;
                this._rowPointsHash = rowHash;
                return {
                        fields: this._getFields(),
                        columns: columnHeaders,
                        rows: rowHeaders,
                        values: this._prepareCells(columnIndexHash, rowIndexHash, !!path)
                    }
            },
            getStyleSettingsInfo: function(cellItem, collapseStateCache, conditionalFormattingInfoCache) {
                var rules = this._getFormatRules(cellItem);
                this._collapseStateCache = collapseStateCache;
                this._conditionalFormattingInfoCache = conditionalFormattingInfoCache;
                return this._getStyleSettingsInfo(cellItem, rules)
            },
            _prepareHierarchy: function(root, headers, areaIndexHash, areaHash, isPartial) {
                var index = 0,
                    currentItem,
                    iteratePoints = function(rootPoint, headers, point, areaIndexHash, areaHash, item) {
                        var children = point.getChildren(),
                            child;
                        if (children && children.length > 0)
                            for (var i = 0; i < children.length; i++) {
                                child = children[i];
                                currentItem = {
                                    index: index++,
                                    value: child.getUniqueValue(),
                                    displayText: child.getDisplayText()
                                };
                                areaHash[child.getUniquePath()] = child;
                                areaIndexHash.push(child);
                                if (item) {
                                    if (!item.children)
                                        item.children = [];
                                    item.children.push(currentItem)
                                }
                                iteratePoints(rootPoint, headers, child, areaIndexHash, areaHash, currentItem)
                            }
                        if (point.getParent() === rootPoint)
                            headers.push(item)
                    };
                iteratePoints(root, headers, root, areaIndexHash, areaHash);
                if (!isPartial) {
                    areaHash[GT_UNIQUE_PATH] = root;
                    areaIndexHash.push(root)
                }
            },
            _getMeasureIds: function() {
                var measureIds = [];
                for (var i = 0; i < this.viewModel.Values.length; i++)
                    measureIds.push(this.viewModel.Values[i].DataId);
                return measureIds
            },
            _prepareCells: function(columnHash, rowHash, partial) {
                var cells = [],
                    mddata = this.multiData,
                    measureIds = this._measureIds,
                    rowIndex = 0,
                    columnIndex = 0,
                    dataIndex = 0,
                    displayValue,
                    columnPoint,
                    rowPoint,
                    fillCell = function() {
                        columnPoint = columnHash[columnIndex];
                        rowPoint = rowHash[rowIndex];
                        displayValue = mddata.getMeasureValueByAxisPoints(measureIds[dataIndex], [columnPoint, rowPoint]).getDisplayText();
                        if (displayValue || !partial) {
                            if (!cells[rowIndex])
                                cells[rowIndex] = [];
                            if (!cells[rowIndex][columnIndex])
                                cells[rowIndex][columnIndex] = [];
                            cells[rowIndex][columnIndex][dataIndex] = displayValue
                        }
                    };
                for (rowIndex = 0; rowIndex < rowHash.length; rowIndex++)
                    for (columnIndex = 0; columnIndex < columnHash.length; columnIndex++)
                        for (dataIndex = 0; dataIndex < measureIds.length; dataIndex++)
                            fillCell();
                return cells
            },
            _getColumnAxis: function() {
                return this.multiData.getAxis(dashboard.itemDataAxisNames.pivotColumnAxis)
            },
            _getRowAxis: function() {
                return this.multiData.getAxis(dashboard.itemDataAxisNames.pivotRowAxis)
            },
            _getStyleSettingsInfo: function(cellItem, rules) {
                var cellInfo,
                    columnAxisPoint,
                    rowAxisPoint;
                if (rules.length > 0) {
                    if (cellItem.area === dashboard.utils.pivotArea.column || cellItem.area === dashboard.utils.pivotArea.data)
                        columnAxisPoint = this._getColumnAxisPointByPath(cellItem.columnPath);
                    if (cellItem.area === dashboard.utils.pivotArea.row || cellItem.area === dashboard.utils.pivotArea.data)
                        rowAxisPoint = this._getRowAxisPointByPath(cellItem.rowPath)
                }
                cellInfo = {
                    columnAxisPoint: columnAxisPoint,
                    rowAxisPoint: rowAxisPoint
                };
                return this._getStyleSettingsInfoCore(cellInfo, rules, dashboard.itemDataAxisNames.pivotColumnAxis, dashboard.itemDataAxisNames.pivotRowAxis)
            },
            _getStyleIndexes: function(rule, cellInfo, points) {
                var that = this,
                    currentStyleIndexes,
                    styleIndexes = [];
                if (rule.ApplyToRow) {
                    currentStyleIndexes = that._findStyleSettingsOnAxis(cellInfo.rowAxisPoint, cellInfo.columnAxisPoint, rule.FormatConditionMeasureId, true);
                    if (currentStyleIndexes.length > 0)
                        styleIndexes = styleIndexes.concat(currentStyleIndexes)
                }
                if (rule.ApplyToColumn) {
                    currentStyleIndexes = that._findStyleSettingsOnAxis(cellInfo.rowAxisPoint, cellInfo.columnAxisPoint, rule.FormatConditionMeasureId, false);
                    if (currentStyleIndexes.length > 0)
                        styleIndexes = styleIndexes.concat(currentStyleIndexes)
                }
                if (!rule.ApplyToRow && !rule.ApplyToColumn) {
                    if (cellInfo.columnAxisPoint)
                        points.push(cellInfo.columnAxisPoint);
                    if (cellInfo.rowAxisPoint)
                        points.push(cellInfo.rowAxisPoint);
                    currentStyleIndexes = that._getMeasureValueByAxisPoints(points, rule.FormatConditionMeasureId);
                    if (currentStyleIndexes)
                        styleIndexes = styleIndexes.concat(currentStyleIndexes)
                }
                return styleIndexes
            },
            _findStyleSettingsOnAxis: function(rowAxisPoint, columnAxisPoint, measureId, isRowAxis) {
                var that = this,
                    currentStyleIndexes,
                    styleIndexes = [],
                    rowPoint = rowAxisPoint ? rowAxisPoint : this._getRowAxis().getRootPoint(),
                    columnPoint = columnAxisPoint ? columnAxisPoint : this._getColumnAxis().getRootPoint(),
                    slicePoint = isRowAxis ? rowPoint : columnPoint,
                    intersectingRootPoint,
                    slice,
                    intersectingPoints = [],
                    cfAxisPoint,
                    conditionalFormattingInfo = {
                        slicePoint: slicePoint,
                        measureId: measureId,
                        styleIndexes: [],
                        toString: function() {
                            return this.slicePoint.getUniquePath() + this.measureId
                        }
                    },
                    iteratePoints = function(intersectingPoints, point) {
                        var children = point.getChildren(),
                            child,
                            collapseState = point.getUniquePath().concat(isRowAxis ? 'column' : 'row'),
                            cachedCollapseState = that._collapseStateCache[collapseState];
                        if (cachedCollapseState === undefined) {
                            intersectingPoints.push(point);
                            if (children && children.length > 0)
                                for (var i = 0; i < children.length; i++) {
                                    child = children[i];
                                    iteratePoints(intersectingPoints, child)
                                }
                        }
                    };
                cfAxisPoint = this._conditionalFormattingInfoCache[conditionalFormattingInfo];
                if (cfAxisPoint)
                    return cfAxisPoint.styleIndexes;
                slice = this.multiData.getSlice(slicePoint);
                intersectingRootPoint = isRowAxis ? this._getColumnAxis().getRootPoint() : this._getRowAxis().getRootPoint();
                iteratePoints(intersectingPoints, intersectingRootPoint);
                $.each(intersectingPoints, function(_, intersectingPoint) {
                    var finalSlice = slice.getSlice(intersectingPoint),
                        currentStyleIndexes = finalSlice.getConditionalFormattingMeasureValue(measureId);
                    if (currentStyleIndexes)
                        styleIndexes = styleIndexes.concat(currentStyleIndexes)
                });
                conditionalFormattingInfo.styleIndexes = styleIndexes;
                this._conditionalFormattingInfoCache[conditionalFormattingInfo] = conditionalFormattingInfo;
                return styleIndexes
            },
            _getFormatRules: function(cellItem) {
                var that = this,
                    dataId,
                    rules = [];
                if (that.cfModel && that.cfModel.RuleModels.length !== 0)
                    switch (cellItem.area) {
                        case dashboard.utils.pivotArea.column:
                            dataId = that._getPointId(that._getColumnAxisPointByPath(cellItem.columnPath));
                            rules = rules.concat(that._getFormatRulesByDataId(dataId));
                            break;
                        case dashboard.utils.pivotArea.row:
                            dataId = that._getPointId(that._getRowAxisPointByPath(cellItem.rowPath));
                            rules = rules.concat(that._getFormatRulesByDataId(dataId));
                            break;
                        default:
                            dataId = that._measureIds[cellItem.cellIndex];
                            rules = $.grep(that.cfModel.RuleModels, function(rule) {
                                return rule.ApplyToRow || rule.ApplyToDataId === dataId
                            });
                            break
                    }
                return rules
            },
            _getColumnAxisPointByPath: function(columnPath) {
                var columnAxisPoint = this._columnPointsHash[columnPath];
                if (!columnAxisPoint)
                    columnAxisPoint = this._getColumnAxis().getPointByUniqueValues(columnPath);
                return columnAxisPoint
            },
            _getRowAxisPointByPath: function(rowPath) {
                var rowAxisPoint = this._rowPointsHash[rowPath];
                if (!rowAxisPoint)
                    rowAxisPoint = this._getRowAxis().getPointByUniqueValues(rowPath);
                return rowAxisPoint
            },
            _getFormatRulesByDataId: function(dataId) {
                var that = this,
                    formatRules = [];
                if (that.cfModel)
                    $.each(that.cfModel.RuleModels, function(_, rule) {
                        if (rule.ApplyToDataId === dataId)
                            formatRules.push(rule)
                    });
                return formatRules
            },
            _getPointId: function(point) {
                var dimension,
                    columnPointId;
                if (point) {
                    dimension = point.getDimension();
                    columnPointId = dimension ? dimension.id : undefined
                }
                return columnPointId
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file chartDataController.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data,
            chartHelper = dashboard.data.chartHelper,
            formatter = data.formatter,
            localizer = data.localizer,
            formatHelper = DX.require("/utils/utils.formatHelper"),
            commonUtils = DX.require("/utils/utils.common");
        data.chartDataController = data.chartDataControllerBase.inherit({
            ctor: function ctor(options) {
                this.callBase(options);
                this._argumentAxisPoints = this.getArgumentAxisPoints()
            },
            getDataSourceAndSeries: function(encodeHtml) {
                var that = this;
                if (!that.viewModel || that._argumentAxisPoints.length == 0)
                    return {
                            dataSource: null,
                            series: null
                        };
                var isDiscreteArgument = that.isDiscreteArgument(),
                    seriesInfoList = that._getSeriesInfo(encodeHtml),
                    legendInfoList = that._getLegendInfo(),
                    categories = [],
                    result = {
                        dataSource: [],
                        series: []
                    };
                if (seriesInfoList.length > 0) {
                    $.each(seriesInfoList, function(_, seriesInfo) {
                        if (seriesInfo.seriesItem)
                            result.series.push(seriesInfo.seriesItem)
                    });
                    if (that.viewModel.AxisX && that.viewModel.AxisX.Reverse)
                        result.series.reverse();
                    $.each(legendInfoList, function(_, legendInfo) {
                        result.series.push({
                            name: legendInfo.name,
                            color: legendInfo.color,
                            type: legendInfo.type
                        })
                    });
                    $.each(that._argumentAxisPoints, function(_, argumentAxisPoint) {
                        var argument = that.getArgument(argumentAxisPoint),
                            dataSourceItem = {x: argument};
                        if (that._isSelectionTagsRequired())
                            dataSourceItem.tag = {axisPoint: argumentAxisPoint};
                        if (isDiscreteArgument)
                            categories.push(argument);
                        $.each(seriesInfoList, function(__, seriesInfo) {
                            if (seriesInfo.originalSeriesType === 'HighLowClose')
                                dataSourceItem['nullColumn'] = null;
                            $.each(seriesInfo.valueFields, function(___, valueField) {
                                dataSourceItem[valueField.name] = valueField.getValue(argumentAxisPoint)
                            })
                        });
                        result.dataSource.push(dataSourceItem)
                    });
                    result.argumentAxis = categories.length > 0 ? {categories: categories} : undefined
                }
                return result
            },
            generatePaneName: function(paneName, paneIndex) {
                return paneName || 'Pane ' + paneIndex
            },
            _getSeriesInfo: function(encodeHtml) {
                var that = this,
                    info,
                    seriesIndex = 0,
                    seriesAxisPoints = this.getSeriesAxisPoints(),
                    isGrandTotal = !this.viewModel.SummarySeriesMember,
                    includeTags = that._isSelectionTagsRequired(),
                    result = [];
                that._iterateSeriesTemplates(function(pane, seriesTemplate, paneIndex, templateIndex) {
                    var paneName = pane ? that.generatePaneName(pane.Name, paneIndex) : undefined,
                        specifyTitleByName = pane ? pane.SpecifySeriesTitlesWithSeriesName : false;
                    $.each(seriesAxisPoints, function(___, seriesAxisPoint) {
                        info = {
                            name: seriesTemplate.Name,
                            paneName: paneName,
                            pointVisible: seriesTemplate.SeriesType === 'Point' ? true : seriesTemplate.ShowPointMarkers,
                            seriesType: data.chartHelper.convertSeriesType(seriesTemplate.SeriesType),
                            originalSeriesType: seriesTemplate.SeriesType,
                            plotOnSecondaryAxis: seriesTemplate.PlotOnSecondaryAxis,
                            ignoreEmptyPoints: seriesTemplate.IgnoreEmptyPoints,
                            axisPoint: seriesAxisPoint,
                            dataMembers: seriesTemplate.DataMembers,
                            colorMeasureId: seriesTemplate.ColorMeasureID,
                            valueFormats: [],
                            valueFields: []
                        };
                        $.each(info.dataMembers, function(____, dataMember) {
                            info.valueFormats.push(that.multiData.getMeasureFormat(dataMember));
                            info.valueFields.push({
                                name: 'y' + seriesIndex.toString(),
                                getValue: function(argumentAxisPoint) {
                                    return that._getCrossSlice(argumentAxisPoint, seriesAxisPoint).getMeasureValue(dataMember).getValue()
                                }
                            });
                            seriesIndex++
                        });
                        if (seriesTemplate.PointLabel)
                            info.pointLabel = {
                                showPointLabels: seriesTemplate.PointLabel.ShowPointLabels,
                                rotationAngle: data.chartHelper.convertPointLabelRotationAngle(seriesTemplate.PointLabel.Orientation),
                                position: data.chartHelper.convertPointLabelPosition(seriesTemplate.PointLabel.Position),
                                showForZeroValues: seriesTemplate.PointLabel.ShowForZeroValues,
                                content: seriesTemplate.PointLabel.Content,
                                scatterContent: seriesTemplate.PointLabel.ScatterContent
                            };
                        if (!isGrandTotal) {
                            info.title = that.getTitle(seriesAxisPoint);
                            if (specifyTitleByName)
                                info.title += ' - ' + seriesTemplate.Name
                        }
                        else
                            info.title = seriesTemplate.Name;
                        info.seriesItem = that._createSeriesItem(info, includeTags, encodeHtml);
                        result.push(info)
                    })
                });
                return result
            },
            customizeTooltipText: function(series, point, seriesFormats, encodeHtml) {
                if (!this._validatePoint(point, series.type))
                    return null;
                var color = this._getCustomizeTooltipTextColor(point);
                return this._getTooltipHtml(series, point, seriesFormats, encodeHtml, color)
            },
            _getTooltipHtml: function(series, point, seriesFormats, encodeHtml, color) {
                var text = this._getTooltipTextInternal(series, point, seriesFormats, encodeHtml);
                if (color)
                    text = "<div>" + DX.utils.renderHelper.rectangle(color, 10, 10) + "&nbsp;&nbsp;" + text + "</div>";
                return text
            },
            _getTooltipTextInternal: function(series, point, seriesFormats, encodeHtml) {
                var that = this,
                    text = series.name + ": ";
                text = encodeHtml ? dashboard.utils.encodeHtml(text) : text;
                switch (series.type) {
                    case'rangebar':
                    case'rangearea':
                        text += that._formatValuesList([point.originalMinValue, point.originalValue], seriesFormats, encodeHtml);
                        break;
                    case'bubble':
                        text += that._formatValuesList([point.originalValue, point.size], seriesFormats, encodeHtml);
                        break;
                    case'stock':
                    case'candlestick':
                        text += that._formatOpenHighLowCloseValues([point.originalOpenValue, point.originalHighValue, point.originalLowValue, point.originalCloseValue], seriesFormats, series.getOptions().openValueField !== 'nullColumn', encodeHtml, '<br>');
                        break;
                    default:
                        text += that._formatValuesList([point.originalValue], seriesFormats, encodeHtml);
                        break
                }
                return text
            },
            getTooltipArgumentText: function(obj) {
                var argument = formatter.getPredefinedString(obj.argument);
                if (commonUtils.isDefined(argument))
                    return argument;
                else if (this.isDiscreteArgument())
                    return obj.originalArgument;
                else
                    return obj.argumentText
            },
            getZoomArguments: function() {
                var that = this,
                    axisX = that.viewModel ? that.viewModel.AxisX : undefined;
                if (axisX && axisX.LimitVisiblePoints && that._argumentAxisPoints.length > axisX.VisiblePointsCount)
                    return {
                            start: that.getArgument(that._argumentAxisPoints[0]),
                            end: that.getArgument(that._argumentAxisPoints[axisX.VisiblePointsCount - 1])
                        }
            },
            getArgumentUniquePath: function(value) {
                var that = this;
                for (var i = 0; i < that._argumentAxisPoints.length; i++)
                    if (that.getArgument(that._argumentAxisPoints[i]) === value)
                        return that._argumentAxisPoints[i].getUniquePath()
            },
            _getArgumentAutoFormat: function() {
                var that = this,
                    min = 0,
                    max = 1;
                if (that._argumentAxisPoints.length > 0) {
                    min = that._argumentAxisPoints[0].getValue();
                    max = that._argumentAxisPoints[that._argumentAxisPoints.length - 1].getValue()
                }
                return formatter.getAxisFormat(min, max, false)
            },
            getArgumentFormat: function(sourceFormat, dataType) {
                var that = this,
                    argumentAxisLabelFormat = sourceFormat ? formatter.convertToFormat(sourceFormat) : undefined;
                if (dataType == 'Numeric')
                    if (!argumentAxisLabelFormat || !sourceFormat.NumericFormat)
                        argumentAxisLabelFormat = that._getArgumentAutoFormat();
                    else if (sourceFormat.NumericFormat.Unit == 'Auto')
                        argumentAxisLabelFormat.unitPower = that._getArgumentAutoFormat().unitPower;
                if (argumentAxisLabelFormat)
                    argumentAxisLabelFormat.showTrailingZeros = false;
                return argumentAxisLabelFormat
            },
            formatArgument: function(argument, argumentAxisLabelFormat) {
                return formatHelper.format(argument.value, argumentAxisLabelFormat)
            },
            _validatePoint: function(point, seriesType) {
                switch (seriesType) {
                    case'rangebar':
                    case'rangearea':
                        return !(point.originalMinValue === null && point.originalValue === null);
                    case'bubble':
                        return !(point.originalValue === null && point.size === null);
                    case'stock':
                    case'candlestick':
                        return !(point.originalOpenValue === null && point.originalHighValue === null && point.originalLowValue === null && point.originalCloseValue === null);
                    default:
                        return !(point.originalValue === null)
                }
            },
            _getCustomizeTooltipTextColor: function(point) {
                if (point.getColor)
                    return point.getColor()
            },
            _getLegendInfo: function() {
                var that = this,
                    values = [],
                    result = [],
                    colorMeasures = that.multiData.getColorMeasures(),
                    argumentPoints = that.getArgumentAxisPoints(that.viewModel.ArgumentColorDimension),
                    seriesPoints = that.getSeriesAxisPoints(that.viewModel.SeriesColorDimension),
                    includeProc = function(axisPoint) {
                        var dim = axisPoint.getDimension();
                        return dim && $.inArray(dim.id, that.viewModel.ColorPathMembers) >= 0
                    },
                    getColorValuesProc = function(axisPoint) {
                        return axisPoint.getValuePath(includeProc)
                    },
                    getColorDisplayTextsProc = function(axisPoint) {
                        return axisPoint.getDisplayPath(includeProc)
                    };
                $.each(colorMeasures, function(_, colorMeasure) {
                    $.each(argumentPoints, function(__, argumentPoint) {
                        $.each(seriesPoints, function(___, seriesPoint) {
                            var color = that._getColorFromData(argumentPoint, seriesPoint, colorMeasure.id);
                            if (color) {
                                var valueSet = getColorValuesProc(argumentPoint).concat(getColorValuesProc(seriesPoint));
                                var displayTexts = getColorDisplayTextsProc(argumentPoint).concat(getColorDisplayTextsProc(seriesPoint));
                                if (colorMeasure.name) {
                                    valueSet.push(colorMeasure.name);
                                    displayTexts.push(colorMeasure.name)
                                }
                                if (!that._valuesContainsValueSet(values, valueSet)) {
                                    values.push(valueSet);
                                    var colorText = displayTexts.join(' - ');
                                    if (!colorText || colorText == '')
                                        colorText = that._getDisplayTextBySeriesTemplates();
                                    var transparentColor = !that.viewModel.ArgumentColorDimension && chartHelper.isTransparentColorType(that._getLastSeriesType(colorMeasure.id));
                                    result.push({
                                        name: colorText,
                                        color: color,
                                        type: transparentColor ? 'bubble' : 'line'
                                    })
                                }
                            }
                        })
                    })
                });
                return result
            },
            _valuesContainsValueSet: function(values, valueSet) {
                for (var i = 0; i < values.length; i++) {
                    if (values[i].length !== valueSet.length)
                        continue;
                    var equal = true;
                    for (var j = 0; j < values[i].length; j++)
                        if (values[i][j] !== valueSet[j]) {
                            equal = false;
                            break
                        }
                    if (equal)
                        return true
                }
                return false
            },
            _getLastSeriesType: function(colorMeasureId) {
                var panes = this.viewModel.Panes;
                for (var i = panes.length - 1; i >= 0; i--)
                    for (var j = panes[i].SeriesTemplates.length - 1; j >= 0; j--)
                        if (colorMeasureId === panes[i].SeriesTemplates[j].ColorMeasureID)
                            return chartHelper.convertSeriesType(panes[i].SeriesTemplates[j].SeriesType)
            },
            _getDisplayTextBySeriesTemplates: function() {
                var displayTexts = [];
                this._iterateSeriesTemplates(function(pane, seriesTemplate, paneIndex, templateIndex) {
                    var name = seriesTemplate.Name;
                    if ($.inArray(name, displayTexts) < 0)
                        displayTexts.push(name)
                });
                return displayTexts.join(', ')
            },
            _iterateSeriesTemplates: function(proc) {
                $.each(this.viewModel.Panes, function(paneIndex, pane) {
                    $.each(pane.SeriesTemplates, function(templateIndex, seriesTemplate) {
                        proc(pane, seriesTemplate, paneIndex, templateIndex)
                    })
                })
            },
            _isSelectionTagsRequired: function() {
                return true
            },
            _createSeriesItem: function(seriesInfo, includeTags, encodeHtml) {
                var that = this,
                    seriesItem = {
                        argumentField: 'x',
                        type: seriesInfo.seriesType,
                        showInLegend: seriesInfo.seriesType === 'stock' || seriesInfo.seriesType === 'candlestick'
                    },
                    setNamesListProc = function(names) {
                        if (seriesInfo.originalSeriesType === 'HighLowClose') {
                            seriesItem[names[0]] = 'nullColumn';
                            names.splice(0, 1)
                        }
                        for (var i = 0; i < Math.min(names.length, seriesInfo.valueFields.length); i++)
                            seriesItem[names[i]] = seriesInfo.valueFields[i].name
                    };
                if (commonUtils.isDefined(seriesInfo.title))
                    seriesItem.name = seriesInfo.title;
                if (commonUtils.isDefined(seriesInfo.paneName))
                    seriesItem.pane = seriesInfo.paneName;
                if (includeTags)
                    seriesItem.tag = {
                        axisPoint: seriesInfo.axisPoint,
                        dataMembers: seriesInfo.dataMembers,
                        valueFormats: seriesInfo.valueFormats,
                        colorMeasureId: seriesInfo.colorMeasureId
                    };
                if (seriesInfo.pointVisible)
                    seriesItem.point = {visible: seriesInfo.pointVisible};
                switch (seriesItem.type) {
                    case'rangebar':
                    case'rangearea':
                        setNamesListProc(["rangeValue1Field", "rangeValue2Field"]);
                        break;
                    case'bubble':
                        setNamesListProc(["valueField", "sizeField"]);
                        break;
                    case'stock':
                    case'candlestick':
                        setNamesListProc(["openValueField", "highValueField", "lowValueField", "closeValueField"]);
                        break;
                    default:
                        setNamesListProc(["valueField"]);
                        break
                }
                seriesItem.axis = (seriesInfo.paneName || '') + (seriesInfo.plotOnSecondaryAxis ? 'secondary' : 'primary');
                if (seriesInfo.ignoreEmptyPoints)
                    seriesItem.ignoreEmptyPoints = seriesInfo.ignoreEmptyPoints;
                if (seriesInfo.pointLabel && seriesInfo.pointLabel.showPointLabels) {
                    var pointLabel = seriesInfo.pointLabel;
                    seriesItem.label = {
                        visible: true,
                        rotationAngle: pointLabel.rotationAngle,
                        customizeText: function() {
                            return that._customizePointLabelText(this, pointLabel, seriesInfo, encodeHtml)
                        }
                    };
                    if (seriesItem.type === 'bar')
                        seriesItem.label.showForZeroValues = pointLabel.showForZeroValues;
                    if (seriesItem.type === 'bar' || seriesItem.type === 'bubble')
                        seriesItem.label.position = pointLabel.position;
                    else if (seriesItem.type === 'fullstackedbar')
                        seriesItem.label.position = 'inside'
                }
                var color = undefined;
                if (commonUtils.isDefined(seriesInfo.colorMeasureId))
                    color = that._getColorFromData(that._argumentAxisPoints[0], seriesInfo.axisPoint, seriesInfo.colorMeasureId);
                if (chartHelper.isSeriesColorSupported(seriesItem.type)) {
                    var argumentRootAxisPoint = this.multiData.getAxis(dashboard.itemDataAxisNames.chartArgumentAxis).getRootPoint();
                    color = that._getElementCustomColor(argumentRootAxisPoint, seriesInfo.axisPoint, seriesInfo.dataMembers, color)
                }
                if (color)
                    seriesItem.color = color;
                return seriesItem
            },
            _customizePointLabelText: function(valueContainer, pointLabel, seriesInfo, encodeHtml) {
                var that = this,
                    formatValueProc = function() {
                        var formats = seriesInfo.valueFormats;
                        switch (seriesInfo.seriesType) {
                            case'rangebar':
                            case'rangearea':
                                return that._formatValuesList([valueContainer.value], [formats[valueContainer.index]], encodeHtml);
                            case'bubble':
                                return that._formatValuesList([valueContainer.value, valueContainer.size], formats, encodeHtml);
                            case'stock':
                            case'candlestick':
                                return that._formatOpenHighLowCloseValues([valueContainer.openValue, valueContainer.highValue, valueContainer.lowValue, valueContainer.closeValue], formats, seriesInfo.originalSeriesType !== 'HighLowClose', encodeHtml, '\n\r');
                            default:
                                return that._formatValuesList([valueContainer.value], formats, encodeHtml)
                        }
                    };
                switch (pointLabel.content) {
                    case'Argument':
                        return valueContainer.argument;
                    case'SeriesName':
                        return valueContainer.seriesName;
                    case'ArgumentAndValue':
                        return valueContainer.argument + ": " + formatValueProc();
                    default:
                        return formatValueProc()
                }
            },
            _formatOpenHighLowCloseValues: function(values, formats, hasOpenValueField, encodeHtml, delimiter) {
                var result = '',
                    delimiter = delimiter || ' ',
                    formatsLength = formats ? formats.length : 0,
                    i,
                    formatIndex,
                    valueNames = [localizer.getString(dashboard.localizationId.OpenCaption), localizer.getString(dashboard.localizationId.HighCaption), localizer.getString(dashboard.localizationId.LowCaption), localizer.getString(dashboard.localizationId.CloseCaption)];
                if (values && formatsLength > 0 && formatsLength <= values.length)
                    for (formatIndex = 0, i = hasOpenValueField ? 0 : 1; formatIndex < formatsLength; formatIndex++, i++)
                        result += delimiter + valueNames[i] + ': ' + this._formatValue(values[i], formats[formatIndex], encodeHtml);
                return result
            },
            _formatValuesList: function(valuesList, formats, encodeHtml) {
                var result = '';
                if (formats && formats.length === valuesList.length)
                    for (var i = 0; i < valuesList.length; i++)
                        result = result + (i === 0 ? '' : ' - ') + this._formatValue(valuesList[i], formats[i], encodeHtml);
                return result
            },
            _formatValue: function(value, format, encodeHtml) {
                var text = formatter.format(value ? value : 0, format);
                return encodeHtml ? dashboard.utils.encodeHtml(text) : text
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file scatterChartDataController.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data,
            formatter = data.formatter;
        data.scatterChartDataController = data.chartDataController.inherit({
            ctor: function ctor(options) {
                this.callBase(options)
            },
            getArgument: function(argumentAxisPoint) {
                var measureId = this.viewModel.AxisXDataMember,
                    slice = this.multiData.getSlice(argumentAxisPoint);
                return slice.getMeasureValue(measureId).getValue()
            },
            getArgumentDisplayPath: function(axisPoint) {
                return axisPoint.getDisplayPath().reverse().join(', ')
            },
            getArgumentFormat: function(sourceFormat, dataType) {
                return this.multiData.getMeasureFormat(this.viewModel.AxisXDataMember)
            },
            formatArgument: function(argument, argumentAxisLabelFormat) {
                var isPercentAxis = this.viewModel.AxisXPercentValues;
                return formatter.formatAxisValue(argument.value, argument.min, argument.max, isPercentAxis)
            },
            _getTooltipHtml: function(series, point, seriesFormats, encodeHtml, color) {
                var that = this,
                    html = '',
                    text,
                    slice = that.multiData.getSlice(point.tag.axisPoint),
                    measureIds = that._getMeasureIds();
                $.each(measureIds, function(index, measureId) {
                    text = that.multiData.getMeasureById(measureId).name + ': ' + slice.getMeasureValue(measureId).getDisplayText();
                    html += '<tr><td>' + (color && index === 0 ? DX.utils.renderHelper.rectangle(color, 10, 10) : '') + '&nbsp;</td><td>' + text + '</td></tr>'
                });
                return '<table>' + html + '</table>'
            },
            _getMeasureIds: function() {
                var measureIds = [];
                measureIds.push(this.viewModel.AxisXDataMember);
                $.each(this.viewModel.Panes[0].SeriesTemplates[0].DataMembers, function(_, measureId) {
                    measureIds.push(measureId)
                });
                return measureIds
            },
            getTooltipArgumentText: function(obj) {
                return this._getTooltipArgumentText(obj.point.tag.axisPoint)
            },
            _getTooltipArgumentText: function(axisPoint) {
                var axisName = axisPoint.getAxisName();
                if (this.drillDownState[axisName])
                    return axisPoint.getDisplayText();
                else
                    return this.getArgumentDisplayPath(axisPoint)
            },
            _customizePointLabelText: function(valueContainer, pointLabel, seriesInfo, encodeHtml) {
                var that = this,
                    axisPoint = valueContainer.point.tag.axisPoint,
                    argument = function() {
                        return that._getTooltipArgumentText(axisPoint)
                    },
                    weight = function() {
                        var dataMembers = that.viewModel.Panes[0].SeriesTemplates[0].DataMembers;
                        if (dataMembers.length > 1) {
                            var measureId = dataMembers[1];
                            var slice = that.multiData.getSlice(axisPoint);
                            return slice.getMeasureValue(measureId).getDisplayText()
                        }
                        return null
                    },
                    values = function() {
                        var text = '',
                            measureIds = that._getMeasureIds(),
                            slice = that.multiData.getSlice(axisPoint);
                        $.each(measureIds, function(index, measureId) {
                            text += (index > 0 ? ' - ' : '') + slice.getMeasureValue(measureId).getDisplayText()
                        });
                        return text
                    };
                switch (pointLabel.scatterContent) {
                    case'Argument':
                        return argument();
                    case'Weight':
                        return weight();
                    case'Values':
                        return values();
                    case'ArgumentAndWeight':
                        return argument() + ": " + weight();
                    case'ArgumentAndValues':
                        return argument() + ": " + values();
                    default:
                        return null
                }
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file rangeFilterDataController.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data;
        data.rangeFilterDataController = data.chartDataController.inherit({
            ctor: function ctor(options) {
                this.callBase(options)
            },
            getArgument: function(argumentAxisPoint) {
                if (argumentAxisPoint.getParent() != null)
                    return argumentAxisPoint.getValue();
                else
                    return localizer.getString(dashboard.localizationId.ChartTotalValue)
            },
            _iterateSeriesTemplates: function(proc) {
                $.each(this.viewModel.SeriesTemplates, function(_, seriesTemplate) {
                    proc(undefined, seriesTemplate)
                })
            },
            _isSelectionTagsRequired: function() {
                return false
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file pieDataController.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data,
            formatter = data.formatter;
        var PieSettingsType = {
                SeriesOnly: 'SeriesOnly',
                ArgumentsOnly: 'ArgumentsOnly',
                ArgumentsAndSeries: 'ArgumentsAndSeries',
                ElementSelection: 'ElementSelection'
            };
        data.pieDataController = data.chartDataControllerBase.inherit({
            ctor: function ctor(options) {
                this.callBase(options);
                if (this.multiData && this.viewModel) {
                    this._measures = this.multiData ? this.multiData.getMeasures() : [];
                    this._argumentAxisPoints = this.getArgumentAxisPoints();
                    if (this.viewModel.ProvideValuesAsArguments)
                        this.settingsType = PieSettingsType.SeriesOnly;
                    else if (!this.viewModel.SummarySeriesMember)
                        this.settingsType = PieSettingsType.ArgumentsOnly;
                    else if (this.viewModel.ContentDescription && this.viewModel.ContentDescription.ElementSelectionEnabled)
                        this.settingsType = PieSettingsType.ElementSelection;
                    else
                        this.settingsType = PieSettingsType.ArgumentsAndSeries
                }
            },
            getPointDisplayTexts: function(pointTag, value, percent) {
                var that = this,
                    valueDataMember = pointTag.dataMembers[0],
                    measure = this.multiData.getMeasureById(valueDataMember);
                return {
                        argumentText: this.settingsType === PieSettingsType.SeriesOnly ? measure.name : that.getTitle(pointTag.axisPoint, '\n'),
                        valueText: measure.format(value),
                        percentText: formatter.formatNumeric(percent, this.viewModel.PercentFormatViewModel)
                    }
            },
            createDataSource: function(seriesAxisPoint, valueDataMember) {
                var that = this,
                    viewModel = that.viewModel,
                    dataSource = [];
                if (that.settingsType === PieSettingsType.SeriesOnly) {
                    var argumentAxisPoint = that._getArgumentAxis().getRootPoint();
                    $.each(that._measures, function(index, measure) {
                        var dataMember = measure.id;
                        if ($.inArray(dataMember, viewModel.ValueDataMembers) !== -1)
                            dataSource.push({
                                x: measure.name,
                                y: that._getCrossSlice(argumentAxisPoint, seriesAxisPoint).getMeasureValue(dataMember).getValue(),
                                tag: {
                                    axisPoint: argumentAxisPoint,
                                    dataMembers: [dataMember],
                                    colorMeasureId: that._getColorDataMemberByIndex(index)
                                }
                            })
                    })
                }
                else
                    $.each(that._argumentAxisPoints, function(_, argumentAxisPoint) {
                        dataSource.push({
                            x: that.getArgument(argumentAxisPoint),
                            y: that._getCrossSlice(argumentAxisPoint, seriesAxisPoint).getMeasureValue(valueDataMember).getValue(),
                            tag: {
                                axisPoint: argumentAxisPoint,
                                dataMembers: [valueDataMember],
                                colorMeasureId: that._getColorDataMemberByMeasureId(valueDataMember)
                            }
                        })
                    });
                return dataSource
            },
            getValueDataMembers: function() {
                var viewModel = this.viewModel;
                switch (this.settingsType) {
                    case PieSettingsType.SeriesOnly:
                        return ['SeriesOnlyInternalFakeValueDataMember'];
                    case PieSettingsType.ArgumentsOnly:
                    case PieSettingsType.ArgumentsAndSeries:
                        return viewModel.ValueDataMembers;
                    case PieSettingsType.ElementSelection:
                        return [viewModel.ValueDataMembers[viewModel.ContentDescription.SelectedElementIndex]]
                }
            },
            getValueDisplayNames: function(seriesAxisPoint, valueDataMemberIndex) {
                var viewModel = this.viewModel;
                switch (this.settingsType) {
                    case PieSettingsType.ArgumentsOnly:
                        return viewModel.ValueDisplayNames[valueDataMemberIndex];
                    case PieSettingsType.SeriesOnly:
                    case PieSettingsType.ArgumentsAndSeries:
                    case PieSettingsType.ElementSelection:
                        return this.getTitle(seriesAxisPoint)
                }
            },
            _getColorDataMemberByMeasureId: function(valueDataMember) {
                var viewModel = this.viewModel;
                switch (this.settingsType) {
                    case PieSettingsType.ArgumentsOnly:
                        var index = $.inArray(valueDataMember, viewModel.ValueDataMembers);
                        return this._getColorDataMemberByIndex(index);
                    case PieSettingsType.ArgumentsAndSeries:
                        return this._getColorDataMemberByIndex(0);
                    case PieSettingsType.ElementSelection:
                        return this._getColorDataMemberByIndex(viewModel.ContentDescription.SelectedElementIndex);
                    default:
                        return undefined
                }
            },
            _getColorDataMemberByIndex: function(index) {
                var colorDataMembers = this.viewModel.ColorDataMembers;
                return colorDataMembers.length == 1 ? colorDataMembers[0] : colorDataMembers[index]
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file filterElementDataController.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data,
            localizer = data.localizer;
        data.VALUE_EXPR = 'value';
        data.ALL_ELEMENT = {
            value: {all: true},
            text: '(All)',
            isAll: true
        };
        data.filterElementDataController = data.dataControllerBase.inherit({
            ctor: function ctor(options) {
                this.callBase(options);
                this.dataSource = [];
                this.selection = [];
                this.update(options ? options.selectedValues : undefined)
            },
            getAllItemIndex: function() {
                return this.dataSource ? $.inArray(data.ALL_ELEMENT, this.dataSource) : -1
            },
            isAllSelected: function() {
                return this.dataSource && this.selection ? this.dataSource.length === this.selection.length : false
            },
            update: function(selectedValues) {
                this.dataSource = [];
                this.selection = [];
                var that = this,
                    hasAllElement = that.viewModel && that.viewModel.ShowAllValue && !that.isMultiselectable(),
                    points = that.multiData ? that.multiData.getAxis('Default').getPoints() : [];
                if (hasAllElement)
                    that.dataSource.push(data.ALL_ELEMENT);
                $.each(points, function(_, point) {
                    var dataItem = {},
                        uniqueValue = point.getUniquePath();
                    dataItem[data.VALUE_EXPR] = uniqueValue;
                    dataItem.text = that.getTitle(point);
                    that.dataSource.push(dataItem);
                    if (dashboard.utils.getValueIndex(selectedValues, uniqueValue) >= 0)
                        that.selection.push(dataItem)
                });
                if (hasAllElement && that.dataSource.length - 1 === that.selection.length)
                    that.selection.splice(0, 0, data.ALL_ELEMENT)
            },
            getInteractionValues: function(elements, selectedValues) {
                var that = this,
                    values = [],
                    items = !that.isMultiselectable() && $.inArray(data.ALL_ELEMENT, elements) >= 0 ? that.dataSource : elements;
                $.each(items, function(_, item) {
                    if (item !== data.ALL_ELEMENT)
                        values.push(that._getDataValue(item))
                });
                return values
            },
            _getDataValue: function(wrappedValue) {
                var itemData = wrappedValue && wrappedValue.itemData || wrappedValue;
                return itemData[data.VALUE_EXPR] || null
            }
        });
        data.comboBoxDataController = data.filterElementDataController.inherit({
            ComboBoxType: {
                Standard: 'Standard',
                Checked: 'Checked'
            },
            ctor: function ctor(options) {
                this.callBase(options)
            },
            isMultiselectable: function() {
                return this.viewModel && this.viewModel.ComboBoxType == this.ComboBoxType.Checked
            }
        });
        data.listBoxDataController = data.filterElementDataController.inherit({
            ListBoxType: {
                Checked: 'Checked',
                Radio: 'Radio'
            },
            ctor: function ctor(options) {
                this.callBase(options)
            },
            isMultiselectable: function() {
                return !this.viewModel || this.viewModel.ListBoxType == this.ListBoxType.Checked
            }
        });
        data.treeViewDataController = data.filterElementDataController.inherit({
            ctor: function ctor(options) {
                this.callBase(options)
            },
            isMultiselectable: function() {
                return true
            },
            getAllItemIndex: function() {
                return -1
            },
            isAllSelected: function() {
                return false
            },
            update: function(selectedValues) {
                var that = this,
                    hash = dashboard.utils.wrapHash(selectedValues),
                    sourceItems = that.multiData ? that.multiData.getAxis('Default').getRootPoint().getChildren() : {},
                    key = 0,
                    createDestNode = function(sourceNode) {
                        return {
                                key: key++,
                                value: sourceNode.getUniqueValue(),
                                text: sourceNode.getDisplayText(),
                                expanded: that.viewModel.AutoExpandNodes
                            }
                    },
                    walkTree = function(sourceNode, destNodeItems, branch) {
                        var children = sourceNode.getChildren(),
                            hasChildren = children && children.length !== 0,
                            subDestNode = createDestNode(sourceNode),
                            currentBranch = branch.slice();
                        currentBranch.push(subDestNode.value);
                        destNodeItems.push(subDestNode);
                        if (hasChildren) {
                            subDestNode.items = [];
                            $.each(children, function(_, node) {
                                walkTree(node, subDestNode.items, currentBranch)
                            })
                        }
                        else
                            subDestNode.selected = !!hash[currentBranch]
                    };
                that.dataSource = [];
                that.selection = [];
                $.each(sourceItems, function(_, sourceItem) {
                    walkTree(sourceItem, that.dataSource, [])
                })
            },
            getInteractionValues: function(elements, selectedValues) {
                var that = this,
                    hash = dashboard.utils.wrapHash(selectedValues),
                    parent = elements.length ? elements[0].parent : undefined,
                    rootBranch = [],
                    resultSelection = [],
                    prepareSelectionItems = function(items, parentBranch) {
                        $.each(items, function(_, item) {
                            var itemBranch = parentBranch.slice(),
                                value = that._getDataValue(item);
                            itemBranch.push(value);
                            if (item.items.length)
                                prepareSelectionItems(item.items, itemBranch);
                            else {
                                var isSelected = !!hash[itemBranch];
                                if (item.selected && !isSelected || !item.selected && isSelected)
                                    resultSelection.push(itemBranch)
                            }
                        })
                    };
                while (parent) {
                    rootBranch.splice(0, 0, that._getDataValue(parent));
                    parent = parent.parent
                }
                prepareSelectionItems(elements, rootBranch);
                return resultSelection
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file choroplethMapDataController.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data;
        data.choroplethMapDataController = data.dataControllerBase.inherit({
            ctor: function ctor(options) {
                this.callBase(options);
                this.axisHash = {};
                this.isEmpty = true;
                this._prepare()
            },
            _prepare: function() {
                var attributeId = this.viewModel.AttributeDimensionId,
                    axis = this.multiData.getAxis(),
                    axisPoints = attributeId ? axis.getPointsByDimension(attributeId) : [];
                this.isEmpty = axisPoints.length == 0;
                for (var i = 0; i < axisPoints.length; i++)
                    this.axisHash[axisPoints[i].getValue()] = axisPoints[i]
            },
            hasRecords: function() {
                return !this.isEmpty
            },
            getDeltaValue: function(attribute, deltaId) {
                var axisPoint = this.axisHash[attribute];
                return axisPoint ? this.multiData.getSlice(axisPoint).getDeltaValue(deltaId) : null
            },
            getValue: function(attribute, measureName) {
                var measureValue = this._getMeasureValue(attribute, measureName);
                return measureValue ? measureValue.getValue() : null
            },
            getDisplayText: function(attribute, measureName) {
                var measureValue = this._getMeasureValue(attribute, measureName);
                return measureValue ? measureValue.getDisplayText() : null
            },
            getUniqueValue: function(attribute) {
                var axisPoint = this.axisHash[attribute];
                return axisPoint ? axisPoint.getUniqueValue() : null
            },
            getMinMax: function(measureName) {
                var that = this,
                    min,
                    max,
                    value;
                $.each(this.axisHash, function(key, axisPoint) {
                    value = that.multiData.getSlice(axisPoint).getMeasureValue(measureName).getValue();
                    if (!min || value < min)
                        min = value;
                    if (!max || value > max)
                        max = value
                });
                return {
                        min: min,
                        max: max
                    }
            },
            getMeasureDescriptorById: function(valueId) {
                return this.multiData.getMeasureById(valueId)
            },
            _getMeasureValue: function(attribute, measureName) {
                var axisPoint = this.axisHash[attribute];
                return axisPoint ? this.multiData.getSlice(axisPoint).getMeasureValue(measureName) : null
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file geoPointMapDataControllerBase.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data;
        data.geoPointMapDataControllerBase = data.dataControllerBase.inherit({
            ctor: function ctor(options) {
                this.callBase(options);
                this.axisPoints = [];
                this._prepare()
            },
            getPoint: function(index) {
                var pointsCount = this._getPointsCount(index);
                return {
                        lat: this._getLatitudeValue(index),
                        lon: this._getLongitudeValue(index),
                        latSel: this._getLatitudeUniqueValue(index),
                        lonSel: this._getLongitudeUniqueValue(index),
                        pointsCount: pointsCount,
                        tooltipDimensions: pointsCount < 2 ? this._getTooltipDimensions(index) : [],
                        tooltipMeasures: this._getTooltipMeasures(index)
                    }
            },
            getCount: function() {
                return this.axisPoints.length
            },
            _prepare: function() {
                var dimensionId = this._getAxisPointDimensionDescriptorId(),
                    axis = this.multiData.getAxis();
                this.axisPoints = dimensionId ? axis.getPointsByDimension(dimensionId) : []
            },
            _getAxisPointDimensionDescriptorId: function() {
                return this.viewModel.LongitudeDataId
            },
            _getMeasure: function(index, measureName) {
                var axisPoint = this._getAxisPoint(index);
                return this.multiData.getSlice(axisPoint).getMeasureValue(measureName)
            },
            _getMeasureValue: function(index, measureName) {
                return this._getMeasure(index, measureName).getValue()
            },
            _getMeasureDisplayText: function(index, measureName) {
                return this._getMeasure(index, measureName).getDisplayText()
            },
            _getLatitude: function(index) {
                var point = this._getAxisPoint(index);
                return point.getParentByDimensionId(this.viewModel.LatitudeDataId)
            },
            _getLatitudeValue: function(index) {
                return this._getLatitude(index).getValue()
            },
            _getLatitudeUniqueValue: function(index) {
                return this._getLatitude(index).getUniqueValue()
            },
            _getLongitude: function(index) {
                var point = this._getAxisPoint(index);
                return point.getParentByDimensionId(this.viewModel.LongitudeDataId)
            },
            _getLongitudeValue: function(index) {
                return this._getLongitude(index).getValue()
            },
            _getLongitudeUniqueValue: function(index) {
                return this._getLongitude(index).getUniqueValue()
            },
            _getPointsCount: function(index) {
                var axisPoint = this._getAxisPoint(index).getParentByDimensionId(this.viewModel.LongitudeDataId);
                return this.multiData.getSlice(axisPoint).getMeasureValue(this.viewModel.PointsCountDataId).getValue()
            },
            _getTooltipDimensions: function(index) {
                var tooltipDimensionsViewModel = this.viewModel.TooltipDimensions,
                    tooltipDimensions = [],
                    values,
                    distinctValues;
                if (tooltipDimensionsViewModel)
                    for (var i = 0; i < tooltipDimensionsViewModel.length; i++) {
                        values = this._getAxisPoint(index).getDisplayTextsByDimensionId(tooltipDimensionsViewModel[i].DataId);
                        distinctValues = $.grep(values, function(el, index) {
                            return index === $.inArray(el, values)
                        });
                        tooltipDimensions.push({
                            caption: tooltipDimensionsViewModel[i].Caption,
                            values: distinctValues
                        })
                    }
                return tooltipDimensions
            },
            _getTooltipMeasures: function(index) {
                var tooltipMeasuresViewModel = this.viewModel.TooltipMeasures;
                var tooltipMeasures = [];
                if (tooltipMeasuresViewModel)
                    for (var i = 0; i < tooltipMeasuresViewModel.length; i++)
                        tooltipMeasures.push({
                            caption: tooltipMeasuresViewModel[i].Caption,
                            value: this._getMeasureDisplayText(index, tooltipMeasuresViewModel[i].DataId)
                        });
                return tooltipMeasures
            },
            _getAxisPoint: function(index) {
                return this.axisPoints[index]
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file geoPointMapDataController.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data;
        data.geoPointMapDataController = data.geoPointMapDataControllerBase.inherit({
            ctor: function ctor(options) {
                this.callBase(options)
            },
            getPoint: function(index) {
                return $.extend(true, this.callBase(index), {text: this._getMeasureDisplayText(index, this.viewModel.ValueId)})
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file bubbleMapDataController.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data;
        data.bubbleMapDataController = data.geoPointMapDataControllerBase.inherit({
            ctor: function ctor(options) {
                this.callBase(options)
            },
            getPoint: function(index) {
                return $.extend(true, this.callBase(index), {
                        weight: this._getMeasureValue(index, this.viewModel.WeightId),
                        color: this._getMeasureValue(index, this.viewModel.ColorId),
                        weightText: this._getMeasureDisplayText(index, this.viewModel.WeightId),
                        colorText: this._getMeasureDisplayText(index, this.viewModel.ColorId)
                    })
            },
            formatColor: function(value) {
                return this.multiData.getMeasureById(this.viewModel.ColorId).format(value)
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file pieMapDataController.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data;
        data.pieMapDataController = data.geoPointMapDataControllerBase.inherit({
            ctor: function ctor(options) {
                this.callBase(options)
            },
            getPoint: function(index, valueIndex) {
                var point = this.callBase(index),
                    viewModel = this.viewModel,
                    filledValues = viewModel.Values && viewModel.Values.length > 0,
                    axisPoint,
                    argument,
                    argumentDisplayText,
                    value,
                    valueDisplayText,
                    valueId;
                if (viewModel.ArgumentDataId) {
                    axisPoint = this._getAxisPoint(index);
                    argument = axisPoint.getUniqueValue();
                    argumentDisplayText = axisPoint.getDisplayText();
                    if (filledValues) {
                        valueId = viewModel.Values[0];
                        value = this._getMeasureValue(index, valueId);
                        valueDisplayText = this._getMeasureDisplayText(index, valueId)
                    }
                    else
                        value = point.pointsCount > 1 ? point.pointsCount : 1
                }
                else {
                    valueId = viewModel.Values[valueIndex];
                    argument = this.multiData.getMeasureById(valueId).name;
                    argumentDisplayText = argument;
                    value = this._getMeasureValue(index, valueId);
                    valueDisplayText = this._getMeasureDisplayText(index, valueId)
                }
                return $.extend(true, point, {
                        argument: argument,
                        argumentDisplayText: argumentDisplayText,
                        value: value,
                        valueDisplayText: valueDisplayText
                    })
            },
            _getAxisPointDimensionDescriptorId: function() {
                return this.viewModel.ArgumentDataId || this.viewModel.LongitudeDataId
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file gridDataController.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data,
            GridColumnType = {
                Dimension: 'Dimension',
                Measure: 'Measure',
                Delta: 'Delta',
                Sparkline: 'Sparkline'
            },
            DeltaValueType = {
                ActualValue: 'ActualValue',
                AbsoluteVariation: 'AbsoluteVariation',
                PercentVariation: 'PercentVariation',
                PercentOfTarget: 'PercentOfTarget'
            };
        data.gridDataController = data.dataControllerBase.inherit({
            ctor: function ctor(options) {
                this.callBase(options);
                this.hasColumns = false;
                this._hasColumnPoints = undefined;
                this._hasSparklinePoints = undefined;
                this._gridAxisPoints = undefined;
                this._gridSparklineAxisPoints = undefined;
                this._columnRepository = {};
                this._selectionMembers = [];
                this._initialize()
            },
            _initialize: function() {
                if (!this.multiData)
                    return;
                var that = this,
                    multiData = that.multiData,
                    viewModel = that.viewModel,
                    columnAxisName = viewModel.ColumnAxisName,
                    sparklineAxisName = viewModel.SparklineAxisName;
                that._hasColumnPoints = !!viewModel.HasDimensionColumns;
                that._hasSparklinePoints = !!sparklineAxisName;
                if (that._hasColumnPoints)
                    that._gridAxisPoints = multiData.getAxis(columnAxisName).getPoints(true);
                if (that._hasSparklinePoints)
                    that._gridSparklineAxisPoints = multiData.getAxis(sparklineAxisName).getPoints();
                that._prepareColumns()
            },
            getDataSource: function() {
                var that = this,
                    list = [],
                    axisPoints = that._gridAxisPoints,
                    columns = that._columnRepository,
                    listItem,
                    dataPostfix = data.dataController.DATA_POSTFIX,
                    valueItem,
                    barCalculator;
                if (that._hasColumnPoints)
                    for (var rowIndex = 0; rowIndex < axisPoints.length; rowIndex++) {
                        listItem = {index: rowIndex};
                        $.each(columns, function(columnName, columnInfo) {
                            valueItem = that._getValueItem(columnInfo, rowIndex);
                            listItem[columnName] = valueItem.getValue();
                            listItem[columnName + dataPostfix] = valueItem
                        });
                        list.push(listItem)
                    }
                else if (that._hasColumns) {
                    listItem = {index: 0};
                    $.each(columns, function(columnName, columnInfo) {
                        valueItem = that._getValueItem(columnInfo);
                        listItem[columnName] = valueItem.getValue();
                        listItem[columnName + dataPostfix] = valueItem
                    });
                    list.push(listItem)
                }
                return {store: {
                            type: 'array',
                            data: list,
                            key: 'index'
                        }}
            },
            getSelectionValues: function(values) {
                var that = this,
                    point,
                    resultPoint,
                    result = [],
                    selectionMembers = that._selectionMembers,
                    fit;
                if (values.length > selectionMembers.length)
                    values = values.slice(-selectionMembers.length);
                $.each(that._gridAxisPoints, function(index, axisPoint) {
                    fit = false;
                    $.each(selectionMembers, function(memberIndex, member) {
                        point = that._findAxisPoint(member, axisPoint);
                        if (point && values.length > memberIndex) {
                            if (!dashboard.utils.checkValuesAreEqual(point.getUniqueValue(), values[memberIndex])) {
                                fit = false;
                                return false
                            }
                            resultPoint = point
                        }
                        fit = true
                    });
                    if (fit && resultPoint) {
                        result = resultPoint.getUniquePath();
                        return
                    }
                });
                return result
            },
            getSelectedRowKeys: function(valuesSet) {
                var that = this,
                    keys = [],
                    selectionMembers = that._selectionMembers,
                    checkAxisPoint = function(axisPoint, values) {
                        var point;
                        for (var i = 0; i < values.length; i++) {
                            point = that._findAxisPoint(selectionMembers[i], axisPoint);
                            if (!dashboard.utils.checkValuesAreEqual(point.getUniqueValue(), values[i]))
                                return false
                        }
                        return true
                    };
                $.each(that._gridAxisPoints, function(index, axisPoint) {
                    $.each(valuesSet, function(_, values) {
                        if (values.length > selectionMembers.length)
                            values = values.slice(-selectionMembers.length);
                        if (checkAxisPoint(axisPoint, values)) {
                            keys.push(index);
                            return
                        }
                    })
                });
                return keys
            },
            getDimensionValues: function(rowIndex) {
                return this._gridAxisPoints[rowIndex].getUniquePath()
            },
            _getValueItem: function(columnInfo, rowIndex) {
                var that = this,
                    column = columnInfo.column;
                switch (column.ColumnType) {
                    case GridColumnType.Measure:
                        return that._getMeasureValue(columnInfo, rowIndex);
                    case GridColumnType.Delta:
                        return that._getDeltaValue(columnInfo, rowIndex);
                    case GridColumnType.Sparkline:
                        return that._getSparklineCellValues(columnInfo, rowIndex);
                    case GridColumnType.Dimension:
                    default:
                        return that._getDimensionCellValue(columnInfo, rowIndex)
                }
            },
            _getMeasureValue: function(columnInfo, rowIndex) {
                if (columnInfo.column.DisplayMode === 'Bar')
                    return this._getBarCellValue(columnInfo, rowIndex);
                else
                    return this._getMeasureCellValue(columnInfo, rowIndex)
            },
            getTotalValue: function(measureId) {
                return this.multiData.getMeasureValue(measureId).getDisplayText()
            },
            _getBarCellValue: function(columnInfo, rowIndex) {
                var that = this,
                    item = this.multiData.getMeasureValueByAxisPoints(columnInfo.columnName, that._getPointArray(rowIndex)),
                    barCalculator = columnInfo.barCalculator;
                barCalculator.addValue(item);
                return {
                        getValue: function() {
                            return item.getValue()
                        },
                        getData: function() {
                            return {
                                    zeroValue: barCalculator.getZeroPosition(),
                                    normalizedValue: barCalculator.getNormalizedValue(rowIndex || 0),
                                    text: item.getDisplayText()
                                }
                        },
                        getStyleSettingsInfo: function() {
                            return that._getStyleSettingsInfo(columnInfo.columnName, rowIndex)
                        }
                    }
            },
            _getMeasureCellValue: function(columnInfo, rowIndex) {
                var that = this,
                    columnName = columnInfo.columnName,
                    item = that.multiData.getMeasureValueByAxisPoints(columnName, that._getPointArray(rowIndex)),
                    value = item.getValue();
                return {
                        getValue: function() {
                            return value
                        },
                        getData: function() {
                            return {
                                    value: item.getValue(),
                                    displayText: item.getDisplayText()
                                }
                        },
                        getStyleSettingsInfo: function() {
                            return that._getStyleSettingsInfo(columnName, rowIndex)
                        }
                    }
            },
            _getDimensionCellValue: function(columnInfo, rowIndex) {
                var that = this,
                    columnName = columnInfo.columnName,
                    item = undefined,
                    value = undefined,
                    obtainItem = function() {
                        if (item === undefined)
                            item = that._findAxisPoint(columnName, that._getColumnAxisPoint(rowIndex)) || {
                                getValue: function() {
                                    return undefined
                                },
                                getUniqueValue: function() {
                                    return undefined
                                },
                                getDisplayText: function() {
                                    return ''
                                }
                            };
                        return item
                    };
                return {
                        getValue: function() {
                            return obtainItem().getValue()
                        },
                        getUniqueValue: function() {
                            return obtainItem().getUniqueValue()
                        },
                        getData: function() {
                            return {
                                    value: obtainItem().getValue(),
                                    displayText: obtainItem().getDisplayText()
                                }
                        },
                        getStyleSettingsInfo: function() {
                            return that._getStyleSettingsInfo(columnName, rowIndex)
                        }
                    }
            },
            _getStyleSettingsInfo: function(columnName, rowIndex) {
                var that = this,
                    rules = [],
                    cellInfo = {rowIndex: rowIndex};
                if (that.cfModel)
                    rules = $.grep(that.cfModel.RuleModels, function(rule) {
                        return rule.ApplyToRow || rule.ApplyToDataId === columnName
                    });
                return that._getStyleSettingsInfoCore(cellInfo, rules, that.viewModel.ColumnAxisName, dashboard.itemDataAxisNames.defaultAxis)
            },
            _getStyleIndexes: function(rule, cellInfo, points) {
                var that = this,
                    axisPoint,
                    currentStyleIndexes,
                    styleIndexes = [];
                axisPoint = cellInfo.rowIndex !== undefined ? that._getAxisPoint(cellInfo.rowIndex, rule.CalcByDataId) : undefined;
                if (axisPoint)
                    points.push(axisPoint);
                currentStyleIndexes = that._getMeasureValueByAxisPoints(points, rule.FormatConditionMeasureId);
                if (currentStyleIndexes)
                    styleIndexes = styleIndexes.concat(currentStyleIndexes);
                return styleIndexes
            },
            _getAxisPoint: function(rowIndex, columnInfo) {
                var axisPoint = rowIndex !== undefined ? this._gridAxisPoints[rowIndex] : undefined,
                    correctAxisPoint = axisPoint ? this._findAxisPoint(columnInfo, axisPoint) : undefined;
                return correctAxisPoint || axisPoint
            },
            _getDeltaValue: function(columnInfo, rowIndex) {
                if (columnInfo.column.DisplayMode === 'Bar')
                    return this._getBarCellValue(columnInfo, rowIndex);
                else
                    return this._getDeltaCellValue(columnInfo, rowIndex)
            },
            _getDeltaCellValue: function(columnInfo, rowIndex) {
                var that = this,
                    deltaValue = null,
                    deltaValueItem = null,
                    measureValue = null,
                    column = columnInfo.column,
                    columnName = columnInfo.columnName,
                    deltaType = column.DeltaValueType,
                    useDefaultColor = column.IgnoreDeltaColor,
                    deltaDesriptor = that.multiData.getDeltaById(columnName),
                    measureItem,
                    getStyleSettingsInfo = function(columnName, rowIndex) {
                        return that._getStyleSettingsInfo(columnInfo.columnName, rowIndex)
                    };
                if (deltaDesriptor) {
                    deltaValue = that.multiData.getDeltaValueByAxisPoints(columnName, that._getPointArray(rowIndex));
                    deltaValueItem = that._getDeltaValueItem(deltaValue, deltaType);
                    return {
                            getValue: function() {
                                return deltaValueItem.getValue()
                            },
                            getData: function() {
                                return {
                                        type: that._convertIndicatorType(deltaValue.getIndicatorType().getValue()),
                                        hasPositiveMeaning: deltaValue.getIsGood().getValue(),
                                        text: {
                                            value: deltaValueItem.getDisplayText(),
                                            useDefaultColor: useDefaultColor
                                        }
                                    }
                            },
                            getStyleSettingsInfo: function() {
                                return that._getStyleSettingsInfo(columnInfo.columnName, rowIndex)
                            }
                        }
                }
                else {
                    measureItem = that.multiData.getMeasureValueByAxisPoints(columnName, that._getPointArray(rowIndex));
                    return {
                            getValue: function() {
                                return measureItem.getValue()
                            },
                            getData: function() {
                                return {
                                        type: null,
                                        hasPositiveMeaning: null,
                                        text: {
                                            value: measureItem.getDisplayText(),
                                            useDefaultColor: null
                                        }
                                    }
                            },
                            getStyleSettingsInfo: function() {
                                return that._getStyleSettingsInfo(columnInfo.columnName, rowIndex)
                            }
                        }
                }
            },
            _getDeltaValueItem: function(deltaValue, deltaValueType) {
                switch (deltaValueType) {
                    case DeltaValueType.ActualValue:
                        return deltaValue.getActualValue();
                    case DeltaValueType.AbsoluteVariation:
                        return deltaValue.getAbsoluteVariation();
                    case DeltaValueType.PercentVariation:
                        return deltaValue.getPercentVariation();
                    case DeltaValueType.PercentOfTarget:
                        return deltaValue.getPercentOfTarget()
                }
            },
            _getSparklineCellValues: function(columnInfo, rowIndex) {
                var that = this,
                    columnName = columnInfo.columnName,
                    measureDescriptor = that.multiData.getMeasureById(columnName),
                    axisPoint = that._getPointArray(rowIndex),
                    getValues = function(getter) {
                        var result = [];
                        if (that._gridSparklineAxisPoints)
                            $.each(that._gridSparklineAxisPoints, function(_, sparklinePoint) {
                                result.push(getter(that.multiData.getMeasureValueByAxisPoints(columnName, axisPoint.concat(sparklinePoint))))
                            });
                        else
                            result.push(getter(that.multiData.getMeasureValueByAxisPoints(columnName, axisPoint)));
                        return result
                    },
                    values = getValues(function(item) {
                        var value = item.getValue();
                        return value || 0
                    });
                return {
                        getValue: function() {
                            return values
                        },
                        getData: function() {
                            var valuesItems = getValues(function(item) {
                                    return item
                                }),
                                startValue = valuesItems[0].getValue(),
                                endValue = valuesItems[valuesItems.length - 1].getValue();
                            return {
                                    sparkline: that._generateSparklineOptions(values, columnInfo.column.SparklineOptions, measureDescriptor.format),
                                    startText: startValue ? valuesItems[0].getDisplayText() : measureDescriptor.format(0),
                                    endText: endValue ? valuesItems[valuesItems.length - 1].getDisplayText() : measureDescriptor.format(0)
                                }
                        },
                        getStyleSettingsInfo: function() {
                            return that._getStyleSettingsInfo(columnInfo.columnName, rowIndex)
                        }
                    }
            },
            _findAxisPoint: function(dataId, axisPoint) {
                if (axisPoint)
                    while (axisPoint.getDimension() && axisPoint.getDimension().id !== dataId)
                        axisPoint = axisPoint.getParent();
                return axisPoint && axisPoint.getParent() ? axisPoint : null
            },
            _getColumnAxisPoint: function(rowIndex) {
                return this._hasColumnPoints ? this._gridAxisPoints[rowIndex] : undefined
            },
            _getPointArray: function(rowIndex) {
                var point = this._getColumnAxisPoint(rowIndex),
                    array = [];
                if (point)
                    array.push(point);
                return array
            },
            _prepareColumns: function() {
                var that = this,
                    columns = that.viewModel.Columns || [],
                    selectionMembers = that.viewModel.SelectionDataMembers,
                    columnName,
                    barModel;
                that._hasColumns = columns.length > 0;
                $.each(columns, function(_, column) {
                    columnName = column.DataId;
                    barModel = column.BarViewModel;
                    that._columnRepository[columnName] = {
                        columnName: columnName,
                        column: column,
                        barCalculator: barModel ? new data.gridBarCalculator({showZeroLevel: barModel.AlwaysShowZeroLevel}) : null
                    };
                    if ($.inArray(columnName, selectionMembers) !== -1)
                        that._selectionMembers.push(columnName)
                })
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file kpiDataController.js */
    (function($, DX, undefined) {
        var utils = DX.utils,
            dashboard = DX.dashboard,
            data = dashboard.data;
        data.kpiDataController = data.dataControllerBase.inherit({
            ctor: function ctor(options) {
                this.callBase(options);
                this.setSourceItemProperties = undefined;
                this._axisPoints = undefined;
                this._sparklineAxisPoints = undefined;
                this._initialize()
            },
            getDataSource: function() {
                var that = this,
                    axisPoints = that._axisPoints,
                    sourceItem,
                    dataSource = [],
                    element,
                    i;
                that._iterateKpiItems(function(kpiElement) {
                    element = kpiElement;
                    if (that._hasSeriesPoints())
                        for (i = 0; i < axisPoints.length; i++) {
                            sourceItem = that._createSourceItem(axisPoints[i], element);
                            dataSource.push(sourceItem)
                        }
                    else {
                        sourceItem = that._createSourceItem(null, element);
                        sourceItem.title = element.Title;
                        dataSource.push(sourceItem)
                    }
                });
                return dataSource
            },
            _createSourceItem: function(axisPoint, element) {
                var that = this,
                    deltaValue = that._getDeltaValue(axisPoint, element),
                    measure = that.multiData.getMeasureById(element.ID),
                    measureValue = that._getMeasureValue(axisPoint, element),
                    sparklineValues = that._getSparklineValues(axisPoint, element),
                    sourceItem,
                    getProperties = function() {
                        return {
                                getActualValue: function() {
                                    return deltaValue.getActualValue().getValue()
                                },
                                getTargetValue: function() {
                                    return deltaValue.getTargetValue().getValue()
                                },
                                getIndicatorType: function() {
                                    return that._convertIndicatorType(deltaValue.getIndicatorType().getValue())
                                },
                                getIsGood: function() {
                                    return deltaValue.getIsGood().getValue()
                                },
                                getMainValueText: function() {
                                    return deltaValue.getDisplayValue().getDisplayText()
                                },
                                getSubValue1Text: function() {
                                    return deltaValue.getDisplaySubValue1().getDisplayText()
                                },
                                getSubValue2Text: function() {
                                    return deltaValue.getDisplaySubValue2().getDisplayText()
                                },
                                getMeasureValue: function() {
                                    return measureValue.getValue()
                                },
                                getMeasureDisplayText: function() {
                                    return measureValue.getDisplayText()
                                },
                                getSparklineOptions: function() {
                                    var sparklineOptions = undefined;
                                    if (sparklineValues)
                                        sparklineOptions = that._generateSparklineOptions(sparklineValues, element.SparklineOptions, measure.format);
                                    return sparklineOptions
                                },
                                getSelectionValues: function() {
                                    return axisPoint ? axisPoint.getUniquePath() : null
                                },
                                getCaptions: function() {
                                    return axisPoint ? axisPoint.getDisplayPath() : []
                                },
                                getGaugeRange: function() {
                                    return that._getGaugeRange(element)
                                }
                            }
                    };
                sourceItem = {onIncidentOccurred: utils.renderHelper.widgetIncidentOccurred};
                that.setSourceItemProperties(sourceItem, element, getProperties());
                return sourceItem
            },
            _getDeltaValue: function(axisPoint, kpiElement) {
                var multiData = this.multiData,
                    measureId = kpiElement.ID;
                return axisPoint ? multiData.getDeltaValueByAxisPoints(measureId, [axisPoint]) : multiData.getDeltaValue(measureId)
            },
            _getMeasureValue: function(axisPoint, kpiElement) {
                var multiData = this.multiData,
                    measureId = kpiElement.ID;
                return axisPoint ? multiData.getMeasureValueByAxisPoints(measureId, [axisPoint]) : multiData.getMeasureValue(measureId)
            },
            _getSparklineValues: function(axisPoint, kpiElement) {
                if (!this._hasSparklinePoints())
                    return;
                var that = this,
                    values = [],
                    measureValue,
                    measureId = kpiElement.ID,
                    multiData = that.multiData;
                $.each(that._sparklineAxisPoints, function(_, sparklinePoint) {
                    measureValue = axisPoint ? multiData.getMeasureValueByAxisPoints(measureId, [axisPoint, sparklinePoint]) : multiData.getMeasureValueByAxisPoints(measureId, [sparklinePoint]);
                    values.push(measureValue.getValue() || 0)
                });
                return values
            },
            _initialize: function() {
                if (!this.multiData)
                    return;
                var that = this,
                    viewModel = that.viewModel,
                    multiData = that.multiData;
                that._axisPoints = that._hasSeriesPoints() ? multiData.getAxis(viewModel.SeriesAxisName).getPoints() : undefined;
                that._sparklineAxisPoints = that._hasSparklinePoints() ? multiData.getAxis(viewModel.SparklineAxisName).getPoints() : undefined
            },
            _hasSeriesPoints: function() {
                return !!this.viewModel.SeriesAxisName
            },
            _hasSparklinePoints: function() {
                return !!this.viewModel.SparklineAxisName
            },
            _iterateKpiItems: function(delegate){},
            _getGaugeRange: function(element){}
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file cardDataController.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data;
        data.cardDataController = data.kpiDataController.inherit({
            ctor: function ctor(options) {
                this.callBase(options)
            },
            _iterateKpiItems: function(delegate) {
                var that = this;
                if (that.viewModel)
                    $.each(that.viewModel.Cards, function(_, card) {
                        delegate(card)
                    })
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file gaugeDataController.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            utils = dashboard.utils,
            data = dashboard.data;
        data.gaugeDataController = data.kpiDataController.inherit({
            ctor: function ctor(options) {
                this.callBase(options);
                this._gaugeRanges = {}
            },
            _iterateKpiItems: function(delegate) {
                var that = this;
                if (that.viewModel)
                    $.each(that.viewModel.Gauges, function(_, gauge) {
                        delegate(gauge)
                    })
            },
            _getGaugeRange: function(element) {
                var elementId = element.ID,
                    range = this._gaugeRanges[elementId],
                    calculator;
                if (!range) {
                    calculator = new data.gaugeRangeCalculator({
                        values: this._getGaugeValues(element),
                        gaugeModel: {
                            Type: this._gaugeViewType,
                            MinValue: element.MinValue,
                            MaxValue: element.MaxValue
                        }
                    });
                    range = calculator.getGaugeRange();
                    this._gaugeRanges[elementId] = range
                }
                return range
            },
            _getGaugeValues: function(element) {
                var multiData = this.multiData,
                    gaugeValues = [],
                    axisPoints = this._axisPoints || [null],
                    getMeasureValue = function(axisPoint) {
                        var getMeasure = axisPoint ? multiData.getMeasureValueByAxisPoints : multiData.getMeasureValue;
                        gaugeValues.push(getMeasure.call(multiData, element.ID, [axisPoint]).getValue())
                    },
                    getDeltaValue = function(axisPoint) {
                        var getDelta = axisPoint ? multiData.getDeltaValueByAxisPoints : multiData.getDeltaValue,
                            deltaValue = getDelta.call(multiData, element.ID, [axisPoint]),
                            actualValue = deltaValue.getActualValue(),
                            targetValue = deltaValue.getTargetValue();
                        gaugeValues.push(actualValue.getValue());
                        gaugeValues.push(targetValue.getValue())
                    },
                    getter = element.DataItemType === utils.KpiValueMode.Measure ? getMeasureValue : getDeltaValue;
                $.each(axisPoints, function(_, axisPoint) {
                    getter(axisPoint)
                });
                return gaugeValues
            },
            _initialize: function() {
                this.callBase();
                this._gaugeViewType = this.viewModel ? this.viewModel.ViewType : undefined
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file dataStorage.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            Class = DevExpress.require("/class"),
            data = dashboard.data;
        data.dataStorage = Class.inherit({
            ctor: function ctor(dto) {
                this._sliceRep = this._createSliceRep(dto)
            },
            _createSliceRep: function(dto) {
                var decodeMaps = dto.EncodeMaps,
                    encodeMaps = {},
                    encodeCounters = {},
                    sliceListDTO = dto.Slices,
                    decode = function(keyId, key) {
                        return decodeMaps[keyId][key]
                    },
                    encode = function(keyId, value) {
                        var map = encodeMaps[keyId];
                        if (!map) {
                            map = {};
                            var decodeMap = decodeMaps[keyId];
                            if (!decodeMap) {
                                decodeMap = [];
                                decodeMaps[keyId] = decodeMap
                            }
                            encodeCounters[keyId] = decodeMap.length;
                            $.each(decodeMap, function(index, value) {
                                map[value] = index
                            });
                            encodeMaps[keyId] = map
                        }
                        var code = map[value];
                        if (code === undefined) {
                            var counter = encodeCounters[keyId];
                            map[value] = counter;
                            encodeCounters[keyId] = ++counter;
                            decodeMaps[keyId].push(value)
                        }
                        return map[value]
                    };
                return new data.dataStorage.sliceRepository(sliceListDTO, decode, encode)
            },
            _initialize: function(){},
            getSlices: function() {
                return this._sliceRep.getAll()
            },
            getSlice: function(sliceKey) {
                return this._sliceRep.get(sliceKey)
            },
            getSliceKey: function(keyIds) {
                return this._sliceRep.getKey(keyIds)
            },
            getSliceByIds: function(keyIds) {
                return this._sliceRep._getByKeyIds(keyIds)
            },
            getOrCreateSlice: function(keyIds) {
                return this._sliceRep.getOrCreate(keyIds)
            },
            findDataRowKey: function(sliceKey, dataRowKey) {
                return this._sliceRep.findDataRowKey(sliceKey, dataRowKey)
            },
            getCrossValue: function(dataRowKeys, valueId) {
                return this._sliceRep.getCrossValue(dataRowKeys, valueId)
            },
            getKeyValue: function(dataRow, keyId) {
                return this._sliceRep.getKeyValue(dataRow, keyId)
            },
            getValue: function(dataRow, valueId) {
                return this._sliceRep.getValue(dataRow, valueId)
            },
            insert: function(ds, sortOrderSlices) {
                var that = this,
                    slices = ds.getSlices(),
                    iterators = {};
                $.each(slices, function(i, slice) {
                    var keyIds = slice.getKeyIds();
                    var ownSlice = that.getOrCreateSlice(keyIds);
                    iterators[ownSlice.getKey()] = ownSlice.append(slice)
                });
                return iterators
            }
        });
        data.dataStorage.dataSlice = DevExpress.require("/class").inherit({
            ctor: function ctor(sliceKey, sliceDTO, decode, encode) {
                var keyIndexById = {},
                    valueIdByKey = {};
                $.each(sliceDTO.KeyIds, function(i, keyId) {
                    keyIndexById[keyId] = i
                });
                $.each(sliceDTO.ValueIds, function(valueId, key) {
                    valueIdByKey[key] = valueId
                });
                this._sliceKey = sliceKey;
                this._sliceDTO = sliceDTO;
                this._decode = decode;
                this._encode = encode;
                this._keyIndexById = keyIndexById;
                this._valueIdByKey = valueIdByKey
            },
            getKey: function() {
                return this._sliceKey
            },
            getValue: function(rowKey, valueId) {
                var that = this,
                    dto = that._sliceDTO,
                    valueKey = dto.ValueIds[valueId],
                    rowDTO = that._getRowDTO(rowKey),
                    value = !!rowDTO && valueKey >= 0 ? rowDTO[valueKey] : null;
                return value === undefined ? null : value
            },
            getRowValues: function(rowKey) {
                var that = this,
                    values = {},
                    valueIdsByKey = that._valueIdByKey,
                    rowDTO = that._getRowDTO(rowKey);
                $.each(rowDTO, function(key, value) {
                    values[valueIdsByKey[key]] = value
                });
                return values
            },
            getRowKeyValues: function(rowKey) {
                var that = this,
                    keyIds = that.getKeyIds(),
                    keyValues = {};
                $.each(keyIds, function(_, keyId) {
                    keyValues[keyId] = that.getKeyValue(rowKey, keyId)
                });
                return keyValues
            },
            _getRowDTO: function(rowKey) {
                var that = this;
                return that._sliceDTO.Data[that._stringifyKey(rowKey)]
            },
            getKeyValue: function(rowKey, keyId) {
                if (keyId === undefined)
                    return null;
                var that = this,
                    keyIndex = that._keyIndexById[keyId];
                return that._decode(keyId, rowKey[keyIndex])
            },
            getKeyIds: function() {
                var that = this;
                return that._sliceDTO.KeyIds
            },
            forEach: function(action) {
                var that = this;
                $.each(that._sliceDTO.Data, function(key) {
                    action({
                        sliceKey: that._sliceKey,
                        rowKey: that._parseKey(key)
                    })
                })
            },
            append: function(slice) {
                var that = this,
                    newRowKeys = [],
                    iterator = {forEach: function(action) {
                            $.each(newRowKeys, function(_, key) {
                                action({
                                    sliceKey: that._sliceKey,
                                    rowKey: key
                                })
                            })
                        }};
                slice.forEach(function(key) {
                    var keyValues = slice.getRowKeyValues(key.rowKey),
                        values = slice.getRowValues(key.rowKey),
                        newRowKey = that.addRow(keyValues, values);
                    newRowKeys.push(newRowKey)
                });
                return iterator
            },
            addRow: function(keyValues, values) {
                var that = this,
                    newRowKey = [],
                    valueIds = that._sliceDTO.ValueIds,
                    encode = that._encode;
                $.each(keyValues, function(keyId, keyValue) {
                    newRowKey.push(encode(keyId, keyValue))
                });
                var valueDTO = {};
                $.each(values, function(valueId, value) {
                    var valueKey = valueIds[valueId];
                    if (valueKey === undefined) {
                        var count = 0;
                        $.each(valueIds, function() {
                            count++
                        });
                        valueKey = count;
                        valueIds[valueId] = valueKey
                    }
                    valueDTO[valueKey] = value
                });
                that._sliceDTO.Data[that._stringifyKey(newRowKey)] = valueDTO;
                return newRowKey
            },
            _parseKey: function(key) {
                return JSON.parse(key)
            },
            _stringifyKey: function(key) {
                return '[' + key + ']'
            }
        });
        data.dataStorage.sliceRepository = DevExpress.require("/class").inherit({
            ctor: function ctor(sliceListDTO, decode, encode) {
                this._sliceListDTO = sliceListDTO;
                this._sliceList = [];
                this._rowKeyConvertMap = {};
                this._sliceJoinCache = {};
                this._decode = decode;
                this._encode = encode;
                this._initialize(decode)
            },
            _initialize: function(decode) {
                var that = this;
                $.each(that._sliceListDTO, function(index, sliceDTO) {
                    var slice = new data.dataStorage.dataSlice(index, sliceDTO, decode, that._encode);
                    that._sliceList.push(slice)
                })
            },
            getAll: function() {
                return this._sliceList
            },
            getKey: function(keyIds) {
                var that = this,
                    slice = that._getByKeyIds(keyIds);
                return slice ? $.inArray(slice, that._sliceList) : -1
            },
            get: function(vsKey) {
                return this._sliceList[vsKey]
            },
            getOrCreate: function(keyIds) {
                var that = this,
                    slice = that._getByKeyIds(keyIds);
                if (!slice) {
                    var sliceDTO = {
                            KeyIds: keyIds,
                            ValueIds: {},
                            Data: {}
                        };
                    if (keyIds.length == 0)
                        sliceDTO.Data['[]'] = {};
                    slice = new data.dataStorage.dataSlice(that._sliceList.length, sliceDTO, that._decode, that._encode);
                    that._sliceList.push(slice)
                }
                return slice
            },
            findDataRowKey: function(sliceKey, dataRowKey) {
                var that = this,
                    newRowKey = [],
                    map = that._getConvertMap(dataRowKey.sliceKey, sliceKey);
                for (var i = 0; i < map.length; i++)
                    newRowKey.push(dataRowKey.rowKey[map[i]]);
                return {
                        sliceKey: sliceKey,
                        rowKey: newRowKey
                    }
            },
            getCrossValue: function(dataRows, valueId) {
                var that = this,
                    dataRow1 = dataRows[0],
                    dataRow2 = dataRows[1],
                    sliceKey = dataRow2 ? that._joinSliceKey(dataRow1.sliceKey, dataRow2.sliceKey) : dataRow1.sliceKey,
                    value = null;
                if (sliceKey >= 0) {
                    var newRowKey = [],
                        map1 = that._getConvertMap(dataRow1.sliceKey, sliceKey),
                        map2 = dataRow2 ? that._getConvertMap(dataRow2.sliceKey, sliceKey) : null;
                    for (var i = 0; i < map1.length; i++) {
                        var index = map1[i],
                            key = index >= 0 ? dataRow1.rowKey[index] : newRowKey[i];
                        newRowKey.push(key)
                    }
                    if (map2 != null)
                        for (var i = 0; i < map2.length; i++) {
                            var index = map2[i],
                                key = index >= 0 ? dataRow2.rowKey[index] : newRowKey[i];
                            newRowKey[i] = key
                        }
                    value = that.get(sliceKey).getValue(newRowKey, valueId)
                }
                return value
            },
            getKeyValue: function(dataRow, keyId) {
                var that = this,
                    slice = that.get(dataRow.sliceKey),
                    value = null;
                if (slice)
                    value = slice.getKeyValue(dataRow.rowKey, keyId);
                return value
            },
            getValue: function(dataRow, valueId) {
                var that = this,
                    slice = that.get(dataRow.sliceKey),
                    value = null;
                if (slice)
                    value = slice.getValue(dataRow.rowKey, valueId);
                return value
            },
            _joinSliceKey: function(key1, key2) {
                var that = this,
                    joinSliceCacheKey = [key1, key2];
                if (key2 < key1)
                    joinSliceCacheKey = joinSliceCacheKey.reverse();
                var joinRes = that._sliceJoinCache[joinSliceCacheKey];
                if (joinRes == undefined) {
                    var slice1 = that.get(key1),
                        slice2 = that.get(key2),
                        keyIds = slice1.getKeyIds().concat(slice2.getKeyIds());
                    joinRes = that.getKey(keyIds);
                    that._sliceJoinCache[joinSliceCacheKey] = joinRes
                }
                return joinRes
            },
            _getByKeyIds: function(keyIds) {
                var that = this,
                    foundSlice = null;
                $.each(that._sliceList, function(_, slice) {
                    if (dashboard.utils.areNotOrderedListsEqual(slice.getKeyIds(), keyIds)) {
                        foundSlice = slice;
                        return false
                    }
                });
                return foundSlice
            },
            _getConvertMap: function(sliceFromKey, sliceToKey) {
                var that = this,
                    convertMapCacheKey = [sliceFromKey, sliceToKey];
                var map = that._rowKeyConvertMap[convertMapCacheKey];
                if (!map) {
                    var fromSlice = that.get(sliceFromKey),
                        toSlice = that.get(sliceToKey);
                    map = [];
                    $.each(toSlice.getKeyIds(), function(_, keyId) {
                        map.push($.inArray(keyId, fromSlice.getKeyIds()))
                    });
                    that._rowKeyConvertMap[convertMapCacheKey] = map
                }
                return map
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file itemDataAxisBuilder.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data;
        data.dataStorageSpecialIds = {
            DisplayText: '_DisplayText_{4873F9E9-65B2-4307-BB25-BFD09F6A2E54}',
            Value: '_Value_{E5597004-313E-4F79-B02E-DEA46EEB1BFE}'
        };
        data.pivotAreaNames = {
            columns: 'Columns',
            rows: 'Rows'
        };
        data.itemDataAxisBuilder = {
            build: function(name, storage, dimensions, sortOrderSlices, metaData, iterators) {
                var that = this;
                var keyIds = [];
                $.each(dimensions, function(_, dimension) {
                    keyIds.push(dimension.id)
                });
                var allSlicesKeyIdsList = that._getKeyIdsList(keyIds),
                    cache = {},
                    levelInfoList = [];
                $.each(allSlicesKeyIdsList, function(index, keyIds) {
                    var baseKeyIds = keyIds.slice(-1),
                        baseKeyId = baseKeyIds.length > 0 ? baseKeyIds[0] : null,
                        metaDataSliceKey = storage.getSliceKey(baseKeyIds),
                        dataSlice = storage.getOrCreateSlice(keyIds),
                        level = index - 1;
                    levelInfoList.push({
                        axisName: name,
                        metaData: metaData,
                        dataSlice: dataSlice,
                        level: level,
                        getMetaDataValue: function(dataRowKey, valueId) {
                            if (metaDataSliceKey < 0)
                                return null;
                            var metaDataRowKey = storage.findDataRowKey(metaDataSliceKey, dataRowKey);
                            return storage.getValue(metaDataRowKey, valueId)
                        },
                        getBaseValue: function(dataRowKey) {
                            return level >= 0 ? storage.getKeyValue(dataRowKey, baseKeyId) : null
                        }
                    })
                });
                $.each(allSlicesKeyIdsList, function(_, keyIds) {
                    if (keyIds.length > 0 && !that._isSortOrderSlice(keyIds, sortOrderSlices))
                        return;
                    var levelInfo = levelInfoList[keyIds.length];
                    var slice = levelInfo.dataSlice;
                    var iterator = slice && iterators ? iterators[slice.getKey()] : slice;
                    if (iterator)
                        iterator.forEach(function(dataRowKey) {
                            var item = null,
                                childItem = null,
                                childIsRaggedItem = false;
                            do {
                                var level = dataRowKey.rowKey.length;
                                item = cache[dataRowKey.rowKey];
                                var isRaggedItem = false;
                                var exists = !!item;
                                if (!exists) {
                                    var levelInfo = levelInfoList[level];
                                    isRaggedItem = levelInfo.getBaseValue(dataRowKey) === dashboard.utils.specialValues.olapNullValueGuid;
                                    item = new data.itemDataAxisPoint(levelInfo, dataRowKey);
                                    cache[dataRowKey.rowKey] = item
                                }
                                if (childItem != null) {
                                    var children = childIsRaggedItem ? item._getRaggedChildren() : item.getChildren();
                                    children.push(childItem);
                                    childItem._setParent(item)
                                }
                                if (exists || level == 0)
                                    break;
                                var prevSliceKey = levelInfoList[level - 1].dataSlice.getKey();
                                dataRowKey = storage.findDataRowKey(prevSliceKey, dataRowKey);
                                childItem = item;
                                childIsRaggedItem = isRaggedItem
                            } while (true)
                        })
                });
                return cache[[]]
            },
            _getKeyIdsList: function(keyIds) {
                var list = [[]];
                $.each(keyIds, function(i, _) {
                    var slice = keyIds.slice(0, i + 1);
                    list.push(slice)
                });
                return list
            },
            _isSortOrderSlice: function(slice, sortOrderSlices) {
                var result = !sortOrderSlices || sortOrderSlices.length == 0;
                if (!result)
                    $.each(sortOrderSlices, function(_, orderSlice) {
                        result = result || dashboard.utils.areNotOrderedListsEqual(slice, orderSlice);
                        return !result
                    });
                return result
            }
        }
    })(jQuery, DevExpress);
    /*! Module dashboard, file itemMetaData.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            Class = DevExpress.require("/class"),
            data = dashboard.data;
        data.deltaValueNames = {
            actualValue: 'actualValue',
            targetValue: 'targetValue',
            absoluteVariation: 'absoluteVariation',
            percentVariation: 'percentVariation',
            percentOfTarget: 'percentOfTarget',
            mainValue: 'mainValue',
            subValue1: 'subValue1',
            subValue2: 'subValue2',
            isGood: 'isGood',
            indicatorType: 'indicatorType'
        };
        data.deltaValueTypes = {
            actualValue: 'ActualValue',
            absoluteVariation: 'AbsoluteVariation',
            percentVariation: 'PercentVariation',
            percentOfTarget: 'PercentOfTarget'
        };
        data.itemMetaData = Class.inherit({
            ctor: function ctor(metaData) {
                this._metaData = metaData;
                this._data = {}
            },
            initialize: function() {
                var data = this._data,
                    metaData = this._metaData;
                data.measuresInfo = this._createMeasureInfo(metaData.MeasureDescriptors);
                data.colorMeasuresInfo = this._createMeasureInfo(metaData.ColorMeasureDescriptors);
                data.conditionalFormattingMeasuresInfo = this._createMeasureInfo(metaData.FormatConditionMeasureDescriptors);
                data.deltaInfo = this._createDeltaInfo();
                data.axesInfo = this._createAxesInfo();
                data.dataSourceColumns = this._metaData.DataSourceColumns
            },
            _createMeasureInfo: function(descriptors) {
                var measures = [],
                    formatByMeasureId = {};
                if (descriptors)
                    $.each(descriptors, function(_, _measure) {
                        var measure = {
                                id: _measure.ID,
                                name: _measure.Name,
                                dataMember: _measure.DataMember,
                                summaryType: _measure.SummaryType,
                                format: function(value) {
                                    var format = _measure.Format,
                                        text = undefined;
                                    if (format)
                                        text = data.formatter.format(value, format);
                                    return text
                                }
                            };
                        measures.push(measure);
                        formatByMeasureId[measure.id] = _measure.Format
                    });
                return {
                        measures: measures,
                        formatByMeasureId: formatByMeasureId
                    }
            },
            _createDeltaInfo: function() {
                var metaData = this._metaData,
                    names = data.deltaValueNames,
                    deltaValueTypes = data.deltaValueTypes,
                    deltas = [],
                    valueIdsByDeltaId = {},
                    formatsByDeltaId = {};
                if (metaData.DeltaDescriptors)
                    $.each(metaData.DeltaDescriptors, function(_, _delta) {
                        var delta = {
                                id: _delta.ID,
                                name: _delta.Name,
                                actualMeasureId: _delta.ActualMeasureID,
                                targetMeasureId: _delta.TargetMeasureID
                            },
                            ids = {},
                            formats = {};
                        deltas.push(delta);
                        ids[names.actualValue] = _delta.ActualValueID;
                        ids[names.targetValue] = _delta.TargetValueID;
                        ids[names.absoluteVariation] = _delta.AbsoluteVariationID;
                        ids[names.percentVariation] = _delta.PercentVariationID;
                        ids[names.percentOfTarget] = _delta.PercentOfTargetID;
                        ids[names.isGood] = _delta.IsGoodID;
                        ids[names.indicatorType] = _delta.IndicatorTypeID;
                        formats[names.actualValue] = _delta.ActualValueFormat;
                        formats[names.targetValue] = _delta.ActualValueFormat;
                        formats[names.absoluteVariation] = _delta.AbsoluteVariationFormat;
                        formats[names.percentVariation] = _delta.PercentVariationFormat;
                        formats[names.percentOfTarget] = _delta.PercentOfTargetFormat;
                        switch (_delta.DeltaValueType) {
                            case deltaValueTypes.actualValue:
                                ids[names.mainValue] = ids[names.actualValue];
                                ids[names.subValue1] = ids[names.absoluteVariation];
                                ids[names.subValue2] = ids[names.percentVariation];
                                formats[names.mainValue] = formats[names.actualValue];
                                formats[names.subValue1] = formats[names.absoluteVariation];
                                formats[names.subValue2] = formats[names.percentVariation];
                                break;
                            case deltaValueTypes.absoluteVariation:
                                ids[names.mainValue] = ids[names.absoluteVariation];
                                ids[names.subValue1] = ids[names.actualValue];
                                ids[names.subValue2] = ids[names.percentVariation];
                                formats[names.mainValue] = formats[names.absoluteVariation];
                                formats[names.subValue1] = formats[names.actualValue];
                                formats[names.subValue2] = formats[names.percentVariation];
                                break;
                            case deltaValueTypes.percentVariation:
                                ids[names.mainValue] = ids[names.percentVariation];
                                ids[names.subValue1] = ids[names.actualValue];
                                ids[names.subValue2] = ids[names.absoluteVariation];
                                formats[names.mainValue] = formats[names.percentVariation];
                                formats[names.subValue1] = formats[names.actualValue];
                                formats[names.subValue2] = formats[names.absoluteVariation];
                                break;
                            case deltaValueTypes.percentOfTarget:
                                ids[names.mainValue] = ids[names.percentOfTarget];
                                ids[names.subValue1] = ids[names.actualValue];
                                ids[names.subValue2] = ids[names.absoluteVariation];
                                formats[names.mainValue] = formats[names.percentOfTarget];
                                formats[names.subValue1] = formats[names.actualValue];
                                formats[names.subValue2] = formats[names.absoluteVariation];
                                break
                        }
                        valueIdsByDeltaId[delta.id] = ids;
                        formatsByDeltaId[delta.id] = formats
                    });
                return {
                        deltas: deltas,
                        valueIdsByDeltaId: valueIdsByDeltaId,
                        formatsByDeltaId: formatsByDeltaId
                    }
            },
            _createAxesInfo: function() {
                var metaData = this._metaData,
                    axes = {},
                    dimensionDescriptors = metaData.DimensionDescriptors || {},
                    levelByDimensionId = {},
                    formatByDimensionId = {},
                    pivotAreaByAxisName = {};
                $.each(dimensionDescriptors, function(_name, _dimensions) {
                    var dimensions = [];
                    if (_dimensions)
                        $.each(_dimensions, function(_, _dimension) {
                            var dimension = {
                                    id: _dimension.ID,
                                    name: _dimension.Name,
                                    dataMember: _dimension.DataMember,
                                    dateTimeGroupInterval: _dimension.DateTimeGroupInterval,
                                    textGroupInterval: _dimension.TextGroupInterval,
                                    getFormat: function() {
                                        return data.formatter.convertToFormat(_dimension.Format)
                                    },
                                    format: function(value) {
                                        var format = _dimension.Format,
                                            text = undefined;
                                        if (format)
                                            text = data.formatter.format(value, format);
                                        return text
                                    }
                                };
                            levelByDimensionId[dimension.id] = _dimension.Level;
                            formatByDimensionId[dimension.id] = _dimension.Format;
                            dimensions.push(dimension)
                        });
                    axes[_name] = dimensions
                });
                if (metaData.ColumnHierarchy)
                    pivotAreaByAxisName[metaData.ColumnHierarchy] = 'Columns';
                if (metaData.RowHierarchy)
                    pivotAreaByAxisName[metaData.RowHierarchy] = 'Rows';
                return {
                        axes: axes,
                        levelByDimensionId: levelByDimensionId,
                        formatByDimensionId: formatByDimensionId,
                        pivotAreaByAxisName: pivotAreaByAxisName
                    }
            },
            getAxes: function() {
                return this._data.axesInfo.axes
            },
            getPivotAreaByAxisName: function(name) {
                return this._data.axesInfo.pivotAreaByAxisName[name]
            },
            getColorMeasures: function() {
                return this._data.colorMeasuresInfo.measures
            },
            getConditionalFormattingMeasures: function() {
                return this._data.conditionalFormattingMeasuresInfo.measures
            },
            getMeasures: function() {
                return this._data.measuresInfo.measures
            },
            getDeltas: function() {
                return this._data.deltaInfo.deltas
            },
            getMeasureById: function(id) {
                var measures = this.getMeasures(),
                    foundMeasures = $.grep(measures, function(measure, i) {
                        return measure.id == id
                    });
                return foundMeasures[0]
            },
            getDeltaById: function(id) {
                var deltas = this.getDeltas(),
                    foundDeltas = $.grep(deltas, function(delta, i) {
                        return delta.id == id
                    });
                return foundDeltas[0]
            },
            getMeasureFormat: function(measureId) {
                return this._data.measuresInfo.formatByMeasureId[measureId]
            },
            getDeltaValueIds: function(deltaId) {
                return this._data.deltaInfo.valueIdsByDeltaId[deltaId]
            },
            getDeltaFormats: function(deltaId) {
                return this._data.deltaInfo.formatsByDeltaId[deltaId]
            },
            getDeltaValueType: function(deltaId){},
            getDimensionLevel: function(dimensionId) {
                return this._data.axesInfo.levelByDimensionId[dimensionId]
            },
            getDimensionFormat: function(dimensionId) {
                return this._data.axesInfo.formatByDimensionId[dimensionId]
            },
            getDataMembers: function() {
                return this._data.dataSourceColumns
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file itemDataAxisHelper.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data;
        data.itemDataAxisHelper = {
            eachPoint: function(point, _process) {
                var that = this,
                    children = point.getChildren();
                if (_process(point) === false)
                    return false;
                $.each(children, function(_, childPoint) {
                    if (that.eachPoint(childPoint, _process) === false)
                        return false
                })
            },
            findFirstPoint: function(root, predicate) {
                var that = this,
                    foundPoint = undefined;
                that.eachPoint(root, function(point) {
                    var points = point.getAxisPath();
                    if (predicate(points)) {
                        foundPoint = point;
                        return false
                    }
                });
                return foundPoint
            },
            findFirstPointByUniqueValues: function(root, values) {
                var that = this;
                return that.findFirstPoint(root, function(points) {
                        return that._searchPredicate(points, values, function(value, point) {
                                return that._areEqual(value, point.getUniqueValue())
                            })
                    })
            },
            findFirstPointByValues: function(root, values) {
                var that = this;
                return that.findFirstPoint(root, function(points) {
                        return that._searchPredicate(points, values, function(value, point) {
                                return that._areEqual(value, point.getValue())
                            })
                    })
            },
            _areEqual: function(value1, value2) {
                return DX.data.utils.toComparable(value1, false) === DX.data.utils.toComparable(value2, false)
            },
            _searchPredicate: function(points, values, equal) {
                values = values || [];
                if (points.length != values.length)
                    return false;
                var passes = true;
                $.each(values, function(index, value) {
                    passes = passes && equal(value, points[index]);
                    return passes
                });
                return passes
            },
            forSamePoints: function(baseItem, item, process) {
                var that = this;
                process(baseItem, item);
                $.each(baseItem.getChildren(), function(_, baseChild) {
                    var child = that.findChildByUniqueValue(item, baseChild.getUniqueValue());
                    if (child)
                        that.forSamePoints(baseChild, child, process)
                })
            },
            findChildByUniqueValue: function(point, value) {
                var that = this,
                    children = point.getChildren(),
                    foundPoint = undefined;
                $.each(children, function(_, child) {
                    if (that._areEqual(value, child.getUniqueValue())) {
                        foundPoint = child;
                        return false
                    }
                });
                return foundPoint
            }
        }
    })(jQuery, DevExpress);
    /*! Module dashboard, file itemDataAxis.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            Class = DevExpress.require("/class"),
            data = dashboard.data,
            helper = data.itemDataAxisHelper;
        data.itemDataAxis = Class.inherit({
            ctor: function ctor(dimensions, axisPoint) {
                this._dimensions = dimensions;
                this._axisPoint = axisPoint
            },
            getDimensions: function() {
                return this._dimensions
            },
            getRootPoint: function() {
                var getRoot = function(point) {
                        var parent = point.getParent();
                        if (parent)
                            return getRoot(parent);
                        return point
                    };
                return getRoot(this._axisPoint)
            },
            getPoints: function(includeRaggedHierarchy) {
                var dimensions = this.getDimensions(),
                    lastLevelDimension = dimensions ? dimensions[dimensions.length - 1] : null;
                return lastLevelDimension ? this.getPointsByDimension(lastLevelDimension.id, !!includeRaggedHierarchy) : []
            },
            getPointsByDimension: function(dimensionId, includeRaggedHierarchy) {
                var root = this.getRootPoint(),
                    points = [];
                if (dimensionId)
                    helper.eachPoint(root, function(point) {
                        var dimension = point.getDimension();
                        if (dimension && dimension.id == dimensionId || includeRaggedHierarchy && point.getParent() && point.getChildren().length === 0)
                            points.push(point)
                    });
                else
                    points.push(root);
                return points
            },
            getPointByUniqueValues: function(values) {
                return helper.findFirstPointByUniqueValues(this.getRootPoint(), values)
            },
            getPointByValues: function(values) {
                return helper.findFirstPointByValues(this.getRootPoint(), values)
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file itemData.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            Class = DevExpress.require("/class"),
            data = dashboard.data;
        data.itemData = Class.inherit({
            ctor: function ctor(data, rootItems) {
                this._data = data;
                this._rootItems = rootItems
            },
            getAllSelectionValues: function(dimensionIds) {
                var multiData = this,
                    allAxisNames = multiData.getAxisNames(),
                    axisNames = [],
                    selectionList = [],
                    fillAvailableValues = function(axes, row, inputSelection) {
                        var firstAxis = multiData.getAxis(axes[0]),
                            nextAxes = axes.slice(1, axes.length - 1),
                            axisPoints = firstAxis.getPoints() || [];
                        $.each(axisPoints, function(_, axisPoint) {
                            var newRow = row.slice();
                            $.each(axisPoint.getAxisPath(), function(__, pathPoint) {
                                if ($.grep(dimensionIds, function(id) {
                                    return id === pathPoint.getDimension().id
                                }).length > 0)
                                    newRow.push(pathPoint.getUniqueValue())
                            });
                            if (axes.length > 1)
                                fillAvailableValues(nextAxes, newRow, inputSelection);
                            else
                                inputSelection.push(newRow)
                        })
                    };
                $.each(dimensionIds, function(_, id) {
                    $.each(allAxisNames, function(__, axisName) {
                        if ($.grep(multiData.getAxis(axisName).getDimensions(), function(descr) {
                            return descr.id === id
                        }).length > 0 && $.inArray(axisName, axisNames) === -1)
                            axisNames.push(axisName)
                    })
                });
                fillAvailableValues(axisNames, [], selectionList);
                return selectionList
            },
            getAxisNames: function() {
                var names = [];
                $.each(this._data.metaData.getAxes(), function(name) {
                    names.push(name)
                });
                return names
            },
            getAxis: function(axisName) {
                if (axisName === undefined)
                    axisName = 'Default';
                var dimensions = this._data.metaData.getAxes()[axisName],
                    root = this._rootItems[axisName];
                return new data.itemDataAxis(dimensions, root)
            },
            getDimensions: function(axisName) {
                return this.getAxis(axisName).getDimensions()
            },
            getColorMeasures: function() {
                return this._data.metaData.getColorMeasures()
            },
            getMeasures: function() {
                return this._data.metaData.getMeasures()
            },
            getDeltas: function() {
                return this._data.metaData.getDeltas()
            },
            getMeasureById: function(id) {
                return this._data.metaData.getMeasureById(id)
            },
            getDeltaById: function(id) {
                return this._data.metaData.getDeltaById(id)
            },
            getSlice: function(value) {
                return value instanceof dashboard.data.itemDataTuple ? this._getSliceByTuple(value) : value instanceof dashboard.data.itemDataAxisPoint ? this._getSliceByAxisPoint(value) : null
            },
            getMeasureFormat: function(measureId) {
                return this._data.metaData.getMeasureFormat(measureId)
            },
            getColorMeasureValue: function(colorMeasureId) {
                return this._getValue(colorMeasureId)
            },
            getConditionalFormattingMeasureValue: function(cfMeasureId) {
                return this._getValue(cfMeasureId)
            },
            getMeasureValue: function(measureId) {
                var that = this,
                    format = that.getMeasureFormat(measureId);
                return that._getMeasureValueByKeys(that._getKeys(), measureId, format)
            },
            _getKeys: function(points) {
                var that = this,
                    rootItems = that._rootItems,
                    keysList = [];
                $.each(rootItems, function(axisName, root) {
                    var userPoint = points && points[axisName],
                        point = userPoint || root;
                    keysList.push(point.getKey())
                });
                return keysList
            },
            _getValue: function(measureId) {
                var that = this;
                return that._getCellValue(that._getKeys(), measureId)
            },
            _getMeasureValueByKeys: function(keys, mId, format) {
                var that = this;
                return {
                        getValue: function() {
                            return that._getCellValue(keys, mId)
                        },
                        getDisplayText: function() {
                            return that._getCellDisplayText(keys, mId, format)
                        }
                    }
            },
            _getDeltaValueByKeys: function(keys, deltaIds, formats) {
                var that = this,
                    names = data.deltaValueNames,
                    getValueItem = function(valueName) {
                        return {
                                getValue: function() {
                                    return that._getCellValue(keys, deltaIds[valueName])
                                },
                                getDisplayText: function() {
                                    var format = formats[valueName];
                                    if (format)
                                        format = {NumericFormat: format};
                                    return that._getCellDisplayText(keys, deltaIds[valueName], format)
                                }
                            }
                    };
                return {
                        getActualValue: function() {
                            return getValueItem(names.actualValue)
                        },
                        getTargetValue: function() {
                            return getValueItem(names.targetValue)
                        },
                        getAbsoluteVariation: function() {
                            return getValueItem(names.absoluteVariation)
                        },
                        getPercentVariation: function() {
                            return getValueItem(names.percentVariation)
                        },
                        getPercentOfTarget: function() {
                            return getValueItem(names.percentOfTarget)
                        },
                        getIsGood: function() {
                            return getValueItem(names.isGood)
                        },
                        getIndicatorType: function() {
                            return getValueItem(names.indicatorType)
                        },
                        getDisplayValue: function() {
                            return getValueItem(names.mainValue)
                        },
                        getDisplaySubValue1: function() {
                            return getValueItem(names.subValue1)
                        },
                        getDisplaySubValue2: function() {
                            return getValueItem(names.subValue2)
                        }
                    }
            },
            _createPointsHash: function(axisPoints) {
                var hash = {};
                for (var i = 0; i < axisPoints.length; i++) {
                    var areaName = axisPoints[i].getAxisName();
                    hash[areaName] = axisPoints[i]
                }
                return hash
            },
            getMeasureValueByAxisPoints: function(measureId, axisPoints) {
                var that = this,
                    format = that.getMeasureFormat(measureId),
                    pointsHash = that._createPointsHash(axisPoints);
                return that._getMeasureValueByKeys(that._getKeys(pointsHash), measureId, format)
            },
            getDeltaValue: function(deltaId) {
                var that = this,
                    metaData = that._data.metaData,
                    deltaIds = metaData.getDeltaValueIds(deltaId),
                    formats = metaData.getDeltaFormats(deltaId);
                return that._getDeltaValueByKeys(that._getKeys(), deltaIds, formats)
            },
            getDeltaValueByAxisPoints: function(deltaId, axisPoints) {
                var that = this,
                    metaData = this._data.metaData,
                    deltaIds = metaData.getDeltaValueIds(deltaId),
                    formats = metaData.getDeltaFormats(deltaId),
                    pointsHash = this._createPointsHash(axisPoints);
                return this._getDeltaValueByKeys(that._getKeys(pointsHash), deltaIds, formats)
            },
            getDataMembers: function() {
                return this._data.metaData.getDataMembers()
            },
            createTuple: function(values) {
                var that = this,
                    axisPoints = [];
                if (values[0] instanceof data.itemDataAxisPoint)
                    axisPoints = values;
                else
                    $.each(values, function(index, axisValue) {
                        var axis = that.getAxis(axisValue.AxisName),
                            axisPoint = axis.getPointByUniqueValues(axisValue.Value);
                        axisPoints.push(axisPoint)
                    });
                return new dashboard.data.itemDataTuple(axisPoints)
            },
            _getCellValue: function(keys, valueId) {
                return this._data.storage.getCrossValue(keys, valueId)
            },
            _getCellDisplayText: function(keys, valueId, format) {
                return format ? data.formatter.format(this._getCellValue(keys, valueId), format) : undefined
            },
            _getSliceByAxisPoint: function(axisPoint) {
                var that = this,
                    rootItems = that._rootItems,
                    newRootItems = {};
                $.each(rootItems, function(name, item) {
                    newRootItems[name] = axisPoint.getAxisName() === name ? axisPoint : item
                });
                return new data.itemData(that._data, newRootItems)
            },
            _getSliceByTuple: function(tuple) {
                var data = this;
                $.each(tuple._axisPoints, function(_, axisPoint) {
                    data = data._getSliceByAxisPoint(axisPoint)
                });
                return data
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file itemDataManager.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            Class = DevExpress.require("/class"),
            data = dashboard.data,
            helper = data.itemDataAxisHelper;
        data.itemDataManager = Class.inherit({
            ctor: function ctor(){},
            initialize: function(itemDataDTO) {
                var that = this,
                    metaDataDTO = itemDataDTO.MetaData,
                    dataStorageDTO = itemDataDTO.DataStorageDTO,
                    metaData = that._createMetaData(metaDataDTO);
                that._metaData = metaData;
                var dataStorage = new data.dataStorage(dataStorageDTO),
                    sortOrderSlices = itemDataDTO.SortOrderSlices,
                    items = {};
                $.each(metaData.getAxes(), function(name, dimensions) {
                    items[name] = data.itemDataAxisBuilder.build(name, dataStorage, dimensions, sortOrderSlices, metaData)
                });
                var itemData = new data.itemData({
                        metaData: that._metaData,
                        storage: dataStorage
                    }, items);
                this._items = items;
                this._itemData = itemData;
                this._dataStorage = dataStorage
            },
            updateExpandedData: function(expandedItemDataDTO, expandInfo) {
                var that = this,
                    areaNames = dashboard.itemDataAxisNames,
                    dataStorageDTO = expandedItemDataDTO.DataStorageDTO,
                    sortOrderSlices = expandedItemDataDTO.SortOrderSlices,
                    area = expandInfo.pivotArea == data.pivotAreaNames.columns ? areaNames.pivotColumnAxis : areaNames.pivotRowAxis,
                    values = expandInfo.values,
                    metaData = that._metaData,
                    dataStorage = new data.dataStorage(dataStorageDTO);
                var iterators = that._dataStorage.insert(dataStorage, sortOrderSlices);
                var expandedAreaNewRootItem = data.itemDataAxisBuilder.build(area, that._dataStorage, metaData.getAxes()[area], sortOrderSlices, metaData, iterators);
                if (!!expandedAreaNewRootItem) {
                    var expandedAreaRootItem = that._items[area],
                        expandedItem = helper.findFirstPointByUniqueValues(expandedAreaRootItem, values),
                        expandedNewItem = helper.findFirstPointByUniqueValues(expandedAreaNewRootItem, values);
                    if (!!expandedNewItem) {
                        var newChildren = expandedNewItem.getChildren();
                        $.each(newChildren, function(_, child) {
                            child._setParent(expandedItem)
                        });
                        expandedItem._setChildren(newChildren)
                    }
                }
            },
            getItemData: function() {
                return this._itemData
            },
            getMetaData: function() {
                return this._metaData
            },
            _createMetaData: function(metaDataDTO) {
                var metaData = new dashboard.data.itemMetaData(metaDataDTO);
                metaData.initialize();
                return metaData
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file itemDataTuple.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data;
        data.itemDataTuple = function(axisPoints) {
            this._axisPoints = axisPoints
        };
        data.itemDataTuple.prototype = {
            constructor: data.itemDataTuple,
            getAxisPoint: function(axisName) {
                if (axisName)
                    return $.grep(this._axisPoints, function(axisPoint) {
                            return axisPoint.getAxisName() == axisName
                        })[0];
                else
                    return this._axisPoints[0]
            }
        }
    })(jQuery, DevExpress);
    /*! Module dashboard, file itemDataAxisPoint.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            data = dashboard.data;
        data.itemDataAxisPoint = function(levelInfo, dataRowKey) {
            var that = this;
            that._info = levelInfo;
            that._dataRowKey = dataRowKey;
            that._children = [];
            that._parent = undefined;
            that._raggedChildren = []
        };
        data.itemDataAxisPoint.prototype = {
            constructor: data.itemDataAxisPoint,
            _getSpecialValue: function(specialId) {
                var that = this,
                    info = that._info;
                return info.getMetaDataValue(that._dataRowKey, specialId)
            },
            getUniqueValue: function() {
                var that = this,
                    info = that._info;
                return info.getBaseValue(that._dataRowKey)
            },
            getValue: function() {
                var that = this;
                var value = that._getSpecialValue(data.dataStorageSpecialIds.Value);
                if (value === null || value === undefined)
                    value = that.getUniqueValue();
                if (value === dashboard.utils.specialValues.nullValueGuid)
                    value = null;
                return value
            },
            _getLevel: function() {
                return this._info.level
            },
            _getServerText: function() {
                var that = this;
                return that._getSpecialValue(data.dataStorageSpecialIds.DisplayText)
            },
            getKey: function() {
                return this._dataRowKey
            },
            getAxisName: function() {
                return this._info.axisName
            },
            getChildren: function() {
                return this._children
            },
            getParent: function() {
                return this._parent
            },
            _setParent: function(parent) {
                this._parent = parent
            },
            _setChildren: function(children) {
                this._children = children
            },
            _getRaggedChildren: function() {
                return this._raggedChildren
            },
            getParentByDimensionId: function(dimensionId) {
                var current = this,
                    dimension;
                while (current.getParent()) {
                    dimension = current.getDimension();
                    if (dimension && dimension.id == dimensionId)
                        return current;
                    current = current.getParent()
                }
                return dimensionId ? this : current
            },
            getDimensionValue: function(dimensionId) {
                var that = this,
                    dimension = that.getDimension();
                if (!dimensionId || dimension && dimension.id == dimensionId)
                    return {
                            getValue: function() {
                                return that.getValue()
                            },
                            getUniqueValue: function() {
                                return that.getUniqueValue()
                            },
                            getDisplayText: function() {
                                return that.getDisplayText()
                            }
                        };
                else {
                    var parent = that.getParent();
                    return parent ? parent.getDimensionValue(dimensionId) : null
                }
            },
            getDisplayText: function() {
                var that = this,
                    displayText = that._getServerText();
                if (displayText == null) {
                    var dimension = that.getDimension();
                    if (dimension) {
                        var format = that._info.metaData.getDimensionFormat(dimension.id),
                            uniqueValue = this.getUniqueValue();
                        displayText = uniqueValue === dashboard.utils.specialValues.nullValueGuid || uniqueValue === dashboard.utils.specialValues.olapNullValueGuid ? data.formatter.format(uniqueValue, format) : data.formatter.format(this.getValue(), format)
                    }
                }
                return displayText
            },
            getDimension: function() {
                var that = this,
                    axisName = that.getAxisName(),
                    dimensions = that._info.metaData.getAxes()[axisName],
                    dimension = dimensions[that._getLevel()];
                return dimension
            },
            getDimensions: function() {
                var that = this,
                    parent = that.getParent();
                return parent ? parent.getDimensions().concat([that.getDimension()]) : []
            },
            getAxisPath: function() {
                return this._selectPath(undefined)
            },
            getUniquePath: function() {
                return this._selectPath(function(point) {
                        return point.getUniqueValue()
                    })
            },
            getValuePath: function(includeProc) {
                return this._selectIncludedPath(includeProc, function(point) {
                        return point.getValue()
                    })
            },
            getDisplayPath: function(includeProc) {
                return this._selectIncludedPath(includeProc, function(point) {
                        return point.getDisplayText()
                    })
            },
            getValues: function() {
                var value = [],
                    axisPoint = this;
                while (axisPoint.getUniqueValue() != null) {
                    value.push(axisPoint.getUniqueValue());
                    if (this.getDimensions().length == 1)
                        break;
                    axisPoint = axisPoint.getParent()
                }
                value.reverse()
            },
            _selectIncludedPath: function(includeProc, pointProc) {
                return this._selectPath(function(point) {
                        if (!includeProc || includeProc(point))
                            return pointProc(point);
                        else
                            return undefined
                    })
            },
            _selectPath: function(predicate) {
                var action = predicate ? predicate : function(axisPoint) {
                        return axisPoint
                    },
                    buildParentsList = function(axisPoint) {
                        var parent = axisPoint.getParent();
                        if (parent) {
                            var newValue = action(axisPoint);
                            return buildParentsList(parent).concat(newValue == undefined ? [] : [newValue])
                        }
                        else
                            return []
                    };
                return buildParentsList(this)
            },
            getPointsByDimensionId: function(dimensionId) {
                return this._getPointsByDimensionId(dimensionId, function(point) {
                        return point
                    })
            },
            getDisplayTextsByDimensionId: function(dimensionId) {
                return this._getPointsByDimensionId(dimensionId, function(point) {
                        return point.getDisplayText()
                    })
            },
            _getPointsByDimensionId: function(dimensionId, pointProc) {
                var result = [];
                this._findPoints(dimensionId, result, pointProc);
                return result
            },
            _findPoints: function(dimensionId, result, pointProc) {
                if (this.getDimension().id === dimensionId) {
                    result.push(pointProc(this));
                    return
                }
                var children = this.getChildren();
                for (var i = 0; i < children.length; i++)
                    children[i]._findPoints(dimensionId, result, pointProc)
            }
        }
    })(jQuery, DevExpress);
    /*! Module dashboard, file drillThroughDataWrapper.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            Class = DevExpress.require("/class"),
            data = dashboard.data;
        data.drillThroughDataWrapper = Class.inherit({
            ctor: function ctor(drillThroughData) {
                this._drillThroughData = drillThroughData
            },
            initialize: function() {
                var that = this;
                that._errorMessage = this._drillThroughData.ErrorMessage;
                if (that.isDataReceived()) {
                    that._data = {};
                    that._data.dataMembers = that._drillThroughData.DataMembers;
                    that._data.listSource = new data.listSource(that._drillThroughData.Data, that._data.dataMembers)
                }
            },
            getRowCount: function() {
                return this._data.listSource.getRowCount()
            },
            getRowValue: function(rowIndex, columnName) {
                return this._data.listSource.getRowValue(rowIndex, columnName)
            },
            getDataMembers: function() {
                return this._data.dataMembers
            },
            isDataReceived: function() {
                return this._drillThroughData && this._drillThroughData.Data != null
            },
            getRequestDataError: function() {
                return this._errorMessage
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file columnWidthCalculator.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            Class = DevExpress.require("/class");
        dashboard.ColumnWidthCalculator = Class.inherit({
            ctor: function ctor(){},
            calcWidth: function(widthOptionsInfo, maxVisibleWidth) {
                var optionsInfo = widthOptionsInfo;
                var columnsWidthInfo = optionsInfo.columnsWidthInfo;
                if (columnsWidthInfo.length !== 0 && maxVisibleWidth !== 0) {
                    if (optionsInfo.columnWidthMode === 'AutoFitToContents')
                        $.each(columnsWidthInfo, function(_, columnsWidthInfo) {
                            columnsWidthInfo.actualWidth = columnsWidthInfo.initialWidth
                        });
                    var actualWidthSum = this._getColumnActualWidthSum(columnsWidthInfo);
                    if (actualWidthSum !== maxVisibleWidth && (optionsInfo.columnWidthMode !== 'AutoFitToContents' || optionsInfo.columnWidthMode === 'AutoFitToContents' && actualWidthSum < maxVisibleWidth))
                        return this._calcWidthCore(optionsInfo, maxVisibleWidth)
                }
                return optionsInfo
            },
            _calcWidthCore: function(widthOptionsInfo, maxVisibleWidth) {
                var optionsInfo = widthOptionsInfo;
                var columnsWidthInfo = optionsInfo.columnsWidthInfo;
                $.each(columnsWidthInfo, function(_, columnInfo) {
                    if (optionsInfo.columnWidthMode !== 'Manual' || columnInfo.isFixedWidth)
                        columnInfo.actualWidth = columnInfo.initialWidth;
                    else
                        columnInfo.actualWidth = 0
                });
                var actualWidthSumOfColumnsWithFixedWidthState = this._getActualWidthSumOfColumnsWithFixedWidthState(optionsInfo);
                var initialWidthSumOfColumnsWithFixedWidthState = this._getInitialWidthSumOfColumnsWithFixedWidthState(optionsInfo);
                var initialFixedWidthState = actualWidthSumOfColumnsWithFixedWidthState !== 0 && actualWidthSumOfColumnsWithFixedWidthState < initialWidthSumOfColumnsWithFixedWidthState ? true : false;
                optionsInfo = this._calcWidthsForColumnsWithCurrentFixedWidthState(optionsInfo, initialFixedWidthState, maxVisibleWidth);
                if (this._getColumnActualWidthSum(optionsInfo.columnsWidthInfo) !== maxVisibleWidth)
                    return this._calcWidthsForColumnsWithCurrentFixedWidthState(optionsInfo, !initialFixedWidthState, maxVisibleWidth);
                return optionsInfo
            },
            _getInitialWidthSumOfColumnsWithFixedWidthState: function(optionsInfo) {
                var widthSum = 0;
                $.each(optionsInfo.columnsWidthInfo, function(_, columnInfo) {
                    if (optionsInfo.columnWidthMode === 'Manual' && columnInfo.isFixedWidth)
                        widthSum += columnInfo.initialWidth
                });
                return widthSum
            },
            _getActualWidthSumOfColumnsWithFixedWidthState: function(optionsInfo) {
                var widthSum = 0;
                $.each(optionsInfo.columnsWidthInfo, function(_, columnInfo) {
                    if (optionsInfo.columnWidthMode === 'Manual' && columnInfo.isFixedWidth)
                        widthSum += columnInfo.actualWidth
                });
                return widthSum
            },
            _calcWidthsForColumnsWithCurrentFixedWidthState: function(widthOptionsInfo, currentFixedWidthState, maxVisibleWidth) {
                var optionsInfo = widthOptionsInfo;
                var that = this;
                var widthSumOfColumnsWithFixedWidthStateNotEqualCurrent = that._getWidthSumOfColumnsWithFixedWidthStateNotEqualCurrent(optionsInfo, currentFixedWidthState);
                var widthSumOfColumnsWithCurrentFixedWidthState = that._getAllWidthSum(optionsInfo.columnsWidthInfo) - widthSumOfColumnsWithFixedWidthStateNotEqualCurrent;
                var newWidthSumOfColumnsWithCurrentFixedWidthState = Math.abs(maxVisibleWidth - widthSumOfColumnsWithFixedWidthStateNotEqualCurrent);
                $.each(optionsInfo.columnsWidthInfo, function(_, info) {
                    if (optionsInfo.columnWidthMode !== 'Manual' || info.isFixedWidth === currentFixedWidthState) {
                        var correctedWidth = Math.round(newWidthSumOfColumnsWithCurrentFixedWidthState * that._getNonEmptyWidth(info) / widthSumOfColumnsWithCurrentFixedWidthState);
                        info.actualWidth = correctedWidth >= info.minWidth ? correctedWidth : info.minWidth
                    }
                });
                return that._reverseOrderedLinearCorrection(optionsInfo, currentFixedWidthState, maxVisibleWidth)
            },
            _getAllWidthSum: function(columnsWidthInfo) {
                var widthSum = 0;
                for (var i = 0; i < columnsWidthInfo.length; i++)
                    widthSum += this._getNonEmptyWidth(columnsWidthInfo[i]);
                return widthSum
            },
            _reverseOrderedLinearCorrection: function(widthOptionsInfo, currentFixedWidthState, maxVisibleWidth) {
                var optionsInfo = widthOptionsInfo;
                var actualWidthSum = this._getColumnActualWidthSum(optionsInfo.columnsWidthInfo);
                for (var i = optionsInfo.columnsWidthInfo.length - 1; i >= 0; i--) {
                    var info = optionsInfo.columnsWidthInfo[i];
                    var remainder = actualWidthSum - maxVisibleWidth;
                    if (remainder === 0)
                        return optionsInfo;
                    if (optionsInfo.columnWidthMode !== 'Manual' || info.isFixedWidth === currentFixedWidthState) {
                        info.actualWidth -= remainder;
                        actualWidthSum -= remainder;
                        if (info.actualWidth < info.minWidth) {
                            actualWidthSum += Math.abs(info.actualWidth - info.minWidth);
                            info.actualWidth = info.minWidth
                        }
                    }
                }
                return optionsInfo
            },
            _getWidthSumOfColumnsWithFixedWidthStateNotEqualCurrent: function(widthOptionsInfo, currentFixedWidthState) {
                var sumNonSpecificWidth = 0;
                var that = this;
                if (widthOptionsInfo.columnWidthMode === 'Manual')
                    $.each(widthOptionsInfo.columnsWidthInfo, function(_, columnInfo) {
                        if (columnInfo.isFixedWidth !== currentFixedWidthState)
                            sumNonSpecificWidth += that._getNonEmptyWidth(columnInfo)
                    });
                return sumNonSpecificWidth
            },
            _getNonEmptyWidth: function(columnInfo) {
                return columnInfo.actualWidth !== 0 ? columnInfo.actualWidth : columnInfo.initialWidth
            },
            _getColumnActualWidthSum: function(columnsWidthInfo) {
                var widthSum = 0;
                for (var i = 0; i < columnsWidthInfo.length; i++)
                    widthSum += columnsWidthInfo[i].actualWidth;
                return widthSum
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file appearanceSettingsProvider.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            Class = DevExpress.require("/class");
        dashboard.appearanceSettingsProvider = Class.inherit({
            AppearanceType: {
                WhiteColor: 'rgb(0xFF, 0xFF, 0xFF)',
                GrayedTextColor: 'rgb(0xD3, 0xD3, 0xD3)',
                LightGradientRedColor: 'rgb(255, 166, 173)',
                LightGradientYellowColor: 'rgb(255, 226, 81)',
                LightGradientGreenColor: 'rgb(139, 210, 78)',
                LightGradientBlueColor: 'rgb(149, 204, 255)',
                LightGradientPurpleColor: 'rgb(223, 166, 232)',
                LightGradientCyanColor: 'rgb(113, 223, 221)',
                LightGradientOrangeColor: 'rgb(255, 182, 90)',
                LightGradientTransparentColor: '#ffffff',
                DarkGradientRedColor: '#AC203D',
                DarkGradientYellowColor: '#FF8A01',
                DarkGradientGreenColor: '#538A31',
                DarkGradientBlueColor: '#4371B0',
                DarkGradientPurpleColor: '#7E53A2',
                DarkGradientCyanColor: '#149BA3',
                DarkGradientOrangeColor: '#D83D00',
                DarkGradientTransparentColor: '#303030',
                LightPaleRedColor: 'rgb(255, 221, 224)',
                LightPaleYellowColor: 'rgb(255, 245, 174)',
                LightPaleGreenColor: 'rgb(208, 239, 172)',
                LightPaleBlueColor: 'rgb(213, 237, 255)',
                LightPalePurpleColor: 'rgb(244, 221, 247)',
                LightPaleCyanColor: 'rgb(194, 244, 243)',
                LightPaleOrangeColor: 'rgb(255, 228, 180)',
                LightPaleGrayColor: 'rgb(234, 234, 234)',
                DarkPaleRedColor: '#5B2D3D',
                DarkPaleYellowColor: '#51492D',
                DarkPaleGreenColor: '#3B4D2D',
                DarkPaleBlueColor: '#2D3F5A',
                DarkPalePurpleColor: '#512D55',
                DarkPaleCyanColor: '#2D4B4B',
                DarkPaleOrangeColor: '#593E2D',
                DarkPaleGrayColor: '#444444',
                LightRedColor: 'rgb(226, 60, 76)',
                LightYellowColor: 'rgb(255, 166, 38)',
                LightGreenColor: 'rgb(101, 172, 80)',
                LightBlueColor: 'rgb(89, 143, 216)',
                LightPurpleColor: 'rgb(148, 105, 184)',
                LightCyanColor: 'rgb(39, 192, 187)',
                LightOrangeColor: 'rgb(255, 92, 12)',
                LightGrayColor: 'rgb(111, 111, 111)',
                DarkRedColor: '#E23C4C',
                DarkYellowColor: '#FFA626',
                DarkGreenColor: '#65AC50',
                DarkBlueColor: '#598FD8',
                DarkPurpleColor: '#9469B8',
                DarkCyanColor: '#27C0BB',
                DarkOrangeColor: '#FF5C0C',
                DarkGrayColor: '#6F6F6F'
            },
            ctor: function ctor() {
                this.lightThemes = ['generic.light'];
                this.darkThemes = ['generic.dark', 'generic.contrast']
            },
            toCssClassBody: function(appearanceType, theme) {
                var isDark = this._isDark(theme),
                    color;
                switch (appearanceType) {
                    case'FontBold':
                        return '{ font-weight: bold; }';
                    case'FontItalic':
                        return '{ font-style: italic; }';
                    case'FontUnderline':
                        return '{ text-decoration: underline; }';
                    case'FontGrayed':
                        return '{ color: ' + this.AppearanceType.GrayedTextColor + '; }';
                    case'FontRed':
                        return '{ color: ' + (isDark ? this.AppearanceType.DarkRedColor : this.AppearanceType.LightRedColor) + '; }';
                    case'FontYellow':
                        return '{ color: ' + (isDark ? this.AppearanceType.DarkYellowColor : this.AppearanceType.LightYellowColor) + '; }';
                    case'FontGreen':
                        return '{ color: ' + (isDark ? this.AppearanceType.DarkGreenColor : this.AppearanceType.LightGreenColor) + '; }';
                    case'FontBlue':
                        return '{ color: ' + (isDark ? this.AppearanceType.DarkBlueColor : this.AppearanceType.LightBlueColor) + '; }';
                    default:
                        color = this.backAndGradientColorGroupsToBackColor(appearanceType, theme);
                        if (color !== undefined)
                            return '{ background-color: ' + color + '; }';
                        color = this._backColorsWithFontGroupToBackColor(appearanceType, isDark);
                        if (color !== undefined)
                            return '{ background-color: ' + color + '; color: ' + this.AppearanceType.WhiteColor + '; }';
                        return '{ }'
                }
            },
            backAndGradientColorGroupsToBackColor: function(appearanceType, theme) {
                var isDark = this._isDark(theme);
                switch (appearanceType) {
                    case'PaleRed':
                        return isDark ? this.AppearanceType.DarkPaleRedColor : this.AppearanceType.LightPaleRedColor;
                    case'PaleYellow':
                        return isDark ? this.AppearanceType.DarkPaleYellowColor : this.AppearanceType.LightPaleYellowColor;
                    case'PaleGreen':
                        return isDark ? this.AppearanceType.DarkPaleGreenColor : this.AppearanceType.LightPaleGreenColor;
                    case'PaleBlue':
                        return isDark ? this.AppearanceType.DarkPaleBlueColor : this.AppearanceType.LightPaleBlueColor;
                    case'PalePurple':
                        return isDark ? this.AppearanceType.DarkPalePurpleColor : this.AppearanceType.LightPalePurpleColor;
                    case'PaleCyan':
                        return isDark ? this.AppearanceType.DarkPaleCyanColor : this.AppearanceType.LightPaleCyanColor;
                    case'PaleOrange':
                        return isDark ? this.AppearanceType.DarkPaleOrangeColor : this.AppearanceType.LightPaleOrangeColor;
                    case'PaleGray':
                        return isDark ? this.AppearanceType.DarkPaleGrayColor : this.AppearanceType.LightPaleGrayColor;
                    case'GradientRed':
                        return isDark ? this.AppearanceType.DarkGradientRedColor : this.AppearanceType.LightGradientRedColor;
                    case'GradientYellow':
                        return isDark ? this.AppearanceType.DarkGradientYellowColor : this.AppearanceType.LightGradientYellowColor;
                    case'GradientGreen':
                        return isDark ? this.AppearanceType.DarkGradientGreenColor : this.AppearanceType.LightGradientGreenColor;
                    case'GradientBlue':
                        return isDark ? this.AppearanceType.DarkGradientBlueColor : this.AppearanceType.LightGradientBlueColor;
                    case'GradientPurple':
                        return isDark ? this.AppearanceType.DarkGradientPurpleColor : this.AppearanceType.LightGradientPurpleColor;
                    case'GradientCyan':
                        return isDark ? this.AppearanceType.DarkGradientCyanColor : this.AppearanceType.LightGradientCyanColor;
                    case'GradientOrange':
                        return isDark ? this.AppearanceType.DarkGradientOrangeColor : this.AppearanceType.LightGradientOrangeColor;
                    case'GradientTransparent':
                        return isDark ? this.AppearanceType.DarkGradientTransparentColor : this.AppearanceType.LightGradientTransparentColor;
                    default:
                }
            },
            _isDark: function(theme) {
                if ($.inArray(theme, this.darkThemes) !== -1)
                    return true;
                if ($.inArray(theme, this.lightThemes) !== -1)
                    return false;
                return false
            },
            _backColorsWithFontGroupToBackColor: function(appearanceType, isDark) {
                switch (appearanceType) {
                    case'Red':
                        return isDark ? this.AppearanceType.DarkRedColor : this.AppearanceType.LightRedColor;
                    case'Yellow':
                        return isDark ? this.AppearanceType.DarkYellowColor : this.AppearanceType.LightYellowColor;
                    case'Green':
                        return isDark ? this.AppearanceType.DarkGreenColor : this.AppearanceType.LightGreenColor;
                    case'Blue':
                        return isDark ? this.AppearanceType.DarkBlueColor : this.AppearanceType.LightBlueColor;
                    case'Purple':
                        return isDark ? this.AppearanceType.DarkPurpleColor : this.AppearanceType.LightPurpleColor;
                    case'Cyan':
                        return isDark ? this.AppearanceType.DarkCyanColor : this.AppearanceType.LightCyanColor;
                    case'Orange':
                        return isDark ? this.AppearanceType.DarkOrangeColor : this.AppearanceType.LightOrangeColor;
                    case'Gray':
                        return isDark ? this.AppearanceType.DarkGrayColor : this.AppearanceType.LightGrayColor;
                    default:
                }
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file styleSettingsProvider.js */
    (function($, DX, undefined) {
        var utils = DX.utils,
            dashboard = DX.dashboard,
            Class = DevExpress.require("/class"),
            appearanceSettingsProvider = dashboard.appearanceSettingsProvider,
            Color = DX.require("/color"),
            browser = DX.require("/utils/utils.browser"),
            ICON_TYPE_NONE = 'None',
            APPEARANCE_TYPE_NONE = 'None',
            APPEARANCE_TYPE_CUSTOM = 'Custom',
            VERTICAL_AXIS_PADDING = 3,
            HIDDEN_TEXT_PREFIX = 'hiddenText',
            TOOLTIP_PREFIX = 'tooltip';
        dashboard.styleSettingsProvider = Class.inherit({
            FontStyle: {
                Bold: 1,
                Italic: 2,
                Underline: 4,
                Strikeout: 8
            },
            DataAttributes: {
                Bar: 'bar',
                Axis: 'axis',
                NormalizedValue: 'normalizedValue',
                ZeroPosition: 'zeroPosition',
                AllowNegativeAxis: 'allowNegativeAxis',
                DrawAxis: 'drawAxis'
            },
            cssClassNames: {
                iconConditionalFormatting: 'dx-icon-dashboard-cf',
                bar: 'dx-dashboard-bar',
                barAxis: 'dx-dashboard-bar-axis',
                customStyle: 'dx-dashboard-cf-style',
                customGradientStyle: 'dx-dashboard-cf-gradient-style',
                dashboardContainer: 'dx-dashboard-container',
                absolutePosition: 'dx-dashboard-absolute-position',
                relativePosition: 'dx-dashboard-relative-position',
                measureValue: 'dx-dashboard-measure',
                leftIcon: 'dx-dashboard-left-icon',
                rightIcon: 'dx-dashboard-right-icon',
                leftIconSpace: 'dx-dashboard-left-icon-space',
                rightIconSpace: 'dx-dashboard-right-icon-space'
            },
            ctor: function ctor() {
                this.cfModel = undefined;
                this.cssCustomClasses = {};
                this.theme = DevExpress.ui.themes.current();
                this.$container = undefined;
                this.id = dashboard.styleSettingsProvider.inctanceCounter++;
                this.appearanceSettingsProvider = new appearanceSettingsProvider
            },
            initialize: function($container, cfModel) {
                if (cfModel) {
                    this.cfModel = cfModel;
                    this._registerCssClasses()
                }
                if ($container)
                    this.$container = $container
            },
            update: function($container) {
                if ($container)
                    this.$container = $container
            },
            updateBarWidth: function($controlRootElement, barPrefix) {
                var that = this,
                    barDivs = $controlRootElement.find('.' + that.DataAttributes.Bar + '_' + barPrefix),
                    axisDivs,
                    containerBoundsArray = this._fillContainerBounds(barDivs);
                $.each(barDivs, function(i, barDiv) {
                    var $bar = $(barDiv),
                        normalizedValue = $bar.data(that.DataAttributes.NormalizedValue),
                        zeroPosition = $bar.data(that.DataAttributes.ZeroPosition),
                        allowNegativeAxis = $bar.data(that.DataAttributes.AllowNegativeAxis),
                        drawAxis = $bar.data(that.DataAttributes.DrawAxis),
                        $container = $bar.parent(),
                        containerBounds = containerBoundsArray[i],
                        axisDivs = $container.find('.' + that.DataAttributes.Axis + '_' + barPrefix + ':first');
                    that._setBarBounds($bar, containerBounds, zeroPosition, normalizedValue, allowNegativeAxis, drawAxis);
                    if (axisDivs.length !== 0)
                        that._setAxisBounds($(axisDivs[0]), containerBounds, zeroPosition)
                })
            },
            applyStyleSettings: function($container, styleSettingsInfo, ignoreImageSettings, barPrefix) {
                var that = this,
                    styleSettingsModel,
                    sortedStyleIndexes,
                    condition,
                    ruleIndex,
                    barInfo,
                    iconType,
                    cssClassname;
                sortedStyleIndexes = $.grep(styleSettingsInfo.styleIndexes, function(v, k) {
                    return $.inArray(v, styleSettingsInfo.styleIndexes) === k
                });
                sortedStyleIndexes.sort(function(a, b) {
                    return a - b
                });
                if (sortedStyleIndexes && sortedStyleIndexes.length > 0)
                    $.each(sortedStyleIndexes, function(_, styleIndex) {
                        styleSettingsModel = that.cfModel.FormatConditionStyleSettings[styleIndex];
                        ruleIndex = styleSettingsInfo.styleAndRuleMappingTable[styleIndex];
                        if (styleSettingsModel.IsBarStyle) {
                            condition = that.cfModel.RuleModels[ruleIndex].ConditionModel;
                            barInfo = that._getBarInfo(styleSettingsModel, styleIndex, condition)
                        }
                        else if (styleSettingsModel.RangeIndex || styleSettingsModel.AppearanceType !== APPEARANCE_TYPE_NONE)
                            $container.addClass(that.cssCustomClasses[styleIndex]);
                        else if (styleSettingsModel.IconType !== ICON_TYPE_NONE)
                            iconType = styleSettingsModel.IconType
                    });
                if (barInfo) {
                    barInfo.normalizedValue = styleSettingsInfo.normalizedValue;
                    barInfo.zeroPosition = styleSettingsInfo.zeroPosition;
                    this._renderBar($container, barInfo, barPrefix)
                }
                else if (!ignoreImageSettings && iconType)
                    this._applyIconSettings($container, iconType)
            },
            _fillContainerBounds: function(barDivs) {
                var containerBoundsArray = [];
                $.each(barDivs, function(_, barDiv) {
                    var $container = $(barDiv).parent(),
                        containerElement = $container.get(0);
                    containerBoundsArray.push(containerElement.getBoundingClientRect())
                });
                return containerBoundsArray
            },
            _createCssClassName: function(prefix, styleIndex) {
                return prefix + '-' + this.id + '-' + styleIndex
            },
            _applyIconSettings: function($container, iconType) {
                var imageAlignmentIsRight = this._imageAlignmentIsRight($container),
                    cssIconClass = imageAlignmentIsRight ? this.cssClassNames.rightIcon : this.cssClassNames.leftIcon,
                    cssSpanClass = imageAlignmentIsRight ? this.cssClassNames.leftIconSpace : this.cssClassNames.rightIconSpace;
                $container.css('white-space', 'nowrap');
                $container[imageAlignmentIsRight ? 'append' : 'prepend']($('<span>').addClass(cssSpanClass));
                $container.prepend($('<span>').addClass(cssIconClass).addClass(this._getConditionalFormattingImageCssClass(iconType)))
            },
            _imageAlignmentIsRight: function($container) {
                var textAlignment = $container.css('text-align');
                return textAlignment === 'left' || textAlignment === 'start' || textAlignment === 'center' || textAlignment === 'justify'
            },
            _getBarInfo: function(styleSettingsModel, styleIndex, condition) {
                return {
                        showBarOnly: condition.BarOptions.ShowBarOnly,
                        allowNegativeAxis: condition.BarOptions.AllowNegativeAxis,
                        drawAxis: condition.BarOptions.DrawAxis,
                        cssClass: this.cssCustomClasses[styleIndex]
                    }
            },
            _renderBar: function($container, barInfo, barPrefix) {
                var displayText = $container.text(),
                    $textDiv,
                    $tooltipDiv,
                    index,
                    tooltipId,
                    containerId;
                $container.addClass(this.cssClassNames.relativePosition);
                $container.text('');
                this._renderBarDiv($container, barInfo, barPrefix);
                if (barInfo.drawAxis)
                    this._renderAxis($container, barPrefix, barInfo.zeroPosition);
                if (!barInfo.showBarOnly)
                    this._renderBarTextValue($container, displayText);
                else {
                    $textDiv = $('<div/>').appendTo($container);
                    $textDiv.html("&nbsp;");
                    index = dashboard.styleSettingsProvider.hiddenTextCounter++;
                    tooltipId = TOOLTIP_PREFIX + index;
                    containerId = HIDDEN_TEXT_PREFIX + index;
                    $container.attr("id", containerId);
                    $tooltipDiv = $('<div/>').appendTo($container).text(displayText).attr("id", tooltipId).dxTooltip({target: '#' + containerId}).dxTooltip("instance");
                    $container.unbind().hover(function() {
                        $tooltipDiv.toggle()
                    })
                }
            },
            _renderBarDiv: function($container, barInfo, barPrefix) {
                var $barDiv = $('<div/>').appendTo($container).addClass(this.cssClassNames.bar).addClass(barInfo.cssClass);
                $barDiv.html("&nbsp;");
                $barDiv.addClass(this.DataAttributes.Bar + '_' + barPrefix);
                $barDiv.data(this.DataAttributes.NormalizedValue, barInfo.normalizedValue);
                $barDiv.data(this.DataAttributes.ZeroPosition, barInfo.zeroPosition);
                $barDiv.data(this.DataAttributes.AllowNegativeAxis, barInfo.allowNegativeAxis);
                $barDiv.data(this.DataAttributes.DrawAxis, barInfo.drawAxis)
            },
            _renderAxis: function($container, barPrefix, zeroPosition) {
                var $axisDiv = $('<div/>').appendTo($container);
                $axisDiv.addClass(this.cssClassNames.absolutePosition).addClass(this.DataAttributes.Axis + '_' + barPrefix + ' ' + this.cssClassNames.barAxis);
                $axisDiv.data(this.DataAttributes.ZeroPosition, zeroPosition)
            },
            _renderBarTextValue: function($container, displayText) {
                var $textDiv = $('<div/>').appendTo($container).addClass(this.cssClassNames.measureValue);
                $textDiv.text(displayText);
                return $textDiv
            },
            _setBarBounds: function($barDiv, containerBounds, zeroPosition, normalizedValue, allowNegativeAxis, drawAxis) {
                var containerWidth = containerBounds.width,
                    containerHeight = containerBounds.height,
                    barWidth = Math.round(Math.abs(normalizedValue) * containerWidth),
                    axisPosition = Math.round(zeroPosition * containerWidth),
                    barPositon = allowNegativeAxis && normalizedValue < 0 ? axisPosition - barWidth : axisPosition;
                if (allowNegativeAxis && drawAxis && normalizedValue < 0 && zeroPosition > 0)
                    barWidth += 1;
                $barDiv.css({width: barWidth + 'px'});
                if (browser.mozilla)
                    $barDiv.css({
                        right: containerWidth - barPositon - barWidth + 'px',
                        height: containerBounds.height + 'px'
                    });
                else
                    $barDiv.css({left: barPositon + 'px'})
            },
            _setAxisBounds: function($axisDiv, containerBounds, zeroPosition) {
                var containerWidth = containerBounds.width,
                    containerHeight = containerBounds.height,
                    axisPosition = Math.round(zeroPosition * containerWidth),
                    height = Math.max(0, containerHeight - VERTICAL_AXIS_PADDING * 2);
                if (browser.mozilla)
                    $axisDiv.css({right: containerWidth - axisPosition + 'px'});
                else
                    $axisDiv.css({left: axisPosition + 'px'})
            },
            _getCustomBackColor: function(color) {
                if (color.toHex)
                    return color.toHex();
                return dashboard.utils.toColor(color)
            },
            _getRangeBackColorStyleSettings: function(styleSettings, ruleIndex) {
                var rangeIndex = styleSettings.RangeIndex,
                    condition = this.cfModel.RuleModels[ruleIndex].ConditionModel,
                    leftIndex = -1,
                    rightIndex = -1,
                    leftModel,
                    rightModel,
                    resultModel,
                    leftColor,
                    rightColor,
                    color;
                $.each(condition.FixedColors, function(index, colorModel) {
                    if (index < rangeIndex && (leftIndex === -1 || index > leftIndex))
                        leftIndex = index;
                    if (index > rangeIndex && (rightIndex === -1 || index < rightIndex))
                        rightIndex = index
                });
                leftModel = condition.FixedColors[leftIndex];
                rightModel = condition.FixedColors[rightIndex];
                leftColor = this._getBackColor(leftModel);
                rightColor = this._getBackColor(rightModel);
                return {
                        AppearanceType: APPEARANCE_TYPE_CUSTOM,
                        Color: leftColor.blend(rightColor, (rangeIndex - leftIndex) / (rightIndex - leftIndex)),
                        ForeColor: leftModel.ForeColor,
                        FontFamily: leftModel.FontFamily,
                        FontSize: leftModel.FontSize,
                        FontStyle: leftModel.FontStyle,
                        IsBarStyle: leftModel.IsBarStyle
                    }
            },
            _getBackColor: function(colorModel) {
                var color;
                if (colorModel.AppearanceType === APPEARANCE_TYPE_CUSTOM)
                    color = dashboard.utils.toColor(colorModel.Color);
                else if (colorModel.AppearanceType !== APPEARANCE_TYPE_NONE)
                    color = this.appearanceSettingsProvider.backAndGradientColorGroupsToBackColor(colorModel.AppearanceType, this.theme);
                return new Color(color)
            },
            _getConditionalFormattingImageCssClass: function(iconType) {
                return this.cssClassNames.iconConditionalFormatting + '-' + iconType.toLowerCase()
            },
            _registerCssClasses: function() {
                var that = this,
                    cssClassName,
                    selector;
                if (that.cfModel != undefined)
                    $.each(that.cfModel.FormatConditionStyleSettings, function(i, styleSettingsModel) {
                        cssClassName = that._createCssClassName(that.cssClassNames.customStyle, i);
                        selector = that._getCssTdSelector(cssClassName);
                        that.cssCustomClasses[i] = cssClassName;
                        if (styleSettingsModel.RangeIndex) {
                            styleSettingsModel = that._getRangeBackColorStyleSettings(styleSettingsModel, styleSettingsModel.RuleIndex);
                            that._createCssClassFromCustomAppearanceType(styleSettingsModel, selector)
                        }
                        else if (styleSettingsModel.AppearanceType === APPEARANCE_TYPE_CUSTOM)
                            that._createCssClassFromCustomAppearanceType(styleSettingsModel, selector);
                        else if (styleSettingsModel.AppearanceType !== APPEARANCE_TYPE_NONE)
                            that._createCssClassFromPredefinedAppearanceType(selector, styleSettingsModel.AppearanceType)
                    })
            },
            _getCssTdSelector: function(cssClassname) {
                return '.' + this.cssClassNames.dashboardContainer + ' td .' + cssClassname
            },
            _createCssClassFromCustomAppearanceType: function(styleSettingsModel, cssSelector) {
                var style = document.createElement('style'),
                    isUnderline = (styleSettingsModel.FontStyle & this.FontStyle.Underline) !== 0,
                    isStrikeout = (styleSettingsModel.FontStyle & this.FontStyle.Strikeout) !== 0,
                    innerHTML = cssSelector + '{ ';
                if (styleSettingsModel.ForeColor)
                    innerHTML += 'color:' + dashboard.utils.toColor(styleSettingsModel.ForeColor) + ';';
                if (styleSettingsModel.Color)
                    innerHTML += 'background-color:' + this._getCustomBackColor(styleSettingsModel.Color) + ';';
                if (styleSettingsModel.FontFamily)
                    innerHTML += 'font-family:' + styleSettingsModel.FontFamily + ';';
                if (styleSettingsModel.FontSize && styleSettingsModel.FontSize > 0)
                    innerHTML += 'font-size:' + styleSettingsModel.FontSize + ';';
                if ((styleSettingsModel.FontStyle & this.FontStyle.Bold) !== 0)
                    innerHTML += 'font-weight:bold;';
                if ((styleSettingsModel.FontStyle & this.FontStyle.Italic) !== 0)
                    innerHTML += 'font-style: italic;';
                if (isUnderline && isStrikeout)
                    innerHTML += 'text-decoration: underline line-through;';
                else if (isUnderline)
                    innerHTML += 'text-decoration: underline;';
                else if (isStrikeout)
                    innerHTML += 'text-decoration: line-through;';
                this._setInnerHTMLToStyle(style, innerHTML.slice(0, -1) + '}');
                style.type = 'text/css';
                document.getElementsByTagName('head')[0].appendChild(style)
            },
            _createCssClassFromPredefinedAppearanceType: function(selector, appearanceType) {
                var innerHTML = selector + this.appearanceSettingsProvider.toCssClassBody(appearanceType, this.theme),
                    style = document.createElement('style');
                this._setInnerHTMLToStyle(style, innerHTML);
                style.type = 'text/css';
                document.getElementsByTagName('head')[0].appendChild(style)
            },
            _setInnerHTMLToStyle: function(style, innerHTML) {
                try {
                    style.innerHTML = innerHTML
                }
                catch(e) {
                    style.cssText = innerHTML
                }
            }
        });
        dashboard.styleSettingsProvider.inctanceCounter = 0;
        dashboard.styleSettingsProvider.hiddenTextCounter = 0
    })(jQuery, DevExpress);
    /*! Module dashboard, file renderHelper.js */
    (function($, DX, undefined) {
        var utils = DX.utils,
            stringUtils = DX.require("/utils/utils.string"),
            logger = DX.require("/utils/utils.console").logger;
        utils.renderHelper = {
            html: function($element, content, encodeHtml) {
                if (content)
                    if (encodeHtml)
                        $element.text(content);
                    else
                        $element.html(content)
            },
            rectangle: function(color, width, height) {
                var w = width != 0 ? width || 10 : 0,
                    h = height || 10;
                return stringUtils.format("<div style='display:inline-block;width:{0}px;height:{1}px;background-color:{2};padding:0px;margin:0px;'></div>", w, h, color)
            },
            getActualBorder: function($element) {
                return {
                        width: $element.outerWidth() - $element.width(),
                        height: $element.outerHeight() - $element.height()
                    }
            },
            getActualSize: function($element) {
                if (!$element || $element.length === 0)
                    return {
                            width: 0,
                            height: 0
                        };
                else {
                    var border = this.getActualBorder($element),
                        isBorderBox = $element.css('box-sizing') == 'border-box';
                    return {
                            width: $element.width() - (isBorderBox ? 0 : border.width),
                            height: $element.height() - (isBorderBox ? 0 : border.height)
                        }
                }
            },
            getDefaultPalette: function() {
                return ['#5F8195', '#B55951', '#AEAF69', '#915E64', '#758E6D', '#85688C', '#91B9C7', '#E49B86']
            },
            getScrollable: function($element) {
                return $element.data('dxScrollable')
            },
            updateScrollable: function($element) {
                var scrollable = this.getScrollable($element);
                if (scrollable)
                    scrollable.update()
            },
            wrapScrollable: function($container, useNativeScrolling, parentOverflow, direction) {
                var scrollableContent = undefined,
                    scrollableOptions = {
                        bounceEnabled: false,
                        showScrollbar: 'onHover',
                        direction: direction
                    };
                if ($container && $container.dxScrollable) {
                    if (useNativeScrolling !== 'auto') {
                        scrollableOptions.useNative = !!useNativeScrolling;
                        scrollableOptions.useSimulatedScrollbar = !useNativeScrolling
                    }
                    $container.dxScrollable(scrollableOptions);
                    scrollableContent = this.getScrollable($container).content()
                }
                else {
                    scrollableContent = $container;
                    scrollableContent.css('overflow', parentOverflow)
                }
                return scrollableContent
            },
            getControlBox: function(controlName, options) {
                var $container = $('<div>');
                $container[controlName](options || {});
                return this.getElementBox($container)
            },
            getElementBox: function($element) {
                var $fakeContainer = $('<div>', {css: {
                            position: 'absolute',
                            top: 0,
                            left: 0,
                            visibility: 'hidden',
                            overflow: 'hidden'
                        }}).appendTo($('body'));
                $fakeContainer.append($element);
                try {
                    return {
                            width: $fakeContainer.outerWidth(),
                            height: $fakeContainer.outerHeight()
                        }
                }
                finally {
                    $fakeContainer.remove()
                }
            },
            widgetIncidentOccurred: function(e) {
                logger.warn(e.target.widget + " incident occurred. Code: " + e.target.id)
            }
        }
    })(jQuery, DevExpress);
    /*! Module dashboard, file consts.js */
    (function(DX) {
        DX.viz.indicators = {consts: {deltaIndicator: {indicatorType: {
                        none: 'none',
                        up: 'up',
                        down: 'down',
                        warning: 'warning'
                    }}}}
    })(DevExpress);
    /*! Module dashboard, file deltaIndicator.js */
    (function($, DX) {
        var Class = DevExpress.require("/class"),
            viz = DX.viz,
            indicators = viz.indicators,
            consts = indicators.consts.deltaIndicator,
            Rect = viz.Rectangle;
        var DeltaIndicator = Class.inherit({
                ctor: function(options) {
                    options = options || {};
                    this._container = $(options.container);
                    this._renderer = options.renderer;
                    this._shape = null;
                    this._init();
                    if (this._container.length)
                        this.draw(options)
                },
                _init: function() {
                    var container = this._container,
                        width = 0,
                        height = 0;
                    if (!container.length)
                        return;
                    width = container.width(),
                    height = container.height();
                    if (!(width > 0 && height > 0))
                        return;
                    this._renderer = new viz.renderers.Renderer({container: container[0]})
                },
                draw: function(options) {
                    var params = this._prepareDrawParams(options);
                    if (params.readyToDraw)
                        this._render(params);
                    return this._shape
                },
                _prepareDrawParams: function(options) {
                    var container = this._container,
                        params = {readyToDraw: false},
                        rectOptions = {
                            left: 0,
                            top: 0,
                            right: 0,
                            bottom: 0
                        };
                    if (container.length) {
                        rectOptions.right = container.width() || 0;
                        rectOptions.bottom = container.height() || 0;
                        params.rectangle = new Rect(rectOptions)
                    }
                    else if (options.rect instanceof Rect)
                        params.rectangle = options.rect.clone();
                    else
                        params.rectangle = new Rect(rectOptions);
                    params.type = options.type || consts.indicatorType.none;
                    params.hasPositiveMeaning = !!options.hasPositiveMeaning;
                    params.readyToDraw = !!(this._renderer && params.rectangle.width() > 0 && params.rectangle.height() > 0);
                    params.drawToContainer = !!this._container.length;
                    return params
                },
                _render: function(params) {
                    var rect = params.rectangle,
                        shape;
                    this._shape = null;
                    if (params.drawToContainer) {
                        this._renderer.resize(rect.width, rect.height);
                        shape = this._drawShape(params);
                        if (shape)
                            shape.append(this._renderer.root)
                    }
                    else
                        shape = this._drawShape(params);
                    this._shape = shape
                },
                _drawShape: function(params) {
                    var shape = null,
                        rect = params.rectangle,
                        cx = ~~rect.horizontalMiddle(),
                        cy = ~~rect.verticalMiddle(),
                        r = ~~(Math.min(rect.width(), rect.height()) / 2),
                        coords = [],
                        colorClassName = indicators.DeltaIndicator.getIndicatorColorType(params.type, params.hasPositiveMeaning);
                    switch (params.type) {
                        case consts.indicatorType.none:
                            break;
                        case consts.indicatorType.up:
                            coords.push(rect.left);
                            coords.push(rect.bottom);
                            coords.push(cx);
                            coords.push(rect.top);
                            coords.push(rect.right);
                            coords.push(rect.bottom);
                            shape = this._renderer.path(coords, "area").attr({'class': colorClassName});
                            shape._useCSSTheme = true;
                            break;
                        case consts.indicatorType.down:
                            coords.push(rect.left);
                            coords.push(rect.top);
                            coords.push(cx);
                            coords.push(rect.bottom);
                            coords.push(rect.right);
                            coords.push(rect.top);
                            shape = this._renderer.path(coords, "area").attr({'class': colorClassName});
                            shape._useCSSTheme = true;
                            break;
                        case consts.indicatorType.warning:
                            shape = this._renderer.circle(cx, cy, r).attr({'class': colorClassName});
                            shape._useCSSTheme = true;
                            break
                    }
                    return shape
                }
            });
        indicators.DeltaIndicator = DeltaIndicator;
        indicators.DeltaIndicator.getIndicatorColorType = function(type, hasPositiveMeaning, useDefaultColor) {
            var color;
            if (useDefaultColor)
                color = 'dx-carditem-default-color';
            else
                switch (type) {
                    case consts.indicatorType.up:
                    case consts.indicatorType.down:
                        color = hasPositiveMeaning ? 'dx-carditem-positive-color' : 'dx-carditem-negative-color';
                        break;
                    case consts.indicatorType.warning:
                        color = 'dx-carditem-warning-color';
                        break;
                    default:
                        color = 'dx-carditem-none-color';
                        break
                }
            return color
        }
    })(jQuery, DevExpress);
    /*! Module dashboard, file simpleIndicator.js */
    (function($, DX, undefined) {
        var IS_IE8 = !DX.viz.renderers.isSvg(),
            indicators = DX.viz.indicators,
            Indicator = indicators.DeltaIndicator,
            _staticPartMarkup = '<svg viewBox="0 0 400 300" width="100%" height="100%" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" style="-webkit-tap-highlight-color: rgba(0, 0, 0, 0); display: block;"><path class="',
            svgIndicators = {
                none: '<svg width="24" height="18" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" style="-webkit-tap-highlight-color: rgba(0, 0, 0, 0); display: block;"></svg>',
                up: _staticPartMarkup + 'dx-carditem-positive-color" d="M 0 300 L 200 0 L 400 300 Z"></path></svg>',
                up_negative: _staticPartMarkup + 'dx-carditem-negative-color" d="M 0 300 L 200 0 L 400 300 Z"></path></svg>',
                down: _staticPartMarkup + 'dx-carditem-positive-color" d="M 0 0 L 200 300 L 400 0 Z"></path></svg>',
                down_negative: _staticPartMarkup + 'dx-carditem-negative-color" d="M 0 0 L 200 300 L 400 0 Z"></path></svg>',
                warning: '<svg viewBox="0 0 18 18" width="100%" height="100%" xmlns="http://www.w3.org/2000/svg" xmlns:xlink="http://www.w3.org/1999/xlink" version="1.1" style="-webkit-tap-highlight-color: rgba(0, 0, 0, 0); display: block;"><circle cx="9" cy="9" r="8.7" class="dx-carditem-warning-color"></circle></svg>'
            };
        indicators.SimpleIndicator = function() {
            var renderedIndicators = svgIndicators;
            this.renderIndicators = function(height) {
                if (IS_IE8)
                    renderedIndicators = generateVmlIndicators(height);
                this.__renderedIndicators = renderedIndicators
            };
            this.getRenderedIndicator = function(type, hasPositiveMeaning) {
                return renderedIndicators[type + ((type === 'up' || type === 'down') && !hasPositiveMeaning ? '_negative' : '')]
            }
        };
        function generateVmlIndicators(height) {
            var styleToContainer = {css: {
                        visibility: 'hidden',
                        position: 'fixed',
                        top: 0,
                        left: 0,
                        width: Math.ceil(1.25 * height),
                        height: height
                    }},
                indicatorTypes = indicators.consts.deltaIndicator.indicatorType,
                renderedIndicators = {},
                curType,
                container = $('<div>', styleToContainer).appendTo(document.body);
            for (curType in indicatorTypes) {
                generateIteration(curType, true);
                if (curType === indicatorTypes.up || curType === indicatorTypes.down)
                    generateIteration(curType, false)
            }
            function generateIteration(type, hasPositiveMeaning) {
                container.html('');
                new Indicator({
                    container: container,
                    type: type,
                    hasPositiveMeaning: hasPositiveMeaning
                });
                renderedIndicators[type + (!hasPositiveMeaning ? '_negative' : '')] = container.html()
            }
            container.remove();
            return renderedIndicators
        }
    })(jQuery, DevExpress);
    /*! Module dashboard, file indicator.js */
    (function(DX, $, undefined) {
        var Class = DevExpress.require("/class"),
            viz = DX.viz,
            commonUtils = DX.require("/utils/utils.common"),
            _formatHelper = DX.require("/utils/utils.formatHelper"),
            _ExternalIndicator = viz.indicators.DeltaIndicator;
        var _round = Math.round,
            _isString = commonUtils.isString,
            _isFunction = commonUtils.isFunction,
            _getIndicatorColorType = _ExternalIndicator.getIndicatorColorType,
            _extend = $.extend;
        var DELTA_INDENT = 10,
            DELTA_SIZE_COEFF = 0.5,
            DELTA_ASPECT_RATIO = 3 / 4;
        viz.gauges.__internals.DeltaIndicator = Class.inherit({
            ctor: function(parameters) {
                this._renderer = parameters.renderer;
                this._root = parameters.renderer.g().attr({'class': 'dxg-delta-indicator'}).linkOn(parameters.container, {
                    name: "delta-indicator",
                    after: "peripheral"
                })
            },
            dispose: function() {
                this._root.linkOff();
                this._renderer = this._root = null;
                return this
            },
            clean: function() {
                this._root.linkRemove().clear();
                this._layout = null;
                return this
            },
            draw: function(options) {
                options = options || {};
                var that = this,
                    textValue = formatText(options.text);
                if (!_isString(textValue) || !textValue.length)
                    return that;
                that._root.linkAppend();
                var text = that._renderer.text(textValue, 0, 0).attr({
                        align: 'center',
                        'class': _getIndicatorColorType(options.type, options.hasPositiveMeaning, options.text.useDefaultColor)
                    }).css(viz.utils.patchFontOptions(options.text.font)).append(that._root);
                var textBox = text.getBBox(),
                    shapeHeight = _round(textBox.height * DELTA_SIZE_COEFF),
                    shapeWidth = _round(shapeHeight / DELTA_ASPECT_RATIO);
                var shape = new _ExternalIndicator({renderer: that._renderer}).draw({
                        type: options.type,
                        hasPositiveMeaning: options.hasPositiveMeaning,
                        rect: new viz.Rectangle({
                            left: 0,
                            right: shapeWidth,
                            top: -textBox.y - shapeHeight,
                            bottom: -textBox.y
                        })
                    });
                var width = textBox.width,
                    height = textBox.height,
                    x = _round(textBox.width / 2),
                    y = -textBox.y;
                if (shape) {
                    shape.append(that._root);
                    width += shapeWidth + DELTA_INDENT;
                    x += shapeWidth + DELTA_INDENT
                }
                text.attr({
                    x: x,
                    y: y
                });
                that._layout = _extend({
                    width: width,
                    height: height
                }, options.layout);
                return that
            },
            getLayoutOptions: function() {
                return this._layout
            },
            shift: function(left, top) {
                this._root.attr({
                    translateX: left,
                    translateY: top
                });
                return this
            }
        });
        function formatText(options) {
            if (options.value !== undefined) {
                var obj = {
                        value: options.value,
                        valueText: _formatHelper.format(options.value, options.format, options.precision)
                    };
                return _isFunction(options.customizeText) ? options.customizeText.call(obj, obj) : obj.valueText
            }
            return null
        }
    })(DevExpress, jQuery);
    /*! Module dashboard, file namespaces.js */
    (function(DevExpress) {
        DevExpress.dashboard.widgetsViewer = {}
    })(DevExpress);
    /*! Module dashboard, file widgetItemFactory.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            viz = DX.viz,
            charts = viz.charts,
            gauges = viz.gauges,
            INSTANCE = 'instance';
        function getAdditionalCircularGaugeOptions(container, options) {
            return null
        }
        var getAdditionalOptionsHandlers = {circulargauge: getAdditionalCircularGaugeOptions};
        var widgetItemFactory = {
                createWidget: function(widgetType, container, options) {
                    switch ((widgetType || '').toLowerCase()) {
                        case'chart':
                            return container.dxChart(options).dxChart(INSTANCE);
                        case'piechart':
                            return container.dxPieChart(options).dxPieChart(INSTANCE);
                        case'circulargauge':
                            return container.dxCircularGauge(options).dxCircularGauge(INSTANCE);
                        case'lineargauge':
                            return container.dxLinearGauge(options).dxLinearGauge(INSTANCE);
                        default:
                            return null
                    }
                },
                getAdditionalOptions: function(widgetType, container, options) {
                    var handler = getAdditionalOptionsHandlers[(widgetType || '').toLowerCase()];
                    return handler ? handler(container, options) : null
                }
            };
        dashboard.widgetsViewer.widgetItemFactory = widgetItemFactory
    })(jQuery, DevExpress);
    /*! Module dashboard, file arrangementInfo.js */
    (function($, DX) {
        var Class = DevExpress.require("/class"),
            positioningDirection = {
                Vertical: "Vertical",
                Horizontal: "Horizontal"
            };
        var ArrangementInfo = Class.inherit({
                ctor: function(totalItemCount, itemsOnRowCount, itemWidth, itemHeight, itemMargin, direction, options) {
                    var that = this;
                    that.totalItemCount = totalItemCount;
                    that.itemsOnRowCount = itemsOnRowCount;
                    that.itemWidth = itemWidth;
                    that.itemHeight = itemHeight;
                    that.direction = direction;
                    that.itemMargin = itemMargin;
                    that.options = options;
                    that.itemsOnColumnCount = Math.ceil(that.totalItemCount / that.itemsOnRowCount)
                },
                getHeight: function(useMargin) {
                    var that = this;
                    var margin = useMargin ? 2 * that.itemMargin.Height : 0;
                    switch (that.direction) {
                        case positioningDirection.Horizontal:
                            return that.itemHeight - margin;
                        case positioningDirection.Vertical:
                            return that.itemWidth - margin;
                        default:
                            return -1
                    }
                },
                getWidth: function(useMargin) {
                    var that = this,
                        margin = useMargin ? 2 * that.itemMargin.Width : 0;
                    switch (that.direction) {
                        case positioningDirection.Horizontal:
                            return that.itemWidth - margin;
                        case positioningDirection.Vertical:
                            return that.itemHeight - margin;
                        default:
                            return 0
                    }
                }
            });
        DX.dashboard.widgetsViewer.ArrangementInfo = ArrangementInfo;
        DX.dashboard.widgetsViewer.positioningDirection = positioningDirection
    })(jQuery, DevExpress);
    /*! Module dashboard, file cssClassNames.js */
    (function($, DX) {
        DX.dashboard.widgetsViewer.cssClassNames = {
            widgetViewerIdPrefix: 'dx-widgets-viewer-style_id',
            widgetViewerContainer: 'dx-widget-viewer-container',
            widgetViewerTable: 'widget-viewer-table',
            widgetViewerRow: 'widget-viewer-row',
            widgetViewerCell: 'widget-viewer-cell',
            cardItem: 'dx-cardItem',
            widgetItem: 'dx-widgetItem',
            cardTitles: 'dx-card-titles',
            cardTitle: 'dx-card-title',
            cardSubtitle: 'dx-card-subTitle',
            cardIndicator: 'dx-card-indicator',
            cardValuesContainer: 'dx-card-values-container',
            cardVariableValue1: 'dx-card-variableValue1',
            cardVariableValue2: 'dx-card-variableValue2',
            cardMainValue: 'dx-card-mainValue',
            cardSparkline: 'dx-card-sparkline',
            cardHiddenContainer: 'dx-card-item-hidden-container',
            cardNowrapHiddenContainer: 'dx-card-item-nowrap-hidden-container',
            selectedItem: 'dx-selected-viewer-item',
            hoveredItem: 'dx-hovered-viewer-item'
        }
    })(jQuery, DevExpress);
    /*! Module dashboard, file baseItem.js */
    (function($, DX, undefined) {
        var Class = DX.require("/class"),
            widgetsViewer = DX.dashboard.widgetsViewer,
            classNames = widgetsViewer.cssClassNames;
        function patchStyles(options) {
            options.style.borderWidth = options.borderWidth || 0;
            options.selectionStyle.borderWidth = options.borderWidth || 0;
            options.hoverStyle.borderWidth = options.borderWidth || 0
        }
        var BaseItem = function(options) {
                this.ctor.apply(this, arguments)
            };
        BaseItem.prototype = {
            ctor: function(options) {
                var that = this;
                that._options = $.extend(true, {}, that._getDefaultOptions(), options);
                that._type = that._options.type || 'unknown';
                that._isSelected = !!that._options.isSelected;
                that.tag = that._options.tag;
                patchStyles(that._options);
                that._hoverEnabled = !!that._options.hoverEnabled;
                that._parentRootElement = that._options.parentRootElement
            },
            _getDefaultOptions: function() {
                return {
                        cursor: 'default',
                        isSelected: false,
                        hoverEnabled: false,
                        style: {
                            backgroundColor: '#ffffff',
                            borderStyle: 'solid',
                            borderColor: '#C8C8CC'
                        },
                        selectionStyle: {
                            backgroundColor: 'rgba(95,139,149,0.35)',
                            borderStyle: 'solid',
                            borderColor: '#5F8B95'
                        },
                        hoverStyle: {
                            backgroundColor: 'rgba(95,139,149,0.25)',
                            borderStyle: 'solid',
                            borderColor: '#5F8B95'
                        }
                    }
            },
            dispose: function() {
                var that = this;
                that._type = null;
                that._isSelected = null;
                that.tag = null;
                that._hoverEnabled = null;
                that._parentRootElement = null;
                that._itemDiv.remove();
                that._itemDiv = null
            },
            _getStyle: function(isSelected) {
                return isSelected ? this._options.selectionStyle : this._options.style
            },
            select: function() {
                var that = this;
                that._isSelected = true;
                if (that._itemDiv)
                    that._itemDiv.addClass(classNames.selectedItem)
            },
            _hover: function(isHover) {
                var that = this;
                if (that._hoverEnabled)
                    if (isHover)
                        that._itemDiv.addClass(classNames.hoveredItem);
                    else
                        that._itemDiv.removeClass(classNames.hoveredItem)
            },
            clearSelection: function() {
                var that = this;
                that._isSelected = false;
                if (that._itemDiv)
                    that._itemDiv.removeClass(classNames.selectedItem)
            },
            setClickHandler: function(handler) {
                var that = this;
                if ($.isFunction(handler) && that._itemDiv) {
                    that._itemDiv.off('click.cardItem');
                    that._itemDiv.on('click.cardItem', function() {
                        handler.call(null, {item: that})
                    })
                }
            },
            setHoverHandler: function(handler) {
                var that = this;
                if (that._itemDiv)
                    that._itemDiv.mouseenter(function() {
                        that._hover(true);
                        if ($.isFunction(handler))
                            handler.call(null, {
                                item: that,
                                state: true
                            })
                    }).mouseleave(function() {
                        that._hover(false);
                        if ($.isFunction(handler))
                            handler.call(null, {
                                item: that,
                                state: false
                            })
                    })
            },
            draw: function(width, height, index) {
                if (!this._itemDiv)
                    return this.initDraw(width, height, index);
                return this._itemDiv[0]
            },
            initDraw: function(width, height, index) {
                var that = this,
                    itemDiv;
                width = width || 0;
                height = height || 0;
                index = index || 0;
                that.index = index;
                itemDiv = $('<div>', {
                    'class': 'dx-' + that._type,
                    css: {cursor: that._options.cursor}
                });
                itemDiv.data('viewerItemIndex', index);
                that._itemDiv = itemDiv;
                return itemDiv[0]
            },
            _applyExtraStyles: function() {
                if (this._isSelected)
                    this.select();
                else
                    this.clearSelection()
            },
            toggleSelection: function() {
                if (this._isSelected)
                    this.clearSelection();
                else
                    this.select()
            },
            getWidget: function() {
                return null
            },
            finishRender: function(params) {
                params = params || {};
                var that = this,
                    clickHandler = params.clickHandler,
                    hoverHandler = params.hoverHandler,
                    drawOptions = params.drawOptions;
                that.setClickHandler(clickHandler);
                that.setHoverHandler(hoverHandler);
                that.rerender(drawOptions);
                that._applyExtraStyles()
            },
            getItemContainer: function() {
                var itemDiv = this._itemDiv;
                if (itemDiv)
                    return itemDiv[0];
                return
            },
            _formStyle: function(selector, cssProperties) {
                var resultCss = '.dx-' + this._type + ' ' + selector + '{',
                    cssProperty;
                for (cssProperty in cssProperties)
                    resultCss += cssProperty + ':' + cssProperties[cssProperty] + 'px;';
                return resultCss + '}'
            },
            getCssStyle: function(width, height, _commonItemsOptions, prefix) {
                var styleOptions = {height: height};
                if (!this._options.ignoreProportions)
                    styleOptions.width = width;
                return prefix + ' ' + this._formStyle('', styleOptions)
            },
            calcCommonItemSpecificOptions: $.noop,
            resize: $.noop,
            rerender: $.noop,
            detachItem: $.noop
        };
        widgetsViewer.BaseItem = BaseItem
    })(jQuery, DevExpress);
    /*! Module dashboard, file widgetItem.js */
    (function($, DX, undefined) {
        var widgetsViewer = DX.dashboard.widgetsViewer,
            BaseItem = widgetsViewer.BaseItem;
        var WidgetItem = function(itemData, options) {
                options = options || {};
                this._widgetType = String(options.widgetType || "").toLowerCase();
                this._itemData = itemData || {};
                this._itemData.encodeHtml = options.encodeHtml;
                options.type = 'widgetItem';
                BaseItem.prototype.ctor.call(this, options)
            };
        $.extend(WidgetItem.prototype, BaseItem.prototype, {
            dispose: function() {
                var that = this;
                BaseItem.prototype.dispose.apply(that, arguments);
                that._itemData = null;
                that._widget = null
            },
            _getDefaultOptions: function() {
                return $.extend(true, {}, BaseItem.prototype._getDefaultOptions.apply(this, arguments), {style: {
                            borderStyle: 'solid',
                            borderColor: '#ffffff'
                        }})
            },
            detachItem: function() {
                var itemDiv = this._itemDiv;
                if (itemDiv)
                    itemDiv.detach()
            },
            draw: function(width, height, index) {
                var that = this,
                    itemDiv;
                BaseItem.prototype.draw.call(that, width, height, index);
                itemDiv = that._itemDiv;
                itemDiv.css('margin', 'auto');
                that._widget = widgetsViewer.widgetItemFactory.createWidget(that._widgetType, itemDiv, that._itemData);
                return itemDiv[0]
            },
            resize: function(width, height, index) {
                if (!this._itemDiv.children().length)
                    return this.draw(width, height, index)
            },
            rerender: function(drawOptions) {
                var that = this,
                    options;
                if (that._widget) {
                    options = widgetsViewer.widgetItemFactory.getAdditionalOptions(that._widgetType, that._itemDiv, that._itemData);
                    options && $.extend(true, that._widget._options, options);
                    that._widget.render(drawOptions)
                }
            },
            getWidget: function() {
                return this._widget
            }
        });
        widgetsViewer.WidgetItem = WidgetItem
    })(jQuery, DevExpress);
    /*! Module dashboard, file cardItem.js */
    (function($, DX) {
        var formatHelper = DX.require("/utils/utils.formatHelper"),
            widgetsViewer = DX.dashboard.widgetsViewer,
            BaseItem = widgetsViewer.BaseItem,
            indicators = DX.viz.indicators,
            Indicator = indicators.DeltaIndicator,
            simpleIndicator = new DX.viz.indicators.SimpleIndicator,
            cssClassNames = widgetsViewer.cssClassNames,
            hiddenContainer = $('<div>', {id: cssClassNames.cardHiddenContainer}),
            nowrapHiddenContainer = $('<div>', {id: cssClassNames.cardNowrapHiddenContainer}),
            IS_IE8 = !DX.viz.renderers.isSvg(),
            _ceil = Math.ceil,
            _floor = Math.floor;
        function getSparklineHeight(cardHeight) {
            return cardHeight * 0.25
        }
        function calcFonts(height) {
            return {
                    title: ~~(height * 0.22),
                    subTitle: ~~(height * 0.14),
                    mainValue: ~~(height * 0.30),
                    variableValue1: ~~(height * 0.14),
                    variableValue2: ~~(height * 0.14)
                }
        }
        indicators.__simpleIndicatorMocks = function() {
            var srcSimpleIndicator = simpleIndicator,
                srcIeMode = IS_IE8;
            return {
                    setMockSimpleIndicator: function(indicator) {
                        this.simpleIndicator = simpleIndicator = indicator
                    },
                    setIeMode: function(isIe) {
                        IS_IE8 = isIe
                    },
                    resetSimpleIndicator: function() {
                        simpleIndicator = srcSimpleIndicator;
                        this.simpleIndicator = null
                    },
                    resetIeMode: function() {
                        IS_IE8 = srcIeMode
                    }
                }
        }();
        function calcRowCount(text) {
            var words = text.split(' '),
                i,
                openSpan = '<span>',
                closeSpan = ' </span>',
                markup = '',
                prevOffsetTop,
                lineCount = 1,
                clientRects;
            for (i = 0; i < words.length; i++)
                markup += openSpan + words[i] + closeSpan;
            hiddenContainer.html(openSpan + markup + closeSpan);
            clientRects = hiddenContainer.children()[0].getClientRects();
            prevOffsetTop = clientRects[0].top;
            for (i = 1; i < clientRects.length; i++)
                if (clientRects[i].top !== prevOffsetTop) {
                    lineCount++;
                    prevOffsetTop = clientRects[i].top
                }
            return lineCount
        }
        var CardItem = function(item, options) {
                options = options || {};
                var that = this,
                    getText = function(text) {
                        if (text)
                            return options.encodeHtml ? $("<div>").text(text).html() : text;
                        return ''
                    },
                    defaultValues = {
                        type: 'none',
                        hasPositiveMeaning: false,
                        text: {
                            value: '',
                            useDefaultColor: false
                        }
                    };
                options.type = 'cardItem';
                options.ignoreProportions = false;
                BaseItem.prototype.ctor.call(that, options);
                item = item || {};
                that.data = item.data || {};
                that.title = getText(item.title);
                that.subTitle = getText(item.subTitle);
                that.sparklineOptions = item.sparklineOptions || {};
                that.variableValue1 = $.extend(true, {}, defaultValues, item.variableValue1);
                that.variableValue2 = $.extend(true, {}, defaultValues, item.variableValue2);
                defaultValues.text.useDefaultColor = true;
                that.mainValue = $.extend(true, {}, defaultValues, item.mainValue);
                that.indicator = {
                    hasPositiveMeaning: that.mainValue.hasPositiveMeaning,
                    type: that.mainValue.type
                };
                if (hiddenContainer.parent().length === 0)
                    hiddenContainer.appendTo(document.body);
                if (nowrapHiddenContainer.parent().length === 0)
                    nowrapHiddenContainer.appendTo(document.body);
                that.subtitleDotsIndex = 0
            };
        $.extend(CardItem.prototype, BaseItem.prototype, {
            dispose: function() {
                var that = this;
                BaseItem.prototype.dispose.apply(that, arguments);
                that.data = null;
                that.sparklineOptions = null;
                that.indicator = null;
                that._options = null
            },
            _getDefaultOptions: function() {
                var defaults = BaseItem.prototype._getDefaultOptions.apply(this, arguments),
                    options = {
                        padding: {
                            top: 3,
                            left: 12,
                            right: 12,
                            bottom: 10
                        },
                        font: {
                            family: 'Segoe UI, HelveticaNeue, Trebuchet MS, Verdana',
                            weight: 'normal'
                        },
                        title: {color: 'black'},
                        subTitle: {color: '#B6B6B6'}
                    };
                return $.extend(true, {}, defaults, options)
            },
            _getText: function(classText) {
                var that = this;
                var valueOptions = that[classText].text,
                    value = valueOptions.value,
                    format = valueOptions.format;
                if (format)
                    return formatHelper.format(parseFloat(value), format);
                else
                    return value.toString()
            },
            _getClassFromIndicator: function(type, hasPositiveMeaning, useDefaultColor) {
                return Indicator.getIndicatorColorType(type, hasPositiveMeaning, useDefaultColor)
            },
            _getCardStyle: function(isSelected) {
                return isSelected ? this._options.selectionStyle : this._options.style
            },
            _getValueClassName: function(classText) {
                var that = this,
                    value = that[classText],
                    useDefaultColor = value.text.useDefaultColor,
                    hasPositiveMeaning = value.hasPositiveMeaning,
                    type = value.type;
                return that._getClassFromIndicator(type, hasPositiveMeaning, useDefaultColor)
            },
            _setSubtitleDotsIndex: function(index) {
                this._subtitleDotsIndex = index
            },
            _getEllipsisText: function(inputText, commonItemsOptions, containerWidth) {
                if (!inputText || inputText === '')
                    return '';
                var that = this,
                    subtitleRowsCount = calcRowCount(inputText),
                    fullTextWidth,
                    proportionalTextLen,
                    updatedText,
                    movingDirection,
                    threeDots = '...',
                    i,
                    endIndex,
                    dotsIndex = this._subtitleDotsIndex;
                if (subtitleRowsCount <= 2)
                    return inputText;
                fullTextWidth = nowrapHiddenContainer.text(inputText).width();
                proportionalTextLen = _floor(2 * containerWidth * inputText.length / fullTextWidth);
                updatedText = inputText.substring(0, proportionalTextLen - 3) + threeDots;
                subtitleRowsCount = calcRowCount(updatedText);
                if (subtitleRowsCount > 2)
                    movingDirection = -1;
                else
                    movingDirection = 1;
                for (i = 1; i < proportionalTextLen; i += 2) {
                    endIndex = dotsIndex ? dotsIndex + i * movingDirection : proportionalTextLen - 3 + i * movingDirection;
                    updatedText = inputText.substring(0, endIndex) + threeDots;
                    subtitleRowsCount = calcRowCount(updatedText);
                    if (movingDirection === 1 && subtitleRowsCount === 3) {
                        updatedText = inputText.substring(0, endIndex - 3) + threeDots;
                        that._setSubtitleDotsIndex(updatedText.length - 3);
                        return updatedText
                    }
                    if (movingDirection === -1 && subtitleRowsCount === 2) {
                        that._setSubtitleDotsIndex(updatedText.length - 3);
                        return updatedText
                    }
                }
                return updatedText
            },
            draw: function(widthCard, heightCard, cardIndex, commonItemsOptions) {
                var that = this,
                    options = that._options,
                    paddings = commonItemsOptions.paddings,
                    sparklineHeight = commonItemsOptions.sparklineHeight,
                    valuesDiv,
                    imgDiv,
                    itemDiv,
                    sparklineContainer,
                    topContentOffset = parseFloat(paddings.top) + 'px',
                    titlesDiv,
                    title,
                    subtitle,
                    variableValue1,
                    variableValue2,
                    mainValue,
                    textSubtitle = options.encodeHtml ? that._getEllipsisText(that.subTitle, commonItemsOptions, _floor((widthCard - paddings.right - paddings.left) * 0.6)) : that.subTitle;
                itemDiv = BaseItem.prototype.draw.call(that, widthCard, heightCard, cardIndex);
                title = ['<div class="', cssClassNames.cardTitle + '">', that.title, '</div>'].join('');
                subtitle = ['<div class="', cssClassNames.cardSubtitle, '">', textSubtitle, '</div>'].join('');
                titlesDiv = ['<div class="', cssClassNames.cardTitles, '"style="left:', paddings.left, 'px;right:', paddings.right, 'px;top:', topContentOffset, '">', title, subtitle, '</div>'].join('');
                imgDiv = ['<div class="', cssClassNames.cardIndicator, '" style="left:', paddings.left + 'px;">', simpleIndicator.getRenderedIndicator(that.indicator.type, that.indicator.hasPositiveMeaning), '</div>'].join('');
                variableValue1 = ['<span class="', cssClassNames.cardVariableValue1, ' ', that._getValueClassName('variableValue1') + '">', that._getText('variableValue1'), '</span>'].join('');
                variableValue2 = ['<span class="', cssClassNames.cardVariableValue2, ' ', that._getValueClassName('variableValue2'), '">', that._getText('variableValue2'), '</span>'].join('');
                mainValue = ['<span class="', cssClassNames.cardMainValue, ' ', that._getValueClassName('mainValue'), '">', that._getText('mainValue'), '</span>'].join('');
                valuesDiv = ['<div class="', cssClassNames.cardValuesContainer, '" style="left:', paddings.left, 'px;right:', paddings.right, 'px;top:', topContentOffset, '">', variableValue1, variableValue2, mainValue, '</div>'].join('');
                itemDiv.innerHTML = valuesDiv + imgDiv + titlesDiv;
                if (options.hasSparkline) {
                    sparklineContainer = $('<div>', {
                        'class': cssClassNames.cardSparkline,
                        css: {
                            left: paddings.left,
                            right: paddings.right
                        }
                    }).appendTo(itemDiv);
                    that.sparklineOptions.size = {
                        width: widthCard - paddings.left - paddings.right,
                        height: sparklineHeight
                    };
                    sparklineContainer.dxSparkline(that.sparklineOptions)
                }
                return itemDiv
            },
            resize: function(width, height, index, commonItemsOptions) {
                var that = this,
                    sparklineHeight = commonItemsOptions.sparklineHeight,
                    itemDiv = that._itemDiv,
                    imgDiv = itemDiv.find('.' + cssClassNames.cardIndicator),
                    subTitle = itemDiv.find('.' + cssClassNames.cardSubtitle),
                    sparklineContainer = itemDiv.find('.' + cssClassNames.cardSparkline),
                    paddings = that._options.padding,
                    widthWithoutpaddings = width - paddings.right - paddings.left;
                if (!itemDiv.children().length) {
                    that.draw(width, height, index, commonItemsOptions);
                    return that._itemDiv
                }
                if (IS_IE8) {
                    imgDiv.html('');
                    imgDiv.html(simpleIndicator.getRenderedIndicator(that.indicator.type, that.indicator.hasPositiveMeaning))
                }
                that._options.encodeHtml && subTitle.text(that._getEllipsisText(subTitle.text(), commonItemsOptions, _floor(widthWithoutpaddings * 0.6)));
                if (sparklineHeight)
                    sparklineContainer.dxSparkline('instance').option('size', {
                        width: widthWithoutpaddings,
                        height: sparklineHeight
                    });
                return itemDiv
            },
            getCssStyle: function(width, height, commonItemsOptions, prefix) {
                var that = this,
                    paddings = commonItemsOptions.paddings,
                    sparklineHeight = commonItemsOptions.sparklineHeight,
                    widthWithoutpaddings = width - paddings.right - paddings.left,
                    fontSizes = commonItemsOptions.fontSizes,
                    baseStyle = BaseItem.prototype.getCssStyle.call(that, width, height, commonItemsOptions, prefix),
                    titlesStyle = prefix + ' ' + that._formStyle('.' + cssClassNames.cardTitles, {'line-height': fontSizes.title}),
                    titleStyle = prefix + ' ' + that._formStyle('.' + cssClassNames.cardTitles + ' .' + cssClassNames.cardTitle, {
                        'font-size': fontSizes.title,
                        'min-height': _ceil(fontSizes.title * 1.3)
                    }),
                    subTitleStyle = prefix + ' ' + that._formStyle('.' + cssClassNames.cardTitles + ' .' + cssClassNames.cardSubtitle, {
                        'min-height': _ceil(fontSizes.subTitle * 1.3) * 2,
                        'font-size': fontSizes.subTitle
                    }),
                    indicatorContainerStyle = prefix + ' ' + that._formStyle('.' + cssClassNames.cardIndicator, {
                        height: _ceil(0.18 * (height - sparklineHeight)),
                        width: _ceil(0.24 * (height - sparklineHeight)),
                        bottom: paddings.bottom + sparklineHeight + 3
                    }),
                    valuesContainerStyle = prefix + ' ' + that._formStyle('.' + cssClassNames.cardValuesContainer, {bottom: paddings.bottom + sparklineHeight}),
                    variableValue1Style = prefix + ' ' + that._formStyle('.' + cssClassNames.cardValuesContainer + ' .' + cssClassNames.cardVariableValue1, {
                        'font-size': fontSizes.variableValue1,
                        'line-height': fontSizes.variableValue1,
                        bottom: fontSizes.variableValue2 + fontSizes.mainValue
                    }),
                    variableValue2Style = prefix + ' ' + that._formStyle('.' + cssClassNames.cardValuesContainer + ' .' + cssClassNames.cardVariableValue2, {
                        'font-size': fontSizes.variableValue2,
                        'line-height': fontSizes.variableValue2,
                        bottom: fontSizes.mainValue
                    }),
                    mainValueStyle = prefix + ' ' + that._formStyle('.' + cssClassNames.cardValuesContainer + ' .' + cssClassNames.cardMainValue, {
                        'font-size': fontSizes.mainValue,
                        'line-height': fontSizes.mainValue
                    }),
                    sparklineContainerStyle = sparklineHeight ? prefix + ' ' + that._formStyle('.' + cssClassNames.cardSparkline, {
                        height: sparklineHeight,
                        width: widthWithoutpaddings
                    }) : '';
                return [baseStyle, titlesStyle, titleStyle, subTitleStyle, indicatorContainerStyle, valuesContainerStyle, variableValue1Style, variableValue2Style, mainValueStyle, sparklineContainerStyle].join(' ')
            },
            calcCommonItemSpecificOptions: function(width, height) {
                var that = this,
                    options = that._options,
                    sparklineHeight = options.hasSparkline ? getSparklineHeight(height) : 0,
                    fontSizes = calcFonts(height - sparklineHeight),
                    paddings = $.extend({}, {
                        top: 0,
                        left: 0,
                        right: 0,
                        bottom: 0
                    }, options.padding),
                    oneRowHeight;
                hiddenContainer.width(_floor((width - paddings.right - paddings.left) * 0.6)).css('fontSize', fontSizes.subTitle);
                nowrapHiddenContainer.css('fontSize', fontSizes.subTitle);
                oneRowHeight = hiddenContainer.text('a').height();
                hiddenContainer.empty();
                simpleIndicator.renderIndicators(_ceil(0.18 * (height - sparklineHeight)));
                return {
                        paddings: paddings,
                        sparklineHeight: sparklineHeight,
                        fontSizes: fontSizes,
                        oneRowHeight: oneRowHeight
                    }
            }
        });
        CardItem.__calcFonts = calcFonts;
        widgetsViewer.CardItem = CardItem
    })(jQuery, DevExpress);
    /*! Module dashboard, file widgetsViewer.js */
    (function($, DX) {
        var DOMComponent = DX.require("/domComponent"),
            widgetsViewer = DX.dashboard.widgetsViewer,
            ArrangementInfo = widgetsViewer.ArrangementInfo,
            positioningDirection = widgetsViewer.positioningDirection,
            CardItem = widgetsViewer.CardItem,
            WidgetItem = widgetsViewer.WidgetItem,
            utils = DX.utils,
            windowUtils = DX.require("/utils/utils.window"),
            commonUtils = DX.require("/utils/utils.common"),
            registerComponent = DX.require("/componentRegistrator"),
            viewerCount = 0,
            cssClassNames = widgetsViewer.cssClassNames,
            IS_IE8 = !DX.viz.renderers.isSvg();
        widgetsViewer.__ieModeMocks = function() {
            var srcIeMode = IS_IE8;
            return {
                    setIeMode: function(isIe) {
                        IS_IE8 = isIe
                    },
                    resetIeMode: function() {
                        IS_IE8 = srcIeMode
                    }
                }
        }();
        registerComponent("dxWidgetsViewer", widgetsViewer, DOMComponent.inherit({
            _getDefaultOptions: function() {
                return $.extend(this.callBase(), {
                        dataSource: [],
                        viewer: {
                            hoverEnabled: false,
                            overflow: 'auto',
                            method: 'auto',
                            count: 1,
                            widgetType: 'card',
                            redrawOnResize: true,
                            onclick: null,
                            onRenderComplete: null,
                            onAllItemsRenderComplete: null,
                            bulkTimesRenderingTimeInterval: 200,
                            useNativeScrolling: 'auto',
                            ignorePadding: false
                        },
                        itemOptions: {
                            encodeHtml: true,
                            minWidth: undefined,
                            proportions: undefined,
                            ignoreProportions: false,
                            itemMargin: {
                                width: 5,
                                height: 5
                            },
                            borderWidth: 1,
                            hasSparkline: false
                        }
                    })
            },
            _init: function() {
                this.callBase();
                var that = this,
                    viewer = that.option('viewer'),
                    useNativeScrolling = that.option('viewer.useNativeScrolling');
                that._viewerID = viewerCount++;
                that._content = utils.renderHelper.wrapScrollable(that._rootContainer(), useNativeScrolling, viewer.overflow, 'both');
                that._scrollBarWidth = that._getScrollBarWidth();
                that._widgetType = viewer.widgetType.toLowerCase();
                that._setWidgetTypeSpecificOptions();
                that._createItems();
                if (viewer.redrawOnResize) {
                    that._resizeHandler = that._getResizeHandler();
                    windowUtils.resizeCallbacks.add(that._resizeHandler)
                }
                that.totalMarginsAndBorders = that._calcTotalMarginsAndBorders()
            },
            _dispose: function() {
                var that = this;
                windowUtils.resizeCallbacks.remove(that._resizeHandler);
                clearTimeout(that._drawTimer);
                that._drawTimer = null;
                $.each(that.itemsList || [], function(_index, item) {
                    item.dispose()
                });
                that._content = null;
                that._scrollBarWidth = null;
                that._widgetType = null;
                that.itemsList = null;
                that.totalMarginsAndBorders = null;
                that._resizeHandler = null;
                if (that._styleTag) {
                    that._styleTag.remove();
                    that._styleTag = null
                }
            },
            _getScrollable: function() {
                return utils.renderHelper.getScrollable(this._rootContainer())
            },
            _updateScrollable: function() {
                utils.renderHelper.updateScrollable(this._rootContainer())
            },
            _scrollTo: function(left, top) {
                var that = this,
                    scrollable = this._getScrollable();
                if (scrollable)
                    scrollable.scrollTo({
                        x: left,
                        y: top
                    });
                else {
                    that._rootContent().scrollLeft(left);
                    that._rootContent().scrollTop(top)
                }
            },
            _scrollOffset: function() {
                var that = this,
                    scrollable = this._getScrollable();
                if (scrollable)
                    return scrollable.scrollOffset();
                else
                    return {
                            left: that._rootContent().scrollLeft(),
                            top: that._rootContent().scrollTop()
                        }
            },
            _rootContainer: function() {
                return this.element()
            },
            _rootContent: function() {
                return this._content
            },
            _parentHeight: function() {
                var offset = 0;
                return this._rootContainer().height() - offset
            },
            _parentWidth: function() {
                var offset = 0;
                return this._rootContainer().width() - offset
            },
            _parentWidthWithoutScroll: function() {
                var that = this;
                return that._parentWidth() - that._scrollBarWidth * that._hasVerticalScroll
            },
            _parentHeightWithoutScroll: function() {
                var that = this;
                return that._parentHeight() - that._scrollBarWidth * that._hasHorizontalScroll
            },
            _getResizeHandler: function() {
                var that = this;
                return function() {
                        that.redraw()
                    }
            },
            _setWidgetTypeSpecificOptions: function() {
                var align = this.option('viewer').align,
                    itemOptions = this.option('itemOptions'),
                    proportions = itemOptions.proportions,
                    width = itemOptions.minWidth;
                if (this._widgetType === 'card') {
                    this.align = align || 'left';
                    this.minItemWidth = this.curItemWidth = width || 180;
                    this._itemProportions = proportions || (itemOptions.hasSparkline ? 0.625 : 0.5)
                }
                else {
                    this.align = align || 'center';
                    this.minItemWidth = this.curItemWidth = width || 200;
                    this._itemProportions = proportions || 1;
                    this._needVerticalCentering = true
                }
                this.minItemHeight = this.curItemHeight = this._itemProportions * this.minItemWidth
            },
            getSizeParams: function() {
                var that = this,
                    scrollOffset = that._scrollOffset(),
                    scrollableContent = that._rootContainer().find('.' + cssClassNames.widgetViewerTable),
                    itemMargin = that.option('itemOptions.itemMargin');
                return {
                        virtualSize: {
                            width: scrollableContent.outerWidth(),
                            height: scrollableContent.outerHeight()
                        },
                        scroll: {
                            top: scrollOffset.top,
                            left: scrollOffset.left,
                            size: that._scrollBarWidth,
                            horizontal: that._hasHorizontalScroll === 1,
                            vertical: that._hasVerticalScroll === 1
                        },
                        itemMargin: {
                            width: itemMargin.width,
                            height: itemMargin.height
                        }
                    }
            },
            getSelectedItems: function() {
                return $.map(this.itemsList, function(item, i) {
                        return item._isSelected ? item : null
                    })
            },
            clearSelections: function() {
                $.each(this.itemsList, function(i, item) {
                    item.clearSelection()
                })
            },
            _createItems: function() {
                var that = this,
                    data = that.option('dataSource'),
                    itemOptions = that.option('itemOptions'),
                    Item = that._widgetType == 'card' ? CardItem : WidgetItem,
                    rootElement = that._rootContent();
                that.itemsList = [];
                $.each(data, function(i, dataItem) {
                    itemOptions.widgetType = that._widgetType;
                    itemOptions.hoverEnabled = dataItem.hoverEnabled;
                    itemOptions.isSelected = dataItem.isSelected;
                    itemOptions.cursor = dataItem.cursor;
                    itemOptions.tag = dataItem.tag;
                    itemOptions.parentRootElement = rootElement;
                    that.itemsList.push(new Item(dataItem, itemOptions))
                });
                that._firstDraw = true;
                delete that._viewerParams
            },
            _optionChanged: function(args) {
                switch (args.name) {
                    case'dataSource':
                    case'itemOptions':
                        this._invalidate();
                        break;
                    default:
                        this.callBase(args);
                        break
                }
                this.callBase(args)
            },
            _refresh: function() {
                this.curItemWidth = this.minItemWidth;
                this.curItemHeight = this.minItemHeight;
                this._createItems();
                this.callBase()
            },
            _invalidate: function() {
                this.callBase()
            },
            _calcTotalMarginsAndBorders: function() {
                var itemOptions = this.option('itemOptions'),
                    borderWidth = itemOptions.borderWidth || 0;
                return {
                        width: 2 * (itemOptions.itemMargin.width + borderWidth),
                        height: 2 * (itemOptions.itemMargin.height + borderWidth)
                    }
            },
            _render: function(drawOptions) {
                var that = this,
                    viewer = that.option('viewer'),
                    onRenderComplete = viewer.onRenderComplete,
                    method = viewer.method.toLowerCase(),
                    itemCount = viewer.count,
                    clickHandler = viewer.onclick,
                    hoverHandler = viewer.onhover,
                    table,
                    contentElement = that._rootContent(),
                    overflowX = contentElement.css('overflow-x'),
                    overflowY = contentElement.css('overflow-y'),
                    parentRoot = contentElement.parent(),
                    overflowXParentRoot = parentRoot.css('overflow-x'),
                    overflowYParentRoot = parentRoot.css('overflow-y'),
                    scrollOffset = that._scrollOffset();
                clearTimeout(that._drawTimer);
                table = that._drawItems(method, itemCount);
                contentElement.html('');
                contentElement.css('overflow', 'hidden');
                parentRoot.css('overflow', 'hidden');
                if (table) {
                    that.innerContainer = $('<div>', {
                        'class': cssClassNames.widgetViewerContainer,
                        css: {
                            textAlign: that.align,
                            padding: 0,
                            margin: 0
                        }
                    }).append(table);
                    contentElement.append(that.innerContainer).find('.test');
                    $.each(that.itemsList, function(_, item) {
                        item.finishRender({
                            clickHandler: clickHandler,
                            hoverHandler: hoverHandler,
                            drawOptions: drawOptions
                        })
                    });
                    if (that._needVerticalCentering && that.innerContainer && that._viewerParams.direction === 'Horizontal')
                        that._verticalCentering(table)
                }
                contentElement.css('overflow-x', overflowX);
                contentElement.css('overflow-y', overflowY);
                parentRoot.css('overflow-x', overflowXParentRoot);
                parentRoot.css('overflow-y', overflowYParentRoot);
                that._updateScrollable();
                that._scrollTo(scrollOffset.left, scrollOffset.top);
                if (onRenderComplete)
                    onRenderComplete.call(null)
            },
            _verticalCentering: function(table) {
                var that = this,
                    differenceTop = that._parentHeight() - that._rootContainer().find('.' + cssClassNames.widgetViewerContainer).height();
                if (differenceTop > 0)
                    that.innerContainer.css({paddingTop: ~~(differenceTop / 2) + 'px'});
                else
                    that.innerContainer.css({paddingTop: '0px'})
            },
            redraw: function() {
                var drawOptions = {animate: false};
                IS_IE8 && (drawOptions.force = true);
                this._render(drawOptions)
            },
            _calcItemIndex: function(i, j, direction, rowCount, columnCount) {
                return direction == positioningDirection.Horizontal ? i * columnCount + j : j * rowCount + i
            },
            _calcVisibleRow: function(cardHeight) {
                var height = this._parentHeight();
                return Math.ceil(height / cardHeight)
            },
            _calcVisibleColumn: function(cardWidth) {
                var width = this._parentWidth();
                return Math.ceil(width / cardWidth)
            },
            _getPartArray: function(array, count, indexOfPart) {
                var result = [],
                    i = 0,
                    beg = count * indexOfPart;
                for (i = beg; i < beg + count; i++)
                    result.push(array[i]);
                return result
            },
            _createTable: function(arrangementInfo) {
                var that = this,
                    tableStruct,
                    i,
                    j,
                    indexItem,
                    col,
                    row,
                    item,
                    isHorizontal = arrangementInfo.direction == positioningDirection.Horizontal,
                    itemMargin = that.option('itemOptions.itemMargin'),
                    ignorePadding = that.option('viewer.ignorePadding'),
                    columnCount = isHorizontal ? arrangementInfo.itemsOnRowCount : arrangementInfo.itemsOnColumnCount,
                    rowCount = isHorizontal ? arrangementInfo.itemsOnColumnCount : arrangementInfo.itemsOnRowCount,
                    action,
                    itemsListLen = that.itemsList.length,
                    tableWidth,
                    widthMarginsAndBorders = that.totalMarginsAndBorders.width,
                    heightMarginsAndBorders = that.totalMarginsAndBorders.height,
                    parentWidthWithoutScroll = that._parentWidthWithoutScroll(),
                    parentHeightWithoutScroll = that._parentHeightWithoutScroll(),
                    rowHeight,
                    rowWidth,
                    cellHeight,
                    cellWidth,
                    curItemWidth = that.curItemWidth,
                    curItemHeight = that.curItemHeight,
                    curItemWidthWithoutWidthMargins = curItemWidth - 2 * itemMargin.width,
                    curItemHeightWithoutHeightMargins = curItemHeight - 2 * itemMargin.height,
                    curItem;
                if (rowCount <= 0)
                    return undefined;
                tableStruct = $('<div>', {
                    'class': cssClassNames.widgetViewerTable + ' ' + cssClassNames.widgetViewerIdPrefix + that._viewerID,
                    css: {
                        overflow: 'hidden',
                        marginLeft: '0px',
                        marginRight: '0px'
                    }
                });
                if (that._widgetType !== 'card') {
                    cellWidth = ~~Math.max((parentWidthWithoutScroll - columnCount * widthMarginsAndBorders) / columnCount, curItemWidthWithoutWidthMargins);
                    cellHeight = !isHorizontal ? ~~Math.max((parentHeightWithoutScroll - rowCount * heightMarginsAndBorders) / rowCount, curItemHeightWithoutHeightMargins) : curItemHeightWithoutHeightMargins
                }
                else {
                    cellWidth = curItemWidthWithoutWidthMargins;
                    cellHeight = curItemHeightWithoutHeightMargins
                }
                rowWidth = (cellWidth + 2 * itemMargin.width) * columnCount;
                if (ignorePadding)
                    rowWidth -= 2 * itemMargin.width;
                tableWidth = rowWidth;
                tableStruct.css({
                    height: '100%',
                    width: tableWidth + 'px'
                });
                for (i = 0; i < rowCount; i++) {
                    rowHeight = curItemHeight;
                    if (ignorePadding)
                        if (i === 0 && rowCount === 1)
                            rowHeight -= 2 * itemMargin.height;
                        else if (i === 0 || i === rowCount - 1)
                            rowHeight -= itemMargin.height;
                    row = $('<div>', {
                        'class': cssClassNames.widgetViewerRow,
                        css: {
                            clear: 'both',
                            padding: '0px',
                            margin: '0px',
                            height: rowHeight,
                            width: rowWidth
                        }
                    });
                    for (j = 0; j < columnCount; j++) {
                        item = null;
                        col = $('<div >', {
                            'class': cssClassNames.widgetViewerCell,
                            css: {
                                paddingLeft: ignorePadding && j === 0 ? 0 : itemMargin.width,
                                paddingRight: ignorePadding && j === columnCount - 1 ? 0 : itemMargin.width,
                                paddingTop: ignorePadding && i === 0 ? 0 : itemMargin.height,
                                paddingBottom: ignorePadding && i === rowCount - 1 ? 0 : itemMargin.height,
                                margin: '0px',
                                width: cellWidth + 'px',
                                height: cellHeight,
                                float: 'left'
                            }
                        });
                        indexItem = that._calcItemIndex(i, j, arrangementInfo.direction, rowCount, columnCount);
                        if (indexItem < itemsListLen) {
                            curItem = that.itemsList[indexItem];
                            if (that._firstDraw) {
                                item = curItem.initDraw(curItemWidth - widthMarginsAndBorders, curItemHeight - heightMarginsAndBorders, indexItem);
                                action = 'draw'
                            }
                            else {
                                curItem.detachItem();
                                item = curItem.getItemContainer();
                                action = 'resize'
                            }
                            if (item)
                                col.append(item)
                        }
                        row.append(col)
                    }
                    tableStruct.append(row)
                }
                that._processBatchItems(action, 0);
                that._firstDraw = false;
                return tableStruct
            },
            _getItemProportions: function() {
                return this._itemProportions
            },
            _calcViewerParams: function(parentWidth, parentHeight, itemCount, method) {
                var that = this,
                    calcRes = that._calculateArrangementInfo(parentWidth, parentHeight, itemCount, method),
                    getRowCount = function() {
                        var rowCount = undefined;
                        if (calcRes.direction === positioningDirection.Vertical)
                            rowCount = calcRes.itemsOnRowCount;
                        if (calcRes.direction === positioningDirection.Horizontal)
                            rowCount = calcRes.itemsOnColumnCount;
                        return rowCount
                    },
                    getColumnCount = function() {
                        var colCount = undefined;
                        if (calcRes.direction === positioningDirection.Vertical)
                            colCount = calcRes.itemsOnColumnCount;
                        if (calcRes.direction === positioningDirection.Horizontal)
                            colCount = calcRes.itemsOnRowCount;
                        return colCount
                    },
                    getWidthByHeight = function(height) {
                        var newItemWidth = ~~(height / (that._getItemProportions() * getRowCount()));
                        return newWidth = newItemWidth * getColumnCount()
                    },
                    getHeightByWidth = function(width) {
                        var newItemHeight = ~~(width * that._getItemProportions() / getColumnCount());
                        return newItemHeight * getRowCount()
                    };
                that._hasHorizontalScroll = 0;
                that._hasVerticalScroll = 0;
                if (getColumnCount() * calcRes.getWidth() > parentWidth) {
                    calcRes = that._calculateArrangementInfo(parentWidth, parentHeight - that._scrollBarWidth, itemCount, method);
                    if (calcRes.direction === positioningDirection.Vertical && getColumnCount() * calcRes.getWidth() < parentWidth) {
                        var newHeight = getHeightByWidth(parentWidth);
                        if (newHeight <= parentHeight)
                            return that._calculateArrangementInfo(parentWidth, newHeight, itemCount, method)
                    }
                    that._hasHorizontalScroll = 1
                }
                if (getRowCount() * calcRes.getHeight() > parentHeight) {
                    calcRes = that._calculateArrangementInfo(parentWidth - that._scrollBarWidth, parentHeight, itemCount, method);
                    if (calcRes.direction === positioningDirection.Horizontal && getRowCount() * calcRes.getHeight() < parentHeight) {
                        var newWidth = getWidthByHeight(parentHeight);
                        if (newWidth <= parentWidth)
                            return that._calculateArrangementInfo(newWidth, parentHeight, itemCount, method)
                    }
                    that._hasVerticalScroll = 1
                }
                return calcRes
            },
            _createArrangementInfo: function(width, lineCount, itemMinWidth, proportions, direction) {
                var that = this,
                    itemWidth,
                    itemHeight,
                    options = {};
                if (lineCount < 1)
                    lineCount = 1;
                if (that.itemsList.length < lineCount)
                    lineCount = that.itemsList.length;
                itemWidth = width / lineCount;
                itemHeight = ~~(itemWidth * proportions);
                if (itemWidth < itemMinWidth) {
                    itemWidth = itemMinWidth;
                    itemHeight = ~~(itemWidth * proportions)
                }
                return new ArrangementInfo(that.itemsList.length, lineCount, itemWidth, itemHeight, that.option('itemOptions').itemMargin, direction, options)
            },
            _calculateArrangementInfo: function(width, height, itemCount, method) {
                var that = this,
                    horzInfo,
                    newHorzInfo,
                    nextHorzInfo,
                    vertInfo,
                    itemHeight,
                    itemWidth,
                    countOnWidth,
                    i,
                    itemMargin = that.option('itemOptions').itemMargin,
                    itemProportions = that._getItemProportions(),
                    options = {};
                switch (method) {
                    case'column':
                        return that._createArrangementInfo(width, itemCount, that.minItemWidth, itemProportions, positioningDirection.Horizontal);
                    case'row':
                        return that._createArrangementInfo(height, itemCount, that.minItemHeight, 1 / itemProportions, positioningDirection.Vertical);
                    case'auto':
                        if (height < that.minItemHeight && width / that.minItemWidth >= that.itemsList.length)
                            return new ArrangementInfo(that.itemsList.length, that.itemsList.length, that.minItemWidth, that.minItemHeight, itemMargin, positioningDirection.Horizontal, options);
                        horzInfo = that._createArrangementInfo(width, ~~(width / that.minItemWidth), that.minItemWidth, itemProportions, positioningDirection.Horizontal);
                        for (i = horzInfo.itemsOnRowCount - 1; i >= 1; i--) {
                            newHorzInfo = that._createArrangementInfo(width, i, that.minItemWidth, itemProportions, positioningDirection.Horizontal);
                            if (height >= newHorzInfo.itemsOnColumnCount * newHorzInfo.getHeight(false))
                                horzInfo = newHorzInfo;
                            else
                                break
                        }
                        nextHorzInfo = that._createArrangementInfo(width, horzInfo.itemsOnRowCount - 1, that.minItemWidth, itemProportions, positioningDirection.Horizontal);
                        vertInfo = that._createArrangementInfo(height, nextHorzInfo.itemsOnColumnCount, that.minItemHeight, 1 / itemProportions, positioningDirection.Vertical);
                        itemHeight = vertInfo.getHeight(false);
                        itemWidth = vertInfo.getWidth(false);
                        countOnWidth = nextHorzInfo.itemsOnRowCount;
                        if (horzInfo.getHeight(false) < itemHeight && width >= countOnWidth * itemWidth)
                            horzInfo = new ArrangementInfo(that.itemsList.length, countOnWidth, itemWidth, itemHeight, itemMargin, positioningDirection.Horizontal, options);
                        if (height < horzInfo.itemsOnColumnCount * horzInfo.getHeight(false)) {
                            vertInfo = that._createArrangementInfo(height, horzInfo.itemsOnColumnCount, that.minItemHeight, 1 / itemProportions, positioningDirection.Vertical);
                            itemHeight = vertInfo.getHeight(false);
                            itemWidth = vertInfo.getWidth(false);
                            countOnWidth = vertInfo.itemsOnColumnCount;
                            if (height >= vertInfo.itemsOnRowCount * itemHeight && width >= countOnWidth * itemWidth)
                                horzInfo = new ArrangementInfo(that.itemsList.length, Math.min(~~(width / itemWidth), that.itemsList.length), itemWidth, itemHeight, itemMargin, positioningDirection.Horizontal, options)
                        }
                        return horzInfo;
                    default:
                        return null
                }
            },
            _drawItems: function(method, itemCount) {
                method = method || 'auto';
                itemCount = itemCount || 1;
                var that = this,
                    parentWidth = that._parentWidth(),
                    parentHeight = that._parentHeight(),
                    viewerParams = that._viewerParams,
                    itemsList = that.itemsList,
                    ignorePadding = that.option('viewer.ignorePadding'),
                    itemMargin = that.option('itemOptions.itemMargin'),
                    extendedWidth = ignorePadding ? 2 * itemMargin.width : 0,
                    extendedHeight = ignorePadding ? 2 * itemMargin.height : 0;
                if (!itemsList.length)
                    return '';
                viewerParams = that._viewerParams = that._calcViewerParams(parentWidth + extendedWidth, parentHeight + extendedHeight, itemCount, method);
                that.curItemHeight = ~~viewerParams.getHeight();
                that.curItemWidth = ~~viewerParams.getWidth();
                return that._createTable(that._viewerParams)
            },
            getItemByIndex: function(index) {
                var indexForCheck,
                    result;
                if (commonUtils.isNumber(index)) {
                    indexForCheck = Number(index);
                    result = this.itemsList[indexForCheck]
                }
                return commonUtils.isDefined(result) ? result : null
            },
            _getScrollBarWidth: function() {
                var that = this,
                    useNativeScrolling = that.option('viewer.useNativeScrolling'),
                    scrollBarWidth = 0;
                if (useNativeScrolling == 'auto' || useNativeScrolling === false)
                    return scrollBarWidth;
                var container = $('<div>', {css: {
                            position: 'absolute',
                            top: 0,
                            left: 0,
                            visibility: 'hidden',
                            width: 200,
                            height: 150,
                            overflow: 'hidden'
                        }}).appendTo($('body'));
                var p = $('<p>', {css: {
                            width: '100%',
                            height: 300
                        }}).appendTo(container);
                var widthWithoutScrollBar = p.width();
                container.css('overflow', that.option('viewer').overflow);
                var widthWithScrollBar = p.width();
                scrollBarWidth = widthWithoutScrollBar - widthWithScrollBar;
                if (scrollBarWidth > 0)
                    scrollBarWidth++;
                container.remove();
                return scrollBarWidth
            },
            _processBatchItems: function(functionName, startIndex) {
                var that = this,
                    dateStart = new Date,
                    itemsList = that.itemsList,
                    itemsListLen = itemsList.length,
                    totalMarginsAndBorders = that.totalMarginsAndBorders,
                    widthMarginsAndBorders = totalMarginsAndBorders.width,
                    heightMarginsAndBorders = totalMarginsAndBorders.height,
                    viewer = that.option('viewer'),
                    onAllItemsRenderComplete = viewer.onAllItemsRenderComplete,
                    bulkTimesRenderingTimeInterval = viewer.bulkTimesRenderingTimeInterval,
                    itemWidth = that.curItemWidth - widthMarginsAndBorders,
                    itemHeight = that.curItemHeight - heightMarginsAndBorders,
                    commonItemsOptions,
                    itemsStyle;
                if (!itemsList[startIndex])
                    return;
                commonItemsOptions = itemsList[startIndex].calcCommonItemSpecificOptions(itemWidth, itemHeight);
                itemsStyle = itemsList[startIndex].getCssStyle(itemWidth, itemHeight, commonItemsOptions, '.' + cssClassNames.widgetViewerIdPrefix + that._viewerID);
                that._styleTag && that._styleTag.remove();
                that._styleTag = $('<style type="text/css">' + itemsStyle + '</style>').appendTo('head');
                that.countCallInternalProcessBatchItems = 0;
                var internalProcessBatchItems = function() {
                        that.countCallInternalProcessBatchItems++;
                        if ($.isFunction(itemsList[startIndex][functionName])) {
                            dateStart = new Date;
                            do {
                                if (startIndex < itemsListLen) {
                                    itemsList[startIndex][functionName](itemWidth, itemHeight, startIndex, commonItemsOptions);
                                    ++startIndex
                                }
                                if (!itemsList[startIndex]) {
                                    if ($.isFunction(onAllItemsRenderComplete))
                                        onAllItemsRenderComplete.call(null);
                                    return
                                }
                            } while (new Date - dateStart < bulkTimesRenderingTimeInterval);
                            that._drawTimer = setTimeout(function() {
                                internalProcessBatchItems()
                            }, 0)
                        }
                    };
                internalProcessBatchItems()
            }
        }))
    })(jQuery, DevExpress);
    /*! Module dashboard, file dropDownMenu.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            Class = DX.require("/class"),
            removeEvent = DX.require("/ui/events/ui.events.remove");
        dashboard.dropDownMenu = DevExpress.require("/class").inherit({
            ctor: function ctor(options) {
                this.options = options;
                this.$imageDiv = $('<div/>', {
                    'class': 'dx-icon-' + options.className,
                    title: options.title
                });
                this._initialize()
            },
            setState: function(enabled) {
                this.options.enabled = enabled
            },
            getButtonImageDiv: function() {
                return this.$imageDiv
            },
            _initialize: function() {
                var that = this,
                    options = that.options,
                    $imageDiv = that.getButtonImageDiv(),
                    timeoutID,
                    $contentList,
                    $mainDiv = $('<div/>', {
                        'class': 'dx-dashboard-menu-background',
                        overflow: 'visible',
                        css: {'z-index': 20000}
                    }),
                    isVisible = function() {
                        return $mainDiv.is(':visible')
                    },
                    setHidden = function() {
                        $mainDiv.hide()
                    },
                    setVisible = function() {
                        $mainDiv.show();
                        var offset = $imageDiv.offset();
                        if ($imageDiv[0].getBoundingClientRect().top + $imageDiv.outerHeight() + $mainDiv.outerHeight() > $(window).height())
                            offset.top -= $mainDiv.outerHeight() + parseInt($imageDiv.css('paddingTop'));
                        else
                            offset.top += $imageDiv.innerHeight() + parseInt($mainDiv.css('paddingTop'));
                        if ($imageDiv[0].getBoundingClientRect().left + $imageDiv.outerWidth() + $mainDiv.outerWidth() > $(window).width())
                            offset.left -= $mainDiv.outerWidth() - $imageDiv.outerWidth();
                        if (offset.left < 0) {
                            $mainDiv.css({'max-width': $mainDiv.width() - Math.abs(offset.left)});
                            offset.left = 0
                        }
                        else
                            $mainDiv.css({'max-width': '100%'});
                        $mainDiv.offset(offset)
                    };
                $imageDiv.appendTo(options.$container);
                $contentList = $('<ul/>').appendTo($mainDiv);
                $.each(options.elementNames, function(index, elementName) {
                    var $li = $('<li/>', {}).text(elementName).on('dxclick', function() {
                            $mainDiv.find('li').css({fontWeight: 'normal'});
                            if (options.selectedIndex !== undefined)
                                $(this).css({fontWeight: 'bold'});
                            options.selectItem(index, elementName);
                            setHidden()
                        }).hover(function() {
                            $(this).addClass('dx-dashboard-menu-item-selected')
                        }, function() {
                            $(this).removeClass('dx-dashboard-menu-item-selected')
                        });
                    if (options.selectedIndex === index)
                        $li.css({fontWeight: 'bold'});
                    $li.appendTo($contentList)
                });
                $mainDiv.on(removeEvent.name, function() {
                    clearTimeout(timeoutID)
                }).mouseleave(function() {
                    timeoutID = setTimeout(setHidden, 400)
                });
                $mainDiv.mouseenter(function() {
                    clearTimeout(timeoutID)
                });
                that._applyEllipsis($mainDiv);
                $mainDiv.appendTo(options.$parentContainer);
                $imageDiv.on('dxclick', function() {
                    if (!that.options.enabled)
                        return;
                    if (isVisible())
                        setHidden();
                    else
                        setVisible()
                });
                $imageDiv.hover(function() {
                    clearTimeout(timeoutID)
                }, function() {
                    timeoutID = setTimeout(setHidden, 400)
                })
            },
            _applyEllipsis: function($menuDiv) {
                var windowWidth = $(window).width(),
                    menuDivWidth = $menuDiv.width(),
                    difference = windowWidth - menuDivWidth,
                    menuDivLeftOffset = $menuDiv[0].getBoundingClientRect().left;
                if (difference > 0) {
                    if (menuDivLeftOffset + menuDivWidth > windowWidth) {
                        $menuDiv.offset({left: 0});
                        $menuDiv.css({position: 'absolute'})
                    }
                }
                else
                    $menuDiv.css({
                        width: windowWidth - menuDivLeftOffset,
                        textOverflow: "ellipsis",
                        "-o-text-overflow": "ellipsis",
                        "-moz-binding": "url( 'bindings.xml#ellipsis' )",
                        "white-space": "nowrap"
                    })
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file dialogForm.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            utils = DX.utils,
            Class = DevExpress.require("/class"),
            Button = DX.require("/ui/widgets/ui.button");
        dashboard.dialogClasses = {
            form: 'dx-dashboard-form',
            element: 'dx-dashboard-dialog-element',
            elementOffset: 'dx-dashboard-dialog-element-offset',
            name: 'dx-dashboard-dialog-element-name',
            disabledName: 'dx-dashboard-dialog-element-name-disabled',
            box: 'dx-dashboard-dialog-element-box',
            buttons: 'dx-dashboard-dialog-buttons',
            elementTextBox: 'dx-dashboard-dialog-element-text-box',
            elementNumberBox: 'dx-dashboard-dialog-element-number-box'
        };
        dashboard.buttonWidth = '90px';
        dashboard.scrollableControlHeight = '390px';
        dashboard.widgetMargin = 1;
        dashboard.dialogControlTypes = {
            dxTagBox: 'dxTagBox',
            dxSelectBox: 'dxSelectBox',
            dxRadioGroup: 'dxRadioGroup',
            dxNumberBox: 'dxNumberBox',
            dxTextBox: 'dxTextBox',
            dxCheckBox: 'dxCheckBox',
            dxDateBox: 'dxDateBox'
        };
        dashboard.dialogForm = Class.inherit({
            ctor: function ctor(options) {
                this.options = options;
                if (this.options.renderControls)
                    this.renderControls = this.options.renderControls;
                if (this.options.setActualState)
                    this.setActualState = this.options.setActualState;
                if (this.options.buttons)
                    this.buttons = this.options.buttons;
                this._initialize()
            },
            _initialize: function() {
                var localizer = dashboard.data.localizer,
                    that = this,
                    options = that.options,
                    buttons = that.buttons,
                    renderControls = that.renderControls,
                    $popup = undefined,
                    popupInstance = undefined,
                    $scrollable = undefined,
                    $hideDialogButton = undefined,
                    $content = undefined,
                    $buttons = undefined;
                $popup = $('<div/>').appendTo(options.$dialogContainer).dxPopup({
                    title: options.title,
                    showCloseButton: true,
                    animation: {
                        show: {
                            type: 'fade',
                            from: 0,
                            to: 1
                        },
                        hide: {
                            type: 'fade',
                            from: 1,
                            to: 0
                        }
                    },
                    position: {
                        my: 'center',
                        at: 'center',
                        of: options.$dialogContainer
                    },
                    container: options.$dialogContainer,
                    onContentReady: function(args) {
                        that.controlCreationCallbacks.fire(args.component, that.popupInstance.content().find('.' + dashboard.dialogClasses.form))
                    },
                    onShowing: function(e) {
                        if (e.component.content().children().data("dxDataGrid")) {
                            this.option("resizeEnabled", true);
                            e.component.content().css("padding-bottom", dashboard.parametersDialogSizes.dataGridBottomPadding);
                            e.component.overlayContent().dxResizable("instance").off("resize", this.content().children().data("dxDataGrid").updateDimensions).on("resize", this.content().children().data("dxDataGrid").updateDimensions).option("minHeight", dashboard.parametersDialogSizes.dialogFormMinHeight);
                            e.component.overlayContent().dxResizable("instance").option("minWidth", dashboard.parametersDialogSizes.dialogFormMinWidth)
                        }
                        var formWidth = that._setLabelsWidth();
                        that.setActualState(formWidth)
                    }
                });
                popupInstance = $popup.data('dxPopup');
                if (options.width)
                    popupInstance.option('width', options.width);
                if (options.height)
                    popupInstance.option('height', options.height);
                that.popupInstance = popupInstance;
                that.controlCreationCallbacks = $.Callbacks();
                $content = popupInstance.content();
                $content.append(options.$content);
                renderControls(that.controlCreationCallbacks);
                that.controlCreationCallbacks.add(function(component) {
                    $buttons = $('<div/>', {'class': dashboard.dialogClasses.buttons}).appendTo(component.content());
                    if (buttons)
                        $.each(buttons, function(index, button) {
                            new Button($('<div/>').appendTo($buttons), {
                                text: button.name,
                                onClick: function() {
                                    button.func();
                                    if (button.hide)
                                        popupInstance.hide()
                                }
                            })
                        })
                })
            },
            showDialog: function() {
                this.popupInstance.show()
            },
            hideDialog: function() {
                this.popupInstance.hide()
            },
            _setLabelsWidth: function() {
                var that = this,
                    width = 0,
                    maxWidth = 400,
                    minWidth = 80,
                    leftOffset = 10,
                    $div = undefined,
                    $span = undefined,
                    $label = undefined,
                    $labelsContainer = $('<div/>', {'class': 'dx-dashboard-labels-container'}).appendTo($('.dx-dashboard-container')),
                    $controlContainer,
                    boxWidth = 0;
                $.each(that.popupInstance.content().find('.' + dashboard.dialogClasses.form).children(), function(index, div) {
                    $div = $(div);
                    $span = $('<span/>').append($div.find('.' + dashboard.dialogClasses.name).text());
                    $labelsContainer.append($span).append('<br/>');
                    $controlContainer = $div.find('.' + dashboard.dialogClasses.box);
                    boxWidth = Math.max(boxWidth, $controlContainer.outerWidth())
                });
                $.each($labelsContainer.children(), function(index, label) {
                    $label = $(label);
                    width = Math.max(width, $label.width())
                });
                width = Math.max(minWidth, Math.min(maxWidth, width)) + leftOffset;
                $labelsContainer.remove();
                $('.' + dashboard.dialogClasses.name).css('width', width);
                return width + boxWidth + dashboard.utils.pxToNumber($('.' + dashboard.dialogClasses.name).css("margin-right")) + 2 * dashboard.widgetMargin
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file parametersDialog.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            Class = DevExpress.require("/class");
        dashboard.dialogClasses.parametersForm = 'dx-dashboard-parameters-form';
        dashboard.dialogClasses.allowNullCheckBox = 'dx-parameter-allownull-checkbox';
        dashboard.dialogClasses.allowNullCheckBoxSize = 'dx-datagrid-checkbox-size';
        dashboard.dialogClasses.valueEditor = 'dx-parameter-value-editor';
        dashboard.parametersDialogSizes = {
            columnParameterNameWidth: '150',
            columnValueWidth: '100%',
            columnAllowNullWidth: '90',
            dialogFormWidth: '500',
            dialogFormHeight: '500',
            dialogFormMinWidth: '392',
            dialogFormMinHeight: '240',
            dataGridHeight: '100%',
            dialogFormElementsHeigth: 172,
            dataGridBottomPadding: 100,
            gridRowHeight: 34
        };
        dashboard.parameterTypes = {
            string: 'String',
            int: 'Int',
            float: 'Float',
            bool: 'Bool',
            dateTime: 'DateTime',
            selector: 'Selector',
            multiselector: 'Multiselector',
            guid: 'Guid'
        };
        dashboard.parametersButton = Class.inherit({
            ctor: function ctor(options) {
                this.options = options;
                if (this.options.showParametersDialog)
                    this.showParametersDialog = this.options.showParametersDialog;
                this._initialize()
            },
            setState: function(state) {
                var that = this,
                    className = !state.loading ? that.options.className : that.options.loadingClassName;
                that.$showParametersButton.attr('class', className);
                that.options.enabled = state.enabled
            },
            _initialize: function() {
                var localizer = dashboard.data.localizer,
                    that = this,
                    options = that.options,
                    showParametersDialog = that.showParametersDialog,
                    $showParametersButton = undefined;
                $showParametersButton = $('<div/>', {
                    'class': options.className,
                    title: localizer.getString(dashboard.localizationId.buttonNames.ParametersFormCaption)
                }).appendTo(options.$container);
                $showParametersButton.click(function() {
                    if (!!that.options.enabled)
                        showParametersDialog()
                });
                that.$showParametersButton = $showParametersButton
            }
        });
        dashboard.parametersDialog = Class.inherit({
            ctor: function ctor(options) {
                this.options = options;
                if (this.options.getParametersCollection)
                    this.getParametersCollection = this.options.getParametersCollection;
                if (this.options.submitParameters)
                    this.submitParameters = this.options.submitParameters;
                this._initialize()
            },
            _initialize: function() {
                var localizer = dashboard.data.localizer,
                    that = this,
                    options = that.options,
                    numberOfParameters = that.getParametersCollection().getVisibleParameters().length,
                    scroll = numberOfParameters > 8,
                    dataGridActualHeight = (numberOfParameters + 1) * dashboard.parametersDialogSizes.gridRowHeight,
                    allowNullColumn,
                    submitParameters = that.submitParameters,
                    parameterEntities = [],
                    $parametersForm = $('<div/>', {'class': dashboard.dialogClasses.form + ' ' + dashboard.dialogClasses.parametersForm});
                $.each(that.getParametersCollection().getVisibleParameters(), function(index, parameter) {
                    if (parameter.getAllowNull()) {
                        allowNullColumn = true;
                        return false
                    }
                });
                var gridColumns = [{
                            dataField: 'description',
                            caption: 'Parameter Name',
                            dataType: 'string',
                            width: dashboard.parametersDialogSizes.columnParameterNameWidth
                        }, {
                            dataField: '$divValueEditor',
                            caption: 'Value',
                            width: dashboard.parametersDialogSizes.columnValueWidth,
                            cssClass: 'dx-parameter-value-editor',
                            alignment: 'center',
                            showEditorAlways: true,
                            editCellTemplate: function(cellElement, cellInfo) {
                                if (cellInfo.data.controlName === "dxCheckBox")
                                    cellInfo.data.$divValueEditor.appendTo(cellElement);
                                else
                                    cellInfo.data.$divValueEditor = cellElement;
                                cellInfo.data._addControl()
                            }
                        }];
                if (allowNullColumn)
                    gridColumns.push({
                        dataField: '$divAllowNull',
                        caption: 'Allow Null',
                        width: dashboard.parametersDialogSizes.columnAllowNullWidth,
                        alignment: 'center',
                        cellTemplate: function(container, options) {
                            options.value.appendTo(container)
                        }
                    });
                that.dialogForm = new dashboard.dialogForm({
                    title: localizer.getString(dashboard.localizationId.buttonNames.ParametersFormCaption),
                    $dialogContainer: options.$parametersDialogContainer,
                    width: allowNullColumn ? dashboard.parametersDialogSizes.dialogFormWidth : dashboard.parametersDialogSizes.dialogFormMinWidth,
                    height: scroll ? dashboard.parametersDialogSizes.dialogFormHeight : dataGridActualHeight + dashboard.parametersDialogSizes.dialogFormElementsHeigth,
                    $content: $parametersForm,
                    buttons: [{
                            name: localizer.getString(dashboard.localizationId.buttonNames.ButtonReset),
                            func: function() {
                                var parametersCollection = that.getParametersCollection();
                                $.each(parameterEntities, function(index, parameterEntity) {
                                    parameterEntity.setValue(parametersCollection.getParameterDefaultValue(parameterEntity.name))
                                })
                            }
                        }, {
                            name: localizer.getString(dashboard.localizationId.buttonNames.ButtonSubmit),
                            hide: true,
                            func: function() {
                                var dashboardParameters = [];
                                $.each(parameterEntities, function(index, parameterEntity) {
                                    dashboardParameters[index] = {
                                        Name: parameterEntity.name,
                                        Value: parameterEntity.getValue()
                                    }
                                });
                                submitParameters(dashboardParameters)
                            }
                        }, {
                            name: localizer.getString(dashboard.localizationId.buttonNames.ButtonCancel),
                            hide: true,
                            func: function(){}
                        }],
                    renderControls: function(controlCreationCallbacks) {
                        $.each(that.getParametersCollection().getVisibleParameters(), function(index, parameter) {
                            parameterEntities.push(that._getParameterEntity(parameter, controlCreationCallbacks))
                        })
                    },
                    setActualState: function() {
                        var parametersCollection = that.getParametersCollection();
                        $.each(parameterEntities, function(index, parameterEntity) {
                            parameterEntity.setValue(parametersCollection.getParameterValue(parameterEntity.name))
                        })
                    }
                });
                $parametersForm.dxDataGrid({
                    dataSource: parameterEntities,
                    showColumnLines: true,
                    showRowLines: true,
                    width: '100%',
                    height: dashboard.parametersDialogSizes.dataGridHeight,
                    allowColumnResizing: true,
                    sorting: {mode: "none"},
                    columns: gridColumns
                })
            },
            show: function() {
                this.dialogForm.showDialog()
            },
            hide: function() {
                this.dialogForm.hideDialog()
            },
            _getControlTypes: function() {
                var controlTypes = [];
                controlTypes[dashboard.parameterTypes.multiselector] = dashboard.dialogControlTypes.dxTagBox;
                controlTypes[dashboard.parameterTypes.selector] = dashboard.dialogControlTypes.dxSelectBox;
                controlTypes[dashboard.parameterTypes.string] = dashboard.dialogControlTypes.dxTextBox;
                controlTypes[dashboard.parameterTypes.int] = dashboard.dialogControlTypes.dxNumberBox;
                controlTypes[dashboard.parameterTypes.float] = dashboard.dialogControlTypes.dxNumberBox;
                controlTypes[dashboard.parameterTypes.bool] = dashboard.dialogControlTypes.dxCheckBox;
                controlTypes[dashboard.parameterTypes.dateTime] = dashboard.dialogControlTypes.dxDateBox;
                controlTypes[dashboard.parameterTypes.guid] = dashboard.dialogControlTypes.dxTextBox;
                return controlTypes
            },
            _getParameterEntity: function(parameter, controlCreationCallbacks) {
                var entityOptions = {
                        name: parameter.getName(),
                        description: parameter.getDescription(),
                        defaultValue: parameter.getDefaultValue(),
                        controlCreationCallbacks: controlCreationCallbacks,
                        allowNull: parameter.getAllowNull(),
                        allowMultiselect: parameter.getAllowMultiselect(),
                        type: parameter.getType(),
                        value: parameter.getValue()
                    },
                    controlTypes = this._getControlTypes(),
                    localizer = dashboard.data.localizer;
                if (parameter.getValues().length > 0) {
                    var values = [];
                    $.each(parameter.getValues(), function(index, value) {
                        values.push({
                            value: value.getValue(),
                            displayValue: value.getDisplayText()
                        })
                    });
                    if (entityOptions.allowMultiselect)
                        return new dashboard.parameterEntity($.extend(true, entityOptions, {
                                controlName: controlTypes[dashboard.parameterTypes.multiselector],
                                valueName: 'values',
                                controlOptions: {
                                    dataSource: {
                                        store: new DevExpress.data.ArrayStore(values),
                                        paginate: false
                                    },
                                    showDropButton: true,
                                    showSelectionControls: true,
                                    itemTemplate: function(item) {
                                        return item.displayValue
                                    },
                                    fieldTemplate: function(selectedItem) {
                                        var values = '';
                                        if (!selectedItem.context) {
                                            if (selectedItem.length > 0 && (typeof selectedItem[0] !== "object" || selectedItem[0] === null)) {
                                                values += selectedItem[0];
                                                this._valuesData.splice(0, 1);
                                                this.option('values').splice(0, 1);
                                                selectedItem.splice(0, 1)
                                            }
                                            for (var i in selectedItem) {
                                                values += selectedItem[i].displayValue;
                                                if (i == selectedItem.length - 1)
                                                    break;
                                                values += ', '
                                            }
                                        }
                                        var $input = $('<input>').addClass('dx-texteditor-input').val(values).prop('readonly', true).css({
                                                padding: "7px 7px 5px",
                                                border: 'none',
                                                width: "100%",
                                                boxSizing: 'border-box',
                                                readOnly: true
                                            });
                                        $input.addClass('dx-dashboard-filter-item-multitext');
                                        var $inputWrapper = $('<div>').css({marginRight: "40px"}).append($input);
                                        var $textBox = $('<div>').css({
                                                float: 'right',
                                                width: "37px"
                                            }).dxTextBox();
                                        return $('<div>').append($textBox).append($inputWrapper)
                                    },
                                    displayExpr: 'displayValue',
                                    valueExpr: 'value',
                                    placeholder: localizer.getString(dashboard.localizationId.ParametersSelectorText)
                                }
                            }));
                    else
                        return new dashboard.parameterEntity($.extend(true, entityOptions, {
                                controlName: controlTypes[dashboard.parameterTypes.selector],
                                valueName: 'value',
                                controlOptions: {
                                    dataSource: {
                                        store: new DevExpress.data.ArrayStore(values),
                                        paginate: false
                                    },
                                    itemTemplate: function(item) {
                                        return item.displayValue
                                    },
                                    displayExpr: 'displayValue',
                                    valueExpr: 'value',
                                    placeholder: localizer.getString(dashboard.localizationId.ParametersSelectorText)
                                }
                            }))
                }
                else {
                    entityOptions.controlName = controlTypes[parameter.getType()];
                    switch (parameter.getType()) {
                        case dashboard.parameterTypes.string:
                            return new dashboard.parameterEntity($.extend(true, entityOptions, {valueName: 'value'}));
                            break;
                        case dashboard.parameterTypes.int:
                            return new dashboard.parameterEntity($.extend(true, entityOptions, {
                                    valueName: 'value',
                                    controlOptions: {
                                        showSpinButtons: true,
                                        step: 1,
                                        onKeyPress: function(e) {
                                            var event = e.jQueryEvent,
                                                str = String.fromCharCode(event.keyCode);
                                            if (!/[0-9]/.test(str))
                                                event.preventDefault()
                                        }
                                    }
                                }));
                            break;
                        case dashboard.parameterTypes.float:
                            return new dashboard.parameterEntity($.extend(true, entityOptions, {
                                    valueName: 'value',
                                    controlOptions: {
                                        showSpinButtons: true,
                                        step: 0.1,
                                        onKeyPress: function(e) {
                                            var str = String.fromCharCode(e.jQueryEvent.keyCode);
                                            if (!/[0-9.,]/.test(str))
                                                event.preventDefault()
                                        }
                                    }
                                }));
                            break;
                        case dashboard.parameterTypes.bool:
                            return new dashboard.parameterEntity($.extend(true, entityOptions, {valueName: 'value'}));
                            break;
                        case dashboard.parameterTypes.dateTime:
                            return new dashboard.parameterEntity($.extend(true, entityOptions, {
                                    valueName: 'value',
                                    controlOptions: {
                                        useCalendar: true,
                                        width: '100%'
                                    }
                                }));
                            break;
                        case dashboard.parameterTypes.guid:
                            return new dashboard.parameterEntity($.extend(true, entityOptions, {
                                    valueName: 'value',
                                    controlOptions: {mask: '99999999-9999-9999-9999-999999999999'}
                                }));
                            break
                    }
                }
            }
        });
        dashboard.parameterEntity = Class.inherit({
            ctor: function ctor(options) {
                this.name = options.name;
                this.type = options.type;
                this.description = options.description ? options.description : this.name;
                this.defaultValue = options.defaultValue;
                this.value = options.value;
                this.values = new Array;
                this.allowNull = options.allowNull;
                this.allowMultiselect = options.allowMultiselect;
                this.controlName = options.controlName;
                this.valueName = options.valueName;
                this.controlCreationCallbacks = options.controlCreationCallbacks;
                this.controlOptions = options.controlOptions;
                this.$divValueEditor = $('<div/>', {'class': this.type.toLowerCase() + '-value-editor ' + dashboard.dialogClasses.valueEditor + ' ' + dashboard.dialogClasses.allowNullCheckBoxSize});
                this.$divAllowNull = this.allowNull ? $('<div/>', {'class': dashboard.dialogClasses.allowNullCheckBox + ' ' + dashboard.dialogClasses.allowNullCheckBoxSize}) : $('<center>n/a</center>')
            },
            getValue: function() {
                if (this.valueName === "values") {
                    var mas = new Array;
                    var found = false;
                    for (var i in this.control.option('values'))
                        if (this.control.option('values')[i] === this.control.option('value'))
                            found = true;
                    if (this.control.option('value') && !found)
                        mas.push(this.control.option('value'));
                    for (var i in this.control.option('values'))
                        mas.push(this.control.option('values')[i]);
                    if (mas.length === 0)
                        mas = null;
                    return mas
                }
                else
                    return this.control.option('value')
            },
            setValue: function(value) {
                var valueClone = [];
                if (value !== null && typeof value === 'object' && value.length > 0)
                    for (var i in value)
                        valueClone[i] = value[i];
                else
                    valueClone = value;
                if (this.valueName === "values") {
                    if (valueClone && typeof valueClone === "object")
                        return this.control.option(this.valueName, valueClone);
                    if (valueClone === null)
                        return this.control.option("value", valueClone);
                    this.values.length = 1;
                    this.values[0] = valueClone;
                    return this.control.option(this.valueName, this.values)
                }
                else
                    this.control.option(this.valueName, value)
            },
            _addControl: function() {
                var that = this,
                    options = $.extend(true, {}, that.controlOptions);
                that.controlCreationCallbacks.add(function(component) {
                    that.$divValueEditor[that.controlName](options);
                    that.control = that.$divValueEditor.data(that.controlName);
                    that.control.option("onValueChanged", that.allowNull ? function(e) {
                        var f = null;
                        if (e.values && e.values !== that.values && e.values.length > 0) {
                            that.values = e.values;
                            f = false
                        }
                        if (e.value !== that.value && e.value !== null) {
                            that.value = e.value;
                            f = false
                        }
                        else if (e.value !== null)
                            f = false;
                        else if (e.value === null)
                            f = true;
                        if (f === false)
                            that.allowNullControl.option('value', false);
                        else if (f === true)
                            that.allowNullControl.option('value', true)
                    } : null);
                    if (that.allowNull) {
                        that.$divAllowNull.dxCheckBox({
                            value: false,
                            onValueChanged: function valueChanged(e) {
                                if (e.value) {
                                    that.control.option('value', null);
                                    if (that.valueName === 'values')
                                        that.control.option('values', new Array)
                                }
                                else {
                                    if (that.control.option('value') !== that.value)
                                        that.control.option('value', that.value);
                                    if (that.valueName === 'values' && that.control.option('values').length !== that.values.length)
                                        that.control.option('values', that.values)
                                }
                            }
                        });
                        that.allowNullControl = that.$divAllowNull.data('dxCheckBox')
                    }
                })
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file exportOptionsGroups.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            Class = DevExpress.require("/class");
        dashboard.labeledEditor = Class.inherit({
            ctor: function ctor(options) {
                this.options = options;
                this._initialize()
            },
            _initialize: function() {
                var that = this;
                var controlOptions = $.extend(true, that._getControlOptions(that.options), that.options.controlOptions || {});
                that.valueName = controlOptions.valueName;
                that.controlName = that.options.controlName;
                that.$label = $('<div/>', {'class': dashboard.dialogClasses.name + ' ' + that._generateElementNameClassName(that.controlName)}).append(that.options.labelText + ':');
                that.$editor = $('<div/>', {'class': dashboard.dialogClasses.box + ' ' + that._getElementClassName(that.controlName)})[that.controlName](controlOptions);
                that.enabled = true
            },
            setEnabled: function(enabled) {
                var that = this;
                that.enabled = enabled;
                var classOperation = enabled ? 'removeClass' : 'addClass';
                that.$label[classOperation](dashboard.dialogClasses.disabledName);
                that.$editor.data(that.controlName).option("disabled", !enabled)
            },
            setVisibility: function(visible) {
                var that = this;
                that.enabled = visible;
                if (visible) {
                    that.$label.css('display', 'inline-block');
                    that.$editor.css('display', 'inline-block')
                }
                else {
                    that.$label.hide();
                    that.$editor.hide()
                }
            },
            set: function(value) {
                var that = this;
                that.$editor.data(that.controlName).option(that.valueName, value)
            },
            get: function() {
                var that = this;
                return that.$editor.data(that.controlName).option(that.valueName)
            },
            _getControlOptions: function(options) {
                switch (options.controlName) {
                    case dashboard.dialogControlTypes.dxSelectBox:
                        return {
                                dataSource: {
                                    store: new DevExpress.data.ArrayStore(options.values),
                                    paginate: false
                                },
                                itemTemplate: function(item) {
                                    return item.displayValue
                                },
                                displayExpr: 'displayValue',
                                valueExpr: 'value',
                                valueName: 'value'
                            };
                    case dashboard.dialogControlTypes.dxRadioGroup:
                        var dataSource = [];
                        $.each(options.values, function(index, value) {
                            dataSource.push(value.value)
                        });
                        return {
                                name: options.name,
                                dataSource: dataSource,
                                itemTemplate: function(item) {
                                    return $.grep(options.values, function(value) {
                                            return value.value === item
                                        })[0].displayValue
                                },
                                valueName: 'value'
                            };
                    case dashboard.dialogControlTypes.dxNumberBox:
                        return {
                                valueName: 'value',
                                onKeyPress: function(e) {
                                    var event = e.jQueryEvent,
                                        str = String.fromCharCode(event.keyCode);
                                    if (!/[0-9]/.test(str))
                                        event.preventDefault()
                                }
                            };
                    case dashboard.dialogControlTypes.dxCheckBox:
                        return {valueName: 'value'};
                    case dashboard.dialogControlTypes.dxTextBox:
                        return {valueName: 'value'}
                }
            },
            _generateElementNameClassName: function(controlName) {
                if (controlName == dashboard.dialogControlTypes.dxRadioGroup || controlName == dashboard.dialogControlTypes.dxCheckBox)
                    return dashboard.dialogClasses.name + '-top';
                return dashboard.dialogClasses.name + '-middle'
            },
            _getElementClassName: function(controlName) {
                switch (controlName) {
                    case dashboard.dialogControlTypes.dxSelectBox:
                    case dashboard.dialogControlTypes.dxTextBox:
                        return dashboard.dialogClasses.elementTextBox;
                    case dashboard.dialogControlTypes.dxNumberBox:
                        return dashboard.dialogClasses.elementNumberBox;
                    default:
                        return ""
                }
            }
        });
        dashboard.optionsGroup = Class.inherit({
            ctor: function ctor(){},
            setEnabled: function(enabled) {
                var that = this;
                that.enabled = enabled;
                $.each(that.getEditors(), function(index, editor) {
                    editor.setEnabled(enabled)
                })
            },
            set: function(documentInfo){},
            apply: function(documentInfo){},
            getEditors: function(){}
        });
        dashboard.captionOptionsGroup = dashboard.optionsGroup.inherit({
            ctor: function ctor(showCaption) {
                this._initialize(showCaption)
            },
            _initialize: function(showCaption) {
                var that = this;
                this.showCaption = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.ShowTitle),
                    controlName: dashboard.dialogControlTypes.dxCheckBox,
                    controlOptions: {onValueChanged: function(args) {
                            that.caption.setEnabled(args.component.option('value'))
                        }}
                });
                this.caption = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.Title),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                });
                this.filterStatePresentation = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.FilterStatePresentation),
                    controlName: dashboard.dialogControlTypes.dxSelectBox,
                    values: [dashboard.filterStatePresentation.none, dashboard.filterStatePresentation.after, dashboard.filterStatePresentation.afterAndSplitPage]
                });
                that.caption.setEnabled(showCaption)
            },
            set: function(documentInfo) {
                this.showCaption.set(documentInfo.commonOptions.includeCaption);
                this.caption.set(documentInfo.commonOptions.caption);
                this.filterStatePresentation.set(documentInfo.commonOptions.filterStatePresentation)
            },
            apply: function(documentInfo) {
                documentInfo.commonOptions.includeCaption = this.showCaption.get();
                documentInfo.commonOptions.caption = this.caption.get();
                documentInfo.commonOptions.filterStatePresentation = this.filterStatePresentation.get()
            },
            getEditors: function() {
                return [this.showCaption, this.caption, this.filterStatePresentation]
            }
        });
        dashboard.scaleModeOptionsGroup = dashboard.optionsGroup.inherit({
            ctor: function ctor(scaleMode) {
                this._initialize(scaleMode)
            },
            _initialize: function(scaleMode) {
                var that = this;
                this.scaleMode = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.ScaleMode),
                    controlName: dashboard.dialogControlTypes.dxSelectBox,
                    values: [dashboard.scaleMode.none, dashboard.scaleMode.useScaleFactor, dashboard.scaleMode.autoFitToPageWidth],
                    controlOptions: {onValueChanged: function(args) {
                            that._setScaleModeOptionsVisibility(args.component.option('value'))
                        }}
                });
                this.scaleFactor = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.ScaleFactor),
                    controlName: dashboard.dialogControlTypes.dxNumberBox,
                    controlOptions: {
                        min: 0,
                        onKeyPress: function(e) {
                            var str = String.fromCharCode(e.jQueryEvent.keyCode);
                            if (!/[0-9.,]/.test(str))
                                event.preventDefault()
                        }
                    }
                });
                this.autoFitPageCount = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.AutoFitPageCount),
                    controlName: dashboard.dialogControlTypes.dxNumberBox,
                    controlOptions: {min: 0}
                });
                that._setScaleModeOptionsVisibility(scaleMode)
            },
            set: function(documentInfo) {
                this.scaleMode.set(documentInfo.scaleMode == dashboard.scaleMode.autoFitWithinOnePage.value ? dashboard.scaleMode.none.value : documentInfo.scaleMode);
                this.scaleFactor.set(documentInfo.scaleFactor);
                this.autoFitPageCount.set(documentInfo.autoFitPageCount);
                this._setScaleModeOptionsVisibility(this.scaleMode.get())
            },
            apply: function(documentInfo) {
                if (this.scaleMode.enabled) {
                    documentInfo.scaleMode = this.scaleMode.get();
                    documentInfo.scaleFactor = this.scaleFactor.get();
                    documentInfo.autoFitPageCount = this.autoFitPageCount.get()
                }
                else {
                    documentInfo.scaleFactor = 1;
                    documentInfo.autoFitPageCount = 1
                }
            },
            getEditors: function() {
                return [this.scaleMode, this.scaleFactor, this.autoFitPageCount]
            },
            _setScaleModeOptionsVisibility: function(scaleMode) {
                var that = this;
                switch (scaleMode) {
                    case dashboard.scaleMode.none.value:
                    case dashboard.scaleMode.autoFitWithinOnePage.value:
                        that.scaleFactor.setVisibility(false);
                        that.autoFitPageCount.setVisibility(false);
                        break;
                    case dashboard.scaleMode.useScaleFactor.value:
                        that.scaleFactor.setVisibility(true);
                        that.autoFitPageCount.setVisibility(false);
                        break;
                    case dashboard.scaleMode.autoFitToPageWidth.value:
                        that.scaleFactor.setVisibility(false);
                        that.autoFitPageCount.setVisibility(true);
                        break
                }
            }
        });
        dashboard.documentOptionsGroup = dashboard.optionsGroup.inherit({
            ctor: function ctor(includeCaption) {
                this._initialize(includeCaption)
            },
            _initialize: function(includeCaption) {
                this.pageLayout = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.PageLayout),
                    controlName: dashboard.dialogControlTypes.dxRadioGroup,
                    values: [dashboard.pageLayout.portrait, dashboard.pageLayout.landscape]
                });
                this.paperKind = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.PaperKind),
                    controlName: dashboard.dialogControlTypes.dxSelectBox,
                    values: [dashboard.paperKind.letter, dashboard.paperKind.legal, dashboard.paperKind.executive, dashboard.paperKind.a5, dashboard.paperKind.a4, dashboard.paperKind.a3]
                });
                this.captionOptionsGroup = new dashboard.captionOptionsGroup(includeCaption)
            },
            set: function(documentInfo) {
                this.pageLayout.set(documentInfo.pageLayout);
                this.paperKind.set(documentInfo.paperKind);
                this.captionOptionsGroup.set(documentInfo)
            },
            apply: function(documentInfo) {
                documentInfo.pageLayout = this.pageLayout.get();
                documentInfo.paperKind = this.paperKind.get();
                this.captionOptionsGroup.apply(documentInfo)
            },
            getEditors: function() {
                var that = this;
                var editors = [this.pageLayout, this.paperKind];
                $.each(that.captionOptionsGroup.getEditors(), function(name, editor) {
                    editors.push(editor)
                });
                return editors
            }
        });
        dashboard.simplyDocumentOptionsWithScaleModeGroup = dashboard.optionsGroup.inherit({
            ctor: function ctor(showCaption, scaleMode) {
                this._initialize(showCaption, scaleMode)
            },
            _initialize: function(showCaption, scaleMode) {
                var that = this;
                this.pageLayout = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.PageLayout),
                    controlName: dashboard.dialogControlTypes.dxRadioGroup,
                    values: [dashboard.pageLayout.portrait, dashboard.pageLayout.landscape]
                });
                this.paperKind = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.PaperKind),
                    controlName: dashboard.dialogControlTypes.dxSelectBox,
                    values: [dashboard.paperKind.letter, dashboard.paperKind.legal, dashboard.paperKind.executive, dashboard.paperKind.a5, dashboard.paperKind.a4, dashboard.paperKind.a3]
                });
                this.showCaption = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.ShowTitle),
                    controlName: dashboard.dialogControlTypes.dxCheckBox,
                    controlOptions: {onValueChanged: function(args) {
                            that.caption.setEnabled(args.component.option('value'))
                        }}
                });
                this.caption = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.Title),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                });
                this.scaleModeOptionsGroup = new dashboard.scaleModeOptionsGroup(scaleMode);
                this.fileName = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.FileName),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                });
                that.caption.setEnabled(showCaption)
            },
            set: function(documentInfo) {
                this.pageLayout.set(documentInfo.pageLayout);
                this.paperKind.set(documentInfo.paperKind);
                this.showCaption.set(documentInfo.commonOptions.includeCaption);
                this.caption.set(documentInfo.commonOptions.caption);
                this.scaleModeOptionsGroup.set(documentInfo);
                this.fileName.set(documentInfo.fileName)
            },
            apply: function(documentInfo) {
                documentInfo.pageLayout = this.pageLayout.get();
                documentInfo.paperKind = this.paperKind.get();
                documentInfo.commonOptions.includeCaption = this.showCaption.get();
                documentInfo.commonOptions.caption = this.caption.get();
                this.scaleModeOptionsGroup.apply(documentInfo);
                documentInfo.fileName = this.fileName.get()
            },
            getEditors: function() {
                var that = this;
                var editors = [this.fileName, this.pageLayout, this.paperKind, this.showCaption, this.caption];
                $.each(that.scaleModeOptionsGroup.getEditors(), function(name, editor) {
                    editors.push(editor)
                });
                return editors
            }
        });
        dashboard.dashboardOptionsGroup = dashboard.optionsGroup.inherit({
            ctor: function ctor(showTitle, scaleMode) {
                this._initialize(showTitle, scaleMode)
            },
            _initialize: function(showTitle, scaleMode) {
                var that = this;
                this.pageLayout = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.PageLayout),
                    controlName: dashboard.dialogControlTypes.dxRadioGroup,
                    values: [dashboard.pageLayout.portrait, dashboard.pageLayout.landscape, dashboard.pageLayout.auto],
                    controlOptions: {onValueChanged: function(args) {
                            that._setScaleModeOptionsVisibility(args.value)
                        }}
                });
                this.paperKind = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.PaperKind),
                    controlName: dashboard.dialogControlTypes.dxSelectBox,
                    values: [dashboard.paperKind.letter, dashboard.paperKind.legal, dashboard.paperKind.executive, dashboard.paperKind.a5, dashboard.paperKind.a4, dashboard.paperKind.a3]
                });
                this.showTitle = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.ShowTitle),
                    controlName: dashboard.dialogControlTypes.dxCheckBox,
                    controlOptions: {onValueChanged: function(args) {
                            that.title.setEnabled(args.component.option('value'))
                        }}
                });
                this.title = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.Title),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                });
                this.filterStatePresentation = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.FilterStatePresentation),
                    controlName: dashboard.dialogControlTypes.dxSelectBox,
                    values: [dashboard.filterStatePresentation.none, dashboard.filterStatePresentation.after, dashboard.filterStatePresentation.afterAndSplitPage]
                });
                that.title.setEnabled(showTitle);
                this.scaleModeOptionsGroup = new dashboard.scaleModeOptionsGroup(scaleMode);
                if (scaleMode == dashboard.scaleMode.autoFitWithinOnePage.value)
                    that._setScaleModeOptionsVisibility(dashboard.pageLayout.auto.value);
                this.fileName = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.FileName),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                })
            },
            set: function(documentInfo) {
                this.pageLayout.set(documentInfo.scaleMode == dashboard.scaleMode.autoFitWithinOnePage.value ? dashboard.pageLayout.auto.value : documentInfo.pageLayout);
                this.paperKind.set(documentInfo.paperKind);
                this.showTitle.set(documentInfo.showTitle);
                this.title.set(documentInfo.title);
                this.filterStatePresentation.set(documentInfo.commonOptions.filterStatePresentation);
                this.scaleModeOptionsGroup.set(documentInfo);
                this.fileName.set(documentInfo.fileName)
            },
            apply: function(documentInfo) {
                if (this.pageLayout.get() != dashboard.pageLayout.auto.value)
                    documentInfo.pageLayout = this.pageLayout.get();
                if (this.pageLayout.get() == dashboard.pageLayout.auto.value)
                    documentInfo.scaleMode = dashboard.scaleMode.autoFitWithinOnePage.value;
                documentInfo.paperKind = this.paperKind.get();
                documentInfo.showTitle = this.showTitle.get();
                documentInfo.title = this.title.get();
                documentInfo.commonOptions.filterStatePresentation = this.filterStatePresentation.get();
                this.scaleModeOptionsGroup.apply(documentInfo);
                documentInfo.fileName = this.fileName.get()
            },
            getEditors: function() {
                var that = this;
                var editors = [this.fileName, this.pageLayout, this.paperKind];
                editors.push(this.showTitle);
                editors.push(this.title);
                editors.push(this.filterStatePresentation);
                $.each(that.scaleModeOptionsGroup.getEditors(), function(name, editor) {
                    editors.push(editor)
                });
                return editors
            },
            _setScaleModeOptionsVisibility: function(pageLayout) {
                var that = this;
                that.scaleModeOptionsGroup.setEnabled(pageLayout != dashboard.pageLayout.auto.value)
            }
        });
        dashboard.gridOptionsGroup = dashboard.optionsGroup.inherit({
            ctor: function ctor(includeCaption, fitToPageWidth, scaleMode) {
                this._initialize(includeCaption, fitToPageWidth, scaleMode)
            },
            _initialize: function(includeCaption, fitToPageWidth, scaleMode) {
                var that = this;
                this.documentOptionsGroup = new dashboard.documentOptionsGroup(includeCaption);
                this.printHeadersOnEveryPage = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.PrintHeadersOnEveryPage),
                    controlName: dashboard.dialogControlTypes.dxCheckBox
                });
                this.fitToPageWidth = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.FitToPageWidth),
                    controlName: dashboard.dialogControlTypes.dxCheckBox,
                    controlOptions: {onValueChanged: function(args) {
                            that._setScaleModeOptionsVisibility(args.component.option('value'))
                        }}
                });
                this.scaleModeOptionsGroup = new dashboard.scaleModeOptionsGroup(scaleMode);
                this._setScaleModeOptionsVisibility(fitToPageWidth);
                this.fileName = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.FileName),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                })
            },
            set: function(documentInfo) {
                this.documentOptionsGroup.set(documentInfo);
                this.printHeadersOnEveryPage.set(documentInfo.gridOptions.printHeadersOnEveryPage);
                this.fitToPageWidth.set(documentInfo.gridOptions.fitToPageWidth);
                this.scaleModeOptionsGroup.set(documentInfo);
                this.fileName.set(documentInfo.fileName)
            },
            apply: function(documentInfo) {
                this.documentOptionsGroup.apply(documentInfo);
                documentInfo.gridOptions.printHeadersOnEveryPage = this.printHeadersOnEveryPage.get();
                documentInfo.gridOptions.fitToPageWidth = this.fitToPageWidth.get();
                this.scaleModeOptionsGroup.apply(documentInfo);
                documentInfo.fileName = this.fileName.get()
            },
            getEditors: function() {
                var that = this;
                var editors = [];
                editors.push(that.fileName);
                $.each(that.documentOptionsGroup.getEditors(), function(name, editor) {
                    editors.push(editor)
                });
                editors.push(that.printHeadersOnEveryPage);
                editors.push(that.fitToPageWidth);
                $.each(that.scaleModeOptionsGroup.getEditors(), function(name, editor) {
                    editors.push(editor)
                });
                return editors
            },
            _setScaleModeOptionsVisibility: function(fitToPageWidth) {
                var that = this;
                that.scaleModeOptionsGroup.setEnabled(!fitToPageWidth)
            }
        });
        dashboard.pivotOptionsGroup = dashboard.optionsGroup.inherit({
            ctor: function ctor(includeCaption, scaleMode) {
                this._initialize(includeCaption, scaleMode)
            },
            _initialize: function(includeCaption, scaleMode) {
                var that = this;
                this.documentOptionsGroup = new dashboard.documentOptionsGroup(includeCaption);
                this.printHeadersOnEveryPage = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.PrintHeadersOnEveryPage),
                    controlName: dashboard.dialogControlTypes.dxCheckBox
                });
                this.scaleModeOptionsGroup = new dashboard.scaleModeOptionsGroup(scaleMode);
                this.fileName = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.FileName),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                })
            },
            set: function(documentInfo) {
                this.documentOptionsGroup.set(documentInfo);
                this.printHeadersOnEveryPage.set(documentInfo.pivotOptions.printHeadersOnEveryPage);
                this.scaleModeOptionsGroup.set(documentInfo);
                this.fileName.set(documentInfo.fileName)
            },
            apply: function(documentInfo) {
                this.documentOptionsGroup.apply(documentInfo);
                documentInfo.pivotOptions.printHeadersOnEveryPage = this.printHeadersOnEveryPage.get();
                this.scaleModeOptionsGroup.apply(documentInfo);
                documentInfo.fileName = this.fileName.get()
            },
            getEditors: function() {
                var that = this;
                var editors = [];
                editors.push(that.fileName);
                $.each(that.documentOptionsGroup.getEditors(), function(name, editor) {
                    editors.push(editor)
                });
                editors.push(that.printHeadersOnEveryPage);
                $.each(that.scaleModeOptionsGroup.getEditors(), function(name, editor) {
                    editors.push(editor)
                });
                return editors
            }
        });
        dashboard.chartOptionsGroup = dashboard.optionsGroup.inherit({
            ctor: function ctor(includeCaption) {
                this._initialize(includeCaption)
            },
            _initialize: function(includeCaption) {
                var that = this;
                this.pageLayout = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.PageLayout),
                    controlName: dashboard.dialogControlTypes.dxRadioGroup,
                    values: [dashboard.pageLayout.portrait, dashboard.pageLayout.landscape, dashboard.pageLayout.auto]
                });
                this.paperKind = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.PaperKind),
                    controlName: dashboard.dialogControlTypes.dxSelectBox,
                    values: [dashboard.paperKind.letter, dashboard.paperKind.legal, dashboard.paperKind.executive, dashboard.paperKind.a5, dashboard.paperKind.a4, dashboard.paperKind.a3]
                });
                this.captionOptionsGroup = new dashboard.captionOptionsGroup(includeCaption);
                this.sizeMode = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.SizeMode),
                    controlName: dashboard.dialogControlTypes.dxRadioGroup,
                    values: [dashboard.sizeMode.none, dashboard.sizeMode.stretch, dashboard.sizeMode.zoom]
                });
                this.fileName = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.FileName),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                })
            },
            set: function(documentInfo) {
                this.pageLayout.set(documentInfo.chartOptions.automaticPageLayout ? dashboard.pageLayout.auto.value : documentInfo.pageLayout);
                this.paperKind.set(documentInfo.paperKind);
                this.captionOptionsGroup.set(documentInfo);
                this.sizeMode.set(documentInfo.chartOptions.sizeMode);
                this.fileName.set(documentInfo.fileName)
            },
            apply: function(documentInfo) {
                if (this.pageLayout.get() != dashboard.pageLayout.auto.value)
                    documentInfo.pageLayout = this.pageLayout.get();
                documentInfo.chartOptions.automaticPageLayout = this.pageLayout.get() == dashboard.pageLayout.auto.value;
                documentInfo.paperKind = this.paperKind.get();
                this.captionOptionsGroup.apply(documentInfo);
                documentInfo.chartOptions.sizeMode = this.sizeMode.get();
                documentInfo.fileName = this.fileName.get()
            },
            getEditors: function() {
                var that = this;
                var editors = [this.fileName, this.pageLayout, this.paperKind];
                $.each(that.captionOptionsGroup.getEditors(), function(name, editor) {
                    editors.push(editor)
                });
                editors.push(that.sizeMode);
                return editors
            }
        });
        dashboard.mapOptionsGroup = dashboard.optionsGroup.inherit({
            ctor: function ctor(includeCaption) {
                this._initialize(includeCaption)
            },
            _initialize: function(includeCaption) {
                var that = this;
                this.pageLayout = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.PageLayout),
                    controlName: dashboard.dialogControlTypes.dxRadioGroup,
                    values: [dashboard.pageLayout.portrait, dashboard.pageLayout.landscape, dashboard.pageLayout.auto]
                });
                this.paperKind = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.PaperKind),
                    controlName: dashboard.dialogControlTypes.dxSelectBox,
                    values: [dashboard.paperKind.letter, dashboard.paperKind.legal, dashboard.paperKind.executive, dashboard.paperKind.a5, dashboard.paperKind.a4, dashboard.paperKind.a3]
                });
                this.captionOptionsGroup = new dashboard.captionOptionsGroup(includeCaption);
                this.sizeMode = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.SizeMode),
                    controlName: dashboard.dialogControlTypes.dxRadioGroup,
                    values: [dashboard.sizeMode.none, dashboard.sizeMode.zoom]
                });
                this.fileName = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.FileName),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                })
            },
            set: function(documentInfo) {
                this.pageLayout.set(documentInfo.chartOptions.automaticPageLayout ? dashboard.pageLayout.auto.value : documentInfo.pageLayout);
                this.paperKind.set(documentInfo.paperKind);
                this.captionOptionsGroup.set(documentInfo);
                this.sizeMode.set(documentInfo.mapOptions.sizeMode);
                this.fileName.set(documentInfo.fileName)
            },
            apply: function(documentInfo) {
                if (this.pageLayout.get() != dashboard.pageLayout.auto.value)
                    documentInfo.pageLayout = this.pageLayout.get();
                documentInfo.mapOptions.automaticPageLayout = this.pageLayout.get() == dashboard.pageLayout.auto.value;
                documentInfo.paperKind = this.paperKind.get();
                this.captionOptionsGroup.apply(documentInfo);
                documentInfo.mapOptions.sizeMode = this.sizeMode.get();
                documentInfo.fileName = this.fileName.get()
            },
            getEditors: function() {
                var that = this;
                var editors = [this.fileName, this.pageLayout, this.paperKind];
                $.each(that.captionOptionsGroup.getEditors(), function(name, editor) {
                    editors.push(editor)
                });
                editors.push(that.sizeMode);
                return editors
            }
        });
        dashboard.rangeFilterOptionsGroup = dashboard.optionsGroup.inherit({
            ctor: function ctor(includeCaption) {
                this._initialize(includeCaption)
            },
            _initialize: function(includeCaption) {
                var that = this;
                this.pageLayout = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.PageLayout),
                    controlName: dashboard.dialogControlTypes.dxRadioGroup,
                    values: [dashboard.pageLayout.portrait, dashboard.pageLayout.landscape, dashboard.pageLayout.auto]
                });
                this.paperKind = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.PaperKind),
                    controlName: dashboard.dialogControlTypes.dxSelectBox,
                    values: [dashboard.paperKind.letter, dashboard.paperKind.legal, dashboard.paperKind.executive, dashboard.paperKind.a5, dashboard.paperKind.a4, dashboard.paperKind.a3]
                });
                this.captionOptionsGroup = new dashboard.captionOptionsGroup(includeCaption);
                this.sizeMode = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.SizeMode),
                    controlName: dashboard.dialogControlTypes.dxRadioGroup,
                    values: [dashboard.sizeMode.none, dashboard.sizeMode.stretch, dashboard.sizeMode.zoom]
                });
                this.fileName = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.FileName),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                })
            },
            set: function(documentInfo) {
                this.pageLayout.set(documentInfo.rangeFilterOptions.automaticPageLayout ? dashboard.pageLayout.auto.value : documentInfo.pageLayout);
                this.paperKind.set(documentInfo.paperKind);
                this.captionOptionsGroup.set(documentInfo);
                this.sizeMode.set(documentInfo.rangeFilterOptions.sizeMode);
                this.fileName.set(documentInfo.fileName)
            },
            apply: function(documentInfo) {
                if (this.pageLayout.get() != dashboard.pageLayout.auto.value)
                    documentInfo.pageLayout = this.pageLayout.get();
                documentInfo.rangeFilterOptions.automaticPageLayout = this.pageLayout.get() == dashboard.pageLayout.auto.value;
                documentInfo.paperKind = this.paperKind.get();
                this.captionOptionsGroup.apply(documentInfo);
                documentInfo.rangeFilterOptions.sizeMode = this.sizeMode.get();
                documentInfo.fileName = this.fileName.get()
            },
            getEditors: function() {
                var that = this;
                var editors = [this.fileName, this.pageLayout, this.paperKind];
                $.each(that.captionOptionsGroup.getEditors(), function(name, editor) {
                    editors.push(editor)
                });
                editors.push(that.sizeMode);
                return editors
            }
        });
        dashboard.pieOptionsGroup = dashboard.optionsGroup.inherit({
            ctor: function ctor(includeCaption, autoArrangeContent, scaleMode) {
                this._initialize(includeCaption, autoArrangeContent, scaleMode)
            },
            _initialize: function(includeCaption, autoArrangeContent, scaleMode) {
                var that = this;
                this.documentOptionsGroup = new dashboard.documentOptionsGroup(includeCaption);
                this.autoArrangeContent = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.AutoArrangeContent),
                    controlName: dashboard.dialogControlTypes.dxCheckBox,
                    controlOptions: {onValueChanged: function(args) {
                            that._setScaleModeOptionsVisibility(args.component.option('value'))
                        }}
                });
                this.scaleModeOptionsGroup = new dashboard.scaleModeOptionsGroup(scaleMode);
                that._setScaleModeOptionsVisibility(autoArrangeContent);
                this.fileName = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.FileName),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                })
            },
            set: function(documentInfo) {
                this.documentOptionsGroup.set(documentInfo);
                this.autoArrangeContent.set(documentInfo.pieOptions.autoArrangeContent);
                this.scaleModeOptionsGroup.set(documentInfo);
                this.fileName.set(documentInfo.fileName)
            },
            apply: function(documentInfo) {
                this.documentOptionsGroup.apply(documentInfo);
                documentInfo.pieOptions.autoArrangeContent = this.autoArrangeContent.get();
                this.scaleModeOptionsGroup.apply(documentInfo);
                documentInfo.fileName = this.fileName.get()
            },
            getEditors: function() {
                var that = this;
                var editors = [];
                editors.push(that.fileName);
                $.each(that.documentOptionsGroup.getEditors(), function(name, editor) {
                    editors.push(editor)
                });
                editors.push(that.autoArrangeContent);
                $.each(that.scaleModeOptionsGroup.getEditors(), function(name, editor) {
                    editors.push(editor)
                });
                return editors
            },
            _setScaleModeOptionsVisibility: function(autoArrangeContent) {
                var that = this;
                that.scaleModeOptionsGroup.setEnabled(!autoArrangeContent)
            }
        });
        dashboard.gaugeOptionsGroup = dashboard.optionsGroup.inherit({
            ctor: function ctor(includeCaption, autoArrangeContent, scaleMode) {
                this._initialize(includeCaption, autoArrangeContent, scaleMode)
            },
            _initialize: function(includeCaption, autoArrangeContent, scaleMode) {
                var that = this;
                this.documentOptionsGroup = new dashboard.documentOptionsGroup(includeCaption);
                this.autoArrangeContent = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.AutoArrangeContent),
                    controlName: dashboard.dialogControlTypes.dxCheckBox,
                    controlOptions: {onValueChanged: function(args) {
                            that._setScaleModeOptionsVisibility(args.component.option('value'))
                        }}
                });
                this.scaleModeOptionsGroup = new dashboard.scaleModeOptionsGroup(scaleMode);
                that._setScaleModeOptionsVisibility(autoArrangeContent);
                this.fileName = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.FileName),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                })
            },
            set: function(documentInfo) {
                this.documentOptionsGroup.set(documentInfo);
                this.autoArrangeContent.set(documentInfo.gaugeOptions.autoArrangeContent);
                this.scaleModeOptionsGroup.set(documentInfo);
                this.fileName.set(documentInfo.fileName)
            },
            apply: function(documentInfo) {
                this.documentOptionsGroup.apply(documentInfo);
                documentInfo.gaugeOptions.autoArrangeContent = this.autoArrangeContent.get();
                this.scaleModeOptionsGroup.apply(documentInfo);
                documentInfo.fileName = this.fileName.get()
            },
            getEditors: function() {
                var that = this;
                var editors = [];
                editors.push(that.fileName);
                $.each(that.documentOptionsGroup.getEditors(), function(name, editor) {
                    editors.push(editor)
                });
                editors.push(that.autoArrangeContent);
                $.each(that.scaleModeOptionsGroup.getEditors(), function(name, editor) {
                    editors.push(editor)
                });
                return editors
            },
            _setScaleModeOptionsVisibility: function(autoArrangeContent) {
                var that = this;
                that.scaleModeOptionsGroup.setEnabled(!autoArrangeContent)
            }
        });
        dashboard.cardOptionsGroup = dashboard.optionsGroup.inherit({
            ctor: function ctor(includeCaption, autoArrangeContent, scaleMode) {
                this._initialize(includeCaption, autoArrangeContent, scaleMode)
            },
            _initialize: function(includeCaption, autoArrangeContent, scaleMode) {
                var that = this;
                this.documentOptionsGroup = new dashboard.documentOptionsGroup(includeCaption);
                this.autoArrangeContent = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.AutoArrangeContent),
                    controlName: dashboard.dialogControlTypes.dxCheckBox,
                    controlOptions: {onValueChanged: function(args) {
                            that._setScaleModeOptionsVisibility(args.component.option('value'))
                        }}
                });
                this.scaleModeOptionsGroup = new dashboard.scaleModeOptionsGroup(scaleMode);
                that._setScaleModeOptionsVisibility(autoArrangeContent);
                this.fileName = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.FileName),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                })
            },
            set: function(documentInfo) {
                this.documentOptionsGroup.set(documentInfo);
                this.autoArrangeContent.set(documentInfo.cardOptions.autoArrangeContent);
                this.scaleModeOptionsGroup.set(documentInfo);
                this.fileName.set(documentInfo.fileName)
            },
            apply: function(documentInfo) {
                this.documentOptionsGroup.apply(documentInfo);
                documentInfo.cardOptions.autoArrangeContent = this.autoArrangeContent.get();
                this.scaleModeOptionsGroup.apply(documentInfo);
                documentInfo.fileName = this.fileName.get()
            },
            getEditors: function() {
                var that = this;
                var editors = [];
                editors.push(that.fileName);
                $.each(that.documentOptionsGroup.getEditors(), function(name, editor) {
                    editors.push(editor)
                });
                editors.push(that.autoArrangeContent);
                $.each(that.scaleModeOptionsGroup.getEditors(), function(name, editor) {
                    editors.push(editor)
                });
                return editors
            },
            _setScaleModeOptionsVisibility: function(autoArrangeContent) {
                var that = this;
                that.scaleModeOptionsGroup.setEnabled(!autoArrangeContent)
            }
        });
        dashboard.imageOptionsGroup = dashboard.optionsGroup.inherit({
            ctor: function ctor(showTitle) {
                this._initialize(showTitle)
            },
            _initialize: function(showTitle) {
                var that = this;
                this.imageFormat = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.ImageFormat),
                    controlName: dashboard.dialogControlTypes.dxSelectBox,
                    values: [dashboard.imageFormat.png, dashboard.imageFormat.gif, dashboard.imageFormat.jpg]
                });
                this.showTitle = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.ShowTitle),
                    controlName: dashboard.dialogControlTypes.dxCheckBox,
                    controlOptions: {onValueChanged: function(args) {
                            that.title.setEnabled(args.component.option('value'))
                        }}
                });
                this.title = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.Title),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                });
                this.filterStatePresentation = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.FilterStatePresentation),
                    controlName: dashboard.dialogControlTypes.dxSelectBox,
                    values: [dashboard.filterStatePresentation.none, dashboard.filterStatePresentation.after]
                });
                this.title.setEnabled(showTitle);
                this.resolution = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.Resolution),
                    controlName: dashboard.dialogControlTypes.dxNumberBox
                });
                this.fileName = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.FileName),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                })
            },
            set: function(documentInfo) {
                this.showTitle.set(documentInfo.commonOptions.includeCaption);
                this.title.set(documentInfo.commonOptions.caption);
                var value = documentInfo.commonOptions.filterStatePresentation == dashboard.filterStatePresentation.afterAndSplitPage.value ? dashboard.filterStatePresentation.after.value : documentInfo.commonOptions.filterStatePresentation;
                this.filterStatePresentation.set(value);
                this.imageFormat.set(documentInfo.imageFormatOptions.format);
                this.resolution.set(documentInfo.imageFormatOptions.resolution);
                this.fileName.set(documentInfo.fileName)
            },
            apply: function(documentInfo) {
                documentInfo.commonOptions.includeCaption = this.showTitle.get();
                documentInfo.commonOptions.caption = this.title.get();
                documentInfo.commonOptions.filterStatePresentation = this.filterStatePresentation.get();
                documentInfo.imageFormatOptions.format = this.imageFormat.get();
                documentInfo.imageFormatOptions.resolution = this.resolution.get();
                documentInfo.fileName = this.fileName.get()
            },
            getEditors: function() {
                return [this.fileName, this.showTitle, this.title, this.filterStatePresentation, this.imageFormat, this.resolution]
            }
        });
        dashboard.excelOptionsGroup = dashboard.optionsGroup.inherit({
            ctor: function ctor(format) {
                this._initialize(format)
            },
            _checkExportFormat: function(format) {
                return format === dashboard.excelFormat.csv.value
            },
            _initialize: function(format) {
                var that = this;
                this.excelFormat = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.ExcelFormat),
                    controlName: dashboard.dialogControlTypes.dxSelectBox,
                    values: [dashboard.excelFormat.xlsx, dashboard.excelFormat.xls, dashboard.excelFormat.csv],
                    controlOptions: {onValueChanged: function(args) {
                            that.separator.setEnabled(that._checkExportFormat(args.component.option('value')))
                        }}
                });
                this.separator = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.CsvValueSeparator),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                });
                this.separator.setEnabled(that._checkExportFormat(format));
                this.fileName = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.FileName),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                })
            },
            set: function(documentInfo) {
                this.fileName.set(documentInfo.fileName);
                this.excelFormat.set(documentInfo.excelFormatOptions.format);
                this.separator.set(documentInfo.excelFormatOptions.csvValueSeparator)
            },
            apply: function(documentInfo) {
                documentInfo.fileName = this.fileName.get();
                documentInfo.excelFormatOptions.format = this.excelFormat.get();
                documentInfo.excelFormatOptions.csvValueSeparator = this.separator.get()
            },
            getEditors: function() {
                return [this.fileName, this.excelFormat, this.separator]
            }
        });
        dashboard.simplyImageOptionsGroup = dashboard.optionsGroup.inherit({
            ctor: function ctor(showTitle) {
                this._initialize(showTitle)
            },
            _initialize: function(showTitle) {
                var that = this;
                this.imageFormat = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.ImageFormat),
                    controlName: dashboard.dialogControlTypes.dxSelectBox,
                    values: [dashboard.imageFormat.png, dashboard.imageFormat.gif, dashboard.imageFormat.jpg]
                });
                this.showTitle = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.ShowTitle),
                    controlName: dashboard.dialogControlTypes.dxCheckBox,
                    controlOptions: {onValueChanged: function(args) {
                            that.title.setEnabled(args.component.option('value'))
                        }}
                });
                this.title = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.Title),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                });
                this.title.setEnabled(showTitle);
                this.resolution = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.Resolution),
                    controlName: dashboard.dialogControlTypes.dxNumberBox
                });
                this.fileName = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.FileName),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                })
            },
            set: function(documentInfo) {
                this.showTitle.set(documentInfo.commonOptions.includeCaption);
                this.title.set(documentInfo.commonOptions.caption);
                this.imageFormat.set(documentInfo.imageFormatOptions.format);
                this.resolution.set(documentInfo.imageFormatOptions.resolution);
                this.fileName.set(documentInfo.fileName)
            },
            apply: function(documentInfo) {
                documentInfo.commonOptions.includeCaption = this.showTitle.get();
                documentInfo.commonOptions.caption = this.title.get();
                documentInfo.imageFormatOptions.format = this.imageFormat.get();
                documentInfo.imageFormatOptions.resolution = this.resolution.get();
                documentInfo.fileName = this.fileName.get()
            },
            getEditors: function() {
                return [this.fileName, this.showTitle, this.title, this.imageFormat, this.resolution]
            }
        });
        dashboard.dashboardImageOptionsGroup = dashboard.optionsGroup.inherit({
            ctor: function ctor(showTitle) {
                this._initialize(showTitle)
            },
            _initialize: function(showTitle) {
                var that = this;
                this.imageFormat = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.ImageFormat),
                    controlName: dashboard.dialogControlTypes.dxSelectBox,
                    values: [dashboard.imageFormat.png, dashboard.imageFormat.gif, dashboard.imageFormat.jpg]
                });
                this.showTitle = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.ShowTitle),
                    controlName: dashboard.dialogControlTypes.dxCheckBox,
                    controlOptions: {onValueChanged: function(args) {
                            that.title.setEnabled(args.component.option('value'))
                        }}
                });
                this.title = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.Title),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                });
                this.filterStatePresentation = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.FilterStatePresentation),
                    controlName: dashboard.dialogControlTypes.dxSelectBox,
                    values: [dashboard.filterStatePresentation.none, dashboard.filterStatePresentation.after]
                });
                this.title.setEnabled(showTitle);
                this.resolution = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.Resolution),
                    controlName: dashboard.dialogControlTypes.dxNumberBox
                });
                this.fileName = new dashboard.labeledEditor({
                    labelText: dashboard.data.localizer.getString(dashboard.localizationId.labelName.FileName),
                    controlName: dashboard.dialogControlTypes.dxTextBox
                })
            },
            set: function(documentInfo) {
                this.showTitle.set(documentInfo.showTitle);
                this.title.set(documentInfo.title);
                var value = documentInfo.commonOptions.filterStatePresentation == dashboard.filterStatePresentation.afterAndSplitPage.value ? dashboard.filterStatePresentation.after.value : documentInfo.commonOptions.filterStatePresentation;
                this.filterStatePresentation.set(value);
                this.imageFormat.set(documentInfo.imageFormatOptions.format);
                this.resolution.set(documentInfo.imageFormatOptions.resolution);
                this.fileName.set(documentInfo.fileName)
            },
            apply: function(documentInfo) {
                documentInfo.showTitle = this.showTitle.get();
                documentInfo.title = this.title.get();
                documentInfo.commonOptions.filterStatePresentation = this.filterStatePresentation.get();
                documentInfo.imageFormatOptions.format = this.imageFormat.get();
                documentInfo.imageFormatOptions.resolution = this.resolution.get();
                documentInfo.fileName = this.fileName.get()
            },
            getEditors: function() {
                return [this.fileName, this.showTitle, this.title, this.filterStatePresentation, this.imageFormat, this.resolution]
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file exportOptionsCache.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            Class = DevExpress.require("/class");
        dashboard.optionName = {
            pageLayout: 'pageLayout',
            paperKind: 'paperKind',
            scaleMode: 'scaleMode',
            autoFitPageCount: 'autoFitPageCount',
            scaleFactor: 'scaleFactor',
            fitToPageWidth: 'fitToPageWidth',
            gridPrintHeadersOnEveryPage: 'gridPrintHeadersOnEveryPage',
            pivotPrintHeadersOnEveryPage: 'pivotPrintHeadersOnEveryPage',
            chartSizeMode: 'chartSizeMode',
            chartAutomaticPageLayout: 'chartAutomaticPageLayout',
            mapSizeMode: 'mapSizeMode',
            mapAutomaticPageLayout: 'mapAutomaticPageLayout',
            rangeFilterSizeMode: 'rangeFilterSizeMode',
            rangeFilterAutomaticPageLayout: 'rangeFilterAutomaticPageLayout',
            pieAutoArrangeContent: 'pieAutoArrangeContent',
            gaugeAutoArrangeContent: 'gaugeAutoArrangeContent',
            cardAutoArrangeContent: 'cardAutoArrangeContent',
            imageFormat: 'format',
            excelFormat: 'format',
            csvValueSeparator: 'csvValueSeparator',
            resolution: 'resolution',
            showCaption: 'showCaption',
            caption: 'caption',
            showTitle: 'showTitle',
            title: 'title',
            filterStatePresentation: 'filterStatePresentation',
            fileName: 'fileName'
        };
        dashboard.exportOptionsCache = Class.inherit({
            ctor: function ctor() {
                this._initialize()
            },
            _initialize: function() {
                this.documentOptions = {};
                this.itemsOptions = {};
                this.imageOptions = {};
                this.excelOptions = {}
            },
            _addOption: function(cache, key, defaultValue, actualValue) {
                if (defaultValue == actualValue)
                    delete cache[key];
                else
                    cache[key] = actualValue
            },
            add: function(name, defaultDocumentInfo, actualDocumentInfo) {
                var that = this;
                this._addOption(that.documentOptions, dashboard.optionName.showTitle, defaultDocumentInfo.showTitle, actualDocumentInfo.showTitle);
                this._addOption(that.documentOptions, dashboard.optionName.title, defaultDocumentInfo.title, actualDocumentInfo.title);
                this._addOption(that.documentOptions, dashboard.optionName.pageLayout, defaultDocumentInfo.pageLayout, actualDocumentInfo.pageLayout);
                this._addOption(that.documentOptions, dashboard.optionName.paperKind, defaultDocumentInfo.paperKind, actualDocumentInfo.paperKind);
                this.itemsOptions[name] = {};
                this._addOption(that.itemsOptions[name], dashboard.optionName.scaleMode, defaultDocumentInfo.scaleMode, actualDocumentInfo.scaleMode);
                this._addOption(that.itemsOptions[name], dashboard.optionName.scaleFactor, defaultDocumentInfo.scaleFactor, actualDocumentInfo.scaleFactor);
                this._addOption(that.itemsOptions[name], dashboard.optionName.autoFitPageCount, defaultDocumentInfo.autoFitPageCount, actualDocumentInfo.autoFitPageCount);
                this._addOption(that.itemsOptions[name], dashboard.optionName.fileName, defaultDocumentInfo.fileName, actualDocumentInfo.fileName);
                this._addOption(that.itemsOptions[name], dashboard.optionName.gridPrintHeadersOnEveryPage, defaultDocumentInfo.gridOptions.printHeadersOnEveryPage, actualDocumentInfo.gridOptions.printHeadersOnEveryPage);
                this._addOption(that.itemsOptions[name], dashboard.optionName.fitToPageWidth, defaultDocumentInfo.gridOptions.fitToPageWidth, actualDocumentInfo.gridOptions.fitToPageWidth);
                this._addOption(that.itemsOptions[name], dashboard.optionName.pivotPrintHeadersOnEveryPage, defaultDocumentInfo.pivotOptions.printHeadersOnEveryPage, actualDocumentInfo.pivotOptions.printHeadersOnEveryPage);
                this._addOption(that.itemsOptions[name], dashboard.optionName.chartSizeMode, defaultDocumentInfo.chartOptions.sizeMode, actualDocumentInfo.chartOptions.sizeMode);
                this._addOption(that.itemsOptions[name], dashboard.optionName.chartAutomaticPageLayout, defaultDocumentInfo.chartOptions.automaticPageLayout, actualDocumentInfo.chartOptions.automaticPageLayout);
                this._addOption(that.itemsOptions[name], dashboard.optionName.mapSizeMode, defaultDocumentInfo.mapOptions.sizeMode, actualDocumentInfo.mapOptions.sizeMode);
                this._addOption(that.itemsOptions[name], dashboard.optionName.mapAutomaticPageLayout, defaultDocumentInfo.mapOptions.automaticPageLayout, actualDocumentInfo.mapOptions.automaticPageLayout);
                this._addOption(that.itemsOptions[name], dashboard.optionName.rangeFilterSizeMode, defaultDocumentInfo.rangeFilterOptions.sizeMode, actualDocumentInfo.rangeFilterOptions.sizeMode);
                this._addOption(that.itemsOptions[name], dashboard.optionName.rangeFilterAutomaticPageLayout, defaultDocumentInfo.rangeFilterOptions.automaticPageLayout, actualDocumentInfo.rangeFilterOptions.automaticPageLayout);
                this._addOption(that.itemsOptions[name], dashboard.optionName.pieAutoArrangeContent, defaultDocumentInfo.pieOptions.autoArrangeContent, actualDocumentInfo.pieOptions.autoArrangeContent);
                this._addOption(that.itemsOptions[name], dashboard.optionName.gaugeAutoArrangeContent, defaultDocumentInfo.gaugeOptions.autoArrangeContent, actualDocumentInfo.gaugeOptions.autoArrangeContent);
                this._addOption(that.itemsOptions[name], dashboard.optionName.cardAutoArrangeContent, defaultDocumentInfo.cardOptions.autoArrangeContent, actualDocumentInfo.cardOptions.autoArrangeContent);
                if (name != "") {
                    this._addOption(that.itemsOptions[name], dashboard.optionName.showCaption, defaultDocumentInfo.commonOptions.includeCaption, actualDocumentInfo.commonOptions.includeCaption);
                    this._addOption(that.itemsOptions[name], dashboard.optionName.caption, defaultDocumentInfo.commonOptions.caption, actualDocumentInfo.commonOptions.caption)
                }
                this._addOption(that.documentOptions, dashboard.optionName.filterStatePresentation, defaultDocumentInfo.commonOptions.filterStatePresentation, actualDocumentInfo.commonOptions.filterStatePresentation);
                this._addOption(that.imageOptions, dashboard.optionName.imageFormat, defaultDocumentInfo.imageFormatOptions.format, actualDocumentInfo.imageFormatOptions.format);
                this._addOption(that.imageOptions, dashboard.optionName.resolution, defaultDocumentInfo.imageFormatOptions.resolution, actualDocumentInfo.imageFormatOptions.resolution);
                this._addOption(that.excelOptions, dashboard.optionName.excelFormat, defaultDocumentInfo.excelFormatOptions.format, actualDocumentInfo.excelFormatOptions.format);
                this._addOption(that.excelOptions, dashboard.optionName.csvValueSeparator, defaultDocumentInfo.excelFormatOptions.csvValueSeparator, actualDocumentInfo.excelFormatOptions.csvValueSeparator)
            },
            _setActualValue: function(cache, key, setActual, defaultValue) {
                var value = cache[key] != undefined ? cache[key] : defaultValue;
                setActual(value)
            },
            getActualDocumentInfo: function(name, defaultDocumentInfo) {
                var that = this;
                var actualDocumentInfo = {
                        commonOptions: {},
                        imageFormatOptions: {},
                        excelFormatOptions: {},
                        pivotOptions: {},
                        gridOptions: {},
                        chartOptions: {},
                        pieOptions: {},
                        gaugeOptions: {},
                        cardOptions: {},
                        mapOptions: {},
                        rangeFilterOptions: {},
                        imageOptions: {}
                    };
                this._setActualValue(that.documentOptions, dashboard.optionName.showTitle, function(actual) {
                    actualDocumentInfo.showTitle = actual
                }, defaultDocumentInfo.showTitle);
                this._setActualValue(that.documentOptions, dashboard.optionName.title, function(actual) {
                    actualDocumentInfo.title = actual
                }, defaultDocumentInfo.title);
                this._setActualValue(that.documentOptions, dashboard.optionName.paperKind, function(actual) {
                    actualDocumentInfo.paperKind = actual
                }, defaultDocumentInfo.paperKind);
                this._setActualValue(that.documentOptions, dashboard.optionName.pageLayout, function(actual) {
                    actualDocumentInfo.pageLayout = actual
                }, defaultDocumentInfo.pageLayout);
                if (this.itemsOptions[name] === undefined)
                    this.itemsOptions[name] = {};
                this._setActualValue(that.itemsOptions[name], dashboard.optionName.scaleMode, function(actual) {
                    actualDocumentInfo.scaleMode = actual
                }, defaultDocumentInfo.scaleMode);
                this._setActualValue(that.itemsOptions[name], dashboard.optionName.scaleFactor, function(actual) {
                    actualDocumentInfo.scaleFactor = actual
                }, defaultDocumentInfo.scaleFactor);
                this._setActualValue(that.itemsOptions[name], dashboard.optionName.autoFitPageCount, function(actual) {
                    actualDocumentInfo.autoFitPageCount = actual
                }, defaultDocumentInfo.autoFitPageCount);
                this._setActualValue(that.itemsOptions[name], dashboard.optionName.fileName, function(actual) {
                    actualDocumentInfo.fileName = actual
                }, defaultDocumentInfo.fileName);
                this._setActualValue(that.itemsOptions[name], dashboard.optionName.gridPrintHeadersOnEveryPage, function(actual) {
                    actualDocumentInfo.gridOptions.printHeadersOnEveryPage = actual
                }, defaultDocumentInfo.gridOptions.printHeadersOnEveryPage);
                this._setActualValue(that.itemsOptions[name], dashboard.optionName.fitToPageWidth, function(actual) {
                    actualDocumentInfo.gridOptions.fitToPageWidth = actual
                }, defaultDocumentInfo.gridOptions.fitToPageWidth);
                this._setActualValue(that.itemsOptions[name], dashboard.optionName.pivotPrintHeadersOnEveryPage, function(actual) {
                    actualDocumentInfo.pivotOptions.printHeadersOnEveryPage = actual
                }, defaultDocumentInfo.pivotOptions.printHeadersOnEveryPage);
                this._setActualValue(that.itemsOptions[name], dashboard.optionName.chartSizeMode, function(actual) {
                    actualDocumentInfo.chartOptions.sizeMode = actual
                }, defaultDocumentInfo.chartOptions.sizeMode);
                this._setActualValue(that.itemsOptions[name], dashboard.optionName.chartAutomaticPageLayout, function(actual) {
                    actualDocumentInfo.chartOptions.automaticPageLayout = actual
                }, defaultDocumentInfo.chartOptions.automaticPageLayout);
                this._setActualValue(that.itemsOptions[name], dashboard.optionName.mapSizeMode, function(actual) {
                    actualDocumentInfo.mapOptions.sizeMode = actual
                }, defaultDocumentInfo.mapOptions.sizeMode);
                this._setActualValue(that.itemsOptions[name], dashboard.optionName.mapAutomaticPageLayout, function(actual) {
                    actualDocumentInfo.mapOptions.automaticPageLayout = actual
                }, defaultDocumentInfo.mapOptions.automaticPageLayout);
                this._setActualValue(that.itemsOptions[name], dashboard.optionName.rangeFilterSizeMode, function(actual) {
                    actualDocumentInfo.rangeFilterOptions.sizeMode = actual
                }, defaultDocumentInfo.rangeFilterOptions.sizeMode);
                this._setActualValue(that.itemsOptions[name], dashboard.optionName.rangeFilterAutomaticPageLayout, function(actual) {
                    actualDocumentInfo.rangeFilterOptions.automaticPageLayout = actual
                }, defaultDocumentInfo.rangeFilterOptions.automaticPageLayout);
                this._setActualValue(that.itemsOptions[name], dashboard.optionName.pieAutoArrangeContent, function(actual) {
                    actualDocumentInfo.pieOptions.autoArrangeContent = actual
                }, defaultDocumentInfo.pieOptions.autoArrangeContent);
                this._setActualValue(that.itemsOptions[name], dashboard.optionName.gaugeAutoArrangeContent, function(actual) {
                    actualDocumentInfo.gaugeOptions.autoArrangeContent = actual
                }, defaultDocumentInfo.gaugeOptions.autoArrangeContent);
                this._setActualValue(that.itemsOptions[name], dashboard.optionName.cardAutoArrangeContent, function(actual) {
                    actualDocumentInfo.cardOptions.autoArrangeContent = actual
                }, defaultDocumentInfo.cardOptions.autoArrangeContent);
                if (name != "") {
                    this._setActualValue(that.itemsOptions[name], dashboard.optionName.showCaption, function(actual) {
                        actualDocumentInfo.commonOptions.includeCaption = actual
                    }, defaultDocumentInfo.commonOptions.includeCaption);
                    this._setActualValue(that.itemsOptions[name], dashboard.optionName.caption, function(actual) {
                        actualDocumentInfo.commonOptions.caption = actual
                    }, defaultDocumentInfo.commonOptions.caption)
                }
                this._setActualValue(that.documentOptions, dashboard.optionName.filterStatePresentation, function(actual) {
                    actualDocumentInfo.commonOptions.filterStatePresentation = actual
                }, defaultDocumentInfo.commonOptions.filterStatePresentation);
                this._setActualValue(that.imageOptions, dashboard.optionName.imageFormat, function(actual) {
                    actualDocumentInfo.imageFormatOptions.format = actual
                }, defaultDocumentInfo.imageFormatOptions.format);
                this._setActualValue(that.imageOptions, dashboard.optionName.resolution, function(actual) {
                    actualDocumentInfo.imageFormatOptions.resolution = actual
                }, defaultDocumentInfo.imageFormatOptions.resolution);
                this._setActualValue(that.excelOptions, dashboard.optionName.excelFormat, function(actual) {
                    actualDocumentInfo.excelFormatOptions.format = actual
                }, defaultDocumentInfo.excelFormatOptions.format);
                this._setActualValue(that.excelOptions, dashboard.optionName.csvValueSeparator, function(actual) {
                    actualDocumentInfo.excelFormatOptions.csvValueSeparator = actual
                }, defaultDocumentInfo.excelFormatOptions.csvValueSeparator);
                return actualDocumentInfo
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file exportDialog.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            stringUtils = DX.require("/utils/utils.string"),
            browser = DX.require("/utils/utils.browser"),
            Class = DevExpress.require("/class"),
            viewerItemTypes = dashboard.viewerItems.types;
        dashboard.dialogClasses.exportForm = 'dx-dashboard-export-form';
        dashboard.exportDialog = Class.inherit({
            ctor: function ctor(options) {
                this.options = options;
                this._initialize()
            },
            _initialize: function() {
                var localizer = dashboard.data.localizer,
                    that = this,
                    options = that.options,
                    $exportForm = $('<div/>', {'class': dashboard.dialogClasses.form + ' ' + dashboard.dialogClasses.exportForm});
                dashboard.paperKind = {
                    letter: {
                        displayValue: localizer.getString(dashboard.localizationId.labelName.PaperKindLetter),
                        value: 'Letter'
                    },
                    legal: {
                        displayValue: localizer.getString(dashboard.localizationId.labelName.PaperKindLegal),
                        value: 'Legal'
                    },
                    executive: {
                        displayValue: localizer.getString(dashboard.localizationId.labelName.PaperKindExecutive),
                        value: 'Executive'
                    },
                    a5: {
                        displayValue: localizer.getString(dashboard.localizationId.labelName.PaperKindA5),
                        value: 'A5'
                    },
                    a4: {
                        displayValue: localizer.getString(dashboard.localizationId.labelName.PaperKindA4),
                        value: 'A4'
                    },
                    a3: {
                        displayValue: localizer.getString(dashboard.localizationId.labelName.PaperKindA3),
                        value: 'A3'
                    }
                };
                dashboard.pageLayout = {
                    auto: {
                        displayValue: localizer.getString(dashboard.localizationId.labelName.PageLayoutAuto),
                        value: 'Auto'
                    },
                    portrait: {
                        displayValue: localizer.getString(dashboard.localizationId.labelName.PageLayoutPortrait),
                        value: 'Portrait'
                    },
                    landscape: {
                        displayValue: localizer.getString(dashboard.localizationId.labelName.PageLayoutLandscape),
                        value: 'Landscape'
                    }
                };
                dashboard.scaleMode = {
                    none: {
                        displayValue: localizer.getString(dashboard.localizationId.labelName.ScaleModeNone),
                        value: 'None'
                    },
                    useScaleFactor: {
                        displayValue: localizer.getString(dashboard.localizationId.labelName.ScaleModeUseScaleFactor),
                        value: 'UseScaleFactor'
                    },
                    autoFitToPageWidth: {
                        displayValue: localizer.getString(dashboard.localizationId.labelName.ScaleModeAutoFitToPageWidth),
                        value: 'AutoFitToPageWidth'
                    },
                    autoFitWithinOnePage: {value: 'AutoFitWithinOnePage'}
                };
                dashboard.filterStatePresentation = {
                    none: {
                        displayValue: localizer.getString(dashboard.localizationId.labelName.FilterStatePresentationNone),
                        value: 'None'
                    },
                    after: {
                        displayValue: localizer.getString(dashboard.localizationId.labelName.FilterStatePresentationAfter),
                        value: 'After'
                    },
                    afterAndSplitPage: {
                        displayValue: localizer.getString(dashboard.localizationId.labelName.FilterStatePresentationAfterAndSplitPage),
                        value: 'AfterAndSplitPage'
                    }
                };
                dashboard.imageFormat = {
                    png: {
                        displayValue: "PNG",
                        value: 'Png'
                    },
                    gif: {
                        displayValue: "GIF",
                        value: 'Gif'
                    },
                    jpg: {
                        displayValue: "JPG",
                        value: 'Jpg'
                    }
                };
                dashboard.excelFormat = {
                    csv: {
                        displayValue: "CSV",
                        value: 'Csv'
                    },
                    xls: {
                        displayValue: "XLS",
                        value: 'Xls'
                    },
                    xlsx: {
                        displayValue: "XLSX",
                        value: 'Xlsx'
                    }
                };
                dashboard.sizeMode = {
                    none: {
                        displayValue: localizer.getString(dashboard.localizationId.labelName.SizeModeNone),
                        value: 'None'
                    },
                    stretch: {
                        displayValue: localizer.getString(dashboard.localizationId.labelName.SizeModeStretch),
                        value: 'Stretch'
                    },
                    zoom: {
                        displayValue: localizer.getString(dashboard.localizationId.labelName.SizeModeZoom),
                        value: 'Zoom'
                    }
                };
                that.$exportForm = $exportForm;
                that.exportOptionsCache = new dashboard.exportOptionsCache;
                that.dialogForm = new dashboard.dialogForm({
                    $dialogContainer: options.$container,
                    width: 'auto',
                    height: 'auto',
                    $content: $exportForm,
                    buttons: [{
                            name: localizer.getString(dashboard.localizationId.buttonNames.ButtonReset),
                            func: function(type, typeExportEntities) {
                                that.group.set(options.documentInfo)
                            },
                            hide: false
                        }, {
                            name: localizer.getString(dashboard.localizationId.buttonNames.ButtonExport),
                            hide: true,
                            func: function() {
                                var actualDocumentInfo = that.exportOptionsCache.getActualDocumentInfo(that.name, options.documentInfo);
                                that.group.apply(actualDocumentInfo);
                                that.exportOptionsCache.add(that.name, options.documentInfo, actualDocumentInfo);
                                that.exportFunction(actualDocumentInfo)
                            }
                        }, {
                            name: localizer.getString(dashboard.localizationId.buttonNames.ButtonCancel),
                            hide: true,
                            func: function(){}
                        }],
                    renderControls: function(controlCreationCallbacks){},
                    setActualState: function(width){}
                })
            },
            showDialog: function(name, type, format, options) {
                var that = this;
                if (type != null)
                    that.options.documentInfo.commonOptions.caption = options.caption;
                that.options.documentInfo.fileName = options.fileName;
                that.name = name;
                that.type = type;
                that.format = format;
                that.$exportForm.empty();
                that._renderControls(that.$exportForm);
                that.dialogForm.popupInstance.option('title', that._getTitle(options.fileName, format));
                that.dialogForm.showDialog()
            },
            _getTitle: function(name, format) {
                var localizer = dashboard.data.localizer,
                    isIE8orIE9 = browser.msie && (browser.version == "8.0" || browser.version == "9.0"),
                    exportString;
                switch (format) {
                    case dashboard.exportFormats.pdf:
                        exportString = localizer.getString(dashboard.localizationId.buttonNames.ExportToPdf);
                        break;
                    case dashboard.exportFormats.image:
                        exportString = localizer.getString(dashboard.localizationId.buttonNames.ExportToImage);
                        break;
                    default:
                        exportString = localizer.getString(dashboard.localizationId.buttonNames.ExportToExcel);
                        break
                }
                return isIE8orIE9 ? exportString : stringUtils.format(localizer.getString(dashboard.localizationId.buttonNames.ExportTemplate), exportString, name)
            },
            setExportFunction: function(exportFunction) {
                if (exportFunction)
                    this.exportFunction = exportFunction
            },
            _createImageGroup: function(type, documentInfo) {
                switch (type) {
                    case null:
                        return new dashboard.dashboardImageOptionsGroup(documentInfo.showTitle);
                    case viewerItemTypes.image:
                    case viewerItemTypes.text:
                        return new dashboard.simplyImageOptionsGroup(documentInfo.commonOptions.includeCaption);
                    default:
                        return new dashboard.imageOptionsGroup(documentInfo.commonOptions.includeCaption)
                }
            },
            _createPdfGroup: function(type, documentInfo) {
                switch (type) {
                    case null:
                    case viewerItemTypes.group:
                        return new dashboard.dashboardOptionsGroup(documentInfo.showTitle, documentInfo.scaleMode);
                    case viewerItemTypes.grid:
                        return new dashboard.gridOptionsGroup(documentInfo.commonOptions.includeCaption, documentInfo.gridOptions.fitToPageWidth, documentInfo.scaleMode);
                    case viewerItemTypes.chart:
                    case viewerItemTypes.scatter:
                        return new dashboard.chartOptionsGroup(documentInfo.commonOptions.includeCaption);
                    case viewerItemTypes.pie:
                        return new dashboard.pieOptionsGroup(documentInfo.commonOptions.includeCaption, documentInfo.pieOptions.autoArrangeContent, documentInfo.scaleMode);
                    case viewerItemTypes.gauge:
                        return new dashboard.gaugeOptionsGroup(documentInfo.commonOptions.includeCaption, documentInfo.gaugeOptions.autoArrangeContent, documentInfo.scaleMode);
                    case viewerItemTypes.card:
                        return new dashboard.cardOptionsGroup(documentInfo.commonOptions.includeCaption, documentInfo.cardOptions.autoArrangeContent, documentInfo.scaleMode);
                    case viewerItemTypes.pivot:
                        return new dashboard.pivotOptionsGroup(documentInfo.commonOptions.includeCaption, documentInfo.scaleMode);
                    case viewerItemTypes.choroplethMap:
                    case viewerItemTypes.geoPointMap:
                    case viewerItemTypes.bubbleMap:
                    case viewerItemTypes.pieMap:
                        return new dashboard.mapOptionsGroup(documentInfo.commonOptions.includeCaption);
                    case viewerItemTypes.rangeFilter:
                        return new dashboard.rangeFilterOptionsGroup(documentInfo.commonOptions.includeCaption);
                    case viewerItemTypes.image:
                    case viewerItemTypes.text:
                        return new dashboard.simplyDocumentOptionsWithScaleModeGroup(documentInfo.commonOptions.includeCaption, documentInfo.scaleMode)
                }
            },
            _createGroup: function() {
                var that = this,
                    documentInfo = that.exportOptionsCache.getActualDocumentInfo(that.name, that.options.documentInfo);
                switch (that.format) {
                    case dashboard.exportFormats.image:
                        that.group = that._createImageGroup(that.type, documentInfo);
                        break;
                    case dashboard.exportFormats.excel:
                        that.group = new dashboard.excelOptionsGroup(documentInfo.excelFormatOptions.format);
                        break;
                    default:
                        that.group = that._createPdfGroup(that.type, documentInfo);
                        break
                }
                that.group.set(documentInfo)
            },
            _renderControls: function($div) {
                var that = this;
                that._createGroup();
                var editors = that.group.getEditors();
                $.each(editors, function(index, editor) {
                    var $container = $('<div/>', {'class': dashboard.dialogClasses.element}).appendTo($div);
                    editor.$label.appendTo($container);
                    editor.$editor.appendTo($container)
                })
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file utils.layout.js */
    (function($, DX, undefined) {
        var Class = DX.require("/class"),
            dashboard = DX.dashboard,
            debug = DX.require("/utils/utils.console").debug;
        dashboard.layout = dashboard.layout || {};
        dashboard.layout.utils = {
            size: function(w, h) {
                return {
                        width: w,
                        height: h,
                        plus: function(arg) {
                            var correctSize = function(value, addition) {
                                    return Number.MAX_VALUE - value >= addition ? value + addition : Number.MAX_VALUE
                                };
                            return new dashboard.layout.utils.size(correctSize(this.width, arg.width), correctSize(this.height, arg.height))
                        },
                        minus: function(arg) {
                            return new dashboard.layout.utils.size(this.width - arg.width, this.height - arg.height)
                        },
                        compareByDirections: function(size) {
                            if (!size)
                                return ['width', 'height'];
                            else {
                                var differentDirections = [];
                                if (size.width != this.width)
                                    differentDirections.push('width');
                                if (size.height != this.height)
                                    differentDirections.push('height');
                                return differentDirections
                            }
                        },
                        constrain: function(constraints) {
                            var that = this,
                                ensureDirection = function(direction) {
                                    return dashboard.layout.utils.ensureRange(that[direction], constraints.min[direction], constraints.max[direction])
                                };
                            return new dashboard.layout.utils.size(ensureDirection('width'), ensureDirection('height'))
                        },
                        clone: function() {
                            return new dashboard.layout.utils.size(this.width, this.height)
                        }
                    }
            },
            constraints: function(pMin, pMax) {
                return {
                        min: pMin,
                        max: pMax,
                        consolidate: function(sourceConstraints, consolidateDirection) {
                            return new dashboard.layout.utils.constraints(this._consolidatePart(sourceConstraints, consolidateDirection, 'min'), this._consolidatePart(sourceConstraints, consolidateDirection, 'max'))
                        },
                        isFixed: function(direction) {
                            if (direction) {
                                var differentDirections = this.min.compareByDirections(this.max);
                                return $.inArray(direction, differentDirections) < 0
                            }
                            else
                                return false
                        },
                        _consolidatePart: function(sourceConstraints, consolidateDirection, part) {
                            var that = this,
                                size = new dashboard.layout.utils.size,
                                direction = consolidateDirection ? consolidateDirection : 'width',
                                crossDirection = dashboard.layout.utils.getCrossDirection(direction),
                                consolidateSumFunc = function(currentDirection) {
                                    var val1 = that[part][currentDirection],
                                        val2 = sourceConstraints[part][currentDirection];
                                    return val1 === Number.MAX_VALUE || val2 === Number.MAX_VALUE ? Number.MAX_VALUE : val1 + val2
                                },
                                consolidateMaxMinFunc = function(currentDirection, isCross) {
                                    var val1 = that[part][currentDirection],
                                        val2 = sourceConstraints[part][currentDirection];
                                    return part === 'min' || isCross ? Math.max(val1, val2) : Math.min(val1, val2)
                                };
                            size[direction] = consolidateDirection ? consolidateSumFunc(direction) : consolidateMaxMinFunc(direction, false);
                            size[crossDirection] = consolidateMaxMinFunc(crossDirection, !!consolidateDirection);
                            return size
                        }
                    }
            },
            nonClientElement: function(width, height) {
                var size = new dashboard.layout.utils.size(width, height);
                return {getBounds: function() {
                            return size.clone()
                        }}
            },
            getCrossDirection: function(direction) {
                return direction === 'width' ? 'height' : 'width'
            },
            defConstraints: function(valueMin, valueMax) {
                var paramValueMin = valueMin === undefined ? 0 : valueMin,
                    paramValueMax = valueMax === undefined ? Number.MAX_VALUE : valueMax;
                return new this.constraints(new this.size(paramValueMin, paramValueMin), new this.size(paramValueMax, paramValueMax))
            },
            defSizeInPercents: function(direction, value) {
                var size = new this.size(1, 1);
                size[direction] = value;
                return size
            },
            checkRange: function(value, min, max) {
                return min <= value && value <= max
            },
            ensureRange: function(value, min, max) {
                return Math.max(Math.min(value, max), min)
            },
            deepCloneObject: function(injectObject, sourceObject, noDeepCopyPropsValues) {
                var copyObj = {};
                $.extend(copyObj, sourceObject);
                for (var prop in noDeepCopyPropsValues)
                    delete copyObj[prop];
                $.extend(true, injectObject, copyObj);
                $.extend(injectObject, noDeepCopyPropsValues);
                return injectObject
            },
            getElementProxy: function($element, defaultMinSize) {
                return {
                        $element: $element,
                        _allowResizing: true,
                        _constraints: undefined,
                        _itemRendered: false,
                        getConstraints: function() {
                            var item = this.getItem();
                            if (item && item.getConstraints) {
                                if (!this._constraints)
                                    this._constraints = item.getConstraints(true);
                                return this._constraints
                            }
                            else
                                return dashboard.layout.utils.defConstraints(defaultMinSize ? defaultMinSize : 0)
                        },
                        getSize: function() {
                            return new dashboard.layout.utils.size($element.outerWidth(), $element.outerHeight())
                        },
                        getItem: function() {
                            return $element.data('resizableViewerElement')
                        },
                        applySize: function(size) {
                            if (this._allowResizing) {
                                var item = this.getItem(),
                                    capitalize = function(str) {
                                        return str.charAt(0).toUpperCase() + str.substr(1)
                                    },
                                    ensureSize = function($resizeElement, direction) {
                                        var funcName = 'outer' + capitalize(direction);
                                        if ($resizeElement[funcName])
                                            $resizeElement[funcName](size[direction])
                                    };
                                if (!item || !this._itemRendered) {
                                    ensureSize($element, 'width');
                                    ensureSize($element, 'height')
                                }
                                if (item)
                                    if (!this._itemRendered && item.render) {
                                        item.render(this.$element);
                                        this._itemRendered = true
                                    }
                                    else if (item.setSize)
                                        item.setSize(size.width, size.height)
                            }
                        }
                    }
            }
        }
    })(jQuery, DevExpress);
    /*! Module dashboard, file customSection.js */
    (function($, DX, undefined) {
        var Class = DX.require("/class"),
            dashboard = DX.dashboard,
            debug = DX.require("/utils/utils.console").debug,
            layout = dashboard.layout;
        layout.cssClassNames = {
            layoutShield: 'dx-dashboard-layout-shield',
            splitterPane: 'dx-dashboard-splitter-pane',
            splitterPaneEmpty: 'dx-dashboard-splitter-pane-empty',
            splitterResizing: 'dx-dashboard-splitter-resizing',
            splitterLayoutPlace: 'dx-dashboard-splitter-layout-place',
            splitterSeparator: 'dx-dashboard-splitter-separator',
            splitterSeparatorH: 'dx-dashboard-splitter-h-separator',
            splitterSeparatorV: 'dx-dashboard-splitter-v-separator',
            movingSeparatorV: 'dx-dashboard-splitter-moving-v-separator',
            movingSeparatorH: 'dx-dashboard-splitter-moving-h-separator'
        };
        layout.ItemType = {
            Group: 'Group',
            Item: 'Item'
        };
        layout.viewerSection = Class.inherit({
            ctor: function ctor(options) {
                this.options = layout.utils.deepCloneObject({}, options, {})
            },
            render: function() {
                var that = this,
                    resultTable,
                    rootLayoutItem = that.options.layoutItem,
                    $rootContainer = that.options.$container;
                that._renderItem(rootLayoutItem, $rootContainer);
                resultTable = rootLayoutItem.tableConstructor ? rootLayoutItem.tableConstructor.$table : null;
                if (resultTable && that.options.useResizer !== false) {
                    that.viewerResizer = new dashboard.layout.viewerResizer;
                    that.viewerResizer.attachResizeHandlers($rootContainer, resultTable)
                }
                return resultTable
            },
            updateSize: function(parentWidth, parentHeight) {
                return this.options.layoutItem.paneTreeItem.setSizeInPixels(new layout.utils.size(parentWidth ? parentWidth : this.width(), parentHeight ? parentHeight : this.height()))
            },
            width: function(width) {
                if (width)
                    this.updateSize(width, null);
                return this.options.layoutItem.paneTreeItem.sizeInPixels.width
            },
            height: function(height) {
                if (height)
                    this.updateSize(null, height);
                return this.options.layoutItem.paneTreeItem.sizeInPixels.height
            },
            _renderItem: function(rootItem, $rootContainer) {
                var that = this,
                    walker = dashboard.utils.treeWalker(rootItem, function(node) {
                        return that._getPanes(node)
                    });
                $rootContainer.empty();
                rootItem.tableConstructor = undefined;
                walker.walk(function(item, parentItem) {
                    item.viewerItem = that.options.items[that._getName(item)];
                    item.separatorSize = that.options.separatorSize;
                    item.direction = that._isOrientationByHeight(item) ? 'height' : 'width';
                    item.$DOMElement = parentItem ? $('<div/>', {'class': layout.cssClassNames.splitterLayoutPlace}) : $rootContainer;
                    item.$DOMElement.data('resizableViewerElement', item.viewerItem);
                    item.paneTreeItem = new layout.utils.paneTreeItem(parentItem ? parentItem.paneTreeItem : null, item.direction, layout.utils.getElementProxy(item.$DOMElement, that.options.minPaneSize), that._getSizeInPercents(item, parentItem));
                    if (parentItem) {
                        if (parentItem.paneTreeItem)
                            parentItem.paneTreeItem.panes.push(item.paneTreeItem);
                        if (!parentItem.tableConstructor)
                            parentItem.tableConstructor = layout.tableConstructor(parentItem.$DOMElement, parentItem.direction);
                        var classCell = that._isPaneEmpty(item) ? layout.cssClassNames.splitterPaneEmpty : layout.cssClassNames.splitterPane,
                            $cell = parentItem.tableConstructor.getNextCell();
                        $cell.append(item.$DOMElement);
                        if (item.viewerItem) {
                            $cell.attr('class', classCell);
                            var borderSize = dashboard.utils.getBorderSizeByClasses([classCell]),
                                offset = item.viewerItem.getOffset ? item.viewerItem.getOffset() : {
                                    width: 0,
                                    height: 0
                                };
                            item.paneTreeItem.nonClientOuterElements.push(new layout.utils.nonClientElement(borderSize.width, borderSize.height));
                            item.paneTreeItem.nonClientInnerElements.push(new layout.utils.nonClientElement(offset.width, offset.height))
                        }
                        if (item !== that._getLastPane(parentItem)) {
                            var isDirectionWidth = parentItem.direction == 'width',
                                splitterSize = isDirectionWidth ? {width: parentItem.separatorSize} : {height: parentItem.separatorSize},
                                optionsCell = {
                                    'class': isDirectionWidth ? layout.cssClassNames.splitterSeparatorV : layout.cssClassNames.splitterSeparatorH,
                                    align: 'center'
                                },
                                optionsDiv = {'class': layout.cssClassNames.splitterSeparator},
                                $separatorCell;
                            $.extend(optionsCell, splitterSize);
                            $.extend(optionsDiv, splitterSize);
                            $separatorCell = parentItem.tableConstructor.getNextCell(optionsCell);
                            $separatorCell.append($('<div/>', optionsDiv));
                            parentItem.paneTreeItem.nonClientInnerElements.push(new layout.utils.nonClientElement(isDirectionWidth ? $separatorCell.width() : 0, isDirectionWidth ? 0 : $separatorCell.height()));
                            $separatorCell.data('resizableElementsAccessor', new dashboard.layout.resizableAccessor(parentItem, item.paneTreeItem))
                        }
                    }
                });
                rootItem.paneTreeItem.setSizeInPixels(rootItem.paneTreeItem.elementProxy.getSize())
            },
            _isOrientationByHeight: function(layoutItem) {
                return layoutItem.Orientation === 'Vertical'
            },
            _isPaneEmpty: function(layoutItem) {
                var viewerItem = layoutItem.viewerItem;
                return viewerItem && viewerItem.isPaneEmpty ? viewerItem.isPaneEmpty() : false
            },
            _getName: function(layoutItem) {
                return layoutItem.Name
            },
            _getLastPane: function(parentLayoutItem) {
                var parentPanes = this._getPanes(parentLayoutItem);
                return parentPanes.length > 0 ? parentPanes[parentPanes.length - 1] : undefined
            },
            _getPanes: function(layoutItem) {
                return layoutItem && layoutItem.Panes ? layoutItem.Panes : []
            },
            _getSizeInPercents: function(layoutItem, parentLayoutItem) {
                var that = this,
                    items = that._getPanes(parentLayoutItem),
                    sizesSum = 0;
                $.each(items, function(i, layoutSubItem) {
                    sizesSum += layoutSubItem.Size
                });
                return layout.utils.defSizeInPercents(parentLayoutItem ? parentLayoutItem.direction : 'width', layoutItem.Size / (sizesSum == 0 ? 1 : sizesSum))
            }
        });
        layout.resizableAccessor = function(parentLayoutItem, currentPaneTreeItem) {
            return {
                    direction: parentLayoutItem.direction,
                    _previousPane: undefined,
                    _nextPane: undefined,
                    getPrevious: function() {
                        if (!this._previousPane) {
                            var prev = currentPaneTreeItem.getNearestResizable(this.direction, -1);
                            this._previousPane = prev ? prev : currentPaneTreeItem
                        }
                        return this._previousPane
                    },
                    getNext: function() {
                        if (!this._nextPane) {
                            var next = currentPaneTreeItem.getNearestResizable(this.direction, +1);
                            this._nextPane = next ? next : currentPaneTreeItem.getNextPane()
                        }
                        return this._nextPane
                    },
                    $prev: function() {
                        return this.getPrevious().elementProxy.$element.parent()
                    },
                    $next: function() {
                        return this.getNext().elementProxy.$element.parent()
                    }
                }
        }
    })(jQuery, DevExpress);
    /*! Module dashboard, file paneTreeItem.js */
    (function($, DX, undefined) {
        var Class = DX.require("/class"),
            dashboard = DX.dashboard,
            layout = dashboard.layout,
            debug = DX.require("/utils/utils.console").debug;
        layout.utils.paneTreeItem = function(parent, direction, elementProxy, sizeInPercents) {
            return {
                    elementProxy: elementProxy,
                    parent: parent,
                    panes: [],
                    nonClientOuterElements: [],
                    nonClientInnerElements: [],
                    sizeInPercents: sizeInPercents ? sizeInPercents : layout.utils.defSizeInPercents(direction, 1),
                    sizeInPixels: new layout.utils.size(0, 0),
                    constraints: layout.utils.defConstraints(),
                    _childDirection: direction,
                    _updateCount: 0,
                    distributeSpace: function(direction, freeSpace, panesData) {
                        var that = this,
                            fullSizeInPercents = 0,
                            currFreeSpace = freeSpace,
                            firstConstrainedPane = undefined,
                            constrainedPanesByMin = [],
                            constrainedPanesByMax = [];
                        $.each(panesData, function(_, paneData) {
                            fullSizeInPercents += paneData.sizeInPercents[direction]
                        });
                        if (fullSizeInPercents == 0)
                            fullSizeInPercents = 1;
                        $.each(panesData, function(i, paneData) {
                            var isLastPane = i === panesData.length - 1,
                                size = isLastPane ? currFreeSpace : Math.floor(paneData.sizeInPercents[direction] * freeSpace / fullSizeInPercents);
                            if (size < paneData.constraints.min[direction]) {
                                paneData.sizeInPixels = paneData.constraints.min[direction];
                                paneData.deltaSizeInPixels = paneData.sizeInPixels - size;
                                constrainedPanesByMin.push(paneData)
                            }
                            else if (size > paneData.constraints.max[direction]) {
                                paneData.sizeInPixels = paneData.constraints.max[direction];
                                paneData.deltaSizeInPixels = size - paneData.sizeInPixels;
                                constrainedPanesByMax.push(paneData)
                            }
                            else
                                paneData.sizeInPixels = size;
                            currFreeSpace -= paneData.sizeInPixels
                        });
                        constrainedPanesByMax.sort(function(dataA, dataB) {
                            return dataB.deltaSizeInPixels - dataA.deltaSizeInPixels
                        });
                        constrainedPanesByMin.sort(function(dataA, dataB) {
                            return dataB.deltaSizeInPixels - dataA.deltaSizeInPixels
                        });
                        firstConstrainedPane = constrainedPanesByMax.length > 0 ? constrainedPanesByMax[0] : constrainedPanesByMin.length > 0 ? constrainedPanesByMin[0] : undefined;
                        if (firstConstrainedPane && panesData.length > 1) {
                            var nextPassPanesData = [],
                                nextPassResult = [];
                            $.each(panesData, function(i, paneDataInternal) {
                                if (paneDataInternal !== firstConstrainedPane)
                                    nextPassPanesData.push(paneDataInternal)
                            });
                            nextPassResult = that.distributeSpace(direction, freeSpace - firstConstrainedPane.sizeInPixels, nextPassPanesData);
                            return $.merge([firstConstrainedPane], nextPassResult)
                        }
                        else
                            return panesData
                    },
                    getClientSize: function() {
                        return this.sizeInPixels.minus(this._getNonClientOuterElementsSize())
                    },
                    getChildrenSize: function() {
                        return this.getClientSize().minus(this._getNonClientInnerElementsSize())
                    },
                    getNextPane: function() {
                        var that = this,
                            thatIndex = $.inArray(that, that.parent.panes),
                            nextIndex = thatIndex + 1;
                        if (nextIndex >= 0 && nextIndex < that.parent.panes.length)
                            return that.parent.panes[nextIndex];
                        else
                            return null
                    },
                    getNearestResizable: function(direction, order) {
                        return this._getNearestResizableRecursive(this, direction, order)
                    },
                    _getNearestResizableRecursive: function(pane, direction, order) {
                        var parent = pane.parent;
                        if (parent && parent._childDirection == direction) {
                            var panes = parent.panes,
                                thatIndex = $.inArray(pane, panes),
                                i,
                                isFixedSize = function(p) {
                                    var constraints = p.getActualConstraints();
                                    return constraints.isFixed(direction)
                                };
                            if (order >= 0) {
                                for (i = thatIndex + 1; i < panes.length; i++)
                                    if (!isFixedSize(panes[i]))
                                        return panes[i]
                            }
                            else
                                for (i = thatIndex; i >= 0; i--)
                                    if (!isFixedSize(panes[i]))
                                        return panes[i];
                            return this._getNearestResizableRecursive(parent, direction, order)
                        }
                        else
                            return undefined
                    },
                    setSizeInPixels: function(newSize) {
                        debug.assert(!isNaN(newSize.width), "newSize.width must be correct value");
                        debug.assert(!isNaN(newSize.height), "newSize.height must be correct value");
                        var constrainedSize = newSize.constrain(this.getActualConstraints());
                        var directions = constrainedSize.compareByDirections(this.sizeInPixels);
                        var isChanged = directions.length > 0;
                        if (isChanged) {
                            this.sizeInPixels = constrainedSize;
                            this.rearrangeChildren(directions)
                        }
                        return isChanged
                    },
                    rearrangeChildren: function(directions) {
                        var that = this;
                        if (!directions)
                            directions = ['width', 'height'];
                        debug.assert(directions.length <= 2, "rearrangeChildren can rearrange by one or two directions");
                        that._beginUpdate();
                        $.each(directions, function(_, direction) {
                            that._arrangeChildren(direction)
                        });
                        that._endUpdate()
                    },
                    _arrangeChildren: function(direction) {
                        debug.assert($.inArray(this._childDirection, ['width', 'height']) != -1, "this._childDirection must be defined");
                        this._assertInvariant();
                        if (this.panes.length == 0)
                            return;
                        var that = this,
                            panesData = [],
                            freeSpace = this.getChildrenSize()[direction],
                            setPaneSizeProc = function(pane, sizeInPixels) {
                                var newSize = pane.sizeInPixels.clone();
                                newSize[direction] = sizeInPixels;
                                pane.setSizeInPixels(newSize)
                            };
                        if (direction === this._childDirection) {
                            $.each(that.panes, function(_, pane) {
                                panesData.push({
                                    constraints: pane.getActualConstraints(),
                                    sizeInPercents: pane.sizeInPercents,
                                    sizeInPixels: null,
                                    deltaSizeInPixels: null,
                                    applyChanges: function() {
                                        setPaneSizeProc(pane, this.sizeInPixels);
                                        debug.assert(this.sizeInPixels >= 0, "_arrangeChildren along direction: sizeInPixels < 0: " + this.sizeInPixels);
                                        debug.assert(this.sizeInPixels === pane.sizeInPixels[direction], "_arrangeChildren  along direction: sizeInPixels != pane.sizeInPixels[direction], delta: " + (this.sizeInPixels - pane.sizeInPixels[direction]))
                                    }
                                })
                            });
                            panesData = that.distributeSpace(direction, freeSpace, panesData);
                            $.each(panesData, function(_, paneData) {
                                paneData.applyChanges()
                            })
                        }
                        else
                            $.each(that.panes, function(_, pane) {
                                setPaneSizeProc(pane, freeSpace)
                            })
                    },
                    _beginUpdate: function() {
                        this._updateCount++;
                        $.each(this.panes, function(i, pane) {
                            pane._beginUpdate()
                        })
                    },
                    _endUpdate: function() {
                        this._updateCount--;
                        $.each(this.panes, function(i, pane) {
                            pane._endUpdate()
                        });
                        if (this._updateCount === 0)
                            this.elementProxy.applySize(this.getClientSize())
                    },
                    _assertInvariant: function() {
                        debug.assert(layout.utils.checkRange(this.sizeInPercents.width.toPrecision(3), 0, 1), "_assertInvariant: doesn't satisfy (0 <= this.sizeInPercents.width <= 1)");
                        debug.assert(layout.utils.checkRange(this.sizeInPercents.height.toPrecision(3), 0, 1), "_assertInvariant: doesn't satisfy (0 <= this.sizeInPercents.height <= 1)");
                        debug.assert(this.sizeInPixels.width >= 0, "_assertInvariant: doesn't satisfy (sizeInPixels.width: >= 0)");
                        debug.assert(this.sizeInPixels.height >= 0, "_assertInvariant: doesn't satisfy (sizeInPixels.height: >= 0)")
                    },
                    getActualConstraints: function() {
                        var that = this,
                            constr = undefined,
                            nonClientElementsSize = that._getNonClientOuterElementsSize().plus(that._getNonClientInnerElementsSize());
                        if (that.panes.length > 0)
                            $.each(that.panes, function(i, pane) {
                                var paneConstraints = pane.getActualConstraints();
                                constr = i == 0 ? paneConstraints : constr.consolidate(paneConstraints, direction)
                            });
                        else
                            constr = this.elementProxy.getConstraints();
                        constr = constr.consolidate(that.constraints);
                        constr.min = constr.min.plus(nonClientElementsSize);
                        constr.max = constr.max.plus(nonClientElementsSize);
                        return constr
                    },
                    _extractSize: function(elements) {
                        var sum = new layout.utils.size(0, 0);
                        $.each(elements, function(_, element) {
                            sum = sum.plus(element.getBounds())
                        });
                        return sum
                    },
                    _getNonClientOuterElementsSize: function() {
                        return this._extractSize(this.nonClientOuterElements)
                    },
                    _getNonClientInnerElementsSize: function() {
                        return this._extractSize(this.nonClientInnerElements)
                    }
                }
        }
    })(jQuery, DevExpress);
    /*! Module dashboard, file resizer.js */
    (function($, DX, undefined) {
        var Class = DX.require("/class"),
            events = DX.ui.events,
            debug = DX.require("/utils/utils.console").debug,
            devices = DX.require("/devices"),
            eventUtils = DX.require("/ui/events/ui.events.utils"),
            pointerEvents = DX.require("/ui/events/pointer/ui.events.pointer"),
            dashboard = DX.dashboard,
            layout = dashboard.layout;
        layout.paneTreeItemResizer = function(previous, next, beginningPosition, direction) {
            var parent = previous.parent,
                lengthPrevious = previous.sizeInPixels[direction],
                lengthNext = next.sizeInPixels[direction],
                length = lengthPrevious + lengthNext,
                totalLengthInPercents = previous.sizeInPercents[direction] + next.sizeInPercents[direction],
                constraintsPrevious = previous.getActualConstraints(),
                constraintsNext = next.getActualConstraints(),
                defaultOffsetMin = Math.max(constraintsPrevious.min[direction], length - constraintsNext.max[direction]) - lengthPrevious,
                defaultOffsetMax = Math.min(constraintsPrevious.max[direction], length - constraintsNext.min[direction]) - lengthPrevious;
            debug.assert(length >= 0, "length in pixels must be greater zero");
            return {
                    offset: 0,
                    move: function(newPosition, initialPosition) {
                        if (defaultOffsetMin < defaultOffsetMax) {
                            var offset = newPosition - initialPosition;
                            this.offset = offset < 0 ? Math.max(offset, defaultOffsetMin) : Math.min(offset, defaultOffsetMax)
                        }
                        return initialPosition + this.offset
                    },
                    applyChanges: function() {
                        if (this.offset != 0) {
                            var offsetInPercents = this.offset * totalLengthInPercents / length;
                            previous.sizeInPercents[direction] = previous.sizeInPercents[direction] + offsetInPercents;
                            next.sizeInPercents[direction] = next.sizeInPercents[direction] - offsetInPercents;
                            debug.assert(totalLengthInPercents.toPrecision(3) === (previous.sizeInPercents[direction] + next.sizeInPercents[direction]).toPrecision(3), "paneTreeItemResizer invariant: (totalSizeInPercents === previous.sizeInPercents[direction] + next.sizeInPercents[direction])");
                            debug.assert(previous.sizeInPercents[direction].toPrecision(3) >= 0, "paneTreeItemResizer invariant: (0 <= previous.sizeInPercents[direction])");
                            debug.assert(next.sizeInPercents[direction].toPrecision(3) >= 0, "paneTreeItemResizer invariant: (0 <= next.sizeInPercents[direction])");
                            parent.rearrangeChildren()
                        }
                    }
                }
        };
        layout.viewerResizer = Class.inherit({
            ctor: function ctor() {
                this.$document = $(document)
            },
            attachResizeHandlers: function($parent, $dest) {
                var that = this,
                    startHandler = function($separator) {
                        var accessor = $separator.data('resizableElementsAccessor'),
                            direction = accessor.direction,
                            movingSeparatorClass = direction == 'width' ? layout.cssClassNames.movingSeparatorV : layout.cssClassNames.movingSeparatorH,
                            offsetDirection = direction == 'width' ? 'left' : 'top',
                            separatorPanesData = {
                                $prev: accessor.$prev(),
                                $next: accessor.$next()
                            },
                            beginningPosition = separatorPanesData.$prev.offset()[offsetDirection];
                        that._prependHoverShield(separatorPanesData.$prev, separatorPanesData.$next);
                        var movementData = {
                                direction: direction,
                                offset: $separator.offset(),
                                $splitterDiv: that.createSplitterDiv($separator, $parent, movingSeparatorClass),
                                separatorPanesData: separatorPanesData,
                                paneTreeItemResizer: dashboard.layout.paneTreeItemResizer(accessor.getPrevious(), accessor.getNext(), beginningPosition, direction)
                            };
                        that.$document.on(eventUtils.addNamespace(pointerEvents.move, 'dxDashboardViewResizer'), movementData, $.proxy(that, '_moveHandler'));
                        that.$document.on(eventUtils.addNamespace(pointerEvents.up, 'dxDashboardViewResizer'), movementData, $.proxy(that, '_endHandler'));
                        return false
                    };
                $dest.on(eventUtils.addNamespace(pointerEvents.down, 'dxDashboardViewResizer'), 'td.' + layout.cssClassNames.splitterSeparatorV, function startWidthDirectionHandler(ev) {
                    return startHandler($(this))
                });
                $dest.on(eventUtils.addNamespace(pointerEvents.down, 'dxDashboardViewResizer'), 'td.' + layout.cssClassNames.splitterSeparatorH, function startHeightDirectionHandler(ev) {
                    return startHandler($(this))
                })
            },
            createSplitterDiv: function($this, $parent, className) {
                var $splitterDiv = $('<div/>', {
                        style: 'position:absolute;',
                        'class': className
                    }),
                    separatorOffset = $this.offset();
                $splitterDiv.addClass(layout.cssClassNames.splitterResizing);
                $splitterDiv.width($this.width());
                $splitterDiv.height($this.height());
                $splitterDiv.appendTo($parent);
                $splitterDiv.offset(separatorOffset);
                return $splitterDiv
            },
            skipDevice: function(ev) {
                return devices.real().ios && eventUtils.isMouseEvent(ev)
            },
            _moveHandler: function(ev) {
                if (!this.skipDevice(ev)) {
                    var data = eventUtils.eventData(ev),
                        offsetDirection = ev.data.direction == 'width' ? 'left' : 'top',
                        dataDirection = ev.data.direction == 'width' ? 'x' : 'y',
                        newOffset = {
                            left: ev.data.offset.left,
                            top: ev.data.offset.top
                        };
                    newOffset[offsetDirection] = ev.data.paneTreeItemResizer.move(data[dataDirection], newOffset[offsetDirection]);
                    ev.data.$splitterDiv.offset(newOffset)
                }
                return false
            },
            _endHandler: function(ev) {
                var that = this,
                    $splitterDiv = ev.data.$splitterDiv,
                    panesData = ev.data.separatorPanesData,
                    $firstElement = panesData.$prev,
                    $secondElement = panesData.$next;
                that.$document.off(eventUtils.addNamespace(pointerEvents.move, "dxDashboardViewResizer"));
                that.$document.off(eventUtils.addNamespace(pointerEvents.up, "dxDashboardViewResizer"));
                $splitterDiv.remove();
                ev.data.paneTreeItemResizer.applyChanges();
                that._deleteHoverShield($firstElement, $secondElement)
            },
            _deleteHoverShield: function($firstLayoutPlace, $secondLayoutPlace) {
                $firstLayoutPlace.find('.' + layout.cssClassNames.layoutShield).remove();
                $secondLayoutPlace.find('.' + layout.cssClassNames.layoutShield).remove()
            },
            _prependHoverShield: function($firstLayoutPlace, $secondLayoutPlace) {
                var that = this;
                if ($firstLayoutPlace.find('.' + layout.cssClassNames.layoutShield).length === 0 && $secondLayoutPlace.find('.' + layout.cssClassNames.layoutShield).length === 0) {
                    $firstLayoutPlace.prepend($('<div>', {
                        'class': layout.cssClassNames.layoutShield,
                        css: {
                            width: $firstLayoutPlace.width(),
                            height: $firstLayoutPlace.height()
                        }
                    }));
                    $secondLayoutPlace.prepend($('<div>', {
                        'class': layout.cssClassNames.layoutShield,
                        css: {
                            width: $secondLayoutPlace.width(),
                            height: $secondLayoutPlace.height()
                        }
                    }))
                }
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file tableConstructor.js */
    (function($, DX, undefined) {
        var Class = DX.require("/class"),
            dashboard = DX.dashboard,
            debug = DX.require("/utils/utils.console").debug;
        dashboard.layout = dashboard.layout ? dashboard.layout : {};
        dashboard.layout.tableConstructor = function($parent, direction) {
            var isDirectionWidth = direction == 'width',
                cssClassTableName = 'dx-viewer-item-table';
            return {
                    getNextCell: function(options) {
                        var that = this,
                            optionsCell = options ? options : {},
                            newCell;
                        newCell = $('<td/>', optionsCell);
                        newCell.appendTo(that._getNextRow());
                        return newCell
                    },
                    _getNextRow: function() {
                        var that = this,
                            result;
                        if (isDirectionWidth) {
                            if (!that.$horizontalRow) {
                                that.$horizontalRow = $('<tr/>');
                                that.$horizontalRow.appendTo(this._getTable())
                            }
                            result = that.$horizontalRow
                        }
                        else {
                            result = $('<tr/>');
                            result.appendTo(that._getTable())
                        }
                        return result
                    },
                    _getTable: function() {
                        if (!this.$table) {
                            this.$table = $('<table/>', {
                                cellpadding: 0,
                                cellspacing: 0,
                                'class': cssClassTableName
                            });
                            this.$table.appendTo($parent)
                        }
                        return this.$table
                    }
                }
        }
    })(jQuery, DevExpress);
    /*! Module dashboard, file dashboardViewer.serviceClient.js */
    (function($, DX, undefined) {
        var Class = DevExpress.require("/class"),
            dashboard = DX.dashboard,
            serviceTasks,
            viewerTasks,
            viewerActions,
            serviceResultCodes;
        dashboard.serviceTasks = {
            initialize: 'Initialize',
            performAction: 'PerformAction',
            reloadData: 'ReloadData',
            exportTo: 'Export'
        };
        dashboard.viewerTasks = {
            initialize: 'Initialize',
            update: 'Update',
            showMessage: 'ShowMessage',
            updateState: 'UpdateState'
        };
        dashboard.viewerActions = {
            setMasterFilter: 'SetMasterFilter',
            setMultipleValuesMasterFilter: 'SetMultipleValuesMasterFilter',
            clearMasterFilter: 'ClearMasterFilter',
            drillDown: 'DrillDown',
            drillUp: 'DrillUp',
            setSelectedElementIndex: 'SetSelectedElementIndex',
            expandValue: 'ExpandValue',
            dataRequest: 'DataRequest',
            getDrillThroughData: 'GetDrillThroughData'
        };
        dashboard.serviceResultCodes = {
            success: 'Success',
            dashboardNotFound: 'DashboardNotFound',
            internalError: 'InternalError'
        };
        dashboard.specialValueNames = {
            NullValue: 'DashboardNullValue',
            OthersValue: 'DashboardOthersValue',
            ErrorValue: 'DashboardErrorValue'
        };
        dashboard.itemDataAxisNames = {
            defaultAxis: 'Default',
            chartSeriesAxis: 'Series',
            chartArgumentAxis: 'Argument',
            sparklineAxis: 'Sparkline',
            pivotColumnAxis: 'Column',
            pivotRowAxis: 'Row'
        };
        dashboard.getMultiData = function(itemDataDTO) {
            var dataManager = undefined;
            if (itemDataDTO && itemDataDTO.MetaData) {
                dataManager = new dashboard.data.itemDataManager;
                dataManager.initialize(itemDataDTO)
            }
            return dataManager && dataManager.getItemData()
        };
        serviceTasks = dashboard.serviceTasks;
        viewerActions = dashboard.viewerActions;
        viewerTasks = dashboard.viewerTasks;
        serviceResultCodes = dashboard.serviceResultCodes;
        dashboard.serviceClient = Class.inherit({
            ctor: function ctor(service, performViewerTask) {
                this._service = service;
                this._performViewerTask = performViewerTask;
                this._service.responseReceived.add($.proxy(this._processServiceResponse, this));
                this._requestHash = {};
                this._requestMarker = -1;
                this.actionAvailabilityChanged = new $.Callbacks;
                this.actionPerformed = new $.Callbacks;
                this.itemClick = new $.Callbacks;
                this.itemHover = new $.Callbacks;
                this.itemVisualInteractivity = new $.Callbacks;
                this.itemSelectionChanged = new $.Callbacks;
                this.itemWidgetCreated = new $.Callbacks;
                this.itemWidgetUpdating = new $.Callbacks;
                this.itemWidgetUpdated = new $.Callbacks;
                this.itemElementCustomColor = new $.Callbacks
            },
            initialize: function(dashboardId, settings) {
                this._dashboardId = dashboardId;
                this._settings = settings;
                this._sessionId = undefined;
                this._context = undefined;
                this._model = {items: {}};
                this._actionBatch = function() {
                    var actions = [],
                        changed = false,
                        locked = false;
                    return {
                            startTrackingChanges: function() {
                                changed = false
                            },
                            hasChanges: function() {
                                return changed
                            },
                            addAction: function(action) {
                                changed = true;
                                actions.push(action)
                            },
                            clear: function() {
                                changed = false;
                                actions = []
                            },
                            getBatch: function() {
                                return actions.slice()
                            },
                            getCallbacks: function() {
                                var callbacks = [];
                                $.each(actions, function(_, action) {
                                    if (action.OnCompleted)
                                        callbacks.push(action.OnCompleted)
                                });
                                return callbacks
                            },
                            lock: function() {
                                locked = true
                            },
                            unlock: function() {
                                locked = false
                            },
                            locked: function() {
                                return locked
                            }
                        }
                }();
                this._sendServiceRequest({
                    Task: serviceTasks.initialize,
                    DashboardId: dashboardId,
                    Settings: settings
                })
            },
            reloadData: function(parameters) {
                var that = this;
                that._sendServiceRequest({
                    Task: serviceTasks.reloadData,
                    DashboardParameters: parameters || that.parametersCollection.getParameterValues()
                });
                that._updateStateOnReloadDataStarting();
                that._performViewerTask(viewerTasks.updateState, {state: that._getViewerState()})
            },
            exportDashboard: function(modeInfo, documentOptions) {
                this._exportTo({
                    mode: 'EntireDashboard',
                    clientState: modeInfo.clientState,
                    format: modeInfo.format,
                    fileName: modeInfo.fileName
                }, documentOptions)
            },
            _getValues: function(itemName, tuples) {
                var values = [],
                    dimensionIds = this._getDimensionIdsByItemName(itemName),
                    dimensionCount = dimensionIds.length;
                $.each(tuples, function(index, tuple) {
                    var value = [],
                        axisPoint = tuple.GetAxisPoint();
                    while (axisPoint.getParent() != null) {
                        value.push(axisPoint.getUniqueValue());
                        if (dimensionCount == 1)
                            break;
                        axisPoint = axisPoint.getParent()
                    }
                    value.reverse();
                    values.push(value)
                });
                return values
            },
            performAction: function(action) {
                var itemActions = this._model.items[action.ItemName],
                    actions = [],
                    values,
                    msgNoParameters = 'Cannot perform interactivity action. No value has been specified.';
                switch (action.ActionName) {
                    case viewerActions.drillDown:
                        if (action.Parameters) {
                            values = action.Parameters instanceof dashboard.data.itemDataTuple ? this._getValues(action.ItemName, [action.Parameters]) : [[action.Parameters]];
                            if (itemActions.actionModel['SetMasterFilter'])
                                actions.push({
                                    ActionName: viewerActions.setMasterFilter,
                                    ItemName: action.ItemName,
                                    Parameters: values
                                });
                            action.Parameters = values[0]
                        }
                        else
                            $.error(msgNoParameters);
                        break;
                    case viewerActions.drillUp:
                        if (itemActions.actionModel['ClearMasterFilter'])
                            actions.push({
                                ActionName: viewerActions.clearMasterFilter,
                                ItemName: action.ItemName
                            });
                        break;
                    case viewerActions.setMasterFilter:
                        if (action.Parameters) {
                            values = action.Parameters[0] instanceof dashboard.data.itemDataTuple ? this._getValues(action.ItemName, action.Parameters) : action.Parameters;
                            action.Parameters = values
                        }
                        else
                            $.error(msgNoParameters);
                        break;
                    case viewerActions.setMultipleValuesMasterFilter:
                        if (action.Parameters.length > 0) {
                            values = action.Parameters[0] instanceof dashboard.data.itemDataTuple ? this._getValues(action.ItemName, action.Parameters) : action.Parameters;
                            action.Parameters = values
                        }
                        break
                }
                actions.push(action);
                this._performBatchAction(actions)
            },
            getItemData: function(itemName) {
                var dataManager = this._getItemDataManager(itemName);
                return dataManager && dataManager.getItemData()
            },
            _getItemDataManager: function(itemName) {
                var that = this,
                    itemModel = that._model.items[itemName];
                if (!itemModel)
                    return null;
                var dataManager = itemModel.dataManager,
                    itemDataDTO = itemModel.itemDataDTO;
                if (!dataManager && itemDataDTO && itemDataDTO.MetaData) {
                    dataManager = new dashboard.data.itemDataManager;
                    dataManager.initialize(itemDataDTO);
                    itemModel.dataManager = dataManager
                }
                return dataManager
            },
            requestUnderlyingData: function(itemName, args, onCompleted) {
                var that = this,
                    metaData = that._getItemDataManager(itemName).getMetaData(),
                    axisPoints = that._getUnderlyingDataArgsAxisPoints(itemName, args),
                    columnNames = args.dataMembers,
                    pivotAreaValues = {};
                $.each(axisPoints, function(_, axisPoint) {
                    var name = axisPoint.getAxisName();
                    pivotAreaValues[metaData.getPivotAreaByAxisName(name)] = axisPoint.getUniquePath()
                });
                that._performBatchAction([{
                        ActionName: viewerActions.getDrillThroughData,
                        ItemName: itemName,
                        OnCompleted: onCompleted,
                        Parameters: [pivotAreaValues['Columns'], pivotAreaValues['Rows'], columnNames]
                    }])
            },
            _getUnderlyingDataArgsAxisPoints: function(itemName, args) {
                var data = this.getItemData(itemName),
                    axisNames = data.getAxisNames(),
                    axisPoints = args.axisPoints;
                if (!axisPoints) {
                    axisPoints = [];
                    $.each(axisNames, function(_, axisName) {
                        var axis = data.getAxis(axisName),
                            axisPoint = undefined;
                        if (args.uniqueValuesByAxisName) {
                            var axisValues = args.uniqueValuesByAxisName[axisName];
                            if (axisValues)
                                axisPoint = axis.getPointByUniqueValues(axisValues)
                        }
                        if (args.valuesByAxisName) {
                            var axisValues = args.valuesByAxisName[axisName];
                            if (axisValues)
                                axisPoint = axis.getPointByValues(axisValues)
                        }
                        if (!axisPoint)
                            axisPoint = axis.getRootPoint();
                        axisPoints.push(axisPoint)
                    })
                }
                $.each(axisNames, function(_, axisName) {
                    var points = $.grep(axisPoints, function(axisPoint) {
                            return axisPoint.getAxisName() === axisName
                        });
                    if (points.length == 0)
                        axisPoints.push(data.getAxis(axisName).getRootPoint())
                });
                return axisPoints
            },
            _sendServiceRequest: function(params, requestMetaData) {
                var that = this;
                that._requestMarker++;
                that._service.sendRequest($.extend(params, {
                    SessionId: that._sessionId,
                    Context: that._context,
                    RequestMarker: that._requestMarker,
                    ClientState: that._prepareClientState()
                }));
                if (params.Task !== serviceTasks.exportTo)
                    that._requestHash[that._requestMarker] = {
                        taskName: params.Task,
                        metaData: requestMetaData
                    }
            },
            _prepareClientState: function() {
                var clientState = {};
                $.each(this._model.items, function(name, item) {
                    if (item.clientState)
                        clientState[name] = item.clientState
                });
                return clientState
            },
            _processServiceResponse: function(result) {
                var that = this,
                    requestMarker = result.RequestMarker,
                    taskName = that._requestHash[requestMarker].taskName,
                    requestMetaData = that._requestHash[requestMarker].metaData,
                    message = result.Error ? result.Error.Message : null;
                delete this._requestHash[requestMarker];
                switch (result.ResultCode) {
                    case serviceResultCodes.success:
                        if (that._actionBatch.hasChanges()) {
                            that._sendBatchActionRequest();
                            return
                        }
                        that._context = result.Context;
                        if (result.LoadingDataErrorMessage && result.LoadingDataErrorMessage.length > 0)
                            that._performViewerTask(viewerTasks.showMessage, {
                                isModal: true,
                                message: result.LoadingDataErrorMessage
                            });
                        var batch = that._actionBatch.getBatch();
                        that._actionBatch.clear();
                        that._actionBatch.startTrackingChanges();
                        that.lockBatchActions();
                        that._processTaskResult(taskName, requestMetaData, result);
                        that._fireActionBatchEvents(batch);
                        that.unlockBatchActions();
                        break;
                    case serviceResultCodes.dashboardNotFound:
                    case serviceResultCodes.internalError:
                        that._performViewerTask(viewerTasks.showMessage, {
                            isModal: false,
                            message: message
                        });
                        break
                }
                if (!that._actionBatch.hasChanges())
                    that._actionBatch.clear()
            },
            _processTaskResult: function(taskName, requestMetaData, result) {
                var that = this,
                    paneContent = result.PaneContent || [];
                that._processPaneContent(paneContent);
                switch (taskName) {
                    case serviceTasks.initialize:
                        that._sessionId = result.SessionId;
                        that.lockBatchActions();
                        that.parametersCollection = new dashboard.data.parametersCollection(result.DashboardParameters);
                        that._setItemsInteractivityOptions();
                        that._fireItemVisualInteractivityEvents();
                        that._performViewerTask(viewerTasks.initialize, {
                            model: {
                                rootPane: result.RootPane,
                                paneContent: result.PaneContent,
                                itemsMultiData: that._generateItemData(),
                                titleViewModel: result.TitleViewModel,
                                masterFilterValues: result.MasterFilterValues,
                                exportData: result.ExportData
                            },
                            localization: result.Localization,
                            handlers: that._getItemEventHandlers(),
                            state: that._getViewerState(),
                            itemsInteractivityOptions: that._getItemsInteractivityOptions()
                        });
                        that.unlockBatchActions();
                        break;
                    case serviceTasks.performAction:
                    case serviceTasks.reloadData:
                        that._fireItemVisualInteractivityEvents();
                        that._performViewerTask(viewerTasks.update, {
                            content: result.PaneContent,
                            itemsMultiData: that._generateItemData(),
                            masterFilterValues: result.MasterFilterValues,
                            state: that._getViewerState(),
                            clientState: result.ClientState,
                            itemsInteractivityOptions: that._getItemsInteractivityOptions()
                        });
                        if (requestMetaData && requestMetaData.length) {
                            var underlyingData = result.UnderlyingData;
                            for (var i = 0; i < underlyingData.length; i++) {
                                var drillThroughData = new dashboard.data.drillThroughDataWrapper(underlyingData[i]);
                                drillThroughData.initialize();
                                requestMetaData[i](drillThroughData)
                            }
                        }
                        break;
                    default:
                        $.error('Unexpected result task name ' + taskName);
                        break
                }
            },
            _setItemsInteractivityOptions: function() {
                $.each(this._model.items, function(name, item) {
                    item.interactivityOptions = {
                        selectionMode: dashboard.dashboardSelectionMode.none,
                        hoverEnabled: false,
                        targetAxes: [],
                        defaultSelectedValues: []
                    }
                })
            },
            _getItemsInteractivityOptions: function() {
                var that = this,
                    interactivityOptions,
                    itemsInteractivityOptions = {};
                $.each(that._model.items, function(itemName, itemModel) {
                    interactivityOptions = itemModel.interactivityOptions;
                    itemsInteractivityOptions[itemName] = {};
                    itemsInteractivityOptions[itemName].selectionMode = interactivityOptions.selectionMode;
                    itemsInteractivityOptions[itemName].hoverEnabled = interactivityOptions.hoverEnabled;
                    itemsInteractivityOptions[itemName].targetAxes = interactivityOptions.targetAxes;
                    itemsInteractivityOptions[itemName].defaultSelectedValues = interactivityOptions.defaultSelectedValues;
                    if (!that._isInteractivityActionEnabled(itemName) && interactivityOptions.defaultSelectedValues.length == 0 && interactivityOptions.selectionMode == dashboard.dashboardSelectionMode.single) {
                        var tuple = [],
                            itemData = that.getItemData(itemName);
                        $.each(interactivityOptions.targetAxes, function(index, axisName) {
                            tuple.push({
                                AxisName: axisName,
                                Value: itemData.getAxis(axisName).getPoints()[0].getUniquePath()
                            })
                        });
                        itemsInteractivityOptions[itemName].defaultSelectedValues = [tuple]
                    }
                });
                return itemsInteractivityOptions
            },
            _generateItemData: function() {
                var that = this,
                    multiData = {};
                $.each(this._model.items, function(name, _) {
                    multiData[name] = that.getItemData(name)
                });
                return multiData
            },
            _getViewerState: function() {
                var model = this._model,
                    items = {};
                $.each(model.items, function(name, item) {
                    items[name] = item.state
                });
                return {
                        items: items,
                        viewer: model.viewerState
                    }
            },
            _getItemEventHandlers: function() {
                var that = this;
                return {
                        onSelected: function(name, actionName, value) {
                            that._performBatchAction([{
                                    ItemName: name,
                                    ActionName: actionName,
                                    Parameters: value
                                }])
                        },
                        onClearMasterFilter: function(name) {
                            that._performBatchAction([{
                                    ItemName: name,
                                    ActionName: viewerActions.clearMasterFilter
                                }])
                        },
                        onDrillUp: function(name, hasSelection) {
                            var itemActions = that._model.items[name],
                                actions = [];
                            if (hasSelection && itemActions.actionModel['ClearMasterFilter'])
                                actions.push({
                                    ItemName: name,
                                    ActionName: viewerActions.clearMasterFilter
                                });
                            actions.push({
                                ItemName: name,
                                ActionName: viewerActions.drillUp
                            });
                            that._performBatchAction(actions)
                        },
                        onContentElementSelection: function(name, contentParams) {
                            that._performBatchAction([{
                                    ItemName: name,
                                    ActionName: viewerActions.setSelectedElementIndex,
                                    Parameters: [contentParams.index]
                                }])
                        },
                        onExpandValue: function(name, expandParams) {
                            that._performBatchAction([{
                                    ItemName: name,
                                    ActionName: viewerActions.expandValue,
                                    Parameters: [expandParams.values, expandParams.isColumn, expandParams.isExpand, expandParams.isRequestData]
                                }])
                        },
                        onDataRequest: function(name) {
                            that._performBatchAction([{
                                    ItemName: name,
                                    ActionName: viewerActions.dataRequest
                                }])
                        },
                        onClientStateUpdate: function(name, clientState) {
                            that._model.items[name].clientState = clientState
                        },
                        onExportTo: function(name, exportParams) {
                            that._exportTo({
                                name: name,
                                mode: exportParams.mode,
                                clientState: exportParams.clientState,
                                format: exportParams.format,
                                fileName: exportParams.fileName,
                                itemType: exportParams.itemType
                            }, exportParams.documentOptions)
                        },
                        onItemClick: function(name, dataPoint) {
                            that._onItemClick(name, dataPoint)
                        },
                        onItemHover: function(name, dataPoint, state) {
                            that._onItemHover(name, dataPoint, state)
                        },
                        onItemSelectionChanged: function(name, tuples) {
                            that._onItemSelectionChanged(name, tuples)
                        },
                        onItemWidgetCreated: function(name, viewControl) {
                            that._onItemWidgetCreated(name, viewControl)
                        },
                        onItemWidgetUpdating: function(name, viewControl) {
                            that._onItemWidgetUpdating(name, viewControl)
                        },
                        onItemWidgetUpdated: function(name, viewControl) {
                            that._onItemWidgetUpdated(name, viewControl)
                        },
                        onItemElementCustomColor: function(name, eventArgs) {
                            that._onItemElementCustomColor(name, eventArgs)
                        }
                    }
            },
            _processPaneContent: function(content) {
                var that = this,
                    model = that._model,
                    updatedNames = [];
                $.each(content, function(_, item) {
                    var itemActions = [],
                        name = item.Name,
                        itemActionModel = item.ActionModel || {AffectedItems: {}},
                        itemModel = model.items[name] || {},
                        updateItemData = item.ContentType === 'FullContent' || item.ContentType === 'CompleteDataSource',
                        updateActionModel = item.ContentType !== 'CompleteDataSource',
                        updateExpandedData = item.ContentType === 'PartialDataSource';
                    updatedNames.push(name);
                    if (updateExpandedData) {
                        var dataManager = that._getItemDataManager(item.Name);
                        if (dataManager && item.ItemData)
                            dataManager.updateExpandedData(item.ItemData, {
                                values: item.Parameters[0],
                                pivotArea: item.Parameters[1] ? dashboard.data.pivotAreaNames.columns : dashboard.data.pivotAreaNames.rows
                            })
                    }
                    else {
                        if (updateItemData) {
                            if (item.ItemData)
                                itemModel.itemDataDTO = item.ItemData;
                            itemModel.dataManager = null;
                            itemModel.data = null
                        }
                        if (updateActionModel) {
                            model.items[name] = itemModel;
                            itemModel.selectedValues = item.SelectedValues;
                            itemModel.axisName = item.AxisName;
                            itemModel.dimensionIds = item.DimensionIds;
                            itemModel.actionModel = itemActionModel.AffectedItems;
                            $.each(viewerActions, function(_, actionName) {
                                var items = itemActionModel.AffectedItems[actionName];
                                if (items)
                                    itemActions.push(actionName)
                            });
                            delete itemActionModel.AffectedItems;
                            itemActionModel.Actions = itemActions
                        }
                    }
                });
                that._updateStateOnOperationCompleted(updatedNames)
            },
            _performBatchAction: function(actions) {
                var that = this;
                for (var i = 0; i < actions.length; i++) {
                    var action = actions[i];
                    if (!action.ActionName)
                        return;
                    that._actionBatch.addAction(action);
                    that._updateStateOnActionStarting(action.ItemName, action.ActionName);
                    that._performViewerTask(viewerTasks.updateState, {
                        state: that._getViewerState(),
                        selection: action.ActionName === viewerActions.setMasterFilter || action.ActionName === viewerActions.setMultipleValuesMasterFilter || action.ActionName == viewerActions.clearMasterFilter ? {
                            itemName: action.ItemName,
                            parameters: action.Parameters ? action.Parameters : []
                        } : null
                    })
                }
                if (!that._isWaitingForResponse() && !that._actionBatch.locked())
                    that._sendBatchActionRequest();
                that.actionAvailabilityChanged.fire()
            },
            _sendBatchActionRequest: function() {
                var that = this;
                that._sendServiceRequest({
                    Task: serviceTasks.performAction,
                    Actions: that._actionBatch.getBatch()
                }, that._actionBatch.getCallbacks());
                that._actionBatch.startTrackingChanges()
            },
            lockBatchActions: function() {
                this._actionBatch.lock()
            },
            unlockBatchActions: function() {
                this._actionBatch.unlock();
                if (this._actionBatch.hasChanges())
                    this._sendBatchActionRequest()
            },
            _fireActionBatchEvents: function(batch) {
                var that = this;
                $.each(batch, function(_, action) {
                    that.actionPerformed.fire({
                        ActionName: action.ActionName,
                        ItemName: action.ItemName,
                        Parameters: action.Parameters
                    })
                });
                that.actionAvailabilityChanged.fire()
            },
            _fireItemVisualInteractivityEvents: function() {
                var that = this,
                    interactivityOptions;
                $.each(that._model.items, function(itemName, itemModel) {
                    interactivityOptions = itemModel.interactivityOptions;
                    that.itemVisualInteractivity.fire({
                        itemName: itemName,
                        getSelectionMode: function() {
                            return interactivityOptions.selectionMode
                        },
                        setSelectionMode: function(value) {
                            interactivityOptions.selectionMode = value
                        },
                        isHighlightingEnabled: function() {
                            return interactivityOptions.hoverEnabled
                        },
                        enableHighlighting: function(value) {
                            interactivityOptions.hoverEnabled = value
                        },
                        getTargetAxes: function() {
                            return interactivityOptions.targetAxes
                        },
                        setTargetAxes: function(value) {
                            interactivityOptions.targetAxes = value
                        },
                        getDefaultSelection: function() {
                            var itemData = that.getItemData(itemName),
                                realTuples = [];
                            $.each(interactivityOptions.defaultSelectedValues, function(_, tuple) {
                                realTuples.push(itemData.createTuple(tuple))
                            });
                            return realTuples
                        },
                        setDefaultSelection: function(realTuples) {
                            var tuples = [],
                                tuple;
                            $.each(realTuples, function(_, realTuple) {
                                tuple = [];
                                $.each(interactivityOptions.targetAxes, function(__, axisName) {
                                    tuple.push({
                                        AxisName: axisName,
                                        Value: realTuple.getAxisPoint(axisName).getUniquePath()
                                    })
                                });
                                tuples.push(tuple)
                            });
                            interactivityOptions.defaultSelectedValues = tuples
                        }
                    })
                })
            },
            getAvailableActions: function() {
                var that = this,
                    model = that._model,
                    interactivityActions = {};
                $.each(model.items, function(name, item) {
                    var actions = [];
                    if (!!item.state.operations.actions)
                        $.each(item.actionModel, function(actionName, _) {
                            actions.push(actionName)
                        });
                    interactivityActions[name] = actions
                });
                return {
                        reloadData: !that._isWaitingForResponse(),
                        actions: interactivityActions
                    }
            },
            getParametersCollection: function() {
                return this.parametersCollection
            },
            getAvailableDrillDownValues: function(itemName) {
                return $.inArray(dashboard.viewerActions.drillDown, this.getAvailableActions().actions[itemName]) != -1 ? this._getAvailableTuples(itemName) : null
            },
            getCurrentDrillDownValues: function(itemName) {
                var that = this,
                    dimensionIds = this._getDimensionIdsByItemName(itemName),
                    dimensionCount = dimensionIds.length;
                if (dimensionCount > 0) {
                    var data = this.getItemData(itemName),
                        axisName = this._getCurrentAxisNameByItemName(itemName),
                        axis = data.getAxis(axisName),
                        axisPoints = axis.getPointsByDimension(dimensionIds[0]);
                    if (axisPoints.length > 0) {
                        var parentPoint = axisPoints[0].getParent();
                        if (parentPoint.getParent() != null)
                            return new dashboard.data.itemDataTuple([parentPoint]);
                        else
                            return null
                    }
                    else
                        return null
                }
                return null
            },
            getAvailableFilterValues: function(itemName) {
                return $.inArray(dashboard.viewerActions.setMasterFilter, this.getAvailableActions().actions[itemName]) != -1 || $.inArray(dashboard.viewerActions.setMultipleValuesMasterFilter, this.getAvailableActions().actions[itemName]) != -1 ? this._getAvailableTuples(itemName) : null
            },
            getCurrentFilterValues: function(itemName) {
                var that = this,
                    dimensionIds = this._getDimensionIdsByItemName(itemName),
                    dimensionCount = dimensionIds.length;
                if (dimensionCount > 0) {
                    var data = this.getItemData(itemName),
                        axisName = this._getCurrentAxisNameByItemName(itemName),
                        axis = data.getAxis(axisName),
                        tuples = [],
                        selectedValues = that._getSelectedValuesByItemName(itemName);
                    if (selectedValues)
                        $.each(selectedValues, function(index, point) {
                            var value = point[0];
                            var axisPoint = $.grep(axis.getPointsByDimension(dimensionIds[0]), function(point) {
                                    return dashboard.utils.checkValuesAreEqual(value, point.getUniqueValue())
                                })[0];
                            for (var i = 1; i < dimensionCount; i++) {
                                value = point[i];
                                axisPoint = $.grep(axisPoint.getChildren(), function(point) {
                                    return dashboard.utils.checkValuesAreEqual(value, point.getUniqueValue())
                                })[0]
                            }
                            tuples.push(new dashboard.data.itemDataTuple([axisPoint]))
                        });
                    return tuples
                }
                return null
            },
            _isInteractivityActionEnabled: function(itemName) {
                var availableActions = this.getAvailableActions().actions[itemName];
                return $.inArray(dashboard.viewerActions.drillDown, availableActions) != -1 || $.inArray(dashboard.viewerActions.setMasterFilter, availableActions) != -1 || $.inArray(dashboard.viewerActions.setMultipleValuesMasterFilter, availableActions) != -1
            },
            _getAvailableTuples: function(itemName) {
                var that = this,
                    dimensionIds = this._getDimensionIdsByItemName(itemName),
                    dimensionCount = dimensionIds.length;
                if (dimensionCount > 0) {
                    var data = this.getItemData(itemName),
                        axisName = this._getCurrentAxisNameByItemName(itemName),
                        axis = data.getAxis(axisName),
                        tuple,
                        tuples = [];
                    $.each(axis.getPointsByDimension(dimensionIds[dimensionCount - 1]), function(index, axisPoint) {
                        tuples.push(new dashboard.data.itemDataTuple([axisPoint]))
                    });
                    return tuples
                }
                return null
            },
            _getSelectedValuesByItemName: function(itemName) {
                var that = this,
                    model = that._model,
                    itemModel = model.items[itemName];
                return itemModel.selectedValues
            },
            _getCurrentAxisNameByItemName: function(itemName) {
                var that = this,
                    model = that._model,
                    itemModel = model.items[itemName];
                return itemModel.axisName
            },
            _getDimensionIdsByItemName: function(itemName) {
                var that = this,
                    model = that._model,
                    itemModel = model.items[itemName];
                return itemModel.dimensionIds
            },
            _isWaitingForResponse: function() {
                return !!this._requestHash[this._requestMarker]
            },
            _exportTo: function(modeInfo, documentOptions) {
                var that = this,
                    fileName = modeInfo.fileName ? modeInfo.fileName.replace(/[\\/:*?"<>|]/g, '_') : "Export";
                that._sendServiceRequest({
                    Task: serviceTasks.exportTo,
                    ExportInfo: {
                        Mode: modeInfo.mode,
                        GroupName: modeInfo.name,
                        FileName: fileName,
                        ClientState: modeInfo.clientState,
                        Format: modeInfo.format,
                        DocumentOptions: documentOptions,
                        ItemType: modeInfo.itemType
                    }
                })
            },
            _createItemEventArgs: function(itemName, dataPoint, state) {
                var that = this,
                    getData = function() {
                        return that.getItemData(itemName)
                    },
                    patchAxisName = function(axisName) {
                        if (axisName === undefined)
                            axisName = 'Default';
                        return axisName
                    },
                    getAxis = function(axisName) {
                        return getData().getAxis(patchAxisName(axisName))
                    },
                    getAxisPoint = function(axisName) {
                        axisName = patchAxisName(axisName);
                        return getAxis(axisName).getPointByUniqueValues(dataPoint.getValues(axisName))
                    };
                return {
                        itemName: itemName,
                        state: state,
                        getData: getData,
                        getAxisPoint: getAxisPoint,
                        getMeasures: function() {
                            return that._getMeasuresByIds(getData(), dataPoint.getMeasureIds())
                        },
                        getDeltas: function() {
                            var ids = dataPoint.getDeltaIds(),
                                deltas = [],
                                data = getData();
                            $.each(ids, function(i, id) {
                                deltas.push(data.getDeltaById(id))
                            });
                            return deltas
                        },
                        getDimensions: function(axisName) {
                            return getAxis(axisName).getDimensions(axisName)
                        },
                        requestUnderlyingData: function(onCompleted, dataMembers) {
                            var axisPoints = [];
                            $.each(getData().getAxisNames(), function(_, axisName) {
                                axisPoints.push(getAxisPoint(axisName))
                            });
                            that.requestUnderlyingData(itemName, {
                                axisPoints: axisPoints,
                                dataMembers: dataMembers
                            }, onCompleted)
                        }
                    }
            },
            _getMeasuresByIds: function(itemData, measureIds) {
                var that = this,
                    measures = [];
                $.each(measureIds, function(i, id) {
                    measures.push(itemData.getMeasureById(id))
                });
                return measures
            },
            _onItemClick: function(name, dataPoint) {
                this.itemClick.fire(this._createItemEventArgs(name, dataPoint, null))
            },
            _onItemHover: function(name, dataPoint, state) {
                this.itemHover.fire(this._createItemEventArgs(name, dataPoint, state))
            },
            _onItemSelectionChanged: function(name, tuples) {
                var that = this,
                    itemData = that.getItemData(name);
                that.itemSelectionChanged.fire({
                    itemName: name,
                    getCurrentSelection: function() {
                        var axisPointTuples = [];
                        $.each(tuples, function(index, tuple) {
                            axisPointTuples.push(itemData.createTuple(tuple))
                        });
                        return axisPointTuples
                    }
                })
            },
            _onItemWidgetCreated: function(name, widget) {
                this.itemWidgetCreated.fire({
                    itemName: name,
                    getWidget: function() {
                        return widget
                    }
                })
            },
            _onItemWidgetUpdating: function(name, widget) {
                this.itemWidgetUpdating.fire({
                    itemName: name,
                    getWidget: function() {
                        return widget
                    }
                })
            },
            _onItemWidgetUpdated: function(name, widget) {
                this.itemWidgetUpdated.fire({
                    itemName: name,
                    getWidget: function() {
                        return widget
                    }
                })
            },
            _onItemElementCustomColor: function(itemName, eventArgs) {
                var that = this,
                    getData = function() {
                        return that.getItemData(itemName)
                    },
                    newEventArgs = {
                        itemName: itemName,
                        getTargetElement: function() {
                            return new dashboard.data.itemDataTuple(eventArgs.targetElement)
                        },
                        getMeasures: function() {
                            return that._getMeasuresByIds(getData(), eventArgs.measureIds)
                        },
                        getColor: function() {
                            return eventArgs.color
                        },
                        setColor: function(color) {
                            eventArgs.color = color
                        }
                    };
                this.itemElementCustomColor.fire(newEventArgs)
            },
            _updateStateOnActionStarting: function(name, actionName) {
                var that = this,
                    model = that._model,
                    items = model.items,
                    actionModel = items[name].actionModel,
                    affectedNames = actionModel && actionModel[actionName];
                $.each(affectedNames || [], function(_, name) {
                    model.items[name].state = {
                        loading: true,
                        operations: {
                            actions: actionName === viewerActions.dataRequest,
                            exportTo: false
                        }
                    }
                });
                $.each(items, function(name, item) {
                    item.state.operations.exportTo = false
                });
                model.viewerState = {
                    loading: false,
                    operations: {
                        exportTo: false,
                        reloadData: false
                    }
                }
            },
            _updateStateOnReloadDataStarting: function() {
                var that = this,
                    model = that._model,
                    items = model.items;
                $.each(items, function(name, item) {
                    item.state = {
                        loading: false,
                        operations: {
                            actions: $.isEmptyObject(item.actionModel),
                            exportTo: false
                        }
                    }
                });
                model.viewerState = {
                    loading: true,
                    operations: {
                        exportTo: false,
                        reloadData: false
                    }
                }
            },
            _updateStateOnOperationCompleted: function(updatedNames) {
                var that = this,
                    model = that._model,
                    items = model.items;
                $.each(updatedNames, function(_, name) {
                    model.items[name].state = {
                        loading: false,
                        operations: {
                            actions: true,
                            exportTo: true
                        }
                    }
                });
                $.each(items, function(name, item) {
                    item.state.operations.exportTo = true
                });
                model.viewerState = {
                    loading: false,
                    operations: {
                        exportTo: true,
                        reloadData: true
                    }
                }
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file dashboardViewer.js */
    (function($, DX, undefined) {
        var ui = DX.ui,
            DOMComponent = DX.require("/domComponent"),
            dashboard = DX.dashboard,
            viewerTasks = dashboard.viewerTasks,
            viewerActions = dashboard.viewerActions,
            utils = DX.utils,
            browser = DX.require("/utils/utils.browser"),
            windowUtils = DX.require("/utils/utils.window"),
            commonUtils = DX.require("/utils/utils.common"),
            devices = DX.require("/devices"),
            registerComponent = DX.require("/componentRegistrator"),
            localizer = dashboard.data.localizer;
        dashboard.SEPARATOR_SIZE = 10;
        dashboard.MIN_PANE_HEIGHT = 50;
        dashboard.MIN_PANE_WIDTH = 50;
        dashboard.USE_NATIVE_SCROLLING = 'auto';
        dashboard.apiActions = {
            setMasterFilter: 'SetMasterFilter',
            clearMasterFilter: 'ClearMasterFilter',
            drillDown: 'DrillDown',
            drillUp: 'DrillUp'
        };
        dashboard.exportFormats = {
            pdf: 'PDF',
            image: 'Image',
            excel: 'Excel'
        };
        var apiActions = dashboard.apiActions;
        registerComponent("dxDashboardViewer", dashboard, DOMComponent.inherit({
            showExportDialog: function(format) {
                var that = this,
                    dialog = that._getExportDialog();
                dialog.setExportFunction(function(documentInfo) {
                    that.exportTo(format, documentInfo)
                });
                dialog.showDialog("", null, format, {fileName: that.model.titleViewModel ? that.model.titleViewModel.Text : "Dashboard"})
            },
            showParametersDialog: function() {
                this.parametersDialog.show()
            },
            hideParametersDialog: function() {
                this.parametersDialog.hide()
            },
            getCurrentSelection: function(itemName) {
                var that = this,
                    itemData = that.getItemData(itemName),
                    tuples = [];
                $.each(that.getViewerItem(itemName).getSelectedTuples(), function(index, tuple) {
                    tuples.push(itemData.createTuple(tuple))
                });
                return tuples
            },
            getInfo: function(viewerItems) {
                var that = this,
                    $container = that._getContainer(),
                    containerPosition = $container.position(),
                    elementsList = [],
                    convertProc = function(element) {
                        $.each(element, function(key, value) {
                            if (value)
                                if (commonUtils.isObject(value))
                                    convertProc(value);
                                else if (!commonUtils.isArray(value) && commonUtils.isNumber(value))
                                    element[key] = Math.ceil(value)
                        })
                    };
                if (!viewerItems)
                    viewerItems = that.viewerItems;
                $.each(viewerItems, function(index, viewerItem) {
                    var element = viewerItem.getInfo();
                    element.position = {
                        left: element.position.left - containerPosition.left,
                        top: element.position.top - containerPosition.top
                    };
                    convertProc(element);
                    elementsList.push(element)
                });
                return {
                        clientSize: {
                            width: $container.outerWidth(),
                            height: $container.outerHeight()
                        },
                        titleHeight: that.title ? that.title.outerHeight() : 0,
                        itemsState: elementsList
                    }
            },
            getViewerItem: function(name) {
                return this.viewerItems[name]
            },
            documentOptions: function(documentOptions) {
                if (!documentOptions)
                    return this._documentOptions;
                else {
                    $.extend(true, this._documentOptions, documentOptions);
                    this._documentOptions.showTitle = documentOptions.commonOptions.includeCaption
                }
            },
            exportTo: function(format, documentOptions) {
                var that = this,
                    fileName = documentOptions == undefined || documentOptions.fileName == undefined ? "Export" : documentOptions.fileName;
                that.serviceClient.exportDashboard({
                    format: format,
                    clientState: that.getInfo(),
                    fileName: fileName
                }, $.extend(true, {}, that._documentOptions, documentOptions))
            },
            getParametersCollection: function() {
                return this.serviceClient.getParametersCollection()
            },
            reloadData: function(parameters) {
                this.beginUpdateParameters();
                if (parameters)
                    this.getParametersCollection().setParameters(parameters);
                this.endUpdateParameters()
            },
            beginUpdateParameters: function() {
                this._isLocked = true
            },
            endUpdateParameters: function() {
                this._isLocked = false;
                this.getParametersCollection().collectionChanged.fire()
            },
            updateContent: function(content) {
                var that = this,
                    originalContent = (that.model.paneContent || []).slice(0);
                if (!originalContent.length) {
                    that.model.paneContent = content;
                    return
                }
                $.each(content, function(newIndex, itemContent) {
                    var name = itemContent.Name,
                        viewerItem = that._getViewerItem(name);
                    viewerItem.updateContent($.extend(itemContent, {
                        animate: false,
                        multiData: that.model.itemsMultiData[name]
                    }))
                })
            },
            updateSize: function(forceRefresh) {
                var that = this;
                if (that.updateSizeTimerId)
                    clearTimeout(that.updateSizeTimerId);
                that.updateSizeTimerId = setTimeout(function() {
                    var $container = that._getContainer(),
                        size = utils.renderHelper.getActualSize($container);
                    if (that.title && !that.title.isEmpty()) {
                        that.title.width(size.width);
                        size.height -= that.title.outerHeight()
                    }
                    var isChanged = !!that.rootSection && that.rootSection.updateSize(size.width, size.height);
                    if (!isChanged && forceRefresh)
                        $.each(that.viewerItems || [], function(index, viewerItem) {
                            viewerItem.setSize(undefined, undefined)
                        })
                }, 100)
            },
            getAvailableActions: function() {
                var that = this,
                    serviceActions = that.serviceClient.getAvailableActions(),
                    currentApiActions = {};
                $.each(serviceActions.actions, function(itemName, actions) {
                    var itemApiActions = [];
                    $.each(actions, function(_, actionName) {
                        var currentApiAction = that._getApiAction(actionName);
                        if (currentApiAction && $.inArray(currentApiAction, itemApiActions) == -1)
                            itemApiActions.push(currentApiAction)
                    });
                    currentApiActions[itemName] = itemApiActions
                });
                return {
                        reloadData: serviceActions.reloadData,
                        actions: currentApiActions
                    }
            },
            getCurrentRange: function(itemName) {
                var viewerItem = this.getViewerItem(itemName);
                return viewerItem.getCurrentRange()
            },
            getEntireRange: function(itemName) {
                var viewerItem = this.getViewerItem(itemName);
                return viewerItem.getEntireRange()
            },
            setRange: function(itemName, range) {
                var viewerItem = this.getViewerItem(itemName);
                viewerItem.setRange(range)
            },
            performInteractivityAction: function(action) {
                var that = this,
                    actions = that.serviceClient.getAvailableActions().actions[action.ItemName],
                    serviceAction = {
                        ItemName: action.ItemName,
                        Parameters: action.Parameters
                    },
                    viewerAction,
                    msgActionNotAllowed = 'Cannot perform interactivity action. This action is not allowed for the current dashboard item.';
                switch (action.ActionName) {
                    case apiActions.setMasterFilter:
                        if (that._containsValue(actions, viewerActions.setMasterFilter))
                            viewerAction = viewerActions.setMasterFilter;
                        if (that._containsValue(actions, viewerActions.setMultipleValuesMasterFilter))
                            viewerAction = viewerActions.setMultipleValuesMasterFilter;
                        break;
                    case apiActions.clearMasterFilter:
                        if (that._containsValue(actions, viewerActions.clearMasterFilter))
                            viewerAction = viewerActions.clearMasterFilter;
                        break;
                    case apiActions.drillDown:
                        if (that._containsValue(actions, viewerActions.drillDown))
                            viewerAction = viewerActions.drillDown;
                        break;
                    case apiActions.drillUp:
                        if (that._containsValue(actions, viewerActions.drillUp))
                            viewerAction = viewerActions.drillUp;
                        break
                }
                if (viewerAction) {
                    serviceAction.ActionName = viewerAction;
                    that.serviceClient.performAction(serviceAction)
                }
                else
                    $.error(msgActionNotAllowed)
            },
            _init: function _init() {
                var that = this;
                that.callBase();
                that.model = {};
                that.viewerItems = {};
                that.serviceClient = new dashboard.serviceClient(that.option('service'), $.proxy(that._processViewerTask, that));
                that.actionAvailabilityChanged = new $.Callbacks;
                that.actionPerformed = new $.Callbacks;
                that.initialized = new $.Callbacks;
                that.itemClick = new $.Callbacks;
                that.itemHover = new $.Callbacks;
                that.itemVisualInteractivity = new $.Callbacks;
                that.itemSelectionChanged = new $.Callbacks;
                that.itemWidgetCreated = new $.Callbacks;
                that.itemWidgetUpdating = new $.Callbacks;
                that.itemWidgetUpdated = new $.Callbacks;
                that.itemElementCustomColor = new $.Callbacks;
                that.parametersChanged = new $.Callbacks;
                that.serviceClient.actionAvailabilityChanged.add($.proxy(that._onClientActionAvailabilityChanged, that));
                that.serviceClient.actionPerformed.add($.proxy(that._onClientActionPerformed, that));
                that.serviceClient.itemClick.add($.proxy(that._onItemClick, that));
                that.serviceClient.itemHover.add($.proxy(that._onItemHover, that));
                that.serviceClient.itemVisualInteractivity.add($.proxy(that._onItemVisualInteractivity, that));
                that.serviceClient.itemSelectionChanged.add($.proxy(that._onItemSelectionChanged, that));
                that.serviceClient.itemWidgetCreated.add($.proxy(that._onItemWidgetCreated, that));
                that.serviceClient.itemWidgetUpdating.add($.proxy(that._onItemWidgetUpdating, that));
                that.serviceClient.itemWidgetUpdated.add($.proxy(that._onItemWidgetUpdated, that));
                that.serviceClient.itemElementCustomColor.add($.proxy(that._onItemElementCustomColor, that));
                that._isLocked = false;
                that._exportDialog = undefined;
                that._documentOptions = {
                    paperKind: 'Letter',
                    pageLayout: 'Portrait',
                    scaleMode: 'None',
                    scaleFactor: 1.0,
                    autoFitPageCount: 1,
                    showTitle: true,
                    title: "Dashboard",
                    imageFormatOptions: {
                        format: 'Png',
                        resolution: 96
                    },
                    excelFormatOptions: {
                        format: 'Xlsx',
                        csvValueSeparator: ','
                    },
                    commonOptions: {filterStatePresentation: 'None'},
                    pivotOptions: {printHeadersOnEveryPage: true},
                    gridOptions: {
                        fitToPageWidth: true,
                        printHeadersOnEveryPage: true
                    },
                    chartOptions: {
                        automaticPageLayout: true,
                        sizeMode: 'Zoom'
                    },
                    pieOptions: {autoArrangeContent: true},
                    gaugeOptions: {autoArrangeContent: true},
                    cardOptions: {autoArrangeContent: true},
                    mapOptions: {
                        automaticPageLayout: true,
                        sizeMode: 'Zoom'
                    },
                    rangeFilterOptions: {
                        automaticPageLayout: true,
                        sizeMode: 'Stretch'
                    }
                };
                that.documentOptions(that.option('exportOptions'));
                devices.current('desktop');
                that.$rootContainer = $('<div />', {
                    width: '100%',
                    height: '100%'
                });
                that.$rootContainer.appendTo(that.element());
                that._ensureFullscreenMode();
                that.serviceClient.initialize(that.option('dashboardId'), {calculateHiddenTotals: that.option('calculateHiddenTotals')})
            },
            _onClientActionAvailabilityChanged: function() {
                this.actionAvailabilityChanged.fire(this.getAvailableActions())
            },
            _onClientActionPerformed: function(args) {
                args.ActionName = this._getApiAction(args.ActionName);
                this.actionPerformed.fire(args)
            },
            _onItemClick: function(args) {
                this.itemClick.fire(args)
            },
            _onItemHover: function(args) {
                this.itemHover.fire(args)
            },
            _onItemVisualInteractivity: function(args) {
                this.itemVisualInteractivity.fire(args)
            },
            _onItemSelectionChanged: function(args) {
                this.itemSelectionChanged.fire(args)
            },
            _onItemWidgetCreated: function(args) {
                this.itemWidgetCreated.fire(args)
            },
            _onItemWidgetUpdating: function(args) {
                this.itemWidgetUpdating.fire(args)
            },
            _onItemWidgetUpdated: function(args) {
                this.itemWidgetUpdated.fire(args)
            },
            _onItemElementCustomColor: function(args) {
                this.itemElementCustomColor.fire(args)
            },
            _onParametersChanged: function(args) {
                this.parametersChanged.fire(args)
            },
            _processViewerTask: function(task, options) {
                var that = this;
                switch (task) {
                    case viewerTasks.initialize:
                        localizer.initialize(options.localization);
                        that.model = options.model;
                        that._createLayoutItems(options.handlers);
                        that._refresh();
                        that._updateState(options.state, true);
                        that._initializeLayoutItems();
                        that.initialized.fire();
                        that._updateItems(options.itemsInteractivityOptions);
                        break;
                    case viewerTasks.update:
                        that.model.masterFilterValues = options.masterFilterValues;
                        that.model.itemsMultiData = options.itemsMultiData;
                        that.updateContent(options.content);
                        that._updateState(options.state, true);
                        that._updateClientState(options.clientState);
                        that._updateItems(options.itemsInteractivityOptions);
                        break;
                    case viewerTasks.updateState:
                        that._updateState(options.state);
                        if (options.selection)
                            that._updateSelection(options.selection.itemName, options.selection.parameters);
                        break;
                    case viewerTasks.showMessage:
                        if (options.isModal)
                            alert(options.message);
                        else
                            this._renderMessage('dx-dashboard-message', options.message);
                        break
                }
            },
            _updateItems: function(itemsInteractivityOptions) {
                var that = this;
                $.each(itemsInteractivityOptions, function(itemName, options) {
                    that.getViewerItem(itemName).updateItem(options)
                })
            },
            _getViewerItem: function(name) {
                var item = this.viewerItems[name];
                if (!item) {
                    $.error("Unexpected item name: '" + name + "'");
                    return
                }
                return item
            },
            _getContainer: function(empty) {
                var $container = this.$rootContainer;
                if (empty) {
                    $container.empty();
                    this._exportDialog = undefined
                }
                return $container
            },
            _render: function() {
                var that = this,
                    $container = that._getContainer(true),
                    rootPane = that.model.rootPane,
                    parametersCollection = undefined,
                    size = undefined;
                $container.addClass('dx-dashboard-container');
                if (rootPane) {
                    if (that.model.titleViewModel && that.model.titleViewModel.Visible)
                        $container.css({'padding-top': '0px'});
                    size = utils.renderHelper.getActualSize($container);
                    parametersCollection = that.getParametersCollection();
                    if (parametersCollection) {
                        parametersCollection.collectionChanged.add(function() {
                            if (!that._isLocked) {
                                that._onParametersChanged(parametersCollection.getParameterValues());
                                that.serviceClient.reloadData(parametersCollection.getParameterValues())
                            }
                        });
                        if (parametersCollection.getVisibleParameters().length > 0)
                            that.parametersDialog = new dashboard.parametersDialog({
                                $parametersDialogContainer: $container,
                                getParametersCollection: function() {
                                    return parametersCollection
                                },
                                submitParameters: function(parameters) {
                                    that.reloadData(parameters)
                                }
                            })
                    }
                    that.title = new dashboard.title({
                        $container: $container,
                        titleViewModel: that.model.titleViewModel,
                        allowExport: that.option('allowExport'),
                        encodeHtml: that.option('encodeHtml'),
                        showExportDialog: $.proxy(that.showExportDialog, that),
                        showParametersDialog: $.proxy(that.showParametersDialog, that)
                    });
                    that.title.render(size.width);
                    that._renderViewer($container, rootPane, size.width, size.height - this.title.outerHeight())
                }
                else
                    that._renderMessage('dx-dashboard-loading', null)
            },
            _renderViewer: function($container, rootPane, width, height) {
                var that = this,
                    $viewerDiv = $('<div/>', {
                        'class': 'dx-dashboard-viewer',
                        width: width,
                        height: height
                    });
                $container.append($viewerDiv);
                that.rootSection = new dashboard.layout.viewerSection({
                    items: that.viewerItems,
                    layoutItem: rootPane,
                    $container: $viewerDiv,
                    separatorSize: dashboard.SEPARATOR_SIZE,
                    minPaneSize: dashboard.MIN_PANE_WIDTH
                });
                that.rootSection.render()
            },
            _renderMessage: function(className, text) {
                var that = this,
                    size = undefined,
                    $container = that._getContainer(true),
                    $centeredDiv = $('<div/>', {'class': className});
                $centeredDiv.appendTo($container);
                if (text)
                    $centeredDiv.html(text);
                size = utils.renderHelper.getActualSize($container);
                if ($centeredDiv.width() > size.width)
                    $centeredDiv.width(size.width);
                if ($centeredDiv.height() > size.height)
                    $centeredDiv.height(size.height);
                $centeredDiv.css({
                    overflow: 'auto',
                    'margin-left': Math.floor((size.width - $centeredDiv.width()) / 2),
                    'margin-top': Math.floor((size.height - $centeredDiv.height()) / 2)
                })
            },
            _createLayoutItems: function(itemHandlers) {
                var that = this,
                    contentOptions = that.model.paneContent || [],
                    groups = {};
                that.viewerItems = {};
                $.each(contentOptions, function(i, itemOptions) {
                    if (itemOptions) {
                        var name = itemOptions.Name,
                            groupName = itemOptions.Group,
                            isGroup = itemOptions.Type == dashboard.viewerItems.types.group,
                            viewerItem = dashboard.data.factory.createItem(undefined, $.extend(itemOptions, {
                                animate: true,
                                allowExport: that.option('allowItemsExport'),
                                encodeHtml: that.option('encodeHtml'),
                                exportCaption: that.model.exportData ? that.model.exportData.ExportCaptions[name] : name,
                                getExportDialog: function() {
                                    return that._getExportDialog()
                                },
                                multiData: that.model.itemsMultiData[name]
                            })),
                            onExportTo = function(name, exportParams) {
                                var params = $.extend(exportParams, {
                                        mode: isGroup ? 'EntireDashboard' : 'SingleItem',
                                        clientState: that.getInfo(isGroup ? groups[name] : [viewerItem]),
                                        documentOptions: $.extend(true, {}, that._documentOptions, exportParams.documentOptions)
                                    });
                                itemHandlers.onExportTo(name, params)
                            };
                        viewerItem.exportTo.add(onExportTo);
                        viewerItem.selected.add(itemHandlers.onSelected);
                        viewerItem.clearMasterFilter.add(itemHandlers.onClearMasterFilter);
                        viewerItem.drillUp.add(itemHandlers.onDrillUp);
                        viewerItem.contentElementSelection.add(itemHandlers.onContentElementSelection);
                        viewerItem.expandValue.add(itemHandlers.onExpandValue);
                        viewerItem.clientStateUpdate.add(itemHandlers.onClientStateUpdate);
                        viewerItem.dataRequest.add(itemHandlers.onDataRequest);
                        viewerItem.itemClick.add(itemHandlers.onItemClick);
                        viewerItem.itemHover.add(itemHandlers.onItemHover);
                        viewerItem.itemSelectionChanged.add(itemHandlers.onItemSelectionChanged);
                        viewerItem.itemWidgetCreated.add(itemHandlers.onItemWidgetCreated);
                        viewerItem.itemWidgetUpdating.add(itemHandlers.onItemWidgetUpdating);
                        viewerItem.itemWidgetUpdated.add(itemHandlers.onItemWidgetUpdated);
                        if (viewerItem.itemElementCustomColor)
                            viewerItem.itemElementCustomColor.add(itemHandlers.onItemElementCustomColor);
                        that.viewerItems[name] = viewerItem;
                        if (groupName) {
                            if (!groups[groupName])
                                groups[groupName] = [];
                            groups[groupName].push(viewerItem)
                        }
                    }
                })
            },
            _initializeLayoutItems: function() {
                $.each(this.viewerItems, function(_, viewerItem) {
                    viewerItem.initialDataRequest()
                })
            },
            getItemData: function(itemName) {
                return this.serviceClient.getItemData(itemName)
            },
            getAvailableDrillDownValues: function(itemName) {
                return this.serviceClient.getAvailableDrillDownValues(itemName)
            },
            getCurrentDrillDownValues: function(itemName) {
                return this.serviceClient.getCurrentDrillDownValues(itemName)
            },
            getAvailableFilterValues: function(itemName) {
                return this.serviceClient.getAvailableFilterValues(itemName)
            },
            getCurrentFilterValues: function(itemName) {
                return this.serviceClient.getCurrentFilterValues(itemName)
            },
            requestUnderlyingData: function(itemName, args, onCompleted) {
                this.serviceClient.requestUnderlyingData(itemName, args, onCompleted)
            },
            _containsValue: function(array, value) {
                return $.inArray(value, array) !== -1
            },
            _getApiAction: function(viewerAction) {
                switch (viewerAction) {
                    case viewerActions.setMasterFilter:
                    case viewerActions.setMultipleValuesMasterFilter:
                        return apiActions.setMasterFilter;
                    case viewerActions.clearMasterFilter:
                        return apiActions.clearMasterFilter;
                    case viewerActions.drillDown:
                        return apiActions.drillDown;
                    case viewerActions.drillUp:
                        return apiActions.drillUp;
                    default:
                        return undefined
                }
            },
            _getExportDialog: function() {
                var that = this;
                if (!that._exportDialog)
                    if (that.option('allowExport') || that.option('allowItemsExport'))
                        that._exportDialog = new dashboard.exportDialog({
                            $container: that._getContainer(),
                            documentInfo: $.extend({}, that._documentOptions, {title: that.model.exportData ? that.model.exportData.ExportTitle : "Dashboard"})
                        });
                return that._exportDialog
            },
            _updateState: function(state, updateMasterFilter) {
                var that = this,
                    items = state.items;
                if (that.title)
                    that.title.updateState(state.viewer, updateMasterFilter ? that.model.masterFilterValues : undefined);
                $.each(items, function(itemName) {
                    that._getViewerItem(itemName).updateState(items[itemName])
                })
            },
            _updateClientState: function(clientState) {
                var that = this;
                $.each(clientState, function(itemName) {
                    that._getViewerItem(itemName).updateClientState(clientState[itemName])
                })
            },
            _updateSelection: function(itemName, newSelectedValues) {
                var viewerItem = this._getViewerItem(itemName),
                    currentSelectedValues = viewerItem.options.SelectedValues || [],
                    hash = dashboard.utils.wrapHash(currentSelectedValues),
                    isUpdateRequired = !currentSelectedValues || currentSelectedValues.length != newSelectedValues.length;
                if (!isUpdateRequired)
                    $.each(newSelectedValues, function(_, item) {
                        if (!hash[item]) {
                            isUpdateRequired = true;
                            return false
                        }
                    });
                if (isUpdateRequired)
                    viewerItem.setSelection(newSelectedValues)
            },
            _ensureFullscreenMode: function() {
                var that = this,
                    fullScreen = that.option('fullScreen'),
                    redrawOnResize = that.option('redrawOnResize'),
                    handler = function() {
                        that.updateSize()
                    };
                if (fullScreen || redrawOnResize) {
                    windowUtils.resizeCallbacks.add(handler);
                    that.on('disposing', function() {
                        windowUtils.resizeCallbacks.remove(handler)
                    })
                }
                if (fullScreen) {
                    var $container = that._getContainer(),
                        isBrowserIE = !!browser.msie,
                        browserMajorVersion = parseInt(browser.version, 10),
                        overflowProperty = "overflow",
                        oldIEOverflowAutoProperty = null,
                        autoWidth = false,
                        autoHeight = false;
                    $container.addClass('dx-dashboard-fullscreen-mode');
                    if (autoWidth && autoHeight) {
                        overflowProperty = null;
                        oldIEOverflowAutoProperty = "overflow"
                    }
                    else if (autoWidth) {
                        overflowProperty = "overflowY";
                        oldIEOverflowAutoProperty = "overflowX"
                    }
                    else if (autoHeight) {
                        overflowProperty = "overflowX";
                        oldIEOverflowAutoProperty = "overflowY"
                    }
                    var element = $container.get(0).parentNode;
                    while (element && element.tagName) {
                        element.style.height = "100%";
                        var tagName = element.tagName.toLowerCase();
                        if (tagName === "form" || tagName === "body" || tagName === "html") {
                            element.style.margin = "0px";
                            element.style.padding = "0px";
                            if (overflowProperty)
                                element.style[overflowProperty] = "hidden";
                            if (isBrowserIE && browserMajorVersion < 9 && tagName === "form" && oldIEOverflowAutoProperty)
                                element.style[oldIEOverflowAutoProperty] = "auto";
                            if ((autoHeight !== autoWidth || isBrowserIE && browserMajorVersion < 9) && (tagName === "body" || tagName === "html"))
                                element.style.overflow = "hidden"
                        }
                        if (tagName === "html")
                            break;
                        element = element.parentNode
                    }
                }
            }
        }))
    })(jQuery, DevExpress);
    /*! Module dashboard, file dashboardViewer.title.js */
    (function($, DX, undefined) {
        var Class = DevExpress.require("/class"),
            dashboard = DX.dashboard,
            utils = DX.utils,
            localizer = dashboard.data.localizer;
        var DEFAULT_TITLE_HEIGHT = 42;
        dashboard.titleClasses = {
            root: 'dx-dashboard-title',
            arguments: 'dx-dashboard-title-arguments',
            caption: 'dx-dashboard-title-caption',
            filterImage: 'dx-dashboard-title-filter-image',
            image: 'dx-dashboard-title-image',
            loading: 'dx-dashboard-title-loading',
            text: 'dx-dashboard-title-text',
            toolbar: 'dx-dashboard-title-toolbar'
        };
        dashboard.title = Class.inherit({
            ctor: function ctor(options) {
                this.options = options;
                this.exportToMenu = undefined;
                this.$titleDiv = undefined;
                this.titleArguments = undefined;
                this.titleTooltip = undefined;
                this.titleShowFilterImage = false
            },
            isEmpty: function() {
                return this.$titleDiv == undefined
            },
            outerHeight: function() {
                return this.isEmpty() ? 0 : this.$titleDiv.outerHeight()
            },
            width: function(width) {
                var that = this,
                    isEmpty = that.isEmpty();
                if (width !== undefined && !isEmpty)
                    if (that.$titleDiv.width() !== width) {
                        that.$titleDiv.width(width);
                        that._updateTitleContent()
                    }
                return isEmpty ? 0 : that.$titleDiv.width()
            },
            updateState: function(viewer, masterFilterValues) {
                var that = this;
                if (that.exportToMenu)
                    that.exportToMenu.setState(!!viewer.operations.exportTo);
                if (that.parametersButton)
                    that.parametersButton.setState({
                        loading: !!viewer.loading,
                        enabled: !!viewer.operations.reloadData
                    });
                if (masterFilterValues !== undefined)
                    that._updateMasterFilter(that.options.titleViewModel, masterFilterValues)
            },
            _updateMasterFilter: function(titleViewModel, masterFilterValues) {
                var that = this,
                    titleArguments = '',
                    singleFiltersCount = 0,
                    multipleFiltersCount = 0,
                    lastSingleFilter,
                    showFilterImage = false,
                    tooltip = that.titleTooltip;
                if (that._titleEnable() && titleViewModel.IncludeMasterFilterValues && masterFilterValues) {
                    if (masterFilterValues.length == 1 && masterFilterValues[0].Values.length == 1)
                        titleArguments = that._formatFilterValue(masterFilterValues[0].Values[0]);
                    if (!titleArguments)
                        showFilterImage = true
                }
                if (tooltip)
                    if (titleViewModel && titleViewModel.IncludeMasterFilterValues && masterFilterValues && showFilterImage) {
                        tooltip.masterFilterValues = masterFilterValues;
                        tooltip.refresh();
                        tooltip.enable()
                    }
                    else
                        tooltip.disable();
                that.titleShowFilterImage = showFilterImage;
                that.titleArguments = titleArguments;
                that._updateTitleContent()
            },
            _formatFilterValue: function(filterValue) {
                var separatorRange = ' - ',
                    formatter = dashboard.data.formatter,
                    rangeLeft,
                    rangeRight;
                if (filterValue.Value)
                    return formatter.format(filterValue.Value, filterValue.Format);
                else if (filterValue.RangeLeft && filterValue.RangeRight) {
                    rangeLeft = formatter.format(filterValue.RangeLeft, filterValue.Format);
                    rangeRight = formatter.format(filterValue.RangeRight, filterValue.Format);
                    return rangeLeft + separatorRange + rangeRight
                }
            },
            render: function(width, height) {
                if (height == undefined)
                    height = DEFAULT_TITLE_HEIGHT;
                var that = this,
                    titleViewModel = that.options.titleViewModel,
                    allowExport = that.options.allowExport,
                    encodeHtml = that.options.encodeHtml,
                    $container = that.options.$container,
                    imageHeight = undefined,
                    $image = undefined,
                    $titleText = undefined,
                    $titleCaption = undefined,
                    $toolbar = undefined,
                    $filterImage = undefined,
                    showParametersButton = titleViewModel ? titleViewModel.ShowParametersButton : false,
                    text = titleViewModel ? titleViewModel.Text : undefined,
                    imageViewModel = titleViewModel && titleViewModel.LayoutModel ? titleViewModel.LayoutModel.ImageViewModel : undefined;
                if (that._titleEnable()) {
                    that.$titleDiv = $('<div/>', {
                        'class': dashboard.titleClasses.root,
                        width: width,
                        height: height
                    }).appendTo($container);
                    if (imageViewModel) {
                        $image = $('<img>', {
                            'class': dashboard.titleClasses.image,
                            src: imageViewModel.Url ? imageViewModel.Url : 'data:' + imageViewModel.MimeType + ';base64,' + imageViewModel.SourceBase64String,
                            css: {visibility: 'hidden'}
                        });
                        that.$titleDiv.append($image)
                    }
                    $titleCaption = $('<div/>', {'class': dashboard.titleClasses.caption}).appendTo(that.$titleDiv);
                    $titleText = $('<span/>', {'class': dashboard.titleClasses.text}).appendTo($titleCaption);
                    utils.renderHelper.html($titleText, text, encodeHtml);
                    $('<span/>', {'class': dashboard.titleClasses.arguments}).appendTo($titleText);
                    $filterImage = $('<span/>', {'class': dashboard.titleClasses.filterImage}).appendTo($titleCaption);
                    that.titleTooltip = new dashboard.titleTooltip({
                        $control: $filterImage,
                        $targetContainer: $container,
                        formatFilterValue: that._formatFilterValue
                    });
                    if (allowExport || showParametersButton) {
                        $toolbar = $('<div/>', {'class': dashboard.titleClasses.toolbar}).appendTo(that.$titleDiv);
                        if (allowExport)
                            that.exportToMenu = new dashboard.dropDownMenu({
                                $parentContainer: $container,
                                $container: $toolbar,
                                className: 'dashboard-export',
                                elementNames: [localizer.getString(dashboard.localizationId.buttonNames.ExportToPdf), localizer.getString(dashboard.localizationId.buttonNames.ExportToImage)],
                                selectItem: function(index, elementName) {
                                    that.options.showExportDialog(index === 0 ? dashboard.exportFormats.pdf : dashboard.exportFormats.image)
                                },
                                title: localizer.getString(dashboard.localizationId.buttonNames.ExportTo),
                                enabled: true
                            });
                        if (showParametersButton)
                            that.parametersButton = new dashboard.parametersButton({
                                $container: $toolbar,
                                className: 'dx-icon-dashboard-parameters',
                                loadingClassName: dashboard.titleClasses.loading,
                                showParametersDialog: function() {
                                    that.options.showParametersDialog()
                                },
                                enabled: true
                            })
                    }
                    if ($image)
                        $image.load(function() {
                            imageHeight = $image.height();
                            if (imageHeight > height) {
                                $image.width(Math.floor($image.width() * (height / imageHeight)));
                                $image.height(height)
                            }
                            else
                                $image.css('margin-top', Math.ceil((height - imageHeight) / 2));
                            $image.css({visibility: 'visible'});
                            that._updateTitleContent()
                        });
                    that._updateTitleContent()
                }
                else
                    that.$titleDiv = undefined
            },
            _updateTitleContent: function() {
                var that = this,
                    titleViewModel = that.options.titleViewModel;
                if (that._titleEnable()) {
                    var titleArguments = that.titleArguments,
                        showFilterImage = that.titleShowFilterImage,
                        $argumentsText = that.$titleDiv.find('.' + dashboard.titleClasses.arguments),
                        $filterImage = that.$titleDiv.find('.' + dashboard.titleClasses.filterImage),
                        $image = that.$titleDiv.find('.' + dashboard.titleClasses.image),
                        $caption = that.$titleDiv.find('.' + dashboard.titleClasses.caption),
                        $text = that.$titleDiv.find('.' + dashboard.titleClasses.text),
                        $toolbar = that.$titleDiv.find('.' + dashboard.titleClasses.toolbar),
                        titleWidth = that.$titleDiv.innerWidth(),
                        imageWidth = $image && $image.length > 0 ? $image.outerWidth() : 0,
                        filterImageWidth = $filterImage && showFilterImage ? $filterImage.outerWidth() : 0,
                        toolbarWidth = $toolbar ? $toolbar.outerWidth() : 0,
                        captionMaxWidth = titleWidth - imageWidth - toolbarWidth - ($caption.outerWidth() - $caption.width()),
                        captionWidth,
                        spaceLeft,
                        offset;
                    if ($argumentsText)
                        if (titleArguments) {
                            $argumentsText.html(titleArguments);
                            $argumentsText.show()
                        }
                        else {
                            $argumentsText.html('');
                            $argumentsText.hide()
                        }
                    if ($filterImage)
                        if (showFilterImage)
                            $filterImage.show();
                        else
                            $filterImage.hide();
                    if ($text)
                        $text.css({'max-width': captionMaxWidth - filterImageWidth});
                    if ($caption)
                        $caption.css({'max-width': captionMaxWidth});
                    captionWidth = $caption ? $caption.outerWidth() : 0;
                    spaceLeft = titleWidth - imageWidth - (captionWidth - filterImageWidth);
                    offset = 2 * (toolbarWidth + filterImageWidth) <= spaceLeft ? Math.floor(spaceLeft / 2) : spaceLeft - toolbarWidth - filterImageWidth - 1;
                    if ($image)
                        $image.css({'margin-left': $image && $image.length > 0 && titleViewModel.LayoutModel && titleViewModel.LayoutModel.Alignment != 'Left' && offset > 0 ? offset : 0});
                    if ($caption)
                        $caption.css({'margin-left': !($image && $image.length > 0) && $caption && ($text && titleViewModel.Text || $argumentsText && titleArguments) && titleViewModel.LayoutModel && titleViewModel.LayoutModel.Alignment != 'Left' && offset > 0 ? offset : 0})
                }
            },
            _titleEnable: function() {
                return this.options.titleViewModel && this.options.titleViewModel.Visible
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file dashboardViewer.title.tooltip.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            Class = DX.require("/class"),
            tooltip = DX.require("/ui/ui.tooltip");
        var FILTER_LIST_SPACE = 8,
            DEFAULT_LINE_HEIGHT = 16;
        dashboard.titleTooltipClasses = {
            root: 'dx-dashboard-title-tooltip',
            list: 'dx-dashboard-title-tooltip-list',
            listText: 'dx-dashboard-title-tooltip-list-text',
            subList: 'dx-dashboard-title-tooltip-sublist',
            subListItem: 'dx-dashboard-title-tooltip-sublist-item'
        };
        dashboard.titleTooltip = Class.inherit({
            ctor: function ctor(options) {
                this.masterFilterValues = null;
                this.content = null;
                this.maxHeight = null;
                this.maxWidth = null;
                this.maxFilterListValues = null;
                this.$control = options.$control;
                this.$targetContainer = options.$targetContainer;
                this._formatFilterValue = options.formatFilterValue
            },
            enable: function() {
                var that = this,
                    $control = that.$control;
                if ($control) {
                    $control.off('dxclick', that._show);
                    $control.off('mouseenter', that._show);
                    $control.off('mouseleave', that._hide);
                    $control.on('dxclick', {context: that}, that._show);
                    $control.mouseenter({context: that}, that._show);
                    $control.mouseleave(that._hide)
                }
            },
            disable: function() {
                var that = this,
                    $control = that.$control;
                if ($control) {
                    $control.off('dxclick', that._show);
                    $control.off('mouseenter', that._show);
                    $control.off('mouseleave', that._hide)
                }
            },
            refresh: function() {
                var that = this;
                that.maxHeight = that._calcMaxHeight();
                that.maxWidth = that._calcMaxWidth();
                that.maxFilterListValues = that._calcMaxFilterListValues();
                that._refreshContent()
            },
            _calcMaxFilterValues: function(maxFilterListValues) {
                var that = this,
                    MIN_FILTER_VALUES = 4,
                    MAX_FILTER_LIST_VALUES = 100,
                    maxFilterValues,
                    curFilterListValues,
                    masterFilterValues = that.masterFilterValues;
                maxFilterListValues = Math.min(maxFilterListValues, MAX_FILTER_LIST_VALUES);
                for (maxFilterValues = Math.max(maxFilterListValues, MIN_FILTER_VALUES); maxFilterValues >= MIN_FILTER_VALUES; maxFilterValues--) {
                    if (maxFilterValues == MIN_FILTER_VALUES)
                        break;
                    curFilterListValues = 0;
                    $.each(masterFilterValues, function(index, dimensionFilterValues) {
                        curFilterListValues += (maxFilterValues < dimensionFilterValues.Values.length ? maxFilterValues : dimensionFilterValues.Values.length) + 1;
                        if (curFilterListValues > maxFilterListValues)
                            return false
                    });
                    if (curFilterListValues <= maxFilterListValues)
                        break
                }
                return maxFilterValues
            },
            _calcMaxHeight: function() {
                return Math.floor($(window).height() * 0.75)
            },
            _calcMaxWidth: function() {
                return Math.floor($(window).width() * 0.3)
            },
            _show: function(e) {
                var that = e.data.context,
                    $control = that.$control,
                    $targetContainer = that.$targetContainer,
                    masterFilterValues = that.masterFilterValues;
                if (!masterFilterValues || !$control || !$targetContainer) {
                    that.disable();
                    return
                }
                var newMaxHeight = that._calcMaxHeight(),
                    newMaxWidth = that._calcMaxWidth();
                if (newMaxHeight != that.maxHeight || newMaxWidth != that.maxWidth) {
                    that.maxHeight = newMaxHeight;
                    that.maxWidth = newMaxWidth;
                    var newMaxFilterListValues = that._calcMaxFilterListValues();
                    if (newMaxFilterListValues != that.maxFilterListValues) {
                        that.maxFilterListValues = newMaxFilterListValues;
                        that._refreshContent()
                    }
                }
                tooltip.show({
                    content: that.content,
                    position: {
                        my: 'bottom center',
                        at: 'top center'
                    },
                    target: $control,
                    container: $targetContainer,
                    animation: {hide: false}
                })
            },
            _hide: function() {
                tooltip.hide()
            },
            _refreshContent: function() {
                var that = this,
                    masterFilterValues = that.masterFilterValues;
                if (!masterFilterValues || masterFilterValues.length == 0)
                    return;
                var maxFilterValues = that._calcMaxFilterValues(that.maxFilterListValues);
                that._renderContent(maxFilterValues)
            },
            _renderContent: function(maxFilterValues) {
                var that = this,
                    $tooltip,
                    $tooltipList,
                    $tooltipListItem,
                    $tooltipSublist,
                    masterFilterValues = that.masterFilterValues;
                $tooltip = $('<div/>', {'class': dashboard.titleTooltipClasses.root});
                $tooltip.css({'max-width': that.maxWidth});
                $tooltipList = $('<ul/>', {'class': dashboard.titleTooltipClasses.list}).appendTo($tooltip);
                $.each(masterFilterValues, function(i, argument) {
                    $tooltipListItem = $('<li/>').appendTo($tooltipList);
                    $tooltipListItem.css({'padding-top': i == 0 ? 0 : FILTER_LIST_SPACE + 'px'});
                    $('<div/>', {'class': dashboard.titleTooltipClasses.listText}).appendTo($tooltipListItem).append(argument.Name);
                    $tooltipSublist = $('<ul/>', {'class': dashboard.titleTooltipClasses.subList}).appendTo($tooltipListItem);
                    for (var j = 0; j < maxFilterValues; j++) {
                        if (j >= argument.Values.length && argument.Truncated || j + 1 == maxFilterValues && (j + 1 < argument.Values.length || argument.Truncated)) {
                            $('<li/>', {'class': dashboard.titleTooltipClasses.subListItem}).appendTo($tooltipSublist).append("...");
                            break
                        }
                        if (j >= argument.Values.length)
                            break;
                        $('<li/>', {'class': dashboard.titleTooltipClasses.subListItem}).appendTo($tooltipSublist).append(that._formatFilterValue(argument.Values[j]))
                    }
                });
                that.content = $tooltip.wrapAll('<div>').parent().html()
            },
            _calcMaxFilterListValues: function() {
                var that = this,
                    masterFilterValues = that.masterFilterValues,
                    lineHeightString = $('<div/>', {'class': dashboard.titleTooltipClasses.root}).css('line-height'),
                    lineHeightTmp = parseInt(lineHeightString, 10),
                    lineHeight = lineHeightTmp && lineHeightString.length > 2 && lineHeightString.substr(lineHeightString.length - 2) == 'px' ? lineHeightTmp : DEFAULT_LINE_HEIGHT,
                    maxHeight = that.maxHeight;
                return Math.floor((maxHeight - (masterFilterValues.length - 1) * FILTER_LIST_SPACE) / lineHeight)
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file baseItem.js */
    (function($, DX, undefined) {
        var Class = DevExpress.require("/class"),
            commonUtils = DX.require("/utils/utils.common"),
            dashboard = DX.dashboard,
            formatter = dashboard.data.formatter,
            localizer = dashboard.data.localizer,
            viewerItems = dashboard.viewerItems,
            styleSettingsProvider = dashboard.styleSettingsProvider,
            utils = DX.utils;
        viewerItems.cssClassNames = {
            item: 'dx-dashboard-item',
            groupItem: 'dx-dashboard-group-item',
            groupItemChild: 'dx-dashboard-group-item-child',
            checked: 'dx-dashboard-checked',
            enabled: 'dx-dashboard-enabled',
            simpleBorder: 'dx-dashboard-simple-border',
            header: 'dx-dashboard-item-header',
            headerText: 'dx-dashboard-item-header-text',
            headerTextCaption: 'dx-dashboard-item-header-text-caption',
            headerTextParameters: 'dx-dashboard-item-header-text-parameters',
            hoverToolbar: 'dx-hover-toolbar-container',
            toolbar: 'dx-dashboard-item-toolbar',
            toolbarInternal: 'dx-db-toolbar-internal',
            iconClearMasterFilter: 'dx-icon-dashboard-clear-master-filter',
            iconClearSelection: 'dx-icon-dashboard-clear-selection',
            iconDrillUp: 'dx-icon-dashboard-drill-up',
            iconMultiselection: 'dx-icon-dashboard-toggle-multiselection'
        };
        dashboard.dashboardSelectionMode = {
            none: 'None',
            single: 'Single',
            multiple: 'Multiple'
        };
        dashboard.interactivityController = Class.inherit({
            ctor: function ctor(getTuples) {
                var that = this;
                that._getTuples = getTuples;
                that.selectionChanged = new $.Callbacks
            },
            clickAction: function(tuples) {
                if (this.selectionMode !== dashboard.dashboardSelectionMode.none) {
                    var that = this,
                        isMultipleMode = that.selectionMode === dashboard.dashboardSelectionMode.multiple,
                        currentTuples = isMultipleMode ? that._getTuples().slice() : [],
                        selectedTuples = [],
                        changed = false;
                    $.each(tuples, function(index, tuple) {
                        if (that._allowSelectTuple(tuple)) {
                            var index = isMultipleMode ? dashboard.utils.checkArrayContainsTuple(currentTuples, tuple) : undefined;
                            if (index == undefined)
                                selectedTuples.push(tuple);
                            else
                                currentTuples.splice(index, 1);
                            changed = true
                        }
                    });
                    if (changed)
                        that.selectionChanged.fire(currentTuples.concat(selectedTuples))
                }
            },
            setOptions: function(selectionMode) {
                this.selectionMode = selectionMode
            },
            _allowSelectTuple: function(tuple) {
                var allowSelect = true;
                $.each(tuple, function(_, axisValue) {
                    if (!dashboard.utils.allowSelectValue(axisValue.Value)) {
                        allowSelect = false;
                        return false
                    }
                });
                return allowSelect
            }
        });
        viewerItems.baseItem = Class.inherit({
            ContentType: {
                Empty: 'Empty',
                ViewModel: 'ViewModel',
                ActionModel: 'ActionModel',
                CompleteDataSource: 'CompleteDataSource',
                PartialDataSource: 'PartialDataSource',
                FullContent: 'FullContent'
            },
            initializeData: function(newOptions) {
                if (!this.options)
                    this.options = newOptions;
                else
                    this.options = {
                        Name: newOptions.Name,
                        Type: newOptions.Type,
                        Group: newOptions.Group,
                        ContentType: newOptions.ContentType,
                        SelectedValues: newOptions.SelectedValues,
                        ViewModel: newOptions.ViewModel || this.options.ViewModel,
                        ActionModel: newOptions.ActionModel || this.options.ActionModel,
                        CaptionViewModel: newOptions.CaptionViewModel || this.options.CaptionViewModel,
                        ConditionalFormattingModel: newOptions.ConditionalFormattingModel || this.options.ConditionalFormattingModel,
                        Parameters: newOptions.Parameters,
                        DrillDownValues: newOptions.DrillDownValues,
                        DrillDownUniqueValues: newOptions.DrillDownUniqueValues,
                        AxisNames: newOptions.AxisNames,
                        DimensionIds: newOptions.DimensionIds,
                        multiData: newOptions.multiData
                    };
                if (this.options.SelectedValues)
                    $.each(this.options.SelectedValues, function(_, selectedValue) {
                        dashboard.utils.cleanNullValues(selectedValue)
                    });
                if (!this.dataController || newOptions.ContentType != this.ContentType.ActionModel) {
                    var drillDownState = {};
                    drillDownState[this._getDrillDownAxisName()] = this.options.DrillDownUniqueValues;
                    this.dataController = dashboard.data.factory.createDataController(this.options.Type, {
                        multiData: this.options.multiData,
                        viewModel: this.options.ViewModel,
                        cfModel: this.options.ConditionalFormattingModel,
                        drillDownState: drillDownState,
                        selectedValues: this.options.SelectedValues
                    })
                }
                this.customSelectedTuples = []
            },
            ctor: function ctor($container, options) {
                this.initializeData(options);
                this.$container = $container;
                this._isFixedHeight = false;
                this._allowExport = options.allowExport;
                this._supportDataAwareExport = this._allowExport && options.ViewModel.SupportDataAwareExport;
                this._encodeHtml = options.encodeHtml == undefined ? true : options.encodeHtml;
                this._exportCaption = options.exportCaption;
                this._getExportDialog = options.getExportDialog;
                this.exportToMenu = undefined;
                this.selectElementMenu = undefined;
                this.$preloaderContainer = undefined;
                this.selected = new $.Callbacks;
                this.clearMasterFilter = new $.Callbacks;
                this.drillUp = new $.Callbacks;
                this.contentElementSelection = new $.Callbacks;
                this.expandValue = new $.Callbacks;
                this.clientStateUpdate = new $.Callbacks;
                this.dataRequest = new $.Callbacks;
                this.exportTo = new $.Callbacks;
                this.itemClick = new $.Callbacks;
                this.itemHover = new $.Callbacks;
                this.itemSelectionChanged = new $.Callbacks;
                this.itemWidgetCreated = new $.Callbacks;
                this.itemWidgetUpdating = new $.Callbacks;
                this.itemWidgetUpdated = new $.Callbacks;
                this.interactivityController = new dashboard.interactivityController($.proxy(this.getSelectedTuples, this));
                this.interactivityController.selectionChanged.add($.proxy(this.onSelectionChanged, this));
                this.customSelectionMode = dashboard.dashboardSelectionMode.none;
                this.customHoverEnabled = false;
                this.customTargetAxes = [];
                this.customDefaultSelectedValues = []
            },
            initialDataRequest: function(){},
            clearSelection: function(){},
            selectTuple: function(tuple, state){},
            setSelection: function(values) {
                this._setSelectedValues(values)
            },
            _getCustomSelectionMode: function() {
                return this.customSelectionMode
            },
            _setCustomSelectionMode: function(value) {
                this.customSelectionMode = value
            },
            _getCustomHoverEnabled: function() {
                return this.customHoverEnabled
            },
            _setCustomHoverEnabled: function(value) {
                this.customHoverEnabled = value
            },
            _getCustomTargetAxes: function() {
                return this.customTargetAxes
            },
            _setCustomTargetAxes: function(value) {
                this.customTargetAxes = value
            },
            _getCustomDefaultSelectedValues: function() {
                return this.customDefaultSelectedValues
            },
            _setCustomDefaultSelectedValues: function(value) {
                this.customDefaultSelectedValues = value
            },
            _getTargetAxes: function() {
                if (!this.isInteractivityActionEnabled())
                    return this._getCustomTargetAxes();
                else
                    return this._getAxisNames()
            },
            getSelectedTuples: function() {
                var that = this,
                    multiData = that.options.multiData,
                    axisNames = that._getAxisNames(),
                    dimensionByAxis = {},
                    tupleValues,
                    axisValues,
                    valueIndex;
                if (that._canSetMasterFilter() || that._canSetMultipleMasterFilter() || that._canPerformDrillDown()) {
                    var tuples = [];
                    if (that.options.SelectedValues == null)
                        return tuples;
                    if (axisNames.length > 1) {
                        $.each(axisNames, function(_, axisName) {
                            dimensionByAxis[axisName] = multiData.getAxis(axisName).getDimensions()
                        });
                        $.each(that.options.SelectedValues, function(_, selection) {
                            tupleValues = [];
                            valueIndex = 0;
                            $.each(axisNames, function(_, axisName) {
                                axisValues = [];
                                $.each(dimensionByAxis[axisName], function() {
                                    axisValues.push(selection[valueIndex++])
                                });
                                tupleValues.push({
                                    AxisName: axisName,
                                    Value: axisValues
                                })
                            });
                            tuples.push(tupleValues)
                        })
                    }
                    else {
                        var drillDownValues = that._getDrillDownValues();
                        $.each(that.options.SelectedValues, function(indexd, value) {
                            tuples.push([{
                                    AxisName: axisNames[0],
                                    Value: drillDownValues.concat(value)
                                }])
                        })
                    }
                    return tuples
                }
                else
                    return that.customSelectedTuples
            },
            updateItem: function(options) {
                var that = this;
                this._setCustomSelectionMode(options.selectionMode);
                this._setCustomTargetAxes(options.targetAxes);
                this._setCustomHoverEnabled(options.hoverEnabled);
                this._setCustomDefaultSelectedValues(options.defaultSelectedValues);
                that.updateInteractivityOptions();
                if (!this.isInteractivityActionEnabled()) {
                    var customDefaultSelectedValues = options.selectionMode == dashboard.dashboardSelectionMode.single ? that.customDefaultSelectedValues.slice(0, 1) : that.customDefaultSelectedValues;
                    that.interactivityController.clickAction(customDefaultSelectedValues)
                }
                this._updateToolbar()
            },
            _changeTuple: function(tuple) {
                var that = this,
                    newTuple = [];
                $.each(tuple, function(index, axisValue) {
                    var axisName = axisValue.AxisName,
                        value = axisValue.Value;
                    newTuple.push({
                        AxisName: axisName,
                        Value: axisName == that._getDrillDownAxisName() ? value.slice(-1) : value
                    })
                });
                return newTuple
            },
            onSelectionChanged: function(tuples) {
                var that = this,
                    currentSelectedValues = that.options.SelectedValues,
                    singleSelection = !!currentSelectedValues && currentSelectedValues.length === 1 ? currentSelectedValues[0] : null,
                    newSelection = [],
                    tupleValues,
                    selectedTuples = that.getSelectedTuples(),
                    actionName,
                    selectedDrillDownValue;
                that._selectTuples(tuples, selectedTuples, true);
                that._selectTuples(selectedTuples, tuples, false);
                if (that.itemSelectionChanged)
                    that.itemSelectionChanged.fire(that._getName(), tuples);
                if (that.isInteractivityActionEnabled()) {
                    $.each(tuples, function(index, tuple) {
                        tupleValues = [];
                        $.each(that._getAxisNames(), function(_, axisName) {
                            tupleValues.push.apply(tupleValues, dashboard.utils.getAxisPointValue(tuple, axisName).slice())
                        });
                        newSelection.push(tupleValues)
                    });
                    newSelection = that._deductDrillDownValues(newSelection);
                    that._setSelectedValues(newSelection);
                    actionName = that._getSelectionCallbackType(newSelection, singleSelection);
                    if (actionName === dashboard.viewerActions.drillDown)
                        newSelection = newSelection[0];
                    if (that._mustSelectingFired(newSelection))
                        that.selected.fire(that._getName(), actionName, newSelection);
                    else
                        that._onClearMasterFilter()
                }
                else {
                    that.customSelectedTuples = [];
                    $.each(tuples, function(index, tuple) {
                        that.customSelectedTuples.push(tuple)
                    })
                }
                if (that.customSelectionMode == dashboard.dashboardSelectionMode.multiple)
                    this._updateToolbar()
            },
            _mustSelectingFired: function(values) {
                return values.length > 0
            },
            _patchTroughDrillDownValues: function(values) {
                var drillDownValues = this._getDrillDownValues(),
                    filterValues = [];
                if (values)
                    $.each(values, function(_, value) {
                        filterValues.push(drillDownValues.concat(value))
                    });
                return filterValues
            },
            _deductDrillDownValues: function(values) {
                var drillDownValues = this._getDrillDownValues(),
                    drillDownValuesLength = drillDownValues.length,
                    cutValue;
                $.each(values, function(_, value) {
                    cutValue = value.slice(0, drillDownValuesLength);
                    if (dashboard.utils.checkValuesAreEqual(cutValue, drillDownValues))
                        value.splice(0, drillDownValuesLength)
                });
                return values
            },
            _getSelectionCallbackType: function(values, singleSelection) {
                var actionName = undefined,
                    viewerActions = dashboard.viewerActions,
                    selectionEquals = values.length === 1 && singleSelection && dashboard.utils.checkValuesAreEqual(values[0], singleSelection);
                if (this._canSetMultipleMasterFilter() && this.itemMultiselectionEnabled)
                    actionName = viewerActions.setMultipleValuesMasterFilter;
                else if (this._canSetMasterFilter())
                    if (selectionEquals) {
                        if (this._canPerformDrillDown())
                            actionName = viewerActions.drillDown
                    }
                    else
                        actionName = viewerActions.setMasterFilter;
                else if (this._canPerformDrillDown())
                    actionName = viewerActions.drillDown;
                return actionName
            },
            _selectTuples: function(tuplesToSelect, unaffectedTuples, isSelect) {
                var that = this;
                $.each(tuplesToSelect, function(index, tuple) {
                    if (dashboard.utils.checkArrayContainsTuple(unaffectedTuples, tuple) == undefined)
                        that.selectTuple(that._hasDrillUpButton() && !that._isMultiDataSupported() ? that._changeTuple(tuple) : tuple, isSelect)
                })
            },
            _renderFooter: function() {
                return undefined
            },
            _updateFooter: function(){},
            renderContent: function(changeExisting){},
            renderPartialContent: function(){},
            updateContentState: function(){},
            getInfo: function() {
                var that = this,
                    $container = that.$container;
                return {
                        name: that._getName(),
                        headerHeight: that.$header ? that.$header.innerHeight() : undefined,
                        position: $container.position(),
                        width: $container.outerWidth(),
                        height: $container.outerHeight(),
                        virtualSize: undefined,
                        scroll: undefined
                    }
            },
            getCaption: function() {
                return this.options.CaptionViewModel ? this.options.CaptionViewModel.Caption : undefined
            },
            hasCaption: function(options) {
                var opts = options || this.options;
                return opts && opts.CaptionViewModel && opts.CaptionViewModel.ShowCaption
            },
            hasGroup: function() {
                return this.options && commonUtils.isDefined(this.options.Group)
            },
            isPaneEmpty: function() {
                return this.hasGroup()
            },
            render: function($container) {
                var that = this;
                that.$container = that.$container || $container;
                if (that.$container) {
                    $.each(that._getOuterBorderClasses(), function(_, className) {
                        that.$container.addClass(className)
                    });
                    that.$container.attr('data-layout-item-name', this._getName())
                }
                that.$contentRoot = $('<div/>');
                $.each(that._getInnerBorderClasses(), function(_, className) {
                    that.$contentRoot.addClass(className)
                });
                dashboard.utils.moveContent(that.$container, that.$contentRoot, true);
                that.$container.append(that.$contentRoot);
                that.$header = that._renderHeader(that.$container);
                that.$container.append(that._renderFooter());
                that._updateContentSize();
                that._changeContent(false)
            },
            updateContent: function(newOptions) {
                var isPrevShowCaption = this.hasCaption(this.options),
                    isNewShowCaption = this.hasCaption(newOptions);
                this.initializeData(newOptions);
                if (commonUtils.isDefined(isNewShowCaption) && isPrevShowCaption !== isNewShowCaption) {
                    if (this.$header) {
                        this.$header.remove();
                        delete this.$header;
                        this.$header = undefined
                    }
                    this.$header = this._renderHeader(this.$container);
                    this._updateContentSize()
                }
                else {
                    this._updateHeader();
                    this._updateFooter()
                }
                switch (newOptions.ContentType) {
                    case this.ContentType.PartialDataSource:
                    case this.ContentType.CompleteDataSource:
                        if (newOptions.DataSource || newOptions.ItemData && newOptions.ItemData.DataStorageDTO)
                            this.renderPartialContent();
                        break;
                    case this.ContentType.ActionModel:
                        if (!this.isInteractivityActionEnabled())
                            this.updateContentState();
                        break;
                    default:
                        this._changeContent(true);
                        break
                }
            },
            updateClientState: function(clientState) {
                if (this.options.ContentType !== this.ContentType.PartialDataSource && this.options.ContentType !== this.ContentType.CompleteDataSource)
                    this._updateClientStateInternal(clientState)
            },
            updateState: function(state) {
                if (!this.$container)
                    return;
                var that = this,
                    position = that._getContainerPosition(),
                    preloaderPosition = undefined,
                    $preloader = that.$preloader,
                    $shieldingElement = that.$shieldingElement;
                if (!!state.loading) {
                    if (!$preloader) {
                        $preloader = $('<div/>', {'class': 'dx-dashboard-item-loading'});
                        if (that.$preloaderContainer) {
                            that.$preloaderContainer.css('background-image', 'none');
                            if (that.itemMultiselectionEnabled)
                                that.$preloaderContainer.removeClass(viewerItems.cssClassNames.checked)
                        }
                        preloaderPosition = that._getPreloaderPosition(position);
                        $preloader.css({
                            left: preloaderPosition.left,
                            top: preloaderPosition.top,
                            'z-index': 100,
                            position: 'absolute'
                        });
                        that.$container.prepend($preloader);
                        that.$preloader = $preloader
                    }
                }
                else if ($preloader) {
                    $preloader.remove();
                    if (this.$preloaderContainer)
                        this.$preloaderContainer.removeAttr('style');
                    delete that.$preloader
                }
                if (!!state.operations.actions) {
                    if ($shieldingElement) {
                        $shieldingElement.remove();
                        delete that.$shieldingElement
                    }
                }
                else if (!$shieldingElement) {
                    $shieldingElement = $('<div/>', {'class': 'dx-dashboard-item-shield'});
                    $shieldingElement.css({
                        left: position.left,
                        top: position.top,
                        width: position.width,
                        height: position.height
                    });
                    that.$shieldingElement = $shieldingElement;
                    that.$container.prepend($shieldingElement)
                }
                if (that.exportToMenu)
                    that.exportToMenu.setState(!!state.operations.exportTo)
            },
            width: function(width) {
                var that = this;
                if ($.isNumeric(width))
                    that.setSize(width, undefined);
                else
                    return that.$container.outerWidth()
            },
            height: function(height) {
                var that = this;
                if ($.isNumeric(height))
                    that.setSize(undefined, height);
                else
                    return that.$container.outerHeight()
            },
            setSize: function(width, height) {
                var that = this,
                    oldSize = {
                        width: that.width(),
                        height: that.height()
                    },
                    newSize = {
                        width: width,
                        height: height
                    };
                if (width)
                    that.$container.outerWidth(width);
                if (height)
                    that.$container.outerHeight(height);
                that._resize(oldSize, newSize)
            },
            getConstraints: function(includeBorders) {
                var borderSize = includeBorders ? dashboard.utils.getBorderSizeByClasses(this._getOuterBorderClasses().concat(this._getInnerBorderClasses())) : {
                        width: 0,
                        height: 0
                    },
                    headerHeight = this._calcHeaderAndFooterHeight(true),
                    contentMinHeight = this._getMinContentHeight(),
                    toolbarWidth = this.$header ? this.$header.find('.' + viewerItems.cssClassNames.toolbar).outerWidth() : 0,
                    height = borderSize.height + headerHeight + contentMinHeight;
                return new dashboard.layout.utils.constraints(new dashboard.layout.utils.size(toolbarWidth + borderSize.width + dashboard.MIN_PANE_WIDTH, height), new dashboard.layout.utils.size(Number.MAX_VALUE, this._isFixedHeight ? height : Number.MAX_VALUE))
            },
            getOffset: function() {
                return {
                        width: 0,
                        height: 0
                    }
            },
            updateInteractivityOptions: function() {
                var that = this,
                    selectionMode = dashboard.dashboardSelectionMode.none;
                if (!that.isInteractivityActionEnabled()) {
                    that.updateContentState();
                    selectionMode = this.customSelectionMode;
                    if (selectionMode == dashboard.dashboardSelectionMode.multiple && !this.itemMultiselectionEnabled)
                        selectionMode = dashboard.dashboardSelectionMode.single
                }
                else if (this._canSetMultipleMasterFilter() && this.itemMultiselectionEnabled)
                    selectionMode = dashboard.dashboardSelectionMode.multiple;
                else if (that.isInteractivityActionEnabled())
                    selectionMode = dashboard.dashboardSelectionMode.single;
                that.interactivityController.setOptions(selectionMode)
            },
            _updateClientStateInternal: function(clientState){},
            _changeContent: function(updateExisting) {
                if (updateExisting)
                    this._raiseItemWidgetUpdating();
                this.renderContent(updateExisting);
                if (updateExisting)
                    this._raiseItemWidgetUpdated();
                else
                    this._raiseItemWidgetCreated()
            },
            _renderHeader: function($container) {
                if (this.hasCaption()) {
                    var caption = this.getCaption(),
                        $headerDiv = $('<div/>', {'class': viewerItems.cssClassNames.header}),
                        $headerTextDiv = $('<div/>', {'class': viewerItems.cssClassNames.headerText}),
                        $headerTextSpan = $('<span/>', {'class': viewerItems.cssClassNames.headerTextCaption});
                    if ($container) {
                        $container.prepend($headerDiv);
                        $headerDiv.append(this._ensureToolbarIsRendered())
                    }
                    if (caption) {
                        $headerDiv.append($headerTextDiv);
                        $headerTextDiv.append($headerTextSpan);
                        utils.renderHelper.html($headerTextSpan, caption, this._encodeHtml);
                        $('<span/>', {'class': viewerItems.cssClassNames.headerTextParameters}).appendTo($headerTextDiv)
                    }
                    return $headerDiv
                }
                else {
                    if ($container)
                        this._renderFloatingToolbar($container);
                    return undefined
                }
            },
            _calcHeaderAndFooterHeight: function(forceNonRendered) {
                var headerAndFooterHeight = 0;
                if (this.$header)
                    headerAndFooterHeight += this.$header.height();
                else if (forceNonRendered)
                    headerAndFooterHeight += utils.renderHelper.getElementBox(this._renderHeader()).height;
                if (this.$footer)
                    headerAndFooterHeight += this.$footer.height();
                return headerAndFooterHeight
            },
            _updateHeader: function() {
                var that = this,
                    caption = that.getCaption(),
                    drillDownValues = that.options.DrillDownValues || [],
                    parameters = '',
                    separator = ' - ';
                if (that.$header && that.hasCaption() && caption) {
                    that.$header.find('.' + viewerItems.cssClassNames.headerTextCaption).html(caption);
                    $.each(drillDownValues, function(index, drillDownValue) {
                        if (drillDownValue)
                            parameters += separator + formatter.format(drillDownValue.Value, drillDownValue.Format)
                    });
                    that.$header.find('.' + viewerItems.cssClassNames.headerTextParameters).html(parameters)
                }
                that._updateToolbar()
            },
            _renderFloatingToolbar: function($container) {
                var that = this,
                    $hoverDiv = $container.find('.' + viewerItems.cssClassNames.hoverToolbar);
                if ($hoverDiv.length === 0) {
                    $hoverDiv = $('<div/>', {'class': viewerItems.cssClassNames.hoverToolbar});
                    $hoverDiv.css({
                        position: 'absolute',
                        visibility: 'hidden',
                        'z-index': 99
                    });
                    $hoverDiv.append(that._ensureToolbarIsRendered());
                    $container.prepend($hoverDiv)
                }
                $container.hover(function() {
                    var position = that._getContainerPosition();
                    if (that.toolbarIsVisible)
                        $hoverDiv.css({
                            left: position.left + position.offsetX - that.$toolbarDiv.outerWidth(true),
                            top: position.top + position.offsetY,
                            visibility: 'visible'
                        })
                }, function() {
                    if ($hoverDiv)
                        $hoverDiv.css({visibility: 'hidden'})
                })
            },
            _ensureToolbarIsRendered: function() {
                var that = this,
                    $parentContainer = that.$container,
                    contentDescription = that.options.ViewModel ? that.options.ViewModel.ContentDescription : undefined,
                    hasClearMasterFilterButton = that._hasClearMasterFilterButton(),
                    hasClearSelectionButton = that._hasClearSelectionButton(),
                    hasToggleSelectionModeButton = that._hasToggleSelectionModeButton(),
                    hasElementSelectionButton = contentDescription && contentDescription.ElementSelectionEnabled,
                    hasInternalButtons = that._hasInternalButtons(),
                    hasDrillUp = that._hasDrillUpButton(),
                    hasExportTo = that._allowExport,
                    supportDataAwareExport = that._supportDataAwareExport,
                    $internalToolbarDiv = undefined,
                    $headerDrillUp,
                    $imageDiv;
                that.toolbarIsVisible = hasInternalButtons || hasClearMasterFilterButton || hasToggleSelectionModeButton || hasElementSelectionButton || hasDrillUp || hasExportTo;
                if (!that.$toolbarDiv) {
                    that.$toolbarDiv = $('<div/>', {'class': viewerItems.cssClassNames.toolbar});
                    $internalToolbarDiv = $('<div/>', {'class': viewerItems.cssClassNames.toolbarInternal});
                    that.$toolbarDiv.append($internalToolbarDiv)
                }
                else
                    $internalToolbarDiv = that.$toolbarDiv.find('.' + viewerItems.cssClassNames.toolbarInternal);
                if (hasInternalButtons)
                    that.$preloaderContainer = that._ensureInternalToolbarIsRendered($internalToolbarDiv);
                if (hasExportTo) {
                    if (!that.exportToMenu) {
                        var elementNames = [localizer.getString(dashboard.localizationId.buttonNames.ExportToPdf), localizer.getString(dashboard.localizationId.buttonNames.ExportToImage)];
                        if (supportDataAwareExport)
                            elementNames.push(localizer.getString(dashboard.localizationId.buttonNames.ExportToExcel));
                        that.exportToMenu = new dashboard.dropDownMenu({
                            $parentContainer: $parentContainer,
                            $container: $internalToolbarDiv,
                            className: 'dashboard-item-export',
                            elementNames: elementNames,
                            selectItem: function(index, elementName) {
                                var exportFormat;
                                switch (index) {
                                    case 0:
                                        exportFormat = dashboard.exportFormats.pdf;
                                        break;
                                    case 1:
                                        exportFormat = dashboard.exportFormats.image;
                                        break;
                                    default:
                                        exportFormat = dashboard.exportFormats.excel;
                                        break
                                }
                                that._showExportDialog(exportFormat)
                            },
                            title: localizer.getString(dashboard.localizationId.buttonNames.ExportTo),
                            enabled: true
                        })
                    }
                    that.$preloaderContainer = that.exportToMenu.getButtonImageDiv()
                }
                if (hasElementSelectionButton) {
                    if (!that.selectElementMenu)
                        that.selectElementMenu = new dashboard.dropDownMenu({
                            $parentContainer: $parentContainer,
                            $container: $internalToolbarDiv,
                            className: 'dashboard-content-selection',
                            elementNames: contentDescription.ElementNames,
                            selectedIndex: contentDescription.SelectedElementIndex,
                            selectItem: function(index, elementName) {
                                that._onContentElementSelection(index, elementName)
                            },
                            title: localizer.getString(dashboard.localizationId.buttonNames.ElementSelection),
                            enabled: true
                        });
                    that.$preloaderContainer = that.selectElementMenu.getButtonImageDiv()
                }
                $imageDiv = $internalToolbarDiv.find('.' + viewerItems.cssClassNames.iconClearMasterFilter);
                if (hasClearMasterFilterButton) {
                    if (!$imageDiv || $imageDiv.length == 0)
                        $imageDiv = $('<div/>', {
                            'class': viewerItems.cssClassNames.iconClearMasterFilter,
                            title: localizer.getString(dashboard.localizationId.buttonNames.ClearMasterFilter)
                        }).appendTo($internalToolbarDiv);
                    $imageDiv.off('click', that._getClearMasterFilterHandler());
                    if (that._isClearMasterFilterEnabled()) {
                        $imageDiv.addClass(viewerItems.cssClassNames.enabled);
                        $imageDiv.on('click', that._getClearMasterFilterHandler())
                    }
                    else
                        $imageDiv.removeClass(viewerItems.cssClassNames.enabled);
                    that.$preloaderContainer = $imageDiv
                }
                else
                    $imageDiv.remove();
                $imageDiv = $internalToolbarDiv.find('.' + viewerItems.cssClassNames.iconClearSelection);
                if (hasClearSelectionButton) {
                    if (!$imageDiv || $imageDiv.length == 0)
                        $imageDiv = $('<div/>', {
                            'class': viewerItems.cssClassNames.iconClearSelection,
                            title: localizer.getString(dashboard.localizationId.buttonNames.ClearSelection)
                        }).appendTo($internalToolbarDiv);
                    $imageDiv.off('click', that._getClearSelectionHandler());
                    if (that._isClearSelectionEnabled()) {
                        $imageDiv.addClass(viewerItems.cssClassNames.enabled);
                        $imageDiv.on('click', that._getClearSelectionHandler())
                    }
                    else
                        $imageDiv.removeClass(viewerItems.cssClassNames.enabled);
                    that.$preloaderContainer = $imageDiv
                }
                else
                    $imageDiv.remove();
                $headerDrillUp = $internalToolbarDiv.find('.' + viewerItems.cssClassNames.iconDrillUp);
                if (hasDrillUp) {
                    if (!$headerDrillUp || $headerDrillUp.length == 0)
                        $headerDrillUp = $('<div/>', {
                            'class': viewerItems.cssClassNames.iconDrillUp,
                            title: localizer.getString(dashboard.localizationId.buttonNames.DrillUp)
                        }).appendTo($internalToolbarDiv);
                    $headerDrillUp.off('click', that._getDrillUpHandler());
                    if (that._isDrillUpEnabled()) {
                        $headerDrillUp.addClass(viewerItems.cssClassNames.enabled);
                        $headerDrillUp.on('click', that._getDrillUpHandler())
                    }
                    else
                        $headerDrillUp.removeClass(viewerItems.cssClassNames.enabled);
                    that.$preloaderContainer = $headerDrillUp
                }
                else
                    $headerDrillUp.remove();
                $imageDiv = $internalToolbarDiv.find('.' + viewerItems.cssClassNames.iconMultiselection);
                if (hasToggleSelectionModeButton) {
                    if (!$imageDiv || $imageDiv.length == 0) {
                        $imageDiv = $('<div/>', {
                            'class': viewerItems.cssClassNames.iconMultiselection,
                            title: localizer.getString(dashboard.localizationId.buttonNames.AllowMultiselection)
                        }).appendTo($internalToolbarDiv);
                        $imageDiv.on('click', that._getToggleSelectionModeHandler())
                    }
                    if (that.itemMultiselectionEnabled)
                        $imageDiv.addClass(viewerItems.cssClassNames.checked);
                    else
                        $imageDiv.removeClass(viewerItems.cssClassNames.checked);
                    that.$preloaderContainer = $imageDiv
                }
                else
                    $imageDiv.remove();
                return that.$toolbarDiv
            },
            _ensureInternalToolbarIsRendered: function($internalToolbarDiv){},
            _updateToolbar: function() {
                this._ensureToolbarIsRendered()
            },
            _showExportDialog: function(format) {
                var that = this,
                    exportCaption = that._exportCaption,
                    exportDialog = that._getExportDialog();
                exportDialog.setExportFunction(function(documentInfo) {
                    that._onExportTo({
                        mode: 'SingleItem',
                        format: format,
                        documentOptions: documentInfo,
                        fileName: documentInfo.fileName
                    })
                });
                exportDialog.showDialog(that._getName(), that.options.Type, format, {
                    caption: exportCaption,
                    fileName: exportCaption
                })
            },
            _getMinContentHeight: function() {
                return dashboard.MIN_PANE_HEIGHT
            },
            _getInnerBorderClasses: function() {
                var classes = [viewerItems.cssClassNames.item];
                if (this._isBorderRequired())
                    classes.push(viewerItems.cssClassNames.simpleBorder);
                return classes
            },
            _getOuterBorderClasses: function() {
                return this.hasGroup() ? [viewerItems.cssClassNames.groupItemChild] : []
            },
            _isBorderRequired: function() {
                return this.isPaneEmpty()
            },
            _resize: function(oldSize, newSize) {
                this._updateContentSize();
                this._allocatePreloader()
            },
            _updateContentSize: function() {
                var containerHeightWithoutPaddings = this.$container.height(),
                    subAreasHeight = this._calcHeaderAndFooterHeight(false);
                var contentHeight = Math.floor(containerHeightWithoutPaddings - subAreasHeight);
                this.$contentRoot.outerHeight(contentHeight)
            },
            _allocatePreloader: function() {
                if (!this.$preloader)
                    return;
                var that = this,
                    $container = that.$container,
                    $shieldingElement = $container.find('.dx-dashboard-item-shield'),
                    position = that._getContainerPosition(),
                    preloaderPosition = that._getPreloaderPosition(position);
                that.$preloader.css({
                    left: preloaderPosition.left,
                    top: preloaderPosition.top
                });
                $shieldingElement.css(position)
            },
            _getPreloaderPosition: function(containerPosition) {
                var buttonOffset = this._getButtonOffset();
                return {
                        left: containerPosition.left + containerPosition.offsetX - buttonOffset,
                        top: containerPosition.top + containerPosition.offsetY + 6
                    }
            },
            _getButtonOffset: function() {
                return 28
            },
            _getAnimationOptions: function() {
                return {
                        enabled: !!this.options.animate,
                        duration: 300
                    }
            },
            _getContainerPosition: function() {
                var that = this,
                    position = that.$container.position(),
                    width = that.$container.outerWidth(),
                    height = that.$container.outerHeight(),
                    marginX = that.$container.css('margin-left'),
                    marginY = that.$container.css('margin-top'),
                    border = that.isPaneEmpty() ? 0 : 1,
                    parseMargin = function(margin) {
                        return margin == 'auto' ? 0 : parseInt(margin)
                    };
                return {
                        left: position.left,
                        top: position.top,
                        width: width,
                        height: height,
                        offsetX: width + parseMargin(marginX) - border,
                        offsetY: parseMargin(marginY) - border
                    }
            },
            _getName: function() {
                return this.options.Name
            },
            _getSelectedValues: function() {
                var selectedValues = this.options.SelectedValues;
                return this._isMultiDataSupported() ? this._patchTroughDrillDownValues(selectedValues) : selectedValues
            },
            _getClearMasterFilterHandler: function() {
                var that = this;
                that._clearMasterFilterHandler = that._clearMasterFilterHandler || function() {
                    that._onClearMasterFilter()
                };
                return that._clearMasterFilterHandler
            },
            _getClearSelectionHandler: function() {
                var that = this;
                that._clearSelectionHandler = that._clearSelectionHandler || function() {
                    that.clearSelection();
                    that.customSelectedTuples = [];
                    if (that.customSelectionMode == dashboard.dashboardSelectionMode.multiple)
                        that._updateToolbar();
                    that.itemSelectionChanged.fire(that._getName(), [])
                };
                return that._clearSelectionHandler
            },
            _getElementInteractionValue: function(element, viewModel){},
            _getToggleSelectionModeHandler: function() {
                var that = this;
                that._toggleSelectionModeHandler = that._toggleSelectionModeHandler || function() {
                    that._onToggleSelectionMode()
                };
                return that._toggleSelectionModeHandler
            },
            _getDrillUpHandler: function() {
                var that = this;
                that._drillUpHandler = that._drillUpHandler || function() {
                    that._onDrillUp()
                };
                return that._drillUpHandler
            },
            _setSelectedValues: function(values) {
                this.options.SelectedValues = values;
                return this.options.SelectedValues
            },
            _raiseItemClick: function(element) {
                var that = this,
                    tuple = [],
                    dataPoint = that._getDataPoint(element),
                    drillDownValues = that._getDrillDownValues(),
                    targetAxes = that._getTargetAxes(),
                    drillDownAxis = drillDownValues && targetAxes.length == 1 ? targetAxes[0] : undefined;
                $.each(targetAxes, function(_, axisName) {
                    var values;
                    if (dataPoint.getSelectionValues)
                        values = dataPoint.getSelectionValues(axisName);
                    else
                        values = dataPoint.getValues(axisName);
                    if (values.length > 0) {
                        if (drillDownAxis && axisName === drillDownAxis && !that._isMultiDataSupported())
                            values = drillDownValues.concat(values);
                        tuple.push({
                            AxisName: axisName,
                            Value: values
                        })
                    }
                });
                if (targetAxes.length != 0 && targetAxes.length == tuple.length)
                    that.interactivityController.clickAction([tuple]);
                if (that.itemClick)
                    that.itemClick.fire(that._getName(), dataPoint)
            },
            _isMultiDataSupported: function() {
                return false
            },
            _getDataPoint: function(element) {
                return null
            },
            _getWidget: function() {
                return null
            },
            _raiseItemWidgetCreated: function() {
                var widget = this._getWidget();
                if (widget)
                    this.itemWidgetCreated.fire(this._getName(), widget)
            },
            _raiseItemWidgetUpdating: function() {
                var widget = this._getWidget();
                if (widget)
                    this.itemWidgetUpdating.fire(this._getName(), widget)
            },
            _raiseItemWidgetUpdated: function() {
                var widget = this._getWidget();
                if (widget)
                    this.itemWidgetUpdated.fire(this._getName(), widget)
            },
            _raiseItemHover: function(element, state) {
                if (this.itemHover) {
                    var dataPoint = this._getDataPoint(element);
                    this.itemHover.fire(this._getName(), dataPoint, state)
                }
            },
            _allowCombineSelectedValues: function(actionName) {
                return actionName === dashboard.viewerActions.setMultipleValuesMasterFilter
            },
            _onClearMasterFilter: function() {
                var name = this._getName();
                this._setSelectedValues(null);
                this.clearMasterFilter.fire(name);
                this.clearSelection()
            },
            _onToggleSelectionMode: function() {
                this.itemMultiselectionEnabled = !this.itemMultiselectionEnabled;
                this._updateToolbar();
                this.updateInteractivityOptions()
            },
            _onDrillUp: function() {
                this.drillUp.fire(this._getName(), !!this._getSelectedValues())
            },
            _onContentElementSelection: function(index, elementName) {
                this.contentElementSelection.fire(this._getName(), {
                    index: index,
                    caption: elementName
                })
            },
            _onExpandValue: function(expandValueParams) {
                this.expandValue.fire(this._getName(), expandValueParams)
            },
            _onClientStateUpdate: function(clientState) {
                this.clientStateUpdate.fire(this._getName(), clientState)
            },
            _onDataRequest: function() {
                this.dataRequest.fire(this._getName())
            },
            _onExportTo: function(exportParams) {
                $.extend(exportParams, {itemType: this.options.Type});
                this.exportTo.fire(this._getName(), exportParams)
            },
            _hasInternalButtons: function() {
                return false
            },
            _hasDrillUpButton: function() {
                var actionModel = this.options.ActionModel;
                return actionModel && actionModel.DrillUpButtonState && actionModel.DrillUpButtonState !== 'Hidden'
            },
            _hasClearMasterFilterButton: function() {
                var actionModel = this.options.ActionModel;
                return actionModel && actionModel.ClearMasterFilterButtonState && actionModel.ClearMasterFilterButtonState !== 'Hidden'
            },
            _hasClearSelectionButton: function() {
                return !this.isInteractivityActionEnabled() && this.customSelectionMode == dashboard.dashboardSelectionMode.multiple
            },
            _hasToggleSelectionModeButton: function() {
                return this._canSetMultipleMasterFilter() || !this.isInteractivityActionEnabled() && this.customSelectionMode == dashboard.dashboardSelectionMode.multiple
            },
            _isDrillUpEnabled: function() {
                var actionModel = this.options.ActionModel;
                return actionModel && actionModel.DrillUpButtonState && actionModel.DrillUpButtonState === 'Enabled'
            },
            _isClearMasterFilterEnabled: function() {
                var actionModel = this.options.ActionModel;
                return actionModel && actionModel.ClearMasterFilterButtonState && actionModel.ClearMasterFilterButtonState === 'Enabled'
            },
            _isClearSelectionEnabled: function() {
                return this.customSelectedTuples.length > 0
            },
            _canPerformAction: function(action) {
                var actionModel = this.options.ActionModel;
                return actionModel && actionModel.Actions && $.inArray(action, actionModel.Actions) != -1
            },
            _canPerformDrillDown: function() {
                return this._canPerformAction(dashboard.viewerActions.drillDown)
            },
            _canSetMasterFilter: function() {
                return this._canPerformAction(dashboard.viewerActions.setMasterFilter)
            },
            _canSetMultipleMasterFilter: function() {
                return this._canPerformAction(dashboard.viewerActions.setMultipleValuesMasterFilter)
            },
            isInteractivityActionEnabled: function() {
                return this._canSetMasterFilter() || this._canSetMultipleMasterFilter() || this._canPerformDrillDown()
            },
            _selectionMode: function() {
                return this.isInteractivityActionEnabled() ? 'multiple' : 'none'
            },
            _getHtml: function(text) {
                return this._encodeHtml ? dashboard.utils.encodeHtml(text) : text
            },
            _getAxisNames: function() {
                return this.options.AxisNames || []
            },
            _getDrillDownAxisName: function() {
                return this._getAxisNames().length > 0 ? this._getAxisNames()[0] : undefined
            },
            _getDrillDownValues: function() {
                var drillDownValues = this.options.DrillDownUniqueValues;
                return drillDownValues != null ? drillDownValues : []
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file groupItem.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            viewerItems = dashboard.viewerItems;
        viewerItems.groupItem = viewerItems.baseItem.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options)
            },
            renderContent: function(changeExisting) {
                this.$contentRoot.addClass(viewerItems.cssClassNames.groupItem)
            },
            getOffset: function() {
                var borderSize = dashboard.utils.getBorderSizeByClasses([viewerItems.cssClassNames.groupItem]);
                return {
                        width: borderSize.width,
                        height: borderSize.height + this._calcHeaderAndFooterHeight(true)
                    }
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file widgetViewerItem.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            viewerItems = dashboard.viewerItems;
        viewerItems.widgetViewerItem = viewerItems.baseItem.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options)
            },
            clearSelection: function() {
                if (this.widgetsViewer)
                    this.widgetsViewer.clearSelections()
            },
            getInfo: function() {
                return $.extend(true, this.callBase(), this.widgetsViewer.getSizeParams())
            },
            renderContent: function(changeExisting) {
                var options = this._getWidgetViewerOptions();
                if (changeExisting && this.widgetsViewer)
                    this.widgetsViewer.option(options);
                else
                    this.widgetsViewer = new dashboard.widgetsViewer.dxWidgetsViewer(this.$contentRoot, options);
                this._applySelection()
            },
            _getContainerPosition: function() {
                var that = this,
                    position = that.callBase(),
                    itemInfo = !that.$headerDiv ? that.getInfo() : undefined,
                    scrollSize = itemInfo && itemInfo.scroll && itemInfo.scroll.vertical ? itemInfo.scroll.size : 0;
                position.offsetX -= scrollSize;
                return position
            },
            _getSpecificWidgetViewerOptions: function() {
                return {itemOptions: {encodeHtml: this._encodeHtml}}
            },
            _getWidgetType: function() {
                return
            },
            _isHoverEnabled: function() {
                return this._selectionMode() !== 'none'
            },
            _configureHover: function(selectionValues) {
                var result = {};
                result.hoverEnabled = selectionValues !== null && this._isHoverEnabled() && dashboard.utils.allowSelectValue(selectionValues);
                result.cursor = result.hoverEnabled ? "pointer" : "default";
                return result
            },
            _getWidgetViewerOptions: function() {
                var viewModel = this.options.ViewModel,
                    contentDescription = viewModel ? viewModel.ContentDescription : undefined,
                    commonOptions = {viewer: {
                            redrawOnResize: false,
                            useNativeScrolling: dashboard.USE_NATIVE_SCROLLING
                        }};
                if (this.dataController)
                    this.dataController.setSourceItemProperties = $.proxy(this._setSourceItemProperties, this);
                commonOptions.dataSource = this._getDataSource();
                commonOptions.viewer.onclick = this._getOnClickHandler();
                commonOptions.viewer.onhover = this._getOnHoverHandler();
                commonOptions.viewer.widgetType = this._getWidgetType();
                commonOptions.viewer.method = contentDescription ? this._convertContentArrangementMode(contentDescription.ArrangementMode) : 'auto';
                commonOptions.viewer.count = contentDescription ? contentDescription.LineCount : 1;
                return $.extend(true, commonOptions, this._getSpecificWidgetViewerOptions())
            },
            _getDataSource: function() {
                if (this.dataController)
                    return this.dataController.getDataSource()
            },
            _getElementInteractionValue: function(element) {
                return element.tag
            },
            _getOnClickHandler: function() {
                var that = this;
                return function(e) {
                        that._raiseItemClick(e.item)
                    }
            },
            _getOnHoverHandler: function() {
                var that = this;
                return function(e) {
                        that._raiseItemHover(e.item, e.state)
                    }
            },
            _convertContentArrangementMode: function(contentArrangementMode) {
                switch (contentArrangementMode) {
                    case'FixedColumnCount':
                        return "column";
                    case'FixedRowCount':
                        return "row";
                    default:
                        return "auto"
                }
            },
            _resize: function(oldSize, newSize) {
                this.callBase(oldSize, newSize);
                this.widgetsViewer.redraw()
            },
            updateContentState: function() {
                var that = this;
                $.each(this.widgetsViewer.itemsList, function(index, viewer) {
                    viewer._hoverEnabled = that._getCustomHoverEnabled()
                })
            },
            _setSourceItemProperties: function(sourceItem, elementModel, props){},
            _isMultiDataSupported: function() {
                return true
            },
            _applySelection: function(){}
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file kpiItem.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            utils = DX.utils,
            viewerItems = dashboard.viewerItems;
        viewerItems.kpiItem = viewerItems.widgetViewerItem.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options)
            },
            renderContent: function(changeExisting) {
                this.callBase(changeExisting);
                this.$contentRoot.addClass('dx-dashboard-widget-viewer-item')
            },
            _showTitle: function() {
                return true
            },
            _getElementsName: function(){},
            selectTuple: function(tuple, state) {
                $.each(this.widgetsViewer.itemsList, function(index, viewer) {
                    if (dashboard.utils.checkValuesAreEqual(viewer.tag, tuple[0].Value))
                        if (state)
                            viewer.select();
                        else
                            viewer.clearSelection()
                })
            },
            setSelection: function(values) {
                this.callBase(values);
                this.clearSelection();
                $.each(this.widgetsViewer.itemsList, function(index, viewer) {
                    $.each(values, function() {
                        if (dashboard.utils.checkValuesAreEqual(viewer.tag, this))
                            viewer.select()
                    })
                })
            },
            _getDataPoint: function(element) {
                var that = this,
                    viewModel = that.options.ViewModel,
                    elementTag = element.tag,
                    titleValues = elementTag ? elementTag : [],
                    elementIndex = elementTag ? 0 : element.index,
                    elViewModel = viewModel[that._getElementsName()][elementIndex];
                return {
                        getValues: function(name) {
                            return name == 'Default' ? titleValues : null
                        },
                        getDeltaIds: function() {
                            return elViewModel.DataItemType === 'Delta' ? [elViewModel.ID] : []
                        },
                        getMeasureIds: function() {
                            return elViewModel.DataItemType === 'Measure' ? [elViewModel.ID] : []
                        },
                        getSelectionValues: function() {
                            return elementTag
                        }
                    }
            },
            _isMultiDataSupported: function() {
                return true
            },
            _setSourceItemProperties: function(sourceItem, elementModel, props) {
                var selectionValues = props.getSelectionValues(),
                    serverSelection = this.options.SelectedValues,
                    currentLine,
                    captions = props.getCaptions(),
                    isSelected = function() {
                        if (serverSelection && selectionValues)
                            for (var i = 0; i < serverSelection.length; i++) {
                                currentLine = serverSelection[i];
                                if (dashboard.utils.checkValuesAreEqual(selectionValues, currentLine))
                                    return true
                            }
                        return false
                    };
                if (this._hasDrillUpButton())
                    captions = captions.slice(-1);
                $.extend(sourceItem, this._configureHover(selectionValues));
                sourceItem.tag = selectionValues;
                sourceItem.isSelected = isSelected();
                this._setTitle(sourceItem, captions)
            },
            _setTitle: function(sourceItem, captions){}
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file cardsItem.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            viewerItems = dashboard.viewerItems,
            utils = dashboard.utils;
        viewerItems.cardsItem = viewerItems.kpiItem.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options);
                this._hasSparkline = false
            },
            renderContent: function(changeExisting) {
                this._hasSparkline = false;
                this.callBase(changeExisting)
            },
            isPaneEmpty: function() {
                return this.callBase() || !this.hasCaption()
            },
            _isBorderRequired: function() {
                return false
            },
            _getSpecificWidgetViewerOptions: function() {
                var that = this,
                    specificOptions = {
                        viewer: {ignorePadding: this.isPaneEmpty()},
                        itemOptions: {hasSparkline: this._hasSparkline}
                    };
                return $.extend(true, specificOptions, that.callBase())
            },
            _getWidgetType: function() {
                return 'card'
            },
            _getElementsName: function() {
                return 'Cards'
            },
            _setSourceItemProperties: function(sourceItem, cardModel, props) {
                this.callBase(sourceItem, cardModel, props);
                var captions = props.getCaptions(),
                    sparklineOptions,
                    indicatorType,
                    isGood;
                if (cardModel.DataItemType === utils.KpiValueMode.Measure)
                    sourceItem.mainValue = {
                        type: undefined,
                        hasPositiveMeaning: undefined,
                        text: {
                            value: props.getMeasureDisplayText(),
                            useDefaultColor: cardModel.IgnoreDeltaColor
                        }
                    };
                else {
                    indicatorType = props.getIndicatorType();
                    isGood = props.getIsGood();
                    sourceItem.mainValue = {
                        type: indicatorType,
                        hasPositiveMeaning: isGood,
                        text: {
                            value: props.getMainValueText(),
                            useDefaultColor: cardModel.IgnoreDeltaColor
                        }
                    };
                    sourceItem.variableValue1 = {
                        type: indicatorType,
                        hasPositiveMeaning: isGood,
                        text: {
                            value: props.getSubValue1Text(),
                            useDefaultColor: cardModel.IgnoreSubValue1DeltaColor
                        }
                    };
                    sourceItem.variableValue2 = {
                        type: indicatorType,
                        hasPositiveMeaning: isGood,
                        text: {
                            value: props.getSubValue2Text(),
                            useDefaultColor: cardModel.IgnoreSubValue2DeltaColor
                        }
                    }
                }
                if (cardModel.ShowSparkline) {
                    sparklineOptions = props.getSparklineOptions();
                    if (sparklineOptions) {
                        sourceItem.sparklineOptions = sparklineOptions;
                        this._hasSparkline = true
                    }
                }
            },
            _setTitle: function(sourceItem, captions) {
                if (captions.length > 0) {
                    sourceItem.title = captions.pop();
                    sourceItem.subTitle = captions.join(' - ')
                }
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file gaugesItem.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            formatter = dashboard.data.formatter,
            viewerItems = dashboard.viewerItems,
            utils = dashboard.utils;
        viewerItems.gaugesItem = viewerItems.kpiItem.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options)
            },
            _getSpecificWidgetViewerOptions: function() {
                var that = this,
                    viewModel = that.options.ViewModel,
                    specificOptions = {itemOptions: {ignoreProportions: true}};
                if (viewModel)
                    switch (viewModel.ViewType) {
                        case utils.gaugeViewType.CircularHalf:
                            specificOptions.itemOptions.proportions = 0.85;
                            specificOptions.itemOptions.minWidth = 155;
                            break;
                        case utils.gaugeViewType.CircularQuarterLeft:
                        case utils.gaugeViewType.CircularQuarterRight:
                            specificOptions.itemOptions.proportions = 1.25;
                            specificOptions.itemOptions.minWidth = 150;
                            break;
                        case utils.gaugeViewType.CircularThreeFourth:
                        case utils.gaugeViewType.CircularFull:
                            specificOptions.itemOptions.proportions = 1;
                            specificOptions.itemOptions.minWidth = 180;
                            break;
                        case utils.gaugeViewType.LinearVertical:
                            specificOptions.itemOptions.proportions = 1.5;
                            specificOptions.itemOptions.minWidth = 150;
                            break;
                        case utils.gaugeViewType.LinearHorizontal:
                            specificOptions.itemOptions.proportions = 0.5;
                            specificOptions.itemOptions.minWidth = 200;
                            break
                    }
                return $.extend(true, specificOptions, that.callBase())
            },
            _getWidgetType: function() {
                var viewModel = this.options.ViewModel;
                if (viewModel)
                    switch (viewModel.ViewType) {
                        case utils.gaugeViewType.LinearVertical:
                        case utils.gaugeViewType.LinearHorizontal:
                            return 'lineargauge';
                        default:
                            return 'circulargauge'
                    }
                return 'circulargauge'
            },
            _getElementsName: function() {
                return 'Gauges'
            },
            _showTitle: function() {
                if (this.options.ViewModel)
                    return this.options.ViewModel.ShowGaugeCaptions;
                else
                    return this.callBase()
            },
            _getWidget: function() {
                var that = this,
                    gaugeWidgetViewers = this.widgetsViewer.itemsList,
                    gaugeList = [];
                $.each(gaugeWidgetViewers, function() {
                    gaugeList.push(this._widget)
                });
                return gaugeList
            },
            _setSourceItemProperties: function(sourceItem, gaugeModel, props) {
                this.callBase(sourceItem, gaugeModel, props);
                var captions = props.getCaptions(),
                    range = props.getGaugeRange(),
                    targetValue;
                this._setVisualProperties(sourceItem, gaugeModel, range);
                if (gaugeModel.DataItemType === utils.KpiValueMode.Measure) {
                    sourceItem.value = props.getMeasureValue();
                    sourceItem.indicator = {text: {
                            value: props.getMeasureDisplayText(),
                            useDefaultColor: gaugeModel.IgnoreDeltaColor
                        }}
                }
                else {
                    sourceItem.value = props.getActualValue();
                    targetValue = props.getTargetValue();
                    if (targetValue)
                        sourceItem.subvalues = [targetValue];
                    sourceItem.indicator = {
                        type: props.getIndicatorType(),
                        hasPositiveMeaning: props.getIsGood(),
                        text: {
                            value: props.getMainValueText(),
                            useDefaultColor: gaugeModel.IgnoreDeltaColor
                        }
                    }
                }
            },
            _setVisualProperties: function(sourceItem, gaugeModel, range) {
                var that = this,
                    viewModel = that.options.ViewModel,
                    minRangeValue = range.min,
                    maxRangeValue = range.max,
                    width = maxRangeValue - minRangeValue,
                    intervalCount = range.majorTickCount - 1,
                    tickInterval;
                switch (viewModel.ViewType) {
                    case utils.gaugeViewType.CircularHalf:
                        sourceItem.geometry = {
                            startAngle: 180,
                            endAngle: 0
                        };
                        break;
                    case utils.gaugeViewType.CircularQuarterLeft:
                        sourceItem.geometry = {
                            startAngle: 180,
                            endAngle: 90
                        };
                        break;
                    case utils.gaugeViewType.CircularQuarterRight:
                        sourceItem.geometry = {
                            startAngle: 90,
                            endAngle: 0
                        };
                        break;
                    case utils.gaugeViewType.CircularThreeFourth:
                        sourceItem.geometry = {
                            startAngle: 220,
                            endAngle: 320
                        };
                        break;
                    case utils.gaugeViewType.CircularFull:
                        sourceItem.geometry = {
                            startAngle: 240,
                            endAngle: 300
                        };
                        break;
                    case utils.gaugeViewType.LinearVertical:
                        sourceItem.geometry = {orientation: 'vertical'};
                        break;
                    case utils.gaugeViewType.LinearHorizontal:
                        sourceItem.geometry = {orientation: 'horizontal'};
                        break
                }
                sourceItem.valueIndicator = {type: sourceItem.geometry.orientation ? 'rangebar' : 'twocolorneedle'};
                sourceItem.subvalueIndicator = {offset: sourceItem.geometry.orientation ? 8 : 0};
                sourceItem.scale = {
                    startValue: minRangeValue,
                    endValue: maxRangeValue,
                    label: {format: formatter.defaultNumericFormat}
                };
                if (width > 0) {
                    tickInterval = width / intervalCount;
                    sourceItem.scale.majorTick = {
                        tickInterval: width >= intervalCount ? Math.round(tickInterval) : tickInterval,
                        useTicksAutoArrangement: false
                    }
                }
                sourceItem.animation = that._getAnimationOptions()
            },
            _setTitle: function(sourceItem, captions) {
                if (captions.length > 0 && this._showTitle())
                    sourceItem.title = {
                        text: captions.join(' - '),
                        font: {size: 16},
                        subtitle: {font: {size: 14}},
                        margin: {
                            top: 4,
                            left: 0,
                            right: 0,
                            bottom: 0
                        }
                    }
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file pieItem.js */
    (function($, DX, undefined) {
        var Class = DevExpress.require("/class"),
            dashboard = DX.dashboard,
            data = DX.dashboard.data,
            utils = DX.utils,
            stringUtils = DX.require("/utils/utils.string"),
            chartHelper = data.chartHelper,
            selectionHelper = dashboard.data.selectionHelper,
            viewerItems = dashboard.viewerItems;
        viewerItems.pieItem = viewerItems.widgetViewerItem.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options);
                this._createPieMouseEventController();
                this.itemElementCustomColor = $.Callbacks()
            },
            initializeData: function(newOptions) {
                this.callBase(newOptions);
                if (this.dataController)
                    this.dataController.elementCustomColor = $.proxy(this._elementCustomColor, this)
            },
            clearSelection: function() {
                var that = this,
                    viewModel = that.options.ViewModel,
                    piesViewer = that.widgetsViewer;
                that.callBase();
                if (piesViewer && viewModel && viewModel.SelectionEnabled && viewModel.SelectionMode !== chartHelper.SelectionMode.Series)
                    $.each(piesViewer.itemsList, function() {
                        this._widget.clearSelection()
                    })
            },
            updateContentState: function() {
                var that = this;
                if (that._getCustomHoverEnabled()) {
                    var argumentHoverMode = 'none',
                        seriesHoverEnabled = false,
                        targetAxes = this._getTargetAxes();
                    if (targetAxes.length == 1)
                        if (targetAxes[0] == dashboard.itemDataAxisNames.chartArgumentAxis)
                            argumentHoverMode = 'allArgumentPoints';
                        else
                            seriesHoverEnabled = true;
                    else if (targetAxes.length == 2) {
                        argumentHoverMode = 'point';
                        seriesHoverEnabled = true
                    }
                    $.each(this.widgetsViewer.itemsList, function(index, viewer) {
                        viewer._widget.option('commonSeriesSettings.hoverMode', argumentHoverMode);
                        viewer._hoverEnabled = seriesHoverEnabled
                    })
                }
            },
            selectTuple: function(tuple, state) {
                var that = this,
                    isPointSelection = that.options.ViewModel.SelectionMode === chartHelper.SelectionMode.Points,
                    seriesValue = dashboard.utils.getAxisPointValue(tuple, dashboard.itemDataAxisNames.chartSeriesAxis),
                    argumentValue = dashboard.utils.getAxisPointValue(tuple, dashboard.itemDataAxisNames.chartArgumentAxis);
                $.each(that.widgetsViewer.itemsList, function(index, viewer) {
                    if (seriesValue) {
                        if (selectionHelper._checkWidgetCorrespondsToValue(viewer, seriesValue))
                            if (argumentValue)
                                selectionHelper.setSelectedArguments(viewer._widget, [argumentValue], state);
                            else if (isPointSelection)
                                selectionHelper.selectWholePie(viewer._widget, state);
                            else
                                selectionHelper.setSelectedWidgetViewer(viewer, [seriesValue], state)
                    }
                    else if (argumentValue)
                        selectionHelper.setSelectedArguments(viewer._widget, [argumentValue], state)
                })
            },
            setSelection: function(values) {
                this.callBase(values);
                var that = this,
                    tuples = that.getSelectedTuples();
                that.clearSelection();
                $.each(tuples, function(_, tuple) {
                    that.selectTuple(tuple, true)
                })
            },
            _elementCustomColor: function(eventArgs) {
                this.itemElementCustomColor.fire(this._getName(), eventArgs)
            },
            _createPieMouseEventController: function() {
                var that = this;
                that.pieMouseEventController = new viewerItems.pieItem.pieMouseEventController;
                that.pieMouseEventController.ready.add(function() {
                    var data = {
                            pie: that.pieMouseEventController.pieData,
                            slice: that.pieMouseEventController.sliceData
                        };
                    that._raiseItemClick(data)
                })
            },
            _isHoverEnabled: function() {
                return this.callBase() && this._isItemSelectionEnabled()
            },
            _isItemSelectionEnabled: function() {
                var viewModel = this.options.ViewModel;
                return viewModel && viewModel.SelectionEnabled && viewModel.SelectionMode === chartHelper.SelectionMode.Series
            },
            _isLabelsVisible: function() {
                var viewModel = this.options.ViewModel;
                return viewModel && viewModel.LabelContentType !== 'None'
            },
            _getSpecificWidgetViewerOptions: function() {
                var that = this,
                    specificOptions = that._isLabelsVisible() ? {itemOptions: {
                            minWidth: 200,
                            proportions: 0.75,
                            ignoreProportions: true
                        }} : {itemOptions: {
                            minWidth: 100,
                            proportions: 1,
                            ignoreProportions: true
                        }};
                return $.extend(true, specificOptions, that.callBase())
            },
            _getWidgetType: function() {
                return 'pieChart'
            },
            _getDataSource: function() {
                if (!this.options.ViewModel)
                    return {};
                var that = this,
                    viewModel = that.options.ViewModel,
                    isPointSelectionEnabled = that._getPointSelectionEnabled(),
                    selectionMode = that._selectionMode(),
                    seriesAxisPoints = that.dataController.getSeriesAxisPoints(),
                    selectedValuesList = that._getSelectedValues(),
                    dataSource = [],
                    dataSourceItem,
                    currentSeriesPath,
                    seriesPropsValues;
                $.each(seriesAxisPoints, function(_, seriesAxisPoint) {
                    currentSeriesPath = seriesAxisPoint.getUniquePath();
                    $.each(that.dataController.getValueDataMembers(), function(valueIndex, valueDataMember) {
                        dataSourceItem = {
                            animation: that._getAnimationOptions(),
                            legend: {visible: false},
                            resolveLabelOverlapping: "shift",
                            onIncidentOccurred: utils.renderHelper.widgetIncidentOccurred,
                            onPointClick: that._getSelectPointsHandler(),
                            onPointHoverChanged: that._getHoverPointsHandler(),
                            palette: utils.renderHelper.getDefaultPalette(),
                            pointSelectionMode: "multiple",
                            tag: {axisPoint: seriesAxisPoint},
                            commonSeriesSettings: {hoverMode: isPointSelectionEnabled && selectionMode !== 'none' ? 'allArgumentPoints' : 'none'},
                            customizePoint: function() {
                                var result = {color: that.dataController.getColor(this.tag.axisPoint, seriesAxisPoint, that._getMeasuresIds(this.tag), this.tag.colorMeasureId)};
                                if (!dashboard.utils.allowSelectValue(that._getElementInteractionValue(this, that.options.ViewModel)))
                                    result.hoverStyle = {hatching: 'none'};
                                return result
                            }
                        };
                        $.extend(dataSourceItem, that._configureHover(currentSeriesPath));
                        if (selectedValuesList && that._isItemSelectionEnabled())
                            $.each(selectedValuesList, function(__, selectedValue) {
                                if (dashboard.utils.checkValuesAreEqual(currentSeriesPath, selectedValue))
                                    dataSourceItem.isSelected = true
                            });
                        if (viewModel.ShowPieCaptions)
                            dataSourceItem.title = {
                                text: that.dataController.getValueDisplayNames(seriesAxisPoint, valueIndex),
                                font: {size: 18}
                            };
                        seriesPropsValues = {
                            type: viewModel.PieType === 'Donut' ? 'doughnut' : 'pie',
                            argumentField: 'x',
                            valueField: 'y',
                            label: {
                                visible: that._isLabelsVisible(),
                                position: 'columns'
                            },
                            point: {visible: true},
                            segmentsDirection: 'anticlockwise',
                            paintNullPoints: true
                        };
                        if (seriesPropsValues.label.visible) {
                            seriesPropsValues.label.connector = {
                                visible: true,
                                width: 1
                            };
                            seriesPropsValues.label.customizeText = that._getFormatLabelHandler(viewModel.LabelContentType)
                        }
                        dataSourceItem.series = [seriesPropsValues];
                        dataSourceItem.dataSource = that.dataController.createDataSource(seriesAxisPoint, valueDataMember);
                        dataSourceItem.tooltip = {enabled: viewModel.TooltipContentType !== 'None'};
                        if (dataSourceItem.tooltip.enabled) {
                            dataSourceItem.tooltip.container = dashboard.utils.tooltipContainerSelector;
                            dataSourceItem.tooltip.customizeTooltip = function(label) {
                                return {text: that._getFormatLabelHandler(viewModel.TooltipContentType)(label)}
                            };
                            dataSourceItem.tooltip.font = {size: 14};
                            dataSourceItem.tooltip.zIndex = 100
                        }
                        dataSource.push(dataSourceItem)
                    })
                });
                return dataSource.length == 1 && dataSource[0].dataSource.length == 0 ? [] : dataSource
            },
            _getPointSelectionEnabled: function() {
                var viewModel = this.options.ViewModel;
                return viewModel.SelectionEnabled && (viewModel.SelectionMode === chartHelper.SelectionMode.Argument || viewModel.SelectionMode === chartHelper.SelectionMode.Points)
            },
            _getFormatLabelHandler: function(valueType) {
                var that = this;
                return function(label) {
                        var pointTexts = that.dataController.getPointDisplayTexts(label.point.tag, label.value, label.percent),
                            tooltipPattern = that._getTooltipPattern(valueType);
                        switch (valueType) {
                            case'Argument':
                                return pointTexts.argumentText;
                            case'Percent':
                                return pointTexts.percentText;
                            case'Value':
                                return pointTexts.valueText;
                            case'ValueAndPercent':
                                return stringUtils.format(tooltipPattern, pointTexts.valueText, pointTexts.percentText);
                            case'ArgumentAndPercent':
                                return stringUtils.format(tooltipPattern, pointTexts.argumentText, pointTexts.percentText);
                            case'ArgumentAndValue':
                                return stringUtils.format(tooltipPattern, pointTexts.argumentText, pointTexts.valueText);
                            case'ArgumentValueAndPercent':
                                return stringUtils.format(tooltipPattern, pointTexts.argumentText, pointTexts.valueText, pointTexts.percentText);
                            default:
                                return ''
                        }
                    }
            },
            _getTooltipPattern: function(valueType) {
                switch (valueType) {
                    case'ValueAndPercent':
                        return '{0} ({1})';
                    case'ArgumentAndPercent':
                    case'ArgumentAndValue':
                        return '{0}: {1}';
                    case'ArgumentValueAndPercent':
                        return '{0}: {1} ({2})';
                    default:
                        return ''
                }
            },
            _getElementInteractionValue: function(element) {
                if (this._isItemSelectionEnabled())
                    return this.callBase(element);
                return element.tag
            },
            _getOnClickHandler: function() {
                var that = this;
                return function(e) {
                        that._pieMouseEventHandler(e.item)
                    }
            },
            _getSelectPointsHandler: function() {
                var that = this;
                return function(e) {
                        var viewModel = that.options.ViewModel,
                            selectionMode = that._selectionMode(),
                            isPointSelectionEnalbed = viewModel.SelectionEnabled && viewModel.SelectionMode === chartHelper.SelectionMode.Argument && selectionMode !== 'none';
                        that._sliceMouseEventHandler(e.target)
                    }
            },
            _getOnHoverHandler: function() {
                var that = this;
                return function(e) {
                        that.pieMouseEventController.pieData = e.item;
                        that._raiseItemHover({pie: e.item}, e.state)
                    }
            },
            _getHoverPointsHandler: function() {
                var that = this;
                return function(e) {
                        that._raiseItemHover({
                            pie: that.pieMouseEventController.pieData,
                            slice: e.target
                        })
                    }
            },
            _pieMouseEventHandler: function(element) {
                this.pieMouseEventController.setPieData(element)
            },
            _sliceMouseEventHandler: function(element) {
                this.pieMouseEventController.setSliceData(element)
            },
            _getDataPoint: function(element) {
                var that = this,
                    viewModel = that.options.ViewModel,
                    slice = element.slice,
                    sliceTag = slice ? slice.tag : undefined,
                    pie = element.pie,
                    pieTag = pie ? pie.tag : undefined,
                    argumentsValues = sliceTag ? dashboard.utils.getTagValue(sliceTag) : [],
                    titleValues = pieTag ? dashboard.utils.getTagValue(pieTag) : [],
                    argumentIndex = slice && !sliceTag ? slice.index : undefined,
                    elementIndex = pie && !pieTag ? pie.index : undefined,
                    measureIndex = argumentIndex ? argumentIndex : elementIndex ? elementIndex : 0;
                return {
                        getValues: function(name) {
                            switch (name) {
                                case dashboard.itemDataAxisNames.chartArgumentAxis:
                                    return argumentsValues;
                                case dashboard.itemDataAxisNames.chartSeriesAxis:
                                    return titleValues;
                                default:
                                    return null
                            }
                        },
                        getDeltaIds: function() {
                            return []
                        },
                        getMeasureIds: function() {
                            return slice ? that._getMeasuresIds(slice.tag) : []
                        }
                    }
            },
            _getMeasuresIds: function(sliceTag) {
                return sliceTag ? sliceTag.dataMembers : []
            },
            _isMultiDataSupported: function() {
                return true
            },
            _getWidget: function() {
                var pieWidgetViewers = this.widgetsViewer.itemsList,
                    piesList = [];
                $.each(pieWidgetViewers, function() {
                    piesList.push(this._widget)
                });
                return piesList
            },
            _applySelection: function() {
                var that = this,
                    tuples = that.getSelectedTuples();
                $.each(tuples, function(_, tuple) {
                    that.selectTuple(tuple, true)
                })
            }
        });
        viewerItems.pieItem.pieMouseEventController = Class.inherit({
            ctor: function ctor() {
                this.pieData = undefined;
                this.sliceData = undefined;
                this.shouldRaise = false;
                this.ready = new $.Callbacks
            },
            setPieData: function(data) {
                this.pieData = data;
                this.shouldRaise = true;
                this._onValueSet()
            },
            setSliceData: function(data) {
                this.sliceData = data;
                this.shouldRaise = true;
                this._onValueSet()
            },
            _onValueSet: function() {
                var that = this;
                window.setTimeout(function() {
                    if (!that.shouldRaise)
                        return;
                    if (that.ready)
                        that.ready.fire();
                    that.pieData = undefined;
                    that.sliceData = undefined;
                    that.shouldRaise = false
                }, 100)
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file pivotGridItem.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            formatter = dashboard.data.formatter,
            localizer = dashboard.data.localizer,
            styleSettingsProvider = dashboard.styleSettingsProvider,
            viewerItems = dashboard.viewerItems,
            PIVOT_BAR_ID = 'pivotBar';
        viewerItems.pivotGridItem = viewerItems.baseItem.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options);
                this._collapseStateCache = {};
                this._conditionalFormattingInfoCache = [];
                this._dataRequestProcessing = false;
                this._styleSettingsProvider = new styleSettingsProvider;
                this._styleSettingsProvider.initialize($container, this.options.ConditionalFormattingModel)
            },
            renderContent: function(changeExisting) {
                if (this.options) {
                    var pivotOptions = this._getPivotGridOptions();
                    if (changeExisting && this.pivotGridViewer)
                        this.pivotGridViewer.option(pivotOptions);
                    else
                        this.pivotGridViewer = new DX.ui.dxPivotGrid(this.$contentRoot, pivotOptions);
                    this._dataRequestProcessing = false;
                    this._conditionalFormattingInfoCache = []
                }
            },
            render: function($container) {
                this._styleSettingsProvider.update($container);
                this.callBase($container)
            },
            renderPartialContent: function() {
                var parameters = this.options.Parameters,
                    isColumn = parameters[1],
                    area = isColumn ? 'column' : 'row',
                    path = parameters[0],
                    pivotDataSource = this.dataController.getDataSource(isColumn, path);
                this._dataRequestProcessing = false;
                this._conditionalFormattingInfoCache = [];
                this.pivotGridViewer.applyPartialDataSource(area, path, pivotDataSource)
            },
            getInfo: function() {
                return $.extend(true, this.callBase(), {scroll: {
                            topPath: this.pivotGridViewer.getScrollPath('row'),
                            leftPath: this.pivotGridViewer.getScrollPath('column'),
                            horizontal: this.pivotGridViewer.hasScroll('column'),
                            vertical: this.pivotGridViewer.hasScroll('row')
                        }})
            },
            getExpandingState: function() {
                var ds = this.pivotGridViewer ? this.pivotGridViewer.getDataSource() : undefined;
                return {
                        rows: ds && ds.state() ? ds.state().rowExpandedPaths : [],
                        columns: ds && ds.state() ? ds.state().columnExpandedPaths : []
                    }
            },
            initializeData: function(newOptions) {
                this.callBase(newOptions);
                if (this._styleSettingsProvider)
                    this._styleSettingsProvider.initialize(this.$contentRoot, this.options.ConditionalFormattingModel)
            },
            _getPivotGridOptions: function() {
                if (!this.options.ViewModel)
                    return {};
                var viewModel = this.options.ViewModel,
                    commonOptions = {};
                commonOptions.fieldChooser = {enabled: false};
                commonOptions.loadPanel = false;
                commonOptions.contextMenuEnabled = false;
                commonOptions.showRowGrandTotals = viewModel.ShowRowGrandTotals;
                commonOptions.showColumnGrandTotals = viewModel.ShowColumnGrandTotals;
                commonOptions.showColumnTotals = viewModel.ShowColumnTotals;
                commonOptions.showRowTotals = viewModel.ShowRowTotals;
                commonOptions.dataSource = this.dataController.getDataSource();
                commonOptions.encodeHtml = this._encodeHtml;
                commonOptions.scrolling = {
                    mode: 'standard',
                    useNative: dashboard.USE_NATIVE_SCROLLING
                };
                commonOptions.onExpandValueChanging = this._getExpandValueChangingHandler();
                commonOptions.texts = {
                    grandTotal: localizer.getString(dashboard.localizationId.PivotGridGrandTotal),
                    total: localizer.getString(dashboard.localizationId.PivotGridTotal),
                    noData: localizer.getString(dashboard.localizationId.MessagePivotHasNoData)
                };
                commonOptions.onCellClick = this._getCellClickHandler();
                commonOptions.onCellPrepared = this._getCellPreparedHandler();
                commonOptions.onContentReady = this._getContentReadyHandler();
                return commonOptions
            },
            _createHeaderHierarchy: function(list) {
                var result = [],
                    tempList = [],
                    current = null,
                    parent = null,
                    index = 0,
                    i,
                    item,
                    parentHash = [];
                if (list && list.length > 0) {
                    for (i = 0; i < list.length; i++) {
                        item = list[i];
                        current = {
                            index: index++,
                            value: item[1],
                            displayText: item[2],
                            parentIndex: item[3]
                        };
                        tempList.push(current);
                        if (current.parentIndex >= 0) {
                            parent = tempList[current.parentIndex];
                            if (!parent.children)
                                parent.children = [];
                            parent.children.push(current)
                        }
                    }
                    for (i = 0; i < tempList.length; i++) {
                        item = tempList[i];
                        if (item.parentIndex < 0)
                            result.push({
                                index: item.index,
                                value: item.value,
                                displayText: item.displayText,
                                children: item.children
                            })
                    }
                }
                return result
            },
            _createCells: function(list) {
                var result = [],
                    columnIndex = -1,
                    rowIndex = -1,
                    dataIndex = -1,
                    value = null,
                    i,
                    elem;
                if (list && list.length > 0)
                    for (i = 0; i < list.length; i++) {
                        elem = list[i];
                        columnIndex = elem[0];
                        rowIndex = elem[1];
                        dataIndex = elem[2];
                        value = elem[3];
                        if (!result[rowIndex])
                            result[rowIndex] = [];
                        if (!result[rowIndex][columnIndex])
                            result[rowIndex][columnIndex] = [];
                        result[rowIndex][columnIndex].splice(dataIndex, 0, value)
                    }
                return result
            },
            _getExpandValueChangingHandler: function() {
                var that = this;
                return function(args) {
                        var isColumn = args.area === 'column',
                            values = args.path;
                        that._dataRequestProcessing = !!args.needExpandData;
                        that._onExpandValue({
                            values: values,
                            isColumn: isColumn,
                            isExpand: args.expanded,
                            isRequestData: that._dataRequestProcessing
                        });
                        that.onCollapseStateChanged(isColumn, values, !args.expanded)
                    }
            },
            onCollapseStateChanged: function(isColumn, values, collapse) {
                var that = this,
                    collapseState = values.concat(isColumn ? 'column' : 'row');
                that._conditionalFormattingInfoCache = [];
                if (collapse)
                    that._collapseStateCache[collapseState] = collapseState;
                else
                    delete that._collapseStateCache[collapseState]
            },
            _resize: function(oldSize, newSize) {
                this.callBase(oldSize, newSize);
                this.pivotGridViewer.resize();
                this._styleSettingsProvider.updateBarWidth(this.pivotGridViewer._$element, PIVOT_BAR_ID)
            },
            _getDataPoint: function(element) {
                var that = this,
                    viewModel = that.options.ViewModel;
                return {
                        getValues: function(name) {
                            switch (name) {
                                case dashboard.itemDataAxisNames.pivotRowAxis:
                                    return element.rowPath;
                                case dashboard.itemDataAxisNames.pivotColumnAxis:
                                    return element.columnPath;
                                default:
                                    return null
                            }
                        },
                        getDeltaIds: function() {
                            return []
                        },
                        getMeasureIds: function() {
                            var dataIndex = element.dataIndex;
                            if (dataIndex != undefined)
                                return [viewModel.Values[dataIndex].DataId];
                            return null
                        }
                    }
            },
            _getCellClickHandler: function() {
                var that = this;
                return function(e) {
                        if (e.area === "data")
                            that._raiseItemClick(e.cell)
                    }
            },
            _getWidget: function() {
                return this.pivotGridViewer
            },
            _getCellPreparedHandler: function() {
                var that = this;
                return function(data) {
                        that._onCellPrepared(data)
                    }
            },
            _getContentReadyHandler: function() {
                var that = this;
                return function(data) {
                        that._onContentReady(data)
                    }
            },
            _onCellPrepared: function(element) {
                var styleSettingsInfo,
                    isMeasureHeader = element.area === dashboard.utils.pivotArea.column && element.cell.dataIndex !== undefined,
                    cellItem = {area: element.area};
                if (!isMeasureHeader) {
                    if (element.area === dashboard.utils.pivotArea.column)
                        cellItem.columnPath = element.cell.path;
                    else if (element.area === dashboard.utils.pivotArea.row)
                        cellItem.rowPath = element.cell.path;
                    else {
                        cellItem.columnPath = element.cell.columnPath;
                        cellItem.rowPath = element.cell.rowPath;
                        cellItem.cellIndex = element.cell.dataIndex
                    }
                    styleSettingsInfo = this.dataController.getStyleSettingsInfo(cellItem, this._collapseStateCache, this._conditionalFormattingInfoCache);
                    this._styleSettingsProvider.applyStyleSettings(element.cellElement, styleSettingsInfo, false, PIVOT_BAR_ID)
                }
            },
            _onContentReady: function(data) {
                this._styleSettingsProvider.updateBarWidth(data.component._$element, PIVOT_BAR_ID)
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file chartItem.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            commonUtils = DX.require("/utils/utils.common"),
            formatter = dashboard.data.formatter,
            chartHelper = dashboard.data.chartHelper,
            selectionHelper = dashboard.data.selectionHelper,
            viewerItems = dashboard.viewerItems,
            utils = DX.utils;
        viewerItems.chartItem = viewerItems.baseItem.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options);
                this.itemElementCustomColor = $.Callbacks()
            },
            initializeData: function(newOptions) {
                this.callBase(newOptions);
                if (this.dataController)
                    this.dataController.elementCustomColor = $.proxy(this._elementCustomColor, this)
            },
            clearSelection: function() {
                if (this.chartViewer)
                    this.chartViewer.clearSelection()
            },
            selectTuple: function(tuple, state) {
                selectionHelper.setSelectedPoint(this.chartViewer, dashboard.utils.getAxisPointValue(tuple, dashboard.itemDataAxisNames.chartSeriesAxis), dashboard.utils.getAxisPointValue(tuple, dashboard.itemDataAxisNames.chartArgumentAxis), state)
            },
            setSelection: function(values) {
                this.callBase(values);
                this._applySelection()
            },
            updateContentState: function() {
                if (this._getCustomHoverEnabled()) {
                    var hoverMode = 'none',
                        targetAxes = this._getTargetAxes();
                    if (targetAxes.length == 1)
                        if (targetAxes[0] == dashboard.itemDataAxisNames.chartArgumentAxis)
                            hoverMode = 'allArgumentPoints';
                        else
                            hoverMode = 'allSeriesPoints';
                    else if (targetAxes.length == 2)
                        hoverMode = 'point';
                    this.chartViewer.option('commonSeriesSettings.hoverMode', hoverMode)
                }
            },
            renderContent: function(changeExisting) {
                var options = this._getChartViewerOptions();
                if (changeExisting && this.chartViewer)
                    this.chartViewer.option(options);
                else
                    this.chartViewer = new DX.viz.charts.dxChart(this.$contentRoot, options);
                var zoomArguments = this.dataController ? this.dataController.getZoomArguments() : null;
                if (zoomArguments)
                    this.chartViewer.zoomArgument(zoomArguments.start, zoomArguments.end);
                this._applySelection()
            },
            getInfo: function() {
                var info = this.callBase();
                if (this.chartViewer.option('scrollingMode') === "all") {
                    var viewport = this.chartViewer.getVisibleArgumentBounds();
                    info = $.extend(true, info, {chartViewport: {
                            min: this.dataController.getArgumentUniquePath(viewport.minVisible),
                            max: this.dataController.getArgumentUniquePath(viewport.maxVisible)
                        }})
                }
                return info
            },
            _elementCustomColor: function(eventArgs) {
                this.itemElementCustomColor.fire(this._getName(), eventArgs)
            },
            _isOverlappingModeHide: function() {
                if (!this.options || !this.options.ViewModel)
                    return false;
                var that = this,
                    viewModel = that.options.ViewModel,
                    isHideMode = false;
                $.each(this.options.ViewModel.Panes, function(_, pane) {
                    $.each(pane.SeriesTemplates, function(_, seriesTemplate) {
                        if (seriesTemplate.PointLabel && (seriesTemplate.PointLabel.OverlappingMode == 'Hide' || seriesTemplate.PointLabel.OverlappingMode == 'Reposition'))
                            isHideMode = true
                    })
                });
                return isHideMode
            },
            _getChartViewerOptions: function() {
                if (!this.options || !this.options.ViewModel)
                    return {};
                var that = this,
                    viewModel = that.options.ViewModel,
                    dataSourceAndSeries = that.dataController.getDataSourceAndSeries(that._encodeHtml),
                    animation = that._getAnimationOptions(),
                    isSelectable = that.isInteractivityActionEnabled(),
                    hoverMode = isSelectable ? that._convertHoverMode(viewModel.SelectionMode) : 'none',
                    pointHoverMode = isSelectable ? that._convertPointHoverMode(viewModel.SelectionMode) : 'none',
                    panes = viewModel.Panes,
                    seriesFormats = {},
                    isDiscreteArgument = that.dataController.isDiscreteArgument(),
                    isQualitativeArgument = that.dataController.isQualitativeArgument(),
                    argumentAxisLabelFormat = undefined,
                    axis = undefined,
                    tooltipArgumentFormat = undefined,
                    legendParams = undefined,
                    rotated = viewModel.Rotated,
                    options = {
                        panes: [],
                        valueAxis: [],
                        redrawOnResize: false,
                        rotated: rotated,
                        pointSelectionMode: 'multiple',
                        seriesSelectionMode: 'multiple',
                        palette: utils.renderHelper.getDefaultPalette(),
                        encodeHtml: that._encodeHtml,
                        onIncidentOccurred: utils.renderHelper.widgetIncidentOccurred,
                        zoomingMode: viewModel.AxisX.EnableZooming ? "all" : "none",
                        scrollingMode: viewModel.AxisX.EnableZooming || viewModel.AxisX.LimitVisiblePoints ? "all" : "none",
                        adjustOnZoom: false,
                        scrollBar: {
                            visible: viewModel.AxisX.EnableZooming || viewModel.AxisX.LimitVisiblePoints,
                            position: "bottom"
                        },
                        customizePoint: function() {
                            var argumentTag = this.tag,
                                seriesTag = this.series.tag,
                                result = {};
                            if (!chartHelper.isFinancialType(this.series.type))
                                result.color = that.dataController.getColor(argumentTag.axisPoint, seriesTag.axisPoint, that._getMeasuresIds(seriesTag), seriesTag.colorMeasureId);
                            if (!dashboard.utils.allowSelectValue(that._getElementInteractionValue(this, that.options.ViewModel)))
                                result.hoverStyle = {hatching: 'none'};
                            return result
                        }
                    };
                if (this._isOverlappingModeHide()) {
                    options.resolveLabelOverlapping = "hide";
                    options.resolveLabelsOverlapping = true
                }
                else {
                    options.resolveLabelOverlapping = "none";
                    options.resolveLabelsOverlapping = false
                }
                $.each(panes, function(index, pane) {
                    var paneName = that.dataController.generatePaneName(pane.Name, index),
                        isPrimaryAxisInPercentFormat = that._isAxisInPercentFormat(pane, false),
                        isSecondaryAxisInPercentFormat = that._isAxisInPercentFormat(pane, true),
                        customizeTextProc = function(isPercentAxis) {
                            return function() {
                                    return formatter.formatAxisValue(this.value, this.min, this.max, isPercentAxis)
                                }
                        };
                    seriesFormats[paneName] = {};
                    options.panes.push({name: paneName});
                    axis = {
                        name: paneName + 'primary',
                        position: rotated ? 'bottom' : 'left',
                        pane: paneName,
                        inverted: pane.PrimaryAxisY.Reverse,
                        label: {
                            visible: pane.PrimaryAxisY.Visible,
                            customizeText: customizeTextProc(isPrimaryAxisInPercentFormat)
                        },
                        title: {text: pane.PrimaryAxisY.Visible ? pane.PrimaryAxisY.Title : undefined},
                        visible: pane.PrimaryAxisY.Visible,
                        grid: {visible: pane.PrimaryAxisY.ShowGridLines},
                        showZero: pane.PrimaryAxisY.ShowZeroLevel
                    };
                    that._configureLogarithmicOptions(axis, pane.PrimaryAxisY);
                    options.valueAxis.push(axis);
                    if (pane.SecondaryAxisY) {
                        axis = {
                            name: paneName + 'secondary',
                            position: rotated ? 'top' : 'right',
                            pane: paneName,
                            inverted: pane.SecondaryAxisY.Reverse,
                            label: {
                                visible: pane.SecondaryAxisY.Visible,
                                customizeText: customizeTextProc(isSecondaryAxisInPercentFormat)
                            },
                            title: {text: pane.SecondaryAxisY.Visible ? pane.SecondaryAxisY.Title : undefined},
                            visible: pane.SecondaryAxisY.Visible,
                            grid: {visible: pane.SecondaryAxisY.ShowGridLines},
                            showZero: pane.SecondaryAxisY.ShowZeroLevel
                        };
                        that._configureLogarithmicOptions(axis, pane.SecondaryAxisY);
                        options.valueAxis.push(axis)
                    }
                });
                if (rotated)
                    options.panes.reverse();
                options.commonSeriesSettings = {
                    hoverMode: hoverMode,
                    point: {
                        visible: false,
                        hoverMode: pointHoverMode
                    },
                    stackedbar: {label: {backgroundColor: 'none'}}
                };
                options.commonPaneSettings = {border: {visible: true}};
                options.argumentAxis = {
                    inverted: viewModel.AxisX.Reverse,
                    label: {
                        visible: viewModel.AxisX.Visible,
                        overlappingBehavior: {mode: !viewModel.Argument.IsOrderedDiscrete && isDiscreteArgument ? 'auto' : 'enlargeTickInterval'},
                        customizeText: function(argument) {
                            var predefinedValue = formatter.getPredefinedString(argument.value);
                            if (commonUtils.isDefined(predefinedValue))
                                return predefinedValue;
                            else
                                return isQualitativeArgument || isDiscreteArgument ? argument.value : that.dataController.formatArgument(argument, argumentAxisLabelFormat) || ""
                        }
                    },
                    title: {text: viewModel.AxisX.Visible ? viewModel.AxisX.Title : undefined},
                    grid: {visible: viewModel.AxisX.ShowGridLines},
                    visible: viewModel.AxisX.Visible,
                    valueMarginsEnabled: chartHelper.allowArgumentAxisMargins(panes)
                };
                that._configureLogarithmicOptions(options.argumentAxis, viewModel.AxisX);
                if (that.dataController.isSingleArgument())
                    tooltipArgumentFormat = that.dataController.getArgumentAxisDimensionFormat(0);
                if (isDiscreteArgument) {
                    options.argumentAxis.type = 'discrete';
                    options.dataPrepareSettings = {sortingMethod: false}
                }
                else if (viewModel.Argument)
                    argumentAxisLabelFormat = that.dataController.getArgumentFormat(viewModel.Argument.AxisXLabelFormat, viewModel.Argument.DataType);
                argumentAxisLabelFormat = argumentAxisLabelFormat ? argumentAxisLabelFormat : tooltipArgumentFormat;
                if (argumentAxisLabelFormat && !(viewModel.Argument && viewModel.Argument.IsContinuousDateTimeScale))
                    switch (argumentAxisLabelFormat.format) {
                        case'monthYear':
                            options.argumentAxis.tickInterval = {months: 1};
                            options.argumentAxis.label.minSpacing = 10;
                            break;
                        case'dayMonthYear':
                            options.argumentAxis.tickInterval = {days: 1};
                            options.argumentAxis.label.minSpacing = 10;
                            break;
                        case'quarterYear':
                            options.argumentAxis.tickInterval = {quarters: 1};
                            options.argumentAxis.label.minSpacing = 10;
                            break;
                        case'dateHour':
                            options.argumentAxis.tickInterval = {hours: 1};
                            options.argumentAxis.label.minSpacing = 10;
                            break;
                        case'dateHourMinute':
                            options.argumentAxis.tickInterval = {minutes: 1};
                            options.argumentAxis.label.minSpacing = 10;
                            break;
                        case'dateHourMinuteSecond':
                            options.argumentAxis.tickInterval = {seconds: 1};
                            options.argumentAxis.label.minSpacing = 10;
                            break;
                        default:
                            break
                    }
                options.tooltip = {
                    enabled: true,
                    container: dashboard.utils.tooltipContainerSelector,
                    customizeTooltip: function(obj) {
                        var pane = obj.point.series.pane,
                            argumentText = that.dataController.getTooltipArgumentText(obj),
                            allSeries = that.chartViewer.getAllSeries(),
                            resultHtml = '<table cellspacing="0px">';
                        resultHtml += '<tr><td align="left" style="padding-bottom:4px;"><b>' + that._getHtml(argumentText) + '</b></td></tr>';
                        $.each(allSeries, function(i, series) {
                            if (series.pane === pane) {
                                var point = series.getPointsByArg(obj.argument)[0];
                                if (point) {
                                    var text = that.dataController.customizeTooltipText(series, point, series.tag.valueFormats, that._encodeHtml);
                                    if (text)
                                        resultHtml += '<tr><td align="left">' + text + '</td></tr>'
                                }
                            }
                        });
                        resultHtml += '</table>';
                        return {html: resultHtml}
                    },
                    font: {size: 14},
                    argumentFormat: tooltipArgumentFormat,
                    zIndex: 100
                };
                options.animation = {
                    enabled: animation.enabled,
                    duration: animation.duration
                };
                options.margin = {
                    top: 10,
                    right: 22,
                    bottom: 22,
                    left: 22
                };
                if (viewModel.Legend) {
                    if (viewModel.Legend.IsInsideDiagram) {
                        legendParams = chartHelper.convertLegendInsidePosition(viewModel.Legend.InsidePosition);
                        legendParams.border = {visible: true}
                    }
                    else
                        legendParams = chartHelper.convertLegendOutsidePosition(viewModel.Legend.OutsidePosition);
                    options.legend = $.extend(true, {}, legendParams, {
                        position: viewModel.Legend.IsInsideDiagram ? 'inside' : 'outside',
                        visible: viewModel.Legend.Visible,
                        itemTextPosition: 'right'
                    });
                    if (viewModel.Legend.IsInsideDiagram)
                        options.legend.margin = 10
                }
                options.onPointClick = that._getPointClickHandler();
                options.onPointHoverChanged = that._getPointHoverHandler();
                return $.extend(true, dataSourceAndSeries, options)
            },
            _configureLogarithmicOptions: function(axis, model) {
                if (model.Logarithmic) {
                    axis.type = 'logarithmic';
                    axis.logarithmBase = model.LogarithmicBase
                }
            },
            _applySelection: function() {
                var that = this,
                    viewModel = that.options.ViewModel,
                    tuples = that.getSelectedTuples();
                if (viewModel && viewModel.SelectionEnabled && tuples.length > 0) {
                    that.chartViewer.clearSelection();
                    $.each(tuples, function(_, tuple) {
                        that.selectTuple(tuple, true)
                    })
                }
            },
            _getDataPoint: function(element) {
                var that = this,
                    elementTag = element.tag,
                    elementSeries = element.series,
                    elementSeriesTag = elementSeries ? elementSeries.tag : undefined,
                    seriesValues = elementSeriesTag ? dashboard.utils.getTagValue(elementSeriesTag) : [],
                    argumentValues = elementTag ? dashboard.utils.getTagValue(elementTag) : [],
                    seriesIndex = elementSeries ? elementSeries.index : undefined;
                return {
                        getValues: function(name) {
                            switch (name) {
                                case dashboard.itemDataAxisNames.chartArgumentAxis:
                                    return argumentValues;
                                case dashboard.itemDataAxisNames.chartSeriesAxis:
                                    return seriesValues;
                                default:
                                    return null
                            }
                        },
                        getDeltaIds: function() {
                            return []
                        },
                        getMeasureIds: function() {
                            return that._getMeasuresIds(elementSeriesTag)
                        }
                    }
            },
            _getMeasuresIds: function(elementSeriesTag) {
                return elementSeriesTag ? elementSeriesTag.dataMembers : []
            },
            _isMultiDataSupported: function() {
                return true
            },
            _getElementInteractionValue: function(element, viewModel) {
                return viewModel.SelectionEnabled && viewModel.SelectionMode === chartHelper.SelectionMode.Series ? element.series.tag : element.tag
            },
            _getPointClickHandler: function() {
                var that = this;
                return function(e) {
                        that._raiseItemClick(e.target)
                    }
            },
            _getPointHoverHandler: function() {
                var that = this;
                return function(e) {
                        that._raiseItemHover(e.target)
                    }
            },
            _isAxisInPercentFormat: function(pane, isSecondaryAxis) {
                var seriesTemplate;
                for (var i = 0; i < pane.SeriesTemplates.length; i++) {
                    seriesTemplate = pane.SeriesTemplates[i];
                    if (isSecondaryAxis == seriesTemplate.PlotOnSecondaryAxis && !seriesTemplate.OnlyPercentValues && !this._isFullStackedSeriesType(seriesTemplate.SeriesType))
                        return false
                }
                return true
            },
            _isFullStackedSeriesType: function(seriesType) {
                switch (seriesType) {
                    case'FullStackedArea':
                    case'FullStackedBar':
                    case'FullStackedLine':
                    case'FullStackedSplineArea':
                        return true;
                    default:
                        return false
                }
            },
            _convertHoverMode: function(selectionMode) {
                switch (selectionMode) {
                    case chartHelper.SelectionMode.Argument:
                        return 'allArgumentPoints';
                    case chartHelper.SelectionMode.Series:
                        return 'allSeriesPoints';
                    case chartHelper.SelectionMode.Points:
                    default:
                        return 'none'
                }
            },
            _convertPointHoverMode: function(selectionMode) {
                switch (selectionMode) {
                    case chartHelper.SelectionMode.Argument:
                        return 'allArgumentPoints';
                    case chartHelper.SelectionMode.Series:
                        return 'allSeriesPoints';
                    case chartHelper.SelectionMode.Points:
                        return "onlyPoint";
                    default:
                        return 'none'
                }
            },
            _resize: function(oldSize, newSize) {
                this.callBase(oldSize, newSize);
                this.chartViewer.render()
            },
            _getWidget: function() {
                return this.chartViewer
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file rangeSelectorItem.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            Class = DevExpress.require("/class"),
            formatter = dashboard.data.formatter,
            chartHelper = dashboard.data.chartHelper,
            viewerItems = dashboard.viewerItems,
            utils = DX.utils;
        viewerItems.rangeSelectorItem = viewerItems.baseItem.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options)
            },
            isPaneEmpty: function() {
                return this.callBase() || !this.hasCaption()
            },
            clearSelection: function() {
                if (this.rangeSelectorViewer)
                    this.rangeSelectorViewer.resetSelectedRange()
            },
            setSelection: function(values) {
                if (values.length == 0)
                    this.clearSelection();
                else {
                    var range = values[0];
                    this.rangeSelectorViewer.setSelectedRange({
                        startValue: range[0],
                        endValue: range[1]
                    })
                }
            },
            getCurrentRange: function() {
                return new dashboard.rangeFilterSelection(this.rangeSelectorViewer.getSelectedRange())
            },
            getEntireRange: function() {
                return new dashboard.rangeFilterSelection(this.rangeSelectorViewer.option("scale"))
            },
            setRange: function(range) {
                var rangeFilterSelection = new dashboard.rangeFilterSelection(range);
                this.selected.fire(this._getName(), dashboard.viewerActions.setMasterFilter, [[rangeFilterSelection.getMinimum(), rangeFilterSelection.getMaximum()]])
            },
            renderContent: function(changeExisting) {
                var options = this._getRangeSelectorViewerOptions();
                if (changeExisting && this.rangeSelectorViewer)
                    this.rangeSelectorViewer.option(options);
                else
                    this.rangeSelectorViewer = new DX.viz.rangeSelector.dxRangeSelector(this.$contentRoot, options)
            },
            _isBorderRequired: function() {
                return false
            },
            _getContainerPosition: function() {
                var that = this,
                    position = that.callBase();
                position.offsetY = position.height - that._getButtonOffset();
                return position
            },
            _getRangeSelectorViewerOptions: function() {
                if (!this.options || !this.options.ViewModel)
                    return {};
                var that = this,
                    viewModel = that.options.ViewModel,
                    allSelectedValues = this._getSelectedValues(),
                    selectedValues = allSelectedValues ? allSelectedValues[0] : null,
                    dataSourceAndSeries = that.dataController.getDataSourceAndSeries(that._encodeHtml),
                    firstDataItemIndex = 0,
                    lastDataItemIndex = dataSourceAndSeries.dataSource ? dataSourceAndSeries.dataSource.length - 1 : -1,
                    isQualitativeArgument = that.dataController.isQualitativeArgument(),
                    tickMarkInterval = undefined,
                    tooltipFormat = undefined,
                    argumentAxisLabelFormat = undefined,
                    animation = that._getAnimationOptions(),
                    options = {encodeHtml: that._encodeHtml};
                if (!that.isPaneEmpty())
                    options.margin = {
                        top: 10,
                        bottom: 15
                    };
                options.scale = {marker: {visible: false}};
                if (lastDataItemIndex > 0) {
                    options.scale.startValue = isQualitativeArgument ? firstDataItemIndex : dataSourceAndSeries.dataSource[firstDataItemIndex].x;
                    options.scale.endValue = isQualitativeArgument ? lastDataItemIndex : dataSourceAndSeries.dataSource[lastDataItemIndex].x
                }
                if (selectedValues && selectedValues.length)
                    options.selectedRange = {
                        startValue: selectedValues[0],
                        endValue: selectedValues[1]
                    };
                if (that.dataController.isSingleArgument()) {
                    tooltipFormat = that.dataController.getArgumentAxisDimensionFormat(0);
                    if (viewModel.Argument)
                        argumentAxisLabelFormat = that.dataController.getArgumentFormat(viewModel.Argument.AxisXLabelFormat, viewModel.Argument.DataType);
                    options.scale.label = {format: argumentAxisLabelFormat ? argumentAxisLabelFormat : tooltipFormat}
                }
                options.sliderMarker = {format: tooltipFormat};
                if (viewModel.Argument && viewModel.Argument.IsContinuousDateTimeScale)
                    options.behavior = {
                        snapToTicks: false,
                        animationEnabled: animation.enabled
                    };
                else {
                    tickMarkInterval = that.dataController.isDiscreteArgument() || viewModel.Argument.Type == 'Integer' ? 1 : chartHelper.convertPresentationUnit(viewModel.Argument);
                    if (tickMarkInterval) {
                        options.scale.minorTickInterval = tickMarkInterval;
                        options.scale.majorTickInterval = tickMarkInterval;
                        options.scale.showMinorTicks = false;
                        options.behavior = {
                            snapToTicks: true,
                            animationEnabled: animation.enabled
                        }
                    }
                }
                options.dataSource = dataSourceAndSeries.dataSource;
                options.chart = {
                    series: dataSourceAndSeries.series,
                    commonSeriesSettings: {type: viewModel.SeriesTemplates && viewModel.SeriesTemplates.length > 0 ? chartHelper.convertSeriesType(viewModel.SeriesTemplates[0].SeriesType) : null},
                    palette: utils.renderHelper.getDefaultPalette()
                };
                options.onSelectedRangeChanged = that._getSelectedRangeChangedHandler();
                return options
            },
            _getSelectedRangeChangedHandler: function() {
                var that = this;
                return function(e) {
                        that.selected.fire(that._getName(), dashboard.viewerActions.setMasterFilter, [[e.startValue, e.endValue]])
                    }
            },
            _resize: function(oldSize, newSize) {
                this.callBase(oldSize, newSize);
                this.rangeSelectorViewer.render()
            },
            _getWidget: function() {
                return this.rangeSelectorViewer
            }
        });
        dashboard.rangeFilterSelection = Class.inherit({
            ctor: function ctor(range) {
                this.setMinimum(range.startValue);
                this.setMaximum(range.endValue)
            },
            getMaximum: function() {
                return this.maximum
            },
            setMaximum: function(value) {
                this.maximum = value
            },
            getMinimum: function() {
                return this.minimum
            },
            setMinimum: function(value) {
                this.minimum = value
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file imageItem.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            viewerItems = dashboard.viewerItems;
        viewerItems.imageItem = viewerItems.baseItem.inherit({
            ctor: function ctor($container, options) {
                this.imgSrc = undefined;
                this.img = undefined;
                this.callBase($container, options)
            },
            initializeData: function(newOptions) {
                this.callBase(newOptions);
                this.imgSrc = this._getImageSource(this.options.ViewModel ? this.options.ViewModel.ImageViewModel : undefined)
            },
            renderContent: function(changeExisting) {
                var that = this;
                if (!changeExisting || !that.img) {
                    that.img = $('<img>').bind('load', function() {
                        that._initialWidth = that.img.width();
                        that._initialHeight = that.img.height();
                        that._loadedImgProcessing()
                    });
                    that.$contentRoot.append(that.img)
                }
                that.img.attr('src', that.imgSrc)
            },
            _loadedImgProcessing: function() {
                var that = this,
                    $contentRoot = that.$contentRoot,
                    containerWidth = $contentRoot.width(),
                    containerHeight = $contentRoot.height(),
                    img = $contentRoot.find('img'),
                    viewModel = that.options.ViewModel,
                    sizeMode = viewModel.SizeMode,
                    horizontalAlignment = viewModel.HorizontalAlignment || 'Right',
                    verticalAlignment = viewModel.VerticalAlignment || 'Top',
                    centeringDirect,
                    curImgHeight,
                    curImgWidth;
                switch (sizeMode) {
                    case'Clip':
                        $contentRoot.css({overflow: 'hidden'});
                        that._setHorizontalAlignment(img, horizontalAlignment);
                        that._setVerticalAlignment(img, verticalAlignment);
                        break;
                    case'Stretch':
                        img.css({
                            width: '100%',
                            height: '100%'
                        });
                        break;
                    case'Squeeze':
                        curImgHeight = img.height();
                        curImgWidth = img.width();
                        if (curImgHeight >= containerHeight && curImgHeight <= that._initialHeight || curImgWidth >= containerWidth && curImgWidth <= that._initialWidth) {
                            centeringDirect = that._setImgSizeWithProportions(img, containerHeight / containerWidth);
                            img.css({
                                marginTop: 0,
                                marginLeft: 0
                            })
                        }
                        else {
                            img.css({
                                width: '',
                                height: ''
                            });
                            that._setHorizontalAlignment(img, horizontalAlignment);
                            that._setVerticalAlignment(img, verticalAlignment)
                        }
                        break;
                    case'Zoom':
                        centeringDirect = that._setImgSizeWithProportions(img, containerHeight / containerWidth);
                        break;
                    default:
                        break
                }
                if (centeringDirect === 'horizontalCentering')
                    that._setHorizontalAlignment(img, horizontalAlignment);
                if (centeringDirect === 'verticalCentering')
                    that._setVerticalAlignment(img, verticalAlignment)
            },
            _setHorizontalAlignment: function($img, horizontalAlignment) {
                if (horizontalAlignment === 'Center') {
                    $img.css({marginLeft: (this.$contentRoot.width() - $img.width()) / 2});
                    return
                }
                $img.attr('align', horizontalAlignment.toLowerCase())
            },
            _setVerticalAlignment: function($img, verticalAlignment) {
                var verticalOffsetCoeff,
                    differenceTop = this.$contentRoot.height() - $img.height();
                switch (verticalAlignment) {
                    case'Bottom':
                        verticalOffsetCoeff = 1;
                        break;
                    case'Center':
                        verticalOffsetCoeff = 0.5;
                        break;
                    case'Top':
                        verticalOffsetCoeff = 0;
                        break
                }
                $img.css({marginTop: Math.floor(differenceTop * verticalOffsetCoeff) + 'px'})
            },
            _setImgSizeWithProportions: function($img, containerProportion) {
                var imgProportion = this._initialHeight / this._initialWidth;
                if (imgProportion > containerProportion) {
                    $img.height('100%');
                    $img.width(Math.floor($img.height() / imgProportion));
                    return 'horizontalCentering'
                }
                else {
                    $img.width('100%');
                    $img.height(Math.floor($img.width() * imgProportion));
                    return 'verticalCentering'
                }
            },
            _getImageSource: function(imageViewModel) {
                if (imageViewModel) {
                    var url = imageViewModel.Url,
                        sourceBase64String = imageViewModel.SourceBase64String,
                        mimeType = imageViewModel.MimeType || '';
                    if (sourceBase64String)
                        return 'data:' + mimeType + ';base64,' + sourceBase64String;
                    if (url)
                        return url
                }
                return ''
            },
            _resize: function() {
                this.callBase();
                this._loadedImgProcessing()
            },
            _getWidget: function() {
                return this.img
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file textItem.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            viewerItems = dashboard.viewerItems;
        viewerItems.textItem = viewerItems.baseItem.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options);
                this.div = undefined
            },
            renderContent: function(changeExisting) {
                var that = this,
                    options = that.options;
                if (!changeExisting || !that.div) {
                    that.div = $('<div>');
                    that.$content = DX.utils.renderHelper.wrapScrollable(that.div, dashboard.USE_NATIVE_SCROLLING, 'auto', 'vertical');
                    that.$contentRoot.append(that.div)
                }
                that.$content.html(options.ViewModel.Html.replace('<body>', '<div>').replace('</body>', '</div>'))
            },
            _getWidget: function() {
                return this.div
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file dataGridItem.js */
    (function($, DX, undefined) {
        var utils = DX.utils,
            dashboard = DX.dashboard,
            stringUtils = DX.require("/utils/utils.string"),
            localizer = dashboard.data.localizer,
            viewerItems = dashboard.viewerItems,
            simpleIndicator,
            dbvIndicator = DX.viz.indicators.DeltaIndicator,
            columnWidthCalculator = dashboard.ColumnWidthCalculator,
            dataPostfix = dashboard.data.dataController.DATA_POSTFIX,
            styleSettingsProvider = dashboard.styleSettingsProvider,
            COLUMN_DATA_TYPE = 'string',
            MAX_CELL_COUNT = 2000,
            HEIGHT_DELTA_INDICATOR = 12,
            COLUMN_MIN_WIDTH = 10,
            DATAGRID_TABLE = "dx-datagrid-table",
            DATAGRID_HEADER_ROW_CLASS = "dx-datagrid-headers",
            DATAGRID_CONTEXT_MENU_ICON = 'dashboard-datagriditem-resetcolumnwidths',
            DIGITS_STRING = '0123456789';
        function createIndicator() {
            if (!simpleIndicator) {
                simpleIndicator = new DX.viz.indicators.SimpleIndicator;
                simpleIndicator.renderIndicators(HEIGHT_DELTA_INDICATOR)
            }
        }
        viewerItems.dataGridItem = viewerItems.baseItem.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options);
                this._widthOptionsInfo = undefined;
                this._calculator = new columnWidthCalculator;
                this._styleSettingsProvider = new styleSettingsProvider;
                this._styleSettingsProvider.initialize($container, this.options.ConditionalFormattingModel);
                this._clientState = undefined;
                this.dataGrid = undefined;
                createIndicator()
            },
            TextAlignment: {
                Left: 'left',
                Right: 'right',
                Center: 'center'
            },
            DisplayMode: {
                Value: 'Value',
                Delta: 'Delta',
                Bar: 'Bar',
                Sparkline: 'Sparkline',
                Image: 'Image'
            },
            SummaryType: {
                Count: 'Count',
                Min: 'Min',
                Max: 'Max',
                Avg: 'Avg',
                Sum: 'Sum'
            },
            clearSelection: function() {
                if (this.dataGrid)
                    this.dataGrid.clearSelection()
            },
            setSelection: function(values) {
                this.callBase(values);
                this._setGridSelection(values)
            },
            selectTuple: function(tuple, state) {
                var that = this,
                    currentSelection = that.dataGrid.getSelectedRowKeys(),
                    processKeys = function(keys) {
                        $.each(keys, function(_, key) {
                            if (state)
                                currentSelection.push(key);
                            else
                                currentSelection.splice($.inArray(key, currentSelection), 1)
                        });
                        return currentSelection
                    };
                that._setGridSelection([tuple[0].Value], processKeys)
            },
            renderContent: function(changeExisting) {
                var that = this,
                    gridOptions;
                if (that.options) {
                    gridOptions = that._getGridViewOptions();
                    if (changeExisting && that.dataGrid)
                        that.dataGrid.option(gridOptions);
                    else {
                        that._charWidth = that._calcCharWidth();
                        that._columnsResized = false;
                        that._widthOptionsInfo = undefined;
                        that.dataGrid = new DX.ui.dxDataGrid(that.$contentRoot, gridOptions)
                    }
                    that._applySelection()
                }
            },
            render: function($container) {
                this._styleSettingsProvider.update($container);
                this.callBase($container)
            },
            updateContentState: function(){},
            getInfo: function() {
                var that = this,
                    gridScroll = that.dataGrid.element().find(".dx-scrollable").first().dxScrollable("instance"),
                    isVScrollbarVisible = that.dataGrid.isScrollbarVisible(),
                    isHScrollbarVisible = gridScroll.scrollWidth() > gridScroll.clientWidth(),
                    scrollRowData = that.dataGrid.getTopVisibleRowData(),
                    topPath = [],
                    leftPath = [];
                if (scrollRowData) {
                    $.each(that.options.ViewModel.RowIdentificatorDataMembers, function(index, dataMember) {
                        var value = scrollRowData[dataMember];
                        topPath.push(value)
                    });
                    leftPath.push(that._getLeftPrintingColumnIndex(gridScroll.scrollLeft()))
                }
                return $.extend(true, this.callBase(), {
                        scroll: {
                            horizontal: isHScrollbarVisible,
                            vertical: isVScrollbarVisible,
                            topPath: topPath,
                            leftPath: leftPath
                        },
                        widthOptionsInfo: that._widthOptionsInfo
                    })
            },
            initializeData: function(newOptions) {
                this.callBase(newOptions);
                if (this._styleSettingsProvider)
                    this._styleSettingsProvider.initialize(this.$contentRoot, this.options.ConditionalFormattingModel)
            },
            _getLeftPrintingColumnIndex: function(hScrollPosition) {
                var that = this,
                    columnIndex = 0,
                    widthSum = 0;
                if (that._widthOptionsInfo !== undefined)
                    $.each(that._widthOptionsInfo.columnsWidthInfo, function(index, info) {
                        if (hScrollPosition < widthSum + info.actualWidth / 2) {
                            columnIndex = index;
                            return false
                        }
                        widthSum += info.actualWidth
                    });
                return columnIndex
            },
            _onResetColumnWidth: function() {
                this._updateWidthOptionsInfoByViewModel();
                this._recalcGridColumnWidhts();
                this._columnsResized = false;
                this._clientState = undefined;
                this.dataGrid.refresh();
                this._applySelection()
            },
            _calcCharWidth: function() {
                var span = $('<span />').addClass('dx-widget').text(DIGITS_STRING).appendTo($('body'));
                var charWidth = span.width() / 10;
                span.remove();
                return charWidth
            },
            _onColumnsChanging: function(e) {
                var grid = e.component,
                    leftColumnIndex;
                if (!this._viewModelHasColumns() || grid.columnCount() !== this.options.ViewModel.Columns.length || this._columnWidthsSetting)
                    return;
                if (grid.getController('data').isLoaded() && (e.optionNames.width || e.optionNames.visibleWidth && grid.columnOption(0, 'visibleWidth') !== undefined)) {
                    this._updateWidthOptionsInfoByViewModel();
                    this._applyClientState()
                }
                if (this._widthOptionsInfo !== undefined)
                    if (e.optionNames.width) {
                        leftColumnIndex = this._getResizedColumnIndex();
                        if (leftColumnIndex >= 0 && leftColumnIndex < this.options.ViewModel.Columns.length - 1)
                            this._onColumnResized(leftColumnIndex)
                    }
                    else if (e.optionNames.visibleWidth && grid.columnOption(0, 'visibleWidth') !== undefined)
                        this._onGridResized()
            },
            _onColumnResized: function(leftColumnIndex) {
                var that = this;
                that._recalcWidthOptionsInfo(that.dataGrid);
                that._widthOptionsInfo.columnWidthMode = 'Manual';
                var leftColumnInfo = that._widthOptionsInfo.columnsWidthInfo[leftColumnIndex];
                var rightColumnInfo = that._widthOptionsInfo.columnsWidthInfo[leftColumnIndex + 1];
                if (that._widthOptionsInfo.allColumnsFixed) {
                    that._widthOptionsInfo.allColumnsFixed = false;
                    $.each(that._widthOptionsInfo.columnsWidthInfo, function(_, info) {
                        info.widthType = 'Weight';
                        info.isFixedWidth = false
                    })
                }
                else {
                    leftColumnInfo.widthType = 'Weight';
                    leftColumnInfo.isFixedWidth = false;
                    rightColumnInfo.widthType = 'Weight';
                    rightColumnInfo.isFixedWidth = false
                }
                var leftColumnWidth = that.dataGrid.columnOption(leftColumnIndex, 'width');
                var rigthColumnWidth = that.dataGrid.columnOption(leftColumnIndex + 1, 'width');
                if (leftColumnWidth < COLUMN_MIN_WIDTH) {
                    rigthColumnWidth = leftColumnWidth + rigthColumnWidth - COLUMN_MIN_WIDTH;
                    leftColumnWidth = COLUMN_MIN_WIDTH
                }
                else if (rigthColumnWidth < COLUMN_MIN_WIDTH) {
                    leftColumnWidth = leftColumnWidth + rigthColumnWidth - COLUMN_MIN_WIDTH;
                    rigthColumnWidth = COLUMN_MIN_WIDTH
                }
                leftColumnInfo.actualWidth = leftColumnWidth;
                rightColumnInfo.actualWidth = rigthColumnWidth;
                var actualWidthSum = 0;
                var weightSum = 0;
                $.each(that._widthOptionsInfo.columnsWidthInfo, function(_, info) {
                    if (info.widthType === 'Weight') {
                        actualWidthSum += info.actualWidth;
                        weightSum += info.weight
                    }
                });
                $.each(that._widthOptionsInfo.columnsWidthInfo, function(_, info) {
                    if (info.widthType === 'Weight') {
                        info.weight = info.actualWidth * weightSum / actualWidthSum;
                        info.initialWidth = info.weight
                    }
                });
                that._columnWidthsSetting = true;
                that.dataGrid.beginUpdate();
                that.dataGrid.columnOption(leftColumnIndex, 'width', undefined);
                that.dataGrid.columnOption(leftColumnIndex + 1, 'width', undefined);
                that.dataGrid.columnOption(leftColumnIndex, 'visibleWidth', leftColumnWidth);
                that.dataGrid.columnOption(leftColumnIndex + 1, 'visibleWidth', rigthColumnWidth);
                that.dataGrid.endUpdate();
                that._columnWidthsSetting = false;
                that._columnsResized = true;
                that._updateClientState()
            },
            _getResizedColumnIndex: function() {
                var columnCount = this.dataGrid.columnCount();
                for (var i = 0; i < columnCount; i++)
                    if (this.dataGrid.columnOption(i, 'width'))
                        return i;
                return -1
            },
            _onGridResized: function() {
                this._recalcGridColumnWidhts()
            },
            _recalcGridColumnWidhts: function() {
                var that = this;
                that._recalcWidthOptionsInfo();
                that._columnWidthsSetting = true;
                that.dataGrid.beginUpdate();
                $.each(that._widthOptionsInfo.columnsWidthInfo, function(i, columnInfo) {
                    that.dataGrid.columnOption(i, 'visibleWidth', columnInfo.actualWidth)
                });
                that.dataGrid.endUpdate();
                that._columnWidthsSetting = false
            },
            _recalcWidthOptionsInfo: function() {
                var maxVisibleWidth = this.dataGrid.element().width() - this.dataGrid.getScrollbarWidth();
                this._widthOptionsInfo = this._calculator.calcWidth(this._widthOptionsInfo, maxVisibleWidth)
            },
            _updateWidthOptionsInfoByViewModel: function() {
                var that = this;
                var columns = that.options.ViewModel.Columns;
                that._widthOptionsInfo = {
                    columnWidthMode: that.options.ViewModel.ColumnWidthMode,
                    allColumnsFixed: that.options.ViewModel.AllColumnsFixed,
                    columnsWidthInfo: []
                };
                $.each(columns, function(i, column) {
                    var manualColumnWidthMode = that._widthOptionsInfo.columnWidthMode == 'Manual';
                    var initialWidth = 0;
                    if (!manualColumnWidthMode || column.WidthType === 'FitToContent')
                        initialWidth = that.dataGrid.columnOption(i, 'bestFitWidth');
                    else if (manualColumnWidthMode && column.WidthType === 'FixedWidth')
                        initialWidth = Math.round(column.FixedWidth * that._charWidth);
                    else
                        initialWidth = column.Weight;
                    that._widthOptionsInfo.columnsWidthInfo[i] = {
                        actualIndex: column.ActualIndex,
                        widthType: column.WidthType,
                        initialWidth: initialWidth,
                        weight: column.Weight,
                        minWidth: COLUMN_MIN_WIDTH,
                        actualWidth: 0,
                        isFixedWidth: column.WidthType !== 'Weight',
                        defaultBestCharacterCount: column.DefaultBestCharacterCount,
                        displayMode: column.DisplayMode,
                        fixedWidth: column.FixedWidth
                    }
                })
            },
            _updateClientState: function() {
                var that = this,
                    found,
                    res = {
                        columnWidthMode: that._widthOptionsInfo.columnWidthMode,
                        allColumnsFixed: that._widthOptionsInfo.allColumnsFixed,
                        columnsWidthInfo: []
                    };
                for (var i = 0; i < that._widthOptionsInfo.columnsWidthInfo.length; i++)
                    res.columnsWidthInfo.push(that._widthOptionsInfo.columnsWidthInfo[i]);
                if (that._clientState)
                    for (var i = 0; i < that._clientState.columnsWidthInfo.length; i++) {
                        found = false;
                        for (var j = 0; j < res.columnsWidthInfo.length; j++)
                            if (that._clientState.columnsWidthInfo[i].actualIndex === res.columnsWidthInfo[j].actualIndex) {
                                found = true;
                                break
                            }
                        if (!found)
                            res.columnsWidthInfo.push(that._clientState.columnsWidthInfo[i])
                    }
                res.columnsResized = that._columnsResized;
                this._clientState = res
            },
            _applyClientState: function() {
                var that = this,
                    clientState = that._clientState,
                    viewModel = that.options.ViewModel,
                    initialWidth,
                    manualColumnWidthMode;
                if (clientState) {
                    manualColumnWidthMode = clientState.columnWidthMode == 'Manual';
                    that._columnsResized = clientState.columnsResized;
                    that._widthOptionsInfo.columnWidthMode = clientState.columnWidthMode;
                    that._widthOptionsInfo.allColumnsFixed = clientState.allColumnsFixed;
                    $.each(that._widthOptionsInfo.columnsWidthInfo, function(i, info) {
                        $.each(clientState.columnsWidthInfo, function(j, clientInfo) {
                            if (info.actualIndex === clientInfo.actualIndex) {
                                info.widthType = clientInfo.widthType;
                                info.weight = clientInfo.weight;
                                info.minWidth = clientInfo.minWidth;
                                info.isFixedWidth = clientInfo.isFixedWidth;
                                if (clientState.columnWidthMode !== 'Manual' || clientInfo.widthType === 'FitToContent')
                                    initialWidth = that.dataGrid.columnOption(i, 'bestFitWidth');
                                else
                                    initialWidth = clientInfo.initialWidth;
                                info.initialWidth = initialWidth
                            }
                            else if (viewModel.ColumnWidthMode != 'Manual' && clientState.columnWidthMode == 'Manual')
                                info.initialWidth = info.weight
                        })
                    })
                }
            },
            _getColumnWeightSum: function() {
                var weightSum = 0,
                    columns = this.options.ViewModel.Columns;
                for (var i = 0; i < columns.length; i++)
                    weightSum += columns[i].Weight;
                return weightSum
            },
            _viewModelHasColumns: function() {
                return this.options && this.options.ViewModel && this.options.ViewModel.Columns && this.options.ViewModel.Columns.length !== 0
            },
            _getGridViewOptions: function() {
                if (!this.options.ViewModel)
                    return {};
                var that = this,
                    viewModel = that.options.ViewModel,
                    commonOptions = {},
                    dataSource = that.dataController.getDataSource(),
                    columns = that._getColumns(),
                    totals = {totalItems: that._getTotals()},
                    onItemClick = function(params) {
                        that._onResetColumnWidth(params)
                    };
                commonOptions.dataSource = dataSource;
                commonOptions.scrolling = {
                    mode: dataSource.store.data.length * (columns.length || 1) >= MAX_CELL_COUNT ? 'virtual' : 'standard',
                    useNative: dashboard.USE_NATIVE_SCROLLING
                };
                commonOptions.columns = columns;
                commonOptions.summary = totals;
                commonOptions.remoteOperations = {summary: true};
                commonOptions.paging = {enabled: false};
                commonOptions.rowAlternationEnabled = viewModel.EnableBandedRows;
                commonOptions.showColumnHeaders = viewModel.ShowColumnHeaders;
                commonOptions.showRowLines = viewModel.ShowHorizontalLines;
                commonOptions.showColumnLines = viewModel.ShowVerticalLines;
                commonOptions.sorting = {
                    mode: 'multiple',
                    ascendingText: localizer.getString(dashboard.localizationId.buttonNames.GridSortAscending),
                    descendingText: localizer.getString(dashboard.localizationId.buttonNames.GridSortDescending),
                    clearText: localizer.getString(dashboard.localizationId.buttonNames.GridClearSorting)
                };
                commonOptions.onContextMenuPreparing = function(options) {
                    if (options.target === 'content')
                        options.items = [{
                                text: localizer.getString(dashboard.localizationId.buttonNames.GridResetColumnWidths),
                                icon: DATAGRID_CONTEXT_MENU_ICON,
                                disabled: !that._columnsResized,
                                onItemClick: onItemClick
                            }]
                };
                commonOptions.onCellClick = that._getCellClickHandler();
                commonOptions.onCellHoverChanged = that._getCellHoverHandler();
                commonOptions.noDataText = localizer.getString(dashboard.localizationId.MessageGridHasNoData);
                commonOptions.width = '100%';
                commonOptions.wordWrapEnabled = viewModel.WordWrap;
                commonOptions.onColumnsChanging = $.proxy(that._onColumnsChanging, that);
                commonOptions.columnAutoWidth = true;
                commonOptions.allowColumnResizing = viewModel.ColumnWidthMode !== 'AutoFitToContents';
                commonOptions.cellHintEnabled = true;
                commonOptions.loadPanel = false;
                commonOptions.searchPanel = false;
                commonOptions.useKeyboard = false;
                return commonOptions
            },
            _getElement: function(values, index) {
                var $element,
                    $elementWidth;
                $element = values.get(index);
                if ($element) {
                    $elementWidth = $element.style.width;
                    if ($elementWidth)
                        return $elementWidth
                }
                return null
            },
            _calcMaxWidth: function(values) {
                var $element,
                    width = 0,
                    maxWidth = 0;
                if (values && values.length) {
                    $element = this._getElement(values, values.length - 1) || this._getElement(values, 0);
                    if ($element)
                        maxWidth = parseInt($element);
                    else
                        values.each(function(_, element) {
                            width = $(element).width();
                            if (width > maxWidth)
                                maxWidth = width
                        })
                }
                return maxWidth
            },
            _changeGridSparklineColumnsWidth: function(gridRootElement, columnName, visibleWidthReset, columnViewModel) {
                var that = this,
                    startValues = gridRootElement.find('.' + columnName + '_startValue'),
                    endValues = gridRootElement.find('.' + columnName + '_endValue'),
                    maxStartWidth = that._calcMaxWidth(startValues),
                    maxEndWidth = that._calcMaxWidth(endValues),
                    sparklineContainers = gridRootElement.find('.' + columnName + '_sparkline'),
                    firstsparkline = sparklineContainers.get(0),
                    columnWidth = firstsparkline ? $(firstsparkline).parent().width() : 0,
                    sparklineWidth = columnWidth - (maxStartWidth + maxEndWidth);
                if (sparklineWidth >= 0 || visibleWidthReset) {
                    if (visibleWidthReset) {
                        startValues.css('float', '');
                        endValues.css('float', '')
                    }
                    else {
                        startValues.css('float', 'left');
                        startValues.width(maxStartWidth);
                        endValues.css('float', 'left');
                        endValues.width(maxEndWidth)
                    }
                    startValues.show();
                    endValues.show();
                    $.each(sparklineContainers, function(_, sparklineContainer) {
                        var $sparklineContainer = $(sparklineContainer);
                        if (visibleWidthReset) {
                            $sparklineContainer.css('float', '');
                            $sparklineContainer.dxSparkline('instance').option('size', {width: Math.round(columnViewModel.DefaultBestCharacterCount * that._charWidth)});
                            $sparklineContainer.show()
                        }
                        else {
                            $sparklineContainer.css('float', 'left');
                            $sparklineContainer.dxSparkline('instance').option('size', {width: sparklineWidth});
                            $sparklineContainer.show()
                        }
                    })
                }
                else {
                    $.each(sparklineContainers, function(_, sparklineContainer) {
                        var $sparklineContainer = $(sparklineContainer);
                        $sparklineContainer.hide()
                    });
                    if (columnWidth >= maxStartWidth + maxEndWidth)
                        startValues.show();
                    else {
                        startValues.hide();
                        if (columnWidth >= maxEndWidth)
                            endValues.show();
                        else
                            endValues.hide()
                    }
                }
            },
            _changeGridBarColumnsWidth: function(gridRootElement, columnName, visibleWidthReset, columnViewModel) {
                var that = this,
                    bars = gridRootElement.find('.' + columnName + '_bar'),
                    firstBar = bars.get(0),
                    columnWidth = firstBar ? $(firstBar).parent().width() : 0;
                $.each(bars, function(_, barContainer) {
                    var $barContainer = $(barContainer);
                    if (columnWidth > 0) {
                        $barContainer.show();
                        $barContainer.dxBullet('instance').option('size', {width: visibleWidthReset ? Math.round(columnViewModel.DefaultBestCharacterCount * that._charWidth) : columnWidth})
                    }
                    else
                        $barContainer.hide()
                })
            },
            _getRowsValues: function(row) {
                var that = this,
                    selectionMembers = that.options.ViewModel.SelectionDataMembers,
                    visibleValues = [],
                    value;
                $.each(selectionMembers, function(_, member) {
                    value = that._getUniqueValue(row, member);
                    if (value !== undefined)
                        visibleValues.push(value)
                });
                return that.dataController.getSelectionValues(visibleValues)
            },
            _getSelectedRowIndices: function() {
                var selectedValues = this._getSelectedValues();
                return selectedValues ? this.dataController.getSelectedRowKeys(selectedValues) : []
            },
            _renderDelta: function($container, deltaValue, styleSettingsInfo) {
                var containerDiv = $('<div>', {css: {display: 'inline-block'}}).appendTo($container),
                    indicatorColorName = dbvIndicator.getIndicatorColorType(deltaValue.type, deltaValue.hasPositiveMeaning, deltaValue.text.useDefaultColor),
                    $textSpan = $('<span>', {
                        css: {display: 'inline-block'},
                        text: deltaValue.text.value
                    });
                if (styleSettingsInfo.styleIndexes.length === 0)
                    $textSpan.addClass(indicatorColorName);
                else
                    this._styleSettingsProvider.applyStyleSettings($textSpan, styleSettingsInfo, true);
                $container.innerHTML = '';
                $container.css({
                    'text-align': 'right',
                    overflow: 'hidden'
                });
                $textSpan.appendTo(containerDiv);
                $('<div>', {css: {
                        display: 'inline-block',
                        'margin-top': '3px',
                        'margin-left': '10px',
                        width: '16px',
                        height: HEIGHT_DELTA_INDICATOR
                    }}).appendTo(containerDiv).html(simpleIndicator.getRenderedIndicator(deltaValue.type, deltaValue.hasPositiveMeaning))
            },
            _renderSparkline: function(sparklineOptionsContainer, sparklineData, styleSettingsInfo) {
                var data = sparklineOptionsContainer.sparklineData,
                    name = sparklineOptionsContainer.columnName,
                    $container = sparklineOptionsContainer.parentContainer,
                    columnViewModel = sparklineOptionsContainer.columnViewModel,
                    sparklineOptions = sparklineData.sparkline,
                    sparklineDiv = $('<div>', {
                        css: {
                            float: 'left',
                            display: 'inline-block'
                        },
                        'class': name + '_sparkline'
                    }),
                    startSpan,
                    endSpan;
                if (columnViewModel.ShowStartEndValues) {
                    startSpan = $('<span>', {
                        css: {
                            display: 'none',
                            float: 'left',
                            textAlign: 'right'
                        },
                        'class': name + '_startValue'
                    }).appendTo($container);
                    sparklineDiv.appendTo($container);
                    endSpan = $('<span>', {
                        css: {
                            display: 'none',
                            float: 'left',
                            textAlign: 'right'
                        },
                        'class': name + '_endValue'
                    }).appendTo($container);
                    this._styleSettingsProvider.applyStyleSettings(startSpan, styleSettingsInfo, true);
                    this._styleSettingsProvider.applyStyleSettings(endSpan, styleSettingsInfo, true);
                    if (data && data.length) {
                        startSpan.text(sparklineData.startText);
                        endSpan.text(sparklineData.endText)
                    }
                }
                else
                    sparklineDiv.appendTo($container);
                sparklineOptions.size = {
                    width: Math.round(columnViewModel.DefaultBestCharacterCount * this._charWidth),
                    height: 20
                };
                sparklineOptions.pointSize = 1;
                new DevExpress.viz.sparklines.dxSparkline(sparklineDiv, sparklineOptions)
            },
            _renderBar: function(barOptionsContainer) {
                var that = this,
                    zerovalue = barOptionsContainer.zeroValue,
                    value = barOptionsContainer.value,
                    barDiv = $('<div>', {'class': barOptionsContainer.columnName + '_bar'}).appendTo(barOptionsContainer.parentContainer);
                barDiv.dxBullet({
                    startScaleValue: -zerovalue,
                    endScaleValue: 1 - zerovalue,
                    value: value,
                    showZeroLevel: value !== 0 && zerovalue !== 0 && zerovalue !== 1,
                    showTarget: false,
                    onIncidentOccurred: utils.renderHelper.widgetIncidentOccurred,
                    tooltip: {
                        container: dashboard.utils.tooltipContainerSelector,
                        customizeTooltip: function() {
                            return {text: barOptionsContainer.tooltipText}
                        },
                        zIndex: 100
                    },
                    size: {
                        width: Math.round(barOptionsContainer.defaultBestCharacterCount * this._charWidth),
                        height: 20
                    }
                })
            },
            _getTotals: function() {
                var that = this,
                    res = [],
                    columns = that.options.ViewModel.Columns || [];
                $.each(columns, function(_, column) {
                    var columnName = column.DataId,
                        totalsViewModel = column.Totals || [];
                    $.each(totalsViewModel, function(_, totalModel) {
                        res.push({
                            column: columnName,
                            summaryType: 'custom',
                            displayFormat: stringUtils.format(totalModel.Caption, that.dataController.getTotalValue(totalModel.DataId))
                        })
                    })
                });
                return res
            },
            _calculateCustomSummary: function(options) {
                options.totalValue = 0
            },
            _getColumns: function() {
                var that = this,
                    res = [],
                    viewModel = that.options.ViewModel,
                    gridName = that.options.Name,
                    columns = viewModel.Columns || [],
                    columnsCount = columns.length,
                    styleSettingsInfo;
                $.each(columns, function(columnIndex, column) {
                    var fieldName = column.DataId,
                        columnName = gridName + fieldName,
                        gridColumn = {
                            dataField: fieldName,
                            dataType: COLUMN_DATA_TYPE,
                            encodeHtml: that._encodeHtml,
                            caption: column.Caption,
                            alignment: column.HorzAlignment === "Right" ? that.TextAlignment.Right : that.TextAlignment.Left,
                            headerAlignment: that.TextAlignment.Left
                        };
                    switch (column.DisplayMode) {
                        case that.DisplayMode.Value:
                            gridColumn.cellTemplate = function($container, options) {
                                var text = that._getCellData(options.data, fieldName).displayText;
                                utils.renderHelper.html($container, text, that._encodeHtml);
                                that._styleSettingsProvider.applyStyleSettings($container, that._getStyleSettingsInfo(options.data, fieldName), false, columnName)
                            };
                            gridColumn.resized = function() {
                                var visibleWidthReset = that.dataGrid && that.dataGrid.columnOption(columnIndex, 'visibleWidth') === undefined,
                                    $gridRootElement = that.dataGrid ? that.dataGrid._$element : undefined;
                                if ($gridRootElement)
                                    that._styleSettingsProvider.updateBarWidth($gridRootElement, columnName)
                            };
                            break;
                        case that.DisplayMode.Delta:
                            gridColumn.cellTemplate = function($container, options) {
                                styleSettingsInfo = that._getStyleSettingsInfo(options.data, fieldName);
                                that._renderDelta($container, that._getCellData(options.data, fieldName), styleSettingsInfo);
                                that._styleSettingsProvider.applyStyleSettings($container, styleSettingsInfo, true)
                            };
                            break;
                        case that.DisplayMode.Sparkline:
                            gridColumn.alignment = that.TextAlignment.Left;
                            gridColumn.cssClass = 'dx-dashboard-datagrid-column-visible-cell-content';
                            gridColumn.cellTemplate = function($container, options) {
                                styleSettingsInfo = that._getStyleSettingsInfo(options.data, fieldName);
                                that._renderSparkline({
                                    columnName: columnName,
                                    parentContainer: $container,
                                    sparklineData: options.data[fieldName],
                                    rowIndex: options.rowIndex,
                                    columnIndex: options.column.index,
                                    columnsCount: columnsCount,
                                    columnViewModel: column
                                }, that._getCellData(options.data, fieldName), styleSettingsInfo);
                                that._styleSettingsProvider.applyStyleSettings($container, styleSettingsInfo, true)
                            };
                            gridColumn.resized = function() {
                                var visibleWidthReset = that.dataGrid && that.dataGrid.columnOption(columnIndex, 'visibleWidth') === undefined,
                                    $gridRootElement = that.dataGrid ? that.dataGrid._$element : undefined;
                                if ($gridRootElement)
                                    that._changeGridSparklineColumnsWidth($gridRootElement, columnName, visibleWidthReset, column)
                            };
                            break;
                        case that.DisplayMode.Bar:
                            gridColumn.alignment = that.TextAlignment.Left;
                            gridColumn.cssClass = 'dx-dashboard-datagrid-column-visible-cell-content';
                            gridColumn.cellTemplate = function($container, options) {
                                var barData = that._getCellData(options.data, fieldName);
                                that._renderBar({
                                    columnName: columnName,
                                    parentContainer: $container,
                                    rowIndex: options.rowIndex,
                                    columnIndex: options.column.index,
                                    columnsCount: columnsCount,
                                    tooltipText: barData.text,
                                    value: barData.normalizedValue,
                                    zeroValue: barData.zeroValue,
                                    defaultBestCharacterCount: column.DefaultBestCharacterCount
                                });
                                that._styleSettingsProvider.applyStyleSettings($container, that._getStyleSettingsInfo(options.data, fieldName), true)
                            };
                            gridColumn.resized = function() {
                                var visibleWidthReset = that.dataGrid && that.dataGrid.columnOption(columnIndex, 'visibleWidth') === undefined,
                                    $gridRootElement = that.dataGrid ? that.dataGrid._$element : undefined;
                                if ($gridRootElement)
                                    that._changeGridBarColumnsWidth($gridRootElement, columnName, visibleWidthReset, column)
                            };
                            break;
                        case that.DisplayMode.Image:
                            gridColumn.cellTemplate = function(container, options) {
                                var imageData = that._getCellData(options.data, fieldName),
                                    base64String = 'data:image/png;base64,' + that._convertByteArrayToBase64(imageData.value),
                                    imageContainer = $('<img />').attr('src', base64String);
                                imageContainer.appendTo(container);
                                that._styleSettingsProvider.applyStyleSettings(container, that._getStyleSettingsInfo(options.data, fieldName), true)
                            };
                            break
                    }
                    res.push(gridColumn)
                });
                return res
            },
            _getCellData: function(data, columnName) {
                return data[columnName + dataPostfix].getData()
            },
            _getStyleSettingsInfo: function(data, columnName) {
                return data[columnName + dataPostfix].getStyleSettingsInfo()
            },
            _getUniqueValue: function(data, columnName) {
                return data[columnName + dataPostfix].getUniqueValue()
            },
            _convertByteArrayToBase64: function(byteArray) {
                var bytes = new Uint8Array(byteArray),
                    len = bytes.byteLength,
                    binary = '';
                for (var i = 0; i < len; i++)
                    binary += String.fromCharCode(bytes[i]);
                return window.btoa(binary)
            },
            _applySelection: function() {
                var selectedValues = this._getSelectedValues();
                if (selectedValues)
                    this._setGridSelection(selectedValues)
            },
            _resize: function(oldSize, newSize) {
                this.callBase(oldSize, newSize);
                this.dataGrid.resize()
            },
            _getDataPoint: function(element) {
                var that = this;
                return {
                        getValues: function(name) {
                            return name === 'Default' ? that._getDimensionValues(element.data) : null
                        },
                        getDeltaIds: function() {
                            return that._getColumnDataIdsByColumnType('Delta')
                        },
                        getMeasureIds: function() {
                            return that._getColumnDataIdsByColumnType('Measure')
                        },
                        getSelectionValues: function(name) {
                            return that._getRowsValues(element.data)
                        }
                    }
            },
            _getDimensionValues: function(row) {
                return this.dataController.getDimensionValues(row.index)
            },
            _getColumnsByColumnType: function(columnType) {
                var columns = this.options.ViewModel.Columns,
                    foundColumns = [];
                $.grep(columns, function(column) {
                    if (column.ColumnType === columnType)
                        foundColumns.push(column)
                });
                return foundColumns
            },
            _getColumnDataIdsByColumnType: function(columnType) {
                var that = this,
                    columns = that._getColumnsByColumnType(columnType),
                    ids = [];
                $.each(columns, function(_, column) {
                    ids.push(column.DataId)
                });
                return ids
            },
            _getElementInteractionValue: function(element, viewModel) {
                return this._getRowsValues(element.data)
            },
            _getCellClickHandler: function() {
                var that = this;
                return function(data) {
                        if (data.rowType === 'data')
                            that._raiseItemClick(data)
                    }
            },
            _getCellHoverHandler: function() {
                var that = this;
                return function(data) {
                        that._raiseItemHover(data, data.eventType == "mouseover")
                    }
            },
            _getWidget: function() {
                return this.dataGrid
            },
            _setGridSelection: function(values, keyProcessingDelegate) {
                if (values && values.length > 0) {
                    var that = this,
                        selectionKeys = that.dataController.getSelectedRowKeys(values);
                    that._selectRows(keyProcessingDelegate ? keyProcessingDelegate(selectionKeys) : selectionKeys)
                }
            },
            _selectRows: function(data) {
                if (this.dataGrid)
                    this.dataGrid.selectRows(data)
            },
            _isMultiDataSupported: function() {
                return true
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file mapItem.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            stringUtils = DX.require("/utils/utils.string"),
            formatter = dashboard.data.formatter,
            localizer = dashboard.data.localizer,
            viewerItems = dashboard.viewerItems;
        viewerItems.map = {cssClassNames: {initialExtent: 'dx-icon-dashboard-map-initial-extent'}};
        var projection = function() {
                var parameters = DX.viz.map.projection.get("mercator").source(),
                    _to = parameters.to,
                    _from = parameters.from;
                parameters.to = function(coordinates) {
                    var coords = _to(coordinates);
                    return [clamp(coords[0], -1, +1), coords[1]]
                };
                parameters.from = function(coordinates) {
                    var coords = [clamp(coordinates[0], -1, +1), coordinates[1]];
                    return _from(coords)
                };
                return DX.viz.map.projection(parameters);
                function clamp(value, min, max) {
                    return Math.min(Math.max(value, min), max)
                }
            }();
        viewerItems.mapItem = viewerItems.baseItem.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options);
                this.viewportChanged = false
            },
            clearSelection: function() {
                if (this.mapViewer)
                    this.mapViewer.clearSelection()
            },
            getInfo: function() {
                return $.extend(true, this.callBase(), {viewport: this._getViewport()})
            },
            _ensureInternalToolbarIsRendered: function($internalToolbarDiv) {
                var $initialExtentDiv = $internalToolbarDiv.find('.' + viewerItems.map.cssClassNames.initialExtent);
                if (!$initialExtentDiv || $initialExtentDiv.length == 0) {
                    $initialExtentDiv = $('<div/>', {
                        'class': viewerItems.map.cssClassNames.initialExtent,
                        title: localizer.getString(dashboard.localizationId.buttonNames.InitialExtent)
                    }).appendTo($internalToolbarDiv);
                    $initialExtentDiv.on('click', this._getInitialExtentHandler())
                }
                return $initialExtentDiv
            },
            _getMapViewerOptions: function() {
                var that = this,
                    viewModel = that.options.ViewModel;
                return {
                        projection: projection,
                        encodeHtml: that._encodeHtml,
                        background: {
                            borderWidth: 0,
                            borderColor: 'none'
                        },
                        controlBar: {enabled: false},
                        zoomFactor: that._calculateZoomFactor(viewModel.Viewport, that.$contentRoot.width(), that.$contentRoot.height()),
                        maxZoomFactor: 1 << 18,
                        center: [viewModel.Viewport.CenterPointLongitude, viewModel.Viewport.CenterPointLatitude],
                        panningEnabled: viewModel.LockNavigation !== true,
                        zoomingEnabled: viewModel.LockNavigation !== true
                    }
            },
            _getLabelSettings: function(viewModel) {
                return {label: {
                            enabled: !!viewModel.ShapeTitleAttributeName,
                            dataField: 'title'
                        }}
            },
            _calculateZoomFactor: function(viewport, width, height) {
                var min = width < height ? width : height,
                    mapWidth = this._translateLon(viewport.RightLongitude, min) - this._translateLon(viewport.LeftLongitude, min),
                    mapHeight = this._translateLat(viewport.BottomLatitude, min) - this._translateLat(viewport.TopLatitude, min),
                    latitudeZoom = width / mapWidth,
                    longitudeZoom = height / mapHeight,
                    zoom = latitudeZoom < longitudeZoom ? latitudeZoom : longitudeZoom;
                if (viewport.CreateViewerPaddings)
                    zoom *= 0.95;
                return zoom
            },
            _translateLon: function(lon, size) {
                var lon_ = lon * Math.PI / 180;
                return size / 2 + size / (2 * Math.PI) * lon_
            },
            _translateLat: function(lat, size) {
                var lat_ = lat * Math.PI / 180;
                return size / 2 - size / (2 * Math.PI) * Math.log(Math.tan(Math.PI / 4 + lat_ / 2))
            },
            _getMapDataSource: function(mapItems, titleName) {
                var mapDataSource = [],
                    data,
                    points,
                    segments,
                    segmentData;
                for (var i = 0; i < mapItems.length; i++) {
                    data = [];
                    if (mapItems[i].Latitude && mapItems[i].Longitude && mapItems[i].Size)
                        data.push([mapItems[i].Longitude, mapItems[i].Latitude]);
                    if (mapItems[i].Segments) {
                        segments = mapItems[i].Segments;
                        for (var j = 0; j < segments.length; j++) {
                            points = segments[j].Points;
                            segmentData = [];
                            for (var k = 0; k < points.length; k++)
                                segmentData.push([points[k].Longitude, points[k].Latitude]);
                            data.push(segmentData)
                        }
                    }
                    mapDataSource.push({
                        coordinates: data,
                        attributes: []
                    });
                    for (var j = 0; j < mapItems[i].Attributes.length; j++)
                        if (mapItems[i].Attributes[j].Name === titleName) {
                            mapDataSource[i].attributes.title = mapItems[i].Attributes[j].Value;
                            break
                        }
                }
                return mapDataSource
            },
            _getLegend: function(legendModel) {
                var legend = legendModel && !!legendModel.Visible ? {} : undefined;
                if (legend)
                    this._updateLegendPosition(legend, legendModel);
                return legend
            },
            _updateLegendPosition: function(legend, legendModel) {
                switch (legendModel.Orientation) {
                    case'Vertical':
                        legend.orientation = 'vertical';
                        legend.inverted = true;
                        break;
                    case'Horizontal':
                        legend.orientation = 'horizontal';
                        break;
                    default:
                        break
                }
                switch (legendModel.Position) {
                    case'TopLeft':
                        legend.verticalAlignment = 'top';
                        legend.horizontalAlignment = 'left';
                        break;
                    case'TopCenter':
                        legend.verticalAlignment = 'top';
                        legend.horizontalAlignment = 'center';
                        break;
                    case'TopRight':
                        legend.verticalAlignment = 'top';
                        legend.horizontalAlignment = 'right';
                        break;
                    case'BottomLeft':
                        legend.verticalAlignment = 'bottom';
                        legend.horizontalAlignment = 'left';
                        break;
                    case'BottomCenter':
                        legend.verticalAlignment = 'bottom';
                        legend.horizontalAlignment = 'center';
                        break;
                    case'BottomRight':
                        legend.verticalAlignment = 'bottom';
                        legend.horizontalAlignment = 'right';
                        break;
                    default:
                        break
                }
            },
            _isSelected: function(current) {
                var selectedValues = this._getSelectedValues(),
                    selected = false,
                    equals;
                if (selectedValues && selectedValues.length > 0 && selectedValues[0].length === current.length)
                    for (var i = 0; i < selectedValues.length; i++) {
                        equals = true;
                        for (var j = 0; j < current.length; j++)
                            if (selectedValues[i][j] !== current[j]) {
                                equals = false;
                                break
                            }
                        if (equals) {
                            selected = true;
                            break
                        }
                    }
                return selected
            },
            _getToolTip: function(name, value) {
                return stringUtils.format("{0}: {1}", this._getHtml(name), this._getHtml(value))
            },
            _getColors: function(colorModels) {
                var colors;
                if (colorModels) {
                    colors = [];
                    for (var i = 0; i < colorModels.length; i++)
                        colors.push('rgb(' + colorModels[i].R + ', ' + colorModels[i].G + ', ' + colorModels[i].B + ')');
                    return colors
                }
            },
            _updateRangeStops: function(rangeStops, min, max, percent) {
                var res = [];
                for (var i = 0; i < rangeStops.length; i++)
                    res.push(rangeStops[i]);
                if (percent)
                    this._updatePercentRangeStops(res, min, max);
                if (res.length > 0 && res[0] > min)
                    res[0] = min;
                if (res[res.length - 1] < max)
                    res.push(max);
                else
                    res.push(res[res.length - 1] + 1);
                return res
            },
            _updatePercentRangeStops: function(rangeStops, min, max) {
                for (var i = 0; i < rangeStops.length; i++)
                    rangeStops[i] = min + rangeStops[i] / 100 * (max - min)
            },
            _getViewport: function() {
                var topLeft = this.mapViewer.convertCoordinates(0, 0),
                    bottomRight = this.mapViewer.convertCoordinates(this.$contentRoot.width(), this.$contentRoot.height()),
                    viewport = this.mapViewer.viewport(),
                    center = this.mapViewer.center();
                return {
                        LeftLongitude: !!topLeft[0] ? topLeft[0] : viewport[0],
                        TopLatitude: !!topLeft[1] ? topLeft[1] : viewport[1],
                        RightLongitude: !!bottomRight[0] ? bottomRight[0] : viewport[2],
                        BottomLatitude: !!bottomRight[1] ? bottomRight[1] : viewport[3],
                        CenterPointLongitude: center[0],
                        CenterPointLatitude: center[1]
                    }
            },
            _getClientContext: function() {
                return {
                        viewport: this._getViewport(),
                        clientSize: {
                            width: this.$contentRoot.width(),
                            height: this.$contentRoot.height()
                        }
                    }
            },
            _updateClientStateInternal: function(clientState) {
                var viewport = clientState.viewport;
                this._updateViewport(clientState.viewport)
            },
            _updateViewport: function(viewport) {
                this.mapViewer.zoomFactor(this._calculateZoomFactor(viewport, this.$contentRoot.width(), this.$contentRoot.height()), true);
                this.mapViewer.center([viewport.CenterPointLongitude, viewport.CenterPointLatitude], true)
            },
            _resize: function(oldSize, newSize) {
                this.callBase(oldSize, newSize);
                var viewport = this.changedViewport ? this.changedViewport.viewport : this.options.ViewModel.Viewport;
                this._updateViewport(viewport);
                this.mapViewer.render();
                this._onClientStateUpdate(this._getClientContext())
            },
            _onViewPortChanged: function() {
                this.changedViewport = this._getClientContext();
                this._onClientStateUpdate(this.changedViewport)
            },
            _onInitialExtent: function() {
                this._updateViewport(this.options.ViewModel.Viewport);
                this.changedViewport = null;
                this._onClientStateUpdate(this._getClientContext())
            },
            _hasInternalButtons: function() {
                return this.options.ViewModel.LockNavigation !== true
            },
            _getInitialExtentHandler: function() {
                var that = this;
                that._initialExtentHandler = that._initialExtentHandler || function() {
                    that._onInitialExtent()
                };
                return that._initialExtentHandler
            },
            _getWidget: function() {
                return this.mapViewer
            },
            _subscribeItemEvents: function() {
                var that = this;
                this.mapViewer.option('onCenterChanged', function() {
                    that._onViewPortChanged()
                });
                this.mapViewer.option('onZoomFactorChanged', function() {
                    that._onViewPortChanged()
                })
            },
            _unsubscribeItemEvents: function() {
                this.mapViewer.option('onCenterChanged', null);
                this.mapViewer.option('onZoomFactorChanged', null)
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file choroplethMapItem.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            formatter = dashboard.data.formatter,
            viewerItems = dashboard.viewerItems,
            deltaIndicatorType = DX.viz.indicators.consts.deltaIndicator.indicatorType;
        viewerItems.choroplethMapItem = viewerItems.mapItem.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options)
            },
            selectTuple: function(tuple, state) {
                var that = this;
                $.each(that.mapViewer.getLayerByName('area').getElements(), function(index, item) {
                    if (item.attribute('selectionName') == tuple[0].Value)
                        item.selected(state)
                })
            },
            setSelection: function(values) {
                this.callBase(values);
                var that = this;
                this.clearSelection();
                $.each(that.mapViewer.getLayerByName('area').getElements(), function(index, item) {
                    $.each(values, function(i, value) {
                        if (item.attribute('selectionName') == value)
                            item.selected(true)
                    })
                })
            },
            updateContentState: function() {
                this.mapViewer.option("layers[0].hoverEnabled", this._getCustomHoverEnabled())
            },
            renderContent: function(changeExisting) {
                var options = this._getMapViewerOptions();
                options = $.extend(true, options, this._getChoroplethMapViewerOptions());
                if (changeExisting && this.mapViewer) {
                    this._unsubscribeItemEvents();
                    this.mapViewer.option(options)
                }
                else
                    this.mapViewer = new DX.viz.map.dxVectorMap(this.$contentRoot, options);
                this._subscribeItemEvents()
            },
            _getChoroplethMapViewerOptions: function() {
                var that = this,
                    viewModel = that.options.ViewModel,
                    mapItems = viewModel.MapItems,
                    mapDataSource = that._getMapDataSource(mapItems, viewModel.ShapeTitleAttributeName),
                    choroplethColorizer = viewModel.ChoroplethColorizer,
                    tooltipAttributeName = viewModel.ToolTipAttributeName,
                    tooltipMeasures = viewModel.TooltipMeasures,
                    colors,
                    rangeStops,
                    legend,
                    i;
                for (i = 0; i < mapItems.length; i++) {
                    var attributeText = that._findAttributeValueByName(mapItems[i].Attributes, tooltipAttributeName);
                    mapDataSource[i].attributes.tooltip = '<b>' + that._getHtml(attributeText) + '</b>'
                }
                if (choroplethColorizer && that.dataController.hasRecords()) {
                    if (choroplethColorizer.ValueName) {
                        that._fillValueMapDataSourceAttrs(mapDataSource, choroplethColorizer, tooltipMeasures, mapItems);
                        rangeStops = that._getRangeStops(choroplethColorizer);
                        colors = that._getColors(choroplethColorizer.Colorizer.Colors);
                        if (!colors)
                            colors = rangeStops.length > 2 ? ['#5F8195', '#B55951'] : ['#5F8195'];
                        legend = that._getColorLegend(viewModel.Legend, that.dataController.getMeasureDescriptorById(choroplethColorizer.ValueId))
                    }
                    if (choroplethColorizer.DeltaValueName) {
                        that._fillDeltaMapDataSourceAttrs(mapDataSource, choroplethColorizer, tooltipMeasures, mapItems);
                        colors = ['rgb(229, 82, 83)', 'rgb(224, 194, 58)', 'rgb(154, 181, 126)'];
                        rangeStops = [0, 1, 2, 3]
                    }
                }
                return {
                        layers: [$.extend({
                                name: "area",
                                type: "area",
                                data: mapDataSource
                            }, that._getArea(viewModel, colors, rangeStops))],
                        onClick: function(e) {
                            if (e.target && e.target.layer.name === "area" && e.target.attribute('selectionName'))
                                that._raiseItemClick(e.target)
                        },
                        legends: [legend],
                        tooltip: {
                            enabled: true,
                            zIndex: 100,
                            container: dashboard.utils.tooltipContainerSelector,
                            customizeTooltip: function(arg) {
                                if (arg.layer.name === "area")
                                    return {html: arg.attribute('tooltip')}
                            }
                        }
                    }
            },
            _getColorLegend: function(legendViewModel, measureDescriptor) {
                var legend = this._getLegend(legendViewModel);
                if (legend) {
                    legend.source = {
                        layer: "area",
                        grouping: "color"
                    };
                    legend.customizeText = function(arg) {
                        return measureDescriptor.format(arg.start)
                    }
                }
                return legend
            },
            _fillMeasureToolTip: function(mapDataSourceItem, attribute, tooltipMeasures) {
                var displayText,
                    tooltipViewModel,
                    i;
                if (tooltipMeasures)
                    for (i = 0; i < tooltipMeasures.length; i++) {
                        tooltipViewModel = tooltipMeasures[i];
                        displayText = this.dataController.getDisplayText(attribute, tooltipViewModel.DataId);
                        if (displayText != null)
                            mapDataSourceItem.attributes.tooltip += '<br>' + this._getToolTip(tooltipViewModel.Caption, displayText)
                    }
            },
            _fillValueMapDataSourceAttrs: function(mapDataSource, choroplethColorizer, tooltipMeasures, mapItems) {
                var attributeName = choroplethColorizer.AttributeName,
                    attribute,
                    selectionName,
                    displayText;
                for (var i = 0; i < mapItems.length; i++) {
                    attribute = this._findAttributeValueByName(mapItems[i].Attributes, attributeName);
                    selectionName = this.dataController.getUniqueValue(attribute);
                    if (selectionName) {
                        displayText = this.dataController.getDisplayText(attribute, choroplethColorizer.ValueId);
                        mapDataSource[i].attributes.selectionName = selectionName;
                        mapDataSource[i].attributes.selected = this._isSelected([selectionName]);
                        mapDataSource[i].attributes.value = this.dataController.getValue(attribute, choroplethColorizer.ValueId);
                        mapDataSource[i].attributes.tooltip += '<br>' + this._getToolTip(choroplethColorizer.ValueName, displayText);
                        this._fillMeasureToolTip(mapDataSource[i], attribute, tooltipMeasures)
                    }
                }
            },
            _fillDeltaMapDataSourceAttrs: function(mapDataSource, choroplethColorizer, tooltipMeasures, mapItems) {
                var attributeName = choroplethColorizer.AttributeName,
                    selectionName,
                    attribute,
                    toolTip,
                    deltaValue,
                    value,
                    isGood,
                    indicatorType;
                for (var i = 0; i < mapItems.length; i++) {
                    attribute = this._findAttributeValueByName(mapItems[i].Attributes, attributeName);
                    selectionName = this.dataController.getUniqueValue(attribute);
                    if (selectionName) {
                        mapDataSource[i].attributes.selectionName = selectionName;
                        mapDataSource[i].attributes.selected = this._isSelected([selectionName]);
                        deltaValue = this.dataController.getDeltaValue(attribute, choroplethColorizer.DeltaValueId);
                        isGood = deltaValue.getIsGood().getValue();
                        indicatorType = this._convertIndicatorType(deltaValue.getIndicatorType().getValue());
                        mapDataSource[i].attributes.value = this._getDeltaColorValue(indicatorType, isGood);
                        toolTip = '<br>' + this._getToolTip(choroplethColorizer.ActualValueName, deltaValue.getActualValue().getDisplayText());
                        value = this._getDeltaValue(deltaValue, choroplethColorizer.DeltaValueType);
                        if (value)
                            toolTip += '<br>' + this._getToolTip(choroplethColorizer.DeltaValueName, value.getDisplayText());
                        mapDataSource[i].attributes.tooltip += toolTip;
                        this._fillMeasureToolTip(mapDataSource[i], attribute, tooltipMeasures)
                    }
                }
            },
            _getDeltaValue: function(deltaValue, deltaValueType) {
                switch (deltaValueType) {
                    case'AbsoluteVariation':
                        return deltaValue.getAbsoluteVariation();
                    case'PercentVariation':
                        return deltaValue.getPercentVariation();
                    case'PercentOfTarget':
                        return deltaValue.getPercentOfTarget();
                    case'ActualValue':
                    default:
                        return null
                }
            },
            _findAttributeValueByName: function(attributes, attributeName) {
                for (var i = 0; i < attributes.length; i++)
                    if (attributes[i].Name === attributeName)
                        return attributes[i].Value
            },
            _getRangeStops: function(choroplethColorizer) {
                var minMax = this.dataController.getMinMax(choroplethColorizer.ValueId);
                return this._updateRangeStops(choroplethColorizer.Colorizer.RangeStops, minMax.min, minMax.max, choroplethColorizer.Colorizer.UsePercentRangeStops)
            },
            _convertIndicatorType: function(type) {
                var indicatorTypes = ["none", "up", "down", "warning"];
                return indicatorTypes[type]
            },
            _getDeltaColorValue: function(indicatorType, isGood) {
                switch (indicatorType) {
                    case deltaIndicatorType.up:
                    case deltaIndicatorType.down:
                        return isGood ? 2.5 : 0.5;
                    case deltaIndicatorType.warning:
                        return 1.5;
                    default:
                        return -1
                }
            },
            _getArea: function(viewModel, colors, rangeStops) {
                var that = this,
                    selectionDisabled = that._selectionMode() === 'none';
                return $.extend(that._getLabelSettings(viewModel), {
                        colorGroupingField: 'value',
                        colorGroups: rangeStops,
                        palette: colors,
                        customize: function(items) {
                            $.each(items, function(_, item) {
                                item.selected(item.attribute('selected'));
                                if (selectionDisabled || item.attribute('value') === undefined)
                                    item.applySettings({
                                        hoveredBorderColor: null,
                                        hoveredClass: null,
                                        hoverEnabled: false
                                    })
                            })
                        },
                        selectionMode: 'multiple'
                    })
            },
            _getDataPoint: function(element) {
                var that = this;
                return {
                        getValues: function() {
                            return that._getElementInteractionValue(element, that.options.ViewModel)
                        },
                        getMeasureIds: function() {
                            return [that.options.ViewModel.ChoroplethColorizer.ValueId]
                        },
                        getDeltaIds: function() {
                            return []
                        }
                    }
            },
            _getElementInteractionValue: function(element) {
                return !!this.options.ViewModel.ChoroplethColorizer ? [element.attribute('selectionName')] : []
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file geoPointMapItemBase.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            formatter = dashboard.data.formatter,
            viewerItems = dashboard.viewerItems;
        viewerItems.geoPointMapItemBase = viewerItems.mapItem.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options);
                var that = this;
                that.raiseTimerClusterizationDataRequest = function() {
                    that._onDataRequest();
                    clearTimeout(that.timer);
                    that.timer = false
                }
            },
            initialDataRequest: function() {
                this._raiseClusterizationDataRequest()
            },
            selectTuple: function(tuple, state) {
                var that = this;
                $.each(that._getMarkerLayers(), function(_, layer) {
                    $.each(layer.getElements(), function(_, item) {
                        if (item.attribute('latSelection') == tuple[0].Value[0] && item.attribute('lonSelection') == tuple[0].Value[1])
                            item.selected(state)
                    })
                })
            },
            setSelection: function(values) {
                this.callBase(values);
                var that = this;
                that.clearSelection();
                $.each(that._getMarkerLayers(), function(_, layer) {
                    $.each(layer.getElements(), function(_, item) {
                        $.each(values, function(i, value) {
                            if (item.attribute('latSelection') == value[0] && item.attribute('lonSelection') == value[1])
                                item.selected(true)
                        })
                    })
                })
            },
            renderContent: function(changeExisting) {
                var options = $.extend(true, this._getMapViewerOptions(), this._getGeoPointMapViewerOptions());
                if (changeExisting && this.mapViewer) {
                    this._unsubscribeItemEvents();
                    this.mapViewer.option(options)
                }
                else
                    this.mapViewer = new DX.viz.map.dxVectorMap(this.$contentRoot, options);
                this._subscribeItemEvents()
            },
            renderPartialContent: function() {
                var viewModel = this.options.ViewModel;
                this._updateMarkerLayers(viewModel)
            },
            updateContentState: function() {
                var that = this,
                    layers = that.mapViewer.option("layers");
                $.each(layers, function(_, layer) {
                    if (layer.type === 'marker')
                        layer.hoverEnabled = that._getCustomHoverEnabled()
                });
                this.mapViewer.option("layers", layers)
            },
            _getGeoPointMapViewerOptions: function() {
                var that = this,
                    viewModel = that.options.ViewModel;
                return {
                        layers: that._configureLayers(viewModel),
                        onClick: function(e) {
                            if (e.target && e.target.layer.type === 'marker')
                                that._raiseItemClick(e.target)
                        },
                        legends: [that._getColorLegend(viewModel)],
                        tooltip: {
                            enabled: true,
                            zIndex: 100,
                            container: dashboard.utils.tooltipContainerSelector,
                            customizeTooltip: function(arg) {
                                var dimensionText,
                                    measureText,
                                    mainText,
                                    resultHtml = "";
                                if (arg.layer.type === "marker") {
                                    dimensionText = arg.attribute('dimensionsTooltip');
                                    mainText = arg.attribute('tooltip');
                                    measureText = arg.attribute('measuresTooltip');
                                    if (dimensionText)
                                        resultHtml += '<tr><td>' + dimensionText + '</td></tr>';
                                    if (mainText)
                                        resultHtml += '<tr><td>' + mainText + (measureText ? '' : '</td></tr>');
                                    if (measureText)
                                        resultHtml += (mainText ? '<br>' : '</td></tr>') + measureText + '</td></tr>'
                                }
                                return {html: resultHtml != "" ? '<table align="left">' + resultHtml + '</table>' : ""}
                            }
                        }
                    }
            },
            _getMarkerLayers: function(){},
            _configureLayers: function(viewModel) {
                var layers = [$.extend({
                            name: "area",
                            type: "area",
                            data: this._getMapDataSource(viewModel.MapItems, viewModel.ShapeTitleAttributeName)
                        }, this._getArea(viewModel))];
                return layers.concat(this._configureMarkerLayers(viewModel))
            },
            _configureMarkerLayers: function(viewModel){},
            _updateMarkerLayers: function(viewModel) {
                var layers = this.mapViewer.option("layers"),
                    markerLayers = this._configureMarkerLayers(viewModel);
                $.each(markerLayers, function(index, layer) {
                    layers[index + 1] = layer
                });
                this.mapViewer.option('layers', layers)
            },
            _getMarker: function(viewModel, markerDataSource) {
                var style;
                return {
                        customize: function(items) {
                            $.each(items, function(_, item) {
                                item.selected(item.attribute('selected'));
                                style = {color: item.attribute('color')};
                                var size = item.attribute('size');
                                if (size)
                                    style.size = size;
                                item.applySettings(style)
                            })
                        },
                        selectionMode: 'multiple'
                    }
            },
            _getArea: function(viewModel) {
                var that = this;
                return $.extend(that._getLabelSettings(viewModel), {
                        hoverEnabled: false,
                        selectionMode: that._selectionMode()
                    })
            },
            _getColorLegend: function(viewModel) {
                var legend = this._getLegend(viewModel.ColorLegend);
                if (legend)
                    legend.source = {grouping: "color"};
                return legend
            },
            _getMinMaxValues: function(markerDataSource) {
                var min,
                    max;
                if (markerDataSource.length > 0)
                    for (var i = 0; i < markerDataSource.length; i++) {
                        if (max === undefined || markerDataSource[i].attributes.value !== undefined && markerDataSource[i].attributes.value > max)
                            max = markerDataSource[i].attributes.value;
                        if (min === undefined || markerDataSource[i].attributes.value !== undefined && markerDataSource[i].attributes.value < min)
                            min = markerDataSource[i].attributes.value
                    }
                return {
                        min: min,
                        max: max
                    }
            },
            _pointsCountTooltip: function(count) {
                return '<b>' + count + ' points</b>'
            },
            _getElementInteractionValue: function(element, viewModel) {
                return [element.attribute('latSelection'), element.attribute('lonSelection')]
            },
            _getDimensionsTooltipHtml: function(tooltipDimensions) {
                var values = [];
                if (tooltipDimensions.length === 1) {
                    if (tooltipDimensions[0].values) {
                        for (var i = 0; i < tooltipDimensions[0].values.length; i++)
                            values.push('<b>' + this._getHtml(tooltipDimensions[0].values[i]) + '</b>');
                        return values.join('<br>')
                    }
                }
                else {
                    for (var i = 0; i < tooltipDimensions.length; i++) {
                        var tooltipDimension = tooltipDimensions[i];
                        if (tooltipDimension.values) {
                            values.push('<b>' + this._getHtml(tooltipDimension.caption) + '</b>');
                            for (var j = 0; j < tooltipDimension.values.length; j++)
                                values.push(this._getHtml(tooltipDimension.values[j]))
                        }
                    }
                    return values.join('<br>')
                }
                return ''
            },
            _getMeasuresTooltipHtml: function(tooltipMeasures) {
                var result = [];
                for (var i = 0; i < tooltipMeasures.length; i++)
                    result.push(this._getToolTip(tooltipMeasures[i].caption, tooltipMeasures[i].value));
                return result.join('<br>')
            },
            _getDataPoint: function(element) {
                var that = this,
                    viewModel = that.options.ViewModel;
                return {
                        getValues: function() {
                            return that._getElementInteractionValue(element, viewModel)
                        },
                        getMeasureIds: function() {
                            return that._getDataPointMeasureIds()
                        },
                        getDeltaIds: function() {
                            return []
                        }
                    }
            },
            _getDataPointMeasureIds: function(){},
            _raiseClusterizationDataRequest: function() {
                if (this.options.ViewModel.EnableClustering) {
                    this._onClientStateUpdate(this._getClientContext());
                    this._onDataRequest()
                }
            },
            _onViewPortChanged: function() {
                this.callBase();
                if (this.options.ViewModel.EnableClustering)
                    if (!this.timer)
                        this.timer = setTimeout(this.raiseTimerClusterizationDataRequest, 500)
            },
            _resize: function(oldSize, newSize) {
                this.callBase(oldSize, newSize);
                this._raiseClusterizationDataRequest()
            },
            _onInitialExtent: function() {
                this.callBase();
                this._raiseClusterizationDataRequest()
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file geoPointMapItem.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            formatter = dashboard.data.formatter,
            viewerItems = dashboard.viewerItems;
        viewerItems.geoPointMapItem = viewerItems.geoPointMapItemBase.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options)
            },
            _getMarkerLayers: function() {
                return [this.mapViewer.getLayerByName('dot'), this.mapViewer.getLayerByName('bubble')]
            },
            _configureMarkerLayers: function(viewModel) {
                var markerDataSource = this._getMarkerDataSource(),
                    dotSettings = markerDataSource.dotDataSource.length > 0 ? this._getDorMarker(viewModel, markerDataSource.dotDataSource) : null,
                    bubbleSettings = markerDataSource.bubbleDataSource.length > 0 ? this._getBubbleMarker(viewModel, markerDataSource.bubbleDataSource) : null;
                return [$.extend({
                            name: "dot",
                            type: "marker",
                            elementType: "dot",
                            data: markerDataSource.dotDataSource
                        }, dotSettings), $.extend({
                            name: "bubble",
                            type: "marker",
                            elementType: "bubble",
                            dataField: "value",
                            data: markerDataSource.bubbleDataSource
                        }, bubbleSettings)]
            },
            _getMarkerDataSource: function() {
                var viewModel = this.options.ViewModel,
                    dotDataSource = [],
                    bubbleDataSource = [],
                    count = this.dataController ? this.dataController.getCount() : 0,
                    tooltip,
                    geoPoint,
                    point;
                for (var i = 0; i < count; i++) {
                    point = this.dataController.getPoint(i);
                    tooltip = this._getToolTip(viewModel.ValueName, point.text);
                    geoPoint = {
                        coordinates: [point.lon, point.lat],
                        attributes: {
                            latSelection: point.latSel,
                            lonSelection: point.lonSel,
                            selected: this._isSelected([point.latSel, point.lonSel]),
                            dimensionsTooltip: this._getDimensionsTooltipHtml(point.tooltipDimensions),
                            measuresTooltip: this._getMeasuresTooltipHtml(point.tooltipMeasures)
                        }
                    };
                    if (point.pointsCount && point.pointsCount > 1) {
                        geoPoint.attributes.value = this._getClusterBubbleSizeIndex(point.pointsCount);
                        geoPoint.attributes.tooltip = this._pointsCountTooltip(point.pointsCount) + '<br>' + tooltip;
                        geoPoint.attributes.color = this._getClusterBubbleColor(point.pointsCount);
                        bubbleDataSource.push(geoPoint)
                    }
                    else {
                        geoPoint.attributes.text = point.text;
                        geoPoint.attributes.tooltip = tooltip;
                        dotDataSource.push(geoPoint)
                    }
                }
                return {
                        dotDataSource: dotDataSource,
                        bubbleDataSource: bubbleDataSource
                    }
            },
            _getDorMarker: function(viewModel, markerDataSource) {
                return $.extend(this._getMarker(viewModel, markerDataSource), {label: {dataField: 'text'}})
            },
            _getBubbleMarker: function(viewModel, markerDataSource) {
                var res = this._getMinMaxValues(markerDataSource);
                return $.extend(true, this._getMarker(viewModel, markerDataSource), {
                        minSize: 30 + res.min * 10,
                        maxSize: 30 + res.max * 10
                    })
            },
            _getColorLegend: function(viewModel){},
            _getClusterBubbleColor: function(value) {
                if (value < 10)
                    return 'rgb(27, 73, 165)';
                if (value < 100)
                    return 'rgb(63, 136, 48)';
                if (value < 1000)
                    return 'rgb(228, 124, 2)';
                return 'rgb(214, 5, 5)'
            },
            _getClusterBubbleSizeIndex: function(value) {
                for (var i = 0; ; i++)
                    if (value < Math.pow(10, i))
                        return i - 1
            },
            _getDataPointMeasureIds: function() {
                var viewModel = this.options.ViewModel,
                    measureIds = [];
                measureIds.push(viewModel.ValueId);
                return measureIds
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file bubbleMapItem.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            formatter = dashboard.data.formatter,
            viewerItems = dashboard.viewerItems,
            Color = DX.require("/color");
        viewerItems.bubbleMapItem = viewerItems.geoPointMapItemBase.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options)
            },
            _getMarkerLayers: function() {
                return [this.mapViewer.getLayerByName('bubble')]
            },
            _configureMarkerLayers: function(viewModel) {
                var markerDataSource = this._getMarkerDataSource(),
                    markerSettings = markerDataSource.length > 0 ? this._getMarker(viewModel, markerDataSource) : null;
                return [$.extend({
                            name: "bubble",
                            type: "marker",
                            elementType: "bubble",
                            dataField: "value",
                            data: markerDataSource
                        }, markerSettings)]
            },
            _getMarkerDataSource: function() {
                var viewModel = this.options.ViewModel,
                    markerDataSource = [],
                    point;
                for (var i = 0; i < this.dataController.getCount(); i++) {
                    point = this.dataController.getPoint(i);
                    markerDataSource.push({
                        coordinates: [point.lon, point.lat],
                        attributes: {
                            latSelection: point.latSel,
                            lonSelection: point.lonSel,
                            selected: this._isSelected([point.latSel, point.lonSel]),
                            value: point.weight || (point.pointsCount > 1 ? 1 : 0),
                            colorValue: point.color || 0,
                            tooltip: this._getBubbleTooltip(viewModel, point.weightText, point.colorText, point.pointsCount),
                            dimensionsTooltip: this._getDimensionsTooltipHtml(point.tooltipDimensions),
                            measuresTooltip: this._getMeasuresTooltipHtml(point.tooltipMeasures)
                        }
                    })
                }
                return markerDataSource
            },
            _getMarker: function(viewModel, markerDataSource) {
                var rangeStops = viewModel.ColorId ? this._getBubbleRangeStops(viewModel.Colorizer, markerDataSource) : [0, 1],
                    colors = this._getBubbleColors(viewModel.Colorizer.Colors, rangeStops.length - 1),
                    minSize,
                    maxSize,
                    options = {
                        palette: colors,
                        colorGroups: rangeStops,
                        colorGroupingField: 'colorValue'
                    };
                if (viewModel.WeightId) {
                    minSize = 20;
                    maxSize = 60
                }
                else {
                    var res = this._getMinMaxValues(markerDataSource);
                    if (res.min !== res.max) {
                        minSize = 20;
                        maxSize = 40
                    }
                    else if (res.min === 1) {
                        minSize = 40;
                        maxSize = 40
                    }
                    else {
                        minSize = 20;
                        maxSize = 20
                    }
                }
                options.minSize = minSize;
                options.maxSize = maxSize;
                return $.extend(true, this.callBase(viewModel, markerDataSource), options)
            },
            _getColorLegend: function(viewModel) {
                var that = this;
                if (!viewModel.ColorId)
                    return;
                var legend = this.callBase(viewModel);
                if (legend) {
                    legend.source.layer = "bubble";
                    legend.customizeText = function(arg) {
                        return that.dataController.formatColor(arg.start)
                    }
                }
                return legend
            },
            _getBubbleTooltip: function(viewModel, weight, color, pointsCount) {
                var strs = [];
                if (pointsCount && pointsCount > 1)
                    strs.push(this._pointsCountTooltip(pointsCount));
                if (weight)
                    strs.push(this._getToolTip(viewModel.WeightName, weight));
                if (color && viewModel.ColorName !== viewModel.WeightName)
                    strs.push(this._getToolTip(viewModel.ColorName, color));
                return strs.join('<br>')
            },
            _getBubbleRangeStops: function(colorizer, markerDataSource) {
                var max = markerDataSource[0].attributes.colorValue,
                    min = markerDataSource[0].attributes.colorValue;
                for (var i = 1; i < markerDataSource.length; i++) {
                    if (markerDataSource[i].attributes.colorValue > max)
                        max = markerDataSource[i].attributes.colorValue;
                    if (markerDataSource[i].attributes.colorValue < min)
                        min = markerDataSource[i].attributes.colorValue
                }
                return this._updateRangeStops(colorizer.RangeStops, min, max, colorizer.UsePercentRangeStops)
            },
            _getBubbleColors: function(colorModels, defaultColorsCount) {
                var colors = this._getColors(colorModels);
                return colors ? colors : this._getDefaultBubbleColorizerColors(defaultColorsCount)
            },
            _getDefaultBubbleColorizerColors: function(count) {
                var startColor = new Color('rgb(54, 170, 206)'),
                    endColor = new Color('rgb(255, 93, 106)'),
                    colors = [];
                if (count === 1)
                    return [startColor.toHex()];
                for (var i = 0; i < count; i++)
                    colors.push(startColor.blend(endColor, i / (count - 1)).toHex());
                return colors
            },
            _getDataPointMeasureIds: function() {
                var viewModel = this.options.ViewModel,
                    measureIds = [];
                measureIds.push(viewModel.WeightId);
                measureIds.push(viewModel.ColorId);
                return measureIds
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file pieMapItem.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            stringUtils = DX.require("/utils/utils.string"),
            formatter = dashboard.data.formatter,
            viewerItems = dashboard.viewerItems;
        viewerItems.pieMapItem = viewerItems.geoPointMapItemBase.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options);
                this.pieUniqueArguments = undefined;
                this.pieArgumentDisplayTexts = undefined
            },
            _getMarkerLayers: function() {
                return [this.mapViewer.getLayerByName('pie')]
            },
            _configureMarkerLayers: function(viewModel) {
                var markerDataSource = this._getMarkerDataSource(),
                    markerSettings = markerDataSource.length > 0 ? this._getMarker(viewModel, markerDataSource) : null;
                return [$.extend({
                            name: "pie",
                            type: "marker",
                            elementType: "pie",
                            dataField: "values",
                            data: markerDataSource
                        }, markerSettings)]
            },
            _getMarkerDataSource: function() {
                var markerDataSource = [],
                    viewModel = this.options.ViewModel,
                    pieSegments = this._getPieSegments(),
                    pies = this._getPiesData(pieSegments, viewModel),
                    rangeStops = viewModel.IsWeighted ? this._getPieRangeStops(pies) : [0, 1, 2],
                    attributes;
                for (var name in pies) {
                    attributes = pies[name].attributes;
                    attributes.size = 20 + this._getRangeStopIndex(pies[name].sizeValue, rangeStops) * 10;
                    markerDataSource.push({
                        coordinates: pies[name].coordinates,
                        attributes: attributes
                    })
                }
                return markerDataSource
            },
            _getColorLegend: function(viewModel) {
                var that = this;
                var legend = this.callBase(viewModel);
                if (legend) {
                    legend.source.layer = "pie";
                    legend.customizeText = function(arg) {
                        return that.pieArgumentDisplayTexts[arg.index]
                    }
                }
                return legend
            },
            _getPieSegments: function() {
                var that = this,
                    viewModel = this.options.ViewModel,
                    pieSegments = [],
                    pieSegment,
                    filledValues = viewModel.Values && viewModel.Values.length > 0,
                    getPieSegment = function(point) {
                        return {
                                clusterCount: point.pointsCount,
                                lat: point.lat,
                                lon: point.lon,
                                latSel: point.latSel,
                                lonSel: point.lonSel,
                                dimensionsTooltipText: that._getDimensionsTooltipHtml(point.tooltipDimensions),
                                measuresTooltipText: that._getMeasuresTooltipHtml(point.tooltipMeasures),
                                value: point.value,
                                valueDisplayText: point.valueDisplayText,
                                argumentValue: point.argument,
                                argumentDisplayText: point.argumentDisplayText
                            }
                    };
                that.pieUniqueArguments = [];
                this.pieArgumentDisplayTexts = [];
                for (var i = 0; i < that.dataController.getCount(); i++)
                    if (viewModel.ArgumentDataId) {
                        pieSegment = getPieSegment(that.dataController.getPoint(i));
                        if ($.inArray(pieSegment.argumentValue, that.pieUniqueArguments) === -1) {
                            that.pieUniqueArguments.push(pieSegment.argumentValue);
                            that.pieArgumentDisplayTexts.push(pieSegment.argumentDisplayText)
                        }
                        pieSegments.push(pieSegment)
                    }
                    else if (filledValues)
                        for (var j = 0; j < viewModel.Values.length; j++) {
                            pieSegment = getPieSegment(that.dataController.getPoint(i, j));
                            if ($.inArray(pieSegment.argumentValue, that.pieUniqueArguments) === -1 || i === 0) {
                                that.pieUniqueArguments.push(pieSegment.argumentValue);
                                that.pieArgumentDisplayTexts.push(pieSegment.argumentDisplayText)
                            }
                            pieSegments.push(pieSegment)
                        }
                return pieSegments
            },
            _getPiesData: function(pieSegments, viewModel) {
                var pies = {},
                    segment,
                    key,
                    tooltip,
                    dimensionsTooltip,
                    measuresTooltip;
                for (var i = 0; i < pieSegments.length; i++) {
                    segment = pieSegments[i];
                    key = stringUtils.format("{0};{1}", segment.lat, segment.lon);
                    if (!pies.hasOwnProperty(key)) {
                        dimensionsTooltip = segment.dimensionsTooltipText;
                        measuresTooltip = segment.measuresTooltipText;
                        pies[key] = {
                            coordinates: [segment.lon, segment.lat],
                            attributes: {
                                latSelection: segment.latSel,
                                lonSelection: segment.lonSel,
                                selected: this._isSelected([segment.latSel, segment.lonSel]),
                                values: this._getEmptyValues(this.pieUniqueArguments.length),
                                dimensionsTooltip: dimensionsTooltip,
                                measuresTooltip: measuresTooltip
                            },
                            sizeValue: viewModel.IsWeighted ? 0 : segment.clusterCount > 1 ? 1 : 0
                        };
                        if (segment.clusterCount > 1)
                            pies[key].attributes.tooltip = this._pointsCountTooltip(segment.clusterCount)
                    }
                    for (var j = 0; j < pies[key].attributes.values.length; j++)
                        if (segment.argumentValue === this.pieUniqueArguments[j])
                            pies[key].attributes.values[j] = segment.value;
                    if (viewModel.Values && viewModel.Values.length > 0)
                        if (segment.argumentValue && segment.value > 0)
                            tooltip = this._getToolTip(segment.argumentDisplayText, segment.valueDisplayText);
                        else
                            tooltip = undefined;
                    else
                        tooltip = segment.argumentDisplayText;
                    if (tooltip)
                        if (pies[key].attributes.tooltip)
                            pies[key].attributes.tooltip += '<br>' + tooltip;
                        else
                            pies[key].attributes.tooltip = tooltip;
                    if (viewModel.IsWeighted)
                        pies[key].sizeValue += segment.value;
                    else if (this._getPieSegmentCount(pies[key]) === 2)
                        pies[key].sizeValue++
                }
                return pies
            },
            _getPieSegmentCount: function(pie) {
                var count = 0;
                for (var i = 0; i < pie.attributes.values.length; i++)
                    if (pie.attributes.values[i] > 0)
                        count++;
                return count
            },
            _getEmptyValues: function(length) {
                var emptyValues = [];
                for (var j = 0; j < length; j++)
                    emptyValues.push(0);
                return emptyValues
            },
            _getPieRangeStops: function(pies) {
                var rangeStops = [],
                    rangeStopsCount = 3,
                    minSizeValue,
                    maxSizeValue;
                for (var name in pies) {
                    if (minSizeValue === undefined || pies[name].sizeValue < minSizeValue)
                        minSizeValue = pies[name].sizeValue;
                    if (maxSizeValue === undefined || pies[name].sizeValue > maxSizeValue)
                        maxSizeValue = pies[name].sizeValue
                }
                for (var i = 0; i <= rangeStopsCount; i++)
                    rangeStops.push(minSizeValue + i / rangeStopsCount * (maxSizeValue - minSizeValue));
                return rangeStops
            },
            _getRangeStopIndex: function(value, rangeStops) {
                if (value < rangeStops[0])
                    return 0;
                for (var i = 0; i < rangeStops.length - 1; i++)
                    if (value >= rangeStops[i] && value < rangeStops[i + 1])
                        return i;
                return rangeStops.length - 1
            },
            _getDataPointMeasureIds: function() {
                var viewModel = this.options.ViewModel,
                    measureIds = [];
                if (viewModel.ArgumentDataId != null && viewModel.Values.length > 0)
                    measureIds.push(viewModel.Values[0]);
                else
                    $.each(viewModel.Values, function(_, value) {
                        measureIds.push(value)
                    });
                return measureIds
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file baseElement.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            viewerItems = dashboard.viewerItems;
        viewerItems.filterElementBaseItem = viewerItems.baseItem.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options);
                this._allowExport = false;
                this._locked = false;
                this.itemMultiselectionEnabled = true
            },
            setSelection: function(values) {
                this.callBase(values);
                this.dataController.update(values)
            },
            renderContent: function(changeExisting) {
                this.callBase(changeExisting)
            },
            initializeData: function(newOptions) {
                this.callBase(newOptions);
                this.isMultiSelectable = this.dataController ? this.dataController.isMultiselectable() : false
            },
            isPaneEmpty: function() {
                return this.callBase() || !this.hasCaption()
            },
            updateInteractivityOptions: function() {
                if (this.isMultiSelectable)
                    this.interactivityController.setOptions(dashboard.dashboardSelectionMode.multiple);
                else
                    this.interactivityController.setOptions(dashboard.dashboardSelectionMode.single)
            },
            _isBorderRequired: function() {
                return false
            },
            _getOptions: function(includeActions){},
            _hasToggleSelectionModeButton: function() {
                return false
            },
            _resize: function(oldSize, newSize) {
                this.callBase(oldSize, newSize)
            },
            _raiseItemClick: function(elements) {
                if (this._isLocked())
                    return;
                var that = this,
                    tuples = [],
                    axisName = that._getAxisNames()[0],
                    newSelectedValues = that.dataController.getInteractionValues(elements, that._getSelectedValues());
                $.each(newSelectedValues, function(_, value) {
                    tuples.push([{
                            AxisName: axisName,
                            Value: value
                        }])
                });
                that.interactivityController.clickAction(tuples)
            },
            _mustSelectingFired: function(values) {
                return true
            },
            _isLocked: function() {
                return this._locked
            },
            _lock: function() {
                this._locked = true
            },
            _unlock: function() {
                this._locked = false
            },
            _isUpdating: function(widget) {
                return !widget || widget._updateLockCount > 0
            },
            _wrapContentRoot: function() {
                return $('<div/>', {}).appendTo(this.$contentRoot)
            },
            _selectTuples: function(tuplesToSelect, unaffectedTuples, isSelect){}
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file listElement.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            viewerItems = dashboard.viewerItems,
            PAGE_SIZE = 100;
        viewerItems.listFilterElement = viewerItems.filterElementBaseItem.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options);
                this.list = undefined;
                this.suppressSelectionChanged = false
            },
            setSelection: function(values) {
                this.callBase(values);
                this._lock();
                try {
                    this.list.option('dataSource', this._getDataSource());
                    this.list.option('selectedItems', this._getSelection())
                }
                finally {
                    this._unlock()
                }
            },
            clearSelection: function(){},
            renderContent: function(changeExisting) {
                var that = this,
                    $root = that._wrapContentRoot(),
                    opts = that._getOptions(true);
                this._lock();
                try {
                    if (changeExisting && that.list)
                        that.list.option(opts);
                    else
                        that.list = new DX.ui.dxList($root, opts)
                }
                finally {
                    this._unlock()
                }
                $root.addClass('dx-list-item-separator-hidden');
                if (that.isPaneEmpty())
                    $root.addClass('dx-list-border-visible')
            },
            _resize: function(oldSize, newSize) {
                this.callBase(oldSize, newSize);
                this.list.updateDimensions()
            },
            _getOptions: function(includeActions) {
                var that = this,
                    onSelectionChanged = undefined,
                    onSelectAllChanged = undefined;
                if (includeActions) {
                    onSelectionChanged = function(e) {
                        if (!that.suppressSelectionChanged)
                            if (that.isMultiSelectable && e.removedItems.length > 0)
                                that._raiseItemClick(e.removedItems);
                            else
                                that._raiseItemClick(e.addedItems)
                    };
                    onSelectAllChanged = function(e) {
                        that.suppressSelectionChanged = true;
                        var selectedItems = e.component.option("selectedItems");
                        if (e.value === true) {
                            var newSelection = $.grep(that.dataController.selection, function(item) {
                                    return $.inArray(item, selectedItems) === -1
                                });
                            e.component.option("selectedItems", that.dataController.selection);
                            if (newSelection.length > 0)
                                that._raiseItemClick(newSelection)
                        }
                        if (e.value === false) {
                            e.component.option("selectedItems", []);
                            if (selectedItems.length > 0)
                                that._raiseItemClick(selectedItems)
                        }
                        that.suppressSelectionChanged = false;
                        return false
                    }
                }
                return {
                        dataSource: that._getDataSource(),
                        selectedItems: that._getSelection(),
                        showSelectionControls: true,
                        focusStateEnabled: false,
                        hoverStateEnabled: false,
                        selectionMode: that.isMultiSelectable ? 'all' : 'single',
                        selectAllText: dashboard.data.ALL_ELEMENT.text,
                        onSelectionChanged: onSelectionChanged,
                        onSelectAllChanged: onSelectAllChanged,
                        pageLoadMode: 'scrollBottom'
                    }
            },
            _getDataSource: function() {
                return {
                        paginate: true,
                        pageSize: PAGE_SIZE,
                        requireTotalCount: true,
                        store: new DX.data.ArrayStore(this.dataController.dataSource)
                    }
            },
            _getSelection: function() {
                return !this.isMultiSelectable && this.dataController.selection.length > 1 ? [this.dataController.selection[0]] : this.dataController.selection
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file comboBoxElement.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            utils = DX.utils,
            commonUtils = DX.require("/utils/utils.common"),
            viewerItems = dashboard.viewerItems;
        viewerItems.cssComboBoxClassNames = {
            item: 'dx-dashboard-combobox-filter-item',
            multiText: 'dx-dashboard-filter-item-multitext'
        };
        viewerItems.comboBoxFilterElement = viewerItems.filterElementBaseItem.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options);
                this.comboBox = undefined;
                this._isFixedHeight = true
            },
            setSelection: function(values) {
                this.callBase(values);
                this._lock();
                try {
                    this.comboBox.option('dataSource', this.dataController.dataSource);
                    if (this.isMultiSelectable)
                        this.comboBox.option('values', this.dataController.selection);
                    else
                        this.comboBox.option('value', this.dataController.selection ? this.dataController.selection[0] : this.dataController.dataSource[0])
                }
                finally {
                    this._unlock()
                }
            },
            clearSelection: function(){},
            renderContent: function(changeExisting) {
                var that = this,
                    $root = that._wrapContentRoot(),
                    controlName = this._getControlName(),
                    opts = that._getOptions(true);
                this._lock();
                try {
                    if (changeExisting && that.comboBox)
                        that.comboBox.option(opts);
                    else
                        that.comboBox = new DX.ui[controlName]($root, opts)
                }
                finally {
                    this._unlock()
                }
            },
            _getControlName: function() {
                return this.isMultiSelectable ? 'dxTagBox' : 'dxSelectBox'
            },
            _getMinContentHeight: function() {
                var controlName = this._getControlName();
                return utils.renderHelper.getControlBox(controlName, this._getOptions(false)).height
            },
            _getInnerBorderClasses: function() {
                var baseClasses = this.callBase();
                if (!this.isPaneEmpty())
                    baseClasses.push(viewerItems.cssComboBoxClassNames.item);
                return baseClasses
            },
            _resize: function(oldSize, newSize) {
                this.callBase(oldSize, newSize)
            },
            _getCustomTemplate: function() {
                var that = this,
                    getItemsText = function(items) {
                        if (items && commonUtils.isArray(items) && items.length > 0)
                            if (items.length == that.dataController.dataSource.length)
                                return dashboard.data.ALL_ELEMENT.text;
                            else {
                                var values = [];
                                $.each(items, function(_, item) {
                                    values.push(item.text)
                                });
                                return values.join('; ')
                            }
                        else
                            return '(none)'
                    };
                return function(items) {
                        var $input = $('<input>').addClass('dx-texteditor-input').val(getItemsText(items)).prop('readonly', true).css({
                                padding: "7px 9px 8px",
                                border: 'none',
                                width: "100%",
                                boxSizing: 'border-box',
                                readOnly: true
                            });
                        $input.addClass(viewerItems.cssComboBoxClassNames.multiText);
                        var $inputWrapper = $('<div>').css({marginRight: "40px"}).append($input);
                        var $textBox = $('<div>').css({
                                float: 'right',
                                width: "37px"
                            }).dxTextBox();
                        return $('<div>').append($textBox).append($inputWrapper)
                    }
            },
            _getOptions: function(includeActions) {
                var that = this,
                    options = {
                        dataSource: that.dataController.dataSource,
                        displayExpr: 'text',
                        valueExpr: 'this',
                        multiSelectEnabled: that.isMultiSelectable,
                        selectAllText: dashboard.data.ALL_ELEMENT.text
                    };
                if (that.isMultiSelectable)
                    $.extend(options, {
                        values: that.dataController.selection,
                        onValuesChanged: !includeActions ? undefined : function(e) {
                            that._raiseItemClick(e.removedItems.length > 0 ? e.removedItems : e.addedItems)
                        },
                        placeholder: '',
                        showSelectionControls: that.isMultiSelectable,
                        showDropButton: true,
                        selectAllText: dashboard.data.ALL_ELEMENT.text,
                        fieldTemplate: that._getCustomTemplate()
                    });
                else
                    $.extend(options, {
                        value: that.dataController.selection ? that.dataController.selection[0] : that.dataController.dataSource[0],
                        onValueChanged: !includeActions ? undefined : function(e) {
                            that._raiseItemClick([e.value])
                        }
                    });
                return options
            }
        })
    })(jQuery, DevExpress);
    /*! Module dashboard, file treeElement.js */
    (function($, DX, undefined) {
        var dashboard = DX.dashboard,
            viewerItems = dashboard.viewerItems;
        viewerItems.treeViewFilterElement = viewerItems.filterElementBaseItem.inherit({
            ctor: function ctor($container, options) {
                this.callBase($container, options);
                this.treeView = undefined
            },
            setSelection: function(values) {
                this.callBase(values);
                this._lock();
                try {
                    this.treeView.option('items', this.dataController.dataSource)
                }
                finally {
                    this._unlock()
                }
            },
            clearSelection: function(){},
            renderContent: function(changeExisting) {
                var that = this,
                    $root = that._wrapContentRoot(),
                    opts = that._getOptions(true);
                this._lock();
                try {
                    if (changeExisting && that.treeView)
                        that.treeView.option(opts);
                    else
                        that.treeView = new DX.ui.dxTreeView($root, opts)
                }
                finally {
                    this._unlock()
                }
                if (that.isPaneEmpty())
                    $root.addClass('dx-treeview-border-visible')
            },
            _resize: function(oldSize, newSize) {
                this.callBase(oldSize, newSize);
                this.treeView.updateDimensions()
            },
            _getOptions: function(includeActions) {
                var that = this,
                    onSelectionChanged = undefined;
                if (includeActions)
                    onSelectionChanged = function(e) {
                        that._raiseItemClick(e.component.getNodes())
                    };
                return {
                        items: that.dataController.dataSource,
                        width: '100%',
                        height: '100%',
                        keyExpr: 'key',
                        hoverStateEnabled: false,
                        scrollDirection: 'both',
                        showCheckBoxes: true,
                        rootValue: null,
                        selectAllEnabled: true,
                        selectAllText: dashboard.data.ALL_ELEMENT.text,
                        selectNodesRecursive: true,
                        onSelectionChanged: onSelectionChanged
                    }
            }
        })
    })(jQuery, DevExpress);
    DevExpress.MOD_DASHBOARD = true
}