// Show notification 
(function() {

    window.xafFramework = window.xafFramework || {};
    window.onmousedown = function () {
        window.MouseDown = true;
    }
    window.onmouseup = function () {
        window.MouseDown = false;
    }

    function ShowNotificationsWindow() {
        var result = xafFramework.isInited && (!window.ActivePopupControls || ActivePopupControls.length === 0) && !window.MouseDown;
        if (result) {
            var now = new Date().toString();
            RaiseXafCallback(globalCallbackControl, 'NotificationController', "ShowNotificationView" + now, '', false, false);
        }
        return result;
    }

    function EnqueueNotificationsWindow() {
        if (!xafFramework.waitHandler) {
            if (!ShowNotificationsWindow()) {
                window.xafFramework.waitHandler = setInterval(function () {
                    if (ShowNotificationsWindow()) {
                        clearInterval(window.xafFramework.waitHandler);
                        window.xafFramework.waitHandler = null;
                    }
                }, 3000);
            }
        }
    }

    function RegisterNotificationCallback(refreshInterval, startDelay, url) {
        window.requestInProgress = false;
        if (!xafFramework.refreshInterval && refreshInterval > startDelay || refreshInterval < 0) {
            setTimeout(function () {
                PerformXmlHttp(url);
            }, startDelay);
        }
        if (refreshInterval > 0) {
            xafFramework.refreshInterval = refreshInterval || xafFramework.refreshInterval || 150000;
            if (!xafFramework._refreshHandler) {
                xafFramework._refreshHandler = setInterval(function () {
                    PerformXmlHttp(url);
                }, xafFramework.refreshInterval);
            }
        }
    }
    function PerformXmlHttp(url) {
        if (!window.requestInProgress) {
            window.requestInProgress = true;
            SendXmlHttp(url);
        }
    }

    function EnqueueNotificationsWindowData(url, showView) {
        if (!xafFramework.waitTimer) {
            if (showView) {
                EnqueueNotificationsWindow();
            }
            else {
                window.xafFramework.waitTimer = setInterval(function () {
                    if (!inProcess) {
                        var xmlHttp = getXmlHttp();
                        xmlHttp.onreadystatechange = function () {
                            if (xmlHttp.readyState == 4) {
                                if (xmlHttp.status == 200) {
                                    inProcess = false;
                                    if (xmlHttp.responseText.indexOf("ShowNotificationView") > -1) {
                                        clearInterval(window.xafFramework.waitTimer);
                                        window.xafFramework.waitTimer = null;
                                        EnqueueNotificationsWindow();
                                    }
                                }
                            }
                        };
                        xmlHttp.open("GET", url, true);
                        xmlHttp.send();
                        var inProcess = true;
                    }
                }, 3000);
            }
        }
    }

    function SendXmlHttp(url) {
        var xmlHttp = getXmlHttp();
        xmlHttp.onreadystatechange = function () {
            if (xmlHttp.readyState == 4) {
                if (xmlHttp.status == 200) {
                    if (xmlHttp.responseText.indexOf("ShowNotificationView") > -1) {
                        EnqueueNotificationsWindowData(url, true);
                    }
                    if (xmlHttp.responseText.indexOf("WaitServiceAvailable") > -1) {
                        EnqueueNotificationsWindowData(url, false);
                    }
                    if (xmlHttp.responseText.indexOf("Resume") > -1) {
                        window.requestInProgress = false;
                    }
                }
            }
        };
        xmlHttp.open("GET", url, true);
        xmlHttp.send();
    }

    function getXmlHttp() {
        var xmlhttp;
        try {
            xmlhttp = new ActiveXObject("Msxml2.XMLHTTP");
        } catch (e) {
            try {
                xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
            } catch (E) {
                xmlhttp = false;
            }
        }
        if (!xmlhttp && typeof XMLHttpRequest != 'undefined') {
            xmlhttp = new XMLHttpRequest();
        }
        return xmlhttp;
    }
    function ClearTimersScript() {
        clearInterval(window.xafFramework.waitTimer);
        window.xafFramework.waitTimer = null;
        clearInterval(xafFramework._refreshHandler);
        xafFramework._refreshHandler = null;
        clearInterval(window.xafFramework.waitTimer);
        window.xafFramework.waitTimer = null;
        xafFramework.refreshInterval = null;
    }

    window.xafFramework.RegisterNotificationCallback = RegisterNotificationCallback;
    window.xafFramework.ClearTimersScript = ClearTimersScript;
})();