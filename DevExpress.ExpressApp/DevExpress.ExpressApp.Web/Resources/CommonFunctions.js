
var isIE = navigator.appVersion.indexOf('MSIE') > -1;
var dialog = {};

function WaitPageLoad(callbackFunction, waitAnimateCompleted) {
    if (window.top.pageLoaded && (window.top.animateComplete || !waitAnimateCompleted)) {
        callbackFunction();
    } else {
        if (!window.top.animateComplete && waitAnimateCompleted) {
            WaitAnimateComplete(function () {
                WaitPageLoadCore(callbackFunction);
            });
        } else {
            WaitPageLoadCore(callbackFunction);
        }
    }
}
function WaitPageLoadCore(callbackFunction) {
    if(window.top.pageLoaded){
        callbackFunction();
    }else{
        if (!window.top.pageLoadedFunctions || window.top.pageLoadedFunctions == null) {
            window.top.pageLoadedFunctions = [];
        }
        window.top.pageLoadedFunctions.push(callbackFunction);
    }
}
function PageLoaded() {
    if (window.top.pageLoadedFunctions && window.top.pageLoadedFunctions != null) {
        for (var i = 0; i < window.top.pageLoadedFunctions.length; i++) {
            window.top.pageLoadedFunctions[i]();
        }
    }
    window.top.pageLoadedFunctions = null;
}
function WaitAnimateComplete(callbackFunction) {
    if (window.top.animateComplete) {
        callbackFunction();
    } else {
        if (!window.top.animateCompleteCallbackFunctions || window.top.animateCompleteCallbackFunctions == null) {
            window.top.animateCompleteCallbackFunctions = [];
        }
        window.top.animateCompleteCallbackFunctions.push(callbackFunction);
    }
}
function AnimateComplete() {
    if (window.top.animateCompleteCallbackFunctions && window.top.animateCompleteCallbackFunctions != null) {
        for (var i = 0; i < window.top.animateCompleteCallbackFunctions.length; i++) {
            window.top.animateCompleteCallbackFunctions[i]();
        }
    }
    window.top.animateCompleteCallbackFunctions = null;
}

function initializePopupWindow(callbackControlID, updatePanelID, callBackFuncName, forcePostBack, targetControlId, wrapperPanelId) {
    window.lockUpdate = true;
    dialog.callbackFunc = callBackFuncName;
    dialog.forcePostBack = forcePostBack;
    dialog.targetControlId = targetControlId;

    var topWindow = window.top;
    var callbackControl = window[callbackControlID];
    var popupControlId = callbackControl.cpControlID.toString();

    topWindow[popupControlId] = callbackControl.cpMarkup;
    if (!callbackControl.cpMarkup) {
        var popupWrapper = document.getElementById(wrapperPanelId);
        if (popupWrapper) {
            topWindow[popupControlId] = popupWrapper.innerHTML;
            popupWrapper.removeNode(true);
        }
    }

    if (topWindow == window) {
        setTimeout(function () { initializePopupWindowCore(popupControlId, updatePanelID); }, 0);
    } else {
        window.top.easyTestOpenPopupWindowTimer = setTimeout(";", 10000);
        initializePopupWindowCore.call(topWindow, popupControlId, updatePanelID);
    }
}
function initializePopupWindowCore(popupControlId, updatePanelID) {
    var mainWindow = window.top;
    var updatePanel = document.getElementById(updatePanelID);
    var popupControlsContainer = getPopupControlsContainer(updatePanel);
    var markupContainer = document.createElement("div");
    popupControlsContainer.appendChild(markupContainer);
    var markup = mainWindow[popupControlId];
    delete mainWindow[popupControlId];
    ASPx.SetInnerHtml(markupContainer, markup);
    mainWindow.ASPx.ProcessScriptsAndLinks('fake_control', true);
    if (!mainWindow.popupControlContainers) {
        mainWindow.popupControlContainers = {};
    }
    mainWindow.popupControlContainers[popupControlId] = markupContainer;
}
function getPopupControlsContainer(updatePanel) {
    var popupControlsContainer = window.parent.document.getElementById("popupControlsContainer");
    if (!popupControlsContainer) {
        popupControlsContainer = document.createElement("div");
        updatePanel.appendChild(popupControlsContainer);
        popupControlsContainer.setAttribute("id", "popupControlsContainer");
    }
    return popupControlsContainer;
}
function closeXafPopupWindow(popupWindowId) {
    var popupControlToClose = null;
    var popupControls = window.top.popupControlsHash;
    if (popupControls) {
        popupControlToClose = popupControls[popupWindowId];
        delete popupControls[popupWindowId];
    }
    if (popupControlToClose) {
        HidePopupCore(popupControlToClose);
    }
}
function HidePopupCore(popupControl) {
    if (popupControl.NewStyle) {
        AnimateNewPopupClose(popupControl);
    } else {
        popupControl.Hide();
    }
}
function AnimateNewPopupClose(popupControl) {
    window.top.animateComplete = false;
    popupControl.isAShown = false;

    window.setTimeout(function() {
        var element = popupControl.GetWindowElement(-1);
        var modalGrayWindow = popupControl.GetWindowModalElement(-1);
        var hideLeftPos = GetClientWidth();
        DevExpress.fx.animate(element, {
            type: 'slide',
            to: { left: hideLeftPos },
            complete: function () {
                HeaderAndFooterShowBodyScroll();
                popupControl.Hide();
                popupControl.isAShown = true;
                window.top.animateComplete = true;
                AnimateComplete();
            }
        })
        DevExpress.fx.animate(modalGrayWindow, {
            type: 'fadeOut'
        })
    }, 0);
}
function HeaderAndFooterHideBodyScroll() {
    var massLenght = xaf.PopupControllersManager.FrameControllers.length;
    if (massLenght == 2) {
        var headerElement = window.top.document.getElementById("headerTableDiv");
        var footerElement = window.top.document.getElementById("footer");
        headerElement.style.paddingRight = '40px';
        footerElement.style.paddingRight = '40px';
    }

}
function HeaderAndFooterShowBodyScroll() {
    var massLenght = xaf.PopupControllersManager.FrameControllers.length;
    if (massLenght == 2) {
        var headerElement = window.top.document.getElementById("headerTableDiv");
        var footerElement = window.top.document.getElementById("footer");
        headerElement.style.paddingRight = '';
        footerElement.style.paddingRight = '';
    }
}

