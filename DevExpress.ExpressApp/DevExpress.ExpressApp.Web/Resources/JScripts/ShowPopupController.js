function OnIFrameLoaded(iframe, popupControl) {
    popupControl.ShowPopupController.CustomSetupPopup(popupControl, popupControl.GetWindowElement(-1));
    if (ASPx.Browser.WebKitTouchUI && popupControl.isMobile && popupControl.NewStyle && !popupControl.showInFindPopup) {
        xaf.PopupControllersManager.CreatePopupScrollController(popupControl);
    }
}
function ShowNewPopup(popupControl, isMobile, showInFindPopup) {
    popupControl.NewStyle = true;
    popupControl.isMobile = isMobile; //for Window tablet, because the ASPx.Browser.WebKitTouchUI sometimes not work for Win tablets
    popupControl.showInFindPopup = showInFindPopup;
    popupControl.SetWindowPos = CustomPopupSetWindowPos;

    var showPopupController = xaf.ControllersManager.CreateController(ShowPopupController);
    popupControl.ShowPopupController = showPopupController;
    ShowNewPopupCore(popupControl, showPopupController);
}
function ShowNewPopupCore(popupControl, showPopupController) {
    showPopupController.ShowPopup(popupControl);
}
//Override popup control function
function CustomPopupSetWindowPos(index, element, x, y) {
    this.ShowPopupController.CustomSetupPopup(this, element);
}
function CustomPopupControlClose(popupControl) {
    if (!popupControl.showInFindPopup) {
        window.top.menuState.Locked -= 1;
        xaf.PopupControllersManager.RemoveLastPopupScrollController();

        xaf.PopupControllersManager.GetLastFrameController().ShowParent();
        if (window.top.menuState.Locked == 0) {
            popupControl.ShowPopupController.RestoreHidenMenu();
        }
    } else {
        xaf.PopupControllersManager.ActivateLastPopupScrollController();
    }
    xaf.PopupControllersManager.RemoveFrameController();
    window.lockUpdate = false;
}

function ShowPopupController() {

}
ShowPopupController.prototype.ShowPopup = function (popupControl) {
    var modalGrayWindow = popupControl.GetWindowModalElement(-1);
    modalGrayWindow.style.opacity = 0;

    xaf.PopupControllersManager.CreatePopupFrameController(popupControl);

    if (popupControl.showInFindPopup) {
        this.ShowFindPopup(popupControl);
    } else {
        this.ShowDialogPopup(popupControl, modalGrayWindow);
    }
}

ShowPopupController.prototype.ShowFindPopup = function (popupControl) {
    xaf.PopupControllersManager.DeactivateLastPopupScrollController();
    var element = popupControl.GetWindowElement(-1);
    var popupPosition = this.CalcFindPopupPosition(popupControl);
    this.FindPopupSetPosition(popupControl, element, popupPosition);
    this.CustomSetupPopup(popupControl, element);
    HeaderAndFooterHideBodyScroll();
}
ShowPopupController.prototype.FindPopupSetPosition = function (popupControl, element, popupPosition) {
    //don't use animate
    popupControl.left = popupPosition.left;
    element.style.left = popupPosition.left + 'px';
    //popupControl.top = popupPosition.top;
    //element.style.top = popupPosition.top + 'px';
}
ShowPopupController.prototype.CalcFindPopupPosition = function (popupControl) {
    var leftPos = 0;
    var popupMinWidth = 670;
    var findPopupHeight = 497;
    var popupWidth = popupMinWidth;
    var popupMaxPading = popupControl.isMobile ? 20 : 50;
    var clientHeight = GetClientHeight();

    var result = new Object();
    result.width = 0;


    result.height = findPopupHeight;// popupControl.isMobile ? clientHeight / 100 * 80 : clientHeight / 100 * 40;


    result.top = (clientHeight - findPopupHeight) / 2;
    result.top += GetScrollPosition();

    var parentPopupFrame;
    if (!popupControl.isMobile) {
        parentPopupFrame = xaf.PopupControllersManager.GetParentDialogPopupFrame(xaf.PopupControllersManager.GetLastFrameController());
    }
    var contentLeftPos;
    var contentWidth;
    if (parentPopupFrame) {
        contentLeftPos = $(parentPopupFrame).offset().left;
        contentWidth = GetWindowWidth(parentPopupFrame.contentWindow);
    } else {
        contentLeftPos = $(document.getElementById("content")).offset().left;
        contentWidth = $(document.getElementById("content")).width();
    }

    result.left = contentLeftPos;
    popupWidth = contentWidth;
    if (contentWidth > popupMinWidth) {
        var padding = ((contentWidth - popupMinWidth) / 2);
        var resultpadding = padding > popupMaxPading ? popupMaxPading : padding;
        result.left += resultpadding;
        popupWidth -= (resultpadding * 2);
    }
    result.width = popupWidth;
    return result;
}

