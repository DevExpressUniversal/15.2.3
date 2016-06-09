ImagePropertyEditorTestControl_JS = function EasyTestJScripts_NavigationTestControl_JS(id, caption) {
    this.inherit = ImagePropertyEditorTestControl_JS.inherit;
    this.inherit(id, caption);

    this.Act = function(value) {
        if (value) {
            switch (value) {
                case 'Clear':
                    if (window[this.id + '_Clear']) {
                        //aspxBClick(this.id + '_Clear');
                        document.getElementById(this.id + '_Clear').click();
                        return;
                    }
                    else if (window[this.id + '_PNL_IE_Clear']) {
                        //aspxBClick(this.id + '_PNL_IE_Clear');
                        document.getElementById(this.id + '_PNL_IE_Clear').click();
                        return;
                    }
                    break;
                case 'Edit':
                    if (window[this.id + '_Edit']) {
                        //aspxBClick(this.id + '_Edit');
                        document.getElementById(this.id + '_Edit').click();
                        return;
                    }
                    else if (window[this.id + '_PNL_IE_Edit']) {
                        //aspxBClick(this.id + '_PNL_IE_Edit');
                        document.getElementById(this.id + '_PNL_IE_Edit').click();
                        return;
                    }
                    break;
            }
            var buttonControl = window[this.id + '_SHB'];
            if (buttonControl && buttonControl.GetText() == value) {
                ASPx.BClick(this.id + '_SHB');
                return;
            }
        }
        this.LogOperationError('The action ' + value + ' is not found in ' + this.caption + ' control');
    }
    this.IsBrowseVisible = function() {
        var browseBlock = document.getElementById(this.control.id + '_UPC');
        if (browseBlock) {
            return browseBlock.style.display != 'none';
        }
        return false;
    }
    this.GetBrowseButtonPosition = function(index) {
        if (!this.IsBrowseVisible()) {
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