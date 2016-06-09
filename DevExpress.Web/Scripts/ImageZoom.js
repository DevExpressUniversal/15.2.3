/// <reference path="_references.js"/>
/// <reference path="ImageControlUtils.js"/>

(function() {
    var CssClassesConstants = {};
    CssClassesConstants.Prefix = "dxiz-";
    CssClassesConstants.Lens = CssClassesConstants.Prefix + "lens";
    CssClassesConstants.ClipPanel = CssClassesConstants.Prefix + "clipPanel";
    CssClassesConstants.Hint = CssClassesConstants.Prefix + "hint";
    CssClassesConstants.Wrapper = CssClassesConstants.Prefix + "wrapper";
    CssClassesConstants.ZoomWindowImage = CssClassesConstants.Prefix + "zwImage";
    CssClassesConstants.ExpandWindowImage = CssClassesConstants.Prefix + "ewImage";

    var Constants = {};
    Constants.ZoomWindowID = "_ZW";
    Constants.ExpandWindowID = "_EW";
    Constants.ExpandWindowPadding = 10;
    Constants.AnimationDuration = 200,
    Constants.ContentBoxClassName = "dx-contentBox";

    var MouseBoxOpacityModeEnum = {
        Inside: 0,
        Outside: 1
    }
    var LargeImageLoadModeEnum = {
        AtOnce: 0,
        AfterInitialize: 1,
        OnAction: 2
    }
    var ZoomWindowPosition = {
        Inside: 0,
        Outside: 1
    }
    var ASPxClientImageZoom = ASPx.CreateClass(ASPxClientControl, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);

            this.image = null;

            this.hint = null;
            this.action = null;
            this.zoomWindow = null;
            this.secondaryZoomWindow = null;
            this.expandWindow = null;
            this.wrapperElement = null;

            this.showHint = true;
            this.showZoomWindow = true;
            this.showExpandWindow = true;
            this.hasDefaultSize = false;
            this.largeImageLoadingLockCount = 0;

            this.width = 0;
            this.height = 0;
            this.zoomWindowWidth = "150%";
            this.zoomWindowHeight = "150%";
            this.zoomWindowPosition = ZoomWindowPosition.Outside;
            this.currentZoomWindowPosition = this.zoomWindowPosition;
            this.previousParentNodeOfClipPanel = null;

            this.mouseBoxOpacityMode = MouseBoxOpacityModeEnum.Inside;
            this.largeImageLoadMode = LargeImageLoadModeEnum.AtOnce;

            this.largeImageUrl = "";
			this.navigator = null;
			this.imageZoomNavigatorName = "";
        },
        /* Initialize */
        InlineInitialize: function() {
            ASPxClientControl.prototype.InlineInitialize.call(this);
            var mainElement = this.GetMainElement();
            if(mainElement.style.width && mainElement.style.height) {
                this.width = ASPx.PxToInt(mainElement.style.width);
                this.height = ASPx.PxToInt(mainElement.style.height);
                this.hasDefaultSize = true;
            }
        },
        Initialize: function() {
            ASPxClientControl.prototype.Initialize.call(this);

            var mainElement = this.GetMainElement();
            mainElement.className += " " + Constants.ContentBoxClassName; //T176191
            mainElement.style.display = "block";

            var image = this.GetImage();
            var navigator = this.GetNavigator();
            if(navigator) {
                this.Create();
                window.setTimeout(function () {
                    navigator.imageZoomName = this.name;
                    navigator.UpdageImageZoomImages();
                }.aspxBind(this), 0);
            }
            else {
                if(ASPx.ImageControlUtils.IsImageLoaded(image))
                    this.Create();
                else
                    ASPx.Evt.AttachEventToElement(image, "load", function () { this.Create(); }.aspxBind(this));
            }
        },
        Create: function() {
            this.ApplyImageSize();
            this.CreateControlHierarchy();
            this.ChangeLoadingPanelLocation();
        },
        ApplyImageSize: function() {
            var image = this.GetImage();
            if(this.hasDefaultSize)
                ASPx.ImageControlUtils.ResizeImage(image, {
                    width: this.width, height: this.height, canUseCanvas: false
                });
            else {
                this.width = image.naturalWidth || image.width;
                this.height = image.naturalHeight || image.height;
                ASPx.SetStyles(this.GetMainElement(), {
                    width: this.width,
                    height: this.height
                });
            }
        },
        CreateControlHierarchy: function() {
            if(this.showHint)
                this.hint = new HintControl(this);
            if(this.enabled) {
                this.action = ASPx.Browser.TouchUI ? new TouchAction(this) : new DesktopAction(this);
                if(this.showZoomWindow)
                    this.zoomWindow = this.zoomWindowPosition == ZoomWindowPosition.Outside ? new OutsideZoomWindowControl(this) : new ZoomWindowControl(this);
                if(this.showExpandWindow)
                    this.expandWindow = new ExpandWindowControl(this);
            }
        },
        ChangeLoadingPanelLocation: function() {
            var loadingPanel = this.GetLoadingPanelElement();
            if(loadingPanel) {
                if(loadingPanel.tagName == "DIV") { //TODO
                    var table = loadingPanel.children[0];
                    table.id = loadingPanel.id;
                    table.style.zIndex = loadingPanel.style.zIndex;
                    table.style.display = loadingPanel.style.display;
                    this.GetWrapperElement().appendChild(table.cloneNode(true));
                }
                else
                    this.GetWrapperElement().appendChild(loadingPanel.cloneNode(true));
                ASPx.RemoveElement(loadingPanel);
            }
        },

        ShowLoadingPanel: function() {
            this.HideHint();
            this.action.StopHadle();
            var loadingPanel = this.GetLoadingPanelElement();
            ASPx.SetElementDisplay(loadingPanel, true);
            ASPx.SetStyles(loadingPanel, {
                top: (this.GetMainElement().offsetHeight - loadingPanel.offsetHeight) / 2,
                left: (this.GetMainElement().offsetWidth - loadingPanel.offsetWidth) / 2
            });
            this.GetMainElement().style.overflow = "hidden";
        },
        HideLoadingPanel: function() {
            this.ShowHint();
            this.action.StartHadle();
            ASPx.SetElementDisplay(this.GetLoadingPanelElement(), false);
            this.GetMainElement().style.overflow = "";
        },
        TryHideLoadingPanel: function() {
            var count = this.zoomWindow && this.expandWindow ? 2 : 1;
            if(this.largeImageLoadingLockCount == count) {
                this.largeImageLoadingLockCount = 0;
                this.HideLoadingPanel();
            }
        },
        ShowHint: function() {
            if(this.hint)
                this.hint.Show();
        },
        HideHint: function() {
            if(this.hint)
                this.hint.Hide();
        },

        OnBrowserWindowResize: function(evt) {
            this.expandWindow.AdjustControl();
        },
        BrowserWindowResizeSubscriber: function() {
            return this.expandWindow && this.expandWindow.IsVisible();
        },
        SwapZoomWindows: function() {
            var tempZoomWindow = this.zoomWindow;
            this.zoomWindow = this.secondaryZoomWindow;
            this.secondaryZoomWindow = tempZoomWindow;
        },
        ChangeZoomWindowPosition: function(newPosition) {
            var clipPanel = this.zoomWindow.GetClipPanel();
            var insideClassName = CssClassesConstants.Prefix + "inside";

            if(!this.secondaryZoomWindow)
                this.secondaryZoomWindow = newPosition == ZoomWindowPosition.Outside ? new OutsideZoomWindowControl(this) : new ZoomWindowControl(this);
            this.SwapZoomWindows();

            if(this.currentZoomWindowPosition == ZoomWindowPosition.Outside) {
                ASPx.AddClassNameToElement(clipPanel, insideClassName);
                this.previousParentNodeOfClipPanel = clipPanel.parentNode;
                this.GetWrapperElement().appendChild(clipPanel);
            }
            else {
                ASPx.RemoveClassNameFromElement(clipPanel, insideClassName);
                this.previousParentNodeOfClipPanel.appendChild(clipPanel);
            }
            
            this.currentZoomWindowPosition = newPosition;
            this.zoomWindow.UpdateAppearance();
        },
        IsAccessibilityCompliant: function () {
            return this.accessibilityCompliant;
        },
        SetImageProperties: function (imageUrl, largeImageUrl, zoomWindowText, expandWindowText, alternateText) {
            alternateText = alternateText || "";

            this.ShowLoadingPanel();
            ASPx.ImageControlUtils.ChangeImageSource(this.GetImage(), imageUrl, function () {
                this.ResetImage();
                this.ApplyImageSize();
                this.GetImage().alt = alternateText;

                if(!(this.zoomWindow || this.expandWindow))
                    this.HideLoadingPanel();
                else {
                    if(this.zoomWindow) {
                        this.zoomWindow.SetText(zoomWindowText || "");
                        if(this.secondaryZoomWindow)
                            this.secondaryZoomWindow.SetText(zoomWindowText || "");
                        ASPx.ImageControlUtils.ChangeImageSource(this.zoomWindow.GetImage(), largeImageUrl, function () {
                            this.zoomWindow.ResetImage();
                            this.zoomWindow.UpdateAppearance();

                            this.zoomWindow.GetImage().alt = alternateText;

                            this.largeImageLoadingLockCount++;
                            this.TryHideLoadingPanel();

                            if(this.secondaryZoomWindow)
                                this.secondaryZoomWindow.ResetImage();
                        }.aspxBind(this));
                    }
                    if(this.expandWindow) {
                        this.expandWindow.SetText(expandWindowText || "");
                        ASPx.ImageControlUtils.ChangeImageSource(this.expandWindow.GetImage(), largeImageUrl, function () {
                            this.expandWindow.ResetImage();
                            this.expandWindow.UpdateAppearance();

                            this.expandWindow.GetImage().alt = alternateText;

                            this.largeImageLoadingLockCount++;
                            this.TryHideLoadingPanel();
                        }.aspxBind(this));
                    }
                }
            }.aspxBind(this));
        },

        /* Elements */
        GetImage: function() {
            if(!this.image)
                this.image = this.GetChildElement("I");
            return this.image;
        },
        ResetImage: function() {
            this.image = null;
        },
        GetWrapperElement: function() {
            if(!this.wrapperElement)
                this.wrapperElement = ASPx.GetNodeByClassName(this.GetMainElement(), CssClassesConstants.Wrapper);
            return this.wrapperElement;
        },
		GetNavigator: function() {
			if (!this.navigator)
				this.navigator = ASPx.GetControlCollection().GetByName(this.imageZoomNavigatorName);
			return this.navigator;
		}
    });

    var DesktopAction = ASPx.CreateClass(ASPxClientImageControlBase, {
        constructor: function(imageZoom) {
            this.imageZoom = imageZoom;
            this.zoomWindowLockCount = 0;

            this.mouseX = 0;
            this.mouseY = 0;
            this.zoomOffsetX = 0;
            this.zoomOffsetY = 0;
            this.canHandle = true;

            this.constructor.prototype.constructor.call(this, imageZoom);
        },
        GetImageZoom: function() {
            return this.control;
        },

        GetZoomWindow: function() {
            return this.GetImageZoom().zoomWindow;
        },
        GetExpandWindow: function() {
            return this.GetImageZoom().expandWindow;
        },

        LockZoomWindow: function() {
            this.zoomWindowLockCount++;
        },
        UnlockZoomWindow: function() {
            this.zoomWindowLockCount--;
        },
        IsZoomWindowLocked: function() {
            return this.zoomWindowLockCount > 0;
        },

        StartHadle: function() {
            this.canHandle = true;
        },
        StopHadle: function() {
            this.canHandle = false;
        },

        InitializeHandlers: function() {
            var element = this.GetImageZoom().GetWrapperElement();
            if(this.HasMouseMoveHandler())
                ASPx.Evt.AttachEventToElement(element, ASPx.TouchUIHelper.touchMouseMoveEventName, function(evt) {
                    this.OnMouseMove(evt);
                }.aspxBind(this));
            if(this.HasMouseEnterHandler())
                ASPx.Evt.AttachMouseEnterToElement(element,
                    function() { this.OnMouseIn(); }.aspxBind(this),
                    function() { this.OnMouseOut(); }.aspxBind(this)
                );
            if(this.HasMouseUpHandler())
                ASPx.Evt.AttachEventToElement(element, ASPx.TouchUIHelper.touchMouseUpEventName, function(evt) {
                    this.OnMouseUp(evt);
                }.aspxBind(this));
            if(this.HasMouseDownHandler())
                ASPx.Evt.AttachEventToElement(element, ASPx.TouchUIHelper.touchMouseDownEventName, function(evt) {
                    this.OnMouseDown(evt);
                }.aspxBind(this));
        },
        OnMouseDown: function(evt) {
        },
        OnMouseUp: function(evt) {
            if(!this.canHandle)
                return;
            if(ASPx.Evt.IsLeftButtonPressed(evt)) {
                var zoomWindow = this.GetZoomWindow();
                if(zoomWindow && zoomWindow.IsVisible())
                    zoomWindow.Hide(true);
                this.GetExpandWindow().Show();
            }
        },
        OnMouseMove: function(evt) {
            if(!this.canHandle)
                return;
            if(!this.IsZoomWindowLocked()) {
                this.mouseX = ASPx.Evt.GetEventX(evt) - this.zoomOffsetX;
                this.mouseY = ASPx.Evt.GetEventY(evt) - this.zoomOffsetY;
                this.GetZoomWindow().SetPosition(this.mouseX, this.mouseY);
            }
        },
        OnMouseIn: function() {
            if(!this.canHandle)
                return;
            if(!this.IsZoomWindowLocked()) {
                this.FillZoomOffsets();

                if(this.imageZoom.zoomWindowPosition == ZoomWindowPosition.Outside) {
                    var outsideZoomWindowPopup = this.imageZoom.currentZoomWindowPosition == ZoomWindowPosition.Inside ? this.imageZoom.secondaryZoomWindow.GetPopup() :
                        this.GetZoomWindow().GetPopup();
                    var intersectRect = ImageZoomUtils.GetIntersectRect(ImageZoomUtils.GetElementRect(this.imageZoom.GetMainElement()),
                       ImageZoomUtils.GetPopupRect(outsideZoomWindowPopup));

                    if(intersectRect && this.imageZoom.currentZoomWindowPosition == ZoomWindowPosition.Outside)
                        this.imageZoom.ChangeZoomWindowPosition(ZoomWindowPosition.Inside);
                    else if(!intersectRect && this.imageZoom.currentZoomWindowPosition == ZoomWindowPosition.Inside)
                        this.imageZoom.ChangeZoomWindowPosition(ZoomWindowPosition.Outside);
                }

                this.GetZoomWindow().Show();
            }
        },
        OnMouseOut: function() {
            if(!this.canHandle)
                return;
            if(!this.IsZoomWindowLocked())
                this.GetZoomWindow().Hide();
        },
        HasMouseMoveHandler: function() {
            return this.imageZoom.showZoomWindow;
        },
        HasMouseDownHandler: function() {
            return false;
        },
        HasMouseUpHandler: function() {
            return this.imageZoom.showExpandWindow;
        },
        HasMouseEnterHandler: function() {
            return this.imageZoom.showZoomWindow;
        },

        FillZoomOffsets: function() {
            var mainElement = this.GetImageZoom().GetMainElement();
            this.zoomOffsetX = ASPx.GetAbsoluteX(mainElement);
            this.zoomOffsetY = ASPx.GetAbsoluteY(mainElement);
        }
    });
    var TouchAction = ASPx.CreateClass(DesktopAction, {
        constructor: function(imageZoom) {
            this.constructor.prototype.constructor.call(this, imageZoom);
        },
        HasMouseDownHandler: function() {
            return this.imageZoom.showExpandWindow || this.imageZoom.showZoomWindow;
        },
        HasMouseUpHandler: function() {
            return this.imageZoom.showExpandWindow || this.imageZoom.showZoomWindow;
        },
        HasMouseEnterHandler: function() {
            if(ASPx.Browser.WindowsPlatform && ASPx.Browser.MajorVersion > 10 && this.imageZoom.showZoomWindow)
                return true;
            return false;
        },
        OnMouseMove: function(evt) {
            if(!this.canHandle)
                return;
            if(!this.IsZoomWindowLocked()) {
                DesktopAction.prototype.OnMouseMove.call(this, evt);
                if(!this.GetZoomWindow().IsVisible()) {
                    this.FillZoomOffsets();
                    this.GetZoomWindow().Show();
                }
                ASPx.Evt.PreventEvent(evt);
            }
        },
        OnMouseDown: function(evt) {
            if(!this.canHandle)
                return;
            ASPx.Evt.PreventEvent(evt);
        },
        OnMouseUp: function(evt) {
            if(!this.canHandle)
                return;
            var zoomWindow = this.GetZoomWindow();
            var expandWindow = this.GetExpandWindow();
            if(expandWindow && zoomWindow && !zoomWindow.IsVisible())
                expandWindow.Show();
            else if(zoomWindow)
                zoomWindow.Hide();
        }
    });

    var HintControl = ASPx.CreateClass(null, {
        constructor: function(imageZoom) {
            this.imageZoom = imageZoom;
            this.hintElement = null;
            this.visible = true;
        },
        Show: function() {
            if(!this.visible) {
                this.visible = true;
                this.GetHintElement().style.display = "";
            }
        },
        Hide: function() {
            if(this.visible) {
                this.visible = false;
                this.GetHintElement().style.display = "none";
            }
        },
        GetHintElement: function() {
            if(!this.hintElement)
                this.hintElement = ASPx.GetNodeByClassName(this.imageZoom.GetMainElement(), CssClassesConstants.Hint);
            return this.hintElement;
        }
    });

    var WindowControlBase = ASPx.CreateClass(ASPxClientImageControlBase, {
        constructor: function(imageZoom) {
            this.popup = null;
            this.image = null;

            this.visible = false;
            this.loadImageStarted = false;
            this.imageLoaded = !imageZoom.largeImageUrl;

            this.constructor.prototype.constructor.call(this, imageZoom);
        },
        GetImageZoom: function() {
            return this.control;
        },
        Initialize: function() {
            if(this.GetImageZoom().largeImageLoadMode == LargeImageLoadModeEnum.AfterInitialize)
                this.StartLoadImage();
            else if(this.imageLoaded)
                this.UpdateAppearance();
        },
        UpdateAppearance: function() {
        },

        GetImage: function() {
            if(!this.image)
                this.image = ASPx.GetNodeByClassName(this.GetImageZoom().GetMainElement(), this.GetImageClassName());
            return this.image;
        },
        GetImageClassName: function() {
            return "";
        },
        ResetImage: function() {
            this.image = null;
        },
        GetPopup: function() {
            if(!this.popup)
                this.popup = ASPx.GetControlCollection().Get(this.GetImageZoom().name + this.GetPopupID());
            return this.popup;
        },
        GetPopupID: function() {
            return "";
        },

        StartLoadImage: function() {
            this.GetImageZoom().ShowLoadingPanel();
            var image = this.GetImage();
            ASPx.Evt.AttachEventToElement(image, "load", function() {
                this.OnImageLoad();
            }.aspxBind(this));
            image.src = this.GetImageZoom().largeImageUrl;
            this.loadImageStarted = true;
        },
        OnImageLoad: function() {
            this.imageLoaded = true;
            this.UpdateAppearance();
            this.GetImageZoom().HideLoadingPanel();
            if(this.GetImageZoom().largeImageLoadMode == LargeImageLoadModeEnum.OnAction)
                this.Show();
        },

        Show: function() {
            if(this.GetImageZoom().largeImageLoadMode == LargeImageLoadModeEnum.OnAction && !this.imageLoaded && !this.loadImageStarted)
                this.StartLoadImage();
            else if(this.imageLoaded) {
                this.visible = true;
                this.ShowCore();
                this.GetImageZoom().HideHint();
            }
        },
        ShowCore: function() {
        },
        Hide: function(preventAnimation) {
            if(this.imageLoaded) {
                this.visible = false;
                this.HideCore(preventAnimation);
                this.GetImageZoom().ShowHint();
            }
        },
        HideCore: function(preventAnimation) {
        },
        SetText: function (text) {
            var popup = this.GetPopup();
            popup.SetFooterText(text);
            popup.SetWindowFooterVisible(-1, !!text);
        },
        IsVisible: function() {
            return this.visible;
        }
    });

    var ExpandWindowControl = ASPx.CreateClass(WindowControlBase, {
        constructor: function(imageZoom) {
            this.imageWidth = 0;
            this.imageHeight = 0;

            this.constructor.prototype.constructor.call(this, imageZoom);
        },
        Initialize: function() {
            WindowControlBase.prototype.Initialize.call(this);
            this.SetVisibilityPointerCursor(true);
            this.GetPopup().fadeAnimationDuration = Constants.AnimationDuration;
        },
        InitializeHandlers: function() {
            var popup = this.GetPopup();
            popup.Closing.AddHandler(function() {
                this.OnClosing();
            }.aspxBind(this));
            popup.PopUp.AddHandler(function() {
                this.OnPopup();
            }.aspxBind(this));
            ASPx.Evt.AttachEventToElement(this.GetImage().parentNode, "click", function(evt) {
                this.Hide();
            }.aspxBind(this));
        },
        UpdateAppearance: function() {
            var image = this.GetImage();
            this.imageWidth = image.naturalWidth || image.width;
            this.imageHeight = image.naturalHeight || image.height;
        },
        OnClosing: function() {
            this.GetImageZoom().ShowHint();
            this.GetImageZoom().action.UnlockZoomWindow();
            this.SetVisibilityPointerCursor(true);
        },
        OnPopup: function() {
            this.AdjustControl();
            this.GetImageZoom().action.LockZoomWindow();
            this.SetVisibilityPointerCursor(false);
        },

        ShowCore: function() {
            this.GetPopup().Show();
        },
        HideCore: function() {
            this.GetPopup().Hide();
        },
        GetImageClassName: function() {
            return CssClassesConstants.ExpandWindowImage
        },
        GetPopupID: function() {
            return Constants.ExpandWindowID;
        },
        SetVisibilityPointerCursor: function(visible) {
            this.GetImageZoom().GetWrapperElement().style.cursor = visible ? ASPx.GetPointerCursor() : "";
        },
        AdjustControl: function() {
            var popup = this.GetPopup();
            var image = this.GetImage();

            ASPx.SetStyles(image, {
                width: this.imageWidth,
                height: this.imageHeight
            });
            popup.SetSize(this.imageWidth, this.imageHeight); // T285677

            ASPx.SetStyles(image, {
                height: "100%",
                width: "auto"
            });
            if(popup.GetHeight() >= window.innerHeight)
                popup.SetSize(0, window.innerHeight - Constants.ExpandWindowPadding);

            if(popup.IsVisible())
                popup.UpdatePosition();
        }
    });

    var ZoomWindowControl = ASPx.CreateClass(WindowControlBase, {
        constructor: function(imageZoom) {
            this.clipPanel = null;

            this.imageWidth = 0;
            this.imageHeight = 0;
            this.largeImageWidth = 0;
            this.largeImageHeight = 0;
            this.offsetX = 0;
            this.offsetY = 0;

            this.zoomWindowWidth = 0;
            this.zoomWindowHeight = 0;

            this.constructor.prototype.constructor.call(this, imageZoom);
        },        
        GetClipPanel: function() {
            if(!this.clipPanel)
                this.clipPanel = ASPx.GetNodeByClassName(this.GetImageZoom().GetMainElement(), CssClassesConstants.ClipPanel);
            return this.clipPanel;
        },
        GetImageClassName: function() {
            return CssClassesConstants.ZoomWindowImage;
        },
        UpdateAppearance: function() {
            var largeImage = this.GetImage();
            if(!ASPx.ImageControlUtils.IsNotEmptyImageSize(largeImage)) //T104051
                ASPx.ImageControlUtils.TryGetImageSize(largeImage, function() { this.UpdateAppearanceCore(); }.aspxBind(this));
            else
                this.UpdateAppearanceCore();
        },
        UpdateAppearanceCore: function() {
            var largeImage = this.GetImage();
            this.largeImageWidth = largeImage.naturalWidth || largeImage.width;
            this.largeImageHeight = largeImage.naturalHeight || largeImage.height;

            var image = this.GetImageZoom().GetImage();
            if(ASPx.Browser.Chrome || ASPx.Browser.Safari) //T135917
                image.offsetParent;
            this.imageWidth = image.width || image.naturalWidth; //T106713
            this.imageHeight = image.height || image.naturalHeight; //T106713
            this.ratio = this.largeImageWidth / this.imageWidth;

            var controlWidth = this.GetImageZoom().width;
            var controlHeight = this.GetImageZoom().height;
            this.offsetX = controlWidth > this.imageWidth ? (controlWidth - this.imageWidth) / 2 : 0;
            this.offsetY = controlHeight > this.imageHeight ? (controlHeight - this.imageHeight) / 2 : 0;

            this.InitializeZoomWindowDimensions();
            ASPx.SetStyles(this.GetClipPanel(), this.GetClipPanelStyle());
        },

        InitializeZoomWindowDimensions: function() {
            this.zoomWindowHeight = this.imageHeight;
            this.zoomWindowWidth = this.imageWidth;
        },
        GetClipPanelStyle: function() {
            var image = this.GetImageZoom().GetImage();
            return {
                width: this.zoomWindowWidth,
                height: this.zoomWindowHeight,
                marginTop: image.style.marginTop,
                marginLeft: image.style.marginLeft
            };
        },

        ShowCore: function() {
            ASPx.AnimationHelper.fadeIn(this.GetClipPanel(), null, Constants.AnimationDuration);
        },
        HideCore: function(preventAnimation) {
            if(preventAnimation)
                ASPx.SetElementOpacity(this.GetClipPanel(), 0);
            else
                ASPx.AnimationHelper.fadeOut(this.GetClipPanel(), null, Constants.AnimationDuration);
        },

        SetText: function() {
        },
        SetPosition: function(x, y) {
            x = x - this.offsetX;
            y = y - this.offsetY;
            var lensWidth = this.zoomWindowWidth / this.ratio;
            var lensHeight = this.zoomWindowHeight / this.ratio;
            var borderX = this.imageWidth - lensWidth;
            var borderY = this.imageHeight - lensHeight;
            x = x - lensWidth / 2;
            y = y - lensHeight / 2;

            if(x < 0)
                x = 0;
            else if(x > borderX)
                x = borderX;

            if(y < 0)
                y = 0;
            else if(y > borderY)
                y = borderY;

            ASPx.SetStyles(this.GetImage(), {
                left: -x * this.ratio,
                top: -y * this.ratio
            });
        }
    });

    var OutsideZoomWindowControl = ASPx.CreateClass(ZoomWindowControl, {
        constructor: function(imageZoom) {
            this.lensControl = null;
            this.constructor.prototype.constructor.call(this, imageZoom);
        },
        Initialize: function() {
            this.lensControl = new LensControl(this);

            var popup = this.GetPopup();
            popup.fadeAnimationDuration = Constants.AnimationDuration;
            popup.AddPopupElement(this.GetImageZoom().GetWrapperElement());

            ZoomWindowControl.prototype.Initialize.call(this);
        },

        UpdateAppearance: function() {
            ZoomWindowControl.prototype.UpdateAppearance.call(this);
            this.GetPopup().SetSize(this.zoomWindowWidth, this.zoomWindowHeight);
            this.lensControl.UpdateAppearance();
        },
        InitializeZoomWindowDimensions: function() {
            this.zoomWindowHeight = typeof (this.GetImageZoom().zoomWindowHeight) == "string" ? this.imageHeight * ASPx.PercentageToFloat(this.GetImageZoom().zoomWindowHeight) : this.GetImageZoom().zoomWindowHeight;
            this.zoomWindowWidth = typeof (this.GetImageZoom().zoomWindowWidth) == "string" ? this.imageWidth * ASPx.PercentageToFloat(this.GetImageZoom().zoomWindowWidth) : this.GetImageZoom().zoomWindowWidth;

            if(this.zoomWindowHeight > this.largeImageHeight)
                this.zoomWindowHeight = this.largeImageHeight;
            if(this.zoomWindowWidth > this.largeImageWidth)
                this.zoomWindowWidth = this.largeImageWidth;
        },
        GetClipPanelStyle: function() {
            return {
                width: this.zoomWindowWidth,
                height: this.zoomWindowHeight,
                marginTop: "",
                marginLeft: "",
                opacity: ""
            };
        },
        GetPopupID: function() {
            return Constants.ZoomWindowID;
        },

        SetPosition: function(x, y) {
            ZoomWindowControl.prototype.SetPosition.call(this, x, y);
            this.lensControl.SetPosition(x, y);
        },

        ShowCore: function() {
            this.lensControl.Show();
            this.GetPopup().Show();
        },
        HideCore: function(preventAnimation) {
            this.lensControl.Hide(preventAnimation);

            var popup = this.GetPopup();
            if(preventAnimation)
                popup.closeAnimationType = "none";
            popup.Hide();
            if(preventAnimation)
                popup.closeAnimationType = "fade";
        },
        SetText: function(text) {
            WindowControlBase.prototype.SetText.call(this, text);
        }
    });

    var LensControl = ASPx.CreateClass(ASPxClientImageControlBase, {
        constructor: function(zoomWindow) {
            this.zoomWindow = zoomWindow
            this.mainElement = null;
            this.panels = null;
            this.topPanel = null;
            this.bottomPanel = null;
            this.leftPanel = null;
            this.rightPanel = null;
            this.centralPanel = null;

            this.lensWidth = 0;
            this.lensHeight = 0;
            this.lensCenterX = 0;
            this.lensCenterY = 0;

            this.constructor.prototype.constructor.call(this, zoomWindow.GetImageZoom());
        },
        GetImageZoom: function() {
            return this.control;
        },
        GetZoomWindow: function() {
            return this.zoomWindow;
        },
        CreateControlHierarchy: function() {
            this.mainElement = this.CreateDiv(CssClassesConstants.Lens + (this.GetImageZoom().mouseBoxOpacityMode == MouseBoxOpacityModeEnum.Outside ? " outside" : ""));
            this.GetImageZoom().GetWrapperElement().appendChild(this.mainElement);

            this.panels = this.CreateDiv(CssClassesConstants.Prefix + "pc");
            this.mainElement.appendChild(this.panels);

            this.topPanel = this.CreateDiv(CssClassesConstants.Prefix + "ltp");
            this.bottomPanel = this.CreateDiv(CssClassesConstants.Prefix + "lbp");
            this.leftPanel = this.CreateDiv(CssClassesConstants.Prefix + "llp");
            this.rightPanel = this.CreateDiv(CssClassesConstants.Prefix + "lrp");
            this.centralPanel = this.CreateDiv(CssClassesConstants.Prefix + "lcp");

            this.panels.appendChild(this.leftPanel);
            this.panels.appendChild(this.rightPanel);
            this.panels.appendChild(this.topPanel);
            this.panels.appendChild(this.bottomPanel);
            this.panels.appendChild(this.centralPanel);
        },
        CreateDiv: function(className) {
            return ASPx.CreateHtmlElement("DIV", { className: className });
        },
        UpdateAppearance: function() {
            var imageWidth = this.GetZoomWindow().imageWidth;
            var imageHeight = this.GetZoomWindow().imageHeight;

            ASPx.SetStyles(this.mainElement, {
                left: this.GetZoomWindow().offsetX, top: this.GetZoomWindow().offsetY,
                width: imageWidth, height: imageHeight
            });

            this.lensWidth = this.GetZoomWindow().zoomWindowWidth / this.GetZoomWindow().ratio;
            this.lensHeight = this.GetZoomWindow().zoomWindowHeight / this.GetZoomWindow().ratio;

            this.lensCenterX = imageWidth - this.lensWidth / 2;
            this.lensCenterY = imageHeight - this.lensHeight / 2;
            ASPx.SetStyles(this.panels, {
                width: imageWidth * 2 - this.lensWidth, height: imageHeight * 2 - this.lensHeight
            });

            this.AdjustPanel(this.topPanel, 0, this.GetZoomWindow().imageWidth - this.lensWidth, this.lensWidth, this.GetZoomWindow().imageHeight - this.lensHeight);
            this.AdjustPanel(this.bottomPanel, this.GetZoomWindow().imageHeight, this.GetZoomWindow().imageWidth - this.lensWidth, this.lensWidth, this.GetZoomWindow().imageHeight - this.lensHeight);
            this.AdjustPanel(this.leftPanel, 0, 0, this.GetZoomWindow().imageWidth - this.lensWidth, this.GetZoomWindow().imageHeight * 2 - this.lensHeight);
            this.AdjustPanel(this.rightPanel, 0, this.GetZoomWindow().imageWidth, this.GetZoomWindow().imageWidth - this.lensWidth, this.GetZoomWindow().imageHeight * 2 - this.lensHeight);
            this.AdjustPanel(this.centralPanel, this.GetZoomWindow().imageHeight - this.lensHeight, this.GetZoomWindow().imageWidth - this.lensWidth, this.lensWidth, this.lensHeight);
        },
        AdjustPanel: function(element, top, left, width, height) {
            ASPx.SetStyles(element, {
                top: top,
                left: left,
                width: width,
                height: height
            });
        },

        Show: function() {
            ASPx.AnimationHelper.fadeIn(this.panels, null, Constants.AnimationDuration);
        },
        Hide: function(preventAnimation) {
            if(preventAnimation)
                ASPx.SetElementOpacity(this.panels, 0);
            else
                ASPx.AnimationHelper.fadeOut(this.panels, null, Constants.AnimationDuration);
        },
        SetPosition: function(x, y) {
            ASPx.SetStyles(this.panels, {
                left: this.GetCorrectedX(x),
                top: this.GetCorrectedY(y)
            });
        },

        GetCorrectedX: function(value) {
            value = value - this.lensCenterX - this.GetZoomWindow().offsetX;
            if(value >= 0)
                value = 0;
            else if(value <= -(this.GetZoomWindow().imageWidth - this.lensWidth))
                value = -(this.GetZoomWindow().imageWidth - this.lensWidth);
            return value;
        },
        GetCorrectedY: function(value) {
            value = value - this.lensCenterY - this.GetZoomWindow().offsetY;
            if(value >= 0)
                value = 0;
            else if(value <= -(this.GetZoomWindow().imageHeight - this.lensHeight))
                value = -(this.GetZoomWindow().imageHeight - this.lensHeight);
            return value;
        }
    });

    var ImageZoomUtils = {
        GetElementRect: function(element) {
            return element.getBoundingClientRect();
        },

        GetPopupRect: function(popup) {
            var currentPopupAnimationType = popup.popupAnimationType;
            var currentCloseAnimationType = popup.closeAnimationType;

            popup.popupAnimationType = "none";
            popup.closeAnimationType = "none";
            popup.Show();

            var result = ImageZoomUtils.GetElementRect(popup.GetWindowElement(-1));

            popup.Hide();
            popup.popupAnimationType = currentPopupAnimationType;
            popup.closeAnimationType = currentCloseAnimationType;

            return result;
        },

        GetIntersectRect: function(rectA, rectB) {
            var left = Math.max(rectA.left, rectB.left);
            var right = Math.min(rectA.right, rectB.right);
            var top = Math.max(rectA.top, rectB.top);
            var bottom = Math.min(rectA.bottom, rectB.bottom);

            if(right >= left && bottom >= top) {
                return {
                    top: top,
                    left: left,
                    right: right,
                    bottom: bottom
                };
            }
            return null;
        }
    }

    window.ASPxClientImageZoom = ASPxClientImageZoom;
})();