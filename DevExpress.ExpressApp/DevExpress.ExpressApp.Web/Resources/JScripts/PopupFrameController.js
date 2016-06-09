function PopupFrameController(popupControl, index) {
    this.control = popupControl;
    this.mainWindow = window.top;
    this.contentHiden = false;
    this.scrollPosition = 0;
    this.parentDispalyState = '';
    this.footerDisplayState;
    this.isNestedController;
    this.controllerIndex = index;
    this.isActive = true;

    if (popupControl) {
        this.isActive = popupControl.isMobile && ASPx.Browser.WebKitTouchUI ? true : false;
        this.isNestedController = true;
    } else {
        this.isNestedController = false;
    }
}
PopupFrameController.prototype.Hide = function () {
    if (!this.isActive) {
        return;
    }
    this.HideCore();
}
PopupFrameController.prototype.HideParent = function () {
    if (!this.isActive) {
        return;
    }
    this.HideParentCore();
}
//TODO Crimp
PopupFrameController.prototype.HideCore = function () {
    if (!this.contentHiden) {
        this.scrollPosition = $(this.mainWindow).scrollTop();
        if (this.isNestedController) {
            this.HidePopupContent();
        } else {
            this.HideMainContentCore();
        }
        this.contentHiden = true;
    }
}
PopupFrameController.prototype.HideParentCore = function () {
    this.scrollPosition = $(this.mainWindow).scrollTop();
    if (this.isNestedController) {
        this.HideParentPopupContent();
    } else {
        this.Hide();
    }
    $(this.mainWindow).scrollTop(0);
}
PopupFrameController.prototype.ShowParent = function () {
    if (!this.isActive) {
        return;
    }
    if (this.isNestedController) {
        this.ShowParentPopup();
    } else {
        this.Show();
    }
}
PopupFrameController.prototype.Show = function () {
    if (!this.isActive) {
        return;
    }
    if (this.contentHiden) {
        if (this.isNestedController) {
            this.ShowPopupContent();
        } else {
            this.ShowMainContentCore();
        }
        this.contentHiden = false;
        this.RestoreScrollPosition();
    }
}
PopupFrameController.prototype.RestoreScrollPosition = function () {
    if (!this.isActive) {
        return;
    }
    $(this.mainWindow).scrollTop(this.scrollPosition);
}
PopupFrameController.prototype.HideMainContent = function (mainDiv, footerDiv, headerTableDiv, headerDivWithShadow, menuAreaDiv) {
    footerDiv.style.display = 'none';
    headerTableDiv.style.display = 'none';
    headerDivWithShadow.style.display = 'none';
    menuAreaDiv.style.display = 'none';

    this.parentDispalyState = mainDiv.style.display;
    mainDiv.style.display = 'none';
}
PopupFrameController.prototype.HideMainContentCore = function () {
    var mainDiv = this.mainWindow.document.getElementById("mainDiv");
    var footerDiv = this.mainWindow.document.getElementById("footer")
    var headerTableDiv = this.mainWindow.document.getElementById("headerTableDiv");
    var headerDivWithShadow = this.mainWindow.document.getElementById("headerDivWithShadow");
    var menuAreaDiv = this.mainWindow.document.getElementById("menuAreaDiv");
    this.HideMainContent(mainDiv, footerDiv, headerTableDiv, headerDivWithShadow, menuAreaDiv);
}
PopupFrameController.prototype.HideParentPopupContent = function () {
    var parentController = xaf.PopupControllersManager.GetParentFrameController(this.controllerIndex - 1);
    if (parentController) {
        parentController.Hide();
    }
}
PopupFrameController.prototype.HidePopupContent = function () {
    this.HidePopupElement();
    if (this.control.showInFindPopup) {
        var parentController = xaf.PopupControllersManager.GetParentFrameController(this.controllerIndex - 1);
        if (parentController) {
            parentController.Hide();
        }
    }
}
PopupFrameController.prototype.HidePopupElement = function () {
    var element = this.control.GetWindowElement(-1); //TODO Crimp Achtung code
    this.parentDispalyState = element.style.display;
    element.style.display = 'none';
}
PopupFrameController.prototype.ShowMainContent = function (mainDiv, footerDiv, headerTableDiv, headerDivWithShadow, menuAreaDiv) {
    menuAreaDiv.style.display = '';
    headerDivWithShadow.style.display = '';
    headerTableDiv.style.display = '';
    footerDiv.style.display = '';
    mainDiv.style.display = this.parentDispalyState;
}
PopupFrameController.prototype.ShowMainContentCore = function () {
    var mainDiv = this.mainWindow.document.getElementById("mainDiv");
    var footerDiv = this.mainWindow.document.getElementById("footer")
    var headerTableDiv = this.mainWindow.document.getElementById("headerTableDiv");
    var headerDivWithShadow = this.mainWindow.document.getElementById("headerDivWithShadow");
    var menuAreaDiv = this.mainWindow.document.getElementById("menuAreaDiv");
    this.ShowMainContent(mainDiv, footerDiv, headerTableDiv, headerDivWithShadow, menuAreaDiv);
}
PopupFrameController.prototype.ShowParentPopup = function () {
    var parentController = xaf.PopupControllersManager.GetParentFrameController(this.controllerIndex - 1);
    if (parentController) {
        parentController.Show();
    }
}
PopupFrameController.prototype.ShowPopupContent = function () {
    this.ShowPopupElement();
    if (this.control.showInFindPopup) {
        var nextController = xaf.PopupControllersManager.GetParentFrameController(this.controllerIndex - 1);
        if (nextController) {
            nextController.Show();
        }
    }
}
PopupFrameController.prototype.ShowPopupElement = function () {
    var element = this.control.GetWindowElement(-1); //TODO Crimp Achtung code
    element.style.display = this.parentDispalyState;
}