ShowPopupController.prototype.ShowDialogPopup = function (popupControl, modalGrayWindow) {
    window.lockUpdate = true;
    window.top.animateComplete = false;
    popupControl.isAShown = false;

    var element = popupControl.GetWindowElement(-1);
    element.style.display = 'none';
    var hideLeftPos = GetClientWidth();
    DevExpress.fx.animate(element, {
        duration: 0,
        type: 'slide',
        to: { left: hideLeftPos }
    });
    this.CustomSetupPopup(popupControl, element);

    window.top.menuState.Locked += 1;

    if (window.menuState.moved) {
        if (window.menuState.Locked == 1) {
            var hideMenuAreaDivTopPosition = $("#menuAreaDiv").height() * -1;
            var hideHeaderDivWithShadowTopPosition = $("#headerDivWithShadow").height() * -1;
            DevExpress.fx.animate($("#menuAreaDiv"), {
                type: 'slide',
                to: { top: hideMenuAreaDivTopPosition }
            });
            var self = this;
            DevExpress.fx.animate($("#headerDivWithShadow"), {
                type: 'slide',
                to: { top: hideHeaderDivWithShadowTopPosition },
                complete: function () {
                    self.AnimateShowDialogPopup(popupControl, element, modalGrayWindow);
                }
            });
        } else {
            this.AnimateShowDialogPopup(popupControl, element, modalGrayWindow);
        }
    } else {
        this.AnimateShowDialogPopup(popupControl, element, modalGrayWindow);
    }
}

