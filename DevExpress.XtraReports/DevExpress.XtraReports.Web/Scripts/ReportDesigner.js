/// <reference path="report-designer.js"/>
/// <reference path="WebClientUIControl.js"/>

(function(window) {
    var ASPxClientReportDesigner = ASPx.CreateClass(ASPxClientControl, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.internalSettings = {};
            this.SaveCommandExecute = new ASPxClientEvent();
            this.CustomizeMenuActions = new ASPxClientEvent();
            this.CustomizeParameterEditors = new ASPxClientEvent();
        },
        Initialize: function() {
            // fix for ASPx.RunStartupScripts with ko usage
            // do not touch the 'this.isInitialized' field
            if(this.selfInitialized) {
                return;
            }
            this.selfInitialized = true;

            ASPxClientControl.prototype.Initialize.call(this);
            var holder = ASPx.GetNodeByClassName(this.GetMainElement(), 'dx-designer');
            var platformErrorElement = ASPx.GetNodeByClassName(holder, 'dxwcuic-platformError');
            if(!ASPx.WebClientUIControl.validateSupportedBrowser(platformErrorElement, 'dxwcuic-hidden')) {
                return;
            }
            var self = this;
            function onSave() {
                var arg = new ASPxClientReportDesignerSaveCommandExecuteEventArgs();
                self.SaveCommandExecute.FireEvent(self, arg);
                if(self.canSaveExecuteCore(arg)) {
                    self.PerformCallback();
                }
            }
            function getRootKeyName(obj) {
                for(var prop in obj) {
                    if(obj.hasOwnProperty(prop)) {
                        return prop;
                    }
                }
                return null;
            }
            function customizeActions(actions) {
                for(var i = 0; i < self.menuItems.length; i++) {
                    var menuItem = self.menuItems[i];
                    actions.push(menuItem);
                }
                var arg = new ASPxClientReportDesignerCustomizeMenuActionsEventArgs(actions);
                self.CustomizeMenuActions.FireEvent(self, arg);
            }
            function customizeParameterEditors(parameter, info) {
                var arg = new ASPxClientCustomizeParameterEditorsEventArgs(parameter, info);
                self.CustomizeParameterEditors.FireEvent(self, arg);
            }

            ASPx.WebClientUIControl.initDebugMode(this);
            ASPx.WebClientUIControl.initViewerHandlerUri(this.viewerHandlerUri);
            ASPx.WebClientUIControl.initDesignerHandlerUri(this.handlerUri);
            ASPxClientReportDesigner.initAce(this.internalSettings);
            if(this.menuItems && this.menuItemActions) {
                ASPx.WebClientUIControl.initMenuItems(this.menuItems, this.menuItemActions);
            }
            if(this.dataSources && this.dataSourcesData) {
                ASPxClientReportDesigner.initDataSources(this.dataSources, this.dataSourcesData);
            }
            if(this.reportModel) {
                this.reportModelRootName = getRootKeyName(this.reportModel);
                var data = {
                    report: ko.observable(this.generateDataReportBase()),
                    reportUrl: ko.observable(this.reportUrl),
                    availableDataSources: this.dataSources,
                    dataSourceRefs: this.dataSourceRefs,
                    subreports: this.subreports,
                    infoDefaults: this.infoDefaults,
                    state: {
                        reportExtensions: this.reportExtensions
                    },
                    isReportServer: this.internalSettings.isReportServer,
                    connectionStrings: this.wizardConnections,
                    disableCustomSql: this.disableCustomSql
                };
                var callbacks = {
                    customizeActions: customizeActions,
                    customizeParameterEditors: customizeParameterEditors
                };
                this.designerModel = DevExpress.Designer.Report.createReportDesigner(holder, data, callbacks, this.localization, this.knownEnums);
                data.report(this.reportModel);
                this.designerModel.isLoading(false);
                ko.computed(function() {
                    self.designerModel.model().onSave = onSave;
                });
            }
        },
        PerformCallback: function(args) {
            if(!ASPx.IsExists(args)) {
                args = '';
            } else if(typeof args === 'object') {
                args = JSON.stringify(arg);
            }
            this.performCallbackCore(args);
        },
        UpdateLocalization: function(localization) {
            this.designerModel.updateLocalization(localization);
        },
        GetDesignerModel: function() {
            return this.designerModel;
        },
        GetJsonReportModel: function() {
            var reportModel = this.designerModel.model().serialize();
            var result = null;
            if(this.reportModelRootName) {
                result = {};
                result[this.reportModelRootName] = reportModel;
            } else {
                result = reportModel;
            }
            return result;
        },
        GetPropertyInfo: function(controlType, propertyDisplayName) {
            return DevExpress.Designer.getPropertyInfo(controlType, propertyDisplayName);
        },
        IsModified: function() {
            return this.designerModel && this.designerModel.isDirty();
        },
        ResetIsModified: function() {
            if(this.designerModel) {
                this.designerModel.isDirty(false);
            }
        },
        performCallbackCore: function(arg) {
            var request = {
                reportLayout: this.GetJsonReportModel(),
                arg: arg
            };
            var requestString = ASPxClientReportDesigner.convertToCallbackString(request);
            this.CreateCallback(requestString, 'save');
        },
        canSaveExecuteCore: function(arg) {
            return !arg.handled;
        },
        generateDataReportBase: function() {
            var dataReportBase = {};
            dataReportBase[this.reportModelRootName] = {
                "@ControlType": this.reportModel[this.reportModelRootName]["@ControlType"]
            };
            return dataReportBase;
        },
        OnBrowserWindowResize: function(evt) {
            this.AdjustControl();
        },
        AdjustControlCore: function() {
            this.designerModel && this.designerModel.updateSurfaceSize();
        }
    });
    ASPxClientReportDesigner.Cast = ASPxClientControl.Cast;

    ASPxClientReportDesigner.initDataSources = function(dataSources, dataSourcesData) {
        for(var i = 0; i < dataSources.length; i++) {
            var dataSource = dataSources[i];
            dataSource.data = dataSourcesData[i];
        }
    };

    ASPxClientReportDesigner.convertToCallbackString = function(obj) {
        var result = '';
        var isFirstIteration = true;
        for(var i in obj) {
            var value = obj[i];
            if(!value) {
                continue;
            }
            if(!isFirstIteration) {
                result += '&';
            }
            var jsonValue = typeof value === 'string' ? value : JSON.stringify(value);
            var encodedValue = encodeURIComponent(jsonValue);
            result += i + '=' + encodedValue;
            isFirstIteration = false;
        }
        return result;
    };

    ASPxClientReportDesigner.initAce = function(settings) {
        var ignoreAceLibsRedirectFromDX = settings && settings.ignoreAceLibsRedirectFromDX;
        var ace = window.ace;
        if(!ignoreAceLibsRedirectFromDX && ace) {
            var config = ace.config;
            config.setModuleUrl('ace/theme/dreamweaver', '<%=WebResource("DevExpress.XtraReports.Web.Scripts.Frameworks.theme-dreamweaver.js")%>');
            config.setModuleUrl('ace/theme/ambiance', '<%=WebResource("DevExpress.XtraReports.Web.Scripts.Frameworks.theme-ambiance.js")%>');
            config.setModuleUrl('ace/mode/csharp', '<%=WebResource("DevExpress.XtraReports.Web.Scripts.Frameworks.mode-csharp.js")%>');
            config.setModuleUrl('ace/mode/vbscript', '<%=WebResource("DevExpress.XtraReports.Web.Scripts.Frameworks.mode-vbscript.js")%>');
            config.setModuleUrl('ace/snippets/text', '<%=WebResource("DevExpress.XtraReports.Web.Scripts.Frameworks.snippets.text.js")%>');
            config.setModuleUrl('ace/snippets/csharp', '<%=WebResource("DevExpress.XtraReports.Web.Scripts.Frameworks.snippets.csharp.js")%>');
            config.setModuleUrl('ace/snippets/vbscript', '<%=WebResource("DevExpress.XtraReports.Web.Scripts.Frameworks.snippets.vbscript.js")%>');
        }
    };
    var ASPxClientReportDesignerSaveCommandExecuteEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function() {
            this.constructor.prototype.constructor.call(this);
            this.handled = false;
        }
    });
    //{
    //}
    var ASPxClientReportDesignerCustomizeMenuActionsEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(actions) {
            this.constructor.prototype.constructor.call(this);
            this.Actions = actions;
        }
    });

    window.ASPxClientReportDesigner = ASPxClientReportDesigner;
    window.ASPxClientReportDesignerSaveCommandExecuteEventArgs = ASPxClientReportDesignerSaveCommandExecuteEventArgs;
    window.ASPxClientReportDesignerCustomizeMenuActionsEventArgs = ASPxClientReportDesignerCustomizeMenuActionsEventArgs;
})(window);