/// <reference path="WebClientUIControl.js"/>

(function (window) {
    var ASPxClientChartDesigner = ASPx.CreateClass(ASPxClientControl, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);
            this.SaveCommandExecute = new ASPxClientEvent();
            this.CustomizeMenuActions = new ASPxClientEvent();
        },
        Initialize: function () {
            // fix for ASPx.RunStartupScripts with ko usage
            // do not touch the 'this.isInitialized' field
            if (this.selfInitialized) {
                return;
            }
            this.selfInitialized = true;

            ASPxClientControl.prototype.Initialize.call(this);
            var holder = ASPx.GetNodeByClassName(this.GetMainElement(), 'dx-designer');
            var self = this;
            var onSave = function (data) {
                var arg = new ASPxClientChartDesignerSaveCommandExecuteEventArgs();
                self.SaveCommandExecute.FireEvent(this, arg);
                if (self.canSaveExecuteCore(arg)) {
                    self.PerformCallback();
                }
            };
            function getRootKeyName(obj) {
                for (var prop in obj) {
                    if (obj.hasOwnProperty(prop)) {
                        return prop;
                    }
                }
                return null;
            }
            function customizeActions(actions) {
                for (var i = 0; i < self.menuItems.length; i++) {
                    var menuItem = self.menuItems[i];
                    actions.push(menuItem);
                }
                var arg = new ASPxClientChartDesignerCustomizeMenuActionsEventArgs(actions);
                self.CustomizeMenuActions.FireEvent(self, arg);
            }

            //ASPxClientWebClientUIControl.initViewerHandlerUri(this.viewerHandlerUri);
            //ASPxClientWebClientUIControl.initDesignerHandlerUri(this.handlerUri);
            if (this.menuItems && this.menuItemActions) {
                for (var i = 0; i < this.menuItems.length; i++) {
                    var menuItem = this.menuItems[i];
                    var menuItemAction = this.menuItemActions[i];
                    menuItem.clickAction = menuItemAction || $.noop;
                }
            }
            if (this.chartModel) {
                this.chartModelRootName = getRootKeyName(this.chartModel);
                var data = {
                    chartSource: ko.observable(this.chartModel[this.chartModelRootName]),
                    width: this.width,
                    height: this.height,
                    dataSource: this.dataSource
                }
                var callbacks = {
                    customizeActions: customizeActions
                };
                this.designerModel = DevExpress.Designer.Chart.createChartDesigner(holder, data, callbacks, this.localization);
                this.designerModel.isLoading(false);
                ko.computed(function () {
                    self.designerModel.model().onSave = onSave;
                });
            }
        },
        PerformCallback: function (args) {
            if (!ASPx.IsExists(args)) {
                args = '';
            } else if (typeof args === 'object') {
                args = JSON.stringify(arg);
            }
            this.performCallbackCore(args);
        },
        performCallbackCore: function (arg) {
            var request = {
                chartLayout: this.GetJsonChartModel()
            };
            var requestString = ASPxClientChartDesigner.convertToCallbackString(request);
            this.CreateCallback(requestString, 'save');
        },
        UpdateLocalization: function (localization) {
            this.designerModel.updateLocalization(localization);
        },
        GetDesignerModel: function () {
            return this.designerModel;
        },
        GetJsonChartModel: function () {
            var chartModel = this.designerModel.model().serialize();
            var result = null;
            if (this.chartModelRootName) {
                result = {};
                result[this.chartModelRootName] = chartModel;
            } else {
                result = chartModel;
            }
            return result;
        },
        canSaveExecuteCore: function (arg) {
            return !arg.handled;
        },
        OnBrowserWindowResize: function (evt) {
            this.AdjustControl();
        },
        AdjustControlCore: function () {
            this.designerModel && this.designerModel.updateSurfaceSize();
        }
    });
    ASPxClientChartDesigner.Cast = ASPxClientControl.Cast;


    ASPxClientChartDesigner.convertToCallbackString = function (obj) {
        var result = '';
        var isFirstIteration = true;
        for (var i in obj) {
            var value = obj[i];
            if (!value) {
                continue;
            }
            if (!isFirstIteration) {
                result += '&';
            }
            var jsonValue = typeof value === 'string' ? value : JSON.stringify(value);
            var encodedValue = encodeURIComponent(jsonValue);
            result += i + '=' + encodedValue;
            isFirstIteration = false;
        }
        return result;
    };

    ASPxClientChartDesigner.initHandlerUri = function (uri) {
        DevExpress.Designer.Chart.HandlerUri = uri;
    };
    var ASPxClientChartDesignerSaveCommandExecuteEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function () {
            this.constructor.prototype.constructor.call(this);
            this.handled = false;
        }
    });
    //{
    //}
    var ASPxClientChartDesignerCustomizeMenuActionsEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function (actions) {
            this.constructor.prototype.constructor.call(this);
            this.actions = actions;
        }
    });

    window.ASPxClientChartDesigner = ASPxClientChartDesigner;
    window.ASPxClientChartDesignerSaveCommandExecuteEventArgs = ASPxClientChartDesignerSaveCommandExecuteEventArgs;
    window.ASPxClientChartDesignerCustomizeMenuActionsEventArgs = ASPxClientChartDesignerCustomizeMenuActionsEventArgs;
})(window);