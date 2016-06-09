/// <reference path="_references.js"/>

(function () {
var ASPxClientTimer = ASPx.CreateClass(ASPxClientComponent, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);

        this.interval = 60000;
        this.clientEnabled = true;
        this.timerID = -1;
        this.Tick = new ASPxClientEvent();
    },
    Initialize: function () {
        if(this.clientEnabled)
            this.Start();
        this.constructor.prototype.Initialize.call(this);
    },

    Start: function () {
        this.Stop();
        this.timerID = window.setInterval(function () { this.DoTick(); }.aspxBind(this), this.interval);
    },
    Stop: function () {
        if(this.timerID == -1) return;
        this.timerID = ASPx.Timer.ClearInterval(this.timerID);
    },
    DoTick: function () {
        var processOnServer = this.RaiseTick();
        if(processOnServer)
            this.SendPostBack("TICK");
    },
    UpdateStateObject: function(){
        this.UpdateStateObjectWithObject({ enabled: this.clientEnabled, interval: this.interval });
    },
    // API
    RaiseTick: function () {
        var processOnServer = this.IsServerEventAssigned("Tick");
        if(!this.Tick.IsEmpty()) {
            var args = new ASPxClientProcessingModeEventArgs(processOnServer);
            this.Tick.FireEvent(this, args);
            processOnServer = args.processOnServer;
        }
        return processOnServer;
    },
    GetEnabled: function () {
        return this.clientEnabled;
    },
    SetEnabled: function (enabled) {
        if(enabled == this.clientEnabled) return;
        if(enabled)
            this.Start();
        else
            this.Stop();
        this.clientEnabled = enabled;
    },
    GetInterval: function () {
        return this.interval;
    },
    SetInterval: function (interval) {
        if(interval < 1) return;
        this.interval = interval;
        if(this.clientEnabled) {
            this.Stop();
            this.Start();
        }
    }
});
ASPxClientTimer.Cast = ASPxClientControl.Cast;

window.ASPxClientTimer = ASPxClientTimer;
})();