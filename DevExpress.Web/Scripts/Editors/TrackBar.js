/// <reference path="..\_references.js"/>

(function () {
    // Constant objects
    TrackBarConsts = {
        BOTH_SCALE_POSITION_SYSTEM_CLASS_NAME: "dxeTBBScaleSys",
        CONTERNT_CONTAINER_SYSTEM_CLASS_NAME: "dxeTBContentContainerSys",
        DEC_BUTTON_ID: "_DB",
        DRAG_HANDLE_CHANGE_SPEED: 50,
        FIRST_ITEM_SYSTEM_CLASS_NAME: "dxeFItemSys",
        FOCUSED_MD_SYSTEM_CLASS_NAME: "dxeFocusedMDHSys",
        FOCUSED_SD_SYSTEM_CLASS_NAME: "dxeFocusedSDHSys",
        HORIZONTAL_ORIENTATION_SYSTEM_CLASS_NAME: "dxeTBHSys",
        INC_BUTTON_ID: "_IB",
        ITEM_SYSTEM_CLASS_NAME: "dxeTBItemSys",
        LAST_ITEM_SYSTEM_CLASS_NAME: "dxeLItemSys",
        LEFT_TOP_SCALE_POSITION_SYSTEM_CLASS_NAME: "dxeTBLTScaleSys",
        MAIN_DRAG_HANDLE_ID: "_MD",
        MASS_INC_DEC_DELAY: 300,
        REVERSED_DIRECTION_SYSTEM_CLASS_NAME: "dxeReversedDirectionSys",
        RIGHT_BOTTOM_SCALE_POSITION_SYSTEM_CLASS_NAME: "dxeTBRBScaleSys",
        SCALE_SYSTEM_CLASS_NAME: "dxeTBScaleSys",
        SECONDARY_DRAG_HANDLE_ID: "_SD",
        TICK_ELEMENT_POSTFIX: "_TK",
        VALUE_TOOLTIP_ZINDEX: 41998,
        VERTICAL_ORIENTATION_SYSTEM_CLASS_NAME: "dxeTBVSys"
    }

    ASPxClientTrackBarAnimationsConsts = {
        INC_DEC_ACCELERATOR_STEP: 0.2,
        TOOLTIP_ANIMATION_SPEED: 50,
        TOOLTIP_ANIMATION_QUALITY: 0.1,
        TRACK_ANIMATION_SPEED: 30,
        TRACK_ANIMATION_QUALITY: 0.7
    };

    // Enums
    var ASPxClientTrackBarPosition = {
        Both: "Both",
        LeftOrTop: "LeftOrTop",
        None: "None",
        RightOrBottom: "RightOrBottom"
    };

    var ASPxClientTrackBarDirection = {
        Normal: "Normal",
        Reversed: "Reversed"
    };

    var ASPxClientTrackHighlightMode = {
        AlongBarHighlight: "AlongBarHighlight",
        HandlePosition: "HandlePosition",
        None: "None"
    };

    ASPxClientDragHandleDisplayMode = {
        OutsideTrack: "OutsideTrack",
        InsideTrack: "InsideTrack"
    };
    var ASPxClientTrackBar = ASPx.CreateClass(ASPxClientEdit, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);

            // Server-Provided Field
            this.animationEnabled = true;
            this.appearValueToolTip = true;
            this.direction = ASPxClientTrackBarDirection.Normal;
            this.enableMouseWheel = true;
            this.isHorizontal = true;
            this.items = null;
            this.scaleLabelFormatString = "{0}";
            this.scaleLabelHighlightMode = ASPxClientTrackHighlightMode.None;
            this.scalePosition = ASPxClientTrackBarPosition.RightOrBottom;
            this.selectedClasses = [''];
            this.selectedCssArray = [''];
            this.showDragHandles = true;
            this.smallTickFrequency = 0;
            this.step = 1;
            this.largeTickInterval = 0;
            this.largeTickStartValue = null;
            this.largeTickEndValue = null;
            this.maxValue = 100;
            this.minValue = 0;
            this.valueChangedDelay = 0;
            this.valueToolTipFormat = '';
            this.valueToolTipPosition = ASPxClientTrackBarPosition.LeftOrTop;
            this.valueToolTipStyle = ['', ''];

            // Private fields
            this.allowRangeSelection = false;
            this.barHighlightStartPos = 0;
            this.behaviorStrategy = null;
            this.dragHandleDisplayMode = ASPxClientDragHandleDisplayMode.OutsideTrack;
            this.incDecAccelerator = 1;
            this.incDecInterval = null;
            this.incDecTimer = null;
            this.isAdjusted = false;
            this.isASPxTrackBar = true;
            this.isButtonPressed = false;
            this.isButtonsExist = true;
            this.isFirstMoveAction = true;
            this.isMainDragHandleFocused = true;
            this.isNormalDirection = true;
            this.isOutsideDragHandleDisplayMode = true;
            this.heldCallArray = [];
            this.movedElement = null;
            this.preventNextBarHighlightMouseClick = false;
            this.preventNextTrackMouseClick = false;
            this.preventNextScaleMouseClick = false;
            this.scaleMap = null;
            this.tickValue = {};
            this.valueChangedDelayTimer = null;

            this.sizingConfig.adjustControl = true;

            // Events
            this.PositionChanging = new ASPxClientEvent();
            this.PositionChanged = new ASPxClientEvent();
            this.Track = new ASPxClientEvent();
            this.TrackStart = new ASPxClientEvent();
            this.TrackEnd = new ASPxClientEvent();
        },

        InlineInitialize: function () {
            ASPxClientEdit.prototype.InlineInitialize.call(this);

            this.filedsInitialize();
            this.applySystemCssClasses();

            if(this.getScaleElement()) {
                this.behaviorStrategy.buildScale();
                this.setTickElementIDs();
                this.generateStateItems();
            }

            if(this.enabled) {
                this.assignElementIDs();
                this.attachToEvents();
            }

            if(!this.clientEnabled)
                this.changeEnabledStateItems(false);
        },

        Initialize: function () {
            ASPxClientEdit.prototype.Initialize.call(this);
        },

        applySystemCssClasses: function () {
            var contentContainerElement = this.getContentContainer();
            var directionClassName = this.isNormalDirection ? "" : TrackBarConsts.REVERSED_DIRECTION_SYSTEM_CLASS_NAME;
            var orientationClassName = this.isHorizontal ? TrackBarConsts.HORIZONTAL_ORIENTATION_SYSTEM_CLASS_NAME :
                TrackBarConsts.VERTICAL_ORIENTATION_SYSTEM_CLASS_NAME;
            var scalePositionClassName = "";
            switch (this.scalePosition) {
                case ASPxClientTrackBarPosition.Both:
                    scalePositionClassName = TrackBarConsts.BOTH_SCALE_POSITION_SYSTEM_CLASS_NAME;
                    break;
                case ASPxClientTrackBarPosition.LeftOrTop:
                    scalePositionClassName = TrackBarConsts.LEFT_TOP_SCALE_POSITION_SYSTEM_CLASS_NAME;
                    break;
                case ASPxClientTrackBarPosition.RightOrBottom:
                    scalePositionClassName = TrackBarConsts.RIGHT_BOTTOM_SCALE_POSITION_SYSTEM_CLASS_NAME;
            }

            var resultClass = directionClassName + " " + orientationClassName + " " + scalePositionClassName;
            contentContainerElement.className = resultClass + " " + TrackBarConsts.CONTERNT_CONTAINER_SYSTEM_CLASS_NAME;
            this.GetMainElement().className += " " + resultClass;
        
            var scaleElement = this.getScaleElement();
            if(scaleElement)
                scaleElement.className += " " + scalePositionClassName + " " + TrackBarConsts.SCALE_SYSTEM_CLASS_NAME;

            if(this.GetItemCount() > 0 && this.scalePosition !== ASPxClientTrackBarPosition.None) {
                var itemCollection = this.getTickCollection();
                if(itemCollection.length > 0)
                    itemCollection[0].className += " " + TrackBarConsts.ITEM_SYSTEM_CLASS_NAME;
            }

            if(ASPx.Browser.MSTouchUI) {
                var draggableElements = [this.getMainDragHandleElement(), this.getSecondaryDragHandleElement(), this.getBarHighlightElement()];
                for(var i in draggableElements){
                    if(draggableElements[i])
                        draggableElements[i].className += " " + ASPx.TouchUIHelper.msTouchDraggableClassName;    
                }
            }
        },

        assignElementIDs: function () {
            var name = this.name;
            var assignElementIDFunc = function (element, id) {
                if(element)
                    element.id = name + id;
            };

            assignElementIDFunc(this.getButtonElement(true), TrackBarConsts.DEC_BUTTON_ID);
            assignElementIDFunc(this.getButtonElement(false), TrackBarConsts.INC_BUTTON_ID);
            assignElementIDFunc(this.getMainDragHandleElement(), TrackBarConsts.MAIN_DRAG_HANDLE_ID);
            assignElementIDFunc(this.getSecondaryDragHandleElement(), TrackBarConsts.SECONDARY_DRAG_HANDLE_ID);
        },

        filedsInitialize: function () {
            this.allowRangeSelection = !!this.getSecondaryDragHandleElement();

            this.isButtonsExist = this.isButtonElementsExist();

            if(!this.valueToolTipFormat)
                this.valueToolTipFormat = this.allowRangeSelection ? "{0}..{1}" : "{0}";

            this.isNormalDirection = this.direction === ASPxClientTrackBarDirection.Normal;

            var largeTickValue = this.largeTickInterval,
                smallTickValue = this.smallTickFrequency === 0 ? largeTickValue : largeTickValue / this.smallTickFrequency;
            this.tickValue = {
                small: smallTickValue,
                large: largeTickValue
            };

            this.isOutsideDragHandleDisplayMode =
				this.dragHandleDisplayMode === ASPxClientDragHandleDisplayMode.OutsideTrack;

            this.behaviorStrategy = !!this.items ? new ASPxTrackBarItemModeStrategy(this) :
				new ASPxTrackBarTickModeStrategy(this);
            this.behaviorStrategy.filedsInitialize();
        },

        generateStateItems: function () {
            var tickElements = this.getTickCollection(),
                stateController = ASPx.GetStateController();
            var name = this.name + TrackBarConsts.TICK_ELEMENT_POSTFIX;
            for(var i = 0; i < tickElements.length; i++)
                stateController.AddSelectedItem(name, this.selectedClasses, this.selectedCssArray, [i], null, null);
        },

        setTickElementIDs: function () {
            var tickElements = this.getTickCollection();
            for(var i = 0; i < tickElements.length; i++)
                tickElements[i].id = this.name + TrackBarConsts.TICK_ELEMENT_POSTFIX + i;
        },

        updateScalePosition: function () {
            var scaleElement = this.getScaleElement();
            this.setElementIdent(scaleElement, this.cache.decButtonSize + this.cache.trackElementBorders / 2);
        },

        updateDragHandlesVisibility: function () {
            if(!this.showDragHandles) {
                ASPx.SetElementVisibility(this.getMainDragHandleElement(), false);
                var secondaryDragHandleElement = this.getSecondaryDragHandleElement();
                if(secondaryDragHandleElement)
                    ASPx.SetElementVisibility(secondaryDragHandleElement, false);
            }
        },

        updateTrackPosition: function () {
            var trackElement = this.getTrackElement();
            var trackElementIdent = this.cache.decButtonSize;
            if(this.isOutsideDragHandleDisplayMode)
                trackElementIdent += Math.floor(this.cache.dragHandleElementSizeHalf);
            this.setElementIdent(trackElement, trackElementIdent);
        },

        /* Held Calls */
        addHeldCall: function (methodName, args) {
            args[0] = ASPx.CloneObject(args[0]); // IE7, IE6 bug
            this.heldCallArray.push({
                name: methodName,
                args: args
            });
        },

        executeHeldCalls: function () {
            for(var i = 0; i < this.heldCallArray.length; i++) {
                var methodName = this.heldCallArray[i].name;
                var args = this.heldCallArray[i].args;
                this[methodName].apply(this, args);
            }
            this.clearHeldCalls();
        },

        clearHeldCalls: function () {
            this.heldCallArray = [];
        },

        /* Attach To Events */
        attachToEvents: function () {
            var mainDragHandleElement = this.getMainDragHandleElement(),
                secondaryDragHandleElement = this.getSecondaryDragHandleElement(),
                scaleElement = this.getScaleElement(),
                barHighlightElement = this.getBarHighlightElement(),
                incButtonElement = this.getButtonElement(false),
                decButtonElement = this.getButtonElement(true);

            if(mainDragHandleElement)
                ASPx.Evt.AttachEventToElement(mainDragHandleElement, ASPx.TouchUIHelper.touchMouseDownEventName, function (evt) {
                    this.startEvtHandler("onDragHandleMouseDown", [evt]);
                } .aspxBind(this));

            if(this.allowRangeSelection) {
                ASPx.Evt.AttachEventToElement(barHighlightElement, ASPx.TouchUIHelper.touchMouseDownEventName, function (evt) {
                    this.startEvtHandler("onBarHighlightMouseDown", [evt]);
                } .aspxBind(this));
                if(secondaryDragHandleElement)
                    ASPx.Evt.AttachEventToElement(secondaryDragHandleElement, ASPx.TouchUIHelper.touchMouseDownEventName, function (evt) {
                        this.startEvtHandler("onDragHandleMouseDown", [evt]);
                    } .aspxBind(this));
            }
            if(scaleElement) {
                ASPx.Evt.AttachEventToElement(scaleElement, "click", function (evt) {
                    this.startEvtHandler("onScaleClick", [evt]);
                } .aspxBind(this));
            }

            if(incButtonElement && decButtonElement) {
                ASPx.Evt.AttachEventToElement(decButtonElement, ASPx.TouchUIHelper.touchMouseDownEventName, function (evt) {
                    this.onButtonDown(true);
                    if(ASPx.TouchUIHelper.isTouchEvent(evt))
                        evt.preventDefault();
                } .aspxBind(this));
                ASPx.Evt.AttachEventToElement(incButtonElement, ASPx.TouchUIHelper.touchMouseDownEventName, function (evt) {
                    this.onButtonDown(false);
                    if(ASPx.TouchUIHelper.isTouchEvent(evt))
                        evt.preventDefault();
                } .aspxBind(this));
            }

            this.attachToCancelDragEvents();
            this.attachToDocumentEvents();
            this.attachToImperativeEvents();
            this.attachToKBSEvents();
            if(mainDragHandleElement)
                this.attachToStateControllerEvents(mainDragHandleElement, secondaryDragHandleElement);
        },

        attachToCancelDragEvents: function () {
            var mainDragHandleElement = this.getMainDragHandleElement(),
                secondaryDragHandleElement = this.getSecondaryDragHandleElement(),
                barHighlightElement = this.getBarHighlightElement();

            if(mainDragHandleElement)
                ASPx.Evt.PreventElementDragAndSelect(mainDragHandleElement, true);
            if(secondaryDragHandleElement)
                ASPx.Evt.PreventElementDragAndSelect(secondaryDragHandleElement, true);
            ASPx.Evt.PreventElementDragAndSelect(barHighlightElement, true);
            if(!ASPx.Browser.IE || ASPx.Browser.Version >= 9) {
                if(mainDragHandleElement)
                    ASPx.Evt.AttachEventToElement(mainDragHandleElement, "dragstart", ASPx.Evt.PreventEvent);
                if(secondaryDragHandleElement)
                    ASPx.Evt.AttachEventToElement(secondaryDragHandleElement, "dragstart", ASPx.Evt.PreventEvent);
            }

            if(ASPx.Browser.Chrome) {
                ASPx.Evt.AttachEventToDocument("selectstart", function (evt) {
                    if(this.getMovedDragHandle())
                        return ASPx.Evt.PreventEventAndBubble(evt);
                } .aspxBind(this));
            }
        },

        attachToDocumentEvents: function () {
            ASPx.Evt.AttachEventToDocument(ASPx.TouchUIHelper.touchMouseUpEventName, function (evt) {
                this.startEvtHandler("onMouseUp", [evt]);
            } .aspxBind(this));
            ASPx.Evt.AttachEventToDocument(ASPx.TouchUIHelper.touchMouseMoveEventName, function (evt) {
                this.onMouseMove(evt);
            } .aspxBind(this));
        },

        attachToImperativeEvents: function () {
            var mainElement = this.GetMainElement(),
                trackElement = this.getTrackElement(),
                barHighlightElement = this.getBarHighlightElement();

            ASPx.Evt.AttachEventToElement(mainElement, ASPx.TouchUIHelper.touchMouseDownEventName, function (evt) {
                this.SetFocus();
            } .aspxBind(this));
            ASPx.Evt.AttachEventToElement(trackElement, "click", function (evt) {
                this.startEvtHandler("onTrackClick", [evt]);
            } .aspxBind(this));
            ASPx.Evt.AttachEventToElement(barHighlightElement, "click", function (evt) {
                this.startEvtHandler("onBarHighlightClick", [evt]);
                return ASPx.Evt.CancelBubble(evt);
            } .aspxBind(this));
        },

        attachToKBSEvents: function () {
            var inputElement = this.GetInputElement();
            ASPx.Evt.AttachEventToElement(inputElement, "keydown", function (evt) {
                this.onKeyDown(evt);
            } .aspxBind(this));
            ASPx.Evt.AttachEventToElement(inputElement, "keyup", function (evt) {
                this.onKeyUp(evt);
            } .aspxBind(this));
            ASPx.Evt.AttachEventToElement(inputElement, "focus", function (evt) {
                this.OnFocus();
            } .aspxBind(this));
            ASPx.Evt.AttachEventToElement(inputElement, "blur", function (evt) {
                this.OnLostFocus();
            } .aspxBind(this));
        },

        attachToStateControllerEvents: function (mainDragHandleElement, secondaryDragHandleElement) {
            var beforeHandler = function (s, e) {
                if(e.element === mainDragHandleElement ||
                    (secondaryDragHandleElement && e.element === secondaryDragHandleElement)) {
                    this.updateFocusedDragHandleClass();
                }
            } .aspxBind(this);
            ASPx.GetStateController().BeforeSetHoverState.AddHandler(beforeHandler);
            ASPx.GetStateController().BeforeSetPressedState.AddHandler(beforeHandler);
        },

        startEvtHandler: function (handlerName, args) {
            if(this.GetEnabled() && !this.readOnly) {
                if(!this.focused)
                    this.addHeldCall(handlerName, args);
                else
                    this[handlerName].apply(this, args);
            }
        },

        /* Timer */
        startIncDecTimer: function (increment) {
            this.stopIncDecTimer();
            this.incDecAccelerator = 1;
            this.incDecTimer = window.setTimeout(function () {
                if(this.isButtonPressed) {
                    this.incDecInterval = window.setInterval(function () {

                        var thisControlIsNotActual = this !== ASPx.GetControlCollection().Get(this.name);
                        if(thisControlIsNotActual) {
                            this.stopIncDecTimer();
                            return;
                        }

                        this.incDecAccelerator += this.behaviorStrategy.getIncDecAcceleratorStep();
                        var step = this.incDecAccelerator * this.step * (increment ? 1 : -1);
                        this.incrementValueInternal(step, true);
                        this.showValueToolTip(true);
                    } .aspxBind(this), TrackBarConsts.DRAG_HANDLE_CHANGE_SPEED);
                }
            } .aspxBind(this), TrackBarConsts.MASS_INC_DEC_DELAY);
        },

        stopIncDecTimer: function () {
            clearInterval(this.incDecInterval);
            clearTimeout(this.incDecTimer);
            this.incDecTimer = this.incDecInterval = null;
        },

        /* Internal cache */
        clearInternalCache: function () {
            this.cache = null;
        },

        isInternaCacheInitialized: function () {
            return !!this.cache;
        },

        updateInternalCache: function (baseParams, trackParams) {
            if(!ASPx.IsElementDisplayed(this.GetMainElement()))
                return;
            if(!this.isInternaCacheInitialized())
                this.cache = {};
            if(baseParams) {
                var dragHandleElement = this.getMainDragHandleElement(),
                    contentContainer = this.getContentContainer(),
                    mainElement = this.GetMainElement(),
                    decButton = this.getButtonElement(true),
                    incButton = this.getButtonElement(false),
                    barHighlightElement = this.getBarHighlightElement();

                contentContainer.style.width = "0px";
                contentContainer.style.height = "0px";

                this.cache.mainElementWidth = mainElement.offsetWidth - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(mainElement);
                this.cache.mainElementHeight = mainElement.offsetHeight - ASPx.GetTopBottomBordersAndPaddingsSummaryValue(mainElement);
                this.cache.mainElementSize = this.isHorizontal ? this.cache.mainElementWidth : this.cache.mainElementHeight;
                this.cache.mainElementReversedSize = this.isHorizontal ? this.cache.mainElementHeight : this.cache.mainElementWidth;

                this.cache.barHighlightElementBorders = this.isHorizontal ? ASPx.GetLeftRightBordersAndPaddingsSummaryValue(barHighlightElement) :
                    ASPx.GetTopBottomBordersAndPaddingsSummaryValue(barHighlightElement);

                if(dragHandleElement) {
                    this.cache.dragHandleElementWidth = dragHandleElement.offsetWidth;
                    this.cache.dragHandleElementHeight = dragHandleElement.offsetHeight;
                    this.cache.dragHandleElementSize = this.isHorizontal ? this.cache.dragHandleElementWidth : this.cache.dragHandleElementHeight;
                    this.cache.dragHandleElementSizeHalf = this.cache.dragHandleElementSize / 2;
                }

                this.cache.decButtonSize = decButton ? this.getElementSize(decButton) + this.getElementMargins(decButton) : 0;
                this.cache.incButtonSize = incButton ? this.getElementSize(incButton) + this.getElementMargins(incButton) : 0;
                this.cache.buttonSizesAmount = this.cache.decButtonSize + this.cache.incButtonSize;
            }
            if(trackParams) {
                var trackElement = this.getTrackElement();
                this.cache.trackElementBorders = this.isHorizontal ? ASPx.GetLeftRightBordersAndPaddingsSummaryValue(trackElement) :
                    ASPx.GetTopBottomBordersAndPaddingsSummaryValue(trackElement);
                this.cache.trackElementSize = this.getElementSize(trackElement) - this.cache.trackElementBorders;
                this.cache.scaleElementSize = this.cache.trackElementSize +
                    (this.isOutsideDragHandleDisplayMode ? this.cache.dragHandleElementSize - 1 : 0);
            }
        },

        /* Internal Events */
        isPostBackAllowed: function () {
            return this.autoPostBack && !this.getMovedDragHandle();
        },

        RaiseFocus: function () {
            ASPxClientEdit.prototype.RaiseFocus.call(this);
            this.executeHeldCalls();
        },

        raisePositionChanged: function (processOnServer) {
            if(!this.PositionChanged.IsEmpty()) {
                var args = new ASPxClientProcessingModeEventArgs(processOnServer);
                this.PositionChanged.FireEvent(this, args);
                processOnServer = args.processOnServer;
            }
            return processOnServer;
        },

        risePositionChanging: function (currentPositionStart, currentPositionEnd, newPositionStart, newPositionEnd) {
            if(!this.PositionChanging.IsEmpty()) {
                var args = new ASPxClientTrackBarPositionChangingEventArgs(currentPositionStart,
                    currentPositionEnd, newPositionStart, newPositionEnd);
                this.PositionChanging.FireEvent(this, args);
                return args.cancel;
            }

            return false;
        },

        riseTrack: function () {
            if(!this.Track.IsEmpty()) {
                var args = new ASPxClientEventArgs();
                this.Track.FireEvent(this, args);
            }
        },

        riseTrackStart: function () {
            if(!this.TrackStart.IsEmpty()) {
                var args = new ASPxClientEventArgs();
                this.TrackStart.FireEvent(this, args);
            }
        },

        riseTrackEnd: function () {
            if(!this.TrackEnd.IsEmpty()) {
                var args = new ASPxClientEventArgs();
                this.TrackEnd.FireEvent(this, args);
            }
            if(this.autoPostBack)
                this.SendPostBackInternal("");
        },

        RaiseValueChangedEvent: function () {
            if(!this.isInitialized) return false;
            var processOnServer = ASPxClientEdit.prototype.RaiseValueChangedEvent.call(this);
            processOnServer = this.raisePositionChanged(processOnServer);
            return processOnServer;
        },

        SendPostBackInternal: function(postBackArg) {
            if(this.autoPostBack)
                this.stopIncDecTimer();
            ASPxClientEdit.prototype.SendPostBackInternal.call(this, postBackArg);
        },

        /* Event Handlers */
        onButtonDown: function (isDecrement) {
            if(!this.readOnly && this.GetEnabled() && !this.isButtonPressed) {
                this.isButtonPressed = true;
                if(isDecrement)
                    this.decrementValue();
                else
                    this.incrementValue();
                this.startIncDecTimer(!isDecrement);
                this.showValueToolTip(true);
            }
        },

        onButtonMouseUp: function () {
            this.stopIncDecTimer();
            this.isButtonPressed = false;
        },

        OnFocus: function () {
            if(this.GetEnabled()) {
                ASPxTrackBarToolTipHelper.OnTrackBarFocus(this);
                ASPxClientEdit.prototype.OnFocus.call(this);
                this.updateFocusedDragHandleClass();
            }
        },

        onMouseUp: function () {
            if(!this.isFirstMoveAction) {
                this.riseTrackEnd();
                this.isFirstMoveAction = true;
            }
            this.onDragHandleMouseUp();
            this.onButtonMouseUp();
        },

        onMouseMove: function (evt) {
            var movedElement = this.getMovedDragHandle();

            if(movedElement && this.GetEnabled() && !this.readOnly) {
                if(ASPx.Browser.WebKitTouchUI && ASPx.TouchUIHelper.isGesture)
                    return;

                if(this.isFirstMoveAction) {
                    this.isFirstMoveAction = false;
                    this.riseTrackStart();
                }

                if(movedElement === this.getBarHighlightElement())
                    this.setSelectionByMouseEvt(evt);
                else
                    this.setValueByMouseEvt(evt);

                this.preventNextBarHighlightMouseClick = this.preventNextTrackMouseClick = true;
                this.preventNextScaleMouseClick = ASPx.Browser.WebKitTouchUI;

                this.riseTrack();
                ASPxTrackBarToolTipHelper.UpdateToolTip(this);
                if(ASPx.TouchUIHelper.isTouchEvent(evt))
                    evt.preventDefault();
            }
        },

        onScaleClick: function (evt) {
            if(!this.preventNextScaleMouseClick) {
                this.setValueByMouseEvt(evt);
                this.showValueToolTip(true);
            }
        },

        onBarHighlightClick: function (evt) {
            if(!this.preventNextBarHighlightMouseClick) {
                this.clearMovedDragHandle();
                this.setValueByMouseEvt(evt);
                this.showValueToolTip(true);
            }
            else
                this.preventNextBarHighlightMouseClick = false;
        },

        onBarHighlightMouseDown: function (evt) {
            this.setMovedDragHandle(ASPx.Evt.GetEventSource(evt));
            if(this.allowRangeSelection) {
                var barHighlightElement = this.getBarHighlightElement(),
                    barHighlightElementSize = this.getElementSize(barHighlightElement);
                this.barHighlightStartPos = this.getMousePosByEvent(evt) - this.getElementAbsolutePos(barHighlightElement);
                this.barHighlightStartPos = this.correctIdentByDirection(this.barHighlightStartPos, barHighlightElementSize);
            }

            this.showValueToolTip();
        },

        OnBrowserWindowResize: function (evt) {
            this.clearInternalCache();
            this.AdjustControl();
        },

        onDragHandleMouseDown: function (evt) {
            this.setMovedDragHandle(ASPx.Evt.GetEventSource(evt));
            this.showValueToolTip();
        },

        onDragHandleMouseUp: function (evt) {
            window.setTimeout(function () {
                this.preventNextScaleMouseClick = false;
            } .aspxBind(this), 100);
            if(this.getMovedDragHandle()) {
                this.clearMovedDragHandle();
                ASPxTrackBarToolTipHelper.HideToolTip(this);
            }
        },

        OnMouseWheel: function (evt) {
            if(!this.enableMouseWheel)
                return;

            var wheelDelta = ASPx.Evt.GetWheelDelta(evt);
            if(wheelDelta > 0)
                this.decrementValue();
            else if(wheelDelta < 0)
                this.incrementValue();
            this.showValueToolTip(true);
            return ASPx.Evt.PreventEvent(evt);
        },

        onKeyDown: function (evt) {
            switch (evt.keyCode) {
                case ASPx.Key.Left:
                    if(this.isHorizontal)
                        this.onButtonDown(this.isNormalDirection);
                    break;
                case ASPx.Key.Right:
                    if(this.isHorizontal)
                        this.onButtonDown(!this.isNormalDirection);
                    break;
                case ASPx.Key.Up:
                    if(!this.isHorizontal)
                        this.onButtonDown(this.isNormalDirection);
                    break;
                case ASPx.Key.Down:
                    if(!this.isHorizontal)
                        this.onButtonDown(!this.isNormalDirection);
                    break;
            }
            this.showValueToolTip(true);
        },

        onKeyUp: function () {
            this.onMouseUp();
        },

        onTrackClick: function (evt) {
            if(!this.preventNextTrackMouseClick) {
                this.setValueByMouseEvt(evt);
                this.showValueToolTip(true);
            }
            else
                this.preventNextTrackMouseClick = false;
        },

        /* Adjust */
        correctElementPositions: function () {
            if(this.scalePosition === ASPxClientTrackBarPosition.Both) {
                var isEvenSize = this.cache.mainElementReversedSize % 2 === 0;
                var mainElementCenter = (this.cache.mainElementReversedSize + (isEvenSize ? 0 : 1)) / 2;

                var cerrectionValue = null;
                if(ASPx.Browser.WebKitFamily && !this.isHorizontal && !isEvenSize)
                    cerrectionValue = 0;
                if(ASPx.Browser.WebKitFamily && this.isHorizontal && !isEvenSize)
                    cerrectionValue = 0;

                if(cerrectionValue !== null) {
                    var trackElement = this.getTrackElement(),
                    incButton = this.getButtonElement(false),
                    decButton = this.getButtonElement(true);
                    var newElementIdent = mainElementCenter + cerrectionValue;
                    this.setElementIdent(trackElement, newElementIdent, true);
                    if(incButton && decButton) {
                        this.setElementIdent(incButton, newElementIdent, true);
                        this.setElementIdent(decButton, newElementIdent, true);
                    }
                }
            }
        },

        getBarHighlightSize: function () {
            var mainDragHandleElement = this.getMainDragHandleElement(),
                mainDragHandlePos = this.getElementAbsolutePos(mainDragHandleElement);
            var result = 0;
            if(this.allowRangeSelection) {
                var secondaryDragHandleElement = this.getSecondaryDragHandleElement(),
                    secondaryDragHandlePos = this.getElementAbsolutePos(secondaryDragHandleElement);
                result = Math.abs(mainDragHandlePos - secondaryDragHandlePos);
            } else {
                var mainDragHandleElementCenterIdent = this.getElementIdent(mainDragHandleElement) +
                    this.cache.dragHandleElementSizeHalf;
                result = this.correctIdentByDirection(mainDragHandleElementCenterIdent, this.cache.trackElementSize);
            }
            var borders = this.cache.barHighlightElementBorders - this.cache.trackElementBorders / 2;
            return result !== 0 ? Math.round(result - borders) : 0;
        },

        AdjustControlCore: function () {
            if(!this.GetMainElement() || !this.getContentContainer()) return;

            this.updateInternalCache(true);
            if(!this.isInternaCacheInitialized())
                return;

            this.adjustContentContainer();
            this.correctElementPositions();
            this.adjustTrack();
            this.updateInternalCache(false, true);
            this.updateDragHandlePositions(true);
            this.updateDragHandlesVisibility();

            this.updateTrackPosition();
            if(this.getScaleElement()) {
                this.adjustScale();
                this.updateScalePosition();
                this.updateTickSelectionStates();
            }

            if(!this.isAdjusted) {
                ASPx.SetElementVisibility(this.GetMainElement(), true);
                this.isAdjusted = true;
            }
        },

        NeedCollapseControlCore: function() {
            return true;
        },

        adjustContentContainer: function () {
            var mainElement = this.GetMainElement(),
                contentContainer = this.getContentContainer();
            contentContainer.style.width = this.cache.mainElementWidth + "px";
            contentContainer.style.height = this.cache.mainElementHeight + "px";
        },

        adjustScale: function () {
            this.updateScaleSize();
            var scaleElement = this.getScaleElement(),
                tickElements = this.getTickCollection();
            var scaleElementReversedSize = this.isHorizontal ? scaleElement.offsetHeight : scaleElement.offsetWidth;

            for(var i = 0; i < tickElements.length; i++) {
                var tickNum = this.isNormalDirection ? i : tickElements.length - i - 1;
                this.behaviorStrategy.initializeScaleElements(tickElements[i], tickNum);
                this.setReversedElementSize(tickElements[i], scaleElementReversedSize);
            }
            this.setElementSize(scaleElement, this.cache.scaleElementSize);
        },

        adjustTrack: function () {
            var trackElement = this.getTrackElement();
            var trackElementBordersAndPaddings = this.isHorizontal ? ASPx.GetLeftRightBordersAndPaddingsSummaryValue(trackElement) :
                ASPx.GetTopBottomBordersAndPaddingsSummaryValue(trackElement);
            var trackSize = this.cache.mainElementSize - this.cache.buttonSizesAmount - trackElementBordersAndPaddings;
            if(this.isOutsideDragHandleDisplayMode)
                trackSize -= this.cache.dragHandleElementSize - 1;
            this.setElementSize(trackElement, trackSize);
        },

        updateScaleSize: function () {
            var scaleElement = this.getScaleElement(),
                mainElement = this.GetMainElement();
            var trackElement = this.getTrackElement();
            if(this.scalePosition === ASPxClientTrackBarPosition.Both) {
                this.setElementSize(scaleElement, this.cache.mainElementSize);
                this.setElementSize(scaleElement, this.cache.mainElementReversedSize, true);
            }
            else {
                var getAbsoluteFunc = this.isHorizontal ? ASPx.GetAbsoluteY : ASPx.GetAbsoluteX;
                var mainElementPos = getAbsoluteFunc(mainElement),
                    trackElementPos = getAbsoluteFunc(trackElement);
                var trackElementReversedSize = this.isHorizontal ? trackElement.offsetHeight : trackElement.offsetWidth;
                var trackElementReversedSizeHalf = Math.ceil(trackElementReversedSize / 2);
                var leftTopSideSize = trackElementPos - mainElementPos;
                var rightBottomSideSize = this.cache.mainElementReversedSize - leftTopSideSize - trackElementReversedSize;
                var scaleSize = this.scalePosition === ASPxClientTrackBarPosition.LeftOrTop ? leftTopSideSize : rightBottomSideSize;
                this.setReversedElementSize(scaleElement, scaleSize + trackElementReversedSizeHalf);
            }
        },

        updateBarHighlightPosition: function () {
            var barHighlightElement = this.getBarHighlightElement(),
                dragHandleElement = this.getMainDragHandleElement(),
                trackElement = this.getTrackElement();

            var barHighlightPos = this.getElementAbsolutePos(trackElement);

            if(this.allowRangeSelection || !this.isNormalDirection) {
                var targetDragHandleElement = dragHandleElement;
                if(this.allowRangeSelection && !this.isNormalDirection)
                    targetDragHandleElement = this.getSecondaryDragHandleElement();
                barHighlightPos = this.getElementAbsolutePos(targetDragHandleElement) +
                    Math.floor(this.cache.dragHandleElementSizeHalf);
            }
            this.setElementSize(barHighlightElement, this.getBarHighlightSize());
            this.setElementAbsolutePos(barHighlightElement, barHighlightPos);
        },

        updateDragHandlePositions: function (withoutAnimation) {
            if(!this.isInternaCacheInitialized())
                return;
            if(this.allowRangeSelection) {
                this.updateDragHandlePosition(this.getMainDragHandleElement(), this.getInternalValue(0), withoutAnimation);
                this.updateDragHandlePosition(this.getSecondaryDragHandleElement(), this.getInternalValue(1), withoutAnimation);
            }
            else
                this.updateDragHandlePosition(this.getMainDragHandleElement(), this.GetValue(), withoutAnimation);
        },

        updateDragHandlePosition: function (dragHandleElement, value, withoutAnimation) {
            var dragHandleElementIdent = this.behaviorStrategy.getTrackIdentByValue(value);
            if(this.animationEnabled && this.isAdjusted && !this.getMovedDragHandle() && !withoutAnimation)
                ASPxTrackBarTrackAnimationHelper.StartTrackAnimation(this, dragHandleElement, dragHandleElementIdent);
            else {
                this.setElementIdent(dragHandleElement, dragHandleElementIdent);
                this.updateBarHighlightPosition();
            }
        },

        updateTickSelectionStates: function () {
            if(this.scaleLabelHighlightMode === ASPxClientTrackHighlightMode.None)
                return;

            var stateController = ASPx.GetStateController(),
                tickElements = this.getTickCollection();

            var currentSelection = this.getSelection();
            for(var i = 0; i < tickElements.length; i++) {
                if(this.behaviorStrategy.isTickSelected(i, currentSelection))
                    stateController.SelectElementBySrcElement(tickElements[i]);
                else
                    stateController.DeselectElementBySrcElement(tickElements[i]);
            }
        },

        /* Internal Elements */

        getMainDragHandleElement: function () {
            return ASPx.CacheHelper.GetCachedElement(this, "mainDragHandle", 
                function() { 
                    return ASPx.GetNodesByTagName(this.getTrackElement(), "A")[0];
                });
        },

        getSecondaryDragHandleElement: function () {
            return ASPx.CacheHelper.GetCachedElement(this, "secondaryDragHandle", 
                function() { 
                    return ASPx.GetNodesByTagName(this.getTrackElement(), "A")[1];
                });
        },

        getBarHighlightElement: function () {
            return this.GetChildElement("S");
        },

        GetInputElement: function () {
            return this.GetChildElement("I");
        },

        getTrackElement: function () {
            return this.GetChildElement("T");
        },

        getScaleElement: function () {
            return ASPx.CacheHelper.GetCachedElement(this, "scale", 
                function() { 
                    return ASPx.GetNodeByTagName(this.GetMainElement(), "UL", 0);
                });
        },

        getContentContainer: function () {
            return ASPx.CacheHelper.GetCachedElement(this, "content", 
                function() { 
                    return ASPx.GetNodeByTagName(this.GetMainElement(), "DIV", 0);
                });
        },

        getFirstLargeTick: function () {
            return ASPx.CacheHelper.GetCachedElement(this, "firstLargeTick", 
                function() { 
                    return this.getTickCollection()[0];
                });
        },

        getButtonElement: function (decButton) {
            if(!this.isButtonsExist)
                return null;

            var contentElement = this.getContentContainer(),
                anchorCollection = ASPx.GetNodesByTagName(contentElement, "A");

            var getOutAnchor = function (last) {
                return anchorCollection[last ? anchorCollection.length - 1 : 0];
            };

            if(decButton) {
                return ASPx.CacheHelper.GetCachedElement(this, "decButton", 
                    function() { 
                        return getOutAnchor(this.isNormalDirection);
                    });
            } else {
                return ASPx.CacheHelper.GetCachedElement(this, "incButton", 
                    function() { 
                        return getOutAnchor(!this.isNormalDirection);
                    });
            }
        },

        getTickCollection: function () {
            return ASPx.GetNodesByTagName(this.getScaleElement(), "LI");
        },

        isButtonElementsExist: function () {
            var contentElement = this.getContentContainer(),
                anchorCollection = ASPx.GetNodesByTagName(contentElement, "A");
            return anchorCollection.length >= 3;
        },

        /* Element Utils */
        correctIdentByDirection: function (ident, elementSize) {
            return !this.isNormalDirection ? elementSize - ident : ident;
        },

        correctTrackIdentByDirection: function (ident) {
            return this.correctIdentByDirection(ident, this.cache.scaleElementSize);
        },

        setElementSize: function (element, size, reversed) {
            if((this.isHorizontal && !reversed) || (!this.isHorizontal && reversed))
                element.style.width = size + "px";
            else
                element.style.height = size + "px";
        },

        getReversedElementSize: function (element) {
            return this.getElementSize(element, true);
        },

        getElementMargins: function (element) {
            var currentStyle = ASPx.GetCurrentStyle(element);
            return this.isHorizontal ? ASPx.PxToInt(currentStyle.marginLeft) + ASPx.PxToInt(currentStyle.marginRight) :
                ASPx.PxToInt(currentStyle.marginTop) + ASPx.PxToInt(currentStyle.marginBottom);
        },

        getElementSize: function (element, reversed) {
            if((this.isHorizontal && !reversed) || (!this.isHorizontal && reversed))
                return element.offsetWidth;
            else
                return element.offsetHeight;
        },

        setReversedElementSize: function (element, size) {
            this.setElementSize(element, size, true);
        },

        setElementAbsolutePos: function (element, pos, reversed) {
            if((this.isHorizontal && !reversed) || (!this.isHorizontal && reversed))
                ASPx.SetAbsoluteX(element, pos);
            else
                ASPx.SetAbsoluteY(element, pos);
        },

        setElementIdent: function (element, ident, reversed) {
            if((this.isHorizontal && !reversed) || (!this.isHorizontal && reversed))
                element.style.left = ident + "px";
            else
                element.style.top = ident + "px";
        },

        getElementAbsolutePos: function (element, reversed) {
            if((this.isHorizontal && !reversed) || (!this.isHorizontal && reversed))
                return ASPx.GetAbsoluteX(element);
            else
                return ASPx.GetAbsoluteY(element);
        },
        getReversedElementAbsolutePos: function (element) {
            return this.getElementAbsolutePos(element, true);
        },

        getElementIdent: function (element) {
            return ASPx.PxToInt(this.isHorizontal ? element.style.left : element.style.top);
        },

        getMousePosByEvent: function (evt) {
            return this.isHorizontal ? ASPx.Evt.GetEventX(evt) : ASPx.Evt.GetEventY(evt);
        },

        /* Core */
        setSelectionByMouseEvt: function (evt) {
            var trackElement = this.getTrackElement();
            var cursorPosition = this.getMousePosByEvent(evt),
                trackPosition = this.getElementAbsolutePos(trackElement);
            var ident = cursorPosition - trackPosition;
            ident += this.isNormalDirection ? -this.barHighlightStartPos : this.barHighlightStartPos;
            this.behaviorStrategy.detectSelection(ident);
        },

        setValueByMouseEvt: function (evt) {
            var trackElement = this.getTrackElement();
            var cursorPosition = this.getMousePosByEvent(evt),
                trackPosition = this.getElementAbsolutePos(trackElement);
            var ident = cursorPosition - trackPosition;
            var newValue = this.behaviorStrategy.getValueByTrackIdent(ident);
            if(typeof (newValue) != "undefined")
                this.setValueInternal(newValue, null, true);
        },

        /* Focus */
        addFocusedClass: function (element) {
            var trackElement = this.getTrackElement();
            trackElement.className += " " + (this.isMainDragHandleFocused ? TrackBarConsts.FOCUSED_MD_SYSTEM_CLASS_NAME
                : TrackBarConsts.FOCUSED_SD_SYSTEM_CLASS_NAME);
        },

        getFocusedDragHandleValue: function () {
            return this.isMainDragHandleFocused ? this.getValueStart() : this.getValueEnd();
        },

        getUnfocusedDragHandleValue: function () {
            return this.isMainDragHandleFocused ? this.getValueEnd() : this.getValueStart();
        },

        getFocusedDragHandleElement: function () {
            return this.isMainDragHandleFocused ? this.getMainDragHandleElement() : this.getSecondaryDragHandleElement();
        },

        removeFocusedClass: function () {
            var trackElement = this.getTrackElement();
            var removeClass = function (element, className) {
                element.className = element.className.replace(" " + className, "");
            };
            removeClass(trackElement, TrackBarConsts.FOCUSED_MD_SYSTEM_CLASS_NAME);
            removeClass(trackElement, TrackBarConsts.FOCUSED_SD_SYSTEM_CLASS_NAME);
        },

        reverseFocus: function () {
            var movedDragHandleElement = this.getMovedDragHandle();

            this.isMainDragHandleFocused = !this.isMainDragHandleFocused;
            this.updateFocusedDragHandleClass();

            if(movedDragHandleElement && movedDragHandleElement !== this.getBarHighlightElement())
                this.updatePressedDragHandle();
        },

        setFocusedDragHandle: function (dragHandleElement) {
            if(dragHandleElement !== this.getBarHighlightElement())
                this.isMainDragHandleFocused = dragHandleElement.id === this.getMainDragHandleElement().id;
            this.updateFocusedDragHandleClass();
        },

        updateFocusedDragHandleClass: function () {
            this.removeFocusedClass();
            this.addFocusedClass();
        },

        updatePressedDragHandle: function () {
            ASPx.GetStateController().SetPressedElement(this.getFocusedDragHandleElement());
            ASPx.GetStateController().SetCurrentPressedElement(null);
        },

        /* ClientEnabled */
        changeEnabledAttribute: function (enabled) {
            var inputElement = this.GetInputElement();
            if(inputElement)
                this.ChangeSpecialInputEnabledAttributes(inputElement, ASPx.Attr.ChangeEventsMethod(enabled));
        },

        changeEnabledStateItems: function (enabled) {
            ASPx.GetStateController().SetElementEnabled(this.getButtonElement(true), enabled);
            ASPx.GetStateController().SetElementEnabled(this.getButtonElement(false), enabled);
            ASPx.GetStateController().SetElementEnabled(this.getMainDragHandleElement(), enabled);
            ASPx.GetStateController().SetElementEnabled(this.getSecondaryDragHandleElement(), enabled);
            ASPx.GetStateController().SetElementEnabled(this.GetMainElement(), enabled);
        },

        /* Value */
        setInternalValue: function (index, value) {
            var inputElement = this.GetInputElement();
            var values = this.getInternalValues();
            values[index] = value;
            inputElement.value = ASPx.Json.ToJson(values);
        },

        getInternalValue: function (index) {
            var values = this.getInternalValues();
            var value = values[index];
            return this.behaviorStrategy.prepareRawValue(value);
        },

        getInternalValues: function () {
            var inputElement = this.GetInputElement();
            return eval(inputElement.value);
        },

        getPositionByValue: function (value) {
            return this.behaviorStrategy.getPositionByValue(value);
        },

        getSelection: function () {
            return { start: this.GetPositionStart(), end: this.GetPositionEnd() };
        },

        setSelection: function (selection, riseEvents) {
            this.SetPositionStart(selection.start, riseEvents);
            this.SetPositionEnd(selection.end, riseEvents)
        },

        setValueInternal: function (firstValue, secondValue, riseEvents) {
            var isAPIChanged = !riseEvents;
            if((this.readOnly && !isAPIChanged) || !this.GetEnabled())
                return;

            if(!this.allowRangeSelection) {
                this.updateInternalValue(firstValue, 0, riseEvents);
                return;
            }

            if(!secondValue)
                secondValue = this.getSecondRangeValue(firstValue);
            if(secondValue === null)
                secondValue = this.behaviorStrategy.getNullValue();

            var newSelection = this.createSelection(firstValue, secondValue),
                currentSelection = this.getSelection();
            this.setSelection(newSelection, riseEvents);
            var startDragHandleElement = this.getMainDragHandleElement(),
                endDragHandleElement = this.getSecondaryDragHandleElement();

            if(newSelection.start !== currentSelection.start) {
                this.updateDragHandlePosition(startDragHandleElement, this.getValueStart());
                if(!this.isMainDragHandleFocused) {
                    this.updateDragHandlePosition(endDragHandleElement, this.getValueEnd(), true);
                    this.reverseFocus();
                }
            }

            var isReflection = newSelection.end == currentSelection.start;
            if(newSelection.end !== currentSelection.end && !isReflection) {
                this.updateDragHandlePosition(endDragHandleElement, this.getValueEnd());
                if(this.isMainDragHandleFocused) {
                    this.updateDragHandlePosition(startDragHandleElement, this.getValueStart(), true);
                    this.reverseFocus();
                }
            }
        },

        updateInternalValue: function (newValue, index, riseEvents, withoutAnimation) {
            var currentSelection = this.getSelection(),
                currentValue = this.getInternalValue(index);
            newValue = this.behaviorStrategy.correctValue(newValue);
            var newPosition = this.behaviorStrategy.getPositionByValue(newValue);

            if(currentValue !== newValue) {
                var isStartValueChanged = index === 0,
                    cancelChanging = false;

                if(riseEvents) {
                    var newPositionStart = isStartValueChanged ? newPosition : currentSelection.start,
                        newPositionEnd = isStartValueChanged ? currentSelection.end : newPosition;
                    cancelChanging = this.risePositionChanging(currentSelection.start, currentSelection.end,
                        newPositionStart, newPositionEnd);
                }
                if(!cancelChanging) {
                    if(isStartValueChanged && newPosition > currentSelection.end)
                        this.setInternalValue(1, newValue);

                    else if(!isStartValueChanged && newPosition < currentSelection.start)
                        this.setInternalValue(0, newValue);

                    this.setInternalValue(index, newValue);
                    this.updateDragHandlePositions(withoutAnimation);
                    if(this.getScaleElement())
                        this.updateTickSelectionStates();
                    if(riseEvents) {
                        if(this.valueChangedDelay != 0)
                            this.startValueChangedDelayTimer(currentSelection);
                        else
                            this.OnValueChanged();
                    }
                }
            }
        },

        getSecondRangeValue: function (firstValue) {
            var currentSelection = this.getSelection();
            var valuePosition = this.behaviorStrategy.getPositionByValue(firstValue);

            var startValueDistance = Math.abs(currentSelection.start - valuePosition),
                endValueDistance = Math.abs(currentSelection.end - valuePosition);
            var unactiveDragHandleValue = this.getUnfocusedDragHandleValue();

            var areDragHandleValuesEqual = startValueDistance === 0 && !this.isMainDragHandleFocused ||
                endValueDistance === 0 && this.isMainDragHandleFocused;
            var isDragging = !!this.getMovedDragHandle();
            var isValueBetweenDragHandles = startValueDistance === endValueDistance;
            var isIncDecTimerPerformed = this.isButtonPressed;

            if(areDragHandleValuesEqual || isDragging || isValueBetweenDragHandles || isIncDecTimerPerformed)
                return unactiveDragHandleValue;
            else
                return startValueDistance < endValueDistance ? this.getValueEnd() : this.getValueStart();
        },

        createSelection: function (start, end) {
            var corectedStartValue = this.behaviorStrategy.correctValue(start),
                corectedEndValue = this.behaviorStrategy.correctValue(end);
            var startPosition = this.behaviorStrategy.getPositionByValue(corectedStartValue),
                endPosition = this.behaviorStrategy.getPositionByValue(corectedEndValue);
            return startPosition > endPosition ? { start: endPosition, end: startPosition} :
                { start: startPosition, end: endPosition };
        },

        incrementValue: function () {
            this.incrementValueInternal(this.step);
        },

        decrementValue: function () {
            this.incrementValueInternal(-this.step);
        },

        incrementValueInternal: function (step, riseEvents) {
            var currentValue = this.allowRangeSelection ? this.getFocusedDragHandleValue() : this.GetValue();
            var newValue = this.behaviorStrategy.getIncrementedValue(currentValue, step);
            this.setValueInternal(newValue, null, true);
        },

        setMovedDragHandle: function (dragHandleElement) {
            this.setFocusedDragHandle(dragHandleElement);
            this.movedElement = dragHandleElement;
        },

        getMovedDragHandle: function () {
            return this.movedElement;
        },

        clearMovedDragHandle: function () {
            this.movedElement = null;
        },

        showValueToolTip: function (hideAfterShow) {
            if(this.appearValueToolTip && this.GetEnabled())
                ASPxTrackBarToolTipHelper.ShowToolTip(this, hideAfterShow);
        },

        getValueToolTipText: function (position) {
            return this.GetItemCount() > 0 ? this.behaviorStrategy.getItemToolTip(position) :
                position;
        },

        getValueEnd: function () {
            return this.getInternalValue(1);
        },

        getValueStart: function () {
            return this.getInternalValue(0);
        },

        setValueEnd: function (value, riseEvents, withoutAnimation) {
            return this.updateInternalValue(value, 1, riseEvents, withoutAnimation);
        },

        setValueStart: function (value, riseEvents, withoutAnimation) {
            return this.updateInternalValue(value, 0, riseEvents, withoutAnimation);
        },

        /* Value Changed Delay */
        clearValueChangedTimer: function () {
            window.clearTimeout(this.valueChangedDelayTimer);
            this.valueChangedDelayTimer = null;
        },

        onValueChangedTimer: function (currentSelection) {
            var currentSelection = this.getSelection();
            if(currentSelection.start != this.savedSelection.start ||
                (this.allowRangeSelection && currentSelection.end != this.savedSelection.end))
                this.OnValueChanged();
            this.valueChangedDelayTimer = null;
        },

        startValueChangedDelayTimer: function (currentSelection) {
            if(this.valueChangedDelayTimer)
                this.clearValueChangedTimer();
            else
                this.savedSelection = currentSelection;

            var valueChangedDelayHandler = function() {
                this.onValueChangedTimer(currentSelection);
            };
            this.valueChangedDelayTimer = ASPx.Timer.SetControlBoundTimeout(valueChangedDelayHandler, this, this.valueChangedDelay);
        },

        /* API */
        GetItemIndexByValue: function (value) {
            if(this.behaviorStrategy.getItemNumByValue)
                return this.behaviorStrategy.getItemNumByValue(value);
            else
                return -1;
        },
        GetItemValue: function (index) {
            if(this.behaviorStrategy.getItemValue)
                return this.behaviorStrategy.getItemValue(index);
        },
        GetItemText: function (index) {
            if(this.behaviorStrategy.getItemText)
                return this.behaviorStrategy.getItemText(index);
        },
        GetItemToolTip: function (index) {
            if(this.behaviorStrategy.getItemToolTip)
                return this.behaviorStrategy.getItemToolTip(index);
        },
        GetItemCount: function () {
            return this.items ? this.items.length : 0;
        },
        SetPositionEnd: function (selectionEnd, riseEvents) {
            var newValue = this.behaviorStrategy.getValueByPosition(selectionEnd);
            this.setValueEnd(newValue, riseEvents, !riseEvents);
        },
        SetPositionStart: function (selectionStart, riseEvents) {
            var newValue = this.behaviorStrategy.getValueByPosition(selectionStart);
            this.setValueStart(newValue, riseEvents, !riseEvents);
        },
        GetPositionEnd: function () {
            return this.behaviorStrategy.getPosition(false);
        },
        GetPositionStart: function () {
            return this.behaviorStrategy.getPosition(true);
        },
        GetPosition: function () {
            return this.GetPositionStart();
        },
        SetPosition: function (position) {
            return this.SetPositionStart(position);
        },

        GetEnabled: function () {
            return this.enabled && this.clientEnabled;
        },

        SetEnabled: function (enabled) {
            ASPxClientEdit.prototype.SetEnabled.call(this, enabled);
            this.changeEnabledStateItems(enabled);
            this.changeEnabledAttribute(enabled);
        },

        SetValue: function (value) {
            this.setValueStart(value, false, true);
        },

        GetValue: function () {
            return this.getValueStart();
        }
    });
    ASPxClientTrackBar.Cast = ASPxClientControl.Cast;

    ASPx.Ident.IsASPxTrackBar = function (obj) {
        return !!obj.isASPxTrackBar;
    };
    var ASPxClientTrackBarPositionChangingEventArgs = ASPx.CreateClass(ASPxClientProcessingModeEventArgs, {
        constructor: function (currentPositionStart, currentPositionEnd, newPositionStart, newPositionEnd) {
            this.constructor.prototype.constructor.call(this, false);
            this.cancel = false;
            this.currentPosition = currentPositionStart;
            this.currentPositionEnd = currentPositionEnd;
            this.currentPositionStart = currentPositionStart;
            this.newPosition = newPositionStart;
            this.newPositionEnd = newPositionEnd;
            this.newPositionStart = newPositionStart;
        }
    });

    ASPxTrackBarTickModeStrategy = ASPx.CreateClass(null, {
        constructor: function (trackBar) {
            this.constructor.prototype.constructor.call(this);
            this.isNormalDirection = trackBar.isNormalDirection;
            this.isOutsideDragHandleDisplayMode = trackBar.isOutsideDragHandleDisplayMode;
            this.largeTickInterval = trackBar.largeTickInterval;
            this.largeTickStartValue = trackBar.largeTickStartValue;
            this.largeTickEndValue = trackBar.largeTickEndValue;
            this.maxValue = trackBar.maxValue;
            this.minValue = trackBar.minValue;
            this.scaleLabelFormatString = trackBar.scaleLabelFormatString;
            this.smallTickFrequency = trackBar.smallTickFrequency;
            this.tickValue = trackBar.tickValue;
            this.trackBar = trackBar;
        },

        addElementCloneToScale: function (element, previousElement) {
            var scaleElement = this.trackBar.getScaleElement();
            var clone = element.cloneNode(true);
            if(this.isNormalDirection)
                scaleElement.appendChild(clone);
            else
                scaleElement.insertBefore(clone, previousElement);
            return clone;
        },

        buildScale: function () {
            var ticksFromServer = this.trackBar.getTickCollection();
            var largeTick = ticksFromServer[0],
                smallTick = ticksFromServer[1];

            this.scaleMap = this.createScaleMap(this.largeTickStartValue, this.largeTickEndValue,
                this.minValue, this.maxValue, this.smallTickFrequency, this.tickValue);

            var previousElement = largeTick;
            for(var i = 0; i < this.scaleMap.length; i++) {
                previousElement = this.addElementCloneToScale(this.scaleMap[i].isLargeTick ? largeTick : smallTick, previousElement);
                if(this.scaleMap[i].isLargeTick)
                    this.setElementLabel(previousElement, this.scaleMap[i].tickValue);
            }

            this.removeElementFromScale(smallTick);
            this.removeElementFromScale(largeTick);
        },

        correctSelection: function (selection) {
            var selectionLength = selection.end - selection.start;
            if(selection.start <= this.minValue)
                selection = { start: this.minValue, end: this.minValue + selectionLength };
            else if(selection.end >= this.maxValue)
                selection = { start: this.maxValue - selectionLength, end: this.maxValue };
            return selection;
        },

        correctValue: function (value) {
            if(value === null)
                return null;

            value = parseFloat(value);
            if(value !== 0 && !value) // value is NaN
                return this.getNullValue();
            if(value < this.minValue)
                return this.minValue;
            else if(value > this.maxValue)
                return this.maxValue;
            else
                return this.differentiateValue(value);
        },

        createScaleMap: function (largeTickStartValue, largeTickEndValue, minValue, maxValue, smallTickFrequency, tickValue) {
            var map = [];
            var firstLargeTickValue = largeTickStartValue || largeTickStartValue === 0 ? largeTickStartValue : minValue,
                lastLargeTickValue = largeTickEndValue || largeTickEndValue === 0 ? largeTickEndValue : maxValue,
                firstLargeTickIdent = firstLargeTickValue - minValue,
                scaleTickIndent = firstLargeTickIdent - Math.floor(firstLargeTickIdent / tickValue.small) * tickValue.small;
            scaleTickIndent = ASPx.CorrectJSFloatNumber(scaleTickIndent);
            var firstTickValue = minValue + scaleTickIndent;
            var tickCount = (maxValue - minValue) / tickValue.small + 1;

            for(var i = 0; i < tickCount; i++) {
                var currentTickValue = ASPx.CorrectJSFloatNumber(firstTickValue + i * tickValue.small);
                if(currentTickValue <= maxValue) {
                    var isLargeTick = (Math.abs(currentTickValue - firstLargeTickValue) / tickValue.large) % 1 === 0;
                    isLargeTick = isLargeTick && currentTickValue >= firstLargeTickValue && currentTickValue <= lastLargeTickValue;

                    if(isLargeTick || (!isLargeTick && smallTickFrequency > 1)) {
                        map.push({
                            isLargeTick: isLargeTick,
                            tickValue: currentTickValue
                        });
                    }
                }
            }
            return map;
        },

        detectSelection: function (ident) {
            var newValue = this.getValueByTrackIdent(ident);
            var newPosition = this.getPositionByValue(newValue);
            var currentSelection = this.trackBar.getSelection();
            var newSelection = {
                start: newPosition,
                end: currentSelection.end + (newPosition - currentSelection.start)
            };
            newSelection = this.correctSelection(newSelection);
            this.trackBar.setSelection(newSelection, true);
        },

        differentiateValue: function (value) {
            var ident = value % this.trackBar.step;
            var result = value - ident;
            if(ident >= this.trackBar.step / 2)
                result += this.trackBar.step;
            return ASPx.CorrectJSFloatNumber(result);
        },

        filedsInitialize: function () {
        },

        getIncDecAcceleratorStep: function () {
            return ASPxClientTrackBarAnimationsConsts.INC_DEC_ACCELERATOR_STEP;
        },

        getIncrementedValue: function (currentValue, step) {
            return currentValue + step;
        },

        getPositionByValue: function (value) {
            if(value === null)
                return this.getNullValue();
            else
                return this.correctValue(value);
        },

        getNullValue: function () {
            return this.minValue;
        },

        getPosition: function (startPosition) {
            var result = this.trackBar.getInternalValue(startPosition ? 0 : 1);
            return result !== null ? result : this.getNullValue();
        },

        getTrackIdentByValue: function (value, ignoreDirection) {
            if(value === null)
                return this.getTrackIdentByValue(this.getNullValue(), ignoreDirection);
            var proc = (value - this.minValue) / (this.maxValue - this.minValue);

            var ident = Math.round((this.trackBar.cache.scaleElementSize - this.trackBar.cache.dragHandleElementSize) * proc);
            if(!ignoreDirection && !this.isNormalDirection)
                ident = this.trackBar.correctTrackIdentByDirection(ident + this.trackBar.cache.dragHandleElementSize);
            if(this.isOutsideDragHandleDisplayMode)
                ident -= Math.floor(this.trackBar.cache.dragHandleElementSizeHalf);
            return ident;
        },

        getValueByPosition: function (position) {
            return this.correctValue(position);
        },

        getValueByTrackIdent: function (ident) {
            if(this.isOutsideDragHandleDisplayMode)
                ident += Math.floor(this.trackBar.cache.dragHandleElementSizeHalf);
            ident = this.trackBar.correctTrackIdentByDirection(ident);
            var proc = (ident - this.trackBar.cache.dragHandleElementSizeHalf) /
                (this.trackBar.cache.scaleElementSize - this.trackBar.cache.dragHandleElementSize);
            return this.minValue + (this.maxValue - this.minValue) * proc;
        },

        initializeScaleElements: function (tickElement, tickNum) {
            var tickElementSize = this.trackBar.cache.dragHandleElementSize;
            this.trackBar.setElementSize(tickElement, tickElementSize);
            var tickElementIndent = this.getTrackIdentByValue(this.scaleMap[tickNum].tickValue);
            if(this.isOutsideDragHandleDisplayMode)
                tickElementIndent += Math.floor(this.trackBar.cache.dragHandleElementSizeHalf);
            this.trackBar.setElementIdent(tickElement, tickElementIndent);
        },

        isTickSelected: function (tickNum, currentSelection) {
            var tickValue = this.trackBar.tickValue.small * tickNum;
            var correctedTickValue = this.trackBar.correctIdentByDirection(tickValue, this.maxValue - this.minValue);

            switch (this.trackBar.scaleLabelHighlightMode) {
                case ASPxClientTrackHighlightMode.AlongBarHighlight:
                    if(this.trackBar.allowRangeSelection)
                        return correctedTickValue >= currentSelection.start && correctedTickValue <= currentSelection.end;
                    else
                        return correctedTickValue <= currentSelection.start;
                case ASPxClientTrackHighlightMode.HandlePosition:
                    if(this.trackBar.allowRangeSelection)
                        return correctedTickValue === currentSelection.start || correctedTickValue === currentSelection.end;
                    else
                        return correctedTickValue === currentSelection.start;
            }
        },

        prepareRawValue: function (value) {
            return value || value === 0 ? parseFloat(value) : null;
        },

        removeElementFromScale: function (element) {
            if(element) {
                var scaleElement = this.trackBar.getScaleElement();
                scaleElement.removeChild(element);
            }
        },

        setElementLabel: function (element, value) {
            var labelElements = ASPx.GetNodesByTagName(element, "SPAN");
            var text = ASPx.Formatter.Format(this.scaleLabelFormatString, value);

            for(var i = 0; i < labelElements.length; i++)
                ASPx.SetInnerHtml(labelElements[i], text);
        }
    });

    ASPxTrackBarItemModeStrategy = ASPx.CreateClass(ASPxTrackBarTickModeStrategy, {
        constructor: function (trackBar) {
            this.constructor.prototype.constructor.call(this, trackBar);
            this.items = this.trackBar.items;
            this.incDecAcceleratorStep = 0;
        },

        buildScale: function () {
            var itemFromServer = this.trackBar.getTickCollection()[0];
            var previousItem = itemFromServer;

            for(var i = 0; i < this.items.length; i++) {
                if(i !== 0)
                    previousItem = this.addElementCloneToScale(itemFromServer, previousItem);
                var itemIndex = this.trackBar.correctIdentByDirection(i, this.items.length - 1);
                this.setElementLabel(previousItem, this.getItemText(itemIndex));
            }
            previousItem.className += " " + (this.isNormalDirection ? TrackBarConsts.LAST_ITEM_SYSTEM_CLASS_NAME :
                TrackBarConsts.FIRST_ITEM_SYSTEM_CLASS_NAME);
            itemFromServer.className += " " + (this.isNormalDirection ? TrackBarConsts.FIRST_ITEM_SYSTEM_CLASS_NAME :
                TrackBarConsts.LAST_ITEM_SYSTEM_CLASS_NAME);
        },

        correctPosition: function (position) {
            if(position < 0)
                return 0;
            else if(position > this.items.length - 1)
                return this.items.length - 1;
            else
                return position;
        },

        correctValue: function (value) {
            if(value === null)
                return null;
            else
                return this.getItemNumByValue(value) !== -1 ? value : this.getItemValue(0);
        },

        detectSelection: function (ident) {
            var newValue = this.getValueByTrackIdent(ident),
                currentSelection = this.trackBar.getSelection();
            if(typeof (newValue) === "undefined")
                return;

            var newSItemNum = this.getItemNumByValue(newValue),
                newEItemNum = newSItemNum + (currentSelection.end - currentSelection.start);

            if(this.isItemNumCorrect(newEItemNum)) {
                var newSelection = {
                    start: newSItemNum,
                    end: newEItemNum
                };
                this.trackBar.setSelection(newSelection, true);
            }
        },

        filedsInitialize: function () {
            this.trackBar.step = 1;
            this.trackBar.isOutsideDragHandleDisplayMode = false;
        },

        getIncDecAcceleratorStep: function () {
            return this.incDecAcceleratorStep;
        },

        getPositionByValue: function (value) {
            if(value === null)
                return 0;
            else
                return this.getItemNumByValue(value);
        },

        getIncrementedValue: function (currentValue, step) {
            var itemNum = this.getItemNumByValue(currentValue);
            var newItemNum = itemNum + step;
            if(this.isItemNumCorrect(newItemNum))
                return this.getItemValue(newItemNum);
            else
                return currentValue;
        },

        getItemElementSize: function () {
            return this.trackBar.cache.trackElementSize / this.items.length;
        },

        getValueByPosition: function (position) {
            return this.getItemValue(this.correctPosition(position));
        },

        getItemNumByValue: function (value) {
            for(var i = 0; i < this.items.length; i++) {
                if(this.getItemValue(i) === value)
                    return i;
            }
            return -1;
        },

        getItemText: function (itemNum) {
            if(this.isItemNumCorrect(itemNum))
                return this.items[itemNum][1];
        },

        getItemToolTip: function (itemNum) {
            if(this.isItemNumCorrect(itemNum)) {
                var toolTip = this.items[itemNum][2];
                return !!toolTip ? toolTip : this.getItemValue(itemNum);
            }
        },

        getItemValue: function (itemNum) {
            if(this.isItemNumCorrect(itemNum))
                return this.items[itemNum][0];
        },

        getNullValue: function () {
            return this.getItemValue(0);
        },

        getPosition: function (startPosition) {
            var value = this.trackBar.getInternalValue(startPosition ? 0 : 1);
            var position = this.getItemNumByValue(value);
            return position === -1 ? 0 : position;
        },

        getTrackIdentByValue: function (value, ignoreDirection) {
            var itemNum = this.getItemNumByValue(value);
            if(itemNum === -1)
                return this.getTrackIdentByValue(this.getNullValue(), ignoreDirection);
            var itemElementSize = this.getItemElementSize();
            var ident = (itemNum * itemElementSize) + itemElementSize / 2;
            if(!this.isNormalDirection)
                ident = this.trackBar.correctTrackIdentByDirection(ident);
            ident -= this.trackBar.cache.dragHandleElementSizeHalf;
            return ident;
        },

        getValueByTrackIdent: function (ident) {
            var itemElementSize = this.getItemElementSize();
            var itemNum = ident / itemElementSize;
            itemNum -= itemNum % 1;
            if(!this.isNormalDirection)
                itemNum = this.trackBar.correctIdentByDirection(itemNum, this.items.length - 1);
            return this.getItemValue(itemNum);
        },

        initializeScaleElements: function (itemElement, itemNum) {
            var itemElementSize = this.getItemElementSize();
            this.trackBar.setElementSize(itemElement, itemElementSize);
            this.trackBar.setElementIdent(itemElement, itemNum * itemElementSize);
        },

        isItemNumCorrect: function (itemNum) {
            return itemNum >= 0 && itemNum < this.items.length;
        },

        isTickSelected: function (itemNum, currentSelection) {
            var correctedItemNum = itemNum;
            switch (this.trackBar.scaleLabelHighlightMode) {
                case ASPxClientTrackHighlightMode.AlongBarHighlight:
                    if(this.trackBar.allowRangeSelection)
                        return correctedItemNum >= currentSelection.start && correctedItemNum <= currentSelection.end;
                    else
                        return correctedItemNum <= currentSelection.start;
                case ASPxClientTrackHighlightMode.HandlePosition:
                    if(this.trackBar.allowRangeSelection)
                        return correctedItemNum === currentSelection.start || correctedItemNum === currentSelection.end;
                    else
                        return correctedItemNum === currentSelection.start;
            }
        },

        prepareRawValue: function (value) {
            return value || value === 0 ? value : null;
        }
    });

    ASPxTrackBarTrackAnimationHelper = {
        // Private Field
        stopCurrentProccess: false,
        nextAnimationAction: null,
        isBusy: false,

        // Public Methods
        StartTrackAnimation: function (trackBar, dragHandleElement, newIdent) {
            var isInternalCall = !trackBar;
            if(!isInternalCall) {
                this.nextAnimationAction = {
                    trackBar: trackBar,
                    dragHandleElement: dragHandleElement,
                    newIdent: newIdent
                };
            }

            if(!this.isBusy && this.nextAnimationAction) {
                this.isBusy = true;
                var currentAnimationAction = this.nextAnimationAction;
                this.nextAnimationAction = null;
                this.stopCurrentProccess = false;
                var barHighlightElement = currentAnimationAction.trackBar.getBarHighlightElement();
                this.animationRecursion(currentAnimationAction.trackBar, currentAnimationAction.dragHandleElement,
                    barHighlightElement, currentAnimationAction.newIdent);
            }
            else
                this.stopCurrentProccess = true;
        },

        //Private Methods
        animationRecursion: function (trackBar, dragHandleElement, barHighlightElement, newIdent) {
            if(this.stopCurrentProccess) {
                this.callback();
                return;
            }

            var currentDragHandleElementIdent = trackBar.getElementIdent(dragHandleElement);
            if(Math.abs(currentDragHandleElementIdent - newIdent) < 3) {
                this.setDragHandleIdent(trackBar, dragHandleElement, newIdent);
                this.callback();
                return;
            }

            this.changeDragHandleIdent(trackBar, dragHandleElement, newIdent, currentDragHandleElementIdent);

            window.setTimeout(function () {
                this.animationRecursion(trackBar, dragHandleElement, barHighlightElement, newIdent);
            } .aspxBind(this), ASPxClientTrackBarAnimationsConsts.TRACK_ANIMATION_SPEED);
        },

        callback: function () {
            this.isBusy = false;
            this.StartTrackAnimation();
        },

        changeDragHandleIdent: function (trackBar, dragHandleElement, targetIdent, currentDragHandleElementIdent) {
            var step = (targetIdent - currentDragHandleElementIdent) * ASPxClientTrackBarAnimationsConsts.TRACK_ANIMATION_QUALITY;
            if(step > -1 && step < 1)
                step = step < 0 ? -1 : 1;
            var dragHandleElementNewIdent = currentDragHandleElementIdent + step;
            this.setDragHandleIdent(trackBar, dragHandleElement, dragHandleElementNewIdent);
        },

        setDragHandleIdent: function (trackBar, dragHandleElement, ident) {
            trackBar.setElementIdent(dragHandleElement, ident);
            trackBar.updateBarHighlightPosition();
            ASPxTrackBarToolTipHelper.UpdateToolTip(trackBar);
        }
    };

    ASPxTrackBarToolTipHelper = {
        // Private Fields
        inProcess: false,
        internalAnimationQueue: [null, null],
        labelToolTipCustomStyles: {},
        toolTipElement: null,
        nextAnimationAction: null,
        timerId: null,

        // Public Methods
        HideToolTip: function (trackBar) {
            if(!trackBar.appearValueToolTip)
                return;

            if(trackBar.animationEnabled)
                this.startToolTipAnimation(trackBar, false);
            else
                this.setToolTipOpacity(0);
        },

        OnTrackBarFocus: function (trackBar) {
            if(trackBar.appearValueToolTip) {
                if(!this.isCustomStyleExist(trackBar) && this.getTrackBarCustomStyleText(trackBar) !== '')
                    this.createLabelToolTipCustomStyle(trackBar);
                this.setToolTipElementStyle(trackBar);
                this.updateToolTipPosition(trackBar); // B215870
            }
        },

        ShowToolTip: function (trackBar, hideAfterShow) {
            if(!trackBar.appearValueToolTip)
                return;

            if(trackBar.animationEnabled)
                this.startToolTipAnimation(trackBar, true, hideAfterShow);
            else {
                this.UpdateToolTip(trackBar);
                this.setToolTipOpacity(1);
                if(hideAfterShow) {
                    if(this.timerId)
                        window.clearTimeout(this.timerId);
                    this.timerId = window.setTimeout(function () {
                        this.setToolTipOpacity(0);
                        this.timerId = null;
                    } .aspxBind(this), 1000);
                }
            }
        },

        UpdateToolTip: function (trackBar) {
            if(trackBar.appearValueToolTip) {
                this.updateToolTipContent(trackBar);
                this.updateToolTipPosition(trackBar);
            }
        },

        // Private Methods
        createLabelToolTipCustomStyle: function (trackBar) {
            var styleSheet = ASPx.GetCurrentStyleSheet();
            if(styleSheet) {
                var customStyleText = this.getTrackBarCustomStyleText(trackBar);
                this.labelToolTipCustomStyles[trackBar.name] = customStyleText ?
                    ASPx.CreateImportantStyleRule(styleSheet, customStyleText) : "";
            }
        },

        getToolTipPosition: function (trackBar) {
            var toolTipElement = this.getToolTipElement(),
                toolTipElementReversedSize = trackBar.getReversedElementSize(toolTipElement),
                toolTipElementSize = trackBar.getElementSize(toolTipElement);

            var toolTipElementPos = 0;
            if(trackBar.allowRangeSelection) {
                var barHighlightElement = trackBar.getBarHighlightElement(),
                    barHighlightElementSize = trackBar.getElementSize(barHighlightElement),
                    barHighlightPos = trackBar.getElementAbsolutePos(barHighlightElement);
                toolTipElementPos = barHighlightPos + barHighlightElementSize / 2;
            }
            else {
                var dragHandleElement = trackBar.getFocusedDragHandleElement();
                var dragHandlePos = trackBar.getElementAbsolutePos(dragHandleElement);
                toolTipElementPos = dragHandlePos + trackBar.cache.dragHandleElementSizeHalf;
            }
            toolTipElementPos -= toolTipElementSize / 2;
            return toolTipElementPos;
        },

        getToolTipReversedPosition: function (trackBar, valueToolTipPosition) {
            var mainElement = trackBar.GetMainElement(),
                toolTipElement = this.getToolTipElement(),
                toolTipElementReversedSize = trackBar.getReversedElementSize(toolTipElement);
            var toolTipElementPos = trackBar.getReversedElementAbsolutePos(mainElement);
            var isLeftOrTop = valueToolTipPosition === ASPxClientTrackBarPosition.LeftOrTop;
            toolTipElementPos += isLeftOrTop ? -toolTipElementReversedSize : trackBar.cache.mainElementReversedSize;
            return toolTipElementPos;
        },

        getTrackBarCustomStyleName: function (trackBar) {
            return trackBar.valueToolTipStyle[0];
        },

        getTrackBarCustomStyleText: function (trackBar) {
            return trackBar.valueToolTipStyle[1];
        },

        isCustomStyleExist: function (trackBar) {
            return !!this.labelToolTipCustomStyles[trackBar.name];
        },

        isToolTipReverseRequired: function (trackBar, toolTipReversedPos) {
            var toolTipElement = this.getToolTipElement(),
                toolTipElementReversedSize = trackBar.getReversedElementSize(toolTipElement);
            var scrollReversedPos = trackBar.isHorizontal ? ASPx.GetDocumentScrollTop() : ASPx.GetDocumentScrollLeft(),
                windowReversedSize = trackBar.isHorizontal ? ASPx.GetDocumentClientHeight() : ASPx.GetDocumentClientWidth();
            var isUpperlimitExceeded = toolTipReversedPos < scrollReversedPos,
                isLowerlimitExceeded = toolTipReversedPos + toolTipElementReversedSize > scrollReversedPos + windowReversedSize;
            return isUpperlimitExceeded || isLowerlimitExceeded;
        },

        setToolTipElementStyle: function (trackBar) {
            var toolTipElement = this.getToolTipElement();
            toolTipElement.className = this.getTrackBarCustomStyleName(trackBar);
            if(this.isCustomStyleExist(trackBar))
                toolTipElement.className += " " + this.labelToolTipCustomStyles[trackBar.name];
        },

        startToolTipAnimation: function (trackBar, show, hideAfterShow) {
            this.UpdateToolTip(trackBar);
            if(show)
                this.nextAnimationAction = hideAfterShow ? "SH" : "S";
            else
                this.nextAnimationAction = "H";

            if(!this.inProcess) {
                this.inProcess = true;
                this.startAnimationCore();
            }
        },

        startAnimationCore: function () {
            var betweenShowAndHide = !this.internalAnimationQueue[0] &&
                this.internalAnimationQueue[1] === "H";

            if(betweenShowAndHide && this.nextAnimationAction === "SH") {
                this.nextAnimationAction = null;
                this.delayAnimationProcess();
                return;
            }

            this.prepareInternalQueue();
            var currentAnimationAction = this.internalAnimationQueue[0];
            if(currentAnimationAction)
                this.animationRecursion(currentAnimationAction);
            else
                this.inProcess = false;
        },

        delayAnimationProcess: function () {
            window.setTimeout(function () {
                ASPxTrackBarToolTipHelper.startAnimationCore();
            }, 500);
        },

        animationRecursion: function (currentAnimationAction) {
            var currentOpacity = this.getToolTipOpacity();
            var newOpacity = currentOpacity;
            if(currentAnimationAction === "S" && currentOpacity < 1) {
                newOpacity += ASPxClientTrackBarAnimationsConsts.TOOLTIP_ANIMATION_QUALITY;
                if(newOpacity > 1)
                    newOpacity = 1;
            } else if(currentAnimationAction === "H" && currentOpacity > 0) {
                newOpacity -= ASPxClientTrackBarAnimationsConsts.TOOLTIP_ANIMATION_QUALITY;
                if(newOpacity < 0.1)
                    newOpacity = 0;
            } else {
                this.internalAnimationQueue[0] = null;
                this.startAnimationCore();
                return;
            }

            this.setToolTipOpacity(newOpacity);
            this.timerId = window.setTimeout(function () {
                ASPxTrackBarToolTipHelper.animationRecursion(currentAnimationAction);
            }, ASPxClientTrackBarAnimationsConsts.TOOLTIP_ANIMATION_SPEED);
        },

        prepareInternalQueue: function () {
            if(!this.internalAnimationQueue[0]) {
                if(this.internalAnimationQueue[1]) {
                    this.internalAnimationQueue[0] = this.internalAnimationQueue[1];
                    this.internalAnimationQueue[1] = null;
                    return;
                } else {
                    if(this.nextAnimationAction === "SH") {
                        this.internalAnimationQueue[0] = "S";
                        this.internalAnimationQueue[1] = "H";
                    } else {
                        if((this.nextAnimationAction === "S" && !this.isToolTipVisible()) ||
                        (this.nextAnimationAction === "H" && this.isToolTipVisible())) {
                            this.internalAnimationQueue[0] = this.nextAnimationAction;
                        }
                    }
                    this.nextAnimationAction = null;
                }
            }
        },

        updateToolTipContent: function (trackBar) {
            var toolTipText = ASPx.Formatter.Format(trackBar.valueToolTipFormat,
                trackBar.getValueToolTipText(trackBar.GetPositionStart()),
                trackBar.getValueToolTipText(trackBar.GetPositionEnd()));
            ASPx.SetInnerHtml(this.getToolTipElement(), toolTipText);
        },

        updateToolTipPosition: function (trackBar) {
            var toolTipElement = this.getToolTipElement();

            var toolTipElementPos = this.getToolTipPosition(trackBar);
            var toolTipElementReversedPos = this.getToolTipReversedPosition(trackBar, trackBar.valueToolTipPosition);

            if(this.isToolTipReverseRequired(trackBar, toolTipElementReversedPos)) {
                var newToolTipPosition = trackBar.valueToolTipPosition;
                if(newToolTipPosition === ASPxClientTrackBarPosition.LeftOrTop)
                    newToolTipPosition = ASPxClientTrackBarPosition.RightOrBottom;
                else
                    newToolTipPosition = ASPxClientTrackBarPosition.LeftOrTop;
                toolTipElementReversedPos = this.getToolTipReversedPosition(trackBar, newToolTipPosition);
            }

            trackBar.setElementAbsolutePos(toolTipElement, toolTipElementPos);
            trackBar.setElementAbsolutePos(toolTipElement, toolTipElementReversedPos, true);
        },

        createToolTip: function () {
            var toolTipElement = document.createElement("DIV");
            document.body.appendChild(toolTipElement);
            this.setToolTipOpacity(0, toolTipElement);
            return toolTipElement;
        },

        getToolTipElement: function () {
            if(!ASPx.IsExistsElement(this.toolTipElement))
                this.toolTipElement = this.createToolTip();
            return this.toolTipElement;
        },

        isToolTipVisible: function () {
            return this.getToolTipOpacity() !== 0;
        },

        setToolTipOpacity: function (value, element) {
            var toolTipElement = element ? element : this.getToolTipElement();
            var newZIndex = TrackBarConsts.VALUE_TOOLTIP_ZINDEX * (value === 0 ? -1 : 1); 
            if(toolTipElement.style.zIndex != newZIndex)
                toolTipElement.style.zIndex = newZIndex; //B216999
            ASPx.SetElementOpacity(toolTipElement, value);
        },

        getToolTipOpacity: function () {
            var toolTipElement = this.getToolTipElement();
            return ASPx.GetElementOpacity(toolTipElement);
        }
    };

    window.ASPxClientTrackBarPosition = ASPxClientTrackBarPosition;
    window.ASPxClientTrackBarDirection = ASPxClientTrackBarDirection;
    window.ASPxClientTrackHighlightMode = ASPxClientTrackHighlightMode;
    window.ASPxClientTrackBar = ASPxClientTrackBar;
    window.ASPxClientTrackBarPositionChangingEventArgs = ASPxClientTrackBarPositionChangingEventArgs;
})();