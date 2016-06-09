/// <reference path="..\_references.js"/>

(function() {

var pageControlPrefix = "FCPC";
var popupFilterControlPrefix = "DXPFC";
var applyButtonPrefix = "FCOKBTN";
var expressionTabTextPrefix = "TextTabEditor";
var textTabValidationSummaryPrefix = "TextTabValidationSummary";
var ASPxClientFilterControl = ASPx.CreateClass(ASPxClientControl, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);
        this.callBacksEnabled = true;
        this.nodeIndex = -1;
        this.editorIndex = "";
        this.prevEditorValue = null;
        this.ownerControl = null;
        this.standAlone = false;
        this.isApplied = false;
        this.delayedApply = false;
        this.userHiddenOperations = {};
        this.inPostBack = false;
        this.enableTextTab = false;
        this.enableColumnsTreeView = false;
        this.maxHierarchyDepth = 0;
        this.columnStrategy = null;
        this.sizingConfig.allowSetWidth = false;
        this.sizingConfig.allowSetHeight = false;
        this.Applied = new ASPxClientEvent();
    },
    Initialize: function(){
        ASPxClientControl.prototype.Initialize.call(this);
        this.InitializeEvents();
        this.InitializeColumnStrategy();
    },
    InitializeEvents: function() {
        if(!this.enableTextTab) return;
        var textTabEditor = this.GetTextTabEditor();
        if(textTabEditor)
            textTabEditor.LostFocus.AddHandler(this.OnTabTextEditorLostFocus.aspxBind(this));
        var validationSummary = this.GetTextTabValidationSummary();
        if(validationSummary)
            ASPx.Evt.AttachEventToElement(validationSummary, "click", this.SetTextTabFocusOnValidationSummaryClick.aspxBind(this));
    },
    InitializeColumnStrategy: function() {
        this.columnStrategy = this.enableColumnsTreeView ? new ASPxFilterControlHierarchicalColumnsStrategy(this) : new ASPxFilterControlFlatColumnsStrategy(this);
    },
    GetFilterExpression: function () { return this.stateObject.filterExpression; },
    GetAppliedFilterExpression: function () { return this.stateObject.appliedFilterExpression; },
    GetEditor: function (editorIndex) { return ASPx.GetControlCollection().Get(this.GetChildEditorsPrefix() + "_" + "DXEdit" + editorIndex.toString()); },
    IsFilterExpressionValid: function () { 
        var so = this.stateObject;
        if(!this.enableTextTab)
            return so.isFilterExpressionValid;
        return so.textTabExpressionValid && so.isFilterExpressionValid;
    },
    TextTabExpressionValid: function() {
        return this.stateObject.textTabExpressionValid;
    },
    GetChildEditorsPrefix: function() { return this.enableTextTab ? this.name + "_" + pageControlPrefix : this.name; },
    GetTextTabEditorId: function() { return this.name + "_" + pageControlPrefix + "_" + expressionTabTextPrefix; },
    GetTextTabEditor: function() { return ASPx.GetControlCollection().GetByName(this.GetTextTabEditorId()); },
    GetTextTabValidationSummary: function() { return document.getElementById(this.name + "_" + pageControlPrefix + "_" + textTabValidationSummaryPrefix); },
    GetNodeIndexByEditor: function (editorIndex) { return Math.round(editorIndex / 1000); },
    GetValueIndexByEditor: function (editorIndex) { return editorIndex % 1000; },
    GetRootTable: function () { return ASPx.GetElementById(this.name); },
    GetRootTD: function () {
        var table = this.GetRootTable();
        if (!table) return null;
        return table.rows[0].cells[0];
    },
    ShowLoadingPanel: function () {
        var mainElement = this.GetMainElement();
        if(mainElement) {
            var offsetElement = this.standAlone ? this.GetRootTD() : mainElement.parentNode;
            this.CreateLoadingPanelWithAbsolutePosition(mainElement.parentNode, offsetElement);
        }
    },
    ShowLoadingDiv: function () {
        var mainElement = this.GetMainElement();
        if(mainElement) {
            var offsetElement = this.standAlone ? this.GetRootTD() : mainElement.parentNode;
            this.CreateLoadingDiv(mainElement.parentNode, offsetElement);
        }
    },
    GetCallbackAnimationElement: function() {
        return this.GetRootTD();
    },
    SetTextTabFocusOnValidationSummaryClick: function(evt) {
        var src = ASPx.Evt.GetEventSource(evt);
        if(src.tagName === "A")
            this.GetTextTabEditor().SetFocus(); 
    },
    FilterCallback: function (action, index, params) {
        if(this.inPostBack) // Q282690
            return;
        if (!ASPx.IsExists(index)) {
            index = this.nodeIndex;
        }
        this.nodeIndex = -1;
        var args = this.SerializeFilterCallbackArgs([action, index].concat(params));
        if (!this.callBack || !this.callBacksEnabled) {
            this.inPostBack = true;
            this.SendPostBack(args);
            return;
        }
        this.ShowLoadingElements();
        this.CreateCallback(args, action);
    },
    SerializeFilterCallbackArgs: function(args) {
        if(!ASPx.IsExists(args) || args.constructor != Array || args.length == 0)
            return "";
        var result = [];
        for(var i = 0; i < args.length; i++) {
            var element = args[i].toString();
            result.push(element.length, "|", element);
        }
        return result.join("");
    },
    OnCallback: function (result) {
        this.ClearEditorsValues();
        if (this.ownerControl != null && !this.delayedApply) {
            this.ownerControl.OnCallback(result);
        } else {
            var rootTD = this.GetRootTD();
            if (rootTD != null) {
                ASPx.SetInnerHtml(rootTD, result.html);
                setTimeout(this.InitializeEvents.aspxBind(this), 0);
            }
            this.UpdateStateObjectWithObject(result.stateObject);
            if(this.enableTextTab)
                this.SetApplyButtonEnabled(this.TextTabExpressionValid());
        }
        if (this.isApplied) {
            this.RaiseFilterApplied();
        }
        this.isApplied = false;
    },
    SetApplyButtonEnabled: function(enabled) {
        var applyButtonId = this.name.replace(new RegExp("_" + popupFilterControlPrefix + "$"), "") + "_" + applyButtonPrefix;
        var applyBtn = ASPx.GetControlCollection().GetByName(applyButtonId);
        if(applyBtn)
            applyBtn.SetEnabled(enabled);       
    },
    DoEndCallback: function () {
        if (this.ownerControl != null && !this.delayedApply) {
            this.ownerControl.DoEndCallback();
            this.ownerControl = null;
        } else {
            ASPxClientControl.prototype.DoEndCallback.call(this);

            if (this.delayedApply) {
                this.delayedApply = false;
                this.Apply(this.ownerControl);
            }
        }
    },
    OnCallbackError: function (result, data) {
        this.isApplied = false;
        alert(result);
        if (this.editorIndex > -1) {
            var editor = this.GetEditor(this.editorIndex);
            editor.SetFocus();
        }
    },
    ShowPopupMenu: function (menuName, evt, index, propertyType, columnIndex) {
        if(this.CheckEditor()) return;
        this.nodeIndex = index;
        var menu = ASPx.GetControlCollection().Get(this.name + "_" + menuName);
        if(menu == null) return;
        if(propertyType && columnIndex) {
            this.CheckOperationMenuItemVisibility(menu, propertyType, columnIndex);
        }
        var element = ASPx.Evt.GetEventSource(evt);
        if(ASPx.Browser.Firefox) {
            while(element && !element.tagName)
                element = element.parentNode;
        }
        menu.ShowAtElement(element);
    },
    ShowFieldNamePopup: function (evt, index, property, showPopupParams) { 
        this.columnStrategy.ShowFieldNamePopup(evt, index, property, showPopupParams);
    },
    ShowOperationPopup: function (evt, index, propertyType, columnIndex) { this.ShowPopupMenu("OperationPopup", evt, index, propertyType, columnIndex); },
    ShowGroupPopup: function (evt, index) { this.ShowPopupMenu("GroupPopup", evt, index); },
	ShowAggregatePopup: function (evt, index, propertyType, columnIndex) { this.ShowPopupMenu("AggregatePopup", evt, index, propertyType, columnIndex); },    
    ChangeFieldName: function (fieldName, index) { this.columnStrategy.ChangeFieldName(fieldName, index) },
    ChangeOperandPropertyValue: function(index, valueIndex, fieldName) { this.columnStrategy.ChangeOperandPropertyValue(index, valueIndex, fieldName); },
    ChangeAggregateProperty: function(aggregateProperty, index) { this.columnStrategy.ChangeAggregateProperty(aggregateProperty, index); },
	ChangeAggregate: function(aggregate, index) { this.FilterCallback("Aggregate", index, this.RemoveDivider(aggregate)) },
    ChangeOperation: function (operation, index) { this.FilterCallback("Operation", index, this.RemoveDivider(operation)); },
    ChangeGroup: function (group, index) {
        if (group.indexOf("|") == 0) {
            this.FilterCallback(group.substr(1), index, "");
        } else {
            this.FilterCallback("GroupType", index, group);
        }
    },
    ApplyTextTabExpression: function(textTabExpression) { 
        if(this.textTabExpression === textTabExpression)
            return;
        this.textTabExpression = textTabExpression;
        this.FilterCallback("ApplyTextTabExpression", -1, textTabExpression); 
    },
    OnTabTextEditorLostFocus: function(s, e) {
        var textTabExpression = s.GetText();
        this.ApplyTextTabExpression(textTabExpression);
    },
    Apply: function (ownerControl) {
        if(this.enableTextTab && !this.TextTabExpressionValid())
            return;
        if (this.editorIndex > -1) {
            var editor = this.GetEditor(this.editorIndex);
            if (editor && !editor.GetIsValid())
                return;
        }
        this.ownerControl = ownerControl;
        if (this.InCallback()) {
            this.delayedApply = true;
            return;
        }
        this.isApplied = true;
        this.FilterCallback("Apply", -1, ownerControl ? "T" : "F");
    },
    Reset: function () { this.FilterCallback("Reset", -1, ""); },
    /* public void Clear() { } */
    Clear: function() { this.FilterCallback("Clear", -1, ""); },
    RemoveNode: function (index) { this.FilterCallback("Remove", index, ""); },
    AddConditionNode: function (index) { this.FilterCallback("AddCondition", index, ""); },
    ChangeOperandType: function(index, valueIndex) { this.FilterCallback("ChangeOperandType", index, valueIndex); },
    AddValue: function (index) { this.FilterCallback("AddValue", index, ""); },
    ShowEditor: function (editorIndex) {
        if (this.CheckEditor()) return;
        var editor = this.ChangeEditorVisibility(editorIndex, true);
        if (editor != null) {
            editor.SetIsValid(true);
            editor.Filter = this;
            editor.Focus();
            this.prevEditorValue = editor.GetValue();
            this.editorIndex = editorIndex;
        }
    },
    HideEditorAndRollbackValue: function () {
        if (this.editorIndex < 0) return;
        this.ChangeEditorVisibility(this.editorIndex, false);
        var editor = this.GetEditor(this.editorIndex);
        if (editor != null) {
            editor.SetValue(this.prevEditorValue);
        }
        this.ClearEditorsValues();
    },
    GetEditorsContainer: function() {
        if(this.enableTextTab)
            return ASPx.GetControlCollection().GetByName(this.name + '_' + pageControlPrefix);
        return this;
    },
    ChangeEditorVisibility: function (editorIndex, visible) {
        var link = this.GetEditorsContainer().GetChildElement("DXValue" + editorIndex);
        var editor = this.GetEditor(editorIndex);
        if (link != null && editor != null) {
            link.style.display = visible ? "none" : "";
            editor.SetVisible(visible);
            return editor;
        }
        return null;
    },
    CheckEditor: function () {
        if (this.editorIndex < 0) return false;
        var editor = this.GetEditor(this.editorIndex);
        if (editor == null) return false;
        editor.Validate();
        if (!editor.GetIsValid()) return true;
        if (editor.GetValue() == this.prevEditorValue) {
            this.HideEditorAndRollbackValue();
            return false;
        }
        var editorIndex = this.editorIndex;
        var value = editor.GetValueString();
        if (value == null) value = "";
        var params = [this.GetValueIndexByEditor(editorIndex).toString(), value];
        this.FilterCallback("Value", this.GetNodeIndexByEditor(editorIndex), params);
        return true;
    },
    ClearEditorsValues: function () {
        this.prevEditorValue = null;
        this.editorIndex = -1;
    },
    CheckOperationMenuItemVisibility: function (menu, propertyType, columnIndex) {
        for (var i = 0; i < menu.GetItemCount(); i++) {
            var item = menu.GetItem(i);
            var visible = propertyType.length > 0 && this.GetBeforeDivider(item.name).indexOf(propertyType) > -1;
            if (visible && this.userHiddenOperations[columnIndex])
                visible = ASPx.Data.ArrayIndexOf(this.userHiddenOperations[columnIndex], i) < 0;
            item.SetVisible(visible);
        }
    },
    RemoveDivider: function (str) {
        var pos = str.indexOf('|');
        if (pos < 0) return str;
        return str.substr(pos + 1);
    },
    GetBeforeDivider: function (str) {
        var pos = str.indexOf('|');
        if (pos < 0) return "";
        return str.substr(0, pos);
    },
    RaiseFilterApplied: function () {
        if (this.Applied.IsEmpty()) return;
        var args = new ASPxClientFilterAppliedEventArgs(this.GetFilterExpression());
        this.Applied.FireEvent(this, args);
    }
});
ASPxClientFilterControl.Cast = ASPxClientControl.Cast;

