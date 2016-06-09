function PopupScrollController(popupControl) {
    var self = this;
    this.mainWindow = window;
    this.pControl = popupControl;
    this.pWindow;
    this.mainWindowEl;
    this.popupMenu;
    this.documentHeight;
    this.endMoveTimer;
    this.checkMoveTimer;
    this.touchMove = false;
    this.startScrollPosition = 0;
    this.menuHidden = false;
    this.active = false;
    this.startScrollPosition = 0;

    this.WindowScrolled = function () {
        if (!self.touchMove) { //if window scrolled because virtual keyboard was shown
            self.ScrollObserverStart();
        }
    }
    this.CustomTouchStart = function (ev) {
        if (self.active) {
            self.touchMove = false;
            self.startScrollPosition = self.mainWindowEl.scrollTop();
        }
    }
    this.CustomTouchEnd = function () {
        self.touchMove = false;
        self.ScrollObserverStart();
    }
    this.CustomTouchMove = function (ev) {
        self.touchMove = true;
        self.HideMenuIfNeed();
    }
};

PopupScrollController.prototype.SubscribeEvents = function () {
    if (this.active) {
        this.mainWindow.addEventListener('scroll', this.WindowScrolled, false);
        this.pWindow.addEventListener("touchstart", this.CustomTouchStart, false);
        this.pWindow.addEventListener("touchmove", this.CustomTouchMove, false);
        this.pWindow.addEventListener("touchend", this.CustomTouchEnd, false);
    }
}
PopupScrollController.prototype.UnsubscribeEvents = function () {
    this.mainWindow.removeEventListener('scroll', this.WindowScrolled);
    if (this.pWindow) {
        this.pWindow.removeEventListener("touchstart", this.CustomTouchStart);
        this.pWindow.removeEventListener("touchmove", this.CustomTouchMove);
        this.pWindow.removeEventListener("touchend", this.CustomTouchEnd);
    }
}

PopupScrollController.prototype.Activate = function () {
    this.active = this.pControl.isMobile && ASPx.Browser.WebKitTouchUI ? true : false;
    if (this.active) {
        this.pWindow = this.pControl.GetContentIFrame().contentWindow;
        this.mainWindowEl = $(window.top);
        this.popupMenu = this.pWindow.document.getElementById("headerTable");
        this.documentHeight = this.pWindow.document.height;
        this.UnsubscribeEvents();
        this.SubscribeEvents();
    }
}
PopupScrollController.prototype.Deactivate = function () {
    this.active = false;
    this.mainWindowEl = null;
    this.UnsubscribeEvents();
}


PopupScrollController.prototype.HideMenuIfNeed = function () {
    if (!this.menuHidden && this.active) {
        var contentCurrentScroll = this.mainWindowEl.scrollTop();
        if (contentCurrentScroll > 0 && (this.startScrollPosition > contentCurrentScroll)) {
            this.HideMenuCore();
        }
    }
}
PopupScrollController.prototype.ScrollObserverStart = function () {
    clearTimeout(this.endMoveTimer);
    var callBackMoveMenuIfNeed = this.MoveMenuIfNeed;
    var controller = this;
    endMoveTimer = setTimeout(function () {
        callBackMoveMenuIfNeed(controller);
    }, 100);
}
PopupScrollController.prototype.HideMenuCore = function () {
    this.menuHidden = true;
    this.popupMenu.style.display = 'none';
}
PopupScrollController.prototype.ShowMenuCore = function (controller) {
    if (controller.active) {
        controller.menuHidden = false;
        controller.popupMenu.style.display = '';
    }
}
PopupScrollController.prototype.MoveMenu = function (top) {
    var callBackShowMenuCore = this.ShowMenuCore;
    var self = this;
    DevExpress.fx.animate(this.popupMenu, {
        type: 'slide',
        to: { top: top },
        complete: function () {
            callBackShowMenuCore(self);
        }
    });
}
PopupScrollController.prototype.MoveMenuIfNeed = function (controller) {
    if (!controller.touchMove && controller.active) {
        var contentCurrentScroll = controller.mainWindowEl.scrollTop();
        if (contentCurrentScroll <= 0) {
            controller.MoveMenu(0);
        } else {
            clearTimeout(controller.checkMoveTimer);
            var self = controller;
            this.checkMoveTimer = setTimeout(function () {
                if (!self.touchMove && self.active) {
                    var contentScroll = self.mainWindowEl.scrollTop();
                    if (contentCurrentScroll == contentScroll) {
                        self.MoveMenu(contentScroll);
                    } else {
                        self.MoveMenuIfNeed(self);
                    }
                }
            }, 100);
        }
    }
}