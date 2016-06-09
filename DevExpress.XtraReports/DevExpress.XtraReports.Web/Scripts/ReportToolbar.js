/// <reference path="ReportViewer.js"/>

(function(window) {
    var ASPxClientReportToolbar;
    ASPxClientReportToolbar = ASPx.CreateClass(ASPxClientControl, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.reportViewer = null;
            this.ignoreItems = {};
            this.enabledItems = [];
            this.delayUpdate = false;
        },
        InlineInitialize: function() {
            this.constructor.prototype.InlineInitialize.call(this);
            this.menu = ASPxClientReportToolbar.findValidControl(this.name + '_Menu');
        },
        Initialize: function() {
            this.constructor.prototype.Initialize.call(this);

            if(this.menu) {
                for(var i = 0; i < this.menu.GetItemCount() ; i++) {
                    var item = this.menu.GetItem(i);
                    if(!item.GetEnabled()) {
                        this.ignoreItems[item.name] = item;
                    }
                }
                this.disableMenuItems();
            }

            this.reportViewer = ASPx.GetControlCollection().Get(this.reportViewerID);
            if(this.reportViewer) {
                var self = this;
                this.reportViewer.PageLoad.AddHandler(function(s, a) { self.pageLoadEventHanler(s, a); });
                this.reportViewer.BeginCallback.AddHandler(function() { self.beginCallbackEventHanler(); });
                this.reportViewer.EndCallback.AddHandler(function() { self.endCallbackEventHanler(); });
            }
        },
        AfterInitialize: function() {
            this.constructor.prototype.AfterInitialize.call(this);
            if(this.delayUpdate) {
                this.updateView();
                this.delayUpdate = false;
            }
        },
        GetMainElement: function() {
            return this.menu
                ? this.menu.GetMainElement()
                : ASPxClientControl.prototype.GetMainElement.call(this);
        },
        isValid: function() {
            return this.menu && ASPx.IsExistsElement(this.menu.GetMainElement());
        },
        disableMenuItems: function() {
            if(!this.enabledItems)
                this.enabledItems = [];
            else if(this.enabledItems.length > 0)
                return;
            for(var i = 0; i < this.menu.GetItemCount() ; i++) {
                var item = this.menu.GetItem(i);
                if(item.GetEnabled()) {
                    this.enabledItems[this.enabledItems.length] = item;
                    item.SetEnabled(false);
                }
            }
        },
        beginCallbackEventHanler: function() {
            this.disableMenuItems();
        },
        endCallbackEventHanler: function() {
            this.updateElements();
        },
        pageLoadEventHanler: function(s, a) {
            this.pageIndex = a.PageIndex;
            this.pageCount = a.PageCount;
            if(this.isInitialized)
                this.updateView();
            else
                this.delayUpdate = true;
        },
        handleButton: function(btnId) {
            if(!ASPx.IsExists(this.reportViewer))
                return;
            if(btnId === "Search") {
                this.reportViewer.Search();
                return;
            }
            if(btnId === "PrintReport") {
                this.reportViewer.Print();
                return;
            }
            if(btnId === "PrintPage") {
                this.reportViewer.Print(this.pageIndex);
                return;
            }
            if(btnId === "SaveToWindow") {
                this.reportViewer.SaveToWindow(this.getSaveFormat());
                return;
            }
            if(btnId === "SaveToDisk") {
                this.reportViewer.SaveToDisk(this.getSaveFormat());
                return;
            }
            if(this.pageIndex < 0 || this.pageCount <= 0)
                return;
            var index = this.pageIndex;
            if(btnId === "FirstPage") {
                index = 0;
            } else if(btnId === "PreviousPage") {
                index = this.pageIndex - 1;
            } else if(btnId === "NextPage") {
                index = this.pageIndex + 1;
            } else if(btnId === "LastPage") {
                index = this.pageCount - 1;
            } else if(btnId === "PageNumber") {
                index = this.getControlIntValue("PageNumber") - 1;
            }
            this.reportViewer.GotoPage(index);
        },
        getSaveFormat: function() {
            var saveFormat = this.getControlValue("SaveFormat");
            return saveFormat || this.reportViewer.DefaultSaveFormat;
        },
        updateView: function() {
            this.updatePageIndexes();
            this.setElemSize("PageCount", this.pageCount.toString().length);
            this.setControlValue("PageCount", this.pageCount);
            this.updateElements();
        },
        updateElements: function() {
            if(this.pageCount > 0) {
                for(var i = 0; i < this.enabledItems.length; i++)
                    this.enabledItems[i].SetEnabled(true);
            }
            var val = this.pageIndex > 0;
            this.setElemEnabled("FirstPage", val);
            this.setElemEnabled("PreviousPage", val);

            val = this.pageCount > 0 && this.pageIndex < this.pageCount - 1;
            this.setElemEnabled("NextPage", val);
            this.setElemEnabled("LastPage", val);
            this.setElemEnabled("Search", this.reportViewer.IsSearchAllowed());
        },
        updatePageIndexes: function() {
            var cbx = this.getTemplateControl("PageNumber");
            if(!cbx)
                return;
            if(cbx.GetItemCount() === this.pageCount) {
                this.updatePageIndexes_selectPageIndex(cbx);
                return;
            }
            cbx.BeginUpdate();
            cbx.ClearItems();
            this.runLoopInPortionsAsync(
                { to: this.pageCount },
                function(i) {
                    var text = (i + 1).toString();
                    cbx.AddItem(text, text);
                    if(i === this.pageIndex) {
                        this.updatePageIndexes_selectPageIndex(cbx);
                    }
                }.aspxBind(this),
                function() { cbx.EndUpdate(); });
        },
        runLoopInPortionsAsync: function runLoopInPortionsAsync(args, actionCallback, completeCallback) {
            var from = args.from || 0;
            var step = args.step || 100;
            if(from > args.to) {
                throw new Error("Argument 'args.from' can not be greater than 'args.to'.");
            }
            if(step <= 0) {
                throw new Error("Argument 'args.step' can not be less than or equal to zero.");
            }
            var nextBound = from + step;
            if(nextBound > args.to) {
                nextBound = args.to;
            }
            var current;
            for(current = from; current < nextBound; current++) {
                actionCallback(current);
            }
            if(current < args.to) {
                var newArgs = { from: current, to: args.to, step: args.step, timeout: args.timeout };
                var timeout = args.timeout || 50;
                setTimeout(function() { runLoopInPortionsAsync(newArgs, actionCallback, completeCallback); }, timeout);
            } else {
                completeCallback();
            }
        },
        updatePageIndexes_selectPageIndex: function(cbx) {
            cbx.SetSelectedIndex(this.pageIndex);
            //temporary workaround
            if(ASPx.Browser.WebKitFamily)
                cbx.GetInputElement().value = (this.pageIndex + 1).toString();
        },
        setElemSize: function(name, size) {
            var control = this.getTemplateControl(name);
            var element = control && control.GetInputElement();
            if(element) {
                element.style.width = size + "em";
            }
        },
        setControlValue: function(name, val) {
            var control = this.getTemplateControl(name);
            if(control)
                control.SetValue(val);
        },
        getControlValue: function(name) {
            var control = this.getTemplateControl(name);
            return control && control.GetValue();
        },
        getTemplateControl: function(name) {
            var item = this.menu.GetItemByName(name);
            return this.getTemplateControlCore(item);
        },
        getControlIntValue: function(name) {
            var val = this.getControlValue(name);
            return val ? parseInt(val, 10) : -1;
        },
        setElemEnabled: function(name, val) {
            if(this.ignoreItems[name])
                return;
            var item = this.menu.GetItemByName(name);
            if(item && item.GetEnabled() != val)
                item.SetEnabled(val);
        },
        getTemplateControlCore: function(menuItem) {
            if(!menuItem) {
                return null;
            }
            var editor = ASPx.GetControlCollection().Get(this.menu.GetItemTemplateContainerID(menuItem.indexPath) + '_' + menuItem.name);
            return editor;
        },
        getItemByEditor: function(editor) {
            var count = this.menu.GetItemCount();
            for(var i = 0; i < count; i++) {
                var item = this.menu.GetItem(i);
                var currentEditor = this.getTemplateControlCore(item);
                if(currentEditor === editor) {
                    return item;
                }
            }
            return null;
        }
    });
    ASPxClientReportToolbar.findValidControl = function(name) {
        var control = ASPx.GetControlCollection().Get(name);
        return control && ASPx.IsExistsElement(control.GetMainElement())
            ? control
            : null;
    };
    ASPxClientReportToolbar.Cast = ASPxClientControl.Cast;
    ASPxClientReportToolbar.prototype.GetItemTemplateControl = function(name) {
        return this.getTemplateControl(name);
    };

    window.ASPxClientReportToolbar = ASPxClientReportToolbar;
})(window);