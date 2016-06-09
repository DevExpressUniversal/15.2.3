(function () {

    var ClientSchedulerGlobalMouseHandler = ASPx.CreateClass(null, {
        constructor: function () {
            this.mouseHandlers = [];
        },
        AddHandler: function (handler) {
            this.mouseHandlers.push(handler);
        },
        RemoveHandler: function (handler) {
            var indx = -1;
            for (var i in this.mouseHandlers)
                if (this.mouseHandlers[i] == handler) {
                    indx = i;
                    break;
                }
            if (indx == -1)
                return;
            this.mouseHandlers.splice(indx, 1);
        },
        MouseMove: function (e) {
            for (var i in this.mouseHandlers)
                this.mouseHandlers[i].MouseMove(e)
        },
        MouseDown: function (e) {
            for (var i in this.mouseHandlers)
                this.mouseHandlers[i].MouseDown(e);
        },
        MouseUp: function (e) {
            for (var i in this.mouseHandlers)
                this.mouseHandlers[i].MouseUp(e);
        },
        MSPointerCancel: function (e) {
            for (var i in this.mouseHandlers)
                this.mouseHandlers[i].MSPointerCancel(e);
        },
        MSPointerUp: function (e) {
            for (var i in this.mouseHandlers)
                this.mouseHandlers[i].MSPointerUp(e);
        },
        MSPointerDown: function (e) {
            for (var i in this.mouseHandlers)
                this.mouseHandlers[i].MSPointerDown(e);
        },
        MSPointerMove: function (e) {
            for (var i in this.mouseHandlers)
                this.mouseHandlers[i].MSPointerMove(e);
        },
        TouchStart: function (e) {
            for (var i in this.mouseHandlers)
                this.mouseHandlers[i].TouchStart(e);
        },
        TouchEnd: function (e) {
            for (var i in this.mouseHandlers)
                this.mouseHandlers[i].TouchEnd(e);
        },
        TouchMove: function (e) {
            for (var i in this.mouseHandlers)
                this.mouseHandlers[i].TouchMove(e);
        }
    });

    ASPx.schedulerGlobalMouseHandler = new ClientSchedulerGlobalMouseHandler();

    ASPx.Evt.AttachEventToDocument("mouseup", function (e) {
        if (ASPx.schedulerGlobalMouseHandler != null)
            ASPx.schedulerGlobalMouseHandler.MouseUp(e);
    });
    ASPx.Evt.AttachEventToDocument("mousedown", function (e) {
        if (ASPx.schedulerGlobalMouseHandler != null)
            ASPx.schedulerGlobalMouseHandler.MouseDown(e);
    });
    ASPx.Evt.AttachEventToDocument("mousemove", function (e) {
        if (ASPx.schedulerGlobalMouseHandler != null)
            ASPx.schedulerGlobalMouseHandler.MouseMove(e);
    });

    if (ASPx.Browser.MSTouchUI) {
        ASPx.Evt.AttachEventToDocument(ASPx.TouchUIHelper.pointerMoveEventName, function (e) {
            if (ASPx.schedulerGlobalMouseHandler != null)
                ASPx.schedulerGlobalMouseHandler.MSPointerMove(e);
        });
        ASPx.Evt.AttachEventToDocument(ASPx.TouchUIHelper.pointerDownEventName, function (e) {
            if (ASPx.schedulerGlobalMouseHandler != null)
                ASPx.schedulerGlobalMouseHandler.MSPointerDown(e);
        });
        ASPx.Evt.AttachEventToDocument(ASPx.TouchUIHelper.pointerUpEventName, function (e) {
            if (ASPx.schedulerGlobalMouseHandler != null)
                ASPx.schedulerGlobalMouseHandler.MSPointerUp(e);
        });
        ASPx.Evt.AttachEventToDocument(ASPx.TouchUIHelper.pointerCancelEventName, function (e) {
            if (ASPx.schedulerGlobalMouseHandler != null)
                ASPx.schedulerGlobalMouseHandler.MSPointerCancel(e);
        });
    }

    if (ASPx.Browser.WebKitTouchUI) {
        ASPx.Evt.AttachEventToDocument("touchstart", function (e) {
            if (ASPx.schedulerGlobalMouseHandler != null)
                ASPx.schedulerGlobalMouseHandler.TouchStart(e);
        });
        ASPx.Evt.AttachEventToDocument("touchend", function (e) {
            if (ASPx.schedulerGlobalMouseHandler != null)
                ASPx.schedulerGlobalMouseHandler.TouchEnd(e);
        });
        ASPx.Evt.AttachEventToDocument("touchmove", function (e) {
            if (ASPx.schedulerGlobalMouseHandler != null)
                ASPx.schedulerGlobalMouseHandler.TouchMove(e);
        });
    }

    var ClientSchedulerMouseHandler = ASPx.CreateClass(null, {
        constructor: function (scheduler) {
            this.scheduler = scheduler;
            this.SwitchToDefaultState();
        },
        MainDivMouseClick: function (e) {
            this.state.MainDivMouseClick(e);
        },
        MainDivMouseDoubleClick: function (e) {
            this.state.MainDivMouseDoubleClick(e);
        },
        MainDivMouseUp: function (e) {
            this.state.MainDivMouseUp(e);
        },
        MainDivMouseDown: function (e) {
            this.state.MainDivMouseDown(e);
        },
        MouseDown: function (e) {
            this.state.MouseDown(e);
        },
        TouchStart: function (e) {
            this.state.TouchStart(e);
        },
        TouchEnd: function (e) {
            this.state.TouchEnd(e);
        },
        TouchMove: function (e) {
            this.state.TouchMove(e);
        },
        MouseMove: function (e) {
            return this.state.MouseMove(e);
        },
        MouseUp: function (e) {
            if (this.scheduler.preventTouchUIMouseScrolling)
                this.scheduler.ClearAllGesture();
            this.scheduler.preventTouchUIMouseScrolling = false;
            this.state.MouseUp(e);
        },
        MSPointerMove: function (e) {
            this.state.MSPointerMove(e);
        },
        MSPointerDown: function (e) {
            if (e.pointerType == "mouse")
                this.scheduler.preventTouchUIMouseScrolling = true;
            this.state.MSPointerDown(e);
        },
        MSPointerUp: function (e) {
            if (this.scheduler.preventTouchUIMouseScrolling)
                this.scheduler.ClearAllGesture();
            this.scheduler.preventTouchUIMouseScrolling = false;
            this.state.MSPointerUp(e);
            //this.scheduler.preventTouchUIMouseScrolling = false;
        },
        MSPointerCancel: function (e) {
            this.state.MSPointerCancel(e);
        },
        SwitchToAppointmentMouseDownState: function (appointmentId, e) {
            var newSate = new ClientSchedulerAppointmentMouseDownState(this.scheduler, this, appointmentId, e);
            this.SwitchStateCore(newSate);
        },
        SwitchToDefaultState: function (e) {
            var newSate = new ClientSchedulerDefaultMouseHandlerState(this.scheduler, this, e);
            this.SwitchStateCore(newSate);
        },
        SwitchToCellSelectionState: function (cell, e) {
            var newState = new ClientSchedulerCellSelectionMouseHandlerState(this.scheduler, this, cell, e);
            this.SwitchStateCore(newState);
        },
        SwitchToAppointmentDragState: function (appointmentId, e) {
            var newState = new ClientSchedulerAppointmentDragState(this.scheduler, this, appointmentId, e);
            this.SwitchStateCore(newState);
        },
        SwitchToAppointmentResizeState: function (appointmentId, e) {
            var newState = new ClientSchedulerAppointmentResizeState(this.scheduler, this, appointmentId, e);
            this.SwitchStateCore(newState);
        },
        SwitchToAppointmentSelectionTouchState: function (appointmentId, e) {
            var newState = new ClientSchedulerAppointmentSelectionTouchState(this.scheduler, this, appointmentId, e);
            this.SwitchStateCore(newState);
        },
        SwitchToCellSelectionTouchState: function (cell, e) {
            var newState = new ClientSchedulerCellSelectionTouchState(this.scheduler, this, cell, e);
            this.SwitchStateCore(newState);
        },
        SwitchStateCore: function (newSate) {
            if (this.state)
                this.state.Stop();
            this.state = newSate;
            this.state.Start();
        }
    });

    var ClientSchedulerMouseHandlerStateBase = ASPx.CreateClass(null, {
        constructor: function (scheduler, mouseHandler) {
            this.scheduler = scheduler;
            this.mouseHandler = mouseHandler;
        },
        MainDivMouseClick: function (e) {
        },
        MainDivMouseDoubleClick: function (e) {
        },
        MainDivMouseUp: function (e) {
        },
        MainDivMouseDown: function (e) {
        },
        MouseMove: function (e) {
        },
        MouseUp: function (e) {
        },
        MouseDown: function (e) {
        },
        Start: function () {
        },
        Stop: function () {
        },
        MSPointerUp: function (e) {
        },
        MSPointerDown: function (e) {
        },
        MSPointerMove: function (e) {
        },
        MSPointerCancel: function (e) {
        },
        TouchStart: function (e) {
        },
        TouchEnd: function (e) {
        },
        TouchMove: function (e) {
        },
        //private 
        CalcHitTest: function (e) {
            return this.scheduler.CalcHitTest(e);
        }
    });
    var ClientSchedulerDefaultMouseHandlerState = ASPx.CreateClass(ClientSchedulerMouseHandlerStateBase, {
        constructor: function (scheduler, mouseHandler, e) {
            this.constructor.prototype.constructor.call(this, scheduler, mouseHandler);
            this.lastEvent = e;
        },
        Start: function () {
            if (!this.lastEvent)
                return;
            var evt = ASPx.Evt.GetEvent(this.lastEvent);
            var eventSource = ASPx.Evt.GetEventSource(evt);
            eventSource = ASPx.GetParent(eventSource, ASPx.SchedulerTestIsSupportViewInfo);
            if (!ASPx.IsExists(eventSource)) {
                this.ResetTooltip();
                return;
            }
            var viewInfo = eventSource.viewInfo;
            var toolTipHelper = this.scheduler.schedulerToolTipHelper;
            if (viewInfo != this.lastViewInfo) {
                this.lastViewInfo = viewInfo;
                toolTipHelper.OnMouseOver(evt, viewInfo, viewInfo.toolTip);
            }
            toolTipHelper.OnMouseMove(evt, viewInfo, viewInfo.toolTip);

            this.lastEvent = null;
        },
        Stop: function (e) {
            this.ResetTooltip();
            var evt = ASPx.Evt.GetEvent(evt);
            if (this.scheduler.activeToolTip != null) {
                var element = ASPx.Evt.GetEventSource(evt);
                if (!ASPx.IsElementPartOfToolTip(element)) {
                    this.scheduler.HideAllToolTips();
                }
            }
            var popupDiv = this.scheduler.GetActivePopupDiv();
            if (popupDiv) {
                if (!this.scheduler.IsOverActivePopup(evt)) {
                    this.scheduler.HidePopupDiv(popupDiv);
                }
            }
        },
        MouseMove: function (e) {
            var eventSource = ASPx.Evt.GetEventSource(e);
            eventSource = ASPx.GetParent(eventSource, ASPx.SchedulerTestIsSupportViewInfo);
            if (!ASPx.IsExists(eventSource)) {
                this.ResetTooltip();
                return;
            }
            var viewInfo = eventSource.viewInfo;
            var toolTipHelper = this.scheduler.schedulerToolTipHelper;
            if (viewInfo != this.lastViewInfo) {
                this.lastViewInfo = viewInfo;
                toolTipHelper.OnMouseOver(e, viewInfo, viewInfo.toolTip);
            }
            toolTipHelper.OnMouseMove(e, viewInfo, viewInfo.toolTip);
        },
        MouseUp: function (evt) {
            var evt = ASPx.Evt.GetEvent(evt);
            if (this.scheduler.activeToolTip != null) {
                var element = ASPx.Evt.GetEventSource(evt);
                if (!ASPx.IsElementPartOfToolTip(element)) {
                    this.scheduler.HideAllToolTips();
                }
            }

            var popupDiv = this.scheduler.GetActivePopupDiv();
            if (popupDiv) {
                if (!this.scheduler.IsOverActivePopup(evt)) {
                    this.scheduler.HidePopupDiv(popupDiv);
                }
            }
        },
        MainDivMouseDown: function (e) {
            var hitTestResult = this.CalcHitTest(e);
            if (ASPx.IsExists(hitTestResult.appointmentDiv)) {
                var appointmentViewInfo = hitTestResult.appointmentDiv.appointmentViewInfo;
                var appointmentId = appointmentViewInfo.appointmentId;
                var ctrlKeyPressed = e.ctrlKey;
                if (hitTestResult.resizeDiv)
                    this.mouseHandler.SwitchToAppointmentResizeState(appointmentId, e);
                else
                    this.mouseHandler.SwitchToAppointmentMouseDownState(appointmentId, e);
                return true;
            }
            if (!ASPx.IsExists(hitTestResult.cell))
                return false;
            if (ASPx.IsExists(hitTestResult.selectionDiv) && ASPx.Evt.IsRightButtonPressed(e))
                return true;
            if (this.scheduler.CanSelect(e)) {
                this.mouseHandler.SwitchToCellSelectionState(hitTestResult.cell, e);
                return true;
            }
        },
        MainDivMouseDoubleClick: function (evt) {
            if (!this.scheduler.IsEventSourceInsideFormContainer(evt))
                ASPx.Selection.Clear();
            evt = ASPx.Evt.GetEvent(evt);
            var hitTestResult = this.CalcHitTest(evt);
            if (ASPx.IsExists(hitTestResult.appointmentDiv)) {
                var appointmentViewInfo = hitTestResult.appointmentDiv.appointmentViewInfo;
                var appointmentId = appointmentViewInfo.appointmentId;
                var handled = this.scheduler.RaiseAppointmentDoubleClick(appointmentId, evt);
                if (handled)
                    return;
                var ctrlKeyPressed = evt.ctrlKey;
                if (!ctrlKeyPressed) {
                    this.scheduler.appointmentSelection.SelectSingleAppointment(appointmentId);
                    this.scheduler.OnActivateInplaceEditor(evt);
                }
            }
        },
        MainDivMouseClick: function (evt) {
            evt = ASPx.Evt.GetEvent(evt);
            var hitTestResult = this.CalcHitTest(evt);
            if (ASPx.IsExists(hitTestResult.appointmentDiv)) {
                var appointmentViewInfo = hitTestResult.appointmentDiv.appointmentViewInfo;
                var appointmentId = appointmentViewInfo.appointmentId;
                this.scheduler.RaiseAppointmentClick(appointmentId, evt);
            }
        },
        TouchStart: function (e) {
            var hitTestResult = this.CalcHitTest(e);
            if (ASPx.IsExists(hitTestResult.appointmentDiv)) {
                var appointmentViewInfo = hitTestResult.appointmentDiv.appointmentViewInfo;
                var appointmentId = appointmentViewInfo.appointmentId;
                var ctrlKeyPressed = e.ctrlKey;
                this.mouseHandler.SwitchToAppointmentSelectionTouchState(appointmentId, e);
                return true;
            }
            if (!ASPx.IsExists(hitTestResult.cell))
                return false;
            if (ASPx.IsExists(hitTestResult.selectionDiv) && ASPx.Evt.IsRightButtonPressed(e))
                return true;
            if (this.scheduler.CanSelect(e)) {
                this.mouseHandler.SwitchToCellSelectionTouchState(hitTestResult.cell, e);
                return true;
            }
        },
        MSPointerDown: function (e) {
            if (e.pointerType == "mouse")
                return;
            var hitTestResult = this.CalcHitTest(e);
            if (ASPx.IsExists(hitTestResult.appointmentDiv)) {
                var appointmentViewInfo = hitTestResult.appointmentDiv.appointmentViewInfo;
                var appointmentId = appointmentViewInfo.appointmentId;
                var ctrlKeyPressed = e.ctrlKey;
                this.mouseHandler.SwitchToAppointmentSelectionTouchState(appointmentId, e);
                return true;
            }
            if (!ASPx.IsExists(hitTestResult.cell))
                return false;
            if (ASPx.IsExists(hitTestResult.selectionDiv) && ASPx.Evt.IsRightButtonPressed(e))
                return true;
            if (this.scheduler.CanSelect(e)) {
                this.mouseHandler.SwitchToCellSelectionTouchState(hitTestResult.cell, e);
                return true;
            }
        },
        ResetTooltip: function () {
            var toolTipHelper = this.scheduler.schedulerToolTipHelper;
            toolTipHelper.Reset();
        }
    });
    var ClientSchedulerCellSelectionMouseHandlerState = ASPx.CreateClass(ClientSchedulerMouseHandlerStateBase, {
        constructor: function (scheduler, mouseHandler, firstCell, e) {
            this.constructor.prototype.constructor.call(this, scheduler, mouseHandler);
            this.ctrlKeyPressed = e.ctrlKey;
            this.shiftKeyPressed = e.shiftKey;
            this.firstCell = firstCell;
        },
        Start: function () {
            this.scheduler.appointmentSelection.ClearSelection();
            if (!this.shiftKeyPressed || !ASPx.IsExists(this.scheduler.selection)) {
                var resource = this.scheduler.GetCellResource(this.firstCell);
                var interval = this.scheduler.GetCellInterval(this.firstCell);
                this.scheduler.SetSelectionCore(new ASPx.SchedulerSelection(interval, resource));
            } else
                this.ContinueSelection(this.firstCell);
        },
        MouseMove: function (e) {
            var cell = this.scheduler.CalcHitTest(e).cell;
            if (ASPx.IsExists(cell)) {
                this.ContinueSelection(cell);
            }
            return true;
        },
        MouseUp: function (e) {
            this.EndSelection(e);
            this.mouseHandler.SwitchToDefaultState(e);
            return true;
        },
        ContinueSelection: function (cell) {
            var interval = this.scheduler.GetCellInterval(cell);
            if (!ASPx.IsExists(interval))
                return;
            var newSelectionInterval = this.CalculateSelectionInterval(interval);
            if (!newSelectionInterval.Equals(this.scheduler.selection.interval)) {
                this.scheduler.SetSelectionInterval(newSelectionInterval);
            }
        },
        EndSelection: function (e) {
            this.CancelSelection();
            this.scheduler.OnSelectionChanged(e);
        },
        CancelSelection: function (e) {
            ASPx.schedulerSelectionHelper = null;
        },
        CalculateSelectionInterval: function (hitInterval) {
            var firstInterval = this.scheduler.selection.firstSelectedInterval;
            if (firstInterval.IntersectsWith(hitInterval))
                return new ASPxClientTimeInterval(firstInterval.GetStart(), ASPx.SchedulerGlobals.DateSubsWithTimezone(hitInterval.GetEnd(), firstInterval.GetStart()));
            if (hitInterval.GetStart() - firstInterval.GetEnd() >= 0)
                return new ASPxClientTimeInterval(firstInterval.GetStart(), ASPx.SchedulerGlobals.DateSubsWithTimezone(hitInterval.GetEnd(), firstInterval.GetStart()));
            else
                return new ASPxClientTimeInterval(hitInterval.GetStart(), ASPx.SchedulerGlobals.DateSubsWithTimezone(firstInterval.GetEnd(), hitInterval.GetStart()));
        }

    });
    var ClientSchedulerAppointmentMouseDownState = ASPx.CreateClass(ClientSchedulerMouseHandlerStateBase, {
        constructor: function (scheduler, mouseHandler, appointmentId, e) {
            this.constructor.prototype.constructor.call(this, scheduler, mouseHandler);
            this.appointmentId = appointmentId;
            this.ctrlKeyPressed = e.ctrlKey;
            this.shiftKeyPressed = e.shiftKey;
        },
        Start: function () {
            if (this.shiftKeyPressed)
                this.scheduler.appointmentSelection.AddAppointmentToSelection(this.appointmentId);
            else {
                if (!this.ctrlKeyPressed) {
                    if (!this.scheduler.appointmentSelection.IsAppointmentSelected(this.appointmentId))
                        this.scheduler.appointmentSelection.SelectSingleAppointment(this.appointmentId);
                } else {
                    if (!this.scheduler.appointmentSelection.IsAppointmentSelected(this.appointmentId))
                        this.scheduler.appointmentSelection.AddAppointmentToSelection(this.appointmentId);
                }
            }
        },
        MouseMove: function (e) {
            this.mouseHandler.SwitchToAppointmentDragState(this.appointmentId, e);
        },
        MainDivMouseUp: function (e) {
            this.mouseHandler.SwitchToDefaultState(e);
        }
    });
    var ClientSchedulerAppointmentResizeState = ASPx.CreateClass(ClientSchedulerAppointmentMouseDownState, {
        constructor: function (scheduler, mouseHandler, appointmentId, e) {
            this.constructor.prototype.constructor.call(this, scheduler, mouseHandler, appointmentId, e);
            this.helper = null;
            this.lastEventArgs = e;
        },
        Start: function () {
            ClientSchedulerAppointmentMouseDownState.prototype.Start.call(this);
            if (!this.lastEventArgs)
                return;
            var hitTestResult = this.CalcHitTest(this.lastEventArgs);
            var resizeDiv = hitTestResult.resizeDiv;
            if (!ASPx.IsExists(resizeDiv)) {
                this.mouseHandler.SwitchToDefaultState();
                return;
            }
            var mousePosition = ASPx.GetMousePosition(this.lastEventArgs);
            var sideHandler = this.GetResizeSideHandler(resizeDiv);
            this.helper = new ASPx.SchedulerResizeHelper(this.scheduler, hitTestResult.appointmentDiv, sideHandler, mousePosition);
            this.lastEventArgs = null;
        },
        Stop: function () {
            if (!this.helper)
                return;
            this.helper.Cancel();
            this.helper = null;
        },
        MouseMove: function (e) {
            if (!this.helper)
                return;
            this.helper.resize(e);
        },
        MainDivMouseUp: function (e) {
            if (!this.helper)
                return;
            this.helper.endResize();
            this.helper = null;
            this.mouseHandler.SwitchToDefaultState(e);
        },
        GetResizeSideHandler: function (resizeDiv) {
            if (!resizeDiv.dxResizeDivType)
                return ASPx.AppointmentResizeLeft;
            switch (resizeDiv.dxResizeDivType) {
                case ASPx.AppointmentResizeSideType.Left:
                    return ASPx.AppointmentResizeLeft;
                case ASPx.AppointmentResizeSideType.Right:
                    return ASPx.AppointmentResizeRight;
                case ASPx.AppointmentResizeSideType.Top:
                    return ASPx.AppointmentResizeTop;
                case ASPx.AppointmentResizeSideType.Bottom:
                    return ASPx.AppointmentResizeBottom;
            }
            return ASPx.AppointmentResizeLeft;
        }
    });
    var ClientSchedulerAppointmentDragState = ASPx.CreateClass(ClientSchedulerMouseHandlerStateBase, {
        constructor: function (scheduler, mouseHandler, appointmentId, e) {
            this.constructor.prototype.constructor.call(this, scheduler, mouseHandler);
            this.helper = null;
            this.lastEventArgs = e;
        },
        Start: function () {
            if (this.lastEventArgs == null)
                return;
            var hitTestResult = this.CalcHitTest(this.lastEventArgs);
            if (!ASPx.IsExists(hitTestResult.appointmentDiv)) {
                this.mouseHandler.SwitchToDefaultState();
                return;
            }
            var appointmentViewInfo = hitTestResult.appointmentDiv.appointmentViewInfo;
            var appointmentId = appointmentViewInfo.appointmentId;
            var ctrlKeyPressed = this.lastEventArgs.ctrlKey;
            var cell = this.scheduler.CalcHitTest(this.lastEventArgs).cell;
            this.helper = new ASPx.AppointmentDragHelper(this.scheduler, hitTestResult.appointmentDiv, cell, this.lastEventArgs, null, this.scheduler);
            this.lastEventArgs = null;
        },
        Stop: function () {
            if (this.helper == null)
                return;
            this.helper.cancelDrag();
            this.helper = null;
        },
        MouseMove: function (e) {
            this.helper.drag(e);
        },
        MouseUp: function (e) {
            if (this.helper == null)
                return;
            this.helper.endDrag(e);
            this.helper = null;
            this.mouseHandler.SwitchToDefaultState(e);
        }
    });
    var ClientSchedulerCellSelectionTouchState = ASPx.CreateClass(ClientSchedulerMouseHandlerStateBase, {
        constructor: function (scheduler, mouseHandler, cell, e) {
            this.constructor.prototype.constructor.call(this, scheduler, mouseHandler);
            this.cell = cell;
            this.lastEventArgs = e;
        },
        Start: function () {
            this.scheduler.appointmentSelection.ClearSelection();
            var resource = this.scheduler.GetCellResource(this.cell);
            var interval = this.scheduler.GetCellInterval(this.cell);
            this.scheduler.SetSelectionCore(new ASPx.SchedulerSelection(interval, resource));
            var toolTipHelper = this.scheduler.schedulerToolTipHelper;
            var selectionElement = this.scheduler.GetHightlightElementFromCell(this.cell);
            toolTipHelper.ShowToolTipInstantly(this.lastEventArgs, selectionElement);
            this.lastEventArgs = null;
        },
        TouchEnd: function (e) {//for touch ios only
            this.mouseHandler.SwitchToDefaultState(e);
        },
        MouseUp: function (e) {
            this.mouseHandler.SwitchToDefaultState(e);
        }
    });
    ClientSchedulerAppointmentSelectionTouchState = ASPx.CreateClass(ClientSchedulerMouseHandlerStateBase, {
        constructor: function (scheduler, mouseHandler, appointmentId, e) {
            this.constructor.prototype.constructor.call(this, scheduler, mouseHandler);
            this.appointmentId = appointmentId;
            this.lastEventArgs = e;
        },
        Start: function () {
            if (!this.scheduler.appointmentSelection.IsAppointmentSelected(this.appointmentId))
                this.scheduler.appointmentSelection.SelectSingleAppointment(this.appointmentId);
            var toolTipHelper = this.scheduler.schedulerToolTipHelper;
            toolTipHelper.ShowToolTipInstantly(this.lastEventArgs);
            this.lastEventArgs = null;
        },
        TouchEnd: function (e) {//for touch ios only
            this.mouseHandler.SwitchToDefaultState(e);
        },
        MouseUp: function (e) {
            this.mouseHandler.SwitchToDefaultState(e);
        }
    });


    ASPx.ClientSchedulerMouseHandler = ClientSchedulerMouseHandler;
    ASPx.ClientSchedulerMouseHandlerStateBase = ClientSchedulerMouseHandlerStateBase;
    ASPx.ClientSchedulerDefaultMouseHandlerState = ClientSchedulerDefaultMouseHandlerState;
    ASPx.ClientSchedulerCellSelectionMouseHandlerState = ClientSchedulerCellSelectionMouseHandlerState;
    ASPx.ClientSchedulerAppointmentMouseDownState = ClientSchedulerAppointmentMouseDownState;
    ASPx.ClientSchedulerAppointmentResizeState = ClientSchedulerAppointmentResizeState;
    ASPx.ClientSchedulerAppointmentDragState = ClientSchedulerAppointmentDragState;
})();