(function (window) {
    var MapViewer = ASPx.CreateClass(ASPxClientControl, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);

            this.MarkerClicked = new ASPxClientEvent();
            this.MapClicked = new ASPxClientEvent();
            this.Customize = new ASPxClientEvent();
        },
        InlineInitialize: function () {
            ASPxClientControl.prototype.InlineInitialize.call(this);

            var self = this;

            this.mapOptions.onClick = function (info) {
                self.MapClicked.FireEvent(this, info.location);
            };

            if (this.mapOptions.width == 0) {
                this.mapOptions.width = 'auto';
            }

            if (this.apiKeys) {
                this.mapOptions.key = apiKeys;
            }

            self.map = $('#' + self.name).dxMap(this.mapOptions).dxMap('instance');
            self.map.parent = self;

            for (var currentMarkerIndex = 0; currentMarkerIndex < this.markers.length; currentMarkerIndex++) {
                var data = this.markers[currentMarkerIndex];
                var latlng = [data.latitude, data.longitude];

                var marker = {
                    location: latlng,
                    tooltip: data.title
                };

                if (self.showDetails) {
                    marker.tooltip += self.GenerateMarkerShowDetailsTooltip(data);
                }

                if (!self.enableMarkersTooltips) {
                    delete marker.tooltip;
                    marker.onClick = function () {
                        self.MarkerClicked.FireEvent(this, data.id);
                    }
                }

                self.map.addMarker(marker);
            }

            if (self.route)
                self.map.addRoute(self.route);

            // for EasyTest
            $[self.name] = self.map;
        },
        AfterInitialize: function () { 
            this.Customize.FireEvent(this, this.map);
        },
        GenerateMarkerShowDetailsTooltip : function (markerData) {
            return '<br><br><a href="javascript:$.' +
                    this.name +
                    '.parent.MarkerClicked.FireEvent($.' +
                    this.name +
                    ', \'' +
                    markerData.id +
                    '\');">' +
                    this.localizedStrings["showDetails"] +
                    '</a>';
        }
    });

    window.MapViewer = MapViewer;
})(window);