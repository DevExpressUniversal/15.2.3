(function() {
var statusInfosQueue = [];

var SchedulerStatusInfoType = { };
SchedulerStatusInfoType.Exception = "Exception";
SchedulerStatusInfoType.Error = "Error";
SchedulerStatusInfoType.Warning= "Warning";
SchedulerStatusInfoType.Info = "Info";

ASPx.SchedulerRefreshStatusInfos = function(name) {
    AfterInitializeStatusInfos();
    var statusInfo = ASPx.GetControlCollection().Get(name);
    if (ASPx.IsExists(statusInfo)) {
        statusInfo.RefreshStatusInfos();        
    }   
}
function AfterInitializeStatusInfos() {
    var count = statusInfosQueue.length;
    if(count == 0)
        return;
    for(var i = count - 1; i >= 0; i--) {
        var statusInfo = statusInfosQueue[i];
        var scheduler = ASPx.GetControlCollection().Get(statusInfo.schedulerControlId);
        if(ASPx.IsExists(scheduler))
            scheduler.statusInfoManager.AddStatusInfo(statusInfo);
            
    }
    statusInfosQueue = [];
}

var SchedulerStatusInfoManager = ASPx.CreateClass(ASPx.SchedulerRelatedControlBase, {   
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        this.statusInfos = [];
        this.visibleStatusInfo = null;
        this.image = {};
        
        this.ClearInfo();
    },
    Initialize: function() {
        this.constructor.prototype.Initialize.call(this);
        var scheduler = ASPx.GetControlCollection().Get(this.schedulerControlId);
        if (ASPx.IsExists(scheduler)) {
            if(ASPx.IsExists(scheduler.statusInfoManager)) {
                //SchedulerStatusInfoManager was recreate during callback.
                //Scheduler has reference to an old object, and we copy it content
                this.statusInfos = scheduler.statusInfoManager.statusInfos;
                this.visibleStatusInfo = scheduler.statusInfoManager.visibleStatusInfo;
                this.contentDiv = scheduler.statusInfoManager.contentDiv;
                this.subjectSpan = scheduler.statusInfoManager.subjectSpan;
                this.detailInfoSpan = scheduler.statusInfoManager.detailInfoSpan;
                this.image = scheduler.statusInfoManager.image;
                this.visibleImage = scheduler.statusInfoManager.visibleImage;
                this.detailInfoLink = scheduler.statusInfoManager.detailInfoLink;
            }
            else {
                this.contentDiv =  ASPx.GetElementById(this.name + "_contentDiv");
                this.subjectSpan = ASPx.GetElementById(this.name + "_subject");
                this.detailInfoSpan = ASPx.GetElementById(this.name + "_detailInfo");
                this.detailInfoLink = ASPx.GetElementById(this.name + "_detailInfoLink");
                this.image[SchedulerStatusInfoType.Exception] = ASPx.GetElementById(this.name + "_exceptionImg");
                this.image[SchedulerStatusInfoType.Error] = ASPx.GetElementById(this.name + "_errorImg");
                this.image[SchedulerStatusInfoType.Warning] = ASPx.GetElementById(this.name + "_warningImg");
                this.image[SchedulerStatusInfoType.Info] = ASPx.GetElementById(this.name + "_infoImg");
                this.visibleImage = null;                
                //do not use ASPx.SchedulerGlobals.RecycleNode(this.contentDiv);
                this.contentDiv.parentNode.removeChild(this.contentDiv);
            }
            scheduler.statusInfoManager = this;
        }
        var func = new Function("ASPx.SchedulerRefreshStatusInfos('" + this.name + "');");        
        window.setTimeout(func, 1);
    },
    SetInfo: function(imageType, subject, detail) {
        this.subject = subject;
        this.detailInfo = detail;
        this.imageType = imageType;
    },
    ClearInfo: function() {
        this.SetInfo("", "", "");
    },
    AddStatusInfo: function(statusInfo) {
        this.statusInfos.push(statusInfo);        
    },
    Clear: function() {
        this.ClearInfo();
        this.RefreshStatusInfos();        
    },    
    RegisterScriptsRestartHandler: function() {    
        var func = new Function("ASPx.SchedulerRefreshStatusInfos('" + this.name + "');");        
        ASPx.AddScriptsRestartHandler(this.name, func);
    },
    RefreshStatusInfos: function() {
        var statusInfo = this.GetStatusInfo();
        if(ASPx.IsExists(this.visibleStatusInfo))
            this.visibleStatusInfo.HideContent();
        if(ASPx.IsExists(statusInfo) && (this.subject != "" || this.detailInfo != "")) {
            this.subjectSpan.innerHTML = this.subject;
            this.detailInfoSpan.innerHTML = this.detailInfo;
            ASPx.SetElementDisplay(this.detailInfoLink, this.detailInfo != "");
            if(ASPx.IsExists(this.visibleImage))
                ASPx.SetElementDisplay(this.visibleImage, false);
            this.visibleImage = this.image[this.imageType];
            if(ASPx.IsExists(this.visibleImage))
                ASPx.SetElementDisplay(this.visibleImage, true);            
            statusInfo.SetContent(this.contentDiv);
            this.visibleStatusInfo = statusInfo;            
        }
    },
    GetStatusInfo: function() {
        var maxPriority = -1;
        var result = null;
        
        for(var i = this.statusInfos.length - 1; i >= 0; i--) {
            var statusInfo = this.statusInfos[i];
            if(statusInfo.IsVisible()) {
                if(statusInfo.priority > maxPriority) {
                    maxPriority = statusInfo.priority;
                    result = statusInfo;
                }
            }
            else
                ASPx.Data.ArrayRemoveAt(this.statusInfos, i);
        }
        return result;        
    },
    ShowExceptionInfo: function(msg) {
        var subjectLengthEnd = msg.indexOf(",");
        var detailInfoLengthEnd = msg.indexOf("|");
        var subjectLength = parseInt(msg.substr(0, subjectLengthEnd));
        var detailInfoLength = parseInt(msg.substr(subjectLengthEnd + 1, detailInfoLengthEnd - subjectLengthEnd - 1));
        var subject = msg.substr(detailInfoLengthEnd + 1, subjectLength);
        var detailInfo = msg.substr(detailInfoLengthEnd + subjectLength + 1, detailInfoLength);
        this.SetInfo(SchedulerStatusInfoType.Exception, subject, detailInfo);
    },
    IsDOMDisposed: function () {
        if (this.contentDiv) {
            var itemInDOM = ASPx.GetElementById(this.name + "_contentDiv");
            return !ASPx.IsExistsElement(itemInDOM);
        }
        return true;
    },
    OnDispose: function () {
        ASPxClientControl.prototype.OnDispose.call(this);
        this.contentDiv = null;
        this.subjectSpan = null;
        this.detailInfoSpan = null;
        this.detailInfoLink = null;
        this.image = [];
        this.visibleImage = null;
    }
});

var SchedulerStatusInfo = ASPx.CreateClass(ASPxClientControl, {
    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);
        this.priority = 0;
        this.currentContent = null;
    },
    Initialize: function () {
        this.constructor.prototype.Initialize.call(this);
        this.contentDiv = ASPx.GetElementById(this.name + "_mainDiv");
        this.outerContainer = this.isInsideRow ? this.contentDiv.parentNode.parentNode : this.contentDiv;
        statusInfosQueue.push(this);
    },
    SetContent: function (content) {
        this.HideContent();
        var newContent = content.cloneNode(true);
        this.contentDiv.appendChild(newContent);
        ASPx.SetElementDisplay(this.outerContainer, true);
        this.currentContent = newContent;
        this.EnsureVisible(this.currentContent);
    },
    EnsureVisible: function (content) {
        var currentYPosition = ASPx.GetAbsolutePositionY(content);
        var scrollTop = ASPx.GetDocumentScrollTop();
        var clientHeight = ASPx.GetDocumentClientHeight();
        if (currentYPosition < scrollTop || scrollTop + clientHeight < currentYPosition)
            ASPx.SetDocumentScrollTop(currentYPosition);
    },
    HideContent: function () {
        if (ASPx.IsExists(this.currentContent)) {
            var parentNode = this.currentContent.parentNode;
            if (ASPx.IsExists(parentNode)) {
                //ASPx.SchedulerGlobals.RemoveChildFromParent(parentNode, this.currentContent);
                parentNode.removeChild(this.currentContent);

            }
        }
        if (ASPx.IsExists(this.contentDiv))
            ASPx.SetElementDisplay(this.outerContainer, false);
        this.currentContent = null;
    },
    IsVisible: function () {
        this.contentDiv = ASPx.GetElementById(this.name + "_mainDiv"); //Find it again every time
        if (ASPx.IsExists(this.contentDiv)) {
            this.outerContainer = this.isInsideRow ? this.contentDiv.parentNode.parentNode : this.contentDiv;
            return true;
        }
        return false;
    }
});

ASPx.SchedulerStatusInfoManager = SchedulerStatusInfoManager;
ASPx.SchedulerStatusInfo = SchedulerStatusInfo;
})();