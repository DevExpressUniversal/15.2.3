window.pageLoaded = false;
window.animateComplete = true;
window.NewStyle = true;
window.navigationState = { positionToShow: -1, Visible: true };
window.menuState = { moved: false, Locked: 0 };
window.menuNewHeight = 0;
window.lockUpdate = true;
window.countVisiblePopup = 0;

var body = document.body,
    scrollTimer,
    hover_disabled = false;

function WindowScroll() {
    RefreshUI();
}
window.addEventListener('scroll', WindowScroll, false);

function ShouldMoveMenu(windowscrollTop, headerDivclientHeight) {
    return windowscrollTop > headerDivclientHeight;
}

function RefreshUI() {
    ForeseRefreshUI(false);
}
function ForeseRefreshUI(forseUpdateMenuPosition) {
    if (!window.lockUpdate && window.pageLoaded) {
        var menuAreaDiv = document.getElementById("menuAreaDiv");
        var viewSite = document.getElementById("viewSite");

        var toggleNavigation_m = document.getElementById("toggleNavigation_m");
        var mainDiv = document.getElementById("mainDiv");
        var headerDivWithShadow = document.getElementById("headerDivWithShadow");
        var menuInnerTable = document.getElementById("menuInnerTable");

        RefreshUICore(menuAreaDiv, viewSite, toggleNavigation_m, mainDiv, headerDivWithShadow, menuInnerTable, forseUpdateMenuPosition);
        if (typeof xaf != 'undefined' && typeof xaf.ASPxClientMenuController != 'undefined') {
            xaf.ASPxClientMenuController.UpdateNestedFrameMenuWidth();
        }
    }
}
function RefreshUICore(menuAreaDiv, viewSite, toggleNavigation_m, mainDiv, headerDivWithShadow, menuInnerTable, forseUpdateMenuPosition) {
    if (menuAreaDiv && viewSite && toggleNavigation_m && mainDiv && headerDivWithShadow && menuInnerTable) {
        UpdateMenuAreaPosition(menuAreaDiv, viewSite.clientWidth, viewSite.clientLeft, viewSite.offsetLeft, viewSite, mainDiv, toggleNavigation_m, headerDivWithShadow, menuInnerTable, window.menuState, forseUpdateMenuPosition);
    }
    if (window.xafNavigation && !window.menuState.moved) {
        window.xafNavigation.SetShowNavigationPosition(document.getElementById("headerTableDiv").clientHeight - GetScrollPosition());
    }
    if (window.xafHeightAdjuster) {
        window.xafHeightAdjuster.Adjust();
    }
}