ShowPopupController.prototype.CustomSetupPopup = function (popupControl, element) {
    if (popupControl.NewStyle) {
        if (popupControl.showInFindPopup) {
            this.CustomSetupFindPopup(popupControl, element);
        } else {
            this.CustomSetupDialogPopup(popupControl, element);
        }
    }
}
ShowPopupController.prototype.CustomSetupFindPopup = function (popupControl, element) {
    var popupPosition = this.CalcFindPopupPosition(popupControl);
    var contentWidth = popupPosition.width;
    var contentHeight = popupPosition.height;

    element.style.width = contentWidth + 'px';
    element.style.height = contentHeight + 'px';
    element.style.top = this.GetTopPosition(popupControl) + 'px';

    var contentIFrame = popupControl.GetContentIFrame();
    if (contentIFrame) {
        contentIFrame.style.height = contentHeight + 'px';
        var contentWrapper = popupControl.GetWindowContentWrapperElement(-1);
        var contentElement = popupControl.GetWindowContentElement(-1);
        contentWrapper.style.height = contentHeight + 'px';
        contentElement.style.height = contentHeight + 'px';
    }
    if (!popupControl.isMobile) { //for resize in screen application
        this.FindPopupSetPosition(popupControl, element, popupPosition);
    }
}
ShowPopupController.prototype.CustomSetupDialogPopup = function (popupControl, element) {
    var contentIFrame = popupControl.GetContentIFrame();
    var contentHeight = GetClientHeight();
    var contentWidth;
    var clientWidth = GetClientWidth() - 2;
    if (popupControl.isMobile) {
        if (ASPx.Browser.WebKitTouchUI) {
            if (contentIFrame) {
                var contentWrapper = popupControl.GetWindowContentWrapperElement(-1);
                var contentElement = popupControl.GetWindowContentElement(-1);
                contentWrapper.style.width = clientWidth + 'px';
                contentElement.style.width = clientWidth + 'px';
                var documentHeight = $(popupControl.GetContentIFrame().contentWindow.document).height();
                if (documentHeight && documentHeight > contentHeight) {
                    contentHeight = documentHeight;
                }
                contentWrapper.style.height = contentHeight + 'px';
                contentElement.style.height = contentHeight + 'px';
            }
        } else { // for mobile IE devises
            contentWidth = clientWidth;
            contentHeight = contentHeight;// + GetScrollPosition();
        }
    } else {
        contentWidth = clientWidth > 768 ? clientWidth / 100 * 70 : clientWidth;
        contentHeight = contentHeight;// + GetScrollPosition();
    }
    element.style.width = contentWidth + 'px';
    element.style.height = contentHeight + 'px';
    element.style.top = this.GetTopPosition(popupControl) + 'px';
    if (contentIFrame) {
        contentIFrame.style.height = contentHeight + 'px';
    }
    if (!popupControl.isMobile && popupControl.isAShown) { //for resize in screen application
        clientWidth = GetClientWidth();
        var leftPosition = clientWidth > 768 ? clientWidth / 100 * 30 : 0;
        DevExpress.fx.animate(element, {
            duration: 0,
            type: 'slide',
            to: { left: leftPosition }
        });
    }
}
ShowPopupController.prototype.GetTopPosition = function (popupControl) {
    var top = 0;
    if (!popupControl.showInFindPopup) {
        if (!popupControl.isMobile) {
            if (window.top.popupControlState && window.top.popupControlState.opened) {
                top = window.top.popupControlState.top;
            }
            if (top <= 0) {
                top = GetScrollPosition();
            }
        }
    } else {
        var clientHeight = GetClientHeight();
        var findPopupHeight = 497;

        top = (clientHeight - findPopupHeight) / 2;

        var scrollPosition = GetScrollPosition();
        if (scrollPosition > 0) {
            top += scrollPosition;
        }
    }
    return top;
}
ShowPopupController.prototype.AnimateShowDialogPopup = function (popupControl, element, modalGrayWindow) {
    DevExpress.fx.animate(modalGrayWindow, {
        type: "fade", from: 0, to: 0.7
    });
    window.setTimeout(function () {
        var popupWindow = popupControl.GetContentIFrame().contentWindow;
        xaf.PopupControllersManager.GetLastFrameController().HideParent();

        var leftPosition = 0;
        var clientWidth = GetClientWidth();
        if (!popupControl.isMobile) {
            leftPosition = clientWidth > 768 ? clientWidth / 100 * 30 : 0;
        }
        element.style.display = 'table';
        DevExpress.fx.animate(element, {
            type: 'slide',
            to: { left: leftPosition },
            complete: function () {
                popupControl.isAShown = true;
                window.top.animateComplete = true;
                popupControl.ShowPopupController.PopupAnimateComplete();
            }
        });
    }, 0);
}
ShowPopupController.prototype.PopupAnimateComplete = function () {
    AnimateComplete();
}
ShowPopupController.prototype.RestoreHidenMenu = function () {
    if (window.menuState.moved) {
        DevExpress.fx.animate($("#menuAreaDiv"), {
            type: 'slide',
            to: { top: 0 }
        })
        DevExpress.fx.animate($("#headerDivWithShadow"), {
            type: 'slide',
            to: { top: 0 },
        })
    }
}