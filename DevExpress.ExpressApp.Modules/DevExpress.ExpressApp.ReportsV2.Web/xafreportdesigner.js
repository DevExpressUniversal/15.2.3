// XAF Report Designer
function XafReportDesignerPopupSetup() {
    var parentWnd = window.parent,
        popupControl = GetActivePopupControl(parentWnd),
        parentGlobalCallbackControl = parentWnd.globalCallbackControl;

    if (popupControl !== null) {
        adjustPopupHeight(popupControl);
        setTimeout(function () {
            adjustPopupStyle(popupControl);
            addClosePopupHandler(popupControl, parentGlobalCallbackControl)
            popupControl.SetMaximized(true);
        }, 1);
    }

    function adjustPopupHeight(popupControl) {
        popupControl.Resize.AddHandler(function (s, e) {
            xafReportDesigner.SetHeight(s.GetContentHeight());
        });
    }
    function addClosePopupHandler(popupControl, globalCallbackControl) {
        popupControl.CloseButtonClick.AddHandler(function () {
            if (ASPx.Browser.Firefox) {
                window.setTimeout(function () {
                    RaiseXafCallback(globalCallbackControl, '', 'XafParentWindowRefresh', '', false);
                }, 2);
            }
            else {
                RaiseXafCallback(globalCallbackControl, '', 'XafParentWindowRefresh', '', false);
            }
        });
    }
}

function XafReportViewerPopupSetup() {
    var popupControl = GetActivePopupControl(window.parent);

    if (popupControl !== null) {
        setTimeout(function () {
            adjustPopupSize(popupControl);
            adjustPopupStyle(popupControl);
        }, 100);
    }

    function adjustPopupSize(popupControl) {
        xafReportViewer.SetHeight(popupControl.GetContentHeight());
        popupControl.Resize.AddHandler(function (s, e) {
            xafReportViewer.SetHeight(s.GetContentHeight());
        });
    }
}
function XafReportPopupSetup(control, padding) {
    var heightPadding = padding,
        reportControl = control,
        popupControl = GetActivePopupControl(window.parent);

    if (popupControl !== null) {
        setTimeout(function () {
            reportControl.SetHeight(popupControl.GetContentHeight() - heightPadding);
        }, 100);
    }
}
function adjustPopupStyle(popupControl) {
    var popupDocument = popupControl.GetContentIFrame().contentDocument;
    createCss(popupDocument, '.ContentCell { padding: 0px !important }');
    createCss(popupDocument, '.Layout div.Item { padding: 0px !important }');
    createCss(popupDocument, '.DialogContent.Content { padding-bottom: 0px !important }');

    function createCss(document, styleInfo) {
        var css = document.createElement('style');
        css.type = 'text/css';
        css.innerHTML = styleInfo;
        document.body.appendChild(css);
    }
}
function xafCustomizeMenuActions(s, e) {
    customizeSaveAction(s, e);
    if (s.cpHiddenMenuItems) {
        hideMenuActions(s, e);
    }
    if (s.cpParametersTypes) {
        initParametersTypes(s)
    }

    function hideMenuActions(s, e) {
        for (index = 0; index < s.cpHiddenMenuItems.length; ++index) {
            var actions = e.Actions.filter(function (x) { return x.text === s.cpHiddenMenuItems[index] });
            if (actions.length > 0) {
                actions[0].visible = false;
            }
        }
    }
    function customizeSaveAction(s, e) {
        var actions = e.Actions.filter(function (x) { return x.text === "Save" });
        if (actions.length > 0) {
            var nativeAction = actions[0].clickAction;
            actions[0].clickAction = function () {
                var currentTab = s.designerModel.navigateByReports.currentTab();
                if (!currentTab.url()) {
                    var layout = s.designerModel.model().serialize();
                    DevExpress.Designer.Report.ReportStorageWeb.setNewData(layout, '').done(function (jsonResult) { currentTab.url(jsonResult.result); }).fail(function (error) { });
                }
                else {
                    nativeAction();
                }
            }
        }
    }
    function initParametersTypes(s) {
        for (var type in s.cpParametersTypes) {
            DevExpress.Designer.Report.Parameter.typeValues.push({ value: type, displayValue: s.cpParametersTypes[type], defaultValue: '', typeImage: '' })
        }
    }
}
function xafInitReportDesigner(s, e) {
    if (s.cpWizardReportTitle) {
        var designer = s;
        setTimeout(function () {
            designer.GetDesignerModel().wizard.start();
            designer.GetDesignerModel().wizard._data.reportTitle = designer.cpWizardReportTitle;
        }, 1);
    }
}