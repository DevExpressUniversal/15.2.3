

(function () {
    ////////////////////////////////////////////////////////////////////////////////
    // SchedulerMenuItemId (similar to XtraScheduler enum declaration)
    ////////////////////////////////////////////////////////////////////////////////
    var SchedulerMenuItemId = ASPx.CreateClass(null, {
    });

    SchedulerMenuItemId.Custom = "Custom";
    SchedulerMenuItemId.OpenAppointment = "OpenAppointment";
    SchedulerMenuItemId.PrintAppointment = "PrintAppointment";
    SchedulerMenuItemId.DeleteAppointment = "DeleteAppointment";
    SchedulerMenuItemId.EditSeries = "EditSeries";
    SchedulerMenuItemId.NewAppointment = "NewAppointment";
    SchedulerMenuItemId.NewAllDayEvent = "NewAllDayEvent";
    SchedulerMenuItemId.NewRecurringAppointment = "NewRecurringAppointment";
    SchedulerMenuItemId.NewRecurringEvent = "NewRecurringEvent";
    SchedulerMenuItemId.GotoThisDay = "GotoThisDay";
    SchedulerMenuItemId.GotoToday = "GotoToday";
    SchedulerMenuItemId.GotoDate = "GotoDate";
    SchedulerMenuItemId.OtherSettings = "OtherSettings";
    SchedulerMenuItemId.CustomizeCurrentView = "CustomizeCurrentView";
    SchedulerMenuItemId.CustomizeTimeRuler = "CustomizeTimeRuler";
    SchedulerMenuItemId.AppointmentDragMove = "AppointmentDragMove";
    SchedulerMenuItemId.AppointmentDragCopy = "AppointmentDragCopy";
    SchedulerMenuItemId.AppointmentDragCancel = "AppointmentDragCancel";
    SchedulerMenuItemId.StatusSubMenu = "StatusSubMenu";
    SchedulerMenuItemId.LabelSubMenu = "LabelSubMenu";
    SchedulerMenuItemId.RulerMenu = "RulerMenu";
    SchedulerMenuItemId.AppointmentMenu = "AppointmentMenu";
    SchedulerMenuItemId.DefaultMenu = "DefaultMenu";
    SchedulerMenuItemId.AppointmentDragMenu = "AppointmentDragMenu";
    SchedulerMenuItemId.RestoreOccurrence = "RestoreOccurrence";
    SchedulerMenuItemId.SwitchViewMenu = "SwitchViewMenu";
    SchedulerMenuItemId.SwitchToDayView = "SwitchToDayView";
    SchedulerMenuItemId.SwitchToWorkWeekView = "SwitchToWorkWeekView";
    SchedulerMenuItemId.SwitchToWeekView = "SwitchToWeekView";
    SchedulerMenuItemId.SwitchToMonthView = "SwitchToMonthView";
    SchedulerMenuItemId.SwitchToTimelineView = "SwitchToTimelineView";
    SchedulerMenuItemId.SwitchToFullWeekView = "SwitchToFullWeekView";
    SchedulerMenuItemId.TimeScaleEnable = "TimeScaleEnable";
    SchedulerMenuItemId.TimeScaleVisible = "TimeScaleVisible";
    ////////////////////////////////////////////////////////////////////////////////
    var AppointmentPropertyNames = ASPx.CreateClass(null, {});
    AppointmentPropertyNames.Normal = "Description;Subject;Start;AllDay;StatusId;LabelId;Location";
    AppointmentPropertyNames.Pattern = "Description;Subject;Start;AllDay;StatusId;LabelId;Location;Pattern";


    var SchedulerAutoHeightMode = ASPx.CreateClass(null, {});
    SchedulerAutoHeightMode.None = "None";
    SchedulerAutoHeightMode.FitToContent = "FitToContent";
    SchedulerAutoHeightMode.LimitHeight = "LimitHeight";

    var RelativeCoordinatesCalculatorBase = ASPx.CreateClass(null, {
        constructor: function (parentElement) {

            this.parentElement = parentElement;
            this.calculatedElements = [];
            this.innerParentElement = this.GetInnerParentElement();
            this.CreateLayers();
            this.SetInnerParentElementPosition();
        },
        GetInnerParentElement: function () {
            return this.parentElement;
        },
        SetInnerParentElementPosition: function () {
        },
        CreateLayers: function () {
            //alert("new layers");
            this.selectionLayer = this.CreateLayer();
            this.selectionLayer.id = this.parentElement.id + "selectionLayer";
            this.appointmentLayer = this.CreateLayer();
            this.appointmentLayer.id = this.parentElement.id + "appointmentLayer";
            this.navButtonLayer = this.CreateLayer();
            this.navButtonLayer.id = this.parentElement.id + "navButtonLayer";
            this.moreButtonLayer = this.CreateLayer();
            this.moreButtonLayer.id = this.parentElement.id + "moreButtonLayer";
            this.timeMarkerLayer = this.CreateLayer();
            this.timeMarkerLayer.id = this.parentElement.id + "timeMarkerLayer";
        },
        CreateLayer: function () {
            var result = document.createElement('DIV');
            if (!(ASPx.Browser.IE && ASPx.SchedulerBrowserHelper.IsIE9CompatibilityView()))
                result.style.height = "0";
            var emptyContent = document.createElement('DIV');
            emptyContent.style.position = "absolute";
            emptyContent.style.height = "0";
            result.appendChild(emptyContent);
            this.innerParentElement.appendChild(result);
            return result;
        },
        RemoveLayers: function (keepPrevAppointments) {
            ASPx.SchedulerGlobals.RemoveChildFromParent(this.innerParentElement, this.selectionLayer);
            if (keepPrevAppointments)
                this.innerParentElement.removeChild(this.appointmentLayer);
            else
                ASPx.SchedulerGlobals.RemoveChildFromParent(this.innerParentElement, this.appointmentLayer);
            ASPx.SchedulerGlobals.RemoveChildFromParent(this.innerParentElement, this.navButtonLayer);
            ASPx.SchedulerGlobals.RemoveChildFromParent(this.innerParentElement, this.moreButtonLayer);
            ASPx.SchedulerGlobals.RemoveChildFromParent(this.innerParentElement, this.timeMarkerLayer);
        },
        // NEW IParent interface impl begin
        CalcRelativeElementLeft: function (element) {
            if (!ASPx.IsExists(element.absoluteCellLeft)) {
                element.absoluteCellLeft = Math.round(ASPx.GetAbsoluteX(element)) - ASPx.GetAbsoluteX(this.innerParentElement);
                this.AddToCalculatedElements(element);
            }
            return element.absoluteCellLeft;
        },
        CalcRelativeElementRight: function (element) {
            if (!ASPx.IsExists(element.absoluteCellRight)) {
                element.absoluteCellRight = this.CalcRelativeElementLeft(element) + element.offsetWidth;
                this.AddToCalculatedElements(element);
            }
            return element.absoluteCellRight;
        },
        CalcRelativeElementTop: function (element) {
            if (!ASPx.IsExists(element.absoluteCellTop)) {
                element.absoluteCellTop = ASPx.GetAbsoluteY(element) - ASPx.GetAbsoluteY(this.innerParentElement);
                this.AddToCalculatedElements(element);
            }
            return element.absoluteCellTop;
        },
        CalcRelativeElementBottom: function (element) {
            if (!ASPx.IsExists(element.absoluteCellBottom)) {
                element.absoluteCellBottom = this.CalcRelativeElementTop(element) + element.offsetHeight;;
                this.AddToCalculatedElements(element);
            }
            return element.absoluteCellBottom;
        },
        CalcRelativeContainerBounds: function (viewInfo, container) {
            if (!ASPx.IsExists(container.isCalculated) || !container.isCalculated) {
                var firstCell = viewInfo.GetCell(container.containerIndex, 0);
                var lastCell = viewInfo.GetCell(container.containerIndex, container.cellCount - 1);
                var left = this.CalcRelativeElementLeft(firstCell);
                var dxtop = this.CalcRelativeElementTop(firstCell);
                var right = this.CalcRelativeElementRight(lastCell);
                var bottom = this.CalcRelativeElementBottom(lastCell);
                var bounds = {};
                bounds.left = left;
                bounds.top = dxtop;
                bounds.width = right - left;
                bounds.height = bottom - dxtop - 1;
                container.bounds = bounds;
                this.AddToCalculatedElements(container);
            }
            return container.bounds;
        },
        AddToCalculatedElements: function (element) {
            if (!ASPx.IsExists(element.isCalculated) || !element.isCalculated) {
                element.isCalculated = true;
                this.calculatedElements.push(element);
            }
        },
        ResetCache: function () {
            var count = this.calculatedElements.length;
            for (var i = 0; i < count; i++) {
                var element = this.calculatedElements[i];
                if (!ASPx.IsExists(element.bounds)) {
                    element.isCalculated = false;
                    element.absoluteCellLeft = null;
                    element.absoluteCellRight = null;
                    element.absoluteCellTop = null;
                    element.absoluteCellBottom = null;
                }
                else {
                    element.isCalculated = false;
                }
            }
        },
        Clear: function () {
            var count = this.calculatedElements.length;
            for (var i = 0; i < count; i++) {
                var element = this.calculatedElements[i];
                if (!ASPx.IsExists(element.bounds)) {
                    element.isCalculated = false;
                    element.absoluteCellLeft = null;
                    element.absoluteCellRight = null;
                    element.absoluteCellTop = null;
                    element.absoluteCellBottom = null;
                }
                else {
                    element.isCalculated = false;
                }
                if (ASPx.IsExists(element.hasMoreButton))
                    element.hasMoreButton = false;
            }
            this.ClearMoreButtonLayer();
            this.SetInnerParentElementPosition();
            ASPx.Data.ArrayClear(this.calculatedElements);
        },
        ClearMoreButtonLayer: function () {
            this.moreButtonLayer.innerHTML = "<div style='position:absolute;height:0'></div>";
        },
        AppendChildToLayer: function (child, layer) {
            layer.appendChild(child);
        },
        AppendChildToNavButtonLayer: function (child, layer) {
            if (!ASPx.IsExists(layer))
                layer = this.navButtonLayer;
            if (child.parentNode != layer)
                layer.appendChild(child);
        },
        AppendChildToAppointmentLayer: function (child) {
            this.appointmentLayer.appendChild(child);
        },
        PrepareAppointmentLayer: function (count) {
            if (count > 0)
                this.appointmentLayer.innerHTML = "";
        },
        AppendChildToMoreButtonLayer: function (child) {
            this.moreButtonLayer.appendChild(child);
        },
        AfterCalculateAppointments: function (appointmentCalculator) {
        },
        CalcCellHeight: function (cell) {
            return cell.offsetHeight;
        }
    });

    var RelativeCoordinatesCalculator = ASPx.CreateClass(RelativeCoordinatesCalculatorBase, {
        constructor: function (parentElement) {
            this.constructor.prototype.constructor.call(this, parentElement);
        }
    });

    var SchedulerHitTestResult = ASPx.CreateClass(null, {
        constructor: function (cell, selectionDiv, appointmentDiv, resizeDiv) {
            this.cell = cell;
            this.selectionDiv = selectionDiv;
            this.appointmentDiv = appointmentDiv;
            this.resizeDiv = resizeDiv;
        }
    });

    var RelativeCoordinatesCalculatorWithScrolling = ASPx.CreateClass(RelativeCoordinatesCalculatorBase, {
        constructor: function (parentElement, viewInfo, scrollableContainer, hasVerticalScrolling) {
            this.viewInfo = viewInfo;
            this.scrollableContainer = scrollableContainer;
            this.constructor.prototype.constructor.call(this, parentElement);
            this.bottomAppointmentIndent = 2;
            this.hasVerticalScrolling = hasVerticalScrolling;
        },
        GetInnerParentElement: function () {
            return this.scrollableContainer;
        },
        SetInnerParentElementPosition: function () {
            var containerCount = this.viewInfo.cellContainers.length;
            var cellCount = this.viewInfo.cellContainers[containerCount - 1].cellCount;
            var startCell = this.viewInfo.GetCell(0, 0);
            var endCell = this.viewInfo.GetCell(containerCount - 1, cellCount - 1);
            if (!ASPx.IsExists(startCell) || !ASPx.IsExists(endCell))
                return;
            var parentLeft = ASPx.GetAbsoluteX(startCell) - ASPx.GetAbsoluteX(this.parentElement);
            var parentRight = ASPx.GetAbsoluteX(endCell) - ASPx.GetAbsoluteX(this.parentElement) + endCell.offsetWidth;
            var parentTop = ASPx.GetAbsoluteY(startCell) - ASPx.GetAbsoluteY(this.parentElement);
            var parentBottom = ASPx.GetAbsoluteY(endCell) - ASPx.GetAbsoluteY(this.parentElement) + endCell.offsetHeight;
            this.innerParentElement.style.left = parentLeft + "px";
            this.innerParentElement.style.top = parentTop + "px";
            var elementWidth = parentRight - parentLeft;
            if (this.hasVerticalScrolling)
                elementWidth += ASPx.GetVerticalScrollBarWidth();
            if (ASPx.Browser.WebKitFamily)
                elementWidth += 1;
            this.innerParentElement.style.width = elementWidth + "px";
            this.innerParentElement.style.height = parentBottom - parentTop + "px";
        },
        CalcCellHeight: function (cell) {
            return this.innerParentElement.scrollHeight;
        },
        AfterCalculateAppointments: function (appointmentCalculator) {
            var startCell = this.viewInfo.GetCell(0, 0);
            if (ASPx.IsExists(this.viewInfo.scheduler.privateAllDayAreaHeight))
                this.innerParentElement.style.height = this.viewInfo.scheduler.privateAllDayAreaHeight;
            else {
                startCell.style.height = "";
                this.innerParentElement.style.height = "";
                if (ASPx.IsExists(appointmentCalculator.maxBottom) && appointmentCalculator.maxBottom > 0) {
                    var correctionHeight = 0;
                    if (ASPx.Browser.Firefox || ASPx.Browser.IE)
                        correctionHeight = 1;
                    var newHeight = Math.max(correctionHeight + appointmentCalculator.maxBottom + this.bottomAppointmentIndent, startCell.offsetHeight);
                    this.innerParentElement.style.height = newHeight + "px";
                }
            }
            if (ASPx.IsExists(startCell)) {
                var borderHeight = startCell.offsetHeight - startCell.clientHeight;
                if (Math.abs(startCell.offsetHeight - this.scrollableContainer.offsetHeight) > 1) {
                    var newStartCellHeight = this.scrollableContainer.offsetHeight - borderHeight;
                    if (newStartCellHeight > 0)
                        startCell.style.height = newStartCellHeight + 1 + "px";
                }
            }
            if (ASPx.Browser.Opera) {
                var scrollWidth = this.innerParentElement.offsetWidth - this.innerParentElement.clientWidth;
                this.innerParentElement.style.width = this.innerParentElement.offsetWidth + scrollWidth + "px";
            }
        }
    });

    ////////////////////////////////////////////////////////////////////////////////
    // ASPxClientScheduler
    ////////////////////////////////////////////////////////////////////////////////
    var ASPxClientScheduler = ASPx.CreateClass(ASPxClientControl, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);
            this.deferredFuncCallbackList = [];
            this.preventTouchUIMouseScrolling = false;
            this.schedulerIsInitialized = false; //B195319
            this.schedulerAfterInitializedCalled = false; //B196717
            //this.appointmentsSnapToCell = false;
            this.cellIdRegExp = /DXCnt(([hv])(\d+))_(\d+)/;
            this.ActiveViewChanging = new ASPxClientEvent();
            this.ActiveViewChanged = new ASPxClientEvent();
            this.SelectionChanged = new ASPxClientEvent();
            this.SelectionChanging = new ASPxClientEvent();
            this.VisibleIntervalChanged = new ASPxClientEvent();
            this.MoreButtonClicked = new ASPxClientEvent();
            this.AppointmentsSelectionChanged = new ASPxClientEvent();
            this.MenuItemClicked = new ASPxClientEvent();
            this.AppointmentDrop = new ASPxClientEvent();
            this.AppointmentResize = new ASPxClientEvent();
            this.AppointmentClick = new ASPxClientEvent();
            this.AppointmentDoubleClick = new ASPxClientEvent();
            this.MouseUp = new ASPxClientEvent();
            this.AppointmentDeleting = new ASPxClientEvent();
            //multi select 

            //this.privateAllowAppointmentCreate = ASPxClientUsedAppointmentType.All;
            //this.onWindowResizing = new Function("OnWindowResizingStub(\"" + this.name + "\");");
            this.isInsideBeginInit = false;
            this.isReadyForCallbacks = false;
            this.changedBlocks = null;
            this.isCallbackMode = true;
            this.supportGestures = true;
            //this.appointmentsBlockChanged = true;
            //this.formMode = false;
            //TODO debug only 

            this.menuManager = new SchedulerMenuManager(this);

            this.activeFormType = ASPx.SchedulerFormType.None;
            this.aptFormVisibility = ASPx.SchedulerFormVisibility.None;
            this.gotoDateFormVisibility = ASPx.SchedulerFormVisibility.None;
            this.recurrentAppointmentDeleteFormVisibility = ASPx.SchedulerFormVisibility.None;
            this.privateAllowAppointmentMultiSelect = true;
            this.privateShowAllAppointmentsOnTimeCells = false;
            this.contextMenuHandlers = [];
            this.prevCallbackResults = {};
            this.funcCallbacks = [];
            this.funcCallbackCount = 0;
            this.visibleIntervals = [];
            this.topRowTimeManager = new TopRowTimeManager(this);
            this.leftColumnPadding = 0;
            this.rightColumnPadding = 2;
            this.appointmentVerticalInterspacing = 2;
            this.propertyController = new ASPx.AppointmentPropertyApplyController(this);
            this.menuContainerChanger = new ASPx.SchedulerMenuContainerChanger(this);
            this.toolTipContainerChanger = new ASPx.SchedulerToolTipContainerChanger(this);
            this.formContainerChanger = new ASPx.SchedulerFormContainerChanger(this);
            this.defaultAutoHeightCellMinimum = 5; //
            this.restoreBlockInfoCollection = [];
            this.mouseHandler = new ASPx.ClientSchedulerMouseHandler(this);
            this.keyboardHandler = new ASPx.ClientSchedulerKeyboardHandler(this);
            if (ASPx.schedulerGlobalMouseHandler != null)
                ASPx.schedulerGlobalMouseHandler.AddHandler(this.mouseHandler);
            if (ASPx.schedulerGlobalKeyboardHandler)
                ASPx.schedulerGlobalKeyboardHandler.AddHandler(this.keyboardHandler);
            this.loadImageChecker = new ASPx.SchedulerLoadImageChecker(this);
            this.loadImageChecker.disabled = !ASPx.Browser.Chrome;
        },
        _constDXAppointment: function () { return "_AptDiv"; },
        _constDXSchedulerContentCell: function () { return "DXCnt"; },
        _constDXSelectionDiv: function () { return "selectionDiv"; },
        _constDXAppointmentAdorner: function () { return "aptAdornerDiv"; },
        _constEmptyResource: function () { return "null"; },
                
        Initialize: function () {
            this.constructor.prototype.Initialize.call(this);
            this.EndInit();
        },
        BeginInit: function () {
            this.TimeMarkerBeginUpdate();
            this.reminderTimer = null;
            if (ASPx.IsExists(this.onCallback))
                this.onCallback();
            this.isInsideBeginInit = true;
            ASPx.ControlResizeManager.Remove(this);
            this.syncColumnCount = -1;
            this.syncMasterRowIndex = -1;
            //this.appointments = [];
            this.appointmentSelection = new ASPx.AppointmentSelection(this);
            this.appointmentSelection.BeginUpdate();
            this.selection = null;
            this.navButtonAnchors = [];
            this.ClearNavigationButtons();
            this.appointments = {};
            this.nonpermanentAppointments = {};
            this.timelineHeaderLevels = [];
            this.privateAllDayAreaHeight = null;
            this.privateShowMoreButtonsOnEachColumn = false;
            this.privateDisableSnapToCells = false;
            this.RecreateViewInfo();

            this.privateRaiseActiveViewTypeChanged = false;
            this.privateRaiseVisibleIntervalChanged = false;
            this.leftColumnPadding = 0
            this.rightColumnPadding = 2;
            this.appointmentVerticalInterspacing = 2;
            this.timeIndicatorVisibility = ASPxClientTimeIndicatorVisibility.Never;
            this.ResetTimeRulers();
            this.cellAutoHeightMode = SchedulerAutoHeightMode.None;
            this.cellAutoHeightConstrant = [0, 0];
            this.schedulerToolTipHelper = new ASPx.SchedulerToolTipHelper();
            this.RestoreBlockProperties();
        },
        RecreateViewInfo: function () {
            var usePrevContainer = this.changedBlocks && !this.changedBlocks.ContainerBlockChanged;
            var usePrevNavButtons = this.changedBlocks && !this.changedBlocks.NavButtonsBlockChanged;
            if (!usePrevNavButtons)
                this.navButtonsCache = {};
            if (usePrevContainer) {
                var keepPrevAppointment = this.changedBlocks && !this.changedBlocks.AppointmentsBlockChanged;
                this.horizontalViewInfo.parent.RemoveLayers(keepPrevAppointment);
                this.verticalViewInfo.parent.RemoveLayers(keepPrevAppointment);
                this.ResetCellsAbsolutePosition();
            }
            if (this.horizontalViewInfo)
                this.horizontalViewInfo.Dispose();
            if (this.verticalViewInfo)
                this.verticalViewInfo.Dispose();

            this.horizontalViewInfo = new ASPx.HorizontalSchedulerViewInfo(this, this.horizontalViewInfo, usePrevContainer);
            this.verticalViewInfo = new ASPx.VerticalSchedulerViewInfo(this, this.verticalViewInfo, usePrevContainer);
        },
        EndInit: function () {
            this.loadImageChecker.Reset();
            var topRowTimeState = this.topRowTimeManager.GetTopRowTimeState(this.privateActiveViewType); //save topRowTime - preserve value from damage
            this.SubscribeEvents();
            this.isInsideBeginInit = false;
            this.resourceNavigatorRow = this.GetChildElement("resourceNavigatorRow");
            this.commonControlsBlock = ASPx.GetElementById(this.name + "_commonControlsBlock_innerContent");
            this.innerContentElement = ASPx.GetElementById(this.name + "_containerBlock_content");
            this.containerTable = ASPx.GetElementById(this.name + "_containerBlock_containerTable");
            this.containerCell = ASPx.GetElementById(this.name + "_containerBlock_innerContent");

            this.selectionDiv = ASPx.GetElementById(this.name + "_commonControlsBlock_selectionDiv");

            this.topResizeDiv = ASPx.GetElementById(this.name + "_commonControlsBlock_topResizeControlDiv");
            this.bottomResizeDiv = ASPx.GetElementById(this.name + "_commonControlsBlock_bottomResizeControlDiv");
            this.leftResizeDiv = ASPx.GetElementById(this.name + "_commonControlsBlock_leftResizeControlDiv");
            this.rightResizeDiv = ASPx.GetElementById(this.name + "_commonControlsBlock_rightResizeControlDiv");
            this.aptAdorner = ASPx.GetElementById(this.name + "_commonControlsBlock_aptAdornerDiv");

            this.timeMarkerImage = ASPx.GetElementById(this.name + "_containerBlock_timeMarkerImg");
            this.timeMarkerLine = ASPx.GetElementById(this.name + "_containerBlock_timeMarkerDiv");

            this.mainDiv = ASPx.GetElementById(this.name);
            this.ObtainCellTables();
            this.AddClearSelectionEvent();  // Q416890, B234069
            this.PrepareSchedulerViewInfos();
            this.appointmentSelection.Prepare();
            if (ASPx.IsExists(this.innerContentElement)) {
                this.RecalcLayout();
                this.ShowCellSelection();
                this.topRowTimeManager.SetTopRowTimeState(topRowTimeState, this.privateActiveViewType); //???
                ASPx.ControlResizeManager.SafeAdd(this);
                this.EnableResize();
            }
            this.appointmentSelection.EndUpdate();
            this.HideLoadingElements();
            //ASPx.RelatedControlManager.Unshade(this);        
            this.formPopupIdDeferred = null;
            this.isReadyForCallbacks = true;
            this.StartReminderTimer();
            this.EnableToolTips();
            this.SetTimeMarkerTimer();
            this.TimeMarkerEndUpdate();
            this.InitScrolling();
            this.schedulerIsInitialized = true; // B195319
            this.loadImageChecker.Add(this.innerContentElement);
            this.LinkToMessageBox();
        },
        LinkToMessageBox: function () {
            var newMessageBox = ASPxSchedulerMessageBoxBase.SchedulerLink[this.name];
            if (newMessageBox == null)
                return;
            this.messageBox = newMessageBox;
            ASPxSchedulerMessageBoxBase.SchedulerLink[this.name] = null;
            this.messageBox.LinkToPopup(this.messageBoxPopup);
        },
        NeedPreventTouchUIMouseScrolling: function (element) {
            return this.preventTouchUIMouseScrolling;
        },
        AllowStartGesture: function () {
            return !this.preventTouchUIMouseScrolling;
        },
        OnCallbackFinalized: function () {
            this.RaiseClientEvents();
            this.menuManager.ShowDeferredMenus();
        },
        InitScrolling: function () {
            //if (ASPx.Browser.WebKitTouchUI) {
            //    var scrollContainer = this.GetContainerElementById("verticalScrollContainer");
            //    if (scrollContainer)
            //        ASPx.TouchUIHelper.MakeScrollable(scrollContainer, { showHorizontalScrollbar: false });
            //}
        },
        AdjustControlCore: function () {
            if (!this.schedulerIsInitialized)//B195319
                return;
            //B186303
            this.ResetVerticalResourceHeadersCache();
            //B145788
            var topRowTimeState = this.topRowTimeManager.GetTopRowTimeState(this.privateActiveViewType); //save topRowTime - preserve value from damage
            this.topRowTimeManager.SetTopRowTimeState(topRowTimeState, this.privateActiveViewType); //???
            if (this.schedulerAfterInitializedCalled)//B196717, Q481892 
                this.RecalcLayout();
        },
        AfterInitialize: function () {
            this.constructor.prototype.AfterInitialize.call(this);
            this.schedulerAfterInitializedCalled = true;
        },
        AssignSlideAnimationDirectionByDate: function (newDate) {
            this.slideAnimationDirection = this.IsCallbackAnimationEnabled() ? ((this.visibleIntervals[0].start < newDate) ? ASPx.AnimationHelper.SLIDE_LEFT_DIRECTION : ASPx.AnimationHelper.SLIDE_RIGHT_DIRECTION) : null;
        },
        RestoreBlockProperties: function () {
            for (var i = 0; i < this.restoreBlockInfoCollection.length; i++) {
                var blockInfo = this.restoreBlockInfoCollection[i];
                this.PrepareBlockProperties(blockInfo.blockId, blockInfo.params, false);
            }

            ASPx.Data.ArrayClear(this.restoreBlockInfoCollection);
        },
        PrepareBlockProperties: function (clientId, params, needRestoreParams) {
            needRestoreParams = ASPx.GetDefinedValue(needRestoreParams, true); //by default restore params

            var block = ASPx.GetElementById(clientId + "_innerContent");
            if (!block)
                return;
            for (var propertyPath in params) {
                var oldValue = this.SetPropertyPath(block, propertyPath, params[propertyPath]);
                if (oldValue)
                    params[propertyPath] = oldValue;
            }
            if (needRestoreParams)
                this.restoreBlockInfoCollection.push(new ASPx.SchedulerBlockPropertiesInfo(clientId, params));
        },
        SetPropertyPath: function (target, propertyPath, value) {
            var propertyPathParts = propertyPath.split('.');
            var partCount = propertyPathParts.length;
            if (partCount < 1)
                return;

            var popertyTarget = target;
            for (var i = 0; i < partCount - 1; i++) {
                var pathPart = propertyPathParts[i]
                popertyTarget = popertyTarget[pathPart];
            }
            var propertyName = propertyPathParts[partCount - 1];
            var oldValue = popertyTarget[propertyName];
            popertyTarget[propertyName] = value;
            return oldValue;
        },
        ConvertToSchedulerUTCDates: function (datesInMs) {
            var dates = [];
            for (var i = 0; i < datesInMs.length; i++) {
                var date = new SchedulerUTCDate(datesInMs[i]);
                dates.push(date);
            }
            return dates;
        },
        AddClearSelectionEvent: function () {
            if (ASPx.Browser.Chrome) {
                var mainElement = this.GetMainElement();
                if (mainElement && !mainElement.onselectstart)
                    mainElement.onselectstart = function () { return false; }
            }
        },
        SetTopRowTimeField: function (dayViewStateString, workWeekViewStateString, fullWeekViewStateString) {
            this.topRowTimeManager.SetTopRowTimeField(dayViewStateString, workWeekViewStateString, fullWeekViewStateString);
        },
        SetVisibleInterval: function (transferVisibleInterval) {
            ASPx.Data.ArrayClear(this.visibleIntervals);
            var count = transferVisibleInterval.length;
            var propertyApplyController = new ASPx.SchedulerPropertyApplyController(this);
            var result = [];
            for (var i = 0; i < count; i++) {
                var transerInterval = transferVisibleInterval[i];
                var interval = propertyApplyController.CreateIntervalPropertyValue(null, transerInterval);
                this.visibleIntervals.push(interval);
            }
        },
        /*Reminders support*/
        SetReminders: function (delay) {
            this.reminderTimer = new ASPx.SchedulerTimer(delay, ASPx.CreateDelegate(this.ReminderAlerted, this));;
        },
        StartReminderTimer: function () {
            if (!this.reminderTimer)
                return;
            this.reminderTimer.Start();
        },
        StopReminderTimer: function () {
            if (!this.reminderTimer)
                return;
            this.reminderTimer.Pause();
        },
        DisableReminderTimer: function () {
            if (!this.reminderTimer)
                return;
            this.reminderTimer.BeginDeferredAlert();
        },
        EnableReminderTimer: function () {
            if (!this.reminderTimer)
                return;
            this.reminderTimer.EndDeferredAlert();
        },
        ReminderAlerted: function () {
            this.menuManager.HideMenu();
            return this.RaiseCallback("PROCESSREMINDER|");
        },

        AddContextMenuEvent: function (elementId, handlerFunc) {
            this.contextMenuHandlers.push([elementId, handlerFunc]);
        },
        SubscribeEvents: function () {
            var count = this.contextMenuHandlers.length;
            for (var i = 0; i < count; i++) {
                var current = this.contextMenuHandlers[i];
                var element = ASPx.GetElementById(current[0]);
                if (ASPx.IsExists(element)) {
                    element.oncontextmenu = new Function("event", current[1]);
                    //ASPx.Evt.AttachEventToElement(element, "contextmenu", new Function("event", current[1]));
                }
            }
        },
        UnsubscribeEvents: function () {
        },
        /*gestures*/
        CanHandleGesture: function (evt) {
            var source = ASPx.Evt.GetEventSource(evt);
            if (ASPx.GetParentByPartialId(source, "navBtnDiv") || ASPx.GetParentByPartialId(source, "MoreButton")) return false;

            if (ASPx.GetIsParent(this.containerCell, source) && (!this.horizontalParent || !ASPx.GetIsParent(this.horizontalParent.appointmentLayer, source)) &&
                (!this.verticalParent || !ASPx.GetIsParent(this.verticalParent.appointmentLayer, source))) {
                var verticalScrollContainer = this.GetContainerElementById("verticalScrollContainer");
                var verticalContainer = this.GetContainerElementById("verticalContainer");
                return (verticalScrollContainer == null) || !ASPx.GetIsParent(verticalScrollContainer, source) || ASPx.GetIsParent(verticalContainer, source);
            }
            return false;
        },
        AllowExecuteGesture: function (value) {
            return !this.preventTouchUIMouseScrolling;;
        },
        ExecuteGesture: function (value) {
            this.slideAnimationDirection = value > 0 ? ASPx.AnimationHelper.SLIDE_RIGHT_DIRECTION : ASPx.AnimationHelper.SLIDE_LEFT_DIRECTION;
            this.RaiseCallback(value > 0 ? "BACK|" : "FORWARD|");
        },
        ClearAllGesture: function () {//fo ie with touch monitor and mouse action instead touch
            this.repeatedGestureValue = 0;
            this.repeatedGestureCount = 0;
        },
        /*popup support*/
        IsOverActivePopup: function (evt) {
            return ASPx.PopupUtils.FindEventSourceParentByTestFunc(evt, ASPx.CreateDelegate(this.TestOverActivePopupDiv, this)) != null;
        },
        TestOverActivePopupDiv: function (element) {
            if (element === this.GetActivePopupDiv())
                return true;
            return false;
        },
        ShowPopupDiv: function (div) {
            this.activePopupDiv = div;
            ASPx.SetElementDisplay(div, true);
        },
        HidePopupDiv: function (div) {
            this.activePopupDiv = null;
            ASPx.SetElementDisplay(div, false);
        },
        GetActivePopupDiv: function () {
            return this.activePopupDiv;
        },
        /*--------------*/
        RaiseClientEvents: function () {
            if (ASPx.IsExists(this.RaiseActiveViewChanged) && this.privateRaiseActiveViewTypeChanged)
                this.RaiseActiveViewChanged();
            if (ASPx.IsExists(this.RaiseVisibleIntervalChanged) && this.privateRaiseVisibleIntervalChanged)
                this.RaiseVisibleIntervalChanged();
            if (this.deferredRaiseSelectionChanged) {
                this.deferredRaiseSelectionChanged = false;
                this.RaiseSelectionChanged();
            }
        },
        GetCurrentSize: function () {
            if (this.mainDiv && this.mainDiv.parentNode)
                return [this.innerContentElement.clientWidth, this.innerContentElement.clientHeight];
            else
                return null;
        },
        PrivateSetActiveViewType: function (newViewType) {
            var activeViewType = this.GetActiveViewType();
            if (activeViewType == newViewType)
                return;
            this.privateActiveViewType = newViewType;
            this.privateRaiseActiveViewTypeChanged = true;
        },
        PrivateSetAllDayAreaHeight: function (height) {
            this.privateAllDayAreaHeight = height;
        },
        PrivateSetGroupType: function (newGroupType) {
            this.privateGroupType = newGroupType;
            this.privateActualGroupType = newGroupType;
        },
        PrivateSetActualGroupType: function (newActualGroupType) {
            this.privateActualGroupType = newActualGroupType;
        },
        PrivateSetAppointmentHeight: function (height) {
            this.privateAppointmentHeight = parseInt(height);
        },
        GetActualGroupType: function () {
            return this.privateActualGroupType;
        },
        BeginAppointmentResizeAtLeft: function (appointmentViewInfo, mousePosition) {
            var appointmentDiv = appointmentViewInfo.contentDiv;
            if (ASPx.IsExists(appointmentDiv))
                new ASPx.SchedulerResizeHelper(this, appointmentDiv, ASPx.AppointmentResizeLeft, mousePosition);
        },
        BeginAppointmentResizeAtRight: function (appointmentViewInfo, mousePosition) {
            var appointmentDiv = appointmentViewInfo.contentDiv;
            if (ASPx.IsExists(appointmentDiv))
                new ASPx.SchedulerResizeHelper(this, appointmentDiv, ASPx.AppointmentResizeRight, mousePosition);
        },
        BeginAppointmentResizeAtTop: function (appointmentViewInfo, mousePosition) {
            var appointmentDiv = appointmentViewInfo.contentDiv;
            if (ASPx.IsExists(appointmentDiv))
                new ASPx.SchedulerResizeHelper(this, appointmentDiv, ASPx.AppointmentResizeTop, mousePosition);
        },
        BeginAppointmentResizeAtBottom: function (appointmentViewInfo, mousePosition) {
            var appointmentDiv = appointmentViewInfo.contentDiv;
            if (ASPx.IsExists(appointmentDiv))
                new ASPx.SchedulerResizeHelper(this, appointmentDiv, ASPx.AppointmentResizeBottom, mousePosition);
        },
        ObtainCellTables: function () {
            this.horzTable = this.GetContainerElementById("horzContainerTable");
            if (!ASPx.IsExists(this.horzTable))
                this.horzTable = this.GetContainerElementById("content");
            this.vertTable = this.GetContainerElementById("vertTable");
            if (!ASPx.IsExists(this.vertTable))
                this.vertTable = this.GetContainerElementById("content");
        },
        GetIsDayBasedView: function () {
            return this.privateActiveViewType == ASPxSchedulerViewType.Day || this.privateActiveViewType == ASPxSchedulerViewType.WorkWeek || this.privateActiveViewType == ASPxSchedulerViewType.FullWeek;
        },
        PrepareSchedulerViewInfos: function () {
            this.horizontalViewInfo.Initialize(this.horzTable);
            this.verticalViewInfo.Initialize(this.vertTable);
            var horizontalContainer = this.GetContainerElementById("horizontalContainer");
            var dayBasedView = this.GetIsDayBasedView();
            var verticalContainer = this.GetContainerElementById("verticalContainer");
            if (!ASPx.IsExists(horizontalContainer))
                this.horizontalParent = new RelativeCoordinatesCalculator(this.containerCell);
            else {
                var scrollableContainer = this.GetContainerElementById("scrollableContainer");
                if (ASPx.IsExists(scrollableContainer)) {
                    var hasScrolling = this.HasVerticalContainerScrolling();
                    this.horizontalParent = new RelativeCoordinatesCalculatorWithScrolling(horizontalContainer, this.horizontalViewInfo, scrollableContainer, hasScrolling);
                }
                else
                    this.horizontalParent = new RelativeCoordinatesCalculator(horizontalContainer);
            }
            this.horizontalViewInfo.Prepare(this.horizontalParent);


            if (!ASPx.IsExists(verticalContainer))
                this.verticalParent = new RelativeCoordinatesCalculator(this.containerCell);
            else
                this.verticalParent = new RelativeCoordinatesCalculator(verticalContainer);

            this.verticalViewInfo.Prepare(this.verticalParent);
        },
        HasVerticalContainerScrolling: function () {
            var masterRow = this.vertTable.rows[this.syncMasterRowIndex];
            var slaveRow = this.horzTable.rows[this.syncSlaveRowIndex];
            return slaveRow.cells.length > this.syncColumnCount;
        },
        EnableResize: function () {
            this.processResize = true;
        },
        DisableResize: function () {
            this.processResize = false;
        },
        IsResizeEnabled: function () {
            return ASPx.IsExists(this.processResize) && this.processResize;
        },
        OnWindowResized: function () {
            if (this.isInsideBeginInit || !this.IsResizeEnabled())
                return;
            this.DisableResize();
            //this.ResetSyncSlaveCellsWidth();
            this.ResetCellsAbsolutePosition();
            this.RecalcLayout();
            this.ShowCellSelection();
            this.AlignNavigationButtons();
            this.appointmentSelection.RecalcSelection();
            this.EnableResize();
            this.ProcessShowFormPopupWindowDeferred(); //try to show form when visible changed
        },
        OnSizeChecked: function () {
        },
        ResetCellsAbsolutePosition: function () {
            this.verticalViewInfo.parent.Clear();
            this.horizontalViewInfo.parent.Clear();
        },
        FormatHeadersContentOptimally: function () {
            this.FormatHeadersContentOptimallyCore(this.datesForFormatsWithoutYearToolTips, this.datesForFormatsWithoutYearLocations, this.datesForFormatsWithoutYear, this.formatsWithoutYear, ASPx.SchedulerMeasurer.SetOptimalHeadersContent);
            this.FormatHeadersContentOptimallyCore(this.datesForFormatsWithoutYearAndWeekDayToolTips, this.datesForFormatsWithoutYearAndWeekDayLocations, this.datesForFormatsWithoutYearAndWeekDay, this.formatsWithoutYearAndWeekDay, ASPx.SchedulerMeasurer.SetOptimalHeadersContent);
            this.FormatHeadersContentOptimallyCore(this.datesForFormatsNewYearLocationsToolTips, this.datesForFormatsNewYearLocations, this.datesForFormatsNewYear, this.formatsNewYear, ASPx.SchedulerMeasurer.SetOptimalHeadersContent);
            this.FormatHeadersContentOptimallyCore(this.daysForDayOfWeekFormatsLocationsToolTips, this.daysForDayOfWeekFormatsLocations, this.daysForDayOfWeekFormats, null, ASPx.SchedulerMeasurer.SetOptimalDayOfWeekHeadersContent);
            this.FormatHeadersContentOptimallyCore(this.datesForDayNumberFormatToolTips, this.datesForDayNumberFormatLocations, this.datesForDayNumberFormat, null, ASPx.SchedulerMeasurer.SetOptimalDayNumberHeaderContent);
            this.FormatHeadersContentOptimallyCore(this.datesForDateCustomFormatToolTips, this.datesForDateCustomFormatLocations, this.datesForDateCustomFormat, this.datesForDateCustomFormatCaptions, ASPx.SchedulerMeasurer.SetCustomDateHeaderContent);
            this.FormatHeadersContentOptimallyCore(this.datesForDayOfWeekCustomFormatToolTips, this.datesForDayOfWeekCustomFormatLocations, this.datesForDayOfWeekCustomFormat, this.datesForDayOfWeekCustomFormatCaptions, ASPx.SchedulerMeasurer.SetCustomDayOfWeekHeaderContent);
        },
        ClearDatesForFormats: function () {
            delete this.datesForFormatsWithoutYearLocations;
            delete this.datesForFormatsWithoutYearAndWeekDayLocations;
            delete this.datesForFormatsNewYearLocations;
            delete this.daysForDayOfWeekFormatsLocations;
            delete this.datesForDayNumberFormatLocations;
            delete this.datesForDateCustomFormatLocations;
            delete this.datesForDayOfWeekCustomFormatLocations;
        },
        FormatHeadersContentOptimallyCore: function (headerToolTips, headerLocations, dates, formats, delegate) {
            if (ASPx.IsExists(headerLocations)) {
                var headers = this.GetHeadersByLocation(headerLocations);
                if (headers.length > 0 && ASPx.IsExists(dates))
                    delegate.call(ASPx.SchedulerMeasurer, headers, dates, formats, headerToolTips);
            }
        },
        GetHeadersByLocation: function (locations) {
            var count = locations.length;
            var headers = [];
            for (var i = 0; i < count; i++) {
                var location = locations[i];
                var rowIndex = location[0];
                var cellIndex = location[1];
                var header = this.horzTable.rows[rowIndex].cells[cellIndex];
                headers.push(header);
            }
            return headers;
        },
        ClearNavigationButtons: function () {
            if (ASPx.IsExists(this.navButtons)) {
                var count = this.navButtons.length;
                for (var i = 0; i < count; i++)
                    this.ClearNavigationButton(this.navButtons[i]);
            }
            this.navButtons = [];
        },
        ClearNavigationButton: function (navBtn) {
            var navBtnDiv = this.GetNavButtonElementById(navBtn.divId);
            if (!ASPx.IsExists(navBtnDiv) || !ASPx.IsExists(navBtnDiv.parentNode))
                return;
            //!!!Do not use ASPx.SchedulerGlobals.RecycleNode(navBtnDiv);
            ASPx.RemoveElement(navBtnDiv)
        },
        AlignNavigationButtons: function () {
            var count = this.navButtons.length;
            for (var i = 0; i < count; i++)
                this.AlignNavigationButton(this.navButtons[i]);
        },
        AlignNavigationButton: function (navBtn) {
            var div = this.GetNavButtonElementById(navBtn.divId);
            if (!ASPx.IsExists(div))
                return;

            var leftTopAnchor = this.navButtonAnchors[navBtn.resourceId + "_Left"];
            var rightBottomAnchor = this.navButtonAnchors[navBtn.resourceId + "_Right"];
            if (!ASPx.IsExists(leftTopAnchor) || !ASPx.IsExists(rightBottomAnchor))
                return;
            var leftTopAnchorCell = this.vertTable.rows[leftTopAnchor[0]].cells[leftTopAnchor[1]]; //_aspxGetParentNode(leftTopAnchor);
            var rightBottomAnchorCell = this.vertTable.rows[rightBottomAnchor[0]].cells[rightBottomAnchor[1]];; //_aspxGetParentNode(rightBottomAnchor);

            if (!ASPx.IsExists(leftTopAnchorCell) || !ASPx.IsExists(rightBottomAnchorCell))
                return;

            var dxtop;
            var bottom;
            var parent;
            var verticalContainer = this.GetContainerElementById("verticalContainer");
            var navButtonLayer = null;
            var navButtonParent = this.containerCell;
            if (ASPx.IsExists(verticalContainer)) {
                navButtonLayer = verticalContainer.parentNode.parentNode;
                parent = this.verticalParent;
                dxtop = ASPx.GetAbsoluteY(navButtonLayer) - ASPx.GetAbsoluteY(navButtonParent);
                bottom = dxtop + navButtonLayer.offsetHeight;
            }
            else {
                parent = this.horizontalParent;
                dxtop = parent.CalcRelativeElementTop(leftTopAnchorCell);
                bottom = parent.CalcRelativeElementBottom(rightBottomAnchorCell);
            }
            parent.AppendChildToNavButtonLayer(div, navButtonLayer); //!!!!!

            var left = parent.CalcRelativeElementLeft(leftTopAnchorCell) + 2;
            if (navBtn.anchorType == "Left")
                div.style.left = left + "px";
            else {
                var right = parent.CalcRelativeElementRight(rightBottomAnchorCell) - 2;
                div.style.left = right - div.offsetWidth + "px";
            }
            div.style.top = (dxtop + bottom - div.offsetHeight) / 2 + "px";
            div.style.zIndex = "11";
            ASPx.SetElementDisplay(div, true);
        },
        PrepareVerticalContainerTable: function () {
            //reflow/recalculate dayview scroll container table//dirty hack
            if (ASPx.Browser.IE && ASPx.Browser.Version >= 9)
                this.RefreshVerticalTable();
        },
        RecalcLayout: function () {
            this.PrepareVerticalContainerTable();
            this.HideAppointments();
            if (ASPx.IsExists(document.recalc))
                document.recalc(true);
            this.SyncDayViewHeadersWithColumns();
            this.FormatHeadersContentOptimally();
            this.horizontalViewInfo.parent.Clear();
            if (this.HasVerticalResourceHeaderColumn()) {
                this.LayoutVerticalResourceHeaderColumn();
            }
            var timeLineHeaderCalculator = new ASPx.TimelineHeaderLayoutCalculator(this);
            timeLineHeaderCalculator.CalculateLayout(this.timelineHeaderLevels);

            if (this.cellAutoHeightMode != SchedulerAutoHeightMode.None)
                this.RecalculateCellAutoHeightConstaint();

            //this.horizontalViewInfo.containers
            var schedulerWidth = this.GetMainElement().clientWidth;
            var headerHeight = this.CalculateDayHeaderHeight();
            this.LayoutAppointments();
            if (headerHeight != this.CalculateDayHeaderHeight()) {
                this.ResetCellsAbsolutePosition();
                this.LayoutAppointments();
            }
            if (this.cellAutoHeightMode != SchedulerAutoHeightMode.None && schedulerWidth != this.GetMainElement().clientWidth) {
                timeLineHeaderCalculator = new ASPx.TimelineHeaderLayoutCalculator(this);
                timeLineHeaderCalculator.CalculateLayout(this.timelineHeaderLevels);
                this.LayoutAppointments();
            }
            this.AlignNavigationButtons();
            this.RecalcTimeMarker();
            //todo: refact
            this.RefreshSchedulerLayout();
        },
        CalculateDayHeaderHeight: function () {
            if (!this.datesForFormatsWithoutYearLocations)
                return 0;
            var headers = this.GetHeadersByLocation(this.datesForFormatsWithoutYearLocations);
            if (headers && headers.length <= 0)
                return 0;
            return headers[0].clientHeight;
        },
        RecalculateCellAutoHeightConstaint: function () {
            //come from options
            var minCellHeight = this.cellAutoHeightConstrant[0];
            var maxCellHeight = this.cellAutoHeightConstrant[1];
            var fitToContent = this.cellAutoHeightMode == SchedulerAutoHeightMode.FitToContent;

            if (minCellHeight < this.defaultAutoHeightCellMinimum)
                minCellHeight = this.defaultAutoHeightCellMinimum
            if (this.cellAutoHeightMode == SchedulerAutoHeightMode.LimitHeight) {
                if (maxCellHeight <= 0)
                    fitToContent = true;
                else if (maxCellHeight < this.defaultAutoHeightCellMinimum)
                    maxCellHeight = this.defaultAutoHeightCellMinimum;
            }

            var containerCount = this.horizontalViewInfo.cellContainers.length;
            for (var i = 0; i < containerCount; i++) {
                var container = this.horizontalViewInfo.cellContainers[i];
                container.cellConstraint = [];
                var headerCell = this.horizontalViewInfo.GetMiddleCompressedCellsHeader(i); ////
                var headerHeight = (headerCell) ? headerCell.clientHeight : 0;

                for (var j = 0; j < container.cellCount; j++) {
                    var minSize = (container.GetCellLocation(j).isCompressed) ? (minCellHeight - headerHeight) / 2 : minCellHeight
                    if (minSize < 0)
                        minSize = minCellHeight;
                    var maxSize = 0;
                    if (!fitToContent)
                        maxSize = (container.GetCellLocation(j).isCompressed) ? (maxCellHeight - headerHeight) / 2 : maxCellHeight

                    var constraint = { minHeight: minSize, maxHeight: maxSize };
                    container.cellConstraint.push(constraint);
                }
            }
        },
        RefreshSchedulerLayout: function () {
            var oldPadding = this.mainDiv.style.padding;
            this.mainDiv.style.padding = "1px";
            this.mainDiv.style.padding = oldPadding;
            if (ASPx.Browser.Firefox) {//bug in firefox4 with top header border
                var oldBorderCollapse = this.mainDiv.style.borderCollapse;
                this.mainDiv.style.borderCollapse = (oldBorderCollapse == "separate") ? "collapsed" : "separate";
                var some = this.mainDiv.offsetHeight;
                this.mainDiv.style.borderCollapse = oldBorderCollapse;
            }
        },
        RefreshVerticalTable: function () {
            if (!ASPx.IsExists(this.vertTable))
                return;
            var viewType = this.GetActiveViewType();
            if (viewType == ASPxSchedulerViewType.Day || viewType == ASPxSchedulerViewType.WorkWeek || viewType == ASPxSchedulerViewType.FullWeek) {
                var parent = this.vertTable.parentNode;
                parent.removeChild(this.vertTable);
                parent.appendChild(this.vertTable);
            }
        },
        HasVerticalResourceHeaderColumn: function () {
            var activeView = this.GetActiveViewType();
            var activeGroup = this.GetActualGroupType();
            if (activeView == ASPxSchedulerViewType.Timeline && activeGroup != ASPxSchedulerGroupType.None)
                return true;
            if ((activeView == ASPxSchedulerViewType.Week || activeView == ASPxSchedulerViewType.Month) && activeGroup == ASPxSchedulerGroupType.Date)
                return true;
            return false;
        },
        ResetVerticalResourceHeadersCache: function () {
            var table = this.GetContainerElementById("content");
            if (table == null)
                return;
            var cells = table.tBodies[0].rows[0].cells;
            var leftCell = cells[0];
            if (leftCell == null)
                return;
            leftCell.correctedWidth = null;
        },
        LayoutVerticalResourceHeaderColumn: function () {
            var table = this.GetContainerElementById("content");
            var cells = table.tBodies[0].rows[0].cells;
            var leftCell = cells[0];
            var rightCell = cells[cells.length - 1];
            if (table == null || leftCell == null || rightCell == null)
                return;
            if (ASPx.IsExists(leftCell.correctedWidth))
                return;
            var userDefinedLeftCellSize = this.GetVerticalResourceHeaderColumnWidthAssignedByUser();
            if (userDefinedLeftCellSize != "") {
                leftCell.style.width = userDefinedLeftCellSize;
                leftCell.correctedWidth = leftCell.style.width;
                return;
            }
            var saveRightCellStyleWidth = rightCell.style.width;
            rightCell.style.width = "100%";
            ASPx.SchedulerGlobals.ChangeTableLayout(table, "auto");
            leftCell.correctedWidth = Math.max(leftCell.offsetWidth, leftCell.clientWidth);
            rightCell.style.width = saveRightCellStyleWidth;
            ASPx.SchedulerGlobals.ChangeTableLayout(table, "fixed");
            ASPx.SchedulerGlobals.SetTableCellOffsetWidth(leftCell, leftCell.correctedWidth);
        },
        GetVerticalResourceHeaderColumnWidthAssignedByUser: function () {
            if (this.MeasureVRHLocation == null)
                return "";
            var cell = ASPx.SchedulerGlobals.GetItemByLocation(this.horzTable, this.MeasureVRHLocation);
            return cell.style["width"];
        },
        LayoutAppointments: function () {
            var horizontalCalculator = this.CreateHorizontalAppointmentsCalculator();
            horizontalCalculator.CalculateLayout(this.horizontalViewInfo.appointmentViewInfos);
            this.horizontalViewInfo.parent.AfterCalculateAppointments(horizontalCalculator);
            var verticalCalculator = this.CreateVerticalAppointmentsCalculator();
            verticalCalculator.CalculateLayout(this.verticalViewInfo.appointmentViewInfos);
            this.verticalViewInfo.parent.AfterCalculateAppointments(verticalCalculator);
            if (this.GetIsDayBasedView()) {
                this.verticalViewInfo.ShowMoreButton();
            }
        },

        HideAppointments: function () {
            //this.horizontalViewInfo.HideAppointments();
            this.verticalViewInfo.HideAppointments();
        },
        CreateVerticalAppointmentsCalculator: function () {
            return new ASPx.VerticalAppointmentLayoutCalculator(this.verticalViewInfo, this.verticalParent, this.privateDisableSnapToCells);
        },
        CreateHorizontalAppointmentsCalculator: function () {
            var activeView = this.GetActiveViewType();
            if (activeView == ASPxSchedulerViewType.Day || activeView == ASPxSchedulerViewType.WorkWeek || activeView == ASPxSchedulerViewType.FullWeek)
                return new ASPx.HorizontalAppointmentLayoutCalculatorInfinityHeight(this.horizontalViewInfo, this.privateDisableSnapToCells);
            else {
                if (this.cellAutoHeightMode == SchedulerAutoHeightMode.None)
                    return new ASPx.HorizontalAppointmentLayoutCalculator(this.horizontalViewInfo, this.privateDisableSnapToCells);
                return new ASPx.CellsAutoHeightClientHorizontalAppointmentLayoutCalculator(this.horizontalViewInfo, this.privateDisableSnapToCells);
            }
        },
        SyncDayViewHeadersWithColumns: function () {
            if (this.syncColumnCount <= 0) {
                this.HideMasterRow();
                return;
            }
            var masterRow = this.vertTable.rows[this.syncMasterRowIndex];
            var slaveRow = this.horzTable.rows[this.syncSlaveRowIndex];
            masterRow.style.display = "";
            slaveRow.style.display = "";
            this.SetColorForSlaveRow(this.syncColumnCount, slaveRow);

            var scrollbarWidth = this.CalcVerticalScrollbarWidth();
            if (scrollbarWidth >= 0 && slaveRow.cells.length > this.syncColumnCount) { //B140685, B187910
                var cell = slaveRow.cells[this.syncColumnCount];
                var scrollCellWidth = scrollbarWidth;
                if (ASPx.Browser.WebKitFamily)
                    scrollCellWidth += 1;
                cell.style.width = scrollCellWidth + "px";
                ASPx.SchedulerGlobals.RefreshTableCell(slaveRow.cells[this.syncColumnCount]);
            }
            this.HideMasterRow();
        },
        SetColorForSlaveRow: function (syncColumnCount, slaveRow) {
            var style = ASPx.GetCurrentStyle(this.GetMainElement());
            if (style == null)//B145784 
                return;
            var backgroundColor = ASPx.Attr.GetAttribute(style, "background-color");
            if (!backgroundColor)
                backgroundColor = style.backgroundColor;
            for (var i = syncColumnCount - 1; i >= 0; i--) {
                slaveRow.cells[i].style.borderColor = backgroundColor;
                slaveRow.cells[i].style.backgroundColor = backgroundColor;
                ASPx.SchedulerGlobals.RefreshTableCell(slaveRow.cells[i]);
            }
        },
        HideMasterRow: function () {
            if (!this.syncMasterRowIndex)
                return;
            if (this.syncMasterRowIndex <= 0)
                return;
            var masterRow = this.vertTable.rows[this.syncMasterRowIndex];
            masterRow.style.display = "none";
        },
        SetSyncCells: function (syncColumnCount, syncMasterRowIndex, syncSlaveRowIndex) {
            this.syncColumnCount = syncColumnCount;
            this.syncMasterRowIndex = syncMasterRowIndex;
            this.syncSlaveRowIndex = syncSlaveRowIndex;
        },
        CalcVerticalScrollbarWidth: function () {
            var vsc = this.GetContainerElementById("verticalScrollContainer");
            if (ASPx.IsExists(vsc)) {
                var clientWidth = vsc.clientWidth;
                if (!ASPx.IsExists(clientWidth))
                    return 0;
                else
                    return vsc.offsetWidth - clientWidth;
            }
            else
                return 0;
        },
        CalculateLayout: function () {
            this.RecalcLayout();
            this.EnableResize();
            //this.SubscribeWindowResizeEvent();
        },
        GetBlockElementId: function (blockId, innerId) {
            return this.name + "_" + blockId + "_" + innerId;
        },
        GetElementById: function (id) {
            return ASPx.GetElementById(this.name + "_" + id);
        },
        GetContainerElementById: function (id) {
            return ASPx.GetElementById(this.name + "_containerBlock_" + id);
        },
        GetNavButtonElementById: function (id) {
            var div = this.navButtonsCache[id];
            if (!ASPx.IsExists(div)) {
                div = ASPx.GetElementById(this.name + "_navButtonsBlock_" + id);
                this.navButtonsCache[id] = div;
            }
            return div;
        },
        GetAppointmentBlockElementById: function (id) {
            return ASPx.GetElementById(this.name + "_aptsBlock_" + id);
        },
        GetMainTable: function () {
            return this.innerContentElement;
        },
        GetCellInterval: function (cell) {
            var interval = cell.interval;
            if (ASPx.IsExists(interval))
                return interval;
            else {
                this.InitializeCell(cell);
                return cell.interval;
            }
        },
        GetCellStartTime: function (cell) {
            return this.GetCellInterval(cell).GetStart();
        },
        GetCellDuration: function (cell) {
            return this.GetCellInterval(cell).GetDuration();
        },
        GetCellEndTime: function (cell) {
            return this.GetCellInterval(cell).GetEnd();
        },
        GetCellResource: function (cell) {
            var resource = cell.resource;
            if (ASPx.IsExists(resource))
                return resource;
            else {
                this.InitializeCell(cell);
                return cell.resource;
            }
        },
        CancelFormChangesAndClose: function (visibility, callbackName) {
            if (visibility == ASPx.SchedulerFormVisibility.FillControlArea) {
                this.RaiseCallback(callbackName);
            }
            else
                this.ClosePopupForm();
        },
        ClosePopupForm: function () {
            var statusInfoManager = this.statusInfoManager;
            var isInplaceEditorOpen = this.activeFormType == ASPx.SchedulerFormType.AppointmentInplace
            if (statusInfoManager && !isInplaceEditorOpen)
                statusInfoManager.Clear();
            this.HideCurrentPopupContainer();
            this.SaveActiveFormTypeState(ASPx.SchedulerFormType.None);
            this.EnableReminderTimer();
        },
        SaveActiveFormTypeState: function (formType) {
            this.activeFormType = formType;
            this.stateObject.formType = formType;
        },
        // TODO: test it
        SaveCurrentPopupContainer: function (popupId) {
            this.HideCurrentPopupContainer();
            this.currentPopupContainer = ASPx.GetControlCollection().Get(this.name + "_formBlock_" + popupId);
        },
        HideCurrentPopupContainer: function () {
            if (ASPx.IsExists(this.currentPopupContainer)) {
                window.setTimeout(ASPx.CreateDelegate(this.HideCurrentPopupContainerCore, this), 0);
            }
        },
        HideCurrentPopupContainerCore: function () {
            if (!ASPx.IsExists(this.currentPopupContainer))
                return;
            this.currentPopupContainer.CloseUp.ClearHandlers();
            this.currentPopupContainer.Hide();
            var mainElement = this.currentPopupContainer.GetWindowElement(-1);

            this.RemoveFormControls(mainElement);

            ASPx.Attr.RemoveAttribute(mainElement, "modalElement");
            mainElement.modalElement = null;
            var modalElement = this.currentPopupContainer.FindWindowModalElement(-1);
            if (modalElement != null && ASPx.IsExists(modalElement.parentNode)) {
                ASPx.SchedulerGlobals.RecycleNode(modalElement);
            }
            if (ASPx.IsExists(mainElement.parentNode)) {
                ASPx.SchedulerGlobals.RecycleNode(mainElement);
            }
            this.RemoveNonpermanentAppointments();
            this.currentPopupContainer = null;
        },
        RemoveFormControls: function (mainElement) {
            var controlsToRemove = [];
            ASPx.GetControlCollection().ProcessControlsInContainer(mainElement, function (control) {
                controlsToRemove.push(control);
            });
            for (var control in controlsToRemove) {
                var controlToDispose = controlsToRemove[control];
                if (controlToDispose.OnDispose)
                    controlToDispose.OnDispose();
                ASPx.GetControlCollection().Remove(controlToDispose);
            }
        },
        RemoveNonpermanentAppointments: function () {
            for (var aptId in this.nonpermanentAppointments)
                this.RemoveAppointment(aptId);
        },
        RemoveAppointment: function (aptId) {
            this.RemoveViewInfosByAppointmentId(aptId);
            this.nonpermanentAppointments[aptId] = null;
            this.appointments[aptId] = null;
        },
        ShowFormPopupWindow: function (popupId) {
            if (ASPx.IsExists(this.currentPopupContainer)) {
                this.DisableReminderTimer();
                var width = ASPx.GetDocumentClientWidth();
                var height = ASPx.GetDocumentClientHeight();
                var pcwElement = this.currentPopupContainer.GetWindowElement(-1);
                this.currentPopupContainer.SetWindowDisplay(-1, true);
                var popupWidth = pcwElement.offsetWidth;
                var popupHeight = pcwElement.offsetHeight;
                this.currentPopupContainer.SetWindowDisplay(-1, false);
                var xOffset = ASPx.GetDocumentScrollLeft();
                var yOffset = ASPx.GetDocumentScrollTop();

                this.currentPopupContainer.ShowAtPos(xOffset + ((width - popupWidth) >> 1), yOffset + ((height - popupHeight) >> 1));
                if (popupWidth == 0 || popupHeight == 0) {
                    var popupWidth = pcwElement.offsetWidth;
                    var popupHeight = pcwElement.offsetHeight;
                    this.currentPopupContainer.ShowAtPos(xOffset + ((width - popupWidth) >> 1), yOffset + ((height - popupHeight) >> 1));
                }
            }
        },
        ShowFormPopupWindowDeferred: function (popupId) {
            this.formPopupIdDeferred = popupId;
            this.formPopupIdDeferredFormType = this.activeFormType;
            this.SaveActiveFormTypeState(ASPx.SchedulerFormType.None);
        },
        ProcessShowFormPopupWindowDeferred: function () {
            if (!this.formPopupIdDeferred)
                return;
            this.ShowFormPopupWindow(this.formPopupIdDeferred);
            this.formPopupIdDeferred = null;
            this.SaveActiveFormTypeState(this.formPopupIdDeferredFormType);
        },
        ShowInplacePopupWindow: function (inplaceEditorPopupId, aptId) {
            if (ASPx.IsExists(this.currentPopupContainer)) {
                this.DisableReminderTimer();
                var aptViewInfos;
                if (ASPx.IsExists(this.verticalViewInfo)) {
                    aptViewInfos = this.verticalViewInfo.FindViewInfosByAppointmentId(aptId);
                }
                if (!ASPx.IsExists(aptViewInfos) || (aptViewInfos.length <= 0)) {
                    if (ASPx.IsExists(this.horizontalViewInfo)) {
                        aptViewInfos = this.horizontalViewInfo.FindViewInfosByAppointmentId(aptId);
                    }
                }
                var count = aptViewInfos.length;
                if (!ASPx.IsExists(aptViewInfos) || (count <= 0))
                    return;
                var firstVisibleAptViewInfo = null;
                for (var i = 0; i < count; i++) {
                    var aptViewInfo = aptViewInfos[i];
                    if (ASPx.GetElementDisplay(aptViewInfo.contentDiv)) {
                        firstVisibleAptViewInfo = aptViewInfo;
                        break;
                    }
                }
                if (!firstVisibleAptViewInfo)
                    return;
                var contentDiv = firstVisibleAptViewInfo.contentDiv;
                if (!ASPx.IsExists(contentDiv))
                    return;
                this.currentPopupContainer.SetSize(Math.max(200, contentDiv.offsetWidth), 0); //contentDiv.offsetHeight
                var div = this.currentPopupContainer.GetWindowElement(-1);
                if (!firstVisibleAptViewInfo.IsHorizontal()) {
                    if (!ASPx.IsExists(contentDiv.parentNode))
                        return;
                    contentDiv.parentNode.appendChild(div);
                    this.currentPopupContainer.ShowAtPos(0, 0);
                    this.ShowInplacePopupWindowCore(div, firstVisibleAptViewInfo);
                }
                else {
                    this.currentPopupContainer.ShowAtPos(ASPx.GetAbsoluteX(contentDiv), ASPx.GetAbsoluteY(contentDiv));
                }
            }
        },
        ShowInplacePopupWindowCore: function (inplaceEditorElement, appointmentViewInfo) {
            var contentDiv = appointmentViewInfo.contentDiv;
            var divLeft = contentDiv.offsetLeft;
            var divTop = contentDiv.offsetTop;
            if (appointmentViewInfo.IsHorizontal()) {
                this.currentPopupContainer.OnDrag(-1, divLeft, divTop);
                return;
            }
            var scrollContainer = this.GetContainerElementById("verticalScrollContainer");
            if (!ASPx.IsExists(scrollContainer) || scrollContainer.style.height == "") {
                this.currentPopupContainer.OnDrag(-1, divLeft, divTop);
                return;
            }
            if (inplaceEditorElement.offsetWidth + divLeft > scrollContainer.clientWidth)
                divLeft = scrollContainer.clientWidth - inplaceEditorElement.offsetWidth;
            if (inplaceEditorElement.offsetHeight + divTop > scrollContainer.scrollHeight)
                divTop = scrollContainer.scrollHeight - inplaceEditorElement.offsetHeight;
            this.currentPopupContainer.OnDrag(-1, divLeft, divTop);

            if (inplaceEditorElement.offsetTop < scrollContainer.scrollTop || inplaceEditorElement.offsetHeight > scrollContainer.offsetHeight) {
                scrollContainer.scrollTop = Math.max(inplaceEditorElement.offsetTop, 0);
                this.EnsureInplaceEditorPositionVisible(inplaceEditorElement);
                return;
            }
            var bottom = inplaceEditorElement.offsetTop + inplaceEditorElement.offsetHeight;
            var scrollBottom = scrollContainer.scrollTop + scrollContainer.offsetHeight;
            if (bottom > scrollBottom) {
                var newScrollTop = scrollContainer.scrollTop + bottom - scrollBottom;
                scrollContainer.scrollTop = Math.max(newScrollTop, 0);
                this.EnsureInplaceEditorPositionVisible(inplaceEditorElement);
                return;
            }
            this.EnsureInplaceEditorPositionVisible(inplaceEditorElement);
        },
        EnsureInplaceEditorPositionVisible: function (inplaceEditorElement) {
            var htmlParent = inplaceEditorElement.offsetParent;
            while (ASPx.IsExists(htmlParent) && htmlParent.tagName != "HTML")
                htmlParent = htmlParent.parentNode;
            if (!ASPx.IsExists(htmlParent))
                return;
            var posY = ASPx.GetAbsolutePositionY(inplaceEditorElement);
            if (htmlParent.scrollTop > posY)
                inplaceEditorElement.scrollIntoView(true);
            else
                if (htmlParent.scrollTop + htmlParent.offsetHeight < posY + inplaceEditorElement.offsetHeight)
                    inplaceEditorElement.scrollIntoView(false);
        },
        // IParent interface impl end
        ShowCellSelection: function () {
            if (ASPx.IsExists(this.selection)) {
                var dayBasedView = this.GetIsDayBasedView();
                var selectionVisible = this.appointmentSelection.selectedAppointmentIds.length == 0;
                this.horizontalViewInfo.ShowCellSelection(this.selection.interval, this.selection.resource, !dayBasedView, selectionVisible);
                this.verticalViewInfo.ShowCellSelection(this.selection.interval, this.selection.resource, true, selectionVisible);
            }
        },
        GetHightlightElementFromCell: function (cell) {
            if (!ASPx.IsExists(this.selection))
                return null;
            var element = this.horizontalViewInfo.GetHightlightElementFromCell(cell);
            if (element)
                return element;
            return this.verticalViewInfo.GetHightlightElementFromCell(cell);
        },
        OnAppointmentSelectionChanged: function (selectedAppointmentIds) {
            this.SaveAppointmentSelectionState();
            this.ApplyAppointmentSelectionToCellSelection(selectedAppointmentIds);
            this.ShowCellSelection();

            if (ASPx.IsExists(this.RaiseAppointmentsSelectionChanged))
                this.RaiseAppointmentsSelectionChanged(selectedAppointmentIds);
        },
        ApplyAppointmentSelectionToCellSelection: function (selectedAppointmentIds) {
            if (selectedAppointmentIds.length < 1)
                return;
            var lastAppointmentIndx = selectedAppointmentIds.length - 1;
            var lastSelectedAppointment = this.GetAppointment(selectedAppointmentIds[lastAppointmentIndx]);
            var start = lastSelectedAppointment.GetStart();
            var duration = lastSelectedAppointment.GetDuration();
            var resourceId = lastSelectedAppointment.GetResource(0);
            this.SetSelection(start, duration, resourceId, start, duration);
        },
        IsOperationSelectionActive: function () {
            return ASPx.schedulerSelectionHelper != null;
        },
        SetFormsInitState: function (activeFormType, aptFormVisibility, gotoDateFormVisibility, recurrentAppointmentDeleteFormVisibility) {
            this.activeFormType = activeFormType;
            this.aptFormVisibility = aptFormVisibility;
            this.gotoDateFormVisibility = gotoDateFormVisibility;
            this.recurrentAppointmentDeleteFormVisibility = recurrentAppointmentDeleteFormVisibility;
        },
        SetCanShowDayTimeMarker: function (canShowDayTimeMarker) {//B183021
            this.canShowDayTimeMarker = canShowDayTimeMarker;
        },
        //TODO: test it!
        SetSelection: function (start, duration, resourceId, firstSelectionStart, firstSelectionDuration) {
            var firstSelectedInterval = new ASPxClientTimeInterval(firstSelectionStart, firstSelectionDuration);
            var selectedInterval = new ASPxClientTimeInterval(start, duration);
            if (!ASPx.IsExists(this.selection) ||
                !this.selection.interval.Equals(selectedInterval) ||
                !this.selection.firstSelectedInterval.Equals(firstSelectedInterval) ||
                this.selection.resource != resourceId) {
                this.SetSelectionCore(new ASPx.SchedulerSelection(selectedInterval, resourceId, firstSelectedInterval));
                this.OnSelectionChanged(null);
            }
        },
        SetSelectionCore: function (newSelection) {
            this.selection = newSelection;
            this.SaveSelectionState();
            if (!this.isInsideBeginInit && this.isInitialized)
                this.ShowCellSelection();
        },
        SetSelectionInterval: function (newInterval) {
            this.selection.interval = newInterval;
            this.SaveSelectionState();
            this.ShowCellSelection();
        },
        SetSelectedAppointmentIds: function (appointmentIds) {
            this.appointmentSelection.BeginUpdate();

            var count = appointmentIds.length;
            for (var i = 0; i < count; i++)
                this.appointmentSelection.AddAppointmentToSelection(appointmentIds[i]);

            this.appointmentSelection.EndUpdate();
        },
        OnSelectionChanged: function (e) {
            if (this.isInsideBeginInit)
                this.deferredRaiseSelectionChanged = true;
            else
                this.RaiseSelectionChanged();
        },
        //TODO: test it!
        GetResourceString: function (resource) {
            return resource != null ? resource : this._constEmptyResource;
        },
        GetStateElement: function (name) {
            return this.GetChildElement("stateBlock_" + name);
        },
        //TODO: test it!
        SaveSelectionState: function () {
            //var resourceString = (this.selection.resource != null) ? this.selection.resource : "";
            var resourceString = this.GetResourceString(this.selection.resource);
            var result = this.selection.interval.ToString() + "," +
                this.selection.firstSelectedInterval.ToString() + "," +
                resourceString;
            this.stateObject.selection = result;
            if (this.schedulerIsInitialized)
                this.RaiseSelectionChanging();
        },
        SaveAppointmentSelectionState: function () {
            var selectedAppointmentsIds = this.appointmentSelection.selectedAppointmentIds;
            var count = selectedAppointmentsIds.length;
            var result = "";
            for (var i = 0; i < count; i++)
                result += selectedAppointmentsIds[i] + ",";
            this.stateObject.appointmentSelection = result;
        },
        AddNavigationButton: function (divId, resourceId, anchorType) {
            var btn = new ASPx.NavigationButton(divId, resourceId, anchorType);
            this.navButtons.push(btn);
        },
        AddAppointmentPattern: function (pattern) {
            this.appointments[pattern.appointmentId] = pattern;
            for (var appointmentId in this.appointments) {
                var appointment = this.GetAppointment(appointmentId); //?
                var aptType = appointment.appointmentType;
                var isRecurring = aptType != ASPxAppointmentType.Normal;
                if (!isRecurring || aptType == ASPxAppointmentType.Pattern)
                    continue;
                var splittedId = appointmentId.split('_');
                var patternId = splittedId[0];
                var recurrenceIndex = splittedId[1];
                if (splittedId.length == 2 && patternId == pattern.appointmentId) {
                    appointment.SetRecurrencePattern(pattern);
                    appointment.RecurrenceIndex = recurrenceIndex;
                }
            }
        },
        AddAppointment: function (aptId, start, duration, resources, flagStr, appointmentType, labelId, statusId, isNonpermanentAppointment, propertyDictionary) {
            this.appointments[aptId] = this.CreateAppointmentFromArgs(aptId, start, duration, resources, flagStr, appointmentType, labelId, statusId, propertyDictionary);
            if (isNonpermanentAppointment == 1) {
                this.nonpermanentAppointments[aptId] = this.appointments[aptId];
            }
        },
        CreateAppointmentFromArgs: function (aptId, start, duration, resources, flagStr, appointmentType, labelId, statusId, propertyDictionary) {
            var flags = new ASPxClientAppointmentFlags();
            flags.allowDelete = flagStr.charAt(0) == "1";
            flags.allowEdit = flagStr.charAt(1) == "1";
            flags.allowResize = flagStr.charAt(2) == "1";
            flags.allowCopy = flagStr.charAt(3) == "1";
            flags.allowDrag = flagStr.charAt(4) == "1";
            flags.allowDragBetweenResources = flagStr.charAt(5) == "1";
            flags.allowInplaceEditor = flagStr.charAt(6) == "1";
            flags.allowConflicts = flagStr.charAt(7) == "1";
            //TODO remove from flags, move to apt client apt
            var interval = new ASPxClientTimeInterval(start, duration);
            var apt = new ASPxClientAppointment(interval, resources, flags, aptId, appointmentType, statusId, labelId);
            if (propertyDictionary)
                this.propertyController.ApplyProperties(apt, propertyDictionary);
            return apt;
        },
        InitializeAppointmentDivCache: function () {
            if (this.changedBlocks && !this.changedBlocks.AppointmentsBlockChanged)
                return;
            if (ASPx.IsExists(this.appointmentDivCache)) {
                for (var divId in this.appointmentDivCache) {
                    var div = this.appointmentDivCache[divId];
                    ASPx.RemoveElement(div);
                }
            }
            this.appointmentDivCache = {};
            var aptsBlock = this.GetAppointmentBlockElementById("innerContent");
            if (ASPx.IsExists(aptsBlock)) {
                var children = aptsBlock.childNodes;
                var count = children.length;
                for (var i = 0; i < count; i++) {
                    var child = children[i];
                    if (ASPx.IsExists(child.tagName) && child.tagName.toUpperCase() == "DIV") {
                        this.appointmentDivCache[child.id] = child;
                    }
                }
            }
        },
        GetAppointmentDivById: function (divId) {
            return this.appointmentDivCache[this.name + "_aptsBlock_" + divId];
        },
        //TODO test it!
        GetAppointmentFlags: function (aptId) {
            return this.GetAppointment(aptId).flags;
        },
        GetAppointment: function (aptId) {
            return this.appointments[aptId];
        },
        //TODO test it!
        GetAppointmentInterval: function (aptId) {
            return this.appointments[aptId].interval;
        },
        //TODO test it!
        GetAppointmentResources: function (aptId) {
            return this.appointments[aptId].resources;
        },
        AddAnchor: function (rowIndex, cellIndex, anchorId) {
            this.navButtonAnchors[anchorId] = [rowIndex, cellIndex];
        },
        AddHorizontalAppointment: function (firstCellIndex, lastCellIndex, startTime, duration, topRelativeIndent, bottomRelativeIndent, divId, appointmentId, statusBackDivId, statusForeDivId, statusStartOffset, statusEndOffset, hasLeftBorder, hasRightBorder) {
            var result1 = this.cellIdRegExp.exec(firstCellIndex);
            if (!ASPx.IsExists(result1) || result1.length == 0)
                return;
            var containerIndex = parseInt(result1[3]);
            var cellIndex1 = parseInt(result1[4]);

            var result2 = this.cellIdRegExp.exec(lastCellIndex);
            if (!ASPx.IsExists(result2) || result2.length == 0)
                return;
            var cellIndex2 = parseInt(result2[4]);
            var apt = new ASPx.HorizontalAppointmentViewInfo(this.horizontalViewInfo, containerIndex, cellIndex1, cellIndex2, startTime, duration, topRelativeIndent, bottomRelativeIndent, divId, appointmentId, hasLeftBorder, hasRightBorder, statusBackDivId, statusForeDivId, statusStartOffset, statusEndOffset);
            this.horizontalViewInfo.AddViewInfo(apt);
        },
        AddVerticalAppointment: function (firstCellIndex, lastCellIndex, startTime, duration, topRelativeIndent, bottomRelativeIndent, divId, appointmentId, statusBackDivId, statusForeDivId, statusStartOffset, statusEndOffset, startHorizontalIndex, endHorizontalIndex, maxIndexInGroup, hasTopBorder, hasBottomBorder) {
            var result1 = this.cellIdRegExp.exec(firstCellIndex);
            if (!ASPx.IsExists(result1) || result1.length == 0)
                return;
            var containerIndex = parseInt(result1[3]);
            var cellIndex1 = parseInt(result1[4]);

            var result2 = this.cellIdRegExp.exec(lastCellIndex);
            if (!ASPx.IsExists(result2) || result2.length == 0)
                return;
            var cellIndex2 = parseInt(result2[4]);

            var viewInfo = new ASPx.VerticalAppointmentViewInfo(this.verticalViewInfo, containerIndex, cellIndex1, cellIndex2, startTime, duration, divId, startHorizontalIndex, endHorizontalIndex + 1, maxIndexInGroup + 1, topRelativeIndent, bottomRelativeIndent, appointmentId, hasTopBorder, hasBottomBorder, statusBackDivId, statusForeDivId, statusStartOffset, statusEndOffset);
            //viewInfo.startCellId = firstCellIndex;
            //viewInfo.endCellId = lastCellIndex;
            this.verticalViewInfo.AddViewInfo(viewInfo);
        },
        AddHorizontalContainer: function (containerIndex, cellCount, containerStartTime, cellsDurations, resource, cellsLocations, middleCompressedCellsHeaderLocation) {
            //cellsDurations --- array. cellsDurations[2*i] contains count of cells, which have duration cellsDurations[2*i + 1].
            this.horizontalViewInfo.AddCellContainer(containerIndex, cellCount, containerStartTime, cellsDurations, resource, cellsLocations, middleCompressedCellsHeaderLocation);
        },
        AddVerticalContainer: function (containerIndex, cellCount, containerStartTime, cellsDurations, resource, cellsLocations) {
            //cellsDurations --- array. cellsDurations[2*i] contains count of cells, which have duration cellsDurations[2*i + 1].
            this.verticalViewInfo.AddCellContainer(containerIndex, cellCount, containerStartTime, cellsDurations, resource, cellsLocations);
        },
        AddTimelineHeader: function (levelIndex, cellLocation, offset, baseCellLocation) { // added by eugen 
            if (this.timelineHeaderLevels.length == levelIndex)
                this.timelineHeaderLevels[levelIndex] = new ASPx.TimelineHeaderLevelViewInfo();
            this.timelineHeaderLevels[levelIndex].Add(cellLocation, offset, baseCellLocation);
        },
        RaiseShowInplaceEditorCallback: function () {
            this.RaiseCallback("INPLACESHOW|");
        },
        MainDivMouseClick: function (evt) {
            this.mouseHandler.MainDivMouseClick(evt);
        },
        MainDivMouseDoubleClick: function (evt) {
            this.mouseHandler.MainDivMouseDoubleClick(evt);
        },
        MainDivMouseUp: function (e) {
            this.mouseHandler.MainDivMouseUp(e);
            this.RaiseMouseUp();
        },
        MainDivMouseDown: function (e) {
            this.mouseHandler.MainDivMouseDown(e);
        },
        IsEventSourceInsideFormContainer: function (evt) {
            if (this.currentPopupContainer == null)
                return false;
            var eventSource = ASPx.Evt.GetEventSource(evt);
            var formContainer = ASPx.GetParent(eventSource, ASPx.CreateDelegate(this.IsElementInsideFormContainer, this));
            if (formContainer)
                return true;
            return false;
        },
        IsElementInsideFormContainer: function (element) {
            var formBlockId = this.GetBlockElementId("formBlock", "innerContent");
            if (element.id == formBlockId)
                return true;
            return false;
        },
        CanSelect: function (e) {
            var eventSource = ASPx.Evt.GetEventSource(e);
            if (!ASPx.IsExists(eventSource))
                return false;
            var id = eventSource.id;
            return id.indexOf("containerBlock_DXCnt") >= 0 || id.indexOf("commonControlsBlock_selectionDiv") >= 0 || id.indexOf("containerBlock_scrollableContainer") >= 0;
        },
        OnActivateInplaceEditor: function (e) {
            if (this.appointmentSelection.selectedAppointmentIds.length != 1)
                return;
            var hitTestResult = this.CalcHitTest(e);
            var aptDiv = hitTestResult.appointmentDiv;
            if (ASPx.IsExists(aptDiv)) {
                var aptFlags = this.GetAppointmentFlags(aptDiv.appointmentId);
                if (aptFlags != null && aptFlags.allowInplaceEditor)
                    this.RaiseShowInplaceEditorCallback();
            }
        },
        LookupCellByMousePosition: function (viewInfo, x, y, containerIndex, firstCellIndex, lastCellIndex) {
            var relX = this.CalcMousePositionX(x, viewInfo);
            var relY = this.CalcMousePositionY(y, viewInfo);
            return viewInfo.FindCellByPosition(containerIndex, firstCellIndex, lastCellIndex, relX, relY);
        },
        LookupCellByMousePositionSlow: function (viewInfo, x, y) {
            var x = this.CalcMousePositionX(x, viewInfo);
            var y = this.CalcMousePositionY(y, viewInfo);
            return viewInfo.FindCellByPositionSlow(x, y);
        },
        CalcMousePositionX: function (x, viewInfo) {
            return x - ASPx.GetAbsoluteX(viewInfo.parent.innerParentElement);
        },
        CalcMousePositionY: function (y, viewInfo) {
            return y - ASPx.GetAbsoluteY(viewInfo.parent.innerParentElement);
        },
        GetCellContainer: function (cell) {
            var container = cell.container;
            if (ASPx.IsExists(container))
                return container;
            this.InitializeCell(cell);
            return cell.container;
        },
        CalcHitTestFromPosition: function (x, y) {
            var cell = this.LookupCellByMousePositionSlow(this.horizontalViewInfo, x, y);
            if (cell == null)
                cell = this.LookupCellByMousePositionSlow(this.verticalViewInfo, x, y);
            var aptViewInfo = this.verticalViewInfo.FindAppointmentViewInfoByPosition(x, y);
            if (aptViewInfo == null)
                aptViewInfo = this.horizontalViewInfo.FindAppointmentViewInfoByPosition(x, y);
            var selectionDiv = null;
            var resizeDiv = null;
            var appointmentDiv = null;
            if (aptViewInfo != null)
                appointmentDiv = aptViewInfo.contentDiv;
            return new SchedulerHitTestResult(cell, selectionDiv, appointmentDiv, resizeDiv);
        },
        CalcHitTest: function (e) {
            var eventSource = ASPx.Evt.GetEventSource(e);
            if (!ASPx.IsExists(eventSource))
                return new SchedulerHitTestResult(null, null, null);
            var appointmentElement = ASPx.GetParentByPartialId(eventSource, this._constDXAppointment());
            var appointmentDiv = appointmentElement;
            if (ASPx.IsExists(appointmentElement) && ASPx.IsExists(appointmentElement.appointmentDiv))
                appointmentDiv = appointmentElement.appointmentDiv;
            if (!ASPx.IsExists(appointmentDiv)) {
                var aptAdornerDiv = ASPx.GetParentByPartialId(eventSource, this._constDXAppointmentAdorner());
                if (ASPx.IsExists(aptAdornerDiv))
                    appointmentDiv = aptAdornerDiv.appointmentDiv;
            }
            else {
                if (!ASPx.IsExists(appointmentDiv.appointmentViewInfo))
                    appointmentDiv = null; //This is dragged appointment
            }
            var resizeDiv = null;
            if (ASPx.IsExists(appointmentDiv))
                resizeDiv = ASPx.GetParentByPartialId(eventSource, "ResizeControlDiv", appointmentDiv); //TODO: string->const
            var selectionDiv = ASPx.GetParentByPartialId(eventSource, this._constDXSelectionDiv());
            var cell = ASPx.GetParentByPartialId(eventSource, this._constDXSchedulerContentCell());
            if (!ASPx.IsExists(cell)) {
                var x = ASPx.Evt.GetEventX(e);
                var y = ASPx.Evt.GetEventY(e);
                if (ASPx.TouchUIHelper.isTouchEvent(e))
                    cell = this.FindCellByMousePositionSlow(x, y);
                else
                    cell = this.FindCellByMousePosition(x, y, appointmentDiv, selectionDiv);
            }
            return new SchedulerHitTestResult(cell, selectionDiv, appointmentDiv, resizeDiv);
        },
        FindCellByMousePosition: function (x, y, appointmentDiv, selectionDiv) {
            var containerIndex = null;
            var firstCellIndex = null;
            var lastCellIndex = null;
            if (ASPx.IsExists(appointmentDiv)) {
                var viewInfo = appointmentDiv.appointmentViewInfo;
                containerIndex = viewInfo.containerIndex;
                firstCellIndex = viewInfo.visibleFirstCellIndex;
                lastCellIndex = viewInfo.visibleLastCellIndex;
            }
            else {
                if (ASPx.IsExists(selectionDiv)) {
                    containerIndex = selectionDiv.container.index;
                    firstCellIndex = 0;
                    lastCellIndex = selectionDiv.container.cellCount - 1;
                }
            }
            var cell = this.LookupCellByMousePosition(this.horizontalViewInfo, x, y, containerIndex, firstCellIndex, lastCellIndex);
            if (!ASPx.IsExists(cell))
                cell = this.LookupCellByMousePosition(this.verticalViewInfo, x, y, containerIndex, firstCellIndex, lastCellIndex);
            return cell;
        },
        FindCellByMousePositionSlow: function (x, y) {
            var cell = this.LookupCellByMousePositionSlow(this.horizontalViewInfo, x, y);
            if (!ASPx.IsExists(cell))
                cell = this.LookupCellByMousePositionSlow(this.verticalViewInfo, x, y);
            return cell;
        },
        InitializeCell: function (cell) {
            var result = this.cellIdRegExp.exec(cell.id);
            if (!ASPx.IsExists(result) || result.length == 0)
                return;
            var horizontalContainer = (result[2] == "h");
            var containerIndex = parseInt(result[3]);
            var cellIndex = parseInt(result[4]);
            if (horizontalContainer)
                this.horizontalViewInfo.InitializeCell(cell, containerIndex, cellIndex);
            else
                this.verticalViewInfo.InitializeCell(cell, containerIndex, cellIndex);
        },

        CanCreateCallback: function () {
            if (!this.isReadyForCallbacks) {
                return false;
            }
            if (this.constructor.prototype.CanCreateCallback.call(this)) {
                return true;
            }
            return this.funcCallbackCount == this.requestCount && this.funcCallbackCount > 0; //True, if functional callback only
        },
        GetCallbackHandler: function (index) {
            if (index < 0 || index >= this.funcCallbacks.length)
                return null;
            var result = this.funcCallbacks[index];
            this.funcCallbacks[index] = null;
            return result;
        },
        RegisterCallbackHandler: function (onCallback) {
            this.funcCallbackCount++;
            var count = this.funcCallbacks.length;
            for (var i = 0; i < count; i++) {
                if (this.funcCallbacks[i] == null) {
                    this.funcCallbacks[i] = onCallback;
                    return i;
                }
            }
            this.funcCallbacks.push(onCallback);
            return this.funcCallbacks.length - 1;
        },
        ClearFuncCallbacks: function () {
            var count = this.funcCallbacks.length;
            for (var i = 0; i < count; i++) {
                if (this.funcCallbacks[i] != null) {
                    this.funcCallbacks[i] = ASPx.SchedulerEmptyFuncCallbackHandler;
                }
            }
        },
        HideLoadingPanelOnCallback: function () {
            return false;
        },
        RaiseFuncCallback: function (callbackName, args, onCallback) {
            if (!ASPx.IsExists(args) || args == "")
                return;
            this.StopReminderTimer();
            this.ClearTimeMarkerTimer();
            if (this.useDeferredFuncCallback)
                this.AddDeferredFuncCallback(callbackName, args, onCallback);
            else
                this.RaiseFuncCallbackCore(callbackName, args, onCallback);
        },
        RaiseFuncCallbackCore: function (callbackName, args, onCallback) {
            if (this.CanCreateCallback()) {
                var id = this.RegisterCallbackHandler(onCallback);
                this.CreateCallback(callbackName + id.toString() + "," + args);
            }
        },
        AddDeferredFuncCallback: function (callbackName, args, onCallback) {
            var funcCallback = new ASPx.SchedulerFuncCallback(callbackName, args, onCallback);
            this.deferredFuncCallbackList.push(funcCallback);
        },
        BeginDeferredFuncCallbackArea: function () {
            this.useDeferredFuncCallback = true;
        },
        EndDeferredFuncCallbackArea: function () {
            this.useDeferredFuncCallback = false;
            for (var i = 0; i < this.deferredFuncCallbackList.length; i++) {
                var funcCallback = this.deferredFuncCallbackList[i];
                funcCallback.Raise(this);
            }
            this.deferredFuncCallbackList = [];
        },
        //TODO: test it!
        RaiseCallback: function (args) {
            this.TimeMarkerBeginUpdate();
            this.UnsubscribeEvents();
            this.StopReminderTimer();
            this.ClearTimeMarkerTimer();
            this.HideAllToolTips();
            this.DisableToolTips();
            if (ASPx.IsExists(this.callBack) && this.isCallbackMode) {
                if (!this.CanCreateCallback())
                    return;
                this.ShowLoadingElements();
                var statusInfoManager = this.statusInfoManager;
                if (ASPx.IsExists(statusInfoManager))
                    statusInfoManager.Clear();
                this.CreateCallback(args, args);
                this.ClearFuncCallbacks();
                this.isReadyForCallbacks = false;
            }
            else {
                ASPx.ControlResizeManager.Clear();
                this.SendPostBack(args);
            }
        },
        PerformCallbackHandler: function (index, res) {
            var handler = this.GetCallbackHandler(index);
            if (handler != null)
                handler(res);
        },
        OnFuncCallback: function (result) {
            eval(result);
        },
        DoCallback: function (result) {
            this.constructor.prototype.DoCallback.call(this, result);
            this.EnsureAllToolTipInitialized();
        },
        OnCallback: function (resultObj) {
            var result = resultObj.result;
            this.EnableToolTips();
            if (result.indexOf("FB|") == 0) {
                this.funcCallbackCount--;
                window.setTimeout("ASPx.SchedulerFuncCallbackHandler(\"" + this.name + "\", \"" + escape(result.substr(3)) + "\");", 0);
                //this.isReadyForCallbacks = true;
                return;
            }
            //this.appointmentsBlockChanged = false;
            this.changedBlocks = {};
            // TO FIX
            if (typeof ASPx.GetDropDownCollection != "undefined") {
                ASPx.GetDropDownCollection().focusedControlName = "";
                ASPx.GetDropDownCollection().droppedControlName = "";
            }

            ASPx.Selection.Clear();
            this.HideCurrentPopupContainerCore();
            var element = this.GetMainElement();
            if (element != null) {
                this.DisableResize();
                //this.UnsubscribeWindowResizeEvent();
                ASPx.RelatedControlManager.ParseResult(result);
            }
            this.UpdateStateObjectWithObject(resultObj.stateObject);
        },
        OnCallbackError: function (result, data) {
            if (ASPx.IsExists(this.onCallbackError))
                this.onCallbackError();
            ASPx.Selection.Clear();
            this.HideLoadingElements();
            this.isReadyForCallbacks = true;
            if (!this.NotifyCallbackError(result))
                alert("CALLBACK ERROR: " + result);
        },
        OnCallbackGeneralError: function (result) {
            var length = result.length;
            this.OnCallbackError(length + ",0|" + result, null);
        },
        OnHandledException: function () {
            if (ASPx.IsExists(this.onCallbackError))
                this.onCallbackError();
            this.StartReminderTimer();
            this.EnableToolTips();
            this.SetTimeMarkerTimer();
            this.TimeMarkerEndUpdate();
            this.HideLoadingElements();
            this.isReadyForCallbacks = true;
            this.EnableResize();
        },
        ShowLoadingPanel: function () {
            this.CreateLoadingPanelWithAbsolutePosition(this.GetMainElement().parentNode, this.GetMainElement());
        },
        ShowLoadingDiv: function () {
            this.schedulerLoadingDiv = this.CreateLoadingDiv(this.GetMainElement().parentNode, this.GetMainElement());
            if (ASPx.IsExistsElement(this.schedulerLoadingDiv)) {
                this.showLoadingDivContextMenuDelegate = ASPx.CreateDelegate(this.ShowLoadingDivContextMenu, this);
                ASPx.Evt.AttachEventToElement(this.schedulerLoadingDiv, "contextmenu", this.showLoadingDivContextMenuDelegate, true);
            }
        },
        HideLoadingDiv: function () {
            if (ASPx.IsExistsElement(this.schedulerLoadingDiv)) {
                ASPx.Evt.DetachEventFromElement(this.schedulerLoadingDiv, "contextmenu", this.showLoadingDivContextMenuDelegate);
                this.schedulerLoadingDiv = null;
                this.showLoadingDivContextMenuDelegate = null;
            }
            this.constructor.prototype.HideLoadingDiv.call(this);
        },
        ShowLoadingDivContextMenu: function (evt) {
            evt = ASPx.Evt.GetEvent(evt);
            var x = ASPx.Evt.GetEventX(evt);
            var y = ASPx.Evt.GetEventY(evt);
            var hitTestResult = this.CalcHitTestFromPosition(x, y);
            if (hitTestResult != null && ASPx.IsExists(hitTestResult.appointmentDiv)) {
                this.menuManager.ShowAptMenu(hitTestResult.appointmentDiv, evt);
            }
            else {
                this.menuManager.ShowViewMenu(this, evt);
            }
            evt.returnValue = false;
            evt.cancelBubble = true;
            if (evt.preventDefault)
                evt.preventDefault();
            return evt.returnValue; //B144126
        },
        GetCallbackAnimationElement: function () {
            return ASPx.GetElementById(this.name + "_containerBlock_innerContent");
        },

        ProcessCallbackResult: function (id, html, params) {
            if (!this.changedBlocks.ContainerBlockChanged && this.GetBlockElementId("containerBlock", "innerContent") == id) {
                this.changedBlocks.ContainerBlockChanged = true;
                this.RemoveContainerBlockChilds();
            }
            if (this.GetBlockElementId("formBlock", "innerContent") == id) {
                if (params == "#UsePrevResult") {
                    html = this.prevCallbackResults.formBlock;
                    params = "";
                }
                else {
                    this.prevCallbackResults.formBlock = html;
                }
            }
            if (!this.changedBlocks.AppointmentsBlockChanged && this.GetBlockElementId("aptsBlock", "innerContent") == id) {
                this.changedBlocks.AppointmentsBlockChanged = true;
                this.ClearCache(this.appointmentDivCache, true);
            }
            ASPx.RelatedControlManager.ProcessCallbackResultDefault(id, html, params);

            if (!this.changedBlocks.NavButtonsBlockChanged && this.GetBlockElementId("navButtonsBlock", "innerContent") == id) {
                this.changedBlocks.NavButtonsBlockChanged = true;
                this.ClearNavigationButtons();
            }
        },
        RemoveContainerBlockChilds: function () {
            this.ClearCache(this.navButtonsCache, false);
            this.ClearCache(this.appointmentDivCache, true);
        },
        ClearCache: function (cache, alwaysCheckParent) {
            var parent = null;
            for (var divId in cache) {
                var div = cache[divId];
                if (!parent || alwaysCheckParent)
                    parent = div.parentNode;
                if (ASPx.IsExists(parent) && ASPx.IsExists(parent.tagName)) {
                    //ASPx.SchedulerGlobals.RemoveChildFromParent(parent, div);
                    parent.removeChild(div);
                }
            }
        },
        NotifyCallbackError: function (msg) {
            var statusInfoManager = this.statusInfoManager;
            if (ASPx.IsExists(statusInfoManager)) {
                statusInfoManager.ShowExceptionInfo(msg);
                return true;
            }
            else
                return false;
        },
        SetErrorInfoRowVisibility: function (cell, visible) {
            var row = cell.parentNode;
            if (ASPx.IsExists(row) && row.tagName == "TR")
                ASPx.SetElementDisplay(row, visible);
        },
        ShowResourceNavigatorRow: function (visible) {
            if (ASPx.IsExists(this.resourceNavigatorRow)) {
                ASPx.SetElementDisplay(this.resourceNavigatorRow, visible);
            }
        },
        SetCheckSums: function (checkSums) {
            this.stateObject.checkSums = checkSums;
        },

        FindViewInfosByAppointmentId: function (appointmentId) {
            var result = this.horizontalViewInfo.FindViewInfosByAppointmentId(appointmentId);
            return result.concat(this.verticalViewInfo.FindViewInfosByAppointmentId(appointmentId));
        },
        RemoveViewInfosByAppointmentId: function (appointmentId) {
            this.horizontalViewInfo.RemoveViewInfosByAppointmentId(appointmentId);
            this.verticalViewInfo.RemoveViewInfosByAppointmentId(appointmentId);
        },
        //TODO: test it!
        NavBtnClick: function (startTime, duration, resourceId) {
            this.RaiseCallback("NVBTN|" + ASPx.SchedulerGlobals.DateTimeToMilliseconds(startTime) + "," + duration + "," + resourceId);
        },
        SetTimelineScalesInfo: function (enabledScales, visibleScales) {
            this.enabledTimeScalesInfo = enabledScales;
            this.visibleTimeScalesInfo = visibleScales;
        },
        SetCurrentTimeScaleMenuItemName: function (name) {
            this.currentTimeScaleMenuItemName = name;
        },
        OnAppointmentToolTipClick: function (sender, e) {
            var viewInfo = null;
            while (sender != null) {
                var viewInfo = sender.viewInfo;
                if (ASPx.IsExists(viewInfo))
                    break;
                sender = sender.parentNode;
            }
            if (!ASPx.IsExists(viewInfo))
                return;
            var appointmentId = viewInfo.appointmentId;
            if (!this.appointmentSelection.IsAppointmentSelected(appointmentId))
                this.appointmentSelection.SelectSingleAppointment(appointmentId);
            sender.appointmentViewInfo = viewInfo;
            if (this.appointmentSelection.IsAppointmentSelected(appointmentId))//Check, if appointment really was selected
                this.menuManager.ShowAptMenu(sender, e);
        },
        CanShowAppointmentMenu: function (appointment) {
            if (!this.menuManager || !appointment)
                return false;
            return this.menuManager.CanShowAppointmentMenu(appointment);
        },
        GetDateInMilliseconds: function (date) {
            return date.valueOf() - 60000 * date.getTimezoneOffset();
        },
        RefreshClientAppointmentPropertiesCore: function (dictionary) {
            if (!dictionary.appointmentId)
                return;
            var apt = this.GetAppointment(dictionary.appointmentId);
            if (apt == null)
                apt = new ASPxClientAppointment();
            var propertyController = new ASPx.AppointmentPropertyApplyController(this);
            propertyController.ApplyProperties(apt, dictionary);
            if (ASPx.IsExists(this.RefreshClientAppointmentPropertiesUserCallbackFunction)) {
                this.RefreshClientAppointmentPropertiesUserCallbackFunction(apt);
            }
        },
        ////ToolTip support
        HideAllToolTips: function () {
            this.schedulerToolTipHelper.HideToolTip();
            this.GetAppointmentDragTooltip().HideToolTip();
            this.GetSelectionToolTip().HideToolTip();
            this.GetAppointmentToolTip().HideToolTip();
            if (ASPx.IsExists(this.activeToolTip) && this.activeToolTip != null) {
                this.activeToolTip.HideToolTip();
                this.activeToolTip = null;
            }
        },
        DisableToolTips: function () {
            this.toolTipsEnable = false;
        },
        EnableToolTips: function () {
            this.toolTipsEnable = true;
        },
        GetAppointmentDragTooltip: function () {
            var toolTip = ASPx.GetControlCollection().Get(this.appointmentDragToolTip);
            return toolTip;
        },
        GetSelectionToolTip: function () {
            var toolTip = ASPx.GetControlCollection().Get(this.selectionToolTip);
            return toolTip;
        },
        GetAppointmentToolTip: function () {
            var toolTip = ASPx.GetControlCollection().Get(this.appointmentToolTip);
            return toolTip;
        },
        EnsureAllToolTipInitialized: function () {
            this.EnsureToolTipInitialized(this.appointmentToolTip);
            this.EnsureToolTipInitialized(this.selectionToolTip);
            this.EnsureToolTipInitialized(this.appointmentDragToolTip);
        },
        EnsureToolTipInitialized: function (toolTipName) {
            var toolTip = ASPx.GetControlCollection().Get(toolTipName);
            if (!toolTip.isToolTipInitialized) {
                toolTip.aspxParentControl = this;
                if (ASPx.IsExists(toolTip.templatedToolTip)) {
                    var userToolTip = toolTip.templatedToolTip;
                    userToolTip.scheduler = this;
                    userToolTip.Initialize();
                    toolTip.isToolTipInitialized = true;
                }
            }
        },
        ShowSelectionToolTipInernal: function (x, y) {
            if (!this.schedulerToolTipHelper)
                return;
            this.schedulerToolTipHelper.ShowToolTipInstantly(x, y, this.GetSelectionToolTip(), null);
        },
        // time marker
        TimeMarkerBeginUpdate: function () {
            this.isTimeMarkerUpdateLocked = false;
        },
        TimeMarkerEndUpdate: function () {
            this.isTimeMarkerUpdateLocked = true;
            if (this.timeMarkerDeferredRecalc) {
                this.timeMarkerDeferredRecalc = false;
                this.RecalcTimeMarkerCore();
            }
        },
        RecalcTimeMarker: function () {
            var mainElement = this.GetMainElement();
            if (!ASPx.IsExistsElement(mainElement))
                return;
            if (this.isTimeMarkerUpdateLocked) {
                this.RecalcTimeMarkerCore();
            }
            else
                this.timeMarkerDeferredRecalc = true;
        },
        RecalcTimeMarkerCore: function () {
            if (this.canShowDayTimeMarker)
                this.verticalViewInfo.LayoutTimeMarker();
            else
                this.horizontalViewInfo.LayoutTimeMarker();
        },
        SetTimeMarkerTimer: function () {
            this.ClearTimeMarkerTimer();
            this.timeMarkerTimer = window.setInterval(ASPx.CreateDelegate(this.RecalcTimeMarker, this), 1 * 60 * 1000);
        },
        ClearTimeMarkerTimer: function () {
            if (this.timeMarkerTimer) {
                ASPx.Timer.ClearTimer(this.timeMarkerTimer);
                this.timeMarkerTimer = null;
            }
        },
        ResetTimeRulers: function () {
            this.timeRulerCollection = null;
        },
        SetTimeRulers: function (timeRulerInfos) {
            //this.timeRulerLocations = timeRulerLocations;
            var count = timeRulerInfos.length;
            this.timeRulerCollection = [];
            for (var i = 0; i < count; i++) {
                var timeRuler = new ASPx.TimeRuler(timeRulerInfos[i]);
                this.timeRulerCollection.push(timeRuler);
            }
        },
        GetTimeRulers: function () {
            return this.timeRulerCollection;
        },
        ShowMessageBox: function (caption, message, okHandler, cancelHandler) {
            this.messageBox.Show(caption, message, okHandler, cancelHandler);
        },       
        OnDispose: function () {
            ASPxClientControl.prototype.OnDispose.call(this);
            if (ASPx.schedulerGlobalMouseHandler != null && this.mouseHandler != null) {
                ASPx.schedulerGlobalMouseHandler.RemoveHandler(this.mouseHandler);
            }
            if (ASPx.schedulerGlobalKeyboardHandler)
                ASPx.schedulerGlobalKeyboardHandler.RemoveHandler(this.keyboardHandler);
            this.mainDiv = null;
            this.messageBox = null;
        }
    });

    ////////////////////////////////////////////////////////////////////////////////

    //////////////////////////////////////////////////////////////////////////////// 
    // SchedulerMenuManager
    ////////////////////////////////////////////////////////////////////////////////
    var SchedulerMenuManager = ASPx.CreateClass(null, {
        constructor: function (scheduler) {
            this.scheduler = scheduler;
            this.deferredMenus = [];
        },
        //TODO: test it!
        // public
        OnAptMenuClick: function (itemName) {
            if (this.OnMenuItemClick(itemName))
                return;
            //if (this.IsClientAPIEnabled()) {
            //    if (this.HandleViewMenuClickAtClientSide(itemName))
            //        return;
            //}
            this.HandleAptMenuClickAtServerSide(itemName);
        },
        OnMenuItemClick: function (itemName) {
            this.HideMenu();
            if (this.scheduler.RaiseMenuItemClicked(itemName))
                return true;
        },
        //TODO: test it!
        // public
        OnViewMenuClick: function (itemName) {
            if (this.IsClientAPIEnabled()) {
                if (this.HandleViewMenuClickAtClientSide(itemName))
                    return;
            }
            if (itemName == SchedulerMenuItemId.GotoThisDay) {
                var activeViewType = this.scheduler.GetActiveViewType();
                if (!this.scheduler.RaiseActiveViewChanging(activeViewType, ASPxSchedulerViewType.Day))
                    return;
            }
            this.HandleViewMenuClickAtServerSide(itemName);
        },
        GetAptMenuName: function () {
            return this.scheduler.name + "_aptMenuBlock_SMAPT";
        },
        PrepareAptMenu: function (appointmentViewInfo) {
            var menu = ASPx.GetControlCollection().Get(this.GetAptMenuName());
            if (!ASPx.IsExists(menu))
                return null;
            if (!this.UpdateAppointmentMenuItems(appointmentViewInfo, menu))
                return null;
            return menu;
        },
        DeferredUpdateAppointmentMenuItems: function (menu, x, y) {
            var hitTest = this.scheduler.CalcHitTestFromPosition(x, y);
            if (hitTest == null)
                return;
            if (!ASPx.IsExists(hitTest.appointmentDiv))
                return;
            var appointmentViewInfo = hitTest.appointmentDiv.appointmentViewInfo;
            this.UpdateAppointmentMenuItems(appointmentViewInfo, menu);
        },
        //TODO: test it!
        // public
        ShowAptMenu: function (sender, e) {
            var appointmentViewInfo = sender.appointmentViewInfo;
            if (!this.scheduler.CanCreateCallback()) {
                this.RegisterMenuToDeferredShow(this.GetAptMenuName(), e, ASPx.CreateDelegate(this.DeferredUpdateAppointmentMenuItems, this));
                return;
            }
            var menu = this.PrepareAptMenu(appointmentViewInfo);
            if (menu)
                this.ShowMenu(e, menu);
        },
        GetViewMenuName: function () {
            return this.scheduler.name + "_viewMenuBlock_SMVIEW";
        },
        PrepareViewMenu: function () {
            var menuName = this.GetViewMenuName();
            var menu = ASPx.GetControlCollection().Get(menuName);
            if (!ASPx.IsExists(menu))
                return null;
            this.UpdateViewMenuItems(menu);
            return menu;

        },
        //TODO: test it!
        // public
        ShowViewMenu: function (sender, e) {
            if (!this.scheduler.CanCreateCallback()) {
                this.RegisterMenuToDeferredShow(this.GetViewMenuName(), e, ASPx.CreateDelegate(this.UpdateViewMenuItems, this));
                return;
            }
            var menu = this.PrepareViewMenu();
            if (menu)
                this.ShowMenu(e, menu);
        },
        ShowDeferredMenus: function () {
            var count = this.deferredMenus.length;
            for (var i = 0; i < count; i++) {
                var deferredMenuInfo = this.deferredMenus[i];
                var menu = ASPx.GetControlCollection().Get(deferredMenuInfo.Name);
                deferredMenuInfo.Update(menu, deferredMenuInfo.X, deferredMenuInfo.Y);
                if (menu) {
                    menu.ShowAtPos(deferredMenuInfo.X, deferredMenuInfo.Y);
                }
            }
            this.deferredMenus = [];
        },
        RegisterMenuToDeferredShow: function (name, evt, updateMethod) {
            var x = ASPx.Evt.GetEventX(evt);
            var y = ASPx.Evt.GetEventY(evt);
            var obj = { 'Name': name, 'X': x, 'Y': y, "Update": updateMethod };
            this.deferredMenus.push(obj);
        },
        //TODO: test it!
        ShowMenu: function (e, menu) {
            this.scheduler.HideAllToolTips();
            if (this.scheduler.activeFormType != ASPx.SchedulerFormType.None)
                return;
            menu.ShowInternal(e);
            e.returnValue = false;
            e.cancelBubble = true;
            ASPx.Selection.Clear();
        },
        HideMenu: function (e, menu) {
            ASPxClientMenuBase.GetMenuCollection().HideAll();
        },
        HandleAptMenuClickAtServerSide: function (itemName) {
            if (itemName == SchedulerMenuItemId.DeleteAppointment) {
                if (this.HandleDeleteMenuItemClick())
                    return;
            }
            this.scheduler.RaiseCallback("MNUAPT|" + itemName);
        },
        HandleDeleteMenuItemClick: function () {
            var aptIds = this.scheduler.appointmentSelection.selectedAppointmentIds;
            var aptIdCollection = [];
            for (var aptIdIndx in aptIds) {
                var aptId = aptIds[aptIdIndx];
                aptIdCollection.push(aptId);
            }
            return this.scheduler.RaseAppointmentDeleting(aptIdCollection);
        },
        HandleViewMenuClickAtClientSide: function (itemName) {
            if (this.OnMenuItemClick(itemName))
                return true;
            switch (itemName) {
                case SchedulerMenuItemId.SwitchToDayView:
                    this.scheduler.SetActiveViewType(ASPxSchedulerViewType.Day);
                    return true;
                case SchedulerMenuItemId.SwitchToWorkWeekView:
                    this.scheduler.SetActiveViewType(ASPxSchedulerViewType.WorkWeek);
                    return true;
                case SchedulerMenuItemId.SwitchToWeekView:
                    this.scheduler.SetActiveViewType(ASPxSchedulerViewType.Week);
                    return true;
                case SchedulerMenuItemId.SwitchToMonthView:
                    this.scheduler.SetActiveViewType(ASPxSchedulerViewType.Month);
                    return true;
                case SchedulerMenuItemId.SwitchToTimelineView:
                    this.scheduler.SetActiveViewType(ASPxSchedulerViewType.Timeline);
                    return true;
                case SchedulerMenuItemId.SwitchToFullWeekView:
                    this.scheduler.SetActiveViewType(ASPxSchedulerViewType.FullWeek);
                    return true;
                default:
                    return false;
            }
        },
        HandleViewMenuClickAtServerSide: function (menuItemName) {
            if (menuItemName == "GotoToday")
                this.scheduler.AssignSlideAnimationDirectionByDate(new Date());
            this.scheduler.RaiseCallback("MNUVIEW|" + menuItemName);
        },
        IsClientAPIEnabled: function () {
            return ASPx.IsExists(this.scheduler.SetActiveViewType);
        },
        CanShowAppointmentMenu: function (appointment) {
            return this.CalculateAppointmentMenuItemVisibility(appointment).isMenuVisible;
        },
        CalculateAppointmentMenuItemVisibility: function (appointment) {
            var itemsInfo = {};
            itemsInfo.isMenuVisible = false;

            var aptFlags = appointment.flags;
            if (aptFlags == null)
                return itemsInfo;

            var canShowForm = this.scheduler.aptFormVisibility != ASPx.SchedulerFormVisibility.None;
            var isSingleAppointment = this.scheduler.appointmentSelection.IsSingleAppointmentSelected();
            itemsInfo.openAppointmentVisible = canShowForm && aptFlags.allowEdit && isSingleAppointment;
            itemsInfo.deleteAppointmentVisible = aptFlags.allowDelete;
            itemsInfo.labelSubMenuVisible = aptFlags.allowEdit;
            itemsInfo.statusSubMenuVisible = aptFlags.allowEdit;

            var aptType = appointment.appointmentType;
            var isRecurring = aptType != ASPxAppointmentType.Normal;
            itemsInfo.editSeriesVisible = canShowForm && aptFlags.allowEdit && isRecurring && isSingleAppointment;

            var isException = (aptType == ASPxAppointmentType.ChangedOccurrence || aptType == ASPxAppointmentType.DeletedOccurrence);
            itemsInfo.restoreOccurrenceVisible = aptFlags.allowEdit && isException && isSingleAppointment;

            itemsInfo.isMenuVisible = itemsInfo.openAppointmentVisible | itemsInfo.deleteAppointmentVisible | itemsInfo.labelSubMenuVisible | itemsInfo.statusSubMenuVisible | aptFlags.alowEdit |
                                        itemsInfo.editSeriesVisible | itemsInfo.restoreOccurrenceVisible;

            return itemsInfo;
        },
        //TODO: test it!
        UpdateAppointmentMenuItems: function (appointmentViewInfo, menu) {
            if (!ASPx.IsExists(appointmentViewInfo))
                return;

            var apt = this.scheduler.GetAppointment(appointmentViewInfo.appointmentId);
            if (apt == null)
                return;
            var itemsInfo = this.CalculateAppointmentMenuItemVisibility(apt);
            if (!itemsInfo || !itemsInfo.isMenuVisible)
                return;

            this.SetMenuItemVisibility(menu, SchedulerMenuItemId.OpenAppointment, itemsInfo.openAppointmentVisible);
            this.SetMenuItemVisibility(menu, SchedulerMenuItemId.DeleteAppointment, itemsInfo.deleteAppointmentVisible);

            this.SetMenuItemVisibility(menu, SchedulerMenuItemId.LabelSubMenu, itemsInfo.labelSubMenuVisible);
            this.SetMenuItemVisibility(menu, SchedulerMenuItemId.StatusSubMenu, itemsInfo.statusSubMenuVisible);

            if (itemsInfo.labelSubMenuVisible || itemsInfo.statusSubMenuVisible) {
                this.UpdateAptSubMenu(menu, SchedulerMenuItemId.LabelSubMenu, apt.labelIndex);
                this.UpdateAptSubMenu(menu, SchedulerMenuItemId.StatusSubMenu, apt.statusIndex);
            }

            this.SetMenuItemVisibility(menu, SchedulerMenuItemId.EditSeries, itemsInfo.editSeriesVisible);
            this.SetMenuItemVisibility(menu, SchedulerMenuItemId.RestoreOccurrence, itemsInfo.restoreOccurrenceVisible);

            return itemsInfo.isMenuVisible;
        },
        UpdateAptSubMenu: function (menu, subMenuName, subMenuItemIndex) {
            var subMenu = menu.GetItemByName(subMenuName);
            if (!ASPx.IsExists(subMenu))
                return;
            this.UncheckAllSubMenuItems(subMenu);
            var item = subMenu.GetItem(subMenuItemIndex);
            if (ASPx.IsExists(item))
                item.SetChecked(true);
        },
        UncheckAllSubMenuItems: function (subMenu) {
            var count = subMenu.items.length;
            for (var i = 0; i < count; i++) {
                var item = subMenu.items[i];
                item.SetChecked(false);
            }
        },
        //TODO: test it!
        UpdateViewMenuItems: function (menu) {
            this.UpdateSwitchViewSubMenu(menu);
            var viewType = this.scheduler.GetActiveViewType();
            if (viewType == ASPxSchedulerViewType.Timeline) {
                this.UpdateEnabledScalesSubMenu(menu);
                this.UpdateVisibleScalesSubMenu(menu);
            }
            else if (viewType == ASPxSchedulerViewType.Day || viewType == ASPxSchedulerViewType.WorkWeek || viewType == ASPxSchedulerViewType.FullWeek)
                this.UpdateTimeSlotMenuItems(menu);

            var canShowAptForm = this.scheduler.aptFormVisibility != ASPx.SchedulerFormVisibility.None;
            this.SetMenuItemVisibility(menu, SchedulerMenuItemId.NewAppointment, canShowAptForm);
            this.SetMenuItemVisibility(menu, SchedulerMenuItemId.NewAllDayEvent, canShowAptForm);
            this.SetMenuItemVisibility(menu, SchedulerMenuItemId.NewRecurringAppointment, canShowAptForm);
            this.SetMenuItemVisibility(menu, SchedulerMenuItemId.NewRecurringEvent, canShowAptForm);

            this.SetMenuItemVisibility(menu, SchedulerMenuItemId.GotoDate, this.scheduler.gotoDateFormVisibility != ASPx.SchedulerFormVisibility.None);
        },
        SetMenuItemVisibility: function (menu, itemName, visible) {
            var item = menu.GetItemByName(itemName);
            if (ASPx.IsExists(item))
                item.SetVisible(visible);
        },
        UpdateSwitchViewSubMenu: function (menu) {
            var subMenu = menu.GetItemByName(SchedulerMenuItemId.SwitchViewMenu);
            if (!ASPx.IsExists(subMenu))
                return;

            var itemId = this.GetCurrentViewMenuId();
            var item = subMenu.GetItemByName(itemId);
            if (!ASPx.IsExists(item))
                return;
            item.SetChecked(true);
        },
        UpdateEnabledScalesSubMenu: function (menu) {
            var subMenu = menu.GetItemByName(SchedulerMenuItemId.TimeScaleEnable);
            if (!ASPx.IsExists(subMenu))
                return;
            var count = subMenu.GetItemCount();
            for (var i = 0; i < count; i++)
                subMenu.GetItem(i).SetChecked(this.scheduler.enabledTimeScalesInfo[i] != 0);
        },
        UpdateVisibleScalesSubMenu: function (menu) {
            var subMenu = menu.GetItemByName(SchedulerMenuItemId.TimeScaleVisible);
            if (!ASPx.IsExists(subMenu))
                return;
            var count = subMenu.GetItemCount();
            for (var i = 0; i < count; i++) {
                var item = subMenu.GetItem(i);
                if (this.scheduler.enabledTimeScalesInfo[i] != 0) {
                    item.SetEnabled(true);
                    item.SetChecked(this.scheduler.visibleTimeScalesInfo[i] != 0);
                }
                else {
                    item.SetEnabled(false);
                    item.SetChecked(false);
                }
            }
        },
        UpdateTimeSlotMenuItems: function (menu) {
            var item = menu.GetItemByName(this.scheduler.currentTimeScaleMenuItemName);
            if (!ASPx.IsExists(item))
                return;
            item.SetChecked(true);
        },
        GetCurrentViewMenuId: function () {
            switch (this.scheduler.GetActiveViewType()) {
                default:
                case ASPxSchedulerViewType.Day:
                    return SchedulerMenuItemId.SwitchToDayView;
                case ASPxSchedulerViewType.WorkWeek:
                    return SchedulerMenuItemId.SwitchToWorkWeekView;
                case ASPxSchedulerViewType.Week:
                    return SchedulerMenuItemId.SwitchToWeekView;
                case ASPxSchedulerViewType.Month:
                    return SchedulerMenuItemId.SwitchToMonthView;
                case ASPxSchedulerViewType.Timeline:
                    return SchedulerMenuItemId.SwitchToTimelineView;
                case ASPxSchedulerViewType.FullWeek:
                    return SchedulerMenuItemId.SwitchToFullWeekView;
            }
        }
    });

    //////////////////////////////////////////////////////////////////////////////// 
    // Global functions for popup menu support
    //////////////////////////////////////////////////////////////////////////////// 
    //TODO: test it!
    ASPx.SchedulerShowViewMenu = function (name, sender, evt) {
        var scheduler = ASPx.GetControlCollection().Get(name);
        evt = ASPx.Evt.GetEvent(evt);
        if (scheduler != null && scheduler.activeFormType != ASPx.SchedulerFormType.None)
            return evt.returnValue;
        if (scheduler != null)
            scheduler.menuManager.ShowViewMenu(sender, evt);
        evt.returnValue = false;
        evt.cancelBubble = true;
        return evt.returnValue; //B144126
    }
    //TODO: test it!
    ASPx.SchedulerShowAptMenu = function (name, sender, evt) {
        evt = ASPx.Evt.GetEvent(evt);
        var scheduler = ASPx.GetControlCollection().Get(name);
        if (scheduler != null)
            scheduler.menuManager.ShowAptMenu(sender, evt);
        evt.returnValue = false;
        evt.cancelBubble = true;
    }
    //TODO: test it!
    function aspxSchedulerGetSchedulerFromMenu(menu) {
        if (menu == null) return null;
        var pos = menu.name.lastIndexOf("_");
        if (pos < 0)
            return null;
        var name = menu.name.substring(0, pos);
        pos = name.lastIndexOf("_");
        if (pos > -1)
            return ASPx.GetControlCollection().Get(menu.name.substring(0, pos));
        return null;
    }
    //TODO: test it!
    ASPx.SchedulerOnViewMenuClick = function (s, args) {
        var scheduler = aspxSchedulerGetSchedulerFromMenu(s);
        if (scheduler != null) {
            if (args.item.GetItemCount() <= 0)
                scheduler.menuManager.OnViewMenuClick(args.item.name);
        }
    }
    //TODO: test it!
    ASPx.SchedulerOnAptMenuClick = function (s, args) {
        var scheduler = aspxSchedulerGetSchedulerFromMenu(s);
        if (scheduler != null) {
            if (args.item.GetItemCount() <= 0)
                scheduler.menuManager.OnAptMenuClick(args.item.name);
        }
    }



    //////////////////////////////////////////////////////////////////////////////// 
    // ASPxClientTimeInterval
    ////////////////////////////////////////////////////////////////////////////////
    var ASPxClientTimeInterval = ASPx.CreateClass(null, {
        constructor: function (start, duration) {
            this.start = start;
            this.duration = duration;
            this.allDay = false;
        },
        __toJsonExceptKeys: ['end'],
        ToString: function () {
            return ASPx.SchedulerGlobals.DateTimeToMilliseconds(this.start) + "," + this.duration;
        },
        GetAllDay: function () {
            return this.allDay;
        },
        SetAllDay: function (value) {
            this.allDay = value;
        },
        GetStart: function () {
            if (this.allDay)
                return ASPxSchedulerDateTimeHelper.TruncToDate(this.start);
            return this.start;
        },
        GetDuration: function () {
            if (this.allDay)
                return this.CalculateAllDayDuration();
            return this.duration;
        },
        GetEnd: function () {
            return ASPx.SchedulerGlobals.DateIncreaseWithUtcOffset(this.GetStart(), this.GetDuration());
        },
        SetStart: function (newStartTime) {
            if (this.allDay) {
                newStartTime = ASPxSchedulerDateTimeHelper.TruncToDate(newStartTime);
                var nonAllDayEnd = ASPx.SchedulerGlobals.DateIncreaseWithUtcOffset(this.start, this.duration);
                var oldStartDate = ASPxSchedulerDateTimeHelper.TruncToDate(this.start);
                this.duration = ASPx.SchedulerGlobals.DateSubsWithTimezone(nonAllDayEnd, oldStartDate);
            }
            this.start = newStartTime;
        },
        SetDuration: function (newDuration) {
            this.duration = newDuration;
        },
        SetEnd: function (newEndTime) {
            this.duration = ASPx.SchedulerGlobals.DateSubsWithTimezone(newEndTime, this.GetStart());
        },
        Equals: function (interval) {
            if (ASPx.IsExists(interval) && ASPx.IsExists(interval.start) && ASPx.IsExists(interval.duration))
                return (interval.GetStart() - this.GetStart()) == 0 && (interval.GetDuration() - this.GetDuration()) == 0;
            else
                return false;
        },
        IntersectsWith: function (interval) {
            return (interval.GetStart() - this.GetStart()) >= 0 && (interval.GetDuration() - this.GetDuration()) <= 0;
        },
        IntersectsWithExcludingBounds: function (interval) {
            if (this.GetDuration() == 0 && interval.GetDuration() == 0 && interval.GetStart() - this.GetStart() == 0)
                return true;
            return (interval.GetEnd() - this.GetStart()) > 0 && (interval.GetStart() - this.GetEnd()) < 0;
        },
        Contains: function (interval) {
            if (interval == null)
                return false;
            return interval.GetStart() >= this.GetStart() && interval.GetDuration() <= this.GetDuration();
        },
        IsSmallerThanDay: function () {
            return this.duration < ASPxSchedulerDateTimeHelper.DaySpan;
        },
        IsDurationEqualToDay: function () {
            return this.duration == ASPxSchedulerDateTimeHelper.DaySpan;
        },
        IsZerroDurationInterval: function () {
            return this.duration == 0;
        },
        Clone: function () {
            return new ASPxClientTimeInterval(this.start, this.duration);
        },
        CalculateAllDayDuration: function () {
            var end = ASPx.SchedulerGlobals.DateIncreaseWithUtcOffset(this.start, this.duration);
            var span = ASPxSchedulerDateTimeHelper.CeilDateTime(end, ASPxSchedulerDateTimeHelper.DaySpan) - ASPxSchedulerDateTimeHelper.TruncToDate(this.start);
            if (span > ASPxSchedulerDateTimeHelper.DaySpan)
                return span;
            return ASPxSchedulerDateTimeHelper.DaySpan;
        }
    });
    ASPxClientTimeInterval.CalculateDuration = function (start, end) {
        return ASPx.SchedulerGlobals.DateSubsWithTimezone(end, start);
    }
    //////////////////////////////////////////////////////////////////////////////// 

    var TopRowTimeManager = ASPx.CreateClass(null, {
        constructor: function (scheduler) {
            this.scheduler = scheduler;
            this.topRowTime = {};
            this.topRowTime.duration = 0;
            this.topRowTime.scrollOffset = -1;
        },
        SetTopRowTime: function (duration) {
            var viewInfo = this.scheduler.verticalViewInfo;
            if (viewInfo.cellContainers.length < 1)
                return;
            var container = viewInfo.cellContainers[0];
            var containerStartDate = container.interval.GetStart();
            var containerDay = ASPxSchedulerDateTimeHelper.TruncToDate(containerStartDate);
            var seekTime = new Date(containerDay.valueOf() + duration);
            var cell = viewInfo.FindStartCellByTime(container, seekTime);
            if (!ASPx.IsExists(cell))
                return false;
            var cellInterval = this.scheduler.GetCellInterval(cell);
            var cellDuration = cellInterval.duration;
            var cellHeight = cell.offsetHeight
            var remainderDuration = seekTime.valueOf() - cellInterval.GetStart().valueOf();
            var scrollOffset = cell.offsetTop + cellHeight * remainderDuration / cellDuration;
            var scrollContainer = viewInfo.GetScrollContainer();
            scrollContainer.scrollTop = scrollOffset;
            return true;
        },
        CaclulateTopRowTime: function () {
            var viewInfo = this.scheduler.verticalViewInfo;
            if (viewInfo.cellContainers.length < 1)
                return null;
            var container = viewInfo.cellContainers[0];
            var scrollContainer = viewInfo.GetScrollContainer();
            var testCell = viewInfo.GetCell(0, 0);
            var scrollTop = scrollContainer.scrollTop;
            var cell = viewInfo.FindCellByPosition(0, 0, container.cellCount - 1, testCell.offsetWidth / 2, scrollTop);
            var isScrollTopChanged = false;
            if (cell == null) {// B145788 can't find any cell, try to search at position - 1 px
                scrollTop += 1;
                isScrollTopChanged = true;
                cell = viewInfo.FindCellByPosition(0, 0, container.cellCount - 1, testCell.offsetWidth / 2, scrollTop);
            }
            if (isScrollTopChanged)
                scrollTop -= 1;
            var reminderOffset = scrollTop - cell.offsetTop;
            var cellHeight = cell.offsetHeight;
            var cellInterval = this.scheduler.GetCellInterval(cell);
            var cellDuration = cellInterval.duration;
            var scrollTimeMs = cellInterval.GetStart().valueOf();
            scrollTimeMs += cellDuration * reminderOffset / cellHeight;
            return ASPxSchedulerDateTimeHelper.ToDayTime(new Date(scrollTimeMs));
        },
        SetTopRowTimeField: function (dayViewStateString, workWeekViewStateString, fullWeekViewStateString) {
            this.SaveTopRowTimeState(this.CreateTopRowTimeStateFromString(dayViewStateString), ASPxSchedulerViewType.Day);
            this.SaveTopRowTimeState(this.CreateTopRowTimeStateFromString(workWeekViewStateString), ASPxSchedulerViewType.WorkWeek);
            this.SaveTopRowTimeState(this.CreateTopRowTimeStateFromString(fullWeekViewStateString), ASPxSchedulerViewType.FullWeek);
        },
        SetTopRowTimeState: function (topRowTimeState, viewType) {
            var viewInfo = this.scheduler.verticalViewInfo;
            if (!this.IsDayView(viewType) || viewInfo.cellContainers.length < 1)
                return;
            if (this.scheduler.privateActiveViewType == viewType) {
                if (topRowTimeState.scrollOffset < 0)
                    this.SetTopRowTime(topRowTimeState.duration);
                else
                    viewInfo.GetScrollContainer().scrollTop = topRowTimeState.scrollOffset;
            }
            else
                this.SaveTopRowTimeState(topRowTimeState, viewType);
        },
        GetTopRowTimeState: function (viewType) {
            if (!this.IsDayView(viewType))
                return 0;
            return this.topRowTime[viewType];
        },
        SaveActiveViewTopRowTime: function () {
            var viewInfo = this.scheduler.verticalViewInfo;
            if (!this.IsDayView(this.scheduler.privateActiveViewType) || viewInfo.cellContainers.length < 1)
                return;
            var duration = this.CaclulateTopRowTime();
            var scrollOffset = viewInfo.GetScrollContainer().scrollTop;
            var topRowTimeState = this.CreateTopRowTimeState(duration, scrollOffset);
            this.SaveTopRowTimeState(topRowTimeState, this.scheduler.privateActiveViewType);
        },
        SaveTopRowTimeState: function (state, viewType) {
            if (!this.IsDayView(viewType))
                return;
            var key = "";
            if (viewType == ASPxSchedulerViewType.Day)
                key = "dayViewTopRowTime";
            else if (viewType == ASPxSchedulerViewType.WorkWeek)
                key = "workWeekViewTopRowTime";
            else if (viewType == ASPxSchedulerViewType.FullWeek)
                key = "fullWeekViewTopRowTime";
            if (key == "")
                return;
            this.topRowTime[viewType] = state;
            this.scheduler.stateObject[key] = state.duration + "," + state.scrollOffset;
        },
        CreateTopRowTimeState: function (duration, scrollOffset) {
            return { 'duration': duration, 'scrollOffset': scrollOffset };
        },
        CreateTopRowTimeStateFromString: function (stringValue) {
            var values = stringValue.split(',');
            return this.CreateTopRowTimeState(parseInt(values[0]), parseInt(values[1]));
        },
        IsDayView: function (viewType) {
            return viewType == ASPxSchedulerViewType.Day || viewType == ASPxSchedulerViewType.WorkWeek || viewType == ASPxSchedulerViewType.FullWeek;
        }
    });
    var SchedulerUTCDate = ASPx.CreateClass(null, {
        constructor: function (timeIntervalInMs) {
            var ms = parseInt(timeIntervalInMs);
            this.date = new Date(ms);
        },
        getTime: function () {
            return this.date.getUTCTime();
        },
        getDate: function () {
            return this.date.getUTCDate();
        },
        getDay: function () {
            return this.date.getUTCDay();
        },
        getFullYear: function () {
            return this.date.getUTCFullYear();
        },
        getMilliseconds: function () {
            return this.date.getUTCMilliseconds();
        },
        getMinutes: function () {
            return this.date.getUTCMinutes();
        },
        getMonth: function () {
            return this.date.getUTCMonth();
        },
        getSeconds: function () {
            return this.date.getUTCSeconds();
        },
        getTime: function () {
            return this.date.getTime();
        },
        getTimezoneOffset: function () {
            return this.date.getTimezoneOffset();
        },
        getUTCDate: function () {
            return this.date.getUTCDate();
        },
        getUTCDay: function () {
            return this.date.getUTCDay();
        },
        getUTCHours: function () {
            return this.date.getUTCHours();
        },
        getHours: function () {
            return this.date.getUTCHours();
        },
        getUTCMilliseconds: function () {
            return this.date.getUTCMilliseconds();
        },
        getUTCMonth: function () {
            return this.date.getUTCMonth();
        },
        getUTCMinutes: function () {
            return this.date.getUTCMinutes();
        },
        getUTCSeconds: function () {
            return this.date.getUTCSeconds();
        },
        getYear: function () {
            return this.date.getYear();
        },
        setSeconds: function (arg) {
            this.date.setSeconds(arg);
        },
        setFullYear: function (arg) {
            this.date.setFullYear(arg);
        },
        setMilliseconds: function (arg) {
            this.date.setMilliseconds(arg);
        },
        setTime: function (arg) {
            this.date.setTime(arg);
        },
        setYear: function (arg) {
            this.date.setYear(arg);
        },
        setDate: function (arg) {
            this.date.setDate(arg);
        },
        setUTCDate: function (arg) {
            this.date.setUTCDate(arg);
        },
        setUTCHours: function (arg) {
            this.date.setUTCHours(arg)
        },
        setHours: function (arg) {
            this.date.setHours(arg);
        },
        setUTCMilliseconds: function (arg) {
            this.date.setUTCMilliseconds(arg);
        },
        setUTCMinutes: function (arg) {
            this.date.setUTCMinutes(arg);
        },
        setMinutes: function (arg) {
            this.date.setMinutes(arg);
        },
        setMonth: function (arg) {
            this.date.setMonth(arg);
        },
        setUTCSeconds: function (arg) {
            this.date.setUTCSeconds(arg);
        },
        setUTCFullYear: function (arg) {
            this.date.setUTCFullYear(arg);
        },
        setUTCMonth: function (arg) {
            this.date.setUTCMonth(arg);
        },
        toGMTString: function () {
            return this.date.toGMTString();
        },
        toLocaleFormat: function () {
            return this.date.toLocaleFormat();
        },
        toLocaleTimeString: function () {
            return this.date.toLocaleTimeString();
        },
        toLocaleDateString: function () {
            return this.date.toLocaleDateString();
        },
        toString: function () {
            return this.date.toString();
        },
        toTimeString: function () {
            return this.date.toTimeString();
        },
        toDateString: function () {
            return this.date.toDateString();
        },
        toUTCString: function () {
            return this.date.toUTCString();
        },
        valueOf: function () {
            return this.date.valueOf();
        },
        getUTCFullYear: function () {
            return this.date.getUTCFullYear();
        }
    });


    ASPx.SchedulerMenuItemId = SchedulerMenuItemId;

    window.AppointmentPropertyNames = AppointmentPropertyNames;
    window.ASPxClientScheduler = ASPxClientScheduler;
    window.ASPxClientTimeInterval = ASPxClientTimeInterval;
    //window.ClientSchedulerDefaultMouseHandlerState = ClientSchedulerDefaultMouseHandlerState;
    //window.ClientSchedulerMouseHandler = ClientSchedulerMouseHandler;
    //window.ClientSchedulerMouseHandlerStateBase = ClientSchedulerMouseHandlerStateBase;
    //window.ClientSchedulerAppointmentDragState = ClientSchedulerAppointmentDragState;
    //window.ClientSchedulerAppointmentMouseDownState = ClientSchedulerAppointmentMouseDownState;
    //window.ClientSchedulerCellSelectionMouseHandlerState = ClientSchedulerCellSelectionMouseHandlerState;
})();