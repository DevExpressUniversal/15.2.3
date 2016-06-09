(function() {
    var InsertDialogBase = ASPx.HtmlEditorClasses.Dialogs.InsertDialogBase;
    var BGColorEditMultiColorClassName = "dxhe-tableDialogCEMultiColor";

    function attachWidthChangeEvent(dialog) {
        attachSizeChangeEvent(dialog, "Width");    
    }
    function attachHeightChangeEvent(dialog) {
        attachSizeChangeEvent(dialog, "Height");    
    }
    function attachSizeChangeEvent(dialog, prefix) {
        dialog.getControl(prefix + "Type").ValueChanged.AddHandler(function(s, e) {
            var isVisible = s.GetValue() == "custom";
            this.getControl(prefix + "Value").SetVisible(isVisible);
            this.getControl(prefix + "ValueType").SetVisible(isVisible);        
        }.aspxBind(dialog));    
    }
    function getPropertiesMap(hasWidth, hasHeight, hasTableProperties, hasTableCellProperties) {
        var result = { "backgroundColor": "BackgroundColor" };
        if(hasTableProperties) {
            result.cellSpacing = "CellSpacing";
            result.cellPadding = "CellPadding";
            result.align = "Alignment";
            result.borderColor = "BorderColor";
            result.borderWidth = "BorderWidth";
            result.accessibility = {
                "summary": "Summary",
                "caption": "Caption",
                "headers": "Headers"
            };    
        }
        if(hasWidth) {
            result.widthType = "WidthType"; 
            result.widthValue = "WidthValue";
            result.widthValueType = "WidthValueType";
            result.width = function() {
                var width = this.widthType;
                if (width == "custom")
                    width = this.widthValue + this.widthValueType;
                return width || this.width;
            };
        }
        if(hasHeight) {
            result.heightType = "HeightType"; 
            result.heightValue = "HeightValue";
            result.heightValueType = "HeightValueType";
            result.height = function() {
                var height = this.heightType;
                if (height == "custom")
                    height = this.heightValue + this.heightValueType;
                return height || this.height;
            };
        }
        if(hasTableCellProperties) {
            result.align = "HorizontalAlignment";
            result.vAlign = "VerticalAlignment";
            result.applyForAll = "ApplyToAllCells";
        }
        return result;
    }

    var TableDialogBase = ASPx.CreateClass(InsertDialogBase, {
        InitializeDialogFields: function (settings) {
            InsertDialogBase.prototype.InitializeDialogFields.call(this, settings);
            var BGColorEdit = this.getControl("BackgroundColor");
            if(BGColorEdit) {
                if(settings && settings.hasMultipleColors) {
                    ASPx.AddClassNameToElement(BGColorEdit.GetMainElement(), BGColorEditMultiColorClassName);
                } else {
                    ASPx.RemoveClassNameFromElement(BGColorEdit.GetMainElement(), BGColorEditMultiColorClassName);
                    if(!settings || !settings.backgroundColor)
                        BGColorEdit.SetValue(BGColorEdit.GetAutomaticColorItemValue());
                }
            };
        },
        getObjectSettings: function() {
            var result = InsertDialogBase.prototype.getObjectSettings.call(this);
            var BGColorEdit = this.getControl("BackgroundColor");
            if(result && BGColorEdit && ASPx.ElementContainsCssClass(BGColorEdit.GetMainElement(), BGColorEditMultiColorClassName))
                result.cantChangeBackground = !(BGColorEdit.IsAutomaticColorSelected() || result.backgroundColor);
            return result;
        }
    });

    var InsertTableDialog = ASPx.CreateClass(TableDialogBase, {
        getCommandName: function() {
            return ASPxClientCommandConsts.INSERTTABLE_COMMAND;
        },
        getDialogPropertiesMap: function() {
            return ASPx.HtmlEditorClasses.Utils.mergeFieldMaps(this.getAdditionalFieldMap(), {
                "tableProperties": getPropertiesMap(true, true, true, false)
            });   
        },
        getAdditionalFieldMap: function() {
            return {
                "columns": "ColumnCount",
                "rows": "RowCount",
                "isColumnEqualWidth": "EqualWidth"
            };
        },
        getFocusField: function () {
            return this.getControl("ColumnCount");
        },
        GetDialogCaptionText: function () {
            return ASPx.HtmlEditorDialogSR.InsertTable;
        },
        attachEvents: function() {
            attachWidthChangeEvent(this);
            attachHeightChangeEvent(this);
            ASPx.HtmlEditorClasses.Utils.executeIfExists(this.getControl("Accessibility"), function(editor) {
                editor.CheckedChanged.AddHandler(function(s, e) {
                    this.getFormLayout().GetItemByName("AccessibilityGroup").SetVisible(s.GetChecked());        
                }.aspxBind(this));
            }.aspxBind(this));
        }
    });
    var TablePropertiesDialog = ASPx.CreateClass(InsertTableDialog, {
        getCommandName: function() {
            return ASPxClientCommandConsts.CHANGETABLE_COMMAND;    
        },
        getAdditionalFieldMap: function() {
            return {
                "tableElement": function() {
                    return this.selectedTable;    
                }.aspxBind(this)
            };
        },
        getFocusField: function () {
            return this.getControl("Alignment");
        },
        GetInitInfoObject: function() {
            this.selectedTable = ASPx.HtmlEditorTableHelper.GetTable(this.getSelectedElement());
            if(this.selectedTable)
                return { "tableProperties": ASPx.HtmlEditorTableHelper.GetTableProperties(this.selectedTable) };
            return null;
        },
        InitializeDialogFields: function(tableInfo) {
            InsertTableDialog.prototype.InitializeDialogFields.call(this, tableInfo);
            this.getFormLayout().GetItemByName("ColumnCount").SetVisible(false);
            this.getFormLayout().GetItemByName("RowCount").SetVisible(false);
            this.getFormLayout().GetItemByName("EqualWidth").SetVisible(false);
            if(tableInfo) {
                TablePropertiesDialog.SetSizeEditors(this.getControl("WidthType"), this.getControl("WidthValueType"), this.getControl("WidthValue"), tableInfo.tableProperties.width);
                TablePropertiesDialog.SetSizeEditors(this.getControl("HeightType"), this.getControl("HeightValueType"), this.getControl("HeightValue"), tableInfo.tableProperties.height);
                this.showAccessibilitySection(tableInfo);
            }
        },
        hasAccessibilitySection: function() {
            return !!this.getControl("Accessibility");
        },
        showAccessibilitySection: function(tableInfo) {
            var accessibility = tableInfo.tableProperties.accessibility;
            var hasContent = accessibility.caption != "" || accessibility.headers != null || accessibility.summary != "";
            if (hasContent && this.hasAccessibilitySection()) {
                this.getControl("Accessibility").SetChecked(true);
                this.getFormLayout().GetItemByName("AccessibilityGroup").SetVisible(true);
            }
        },
        GetDialogCaptionText: function() {
            return ASPx.HtmlEditorDialogSR.ChangeTable;
        }
    });

    TablePropertiesDialog.SetSizeEditors = function(cmb, cmbType, spinEdit, value) {
        cmb.SetValue(value);
        var selectedItem = cmb.GetSelectedItem();
        if(selectedItem == null) {
            cmb.SetValue("custom");
            var res = ASPx.HtmlEditorTableHelper.ParseSizeString(value);
            if(res.valueType) {
                value = res.value;
                cmbType.SetValue(res.valueType);
                cmbType.RaiseValueChangedEvent();
            }
            spinEdit.SetText(value);
        }
        cmb.RaiseValueChangedEvent();
    }

    var TableElementPropertiesDialogBase = ASPx.CreateClass(TableDialogBase, {
        hasWidth: function() {
            return false;    
        },
        hasHeight: function() {
            return false;    
        },
        getDialogPropertiesMap: function() {
            return getPropertiesMap(this.hasWidth(), this.hasHeight(), false, true);
        },
        getObjectSettings: function() {
            return {
                properties: TableDialogBase.prototype.getObjectSettings.call(this),
                cell: this.selectedCell
            };
        },
        getFocusField: function () {
            return this.getControl("HorizontalAlignment");
        },
        InitializeDialogFields: function (settings) {
            TableDialogBase.prototype.InitializeDialogFields.call(this, settings);
            if(settings) {
                if(this.hasWidth())
                    TablePropertiesDialog.SetSizeEditors(this.getControl("WidthType"), this.getControl("WidthValueType"), this.getControl("WidthValue"), settings.width);
                if(this.hasHeight())
                    TablePropertiesDialog.SetSizeEditors(this.getControl("HeightType"), this.getControl("HeightValueType"), this.getControl("HeightValue"), settings.height);
            }
        },
        GetInitInfoObject: function() {
            this.selectedCell = ASPx.HtmlEditorTableHelper.GetTableCellBySelection(this.selectionInfo.selectedElement,
                                                                            this.selectionInfo.endSelectedElement);
            if(this.selectedCell)
                return this.extractSettingsFromCell(this.selectedCell);
            return null;
        },
        extractSettingsFromCell: function(cell) {
            return null;    
        },
        attachEvents: function() {
            if(this.hasWidth())
                attachWidthChangeEvent(this);
            if(this.hasHeight())
                attachHeightChangeEvent(this);
        }
    });

    var TableCellPropertiesDialog = ASPx.CreateClass(TableElementPropertiesDialogBase, {
        getObjectSettings: function() {
            return {
                properties: TableDialogBase.prototype.getObjectSettings.call(this),
                cellElement: this.selectedCell
            };
        },
        getCommandName: function() {
            return ASPxClientCommandConsts.CHANGETABLECELL_COMMAND;    
        },
        GetDialogCaptionText: function() {
            return ASPx.HtmlEditorDialogSR.ChangeTableCell;
        },
        extractSettingsFromCell: function(cell) {
            return ASPx.HtmlEditorTableHelper.GetCellProperties(cell);
        }
    });
    var TableColumnPropertiesDialog = ASPx.CreateClass(TableElementPropertiesDialogBase, {
        hasWidth: function() {
            return true;    
        },
        hasHeight: function() {
            return false;    
        },
        getCommandName: function() {
            return ASPxClientCommandConsts.CHANGETABLECOLUMN_COMMAND;    
        },
        GetDialogCaptionText: function() {
            return ASPx.HtmlEditorDialogSR.ChangeTableColumn;
        },
        extractSettingsFromCell: function(cell) {
            return ASPx.HtmlEditorTableHelper.GetColumnProperties(cell);
        }
    });
    var TableRowPropertiesDialog = ASPx.CreateClass(TableElementPropertiesDialogBase, {
        hasWidth: function() {
            return false;    
        },
        hasHeight: function() {
            return true;    
        },
        getCommandName: function() {
            return ASPxClientCommandConsts.CHANGETABLEROW_COMMAND;    
        },
        GetDialogCaptionText: function() {
            return ASPx.HtmlEditorDialogSR.ChangeTableRow;
        },
        extractSettingsFromCell: function(cell) {
            return ASPx.HtmlEditorTableHelper.GetRowProperties(cell);
        }
    });

    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.INSERTTABLE_DIALOG_COMMAND] = new InsertTableDialog(ASPxClientCommandConsts.INSERTTABLE_DIALOG_COMMAND);
    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.TABLEPROPERTIES_DIALOG_COMMAND] = new TablePropertiesDialog(ASPxClientCommandConsts.INSERTTABLE_DIALOG_COMMAND);
    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.TABLECELLPROPERTIES_DIALOG_COMMAND] = new TableCellPropertiesDialog(ASPxClientCommandConsts.TABLECELLPROPERTIES_DIALOG_COMMAND);
    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.TABLECOLUMNPROPERTIES_DIALOG_COMMAND] = new TableColumnPropertiesDialog(ASPxClientCommandConsts.TABLECOLUMNPROPERTIES_DIALOG_COMMAND);
    ASPx.HtmlEditorDialogList[ASPxClientCommandConsts.TABLEROWPROPERTIES_DIALOG_COMMAND] = new TableRowPropertiesDialog(ASPxClientCommandConsts.TABLEROWPROPERTIES_DIALOG_COMMAND);
})();