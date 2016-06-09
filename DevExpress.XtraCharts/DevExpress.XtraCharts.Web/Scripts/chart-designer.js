var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Chart;
        (function (Chart) {
            var CollectionLookupEditorModel = (function (_super) {
                __extends(CollectionLookupEditorModel, _super);
                function CollectionLookupEditorModel(info, level) {
                    _super.call(this, info, level);
                    this.selectedItem = ko.observable();
                }
                return CollectionLookupEditorModel;
            })(Designer.Widgets.Editor);
            Chart.CollectionLookupEditorModel = CollectionLookupEditorModel;
        })(Chart = Designer.Chart || (Designer.Chart = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Chart;
        (function (Chart) {
            Chart.editorTemplates = {
                dataSource: { header: "dxrd-datasource" },
                series: { header: "dxrd-collection-lookup-header", content: "dxrd-series-item", editorType: Chart.CollectionLookupEditorModel },
                titles: { header: "dxrd-collection-lookup-header", content: "dxrd-titles-item", editorType: Chart.CollectionLookupEditorModel },
                views: { header: "dxrd-viewSelect" },
            };
            Chart.defaultBooleanValues = {
                "True": "True",
                "False": "False",
                "Default": "Default"
            }, Chart.scaleTypeValues = {
                "Qualitative": "Qualitative",
                "Numerical": "Numerical",
                "DateTime": "DateTime",
                "Auto": "Auto"
            }, Chart.stringAlignmentValues = {
                "Near": "Near",
                "Center": "Center",
                "Far": "Far"
            };
            Chart.angle = { propertyName: "angle", modelName: "@Angle", defaultVal: 0, from: Designer.floatFromModel, displayName: "Angle", editor: Designer.Widgets.editorTemplates.numeric };
            Chart.borderColor = { propertyName: "borderColor", modelName: "@BorderColor", from: Designer.colorFromString, toJsonObject: Designer.colorToString, displayName: "Border Color", editor: Designer.Widgets.editorTemplates.color };
            Chart.backColor = { propertyName: "backColor", modelName: "@BackColor", from: Designer.colorFromString, toJsonObject: Designer.colorToString, displayName: "Background Color", editor: Designer.Widgets.editorTemplates.color };
            Chart.dataMember = { propertyName: "dataMember", modelName: "@DataMember" };
            Chart.text = { propertyName: "text", modelName: "@Text", defaultVal: "", displayName: "Text", editor: Designer.Widgets.editorTemplates.text };
            Chart.visible = { propertyName: "visible", modelName: "@Visible", defaultVal: true, from: Designer.parseBool, editor: Designer.Widgets.editorTemplates.bool, displayName: "Visible" };
            Chart.name = { propertyName: "name", modelName: "@Name", displayName: "Name", editor: Designer.Widgets.editorTemplates.text };
            Chart.thickness = { propertyName: "thickness", modelName: "@Thickness", displayName: "Thickness", defaultVal: 1, editor: Designer.Widgets.editorTemplates.numeric }, Chart.visibility = { propertyName: "visibility", modelName: "@Visibility", displayName: "Visibility", defaultVal: "Default", editor: Designer.Widgets.editorTemplates.combobox, values: Chart.defaultBooleanValues }, Chart.color = { propertyName: "color", modelName: "@Color", displayName: "Color", from: Designer.colorFromString, toJsonObject: Designer.colorToString, editor: Designer.Widgets.editorTemplates.color }, Chart.titleAlignment = { propertyName: "titleAlignment", modelName: "@Alignment", displayName: "Alignment", defaultVal: "Center", editor: Designer.Widgets.editorTemplates.combobox, values: Chart.stringAlignmentValues }, Chart.textPattern = { propertyName: "textPattern", modelName: "@TextPattern", displayName: "Text Pattern", editor: Designer.Widgets.editorTemplates.text }, Chart.textAlignment = { propertyName: "textAlignment", modelName: "@TextAlignment", displayName: "Text Alignment", editor: Designer.Widgets.editorTemplates.combobox, values: Chart.stringAlignmentValues }, Chart.maxLineCount = { propertyName: "maxLineCount", modelName: "@MaxLineCount", displayName: "Max Line Count", editor: Designer.Widgets.editorTemplates.numeric }, Chart.maxWidth = { propertyName: "maxWidth", modelName: "@MaxWidth", displayName: "Max Width", editor: Designer.Widgets.editorTemplates.numeric }, Chart.textColor = { propertyName: "textColor", modelName: "@TextColor", displayName: "Text Color", from: Designer.colorFromString, toJsonObject: Designer.colorToString, editor: Designer.Widgets.editorTemplates.color }, Chart.antialiasing = { propertyName: "antialiasing", modelName: "@Antialiasing", displayName: "Antialiasing", editor: Designer.Widgets.editorTemplates.bool }, Chart.font = { propertyName: "font", modelName: "@Font", displayName: "Font", defaultVal: "Tahoma, 8pt", editor: Designer.Widgets.editorTemplates.font };
            Chart.enableAxisXZooming = { propertyName: "enableAxisXZooming", modelName: "@EnableAxisXZooming", displayName: "Enable Axis X Zooming" }, Chart.enableAxisXScrolling = { propertyName: "enableAxisXScrolling", modelName: "@EnableAxisXScrolling", displayName: "Enable Axis X Scrolling" }, Chart.enableAxisYZooming = { propertyName: "enableAxisYZooming", modelName: "@EnableAxisYZooming", displayName: "Enable Axis Y Zooming" }, Chart.enableAxisYScrolling = { propertyName: "enableAxisYScrolling", modelName: "@EnableAxisYScrolling", displayName: "Enable Axis Y Scrolling" }, Chart.rotated = { propertyName: "rotated", modelName: "@Rotated", displayName: "Rotated", defaultVal: false, editor: Designer.Widgets.editorTemplates.bool }, Chart.typeNameNotShow = { propertyName: "typeNameSerializable", modelName: "@TypeNameSerializable" };
            Chart.left = { propertyName: "left", modelName: "@Left", displayName: "Left", localizationId: "dx_reportDesigner_DevExpress.XtraPrinting.PaddingInfo.Left", editor: Designer.Widgets.editorTemplates.numeric }, Chart.right = { propertyName: "right", modelName: "@Top", displayName: "Top", localizationId: "dx_reportDesigner_DevExpress.XtraPrinting.PaddingInfo.Top", editor: Designer.Widgets.editorTemplates.numeric }, Chart.top = { propertyName: "top", modelName: "@Right", displayName: "Right", localizationId: "dx_reportDesigner_DevExpress.XtraPrinting.PaddingInfo.Right", editor: Designer.Widgets.editorTemplates.numeric }, Chart.bottom = { propertyName: "bottom", modelName: "@Bottom", displayName: "Bottom", localizationId: "dx_reportDesigner_DevExpress.XtraPrinting.PaddingInfo.Bottom", editor: Designer.Widgets.editorTemplates.numeric };
            Chart.margin = { propertyName: "chartMargins", modelName: "Margins", displayName: "Margins", info: [Chart.left, Chart.right, Chart.top, Chart.bottom], editor: Designer.Widgets.editorTemplates.objecteditor };
            Chart.font18 = { propertyName: "font18", modelName: "@Font", displayName: "Font", defaultVal: "Tahoma, 18pt", editor: Designer.Widgets.editorTemplates.font }, Chart.font12 = { propertyName: "font12", modelName: "@Font", displayName: "Font", defaultVal: "Tahoma, 12pt", editor: Designer.Widgets.editorTemplates.font }, Chart.font8 = { propertyName: "font8", modelName: "@Font", displayName: "Font", defaultVal: "Tahoma, 8pt", editor: Designer.Widgets.editorTemplates.font };
            Chart.paneSerializationsInfo = [Chart.enableAxisXScrolling, Chart.enableAxisYScrolling, Chart.enableAxisYZooming, Chart.enableAxisXZooming, Chart.backColor, Chart.borderColor], Chart.defaultPane = { propertyName: "defaultPane", modelName: "DefaultPane", displayName: "Default Pane", info: Chart.paneSerializationsInfo, defaultVal: {}, editor: Designer.Widgets.editorTemplates.objecteditor };
        })(Chart = Designer.Chart || (Designer.Chart = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Chart;
        (function (Chart) {
            var minValue = { propertyName: "minValue", modelName: "@MinValue", displayName: "Min Value", editor: Designer.Widgets.editorTemplates.numeric }, maxValue = { propertyName: "maxValue", modelName: "@MaxValue", displayName: "Max Value", editor: Designer.Widgets.editorTemplates.numeric }, auto = { propertyName: "auto", modelName: "@Auto", displayName: "Auto", defaultVal: false, editor: Designer.Widgets.editorTemplates.bool, from: Designer.parseBool }, autoSideMargins = { propertyName: "autoSideMargins", modelName: "@AutoSideMargins", displayName: "Auto Side Margins", defaultVal: false, editor: Designer.Widgets.editorTemplates.bool, from: Designer.parseBool }, sideMarginsValue = { propertyName: "sideMarginsValue", modelName: "@SideMarginsValue", displayName: "Side Margins Value", editor: Designer.Widgets.editorTemplates.numeric };
            Chart.visualRangeSerializationsInfo = [auto, autoSideMargins, minValue, maxValue, sideMarginsValue], Chart.visualRange = { propertyName: "visualRange", modelName: "VisualRange", displayName: "Visual Range", info: Chart.visualRangeSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor };
            var alwaysShowZeroLevel = { propertyName: "alwaysShowZeroLevel", modelName: "@AlwaysShowZeroLevel", displayName: "Always Show Zero Level", editor: Designer.Widgets.editorTemplates.bool, defaultVal: false, from: Designer.parseBool };
            Chart.wholeRangeSerializationsInfo = Chart.visualRangeSerializationsInfo.concat(alwaysShowZeroLevel), Chart.wholeRange = { propertyName: "wholeRange", modelName: "WholeRange", displayName: "Whole Range", info: Chart.wholeRangeSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor };
            Chart.radarWholeRange = { propertyName: "radarWholeRange", modelName: "WholeRange", displayName: "Whole Range", info: Chart.visualRangeSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor };
            var dashStyle = {
                propertyName: "dashStyle",
                modelName: "@DashStyle",
                displayName: "Dash Style",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Empty": "Empty",
                    "Solid": "Solid",
                    "Dash": "Dash",
                    "Dot": "Dot",
                    "DashDot": "DashDot",
                    "DashDotDot": "DashDotDot"
                }
            };
            Chart.lineStyleSerializationsInfo = [Chart.thickness, dashStyle], Chart.lineStyle = { propertyName: "lineStyle", modelName: "LineStyle", displayName: "Line Style", info: Chart.lineStyleSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor }, Chart.minorLineStyle = { propertyName: "minorLineStyle", modelName: "MinorLineStyle", displayName: "Minor Line Style", info: Chart.lineStyleSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor };
            var scaleMode = {
                propertyName: "scaleName",
                modelName: "@ScaleMode",
                displayName: "ScaleMode",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Automatic": "Automatic",
                    "Manual": "Manual",
                    "Continuous": "Continuous"
                }
            }, aggregateFunction = {
                propertyName: "aggregateFunction",
                modelName: "@AggregateFunction",
                displayName: "AggregateFunction",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "None": "None",
                    "Average": "Average",
                    "Sum": "Sum",
                    "Minimum": "Minimum",
                    "Maximum": "Maximum",
                    "Count": "Count",
                    "Financial": "Financial"
                }
            }, gridSpacing = { propertyName: "gridSpacing", modelName: "@GridSpacing", displayName: "Grid Spacing", editor: Designer.Widgets.editorTemplates.numeric }, autoGrid = { propertyName: "autoGrid", modelName: "@AutoGrid", displayName: "Auto Grid", editor: Designer.Widgets.editorTemplates.bool, defaultVal: false, from: Designer.parseBool }, gridOffset = { propertyName: "gridOffset", modelName: "@GridOffset", displayName: "Grid Offset", editor: Designer.Widgets.editorTemplates.numeric };
            Chart.scaleOptionsBaseSerializationsInfo = [autoGrid, aggregateFunction, gridOffset, gridSpacing, scaleMode];
            var numericMeasureUnit = {
                propertyName: "measureUnit",
                modelName: "@MeasureUnit",
                displayName: "Measure Unit",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Ones": "Ones",
                    "Tens": "Tens",
                    "Hundreds": "Hundreds",
                    "Thousands": "Thousands",
                    "Millions": "Millions",
                    "Billions": "Billions",
                    "Custom": "Custom"
                }
            }, numericGridAlignment = {
                propertyName: "gridAlignment",
                modelName: "@GridAlignment",
                displayName: "Grid Alignment",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Ones": "Ones",
                    "Tens": "Tens",
                    "Hundreds": "Hundreds",
                    "Thousands": "Thousands",
                    "Millions": "Millions",
                    "Billions": "Billions",
                    "Custom": "Custom"
                }
            }, customGridAlignment = { propertyName: "customGridAlignment", modelName: "@CustomGridAlignment", defaultVal: null, displayName: "Custom Grid Alignment", editor: Designer.Widgets.editorTemplates.numeric }, customMeasureUnit = { propertyName: "customMeasureUnit", modelName: "@CustomMeasureUnit", defaultVal: null, displayName: "Custom Measure Unit", editor: Designer.Widgets.editorTemplates.numeric };
            Chart.numericScaleOptionsSerializationsInfo = [numericMeasureUnit, numericGridAlignment, customGridAlignment, customMeasureUnit].concat(Chart.scaleOptionsBaseSerializationsInfo), Chart.numericScaleOptions = { propertyName: "numericScaleOptions", modelName: "NumericScaleOptions", displayName: "Numeric Scale Options", info: Chart.numericScaleOptionsSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor };
            Chart.radarAxisYNumericScaleOptionsSerializationsInfo = [autoGrid, numericGridAlignment, gridOffset, gridSpacing], Chart.radarAxisYNumericScaleOptions = { propertyName: "radarAxisYNumericScaleOptions", modelName: "NumericScaleOptions", displayName: "Numeric Scale Options", info: Chart.radarAxisYNumericScaleOptionsSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor };
            Chart.radarAxisXNumericScaleOptionsSerializationsInfo = Chart.radarAxisYNumericScaleOptionsSerializationsInfo.concat(scaleMode), Chart.radarAxisXNumericScaleOptions = { propertyName: "radarAxisXNumericScaleOptions", modelName: "NumericScaleOptions", displayName: "Numeric Scale Options", info: Chart.radarAxisXNumericScaleOptionsSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor };
            var dateMeasureUnit = {
                propertyName: "measureUnit",
                modelName: "@MeasureUnit",
                displayName: "Measure Unit",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Millisecond": "Millisecond",
                    "Second ": "Second",
                    "Minute": "Minute",
                    "Hour": "Hour",
                    "Day": "Day",
                    "Week": "Week",
                    "Month": "Month",
                    "Quarter": "Quarter",
                    "Year": "Year"
                }
            }, dateGridAlignment = {
                propertyName: "gridAlignment",
                modelName: "@GridAlignment",
                displayName: "Grid Alignment",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Millisecond": "Millisecond",
                    "Second ": "Second",
                    "Minute": "Minute",
                    "Hour": "Hour",
                    "Day": "Day",
                    "Week": "Week",
                    "Month": "Month",
                    "Quarter": "Quarter",
                    "Year": "Year"
                }
            }, workdaysOnly = { propertyName: "workdaysOnly", modelName: "@WorkdaysOnly", displayName: "Workdays Only", editor: Designer.Widgets.editorTemplates.bool, defaultVal: false, from: Designer.parseBool };
            Chart.dateTimeScaleOptionsSerializationsInfo = [dateGridAlignment, dateMeasureUnit, workdaysOnly].concat(Chart.scaleOptionsBaseSerializationsInfo), Chart.dateTimeScaleOptions = { propertyName: "dateTimeScaleOptions", modelName: "DateTimeScaleOptions", displayName: "DateTimeScaleOptions", info: Chart.dateTimeScaleOptionsSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor };
            var visibleInPanesSerializable = { propertyName: "visibleInPanesSerializable", modelName: "@VisibleInPanesSerializable", displayName: "Visible In Panes Serializable", editor: Designer.Widgets.editorTemplates.text }, minorVisible = { propertyName: "minorVisible", modelName: "@MinorVisible", displayName: "Minor Visible", defaultVal: false, editor: Designer.Widgets.editorTemplates.bool }, minorColor = { propertyName: "minorColor", modelName: "@MinorColor", displayName: "Minor Color", from: Designer.colorFromString, toJsonObject: Designer.colorToString, editor: Designer.Widgets.editorTemplates.color };
            var visibleDefaultValueFalse = { propertyName: "chartVisible", modelName: "@Visible", defaultVal: false, from: Designer.parseBool, editor: Designer.Widgets.editorTemplates.bool, displayName: "Visible" }, gridLinesAxisBaseSerializationsInfo = [minorVisible, Chart.color, minorColor, Chart.lineStyle, Chart.minorLineStyle];
            Chart.gridLinesAxisXSerializationsInfo = [visibleDefaultValueFalse].concat(gridLinesAxisBaseSerializationsInfo), Chart.gridLinesAxisX = { propertyName: "gridLinesAxisX", modelName: "GridLines", displayName: "Grid Lines", info: Chart.gridLinesAxisXSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor }, Chart.gridLinesAxisYSerializationsInfo = [Chart.visible].concat(gridLinesAxisBaseSerializationsInfo), Chart.gridLinesAxisY = { propertyName: "gridLinesAxisY", modelName: "GridLines", displayName: "Grid Lines", info: Chart.gridLinesAxisYSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor };
            Chart.radarGridLinesAxisXSerializationsInfo = [Chart.visible].concat(gridLinesAxisBaseSerializationsInfo), Chart.radarGridLinesAxisX = { propertyName: "radarGridLinesAxisX", modelName: "GridLines", displayName: "Grid Lines", info: Chart.radarGridLinesAxisXSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor };
            var straggered = { propertyName: "straggered", modelName: "@Straggered", displayName: "Straggered", editor: Designer.Widgets.editorTemplates.bool, defaultVal: false, from: Designer.parseBool }, allowStagger = { propertyName: "allowStagger", modelName: "@AllowStagger", displayName: "Allow Stagger", editor: Designer.Widgets.editorTemplates.bool, defaultVal: false, from: Designer.parseBool }, allowRotate = { propertyName: "allowRotate", modelName: "@AllowRotate", displayName: "Allow Rotate", editor: Designer.Widgets.editorTemplates.bool, defaultVal: false, from: Designer.parseBool }, allowHide = { propertyName: "allowHide", modelName: "@AllowHide", displayName: "Allow Hide", editor: Designer.Widgets.editorTemplates.bool, defaultVal: false, from: Designer.parseBool }, minIndent = { propertyName: "minIndent", modelName: "@MinIndent", displayName: "Min Indent", editor: Designer.Widgets.editorTemplates.numeric }, axisLabelResolveOverlappingOptionsSerializationsInfo = [allowStagger, allowRotate, allowHide, minIndent], axisLabelResolveOverlappingOptions = { propertyName: "resolveOverlappingOptions", modelName: "ResolveOverlappingOptions", displayName: "Resolve Overlapping Options", editor: Designer.Widgets.editorTemplates.objecteditor, info: axisLabelResolveOverlappingOptionsSerializationsInfo }, enableAntialiasing = { propertyName: "enableAntialiasing", modelName: "@EnableAntialiasing", displayName: "Enable Antialiasing", editor: Designer.Widgets.editorTemplates.combobox, values: Chart.defaultBooleanValues };
            var axisLabelBaseSerializationsInfo = [enableAntialiasing, Chart.font, Chart.maxLineCount, Chart.maxWidth, axisLabelResolveOverlappingOptions, Chart.textAlignment, Chart.textColor, Chart.textPattern, Chart.visible];
            Chart.axisLabelSerializationsInfo = [Chart.angle, straggered].concat(axisLabelBaseSerializationsInfo), Chart.axisLabel = { propertyName: "axisLabel", modelName: "Label", displayName: "Label", editor: Designer.Widgets.editorTemplates.objecteditor, info: Chart.axisLabelSerializationsInfo };
            Chart.minorCount = { propertyName: "minorCount", modelName: "@MinorCount", displayName: "Minor Count", editor: Designer.Widgets.editorTemplates.numeric }, Chart.interlaced = { propertyName: "interlaced", modelName: "@Interlaced", displayName: "Interlaced", from: Designer.parseBool, defaultVal: false, editor: Designer.Widgets.editorTemplates.bool }, Chart.interlacedColor = { propertyName: "interlacedColor", modelName: "@InterlacedColor", displayName: "Interlaced Color", editor: Designer.Widgets.editorTemplates.color, from: Designer.colorFromString };
            Chart.axisBaseSerializationsInfo = [Chart.visualRange, Chart.wholeRange, Chart.numericScaleOptions, Chart.dateTimeScaleOptions, Chart.minorCount, Chart.interlacedColor];
            var fillMode = {
                propertyName: "fillMode",
                modelName: "@FillMode",
                displayName: "Fill Mode",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Empty": "Empty",
                    "Solid": "Solid",
                    "Gradient": "Gradient",
                    "Hatch": "Hatch"
                }
            }, fillStyle2D = [fillMode];
            Chart.rectangleFillStyleSerializationsInfo = [].concat(fillStyle2D), Chart.interlacedFillStyle = { propertyName: "InterlacedFillStyle", modelName: "InterlacedFillStyle", displayName: "Interlaced Fill Style", info: Chart.rectangleFillStyleSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor };
            var crossAxis = { propertyName: "crossAxis", modelName: "@CrossAxis", displayName: "Cross Axis", editor: Designer.Widgets.editorTemplates.bool, defaultVal: false, from: Designer.parseBool }, minorThickness = { propertyName: "minorThickness", modelName: "@MinorThickness", displayName: "Minor Thickness", editor: Designer.Widgets.editorTemplates.numeric, defaultVal: 1 }, minorLenght = { propertyName: "minorLength", modelName: "@MinorLength", displayName: "Minor Length", editor: Designer.Widgets.editorTemplates.numeric, defaultVal: 2 }, lenghtinfo = { propertyName: "lenght", modelName: "@Length", displayName: "Length", editor: Designer.Widgets.editorTemplates.numeric, defaultVal: 5 }, tickmarksMinorVisible = { propertyName: "minorVisible", modelName: "@MinorVisible", displayName: "Minor Visible", defaultVal: true, editor: Designer.Widgets.editorTemplates.bool }, tickmarksBaseSerializationsInfo = [Chart.visible, tickmarksMinorVisible, crossAxis, Chart.thickness, minorThickness, lenghtinfo, minorLenght], tickmarksSerializationsInfo = [].concat(tickmarksBaseSerializationsInfo);
            Chart.tickmarks = { propertyName: "tickmarks", modelName: "Tickmarks", displayName: "Tickmarks", editor: Designer.Widgets.editorTemplates.objecteditor, info: tickmarksSerializationsInfo };
            Chart.axisAlignment = {
                propertyName: "axisAlignment",
                modelName: "@AxisAlignment",
                displayName: "Alignment",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Near": "Near",
                    "Zero": "Zero",
                    "Far": "Far"
                }
            };
            var pattern = { propertyName: "pattern", modelName: "@Pattern", displayName: "Pattern", editor: Designer.Widgets.editorTemplates.text }, crosshairAxisLabelOptionsSerializationsInfo = [Chart.visibility, pattern, Chart.backColor, Chart.textColor, Chart.font];
            Chart.crosshairAxisLabelOptions = { propertyName: "crosshairAxisLabelOptions", modelName: "CrosshairAxisLabelOptions", displayName: "Crosshair Axis Label Options", editor: Designer.Widgets.editorTemplates.objecteditor, info: crosshairAxisLabelOptionsSerializationsInfo };
            var axisTitleVisibility = { propertyName: "axisTitleVisibility", modelName: "@Visibility", displayName: "Visibility", defaultVal: "False", editor: Designer.Widgets.editorTemplates.combobox, values: Chart.defaultBooleanValues };
            Chart.axisTitleSerializationsInfo = [Chart.text, Chart.titleAlignment, Chart.antialiasing, axisTitleVisibility, Chart.textColor, Chart.font12], Chart.axisTitle = { propertyName: "axisTitle", modelName: "Title", displayName: "Title", defaultVal: {}, info: Chart.axisTitleSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor };
            var crosshairLabelVisibility = { propertyName: "crosshairLabelVisibility", modelName: "@CrosshairLabelVisibility", displayName: "Crosshair Label Visibility", editor: Designer.Widgets.editorTemplates.combobox, values: Chart.defaultBooleanValues };
            Chart.axis2DSerializationsInfo = [Chart.interlaced, Chart.interlacedFillStyle, Chart.tickmarks, Chart.axisTitle, Chart.visibility, Chart.axisAlignment, Chart.axisLabel, Chart.thickness, Chart.color, crosshairLabelVisibility, visibleInPanesSerializable, Chart.crosshairAxisLabelOptions, visibleInPanesSerializable].concat(Chart.axisBaseSerializationsInfo);
            Chart.axisY3DInterlaced = { propertyName: "axisY3DInterlaced", modelName: "@Interlaced", displayName: "Interlaced", from: Designer.parseBool, defaultVal: true, editor: Designer.Widgets.editorTemplates.bool };
            var fillMode3D = {
                propertyName: "fillMode",
                modelName: "@FillMode",
                displayName: "Fill Mode",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Empty": "Empty",
                    "Solid": "Solid",
                    "Gradient": "Gradient"
                }
            }, fillStyle3D = [fillMode];
            var rectangleFillStyle3DSerializationsInfo = [].concat(fillStyle3D), rectangleFillStyle3D = { propertyName: "interlacedFillStyle", modelName: "InterlacedFillStyle", displayName: "Interlaced Fill Style", info: rectangleFillStyle3DSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor }, axisLabel3DPosition = {
                propertyName: "axisLabel3DPosition",
                modelName: "@AxisLabel3DPosition",
                displayName: "Axis Label 3D Position",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Bottom": "Bottom",
                    "Left": "Left",
                    "Right": "Right",
                    "Top": "Top",
                    "Auto": "Auto"
                }
            };
            Chart.axisLabel3DSerializationsInfo = [axisLabel3DPosition].concat(Chart.axisLabelSerializationsInfo), Chart.axisLabel3D = { propertyName: "axisLabel3D", modelName: "Label", displayName: "Label", info: Chart.axisLabel3DSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor };
            Chart.axis3DSerializationsInfo = [rectangleFillStyle3D, Chart.axisLabel3D].concat(Chart.axisBaseSerializationsInfo);
            var sizeInPixels = { propertyName: "sizeInPixels", modelName: "@SizeInPixels", displayName: "Size In Pixels", editor: Designer.Widgets.editorTemplates.numeric }, scaleBreakStyle = {
                propertyName: "style",
                modelName: "@Style",
                displayName: "Style",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Straight": "Straight",
                    "Ragged": "Ragged",
                    "Waved": "Waved"
                }
            }, scaleBreaksOptionsSerializationsInfo = [sizeInPixels, Chart.color, scaleBreakStyle];
            Chart.scaleBreaksOptions = { propertyName: "scaleBreakOptions", modelName: "ScaleBreakOptions", displayName: "Scale Break Options", info: scaleBreaksOptionsSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor };
            var enabled = { propertyName: "enabled", modelName: "@Enabled", displayName: "Enabled", defaultVal: false, editor: Designer.Widgets.editorTemplates.bool, from: Designer.parseBool }, maxCount = { propertyName: "maxCount", modelName: "@MaxCount", displayName: "Max Count", editor: Designer.Widgets.editorTemplates.numeric }, autoScaleBreaksSerializationsInfo = [enabled, maxCount];
            Chart.autoScaleBreaks = { propertyName: "autoScaleBreaks", modelName: "AutoScaleBreaks", displayName: "Auto Scale Breaks", editor: Designer.Widgets.editorTemplates.objecteditor, info: autoScaleBreaksSerializationsInfo }, Chart.reverse = { propertyName: "axisReverse", modelName: "@Reverse", displayName: "Reverse", editor: Designer.Widgets.editorTemplates.bool };
            Chart.axisSerializationsInfo = [Chart.reverse, Chart.scaleBreaksOptions, Chart.autoScaleBreaks].concat(Chart.axis2DSerializationsInfo);
            Chart.axisXYSerializationsInfo = [Chart.visible].concat(Chart.axisSerializationsInfo);
            Chart.topLevel = { propertyName: "topLevel", modelName: "@TopLevel", displayName: "Top Level", editor: Designer.Widgets.editorTemplates.numeric }, Chart.radarAxisXLabelTextDirection = {
                propertyName: "textDirection",
                modelName: "TextDirection",
                displayName: "Text Direction",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "LeftToRight": "LeftToRight",
                    "TopToBottom": "TopToBottom",
                    "BottomToTop": "BottomToTop",
                    "Radial": "Radial",
                    "Tangent": "Tangent"
                }
            };
            Chart.radarAxisXLabelSerializationsInfo = axisLabelBaseSerializationsInfo.concat(Chart.radarAxisXLabelTextDirection), Chart.radarAxisXLabel = { propertyName: "radarAxisXLabel", modelName: "Label", displayName: "Label", info: Chart.radarAxisXLabelSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor }, Chart.radarAxisYLabel = { propertyName: "radarAxisYLabel", modelName: "Label", displayName: "Label", info: axisLabelBaseSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor };
            var radarAxisSerializationsInfo = [Chart.interlaced, Chart.interlacedColor, Chart.interlacedFillStyle, Chart.visualRange, Chart.minorCount], radarAxisXSerializationsInfo = [Chart.radarAxisXNumericScaleOptions, Chart.radarWholeRange, Chart.radarGridLinesAxisX, Chart.radarAxisXLabel].concat(radarAxisSerializationsInfo), radarAxisYSerializationsInfo = [Chart.color, Chart.thickness, Chart.visible, Chart.radarAxisYNumericScaleOptions, Chart.topLevel, Chart.wholeRange, Chart.gridLinesAxisY, Chart.radarAxisYLabel, Chart.tickmarks].concat(radarAxisSerializationsInfo);
            Chart.radarAxisX = { propertyName: "axisX", modelName: "AxisX", displayName: "Axis X", info: radarAxisXSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor }, Chart.radarAxisY = { propertyName: "axisY", modelName: "AxisY", displayName: "Axis Y", info: radarAxisYSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor };
            Chart.axisX3D = { propertyName: "axisX", modelName: "AxisX", displayName: "Axis X", info: [Chart.gridLinesAxisX, Chart.interlaced].concat(Chart.axis3DSerializationsInfo), editor: Designer.Widgets.editorTemplates.objecteditor }, Chart.axisY3D = { propertyName: "axisY", modelName: "AxisY", displayName: "Axis Y", info: [Chart.gridLinesAxisY, Chart.axisY3DInterlaced].concat(Chart.axis3DSerializationsInfo), editor: Designer.Widgets.editorTemplates.objecteditor };
            Chart.axisX = { propertyName: "axisX", modelName: "AxisX", displayName: "Axis X", defaultVal: {}, info: [Chart.gridLinesAxisX].concat(Chart.axisXYSerializationsInfo), editor: Designer.Widgets.editorTemplates.objecteditor }, Chart.axisY = { propertyName: "axisY", modelName: "AxisY", displayName: "Axis Y", defaultVal: {}, info: [Chart.gridLinesAxisY].concat(Chart.axisXYSerializationsInfo), editor: Designer.Widgets.editorTemplates.objecteditor };
        })(Chart = Designer.Chart || (Designer.Chart = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Chart;
        (function (Chart) {
            Chart.dimension = { propertyName: "dimension", modelName: "@Dimension", displayName: "Dimension", editor: Designer.Widgets.editorTemplates.numeric }, Chart.equalPieSize = { propertyName: "equalPieSize", modelName: "@EqualPieSize", displayName: "EqualPieSize", editor: Designer.Widgets.editorTemplates.bool }, Chart.typeNameNotShowDiagram = { propertyName: "typeNameSerializable", modelName: "@TypeNameSerializable" };
            Chart.diagramSerializationsInfo = [Chart.typeNameNotShowDiagram, Chart.defaultPane], Chart.radarSerializationsInfo = [Chart.radarAxisX, Chart.radarAxisY, Chart.margin, Chart.backColor].concat(Chart.diagramSerializationsInfo), Chart.polarSerializationsInfo = [Chart.radarAxisX, Chart.radarAxisY, Chart.margin, Chart.backColor].concat(Chart.diagramSerializationsInfo), Chart.simple3DSerializationsInfo = [Chart.dimension, Chart.margin, Chart.equalPieSize].concat(Chart.diagramSerializationsInfo), Chart.funnel3DSerializationsInfo = [].concat(Chart.simple3DSerializationsInfo), Chart.simpleSerializationsInfo = [Chart.dimension, Chart.margin, Chart.equalPieSize].concat(Chart.diagramSerializationsInfo), Chart.XY2DSerializationsInfo = [Chart.margin, Chart.defaultPane, Chart.enableAxisXScrolling, Chart.enableAxisXZooming, Chart.enableAxisYScrolling, Chart.enableAxisYZooming, Chart.typeNameNotShowDiagram], Chart.XYSerializationsInfo = [Chart.axisX, Chart.axisY, Chart.rotated].concat(Chart.XY2DSerializationsInfo), Chart.XY3DSerializationsInfo = [Chart.axisX3D, Chart.axisY3D, Chart.backColor, Chart.typeNameNotShowDiagram], Chart.GanttDiagramSerializationsInfo = [].concat(Chart.XYSerializationsInfo);
            var XYObject = { info: Chart.XYSerializationsInfo, type: "XYDiagram" }, XY2DObject = { info: Chart.XY2DSerializationsInfo, type: "SwiftPlotDiagram" }, XY3DObject = { info: Chart.XY3DSerializationsInfo, type: "XYDiagram3D" }, radarObject = { info: Chart.radarSerializationsInfo, type: "RadarDiagram" }, polarObject = { info: Chart.polarSerializationsInfo, type: "PolarDiagram" }, simpleObject = { info: Chart.simpleSerializationsInfo, type: "SimpleDiagram" }, simple3DObject = { info: Chart.simple3DSerializationsInfo, type: "SimpleDiagram3D" }, funnel3DObject = { info: Chart.funnel3DSerializationsInfo, type: "FunnelDiagram" }, gantObject = { info: Chart.GanttDiagramSerializationsInfo, type: "GanttDiagram" };
            Chart.diagramMapper = {
                "SideBySideBarSeriesView": XYObject,
                "StackedBarSeriesView": XYObject,
                "FullStackedBarSeriesView": XYObject,
                "SideBySideStackedBarSeriesView": XYObject,
                "SideBySideFullStackedBarSeriesView": XYObject,
                "SideBySideBar3DSeriesView": XY3DObject,
                "StackedBar3DSeriesView": XY3DObject,
                "FullStackedBar3DSeriesView": XY3DObject,
                "SideBySideStackedBar3DSeriesView": XY3DObject,
                "SideBySideFullStackedBar3DSeriesView": XY3DObject,
                "ManhattanBarSeriesView": XY3DObject,
                "PointSeriesView": XYObject,
                "BubbleSeriesView": XYObject,
                "LineSeriesView": XYObject,
                "StackedLineSeriesView": XYObject,
                "FullStackedLineSeriesView": XYObject,
                "StepLineSeriesView": XYObject,
                "SplineSeriesView": XYObject,
                "ScatterLineSeriesView": XYObject,
                "SwiftPlotSeriesView": XY2DObject,
                "Line3DSeriesView": XY3DObject,
                "StackedLine3DSeriesView": XY3DObject,
                "FullStackedLine3DSeriesView": XY3DObject,
                "StepLine3DSeriesView": XY3DObject,
                "Spline3DSeriesView": XY3DObject,
                "PieSeriesView": simpleObject,
                "DoughnutSeriesView": simpleObject,
                "NestedDoughnutSeriesView": simpleObject,
                "Pie3DSeriesView": simple3DObject,
                "Doughnut3DSeriesView": simple3DObject,
                "FunnelSeriesView": simpleObject,
                "Funnel3DSeriesView": funnel3DObject,
                "AreaSeriesView": XYObject,
                "StackedAreaSeriesView": XYObject,
                "FullStackedAreaSeriesView": XYObject,
                "StepAreaSeriesView": XYObject,
                "SplineAreaSeriesView": XYObject,
                "StackedSplineAreaSeriesView": XYObject,
                "FullStackedSplineAreaSeriesView": XYObject,
                "Area3DSeriesView": XY3DObject,
                "StackedArea3DSeriesView": XY3DObject,
                "FullStackedArea3DSeriesView": XY3DObject,
                "StepArea3DSeriesView": XY3DObject,
                "SplineArea3DSeriesView": XY3DObject,
                "StackedSplineArea3DSeriesView": XY3DObject,
                "FullStackedSplineArea3DSeriesView": XY3DObject,
                "OverlappedRangeBarSeriesView": XYObject,
                "SideBySideRangeBarSeriesView": XYObject,
                "RangeAreaSeriesView": XYObject,
                "RangeArea3DSeriesView": XY3DObject,
                "RadarPointSeriesView": radarObject,
                "RadarLineSeriesView": radarObject,
                "RadarAreaSeriesView": radarObject,
                "PolarPointSeriesView": polarObject,
                "PolarLineSeriesView": polarObject,
                "PolarAreaSeriesView": polarObject,
                "StockSeriesView": XYObject,
                "CandleStickSeriesView": XYObject,
                "OverlappedGanttSeriesView": gantObject,
                "SideBySideGanttSeriesView": gantObject
            };
        })(Chart = Designer.Chart || (Designer.Chart = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Chart;
        (function (Chart) {
            var CommonValueDataMembers = (function () {
                function CommonValueDataMembers(value) {
                    this.value = ko.observable();
                    this.value(value);
                }
                CommonValueDataMembers.from = function (value) {
                    return new CommonValueDataMembers(value);
                };
                CommonValueDataMembers.toJson = function (value) {
                    return value.toString() || {};
                };
                CommonValueDataMembers.prototype.toString = function () {
                    return this.value();
                };
                CommonValueDataMembers.prototype.getInfo = function () {
                    return valueDataMembersSerializationsInfo;
                };
                return CommonValueDataMembers;
            })();
            Chart.CommonValueDataMembers = CommonValueDataMembers;
            var ValueWeightDataMembers = (function () {
                function ValueWeightDataMembers(value) {
                    this.value = ko.observable();
                    this.weight = ko.observable();
                    if (value) {
                        var result = value.split(';');
                        this.value(result[0]);
                        this.weight(result[1]);
                    }
                }
                ValueWeightDataMembers.prototype.toString = function () {
                    if (this.value() || this.weight()) {
                        return this.value() + ";" + this.weight();
                    }
                    return null;
                };
                ValueWeightDataMembers.prototype.getInfo = function () {
                    return valueWeightDataMembersSerializationsInfo;
                };
                return ValueWeightDataMembers;
            })();
            Chart.ValueWeightDataMembers = ValueWeightDataMembers;
            var Value1Value2DataMembers = (function () {
                function Value1Value2DataMembers(value) {
                    this.value1 = ko.observable();
                    this.value2 = ko.observable();
                    if (value) {
                        var result = value.split(';');
                        this.value1(result[0]);
                        this.value2(result[1]);
                    }
                }
                Value1Value2DataMembers.prototype.toString = function () {
                    if (this.value1() || this.value2()) {
                        return this.value1() + ";" + this.value2();
                    }
                    return null;
                };
                Value1Value2DataMembers.prototype.getInfo = function () {
                    return value1Value2DataMembersSerializationsInfo;
                };
                return Value1Value2DataMembers;
            })();
            Chart.Value1Value2DataMembers = Value1Value2DataMembers;
            var StockValueDataMembers = (function () {
                function StockValueDataMembers(value) {
                    this.close = ko.observable();
                    this.hight = ko.observable();
                    this.low = ko.observable();
                    this.open = ko.observable();
                    if (value) {
                        var result = value.split(';');
                        this.close(result[0]);
                        this.hight(result[1]);
                        this.low(result[2]);
                        this.open(result[3]);
                    }
                }
                StockValueDataMembers.prototype.toString = function () {
                    if (this.close() || this.hight() || this.low() || this.open()) {
                        return this.close() + ";" + this.hight() + ";" + this.low() + ";" + this.open();
                    }
                    return null;
                };
                StockValueDataMembers.prototype.getInfo = function () {
                    return stockDataMembersSerializationsInfo;
                };
                return StockValueDataMembers;
            })();
            Chart.StockValueDataMembers = StockValueDataMembers;
            var viewTypesDataMembers = {
                "BubbleSeriesView": ValueWeightDataMembers,
                "OverlappedRangeBarSeriesView": Value1Value2DataMembers,
                "SideBySideRangeBarSeriesView": Value1Value2DataMembers,
                "RangeAreaSeriesView": Value1Value2DataMembers,
                "RangeArea3DSeriesView": Value1Value2DataMembers,
                "OverlappedGanttSeriesView": Value1Value2DataMembers,
                "SideBySideGanttSeriesView": Value1Value2DataMembers,
                "StockSeriesView": StockValueDataMembers,
                "CandleStickSeriesView": StockValueDataMembers
            };
            var ChartViewModel = (function (_super) {
                __extends(ChartViewModel, _super);
                function ChartViewModel(model, serializer) {
                    var _this = this;
                    _super.call(this, Designer.cutRefs(model), serializer, Chart.chartSerializationsInfo);
                    this.displayName = ko.observable("Chart");
                    var oldType = ko.observable("");
                    this.diagram = ko.computed(function () {
                        var diagramModel = model && model["Diagram"] || {};
                        model["Diagram"] = null;
                        var typeName = "";
                        if (_this.dataContainer.seriesDataMember() || _this.dataContainer.series().length === 0) {
                            typeName = _this.dataContainer.seriesTemplate.view.typeName();
                        }
                        else {
                            typeName = _this.dataContainer.series()[0].view.typeName();
                        }
                        if (oldType.peek() !== Chart.diagramMapper[typeName].type) {
                            oldType(Chart.diagramMapper[typeName].type);
                            return DiagramViewModel.createDiagram(diagramModel, typeName, serializer);
                        }
                        return _this.diagram.peek();
                    });
                    this.titles = Designer.deserializeArray(model && model.Titles || [], function (title) {
                        return new TitleViewModel(title, _this.titles, serializer);
                    });
                    this.titles().forEach(function (title) {
                        title.parent = _this.titles;
                    });
                    this.titles()["displayName"] = ko.observable("Titles");
                    var actions = [
                        {
                            text: "Add Title",
                            imageClassName: "dxrd-image-chart-title-top_left",
                            disabled: ko.observable(false),
                            visible: true,
                            clickAction: function () {
                                _this.addTitle({ "@Alignment": "Near" });
                            }
                        },
                        {
                            text: "Add Title",
                            imageClassName: "dxrd-image-chart-title-top_center",
                            disabled: ko.observable(false),
                            visible: true,
                            clickAction: function () {
                                _this.addTitle({ "@Alignment": "Center" });
                            }
                        },
                        {
                            text: "Add Title",
                            imageClassName: "dxrd-image-chart-title-top_right",
                            disabled: ko.observable(false),
                            visible: true,
                            clickAction: function () {
                                _this.addTitle({ "@Alignment": "Far" });
                            }
                        },
                        {
                            text: "Add Title",
                            imageClassName: "dxrd-image-chart-title-right_top_vertical",
                            disabled: ko.observable(false),
                            visible: true,
                            clickAction: function () {
                                _this.addTitle({ "@Dock": "Right", "@Alignment": "Near" });
                            }
                        },
                        {
                            text: "Add Title",
                            imageClassName: "dxrd-image-chart-title-right_center_vertical",
                            disabled: ko.observable(false),
                            visible: true,
                            clickAction: function () {
                                _this.addTitle({ "@Dock": "Right", "@Alignment": "Center" });
                            }
                        },
                        {
                            text: "Add Title",
                            imageClassName: "dxrd-image-chart-title-right_bottom_vertical",
                            disabled: ko.observable(false),
                            visible: true,
                            clickAction: function () {
                                _this.addTitle({ "@Dock": "Right", "@Alignment": "Far" });
                            }
                        },
                        {
                            text: "Add Title",
                            imageClassName: "dxrd-image-chart-title-bottom_left",
                            disabled: ko.observable(false),
                            visible: true,
                            clickAction: function () {
                                _this.addTitle({ "@Dock": "Bottom", "@Alignment": "Near" });
                            }
                        },
                        {
                            text: "Add Title",
                            imageClassName: "dxrd-image-chart-title-bottom_center",
                            disabled: ko.observable(false),
                            visible: true,
                            clickAction: function () {
                                _this.addTitle({ "@Dock": "Bottom", "@Alignment": "Center" });
                            }
                        },
                        {
                            text: "Add Title",
                            imageClassName: "dxrd-image-chart-title-bottom_right",
                            disabled: ko.observable(false),
                            visible: true,
                            clickAction: function () {
                                _this.addTitle({ "@Dock": "Bottom", "@Alignment": "Far" });
                            }
                        },
                        {
                            text: "Add Title",
                            imageClassName: "dxrd-image-chart-title-left_bottom_vertical",
                            disabled: ko.observable(false),
                            visible: true,
                            clickAction: function () {
                                _this.addTitle({ "@Dock": "Left", "@Alignment": "Near" });
                            }
                        },
                        {
                            text: "Add Title",
                            imageClassName: "dxrd-image-chart-title-left_center_vertical",
                            disabled: ko.observable(false),
                            visible: true,
                            clickAction: function () {
                                _this.addTitle({ "@Dock": "Left", "@Alignment": "Center" });
                            }
                        },
                        {
                            text: "Add Title",
                            imageClassName: "dxrd-image-chart-title-left_top_vertical",
                            disabled: ko.observable(false),
                            visible: true,
                            clickAction: function () {
                                _this.addTitle({ "@Dock": "Left", "@Alignment": "Far" });
                            }
                        }
                    ];
                    this.titles()["innerActions"] = createInnerActionsWithPopover("Add Title", "addtitles-action", actions);
                }
                ChartViewModel.from = function (model, serializer) {
                    return new ChartViewModel(model, serializer);
                };
                ChartViewModel.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, Chart.chartSerializationsInfo, refs);
                };
                ChartViewModel.prototype.getInfo = function () {
                    return Chart.chartSerializationsInfo;
                };
                ChartViewModel.prototype.addTitle = function (model) {
                    this.titles()["innerActions"][0].closePopover();
                    this.titles.push(new TitleViewModel(model, this.titles));
                };
                return ChartViewModel;
            })(Designer.SerializableModel);
            Chart.ChartViewModel = ChartViewModel;
            var TitleViewModel = (function (_super) {
                __extends(TitleViewModel, _super);
                function TitleViewModel(model, parent, serializer) {
                    var _this = this;
                    _super.call(this, model, serializer, Chart.titleSerializationsInfo);
                    this.parent = parent;
                    this.name = ko.computed({
                        read: function () {
                            return _this["text"] ? _this["text"]() : "title";
                        },
                        write: function (val) {
                            _this["text"](val);
                        }
                    });
                    this["displayName"] = this.name;
                    this["className"] = ko.computed(function () {
                        return "titleviewmodel";
                    });
                    this["innerActions"] = [
                        {
                            text: "Remove Title",
                            imageClassName: "dxrd-image-recycle-bin",
                            disabled: ko.observable(false),
                            visible: true,
                            clickAction: function () {
                                _this.parent.remove(_this);
                            },
                        },
                    ];
                }
                TitleViewModel.from = function (model, serializer) {
                    return new TitleViewModel(model, null, serializer);
                };
                TitleViewModel.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, Chart.titleSerializationsInfo, refs);
                };
                TitleViewModel.prototype.getInfo = function () {
                    return Chart.titleSerializationsInfo;
                };
                return TitleViewModel;
            })(Designer.SerializableModel);
            Chart.TitleViewModel = TitleViewModel;
            var DataContainerViewModel = (function (_super) {
                __extends(DataContainerViewModel, _super);
                function DataContainerViewModel(model, serializer) {
                    var _this = this;
                    _super.call(this, model, serializer, Chart.dataContainerSerializationsInfo);
                    this.displayName = ko.observable("Data Container");
                    this.series = Designer.deserializeArray(model && model.SeriesSerializable || [], function (item) {
                        return new SeriesViewModel(item, _this.series, serializer);
                    });
                    this.series().forEach(function (series) {
                        series.parent = _this.series;
                    });
                    this.series()["displayName"] = ko.observable("Series");
                    var typeArray = Chart.typeNameSerializable.values;
                    var actions = [];
                    $.each(typeArray, function (name, value) {
                        actions.push({
                            text: value,
                            imageClassName: "dxrd-image-fieldlist-" + SeriesViewModel.getClassName(name),
                            disabled: ko.observable(false),
                            visible: true,
                            clickAction: function () {
                                _this.series()["innerActions"][0].closePopover();
                                _this.series.push(new SeriesViewModel({ "View": { "@TypeNameSerializable": name } }, _this.series));
                            }
                        });
                    });
                    this.series()["innerActions"] = createInnerActionsWithPopover("Add Series", "addseries-action", actions);
                }
                DataContainerViewModel.from = function (model, serializer) {
                    return new DataContainerViewModel(model, serializer);
                };
                DataContainerViewModel.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, Chart.dataContainerSerializationsInfo, refs);
                };
                DataContainerViewModel.prototype.getInfo = function () {
                    return Chart.dataContainerSerializationsInfo;
                };
                return DataContainerViewModel;
            })(Designer.SerializableModel);
            Chart.DataContainerViewModel = DataContainerViewModel;
            var SeriesTemplateViewModel = (function (_super) {
                __extends(SeriesTemplateViewModel, _super);
                function SeriesTemplateViewModel(model, serializer, info) {
                    var _this = this;
                    _super.call(this, model, serializer, info);
                    this.displayName = ko.observable("Series Template");
                    this.valueDataMembers = ko.observable(new (viewTypesDataMembers[this.view.typeName()] || CommonValueDataMembers)(this.valueDataMembers.toString()));
                    ko.computed(function () {
                        if (_this.label && _this.label.typeNameSerializable) {
                            _this.label.typeNameSerializable(mapTypes[_this.view.typeName()]);
                        }
                    });
                    ko.computed(function () {
                        _this.valueDataMembers(new (viewTypesDataMembers[_this.view.typeName()] || CommonValueDataMembers)(_this.valueDataMembers.peek().toString()));
                    });
                }
                SeriesTemplateViewModel.from = function (model, serializer) {
                    return new SeriesTemplateViewModel(model || {}, serializer, Chart.seriesTemplateSerializationsInfo);
                };
                SeriesTemplateViewModel.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, Chart.seriesTemplateSerializationsInfo, refs);
                };
                SeriesTemplateViewModel.prototype.getInfo = function () {
                    return Chart.seriesTemplateSerializationsInfo;
                };
                return SeriesTemplateViewModel;
            })(Designer.SerializableModel);
            Chart.SeriesTemplateViewModel = SeriesTemplateViewModel;
            var SeriesViewModel = (function (_super) {
                __extends(SeriesViewModel, _super);
                function SeriesViewModel(model, parent, serializer) {
                    var _this = this;
                    _super.call(this, model, serializer, Chart.seriesSerializationsInfo);
                    this.isIncompatible = ko.observable(false);
                    this.parent = parent;
                    this["displayName"] = ko.computed(function () {
                        return _this.isIncompatible() ? "(incompatible) " + _this["name"]() : _this["name"]();
                    });
                    this["className"] = ko.computed(function () {
                        return SeriesViewModel.getClassName(_this.view.typeName());
                    });
                    this["innerActions"] = [
                        {
                            text: "Remove Series",
                            imageClassName: "dxrd-image-recycle-bin",
                            disabled: ko.observable(false),
                            visible: true,
                            clickAction: function () {
                                _this.parent.remove(_this);
                            },
                        },
                    ];
                }
                SeriesViewModel.from = function (model, serializer) {
                    return new SeriesViewModel(model, null, serializer);
                };
                SeriesViewModel.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, Chart.seriesSerializationsInfo, refs);
                };
                SeriesViewModel.getClassName = function (typeName) {
                    return typeName.toLowerCase().split("seriesview")[0];
                };
                SeriesViewModel.prototype.getInfo = function () {
                    return Chart.seriesSerializationsInfo;
                };
                return SeriesViewModel;
            })(SeriesTemplateViewModel);
            Chart.SeriesViewModel = SeriesViewModel;
            var SeriesViewViewModel = (function (_super) {
                __extends(SeriesViewViewModel, _super);
                function SeriesViewViewModel(model, serializer) {
                    _super.call(this, model, serializer, Chart.viewSerializationsInfo);
                    if (this.typeName && this.typeName() === "")
                        this.typeName("SideBySideBarSeriesView");
                }
                SeriesViewViewModel.from = function (model, serializer) {
                    return new SeriesViewViewModel(model, serializer);
                };
                SeriesViewViewModel.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, Chart.viewSerializationsInfo, refs);
                };
                SeriesViewViewModel.prototype.getInfo = function () {
                    return Chart.viewSerializationsInfo;
                };
                return SeriesViewViewModel;
            })(Designer.SerializableModel);
            Chart.SeriesViewViewModel = SeriesViewViewModel;
            var SeriesLabelViewModel = (function (_super) {
                __extends(SeriesLabelViewModel, _super);
                function SeriesLabelViewModel(model, serializer) {
                    _super.call(this, model, serializer, seriesLabelSerializationsInfo);
                }
                SeriesLabelViewModel.from = function (model, serializer) {
                    return new SeriesLabelViewModel(model, serializer);
                };
                SeriesLabelViewModel.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, seriesLabelSerializationsInfo, refs);
                };
                SeriesLabelViewModel.prototype.getInfo = function () {
                    return seriesLabelSerializationsInfo;
                };
                return SeriesLabelViewModel;
            })(Designer.SerializableModel);
            Chart.SeriesLabelViewModel = SeriesLabelViewModel;
            var DiagramViewModel = (function (_super) {
                __extends(DiagramViewModel, _super);
                function DiagramViewModel(model, serializer) {
                    _super.call(this, model, serializer, Chart.diagramSerializationsInfo);
                    this.displayName = ko.observable("Diagram");
                    DiagramViewModel.initDisplayName(this);
                }
                DiagramViewModel.createDiagram = function (model, type, serializer) {
                    if (serializer === void 0) { serializer = null; }
                    var newDiagram = { "getInfo": function () {
                        return Chart.diagramMapper[type].info;
                    } };
                    (serializer || new Designer.DesignerModelSerializer()).deserialize(newDiagram, $.extend(model, { "@TypeNameSerializable": Chart.diagramMapper[type].type }));
                    this.initDisplayName(newDiagram);
                    return newDiagram;
                };
                DiagramViewModel.from = function (model, serializer) {
                    return new DiagramViewModel(model, serializer);
                };
                DiagramViewModel.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, refs);
                };
                DiagramViewModel.initDisplayName = function (diagram) {
                    diagram["displayName"] = diagram["typeNameSerializable"];
                    if (diagram["axisX"]) {
                        diagram["axisX"].displayName = ko.observable("Axis X");
                    }
                    if (diagram["axisY"]) {
                        diagram["axisY"].displayName = ko.observable("Axis Y");
                    }
                };
                DiagramViewModel.prototype.getInfo = function () {
                    return Chart.diagramSerializationsInfo;
                };
                return DiagramViewModel;
            })(Designer.SerializableModel);
            Chart.DiagramViewModel = DiagramViewModel;
            var LegendViewModel = (function (_super) {
                __extends(LegendViewModel, _super);
                function LegendViewModel(model, serializer) {
                    _super.call(this, model, serializer, Chart.legendSerializationsInfo);
                    this.displayName = ko.observable("Legend");
                }
                LegendViewModel.from = function (model, serializer) {
                    return new LegendViewModel(model, serializer);
                };
                LegendViewModel.toJson = function (value, serializer, refs) {
                    return serializer.serialize(value, Chart.legendSerializationsInfo, refs);
                };
                LegendViewModel.prototype.getInfo = function () {
                    return Chart.legendSerializationsInfo;
                };
                return LegendViewModel;
            })(Designer.SerializableModel);
            Chart.LegendViewModel = LegendViewModel;
            function createInnerActionsWithPopover(text, id, actions) {
                var object = {
                    text: text,
                    imageClassName: "dxrd-image-add",
                    disabled: ko.observable(false),
                    id: id,
                    _visible: ko.observable(false),
                    popoverVisible: null,
                    togglePopoverVisible: null,
                    closePopover: null,
                    templateName: "dxrd-collectionactions-template",
                    actions: actions
                };
                object.popoverVisible = ko.computed(function () {
                    return object._visible();
                });
                object.togglePopoverVisible = function () {
                    object._visible(!object._visible());
                };
                object.closePopover = function () {
                    object._visible(false);
                };
                return [object];
            }
            ;
            var valueDataMembersSerializationsInfo = [
                { propertyName: "valueEditable", displayName: "Value", editor: Designer.Widgets.editorTemplates.field },
            ];
            var valueWeightDataMembersSerializationsInfo = [
                { propertyName: "valueEditable", displayName: "Value", editor: Designer.Widgets.editorTemplates.field },
                { propertyName: "weightEditable", displayName: "Weight", editor: Designer.Widgets.editorTemplates.field },
            ];
            var value1Value2DataMembersSerializationsInfo = [
                { propertyName: "value1Editable", displayName: "Value", editor: Designer.Widgets.editorTemplates.field },
                { propertyName: "value2Editable", displayName: "Value", editor: Designer.Widgets.editorTemplates.field },
            ];
            var stockDataMembersSerializationsInfo = [
                { propertyName: "closeEditable", displayName: "Close", editor: Designer.Widgets.editorTemplates.field },
                { propertyName: "hightEditable", displayName: "Hight", editor: Designer.Widgets.editorTemplates.field },
                { propertyName: "lowEditable", displayName: "Low", editor: Designer.Widgets.editorTemplates.field },
                { propertyName: "openEditable", displayName: "Open", editor: Designer.Widgets.editorTemplates.field },
            ];
            var mapTypes = {
                "SideBySideBarSeriesView": "SideBySideBarSeriesLabel",
                "StackedBarSeriesView": "StackedBarSeriesLabel",
                "FullStackedBarSeriesView": "FullStackedBarSeriesLabel",
                "SideBySideStackedBarSeriesView": "StackedBarSeriesLabel",
                "SideBySideFullStackedBarSeriesView": "FullStackedBarSeriesLabel",
                "SideBySideBar3DSeriesView": "Bar3DSeriesLabel",
                "StackedBar3DSeriesView": "StackedBar3DSeriesLabel",
                "FullStackedBar3DSeriesView": "FullStackedBar3DSeriesLabel",
                "SideBySideStackedBar3DSeriesView": "StackedBar3DSeriesLabel",
                "SideBySideFullStackedBar3DSeriesView": "FullStackedBar3DSeriesLabel",
                "ManhattanBarSeriesView": "Bar3DSeriesLabel",
                "PointSeriesView": "PointSeriesLabel",
                "BubbleSeriesView": "BubbleSeriesLabel",
                "LineSeriesView": "PointSeriesLabel",
                "StackedLineSeriesView": "StackedLineSeriesLabel",
                "FullStackedLineSeriesView": "StackedLineSeriesLabel",
                "StepLineSeriesView": "PointSeriesLabel",
                "SplineSeriesView": "PointSeriesLabel",
                "ScatterLineSeriesView": "PointSeriesLabel",
                "SwiftPlotSeriesView": null,
                "Line3DSeriesView": "Line3DSeriesLabel",
                "StackedLine3DSeriesView": "StackedLine3DSeriesLabel",
                "FullStackedLine3DSeriesView": "StackedLine3DSeriesLabel",
                "StepLine3DSeriesView": "Line3DSeriesLabel",
                "Spline3DSeriesView": "Line3DSeriesLabel",
                "PieSeriesView": "PieSeriesLabel",
                "DoughnutSeriesView": "DoughnutSeriesLabel",
                "NestedDoughnutSeriesView": "NestedDoughnutSeriesLabel",
                "Pie3DSeriesView": "Pie3DSeriesLabel",
                "Doughnut3DSeriesView": "Doughnut3DSeriesLabel",
                "FunnelSeriesView": "FunnelSeriesLabel",
                "Funnel3DSeriesView": "Funnel3DSeriesLabel",
                "AreaSeriesView": "PointSeriesLabel",
                "StackedAreaSeriesView": "PointSeriesLabel",
                "FullStackedAreaSeriesView": "FullStackedAreaSeriesLabel",
                "StepAreaSeriesView": "PointSeriesLabel",
                "SplineAreaSeriesView": "PointSeriesLabel",
                "StackedSplineAreaSeriesView": "PointSeriesLabel",
                "FullStackedSplineAreaSeriesView": "FullStackedSplineAreaSeriesLabel",
                "Area3DSeriesView": "Area3DSeriesLabel",
                "StackedArea3DSeriesView": "StackedArea3DSeriesLabel",
                "FullStackedArea3DSeriesView": "FullStackedArea3DSeriesLabel",
                "StepArea3DSeriesView": "Area3DSeriesLabel",
                "SplineArea3DSeriesView": "Area3DSeriesLabel",
                "StackedSplineArea3DSeriesView": "StackedArea3DSeriesLabel",
                "FullStackedSplineArea3DSeriesView": "FullStackedArea3DSeriesLabel",
                "OverlappedRangeBarSeriesView": "RangeBarSeriesLabel",
                "SideBySideRangeBarSeriesView": "RangeBarSeriesLabel",
                "RangeAreaSeriesView": "RangeAreaSeriesLabel",
                "RangeArea3DSeriesView": "RangeArea3DSeriesLabel",
                "RadarPointSeriesView": "RadarPointSeriesLabel",
                "RadarLineSeriesView": "RadarPointSeriesLabel",
                "RadarAreaSeriesView": "RadarPointSeriesLabel",
                "PolarPointSeriesView": "RadarPointSeriesLabel",
                "PolarLineSeriesView": "RadarPointSeriesLabel",
                "PolarAreaSeriesView": "RadarPointSeriesLabel",
                "StockSeriesView": "StockSeriesLabel",
                "CandleStickSeriesView": "StockSeriesLabel",
                "OverlappedGanttSeriesView": "RangeBarSeriesLabel",
                "SideBySideGanttSeriesView": "RangeBarSeriesLabel"
            };
            Chart.typeNameSerializable = {
                propertyName: "typeName",
                modelName: "@TypeNameSerializable",
                displayName: "Type",
                defaultVal: "",
                editor: Chart.editorTemplates.views,
                values: {
                    "SideBySideBarSeriesView": "Bar",
                    "StackedBarSeriesView": "Bar Stacked",
                    "FullStackedBarSeriesView": "Bar Stacked 100%",
                    "SideBySideStackedBarSeriesView": "Side By Side Bar Stacked",
                    "SideBySideFullStackedBarSeriesView": "Side By Side Bar Stacked 100%",
                    "SideBySideBar3DSeriesView": "Bar 3D",
                    "StackedBar3DSeriesView": "Bar 3D Stacked",
                    "FullStackedBar3DSeriesView": "Bar 3D Stacked 100%",
                    "SideBySideStackedBar3DSeriesView": "Side By Side Bar 3D Stacked ",
                    "SideBySideFullStackedBar3DSeriesView": "Side By Side Bar 3D Stacked 100%",
                    "ManhattanBarSeriesView": "Manhattan Bar",
                    "PointSeriesView": "Point",
                    "BubbleSeriesView": "Bubble",
                    "LineSeriesView": "Line",
                    "StackedLineSeriesView": "Line Stacked",
                    "FullStackedLineSeriesView": "Line Stacked 100%",
                    "StepLineSeriesView": "Step Line",
                    "SplineSeriesView": "Spline",
                    "ScatterLineSeriesView": "Scatter Line",
                    "SwiftPlotSeriesView": "Swift Plot",
                    "Line3DSeriesView": "Line 3D",
                    "StackedLine3DSeriesView": "Line 3D Stacked",
                    "FullStackedLine3DSeriesView": "Line 3D Stacked 100%",
                    "StepLine3DSeriesView": "Step Line 3D",
                    "Spline3DSeriesView": "Spline 3D",
                    "PieSeriesView": "Pie",
                    "DoughnutSeriesView": "Doughnut",
                    "NestedDoughnutSeriesView": "Nested Doughnut",
                    "Pie3DSeriesView": "Pie 3D",
                    "Doughnut3DSeriesView": "Doughnut 3D",
                    "FunnelSeriesView": "Funnel",
                    "Funnel3DSeriesView": "Funnel 3D",
                    "AreaSeriesView": "Area",
                    "StackedAreaSeriesView": "Area Stacked",
                    "FullStackedAreaSeriesView": "Area Stacked 100%",
                    "StepAreaSeriesView": "Step Area",
                    "SplineAreaSeriesView": "Spline Area",
                    "StackedSplineAreaSeriesView": "Spline Area Stacked",
                    "FullStackedSplineAreaSeriesView": "Spline Area Stacked 100%",
                    "Area3DSeriesView": "Area 3D",
                    "StackedArea3DSeriesView": "Area 3D Stacked",
                    "FullStackedArea3DSeriesView": "Area 3D Stacked 100%",
                    "StepArea3DSeriesView": "Step 3D Area",
                    "SplineArea3DSeriesView": "Spline 3D Area",
                    "StackedSplineArea3DSeriesView": "Spline Area 3D Stacked",
                    "FullStackedSplineArea3DSeriesView": "Spline Area 3D Stacked 100%",
                    "OverlappedRangeBarSeriesView": "Range Bar",
                    "SideBySideRangeBarSeriesView": "Side By Side Range Bar",
                    "RangeAreaSeriesView": "Range Area",
                    "RangeArea3DSeriesView": "Range Area 3D",
                    "RadarPointSeriesView": "Radar Point",
                    "RadarLineSeriesView": "Radar Line",
                    "RadarAreaSeriesView": "Radar Area",
                    "PolarPointSeriesView": "Polar Point",
                    "PolarLineSeriesView": "Polar Line",
                    "PolarAreaSeriesView": "Polar Area",
                    "StockSeriesView": "Stock Series",
                    "CandleStickSeriesView": "Candle Stick",
                    "OverlappedGanttSeriesView": "Gantt",
                    "SideBySideGanttSeriesView": "Side By Side Gantt"
                }
            };
            Chart.diagram = { propertyName: "diagram", modelName: "Diagram" };
            var sideBySideEqualBarWidth = { propertyName: "sideBySideEqualBarWidth", modelName: "@SideBySideEqualBarWidth", displayName: "Side By Side Equal Bar Width", defaultVal: true, editor: Designer.Widgets.editorTemplates.bool }, sideBySideBarDistanceFixed = { propertyName: "sideBySideBarDistanceFixed", modelName: "@SideBySideBarDistanceFixed", displayName: "Side By Side Bar Distance Fixed", defaultVal: 1, editor: Designer.Widgets.editorTemplates.numeric }, sideBySideBarDistance = { propertyName: "sideBySideBarDistance", modelName: "@SideBySideBarDistance", displayName: "Side By Side Bar Distance", defaultVal: 0.0, editor: Designer.Widgets.editorTemplates.numeric };
            Chart.seriesPointsSorting = {
                propertyName: "seriesPointsSorting",
                modelName: "@SeriesPointsSorting",
                displayName: "Series Points Sorting",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "None": "None",
                    "Ascending": "Ascending",
                    "Descending": "Descending"
                }
            }, Chart.seriesPointsSortingKey = {
                propertyName: "seriesPointsSortingKey",
                modelName: "@SeriesPointsSortingKey",
                displayName: "Series Points Sorting Key",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Argument": "Argument",
                    "Value_1": "Value_1",
                    "Value_2": "Value_2",
                    "Value_3": "Value_3",
                    "Value_4": "Value_4"
                }
            }, Chart.toolTipEnabled = { propertyName: "toolTipEnabled", modelName: "@ToolTipEnabled", displayName: "Tool Tip Enabled", editor: Designer.Widgets.editorTemplates.combobox, values: Chart.defaultBooleanValues }, Chart.crosshairHighlightPoints = { propertyName: "crosshairHighlightPoints", modelName: "@CrosshairHighlightPoints", displayName: "Crosshair Highlight Points", editor: Designer.Widgets.editorTemplates.combobox, values: Chart.defaultBooleanValues }, Chart.checkedInLegend = { propertyName: "checkedInLegend", modelName: "@CheckedInLegend", displayName: "Checked In Legend", editor: Designer.Widgets.editorTemplates.bool }, Chart.checkableInLegend = { propertyName: "checkableInLegend", modelName: "@CheckableInLegend", displayName: "Checkable In Legend", editor: Designer.Widgets.editorTemplates.bool }, Chart.legendTextPattern = { propertyName: "legendTextPattern", modelName: "@LegendTextPattern", displayName: "Legend Text Pattern", editor: Designer.Widgets.editorTemplates.text }, Chart.legendText = { propertyName: "legendText", modelName: "@LegendText", displayName: "Legend Text", editor: Designer.Widgets.editorTemplates.text }, Chart.tag_type = { propertyName: "tag_type", modelName: "@Tag_type", displayName: "Tag_type", editor: Designer.Widgets.editorTemplates.text }, Chart.argumentScaleType = { propertyName: "argumentScaleType", modelName: "@ArgumentScaleType", displayName: "Argument Scale Type", editor: Designer.Widgets.editorTemplates.combobox, values: Chart.scaleTypeValues }, Chart.valueScaleType = { propertyName: "valueScaleType", modelName: "@ValueScaleType", displayName: "Value Scale Type", editor: Designer.Widgets.editorTemplates.combobox, values: Chart.scaleTypeValues }, Chart.labelsVisibility = { propertyName: "labelsVisibility", modelName: "@LabelsVisibility", displayName: "Labels Visibility", defaultVal: "Default", editor: Designer.Widgets.editorTemplates.combobox, values: Chart.defaultBooleanValues }, Chart.argumentDataMember = { propertyName: "argumentDataMember", modelName: "@ArgumentDataMember" }, Chart.valueDataMembersSerializable = { propertyName: "valueDataMembers", modelName: "@ValueDataMembersSerializable", editor: Designer.Widgets.editorTemplates.objecteditor, displayName: "Value Data Members", from: CommonValueDataMembers.from, toJsonObject: CommonValueDataMembers.toJson }, Chart.argumentDataMemberEditable = { propertyName: "argumentDataMemberEditable", displayName: "ArgumentDataMember", editor: Designer.Widgets.editorTemplates.field }, Chart.showInLegend = { propertyName: "showInLegend", modelName: "@ShowInLegend", displayName: "Show In Legend", defaultVal: true, editor: Designer.Widgets.editorTemplates.bool };
            var gradientMode = { propertyName: "gradientMode", modelName: "@GradientMode", displayName: "Gradient Mode", editor: Designer.Widgets.editorTemplates.text }, color2 = { propertyName: "color2", modelName: "@Color2", displayName: "Color2", from: Designer.colorFromString, toJsonObject: Designer.colorToString, editor: Designer.Widgets.editorTemplates.color }, typeNameSerializableOptions = { propertyName: "typeNameSerializable", modelName: "@TypeNameSerializable", displayName: "Type Name Serializable", editor: Designer.Widgets.editorTemplates.text }, colorEach = { propertyName: "colorEach", modelName: "@ColorEach", displayName: "Color Each", defaultVal: false, editor: Designer.Widgets.editorTemplates.bool }, transparency = { propertyName: "transparency", modelName: "@Transparency", displayName: "Transparency", editor: Designer.Widgets.editorTemplates.numeric }, barWidth = { propertyName: "barWidth", modelName: "@BarWidth", displayName: "BarWidth", editor: Designer.Widgets.editorTemplates.numeric }, fillMode = {
                propertyName: "fillMode",
                modelName: "@FillMode",
                displayName: "Fill Mode",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Empty": "Empty",
                    "Solid": "Solid",
                    "Gradient": "Gradient",
                    "Hatch": "Hatch"
                }
            }, fillMode3D = {
                propertyName: "fillMode3D",
                modelName: "@FillMode",
                displayName: "Fill Mode",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Empty": "Empty",
                    "Solid": "Solid",
                    "Gradient": "Gradient"
                }
            }, size = { propertyName: "size", modelName: "@Size", displayName: "Size", defaultVal: 2, editor: Designer.Widgets.editorTemplates.numeric }, hatchStyle = { propertyName: "hatchStyle", modelName: "@HatchStyle", displayName: "Hatch Style", editor: Designer.Widgets.editorTemplates.text };
            var enabled = { propertyName: "enabled", modelName: "@Enabled", displayName: "Enabled", defaultVal: false, editor: Designer.Widgets.editorTemplates.bool }, mode = {
                propertyName: "mode",
                modelName: "@Mode",
                displayName: "Mode",
                editor: Designer.Widgets.editorTemplates.combobox,
                defaultValue: "Count",
                values: {
                    "Count": "Count",
                    "ThresholdValue": "Threshold Value",
                    "ThresholdPercent": "Threshold Percent"
                }
            }, count = { propertyName: "count", modelName: "@Count", displayName: "Count", defaultVal: 5, editor: Designer.Widgets.editorTemplates.numeric }, showOthers = { propertyName: "showOthers", modelName: "@ShowOthers", displayName: "Show Others", editor: Designer.Widgets.editorTemplates.bool }, othersArgument = { propertyName: "othersArgument", modelName: "@OthersArgument", displayName: "OthersArgument", editor: Designer.Widgets.editorTemplates.text }, thresholdValue = { propertyName: "thresholdValue", modelName: "@ThresholdValue", displayName: "Threshold Value", editor: Designer.Widgets.editorTemplates.numeric }, thresholdPercent = { propertyName: "thresholdPercent", modelName: "@ThresholdPercent", displayName: "Threshold Percent", editor: Designer.Widgets.editorTemplates.numeric };
            Chart.textOrientation = {
                propertyName: "textOrientation",
                modelName: "@TextOrientation",
                displayName: "Text Orientation",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Horizontal ": "Horizontal",
                    "TopToBottom": "Top To Bottom",
                    "BottomToTop": "Bottom To Top"
                }
            }, Chart.resolveOverlappingMode = {
                propertyName: "resolveOverlappingMode",
                modelName: "@ResolveOverlappingMode",
                displayName: "Resolve Overlapping Mode",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "None": "None",
                    "Default": "Default",
                    "HideOverlapped": "Hide Overlapped",
                    "JustifyAroundPoint": "Justify Around Point",
                    "JustifyAllAroundPoint": "Justify All Around Point"
                }
            }, Chart.lineColor = { propertyName: "lineColor", modelName: "@LineColor", displayName: "Line Color", from: Designer.colorFromString, toJsonObject: Designer.colorToString, editor: Designer.Widgets.editorTemplates.color }, Chart.lineVisibility = { propertyName: "lineVisibility", modelName: "@LineVisibility", displayName: "Line Visibility", editor: Designer.Widgets.editorTemplates.combobox, values: Chart.defaultBooleanValues }, Chart.lineLength = { propertyName: "lineLength", modelName: "@LineLength", displayName: "Line Length", editor: Designer.Widgets.editorTemplates.numeric }, Chart.barPosition = {
                propertyName: "barPosition",
                modelName: "@Position",
                displayName: "Position",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Top": "Top",
                    "Center": "Center",
                    "TopInside": "Top Inside",
                    "BottomInside": "Bottom Inside"
                }
            }, Chart.showForZeroValues = { propertyName: "showForZeroValues", modelName: "@ShowForZeroValues", displayName: "Show for Zero Values", editor: Designer.Widgets.editorTemplates.bool };
            var dashStyle = {
                propertyName: "dashStyle",
                modelName: "@DashStyle",
                displayName: "Dash Style",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Empty": "Empty",
                    "Solid": "Solid",
                    "Dash": "Dash",
                    "Dot": "Dot",
                    "DashDot": "Dash-Dot",
                    "DashDotDot": "Dash-Dot-Dot"
                }
            };
            var markerVisibility = { propertyName: "markerVisibility", modelName: "@MarkerVisibility", displayName: "Marker Visibility", editor: Designer.Widgets.editorTemplates.combobox, values: Chart.defaultBooleanValues }, markerKind = {
                propertyName: "kind",
                modelName: "@Kind",
                displayName: "Kind",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Square": "Square",
                    "Diamond": "Diamond",
                    "Triangle": "Triangle",
                    "InvertedTriangle": "Inverted Triangle",
                    "Circle": "Circle",
                    "Plus": "Plus",
                    "Cross": "Cross",
                    "Star": "Star",
                    "Pentagon": "Pentagon",
                    "Hexagon": "Hexagon"
                }
            }, borderVisible = { propertyName: "borderVisible", modelName: "@BorderVisible", displayName: "Border Visible", editor: Designer.Widgets.editorTemplates.bool };
            Chart.direction = {
                propertyName: "direction",
                modelName: "@Direction",
                displayName: "Direction",
                defaultVal: "TopToBottom",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "TopToBottom": "Top To Bottom",
                    "BottomToTop": "Bottom To Top",
                    "LeftToRight": "Left To Right",
                    "RightToLeft": "Right To Left"
                }
            }, Chart.alignmentVertical = {
                propertyName: "alignmentVertical",
                modelName: "@AlignmentVertical",
                displayName: "Alignment Vertical",
                defaultVal: "Top",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Top": "Top",
                    "TopOutside": "Top Outside",
                    "Center": "Center",
                    "Bottom": "Bottom",
                    "BottomOutside": "Bottom Outside"
                }
            }, Chart.alignmentHorizontal = {
                propertyName: "alignmentHorizontal",
                modelName: "@AlignmentHorizontal",
                displayName: "Alignment Horizontal",
                defaultVal: "RightOutside",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Left": "Left",
                    "LeftOutside": "Left Outside",
                    "Center": "Center",
                    "Right": "Right",
                    "RightOutside": "Right Outside"
                }
            };
            Chart.dock = {
                propertyName: "dock",
                modelName: "@Dock",
                displayName: "Dock",
                defaultVal: "Top",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Top": "Top",
                    "Bottom": "Bottom",
                    "Left": "Left",
                    "Right": "Right"
                }
            };
            Chart.chartTitleText = { propertyName: "text", modelName: "@Text", displayName: "Text", editor: Designer.Widgets.editorTemplates.text };
            Chart.padding = { propertyName: "chartPadding", modelName: "Padding", displayName: "Padding", info: [Chart.left, Chart.right, Chart.top, Chart.bottom], editor: Designer.Widgets.editorTemplates.objecteditor };
            var optionsSerializationsInfo = [gradientMode, color2, typeNameSerializableOptions], options = { propertyName: "options", modelName: "Options", displayName: "Options", info: optionsSerializationsInfo, defaultVal: {}, editor: Designer.Widgets.editorTemplates.objecteditor };
            var lineMarkerOptionsSerializationsInfo = [Chart.color, colorEach, markerVisibility], lineMarker = { propertyName: "lineMarker", modelName: "LineMarker", displayName: "Line Marker", info: lineMarkerOptionsSerializationsInfo, editor: Designer.Widgets.editorTemplates.objecteditor };
            Chart.borderSerializationsInfo = [Chart.color, Chart.thickness, Chart.visibility], Chart.border = { propertyName: "border", modelName: "Border", displayName: "Border", info: Chart.borderSerializationsInfo, defaultVal: {}, editor: Designer.Widgets.editorTemplates.objecteditor };
            var topNOptionsSerializationsInfo = [enabled, mode, count, thresholdPercent, thresholdValue, showOthers, othersArgument], topNOptions = { propertyName: "topNOptions", modelName: "TopNOptions", displayName: "Top N Options", info: topNOptionsSerializationsInfo, defaultVal: {}, editor: Designer.Widgets.editorTemplates.objecteditor };
            var lineStyleSerializationsInfo = [Chart.thickness, dashStyle], lineStyle = { propertyName: "lineStyle", modelName: "LineStyle", displayName: "Line Style", info: lineStyleSerializationsInfo, defaultVal: {}, editor: Designer.Widgets.editorTemplates.objecteditor };
            var seriesLabelSerializationsInfo = [Chart.typeNameNotShow, Chart.textPattern, Chart.textAlignment, Chart.maxLineCount, Chart.maxWidth, Chart.textOrientation, Chart.resolveOverlappingMode, Chart.lineColor, Chart.lineVisibility, Chart.lineLength, Chart.antialiasing, Chart.backColor, Chart.textColor, Chart.barPosition, Chart.showForZeroValues, Chart.font8, lineStyle, Chart.border];
            Chart.seriesLabel = { propertyName: "label", modelName: "Label", displayName: "Label", info: seriesLabelSerializationsInfo, defaultVal: {}, from: SeriesLabelViewModel.from, toJsonObject: SeriesLabelViewModel.toJson, editor: Designer.Widgets.editorTemplates.objecteditor };
            Chart.viewSerializationsInfo = [Chart.typeNameSerializable, Chart.color, colorEach, Chart.border], Chart.view = {
                propertyName: "view",
                modelName: "View",
                displayName: "View",
                defaultVal: {},
                info: Chart.viewSerializationsInfo,
                from: SeriesViewViewModel.from,
                toJsonObject: SeriesViewViewModel.toJson,
                editor: Designer.Widgets.editorTemplates.objecteditor
            }, Chart.seriesTemplateSerializationsInfo = [Chart.labelsVisibility, Chart.argumentDataMember, Chart.valueDataMembersSerializable, Chart.argumentDataMemberEditable, Chart.showInLegend, Chart.view, Chart.seriesLabel, topNOptions], Chart.seriesTemplate = { propertyName: "seriesTemplate", modelName: "SeriesTemplate", displayName: "Series Template", info: Chart.seriesTemplateSerializationsInfo, from: SeriesTemplateViewModel.from, toJsonObject: SeriesTemplateViewModel.toJson, editor: Designer.Widgets.editorTemplates.objecteditor }, Chart.seriesSerializationsInfo = [Chart.name, Chart.visible].concat(Chart.seriesTemplateSerializationsInfo), Chart.seriesSerializable = { propertyName: "series", modelName: "SeriesSerializable", displayName: "Series", array: true, editor: Chart.editorTemplates.series }, Chart.argumentDataMember = { propertyName: "argumentDataMember", modelName: "@ArgumentDataMember" }, Chart.argumentDataMemberEditable = { propertyName: "argumentDataMemberEditable", displayName: "Argument Data Member", editor: Designer.Widgets.editorTemplates.field }, Chart.seriesDataMember = { propertyName: "seriesDataMember", modelName: "@SeriesDataMember" }, Chart.seriesDataMemberEditable = { propertyName: "seriesDataMemberEditable", displayName: "Series Data Member", editor: Designer.Widgets.editorTemplates.field }, Chart.dataContainerSerializationsInfo = [Chart.seriesDataMember, Chart.seriesSerializable, Chart.seriesTemplate, Chart.dataMember], Chart.dataContainer = { propertyName: "dataContainer", modelName: "DataContainer", displayName: "Data Container", info: Chart.dataContainerSerializationsInfo, from: DataContainerViewModel.from, toJsonObject: DataContainerViewModel.toJson, editor: Designer.Widgets.editorTemplates.objecteditorCustom };
            Chart.titleSerializationsInfo = [Chart.chartTitleText, Chart.textColor, Chart.dock, Chart.titleAlignment, Chart.visibility, Chart.font18], Chart.titles = { propertyName: "titles", modelName: "Titles", displayName: "Titles", array: true, from: TitleViewModel.from, toJsonObject: TitleViewModel.toJson, editor: Chart.editorTemplates.titles };
            Chart.legendSerializationsInfo = [Chart.textColor, Chart.backColor, Chart.direction, Chart.alignmentVertical, Chart.alignmentHorizontal, Chart.visibility, Chart.border, Chart.margin, Chart.padding, Chart.font8], Chart.legend = { propertyName: "legend", modelName: "Legend", displayName: "Legend", info: Chart.legendSerializationsInfo, from: LegendViewModel.from, toJsonObject: LegendViewModel.toJson, defaultVal: {}, editor: Designer.Widgets.editorTemplates.objecteditor };
            Chart.appearanceName = {
                propertyName: "appearanceName",
                modelName: "@AppearanceNameSerializable",
                displayName: "Appearance Name",
                defaultVal: "Default",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Nature Colors": "Nature Colors",
                    "Pastel Kit": "Pastel Kit",
                    "In A Fog": "In A Fog",
                    "Terracotta Pie": "Terracotta Pie",
                    "Northern Lights": "Northern Lights",
                    "Chameleon": "Chameleon",
                    "The Trees": "The Trees",
                    "Light": "Light",
                    "Gray": "Gray",
                    "Dark": "Dark",
                    "Dark Flat": "Dark Flat",
                    "Default": "Default"
                }
            }, Chart.paletteName = {
                propertyName: "paletteName",
                modelName: "@PaletteName",
                displayName: "Palette Name",
                defaultVal: "Default",
                editor: Designer.Widgets.editorTemplates.combobox,
                values: {
                    "Default": "Default",
                    "Nature Colors": "Nature Colors",
                    "Pastel Kit": "Pastel Kit",
                    "In A Fog": "In A Fog",
                    "Terracotta Pie": "Terracotta Pie",
                    "Northern Lights": "Northern Lights",
                    "Chameleon": "Chameleon",
                    "The Trees": "The Trees",
                    "Mixed": "Mixed",
                    "Office": "Office",
                    "Black and White": "Black and White",
                    "Grayscale": "Grayscale",
                    "Apex": "Apex",
                    "Aspect": "Aspect",
                    "Civic": "Civic",
                    "Concourse": "Concourse",
                    "Equity": "Equity",
                    "Flow": "Flow",
                    "Foundry": "Foundry",
                    "Median": "Median",
                    "Metro": "Metro",
                    "Module": "Module",
                    "Opulent": "Opulent",
                    "Oriel": "Oriel",
                    "Origin": "Origin",
                    "Paper": "Paper",
                    "Solstice": "Solstice",
                    "Technic": "Technic",
                    "Trek": "Trek",
                    "Urban": "Urban",
                    "Verve": "Verve",
                    "Office2013": "Office2013",
                    "Blue Warm": "Blue Warm",
                    "Blue": "Blue",
                    "Blue II": "Blue II",
                    "Blue Green": "Blue Green",
                    "Green": "Green",
                    "Green Yellow": "Green Yellow",
                    "Yellow": "Yellow",
                    "Yellow Orange": "Yellow Orange",
                    "Orange": "Orange",
                    "Orange Red": "Orange Red",
                    "Red Orange": "Red Orange",
                    "Red": "Red",
                    "Red Violet": "Red Violet",
                    "Violet": "Violet",
                    "Violet II": "Violet II",
                    "Marquee": "Marquee",
                    "Slipstream": "Slipstream"
                }
            };
            Chart.chartSerializationsInfo = [Chart.appearanceName, Chart.paletteName, Chart.dataContainer, Chart.diagram, Chart.titles, Chart.legend], Chart.chart = { propertyName: "chart", modelName: "Chart", displayName: "Chart", from: ChartViewModel.from, toJsonObject: ChartViewModel.toJson };
        })(Chart = Designer.Chart || (Designer.Chart = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Chart;
        (function (Chart) {
            var DataSourceMemberHelper = (function () {
                function DataSourceMemberHelper(dataSource, targetMember) {
                    var _this = this;
                    this._dataSourceMember = {
                        fullPath: ko.computed({
                            read: function () {
                                if (dataSource()) {
                                    return dataSource().id + (targetMember() ? "." + targetMember() : "");
                                }
                                return targetMember();
                            },
                            write: function (val) {
                                targetMember((new Designer.Widgets.PathRequest(val)).path);
                            }
                        }),
                        displayExpr: function (value) {
                            return (new Designer.Widgets.PathRequest(value)).path;
                        }
                    };
                    this.dataSourceMemberEditable = ko.computed({
                        read: function () {
                            return _this._dataSourceMember;
                        },
                        write: function (value) {
                            _this._dataSourceMember.fullPath(value);
                        }
                    });
                }
                return DataSourceMemberHelper;
            })();
            Chart.DataSourceMemberHelper = DataSourceMemberHelper;
            var ChartControlViewModel = (function (_super) {
                __extends(ChartControlViewModel, _super);
                function ChartControlViewModel(chartSource, dataSource, size) {
                    var _this = this;
                    var serializer = new Designer.DesignerModelSerializer();
                    _super.call(this, chartSource, null, serializer);
                    this.dataSource = ko.observable(dataSource);
                    this._chartStructure = ko.observable();
                    this._chartElement = ko.observable(null);
                    this.controlType = "ChartControl";
                    this.name("ChartControl");
                    this.size = size;
                    this["appearanceName"] = this.chart["appearanceName"];
                    this["paletteName"] = this.chart["paletteName"];
                    ko.computed(function () {
                        _this.rotated = _this.chart.diagram() && _this.chart.diagram()["rotated"] ? _this.chart.diagram()["rotated"] : undefined;
                    });
                    this.fakeChart = {
                        seriesTemplate: this.chart.dataContainer.seriesTemplate,
                        series: this.chart.dataContainer.series,
                        titles: this.chart.titles,
                        legend: this.chart["legend"],
                        axisX: ko.computed(function () {
                            return _this.chart.diagram() && _this.chart.diagram()["axisX"];
                        }),
                        axisY: ko.computed(function () {
                            return _this.chart.diagram() && _this.chart.diagram()["axisY"];
                        }),
                        getInfo: function () {
                            return fakeChartSerializationInfo;
                        }
                    };
                    this._chartStructureProvider = new Designer.ObjectStructureProvider(this.fakeChart, "Chart");
                    this.chartStructureTreeListController = new Designer.ObjectStructureTreeListController(this._chartStructureProvider, ["chart", "titles", "legend", "series", "axisX", "axisY", "seriesTemplate", "TitleViewModel", "SeriesViewModel"], ["chart", "titles", "series"]);
                    this._chartStructure({
                        "itemsProvider": this._chartStructureProvider,
                        "treeListController": this.chartStructureTreeListController,
                        "selectedPath": this._chartStructureProvider.selectedPath
                    });
                    ko.computed(function () {
                        _this.chart.dataContainer.series().forEach(function (series) {
                            var memberHelper1 = new DataSourceMemberHelper(_this.dataSource, Designer.dataMemberWrapperCreator(_this.chart.dataContainer.dataMember, series.argumentDataMember));
                            series.argumentDataMemberEditable = memberHelper1.dataSourceMemberEditable;
                        });
                    });
                    ko.computed(function () {
                        _this.chart.dataContainer.series().forEach(function (series) {
                            $.each(series.valueDataMembers() || {}, function (name, value) {
                                if (ko.isObservable(series.valueDataMembers()[name])) {
                                    var memberHelper2 = new DataSourceMemberHelper(_this.dataSource, Designer.dataMemberWrapperCreator(_this.chart.dataContainer.dataMember, series.valueDataMembers()[name]));
                                    series.valueDataMembers()[name + "Editable"] = memberHelper2.dataSourceMemberEditable;
                                }
                            });
                        });
                    });
                    var memberHelper = new DataSourceMemberHelper(this.dataSource, Designer.dataMemberWrapperCreator(this.chart.dataContainer.dataMember, this.chart.dataContainer.seriesTemplate.argumentDataMember));
                    this.chart.dataContainer.seriesTemplate.argumentDataMemberEditable = memberHelper.dataSourceMemberEditable;
                    ko.computed(function () {
                        $.each(_this.chart.dataContainer.seriesTemplate.valueDataMembers() || {}, function (name, value) {
                            if (ko.isObservable(_this.chart.dataContainer.seriesTemplate.valueDataMembers()[name])) {
                                var memberHelper2 = new DataSourceMemberHelper(_this.dataSource, Designer.dataMemberWrapperCreator(_this.chart.dataContainer.dataMember, _this.chart.dataContainer.seriesTemplate.valueDataMembers()[name]));
                                _this.chart.dataContainer.seriesTemplate.valueDataMembers()[name + "Editable"] = memberHelper2.dataSourceMemberEditable;
                            }
                        });
                    });
                    var memberHelper1 = new DataSourceMemberHelper(this.dataSource, Designer.dataMemberWrapperCreator(this.chart.dataContainer.dataMember, this.chart.dataContainer.seriesDataMember));
                    this.chart.dataContainer.seriesDataMemberEditable = memberHelper1.dataSourceMemberEditable;
                    var memberHelper2 = new DataSourceMemberHelper(this["dataSource"], this.chart.dataContainer.dataMember);
                    this["dataMemberEditable"] = memberHelper2.dataSourceMemberEditable;
                    this["dataMemberEditable"]()["treeListController"] = new Designer.Widgets.DataMemberTreeListController();
                    this["seriesDataMember"] = this.chart.dataContainer.seriesDataMember;
                    this["seriesDataMemberEditable"] = this.chart.dataContainer.seriesDataMemberEditable;
                    this._chartStructureProvider.selectedMember.subscribe(function (newValue) {
                        var selectedElement = null;
                        var pathComponets = _this._chartStructureProvider.selectedPath().split(".");
                        if (newValue && pathComponets.length > 1) {
                            selectedElement = newValue;
                            if (selectedElement.getInfo === void 0) {
                                var propertySerializationInfo = fakeChartSerializationInfo.filter(function (info) {
                                    return info.propertyName === pathComponets[1];
                                })[0];
                                if (propertySerializationInfo.info) {
                                    selectedElement.getInfo = function () {
                                        return propertySerializationInfo.info;
                                    };
                                }
                                else {
                                    selectedElement = {
                                        element: selectedElement,
                                        getInfo: function () {
                                            return [$.extend({}, propertySerializationInfo, { propertyName: "element" })];
                                        }
                                    };
                                }
                            }
                        }
                        _this._chartElement(selectedElement);
                    });
                    this._chartStructureProvider.selectedPath("");
                    this.chart.dataContainer.series.subscribe(this._createCollectionSubscriptionDelegate("Chart", "series"), null, "arrayChange");
                    this.chart.titles.subscribe(this._createCollectionSubscriptionDelegate("Chart", "titles"), null, "arrayChange");
                }
                ChartControlViewModel.prototype.getInfo = function () {
                    return Chart.chartControlSerializationsInfo;
                };
                ChartControlViewModel.prototype.getControlFactory = function () {
                    return Chart.controlsFactory;
                };
                ChartControlViewModel.prototype._createCollectionSubscriptionDelegate = function (propertyPath, propertyName) {
                    var self = this, path = propertyPath + "." + propertyName;
                    return function (args) {
                        args.forEach(function (changeSet) {
                            if (changeSet.status) {
                                if (changeSet.status === "deleted") {
                                    self._chartStructureProvider.selectedPath(path);
                                    if (self.fakeChart[propertyName]().length !== 0) {
                                        self._chartStructureProvider.selectedPath(path + ".0");
                                    }
                                }
                                else {
                                    if (!changeSet.value.name()) {
                                        var prefix = propertyName.charAt(0).toUpperCase() + propertyName.substr(1) + " ";
                                        changeSet.value.name(Designer.getUniqueNameForNamedObjectsArray(self.fakeChart[propertyName](), prefix));
                                    }
                                    self._chartStructureProvider.selectedPath(path + "." + changeSet.index.toString());
                                }
                            }
                        });
                    };
                };
                ChartControlViewModel.prototype.serialize = function () {
                    return (new Designer.DesignerModelSerializer()).serialize(this);
                };
                ChartControlViewModel.prototype.save = function () {
                    var data = this.serialize();
                    if (this.onSave) {
                        this.onSave(data);
                    }
                    return data;
                };
                return ChartControlViewModel;
            })(Designer.ElementViewModel);
            Chart.ChartControlViewModel = ChartControlViewModel;
            Chart.margins = { propertyName: "margins", modelName: "@Margins", from: Designer.Margins.fromString, displayName: "Margins" };
            Chart.pageWidth = { propertyName: "pageWidth", modelName: "@PageWidth", defaultVal: 850, from: Designer.floatFromModel, displayName: "Page Width", editor: Designer.Widgets.editorTemplates.numeric };
            Chart.pageHeight = { propertyName: "pageHeight", modelName: "@PageHeight", defaultVal: 1250, from: Designer.floatFromModel, displayName: "Page Height", editor: Designer.Widgets.editorTemplates.numeric };
            var fakeChartSerializationInfo = [Chart.seriesTemplate, Chart.seriesSerializable, Chart.titles, Chart.legend, Chart.axisX, Chart.axisY];
            Chart.size = { propertyName: "size", modelName: "@SizeF", from: Designer.Size.fromString, displayName: "Size", editor: Designer.Widgets.editorTemplates.objecteditor };
            Chart.chartStructure = { propertyName: "_chartStructure", displayName: "Chart Structure", editor: Designer.Widgets.editorTemplates.treelist };
            Chart.chartElement = { propertyName: "_chartElement", displayName: "Selected Element", editor: Designer.Widgets.editorTemplates.objecteditor };
            Chart.chartControlSerializationsInfo = [Chart.name, Chart.chart, Chart.chartStructure, Chart.chartElement, Chart.size, Chart.margins];
            var ChartControlSurface = (function (_super) {
                __extends(ChartControlSurface, _super);
                function ChartControlSurface(control, zoom) {
                    var _this = this;
                    if (zoom === void 0) { zoom = ko.observable(1); }
                    _super.call(this, control, {
                        measureUnit: ko.observable("Pixels"),
                        zoom: zoom,
                        dpi: ko.observable(100)
                    }, ChartControlSurface._unitProperties);
                    this.imageSrc = ko.observable("");
                    this.allowMultiselect = false;
                    this.focused = ko.observable(false);
                    this.selected = ko.observable(false);
                    this.underCursor = ko.observable(new Designer.HoverInfo());
                    this.templateName = "dx-chart-surface";
                    this.margins = { bottom: this["_bottom"], left: this["_left"], right: this["_right"], top: this["_top"] };
                    this.zoom = zoom;
                    ko.computed(function () {
                        var series = control.chart.dataContainer.series();
                        series.forEach(function (val) {
                            val.view.typeName();
                        });
                        var chartJson = new Designer.DesignerModelSerializer().serialize(control["chart"], Chart.chartSerializationsInfo);
                        var _self = _this;
                        if (Designer.Chart.HandlerUri) {
                            $.post(Designer.Chart.HandlerUri, {
                                actionKey: 'chart',
                                arg: encodeURIComponent(JSON.stringify({
                                    width: control.size.width() * zoom(),
                                    height: control.size.height() * zoom(),
                                    Chart: JSON.stringify({
                                        'ChartXmlSerializer': {
                                            Chart: chartJson
                                        }
                                    })
                                }))
                            }).done(function (jsonResult) {
                                var allSeries = _self._control.chart.dataContainer.series();
                                allSeries.forEach(function (val) {
                                    val.isIncompatible(false);
                                });
                                _self.imageSrc("data:image/x;base64," + jsonResult.result.Image);
                                jsonResult.result.Indexes.forEach(function (val) {
                                    var series = allSeries[val];
                                    series.isIncompatible(true);
                                });
                            }).fail(function (result) {
                                Designer.NotifyAboutWarning("Impossible to get chart image.");
                            });
                        }
                    }).extend({ throttle: 1 });
                }
                Object.defineProperty(ChartControlSurface.prototype, "measureUnit", {
                    get: function () {
                        return this._context.measureUnit;
                    },
                    enumerable: true,
                    configurable: true
                });
                Object.defineProperty(ChartControlSurface.prototype, "dpi", {
                    get: function () {
                        return this._context.dpi;
                    },
                    enumerable: true,
                    configurable: true
                });
                ChartControlSurface.prototype.checkParent = function (surfaceParent) {
                    return false;
                };
                ChartControlSurface.prototype.getChildrenCollection = function () {
                    return ko.observableArray([]);
                };
                ChartControlSurface._unitProperties = {
                    _width: function (o) {
                        return o.size.width;
                    },
                    _height: function (o) {
                        return o.size.height;
                    },
                    height: function (o) {
                        return o.size.height;
                    },
                    width: function (o) {
                        return o.size.width;
                    },
                    _bottom: function (o) {
                        return o.margins.bottom;
                    },
                    _left: function (o) {
                        return o.margins.left;
                    },
                    _right: function (o) {
                        return o.margins.right;
                    },
                    _top: function (o) {
                        return o.margins.top;
                    }
                };
                return ChartControlSurface;
            })(Designer.SurfaceElementBase);
            Chart.ChartControlSurface = ChartControlSurface;
        })(Chart = Designer.Chart || (Designer.Chart = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Chart;
        (function (Chart) {
            Chart.controlsFactory = new Designer.ControlsFactory();
            function registerControls() {
                Chart.controlsFactory.registerControl("ChartControl", {
                    info: Chart.chartControlSerializationsInfo,
                    surfaceType: Chart.ChartControlSurface,
                    type: Chart.ChartControlViewModel,
                    elementActionsTypes: [],
                    isContainer: true,
                    nonToolboxItem: true
                });
            }
            Chart.registerControls = registerControls;
            Chart.HandlerUri = "DXXRD.axd";
            function customizeDesignerActions(designerModel, nextCustomizer) {
                var chart = designerModel.model;
                return (function (actions) {
                    var save = {
                        text: "Save",
                        imageClassName: "dxrd-image-save",
                        disabled: ko.observable(false),
                        visible: true,
                        hasSeparator: false,
                        hotKey: "Ctrl+S",
                        clickAction: function () {
                            chart().save();
                        }
                    };
                    actions.push(save);
                    nextCustomizer && nextCustomizer(actions);
                });
            }
            function ajax(action, arg) {
                var deferred = $.Deferred();
                $.post(Chart.HandlerUri, {
                    actionKey: action,
                    arg: arg
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    Designer.NotifyAboutWarning(errorThrown);
                }).done(function (data) {
                    if (data.success) {
                        deferred.resolve(data.result);
                    }
                    else {
                        Designer.NotifyAboutWarning(data.error);
                    }
                });
                return deferred.promise();
            }
            var fieldListCallback = function (request) {
                var requestJson = JSON.stringify(request);
                var encodedJson = encodeURIComponent(requestJson);
                return ajax('fieldList', encodedJson);
            };
            function updateChartSurfaceContentSize(surfaceSize) {
                return function () {
                    var rightAreaWidth = $(".dxcd-designer .dxrd-right-panel").outerWidth() + $(".dxcd-right-tabs").outerWidth();
                    var leftAreaWidth = $(".dxcd-designer .dx-chart-left-panel").outerWidth();
                    $(".dxrd-surface-wrapper").css("right", rightAreaWidth);
                    $(".dxrd-surface-wrapper").css("left", leftAreaWidth);
                    var otherWidth = rightAreaWidth + leftAreaWidth, surfaceWidth = $(".dxrd-designer").width() - (otherWidth + 5);
                    $(".dxrd-surface-wrapper").css("width", surfaceWidth);
                    surfaceSize(surfaceWidth);
                };
            }
            Chart.updateChartSurfaceContentSize = updateChartSurfaceContentSize;
            function createChartDesigner(element, data, callbacks, localization) {
                if (localization) {
                    Globalize.addCultureInfo("default", {
                        messages: localization
                    });
                }
                callbacks.fieldLists = callbacks.fieldLists || fieldListCallback;
                registerControls();
                var chartControlModel = ko.observable(), surface = ko.observable(), fieldListProvider = ko.observable(), size = new Designer.Size(data.width, data.height);
                var init = function (chartSourceValue) {
                    chartControlModel(new Chart.ChartControlViewModel(data.chartSource(), data.dataSource, size));
                    surface(new Chart.ChartControlSurface(chartControlModel())), fieldListProvider(new Designer.FieldListProvider(callbacks.fieldLists, ko.observableArray([data.dataSource])));
                };
                data.chartSource.subscribe(function (newValue) {
                    init(newValue);
                    designerModel.chartStructure = chartControlModel()._chartStructure;
                    designerModel.selectedElement = chartControlModel()._chartElement;
                });
                init(data.chartSource());
                var designerModel = Designer.createDesigner(chartControlModel, surface, Chart.controlsFactory);
                designerModel.rootStyle = "dxcd-designer";
                designerModel.chartStructure = chartControlModel()._chartStructure;
                designerModel.selectedElement = new Designer.Widgets.ObjectProperties(chartControlModel()._chartElement);
                designerModel.parts = [
                    { templateName: "dx-chart-middlePart", model: designerModel },
                    { templateName: "dxcd-toolbar", model: designerModel },
                    { templateName: "dx-right-panel-lightweight", model: designerModel },
                    { templateName: "dx-chart-leftPanel", model: designerModel }
                ];
                designerModel.tabPanel.tabs[0].template = "dxrd-propertygridtab";
                designerModel.fieldListProvider = fieldListProvider;
                designerModel.actionLists = new Designer.ActionLists(surface, designerModel.selection, designerModel.undoEngine, customizeDesignerActions(designerModel, callbacks.customizeActions));
                designerModel.actionLists.toolbarItems = designerModel.actionLists.toolbarItems.filter(function (item) {
                    return ["Cut", "Copy", "Paste", "Delete"].indexOf(item.text) === -1;
                });
                designerModel.actionLists.toolbarItems[0].hasSeparator = true;
                var lastItem = designerModel.actionLists.toolbarItems.pop();
                designerModel.actionLists.toolbarItems = [].concat(lastItem, designerModel.actionLists.toolbarItems);
                designerModel.isLoading(false);
                designerModel.selection.focused(surface());
                $(element).children().remove();
                ko.applyBindings(designerModel, element);
                var updateSurfaceContentSize_ = updateChartSurfaceContentSize(designerModel.surfaceSize);
                $(window).bind("resize", function () {
                    updateSurfaceContentSize_();
                });
                designerModel.tabPanel.width.subscribe(function () {
                    updateSurfaceContentSize_();
                });
                designerModel.updateSurfaceSize = function () {
                    updateSurfaceContentSize_();
                };
                designerModel.updateSurfaceSize();
                return designerModel;
            }
            Chart.createChartDesigner = createChartDesigner;
        })(Chart = Designer.Chart || (Designer.Chart = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
//# sourceMappingURL=chart-designer.js.map