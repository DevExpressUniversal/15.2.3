

(function() {
////////////////////////////////////////////////////////////////////////////////
//////////////////////// ASPxClientRecurrenceTypeEdit //////////////////////////
////////////////////////////////////////////////////////////////////////////////
var ASPxClientRecurrenceTypeEdit = ASPx.CreateClass(ASPxClientRadioButtonList, {
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
    },
    InlineInitialize: function() {
        ASPxClientRadioButtonList.prototype.InlineInitialize.call(this);
        this.recurrenceType = this.GetRecurrenceType();
    },
    RaiseSelectedIndexChanged: function(processOnServer) {
        this.recurrenceType = this.GetRecurrenceType();
        ASPxClientRadioButtonList.prototype.RaiseSelectedIndexChanged.call(this, processOnServer); 
    },
    GetRecurrenceType: function() {
        var index = this.GetSelectedIndex();
        if (!this.EnableHourlyRecurrence)
            index += 1;
        switch (index) {
            case 0:
                return ASPxClientRecurrenceType.Hourly;
            case 1:
                return ASPxClientRecurrenceType.Daily;
            case 2:
                return ASPxClientRecurrenceType.Weekly;
            case 3:
                return ASPxClientRecurrenceType.Monthly;
            case 4:
                return ASPxClientRecurrenceType.Yearly;
            case 5:
                return ASPxClientRecurrenceType.Minutely;            
        }
        return null;
    },
    SetRecurrenceType: function(recurrenceType) {
        var selectedIndex = 0;
        switch (recurrenceType) {
            case ASPxClientRecurrenceType.Hourly:
                selectedIndex = 0;
                break;
            case ASPxClientRecurrenceType.Daily:
                selectedIndex = 1;
                break;
            case ASPxClientRecurrenceType.Weekly:
                selectedIndex = 2;
                break;
            case ASPxClientRecurrenceType.Monthly:
                selectedIndex = 3;
                break;
            case ASPxClientRecurrenceType.Yearly:
                selectedIndex = 4;
                break;
            case ASPxClientRecurrenceType.Minutely:
                selectedIndex = 5;
                break;            
        }
        if (!this.EnableHourlyRecurrence)
            selectedIndex -= 1;
        this.SetSelectedIndex(selectedIndex);
    }
});

window.ASPxClientRecurrenceTypeEdit = ASPxClientRecurrenceTypeEdit;
})();