function closeActiveXafPopupWindow() {
    var popupControl = GetActivePopupControl(window.parent);
    if (popupControl) {
        HidePopupCore(popupControl);
    }
}
function onPopupControlPopUp(popupControl, canManageSize, defaultHeight, isAutoHeightEnabled, isNewStyle, isMobile, showInFindPopup) {
    if (isNewStyle) {
        ShowNewPopup(popupControl, isMobile, showInFindPopup);
    }

    var contentIFrame = popupControl.GetContentIFrame();
    if (contentIFrame) {
        attachElementEvent(contentIFrame, 'load', function (s) { OnIFrameLoad(s, canManageSize, popupControl); });
    }
    popupControl.DefaultHeight = defaultHeight;
    popupControl.IsAutoHeightEnabled = isAutoHeightEnabled;
    RemoveInvalidPopupControls();
    if (!window.ActivePopupControls) {
        window.ActivePopupControls = [];
    }
    window.ActivePopupControls.push(popupControl);
}
function onPopupControlShown(popupControl) {
    var iframe = popupControl.GetContentIFrame();
    if (iframe && !iframe.isLoaded) {
        startProgress(iframe);
    }
}
function onPopupControlCloseUp(popupControl) {
    if (popupControl.NewStyle) {
        CustomPopupControlClose(popupControl);
    }
    popupControl.SetContentUrl('');
    for (var i = window.ActivePopupControls.length - 1; i >= 0; i--) {
        if (window.ActivePopupControls[i] === popupControl) {
            var popupWindow = GetIFrameWindow(popupControl);
            if (popupWindow && popupWindow.PopupClosed) {
                popupWindow.PopupClosed();
            }
            window.ActivePopupControls.splice(i, 1);
            break;
        }
    }
    var popupControlContainer = window.popupControlContainers[popupControl.uniqueID];
    if (popupControlContainer) {
        delete window.popupControlContainers[popupControl.uniqueID];
        window.setTimeout(function () { popupControlContainer.parentNode.removeChild(popupControlContainer); }, 0);
    }
}
function onPopupControlCloseButtonClick(popupControl) {
    var popupWindow = GetIFrameWindow(popupControl);
    if (popupWindow && popupWindow.dialogOpener && popupWindow.dialogOpener.NotifyWindowCloseControl && popupWindow.NotifyWindowCloseControl) {
        popupWindow.dialogOpener.NotifyWindowCloseControl.PerformCallback(popupWindow.NotifyWindowCloseControl.cpServerWindowID);
    }
}

