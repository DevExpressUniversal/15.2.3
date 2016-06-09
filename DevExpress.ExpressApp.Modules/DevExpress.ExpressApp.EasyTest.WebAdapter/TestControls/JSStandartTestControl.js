StandartTestControl_JS = function EasyTestJScripts_StandartTestControl_JS(id, caption) {
    this.inherit = StandartTestControl_JS.inherit;
    this.inherit(id, caption);
    this.SetText = function(value) {
        if (!IsNull(this.control.readOnly)) {
            if (this.control.readOnly) {
                this.LogOperationError('The "' + this.caption + '" editor is readonly.');
                return;
            }
        }
        this.control.value = value;
    }
    this.GetText = function() {
        if (this.control.value) {
            return this.control.value;
        }
        return this.control.innerText;
    }
    this.IsEnabled = function() {
        return !this.control.disabled;
    }
    this.Trim = function(str) {
        while (str.substring(0, 1) == ' ') {
            str = str.substring(1, str.length);
        }
        while (str.substring(str.length - 1, str.length) == ' ') {
            str = str.substring(0, str.length - 1);
        }
        str = str.replace(/^(\r\n)+/g, '');
        str = str.replace(/(\r\n)+$/g, '');
        return str;
    }
    this.RemoveLineBrakes = function(str) {
        str = str.replace(/\r/g, '');
        str = str.replace(/ \n+/g, ' ');
        str = str.replace(/\n+ /g, ' ');
        str = str.replace(/\n+/g, ' ');
        str = str.replace(/ (<BR>)+/g, ' ');
        str = str.replace(/(<BR>)+ /g, ' ');
        str = str.replace(/(<BR>)+/g, ' ');
        return str;
    }
    var mX, mY;
    var elemId;
    var parentWindowOnMouseMove;

    this.StartObserveMouseMove = function () {
        parentWindowOnMouseMove = document.parentWindow.onmousemove;
        document.parentWindow.onmousemove = function (e) {
            mX = e.pageX;
            mY = e.pageY;
        };
    }
    this.EndObserveMouseMove = function () {
        document.parentWindow.onmousemove = parentWindowOnMouseMove;
    }
    this.GetControlCenterPosition = function (controlId) {
        elemId = controlId;
        var control = document.getElementById(controlId);
        if (!control) {
            this.LogOperationError('Cannot retrieve an element. id= "' + this.id + '"');
            return;
        }
        var rcts = control.getBoundingClientRect();
        var curleft = rcts.left + document.parentWindow.screenLeft;
        var curtop = rcts.top + document.parentWindow.screenTop;
        return [Math.round(curleft + (rcts.width / 2)), Math.round(curtop + (rcts.height / 2))];
    }
    this.MousDistanceToControl = function (index) {
        var elem = document.getElementById(elemId);
        var rcts = elem.getBoundingClientRect();
        var mousDistanceX = mX - (rcts.left + (rcts.width / 2));
        var mousDistanceY = mY - (rcts.top + (rcts.height / 2));
        return [Math.round(mousDistanceX), Math.round(mousDistanceY)][index];
    }
}