(function() {
    ASPx.HtmlEditorClasses.Commands.Tables.Cell = {
        // cmdValue = { cellElement, cellProperties }
        // cellProperties = {vAlign, align, backgroundColor}
        Get: function(wrapper) {
            // TODO
            if (wrapper.needGetElementFromSelection("tableCell")) {
                var curSelection = ASPxClientHtmlEditorSelection.Create(wrapper.getWindow());
                var parentElem = curSelection.GetParentElement();
                var endElem = null;
                if (!ASPx.Browser.IE)
                    endElem = curSelection.GetEndContainer();
                wrapper.setSelectedElement("tableCell", ASPx.HtmlEditorTableHelper.GetTableCellBySelection(parentElem, endElem));
            }
            return wrapper.getSelectedElement("tableCell");
        },
        IsSelected: function(wrapper) {
            return !!ASPx.HtmlEditorClasses.Commands.Tables.Cell.Get(wrapper);
        },
        Change: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
            Execute: function(cmdValue, wrapper) {
                ASPx.HtmlEditorClasses.Commands.Command.prototype.Execute.apply(this, arguments);
                if (cmdValue.properties.applyForAll)
                    ASPx.HtmlEditorTableHelper.SetCellPropertiesForAllCell(ASPx.GetParentByTagName(cmdValue.cellElement, "TABLE"),
                                                                    cmdValue.properties);
                else
                    ASPx.HtmlEditorTableHelper.SetCellProperties(cmdValue.cellElement, cmdValue.properties);
                return true;
            }
        }),
        SplitHorizontally: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Tables.InsertColumnAndRowBase, {
            Execute: function(cmdValue, wrapper) {
                ASPx.HtmlEditorClasses.Commands.Tables.InsertColumnAndRowBase.prototype.Execute.apply(this, arguments);
                ASPx.HtmlEditorTableHelper.SplitCellHorizontal(ASPx.HtmlEditorClasses.Commands.Tables.Cell.Get(wrapper), wrapper);
                return true;
            }
        }),
        SplitVertically: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Tables.InsertColumnAndRowBase, {
            Execute: function(cmdValue, wrapper) {
                ASPx.HtmlEditorClasses.Commands.Tables.InsertColumnAndRowBase.prototype.Execute.apply(this, arguments);
                ASPx.HtmlEditorTableHelper.SplitCellVertical(ASPx.HtmlEditorClasses.Commands.Tables.Cell.Get(wrapper), wrapper);
                return true;
            }
        }),
        MergeRight: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Tables.InsertColumnAndRowBase, {
            Execute: function(cmdValue, wrapper) {
                ASPx.HtmlEditorClasses.Commands.Tables.InsertColumnAndRowBase.prototype.Execute.apply(this, arguments);
                ASPx.HtmlEditorTableHelper.MergeCellHorizontal(ASPx.HtmlEditorClasses.Commands.Tables.Cell.Get(wrapper), wrapper);
                return true;
            },
            IsLocked: function(core) {
                var cell = ASPx.HtmlEditorClasses.Commands.Tables.Cell.Get(core);
                if (cell)
                    return !ASPx.HtmlEditorTableHelper.IsMergeCellHorizontalAllow(cell, core);
                return true;
            }
        }),
        MergeLeft: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Tables.InsertColumnAndRowBase, {
            Execute: function(cmdValue, wrapper) {
                ASPx.HtmlEditorClasses.Commands.Tables.InsertColumnAndRowBase.prototype.Execute.apply(this, arguments);
                var currentCell = ASPx.HtmlEditorClasses.Utils.getPreviousElementSibling(ASPx.HtmlEditorClasses.Commands.Tables.Cell.Get(wrapper));
                if(currentCell)
                    ASPx.HtmlEditorTableHelper.MergeCellHorizontal(currentCell, wrapper);
                return true;
            },
            IsLocked: function(core) {
                var cell = ASPx.HtmlEditorClasses.Utils.getPreviousElementSibling(ASPx.HtmlEditorClasses.Commands.Tables.Cell.Get(core));
                if (cell)
                    return !ASPx.HtmlEditorTableHelper.IsMergeCellHorizontalAllow(cell, core);
                return true;
            }
        }),
        MergeDown: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Tables.InsertColumnAndRowBase, {
            Execute: function(cmdValue, wrapper) {
                ASPx.HtmlEditorClasses.Commands.Tables.InsertColumnAndRowBase.prototype.Execute.apply(this, arguments);
                ASPx.HtmlEditorTableHelper.MergeCellVertical(ASPx.HtmlEditorClasses.Commands.Tables.Cell.Get(wrapper), wrapper);
                return true;
            },
            IsLocked: function(wrapper) {
                var cell = ASPx.HtmlEditorClasses.Commands.Tables.Cell.Get(wrapper);
                if (cell)
                    return !ASPx.HtmlEditorTableHelper.IsMergeCellVerticalAllow(cell, wrapper);
                return true;
            }
        })
    };
})();