var ASPxFilterControlFlatColumnsStrategy = ASPx.CreateClass(null, {
    constructor: function(filterControl) {
        this.fc = filterControl;
    },
    GetFieldNamePopupName: function() { return "FieldNamePopup"; },
    GetFieldNameControl: function() { return this.GetFieldNamePopup(); },
    GetFieldNamePopup: function() {
        var fieldNameControlName = this.fc.name + '_' + this.GetFieldNamePopupName();
        return ASPx.GetControlCollection().GetByName(fieldNameControlName);
    },
    ChangeFieldName: function(fieldName, index) { this.fc.FilterCallback("FieldName", index, fieldName); },
    ChangeOperandPropertyValue: function(index, valueIndex, fieldName) { this.fc.FilterCallback('ChangeOperandPropertyValue', index, [valueIndex, fieldName]); },
    ChangeAggregateProperty: function(aggregateProperty, index) { this.fc.FilterCallback("AggregateProperty", index, aggregateProperty); },
    GetItemClickEvent: function(fieldNameControl) { return fieldNameControl.ItemClick; },
    GetItemName: function(e) { return e.item.name; },
    ChangeItemClickHandler: function(fieldNameControl, itemClickHandler) {
        this.GetItemClickEvent(fieldNameControl).ClearHandlers();
        this.GetItemClickEvent(fieldNameControl).AddHandler(function(s, e) { itemClickHandler(this.fc.name, this.GetItemName(e)); }.aspxBind(this));
    },
    ShowFieldNamePopup: function(evt, index, property, showPopupParams) {
        var fieldNameControl = this.GetFieldNameControl(showPopupParams.SubMenuKey);
        this.SetFieldNamePopupItemsEnabled(showPopupParams);
        this.ChangeItemClickHandler(fieldNameControl, eval(showPopupParams.ItemClickHandler));
        this.fc.ShowPopupMenu(this.GetFieldNamePopupName(), evt, index, undefined, undefined, showPopupParams.SubMenuKey); 
    },
    SetFieldNamePopupItemsEnabled: function(showPopupParams) {}
});

