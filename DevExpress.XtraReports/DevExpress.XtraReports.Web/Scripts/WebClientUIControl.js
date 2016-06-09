

(function(window) {
    function checkSupportedMethods() {
        if(Object && Object.defineProperty) {
            var tObj = {};
            try {
                Object.defineProperty(tObj, 'test', { value: 0 });
                if(!Array.prototype.forEach || !Array.prototype.indexOf || [3, 1, 3].indexOf(3, 1) !== 2 || !Function.prototype.bind || !String.prototype.trim) {
                    return false;
                }
                tObj = {};
                tObj.newDataProperty = "abc";
                var tDescriptor = Object.getOwnPropertyDescriptor(tObj, "newDataProperty");
                tDescriptor.writable = false;
                Object.defineProperty(tObj, "newDataProperty", tDescriptor);
            } catch(e) {
                return false;
            }
            return true;
        }
        return false;
    }

    var WebClientUIControl = {
        validateSupportedBrowser: function(hiddenMessageElement, hiddenClassName) {
            var testObject = {};
            var isSupportedBrowser = checkSupportedMethods();
            if(!isSupportedBrowser) {
                $(hiddenMessageElement).removeClass(hiddenClassName);
                if(console && console.error) {
                    console.error('Internet Explorer versions lower than 9 are not supported.');
                }
            }
            return isSupportedBrowser;
        },
        initMenuItems: function(menuItems, menuItemActions) {
            for(var i = 0; i < menuItems.length; i++) {
                var menuItem = menuItems[i];
                var menuItemAction = menuItemActions[i];
                menuItem.clickAction = menuItemAction || $.noop;
            }
        },
        initViewerHandlerUri: function(uri) {
            uri && (DevExpress.Report.Preview.HandlerUri = uri);
        },
        initDesignerHandlerUri: function(uri) {
            uri && (DevExpress.Designer.Report.HandlerUri = uri);
        },
        initClientTimeOut: function(timeOut) {
            DevExpress.Report.Preview.TimeOut = timeOut;
        },
        initDebugMode: function(owner) {
            if(owner) {
                var debugMode = owner.debugMode;
                DevExpress.Designer.DEBUG = debugMode;
            }
        }
    };
    var ASPxClientCustomizeParameterEditorsEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(parameter, info) {
            this.constructor.prototype.constructor.call(this);
            this.parameter = parameter;
            this.info = info;
        }
    });
    //{
    //}
    //{
    //}
    //{
    //}
    //{
    //}

    window.ASPx = window.ASPx || {};
    window.ASPx.WebClientUIControl = WebClientUIControl;
    window.ASPxClientCustomizeParameterEditorsEventArgs = ASPxClientCustomizeParameterEditorsEventArgs;
}(window));