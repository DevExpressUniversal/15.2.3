/// <reference path="ReportViewer.js"/>

(function(window) {
    var ASPxClientReportDocumentMap = ASPx.CreateClass(ASPxClientControl, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.ContentChanged = new ASPxClientEvent();
        },
        InlineInitialize: function() {
            ASPxClientControl.prototype.InlineInitialize.call(this);
            var callbackPanel = ASPxClientReportDocumentMap.findValidControl(this.callbackPanelId);
            if(callbackPanel) {
                this.callbackPanel = callbackPanel;

                var self = this;
                callbackPanel.EndCallback.AddHandler(function() { self.updateContent(); });
                this.updateTreeViewInstance();
            }
        },
        Initialize: function() {
            this.constructor.prototype.Initialize.call(this);
            if(this.reportViewerId) {
                this.reportViewer = ASPx.GetControlCollection().Get(this.reportViewerId);
                if(this.reportViewer) {
                    this.subscribeReportViewerEvents();
                }
            }
        },
        isValid: function() {
            return this.callbackPanel && ASPx.IsExistsElement(this.callbackPanel.GetMainElement());
        },
        hasTreeView: function() {
            return ASPx.IsExists(this.treeView);
        },
        subscribeReportViewerEvents: function() {
            var self = this;
            this.reportViewer.submitParametersComplete.AddHandler(function(s) { self.submitParametersComplete(s); });
            this.reportViewer.refreshRising.AddHandler(function(s) { self.submitParametersComplete(s); });
        },
        updateTreeViewInstance: function() {
            var treeView = ASPxClientReportDocumentMap.findValidControl(this.callbackPanel.name + '_TreeView');
            if(!treeView) {
                return;
            }
            this.treeView = treeView;
            var treeViewInit = function treeVewInit() {
                treeView.Init.RemoveHandler(treeViewInit);
                var rootNode = treeView.GetNode(0);
                if(rootNode) {
                    rootNode.SetExpanded(false); // T129054
                    rootNode.SetExpanded(true);
                }
            };
            treeView.Init.AddHandler(treeViewInit);
        },
        updateContent: function() {
            this.updateTreeViewInstance();
            this.ContentChanged.FireEvent(this, new ASPxClientEventArgs());
        },
        submitParametersComplete: function() {
            this.callbackPanel.PerformCallback();
        }
    });

    ASPxClientReportDocumentMap.findValidControl = function(name) {
        var control = ASPx.GetControlCollection().Get(name);
        return control && ASPx.IsExistsElement(control.GetMainElement())
            ? control
            : null;
    };

    window.ASPxClientReportDocumentMap = ASPxClientReportDocumentMap;
})(window);