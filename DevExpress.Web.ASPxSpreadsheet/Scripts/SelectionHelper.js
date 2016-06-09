

(function () {
    var ASPxClientSpreadsheetSelectionChangedEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(selection) {
            this.constructor.prototype.constructor.call(this);
            this.selection = selection;
        }
    });
    ASPxClientSpreadsheet.ASPxClientSpreadsheetSelection = (function(){
        return function(activeCellColumnIndex, activeCellRowIndex, leftColumnIndex, topRowIndex, rightColumnIndex, bottomRowIndex) {

            this.activeCellColumnIndex = activeCellColumnIndex;
            this.activeCellRowIndex    = activeCellRowIndex;

            this.leftColumnIndex    = leftColumnIndex;
            this.topRowIndex        = topRowIndex;
            this.rightColumnIndex   = rightColumnIndex;
            this.bottomRowIndex     = bottomRowIndex;
        }
    })();

    window.ASPxClientSpreadsheetSelectionChangedEventArgs = ASPxClientSpreadsheetSelectionChangedEventArgs;
})();