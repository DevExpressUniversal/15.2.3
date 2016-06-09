/// <reference path="_references.js"/>

(function () {
    var setAttributeIfValueExists = function(element, name, value) {
        if(ASPx.IsExists(value))
            ASPx.Attr.SetAttribute(element, name, value);
    };

    var CssClasses = {
        HeaderContentWrapper: "dxrpHCW",
        AnimationWrapper: "dxrpAW",
        ContentWrapper: "dxrpCW",
        Collapsed: "dxrpCollapsed",
        CollapseButtonRtl: "dxrpCollapseButtonRtl",
        CollapseButton: "dxrpCollapseButton"
    };
    var IDSuffixes = {
        HeaderElement: "_HC",
        GroupBoxCaption: "_GBC",
        ContentElement: "_RPC",
        HeaderTextContainer: "_RPHT"
    };
    var VerticalAlignEnum = {
        Top: 1,
        Middle: 2,
        Bottom: 3,
        NotSet: 0
    };
    var ASPxClientRoundPanel = ASPx.CreateClass(ASPxClientPanelBase, {
        constructor: function (name) {
            this.constructor.prototype.constructor.call(this, name);
            this.animationDuration = 200;
            this.enableAnimation = true;
            this.allowCollapsingByHeaderClick = false;
            this.collapsed = false;
            this.isCollapsingAllowed = null;
            this.headerTextEmpty = false;
            this.collapseButton = null;
            this.collapseButtonImage = null;
            this.contentWrapper = null;
            this.animationWrapper = null;
            this.headerContentWrapper = null;

            this.isGroupBox = false;

            this.contentHeightBeforeCollapse = 0;
            this.contentWrapperHeight = 0;
            this.initialContentHeight = 0;
            this.tableInlineHeight = "";
            this.contentInlineHeight = "";
            this.headerCollapsedBorderSettings = {
                borderBottom: "",
                borderBottomLeftRadius: "",
                borderBottomRightRadius: ""
            };
            this.headerExpandedBorderSettings = {
                borderBottom: "",
                borderBottomLeftRadius: "",
                borderBottomRightRadius: ""
            };
            this.headerVerticalAlign = VerticalAlignEnum.Middle;

            this.loadContentViaCallback = false;
            this.isContentLoaded = false;
            this.CollapsedChanged = new ASPxClientEvent();
            this.CollapsedChanging = new ASPxClientEvent();
        },
        SetEnabled: function (enabled) {
            ASPx.GetStateController().SetElementWithChildNodesEnabled(this.name, enabled);
            this.isCollapsingAllowed = enabled;
            ASPxClientPanelBase.prototype.SetEnabled.call(this, enabled);
        },
        AdjustHeader: function () {
            var collapseButton = this.GetCollapseButton();
            var textWrapper = this.GetHeaderContentWrapper();
            if (!collapseButton || !textWrapper)
                return;
            var offset = Math.floor((collapseButton.offsetHeight - textWrapper.offsetHeight) * this.GetMarginTopMultiplier());
            if (offset > 0)
                textWrapper.style.marginTop = Math.abs(offset) + "px";
            if (offset < 0)
                collapseButton.style.marginTop = Math.abs(offset) + "px";
            var computedStyle = ASPx.GetCurrentStyle(collapseButton);
            var padding = collapseButton.offsetWidth + ASPx.PxToInt(computedStyle.marginRight) + ASPx.PxToInt(computedStyle.marginLeft);
            if (this.rtl)
                textWrapper.style.paddingLeft = padding + "px";
            else
                textWrapper.style.paddingRight = padding + "px";
        },
        GetMarginTopMultiplier: function () {
            switch (this.headerVerticalAlign) {
                case VerticalAlignEnum.Middle:
                    return 0.5;
                case VerticalAlignEnum.Bottom:
                    return 1;
                default:
                    return 0;
            }
        },
        GetHeaderContentWrapper: function () {
            if (this.headerContentWrapper == null)
                this.headerContentWrapper = ASPx.GetChildByClassName(this.GetHeaderElement(), CssClasses.HeaderContentWrapper);
            return this.headerContentWrapper;
        },
        InlineInitialize: function () {
            ASPxClientPanelBase.prototype.InlineInitialize.call(this);

            this.SetCollapsed(this.collapsed);
            this.tableInlineHeight = this.GetMainElement().style.height;
            this.contentInlineHeight = this.GetContentElement().style.height;
            this.initialContentHeight = (+ASPx.PxToInt(this.contentInlineHeight));
            this.isContentLoaded = !this.GetCollapsed();
        },
        Initialize: function () {
            this.RemoveCollapsedCssClass();

            ASPxClientPanelBase.prototype.Initialize.call(this);

            this.InitializeHeaderBorderSettings(this.headerExpandedBorderSettings, false);
            this.InitializeHeaderBorderSettings(this.headerCollapsedBorderSettings, true);
            if (this.IsCollapsingAllowed()) {
                ASPx.Evt.AttachEventToElement(this.GetHeaderElement(), ASPx.TouchUIHelper.touchMouseUpEventName,
                    function (evt) {
                        if (ASPx.Evt.IsLeftButtonPressed(evt))
                            this.OnHeaderMouseUp(evt);
                    }.aspxBind(this));

                if (this.GetCollapsed()) {
                    if (this.GetAnimationWrapper())
                        this.contentHeightBeforeCollapse = this.GetAnimationWrapper().offsetHeight;
                    this.AssignCollapsedStyle();
                }
            }
            if (!this.clientEnabled)
                this.SetEnabled(this.clientEnabled);
        },
        GetAffectingSizeBordersAndPaddings: function (element, isVertical) {
            var boxSizing = ASPx.GetCurrentStyle(element).MozBoxSizing || ASPx.GetCurrentStyle(element).boxSizing;
            if (boxSizing == "padding-box")
                return isVertical ? ASPx.GetVerticalBordersWidth(element) : ASPx.GetHorizontalBordersWidth(element);
            else if (boxSizing == "border-box")
                return 0;
            else
                return isVertical ? ASPx.GetTopBottomBordersAndPaddingsSummaryValue(element) :
                                    ASPx.GetLeftRightBordersAndPaddingsSummaryValue(element);
        },
        InitializeHeaderBorderSettings: function (settings, isTopRadius) {
            if (!this.GetHeaderElement())
                return;
            var currentStyle = ASPx.GetCurrentStyle(this.GetHeaderElement());
            if (isTopRadius) {
                settings.borderBottomLeftRadius = currentStyle.borderTopLeftRadius;
                settings.borderBottomRightRadius = currentStyle.borderTopRightRadius;
                settings.borderBottom = currentStyle.borderTopWidth + " " + currentStyle.borderTopStyle + " " + currentStyle.borderTopColor;
            } else {
                settings.borderBottomLeftRadius = currentStyle.borderBottomLeftRadius;
                settings.borderBottomRightRadius = currentStyle.borderBottomRightRadius;
                settings.borderBottom = currentStyle.borderBottomWidth + " " + currentStyle.borderBottomStyle + " " + currentStyle.borderBottomColor;
            }
        },
        OnHeaderMouseUp: function (evt) {
            var source = ASPx.Evt.GetEventSource(evt);
            var isButtonClick = source == this.GetCollapseButton() || source == this.GetCollapseButtonImage();
            if (isButtonClick || (this.allowCollapsingByHeaderClick && !ASPx.IsInteractiveControl(source))) {
                this.setCollapsedInternal(!this.GetCollapsed(), true);
            }
        },
        GetContentContainer: function () {
            return this.GetContentWrapper() || this.GetContentElement();
        },
        PerformCallback: function (parameter) {
            if (this.CanPerformCallback())
                this.CreateCallback(parameter);
        },
        CreateCallback: function (arg, command, callbackInfo) {
            ASPxClientControl.prototype.CreateCallback.call(this, arg, command);
            this.ShowLoadingElementsInternal();
        },
        OnCallback: function (html) {
            this.isContentLoaded = true;
            this.UpdateContentHtml(html);
        },
        UpdateContentHtml: function (html) {
            if (this.GetAnimationWrapper()) {
                var size = this.lockElementSize(this.GetAnimationWrapper(), false);
                this.SetContentHtmlInternal(html);
                this.StretchAnimationWrapper(size);
            } 
            else 
                this.SetContentHtmlInternal(html);
        },
        SetContentHtmlInternal: function (html) {
            var container = this.GetContentContainer();
            ASPx.SetInnerHtml(container, html);
            this.lockElementSize(container, true);
        },
        lockElementSize: function (element, removeBlock) {
            var size = {
                width: element.offsetWidth - this.GetAffectingSizeBordersAndPaddings(element, false),
                height: element.offsetHeight - this.GetAffectingSizeBordersAndPaddings(element, true)
            };
            ASPx.SetStyles(element, {
                overflow: removeBlock ? "" : "hidden",
                width: removeBlock ? "" : size.width,
                height: removeBlock ? "" : size.height
            });
            return size;
        },
        StretchAnimationWrapper: function (size) {
            var contentWrapper = this.GetContentWrapper();
            ASPx.SetStyles(contentWrapper, { display: "table", height: "0px" });
            var width = contentWrapper.offsetWidth;
            var height = contentWrapper.offsetHeight;
            ASPx.Attr.RemoveStyleAttribute(contentWrapper, "height");
            ASPx.Attr.RemoveStyleAttribute(contentWrapper, "display");

            this.StartAnimation(this.GetAnimationWrapper(), function () {
                this.lockElementSize(this.GetAnimationWrapper(), true);
                this.RaiseCollapsedChanged();
            }.aspxBind(this),
            [
                { property: "height", to: height, from: size.height },
                { property: "width", to: width, from: size.width }
            ]);
        },
        ShowLoadingElementsInternal: function () {
            this.RestoreLoadingDivOpacity();
            this.ShowLoadingPanel();
            if (this.lpDelay > 0) {
                var clonedPanel = this.GetClonedLoadingPanel();
                clonedPanel.style.visibility = "hidden";
                window.setTimeout(function () {
                    clonedPanel.style.visibility = "";
                }, this.lpDelay);
            }
        },
        CanPerformCallback: function () {
            return this.loadContentViaCallback && !this.InCallback();
        },
        CanPerformLoadContentCallback: function () {
            return this.CanPerformCallback() && !this.isContentLoaded;
        },
        ShowLoadingPanel: function () {
            var blockContentContainer = !this.IsContentHtmlEmpty();
            if (blockContentContainer)
                this.lockElementSize(this.GetContentContainer());
            this.CreateLoadingPanelInsideContainer(this.GetContentContainer());
            if (!blockContentContainer)
                this.CenterLoadingPanel();
        },
        CenterLoadingPanel: function () {
            var loadingPanel = this.GetClonedLoadingPanel();
            if (!loadingPanel)
                return;
            var contentContainer = this.GetContentContainer();
            var parentHeight = ASPx.GetClearClientHeight(contentContainer);
            if (parentHeight > loadingPanel.offsetHeight)
                ASPx.SetStyles(loadingPanel, { marginTop: Math.floor((parentHeight - loadingPanel.offsetHeight) / 2) });
        },
        AdjustControlCore: function () {
            if (this.GetHeaderElement())
                this.AdjustHeader();
            this.UpdateContentHeight();
        },
        UpdateContentHeight: function () {
            if (!ASPx.PxToInt(this.contentInlineHeight) || !this.GetContentWrapper())
                return;
            var contentWrapper = this.GetContentWrapper();
            var height = ASPx.PxToInt(this.contentInlineHeight);

            ASPx.SetStyles(contentWrapper, { display: "inline-block", height: "auto" });
            var realContentHeight = contentWrapper.offsetHeight;
            ASPx.Attr.RemoveStyleAttribute(contentWrapper, "height");
            ASPx.Attr.RemoveStyleAttribute(contentWrapper, "display");
            var contentHeight = Math.max(realContentHeight, this.initialContentHeight);
            if (contentHeight > 0) {
                this.contentInlineHeight = Math.max(realContentHeight, this.initialContentHeight) + "px";
                this.GetContentElement().style.height = this.contentInlineHeight;
            }
        },
        IsCollapsingAllowed: function () {
            if (this.isCollapsingAllowed != null)
                return this.isCollapsingAllowed;
            return !this.isGroupBox && this.GetHeaderElement() && this.GetContentWrapper();
        },
        GetContentElement: function () {
            return this.GetChildElement(IDSuffixes.ContentElement);
        },
        GetCollapseButton: function () {
            if (this.collapseButton == null)
                this.collapseButton = ASPx.GetNodesByClassName(this.GetMainElement(),
                    this.rtl ? CssClasses.CollapseButtonRtl : CssClasses.CollapseButton)[0];
            return this.collapseButton;
        },
        GetCollapseButtonImage: function () {
            if (this.collapseButtonImage == null)
                this.collapseButtonImage = ASPx.GetNodeByTagName(this.GetCollapseButton(), "IMG", 0);
            return this.collapseButtonImage;
        },
        GetHeaderElement: function () {
            return this.GetChildElement(IDSuffixes.HeaderElement);
        },
        GetGroupBoxCaptionElement: function () {
            return this.GetChildElement(IDSuffixes.GroupBoxCaption);
        },
        GetHeaderTextContainer: function () {
            return this.GetChildElement(IDSuffixes.HeaderTextContainer);
        },
        GetHeaderText: function () {
            var textContainer = this.GetHeaderTextContainer();
            if (ASPx.IsExistsElement(textContainer) && textContainer.innerHTML != "&nbsp;")
                return ASPx.Str.Trim(textContainer.innerHTML);
            return "";
        },
        SetHeaderText: function (text) {
            var textContainer = this.GetHeaderTextContainer();
            if (ASPx.IsExistsElement(textContainer))
                textContainer.innerHTML = this.GetPreparedText(text);
        },
        GetContentHtml: function () {
            var contentElement = this.GetContentContainer();
            if (ASPx.IsExistsElement(contentElement)) {
                if (this.isGroupBox)
                    var caption = this.RemoveGroupBoxCaptionElement();
                var contentHTML = contentElement.innerHTML;
                if (this.isGroupBox)
                    this.RestoreGroupBoxCaptionElement(caption);
                if (contentHTML == "&nbsp;")
                    contentHTML = "";
                return contentHTML;
            }
            return null;
        },
        SetContentHtml: function (html) {
            var contentElement = this.GetContentContainer();
            if (ASPx.IsExistsElement(contentElement)) {
                if (this.isGroupBox)
                    var caption = this.RemoveGroupBoxCaptionElement();
                ASPx.SetInnerHtml(contentElement, this.GetPreparedText(html));
                if (this.isGroupBox)
                    this.RestoreGroupBoxCaptionElement(caption);
                this.UpdateContentHeight();
            }
        },
        IsContentHtmlEmpty: function () {
            return !ASPx.Str.Trim(this.GetContentHtml());
        },
        GetPreparedText: function (text) {
            if (!text || ASPx.Str.Trim(text) == "")
                text = "&nbsp;";
            return text;
        },
        RemoveGroupBoxCaptionElement: function () {
            var captionElement = this.GetGroupBoxCaptionElement();
            return captionElement ? captionElement.parentNode.removeChild(captionElement) : null;
        },
        RestoreGroupBoxCaptionElement: function (captionElement) {
            var contentElement = this.GetContentElement();
            if (contentElement) {
                if (contentElement.hasChildNodes())
                    contentElement.insertBefore(captionElement, contentElement.firstChild);
                else
                    contentElement.appendChild(captionElement);
            }
        },
        GetCollapsed: function () {
            if (!this.IsCollapsingAllowed())
                return false;
            return this.collapsed;
        },
        SetCollapsed: function (collapsed) {
            var isUserAction = false;
            this.setCollapsedInternal(collapsed, isUserAction);
        },
        setCollapsedInternal: function (collapsed, isUserAction) {
            if (isUserAction) {
                var args = this.RaiseCollapsedChanging();
                if (args && args.cancel) {
                    return;
                };
            }
            if (this.IsCollapsingAllowed() && this.GetCollapsed() != collapsed) {
                this.collapsed = collapsed;
                this.ToggleAppearance();
            }
        },
        UpdateStateObject: function(){
            this.UpdateStateObjectWithObject({ collapsed: this.collapsed });
        },

        ToggleAppearance: function () {
            this.switchButtonAppearance();
            if (this.GetCollapsed())
                this.CollapseContent();
            else
                this.ExpandContent();
            if (!this.enableAnimation)
                this.RaiseCollapsedChanged();
        },
        switchButtonAppearance: function() {
            var image = this.GetCollapseButtonImage();
            if(!image)
                return;
            var props = this.GetCollapsed() ? this.expandImageProperties : this.collapseImageProperties;
            setAttributeIfValueExists(image, "src", props.s);
            setAttributeIfValueExists(image, "alt", props.a);
            setAttributeIfValueExists(image, "title", props.t);
            var styles = {};
            if(ASPx.IsExists(props.so.spriteBackground))
                styles.background = props.so.spriteBackground;
            if(ASPx.IsExists(props.w))
                styles.width = props.w;
            if(ASPx.IsExists(props.h))
                styles.height = props.h;
            if(ASPx.IsExists(props.so.spriteCssClass))
                styles.className = props.so.spriteCssClass;
            ASPx.SetStyles(image, styles);
        },
        GetAnimationWrapper: function () {
            if (this.animationWrapper == null)
                this.animationWrapper = ASPx.GetChildByClassName(this.GetContentElement(), CssClasses.AnimationWrapper);
            return this.animationWrapper;
        },
        GetContentWrapper: function () {
            if (this.contentWrapper == null)
                this.contentWrapper = ASPx.GetNodeByClassName(this.GetContentElement(), CssClasses.ContentWrapper);
            return this.contentWrapper;
        },
        CollapseContent: function () {
            if (this.enableAnimation) {
                this.PrepareElementsToCollapseAnimation();
                this.StartAnimation(this.GetAnimationWrapper(), function () {
                    this.CompleteCollapsing();
                    this.RaiseCollapsedChanged();
                }.aspxBind(this), [{ property: "height", to: 0, from: this.contentHeightBeforeCollapse }]);
            } else
                this.CompleteCollapsing();
        },
        UpdateContentHeightBeforeExpand: function () {
            if (this.enableAnimation) {
                var deltaHeight = this.GetContentWrapper().offsetHeight - this.contentWrapperHeight;
                if (deltaHeight != 0 && (this.contentHeightBeforeCollapse + deltaHeight) > 0)
                    this.contentHeightBeforeCollapse += deltaHeight;
            }
        },
        ExpandContent: function () {
            var contentLoadsOnCallback = this.CanPerformLoadContentCallback();
            if (contentLoadsOnCallback)
                this.PerformCallback();
            this.UpdateContentHeightBeforeExpand();
            this.RemoveCollapsedCssClass();
            this.PrepareElementsToExpand();
            if (this.enableAnimation)
                this.StartAnimation(this.GetAnimationWrapper(), function () {
                    this.CompleteExpanding();
                    if (!contentLoadsOnCallback)
                        this.RaiseCollapsedChanged();
                }.aspxBind(this), [{ property: "height", from: 0, to: this.contentHeightBeforeCollapse }]);
        },
        PrepareElementsToCollapseAnimation: function () {
            this.contentHeightBeforeCollapse = this.GetAnimationWrapper().offsetHeight;
            ASPx.Attr.RemoveStyleAttribute(this.GetContentElement(), "height");
            ASPx.ClearHeight(this.GetMainElement());
        },
        GetContentHeight: function () {
            return this.GetContentElement().offsetHeight;
        },
        CompleteCollapsing: function () {
            this.AssignCollapsedStyle();
            if (this.enableAnimation)
                this.contentWrapperHeight = this.GetContentWrapper().offsetHeight;
        },
        CompleteExpanding: function () {
            ASPx.ClearHeight(this.GetAnimationWrapper());
            if (this.tableInlineHeight)
                ASPx.SetStyles(this.GetMainElement(), { height: this.tableInlineHeight });
            if (this.contentInlineHeight)
                ASPx.SetStyles(this.GetContentElement(), { height: this.contentInlineHeight });
            this.UpdateContentHeight();
        },
        AssignCollapsedStyle: function () {
            ASPx.AddClassNameToElement(this.GetMainElement(), CssClasses.Collapsed);
            ASPx.SetStyles(this.GetHeaderElement(), this.headerCollapsedBorderSettings);
            this.ChangeSelectStateForCollapseButton();
        },
        PrepareElementsToExpand: function () {
            ASPx.SetStyles(this.GetHeaderElement(), this.headerExpandedBorderSettings);
            this.ChangeSelectStateForCollapseButton();
        },
        RemoveCollapsedCssClass: function () {
            var element = this.GetMainElement();
            var regex = new RegExp("(?:^|\\s)" + CssClasses.Collapsed + "(?!\\S)");
            element.className = element.className.replace(regex, "");
        },
        StartAnimation: function (element, onComplete, propsToAnimate) {
            if (!propsToAnimate || !element)
                return;
            var props = [];
            for (var i = 0; i < propsToAnimate.length; i++) {
                if (propsToAnimate[i].from != propsToAnimate[i].to)
                    props.push(propsToAnimate[i]);
            }
            if (props.length == 0)
                return;
            if (props.length == 1) {
                ASPx.AnimationHelper.createAnimationTransition(element, {
                    property: props[0].property, unit: "px",
                    duration: this.animationDuration, onComplete: onComplete
                }).Start(props[0].from, props[0].to);
            } else {
                var animationProperties = {};
                for (var i = 0; i < props.length; i++)
                    animationProperties[props[i].property] = { unit: "px", from: props[i].from, to: props[i].to };
                ASPx.AnimationHelper.createMultipleAnimationTransition(element, {
                    duration: this.animationDuration,
                    onComplete: onComplete
                }).Start(animationProperties);
            }
        },
        RaiseCollapsedChanged: function () {
            if (!this.CollapsedChanged.IsEmpty())
                this.CollapsedChanged.FireEvent(this);
        },
        RaiseCollapsedChanging: function () {
            if (!this.CollapsedChanging.IsEmpty()) {
                var args = new ASPxClientCancelEventArgs();
                this.CollapsedChanging.FireEvent(this, args);
                return args;
            }
            return null;
        },
        ChangeSelectStateForCollapseButton: function () {
            if (!this.IsStateControllerEnabled() || !this.GetCollapseButton())
                return;
            if (this.GetCollapsed())
                ASPx.GetStateController().SelectElementBySrcElement(this.GetCollapseButton());
            else
                ASPx.GetStateController().DeselectElementBySrcElement(this.GetCollapseButton());
        }
    });
    ASPxClientRoundPanel.Cast = ASPxClientControl.Cast;

    window.ASPxClientRoundPanel = ASPxClientRoundPanel;
})();