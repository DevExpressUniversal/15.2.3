PivotGridTestControl_JS = function EasyTestJScripts_PivotGridTestControl_JS(id, caption, fieldCaptions) {
    this.inherit = PivotGridTestControl_JS.inherit;
    this.inherit(id, caption);
    this.InitFields = function(fieldCaptions) {
        this.fieldHeaders = new Array();
        var fieldCaptionParameters = fieldCaptions.split(';');
        for (var i = 0; i < fieldCaptionParameters.length; i++) {
            var fieldCaptionId = fieldCaptionParameters[i].split('=');
            var fieldCaption = fieldCaptionId[0];
            var fieldId = fieldCaptionId[1];
            this.fieldHeaders[fieldCaption] = fieldId;
        }
    }
    this.InitFields(fieldCaptions);

    this.GetAreaId = function(areaName) {
        var areaId = '';
        switch (areaName) {
            case "RowArea":
                areaId = "0";
                break;
            case "ColumnArea":
                areaId = "1";
                break;
            case "FilterArea":
                areaId = "2";
                break;
            case "DataArea":
                areaId = "3";
                break;
        }
        if (areaId == '') {
            this.LogOperationError("Incorrect Pivot Area '" + areaName + "'");
        }
        return "pgArea" + areaId;
    }
    this.MoveFieldToArea = function(fieldCaption, areaName) {
        var aspxControl = window[this.id];
        var areaId = this.GetAreaId(areaName);
        var fieldHeaderId = this.fieldHeaders[fieldCaption];
        if (fieldHeaderId != undefined) {
            aspxControl.PerformCallbackInternal(aspxControl.GetMainTable(), 'D|"' + fieldHeaderId + "|" + areaId + '|true');
        }
    }
}