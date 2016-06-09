
window.EasyTestScriptLoader = {
    scripts: {},
    composites: {},
    loaded: 3,
    Initialize: function() {
        var scripts = document.getElementsByTagName("script");
    },
    LoadScript: function(name, executionCallback) {
        var scriptInfo = this.getScriptInfo(name);
        scriptInfo._callback = executionCallback;
        var existingList = existing = this.toIndex(existingList);
        this.foreachCall(scriptInfo, "_callback");
    },
    defineScripts: function(defaultScriptInfo, scriptInfos) {
        this.foreach(scriptInfos, function(scriptInfo) {
            this.defineScript(merge(null, defaultScriptInfo, scriptInfo));
        });
    },
    getScriptInfo: function(name) {
        return this.resolveScriptInfo(name) || (this.scripts[name] = { name: name });
    },
    resolveScriptInfo: function(name) {
        var info = typeof (name) == "string" ?
                       (this.scripts[name] || this.composites[name]) :
                       (name ? (name.script || name) : null);
        if (info && !info._isScript) {
            info = null;
        }
        return info;
    },
    getAndRemove: function(obj, field) {
        var r = obj[field];
        delete obj[field];
        return r;
    },
    foreachCall: function(obj, field, args) {
        this.foreach(this.getAndRemove(obj, field), function(callback) {
            callback.apply(null, args || []);
        });
    },
    toIndex: function(array) {
        var obj = {};
        this.foreach(array, function(name) {
            obj[name] = true;
        });
        return obj;
    },
    forIn: function(obj, callback) {
        for (var z in obj) {
            callback(obj[z], z);
        }
    },
    foreach: function(obj, callback, start) {
        var cancelled;
        if (obj) {
            if (!(obj instanceof Array ||
                  (typeof (obj.length) == 'number' &&
                   (typeof (obj.callee) == "function" || (obj.item && typeof (obj.nodeType) == "undefined") && !obj.addEventListener && !obj.attachEvent)))) {
                obj = [obj];
            }
            for (var i = start || 0, l = obj.length; i < l; i++) {
                if (callback(obj[i], i)) {
                    cancelled = true;
                    break;
                }
            }
        }
        return !cancelled;
    },
    foreachScriptInfo: function(obj, callback) {
        var cancelled;
        if (obj) {
            for (var i = 0, l = obj.length; i < l; i++) {
                if (callback(getScriptInfo(obj[i]))) {
                    cancelled = true;
                    break;
                }
            }
        }
        return !cancelled;
    }
};

(function() {
    EasyTestScriptLoader.Initialize();
})();