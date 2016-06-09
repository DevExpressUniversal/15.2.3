NavigationTabsTestControl_JS = function EasyTestJScripts_JSNavigationTabsTestControl(id, caption) {
    this.inherit = NavigationTabsTestControl_JS.inherit;
    this.inherit(id, caption);

    this.GetText = function() {
        var pageControlId = this.id + '_PC';
        var aspxControl = window[pageControlId];
        var result = aspxControl.GetTabText(aspxControl.activeTabIndex);
        var activeTabMenuControl = window[this.id + '_PC_M' + aspxControl.activeTabIndex + '_M'];
        var selectedItem = activeTabMenuControl.GetSelectedItem();
        if (selectedItem) {
            result += '.' + selectedItem.GetText();
        }
        return result;
    }

    this.GetCellValue = function(row) {
        var executableControl = this.GetExecutableControl(row);
        if (executableControl && executableControl.GetEnabled()) {
            if (executableControl.GetTabText) {
                return executableControl.GetTabText() + '.' + executableControl.GetText();
            }
            else {
                return executableControl.GetText();
            }
        }
        return null;
    }
    this.GetRowCount = function() {
        var count = 0;
        while (this.GetExecutableControl(count)) {
            count++;
        }
        return count;
    }
}