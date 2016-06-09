TabsTestControl_JS = function EasyTestJScripts_JSTabsTestControl(id, caption) {
    this.inherit = TabsTestControl_JS.inherit;
    this.inherit(id, caption);

    this.GetText = function() {
        var aspxControl = window[this.id];
        var result = aspxControl.GetTabText(aspxControl.activeTabIndex);
        if (result && result != '') {
            return result;
        }
        else {
            this.LogOperationError('There is now active layout tab or it\'s text is empty.');
        }
    }

    this.SetText = function(value) {
        var aspxControl = window[this.id];
        var control = this.control;
        var actionContainers = document.getElementById(this.id + '_CC');
        var itemIndex = -1;
        var itemCount = aspxControl.GetTabCount() ? aspxControl.GetTabCount() : aspxControl.tabCount;
        for (var i = 0; i < itemCount; i++) {
            var item = document.getElementById(this.id + '_T' + i);
            if (item.innerText.indexOf(value) != -1) {
                itemIndex = i;
                break;
            }
            item = document.getElementById(this.id + '_AT' + i);
            if (item.innerText.indexOf(value) != -1) {
                itemIndex = i;
                break;
            }
        }
        if (itemIndex != -1) {
            aspxControl.ChangeActiveTab(itemIndex, false);
        }
        else {
            this.LogOperationError('The "' + value + '" navigation item does not exist');
        }
    }

    this.Act = function(value) {
        var executableControl = this.GetExecutableControl(value);
        if (executableControl) {
            if (executableControl.GetEnabled()) {
                if (this.navGroupActionContainer) {
                    this.navGroupActionContainer.DoItemClick(executableControl.GetIndexPath(), false, null);
                }
                else {
                    if (executableControl.onclick) {
                        executableControl.click();
                    }
                }
            }
            else {
                this.LogOperationError('The "' + value + '" item of the "' + this.caption + '" Action is disabled');
            }
        }
        else {
            this.LogOperationError('The "' + this.caption + '" Action does not contain the "' + value + '" item');
        }
    }

    this.GetExecutableControl = function(value) {
        var pageControlId = this.id + '_PC';
        var aspxControl = window[pageControlId];
        var control = this.control;
        var actionContainers = document.getElementById(pageControlId + '_CC');
        var executableControl = null;
        var groupCount = aspxControl.GetTabCount() ? aspxControl.GetTabCount() : aspxControl.tabCount;
        var itemCount = groupCount;
        var executableControlCaption = value;
        var tabCaption;

        var currentIndex = 0;

        if (value.indexOf && value.indexOf('.') != -1) {
            tabCaption = value.substring(0, value.indexOf('.'));
            executableControlCaption = value.substring(value.indexOf('.') + 1, value.length);
        }
        for (var i = 0; i < groupCount && executableControl == null; i++) {
            var navGroupActionContainerId = this.id + '_PC_M' + i + '_M';
            this.navGroupActionContainer = null;
            if (document.getElementById(navGroupActionContainerId)) {
                this.navGroupActionContainer = window[navGroupActionContainerId];
            }
            var tabGroup;
            //if (tabCaption) {
            tabGroup = document.getElementById(this.id + '_T' + i);
            if (!tabGroup) {
                tabGroup = document.getElementById(this.id + '_PC_T' + i);
            }
            //}
            if (this.navGroupActionContainer) {
                var tabGroupCaption;
                if (tabGroup) {
                    tabGroupCaption = this.Trim(tabGroup.innerText);
                }
                if (tabCaption && tabGroupCaption && tabCaption != tabGroupCaption) {
                    continue;
                }
                for (var j = 0; j < this.navGroupActionContainer.GetItemCount(); j++) {
                    var item = this.navGroupActionContainer.GetItem(j);
                    if (item.GetText() == executableControlCaption || currentIndex == value) {
                        executableControl = item;
                        executableControl.tabText = tabGroupCaption;
                        executableControl.GetTabText = function() {
                            return executableControl.tabText;
                        }
                        break;
                    }
                    currentIndex++;
                }
            }
        }
        if ((currentIndex == 0 || typeof(value) != typeof 1) && !executableControl) {
            for (var i = 0; i < itemCount; i++) {
                var item = document.getElementById(pageControlId + '_T' + i);
                var active = false;
                if (!item) {
                    item = document.getElementById(pageControlId + '_AT' + i);
                    active = true;
                }
                if (this.Trim(this.RemoveLineBrakes(item.innerText)) == value || currentIndex == value) {
                    executableControl = item;
                    executableControl.disabled = !active && !executableControl.onclick;
                    executableControl.GetEnabled = function() {
                        return !this.disabled;
                    }
                    executableControl.GetVisible = function() {
                        return true;
                    }
                    executableControl.GetText = function() {
                    return this.Trim(this.RemoveLineBrakes(item.innerText));
                    }
                    break;
                }
                currentIndex++;
            }
        }
        return executableControl;
    }
    this.IsActionItemVisible = function(actionItemName) {
        var executableControl = this.GetExecutableControl(actionItemName);
        if (executableControl) {
            return executableControl.GetVisible();
        }
        return false;
    }
    this.IsActionItemEnabled = function(actionItemName) {
        var executableControl = this.GetExecutableControl(actionItemName);
        if (executableControl) {
            return executableControl.GetEnabled();
        }
        return false;
    }
}