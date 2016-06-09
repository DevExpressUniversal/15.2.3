/// <reference path="web-document-viewer.js"/>
/// <reference path="WebClientUIControl.js"/>

(function(window) {
    var ASPxClientWebDocumentViewer = ASPx.CreateClass(ASPxClientControl, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.CustomizeMenuActions = new ASPxClientEvent();
            this.CustomizeParameterEditors = new ASPxClientEvent();
        },
        Initialize: function() {
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
            ASPx.WebClientUIControl.initDebugMode(this);
            ASPx.WebClientUIControl.initViewerHandlerUri(this.viewerHandlerUri);

            if(this.menuItems && this.menuItemActions) {
                ASPx.WebClientUIControl.initMenuItems(this.menuItems, this.menuItemActions);
            }

            var self = this;
            function customizeParameterEditors(parameter, info) {
                var arg = new ASPxClientCustomizeParameterEditorsEventArgs(parameter, info);
                self.CustomizeParameterEditors.FireEvent(self, arg);
            }

            function customizeActions(actions) {
                if(self.menuItems) {
                    for(var i = 0; i < self.menuItems.length; i++) {
                        var menuItem = self.menuItems[i];
                        actions.push(menuItem);
                    }
                }
                var arg = new ASPxClientWebDocumentViewerCustomizeMenuActionsEventArgs(actions);
                self.CustomizeMenuActions.FireEvent(self, arg);
            }

            var callbacks = {
                customizeParameterEditors: customizeParameterEditors,
                customizeActions: customizeActions
            };
            this.previewModel = DevExpress.Report.Preview.createPreview(holder, callbacks, this.localization, this.parametersInfo, this.handlerUri);
            if(this.reportId) {
                var deferred = $.Deferred();
                var previewInitializeInfo = { result: { reportId: this.reportId, exportOptions: this.exportOptions, parametersInfo: this.parametersInfo, pageHeight: this.pageHeight, pageWidth: this.pageWidth } };
                this.previewModel.reportPreview.initialize(deferred.resolve(previewInitializeInfo));
            }
        },
        GetPreviewModel: function() {
            return this.previewModel;
        },
        OpenReport: function(reportName) {
            return this.previewModel && this.previewModel.reportPreview && this.previewModel.reportPreview.openReport(reportName);
        },
        Print: function(pageIndex) {
            return this.previewModel && this.previewModel.reportPreview && this.previewModel.reportPreview.printDocument(pageIndex);
        }
    });
    ASPxClientWebDocumentViewer.Cast = ASPxClientControl.Cast;
    //{
    //}
    var ASPxClientWebDocumentViewerCustomizeMenuActionsEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(actions) {
            this.constructor.prototype.constructor.call(this);
            this.Actions = actions;
        }
    });

    window.ASPxClientWebDocumentViewer = ASPxClientWebDocumentViewer;
    window.ASPxClientWebDocumentViewerCustomizeMenuActionsEventArgs = ASPxClientWebDocumentViewerCustomizeMenuActionsEventArgs;
})(window);