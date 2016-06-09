TestControlBase_JS = function EasyTestJScripts_TestControlBase_JS(id, caption) {

    this.id = id;
    this.targetErrorControl = this;
    this.error = null;
    this.operationError = false;
    this.callStack = '';
    this.caption = caption;
    this.traceMessages = null;
    for (prop in this) if (this.hasOwnProperty(prop)) {
        this.traceMessages += '\n\r';
    }
    this.LogTraceMessage = function(message) {
        this.traceMessages += message + '\n\r';
    }
    this.LogEntry = function(name) {
        this.callStack += '>' + name + '\n\r';
    }
    this.LogExit = function(name) {
        this.callStack += '<' + name + '\n\r';
    }
    this.LogError = function(message) {
        if (this.error == null) {
            this.error = 'The ' + this.caption + ' control of ' + this.className + ' raised an exception: ' + message + '. Call Stack:' + this.callStack;
        }
    }
    this.LogOperationError = function(message) {
        if (this.error == null) {
            if (this.targetErrorControl != this) {
                this.targetErrorControl.LogOperationError(message);
            }
            else {
                this.operationError = true;
                this.error = message;
            }
        }
    }
    this.IsVisible = function() {
        this.InitControl();
        if (this.error) {
            return false;
        }
        return true;
    }
    this.InitControl = function() {
        if (!this.control) {
            this.control = document.getElementById(this.id.replace(/\$/g, '_'));
            if (!this.control) {
                this.LogOperationError('Cannot retrieve an element. id= "' + this.id + '" name="' + this.caption + '"');
                return;
            }
        }
    }
    TestControlBase.prototype.baseInitControl = this.InitControl;
    TestControlBase.prototype.baseIsVisible = this.IsVisible;
}

EasyWrappedClass_JS = function EasyTestJScripts_EasyWrappedClass_JS(mainObj) {
    this.WrappFunc = function(func, name) {
        var that = this;
        return function() {
            mainObj.LogEntry(name);
            mainObj.InitControl.apply(that);
            var result = func.apply(that, arguments);
            mainObj.LogExit(name);
            return result;
        }
    };
    for (var name in mainObj) {
        if (typeof (mainObj[name]) == 'function') {
            this[name] = this.WrappFunc(mainObj[name], name);
        } else {
            this[name] = mainObj[name];
        }
    }
}

WrappedClass_JS = function EasyTestJScripts_WrappedClass_JS(mainObj) {
    this.mainObj = mainObj;
    this.easyWrapper = new EasyWrappedClass_JS(mainObj);
    this.easyWrapper.targetErrorControl = this.easyWrapper;
    this.CollectVariables = function() {
        for (var _name in this.easyWrapper) {
            if (typeof (this.easyWrapper[_name]) != 'function') {
                this[_name] = this.easyWrapper[_name];
            }
        }
    }
    this.WrappFunc = function(func, name) {
        return function() {
            try {
                this.mainObj.callStack = '';
                var result = this.easyWrapper[name].apply(this.easyWrapper, arguments);
                this.CollectVariables();
                return result;
            }
            catch (e) {
                if (e.operationError) {
                    this.easyWrapper.LogOperationError(e.message);
                    this.CollectVariables();
                }
                else {
                    if (!this.easyWrapper.operationError) {
                        this.easyWrapper.LogError(e.message);
                        this.CollectVariables();
                        throw e;
                    }
                }
            }
        }
    };
    for (var name in mainObj) {
        if (typeof (mainObj[name]) == 'function') {
            this[name] = this.WrappFunc(mainObj[name], name);
        } else {
            this[name] = mainObj[name];
        }
    }
}