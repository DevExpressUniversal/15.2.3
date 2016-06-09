(function () {
    var ASPxClientWebChartControl = ASPx.CreateClass(ASPxClientControl, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);
            this.ObjectHotTracked = new ASPxClientEvent();
            this.ObjectSelected = new ASPxClientEvent();
            this.CustomDrawCrosshair = new ASPxClientEvent();
            this.printFrame = null;
            this.exportWindow = null;
            this.printOptions = new ASPxClientChartPrintOptions();
            ASPx.Evt.AttachEventToElement(this.GetMainDOMElement(), "mouseout", function (evt) { this.HideElements(evt); }.aspxBind(this));
        },
        HideElements: function (evt) {
            var target;
            if (ASPx.IsExists(evt.toElement)) {
                target = evt.toElement;
            }
            else {
                if (ASPx.IsExists(evt.relatedTarget)) {
                    target = evt.relatedTarget;
                }
            }
            if (ASPx.IsExists(target)) {
                var parent = ASPx.GetParentById(target, this.name);
                if (ASPx.IsExists(parent)) {
                    return;
                }
            }
            if (ASPx.IsExists(this.crosshairRenderer)) {
                this.crosshairRenderer.Hide();
            }
            if (ASPx.IsExists(this.chart) && ASPx.IsExists(this.chart.toolTipController)) {
                this.chart.toolTipController.Hide();
            }
        },
        CalculateEventX: function (clickedElement, mouseEventX) {
            var left = ASPx.GetAbsoluteX(clickedElement);
            return Math.abs(mouseEventX - left);
        },
        CalculateEventY: function (clickedElement, mouseEventY) {
            var top = ASPx.GetAbsoluteY(clickedElement);
            return Math.abs(mouseEventY - top);
        },
        OnClick: function (evt) {
            var processOnServer = this.IsServerEventAssigned("ObjectSelected");
            var mouseEventX = ASPx.Evt.GetEventX(evt);
            var mouseEventY = ASPx.Evt.GetEventY(evt);
            var htmlElement = ASPx.Evt.GetEventSource(evt);
            if (ASPx.GetIsParent(this.GetMainDOMElement(), htmlElement)) {
                htmlElement = this.GetImageElement();
            }
            var x = this.CalculateEventX(htmlElement, mouseEventX);
            var y = this.CalculateEventY(htmlElement, mouseEventY);
            var showToolTip = ASPx.IsExists(this.chart.toolTipController);
            var raiseObjectSelected = ASPx.IsExists(this.RaiseObjectSelected);
            var hitObjects, hitInfo;
            if (showToolTip || raiseObjectSelected) {
                hitObjects = this.HitTest(x, y);
                if (hitObjects != null) {
                    hitInfo = new ASPxClientWebChartHitInfo(hitObjects);
                }
            }
            if (ASPx.Browser.HardwareAcceleration) {
                x = Math.ceil(x);
                y = Math.ceil(y);
            }
            if (raiseObjectSelected) {
                processOnServer = this.RaiseObjectSelected(x, y, htmlElement, mouseEventX, mouseEventY, hitObjects, hitInfo);
            }
            if (processOnServer || this.chart.selectionMode != "" && (hitInfo.inSeries || hitInfo.inSeriesPoint)) {
                var ctrlPressed = 0;
                var shiftPressed = 0;
                if (parseInt(navigator.appVersion) > 3) {
                    if (document.layers && navigator.appName == "Netscape" && parseInt(navigator.appVersion) == 4) {
                        var mString = (e.modifiers + 32).toString(2).substring(3, 6);
                        shiftPressed = (mString.charAt(0) == "1");
                        ctrlPressed = (mString.charAt(1) == "1");
                    }
                    else {
                        shiftPressed = evt.shiftKey;
                        ctrlPressed = evt.ctrlKey;
                    }
                }
                var eventParams = "SELECT:" + x + ":" + y + ":" + shiftPressed + ":" + ctrlPressed;
                if (this.autoPostBack) {
                    this.SendPostBack(eventParams);
                }
                else if (ASPx.IsExists(this.callBack)) {
                    this.ChartCallback(eventParams);
                }
            }
            if (showToolTip && this.chart.toolTipController.openMode == "OnClick") {
                if (hitObjects == null)
                    return;
                this.chart.toolTipController.Show(mouseEventX, mouseEventY, hitInfo, ASPx.GetAbsoluteX(htmlElement), ASPx.GetAbsoluteY(htmlElement));
            }
            if (this.chart.legend.useCheckBoxes) {
                for (var i = 0; i < hitObjects.length; i++) {
                    if (hitObjects[i].object instanceof ASPxClientLegend) {
                        var legendCheckboxObj = hitObjects[i].additionalObject;
                        if (ASPx.IsExists(legendCheckboxObj)) {
                            var checkableLegendItemID = legendCheckboxObj.legendItemId;
                            var eventParams = "CHANGE_CHECKED_IN_LEGEND:" + checkableLegendItemID;
                            this.ChartCallback(eventParams);
                        }
                    }
                }
            }
        },
        OnMouseMove: function (evt) {
            var raiseObjectHotTracked = ASPx.IsExists(this.RaiseObjectHotTracked);
            var showToolTip = ASPx.IsExists(this.chart) ? ASPx.IsExists(this.chart.toolTipController) : false;
            var showCrosshair = ASPx.IsExists(this.chart) ? this.chart.showCrosshair : false;
            if (raiseObjectHotTracked || showToolTip || showCrosshair) {
                var mouseEventX = ASPx.Evt.GetEventX(evt);
                var mouseEventY = ASPx.Evt.GetEventY(evt);
                var htmlElement = ASPx.Evt.GetEventSource(evt);
                if (ASPx.GetIsParent(this.GetMainDOMElement(), htmlElement))
                    htmlElement = this.GetImageElement();
                var x = this.CalculateEventX(htmlElement, mouseEventX);
                var y = this.CalculateEventY(htmlElement, mouseEventY);
                var hitObjects = this.HitTest(x, y);
                if (hitObjects == null)
                    return;
                var hitInfo = new ASPxClientWebChartHitInfo(hitObjects);
                if (raiseObjectHotTracked)
                    this.RaiseObjectHotTracked(x, y, htmlElement, mouseEventX, mouseEventY, hitObjects, hitInfo);
                if (showCrosshair) {
                    if (!ASPx.IsExists(this.crosshairRenderer))
                        this.crosshairRenderer = new ASPx.CrosshairRenderer(this, this.name + '_CH');
                    this.crosshairRenderer.UpdateCrosshair(x, y, this);
                }
                if (showToolTip) {
                    if (this.chart.toolTipController.openMode == "OnHover")
                        this.chart.toolTipController.Show(mouseEventX, mouseEventY, hitInfo, ASPx.GetAbsoluteX(htmlElement), ASPx.GetAbsoluteY(htmlElement));
                }
            }
        },

        ensurePrintFrame: function () {
            if (this.frameElement != null && ASPx.Browser.IE) {
                this.frameElement.parentNode.removeChild(this.frameElement);
                this.frameElement = null;
            }
            if (this.frameElement == null)
                this.frameElement = this.createFrameElement("DXPrinter");
            this.printFrame = window.frames[this.frameElement.id];
        },
        createFrameElement: function (name) {
            var f = document.createElement("iframe");
            f.frameBorder = "0";
            f.style.overflow = "hidden";
            var frameSize = ASPx.Browser.Safari ? "1px" : "0px";
            var position = ASPx.Browser.Safari ? "0px" : "-100px";
            f.style.width = frameSize;
            f.style.height = frameSize;
            f.name = name;
            f.id = name;
            f.style.position = "absolute";
            f.style.top = position;
            f.style.left = position;
            if (ASPx.Browser.Safari)
                f.style.opacity = 0;
            document.body.appendChild(f);
            return f;
        },
        getFrame: function () {
            this.ensurePrintFrame();
            return this.printFrame;
        },
        CallbackSaveToDisk: function (result) {
            this.getFrame().location = result;
        },
        CallbackSaveToWindow: function (result) {
            if (this.exportWindow != null && !this.exportWindow.closed) {
                this.exportWindow.location = result;
            }
        },
        OnCallback: function (resultObj) {
            var result = resultObj.result;
            if (result) {
                if (ASPx.IsExists(this.chart)) {
                    if (ASPx.IsExists(this.crosshairRenderer)) {
                        this.crosshairRenderer.Hide();
                    }
                    if (ASPx.IsExists(this.chart) && ASPx.IsExists(this.chart.toolTipController)) {
                        this.chart.toolTipController.Hide();
                    }
                }
                var printSign = "PRINT:";
                if (result.indexOf(printSign) >= 0) {
                    this.CallbackSaveToDisk(result.substring(printSign.length));
                    return;
                }
                var saveToDiskSign = "SAVETODISK:";
                if (result.indexOf(saveToDiskSign) >= 0) {
                    this.CallbackSaveToDisk(result.substring(saveToDiskSign.length));
                    return;
                }
                var saveToWindowSign = "SAVETOWINDOW:";
                if (result.indexOf(saveToWindowSign) >= 0) {
                    this.CallbackSaveToWindow(result.substring(saveToWindowSign.length));
                    return;
                }
            }

            if (resultObj.url) {
                var htmlImage = this.GetImageElement();
                if (ASPx.IsExists(htmlImage))
                    htmlImage.setAttribute("src", resultObj.url);
            }
            if (resultObj.hitInfo)
                this.LoadHitInfo(eval(resultObj.hitInfo));
            if (resultObj.objectModel)
                this.InitObjectModel(eval("(" + resultObj.objectModel + ")"));
            this.UpdateStateObjectWithObject(resultObj.stateObject);
        },
        LoadHitInfo: function (hitInfo) {
            this.hitTestController = new ASPx.ChartHitTestController(hitInfo);
        },
        InitObjectModel: function (objectModel) {
            this.chart = new ASPxClientWebChart(this, objectModel);
            this.ResetCrosshairRenderer();
        },
        ResetCrosshairRenderer: function () {
            if (ASPx.IsExists(this.crosshairRenderer)) {
                this.crosshairRenderer.Hide();
                this.crosshairRenderer = null;
            }
        },
        SetOperaCursor: function (cursor, htmlElement) {
            var divId = this.name + "_DIV";
            var div = ASPx.GetElementById(divId);
            if (!ASPx.IsExists(div) || (div.tagName != "DIV") || (div != htmlElement.parentNode))
                div = null;

            oldCursor = div != null ? div.style.cursor : htmlElement.style.cursor;
            if (!ASPx.IsExists(oldCursor))
                oldCursor = "default";

            if (cursor != oldCursor) {
                if (div == null) {
                    div = document.createElement("div");
                    div.id = divId;
                    div.style.backgroundColor = "transparent";
                    div.style.border = "none";
                    div.style.cursor = cursor;
                    htmlElement.parentNode.replaceChild(div, htmlElement);
                    div.appendChild(htmlElement);
                }
                else
                    div.parentNode.replaceChild(htmlElement, div);
            }
        },
        ChartCallback: function (eventParams, command) {
            if (ASPx.IsExists(this.callBack)) {
                this.ResetCrosshairRenderer();
                if (ASPx.IsExists(this.chart) && ASPx.IsExists(this.chart.toolTipController)) {
                    this.chart.toolTipController.Hide();
                }
                this.ShowLoadingElements();
                this.CreateCallback(eventParams, command);
            }
            else
                this.SendPostBack(eventParams);
        },
        ShowLoadingPanel: function () {
            this.CreateLoadingPanelWithAbsolutePosition(this.GetMainElement().parentNode, this.GetMainElement());
        },
        GetCallbackAnimationElement: function () {
            return this.GetMainElement();
        },
        GetImageElement: function () {
            return document.getElementById(this.name + '_IMG');
        }
    });

    ASPx.chartClick = function (evt, name) {
        var control = ASPx.GetControlCollection().Get(name);
        if (control != null)
            control.OnClick(evt);

    }
    ASPx.chartMouseMove = function (evt, name) {
        var control = ASPx.GetControlCollection().Get(name);
        if (control != null)
            control.OnMouseMove(evt);
    }

    // API

    var ASPxClientScaleType = ASPx.CreateClass(null, {});
    ASPxClientScaleType.Qualitative = "Qualitative";
    ASPxClientScaleType.Numerical = "Numerical";
    ASPxClientScaleType.DateTime = "DateTime";

    var ASPxClientControlCoordinatesVisibility = ASPx.CreateClass(null, {});
    ASPxClientControlCoordinatesVisibility.Visible = "Visible";
    ASPxClientControlCoordinatesVisibility.Hidden = "Hidden";
    ASPxClientControlCoordinatesVisibility.Undefined = "Undefined";
    ASPxClientWebChartControl.Cast = ASPxClientControl.Cast;
    ASPxClientWebChartControl.prototype.GetChart = function () {
        return this.chart;
    };
    ASPxClientWebChartControl.prototype.GetPrintOptions = function () {
        return this.printOptions;
    };
    ASPxClientWebChartControl.prototype.SetCursor = function (cursor) {
        var htmlElement = this.GetMainElement();
        if (ASPx.IsExists(htmlElement)) {
            if (ASPx.Browser.Opera)
                this.SetOperaCursor(cursor, htmlElement);
            else
                htmlElement.style.cursor = cursor;
        }
    };
    ASPxClientWebChartControl.prototype.RaiseObjectHotTracked = function (x, y, htmlElement, absoluteX, absoluteY, hitObjects, hitInfo) {
        for (var i = 0; i < hitObjects.length; i++) {
            var args = new ASPxClientWebChartControlHotTrackEventArgs(
                false,
                hitObjects[i].object,
                hitObjects[i].additionalObject,
                hitInfo,
                this.chart,
                htmlElement,
                x, y,
                absoluteX, absoluteY);
            this.ObjectHotTracked.FireEvent(this, args);
            if (!args.cancel)
                break;

        }
    };
    ASPxClientWebChartControl.prototype.RaiseCustomDrawCrosshair = function (CursorCrosshairAxisLabelElements, CursorCrosshairLineElement, CrosshairElementGroups) {
        var args = new ASPxClientWebChartControlCustomDrawCrosshairEventArgs(
            false,
            CursorCrosshairAxisLabelElements,
            CursorCrosshairLineElement,
            CrosshairElementGroups);
        this.CustomDrawCrosshair.FireEvent(this, args);
    };
    ASPxClientWebChartControl.prototype.RaiseObjectSelected = function (x, y, htmlElement, absoluteX, absoluteY, hitObjects, hitInfo) {
        var processOnServer = this.IsServerEventAssigned("ObjectSelected");
        if (hitObjects == null)
            return processOnServer;
        for (var i = 0; i < hitObjects.length; i++) {
            var args = new ASPxClientWebChartControlHotTrackEventArgs(
                processOnServer,
                hitObjects[i].object,
                hitObjects[i].additionalObject,
                hitInfo,
                this.chart,
                htmlElement,
                x, y,
                absoluteX, absoluteY);
            this.ObjectSelected.FireEvent(this, args);
            processOnServer = args.processOnServer;
            if (!args.cancel)
                break;
        }
        return processOnServer;
    };
    ASPxClientWebChartControl.prototype.HitTest = function (x, y) {
        if (ASPx.IsExists(this.hitTestController))
            return this.hitTestController.HitTest(x, y);
        else
            return null;
    };
    ASPxClientWebChartControl.prototype.PerformCallback = function (args) {
        if (!ASPx.IsExists(args)) args = "";
        this.ChartCallback("CUSTOMCALLBACK:" + args, "CUSTOMCALLBACK");
    };
    ASPxClientWebChartControl.prototype.Print = function () {
        if (!ASPx.IsExists(this.callBack)) return;
        var options = this.printOptions.createEventParameters();
        this.ChartCallback("PRINT:" + options);
    };
    ASPxClientWebChartControl.prototype.LoadFromObjectModel = function (serializedChartObjectModel) {
        if (!ASPx.IsExists(this.callBack)) return;
        this.ChartCallback("LOAD_LAYOUT:" + serializedChartObjectModel);
    };
    ASPxClientWebChartControl.prototype.SaveToDisk = function (format, filename) {
        if (!ASPx.IsExists(this.callBack)) return;
        var options = this.printOptions.createEventParameters();
        if (filename == undefined)
            this.ChartCallback("SAVETODISK:" + format + ":" + options + ":");
        else
            this.ChartCallback("SAVETODISK:" + format + ":" + options + ":" + filename);
    };
    ASPxClientWebChartControl.prototype.SaveToWindow = function (format) {
        if (!ASPx.IsExists(this.callBack)) return;
        this.exportWindow = window.open('', '_blank', 'toolbars=no, resizable=yes, scrollbars=yes');
        var options = this.printOptions.createEventParameters();
        this.ChartCallback("SAVETOWINDOW:" + format + ":" + options);
    };
    ASPxClientWebChartControl.prototype.GetMainDOMElement = function () {
        return this.GetMainElement();
    }
    var ASPxClientWebChartControlCustomDrawCrosshairEventArgs = ASPx.CreateClass(ASPxClientProcessingModeEventArgs, {
        constructor: function (processOnServer, cursorCrosshairAxisLabelElements, cursorCrosshairLineElement, crosshairElementGroups) {
            this.constructor.prototype.constructor.call(this, processOnServer);
            this.crosshairElements = [];
            for (var i = 0; i < crosshairElementGroups.length; i++)
                for (var j = 0; j < crosshairElementGroups[i].CrosshairElements.length; j++)
                    this.crosshairElements.push(crosshairElementGroups[i].CrosshairElements[j]);
            this.cursorCrosshairAxisLabelElements = cursorCrosshairAxisLabelElements;
            this.cursorCrosshairLineElement = cursorCrosshairLineElement;
            this.crosshairGroupHeaderElements = [];
            for (var i = 0; i < crosshairElementGroups.length; i++)
                if (crosshairElementGroups[i].HeaderElement != null)
                    this.crosshairGroupHeaderElements.push(crosshairElementGroups[i].HeaderElement);
            this.crosshairElementGroups = crosshairElementGroups;
        }
    });
    var ASPxClientCrosshairDrawInfoList = ASPx.CreateClass(null, {
        constructor: function (crosshairDrawInfos, crosshairGroups) {
            if (crosshairDrawInfos.length < 1)
                return;
            this.CrosshairElementGroups = [];
            if (crosshairGroups.length > 0)
                for (var i = 0; i < crosshairGroups.length; i++) {
                    var headerElement = crosshairGroups[i].crosshairGroupHeaderElement;
                    var elementGroup = new ASPxClientCrosshairElementGroup(headerElement, crosshairGroups[i].crosshairElements);
                    this.CrosshairElementGroups.push(elementGroup);
                }
            else {
                for (var i = 0; i < crosshairDrawInfos.length; i++) {
                    var crosshairDrawInfo = crosshairDrawInfos[i];
                    for (var j = 0; j < crosshairDrawInfo.CrosshairElements.length; j++) {
                        var elements = [];
                        elements.push(crosshairDrawInfo.CrosshairElements[j]);
                        var elementGroup = new ASPxClientCrosshairElementGroup(null, elements);
                        this.CrosshairElementGroups.push(elementGroup);
                    }
                }
            }
            cursorCrosshairLineElement = crosshairDrawInfos[0].CursorCrosshairLineElement;
            for (var i = 0; i < crosshairDrawInfos.length; i++) {
                if (ASPx.IsExists(crosshairDrawInfos[i].CursorCrosshairLineDrawInfo)) {
                    crosshairDrawInfos[i].CursorCrosshairLineElement = cursorCrosshairLineElement;
                    crosshairDrawInfos[i].CursorCrosshairLineDrawInfo = { cursorCrosshairLineElement: cursorCrosshairLineElement, crosshairLine: crosshairDrawInfos[i].CursorCrosshairLineDrawInfo.crosshairLine };
                }
            }
            this.CrosshairDrawInfoList = crosshairDrawInfos;
            this.CursorCrosshairLineElement = cursorCrosshairLineElement;
            this.CursorCrosshairAxisLabelElements = [];
            for (var i = 0; i < this.CrosshairDrawInfoList.length; i++) {
                var crosshairDrawInfo = this.CrosshairDrawInfoList[i];
                for (var j = 0; j < crosshairDrawInfo.CursorCrosshairAxisLabelElements.length; j++) {
                    this.CursorCrosshairAxisLabelElements.push(crosshairDrawInfo.CursorCrosshairAxisLabelElements[j]);
                }
            }
        }
    });
    var ASPxClientCrosshairDrawInfo = ASPx.CreateClass(null, {
        constructor: function (chartControl) {
            this.chartControl = chartControl;
            this.CrosshairElements = [];
            this.CursorCrosshairAxisLabelElements = [];
            this.CursorCrosshairAxisLabelDrawInfos = [];
        },
        AddPaneCrosshairInfo: function (paneCrosshairInfo, rotated) {
            crosshairOptions = this.chartControl.chart.crosshairOptions;
            this.labelsBounds = paneCrosshairInfo.labelsBounds;
            for (var i = 0; i < paneCrosshairInfo.pointsInfo.length; i++) {
                var point = paneCrosshairInfo.pointsInfo[i];
                var crosshairLineElement = new ASPxClientCrosshairLineElement(crosshairOptions, point.crosshairLine, rotated);
                var crosshairAxisLabelElement = this.CreateCrosshairAxisLabelElement(point.crosshairLabel, crosshairOptions);
                var crosshairSeriesLabelElement = new ASPxClientCrosshairSeriesLabelElement(point, crosshairOptions.showCrosshairLabels);
                var crosshairElement = new ASPxClientCrosshairElement(point, crosshairLineElement, crosshairAxisLabelElement, crosshairSeriesLabelElement);
                this.CrosshairElements.push(crosshairElement);
            }
            if (ASPx.IsExists(paneCrosshairInfo.cursorLine)) {
                this.CursorCrosshairLineElement = new ASPxClientCrosshairLineElement(crosshairOptions, paneCrosshairInfo.cursorLine, rotated);
                this.CursorCrosshairLineDrawInfo = { cursorCrosshairLineElement: this.CursorCrosshairLineElement, crosshairLine: paneCrosshairInfo.cursorLine };
            }
            for (var i = 0; i < paneCrosshairInfo.cursorLabel.length; i++) {
                var crosshairAxisLabelElement = this.CreateCrosshairAxisLabelElement(paneCrosshairInfo.cursorLabel[i], crosshairOptions);
                this.CursorCrosshairAxisLabelElements.push(crosshairAxisLabelElement);
                this.CursorCrosshairAxisLabelDrawInfos.push({ cursorCrosshairAxisLabelElement: crosshairAxisLabelElement, crosshairAxisLabel: paneCrosshairInfo.cursorLabel[i] });
            }
        },
        CreateCrosshairAxisLabelElement: function (crosshairLabel, crosshairOptions) {
            var option;
            if (crosshairLabel.isHorizontal) {
                option = this.chartControl.chart.diagram.axisY.crosshairAxisLabelOptions;
            }
            else {
                option = this.chartControl.chart.diagram.axisX.crosshairAxisLabelOptions;
            }
            return new ASPxClientCrosshairAxisLabelElement(option, crosshairLabel, this.chartControl.chart.diagram.rotated);
        }
    });
    var ASPxClientCrosshairElement = ASPx.CreateClass(null, {
        constructor: function (point, crosshairLineElement, crosshairAxisLabelElement, crosshairSeriesLabelElement) {
            this.Series = point.point.series;
            this.Point = point;
            this.LineElement = crosshairLineElement;
            this.AxisLabelElement = crosshairAxisLabelElement;
            this.LabelElement = crosshairSeriesLabelElement;
            this.visible = true;
        }
    });
    var ASPxClientCrosshairLineElement = ASPx.CreateClass(null, {
        constructor: function (crosshairOptions, crosshairLine, rotated) {
            if (crosshairLine.isHorizontal ^ rotated) {
                this.visible = crosshairOptions.showValueLine;
                this.color = crosshairOptions.valueLineColor;
                this.lineStyle = crosshairOptions.valueLineStyle;
            }
            else {
                this.visible = crosshairOptions.showArgumentLine;
                this.color = crosshairOptions.argumentLineColor;
                this.lineStyle = crosshairOptions.argumentLineStyle;
            }
        }
    });
    var ASPxClientCrosshairAxisLabelElement = ASPx.CreateClass(null, {
        constructor: function (crosshairAxisLabelOptions, crosshairLabel, rotated) {
            this.backColor = crosshairAxisLabelOptions.backColor;
            this.textColor = crosshairAxisLabelOptions.textColor;
            var axisValuePair = crosshairLabel.axisValuePair;
            var value = axisValuePair.axis.GetNativeArgument(axisValuePair.internalValue);
            var isValueAxis = crosshairLabel.isHorizontal ? !rotated : rotated;
            this.text = ASPx.ToolTipPatternHelper.GetAxisLabelText(axisValuePair.axis, isValueAxis, value);
            this.font = crosshairAxisLabelOptions.font;
            this.visible = crosshairAxisLabelOptions.visibility;
        }
    });
    var ASPxClientCrosshairGroupHeaderElement = ASPx.CreateClass(null, {
        constructor: function (group) {
            this.seriesPoints = [];
            this.text = group.headerText;
            this.font = { fontSize: 12, fontFamily: "Tahoma" };
            this.visible = true;
            this.textColor = "#000000";
        }
    });
    var ASPxClientCrosshairSeriesLabelElement = ASPx.CreateClass(null, {
        constructor: function (point, visible) {
            var color = "#" + (ASPx.IsExists(point.point.color) ? point.point.color : point.point.series.color);
            this.defaultMarker = "<div style='width:15px; height:12px; background-color:" + color + "' />"
            this.marker = this.defaultMarker;
            this.text = ASPx.ToolTipPatternHelper.GetPointToolTipText(point.point.series.crosshairLabelPattern, point.point, point.point.series);
            this.headerText = "";
            this.footerText = "";
            this.textColor = "#000000";
            this.font = { fontSize: 12, fontFamily: "Tahoma" };
            this.visible = point.point.series.actualCrosshairLabelVisibility;
            this.markerVisible = true;
            this.markerColor = color;
            this.textVisible = true;
        }
    });
    var ASPxClientCrosshairElementGroup = ASPx.CreateClass(null, {
        constructor: function (headerElement, crosshairElements) {
            this.HeaderElement = headerElement;
            this.CrosshairElements = crosshairElements;
        }
    });
    var ASPxClientWebChartControlHotTrackEventArgs = ASPx.CreateClass(ASPxClientProcessingModeEventArgs, {
        constructor: function (processOnServer, hitObject, additionalHitObject, hitInfo, chart, htmlElement, x, y, absoluteX, absoluteY) {
            this.constructor.prototype.constructor.call(this, processOnServer);
            this.hitObject = hitObject;
            this.additionalHitObject = additionalHitObject;
            this.hitInfo = hitInfo;
            this.chart = chart;
            this.htmlElement = htmlElement;
            this.x = x;
            this.y = y;
            if (typeof (absoluteX) != "undefined")
                this.absoluteX = absoluteX;
            else
                this.absoluteX = 0;
            if (typeof (absoluteY) != "undefined")
                this.absoluteY = absoluteY;
            else
                this.absoluteY = 0;
            this.cancel = false;
        }
    });
    var ASPxClientHitObject = ASPx.CreateClass(null, {
        constructor: function (object, additionalObject) {
            this.object = object;
            this.additionalObject = additionalObject;
        }
    });
    var ASPxClientWebChartHitInfo = ASPx.CreateClass(null, {
        constructor: function (hitObjects) {
            this.inChart = false;
            this.inChartTitle = false;
            this.inAxis = false;
            this.inAxisLabelItem = false;
            this.inAxisTitle = false;
            this.inConstantLine = false;
            this.inDiagram = false;
            this.inNonDefaultPane = false;
            this.inLegend = false;
            this.inSeries = false;
            this.inSeriesLabel = false;
            this.inSeriesPoint = false;
            this.inSeriesTitle = false;
            this.inTrendLine = false;
            this.inFibonacciIndicator = false;
            this.inRegressionLine = false;
            this.inIndicator = false;
            this.inAnnotation = false;
            this.inHyperlink = false;
            this.chart = null;
            this.chartTitle = null;
            this.axis = null;
            this.constantLine = null;
            this.diagram = null;
            this.nonDefaultPane = null;
            this.legend = null;
            this.series = null;
            this.seriesLabel = null;
            this.seriesTitle = null;
            this.trendLine = null;
            this.fibonacciIndicator = null;
            this.regressionLine = null;
            this.indicator = null;
            this.annotation = null;
            this.seriesPoint = null;
            this.axisLabelItem = null;
            this.axisTitle = null;
            this.hyperlink = null;

            for (var i = 0; i < hitObjects.length; i++) {
                var obj = hitObjects[i].object;
                this.ProcessPrimaryObject(obj);
                var additionalObj = hitObjects[i].additionalObject;
                if (additionalObj != null)
                    this.ProcessAdditionalObject(obj, additionalObj);
            }
        },
        ProcessPrimaryObject: function (obj) {
            if (obj instanceof ASPxClientWebChart) {
                if (!this.inChart) {
                    this.inChart = true;
                    this.chart = obj;
                }
            }
            else if (obj instanceof ASPxClientChartTitle) {
                if (!this.inChartTitle) {
                    this.inChartTitle = true;
                    this.chartTitle = obj;
                }
            }
            else if ((obj instanceof ASPxClientAxis) || (obj instanceof ASPxClientSwiftPlotDiagramAxis) ||
                     (obj instanceof ASPxClientAxis3D) || (obj instanceof ASPxClientRadarAxis)) {
                if (!this.inAxis) {
                    this.inAxis = true;
                    this.axis = obj;
                }
            }
            else if (obj instanceof ASPxClientConstantLine) {
                if (!this.inConstantLine) {
                    this.inConstantLine = true;
                    this.constantLine = obj;
                }
            }
            else if ((obj instanceof ASPxClientXYDiagram) || (obj instanceof ASPxClientSwiftPlotDiagram) ||
                     (obj instanceof ASPxClientXYDiagram3D) || (obj instanceof ASPxClientRadarDiagram)) {
                if (!this.inDiagram) {
                    this.inDiagram = true;
                    this.diagram = obj;
                }
            }
            else if (obj instanceof ASPxClientXYDiagramPane) {
                if (!this.inNonDefaultPane) {
                    this.inNonDefaultPane = true;
                    this.nonDefaultPane = obj;
                }
            }
            else if (obj instanceof ASPxClientLegend) {
                if (!this.inLegend) {
                    this.inLegend = true;
                    this.legend = obj;
                }
            }
            else if (obj instanceof ASPxClientSeries) {
                if (!this.inSeries) {
                    this.inSeries = true;
                    this.series = obj;
                }
            }
            else if (obj instanceof ASPxClientSeriesLabel) {
                if (!this.inSeriesLabel) {
                    this.inSeriesLabel = true;
                    this.seriesLabel = obj;
                }
            }
            else if (obj instanceof ASPxClientSeriesTitle) {
                if (!this.inSeriesTitle) {
                    this.inSeriesTitle = true;
                    this.seriesTitle = obj;
                }
            }
            else if (obj instanceof ASPxClientIndicator) {
                this.inIndicator = true;
                this.indicator = obj;
                if (obj instanceof ASPxClientTrendLine) {
                    if (!this.inTrendLine) {
                        this.inTrendLine = true;
                        this.trendLine = obj;
                    }
                }
                else if (obj instanceof ASPxClientFibonacciIndicator) {
                    if (!this.inFibonacciIndicator) {
                        this.inFibonacciIndicator = true;
                        this.fibonacciIndicator = obj;
                    }
                }
                else if (obj instanceof ASPxClientRegressionLine) {
                    if (!this.inRegressionLine) {
                        this.inRegressionLine = true;
                        this.regressionLine = obj;
                    }
                }
            }
            else if (obj instanceof ASPxClientAnnotation) {
                if (!this.inAnnotation) {
                    this.inAnnotation = true;
                    this.annotation = obj;
                }
            }
        },
        ProcessAdditionalObject: function (obj, additionalObj) {
            if (obj instanceof ASPxClientLegend) {
                if (additionalObj instanceof ASPxClientSeriesPoint) {
                    if (!this.inSeriesPoint)
                        this.seriesPoint = additionalObj;
                }
                else if (additionalObj instanceof ASPxClientConstantLine) {
                    if (!this.inConstantLine)
                        this.constantLine = additionalObj;
                }
                else if (additionalObj instanceof ASPxClientSeries) {
                    if (!this.inSeries)
                        this.series = additionalObj;
                }
                else if (additionalObj instanceof ASPxClientIndicator) {
                    if (!this.inIndicator)
                        this.indicator = additionalObj;
                }
            }
            else {
                if (additionalObj instanceof ASPxClientSeriesPoint) {
                    if (!this.inSeriesPoint) {
                        this.inSeriesPoint = true;
                        this.seriesPoint = additionalObj;
                    }
                }
                if (additionalObj instanceof ASPxClientAxisLabelItem) {
                    if (!this.inAxisLabelItem) {
                        this.inAxisLabelItem = true;
                        this.axisLabelItem = additionalObj;
                    }
                }
                if (additionalObj instanceof ASPxClientAxisTitle) {
                    if (!this.inAxisTitle) {
                        this.inAxisTitle = true;
                        this.axisTitle = additionalObj;
                    }
                }
            }
            if (additionalObj instanceof ASPx.HyperlinkSource) {
                this.inHyperlink = true;
                this.hyperlink = additionalObj.hyperlink;
            }
        }
    });
    var ASPxClientDiagramCoordinates = ASPx.CreateClass(null, {
        constructor: function () {
            this.argumentScaleType = '';
            this.valueScaleType = '';
            this.qualitativeArgument = '';
            this.numericalArgument = 0;
            this.dateTimeArgument = null;
            this.numericalValue = 0;
            this.dateTimeValue = null;
            this.axisX = null;
            this.axisY = null;
            this.pane = null;

            this.axisValueList = [];
        },
        SetAxisValue: function (axis, valueInternal) {
            var value = axis.GetNativeArgument(valueInternal);
            var axisValue = new ASPxClientAxisValue();
            axisValue.axis = axis;
            if (typeof (value) == 'string') {
                axisValue.qualitativeValue = value;
                axisValue.scaleType = ASPxClientScaleType.Qualitative;
            }
            else if (value instanceof Date) {
                axisValue.dateTimeValue = value;
                axisValue.scaleType = ASPxClientScaleType.DateTime;
            }
            else {
                axisValue.numericalValue = value;
                axisValue.scaleType = ASPxClientScaleType.Numerical;
            }
            this.axisValueList.push(axisValue);
        },
        SetArgumentAndValue: function (axisX, axisY, argumentInternal, valueInternal) {
            var argument = axisX.GetNativeArgument(argumentInternal);
            var value = axisY.GetNativeArgument(valueInternal);
            if (typeof (argument) == 'string') {
                this.qualitativeArgument = argument;
                this.argumentScaleType = ASPxClientScaleType.Qualitative;
            }
            else if (argument instanceof Date) {
                this.dateTimeArgument = argument;
                this.argumentScaleType = ASPxClientScaleType.DateTime;
            }
            else {
                this.numericalArgument = argumentInternal;
                this.argumentScaleType = ASPxClientScaleType.Numerical;
            }
            if (value instanceof Date) {
                this.dateTimeValue = value;
                this.valueScaleType = ASPxClientScaleType.DateTime;
            }
            else {
                this.numericalValue = valueInternal;
                this.valueScaleType = ASPxClientScaleType.Numerical;
            }
            this.SetAxisValue(axisX, argumentInternal);
            this.SetAxisValue(axisY, valueInternal);
        },
        IsEmpty: function () {
            return this.argumentScaleType == '';
        },
        GetAxisValue: function (axis) {
            for (var i = 0; i < this.axisValueList.length; i++)
                if (axis == this.axisValueList[i].axis)
                    return this.axisValueList[i];
            return null;
        }
    });
    var ASPxClientAxisValue = ASPx.CreateClass(null, {
        constructor: function () {
            this.scaleType = '';
            this.qualitativeValue = '';
            this.numericalValue = 0;
            this.dateTimeValue = null;

            this.axis = null;
        }
    });
    var ASPxClientControlCoordinates = ASPx.CreateClass(null, {
        constructor: function () {
            this.pane = null;
            this.x = 0;
            this.y = 0;
            this.visibility = ASPxClientControlCoordinatesVisibility.Undefined;
        },
        GetValueSign: function (value) {
            if (value < 0)
                return -1;
            if (value > 0)
                return 1;
            return 0;
        },
        StrongRound: function (value) {
            return this.GetValueSign(value) * Math.floor(Math.abs(value) + 0.5);
        },
        SetPoint: function (x, y) {
            this.x = this.StrongRound(x);
            this.y = this.StrongRound(y);
        }
    });

    var ASPxClientLegendCheckBox = ASPx.CreateClass(null, {
        constructor: function (legendItemId) {
            this.legendItemId = legendItemId;
        }
    });
    var ASPxClientWebChartElement = ASPx.CreateClass(null, {
        constructor: function (chart, interimObject) {
            this.chart = chart;
            if (ASPx.IsExists(interimObject)) {
                this.InitializeProperties(interimObject);
                this.InitializeHitObjects(interimObject);
            }
            else
                this.InitializeDefault();
        },
        InitializeProperties: function (interimObject) {
            throw "ASPxClientWebChartElement abstract error";
        },
        InitializeDefault: function () {
            throw "ASPxClientWebChartElement abstract error";
        },
        InitializeHitObjects: function (interimObject) {
            var chartControl = this.chart != null ? this.chart.chartControl : this.chartControl;
            if (ASPx.IsExists(chartControl)) {
                if (ASPx.IsExists(chartControl.hitTestController)) {
                    var hitTestController = chartControl.hitTestController;
                    if (ASPx.IsExists(interimObject.hi))
                        hitTestController.objects[interimObject.hi] = this;
                    if (ASPx.IsExists(interimObject.hia))
                        hitTestController.additionalObjects[interimObject.hia] = this;
                }
            }
        },
        CreateArray: function (interimArray, createArrayItem) {
            if (!ASPx.IsExists(interimArray))
                return [];
            if (!(interimArray instanceof Array))
                throw ASPxClientWebChartElement.objectModelError;

            var result = [];
            for (var i = 0; i < interimArray.length; i++)
                result.push(createArrayItem(this.chart, this, interimArray[i]));
            return result;
        }
    });
    ASPxClientWebChartElement.objectModelError = "Client object model error";
    var ASPxClientWebChartEmptyElement = ASPx.CreateClass(ASPxClientWebChartElement, {
        constructor: function (chart, interimObject) {
            this.constructor.prototype.constructor.call(this, chart, interimObject);
        },
        InitializeProperties: function (interimObject) {
        },
        InitializeDefault: function () {
        }
    });
    var ASPxClientWebChartRequiredElement = ASPx.CreateClass(ASPxClientWebChartElement, {
        constructor: function (chart, interimObject) {
            this.constructor.prototype.constructor.call(this, chart, interimObject);
        },
        InitializeDefault: function () {
            throw ASPxClientWebChartElement.objectModelError;
        }
    });
    var ASPxClientWebChartElementNamed = ASPx.CreateClass(ASPxClientWebChartRequiredElement, {
        constructor: function (chart, interimObject) {
            this.constructor.prototype.constructor.call(this, chart, interimObject);
        },
        InitializeProperties: function (interimObject) {
            if (!ASPx.IsExists(interimObject.n))
                throw ASPxClientWebChartElement.objectModelError;
            this.name = interimObject.n;
        }
    });
    var ASPxClientWebChart = ASPx.CreateClass(ASPxClientWebChartRequiredElement, {
        constructor: function (chartControl, interimChart) {
            this.chartControl = chartControl;
            this.constructor.prototype.constructor.call(this, null, interimChart);
        },
        InitializeProperties: function (interimObject) {
            this.diagram =
                ASPx.IsExists(interimObject.d) ?
                this.CreateXYDiagram(interimObject.d) :
                new ASPxClientSimpleDiagram(this);
            this.series = this.CreateSeriesArray(interimObject.s);
            this.titles = this.CreateTitles(interimObject.ti);
            this.annotations = this.CreateAnnotations(interimObject.a);
            this.legend = new ASPxClientLegend(this, interimObject.l);
            this.appearanceName = interimObject.an;
            this.paletteName = interimObject.pn;
            this.showSeriesToolTip = ASPx.IsExists(interimObject.sst) ? interimObject.sst : false;
            this.showPointToolTip = ASPx.IsExists(interimObject.spt) ? interimObject.spt : false;
            this.showCrosshair = ASPx.IsExists(interimObject.sc) ? interimObject.sc : false;
            this.toolTipPosition = this.CreateToolTipPosition(this, interimObject.ttp);
            this.toolTipController = this.CreateToolTipController(this, interimObject.ttc);
            this.crosshairOptions = this.CreateCrosshairOptions(this, interimObject.co);
            this.cssPostfix = ASPx.IsExists(interimObject.css) ? interimObject.css : "";
            this.selectionMode = ASPx.IsExists(interimObject.sm) ? interimObject.sm : "";
        },
        CreateXYDiagram: function (interimXYDiagram) {
            if (interimXYDiagram.t == "XYD")
                return new ASPxClientXYDiagram(this, interimXYDiagram);
            else if (interimXYDiagram.t == "SPD")
                return new ASPxClientSwiftPlotDiagram(this, interimXYDiagram);
            else if (interimXYDiagram.t == "XYD3")
                return new ASPxClientXYDiagram3D(this, interimXYDiagram);
            else if (interimXYDiagram.t == "RD")
                return new ASPxClientRadarDiagram(this, interimXYDiagram);
            else
                throw ASPxClientWebChartElement.objectModelError;
        },
        CreateSeriesArray: function (interimSeriesArray) {
            return this.CreateArray(interimSeriesArray, function (nullChart, chart, intermSeries) {
                return new ASPxClientSeries(chart, intermSeries);
            });
        },
        CreateTitles: function (interimTitles) {
            return this.CreateArray(interimTitles, function (nullChart, chart, interimTitle) {
                return new ASPxClientChartTitle(chart, interimTitle);
            });
        },
        CreateAnnotations: function (interimAnnotations) {
            return this.CreateArray(interimAnnotations, function (nullChart, chart, interimAnnotation) {
                if (interimAnnotation.t == "TA")
                    return new ASPxClientTextAnnotation(chart, interimAnnotation);
                else if (interimAnnotation.t == "IA")
                    return new ASPxClientImageAnnotation(chart, interimAnnotation);
                else
                    throw ASPxClientWebChartElement.objectModelError;
            });
        },
        CreateToolTipPosition: function (chart, interimPosition) {
            if (ASPx.IsExists(interimPosition) && (chart.showSeriesToolTip || chart.showPointToolTip)) {
                if (interimPosition.t == "FP")
                    return new ASPxClientToolTipFreePosition(interimPosition);
                else if (interimPosition.t == "RP")
                    return new ASPxClientToolTipRelativePosition(interimPosition);
                else if (interimPosition.t == "MP")
                    return new ASPxClientToolTipMousePosition(interimPosition);
            }
            return null;
        },
        CreateToolTipController: function (chart, interimToolTipController) {
            if (ASPx.IsExists(interimToolTipController) && (chart.showSeriesToolTip || chart.showPointToolTip))
                return new ASPxClientToolTipController(chart, interimToolTipController);
            return null;
        },
        CreateCrosshairOptions: function (chart, interimCrosshairOptions) {
            if (chart.showCrosshair)
                return new ASPxClientCrosshairOptions(chart, interimCrosshairOptions);
            return null;
        }
    });
    var ASPxClientSimpleDiagram = ASPx.CreateClass(ASPxClientWebChartEmptyElement, {
        constructor: function (chart) {
            this.constructor.prototype.constructor.call(this, chart);
        }
    });
    var ASPxClientXYDiagramBase = ASPx.CreateClass(ASPxClientWebChartRequiredElement, {
        constructor: function (chart, interimXYDiagram) {
            this.constructor.prototype.constructor.call(this, chart, interimXYDiagram);
        },
        InitializeProperties: function (interimObject) {
            this.axisX = this.CreateAxis(interimObject.x);
            this.axisY = this.CreateAxis(interimObject.y);
        },
        CreateAxis: function (interimAxis) {
            throw "ASPxClientXYDiagramBase abstract error";
        }
    });
    var ASPxClientXYDiagram2D = ASPx.CreateClass(ASPxClientXYDiagramBase, {
        constructor: function (chart, interimXYDiagram) {
            this.constructor.prototype.constructor.call(this, chart, interimXYDiagram);
        },
        InitializeProperties: function (interimObject) {
            ASPxClientXYDiagramBase.prototype.InitializeProperties.call(this, interimObject);
            this.secondaryAxesX = this.CreateSecondaryAxes(interimObject.sx);
            this.secondaryAxesY = this.CreateSecondaryAxes(interimObject.sy);
            this.defaultPane = new ASPxClientXYDiagramPane(this.chart, this, interimObject.dp);
            this.panes = this.CreatePanes(interimObject.pa);
            if (ASPx.IsExists(interimObject.pld))
                this.paneLayoutDirection = interimObject.pld;
        },
        CreateSecondaryAxes: function (interimSecondaryAxes) {
            throw "ASPxClientXYDiagramBase abstract error";
        },
        CreatePanes: function (interimPanes) {
            return this.CreateArray(interimPanes, function (chart, diagram, interimPane) {
                return new ASPxClientXYDiagramPane(chart, diagram, interimPane);
            });
        },
        FindPaneByXY: function (x, y) {
            if (this.defaultPane.InPane(x, y))
                return this.defaultPane;
            for (var i = 0; i < this.panes.length; i++) {
                if (this.panes[i].InPane(x, y))
                    return this.panes[i];
            }
            return null;
        },
        FindPaneByID: function (paneID) {
            if (paneID == -1)
                return this.defaultPane;
            if (this.panes != null) {
                for (var pane in this.panes) {
                    if (this.panes[pane].paneID == paneID)
                        return this.panes[pane];
                }
            }
            return null;
        },
        FindAxisXByID: function (axisID) {
            if (this.axisX.axisID == axisID)
                return this.axisX;
            for (var i = 0; i < this.secondaryAxesX.length; i++) {
                if (this.secondaryAxesX[i].axisID == axisID)
                    return this.secondaryAxesX[i];
            }
            return null;
        },
        FindAxisYByID: function (axisID) {
            if (this.axisY.axisID == axisID)
                return this.axisY;
            for (var i = 0; i < this.secondaryAxesY.length; i++) {
                if (this.secondaryAxesY[i].axisID == axisID)
                    return this.secondaryAxesY[i];
            }
            return null;
        },
        CalculateAxisValues: function (pane, axis, location, length) {
            if (axis == null)
                return null;
            if (axis.reverse)
                location = length - location;
            var cacheItem = axis.intervalBoundsCaches.GetIntervalBoundsCacheItemByPaneID(pane.paneID);
            if (cacheItem == null)
                return null;
            var index = cacheItem.GetIntervalBoundsIndexByLocation(location);
            if (index < 0 || index >= axis.intervals.length)
                return null;
            var intervalBounds = cacheItem.intervalBoundsArray[index];
            var ratio = (location - intervalBounds.position) / intervalBounds.length;
            var interval = axis.intervals[index];
            var valueInternal = interval.GetInternalValue(ratio);
            if (ASPx.IsExists(axis.scale) && ASPx.IsExists(axis.scale.logarithmic))
                if (axis.scale.logarithmic)
                    valueInternal = this.GetNativeValueForLogarithmic(ratio, interval.minLimit, interval.maxLimit, axis.scale);
            return valueInternal;
        },
        GetNativeValueForLogarithmic: function (ratio, minLimit, maxLimit, scale) {
            var logMinLimit = scale.TransformForward(minLimit);
            var logMaxLimit = scale.TransformForward(maxLimit);
            var logValue = (logMaxLimit - logMinLimit) * ratio + logMinLimit;
            return scale.TransformBackward(logValue);
        },
        GetNativeFromLogarithmicValue: function (value, minLimit, maxLimit, logarithmicBase) {
            var nativeValue = 0;
            var logMinLimit = minLimit == 0 ? 0 : Math.log(Math.abs(minLimit)) / Math.log(logarithmicBase)
            var logMaxLimit = maxLimit == 0 ? 0 : Math.log(Math.abs(maxLimit)) / Math.log(logarithmicBase)
            var logValue = Math.log(Math.abs(value)) / Math.log(logarithmicBase)
            nativeValue = maxLimit * (logValue - logMinLimit) / (logMaxLimit - logMinLimit);
            return nativeValue;
        },
        AddAxisValueToAxisValueList: function (axisValues, pane, axis, location, length) {
            valueInternal = this.CalculateAxisValues(pane, axis, location, length);
            if (valueInternal != null)
                axisValues.push(new ASPx.AxisValuePair(axis, valueInternal, location));
        },
        MapPointToInternal: function (pane, x, y) {
            var axisValues = [];
            var axesCoords = this.GetAxesCoordinates(pane, x, y);
            if (axesCoords == null)
                return axisValues;
            this.AddAxisValueToAxisValueList(axisValues, pane, this.FindAxisXByID(pane.primaryAxisXID), axesCoords.xLocation, axesCoords.xLength);
            this.AddAxisValueToAxisValueList(axisValues, pane, this.FindAxisYByID(pane.primaryAxisYID), axesCoords.yLocation, axesCoords.yLength);
            for (var i = 0; i < this.secondaryAxesX.length; i++)
                this.AddAxisValueToAxisValueList(axisValues, pane, this.secondaryAxesX[i], axesCoords.xLocation, axesCoords.xLength);
            for (var i = 0; i < this.secondaryAxesY.length; i++)
                this.AddAxisValueToAxisValueList(axisValues, pane, this.secondaryAxesY[i], axesCoords.yLocation, axesCoords.yLength);
            return axisValues;
        },
        AddAxisValue: function (coordinates, pane, axis, location, length) {
            var valueInternal = this.CalculateAxisValues(pane, axis, location, length);
            if (valueInternal != null)
                coordinates.SetAxisValue(axis, valueInternal);
        },
        GetAxesCoordinates: function (pane, x, y) {
            x -= pane.boundsLeft;
            y -= pane.boundsTop;
            if (x < 0 || x > pane.boundsWidth || y < 0 || y > pane.boundsHeight)
                return null;
            y = pane.boundsHeight - y;
            if (this.rotated)
                return { xLocation: y, yLocation: x, xLength: pane.boundsHeight, yLength: pane.boundsWidth };
            else
                return { xLocation: x, yLocation: y, xLength: pane.boundsWidth, yLength: pane.boundsHeight };
        },
        PointToDiagram: function (x, y) {
            var coordinates = new ASPxClientDiagramCoordinates();
            var pane = this.FindPaneByXY(x, y);
            if (pane == null)
                return coordinates;
            coordinates.pane = pane;
            var axesCoords = this.GetAxesCoordinates(pane, x, y);
            if (axesCoords == null)
                return coordinates;
            coordinates.axisX = this.FindAxisXByID(pane.primaryAxisXID);
            coordinates.axisY = this.FindAxisYByID(pane.primaryAxisYID);
            var argumentInternal = this.CalculateAxisValues(pane, coordinates.axisX, axesCoords.xLocation, axesCoords.xLength);
            var valueInternal = this.CalculateAxisValues(pane, coordinates.axisY, axesCoords.yLocation, axesCoords.yLength);
            if (argumentInternal != null && valueInternal != null)
                coordinates.SetArgumentAndValue(coordinates.axisX, coordinates.axisY, argumentInternal, valueInternal);
            for (var i = 0; i < this.secondaryAxesX.length; i++)
                this.AddAxisValue(coordinates, pane, this.secondaryAxesX[i], axesCoords.xLocation, axesCoords.xLength);
            for (var i = 0; i < this.secondaryAxesY.length; i++)
                this.AddAxisValue(coordinates, pane, this.secondaryAxesY[i], axesCoords.yLocation, axesCoords.yLength);
            return coordinates;
        },
        CalcIntervalIndexes: function (value, intervals, scale) {
            if (scale.logarithmic)
                value = scale.TransformBackward(value);
            var resultIndexes = [];
            var aboveIndex = -1;
            var belowIndex = -1;
            for (var i = 0; i < intervals.length; i++) {
                if (intervals[i].minLimit <= value && value <= intervals[i].maxLimit) {
                    resultIndexes.push(i);
                    return { indexes: resultIndexes, inRange: true };
                }
                if (value > intervals[i].maxLimit) {
                    if (belowIndex == -1)
                        belowIndex = i;
                    else if (intervals[belowIndex].maxLimit < intervals[i].maxLimit)
                        belowIndex = i;
                }
                if (value < intervals[i].minLimit) {
                    if (aboveIndex == -1)
                        aboveIndex = i;
                    else if (intervals[aboveIndex].minLimit > intervals[i].minLimit)
                        aboveIndex = i;
                }
            }
            if (belowIndex != -1)
                resultIndexes.push(belowIndex);
            if (aboveIndex != -1)
                resultIndexes.push(aboveIndex);
            return { indexes: resultIndexes, inRange: false };
        },
        CalcCoordWithinAndOutRange: function (value, intervals, cacheItem, index, scale) {
            var min = intervals[index].minLimit;
            var max = intervals[index].maxLimit;

            if (scale.logarithmic) {
                min = scale.TransformForward(min);
                max = scale.TransformForward(max);
            }

            var factor = max - min != 0 ? (value - min) / (max - min) : 0;
            var intervalBounds = cacheItem.intervalBoundsArray[index];
            return factor * intervalBounds.length + intervalBounds.position;
        },
        CalcCoordInScaleBreak: function (value, intervals, cacheItem, index1, index2) {
            var min = intervals[index1].maxLimit;
            var max = intervals[index2].minLimit;
            var factor = max - min != 0 ? (value - min) / (max - min) : 0;
            var intervalBoundsMin = cacheItem.intervalBoundsArray[index1];
            var intervalBoundsMax = cacheItem.intervalBoundsArray[index2];
            return factor * (intervalBoundsMax.position - intervalBoundsMin.highBound) + intervalBoundsMin.highBound;
        },
        CalcCoord: function (value, indexes, axis, cacheItem, length) {
            var coord;
            if (indexes.length == 1)
                coord = this.CalcCoordWithinAndOutRange(value, axis.intervals, cacheItem, indexes[0], axis.scale);
            else
                coord = this.CalcCoordInScaleBreak(value, axis.intervals, cacheItem, indexes[0], indexes[1]);
            if (axis.reverse)
                coord = length - coord;
            return coord;
        },
        CheckIndexes: function (indexes, intervals, cacheItem) {
            if (indexes.length < 1 || indexes.length > 2)
                return false;
            for (var i = 0; i < indexes.length; i++) {
                if (indexes[i] < 0 || indexes[i] >= intervals.length || indexes[i] >= cacheItem.intervalBoundsArray.length)
                    return false;
            }
            return true;
        },
        IsAxisValueVisible: function (pane, axis, value) {

            if (axis.scale.logarithmic)
                value = axis.scale.TransformForward(value);

            var cacheItem = axis.intervalBoundsCaches.GetIntervalBoundsCacheItemByPaneID(pane.paneID);
            if (cacheItem == null)
                return false;
            var result = this.CalcIntervalIndexes(value, axis.intervals, axis.scale);
            if (!result.inRange)
                return false;
            var indexes = result.indexes;
            if (!this.CheckIndexes(indexes, axis.intervals, cacheItem))
                return false;
            return true;
        },
        MapInternalToPoint: function (pane, axis, value) {
            var isHorizontal = axis.isArgumentAxis ^ this.rotated;
            if (axis.scale.logarithmic)
                value = axis.scale.TransformForward(value);

            var cacheItem = axis.intervalBoundsCaches.GetIntervalBoundsCacheItemByPaneID(pane.paneID);
            if (cacheItem == null)
                return null;
            var result = this.CalcIntervalIndexes(value, axis.intervals, axis.scale);
            if (!result.inRange)
                return null;
            var indexes = result.indexes;
            if (!this.CheckIndexes(indexes, axis.intervals, cacheItem))
                return null;
            var length = isHorizontal ? pane.boundsWidth : pane.boundsHeight;
            var pointValue = this.CalcCoord(value, indexes, axis, cacheItem, length);
            if (isHorizontal)
                pointValue += pane.boundsLeft;
            else
                pointValue = pane.boundsHeight - pointValue + pane.boundsTop;
            return pointValue;
        },
        DiagramToPoint: function (argument, value, axisX, axisY, pane) {
            var coordinates = new ASPxClientControlCoordinates();
            if (!ASPx.IsExists(axisX.intervalBoundsCaches) || !ASPx.IsExists(axisY.intervalBoundsCaches))
                return coordinates;
            coordinates.pane = pane;
            var cacheItemX = axisX.intervalBoundsCaches.GetIntervalBoundsCacheItemByPaneID(pane.paneID);
            var cacheItemY = axisY.intervalBoundsCaches.GetIntervalBoundsCacheItemByPaneID(pane.paneID);
            if (cacheItemX == null || cacheItemY == null)
                return coordinates;
            var argumentInternal = axisX.GetInternalArgument(argument);
            if (axisX.scale instanceof ASPxClientQualitativeMap
                && (argumentInternal < axisX.intervals[0].minLimit || argumentInternal > axisX.intervals[axisX.intervals.length - 1].maxLimit))
                return coordinates;
            var valueInternal = axisY.GetInternalArgument(value);
            var resultsX = this.CalcIntervalIndexes(argumentInternal, axisX.intervals, axisX.scale);
            var resultsY = this.CalcIntervalIndexes(valueInternal, axisY.intervals, axisY.scale);
            var indexesX = resultsX.indexes;
            var indexesY = resultsY.indexes;
            if (!this.CheckIndexes(indexesX, axisX.intervals, cacheItemX)
                || !this.CheckIndexes(indexesY, axisY.intervals, cacheItemY))
                return coordinates;
            coordinates.visibility = resultsX.inRange && resultsY.inRange ? ASPxClientControlCoordinatesVisibility.Visible
                : ASPxClientControlCoordinatesVisibility.Hidden;
            var x, y;
            if (!this.rotated) {
                x = this.CalcCoord(argumentInternal, indexesX, axisX, cacheItemX, pane.boundsWidth);
                y = this.CalcCoord(valueInternal, indexesY, axisY, cacheItemY, pane.boundsHeight);
            }
            else {
                x = this.CalcCoord(valueInternal, indexesY, axisY, cacheItemY, pane.boundsWidth);
                y = this.CalcCoord(argumentInternal, indexesX, axisX, cacheItemX, pane.boundsHeight);
            }
            coordinates.SetPoint(x + pane.boundsLeft, pane.boundsTop + pane.boundsHeight - y);
            return coordinates;
        }
    });
    var ASPxClientXYDiagram = ASPx.CreateClass(ASPxClientXYDiagram2D, {
        constructor: function (chart, interimXYDiagram) {
            this.constructor.prototype.constructor.call(this, chart, interimXYDiagram);
        },
        InitializeProperties: function (interimObject) {
            ASPxClientXYDiagram2D.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.dr))
                throw ASPxClientWebChartElement.objectModelError;
            this.rotated = interimObject.dr;
        },
        CreateAxis: function (interimAxis) {
            return new ASPxClientAxis(this.chart, this, interimAxis);
        },
        CreateSecondaryAxes: function (interimSecondaryAxes) {
            return this.CreateArray(interimSecondaryAxes, function (chart, diagram, interimSecondaryAxis) {
                return new ASPxClientAxis(chart, diagram, interimSecondaryAxis);
            });
        }
    });
    var ASPxClientSwiftPlotDiagram = ASPx.CreateClass(ASPxClientXYDiagram2D, {
        constructor: function (chart, interimXYDiagram) {
            this.constructor.prototype.constructor.call(this, chart, interimXYDiagram);
        },
        InitializeProperties: function (interimObject) {
            ASPxClientXYDiagram2D.prototype.InitializeProperties.call(this, interimObject);
        },
        CreateAxis: function (interimAxis) {
            return new ASPxClientSwiftPlotDiagramAxis(this.chart, this, interimAxis);
        },
        CreateSecondaryAxes: function (interimSecondaryAxes) {
            return this.CreateArray(interimSecondaryAxes, function (chart, diagram, interimSecondaryAxis) {
                return new ASPxClientSwiftPlotDiagramAxis(chart, diagram, interimSecondaryAxis);
            });
        }
    });
    var ASPxClientXYDiagramPane = ASPx.CreateClass(ASPxClientWebChartElementNamed, {
        constructor: function (chart, diagram, interimPane) {
            this.constructor.prototype.constructor.call(this, chart, interimPane);
            this.diagram = diagram;
        },
        InitializeProperties: function (interimObject) {
            ASPxClientWebChartElementNamed.prototype.InitializeProperties.call(this, interimObject);
            this.primaryAxisXID = interimObject.paxi;
            this.primaryAxisYID = interimObject.payi;
            this.paneID = interimObject.id;
            if (ASPx.IsExists(interimObject.dx))
                this.boundsLeft = interimObject.dx;
            if (ASPx.IsExists(interimObject.dy))
                this.boundsTop = interimObject.dy;
            if (ASPx.IsExists(interimObject.dw))
                this.boundsWidth = interimObject.dw;
            if (ASPx.IsExists(interimObject.dh))
                this.boundsHeight = interimObject.dh;
            if (ASPx.IsExists(interimObject.dh))
                this.visible = interimObject.vsb;
            if (ASPx.IsExists(interimObject.lb))
                this.axisLabelBounds = this.CreateAxisLabelBounds(interimObject.lb);
        },
        CreateAxisLabelBounds: function (interimAxisLabelBounds) {
            return this.CreateArray(interimAxisLabelBounds, function (chart, pane, interimObject) {
                return new ASPxClientAxisLabelBounds(chart, pane, interimObject);
            });
        },
        InPane: function (x, y) {
            if (ASPx.IsExists(this.boundsLeft) && ASPx.IsExists(this.boundsTop) &&
                ASPx.IsExists(this.boundsWidth) && ASPx.IsExists(this.boundsHeight) &&
                (this.boundsWidth > 0) && (this.boundsHeight > 0)) {
                x -= this.boundsLeft;
                y -= this.boundsTop;
                return (x >= 0) && (x <= this.boundsWidth) && (y >= 0) && (y <= this.boundsHeight);
            }
            return false;
        }
    });
    var ASPxClientXYDiagram3D = ASPx.CreateClass(ASPxClientXYDiagramBase, {
        constructor: function (chart, interimXYDiagram) {
            this.constructor.prototype.constructor.call(this, chart, interimXYDiagram);
        },
        CreateAxis: function (interimAxis) {
            return new ASPxClientAxis3D(this.chart, this, interimAxis);
        }
    });
    var ASPxClientRadarDiagram = ASPx.CreateClass(ASPxClientXYDiagramBase, {
        constructor: function (chart, interimXYDiagram) {
            this.constructor.prototype.constructor.call(this, chart, interimXYDiagram);
        },
        InitializeProperties: function (interimObject) {
            ASPxClientXYDiagramBase.prototype.InitializeProperties.call(this, interimObject);
            if (ASPx.IsExists(interimObject.m))
                this.mapping = new ASPxClientRadarDiagramMapping(interimObject.m);
        },
        CreateAxis: function (interimAxis) {
            return new ASPxClientRadarAxis(this.chart, this, interimAxis);
        },
        CorrectAngle: function (angle) {
            if (this.mapping.revertAngle && angle != 0)
                angle = 2 * Math.PI - angle;
            if (this.mapping.diagramStartAngle >= 0) {
                if (angle >= this.mapping.diagramStartAngle)
                    angle -= this.mapping.diagramStartAngle;
                else
                    angle = 2 * Math.PI - this.mapping.diagramStartAngle + angle;
            }
            else {
                if (angle < 2 * Math.PI + this.mapping.diagramStartAngle)
                    angle -= this.mapping.diagramStartAngle;
                else
                    angle = angle - 2 * Math.PI - this.mapping.diagramStartAngle;
            }
            return angle;
        },
        CalcArgumentAndValueForPolygon: function (angle, dx, dy) {
            if (this.mapping.vertices.length <= 1)
                return null;
            var k = angle / (2 * Math.PI / (this.mapping.vertices.length - 1));
            var index = Math.floor(k);
            if (index < 0 || index >= this.mapping.vertices.length - 1)
                return null;
            var min = this.mapping.vertices[index];
            var max = this.mapping.vertices[index + 1];
            var divisor1 = (min.x - max.x) * dy - (min.y - max.y) * dx;
            if (divisor1 == 0)
                return null;
            var maxFactor = (min.x * dy - min.y * dx) / divisor1;
            var dist;
            var divisor2 = (min.x - max.x) * maxFactor - min.x;
            var divisor3 = (min.y - max.y) * maxFactor - min.y;
            if (dx != 0 && divisor2 != 0)
                dist = dx / divisor2;
            else if (dy != 0 && divisor3 != 0)
                dist = dy / divisor3;
            else
                return null;
            if (dist > this.mapping.radius)
                return null;
            valueInternal = dist / this.mapping.valueScaleFactor + this.mapping.minValue;
            argumentInternal = (max.argument - min.argument) * maxFactor + min.argument;
            return { argument: argumentInternal, value: valueInternal }
        },
        PointToDiagram: function (x, y) {
            var coordinates = new ASPxClientDiagramCoordinates();
            if (!ASPx.IsExists(this.mapping))
                return coordinates;
            if (this.axisX == null || this.axisY == null)
                return coordinates;
            coordinates.axisX = this.axisX;
            coordinates.axisY = this.axisY;
            var dy = this.mapping.centerY - y;
            var dx = this.mapping.centerX - x;
            var distance = Math.sqrt(dy * dy + dx * dx);
            if (this.mapping.radius == 0 || distance > this.mapping.radius)
                return coordinates;
            var argumentInternal, valueInternal;
            if (distance == 0) {
                argumentInternal = this.mapping.minArgument;
                valueInternal = this.mapping.minValue;
            }
            else {
                var angleCos = dy / distance;
                if (angleCos > 1 || angleCos < -1)
                    return coordinates;
                var angle = Math.acos(angleCos);
                if (x < this.mapping.centerX)
                    angle = Math.PI * 2 - angle;
                angle = this.CorrectAngle(angle);
                if (this.mapping.circle) {
                    valueInternal = distance / this.mapping.valueScaleFactor + this.mapping.minValue;
                    argumentInternal = angle / Math.PI / 2.0 * this.mapping.argumentDiapason + this.mapping.minArgument;

                }
                else {
                    var values = this.CalcArgumentAndValueForPolygon(angle, dx, dy);
                    if (values == null)
                        return coordinates;
                    argumentInternal = values.argument;
                    valueInternal = values.value;
                }
            }
            if (this.axisX.scale instanceof ASPxClientQualitativeMap)
                if (argumentInternal > this.mapping.minArgument + this.mapping.argumentDiapason - this.mapping.argumentDelta / 2)
                    argumentInternal -= this.mapping.argumentDiapason;

            if (this.axisX.scale.logarithmic)
                argumentInternal = this.axisX.scale.TransformBackward(argumentInternal);
            if (this.axisY.scale.logarithmic)
                valueInternal = this.axisY.scale.TransformBackward(valueInternal);

            coordinates.SetArgumentAndValue(this.axisX, this.axisY, argumentInternal, valueInternal);
            return coordinates;
        },
        DiagramToPoint: function (argument, value) {
            var coordinates = new ASPxClientControlCoordinates();
            if (!ASPx.IsExists(this.mapping))
                return coordinates;
            var argumentInternal = this.axisX.GetInternalArgument(argument);
            var valueInternal = this.axisY.GetInternalArgument(value);
            var point = this.mapping.GetGetScreenPoint(argumentInternal, valueInternal);
            if (point == null)
                return coordinates;
            coordinates.visibility = valueInternal > this.mapping.maxValue ? ASPxClientControlCoordinatesVisibility.Hidden : ASPxClientControlCoordinatesVisibility.Visible;
            coordinates.SetPoint(point.x, point.y);
            return coordinates;
        }
    });

    var ASPxClientRadarDiagramMapping = ASPx.CreateClass(null, {
        constructor: function (interimMapping) {
            if (ASPx.IsExists(interimMapping.r))
                this.radius = interimMapping.r;
            if (ASPx.IsExists(interimMapping.cx))
                this.centerX = interimMapping.cx;
            if (ASPx.IsExists(interimMapping.cy))
                this.centerY = interimMapping.cy;
            if (ASPx.IsExists(interimMapping.ma))
                this.minArgument = interimMapping.ma;
            if (ASPx.IsExists(interimMapping.mv))
                this.minValue = interimMapping.mv;
            if (ASPx.IsExists(interimMapping.ra))
                this.revertAngle = interimMapping.ra;
            if (ASPx.IsExists(interimMapping.a))
                this.diagramStartAngle = interimMapping.a;
            if (ASPx.IsExists(interimMapping.f))
                this.valueScaleFactor = interimMapping.f;
            if (ASPx.IsExists(interimMapping.d))
                this.argumentDiapason = interimMapping.d;
            if (ASPx.IsExists(interimMapping.ad))
                this.argumentDelta = interimMapping.ad;
            if (ASPx.IsExists(interimMapping.ci))
                this.circle = interimMapping.ci;
            if (ASPx.IsExists(interimMapping.c))
                this.clipArgument = interimMapping.c;
            if (ASPx.IsExists(interimMapping.mxa))
                this.maxArgument = interimMapping.mxa;
            if (ASPx.IsExists(interimMapping.mxv))
                this.maxValue = interimMapping.mxv;
            if (ASPx.IsExists(interimMapping.sa))
                this.startAngle = interimMapping.sa;
            this.vertices = this.CreateArray(interimMapping.v, function (interimVertex) {
                return new ASPxClientVertex(interimVertex);
            });
        },
        CreateArray: function (interimArray, createArrayItem) {
            if (!ASPx.IsExists(interimArray))
                return [];
            if (!(interimArray instanceof Array))
                throw ASPxClientWebChartElement.objectModelError;
            var result = [];
            for (var i = 0; i < interimArray.length; i++)
                result.push(createArrayItem(interimArray[i]));
            return result;
        },
        GetGetScreenPoint: function (argument, value) {
            if (this.clipArgument && (argument < this.minArgument || argument > this.maxArgument))
                return null;
            while (argument < this.minArgument)
                argument += this.argumentDiapason;
            while (argument > this.maxArgument)
                argument -= this.argumentDiapason;
            if (value < this.minValue)
                value = this.minValue;
            var x, y;
            if (this.circle) {
                var distance = (value - this.minValue) * this.valueScaleFactor;
                var angle = (argument - this.minArgument) / this.argumentDiapason * Math.PI * 2.0;
                if (this.revertAngle)
                    angle = -angle;
                angle += this.startAngle;
                x = Math.cos(angle) * distance + this.centerX;
                y = Math.sin(angle) * distance + this.centerY;
            }
            else {
                var distance = (value - this.minValue) * this.valueScaleFactor;
                var coord = (argument - this.minArgument) / this.argumentDelta;
                var index = Math.floor(coord);
                if (index < 0)
                    index = 0;
                else if (index >= this.vertices.length - 1)
                    index = this.vertices.length - 2;
                var min = this.vertices[index];
                var max = this.vertices[index + 1];
                var maxFactor = (argument - min.argument) / (max.argument - min.argument);
                var minFactor = 1.0 - maxFactor;
                x = (min.x * minFactor + max.x * maxFactor) * distance + this.centerX;
                y = (min.y * minFactor + max.y * maxFactor) * distance + this.centerY;
            }
            return { x: x, y: y };
        }
    });

    var ASPxClientVertex = ASPx.CreateClass(null, {
        constructor: function (interimVertex) {
            if (!ASPx.IsExists(interimVertex.a))
                throw ASPxClientWebChartElement.objectModelError;
            this.argument = interimVertex.a;
            if (!ASPx.IsExists(interimVertex.x))
                throw ASPxClientWebChartElement.objectModelError;
            this.x = interimVertex.x;
            if (!ASPx.IsExists(interimVertex.y))
                throw ASPxClientWebChartElement.objectModelError;
            this.y = interimVertex.y;
        }
    });

    var ASPxClientQualitativeMap = ASPx.CreateClass(null, {
        constructor: function (interimMap) {
            if (!ASPx.IsExists(interimMap.vl))
                throw ASPxClientWebChartElement.objectModelError;
            this.values = interimMap.vl;
        },
        GetNativeValue: function (internalValue) {
            var roundedValue = Math.round(internalValue);
            return (roundedValue < 0 || roundedValue >= this.values.length) ? null : this.values[roundedValue];
        },
        GetInternalValue: function (value) {
            for (var i = 0; i < this.values.length; i++) {
                if (this.values[i] == value)
                    return i;
            }
            return -1;
        }
    });

    var ASPxClientNumericalMap = ASPx.CreateClass(null, {
        constructor: function (interimMap) {
            if (!ASPx.IsExists(interimMap.l))
                throw ASPxClientWebChartElement.objectModelError;
            this.logarithmic = interimMap.l;
            if (this.logarithmic) {
                if (!ASPx.IsExists(interimMap.lb) || !ASPx.IsExists(interimMap.mlv))
                    throw ASPxClientWebChartElement.objectModelError;
                this.logarithmicBase = interimMap.lb;
                this.minLogValue = interimMap.mlv;
            }
        },
        GetValueSign: function (value) {
            if (value < 0)
                return -1;
            if (value > 0)
                return 1;
            return 0;
        },
        TransformBackward: function (value) {
            return isFinite(value) ? Math.pow(this.logarithmicBase, Math.abs(value) + this.minLogValue) * this.GetValueSign(value) : value;
        },
        TransformForward: function (value) {
            if (!isFinite(value))
                return value;
            var transformedAbsValue = Math.log(Math.abs(value)) / Math.log(this.logarithmicBase) - this.minLogValue;
            if (transformedAbsValue < 0)
                transformedAbsValue = 0;
            return this.GetValueSign(value) * transformedAbsValue;
        },
        GetNativeValue: function (internalValue) {
            return this.logarithmic ? this.TransformBackward(internalValue) : internalValue;
        },
        GetInternalValue: function (value) {
            return this.logarithmic ? this.TransformForward(value) : value;
        }
    });

    var ASPxClientDateTimeMap = ASPx.CreateClass(null, {
        constructor: function (interimMap) {
            if (!ASPx.IsExists(interimMap.su) || !ASPx.IsExists(interimMap.sa) || !ASPx.IsExists(interimMap.sv))
                throw ASPxClientWebChartElement.objectModelError;

            this.measureUnit = interimMap.su;
            this.gridAlignment = interimMap.sa;
            this.startDate = this.ParseStartDate(interimMap.sv);
            if (ASPx.IsExists(interimMap.swo))
                this.workdaysOnly = interimMap.swo;
            else
                this.workdaysOnly = false;
            if (this.workdaysOnly) {
                if (!ASPx.IsExists(interimMap.sw))
                    throw ASPxClientWebChartElement.objectModelError;
                this.workdays = interimMap.sw;
                this.holidaysCount = this.CalcHolidaysCountInWeek();
                this.workdaysCount = 7 - this.holidaysCount;
                if (ASPx.IsExists(interimMap.sh))
                    this.holidays = interimMap.sh;
                else
                    this.holidays = [];
                if (ASPx.IsExists(interimMap.sew))
                    this.exactWorkdays = interimMap.sew;
                else
                    this.exactWorkdays = [];
            }
        },
        ParseStartDate: function (startDateString) {
            var date = new Date(1, 1, 1);
            var year = startDateString.substring(0, 4);
            var month = startDateString.substring(5, 7) - 1;
            var day = startDateString.substring(8, 10);
            date.setFullYear(year, month, day);
            return date;
        },
        CalcHolidaysCountInWeek: function () {
            var result = 0;
            var dayFlag = 1;
            for (var i = 0; i < 7; i++) {
                if ((this.workdays & dayFlag) == 0)
                    result++;
                dayFlag <<= 1;
            }
            return result;
        },
        AddDays: function (dateTime, count) {
            return new Date(dateTime.getTime() + count * 1000 * 60 * 60 * 24);
        },
        GetTotalDays: function (fromDate, toDate) {
            return (toDate.getTime() - fromDate.getTime()) / 24 / 60 / 60 / 1000;
        },
        GetDate: function (n) {
            var diffMinutes = this.startDate.getTimezoneOffset() - new Date(this.startDate.getTime() + n).getTimezoneOffset();
            var diffMilliseconds = diffMinutes * 60 * 1000;
            return new Date(this.startDate.getTime() + n - diffMilliseconds);
        },
        GetMonth: function (n, date) {
            var fromDate = ASPx.IsExists(date) ? date : this.startDate;
            var monthsCount = Math.abs(n);
            var years = Math.floor(monthsCount / 12);
            var monthsFractional = monthsCount - years * 12;
            var months = Math.floor(monthsFractional);
            var fraction = monthsFractional - months;
            if (n < 0) {
                years = -years;
                months = -months;
                fraction = -fraction;
            }
            var year = fromDate.getFullYear() + years;
            var month = fromDate.getMonth() + months;
            if (month < 0) {
                year = year - 1;
                month = month + 12;
            }
            else if (month >= 12) {
                year = year + 1;
                month = month - 12;
            }
            var result = new Date(1, 1, 1);
            result.setFullYear(year, month, 1);
            if (fraction < 0) {
                var previousMonth;
                if (month == 0)
                    previousMonth = new Date(year - 1, 11, 1);
                else
                    previousMonth = new Date(year, month - 1, 1);
                return new Date(result.getTime() + (result.getTime() - previousMonth.getTime()) * fraction);
            }
            else {
                var nextMonth;
                if (month == 11)
                    nextMonth = new Date(year + 1, 1, 1);
                else
                    nextMonth = new Date(year, month + 1, 1);
                return new Date(result.getTime() + (nextMonth.getTime() - result.getTime()) * fraction);
            }
        },
        TruncateTime: function (date) {
            var truncatedDate = new Date(1, 1, 1);
            truncatedDate.setFullYear(date.getFullYear(), date.getMonth(), date.getDate());
            return truncatedDate;
        },
        IsHoliday: function (dateTime, applyHolidays, applyExactWorkdays) {
            if (!this.workdaysOnly)
                return false;
            var isHoliday = (this.workdays & (1 << dateTime.getDay())) == 0;
            if (applyHolidays || applyExactWorkdays) {
                var date = new Date(1, 1, 1);
                date.setFullYear(dateTime.getFullYear(), dateTime.getMonth(), dateTime.getDate());
                var time = date.getTime();
                if (applyHolidays)
                    for (holiday in this.holidays)
                        if (time == this.holidays[holiday].getTime()) {
                            isHoliday = true;
                            break;
                        }
                if (applyExactWorkdays)
                    for (exactWorkDay in this.exactWorkdays)
                        if (time == this.exactWorkdays[exactWorkDay].getTime()) {
                            isHoliday = false;
                            break;
                        }
            }
            return isHoliday;
        },
        SkipHoliday: function (dateTime) {
            var result = new Date(dateTime.getTime());
            if (this.workdays == 0)
                return result;
            while (this.IsHoliday(result, true, true)) {
                var newDate = this.AddDays(result, 1);
                if (newDate.getDate() == result.getDate()) {
                    //Time zone changing
                    newDate = new Date(result.getTime() + 1000 * 60 * 60 * 26);
                }
                result = this.TruncateTime(newDate);
            }
            return result;
        },
        Floor: function (dateTime) {
            return this.FloorToDateTimeMeasureUnit(dateTime, this.measureUnit);
        },
        FloorToDateTimeMeasureUnit: function (dateTime, dateTimeMeasureUnit) {
            var result = new Date(dateTime.getTime());
            switch (dateTimeMeasureUnit) {
                case 'Year':
                    result.setMonth(0, 1);
                    result = this.TruncateTime(result);
                    break;
                case 'Month':
                    result.setDate(1);
                    result = this.TruncateTime(result);
                    break;
                case 'Day':
                    result = this.TruncateTime(result);
                    break;
                case 'Hour':
                    result.setMinutes(0, 0, 0);
                    break;
                case 'Minute':
                    result.setSeconds(0, 0);
                    break;
                case 'Second':
                    result.setMilliseconds(0);
                    break;
                case 'Quarter':
                    result = this.GetMonth(-(result.getMonth() % 3), result);
                    result.setDate(1);
                    break;
                case 'Week': {
                    var offset = result.getDay() - this.startDate.getDay();
                    result = this.TruncateTime(this.AddDays(result, -(offset >= 0 ? offset : (offset + 7))));
                    break;
                }
            }
            return result;
        },
        Round: function (dateTime) {
            if (this.measureUnit != 'Millisecond') {
                if (dateTime.getMilliseconds() >= 500)
                    dateTime.setSeconds(dateTime.getSeconds() + 1);
                dateTime.setMilliseconds(0);
                if (this.measureUnit != 'Second') {
                    if (dateTime.getSeconds() >= 30)
                        dateTime.setMinutes(dateTime.getMinutes() + 1);
                    dateTime.setSeconds(0);
                    if (this.measureUnit != 'Minute') {
                        if (dateTime.getMinutes() >= 30)
                            dateTime.setHours(dateTime.getHours() + 1);
                        dateTime.setMinutes(0);
                        if (this.measureUnit != 'Hour') {
                            if (dateTime.getHours() >= 12)
                                dateTime.setDate(dateTime.getDate() + 1);
                            dateTime.setHours(0);
                            if (this.measureUnit != 'Day') {
                                if (this.measureUnit == 'Week') {
                                    var date = dateTime.getDate() - dateTime.getDay() + this.startDate.getDay();
                                    if (dateTime.getDay() >= this.startDate.getDay())
                                        dateTime.setDate(date);
                                    else
                                        dateTime.setDate(date - 7);
                                }
                                else {
                                    var half;
                                    if (dateTime.getMonth() == 2)
                                        half = 15;
                                    else
                                        half = 16;
                                    if (dateTime.getDate() >= half)
                                        dateTime.setMonth(dateTime.getMonth() + 1);
                                    dateTime.setDate(1);
                                    if (this.measureUnit != 'Month') {
                                        if (this.measureUnit == 'Quarter')
                                            dateTime.setMonth(Math.floor(dateTime.getMonth() / 3) * 3);
                                        else if (this.measureUnit == 'Year') {
                                            if (dateTime.getMonth() >= 6)
                                                dateTime.setFullYear(dateTime.getFullYear() + 1);
                                            dateTime.setMonth(0);
                                        }
                                        else
                                            return initialDate;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return dateTime;
        },
        AddUnits: function (range, factor) {
            range = range * factor;
            var integerRange = Math.floor(range);
            var remain = range - integerRange;
            var result = this.GetMonth(integerRange, this.startDate);
            var daysInMonth = this.GetTotalDays(result, this.GetMonth(1, result));
            if (this.workdaysOnly) {
                var dayCount = 0;
                var currentDate = new Date(result.getTime());
                for (var i = 0; i < daysInMonth; i++) {
                    if (!this.IsHoliday(currentDate, true, true))
                        dayCount++;
                    currentDate = this.AddDays(currentDate, 1);
                }
                remain *= dayCount;
                while (remain >= 0.5) {
                    if (!this.IsHoliday(result, true, true))
                        remain -= 1;
                    result = this.AddDays(result, 1);
                }
                if (remain > 0)
                    while (this.IsHoliday(result, true, true))
                        result = this.AddDays(result, 1);
                return this.SkipHoliday(this.AddDays(result, remain));
            }
            else
                return this.AddDays(result, daysInMonth * remain);
        },
        CorrectRangeBasedOnHolidays: function (range, multiplier) {
            if (range == 0 || this.holidaysCount == 7)
                return range;
            var fromDate = this.TruncateTime(this.startDate);
            var actualDate = new Date(fromDate.getTime());
            var dayRange = Math.floor((range + (this.startDate.getTime() - actualDate.getTime()) / (1000 * 60 * 60 * 24) * multiplier) / multiplier);
            var remain = range - (dayRange * multiplier);
            if (dayRange > 0) {
                var fullWeeks = Math.floor(dayRange / this.workdaysCount);
                var remainWeekDays = dayRange % this.workdaysCount;
                for (; remainWeekDays > 0;) {
                    if (this.IsHoliday(actualDate, false, false))
                        dayRange++;
                    else
                        remainWeekDays--;
                    actualDate = this.AddDays(actualDate, 1);
                }
                dayRange += fullWeeks * this.holidaysCount;
                actualDate = this.AddDays(fromDate, dayRange);
                while (this.IsHoliday(actualDate, false, false)) {
                    dayRange++;
                    actualDate = this.AddDays(actualDate, 1);
                }
                for (holiday in this.holidays) {
                    var holidayDate = this.holidays[holiday];
                    if (holidayDate >= fromDate && holidayDate <= actualDate && !this.IsHoliday(holidayDate, false, false))
                        do {
                            dayRange++;
                            actualDate = this.AddDays(actualDate, 1);
                        } while (this.IsHoliday(actualDate, false, false));
                }
                for (workday in this.exactWorkdays) {
                    var workDate = this.exactWorkdays[workday];
                    if (workDate >= fromDate && workDate <= actualDate && this.IsHoliday(workDate, true, false))
                        do {
                            dayRange--;
                            actualDate = this.AddDays(actualDate, -1);
                        } while (this.IsHoliday(actualDate, true, true));
                }
                range = dayRange * multiplier + remain;
            }
            else {
                var fullWeeks = Math.ceil(dayRange / this.workdaysCount);
                var remainWeekDays = dayRange % this.workdaysCount;
                for (; remainWeekDays < 0;) {
                    if (this.IsHoliday(actualDate, false, false))
                        dayRange--;
                    else
                        remainWeekDays++;
                    actualDate = this.AddDays(actualDate, -1);
                }
                dayRange += fullWeeks * this.holidaysCount;
                actualDate = this.AddDays(fromDate, dayRange);
                while (this.IsHoliday(actualDate, false, false)) {
                    dayRange--;
                    actualDate = this.AddDays(actualDate, -1);
                }
                var revertedHolidays = [];
                for (holiday in this.holidays)
                    revertedHolidays.unshift(this.holidays[holiday]);
                for (holiday in revertedHolidays) {
                    var holidayDate = revertedHolidays[holiday];
                    if (holidayDate >= actualDate && holidayDate <= fromDate && !this.IsHoliday(holidayDate, false, false))
                        do {
                            dayRange--;
                            actualDate = this.AddDays(actualDate, -1);
                        } while (this.IsHoliday(actualDate, false, false));
                }
                for (workday in this.exactWorkdays) {
                    var workDate = this.exactWorkdays[workday];
                    if (workDate >= actualDate && workDate <= fromDate && this.IsHoliday(workDate, true, false))
                        do {
                            dayRange++;
                            actualDate = this.AddDays(actualDate, 1);
                        } while (this.IsHoliday(actualDate, true, true));
                }
                range = dayRange * multiplier + remain;
            }
            return range;
        },
        GetNativeValue: function (internalValue) {
            var nativeValue;
            switch (this.measureUnit) {
                case 'Year':
                    nativeValue = this.AddUnits(internalValue, 12);
                    break;
                case 'Quarter':
                    nativeValue = this.AddUnits(internalValue, 3);
                    break;
                case 'Month':
                    nativeValue = this.AddUnits(internalValue, 1);
                    break;
                case 'Week':
                    if (this.workdaysOnly) {
                        var fullWeeks = Math.floor(internalValue);
                        var remain = internalValue - fullWeeks;
                        nativeValue = this.AddDays(this.startDate, fullWeeks * 7);
                        var weekDaysCount = 0;
                        for (var i = 0; i < 7; i++)
                            if (!this.IsHoliday(this.AddDays(nativeValue, i), true, true))
                                weekDaysCount++;
                        remain *= weekDaysCount;
                        while (remain >= 0.5) {
                            if (!this.IsHoliday(nativeValue, true, true))
                                remain -= 1;
                            nativeValue = this.AddDays(nativeValue, 1);
                        }
                        nativeValue = this.SkipHoliday(this.AddDays(nativeValue, remain));
                    }
                    else
                        nativeValue = this.GetDate(internalValue * 1000 * 60 * 60 * 24 * 7);
                    break;
                case 'Day':
                    if (this.workdaysOnly)
                        internalValue = this.CorrectRangeBasedOnHolidays(internalValue, 1)
                    nativeValue = this.AddDays(this.startDate, internalValue);
                    break;
                case 'Hour':
                    if (this.workdaysOnly)
                        internalValue = this.CorrectRangeBasedOnHolidays(internalValue, 24)
                    nativeValue = this.GetDate(internalValue * 1000 * 60 * 60);
                    break;
                case 'Minute':
                    if (this.workdaysOnly)
                        internalValue = this.CorrectRangeBasedOnHolidays(internalValue, 24 * 60)
                    nativeValue = this.GetDate(internalValue * 1000 * 60);
                    break;
                case 'Second':
                    if (this.workdaysOnly)
                        internalValue = this.CorrectRangeBasedOnHolidays(internalValue, 24 * 60 * 60)
                    nativeValue = this.GetDate(internalValue * 1000);
                    break;
                case 'Millisecond':
                    if (this.workdaysOnly)
                        internalValue = this.CorrectRangeBasedOnHolidays(internalValue, 24 * 60 * 60 * 1000)
                    nativeValue = this.GetDate(internalValue);
                    break;
                default:
                    nativeValue = this.GetDate(0);
                    break;
            }
            return this.Round(nativeValue);
        },
        TotalUnits: function (dateTime, majorUnit) {
            var monthRounded = this.FloorToDateTimeMeasureUnit(dateTime, 'Month');
            var dayRounded = this.FloorToDateTimeMeasureUnit(dateTime, 'Day');
            var daysInMonth = this.GetTotalDays(monthRounded, this.GetMonth(1, monthRounded));
            var monthAddition;
            if (this.workdaysOnly) {
                var dayCount = 0;
                var actualDaysInMonth = 0;
                var currentDate = new Date(monthRounded.getTime());
                for (var i = 0; i < daysInMonth; i++) {
                    if (!this.IsHoliday(currentDate, true, true)) {
                        actualDaysInMonth++;
                        if (currentDate < dayRounded)
                            dayCount++;
                    }
                    currentDate = this.AddDays(currentDate, 1);
                }
                monthAddition = actualDaysInMonth == 0 ? 0 : (dayCount / actualDaysInMonth);
                daysInMonth = actualDaysInMonth;
            }
            else
                monthAddition = this.GetTotalDays(monthRounded, dayRounded) / daysInMonth;
            var divider = daysInMonth * 24 * 60 * 60 * 1000;
            return (monthRounded.getFullYear() * 12 + monthRounded.getMonth() - 1 + monthAddition +
                    (dateTime.getTime() - dayRounded.getTime()) / divider) / majorUnit;
        },
        CorrectDifferenceBasedOnHolidays: function (value, multiplier) {
            if (this.holidaysCount == 7)
                return value;
            var actualDate = this.TruncateTime(this.startDate);
            var totalDays = Math.floor((value + this.GetTotalDays(actualDate, this.startDate) * multiplier) / multiplier);
            if (totalDays > 0) {
                var fullWeeks = Math.floor(totalDays / 7);
                value -= fullWeeks * this.holidaysCount * multiplier;
                var remainWeekDays = totalDays % 7;
                for (var i = 0; i < remainWeekDays; i++) {
                    if (this.IsHoliday(actualDate, false, false))
                        value -= multiplier;
                    actualDate = this.AddDays(actualDate, 1);
                }
            }
            else if (totalDays < 0) {
                var fullWeeks = Math.ceil(totalDays / 7);
                value -= fullWeeks * this.holidaysCount * multiplier;
                var remainWeekDays = totalDays % 7;
                for (var i = 0; i > remainWeekDays; i--) {
                    if (this.IsHoliday(actualDate, false, false))
                        value += multiplier;
                    actualDate = this.AddDays(actualDate, -1);
                }
            }
            actualDate = this.TruncateTime(this.startDate);
            var finishDate = this.AddDays(actualDate, totalDays);
            if (finishDate < actualDate) {
                for (holiday in this.holidays) {
                    var holidayDate = this.holidays[holiday];
                    if (holidayDate <= actualDate && holidayDate > finishDate && !this.IsHoliday(holidayDate, false, false))
                        value += multiplier;
                }
                for (exactWorkdate in this.exactWorkdays) {
                    var workDate = this.exactWorkdays[exactWorkdate];
                    if (workDate <= actualDate && workDate > finishDate && this.IsHoliday(workDate, true, false))
                        value -= multiplier;
                }
            }
            else {
                for (holiday in this.holidays) {
                    var holidayDate = this.holidays[holiday];
                    if (holidayDate >= actualDate && holidayDate < finishDate && !this.IsHoliday(holidayDate, false, false))
                        value -= multiplier;
                }
                for (exactWorkdate in this.exactWorkdays) {
                    var workDate = this.exactWorkdays[exactWorkdate];
                    if (workDate >= actualDate && workDate < finishDate && this.IsHoliday(workDate, true, false))
                        value += multiplier;
                }
            }
            return value;
        },
        GetTimeDifference: function (date1, date2) {
            return date1.getTime() - date2.getTime() - (date1.getTimezoneOffset() - date2.getTimezoneOffset()) * 60 * 1000;
        },
        GetInternalValue: function (value) {
            if (this.workdaysOnly)
                value = this.SkipHoliday(value);
            var roundedToDate = this.Floor(value);
            switch (this.measureUnit) {
                case 'Year':
                    return this.TotalUnits(roundedToDate, 12) - this.TotalUnits(this.startDate, 12);
                case 'Quarter':
                    return this.TotalUnits(roundedToDate, 3) - this.TotalUnits(this.startDate, 3);
                case 'Month':
                    return this.TotalUnits(roundedToDate, 1) - this.TotalUnits(this.startDate, 1);
                case 'Week': {
                    if (!this.workdaysOnly)
                        return (roundedToDate.getTime() - this.startDate.getTime()) / 7 / 24 / 60 / 60 / 1000;
                    var roundedToWeek = this.FloorToDateTimeMeasureUnit(value, 'Week');
                    var roundedToDay = this.FloorToDateTimeMeasureUnit(value, 'Day');
                    var weekDaysCount = 0;
                    var spentDaysCount = 0;
                    var tempDate = roundedToWeek;
                    for (var i = 0; i < 7; i++) {
                        if (!this.IsHoliday(tempDate, true, true)) {
                            weekDaysCount++;
                            if (tempDate < roundedToDay)
                                spentDaysCount++;
                        }
                        tempDate = this.AddDays(tempDate, 1);
                    }
                    return this.GetTotalDays(this.startDate, roundedToWeek) / 7 +
                       (spentDaysCount + this.GetTotalDays(roundedToDay, value)) / weekDaysCount;
                }
                case 'Day': {
                    var totalDays = this.GetTotalDays(this.startDate, roundedToDate);
                    if (this.workdaysOnly)
                        totalDays = this.CorrectDifferenceBasedOnHolidays(totalDays, 1);
                    return totalDays;
                }
                case 'Hour': {
                    var totalHours = this.GetTimeDifference(roundedToDate, this.startDate) / 60 / 60 / 1000;
                    if (this.workdaysOnly)
                        totalHours = this.CorrectDifferenceBasedOnHolidays(totalHours, 24);
                    return totalHours;
                }
                case 'Minute': {
                    var totalMinutes = this.GetTimeDifference(roundedToDate, this.startDate) / 60 / 1000;
                    if (this.workdaysOnly)
                        totalMinutes = this.CorrectDifferenceBasedOnHolidays(totalMinutes, 24 * 60);
                    return totalMinutes;
                }
                case 'Second': {
                    var totalSeconds = this.GetTimeDifference(roundedToDate, this.startDate) / 1000;
                    if (this.workdaysOnly)
                        totalSeconds = this.CorrectDifferenceBasedOnHolidays(totalSeconds, 24 * 60 * 60);
                    return totalSeconds;
                }
                case 'Millisecond': {
                    var totalMilliseconds = this.GetTimeDifference(roundedToDate, this.startDate);
                    if (this.workdaysOnly)
                        totalMilliseconds = this.CorrectDifferenceBasedOnHolidays(totalMilliseconds, 24 * 60 * 60 * 1000);
                    return totalMilliseconds;
                }
                default:
                    return 0;
            }
        }
    });
    var ASPxClientAxisBase = ASPx.CreateClass(ASPxClientWebChartElementNamed, {
        constructor: function (chart, diagram, interimAxis) {
            this.constructor.prototype.constructor.call(this, chart, interimAxis);
            this.diagram = diagram;
        },
        InitializeProperties: function (interimObject) {
            ASPxClientWebChartElementNamed.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.r))
                throw ASPxClientWebChartElement.objectModelError;
            this.range = new ASPxClientAxisRange(this.chart, this, interimObject.r);

            if (ASPx.IsExists(interimObject.m))
                this.scale = this.CreateMap(interimObject.m);
            if (ASPx.IsExists(interimObject.l))
                this.labelItems = this.CreateLabelItems(interimObject.l);
        },
        CreateMap: function (interimMap) {
            if (interimMap.t == "N")
                return new ASPxClientNumericalMap(interimMap);
            if (interimMap.t == "Q")
                return new ASPxClientQualitativeMap(interimMap);
            if (interimMap.t == "D")
                return new ASPxClientDateTimeMap(interimMap);
            throw ASPxClientWebChartElement.objectModelError;
        },
        CreateLabelItems: function (interimLabelItems) {
            return this.CreateArray(interimLabelItems, createLabelItem = function (chart, axis, interimLabelItem) {
                return new ASPxClientAxisLabelItem(chart, axis, interimLabelItem);
            });
        },
        GetNativeArgument: function (value) {
            return (this.scale == null) ? value : this.scale.GetNativeValue(value);
        },
        GetInternalArgument: function (value) {
            return (this.scale == null) ? value : this.scale.GetInternalValue(value);
        }
    });
    var ASPxClientAxis2D = ASPx.CreateClass(ASPxClientAxisBase, {
        constructor: function (chart, diagram, interimAxis) {
            this.constructor.prototype.constructor.call(this, chart, diagram, interimAxis);
        },
        InitializeProperties: function (interimObject) {
            ASPxClientAxisBase.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.t))
                throw ASPxClientWebChartElement.objectModelError;
            this.axisTitle = new ASPxClientAxisTitle(this.chart, this, interimObject.t);
            this.strips = this.CreateStrips(interimObject.s);
            this.constantLines = this.CreateConstantLines(interimObject.cl);
            this.intervals = this.CreateIntervals(interimObject.i);
            if (ASPx.IsExists(interimObject.ibc))
                this.intervalBoundsCaches = new ASPxClientIntervalBoundsCache(interimObject.ibc);
            this.axisID = interimObject.id;
            this.isArgumentAxis = interimObject.x;
            if (ASPx.IsExists(interimObject.cao))
                this.crosshairAxisLabelOptions = new ASPxClientCrosshairAxisLabelOptions(this.chart, interimObject.cao);
        },
        CreateStrips: function (interimStrips) {
            return this.CreateArray(interimStrips, createStrip = function (chart, axis, interimStrip) {
                return new ASPxClientStrip(chart, axis, interimStrip);
            });
        },
        CreateConstantLines: function (interimConstantLines) {
            return this.CreateArray(interimConstantLines, function (chart, axis, interimConstantLine) {
                return new ASPxClientConstantLine(chart, axis, interimConstantLine);
            });
        },
        CreateIntervals: function (interimIntervals) {
            return this.CreateArray(interimIntervals, function (chart, axis, interimInterval) {
                return new ASPxClientAxisInterval(interimInterval);
            });
        }
    });
    var ASPxClientAxis = ASPx.CreateClass(ASPxClientAxis2D, {
        constructor: function (chart, diagram, interimAxis) {
            this.constructor.prototype.constructor.call(this, chart, diagram, interimAxis);
        },
        InitializeProperties: function (interimObject) {
            ASPxClientAxis2D.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.ar))
                throw ASPxClientWebChartElement.objectModelError;
            this.reverse = interimObject.ar;
        }
    });
    var ASPxClientSwiftPlotDiagramAxis = ASPx.CreateClass(ASPxClientAxis2D, {
        constructor: function (chart, diagram, interimAxis) {
            this.constructor.prototype.constructor.call(this, chart, diagram, interimAxis);
        },
        InitializeProperties: function (interimObject) {
            ASPxClientAxis2D.prototype.InitializeProperties.call(this, interimObject);
        }
    });
    var ASPxClientAxis3D = ASPx.CreateClass(ASPxClientAxisBase, {
        constructor: function (chart, diagram, interimAxis) {
            this.constructor.prototype.constructor.call(this, chart, diagram, interimAxis);
        }
    });
    var ASPxClientRadarAxis = ASPx.CreateClass(ASPxClientAxisBase, {
        constructor: function (chart, diagram, interimAxis) {
            this.constructor.prototype.constructor.call(this, chart, diagram, interimAxis);
        }
    });
    var ASPxClientAxisTitle = ASPx.CreateClass(ASPxClientWebChartRequiredElement, {
        constructor: function (chart, axis, interimAxisTitle) {
            this.constructor.prototype.constructor.call(this, chart, interimAxisTitle);
            this.axis = axis;
        },
        InitializeProperties: function (interimObject) {
            if (!ASPx.IsExists(interimObject.tx))
                throw ASPxClientWebChartElement.objectModelError;
            this.text = interimObject.tx;
        }
    });
    var ASPxClientAxisLabelItem = ASPx.CreateClass(ASPxClientWebChartRequiredElement, {
        constructor: function (chart, axis, interimLabelItem) {
            this.constructor.prototype.constructor.call(this, chart, interimLabelItem);
            this.axis = axis;
        },
        InitializeProperties: function (interimObject) {
            if (!ASPx.IsExists(interimObject.tx) || !ASPx.IsExists(interimObject.av) || !ASPx.IsExists(interimObject.iv))
                throw ASPxClientWebChartElement.objectModelError;
            this.text = interimObject.tx;
            this.axisValue = interimObject.av;
            this.axisValueInternal = interimObject.iv;
        }
    });
    var ASPxClientAxisRange = ASPx.CreateClass(ASPxClientWebChartRequiredElement, {
        constructor: function (chart, axis, interimRange) {
            this.constructor.prototype.constructor.call(this, chart, interimRange);
            this.axis = axis;
        },
        InitializeProperties: function (interimObject) {
            if (!ASPx.IsExists(interimObject.ii) || !ASPx.IsExists(interimObject.ia))
                throw ASPxClientWebChartElement.objectModelError;
            this.minValue = interimObject.mi;
            this.maxValue = interimObject.ma;
            this.minValueInternal = interimObject.ii;
            this.maxValueInternal = interimObject.ia;
        }
    });

    var ASPxClientAxisInterval = ASPx.CreateClass(null, {
        constructor: function (interimInterval) {
            this.minLimit = interimInterval.mi;
            this.maxLimit = interimInterval.ma;
        },
        GetInternalValue: function (ratio) {
            return this.minLimit + (this.maxLimit - this.minLimit) * ratio;
        }
    });

    var ASPxClientIntervalBoundsCache = ASPx.CreateClass(null, {
        constructor: function (interimCache) {
            this.items = this.CreateItems(interimCache.i);
        },
        CreateItems: function (interimItems) {
            var result = [];
            for (var i = 0; i < interimItems.length; i++)
                result.push(new ASPxClientIntervalBoundsCacheItem(interimItems[i]));
            return result;
        },
        GetIntervalBoundsCacheItemByPaneID: function (paneID) {
            for (var i = 0; i < this.items.length; i++)
                if (this.items[i].paneID == paneID)
                    return this.items[i];
            return null;
        }
    });

    var ASPxClientIntervalBoundsCacheItem = ASPx.CreateClass(null, {
        constructor: function (interimCacheItem) {
            this.paneID = interimCacheItem.pid;
            this.intervalBoundsArray = this.CreateIntervalBoundsArray(interimCacheItem.ibl);
        },
        CreateIntervalBoundsArray: function (interimIntervalBoundsArray) {
            var result = [];
            for (var i = 0; i < interimIntervalBoundsArray.length; i++)
                result.push(new ASPxClientIntervalBounds(interimIntervalBoundsArray[i]));
            return result;
        },
        GetIntervalBoundsIndexByLocation: function (location) {
            for (var i = 0; i < this.intervalBoundsArray.length; i++)
                if (location >= this.intervalBoundsArray[i].position && location <= this.intervalBoundsArray[i].highBound)
                    return i;
            return -1;
        }
    });

    var ASPxClientIntervalBounds = ASPx.CreateClass(null, {
        constructor: function (interimBounds) {
            this.position = interimBounds.p;
            this.length = interimBounds.l;
            this.highBound = this.position + this.length;
        }
    });
    var ASPxClientStrip = ASPx.CreateClass(ASPxClientWebChartElementNamed, {
        constructor: function (chart, axis, interimStrip) {
            this.constructor.prototype.constructor.call(this, chart, interimStrip);
            this.axis = axis;
        },
        InitializeProperties: function (interimObject) {
            ASPxClientWebChartElementNamed.prototype.InitializeProperties.call(this, interimObject);
            if (ASPx.IsExists(interimObject.mi))
                this.minValue = interimObject.mi;
            if (ASPx.IsExists(interimObject.ma))
                this.maxValue = interimObject.ma;
        }
    });
    var ASPxClientConstantLine = ASPx.CreateClass(ASPxClientWebChartElementNamed, {
        constructor: function (chart, axis, interimConstantLine) {
            this.constructor.prototype.constructor.call(this, chart, interimConstantLine);
            this.axis = axis;
        },
        InitializeProperties: function (interimObject) {
            ASPxClientWebChartElementNamed.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.v))
                throw ASPxClientWebChartElement.objectModelError;
            this.value = interimObject.v;
            this.title = ASPx.IsExists(interimObject.ti) ? interimObject.ti : "";
        }
    });
    var ASPxClientSeries = ASPx.CreateClass(ASPxClientWebChartElementNamed, {
        constructor: function (chart, interimSeries) {
            this.constructor.prototype.constructor.call(this, chart, interimSeries);
        },
        InitializeProperties: function (interimObject) {
            ASPxClientWebChartElementNamed.prototype.InitializeProperties.call(this, interimObject);
            this.viewType = ASPx.IsExists(interimObject.v) ? interimObject.v : "Bar";
            this.argumentScaleType = ASPx.IsExists(interimObject.as) ? interimObject.as : "Qualitative";
            this.valueScaleType = ASPx.IsExists(interimObject.vs) ? interimObject.vs : "Numerical";
            if (ASPx.IsExists(interimObject.ax))
                this.axisX = interimObject.ax;
            if (ASPx.IsExists(interimObject.ay))
                this.axisY = interimObject.ay;
            if (ASPx.IsExists(interimObject.pa))
                this.pane = interimObject.pa;
            this.visible = !ASPx.IsExists(interimObject.nvi);
            this.toolTipEnabled = ASPx.IsExists(interimObject.tte);
            this.toolTipText = interimObject.st;
            this.toolTipImage = interimObject.tti;
            this.label = new ASPxClientSeriesLabel(this.chart, this, interimObject.l);
            this.points = this.CreatePoints(interimObject.p);
            if (ASPx.IsExists(interimObject.ti))
                this.titles = this.CreateTitles(interimObject.ti);
            if (ASPx.IsExists(interimObject.id))
                this.indicators = this.CreateIndicators(interimObject.id);
            this.regressionLines = this.indicators;
            this.trendLines = this.indicators;
            this.fibonacciIndicators = this.indicators;
            if (ASPx.IsExists(interimObject.scc)) {
                this.color = interimObject.scc;
            }
            if (ASPx.IsExists(interimObject.sg))
                this.stackedGroup = interimObject.sg;
            if (ASPx.IsExists(interimObject.clp)) {
                this.crosshairLabelPattern = interimObject.clp;
            }
            this.groupedElementsPattern = ASPx.IsExists(interimObject.gep) ? interimObject.gep : "";
            this.crosshairValueItems = this.CreateCrosshairValueItemsArray(interimObject.cvi);
            if (ASPx.IsExists(interimObject.ace)) {
                this.actualCrosshairEnabled = interimObject.ace;
            }
            if (ASPx.IsExists(interimObject.aclv)) {
                this.actualCrosshairLabelVisibility = interimObject.aclv;
            }
            if (ASPx.IsExists(interimObject.is)) {
                this.invertedStep = interimObject.is;
            }
            if (ASPx.IsExists(interimObject.axi))
                this.axisXID = interimObject.axi;
            if (ASPx.IsExists(interimObject.ayi))
                this.axisYID = interimObject.ayi;
            if (ASPx.IsExists(interimObject.pi))
                this.paneID = interimObject.pi;
        },
        CreatePoints: function (interimPoints) {
            return this.CreateArray(interimPoints, createPoint = function (chart, series, interimPoint) {
                return new ASPxClientSeriesPoint(chart, series, interimPoint);
            });
        },
        CreateCrosshairValueItemsArray: function (interimValueItems) {
            return this.CreateArray(interimValueItems, createValueItem = function (chart, series, interimValueItem) {
                return new ASPxClientCrosshairValueItem(interimValueItem);
            });
        },
        CreateTitles: function (interimTitles) {
            return this.CreateArray(interimTitles, createTitle = function (chart, series, interimTitle) {
                return new ASPxClientSeriesTitle(chart, series, interimTitle);
            });
        },
        CreateIndicators: function (interimIndicators) {
            return this.CreateArray(interimIndicators, createIndicator = function (chart, series, interimIndicator) {
                if (interimIndicator.t == "RL")
                    return new ASPxClientRegressionLine(chart, series, interimIndicator);
                if (interimIndicator.t == "TL")
                    return new ASPxClientTrendLine(chart, series, interimIndicator);
                if (interimIndicator.t == "FI")
                    return new ASPxClientFibonacciIndicator(chart, series, interimIndicator);
                if (interimIndicator.t == "SMA")
                    return new ASPxClientSimpleMovingAverage(chart, series, interimIndicator);
                if (interimIndicator.t == "EMA")
                    return new ASPxClientExponentialMovingAverage(chart, series, interimIndicator);
                if (interimIndicator.t == "WMA")
                    return new ASPxClientWeightedMovingAverage(chart, series, interimIndicator);
                if (interimIndicator.t == "TMA")
                    return new ASPxClientTriangularMovingAverage(chart, series, interimIndicator);
                if (interimIndicator.t == "TEMA")
                    return new ASPxClientTripleExponentialMovingAverageTema(chart, series, interimIndicator);
                if (interimIndicator.t == "BB")
                    return new ASPxClientBollingerBands(chart, series, interimIndicator);
                if (interimIndicator.t == "MP")
                    return new ASPxClientMedianPrice(chart, series, interimIndicator);
                if (interimIndicator.t == "TP")
                    return new ASPxClientTypicalPrice(chart, series, interimIndicator);
                if (interimIndicator.t == "WC")
                    return new ASPxClientWeightedClose(chart, series, interimIndicator);
                if (interimIndicator.t == "ATR")
                    return new ASPxClientAverageTrueRange(chart, series, interimIndicator);
                if (interimIndicator.t == "ChV")
                    return new ASPxClientChaikinsVolatility(chart, series, interimIndicator);
                if (interimIndicator.t == "CCI")
                    return new ASPxClientCommodityChannelIndex(chart, series, interimIndicator);
                if (interimIndicator.t == "DPO")
                    return new ASPxClientDetrendedPriceOscillator(chart, series, interimIndicator);
                if (interimIndicator.t == "MI")
                    return new ASPxClientMassIndex(chart, series, interimIndicator);
                if (interimIndicator.t == "MACD")
                    return new ASPxClientMovingAverageConvergenceDivergence(chart, series, interimIndicator);
                if (interimIndicator.t == "RoC")
                    return new ASPxClientRateOfChange(chart, series, interimIndicator);
                if (interimIndicator.t == "RSI")
                    return new ASPxClientRelativeStrengthIndex(chart, series, interimIndicator);
                if (interimIndicator.t == "StdDev")
                    return new ASPxClientStandardDeviation(chart, series, interimIndicator);
                if (interimIndicator.t == "TRIX")
                    return new ASPxClientTripleExponentialMovingAverageTrix(chart, series, interimIndicator);
                if (interimIndicator.t == "WR")
                    return new ASPxClientWilliamsR(chart, series, interimIndicator);
                throw ASPxClientWebChartElement.objectModelError;
            });
        }
    });
    var ASPxClientSeriesLabel = ASPx.CreateClass(ASPxClientWebChartElement, {
        constructor: function (chart, series, interimSeriesLabel) {
            this.constructor.prototype.constructor.call(this, chart, interimSeriesLabel);
            this.series = series;
        },
        InitializeProperties: function (interimObject) {
        },
        InitializeDefault: function () {
            this.text = "";
        }
    });
    var ASPxClientSeriesPoint = ASPx.CreateClass(ASPxClientWebChartRequiredElement, {
        constructor: function (chart, series, interimSeriesPoint) {
            this.constructor.prototype.constructor.call(this, chart, interimSeriesPoint);
            this.series = series;
        },
        InitializeProperties: function (interimObject) {
            if (!ASPx.IsExists(interimObject.x) || !ASPx.IsExists(interimObject.y) || !(interimObject.y instanceof Array))
                throw ASPxClientWebChartElement.objectModelError;
            this.argument = interimObject.x;
            this.values = interimObject.y;
            this.toolTipText = interimObject.pt;
            if (ASPx.IsExists(interimObject.pcc)) {
                this.color = interimObject.pcc;
            }
            if (ASPx.IsExists(interimObject.pv)) {
                this.percentValue = interimObject.pv;
            }
            if (ASPx.IsExists(interimObject.h)) {
                this.toolTipHint = interimObject.h;
            }
            if (ASPx.IsExists(interimObject.chv)) {
                this.crosshairValues = interimObject.chv;
            }
            if (ASPx.IsExists(interimObject.o)) {
                this.offset = interimObject.o;
            }
            if (ASPx.IsExists(interimObject.fo)) {
                this.fixedOffset = interimObject.fo;
            }
            if (ASPx.IsExists(interimObject.bw)) {
                this.barWidth = interimObject.bw;
            }
            if (ASPx.IsExists(interimObject.ie)) {
                this.isEmpty = interimObject.ie;
            }
        }
    });
    var ASPxClientLegend = ASPx.CreateClass(ASPxClientWebChartEmptyElement, {
        constructor: function (chart, interimLegend) {
            this.constructor.prototype.constructor.call(this, chart, interimLegend);
            this.useCheckBoxes = ASPx.IsExists(interimLegend) && ASPx.IsExists(interimLegend.chb);
        }
    });
    var ASPxClientTitleBase = ASPx.CreateClass(ASPxClientWebChartRequiredElement, {
        constructor: function (chart, interimTitle) {
            this.constructor.prototype.constructor.call(this, chart, interimTitle);
        },
        InitializeProperties: function (interimObject) {
            if (!ASPx.IsExists(interimObject.l) || !(interimObject.l instanceof Array))
                throw ASPxClientWebChartElement.objectModelError;
            this.lines = interimObject.l;
            this.alignment = interimObject.a;
            this.dock = interimObject.d;
        }
    });
    var ASPxClientChartTitle = ASPx.CreateClass(ASPxClientTitleBase, {
        constructor: function (chart, interimChartTitle) {
            this.constructor.prototype.constructor.call(this, chart, interimChartTitle);
        }
    });
    var ASPxClientSeriesTitle = ASPx.CreateClass(ASPxClientTitleBase, {
        constructor: function (chart, series, interimSeriesTitle) {
            this.constructor.prototype.constructor.call(this, chart, interimSeriesTitle);
            this.series = series;
        }
    });
    var ASPxClientIndicator = ASPx.CreateClass(ASPxClientWebChartElementNamed, {
        constructor: function (chart, series, interimIndicator) {
            this.constructor.prototype.constructor.call(this, chart, interimIndicator);
            this.series = series;
        },
        InitializeProperties: function (interimObject) {
            ASPxClientWebChartElementNamed.prototype.InitializeProperties.call(this, interimObject);
        }
    });
    var ASPxClientFinancialIndicator = ASPx.CreateClass(ASPxClientIndicator, {
        constructor: function (chart, series, interimFinancialIndicator) {
            this.constructor.prototype.constructor.call(this, chart, series, interimFinancialIndicator);
        },
        InitializeProperties: function (interimObject) {
            ASPxClientIndicator.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.p1) || !ASPx.IsExists(interimObject.p2))
                throw ASPxClientWebChartElement.objectModelError;
            this.point1 = new ASPxClientFinancialIndicatorPoint(this.chart, this, interimObject.p1);
            this.point2 = new ASPxClientFinancialIndicatorPoint(this.chart, this, interimObject.p2);
        }
    });
    var ASPxClientTrendLine = ASPx.CreateClass(ASPxClientFinancialIndicator, {
        constructor: function (chart, series, interimTrendLine) {
            this.constructor.prototype.constructor.call(this, chart, series, interimTrendLine);
        }
    });
    var ASPxClientFibonacciIndicator = ASPx.CreateClass(ASPxClientFinancialIndicator, {
        constructor: function (chart, series, interimFibonacciIndicator) {
            this.constructor.prototype.constructor.call(this, chart, series, interimFibonacciIndicator);
        }
    });
    var ASPxClientFinancialIndicatorPoint = ASPx.CreateClass(ASPxClientWebChartRequiredElement, {
        constructor: function (chart, financialIndicator, interimFinancialIndicatorPoint) {
            this.constructor.prototype.constructor.call(this, chart, interimFinancialIndicatorPoint);
            this.financialIndicator = financialIndicator;
        },
        InitializeProperties: function (interimObject) {
            if (!ASPx.IsExists(interimObject.a) || !ASPx.IsExists(interimObject.vl))
                throw ASPxClientWebChartElement.objectModelError;
            this.argument = interimObject.a;
            this.valueLevel = interimObject.vl;
        }
    });
    var ASPxClientSingleLevelIndicator = ASPx.CreateClass(ASPxClientIndicator, {
        constructor: function (chart, series, interimIndicator) {
            this.constructor.prototype.constructor.call(this, chart, series, interimIndicator);
        },
        InitializeProperties: function (interimObject) {
            ASPxClientIndicator.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.vl))
                throw ASPxClientWebChartElement.objectModelError;
            this.valueLevel = interimObject.vl;
        }
    });
    var ASPxClientRegressionLine = ASPx.CreateClass(ASPxClientSingleLevelIndicator, {
        constructor: function (chart, series, interimRegressionLine) {
            this.constructor.prototype.constructor.call(this, chart, series, interimRegressionLine);
        }
    });
    var ASPxClientMovingAverage = ASPx.CreateClass(ASPxClientSingleLevelIndicator, {
        constructor: function (chart, series, interimMovingAverage) {
            this.constructor.prototype.constructor.call(this, chart, series, interimMovingAverage);
        },
        InitializeProperties: function (interimObject) {
            ASPxClientSingleLevelIndicator.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.pc) || !ASPx.IsExists(interimObject.ki))
                throw ASPxClientWebChartElement.objectModelError;
            this.pointsCount = interimObject.pc;
            this.kind = interimObject.ki;
            if (this.kind == "Envelope" || this.kind == "MovingAverageAndEnvelope") {
                if (!ASPx.IsExists(interimObject.ep))
                    throw ASPxClientWebChartElement.objectModelError;
                this.envelopePercent = interimObject.ep;
            }
        }
    });
    var ASPxClientSimpleMovingAverage = ASPx.CreateClass(ASPxClientMovingAverage, {
        constructor: function (chart, series, interimMovingAverage) {
            this.constructor.prototype.constructor.call(this, chart, series, interimMovingAverage);
        }
    });
    var ASPxClientExponentialMovingAverage = ASPx.CreateClass(ASPxClientMovingAverage, {
        constructor: function (chart, series, interimMovingAverage) {
            this.constructor.prototype.constructor.call(this, chart, series, interimMovingAverage);
        }
    });
    var ASPxClientWeightedMovingAverage = ASPx.CreateClass(ASPxClientMovingAverage, {
        constructor: function (chart, series, interimMovingAverage) {
            this.constructor.prototype.constructor.call(this, chart, series, interimMovingAverage);
        }
    });
    var ASPxClientTriangularMovingAverage = ASPx.CreateClass(ASPxClientMovingAverage, {
        constructor: function (chart, series, interimMovingAverage) {
            this.constructor.prototype.constructor.call(this, chart, series, interimMovingAverage);
        }
    });
    var ASPxClientTripleExponentialMovingAverageTema = ASPx.CreateClass(ASPxClientMovingAverage, {
        constructor: function (chart, series, interimMovingAverage) {
            this.constructor.prototype.constructor.call(this, chart, series, interimMovingAverage);
        }
    });
    var ASPxClientBollingerBands = ASPx.CreateClass(ASPxClientIndicator, {
        constructor: function (chart, series, interimIndicator) {
            this.constructor.prototype.constructor.call(this, chart, series, interimIndicator);
        },
        InitializeProperties: function (interimObject) {
            ASPxClientIndicator.prototype.InitializeProperties.call(this, interimObject);
            if (ASPx.IsExists(interimObject.pc))
                this.pointsCount = interimObject.pc;
            if (ASPx.IsExists(interimObject.vl))
                this.valueLevel = interimObject.vl;
        }
    });
    var ASPxClientMedianPrice = ASPx.CreateClass(ASPxClientIndicator, {
        constructor: function (chart, series, interimIndicator) {
            this.constructor.prototype.constructor.call(this, chart, series, interimIndicator);
        }
    });
    var ASPxClientTypicalPrice = ASPx.CreateClass(ASPxClientIndicator, {
        constructor: function (chart, series, interimIndicator) {
            this.constructor.prototype.constructor.call(this, chart, series, interimIndicator);
        }
    });
    var ASPxClientWeightedClose = ASPx.CreateClass(ASPxClientIndicator, {
        constructor: function (chart, series, interimIndicator) {
            this.constructor.prototype.constructor.call(this, chart, series, interimIndicator);
        }
    });
    var ASPxSeparatePaneIndicator = ASPx.CreateClass(ASPxClientIndicator, {
        constructor: function (chart, series, interimMovingAverage) {
            this.constructor.prototype.constructor.call(this, chart, series, interimMovingAverage);
        },
        InitializeProperties: function (interimObject) {
            ASPxClientIndicator.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.ayi) || !ASPx.IsExists(interimObject.pi))
                throw ASPxClientWebChartElement.objectModelError;
            this.axisYID = interimObject.ayi;
            this.paneID = interimObject.pi;
            this.axisY = interimObject.ay;
            this.pane = interimObject.pa;
        }
    });
    var ASPxClientAverageTrueRange = ASPx.CreateClass(ASPxSeparatePaneIndicator, {
        constructor: function (chart, series, interimIndicator) {
            this.constructor.prototype.constructor.call(this, chart, series, interimIndicator);
        },
        InitializeProperties: function (interimObject) {
            ASPxSeparatePaneIndicator.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.pc))
                this.pointsCount = interimObject.pc;
        }
    });
    var ASPxClientChaikinsVolatility = ASPx.CreateClass(ASPxSeparatePaneIndicator, {
        constructor: function (chart, series, interimIndicator) {
            this.constructor.prototype.constructor.call(this, chart, series, interimIndicator);
        },
        InitializeProperties: function (interimObject) {
            ASPxSeparatePaneIndicator.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.pc))
                this.pointsCount = interimObject.pc;
        }
    });
    var ASPxClientCommodityChannelIndex = ASPx.CreateClass(ASPxSeparatePaneIndicator, {
        constructor: function (chart, series, interimIndicator) {
            this.constructor.prototype.constructor.call(this, chart, series, interimIndicator);
        },
        InitializeProperties: function (interimObject) {
            ASPxSeparatePaneIndicator.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.pc))
                this.pointsCount = interimObject.pc;
        }
    });
    var ASPxClientDetrendedPriceOscillator = ASPx.CreateClass(ASPxSeparatePaneIndicator, {
        constructor: function (chart, series, interimIndicator) {
            this.constructor.prototype.constructor.call(this, chart, series, interimIndicator);
        },
        InitializeProperties: function (interimObject) {
            ASPxSeparatePaneIndicator.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.pc))
                this.pointsCount = interimObject.pc;
            if (!ASPx.IsExists(interimObject.vl))
                this.valueLevel = interimObject.vl;
        }
    });
    var ASPxClientMassIndex = ASPx.CreateClass(ASPxSeparatePaneIndicator, {
        constructor: function (chart, series, interimIndicator) {
            this.constructor.prototype.constructor.call(this, chart, series, interimIndicator);
        },
        InitializeProperties: function (interimObject) {
            ASPxSeparatePaneIndicator.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.mapc))
                this.movingAveragePointsCount = interimObject.mapc;
            if (!ASPx.IsExists(interimObject.spc))
                this.sumPointsCount = interimObject.spc;
        }
    });
    var ASPxClientMovingAverageConvergenceDivergence = ASPx.CreateClass(ASPxSeparatePaneIndicator, {
        constructor: function (chart, series, interimIndicator) {
            this.constructor.prototype.constructor.call(this, chart, series, interimIndicator);
        },
        InitializeProperties: function (interimObject) {
            ASPxSeparatePaneIndicator.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.shp))
                this.shortPeriod = interimObject.shp;
            if (!ASPx.IsExists(interimObject.lp))
                this.longPeriod = interimObject.lp;
            if (!ASPx.IsExists(interimObject.ssp))
                this.signalSmoothingPeriod = interimObject.ssp;
        }
    });
    var ASPxClientRateOfChange = ASPx.CreateClass(ASPxSeparatePaneIndicator, {
        constructor: function (chart, series, interimIndicator) {
            this.constructor.prototype.constructor.call(this, chart, series, interimIndicator);
        },
        InitializeProperties: function (interimObject) {
            ASPxSeparatePaneIndicator.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.pc))
                this.pointsCount = interimObject.pc;
            if (!ASPx.IsExists(interimObject.vl))
                this.valueLevel = interimObject.vl;
        }
    });
    var ASPxClientRelativeStrengthIndex = ASPx.CreateClass(ASPxSeparatePaneIndicator, {
        constructor: function (chart, series, interimIndicator) {
            this.constructor.prototype.constructor.call(this, chart, series, interimIndicator);
        },
        InitializeProperties: function (interimObject) {
            ASPxSeparatePaneIndicator.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.pc))
                this.pointsCount = interimObject.pc;
            if (!ASPx.IsExists(interimObject.vl))
                this.valueLevel = interimObject.vl;
        }
    });
    var ASPxClientStandardDeviation = ASPx.CreateClass(ASPxSeparatePaneIndicator, {
        constructor: function (chart, series, interimIndicator) {
            this.constructor.prototype.constructor.call(this, chart, series, interimIndicator);
        },
        InitializeProperties: function (interimObject) {
            ASPxSeparatePaneIndicator.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.pc))
                this.pointsCount = interimObject.pc;
            if (!ASPx.IsExists(interimObject.vl))
                this.valueLevel = interimObject.vl;
        }
    });
    var ASPxClientTripleExponentialMovingAverageTrix = ASPx.CreateClass(ASPxSeparatePaneIndicator, {
        constructor: function (chart, series, interimIndicator) {
            this.constructor.prototype.constructor.call(this, chart, series, interimIndicator);
        },
        InitializeProperties: function (interimObject) {
            ASPxSeparatePaneIndicator.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.pc))
                this.pointsCount = interimObject.pc;
            if (!ASPx.IsExists(interimObject.vl))
                this.valueLevel = interimObject.vl;
        }
    });
    var ASPxClientWilliamsR = ASPx.CreateClass(ASPxSeparatePaneIndicator, {
        constructor: function (chart, series, interimIndicator) {
            this.constructor.prototype.constructor.call(this, chart, series, interimIndicator);
        },
        InitializeProperties: function (interimObject) {
            ASPxSeparatePaneIndicator.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.pc))
                this.pointsCount = interimObject.pc;
        }
    });
    var ASPxClientAnnotation = ASPx.CreateClass(ASPxClientWebChartElementNamed, {
        constructor: function (chart, interimAnnotation) {
            this.constructor.prototype.constructor.call(this, chart, interimAnnotation);
        },
        InitializeProperties: function (interimObject) {
            ASPxClientWebChartElementNamed.prototype.InitializeProperties.call(this, interimObject);
        }
    });
    var ASPxClientTextAnnotation = ASPx.CreateClass(ASPxClientAnnotation, {
        constructor: function (chart, interimTextAnnotation) {
            this.constructor.prototype.constructor.call(this, chart, interimTextAnnotation);
        },
        InitializeProperties: function (interimObject) {
            ASPxClientAnnotation.prototype.InitializeProperties.call(this, interimObject);
            if (!ASPx.IsExists(interimObject.l) || !(interimObject.l instanceof Array))
                throw ASPxClientWebChartElement.objectModelError;
            this.lines = interimObject.l;
        }
    });
    var ASPxClientImageAnnotation = ASPx.CreateClass(ASPxClientAnnotation, {
        constructor: function (chart, interimImageAnnotation) {
            this.constructor.prototype.constructor.call(this, chart, interimImageAnnotation);
        },
        InitializeProperties: function (interimObject) {
            ASPxClientAnnotation.prototype.InitializeProperties.call(this, interimObject);
        }
    });
    var ASPxClientCrosshairValueItem = ASPx.CreateClass(null, {
        constructor: function (interimObject) {
            this.value = interimObject.v;
            this.pointIndex = interimObject.pi;
        }
    });
    var ASPxClientToolTipController = ASPx.CreateClass(ASPxClientWebChartEmptyElement, {
        constructor: function (chart, interimToolTipController) {
            this.constructor.prototype.constructor.call(this, chart, interimToolTipController);
            this.showImage = interimToolTipController.si;
            this.showText = interimToolTipController.st;
            this.imagePosition = interimToolTipController.ip;
            this.openMode = interimToolTipController.om;
            this.toolTipObject = null;
            this.toolTip = null;
        },
        Show: function (x, y, hitInfo, htmlElementX, htmlElementY) {
            var newObject = null;
            if (hitInfo.seriesPoint != null && this.chart.showPointToolTip) {
                newObject = hitInfo.seriesPoint;
            }
            else {
                if (hitInfo.series != null && this.chart.showSeriesToolTip) {
                    newObject = hitInfo.series;
                }
            }
            if (this.toolTipObject != newObject) {
                this.toolTipObject = newObject;
                if (this.toolTipObject == null) {
                    this.Hide();
                    return;
                }
                var series = hitInfo.series != null ? hitInfo.series : hitInfo.seriesPoint.series;
                if (!series.toolTipEnabled) {
                    this.Hide();
                    return;
                }
                var showForPoint = hitInfo.inSeriesPoint && this.chart.showPointToolTip;
                var showForSeries = !hitInfo.inSeriesPoint && hitInfo.inSeries && this.chart.showSeriesToolTip;
                if (showForPoint || showForSeries) {
                    if (this.toolTip == null) {
                        this.toolTip = ASPx.CreateChartToolTip(this.chart.chartControl, ASPx.chartToolTipID, 0);
                    }
                    if (showForPoint) {
                        var image = series != null ? series.toolTipImage : null;
                        this.setToolTipContent(image, this.toolTipObject.toolTipText);
                        this.showToolTip(x, y, hitInfo, htmlElementX, htmlElementY);
                        return;
                    }
                    if (showForSeries) {
                        this.setToolTipContent(this.toolTipObject.toolTipImage, this.toolTipObject.toolTipText);
                        this.showToolTip(x, y, hitInfo, htmlElementX, htmlElementY);
                        return;
                    }
                }
                this.toolTipObject = null;
            }
        },
        Hide: function () {
            if (this.toolTip != null) {
                ASPx.RemoveChartDiv(this.chart.chartControl, ASPx.chartToolTipID, 0);
                this.toolTip = null;
                this.toolTipObject = null;
            }
        },
        getImageHTML: function (image) {
            return "<img src='" + image + "' />";
        },
        getVerticalTable: function (cell1, cell2) {
            return "<table><tr><td>" + cell1 + "</td></tr><tr><td>" + cell2 + "</td></tr></table>";
        },
        setToolTipContent: function (image, text) {
            var toolTipContent = "";
            if (this.showText && this.showImage && image != null) {
                var imageHTML = this.getImageHTML(image);
                switch (this.imagePosition) {
                    case "Right":
                        toolTipContent = ASPx.ChartGetHorizontalTable(text, imageHTML);
                        break;
                    case "Top":
                        toolTipContent = this.getVerticalTable(imageHTML, text);
                        break;
                    case "Bottom":
                        toolTipContent = this.getVerticalTable(text, imageHTML);
                        break;
                    default:
                        toolTipContent = ASPx.ChartGetHorizontalTable(imageHTML, text);
                        break;
                }
            }
            else if (this.showImage && image != null) {
                toolTipContent = this.getImageHTML(image);
            }
            else if (this.showText) {
                toolTipContent = text;
            }
            this.toolTip.innerHTML = toolTipContent;
        },
        showToolTip: function (x, y, hitInfo, htmlElementX, htmlElementY) {
            var minIndent = 5;
            var chartImage = this.chart.chartControl.GetImageElement();
            var diff = htmlElementX + chartImage.clientWidth - (x + this.toolTip.clientWidth + minIndent);
            if (diff < 0)
                x += diff;

            if (this.chart.toolTipPosition instanceof ASPxClientToolTipMousePosition) {
                ASPx.ChartSetDivPosition(this.toolTip, x, y);
                return;
            }
            if (this.chart.toolTipPosition instanceof ASPxClientToolTipFreePosition) {
                ASPx.ShowInFreePosition(this.toolTip, this.chart, this.chart.toolTipPosition, htmlElementX, htmlElementY);
                return;
            }
            if (this.chart.toolTipPosition instanceof ASPxClientToolTipRelativePosition) {
                this.showToolTipInRelativePosition(x, y, hitInfo, htmlElementX, htmlElementY);
            }
        },
        showToolTipInRelativePosition: function (x, y, hitInfo, htmlElementX, htmlElementY) {
            var left;
            var top;
            if (hitInfo.seriesPoint != null && !hitInfo.inLegend) {
                left = htmlElementX + hitInfo.seriesPoint.toolTipPoint[0] - this.toolTip.offsetWidth / 2 + this.chart.toolTipPosition.offsetX;
                top = htmlElementY + hitInfo.seriesPoint.toolTipPoint[1] - this.toolTip.offsetHeight + this.chart.toolTipPosition.offsetY;
            }
            else {
                left = x + this.chart.toolTipPosition.offsetX;
                top = y + this.chart.toolTipPosition.offsetY;
            }
            ASPx.ChartSetDivPosition(this.toolTip, left, top);
        }
    });
    var ASPxClientToolTipPosition = ASPx.CreateClass(null, {
        constructor: function (interimObject) {
        }
    });
    var ASPxClientToolTipMousePosition = ASPx.CreateClass(ASPxClientToolTipPosition, {
        constructor: function (interimObject) {
            this.constructor.prototype.constructor.call(this, interimObject);
        }
    });
    var ASPxClientToolTipRelativePosition = ASPx.CreateClass(ASPxClientToolTipPosition, {
        constructor: function (interimObject) {
            this.constructor.prototype.constructor.call(this, interimObject);
            this.offsetX = interimObject.ox;
            this.offsetY = interimObject.oy;
        }
    });
    var ASPxClientToolTipFreePosition = ASPx.CreateClass(ASPxClientToolTipPosition, {
        constructor: function (interimObject) {
            this.constructor.prototype.constructor.call(this, interimObject);
            this.offsetX = interimObject.ox;
            this.offsetY = interimObject.oy;
            this.paneID = interimObject.dt;
            this.dockPosition = interimObject.dp;
        }
    });
    var ASPxClientCrosshairPosition = ASPx.CreateClass(null, {
        constructor: function (interimObject) {
            this.offsetX = interimObject.ox;
            this.offsetY = interimObject.oy;
        }
    });
    var ASPxClientCrosshairMousePosition = ASPx.CreateClass(ASPxClientCrosshairPosition, {
        constructor: function (interimObject) {
            this.constructor.prototype.constructor.call(this, interimObject);
        }
    });
    var ASPxClientCrosshairFreePosition = ASPx.CreateClass(ASPxClientCrosshairPosition, {
        constructor: function (interimObject) {
            this.constructor.prototype.constructor.call(this, interimObject);
            this.paneID = interimObject.dt;
            this.dockPosition = interimObject.dp;
        }
    });
    var ASPxClientLineStyle = ASPx.CreateClass(ASPxClientWebChartElement, {
        constructor: function (chart, interimLineStyle) {
            this.constructor.prototype.constructor.call(this, chart, interimLineStyle);
        },
        InitializeProperties: function (interimObject) {
            this.dashStyle = interimObject.ds;
            this.thickness = interimObject.th;
            this.lineJoin = interimObject.lj;
        }
    });
    var ASPxClientCrosshairOptions = ASPx.CreateClass(ASPxClientWebChartEmptyElement, {
        constructor: function (chart, interimObject) {
            this.constructor.prototype.constructor.call(this, chart, interimObject);
            this.showArgumentLabels = interimObject.sx;
            this.showValueLabels = interimObject.sy;
            this.showCrosshairLabels = interimObject.sl;
            this.showArgumentLine = interimObject.sxl;
            this.showValueLine = interimObject.syl;
            this.showOnlyInFocusedPane = interimObject.sfp;
            this.snapMode = interimObject.sm;
            this.crosshairLabelMode = interimObject.lm;
            this.showGroupHeaders = interimObject.sgh;
            this.groupHeaderPattern = ASPx.IsExists(interimObject.ghp) ? interimObject.ghp : "";
            if (ASPx.IsExists(interimObject.clp)) {
                this.crosshairLabelPosition = this.CreateCrosshairPosition(interimObject.clp);
            }
            if (ASPx.IsExists(interimObject.alc)) {
                this.argumentLineColor = interimObject.alc;
            }
            if (ASPx.IsExists(interimObject.vlc)) {
                this.valueLineColor = interimObject.vlc;
            }
            if (ASPx.IsExists(interimObject.als)) {
                this.argumentLineStyle = new ASPxClientLineStyle(chart, interimObject.als);
            }
            if (ASPx.IsExists(interimObject.vls)) {
                this.valueLineStyle = new ASPxClientLineStyle(chart, interimObject.vls);
            }
        },
        CreateCrosshairPosition: function (interimPosition) {
            if (interimPosition.t == "CFP")
                return new ASPxClientCrosshairFreePosition(interimPosition);
            else if (interimPosition.t == "CMP")
                return new ASPxClientCrosshairMousePosition(interimPosition);
            return null;
        }
    });

    var ASPxClientAxisLabelBounds = ASPx.CreateClass(ASPxClientWebChartEmptyElement, {
        constructor: function (chart, pane, interimObject) {
            this.constructor.prototype.constructor.call(this, chart, interimObject);
            this.pane = pane;
            this.axisID = interimObject.ai;
            this.isArgumentAxis = interimObject.ax;
            this.axis = null;
            this.left = interimObject.ll;
            this.top = interimObject.lt;
            this.height = interimObject.lh;
            this.width = interimObject.lw;
        },
        GetAxis: function () {
            if (this.axis == null) {
                var diagram = this.pane.diagram;
                this.axis = this.isArgumentAxis ? diagram.FindAxisXByID(this.axisID) : diagram.FindAxisYByID(this.axisID);
            }
            return this.axis;
        }
    });

    var ASPxClientFont = ASPx.CreateClass(ASPxClientWebChartEmptyElement, {
        constructor: function (chart, interimObject) {
            this.constructor.prototype.constructor.call(this, chart, interimObject);
            this.fontSize = interimObject.fs;
            this.fontFamily = interimObject.ff;
        }
    });

    var ASPxClientCrosshairAxisLabelOptions = ASPx.CreateClass(ASPxClientWebChartEmptyElement, {
        constructor: function (chart, interimObject) {
            this.constructor.prototype.constructor.call(this, chart, interimObject);
            if (ASPx.IsExists(interimObject.clp)) {
                this.pattern = interimObject.clp;
            }
            this.visibility = interimObject.clv;
            if (ASPx.IsExists(interimObject.clbc)) {
                this.backColor = interimObject.clbc;
            }
            if (ASPx.IsExists(interimObject.cltc)) {
                this.textColor = interimObject.cltc;
            }
            if (ASPx.IsExists(interimObject.clf)) {
                this.font = new ASPxClientFont(chart, interimObject.clf);
            }
        }
    });
    var ASPxClientChartPrintOptions = ASPx.CreateClass(null, {
        constructor: function () {
            this.sizeMode = "None";
            this.landscape = false;
            this.marginLeft = "";
            this.marginTop = "";
            this.marginRight = "";
            this.marginBottom = "";
            this.paperKind = "";
            this.customPaperWidth = "";
            this.customPaperHeight = "";
            this.customPaperName = "";
        },
        createEventParameters: function () {
            return this.concatParameters([this.sizeMode, this.landscape, this.paperKind, this.customPaperName, this.customPaperWidth, this.customPaperHeight,
                this.marginLeft, this.marginTop, this.marginRight, this.marginBottom]);
        },
        concatParameters: function (options) {
            var length = options.length;
            if (length == 0)
                return "";
            var optionsString = options[0];
            for (var i = 1; i < options.length; i++) {
                optionsString = optionsString + "&" + options[i];
            }
            return optionsString;
        },
        GetSizeMode: function () {
            return this.sizeMode;
        },
        SetSizeMode: function (sizeMode) {
            this.sizeMode = sizeMode;
        },
        GetLandscape: function () {
            return this.landscape;
        },
        SetLandscape: function (landscape) {
            this.landscape = landscape;
        },
        GetMarginLeft: function () {
            return this.marginLeft;
        },
        SetMarginLeft: function (marginLeft) {
            this.marginLeft = marginLeft;
        },
        GetMarginTop: function () {
            return this.marginTop;
        },
        SetMarginTop: function (marginTop) {
            this.marginTop = marginTop;
        },
        GetMarginRight: function () {
            return this.marginRight;
        },
        SetMarginRight: function (marginRight) {
            this.marginRight = marginRight;
        },
        GetMarginBottom: function () {
            return this.marginBottom;
        },
        SetMarginBottom: function (marginBottom) {
            this.marginBottom = marginBottom;
        },
        GetPaperKind: function () {
            return this.paperKind;
        },
        SetPaperKind: function (paperKind) {
            this.paperKind = paperKind;
        },
        GetCustomPaperWidth: function () {
            return this.customPaperWidth;
        },
        SetCustomPaperWidth: function (customPaperWidth) {
            this.customPaperWidth = customPaperWidth;
        },
        GetCustomPaperHeight: function () {
            return this.customPaperHeight;
        },
        SetCustomPaperHeight: function (customPaperHeight) {
            this.customPaperHeight = customPaperHeight;
        },
        GetCustomPaperName: function () {
            return this.customPaperName;
        },
        SetCustomPaperName: function (customPaperName) {
            this.customPaperName = customPaperName;
        }
    });

    window.ASPxClientWebChartControl = ASPxClientWebChartControl;
    window.ASPxClientScaleType = ASPxClientScaleType;
    window.ASPxClientControlCoordinatesVisibility = ASPxClientControlCoordinatesVisibility;
    window.ASPxClientWebChartControlCustomDrawCrosshairEventArgs = ASPxClientWebChartControlCustomDrawCrosshairEventArgs;
    window.ASPxClientCrosshairDrawInfoList = ASPxClientCrosshairDrawInfoList;
    window.ASPxClientCrosshairDrawInfo = ASPxClientCrosshairDrawInfo;
    window.ASPxClientCrosshairElement = ASPxClientCrosshairElement;
    window.ASPxClientCrosshairLineElement = ASPxClientCrosshairLineElement;
    window.ASPxClientCrosshairAxisLabelElement = ASPxClientCrosshairAxisLabelElement;
    window.ASPxClientCrosshairGroupHeaderElement = ASPxClientCrosshairGroupHeaderElement;
    window.ASPxClientCrosshairSeriesLabelElement = ASPxClientCrosshairSeriesLabelElement;
    window.ASPxClientWebChartControlHotTrackEventArgs = ASPxClientWebChartControlHotTrackEventArgs;
    window.ASPxClientHitObject = ASPxClientHitObject;
    window.ASPxClientWebChartHitInfo = ASPxClientWebChartHitInfo;
    window.ASPxClientDiagramCoordinates = ASPxClientDiagramCoordinates;
    window.ASPxClientAxisValue = ASPxClientAxisValue;
    window.ASPxClientControlCoordinates = ASPxClientControlCoordinates;
    window.ASPxClientLegendCheckBox = ASPxClientLegendCheckBox;
    window.ASPxClientWebChartElement = ASPxClientWebChartElement;
    window.ASPxClientWebChartEmptyElement = ASPxClientWebChartEmptyElement;
    window.ASPxClientWebChartRequiredElement = ASPxClientWebChartRequiredElement;
    window.ASPxClientWebChartElementNamed = ASPxClientWebChartElementNamed;
    window.ASPxClientWebChart = ASPxClientWebChart;
    window.ASPxClientSimpleDiagram = ASPxClientSimpleDiagram;
    window.ASPxClientXYDiagramBase = ASPxClientXYDiagramBase;
    window.ASPxClientXYDiagram2D = ASPxClientXYDiagram2D;
    window.ASPxClientXYDiagram = ASPxClientXYDiagram;
    window.ASPxClientSwiftPlotDiagram = ASPxClientSwiftPlotDiagram;
    window.ASPxClientXYDiagramPane = ASPxClientXYDiagramPane;
    window.ASPxClientXYDiagram3D = ASPxClientXYDiagram3D;
    window.ASPxClientRadarDiagram = ASPxClientRadarDiagram;
    window.ASPxClientRadarDiagramMapping = ASPxClientRadarDiagramMapping;
    window.ASPxClientVertex = ASPxClientVertex;
    window.ASPxClientQualitativeMap = ASPxClientQualitativeMap;
    window.ASPxClientNumericalMap = ASPxClientNumericalMap;
    window.ASPxClientDateTimeMap = ASPxClientDateTimeMap;
    window.ASPxClientAxisBase = ASPxClientAxisBase;
    window.ASPxClientAxis2D = ASPxClientAxis2D;
    window.ASPxClientAxis = ASPxClientAxis;
    window.ASPxClientSwiftPlotDiagramAxis = ASPxClientSwiftPlotDiagramAxis;
    window.ASPxClientAxis3D = ASPxClientAxis3D;
    window.ASPxClientRadarAxis = ASPxClientRadarAxis;
    window.ASPxClientAxisTitle = ASPxClientAxisTitle;
    window.ASPxClientAxisLabelItem = ASPxClientAxisLabelItem;
    window.ASPxClientAxisRange = ASPxClientAxisRange;
    window.ASPxClientAxisInterval = ASPxClientAxisInterval;
    window.ASPxClientIntervalBoundsCache = ASPxClientIntervalBoundsCache;
    window.ASPxClientIntervalBoundsCacheItem = ASPxClientIntervalBoundsCacheItem;
    window.ASPxClientIntervalBounds = ASPxClientIntervalBounds;
    window.ASPxClientStrip = ASPxClientStrip;
    window.ASPxClientConstantLine = ASPxClientConstantLine;
    window.ASPxClientSeries = ASPxClientSeries;
    window.ASPxClientSeriesLabel = ASPxClientSeriesLabel;
    window.ASPxClientSeriesPoint = ASPxClientSeriesPoint;
    window.ASPxClientLegend = ASPxClientLegend;
    window.ASPxClientTitleBase = ASPxClientTitleBase;
    window.ASPxClientChartTitle = ASPxClientChartTitle;
    window.ASPxClientSeriesTitle = ASPxClientSeriesTitle;
    window.ASPxClientIndicator = ASPxClientIndicator;
    window.ASPxClientFinancialIndicator = ASPxClientFinancialIndicator;
    window.ASPxClientTrendLine = ASPxClientTrendLine;
    window.ASPxClientFibonacciIndicator = ASPxClientFibonacciIndicator;
    window.ASPxClientFinancialIndicatorPoint = ASPxClientFinancialIndicatorPoint;
    window.ASPxClientSingleLevelIndicator = ASPxClientSingleLevelIndicator;
    window.ASPxClientRegressionLine = ASPxClientRegressionLine;
    window.ASPxClientMovingAverage = ASPxClientMovingAverage;
    window.ASPxClientSimpleMovingAverage = ASPxClientSimpleMovingAverage;
    window.ASPxClientExponentialMovingAverage = ASPxClientExponentialMovingAverage;
    window.ASPxClientWeightedMovingAverage = ASPxClientWeightedMovingAverage;
    window.ASPxClientTriangularMovingAverage = ASPxClientTriangularMovingAverage;
    window.ASPxClientTripleExponentialMovingAverageTema = ASPxClientTripleExponentialMovingAverageTema;
    window.ASPxClientAnnotation = ASPxClientAnnotation;
    window.ASPxClientTextAnnotation = ASPxClientTextAnnotation;
    window.ASPxClientImageAnnotation = ASPxClientImageAnnotation;
    window.ASPxClientCrosshairValueItem = ASPxClientCrosshairValueItem;
    window.ASPxClientToolTipController = ASPxClientToolTipController;
    window.ASPxClientToolTipPosition = ASPxClientToolTipPosition;
    window.ASPxClientToolTipMousePosition = ASPxClientToolTipMousePosition;
    window.ASPxClientToolTipRelativePosition = ASPxClientToolTipRelativePosition;
    window.ASPxClientToolTipFreePosition = ASPxClientToolTipFreePosition;
    window.ASPxClientCrosshairPosition = ASPxClientCrosshairPosition;
    window.ASPxClientCrosshairMousePosition = ASPxClientCrosshairMousePosition;
    window.ASPxClientCrosshairFreePosition = ASPxClientCrosshairFreePosition;
    window.ASPxClientLineStyle = ASPxClientLineStyle;
    window.ASPxClientCrosshairOptions = ASPxClientCrosshairOptions;
    window.ASPxClientAxisLabelBounds = ASPxClientAxisLabelBounds;
    window.ASPxClientFont = ASPxClientFont;
    window.ASPxClientCrosshairAxisLabelOptions = ASPxClientCrosshairAxisLabelOptions;
    window.ASPxClientChartPrintOptions = ASPxClientChartPrintOptions;
})();