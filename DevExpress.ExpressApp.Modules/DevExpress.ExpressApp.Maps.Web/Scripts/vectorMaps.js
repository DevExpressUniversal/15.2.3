(function (window) {
    var VectorMapViewer = ASPx.CreateClass(ASPxClientControl, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);

            this.AreaClicked = new ASPxClientEvent();
            this.MarkerClicked = new ASPxClientEvent();
            this.MapClicked = new ASPxClientEvent();
            this.Customize = new ASPxClientEvent();
        },
        InlineInitialize: function () {
            ASPxClientControl.prototype.InlineInitialize.call(this);

            var self = this;

            var isAreasNotEmpty = Object.keys(self.vectorMapSettings.areasValues).length > 0;
            $.each(self.vectorMapSettings.mapResource, function (name, item) {
                item.attributes.label = item.attributes.name;
                if (isAreasNotEmpty && item.attributes.name in self.vectorMapSettings.areasValues)
                    item.attributes.datavalue = self.vectorMapSettings.areasValues[item.attributes.name];
                else
                    item.attributes.datavalue = self.vectorMapSettings.defaultAreaValue;
            });

            var legendIntervals = new Array;
            for (var o in self.vectorMapSettings.legendItems) {
                legendIntervals.push(self.vectorMapSettings.legendItems[o]);
            }

            var mapObject = {
                mapData: self.vectorMapSettings.mapResource,
                bounds: self.vectorMapSettings.bounds,
                center: self.vectorMapSettings.center,
                zoomFactor: self.vectorMapSettings.zoomFactor,
                controlBar: {
                    visibility: self.vectorMapSettings.controlsEnabled
                },
                background: {
                    color: self.vectorMapSettings.backgroundColor
                },
                markers: self.vectorMapSettings.markers,
                areaSettings: {
                    palette: self.vectorMapSettings.palette,
                    colorGroups: legendIntervals,
                    colorGroupingField: 'datavalue',
                    selectedColor: 'mediumaquamarine',
                    selectedBorderColor: 'darkgreen',
                    selectionMode: 'multiple'
                },
                markerSettings: {
                    // type: 'dot',
                    type: self.vectorMapSettings.markersType,
                    minSize: self.vectorMapSettings.markerMinSize,
                    maxSize: self.vectorMapSettings.markerMaxSize,
                    size: self.vectorMapSettings.pieMarkerSize,
                    sizeGroups: legendIntervals,
                    selectionMode: 'multiple',
                },
                tooltip: {
                    enabled: true,
                    customizeTooltip: function (element) {
                        if (element.type == 'area') {
                            var areaName = element.attribute('name');
                            if (areaName in self.vectorMapSettings.areasTooltips) {
                                return {
                                    text: self.vectorMapSettings.areasTooltips[areaName]
                                }
                            } else {
                                return {
                                    text: areaName
                                };
                            }
                        } if (element.type == 'marker') {
                            return {
                                text: element.attribute("tooltip")
                            };
                        }
                    }
                }
            };

            if (!self.vectorMapSettings.markersTitlesEnabled) {
                mapObject.markerSettings.label = {
                    enabled: false,
                };
            }

            if (self.vectorMapSettings.areasTitlesEnabled) {
                mapObject.areaSettings.label = {
                    enabled: true,
                    dataField: "label"
                };
            }

            if (self.vectorMapSettings.legendEnabled) {
                for (var i = 0; i < self.vectorMapSettings.legendItems.Length; i++) {
                    alert(Object.keys(self.vectorMapSettings.legendItems)[i] + ' ' + self.vectorMapSettings.legendItems[i]);
                }
                mapObject.legends = [{
                    source: self.vectorMapSettings.legendType,
                    customizeText: function (arg) {
                        var offset = self.vectorMapSettings.legendType == "markerSizeGroups" ? 1 : 0;
                        return Object.keys(self.vectorMapSettings.legendItems)[arg.index + offset];
                    }
                }]

                if (self.vectorMapSettings.markersType == "bubble") {
                    mapObject.legends[0].markerType = 'circle';
                }
            }

            mapObject.onClick = function (info) {
                var location = {};
                var coords = info.component.convertCoordinates(info.jQueryEvent.x, info.jQueryEvent.y);
                location.lng = coords[0];
                location.lat = coords[1];
                self.MapClicked.FireEvent(self, location);
            };

            mapObject.onAreaClick = function (info) {
                if (info.jQueryEvent.ctrlKey) {
                    var clickeArea = info.target;
                    clickeArea.selected(!clickeArea.selected());
                } else {
                    var args = {};
                    args.areaName = info.target.attribute('name');
                    args.vectorMapSettings = self.vectorMapSettings;
                    self.AreaClicked.FireEvent(self, args);
                }
            }

            mapObject.onMarkerClick = function (info) {
                if (info.jQueryEvent.ctrlKey) {
                    var clickedMarker = info.target;
                    clickedMarker.selected(!clickedMarker.selected());
                } else {
                    var marker = info.target;
                    var args = {};
                    args.id = marker.attribute("id");
                    args.markerName = marker.text;
                    args.vectorMapSettings = self.vectorMapSettings;
                    self.MarkerClicked.FireEvent(self, args);
                }
            }

            self.map = $('#' + self.name).dxVectorMap(mapObject).dxVectorMap('instance');

            self.map.AreaClickedHandler = mapObject.onAreaClick;
            self.map.MarkerClickedHandler = mapObject.onMarkerClick;
            self.map.VectorMapSettings = self.vectorMapSettings;
        },
        GetAreaId: function (e) {
            var areaName = e.areaName;
            var vectorMapSettings = e.vectorMapSettings;
            if (areaName in vectorMapSettings.areasIds) {
                return vectorMapSettings.areasIds[areaName];
            }
        },
        AfterInitialize: function () {
            this.Customize.FireEvent(this, this.map);
        },
    });

    window.VectorMapViewer = VectorMapViewer;
})(window);