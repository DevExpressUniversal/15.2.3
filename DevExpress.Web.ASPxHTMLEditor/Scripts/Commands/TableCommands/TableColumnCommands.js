(function() {
    ASPx.HtmlEditorClasses.Commands.Tables.Column = {
        Insert: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Tables.InsertColumnAndRowBase, {
            Execute: function(cmdValue, wrapper) {
                ASPx.HtmlEditorClasses.Commands.Tables.InsertColumnAndRowBase.prototype.Execute.apply(this, arguments);
                var cell = ASPx.HtmlEditorClasses.Commands.Tables.Cell.Get(wrapper);
                if (cell) {
                    ASPx.HtmlEditorTableHelper.InsertColumn(cell, this.GetShift(wrapper.settings.rtl), wrapper);
                    wrapper.focus();
                    return true;
                }
                return false;
            },
            GetShift: function(rtl) {
                if(rtl)
                    return this.commandID == ASPxClientCommandConsts.INSERTTABLECOLUMNTORIGHT_COMMAND ? 0 : 1;
                else
                    return this.commandID == ASPxClientCommandConsts.INSERTTABLECOLUMNTORIGHT_COMMAND ? 1 : 0;
            }
        }),
        Change: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
            Execute: function(cmdValue, wrapper) {
                ASPx.HtmlEditorClasses.Commands.Command.prototype.Execute.apply(this, arguments);
                ASPx.HtmlEditorTableHelper.SetColumnProperties(cmdValue.cell, cmdValue.properties);
                return true;
            }
        }),
        Delete: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Tables.DeleteColumnAndRowBase, {
            Execute: function(cmdValue, wrapper) {
                ASPx.HtmlEditorClasses.Commands.Tables.DeleteColumnAndRowBase.prototype.Execute.apply(this, arguments);
                var curSelection = ASPxClientHtmlEditorSelection.Create(wrapper.getWindow());
                var selElem = curSelection.GetParentElement();

                var table = ASPx.HtmlEditorTableHelper.GetTable(selElem);
                var row = ASPx.HtmlEditorTableHelper.GetTableRow(selElem);
                var cell = ASPx.HtmlEditorClasses.Commands.Tables.Cell.Get(wrapper);
                return ASPx.HtmlEditorTableHelper.RemoveColumn(cell, wrapper);
            }
        })
    };
})();