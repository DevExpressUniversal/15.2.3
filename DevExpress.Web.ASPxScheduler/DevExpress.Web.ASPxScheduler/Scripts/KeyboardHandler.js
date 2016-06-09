(function () {

var ClientSchedulerGlobalKeyboardHandler = ASPx.CreateClass(null, {
    constructor: function () {
        this.keyboardHandlers = [];
    },
    AddHandler: function (handler) {
        this.keyboardHandlers.push(handler);
    },
    RemoveHandler: function (handler) {
        var indx = -1;
        for (var i in this.keyboardHandlers)
            if (this.keyboardHandlers[i] == handler) {
                indx = i;
                break;
            }
        if (indx == -1)
            return;
        this.keyboardHandlers.splice(indx, 1);
    },
    OnDocumentKeyDown: function (e) {
        for (var i in this.keyboardHandlers)
            this.keyboardHandlers[i].OnDocumentKeyDown(e);
    }
});

ASPx.schedulerGlobalKeyboardHandler = new ClientSchedulerGlobalKeyboardHandler();

ASPx.Evt.AttachEventToDocument("keydown", function (e) {
    if (ASPx.schedulerGlobalKeyboardHandler != null)
        ASPx.schedulerGlobalKeyboardHandler.OnDocumentKeyDown(e);
});

var ClientSchedulerKeyboardHandler = ASPx.CreateClass(null, {
    constructor: function (scheduler) {
        this.scheduler = scheduler;
    },
    OnDocumentKeyDown: function (e) {
        if (!this.scheduler.mouseHandler)
            return;
        if (e.keyCode == 27)
            this.scheduler.mouseHandler.SwitchToDefaultState();
        return true;
    }
});

ASPx.ClientSchedulerKeyboardHandler = ClientSchedulerKeyboardHandler;

})();