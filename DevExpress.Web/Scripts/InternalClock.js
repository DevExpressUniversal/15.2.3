/// <reference path="_references.js"/>

(function() {
var InternalClock = ASPx.CreateClass(ASPxClientControl, {
    IEMatrixFilter : "progid:DXImageTransform.Microsoft.Matrix(M11='1', M12='0', M21='0', M22='1', SizingMethod='auto expand')",
    
    constructor: function(name) {
        this.constructor.prototype.constructor.call(this, name);
        
        this.rectBag = [ ];
        this.initialized = false;
    },

    InlineInitialize: function() {
        ASPxClientControl.prototype.InlineInitialize.call(this);
        this.InitializeElements();
	},

    InitializeElements: function() {
        this.GetMainElement().style.position = "relative";
        ASPx.Evt.PreventImageDragging(this.GetClockFace());
        var hands = this.GetHands();
        for(var i = 0; i < hands.length; i++) {
            var hand = hands[i];
            if(!hand) continue;
            hand.style.position = "absolute";
            ASPx.Evt.PreventImageDragging(hand);
        }
    },

    GetClockFace: function() { return this.GetChildElement("D"); },
    GetHourHand: function() { return this.GetChildElement("H"); },
    GetMinutHand: function() { return this.GetChildElement("M"); },
    GetSecondHand: function() { return this.GetChildElement("S"); },
    GetHands: function() { return [ this.GetHourHand(), this.GetMinutHand(), this.GetSecondHand() ]; },

    AdjustControlCore: function() {
        this.EnsureRectBag();
    },

    SetDate: function(date) {
        if(this.date == date) return;
        this.date = date;
        this.EnsureRectBag();

        this.RotateHands();
	},

    RotateHands: function() {
        if(this.rectBag.length == 0) 
            return;
        var hands = this.GetHands();
        for(var i = 0; i < hands.length; i++) {
            var hand = hands[i];
            if(!hand) continue;
            var degree = this.GetDegree(this.date, i);
            if(ASPx.Browser.IE && ASPx.Browser.Version < 9)
                this.RotateOldIE(degree, hand, this.rectBag[i]);
            else 
                this.Rotate(degree, hand);
        }
    },

    GetDegree: function(date, handIndex) {
        if(!date)
            return 0;
        var hours = date.getHours();
        var minutes = date.getMinutes();
        var seconds = date.getSeconds();

        switch(handIndex) {
            case 0:
                return (hours * 60 + minutes) / 2;
            case 1:
                return minutes * 6;
            case 2:
                return seconds * 6;
        }
    },

    EnsureRectBag: function() {
        if(this.rectBag.length > 0 || !ASPx.IsElementDisplayed(this.GetMainElement()))
            return;
        this.PopulateSizeBag();
        this.InitializeHands();
        this.RotateHands();
    },

    PopulateSizeBag: function() {
        var clockFace = this.GetClockFace();
        var clockFaceCenterX = (clockFace.offsetWidth - ASPx.GetLeftRightBordersAndPaddingsSummaryValue(clockFace)) / 2;
        var clockFaceCenterY = (clockFace.offsetHeight - ASPx.GetTopBottomBordersAndPaddingsSummaryValue(clockFace)) / 2;

        var hands = this.GetHands();
        for(var i = 0; i < hands.length; i++) {
            var hand = hands[i];
            var rect = { };
            this.rectBag.push(rect);
            if(!hand) 
                continue;

            rect.fulcrumX = hand.offsetWidth / 2;
            rect.fulcrumY = clockFaceCenterY;

            rect.left = clockFaceCenterX - rect.fulcrumX;
            rect.top = 0;

            rect.width = hand.offsetWidth;
            rect.height = hand.offsetHeight;
        }
    },

    InitializeHands: function() {
        var hands = this.GetHands();
        for(var i = 0; i < hands.length; i++) {
            var hand = hands[i];
            if(!hand) continue;
            var rect = this.rectBag[i];
            hand.style.left = rect.left + "px";
            hand.style.top = rect.top + "px";

            if(ASPx.Browser.IE && ASPx.Browser.Version < 9)
                hand.style.filter = this.IEMatrixFilter;
            else
                hand.style[this.GetBrowserPrefix() + "ransformOrigin"] = rect.fulcrumX + "px " + rect.fulcrumY + "px";
        }
    },

    GetBrowserPrefix: function() {
        if(ASPx.Browser.IE && ASPx.Browser.Version >= 9)
            return "msT";
        if(ASPx.Browser.Firefox)
            return "MozT";
        if(ASPx.Browser.WebKitFamily)
            return "WebkitT";
        if(ASPx.Browser.Opera)
            return "OT";
        return "t";
    },

    Rotate: function(degree, hand) {
        hand.style[this.GetBrowserPrefix() + "ransform"] = "rotate(" + degree + "deg)";
    },

    RotateOldIE: function(degree, hand, rect) {
        degree = degree % 360;
        var angle = Math.PI * degree / 180;
        var cos = Math.cos(angle);
        var sin = Math.sin(angle);
        var matrix = { 
            M11 : cos, M12 : -sin, 
            M21 : sin, M22 : cos 
        };

        var resizeOffset = this.GetIEResizeOffset(Math.floor(degree / 90) + 1, rect.width, rect.height, sin, cos);
        var fulcrumOffset = this.GetIEFulcrumOffset(rect.fulcrumX, rect.fulcrumY, matrix);
        
        hand.style.left = (rect.left - resizeOffset.x - fulcrumOffset.x) + "px";
        hand.style.top = (rect.top - resizeOffset.y - fulcrumOffset.y) + "px";
           
        var filter = hand.filters.item(0);
        filter.M11 = matrix.M11;
        filter.M12 = matrix.M12;
        filter.M21 = matrix.M21;
        filter.M22 = matrix.M22;
    },

    GetIEResizeOffset: function (quadrant, width, height, sin, cos) {
        switch(quadrant) {
            case 1:
                return { x: height * sin, y: 0 };
            case 2:
                return { x: sin * height - cos * width, y: -height * cos };
            case 3:
                return { x: -width * cos, y: -sin * width - cos * height };
            case 4:
                return { x: 0, y: -width * sin };
        }
    },

    GetIEFulcrumOffset: function(x, y, matrix) {
        return {
            x: matrix.M11 * x + matrix.M12 * y - x, 
            y: matrix.M21 * x + matrix.M22 * y - y
        };
    }
});

ASPx.InternalClock = InternalClock;
})();