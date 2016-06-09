(function() {
ASPx.Insp = function(obj) {
	alert(ASPx.GetObjInfo(obj));
}

ASPx.GetObjInfo = function(obj) {
	var array = [];
	for(var key in obj) {
	    if(key.indexOf("on") != 0 && key.indexOf("outer") != 0 && key.indexOf("inner") != 0) {
	        try{
			    var value = "" + eval("obj." + key);
			    if(value.indexOf("function") < 0)
				    array.push(" " + key + " = " + value);
		    }
		    catch(e){
		    }
		}
	}
	array.sort();
	return array.join("\t");
}

ASPx.GetObjProps = function(obj, propNames, namePrefix) {
    if(!namePrefix)
        namePrefix = "";
    var sb = [ ];
    for(var i = 0; i < propNames.length; i++) {
        var propName = propNames[i];
        sb.push(namePrefix + propName + " = " + String(obj[propName]) + ",\r\n");
    }
    return sb.join("").replace(/\r\n/g, "<br />\r\n");
}

var JSProfilerCallInfo = function(procName, args) {
    this.procName = procName;
    this.argumentsList = (typeof(args) != "undefined" && args != null && args.length > 0) ? args : [ ];
    this.callDate = new Date();
    this.exitDate = null;
    
    this.Exit = function() {
        this.exitDate = new Date();
    }
    this.GetEnterInfo = function() {
        return this.GetInfo(">>");
    }
    this.GetExitInfo = function() {
        if (this.exitDate == null)
            throw 'JSProfilerCallInfo Exit() method was not called.';
        return this.GetInfo("<<", true);
    }
    this.GetInfo = function(prefix, writeExecutionTime) {
        var info = "";
        info += prefix;
        info += "&nbsp;";
        info += "<span style=\"color: blue;\">";
        info += this.procName;
        info += "(";
        info += "<span style=\"color: #F757FA;\">";
        for(var i = 0; i < this.argumentsList.length; i++) {
            info += this.argumentsList[i];
            if (i < this.argumentsList.length - 1)
                info += ", ";
        }
        info += "</span>";
        info += ")";
        info += "</span>";
        if (writeExecutionTime)
            info += "&nbsp; (execution time: <span style=\"color: blue\">" + this.GetExecutionSeconds(this.callDate, this.exitDate) + " sec</span>)";
        return info;
    }
    this.GetExecutionTime = function() {
        return this.GetExecutionSeconds(this.callDate, this.exitDate);
    }    
    this.GetExecutionSeconds = function(callDate, exitDate) {
        return (exitDate.getTime() - callDate.getTime()) / 1000.0;
    }
}

var JSProfiler = {
    callStack: [ ],
    
    // API
    Enter: function(procName, args, needWriteResultToBody) {
        needWriteResultToBody = ASPx.IsExists(needWriteResultToBody) ? needWriteResultToBody : true;
        var callInfo = new JSProfilerCallInfo(procName, args);
        if (needWriteResultToBody)
            JSProfiler.WriteMessage(callInfo.GetEnterInfo());
        JSProfiler.callStack.push(callInfo);
    },
    Exit: function(needWriteResultToBody) {
        needWriteResultToBody = ASPx.IsExists(needWriteResultToBody) ? needWriteResultToBody : true;
        if (JSProfiler.callStack.length == 0)
            throw "CallStack is empty.";
        var callInfo = JSProfiler.callStack[JSProfiler.callStack.length - 1];
        callInfo.Exit();
        JSProfiler.callStack.pop();
        var exitInfo = callInfo.GetExitInfo();
        if (needWriteResultToBody)
            JSProfiler.WriteMessage(exitInfo);
        else
            return { message: exitInfo, executionTime: callInfo.GetExecutionTime() };
    },
    
    // Utils
    CreateIndentString: function() {
        var indent = "";
        for (var i = 0; i < JSProfiler.callStack.length; i++)
            indent += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
        return indent;
    },
    WriteError: function(message) {
        JSProfiler.WriteMessageCore(message, "red");
    },
    WriteWarning: function(message) {
        JSProfiler.WriteMessageCore(message, "#E8DD44");
    },
    Write: function(message) {
        JSProfiler.WriteMessage(message);
    },
    WriteMessage: function(message) {
        JSProfiler.WriteMessageCore(message, "green");
    },
    WriteMessageCore: function(message, colorStr) {
        var para = document.createElement("P");
        para.style.fontSize = "12px";
        para.style.margin = "1px 0";
        para.style.color = colorStr;
        para.style.whiteSpace = "nowrap";
        para.style.fontFamily = "Consolas, Arial, Tahoma";
        para.innerHTML = JSProfiler.CreateIndentString() + message;
        this.AddElementToDOM(para);
    },
    AddElementToDOM: function(element) {
        document.body.appendChild(element);
    }
};

ASPx.JSProfiler = JSProfiler;
})();