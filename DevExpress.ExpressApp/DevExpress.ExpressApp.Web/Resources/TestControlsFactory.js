function SetTimeoutDelegate(script, timeoutExecute, setTimeoutDelegateID) {
    this.script = script;
    this.timeoutExecute = timeoutExecute;
    this.Delegate = function() {
        TestControls.Instance.executingScript = script;
        if (typeof (script) == 'function') {
            script.call();
        } else {
            eval(script);
        }
        timeoutExecute.call(this, setTimeoutDelegateID);
        delete TestControls.Instance.executingScript;
    };
}

function GetAsString(someValue) {
    if (someValue === null) {
        return "null";
    }
    else if (someValue === undefined) {
        return "undefined";
    }
    else {
        return someValue.toString();
    }
}



function TestControls(keyHashCode) {
    this.testControlsDescriptions = new Array();
    this.setTimeoutDelegateID = 1;
    this.timeouts = new Array();
    this.timeoutStacks = new Array();
    this.IsCallBack = false;
	this.IsInited = false;    
	this.IsShowModalWindow = false;
	this.IsSubmitting = false;
	this.IsFrameLoading = false;
	this.WebForm_ExecuteCallback_Old;
	this.WebForm_DoCallback_Old;
    this.oldWindowTimeout;
    this.oldWindowclearTimeout;
    this.TimeoutCount = 0;
    
    this.IsCallBackExecuted;
    this.keyHashCode = keyHashCode;
    this.traceMessages = "";
    this.Log = function (str) {
        this.traceMessages += str + "\r\n";
    };
    this.GetTraceMessages = function () {
        return this.traceMessages;
    };
    this.Log("  function TestControls, TimeoutCount = 0");
	this.OnSubmit = function() {
	    TestControls.Instance.IsSubmitting = true;
        return true;
    };
    this.onsubmit = function() {
        var result = true;
        if(this.testControlOnSubmit) {
            result = this.testControlOnSubmit();
        }
        TestControls.Instance.IsSubmitting = true;
        return result;
    };
    this.TimeOutExecuted = function(setTimeoutDelegateID) {
        TestControls.Instance.Log(">TimeOutExecuted, setTimeoutDelegateID = " + GetAsString(setTimeoutDelegateID));
        TestControls.Instance.Log("  TimeOutExecuted, initial TimeoutCount=" + GetAsString(TestControls.Instance.TimeoutCount));
        TestControls.Instance.Log("TestControls.Instance.TimeoutCount=" + TestControls.Instance.TimeoutCount);
        if (TestControls.Instance.timeouts[setTimeoutDelegateID]) {
            TestControls.Instance.TimeoutCount--;
            TestControls.Instance.Log("TestControls.Instance.TimeoutCount=" + TestControls.Instance.TimeoutCount);
            if (TestControls.Instance.TimeoutCount <= 0) {
                TestControls.Instance.TimeoutCount = 0;
                if (TestControls.Instance.IsCallBackExecuted) {
                    TestControls.Instance.Log("setting TestControls.Instance.IsCallBack  to FALSE ");
                    TestControls.Instance.IsCallBack = false;
                }
            }
            TestControls.Instance.timeouts[setTimeoutDelegateID] = 0;
            TestControls.Instance.timeoutStacks[setTimeoutDelegateID] = "executed";
            TestControls.Instance.Log("  TimeOutExecuted, current TimeoutCount=" + GetAsString(TestControls.Instance.TimeoutCount));
        }
        else {
            TestControls.Instance.Log("  TimeOutExecuted, TestControls.Instance.timeouts doesn't contain entry. Existing entries:");
            for (var timeoutsEntryKey in TestControls.Instance.timeouts) {
                TestControls.Instance.Log("  TimeOutExecuted, timeoutsEntryKey = " + timeoutsEntryKey.toString() + ", val=" + TestControls.Instance.timeouts[timeoutsEntryKey]);
            }
        }
        TestControls.Instance.Log("<TimeOutExecuted, setTimeoutDelegateID = " + GetAsString(setTimeoutDelegateID));
    };
    this.TimeoutCountTest = function () {
        return this.TimeoutCount == 0 || typeof window == "undefined" || window.closed;
    }
    this.GetCurrentTimeoutList = function () {
        var result = "TimeoutCount=" + this.TimeoutCount.toString() + "\r\n";
        result += "TestControls.Instance.timeouts=" + TestControls.Instance.timeouts.length.toString() + "\r\n";
        
        for (var callStackEntryKey in TestControls.Instance.timeoutStacks) {
            result += "\r\n" + callStackEntryKey + " - " + TestControls.Instance.timeoutStacks[callStackEntryKey];
        }
        return result;
    }
    var enabledubugging = false;
    this.Timeout = function (handler, timeout) {
        if (timeout === undefined) {
            timeout = 0;
        }
        var handlerStr = handler.toString();
        TestControls.Instance.Log(">Timeout, handler = " + handlerStr + ", timeout=" + timeout.toString());
        TestControls.Instance.Log("  Timeout, initial TimeoutCount=" + GetAsString(TestControls.Instance.TimeoutCount));
        var timeoutID;
        var isASPxControlResizeManager_CheckResize = (handlerStr.indexOf("function() {\r\n  ControlResizeManager.CheckResize();") > -1); //T149743, MainDemo_Web_Scheduler_DeleteChangedOccurrence failed
        var isASPxClientTemporaryCache_Get = (handlerStr.indexOf("function() {\r\n  return func.apply(scope, arguments);\r\n }") > -1); //Function.prototype.aspxBind, AppStudio_Web_ActionAsModeWithConfirmation.ets failed

        //FeatureCenter_PropertyEditors_FileAttachment_UploadDownload
        //13 - handler: function() {
        //    var menu = aspxGetMenuCollection().Get(name);
        //    if(menu != null)
        //        menu.OnItemOutTimer();
        //}
        //callstack: function(name, timeout) {
        //    this.disappearTimerID = window.setTimeout(function() {
        //        var menu = aspxGetMenuCollection().Get(name);
        //        if(menu != null)
        //            menu.OnItemOutTimer();
        //    }, timeout);
        //}

        //any function template can become outdated. this is an expected case: make a new function template and include it in addition to an old template.
        var isASPxMenu_SetDisappearTimer_AnonymousTimeoutFunction = ((handlerStr.indexOf("var menu = aspxGetMenuCollection().Get(name);") > -1) && (handlerStr.indexOf("menu.OnItemOutTimer();") > -1));
        var isASPxMenu_OnItemOverTimer_AnonymousTimeoutFunction_T215453 = ((handlerStr.indexOf("var menu = aspxGetMenuCollection().Get(name);") > -1) && (handlerStr.indexOf("menu.OnItemOverTimer(indexPath);") > -1));
        var isVS2013_BrowserLink_Function_T261174 = (handlerStr.indexOf("function(){f(") > -1) || (handlerStr.indexOf("function(){var f=[]") > -1);

        if ((TestControls.Instance.executingScript == handler) || isASPxControlResizeManager_CheckResize || isASPxClientTemporaryCache_Get
            || isASPxMenu_SetDisappearTimer_AnonymousTimeoutFunction || isASPxMenu_OnItemOverTimer_AnonymousTimeoutFunction_T215453 || isVS2013_BrowserLink_Function_T261174) {
            TestControls.Instance.Log("  Timeout, skip handler await as a known infinite handler");
            timeoutID = window.oldWindowTimeout(handler, timeout);
        }
        else {
            TestControls.Instance.Log("  Timeout, current TestControls.Instance.setTimeoutDelegateID = " + TestControls.Instance.setTimeoutDelegateID.toString());
            var timeoutDelegate = new SetTimeoutDelegate(handler, TestControls.Instance.TimeOutExecuted, TestControls.Instance.setTimeoutDelegateID);
            timeoutID = window.oldWindowTimeout(timeoutDelegate.Delegate, timeout);
            TestControls.Instance.Log("  Timeout, current TestControls.Instance.TimeoutCount = " + TestControls.Instance.TimeoutCount.toString());
            TestControls.Instance.TimeoutCount++;
            TestControls.Instance.Log("  Timeout, TestControls.Instance.TimeoutCount++, current TimeoutCount=" + GetAsString(TestControls.Instance.TimeoutCount));
            TestControls.Instance.Log("  Timeout, TestControls.Instance.timeouts = " + TestControls.Instance.timeouts.length);
            TestControls.Instance.timeouts[TestControls.Instance.setTimeoutDelegateID] = timeoutID;
            TestControls.Instance.Log("  Timeout, TestControls.Instance.timeoutStacks = " + TestControls.Instance.timeoutStacks.length);
            var stackInfo = "";
            try {
                stackInfo += "handler: " + handler.toString();
            }
            catch(ex) {
                stackInfo += "handler: exception occured, " + ex.toString();
            }
            stackInfo += "\r\n"
            try {
                stackInfo += "callstack: " + arguments.callee.caller.toString();
            }
            catch (ex) {
                stackInfo += "callstack: " + ex.toString();
            }
            TestControls.Instance.timeoutStacks[TestControls.Instance.setTimeoutDelegateID] = stackInfo;
            TestControls.Instance.Log("  Timeout, TestControls.Instance.setTimeoutDelegateID++");
            TestControls.Instance.setTimeoutDelegateID++;
            TestControls.Instance.Log("  Timeout, TestControls.Instance.setTimeoutDelegateID = " + TestControls.Instance.setTimeoutDelegateID);
        }
        TestControls.Instance.Log("<Timeout, handler = " + handler.toString() + ", timeout=" + timeout.toString());
        return timeoutID;
    };
    this.ClearTimeout = function (iTimeoutID) {
        TestControls.Instance.Log(">ClearTimeout, iTimeoutID = " + GetAsString(iTimeoutID));
        TestControls.Instance.Log("  ClearTimeout, initial TimeoutCount=" + GetAsString(TestControls.Instance.TimeoutCount));
        var setTimeoutDelegateID = -1;
        for(var id in TestControls.Instance.timeouts) {
            if (TestControls.Instance.timeouts[id] == iTimeoutID) {
                setTimeoutDelegateID = id;
                TestControls.Instance.Log("  ClearTimeout, entry is found. setTimeoutDelegateID = " + setTimeoutDelegateID);
                break;
            }
        }
        TestControls.Instance.TimeOutExecuted(setTimeoutDelegateID);
        window.oldWindowClearTimeout(iTimeoutID);
        TestControls.Instance.Log("  ClearTimeout, current TimeoutCount=" + GetAsString(TestControls.Instance.TimeoutCount));
        TestControls.Instance.Log("<ClearTimeout, iTimeoutID = " + GetAsString(iTimeoutID));
    }
    this.ExecuteCallback = function (callbackObject) {
        TestControls.Instance.Log(">>ExecuteCallback ");
        TestControls.Instance.Log("TestControls.Instance.TimeoutCount=" + TestControls.Instance.TimeoutCount);
        TestControls.Instance.TimeoutCount = 0;
        TestControls.Instance.Log("TestControls.Instance.TimeoutCount=" + TestControls.Instance.TimeoutCount);
        TestControls.Instance.timeouts = new Array();
        TestControls.Instance.timeoutStacks = new Array();
        TestControls.Instance.WebForm_ExecuteCallback_Old(callbackObject);
        TestControls.Instance.Log("TestControls.Instance.TimeoutCount=" + TestControls.Instance.TimeoutCount);
        if (TestControls.Instance.TimeoutCount == 0) {
            TestControls.Instance.Log("setting TestControls.Instance.IsCallBack to FALSE" );
            TestControls.Instance.IsCallBack = false;
        }
        TestControls.Instance.IsCallBackExecuted = true;
        TestControls.Instance.Log("<<ExecuteCallback ");
    };
    this.DoCallback = function (eventTarget, eventArgument, eventCallback, context, errorCallback, useAsync) {
        TestControls.Instance.Log(">>DoCallback");
        TestControls.Instance.Log("setting TestControls.Instance.IsCallBack to TRUE");
		TestControls.Instance.IsCallBack = true;
		TestControls.Instance.IsCallBackExecuted = false;
		TestControls.Instance.WebForm_DoCallback_Old(eventTarget, eventArgument, eventCallback, context, errorCallback, useAsync)
		TestControls.Instance.Log("<<DoCallback");
    };
    this.CheckFrameLoading = function() {
        if (TestControls.Instance.IsFrameLoading) {
            return true;
        }
        else {
            if (typeof window != "undefined" && window.parent && window.parent.TestControls) {
                return window.parent.TestControls.Instance.IsFrameLoading;
            }
        }
        return false;
    }
    this.FrameLoading = function() {
        TestControls.Instance.IsFrameLoading = true;
    };
    this.FrameLoaded = function() {
        TestControls.Instance.IsFrameLoading = false;
    };
    this.PopupClosed = function () {
        TestControls.Instance.Log(">>PopupClosed");
        TestControls.Instance.IsFrameLoading = false;
        TestControls.Instance.Log("setting TestControls.Instance.IsCallBack to FALSE");
        TestControls.Instance.IsCallBack = false;
        TestControls.Instance.Log("TestControls.Instance.TimeoutCount=" + TestControls.Instance.TimeoutCount);
        TestControls.Instance.Log("setting TestControls.Instance.TimeoutCount to 0");
        TestControls.Instance.TimeoutCount = 0;
        TestControls.Instance.timeouts = new Array();
        TestControls.Instance.timeoutStacks = new Array();
        TestControls.Instance.Log("<<PopupClosed");
    };
    TestControls.prototype.Instance = this;
    for(i=0; i < window.document.forms.length; i++){
        form = window.document.forms.item(i);
        if(form.attachEvent) {
            form.attachEvent('onsubmit', this.OnSubmit);
            form.testControlOnSubmit = form.onsubmit;
            form.onsubmit = this.onsubmit;
        }
    }
	if(window.WebForm_DoCallback && window.WebForm_ExecuteCallback) {
		this.WebForm_DoCallback_Old = window.WebForm_DoCallback;
		window.WebForm_DoCallback = this.DoCallback;
		this.WebForm_ExecuteCallback_Old = window.WebForm_ExecuteCallback;
		window.WebForm_ExecuteCallback = this.ExecuteCallback;
	}
	window.oldWindowTimeout = window.setTimeout;
	window.oldWindowClearTimeout = window.clearTimeout;
	window.setTimeout = this.Timeout;
	this.Log("  function TestControls, window.setTimeout = this.Timeout");
	window.clearTimeout = this.ClearTimeout;
	window.FrameLoading = this.FrameLoading;
	window.FrameLoaded = this.FrameLoaded;
	window.PopupClosed = this.PopupClosed;
	this.DoAjaxRequest = function(strURL, queryString) {
        // Mozilla/Safari
        var xmlHttpReq = null;
        if(window.XMLHttpRequest) {
            xmlHttpReq = new XMLHttpRequest();
        }
        // IE
        else if(window.ActiveXObject) {
            xmlHttpReq = new ActiveXObject("Microsoft.XMLHTTP");
        }
        var localURL = window.location.protocol + "//" + window.location.host;
        var pathname = window.location.pathname.split("/");
        for (var i = 1; i < pathname.length - 1; i++) {
            localURL += "/" + pathname[i];
        }
        localURL += strURL;
        xmlHttpReq.open('GET', localURL + "?" + queryString, false);
        xmlHttpReq.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xmlHttpReq.send(null);
        var result = null;
        if (xmlHttpReq.responseText && xmlHttpReq.responseText != "") {
            eval('result = ' + xmlHttpReq.responseText + ';');
        }
        return result;
	};
	this.TestControlsHandlerUrl = "/DXX.axd";
	this.FindControlsFullName = function(controlType, caption, fullCaption) {
	    return this.DoAjaxRequest(this.TestControlsHandlerUrl, "handlerName=TestControls&method=FindControlsFullName&keyHashCode=" + this.keyHashCode + "&controlType=" + this.UrlEncode(controlType) + "&caption=" + this.UrlEncode(caption) + "&fullCaption=" + this.UrlEncode(fullCaption));
    };
    this.FindControl = function(controlType, caption, fullCaption) {
        var obj = this.DoAjaxRequest(this.TestControlsHandlerUrl, "handlerName=TestControls&method=FindControl&keyHashCode=" + this.keyHashCode + "&controlType=" + this.UrlEncode(controlType) + "&caption=" + this.UrlEncode(caption) + "&fullCaption=" + this.UrlEncode(fullCaption));
        if(!IsNull(obj) && obj.id == undefined && obj.IsVisible == undefined){
            return obj;
        }
        if (!IsNull(obj) && obj.id && (obj.id == 'unknown' || obj.IsVisible())) {
            return obj;
        }
        else {
            return null;
        }
    };
    this.GetTestControlsDescriptions = function() {
        return this.DoAjaxRequest(this.TestControlsHandlerUrl, "handlerName=TestControls&method=GetTestControlsDescriptions&keyHashCode=" + this.keyHashCode);
    };
    this.CreateControl = function(className, clientID, fullCaption) {
        var result = null;
        eval('result = new ' + className + '("' + clientID + '", "' + fullCaption + '");');
        return result;
    };
    this.UrlEncode = function(value) {
        if(value == undefined) {
            return '';
        }
        else {
            return encodeURIComponent(value);
        }
    };
	this.ExecuteScript = function(scriptText) {
	    this.AddScript(scriptText, "scriptFunc");
	    var result = this.scriptFunc();
	    if (IsNull(result)) {
	        return null;
	    }
	    else {
	        return result;
	    }
	};
	this.AddScript = function(scriptText, functionName, params) {
	    if (functionName != "") {
	        if (params) {
	            eval('this.' + functionName + ' = function(' + params + ') {' + scriptText + '}');
	        } else {
	            eval('this.' + functionName + ' = function() {' + scriptText + '}');
	        }
	    } else {
	        eval(scriptText);
	    }
	};
}

