/// <reference path="_references.js"/>

(function () {
var ASPxClientRatingControl = ASPx.CreateClass(ASPxClientControl, {
    __isASPxRatingControl: true,

    INDEX_DEFAULT: 0,
    INDEX_USER: 1,
    INDEX_CHECKED: 2,
    INDEX_HOVER: 3,

    FILLPRECISION_EXACT: 0,
    FILLPRECISION_HALF: 1,
    FILLPRECISION_FULL: 2,

    constructor: function (name) {
        this.constructor.prototype.constructor.call(this, name);

        this.itemCount = 0;
        this.itemWidth = 0;
        this.itemHeight = 0;

        this.value = 0;
        this.readOnly = false;

        this.mainDiv = null;
        this.checkedDiv = null;
        this.hoverDiv = null;
        this.titles = [];
        this.fillPrecision = 0;
        this.hoverItemIndex = -1;

        this.hideSelectionOnHover = true;
        this.checkedDivWidth = "";
        this.ItemClick = new ASPxClientEvent();
        this.ItemMouseOver = new ASPxClientEvent();
        this.ItemMouseOut = new ASPxClientEvent();
    },

    SetDimensions: function (itemCount, itemWidth, itemHeight) {
        this.itemCount = itemCount;
        this.itemWidth = itemWidth;
        this.itemHeight = itemHeight;
    },
    Initialize: function () {
        this.constructor.prototype.Initialize.call(this);
        this.AddAnchors();
    },
    AddAnchors: function () {
        var div = this.GetMainDiv();
        if(div.lastChild.nodeName == "A")
            return;
        for(var i = 0; i < this.itemCount; i++)
            div.appendChild(this.CreateAnchor(i))
    },
    CreateAnchor: function (index) {
        var anchor = document.createElement("a");
        anchor.style.width = this.itemWidth + "px";
        anchor.style.height = this.itemHeight + "px";
        anchor.style[this.rtl ? "marginRight" : "marginLeft"] = index * this.itemWidth + "px";
        anchor.style.marginTop = -this.itemHeight + "px";
        anchor.style.display = "block";
        if(this.GetReadOnly())
                anchor.style.cursor = 'default';

        ASPx.Attr.SetAttribute(anchor, "DXIndex", index);
        ASPx.Attr.SetAttribute(anchor, "href", "javascript:void(0)");
        var title = this.GetTitle(index);
        if(title)
            ASPx.Attr.SetAttribute(anchor, "title", title);
        ASPx.Evt.AttachEventToElement(anchor, "click", function () { aspxRatingControlVote(this.name, index); }.aspxBind(this));
        return anchor;
    },
    GetTitle: function (index) {
        if(!this.GetEnabled())
            return null;
        if(this.titles && this.titles[index])
            return this.titles[index];
        if(this.toolTip)
            return this.toolTip;
        return null;
    },

    GetChildDiv: function (parent) {
        for(var i = 0; i < parent.childNodes.length; i++) {
            var child = parent.childNodes[i];
            if(child.tagName == "DIV")
                return child;
        }
    },
    GetMainDiv: function () {
        if(this.mainDiv == null)
            this.mainDiv = this.GetMainElement();
        return this.mainDiv;
    },
    GetCheckedDiv: function () {
        if(this.checkedDiv == null)
            this.checkedDiv = this.GetChildDiv(this.GetMainDiv());
        return this.checkedDiv;
    },
    GetHoverDiv: function () {
        if(this.hoverDiv == null)
            this.hoverDiv = this.GetChildDiv(this.GetCheckedDiv());
        return this.hoverDiv;
    },

    UpdateStateObject: function(){
        this.UpdateStateObjectWithObject({ readOnly: this.readOnly, value: this.value });
    },
    GetEnabled: function () {
        var disabled = ASPx.Attr.GetAttribute(this.GetMainElement(), "disabled");
        if(disabled && disabled === "disabled")
            return false;
        return true;
    },
    GetReadOnly: function () {
        if(!this.GetEnabled())
            return null;
        return this.readOnly;
    },
    SetReadOnly: function (readOnly) {
        if(!this.GetEnabled())
            return;
        this.readOnly = readOnly;
    },
    GetValue: function () {
        if(!this.GetEnabled())
            return null;
        return this.value;
    },
    SetValue: function (value, isUi) {
        if(!this.GetEnabled())
            return;
        if(value > this.itemCount)
            value = this.itemCount;
        this.value = value;
        this.UpdateCheckDiv(value, isUi);
        this.UpdateHoverDiv(-1);
    },
    UpdateCheckDiv: function (value, isUi) {
        var div = this.GetCheckedDiv();
        var index = isUi ? this.INDEX_USER : this.INDEX_CHECKED;
        div.style.backgroundPosition = "0 " + (-this.itemHeight * index) + "px";
        if(!isUi)
            value = this.QuantizeValue(value);
        div.style.width = this.itemWidth * value + "px";
    },
    QuantizeValue: function (input) {
        switch (this.fillPrecision) {
            case this.FILLPRECISION_EXACT:
                return input;
            case this.FILLPRECISION_FULL:
                return Math.round(input);
            case this.FILLPRECISION_HALF:
                return Math.round(input * 2) / 2;
        }
    },
    HandleVote: function (index) {
        if(this.hideSelectionOnHover)
            this.checkedDivWidth = "";
        if(this.GetReadOnly())
            return;
        this.SetValue(index + 1, true);
        var processOnServer = this.RaiseItemClick(index);
        if(processOnServer)
            this.SendPostBack(index);
    },

    HandleMouseMove: function (htmlEvent) {
        var index = this.GetItemIndexAtCursor(htmlEvent);
        if(index != -1) {
            if(this.hideSelectionOnHover)
                this.resetCheckedWidth();
            this.UpdateHoverDiv(index);
            this.hoverItemIndex = index;
            this.RaiseItemMouseOver(this.hoverItemIndex);
        }
    },

    HandleMouseOut: function () {
        this.UpdateHoverDiv(-1);
        if(this.hideSelectionOnHover)
            this.restoreCheckedWidth();
        this.RaiseItemMouseOut(this.hoverItemIndex);
        this.hoverItemIndex = -1;
    },

    GetItemIndexAtCursor: function (htmlEvent) {
        if(ASPx.Evt.GetEventSource(htmlEvent).nodeName == "A")
            return parseInt(ASPx.Evt.GetEventSource(htmlEvent).attributes.getNamedItem("DXIndex").value);
        return -1;
    },

    saveCheckedWidth: function () {
        if(this.GetCheckedDiv().style.width !== "0px")
            this.checkedDivWidth = this.GetCheckedDiv().style.width;
    },
    resetCheckedWidth: function () {
        this.saveCheckedWidth();
        this.GetCheckedDiv().style.width = "0px";
    },
    restoreCheckedWidth: function () {
        if(this.checkedDivWidth !== "")
            this.GetCheckedDiv().style.width = this.checkedDivWidth;
        this.checkedDivWidth = "";
    },

    UpdateHoverDiv: function (itemIndex) {
        this.GetHoverDiv().style.width = (itemIndex + 1) * this.itemWidth + "px";
    },

    Clear: function() {
        this.SetValue(0);
        return true;
    },

    RaiseItemClick: function (index) {
        var processOnServer = this.autoPostBack || this.IsServerEventAssigned("ItemClick");
        if(!this.ItemClick.IsEmpty()) {
            var args = new ASPxClientRatingControlItemClickEventArgs(processOnServer, index);
            this.ItemClick.FireEvent(this, args);
            processOnServer = args.processOnServer;
        }
        return processOnServer;
    },
    RaiseItemMouseOver: function (itemIndex) {
        if(!this.ItemMouseOver.IsEmpty()) {
            var args = new ASPxClientRatingControlItemMouseEventArgs(itemIndex);
            this.ItemMouseOver.FireEvent(this, args)
        }
    },
    RaiseItemMouseOut: function (itemIndex) {
        if(!this.ItemMouseOut.IsEmpty()) {
            var args = new ASPxClientRatingControlItemMouseEventArgs(itemIndex);
            this.ItemMouseOut.FireEvent(this, args)
        }
    }
});
ASPxClientRatingControl.Cast = ASPxClientControl.Cast;

var elementUnderCursor = null;
var activeRatingControl = null;
var DocMouseMoveHandler = function (htmlEvent) {
    var element = ASPx.Evt.GetEventSource(htmlEvent);
    if(!ASPx.IsValidElement(elementUnderCursor))
        elementUnderCursor = null;
    if(element == elementUnderCursor)
        return;
    elementUnderCursor = element;
    for(var i = 0; i < 3 && element != null; i++) {
        if(element.id) {
            var obj = ASPx.GetControlCollection().Get(element.id);
            if(obj != null && obj.__isASPxRatingControl && obj.GetEnabled()) {
                if(activeRatingControl != null)
                    activeRatingControl.HandleMouseOut();
                activeRatingControl = obj;
                if(!obj.GetReadOnly())
                    obj.HandleMouseMove(htmlEvent);
                return;
            }
        }
        element = element.parentNode;
    }
    if(activeRatingControl != null && activeRatingControl.GetEnabled()) {
        activeRatingControl.HandleMouseOut();
        activeRatingControl = null;
    }
};

function aspxRatingControlVote(name, index) {
    var control = ASPx.GetControlCollection().Get(name);
    if(control)
        control.HandleVote(index);
}
var ASPxClientRatingControlItemClickEventArgs = ASPx.CreateClass(ASPxClientProcessingModeEventArgs, {
    constructor: function (processOnServer, index) {
        this.constructor.prototype.constructor.call(this, processOnServer);
        this.index = index;
    }
});
var ASPxClientRatingControlItemMouseEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
    constructor: function (index) {
        this.constructor.prototype.constructor.call(this);
        this.index = index;
    }
});

var handlerAssigned = false;
if(!handlerAssigned && !ASPx.Browser.TouchUI) {
    ASPx.Evt.AttachEventToDocument("mousemove", DocMouseMoveHandler);
    handlerAssigned = true;
}

window.ASPxClientRatingControl = ASPxClientRatingControl;
window.ASPxClientRatingControlItemClickEventArgs = ASPxClientRatingControlItemClickEventArgs;
window.ASPxClientRatingControlItemMouseEventArgs = ASPxClientRatingControlItemMouseEventArgs;
})();