function DoCallback(callbackFunc, forcePostBack, targetControlId) {
    eval(callbackFunc);
    if (forcePostBack && __doPostBack != null) {
        __doPostBack(targetControlId, 'forcePostBack');
    }
}
function GetActivePopupControl(mainWindow) {
    var activePopupControls = mainWindow.ActivePopupControls;
    if (activePopupControls && activePopupControls.length > 0) {
        return activePopupControls[activePopupControls.length - 1];
    }
    return null;
}
function GetIFrameIndex(iframe) {
    if (window.ActivePopupControls) {
        for (var index = 0; index < window.ActivePopupControls.length; index++) {
            if (window.ActivePopupControls[index].GetContentIFrame() == iframe) {
                return index;
            }
        }
    }
    return -1;
}
function GetWindowHeight() {
    var height = 0;
    if (typeof (window.innerHeight) == 'number') {
        height = window.innerHeight;
    } else if (document.documentElement && document.documentElement.clientHeight) {
        height = document.documentElement.clientHeight;
    } else if (document.body && document.body.clientHeight) {
        height = document.body.clientHeight;
    }
    var margin = 0;
    if (document.body.currentStyle) {
        margin = parseInt(document.body.currentStyle.margin);
    }
    return parseInt(height) - (margin * 2);
}
function UpdatePopupWindowSize(popupControl) {
    var popupBody = GetIFrameWindow(popupControl).document.body;
    ASPx.RemoveClassNameFromElement(popupBody, "PopupSizeCalculated");
    var headerElementHeight = popupControl.GetWindowHeaderElement(-1) ? popupControl.GetWindowHeaderElement(-1).offsetHeight : 0;
    var footerElementHeight = popupControl.GetWindowFooterElement(-1) ? popupControl.GetWindowFooterElement(-1).offsetHeight : 0;
    var contentHeight = popupBody.scrollHeight + headerElementHeight + footerElementHeight + 10;
    var windowHeight = GetWindowHeight();
    contentHeight = contentHeight < windowHeight ? contentHeight : windowHeight;
    popupControl.SetHeight(popupControl.IsAutoHeightEnabled && contentHeight < popupControl.DefaultHeight ? popupControl.DefaultHeight : contentHeight);
    ASPx.AddClassNameToElement(popupBody, "PopupSizeCalculated");
}
function IsValidPopupControl(popupControl) {
    return popupControl.GetWindowElement(-1) != null;
}
function RemoveInvalidPopupControls() {
    if (window.ActivePopupControls) {
        for (var i = window.ActivePopupControls.length - 1; i >= 0; i--) {
            if (!IsValidPopupControl(window.ActivePopupControls[i])) {
                window.ActivePopupControls.splice(i, 1);
            }
        }
    }
}
function OnIFrameLoad(evt, canManageSize, popupControl) {
    var iframe = ASPx.Evt.GetEventSource(evt);
    var index = GetIFrameIndex(iframe);
    if (index >= 0) {
        var activePopupControl = window.ActivePopupControls[index];
        if (activePopupControl) {
            if (canManageSize) {
                UpdatePopupWindowSize(activePopupControl);
            }
            if (!popupControl.NewStyle) {
                activePopupControl.UpdatePosition();
            }
        }
        iframe.contentWindow.dialogOpener = index > 0 ? GetIFrameWindow(window.ActivePopupControls[index - 1]) : window;
    }
    if (!iframe.isLoaded) {
        iframe.isLoaded = true;
        stopProgress();
        if (window.FrameLoaded) {
            window.FrameLoaded();
        }

        UpdatePopupControlsHash(popupControl);

        if (window.top.easyTestOpenPopupWindowTimer) {
            clearTimeout(window.top.easyTestOpenPopupWindowTimer);
            window.top.easyTestOpenPopupWindowTimer = null;
        }
        if (window.OnIFrameLoaded) {
            window.OnIFrameLoaded(iframe, popupControl);
        }
    }
}
function UpdatePopupControlsHash(popupControl) {
    if (!window.top.popupControlsHash) {
        window.top.popupControlsHash = {};
    }
    if (window.top.activePopupWindowID) {
        window.top.popupControlsHash[activePopupWindowID] = popupControl;
        window.top.activePopupWindowID = null;
    }
}
function GetIFrameWindow(popupControl) {
    return popupControl ? popupControl.GetContentIFrame().contentWindow : null;
}
function showDialogWindow(url, width, height) {
    var openWindowAttrs = 'status=yes,dependent=yes,resizable=yes,scrollbars=yes,width=' + width + ',height=' + height;
    if (isIE) {
        openWindowAttrs += ',left=' + (window.screen.availWidth - width) / 2 + ',top=' + (window.screen.availHeight - height) / 2;
    } else {
        openWindowAttrs += ',screenX=' + window.screenX + ((window.outerWidth - width) / 2) + ',screenY=' + window.screenY + ((window.outerHeight - height) / 2);
    }
    window.open(url, (new Date()).getTime(), openWindowAttrs);
}
function attachWindowEvent(name, handler) {
    attachElementEvent(window, name, handler);
}
function detachWindowEvent(name, handler) {
    detachElementEvent(window, name, handler);
}
function attachElementEvent(element, name, handler) {
    if(element.addEventListener) {
        element.addEventListener(name, handler, false);
    }
    else {
        element.attachEvent('on' + name, handler);
    }
}
function detachElementEvent(element, name, handler) {
    if(element.removeEventListener) {
        element.removeEventListener(name, handler, false);
    }
    else {
        element.detachEvent('on' + name, handler);
    }
}
function xafHtmlDecode(encodedString) {
    if (encodedString) {
        return encodedString.replace(/&lt;/g, '<').replace(/&gt;/g, '>').replace(/&quot;/g, '"').replace(/&amp;/g, '&').replace(/&#39;/g, "'");
    }
    return '';
}
function disableEnterKey(e) {
    var key = window.event ? window.event.keyCode : e.which;
    var srcElement = window.event ? window.event.srcElement : e.target;
    if (srcElement && srcElement.tagName.toLowerCase() == 'input' && srcElement.type.toLowerCase() != 'submit' && key == 13) {
        if (window.event) {
            window.event.returnValue = false;
            window.event.cancelBubble = true;
        } else {
            e.preventDefault();
            e.stopPropagation();
        }
        if (window.xafProcessEnterKeyDownScript) {
            eval(window.xafProcessEnterKeyDownScript);
        }
        return false;
    }
    return true;
}
function xafEvalFunc(scriptText) {
    eval(scriptText);
}
function xafDoPostBack(targetId, argument) {
    __doPostBack(targetId, argument);
}
var isCancelProgress = false;
function cancelProgress() {
    isCancelProgress = true;
}
function startProgress(htmlElement) {
    if (!isCancelProgress) {
        if (window.NewStyle) {
            xafProgressControl.Show();
        } else {
            if (!htmlElement) {
                xafProgressControl.Show();
            } else {
                xafProgressControl.ShowInElement(htmlElement);
            }
        }
        document.onstop = stopProgress;
        if (xafProgressControl.cpUseOnBlurInStartProgress) {
            window.onblur = stopProgress;
        }
    }
    isCancelProgress = false;
}
function stopProgress() {
    if (xafProgressControl && xafProgressControl.shown) {
        xafProgressControl.Hide();
        xafProgressControl.started = false;
    }
    document.body.onscroll = null;
    document.body.onresize = null;
}
function runProgressWithDelay() {
    if (!xafProgressControl.started) {
        xafProgressControl.started = true;
        if (ASPx.Browser.Opera) {
            startProgress();
        }
        else {
            window.setTimeout('startProgress()', 1);
        }
    }
    return true;
}
function SessionKeepAliveReconnect() {
    var sessionKeepAliveImage = new Image(1, 1);
    sessionKeepAliveImage.src = 'DXX.axd?handlerName=SessionKeepAliveReconnect&t=' + encodeURI(new Date().toString());
}
if (window.document && window.document.body) {
    attachElementEvent(window.document.body, "keypress", disableEnterKey);
}
var scrollableControl;
var scrollXPositionHolder, scrollYPositionHolder;
function scrollControlOnLoadCore(XId, YId, scrollableControlId) {
    if (scrollableControlId && scrollableControlId != '') {
        scrollableControl = document.getElementById(scrollableControlId);
        attachElementEvent(scrollableControl, "scroll", scrollControlOnScroll);
    }
    if (!scrollableControl) {
        scrollableControl = document.documentElement;
        attachWindowEvent("scroll", scrollControlOnScroll);
    }
    scrollXPositionHolder = document.getElementById(XId);
    scrollYPositionHolder = document.getElementById(YId);
    if (scrollXPositionHolder && scrollYPositionHolder) {
        var leftPosition = scrollXPositionHolder.value;
        var topPosition = scrollYPositionHolder.value;

        scrollableControl.scrollLeft = parseInt(leftPosition);
        scrollableControl.scrollTop = parseInt(topPosition);
    }
}
function scrollControlOnScroll() {
    scrollXPositionHolder.value = scrollableControl.scrollLeft;
    scrollYPositionHolder.value = scrollableControl.scrollTop;
}
function xafDoCallback(targetId, argument, clientCallback, context, clientErrorCallback, useAsync) {
    WebForm_DoCallback(targetId, argument, clientCallback, context, clientErrorCallback, useAsync);
}
function xafWaitForCallback(onEndCallbackDelegate) {
    if (!window.xafPendingDelegates) {
        window.xafPendingDelegates = new Array();
    }
    var delegate = function () {
        if (typeof (onEndCallbackDelegate) == 'function') {
            onEndCallbackDelegate.call();
        } else {
            eval(onEndCallbackDelegate);
        }
    };
    xafPendingDelegates[xafPendingDelegates.length] = delegate;
    xafCheckPendingDelegates();
}
function xafHasPendingCallbacks() {
    var hasPendingCallbacks = false;
    try {
        if (__pendingCallbacks) {
            for (var i = 0; i < __pendingCallbacks.length; i++) {
                if (__pendingCallbacks[i]) {
                    hasPendingCallbacks = true;
                    break;
                }
            }
        }
    }
    catch (err) { }
    return hasPendingCallbacks;
}
function xafCheckPendingDelegates() {
    if (xafHasPendingCallbacks()) {
        window.setTimeout("xafCheckPendingDelegates()", 100);
    } else {
        if (window.xafPendingDelegates) {
            var pendingDelegates = window.xafPendingDelegates;
            window.xafPendingDelegates = null;
            for (var i = 0; i < pendingDelegates.length; i++) {
                pendingDelegates[i].call();
            }
        }
    }
}
function xafDropDownLookupProcessNewObject(targetId) {
    xafDoCallback(targetId, window.ddLookupResult, xafDropDownLookupCallback, null, null, false);
}
function xafDropDownLookupCallback(data) {
    var args = data.split('><');
    var editor = ASPx.GetControlCollection().Get(args[0]);
    var items = args[1].split('|');
    editor.BeginUpdate();
    editor.ClearItems();
    var index = 0;
    for (var i = 0; i < items.length; i++) {
        var iteminfo = items[i].split('<');
        editor.AddItem(xafHtmlDecode(iteminfo[0]), iteminfo[1]);
        if (iteminfo.length > 2) {
            index = i;
        }
    }
    editor.SetSelectedIndex(index);
    editor.EndUpdate();
    var processOnServer = editor.RaiseValueChangedEvent();
    if (processOnServer) {
        editor.SendPostBackInternal('');
    }
}
function xafFindLookupCallback(data) {
    var args = data.split('><');
    var editor = ASPx.GetControlCollection().Get(args[0]);
    editor.SetValue(xafHtmlDecode(args[1]));
    editor.RaiseValueChanged();
}
function xafFindLookupProcessFindObject(targetId, hiddenFieldId, objectKey, immediatePostDataScript) {
    document.getElementById(hiddenFieldId).value = objectKey;
    if (immediatePostDataScript) {
        eval(immediatePostDataScript);
    } else {
        xafDoCallback(targetId, 'found' + objectKey, xafFindLookupCallback, null, null, false);
    }
}
function GetMenuItemFullName(item) {
    var itemFullPath = item.name;
    while (item.parent != null) {
        itemFullPath = item.parent.name + '.' + itemFullPath;
        item = item.parent;
    }
    return itemFullPath;
}
function ForceButtonClick(s, e, script) {
    ASPxClientUtils.PreventEventAndBubble(e);
    var pressed = 0;
    var windowEvent = null;

    if (window.event) {
        windowEvent = window.event;
    }
    else {
        if (parent && parent.event) {
            windowEvent = parent.event;
        }
    }

    if (windowEvent) {
        if (windowEvent.keyCode == 13) {
            windowEvent.returnValue = false;
            windowEvent.cancelBubble = true;
            script(s, e);
        }
    }
    else {
        if (e && e.htmlEvent) {
            e = e.htmlEvent;
            if (e.keyCode == 13) {
                e.preventDefault();
                e.stopPropagation();
                script(s, e);
            }
        }
    }
    return false;
}

function SetMenuItemEnabled(menu, itemName, enabled) {
    if (menu) {
        var item = menu.GetItemByName(itemName);
        if (item) {
            item.SetEnabled(enabled);
        }
    }
}
function SetMenuItemVisible(menu, itemName, visible) {
    if (menu) {
        var item = menu.GetItemByName(itemName);
        if (item) {
            item.SetVisible(visible);
        }
    }
}
function SetMenuProperty(menu, propertyName, propertyValue) {
    if (menu) {
        menu[propertyName] = propertyValue;
    }
}
function ParametrizedActionClientControl(clientID) {
    this.clientID = clientID;
    function getButton() {
        return window[this.clientID + '_B'];
    }
    function getEditor() {
        return window[this.clientID + '_Ed'];
    }
    function getEnabled() {
        return this.GetButton().GetEnabled();
    }
    function setEnabled(enabled) {
        this.GetButton().SetEnabled(enabled);
        this.GetEditor().SetEnabled(enabled);
    }
    function setText(value) {
        this.GetEditor().SetText(value);
    }
    function getText() {
        return this.GetEditor().GetText();

    }
    function doClick() {
        this.GetButton().DoClick();
    }
    this.SetEnabled = setEnabled;
    this.SetText = setText;
    this.DoClick = doClick;
    this.GetText = getText;
    this.GetEnabled = getEnabled;
    this.GetButton = getButton;
    this.GetEditor = getEditor;
}
function DropDownSingleChoiceActionClientControl(clientID) {
    function getLabel() {
        return window[this.clientID + '_L'];
    }
    function getComboBox() {
        return window[this.clientID + '_Cb'];
    }
    function getEnabled() {
        return this.GetComboBox().GetEnabled();
    }
    function getText() {
        return this.GetComboBox().GetText();
    }
    function setText(value) {
        return this.Act(value);
    }
    function throwOperationException(message) {
        var exception = new Object();
        exception.operationError = true;
        exception.message = message;
        throw exception;
    }
    function setEnabled(enabled) {
        this.GetComboBox().SetEnabled(enabled);
        this.GetLabel().SetEnabled(enabled);
    }
    function act(value) {
        if (!value || value == '') {
            this.ThrowOperationException('Item is not specified.');
            return;
        }
        var isFound = false;
        for (i = 0; i < this.GetComboBox().GetItemCount() ; i++) {
            if (this.GetComboBox().GetItem(i).text == value) {
                this.GetComboBox().SetSelectedIndex(i);
                ASPx.EValueChanged(this.GetComboBox().name);
                isFound = true;
                break;
            }
        }
        if (!isFound) {
            this.ThrowOperationException('It is impossible to set value in control: ' + this.caption + '. The list does not contains value');
        }
    }
    this.clientID = clientID;
    this.GetComboBox = getComboBox;
    this.GetLabel = getLabel;
    this.SetEnabled = setEnabled;
    this.GetEnabled = getEnabled;
    this.GetText = getText;
    this.SetText = setText;
    this.ThrowOperationException = throwOperationException;
    this.Act = act;
}
function TreeSingleChoiceActionClientControl(clientID) {
    function getMainButton() {
        return window[this.clientID + '_MB'];
    }
    function getDropDownButton() {
        return window[this.clientID + '_DDB'];
    }
    function getMenu() {
        return window[this.clientID + '_M'];
    }
    function getEnabled() {
        return this.GetMainButton().GetEnabled();
    }
    function throwOperationException(message) {
        var exception = new Object();
        exception.operationError = true;
        exception.message = message;
        throw exception;
    }
    function setEnabled(enabled) {
        this.GetMainButton().SetEnabled(enabled);
        this.GetDropDownButton().SetEnabled(enabled);
    }
    function act(value) {
        if (!value || value == '') {
            this.GetMainButton().DoClick();
            return;
        }
        var path = value.split('.');
        if (path.length == 0) {
            this.ThrowOperationException('Item is not specified.');
        }
        var currentItem = null;
        var itemHolder = this.GetMenu();
        var pathIndex = 0;
        while (pathIndex < path.length) {
            var found = false;
            for (var i = 0; i < itemHolder.GetItemCount() ; i++) {
                if (itemHolder.GetItem(i).GetText() == path[pathIndex]) {
                    currentItem = itemHolder.GetItem(i);
                    itemHolder = currentItem;
                    found = true;
                    pathIndex++;
                    break;
                }
            }
            if (!found) {
                this.ThrowOperationException('The ' + value + ' item not found.');
            }
        }
        if (currentItem.GetEnabled()) {
            this.GetMenu().DoItemClick(currentItem.GetIndexPath(), false, null);
        }
        else {
            this.ThrowOperationException('The ' + value + ' item is disabled.');
        }
    }
    this.clientID = clientID;
    this.GetMainButton = getMainButton;
    this.GetDropDownButton = getDropDownButton;
    this.GetMenu = getMenu;
    this.SetEnabled = setEnabled;
    this.GetEnabled = getEnabled;
    this.ThrowOperationException = throwOperationException;
    this.Act = act;
}
function ShowConfirmationMessage(confirmation) {
    var confirmed = true;
    if (confirmation != '') {
        confirmed = confirm(xafHtmlDecode(confirmation));
    }
    return confirmed;
}

function GetScrollPosition() {
    return $(window.top.document).scrollTop();
}
function GetClientHeight() {
    return (window.top.innerHeight > 0) ? window.top.innerHeight : screen.height;
}
function GetClientWidth() {
    return GetWindowWidth(window.top);
}
function GetWindowWidth(targetWindow) {
    return (targetWindow.innerWidth > 0) ? targetWindow.innerWidth : screen.width;
}
function FillClientParams() {
    if (this.ClientParams != undefined) {
        ClientParams.Set("ClientWidth", GetClientWidth());
        ClientParams.Set("ClientHeight", GetClientHeight());
        ClientParams.Set("ScrollPosition", GetScrollPosition());
        if (window.menuState && !window.menuState.moved && window.xafNavigation) {
            ClientParams.Set("HeaderHeight", window.xafNavigation.GetShowNavigationPosition());
        } else {
            ClientParams.Set("HeaderHeight", -1);
        }
        if (window.top.popupControlState && window.top.popupControlState.opened) {
            ClientParams.Set("popupControlTop", window.top.popupControlState.top);
        }
    }
}
function RaiseXafCallback(callbackControl, handlerId, parameters, confirmation, usePostBack, endCallbackHandler) {
    FillClientParams();

    var isRaised = false;
    if (ShowConfirmationMessage(confirmation)) {
        var parameter = handlerId + ':' + parameters;

        if (usePostBack) {
            callbackControl.SendPostBack(parameter);
        } else {
            if (endCallbackHandler) {
                var endCallbackHandlerWithUnsubscription = function () { endCallbackHandler.call(); callbackControl.EndCallback.RemoveHandler(endCallbackHandlerWithUnsubscription); };
                callbackControl.EndCallback.AddHandler(endCallbackHandlerWithUnsubscription);
            }
            startProgress();
            callbackControl.PerformCallback(parameter);
        }
        isRaised = true;
    }
    return isRaised;
}
function StartCheckQueryString(navigationHandlerId) {
    CheckQueryString(navigationHandlerId);
    window.xafFramework = window.xafFramework || {};
    window.xafFramework.isInited = true;
    setInterval(function () { CheckQueryString(navigationHandlerId); }, 300);
}
function CheckQueryString(navigationHandlerId) {
    var newHash = document.location.hash;
    if (newHash.length > 0) {
        newHash = newHash.substring(1);
    }
    var isRefreshRequired = window.XAFCurrentQueryString != newHash;
    isRefreshRequired = isRefreshRequired && decodeURI(window.XAFCurrentQueryString) != newHash;
    if(isRefreshRequired) {
        window.XAFCurrentQueryString = newHash;
        var query = newHash;
        if (!query) {
            query = 'startup_view';
        }
        startProgress();
        globalCallbackControl.PerformCallback(navigationHandlerId + ':' + query);
    }
}
function UpdateDocumentLocation(hash) {
    window.XAFCurrentQueryString = hash;
    document.location.hash = hash;
}
function ProcessCallbackResult(callbackResult) {
    stopProgress();
    if (callbackResult.cpPageTitle) {
        document.title = callbackResult.cpPageTitle;
    }
    ProcessMarkup(callbackResult);
    if (callbackResult.cpCurrentView && callbackResult.cpCurrentView != '') {
        UpdateDocumentLocation(callbackResult.cpCurrentView);
    }
}
function ProcessMarkup(callbackResult, processScripts) {
    if(callbackResult.cpControlsToUpdate) {
        var controlsToUpdate;
        if(!callbackResult.cpElementsToUpdate) {
            controlsToUpdate = callbackResult.cpControlsToUpdate.split(';');
        } else {
            var sourceControlsToUpdate = callbackResult.cpControlsToUpdate.split(';');
            var elementToUpdate = callbackResult.cpElementsToUpdate.split(';');
            var controlsToUpdate = [];
            for(var i = 0; i < sourceControlsToUpdate.length; i++) {
                for(var x = 0; x < elementToUpdate.length; x++) {
                    if(sourceControlsToUpdate[i].indexOf(elementToUpdate[x]) > -1) {
                        controlsToUpdate[controlsToUpdate.length] = sourceControlsToUpdate[i];
                        break;
                    }
                }
            }
            delete callbackResult.cpElementsToUpdate;
        }
        delete callbackResult.cpControlsToUpdate;
        for(var i = 0; i < controlsToUpdate.length; i++) {
            var containerId = controlsToUpdate[i];
            var markup = callbackResult['cp' + containerId];
            delete callbackResult['cp' + containerId];
            var container = document.getElementById(containerId);
            if(container) {
                ASPx.SetInnerHtml(container, markup);
            }
        }
        if(processScripts) {
            ASPx.ProcessScriptsAndLinks('_fakeControlId');
        }
        if(ASPx.Browser.IE && ASPx.Browser.Version < 9) {
            document.body.className = document.body.className;
        }
    }
}

function ProcessObjectEditResult(editorClientId) {
    editorClientId.SetText(window.resultObject);
}
function ProcessGridRowClick(s, visibleIndex, htmlEvent, script) {
    var selection = document.selection ? document.selection.createRange().text : window.getSelection().toString();
    if(!selection && !s.cpBatchEditMode) {
        var isCommand = false;
        if(htmlEvent.target) {
            isCommand = htmlEvent.target.getAttribute('IsCommand') || htmlEvent.target.getAttribute('IsDetail');
        }
        if(htmlEvent.srcElement) {
            isCommand = htmlEvent.srcElement.getAttribute('IsCommand') || htmlEvent.target.getAttribute('IsDetail');
        }
        if(!isCommand) {
            var row = s.GetItem(visibleIndex);
            var actionArg = row ? row.getAttribute('ActionArg') : null;
            if(actionArg) {
                script = script.replace('ActionArg', '\'' + actionArg + '\'');
                var confirmation = row.getAttribute('Cnfrm');
                if(confirmation && confirmation != '') {
                    confirmation = xafHtmlDecode(confirmation);
                    script = script.replace('Cnfrm', confirmation);
                }
                else {
                    script = script.replace('Cnfrm', '');
                }
                eval(script);
            }
        }
    }
}
function ProcessGridStartBatchEditing(grid, args, securityText) {
    if (args.focusedColumn) {
        if (args.rowValues[args.focusedColumn.index] && args.rowValues[args.focusedColumn.index].text === securityText) {
            args.cancel = true;
        }
        else {
            var condition = true;
            var editor = grid.GetEditor(args.focusedColumn.fieldName);
            if (editor) {
                if (grid.cpReadOnlyColumns && grid.cpReadOnlyColumns.indexOf(args.focusedColumn.fieldName) >= 0) {
                    condition = false;
                }
                if (grid.cpReadOnlyCells && grid.cpReadOnlyCells.indexOf(args.focusedColumn.fieldName + args.visibleIndex) >= 0) {
                    condition = false;
                }
                editor.SetEnabled(condition);
            }
        }
    }
}
function UpdateResizableControlContainer(containerID, controlID, sizeStorageID, minWidth, minHeight) {
    var control = document.getElementById(controlID);
    if (control) {
        control.style.visibility = 'hidden';
    }
    if (document.getElementById(containerID)) {
        var timerId = containerID + 'Timer';
        var elapsedFlag = containerID + 'TimerElapsed';
        if (window[timerId]) {
            window.clearTimeout(window[timerId]);
        }
        if (!window[elapsedFlag]) {
            var script = function () {
                window[elapsedFlag] = true;
                UpdateResizableControlContainer(containerID, controlID, sizeStorageID, minWidth, minHeight);
            };
            window[timerId] = window.setTimeout(script, 500);
        }
        else {
            window[timerId] = undefined;
            window[elapsedFlag] = undefined;
            var container = window[containerID];
            var width = container.GetWidth() - 1;
            var height = container.GetHeight() - 1;
            if (width > 0 && height > 0) {
                var sizeString = width + '/' + height;
                var sizeStorage = document.getElementById(sizeStorageID);
                sizeStorage.value = sizeString;
                container.PerformCallback(sizeString);
            }
        }
    }
}
function RCPVisibilityIntervalHandler(containerID, controlID, sizeStorageID, minWidth, minHeight, intervalID) {
    var isContainerVisible = ASPx.IsElementVisible(document.getElementById(containerID));
    if (isContainerVisible) {
        window.clearInterval(window[intervalID]);
        window[intervalID] = undefined;
        UpdateResizableControlContainer(containerID, controlID, sizeStorageID, minWidth, minHeight);
        attachWindowEvent('resize', function () { UpdateResizableControlContainer(containerID, controlID, sizeStorageID, minWidth, minHeight); });
    }
}
var textSeparator = ";";
function OnListBoxSelectionChanged(listBox, args, checkComboBox) {
    if (args.index == 0)
        args.isSelected ? listBox.SelectAll() : listBox.UnselectAll();
    UpdateSelectAllItemState(listBox);
    UpdateText(listBox, checkComboBox);
}
function UpdateSelectAllItemState(checkListBox) {
    IsAllSelected(checkListBox) ? checkListBox.SelectIndices([0]) : checkListBox.UnselectIndices([0]);
}
function IsAllSelected(checkListBox) {
    for (var i = 1; i < checkListBox.GetItemCount() ; i++)
        if (!checkListBox.GetItem(i).selected)
            return false;
    return true;
}
function UpdateText(checkListBox, checkComboBox) {
    var selectedItems = checkListBox.GetSelectedItems();
    checkComboBox.SetText(GetSelectedItemsText(selectedItems));
}
function SynchronizeListBoxValues(dropDown, args, checkListBox) {
    checkListBox.UnselectAll();
    var texts = dropDown.GetText().split(textSeparator);
    var values = GetValuesByTexts(texts, checkListBox);
    checkListBox.SelectValues(values);
    UpdateSelectAllItemState(checkListBox);
    UpdateText(checkListBox, dropDown);
}
function GetSelectedItemsText(items) {
    var texts = [];
    for (var i = 0; i < items.length; i++)
        if (items[i].index != 0)
            texts.push(items[i].text);
    return texts.join(textSeparator);
}
function GetValuesByTexts(texts, checkListBox) {
    var actualValues = [];
    var value = "";
    for (var i = 0; i < texts.length; i++) {
        value = GetValueByText(texts[i], checkListBox);
        if (value != null)
            actualValues.push(value);
    }
    return actualValues;
}
function GetValueByText(text, checkListBox) {
    for (var i = 0; i < checkListBox.GetItemCount() ; i++)
        if (ASPx.Str.Trim(checkListBox.GetItem(i).text.toUpperCase()) == ASPx.Str.Trim(text.toUpperCase()))
            return checkListBox.GetItem(i).value;
    return null;
}
function ShowHideImageControlEditMode(imageHolderId, uploadControlHolderId, buttonsTableId, isVisible) {
    SetControlVisibility(imageHolderId, isVisible);
    SetControlVisibility(uploadControlHolderId, !isVisible);
    SetControlVisibility(buttonsTableId, isVisible);
}
function SetControlVisibility(controlId, isVisible) {
    var control = document.getElementById(controlId);
    if (control) {
        control.style.display = isVisible ? "" : "none";
        ForceRedrawAppearance(control.parentElement);
    }
    else {
        attachElementEvent(window, "load", function () { SetControlVisibility(controlId, isVisible) });
    }
}
function GetControlVisible(controlId) {
    var control = document.getElementById(controlId);
    if (control) {
        return control.style.display != "none";
    }
    else {
        return false;
    }
}
function ForceRedrawAppearance(element) {
    var width = element.style.width;
    element.style.width = "0px";
    var dummy = element.offsetWidth;
    element.style.width = width;
}