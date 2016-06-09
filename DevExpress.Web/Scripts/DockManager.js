/// <reference path="_references.js"/>

(function() {
var DockPanelBag = ASPx.CreateClass(null, {
    //Ctor
    constructor: function () {
        this.panels = {};
    },

    RegisterPanel: function (panel) {
        this.panels[panel.panelUID] = panel;
    },

    GetPanelByUID: function (panelUID) {
        return this.panels[panelUID];
    },

    ForEachPanel: function (action) {
        for(var key in this.panels) {
            if(!this.panels.hasOwnProperty(key))
                continue;
            var panel = this.panels[key];
            if(panel.GetMainElement())
                action(this.panels[key]);
        }
    },

    GetPanelList: function () {
        var panelList = [];

        this.ForEachPanel(function (panel) {
            panelList.push(panel);
        });

        return panelList;
    }
});

DockPanelBag.instance = null;
DockPanelBag.Get = function () {
    if(!DockPanelBag.instance)
        DockPanelBag.instance = new DockPanelBag();
    return DockPanelBag.instance;
};
var ASPxClientDockManager = ASPx.CreateClass(ASPxClientComponent, {
    //Consts
    BeforeDockServerEventName: "BeforeDock",
    AfterDockServerEventName: "AfterDock",
    BeforeFloatServerEventName: "BeforeFloat",
    AfterFloatServerEventName: "AfterFloat",
    RaiseBeforeDockEventCommand: "EBD",
    RaiseAfterDockEventCommand: "EAD",
    RaiseBeforeFloatEventCommand: "EBF",
    RaiseAfterFloatEventCommand: "EAF",

    //Ctor
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);

        if(!ASPxClientDockManager.instance)
            ASPxClientDockManager.instance = this;

        this.inPostback = false;

        //Server-provided fields
        this.cookieName = '';
        this.clientLayoutState = {};

        //Events
        this.BeforeDock = new ASPxClientEvent();
        this.AfterDock = new ASPxClientEvent();
        this.BeforeFloat = new ASPxClientEvent();
        this.AfterFloat = new ASPxClientEvent();
        this.StartPanelDragging = new ASPxClientEvent();
        this.EndPanelDragging = new ASPxClientEvent();
        this.PanelClosing = new ASPxClientEvent();
        this.PanelCloseUp = new ASPxClientEvent();
        this.PanelPopUp = new ASPxClientEvent();
        this.PanelShown = new ASPxClientEvent();
        this.PanelResize = new ASPxClientEvent();
    },
    PerformCallback: function (parameter) {
        if(!ASPx.IsExists(parameter)) parameter = "";
        this.CreateCallback(parameter);
    },

    //NOTE: this helps prevent duplicate form submission is some cases (see B189327)
    SendPostBack: function (params) {
        if(!this.inPostback) {
            this.inPostback = true;
            ASPxClientControl.prototype.SendPostBack.call(this, params);
        }
    },

    //Layout state
    UpdatePanelLayoutState: function(panel) {
        this.clientLayoutState[panel.panelUID] = panel.GetLayoutStateObject();
    },

    UpdatePanelsLayoutState: function () {
        var instance = this;
        var panelBag = DockPanelBag.Get();

        panelBag.ForEachPanel(function(panel) {
            instance.UpdatePanelLayoutState(panel);
        });

        if(this.cookieName && this.cookieName !== '') {
            ASPx.Cookie.DelCookie(this.cookieName);
            ASPx.Cookie.SetCookie(this.cookieName, ASPx.Json.ToJson(this.clientLayoutState));
        }
    },


    //Events
    GetBeforeDockPostbackArgs: function (panel, zone) {
        return [
            this.RaiseBeforeDockEventCommand,
            panel.panelUID,
            zone.zoneUID,
            zone.GetPanelAfterPlaceholderVisibleIndex() + 1
        ];
    },

    GetAfterDockPostbackArgs: function (panel, zone) {
        return [
            this.RaiseAfterDockEventCommand,
            panel.panelUID,
            zone.zoneUID
        ];
    },

    GetBeforeFloatPostbackArgs: function (panel, zone) {
        return [
            this.RaiseBeforeFloatEventCommand,
            panel.panelUID,
            zone.zoneUID
        ];
    },

    GetAfterFloatPostbackArgs: function (panel, zone) {
        return [
            this.RaiseAfterFloatEventCommand,
            panel.panelUID,
            zone.zoneUID
        ];
    },

    RaiseBeforeDock: function (panel, zone) {
        var processOnServer = this.IsServerEventAssigned(this.BeforeDockServerEventName);
        var args = new ASPxClientDockManagerProcessingModeCancelEventArgs(processOnServer, panel, zone);
        if(!this.BeforeDock.IsEmpty())
            this.BeforeDock.FireEvent(this, args);

        if(!args.cancel && args.processOnServer && this.isInitialized) {
            var postbackArgs = this.GetBeforeDockPostbackArgs(panel, zone);
            this.SendPostBack(ASPx.Json.ToJson(postbackArgs));
            return;
        }

        return !args.cancel;
    },

    RaiseAfterDock: function (panel, zone) {
        var processOnServer = this.IsServerEventAssigned(this.AfterDockServerEventName);
        var args = new ASPxClientDockManagerProcessingModeEventArgs(processOnServer, panel, zone);
        if(!this.AfterDock.IsEmpty())
            this.AfterDock.FireEvent(this, args);
        if(args.processOnServer && this.isInitialized) {
            var postbackArgs = this.GetAfterDockPostbackArgs(panel, zone);
            this.SendPostBack(ASPx.Json.ToJson(postbackArgs));
            return;
        }
    },

    RaiseBeforeFloat: function (panel, zone) {
        var processOnServer = this.IsServerEventAssigned(this.BeforeFloatServerEventName);
        var args = new ASPxClientDockManagerProcessingModeCancelEventArgs(processOnServer, panel, zone);
        if(!this.BeforeFloat.IsEmpty())
            this.BeforeFloat.FireEvent(this, args);

        if(!args.cancel && args.processOnServer && this.isInitialized) {
            var postbackArgs = this.GetBeforeFloatPostbackArgs(panel, zone);
            this.SendPostBack(ASPx.Json.ToJson(postbackArgs));
            return;
        }

        return !args.cancel;
    },

    RaiseAfterFloat: function (panel, zone) {
        var processOnServer = this.IsServerEventAssigned(this.AfterFloatServerEventName);
        var args = new ASPxClientDockManagerProcessingModeEventArgs(processOnServer, panel, zone);
        if(!this.AfterFloat.IsEmpty())
            this.AfterFloat.FireEvent(this, args);

        if(args.processOnServer && this.isInitialized) {
            var postbackArgs = this.GetAfterFloatPostbackArgs(panel, zone);
            this.SendPostBack(ASPx.Json.ToJson(postbackArgs));
            return;
        }
    },

    RaiseStartPanelDragging: function (panel) {
        if(!this.StartPanelDragging.IsEmpty())
            this.StartPanelDragging.FireEvent(this, new ASPxClientDockManagerEventArgs(panel));
    },

    RaiseEndPanelDragging: function (panel) {
        if(!this.EndPanelDragging.IsEmpty())
            this.EndPanelDragging.FireEvent(this, new ASPxClientDockManagerEventArgs(panel));
    },

    RaisePanelClosing: function (panel) {
        if(this.PanelClosing.IsEmpty())
            return false;

        var args = new ASPxClientDockManagerCancelEventArgs(panel);
        this.PanelClosing.FireEvent(this, args);

        return args.cancel;
    },

    RaisePanelCloseUp: function (panel) {
        if(!this.PanelCloseUp.IsEmpty())
            this.PanelCloseUp.FireEvent(this, new ASPxClientDockManagerEventArgs(panel));
    },

    RaisePanelPopUp: function (panel) {
        if(!this.PanelPopUp.IsEmpty())
            this.PanelPopUp.FireEvent(this, new ASPxClientDockManagerEventArgs(panel));
    },

    RaisePanelShown: function (panel) {
        if(!this.PanelShown.IsEmpty())
            this.PanelShown.FireEvent(this, new ASPxClientDockManagerEventArgs(panel));
    },

    RaisePanelResize: function (panel) {
        if(!this.PanelResize.IsEmpty())
            this.PanelResize.FireEvent(this, new ASPxClientDockManagerEventArgs(panel));
    },

    //API
    GetZoneByUID: function (zoneUID) {
        var zoneBag = ASPx.DockZoneBag.Get();
        return zoneBag.GetZoneByUID(zoneUID);
    },
    GetPanelByUID: function (panelUID) {
        var panelBag = DockPanelBag.Get();
        return panelBag.GetPanelByUID(panelUID);
    },
    GetPanels: function (filterPredicate) {
        var panelBag = DockPanelBag.Get();
        return ASPx.RetrieveByPredicate(panelBag.GetPanelList(), filterPredicate);
    },
    GetZones: function (filterPredicate) {
        var zoneBag = ASPx.DockZoneBag.Get();
        return ASPx.RetrieveByPredicate(zoneBag.GetZoneList(), filterPredicate);
    }
});

