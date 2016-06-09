(function() {
var CustomizationTreeView = ASPx.CreateClass(ASPxClientTreeView, {
    constructor: function(name) {
        var instance = this;
        this.constructor.prototype.constructor.call(this, name);
        this.pivotGrid = null;

        this.ExpandedChanging.AddHandler(function(s, e) {
            var isExpanded, from, to;
            isExpanded = e.node.GetExpanded();
            from = isExpanded ? instance.folderOpenClass : instance.folderClosedClass;
            to = isExpanded ? instance.folderClosedClass : instance.folderOpenClass;
            instance.changeClass(e.node.GetHtmlElement(), from, to);
        });
    },
    changeClass: function(elem, from, to) {
        var image = ASPx.GetNodeByTagName(elem, 'IMG', 0);
        image.className = image.className.replace(from, to);
    },
    InitializePivotGrid: function(pivotGrid, deferred) {
        this.pivotGrid = pivotGrid;
        this.deferred = deferred;
        this.headersList = this.pivotGrid.GetCustomizationHeaders();
        this.addNodeKeysAndHandlers();
    },
    addNodeKeysAndHandlers: function() {
        var i;
        for(i = 0; i < this.GetNodeCount(); i++)
            this.addNodeKeysAndHandlersCore(this.GetNode(i));
    },
    addNodeKeysAndHandlersCore: function(node) {
        var count, properties, images, instance, elem, i;
        count = node.GetNodeCount();
        if(count === 0) {
            instance = this;
            elem = node.GetHtmlElement();
            if(!elem)
                return;
            properties = node.name.split(',');
            node.contentID = elem.id;
            node.name = properties[0];
            node.allowDrag = properties[1] === 'T';
            if(node.allowDrag)
                ASPx.Evt.AttachEventToElement(elem, "mousedown", function (s) { instance.pivotGrid.headerMouseDown(instance.getHeader(elem), ASPx.Evt.GetEvent(s)); });
            images = ASPx.GetNodesByTagName(elem, "img");
            if(images && images.length)
                for (i = 0; i < images.length; i++)
                    images[i].ondragstart = function () { return false };
            return;
        }
        for(i = 0; i < node.GetNodeCount(); i++)
            this.addNodeKeysAndHandlersCore(node.GetNode(i));
    },
    getHeader: function(elem) {
        var node = this.GetRootNode().GetNodeBySelector(function(node) { return node.contentID && node.contentID == elem.id; });
        return this.findHeaderByNode(node);
    },
    findHeaderByNode: function (node) {
        var id, header;
        id = this.headersList.id.replace("listCF", node.name);
        header = ASPx.GetElementById(id);
        if (!header)
            header = ASPx.GetElementById(id.replace("pgHeader", "sortedpgHeader"));
        return header;
    },
    getHeaderId: function(id) {
        var index = id.lastIndexOf('pgHeader');
        if(index === -1)
            index = id.lastIndexOf('pgGroupHeader');
        return id.substring(index);
    },
    HandleDragFromCompleted: function(drag) {
        var fieldId, node;
        fieldId = this.getHeaderId(drag.obj.id);
        node = this.GetNodeByName(fieldId);
        this.hide(node);
    },
    HandleDragToCompleted: function(drag) {
        var fieldId, node;
        fieldId = this.getHeaderId(drag.obj.id);
        node = this.GetNodeByName(fieldId);
        this.show(node);
    },
    hide: function(node) {
        var current = node;
        while(current) {
            if(this.getVisibleChildNodesCount(current) !== 0)
                break;
            current.SetVisible(false);
            current = current.parent;
        }
    },
    show: function(node) {
        var current = node;
        while(current) {
            if(current.GetVisible())
                break;
            current.SetVisible(true);
            current = current.parent;
        }
    },
    getVisibleChildNodesCount: function(node) {
        var count, i;
        count = 0;
        for(i = 0; i < node.GetNodeCount(); i++) {
            if(node.GetNode(i).GetVisible())
                count++;
        }
        return count;
    }
});

ASPx.CustomizationTreeView = CustomizationTreeView;
})();