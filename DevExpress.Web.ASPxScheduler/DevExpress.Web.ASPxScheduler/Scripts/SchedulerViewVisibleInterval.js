(function() {
////////////////////////////////////////////////////////////////////////////////
// ASPxClientSchedulerViewVisibleInterval
////////////////////////////////////////////////////////////////////////////////
var ASPxClientSchedulerViewVisibleInterval = ASPx.CreateClass(ASPxClientControl, {
	constructor: function(name) {
		this.constructor.prototype.constructor.call(this, name);
		this.schedulerControlId = "";
    }
});
////////////////////////////////////////////////////////////////////////////////

window.ASPxClientSchedulerViewVisibleInterval = ASPxClientSchedulerViewVisibleInterval;
})();