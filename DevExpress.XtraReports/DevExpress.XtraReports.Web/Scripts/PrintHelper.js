/// <reference path="ReportViewer.js"/>
/// <reference path="ReportViewerOpera.js"/>

(function(window) {
    /* class PrintHelper */
    var dx_PrintHelper = ASPx.CreateClass(null, {
        constructor: function() {
            this.printFrame = null;
            if(ASPx.Browser.Opera) {
                this.printElementName = "DXOperaPrinter";
                this.printElement = null;
                this.mergeProperties(ASPx.dx_XROperaPrintTrick);
            }
        },
        pdfExists: function() {
            if(!ASPx.IsExists(this.pdf)) {
                this.pdf = this.existsIEPlugin() || this.existsFFPlugin();
            }
            return this.pdf;
        },
        existsIEPlugin: function() {
            return this.createActiveXObject("PDF.PdfCtrl.5")
                || this.createActiveXObject("PDF.PdfCtrl.6")
                || this.createActiveXObject("AcroPDF.PDF.1");
        },
        createActiveXObject: function(classid) {
            if(window["ActiveXObject"] === undefined) {
                return null;
            }
            try {
                return new ActiveXObject(classid);
            } catch(ex) {
                return null;
            }
        },
        existsFFPlugin: function() {
            var plugins = navigator.mimeTypes["application/pdf"];
            var plugin = plugins && plugins.enabledPlugin;
            return plugin
                && (plugin.name === "Chrome PDF Viewer" || (plugin.description.indexOf("Adobe") !== -1)
                    && (plugin.description.indexOf("Version") === -1 || parseFloat(plugin.description.split("Version")[1]) >= 6));
        },
        getFrame: function() {
            this.ensurePrintFrame();
            return this.printFrame;
        },
        getFrameRecreated: function() {
            var frameElement = document.getElementById("DXPrinter");
            if(frameElement) {
                document.body.removeChild(frameElement);
            }
            if(this.printFrame) {
                this.printFrame = null;
            }
            return this.getFrame();
        },
        mergeProperties: function(properties) {
            for(var name in properties) {
                this[name] = properties[name];
            }
        },
        print: function(result) {
            this.ensurePrintElement();
            this.fillPrintElement(result);
            var printDocument = this.printFrame.document;
            if(ASPx.Browser.IE && printDocument.readyState !== "complete") {
                ASPx.Evt.AttachEventToElement(printDocument, "readystatechange", function() {
                    if(printDocument.readyState === "complete") {
                        this.printCore();
                    }
                }.aspxBind(this));
            } else
                this.printCore();
        },
        printCore: function() {
            if(!ASPx.IsExists(this.printFrame)) {
                return;
            }
            var style = null;
            if(ASPx.Browser.IE) {  //Bug B91836, B92764
                style = ASPx.GetElementById("DXRPrintHideContent");
                this.setIEStyleDisabled(style, false);
            }
            this.printFrame.focus();
            this.printFrame.print();
            if(ASPx.Browser.IE) {
                this.setIEStyleDisabled(style, true);
            }
        },
        setIEStyleDisabled: function(style, disabled) {
            if(!ASPx.IsExists(style)) {
                return;
            }
            var sheet = ASPx.Browser.Version < 9
                ? style.styleSheet
                : style.sheet;
            sheet.disabled = disabled;
        },
        fillPrintElement: function(content) {
            var printDocument = this.printFrame.document;
            printDocument.open("text/html", "replace");
            printDocument.write(content);
            printDocument.close();
        },
        ensurePrintElement: function() {
            this.ensurePrintFrame();
        },
        ensurePrintFrame: function() {
            if(this.printFrame) {
                return;
            }
            var elementId = "DXPrinter";
            if(ASPx.Browser.Firefox) {
                //https://bugzilla.mozilla.org/show_bug.cgi?id=350023
                try {
                    delete window.frames[elementId];
                } catch(e) { }
            }
            this.createFrameElement(elementId);
            this.printFrame = window.frames[elementId];
        },
        createFrameElement: function(name) {
            var f = document.createElement("iframe");
            f.frameBorder = "0";
            f.style.overflow = "hidden";
            var frameSize = ASPx.Browser.Safari ? "1px" : "0px"; //B142603 workaround
            var position = ASPx.Browser.Safari ? "0px" : "-100px"; //B142603 workaround
            f.style.width = frameSize;
            f.style.height = frameSize;
            f.name = name;
            f.id = name;
            f.style.position = "absolute";
            f.style.top = position;
            f.style.left = position;
            if (ASPx.Browser.Safari) {
                f.style.opacity = 0;
            }
            if(ASPxClientReportViewer.__useMobileSpecificExport) {
                f.setAttribute('sandbox', "allow-same-origin allow-scripts allow-forms");
            }
            document.body.appendChild(f);
            return f;
        }
    });

    ASPx.dx_PrintHelper = dx_PrintHelper;
})(window);