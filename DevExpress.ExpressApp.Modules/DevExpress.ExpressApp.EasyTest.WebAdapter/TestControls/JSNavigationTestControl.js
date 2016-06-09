NavigationTestControl_JS = function EasyTestJScripts_NavigationTestControl_JS(id, caption, cpGroupToTreeViewMap) {
    this.inherit = NavigationTestControl_JS.inherit;
    this.inherit(id, caption);
    this.InitGroupToTreeViewMap = function(map) {
        if(map) {
            this.cpGroupToTreeViewMap = [];
            var parameters = map.split(';');
            for(var i = 0; i < parameters.length; i++) {
                var parameter = parameters[i].split('=');
                var parameterKey = parameter[0];
                var parameterValue = parameter[1];
                this.cpGroupToTreeViewMap[parameterKey] = parameterValue;
            }
        }
    }

    this.InitGroupToTreeViewMap(cpGroupToTreeViewMap);

    this.GetFullCaption = function(captionString) {
        var fullCaption;
        var fullCaptions = this.FindFullCaptions(captionString, true);
        if(fullCaptions.length == 1) {
            fullCaption = fullCaptions[0];
        }
        else if(fullCaptions.length > 1) {
            this.LogOperationError('The "' + this.caption + '" action contains several items with the "' + captionString + '" caption. (' + fullCaptions.join(", ") + ')');
        }
        else {
            fullCaptions = this.FindFullCaptions(captionString, false);
            if(fullCaptions.length == 1) {
                fullCaption = fullCaptions[0];
            }
            else if(fullCaptions.length > 1) {
                this.LogOperationError('The "' + this.caption + '" action contains several items with the "' + captionString + '" caption. (' + fullCaptions.join(", ") + ')');
            }
            else {
                this.LogOperationError('The "' + this.caption + '" Action does not contain the "' + captionString + '" item');
            }
        }
        return fullCaption;
    }
    this.FindFullCaptions = function(captionString, caseSensitive) {
        var result = [];
        var toSearch = captionString;
        if(!caseSensitive) {
            toSearch = toSearch.toLowerCase();
        }
        var fullCaptionsByCaption = this.CollectFullCaptionsByCaptionDictionary();
        for(var caption in fullCaptionsByCaption) {
            var fullCaptions = fullCaptionsByCaption[caption];
            for(var i = 0; i < fullCaptions.length; i++) {
                var entry = fullCaptions[i];
                if(!caseSensitive) {
                    entry = entry.toLowerCase();
                }
                if(toSearch == entry) {
                    result.push(fullCaptions[i]);
                }
            }
        }
        if(result.length == 0) {
            for(var caption in fullCaptionsByCaption) {
                var entry = caption;
                if(!caseSensitive) {
                    entry = entry.toLowerCase();
                }
                if(toSearch == entry) {
                    result = result.concat(fullCaptionsByCaption[caption]);
                }
            }
        }
        return result;
    }
    this.CollectFullCaptionsByCaptionDictionary = function() {
        var fullCaptionsByCaption;
        var aspxControl = window[this.id];
        if(aspxControl.groups) {
            fullCaptionsByCaption = this.CollectFullCaptionsByCaptionFromNavBar(aspxControl);
        } else {
            fullCaptionsByCaption = this.CollectFullCaptionsByCaptionFromTreeList(aspxControl);
        }
        return fullCaptionsByCaption;
    }
    this.CollectFullCaptionsByCaptionFromNavBar = function(navBar) {
        var fullCaptionsByCaption = [];
        for(var i = 0; i < navBar.groups.length; i++) {
            var groupName = navBar.groups[i].name;
            var groupCaptions = [];
            this.AddCaptionToArray(groupCaptions, groupName, groupName);
            var treeId = this.cpGroupToTreeViewMap ? this.cpGroupToTreeViewMap[groupName] : null;
            if(treeId) {
                var tree = window[treeId];
                var treeCaptions = this.CollectFullCaptionsByCaptionFromTreeList(tree, groupName);
                this.MergeArrays(groupCaptions, treeCaptions);
            }
            else {
                var groupItems = navBar.groups[i].items;
                for(var j = 0; j < groupItems.length; j++) {
                    var itemText = groupItems[j].GetText();
                    this.AddCaptionToArray(groupCaptions, itemText, groupName + '.' + itemText);
                }
            }
            this.MergeArrays(fullCaptionsByCaption, groupCaptions);
        }
        return fullCaptionsByCaption;
    }
    this.CollectFullCaptionsByCaptionFromTreeList = function(tree, parentCaption) {
        var fullCaptionsByCaption = [];
        var keysByCaption = tree.cpNodeCaptionToNodeKeyMap;
        for(var caption in keysByCaption) {
            var keys = keysByCaption[caption];
            for(var j = 0; j < keys.length; j++) {
                var fullCaption = tree.cpNodeNodeKeyToFullCaptionMap[keys[j]];
                if(parentCaption) {
                    fullCaption = parentCaption + '.' + fullCaption;
                }
                this.AddCaptionToArray(fullCaptionsByCaption, caption, fullCaption);
            }
        }
        return fullCaptionsByCaption;
    }
    this.MergeArrays = function(targetArray, array) {
        for(var caption in array) {
            var fullCaptions = array[caption];
            for(var j = 0; j < fullCaptions.length; j++) {
                this.AddCaptionToArray(targetArray, caption, fullCaptions[j]);
            }
        }
    }
    this.AddCaptionToArray = function(array, caption, fullCaption) {
        var fullCaptionArray = array[caption];
        if(!fullCaptionArray) {
            fullCaptionArray = [];
            array[caption] = fullCaptionArray;
        }
        fullCaptionArray[fullCaptionArray.length] = fullCaption;
    }

    this.ProcessFullCaptionByNavBar = function (navBar, fullCaption, forceExecute) {
        this.LogTraceMessage(">>ProcessFullCaptionByNavBar ");
        var result = false;
        var dotIndex = fullCaption.indexOf('.');
        var isRootGroup = dotIndex == -1;
        if(isRootGroup) {
            this.LogOperationError('The "' + this.caption + '" Action does not contain the "' + fullCaption + '" item');
        }
        else {
            var groupName = fullCaption.substring(0, dotIndex); //TODO: dot in NavBarGroupCaption with nested tree is not supported. T216057
            var fullCaptionTail = fullCaption.substring(dotIndex + 1, fullCaption.length);
            var treeId = this.cpGroupToTreeViewMap ? this.cpGroupToTreeViewMap[groupName] : null;
            if(treeId) {
                var tree = window[treeId];
                result = this.ProcessFullCaptionByTreeList(tree, fullCaptionTail, forceExecute);
            }
            else {
                var group = navBar.GetGroupByName(groupName);
                var groupItems = group.items;
                var groupItem;
                for(var i = 0; i < groupItems.length; i++) {
                    if(this.Trim(groupItems[i].GetText()) == fullCaptionTail) {
                        groupItem = groupItems[i];
                        break;
                    }
                }
                var isActionItemEnabled = navBar.DoItemClick != undefined && !groupItem.disabled;
                if(forceExecute) {
                    if(isActionItemEnabled) {
                        navBar.DoItemClick(group.index, groupItem.index, false);
                        result = true;
                    }
                    else {
                        this.LogOperationError('The "' + caption + '" item of the "' + this.caption + '" Action is disabled');
                        result = false;
                    }
                }
                else {
                    result = isActionItemEnabled;
                }
            }
        }
        this.LogTraceMessage("<<ProcessFullCaptionByNavBar ");
        return result;
    }
    this.LogNode = function (node, tabCount) {
        var prefix = "";
        for (var k = 0; k < tabCount; k++) {
            prefix += '\t';
        }
        this.LogTraceMessage(prefix + node.name);
        for(var i =0; i<node.GetNodeCount();i++) {
            var subNode = node.GetNode(i);
            this.LogNode(subNode, tabCount+1);
        }
    }
    this.LogTree = function(tree) {
        var rootNode = tree.GetRootNode();
        this.LogNode(rootNode,0);
    }
    this.ProcessFullCaptionByTreeList = function (tree, fullCaption, forceExecute) {
        this.LogTraceMessage(">>ProcessFullCaptionByTreeList");
        var result = false;
        var keys = tree.cpNodeFullCaptionToNodeKeyMap[fullCaption];
        var key = keys[0];
        var canClick = tree.cpNodeKeyToInfoMap[key];
        this.LogTraceMessage("canClick=" + canClick);
        if(canClick) {
            var treeNode = tree.GetNodeByName(key);
            this.LogTraceMessage("treeNode=" + treeNode);
            this.LogTraceMessage("key=" + key);
            for (var i = 0; i < keys.length; i++) {
                this.LogTraceMessage("keys[" + i.toString() + "]=" + keys[i]);
                this.LogTree(tree);
            }
            if(treeNode && treeNode.GetEnabled()) {
                var nodeElement = window[treeNode.contentElementID];
                this.LogTraceMessage("nodeElement=" + nodeElement);
                if (nodeElement) {
                    this.LogTraceMessage("forceExecute=" + forceExecute);
                    if(forceExecute) {
                        nodeElement.click();
                    }
                    result = true;
                }
            }
        }
        this.LogTraceMessage("<<ProcessFullCaptionByTreeList");
        return result;
    }

    this.GetEntriesAsJSONString = function() {
        var aspxControl = window[this.id];
        return aspxControl.cpEntriesAsJSON; //should be the same name for 'NavBar' and 'TreeList' implementations.
    }
    this.ExpandTreeNodeByNodeName = function (treeId, nodeName) {
        this.LogTraceMessage(">>ExpandTreeNodeByNodeName");
        this.LogTraceMessage("treeId = " + treeId);
        this.LogTraceMessage("nodeName = " + nodeName);
        var tree = window[treeId];
        this.LogTraceMessage("tree= " + tree);
        if (tree && nodeName) {
            var node = tree.GetNodeByName(nodeName);
            this.LogTraceMessage("node " + node);
            node.SetExpanded(true);
            this.LogTraceMessage("node.GetExpanded() " + node.GetExpanded());
        }
        this.LogTraceMessage("<<ExpandTreeNodeByNodeName");
    }
    this.GetFullPath = function(value) {
        return this.GetFullCaption(value);
    }
    this.ExpandPath = function (value) {
        this.LogTraceMessage(">>ExpandPath ");
        var fullCaption = this.GetFullCaption(value);
        if(fullCaption) {
            var aspxControl = window[this.id];
            var tree;
            if(aspxControl.groups) {
                var groupName;
                if(fullCaption.indexOf('.') != -1) {
                    groupName = value.substring(0, value.indexOf('.'));
                    fullCaption = value.substring(value.indexOf('.') + 1, value.length);
                }
                var treeId = this.cpGroupToTreeViewMap ? this.cpGroupToTreeViewMap[groupName] : null;
                if(treeId) {
                    tree = window[treeId];
                }
            } else {
                tree = aspxControl;
            }
            if(tree) {
                if(fullCaption.lastIndexOf('.') != -1) {
                    fullCaption = fullCaption.substring(0, fullCaption.lastIndexOf('.'));
                    var nodeName = tree.cpNodeFullCaptionToNodeKeyMap[fullCaption];
                    tree.GetNodeByName(nodeName[0]).SetExpanded(true);
                }
            }
        }
        this.LogTraceMessage("<<ExpandPath ");
    }
    this.GetText = function() {
        var result;
        var aspxControl = window[this.id];
        if(aspxControl.groups) {
            for(var i = 0; i < aspxControl.groups.length; i++) {
                var groupName = aspxControl.groups[i].name;
                var treeId = this.cpGroupToTreeViewMap ? this.cpGroupToTreeViewMap[groupName] : null;
                if(treeId) {
                    var tree = window[treeId];
                    var selectedNode = tree.GetSelectedNode();
                    if(selectedNode) {
                        result = groupName + '.' + tree.cpNodeNodeKeyToFullCaptionMap[selectedNode.name]
                    }
                }
            }
        } else {
            var selectedNode = aspxControl.GetSelectedNode();
            if(selectedNode) {
                result = aspxControl.cpNodeNodeKeyToFullCaptionMap[selectedNode.name];
            }
        }
        return result;
    }
    this.SetText = function(value) {
        if(!IsNull(this.control.readOnly) && this.control.readOnly) {
            this.LogOperationError('The "' + this.caption + '" editor is readonly.');
            return;
        }
        this.control.value = value;
    }
    this.IsEnabled = function() {
        return true;
    }
    this.Act = function (value) {
        this.LogTraceMessage(">>Act");
        var fullCaption = this.GetFullCaption(value);
        this.LogTraceMessage("fullCaption= " + fullCaption);
        if(fullCaption) {
            var aspxControl = window[this.id];
            if(aspxControl.groups) {
                this.ProcessFullCaptionByNavBar(aspxControl, fullCaption, true);
            } else {
                this.ProcessFullCaptionByTreeList(aspxControl, fullCaption, true);
            }
        }
        this.LogTraceMessage("<<Act");
    }
    this.GetCellValue = function(row) {
        var fullCaptionsByCaption = this.CollectFullCaptionsByCaptionDictionary();
        var aspxControl = window[this.id];
        var isNavBar = aspxControl.groups;
        var currentRow = 0;
        for(var caption in fullCaptionsByCaption) {
            var fullCaptions = fullCaptionsByCaption[caption];
            for(var i = 0; i < fullCaptions.length; i++) {
                if(currentRow == row) {
                    if((isNavBar || fullCaptions.length > 1) && fullCaptions[i] == caption) {
                        return null;
                    }
                    else {
                        return fullCaptions[i];
                    }
                }
                currentRow++;
            }
        }
        return null;
    }
    this.GetRowCount = function() {
        var fullCaptionsByCaption = this.CollectFullCaptionsByCaptionDictionary();
        var count = 0;
        for(var item in fullCaptionsByCaption) {
            count += fullCaptionsByCaption[item].length;
        }
        return count;
    }
    this.IsActionItemEnabled = function(actionItemName) {
        var fullCaption = this.GetFullCaption(actionItemName);
        if(fullCaption) {
            var aspxControl = window[this.id];
            if(aspxControl.groups) {
                return this.ProcessFullCaptionByNavBar(aspxControl, fullCaption, false);
            } else {
                return this.ProcessFullCaptionByTreeList(aspxControl, fullCaption, false);
            }
        }
        else {
            return false;
        }
    }
    this.IsActionItemVisible = function(actionItemName) {
        var fullCaption = this.GetFullCaption(actionItemName);
        return fullCaption ? true : false;
    }
}