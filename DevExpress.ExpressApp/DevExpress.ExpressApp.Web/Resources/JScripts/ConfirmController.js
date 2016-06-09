function ConfirmUnsavedChangedController() {
    var self = this;
    this.mainContainerId = "mainContainer";
    this.confirmActions = [];
    this.Containers = [];
    this.ViewContainersMap = [];
    this.grids = [];
    this.clearLockerTimerDefaultDelay = 15;
    this.clearLockerTimerCustomizeDelay = 1000;
    this.ownerWatcher = null;
    this.isActive = false;
    this.questionMessage = 'Unsaved changes will be lost. Do you want to proceed?';

    this.XafGridViewUpdateWatcherHelper = function (grid, confirmActions, isBatchEdit) {
        return new (ASPx.CreateClass(ASPx.UpdateWatcherHelper, {
            confirmActions: [],
            isBatchEdit: false,
            GridFuncCallbackWaitingTimeout: 15,
            lastArg: null,
            wasConfirmMessage: false,
            constructor: function (grid) {
                this.grid = grid;
                this.constructor.prototype.constructor.call(this, grid);
                this.confirmActions = confirmActions;
                this.isBatchEdit = isBatchEdit;
                if (xaf.ConfirmUnsavedChangedController.ownerWatcher == null) {
                    xaf.ConfirmUnsavedChangedController.clearLockerTimerDefaultDelay = this.ownerWatcher.clearLockerTimerDelay;
                    xaf.ConfirmUnsavedChangedController.ownerWatcher = this.ownerWatcher;
                }
            },
            NeedShowMessageForAction: function (actionID) {
                return this.confirmActions.indexOf(actionID) > -1;
            },
            CanProcessCallbackForGridAction: function (actionID) {
                if (this.NeedShowMessageForAction(actionID)) {
                    return this.ShowMessage(true);
                }
                return true;
            },
            CanProcessCallbackForGridView: function (s, e) {
                this.RemoveConfirmHandlerForAllGrids();
                if (!this.isBatchEdit && s.GetItem(e.visibleIndex)) {
                    var result = false;
                    if (!this.wasConfirmMessage) {
                        result = this.ShowMessage();
                    }
                    this.wasConfirmMessage = false;
                    return result;
                } else {
                    return true;
                }
            },
            AddConfirmHandlerForAllGrids: function () {
                for (var key in xaf.ConfirmUnsavedChangedController.Containers) {
                    var grid = ASPx.GetControlCollection().Get(key);
                    if (grid != undefined && grid.BatchEditConfirmShowing != undefined) {
                        grid.BatchEditConfirmShowing.AddHandler(this.BatchEditConfirmHandler);
                    }
                }
            },
            RemoveConfirmHandlerForAllGrids: function (dropShownConfirmMessage) {
                for (var key in xaf.ConfirmUnsavedChangedController.Containers) {
                    var grid = ASPx.GetControlCollection().Get(key);
                    if (grid != undefined && grid.BatchEditConfirmShowing != undefined) {
                        grid.BatchEditConfirmShowing.RemoveHandler(grid.XafGridViewUpdateWatcherHelper.BatchEditConfirmHandler);
                        if (dropShownConfirmMessage) {
                            grid.XafGridViewUpdateWatcherHelper.wasConfirmMessage = false;
                        }
                    }
                }
            },
            BatchEditConfirmHandler: function (s, e) {
                e.cancel = true;
                s.XafGridViewUpdateWatcherHelper.RemoveConfirmHandlerForAllGrids(true);
            },
            ShowMessage: function (registerConfirmHandlers) {
                //if (this.HasChanges()) {
                if (xaf.ConfirmUnsavedChangedController.IsModified()) {
                    this.wasConfirmMessage = true;
                    if (registerConfirmHandlers) {
                        this.AddConfirmHandlerForAllGrids();
                    }
                    if (!confirm(this.GetConfirmUpdateText())) {
                        return false;
                    }
                }
                xaf.ConfirmUnsavedChangedController.DropModified(this.grid.name);
                return true;
            },
            GetName: function () {
                return "xaf_" + this.grid.name;
            },
            GetControlMainElement: function () {
                return this.grid.GetMainElement();
            },
            CanShowConfirm: function (requestOwnerID) {
                return !this.grid.RaiseBatchEditConfirmShowing(requestOwnerID);
            },
            HasChanges: function () {
                if (this.isBatchEdit) {
                    return xaf.ConfirmUnsavedChangedController.IsActive() && this.grid.batchEditApi.HasChanges();
                } else {
                    return xaf.ConfirmUnsavedChangedController.IsActive() && grid.IsEditing() && xaf.ConfirmUnsavedChangedController.IsModified(this.grid.name);
                }
            },
            GetConfirmUpdateText: function () {
                return xaf.ConfirmUnsavedChangedController.questionMessage;
            },
            NeedConfirmOnCallback: function (dxCallbackOwner, arg) {
                this.lastArg = arg;
                var updateOnCallback = ASPx.GetIsParent(dxCallbackOwner.GetMainElement(), this.GetControlMainElement());
                var result = updateOnCallback && !this.IsGridFuncCallback(dxCallbackOwner) && this.IsExclusionCallBackCommand(arg);
                if (this.isBatchEdit) {
                    if (!result) {
                        this.AddConfirmHandlerForAllGrids();
                    }
                    if (this.wasConfirmMessage) {
                        result = false;
                    }
                }
                this.wasConfirmMessage = false;
                return result;
            },
            IsExclusionCallBackCommand: function (arg) {
                if (typeof arg === 'string' || arg instanceof String) {
                    return !this.IsGridCancelEditCallBackCommand(arg) && !this.IsGridUpdateEditCallBackCommand(arg) &&
                           !this.IsGridSelectionCallBackCommand(arg) && !this.IsGridSelectionRowsCallBackCommand(arg) &&
                           !this.IsGridShowDetailCallBackCommand(arg) && !this.IsGridHideDetailCallBackCommand(arg) &&
                           !this.IsGridSelectionRowCallBackCommand(arg);
                }
                return false;
            },
            IsGridSelectionRowCallBackCommand: function (arg) {
                return arg ? arg.indexOf(ASPxClientGridViewCallbackCommand.Selection) !== -1 : false;
            },
            IsGridShowDetailCallBackCommand: function (arg) {
                return arg ? arg.indexOf(ASPxClientGridViewCallbackCommand.ShowDetailRow) !== -1 : false;
            },
            IsGridHideDetailCallBackCommand: function (arg) {
                return arg ? arg.indexOf(ASPxClientGridViewCallbackCommand.HideDetailRow) !== -1 : false;
            },
            IsGridCancelEditCallBackCommand: function (arg) {
                return arg ? arg.indexOf(ASPxClientGridViewCallbackCommand.CancelEdit) !== -1 : false;
            },
            IsGridSelectionCallBackCommand: function (arg) {
                return arg ? arg.indexOf(ASPxClientGridViewCallbackCommand.Selection) !== -1 : false;
            },
            IsGridSelectionRowsCallBackCommand: function (arg) {
                return arg ? arg.indexOf(ASPxClientGridViewCallbackCommand.SelectRows) !== -1 : false;
            },
            IsGridUpdateEditCallBackCommand: function (arg) {
                return arg ? arg.indexOf(ASPxClientGridViewCallbackCommand.UpdateEdit) !== -1 : false;
            },
            IsGridStartEditCallBackCommand: function (arg) {
                return arg ? arg.indexOf(ASPxClientGridViewCallbackCommand.StartEdit) !== -1 : false;
            },
            IsGridAddNewRowCallBackCommand: function (arg) {
                return arg ? arg.indexOf(ASPxClientGridViewCallbackCommand.AddNewRow) !== -1 : false;
            },
            ResetClientChanges: function () {
                if (ClientParams != undefined && !this.IsGridStartEditCallBackCommand(this.lastArg) && !this.IsGridAddNewRowCallBackCommand(this.lastArg)) {
                    ClientParams.Set("CancelEditGrid", true);
                }
            },
            IsGridFuncCallback: function (dxCallbackOwner) {
                if (dxCallbackOwner !== this.grid)
                    return false;
                var date = new Date();
                for (var i = 0; i < this.grid.funcCallbacks.length; i++) {
                    var callbackItem = this.grid.funcCallbacks[i];
                    if (callbackItem && (date - callbackItem.date) < this.GridFuncCallbackWaitingTimeout)
                        return true;
                }
                return false;
            }
        })
        )(grid);
    }
    this.IsActive = function () {
        return this.isActive;
    }
    this.DropModified = function (id) {
        if (id) {
            if (this.Containers[id] != undefined) {
                this.Containers[id] = false;
            }
        } else {
            for (var key in this.Containers) {
                this.Containers[key] = false;
            }
            for (var key in this.grids) {
                delete this.grids[key];
            }
        }

    }
    this.IsModified = function (id) {
        if (id) {
            if (this.Containers[id] != undefined) {
                return this.Containers[id];
            }
            return null;
        } else {
            for (var key in this.Containers) {
                if (this.Containers[key]) {
                    return true;
                }
            }
            for (var key in this.grids) {
                if (this.grids[key].batchEditApi.HasChanges()) {
                    return true;
                }
            }
            return false;
        }
    }
    this.RegisterBatchEditGrid = function (grid) {
        this.grids[grid.name] = grid;
    }
    this.LockConfirmOnBeforeUnloadInBatchEdit = function (grid) {
        this.ownerWatcher.clearLockerTimerDelay = this.clearLockerTimerCustomizeDelay;
        window.setTimeout(function () {
            xaf.ConfirmUnsavedChangedController.ownerWatcher.clearLockerTimerDelay = xaf.ConfirmUnsavedChangedController.clearLockerTimerDefaultDelay;
        }, this.clearLockerTimerCustomizeDelay + 1);
    }
    this.SetModified = function (isModified, containerId) {
        containerId = containerId ? containerId : this.mainContainerId;
        if (this.Containers[containerId] == undefined) {
            return;
        }
        this.Containers[containerId] = isModified;
    }
    this.AddContainer = function (viewId, containerId) {
        if (this.Containers[containerId] == undefined) {
            this.Containers[containerId] = false;
        }
        if (this.ViewContainersMap[viewId] == undefined) {
            this.ViewContainersMap[viewId] = [];
        }
        if (this.ViewContainersMap[viewId].indexOf(containerId) == -1) {
            this.ViewContainersMap[viewId].push(containerId);
        }
    }
    this.RemoveContainer = function (viewId) {
        if (this.ViewContainersMap[viewId] != undefined) {
            for (var i = 0; i < this.ViewContainersMap[viewId].length; i++) {
                var containerId = this.ViewContainersMap[viewId][i];
                delete this.Containers[containerId];
                delete this.grids[containerId];
            }
            delete this.ViewContainersMap[viewId];
        }
    }
    this.CustomizeNavBar = function (navBar) {
        navBar.oldSetSelectedItemInternal = navBar.SetSelectedItemInternal;
        navBar.SetSelectedItemInternal = function (indexPath, modifyHotTrackSelection) {
            if (!xaf.ConfirmUnsavedChangedController.IsModified()) {
                this.oldSetSelectedItemInternal(indexPath, modifyHotTrackSelection);
            }
        }
    }
    this.SetActive = function (isActive) {
        self.isActive = isActive;
        if (isActive) {
            this.AttachEventToElement(window, "beforeunload", this.CustomOnBeforeUnload);
        } else {
            this.DetachEventFromElement(window, "beforeunload", this.CustomOnBeforeUnload);
        }
    }
    this.SetMessage = function (message) {
        this.questionMessage = message;
    }
    this.GetParentContainer = function (s, e) {
        var parentsId = [];
        for (var id in this.Containers) {
            var container = document.getElementById(id);
            if (container) {
                if (ASPx.GetIsParent(container, document.getElementById(s.name))) {
                    parentsId.push(id);
                }
            }
        }
        if (parentsId.length >= 1) {
            while (parentsId.length > 1) {
                if (ASPx.GetIsParent(document.getElementById(parentsId[0]), document.getElementById(parentsId[1]))) {
                    parentsId.splice(0, 1);
                } else {
                    parentsId.splice(1, 1);
                }
            }
            return parentsId[0];
        } else {
            return null;
        }
    }

    this.EditorValueChanged = function (s, e) {
        var containerId = self.GetParentContainer(s, e);
        containerId = containerId ? containerId : self.mainContainerId;
        self.SetModified(true, containerId);
    }
    this.RegisterAction = function (actionId) {
        if (this.confirmActions.indexOf(actionId) == -1) {
            this.confirmActions.push(actionId);
        }
    }
    this.GetActionName = function (s, e) {
        if (e.item && e.item.menu) {
            return e.item.menu.name + '_' + e.item.name;
        }
        return e.item.name;
    }
    this.GetActionsParentName = function (s, e) {
        if (e.item && e.item.menu && e.item.parent && e.item.parent.name) {
            return e.item.menu.name + '_' + e.item.parent.name;
        }
        return "";
    }
    this.NeedShowMessageByAction = function (s, e, actionId) {
        if (!actionId) {
            actionId = this.GetActionName(s, e);
        }
        var actionsParentId = this.GetActionsParentName(s, e);
        return this.confirmActions.indexOf(actionId) > -1 || this.confirmActions.indexOf(actionsParentId) > -1;
    }
    this.CanProcessCallbackForAction = function (s, e) {
        if (this.IsActive()) {
            this.PreventDefault(e.htmlEvent);//handle page OnBeforeUnload
            if (this.NeedShowMessageByAction(s, e)) {
                return this.ShowMessage();
            }
        }
        return true;
    }
    this.CanProcessCallbackForNavigation = function (s, e) {
        return !this.IsActive() || this.ShowMessage();
    }
    this.CanProcessCallbackForChoiceAction = function (s, e, actionID) {
        if (this.IsActive()) {
            if (this.NeedShowMessageByAction(s, e, actionID) && !this.ShowMessage()) {
                s.SetSelectedIndex(s.currentSelectedIndex != undefined ? s.currentSelectedIndex : -1);
                return false;
            }
        }
        return true;
    }
    this.PreventClosing = function (s, e) {
        e.cancel = true;
        s.Closing.RemoveHandler(self.PreventClosing);
    }
    this.CanClosePopup = function (s, e) {
        var canClosePopup = true;
        if (this.IsActive()) {
            canClosePopup = this.ShowMessage();

            //handle popup close
            if (canClosePopup) {
                s.Closing.RemoveHandler(this.PreventClosing);
            } else {
                s.Closing.AddHandler(this.PreventClosing);
            }
        }
        return canClosePopup;
    }
    this.ShowMessage = function () {
        if (this.IsModified()) {
            if (!confirm(this.questionMessage)) {
                stopProgress();
                return false;
            }
        }
        if (this.ownerWatcher != null) { 
            this.LockConfirmOnBeforeUnloadInBatchEdit();
        }
        this.DropModified();
        return true;
    }
    this.CanUnloadPage = function () {
        return !this.IsActive() || !(this.IsModified() && !xaf.FormsManager.isSubmit);
    }
    this.PreventDefault = function (e) {
        if (e) {
            if (e.preventDefault) {
                e.preventDefault();
            }
            e.returnValue = false;
        }
    }
    this.CustomOnBeforeUnload = function (e) {
        if (!self.CanUnloadPage()) {
            stopProgress();
            e.returnValue = self.questionMessage;
            return self.questionMessage;
        }
        self.submitted = false;
        xaf.FormsManager.isSubmit = false;
    }
    this.AttachEventToElement = function (element, eventName, func, onlyBubbling) {
        if (element.addEventListener)
            element.addEventListener(eventName, func, !onlyBubbling);
        else
            element.attachEvent("on" + eventName, func);
    }
    this.DetachEventFromElement = function (element, eventName, func) {
        if (element.removeEventListener)
            element.removeEventListener(eventName, func, true);
        else
            element.detachEvent("on" + eventName, func);
    }
}

if (typeof xaf.ConfirmUnsavedChangedController == "undefined") {
    xaf.ConfirmUnsavedChangedController = xaf.ControllersManager.CreateController(ConfirmUnsavedChangedController);
}