var ASPxFilterControlHierarchicalColumnsStrategy = ASPx.CreateClass(ASPxFilterControlFlatColumnsStrategy, {
    constructor: function(filterControl) {
        this.constructor.prototype.constructor.call(this, filterControl);
        this.currentView = null;
    },
    GetFieldNamePopupName: function() { return "FieldNameTreeViewPopup"; },
    GetFieldNameControl: function(subMenuKey) { 
        this.SetCorrectFieldMenuVisible(this.GetFieldNamePopupName(), subMenuKey);
        return this.currentView;
    },
    ChangeFieldName: function (fieldName, index) { 
        this.GetFieldNamePopup().SetVisible(false);
        ASPxFilterControlFlatColumnsStrategy.prototype.ChangeFieldName.call(this, fieldName, index);
    },
    ChangeOperandPropertyValue: function(index, valueIndex, fieldName) {
        this.GetFieldNamePopup().SetVisible(false);
        ASPxFilterControlFlatColumnsStrategy.prototype.ChangeOperandPropertyValue.call(this, index, valueIndex, fieldName);
    },
    ChangeAggregateProperty: function(aggregateProperty, index) {
        this.GetFieldNamePopup().SetVisible(false);
        ASPxFilterControlFlatColumnsStrategy.prototype.ChangeAggregateProperty.call(this, aggregateProperty, index);
    },
    GetItemClickEvent: function(fieldNameControl) { return fieldNameControl.NodeClick; },
    GetItemName: function(e) { return e.node.name; },
    ShowFieldNamePopup: function(evt, index, property, showPopupParams) {
        ASPxFilterControlFlatColumnsStrategy.prototype.ShowFieldNamePopup.call(this, evt, index, property, showPopupParams);
        this.ExpandAndSelectNode(this.currentView, property);
    },
    SetCorrectFieldMenuVisible: function(menuName, property) {
        var popupMenu = ASPx.GetControlCollection().Get(this.fc.name + "_" + menuName);
        if(!popupMenu.views)
            return;
        for(var i = 0; i < popupMenu.views.length; i++) {
            var treeView = ASPx.GetControlCollection().GetByName(popupMenu.views[i].treeViewId);
            var visible = popupMenu.views[i].key == property
            treeView.SetClientVisible(visible);
            if(visible)
                this.currentView = treeView;
        }
    },
    ExpandAndSelectNode: function(treeView, name) {
        if(!name || !treeView.GetNodeByName(name)) return;
        var node = treeView.GetNodeByName(name);
        var parent = node.parent;
        while(parent != null) {
            parent.SetExpanded(true);
            parent = parent.parent
        }
        treeView.SetSelectedNode(node);
    },
    SetFieldNamePopupItemsEnabled: function(showPopupParams) {
        var view = this.GetFieldNameControl(showPopupParams.SubMenuKey);
        for(var i = 0; i < view.GetNodeCount(); i++)
            this.SetFieldNamePopupItemEnabled(view.GetNode(i), showPopupParams.SubMenuDepthLevel, 0);
    },
    SetFieldNamePopupItemEnabled: function(node, startDepth, nodeDepth) {
        var enabled = nodeDepth + startDepth <= this.fc.maxHierarchyDepth;
        node.SetEnabled(enabled);
        if(!enabled) return;
        for(var i = 0; i < node.GetNodeCount(); i++) {
            this.SetFieldNamePopupItemEnabled(node.GetNode(i), startDepth, nodeDepth + 1);
        }
    }
});
var ASPxClientFilterAppliedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
	constructor: function(filterExpression){
	    this.constructor.prototype.constructor.call(this);
        this.filterExpression = filterExpression;
    }
});

