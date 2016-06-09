(function(undefined) {
    var SelectionClass = ASPxClientSpreadsheet.Selection,
        definedNames = { };

    var SpreadsheetFormulaParser = {
        parse: function(formula, baseCell, sheetName, cursorPosition) {
            var highlightedTerms = {
                    terms: [ ],
                    textPosition: undefined
                },
                termPositions = { },
                re = /([\!|\$|A-Z|\d|\:]+)/gi,
                term;

            while(term = re.exec(formula)) {
                var equalCorrection = formula[0] === "=" ? 1 : 0;
                if(term.index <= cursorPosition + equalCorrection && term.index + term[0].length >= cursorPosition) {
                    highlightedTerms.terms.push(term[0]);
                    highlightedTerms.textPosition = {
                        start: term.index,
                        length: term[0].length
                    };
                } else {
                    termPositions[term[0]] = {
                        start: term.index,
                        length: term[0].length
                    };
                }
            }

            formula = processDefinedNames(formula, baseCell);

            return parseCore(formula, baseCell, parseCells, sheetName, highlightedTerms, termPositions);
        },
        definedNames: definedNames
    };

    function getColumnIndex(columnName) {
        var result = 0;

        for(var i = 0; i < columnName.length; i++) {
            result += columnName.charCodeAt(i) - 64 + result * 25;
        }

        return result - 1;
    }

    function compileDefinedNamesRegex() {
        var result = [ ];

        for(var definedName in definedNames) {
            result.push(definedName);
        }

        return result.length ? new RegExp(result.join("|"), "g") : null;
    }

    function getCellCoords(cellName, isRange) {
        var pureName = cellName.replace(/'|!/g, "").toUpperCase(),
            r1c1Ref = /R(?:\[(\-?\d+)\])?(\-?\d+)?(?:(C)(?:\[(\-?\d+)\])?(\-?\d+)?)?/.exec(pureName);

//  Commented due to not supporting R1C1 references by core
/*      if(r1c1Ref) {
            if(r1c1Ref[1] || pureName === "R") {
                return {
                    r1c1Style: true,
                    relative: true,
                    fullRow: !r1c1Ref[3],
                    col: r1c1Ref[4] ? parseInt(r1c1Ref[4]) : 0,
                    row: parseInt(r1c1Ref[1] || 0)
                };
            } else if(r1c1Ref[2] && r1c1Ref[5]) {
                return {
                    absolute: true,
                    col: parseInt(r1c1Ref[5]),
                    row: parseInt(r1c1Ref[2])
                };
            } else {
                return false;
            }
        }*/

        var row = /(\$)?(\d+)/.exec(pureName),
            col = /(\$)?([A-Z]+)/.exec(pureName),
            result = { };

        if(row && row.length) {
            result.row = parseInt(row[2]) - 1;
            result.relative = !(row && row[1] || col && col[1]);
            if(col && col.length) {
                result.col = getColumnIndex(col[2]);
            } else if(isRange) {
                result.absolute = true;
                result.fullRow = true;
            } else {
                return false;
            }

            return result;
        } else if(isRange && col && col.length) {
            return {
                absolute: true,
                fullColumn: true,
                col: getColumnIndex(col[0])
            }
        } else {
            return false;
        }
    }

    function parseCells(cells, baseCell) {
        var isRange = cells.length === 2,
            firstCell = getCellCoords(cells[0], isRange);

        if(!firstCell) {
            return;
        }

        var selection = new SelectionClass,
            firstColumnIndex = undefined,
            firstRowIndex = undefined,
            secondColumnIndex = undefined,
            secondRowIndex = undefined;

        if(firstCell.absolute) {
            if(firstCell.fullColumn) {
                firstColumnIndex = firstCell.col;
                firstRowIndex = 0;
            } else if(firstCell.fullRow) {
                firstColumnIndex = 0;
                firstRowIndex = firstCell.row;
            } else {
                firstColumnIndex = firstCell.col - 1;
                firstRowIndex = firstCell.row - 1;
            }
        } else if(firstCell.r1c1Style) {
            var baseCellParsed = baseCell.row !== undefined ? baseCell : getCellCoords(baseCell);

            firstRowIndex = baseCellParsed.row + firstCell.row;
            if(firstCell.fullRow) {
                firstColumnIndex = 0;
                secondRowIndex = firstRowIndex;
                secondColumnIndex = ASPxClientSpreadsheet.Range.MAX_COL_COUNT;
            } else {
                firstColumnIndex = baseCellParsed.col + firstCell.col;
            }
        } else {
            firstColumnIndex = firstCell.col;
            firstRowIndex = firstCell.row;
        }

        if(isRange) {
            var secondCell = getCellCoords(cells[1], isRange);

            if(secondCell.fullColumn) {
                secondColumnIndex = secondCell.col;
                secondRowIndex = ASPxClientSpreadsheet.Range.MAX_ROW_COUNT;
            } else if(secondCell.fullRow) {
                secondColumnIndex = ASPxClientSpreadsheet.Range.MAX_COL_COUNT;
                secondRowIndex = secondCell.row;
            } else if(secondCell.r1c1Style) {
                var baseCellParsed = baseCell.row ? baseCell : getCellCoords(baseCell);

                secondColumnIndex = baseCellParsed.col + secondCell.col;
                secondRowIndex = baseCellParsed.row + secondCell.row;
            } else {
                secondColumnIndex = secondCell.col;
                secondRowIndex = secondCell.row;
            }
        }

        selection.range.set(firstColumnIndex, firstRowIndex, secondColumnIndex, secondRowIndex);

        return selection;
    }

    function parseCore(formula, baseCell, cellsParser, sheetName, highlightedTerms, termPositions) {
        var result = [ ],
            terms = formula.split(/=|\w+\(|\)|(?:[\,\+\-\/\*](?!\s*\d+\s*\]))/i);

        ASPx.Data.ForEach(terms, function(term) {
            var cells = term.replace(/\s+/g, "").split(":");

            if(!(cells.length && cells[0].length && checkCellsSheet(cells, sheetName))) {
                return;
            }

            ASPx.Data.ForEach(cells, function(cell, index) {
                cells[index] = cell.substr(cell.indexOf("!") + 1);
            });

            var parsedCells = cellsParser(cells, baseCell);

            if(parsedCells) {
                if(ASPx.Data.ArrayIndexOf(highlightedTerms.terms, ASPx.Str.Trim(term)) > -1) {
                    parsedCells.range.isHighlighted = true;
                    parsedCells.range.textPosition = highlightedTerms.textPosition;
                } else {
                    parsedCells.range.textPosition = termPositions[ASPx.Str.Trim(term)];
                }
                result.push(parsedCells);
            }
        });

        return result;
    }

    function checkCellsSheet(cells, sheetName) {
        if(sheetName === undefined) {
            return true;
        }

        var result = true;

        ASPx.Data.ForEach(cells, function(cell) {
            var cellParts = cell.split("!");

            if(cellParts.length === 2 && cellParts[0] !== sheetName) {
                result = false;
            }
        });

        return result;
    }

    function processDefinedNames(formula, baseCell) {
        var definedNamesRegex = compileDefinedNamesRegex();

        if(definedNamesRegex) {
            var rootBaseCellParsed = baseCell;

            if(!rootBaseCellParsed.row) {
                rootBaseCellParsed = getCellCoords(baseCell);
            }

            formula = formula.replace(definedNamesRegex, function(definedName) {
                var definedNameValue = definedNames[definedName],
                    compiledFormula = definedNameValue.cell,
                    baseCellParsed = getCellCoords(definedNameValue.baseCell);

                if(definedNamesRegex.test(compiledFormula)) {
                    compiledFormula = processDefinedNames(compiledFormula, baseCellParsed);
                }
                parseCore(compiledFormula, rootBaseCellParsed, function(cells) {
                    ASPx.Data.ForEach(cells, function(cell) {
                        var parsedCell = getCellCoords(cell);

                        if(parsedCell.relative && !parsedCell.r1c1Style) {
                            compiledFormula = compiledFormula.replace(cell, "R[" + (parsedCell.row - baseCellParsed.row) +
                                "]C[" + (parsedCell.col - baseCellParsed.col) + "]")
                        }
                    });
                });

                return compiledFormula;
            });
        }

        return formula;
    }

    ASPx.SpreadsheetFormulaParser = SpreadsheetFormulaParser;
})();