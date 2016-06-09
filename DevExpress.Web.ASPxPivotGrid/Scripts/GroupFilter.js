(function() {
var PivotGroupFilterTree = ASPx.CreateClass(ASPxClientTreeView, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.pivotGrid = null;
        this.callbackCount = 0;

        var instance = this;
        this.CheckedChanged.AddHandler(function(s, e) { instance.HandleCheckedChanged(e.node); });
        this.ExpandedChanged.AddHandler(function(s, e) { instance.HandleExpandedChanged(e.node); });
    },
    getFullNodeIndex: function(node) {
        var index = 0, currentIndex = node.index;
        var parentNode = node.parent;
        while(true) {
            if(parentNode == null)
                parentNode = this.rootNode;
            for(var i = currentIndex - 1; i >= 0; i--) {
                var childNode = parentNode.GetNode(i);
                index += this.getRecursiveNodeCount(childNode) + 1;
            }
            if(parentNode == this.rootNode) break;
            index++;
            currentIndex = parentNode.index;
            parentNode = parentNode.parent;
        }
        return index;
    },
    getRecursiveNodeCount: function(node) {
        if(node == null) return 0;
        var count = node.GetNodeCount();
        for(var i = 0; i < count; i++) {
            var childNode = node.GetNode(i);
            count += this.getRecursiveNodeCount(childNode);
        }
        return count;
    },
    getCheckState: function(node) {
        if(node.GetCheckState() == ASPx.CheckBoxCheckState.Indeterminate)
            return null;
        return node.GetCheckState() == ASPx.CheckBoxCheckState.Checked;
    },
    getShowAllCheckState: function() {
        var nodesCount = 0, checkCount = 0, count = this.GetNodeCount();
        for(var i = 0; i < count; i++) {
            var node = this.GetNode(i);
            if(this.isShowAllNode(node)) continue;
            nodesCount++;
            var checkState = node.GetCheckState();
            switch(checkState) {
                case ASPx.CheckBoxCheckState.Indeterminate:
                    return checkState;
                case ASPx.CheckBoxCheckState.Checked:
                    checkCount++;
                    break;
                case ASPx.CheckBoxCheckState.Unchecked:
                    if(checkCount > 0)
                        return ASPx.CheckBoxCheckState.Indeterminate;
                    break;
            }
        }
        if(checkCount == 0) return ASPx.CheckBoxCheckState.Unchecked;
        return checkCount == nodesCount ? ASPx.CheckBoxCheckState.Checked : ASPx.CheckBoxCheckState.Indeterminate;
    },
    isShowAllNode: function(node) {
        return (node != null) && (node.name == "All");
    },
    updateShowAllCheckState: function() {
        var checkState = this.getShowAllCheckState();
        this.rootNode.nodes[0].SetCheckState(checkState);
    },
    hideNodeExpandCollapseButton: function(clientNode) {
        var listItem = this.GetNodeListItem(clientNode);
        var button = ASPx.GetNodesByPartialClassName(listItem, this.ButtonCssClassName)[0];
        if(button != null)
            button.style.display = "none";
    },
    CheckAll: function(checked) {
        var count = this.rootNode.nodes.length;
        for(var i = 0; i < count; i++) {
            var node = this.rootNode.nodes[i];
            if(this.isShowAllNode(node)) continue;
            node.SetChecked(checked);
        }
        this.pivotGrid.SelectAllFilterValues(checked);
        this.pivotGrid.UpdateFilterButtons();
    },
    HandleCheckedChanged: function(node) {
        if(this.isShowAllNode(node)) {
            this.CheckAll(node.GetChecked());
            return;
        }
        var parentNode = node;
        while(parentNode != null) {
            var index = this.getFullNodeIndex(parentNode),
                count = (parentNode != node) ? 1 : this.getRecursiveNodeCount(parentNode) + 1;
            this.pivotGrid.UpdateFilterValues(index - 1, count, this.getCheckState(parentNode)); // ShowAll index excluded
            parentNode = parentNode.parent;
        }
        this.updateShowAllCheckState();
        this.pivotGrid.UpdateFilterButtons();
    },
    HandleExpandedChanged: function(node) {
        if(node.GetNodeCount() == 0)
            this.hideNodeExpandCollapseButton(node);
    },
    CreateCallback: function(callbackString) {
        if(this.pivotGrid != null) {
            this.pivotGrid.SendTreeViewCallback(this, callbackString);
            this.callbackCount++;
        } else {
            pivotGrid_WasNotFound();
        }
    },
    OnCallback: function(resultObj) {
        if(this.callbackCount > 0)
            this.callbackCount--;
        if(this.callbackCount <= 0)
            this.pivotGrid.ClearCallbackOwner();
        ASPxClientTreeView.prototype.OnCallback.apply(this, arguments);
    },
    InitPivotGridCallbacks: function(pg) {
        this.pivotGrid = pg;
    }
});

ASPx.PivotGroupFilterTree = PivotGroupFilterTree;
})();