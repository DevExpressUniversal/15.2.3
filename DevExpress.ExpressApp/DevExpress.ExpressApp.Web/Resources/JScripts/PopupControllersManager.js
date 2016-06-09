/*PopupControllersManager singleton*/
(function () {
    if (typeof xaf.PopupControllersManager == "undefined") {
        if (typeof window.top.xaf.PopupControllersManager == "undefined") {
            xaf.PopupControllersManager = {
                ScrollControllers: [],
                FrameControllers: [],

                CreatePopupFrameController: function (popupControl) {
                    var frameController = xaf.ControllersManager.CreateController(PopupFrameController, [popupControl, this.FrameControllers.length]);
                    this.FrameControllers.push(frameController);
                },
                GetParentDialogPopupFrame: function (popupController) {
                    if (popupController && popupController.controllerIndex > 0) {
                        var parentController = this.GetParentFrameController(popupController.controllerIndex - 1);
                        if (parentController && parentController.isNestedController) {
                            if (!parentController.control.showInFindPopup) {
                                return parentController.control.GetContentIFrame();
                            } else {
                                return this.GetParentDialogPopupFrame(parentController);
                            }
                        }
                    }
                    return null;
                },
                GetLastFrameController: function () {
                    var result = null;
                    var massLenght = this.FrameControllers.length;
                    if (massLenght > 0) {
                        result = this.FrameControllers[massLenght - 1];
                    }
                    return result;
                },
                GetParentFrameController: function (index) {
                    var result;
                    if (index >= 0) {
                        var massLenght = this.FrameControllers.length;
                        if (massLenght > index) {
                            result = this.FrameControllers[index];
                        }
                    }
                    return result;
                },
                RemoveFrameController: function () {
                    this.FrameControllers.pop();
                },



                GetLastPopupScrollController: function () {
                    var lenght = this.ScrollControllers.length;
                    if (lenght > 0) {
                        return this.ScrollControllers[lenght - 1];
                    }
                    return null;
                },
                CreatePopupScrollController: function (popupControl) {
                    this.DeactivateLastPopupScrollController();
                    if (popupControl) {
                        var popupScrollController = xaf.ControllersManager.CreateController(PopupScrollController, [popupControl]);
                        this.ScrollControllers.push(popupScrollController);
                        popupScrollController.Activate();
                    }
                },
                RemoveLastPopupScrollController: function () {
                    if (this.ScrollControllers.length > 0) {
                        var controller = this.ScrollControllers.pop();
                        if (controller) {
                            controller.Deactivate();
                        }
                        this.ActivateLastPopupScrollController();
                    }
                },
                ActivateLastPopupScrollController: function () {
                    var parentController = this.GetLastPopupScrollController();
                    if (parentController) {
                        parentController.Activate();
                    }
                },
                DeactivateLastPopupScrollController: function () {
                    var parentController = this.GetLastPopupScrollController();
                    if (parentController) {
                        parentController.Deactivate();
                    }
                }

            };
        }
        else {
            xaf.PopupControllersManager = window.top.xaf.PopupControllersManager;
        }
    }
})();