ASPx.FCShowFieldNamePopup = function(name, evt, index, property, showPopupParams) {
    var control = ASPx.GetControlCollection().Get(name); 
    if(control != null) {
        showPopupParams = ASPx.Json.Eval(showPopupParams);
        control.ShowFieldNamePopup(evt, index, property, showPopupParams);
    }
}
ASPx.FCShowOperationPopup = function(name, evt, index, propertyType, columnIndex) {
    var control = ASPx.GetControlCollection().Get(name); 
    if(control != null) {
        control.ShowOperationPopup(evt, index, propertyType, columnIndex);
    }
}
ASPx.FCShowAggregatePopup = function(name, evt, index, propertyType, columnIndex) {
    var control = ASPx.GetControlCollection().Get(name); 
    if(control != null) {
        control.ShowAggregatePopup(evt, index, propertyType, columnIndex);
    }
}
ASPx.FCShowGroupPopup = function(name, evt, index) {
    var control = ASPx.GetControlCollection().Get(name); 
    if(control != null) {
        control.ShowGroupPopup(evt, index);
    }
}
ASPx.FCChangeFieldName = function(name, fieldName) {
    var control = ASPx.GetControlCollection().Get(name); 
    if(control != null) {
        control.ChangeFieldName(fieldName);
    }
}
ASPx.FCPopupTreeViewCollapseAll = function(s, e) {
    for(var i = 0; i < s.views.length; i++) {
        var treeView = ASPx.GetControlCollection().GetByName(s.views[i].treeViewId);
        treeView.CollapseAll();
    }
}
ASPx.FCChangeOperation = function(name, item) {
    var operation = item.name;
    var hasSubMenu = item.GetItemCount() > 0;
    var control = ASPx.GetControlCollection().Get(name); 
    if(control != null && !hasSubMenu) {
        control.ChangeOperation(operation);
    }
}
ASPx.FCChangeAggregate = function(name, aggregate) {
    var control = ASPx.GetControlCollection().Get(name);
    if(control != null) {
        control.ChangeAggregate(aggregate);
    }
}
ASPx.FCChangeAggregateProperty = function(name, propertyName) {
    var control = ASPx.GetControlCollection().Get(name);
    if(control != null) {
        control.ChangeAggregateProperty(propertyName);
    }
}
ASPx.FCAddValue = function(name, index) {
    var control = ASPx.GetControlCollection().Get(name); 
    if(control != null) {
        control.AddValue(index);
    }
}
ASPx.FCChangeGroup = function(name, group) {
    var control = ASPx.GetControlCollection().Get(name); 
    if(control != null) {
        control.ChangeGroup(group);
    }
}
ASPx.FCRemoveNode = function(name, index) {
    var control = ASPx.GetControlCollection().Get(name); 
    if(control != null) {
        control.RemoveNode(index);
    }
}
ASPx.FCAddConditionNode = function(name, index) {
    var control = ASPx.GetControlCollection().Get(name); 
    if(control != null) {
        control.AddConditionNode(index);
    }
}
ASPx.FCNodeValueClick = function(name, index) {
    var control = ASPx.GetControlCollection().Get(name); 
    if(control != null) {
        control.ShowEditor(index);
    }
}
ASPx.FCChangeOperandType = function(name, index, valueIndex) {
    var control = ASPx.GetControlCollection().Get(name);
    if(control != null) {
        control.ChangeOperandType(index, valueIndex);
    }
}
ASPx.FCChangeOperandPropertyValue = function(name, index, valueIndex, fieldName) {
    var control = ASPx.GetControlCollection().Get(name);
    if(control != null) {
        control.ChangeOperandPropertyValue(index, valueIndex, fieldName);
    }
}
ASPx.FCEditorKeyDown = function(s, e) {
    var keyCode = ASPx.Evt.GetKeyCode(e.htmlEvent);
	if(keyCode == ASPx.Key.Enter)
		ASPx.Evt.PreventEventAndBubble(e.htmlEvent); // prevent form submission
}
ASPx.FCEditorKeyUp = function(s, e) {
	var filter = s.Filter;
	if(!filter) return;
    var keyCode = ASPx.Evt.GetKeyCode(e.htmlEvent);
    if(keyCode == ASPx.Key.Enter) {
		filter.CheckEditor();
    } else if(keyCode == ASPx.Key.Esc) {
        filter.HideEditorAndRollbackValue();
    }
}
ASPx.FCEditorLostFocus = function(s, e) {
	if(s.Filter)
        s.Filter.CheckEditor();
}

ASPx.FCPopupInit = function(s, e) { s.Show(); }
ASPx.FCPopupShown = function(s, e) { s.GetWindowContentElement(-1).style.height = 0; } // ask Serge S

window.ASPxClientFilterControl = ASPxClientFilterControl;
window.ASPxClientFilterAppliedEventArgs = ASPxClientFilterAppliedEventArgs;
})();