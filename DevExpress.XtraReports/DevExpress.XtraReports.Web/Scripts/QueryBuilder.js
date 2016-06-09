/// <reference path="WebClientUIControl.js"/>

(function (window) {
    var ASPxClientQueryBuilder = ASPx.CreateClass(ASPxClientControl, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);

            this.CustomizeToolbarActions = new ASPxClientEvent();
            this.SaveCommandExecute = new ASPxClientEvent();
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
            var platformErrorElement = ASPx.GetNodeByClassName(holder, 'dxwcuic-platformError')[0];
            if(!ASPx.WebClientUIControl.validateSupportedBrowser(platformErrorElement, 'dxwcuic-hidden')) {
                return;
            }
            var self = this;
            function onSave() {
                var arg = new ASPxClientDesignerSaveCommandExecuteEventArgs();
                self.SaveCommandExecute.FireEvent(self, arg);
                if (self.canSaveExecuteCore(arg)) {
                    self.PerformCallback();
                }
            }
            function getRootKeyName(obj) {
                for (var prop in obj) {
                    if (obj.hasOwnProperty(prop)) {
                        return prop;
                    }
                }
                return null;
            }
            function customizeActions(actions) {
                for (var i = 0; i < (self.menuItems || []).length; i++) {
                    var menuItem = self.menuItems[i];
                    actions.push(menuItem);
                }
                var arg = new ASPxClientQueryBuilderCustomizeToolbarActionsEventArgs(actions);
                self.CustomizeToolbarActions.FireEvent(self, arg);
            }

            ASPx.WebClientUIControl.initDebugMode(this);
            //ASPxClientWebClientUIControl.initViewerHandlerUri(this.viewerHandlerUri);
            //ASPxClientWebClientUIControl.initDesignerHandlerUri(this.handlerUri);
            if (this.menuItems && this.menuItemActions) {
                for (var i = 0; i < this.menuItems.length; i++) {
                    var menuItem = this.menuItems[i];
                    var menuItemAction = this.menuItemActions[i];
                    menuItem.clickAction = menuItemAction || $.noop;
                }
            }
            if (this.dataSourceBase64) {
                var callbacks = {
                    customizeActions: customizeActions
                };

                var dataSource = new DevExpress.Data.SqlDataSource({}, this.dataSourceBase64);
                var dbSchemaProvider = ko.observable(new DevExpress.Data.DBSchemaProvider(dataSource));
                this.designerModel = DevExpress.Designer.QueryBuilder.createQueryBuilder(holder, { querySource: ko.observable(this.queryModel["Query"]), dbSchemaProvider: dbSchemaProvider }, callbacks, this.localization);
                ko.computed(function() {
                    self.designerModel.model().onSave = onSave;
                });
            }
            this.CallbackError.AddHandler(function (s, e) {
                DevExpress.Designer.ShowMessage(e.message);
                e.handled = true;
            });
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
                queryLayout: this.GetJsonQueryModel(),
                dataSourceBase64: this.dataSourceBase64
            };
            var requestString = ASPxClientQueryBuilder.convertToCallbackString(request);
            this.CreateCallback(requestString, 'save');
        },
        UpdateLocalization: function (localization) {
            this.designerModel.updateLocalization(localization);
        },
        GetQueryBuilderModel: function () {
            return this.designerModel;
        },
        GetJsonQueryModel: function () {
            var queryModel = { "Query": this.designerModel.model().serialize() };
            var result = null;
            if (this.queryModelRootName) {
                result = {};
                result[this.queryModelRootName] = queryModel;
            } else {
                result = queryModel;
            }
            return result;
        },
        canSaveExecuteCore: function (arg) {
            return !arg.handled;
        },
        //generateDataReportBase: function () {
        //    var dataReportBase = {};
        //    dataReportBase[this.reportModelRootName] = {
        //        "@ControlType": this.reportModel[this.reportModelRootName]["@ControlType"]
        //    };
        //    return dataReportBase;
        //},
        OnBrowserWindowResize: function (evt) {
            this.AdjustControl();
        },
        AdjustControlCore: function () {
            this.designerModel && this.designerModel.updateSurfaceSize();
        },
        Save: function(){
            this.designerModel && this.designerModel.model().onSave();
        },
        ShowPreview: function(){
            this.designerModel && this.designerModel.showPreview();
        },
        IsQueryValid: function () {
            return this.designerModel && this.designerModel.model().isValid();
        }
    });
    ASPxClientQueryBuilder.Cast = ASPxClientControl.Cast;

    ASPxClientQueryBuilder.convertToCallbackString = function (obj) {
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

    ASPxClientQueryBuilder.initHandlerUri = function (uri) {
        DevExpress.Designer.QueryBuilder.HandlerUri = uri;
    };
    var ASPxClientDesignerSaveCommandExecuteEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function () {
            this.constructor.prototype.constructor.call(this);
            this.handled = false;
        }
    });

    var ASPxClientQueryBuilderCustomizeToolbarActionsEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function (actions) {
            this.constructor.prototype.constructor.call(this);
            this.Actions = actions;
        }
    });

    window.ASPxClientQueryBuilder = ASPxClientQueryBuilder;
    window.ASPxQueryBuilderSaveCommandExecuteEventArgs = ASPxClientDesignerSaveCommandExecuteEventArgs;
    window.ASPxClientQueryBuilderCustomizeToolbarActionsEventArgs = ASPxClientQueryBuilderCustomizeToolbarActionsEventArgs;
})(window);