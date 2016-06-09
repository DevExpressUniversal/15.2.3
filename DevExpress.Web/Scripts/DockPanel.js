/// <reference path="_references.js"/>

(function() {
var DockPanelStateObserver = ASPx.CreateClass(null, {
    //Ctor
    constructor: function(observedPanel) {
        this.panel = observedPanel;
        this.trackState = true;
        this.previousState = {
            zone: null
        };
        this.currentState = {
            zone: this.panel.zone
        };
    },

    UpdateState: function() {
        if(!this.trackState)
            return;

        this.previousState = this.currentState;
        this.currentState = {
            zone: this.panel.zone
        }
    },

    IsBeingDocked: function() {
        return this.currentState.zone && this.previousState.zone != this.currentState.zone;
    },

    IsBeingFloated: function() {
        return this.previousState.zone && !this.currentState.zone;
    }
});

var ASPxClientDockPanelModes = {
    All: "All",
    DockedOnly: "DockedOnly",
    FloatOnly: "FloatOnly"
};
var ASPxClientDockPanel = ASPx.CreateClass(ASPxClientPopupControl, {
    //Const
    DefaultWindowIndex: -1,
    AnimationDelay: 30,
    MaxAnimationTime: 400,
    AnimationOffset: 20,
    UndockOnDoubleClickOffset: 5,
    StateHiddenInputIDPostfix: '_SHF',
    BeforeDockServerEventName: "BeforeDock",
    AfterDockServerEventName: "AfterDock",
    BeforeFloatServerEventName: "BeforeFloat",
    AfterFloatServerEventName: "AfterFloat",
    RaiseBeforeDockEventCommand: "EBD",
    RaiseAfterDockEventCommand: "EAD",
    RaiseBeforeFloatEventCommand: "EBF",
    RaiseAfterFloatEventCommand: "EAF",


    //Ctor
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);

        //Server-provided fields
        this.panelUID = null;
        this.forbiddenZones = [];
        this.mode = ASPxClientDockPanelModes.All;
        this.requireFreezingLayout = false;

        //Fields
        this.zone = null;
        this.initialParentNode = null;
        this.trackDimensions = true;
        this.allowCorrectYOffsetPosition = false;
        this.fixated = false;
        this.initialShadowVisible = false;
        this.freezed = false;
        this.allowEnsureContent = true;
        this.firstShowProcessed = false;
        this.floatingState = true;
        this.completeSwitchingToFloatingStateOnCallback = false;
        this.floatingStateAllowResize = false;
        this.floatingStateEnableContentScrolling = false;
        this.animationLocked = false;
        this.inUndockedState = false;
        this.contentFlexibilityEnabled = false;
        this.widthFixed = false;
        this.heightFixed = false;
        this.inPostback = false;
        this.dockRestorePanelData = null;
        this.dockedDimensionsCalculating = false;
        this.floatingStateDimensions = {
            width: 0,
            height: 0,
            minWidth: 0,
            minHeight: 0,
            maxWidth: 0,
            maxHeight: 0
        };
        this.floatingStateContentOverflow = {
            x: 'visible',
            y: 'visible',
            both: 'visible'
        };
        this.stateObserver = new DockPanelStateObserver(this);
        this.shouldProcessFirstShowWindow = false;

        //Events
        this.BeforeDock = new ASPxClientEvent();
        this.AfterDock = new ASPxClientEvent();
        this.BeforeFloat = new ASPxClientEvent();
        this.AfterFloat = new ASPxClientEvent();
        this.StartDragging = new ASPxClientEvent();
        this.EndDragging = new ASPxClientEvent();
    },

    //NOTE: this helps prevent duplicate form submission is some cases (see B189327)
    SendPostBack: function(params) {
        if(!this.inPostback) {
            this.inPostback = true;
            ASPxClientControl.prototype.SendPostBack.call(this, params);
        }
    },

    //Controls
    GetMainElement: function() {
        return this.GetWindowElement(this.DefaultWindowIndex);
    },

    GetContentScrollbarsOwner: function() {
        return this.GetWindowContentElement(this.DefaultWindowIndex);
    },

    //Initialization
    InlineInitialize: function() {
        ASPxClientPopupControl.prototype.InlineInitialize.call(this);

        var mainElement = this.GetMainElement();
        this.initialParentNode = mainElement.parentNode;
        mainElement.panelUID = this.panelUID;

        this.heightFixed = this.height > 0;

        ASPx.DockPanelBag.Get().RegisterPanel(this);
    },

    Initialize: function() {
        ASPxClientPopupControl.prototype.Initialize.call(this);

        this.AfterResizing.AddHandler(function(s, e) {
            s.StoreFloatingStateDimensions();
        });

        this.StartDragging.AddHandler(function(s, e) {
            if(!s.GetCollapsed() && s.floatingState) {
                var storedTrackDimensions = s.trackDimensions;
                s.trackDimensions = true;
                s.StoreFloatingStateDimensions();
                s.trackDimensions = storedTrackDimensions;
            }
        });

        this.Shown.AddHandler(function(s, e) {
            s.UpdateManagerClientLayoutState();
        });

        if(this.allowDragging)
            this.AssignDoubleClickEventHandlers();
    },

    InitializeWindow: function(index) {
        ASPxClientPopupControl.prototype.InitializeWindow.call(this, index);

        //HACK: ifwindow is not visible on page load, we need to show it anyway to
        //process docking on first show
        if(!this.GetShowOnPageLoad(index) && this.GetZoneUID()) {
            var savedShown = this.Shown,
                savedCloseUp = this.CloseUp,
                savedClosing = this.Closing,
                eventStub = new ASPxClientEvent();

            //Q416636
            this.Shown = eventStub;
            this.CloseUp = eventStub;
            this.Closing = eventStub;

            this.allowEnsureContent = false;
            this.FirstShowWindow(index, false);
            this.Hide();
            this.allowEnsureContent = true;

            this.Shown = savedShown;
            this.CloseUp = savedCloseUp;
            this.Closing = savedClosing;
        }
    },

    InitializeZone: function() {
        var zoneUID = this.GetZoneUID();
        if(!zoneUID)
            return;

        var zoneList = ASPx.DockZoneBag.Get().GetZoneList();
        for(var i = 0; i < zoneList.length; i++) {
            if(zoneList[i].zoneUID === zoneUID) {
                this.zone = zoneList[i];
                break;
            }
        }
    },

    AssignDoubleClickEventHandlers: function() {
        if(this.isWindowDragging) {
            var mainElement = this.GetMainElement();
            ASPx.Evt.AttachEventToElement(mainElement, 'dblclick', this.GetDoubleClickHandler());
        } else {
            var header = this.GetWindowHeaderElement(this.DefaultWindowIndex);
            if(header)
                ASPx.Evt.AttachEventToElement(header, 'dblclick', this.GetDoubleClickHandler());
        }
    },

    GetDoubleClickHandler: function() {
        var headerBtns = [
            this.GetWindowCloseButton(this.DefaultWindowIndex),
            this.GetWindowPinButton(this.DefaultWindowIndex),
            this.GetWindowRefreshButton(this.DefaultWindowIndex),
            this.GetWindowCollapseButton(this.DefaultWindowIndex),
            this.GetWindowMaximizeButton(this.DefaultWindowIndex)
        ];
        var instance = this;

        return function(evt) {
            var source = ASPx.Evt.GetEventSource(evt);

            for(var i = 0; i < headerBtns.length; i++) {
                if(headerBtns[i] && ASPx.GetIsParent(headerBtns[i], source))
                    return;
            }

            instance.ProcessMouseDoubleClick();
        }
    },

    StoreInitialSettings: function() {
        var contentCurrentStyle = ASPx.GetCurrentStyle(this.GetContentScrollbarsOwner());

        this.floatingStateContentOverflow.x = contentCurrentStyle.overflowX;
        this.floatingStateContentOverflow.y = contentCurrentStyle.overflowY;
        this.floatingStateContentOverflow.both = contentCurrentStyle.overflow;
        this.floatingStateDimensions.minWidth = this.minWidth;
        this.floatingStateDimensions.minHeight = this.minHeight;
        this.floatingStateDimensions.maxWidth = this.maxWidth;
        this.floatingStateDimensions.maxHeight = this.maxHeight;
        this.floatingStateAllowResize = this.allowResize;
        this.floatingStateEnableContentScrolling = this.enableContentScrolling;

        this.initialShadowVisible = this.shadowVisible;
    },

    SetPanelElementsVisibility: function(visible) {
        var elements = [
            this.GetWindowHeaderElement(this.DefaultWindowIndex),
            this.GetWindowContentElement(this.DefaultWindowIndex),
            this.GetWindowFooterElement(this.DefaultWindowIndex)
        ];

        for(var i = 0; i < elements.length; i++) {
            if(elements[i])
                ASPx.SetElementVisibility(elements[i], visible);
        }
    },

    //Q441777
    GetIsDragged: function() {
        return true;
    },

    FirstShowWindow: function(index, allowChangeZIndex) {
        ASPxClientPopupControl.prototype.FirstShowWindow.call(this, index, allowChangeZIndex);

        this.SetPanelElementsVisibility(false);

        //HACK: this made mainly forASPxHtmlEditor, but also works when any
        //iFrame is present inside panel. We need to delay the first show and
        //DOM hierarchy manipulations till all controls initializations and
        //iFrames loading
        var iFrame = ASPx.GetNodeByTagName(this.GetMainElement(), 'IFRAME', 0);
        if(iFrame) 
            this.shouldProcessFirstShowWindow = true;
        else
            this.ProcessFirstShowWindow();
    },
    OnGlobalControlsInitialized: function(args) { 
        if(!this.shouldProcessFirstShowWindow) return;

        window.setTimeout(function() { this.ProcessFirstShowWindow(); }.aspxBind(this), 0);
        this.shouldProcessFirstShowWindow = false;
    },

    ProcessFirstShowWindow: function() {
        if(this.firstShowProcessed)
            return;

        if(this.GetIsCollapsed() || this.GetIsMaximized()) {
            var restoredWindowData = this.GetRestoredWindowData();
            this.StoreFloatingStateDimensionsCore(restoredWindowData.width, restoredWindowData.height);
            this.UpdateManagerClientLayoutState();
        } else
            this.StoreFloatingStateDimensions();
        this.DockOnFirstShow();

        if(this.requireFreezingLayout) {
            this.freezed = true;
            if(this.zone)
                this.DisableDragging();
        }

        this.SetPanelElementsVisibility(true);
        this.SetLastFloatState();
        this.firstShowProcessed = true;
        this.ShowWindowContentUrl(this.DefaultWindowIndex);
    },

    ShowWindowContentUrl: function(index) {
        if(this.firstShowProcessed)
            ASPxClientPopupControl.prototype.ShowWindowContentUrl.call(this, index);
    },

    //Mouse and keyboard events
    ProcessMouseDoubleClick: function() {
        if(this.zone && !this.floatingState && this.mode != ASPxClientDockPanelModes.DockedOnly) {
            if(!this.GetLastFloatPosition()) {
                var position = this.GetDefaultUndockPosition();
                this.SetWindowLeft(this.DefaultWindowIndex, position.x);
                this.SetWindowTop(this.DefaultWindowIndex, position.y);
                this.UpdateWindowsStateCookie();
            }

            if(!this.RaiseBeforeFloat())
                return;

            this.zone.HidePanelPlaceholder();
            this.MakeFloatInternal(this.GetLastFloatPosition());

            if(this.stateObserver.IsBeingFloated())
                this.RaiseAfterFloat();
        }
        else if(this.mode === ASPxClientDockPanelModes.All) {
            if(this.GetIsMaximized(-1)) return;
            var lastDockedState = this.GetState().lastDockedState;
            var zone = ASPx.DockZoneBag.Get().GetZoneByUID(lastDockedState.zoneUID);

            if(!zone || !this.RaiseBeforeDock(zone))
                return;
            this.StoreFloatingStateDimensions();
            this.DockToLastZone();

            if(this.stateObserver.IsBeingDocked())
                this.RaiseAfterDock();
        }
    },

    //State
    GetState: function() {
        var state = this.dockState;
        return {
            zoneUID: state[0],
            visibleIndex: state[1],
            lastDockedState: {
                zoneUID: state[2],
                visibleIndex: state[3]
            },
            lastFloatState: {
                left: state[4],
                top: state[5]
            }
        };
    },

    GetZoneUID: function() {
        return this.GetState().zoneUID;
    },

    SetZoneUID: function(zoneUID) {
        var state = this.GetState();
        this.dockState = [zoneUID || '', state.visibleIndex, state.lastDockedState.zoneUID, state.lastDockedState.visibleIndex, state.lastFloatState.left, state.lastFloatState.top];
    },

    SetLastDockedState: function(lastDockedState) {
        var state = this.GetState();
        this.dockState = [state.zoneUID, state.visibleIndex, lastDockedState.zoneUID, lastDockedState.visibleIndex, state.lastFloatState.left, state.lastFloatState.top];
    },

    SetLastFloatState: function() {
        if(!this.floatingState)
            return;

        var state = this.GetState();
        var position = this.GetElementPosInInitialParentNode(this.GetMainElement());
        position.x = Math.round(position.x);
        position.y = Math.round(position.y);
        this.dockState = [state.zoneUID, state.visibleIndex, state.lastDockedState.zoneUID, state.lastDockedState.visibleIndex, position.x, position.y];
    },

    GetLastFloatPosition: function() {
        var lastFloatState = this.GetState().lastFloatState;

        if(lastFloatState.left && lastFloatState.top) {
            return {
                x: lastFloatState.left,
                y: lastFloatState.top
            }
        }

        return null;
    },

    //Layout state
    GetLayoutStateObject: function() {
        return [
            this.GetVisible(),
            this.mode,
            this.GetZoneUID(),
            this.widthFixed ? (this.floatingStateDimensions.width + '') : '0',
            this.heightFixed ? (this.floatingStateDimensions.height + '') : '0',
            Math.ceil(this.GetCurrentLeft(this.DefaultWindowIndex)),
            Math.ceil(this.GetCurrentTop(this.DefaultWindowIndex)),
            this.GetVisibleIndex()
        ];
    },

    UpdateManagerClientLayoutState: function() {
        var dockManager = ASPxClientDockManager.Get();
        if(dockManager)
            dockManager.UpdatePanelsLayoutState();
    },
    UpdateStateObject: function(){
        ASPxClientPopupControl.prototype.UpdateStateObject.call(this);
        this.UpdateStateObjectWithObject({ dockState: this.dockState });
    },

    //Metrics
    GetCursorOverPanelLocation: function(cursorPos, panelSpacing, isHorizontal) {
        var mainElement = this.GetMainElement();
        var x = ASPx.GetAbsoluteX(mainElement);
        var y = ASPx.GetAbsoluteY(mainElement);
        var width = this.GetWidth();
        var height = this.GetHeight();

        if(isHorizontal)
            width += panelSpacing;
        else
            height += panelSpacing;

        if(cursorPos.x < x || cursorPos.x > x + width || cursorPos.y < y || cursorPos.y > y + height)
            return null;
        if(isHorizontal)
            return cursorPos.x > (x + width / 2) ? 'right' : 'left';
        return cursorPos.y > (y + height / 2) ? 'bottom' : 'top';
    },

    GetDockedDimensions: function(zoneResizableDimension, isHorizontal) {
        this.UpdateRestoredWindowSizeLock();

        var storedDimensions = this.floatingState ? this.floatingStateDimensions :
            { width: this.GetWidth(), height: this.GetHeight() };

        if(this.floatingState)
            this.EnableContentFlexibility();

        var storedContentFlexibilityEnabled = this.contentFlexibilityEnabled;
        var dimensions;

        if(isHorizontal) {
            if(this.floatingStateDimensions.height < zoneResizableDimension && !this.widthFixed) {
                if(storedContentFlexibilityEnabled)
                    this.DisableContentFlexibility();
                this.SetSizeInternal(1, zoneResizableDimension);
                if(storedContentFlexibilityEnabled)
                    this.EnableContentFlexibility();
            } else
                this.SetSizeInternal(this.floatingStateDimensions.width, zoneResizableDimension);
            dimensions = { width: this.GetWidth(), height: zoneResizableDimension };
        }
        else {
            if(this.floatingStateDimensions.width < zoneResizableDimension && !this.heightFixed) {
                if(storedContentFlexibilityEnabled)
                    this.DisableContentFlexibility();
                this.SetSizeInternal(zoneResizableDimension, 1);
                if(storedContentFlexibilityEnabled)
                    this.EnableContentFlexibility();
            } else
                this.SetSizeInternal(zoneResizableDimension, this.floatingStateDimensions.height);
            dimensions = { width: zoneResizableDimension, height: this.GetHeight() };
        }

        if(this.floatingState)
            this.DisableContentFlexibility();

        if(storedDimensions.width !== dimensions.width || storedDimensions.height !== dimensions.height)
            this.SetSizeInternal(storedDimensions.width, storedDimensions.height);

        this.UpdateRestoredWindowSizeUnlock();
        return dimensions;
    },

    StoreFloatingStateDimensions: function() {
        if(this.trackDimensions) {
            this.StoreFloatingStateDimensionsCore(this.GetWidth(), this.GetHeight());
            this.UpdateManagerClientLayoutState();
        }
    },

    StoreFloatingStateDimensionsCore: function(width, height) {
        this.floatingStateDimensions.width = width;
        this.floatingStateDimensions.height = height;
    },

    GetCurrentWindowWidth: function(index) {
        if(!this.widthFixed)
            return ASPx.InvalidDimension;
        if(!this.floatingState)
            return this.floatingStateDimensions.width;
        return ASPxClientPopupControl.prototype.GetCurrentWindowWidth.call(this, index);
    },

    GetCurrentWindowHeight: function(index) {
        if(!this.heightFixed)
            return this.GetDefaultWindowHeight(index);
        if(!this.floatingState)
            return this.floatingStateDimensions.height;
        return ASPxClientPopupControl.prototype.GetCurrentWindowHeight.call(this, index);
    },

    GetDefaultWindowHeight: function(index) {
        var result = null;
        var useDockedDimensionsHeight = !this.dockedDimensionsCalculating && this.IsDocked() && this.CollapseExecuting();
        if(useDockedDimensionsHeight) {
            try {
                this.dockedDimensionsCalculating = true;
                result = this.GetDockingInfo().dimensions.height;
            }
            finally {
                this.dockedDimensionsCalculating = false;
            }
        }
        else
            result = ASPx.InvalidDimension;
        return result;
    },

    ShoulUpdatedRestoredWindowSizeOnCollapse: function(index) {
        var baseValue = ASPxClientPopupControl.prototype.ShoulUpdatedRestoredWindowSizeOnCollapse.call(this, index);
        return baseValue && this.floatingState;
    },

    //Shadow
    SetShadowVisibility: function(visible) {
        this.shadowVisible = visible;
        this.SetShadowVisibilityLite(visible);
    },

    SetShadowVisibilityLite: function(visible) {
        var mainElement = this.GetWindowMainCell(this.GetMainElement());
        var shadowClassName = ASPx.PopupControlCssClasses.ShadowLiteCssClassName;
        if(visible) {
            if(!ASPx.ElementHasCssClass(mainElement, shadowClassName))
                mainElement.className = ASPx.Str.Trim(mainElement.className) + ' ' + shadowClassName;
        } else
            mainElement.className = ASPx.Str.Trim(mainElement.className.replace(shadowClassName, ''));
    },

    //Resizing
    EnableContentFlexibility: function() {
        var contentElement = this.GetContentScrollbarsOwner();
        this.enableContentScrolling = true;
        this.contentFlexibilityEnabled = true;
        contentElement.style.overflow = 'auto';
        contentElement.style.overflowX = 'auto';
        contentElement.style.overflowY = 'auto';
        this.minWidth = null;
        this.minHeight = null;
        this.maxWidth = null;
        this.maxHeight = null;
        this.contentFlexibilityEnabled = true;
    },

    DisableContentFlexibility: function() {
        var contentElement = this.GetContentScrollbarsOwner();
        contentElement.style.overflowX = this.floatingStateContentOverflow.x;
        contentElement.style.overflowY = this.floatingStateContentOverflow.y;
        contentElement.style.overflow = this.floatingStateContentOverflow.both;
        this.enableContentScrolling = this.floatingStateEnableContentScrolling;
        this.minWidth = this.floatingStateDimensions.minWidth;
        this.minHeight = this.floatingStateDimensions.minHeight;
        this.maxWidth = this.floatingStateDimensions.maxWidth;
        this.maxHeight = this.floatingStateDimensions.maxHeight;
        this.contentFlexibilityEnabled = false;
    },

    SetAllowResize: function(allowResize) {
        this.allowResize = allowResize;
        this.SetAllowResizeLite(allowResize);
    },

    SetAllowResizeLite: function(allowResize) {
        var windowElement = this.GetWindowElement(this.DefaultWindowIndex);
        var windowMainCell = this.GetWindowMainCell(windowElement);
        var windowHeader = this.GetWindowHeaderElement(this.DefaultWindowIndex);
        var sizeGrip = this.GetWindowSizeGripLite(this.DefaultWindowIndex);
        var storedHanlder = this.GetWindowElementMouseMoveEventHandler(this.DefaultWindowIndex);

        if(allowResize) {
            ASPx.Attr.RestoreStyleAttribute(windowElement, 'cursor');
            ASPx.Evt.AttachEventToElement(windowElement, 'mousemove', storedHanlder);
        } else {
            ASPx.Attr.ChangeStyleAttribute(windowElement, 'cursor', 'default');
            ASPx.Attr.RemoveStyleAttribute(windowMainCell, 'cursor');
            if(windowHeader)
                windowHeader.style.cursor = 'move';

            ASPx.Evt.DetachEventFromElement(windowElement, 'mousemove', storedHanlder);
        }

        if(sizeGrip)
            ASPx.SetElementDisplay(sizeGrip, allowResize);
    },

    ResizeForDock: function(dockedDimensions) {
        this.SetSizeInternal(dockedDimensions.width, dockedDimensions.height);
    },

    OnResize: function(evt, index, cursor, resizePanel) {
        ASPxClientPopupControl.prototype.OnResize.call(this, evt, index, cursor, resizePanel);

        this.widthFixed = cursor.horizontalDirection == "w" || cursor.horizontalDirection == "e";
        this.heightFixed = cursor.verticalDirection == "n" || cursor.verticalDirection == "s";
    },

    needToHidePinnedOutFromViewPort: function(index) {
        return !this.zone && ASPxClientPopupControl.prototype.needToHidePinnedOutFromViewPort.call(this, index);
    },

    //Positioning
    Fixate: function() {
        var mainElement = this.GetMainElement();
        var offset = {
            x: mainElement.offsetLeft,
            y: mainElement.offsetTop
        };

        mainElement.style.left = offset.x + 'px';
        mainElement.style.top = offset.y + 'px';
        mainElement.style.position = 'absolute';
        this.fixated = true;
    },

    RemoveFixation: function() {
        var mainElement = this.GetMainElement();
        mainElement.style.position = 'static';
        this.fixated = false;
    },

    //Content loading
    EnsureContent: function(windowIndex, isInit) {
        if(this.contentLoadingMode != 'OnDock' && this.contentLoadingMode != 'OnFloating' &&
            this.contentLoadingMode != 'OnDockStateChange' && this.allowEnsureContent) {
            ASPxClientPopupControl.prototype.EnsureContent.call(this, windowIndex, isInit);
        }
    },

    OnCallbackInternal: function(html, windowIndex, isError) {
        ASPxClientPopupControl.prototype.OnCallbackInternal.call(this, html, windowIndex, isError);
        ASPx.SetElementVisibility(this.GetContentContainer(windowIndex), true);
        var instance = this;

        window.setTimeout(function() {
            if(instance.completeSwitchingToFloatingStateOnCallback) {
                instance.CompleteSwitchingToFloatingState();
                instance.completeSwitchingToFloatingStateOnCallback = false;
            }
            instance.StoreFloatingStateDimensions();
        }, 0);
    },

    LoadContent: function() {
        var mainElement = this.GetMainElement();
        if(!mainElement.loading) {
            mainElement.loading = true;
            this.CreateWindowCallback(this.DefaultWindowIndex, this.DefaultWindowIndex);
        }
    },

    //Dragging
    DisableDragging: function() {
        this.allowDragging = false;
        var elements = [
            this.GetWindowHeaderElement(this.DefaultWindowIndex),
            this.GetWindowMainCell(this.GetMainElement())
        ];

        for(var i = 0; i < elements.length; i++) {
            if(elements[i])
                elements[i].style.cursor = 'default';
        }
    },

    OnDragStart: function(evt, index) {
        if(!this.allowDragging || this.animationLocked)
            return;

        this.ApplyZonesAllowedStyle();
        this.ApplyZonesForbiddenStyle();

        var instance = this;
        //HACK: we need this mock because 'awesome' IE browser can't access
        //evt-object outside execution context in which it was created
        var evtMock = ASPx.Browser.IE ? {
            clientX: evt.clientX,
            clientY: evt.clientY
        } : evt;
        window.setTimeout(function() {
            ASPxClientPopupControl.prototype.OnDragStart.call(instance, evtMock, index);
        }, 0);
    },

    OnDrag: function(index, x, y, xClientCorrection, yClientCorrection, evt) {
        var retValue = ASPxClientPopupControl.prototype.OnDrag.call(this, index, x, y, xClientCorrection, yClientCorrection);

        if(!this.inUndockedState && !this.animationLocked) {
            this.trackDimensions = false;

            var cursorPos = {
                x: ASPx.Evt.GetEventX(evt),
                y: ASPx.Evt.GetEventY(evt)
            };

            if(this.zone) {
                this.UndockInternal(true);
                //Q330211
                //HACK: dragging offsets are initialized on DragStart in popup control.
                //But ifpanel placed inside another offset parent this offsets may change.
                //So fordock panel we are reinitializing offsets after undocking.
                ASPxClientPopupControl.prototype.InitDragInfo.call(this, index, evt);
            }

            this.SearchForCurrentZone(cursorPos);

            this.RaiseStartDragging();

            this.inUndockedState = true;
        }

        var popupCollection = ASPx.GetPopupControlCollection();
        var cursorPos = {
            x: x - popupCollection.gragXOffset,
            y: y - popupCollection.gragYOffset
        };
        if(!evt.ctrlKey)
            this.SearchForCurrentZone(cursorPos);
        else if(this.zone) {
            this.zone.HidePanelPlaceholder();
            this.zone = null;
        }
        return retValue;
    },

    SearchForCurrentZone: function(cursorPos) {
        var newZone = null;
        var zoneList = ASPx.DockZoneBag.Get().GetZoneList();
        for(var i = 0; i < zoneList.length; i++) {
            if(zoneList[i].IsCursorInsideZone(cursorPos)) {
                if(this.zone && this.zone.zoneUID === zoneList[i].zoneUID) {
                    this.zone.MovePanelPlaceholder(cursorPos);
                    return;
                }
                newZone = zoneList[i];
                break;
            }
        }
        if(this.zone) {
            this.zone.HidePanelPlaceholder();
            this.zone = null;
        }
        if(newZone) {
            this.zone = newZone;
            this.zone.MovePanelPlaceholder(cursorPos);
            this.zone.ShowPanelPlaceholder(this);
        }
    },

    OnDragStop: function(index) {
        ASPxClientPopupControl.prototype.OnDragStop.call(this, index);

        this.RemoveZonesAllowedStyle();
        this.RemoveZonesForbiddenStyle();

        if(!this.inUndockedState)
            return;

        this.inUndockedState = false;

        this.RaiseEndDragging();

        var dockingInfo = this.GetDockingInfo();

        if(dockingInfo && dockingInfo.canDock && this.RaiseBeforeDock(this.zone)) {
            this.DockInternal(dockingInfo.dimensions);
            return;
        }

        if(this.zone)
            this.zone.HidePanelPlaceholder();

        var lastDockedState = this.GetState().lastDockedState;
        if(this.mode === ASPxClientDockPanelModes.DockedOnly && lastDockedState.zoneUID) {
            this.stateObserver.trackState = false;
            this.DockToLastZone();
            this.stateObserver.trackState = true;
            return;
        }

        if(this.stateObserver.IsBeingDocked() && !this.RaiseBeforeFloat()) {
            this.stateObserver.trackState = false;
            this.DockToLastZone();
            this.stateObserver.trackState = true;
            return;
        }

        this.SwitchToFloatingState();
        this.SetLastFloatState();

        if(this.stateObserver.IsBeingFloated())
            this.RaiseAfterFloat();
    },

    IsZoneForbidden: function(zone) {
        for(var i = 0; i < this.forbiddenZones.length; i++) {
            if(this.forbiddenZones[i] === zone.zoneUID)
                return true;
        }
        return false;
    },

    GetDockingInfo: function() {
        if(!this.zone)
            return null;

        var dockedDimensions = null;
        var canDock = this.mode != ASPxClientDockPanelModes.FloatOnly && !this.freezed && !this.IsZoneForbidden(this.zone);

        if(canDock) {
            if(this.zone.IsFillOrientation()) {
                dockedDimensions = { width: this.zone.initialWidth, height: this.zone.initialHeight };
                canDock &= this.zone.CanDockPanel();
            } else {
                var isHorizontalZone = this.zone.IsHorizontalOrientation();
                var zoneResizableDimension = isHorizontalZone ? this.zone.initialHeight : this.zone.initialWidth;
                dockedDimensions = this.GetDockedDimensions(zoneResizableDimension, isHorizontalZone);
                canDock &= this.zone.CanDockPanel(isHorizontalZone ? dockedDimensions.width : dockedDimensions.height);
            }
        }

        return {
            canDock: canDock,
            dimensions: dockedDimensions
        };
    },

    //Dock states
    SwitchToFloatingState: function() {
        var requireContentUpdate = !this.floatingState &&
            (this.contentLoadingMode === 'OnFloating' || this.contentLoadingMode === 'OnDockStateChange');

        this.zone = null;
        this.floatingState = true;

        if(this.floatingStateAllowResize)
            this.SetAllowResize(true);
        this.DisableContentFlexibility();

        if(requireContentUpdate) {
            this.LoadContent();
            this.completeSwitchingToFloatingStateOnCallback = true;
        } else
            this.CompleteSwitchingToFloatingState();
    },

    CompleteSwitchingToFloatingState: function() {
        this.UpdateRestoredWindowSizeLock();
        this.stateObserver.UpdateState();
        this.SetSizeInternal(this.floatingStateDimensions.width, this.floatingStateDimensions.height);

        this.trackDimensions = true;

        if(!this.GetIsCollapsed())
            this.StoreFloatingStateDimensions();

        if(this.IsVisible())
            this.AdjustContentOnDockStateChanged();

        this.UpdateManagerClientLayoutState();
        this.UpdateRestoredWindowSizeUnlock();
    },

    CompleteDocking: function(mainElement, dockedDimensions, onFirstShow) {
        this.zone.DockPanel(this, dockedDimensions, onFirstShow);
        this.stateObserver.UpdateState();
        mainElement.style.position = 'static';

        var requireRefreshContent = this.stateObserver.IsBeingDocked() &&
            (this.contentLoadingMode === 'OnDock' || this.contentLoadingMode === 'OnDockStateChange');

        if(requireRefreshContent)
            this.LoadContent();

        if(!onFirstShow)
            this.RaiseAfterDock();
        this.UpdateManagerClientLayoutState();

        this.zone.AdjustControlCore();
        if(this.IsVisible())
            this.AdjustContentOnDockStateChanged();

        this.SetLastDockedState({ zoneUID: this.GetZoneUID(), visibleIndex: this.GetVisibleIndex() });

        //NOTE: Fix forIE6 iFrames
        var windowIFrame = this.FindWindowIFrame(this.DefaultWindowIndex);
        if(windowIFrame)
            ASPx.SetElementDisplay(windowIFrame, false);

        if(this.animationLocked)
            this.animationLocked = false;
        this.UpdateRestoredWindowSizeUnlock();//HACK: lock is called in DockInternal method (fix foranimation)
        this.HideNativeScrollbarsOnAndroid();
        if(this.GetCollapsed()) {
            var restoredWindowData = ASPxClientPopupControl.prototype.GetRestoredWindowData.call(this);
            if(restoredWindowData.width && restoredWindowData.height)
                this.StoreFloatingStateDimensionsCore(restoredWindowData.width, restoredWindowData.height);
        }
    },

    HideNativeScrollbarsOnAndroid: function() {
        if(ASPx.Browser.AndroidMobilePlatform) {//B236759
            var contentElement = this.GetContentScrollbarsOwner();
            contentElement.style.overflow = 'hidden';
            contentElement.style.overflowX = 'hidden';
            contentElement.style.overflowY = 'hidden';
        }
    },

    AdjustContentOnDockStateChanged: function() {
        var contentElement = this.GetContentContainer(this.DefaultWindowIndex);
        ASPx.GetControlCollection().AdjustControls(contentElement);
    },

    DockOnFirstShow: function() {
        var zoneUID = this.GetZoneUID();
        if(!zoneUID)
            return;

        this.InitializeZone(); //T157086

        if(this.zone)
            this.zone.AdjustControl();

        this.trackDimensions = false;
        var dockingInfo = this.GetDockingInfo();

        if(dockingInfo && dockingInfo.canDock)
            this.DockInternal(dockingInfo.dimensions, true);
        else
            this.SwitchToFloatingState();
    },

    DockToLastZone: function() {
        var lastDockedState = this.GetState().lastDockedState;

        if(!lastDockedState.zoneUID)
            return;
        var zoneBag = ASPx.DockZoneBag.Get();
        var zone = zoneBag.GetZoneByUID(lastDockedState.zoneUID);
        if(zone)
            this.Dock(zone, lastDockedState.visibleIndex);
    },

    DockInternal: function(dockedDimensions, onFirstShow) {
        this.UpdateRestoredWindowSizeLock();//HACK: unlock is called in CompleteDocking method (fix foranimation)

        var mainElement = this.GetMainElement();
        this.floatingState = false;

        if(this.floatingStateAllowResize)
            this.SetAllowResize(false);

        if(this.initialShadowVisible)
            this.SetShadowVisibility(false);

        this.EnableContentFlexibility();

        if(this.zone.IsHorizontalOrientation())
            ASPx.SetElementFloat(mainElement, 'left');

        if(this.enableAnimation && !onFirstShow)
            this.StartDockAnimation(mainElement, dockedDimensions);
        else
            this.CompleteDocking(mainElement, dockedDimensions, onFirstShow);
    },

    UndockInternal: function(showPlaceholder) {
        var mainElement = this.GetMainElement();
        var position = this.GetUndockToPosition();

        if(showPlaceholder) {
            this.zone.FixatePanels(this);
            this.zone.MovePanelPlaceholderToPanel(this);
        }

        this.initialParentNode.appendChild(mainElement);
        mainElement.style.position = 'absolute';
        ASPx.SetElementFloat(mainElement, 'none');

        this.SetWindowPos(this.DefaultWindowIndex, mainElement, position.x, position.y);
        if(!this.IsVisible()) {
            this.SetWindowLeft(this.DefaultWindowIndex, position.x);
            this.SetWindowTop(this.DefaultWindowIndex, position.y);
        }

        if(this.initialShadowVisible)
            this.SetShadowVisibility(true);
        this.zone.UndockPanel(this);

        if(showPlaceholder)
            this.zone.RemovePanelsFixation();

        //NOTE: Fix forIE6 iFrames
        var windowIFrame = this.FindWindowIFrame(this.DefaultWindowIndex);
        if(windowIFrame)
            ASPx.SetElementDisplay(windowIFrame, true);

        this.zone = null;
    },

    GetUndockToPosition: function() {
        var mainElement = this.GetMainElement();

        if(this.IsVisible())
            return this.GetElementPosInInitialParentNode(mainElement)

        var storedDisplay = mainElement.style.display;
        mainElement.style.display = 'block';

        var position = this.GetElementPosInInitialParentNode(mainElement);

        mainElement.style.display = storedDisplay;

        return position;
    },

    MakeFloatInternal: function(position) {
        if(this.floatingState)
            return;

        this.UndockInternal();
        this.SwitchToFloatingState();

        if(!position)
            position = this.GetDefaultUndockPosition();

        this.SetWindowPos(this.DefaultWindowIndex, this.GetMainElement(), position.x, position.y);

        //SetWindowPos sets zero values forleft and top ifpanel is invisible
        //In that case we should save position by ourselves
        if(!this.IsVisible()) {
            this.SetWindowLeft(this.DefaultWindowIndex, position.x);
            this.SetWindowTop(this.DefaultWindowIndex, position.y);
        }
    },

    GetDefaultUndockPosition: function() {
        var undockToPos = this.GetUndockToPosition();
        return {
            x: undockToPos.x + this.UndockOnDoubleClickOffset,
            y: undockToPos.y + this.UndockOnDoubleClickOffset

        }
    },

    //Animation
    StartDockAnimation: function(mainElement, dockedDimensions) {
        this.animationLocked = true;
        mainElement.animationIterationCount = 0;
        mainElement.dockedDimensions = dockedDimensions;
        mainElement.destPosition = this.zone.GetPanelPlaceholderPositionForElement(mainElement);
        mainElement.isHorizontalZone = this.zone.IsHorizontalOrientation();

        this.IntializeAnimationOffsets(mainElement, dockedDimensions);

        mainElement.animationStart = new Date();
        this.HandleDockAnimation();
    },

    IntializeAnimationOffsets: function(mainElement, dockedDimensions) {
        var intialHeight = this.GetHeight();
        var initialWidth = this.GetWidth();
        var position = this.GetElementPosInInitialParentNode(mainElement);

        mainElement.moveAnimationOffset = {
            horizontal: position.x > mainElement.destPosition.x ? -this.AnimationOffset : this.AnimationOffset,
            vertical: position.y > mainElement.destPosition.y ? -this.AnimationOffset : this.AnimationOffset
        }

        mainElement.resizeAnimationOffset = {
            horizontal: initialWidth > dockedDimensions.width ? -this.AnimationOffset : this.AnimationOffset,
            vertical: intialHeight > dockedDimensions.height ? -this.AnimationOffset : this.AnimationOffset
        };
    },

    GetAnimationState: function(mainElement) {
        var moveIterationCoeff = Math.log(mainElement.animationIterationCount);
        var resizeIterationCoeff = Math.sqrt(mainElement.animationIterationCount);

        var position = this.GetElementPosInInitialParentNode(mainElement);

        var dimensions = {
            width: this.GetWidth(),
            height: this.GetHeight()
        };

        var newPosition = {
            x: position.x + moveIterationCoeff * mainElement.moveAnimationOffset.horizontal,
            y: position.y + moveIterationCoeff * mainElement.moveAnimationOffset.vertical
        };

        var newDimensions = {
            width: dimensions.width + resizeIterationCoeff * mainElement.resizeAnimationOffset.horizontal,
            height: dimensions.height + resizeIterationCoeff * mainElement.resizeAnimationOffset.vertical
        };

        var positionReached = {
            x: mainElement.moveAnimationOffset.horizontal > 0 ?
                newPosition.x >= mainElement.destPosition.x :
                newPosition.x <= mainElement.destPosition.x,
            y: mainElement.moveAnimationOffset.vertical > 0 ?
                newPosition.y >= mainElement.destPosition.y :
                newPosition.y <= mainElement.destPosition.y
        };

        var dimensionReached = {
            width: mainElement.resizeAnimationOffset.horizontal > 0 ?
                newDimensions.width >= mainElement.dockedDimensions.width :
                newDimensions.width <= mainElement.dockedDimensions.width,
            height: mainElement.resizeAnimationOffset.vertical > 0 ?
                newDimensions.height >= mainElement.dockedDimensions.height :
                newDimensions.height <= mainElement.dockedDimensions.height
        };

        return {
            newPosition: newPosition,
            newDimensions: newDimensions,
            positionReached: positionReached,
            dimensionReached: dimensionReached
        };
    },

    HandleDockAnimation: function() {
        var mainElement = this.GetMainElement();

        mainElement.animationIterationCount++;

        var state = this.GetAnimationState(mainElement);

        var finished = (state.positionReached.x && state.positionReached.y && state.dimensionReached.width &&
            state.dimensionReached.height) || (new Date() - mainElement.animationStart > this.MaxAnimationTime);

        if(finished) {
            this.CompleteDocking(mainElement, mainElement.dockedDimensions);
            return;
        }

        this.SetWindowPos(this.DefaultWindowIndex, mainElement,
            state.positionReached.x ? mainElement.destPosition.x : state.newPosition.x,
            state.positionReached.y ? mainElement.destPosition.y : state.newPosition.y);

        this.SetSizeInternal(state.dimensionReached.width ? mainElement.dockedDimensions.width : state.newDimensions.width,
            state.dimensionReached.height ? mainElement.dockedDimensions.height : state.newDimensions.height);

        var instance = this;
        window.setTimeout(function() { instance.HandleDockAnimation(); }, this.AnimationDelay);
    },

    //Callbacks
    ShowLoadingPanel: function(windowIndex) {
        ASPx.SetElementVisibility(this.GetContentContainer(windowIndex), false);
        ASPxClientPopupControl.prototype.ShowLoadingPanel.call(this, windowIndex);
    },

    //Zone highlighting
    ApplyZonesAllowedStyle: function() {
        this.ProcessZones(this.GetAllowedZones(), function(zone) {
            zone.ApplyDockingAllowedStyle()
        });
    },

    RemoveZonesAllowedStyle: function() {
        this.ProcessZones(this.GetAllowedZones(), function(zone) {
            zone.RemoveDockingAllowedStyle()
        });
    },

    ApplyZonesForbiddenStyle: function() {
        this.ProcessZones(this.GetForbiddenZones(), function(zone) {
            zone.ApplyDockingForbiddenStyle()
        });
    },

    RemoveZonesForbiddenStyle: function() {
        this.ProcessZones(this.GetForbiddenZones(), function(zone) {
            zone.RemoveDockingForbiddenStyle()
        });
    },

    ProcessZones: function(zones, action) {
        for(var i = 0; i < zones.length; i++)
            action(zones[i]);
    },

    GetForbiddenZones: function() {
        var forbiddenZones = [];
        var zoneBag = ASPx.DockZoneBag.Get();
        for(var i = 0; i < this.forbiddenZones.length; i++) {
            var zone = zoneBag.GetZoneByUID(this.forbiddenZones[i]);
            if(zone)
                forbiddenZones.push(zone);
        }
        return forbiddenZones;
    },

    GetAllowedZones: function() {
        var zoneCollection = ASPx.DockZoneBag.Get().GetZoneList();
        var allowedZones = [];
        for(var i = 0; i < zoneCollection.length; i++) {
            var zone = zoneCollection[i];
            if(!this.IsZoneForbidden(zone))
                allowedZones.push(zone);
        }
        return allowedZones;
    },

    //Events
    GetBeforeDockPostbackArgs: function(zone) {
        return [
            this.RaiseBeforeDockEventCommand,
            zone.zoneUID,
            zone.GetPanelAfterPlaceholderVisibleIndex() + 1
        ];
    },

    GetBeforeFloatPostbackArgs: function() {
        return [
            this.RaiseBeforeFloatEventCommand,
            this.GetState().lastDockedState.zoneUID
        ];
    },

    GetAfterFloatPostbackArgs: function() {
        return [
            this.RaiseAfterFloatEventCommand,
            this.GetState().lastDockedState.zoneUID
        ];
    },

    RaiseBeforeDock: function(zone) {
        var processOnServer = this.IsServerEventAssigned(this.BeforeDockServerEventName);
        var args = new ASPxClientDockPanelProcessingModeCancelEventArgs(processOnServer, zone);
        if(!this.BeforeDock.IsEmpty())
            this.BeforeDock.FireEvent(this, args);

        if(!args.cancel && args.processOnServer && this.isInitialized) {
            var postbackArgs = this.GetBeforeDockPostbackArgs(zone);
            this.SendPostBack(ASPx.Json.ToJson(postbackArgs));
            return;
        }

        var dockManager = ASPxClientDockManager.Get();

        return !args.cancel && zone.RaiseBeforeDock(this) &&
            (dockManager ? dockManager.RaiseBeforeDock(this, zone) : true);
    },

    RaiseAfterDock: function() {
        var processOnServer = this.IsServerEventAssigned(this.AfterDockServerEventName);
        var args = new ASPxClientProcessingModeEventArgs(processOnServer);
        if(!this.AfterDock.IsEmpty())
            this.AfterDock.FireEvent(this, args);

        if(args.processOnServer && this.isInitialized) {
            this.SendPostBack(ASPx.Json.ToJson([this.RaiseAfterDockEventCommand]));
            return;
        }

        this.zone.RaiseAfterDock(this);

        var dockManager = ASPxClientDockManager.Get();

        if(dockManager)
            dockManager.RaiseAfterDock(this, this.zone);
    },

    RaiseBeforeFloat: function() {
        var processOnServer = this.IsServerEventAssigned(this.BeforeFloatServerEventName);
        var zone = ASPx.DockZoneBag.Get().GetZoneByUID(this.GetState().lastDockedState.zoneUID);
        var args = new ASPxClientDockPanelProcessingModeCancelEventArgs(processOnServer, zone);

        if(!this.BeforeFloat.IsEmpty())
            this.BeforeFloat.FireEvent(this, args);

        if(args.processOnServer && this.isInitialized) {
            var postbackArgs = this.GetBeforeFloatPostbackArgs();
            this.SendPostBack(ASPx.Json.ToJson(postbackArgs));
            return;
        }

        var dockManager = ASPxClientDockManager.Get();

        if(dockManager && !args.cancel)
            return dockManager.RaiseBeforeFloat(this, zone);

        return !args.cancel;
    },

    RaiseAfterFloat: function() {
        var processOnServer = this.IsServerEventAssigned(this.AfterFloatServerEventName);
        var zone = ASPx.DockZoneBag.Get().GetZoneByUID(this.GetState().lastDockedState.zoneUID);
        var args = new ASPxClientDockPanelProcessingModeEventArgs(processOnServer, zone);

        if(!this.AfterFloat.IsEmpty())
            this.AfterFloat.FireEvent(this, args);

        if(args.processOnServer && this.isInitialized) {
            var postbackArgs = this.GetAfterFloatPostbackArgs();
            this.SendPostBack(ASPx.Json.ToJson(postbackArgs));
            return;
        }

        var dockManager = ASPxClientDockManager.Get();

        if(dockManager) {
            dockManager.RaiseAfterFloat(this, zone);
        }
    },

    RaiseStartDragging: function() {
        if(!this.StartDragging.IsEmpty())
            this.StartDragging.FireEvent(this, new ASPxClientEventArgs());

        var dockManager = ASPxClientDockManager.Get();

        if(dockManager)
            dockManager.RaiseStartPanelDragging(this);
    },

    RaiseEndDragging: function() {
        if(!this.EndDragging.IsEmpty())
            this.EndDragging.FireEvent(this, new ASPxClientEventArgs());

        var dockManager = ASPxClientDockManager.Get();

        if(dockManager)
            dockManager.RaiseEndPanelDragging(this);
    },

    RaiseClosing: function(index, closeReason) {
        var dockManager = ASPxClientDockManager.Get();
        var managerCancel = false;

        if(dockManager)
            managerCancel = dockManager.RaisePanelClosing(this);

        if(!this.Closing.IsEmpty())
            return ASPxClientPopupControl.prototype.RaiseClosing.call(this, index, closeReason);

        return managerCancel;
    },

    RaiseCloseUp: function(index, closeReason) {
        ASPxClientPopupControl.prototype.RaiseCloseUp.call(this, index, closeReason);
		this.UpdateManagerClientLayoutState(); //T183006
        var dockManager = ASPxClientDockManager.Get();
        if(dockManager)
            dockManager.RaisePanelCloseUp(this);
    },

    RaisePopUp: function(index) {
        ASPxClientPopupControl.prototype.RaisePopUp.call(this, index);

        var dockManager = ASPxClientDockManager.Get();

        if(dockManager)
            dockManager.RaisePanelPopUp(this);
    },

    RaiseShown: function(index) {
        ASPxClientPopupControl.prototype.RaiseShown.call(this, index);

        var dockManager = ASPxClientDockManager.Get();

        if(dockManager)
            dockManager.RaisePanelShown(this);
    },

    RaiseResize: function(index, resizeState) {
        ASPxClientPopupControl.prototype.RaiseResize.call(this, index, resizeState);

        var dockManager = ASPxClientDockManager.Get();

        if(dockManager)
            dockManager.RaisePanelResize(this);
    },

    //API - Visibility
    DoShowWindowAtPos: function(index, x, y, ignorePopupElement, closeOtherWindows, allowChangeZIndex, closeOtherReason) {
        if(this.floatingState) {
            ASPxClientPopupControl.prototype.DoShowWindowAtPos.call(this, index, x, y, ignorePopupElement,
                false, allowChangeZIndex, closeOtherReason);
            this.StoreInitialSettings();
            return;
        }

        var visible = ASPxClientPopupControl.prototype.InternalIsWindowVisible.call(this, this.DefaultWindowIndex);
        if(visible)
            return;

        this.RaisePopUp(this.DefaultWindowIndex);

        var mainElement = this.GetMainElement();
        mainElement.style.display = this.storedMainElementDisplay;
        ASPx.SetElementVisibility(mainElement, true);

        this.AdjustContentOnShow(index);
        this.ApplyPanelCachedSize(index);

        if(this.zone) {
            this.zone.UpdatePanelsVisibleIndices();
            this.zone.ApplyPanelSpacing();
            this.zone.CorrectResizableDimensionCore();
        }

        this.registerAndActivateWindow(mainElement, index, allowChangeZIndex);

        ASPxClientPopupControl.prototype.UpdateWindowsStateCookie.call(this);

        ASPxClientPopupControl.prototype.OnWindowShown.call(this, this.DefaultWindowIndex);
    },

    ApplyPanelCachedSize: function() {
        var cachedSize = this.GetWindowCachedSize(this.DefaultWindowIndex);
        if(cachedSize != null) {
            this.SetWindowSizeInternal(this.GetWindow(this.DefaultWindowIndex), cachedSize.width, cachedSize.height);
            this.ResetWindowCachedSize(this.DefaultWindowIndex);
        }
    },

    DoHideWindowCore: function(index) {
        this.MakeFloatBeforeHideInZoneWithFillOrientation();
        var mainElement = this.GetMainElement();
        this.storedMainElementDisplay = mainElement.style.display;

        ASPxClientPopupControl.prototype.DoHideWindowCore.call(this, index);
        if(this.zone) {
            this.zone.UpdatePanelsVisibleIndices();
            this.zone.ApplyPanelSpacing();
            this.zone.CorrectResizableDimensionCore();
        }
    },

    MakeFloatBeforeHideInZoneWithFillOrientation: function() {
        if(this.zone && this.zone.IsFillOrientation() && !this.floatingState)
            this.MakeFloatInternal();
    },

    DoCollapse: function(index, minimization) {
        ASPxClientPopupControl.prototype.DoCollapse.call(this, index, minimization);
        if(this.zone)
            this.zone.CorrectResizableDimension();
    },

    //API - Size
    SetWindowSize: function(window, width, height) {
        if(this.floatingState) {
            this.SetWindowSizeInternal(window, width, height);
            if(!this.IsVisible()) {
                this.StoreFloatingStateDimensionsCore(width, height);
                this.UpdateManagerClientLayoutState();
            }

            return;
        }

        this.StoreFloatingStateDimensionsCore(width, height);
        this.UpdateManagerClientLayoutState();
    },

    SetSize: function(width, height) {
        ASPxClientPopupControl.prototype.SetSize.call(this, width, height);
        this.widthFixed = true;
        this.heightFixed = true;
    },

    SetWidth: function(width) {
        ASPxClientPopupControl.prototype.SetWidth.call(this, width);
        this.widthFixed = true;
    },

    SetHeight: function(height) {
        ASPxClientPopupControl.prototype.SetHeight.call(this, height);
        this.heightFixed = true;
    },

    SetSizeInternal: function(width, height) {
        this.SetWindowSizeInternal(null, width, height);
    },

    SetHeightInternal: function(height) {
        this.SetSizeInternal(this.GetWidth(), height);
    },

    SetWidthInternal: function(width) {
        this.SetSizeInternal(width, this.GetHeight());
    },

    SetWindowSizeInternal: function(window, width, height) {
        ASPxClientPopupControl.prototype.SetWindowSize.call(this, window, width, height);
    },

    //API - Docking
    GetOwnerZone: function() {
        if(!this.zone && !this.firstShowProcessed)
            this.InitializeZone();
        return this.zone;
    },
    Dock: function(zone, visibleIndex) {
        var destinationZoneExists = zone && zone.zoneUID;

        if(!destinationZoneExists || this.IsDockInSamePlace(zone, visibleIndex))
            return;

        if(this.IsDockInSameZone(zone)) {
            this.SetVisibleIndex(visibleIndex);
            return;
        }

        if(this.zone)
            this.MakeFloatInternal();

        this.zone = zone;
        this.trackDimensions = false;

        var dockingInfo = this.GetDockingInfo();
        var canDock = dockingInfo && dockingInfo.canDock;

        if(!canDock) {
            this.zone = null;
            return;
        }

        this.SetVisibleIndexCore(ASPx.IsExists(visibleIndex) ?
            visibleIndex : this.zone.GetDockedPanelsMaxVisibleIndex() + 1);
        this.DockInternal(dockingInfo.dimensions, true);
        this.zone.UpdatePanelsVisibleIndices();
    },
    MakeFloat: function(x, y) {
        if(ASPx.IsExists(x) && ASPx.IsExists(y))
            this.MakeFloatInternal({ x: x, y: y });
        else
            this.MakeFloatInternal();
    },
    GetVisibleIndex: function() {
        return this.GetState().visibleIndex;
    },
    SetVisibleIndex: function(visibleIndex) {
        if(this.floatingState) {
            this.SetVisibleIndexCore(visibleIndex);
            return;
        }

        this.zone.GetMainElement().insertBefore(this.GetMainElement(), this.GetInsertBeforePanelNode(visibleIndex));
        this.zone.UpdatePanelsVisibleIndices();
        this.zone.ApplyPanelSpacing();
    },
    IsDocked: function() {
        return !!this.zone;
    },

    //Misc
    SetVisibleIndexCore: function(visibleIndex) {
        var state = this.GetState();
        this.dockState = [state.zoneUID, visibleIndex, state.lastDockedState.zoneUID, state.lastDockedState.visibleIndex, state.lastFloatState.left, state.lastFloatState.top];
    },

    IsDockInSameZone: function(destinationZone) {
        return this.zone && this.zone.zoneUID === destinationZone.zoneUID;
    },

    IsDockInSamePlace: function(destinationZone, visibleIndex) {
        if(!this.IsDockInSameZone(destinationZone))
            return false;

        var dockedPanels = this.zone.GetOrderedPanelsList();
        return ASPx.IsExists(visibleIndex) ?
            this.GetVisibleIndex() === visibleIndex : dockedPanels[dockedPanels.length - 1].panelUID === this.panelUID;
    },

    GetInsertBeforePanelNode: function(visibleIndex) {
        var panels = this.zone.GetOrderedPanelsList();
        var result = [];

        for(var i = 0; i < panels.length; i++) {
            if(panels[i].panelUID !== this.panelUID && panels[i].IsVisible())
                result.push(panels[i]);
        }

        var insertBeforePanel = result[visibleIndex];
        return insertBeforePanel ? insertBeforePanel.GetMainElement() : null;
    },

    GetElementPosInInitialParentNode: function(element) {
        return {
            x: ASPx.PrepareClientPosElementForOtherParent(ASPx.GetAbsoluteX(element), element, this.initialParentNode, true),
            y: ASPx.PrepareClientPosElementForOtherParent(ASPx.GetAbsoluteY(element), element, this.initialParentNode, false)
        };
    },
    OnMaximizeButtonClick: function(index) {
        var maximizing = !this.GetIsMaximized(index);
        if(maximizing) {
            this.dockRestorePanelData = { zone: this.GetOwnerZone() };
            if(this.dockRestorePanelData.zone) {
                this.dockRestorePanelData.visibleIndex = this.GetVisibleIndex();
                this.MakeFloat();
            }
        }
        ASPxClientPopupControl.prototype.OnMaximizeButtonClick.call(this, index);
        if(!maximizing) {
            if(this.dockRestorePanelData && this.dockRestorePanelData.zone)
                this.Dock(this.dockRestorePanelData.zone, this.dockRestorePanelData.visibleIndex);
            this.dockRestorePanelData = null;
        }
    },
    GetRestoredWindowData: function(index) {
        var restoredWindowData = ASPxClientPopupControl.prototype.GetRestoredWindowData.call(this, index);
        if(this.zone) {
            restoredWindowData.width = this.UseZoneSize(true) ? ASPx.GetClearClientWidth(this.zone.GetMainElement()) : this.GetCurrentWindowWidth(index);
            restoredWindowData.height = this.UseZoneSize(false) ? ASPx.GetClearClientHeight(this.zone.GetMainElement()) : this.GetCurrentWindowHeight(index);
        }
        return restoredWindowData;
    },
    UseZoneSize: function(isWidth) {
        return this.zone.IsFillOrientation() || isWidth && !this.zone.IsHorizontalOrientation() || !isWidth && this.zone.IsHorizontalOrientation();
    }
});
ASPxClientDockPanel.Cast = ASPxClientControl.Cast;
var ASPxClientDockPanelProcessingModeCancelEventArgs = ASPx.CreateClass(ASPxClientProcessingModeCancelEventArgs, {
    constructor: function(processOnServer, zone) {
        this.constructor.prototype.constructor.call(this, processOnServer);
        this.zone = zone;
    }
});
var ASPxClientDockPanelProcessingModeEventArgs = ASPx.CreateClass(ASPxClientProcessingModeEventArgs, {
    constructor: function(processOnServer, zone) {
        this.constructor.prototype.constructor.call(this, processOnServer);
        this.zone = zone;
    }
});

window.ASPxClientDockPanelModes = ASPxClientDockPanelModes;
window.ASPxClientDockPanel = ASPxClientDockPanel;
window.ASPxClientDockPanelProcessingModeCancelEventArgs = ASPxClientDockPanelProcessingModeCancelEventArgs;
window.ASPxClientDockPanelProcessingModeEventArgs = ASPxClientDockPanelProcessingModeEventArgs;
})();