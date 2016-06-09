(function(window) {
    var dx_XROperaPrintTrick = {
        printCore: function() {
            this.runAfterImagesLoaded(function() { self.print(); });
        },
        fillPrintElement: function(content) {
            this.printElement.innerHTML = content;
        },
        ensurePrintElement: function(className) {
            if(!ASPx.IsExists(this.printElement)) {
                this.printElement = this.createDivElement(this.printElementName, className);
                this.ensurePrintStyleElement();
            }
        },
        ensurePrintStyleElement: function() {
            if(ASPx.GetElementById(this.printElementName + "Style")) {
                return;
            }
            var htmlElement = document.getElementsByTagName("HTML")[0];
            var elements = htmlElement.getElementsByTagName("HEAD");
            var headElement = (elements && elements.length > 0) ? elements[0] : htmlElement.appendChild(document.createElement("HEAD"));
            var styleElement = headElement.appendChild(document.createElement("STYLE"));
            styleElement.innerText = "@media print {div." + this.printElementName + " { display: block } body>* { display: none } body { background-color: #FFFFFF;  background-image: none; margin: 0px 0px 0px 0px } } @media screen,projection {div." + this.printElementName + " { display: none }}";
            styleElement.id = this.printElementName + "Style";
        },
        createDivElement: function(name, className) {
            var d = document.createElement("div");
            d.name = name;
            d.id = name;
            d.className = name + " " + className;
            document.body.appendChild(d);
            return d;
        },
        runAfterImagesLoaded: function(func) {
            var reqs = [];
            reqs.remove = function(req) {
                for(var i = 0; i < this.length; i++) {
                    if(this[i] == req) {
                        this.splice(i, 1);
                    }
                }
            };
            var onReadyStateChangeHandler = function() {
                if(this.readyState === 4) {
                    reqs.remove(this);
                    if(reqs.length === 0) {
                        func();
                    }
                }
            };
            var runRequest = function() {
                var req = new XMLHttpRequest();
                req.open("GET", images[i].src, true);
                req.send(null);
                req.onreadystatechange = onReadyStateChangeHandler;
                return req;
            };
            var images = this.printElement.getElementsByTagName("IMG");
            for(var i = 0; i < images.length; i++) {
                if(!images[i].complete) {
                    reqs.push(runRequest());
                }
            }
            if(reqs.length === 0) {
                func();
            }
        }
    };

    ASPx.dx_XROperaPrintTrick = dx_XROperaPrintTrick;
})(window);