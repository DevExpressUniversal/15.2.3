module __aspxRichEdit {
    export class JSONPropertyNames {
        static CHARACTER_STYLES = "characterStyles";
        static PARAGRAPH_STYLES = "paragraphStyles";
        static NUMBERING_LIST_STYLES = "numberingListStyles";
        static TABLE_STYLES = "tableStyles";
        static TABLE_CELL_STYLES = "tableCellStyles";
        static DEFAULT_TAB_WIDTH = "defaultTabWidth";
        static DIFFERENT_ODD_AND_EVEN_PAGES = "differentOddAndEvenPages";
        static DISPLAY_BACKGROUND_SHAPE = "displayBackgroundShape";
        static PAGE_BACK_COLOR = "pageBackColor";
        static DEFAULT_CHARACTER_PROPERTIES = "defaultCharacterProperties";
        static DEFAULT_PARAGRAPH_PROPERTIES = "defaultParagraphProperties";
        static DEFAULT_TABLE_PROPERTIES = "defaultTableProperties";
        static DEFAULT_TABLE_ROW_PROPERTIES = "defaultTableRowProperties";
        static DEFAULT_TABLE_CELL_PROPERTIES = "defaultTableCellProperties";

        static CACHE = "cache";
        static SUBDOCUMENT_ID = "subDocumentId";
        static TYPE = "type";

        static CHARACTER_PROPERTIES_CACHE = "characterPropertiesCache";
        static PARAGRAPH_PROPERTIES_CACHE = "paragraphPropertiesCache";
        static LIST_LEVEL_PROPERTIES_CACHE = "listLevelPropertiesCache";

        static SECTION_PROPERTIES_CACHE = "sectionPropertiesCache";
        static SECTION_PROPERTIES_CACHE_INDEX = "sectionPropertiesCacheIndex";
        static ABSTRACT_NUMBERING_LISTS = "abstractNumberingLists";

        static NUMBERING_LISTS = "numberingLists";
        static ABSTRACT_NUMBERING_LIST_TEMPLATES = "abstractNumberingListTemplates";

        static FIELDS = "fields";
        static TABLES = "tables";
        static BOOKMARKS = "bookmarks";
    }

    export class JSONImporter {
        static importStyles(documentModel: DocumentModel, content: any) {
            JSONImporter.importCharacterStyles(documentModel, content[JSONPropertyNames.CHARACTER_STYLES]); 
            JSONImporter.importParagraphStyles(documentModel, content[JSONPropertyNames.PARAGRAPH_STYLES]);
            JSONImporter.importNumberingStyles(documentModel, content[JSONPropertyNames.NUMBERING_LIST_STYLES]);
            JSONImporter.importTableStyles(documentModel, content[JSONPropertyNames.TABLE_STYLES]);
            JSONImporter.importTableCellStyles(documentModel, content[JSONPropertyNames.TABLE_CELL_STYLES]);

            JSONImporter.finishCharacterStylesImport(documentModel, content[JSONPropertyNames.CHARACTER_STYLES]);
            JSONImporter.finishParagraphStylesImport(documentModel, content[JSONPropertyNames.PARAGRAPH_STYLES]);
            JSONImporter.finishNumberingListStylesImport(documentModel, content[JSONPropertyNames.NUMBERING_LIST_STYLES]);
            JSONImporter.finishTableStylesImport(documentModel, content[JSONPropertyNames.TABLE_STYLES]);
            JSONImporter.finishTableCellStylesImport(documentModel, content[JSONPropertyNames.TABLE_CELL_STYLES]);
        }

        static importDocumentProperties(documentModel: DocumentModel, content: any) {
            documentModel.defaultTabWidth = content[JSONPropertyNames.DEFAULT_TAB_WIDTH];
            documentModel.differentOddAndEvenPages = content[JSONPropertyNames.DIFFERENT_ODD_AND_EVEN_PAGES];
            documentModel.displayBackgroundShape = content[JSONPropertyNames.DISPLAY_BACKGROUND_SHAPE];
            documentModel.pageBackColor = content[JSONPropertyNames.PAGE_BACK_COLOR];
            documentModel.setDefaultCharacterProperties(content[JSONPropertyNames.DEFAULT_CHARACTER_PROPERTIES]);
            documentModel.setDefaultParagraphProperties(content[JSONPropertyNames.DEFAULT_PARAGRAPH_PROPERTIES]);

            documentModel.defaultTableProperties = JSONTablePropertiesConverter.convertFromJSON(content[JSONPropertyNames.DEFAULT_TABLE_PROPERTIES]);
            documentModel.defaultTableRowProperties = documentModel.cache.tableRowPropertiesCache.addItemIfNonExists(JSONTableRowPropertiesConverter.convertFromJSON(content[JSONPropertyNames.DEFAULT_TABLE_ROW_PROPERTIES]));
            documentModel.defaultTableCellProperties = documentModel.cache.tableCellPropertiesCache.addItemIfNonExists(JSONTableCellPropertiesConverter.convertFromJSON(content[JSONPropertyNames.DEFAULT_TABLE_CELL_PROPERTIES]));
        }

        static importHeadersFooters(documentModel: DocumentModel, contentHeaders: any, contentFooters: any) {
            for (let i = 0, contentHeader: any; contentHeader = contentHeaders[JSONPropertyNames.CACHE][i]; i++) {
                let header = new HeaderSubDocumentInfo(contentHeader[JSONPropertyNames.SUBDOCUMENT_ID]);
                header.headerFooterType = contentHeader[JSONPropertyNames.TYPE];
                documentModel.headers.push(header);
            }

            for (let i = 0, contentFooter: any; contentFooter = contentFooters[JSONPropertyNames.CACHE][i]; i++) {
                let footer = new FooterSubDocumentInfo(contentFooter[JSONPropertyNames.SUBDOCUMENT_ID]);
                footer.headerFooterType = contentFooter[JSONPropertyNames.TYPE];
                documentModel.footers.push(footer);
            }
        }

        static importSections(documentModel: DocumentModel, content: any) {
            var tempCache: IndexedCache<SectionProperties> = new IndexedCache<SectionProperties>();
            tempCache.merge(content[JSONPropertyNames.SECTION_PROPERTIES_CACHE], JSONSectionPropertiesConverter.convertFromJSON);
            for(var i = 0, obj: any; obj = content.sections[i]; i++)
                documentModel.sections.push(new Section(documentModel, documentModel.mainSubDocument.positionManager.registerPosition(obj["start"]), obj["length"], tempCache.getItem(obj[JSONPropertyNames.SECTION_PROPERTIES_CACHE_INDEX]).clone()));
        }

        static importNumberingLists(documentModel: DocumentModel, content: any) {
            if(content[JSONPropertyNames.CHARACTER_PROPERTIES_CACHE])
                documentModel.cache.maskedCharacterPropertiesCache.merge(content[JSONPropertyNames.CHARACTER_PROPERTIES_CACHE], JSONMaskedCharacterPropertiesConverter.convertFromJSON);
            if(content[JSONPropertyNames.PARAGRAPH_PROPERTIES_CACHE])
                documentModel.cache.maskedParagraphPropertiesCache.merge(content[JSONPropertyNames.PARAGRAPH_PROPERTIES_CACHE], JSONMaskedParagraphPropertiesConverter.convertFromJSON);
            if(content[JSONPropertyNames.LIST_LEVEL_PROPERTIES_CACHE])
                documentModel.cache.listLevelPropertiesCache.merge(content[JSONPropertyNames.LIST_LEVEL_PROPERTIES_CACHE], JSONListLevelPropertiesConverter.convertFromJSON);

            for (var i = 0, obj: any; obj = content[JSONPropertyNames.ABSTRACT_NUMBERING_LISTS][i]; i++) {
                var abstractNumberingList = new AbstractNumberingList(documentModel);
                abstractNumberingList.deleted = obj["deleted"];
                abstractNumberingList.innerId = obj["id"];
                abstractNumberingList.levels = JSONImporter.getAbstractNumberingListLevels(documentModel, obj["levels"]);
                documentModel.abstractNumberingLists.push(abstractNumberingList);
            }

            for (var i = 0, obj: any; obj = content[JSONPropertyNames.NUMBERING_LISTS][i]; i++) {
                var numberingList = new NumberingList(documentModel, obj["alIndex"]);
                numberingList.deleted = obj["deleted"];
                numberingList.innerId = obj["id"];
                numberingList.levels = JSONImporter.getNumberingListLevels(documentModel, obj["levels"], numberingList);
                documentModel.numberingLists.push(numberingList);
            }

            if (content[JSONPropertyNames.ABSTRACT_NUMBERING_LIST_TEMPLATES]) {
                for (var i = 0, obj: any; obj = content[JSONPropertyNames.ABSTRACT_NUMBERING_LIST_TEMPLATES][i]; i++) {
                    var abstractNumberingListTemplate = new AbstractNumberingList(documentModel);
                    abstractNumberingListTemplate.deleted = obj["deleted"];
                    abstractNumberingListTemplate.innerId = obj["id"];
                    abstractNumberingListTemplate.levels = JSONImporter.getAbstractNumberingListLevels(documentModel, obj["levels"]);
                    documentModel.abstractNumberingListTemplates.push(abstractNumberingListTemplate);
                }
            }
        }

        static importSubDocumentProperties(subDocument: SubDocument, content: any) {
            JSONImporter.importFields(subDocument, content[JSONPropertyNames.FIELDS]);
            JSONImporter.importTables(subDocument, content[JSONPropertyNames.TABLES]);
            JSONImporter.importBookmarks(subDocument, content[JSONPropertyNames.BOOKMARKS]);
        }

        static importFields(subDocument: SubDocument, content: any) {
            if(!content)
                return;
            var subDocumentIdToFieldList: any = content[JSONPropertyNames.FIELDS];
            var currSubDocFieldList: Field[] = subDocument.fields;
            for (var fieldIndex = 0, requestField: any; requestField = subDocumentIdToFieldList[fieldIndex]; fieldIndex++) {
                var hyperlinkInfo: HyperlinkInfo;
                if(requestField.uri !== undefined || requestField.anchor !== undefined)
                    hyperlinkInfo = new HyperlinkInfo(requestField.uri, requestField.anchor, requestField.tip, requestField.visited);
                else
                    hyperlinkInfo = undefined;
                subDocument.fields.push(new Field(subDocument.positionManager, fieldIndex, requestField.start, requestField.separator, requestField.end, false, hyperlinkInfo));
            }

            // in server fields sorted by end position
            subDocument.fields.sort((a: Field, b: Field) => a.getCodeStartPosition() - b.getCodeStartPosition());

            for(var fieldIndex = 0, field: Field; field = subDocument.fields[fieldIndex]; fieldIndex++) {
                field.index = fieldIndex;
                field.initParent(subDocument.fields);
            }
        }

        static importBookmarks(subDocument: SubDocument, content: any) {
            if (!content)
                return;
            Utils.foreach(<any[]>content[JSONPropertyNames.BOOKMARKS], (obj) => {
                var bookmark: Bookmark = new Bookmark();
                bookmark.name = obj["name"];
                var start = parseInt(obj["start"]);
                var end = start + parseInt(obj["length"]);
                //TODO isHidden
                bookmark.start = subDocument.positionManager.registerPosition(start);
                bookmark.end = subDocument.positionManager.registerPosition(end);
                subDocument.bookmarks.push(bookmark);
            });
        }

        static importTables(subDocument: SubDocument, rawTablesInfo: any) {
            if (!rawTablesInfo)
                return;
            let tempTablePropertiesCache = new TablePropertiesCache();
            tempTablePropertiesCache.merge(rawTablesInfo["tablePropertiesCache"], JSONTablePropertiesConverter.convertFromJSON);
            let tempTableRowPropertiesCache = new TableRowPropertiesCache();
            tempTableRowPropertiesCache.merge(rawTablesInfo["tableRowPropertiesCache"], JSONTableRowPropertiesConverter.convertFromJSON);
            let tempTableCellPropertiesCache = new TableCellPropertiesCache();
            tempTableCellPropertiesCache.merge(rawTablesInfo["tableCellPropertiesCache"], JSONTableCellPropertiesConverter.convertFromJSON);

            let tablesMap: { [index: number]: Table } = {};
            const subDocumentTables: Table[] = subDocument.tables;
            const tablesOnSubDocument: any = rawTablesInfo[JSONPropertyNames.TABLES];
            for(let rawTableIndex: number = 0, rawTable: any; rawTable = tablesOnSubDocument[rawTableIndex]; rawTableIndex++) {
                let tableProperties = tempTablePropertiesCache.getItem(rawTable["tablePropertiesIndex"]).clone();
                let tableStyle = subDocument.documentModel.tableStyles[rawTable["styleIndex"]];
                const newTable: Table = new Table(tableProperties, tableStyle);
                subDocumentTables.push(newTable);

                newTable.index = rawTable["index"];
                tablesMap[newTable.index] = newTable;

                newTable.nestedLevel = rawTable["nestedLevel"];
                newTable.preferredWidth = JSONTableWidthUnitConverter.convertFromJSON(rawTable["preferredWidth"]);
                newTable.lookTypes = rawTable["lookTypes"];
                
                const parentCellContent: any = rawTable["parentCell"];
                newTable.parentCell = parentCellContent ? JSONImporter.getParentCell(parentCellContent, tablesMap) : null;

                for(let rowIndex: number = 0, rawRow: any; rawRow = rawTable["rows"][rowIndex]; rowIndex++) {
                    let tableRowProperties = subDocument.documentModel.cache.tableRowPropertiesCache.addItemIfNonExists(tempTableRowPropertiesCache.getItem(rawRow["tableRowPropertiesIndex"]));
                    const newTableRow: TableRow = new TableRow(newTable, tableRowProperties);
                    newTable.rows.push(newTableRow);
                    newTableRow.gridBefore = rawRow["gridBefore"];
                    newTableRow.gridAfter = rawRow["gridAfter"];
                    newTableRow.widthBefore = JSONTableWidthUnitConverter.convertFromJSON(rawRow["widthBefore"]);
                    newTableRow.widthAfter = JSONTableWidthUnitConverter.convertFromJSON(rawRow["widthAfter"]);
                    newTableRow.height = JSONTableHeightUnitConverter.convertFromJSON(rawRow["height"]);

                    newTableRow.tablePropertiesException = tempTablePropertiesCache.getItem(rawRow["tablePropertiesExceptionIndex"]).clone();

                    for (let cellIndex = 0, rawCell: any; rawCell = rawRow["cells"][cellIndex]; cellIndex++) {
                        const newTableCell: TableCell = JSONImporter.importTableCell(rawCell, subDocument.documentModel, tempTableCellPropertiesCache, subDocument, newTableRow);
                        newTableCell.parentRow = newTableRow;
                        newTableRow.cells.push(newTableCell);
                    }
                }
                TableConditionalFormattingCalculator.updateTableWithoutHistory(subDocument.documentModel, newTable);
            }
            Table.sort(subDocumentTables);

            // fill tablesByLevels
            const tableByLevels: Table[][] = subDocument.tablesByLevels;
            let tableByLevelsLength: number = tableByLevels.length;
            for (let tableIndex = 0, table: Table; table = subDocumentTables[tableIndex]; tableIndex++) {
                table.index = tableIndex;
                if(table.nestedLevel >= tableByLevelsLength) {
                    tableByLevels.push([]);
                    tableByLevelsLength++;
                }
                tableByLevels[table.nestedLevel].push(table);
            }
        }

        private static getParentCell(content: any, tablesMap: { [index: number]: Table }): TableCell {
            let tableIndex = content["tableIndex"];
            let table = tablesMap[tableIndex];
            return table.rows[content["rowIndex"]].cells[content["cellIndex"]];
        }

        private static importTableCell(content: any, documentModel: DocumentModel, tempTableCellPropertiesCache: TableCellPropertiesCache, subDocument: SubDocument, parentRow: TableRow): TableCell {
            let tableCellProperties = documentModel.cache.tableCellPropertiesCache.addItemIfNonExists(tempTableCellPropertiesCache.getItem(content["tableCellPropertiesIndex"]));
            var newTableCell: TableCell = new TableCell(parentRow, tableCellProperties);
            newTableCell.style = null;
            newTableCell.columnSpan = content["columnSpan"];
            newTableCell.preferredWidth = JSONTableWidthUnitConverter.convertFromJSON(content["preferredWidth"]);
            newTableCell.verticalMerging = content["verticalMerging"];
            newTableCell.startParagraphPosition = subDocument.positionManager.registerPosition(content["startParagraphPosition"]);
            newTableCell.endParagrapPosition = subDocument.positionManager.registerPosition(content["endParagraphPosition"]);
            return newTableCell;
        }

        static importFixedLengthText(subDocument: SubDocument, content: any, customRunAction?: { (run: TextRun): void }) {
            if(content["mergedCharacterPropertiesCache"])
                subDocument.documentModel.cache.mergedCharacterPropertiesCache.merge(content["mergedCharacterPropertiesCache"], JSONCharacterPropertiesConverter.convertFromJSON);
            if(content[JSONPropertyNames.CHARACTER_PROPERTIES_CACHE])
                subDocument.documentModel.cache.maskedCharacterPropertiesCache.merge(content[JSONPropertyNames.CHARACTER_PROPERTIES_CACHE], JSONMaskedCharacterPropertiesConverter.convertFromJSON);
            if(content["mergedParagraphPropertiesCache"])
                subDocument.documentModel.cache.mergedParagraphPropertiesCache.merge(content["mergedParagraphPropertiesCache"], JSONParagraphPropertiesConverter.convertFromJSON);
            if(content[JSONPropertyNames.PARAGRAPH_PROPERTIES_CACHE])
                subDocument.documentModel.cache.maskedParagraphPropertiesCache.merge(content[JSONPropertyNames.PARAGRAPH_PROPERTIES_CACHE], JSONMaskedParagraphPropertiesConverter.convertFromJSON);
            if(content["paragraphs"])
                JSONImporter.mergeParagraphs(subDocument.documentModel, subDocument, content["paragraphs"]);
            if(content["chunks"])
                JSONImporter.mergeChunks(subDocument.documentModel, subDocument, content["chunks"], customRunAction);
        }

        static importOptions(controlOptions: ControlOptions, content: any) {
            if(content["copy"] !== undefined)
                controlOptions.copy = content["copy"];
            if(content["createNew"] !== undefined)
                controlOptions.createNew = content["createNew"];
            if(content["cut"] !== undefined)
                controlOptions.cut = content["cut"];
            if(content["drag"] !== undefined)
                controlOptions.drag = content["drag"];
            if(content["drop"] !== undefined)
                controlOptions.drop = content["drop"];
            if(content["open"] !== undefined)
                controlOptions.open = content["open"];
            if(content["paste"] !== undefined)
                controlOptions.paste = content["paste"];
            if(content["printing"] !== undefined)
                controlOptions.printing = content["printing"];
            if(content["save"] !== undefined)
                controlOptions.save = content["save"];
            if(content["saveAs"] !== undefined)
                controlOptions.saveAs = content["saveAs"];
            if(content["tabMarker"] !== undefined)
                controlOptions.tabMarker = content["tabMarker"];
            if(content["pageBreakInsertMode"] !== undefined)
                controlOptions.pageBreakInsertMode = content["pageBreakInsertMode"];
            if(content["fullScreen"] !== undefined)
                controlOptions.fullScreen = content["fullScreen"];

            if(content["characterFormatting"] !== undefined)
                controlOptions.characterFormatting = content["characterFormatting"];
            if(content["characterStyle"] !== undefined)
                controlOptions.characterStyle = content["characterStyle"];
            if (content[JSONPropertyNames.FIELDS] !== undefined)
                controlOptions.fields = content[JSONPropertyNames.FIELDS];
            if(content["hyperlinks"] !== undefined)
                controlOptions.hyperlinks = content["hyperlinks"];
            if(content["inlinePictures"] !== undefined)
                controlOptions.inlinePictures = content["inlinePictures"];
            if(content["paragraphFormatting"] !== undefined)
                controlOptions.paragraphFormatting = content["paragraphFormatting"];
            if(content["paragraphs"] !== undefined)
                controlOptions.paragraphs = content["paragraphs"];
            if(content["paragraphStyle"] !== undefined)
                controlOptions.paragraphStyle = content["paragraphStyle"];
            if(content["paragraphTabs"] !== undefined)
                controlOptions.paragraphTabs = content["paragraphTabs"];
            if(content["sections"] !== undefined)
                controlOptions.sections = content["sections"];
            if(content["tabSymbol"] !== undefined)
                controlOptions.tabSymbol = content["tabSymbol"];
            if(content["undo"] !== undefined)
                controlOptions.undo = content["undo"];
            if(content["numberingBulleted"] !== undefined)
                controlOptions.numberingBulleted = content["numberingBulleted"];
            if(content["numberingMultiLevel"] !== undefined)
                controlOptions.numberingMultiLevel = content["numberingMultiLevel"];
            if(content["numberingSimple"] !== undefined)
                controlOptions.numberingSimple = content["numberingSimple"];
            if(content["headersFooters"] !== undefined)
                controlOptions.headersFooters = content["headersFooters"];
            if (content[JSONPropertyNames.TABLES] !== undefined)
                controlOptions.tables = content[JSONPropertyNames.TABLES];
            if(content["tableStyle"] !== undefined)
                controlOptions.tableStyle = content["tableStyle"];
        }

        static importStringResources(stringResources: StringResources, obj: any) { // TODO: move it to JSONImporter
            stringResources.evenPageFooter = obj["evenPageFooter"];
            stringResources.evenPageHeader = obj["evenPageHeader"];
            stringResources.firstPageFooter = obj["firstPageFooter"];
            stringResources.firstPageHeader = obj["firstPageHeader"];
            stringResources.footer = obj["footer"];
            stringResources.header = obj["header"];
            stringResources.oddPageFooter = obj["oddPageFooter"];
            stringResources.oddPageHeader = obj["oddPageHeader"];
            stringResources.sameAsPrevious = obj["sameAsPrevious"];
        }

        static importSubDocuments(documentModel: DocumentModel, content: any) {
            for(var id in content) {
                if(!content.hasOwnProperty(id)) continue;
                const subDocumentId = parseInt(id);
                const type = <SubDocumentInfoType>content[id][0];
                let info: SubDocumentInfoBase;
                switch(type) {
                    case SubDocumentInfoType.Header:
                        info = this.getInfoBySubDocumentId(documentModel.headers, subDocumentId);
                        break;
                    case SubDocumentInfoType.Footer:
                        info = this.getInfoBySubDocumentId(documentModel.footers, subDocumentId);
                        break;
                    default:
                        throw new Error("Unknown subDocumentType");
                }
                var subDocument = documentModel.createSubDocument(type, info);
                JSONImporter.importFixedLengthText(subDocument, content[id][1]["fixedLengthFormattedText"]);
                JSONImporter.importSubDocumentProperties(subDocument, content[id][1]);
            }
        }

        private static getInfoBySubDocumentId(infos: SubDocumentInfoBase[], subDocumentId: number): SubDocumentInfoBase {
            for(let i = 0, info: SubDocumentInfoBase; info = infos[i]; i++) {
                if(info.subDocumentId === subDocumentId)
                    return info;
            }
            throw new Error("Info not found");
        }

        static finishImportSections(documentModel: DocumentModel, content: any) {
            for(var i = 0, obj: any; obj = content["sections"][i]; i++) {
                var section = documentModel.sections[i];
                this.importHeaderFooter(section.headers, obj["headers"]);
                this.importHeaderFooter(section.footers, obj["footers"]);
            }
        }

        private static importHeaderFooter<T extends HeaderFooterSubDocumentInfoBase>(container: SectionHeadersFooters<T>, content: any) {
            for(var type in content) {
                if(!content.hasOwnProperty(type)) continue;
                container.setObjectIndex(type, content[type]);
            }
        }

        private static getAbstractNumberingListLevels(documentModel: DocumentModel, content: any): ListLevel[] {
            var result: ListLevel[] = [];
            for(var i = 0, obj: any; obj = content[i]; i++) {
                var maskedCharacterProperties = documentModel.cache.maskedCharacterPropertiesCache.getItem(obj["characterPropertiesCacheIndex"]);
                var maskedParagraphProperties = documentModel.cache.maskedParagraphPropertiesCache.getItem(obj["paragraphPropertiesCacheIndex"]);
                var listLevelProperties = documentModel.cache.listLevelPropertiesCache.getItem(obj["listLevelPropertiesCacheIndex"]);
                result.push(new ListLevel(documentModel, maskedCharacterProperties, maskedParagraphProperties, listLevelProperties));
            }
            return result;
        }

        private static getNumberingListLevels(documentModel: DocumentModel, content: any, numberingList: NumberingList): IOverrideListLevel[] {
            var result: IOverrideListLevel[] = [];
            var listLevel: IOverrideListLevel;
            for(var i = 0, obj: any; obj = content[i]; i++) {
                if(obj["level"] === undefined) {
                    var maskedCharacterProperties = documentModel.cache.maskedCharacterPropertiesCache.getItem(obj["characterPropertiesCacheIndex"]);
                    var maskedParagraphProperties = documentModel.cache.maskedParagraphPropertiesCache.getItem(obj["paragraphPropertiesCacheIndex"]);
                    var listLevelProperties = documentModel.cache.listLevelPropertiesCache.getItem(obj["listLevelPropertiesCacheIndex"]);
                    listLevel = new OverrideListLevel(documentModel, maskedCharacterProperties, maskedParagraphProperties, listLevelProperties);
                }
                else {
                    listLevel = new NumberingListReferenceLevel(numberingList, <number>obj["level"]);
                }
                listLevel.setNewStart(obj["newStart"]);
                listLevel.overrideStart = obj["overrideStart"];
                result.push(listLevel);
            }
            return result;
        }

        private static mergeParagraphs(documentModel: DocumentModel, subDocument: SubDocument, contentParagraphs: any) {
            for(var i = 0, paragraph: any; paragraph = contentParagraphs[i]; i++) {
                var paragraphIndex = Utils.binaryIndexOf(subDocument.paragraphs,(p: Paragraph) => p.startLogPosition.value - paragraph.logPosition);
                if(paragraphIndex < 0) {
                    paragraphIndex = ~paragraphIndex;
                    var modelParagraph: Paragraph = new Paragraph(subDocument, subDocument.positionManager.registerPosition(paragraph.logPosition), paragraph.length,
                        documentModel.paragraphStyles[paragraph.paragraphStyleIndex], null, paragraph.maskedParagraphPropertiesIndex);
                    modelParagraph.numberingListIndex = paragraph.listIndex;
                    modelParagraph.listLevelIndex = paragraph.listLevelIndex;
                    var tabInfos = paragraph.tabs;
                    if(tabInfos) {
                        for(var j = 0, tabInfo: any; tabInfo = tabInfos[j]; j++) {
                            var modelTabInfo: TabInfo = JSONTabConverter.convertFromJSON(tabInfo);
                            modelParagraph.tabs.tabsInfo.push(modelTabInfo);
                        }
                    }
                    modelParagraph.setParagraphMergedProperiesByIndexInCache(paragraph.mergedParagraphFormattingCacheIndex);
                    subDocument.paragraphs.splice(paragraphIndex, 0, modelParagraph);
                }
            }
        }

        private static mergeChunks(documentModel: DocumentModel, subDocument: SubDocument, contentChunks: any, customRunAction?: { (run: TextRun): void }) {
            for(var i = 0, chunk: any; chunk = contentChunks[i]; i++) {
                var modelChunk = new Chunk(subDocument.positionManager.registerPosition(chunk.start), chunk.textBuffer, !!chunk.isLast);
                var chunkIndex = Utils.binaryIndexOf(subDocument.chunks,(c: Chunk) => c.startLogPosition.value - chunk.start);
                if(chunkIndex < 0) {
                    chunkIndex = ~chunkIndex;
                    subDocument.chunks.splice(chunkIndex, 0, modelChunk);
                }
                for(var j = 0, textRun: any; textRun = chunk.runs[j]; j++) {
                    var runParagraphIndex = Utils.normedBinaryIndexOf(subDocument.paragraphs,(p: Paragraph) => p.startLogPosition.value - (chunk.start + textRun.startIndex));
                    var run = TextRun.create(textRun.startIndex, textRun.length, textRun.type, subDocument.paragraphs[runParagraphIndex],
                        subDocument.documentModel.characterStyles[textRun.characterStyleIndex],
                        subDocument.documentModel.cache.maskedCharacterPropertiesCache.getItem(textRun.maskedCharacterPropertiesCacheIndex));
                    run.setCharacterMergedProperiesByIndexInCache(textRun.mergedCharacterFormattingCacheIndex);
                    if(run.type === TextRunType.InlinePictureRun) {
                        (<InlinePictureRun>run).id = textRun.id;
                        (<InlinePictureRun>run).lockAspectRatio = textRun.lockAspectRatio;
                        (<InlinePictureRun>run).originalHeight = textRun.originalHeight;
                        (<InlinePictureRun>run).originalWidth = textRun.originalWidth;
                        (<InlinePictureRun>run).scaleX = textRun.scaleX;
                        (<InlinePictureRun>run).scaleY = textRun.scaleY;
                    }
                    if(customRunAction)
                        customRunAction(run);
                    modelChunk.textRuns.push(run);
                }
            }
        }

        private static importCharacterStyles(documentModel: DocumentModel, content) {
            documentModel.cache.maskedCharacterPropertiesCache.merge(content[JSONPropertyNames.CHARACTER_PROPERTIES_CACHE], JSONMaskedCharacterPropertiesConverter.convertFromJSON);
            for (var i = 0, obj: any; obj = content[JSONPropertyNames.CHARACTER_STYLES][i]; i++) {
                var maskedCharacterProperties = documentModel.cache.maskedCharacterPropertiesCache.getItem(obj["characterPropertiesCacheIndex"]);
                documentModel.characterStyles.push(new CharacterStyle(obj["styleName"], obj["localizedStyleName"], obj["deleted"], obj["hidden"], obj["semihidden"], obj["isDefault"], maskedCharacterProperties));
            }
        }
        private static importParagraphStyles(documentModel: DocumentModel, content) {
            documentModel.cache.maskedCharacterPropertiesCache.merge(content[JSONPropertyNames.CHARACTER_PROPERTIES_CACHE], JSONMaskedCharacterPropertiesConverter.convertFromJSON);
            documentModel.cache.maskedParagraphPropertiesCache.merge(content[JSONPropertyNames.PARAGRAPH_PROPERTIES_CACHE], JSONMaskedParagraphPropertiesConverter.convertFromJSON);
            for (var i = 0, obj: any; obj = content[JSONPropertyNames.PARAGRAPH_STYLES][i]; i++) {
                var maskedParagraphProperties = documentModel.cache.maskedParagraphPropertiesCache.getItem(obj["paragraphPropertiesCacheIndex"]);
                var maskedCharacterProperties = documentModel.cache.maskedCharacterPropertiesCache.getItem(obj["characterPropertiesCacheIndex"]);
                var tabs: TabInfo[] = [];
                for(var j = 0, tabObj: any; tabObj = obj["tabs"][j]; j++)
                    tabs.push(JSONTabConverter.convertFromJSON(tabObj));
                documentModel.paragraphStyles.push(new ParagraphStyle(obj["styleName"], obj["localizedStyleName"], obj["deleted"], obj["hidden"], obj["semihidden"], obj["isDefault"], maskedCharacterProperties, maskedParagraphProperties, tabs, obj["autoUpdate"], obj["numberingListIndex"], obj["listLevelIndex"]));
            }
        }
        private static importNumberingStyles(documentModel: DocumentModel, content) {
            for (var i = 0, obj: any; obj = content[JSONPropertyNames.NUMBERING_LIST_STYLES][i]; i++)
                documentModel.numberingListStyles.push(new NumberingListStyle(obj["styleName"], obj["localizedStyleName"], obj["deleted"], obj["hidden"], obj["semihidden"], obj["isDefault"], obj["numberingListIndex"]));
        }

        private static importTableStyles(documentModel: DocumentModel, content) {
            if(!content)
                return;
            var tempTablePropertiesCache = new TablePropertiesCache();
            var tempTableRowPropertiesCache = new TableRowPropertiesCache();
            var tempTableCellPropertiesCache = new TableCellPropertiesCache();

            documentModel.cache.maskedCharacterPropertiesCache.merge(content[JSONPropertyNames.CHARACTER_PROPERTIES_CACHE], JSONMaskedCharacterPropertiesConverter.convertFromJSON);
            documentModel.cache.maskedParagraphPropertiesCache.merge(content[JSONPropertyNames.PARAGRAPH_PROPERTIES_CACHE], JSONMaskedParagraphPropertiesConverter.convertFromJSON);
            tempTablePropertiesCache.merge(content["tablePropertiesCache"], JSONTablePropertiesConverter.convertFromJSON);
            tempTableRowPropertiesCache.merge(content["tableRowPropertiesCache"], JSONTableRowPropertiesConverter.convertFromJSON);
            tempTableCellPropertiesCache.merge(content["tableCellPropertiesCache"], JSONTableCellPropertiesConverter.convertFromJSON);

            for (var tableStylesIndex = 0, rawStyle: any; rawStyle = content[JSONPropertyNames.TABLE_STYLES][tableStylesIndex]; tableStylesIndex++)
                documentModel.tableStyles.push(JSONTableStyleConverter.convertFromJSON(rawStyle,
                    documentModel,
                    tempTablePropertiesCache,
                    tempTableRowPropertiesCache,
                    tempTableCellPropertiesCache));
        }

        private static importTableCellStyles(documentModel: DocumentModel, content) {
            if(!content)
                return;
        }
        private static finishCharacterStylesImport(documentModel: DocumentModel, content) {
            for(var i = 0, style: CharacterStyle; style = documentModel.characterStyles[i]; i++) {
                style.linkedStyle = documentModel.getParagraphStyleByName(content[JSONPropertyNames.CHARACTER_STYLES][i]["linkedStyleName"]);
                style.parent = documentModel.getCharacterStyleByName(content[JSONPropertyNames.CHARACTER_STYLES][i]["parentStyleName"]);
            }
        }
        private static finishParagraphStylesImport(documentModel: DocumentModel, content) {
            for(var i = 0, style: ParagraphStyle; style = documentModel.paragraphStyles[i]; i++) {
                style.linkedStyle = documentModel.getCharacterStyleByName(content[JSONPropertyNames.PARAGRAPH_STYLES][i]["linkedStyleName"]);
                style.parent = documentModel.getParagraphStyleByName(content[JSONPropertyNames.PARAGRAPH_STYLES][i]["parentStyleName"]);
                style.nextParagraphStyle = documentModel.getParagraphStyleByName(content[JSONPropertyNames.PARAGRAPH_STYLES][i]["nextParagraphStyleName"]);
            }
        }
        private static finishNumberingListStylesImport(documentModel: DocumentModel, content) {
            for(var i = 0, style: NumberingListStyle; style = documentModel.numberingListStyles[i]; i++) {
                style.parent = documentModel.getNumberingListStyleByName(content[JSONPropertyNames.NUMBERING_LIST_STYLES][i]["parentStyleName"]);
            }
        }
        private static finishTableStylesImport(documentModel: DocumentModel, content) {
            if(!content)
                return;
            for (var i = 0, style: TableStyle; style = documentModel.tableStyles[i]; i++)
                style.parent = documentModel.getTableStyleByName(content[JSONPropertyNames.TABLE_STYLES][i][JSONTableStyleProperty.ParentName]);
        }
        private static finishTableCellStylesImport(documentModel: DocumentModel, content) {
            if(!content)
                return;
            //for (var i = 0, style: TableCellStyle; style = documentModel.tableCellStyles[i]; i++)
            //    style.parent = documentModel.getTableCellStyleByName(content["tableCellStyles"][i]["parentStyleName"]);
        }
    }
} 