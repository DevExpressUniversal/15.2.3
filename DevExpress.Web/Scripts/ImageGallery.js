/// <reference path="_references.js"/>
/// <reference path="ImageControlUtils.js"/>

(function() {
    var Constants = {};
    Constants.Hash = "#dxig";
    Constants.InitialOpacityValue = 0;
    Constants.InitialFadedOpacityValue = 0.2;
    Constants.EmptyHash = " ";
    Constants.SlideAnimationDuration = 300;
    Constants.FadeAnimationDuration = 200;

    var CssClassesConstants = {};
    CssClassesConstants.Prefix = "dxig-";
    CssClassesConstants.ThumbnailTextAreaClassName = CssClassesConstants.Prefix + "thumbnailTextArea";
    CssClassesConstants.ThumbnailBorderClassName = CssClassesConstants.Prefix + "thumbnailBorder";
    CssClassesConstants.ThumbnailWrapperClassName = CssClassesConstants.Prefix + "thumbnailWrapper";
    CssClassesConstants.ImageSliderWrapperClassName = CssClassesConstants.Prefix + "imageSliderWrapper";
    CssClassesConstants.BottomPanelClassName = CssClassesConstants.Prefix + "bottomPanel";
    CssClassesConstants.PrevButtonAreaClassName = CssClassesConstants.Prefix + "prevButtonArea";
    CssClassesConstants.NextButtonAreaClassName = CssClassesConstants.Prefix + "nextButtonArea";
    CssClassesConstants.NavigationBarMarkerClassName = CssClassesConstants.Prefix + "navigationBarMarker";
    CssClassesConstants.OverflowPanelClassName = CssClassesConstants.Prefix + "overflowPanel";
    CssClassesConstants.PlayPauseButtonWrapperClassName = CssClassesConstants.Prefix + "playPauseButtonWrapper";
    CssClassesConstants.FullscreenViewerTextAreaClassName = CssClassesConstants.Prefix + "fullscreenViewerTextArea";
    CssClassesConstants.ExpandedText = " dxigExpandedText";

    var ControlIDConstants = {
        FVContainerID: "_FVCell",
        NextNavButton: "_nextBtn",
        PrevNavButton: "_prevBtn",
        Popup: "_Popup",
        Slider: "_Slider",
        CloseButton: "_ClsBtn",
        NavigationBar: "_NavigationBar"
    };

    ElementVisibilityModeEnum = {
        None: 0,
        Faded: 1,
        OnMouseOver: 2,
        Always: 3
    }
    var NavigationButton = ASPx.CreateClass(null, {
        constructor: function(owner, id, cssClassName, isNext, isEnabledEvalFunc) {
            /// <param name="owner" type="FullscreenViewer">FullscreenViewer - owner</param>
            this.owner = owner;
            this.id = owner.popup.name + id;
            this.isEnabledEvalFunc = isEnabledEvalFunc;
            this.cssClassName = cssClassName;
            this.isNext = isNext;
            this.button = null;
            this.relatedButton = null;
            this.buttonDisabled = false;
        },
        adjust: function() {
            var button = this.getButtonArea();
            var textArea = this.owner.getFullscreenViewerTextArea();
            var bottomPanelHeight = 0;
            if(textArea)
                bottomPanelHeight = textArea.offsetHeight;
            if(this.owner.isAlwaysVisibleNavBar)
                bottomPanelHeight += this.owner.getNavigationBarHeight();
            ASPx.SetStyles(button, { height: "100%" });
            ASPx.SetStyles(button, { height: button.offsetHeight - bottomPanelHeight });
        },
        prepare: function() {
            var style = { className: this.cssClassName };
            if(ASPx.Browser.WebKitFamily)
                style.webkitUserSelect = "none";
            if(this.owner.canHandleMouseOverForButtons)
                style.opacity = this.owner.initialOpacity;
            ASPx.SetStyles(this.getButtonArea(), style);
            this.assignHandlers();
        },
        getButton: function() {
            if(!this.button)
                this.button = ASPx.GetElementById(this.id);
            return this.button;
        },
        getButtonArea: function() {
            var button = this.getButton();
            return button ? button.parentNode : null;
        },
        assignHandlers: function() {
            if(this.owner.enabled) {
                var isNext = this.isNext;
                ASPx.Evt.AttachEventToElement(this.getButtonArea(), this.owner.clickEventName, function(evt) {
                    if(!this.canPreventMouseDownEvent())
                        this.onNavigationButtonMouseDown(evt, isNext);
                }.aspxBind(this.owner));
            }
        },
        updateEnabled: function(force, skipRtl) {
            if(this.owner.rtl && !skipRtl)
                this.relatedButton.updateEnabled(force, true);
            var stateController = typeof (ASPx.GetStateController) != "undefined" ? ASPx.GetStateController() : null;
            if(stateController) {
                var buttonDisabled = this.isEnabledEvalFunc();
                if(force || this.buttonDisabled != buttonDisabled)
                    stateController.SetElementEnabled(this.getButton(), buttonDisabled);
                this.buttonDisabled = buttonDisabled;
            }
        }
    });
    var FullscreenViewer = ASPx.CreateClass(null, {
        constructor: function(owner) {
            /// <param name="owner" type="ASPxClientImageGallery">gallery - owner</param>
            this.hasFVTextTemplate = owner.hasFVTextTemplate;
            this.hasItemFVTextTemplate = owner.hasItemFVTextTemplate;
            this.disappearElementsTimerId = -1;
            this.lastActiveItemIndex = owner.lastActiveItemIndex;
            this.elementsVisible = false;
            this.owner = owner;
            this.rtl = owner.rtl;
            this.enabled = owner.enabled;
            this.useHash = owner.useHash;
            this.isVisibleNavigationButtons = owner.navBtnVisibility != ElementVisibilityModeEnum.None;
            this.canHandleMouseOverForButtons = this.isVisibleNavigationButtons && owner.navBtnVisibility != ElementVisibilityModeEnum.Always;
            this.canHandleMouseOverForBottomPanel = owner.navBarVisibility == ElementVisibilityModeEnum.OnMouseOver;
            this.clickEventName = ASPx.Browser.TouchUI ? ASPx.TouchUIHelper.touchMouseUpEventName : "click";
            this.initialOpacity = ASPx.Browser.TouchUI ? Constants.InitialOpacityValue : Constants.InitialFadedOpacityValue;
            this.isAlwaysVisibleNavBar = owner.navBarVisibility == ElementVisibilityModeEnum.Always;
            this.enablePagingByClick = owner.enablePagingByClick;
            this.keyboardSupport = owner.keyboardSupport;

            this.popup = null;
            this.slider = null;
            this.navigationBar = null;
            this.imageSliderWrapper = null;
            this.bottomPanel = null;
            this.navigationBarMarker = null;
            this.fulscreenViewerTextArea = null;
            this.nextNavigationButton = null;
            this.prevNavigationButton = null;
            this.closeButtonWrapper = null;
            this.playPauseButtonWrapper = null;

            this.lastMouseX = 0;
            this.lastMouseY = 0;
            this.contentPaddings = null;
        },
        /* Common */
        initialize: function() {
            this.reset();
            this.prepare();
            var index = this.useHash ? this.owner.getIndexFromString(window.location) : -1;
            if(index != -1)
                this.show(index);
        },
        canPreventMouseDownEvent: function() {
            return ASPx.Browser.TouchUI && this.canHandleMouseOverForButtons && !this.elementsVisible;
        },
        prepare: function() {
            if(this.isDOMInitialized()) {
                this.ensureControlsCreated();

                this.preparePlayPauseButton();
                this.prepareCloseButtonWrapper();
                this.prepareNavigationButtons();
                this.prepareImagePanel();
                this.prepareBottomPanel();
                if(this.enabled) {
                    this.assignHandlersToPopup();
                    this.assignHandlersToImagePanel();
                    this.assignHandlersToBottomPanel();
                }
                this.popup.fadeAnimationDuration = Constants.FadeAnimationDuration;
            }
        },
        ensureControlsCreated: function() {
            this.popup = this.getPopup();
            this.slider = this.getSlider();
            if(this.isVisibleNavigationButtons) {
                this.nextNavigationButton = new NavigationButton(this, ControlIDConstants.NextNavButton, CssClassesConstants.NextButtonAreaClassName, !this.rtl,
                    function() {
                        return this.getActiveItemIndex() != this.getItemCount() - 1;
                    }.aspxBind(this));
                this.prevNavigationButton = new NavigationButton(this, ControlIDConstants.PrevNavButton, CssClassesConstants.PrevButtonAreaClassName, this.rtl,
                    function(evt) {
                        return this.getActiveItemIndex() != 0;
                    }.aspxBind(this));
                this.nextNavigationButton.relatedButton = this.prevNavigationButton;
                this.prevNavigationButton.relatedButton = this.nextNavigationButton;
            }
        },
        getPopup: function() {
            return ASPx.GetControlCollection().Get(this.owner.name + ControlIDConstants.Popup);
        },
        getSlider: function() {
            return ASPx.GetControlCollection().Get(this.popup.name + ControlIDConstants.Slider);
        },
        isDOMInitialized: function() {
            return !!this.getPopup() && this.getPopup().IsDOMInitialized();
        },
        reset: function() {
            this.popup = null;
            this.slider = null;
            this.navigationBar = null;
            this.imageSliderWrapper = null;
            this.bottomPanel = null;
            this.navigationBarMarker = null;
            this.fulscreenViewerTextArea = null;

            this.closeButtonWrapper = null;
            this.playPauseButtonWrapper = null;
            this.nextNavigationButton = null;
            this.prevNavigationButton = null;
        },
        show: function(index, skipEvent) {
            if(!skipEvent && this.owner.raiseFullscreenViewerShowing(index))
                return;
            if(ASPx.Browser.WebKitTouchUI) {
                window.scrollTo(0, 0);
                ASPx.Attr.ChangeStyleAttribute(this.owner.GetContentCell(), "display", "none");
            }
            this.popup.Show();
            this.setActiveItemIndex(index, true);
            this.updateHash();
            this.updateFullscreenViewerText();
            this.updateNavigationButtonsState(true);

            if(this.canHandleMouseOverForBottomPanel)
                this.slideOutNavigationBar(true, true);
            window.setTimeout(function() {
                this.adjustImagePanel();
            }.aspxBind(this), 0);
        },
        hide: function() {
            this.popup.Hide();
            this.clearHash();
            this.pauseSlideShow();
            this.hideElements(); //TODO
        },
        updateFullscreenViewerText: function() {
            var textArea = this.getFullscreenViewerTextArea();
            if(textArea) {
                if(this.hasItemFVTextTemplate) {
                    var activeItemIndex = this.getActiveItemIndex();
                    if(this.lastActiveItemIndex < this.getItemCount())
                        ASPx.SetStyles(textArea.children[this.lastActiveItemIndex], { display: "" });
                    ASPx.SetStyles(textArea.children[activeItemIndex], { display: "block" });
                    this.lastActiveItemIndex = activeItemIndex;
                } else if(!this.hasFVTextTemplate)
                    textArea.innerHTML = this.slider.GetActiveItem().text || "";
            }
        },
        getActiveItemIndex: function() {
            return this.slider.GetActiveItemIndex();
        },
        setActiveItemIndex: function(index, preventAnimation) {
            this.slider.SetActiveItemIndex(index, preventAnimation);
            this.slider.Focus();
        },
        setPlayPauseButtonState: function(play) {
            if(this.getPlayPauseButtonWrapper()) {
                ASPx.SetElementDisplay(this.getPlayPauseButtonWrapper().children[0], play);
                ASPx.SetElementDisplay(this.getPlayPauseButtonWrapper().children[1], !play);
            }
        },
        isVisible: function() {
            return !!this.popup && this.popup.IsVisible();
        },
        changeElementsVisibility: function() {
            if(this.canHandleMouseOverForButtons || this.canHandleMouseOverForBottomPanel) {
                this.elementsVisible = !this.elementsVisible;
                var opacity = this.elementsVisible ? 1 : this.initialOpacity;
                if(this.canHandleMouseOverForButtons) {
                    if(this.getCloseButtonWrapper())
                        ASPx.AnimationHelper.fadeTo(this.getCloseButtonWrapper(), { to: opacity });
                    if(this.getPlayPauseButtonWrapper())
                        ASPx.AnimationHelper.fadeTo(this.getPlayPauseButtonWrapper(), { to: opacity });
                    if(this.prevNavigationButton)
                        ASPx.AnimationHelper.fadeTo(this.prevNavigationButton.getButtonArea(), { to: opacity });
                    if(this.nextNavigationButton)
                        ASPx.AnimationHelper.fadeTo(this.nextNavigationButton.getButtonArea(), { to: opacity });
                }
                if(ASPx.Browser.TouchUI && this.canHandleMouseOverForBottomPanel) {
                    if(this.elementsVisible)
                        this.onBottomPanelMouseIn();
                    else
                        this.onBottomPanelMouseOut();
                }
            }
        },
        hideElements: function() {
            if(ASPx.Browser.TouchUI && (this.canHandleMouseOverForButtons || this.canHandleMouseOverForBottomPanel)) {
                if(this.canHandleMouseOverForButtons) {
                    if(this.getCloseButtonWrapper())
                        ASPx.SetElementOpacity(this.getCloseButtonWrapper(), this.initialOpacity);
                    if(this.getPlayPauseButtonWrapper())
                        ASPx.SetElementOpacity(this.getPlayPauseButtonWrapper(), this.initialOpacity);
                    ASPx.SetElementOpacity(this.prevNavigationButton.getButtonArea(), this.initialOpacity);
                    ASPx.SetElementOpacity(this.nextNavigationButton.getButtonArea(), this.initialOpacity);
                }
                if(this.canHandleMouseOverForBottomPanel)
                    this.onBottomPanelMouseOut(true);
                this.elementsVisible = false;
            }
        },
        fadeInTextArea: function() {
            var textArea = this.getFullscreenViewerTextArea();
            if(textArea && this.canHandleMouseOverForBottomPanel)
                ASPx.AnimationHelper.fadeTo(textArea, { to: 1 });
        },
        doSetPrevItemIndex: function() {
            var index = this.getActiveItemIndex() - 1;
            if(index >= 0)
                this.setActiveItemIndex(index);
        },
        doSetNextItemIndex: function() {
            var index = this.getActiveItemIndex() + 1;
            if(index < this.getItemCount())
                this.setActiveItemIndex(index);
        },
        playSlideShow: function() {
            this.slider.Play();
            this.setPlayPauseButtonState(false);
        },
        pauseSlideShow: function() {
            this.slider.Pause();
            this.setPlayPauseButtonState(true);
        },
        animateElements: function() {
            if(ASPx.Browser.TouchUI || !this.canHandleMouseOverForButtons)
                return;
            if(!this.elementsVisible)
                this.changeElementsVisibility();
            ASPx.Timer.ClearTimer(this.disappearElementsTimerId);
            this.disappearElementsTimerId = window.setTimeout(function() {
                this.changeElementsVisibility();
            }.aspxBind(this), 2000);
        },
        fadeOutTextArea: function(preventAnimation) {
            var textArea = this.getFullscreenViewerTextArea();
            if(textArea && this.canHandleMouseOverForBottomPanel) {
                if(preventAnimation)
                    ASPx.SetElementOpacity(textArea, Constants.InitialFadedOpacityValue);
                else
                    ASPx.AnimationHelper.fadeTo(textArea, { to: Constants.InitialFadedOpacityValue });
            }
        },
        updateHash: function() {
            if(this.useHash) {
                var newHash = Constants.Hash + this.getActiveItemIndex();
                if(window.location.hash != newHash) {
                    var newUrl = location.pathname + location.search + newHash;
                    if(history.replaceState)
                        history.replaceState("", "", newUrl)
                    else
                        location.replace(newUrl);
                }
            }
        },
        clearHash: function() {
            if(this.useHash) {
                if(history.replaceState)
                    history.replaceState("", "", location.pathname + location.search);
                else
                    location.replace(location.pathname + location.search + "#");
            }
        },
        updateNavigationButtonsState: function(force) {
            if(this.nextNavigationButton)
                this.nextNavigationButton.updateEnabled(force);
            if(this.prevNavigationButton)
                this.prevNavigationButton.updateEnabled(force);
        },
        getItemCount: function() {
            return this.slider.GetItemCount();
        },
        setNavigationBarMarkerVisibility: function(value) {
            var marker = this.getNavigationBarMarker();
            if(marker)
                ASPx.SetElementDisplay(marker, value);
        },
        slideInNavigationBar: function(preventAnimation) {
            if(this.canHandleMouseOverForBottomPanel) {
                this.setNavigationBarMarkerVisibility(false);
                ASPx.AnimationHelper.slideTo(this.getNavigationBar().GetMainElement(), {
                    to: 0,
                    direction: ASPx.AnimationHelper.SLIDE_VERTICAL_DIRECTION,
                    duration: Constants.SlideAnimationDuration
                });
            }
        },
        slideOutNavigationBar: function(preventAnimation, preventMarker) {
            if(this.canHandleMouseOverForBottomPanel) {
                if(!preventMarker)
                    this.setNavigationBarMarkerVisibility(true);
                var element = this.getNavigationBar().GetMainElement();
                if(preventAnimation)
                    ASPx.AnimationUtils.SetTransformValue(element, this.getNavigationBarHeight(), true);
                else
                    ASPx.AnimationHelper.slideTo(element, {
                        to: this.getNavigationBarHeight(),
                        direction: ASPx.AnimationHelper.SLIDE_VERTICAL_DIRECTION,
                        duration: Constants.SlideAnimationDuration
                    });
            }
        },
        getNavigationBarHeight: function() {
            return this.getNavigationBar().GetMainElement().offsetHeight;
        },
        /* Prepare */
        prepareImagePanel: function() {
            var element = this.getImageSliderWrapper();
            var style = ASPx.GetCurrentStyle(element);
            this.contentPaddings = {
                paddingLeft: ASPx.PxToInt(style.paddingLeft),
                paddingTop: ASPx.PxToInt(style.paddingTop),
                paddingRight: ASPx.PxToInt(style.paddingRight),
                paddingBottom: ASPx.PxToInt(style.paddingBottom)
            };
            ASPx.SetStyles(element, { padding: 0 });
        },
        prepareBottomPanel: function() {
            var navigationBar = this.getNavigationBar();
            if(navigationBar)
                navigationBar.disableSelectedStateAnimation = true;
            if(this.getNavigationBarMarker()) {
                this.getOverflowPanel().className += " dxigOPWM";
                ASPx.SetStyles(navigationBar.GetMainElement(), { zIndex: "1" });
            }
            this.fadeOutTextArea(true);
        },
        prepareNavigationButtons: function() {
            if(this.nextNavigationButton)
                this.nextNavigationButton.prepare();
            if(this.prevNavigationButton)
                this.prevNavigationButton.prepare();
        },
        preparePlayPauseButton: function() {
            var playPauseWrapper = this.getPlayPauseButtonWrapper();
            if(playPauseWrapper) {
                if(this.canHandleMouseOverForButtons)
                    ASPx.SetElementOpacity(playPauseWrapper, this.initialOpacity);
                this.setPlayPauseButtonState(true);
            }
        },
        prepareCloseButtonWrapper: function() {
            var closeButtonWrapper = this.getCloseButtonWrapper();
            if(closeButtonWrapper && this.canHandleMouseOverForButtons)
                ASPx.SetElementOpacity(closeButtonWrapper, this.initialOpacity);
        },
        /* HTML elements API */
        getPlayPauseButtonWrapper: function() {
            if(!this.playPauseButtonWrapper)
                this.playPauseButtonWrapper = this.getElementByClassName(CssClassesConstants.PlayPauseButtonWrapperClassName);
            return this.playPauseButtonWrapper;
        },
        getCloseButtonWrapper: function() {
            if(!this.closeButtonWrapper) {
                var closeButton = this.getCloseButton();
                if(closeButton)
                    this.closeButtonWrapper = closeButton.parentNode;
            }
            return this.closeButtonWrapper;
        },
        getCloseButton: function() {
            return ASPx.GetElementById(this.popup.name + ControlIDConstants.CloseButton);
        },
        getImageSliderWrapper: function() {
            if(!this.imageSliderWrapper)
                this.imageSliderWrapper = this.getElementByClassName(CssClassesConstants.ImageSliderWrapperClassName);
            return this.imageSliderWrapper;
        },
        getBottomPanel: function() {
            if(!this.bottomPanel)
                this.bottomPanel = this.getElementByClassName(CssClassesConstants.BottomPanelClassName);
            return this.bottomPanel;
        },
        getOverflowPanel: function() {
            if(!this.overflowPanel)
                this.overflowPanel = this.getElementByClassName(CssClassesConstants.OverflowPanelClassName);
            return this.overflowPanel;
        },
        getNavigationBar: function() {
            if(!this.navigationBar)
                this.navigationBar = ASPx.GetControlCollection().Get(this.popup.name + ControlIDConstants.NavigationBar);
            return this.navigationBar;
        },
        getNavigationBarMarker: function() {
            if(!ASPx.Browser.TouchUI && !this.navigationBarMarker)
                this.navigationBarMarker = this.getElementByClassName(CssClassesConstants.NavigationBarMarkerClassName);
            return this.navigationBarMarker;
        },
        getFullscreenViewerTextArea: function() {
            if(!this.fulscreenViewerTextArea)
                this.fulscreenViewerTextArea = this.getElementByClassName(CssClassesConstants.FullscreenViewerTextAreaClassName);
            return this.fulscreenViewerTextArea;
        },
        /* Adjust elements */
        adjustImagePanel: function() {
            this.adjustPopupContent();
            this.adjustNavigationButtons();
            this.slider.AdjustControl();
        },
        adjustBottomPanel: function() {
            var navigationBar = this.getNavigationBar();
            if(navigationBar)
                navigationBar.AdjustControl();
        },
        adjustPopupContent: function() {
            var wrapper = this.getImageSliderWrapper();
            var popupContentElement = wrapper.parentNode.parentNode;
            var navBarHeight = this.isAlwaysVisibleNavBar ? this.getNavigationBarHeight() : 0;
            var style = {
                width: popupContentElement.clientWidth - this.contentPaddings.paddingLeft - this.contentPaddings.paddingRight,
                height: popupContentElement.clientHeight - this.contentPaddings.paddingTop - this.contentPaddings.paddingBottom - navBarHeight,
                marginTop: this.contentPaddings.paddingTop
            };
            if(this.rtl)
                style.marginRight = this.contentPaddings.paddingLeft;
            else
                style.marginLeft = this.contentPaddings.paddingLeft;
            ASPx.SetStyles(wrapper, style);
            ASPx.SetStyles(this.getBottomPanel(), { width: "", height: "" });
        },
        adjustNavigationButtons: function() {
            if(this.prevNavigationButton)
                this.prevNavigationButton.adjust();
            if(this.nextNavigationButton)
                this.nextNavigationButton.adjust();
        },
        /* Assign handlers */
        assignHandlersToPopup: function() {
            var popup = this.popup;
            if(popup) {
                if(!ASPx.Browser.TouchUI) {
                    ASPx.Evt.AttachEventToElement(popup.GetWindowElement(-1), "mousemove", function(evt) { this.onPopupElementMouseMove(evt); }.aspxBind(this));
                    if(this.keyboardSupport) {
                        popup.Shown.AddHandler(function() { this.slider.Focus(); }.aspxBind(this));
                        popup.Closing.AddHandler(function() { 
                            this.clearHash();
                            this.pauseSlideShow();
                        }.aspxBind(this));
                    }
                }
                if(ASPx.Browser.MacOSMobilePlatform) {
                    popup.Shown.AddHandler(function(s, e) { s.preventParentOverlowOnIos(-1); }.aspxBind(this));
                    popup.CloseUp.AddHandler(function(s, e) { s.restoreParentOverflowOnIos(-1); }.aspxBind(this));
                }
                if(ASPx.Browser.WebKitTouchUI)
                    popup.CloseUp.AddHandler(function(s, e) { ASPx.Attr.RestoreStyleAttribute(this.owner.GetContentCell(), "display"); }.aspxBind(this));
                popup.BeforeResizing.AddHandler(function() { 
                    ASPx.SetStyles(this.getImageSliderWrapper(), { width: 1, height: 1 });
                    if(this.prevNavigationButton)
                        ASPx.SetStyles(this.prevNavigationButton.getButtonArea(), { height: 1 });
                    if(this.nextNavigationButton)
                        ASPx.SetStyles(this.nextNavigationButton.getButtonArea(), { height: 1 });
                    ASPx.SetStyles(this.getBottomPanel(), { width: 1, height: 1 });
                }.aspxBind(this));
                popup.AfterResizing.AddHandler(function() {
                    this.adjustImagePanel();
                    this.adjustBottomPanel();
                }.aspxBind(this));
            }
        },
        assignHandlersToImagePanel: function() {
            this.assignHandlersToImageSlider();
            this.assignHandlersToPlayPauseButton();
            this.assignHandlersToCloseButtonWrapper();
        },
        assignHandlersToBottomPanel: function() {
            if(!ASPx.Browser.TouchUI && this.canHandleMouseOverForBottomPanel) {
                ASPx.Evt.AttachMouseEnterToElement(this.getElementByClassName(CssClassesConstants.BottomPanelClassName),
                    function() { this.onBottomPanelMouseIn(); }.aspxBind(this),
                    function() { this.onBottomPanelMouseOut(); }.aspxBind(this)
                );
            }
            this.assignHandlersToNavigationBar();
        },
        assignHandlersToNavigationBar: function() {
            var navigationBar = this.getNavigationBar();
            if(navigationBar)
                navigationBar.ActiveItemChanged.AddHandler(function(s, e) { this.slider.SetActiveItemIndex(e.item.index); }.aspxBind(this));
        },
        assignHandlersToImageSlider: function() {
            this.slider.ActiveItemChanged.AddHandler(function(s, e) { this.onImageSliderActiveItemChanged(s, e); }.aspxBind(this));
        },
        assignHandlersToPlayPauseButton: function() {
            var playPauseButtonWrapper = this.getPlayPauseButtonWrapper();
            if(playPauseButtonWrapper)
                ASPx.Evt.AttachEventToElement(playPauseButtonWrapper, this.clickEventName, function(evt) {
                    if(!this.canPreventMouseDownEvent())
                        this.onPlayPauseButtonMouseDown(evt);
                }.aspxBind(this));
        },
        assignHandlersToCloseButtonWrapper: function() {
            var closeButtonWrapper = this.getCloseButtonWrapper();
            if(closeButtonWrapper)
                ASPx.Evt.AttachEventToElement(closeButtonWrapper, this.clickEventName, function(evt) {
                    if(!this.canPreventMouseDownEvent())
                        this.onCloseButtonMouseDown(evt)
                }.aspxBind(this));
        },
        /* Handlers */
        onPopupElementMouseMove: function(evt) {
            if(!this.isVisible())
                return;
            if(ASPx.Browser.IE && this.slider.IsSlideShowPlaying()) {
                var x = ASPx.Evt.GetEventX(evt);
                var y = ASPx.Evt.GetEventY(evt);
                if(this.lastMouseX == x && this.lastMouseY == y)
                    return;
                this.lastMouseX = x;
                this.lastMouseY = y;
            }
            this.animateElements();
        },
        onHashChange: function() {
            if(!this.enabled || !this.isVisible())
                return;
            var index = this.owner.getIndexFromString(window.location);
            if(index != -1)
                this.setActiveItemIndex(index);
        },
        onPlayPauseButtonMouseDown: function(evt) {
            this.animateElements();
            if(this.slider.IsSlideShowPlaying())
                this.pauseSlideShow();
            else
                this.playSlideShow();
            ASPx.Evt.PreventEvent(evt);
        },
        onCloseButtonMouseDown: function(evt) {
            this.hide();
            ASPx.Evt.PreventEvent(evt);
        },
        onNavigationButtonMouseDown: function(evt, isNext) {
            this.animateElements();
            if(isNext)
                this.doSetNextItemIndex();
            else
                this.doSetPrevItemIndex();
            ASPx.Evt.PreventEvent(evt);
        },
        onImageSliderActiveItemChanged: function(s, e) {
            var navigationBar = this.getNavigationBar();
            if(navigationBar)
                navigationBar.SetActiveItemIndex(e.item.index);
            this.updateHash();

            this.updateFullscreenViewerText();
            this.updateNavigationButtonsState();

            this.owner.raiseFullscreenViewerActiveItemIndexChanged();
        },
        onImageSliderItemClick: function() {
            if(ASPx.Browser.TouchUI)
                this.changeElementsVisibility();
            else if(this.enablePagingByClick)
                this.doSetNextItemIndex();
        },
        onBottomPanelMouseIn: function() {
            if(this.slider.IsAdjusted()) {
                this.slideInNavigationBar();
                this.fadeInTextArea();
            }
        },
        onBottomPanelMouseOut: function(preventAnimation) {
            this.slideOutNavigationBar(preventAnimation);
            this.fadeOutTextArea(preventAnimation);
        },
        /* Utils */
        getElementByClassName: function(className) {
            var elements = ASPx.GetNodesByClassName(this.owner.GetMainElement(), className);
            return elements.length > 0 ? elements[0] : null;
        }
    });
    var DataContainer = ASPx.CreateClass(null, {
        constructor: function(owner) {
            this.owner = owner;
            this.dataItems = [];
        },
        clearDataItems: function() {
            this.dataItems = [];    
        },
        hasSliderItems: function() {
            var fullscreenViewer = this.owner.fullscreenViewer;
            return fullscreenViewer && fullscreenViewer.slider && fullscreenViewer.slider.IsDOMInitialized();  
        },
        getSliderItems: function() {
            var fullscreenViewer = this.owner.fullscreenViewer;
            return this.hasSliderItems() ? fullscreenViewer.slider.items : null;
        },
        createDataItems: function() {
            var galleryData = this.owner.items;
            var sliderData = this.getSliderItems();
            var result = [];
            for (var i = 0; i < galleryData.length; i++) {
                var galleryItem = galleryData[i];
                var sliderItem = sliderData ? sliderData[galleryItem.i] : {};
                var existItem = this.dataItems[i];
                result[i] = {
                    index: galleryItem.i,
                    name: galleryItem.n || sliderItem.n,
                    isProcessed: existItem && existItem.index == galleryItem.i
                };
            }
            this.dataItems = result;
        },
        getItemName: function(index) {
            if(this.hasSliderItems())
                return this.getSliderItems()[index].n;
            else if(this.dataItems[index])
                return this.dataItems[index].name;
        },
        getItemCount: function() {
            return this.dataItems.length;
        },
        isItemProcessed: function(index) {
            var item = this.dataItems[index];
            var tmp = item.isProcessed;
            item.isProcessed = true;
            return tmp;
        }
    });
    var LayoutItemProcessorBase = ASPx.CreateClass(null, {
        constructor: function(owner) {
            this.owner = owner;
            this.items = this.getItems();
        },
        getItems: function() {
        },
        createItem: function(index, itemIndex) {
            var item = this.items[index];
            var hyperLink = ASPx.GetNodeByTagName(this.items[index], "A", 0);
            var textElement = ASPx.GetNodeByClassName(this.items[index], CssClassesConstants.ThumbnailTextAreaClassName);
            var width = ASPx.PxToInt(item.style.width);
            var height = ASPx.PxToInt(item.style.height);
            var image = ASPx.GetNodeByClassName(this.items[index], "dxig-img");
            var prepareImgFunc = function() {
                ASPx.ImageControlUtils.ResizeImage(image, {
                    width: width,
                    height: height,
                    sizeMode: this.owner.thumbImgSizeMode,
                    onEndResize: function(element, isCanvas) {
                        if(!isCanvas)
                            element.onload = null;
                        element.style.visibility = "visible";
                        ASPx.ImageControlUtils.RemoveLoadingGif(item);
                    }
                });
            }.aspxBind(this);
            if(ASPx.ImageControlUtils.IsImageLoaded(image))
                prepareImgFunc();
            else
                image.onload = prepareImgFunc;
            var borderElement = ASPx.GetNodeByClassName(item, CssClassesConstants.ThumbnailBorderClassName);
            ASPx.SetStyles(borderElement, { width: width, height: height, display: "block" });
            ASPx.SetStyles(borderElement, { width: width - (borderElement.offsetWidth - width), height: height - (borderElement.offsetHeight - height) });
            if(hyperLink.parentNode.className == CssClassesConstants.ThumbnailWrapperClassName)
                ASPx.SetStyles(hyperLink.parentNode, { width: width, height: height });
            this.prepareTextElement(textElement, this.owner.thumbTxtVisibility, this.owner.allowExpandText);

            if(this.owner.enabled) {
                this.assignHandlersToHyperLink(hyperLink, itemIndex, this.owner.useHash);
                this.assignHandlersToTextElement(textElement, item, this.owner.thumbTxtVisibility, this.owner.allowExpandText);
            }
        },
        /* Assign Handlers */
        assignHandlersToHyperLink: function(hyperLink, index, useHash) {
            if(!!ASPx.Attr.GetAttribute(hyperLink, "href"))
                return;
            if(useHash) {
                ASPx.Attr.SetAttribute(hyperLink, "target", "_blank");
                ASPx.Attr.SetAttribute(hyperLink, "href", Constants.Hash + index);
                if(!ASPx.Browser.TouchUI)
                    ASPx.Evt.AttachEventToElement(hyperLink, ASPx.TouchUIHelper.touchMouseDownEventName, function(evt) {
                        this.onHyperLinkMouseDown(evt);
                    }.aspxBind(this.owner));
                ASPx.Evt.AttachEventToElement(hyperLink, "click", function(evt) {
                    this.onHyperLinkClick(evt);
                }.aspxBind(this.owner));
            } else {
                ASPx.SetStyles(hyperLink, { cursor: "pointer" });
                ASPx.Evt.AttachEventToElement(hyperLink, "click", function() {
                    this.ShowFullscreenViewer(index);
                }.aspxBind(this.owner));
            }
        },
        assignHandlersToTextElement: function(textElement, item, elementVisibility, allowExpandText) {
            if(textElement) {
                if(elementVisibility != ElementVisibilityModeEnum.Always) {
                    var opacity = Constants.InitialOpacityValue;
                    if(elementVisibility == ElementVisibilityModeEnum.Faded)
                        opacity = Constants.InitialFadedOpacityValue;
                    ASPx.Evt.AttachMouseEnterToElement(item,
                        function(element) { ASPx.AnimationHelper.fadeTo(textElement, { to: 1 }); },
                        function(element) { ASPx.AnimationHelper.fadeTo(textElement, { to: opacity }); }
                    );
                }
                if(allowExpandText)
                    ASPx.Evt.AttachEventToElement(textElement, "click", function() {
                        if(textElement.dxIsExpanded)
                            textElement.className = textElement.className.replace(CssClassesConstants.ExpandedText, "");
                        else
                            textElement.className += CssClassesConstants.ExpandedText;
                        textElement.dxIsExpanded = !textElement.dxIsExpanded;
                    }.aspxBind(this));
            }
        },
        assignMouseEnterHandler: function(textElement, item, elementVisibility) {
            if(elementVisibility == ElementVisibilityModeEnum.Always)
                return;
            var opacity = Constants.InitialOpacityValue;
            if(elementVisibility == ElementVisibilityModeEnum.Faded)
                opacity = Constants.InitialFadedOpacityValue;
            ASPx.Evt.AttachMouseEnterToElement(item,
                function(element) { ASPx.AnimationHelper.fadeTo(textElement, { to: 1 }); },
                function(element) { ASPx.AnimationHelper.fadeTo(textElement, { to: opacity }); }
            );
        },
        prepareTextElement: function(textElement, elementVisibility, allowExpandText) {
            if(textElement) {
                var style = { display: "block" };
                if(allowExpandText)
                    style.cursor = "default";
                if(elementVisibility != ElementVisibilityModeEnum.Always) {
                    style.opacity = Constants.InitialOpacityValue;
                    if(elementVisibility == ElementVisibilityModeEnum.Faded)
                        style.opacity = Constants.InitialFadedOpacityValue;
                }
                ASPx.SetStyles(textElement, style);
            }
        }
    });
    var FlowLayoutItemProcessor = ASPx.CreateClass(LayoutItemProcessorBase, {
        constructor: function(owner) {
            this.constructor.prototype.constructor.call(this, owner);
        },
        getItems: function() {
            var table = ASPx.GetNodeByTagName(this.owner.GetItemsCell(), "table", 0);
            var tbody = ASPx.GetNodeByTagName(table, "tbody", 0);
            var tr = ASPx.GetNodeByTagName(tbody, "tr", 0);
            return ASPx.GetNodeByTagName(tr, "td", 0).childNodes;
        }
    });
    var TableLayoutItemProcessor = ASPx.CreateClass(LayoutItemProcessorBase, {
        constructor: function(owner) {
            this.constructor.prototype.constructor.call(this, owner);
        },
        getItems: function() {
            var items = ASPx.GetNodesByClassName(this.owner.GetItemsCell(), CssClassesConstants.ThumbnailWrapperClassName);
            for (var i = 0; i < items.length; i++)
                items[i] = items[i].parentNode;
            return items;
        }
    });
    var ASPxClientImageGallery = ASPx.CreateClass(ASPxClientDataView, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            // from server
            this.keyboardSupport = true;
            this.enablePagingByClick = true;
            this.useHash = true;
            this.navBarVisibility = 2;
            this.navBtnVisibility = 2;
            this.thumbTxtVisibility = 2;
            this.thumbImgSizeMode = 2;
            this.allowExpandText = true;
            this.items = [];
            // templates
            this.hasFVTextTemplate = false;
            this.hasItemFVTextTemplate = false;

            this.lastActiveItemIndex = 0;
            this.isLeftButtonPressed = false;
            this.isFlowLayout = false;

            this.needUpdateFullscreenViewer = false;
            this.fullscreenViewer = null;
            this.dataContainer = null;
            this.FullscreenViewerShowing = new ASPxClientEvent();
            this.FullscreenViewerActiveItemIndexChanged = new ASPxClientEvent();
        },
        InlineInitialize: function() {
            ASPxClientDataView.prototype.InlineInitialize.call(this);
            this.fullscreenViewer = new FullscreenViewer(this);
            this.createImages();
        },
        Initialize: function() {
            ASPxClientDataView.prototype.Initialize.call(this);
            if(this.canUseFullscreenViewer())
                this.fullscreenViewer.initialize();
        },
        canUseFullscreenViewer: function() {
            return !!ASPx.GetControlCollection().Get(this.name + ControlIDConstants.Popup);    
        },
        createImages: function() {
            var dataContainer = this.getDataContainer();
            dataContainer.createDataItems();
            var length = dataContainer.getItemCount();
            if(length == 0)
                return;
            var itemProcessor = this.isFlowLayout ? new FlowLayoutItemProcessor(this) : new TableLayoutItemProcessor(this);
            for (var i = 0; i < length; i++) {
                if(!dataContainer.isItemProcessed(i) && dataContainer.dataItems[i])
                    itemProcessor.createItem(i, dataContainer.dataItems[i].index);
            }
        },
        getDataContainer: function() {
            if(!this.dataContainer)
                this.dataContainer = new DataContainer(this);
            return this.dataContainer;
        },
        /* Events */
        raiseFullscreenViewerActiveItemIndexChanged: function() {
            if(!this.FullscreenViewerActiveItemIndexChanged.IsEmpty()) {
                var index = this.GetFullscreenViewerActiveItemIndex();
                var args = new ASPxClientImageGalleryFullscreenViewerEventArgs(index, this.getDataContainer().getItemName(index));
                this.FullscreenViewerActiveItemIndexChanged.FireEvent(this, args)
            }
        },
        raiseFullscreenViewerShowing: function(index) {
            var cancel = false;
            if(!this.FullscreenViewerShowing.IsEmpty()) {
                if(index == undefined)
                    index = this.GetFullscreenViewerActiveItemIndex();
                var args = new ASPxClientImageGalleryCancelEventArgs(index, this.getDataContainer().getItemName(index));
                this.FullscreenViewerShowing.FireEvent(this, args)
                cancel = args.cancel;
            }
            return cancel;
        },
        /* Public API */
        ShowFullscreenViewer: function(index) {
            if(!this.raiseFullscreenViewerShowing(index) && this.canUseFullscreenViewer())
                this.fullscreenViewer.show(index, true);
        },
        HideFullscreenViewer: function() {
            if(this.canUseFullscreenViewer())
                this.fullscreenViewer.hide();
        },
        SetFullscreenViewerActiveItemIndex: function(index, preventAnimation) {
            if(this.canUseFullscreenViewer())
                this.fullscreenViewer.setActiveItemIndex(index, preventAnimation);
        },
        GetFullscreenViewerItemCount: function() {
            if(this.canUseFullscreenViewer())
                return this.fullscreenViewer.getItemCount();
            return 0;
        },
        GetFullscreenViewerActiveItemIndex: function() {
            if(this.canUseFullscreenViewer())
                return this.fullscreenViewer.getActiveItemIndex();
            return 0;
        },
        PlaySlideShow: function() {
            if(this.canUseFullscreenViewer())
                this.fullscreenViewer.playSlideShow();
        },
        PauseSlideShow: function() {
            if(this.canUseFullscreenViewer())
                this.fullscreenViewer.pauseSlideShow();
        },
        /* Callback */
        OnCallback: function(result) {
            var areStatesEqual = this.GetPageIndex() == result.index && this.GetPageSize() == result.size;
            var isEndlessPagingCallback = result.epHtml != undefined;

            ASPxClientDataView.prototype.OnCallback.call(this, result);
            if(ASPx.IsExists(result.fv)) {
                this.needUpdateFullscreenViewer = true;
                ASPx.SetInnerHtml(this.GetChildElement(ControlIDConstants.FVContainerID), result.fv);
            }
            if(result.items) {
                if(this.UseEndlessPaging() && isEndlessPagingCallback) {
                    if(!areStatesEqual)
                        this.items = ASPx.Data.CollectionsUnionToArray(this.items, result.items);
                } else {
                    this.items = result.items;
                    this.getDataContainer().clearDataItems();    
                }
            }
        },
        DoEndCallback: function() {
            if(this.needUpdateFullscreenViewer) {
                this.fullscreenViewer.reset();
                this.fullscreenViewer.prepare();
                if(this.fullscreenViewer.isVisible())
                    this.fullscreenViewer.show(this.fullscreenViewer.getActiveItemIndex(), true);
                this.needUpdateFullscreenViewer = false;
            }
            this.createImages();
            ASPxClientDataView.prototype.DoEndCallback.call(this);
        },
        /* Utils */
        getIndexFromString: function(str) {
            var hash = (new RegExp(Constants.Hash + '[0-9]+')).exec(str);
            if(!hash)
                return -1;
            return parseInt(hash[0].replace(new RegExp(Constants.Hash), ""));
        },
        onHyperLinkMouseDown: function(evt) {
            this.isLeftButtonPressed = ASPx.Evt.IsLeftButtonPressed(evt);
            ASPx.Evt.PreventEvent(evt);
        },
        onHyperLinkClick: function(evt) {
            if(ASPx.Browser.TouchUI || this.isLeftButtonPressed) {
                var hyperLink = ASPx.Evt.GetEventSource(evt).parentNode;
                this.ShowFullscreenViewer(this.getIndexFromString(ASPx.Attr.GetAttribute(hyperLink, "href")));
                ASPx.Evt.PreventEvent(evt);
            }
            this.isLeftButtonPressed = false;
        },
        onImageSliderItemClick: function() {
            if(this.canUseFullscreenViewer())
                this.fullscreenViewer.onImageSliderItemClick();
        }
    });
    ASPxClientImageGallery.Cast = ASPxClientDataView.Cast;
    ASPxClientImageGalleryCancelEventArgs = ASPx.CreateClass(ASPxClientCancelEventArgs, {
        constructor: function(index, name) {
            this.constructor.prototype.constructor.call(this);
            this.index = index;
            this.name = name;
        }
    });
    ASPxClientImageGalleryFullscreenViewerEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(index, name) {
            this.constructor.prototype.constructor.call(this);
            this.index = index;
            this.name = name;
        }
    });

    window.ASPxClientImageGallery = ASPxClientImageGallery;
    window.ASPxClientImageGalleryCancelEventArgs = ASPxClientImageGalleryCancelEventArgs;
    window.ASPxClientImageGalleryFullscreenViewerEventArgs = ASPxClientImageGalleryFullscreenViewerEventArgs;

    ASPx.IGItemClick = function(id) {
        var gallery = ASPx.GetControlCollection().Get(id);
        if(gallery)
            gallery.onImageSliderItemClick();
    }
    ASPx.Evt.AttachEventToElement(window, "hashchange", function() {
        ASPx.GetControlCollection().ForEachControl(function(control) {
            if(control.onHashChange)
                control.onHashChange();
        });
    });
})();