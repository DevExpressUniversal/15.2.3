JSFileDataTestControl = function EasyTestJScripts_JSFileDataTestControl(id, caption) {
    this.inherit = JSFileDataTestControl.inherit;
    this.inherit(id, caption);
    this.GetIsAutoPostBackAction = function(name) {
        if (name == 'Change file') {
            return false;
        }
        else {
            return this.AutoPostBack;
        }
    }
    this.IsFileExists = function() {
        var linkElement = this.control.cells[0].childNodes[0].cells[0].childNodes[0];
        return linkElement && linkElement.style.display != 'none';
    }
    this.IsViewMode = function() {
        return this.control.cells[1].children.length == 0;
    }
    this.IsBrowseVisible = function() {
        if (this.IsViewMode()) {
            return false;
        }
        else {
            var browseBlock = document.getElementById(this.control.id + '_UPC');
            if (browseBlock) {
                return browseBlock.style.display != 'none';
            }
            return false;
        }
    }
    this.GetHrefControl = function() {
        var fileExists = this.IsFileExists();
        var isViewMode = this.IsViewMode();
        var result;
        if (fileExists) {
            result = this.control.cells[0].childNodes[0].cells[0].childNodes[0];
        }
        else if (isViewMode) {
            result = this.control.cells[1];
        }
        return result;
    }
    this.GetText = function() {
        var fileExists = this.IsFileExists();
        var isViewMode = this.IsViewMode();
        var isBrowseVisible = this.IsBrowseVisible();
        if (this.error) {
            return;
        }
        var hrefControl = this.GetHrefControl();
        var result = '';
        if (fileExists) {
            result = hrefControl.innerHTML;
            if (!isViewMode) {
                if (isBrowseVisible)
                    result = '(With actions & visible browse)' + result;
                else
                    result = '(With actions & invisible browse)' + result;
            }
        }
        else if (isViewMode) {
            result = hrefControl.innerHTML;
        }
        return result;
    }
    this.SetText = function(value) {
        this.LogOperationError('It is impossible to set the value of fileinput field by JavaScript or control is in view mode');
    }
    this.Act = function(value) {
        if (value) {
            switch (value) {
                case 'Clear':
                    if (this.IsViewMode()) {
                        this.LogOperationError('Control is in view mode');
                    }
                    else {
                        //aspxBClick(this.id + '_Clear');
                        document.getElementById(this.id + '_Clear').click();
                    }
                    break;
                case 'Change file':
                    if (this.IsViewMode()) {
                        this.LogOperationError('Control is in view mode');
                    }
                    else {
                        //aspxBClick(this.id + '_Edit');
                        document.getElementById(this.id + '_Edit').click();
                    }
                    break;
            }
        }
        else {
            var hrefControl = this.GetHrefControl();
            if (hrefControl) {
                hrefControl.click();
            }
        }
    }
    this.GetBrowseButtonPosition = function(index) {
        if (!this.IsBrowseVisible() && this.IsFileExists()) {
            this.LogOperationError('Browser button is not visible');
        }
        else {
            var browserPosition = this.GetControlCenterPosition(this.id + '_UPC_Browse0');
            if (browserPosition) {
                return browserPosition[index];
            }
        }
        return null;
    }
}