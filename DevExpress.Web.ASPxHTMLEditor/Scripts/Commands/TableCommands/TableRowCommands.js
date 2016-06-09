(function() {

    ASPx.HtmlEditorClasses.Commands.Tables.Row = {
        Insert: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Tables.InsertColumnAndRowBase, {
            Execute: function(cmdValue, wrapper) {
                ASPx.HtmlEditorClasses.Commands.Tables.InsertColumnAndRowBase.prototype.Execute.apply(this, arguments);

                var cell = ASPx.HtmlEditorClasses.Commands.Tables.Cell.Get(wrapper);

                if (cell) {
                    ASPx.HtmlEditorTableHelper.InsertRow(cell, this.GetShift(), wrapper);

                    wrapper.focus();
                    return true;
                }
                return false;
            },
            GetShift: function() {
                var ret = 0; // Left
                if (this.commandID == ASPxClientCommandConsts.INSERTTABLEROWBELOW_COMMAND)
                    ret = 1;
                return ret;
            }
        }),
        Change: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
            Execute: function(cmdValue, wrapper) {
                ASPx.HtmlEditorClasses.Commands.Command.prototype.Execute.apply(this, arguments);
                ASPx.HtmlEditorTableHelper.SetRowProperties(cmdValue.cell, cmdValue.properties);
                return true;
            }
        }),
        Delete: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Tables.DeleteColumnAndRowBase, {
            Execute: function(cmdValue, wrapper) {
                ASPx.HtmlEditorClasses.Commands.Tables.DeleteColumnAndRowBase.prototype.Execute.apply(this, arguments);
                var element = null;
                if(cmdValue && cmdValue.selectedElement)
                    element = ASPx.HtmlEditorTableHelper.getFirstCellFromContent(cmdValue.selectedElement); 
                else
                    element = ASPx.HtmlEditorClasses.Commands.Tables.Cell.Get(wrapper);
                return ASPx.HtmlEditorTableHelper.RemoveRow(element, wrapper);
            }
        })
    };
})();