function MoveMenu(menuAreaDiv, viewSite, toggleNavigation_m, mainDiv, menuState, forseUpdateMenuPosition) {
    if (!menuState.moved) {
        viewSite.style.marginTop = menuAreaDiv.clientHeight + "px";;
        ASPx.AddClassNameToElement(headerDivWithShadow, "Shadow darkGray width100");
        ASPx.AddClassNameToElement(menuAreaDiv, "paddings");
        ASPx.RemoveClassNameFromElement(toggleNavigation_m, "xafHidden");
        ASPx.AddClassNameToElement(menuInnerTable, "movedInnerTable");
        ASPx.AddClassNameToElement(menuAreaDiv, "movedMenu darkGray width100");

        var toggleSeparator_m = document.getElementById("toggleSeparator_m");
        ASPx.RemoveClassNameFromElement(toggleSeparator_m, "xafHidden");

        window.menuNewHeight = $(menuAreaDiv).height();
        if (window.xafNavigation) { //TODO Crimp WTF?
            window.xafNavigation.SetShowNavigationPosition(menuNewHeight);
        }
        menuState.moved = true;

        UpdateMenuAreaPositionCore(menuAreaDiv, null, viewSite, mainDiv, false);
    }
    else {
        if (forseUpdateMenuPosition) {
            UpdateMenuAreaPositionCore(menuAreaDiv, null, viewSite, mainDiv, false);
        }
    }
    headerDivWithShadow.style.height = menuAreaDiv.clientHeight + "px";
}
function RestoreMenu(menuAreaDiv, viewSite, viewSiteclientWidth, toggleNavigation_m, headerDivWithShadow, menuInnerTable, headerDiv, menuState, forseUpdateMenuPosition) {
    if (menuState.moved) {
        var toggleSeparator_m = document.getElementById("toggleSeparator_m");
        ASPx.AddClassNameToElement(toggleSeparator_m, "xafHidden");

        ASPx.AddClassNameToElement(toggleNavigation_m, "xafHidden");
        ASPx.RemoveClassNameFromElement(menuAreaDiv, "movedMenu darkGray width100");
        ASPx.RemoveClassNameFromElement(menuAreaDiv, "paddings");
        ASPx.RemoveClassNameFromElement(headerDivWithShadow, "Shadow darkGray width100");
        ASPx.RemoveClassNameFromElement(menuInnerTable, "movedInnerTable");
        headerDivWithShadow.style.height = "0px";
        viewSite.style.marginTop = "0px";
        window.xafNavigation.SetShowNavigationPosition(headerDiv.clientHeight);
        menuState.moved = false;

        UpdateMenuAreaPositionCore(menuAreaDiv, viewSiteclientWidth, viewSite, mainDiv, true);
    } else {
        if (forseUpdateMenuPosition) {
            UpdateMenuAreaPositionCore(menuAreaDiv, viewSiteclientWidth, viewSite, mainDiv, true);
        }
    }
}
function RefreshMainMenu() {
    if (window.mainMenu) {
        ForeseRefreshUI(true);
        mainMenu.AdjustControl();
    }
}
function UpdateMenuAreaPosition(menuAreaDiv, viewSiteclientWidth, viewSiteclientLeft, viewSiteoffsetLeft, viewSite, mainDiv, toggleNavigation_m, headerDivWithShadow, menuInnerTable, menuState, forseUpdateMenuPosition) {
    var headerTableDiv = document.getElementById("headerTable");
    if (ShouldMoveMenu($(window).scrollTop(), headerTableDiv.scrollHeight)) {
        MoveMenu(menuAreaDiv, viewSite, toggleNavigation_m, mainDiv, menuState, forseUpdateMenuPosition);
    }
    else {
        RestoreMenu(menuAreaDiv, viewSite, viewSite.scrollWidth, toggleNavigation_m, headerDivWithShadow, menuInnerTable, headerTableDiv, window.menuState, forseUpdateMenuPosition);
    }
}
function UpdateMenuAreaPositionCore(menuAreaDiv, viewSiteclientWidth, viewSite, mainDiv, menuMoved) {
    if (menuMoved) {
        menuAreaDiv.style.width = viewSiteclientWidth + "px";
        menuAreaDiv.style.left = viewSite.clientLeft + viewSite.offsetLeft + "px";
    } else {
        menuAreaDiv.style.left = mainDiv.clientLeft + mainDiv.offsetLeft + "px";
        menuAreaDiv.style.width = $(mainDiv).width() + "px";
    }
}
//##############################################


function InitHeader(header) {
    header.style.height = header.scrollHeight + "px";
}

var windowWidth = 0;
var menuRefreshedInNavController = false; //lock for rotating mobile devices
function WindowResized() {
    var newWindowWidth = GetClientWidth();
    if (windowWidth != newWindowWidth) {
        windowWidth = newWindowWidth;
        if (window.xafNavigation) {
            window.xafNavigation.RefreshState();
        }
        RefreshUI();
        if (!menuRefreshedInNavController) {
            RefreshMainMenu();
        } else {
            menuRefreshedInNavController = false;
        }
    }
}
function WindowLoaded() {
    window.pageLoaded = true;
    var navigationElement = document.getElementById("Vertical_navigation");
    var toggleNavigationElement = document.getElementById("toggleNavigation");
    var toggleNavigation_mElement = document.getElementById("toggleNavigation_m");
    if (navigationElement && toggleNavigationElement && toggleNavigation_mElement) {
        var showNavigation = window.ClientParams.Get("ShowNavigationPanelOnStart");
        window.xafNavigation = new XafNavigation(navigationElement, toggleNavigationElement, toggleNavigation_mElement, showNavigation);
    }
    var mainDivElement = document.getElementById("mainDiv");
    if (mainDivElement) {
        window.xafHeightAdjuster = new XafHeightAdjuster(mainDivElement);
    }
    var headerTable = document.getElementById("headerTable");
    if (headerTable) {
        InitHeader(headerTable);
    }
    RefreshUI();
    if (PageLoaded) {
        PageLoaded();
    }
    if (xaf.PopupControllersManager) {
        xaf.PopupControllersManager.CreatePopupFrameController(null);
    }
}

$(window).resize(function () {
    WindowResized();
});

$(window).load(function () {
    WindowLoaded();
});

function Init() {
    if (window.xafNavigation && window.xafNavigation.IsOnTop()) {
        window.xafNavigation.Hide();
    }

    windowWidth = GetClientWidth();
    window.lockUpdate = false;

    RefreshUI();
}