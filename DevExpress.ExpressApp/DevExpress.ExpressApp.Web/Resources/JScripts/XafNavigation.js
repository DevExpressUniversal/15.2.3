function XafNavigation(navEl, toggleNavigationEl, toggleNavigation_mEl, showOnStart) {
    var self = this;
    var states = {
        hidden: "hidden",
        inplace: "inplace",
        onTop: "onTop",
        onTopAfterShow: "onTopAfterShow"
    }
    var xafNavTogleActiveDiv = $(document.getElementById("xafNavTogleActive"));
    var xafNavTogleDiv = $(document.getElementById("xafNavTogle"));
    var xafNavTogleActiveDiv_m = $(document.getElementById("xafNavTogleActive_m"));
    var xafNavTogleDiv_m = $(document.getElementById("xafNavTogle_m"));

    var navElement = $(navEl);
    var navContentElement = $(navElement.children()[0]);
    var toggleNavigation = $(toggleNavigationEl);
    var toggleNavigation_m = $(toggleNavigation_mEl);
    var coverElement = $("<div class=\"xafCover xafFrontLayer\"></div>");
    var maxInplaceWidth = 1000;
    var currentState;
    var showNavigationPosition;
    var stateIsManuallySet = false;
    this.SetShowNavigationPosition = function (value) {
        this.showNavigationPosition = value;
    }
    //TODO Crimp, remove this function
    this.GetShowNavigationPosition = function () {
        return this.showNavigationPosition;
    }
    function CanBeInplace() {
        return $(window).width() > maxInplaceWidth;
    }
    function HideCore() {
        if ((maxInplaceWidth > GetClientWidth() && navElement.hasClass("xafNavHidden")) && !navElement.hasClass("xafNavVisibleManually")) {
            return;
        }
        menuRefreshedInNavController = true;
        var width = navContentElement.width();
        DevExpress.fx.animate(navElement, {
            type: 'slide',
            from: { left: 0 },
            to: { left: -width * 2 },
            complete: function () {
                SetStylesForHiddenState();
            }
        });
    }
    function SetStylesForHiddenState() {
        navElement.removeClass("xafNavVisibleManually");
        navElement.addClass("xafHidden");
        RestoreBodyScroll();
        coverElement.addClass("xafHidden");
        $("html").removeClass("xafBackLayer");
        SetNavTogleState(false);
        SetEmptyHeaderDivVisibility(false);
        RefreshMainMenu();
    }
    function MakeInplace() {
        if (maxInplaceWidth < GetClientWidth() && !coverElement.hasClass("xafHidden")) {
            return;
        }
        if (!self.IsVisible()) {
            menuRefreshedInNavController = true;
            navElement.addClass("xafNavVisibleManually");
            navElement.removeClass("xafHidden");
            coverElement.addClass("xafHidden");
            RestoreBodyScroll();
            navElement.removeClass("xafFrontLayer");
            navContentElement.css("margin-top", '');
            navContentElement.css("margin-bottom", '');
            SetNavTogleState(true);
            RefreshMainMenu();
            var width = navContentElement.width();
            DevExpress.fx.animate(navElement, {
                type: 'slide',
                from: { left: -width * 2 },
                to: { left: 0 }
            });
        }
    }
    function MakeOnTop() {
        if (!self.IsVisible()) {
            navElement.addClass("xafNavVisibleManually");
            navElement.removeClass("xafHidden");
            HideBodyScroll();
            coverElement[0].style.top = self.showNavigationPosition + "px";

            var width = navContentElement.width();
            navContentElement.css("margin-top", '0px');
            navContentElement.css("margin-bottom", '0px');
            coverElement.removeClass("xafHidden");

            navElement.addClass("xafFrontLayer");
            navElement.scrollTop(0);
            navElement[0].style.top = self.showNavigationPosition + "px";

            DevExpress.fx.animate(navElement, {
                type: 'slide',
                from: { left: -width },
                to: { left: 0 },
                complete: function () {
                    NavScrollSetVisibleState(states.onTopAfterShow);
                }
            })
            SetNavTogleState(true);
            SetEmptyHeaderDivVisibility(true);
        }
    }
    function SetNavTogleState(navVisible) {
        if (navVisible) {
            xafNavTogleActiveDiv_m.removeClass("xafHidden");
            xafNavTogleActiveDiv_m.removeClass("xafNavHidden");
            xafNavTogleDiv_m.addClass("xafHidden");
            xafNavTogleDiv_m.removeClass("xafNavVisible");

            xafNavTogleActiveDiv.removeClass("xafHidden");
            xafNavTogleActiveDiv.removeClass("xafNavHidden");
            xafNavTogleDiv.addClass("xafHidden");
            xafNavTogleDiv.removeClass("xafNavVisible");
        } else {
            xafNavTogleActiveDiv_m.addClass("xafHidden");
            xafNavTogleActiveDiv_m.addClass("xafNavHidden");
            xafNavTogleDiv_m.removeClass("xafHidden");
            xafNavTogleDiv_m.removeClass("xafNavVisible");

            xafNavTogleActiveDiv.addClass("xafHidden");
            xafNavTogleActiveDiv.addClass("xafNavHidden");
            xafNavTogleDiv.removeClass("xafHidden");
            xafNavTogleDiv.removeClass("xafNavVisible");
        }
    }
    function SetState(value) {
        NavScrollSetVisibleState(value);
        if (self.currentState === value) {
            return;
        }
        if (value === states.hidden) {
            HideCore();
        }
        else {
            if (value === states.inplace) {
                MakeInplace();
            }
            else {
                if (value === states.onTop) {
                    MakeOnTop();
                }
                else {
                    throw "Invalid state";
                }
            }
        }
        self.currentState = value;
    }
    function HideBodyScroll() {
        if (ASPx.Browser.WebKitTouchUI)
            return;
        var verticalScrollMustBeReplacedByMargin = IsVerticalScrollExists();
        if (verticalScrollMustBeReplacedByMargin) {
            ASPx.Attr.ChangeStyleAttribute(document.body, "margin-right", ASPx.GetVerticalScrollBarWidth() + "px");
        }
        $("html").addClass("xafBackLayer");
    }
    function RestoreBodyScroll() {
        if (ASPx.Browser.WebKitTouchUI)
            return;
        $("html").removeClass("xafBackLayer");
        ASPx.Attr.RestoreStyleAttribute(document.body, "margin-right");
    }
    function IsVerticalScrollExists() {
        var scrollIsNotHidden = ASPx.GetCurrentStyle(document.body).overflowY !== "hidden" && ASPx.GetCurrentStyle(document.documentElement).overflowY !== "hidden";
        return (scrollIsNotHidden && ASPx.GetDocumentHeight() > ASPx.GetDocumentClientHeight());
    }
    function NavScrollSetVisibleState(value) {
        if (value === states.inplace) {
            navContentElement.css("overflow", 'visible');
            navContentElement.css("height", '');
        }
        if (value === states.onTopAfterShow) {
            navContentElement.css("overflow", 'visible');
        }
        if (value === states.onTop || (value === states.hidden && self.currentState === states.onTop)) {
            var contentHeight = GetClientHeight() - self.showNavigationPosition;
            navContentElement.css("height", contentHeight + 'px');
            navContentElement.css("overflow", 'hidden');
        }
    }
    function SetEmptyHeaderDivVisibility(visible) {
        if (!ASPx.Browser.WebKitTouchUI) {
            var emptyHeaderTableDiv = document.getElementById("TestheaderTableDiv");
            var headerTableDiv = document.getElementById("headerTableDiv");
            if (visible) {
                emptyHeaderTableDiv.style.display = '';
                emptyHeaderTableDiv.style.height = headerTableDiv.clientHeight + 'px';
                emptyHeaderTableDiv.style.width = document.body.style.marginRight;
            }
            else {
                emptyHeaderTableDiv.style.display = 'none';
                emptyHeaderTableDiv.style.height = '';
                emptyHeaderTableDiv.style.width = '';
            }
        }
    }
    this.NavigationWidth = function () {
        return navContentElement.width();
    }
    this.RefreshState = function () {
        if (self.currentState == states.hidden && self.stateIsManuallySet) {
            return;
        }
        var calculatedState;
        if (CanBeInplace()) {
            calculatedState = states.inplace;
        }
        else if (self.IsOnTop()) {
            calculatedState = states.onTop;
        }
        else {
            calculatedState = states.hidden;
        }
        SetState(calculatedState);
    }

    this.SetMaxInplaceWidth = function (value) {
        maxInplaceWidth = value;
    }
    this.IsVisible = function () {
        return self.currentState !== states.hidden && navElement.css("display") != 'none';
    }
    this.IsOnTop = function () {
        return self.currentState === states.onTop;
    }
    this.Toggle = function () {
        if (self.IsVisible()) {
            self.stateIsManuallySet = true;
            self.Hide(true);
        }
        else {
            self.stateIsManuallySet = false;
            self.Show(true);
        }
    }
    this.Show = function (manuallySet) {
        if (CanBeInplace()) {
            SetState(states.inplace);
        }
        else {
            SetState(states.onTop);
        }
    }
    this.Hide = function () {
        SetState(states.hidden);
    }

    navElement.before(coverElement);
    navElement.click(function (e) {
        if (navElement.is(e.target) && self.currentState === states.onTop) {
            self.Hide(); // сюда зайдет только когда ткнул мимо навигации ( она занимает всю страницу если показывается поверх контента)
        }
    });
    toggleNavigation.click(this.Toggle);
    toggleNavigation_m.click(this.Toggle);
    
    if (!showOnStart) {
        SetStylesForHiddenState();
        self.currentState = states.hidden;
        self.stateIsManuallySet = true;
    }
    else {
        this.RefreshState();
    }
}