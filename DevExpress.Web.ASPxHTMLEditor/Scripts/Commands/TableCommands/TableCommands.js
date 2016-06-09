    /*region* * * * * * * * * * * * * * *  Table command  * * * * * * * * * * * * * * * */

(function() {
    var emptyBorderTableClassName = "dxEmptyBorderTable";
    var sizeTypeRegExp = new RegExp("([0-9]*[,.]?[0-9]*)(px|%)");

    var XHTMLCellPaddingProperty = ASPx.CreateClass(null, {
        constructor: function(table) {
            this.table = table;    
        },
        get: function() {
            return this.getCellPadding();    
        },
        set: function(value) {
            this.table.cellPadding = value;
            this.forEachCell(function(cell) { cell.style.padding = ""; });
        },
        getCellPadding: function() {
            return parseInt(this.table.cellPadding) || 0;        
        },
        forEachCell: function(func) {
            var cells = ASPx.GetNodesByTagName(this.table, "TD");
            for(var i = 0; i < cells.length; i++) {
                if(func(cells[i]))
                    return;
            }
        }
    });

    var HTML5CellPaddingProperty = ASPx.CreateClass(XHTMLCellPaddingProperty, {
        get: function() {
            return this.getCellsCommonPadding() || 0;
        },
        set: function(value) {
            this.table.cellPadding = 0;
            this.forEachCell(function(cell) { ASPx.SetStyles(cell, { padding: value }); });
        },
        getCellsCommonPadding: function() {
            var result = null;
            this.forEachCell(function(cell) {
                if(!cell.style.padding)
                    return true;
                var padding = ASPx.PxToInt(cell.style.padding);
                if(result === null)
                    result = padding;
                else if(padding !== result) {
                    result = null;
                    return true;
                }
            });
            return result;    
        }
    });

    var BothCellPaddingProperty = ASPx.CreateClass(HTML5CellPaddingProperty, {
        get: function() {
            var html5Value = this.getCellsCommonPadding();
            return html5Value === null ? this.getCellPadding() : html5Value;
        }
    });

    var XHTMLCellSpacingProperty = ASPx.CreateClass(null, {
        constructor: function(table) {
            this.table = table;    
        },
        get: function() {
            return parseInt(this.table.cellSpacing) || 0;
        },
        set: function(value) {
            this.table.cellSpacing = value;
            if(this.table.style.borderSpacing)
                this.table.style.borderSpacing = "";
        }    
    });

    var HTML5CellSpacingProperty = ASPx.CreateClass(XHTMLCellSpacingProperty, {
        get: function() {
            return (this.table.style.borderSpacing && ASPx.PxToInt(this.table.style.borderSpacing)) || 0;    
        },
        set: function(value) {
            ASPx.SetStyles(this.table, { borderSpacing: value });
            this.table.cellSpacing = 0;
        }
    });

    var BothCellSpacingProperty = ASPx.CreateClass(HTML5CellSpacingProperty, {
        get: function() {
            return (this.table.style.borderSpacing && ASPx.PxToInt(this.table.style.borderSpacing)) || parseInt(this.table.cellSpacing) || 0;
        }
    });

    var HtmlEditorTableHelper = {
         isTableCellElement: function(el) {
            return !!el && el.nodeType == 1 && (el.tagName.toUpperCase() == "TD" || el.tagName.toUpperCase() == "TH");    
        },
        getFirstCellFromContent: function(element) {
            if(element && element.nodeType == 1)
                return this.isTableCellElement(element) ? element : ASPx.GetNodes(element, this.isTableCellElement.aspxBind(this))[0];
            return null;
        },
        HasEmptyBorderClassName: function(table) {
            return table.className.indexOf(emptyBorderTableClassName) > -1;
        },
        GetParentWithEmptyBorderClassName: function(elem) {
            while(elem && elem.nodeName != "BODY") {
                if(HtmlEditorTableHelper.HasEmptyBorderClassName(elem))
                    return elem;
                elem = elem.parentNode;
            }
            return null;
        },
        AppendEmptyBorderClassName: function(elem) {
            if (HtmlEditorTableHelper.HasEmptyBorderClassName(elem))
                return;
            elem.className += " " + emptyBorderTableClassName;
        },
        IsEmptyBorder: function(table) {
            if (!table)
                return false;

            return HtmlEditorTableHelper.GetTableBorderWidth(table) == 0;
        },
        GetBorderColor: function(style) {
            return style.borderColor || style.borderLeftColor || style.borderRightColor || style.borderTopColor || style.borderBottomColor;
        },
        GetBorderStyle: function(style) {
            return style.borderStyle || style.borderLeftStyle || style.borderRightStyle || style.borderTopStyle || style.borderBottomStyle;
        },
        RemoveEmptyBorderClassName: function(elem) {
            elem.className = elem.className.replace(emptyBorderTableClassName, "");
        },
        GetTableBorderWidth: function(table) {
            var width = -1;
            for(var i = 0, row; row = table.rows[i]; i++) {
                for(var j = 0, cell; cell = row.cells[j]; j++) {
                    if(cell.style.display == "none")
                        continue;
                    if(width < 0)
                        width = ASPx.PxToInt(cell.style.borderWidth);
                    else if(width != ASPx.PxToInt(cell.style.borderWidth))
                        return 0;
                }
            }
            return width;
        },
        getTableCellPadding: function(table) {
            return this.getCellPaddingProperty(table).get();    
        },
        getTableCellSpacing: function(table) {
            return this.getCellSpacingProperty(table).get();
        },
        setTableCellPadding: function(table, value) {
            this.getCellPaddingProperty(table).set(value);
        },
        setTableCellSpacing: function(table, value) {
            this.getCellSpacingProperty(table).set(value);
        },
        getCellPaddingProperty: function(element) {
            switch(this.getDocType(element)) {
                case ASPx.HtmlEditorClasses.DocumentType.Both:
                    return new BothCellPaddingProperty(element);
                case ASPx.HtmlEditorClasses.DocumentType.HTML5:
                    return new HTML5CellPaddingProperty(element);
                default:
                    return new XHTMLCellPaddingProperty(element);
            }
        },
        getCellSpacingProperty: function(element) {
            switch(this.getDocType(element)) {
                case ASPx.HtmlEditorClasses.DocumentType.Both:
                    return new BothCellSpacingProperty(element);
                case ASPx.HtmlEditorClasses.DocumentType.HTML5:
                    return new HTML5CellSpacingProperty(element);
                default:
                    return new XHTMLCellSpacingProperty(element);
            }
        },
        getDocType: function(element) {
            var body = element.ownerDocument.body;
            var className = body.className || "";
            var prefix = "dxhe-docType";
            if(className.indexOf(prefix + ASPx.HtmlEditorClasses.DocumentType.HTML5) > -1)
                return ASPx.HtmlEditorClasses.DocumentType.HTML5;
            if(className.indexOf(prefix + ASPx.HtmlEditorClasses.DocumentType.Both) > -1)
                return ASPx.HtmlEditorClasses.DocumentType.Both;
            return ASPx.HtmlEditorClasses.DocumentType.XHTML;
        },

        GetTable: function(element) {
            if(!element) return null;
            return element.tagName == "TABLE" ? element : ASPx.GetParentByTagName(element, "TABLE");
        },
        GetTableRow: function(elem) {
            if (elem && elem.nodeType == 1 && elem.tagName.toUpperCase() == "TR")
                return elem;

            var cell = this.GetTableCell(elem);
            if (cell)
                return cell.parentNode;
            return null;
        },
        GetTableCellBySelection: function(startElem, endElem) {
            var cell = HtmlEditorTableHelper.GetTableCell(startElem);

            if (!cell && endElem) {
                cell = HtmlEditorTableHelper.GetTableCell(endElem);

                if (ASPx.Browser.Safari && startElem && startElem.tagName.toUpperCase() == "TR") { // HACK for Safari 
                    var index = cell.cellIndex - 1;
                    if (index < 0)
                        cell = startElem.cells[startElem.cells.length - 1];
                    else
                        cell = startElem.cells[index];
                }
            }
            return cell;
        },
        GetTableCell: function(element) {
            if (!element)
                return null;
            if (element.nodeType == "1" &&
                (element.tagName.toUpperCase() == "TD" || element.tagName.toUpperCase() == "TH"))
                return element;
            var cell = ASPx.GetParentByTagName(element, "TD");
            if (!cell)
                cell = ASPx.GetParentByTagName(element, "TH");
            return cell;
        },

        //  TableProperties = { 
        //      borderWidth, borderColor, 
        //      backgroundColor,
        //      width, height,
        //      cellPadding, cellSpacing, align, accessibility
        //      
        // }
        GetTableProperties: function(table) {
            var tableInfoObject = {
                borderWidth: 0,
                borderColor: null,
                backgroundColor: null,
                width: null,
                height: null,
                cellPadding: 0,
                cellSpacing: 0,
                align: null,
                accessibility: null
            };
            if (table.className.indexOf(emptyBorderTableClassName) == -1) {
                tableInfoObject.borderWidth = this.GetTableBorderWidth(table);
                var borderColor = this.GetTableBorderColor(table);
                if (ASPx.IsExists(borderColor))
                    tableInfoObject.borderColor = borderColor;
            }
            if (table.style.backgroundColor)
                tableInfoObject.backgroundColor = table.style.backgroundColor;

            if (table.style.width)
                tableInfoObject.width = table.style.width;
            if (table.style.height)
                tableInfoObject.height = table.style.height;

            tableInfoObject.cellPadding = this.getTableCellPadding(table);
            tableInfoObject.cellSpacing = this.getTableCellSpacing(table);

            if (table.align)
                tableInfoObject.align = table.align;
            var accessibility = { caption: "", summary: "", headers: "" };
            accessibility.headers = this.GetAccessibilityHeadersValue(table);
            accessibility.caption = this.GetAccessibilityCaption(table);
            accessibility.summary = table.summary;

            tableInfoObject.accessibility = accessibility;

            return tableInfoObject;
        },
        SetTableProperties: function(table, properties, wrapper, owner) {
            if (properties) {
                if (ASPx.IsExists(properties.borderWidth))
                    this.SetTableBorderWidth(table, properties.borderWidth, wrapper, owner);
                if (ASPx.IsExists(properties.borderColor))
                    this.SetTableBorderColor(table, properties.borderColor);

                this.SetBackgroundColor(table, properties.backgroundColor);

                if(ASPx.IsExists(properties.cellSpacing))
                    this.setTableCellSpacing(table, properties.cellSpacing);
                if(ASPx.IsExists(properties.cellPadding))
                    this.setTableCellPadding(table, properties.cellPadding);

                if (properties.width)
                    table.style.width = properties.width;
                else if (ASPx.IsExists(table.style.width)) {
                    ASPx.Attr.RemoveAttribute(table, "width");
                    ASPx.Attr.RemoveStyleAttribute(table, "width");
                }

                if (properties.height)
                    table.style.height = properties.height;
                else if (ASPx.IsExists(table.style.height)) {
                    ASPx.Attr.RemoveAttribute(table, "height");
                    table.style.height = "";
                    ASPx.Attr.RemoveStyleAttribute(table, "height");
                }

                if (properties.align && !/^none$/i.test(properties.align))
                    table.align = properties.align;
                else if (ASPx.IsExists(table.align)) {
                    table.align = "";
                    ASPx.Attr.RemoveAttribute(table, "align");
                }
                if (properties.accessibility)
                    this.SetTableAccessibility(table, properties.accessibility);
            }
            table.style.borderCollapse = (properties && properties.cellSpacing > 0) ? "" : "collapse";  
        },
        SetTableBorderWidth: function(table, borderWidth, wrapper, owner) {
            if (ASPx.Browser.Firefox && table.style.borderCollapse == "collapse") {
                table.style.borderCollapse = "separate";
                window.setTimeout(function() { 
                    this.SetTableBorderWidthCore(table, borderWidth);
                    table.style.borderCollapse = "collapse";
                    this.SetEmptyTableBorderClass(table);
                    // Hack - When borderWidth is set and borderCollapse is "collapse" then table isn't refresh.
                    //        To refresh, we have to set borderWidth with delay
                    //        To perfrom correct Undo operation, we have to update last item in restoreHtmlArray
                    owner.updateLastItemInRestoreHtmlArray();
                }.aspxBind(this), 100);
            }
            else {
                this.SetTableBorderWidthCore(table, borderWidth);
                this.SetEmptyTableBorderClass(table);
            }
        },
        SetEmptyTableBorderClass: function(table) {
            if (this.IsEmptyBorder(table))
                this.AppendEmptyBorderClassName(table);
            else
                this.RemoveEmptyBorderClassName(table);
        },
        // *** Cell *** 
        GetCellProperties: function(cell) {
            var cellInfo = {
                backgroundColor: null,
                align: null,
                vAlign: null
            };
            if (cell.style.textAlign)
                cellInfo.align = cell.style.textAlign;
            if (cell.style.verticalAlign)
                cellInfo.vAlign = cell.style.verticalAlign;

            if (cell.style.backgroundColor)
                cellInfo.backgroundColor = cell.style.backgroundColor;
            return cellInfo;
        },
        // properties = {vAlign, align, backgroundColor}
        SetCellProperties: function(cell, properties) {
            if (properties.vAlign)
                cell.style.verticalAlign = properties.vAlign.toLowerCase();
            else if (ASPx.IsExists(cell.vAlign))
                ASPx.Attr.RemoveStyleAttribute(cell, "verticalAlign");

            if (properties.align)
                cell.style.textAlign = properties.align.toLowerCase();
            else if (ASPx.IsExists(cell.align))
                ASPx.Attr.RemoveStyleAttribute(cell, "textAlign");

            if(!properties.cantChangeBackground)
                this.SetBackgroundColor(cell, properties.backgroundColor);
        },
        SetCellPropertiesForAllCell: function(table, properties) {
            for (var i = 0; i < table.rows.length; i++) {
                for (var j = 0; j < table.rows[i].cells.length; j++)
                    this.SetCellProperties(this.GetCell(table, i, j), properties);
            }
        },
        // *** Column ***
        // properties = {width, vAlign, align, backgroundColor}
        SetColumnProperties: function(cell, properties) {
            var table = ASPx.GetParentByTagName(cell, "TABLE");
            var row = cell.parentNode;
            var tableModel = HtmlEditorTableHelper.CreateTableModel(table);
            var columnIndex = this.GetColumnIndexByTableModel(tableModel, row.rowIndex, cell);
            var colSpan = this.GetColSpan(cell);

            for (var i = 0; i < tableModel.length; i++) {
                for (var j = columnIndex; j < columnIndex + colSpan; j++) {
                    var curCell = tableModel[i][j];

                    if (this.GetColSpan(curCell) <= colSpan) {
                        this.SetCellProperties(curCell, properties);
                        if (properties.width)
                            curCell.style.width = properties.width;
                        else if (ASPx.IsExists(curCell.style.width)) {
                            curCell.style.width = "";
                            ASPx.Attr.RemoveStyleAttribute(curCell, "width");
                        }
                    }
                }
            }
        },
        GetColumnProperties: function(cell) {
            var columnInfo = {
                backgroundColor: null,
                width: null,
                align: null,
                vAlign: null
            };

            var table = ASPx.GetParentByTagName(cell, "TABLE");
            var row = cell.parentNode;
            var tableModel = HtmlEditorTableHelper.CreateTableModel(table);
            var columnIndex = this.GetColumnIndexByTableModel(tableModel, row.rowIndex, cell);
            var colSpan = this.GetColSpan(cell);

            var curCellInfo = this.GetCellProperties(cell);
            var align = columnInfo.align = curCellInfo.align;
            var vAlign = columnInfo.vAlign = curCellInfo.vAlign;
            var backgroundColor = columnInfo.backgroundColor = curCellInfo.backgroundColor;

            var maxCellWidth = 0;
            var isWidthDefined = true;

            for (var i = 0; i < tableModel.length; i++) {
                for (var j = columnIndex; j < columnIndex + colSpan; j++) {
                    var curCell = tableModel[i][j];

                    if (this.GetColSpan(curCell) <= colSpan) {
                        var curCellInfo = this.GetCellProperties(curCell);
                        if (curCell.style.width && ASPx.GetClearClientWidth(curCell) > maxCellWidth)
                            maxCellWidth = curCell.style.width;
                        if (align != curCellInfo.align)
                            columnInfo.align = null;
                        if (vAlign != curCellInfo.vAlign)
                            columnInfo.vAlign = null;
                        if (backgroundColor != curCellInfo.backgroundColor) {
                            columnInfo.hasMultipleColors = true;
                            columnInfo.backgroundColor = null;
                        }

                        if (isWidthDefined) {
                            var curCellColSpan = this.GetColSpan(curCell);
                            if (curCellColSpan != colSpan)
                                isWidthDefined = false;
                            else if (columnIndex != this.GetColumnIndexByTableModel(tableModel, i, curCell))
                                isWidthDefined = false;
                        }
                    }
                }
            }

            if (isWidthDefined && this.ParseSizeString(maxCellWidth).value > 0)
                columnInfo.width = maxCellWidth;
            return columnInfo;
        },

        // *** Row ***
        GetRowProperties: function(cell) {
            var rowInfo = {
                backgroundColor: null,
                height: null,
                align: null,
                vAlign: null
            };
            var table = ASPx.GetParentByTagName(cell, "TABLE");
            var row = cell.parentNode;
            var tableModel = HtmlEditorTableHelper.CreateTableModel(table);
            var rowSpan = this.GetRowSpan(cell);

            var curCellInfo = this.GetCellProperties(cell);
            var align = rowInfo.align = curCellInfo.align;
            var vAlign = rowInfo.vAlign = curCellInfo.vAlign;
            var backgroundColor = rowInfo.backgroundColor = curCellInfo.backgroundColor;

            var maxCellHeight = 0;
            var isHeightDefined = true;

            for (var i = row.rowIndex; i < row.rowIndex + rowSpan; i++) {
                for (var j = 0; j < tableModel[i].length; j++) {
                    var curCell = tableModel[i][j];

                    if (this.GetRowSpan(curCell) <= rowSpan) {
                        var curCellInfo = this.GetCellProperties(curCell);

                        if (curCell.style.height && ASPx.GetClearClientHeight(curCell) > maxCellHeight)
                            maxCellHeight = curCell.style.height;
                        if (align != curCellInfo.align)
                            rowInfo.align = null;
                        if (vAlign != curCellInfo.vAlign)
                            rowInfo.vAlign = null;
                        if (backgroundColor != curCellInfo.backgroundColor) {
                            rowInfo.hasMultipleColors = true;
                            rowInfo.backgroundColor = null;
                        }

                        if (isHeightDefined) {
                            var curCellRowSpan = this.GetRowSpan(curCell);
                            if (curCellRowSpan != rowSpan)
                                isHeightDefined = false;
                            else if (row.rowIndex != curCell.parentNode.rowIndex)
                                isHeightDefined = false;
                        }
                    }
                }
            }

            if (isHeightDefined && this.ParseSizeString(maxCellHeight).value > 0)
                rowInfo.height = maxCellHeight;
            return rowInfo;
        },
        // properties = {height, vAlign, align, backgroundColor}
        SetRowProperties: function(cell, properties) {
            var table = ASPx.GetParentByTagName(cell, "TABLE");
            var row = cell.parentNode;
            var tableModel = HtmlEditorTableHelper.CreateTableModel(table);
            var columnIndex = this.GetColumnIndexByTableModel(tableModel, row.rowIndex, cell);
            var rowSpan = this.GetRowSpan(cell);
            var rowIndex = row.rowIndex;

            for (var i = rowIndex; i < rowIndex + rowSpan; i++) {
                for (var j = 0; j < tableModel[i].length; j++) {
                    if (this.GetRowSpan(tableModel[i][j]) <= rowSpan) {
                        //B142212
                        this.SetCellProperties(tableModel[i][j], properties);
                        if (properties.height)
                            tableModel[i][j].style.height = properties.height;
                        else if (ASPx.IsExists(tableModel[i][j].style.width)) {
                            tableModel[i][j].style.height = "";
                            ASPx.Attr.RemoveStyleAttribute(tableModel[i][j], "height");
                        }
                    }
                }
            }
        },

        /*region* * * * * * * * * * * * * * * * * *  Accessibility  * * * * * * * * * * * * * * * * * * */

        // properties = { summary, caption, headers = (null, 'both', 'column', row) }
        SetTableAccessibility: function(table, properties) {
            if (properties.caption) {
                var caption = table.createCaption();
                caption.innerHTML = properties.caption;
            }
            else
                table.deleteCaption();

            if (properties.summary)
                table.summary = properties.summary;
            else
                ASPx.Attr.RemoveAttribute(table, "summary");

            var hasColumnHeaders = this.GetHasColumnHeaders(table);
            var headers = properties.headers;

            this.SetRowAndBothHeaders(table, headers);
            this.SetColumnAndBothHeaders(table, headers, hasColumnHeaders);
        },
        SetColumnAndBothHeaders: function(table, headers, hasColumnHeaders) {
            if ((headers == 'column' || headers == 'both') && !this.GetHasColumnHeaders(table)) {
                for (var i = 0; i < table.rows.length; i++) {
                    var newCell = ASPx.ReplaceTagName(table.rows[i].cells[0], 'TH');
                    if (newCell != null && !(headers == 'both' && newCell.scope == 'col'))
                        newCell.scope = 'row';
                }
            }
            if (hasColumnHeaders && headers != 'column' && headers != 'both') {
                for (var i = 0; i < table.rows.length; i++) {
                    var row = table.rows[i];
                    if (row.parentNode.nodeName.toUpperCase() == 'TBODY') {
                        var newCell = ASPx.ReplaceTagName(row.cells[0], 'TD');
                        if (newCell != null)
                            newCell.removeAttribute('scope');
                    }
                }
            }
        },
        SetRowAndBothHeaders: function(table, headers) {
            var hasRowHeaders = this.GetHasRowHeaders(table);
            if (!hasRowHeaders && (headers == 'row' || headers == 'both')) {
                var firstRow = table.rows[0];

                for (var i = 0; i < firstRow.childNodes.length; i++) {
                    if (firstRow.childNodes[i].nodeType == 1) {
                        var th = ASPx.ReplaceTagName(firstRow.childNodes[i], 'TH');
                        if (th)
                            th.scope = 'col';
                    }
                }
                var tHead = table.tHead;
                if (!tHead)
                    tHead = table.createTHead();

                if (tHead.childNodes.length == 0)
                    tHead.appendChild(firstRow);

                for (var i = table.tBodies.length - 1; i >= 0; i--) {
                    if (ASPx.Str.Trim(table.tBodies[i].innerHTML) == "")
                        table.removeChild(table.tBodies[i]);
                }
            }
            if (hasRowHeaders && headers != 'row' && headers != 'both') {
                var firstRow = table.rows[0];
                for (var i = 0; i < firstRow.cells.length; i++) {
                    var newCell = ASPx.ReplaceTagName(firstRow.cells[i], "TD");
                    if (newCell != null)
                        ASPx.Attr.RemoveAttribute(newCell, "scope");
                }
                if (table.tHead && table.tHead.childNodes.length > 0) {
                    var firstRow = table.tBodies[0].firstChild;
                    for (var i = table.tHead.childNodes.length - 1; i >= 0; i--) {
                        if (table.tHead.childNodes[i].nodeType == 1)
                            firstRow = table.tBodies[0].insertBefore(table.tHead.childNodes[i], firstRow);
                    }
                    table.removeChild(table.tHead);
                }
            }
        },
        GetAccessibilityHeadersValue: function(table) {
            var hasColumnHeaders = this.GetHasColumnHeaders(table);
            var hasRowHeaders = this.GetHasRowHeaders(table);
            var ret = null;
            if (hasColumnHeaders && hasRowHeaders)
                ret = "both";
            else if (hasColumnHeaders)
                ret = "column";
            else if (hasRowHeaders)
                ret = "row";
            return ret;
        },
        GetAccessibilityCaption: function(table) {
            var caption = ASPx.GetNodeByTagName(table, "CAPTION", 0);
            if (caption)
                return caption.innerHTML;
            return "";
        },
        GetHasColumnHeaders: function(table) {
            for (var i = 0; i < table.rows.length; i++) {
                if (table.rows[i].cells[0].nodeName.toUpperCase() != 'TH')
                    return false;
            }
            return true;
        },
        GetHasRowHeaders: function(table) {
            if (table.rows.length == 0)
                return false;

            for (var i = 0; i < table.rows[0].cells.length; i++) {
                if (table.rows[0].cells[i].nodeName.toUpperCase() != 'TH')
                    return false;
            }
            return true;
        },

        /*region* * * * * * * * * * * * * * * * * *  Table operation  * * * * * * * * * * * * * * * * * * */
        IsTableColumnEqual: function(table) {
            if (table.rows.length < 0)
                return false;

            var cellWidth = table.rows[0].cells[0].style.width;
            if (!cellWidth)
                return false;

            for (var i = 0; i < table.rows.length; i++) {
                var curRow = table.rows[i];
                for (var j = 0; j < curRow.cells.length; j++)
                    if (cellWidth && cellWidth != this.GetCell(table, i, j).style.width)
                    return false;
            }
            return true;
        },
        AdjustColumnWidth: function(table) {
            var cellWidth = 100 / table.rows[0].cells.length + "%";
            for (var i = 0; i < table.rows.length; i++) {
                var curRow = table.rows[i];
                for (var j = 0; j < curRow.cells.length; j++) {
                    ASPx.Attr.RemoveAttribute(this.GetCell(table, i, j), "width");
                    this.GetCell(table, i, j).style.width = cellWidth;
                }
            }
        },

        // shift = {0,1}; 0 - Left; 1 - Right
        InsertColumn: function(cell, shift, wrapper) {
            if (cell.nodeName.toUpperCase() != "TD" && cell.nodeName.toUpperCase() != "TH")
                return "";

            var table = this.GetTable(cell);
            var tableModel = this.CreateTableModel(table);

            var row = cell.parentNode;
            var columnIndex = this.GetColumnIndexByTableModel(tableModel, row.rowIndex, cell);
            var colSpan = this.GetColSpan(cell);

            var nextColumnIndex = -1;
            if (shift > 0)
                nextColumnIndex = columnIndex + colSpan;
            else
                nextColumnIndex = columnIndex;

            var isTableColumnEqual = this.IsTableColumnEqual(table);
            var doc = ASPx.GetElementDocument(cell);

            for (var r = 0; r < tableModel.length; r++) {

                var curCell = shift > 0 ? tableModel[r][nextColumnIndex - 1] : tableModel[r][nextColumnIndex];
                if(!curCell) continue;

                var curCellColumnIndex = this.GetColumnIndexByTableModel(tableModel, r, curCell);
                var curCellColSpan = this.GetColSpan(curCell);

                if ((shift > 0 && curCellColumnIndex + curCellColSpan == columnIndex + colSpan) ||
                    (shift == 0 && curCellColumnIndex == columnIndex)) {

                    var curCellRowSpan = this.GetRowSpan(curCell);
                    var newCell = this.CreateNewCellByCell(tableModel[r][columnIndex], true);

                    for (var j = r; j < r + curCellRowSpan; j++)
                        ASPx.Data.ArrayInsert(tableModel[j], newCell, nextColumnIndex);
                    r = j - 1;
                }
                else
                    ASPx.Data.ArrayInsert(tableModel[r], curCell, nextColumnIndex);
            }

            var rowIndex = row.rowIndex;

            this.GenerateTableFromModel(tableModel, table);
            if (isTableColumnEqual)
                this.AdjustColumnWidth(table);

            if (wrapper)
                this.SelectCellCore(tableModel[rowIndex][nextColumnIndex], wrapper);
        },
        // shift = {0,1}; 0 - Above; 1 - Below
        InsertRow: function(cell, shift, wrapper) {
            if (cell.nodeName.toUpperCase() != "TD" && cell.nodeName.toUpperCase() != "TH")
                return "";

            var table = this.GetTable(cell);
            var tableModel = this.CreateTableModel(table);

            var row = cell.parentNode;
            var rowIndex = row.rowIndex;
            var columnIndex = this.GetColumnIndexByTableModel(tableModel, rowIndex, cell);
            var rowSpan = this.GetRowSpan(cell);
            var doc = ASPx.GetElementDocument(cell);

            var newRowIndex = -1;
            var rowIndexInModel = -1;

            if (shift > 0) {
                rowIndexInModel = rowIndex + rowSpan - 1;
                newRowIndex = rowIndex + rowSpan + shift - 1;
            }
            else {
                newRowIndex = rowIndex;
                rowIndexInModel = rowIndex;
            }
            var newRowModel = [ ];

            for (var i = 0; i < tableModel[rowIndexInModel].length; i++) {
                var curCell = tableModel[rowIndexInModel][i];
                var curCellRow = curCell.parentNode;
                var curCellRowSpan = this.GetRowSpan(curCell);

                if ((shift > 0 && (curCellRow.rowIndex + curCellRowSpan == rowIndex + rowSpan)) ||
                    (shift == 0 && (curCellRow.rowIndex == rowIndex))) {

                    var curCellColSpan = this.GetColSpan(curCell);
                    var newCell = this.CreateNewCellByCell(curCell);

                    for (var j = i; j < i + curCellColSpan; j++)
                        newRowModel[j] = newCell;
                    i = j - 1;
                }
                else
                    newRowModel[i] = tableModel[rowIndexInModel][i];
            }
            ASPx.Data.ArrayInsert(tableModel, newRowModel, newRowIndex);

            var newRow = null;
            if (ASPx.GetParentByTagName(cell, "THEAD"))
                newRow = table.tHead.insertRow(newRowIndex);
            else
                newRow = table.insertRow(newRowIndex);

            ASPx.Attr.CopyAllAttributes(row, newRow);
            ASPx.Attr.RemoveAttribute(newRow, "id");

            this.GenerateTableFromModel(tableModel, table);
            if (wrapper)
                this.SelectCellCore(tableModel[newRowIndex][columnIndex], wrapper);
        },
        RemoveRow: function(cell, wrapper) {
            var row = HtmlEditorTableHelper.GetTableRow(cell);
            if (row) {
                var table = this.GetTable(cell);
                var tableModel = this.CreateTableModel(table);

                var columnIndex = this.GetColumnIndexByTableModel(tableModel, row.rowIndex, cell);
                var rowIndex = row.rowIndex;
                var rowSpan = this.GetRowSpan(tableModel[rowIndex][columnIndex]);
                for (var r = rowSpan + rowIndex - 1; r >= rowIndex; r--) {
                    ASPx.Data.ArrayRemoveAt(tableModel, r);
                    if (r < table.rows.length)
                        table.deleteRow(r);
                }

                if (table.rows.length > 0) {
                    this.GenerateTableFromModel(tableModel, table);
                    if (wrapper) {
                        rowIndex = Math.min(rowIndex, table.rows.length - 1);
                        this.SelectCellCore(tableModel[rowIndex][columnIndex], wrapper);
                    }
                }
                else
                    ASPx.RemoveElement(table);
                return true;
            }
            return false;
        },
        RemoveColumn: function(cell, wrapper) {
            var row = HtmlEditorTableHelper.GetTableRow(cell);
            if (row) {
                var table = this.GetTable(cell);
                var tableModel = this.CreateTableModel(table);

                var columnIndex = this.GetColumnIndexByTableModel(tableModel, row.rowIndex, cell);
                var rowIndex = row.rowIndex;
                var colSpan = this.GetColSpan(tableModel[rowIndex][columnIndex]);
                for (var r = 0; r < tableModel.length; r++) {
                    for (var c = columnIndex + colSpan - 1; c >= columnIndex; c--) {
                        ASPx.Data.ArrayRemoveAt(tableModel[r], c);
                    }
                }

                if (tableModel[rowIndex].length > 0) {
                    this.GenerateTableFromModel(tableModel, table);
                    if (wrapper) {
                        columnIndex = Math.min(columnIndex, tableModel[rowIndex].length - 1);
                        this.SelectCellCore(tableModel[rowIndex][columnIndex], wrapper);
                    }
                }
                else
                    ASPx.RemoveElement(table);
                return true;
            }
            return false;
        },
        SplitCellHorizontal: function(cell, wrapper) {
            if (!cell)
                return false;

            var table = ASPx.GetParentByTagName(cell, "TABLE");
            var row = cell.parentNode;
            var rowIndex = row.rowIndex;
            var tableModel = HtmlEditorTableHelper.CreateTableModel(table);
            var columnIndex = this.GetColumnIndexByTableModel(tableModel, row.rowIndex, cell);
            var colSpan = isNaN(cell.colSpan) ? 1 : cell.colSpan;

            var doc = ASPx.GetElementDocument(table);

            var newCellWidth = "";
            var newCell = doc.createElement(cell.nodeName);
            newCell.style.cssText = cell.style.cssText;
        
            this.AddDefaultContentToCell(newCell);

            if (colSpan > 1) {
                var newColSpan = Math.ceil(colSpan / 2);

                var startIdx = columnIndex + newColSpan;
                var endIdx = columnIndex + colSpan;
                var rowSpan = isNaN(cell.rowSpan) ? 1 : cell.rowSpan;

                for (var r = rowIndex; r < rowIndex + rowSpan; r++) {
                    for (var i = startIdx; i < endIdx; i++)
                        tableModel[r][i] = newCell;
                }
            }
            else {
                var newTableModel = [ ];
                var newRowHash = {};

                for (var i = 0; i < tableModel.length; i++) {

                    var newRow = tableModel[i].slice(0, columnIndex);
                    if (tableModel[i].length <= columnIndex) {
                        newTableModel.push(newRow);
                        continue;
                    }

                    // insert new cell on right
                    if (tableModel[i][columnIndex] == cell) {
                        newRow.push(cell);
                        newRow.push(newCell);
                    }
                    else {
                        newRow.push(tableModel[i][columnIndex]);
                        newRow.push(tableModel[i][columnIndex]);
                    }
                    // insert old cell on right
                    for (var j = columnIndex + 1; j < tableModel[i].length; j++)
                        newRow.push(tableModel[i][j]);
                    newTableModel.push(newRow);
                }
                tableModel = newTableModel;
            }
            // Calc new width
            var cellWidthInfo = HtmlEditorTableHelper.ParseSizeString(tableModel[rowIndex][columnIndex].style.width);
            if (cellWidthInfo.valueType) {
                var newCellWidth = cellWidthInfo.value / 2;
                tableModel[rowIndex][columnIndex].style.width = newCellWidth + cellWidthInfo.valueType;
                newCell.style.width = newCellWidth + cellWidthInfo.valueType;
            }
            this.GenerateTableFromModel(tableModel, table);

            if (wrapper)
                this.SelectCellCore(tableModel[rowIndex][columnIndex], wrapper);
        },
        SplitCellVertical: function(cell, wrapper) {
            if (!cell)
                return false;

            var table = ASPx.GetParentByTagName(cell, "TABLE");
            var row = cell.parentNode;
            var rowIndex = row.rowIndex;
            var tableModel = HtmlEditorTableHelper.CreateTableModel(table);
            var columnIndex = this.GetColumnIndexByTableModel(tableModel, rowIndex, cell);
            var rowSpan = this.GetRowSpan(cell);

            var doc = ASPx.GetElementDocument(table);
        
            if (rowSpan > 1) {
                var newRowSpan = Math.ceil(rowSpan / 2);
                var newCell = doc.createElement(cell.nodeName);
                newCell.style.cssText = cell.style.cssText;

                this.AddDefaultContentToCell(newCell);

                var startRowIndex = rowIndex + newRowSpan;
                var endRowIndex = rowIndex + rowSpan;
                var curColumnIndex = columnIndex;

                while (tableModel[rowIndex][curColumnIndex] == cell) {
                    for (var r = startRowIndex; r < endRowIndex && r < tableModel.length; r++) {
                        if (tableModel[r][curColumnIndex] == cell)
                            tableModel[r][curColumnIndex] = newCell;
                    }
                    curColumnIndex++;
                }
            }
            else {
                var newRowHash = {};
                var newRowIndex = rowIndex + 1;
                var isThead = !!ASPx.GetParentByTagName(cell, "THEAD");

                var newRow = null;
                if (isThead)
                    newRow = table.tHead.insertRow(newRowIndex);
                else
                    newRow = table.insertRow(newRowIndex);

                ASPx.Attr.CopyAllAttributes(row, newRow);
                ASPx.Attr.RemoveAttribute(newRow, "id");

                ASPx.Data.ArrayInsert(tableModel, [ ], newRowIndex);
                for (var i = 0; i < tableModel[rowIndex].length; i++) {
                    if (tableModel[rowIndex][i] == cell) {
                        if (!newRowHash[cell]) {
                            var newCell = doc.createElement(cell.nodeName);
                            newCell.style.cssText = cell.style.cssText;
                            newRowHash[cell] = newCell;
                            this.AddDefaultContentToCell(newRowHash[cell]);
                        }
                        tableModel[newRowIndex][i] = newRowHash[cell];
                    }
                    else
                        tableModel[newRowIndex][i] = tableModel[rowIndex][i];
                }
            }
            this.GenerateTableFromModel(tableModel, table);
            if (wrapper)
                this.SelectCellCore(tableModel[rowIndex][columnIndex], wrapper);
        },
        MergeCellHorizontal: function(cell, wrapper) {
            var table = ASPx.GetParentByTagName(cell, "TABLE");
            var row = cell.parentNode;
            var rowIndex = row.rowIndex;
            var tableModel = HtmlEditorTableHelper.CreateTableModel(table);

            var columnIndex = this.GetColumnIndexByTableModel(tableModel, rowIndex, cell);
            var nextCellColumnIndex = this.GetNextCellColumnIndexByTableModel(tableModel, rowIndex, columnIndex);
            var nextCell = tableModel[rowIndex][nextCellColumnIndex];
            var rowSpan = isNaN(cell.rowSpan) ? 1 : cell.rowSpan;
            var newCell = this.MergeCell(cell, nextCell);

            // calc new width
            var cellWidthInfo = HtmlEditorTableHelper.ParseSizeString(tableModel[rowIndex][columnIndex].style.width);
            var mergedCellWidthInfo = HtmlEditorTableHelper.ParseSizeString(tableModel[rowIndex][nextCellColumnIndex].style.width);
            if (cellWidthInfo.valueType && cellWidthInfo.valueType == mergedCellWidthInfo.valueType)
                newCell.style.width = cellWidthInfo.value + mergedCellWidthInfo.value + cellWidthInfo.valueType;

            // replace cell in model
            for (var i = rowIndex; i < rowIndex + rowSpan; i++) {
                var curColumnIndex = columnIndex;
                while (tableModel[i] &&
                        (tableModel[i][curColumnIndex] == cell || tableModel[i][curColumnIndex] == nextCell)) {
                    tableModel[i][curColumnIndex] = newCell;
                    curColumnIndex++;
                }
            }

            this.GenerateTableFromModel(tableModel, table);

            if (wrapper)
                this.SelectCellCore(tableModel[rowIndex][columnIndex], wrapper);
        },

        MergeCellVertical: function(cell, wrapper) {
            var table = ASPx.GetParentByTagName(cell, "TABLE");
            var row = cell.parentNode;
            var rowIndex = row.rowIndex;
            var tableModel = HtmlEditorTableHelper.CreateTableModel(table);

            var columnIndex = this.GetColumnIndexByTableModel(tableModel, rowIndex, cell);
            var nextCellRowIndex = this.GetNextCellRowIndexByTableModel(tableModel, rowIndex, columnIndex);

            var colSpan = this.GetColSpan(cell);

            var nextCell = tableModel[nextCellRowIndex][columnIndex];
            var newCell = this.MergeCell(cell, nextCell);

            // replace cell in model
            for (var i = columnIndex; i < columnIndex + colSpan; i++) {
                var curRowIndex = rowIndex;
                while (tableModel[curRowIndex] &&
                        (tableModel[curRowIndex][i] == cell || tableModel[curRowIndex][i] == nextCell)) {
                    tableModel[curRowIndex][i] = newCell;
                    curRowIndex++;
                }
            }

            this.GenerateTableFromModel(tableModel, table);

            if (wrapper)
                this.SelectCellCore(tableModel[rowIndex][columnIndex], wrapper);
        },
        IsMergeCellHorizontalAllow: function(cell, wrapper) {
            var isAllow = false;
            var tableModel = HtmlEditorTableHelper.CreateTableModel(ASPx.GetParentByTagName(cell, "TABLE"));
            var row = cell.parentNode;

            var colIndx = this.GetColumnIndexByTableModel(tableModel, row.rowIndex, cell);
            var nextCellColIndx = colIndx + this.GetColSpan(cell);
            var nextCell = tableModel[row.rowIndex][nextCellColIndx];

            if (nextCell) {
                if (this.GetRowSpan(cell) == this.GetRowSpan(nextCell)) {
                    var realNextCellRow = nextCell.parentNode;
                    isAllow = realNextCellRow.rowIndex == row.rowIndex;
                }
            }
            return isAllow;
        },
        IsMergeCellVerticalAllow: function(cell, wrapper) {
            var isAllow = false;
            var table = ASPx.GetParentByTagName(cell, "TABLE");
            var nextCell = this.GetNextCellVertical(cell);
            if (nextCell) {
                var colSpan = isNaN(cell.colSpan) ? 1 : cell.colSpan;
                var nextCellColSpan = isNaN(nextCell.colSpan) ? 1 : nextCell.colSpan;
                isAllow = colSpan == nextCellColSpan;
            }
            return isAllow;
        },

        MergeCell: function(firstCell, secondCell) {
            var doc = ASPx.GetElementDocument(firstCell);
            var newCell = doc.createElement(firstCell.nodeName);
            newCell.innerHTML = firstCell.innerHTML;

            var emptyHtmlRegExp = new RegExp(ASPx.HtmlEditorClasses.EmptyHtmlRegExpPattern, "ig");
            if (!emptyHtmlRegExp.test(secondCell.innerHTML))
                newCell.innerHTML += "<br/>" + secondCell.innerHTML;
            ASPx.Attr.CopyAllAttributes(firstCell, newCell);
            return newCell;
        },

        /*region* * * * * * * * * * * * * * * * * *  Table model  * * * * * * * * * * * * * * * * * * */

        CreateTableModel: function(table) {
            var rows = table.rows;
            var rowCount = 0;

            var model = [ ];

            for (var i = 0; i < rows.length; i++) {
                if (!model[rowCount])
                    model[rowCount] = [ ];

                var colCount = 0;

                for (var j = 0, cl = rows[i].cells.length; j < cl; j++) {
                    var curCell = rows[i].cells[j];

                    while (model[rowCount][colCount])
                        colCount++;

                    var colSpan = this.GetColSpan(curCell);
                    var rowSpan = this.GetRowSpan(curCell);

                    for (var rs = 0; rs < rowSpan; rs++) {
                        var rowInd = rowCount + rs;

                        if (rowInd >= rows.length)
                            break;

                        if (!model[rowInd])
                            model[rowInd] = [ ];

                        for (var cs = 0; cs < colSpan; cs++)
                            model[rowInd][colCount + cs] = curCell;
                    }
                    colCount += colSpan - 1;
                }
                rowCount++;
            }
            return model;
        },
        GenerateTableFromModel: function(model, sourceTable) {
            var rowSpanAttr = ASPx.Browser.IE ? "_dxrowspan" : "rowSpan";

            for (var i = 0; i < model.length; i++) {
                for (var j = 0; j < model[i].length; j++) {
                    if (model[i][j].parentNode)
                        model[i][j].parentNode.removeChild(model[i][j]);
                    model[i][j].colSpan = model[i][j][rowSpanAttr] = 1;
                    model[i][j].rowSpan = 1;
                    model[i][j].rowIsNotSpanned = false;
                    model[i][j].colIsNotSpanned = false;
                }
            }

            // set colSpan
            var maxColumnCount = 0;
            for (var i = 0; i < model.length; i++) {
                for (var j = 0; j < model[i].length; j++) {
                    if (model[i][j]) {
                        var cell = model[i][j];
                        if (j > maxColumnCount)
                            maxColumnCount = j;
                        if (!cell.colIsNotSpanned) {
                            if (model[i][j - 1] == cell)
                                cell.colSpan++;
                            if (model[i][j + 1] != cell)
                                cell.colIsNotSpanned = true;
                        }
                    }
                }
            }

            //set rowSpan
            var sourceRows = [];
            for (var i = 0; i <= maxColumnCount; i++) {
                for (var j = 0; j < model.length; j++) {
                    if (model[j] && model[j][i] && !model[j][i].rowIsNotSpanned) {
                        var cell = model[j][i];
                        if (model[j - 1] && model[j - 1][i] == cell)
                            cell[rowSpanAttr]++;
                        if (!model[j + 1] || model[j + 1][i] != cell)
                            cell.rowIsNotSpanned = true;
                    }
                }
            }
            // generate table
            var doc = ASPx.GetElementDocument(sourceTable);
            for (var i = 0; i < model.length; i++) {
                var rowObj = doc.createElement(sourceTable.rows[i].tagName);
                ASPx.Attr.CopyAllAttributes(sourceTable.rows[i], rowObj);
                for (var j = 0; j < model[i].length; ) {
                    var cell = model[i][j];
                    ASPx.Attr.RemoveAttribute(cell, "rowIsNotSpanned");
                    ASPx.Attr.RemoveAttribute(cell, "colIsNotSpanned");

                    if (model[i - 1] && model[i - 1][j] == cell) {
                        j += cell.colSpan;
                        continue;
                    }
                    rowObj.appendChild(cell);

                    var isEmptyRowSpan = cell.rowSpan == 1;

                    if (rowSpanAttr != 'rowSpan' && ASPx.IsExists(cell[rowSpanAttr])) {
                        if (cell[rowSpanAttr] > 1)
                            isEmptyRowSpan = false;
                        
                            cell.rowSpan = cell[rowSpanAttr];
                            cell.removeAttribute(rowSpanAttr);
                        }
                    if (cell.colSpan == 1)
                        cell.removeAttribute('colspan');
                    if (isEmptyRowSpan)
                        cell.removeAttribute('rowspan');
                    j += cell.colSpan;
                }
                sourceTable.rows[i].parentNode.replaceChild(rowObj, sourceTable.rows[i]);
            }
        },

        /*region* * * * * * * * * * * * * * * * * *  Utils  * * * * * * * * * * * * * * * * * * */
        AddDefaultContentToCell: function(cell) {
            cell.innerHTML = "&nbsp;";
        },
        CreateNewCellByCell: function(cell, resetWidth) {
            var newCell = ASPx.GetElementDocument(cell).createElement(cell.nodeName);
            if (cell.style.cssText !== '') {
                newCell.style.cssText = cell.style.cssText;
                if (resetWidth && HtmlEditorTableHelper.ParseSizeString(newCell.style.width).valueType == "%")
                    newCell.style.width = "";
            }
            this.AddDefaultContentToCell(newCell);
            return newCell;
        },
        ParseSizeString: function(sizeString) {
            var ret = { value: null, valueType: null };
            var res = sizeTypeRegExp.exec(sizeString);
            if (res && res.length > 2) {
                ret.value = parseFloat(res[1]);
                ret.valueType = res[2];
            }
            return ret;
        },
        SetBackgroundColor: function(element, color) {
            if (color)
                element.style.backgroundColor = color;
            else {
                element.style.backgroundColor = "";
                ASPx.Attr.RemoveStyleAttribute(element.style, "backgroundColor");
            }
        },
        SetTableBorderColor: function(table, color) {
            ASPx.Attr.RemoveAttribute(table, ASPx.Browser.IE ? "borderColor" : "bordercolor");
            for(var i = 0, row; row = table.rows[i]; i++) {
                for(var j = 0, cell; cell = row.cells[j]; j++) {
                    cell.style.borderColor = color;
                }
            }
        },
        SetTableBorderWidthCore: function(table, width) {
            ASPx.Attr.RemoveAttribute(table, "border");
            var className = "";
            if(HtmlEditorTableHelper.HasEmptyBorderClassName(table)) {
                className = table.className;
                HtmlEditorTableHelper.RemoveEmptyBorderClassName(table);
                table.offsetWidth; // Hack: force updating current styles
            }
            var parentElement = HtmlEditorTableHelper.GetParentWithEmptyBorderClassName(table);
            for(var i = 0, row; row = table.rows[i]; i++) {
                for(var j = 0, cell; cell = row.cells[j]; j++) {
                    if(width > 0) {
                        cell.style.borderWidth = width + "px";
                        var currentStyle = ASPx.GetCurrentStyle(cell);
                        var bc = HtmlEditorTableHelper.GetBorderColor(currentStyle);
                        var bs = HtmlEditorTableHelper.GetBorderStyle(currentStyle);
                        if(!bc || bc == "transparent")
                            cell.style.borderColor = "#000000";
                        if(!bs || bs == "none" || parentElement)
                            cell.style.borderStyle = "solid";
                    }
                    else {
                        cell.style.borderWidth = "";
                        cell.style.borderColor = "";
                        cell.style.borderStyle = "";
                    }
                }
            }
            if(className)
                table.className = className;
        },
        GetTableBorderColor: function(table, color) {
            var color = null;
            for(var i = 0, row; row = table.rows[i]; i++) {
                for(var j = 0, cell; cell = row.cells[j]; j++) {
                    if(color === null)
                        color = cell.style.borderColor;
                    if(color !== cell.style.borderColor)
                        return null;
                }
            }
            return color;
        },
        GetRowIndex: function(cell) {
            var row = cell.parentNode;
            return row.rowIndex;
        },
        GetCell: function(table, rowIndex, colIndex) {
            return table.rows[rowIndex].cells[colIndex];
        },
        GetNextCellVertical: function(cell) {
            var nextCell = null;
            var row = cell.parentNode;
            var table = ASPx.GetParentByTagName(cell, "TABLE");

            var rowSpan = this.GetRowSpan(cell);

            if (table.rows.length > row.rowIndex + rowSpan) {
                var nextRow = table.rows[row.rowIndex + rowSpan];

                var prevSumSpan = 0;
                for(var i = 0, prevInd = cell.cellIndex; i <= prevInd; i++)
                    prevSumSpan +=  this.GetColSpan(row.cells[i]);

                var nextSumSpan = 0;
                for(var i = 0, rowLen = nextRow.cells.length; i < rowLen; i++) {
                    var nextSpan = this.GetColSpan(nextRow.cells[i]);
                    nextSumSpan += nextSpan;
                
                    if((this.GetColSpan(cell) == nextSpan) && (nextSumSpan == prevSumSpan)) {
                        nextCell = nextRow.cells[i];
                        break;
                    }
                }
            }
            return nextCell;
        },
        GetColumnIndexByTableModel: function(model, rowIndex, cell) {
            if (model.length < rowIndex + 1)
                return -1;

            var row = model[rowIndex];

            for (var i = 0; i < row.length; i++) {
                if (row[i] == cell)
                    return i;
            }
            return -1;
        },
        GetNextCellColumnIndexByTableModel: function(model, rowIndex, columnIndex) {
            if (model.length < rowIndex + 1)
                return -1;
            var cell = model[rowIndex][columnIndex];
            for (var i = columnIndex; i < model[rowIndex].length; i++) {
                if (model[rowIndex][i] != cell)
                    return i;
            }
            return -1;
        },
        GetNextCellRowIndexByTableModel: function(model, rowIndex, columnIndex) {
            if (model.length < rowIndex + 1)
                return -1;
            var cell = model[rowIndex][columnIndex];
            for (var i = rowIndex; i < model.length; i++) {
                if (model[i][columnIndex] != cell)
                    return i;
            }
            return -1;
        },

        GetNextRow: function(row) {
            var table = ASPx.GetParentByTagName(row, "TABLE");
            var i = 0;
            for (i = 0; i < table.rows.length; i++)
                if (table.rows[i] == row)
                break;
            return i + 1 < table.rows.length ? table.rows[i + 1] : null;
        },
        GetColSpan: function(cell) {
            return isNaN(cell.colSpan) ? 1 : cell.colSpan;
        },
        GetRowSpan: function(cell) {
            return isNaN(cell.rowSpan) ? 1 : cell.rowSpan;
        },

        SelectCell: function(table, rowIndex, colIndex, wrapper) {
            var cell = this.GetCell(table, rowIndex, colIndex);
            if (cell)
                this.SelectCellCore(cell, wrapper);
        },
        SelectCellCore: function(cell, wrapper) {
            var elem = cell;
            if (cell.childNodes.length > 0 && !(ASPx.Browser.IE && ASPx.Browser.Version < 11))
                elem = cell.childNodes[0];
            if (ASPx.Browser.IE && ASPx.Browser.MajorVersion > 10)
                wrapper.removeFocus();
            if (!ASPx.Browser.IE)
                wrapper.focus();
            ASPxClientHtmlEditorSelection.SelectElement(elem, wrapper.getWindow(), ASPx.Browser.Opera);
        },

        DeleteRows: function(table) {
            for (var i = 0; i < table.rows.length; i++) {
                var rIndex = table.rows[i].sectionRowIndex;
                table.deleteRow(rIndex);
            }
        }
    };

    ASPx.HtmlEditorClasses.Commands.Tables = {};

    ASPx.HtmlEditorClasses.Commands.Tables.Table = {
        IsSelected: function(wrapper) {
            var curSelection = ASPxClientHtmlEditorSelection.Create(wrapper.getWindow());
            return !!HtmlEditorTableHelper.GetTable(curSelection.GetParentElement());
        },
        SetRowAndColumnCount: function(table, rowCount, columnCount, isColumnEqualWidth) {
            HtmlEditorTableHelper.DeleteRows(table);
            var cellWidth = 100 / columnCount + "%";
            for (var i = 0; i < rowCount; i++) {
                var newRow = table.insertRow(i);
                for (var j = 0; j < columnCount; j++) {
                    var cell = newRow.insertCell(j);
                    if (isColumnEqualWidth)
                        cell.style.width = cellWidth;
                }
            }
        },
        // cmdValue = { rows, columns, isColumnEqualWidth, tableProperties };
        //  TableProperties = { 
        //      borderWidth, borderColor, 
        //      backgroundColor,
        //      width, height,
        //      cellPadding, cellSpacing, align, float
        //  }
        Insert: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
            Execute: function(cmdValue, wrapper) {
                if(!cmdValue)
                    return false;
                else if(cmdValue.rowCount && cmdValue.cellCount && !cmdValue.tableProperties) {
                    cmdValue.tableProperties = this.getDefaultProperties();
                    cmdValue.rows = cmdValue.rowCount;
                    cmdValue.columns = cmdValue.cellCount;
                }
                ASPx.HtmlEditorClasses.Commands.Command.prototype.Execute.apply(this, arguments);
                var doc = wrapper.getDocument();

                var tableId = ASPx.HtmlEditorClasses.Selection.CreateUniqueID();
                var emptyTableHtml = "<table id='" + tableId + "'></table>";

                this.owner.getCommand(ASPxClientCommandConsts.PASTEHTML_COMMAND).Execute(emptyTableHtml, wrapper);

                var emptyTable = ASPx.GetElementByIdInDocument(doc, tableId);
                emptyTable.id = "";
                ASPx.Attr.RemoveAttribute(emptyTable, "id");

                ASPx.HtmlEditorClasses.Commands.Tables.Table.SetRowAndColumnCount(emptyTable, cmdValue.rows, cmdValue.columns, cmdValue.isColumnEqualWidth);
                this.InitializeTable(emptyTable);
                HtmlEditorTableHelper.SetTableProperties(emptyTable, cmdValue.tableProperties, wrapper, this.owner);
                HtmlEditorTableHelper.SelectCell(emptyTable, 0, 0, wrapper);
                return true;
            },
            InitializeTable: function(table) {
                for (var i = 0; i < table.rows.length; i++) {
                    var row = table.rows[i];
                    for (var j = 0; j < row.cells.length; j++)
                        HtmlEditorTableHelper.AddDefaultContentToCell(row.cells[j]);
                }
            },
            getDefaultProperties: function() {
                return {
                    align: "none",
                    backgroundColor: null,
                    borderColor: "#000000",
                    borderWidth: 1,
                    cellPadding: 3,
                    cellSpacing: 0,
                    height: null,
                    heightType: null,
                    heightValue: 0,
                    heightValueType: "px",
                    width: "100%",
                    widthType: "100%",
                    widthValue: 0,
                    widthValueType: "px"
                }
            }
        }),
        // cmdValue = { tableElement, tableProperties }
        Change: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
            Execute: function(cmdValue, wrapper) {
                ASPx.HtmlEditorClasses.Commands.Command.prototype.Execute.apply(this, arguments);
                HtmlEditorTableHelper.SetTableProperties(cmdValue.tableElement, cmdValue.tableProperties, wrapper, this.owner);
                HtmlEditorTableHelper.SelectCell(cmdValue.tableElement, 0, 0, wrapper);
                return true;
            }
        }),
        Delete: ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
            Execute: function(cmdValue, wrapper) {
                ASPx.HtmlEditorClasses.Commands.Command.prototype.Execute.apply(this, arguments);
                var curSelection = ASPxClientHtmlEditorSelection.Create(wrapper.getWindow());
                var table = HtmlEditorTableHelper.GetTable(curSelection.GetParentElement());
                if (table) {
                    ASPxClientHtmlEditorSelection.SelectElement(table, wrapper.getWindow());
                    this.owner.getCommand(ASPxClientCommandConsts.DELETE_COMMAND).Execute(null, wrapper);
                    if (ASPx.Browser.Opera)
                        ASPx.RemoveElement(table);
                    wrapper.focus();
                    return true;
                }
                return false;
            },
            IsLocked: function(wrapper) {
                return !ASPx.HtmlEditorClasses.Commands.Tables.Table.IsSelected(wrapper);
            }
        })
    };

    ASPx.HtmlEditorClasses.Commands.Tables.InsertColumnAndRowBase = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        IsLocked: function(wrapper) {
            return !ASPx.HtmlEditorClasses.Commands.Tables.Cell.Get(wrapper);
        }
    });
    ASPx.HtmlEditorClasses.Commands.Tables.DeleteColumnAndRowBase = ASPx.CreateClass(ASPx.HtmlEditorClasses.Commands.Command, {
        IsLocked: function(wrapper) {
            if (wrapper.needGetElementFromSelection("tableRow")) {
                var curSelection = ASPxClientHtmlEditorSelection.Create(wrapper.getWindow());
                wrapper.setSelectedElement("tableRow", HtmlEditorTableHelper.GetTableRow(curSelection.GetParentElement()));
            }
            return !wrapper.getSelectedElement("tableRow");
        }
    });

ASPx.HtmlEditorTableHelper = HtmlEditorTableHelper;
ASPx.HtmlEditorTableHelper.EmptyBorderTableClassName = emptyBorderTableClassName;
})();