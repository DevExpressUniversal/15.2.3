/*! DevExpress HTML/JS Designer - v15.2.2 - 2015-12-01
* http://www.devexpress.com
* Copyright (c) 2015 Developer Express Inc; Licensed Commercial */

var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Diagram;
        (function (Diagram) {
            Diagram.name = { propertyName: "name", modelName: "@Name", displayName: "Name", editor: DevExpress.JS.Widgets.editorTemplates.text, validationRules: Designer.nameValidationRules };
            Diagram.text = { propertyName: "text", modelName: "@Text", displayName: "Text", editor: DevExpress.JS.Widgets.editorTemplates.text };
            Diagram.size = { propertyName: "size", modelName: "@Size", defaultVal: "100,50", from: Designer.Size.fromString, displayName: "Size", editor: DevExpress.JS.Widgets.editorTemplates.objecteditor };
            Diagram.location = { propertyName: "location", modelName: "@Location", from: Designer.Point.fromString, displayName: "Location", editor: DevExpress.JS.Widgets.editorTemplates.objecteditor };
            Diagram.sizeLocation = [Diagram.size, Diagram.location];
            Diagram.unknownSerializationsInfo = [Diagram.name].concat(Diagram.sizeLocation);
        })(Diagram = Designer.Diagram || (Designer.Diagram = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
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
        var Diagram;
        (function (Diagram) {
            var DiagramElementBaseViewModel = (function (_super) {
                __extends(DiagramElementBaseViewModel, _super);
                function DiagramElementBaseViewModel(control, parent, serializer) {
                    _super.call(this, control, parent, serializer);
                }
                DiagramElementBaseViewModel.prototype.getControlFactory = function () {
                    return Diagram.controlsFactory;
                };
                return DiagramElementBaseViewModel;
            })(Designer.ElementViewModel);
            Diagram.DiagramElementBaseViewModel = DiagramElementBaseViewModel;
            var DiagramElementBaseSurface = (function (_super) {
                __extends(DiagramElementBaseSurface, _super);
                function DiagramElementBaseSurface(control, context, unitProperties) {
                    _super.call(this, control, context, $.extend({}, DiagramElementBaseSurface._unitProperties, unitProperties));
                    this.template = "dx-diagram-element";
                    this.selectiontemplate = "dx-diagram-element-selection";
                    this.contenttemplate = "dx-diagram-element-content";
                    this.isSnapTarget = true;
                    this.margin = ko.observable(0);
                }
                DiagramElementBaseSurface._unitProperties = {
                    _height: function (o) {
                        return o.size.height;
                    },
                    _width: function (o) {
                        return o.size.width;
                    },
                    _x: function (o) {
                        return o.location.x;
                    },
                    _y: function (o) {
                        return o.location.y;
                    }
                };
                return DiagramElementBaseSurface;
            })(Designer.SurfaceElementBase);
            Diagram.DiagramElementBaseSurface = DiagramElementBaseSurface;
            Diagram.diagramElementSerializationInfo = [Diagram.size, Diagram.location, Diagram.name, Diagram.text, { propertyName: "type", modelName: "@Type" }];
            var DiagramElementViewModel = (function (_super) {
                __extends(DiagramElementViewModel, _super);
                function DiagramElementViewModel(control, parent, serializer) {
                    var _this = this;
                    _super.call(this, $.extend({ "@ControlType": "DiagramElement" }, control), parent, serializer);
                    this.connectingPoints = DevExpress.JS.Utils.deserializeArray(control && control.ConnectingPoints || [], function (item) {
                        return new ConnectingPointViewModel(item, _this, serializer);
                    });
                    if (this.text() === undefined) {
                        this.text(this.name());
                    }
                }
                return DiagramElementViewModel;
            })(DiagramElementBaseViewModel);
            Diagram.DiagramElementViewModel = DiagramElementViewModel;
            var DiagramElementSurface = (function (_super) {
                __extends(DiagramElementSurface, _super);
                function DiagramElementSurface(control, context) {
                    _super.call(this, control, context, null);
                    this.contenttemplate = "dxdd-element-content-with-connecting-points";
                }
                DiagramElementSurface.prototype._getChildrenHolderName = function () {
                    return "connectingPoints";
                };
                return DiagramElementSurface;
            })(DiagramElementBaseSurface);
            Diagram.DiagramElementSurface = DiagramElementSurface;
            var ConnectingPointViewModel = (function (_super) {
                __extends(ConnectingPointViewModel, _super);
                function ConnectingPointViewModel(control, parent, serializer) {
                    var _this = this;
                    _super.call(this, $.extend({ "@ControlType": "ConnectingPoint" }, control), parent, serializer);
                    this.side = ko.pureComputed(function () {
                        if (_this.percentOffsetY() >= _this.percentOffsetX()) {
                            if (_this.percentOffsetY() > 1 - _this.percentOffsetX()) {
                                return 1 /* South */;
                            }
                            else {
                                return 3 /* West */;
                            }
                        }
                        else {
                            if (_this.percentOffsetY() > 1 - _this.percentOffsetX()) {
                                return 0 /* East */;
                            }
                            else {
                                return 2 /* North */;
                            }
                        }
                    });
                    this.size = new Designer.Size(7, 7);
                    this.location = new Designer.Point(0, 0);
                    this.location.x = ko.pureComputed(function () {
                        var parentModel = _this.parentModel();
                        return parentModel.location.x() + parentModel.size.width() * _this.percentOffsetX();
                    });
                    this.location.y = ko.pureComputed(function () {
                        var parentModel = _this.parentModel();
                        return parentModel.location.y() + parentModel.size.height() * _this.percentOffsetY();
                    });
                }
                return ConnectingPointViewModel;
            })(DiagramElementBaseViewModel);
            Diagram.ConnectingPointViewModel = ConnectingPointViewModel;
            Diagram.connectingPointSerializationInfo = [
                { propertyName: "percentOffsetX", modelName: "@PercentOffsetX", defaultVal: 0.5, from: Designer.floatFromModel },
                { propertyName: "percentOffsetY", modelName: "@PercentOffsetY", defaultVal: 0.5, from: Designer.floatFromModel }
            ];
            var ConnectingPointSurface = (function (_super) {
                __extends(ConnectingPointSurface, _super);
                function ConnectingPointSurface(control, context) {
                    _super.call(this, control, context, ConnectingPointSurface._unitProperties);
                    this.template = "dxdd-connecting-point";
                    this.selectiontemplate = "dxdd-connection-point-selection";
                    this.contenttemplate = "";
                }
                ConnectingPointSurface._unitProperties = {
                    _x: function (o) {
                        return ko.pureComputed(function () {
                            return o.location.x() - o.parentModel().location.x();
                        });
                    },
                    _y: function (o) {
                        return ko.pureComputed(function () {
                            return o.location.y() - o.parentModel().location.y();
                        });
                    }
                };
                return ConnectingPointSurface;
            })(DiagramElementBaseSurface);
            Diagram.ConnectingPointSurface = ConnectingPointSurface;
        })(Diagram = Designer.Diagram || (Designer.Diagram = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Diagram;
        (function (Diagram) {
            var ConnectionPointViewModel = (function (_super) {
                __extends(ConnectionPointViewModel, _super);
                function ConnectionPointViewModel(control, parent, serializer) {
                    var _this = this;
                    _super.call(this, $.extend(control, { "@ControlType": "ConnectionPoint" }), parent, serializer);
                    var _x = this.location.x, _y = this.location.y;
                    this.location.x = ko.pureComputed({
                        read: function () {
                            return _this.connectingPoint() && _this.connectingPoint().location.x() || _x();
                        },
                        write: function (value) {
                            _this.connectingPoint(null);
                            _x(value);
                        }
                    });
                    this.location.y = ko.pureComputed({
                        read: function () {
                            return _this.connectingPoint() && _this.connectingPoint().location.y() || _y();
                        },
                        write: function (value) {
                            _this.connectingPoint(null);
                            _y(value);
                        }
                    });
                }
                return ConnectionPointViewModel;
            })(Diagram.DiagramElementBaseViewModel);
            Diagram.ConnectionPointViewModel = ConnectionPointViewModel;
            Diagram.connectionPointSerializationInfo = [
                Diagram.location,
                { propertyName: "connectingPoint", modelName: "@ConnectingPoint", link: true }
            ];
            var ConnectionPointSurface = (function (_super) {
                __extends(ConnectionPointSurface, _super);
                function ConnectionPointSurface(control, context) {
                    var _this = this;
                    _super.call(this, control, context, ConnectionPointSurface._unitProperties);
                    this.template = "dx-diagram-connection-point";
                    this.selectiontemplate = "dx-diagram-connection-point";
                    this.relativeX = ko.pureComputed(function () {
                        return _this.rect().left - _this.parent.rect().left;
                    });
                    this.relativeY = ko.pureComputed(function () {
                        return _this.rect().top - _this.parent.rect().top;
                    });
                }
                ConnectionPointSurface._unitProperties = {
                    _x: function (o) {
                        return o.location.x;
                    },
                    _y: function (o) {
                        return o.location.y;
                    }
                };
                return ConnectionPointSurface;
            })(Designer.SurfaceElementBase);
            Diagram.ConnectionPointSurface = ConnectionPointSurface;
            var ConnectorViewModel = (function (_super) {
                __extends(ConnectorViewModel, _super);
                function ConnectorViewModel(control, parent, serializer) {
                    var _this = this;
                    _super.call(this, $.extend({ "@ControlType": "Connector" }, control), parent, serializer);
                    this.startPoint(this.startPoint() || new ConnectionPointViewModel({ "@Location": "0, 0" }, this, serializer));
                    this.endPoint(this.endPoint() || new ConnectionPointViewModel({ "@Location": "150, 75" }, this, serializer));
                    this.location = new Designer.Point(0, 0);
                    this.location.x = ko.pureComputed({
                        read: function () {
                            return _this.getX();
                        },
                        write: function (value) {
                            var oldValue = _this.startPoint().location.x() < _this.endPoint().location.x() ? _this.startPoint().location.x() : _this.endPoint().location.x();
                            var delta = value - oldValue;
                            _this.startPoint().location.x(_this.startPoint().location.x() + delta);
                            _this.endPoint().location.x(_this.endPoint().location.x() + delta);
                        }
                    });
                    this.location.y = ko.pureComputed({
                        read: function () {
                            return _this.getY();
                        },
                        write: function (value) {
                            var oldValue = _this.startPoint().location.y() < _this.endPoint().location.y() ? _this.startPoint().location.y() : _this.endPoint().location.y();
                            var delta = value - oldValue;
                            _this.startPoint().location.y(_this.startPoint().location.y() + delta);
                            _this.endPoint().location.y(_this.endPoint().location.y() + delta);
                        }
                    });
                    this.size = new Designer.Size(0, 0);
                    this.size.width = ko.pureComputed({
                        read: function () {
                            return _this.getWidth();
                        },
                        write: function (value) {
                            if (_this.startPoint().location.x() < _this.endPoint().location.x()) {
                                _this.endPoint().location.x(_this.startPoint().location.x() + value);
                            }
                            else {
                                _this.startPoint().location.x(_this.endPoint().location.x() + value);
                            }
                        }
                    });
                    this.size.height = ko.pureComputed({
                        read: function () {
                            return _this.getHeight();
                        },
                        write: function (value) {
                            if (_this.startPoint().location.y() < _this.endPoint().location.y()) {
                                _this.endPoint().location.y(_this.startPoint().location.y() + value);
                            }
                            else {
                                _this.startPoint().location.y(_this.endPoint().location.y() + value);
                            }
                        }
                    });
                }
                ConnectorViewModel.prototype.getX = function () {
                    return this.startPoint().location.x() < this.endPoint().location.x() ? this.startPoint().location.x() : this.endPoint().location.x();
                };
                ConnectorViewModel.prototype.getY = function () {
                    return this.startPoint().location.y() < this.endPoint().location.y() ? this.startPoint().location.y() : this.endPoint().location.y();
                };
                ConnectorViewModel.prototype.getWidth = function () {
                    return Math.abs(this.startPoint().location.x() - this.endPoint().location.x()) || ConnectorViewModel.MIN_LINE_THICKNESS;
                };
                ConnectorViewModel.prototype.getHeight = function () {
                    return Math.abs(this.startPoint().location.y() - this.endPoint().location.y()) || ConnectorViewModel.MIN_LINE_THICKNESS;
                };
                ConnectorViewModel.MIN_LINE_THICKNESS = 3;
                return ConnectorViewModel;
            })(Diagram.DiagramElementBaseViewModel);
            Diagram.ConnectorViewModel = ConnectorViewModel;
            var ConnectorSurface = (function (_super) {
                __extends(ConnectorSurface, _super);
                function ConnectorSurface(control, context) {
                    _super.call(this, control, context, null);
                    this.template = "dxdd-connector";
                    this.selectiontemplate = "dxdd-connector-selection";
                    this.startPoint = ko.pureComputed(function () {
                        return new ConnectionPointSurface(control.startPoint(), context);
                    });
                    this.endPoint = ko.pureComputed(function () {
                        return new ConnectionPointSurface(control.endPoint(), context);
                    });
                }
                return ConnectorSurface;
            })(Diagram.DiagramElementBaseSurface);
            Diagram.ConnectorSurface = ConnectorSurface;
            (function (PointSide) {
                PointSide[PointSide["East"] = 0] = "East";
                PointSide[PointSide["South"] = 1] = "South";
                PointSide[PointSide["North"] = 2] = "North";
                PointSide[PointSide["West"] = 3] = "West";
            })(Diagram.PointSide || (Diagram.PointSide = {}));
            var PointSide = Diagram.PointSide;
            function determineConnectingPoints(startObject, endObject) {
                var result = { start: null, end: null };
                if (endObject.leftConnectionPoint.location.x() > startObject.rightConnectionPoint.location.x() + Diagram.RoutedConnectorViewModel.GRID_SIZE * 2) {
                    result.start = startObject.rightConnectionPoint;
                    result.end = endObject.leftConnectionPoint;
                }
                else if (startObject.leftConnectionPoint.location.x() > endObject.rightConnectionPoint.location.x() + Diagram.RoutedConnectorViewModel.GRID_SIZE * 2) {
                    result.start = startObject.leftConnectionPoint;
                    result.end = endObject.rightConnectionPoint;
                }
                else {
                    var startCenter = (startObject.rightConnectionPoint.location.x() + startObject.rightConnectionPoint.location.x()) / 2;
                    var endCenter = (endObject.rightConnectionPoint.location.x() + endObject.rightConnectionPoint.location.x()) / 2;
                    if (startCenter > endCenter) {
                        result.start = startObject.rightConnectionPoint;
                        result.end = endObject.rightConnectionPoint;
                    }
                    else {
                        result.start = startObject.leftConnectionPoint;
                        result.end = endObject.leftConnectionPoint;
                    }
                }
                return result;
            }
            Diagram.determineConnectingPoints = determineConnectingPoints;
            var RoutedConnectorViewModel = (function (_super) {
                __extends(RoutedConnectorViewModel, _super);
                function RoutedConnectorViewModel(control, parent, serializer) {
                    var _this = this;
                    _super.call(this, $.extend({ "@ControlType": "RoutedConnector" }, control), parent, serializer);
                    this._isUpdating = false;
                    this.routePoints = ko.observable([]);
                    this.freezeRoute = ko.observable(false);
                    this._disposables.push(ko.computed(function () {
                        var freezeRoute = !(1 + _this.startPoint().location.x() + _this.startPoint().location.y() + _this.endPoint().location.x() + _this.endPoint().location.y());
                        if (!_this._isUpdating) {
                            _this.freezeRoute(freezeRoute);
                        }
                    }));
                    this._disposables.push(ko.computed(function () {
                        if (!_this.freezeRoute()) {
                            var result = [];
                            var startPointSide = _this._getStartPointSide();
                            var endPointSide = _this._getEndPointSide();
                            var startPoint = new Designer.Point(_this.startPoint().location.x(), _this.startPoint().location.y()), endPoint = new Designer.Point(_this.endPoint().location.x(), _this.endPoint().location.y());
                            if (_this.startPoint().connectingPoint()) {
                                _this._fixPoint(startPoint, _this.startPoint().connectingPoint().side());
                                result.push(startPoint);
                            }
                            if (_this.endPoint().connectingPoint()) {
                                _this._fixPoint(endPoint, _this.endPoint().connectingPoint().side());
                            }
                            var baseX = Math.min(startPoint.x(), endPoint.x()), baseY = Math.min(startPoint.y(), endPoint.y()), width = Math.abs(startPoint.x() - endPoint.x()), height = Math.abs(startPoint.y() - endPoint.y());
                            if (startPoint.y() - endPoint.y() > 0) {
                                if (startPoint.x() - endPoint.x() > 0) {
                                    if (startPointSide === 2 /* North */ || startPointSide === 0 /* East */) {
                                        if (endPointSide === 2 /* North */ || endPointSide === 0 /* East */) {
                                            result.push(new Designer.Point(baseX + width, baseY));
                                        }
                                        else {
                                            result.push(new Designer.Point(baseX + width, baseY + height / 2));
                                            result.push(new Designer.Point(baseX, baseY + height / 2));
                                        }
                                    }
                                    else {
                                        if (endPointSide === 1 /* South */ || endPointSide === 3 /* West */) {
                                            result.push(new Designer.Point(baseX, baseY + height));
                                        }
                                        else {
                                            result.push(new Designer.Point(baseX + width / 2, baseY + height));
                                            result.push(new Designer.Point(baseX + width / 2, baseY));
                                        }
                                    }
                                }
                                else {
                                    if (startPointSide === 2 /* North */ || startPointSide === 3 /* West */) {
                                        if (endPointSide === 2 /* North */ || endPointSide === 3 /* West */) {
                                            result.push(new Designer.Point(baseX, baseY));
                                        }
                                        else {
                                            result.push(new Designer.Point(baseX, baseY + height / 2));
                                            result.push(new Designer.Point(baseX + width, baseY + height / 2));
                                        }
                                    }
                                    else {
                                        if (endPointSide === 1 /* South */ || endPointSide === 0 /* East */) {
                                            result.push(new Designer.Point(baseX + width, baseY + height));
                                        }
                                        else {
                                            result.push(new Designer.Point(baseX + width / 2, baseY + height));
                                            result.push(new Designer.Point(baseX + width / 2, baseY));
                                        }
                                    }
                                }
                            }
                            else {
                                if (startPoint.x() - endPoint.x() > 0) {
                                    if (startPointSide === 1 /* South */ || startPointSide === 0 /* East */) {
                                        if (endPointSide === 1 /* South */ || endPointSide === 0 /* East */) {
                                            result.push(new Designer.Point(baseX + width, baseY + height));
                                        }
                                        else {
                                            result.push(new Designer.Point(baseX + width, baseY + height / 2));
                                            result.push(new Designer.Point(baseX, baseY + height / 2));
                                        }
                                    }
                                    else {
                                        if (endPointSide === 2 /* North */ || endPointSide === 3 /* West */) {
                                            result.push(new Designer.Point(baseX, baseY));
                                        }
                                        else {
                                            result.push(new Designer.Point(baseX + width / 2, baseY));
                                            result.push(new Designer.Point(baseX + width / 2, baseY + height));
                                        }
                                    }
                                }
                                else {
                                    if (startPointSide === 1 /* South */ || startPointSide === 3 /* West */) {
                                        if (endPointSide === 1 /* South */ || endPointSide === 3 /* West */) {
                                            result.push(new Designer.Point(baseX, baseY + height));
                                        }
                                        else {
                                            result.push(new Designer.Point(baseX, baseY + height / 2));
                                            result.push(new Designer.Point(baseX + width, baseY + height / 2));
                                        }
                                    }
                                    else {
                                        if (endPointSide === 2 /* North */ || endPointSide === 0 /* East */) {
                                            result.push(new Designer.Point(baseX + width, baseY));
                                        }
                                        else {
                                            result.push(new Designer.Point(baseX + width / 2, baseY));
                                            result.push(new Designer.Point(baseX + width / 2, baseY + height));
                                        }
                                    }
                                }
                            }
                            if (_this.endPoint().connectingPoint()) {
                                result.push(endPoint);
                            }
                            _this.routePoints(result);
                        }
                    }));
                }
                RoutedConnectorViewModel.prototype.getX = function () {
                    var result = _super.prototype.getX.call(this);
                    this.routePoints && this.routePoints().forEach(function (point) {
                        if (point.x() < result) {
                            result = point.x();
                        }
                    });
                    return result;
                };
                RoutedConnectorViewModel.prototype.getY = function () {
                    var result = _super.prototype.getY.call(this);
                    this.routePoints && this.routePoints().forEach(function (point) {
                        if (point.y() < result) {
                            result = point.y();
                        }
                    });
                    return result;
                };
                RoutedConnectorViewModel.prototype.getWidth = function () {
                    var result = _super.prototype.getWidth.call(this);
                    var baseX = this.getX();
                    this.routePoints && [this.startPoint().location, this.endPoint().location].concat(this.routePoints()).forEach(function (point) {
                        if (point.x() - baseX > result) {
                            result = point.x() - baseX;
                        }
                    });
                    return result;
                };
                RoutedConnectorViewModel.prototype.getHeight = function () {
                    var result = _super.prototype.getHeight.call(this);
                    var baseY = this.getY();
                    this.routePoints && [this.startPoint().location, this.endPoint().location].concat(this.routePoints()).forEach(function (point) {
                        if (point.y() - baseY > result) {
                            result = point.y() - baseY;
                        }
                    });
                    return result;
                };
                RoutedConnectorViewModel.prototype._fixPoint = function (point, side) {
                    switch (side) {
                        case 2 /* North */:
                            point.y(point.y() - RoutedConnectorViewModel.GRID_SIZE);
                            break;
                        case 0 /* East */:
                            point.x(point.x() + RoutedConnectorViewModel.GRID_SIZE);
                            break;
                        case 3 /* West */:
                            point.x(point.x() - RoutedConnectorViewModel.GRID_SIZE);
                            break;
                        case 1 /* South */:
                            point.y(point.y() + RoutedConnectorViewModel.GRID_SIZE);
                    }
                };
                RoutedConnectorViewModel.prototype._getStartPointSide = function () {
                    if (this.startPoint().connectingPoint()) {
                        return this.startPoint().connectingPoint().side();
                    }
                    if (this.startPoint().location.y() !== this.endPoint().location.y()) {
                        if (this.startPoint().location.y() > this.endPoint().location.y()) {
                            return 2 /* North */;
                        }
                        else {
                            return 1 /* South */;
                        }
                    }
                    else {
                        if (this.startPoint().location.x() > this.endPoint().location.x()) {
                            return 3 /* West */;
                        }
                        else {
                            return 0 /* East */;
                        }
                    }
                };
                RoutedConnectorViewModel.prototype._getEndPointSide = function () {
                    if (this.endPoint().connectingPoint()) {
                        return this.endPoint().connectingPoint().side();
                    }
                    if (this.startPoint().location.y() !== this.endPoint().location.y()) {
                        if (this.startPoint().location.y() > this.endPoint().location.y()) {
                            return 1 /* South */;
                        }
                        else {
                            return 2 /* North */;
                        }
                    }
                    else {
                        if (this.startPoint().location.x() > this.endPoint().location.x()) {
                            return 0 /* East */;
                        }
                        else {
                            return 3 /* West */;
                        }
                    }
                };
                RoutedConnectorViewModel.prototype.beginUpdate = function () {
                    this._isUpdating = true;
                };
                RoutedConnectorViewModel.prototype.endUpdate = function () {
                    this._isUpdating = false;
                };
                RoutedConnectorViewModel.GRID_SIZE = 10;
                return RoutedConnectorViewModel;
            })(ConnectorViewModel);
            Diagram.RoutedConnectorViewModel = RoutedConnectorViewModel;
            var RoutedConnectorSurface = (function (_super) {
                __extends(RoutedConnectorSurface, _super);
                function RoutedConnectorSurface(control, context) {
                    var _this = this;
                    _super.call(this, control, context, null);
                    this.template = "dxdd-routed-connector";
                    this.selectiontemplate = "dxdd-routed-connector-selection";
                    this.showArrow = ko.observable(false);
                    this.routePoints = ko.observableArray();
                    this.routePointsSet = ko.pureComputed(function () {
                        var points = [];
                        _this.routePoints().forEach(function (point) {
                            points.push(point.x() + " " + point.y());
                        });
                        return points.join(", ");
                    });
                    this.routeLineWrappers = ko.pureComputed(function () {
                        var result = [];
                        for (var i = 1; i < _this.routePoints().length; i++) {
                            result.push(_this._createRouteLineWrapper(_this.routePoints()[i - 1], _this.routePoints()[i], i === 1 || i === _this.routePoints().length - 1));
                        }
                        return result;
                    });
                    this.startPoint = ko.pureComputed(function () {
                        return new ConnectionPointSurface(control.startPoint(), context);
                    });
                    this.endPoint = ko.pureComputed(function () {
                        return new ConnectionPointSurface(control.endPoint(), context);
                    });
                    this._disposables.push(control.routePoints.subscribe(function (routePoints) {
                        _this._updateRoutePoints();
                    }));
                    this._updateRoutePoints();
                }
                RoutedConnectorSurface.prototype._createRoutePoint = function (point, base) {
                    return {
                        x: ko.pureComputed(function () {
                            return Math.round(point.x() - base.x());
                        }),
                        y: ko.pureComputed(function () {
                            return Math.round(point.y() - base.y());
                        }),
                        modelPoint: point
                    };
                };
                RoutedConnectorSurface.prototype._createRouteLineWrapper = function (point1, point2, isLocked) {
                    if (isLocked === void 0) { isLocked = false; }
                    var _self = this, isVerticalLine = Math.abs(point1.x.peek() - point2.x.peek()) < 1, absoluteTop = point1.modelPoint.y.peek(), absoluteLeft = point1.modelPoint.x.peek(), position = {
                        top: Math.min(point1.y.peek(), point2.y.peek()) - 4,
                        left: Math.min(point1.x.peek(), point2.x.peek()) - 4,
                        width: Math.abs(point1.x.peek() - point2.x.peek()) + 6,
                        height: Math.abs(point1.y.peek() - point2.y.peek()) + 6
                    }, resizeHandler = function (params) {
                        _self._control.freezeRoute(true);
                        try {
                            _self._control.beginUpdate();
                            if (isVerticalLine) {
                                var newX = absoluteLeft + params.delta.dx;
                                point1.modelPoint.x(newX);
                                point2.modelPoint.x(newX);
                            }
                            else {
                                var newY = absoluteTop + params.delta.dy;
                                point1.modelPoint.y(newY);
                                point2.modelPoint.y(newY);
                            }
                        }
                        finally {
                            _self._control.endUpdate();
                        }
                    };
                    return {
                        position: position,
                        isVerticalLine: isVerticalLine,
                        resizeHandler: resizeHandler,
                        resizeStopped: function () {
                            _self._control.routePoints.notifySubscribers();
                        },
                        isLocked: ko.observable(isLocked)
                    };
                };
                RoutedConnectorSurface.prototype._updateRoutePoints = function () {
                    var _this = this;
                    var points = [], control = this.getControlModel(), base = control.location;
                    points.push(this._createRoutePoint(control.startPoint().location, base));
                    control.routePoints().forEach(function (point) {
                        points.push(_this._createRoutePoint(point, base));
                    });
                    points.push(this._createRoutePoint(control.endPoint().location, base));
                    this.routePoints(points);
                };
                return RoutedConnectorSurface;
            })(Diagram.DiagramElementBaseSurface);
            Diagram.RoutedConnectorSurface = RoutedConnectorSurface;
            ko.bindingHandlers["routeLineDraggable"] = {
                init: function (element, valueAccessor) {
                    var values = valueAccessor(), options = $.extend({ snap: '.dxrd-drag-snap-line', snapTolerance: Designer.SnapLinesHelper.snapTolerance }, ko.unwrap(values), {
                        start: function (event, ui) {
                            values.starting();
                        },
                        stop: function (event, ui) {
                            values.stopped();
                        },
                        drag: function (event, ui) {
                            var dx = ui.position.left - ui["originalPosition"].left, dy = ui.position.top - ui["originalPosition"].top;
                            values.forceResize({ delta: { dx: dx, dy: dy } });
                        }
                    });
                    $(element).draggable(options);
                }
            };
        })(Diagram = Designer.Diagram || (Designer.Diagram = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Diagram;
        (function (Diagram) {
            var DiagramViewModel = (function (_super) {
                __extends(DiagramViewModel, _super);
                function DiagramViewModel(diagramSource) {
                    var serializer = new DevExpress.JS.Utils.ModelSerializer();
                    _super.call(this, diagramSource, null, serializer);
                    this.controlType = "Diagram";
                    this.controls = ko.observableArray();
                    this.name("Diagram");
                }
                DiagramViewModel.prototype.getInfo = function () {
                    return Diagram.diagramSerializationsInfo;
                };
                return DiagramViewModel;
            })(Diagram.DiagramElementBaseViewModel);
            Diagram.DiagramViewModel = DiagramViewModel;
            Diagram.margins = { propertyName: "margins", modelName: "@Margins", from: Designer.Margins.fromString, displayName: "Margins" };
            Diagram.pageWidth = { propertyName: "pageWidth", modelName: "@PageWidth", defaultVal: 850, from: Designer.floatFromModel, displayName: "Page Width", editor: DevExpress.JS.Widgets.editorTemplates.numeric };
            Diagram.pageHeight = { propertyName: "pageHeight", modelName: "@PageHeight", defaultVal: 1250, from: Designer.floatFromModel, displayName: "Page Height", editor: DevExpress.JS.Widgets.editorTemplates.numeric };
            Diagram.diagramSerializationsInfo = [Diagram.name, Diagram.pageWidth, Diagram.pageHeight, Diagram.margins];
            var DiagramSurface = (function (_super) {
                __extends(DiagramSurface, _super);
                function DiagramSurface(diagram, zoom) {
                    if (zoom === void 0) { zoom = ko.observable(1); }
                    _super.call(this, diagram, {
                        measureUnit: ko.observable("Pixels"),
                        zoom: zoom,
                        dpi: ko.observable(100)
                    }, DiagramSurface._unitProperties);
                    this.measureUnit = ko.observable("Pixels");
                    this.dpi = ko.observable(100);
                    this.controls = ko.observableArray();
                    this.allowMultiselect = false;
                    this.focused = ko.observable(false);
                    this.selected = ko.observable(false);
                    this.underCursor = ko.observable(new Designer.HoverInfo());
                    this.parent = null;
                    this.templateName = "dx-diagram-surface";
                    this.margins = { bottom: this["_bottom"], left: this["_left"], right: this["_right"], top: this["_top"] };
                    this.zoom = zoom;
                    this._context = this;
                    Designer.createObservableArrayMapCollection(diagram.controls, this.controls, this._createSurface);
                }
                DiagramSurface.prototype.checkParent = function (surfaceParent) {
                    return false;
                };
                DiagramSurface.prototype.getChildrenCollection = function () {
                    return ko.observableArray([]);
                };
                DiagramSurface._unitProperties = {
                    _width: function (o) {
                        return o.pageWidth;
                    },
                    _height: function (o) {
                        return o.pageWidth;
                    },
                    pageWidth: function (o) {
                        return o.pageWidth;
                    },
                    pageHeight: function (o) {
                        return o.pageHeight;
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
                return DiagramSurface;
            })(Designer.SurfaceElementBase);
            Diagram.DiagramSurface = DiagramSurface;
        })(Diagram = Designer.Diagram || (Designer.Diagram = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Diagram;
        (function (Diagram) {
            var ConnectionPointDragHandler = (function (_super) {
                __extends(ConnectionPointDragHandler, _super);
                function ConnectionPointDragHandler(surface, selection, undoEngine, snapHelper, dragHelperContent) {
                    _super.call(this, surface, selection, undoEngine, snapHelper);
                    this.currentConnectionPoint = null;
                    this.cursor = 'arrow';
                    this.containment = '.dxrd-ghost-container';
                    this["helper"] = function (draggable) {
                        dragHelperContent.update(draggable);
                    };
                }
                ConnectionPointDragHandler.prototype.startDrag = function (control) {
                    if (!(control instanceof Diagram.ConnectionPointSurface)) {
                        throw new Error("ConnectionPointDragHandler can be applied to the ConnectionPoint only.");
                    }
                    this.currentConnectionPoint = control;
                };
                ConnectionPointDragHandler.prototype.drag = function (event, ui) {
                    ui.position.left += ui["scroll"].left;
                    ui.position.top += ui["scroll"].top;
                    var position = this._getAbsoluteSurfacePosition(ui);
                    this.currentConnectionPoint.rect({ top: position.top, left: position.left });
                };
                ConnectionPointDragHandler.prototype.doStopDrag = function () {
                    if (this.selection.dropTarget) {
                        var dropTarget = this.selection.dropTarget.getControlModel();
                        if (dropTarget instanceof Diagram.ConnectingPointViewModel) {
                            var connector = this.currentConnectionPoint.parent.getControlModel();
                            if (this.currentConnectionPoint.getControlModel() === connector.startPoint()) {
                                connector.startPoint().connectingPoint(dropTarget);
                            }
                            else {
                                connector.endPoint().connectingPoint(dropTarget);
                            }
                        }
                        else if (dropTarget instanceof Diagram.DiagramElementViewModel) {
                            var connector = this.currentConnectionPoint.parent.getControlModel();
                            var connectings = dropTarget.connectingPoints();
                            if (this.currentConnectionPoint.getControlModel() === connector.startPoint()) {
                                connector.startPoint().connectingPoint(connectings[0]);
                            }
                            else {
                                connector.endPoint().connectingPoint(connectings[0]);
                            }
                        }
                    }
                };
                return ConnectionPointDragHandler;
            })(Designer.DragDropHandler);
            Diagram.ConnectionPointDragHandler = ConnectionPointDragHandler;
            var ConnectingPointDragHandler = (function (_super) {
                __extends(ConnectingPointDragHandler, _super);
                function ConnectingPointDragHandler(surface, selection, undoEngine, snapHelper, dragHelperContent) {
                    _super.call(this, surface, selection, undoEngine, snapHelper);
                    this.startConnectingPoint = null;
                    this.newConnector = null;
                    this.cursor = 'arrow';
                    this.containment = '.dxrd-ghost-container';
                    this["helper"] = function (draggable) {
                        dragHelperContent.update(draggable);
                    };
                }
                ConnectingPointDragHandler.prototype.startDrag = function (control) {
                    if (!(control instanceof Diagram.ConnectingPointSurface)) {
                        throw new Error("ConnectingPointDragHandler can be applied to the ConnectingPoint only.");
                    }
                    this.startConnectingPoint = control;
                    var diagramElement = this.startConnectingPoint.parent.getControlModel();
                    this.newConnector = diagramElement.parentModel().createChild({ "@ControlType": "RoutedConnector" });
                    this.newConnector.startPoint().connectingPoint(this.startConnectingPoint.getControlModel());
                };
                ConnectingPointDragHandler.prototype.drag = function (event, ui) {
                    ui.position.left += ui["scroll"].left;
                    ui.position.top += ui["scroll"].top;
                    var position = this._getAbsoluteSurfacePosition(ui);
                    this.newConnectorSurface.endPoint().rect({ top: position.top, left: position.left });
                };
                ConnectingPointDragHandler.prototype.doStopDrag = function () {
                    if (this.selection.dropTarget) {
                        var dropTarget = this.selection.dropTarget.getControlModel();
                        if (dropTarget instanceof Diagram.ConnectingPointViewModel) {
                            this.newConnector.endPoint().connectingPoint(dropTarget);
                        }
                        else if (dropTarget instanceof Diagram.DiagramElementViewModel) {
                            var connectings = dropTarget.connectingPoints();
                            this.newConnector.endPoint().connectingPoint(connectings[0]);
                        }
                        this.selection.initialize(this.newConnectorSurface);
                    }
                };
                Object.defineProperty(ConnectingPointDragHandler.prototype, "newConnectorSurface", {
                    get: function () {
                        return this.newConnector && Designer.findSurface(this.newConnector);
                    },
                    enumerable: true,
                    configurable: true
                });
                return ConnectingPointDragHandler;
            })(Designer.DragDropHandler);
            Diagram.ConnectingPointDragHandler = ConnectingPointDragHandler;
        })(Diagram = Designer.Diagram || (Designer.Diagram = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var Diagram;
        (function (Diagram) {
            Diagram.controlsFactory = new Designer.ControlsFactory();
            function registerControls() {
                Diagram.controlsFactory.registerControl("Unknown", {
                    info: Diagram.unknownSerializationsInfo,
                    type: Designer.ElementViewModel,
                    nonToolboxItem: true,
                    surfaceType: Designer.SurfaceElementBase
                });
                Diagram.controlsFactory.registerControl("Connector", {
                    info: [
                        Diagram.name,
                        { propertyName: "location", displayName: "Location", editor: DevExpress.JS.Widgets.editorTemplates.objecteditor },
                        { propertyName: "startPoint", modelName: "@StartPoint", link: true },
                        { propertyName: "endPoint", modelName: "@EndPoint", link: true }
                    ],
                    surfaceType: Diagram.ConnectorSurface,
                    type: Diagram.ConnectorViewModel,
                    elementActionsTypes: [Designer.ElementActions],
                    nonToolboxItem: false
                });
                Diagram.controlsFactory.registerControl("RoutedConnector", {
                    info: [
                        Diagram.name,
                        { propertyName: "location", displayName: "Location", editor: DevExpress.JS.Widgets.editorTemplates.objecteditor },
                        { propertyName: "startPoint", modelName: "@StartPoint", link: true },
                        { propertyName: "endPoint", modelName: "@EndPoint", link: true }
                    ],
                    surfaceType: Diagram.RoutedConnectorSurface,
                    type: Diagram.RoutedConnectorViewModel,
                    elementActionsTypes: [Designer.ElementActions],
                    nonToolboxItem: false
                });
                Diagram.controlsFactory.registerControl("ConnectionPoint", {
                    info: Diagram.connectionPointSerializationInfo,
                    surfaceType: Diagram.ConnectionPointSurface,
                    type: Diagram.ConnectionPointViewModel,
                    elementActionsTypes: [],
                    nonToolboxItem: true
                });
                Diagram.controlsFactory.registerControl("Diagram", {
                    info: Diagram.diagramSerializationsInfo,
                    surfaceType: Diagram.DiagramSurface,
                    popularProperties: ["name"],
                    type: Diagram.DiagramViewModel,
                    elementActionsTypes: [],
                    isContainer: true,
                    nonToolboxItem: true
                });
                Diagram.controlsFactory.registerControl("DiagramElement", {
                    info: Diagram.diagramElementSerializationInfo,
                    defaultVal: {
                        "@SizeF": "150,50",
                        "ConnectingPoints": {
                            "Item1": {
                                "@ControlType": "ConnectingPoint",
                                "@PercentOffsetX": "1",
                                "@PercentOffsetY": "0.5",
                            },
                            "Item2": {
                                "@ControlType": "ConnectingPoint",
                                "@PercentOffsetX": "0.5",
                                "@PercentOffsetY": "1",
                            },
                            "Item3": {
                                "@ControlType": "ConnectingPoint",
                                "@PercentOffsetX": "0.5",
                                "@PercentOffsetY": "0",
                            },
                            "Item4": {
                                "@ControlType": "ConnectingPoint",
                                "@PercentOffsetX": "0",
                                "@PercentOffsetY": "0.5",
                            }
                        }
                    },
                    surfaceType: Diagram.DiagramElementSurface,
                    popularProperties: ["text"],
                    type: Diagram.DiagramElementViewModel,
                    elementActionsTypes: [Designer.ElementActions],
                    nonToolboxItem: false
                });
                Diagram.controlsFactory.registerControl("Ellipse", {
                    info: Diagram.diagramElementSerializationInfo,
                    defaultVal: {
                        "@SizeF": "150,50",
                        "@Type": "Ellipse",
                        "ConnectingPoints": {
                            "Item1": {
                                "@ControlType": "ConnectingPoint",
                                "@PercentOffsetX": "1",
                                "@PercentOffsetY": "0.5",
                            },
                            "Item2": {
                                "@ControlType": "ConnectingPoint",
                                "@PercentOffsetX": "0.5",
                                "@PercentOffsetY": "1",
                            },
                            "Item3": {
                                "@ControlType": "ConnectingPoint",
                                "@PercentOffsetX": "0.5",
                                "@PercentOffsetY": "0",
                            },
                            "Item4": {
                                "@ControlType": "ConnectingPoint",
                                "@PercentOffsetX": "0",
                                "@PercentOffsetY": "0.5",
                            }
                        }
                    },
                    surfaceType: Diagram.DiagramElementSurface,
                    popularProperties: ["text"],
                    type: Diagram.DiagramElementViewModel,
                    elementActionsTypes: [Designer.ElementActions],
                    nonToolboxItem: false
                });
                Diagram.controlsFactory.registerControl("Condition", {
                    info: Diagram.diagramElementSerializationInfo,
                    defaultVal: {
                        "@SizeF": "150,50",
                        "@Type": "Condition",
                        "ConnectingPoints": {
                            "Item1": {
                                "@ControlType": "ConnectingPoint",
                                "@PercentOffsetX": "1",
                                "@PercentOffsetY": "0.5",
                            },
                            "Item2": {
                                "@ControlType": "ConnectingPoint",
                                "@PercentOffsetX": "0.5",
                                "@PercentOffsetY": "1",
                            },
                            "Item3": {
                                "@ControlType": "ConnectingPoint",
                                "@PercentOffsetX": "0.5",
                                "@PercentOffsetY": "0",
                            },
                            "Item4": {
                                "@ControlType": "ConnectingPoint",
                                "@PercentOffsetX": "0",
                                "@PercentOffsetY": "0.5",
                            }
                        }
                    },
                    surfaceType: Diagram.DiagramElementSurface,
                    popularProperties: ["text"],
                    type: Diagram.DiagramElementViewModel,
                    elementActionsTypes: [Designer.ElementActions],
                    nonToolboxItem: false
                });
                Diagram.controlsFactory.registerControl("ConnectingPoint", {
                    info: Diagram.connectingPointSerializationInfo,
                    surfaceType: Diagram.ConnectingPointSurface,
                    type: Diagram.ConnectingPointViewModel,
                    elementActionsTypes: [],
                    nonToolboxItem: true
                });
            }
            Diagram.registerControls = registerControls;
            Diagram.groups = {
                "Appearance": [],
                "Behavior": [],
                "Design": [Diagram.name],
                "Layout": [Diagram.location, Diagram.size, Diagram.pageWidth, Diagram.pageHeight]
            };
            function createDiagramDesigner(element, diagramSource, localization) {
                if (localization) {
                    Globalize.addCultureInfo("default", {
                        messages: localization
                    });
                }
                registerControls();
                var diagram = ko.pureComputed(function () {
                    return new Diagram.DiagramViewModel(diagramSource());
                }), surface = ko.pureComputed(function () {
                    var surface = new Diagram.DiagramSurface(diagram());
                    return surface;
                });
                var designerModel = Designer.createDesigner(diagram, surface, Diagram.controlsFactory, Diagram.groups);
                designerModel.connectionPointDragHandler = new Diagram.ConnectionPointDragHandler(surface, designerModel.selection, designerModel.undoEngine, designerModel.snapHelper, designerModel.dragHelperContent);
                designerModel.connectingPointDragHandler = new Diagram.ConnectingPointDragHandler(surface, designerModel.selection, designerModel.undoEngine, designerModel.snapHelper, designerModel.dragHelperContent);
                designerModel.isLoading(false);
                designerModel.selection.focused(surface());
                $(element).children().remove();
                ko.applyBindings(designerModel, element);
                var updateSurfaceContentSize_ = Designer.updateSurfaceContentSize(designerModel.surfaceSize);
                $(window).bind("resize", function () {
                    updateSurfaceContentSize_();
                });
                designerModel.tabPanel.width.subscribe(function () {
                    updateSurfaceContentSize_();
                });
                updateSurfaceContentSize_();
                return designerModel;
            }
            Diagram.createDiagramDesigner = createDiagramDesigner;
        })(Diagram = Designer.Diagram || (Designer.Diagram = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var QueryBuilder;
        (function (QueryBuilder) {
            QueryBuilder.editorTemplates = {
                bool: { header: "dx-boolean", custom: "dxqb-property-editor" },
                combobox: { header: "dx-combobox", custom: "dxqb-property-editor" },
                text: { header: "dx-text", custom: "dxqb-property-editor" },
                filterEditor: { header: "dxrd-filterstring", custom: "dxqb-property-editor" },
                numeric: { header: "dx-numeric", custom: "dxqb-property-editor" }
            };
            QueryBuilder.name = { propertyName: "name", modelName: "@Name", displayName: "Name", disabled: true, editor: QueryBuilder.editorTemplates.text };
            QueryBuilder.alias = { propertyName: "alias", modelName: "@Alias", displayName: "Alias", defaultVal: null, editor: QueryBuilder.editorTemplates.text };
            QueryBuilder.text = { propertyName: "text", modelName: "@Text", displayName: "Text", editor: QueryBuilder.editorTemplates.text };
            QueryBuilder.selected = { propertyName: "selected", displayName: "Output", editor: QueryBuilder.editorTemplates.bool };
            QueryBuilder.size = { propertyName: "size", modelName: "@Size", defaultVal: "100,125", from: Designer.Size.fromString };
            QueryBuilder.location = { propertyName: "location", modelName: "@Location", from: Designer.Point.fromString };
            QueryBuilder.sizeLocation = [QueryBuilder.size, QueryBuilder.location];
            QueryBuilder.unknownSerializationsInfo = [QueryBuilder.name].concat(QueryBuilder.sizeLocation);
        })(QueryBuilder = Designer.QueryBuilder || (Designer.QueryBuilder = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Data;
    (function (Data) {
        function deserializeToCollection(model, types, createItem, collection) {
            var collection = collection || [];
            if (!model)
                return collection;
            types.forEach(function (type) {
                $.each(model[type] || [], function (key, item) {
                    collection.push(createItem(item));
                });
            });
            return collection;
        }
        var DBSchema = (function () {
            function DBSchema(model) {
                var tables = deserializeToCollection(model["Tables"], ["DBTable", "DBTableWithAlias"], function (tableModel) {
                    return new DBTable(tableModel);
                });
                tables.sort(function (a, b) {
                    return a.name.localeCompare(b.name);
                });
                var views = deserializeToCollection(model["Views"], ["DBTable", "DBTableWithAlias"], function (tableModel) {
                    return new DBTable(tableModel);
                });
                views.sort(function (a, b) {
                    return a.name.localeCompare(b.name);
                });
                this.tables = tables.concat(views);
                this.procedures = deserializeToCollection(model["Procedures"], ["DBStoredProcedure"], function (procModel) {
                    return new DBStoredProcedure(procModel);
                });
            }
            return DBSchema;
        })();
        Data.DBSchema = DBSchema;
        var DBTable = (function () {
            function DBTable(model) {
                this.name = model["@Name"];
                this.isView = model["@IsView"] === "true" || model["@IsView"] === true;
                this.columns = deserializeToCollection(model["Columns"], ["DBColumn", "DBColumnWithAlias"], function (columnModel) {
                    return new DBColumn(columnModel);
                });
                this.foreignKeys = deserializeToCollection(model["ForeignKeys"], ["DBForeignKey"], function (columnModel) {
                    return new DBForeignKey(columnModel);
                });
            }
            return DBTable;
        })();
        Data.DBTable = DBTable;
        var DBForeignKey = (function () {
            function DBForeignKey(model) {
                this.name = model["@Name"];
                this.primaryKeyTable = model["@PrimaryKeyTable"];
                this.column = model["Columns"]["string"];
                this.primaryKeyColumn = model["PrimaryKeyTableKeyColumns"]["string"];
            }
            return DBForeignKey;
        })();
        Data.DBForeignKey = DBForeignKey;
        var DBStoredProcedure = (function () {
            function DBStoredProcedure(model) {
                this.name = model["@Name"];
                this.arguments = deserializeToCollection(model["Arguments"], ["DBStoredProcedureArgument"], function (argModel) {
                    return new DBStoredProcedureArgument(argModel);
                });
            }
            return DBStoredProcedure;
        })();
        Data.DBStoredProcedure = DBStoredProcedure;
        var DBStoredProcedureArgument = (function () {
            function DBStoredProcedureArgument(model) {
                this.name = model["@Name"];
                this.type = model["@Type"];
                this.direction = model["@Direction"];
            }
            DBStoredProcedureArgument.directions = { in: "In", out: "Out", inOut: "InOut" };
            return DBStoredProcedureArgument;
        })();
        Data.DBStoredProcedureArgument = DBStoredProcedureArgument;
        var DBColumn = (function () {
            function DBColumn(model) {
                this.name = model["@Name"];
                this.type = model["@ColumnType"];
                this.size = model["@Size"];
            }
            DBColumn.GetType = function (dbColumnType) {
                switch (dbColumnType) {
                    case "Boolean":
                        return "System.Boolean";
                    case "Byte":
                        return "System.Byte";
                    case "SByte":
                        return "System.SByte";
                    case "Char":
                        return "System.Char";
                    case "Decimal":
                        return "System.Decimal";
                    case "Double":
                        return "System.Double";
                    case "Single":
                        return "System.Single";
                    case "Int32":
                        return "System.Int32";
                    case "UInt32":
                        return "System.UInt32";
                    case "Int16":
                        return "System.Int16";
                    case "UInt16":
                        return "System.UInt16";
                    case "Int64":
                        return "System.Int64";
                    case "UInt64":
                        return "System.UInt64";
                    case "String":
                        return "System.String";
                    case "DateTime":
                        return "System.DateTime";
                    case "Guid":
                        return "System.Guid";
                    case "TimeSpan":
                        return "System.TimeSpan";
                    case "ByteArray":
                        return "System.Byte[]";
                    default:
                        return "System.Object";
                }
            };
            return DBColumn;
        })();
        Data.DBColumn = DBColumn;
    })(Data = DevExpress.Data || (DevExpress.Data = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Data;
    (function (Data) {
        function getDBSchemaCallback(connectionString, dataSourceBase64, tableName) {
            return $.post(DevExpress.Designer.QueryBuilder.HandlerUri, {
                actionKey: 'getDBSchema',
                arg: encodeURIComponent(JSON.stringify({
                    connectionString: this._connectionString(),
                    dataSourceBase64: this._dataSource.base64(),
                    tableName: tableName
                }))
            }).fail(function (data) {
                DevExpress.Designer.NotifyAboutWarning("Schema loading failed. " + DevExpress.Designer.getErrorMessage(data), true);
            });
        }
        Data.getDBSchemaCallback = getDBSchemaCallback;
        var DBSchemaProvider = (function (_super) {
            __extends(DBSchemaProvider, _super);
            function DBSchemaProvider(_dataSource, _getDBShcemaCallBack) {
                var _this = this;
                if (_getDBShcemaCallBack === void 0) { _getDBShcemaCallBack = Data.getDBSchemaCallback; }
                _super.call(this);
                this._dataSource = _dataSource;
                this._getDBShcemaCallBack = _getDBShcemaCallBack;
                this._tables = {};
                this._connectionString = this._dataSource.connection.name;
                this._disposables.push(this._connectionString.subscribe(function () {
                    _this._tables = {};
                    _this._dbSchema = null;
                }));
                this.getItems = function (pathRequest) {
                    var result = $.Deferred();
                    if (!pathRequest.fullPath) {
                        _this.getDbSchema().done(function (dbSchema) {
                            result.resolve($.map(dbSchema.tables, function (item) {
                                var dataMemberInfo = {
                                    name: item.name,
                                    displayName: item.name,
                                    isList: false,
                                    specifics: "list",
                                    dragData: { noDragable: false }
                                };
                                return dataMemberInfo;
                            }));
                        });
                    }
                    else {
                        result.resolve([]);
                    }
                    return result.promise();
                };
            }
            DBSchemaProvider.prototype._getDBSchema = function (tableName) {
                return this._getDBShcemaCallBack(this._connectionString(), this._dataSource.base64(), tableName);
            };
            DBSchemaProvider.prototype.getDbSchema = function () {
                if (!this._dbSchema) {
                    var deferred = $.Deferred();
                    this._dbSchema = deferred.promise();
                    this._getDBSchema().done(function (response) {
                        deferred.resolve(new Data.DBSchema(JSON.parse(response.result.dbSchemaJSON)["DBSchema"]));
                    }).fail(function () {
                        deferred.reject();
                    });
                }
                return this._dbSchema;
            };
            DBSchemaProvider.prototype.getDbTable = function (tableName) {
                if (!this._tables[tableName]) {
                    var deferred = $.Deferred();
                    this._tables[tableName] = deferred.promise();
                    this._getDBSchema(tableName).done(function (response) {
                        var dbSchema = new Data.DBSchema(JSON.parse(response.result.dbSchemaJSON)["DBSchema"]);
                        deferred.resolve(dbSchema.tables[0]);
                    }).fail(function () {
                        deferred.reject();
                    });
                }
                return this._tables[tableName];
            };
            DBSchemaProvider.prototype.dataSource = function () {
                return this._dataSource;
            };
            return DBSchemaProvider;
        })(DevExpress.Designer.Disposable);
        Data.DBSchemaProvider = DBSchemaProvider;
    })(Data = DevExpress.Data || (DevExpress.Data = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var QueryBuilder;
        (function (QueryBuilder) {
            var QueryElementBaseViewModel = (function (_super) {
                __extends(QueryElementBaseViewModel, _super);
                function QueryElementBaseViewModel(control, parent, serializer) {
                    _super.call(this, control, parent, serializer);
                }
                QueryElementBaseViewModel.prototype.getControlFactory = function () {
                    return QueryBuilder.controlsFactory;
                };
                return QueryElementBaseViewModel;
            })(Designer.ElementViewModel);
            QueryBuilder.QueryElementBaseViewModel = QueryElementBaseViewModel;
            var QueryElementBaseSurface = (function (_super) {
                __extends(QueryElementBaseSurface, _super);
                function QueryElementBaseSurface(control, context, unitProperties) {
                    _super.call(this, control, context, $.extend({}, QueryElementBaseSurface._unitProperties, unitProperties));
                    this.template = "dx-diagram-element";
                    this.selectiontemplate = "dx-diagram-element-selection";
                    this.contenttemplate = "dx-diagram-element-content";
                    this.isSnapTarget = true;
                    this.margin = ko.observable(0);
                }
                QueryElementBaseSurface._unitProperties = {
                    _height: function (o) {
                        return o.size.height;
                    },
                    _width: function (o) {
                        return o.size.width;
                    },
                    _x: function (o) {
                        return o.location.x;
                    },
                    _y: function (o) {
                        return o.location.y;
                    }
                };
                return QueryElementBaseSurface;
            })(Designer.SurfaceElementBase);
            QueryBuilder.QueryElementBaseSurface = QueryElementBaseSurface;
            function findFirstItemMatchesCondition(array, predicate) {
                var result = null;
                array.some(function (value) {
                    if (predicate(value)) {
                        result = value;
                    }
                    return !!result;
                });
                return result;
            }
            QueryBuilder.findFirstItemMatchesCondition = findFirstItemMatchesCondition;
        })(QueryBuilder = Designer.QueryBuilder || (Designer.QueryBuilder = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var QueryBuilder;
        (function (QueryBuilder) {
            QueryBuilder.joinType = { propertyName: "joinType", modelName: "@Type", displayName: "Join Type", defaultVal: "Inner" };
            QueryBuilder.relationSerializationInfo = [
                { propertyName: "joinType", modelName: "@Type" },
                { propertyName: "parentTableName", modelName: "@Parent" },
                { propertyName: "nestedTableName", modelName: "@Nested" },
                { propertyName: "conditions", modelName: "KeyColumn", defaultVal: [], array: true }
            ];
            var RelationViewModel = (function (_super) {
                __extends(RelationViewModel, _super);
                function RelationViewModel(model, query, serializer) {
                    var _this = this;
                    _super.call(this, $.extend(model, { "@ControlType": "Relation" }), query, serializer);
                    this.parentTable = ko.observable(query.getTable(this.parentTableName.peek()));
                    this.nestedTable = ko.observable(query.getTable(this.nestedTableName.peek()));
                    this.parentTableName = ko.pureComputed(function () { return _this.parentTable().actualName(); });
                    this.nestedTableName = ko.pureComputed(function () { return _this.nestedTable().actualName(); });
                    this.conditions = DevExpress.JS.Utils.deserializeArray(model["KeyColumn"], function (item) {
                        return new JoinConditionViewModel(item, _this, serializer);
                    });
                }
                RelationViewModel.prototype.getInfo = function () {
                    return QueryBuilder.relationSerializationInfo;
                };
                RelationViewModel.prototype.addChild = function (control) {
                    var condition = control;
                    if (this.conditions && this.conditions.indexOf(condition) === -1) {
                        condition.parentModel(this);
                        this.conditions.push(condition);
                    }
                };
                RelationViewModel.prototype.removeChild = function (control) {
                    var index = this.conditions().indexOf(control);
                    if (index > -1)
                        this.conditions.splice(index, 1);
                    if (this.conditions().length === 0)
                        this.parentModel().removeChild(this);
                };
                return RelationViewModel;
            })(QueryBuilder.QueryElementBaseViewModel);
            QueryBuilder.RelationViewModel = RelationViewModel;
            var RelationSurface = (function (_super) {
                __extends(RelationSurface, _super);
                function RelationSurface(control, context) {
                    _super.call(this, control, context, null);
                    this.conditions = ko.observableArray();
                    this.template = "dxqb-relation";
                    Designer.createObservableArrayMapCollection(control.conditions, this.conditions, this._createSurface);
                }
                RelationSurface.prototype._getChildrenHolderName = function () {
                    return "conditions";
                };
                return RelationSurface;
            })(Designer.SurfaceElementBase);
            QueryBuilder.RelationSurface = RelationSurface;
            QueryBuilder.ConditionType = {
                Equal: "Equal",
                NotEqual: "NotEqual",
                Greater: "Greater",
                GreaterOrEqual: "GreaterOrEqual",
                Less: "Less",
                LessOrEqual: "LessOrEqual"
            };
            QueryBuilder.joinConditionSerializationInfo = [
                { propertyName: "left", displayName: "Left", editor: QueryBuilder.editorTemplates.text, disabled: true },
                { propertyName: "right", displayName: "Right", editor: QueryBuilder.editorTemplates.text, disabled: true },
                { propertyName: "parentColumnName", modelName: "@Parent" },
                { propertyName: "nestedColumnName", modelName: "@Nested" },
                {
                    propertyName: "joinType",
                    displayName: "Join Type",
                    editor: QueryBuilder.editorTemplates.combobox,
                    defaultVal: "Inner",
                    values: {
                        "Inner": "Inner",
                        "LeftOuter": "Left Outer"
                    }
                },
                {
                    propertyName: "operator",
                    modelName: "@Operator",
                    displayName: "Operator",
                    editor: QueryBuilder.editorTemplates.combobox,
                    defaultVal: QueryBuilder.ConditionType.Equal,
                    values: {
                        "Equal": "Equals to",
                        "NotEqual": "Does not equal to",
                        "Greater": "Is greater than",
                        "GreaterOrEqual": "Is greater than or equal to",
                        "Less": "Is less than",
                        "LessOrEqual": "Is less than or equal to"
                    }
                }
            ];
            var JoinConditionViewModel = (function (_super) {
                __extends(JoinConditionViewModel, _super);
                function JoinConditionViewModel(control, relation, serializer) {
                    var _this = this;
                    this.startPoint = ko.observable();
                    this.endPoint = ko.observable();
                    _super.call(this, $.extend(control, { "@ControlType": "JoinCondition" }), relation, serializer);
                    this.parentColumn = ko.pureComputed(function () { return relation.parentTable().getColumn(_this.parentColumnName()); });
                    this.nestedColumn = ko.pureComputed(function () { return relation.nestedTable().getColumn(_this.nestedColumnName()); });
                    this.joinType = relation.joinType;
                    this.left = ko.pureComputed(function () { return relation.parentTableName() + '.' + _this.parentColumnName(); });
                    this.right = ko.pureComputed(function () { return relation.nestedTableName() + '.' + _this.nestedColumnName(); });
                    this._disposables.push(ko.computed(function () {
                        if (_this.parentColumn() && _this.nestedColumn()) {
                            var result = Designer.Diagram.determineConnectingPoints(_this.parentColumn(), _this.nestedColumn());
                            _this.startPoint().connectingPoint(result.start);
                            _this.endPoint().connectingPoint(result.end);
                        }
                    }));
                }
                JoinConditionViewModel.prototype.getControlFactory = function () {
                    return QueryBuilder.controlsFactory;
                };
                return JoinConditionViewModel;
            })(Designer.Diagram.RoutedConnectorViewModel);
            QueryBuilder.JoinConditionViewModel = JoinConditionViewModel;
            var JoinConditionSurface = (function (_super) {
                __extends(JoinConditionSurface, _super);
                function JoinConditionSurface(control, context) {
                    _super.call(this, control, context);
                    this.showArrow = ko.pureComputed(function () {
                        return control.joinType() === "LeftOuter";
                    });
                }
                return JoinConditionSurface;
            })(Designer.Diagram.RoutedConnectorSurface);
            QueryBuilder.JoinConditionSurface = JoinConditionSurface;
        })(QueryBuilder = Designer.QueryBuilder || (Designer.QueryBuilder = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var QueryBuilder;
        (function (QueryBuilder) {
            QueryBuilder.tableSerializationInfo = [
                QueryBuilder.name,
                QueryBuilder.alias,
                { propertyName: "selectedColumns", modelName: "Column", array: true }
            ];
            var TableViewModel = (function (_super) {
                __extends(TableViewModel, _super);
                function TableViewModel(model, parent, serializer) {
                    var _this = this;
                    _super.call(this, model, parent, serializer);
                    this.size = new Designer.Size(135, 123);
                    this.location = new Designer.Point(0, 0);
                    this._columnsConnectionPointLeftX = ko.pureComputed(function () {
                        return _this.location.x();
                    });
                    this._columnsConnectionPointRightX = ko.pureComputed(function () {
                        return _this.location.x() + _this.size.width();
                    });
                    this._columns = ko.observableArray();
                    this._initialized = ko.observable(false);
                    this.actualName = ko.pureComputed(function () {
                        return _this.alias() || _this.name();
                    });
                    this.selectedColumns = ko.observableArray();
                    this.allColumnsSelected = ko.pureComputed({
                        read: function () {
                            var selectedColumns = _this.selectedColumns();
                            if (selectedColumns.length === 0) {
                                return false;
                            }
                            if (selectedColumns.length === _this._columns.peek().length) {
                                return true;
                            }
                            return false;
                        },
                        deferEvaluation: true
                    });
                    this.isDisabled = ko.observable(false);
                    this.isInitialized = ko.pureComputed(function () { return _this._initialized(); });
                    this._retreiveDbTableProperties(model, serializer);
                    this.size.height = ko.pureComputed({
                        read: function () {
                            return TableViewModel.COLUMNS_OFFSET + (TableViewModel.COLUMN_HEIGHT + TableViewModel.COLUMN_MARGIN) * _this._columns().length + 4;
                        },
                        write: function () {
                        }
                    });
                    this.controlType = "Table";
                }
                TableViewModel.prototype._retreiveDbTableProperties = function (model, serializer) {
                    var _this = this;
                    (this.parentModel()).dbSchemaProvider.getDbTable(this.name()).done(function (dbTable) {
                        var selectedColumns = $.isPlainObject(model["Column"]) ? $.map(model["Column"], function (item) {
                            return item;
                        }) : [];
                        dbTable.columns.forEach(function (item) {
                            var columnModel = model["Column"] ? Designer.getFirstItemByPropertyValue(selectedColumns, "@Name", item.name) : null;
                            var column = new QueryBuilder.ColumnViewModel(columnModel || { "@Name": item.name }, item, _this, serializer);
                            column.selected(!!columnModel);
                            _this._columns.push(column);
                        });
                        _this._initialized(true);
                    });
                };
                TableViewModel.prototype.columns = function () {
                    return this._columns();
                };
                TableViewModel.prototype.toggleSelectedColumns = function () {
                    var value = !this.allColumnsSelected.peek();
                    this._columns.peek().forEach(function (column) {
                        column.selected(value);
                    });
                };
                TableViewModel.prototype.getColumnConnectionPoints = function (column) {
                    var _this = this;
                    var y = ko.pureComputed({
                        read: function () {
                            var index = _this._columns.indexOf(column);
                            return _this.location.y() + TableViewModel.COLUMNS_OFFSET + TableViewModel.COLUMN_MARGIN * index + TableViewModel.COLUMN_HEIGHT * (index + 0.5);
                        },
                        deferEvaluation: true
                    });
                    return {
                        left: { x: this._columnsConnectionPointLeftX, y: y },
                        right: { x: this._columnsConnectionPointRightX, y: y }
                    };
                };
                TableViewModel.prototype.getInfo = function () {
                    return QueryBuilder.tableSerializationInfo;
                };
                TableViewModel.prototype.getColumn = function (name) {
                    return Designer.getFirstItemByPropertyValue(this._columns(), "name", name);
                };
                TableViewModel.COLUMNS_OFFSET = 55;
                TableViewModel.COLUMN_HEIGHT = 20;
                TableViewModel.COLUMN_MARGIN = 3;
                TableViewModel.TABLE_MIN_WIDTH = 80;
                return TableViewModel;
            })(QueryBuilder.QueryElementBaseViewModel);
            QueryBuilder.TableViewModel = TableViewModel;
            var TableSurface = (function (_super) {
                __extends(TableSurface, _super);
                function TableSurface(control, context) {
                    var _this = this;
                    _super.call(this, control, context, null);
                    this.contenttemplate = "dxqb-table";
                    this.toggleSelected = function () {
                        _this.getControlModel().toggleSelectedColumns();
                    };
                    this.selectedWrapper = ko.pureComputed(function () {
                        return _this.getControlModel().allColumnsSelected();
                    });
                    this.columns = ko.pureComputed(function () {
                        return control.columns().map(function (columnVewModel) {
                            return new QueryBuilder.ColumnSurface(columnVewModel, context);
                        });
                    });
                }
                return TableSurface;
            })(QueryBuilder.QueryElementBaseSurface);
            QueryBuilder.TableSurface = TableSurface;
        })(QueryBuilder = Designer.QueryBuilder || (Designer.QueryBuilder = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var QueryBuilder;
        (function (QueryBuilder) {
            var OperandPropertyQBSurface = (function (_super) {
                __extends(OperandPropertyQBSurface, _super);
                function OperandPropertyQBSurface(operator, parent, fieldListProvider, path) {
                    _super.call(this, operator, parent, fieldListProvider, path);
                }
                OperandPropertyQBSurface.prototype._updateSpecifics = function () {
                    var _this = this;
                    if (ko.unwrap(this.fieldListProvider)) {
                        var path = this.propertyName().split('.')[0];
                        ko.unwrap(this.fieldListProvider).getItems(new DevExpress.JS.Widgets.PathRequest(path)).done(function (result) {
                            var item = result.filter(function (item) {
                                return item.name === _this.propertyName();
                            })[0];
                            if (item) {
                                _this.specifics(item.specifics.toLowerCase());
                            }
                        });
                    }
                };
                return OperandPropertyQBSurface;
            })(DevExpress.JS.Widgets.OperandPropertySurface);
            QueryBuilder.OperandPropertyQBSurface = OperandPropertyQBSurface;
            var OperandParameterQBSurface = (function (_super) {
                __extends(OperandParameterQBSurface, _super);
                function OperandParameterQBSurface(operator, parent, fieldListProvider, path) {
                    var _this = this;
                    _super.call(this, operator, parent, fieldListProvider, path);
                    this.createParameter = function () {
                        _this.model.parameterName = _this.parameterName();
                        ko.unwrap(_this.fieldListProvider)["createParameter"](_this.parameterName(), _this.fieldsOptions() && _this.fieldsOptions().selected() && _this.fieldsOptions().selected()["dataType"] || "System.String");
                    };
                    this._parameterName = ko.observable("");
                    this.isEditable = ko.observable(false);
                    this._parameterName(operator.parameterName);
                    this.fieldsOptions = parent.leftPart.fieldsOptions;
                    this.parameterName = ko.pureComputed({
                        read: function () {
                            return _this._parameterName() || OperandParameterQBSurface.defaultDisplay;
                        },
                        write: function (newVal) {
                            if (newVal !== OperandParameterQBSurface.defaultDisplay) {
                                _this.model.parameterName = ko.unwrap(newVal);
                                _this._parameterName(_this.model.parameterName);
                            }
                        }
                    });
                }
                OperandParameterQBSurface.defaultDisplay = "Create new parameter";
                return OperandParameterQBSurface;
            })(DevExpress.JS.Widgets.OperandParameterSurface);
            QueryBuilder.OperandParameterQBSurface = OperandParameterQBSurface;
            var QBTablesProvider = (function () {
                function QBTablesProvider(query) {
                    this.getItems = function (pathRequest) {
                        var result = $.Deferred();
                        var items = [];
                        if (pathRequest.fullPath === "") {
                            items = query().tables().map(function (table) {
                                return {
                                    displayName: table.actualName(),
                                    name: table.actualName(),
                                    isList: true,
                                    specifics: "Default",
                                    collapsed: ko.observable(true)
                                };
                            });
                        }
                        else {
                            var table = query().tables().filter(function (t) {
                                return t.actualName() === pathRequest.fullPath;
                            })[0];
                            var columns = table && table.columns() || [];
                            items = columns.map(function (column) {
                                return {
                                    displayName: column.name.peek(),
                                    isList: false,
                                    specifics: "string",
                                    dataType: column.dataType.peek(),
                                    name: table.actualName() + "." + column.name.peek()
                                };
                            });
                        }
                        result.resolve(items);
                        return result.promise();
                    };
                    this.createParameter = function (name, dataType) {
                        if (name !== "" && name !== OperandParameterQBSurface.defaultDisplay && query().parameters().filter(function (parameter) {
                            return parameter.name() === name;
                        }).length === 0) {
                            var parameter = new DevExpress.Data.DataSourceParameter({ "@Name": name, "@Type": dataType });
                            query().parameters.push(parameter);
                        }
                    };
                }
                return QBTablesProvider;
            })();
            QueryBuilder.QBTablesProvider = QBTablesProvider;
            var QueryViewModel = (function (_super) {
                __extends(QueryViewModel, _super);
                function QueryViewModel(querySource, dbSchemaProvider, serializer) {
                    var _this = this;
                    _super.call(this, querySource, null, serializer);
                    this._findAncestorsRelations = function (table) {
                        var relations;
                        var result = { inner: 0, outer: 0, relations: [] };
                        _this.relations().forEach(function (item) {
                            if (item.nestedTable() === table) {
                                result.relations.push(item);
                                item.joinType() === "LeftOuter" ? result.outer++ : result.inner++;
                                var parentResult = _this._findAncestorsRelations(item.parentTable());
                                result.inner += parentResult.inner;
                                result.outer += parentResult.outer;
                                result.relations.push.apply(result.relations, parentResult.relations);
                            }
                        });
                        return result;
                    };
                    if (!this.name()) {
                        this.name("Query");
                    }
                    this["type"]("TableQuery");
                    this.controlType = "Query";
                    this.dbSchemaProvider = dbSchemaProvider;
                    this.filterString = new DevExpress.JS.Widgets.FilterStringOptions(this._filterString);
                    this.orderBy = DevExpress.JS.Utils.deserializeArray(querySource["OrderBy"], function (item) { return new OrderByItem(item, serializer); });
                    this.filterString.helper.handlers.changeParameter = function (criteria, popupService) {
                        return {
                            data: new DevExpress.JS.Widgets.FilterEditorAddOn(criteria, popupService, "changeParameter", "items", "dxqb-filtereditor-parameterspopup"),
                            templateName: "dxqb-filtereditor-changeparameter"
                        };
                    };
                    this.filterString.helper.mapper.Parameter = OperandParameterQBSurface;
                    this.filterString.helper.mapper.Property = OperandPropertyQBSurface;
                    this.filterString.helper.canCreateParameters = true;
                    this.filterString.helper.canSelectLists = false;
                    this.tables = DevExpress.JS.Utils.deserializeArray(querySource["Table"], function (item) {
                        return new QueryBuilder.TableViewModel(item, _this, serializer);
                    });
                    this.tables().reduce(function (posX, tableModel) {
                        tableModel.location.x(posX);
                        tableModel.location.y(65);
                        return posX + tableModel.size.width() + tableModel.size.width() / 2;
                    }, 30);
                    this.relations = DevExpress.JS.Utils.deserializeArray(querySource["Relation"], function (item) {
                        return new QueryBuilder.RelationViewModel(item, _this, serializer);
                    });
                    this.parameters = DevExpress.JS.Utils.deserializeArray(querySource["Parameter"], function (item) {
                        return new DevExpress.Data.DataSourceParameter(item, serializer);
                    });
                    this.filterString.helper.parameters = this.parameters;
                    this.pageWidth = ko.pureComputed(function () {
                        var result = 500;
                        _this.tables().forEach(function (table) {
                            var right = table.location.x() + table.size.width();
                            if (right > result) {
                                result = right;
                            }
                        });
                        return result;
                    });
                    this.pageHeight = ko.pureComputed(function () {
                        var result = 500;
                        _this.tables().forEach(function (table) {
                            var bottom = table.location.y() + table.size.height();
                            if (bottom > result) {
                                result = bottom;
                            }
                        });
                        return result;
                    });
                    this.isValid = ko.pureComputed(function () {
                        return _this._validate();
                    });
                }
                QueryViewModel.prototype.getInfo = function () {
                    return QueryBuilder.querySerializationsInfo;
                };
                QueryViewModel.prototype.createChild = function (info) {
                    if (info["@ControlType"] !== "Table") {
                        return _super.prototype.createChild.call(this, info);
                    }
                    var newControl = new QueryBuilder.TableViewModel(info, this);
                    this.addChild(newControl);
                    return newControl;
                };
                QueryViewModel.prototype.tryToCreateRelationsByFK = function (sourceTable) {
                    var _this = this;
                    if (QueryBuilder.isJoinsResolvingDisabled)
                        return;
                    this.dbSchemaProvider.getDbSchema().done(function (dbSchema) {
                        var dbSourceTable = dbSchema.tables.filter(function (item) {
                            return item.name === sourceTable.name();
                        })[0];
                        if (dbSourceTable) {
                            dbSourceTable.foreignKeys.forEach(function (fk) {
                                var pkTable = Designer.getFirstItemByPropertyValue(_this.tables.peek(), "name", fk.primaryKeyTable);
                                if (pkTable) {
                                    var column1 = Designer.getFirstItemByPropertyValue(sourceTable.columns(), "name", fk.column);
                                    var column2 = Designer.getFirstItemByPropertyValue(pkTable.columns(), "name", fk.primaryKeyColumn);
                                    if (column1 && column2) {
                                        _this.cerateJoinCondition(column1, column2);
                                    }
                                }
                            });
                        }
                        _this.tables.peek().forEach(function (table) {
                            var dbTable = dbSchema.tables.filter(function (item) {
                                return item.name === table.name();
                            })[0];
                            if (dbTable) {
                                dbTable.foreignKeys.forEach(function (fk) {
                                    if (fk.primaryKeyTable === sourceTable.name()) {
                                        var column1 = Designer.getFirstItemByPropertyValue(sourceTable.columns(), "name", fk.primaryKeyColumn);
                                        var column2 = Designer.getFirstItemByPropertyValue(table.columns(), "name", fk.column);
                                        if (column1 && column2) {
                                            _this.cerateJoinCondition(column2, column1);
                                        }
                                    }
                                });
                            }
                        });
                    });
                };
                QueryViewModel.prototype.addChild = function (control) {
                    if (control instanceof QueryBuilder.RelationViewModel) {
                        if (this.relations.indexOf(control) > -1)
                            return;
                        control.parentModel(this);
                        this.relations.push(control);
                    }
                    else if (control instanceof QueryBuilder.TableViewModel) {
                        if (this.tables.indexOf(control) > -1)
                            return;
                        control.parentModel(this);
                        if (Designer.getFirstItemByPropertyValue(this.tables(), "actualName", control.name()) !== null) {
                            control.alias(Designer.getUniqueName(this.tables().map(function (table) {
                                return table.actualName();
                            }), control.name() + '_'));
                        }
                        this.tables.push(control);
                    }
                    else {
                        Designer.NotifyAboutWarning("Attempt to add wrong child control.");
                    }
                };
                QueryViewModel.prototype.removeChild = function (control) {
                    if (control instanceof QueryBuilder.RelationViewModel) {
                        if (this.relations().length < 1)
                            return;
                        this.relations.splice(this.relations().indexOf(control), 1);
                    }
                    else if (control instanceof QueryBuilder.TableViewModel) {
                        if (this.tables().length < 1)
                            return;
                        this.tables.splice(this.tables().indexOf(control), 1);
                        var relations = this.relations();
                        for (var i = relations.length - 1; i > -1; i--) {
                            if (relations[i].parentTable() === control || relations[i].nestedTable() === control) {
                                this.removeChild(relations[i]);
                            }
                        }
                        for (var i = this.orderBy().length - 1; i > -1; i--) {
                            if (this.orderBy()[i].table() === control.name()) {
                                this.orderBy.remove(this.orderBy()[i]);
                            }
                        }
                        for (var i = this.groupBy().length - 1; i > -1; i--) {
                            if (this.groupBy()[i].table() === control.name()) {
                                this.groupBy.remove(this.groupBy()[i]);
                            }
                        }
                    }
                    else {
                        Designer.NotifyAboutWarning("Attempt to remove wrong child control.");
                    }
                };
                QueryViewModel.prototype.getTable = function (name) {
                    return Designer.getFirstItemByPropertyValue(this.tables(), "actualName", name);
                };
                QueryViewModel.prototype._findTableInAncestors = function (child, probablyAncestor) {
                    var _this = this;
                    return this.relations().some(function (relation) {
                        return relation.nestedTable() === child && (relation.parentTable() === probablyAncestor || _this._findTableInAncestors(relation.parentTable(), probablyAncestor));
                    });
                };
                QueryViewModel.prototype._findHead = function (table) {
                    var result = null;
                    this.relations().some(function (relation) {
                        if (relation.nestedTable() === table)
                            result = relation;
                        return !!result;
                    });
                    return result ? this._findHead(result.parentTable()) : table;
                };
                QueryViewModel.prototype._isHead = function (table) {
                    return !this.relations().some(function (relation) { return relation.nestedTable() === table; });
                };
                QueryViewModel.prototype._reverseRelations = function (table, relationsToReverse) {
                    relationsToReverse.forEach(function (item) {
                        var tempTable = item.parentTable();
                        item.parentTable(item.nestedTable());
                        item.nestedTable(tempTable);
                        item.conditions().forEach(function (condition) {
                            var tempColumn = condition.parentColumnName();
                            condition.parentColumnName(condition.nestedColumnName());
                            condition.nestedColumnName(tempColumn);
                        });
                    });
                };
                QueryViewModel.prototype.cerateJoinCondition = function (parentColumn, nestedColumn) {
                    var parentTable = parentColumn.parentModel();
                    var nestedTable = nestedColumn.parentModel();
                    if (parentTable === nestedTable)
                        return null;
                    var isColumnsReplaced = false;
                    var relation = QueryBuilder.findFirstItemMatchesCondition(this.relations(), function (relation) {
                        isColumnsReplaced = relation.parentTable() === nestedTable && relation.nestedTable() === parentTable;
                        return relation.parentTable() === parentTable && relation.nestedTable() === nestedTable || isColumnsReplaced;
                    });
                    if (relation) {
                    }
                    else if (this._findTableInAncestors(parentTable, nestedTable)) {
                        isColumnsReplaced = true;
                    }
                    else if (this._findHead(parentTable) !== this._findHead(nestedTable) && !this._isHead(nestedTable)) {
                        var parentRelations = this._findAncestorsRelations(parentTable);
                        var nestedRelations = this._findAncestorsRelations(nestedTable);
                        if (parentRelations.outer > nestedRelations.outer) {
                            this._reverseRelations(nestedTable, nestedRelations.relations);
                        }
                        else if (parentRelations.outer < nestedRelations.outer) {
                            this._reverseRelations(parentTable, parentRelations.relations);
                            isColumnsReplaced = true;
                        }
                        else if (parentRelations.inner >= nestedRelations.inner) {
                            this._reverseRelations(nestedTable, nestedRelations.relations);
                        }
                        else if (parentRelations.inner < nestedRelations.inner) {
                            this._reverseRelations(parentTable, parentRelations.relations);
                            isColumnsReplaced = true;
                        }
                    }
                    if (isColumnsReplaced) {
                        var tempTable = parentTable;
                        parentTable = nestedTable;
                        nestedTable = tempTable;
                        var tempColumn = parentColumn;
                        parentColumn = nestedColumn;
                        nestedColumn = tempColumn;
                    }
                    relation = relation || this.createChild({
                        "@ControlType": "Relation",
                        "@Parent": parentTable.actualName(),
                        "@Nested": nestedTable.actualName(),
                        "@Type": "Inner",
                        "KeyColumn": []
                    });
                    var joinCondition = QueryBuilder.findFirstItemMatchesCondition(relation.conditions(), function (condition) {
                        return condition.parentColumn() === parentColumn && condition.nestedColumn() === nestedColumn;
                    });
                    if (!joinCondition) {
                        joinCondition = relation.createChild({ "@ControlType": "JoinCondition", "@Parent": parentColumn.name(), "@Nested": nestedColumn.name() });
                    }
                    return joinCondition;
                };
                QueryViewModel.prototype._validate = function () {
                    if (this.tables().length === 0)
                        return false;
                    var tables = this.tables().map(function (table) { return table.actualName(); });
                    this._validateTable(tables, tables[0]);
                    return tables.length < 1;
                };
                QueryViewModel.prototype._validateTable = function (tables, tableName) {
                    var _this = this;
                    var index = tables.indexOf(tableName);
                    if (index < 0)
                        return;
                    tables.splice(index, 1);
                    var connectedTables = this.relations().map(function (relation) {
                        if (relation.parentTableName() === tableName)
                            return relation.nestedTableName();
                        if (relation.nestedTableName() === tableName)
                            return relation.parentTableName();
                        return null;
                    });
                    connectedTables.forEach(function (item) { return _this._validateTable(tables, item); });
                };
                QueryViewModel.prototype.serialize = function (includeRootTag) {
                    if (includeRootTag === void 0) { includeRootTag = false; }
                    return includeRootTag ? { "Query": this.serialize() } : (new DevExpress.JS.Utils.ModelSerializer()).serialize(this);
                };
                QueryViewModel.prototype.save = function () {
                    var data = this.serialize(true);
                    if (this.onSave) {
                        this.onSave(data);
                    }
                    return data;
                };
                return QueryViewModel;
            })(QueryBuilder.QueryElementBaseViewModel);
            QueryBuilder.QueryViewModel = QueryViewModel;
            QueryBuilder.margins = { propertyName: "margins", modelName: "@Margins", from: Designer.Margins.fromString };
            QueryBuilder.querySerializationsInfo = [
                QueryBuilder.margins,
                { propertyName: "tables", modelName: "Table", array: true },
                { propertyName: "relations", modelName: "Relation", array: true },
                { propertyName: "parameters", modelName: "Parameter", array: true },
                { propertyName: "type", modelName: "@Type" },
                { propertyName: "name", modelName: "@Name", displayName: "Name", editor: QueryBuilder.editorTemplates.text },
                { propertyName: "_filterString", modelName: "Filter", defaultVal: "" },
                { propertyName: "filterString", defaultVal: "", displayName: "Filter", editor: QueryBuilder.editorTemplates.filterEditor },
                { propertyName: "orderBy", modelName: "OrderBy", array: true },
                {
                    propertyName: "groupBy",
                    modelName: "GroupBy",
                    array: true,
                    info: [
                        { propertyName: "table", modelName: "@Table" },
                        { propertyName: "column", modelName: "@Column" }
                    ]
                },
            ];
            var QuerySurface = (function (_super) {
                __extends(QuerySurface, _super);
                function QuerySurface(query, zoom) {
                    if (zoom === void 0) { zoom = ko.observable(1); }
                    _super.call(this, query, {
                        measureUnit: ko.observable("Pixels"),
                        zoom: zoom,
                        dpi: ko.observable(100)
                    }, QuerySurface._unitProperties);
                    this.tables = ko.observableArray();
                    this.relations = ko.observableArray();
                    this.allowMultiselect = false;
                    this.focused = ko.observable(false);
                    this.selected = ko.observable(false);
                    this.underCursor = ko.observable(new Designer.HoverInfo());
                    this.templateName = "dx-query-surface";
                    this.measureUnit = this._context.measureUnit;
                    this.dpi = this._context.dpi;
                    this._context = this;
                    this.margins = { bottom: this["_bottom"], left: this["_left"], right: this["_right"], top: this["_top"] };
                    this.zoom = zoom;
                    Designer.createObservableArrayMapCollection(query.tables, this.tables, this._createSurface);
                    Designer.createObservableArrayMapCollection(query.relations, this.relations, this._createSurface);
                }
                QuerySurface.prototype.checkParent = function (surfaceParent) {
                    return false;
                };
                QuerySurface.prototype.getChildrenCollection = function () {
                    return this.tables;
                };
                QuerySurface._unitProperties = {
                    _width: function (o) {
                        return o.pageWidth;
                    },
                    _height: function (o) {
                        return o.pageWidth;
                    },
                    pageWidth: function (o) {
                        return o.pageWidth;
                    },
                    pageHeight: function (o) {
                        return o.pageHeight;
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
                return QuerySurface;
            })(Designer.SurfaceElementBase);
            QueryBuilder.QuerySurface = QuerySurface;
            QueryBuilder.orderByItemSerializationsInfo = [
                { propertyName: "table", modelName: "@Table" },
                { propertyName: "column", modelName: "@Column" },
                { propertyName: "descending", modelName: "@Descending", defaultVal: false }
            ];
            var OrderByItem = (function () {
                function OrderByItem(model, serializer) {
                    serializer = serializer || new DevExpress.JS.Utils.ModelSerializer();
                    serializer.deserialize(this, model);
                }
                OrderByItem.prototype.getInfo = function () {
                    return QueryBuilder.orderByItemSerializationsInfo;
                };
                return OrderByItem;
            })();
            QueryBuilder.OrderByItem = OrderByItem;
        })(QueryBuilder = Designer.QueryBuilder || (Designer.QueryBuilder = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var QueryBuilder;
        (function (QueryBuilder) {
            var ColumnDragHandler = (function (_super) {
                __extends(ColumnDragHandler, _super);
                function ColumnDragHandler(surface, selection, undoEngine, snapHelper, dragHelperContent) {
                    _super.call(this, surface, selection, undoEngine, snapHelper);
                    this._dragColumn = null;
                    this.dragDropConnector = ko.observable(null);
                    this.cursor = 'arrow';
                    this.containment = '.dxrd-ghost-container';
                    this["helper"] = function (draggable) {
                        dragHelperContent.update(draggable);
                    };
                }
                ColumnDragHandler.prototype._needToCreateRelation = function () {
                    if (!(this.selection.dropTarget && this.selection.dropTarget instanceof QueryBuilder.ColumnSurface))
                        return false;
                    var table = this.selection.dropTarget.getControlModel().parentModel();
                    return !table.isDisabled();
                };
                ColumnDragHandler.prototype.startDrag = function (control) {
                    if (!(control instanceof QueryBuilder.ColumnSurface)) {
                        throw new Error("ColumnDragHandler can be applied to the Column only.");
                    }
                    this._dragColumn = control;
                    var connectorModel = new Designer.Diagram.RoutedConnectorViewModel({}, this._dragColumn.getControlModel().root);
                    this.dragDropConnector(new Designer.Diagram.RoutedConnectorSurface(connectorModel, this.surface()));
                };
                ColumnDragHandler.prototype.setConnectorPoints = function (cursorPosition) {
                    var startColumn = this._dragColumn.getControlModel(), connectorModel = this.dragDropConnector().getControlModel();
                    if (this._needToCreateRelation()) {
                        var points = Designer.Diagram.determineConnectingPoints(startColumn, this.selection.dropTarget.getControlModel());
                        connectorModel.startPoint().connectingPoint(points.start);
                        connectorModel.endPoint().connectingPoint(points.end);
                    }
                    else {
                        var point = Math.abs(startColumn.leftConnectionPoint.location.x() - cursorPosition.left) > Math.abs(startColumn.rightConnectionPoint.location.x() - cursorPosition.left) ? startColumn.rightConnectionPoint : startColumn.leftConnectionPoint;
                        connectorModel.startPoint().connectingPoint(point);
                        this.dragDropConnector().endPoint().rect({ top: cursorPosition.top, left: cursorPosition.left });
                    }
                };
                ColumnDragHandler.prototype.drag = function (event, ui) {
                    ui.position.left += ui["scroll"].left;
                    ui.position.top += ui["scroll"].top;
                    this.setConnectorPoints(this._getAbsoluteSurfacePosition(ui));
                };
                ColumnDragHandler.prototype.doStopDrag = function () {
                    this.dragDropConnector(null);
                    var parentColumn = this._dragColumn.getControlModel(), query = parentColumn.root;
                    if (this._needToCreateRelation()) {
                        var nestedColumn = this.selection.dropTarget.getControlModel();
                        var condition = query.cerateJoinCondition(parentColumn, nestedColumn);
                        if (condition !== null) {
                            this.selection.initialize(Designer.findSurface(query.cerateJoinCondition(parentColumn, nestedColumn)));
                        }
                    }
                    ColumnDragHandler.resetTablesDisabledState(query);
                };
                ColumnDragHandler.prototype.dragColumn = function () {
                    return this._dragColumn;
                };
                ColumnDragHandler.setDisableStateForTables = function (query, startTable) {
                    startTable.isDisabled(true);
                    if (query.relations().length === 0)
                        return;
                    var tablesWithRelations = [];
                    query.relations().forEach(function (relation) {
                        if (tablesWithRelations.indexOf(relation.parentTable) < 0)
                            tablesWithRelations.push(relation.parentTable);
                        if (tablesWithRelations.indexOf(relation.nestedTable) < 0)
                            tablesWithRelations.push(relation.nestedTable);
                    });
                    if (tablesWithRelations.indexOf(startTable) > -1)
                        return;
                    query.tables().forEach(function (table) {
                        if (tablesWithRelations.indexOf(table) < 0)
                            table.isDisabled(true);
                    });
                };
                ColumnDragHandler.resetTablesDisabledState = function (query) {
                    query.tables().forEach(function (table) {
                        table.isDisabled(false);
                    });
                };
                return ColumnDragHandler;
            })(Designer.DragDropHandler);
            QueryBuilder.ColumnDragHandler = ColumnDragHandler;
            var DbObjectDragDropHandler = (function (_super) {
                __extends(DbObjectDragDropHandler, _super);
                function DbObjectDragDropHandler(surface, selection, _undoEngine, snapHelper, dragHelperContent) {
                    var _this = this;
                    _super.call(this, surface, selection, _undoEngine, snapHelper);
                    this._undoEngine = _undoEngine;
                    this._querySurface = surface;
                    this.cursor = 'arrow';
                    this.containment = '.dxrd-designer';
                    this["cursorAt"] = {
                        top: 0,
                        left: 0
                    };
                    this["helper"] = function (draggable) {
                        _super.prototype.helper.call(_this, draggable);
                        var key = draggable.data.isList ? "List" : draggable.data.specifics;
                        _this.recalculateSize(Designer.Size.fromString("135, 123"));
                        dragHelperContent.setContent(new Designer.Rectangle(0, 0, _this._size.width(), _this._size.height()));
                    };
                    this._drop = function (memberInfo, query) {
                        var newControl = query.createChild($.extend({ "@ControlType": "Table", "@Name": memberInfo.name }, QueryBuilder.controlsFactory.controlsMap["Table"].defaultVal));
                        if (newControl.isInitialized()) {
                            query.tryToCreateRelationsByFK(newControl);
                        }
                        else {
                            newControl.isInitialized.subscribe(function () {
                                _this._undoEngine().start();
                                query.tryToCreateRelationsByFK(newControl);
                                _this._undoEngine().end();
                            });
                        }
                        return newControl;
                    };
                }
                DbObjectDragDropHandler.prototype.doStopDrag = function (ui, draggable) {
                    if (this._querySurface().underCursor() && this._querySurface().underCursor().isOver) {
                        var position = this._getAbsoluteSurfacePosition(ui);
                        this._querySurface().underCursor().x = position.left - this._querySurface()["absolutePosition"].x();
                        this._querySurface().underCursor().y = position.top - this._querySurface()["absolutePosition"].y();
                        var item = draggable;
                        var key = draggable.data.isList ? "List" : draggable.data.specifics;
                        var control = this._drop(item.data, this._querySurface().getControlModel());
                        this.addControl(control, this._querySurface(), this._size);
                    }
                };
                DbObjectDragDropHandler.prototype.addControl = function (control, dropTargetSurface, size) {
                    dropTargetSurface.getControlModel().addChild(control);
                    var controlSurface = Designer.findSurface(control);
                    controlSurface.rect({ left: dropTargetSurface.underCursor().x, top: dropTargetSurface.underCursor().y, width: size.width() });
                    this.selection.initialize(controlSurface);
                };
                return DbObjectDragDropHandler;
            })(Designer.DragDropHandler);
            QueryBuilder.DbObjectDragDropHandler = DbObjectDragDropHandler;
        })(QueryBuilder = Designer.QueryBuilder || (Designer.QueryBuilder = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var QueryBuilder;
        (function (QueryBuilder) {
            QueryBuilder.controlsFactory = new Designer.ControlsFactory();
            function registerControls() {
                Designer.Diagram.registerControls();
                QueryBuilder.controlsFactory.registerControl("Unknown", {
                    info: QueryBuilder.unknownSerializationsInfo,
                    type: Designer.ElementViewModel,
                    nonToolboxItem: true,
                    surfaceType: Designer.SurfaceElementBase,
                    isDeleteDeny: true
                });
                QueryBuilder.controlsFactory.registerControl("Relation", {
                    info: QueryBuilder.relationSerializationInfo,
                    defaultVal: {},
                    surfaceType: QueryBuilder.RelationSurface,
                    popularProperties: [],
                    type: QueryBuilder.RelationViewModel,
                    elementActionsTypes: [],
                    nonToolboxItem: true
                });
                QueryBuilder.controlsFactory.registerControl("JoinCondition", {
                    info: QueryBuilder.joinConditionSerializationInfo,
                    defaultVal: {},
                    surfaceType: QueryBuilder.JoinConditionSurface,
                    popularProperties: ["_parentColumnName", "_nestedColumnName", "joinType"],
                    type: QueryBuilder.JoinConditionViewModel,
                    elementActionsTypes: [Designer.ElementActions],
                    nonToolboxItem: true
                });
                QueryBuilder.controlsFactory.registerControl("Table", {
                    info: QueryBuilder.tableSerializationInfo,
                    defaultVal: {
                        "Column": []
                    },
                    surfaceType: QueryBuilder.TableSurface,
                    popularProperties: ["name", "alias", "columns"],
                    type: QueryBuilder.TableViewModel,
                    elementActionsTypes: [Designer.ElementActions],
                    nonToolboxItem: true
                });
                QueryBuilder.controlsFactory.registerControl("Column", {
                    info: QueryBuilder.columnSerializationInfo,
                    defaultVal: {},
                    surfaceType: QueryBuilder.ColumnSurface,
                    popularProperties: ["name", "alias", "selected"],
                    type: QueryBuilder.ColumnViewModel,
                    elementActionsTypes: [Designer.ElementActions],
                    nonToolboxItem: true,
                    isDeleteDeny: true
                });
                QueryBuilder.controlsFactory.registerControl("Query", {
                    info: QueryBuilder.querySerializationsInfo,
                    surfaceType: QueryBuilder.QuerySurface,
                    popularProperties: ["name", "filterString"],
                    type: QueryBuilder.QueryViewModel,
                    elementActionsTypes: [Designer.ElementActions],
                    isContainer: true,
                    nonToolboxItem: true,
                    isDeleteDeny: true
                });
            }
            QueryBuilder.registerControls = registerControls;
            QueryBuilder.HandlerUri = "DXQB.axd";
            function customizeDesignerActions(designerModel, nextCustomizer) {
                var query = designerModel.model;
                return (function (actions) {
                    var del = QueryBuilder.findFirstItemMatchesCondition(actions, function (action) { return action.text === "Delete"; });
                    del.imageClassName = "dx-icon-dxrd-image-recycle-bin";
                    var undo = QueryBuilder.findFirstItemMatchesCondition(actions, function (action) { return action.text === "Undo"; });
                    undo.disabled = ko.pureComputed(function () { return designerModel.isLoading() || !designerModel.undoEngine().undoEnabled(); });
                    var redo = QueryBuilder.findFirstItemMatchesCondition(actions, function (action) { return action.text === "Redo"; });
                    actions.splice(0, actions.length, del, undo, redo);
                    actions.push({
                        text: "Save",
                        imageClassName: "dxqb-image-save",
                        disabled: designerModel.isLoading,
                        visible: true,
                        hotKey: { ctrlKey: true, keyCode: "S".charCodeAt(0) },
                        clickAction: function () {
                            query().save();
                        },
                        hasSeparator: true
                    });
                    actions.push({
                        text: "Preview Results",
                        imageClassName: "dxrd-image-data-preview",
                        disabled: designerModel.isLoading,
                        visible: true,
                        hotKey: { ctrlKey: true, keyCode: "P".charCodeAt(0) },
                        clickAction: function () {
                            designerModel.showPreview();
                        },
                        hasSeparator: true
                    });
                    nextCustomizer && nextCustomizer(actions);
                });
            }
            QueryBuilder.previewDataCallback = function (sqlDataSource, queryJSON) {
                return $.post(Designer.QueryBuilder.HandlerUri, {
                    actionKey: 'getDataPreview',
                    arg: encodeURIComponent(JSON.stringify({
                        connectionString: sqlDataSource.connection.name(),
                        dataSourceBase64: sqlDataSource.base64(),
                        sqlQueryJSON: queryJSON
                    }))
                }).fail(function (data) {
                });
            };
            function ajax(action, arg) {
                var deferred = $.Deferred();
                $.post(QueryBuilder.HandlerUri, {
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
            function updateQueryBuilderSurfaceContentSize(surfaceSize) {
                return function () {
                    var rightAreaWidth = $(".dxqb-designer .dxrd-right-panel").outerWidth();
                    $(".dxqb-designer .dxrd-surface-wrapper").css("right", rightAreaWidth);
                    var surfaceWidth = $(".dxqb-designer").width() - (rightAreaWidth + 5);
                    $(".dxqb-designer .dxrd-surface-wrapper").css("width", surfaceWidth);
                    surfaceSize(surfaceWidth);
                };
            }
            QueryBuilder.updateQueryBuilderSurfaceContentSize = updateQueryBuilderSurfaceContentSize;
            function createIsLoadingFlag(model, dbSchemaProvider) {
                var isDbSchemaLoaded = ko.observable(false);
                dbSchemaProvider.subscribe(function () {
                    isDbSchemaLoaded(false);
                });
                return ko.pureComputed(function () {
                    dbSchemaProvider.peek().getDbSchema().done(function () {
                        isDbSchemaLoaded(true);
                    });
                    if (isDbSchemaLoaded()) {
                        return model().tables.peek().some(function (table) {
                            return !table.isInitialized();
                        });
                    }
                    else {
                        return true;
                    }
                });
            }
            QueryBuilder.createIsLoadingFlag = createIsLoadingFlag;
            function updateLocalization(object) {
                var messages = {};
                $.each(object, function (name, value) {
                    messages[Designer.localization_values[name] ? Designer.localization_values[name] : name] = value;
                });
                Globalize.addCultureInfo("default", {
                    messages: messages
                });
            }
            QueryBuilder.updateLocalization = updateLocalization;
            ;
            QueryBuilder.isJoinsResolvingDisabled = false;
            function createQueryBuilder(element, data, callbacks, localization) {
                if (localization) {
                    Globalize.addCultureInfo("default", {
                        messages: localization
                    });
                }
                registerControls();
                var query = ko.observable(), surface = ko.observable(), treeListOptions = ko.observable();
                query.subscribe(function (newValue) {
                    surface(new QueryBuilder.QuerySurface(newValue));
                });
                var init = function (querySource) {
                    var _query = new QueryBuilder.QueryViewModel(querySource, data.dbSchemaProvider());
                    _query.filterString.helper.canChoiceParameters = !!data.parametersEditingEnabled;
                    query(_query);
                    treeListOptions({
                        itemsProvider: data.dbSchemaProvider(),
                        selectedPath: ko.observable()
                    });
                };
                data.querySource.subscribe(function (newValue) {
                    init(newValue);
                });
                init(data.querySource());
                var designerModel = Designer.createDesigner(query, surface, QueryBuilder.controlsFactory);
                designerModel.rootStyle = "dxqb-designer";
                designerModel.dataPreview = {
                    isLoading: ko.observable(false),
                    isVisible: ko.observable(false),
                    data: ko.observable(),
                    okButtonHandler: function (e) {
                        e.model.isVisible(false);
                    }
                };
                designerModel.parts = [
                    { templateName: "dxqb-toolbar", model: designerModel },
                    { templateName: "dxrd-surface-template-base", model: designerModel },
                    { templateName: "dx-right-panel-lightweight", model: designerModel },
                    { templateName: "dxqb-data-preview", model: designerModel.dataPreview }
                ];
                designerModel.columnDragHandler = new QueryBuilder.ColumnDragHandler(surface, designerModel.selection, designerModel.undoEngine, designerModel.snapHelper, designerModel.dragHelperContent);
                designerModel.resizeHandler["handles"] = "e, w";
                var tablesTop = ko.observable(0);
                designerModel.tabPanel.tabs.length = 0;
                designerModel.tabPanel.tabs.push(new Designer.TabInfo("Properties", "dxqb-properties-wrapper", {
                    editableObject: designerModel.editableObject,
                    properties: new DevExpress.JS.Widgets.ObjectProperties(ko.pureComputed(function () {
                        return designerModel.selection.focused() && designerModel.selection.focused().getControlModel();
                    }), null, 1),
                    fieldListModel: { treeListOptions: treeListOptions },
                    tablesTop: tablesTop
                }));
                designerModel.fieldDragHandler = new QueryBuilder.DbObjectDragDropHandler(surface, designerModel.selection, designerModel.undoEngine, designerModel.snapHelper, designerModel.dragHelperContent);
                designerModel.dataBindingsProvider = designerModel.fieldListProvider = new QueryBuilder.QBTablesProvider(query);
                designerModel.isLoading = createIsLoadingFlag(designerModel.model, data.dbSchemaProvider);
                designerModel.actionLists = new Designer.ActionLists(surface, designerModel.selection, designerModel.undoEngine, customizeDesignerActions(designerModel, callbacks && callbacks.customizeActions));
                designerModel.selection.focused(surface());
                surface.subscribe(function (newValue) {
                    designerModel.selection.focused(newValue);
                });
                if (!designerModel.isLoading()) {
                    designerModel.undoEngine && designerModel.undoEngine().clearHistory();
                }
                designerModel.isLoading.subscribe(function (value) {
                    designerModel.undoEngine && designerModel.undoEngine().clearHistory();
                });
                designerModel.selection.focused.subscribe(function (newValue) {
                    tablesTop(null);
                    tablesTop.notifySubscribers();
                });
                if (element) {
                    $(element).children().remove();
                    ko.cleanNode(element);
                    ko.applyBindings(designerModel, element);
                }
                var updateSurfaceContentSize_ = updateQueryBuilderSurfaceContentSize(designerModel.surfaceSize);
                $(window).bind("resize", function () {
                    updateSurfaceContentSize_();
                });
                designerModel.tabPanel.width.subscribe(function () {
                    updateSurfaceContentSize_();
                });
                designerModel.updateSurfaceSize = function () {
                    updateSurfaceContentSize_();
                };
                designerModel.updateSurface = function () {
                    updateSurfaceContentSize_();
                    tablesTop(167);
                };
                designerModel.updateSurface();
                designerModel.showPreview = function () {
                    designerModel.dataPreview.isLoading(true);
                    designerModel.dataPreview.isVisible(true);
                    QueryBuilder.previewDataCallback(data.dbSchemaProvider().dataSource(), JSON.stringify(query().serialize(true))).done(function (data) {
                        designerModel.dataPreview.data(JSON.parse(data.result));
                        designerModel.dataPreview.isLoading(false);
                    }).fail(function (data) {
                        designerModel.dataPreview.isVisible(false);
                        Designer.ShowMessage(Designer.getErrorMessage(data));
                        Designer.NotifyAboutWarning(Designer.getErrorMessage(data));
                    });
                };
                designerModel.updateLocalization = updateLocalization;
                return designerModel;
            }
            QueryBuilder.createQueryBuilder = createQueryBuilder;
            ko.bindingHandlers['dxQueryBuilder'] = {
                init: function (element, valueAccessor) {
                    var templateHtml = $('#dxrd-querybuilder').text(), $element = $(element).append(templateHtml);
                    ko.applyBindings(ko.unwrap(valueAccessor()), $element.children()[0]);
                    return { controlsDescendantBindings: true };
                }
            };
            ko.bindingHandlers['dxdTableView'] = {
                init: function (element, valueAccessor) {
                    var templateHtml = $('#dxd-tableview').text(), $element = $(element).append(templateHtml);
                    var value = ko.unwrap(valueAccessor()), appendFakeRow = ko.observable(false);
                    ko.applyBindings({ data: value, appendFakeRow: appendFakeRow }, $element.children()[0]);
                    var $titles = $element.find(".dxd-tableview-titles");
                    var $content = $element.find(".dxd-tableview-data table");
                    appendFakeRow($element.height() > $titles.height() + $content.height());
                    $element.find(".dxd-tableview-titles .dxd-tableview-resizable").each(function (index, resizable) {
                        var $title = $(resizable).find(".dxd-tableview-cell-text");
                        var $column = $element.find(".dxd-tableview-data .dxd-tableview-resizable" + index);
                        if (index < value.schema.length - 1) {
                            $(resizable).resizable({
                                handles: "e",
                                alsoResize: $column.parent(),
                                resize: function (e, ui) {
                                    $title.outerWidth(ui.size.width);
                                    $column.outerWidth(ui.size.width);
                                }
                            });
                        }
                        var maxWidth = Math.max($title.width(), $column.width());
                        $title.width(maxWidth);
                        $column.width(maxWidth);
                    });
                    $(".dxd-tableview-data").dxScrollView({
                        onScroll: function (e) {
                            if (e.scrollOffset.left >= 0) {
                                $titles.offset({ left: $content.offset().left, top: $titles.offset().top });
                            }
                        }
                    });
                    return { controlsDescendantBindings: true };
                }
            };
        })(QueryBuilder = Designer.QueryBuilder || (Designer.QueryBuilder = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Data;
    (function (Data) {
        Data.dsParameterNameValidationRules = [{
            type: "custom",
            validationCallback: function (options) {
                return DataSourceParameter.validateName(options.value);
            },
            message: DevExpress.Designer.getLocalization('Name is required and should be a valid identifier.')
        }];
        Data.parameterValueSerializationsInfo = { propertyName: "value", displayName: "Value", editor: DevExpress.JS.Widgets.editorTemplates.text };
        Data.dsParameterSerializationInfo = [
            { propertyName: "_name", modelName: "@Name" },
            { propertyName: "name", displayName: "Name", validationRules: Data.dsParameterNameValidationRules, editor: DevExpress.JS.Widgets.editorTemplates.text },
            {
                propertyName: "type",
                displayName: "Type",
                modelName: "@Type",
                editor: DevExpress.JS.Widgets.editorTemplates.combobox,
                values: {
                    "System.String": "String",
                    "System.DateTime": "Date",
                    "System.Int16": "Number (16 bit integer)",
                    "System.Int32": "Number (32 bit integer)",
                    "System.Int64": "Number (64 bit integer)",
                    "System.Single": "Number (floating-point)",
                    "System.Double": "Number (double-precision floating-point)",
                    "System.Decimal": "Number (decimal)",
                    "System.Boolean": "Boolean",
                    "System.Guid": "Guid",
                    "DevExpress.DataAccess.Expression": "Expression"
                }
            },
            { propertyName: "_value", modelName: "Value" },
            Data.parameterValueSerializationsInfo
        ];
        var DataSourceParameter = (function (_super) {
            __extends(DataSourceParameter, _super);
            function DataSourceParameter(model, serializer) {
                var _this = this;
                _super.call(this);
                this._valueInfo = ko.observable(Data.parameterValueSerializationsInfo);
                serializer = serializer || new DevExpress.JS.Utils.ModelSerializer();
                serializer.deserialize(this, model);
                this.name = ko.pureComputed({
                    read: function () {
                        return _this._name();
                    },
                    write: function (value) {
                        if (DataSourceParameter.validateName(value))
                            _this._name(value);
                    }
                });
                this._expressionValue = ko.observable({
                    value: this._value
                });
                this._disposables.push(this.type.subscribe(function (val) {
                    _this._updateValueInfo(val);
                }));
                this.value = ko.pureComputed({
                    read: function () {
                        return _this.type() === "DevExpress.DataAccess.Expression" ? _this._expressionValue() : _this._value();
                    },
                    write: function (val) {
                        _this._value(val);
                    }
                });
                this._updateValueInfo(this.type.peek());
            }
            DataSourceParameter.prototype._getTypeValue = function (typeName) {
                var result = DataSourceParameter.typeValues.filter(function (type) {
                    return type.name === typeName;
                });
                if (result.length > 0)
                    return result[0];
                return { name: typeName, defaultValue: null, specifics: "String", disableEditor: true };
            };
            DataSourceParameter.prototype._tryConvertValue = function (value, typeValue) {
                if (!DataSourceParameter._isValueValid(value))
                    return typeValue.defaultValue;
                var converter = typeValue.valueConverter || (function (val) {
                    return val;
                }), newValue = converter(value);
                return DataSourceParameter._isValueValid(value) ? newValue : typeValue.defaultValue;
            };
            DataSourceParameter._isValueValid = function (value) {
                return value !== void 0 && value !== null && !isNaN(typeof value === "string" ? "" : value);
            };
            DataSourceParameter.prototype._updateValueInfo = function (newType) {
                var typeValue = this._getTypeValue(newType);
                this.value(this._tryConvertValue(this._value(), typeValue));
                this._valueInfo($.extend({}, Data.parameterValueSerializationsInfo, { editor: DevExpress.Designer.getEditorType(typeValue.name), disabled: typeValue.disableEditor === true }));
            };
            Object.defineProperty(DataSourceParameter.prototype, "specifics", {
                get: function () {
                    var _this = this;
                    var result = DataSourceParameter.typeValues.filter(function (type) {
                        return type.name === _this.type();
                    });
                    if (result.length > 0)
                        return result[0].specifics;
                    return "string";
                },
                enumerable: true,
                configurable: true
            });
            DataSourceParameter.validateName = function (nameCandidate) {
                return nameCandidate && !nameCandidate.match(/[~`!"№;%\^:\?*\(\)&\-\+={}\[\]\|\\\/,\.<>'\s]/);
            };
            DataSourceParameter.prototype.getInfo = function () {
                if (this.type) {
                    var info = $.extend(true, [], Data.dsParameterSerializationInfo);
                    info.splice(info.indexOf(info.filter(function (prop) {
                        return prop.propertyName === "value";
                    })[0]), 1, this._valueInfo());
                    return info;
                }
                return Data.dsParameterSerializationInfo;
            };
            DataSourceParameter.typeValues = [
                { name: "System.DateTime", defaultValue: new Date(new Date().setHours(0, 0, 0, 0)), specifics: "Date", valueConverter: function (val) {
                    return Globalize.parseDate(val, ["yyyy-MM-dd", "MM/dd/yyyy HH:mm:ss"]);
                } },
                { name: "System.String", defaultValue: "", specifics: "String" },
                { name: "System.SByte", defaultValue: 0, specifics: "Integer", valueConverter: function (val) {
                    return parseInt(val);
                } },
                { name: "System.Int16", defaultValue: 0, specifics: "Integer", valueConverter: function (val) {
                    return parseInt(val);
                } },
                { name: "System.Int32", defaultValue: 0, specifics: "Integer", valueConverter: function (val) {
                    return parseInt(val);
                } },
                { name: "System.Int64", defaultValue: "0", specifics: "String" },
                { name: "System.Byte", defaultValue: 0, specifics: "Integer", valueConverter: function (val) {
                    return parseInt(val);
                } },
                { name: "System.UInt16", defaultValue: 0, specifics: "Integer", valueConverter: function (val) {
                    return parseInt(val);
                } },
                { name: "System.UInt32", defaultValue: 0, specifics: "Integer", valueConverter: function (val) {
                    return parseInt(val);
                } },
                { name: "System.UInt64", defaultValue: "0", specifics: "String" },
                { name: "System.Decimal", defaultValue: "0", specifics: "String" },
                { name: "System.Double", defaultValue: 0, specifics: "Float", valueConverter: function (val) {
                    return parseFloat(val);
                } },
                { name: "System.Single", defaultValue: 0, specifics: "Float", valueConverter: function (val) {
                    return parseFloat(val);
                } },
                { name: "System.Boolean", defaultValue: false, specifics: "Bool", valueConverter: function (val) {
                    return val !== void 0 ? String(val).toLowerCase() === "true" : val;
                } },
                { name: "System.Guid", defaultValue: "00000000-0000-0000-0000-000000000000", specifics: "String" },
                { name: "DevExpress.DataAccess.Expression", defaultValue: "", specifics: "String" },
                { name: "System.Char", defaultValue: "", specifics: "String" }
            ];
            return DataSourceParameter;
        })(DevExpress.Designer.Disposable);
        Data.DataSourceParameter = DataSourceParameter;
    })(Data = DevExpress.Data || (DevExpress.Data = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Data;
    (function (Data) {
        var MasterDetailEditorPopupManager = (function () {
            function MasterDetailEditorPopupManager(target, popupService, action, popupItems) {
                var _this = this;
                this.showPopup = function (args) {
                    _this._popupService.title("");
                    _this._updateActions(_this.target);
                    _this._popupService.target(args.element);
                    _this._popupService.visible(true);
                };
                this.target = target;
                this._action = action;
                this._popupService = popupService;
                this._popupItems = popupItems;
            }
            MasterDetailEditorPopupManager.prototype._updateActions = function (viewModel) {
                var _this = this;
                this._popupService.data({
                    data: this._popupItems,
                    template: "dx-filtereditor-popup-common",
                    click: function (data) {
                        viewModel[_this._action](data);
                        _this._popupService.visible(false);
                    }
                });
            };
            return MasterDetailEditorPopupManager;
        })();
        Data.MasterDetailEditorPopupManager = MasterDetailEditorPopupManager;
        var MasterDetailEditor = (function () {
            function MasterDetailEditor(dataSource, saveCallBack) {
                var _this = this;
                this.isValid = ko.observable(true);
                this.popupVisible = ko.observable(false);
                this.loadPanelVisible = ko.observable(false);
                this.buttonItems = [];
                this.masterQueries = ko.observableArray();
                this.popupService = new DevExpress.Designer.PopupService();
                this.save = function () {
                    var emptyFieldsExist = dataSource.relations().some(function (relation) {
                        return !relation.detailQuery() || !relation.masterQuery() || relation.keyColumns().some(function (column) { return (!column.detailColumn() || !column.masterColumn()); });
                    });
                    if (emptyFieldsExist) {
                        DevExpress.Designer.ShowMessage(DevExpress.Designer.getLocalization("Some fields are empty. Please fill all empty fields or remove the corresponding conditions to proceed."));
                    }
                    else {
                        saveCallBack().done(function () {
                            _this.popupVisible(false);
                        });
                    }
                };
                this.createRelation = function (target) {
                    var popupItems = dataSource.resultSet.tables().filter(function (table) { return table.tableName() !== target.queryName; }).map(function (table) {
                        return { name: table.tableName() };
                    });
                    return {
                        data: new MasterDetailEditorPopupManager(target, _this.popupService, "create", popupItems),
                        templateName: "dx-filtereditor-create"
                    };
                };
                this.setColumn = function (target) {
                    var table = DevExpress.Designer.getFirstItemByPropertyValue(dataSource.resultSet.tables(), "tableName", target.queryName);
                    return {
                        data: new MasterDetailEditorPopupManager(target, _this.popupService, "setColumn", table ? table.columns() : []),
                        templateName: "dx-masterdetail-editor-setColumn"
                    };
                };
                this._createMainPopupButtons();
                var masterQueries = {};
                dataSource.resultSet.tables().forEach(function (table) {
                    masterQueries[table.tableName()] = new MasterQuerySurface(table.tableName(), dataSource.relations);
                });
                dataSource.relations().forEach(function (relation) {
                    masterQueries[relation.masterQuery()] = masterQueries[relation.masterQuery()] || new MasterQuerySurface(relation.masterQuery(), dataSource.relations);
                    masterQueries[relation.masterQuery()].add(relation);
                });
                this.masterQueries($.map(masterQueries, function (value) { return value; }));
            }
            MasterDetailEditor.prototype._createMainPopupButtons = function () {
                var self = this;
                this.buttonItems = [
                    { toolbar: 'bottom', location: 'after', widget: 'button', options: { text: 'Save', onClick: function () {
                        self.save();
                    } } },
                    { toolbar: 'bottom', location: 'after', widget: 'button', options: { text: 'Cancel', onClick: function () {
                        self.popupVisible(false);
                    } } }
                ];
            };
            return MasterDetailEditor;
        })();
        Data.MasterDetailEditor = MasterDetailEditor;
        var MasterQuerySurface = (function () {
            function MasterQuerySurface(masterQueryName, relations) {
                var _this = this;
                this.relations = ko.observableArray();
                this.queryName = masterQueryName;
                this.add = function (relation) {
                    _this.relations.push(new MasterDetailRelationSurface(relation, _this));
                };
                this.create = function (detailQueryItem) {
                    var newRelation = new Data.MasterDetailRelation({ "@Master": _this.queryName, "@Detail": detailQueryItem.name });
                    if (DevExpress.Designer.getFirstItemByPropertyValue(_this.relations(), "relationName", newRelation.name())) {
                        newRelation.name(DevExpress.Designer.getUniqueName(_this.relations().map(function (item) { return item.relationName(); }), newRelation.name() + '_'));
                    }
                    newRelation.keyColumns.push({
                        masterColumn: ko.observable(),
                        detailColumn: ko.observable()
                    });
                    _this.add(newRelation);
                    relations.push(newRelation);
                };
                this.remove = function (relationSurface) {
                    _this.relations.remove(relationSurface);
                    relations.remove(function (item) { return item.name === relationSurface.relationName; });
                };
            }
            return MasterQuerySurface;
        })();
        Data.MasterQuerySurface = MasterQuerySurface;
        var MasterDetailRelationSurface = (function () {
            function MasterDetailRelationSurface(relation, parent) {
                var _this = this;
                this.isEditable = ko.observable(false);
                this.relationName = relation.name;
                this.keyColumns = ko.pureComputed(function () {
                    return relation.keyColumns().map(function (item) {
                        return {
                            master: new KeyColumnSurface(item.masterColumn, relation.masterQuery()),
                            detail: new KeyColumnSurface(item.detailColumn, relation.detailQuery())
                        };
                    });
                });
                this.create = function () {
                    relation.keyColumns.push({
                        masterColumn: ko.observable(),
                        detailColumn: ko.observable()
                    });
                };
                this.remove = function (data) {
                    relation.keyColumns.remove(function (item) { return item.masterColumn === data.master.column && item.detailColumn === data.detail.column; });
                    if (relation.keyColumns().length === 0)
                        parent.remove(_this);
                };
            }
            return MasterDetailRelationSurface;
        })();
        Data.MasterDetailRelationSurface = MasterDetailRelationSurface;
        var KeyColumnSurface = (function () {
            function KeyColumnSurface(column, queryName) {
                var _this = this;
                this.column = column;
                this.queryName = queryName;
                this.setColumn = function (resultColumn) {
                    _this.column(resultColumn.name());
                };
            }
            return KeyColumnSurface;
        })();
        Data.KeyColumnSurface = KeyColumnSurface;
    })(Data = DevExpress.Data || (DevExpress.Data = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Data;
    (function (Data) {
        var resultSetSerializationInfo = [
            { propertyName: "name", modelName: "@Name" },
            { propertyName: "tables", modelName: "View", array: true }
        ];
        var ResultSet = (function () {
            function ResultSet(model, serializer) {
                serializer = serializer || new DevExpress.JS.Utils.ModelSerializer();
                serializer.deserialize(this, model);
                this.tables = DevExpress.JS.Utils.deserializeArray(model["View"], function (item) {
                    return new ResultTable(item, serializer);
                });
            }
            ResultSet.prototype.getInfo = function () {
                return resultSetSerializationInfo;
            };
            return ResultSet;
        })();
        Data.ResultSet = ResultSet;
        var resultTableSerializationInfo = [
            { propertyName: "tableName", modelName: "@Name" },
            {
                propertyName: "columns",
                modelName: "Field",
                array: true,
                info: [
                    { propertyName: "name", modelName: "@Name" },
                    { propertyName: "propertyType", modelName: "@Type" }
                ]
            }
        ];
        var ResultTable = (function () {
            function ResultTable(model, serializer) {
                serializer = serializer || new DevExpress.JS.Utils.ModelSerializer();
                serializer.deserialize(this, model);
            }
            ResultTable.prototype.getInfo = function () {
                return resultTableSerializationInfo;
            };
            return ResultTable;
        })();
        Data.ResultTable = ResultTable;
    })(Data = DevExpress.Data || (DevExpress.Data = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Data;
    (function (Data) {
        Data.SqlQueryType = {
            customSqlQuery: "CustomSqlQuery",
            tableQuery: "TableQuery",
            storedProcQuery: "StoredProcQuery"
        };
        var SqlDataSource = (function (_super) {
            __extends(SqlDataSource, _super);
            function SqlDataSource(model, base64, serializer) {
                var _this = this;
                _super.call(this);
                serializer = serializer || new DevExpress.JS.Utils.ModelSerializer();
                serializer.deserialize(this, model);
                this.base64 = function () {
                    return base64;
                };
                this.queries = DevExpress.JS.Utils.deserializeArray(model["Query"], function (item) {
                    if (item["@Type"] === Data.SqlQueryType.customSqlQuery) {
                        return new CustomSqlQuery(item, _this, serializer);
                    }
                    else if (item["@Type"] === Data.SqlQueryType.tableQuery) {
                        return new TableQuery(item, _this, serializer);
                    }
                    else if (item["@Type"] === Data.SqlQueryType.storedProcQuery) {
                        return new StoredProcQuery(item, _this, serializer);
                    }
                    else {
                        throw new Error("Unknown sql query type.");
                    }
                });
                this.relations = DevExpress.JS.Utils.deserializeArray(model["Relation"], function (item) {
                    return new MasterDetailRelation(item, serializer);
                });
                this.dbSchemaProvider = new Data.DBSchemaProvider(this);
                this._disposables.push(this.connection.name.subscribe(function () {
                    _this.queries([]);
                    _this.relations([]);
                    _this.resultSet = null;
                }));
            }
            SqlDataSource.prototype.getInfo = function () {
                return sqlDataSourceSerializationInfo;
            };
            return SqlDataSource;
        })(DevExpress.Designer.Disposable);
        Data.SqlDataSource = SqlDataSource;
        var SqlDataSourceConnection = (function () {
            function SqlDataSourceConnection(model, serializer) {
                this.name = ko.observable();
                serializer = serializer || new DevExpress.JS.Utils.ModelSerializer();
                serializer.deserialize(this, model);
            }
            SqlDataSourceConnection.from = function (model, serializer) {
                return new SqlDataSourceConnection(model, serializer);
            };
            SqlDataSourceConnection.prototype.getInfo = function () {
                return sqlDataSourceConnectionSerializationInfo;
            };
            return SqlDataSourceConnection;
        })();
        Data.SqlDataSourceConnection = SqlDataSourceConnection;
        var sqlDataSourceConnectionSerializationInfo = [
            { propertyName: "name", modelName: "@Name" },
            { propertyName: "fromAppConfig", modelName: "@FromAppConfig", defaultVal: false, from: DevExpress.Designer.parseBool }
        ];
        Data.masterDetailRelationSerializationsInfo = [
            { propertyName: "masterQuery", modelName: "@Master" },
            { propertyName: "detailQuery", modelName: "@Detail" },
            { propertyName: "_customName", modelName: "@Name" },
            {
                propertyName: "keyColumns",
                modelName: "KeyColumn",
                array: true,
                info: [
                    { propertyName: "masterColumn", modelName: "@Master" },
                    { propertyName: "detailColumn", modelName: "@Detail" }
                ]
            }
        ];
        var sqlDataSourceSerializationInfo = [
            { propertyName: "name", modelName: "Name" },
            { propertyName: "connection", modelName: "Connection", from: SqlDataSourceConnection.from },
            { propertyName: "queries", modelName: "Query", array: true },
            { propertyName: "relations", modelName: "Relation", array: true },
            {
                propertyName: "resultSet",
                modelName: "ResultSchema",
                from: function (val, serializer) {
                    return !val ? null : new Data.ResultSet(val["DataSet"], serializer);
                }
            }
        ];
        Data.customQuerySerializationsInfo = [
            { propertyName: "type", modelName: "@Type" },
            { propertyName: "name", modelName: "@Name" },
            { propertyName: "sqlString", modelName: "Sql", defaultVal: "" },
            { propertyName: "parameters", modelName: "Parameter", array: true }
        ];
        var CustomSqlQuery = (function () {
            function CustomSqlQuery(model, parent, serializer) {
                this.parent = parent;
                (serializer || new DevExpress.JS.Utils.ModelSerializer()).deserialize(this, model);
                this.name(this.name.peek() || "Query");
                this.type = ko.pureComputed(function () {
                    return Data.SqlQueryType.customSqlQuery;
                });
                this.parameters = DevExpress.JS.Utils.deserializeArray(model["Parameter"], function (item) {
                    return new Data.DataSourceParameter(item, serializer);
                });
            }
            CustomSqlQuery.prototype.getInfo = function () {
                return Data.customQuerySerializationsInfo;
            };
            return CustomSqlQuery;
        })();
        Data.CustomSqlQuery = CustomSqlQuery;
        Data.tableQuerySerializationsInfo = [
            { propertyName: "type", modelName: "@Type" },
            { propertyName: "name", modelName: "@Name", defaultVal: "TableQuery" },
            { propertyName: "parameters", modelName: "Parameter", array: true },
            { propertyName: "filterString", modelName: "Filter", defaultVal: "" }
        ];
        var TableQuery = (function () {
            function TableQuery(model, parent, serializer) {
                this.parent = parent;
                (serializer || new DevExpress.JS.Utils.ModelSerializer()).deserialize(this, model);
                this.type = ko.pureComputed(function () {
                    return Data.SqlQueryType.tableQuery;
                });
                this.parameters = DevExpress.JS.Utils.deserializeArray(model["Parameter"], function (item) {
                    return new Data.DataSourceParameter(item, serializer);
                });
            }
            TableQuery.prototype.getInfo = function () {
                return Data.tableQuerySerializationsInfo;
            };
            return TableQuery;
        })();
        Data.TableQuery = TableQuery;
        Data.storedProcQuerySerializationsInfo = [
            { propertyName: "type", modelName: "@Type" },
            { propertyName: "name", modelName: "@Name" },
            { propertyName: "procName", modelName: "ProcName" },
            { propertyName: "parameters", modelName: "Parameter", array: true }
        ];
        var StoredProcQuery = (function () {
            function StoredProcQuery(model, parent, serializer) {
                this.parent = parent;
                (serializer || new DevExpress.JS.Utils.ModelSerializer()).deserialize(this, model);
                this.type = ko.pureComputed(function () {
                    return Data.SqlQueryType.storedProcQuery;
                });
                this.parameters = DevExpress.JS.Utils.deserializeArray(model["Parameter"], function (item) {
                    return new Data.DataSourceParameter(item, serializer);
                });
            }
            StoredProcQuery.prototype.getInfo = function () {
                return Data.storedProcQuerySerializationsInfo;
            };
            return StoredProcQuery;
        })();
        Data.StoredProcQuery = StoredProcQuery;
        var MasterDetailRelation = (function () {
            function MasterDetailRelation(model, serializer) {
                var _this = this;
                this.name = ko.pureComputed({
                    read: function () {
                        return _this._customName() || _this.masterQuery() + _this.detailQuery();
                    },
                    write: function (value) {
                        _this._customName(value);
                    },
                    deferEvaluation: true
                });
                (serializer || new DevExpress.JS.Utils.ModelSerializer()).deserialize(this, model);
            }
            MasterDetailRelation.prototype.getInfo = function () {
                return Data.masterDetailRelationSerializationsInfo;
            };
            return MasterDetailRelation;
        })();
        Data.MasterDetailRelation = MasterDetailRelation;
    })(Data = DevExpress.Data || (DevExpress.Data = {}));
})(DevExpress || (DevExpress = {}));
var DevExpress;
(function (DevExpress) {
    var Designer;
    (function (Designer) {
        var QueryBuilder;
        (function (QueryBuilder) {
            QueryBuilder.AggregationType = {
                None: "None",
                Count: "Count",
                Max: "Max",
                Min: "Min",
                Avg: "Avg",
                Sum: "Sum"
            };
            QueryBuilder.columnSerializationInfo = [
                QueryBuilder.name,
                { propertyName: "displayType", displayName: "Type", disabled: true, editor: QueryBuilder.editorTemplates.text },
                QueryBuilder.alias,
                QueryBuilder.selected,
                {
                    propertyName: "sortingType",
                    displayName: "Sort Type",
                    editor: QueryBuilder.editorTemplates.combobox,
                    defaultVal: "Unsorted",
                    values: {
                        "Unsorted": "Unsorted",
                        "Ascending": "Ascending",
                        "Descending": "Descending"
                    }
                },
                { propertyName: "sortOrder", displayName: "Sort Order", editor: QueryBuilder.editorTemplates.numeric },
                { propertyName: "groupBy", displayName: "Group By", editor: QueryBuilder.editorTemplates.bool, defaultVal: false },
                { propertyName: "aggregate", modelName: "@Aggregate", displayName: "Aggregate", editor: QueryBuilder.editorTemplates.combobox, values: QueryBuilder.AggregationType, defaultVal: QueryBuilder.AggregationType.None }
            ];
            var ColumnViewModel = (function (_super) {
                __extends(ColumnViewModel, _super);
                function ColumnViewModel(model, dbColumn, parent, serializer) {
                    var _this = this;
                    _super.call(this, $.extend({ "@ControlType": "Column" }, model), parent, serializer);
                    this.displayType = ko.pureComputed(function () {
                        return dbColumn.type + (dbColumn.size ? '(' + dbColumn.size + ')' : "");
                    });
                    this.dataType = ko.pureComputed(function () {
                        return DevExpress.Data.DBColumn.GetType(dbColumn.type);
                    });
                    this.actualName = ko.pureComputed(function () {
                        return _this.alias() || _this.name();
                    });
                    var points = parent.getColumnConnectionPoints(this);
                    this.rightConnectionPoint = {
                        side: ko.observable(0 /* East */),
                        location: points.right
                    };
                    this.leftConnectionPoint = {
                        side: ko.observable(3 /* West */),
                        location: points.left
                    };
                    var orderByCollection = parent.parentModel().orderBy;
                    var orderByItem = ko.computed(function () {
                        return QueryBuilder.findFirstItemMatchesCondition(orderByCollection(), function (item) { return item.table() === parent.name() && item.column() === _this.name(); });
                    });
                    this.sortingType = ko.computed({
                        read: function () {
                            if (!orderByItem())
                                return "Unsorted";
                            return orderByItem().descending() ? "Descending" : "Ascending";
                        },
                        write: function (newValue) {
                            if (newValue !== "Unsorted") {
                                if (orderByItem()) {
                                    orderByItem().descending(newValue === "Descending");
                                }
                                else {
                                    orderByCollection.push(new QueryBuilder.OrderByItem({ "@Table": parent.name(), "@Column": _this.name(), "@Descending": newValue === "Descending" }));
                                }
                            }
                            else if (orderByItem()) {
                                orderByCollection.remove(orderByItem());
                            }
                        }
                    });
                    this.sortOrder = ko.computed({
                        read: function () {
                            var index = orderByCollection().indexOf(orderByItem());
                            return index < 0 ? undefined : index + 1;
                        },
                        write: function (newValue) {
                            if (!orderByItem())
                                return;
                            newValue = Math.min(newValue, orderByCollection().length);
                            newValue = Math.max(newValue, 1);
                            var oldValue = orderByCollection().indexOf(orderByItem());
                            var item = orderByCollection.splice(oldValue, 1);
                            orderByCollection.splice(newValue - 1, 0, item[0]);
                        }
                    });
                    var groupByCollection = parent.parentModel().groupBy;
                    var groupByItem = ko.computed(function () {
                        return QueryBuilder.findFirstItemMatchesCondition(groupByCollection(), function (item) { return item.table() === parent.name() && item.column() === _this.name(); });
                    });
                    this.aggregate.subscribe(function (value) {
                        if (value !== QueryBuilder.AggregationType.None) {
                            _this.groupBy(false);
                            if (!_this.alias() || _this._isAliasAutoGenerated()) {
                                _this.alias(_this.name() + '_' + value);
                            }
                        }
                        else if (_this._isAliasAutoGenerated()) {
                            _this.alias(null);
                        }
                    });
                    this.groupBy = ko.computed({
                        read: function () {
                            return !!groupByItem();
                        },
                        write: function (value) {
                            if (value) {
                                groupByCollection.push({
                                    table: ko.observable(parent.name()),
                                    column: ko.observable(_this.name())
                                });
                                _this.aggregate(QueryBuilder.AggregationType.None);
                            }
                            else {
                                groupByCollection.remove(groupByItem());
                            }
                        }
                    });
                    var isSelected = ko.pureComputed(function () { return parent.selectedColumns.indexOf(_this) >= 0; });
                    this.selected = ko.pureComputed({
                        read: function () { return isSelected(); },
                        write: function (value) {
                            if (isSelected() === value)
                                return;
                            if (value) {
                                parent.selectedColumns.push(_this);
                            }
                            else {
                                parent.selectedColumns.remove(_this);
                            }
                        }
                    });
                }
                ColumnViewModel.prototype._isAliasAutoGenerated = function () {
                    if (!this.alias() || this.alias().indexOf(this.name() + '_') !== 0)
                        return false;
                    var funcName = this.alias().substring(this.name().length + 1);
                    return Object.keys(QueryBuilder.AggregationType).indexOf(funcName) > 0;
                };
                ColumnViewModel.prototype.getInfo = function () {
                    return QueryBuilder.columnSerializationInfo;
                };
                ColumnViewModel.prototype.isPropertyDisabled = function (name) {
                    if (name === "sortOrder") {
                        return this.sortingType() === "Unsorted";
                    }
                    else if (name === "aggregate") {
                        return !this.selected();
                    }
                };
                return ColumnViewModel;
            })(QueryBuilder.QueryElementBaseViewModel);
            QueryBuilder.ColumnViewModel = ColumnViewModel;
            var ColumnSurface = (function (_super) {
                __extends(ColumnSurface, _super);
                function ColumnSurface(control, context) {
                    var _this = this;
                    _super.call(this, control, context, null);
                    this.template = "dxqb-table-field";
                    this.toggleSelected = function () {
                        _this.getControlModel().selected(!_this.getControlModel().selected());
                    };
                    this.selectedWrapper = ko.pureComputed(function () {
                        return _this.getControlModel().selected();
                    });
                    this.isAggregate = ko.pureComputed(function () {
                        return _this.getControlModel().aggregate() !== QueryBuilder.AggregationType.None;
                    });
                    this.isAscending = ko.pureComputed(function () {
                        return _this.getControlModel().sortingType() === "Ascending";
                    });
                    this.isDescending = ko.pureComputed(function () {
                        return _this.getControlModel().sortingType() === "Descending";
                    });
                }
                return ColumnSurface;
            })(Designer.SurfaceElementBase);
            QueryBuilder.ColumnSurface = ColumnSurface;
        })(QueryBuilder = Designer.QueryBuilder || (Designer.QueryBuilder = {}));
    })(Designer = DevExpress.Designer || (DevExpress.Designer = {}));
})(DevExpress || (DevExpress = {}));
//# sourceMappingURL=query-builder.js.map