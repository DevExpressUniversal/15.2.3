// API

(function() {
var ASPxClientDashboardViewer = ASPx.CreateClass(ASPxClientControl, {
    constructor: function (name) {
        var self = this;
        this.constructor.prototype.constructor.call(this, name);
        this.$root = $('#' + this.name);
        $.holdReady(true);
        this.serviceProxy = this.createServiceProxy();
        this.serviceProxy.requestSent.add(function (args) {
            var jsonArgs = ASPx.Json.ToJson(args);
            if (args.Task == "Export")
                self.SendPostBack(jsonArgs);
            else
                self.CreateCallback(jsonArgs);
        });
        this.ActionAvailabilityChanged = new ASPxClientEvent();
        this.MasterFilterSet = new ASPxClientEvent();
        this.MasterFilterCleared = new ASPxClientEvent();
        this.DrillDownPerformed = new ASPxClientEvent();
        this.DrillUpPerformed = new ASPxClientEvent();
        this.Loaded = new ASPxClientEvent();
        this.ItemClick = new ASPxClientEvent();
        this.ItemVisualInteractivity = new ASPxClientEvent();
        this.ItemWidgetCreated = new ASPxClientEvent();
        this.ItemWidgetUpdating = new ASPxClientEvent();
        this.ItemWidgetUpdated = new ASPxClientEvent();
        this.ItemBeforeWidgetDisposed = new ASPxClientEvent();// obsolete since v15.2
        this.ItemSelectionChanged = new ASPxClientEvent();
        this.ItemElementCustomColor = new ASPxClientEvent();
    },
    Initialize: function () {
        ASPxClientControl.prototype.Initialize.call(this);
        var self = this,
            options = $.extend(true, {}, self.initOptions, {
                service: self.serviceProxy
            });
        self.$root.dxDashboardViewer(options);
        self.dashboardViewer = self.$root.dxDashboardViewer('instance');
        self.dashboardViewer.actionAvailabilityChanged.add($.proxy(self.OnActionAvailabilityChanged, self));
        self.dashboardViewer.actionPerformed.add($.proxy(self.OnActionPerformed, self));
        self.dashboardViewer.initialized.add($.proxy(self.OnLoaded, self));
        self.dashboardViewer.itemClick.add($.proxy(self.OnItemClick, self));
        self.dashboardViewer.itemHover.add($.proxy(self.OnItemHover, self));
        self.dashboardViewer.itemVisualInteractivity.add($.proxy(self.OnItemVisualInteractivity, self));
        self.dashboardViewer.itemWidgetCreated.add($.proxy(self.OnItemWidgetCreated, self));
        self.dashboardViewer.itemWidgetUpdating.add($.proxy(self.OnItemWidgetUpdating, self));
        self.dashboardViewer.itemWidgetUpdated.add($.proxy(self.OnItemWidgetUpdated, self));
        self.dashboardViewer.itemSelectionChanged.add($.proxy(self.OnItemSelectionChanged, self));
        self.dashboardViewer.itemElementCustomColor.add($.proxy(self.OnItemElementCustomColor, self));
		$.holdReady(false);
    },
    OnLoaded: function (args) {
        if (!this.Loaded.IsEmpty()) {
            var newArgs = new ASPxClientEventArgs(args);
            this.Loaded.FireEvent(this, newArgs);
        }
    },
    OnItemClick: function (args) {
        if (!this.ItemClick.IsEmpty()) {
            var newArgs = new ASPxClientDashboardItemClickEventArgs(args);
            this.ItemClick.FireEvent(this, newArgs);
        }
    },
    OnItemVisualInteractivity: function (args) {
        if (!this.ItemVisualInteractivity.IsEmpty()) {
            var newArgs = new ASPxClientDashboardItemVisualInteractivityEventArgs(args);
            this.ItemVisualInteractivity.FireEvent(this, newArgs);
        }
    },
    OnItemWidgetCreated: function (args) {
        if (!this.ItemWidgetCreated.IsEmpty()) {
            var newArgs = new ASPxClientDashboardItemWidgetEventArgs(args);
            this.ItemWidgetCreated.FireEvent(this, newArgs);
        }
    },
    OnItemWidgetUpdating: function (args) {
        if (!this.ItemWidgetUpdating.IsEmpty()) {
            var newArgs = new ASPxClientDashboardItemWidgetEventArgs(args);
            this.ItemWidgetUpdating.FireEvent(this, newArgs);
        }
    },
    OnItemWidgetUpdated: function (args) {
        if (!this.ItemWidgetUpdated.IsEmpty()) {
            var newArgs = new ASPxClientDashboardItemWidgetEventArgs(args);
            this.ItemWidgetUpdated.FireEvent(this, newArgs);
        }
    },
	OnItemSelectionChanged: function (args) {
        if (!this.ItemSelectionChanged.IsEmpty()) {
            var newArgs = new ASPxClientDashboardItemSelectionChangedEventArgs(args);
            this.ItemSelectionChanged.FireEvent(this, newArgs);
        }
	},
	OnItemElementCustomColor: function (args) {
	    if (!this.ItemElementCustomColor.IsEmpty()) {
	        var newArgs = new ASPxClientDashboardItemElementCustomColorEventArgs(args);
	        this.ItemElementCustomColor.FireEvent(this, newArgs);
	    }
	},
    OnActionAvailabilityChanged: function (args) {
        if (!this.ActionAvailabilityChanged.IsEmpty()) {
            var newArgs = new ASPxClientDashboardActionAvailabilityChangedEventArgs(args);
            this.ActionAvailabilityChanged.FireEvent(this, newArgs);
        }
    },
    OnActionPerformed: function (args) {
        var self = this;
        switch (args.ActionName) {
            case 'SetMasterFilter':
                self.RaiseMasterFilterSet(args);
                break;
            case 'ClearMasterFilter':
                self.RaiseMasterFilterCleared(args);
                break;
            case 'DrillDown':
                self.RaiseDrillDownPerformed(args);
                break;
            case 'DrillUp':
                self.RaiseDrillUpPerformed(args);
                break;
        }
    },
    RaiseMasterFilterSet: function (args) {
        if (!this.MasterFilterSet.IsEmpty()) {
            var newArgs = new ASPxClientDashboardMasterFilterSetEventArgs(args);
            this.MasterFilterSet.FireEvent(this, newArgs);
        }
    },
    RaiseMasterFilterCleared: function (args) {
        if (!this.MasterFilterCleared.IsEmpty()) {
            var newArgs = new ASPxClientDashboardMasterFilterClearedEventArgs(args);
            this.MasterFilterCleared.FireEvent(this, newArgs);
        }
    },
    RaiseDrillDownPerformed: function (args) {
        if (!this.DrillDownPerformed.IsEmpty()) {
            var newArgs = new ASPxClientDashboardDrillDownPerformedEventArgs(args);
            this.DrillDownPerformed.FireEvent(this, newArgs);
        }
    },
    RaiseDrillUpPerformed: function (args) {
        if (!this.DrillUpPerformed.IsEmpty()) {
            var newArgs = new ASPxClientDashboardDrillUpPerformedEventArgs(args);
            this.DrillUpPerformed.FireEvent(this, newArgs);
        }
    },
    IsActionAvailable: function (itemName, action) {
        var actions = this.dashboardViewer.getAvailableActions().actions[itemName];
        return ($.inArray(action, actions) !== -1);
    },
    OnCallback: function (result) {
        this.serviceProxy.receiveResponse(eval(result));
    },
    AdjustControlCore: function () {
        if (this.dashboardViewer) {
            this.dashboardViewer.updateSize(true);
        }
    },
    createServiceProxy: function () {
        var _responseReceived = new $.Callbacks();
        var _requestSent = new $.Callbacks();
        return {
            responseReceived: _responseReceived,
            sendRequest: function (args) {
                _requestSent.fire(args);
            },
            requestSent: _requestSent,
            receiveResponse: function (args) {
                _responseReceived.fire(args);
            }
        };
    }
});
ASPxClientDashboardViewer.Cast = ASPxClientControl.Cast;
ASPxClientDashboardViewer.prototype.ReloadData = function () {
    this.dashboardViewer.reloadData();
};
ASPxClientDashboardViewer.prototype.ReloadData = function (parameters) {
    this.dashboardViewer.reloadData(parameters);
};
ASPxClientDashboardViewer.prototype.GetParameters = function () {
    return new ASPxClientDashboardParameters(this.dashboardViewer.getParametersCollection());
};
ASPxClientDashboardViewer.prototype.BeginUpdateParameters = function () {
    this.dashboardViewer.beginUpdateParameters();
};
ASPxClientDashboardViewer.prototype.EndUpdateParameters = function () {
    this.dashboardViewer.endUpdateParameters();
};
ASPxClientDashboardViewer.prototype.GetCurrentRange = function (itemName) {
    return new ASPxClientDashboardRangeFilterSelection(this.dashboardViewer.getCurrentRange(itemName));
};
ASPxClientDashboardViewer.prototype.GetEntireRange = function (itemName) {
    return new ASPxClientDashboardRangeFilterSelection(this.dashboardViewer.getEntireRange(itemName));
};
ASPxClientDashboardViewer.prototype.SetRange = function (itemName, range) {
    this.dashboardViewer.setRange(itemName, { startValue: range.Minimum, endValue: range.Maximum });
};
ASPxClientDashboardViewer.prototype.GetAvailableDrillDownValues = function (itemName) {
    return this.dashboardViewer.getAvailableDrillDownValues(itemName);
};
ASPxClientDashboardViewer.prototype.GetCurrentDrillDownValues = function (itemName) {
    return this.dashboardViewer.getCurrentDrillDownValues(itemName);
};
ASPxClientDashboardViewer.prototype.GetAvailableFilterValues = function (itemName) {
    return this.dashboardViewer.getAvailableFilterValues(itemName);
};
ASPxClientDashboardViewer.prototype.GetCurrentFilterValues = function (itemName) {
    return this.dashboardViewer.getCurrentFilterValues(itemName);
};
ASPxClientDashboardViewer.prototype.GetCurrentSelection = function (itemName) {
    return this.dashboardViewer.getCurrentSelection(itemName);
};
ASPxClientDashboardViewer.prototype.RequestUnderlyingData = function (itemName, args, onCompleted) {
    var baseArgs = {
        dataMembers: args.DataMembers,
        axisPoints: args.AxisPoints,
        valuesByAxisName: args.ValuesByAxisName,
        uniqueValuesByAxisName: args.UniqueValuesByAxisName
    };
    this.dashboardViewer.requestUnderlyingData(itemName, baseArgs, function (data) {
        if (onCompleted)
            onCompleted(new ASPxClientDashboardItemUnderlyingData(data));
    });
};
ASPxClientDashboardViewer.prototype.ShowParametersDialog = function () {
    this.dashboardViewer.showParametersDialog();
};
ASPxClientDashboardViewer.prototype.HideParametersDialog = function () {
    this.dashboardViewer.hideParametersDialog();
};
ASPxClientDashboardViewer.prototype.GetExportOptions = function () {
    return this.dashboardViewer.documentOptions();
};
ASPxClientDashboardViewer.prototype.SetExportOptions = function (options) {
    this.dashboardViewer.documentOptions(options);
};
ASPxClientDashboardViewer.prototype.ExportToPdf = function () {
    this.dashboardViewer.exportTo('PDF');
};
ASPxClientDashboardViewer.prototype.ExportToPdf = function (options) {
    this.dashboardViewer.exportTo('PDF', options);
};
ASPxClientDashboardViewer.prototype.ExportToImage = function () {
    this.dashboardViewer.exportTo('Image');
};
ASPxClientDashboardViewer.prototype.ExportToImage = function (options) {
    this.dashboardViewer.exportTo('Image', options);
};
ASPxClientDashboardViewer.prototype.GetWidth = function () {
    return this.$root.width();
};
ASPxClientDashboardViewer.prototype.GetHeight = function () {
    return this.$root.height();
};
ASPxClientDashboardViewer.prototype.SetWidth = function (width) {
    this.$root.width(width);
    this.dashboardViewer.updateSize();
};
ASPxClientDashboardViewer.prototype.SetHeight = function (height) {
    this.$root.height(height);
    this.dashboardViewer.updateSize();
};
ASPxClientDashboardViewer.prototype.SetSize = function (width, height) {
    this.$root.width(width);
    this.$root.height(height);
    this.dashboardViewer.updateSize();
};
ASPxClientDashboardViewer.prototype.SetMasterFilter = function (itemName, values) {
    this.dashboardViewer.performInteractivityAction({
        ItemName: itemName,
        Parameters: values,
        ActionName: 'SetMasterFilter'
    });
};
ASPxClientDashboardViewer.prototype.PerformDrillDown = function (itemName, value) {
    this.dashboardViewer.performInteractivityAction({
        ItemName: itemName,
        Parameters: value,
        ActionName: 'DrillDown'
    });
};
ASPxClientDashboardViewer.prototype.ClearMasterFilter = function (itemName) {
    this.dashboardViewer.performInteractivityAction({
        ItemName: itemName,
        ActionName: 'ClearMasterFilter'
    });
};
ASPxClientDashboardViewer.prototype.PerformDrillUp = function (itemName) {
    this.dashboardViewer.performInteractivityAction({
        ItemName: itemName,
        ActionName: 'DrillUp'
    });
};
ASPxClientDashboardViewer.prototype.CanSetMasterFilter = function (itemName) {
    return this.IsActionAvailable(itemName, 'SetMasterFilter');
};
ASPxClientDashboardViewer.prototype.CanClearMasterFilter = function (itemName) {
    return this.IsActionAvailable(itemName, 'ClearMasterFilter');
};
ASPxClientDashboardViewer.prototype.CanPerformDrillDown = function (itemName) {
    return this.IsActionAvailable(itemName, 'DrillDown');
};
ASPxClientDashboardViewer.prototype.CanPerformDrillUp = function (itemName) {
    return this.IsActionAvailable(itemName, 'DrillUp');
};
ASPxClientDashboardViewer.prototype.GetItemData = function (itemName) {
    var itemData = this.dashboardViewer.getItemData(itemName);
    return itemData ? new ASPxClientDashboardItemData(itemData) : null;
};
var ASPxClientDashboardRangeFilterSelection = ASPx.CreateClass(null, {
    constructor: function (args) {
        this.constructor.prototype.constructor.call(this);
        this.Maximum = args ? args.maximum : null;
        this.Minimum = args ? args.minimum : null;
    }
});
var ASPxClientDashboardParameters = ASPx.CreateClass(null, {
    constructor: function (args) {
        this.constructor.prototype.constructor.call(this);
        this.GetParameterList = ASPx.ArrayFunctionWrapper(args.getParameters, args, ASPxClientDashboardParameter);
        this.GetParameterByName = ASPx.FunctionWrapper(args.getParameterByName, args, ASPxClientDashboardParameter);
        this.GetParameterByIndex = ASPx.FunctionWrapper(args.getParameterByIndex, args, ASPxClientDashboardParameter);
    }
});
var ASPxClientDashboardParameter = ASPx.CreateClass(null, {
    constructor: function (args) {
        this.constructor.prototype.constructor.call(this);
        this.Name = args.getName();
        this.Value = args.getValue();
        this.GetName = ASPx.FunctionWrapper(args.getName, args);
        this.GetValue = ASPx.FunctionWrapper(args.getValue, args);
        this.SetValue = ASPx.FunctionWrapper(args.setValue, args);
        this.GetDefaultValue = ASPx.FunctionWrapper(args.getDefaultValue, args);
        this.GetDescription = ASPx.FunctionWrapper(args.getDescription, args);
        this.GetType = ASPx.FunctionWrapper(args.getType, args);
        this.GetValues = ASPx.ArrayFunctionWrapper(args.getValues, args, ASPxClientDashboardParameterValue);
    }
});
var ASPxClientDashboardParameterValue = ASPx.CreateClass(null, {
    constructor: function (args) {
        this.constructor.prototype.constructor.call(this);
        this.GetDisplayText = ASPx.FunctionWrapper(args.getDisplayText, args);
        this.GetValue = ASPx.FunctionWrapper(args.getValue, args);
    }
});
var ASPxClientDashboardActionAvailabilityChangedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(args) {
        var self = this;
        this.constructor.prototype.constructor.call(this);
        this.IsReloadDataAvailable = args.reloadData;
        this.ItemActions = [];
        $.each(args.actions, function (itemName, actions){
            self.ItemActions.push(new ASPxClientDashboardItemAction(itemName, actions));
        });
    }
});
var ASPxClientDashboardItemAction = ASPx.CreateClass(null, {
    constructor: function(itemName, actions) {
        this.ItemName = itemName;
        this.Actions = actions;
    }
});
var ASPxClientDashboardMasterFilterSetEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(args) {
        this.constructor.prototype.constructor.call(this);
        this.ItemName = args.ItemName;
        this.Values = args.Parameters;
    },
    IsNullValue: function (value) {
        return DashboardSpecialValues.IsNullValue(value);
    },
    IsOthersValue: function (value) {
        return DashboardSpecialValues.IsOthersValue(value);
    }
});
var ASPxClientDashboardMasterFilterClearedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(args) {
        this.constructor.prototype.constructor.call(this);
        this.ItemName = args.ItemName;
    }
});
var ASPxClientDashboardDrillDownPerformedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(args) {
        this.constructor.prototype.constructor.call(this);
        this.ItemName = args.ItemName;
        this.Values = args.Parameters;
    },
    IsNullValue: function (value) {
        return DashboardSpecialValues.IsNullValue(value);
    },
    IsOthersValue: function (value) {
        return DashboardSpecialValues.IsOthersValue(value);
    }
});
var ASPxClientDashboardDrillUpPerformedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function(args) {
        this.constructor.prototype.constructor.call(this);
        this.ItemName = args.ItemName;
    }
});
var DashboardSelectionMode = {
    None: DevExpress.dashboard.dashboardSelectionMode.none,
    Single: DevExpress.dashboard.dashboardSelectionMode.single,
    Multiple: DevExpress.dashboard.dashboardSelectionMode.multiple
};
var DashboardSpecialValues = {
    NullValue: "D86D8A6C-0D87-4CA4-9C15-3356A83699B5",
    OthersValue: "5821CCA5-303B-425D-909F-B8373FB7FAE3",
    OlapNullValue: "764E2930-72BE-4464-ACB6-4ADB205BD414",
    IsNullValue: function (value) {
        return (value == this.NullValue);
    },
    IsOlapNullValue: function (value) {
        return (value == this.OlapNullValue);
    },
    IsOthersValue: function (value) {
        return (value == this.OthersValue);
    }
};

