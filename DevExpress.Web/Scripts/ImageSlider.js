/// <reference path="_references.js"/>
/// <reference path="ImageControlUtils.js"/>

(function () {
    var CssClassesConstants = {};
    CssClassesConstants.Prefix = "dxis-";
    CssClassesConstants.OverlayElementCssClass = CssClassesConstants.Prefix + "overlayElement";
    CssClassesConstants.TextAreaCssClassName = CssClassesConstants.Prefix + "itemTextArea";
    CssClassesConstants.ImageAreaSlidePanelCssClassName = CssClassesConstants.Prefix + "slidePanel";
    CssClassesConstants.NavigationBarHoverStateElementCssClassName = CssClassesConstants.Prefix + "nbHoverItem";
    CssClassesConstants.NavigationBarSelectedStateCssClassName = " " + CssClassesConstants.Prefix + "selected";
    CssClassesConstants.NavigationBarHoverStateCssClassName = CssClassesConstants.Prefix + "hover";
    CssClassesConstants.NavigationBarSlidePanelCssClassName = CssClassesConstants.Prefix + "nbSlidePanel";
    CssClassesConstants.NavigationBarSlidePanelWrapperCssClassName = CssClassesConstants.Prefix + "nbSlidePanelWrapper";
    CssClassesConstants.PassePartoutCssClassName = CssClassesConstants.Prefix + "passePartout";
    CssClassesConstants.ItemHyperLinkCssClassName = CssClassesConstants.Prefix + "hl";

    CssClassesConstants.PlayPauseWrapperCssClassName = CssClassesConstants.Prefix + "playPauseBtnWrapper";

    CssClassesConstants.NavigationButtonUpWrapperCssClassName = CssClassesConstants.Prefix + "prevBtnVertWrapper";
    CssClassesConstants.NavigationButtonDownWrapperCssClassName = CssClassesConstants.Prefix + "nextBtnVertWrapper";
    CssClassesConstants.NavigationButtonBackwardWrapperCssClassName = CssClassesConstants.Prefix + "prevBtnHorWrapper";
    CssClassesConstants.NavigationButtonForwardWrapperCssClassName = CssClassesConstants.Prefix + "nextBtnHorWrapper";

    CssClassesConstants.NavigationButtonUpOutsideWrapperCssClassName = CssClassesConstants.Prefix + "prevBtnVertOutsideWrapper";
    CssClassesConstants.NavigationButtonDownOutsideWrapperCssClassName = CssClassesConstants.Prefix + "nextBtnVertOutsideWrapper";
    CssClassesConstants.NavigationButtonBackwardOutsideWrapperCssClassName = CssClassesConstants.Prefix + "prevBtnHorOutsideWrapper";
    CssClassesConstants.NavigationButtonForwardOutsideWrapperCssClassName = CssClassesConstants.Prefix + "nextBtnHorOutsideWrapper";

    CssClassesConstants.NavigationButtonWrapperHoverCssClassPostfix = "Hover";
    CssClassesConstants.NavigationButtonWrapperPressedCssClassPostfix = "Pressed";
    CssClassesConstants.NavigationButtonWrapperDisabledCssClassPostfix = "Disabled";

    CssClassesConstants.ImageZoomNavigatorCssClassName = CssClassesConstants.Prefix + "zoomNavigator";

    /* Enums */
    var LoadModeEnum = {
        AllImages: 0,
        DynamicLoadAndCache: 1,
        DynamicLoad: 2
    };
    var AnimationTypeEnum = {
        Slide: 0,
        Fade: 1,
        None: 2
    };
    var AnimationDirectionEnum = {
        Horizontal: 0,
        Vertical: 1
    };
    var NavigationBarPositionEnum = {
        Bottom: 0,
        Top: 1,
        Left: 2,
        Right: 3
    };
    var NavigationBarModeEnum = {
        Thumbnails: 0,
        Dots: 1
    };
    var ElementVisibilityModeEnum = {
        None: 0,
        Faded: 1,
        OnMouseOver: 2,
        Always: 3
    };
    var NavigationBarPagingModeEnum = {
        Page: 0,
        Single: 1
    };
    var ExtremeItemClickModeEnum = {
        SelectAndSlide: 0,
        Select: 1
    };
    var NavigationBarButtonPositionEnum = {
        Inside: 0,
        Outside: 1
    };
    var ActiveItemChangeActionEnum = {
        Click: 0,
        Hover: 1
    };
    var NavigationButtonVisibilityModeEnum = {
        Auto: 0,
        Always: 1
    };

    var FadedDefaultOpacity = 0.3;

    var SizeUtils = {
        GetClientWidth: function (element, clear) {
            return clear ? ASPx.GetClearClientWidth(element) : element.offsetWidth;
        },
        GetClientHeight: function (element, clear) {
            return clear ? ASPx.GetClearClientHeight(element) : element.offsetHeight;
        }
    };

    /* MouseEnterHelper */
    var MouseEnterHelper = {
        data: [],
        AddHandler: function (element, mouseOver, mouseOut) {
            var dataItem = this.GetDataItem(element);
            if(!dataItem)
                dataItem = this.CreateDataItem(element, mouseOver, mouseOut);
            dataItem.mouseOverHandlers.push(mouseOver);
            dataItem.mouseOutHandlers.push(mouseOut);
        },
        GetDataItem: function (element) {
            for(var i = 0; i < this.data.length; i++) {
                var dataItem = this.data[i];
                if(dataItem && dataItem.element == element)
                    return dataItem;
            }
            return null;
        },
        CreateDataItem: function (element, mouseOver, mouseOut) {
            var dataItem = { element: element, mouseOverHandlers: [mouseOver], mouseOutHandlers: [mouseOut] };
            this.data.push(dataItem);
            ASPx.Evt.AttachMouseEnterToElement(element, MouseEnterHelper.OnMouseOverHandler, MouseEnterHelper.OnMouseOutHandler);
            return dataItem;
        },
        OnMouseOverHandler: function (element) {
            MouseEnterHelper.PerformHandlers(MouseEnterHelper.GetDataItem(element).mouseOverHandlers);
        },
        OnMouseOutHandler: function (element) {
            MouseEnterHelper.PerformHandlers(MouseEnterHelper.GetDataItem(element).mouseOutHandlers);
        },
        PerformHandlers: function (array) {
            for(var i = 0; i < array.length; i++)
                array[i]();
        }
    };

    var ControlBase = ASPx.CreateClass(ASPxClientImageControlBase, {
        constructor: function (imageSlider) {
            this.constructor.prototype.constructor.call(this, imageSlider);
        },
        AdjustControl: function () {
            this.CalculateSize();
        },
        ResetControlStyle: function () {
        },
        CalculateSize: function () {
        },
        InitializeHandlers: function () {
        },
        IsAdjustedSize: function () {
            return this.GetImageSlider().IsAdjusted();
        },
        GetImageSlider: function () {
            return this.control;
        },
        GetClientControlName: function () {
            return this.GetImageSlider().name;
        },
        IsHorizontalNavigation: function () {
            return this.GetImageSlider().navDirection == AnimationDirectionEnum.Horizontal;
        },
        GetAnimationType: function () {
            return this.GetImageSlider().GetAnimationType();
        },
        IsRtl: function (withoutHorizontalNavigation) {
            var value = !!this.GetImageSlider().rtl;
            if(!withoutHorizontalNavigation)
                value = value && this.IsHorizontalNavigation();
            return value;
        },
        /* Utils */
        CreateDiv: function () {
            return document.createElement("DIV");
        },
        GetStateController: function () {
            return typeof (ASPx.GetStateController) != "undefined" ? ASPx.GetStateController() : null;
        },
        PatchElementForMSTouch: function (element) {
            if(ASPx.Browser.MSTouchUI)
                element.className += " " + ASPx.TouchUIHelper.msTouchDraggableClassName;
        }
    });

    var ButtonBase = ASPx.CreateClass(ControlBase, {
        constructor: function (itemsOwner) {
            this.itemsOwner = itemsOwner;
            this.defaultOpacity = 0;
            this.isLeftButtonPressed = false;
            this.visibility = true;
            this.constructor.prototype.constructor.call(this, itemsOwner.GetImageSlider());
        },
        Initialize: function () {
            if(this.GetVisibilityMode() == ElementVisibilityModeEnum.Faded)
                this.defaultOpacity = FadedDefaultOpacity;
        },
        GetOwner: function () {
            return this.itemsOwner;
        },
        GetContainer: function () {
            return this.GetOwner().GetButtonsContainer()
        },
        GetMouseUpElement: function () {
            return this.GetContainer();
        },
        GetVisibilityMode: function () {
            return 0;
        },
        CanHandle: function () {
            return this.GetOwner().CanHandleButtons();
        },
        PrepareControlHierarchy: function () {
            if(this.GetVisibilityMode() == ElementVisibilityModeEnum.Always)
                return;
            var elements = this.GetAnimationElements();
            for(var i = 0; i < elements.length; i++) {
                ASPx.SetStyles(elements[i], { opacity: this.defaultOpacity });
                this.PatchElementForMSTouch(elements[i]);
            }
        },
        /* Handlers */
        InitializeHandlers: function () {
            ASPx.Evt.AttachEventToElement(this.GetMouseUpElement(), ASPx.TouchUIHelper.touchMouseDownEventName, function (evt) {
                this.isLeftButtonPressed = ASPx.Evt.IsLeftButtonPressed(evt);
            }.aspxBind(this));
            ASPx.Evt.AttachEventToElement(this.GetMouseUpElement(), ASPx.Browser.TouchUI ? ASPx.TouchUIHelper.touchMouseUpEventName : "click", function (evt) {
                if(this.visibility && this.isLeftButtonPressed && this.CanHandle())
                    this.OnMouseUp(evt);
            }.aspxBind(this));
            if(!ASPx.Browser.TouchUI && this.GetVisibilityMode() != ElementVisibilityModeEnum.Always)
                MouseEnterHelper.AddHandler(this.GetContainer(), function () { this.Appear() }.aspxBind(this), function () { this.Disappear() }.aspxBind(this));
        },
        OnMouseUp: function (evt) {
        },
        /* Animations */
        Appear: function () {
            this.Animate(1);
        },
        Disappear: function () {
            this.Animate(this.defaultOpacity);
        },
        GetAnimationElements: function () {
            return [];
        },
        Animate: function (value) {
            var elements = this.GetAnimationElements();
            var hasAnimation = this.GetAnimationType() != AnimationTypeEnum.None;
            for(var i = 0; i < elements.length; i++) {
                if(hasAnimation)
                    ASPx.AnimationHelper.fadeTo(elements[i], { to: value });
                else
                    ASPx.SetStyles(elements[i], { opacity: value });
            }
        },
        /* Utils */
        SetInnerHtml: function (element, html) {
            element.innerHTML = html;
        }
    });

    var SlideShowButton = ASPx.CreateClass(ButtonBase, {
        constructor: function (itemsOwner) {
            this.wrapperElement = null;
            this.playButton = null;
            this.pauseButton = null;
            this.itemsOwner = itemsOwner;
            this.constructor.prototype.constructor.call(this, itemsOwner);
        },
        GetMouseUpElement: function () {
            return this.wrapperElement;
        },
        GetAnimationElements: function () {
            return [this.wrapperElement];
        },
        GetVisibilityMode: function () {
            return this.GetImageSlider().playPauseBtnVbl;
        },
        CreateControlHierarchy: function () {
            var slider = this.GetImageSlider();
            this.wrapperElement = this.CreateDiv();
            this.wrapperElement.innerHTML = slider.playBH + slider.pauseBH;
            this.playButton = this.wrapperElement.children[0];
            this.pauseButton = this.wrapperElement.children[1];
            this.GetContainer().appendChild(this.wrapperElement);
        },
        PrepareControlHierarchy: function () {
            ButtonBase.prototype.PrepareControlHierarchy.call(this);
            this.wrapperElement.className = CssClassesConstants.PlayPauseWrapperCssClassName;
            this.SetButtonState(true);
        },
        IsPlaying: function () {
            return this.GetImageSlider().IsSlideShowPlaying();
        },
        OnMouseUp: function (evt) {
            var slider = this.GetImageSlider();
            if(this.IsPlaying())
                slider.Pause()
            else
                slider.Play()
        },
        SetButtonState: function (play) {
            ASPx.SetElementDisplay(this.playButton, play);
            ASPx.SetElementDisplay(this.pauseButton, !play);
        }
    });

    var NavigationButtons = ASPx.CreateClass(ButtonBase, {
        constructor: function (itemsOwner) {
            this.prevButtonEnable = true;
            this.nextButtonEnable = true;
            this.prevButton = null;
            this.nextButton = null;
            this.prevButtonWrapper = null;
            this.nextButtonWrapper = null;
            this.constructor.prototype.constructor.call(this, itemsOwner);
        },
        GetUniqueId: function () {
            return this.GetOwner().GetUniqueId();
        },
        IsEnablePagingByClick: function () {
            return this.GetOwner().IsEnablePagingByClick();
        },
        IsHorizontal: function () {
            return this.GetOwner().IsHorizontalNavigation();
        },
        GetVisibilityMode: function () {
            return this.GetOwner().GetNavigationBtnsVisibilityMode();
        },
        OnPrevButtonMouseUp: function (evt) {
            if(this.prevButtonEnable)
                this.GetOwner().PerformBackward();
            return ASPx.Evt.PreventEvent(evt);
        },
        OnNextButtonMouseUp: function (evt) {
            if(this.nextButtonEnable)
                this.GetOwner().PerformForward();
            return ASPx.Evt.PreventEvent(evt);
        },
        /* Create and prepare hierarchy */
        CreateControlHierarchy: function () {
            this.prevButtonWrapper = this.CreateDiv();
            this.nextButtonWrapper = this.CreateDiv();
            this.SetInnerHtml(this.prevButtonWrapper, this.GetOwner().GetPrevButtonHtml());
            this.SetInnerHtml(this.nextButtonWrapper, this.GetOwner().GetNextButtonHtml());
            this.prevButton = this.prevButtonWrapper.firstChild;
            this.nextButton = this.nextButtonWrapper.firstChild;

            var container = this.GetContainer();
            container.appendChild(this.prevButtonWrapper);
            container.appendChild(this.nextButtonWrapper);
        },
        PrepareControlHierarchy: function () {
            ButtonBase.prototype.PrepareControlHierarchy.call(this);
            this.SetId(this.prevButtonWrapper, "nbwb");
            this.SetId(this.nextButtonWrapper, "nbwf");
            this.PrepareButtonWrapper(this.prevButtonWrapper, this.GetPrevButtonWrapperCssClassName());
            this.PrepareButtonWrapper(this.nextButtonWrapper, this.GetNextButtonWrapperCssClassName());
        },
        GetPrevButtonWrapperCssClassName: function () {
            return this.IsHorizontal() ?
                CssClassesConstants.NavigationButtonBackwardWrapperCssClassName : CssClassesConstants.NavigationButtonUpWrapperCssClassName;
        },
        GetNextButtonWrapperCssClassName: function () {
            return this.IsHorizontal() ?
                    CssClassesConstants.NavigationButtonForwardWrapperCssClassName : CssClassesConstants.NavigationButtonDownWrapperCssClassName;
        },
        PrepareButtonWrapper: function (wrapper, className) {
            wrapper.className = className;
            var stateController = this.GetStateController();
            if(stateController) {
                if(this.IsEnabled()) {
                    stateController.AddHoverItem(wrapper.id, [className + CssClassesConstants.NavigationButtonWrapperHoverCssClassPostfix], [""], [""], null, null, false);
                    stateController.AddPressedItem(wrapper.id, [className + CssClassesConstants.NavigationButtonWrapperPressedCssClassPostfix], [""], [""], null, null, false);
                }
                stateController.AddDisabledItem(wrapper.id, [className + CssClassesConstants.NavigationButtonWrapperDisabledCssClassPostfix], [""], [""], null, null, false);
            }
        },
        SetId: function (element, id) {
            element.id = this.GetClientControlName() + this.GetOwner().GetUniqueId() + id;
        },
        /* Handlers */
        OnMouseUp: function (evt) {
            var value = true;
            var container = this.GetContainer();
            var source = ASPx.Evt.GetEventSource(evt);
            if(this.IsEnablePagingByClick() && !this.IsSlideShowButton(source) && !ASPx.IsInteractiveControl(source)) {
                var containerSize = this.IsHorizontal() ? container.offsetWidth : container.offsetHeight;
                var leftLimitPos = containerSize * 0.35,
                    rightLimitPos = containerSize * 0.65;
                var mousePosInElement = this.IsHorizontal() ? ASPx.Evt.GetEventX(evt) - ASPx.GetAbsolutePositionX(container) : ASPx.Evt.GetEventY(evt) - ASPx.GetAbsolutePositionY(container);
                if(mousePosInElement < leftLimitPos)
                    value = this.OnPrevButtonMouseUp(evt);
                else if(mousePosInElement > rightLimitPos)
                    value = this.OnNextButtonMouseUp(evt);
            }
            else {
                if(source == this.prevButtonWrapper || source == this.prevButton)
                    value = this.OnPrevButtonMouseUp(evt);
                else if(source == this.nextButtonWrapper || source == this.nextButton)
                    value = this.OnNextButtonMouseUp(evt);
            }
            return value;
        },
        IsSlideShowButton: function (element) {
            var className = CssClassesConstants.PlayPauseWrapperCssClassName;
            return element.className == className || element.parentNode.className == className;
        },
        SetEnablePrevButton: function (enable) {
            this.prevButtonEnable = enable;
            var stateController = this.GetStateController();
            if(stateController) {
                stateController.SetElementEnabled(this.prevButtonWrapper, enable);
                stateController.SetElementEnabled(this.prevButton, enable);
            }
        },
        SetEnableNextButton: function (enable) {
            this.nextButtonEnable = enable;
            var stateController = this.GetStateController();
            if(stateController) {
                stateController.SetElementEnabled(this.nextButtonWrapper, enable);
                stateController.SetElementEnabled(this.nextButton, enable);
            }
        },
        SetVisibilityButtons: function (visible) {
            if(this.visibility == visible)
                return;
            this.visibility = visible;
            ASPx.SetElementDisplay(this.prevButtonWrapper, this.visibility);
            ASPx.SetElementDisplay(this.nextButtonWrapper, this.visibility);
        },
        GetVisibilityButtons: function () {
            return this.visibility;
        },
        GetAnimationElements: function () {
            return [this.prevButtonWrapper, this.nextButtonWrapper];
        },
        GetPrevButtonWrapperSize: function () {
            return this.GetButtonSize(this.prevButtonWrapper);
        },
        GetNextButtonWrapperSize: function () {
            return this.GetButtonSize(this.nextButtonWrapper);
        },
        GetButtonSize: function (element) {
            return { width: element.offsetWidth, height: element.offsetHeight };
        }
    });
    var OutsideNavigationButtons = ASPx.CreateClass(NavigationButtons, {
        constructor: function (itemsOwner) {
            this.constructor.prototype.constructor.call(this, itemsOwner);
        },
        GetAnimationElements: function () {
            return [];
        },
        GetPrevButtonWrapperCssClassName: function () {
            return this.IsHorizontal() ?
                CssClassesConstants.NavigationButtonBackwardOutsideWrapperCssClassName : CssClassesConstants.NavigationButtonUpOutsideWrapperCssClassName;
        },
        GetNextButtonWrapperCssClassName: function () {
            return this.IsHorizontal() ?
                    CssClassesConstants.NavigationButtonForwardOutsideWrapperCssClassName : CssClassesConstants.NavigationButtonDownOutsideWrapperCssClassName;
        }
    });
    /* Items owner class */
    var ItemsOwnerBase = ASPx.CreateClass(ControlBase, {
        constructor: function (imageSlider) {
            this.activeItemIndex = 0;
            this.prevItemIndex = -1;
            this.fullItemWidth = 0;
            this.fullItemHeight = 0;
            this.clearItemWidth = 0;
            this.clearItemHeight = 0;
            this.itemElementsManager = null; //instance of ItemElementsManagerBase
            this.navigationButtons = null; // instance of NavigationButtons
            this.slidePanelElement = null;
            this.constructor.prototype.constructor.call(this, imageSlider);
        },
        Initialize: function () {
            this.activeItemIndex = this.GetRtlIndex(this.GetImageSlider().activeItemIndex);
            if(this.IsRtl())
                this.prevItemIndex = this.activeItemIndex + 1;
        },
        GetRtlIndex: function (index) {
            if(this.IsRtl())
                return this.GetItemCount() - 1 - index;
            return index;
        },
        ForEachItem: function (func) {
            var count = this.GetItemCount();
            if(!this.IsRtl()) {
                for(var i = 0; i < count ; i++)
                    func.call(this, i);
            }
            else {
                for(var i = count - 1; i >= 0; i--)
                    func.call(this, i);
            }
        },
        GetUniqueId: function () {
            return "";
        },
        IsAccessibilityCompliant: function () {
            return this.GetImageSlider().accessibilityCompliant;
        },
        /* Data items API */
        GetItemElementsManager: function () {
            if(!this.itemElementsManager)
                this.itemElementsManager = this.CreateItemElementsManager();
            return this.itemElementsManager;
        },
        CreateItemElementsManager: function () {
            return new ItemElementsManagerBase(this);
        },
        GetTemplate: function (index) {
            return this.IsValidItemIndex(index) ? this.GetItem(index).tpl : null;
        },
        GetNavigateUrl: function (index) {
            return this.IsValidItemIndex(index) ? this.GetItem(index).u : null;
        },
        GetImageSrc: function (index) {
            return this.IsValidItemIndex(index) ? this.GetItem(index).s : null;
        },
        GetItemAlternateText: function (index) {
            if(this.IsValidItemIndex(index)) {
                var item = this.GetItem(index);
                return item.t || item.at || "";
            }
            return "";
        },
        GetItem: function (index) {
            return this.GetImageSlider().GetItemInternal(this.GetRtlIndex(index));
        },
        GetItemCount: function () {
            return this.GetImageSlider().GetItemCount();
        },
        /* Items management */
        SetActiveItemIndex: function (index, preventAnimation) {
            if(!this.IsEnabled() || !this.IsValidItemIndex(index) || index == this.GetActiveItemIndex())
                return;
            this.prevItemIndex = this.GetActiveItemIndex();
            this.activeItemIndex = index;
            this.SetActiveItemIndexInternal(index, preventAnimation);
        },
        SetActiveItemIndexInternal: function (index, preventAnimation) {
            this.UpdateNavigationButtonsState();
        },
        GetActiveItemIndex: function () {
            return this.activeItemIndex;
        },
        GetPrevItemIndex: function () {
            return this.prevItemIndex;
        },
        /* Html elements API */
        GetItemElement: function (index) {
            return this.IsValidItemIndex(index) ? this.GetItemElementsManager().GetCollection()[index] : null;
        },
        GetImageElement: function (index) {
            return ASPx.GetChildByTagName(this.GetItemElement(index), "IMG");
        },
        GetCanvasElement: function (index) {
            return ASPx.GetChildByTagName(this.GetItemElement(index), "CANVAS");
        },
        GetHyperLinkElement: function (index) {
            return ASPx.GetChildByClassName(this.GetItemElement(index), CssClassesConstants.ItemHyperLinkCssClassName);
        },
        GetImageContainerElement: function (index) {
            return this.GetHyperLinkElement(index) || this.GetItemElement(index);
        },
        GetItemElementSize: function (clear) {
            return this.IsHorizontalNavigation() ? this.GetItemElementWidth(clear) : this.GetItemElementHeight(clear);
        },
        GetItemElementWidth: function (clear) {
            return clear ? this.clearItemWidth : this.fullItemWidth;
        },
        GetItemElementHeight: function (clear) {
            return clear ? this.clearItemHeight : this.fullItemHeight;
        },
        GetSlidePanelElement: function () {
            return this.slidePanelElement;
        },
        GetItemsContainer: function () {
            return null;
        },
        /* SlideShow */
        StopPlayingWhenPaging: function () {
            this.GetImageSlider().StopPlayingWhenPaging();
        },
        /* Navigation buttons */
        CanCreateNavigationButtons: function () {
            return true;
        },
        CreateNavigationButtons: function () {
            if(this.GetNavigationBtnsVisibilityMode() == ElementVisibilityModeEnum.None)
                return null;
            return new NavigationButtons(this);
        },
        GetNavigationBtnsVisibilityMode: function () {
            return this.GetImageSlider().navBtnsVbl;
        },
        UpdateNavigationButtonsState: function () {
            return false;
        },
        GetButtonsContainer: function () {
            return null;
        },
        GetPrevButtonHtml: function () {
            return "";
        },
        GetNextButtonHtml: function () {
            return "";
        },
        CanHandleButtons: function () {
            return !this.IsExecutedGesture();
        },
        IsExecutedGesture: function () {
            return ASPx.GesturesHelper.IsExecutedGesture();
        },
        IsEnablePagingByClick: function () {
            return false;
        },
        SetEnablePrevButton: function (enabled) {
            if(this.navigationButtons)
                this.navigationButtons.SetEnablePrevButton(enabled);
        },
        SetEnableNextButton: function (enabled) {
            if(this.navigationButtons)
                this.navigationButtons.SetEnableNextButton(enabled);
        },
        SetVisibilityButtons: function (visible) {
            if(this.navigationButtons)
                this.navigationButtons.SetVisibilityButtons(visible);
        },
        PerformBackward: function () {
        },
        PerformForward: function () {
        },
        PerformRollBack: function () {
        },
        GetEnableLoopNavigation: function () {
            return false;
        },
        /* Partial loading images */
        GetImageLoadMode: function () {
            return this.GetImageSlider().imageLoadMode;
        },
        UpdateItemElementsIfRequired: function () {
            this.GetItemElementsManager().UpdateHierarchyIfRequired();
        },
        UpdateItemElementsAfterDelayIfRequired: function () {
            this.GetItemElementsManager().UpdateHierarchyAfterDelayIfRequired();
        },
        /* Navigate urls */
        IsNeedCreateHyperLink: function () {
            return true;
        },
        GetTarget: function () {
            return this.GetImageSlider().target;
        },
        /* Create and prepare hierarchy */
        CreateTemplates: function () {
            this.ForEachItem(function (i) {
                var template = this.GetTemplate(i);
                if(template) {
                    var itemElement = this.GetItemElement(i);
                    var child = itemElement.firstChild ? itemElement.firstChild.cloneNode(true) : null;
                    itemElement.innerHTML = template;
                    if(child)
                        itemElement.appendChild(child);
                }
            });
        },
        CreateControlHierarchy: function () {
            this.CreateControlHierarchyInternal();
            if(this.CanCreateNavigationButtons())
                this.navigationButtons = this.CreateNavigationButtons();
            this.GetItemElementsManager().CreateItemsIfRequired();
        },
        CreateControlHierarchyInternal: function () {
        },
        CreateItemElements: function () {
            this.ForEachItem(function (i) {
                this.GetItemsContainer().appendChild(this.CreateItemElement(i));
            });
        },
        CreateItemElement: function (index) {
            var itemElement = this.CreateDiv();
            if(!this.GetTemplate(index)) {
                var hyperLinkElement = this.CreateHyperLinkElement(index);
                if(hyperLinkElement)
                    itemElement.appendChild(hyperLinkElement);
            }
            return itemElement;
        },
        CreateImageElement: function (index) {
            var image = new Image();
            ASPx.SetElementDisplay(image, false);
            this.AttachLoadEvent(image);
            this.GetImageContainerElement(index).appendChild(image);
            image.dxIndex = index;
            image.dxImageSlider = this;
            image.src = this.GetImageSrc(index);
            image.alt = this.IsAccessibilityCompliant() ? this.GetItemAlternateText(index) : "";
            return image;
        },
        CreateHyperLinkElement: function (index) {
            var url = this.GetNavigateUrl(index);
            if(!this.IsNeedCreateHyperLink() || !url)
                return null;
            var element = document.createElement("A");
            element.target = this.GetTarget();
            element.className = CssClassesConstants.ItemHyperLinkCssClassName;
            element.href = url;
            return element;
        },
        PrepareControlHierarchy: function () {
            this.GetItemElementsManager().PrepareItemsIfRequired();
        },
        PrepareItemElements: function () {
            this.ForEachItem(function (i) {
                this.PrepareItemElement(i);
            });
        },
        PrepareItemElement: function (index) {
            var itemElement = this.GetItemElement(index);
            ASPx.SetStyles(itemElement, {
                className: this.GetItemElementCssClass(),
                cssText: this.GetItemElementStyle()
            });
            if(this.GetTemplate(index))
                ASPx.ImageControlUtils.RemoveLoadingGif(itemElement);
        },
        AdjustControl: function () {
            ControlBase.prototype.AdjustControl.call(this);
            if(this.navigationButtons)
                this.navigationButtons.AdjustControl();
            this.AdjustControlInternal();
            this.SetActiveItemIndexInternal(this.GetActiveItemIndex(), true);
        },
        AdjustControlInternal: function () {
            this.AdjustItemElements();
        },
        AdjustItemElements: function () {
            this.GetItemElementsManager().AdjustItemsIfRequired();
        },
        CalculateSize: function () {
            var fakeElement = this.CreateDiv();
            ASPx.SetStyles(fakeElement, {
                className: this.GetItemElementCssClass(),
                cssText: this.GetItemElementStyle()
            });
            this.CalculateItemElementSize(fakeElement);
            ASPx.RemoveElement(fakeElement);
        },
        CalculateItemElementSize: function (element) {
        },
        AdjustItemElement: function (index) {
            ASPx.SetStyles(this.GetItemElement(index), {
                width: this.GetItemElementWidth(true),
                height: this.GetItemElementHeight(true)
            });
        },
        AdjustImageElement: function (index) {
            if(this.GetTemplate(index) || !this.GetImageSrc(index))
                return;
            var image = this.GetImageElement(index);
            if(!image)
                image = this.CreateImageElement(index);
            else if(ASPx.ImageControlUtils.IsImageLoaded(image))
                this.ResizeImage(image, index);
        },
        GetItemElementCssClass: function () {
            return "";
        },
        GetItemElementStyle: function () {
            return "";
        },
        /* Images processing */
        ResizeImage: function (image, index) {
            ASPx.ImageControlUtils.ResizeImage(image, {
                width: this.GetItemElementWidth(true),
                height: this.GetItemElementHeight(true),
                sizeMode: this.GetImageSizeMode(),
                rtl: this.IsRtl(true),
                onEndResize: function(element, isCanvas) {
                    if(isCanvas)
                        this.PrepareCanvasElement(index);
                }.aspxBind(this),
                canUseCanvas: !this.IsAccessibilityCompliant()
            });
        },
        PrepareCanvasElement: function (index) {
        },
        GetImageSizeMode: function () {
            return this.GetImageSlider().imageSizeMode;
        },
        /* Swipe slide gesture */
        GetSwipeGestureElement: function () {
            return this.GetSlidePanelElement();
        },
        CanCreateSwipeGestureHandler: function () {
            return false;
        },
        CreateSwipeGestureHandler: function () {
            ASPx.GesturesHelper.AddSwipeSlideGestureHandler(
                this.GetClientControlName() + this.GetUniqueId(),
                function () { return this.GetSwipeGestureElement(); }.aspxBind(this),
                this.IsHorizontalNavigation() ? ASPx.AnimationHelper.SLIDE_HORIZONTAL_DIRECTION : ASPx.AnimationHelper.SLIDE_VERTICAL_DIRECTION,
                function (evt) { return this.CanHandleSwipeGesture(evt); }.aspxBind(this),
                function () { this.PerformBackward(); }.aspxBind(this),
                function () { this.PerformForward(); }.aspxBind(this),
                function () { this.PerformRollBack(); }.aspxBind(this)
            );
        },
        CanHandleSwipeGesture: function (evt) {
            return !!ASPx.GetIsParent(this.GetSwipeGestureElement(), ASPx.Evt.GetEventSource(evt));
        },
        /* InitializeHandlers */
        InitializeHandlers: function () {
            if(this.GetImageSlider().allowMouseWheel)
                ASPx.Evt.AttachEventToElement(this.GetItemsContainer(), ASPx.Evt.GetMouseWheelEventName(), function (evt) { this.OnMouseWheel(evt); }.aspxBind(this));
            if(this.CanCreateSwipeGestureHandler() && this.GetImageSlider().enablePagingGestures)
                this.CreateSwipeGestureHandler();
        },
        AttachLoadEvent: function (image) {
            ASPx.Evt.AttachEventToElement(image, "load", _onImageLoad);
        },
        DetachLoadEvents: function (image) {
            ASPx.Evt.DetachEventFromElement(image, "load", _onImageLoad);
        },
        /* Handlers */
        OnMouseWheel: function (evt) {
            if(!this.IsFocused())
                return;
            if(ASPx.Evt.GetWheelDelta(evt) > 0)
                this.PerformBackward();
            else
                this.PerformForward();
            return ASPx.Evt.PreventEvent(evt);
        },
        OnImageLoad: function (image) {
            this.InitializeImage(image);
            this.ResizeImage(image, image.dxIndex);
            ASPx.ImageControlUtils.RemoveLoadingGif(this.GetItemElement(image.dxIndex));
        },
        InitializeImage: function (image) {
            if(image.naturalWidth && image.naturalHeight)
                return;
            image.naturalWidth = image.width;
            image.naturalHeight = image.height;
        },
        /* Css transform animation */
        GetElementTransformPosition: function (element) {
            return ASPx.AnimationUtils.GetTransformValue(element, !this.IsHorizontalNavigation());
        },
        SetElementTransformPosition: function (element, position) {
            ASPx.AnimationUtils.SetTransformValue(element, position, !this.IsHorizontalNavigation());
        },
        /* Public */
        GetWidth: function () {
            return 0;
        },
        GetHeight: function () {
            return 0;
        },
        /* Utils */
        IsFocused: function () {
            return this.GetImageSlider().IsFocused();
        },
        CreateDocumentFragment: function () {
            var fragment = null;
            if(document.createDocumentFragment)
                fragment = document.createDocumentFragment();
            return fragment;
        },
        IsValidItemIndex: function (index) {
            if(index >= 0 && index < this.GetItemCount())
                return true;
            return false;
        }
    });
    function _onImageLoad(evt) {
        var image = evt.srcElement || this;
        if(!image.dxImageSlider)
            return;
        var imageSlider = image.dxImageSlider;
        imageSlider.DetachLoadEvents(image);
        if(!image.parentNode) return;
        imageSlider.OnImageLoad(image);
    }

    /* Animation strategy classes */
    var ImageAreaStrategy = ASPx.CreateClass(ItemsOwnerBase, {
        constructor: function (imageSlider) {
            this.currentItemTextElementOpacity = 0;
            this.defaultItemTextElementOpacity = 0;
            this.touchDeviceElementsVisible = false;
            this.imageAreaStyleSize = { width: "", height: "" };
            this.imageAreaElement = null;
            this.slideShowButton = null; //Instance of SlideShowButton
            this.overlayLinkElement = null;
            this.overlayImageElement = null;

            this.constructor.prototype.constructor.call(this, imageSlider);
        },
        Initialize: function () {
            ItemsOwnerBase.prototype.Initialize.call(this);
            if(this.GetItemTextVisibilityMode() == ElementVisibilityModeEnum.Faded)
                this.defaultItemTextElementOpacity = FadedDefaultOpacity;
            else if(ASPx.Browser.Firefox)
                this.defaultItemTextElementOpacity = 0.01; //B231240
            this.currentItemTextElementOpacity = this.defaultItemTextElementOpacity;
        },
        GetUniqueId: function () {
            return "_ia_";
        },
        /* Items management */
        CreateItemElementsManager: function () {
            return new ImageAreaItemElementsManager(this);
        },
        GetTextTemplate: function (index) {
            return this.GetItem(index).ttpl;
        },
        GetItemText: function (index) {
            return this.IsValidItemIndex(index) ? this.GetItem(index).t : null;
        },
        /* SlideShow */
        SetSlideShowButtonState: function (play) {
            if(this.slideShowButton)
                this.slideShowButton.SetButtonState(play);
        },
        /* Navigation buttons */
        IsEnablePagingByClick: function () {
            return this.GetImageSlider().enablePagingByClick;
        },
        GetButtonsContainer: function () {
            return this.GetImageSlider().GetPassePartoutElement();
        },
        GetPrevButtonHtml: function () {
            return this.GetImageSlider().pbh;
        },
        GetNextButtonHtml: function () {
            return this.GetImageSlider().nbh;
        },
        UpdateNavigationButtonsState: function () {
            if(this.navigationButtons) {
                if(this.GetItemCount() == 1)
                    this.SetVisibilityButtons(false);
                else {
                    var currentItemIndex = this.GetActiveItemIndex();
                    this.SetEnablePrevButton(currentItemIndex != 0);
                    this.SetEnableNextButton(currentItemIndex != this.GetItemCount() - 1);
                }
            }
            return false;
        },
        PerformBackward: function () {
            this.StopPlayingWhenPaging();
            var index = this.GetActiveItemIndex();
            index--;
            if(index < 0 && this.GetEnableLoopNavigation())
                index = this.GetItemCount() - 1;
            this.GetImageSlider().SetActiveItemIndexInternal(index);
        },
        PerformForward: function () {
            this.StopPlayingWhenPaging();
            var index = this.GetActiveItemIndex();
            index++;
            if(index == this.GetItemCount() && this.GetEnableLoopNavigation())
                index = 0;
            this.GetImageSlider().SetActiveItemIndexInternal(index);
        },
        PerformRollBack: function () {
            this.SetActiveItemIndexInternal(this.GetActiveItemIndex(), false);
        },
        /* Create and prepare hierarchy */
        CreateControlHierarchy: function () {
            ItemsOwnerBase.prototype.CreateControlHierarchy.call(this);
            this.CreateOverlayElements();
        },
        CreateTemplates: function () {
            ItemsOwnerBase.prototype.CreateTemplates.call(this);
            this.ForEachItem(function (i) {
                var template = this.GetTextTemplate(i);
                if(template)
                    this.GetItemTextElement(i).innerHTML = template;
            });
        },
        CreateControlHierarchyInternal: function () {
            ItemsOwnerBase.prototype.CreateControlHierarchyInternal.call(this);
            this.CreateImageAreaElement();
            this.CreateSlideShowButton();
        },
        CreateImageAreaElement: function () {
            this.imageAreaElement = document.createElement("DIV");
            this.GetImageSlider().GetPassePartoutElement().appendChild(this.imageAreaElement);
        },
        CreateSlideShowButton: function () {
            if(this.GetImageSlider().playPauseBtnVbl != ElementVisibilityModeEnum.None)
                this.slideShowButton = new SlideShowButton(this);
        },
        CreateOverlayElements: function () {
            var itemsContainer = this.GetItemsContainer();
            if(!itemsContainer)
                return;
            if(this.GetImageSlider().HasNavigateUrls()) {
                this.overlayLinkElement = document.createElement("A");
                itemsContainer.insertBefore(this.overlayLinkElement, itemsContainer.firstChild);
                this.overlayLinkElement.className = CssClassesConstants.OverlayElementCssClass;
                this.overlayLinkElement.target = this.GetTarget();
            }
            this.overlayImageElement = document.createElement("IMG");
            this.overlayImageElement.src = ASPx.EmptyImageUrl;
            this.overlayImageElement.alt = "";
            if(this.overlayLinkElement)
                this.overlayLinkElement.appendChild(this.overlayImageElement);
            else {
                this.overlayImageElement.className = CssClassesConstants.OverlayElementCssClass;
                itemsContainer.insertBefore(this.overlayImageElement, itemsContainer.firstChild);
            }
        },
        UpdateOverlayParameters: function () {
            var activeItemIndex = this.GetActiveItemIndex();
            var currentItem = this.GetItem(activeItemIndex);
            if(this.overlayLinkElement) {
                if(!currentItem.u)
                    this.overlayLinkElement.removeAttribute("href");
                else if(this.overlayLinkElement.href != currentItem.u)
                    this.overlayLinkElement.href = currentItem.u;
            }
            if(this.overlayImageElement) {
                if(currentItem.s)
                    this.overlayImageElement.src = currentItem.s;
                if(currentItem.tpl)
                    this.overlayImageElement.style.display = "none";
                else {
                    this.overlayImageElement.style.display = "";
                    this.overlayImageElement.alt = this.IsAccessibilityCompliant() ? this.GetItemAlternateText(activeItemIndex) : "";
                }
            }
        },
        PrepareControlHierarchy: function () {
            ItemsOwnerBase.prototype.PrepareControlHierarchy.call(this);
            this.UpdateOverlayParameters();
            var imageAreaElement = this.GetImageAreaElement();
            ASPx.SetStyles(imageAreaElement, {
                className: this.GetImageSlider().imgAreaCssCl,
                cssText: this.GetImageSlider().imgAreaStyle
            });
            this.imageAreaStyleSize = {
                width: imageAreaElement.style.width,
                height: imageAreaElement.style.height
            };
            this.GetItemsContainer().style.zIndex = "0";//Q571002
        },
        AdjustControl: function (width, height) {
            this.AdjustImageAreaElement(width, height);
            ItemsOwnerBase.prototype.AdjustControl.call(this);
        },
        AdjustImageAreaElement: function (width, height) {
            var imageAreaElement = this.GetImageAreaElement();
            if(!imageAreaElement.style.width) {
                ASPx.SetStyles(imageAreaElement, { width: width });
                if(imageAreaElement.offsetWidth > width)
                    ASPx.SetStyles(imageAreaElement, { width: width - (imageAreaElement.offsetWidth - width) });
            }
            if(!imageAreaElement.style.height) {
                ASPx.SetStyles(imageAreaElement, { height: height });
                if(imageAreaElement.offsetHeight > height)
                    ASPx.SetStyles(imageAreaElement, { height: height - (imageAreaElement.offsetHeight - height) });
            }
            if(imageAreaElement.offsetHeight < height)
                ASPx.SetStyles(imageAreaElement, { top: (height - imageAreaElement.offsetHeight) / 2 });
        },
        ResetControlStyle: function () {
            var imageAreaElement = this.GetImageAreaElement();
            ASPx.SetStyles(imageAreaElement, this.imageAreaStyleSize);
            ASPx.SetStyles(imageAreaElement, { top: 0 });
        },
        IsNeedCreateHyperLink: function () {
            return false;
        },
        SetActiveItemIndexInternal: function (index, preventAnimation) {
            ItemsOwnerBase.prototype.SetActiveItemIndexInternal.call(this, index, preventAnimation);
            this.UpdateOverlayParameters();
        },
        CalculateItemElementSize: function (fakeElement) {
            var imageArea = this.GetImageAreaElement();
            imageArea.appendChild(fakeElement);
            this.fullItemWidth = SizeUtils.GetClientWidth(imageArea, true);
            this.fullItemHeight = SizeUtils.GetClientHeight(imageArea, true);
            ASPx.SetStyles(fakeElement, {
                width: this.fullItemWidth,
                height: this.fullItemHeight
            });
            this.clearItemWidth = this.fullItemWidth - (SizeUtils.GetClientWidth(fakeElement) - this.fullItemWidth);
            this.clearItemHeight = this.fullItemHeight - (SizeUtils.GetClientHeight(fakeElement) - this.fullItemHeight);
        },
        CreateItemElement: function (index) {
            var itemElement = ItemsOwnerBase.prototype.CreateItemElement.call(this, index);
            var textElement = this.CreateItemTextElement(index);
            if(!textElement)
                return itemElement;
            itemElement.appendChild(textElement);
            return itemElement;
        },
        CreateItemTextElement: function (index) {
            var text = this.GetItemText(index);
            var textTemplate = this.GetTextTemplate(index);
            if(this.GetItemTextVisibilityMode() == ElementVisibilityModeEnum.None || (!text && !textTemplate))
                return null;
            var itemTextElement = this.CreateDiv();
            if(!textTemplate)
                itemTextElement.innerHTML = text;
            ASPx.SetStyles(itemTextElement, { className: this.GetImageSlider().itemTxtCssCl });
            return itemTextElement;
        },
        PrepareItemTextElement: function (index) {
            var itemTextElement = this.GetItemTextElement(index);
            if(itemTextElement) {
                var style = { cssText: this.GetImageSlider().itemTxtStyle, zIndex: 2, className: itemTextElement.className + " dx-wbv" };
                if(this.GetItemTextVisibilityMode() != ElementVisibilityModeEnum.Always)
                    style.opacity = this.currentItemTextElementOpacity;
                ASPx.SetStyles(itemTextElement, style);
            }
        },
        PrepareItemElement: function (index) {
            ItemsOwnerBase.prototype.PrepareItemElement.call(this, index);
            this.PrepareItemTextElement(index);
        },
        GetItemElementCssClass: function () {
            return this.GetImageSlider().itemCssCl;
        },
        GetItemElementStyle: function () {
            return this.GetImageSlider().itemStyle;
        },
        PrepareCanvasElement: function (index) {
            //Q565221
            var canvas = this.GetCanvasElement(index);
            canvas.style.position = "absolute";
            canvas.style.zIndex = "0";
        },
        /* Html elements API */
        GetItemTextElement: function (index) {
            var itemElement = this.GetItemElement(index);
            return itemElement ? ASPx.GetChildByClassName(itemElement, CssClassesConstants.TextAreaCssClassName) : null;
        },
        GetImageAreaElement: function () {
            return this.imageAreaElement;
        },
        GetItemTextVisibilityMode: function () {
            return this.GetImageSlider().itemTxtVbl;
        },
        /* NavigationBar */
        GetNavigationBarStrategy: function () {
            return this.GetImageSlider().GetNavigationBarStrategy();
        },
        /* MouseOver behavior forTouch device */
        HasMouseOverForTouchDevice: function () {
            if(!ASPx.Browser.TouchUI)
                return false;
            return this.HasMouseOverImageAreaNavigationButtons() || this.HasMouseOverNavigationBarNavigationButtons() || this.HasMouseOverTextArea() || this.HasMouseOverSlideShowButton();
        },
        HasMouseOverImageAreaNavigationButtons: function () {
            return this.IsMouseOverOrFaded(this.GetNavigationBtnsVisibilityMode());
        },
        HasMouseOverNavigationBarNavigationButtons: function () {
            var navigationBar = this.GetNavigationBarStrategy();
            if(!navigationBar)
                return false;
            return this.IsMouseOverOrFaded(navigationBar.GetNavigationBtnsVisibilityMode());
        },
        HasMouseOverTextArea: function () {
            return this.IsMouseOverOrFaded(this.GetItemTextVisibilityMode());
        },
        HasMouseOverSlideShowButton: function () {
            return this.IsMouseOverOrFaded(this.GetImageSlider().playPauseBtnVbl);
        },
        IsMouseOverOrFaded: function (visibility) {
            return visibility == ElementVisibilityModeEnum.Faded || visibility == ElementVisibilityModeEnum.OnMouseOver;
        },
        /* Handlers */
        InitializeHandlers: function () {
            ItemsOwnerBase.prototype.InitializeHandlers.call(this);
            var imageSlider = this.GetImageSlider();

            imageSlider.StrategiesCreated.AddHandler(function () {
                if(this.CanHandleClickEvent())
                    ASPx.Evt.AttachEventToElement(this.GetImageAreaElement(), ASPx.Browser.TouchUI ? ASPx.TouchUIHelper.touchMouseUpEventName : "click", function (evt) { this.OnImageAreaClick(evt); }.aspxBind(this));
            }.aspxBind(this));

            if(!ASPx.Browser.TouchUI && this.IsMouseOverOrFaded(this.GetItemTextVisibilityMode()))
                MouseEnterHelper.AddHandler(
                    imageSlider.GetPassePartoutElement(),
                    function () { this.AppearTextElements(); }.aspxBind(this),
                    function () { this.DisappearTextElements(); }.aspxBind(this)
                );
        },
        CanHandleClickEvent: function () {
            var imageSlider = this.GetImageSlider();
            return !imageSlider.ItemClick.IsEmpty() || imageSlider.HasNavigateUrls() || this.HasMouseOverForTouchDevice();
        },
        OnImageAreaClick: function (evt) {
            this.GetImageSlider().RaiseItemClick();
            if(!this.HasMouseOverForTouchDevice())
                return;
            if(this.touchDeviceElementsVisible) {
                if(this.HasMouseOverTextArea())
                    this.DisappearTextElements();
                if(this.HasMouseOverImageAreaNavigationButtons())
                    this.navigationButtons.Disappear();
                if(this.HasMouseOverNavigationBarNavigationButtons())
                    this.GetNavigationBarStrategy().navigationButtons.Disappear();
                if(this.HasMouseOverSlideShowButton())
                    this.slideShowButton.Disappear();
            }
            else {
                if(this.HasMouseOverTextArea())
                    this.AppearTextElements();
                if(this.HasMouseOverImageAreaNavigationButtons())
                    this.navigationButtons.Appear();
                if(this.HasMouseOverNavigationBarNavigationButtons())
                    this.GetNavigationBarStrategy().navigationButtons.Appear();
                if(this.HasMouseOverSlideShowButton())
                    this.slideShowButton.Appear();
            }
            this.touchDeviceElementsVisible = !this.touchDeviceElementsVisible;
        },
        /* Item text elements animations*/
        AppearTextElements: function () {
            this.FadeItemTextElements(1);
        },
        DisappearTextElements: function () {
            this.FadeItemTextElements(this.defaultItemTextElementOpacity);
        },
        FadeItemTextElements: function (value) {
            this.currentItemTextElementOpacity = value;
            var activeItemIndex = this.GetActiveItemIndex();
            this.ForEachItem(function (i) {
                var preventAnimation = true;
                if(i == activeItemIndex && this.GetAnimationType() != AnimationTypeEnum.None)
                    preventAnimation = false;
                this.FadeItemTextElement(i, preventAnimation);
            });
        },
        FadeItemTextElement: function (index, preventAnimation) {
            var textElement = this.GetItemTextElement(index);
            if(!textElement) // Q455843
                return;
            if(preventAnimation)
                ASPx.SetStyles(textElement, { opacity: this.currentItemTextElementOpacity })
            else
                ASPx.AnimationHelper.fadeTo(textElement, { to: this.currentItemTextElementOpacity });
        },
        /* Public */
        GetWidth: function () {
            return SizeUtils.GetClientWidth(this.GetImageAreaElement());
        },
        GetHeight: function () {
            return SizeUtils.GetClientHeight(this.GetImageAreaElement());
        }
    });

    /* Slide ImageArea strategy */
    var SlideImageAreaStrategy = ASPx.CreateClass(ImageAreaStrategy, {
        constructor: function (imageSlider) {
            this.constructor.prototype.constructor.call(this, imageSlider);
        },
        /* Items management */
        SetActiveItemIndexInternal: function (index, preventAnimation) {
            ImageAreaStrategy.prototype.SetActiveItemIndexInternal.call(this, index, preventAnimation);
            this.UpdateItemElementsAfterDelayIfRequired();

            var position = this.GetItemElementPosition(index);
            if(preventAnimation) {
                this.SetElementTransformPosition(this.GetSlidePanelElement(), position);
                this.UpdateItemElementsIfRequired();
            }
            else
                ASPx.AnimationHelper.slideTo(this.GetSlidePanelElement(), {
                    to: position, duration: this.GetImageSlider().animationDuration,
                    direction: this.IsHorizontalNavigation() ? ASPx.AnimationHelper.SLIDE_HORIZONTAL_DIRECTION : ASPx.AnimationHelper.SLIDE_VERTICAL_DIRECTION,
                    onComplete: function () { this.UpdateItemElementsIfRequired(); }.aspxBind(this)
                });
        },
        PerformRollBack: function () {
            this.SetActiveItemIndexInternal(this.GetActiveItemIndex(), false);
        },
        /* Create and prepare hierarchy */
        CreateControlHierarchyInternal: function () {
            ImageAreaStrategy.prototype.CreateControlHierarchyInternal.call(this);
            this.slidePanelElement = this.CreateDiv();
            this.GetImageAreaElement().appendChild(this.slidePanelElement);
        },
        PrepareControlHierarchy: function () {
            ImageAreaStrategy.prototype.PrepareControlHierarchy.call(this);
            var slideElement = this.GetSlidePanelElement();
            slideElement.className = CssClassesConstants.ImageAreaSlidePanelCssClassName;
            this.PatchElementForMSTouch(slideElement);
        },
        AdjustControlInternal: function () {
            ImageAreaStrategy.prototype.AdjustControlInternal.call(this);
            var horizontal = this.IsHorizontalNavigation();
            var width = horizontal ? this.GetItemCount() * this.GetItemElementWidth() : this.GetItemElementWidth();
            var height = !horizontal ? this.GetItemCount() * this.GetItemElementHeight() : this.GetItemElementHeight();
            ASPx.SetStyles(this.GetSlidePanelElement(), {
                width: width,
                height: height
            });
        },
        AdjustItemElement: function (index) {
            ImageAreaStrategy.prototype.AdjustItemElement.call(this, index);
            var itemElement = this.GetItemElement(index);
            var position = this.IsHorizontalNavigation() ? this.GetItemElementWidth() * index : this.GetItemElementHeight() * index;
            if(this.IsHorizontalNavigation())
                ASPx.SetStyles(itemElement, { left: position });
            else
                ASPx.SetStyles(itemElement, { top: position });
        },
        /* Html elements api */
        GetItemsContainer: function () {
            return this.GetSlidePanelElement();
        },
        /* Swipe slide gesture */
        CanCreateSwipeGestureHandler: function () {
            return true;
        },
        /* Navigation buttons */
        IsEnablePagingByClick: function () {
            var imageSlider = this.GetImageSlider();
            return imageSlider.ItemClick.IsEmpty() && !imageSlider.HasNavigateUrls() && ImageAreaStrategy.prototype.IsEnablePagingByClick.call(this);
        },
        /* Handlers */
        OnImageAreaClick: function (evt) {
            if(this.IsExecutedGesture())
                return ASPx.Evt.PreventEventAndBubble(evt);
            else
                ImageAreaStrategy.prototype.OnImageAreaClick.call(this, evt);
        },
        /* Utils */
        GetItemElementPosition: function (index) {
            return this.IsHorizontalNavigation() ? -(index * this.GetItemElementWidth()) : -(index * this.GetItemElementHeight());
        }
    });

    /* Slide loop ImageArea strategy */
    var SlideLoopNavigationImageAreaStrategy = ASPx.CreateClass(SlideImageAreaStrategy, {
        constructor: function (imageSlider) {
            this.simpleTransition = null;
            this.constructor.prototype.constructor.call(this, imageSlider);
        },
        CreateItemElementsManager: function () {
            return new LoopImageAreaItemElementsManager(this);
        },
        GetEnableLoopNavigation: function () {
            return true;
        },
        StartSlideSimpleTransition: function (from, to, preventAnimation) {
            if(preventAnimation)
                this.UpdateItemsPosition(to - from);
            else {
                this.simpleTransition = ASPx.AnimationHelper.createSimpleAnimationTransition({
                    duration: this.GetImageSlider().animationDuration,
                    onUpdate: function (value) {
                        this.UpdateItemsPosition(value);
                    }.aspxBind(this)
                });
                this.simpleTransition.Start(from, to);
            }
        },
        StopSimpleTransition: function () {
            if(this.simpleTransition) {
                this.simpleTransition.Cancel();
                this.simpleTransition = null;
            }
        },
        UpdateItemsPosition: function (value) {
            this.ForEachItem(function (i) {
                var element = this.GetItemElement(i);
                if(element) {
                    element.dxPosition = element.dxPosition + value;
                    this.SetItemPosition(element);
                }
            });
        },
        OnMouseMoveUpdatePosition: function (position) {
            this.StopSimpleTransition();
            var itemSize = this.GetItemElementSize();

            this.ForEachItem(function (i) {
                var element = this.GetItemElement(i);
                if(element)
                    element.dxPosition = element.dxPosition + position;
            });

            this.ForEachItem(function (i) {
                var element = this.GetItemElement(i);
                if(!element)
                    return;
                this.SetItemPosition(element);

                if(element.dxPosition < 0 && ((element.dxPosition + itemSize) > 0)) {
                    var index = i + 1;
                    if(index == this.GetItemCount())
                        index = 0;
                    var nextItem = this.GetItemElement(index);
                    nextItem.dxPosition = element.dxPosition + itemSize;
                    this.SetItemPosition(nextItem);
                }
                if(element.dxPosition < itemSize && ((element.dxPosition + itemSize) > itemSize)) {
                    var index = i - 1;
                    if(index < 0)
                        index = this.GetItemCount() - 1;
                    var nextItem = this.GetItemElement(index);
                    nextItem.dxPosition = element.dxPosition - itemSize;
                    this.SetItemPosition(nextItem);
                }
            });
        },
        SetItemPosition: function (itemElement) {
            this.SetElementTransformPosition(itemElement, itemElement.dxPosition);
        },
        /* Items management */
        SetActiveItemIndexInternal: function (index, preventAnimation, isRollback) {
            ImageAreaStrategy.prototype.SetActiveItemIndexInternal.call(this, index, preventAnimation);
            this.StopSimpleTransition();
            this.UpdateItemElementsIfRequired();

            var currentIndex = this.GetPrevItemIndex();
            if(currentIndex == -1)
                currentIndex = 0;

            if(!isRollback) {
                var isLeft = currentIndex > index;
                if(currentIndex == 0 && index == this.GetItemCount() - 1)
                    isLeft = true;
                if(currentIndex == this.GetItemCount() - 1 && index == 0)
                    isLeft = false;

                var prevItem = this.GetItemElement(currentIndex);
                var nextIndex = isLeft ? currentIndex - 1 : currentIndex + 1;

                for(var i = 1; i < this.GetItemCount() ; i++) {
                    if(isLeft && nextIndex < 0)
                        nextIndex = this.GetItemCount() - 1;
                    else if(nextIndex == this.GetItemCount())
                        nextIndex = 0;

                    var nextItem = this.GetItemElement(nextIndex);
                    if(nextItem) {
                        nextItem.dxPosition = (isLeft ? -1 : 1) * (this.GetItemElementSize() * i) + prevItem.dxPosition;
                        this.SetItemPosition(nextItem);
                    }
                    nextIndex = isLeft ? nextIndex - 1 : nextIndex + 1;
                }
            }

            var offset = this.GetItemElement(index).dxPosition;
            this.ForEachItem(function (i) {
                var element = this.GetItemElement(i);
                if(element)
                    element.dxPosition = element.dxPosition - offset;
            });

            this.StartSlideSimpleTransition(offset, 0, preventAnimation);
        },
        PerformRollBack: function () {
            this.SetActiveItemIndexInternal(this.GetActiveItemIndex(), false, true);
        },
        /* Create and prepare hierarchy */
        CreateControlHierarchyInternal: function () {
            ImageAreaStrategy.prototype.CreateControlHierarchyInternal.call(this);
        },
        PrepareControlHierarchy: function () {
            ImageAreaStrategy.prototype.PrepareControlHierarchy.call(this);
            this.PatchElementForMSTouch(this.GetImageAreaElement());
        },
        AdjustControlInternal: function () {
            ImageAreaStrategy.prototype.AdjustControlInternal.call(this);
        },
        AdjustItemElement: function (index) {
            ImageAreaStrategy.prototype.AdjustItemElement.call(this, index);
            var itemElement = this.GetItemElement(index);
            itemElement.dxPosition = this.IsHorizontalNavigation() ? this.GetItemElementWidth() * index : this.GetItemElementHeight() * index;;
            this.SetItemPosition(itemElement);
        },
        CreateSwipeGestureHandler: function () {
            ASPx.GesturesHelper.AddSwipeSlideGestureHandler(
                this.GetClientControlName() + this.GetUniqueId(),
                function () { return this.GetSwipeGestureElement(); }.aspxBind(this),
                this.IsHorizontalNavigation() ? ASPx.AnimationHelper.SLIDE_HORIZONTAL_DIRECTION : ASPx.AnimationHelper.SLIDE_VERTICAL_DIRECTION,
                function (evt) { return this.CanHandleSwipeGesture(evt); }.aspxBind(this),
                function () { this.PerformBackward(); }.aspxBind(this),
                function () { this.PerformForward(); }.aspxBind(this),
                function () { this.PerformRollBack(); }.aspxBind(this),
                function (position) { this.OnMouseMoveUpdatePosition(position); }.aspxBind(this)
            );
        },

        UpdateOverlayParameters: function () {
            SlideImageAreaStrategy.prototype.UpdateOverlayParameters.call(this);
            if(this.overlayImageElement && ASPx.Browser.Chrome) {
		        var textElement = this.GetItemTextElement(this.GetActiveItemIndex());
		        if(textElement)
			        this.overlayImageElement.style.height = (this.GetImageAreaElement().offsetHeight - textElement.offsetHeight) + "px";
	        }
        },
        /* Html elements api */
        GetSwipeGestureElement: function () {
            return this.GetImageAreaElement();
        },
        GetItemsContainer: function () {
            return this.GetImageAreaElement();
        },
        UpdateNavigationButtonsState: function () {
            return false;
        }
    });

    /* Fade ImageArea strategy */
    FadeImageAreaStrategy = ASPx.CreateClass(ImageAreaStrategy, {
        constructor: function (imageSlider) {
            this.constructor.prototype.constructor.call(this, imageSlider);
        },
        /* Items management */
        SetActiveItemIndexInternal: function (index, preventAnimation) {
            ImageAreaStrategy.prototype.SetActiveItemIndexInternal.call(this, index, preventAnimation);
            this.UpdateItemElementsIfRequired();

            var appearItem = this.GetItemElement(index);
            var disapperItem = this.GetItemElement(this.GetPrevItemIndex());
            if(preventAnimation) {
                this.SetVisible(appearItem);
                if(disapperItem)
                    this.SetInvisible(disapperItem);
            }
            else {
                var duration = this.GetImageSlider().animationDuration;
                ASPx.SetElementDisplay(appearItem, true);
                ASPx.AnimationHelper.fadeTo(disapperItem, {
                    to: 0, duration: duration,
                    onComplete: function (el) { ASPx.SetElementDisplay(el, false); }
                });
                ASPx.AnimationHelper.fadeTo(appearItem, {
                    to: 1, duration: duration
                });
            }
        },
        UpdateNavigationButtonsState: function () {
            if(!this.GetEnableLoopNavigation())
                return ImageAreaStrategy.prototype.UpdateNavigationButtonsState.call(this);
            return false;
        },
        AdjustItemElement: function (index) {
            ImageAreaStrategy.prototype.AdjustItemElement.call(this, index);
            this.SetInvisible(this.GetItemElement(index));
        },
        GetEnableLoopNavigation: function () {
            return this.GetImageSlider().enableLoopNavigation;
        },
        CanCreateSwipeGestureHandler: function () {
            return true;
        },
        GetSwipeGestureElement: function () {
            return this.GetImageAreaElement();
        },
        OnImageAreaClick: function (evt) {
            if(this.IsExecutedGesture())
                return ASPx.Evt.PreventEventAndBubble(evt);
            else
                ImageAreaStrategy.prototype.OnImageAreaClick.call(this, evt);
        },
        CreateSwipeGestureHandler: function () {
            ASPx.GesturesHelper.AddSwipeSlideGestureHandler(
                this.GetClientControlName() + this.GetUniqueId(),
                function () { return this.GetSwipeGestureElement(); }.aspxBind(this),
                this.IsHorizontalNavigation() ? ASPx.AnimationHelper.SLIDE_HORIZONTAL_DIRECTION : ASPx.AnimationHelper.SLIDE_VERTICAL_DIRECTION,
                function (evt) { return this.CanHandleSwipeGesture(evt); }.aspxBind(this),
                function () { this.PerformBackward(); }.aspxBind(this),
                function () { this.PerformForward(); }.aspxBind(this),
                function () { },
                function () { }
            );
        },
        /* Html elements API */
        GetItemsContainer: function () {
            return this.GetImageAreaElement();
        },
        /* Utils */
        SetVisible: function (element) {
            ASPx.SetStyles(element, {
                display: "",
                opacity: 1
            });
        },
        SetInvisible: function (element) {
            ASPx.SetStyles(element, {
                display: "none",
                opacity: 0
            });
        }
    });

    /* None ImageArea strategy */
    var NoneImageAreaStrategy = ASPx.CreateClass(FadeImageAreaStrategy, {
        constructor: function (imageSlider) {
            this.constructor.prototype.constructor.call(this, imageSlider);
        },
        /* Items management */
        SetActiveItemIndexInternal: function (index, preventAnimation) {
            FadeImageAreaStrategy.prototype.SetActiveItemIndexInternal.call(this, index, true);
        }
    });

    /* NavigationBar strategy */
    var NavigationBarStrategy = ASPx.CreateClass(ItemsOwnerBase, {
        constructor: function (imageSlider) {
            this.navigationBarElement = null;
            this.slidePanelWrapperElement = null;
            this.lastSlideItemsPortionDate = new Date();

            this.slideElementPosition = 0;

            this.constructor.prototype.constructor.call(this, imageSlider);
        },
        GetUniqueId: function () {
            return "_nb_";
        },
        IsHorizontalNavigation: function () {
            var position = this.GetPositionMode();
            return position == NavigationBarPositionEnum.Top || position == NavigationBarPositionEnum.Bottom;
        },
        /* Navigate urls */
        IsNeedCreateHyperLink: function () {
            return false;
        },
        /* Items management */
        CreateItemElementsManager: function () {
            return new NavigationBarItemElementsManager(this);
        },
        GetTemplate: function (index) {
            return this.GetItem(index).tmtpl;
        },
        GetImageSrc: function (index) {
            var thumbnailImageSrc = this.GetItem(index).ts;
            if(thumbnailImageSrc)
                return thumbnailImageSrc;
            return this.GetItem(index).s;
        },
        SetActiveItemIndexInternal: function (index, preventAnimation) {
            var snapToLeft = this.IsSnapToLeft(index);
            var nextIndex = snapToLeft ? index - 1 : index + 1;
            if(this.GetImageSlider().extremeItemClickMode == ExtremeItemClickModeEnum.Select || !this.IsValidItemIndex(nextIndex))
                nextIndex = index;
            if(!this.IsVisibleItemElement(nextIndex, true))
                this.SlideToItem(nextIndex, snapToLeft, preventAnimation);
            this.SetActiveItemVisually(index, preventAnimation);
        },
        IsVisibleItemElement: function (index, considerSize) {
            var slideWrapperElementSize = this.GetElementSize(this.GetSlidePanelWrapperElement());
            var itemPosition = (this.GetItemElementSize() + this.GetItemSpacing()) * index;
            var itemPositionInSlideWrapper = this.ConvertItemPositionToSlideWrapperPosition(itemPosition);

            if(considerSize)
                return itemPositionInSlideWrapper >= 0 && itemPositionInSlideWrapper + this.GetItemElementSize() <= slideWrapperElementSize;
            return itemPositionInSlideWrapper + this.GetItemElementSize() >= 0 && itemPositionInSlideWrapper <= slideWrapperElementSize;
        },
        IsSnapToLeft: function (index) {
            var slideWrapperElementSize = this.GetElementSize(this.GetSlidePanelWrapperElement());
            var itemPosition = (this.GetItemElementSize() + this.GetItemSpacing()) * index;
            var lastItemSize = this.GetItemElementSize() + (index != this.GetItemCount() - 1 ? this.GetItemSpacing() : 0);
            return this.ConvertItemPositionToSlideWrapperPosition(itemPosition + lastItemSize) < (slideWrapperElementSize / 2);
        },
        ConvertItemPositionToSlideWrapperPosition: function (itemPosition) {
            return this.GetElementTransformPosition(this.GetSlidePanelElement()) + itemPosition;
        },
        SlideToItem: function (index, snapToLeft, preventAnimation) {
            this.UpdateItemElementsAfterDelayIfRequired();
            this.CalculateSlideElementPosition(index, snapToLeft);
            this.PerformSlideAction(preventAnimation);
        },
        CalculateSlideElementPosition: function (index, snapToLeft) {
            var slideWrapperElementSize = this.GetElementSize(this.GetSlidePanelWrapperElement());
            var itemPosition = (this.GetItemElementSize() + this.GetItemSpacing()) * index;
            if(snapToLeft) {
                this.slideElementPosition = -itemPosition;
                var limitPosition = -(this.GetElementSize(this.GetSlidePanelElement()) - this.GetElementSize(this.GetSlidePanelWrapperElement()));
                var next = this.GetElementTransformPosition(this.GetSlidePanelElement()) > this.slideElementPosition;
                if(this.slideElementPosition < limitPosition || next && Math.abs(-limitPosition + this.slideElementPosition) < this.GetItemElementSize())
                    this.slideElementPosition = limitPosition;
            }
            else {
                this.slideElementPosition = slideWrapperElementSize - (itemPosition + this.GetItemElementSize());
                var prev = this.GetElementTransformPosition(this.GetSlidePanelElement()) < this.slideElementPosition;
                if(this.slideElementPosition > 0 || prev && this.slideElementPosition > -this.GetItemElementSize())
                    this.slideElementPosition = 0;
            }
        },
        PerformSlideAction: function (preventAnimation) {
            this.UpdateNavigationButtonsState(); //B219437
            if(preventAnimation)
                this.SetElementTransformPosition(this.GetSlidePanelElement(), this.slideElementPosition);
            else {
                ASPx.AnimationHelper.slideTo(this.GetSlidePanelElement(), {
                    to: this.slideElementPosition, callBack: function () { this.UpdateItemElementsIfRequired(); }.aspxBind(this),
                    duration: this.IsSinglePagingMode() ? 300 : ASPx.AnimationConstants.Durations.Default,
                    direction: this.IsHorizontalNavigation() ? ASPx.AnimationHelper.SLIDE_HORIZONTAL_DIRECTION : ASPx.AnimationHelper.SLIDE_VERTICAL_DIRECTION
                });
            }
        },
        SetActiveItemVisually: function (index, preventAnimation) {
        },
        /* Navigation buttons */
        GetNavigationBtnsVisibilityMode: function () {
            return this.GetImageSlider().thumbNavBtnsVbl;
        },
        GetButtonsContainer: function () {
            return this.GetNavigationBarElement();
        },
        GetPrevButtonHtml: function () {
            return this.GetImageSlider().nbpbh;
        },
        GetNextButtonHtml: function () {
            return this.GetImageSlider().nbnbh;
        },
        UpdateNavigationButtonsState: function () {
            if(this.navigationButtons) {
                var oldButtonsVisibility = this.navigationButtons.GetVisibilityButtons();
                if(this.IsWrapperOverlapsItemCollection())
                    this.SetVisibilityButtons(false);
                else {
                    this.SetVisibilityButtons(true);
                    var minOffset = -(this.GetElementSize(this.GetSlidePanelElement(), false) - this.GetElementSize(this.GetSlidePanelWrapperElement(), true)), maxOffset = 0;
                    this.SetEnablePrevButton(this.slideElementPosition != maxOffset);
                    this.SetEnableNextButton(this.slideElementPosition != minOffset);
                }
                return oldButtonsVisibility != this.navigationButtons.GetVisibilityButtons();
            }
            return false;
        },
        PerformBackward: function () {
            if(this.IsSinglePagingMode())
                this.SlideItemsPortionSingleMode(true);
            else
                this.SlideItemsPortionPageMode(true);
        },
        PerformForward: function () {
            if(this.IsSinglePagingMode())
                this.SlideItemsPortionSingleMode(false);
            else
                this.SlideItemsPortionPageMode(false);
        },
        PerformRollBack: function () {
            var position = this.GetElementTransformPosition(this.GetSlidePanelElement());
            var slidePanelElementSize = this.GetElementSize(this.GetSlidePanelElement());
            var slidePanelWrapperElementSize = this.GetElementSize(this.GetSlidePanelWrapperElement());
            var minPosition = slidePanelWrapperElementSize - slidePanelElementSize;

            if(this.IsWrapperOverlapsItemCollection())
                this.slideElementPosition = (slidePanelWrapperElementSize - slidePanelElementSize) / 2;
            else if(position > 0)
                this.slideElementPosition = 0;
            else if(position < minPosition)
                this.slideElementPosition = minPosition;
            this.PerformSlideAction();
        },
        SlideItemsPortionPageMode: function (backward) {
            var index = backward ? 0 : this.GetItemCount() - 1;
            var lastIndex = backward ? this.GetItemCount() - 1 : 0;
            var previousInvisibleIndex = index;
            while(index != lastIndex) {
                if(this.IsVisibleItemElement(index, true)) {
                    this.SlideToItem(previousInvisibleIndex, !backward);
                    break;
                }
                previousInvisibleIndex = index;
                index += backward ? 1 : -1;
            }
        },
        SlideItemsPortionSingleMode: function (backward) {
            var index = backward ? 0 : this.GetItemCount() - 1;
            var lastIndex = backward ? this.GetItemCount() - 1 : 0;
            var previousInvisibleIndex = index;
            var considerSize = new Date() - this.lastSlideItemsPortionDate < 350 ? false : true;
            while(index != lastIndex) {
                if(this.IsVisibleItemElement(index, considerSize)) {
                    this.SlideToItem(previousInvisibleIndex, backward);
                    break;
                }
                previousInvisibleIndex = index;
                index += backward ? 1 : -1;
            }
            this.lastSlideItemsPortionDate = new Date();
        },
        /* Create and prepare hierarchy */
        CreateControlHierarchyInternal: function () {
            ItemsOwnerBase.prototype.CreateControlHierarchyInternal.call(this);
            var mainElement = this.GetMainElement();
            this.navigationBarElement = this.CreateDiv();
            this.slidePanelWrapperElement = this.CreateDiv();

            switch (this.GetPositionMode()) {
                case NavigationBarPositionEnum.Top:
                case NavigationBarPositionEnum.Left:
                    var passePartour = this.GetImageSlider().GetPassePartoutElement();
                    if(passePartour) {
                        mainElement.insertBefore(this.navigationBarElement, passePartour);
                        break;
                    }
                case NavigationBarPositionEnum.Bottom:
                case NavigationBarPositionEnum.Right:
                    mainElement.appendChild(this.navigationBarElement);
                    break;
            }
            this.slidePanelElement = this.CreateDiv();
            this.slidePanelWrapperElement.appendChild(this.slidePanelElement);
            this.navigationBarElement.appendChild(this.slidePanelWrapperElement);
        },
        GetItemsContainer: function () {
            return this.GetSlidePanelElement();
        },
        PrepareControlHierarchy: function () {
            ItemsOwnerBase.prototype.PrepareControlHierarchy.call(this);
            this.PrepareNavigationBarElement();
            this.PrepareSlidePanelElement();
        },
        PrepareNavigationBarElement: function () {
            var navigationBarElement = this.GetNavigationBarElement();
            ASPx.SetStyles(navigationBarElement, {
                className: this.GetImageSlider().navBarCssCl,
                cssText: this.GetImageSlider().navBarStyle
            });
        },
        PrepareSlidePanelElement: function () {
            var slideElement = this.GetSlidePanelElement();
            slideElement.className = CssClassesConstants.NavigationBarSlidePanelCssClassName;
            this.GetSlidePanelWrapperElement().className = CssClassesConstants.NavigationBarSlidePanelWrapperCssClassName;
            this.PatchElementForMSTouch(slideElement);
        },
        AdjustControlInternal: function () {
            ItemsOwnerBase.prototype.AdjustControlInternal.call(this);
            this.AdjustNavigationBarElement();
            this.AdjustSlidePanelElement();
            this.UpdateItemElementsIfRequired();
            // T107146, if buttons have outside location and it's visibility was changed slidePanelWrapper should be adjusted
            if(this.UpdateNavigationButtonsState())
                this.AdjustNavigationBarElement();
        },
        AdjustItemElements: function () {
            if(!this.IsAdjustedSize()) {
                ItemsOwnerBase.prototype.AdjustItemElements.call(this);
                var horizontal = this.IsHorizontalNavigation();
                var width = horizontal ? this.GetItemCount() * this.GetItemWidthWithSpacing() - this.GetItemSpacing() : this.GetItemElementWidth();
                var height = horizontal ? this.GetItemElementHeight() : this.GetItemCount() * this.GetItemHeightWithSpacing() - this.GetItemSpacing();
                ASPx.SetStyles(this.GetSlidePanelElement(), { width: width, height: height });
            }
        },
        AdjustNavigationBarElement: function () {
            var styles = this.GetNavigationBarElementsStyles();
            ASPx.SetStyles(this.GetNavigationBarElement(), styles.navBarStyle);
            ASPx.SetStyles(this.GetSlidePanelWrapperElement(), styles.wrapperStyle);
        },
        GetNavigationBarElementsStyles: function () {
            var isHorizontal = this.IsHorizontalNavigation();
            var styles = {};
            if(this.CanUseVisibleItemCount())
                styles = {
                    width: isHorizontal ? this.GetItemWidthWithSpacing() * this.GetVisibleItemCount() - this.GetItemSpacing() : this.GetItemElementWidth(),
                    height: isHorizontal ? this.GetItemElementHeight() : this.GetItemHeightWithSpacing() * this.GetVisibleItemCount() - this.GetItemSpacing()
                };
            else {
                var navigationBar = this.GetNavigationBarElement();
                var mainElementWidth = SizeUtils.GetClientWidth(this.GetMainElement(), true);
                var mainElementHeight = SizeUtils.GetClientHeight(this.GetMainElement(), true);
                styles = {
                    width: this.IsHorizontalNavigation() ? mainElementWidth : this.GetItemElementWidth(),
                    height: this.IsHorizontalNavigation() ? this.GetItemElementHeight() : mainElementHeight
                };

                ASPx.SetStyles(navigationBar, styles);
                styles.width = this.IsHorizontalNavigation() ? mainElementWidth - (navigationBar.offsetWidth - mainElementWidth) : this.GetItemElementWidth();
                styles.height = this.IsHorizontalNavigation() ? this.GetItemElementHeight() : mainElementHeight - (navigationBar.offsetHeight - mainElementHeight);
            }
            return {
                navBarStyle: {
                    width: styles.width,
                    height: styles.height
                },
                wrapperStyle: {
                    width: styles.width,
                    height: styles.height
                }
            };
        },
        CanUseVisibleItemCount: function () {
            return !this.GetImageSlider().showImageArea && this.GetVisibleItemCount() > 0;
        },
        AdjustSlidePanelElement: function () {
            this.slideElementPosition = this.GetPostDataOffset();
            if(this.IsWrapperOverlapsItemCollection())
                this.slideElementPosition = (this.GetElementSize(this.GetSlidePanelWrapperElement()) - this.GetElementSize(this.GetSlidePanelElement())) / 2;
            this.SetElementTransformPosition(this.GetSlidePanelElement(), this.slideElementPosition);
        },
        IsWrapperOverlapsItemCollection: function () {
            return this.GetElementSize(this.GetSlidePanelWrapperElement()) > this.GetElementSize(this.GetSlidePanelElement());
        },
        GetPostDataOffset: function () {
            var offset = this.IsAdjustedSize() ? 0 : this.GetImageSlider().navBarOffset;
            if(offset == 0 && this.IsRtl())
                offset = this.GetElementSize(this.GetSlidePanelWrapperElement()) - this.GetElementSize(this.GetSlidePanelElement());
            return offset;
        },
        PrepareItemElement: function (index) {
            ItemsOwnerBase.prototype.PrepareItemElement.call(this, index);
            this.GetItemElement(index).itemIndex = index;
        },
        AdjustItemElement: function (index) {
            ItemsOwnerBase.prototype.AdjustItemElement.call(this, index);
            ASPx.SetStyles(this.GetItemElement(index), this.IsHorizontalNavigation() ? { left: this.GetItemWidthWithSpacing() * index } : { top: this.GetItemHeightWithSpacing() * index });
        },
        CalculateSize: function () {
            if(!this.IsAdjustedSize())
                ItemsOwnerBase.prototype.CalculateSize.call(this);
        },
        CalculateItemElementSize: function (fakeElement) {
            this.GetNavigationBarElement().appendChild(fakeElement);
            this.fullItemWidth = SizeUtils.GetClientWidth(fakeElement);
            this.fullItemHeight = SizeUtils.GetClientHeight(fakeElement);
            this.clearItemWidth = SizeUtils.GetClientWidth(fakeElement, true);
            this.clearItemHeight = SizeUtils.GetClientHeight(fakeElement, true);
        },
        ResetControlStyle: function () {
            ASPx.SetStyles(this.GetNavigationBarElement(), { width: 0, height: 0 });
        },
        /* Swipe slide gesture */
        CanCreateSwipeGestureHandler: function () {
            return true;
        },
        /* Handlers */
        InitializeHandlers: function () {
            ItemsOwnerBase.prototype.InitializeHandlers.call(this);
            ASPx.Evt.AttachEventToElement(this.GetSlidePanelElement(), this.GetMouseUpEventName(), function (evt) { this.OnNavigationBarMouseUp(evt); }.aspxBind(this)); //B230893
        },
        OnNavigationBarMouseUp: function (evt) {
            if(this.CanCandleMouseUpEvent(evt)) {
                var index = this.GetItemIndexFromEvent(evt);
                if(index != -1) {
                    this.GetImageSlider().SetActiveItemIndexInternal(index);
                    this.StopPlayingWhenPaging();
                    this.GetImageSlider().RaiseThumbnailItemClick();
                }
            }
        },
        GetMouseUpEventName: function () {
            return ASPx.TouchUIHelper.touchMouseUpEventName;
        },
        CanCandleMouseUpEvent: function (evt) {
            return !this.IsExecutedGesture() && ASPx.Evt.IsLeftButtonPressed(evt);
        },
        GetItemIndexFromEvent: function (evt) {
            var itemElement = ASPx.GetParentByClassName(ASPx.Evt.GetEventSource(evt),this.GetItemElementCssClass());
            return itemElement ? itemElement.itemIndex : -1;
        },
        /* Images processing */
        GetImageSizeMode: function () {
            return ASPx.ImageControlUtils.ImageSizeModeEnum.FitAndCrop;
        },
        /* Html elements API */
        GetMainElement: function () {
            return this.GetImageSlider().GetMainElement();
        },
        GetSlidePanelWrapperElement: function () {
            return this.slidePanelWrapperElement;
        },
        GetNavigationBarElement: function () {
            return this.navigationBarElement;
        },

        GetElementSize: function (element, clear) {
            return this.IsHorizontalNavigation() ? SizeUtils.GetClientWidth(element, clear) : SizeUtils.GetClientHeight(element, clear);
        },
        GetItemWidthWithSpacing: function () {
            return this.GetItemElementWidth() + this.GetItemSpacing();
        },
        GetItemHeightWithSpacing: function () {
            return this.GetItemElementHeight() + this.GetItemSpacing();
        },
        GetPositionMode: function () {
            return this.GetImageSlider().GetNavigationBarPosition();
        },
        GetItemSpacing: function () {
            return this.GetImageSlider().itemSpacing;
        },
        IsSinglePagingMode: function () {
            return this.GetImageSlider().navBarPagingMode == NavigationBarPagingModeEnum.Single;
        },
        GetVisibleItemCount: function () {
            return this.GetImageSlider().visibleItemsCount;
        },
        /* Public */
        GetSlideElementPosition: function () {
            return Math.round(this.slideElementPosition);
        },
        GetWidth: function () {
            var navigationBarElement = this.GetNavigationBarElement();
            var currentStyle = ASPx.GetCurrentStyle(navigationBarElement);
            var width = SizeUtils.GetClientWidth(navigationBarElement) + ASPx.PxToInt(currentStyle.marginLeft) + ASPx.PxToInt(currentStyle.marginRight);
            return width > 0 ? width : 0;
        },
        GetHeight: function () {
            var navigationBarElement = this.GetNavigationBarElement();
            var currentStyle = ASPx.GetCurrentStyle(navigationBarElement);
            var height = SizeUtils.GetClientHeight(navigationBarElement) + ASPx.PxToInt(currentStyle.marginTop) + ASPx.PxToInt(currentStyle.marginBottom);
            return height > 0 ? height : 0;
        }
    });

    /* Thumbnail navigationBar strategy */
    var NavigationBarThumbnailStrategy = ASPx.CreateClass(NavigationBarStrategy, {
        constructor: function (imageSlider) {
            this.selectedStateElement = null;
            this.hoverStateElementSize = {};
            this.innerHoverStateElementSize = {};
            this.setActiveIndexCounter = 0;
            this.constructor.prototype.constructor.call(this, imageSlider);
        },
        BeginSetActiveIndex: function () {
            this.setActiveIndexCounter++;
        },
        EndSetActiveIndex: function () {
            this.setActiveIndexCounter--;
        },
        NeedOffsetCorrection: function () {
            return this.setActiveIndexCounter > 0 && this.GetImageSlider().extremeItemClickMode == ExtremeItemClickModeEnum.SelectAndSlide;
        },
        GetNavigationButtonsPosition: function () {
            return this.GetImageSlider().thumbNavBtnsPos;
        },
        /* Items management */
        GetItemIndexFromEvent: function(evt) {
            var result = NavigationBarStrategy.prototype.GetItemIndexFromEvent.call(this, evt);
            if(result == -1) {
                var eventSource = ASPx.Evt.GetEventSource(evt);
                var selectedItem = this.GetSelectedStateElement();
                if (eventSource.parentNode == selectedItem || eventSource == selectedItem)
                    result = this.GetRtlIndex(this.GetImageSlider().GetActiveItemIndex());
            }
            return result;
        },
        SetActiveItemIndexInternal: function (index, preventAnimation) {
            this.BeginSetActiveIndex();
            NavigationBarStrategy.prototype.SetActiveItemIndexInternal.call(this, index, preventAnimation);
            this.EndSetActiveIndex();
        },
        SetActiveItemVisually: function (index, preventAnimation) {
            this.SelectItemElement(index);
            this.MoveSelectedItem(index, preventAnimation);
        },
        SelectItemElement: function (index) {
            var currentItemElement = this.GetItemElement(index);
            var prevItemElement = this.GetItemElement(this.GetPrevItemIndex());
            if(prevItemElement)
                prevItemElement.className = prevItemElement.className.replace(CssClassesConstants.NavigationBarSelectedStateCssClassName, "");
            if(currentItemElement && !ASPx.ElementContainsCssClass(currentItemElement, CssClassesConstants.NavigationBarSelectedStateCssClassName))
                currentItemElement.className += CssClassesConstants.NavigationBarSelectedStateCssClassName;
        },
        MoveSelectedItem: function (index, preventAnimation) {
            var selectedStateElement = this.GetSelectedStateElement();
            var position = (this.GetItemElementSize() + this.GetItemSpacing()) * index;
            preventAnimation = this.GetImageSlider().disableSelectedStateAnimation || preventAnimation; // forASPxImageGallery
            if(preventAnimation)
                this.SetElementTransformPosition(selectedStateElement, position);
            else
                ASPx.AnimationHelper.slideTo(selectedStateElement, {
                    to: position,
                    direction: this.IsHorizontalNavigation() ? ASPx.AnimationHelper.SLIDE_HORIZONTAL_DIRECTION : ASPx.AnimationHelper.SLIDE_VERTICAL_DIRECTION
                });
        },
        SlideItemsPortionPageMode: function (backward) {
            this.PerformSlideToExtremeInvisibleElement(backward, true, !backward);
        },
        PerformRollBack: function () {
            NavigationBarStrategy.prototype.PerformRollBack.call(this);
        },
        /* Partial loading images */
        UpdateItemElementsIfRequired: function () {
            NavigationBarStrategy.prototype.UpdateItemElementsIfRequired.call(this);
            if(this.GetImageLoadMode() != LoadModeEnum.AllImages)
                this.SelectItemElement(this.GetActiveItemIndex());
        },
        /* Create and prepare Hierarchy */
        CanCreateNavigationButtons: function () {
            if(!this.CanUseVisibleItemCount())
                return true;
            return this.GetVisibleItemCount() < this.GetItemCount();
        },
        CreateNavigationButtons: function () {
            if(this.GetNavigationBtnsVisibilityMode() == ElementVisibilityModeEnum.None)
                return null;
            if(this.GetNavigationButtonsPosition() == NavigationBarButtonPositionEnum.Outside)
                return new OutsideNavigationButtons(this);
            return new NavigationButtons(this);
        },
        CreateControlHierarchy: function () {
            NavigationBarStrategy.prototype.CreateControlHierarchy.call(this);
            this.CreateSelectedStateElement();
        },
        CreateItemElement: function (index) {
            var itemElement = NavigationBarStrategy.prototype.CreateItemElement.call(this, index);
            if(this.IsEnabled() && !ASPx.Browser.TouchUI)
                itemElement.appendChild(this.CreateHoverStateElement());
            return itemElement;
        },
        CreateHoverStateElement: function (itemElement) {
            var element = this.CreateDiv();
            element.className = CssClassesConstants.NavigationBarHoverStateElementCssClassName;
            element.appendChild(this.CreateDiv());
            return element;
        },
        CreateSelectedStateElement: function () {
            this.selectedStateElement = this.CreateDiv();
            this.selectedStateElement.appendChild(this.CreateDiv()); //B218759
            this.GetSlidePanelElement().appendChild(this.selectedStateElement);
        },
        PrepareControlHierarchy: function () {
            NavigationBarStrategy.prototype.PrepareControlHierarchy.call(this);
            ASPx.SetStyles(this.GetSelectedStateElement(), {
                className: this.GetImageSlider().thSelCssCl,
                cssText: this.GetImageSlider().thSelStyle
            });
        },
        PrepareItemElement: function (index) {
            NavigationBarStrategy.prototype.PrepareItemElement.call(this, index);
            var stateController = this.GetStateController();
            if(this.IsEnabled() && !ASPx.Browser.TouchUI && stateController && this.GetEnableItemHoverState()) {
                var itemElement = this.GetItemElement(index);
                itemElement.id = this.GetClientControlName() + "_nb_" + index;
                stateController.AddHoverItem(itemElement.id, [CssClassesConstants.NavigationBarHoverStateCssClassName], [""], [""], null, null, false);
            }
        },
        GetEnableItemHoverState: function () {
            return true;
        },
        CalculateItemElementSize: function (fakeElement) {
            NavigationBarStrategy.prototype.CalculateItemElementSize.call(this, fakeElement);
            if(this.IsEnabled() && !ASPx.Browser.TouchUI)
                this.CalculateHoverStateElement(fakeElement);
        },
        CalculateHoverStateElement: function (parent) {
            var actualWidth = this.GetItemElementWidth();
            var actualHeight = this.GetItemElementHeight();
            var hoverStateElement = this.CreateHoverStateElement();
            parent.appendChild(hoverStateElement);

            ASPx.SetStyles(hoverStateElement, { display: "block" });
            this.hoverStateElementSize = this.GetBorderBoxSize(hoverStateElement, this.GetItemElementWidth(), this.GetItemElementHeight());

            var innerDiv = this.GetFirstChild(hoverStateElement);
            this.innerHoverStateElementSize = this.GetBorderBoxSize(innerDiv, this.hoverStateElementSize.width, this.hoverStateElementSize.height);
        },
        AdjustControlInternal: function () {
            NavigationBarStrategy.prototype.AdjustControlInternal.call(this);
            this.AdjustSelectedStateElement();
        },
        AdjustItemElement: function (index) {
            NavigationBarStrategy.prototype.AdjustItemElement.call(this, index);
            if(this.IsEnabled() && !ASPx.Browser.TouchUI)
                this.AdjustHoverStateElement(index);
        },
        AdjustHoverStateElement: function (index) {
            var hoverStateElement = this.GetHoverStateElement(index);
            ASPx.SetStyles(hoverStateElement, this.hoverStateElementSize);
            ASPx.SetStyles(this.GetFirstChild(hoverStateElement), this.innerHoverStateElementSize);
        },
        AdjustSelectedStateElement: function () {
            var selectedStateElement = this.GetSelectedStateElement();
            var size = this.GetBorderBoxSize(selectedStateElement, this.GetItemElementWidth(), this.GetItemElementHeight());
            ASPx.SetStyles(selectedStateElement, size);

            var innerDiv = this.GetFirstChild(selectedStateElement);
            size = this.GetBorderBoxSize(selectedStateElement, size.width, size.height);
            ASPx.SetStyles(innerDiv, size);
        },
        GetHoverStateElement: function (index) {
            return ASPx.GetChildByClassName(this.GetItemElement(index), CssClassesConstants.NavigationBarHoverStateElementCssClassName);
        },
        GetSelectedStateElement: function () {
            return this.selectedStateElement;
        },
        GetItemElementCssClass: function () {
            return this.GetImageSlider().thCssCl;
        },
        GetItemElementStyle: function () {
            return this.GetImageSlider().thStyle;
        },
        CalculateSlideElementPosition: function (index, snapToLeft) {
            var prevSlideElementPosition = this.slideElementPosition;
            NavigationBarStrategy.prototype.CalculateSlideElementPosition.call(this, index, snapToLeft);
            this.slideElementPosition = this.CorrectSelectAndSlideOffsetPosition(index, prevSlideElementPosition, this.slideElementPosition);
        },
        IsSnapToLeft: function (index) {
            var shownItemIndexes = this.GetShownItemIndexes(true);
            for(var i = 0; i < shownItemIndexes.length; i++)
                if(shownItemIndexes[i] == index)
                    return i < shownItemIndexes.length / 2;
            return index < shownItemIndexes[0];
        },
        SlideItemsPortionSingleMode: function (backward) {
            var considerSize = new Date() - this.lastSlideItemsPortionDate >= 350;
            this.PerformSlideToExtremeInvisibleElement(backward, considerSize, backward);
            this.lastSlideItemsPortionDate = new Date();
        },
        PerformSlideToExtremeInvisibleElement: function (backward, considerSize, preventAnimation) {
            var shownItemIndexes = this.GetShownItemIndexes(false);
            var lastIndex = backward ? -1 : this.GetItemCount();
            var incValue = backward ? -1 : 1;
            for(var index = shownItemIndexes[backward ? 0 : shownItemIndexes.length - 1]; index != lastIndex; index += incValue)
                if(!this.IsVisibleItemElement(index, considerSize)) {
                    this.SlideToItem(index, preventAnimation);
                    break;
                }
        },
        GetShownItemIndexes: function (getUncroppedItems) {
            var result = [];
            var itemSize = this.GetItemElementSize();
            var constOffset = this.GetElementTransformPosition(this.GetSlidePanelElement());
            var itemFullSize = itemSize + this.GetItemSpacing();

            var minValue = getUncroppedItems ? -1 : -itemSize;
            var maxValue = getUncroppedItems ? this.GetSlidePanelWrapperSize() - itemSize + 1 : this.GetSlidePanelWrapperSize();

            var tempValue = null;
            for(var index = 0; index < this.GetItemCount() ; index++) {
                tempValue = constOffset + itemFullSize * index;
                if(tempValue > minValue && tempValue < maxValue)
                    result.push(index);
            }
            return result;
        },
        /* Utils */
        CorrectSelectAndSlideOffsetPosition: function (nextItemIndex, prevPos, nextPos) {
            var index = this.GetActiveItemIndex();
            if(!this.NeedOffsetCorrection() || nextItemIndex == index)
                return nextPos;
            var itemElementSize = this.GetItemElementSize();
            var sliderWrapperSize = this.GetSlidePanelWrapperSize();
            if(sliderWrapperSize < (itemElementSize * 3)) {
                var offsetDelta = Math.abs(Math.abs(nextPos) - Math.abs(prevPos));
                var itemSpacing = this.GetItemSpacing();
                var itemPosition = (itemElementSize + itemSpacing) * index;
                var itemPositionInSlideWrapper = this.ConvertItemPositionToSlideWrapperPosition(itemPosition) +
                    (nextItemIndex < index ? offsetDelta : -offsetDelta);
                nextPos += (sliderWrapperSize - itemElementSize) / 2 - itemPositionInSlideWrapper;
            }
            return nextPos;
        },
        GetBorderBoxSize: function (element, width, height) {
            ASPx.SetStyles(element, { width: width, height: height });
            if(element.offsetWidth > width)
                width -= element.offsetWidth - width;
            if(element.offsetHeight > height)
                height -= element.offsetHeight - height;
            return { width: width, height: height };
        },
        GetFirstChild: function (parent) {
            return parent.children[0];
        },
        GetSlidePanelWrapperSize: function () {
            return this.GetElementSize(this.GetSlidePanelWrapperElement());
        },
        GetNavigationBarElementsStyles: function () {
            var styles = NavigationBarStrategy.prototype.GetNavigationBarElementsStyles.call(this);
            if(this.GetNavigationButtonsPosition() == NavigationBarButtonPositionEnum.Outside && this.navigationButtons) {
                var prevButtonSize = this.navigationButtons.GetPrevButtonWrapperSize();
                var nextButtonSize = this.navigationButtons.GetNextButtonWrapperSize();

                var isHorizontal = this.IsHorizontalNavigation();
                var styleName = isHorizontal ? "width" : "height";
                if(this.CanUseVisibleItemCount())
                    styles.navBarStyle[styleName] += prevButtonSize[styleName] + nextButtonSize[styleName];
                else
                    styles.wrapperStyle[styleName] -= prevButtonSize[styleName] + nextButtonSize[styleName];
                styles.wrapperStyle[isHorizontal ? "left" : "top"] = prevButtonSize[styleName];
            }
            return styles;
        }
    });

    /* Dots navigationBar strategy */
    var NavigationBarDotsStrategy = ASPx.CreateClass(NavigationBarStrategy, {
        constructor: function (imageSlider) {
            this.cloneElement = null;
            this.constructor.prototype.constructor.call(this, imageSlider);
        },
        Initialize: function () {
            NavigationBarStrategy.prototype.Initialize.call(this);
            this.cloneElement = ASPx.CreateHtmlElementFromString(this.GetImageSlider().dbh);
        },
        /* Items management */
        SetActiveItemVisually: function (index, preventAnimation) {
            var stateController = this.GetStateController();
            if(stateController) {
                if(this.GetPrevItemIndex() != -1)
                    stateController.DeselectElementBySrcElement(this.GetItemElement(this.GetPrevItemIndex()));
                stateController.SelectElementBySrcElement(this.GetItemElement(index));
            }
        },
        /* Partial loading images */
        GetImageLoadMode: function () {
            return LoadModeEnum.AllImages;
        },
        /* Handlers */
        OnNavigationBarMouseUp: function (evt) {
            ASPx.ClearHoverState();//Q452004
            NavigationBarStrategy.prototype.OnNavigationBarMouseUp.call(this, evt);
        },
        CanCandleMouseUpEvent: function (evt) {
            if(ASPx.Browser.IE && ASPx.Browser.Version < 9)
                return !this.IsExecutedGesture();
            return NavigationBarStrategy.prototype.CanCandleMouseUpEvent.call(this, evt);
        }, 
        GetMouseUpEventName: function () {
            return ASPx.Browser.IE && ASPx.Browser.Version < 9 ? "click" : ASPx.TouchUIHelper.touchMouseUpEventName; //Q374705
        },
        /* Navigation buttons */
        GetNavigationBtnsVisibilityMode: function () {
            return ElementVisibilityModeEnum.None;
        },
        /* Create and prepare Hierarchy */
        CreateItemElement: function (index) {
            var itemElement = this.CreateDiv();
            itemElement.id = this.GetItemElementId(index);
            return itemElement;
        },
        PrepareControlHierarchy: function () {
            NavigationBarStrategy.prototype.PrepareControlHierarchy.call(this);
            var imageSlider = this.GetImageSlider();
            var stateController = this.GetStateController();
            if(!stateController)
                return;
            this.ForEachItem(function (i) {
                if(this.IsEnabled()) {
                    stateController.AddHoverItem(this.GetItemElementId(i), [imageSlider.dotHCssCl], [imageSlider.dotHStyle],
                        null, [imageSlider.dotHSprt], [""], false);
                    stateController.AddPressedItem(this.GetItemElementId(i), [imageSlider.dotPCssCl], [imageSlider.dotPStyle],
                        null, [imageSlider.dotPSprt], [""], false);
                }
                stateController.AddSelectedItem(this.GetItemElementId(i), [imageSlider.dotSCssCl], [imageSlider.dotSStyle],
                    null, [imageSlider.dotSSprt], [""], false);
            });
        },
        AdjustImageElement: function (index) {
        },
        GetItemElementCssClass: function () {
            return this.cloneElement.className;
        },
        GetItemElementStyle: function () {
            return this.cloneElement.style.cssText;
        },
        CanHandleSwipeGesture: function (evt) {
            return !this.IsWrapperOverlapsItemCollection() && ItemsOwnerBase.prototype.CanHandleSwipeGesture.call(this, evt);
        },
        /* Utils */
        GetItemElementId: function (index) {
            return this.GetClientControlName() + "_dot_" + index;
        }
    });

    /* ItemElementsManager */
    var ItemElementsManagerBase = ASPx.CreateClass(null, {
        constructor: function (itemsOwner) {
            this.itemsOwner = itemsOwner;
            this.timerId == -1
            this.itemCollection = [];
        },
        GetOwner: function () {
            return this.itemsOwner;
        },
        GetCollection: function () {
            if(this.IsEnablePartialLoading())
                return this.itemCollection;
            return ASPx.GetChildNodesByTagName(this.GetOwner().GetItemsContainer(), "DIV");
        },
        HasItem: function (index) {
            return !!this.GetCollection()[index];
        },
        SaveItem: function (index, item) {
            if(this.IsEnablePartialLoading())
                this.itemCollection[index] = item;
        },
        GetImageLoadMode: function () {
            return this.GetOwner().GetImageLoadMode();
        },
        IsEnablePartialLoading: function () {
            return this.GetImageLoadMode() != LoadModeEnum.AllImages;
        },
        CreateItemsIfRequired: function () {
            if(this.GetImageLoadMode() == LoadModeEnum.AllImages)
                this.GetOwner().CreateItemElements();
        },
        PrepareItemsIfRequired: function () {
            if(this.GetImageLoadMode() == LoadModeEnum.AllImages)
                this.GetOwner().PrepareItemElements();
        },
        AdjustItemsIfRequired: function () {
            var owner = this.GetOwner();
            for(var i = 0; i < owner.GetItemCount() ; i++) {
                if(this.IsEnablePartialLoading() && !this.HasItem(i))
                    continue;
                this.GetOwner().AdjustItemElement(i);
                this.GetOwner().AdjustImageElement(i);
            }
        },
        UpdateHierarchyIfRequired: function () {
            if(this.IsEnablePartialLoading()) {
                this.timerId = ASPx.Timer.ClearTimer(this.timerId);
                this.UpdateHierarchyCore();
            }
        },
        CreateItemAndAppend: function (index, parent) {
            var owner = this.GetOwner();
            var element = owner.CreateItemElement(index);
            this.SaveItem(index, element);
            parent.appendChild(element);
            owner.PrepareItemElement(index);
            owner.AdjustItemElement(index);
            owner.AdjustImageElement(index);
        },
        RemoveItem: function (index) {
            if(this.GetImageLoadMode() == LoadModeEnum.DynamicLoadAndCache)
                return;
            var owner = this.GetOwner();
            var element = owner.GetItemElement(index);
            if(element) {
                var image = owner.GetImageElement(index);
                if(image)
                    owner.DetachLoadEvents(image);
                this.itemCollection[index] = undefined;
                ASPx.RemoveElement(element);
            }
        },
        UpdateHierarchyCore: function () {
            ;
        },
        GetDelay: function () {
            return 450;
        },
        UpdateHierarchyAfterDelayIfRequired: function () {
            if(this.IsEnablePartialLoading() && this.timerId == -1)
                this.timerId = window.setTimeout(function () { this.UpdateHierarchyIfRequired(); }.aspxBind(this), this.GetDelay());
        }
    });
    var ImageAreaItemElementsManager = ASPx.CreateClass(ItemElementsManagerBase, {
        constructor: function (imageAreaStrategy) {
            this.constructor.prototype.constructor.call(this, imageAreaStrategy);
        },
        GetDelay: function () {
            return this.GetOwner().GetImageSlider().animationDuration + 50;
        },
        UpdateHierarchyCore: function () {
            var owner = this.GetOwner();
            var activeItemIndex = owner.GetActiveItemIndex();
            var prevItemIndex = owner.GetPrevItemIndex();
            if(prevItemIndex == -1) //iffirst load
                prevItemIndex = 0;
            var isForwardDirection = (activeItemIndex - prevItemIndex) >= 0;

            var fragment = owner.CreateDocumentFragment();
            var itemsContainer = owner.GetItemsContainer();

            if(!this.HasItem(activeItemIndex))
                this.CreateItemAndAppend(activeItemIndex, fragment || itemsContainer);

            if(this.CanUpdateItemElements(isForwardDirection, isForwardDirection ? activeItemIndex + 1 : activeItemIndex - 1)) {
                for(var i = 0; i < owner.GetItemCount() ; i++) {
                    if(this.CanRemoveItem(i, activeItemIndex, prevItemIndex, isForwardDirection))
                        this.RemoveItem(i);
                    else if(!this.HasItem(i))
                        this.CreateItemAndAppend(i, fragment || itemsContainer);
                }
            }
            if(fragment)
                itemsContainer.appendChild(fragment);
        },
        CanRemoveItem: function (index, activeItemIndex, prevItemIndex, forwardDirection) {
            var visibleItemCount = 6;
            var result = forwardDirection ? (index < activeItemIndex || index > (activeItemIndex + visibleItemCount)) :
                    (index > activeItemIndex || index < (activeItemIndex - visibleItemCount));
            return result && prevItemIndex != index; //prevItemIndex forfade animation
        },
        CanUpdateItemElements: function (forwardDirection, followingItemIndex) {
            return forwardDirection ? followingItemIndex < this.GetOwner().GetItemCount() && !this.HasItem(followingItemIndex) :
                followingItemIndex > 0 && !this.HasItem(followingItemIndex);
        }
    });
    var LoopImageAreaItemElementsManager = ASPx.CreateClass(ItemElementsManagerBase, {
        constructor: function (imageAreaStrategy) {
            this.constructor.prototype.constructor.call(this, imageAreaStrategy);
        },
        UpdateHierarchyCore: function () {
            var owner = this.GetOwner();
            var activeIndex = owner.GetActiveItemIndex();

            var fragment = owner.CreateDocumentFragment();
            var itemsContainer = owner.GetItemsContainer();

            if(!this.HasItem(activeIndex))
                this.CreateItemAndAppend(activeIndex, fragment || itemsContainer);
            var activeItem = owner.GetItemElement(activeIndex);

            var nextIndex = activeIndex + 1;
            if(nextIndex == owner.GetItemCount())
                nextIndex = 0;
            if(!this.HasItem(nextIndex)) {
                this.CreateItemAndAppend(nextIndex, fragment || itemsContainer);
                var item = owner.GetItemElement(nextIndex);
                item.dxPosition = activeItem.dxPosition + owner.GetItemElementSize();
                owner.SetItemPosition(item);
            }

            var prevIndex = activeIndex - 1;
            var prevIndex = activeIndex - 1;
            if(prevIndex < 0)
                prevIndex = owner.GetItemCount() - 1;
            if(!this.HasItem(prevIndex)) {
                this.CreateItemAndAppend(prevIndex, fragment || itemsContainer);
                var item = owner.GetItemElement(prevIndex);
                item.dxPosition = activeItem.dxPosition - owner.GetItemElementSize();
                owner.SetItemPosition(item);
            }

            for(var i = 0; i < owner.GetItemCount() ; i++)
                if(i != activeIndex && i != prevIndex && i != nextIndex && owner.GetPrevItemIndex() != i && owner.GetItemElement(i))
                    this.RemoveItem(i);
            if(fragment)
                itemsContainer.appendChild(fragment);
        }
    });
    var NavigationBarItemElementsManager = ASPx.CreateClass(ItemElementsManagerBase, {
        constructor: function (navigationBarStrategy) {
            this.constructor.prototype.constructor.call(this, navigationBarStrategy);
        },
        CreateDummyItem: function (index, parent) {
            if(!this.HasItem(index))
                this.CreateAndPrepareItemElement(index, parent);
            else if(this.GetImageLoadMode() != LoadModeEnum.DynamicLoadAndCache) {
                var owner = this.GetOwner();
                var element = owner.GetItemElement(index);
                if(element) {
                    element.style.backgroundImage = "";
                    var image = owner.GetImageElement(index);
                    if(image)
                        owner.DetachLoadEvents(image);
                    ASPx.RemoveElement(image);
                    ASPx.RemoveElement(owner.GetCanvasElement(index));
                }
                this.SetDummyFlagToElement(index, true);
            }
        },
        SetDummyFlagToElement: function (index, isDummy) {
            var element = this.GetOwner().GetItemElement(index);
            if(element)
                element.isDummy = isDummy;
        },
        IsElementDummy: function (index) {
            var element = this.GetOwner().GetItemElement(index);
            if(element)
                return !!element.isDummy;
            return false;
        },
        CreateItemAndAppend: function (index, parent) {
            if(!this.HasItem(index))
                this.CreateAndPrepareItemElement(index, parent);
            if(this.IsElementDummy(index)) {
                this.SetDummyFlagToElement(index, false);
                this.GetOwner().AdjustImageElement(index);
            }
        },
        CreateAndPrepareItemElement: function (index, parent) {
            var owner = this.GetOwner();
            var element = owner.CreateItemElement(index);
            this.SaveItem(index, element);
            parent.appendChild(element);
            owner.PrepareItemElement(index);
            owner.AdjustItemElement(index);
            this.SetDummyFlagToElement(index, true);
        },
        UpdateHierarchyCore: function () {
            var owner = this.GetOwner();
            var navigationBarSize = owner.GetElementSize(owner.GetSlidePanelWrapperElement()),
                slidePanelPosition = owner.GetElementTransformPosition(owner.GetSlidePanelElement());

            var fragment = owner.CreateDocumentFragment();
            var itemsContainer = owner.GetItemsContainer();


            var itemFullSize = owner.GetItemElementSize() + owner.GetItemSpacing();

            var lPos = Math.min(-navigationBarSize * 0.3, -itemFullSize);
            var rPos = Math.max(navigationBarSize * 1.3, navigationBarSize + itemFullSize);
            var lDummyPos = lPos - navigationBarSize;
            var rDummyPos = rPos + navigationBarSize;

            var itemPos = 0;
            for(var i = 0; i < owner.GetItemCount() ; i++) {
                itemPos = slidePanelPosition + itemFullSize * i;
                if(itemPos > lDummyPos && itemPos < rDummyPos && (itemPos < lPos || itemPos > rPos))
                    this.CreateDummyItem(i, fragment || itemsContainer);
                else {
                    if(itemPos <= lDummyPos || itemPos >= rDummyPos)
                        this.RemoveItem(i);
                    else
                        this.CreateItemAndAppend(i, fragment || itemsContainer);
                }
            }
            if(fragment)
                itemsContainer.appendChild(fragment);
        }
    });
    var ASPxClientImageSlider = ASPx.CreateClass(ASPxClientControl, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);

            this.activeItemIndex = 0;
            this.imageAreaStrategy = null; // ImageAreaStrategy
            this.navigationBarStrategy = null; // NavigationBarStrategy
            this.passePartoutElement = null;
            this.hiddenField = null;

            this.slideShowEnabled = false;
            this.templateCreated = false;
            this.timerID = -1;

            this.defaultControlSize = { width: "", height: "" };

            /* From server */
            this.showImageArea = true;
            this.showNavBar = true;

            this.seoFriendlyItems = [];
            this.items = []; // { s: "", ts: "", t: "", n: "", u: "", tpl: "", ttpl: "", tmtpl: "", at: ""  }
            this.enableLoopNavigation = false;
            this.enableKeyboardSupport = true;
            this.enablePagingByClick = true;
            this.enablePagingGestures = true;
            this.extremeItemClickMode = 0;

            this.animationDuration = 400;
            this.allowMouseWheel = false;
            this.imageLoadMode = 1;
            this.imageSizeMode = 0;
            this.animationType = 0;
            this.navDirection = 0;
            this.itemSpacing = 5;
            this.target = "";
            this.hasNavigateUrls = false;
            this.hasTemplates = false;

            this.navBtnsVbl = 2;
            this.itemTxtVbl = 3;

            this.interval = 5000;
            this.autoPlay = false;
            this.stopPlayingWhenPaging = false;
            this.stopPlayingWhenPagingExecuted = false;
            this.pausePlayingWhenMouseOver = false;
            this.playPauseBtnVbl = 0;

            this.visibleItemsCount = 0;
            this.navBarPos = 0;
            this.navBarPagingMode = 0;
            this.thumbNavBtnsVbl = 2;
            this.thumbNavBtnsPos = 0;


            this.navBarOffset = 0;

            /* Styles and Css */
            this.passePartoutStyle = "";

            this.imgAreaCssCl = "";
            this.imgAreaStyle = "";

            this.itemCssCl = "";
            this.itemStyle = "";

            this.navBarCssCl = "";
            this.navBarStyle = "";
            this.thCssCl = "";
            this.thStyle = "";

            this.thSelCssCl = "";
            this.thSelStyle = "";

            this.itemTxtCssCl = "";
            this.itemTxtStyle = "";

            this.dotHSprt = null;
            this.dotHCssCl = "";
            this.dotHStyle = "";
            this.dotPSprt = null;
            this.dotPCssCl = "";
            this.dotPStyle = "";
            this.dotSSprt = null;
            this.dotSCssCl = "";
            this.dotSStyle = "";

            this.dbh = "";
            this.playBH = "";
            this.pauseBH = "";
            this.pbh = "";
            this.nbh = "";
            this.nbpbh = "";
            this.nbnbh = "";

            this.sizingConfig.adjustControl = true;

            /* ASPxImageGallery, ASPxImageZoomNavigator */
            this.disableSelectedStateAnimation = false;

            /* Events */
            this.ActiveItemChanged = new ASPxClientEvent();
            this.ItemClick = new ASPxClientEvent();
            this.ThumbnailItemClick = new ASPxClientEvent();
            this.StrategiesCreated = new ASPxClientEvent();
        },
        /* Initialize */
        InlineInitialize: function () {
            ASPxClientControl.prototype.InlineInitialize.call(this);
            this.PopulateItemsSeoMode();
        },
        Initialize: function () {
            ASPxClientControl.prototype.Initialize.call(this);
            if(this.hasTemplates)
                for(var i = 0; i < this.seoFriendlyItems.length; i++) {
                    this.items[i].s = this.seoFriendlyItems[i].s;
                    this.items[i].ts = this.seoFriendlyItems[i].ts;
                    this.items[i].t = this.seoFriendlyItems[i].t;
                    this.items[i].u = this.seoFriendlyItems[i].u;
                }
            this.seoFriendlyItems = null;
            this.CreateControlHierarchy();
            this.PrepareControlHierarchy();
            this.InitializeElementsSize();
        },
        PopulateItemsSeoMode: function () {
            var elements = ASPx.GetNodesByTagName(this.GetMainElement(), "IMG");
            if(elements.length > 0)
                this.populateItemCollection(elements, this.hasTemplates ? this.seoFriendlyItems : this.items);
        },
        populateItemCollection: function(imageCollection, itemsCollection, image) {
            var index = 0;
            while(image = imageCollection[0]) {
                var collectionItem = (itemsCollection[index] || (itemsCollection[index] = {}));
                collectionItem.s = ASPx.Attr.GetAttribute(image, "src");
                collectionItem.t = ASPx.Attr.GetAttribute(image, "alt");
                if(image.parentNode.tagName == "A") {
                    collectionItem.u = ASPx.Attr.GetAttribute(image.parentNode, "href");
                    ASPx.RemoveElement(image.parentNode);
                } else 
                    ASPx.RemoveElement(image);
                index++;
            }
        },
        InitializeElementsSize: function () {
            var mainElement = this.GetMainElement();
            this.defaultControlSize = {
                width: mainElement.style.width,
                height: mainElement.style.height
            };
        },
        AfterInitialize: function () {
            this.constructor.prototype.AfterInitialize.call(this);
            this.InitializeHandlers();
        },
        InitializeHandlers: function () {
            if(ASPx.Browser.TouchUI)
                return;
            if(this.enableKeyboardSupport) {
                var element = this.GetMainElement();
                ASPx.Evt.AttachEventToElement(element, "keydown", function (evt) { this.OnKeyDown(evt) }.aspxBind(this));
                ASPx.Evt.AttachEventToElement(element, "mousedown", function () { this.Focus(); }.aspxBind(this));
            }
            if(this.pausePlayingWhenMouseOver)
                MouseEnterHelper.AddHandler(this.GetMainElement(), function () {
                    if(this.slideShowEnabled)
                        this.PauseInternal();
                }.aspxBind(this), function () {
                    if(this.slideShowEnabled && !this.stopPlayingWhenPagingExecuted)
                        this.PlayInternal();
                }.aspxBind(this));
        },
        /* State */
        GetNavigationBarOffset: function(){
            var offset = 0;
            var navigationBar = this.GetNavigationBarStrategy();
            if(navigationBar) 
                offset = navigationBar.GetSlideElementPosition();
            return isNaN(offset) ? 0 : offset;
        },
        UpdateStateObject: function(){
            this.UpdateStateObjectWithObject({ activeItemIndex: this.GetActiveItemIndex(), navigationBarOffset: this.GetNavigationBarOffset() });
        },
        /* Create and prepare hierarchy */
        CreateTemplates: function () {
            var imageArea = this.GetImageAreaStrategy();
            if(imageArea)
                imageArea.CreateTemplates();
            var navigationBar = this.GetNavigationBarStrategy();
            if(navigationBar)
                navigationBar.CreateTemplates();
            ASPx.ProcessScriptsAndLinks(this.name);
            this.templateCreated = true;
        },
        AdjustControlCore: function () {
            this.AdjustControlCoreInternal();
            if(this.HasTemplates() && !this.templateCreated)
                window.setTimeout(function () { this.CreateTemplates(); }.aspxBind(this), 0);
        },
        SetSizeCore: function(sizePropertyName, size, getFunctionName, corrected) {
            this.defaultControlSize[sizePropertyName] = size;
            ASPxClientControl.prototype.SetSizeCore.call(this, sizePropertyName, size, getFunctionName, corrected);
        },
        AdjustControlCoreInternal: function () {
            this.AllowRubberLayout();
            var mainElementStyle = {
                width: SizeUtils.GetClientWidth(this.GetMainElement(), true),
                height: SizeUtils.GetClientHeight(this.GetMainElement(), true)
            };
            var passePartourStyle = {
                width: mainElementStyle.width,
                height: mainElementStyle.height
            };

            this.AdjustNavigationBar(mainElementStyle, passePartourStyle);
            this.AdjustImageArea(mainElementStyle, passePartourStyle);

            ASPx.SetStyles(this.GetMainElement(), mainElementStyle);
            if(this.showImageArea)
                ASPx.SetStyles(this.GetPassePartoutElement(), passePartourStyle);

            this.DenyRubberLayout();
        },
        AdjustNavigationBar: function (mainElementStyle, passePartourStyle) {
            var navigationBar = this.GetNavigationBarStrategy();
            if(navigationBar) {
                navigationBar.AdjustControl();
                var width = navigationBar.GetWidth();
                var height = navigationBar.GetHeight();
                if(this.showImageArea) {
                    switch (this.GetNavigationBarPosition()) {
                        case NavigationBarPositionEnum.Right:
                        case NavigationBarPositionEnum.Left:
                            passePartourStyle.float = "left";
                            passePartourStyle.width -= width;
                            break;
                        case NavigationBarPositionEnum.Bottom:
                        case NavigationBarPositionEnum.Top:
                            passePartourStyle.height -= height;
                            break;
                    }
                }
                else {
                    mainElementStyle.width = width;
                    mainElementStyle.height = height;
                }
            }
        },
        AdjustImageArea: function (mainElementStyle, passePartourStyle) {
            var imageAreaStrategy = this.GetImageAreaStrategy();
            if(imageAreaStrategy) {
                imageAreaStrategy.AdjustControl(passePartourStyle.width, passePartourStyle.height);
                var imageAreaWidth = imageAreaStrategy.GetWidth();
                var imageAreaHeight = imageAreaStrategy.GetHeight();

                var navigationBar = this.GetNavigationBarStrategy();
                var navigationBarWidht = navigationBar ? navigationBar.GetWidth() : 0;
                var navigationBarHeight = navigationBar ? navigationBar.GetHeight() : 0;

                if(imageAreaWidth > passePartourStyle.width) {
                    passePartourStyle.width = imageAreaWidth;
                    mainElementStyle.width = imageAreaWidth + navigationBarWidht;
                }
                if(imageAreaHeight > passePartourStyle.height) {
                    passePartourStyle.height = imageAreaHeight;
                    mainElementStyle.height = imageAreaHeight + navigationBarHeight;
                }
            }
        },
        AllowRubberLayout: function () {
            this.ResetControlStyle();
            ASPx.SetStyles(this.GetMainElement(), {
                borderSpacing: 0,
                overflow: "hidden",
                display: "table" //Q495945
            });
            if(this.showImageArea)
                ASPx.SetStyles(this.GetPassePartoutElement(), { width: "", height: "" });
        },
        DenyRubberLayout: function () {
            ASPx.SetStyles(this.GetMainElement(), {
                display: "",
                overflow: "",
                borderSpacing: ""
            });
            if(this.showImageArea)
                ASPx.SetStyles(this.GetPassePartoutElement(), { display: "" });
        },
        ResetControlStyle: function () {
            ASPx.SetStyles(this.GetMainElement(), this.defaultControlSize);
            var navigationBar = this.GetNavigationBarStrategy();
            if(navigationBar)
                navigationBar.ResetControlStyle();
            var imageAreaStrategy = this.GetImageAreaStrategy();
            if(imageAreaStrategy)
                imageAreaStrategy.ResetControlStyle();
        },
        CreateControlHierarchy: function () {
            if(this.showImageArea) {
                this.passePartoutElement = document.createElement("DIV");
                this.GetMainElement().appendChild(this.passePartoutElement);
                this.imageAreaStrategy = this.CreateImageAreaStrategy();
            }
            if(this.showNavBar)
                this.navigationBarStrategy = this.CreateNavigationBarStrategy();
            this.RaiseStrategiesCreated();
        },
        CreateImageAreaStrategy: function () {
            switch (this.GetAnimationType()) {
                case AnimationTypeEnum.Slide:
                    return this.CreateSlideImageAreaStrategy();
                    break;
                case AnimationTypeEnum.Fade:
                    return new FadeImageAreaStrategy(this);
                    break;
                case AnimationTypeEnum.None:
                    return new NoneImageAreaStrategy(this);
                    break;
            }
        },
        CreateSlideImageAreaStrategy: function () {
            return this.enableLoopNavigation ? new SlideLoopNavigationImageAreaStrategy(this) : new SlideImageAreaStrategy(this);
        },
        CreateNavigationBarStrategy: function () {
            switch (this.GetNavigationBarMode()) {
                case NavigationBarModeEnum.Thumbnails:
                    return new NavigationBarThumbnailStrategy(this);
                    break;
                case NavigationBarModeEnum.Dots:
                    return new NavigationBarDotsStrategy(this);
                    break;
            }
        },
        PrepareControlHierarchy: function () {
            this.PrepareMainElement();
            if(this.showImageArea)
                this.PreparePassePartoutElement();
            if(this.autoPlay)
                this.Play();
        },
        PrepareMainElement: function () {
            var mainElement = this.GetMainElement();
            mainElement.tabIndex = Math.max(mainElement.tabIndex, 0);
            ASPx.SetElementDisplay(mainElement, true);
        },
        PreparePassePartoutElement: function () {
            ASPx.SetStyles(this.GetPassePartoutElement(), {
                cssText: this.passePartoutStyle,
                className: CssClassesConstants.PassePartoutCssClassName
            });

        },
        /* Handlers */
        OnBrowserWindowResize: function(evt) {
            this.AdjustControl();
        },
        OnKeyDown: function (evt) {
            var keyKode = ASPx.Evt.GetKeyCode(evt);
            switch (keyKode) {
                case ASPx.Key.Left:
                    if(this.rtl) {
                        this.SetActiveItemIndex(this.GetActiveItemIndex() + 1);
                        return ASPx.Evt.PreventEvent(evt);
                    }
                case ASPx.Key.Up:
                    this.SetActiveItemIndex(this.GetActiveItemIndex() - 1);
                    return ASPx.Evt.PreventEvent(evt);
                case ASPx.Key.Right:
                    if(this.rtl) {
                        this.SetActiveItemIndex(this.GetActiveItemIndex() - 1);
                        return ASPx.Evt.PreventEvent(evt);
                    }
                case ASPx.Key.Down:
                    this.SetActiveItemIndex(this.GetActiveItemIndex() + 1);
                    return ASPx.Evt.PreventEvent(evt);
                case ASPx.Key.Home:
                    this.SetActiveItemIndex(0);
                    return ASPx.Evt.PreventEvent(evt);
                case ASPx.Key.End:
                    this.SetActiveItemIndex(this.GetItemCount() - 1);
                    return ASPx.Evt.PreventEvent(evt);
            }
        },
        /* Get elements */
        GetImageAreaStrategy: function () {
            return this.imageAreaStrategy;
        },
        GetNavigationBarStrategy: function () {
            return this.navigationBarStrategy;
        },
        GetPassePartoutElement: function () {
            return this.passePartoutElement;
        },
        GetItem: function (index) {
            var item = this.GetItemInternal(index);
            if(item)
                return new ASPxClientImageSliderItem(this, index, item.n, item.s, item.t);
            return null;
        },
        GetItemByName: function (name) {
            for(var i = 0; i < this.GetItemCount() ; i++)
                if(this.GetItemInternal(i).n == name)
                    return this.GetItem(i);
            return null;
        },
        GetActiveItemIndex: function () {
            return this.GetRtlIndex(this.GetActiveItemIndexInternal());
        },
        SetActiveItemIndex: function (index, preventAnimation) {
            this.SetActiveItemIndexInternal(this.GetRtlIndex(index), preventAnimation);
        },
        GetActiveItem: function () {
            return this.GetItem(this.GetActiveItemIndex());
        },
        SetActiveItem: function (item, preventAnimation) {
            this.SetActiveItemIndex(item.index, preventAnimation);
        },
        GetItemCount: function () {
            return this.items.length;
        },
        Focus: function () {
            if(!ASPx.Browser.TouchUI && this.enableKeyboardSupport)
                ASPx.SetFocus(this.GetMainElement());
        },
        Play: function () {
            if(this.enabled) {
                this.PlayInternal();
                this.slideShowEnabled = true;
            }
        },
        Pause: function () {
            if(this.enabled) {
                this.PauseInternal();
                this.slideShowEnabled = false;
            }
        },
        IsSlideShowPlaying: function () {
            return this.timerID != -1;
        },
        /* Private api */
        IsFocused: function () {
            return ASPx.GetActiveElement() == this.GetMainElement();
        },
        PlayInternal: function () {
            if(this.IsSlideShowPlaying())
                return;
            this.timerID = window.setInterval(function () { this.DoSeActivetNextItem(); }.aspxBind(this), this.interval);
            this.stopPlayingWhenPagingExecuted = false;
            if(this.GetImageAreaStrategy())
                this.GetImageAreaStrategy().SetSlideShowButtonState(false);
        },
        PauseInternal: function () {
            this.timerID = ASPx.Timer.ClearInterval(this.timerID);
            if(this.GetImageAreaStrategy())
                this.GetImageAreaStrategy().SetSlideShowButtonState(true);
        },
        ResetSlideShowTimer: function () {
            if(this.IsSlideShowPlaying()) {
                this.timerID = ASPx.Timer.ClearInterval(this.timerID);
                this.timerID = window.setInterval(function () { this.DoSeActivetNextItem(); }.aspxBind(this), this.interval);
            }
        },
        StopPlayingWhenPaging: function () {
            if(this.stopPlayingWhenPaging) {
                this.Pause();
                this.stopPlayingWhenPagingExecuted = true;
            }
        },
        DoSeActivetNextItem: function () {
            var activeIndex = this.GetActiveItemIndex() + 1;
            if(activeIndex >= this.GetItemCount())
                activeIndex = 0;
            this.SetActiveItemIndex(activeIndex);
        },
        RaiseStrategiesCreated: function () {
            if(!this.StrategiesCreated.IsEmpty())
                this.StrategiesCreated.FireEvent(this, null);
        },
        RaiseActiveItemChanged: function () {
            this.RaiseItemEvent(this.ActiveItemChanged);
        },
        RaiseItemClick: function () {
            this.RaiseItemEvent(this.ItemClick);
        },
        RaiseThumbnailItemClick: function () {
            this.RaiseItemEvent(this.ThumbnailItemClick);
        },
        RaiseItemEvent: function (event) {
            if(!event.IsEmpty()) {
                var args = new ASPxClientImageSliderItemEventArgs(this.GetActiveItem());
                event.FireEvent(this, args);
            }
        },
        SetActiveItemIndexInternal: function (index, preventAnimation) {
            if(index >= 0 && index < this.GetItemCount() && index != this.GetActiveItemIndexInternal()) {
                if(this.GetImageAreaStrategy())
                    this.GetImageAreaStrategy().SetActiveItemIndex(index, preventAnimation);
                if(this.GetNavigationBarStrategy())
                    this.GetNavigationBarStrategy().SetActiveItemIndex(index, preventAnimation);

                this.ResetSlideShowTimer();
                this.RaiseActiveItemChanged();
            }
        },
        GetActiveItemIndexInternal: function() {
            var strategy = this.GetImageAreaStrategy() || this.GetNavigationBarStrategy();
            if(strategy)
                return strategy.GetActiveItemIndex();
            return 0;
        },
        GetItemInternal: function (index) {
            if(index < 0 || index >= this.GetItemCount())
                return null;
            return this.items[index];
        },
        GetRtlIndex: function (index) {
            if(this.rtl)
                return this.GetItemCount() - 1 - index;
            return index;
        },
        /* Properties */
        GetAnimationType: function () {
            return this.animationType;
        },
        HasNavigateUrls: function () {
            return this.hasNavigateUrls;
        },
        HasTemplates: function () {
            return this.hasTemplates;
        },
        GetNavigationBarPosition: function () {
            return this.navBarPos;
        },
        GetNavigationBarMode: function () {
            return this.dotHCssCl != "" ? 1 : 0;
        }
    });
    ASPxClientImageSlider.Cast = ASPxClientControl.Cast;
    var ASPxClientImageSliderItemEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function (item) {
            this.constructor.prototype.constructor.call(this);
            this.item = item;
        }
    });
    var ASPxClientImageSliderItem = ASPx.CreateClass(null, {
        constructor: function (imageSlider, index, name, imageUrl, text) {
            this.imageSlider = imageSlider;
            this.index = index;
            this.name = name;
            this.imageUrl = imageUrl;
            this.text = text;
        }
    });
    var ASPxClientImageZoomNavigator = ASPx.CreateClass(ASPxClientImageSlider, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);
            this.disableSelectedStateAnimation = true;

            this.imageZoomName = "";
            this.changeAction = 0;
            this.buttonVisibilityMode = 0;
        },
        Initialize: function () {
            ASPxClientImageSlider.prototype.Initialize.call(this);
            this.ActiveItemChanged.AddHandler(function () {
                this.OnActiveItemChanged();
            }.aspxBind(this));
        },
        CreateNavigationBarStrategy: function () {
            return new ImageZoomNavigationBarStrategy(this);
        },
        PrepareControlHierarchy: function () {
            ASPxClientImageSlider.prototype.PrepareControlHierarchy.call(this);
            ASPx.AddClassNameToElement(this.GetMainElement(), CssClassesConstants.ImageZoomNavigatorCssClassName);
        },
        OnActiveItemChanged: function () {
            this.UpdageImageZoomImages();
        },
        UpdageImageZoomImages: function() {
            var imageZoom = this.GetImageZoom();
            if(imageZoom) {
                var item = this.GetItemInternal(this.GetActiveItemIndex());
                imageZoom.SetImageProperties(item.s, item.liu || item.s, item.zt, item.et, item.zt || item.at);
            }
        },
        GetImageZoom: function() {
            return this.imageZoomName ? ASPx.GetControlCollection().Get(this.imageZoomName) : null;
        }
    });
    var ImageZoomNavigationBarStrategy = ASPx.CreateClass(NavigationBarThumbnailStrategy, {
        constructor: function (imageZoomNavigator) {
            this.visibleItemCount = 0;
            this.constructor.prototype.constructor.call(this, imageZoomNavigator);
        },
        InitializeHandlers: function () {
            if(this.GetImageSlider().changeAction == ActiveItemChangeActionEnum.Hover) {
                ItemsOwnerBase.prototype.InitializeHandlers.call(this);
                ASPx.Evt.AttachEventToElement(this.GetSlidePanelElement(), "mouseover", function (evt) { this.OnNavigationBarMouseOver(evt); }.aspxBind(this));
            }
            else
                NavigationBarThumbnailStrategy.prototype.InitializeHandlers.call(this);
        },
        OnNavigationBarMouseOver: function (evt) {
            if(!this.IsExecutedGesture()) {
                var index = this.GetItemIndexFromEvent(evt);
                if(index != -1)
                    this.GetImageSlider().SetActiveItemIndexInternal(index);
            }
        },
        GetEnableItemHoverState: function () {
            return this.GetImageSlider().changeAction == ActiveItemChangeActionEnum.Click;
        },
        GetItemAlternateText: function (index) {
            if(this.IsValidItemIndex(index)) {
                var item = this.GetItem(index);
                return item.zt || item.at || "";
            }
            return "";
        },
        CalculateSize: function() {
            NavigationBarThumbnailStrategy.prototype.CalculateSize.call(this);
            if(!this.CanUseVisibleItemCount()) {
                var isHorizontal = this.IsHorizontalNavigation();
                var size = {
                    width: isHorizontal ? this.GetItemWidthWithSpacing() * this.GetItemCount() - this.GetItemSpacing() : this.GetItemElementWidth(),
                    height: isHorizontal ? this.GetItemElementHeight() : this.GetItemHeightWithSpacing() * this.GetItemCount() - this.GetItemSpacing()
                };
                var mainElementWidth = SizeUtils.GetClientWidth(this.GetMainElement(), true);
                var mainElementHeight = SizeUtils.GetClientHeight(this.GetMainElement(), true);

                if(isHorizontal && size.width < mainElementWidth || !isHorizontal && size.height < mainElementHeight) {
                    this.visibleItemCount = this.GetItemCount();
                    if(this.GetImageSlider().buttonVisibilityMode == NavigationButtonVisibilityModeEnum.Auto)
                        this.SetVisibilityButtons(false);
                }
            }
        },
        UpdateNavigationButtonsState: function () {
            if(this.GetVisibleItemCount() != this.GetItemCount() || this.GetImageSlider().buttonVisibilityMode != NavigationButtonVisibilityModeEnum.Auto)
                return NavigationBarThumbnailStrategy.prototype.UpdateNavigationButtonsState.call(this);
            return false;
        },
        CanCreateNavigationButtons: function() {
            if(this.GetImageSlider().buttonVisibilityMode == NavigationButtonVisibilityModeEnum.Always)
                return true;
            return NavigationBarThumbnailStrategy.prototype.CanCreateNavigationButtons.call(this);
        },
        GetVisibleItemCount: function() {
            return this.GetImageSlider().visibleItemsCount || this.visibleItemCount;
        }
    });

    window.ASPxClientImageSlider = ASPxClientImageSlider;
    window.ASPxClientImageSliderItemEventArgs = ASPxClientImageSliderItemEventArgs;
    window.ASPxClientImageSliderItem = ASPxClientImageSliderItem;
    window.ASPxClientImageZoomNavigator = ASPxClientImageZoomNavigator;
})();