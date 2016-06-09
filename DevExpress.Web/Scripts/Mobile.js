/// <reference path="_references.js"/>    

(function(){
    var SCROLLBAR_CLASSNAMES = {
        VERTICAL: "dxTouchVScrollHandle",
        HORIZONTAL: "dxTouchHScrollHandle",
        SHOWN_VERTICAL: "dxTouchScrollHandleVisible",
        SHOWN_HORIZONTAL: "dxTouchScrollHandleVisible"
    }

    ASPx.Evt.AttachEventToDocument("gesturestart", function() { 
        ASPx.TouchUIHelper.isGesture = true; 
    });
    ASPx.Evt.AttachEventToDocument("gestureend", function() { 
        ASPx.TouchUIHelper.isGesture = false; 
    });
    
    ASPx.TouchUIHelper.MakeScrollable = function(element, options) {
        return new ASPx.TouchUIHelper.ScrollExtender(element, options);
    }
    ASPx.TouchUIHelper.ScrollExtender = function(element, options) {
        this.parseOptions(options ? options : {});
        this.create(element);
    };
    ASPx.TouchUIHelper.preventScrollOnEvent = function(evt){
        evt.ASPxTouchUIScrollOff = true;
    },

    ASPx.TouchUIHelper.ScrollExtender.prototype = {
        ChangeElement: function(element){
            this.destroy();
            this.create(element);
        },
        
        acceptElement: function(element) {
            if(typeof(element) == "string")
                element = document.getElementById(element);
            this.element = element;
            this.touchEventHandlersElement = this.options.touchEventHandlersElement ? this.options.touchEventHandlersElement : this.element;

            if(ASPx.Browser.AndroidMobilePlatform){
                element.style["overflow-x"] = "hidden";
                element.style["overflow-y"] = "hidden";
            }
            return element;
        },
        create: function(element) {
            this.acceptElement(element);

            if(this.options.nativeScrolling) {
                this.InitNativeScrolling();
            } else {
                this.createScrollHandlers();
                this.createEventHandlers();
                this.updateInitData();
                this.updateScrollHandles();
                this.attachEventHandlers();
            }
        },
        destroy: function() {
            this.destroyScrollHandlers();
            this.detachEventHandlers();
        },
        parseOptions: function(options) {
            this.options = {};
            this.options.showHorizontalScrollbar = options.showHorizontalScrollbar !== false;
            this.options.showVerticalScrollbar = options.showVerticalScrollbar !== false;
            this.options.acceleration = options.acceleration || 0.8;
            this.options.timeStep = options.timeStep || 50;
            this.options.minScrollbarSize = options.minScrollbarSize || 20;
            this.options.vScrollClassName = options.vScrollClassName || SCROLLBAR_CLASSNAMES.VERTICAL;
            this.options.hScrollClassName = options.hScrollClassName || SCROLLBAR_CLASSNAMES.HORIZONTAL;
            this.options.vScrollClassNameShown = [
                this.options.vScrollClassName, 
                options.vScrollClassNameShown || SCROLLBAR_CLASSNAMES.SHOWN_VERTICAL
            ].join(" ");
            this.options.hScrollClassNameShown = [
                this.options.hScrollClassName, 
                options.hScrollClassNameShown || SCROLLBAR_CLASSNAMES.SHOWN_HORIZONTAL
            ].join(" ");
            this.options.forceCustomScroll = options.forceCustomScroll === true;

            var nativeScrollPossible = !options.acceleration && !options.timeStep && !options.minScrollbarSize && !options.vScrollClassName && !options.hScrollClassName && !options.forceCustomScroll;
            if (nativeScrollPossible && ASPx.TouchUIHelper.IsNativeScrolling())
                this.options.nativeScrolling = true;
            this.options.touchEventHandlersElement = options.touchEventHandlersElement;
        },

        InitNativeScrolling: function(){
            if(this.options.showHorizontalScrollbar || this.options.showVerticalScrollbar){
                this.element.style["overflow"] = "scroll";
                this.element.style["overflow-x"] = this.options.showHorizontalScrollbar ? "scroll" : "hidden";
                this.element.style["overflow-y"] = this.options.showVerticalScrollbar ? "scroll" : "hidden";
                this.element.style["-webkit-overflow-scrolling"] = "touch";
            }
        },
        createEventHandlers: function() {
            var instance = this;
            this.onTouchStart = function(e) {
                if(!ASPx.TouchUIHelper.isGesture){
                    if(!ASPx.TouchUIHelper.ScrollExtender.activeScrolling) {
                        ASPx.TouchUIHelper.ScrollExtender.activeScrolling = instance;
                        instance.startScroll(e);
                    }
                }
            }
            this.onTouchMove = function(e) {
                if(!ASPx.TouchUIHelper.isGesture && instance.ScrollingActive(e)) {
                    instance.scroll(e);
                    if(instance.scrollBarsShown)
                        e.preventDefault();
                }
            }
            this.onTouchEnd = function(e) {
                if(!ASPx.TouchUIHelper.isGesture) {
                    instance.stopScroll();
                    if(ASPx.TouchUIHelper.ScrollExtender.activeScrolling && ASPx.TouchUIHelper.ScrollExtender.activeScrolling.initPageX == instance.initPageX && ASPx.TouchUIHelper.ScrollExtender.activeScrolling.initPageY == instance.initPageY)
                        instance.MouseEventEmulationProtectHelper.onTouchEnd(instance.initPageX, instance.initPageY, e);
                    instance.ReleaseScrolling();
                }
            }
            this.onScroll = function(e) {
                if(ASPx.TouchUIHelper.isGesture && instance.ScrollingActive(e)) {
                    instance.showScrollBars()
                    instance.updateScrollHandles();
                }
            }
            this.onClick = function() {
                instance.MouseEventEmulationProtectHelper.onClick();
            }
        },
        createScrollHandlers: function() {
            if(this.options.showHorizontalScrollbar) {
                this.hScrollHandleElement = document.createElement("DIV");
                this.hScrollHandleElement.className = this.options.hScrollClassName;
                this.element.appendChild(this.hScrollHandleElement);
                this.hEndMargin = this.options.showVerticalScrollbar ? ASPx.PxToInt(ASPx.GetCurrentStyle(this.hScrollHandleElement).marginRight) : 0;
            }
            if(this.options.showVerticalScrollbar) {
                this.vScrollHandleElement = document.createElement("DIV");
                this.vScrollHandleElement.className = this.options.vScrollClassName;
                this.element.appendChild(this.vScrollHandleElement);
                this.vEndMargin = this.options.showHorizontalScrollbar ? ASPx.PxToInt(ASPx.GetCurrentStyle(this.vScrollHandleElement).marginBottom) : 0;
            }
        },
        destroyScrollHandlers: function() {
            if(this.hScrollHandleElement && this.hScrollHandleElement.parentNode)
                this.hScrollHandleElement.parentNode.removeChild(this.hScrollHandleElement)
                
            if(this.vScrollHandleElement && this.vScrollHandleElement.parentNode)
                this.vScrollHandleElement.parentNode.removeChild(this.vScrollHandleElement)

            this.hScrollHandleElement = null;
            this.vScrollHandleElement = null;
        },
        attachEventHandlers: function() {
            this.touchEventHandlersElement.addEventListener("touchstart", this.onTouchStart   , false);
            this.touchEventHandlersElement.addEventListener("touchend"  , this.onTouchEnd     , false);
            this.touchEventHandlersElement.addEventListener("touchmove" , this.onTouchMove    , false);
            this.touchEventHandlersElement.addEventListener("scroll"    , this.onScroll       , false);
            this.touchEventHandlersElement.addEventListener("click"     , this.onClick        , false);
        },
        detachEventHandlers: function() {
            this.touchEventHandlersElement.removeEventListener("touchstart", this.onTouchStart, false);
            this.touchEventHandlersElement.removeEventListener("touchend", this.onTouchEnd, false);
            this.touchEventHandlersElement.removeEventListener("touchmove", this.onTouchMove, false);
            this.touchEventHandlersElement.removeEventListener("scroll", this.onScroll, false);
            this.touchEventHandlersElement.removeEventListener("click", this.onClick, false);
        },
        updateInitData: function() {
            window.clearTimeout(this.inertialStopTimerId);
            this.initScrollLeft = this.element.scrollLeft;
            this.initScrollTop = this.element.scrollTop;
            this.initElementX = ASPx.GetAbsoluteX(this.element);
            this.initElementY = ASPx.GetAbsoluteY(this.element);
            this.scrollTime = new Date();
            this.vx = 0;
            this.vy = 0;
        },

        ScrollingActive: function(e){
            return (!e || !e.ASPxTouchUIScrollOff) && ASPx.TouchUIHelper.ScrollExtender.activeScrolling == this;
        },
        ReleaseScrolling: function(){
            if(this.ScrollingActive())
                ASPx.TouchUIHelper.ScrollExtender.activeScrolling = null;
        },
        startScroll: function(e) {
            this.initPageX = ASPx.TouchUIHelper.getEventX(e);
            this.initPageY = ASPx.TouchUIHelper.getEventY(e);
            this.updateInitData();
            this.updateScrollHandles();
            this.showScrollBars();
        },
        scroll: function(e) {
            var newX = this.initScrollLeft + (this.initPageX - ASPx.TouchUIHelper.getEventX(e));
            var newY = this.initScrollTop  + (this.initPageY - ASPx.TouchUIHelper.getEventY(e));
            var dt = (new Date() - this.scrollTime);
            var dx = newX - this.element.scrollLeft;
            var dy = newY - this.element.scrollTop;
            this.vx = dx / dt;
            this.vy = dy / dt;
            if(this.options.showHorizontalScrollbar)
                this.element.scrollLeft = newX;
            if(this.options.showVerticalScrollbar)
                this.element.scrollTop = newY;

            this.updateScrollHandles();
            this.scrollTime = new Date();
        },
        stopScroll: function() {
            var instance = this;
            var element = this.element;
            var acceleration = this.options.acceleration;
            var timeStep = this.options.timeStep;
            this.inertialStopTimerId = window.setTimeout(function() {
                instance.vx *= acceleration;
                instance.vy *= acceleration;
            
                if(Math.abs(instance.vx) < 0.1)
                    instance.vx = 0;
                if(Math.abs(instance.vy) < 0.1) 
                    instance.vy = 0;
            
                if(instance.vx == 0 && instance.vy == 0) {
                    instance.hideScrollBars();
                    return;
                }

                var dx = instance.vx * timeStep;
                var dy = instance.vy * timeStep;

                if(instance.options.showHorizontalScrollbar)
                    element.scrollLeft += dx;
                if(instance.options.showVerticalScrollbar)
                    element.scrollTop += dy;
            
                if(element.scrollLeft + element.clientWidth >= element.scrollWidth || element.scrollLeft <= 0)
                    instance.vx = 0;
            
                if(element.scrollTop + element.clientHeight>= element.scrollHeight || element.scrollTop <= 0)
                    instance.vy = 0;
            
                instance.updateScrollHandles();
                instance.stopScroll()
            }, timeStep);
        },
        updateScrollHandles: function() {
            if(this.hScrollHandleElement) {
                var scrollHandler = this.calcScrollHandles(this.element.scrollWidth, this.element.clientWidth,
                    this.options.minScrollbarSize, this.element.scrollLeft, this.hEndMargin);

                this.hScrollHandleElement.style.width = scrollHandler.size + "px";
                ASPx.SetAbsoluteX(this.hScrollHandleElement, this.initElementX + scrollHandler.pos);
                ASPx.SetAbsoluteY(this.hScrollHandleElement, this.initElementY + this.element.clientHeight - 
                    this.hScrollHandleElement.offsetHeight);
            }
            if(this.vScrollHandleElement) {
                var scrollHandler = this.calcScrollHandles(this.element.scrollHeight, this.element.clientHeight,
                    this.options.minScrollbarSize, this.element.scrollTop, this.vEndMargin);
                this.vScrollHandleElement.style.height = scrollHandler.size + "px";
                ASPx.SetAbsoluteX(this.vScrollHandleElement, this.initElementX + this.element.clientWidth - this.vScrollHandleElement.offsetWidth);
                ASPx.SetAbsoluteY(this.vScrollHandleElement, this.initElementY + scrollHandler.pos);
            }
        },
        calcScrollHandles: function(scrollSize, clientSize, scrollBarMinSize, scrollPos, endMargin) {
            var scrollBarSize = clientSize * clientSize / scrollSize;
            scrollBarSize = scrollBarSize > scrollBarMinSize ? scrollBarSize : scrollBarMinSize;
            var k = (scrollSize == clientSize) ? 0 :
                (clientSize - scrollBarSize - endMargin) / (scrollSize - clientSize);
            return {size:scrollBarSize, pos:scrollPos * k};
        },
        showScrollBars: function() {
            var needVScrollHandle = this.element.scrollHeight > this.element.clientHeight;
            var needHScrollHandle = this.element.scrollWidth > this.element.clientWidth;
            if(this.vScrollHandleElement && needVScrollHandle)
                this.vScrollHandleElement.className = this.options.vScrollClassNameShown;
            if(this.hScrollHandleElement && needHScrollHandle)
                this.hScrollHandleElement.className = this.options.hScrollClassNameShown;
            this.scrollBarsShown = needVScrollHandle || needHScrollHandle;
        },
        hideScrollBars: function() {
            if(this.vScrollHandleElement)
                this.vScrollHandleElement.className = this.options.vScrollClassName;
            if(this.hScrollHandleElement)
                this.hScrollHandleElement.className = this.options.hScrollClassName;
            this.scrollBarsShown = false;
        },
        MouseEventEmulationProtectHelper: {
            onTouchEnd: function(initPageX, initPageY, e) {
                var difX = initPageX - ASPx.TouchUIHelper.getEventX(e);
                var difY = initPageY - ASPx.TouchUIHelper.getEventY(e);
                if(Math.abs(difX) > ASPx.TouchUIHelper.clickSensetivity || Math.abs(difY) > ASPx.TouchUIHelper.clickSensetivity){
                    ASPx.TouchUIHelper.isMouseEventFromScrolling = true;
                    window.setTimeout(function(){ ASPx.TouchUIHelper.isMouseEventFromScrolling = false; }, 100);
                }
            },
            onClick: function(){
                if(ASPx.TouchUIHelper.isMouseEventFromScrolling){
                    window.setTimeout(function(){ ASPx.TouchUIHelper.isMouseEventFromScrolling = false; }, 0);
                }
            }
        }
    };
    ASPx.TouchUIHelper.FastTapHelper = (function() {
        var actions = [];
        var DISTANCE_LIMIT = 10;
        var startX;
        var startY;
        var preventCommonClickEvents = false;
        var invokeActions = function(actions) {
            for(var i = 0; i < actions.length; i++)
                actions[i]();
        };
        var onTouchStart = function(e) {
            if(preventCommonClickEvents)
                e.stopPropagation();
            if(actions.length > 1)
                return;
            startX = e.touches[0].clientX;
            startY = e.touches[0].clientY;
            if(ASPx.Browser.AndroidDefaultBrowser)
                e.currentTarget.addEventListener("click", onClick, false);
            else
                e.currentTarget.addEventListener("touchend", onTouchEnd, false);
        };
        if(ASPx.Browser.AndroidDefaultBrowser) {
            var onClick = function(e) {
                invokeActions(actions);
                actions = [];
                e.currentTarget.removeEventListener("click", onClick, false);
            };
        } else {
            var onTouchEnd = function(e) {
                if(preventCommonClickEvents) {
                    e.preventDefault();
                    e.stopPropagation();
                }
                var stopX = e.changedTouches[0].clientX;
                var stopY = e.changedTouches[0].clientY;
                var distanceX = Math.abs(startX - stopX);
                var distanceY = Math.abs(startY - stopY);
                var allowClick = distanceX < DISTANCE_LIMIT && distanceY < DISTANCE_LIMIT;
                if(allowClick) {
                    var actionsToInvoke = actions;
                    setTimeout(function() { invokeActions(actionsToInvoke); }, 0);
                }
				actions = [];
                e.currentTarget.removeEventListener("touchend", onTouchEnd, false);
            };
        }
        return {
            HandleFastTap: function(e, tapAction, preventClickEvents) {
                if(e.touches.length > 1)
                    return;
                preventCommonClickEvents = preventClickEvents;
                actions.push(tapAction);
                onTouchStart(e);
            }
        };
    })();
    ASPx.TouchUIHelper.doubleTapEventName = "dxDoubleTap";
    ASPx.TouchUIHelper.AttachDoubleTapEventToElement = function(element, action) {
        var DOUBLE_TAP_DELAY = 600;
        var DISTANCE_LIMIT = 10;
        
        var onTouchEnd = function(e) {
            e.currentTarget.removeEventListener("touchend", onTouchEnd, false);
            e[ASPx.TouchUIHelper.doubleTapEventName] = true;
            var startActionAfterFastTap = function() {
                window.setTimeout(function(){ action(e); }, 0);
            }
            startActionAfterFastTap();
        };
        var onTouchStart = function(e) {
            var currentTapTime = e.timeStamp;
            var currentX = e.changedTouches[0].clientX;
            var currentY = e.changedTouches[0].clientY;
            var lastTapTime = this["lastTap"] || currentTapTime;
            var lastX = this["lastX"] || currentX;
            var lastY = this["lastY"] || currentY;

            this["lastTap"] = currentTapTime;
            this["lastX"] = currentX;
            this["lastY"] = currentY;
            var delay = currentTapTime - lastTapTime;
            if(!delay || delay > DOUBLE_TAP_DELAY || e.touches.length > 1 || 
                Math.abs(currentX - lastX) > DISTANCE_LIMIT || 
                Math.abs(currentY - lastY) > DISTANCE_LIMIT)
                    return;

            var preventZoom = function() { e.preventDefault(); }
            preventZoom();
            e.currentTarget.addEventListener("touchend", onTouchEnd, false);
        };
        element.addEventListener("touchstart", onTouchStart, false);
    };
    window.ASPxClientTouchUI = {};
    window.ASPxClientTouchUI.MakeScrollable = ASPx.TouchUIHelper.MakeScrollable;
    window.ASPxClientTouchUI.ScrollExtender = ASPx.TouchUIHelper.ScrollExtender;
})();