ASPxClientDockManager.instance = null;

ASPxClientDockManager.Get = function () {
    return ASPxClientDockManager.instance;
};
ASPxClientDockManager.Cast = ASPxClientControl.Cast;
var ASPxClientDockManagerProcessingModeCancelEventArgs = ASPx.CreateClass(ASPxClientProcessingModeCancelEventArgs, {
    constructor: function (processOnServer, panel, zone) {
        this.constructor.prototype.constructor.call(this, processOnServer);
        this.panel = panel;
        this.zone = zone;
    }
});
var ASPxClientDockManagerProcessingModeEventArgs = ASPx.CreateClass(ASPxClientProcessingModeEventArgs, {
    constructor: function (processOnServer, panel, zone) {
        this.constructor.prototype.constructor.call(this, processOnServer);
        this.panel = panel;
        this.zone = zone;
    }
});
var ASPxClientDockManagerEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function (panel) {
        this.constructor.prototype.constructor.call(this);
        this.panel = panel;
    }
});
var ASPxClientDockManagerCancelEventArgs = ASPx.CreateClass(ASPxClientCancelEventArgs, {
    constructor: function (panel) {
        this.constructor.prototype.constructor.call(this);
        this.panel = panel;
    }
});

ASPx.DockPanelBag = DockPanelBag;

window.ASPxClientDockManager = ASPxClientDockManager;
window.ASPxClientDockManagerProcessingModeCancelEventArgs = ASPxClientDockManagerProcessingModeCancelEventArgs;
window.ASPxClientDockManagerProcessingModeEventArgs = ASPxClientDockManagerProcessingModeEventArgs;
window.ASPxClientDockManagerEventArgs = ASPxClientDockManagerEventArgs;
window.ASPxClientDockManagerCancelEventArgs = ASPxClientDockManagerCancelEventArgs;
})();