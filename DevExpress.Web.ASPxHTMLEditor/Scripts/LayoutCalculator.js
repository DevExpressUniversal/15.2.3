(function() {
    var LayoutCalculator = ASPx.CreateClass(null, {
    
        updateLayout: function(htmlEditor, activeView, isInitializing) {
            if(ASPx.Browser.Opera)
                htmlEditor.layoutManager.showHideViewAreasDependingOnActiveView(activeView);
            if(!htmlEditor.IsVisible())
                return;
            htmlEditor.layoutManager.hideViewAreas();
            this.UpdateToolbarRowDisplay(htmlEditor, activeView);
            htmlEditor.layoutManager.collapseViewAreas();
            htmlEditor.layoutManager.showHideViewAreasDependingOnActiveView(activeView);
            if(ASPx.Browser.NetscapeFamily && !htmlEditor.isHtmlView(activeView))
                this.FixToolbarTableBorder_NS(htmlEditor);
            var ffi = htmlEditor.getFakeFocusInputElement();
            ffi.style.top = "0px"; //B156243
            ffi.tabIndex = "-1"; //Q483286
            this.calculateSizes(htmlEditor);
        },
    
        calculateSizes: function(htmlEditor) {
            var layoutManager = htmlEditor.layoutManager;
            if(layoutManager.currentHeight == 0){
                var mainElement = layoutManager.getMainElement();
                var width = ASPx.PxToInt(mainElement.style.width);
                var height = ASPx.PxToInt(mainElement.style.height);
                if(width == 0) { // percentage
                    var percentWidth = ASPx.PercentageToFloat(mainElement.style.width) * 100;
                    if(percentWidth > 0) {
                        layoutManager.isWidthDefinedInPercent = true;
                        layoutManager.initialMainElementWidth = mainElement.style.width;
                        htmlEditor.SetClientStateFieldValue("CurrentWidth", Math.round(percentWidth));
                        layoutManager.percentSizeDiv = ASPx.CreateHtmlElementFromString("<div style='height:0px;font-size:0px;line-height:0;width:100%;'></div>");
                        ASPx.InsertElementAfter(layoutManager.percentSizeDiv, mainElement);
                        mainElement.parentNode.style.width = layoutManager.initialMainElementWidth;
                        htmlEditor.UpdateAdjustmentFlags();
                    }
                }
                layoutManager.currentHeight = (height == 0) ? mainElement.offsetHeight : height;
                layoutManager.currentWidth = (width == 0) ? mainElement.offsetWidth : width;
                mainElement.style.height = "";
                mainElement.style.width = "";
            }
            htmlEditor.SetClientStateFieldValue("IsPercentWidth", layoutManager.isWidthDefinedInPercent ? 1 : 0);
            if(layoutManager.isInFullscreen()) {
                layoutManager.adjustSizeInFullscreen();
                return;
            }
            if(layoutManager.isWidthDefinedInPercent)
                layoutManager.adjustSizeInPercent();
            else
                layoutManager.setWidthInternal(layoutManager.currentWidth, true, true);

            htmlEditor.validationManager.hideErrorFrame();
            layoutManager.setHeightInternal(layoutManager.currentHeight, true, true);
            htmlEditor.validationManager.updateErrorFrame();
        },
        UpdateToolbarRowDisplay: function(htmlEditor, activeView) {
            this.ShowHideToolbarRow(htmlEditor, this.IsToolbarRowVisible(htmlEditor, activeView));
        },
        IsToolbarRowVisible: function(htmlEditor, activeView) {
            return htmlEditor.isDesignViewAllowed() && htmlEditor.isDesignView(activeView);
        },
        ShowHideToolbarRow: function(htmlEditor, isDisplayed) {
            var toolbarRow = htmlEditor.getToolbarRow();
            if(toolbarRow)
                ASPx.SetElementDisplay(toolbarRow, isDisplayed);
        },
        FixToolbarTableBorder_NS: function(htmlEditor) {
            var table = htmlEditor.getToolbarTable();
            if(table) {
                var borderCollapse = table.style.borderCollapse;
                table.style.borderCollapse = "";
                table.style.borderCollapse = borderCollapse;
            }
        }
    });

    layoutCalculator = null;
    function aspxGetHtmlEditorLayoutCalculator() {
        if(layoutCalculator == null)
            layoutCalculator = new LayoutCalculator();
        return layoutCalculator;
    }
    ASPx.GetHtmlEditorLayoutCalculator = aspxGetHtmlEditorLayoutCalculator;
})();