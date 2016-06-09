(function() {
////////////////////////////////////////////////////////////////////////////////
// ASPxClientSchedulerResourceNavigator
////////////////////////////////////////////////////////////////////////////////
var ASPxClientSchedulerResourceNavigator = ASPx.CreateClass(ASPx.SchedulerRelatedControlBase, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);
        this.visibleResCount = 0;
        this.savedIndex = -1;
    },
    Initialize: function () {
        this.DecorateCombo();
    },
    RegisterScriptsRestartHandler: function () {
        var func = new Function("ASPx.SchedulerResNavDecorateCombo('" + this.name + "');");
        ASPx.AddScriptsRestartHandler(this.name, func);
    },
    ProcessCallbackResult: function (id, html, params) {
        ASPx.RelatedControlManager.ProcessCallbackResultDefault(id, html, "");
        this.visibleResCount = params;
    },

    GetCombo: function () {
        return ASPx.GetControlCollection().Get(this.name + "_cmb");
    },
    DecorateCombo: function () {
        if (this.visibleResCount < 2)
            return;
        var combo = this.GetCombo();
        if (ASPx.IsExists(combo)) {
            var bag = [];
            var item;
            var startIndex = combo.GetSelectedIndex();
            if (startIndex < 0)
                startIndex = this.savedIndex;
            else
                this.savedIndex = startIndex;
            for (var i = 0; i < this.visibleResCount; i++) {
                item = combo.GetItem(startIndex + i);
                if (ASPx.IsExists(item))
                    bag.push(item.text);
            }
            if (combo.GetInputElement() != null) {
                ASPxClientDropDownEdit.prototype.SetTextBase.call(combo, bag.join(", "));//B203980
            }
        }
    },
    ExecuteCallbackCommand: function (cmdId, params) {
        var schedulerControl = ASPx.GetControlCollection().Get(this.schedulerControlId);
        if (ASPx.IsExists(schedulerControl))
            schedulerControl.RaiseCallback(cmdId + "|" + params);
    }
});
////////////////////////////////////////////////////////////////////////////////

ASPx.SchedulerResNavCmd = function(name, cmdId, params) {
    var resourceNavigator = ASPx.GetControlCollection().Get(name);
    if (ASPx.IsExists(resourceNavigator))
        resourceNavigator.ExecuteCallbackCommand(cmdId, params);
}
ASPx.SchedulerResNavDecorateCombo = function(name) {
    var resourceNavigator = ASPx.GetControlCollection().Get(name);
    if (ASPx.IsExists(resourceNavigator))
        resourceNavigator.DecorateCombo();
}

window.ASPxClientSchedulerResourceNavigator = ASPxClientSchedulerResourceNavigator;
})();