function IsNull(obj) {
	return obj == null || obj == 'undefined' || obj == '';
}

function TestControlBase(id, caption) {
	this.id = id;
	this.caption = caption;
	this.targetErrorControl = this;
	this.error = null;
    this.operationError = false;
    this.callStack = '';
    this.traceMessages = null;
    this.CallWithLog = function(functionName, requireInit, functionHandler) {
        this.LogEntry(functionName);
        try {
            if(requireInit) {
                this.InitControl();
            }
            return functionHandler.call(this);
        }
        catch(e) {
            e.message = 'ClassName: \'' + this.className + '\'\r\n' + e.message;
            e.description = 'ClassName: \'' + this.className + '\'\r\n' + e.description;
            if(e.operationError) {
                this.LogOperationError(e.message);
            }
            else if(!this.operationError) {
                this.LogError(e.message);
                throw e;
            }
        }
        finally {
            this.LogExit(functionName);
        }
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
		if(this.error == null) {
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
	    if(this.error) {
	        return false;
	    }
	    return true;
	}
	this.InitControl = function() {
	    if(!this.control) {
	        this.control = document.getElementById(this.id.replace(/\$/g, '_'));
	        if(!this.control) {
		        this.LogOperationError('Cannot retrieve an element. id= "' + this.id + '" name="' + this.caption + '"');
		        return;
	        }
	    }
	}
	this.CompareString = function(actualString, patternString) {
	    var items = patternString.split("*");
	    if (items.length == 1) {
	        return (patternString == actualString);
	    }
	    var pos = 0;
	    for (count = 0; count < items.length; count++) {
	        if (count == 0) {
	            if ((pos = actualString.indexOf(items[count], pos)) != 0)
	                return false;
	        }
	        else {
	            if ((pos = actualString.indexOf(items[count], pos)) == -1)
	                return false;
	        }
	        pos = pos + items[count].length;
	    }
	    return true;
	} 
	TestControlBase.prototype.baseInitControl = this.InitControl;
	TestControlBase.prototype.baseIsVisible = this.IsVisible;
}

function TestColumn(fieldName, editorTestClassNameEditMode, editorTestClassNameViewMode, editorId, indexInGrid, columnCaption) {
    this.fieldName = fieldName;
    this.editorTestClassNameEditMode = editorTestClassNameEditMode;
    this.editorTestClassNameViewMode = editorTestClassNameViewMode;
    this.editorId = editorId;
    this.indexInGrid = indexInGrid;
    this.columnCaption = columnCaption;
    this.CreateTestControlWithId = function (id, viewModeOnly) {
        if (id) {
            var editorTestClassName = editorTestClassNameEditMode;
            if ((viewModeOnly == undefined && id.search(/(Edit|Edit_DropDown)[0-9]*$/g) == -1) || viewModeOnly == "ViewMode") {
                editorTestClassName = editorTestClassNameViewMode;
            }
            if (editorTestClassName && editorTestClassName != '') {
                return eval("new " + editorTestClassName + "('" + id + "', '" + this.fieldName + "')");
            }
        }
        return null;
    }
}

function TestBatchCellEditor(editorId, rowIndex, fieldName) {
    this.editorId = editorId;
    this.rowIndex = rowIndex;
    this.columnCaption = fieldName;
    this.error = null;
    this.SetText = function (value) {
        var clientGridView = window[editorId];
        if (clientGridView) {
            clientGridView.batchEditApi.SetCellValue(rowIndex, fieldName, value);
        }
    }
    this.IsEnabled = function () { return true;}
}
function TestColumns(gridId) {
    this.gridId = gridId;
    this.columns = new Array();
    this.AddColumn = function(fieldName, editorTestClassNameEditMode, editorTestClassNameViewMode, editorId, columnIndex, columnCaption) {
        var index = this.GetColumnIndexByColumnCaption(columnCaption);
        if (index == -1) {
            this.columns[this.columns.length] = new TestColumn(fieldName, editorTestClassNameEditMode, editorTestClassNameViewMode, editorId, columnIndex, columnCaption);
        }
        else {
            this.columns[index].editorTestClassNameEditMode = editorTestClassNameEditMode;
            this.columns[index].editorTestClassNameViewMode = editorTestClassNameViewMode;
            this.columns[index].editorId = editorId;
            this.columns[index].indexInGrid = columnIndex;
            this.columns[index].columnCaption = columnCaption;
        }
    }
    this.GetColumnFieldNameByColumnCaption = function(columnCaption) {
		for(var i = 0; i < this.columns.length; i++) {
			if(this.columns[i].columnCaption == columnCaption) {                
				return this.columns[i].fieldName;
			}
		}
		return "";
    }
    this.GetColumnByColumnCaption = function (columnCaption) {
        for (var i = 0; i < this.columns.length; i++) {
            if (this.columns[i].columnCaption == columnCaption) {
                return this.columns[i];
            }
        }
    }
    this.GetColumnIndexByColumnCaption = function(columnCaption) {
		var index = -1;
		for(var i = 0; i < this.columns.length; i++) {
		    if (this.columns[i].columnCaption == columnCaption) {                
				index = parseInt(this.columns[i].indexInGrid);
                break;
			}
		}
		return index;
    }
    this.GetColumnIndexByFieldName = function(fieldName) {
        var index = -1;
        for(var i = 0; i < this.columns.length; i++) {
            if(this.columns[i].fieldName == fieldName) {                
                index = i;
                break;
            }
        }
        return index;
    }
    this.FindColumnByFieldName = function(fieldName) {
        var index = this.GetColumnIndexByFieldName(fieldName);
        if(index == -1) {
            return null;
        }
        else {
            return this.columns[index];
        }
    }
    this.FindColumnByIndexInGrid = function(indexInGrid) {        
        for(var i = 0; i < this.columns.length; i++) {
            if(this.columns[i].indexInGrid == indexInGrid) {                
                return this.columns[i];
            }
        }
        return null;
    }
}