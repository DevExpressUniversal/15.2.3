(function() {
    var cdDPTableIDSuffix = "_IT";
    var cdDPTableCellIDSuffixPart = "_IC";
    var cdDPTableCellIndexAttribute = "_dxIPIndex";
    var cdDPItemPickerClientId = "ItemPicker";
    var heItemPickerCellCssClassName = "dxHEIPCell";

    ASPx.HtmlEditorClasses.Controls.ToolbarItemPicker = ASPx.CreateClass(ASPxClientControl, {
        constructor: function(name) {
            this.constructor.prototype.constructor.call(this, name);
            this.ItemPickerItemClick = new ASPxClientEvent;
            this.itemsValues = [];
            this.useItemPickerImageMode = ASPx.HtmlEditorClasses.ItemPickerImageMode.ExecuteSelectedItemAction; // TODO
            this.curIndex = 0;
            this.tableCellStyleCssClassName = "";
            this.tableCellStyleCssText = "";
            this.tableCellStyleImageSpacing = "";
            this.imagePosition = "Left";
            this.itemHeight = 0;
            this.itemWidth = 0;
            this.menuID = 0;
        },
        Initialize: function() {
            this.constructor.prototype.Initialize.call(this);
            var index = 0;
            var table = this.GetPickerTableElement();
            if(!table)
                return;
            this.InitializeStyles();

            for(var i = 0; i < table.rows.length; i++) {
                var cellCount = table.rows[i].cells.length;
                for(var j = 0; j < cellCount; j++) {
                    var cell = table.rows[i].cells[j];
                    if(i * cellCount + j < this.itemsValues.length) {
                        ASPx.Attr.SetAttribute(cell, cdDPTableCellIndexAttribute, index);
                        cell.className = this.tableCellStyleCssClassName + " " + this.GetStyleName();
                        ASPx.Attr.SetAttribute(cell, "id", this.GetPickerTableCellElementID(index++));
                    }
                }
            }
            if(this.itemHeight > 0 || this.itemWidth > 0)
                this.PrepareCellSize();
        },

        InitializeStyles: function() {
            var styleSheet = ASPx.GetCurrentStyleSheet();
            ASPx.AddStyleSheetRule(styleSheet, "." + this.GetStyleName(), this.tableCellStyleCssText);
            ASPx.AddStyleSheetRule(styleSheet, "." + this.GetStyleName(), "white-space: nowrap;");
            ASPx.AddStyleSheetRule(styleSheet, "." + this.GetStyleName(), "text-align: center;");

            if(this.tableCellStyleImageSpacing != "") {
                var imageMarginRule = (this.imagePosition == "Left" ? "margin-right" : "margin-bottom") + ": " + this.tableCellStyleImageSpacing + ";";
                var imageMarginSelector = "." + this.GetStyleName() + " img";
                ASPx.AddStyleSheetRule(styleSheet, imageMarginSelector, imageMarginRule);
            }
        },
        PrepareCellSize: function() {
            var styleSheet = ASPx.GetCurrentStyleSheet();
            var table = this.GetPickerTableElement();
            var firstCell = table.rows[0].cells[0];
            var curStyle = ASPx.GetCurrentStyle(firstCell);
            if(!curStyle)
                curStyle = firstCell.style;

            if(this.itemWidth > 0) {
                this.itemWidth -= ASPx.PxToInt(curStyle.paddingLeft) + ASPx.PxToInt(curStyle.paddingRight);
                this.itemWidth -= ASPx.PxToInt(curStyle.borderLeftWidth) + ASPx.PxToInt(curStyle.borderRightWidth);
                ASPx.AddStyleSheetRule(styleSheet, "." + this.GetStyleName(), "width: " + this.itemWidth + "px;");
            }
            if(this.itemHeight > 0) {
                this.itemHeight -= ASPx.PxToInt(curStyle.borderTopWidth) + ASPx.PxToInt(curStyle.borderBottomWidth);
                if(ASPx.Browser.IE || ASPx.Browser.WebKitFamily)
                    this.itemHeight -= ASPx.PxToInt(curStyle.paddingTop) + ASPx.PxToInt(curStyle.paddingBottom);
                ASPx.AddStyleSheetRule(styleSheet, "." + this.GetStyleName(), "height: " + this.itemHeight + "px;");
            }
        },

        GetPickerTableElement: function() {
            return ASPx.GetElementById(this.name + cdDPTableIDSuffix);
        },
        GetPickerTableCellElementID: function(itemIndex) {
            return this.name + cdDPTableCellIDSuffixPart + itemIndex.toString();
        },
        OnControlClick: function(clickedElement, htmlEvent) {
            var element = clickedElement.tagName == "TD" ? clickedElement : clickedElement.parentNode;
            if(element.tagName == "TD") {
                var index = ASPx.Attr.GetAttribute(element, cdDPTableCellIndexAttribute);
                if(index >= 0 && this.itemsValues.length > index) {
                    this.curIndex = index;
                    this.RaiseItemClickEvent(this.GetValue());
                }
            }
            ASPx.GetMenuCollection().HideAll();
        },

        RaiseItemClickEvent: function(value) {
            if(!this.ItemPickerItemClick.IsEmpty()) {
                var args = new ASPx.HtmlEditorClasses.Controls.ToolbarItemPickerItemClickEventArgs(value);
                this.ItemPickerItemClick.FireEvent(this, args);
            }
        },

        GetValue: function() {
            return this.itemsValues[this.curIndex];
        },

        GetImage: function() {
            var cell = ASPx.GetElementById(this.GetPickerTableCellElementID(this.curIndex));
            return cell && ASPx.GetNodeByTagName(cell, "IMG", 0) || null;
        },
        GetImageUrl: function() {
            var image = this.GetImage();
            return image || "";
        },

        GetText: function() {
            var cell = ASPx.GetElementById(this.GetPickerTableCellElementID(this.curIndex));
            return cell ? ASPx.GetInnerText(cell) : "";
        },
        GetTooltip: function() {
            var cell = ASPx.GetElementById(this.GetPickerTableCellElementID(this.curIndex));
            return cell ? cell.title : "";
        },
        GetStyleName: function() {
            return heItemPickerCellCssClassName + "_" + this.name;
        }
    });

    ASPx.HtmlEditorClasses.Controls.ToolbarItemPicker.FindControlByMenuItem = function(menuItem) {
        var name = menuItem.menu.GetMenuTemplateContainerID(menuItem.indexPath) + "_" + cdDPItemPickerClientId;
        return ASPx.GetControlCollection().Get(name);
    }

    ASPx.HtmlEditorClasses.Controls.ToolbarItemPickerItemClickEventArgs = ASPx.CreateClass(ASPxClientEventArgs, {
        constructor: function(value) {
            this.value = value;
        }
    });

})();