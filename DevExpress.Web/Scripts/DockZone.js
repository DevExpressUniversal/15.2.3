/// <reference path="_references.js"/>

(function() {
var DockZoneBag = ASPx.CreateClass(null, {
    //Ctor
    constructor: function() {
        this.zones = {};
    },

    ForEachZone: function(action) {
        for(var key in this.zones) {
            if(!this.zones.hasOwnProperty(key))
                continue;
            action(this.zones[key]);
        }
    },

    RegisterZone: function(zone) {
        this.zones[zone.zoneUID] = zone;
    },

    GetZoneByUID: function(zoneUID) {
        return this.zones[zoneUID];
    },

    GetZoneList: function() {
        var zoneList = [];

        this.ForEachZone(function(zone) {
            if(zone.GetMainElement())
                zoneList.push(zone);
        });

        return zoneList;
    }

});

DockZoneBag.instance = null;

DockZoneBag.Get = function() {
    if(!DockZoneBag.instance)
        DockZoneBag.instance = new DockZoneBag();
    return DockZoneBag.instance;
};
var ASPxClientDockZone = ASPx.CreateClass(ASPxClientControl, {
    //Consts
    HorizontalOrientationCssClassName: 'dxdzControlHor',
    FillOrientationCssClassName: 'dxdzControlFill',
    PanelPlaceHolderCssClassName: 'dxdz-pnlPlcHolder',
    StyleSheetIDPostfix: '_SS',
    DefaultHorizontalOrientationWidth: 400,
    DefaultHorizontalOrientationHeight: 200,
    DefaultVerticalOrientationWidth: 200,
    DefaultVerticalOrientationHeight: 400,
    DefaultFillOrientationWidth: 400,
    DefaultFillOrientationHeight: 400,
    BeforeDockServerEventName: "BeforeDock",
    AfterDockServerEventName: "AfterDock",
    RaiseBeforeDockEventCommand: "EBD",
    RaiseAfterDockEventCommand: "EAD",

    //Ctor
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);

        //Server-provided fields
        this.zoneUID = null;
        this.allowGrow = true;
        this.panelSpacing = 0;


        if(this.IsFillOrientation()) {
            this.initialWidth = this.DefaultFillOrientationWidth;
            this.initialHeight = this.DefaultFillOrientationHeight;
        } else {
            var isHorizontal = this.IsHorizontalOrientation();
            this.initialWidth = isHorizontal ? this.DefaultHorizontalOrientationWidth : this.DefaultVerticalOrientationWidth;
            this.initialHeight = isHorizontal ? this.DefaultHorizontalOrientationHeight : this.DefaultVerticalOrientationHeight;
        }

        this.inPostback = false;

        this.initialStyleDimensions = {
            width: '',
            height: ''
        };

        //Fields
        this.dockedPanels = {};

        //Styles
        this.zoneStyleSheet = ASPx.GetCurrentStyleSheet();
        this.dockingAllowedClassName = '';
        this.dockingForbiddenClassName = '';

        //Events
        this.BeforeDock = new ASPxClientEvent();
        this.AfterDock = new ASPxClientEvent();
    },

    //NOTE: this helps prevent duplicate form submission is some cases (see B189327)
    SendPostBack: function(params) {
        if(!this.inPostback) {
            this.inPostback = true;
            ASPxClientControl.prototype.SendPostBack.call(this, params);
        }
    },

    //Initialization
    InlineInitialize: function() {
        ASPxClientControl.prototype.InlineInitialize.call(this);

        var panelPlaceholder = this.GetPanelPlaceholder();
        ASPx.SetElementDisplay(panelPlaceholder, false);

        var mainElement = this.GetMainElement();
        this.initialStyleDimensions.width = mainElement.style.width;
        this.initialStyleDimensions.height = mainElement.style.height;

        this.InitializeBoxSizing(); //T175194
        DockZoneBag.Get().RegisterZone(this);
    },
    InitializeBoxSizing: function() {
        if(this.IsPercentageWidth()) 
            this.GetMainElement().style.boxSizing = "border-box";
    },

    //Adjust control
    AdjustControlCore: function() {
        var mainElement = this.GetMainElement();
        mainElement.style.overflow = 'hidden';

        if(this.IsHorizontalOrientation()) {
            if(this.IsPercentageWidth())
                this.CorrectWidthOnAdjust();
            this.correctPanelsDimensionsOnAdjust(this.IsPercentageHeigth(), false);
        } else if(!this.IsFillOrientation()) {
            if(this.IsPercentageHeigth())
                this.CorrectHeightOnAdjust();
            this.correctPanelsDimensionsOnAdjust(false, this.IsPercentageWidth());
        } else {
            this.correctPanelsDimensionsOnAdjust(this.IsPercentageHeigth(), this.IsPercentageWidth());
            if(this.IsPercentageWidth())
                this.CorrectWidthOnAdjust();
            if(this.IsPercentageHeigth())
                this.CorrectHeightOnAdjust();
        }

        mainElement.style.overflow = '';
    },
    correctPanelsDimensionsOnAdjust: function(height, width) {
        if(height) this.correctPanelsOnAdjust(true);
        if(width) this.correctPanelsOnAdjust(false);
    },
    correctPanelsOnAdjust: function(isHeight) {
        var mainElement = this.GetMainElement();
        if(isHeight)
            this.initialHeight = mainElement.offsetHeight - ASPx.GetTopBottomBordersAndPaddingsSummaryValue(mainElement);
        else
            this.initialWidth = mainElement.offsetWidth - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(mainElement);
        this.ForEachDockedPanel(function(panel) {
            panel.UpdateRestoredWindowSizeLock();
            isHeight ? panel.SetHeightInternal(this.initialHeight) : panel.SetWidthInternal(this.initialWidth);;
            panel.UpdateRestoredWindowSizeUnlock();
        }.aspxBind(this));
    },

    CorrectWidthOnAdjust: function() {
        var mainElement = this.GetMainElement();

        this.initialWidth = 0;
        this.CorrectWidth();
        var contentWidth = ASPx.GetClearClientWidth(mainElement);

        mainElement.style.width = this.initialStyleDimensions.width;
        var percentageWidth = ASPx.GetClearClientWidth(mainElement);

        if(contentWidth > percentageWidth)
            mainElement.style.width = contentWidth + 'px';
        else
            this.initialWidth = percentageWidth;
    },

    CorrectHeightOnAdjust: function() {
        var mainElement = this.GetMainElement();

        this.initialHeight = 0;
        this.CorrectHeight();
        var contentHeight = ASPx.GetClearClientHeight(mainElement);

        mainElement.style.height = this.initialStyleDimensions.height;
        var percentageHeight = ASPx.GetClearClientHeight(mainElement);

        if(contentHeight > percentageHeight)
            mainElement.style.height = contentHeight + 'px';
        else
            this.initialHeight = percentageHeight;
    },

    IsPercentageWidth: function() {
        return ASPx.IsPercentageSize(this.initialStyleDimensions.width);
    },

    IsPercentageHeigth: function() {
        return ASPx.IsPercentageSize(this.initialStyleDimensions.height);
    },

    BrowserWindowResizeSubscriber: function() {
        return true;
    },

    OnBrowserWindowResize: function() {
        this.AdjustControl();
    },

    //Metrics
    IsCursorInsideZone: function(cursorPos) {
        var mainElement = this.GetMainElement();
        var width = mainElement.offsetWidth;
        var height = mainElement.offsetHeight;
        var x = ASPx.GetAbsoluteX(mainElement);
        var y = ASPx.GetAbsoluteY(mainElement);
        var bordersAndPaddingsValues = this.GetBordersAndPaddingsValues();
        var bounds = {
            left: x + bordersAndPaddingsValues.left,
            top: y + bordersAndPaddingsValues.top,
            right: x + width - bordersAndPaddingsValues.right,
            bottom: y + height - bordersAndPaddingsValues.bottom
        };
        return cursorPos.x >= bounds.left && cursorPos.x <= bounds.right &&
            cursorPos.y >= bounds.top && cursorPos.y <= bounds.bottom;
    },

    GetBordersAndPaddingsValues: function() {
        var mainElement = this.GetMainElement();
        var currentStyle = ASPx.GetCurrentStyle(mainElement);
        var leftValue = ASPx.PxToInt(currentStyle.paddingLeft);
        if(currentStyle.borderLeftStyle != "none")
            leftValue += ASPx.PxToInt(currentStyle.borderLeftWidth);
        var topValue = ASPx.PxToInt(currentStyle.paddingTop);
        if(currentStyle.borderTopStyle != "none")
            topValue += ASPx.PxToInt(currentStyle.borderTopWidth);
        var rightValue = ASPx.PxToInt(currentStyle.paddingRight);
        if(currentStyle.borderRightStyle != "none")
            rightValue += ASPx.PxToInt(currentStyle.borderRightWidth);
        var bottomValue = ASPx.PxToInt(currentStyle.paddingBottom);
        if(currentStyle.borderBottomStyle != "none")
            bottomValue += ASPx.PxToInt(currentStyle.borderBottomWidth);
        return { left: leftValue, top: topValue, right: rightValue, bottom: bottomValue };
    },

    IsHorizontalOrientation: function() {
        var mainElement = this.GetMainElement();
        return ASPx.ElementHasCssClass(mainElement, this.HorizontalOrientationCssClassName);
    },

    IsFillOrientation: function() {
        var mainElement = this.GetMainElement();
        return ASPx.ElementHasCssClass(mainElement, this.FillOrientationCssClassName);
    },

    GetDockedPanelsSummaryHeight: function() {
        var dockedPanelsSummaryHeight = 0;

        this.ForEachDockedPanel(function(panel) {
            if(!panel.IsVisible())
                return;

            var panelMainElement = panel.GetMainElement();
            dockedPanelsSummaryHeight += panel.GetHeight() + ASPx.PxToInt(panelMainElement.style.marginTop);
        });

        return dockedPanelsSummaryHeight;
    },

    GetDockedPanelsSummaryWidth: function() {
        var dockedPanelsSummaryWidth = 0;

        this.ForEachDockedPanel(function(panel) {
            if(!panel.IsVisible())
                return;

            var panelMainElement = panel.GetMainElement();
            dockedPanelsSummaryWidth += panel.GetWidth() + ASPx.PxToInt(panelMainElement.style.marginLeft);
        });

        return dockedPanelsSummaryWidth;
    },

    CorrectHeight: function() {
        var height = this.GetDockedPanelsSummaryHeight();
        var panelPlaceholder = this.GetPanelPlaceholder();
        var mainElement = this.GetMainElement();

        if(ASPx.GetElementDisplay(panelPlaceholder))
            height += panelPlaceholder.offsetHeight + ASPx.PxToInt(panelPlaceholder.style.marginTop);

        height = Math.max(height, this.initialHeight);
        mainElement.style.height = height + 'px';
    },

    CorrectWidth: function() {
        var width = this.GetDockedPanelsSummaryWidth();
        var panelPlaceholder = this.GetPanelPlaceholder();
        var mainElement = this.GetMainElement();

        if(ASPx.GetElementDisplay(panelPlaceholder))
            width += panelPlaceholder.offsetWidth + ASPx.PxToInt(panelPlaceholder.style.marginLeft);

        width = Math.max(width, this.initialWidth);
        mainElement.style.width = width + 'px';
    },

    CorrectResizableDimension: function() {
        if(!this.allowGrow)
            return;

        this.CorrectResizableDimensionCore();
    },

    CorrectResizableDimensionCore: function() {
        if(this.IsHorizontalOrientation())
            this.CorrectWidth();
        else
            this.CorrectHeight();
    },

    //Panel placeholder
    GetPanelPlaceholder: function() {
        return ASPx.GetChildByClassName(this.GetMainElement(), this.PanelPlaceHolderCssClassName);
    },

    GetPanelPlaceholderPositionForElement: function(element) {
        var placeholder = this.GetPanelPlaceholder();
        if(placeholder.style.display === "none" && this.IsFillOrientation())
            placeholder.style.display = "";
        return {
            x: ASPx.PrepareClientPosForElement(ASPx.GetAbsoluteX(placeholder), element, true),
            y: ASPx.PrepareClientPosForElement(ASPx.GetAbsoluteY(placeholder), element, false)
        };
    },

    MovePanelPlaceholder: function(cursorPos) {
        var cursorOverPanelLocation = null;
        var mainElement = this.GetMainElement();
        var panelPlaceholder = this.GetPanelPlaceholder();
        var isHorizontal = this.IsHorizontalOrientation();

        for(var key in this.dockedPanels) {
            if(!this.dockedPanels.hasOwnProperty(key))
                continue;
            var panel = this.dockedPanels[key];
            cursorOverPanelLocation = panel.GetCursorOverPanelLocation(cursorPos, this.panelSpacing, isHorizontal);

            if(cursorOverPanelLocation === 'top' || cursorOverPanelLocation === 'left') {
                mainElement.insertBefore(panelPlaceholder, panel.GetMainElement());
                this.ApplyPanelSpacing();
                return;
            }

            if(cursorOverPanelLocation === 'bottom' || cursorOverPanelLocation === 'right') {
                var panelElementSibling = panel.GetMainElement().nextSibling;
                mainElement.insertBefore(panelPlaceholder, panelElementSibling);
                this.ApplyPanelSpacing();
                return;
            }
        }

        //NOTE: ifplaceholder is shown and it's doesn't affect any panel it's already positioned
        //in the correct place and don't need to be moved
        if(panelPlaceholder.style.display === 'none') {
            mainElement.appendChild(panelPlaceholder);
            this.ApplyPanelSpacing();
        }
    },

    MovePanelPlaceholderToPanel: function(panel) {
        var panelPlaceholder = this.GetPanelPlaceholder();
        var mainElement = this.GetMainElement();
        var panelMainElement = panel.GetMainElement();
        //B184906
        mainElement.insertBefore(panelPlaceholder, panelMainElement.nextSibling);
    },

    ShowPanelPlaceholder: function(panel) {
        var panelPlaceholder = this.GetPanelPlaceholder();
        var isHorizontal = this.IsHorizontalOrientation();
        var isFill = this.IsFillOrientation();
        var panelDockedDimensions = null;
        var canDockPanel = panel.mode != ASPxClientDockPanelModes.FloatOnly && !panel.freezed && !panel.IsZoneForbidden(this);

        if(isFill) {
            panelDockedDimensions = { width: this.initialWidth, height: this.initialHeight };
            canDockPanel &= this.CanDockPanel();
        } else {
            var zoneResizableDimension = isHorizontal ? this.initialHeight : this.initialWidth;
            panelDockedDimensions = panel.GetDockedDimensions(zoneResizableDimension, isHorizontal);
            canDockPanel &= this.CanDockPanel(isHorizontal ? panelDockedDimensions.width : panelDockedDimensions.height);
        }

        if(canDockPanel) {
            var panelPlaceholderWidth = panelDockedDimensions.width - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(panelPlaceholder);
            var panelPlaceholderHeight = panelDockedDimensions.height - ASPx.GetTopBottomBordersAndPaddingsSummaryValue(panelPlaceholder);

            ASPx.SetStyles(panelPlaceholder, {
                width: panelPlaceholderWidth,
                height: panelPlaceholderHeight,
                display: ""
            });

            if(!isFill)
                this.ApplyPanelSpacing();

            this.CorrectResizableDimension();
            this.AdjustControlCore();
        }
    },

    HidePanelPlaceholder: function() {
        var panelPlaceholder = this.GetPanelPlaceholder();
        ASPx.SetElementDisplay(panelPlaceholder, false);
        this.ApplyPanelSpacing();
        this.CorrectResizableDimension();
        this.AdjustControlCore();
    },

    //Panels
    ForEachDockedPanel: function(action) {
        for(var key in this.dockedPanels) {
            if(!this.dockedPanels.hasOwnProperty(key))
                continue;
            action(this.dockedPanels[key]);
        }
    },

    DockPanel: function(panel, dockedDimensions, considerVisibleIndex) {
        var isHorizontal = this.IsHorizontalOrientation();

        panel.ResizeForDock(dockedDimensions);
        this.HidePanelPlaceholder();
        this.ConsumePanelElement(panel, considerVisibleIndex);
        this.ApplyPanelSpacing();

        this.dockedPanels[panel.panelUID] = panel;
        panel.SetZoneUID(this.zoneUID);
        if(!considerVisibleIndex)
            this.UpdatePanelsVisibleIndices();
        this.CorrectResizableDimension();
    },

    ConsumePanelElement: function(panel, considerVisibleIndex) {
        var mainElement = this.GetMainElement();
        var panelMainElement = panel.GetMainElement()

        if(!considerVisibleIndex) {
            var panelPlaceholder = this.GetPanelPlaceholder();
            mainElement.insertBefore(panelMainElement, panelPlaceholder);
            return;
        }

        //NOTE: Here is the deal: we are searching forpanel
        //with the minimal visible index which is more than
        //the given then insert panel before it 
        var nextPanel = null;
        var visibleIndex = panel.GetVisibleIndex();

        this.ForEachDockedPanel(function(dockedPanel) {
            var dockedPanelVisibleIndex = dockedPanel.GetVisibleIndex();
            if(!dockedPanel.IsVisible() && dockedPanelVisibleIndex === visibleIndex)
                return;
            if(dockedPanelVisibleIndex > visibleIndex) {
                if(nextPanel && nextPanel.GetVisibleIndex() <= dockedPanelVisibleIndex)
                    return;
                nextPanel = dockedPanel;
            }
        });

        var insertBeforeNode = null;
        if(nextPanel)
            insertBeforeNode = nextPanel.GetMainElement();
        mainElement.insertBefore(panelMainElement, insertBeforeNode);
    },

    CanDockPanel: function(resizableDimension) {
        if(this.IsFillOrientation())
            return !this.HasDockedPanels();

        if(this.allowGrow)
            return true;
        var panelPlaceholder = this.GetPanelPlaceholder();
        var spacing = 0;
        if(panelPlaceholder.prevSibling && panelPlaceholder.prevSibling.nodeType === 1)
            spacing = this.panelSpacing;
        if(this.IsHorizontalOrientation())
            return this.GetDockedPanelsSummaryWidth() + resizableDimension + spacing <= this.initialWidth;
        return this.GetDockedPanelsSummaryHeight() + resizableDimension + spacing <= this.initialHeight;
    },

    UpdatePanelsVisibleIndices: function() {
        var zoneChildElements = this.GetMainElement().childNodes;
        var index = 0;
        for(var i = 0; i < zoneChildElements.length; i++) {
            if(zoneChildElements[i].panelUID) {
                var panel = this.dockedPanels[zoneChildElements[i].panelUID];

                if(!panel.IsVisible())
                    continue;

                panel.SetVisibleIndexCore(index);
                index++;
            }
        }
    },

    GetDockedPanelsMaxVisibleIndex: function() {
        var maxIndex = 0;
        this.ForEachDockedPanel(function(panel) {
            maxIndex = Math.max(panel.GetVisibleIndex(), maxIndex);
        });
        return maxIndex;
    },

    GetPanelAfterPlaceholderVisibleIndex: function() {
        var placeholder = this.GetPanelPlaceholder();
        var sibling = placeholder.previousSibling;

        while(sibling) {
            if(sibling.panelUID) {
                var panel = this.dockedPanels[sibling.panelUID];
                if(panel.IsVisible())
                    return panel.GetVisibleIndex();
            }
            sibling = sibling.previousSibling;
        }

        return -1;
    },

    GetOrderedPanelsList: function(startFromPanel) {
        var panels = [];
        var mainElement = this.GetMainElement();
        var element = startFromPanel ? startFromPanel.GetMainElement() : mainElement.firstChild;

        while(element) {
            if(element.panelUID) {
                var panel = this.dockedPanels[element.panelUID];
                if(panel)
                    panels.push(panel);
            }
            element = element.nextSibling;
        }

        return panels;
    },

    FixatePanels: function(startFromPanel) {
        var mainElement = this.GetMainElement();
        mainElement.style.position = 'relative';
        mainElement.style.top = 0;
        mainElement.style.left = 0;

        var panels = this.GetOrderedPanelsList(startFromPanel);
        for(var i = panels.length - 1; i >= 0; i--)
            panels[i].Fixate();
    },

    RemovePanelsFixation: function() {
        var panels = this.GetOrderedPanelsList();
        for(var i = 0; i < panels.length; i++)
            panels[i].RemoveFixation();

        var mainElement = this.GetMainElement();
        mainElement.style.position = 'static';
    },

    UndockPanel: function(panel) {
        delete this.dockedPanels[panel.panelUID];
        panel.SetZoneUID(null);
        this.RemovePanelSpacing(panel.GetMainElement());
        this.UpdatePanelsVisibleIndices();
        this.ApplyPanelSpacing();
    },

    HasDockedPanels: function() {
        for(var key in this.dockedPanels) {
            if(this.dockedPanels.hasOwnProperty(key))
                return true;
        }

        return false;
    },

    //Spacings
    ApplyPanelSpacing: function() {
        if(!this.panelSpacing)
            return;

        var mainElement = this.GetMainElement();
        var instance = this;
        var elements = ASPx.GetChildElementNodesByPredicate(mainElement, function(element) {
            var isPlaceholder = ASPx.ElementHasCssClass(element, instance.PanelPlaceHolderCssClassName);
            return (element.panelUID || isPlaceholder) && ASPx.GetElementDisplay(element);
        });

        for(var i = 0; i < elements.length; i++) {
            var element = elements[i];
            this.RemovePanelSpacing(element);
            if(i > 0) {
                if(this.IsHorizontalOrientation())
                    element.style.marginLeft = this.panelSpacing + 'px';
                else
                    element.style.marginTop = this.panelSpacing + 'px';
            }
        }
    },

    RemovePanelSpacing: function(panelMainElement) {
        panelMainElement.style.marginTop = '';
        panelMainElement.style.marginLeft = '';
    },

    //Styles
    CreateClientCssStyles: function(stylesObj) {
        this.dockingForbiddenClassName = this.CreateClientCssStyle(stylesObj.dfs);
        this.dockingAllowedClassName = this.CreateClientCssStyle(stylesObj.das);
    },

    CreateClientCssStyle: function(style) {
        if(!style)
            return '';
        var result = style.className;
        if(style.inlineStyle)
            result += " " + ASPx.CreateImportantStyleRule(this.zoneStyleSheet, style.inlineStyle);
        return ASPx.Str.Trim(result);
    },

    ApplyDockingAllowedStyle: function() {
        this.ApplyZoneCssClass(this.dockingAllowedClassName);
    },

    RemoveDockingAllowedStyle: function() {
        this.RemoveZoneCssClass(this.dockingAllowedClassName);
    },

    ApplyDockingForbiddenStyle: function() {
        this.ApplyZoneCssClass(this.dockingForbiddenClassName);
    },

    RemoveDockingForbiddenStyle: function() {
        this.RemoveZoneCssClass(this.dockingForbiddenClassName);
    },

    ApplyZoneCssClass: function(cssClassName) {
        var mainElement = this.GetMainElement();
        var tempClassName = mainElement.className.replace(cssClassName, "");
        mainElement.className = ASPx.Str.Trim(tempClassName + " " + cssClassName);
    },

    RemoveZoneCssClass: function(cssClassName) {
        var mainElement = this.GetMainElement();
        mainElement.className = mainElement.className.replace(cssClassName, "");;
    },

    //Events
    GetBeforeDockPostbackArgs: function(panel) {
        return [
            this.RaiseBeforeDockEventCommand,
            panel.panelUID,
            this.GetPanelAfterPlaceholderVisibleIndex() + 1
        ];
    },

    GetAfterDockPostbackArgs: function(panel) {
        return [
            this.RaiseAfterDockEventCommand,
            panel.panelUID
        ];
    },

    RaiseBeforeDock: function(panel) {
        var processOnServer = this.IsServerEventAssigned(this.BeforeDockServerEventName);
        var args = new ASPxClientDockZoneCancelEventArgs(processOnServer, panel);
        if(!this.BeforeDock.IsEmpty())
            this.BeforeDock.FireEvent(this, args);

        if(!args.cancel && args.processOnServer && this.isInitialized) {
            var postbackArgs = this.GetBeforeDockPostbackArgs(panel);
            this.SendPostBack(ASPx.Json.ToJson(postbackArgs));
            return;
        }

        return !args.cancel;
    },

    RaiseAfterDock: function(panel) {
        var processOnServer = this.IsServerEventAssigned(this.AfterDockServerEventName);
        var args = new ASPxClientDockZoneProcessingModeEventArgs(processOnServer, panel);
        if(!this.AfterDock.IsEmpty())
            this.AfterDock.FireEvent(this, args);
        if(args.processOnServer && this.isInitialized) {
            var postbackArgs = this.GetAfterDockPostbackArgs(panel);
            this.SendPostBack(ASPx.Json.ToJson(postbackArgs));
            return;
        }
    },

    //API

    SetWidth: function(width) {
        var mainElement = this.GetMainElement();
        var actualWidth = width - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(mainElement);

        this.initialWidth = actualWidth;
        mainElement.style.width = actualWidth + "px";

        if(!this.IsHorizontalOrientation())
            this.ForEachDockedPanel(function(panel) { panel.SetWidthInternal(actualWidth) });
    },

    SetHeight: function(height) {
        var mainElement = this.GetMainElement();
        var actualHeight = height - ASPx.GetTopBottomBordersAndPaddingsSummaryValue(mainElement);

        this.initialHeight = actualHeight;
        mainElement.style.height = actualHeight + "px";

        if(this.IsHorizontalOrientation())
            this.ForEachDockedPanel(function(panel) { panel.SetHeightInternal(actualHeight) });
    },
    IsVertical: function() {
        return !this.IsHorizontalOrientation();
    },
    GetAllowGrowing: function() {
        return this.allowGrow;
    },
    GetPanelCount: function() {
        return this.GetOrderedPanelsList().length;
    },
    GetPanelByUID: function(panelUID) {
        var panels = this.GetOrderedPanelsList();

        for(var i = 0; i < panels.length; i++) {
            if(panels[i].panelUID === panelUID)
                return panels[i];
        }

        return null;
    },
    GetPanelByVisibleIndex: function(visibleIndex) {
        this.UpdatePanelsVisibleIndices();
        var panels = this.GetOrderedPanelsList();
        return panels[visibleIndex];
    },
    GetPanels: function(filterPredicate) {
        var panels = this.GetOrderedPanelsList();
        return ASPx.RetrieveByPredicate(panels, filterPredicate);
    }
});
ASPxClientDockZone.Cast = ASPxClientControl.Cast;
var ASPxClientDockZoneCancelEventArgs = ASPx.CreateClass(ASPxClientProcessingModeCancelEventArgs, {
    constructor: function(processOnServer, panel) {
        this.constructor.prototype.constructor.call(this, processOnServer);
        this.panel = panel;
    }
});
var ASPxClientDockZoneProcessingModeEventArgs = ASPx.CreateClass(ASPxClientProcessingModeEventArgs, {
    constructor: function(processOnServer, panel) {
        this.constructor.prototype.constructor.call(this, processOnServer);
        this.panel = panel;
    }
});

ASPx.DockZoneBag = DockZoneBag;

window.ASPxClientDockZone = ASPxClientDockZone;
window.ASPxClientDockZoneCancelEventArgs = ASPxClientDockZoneCancelEventArgs;
window.ASPxClientDockZoneProcessingModeEventArgs = ASPxClientDockZoneProcessingModeEventArgs;
})();