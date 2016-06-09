/// <reference path="_references.js"/>

(function () {
var ASPxClientObjectContainer = ASPx.CreateClass(ASPxClientControl, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);
        this.needFixObjectBounds = false;
        this.FlashScriptCommand = new ASPxClientEvent();
    },
    Initialize: function () {
        if(this.needFixObjectBounds)
            this.FixObjectBounds();
        if(ASPx.Browser.WebKitFamily && !this.GetVisible())
            this.SetVisible_Safari(false);
        this.constructor.prototype.Initialize.call(this);
    },
    DoFlashScriptCommand: function (command, args) {
        this.RaiseFlashScriptCommand(command, args);
    },
    FixObjectBounds: function () {
        var mainElement = this.GetMainElement();
        if(mainElement != null && mainElement.body != null &&
                mainElement.body.style != null) {
            mainElement.body.style.border = "none";
            mainElement.body.style.margin = "0px";
            mainElement.body.style.padding = "0px";
            mainElement.body.style.overflow = "hidden";
        }
    },
    SetVisible: function (visible) {
        ASPxClientControl.prototype.SetVisible.call(this, visible);
        if(ASPx.Browser.WebKitFamily) //B32204
            this.SetVisible_Safari(visible);
    },
    SetVisible_Safari: function (visible) {
        var mainElement = this.GetMainElement();
        var method = visible ? ASPx.Attr.RestoreStyleAttribute : ASPx.Attr.ChangeStyleAttribute;
        method(mainElement, "width", "0px");
        method(mainElement, "height", "0px");
    },
    // API
    RaiseFlashScriptCommand: function (command, args) {
        if(!this.FlashScriptCommand.IsEmpty()) {
            var eventArgs = new ASPxClientFlashScriptCommandEventArgs(command, args);
            this.FlashScriptCommand.FireEvent(this, eventArgs);
        }
    },
    /* Flash API */
    Back: function () {
        this.GetMainElement().Back();
    },
    GetVariable: function (name) {
        return this.GetMainElement().GetVariable(name);
    },
    Forward: function () {
        this.GetMainElement().Forward();
    },
    GotoFrame: function (frameNumber) {
        this.GetMainElement().GotoFrame(frameNumber);
    },
    IsPlaying: function () {
        return ASPx.Browser.IE ? this.GetMainElement().Playing : this.GetMainElement().IsPlaying();
    },
    LoadMovie: function (layerNumber, url) {
        this.GetMainElement().LoadMovie(layerNumber, url);
    },
    Pan: function (x, y, mode) {
        this.GetMainElement().Pan(x, y, mode);
    },
    PercentLoaded: function () {
        this.GetMainElement().PercentLoaded();
    },
    Play: function () {
        this.GetMainElement().Play();
    },
    Rewind: function () {
        this.GetMainElement().Rewind();
    },
    SetVariable: function (name, value) {
        this.GetMainElement().SetVariable(name, value);
    },
    SetZoomRect: function (left, top, right, bottom) {
        this.GetMainElement().SetZoomRect(left, top, right, bottom);
    },
    StopPlay: function () {
        if(ASPx.Browser.IE)
            this.GetMainElement().Stop();
        else
            this.GetMainElement().StopPlay();
    },
    TotalFrames: function () {
        return ASPx.Browser.IE ? this.GetMainElement().TotalFrames : this.GetMainElement().TotalFrames();
    },
    Zoom: function (percent) {
        this.GetMainElement().Zoom(percent);
    },
    /* QuickTime API */
    QTPlay: function () {
        this.GetMainElement().Play();
    },
    QTStopPlay: function () {
        this.GetMainElement().Stop();
    },
    QTRewind: function () {
        this.GetMainElement().Rewind();
    },
    QTStep: function (count) {
        this.GetMainElement().Step(count);
    }
});
ASPxClientObjectContainer.Cast = ASPxClientControl.Cast;
var ASPxClientFlashScriptCommandEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function (command, args) {
        this.command = command;
        this.args = args;
    }
});

window.ASPxClientObjectContainer = ASPxClientObjectContainer;
window.ASPxClientFlashScriptCommandEventArgs = ASPxClientFlashScriptCommandEventArgs;
})();