window.ASPxClientDashboardViewer = ASPxClientDashboardViewer;
window.ASPxClientDashboardParameters = ASPxClientDashboardParameters;
window.ASPxClientDashboardParameter = ASPxClientDashboardParameter;
window.ASPxClientDashboardParameterValue = ASPxClientDashboardParameterValue;
window.ASPxClientDashboardRangeFilterSelection = ASPxClientDashboardRangeFilterSelection;

window.ASPxClientDashboardActionAvailabilityChangedEventArgs = ASPxClientDashboardActionAvailabilityChangedEventArgs;
window.ASPxClientDashboardItemAction = ASPxClientDashboardItemAction;
window.ASPxClientDashboardMasterFilterSetEventArgs = ASPxClientDashboardMasterFilterSetEventArgs;
window.ASPxClientDashboardMasterFilterClearedEventArgs = ASPxClientDashboardMasterFilterClearedEventArgs;
window.ASPxClientDashboardDrillDownPerformedEventArgs = ASPxClientDashboardDrillDownPerformedEventArgs;
window.ASPxClientDashboardDrillUpPerformedEventArgs = ASPxClientDashboardDrillUpPerformedEventArgs;
window.DashboardSelectionMode = DashboardSelectionMode;
window.DashboardSpecialValues = DashboardSpecialValues;
})();