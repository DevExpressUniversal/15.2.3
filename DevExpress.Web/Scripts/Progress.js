/// <reference path="_references.js"/>

(function () {
var ProgressBarIDSuffix = {
    DivIndicator: "_DI",
    ValueIndicatorCell: "_VIC"
};

var ASPxClientProgressBarBase = ASPx.CreateClass(ASPxClientControl, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);

        this.displayMode = ASPxClientProgressBarBase.DisplayMode.Percentage;
        this.displayFormat = null;
        this.minimum = 0;
        this.maximum = 0;
        this.position = 0;
        this.onePercentValue = 0;
        this.hasOwner = true;
        this.customDisplayFormat = "";
    },

    // Initialization
    InlineInitialize: function (calledByOwner) {
        ASPxClientControl.prototype.InlineInitialize.call(this);

        if(calledByOwner || !this.hasOwner) {
            this.OnePercentValueUpdate();
            if(this.IsIndicatorDivWidthCorrectionRequired())
                this.SetCalculatedDivIndicatorWidth();
        }
    },

    OnePercentValueUpdate: function () {
        this.onePercentValue = (this.maximum - this.minimum) / 100;
    },

    // Elements access
    GetMainCell: function () {
        if(!this.mainCell)
            this.mainCell = ASPx.GetNodeByTagName(this.GetMainElement(), "TD", 0);
        return this.mainCell;
    },
    GetIndicatorDiv: function () {
        if(!this.divIndicator)
            this.divIndicator = ASPx.GetElementById(this.name + ProgressBarIDSuffix.DivIndicator);
        return this.divIndicator;
    },
    GetValueIndicatorTable: function () {
        if(!this.valueIndicatorTable)
            this.valueIndicatorTable = ASPx.GetParentByTagName(this.GetValueIndicatorCell(), "TABLE");
        return this.valueIndicatorTable;
    },
    GetValueIndicatorCell: function () {
        if(!this.valueIndicatorCell)
            this.valueIndicatorCell = ASPx.GetElementById(this.name + ProgressBarIDSuffix.ValueIndicatorCell);
        return this.valueIndicatorCell;
    },

    // Size adjustment
    AdjustControlCore: function () {
        ASPxClientControl.prototype.AdjustControlCore.call(this);
        this.UpdateIndicators();
        this.CorrectIndicatorHeight();
    },
    CorrectIndicatorHeight: function () {
        var mainCell = this.GetMainCell();
        var valueIndicatorTable = this.GetValueIndicatorTable();
        var indicatorDiv = this.GetIndicatorDiv();
        
        if(indicatorDiv)
            indicatorDiv.style.height = "";
        if(valueIndicatorTable) {
            valueIndicatorTable.style.height = "";
            valueIndicatorTable.style.marginTop = "";
        }

        var height = ASPx.GetClearClientHeight(mainCell);
        
        if(indicatorDiv)
            indicatorDiv.style.height = (height - ASPx.GetTopBottomBordersAndPaddingsSummaryValue(indicatorDiv)) + "px";
        if(valueIndicatorTable) {
            valueIndicatorTable.style.height = (height - ASPx.GetTopBottomBordersAndPaddingsSummaryValue(valueIndicatorTable)) + "px";
            valueIndicatorTable.style.marginTop = -height + "px";
        }

        if(ASPx.Browser.IE && ASPx.Browser.MajorVersion == 8) {
            var valueIndicatorCell = this.GetValueIndicatorCell();
            if(valueIndicatorCell)
                valueIndicatorCell.innerHTML = valueIndicatorCell.innerHTML;
        }
    },
    ResetIndicatorHeight: function () {
        ASPx.SetOffsetHeight(this.GetIndicatorDiv(), 1);
        var valueIndicatorTable = this.GetValueIndicatorTable();
        if(valueIndicatorTable)
            ASPx.SetOffsetHeight(valueIndicatorTable, 1);
    },
    GetCalculatedIndicatorDivWidth: function (percent) {
        var progressWidth = ASPx.GetClearClientWidth(this.GetMainCell());
        var indicatorDivStyle = ASPx.GetCurrentStyle(this.GetIndicatorDiv());
        progressWidth -= ASPx.PxToInt(indicatorDivStyle.borderLeftWidth) + ASPx.PxToInt(indicatorDivStyle.borderRightWidth);
        return progressWidth / 100 * percent;
    },

    UpdateIndicators: function () {
        if(this.IsIndicatorDivWidthCorrectionRequired()) {
            this.SetCalculatedDivIndicatorWidth();
        } else {
            var percent = this.GetPercent();
            this.GetIndicatorDiv().style.width = percent < 0 ? 0 : percent + "%";
        }

        var cell = this.GetValueIndicatorCell();
        if(cell) {
            cell.innerHTML = this.GetIndicatorText();
        }
    },
    GetIndicatorText: function () {
        if(this.displayMode == ASPxClientProgressBarBase.DisplayMode.Custom)
            return this.GetCustomText();
        var indicatorValue = this.displayMode == ASPxClientProgressBarBase.DisplayMode.Position ? this.position : this.GetPercent();
        if(this.displayFormat != null)
            indicatorValue = ASPx.Formatter.Format(this.displayFormat, indicatorValue);

        if(this.displayMode == ASPxClientProgressBarBase.DisplayMode.Position)
            return indicatorValue;
        if(this.rtl && ASPx.CultureInfo.percentPattern == 0)
            return indicatorValue + " %";
        return indicatorValue + "%";
    },
    SetCalculatedDivIndicatorWidth: function () {
        var indicatorWidth = this.GetCalculatedIndicatorDivWidth(this.GetPercent());
        if(indicatorWidth >= 0)
            this.GetIndicatorDiv().style.width = indicatorWidth + "px";
    },
    IsIndicatorDivWidthCorrectionRequired: function () {
        if(!ASPx.IsExistsElement(this.GetIndicatorDiv()))
            return false;
        var indicatorDivStyle = ASPx.GetCurrentStyle(this.GetIndicatorDiv());
        return ASPx.PxToInt(indicatorDivStyle.borderLeftWidth) > 0 || ASPx.PxToInt(indicatorDivStyle.borderRightWidth) > 0;
    },

    // API
    SetCustomDisplayFormat: function (value) {
        this.customDisplayFormat = value;
        this.UpdateIndicators();
    },
    GetDisplayText: function () {
        return this.GetIndicatorText();
    },
    GetCustomText: function () {
        if(this.displayFormat != null) {
            return this.customDisplayFormat
                .replace("{0}", ASPx.Formatter.Format(this.displayFormat, this.position))
                .replace("{1}", ASPx.Formatter.Format(this.displayFormat, this.minimum))
                .replace("{2}", ASPx.Formatter.Format(this.displayFormat, this.maximum));
        }
        else {
            return this.customDisplayFormat
                .replace("{0}", this.position)
                .replace("{1}", this.minimum)
                .replace("{2}", this.maximum);
        }
    },
    SetPosition: function (value) {
        this.position = Math.min(Math.max(value, this.minimum), this.maximum);
        this.UpdateIndicators();
    },
    SetMinMaxValues: function (minValue, maxValue) {
        var preparedMinValue = parseInt(minValue.toString(), 10);
        var preparedMaxValue = parseInt(maxValue.toString(), 10);
        if(isNaN(preparedMinValue))
            preparedMinValue = this.minimum;
        if(isNaN(preparedMaxValue))
            preparedMaxValue = this.maximum;
        if(preparedMaxValue > preparedMinValue) {
            this.maximum = preparedMaxValue;
            this.minimum = preparedMinValue;
            this.OnePercentValueUpdate();
            this.SetPosition(this.position);
        }
    },
    GetPosition: function () {
        return this.position;
    },
    GetPercent: function () {
        if(this.minimum === this.maximum)
            return 0;
        return (this.position - this.minimum) / this.onePercentValue;
    }
});
ASPxClientProgressBarBase.DisplayMode = {
    Percentage: 0,
    Position: 1,
    Custom: 2
};

window.ASPxClientProgressBarBase = ASPxClientProgressBarBase;
})();