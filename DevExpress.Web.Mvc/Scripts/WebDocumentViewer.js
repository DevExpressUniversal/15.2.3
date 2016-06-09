

(function(window) {
    var MVCxClientWebDocumentViewer = ASPx.CreateClass(ASPxClientWebDocumentViewer, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
        }
    });

    window.MVCxClientWebDocumentViewer = MVCxClientWebDocumentViewer;
})(window);