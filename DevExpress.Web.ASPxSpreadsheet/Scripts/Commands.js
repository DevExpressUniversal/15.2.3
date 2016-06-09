ASPxClientSpreadsheet.ServerCommands = (function() {
    var CommandIDs = {
        //web server commands
        LoadSheet                                       : {id: "w0", enabled : {editMode: true, readonlyMode: true, lockedSheet: true}, UIBlocking: true},
        LoadInvisibleTiles                              : {id: "w1", enabled : {editMode: true, readonlyMode: true, lockedSheet: true}, UIBlocking: true},
        UpdateCell                                      : {id: "w2", enabled : {editMode: true, lockedSheet: true}, disabled: { lockedCell: true } },
        FormatFontName                                  : {id: "w3"},
        FormatFontSize                                  : {id: "w4"},
        ResizeColumn                                    : {id: "w5", UIBlocking: true},
        ResizeRow                                       : {id: "w6", UIBlocking: true},
        FormatFontColor                                 : {id: "w7"},
        FormatFillColor                                 : {id: "w8"},
        FormatBorderLineColor                           : {id: "w9"},
        FormatBorderLineStyle                           : {id: "w10"}, 
        InsertHyperlink                                 : {id: "w11", menuVisible : { isLink: false, isDrawing: false, isRowHeader: false, isColumnHeader: false }},
        FileNew                                         : {id: "w12", spreadsheetExecuteMethod : "CreateNewDocumentCallback", enabled : {lockedSheet: true}},
        FileSave                                        : {id: "w13", enabled : {editMode: true, lockedSheet: true}},
        InsertPicture                                   : {id: "w14"}, 
        RenameSheet                                     : {id: "w15", enabled : {lockedSheet: true}, disabled: {lockedWorkbook: true}, menuVisible : { isTabSelectorMenu: true }, menuEnabled: { lockedWorkbook: false }},
        EditHyperlink                                   : {id: "w16", menuVisible : { isLink: true, isDrawing: false }},
        ShapeMoveAndResize                              : {id: "w17"}, 
        FileOpen                                        : {id: "w18", enabled : {                readonlyMode: true, lockedSheet: true}, UIBlocking: true},
        FileSaveAs                                      : {id: "w19", enabled : {editMode: true, readonlyMode: true, lockedSheet: true}, UIBlocking: true},
        UnhideRows                                      : {id: "w20", menuVisible : { isRowHeader: true }},
        UnhideColumns                                   : {id: "w21", menuVisible : { isColumnHeader: true }},
        FormatRowHeight                                 : {id: "w22", UIBlocking: true},          
        FormatColumnWidth                               : {id: "w23", UIBlocking: true},        
        FormatDefaultColumnWidth                        : {id: "w24", UIBlocking: true}, 
        UnhideSheet                                     : {id: "w25", enabled : {lockedSheet: true}, disabled: {lockedWorkbook: true}, menuVisible : { isTabSelectorMenu: true }, menuEnabled: { lockedWorkbook: false, canUnhideSheet: true }},
        ChartChangeType                                 : {id: "w26", menuVisible : { isChart: true }},
        ChartSelectData                                 : {id: "w27", menuVisible : { isChart: true }},
        GetLocalizedStringConstant                      : {id: "w28", enabled : {lockedSheet: true}},        
        ModifyChartLayout                               : {id: "w29", menuVisible : { isChart: true }},
        ChartChangeTitle                                : {id: "w30", menuVisible : { isChart: true, hasTitle: true }},
        ChartChangeHorizontalAxisTitle                  : {id: "w31", menuVisible : { isChart: true, hasHTitle: true }},
        ChartChangeVerticalAxisTitle                    : {id: "w32", menuVisible : { isChart: true, hasVTitle: true }},
        ModifyChartStyle                                : {id: "w33", menuVisible : { isChart: true }},
        DownLoadCopy                                    : {id: "w34"},
        Print                                           : {id: "w35", enabled : {readonlyMode: true, lockedSheet: true}},
		FindAll                                         : {id: "w36", enabled : {readonlyMode: true, lockedSheet: true}/*, UIBlocking: true*/}, 
        ScrollTo                                        : {id: "w37", enabled : {readonlyMode: true, lockedSheet: true} /*, UIBlocking: true*/ },
        SetPaperKind                                    : {id: "w38"},

        Paste                                           : {id: "w39", enabled : {editMode: true, lockedSheet: true}, disabled: { lockedCell: true}, UIBlocking: true },

        GetPictureContent                               : {id: "w40"}, /*initialized on server*/
        InsertTable                                     : {id: "w41", enabled : { canInsertTable: true }, isTableCommand: true },
        FetchAutoFilterViewModel                        : {id: "w42"},
        ApplyAutoFilter                                 : {id: "w43"},

        FetchDataValidationViewModel                    : {id: "w44"},
        ApplyDataValidation                             : {id: "w45"},
        FetchListAllowedValues                          : {id: "w46"},
        MoveOrCopySheet                                 : {id: "w47", menuVisible : { isTabSelectorMenu: true }},
        FetchMessageForCell                             : {id: "w48"},
        TableToolsRenameTable                           : {id: "w49", isTableCommand: true },
        ModifyTableStyle                                : {id: "w50", isTableCommand: true },
        FormatAsTable                                   : {id: "w51", enabled : { canFormatAsTable: true }, isTableCommand: true },
        InsertTableWithStyle                            : {id: "w52"},
        RemoveSheet                                     : {id: "w53", enabled : {lockedSheet: true}, disabled: {lockedWorkbook: true}, menuVisible : { isTabSelectorMenu: true }, menuEnabled: { lockedWorkbook: false, canRemoveSheet: true }},
        PageSetup                                       : {id: "w54"},
        FetchPageSetupViewModel                         : {id: "w55"},
        ApplyPageSetupSettings                          : {id: "w56"},
        MoveRange                                       : {id: "w57"},
        FreezePanes                                     : {id: "w58" },
        FreezeRow                                       : {id: "w59" },
        FreezeColumn                                    : { id: "w60" },
        AutoFitHeaderSize                               : { id: "w61" },

        //virtual commands
        LoadInvisibleTilesForFullScreenMode             : {id: "v1", enabled : {editMode: true, readonlyMode: true, lockedSheet: true}, UIBlocking: true},
        //client commands
        FullScreen                                      : {id: "c1", spreadsheetExecuteMethod : "setFullScreenModeInternal", ribbonManagerUpdateMethod: "onFullScreenCommandExecuted", enabled : {readonlyMode: true, lockedSheet: true}},
        //Server commands
        FileUndo                                        : {id: 8, enabled : {lockedSheet: true}}, 
        FileRedo                                        : {id: 9, enabled : {lockedSheet: true}}, 
        FormatFontBold                                  : {id: 100},
        FormatFontItalic                                : {id: 101},
        FormatFontUnderline                             : {id: 102},
        FormatFontStrikeout                             : {id: 103},
        FormatIncreaseFontSize                          : {id: 106},
        FormatDecreaseFontSize                          : {id: 107},
        FormatAlignmentLeft                             : {id: 110},
        FormatAlignmentCenter                           : {id: 111},
        FormatAlignmentRight                            : {id: 112},
        FormatAlignmentTop                              : {id: 113},
        FormatAlignmentMiddle                           : {id: 114},
        FormatAlignmentBottom                           : {id: 115},
        FormatWrapText                                  : {id: 116},
        FormatIncreaseIndent                            : {id: 117},
        FormatDecreaseIndent                            : {id: 118}, 
        FormatThickBorder                               : {id: 121}, //in right bottom corner 1 pixel don't showing
        FormatOutsideBorders                            : {id: 122}, 
        FormatLeftBorder                                : {id: 123},
        FormatRightBorder                               : {id: 124}, 
        FormatTopBorder                                 : {id: 125}, 
        FormatBottomBorder                              : {id: 126}, 
        FormatBottomDoubleBorder                        : {id: 127}, // TODO, atm (at this moment) medium bottom line
        FormatBottomThickBorder                         : {id: 128},
        FormatTopAndBottomBorder                        : {id: 129}, 
        FormatTopAndThickBottomBorder                   : {id: 130}, 
        FormatTopAndDoubleBottomBorder                  : {id: 131}, // TODO, atm medium bottom and thin top lines
        FormatAllBorders                                : {id: 132}, 
        FormatNoBorders                                 : {id: 133}, 
        FormatBordersCommandGroup                       : {id: 134},        
        FormatNumber                                    : {id: 150},
        FormatNumberPercent                             : {id: 153},
        FormatNumberPercentage                          : {id: 154},
        FormatNumberScientific                          : {id: 155},
        FormatNumberAccounting                          : {id: 157},
        FormatNumberPredefined4                         : {id: 164},
        FormatNumberPredefined8                         : {id: 165},
        FormatNumberPredefined15                        : {id: 166},
        FormatNumberPredefined18                        : {id: 167},
        FormatNumberAccountingUS                        : {id: 168},
        FormatNumberAccountingUK                        : {id: 169},
        FormatNumberAccountingPRC                       : {id: 170},
        FormatNumberAccountingEuro                      : {id: 171},
        FormatNumberAccountingSwiss                     : {id: 172},
        FormatNumberAccountingCommandGroup              : {id: 173},
        FormatNumberIncreaseDecimal                     : {id: 174},
        FormatNumberDecreaseDecimal                     : {id: 175},
        FormatClearCommandGroup                         : {id: 200},
        FormatClearAll                                  : {id: 201},
        FormatClearFormats                              : {id: 202},
        FormatClearContents                             : {id: 203, enabled : {lockedSheet: true}, menuVisible : { isDrawing: false }},
        FormatClearContentsContextMenuItem              : {id: 204},
        FormatClearComments                             : {id: 205},
        FormatClearHyperlinks                           : {id: 206},
        FormatRemoveHyperlinks                          : {id: 207},
        EditingAutoSumCommandGroup                      : {id: 208},
        EditingMergeAndCenterCells                      : {id: 209},
        EditingMergeCellsAcross                         : {id: 210},
        EditingMergeCells                               : {id: 211},
        EditingUnmergeCells                             : {id: 212},
        EditingMergeCellsCommandGroup                   : {id: 213},
        EditingFillDown                                 : {id: 214},
        EditingFillUp                                   : {id: 215},
        EditingFillLeft                                 : {id: 216},
        EditingFillRight                                : {id: 217},
        EditingFillCommandGroup                         : {id: 218},
        EditingSortAndFilterCommandGroup                : {id: 219},
        EditingFindAndSelectCommandGroup                : {id: 220},
        FunctionsInsertSpecificFunction                 : {id: 400, enabled : {editMode: true}},
        FunctionsFinancialCommandGroup                  : {id: 401, enabled : {editMode: true}},
        FunctionsLogicalCommandGroup                    : {id: 402, enabled : {editMode: true}},
        FunctionsTextCommandGroup                       : {id: 403, enabled : {editMode: true}},
        FunctionsDateAndTimeCommandGroup                : {id: 404, enabled : {editMode: true}},
        FunctionsLookupAndReferenceCommandGroup         : {id: 405, enabled : {editMode: true}},
        FunctionsMathAndTrigonometryCommandGroup        : {id: 406, enabled : {editMode: true}},
        FunctionsMoreCommandGroup                       : {id: 407, enabled : {editMode: true}},
        FunctionsStatisticalCommandGroup                : {id: 408, enabled : {editMode: true}},
        FunctionsEngineeringCommandGroup                : {id: 409, enabled : {editMode: true}},
        FunctionsCubeCommandGroup                       : {id: 410, enabled : {editMode: true}},
        FunctionsInformationCommandGroup                : {id: 411, enabled : {editMode: true}},
        FunctionsCompatibilityCommandGroup              : {id: 412, enabled : {editMode: true}},
        FunctionsAutoSumCommandGroup                    : {id: 413, enabled : {editMode: true}},
        FunctionsInsertSum                              : {id: 414, enabled : {editMode: true}},
        FunctionsInsertAverage                          : {id: 415, enabled : {editMode: true}},
        FunctionsInsertCountNumbers                     : {id: 416, enabled : {editMode: true}},
        FunctionsInsertMax                              : {id: 417, enabled : {editMode: true}},
        FunctionsInsertMin                              : {id: 418, enabled : {editMode: true}},
        FormulasCalculateNow                            : {id: 422},
        FormulasCalculateSheet                          : {id: 423},
        FormulasCalculationOptionsCommandGroup          : {id: 424},
        FormulasCalculationModeAutomatic                : {id: 425},
        FormulasCalculationModeManual                   : {id: 426},
        ViewShowGridlines                               : {id: 603, enabled : { readonlyMode: true, lockedSheet: true} },
        ViewUnfreezePanes                               : {id: 607},
        PageSetupMarginsNormal                          : {id: 650},
        PageSetupMarginsNarrow                          : {id: 651},
        PageSetupMarginsWide                            : {id: 652},
        PageSetupMarginsCommandGroup                    : {id: 653},
        PageSetupOrientationPortrait                    : {id: 654},
        PageSetupOrientationLandscape                   : {id: 655},
        PageSetupOrientationCommandGroup                : {id: 656},
        PageSetupPaperKindCommandGroup                  : {id: 658},
        PageSetupPrintGridlines                         : {id: 663, enabled : {readonlyMode: true, lockedSheet: true}},
        PageSetupPrintHeadings                          : {id: 664, enabled : {readonlyMode: true, lockedSheet: true}},
        InsertCellsCommandGroup                         : {id: 700, enabled : {lockedSheet: true}},
        InsertSheet                                     : {id: 701, enabled : {lockedSheet: true}, disabled: {lockedWorkbook: true}, menuVisible : { isTabSelectorMenu: true }, menuEnabled: { lockedWorkbook: false }},
        InsertSheetRows                                 : {id: 703},
        InsertSheetColumns                              : {id: 705},
        InsertHyperlinkContextMenuItem                  : {id: 712},
        EditHyperlinkContextMenuItem                    : {id: 713},
        OpenHyperlinkContextMenuItem                    : {id: 714},
        RemoveHyperlinkContextMenuItem                  : {id: 715, menuVisible : { isLink: true, isDrawing: false }},
        RemoveSheetRows                                 : {id: 1002},        
        RemoveSheetColumns                              : {id: 1004},        
        RemoveCellsCommandGroup                         : {id: 1006, enabled : {lockedSheet: true}},
        CopySelection                                   : {id: 1100, enabled : {editMode: true, readonlyMode: true, lockedSheet: true}, menuVisible : { isChart: false, isDrawing: false }},
        PasteSelection                                  : {id: 1101, enabled : {editMode: true, lockedSheet: true}, disabled: { lockedCell: true}, menuVisible : { isChart: false, isDrawing: false } },
        CutSelection                                    : {id: 1102, enabled : {editMode: true, lockedSheet: true}, disabled: { lockedCell: true}, menuVisible : { isChart: false, isDrawing: false }},
        FormatCommandGroup                              : {id: 1200, enabled : {lockedSheet: true}},
        FormatAutoFitRowHeight                          : {id: 1201},
        FormatAutoFitColumnWidth                        : {id: 1202},
        HideRows                                        : {id: 1203, menuVisible : { isRowHeader: true }},
        HideColumns                                     : {id: 1205, menuVisible : { isColumnHeader: true }},
        HideSheet                                       : {id: 1207, enabled : {lockedSheet: true}, disabled: {lockedWorkbook: true}, menuVisible : { isTabSelectorMenu: true }, menuEnabled: { lockedWorkbook: false, canHideSheet: true }},
        HideAndUnhideCommandGroup                       : {id: 1217, enabled : {lockedSheet: true}},
        DataSortAscending                               : {id: 1450, enabled: { canSort: true }, disabled : { readonlyMode: true }, menuVisible : { isFilterMenu: true }, isTableCommand: true },
        DataSortDescending                              : {id: 1451, enabled: { canSort: true }, disabled : { readonlyMode: true }, menuVisible : { isFilterMenu: true }, isTableCommand: true },

        DataFilterEquals                                : {id: 1452},
        DataFilterDoesNotEqual                          : {id: 1453},
        DataFilterGreaterThan                           : {id: 1454},
        DataFilterGreaterThanOrEqualTo                  : {id: 1455},
        DataFilterLessThan                              : {id: 1456},
        DataFilterLessThanOrEqualTo                     : {id: 1457},
        DataFilterBetween                               : {id: 1458},
        DataFilterTop10                                 : {id: 1459},
        DataFilterAboveAverage                          : {id: 1460},
        DataFilterBelowAverage                          : {id: 1461},
        DataFilterBeginsWith                            : {id: 1462},
        DataFilterEndsWith                              : {id: 1463},
        DataFilterContains                              : {id: 1464},
        DataFilterDoesNotContain                        : {id: 1465},
        DataFilterCustom                                : {id: 1466},
        DataFilterClear                                 : {id: 1467, enabled : { isFilterApplied: true }, isTableCommand: true },
        DataFilterColumnClear                           : {id: 1468, menuVisible : { isFilterMenu: true }, menuEnabled: { isFilterApplied: true }},
        DataFilterReApply                               : {id: 1469, enabled : { isFilterApplied: true }, isTableCommand: true },
        DataFilterToggle                                : {id: 1470, enabled : { canToggleFilter: true }, checked : { isFilterEnabled: true }, isTableCommand: true },

        DataFilterSimple                                : {id: 1471, menuVisible : { isFilterMenu: true }},

        DataFilterNumberFiltersCommandGroup             : {id: 1473, menuVisible : { isFilterMenu: true, isNumberColumn: true }},
        DataFilterTextFiltersCommandGroup               : {id: 1474, menuVisible : { isFilterMenu: true, isTextColumn: true }},
        DataFilterDateFiltersCommandGroup               : {id: 1475, menuVisible : { isFilterMenu: true, isDateColumn: true }},
        DataFilterAllDatesInPeriodCommandGroup          : {id: 1476},

        DataFilterToday                                 : {id: 1600},
        DataFilterYesterday                             : {id: 1601},
        DataFilterTomorrow                              : {id: 1602},
        DataFilterThisWeek                              : {id: 1603},
        DataFilterLastWeek                              : {id: 1604},
        DataFilterNextWeek                              : {id: 1605},
        DataFilterThisMonth                             : {id: 1606},
        DataFilterLastMonth                             : {id: 1607},
        DataFilterNextMonth                             : {id: 1608},
        DataFilterThisQuarter                           : {id: 1609},
        DataFilterLastQuarter                           : {id: 1610},
        DataFilterNextQuarter                           : {id: 1611},
        DataFilterThisYear                              : {id: 1612},
        DataFilterLastYear                              : {id: 1613},
        DataFilterNextYear                              : {id: 1614},
        DataFilterYearToDa                              : {id: 1615},
        DataFilterQuarter1                              : {id: 1616},
        DataFilterQuarter2                              : {id: 1617},
        DataFilterQuarter3                              : {id: 1618},
        DataFilterQuarter4                              : {id: 1619},
        DataFilterMonthJanuary                          : {id: 1620},
        DataFilterMonthFebruary                         : {id: 1621},
        DataFilterMonthMarch                            : {id: 1622},
        DataFilterMonthApril                            : {id: 1623},
        DataFilterMonthMay                              : {id: 1624},
        DataFilterMonthJune                             : {id: 1625},
        DataFilterMonthJuly                             : {id: 1626},
        DataFilterMonthAugust                           : {id: 1627},
        DataFilterMonthSeptember                        : {id: 1628},
        DataFilterMonthOctober                          : {id: 1630},
        DataFilterMonthNovember                         : {id: 1631},
        DataFilterMonthDecember                         : {id: 1632},

        DataFilterDateEquals                            : {id: 1633},
        DataFilterDateBefore                            : {id: 1634},
        DataFilterDateAfter                             : {id: 1635},
        DataFilterDateBetween                           : {id: 1636},
        DataFilterDateCustom                            : {id: 1637},

        DataToolsCircleInvalidData                      : {id: 1702},
        DataToolsClearValidationCircles                 : {id: 1703},

        InsertChartColumnCommandGroup                   : {id: 2170},
        InsertChartBarCommandGroup                      : {id: 2171},
        InsertChartColumn2DCommandGroup                 : {id: 2172},
        InsertChartColumn3DCommandGroup                 : {id: 2173},
        InsertChartCylinderCommandGroup                 : {id: 2174},
        InsertChartConeCommandGroup                     : {id: 2175},
        InsertChartPyramidCommandGroup                  : {id: 2176},
        InsertChartBar2DCommandGroup                    : {id: 2177},
        InsertChartBar3DCommandGroup                    : {id: 2178},
        InsertChartHorizontalCylinderCommandGroup       : {id: 2179},
        InsertChartHorizontalConeCommandGroup           : {id: 2180},
        InsertChartHorizontalPyramidCommandGroup        : {id: 2181},
        InsertChartPieCommandGroup                      : {id: 2182},
        InsertChartPie2DCommandGroup                    : {id: 2183},
        InsertChartPie3DCommandGroup                    : {id: 2184},
        InsertChartLineCommandGroup                     : {id: 2185},
        InsertChartLine2DCommandGroup                   : {id: 2186},
        InsertChartLine3DCommandGroup                   : {id: 2187},
        InsertChartAreaCommandGroup                     : {id: 2188},
        InsertChartArea2DCommandGroup                   : {id: 2189},
        InsertChartArea3DCommandGroup                   : {id: 2190},
        InsertChartScatterCommandGroup                  : {id: 2191},
        InsertChartBubbleCommandGroup                   : {id: 2192},
        InsertChartDoughnut2DCommandGroup               : {id: 2193},
        InsertChartOtherCommandGroup                    : {id: 2194},
        InsertChartStockCommandGroup                    : {id: 2195},
        InsertChartRadarCommandGroup                    : {id: 2196},
        InsertChartColumnClustered2D                    : {id: 2200},
        InsertChartColumnStacked2D                      : {id: 2201},
        InsertChartColumnPercentStacked2D               : {id: 2202},
        InsertChartColumnClustered3D                    : {id: 2203},
        InsertChartColumnStacked3D                      : {id: 2204},
        InsertChartColumnPercentStacked3D               : {id: 2205},
        InsertChartCylinderClustered                    : {id: 2206},
        InsertChartCylinderStacked                      : {id: 2207},
        InsertChartCylinderPercentStacked               : {id: 2208},
        InsertChartConeClustered                        : {id: 2209},
        InsertChartConeStacked                          : {id: 2210},
        InsertChartConePercentStacked                   : {id: 2211},
        InsertChartPyramidClustered                     : {id: 2212},
        InsertChartPyramidStacked                       : {id: 2213},
        InsertChartPyramidPercentStacked                : {id: 2214},
        InsertChartBarClustered2D                       : {id: 2215},
        InsertChartBarStacked2D                         : {id: 2216},
        InsertChartBarPercentStacked2D                  : {id: 2217},
        InsertChartBarClustered3D                       : {id: 2218},
        InsertChartBarStacked3D                         : {id: 2219},
        InsertChartBarPercentStacked3D                  : {id: 2220},
        InsertChartHorizontalCylinderClustered          : {id: 2221},
        InsertChartHorizontalCylinderStacked            : {id: 2222},
        InsertChartHorizontalCylinderPercentStacked     : {id: 2223},
        InsertChartHorizontalConeClustered              : {id: 2224},
        InsertChartHorizontalConeStacked                : {id: 2225},
        InsertChartHorizontalConePercentStacked         : {id: 2226},
        InsertChartHorizontalPyramidClustered           : {id: 2227},
        InsertChartHorizontalPyramidStacked             : {id: 2228},
        InsertChartHorizontalPyramidPercentStacked      : {id: 2229},
        InsertChartColumn3D                             : {id: 2230},
        InsertChartCylinder                             : {id: 2231},
        InsertChartCone                                 : {id: 2232},
        InsertChartPyramid                              : {id: 2233},
        InsertChartPie2D                                : {id: 2234},
        InsertChartPie3D                                : {id: 2235},
        InsertChartPieExploded2D                        : {id: 2236},
        InsertChartPieExploded3D                        : {id: 2237},
        InsertChartLine                                 : {id: 2238},
        InsertChartStackedLine                          : {id: 2239},
        InsertChartPercentStackedLine                   : {id: 2240},
        InsertChartLineWithMarkers                      : {id: 2241},
        InsertChartStackedLineWithMarkers               : {id: 2242},
        InsertChartPercentStackedLineWithMarkers        : {id: 2243},
        InsertChartLine3D                               : {id: 2244},
        InsertChartArea                                 : {id: 2245},
        InsertChartStackedArea                          : {id: 2246},
        InsertChartPercentStackedArea                   : {id: 2247},
        InsertChartArea3D                               : {id: 2248},
        InsertChartStackedArea3D                        : {id: 2249},
        InsertChartPercentStackedArea3D                 : {id: 2250},
        InsertChartScatterMarkers                       : {id: 2251},
        InsertChartScatterLines                         : {id: 2252},
        InsertChartScatterSmoothLines                   : {id: 2253},
        InsertChartScatterLinesAndMarkers               : {id: 2254},
        InsertChartScatterSmoothLinesAndMarkers         : {id: 2255},
        InsertChartBubble                               : {id: 2256},
        InsertChartBubble3D                             : {id: 2257},
        InsertChartDoughnut2D                           : {id: 2258},
        InsertChartDoughnutExploded2D                   : {id: 2259},
        InsertChartRadar                                : {id: 2260},
        InsertChartRadarWithMarkers                     : {id: 2261},
        InsertChartRadarFilled                          : {id: 2262},
        InsertChartStockHighLowClose                    : {id: 2263},
        InsertChartStockOpenHighLowClose                : {id: 2264},
        ChartSwitchRowColumn                            : {id: 2379, menuVisible : { isChart: true }},
        TableToolsToggleHeaderRow                       : {id: 2400, checked : { showHeaderRow    : true }, isTableCommand: true, isContextCommand: true },
        TableToolsToggleTotalRow                        : {id: 2401, checked : { showTotalRow     : true }, isTableCommand: true, isContextCommand: true },
        TableToolsToggleBandedColumns                   : {id: 2402, checked : { showBandedColumns: true }, isTableCommand: true, isContextCommand: true },
        TableToolsToggleBandedRows                      : {id: 2403, checked : { showBandedRows   : true }, isTableCommand: true, isContextCommand: true },
        TableToolsToggleFirstColumn                     : {id: 2404, checked : { showFirstColumn  : true }, isTableCommand: true, isContextCommand: true },
        TableToolsToggleLastColumn                      : {id: 2405, checked : { showLastColumn   : true }, isTableCommand: true, isContextCommand: true },
        TableToolsConvertToRange                        : {id: 2408, isTableCommand: true },
        ArrangeBringForwardCommandGroup                 : {id: 2600, menuVisible : { isDrawing: true }, menuEnabled : { isArrangeEnabled: true }, enabled : { isArrangeEnabled: true }, isPictureCommand: true, isChartCommand: true },
        ArrangeBringForward                             : {id: 2602, enabled : { isArrangeEnabled: true }, isPictureCommand: true, isChartCommand: true },
        ArrangeBringToFront                             : {id: 2603},
        ArrangeSendBackwardCommandGroup                 : {id: 2604, menuVisible : { isDrawing: true }, menuEnabled : { isArrangeEnabled: true }, enabled : { isArrangeEnabled: true }, isPictureCommand: true, isChartCommand: true },
        ArrangeSendBackward                             : {id: 2606},
        ArrangeSendToBack                               : {id: 2607},
        ChartTitleCommandGroup                          : {id: 2341, menuVisible : { isChart: true }},
        ChartTitleNone                                  : {id: 2342},
        ChartTitleCenteredOverlay                       : {id: 2343},
        ChartTitleAbove                                 : {id: 2344},
        ChartAxisTitlesCommandGroup                     : {id: 2332, menuVisible : { isChart: true }},
        ChartPrimaryHorizontalAxisTitleCommandGroup     : {id: 2333},
        ChartPrimaryVerticalAxisTitleCommandGroup       : {id: 2334},
        ChartPrimaryHorizontalAxisTitleNone             : {id: 2335},
        ChartPrimaryHorizontalAxisTitleBelow            : {id: 2336},
        ChartPrimaryVerticalAxisTitleNone               : {id: 2337},
        ChartPrimaryVerticalAxisTitleRotated            : {id: 2338},
        ChartPrimaryVerticalAxisTitleVertical           : {id: 2339},
        ChartPrimaryVerticalAxisTitleHorizontal         : {id: 2340},
        ChartLegendCommandGroup                         : {id: 2345, menuVisible : { isChart: true }},
        ChartLegendNone                                 : {id: 2346},
        ChartLegendAtRight                              : {id: 2347},
        ChartLegendAtTop                                : {id: 2348},
        ChartLegendAtLeft                               : {id: 2349},
        ChartLegendAtBottom                             : {id: 2350},
        ChartLegendOverlayAtRight                       : {id: 2351},
        ChartLegendOverlayAtLeft                        : {id: 2352},
        ChartDataLabelsCommandGroup                     : {id: 2353, menuVisible : { isChart: true }},
        ChartDataLabelsNone                             : {id: 2354},
        ChartDataLabelsDefault                          : {id: 2355},
        ChartDataLabelsCenter                           : {id: 2356},
        ChartDataLabelsInsideEnd                        : {id: 2357},
        ChartDataLabelsInsideBase                       : {id: 2358},
        ChartDataLabelsOutsideEnd                       : {id: 2359},
        ChartDataLabelsBestFit                          : {id: 2360},
        ChartDataLabelsLeft                             : {id: 2361},
        ChartDataLabelsRight                            : {id: 2362},
        ChartDataLabelsAbove                            : {id: 2363},
        ChartDataLabelsBelow                            : {id: 2364},
        ChartGridlinesCommandGroup                      : {id: 2321, menuVisible : { isChart: true }},
        ChartPrimaryHorizontalGridlinesCommandGroup     : {id: 2322},
        ChartPrimaryVerticalGridlinesCommandGroup       : {id: 2323},
        ChartPrimaryHorizontalGridlinesNone             : {id: 2324},
        ChartPrimaryHorizontalGridlinesMajor            : {id: 2325},
        ChartPrimaryHorizontalGridlinesMinor            : {id: 2326},
        ChartPrimaryHorizontalGridlinesMajorAndMinor    : {id: 2327},
        ChartPrimaryVerticalGridlinesNone               : {id: 2328},
        ChartPrimaryVerticalGridlinesMajor              : {id: 2329},
        ChartPrimaryVerticalGridlinesMinor              : {id: 2330},
        ChartPrimaryVerticalGridlinesMajorAndMinor      : {id: 2331},
        ChartAxesCommandGroup                           : {id: 2300, menuVisible : { isChart: true }},
        ChartPrimaryHorizontalAxisCommandGroup          : {id: 2301},
        ChartPrimaryVerticalAxisCommandGroup            : {id: 2302},
        ChartHidePrimaryHorizontalAxis                  : {id: 2303},
        ChartPrimaryHorizontalAxisLeftToRight           : {id: 2304},
        ChartPrimaryHorizontalAxisRightToLeft           : {id: 2305},
        ChartPrimaryHorizontalAxisHideLabels            : {id: 2306},
        ChartPrimaryHorizontalAxisDefault               : {id: 2307},
        ChartPrimaryHorizontalAxisScaleLogarithm        : {id: 2308},
        ChartPrimaryHorizontalAxisScaleThousands        : {id: 2309},
        ChartPrimaryHorizontalAxisScaleMillions         : {id: 2310},
        ChartPrimaryHorizontalAxisScaleBillions         : {id: 2311},
        ChartHidePrimaryVerticalAxis                    : {id: 2312},
        ChartPrimaryVerticalAxisLeftToRight             : {id: 2313},
        ChartPrimaryVerticalAxisRightToLeft             : {id: 2314},
        ChartPrimaryVerticalAxisHideLabels              : {id: 2315},
        ChartPrimaryVerticalAxisDefault                 : {id: 2316},
        ChartPrimaryVerticalAxisScaleLogarithm          : {id: 2317},
        ChartPrimaryVerticalAxisScaleThousands          : {id: 2318},
        ChartPrimaryVerticalAxisScaleMillions           : {id: 2319},
        ChartPrimaryVerticalAxisScaleBillions           : {id: 2320}
    };

    var commandSeparator = ";#";

    function createCommand(CommandID, commandParamNames) {
        return function(spreadSheet, commandParams) {
            executeCommandBase(spreadSheet, CommandID, commandParamNames, commandParams);
        }
    }
    function createCustomProcessedCommand(CommandID, commandParamNames) {
        var customResultProcessingFunctionName = commandParamNames["CustomResultProcessingFunction"];
        return function(spreadSheet, commandParams) {
            if(!commandParams["CustomResultProcessingFunction"])
                commandParams["CustomResultProcessingFunction"] = customResultProcessingFunctionName;
            executeCommandBase(spreadSheet, CommandID, commandParamNames, commandParams);
        }
    }
    function createSaveCommand(CommandID, commandParamNames) {
        return function(spreadSheet, commandParams) {
            if (spreadSheet.documentName == "") {
                spreadSheet.onUICommand(CommandIDs.FileSaveAs.id, null);
            } else {
                spreadSheet.SaveDocumentCallback();
            }
        }
    }
    function createBorderCommand(CommandID, commandParamNames) {
        return function(spreadSheet, commandParams) {
            if (commandParams.indexOf(commandSeparator))
                commandParams = commandParams.split(commandSeparator)[1];
            executeCommandBase(spreadSheet, CommandID, commandParamNames, commandParams);
        }
    }

    function createSetPaperKindCommand(CommandID, commandParamNames) {
        return function(spreadSheet, commandParams) {
            if (commandParams.indexOf(commandSeparator))
                commandParams = commandParams.split(commandSeparator)[1];
            executeCommandBase(spreadSheet, CommandID, commandParamNames, commandParams);
        }
    }
    function createAutoSumCommand(CommandID, commandParamNames) {
        return function(spreadSheet, commandParams) {
            if (spreadSheet.isSingleCellOrEmptyRangeSelected()) 
                spreadSheet.insertSpecificFunctionToActiveEditor(getFunctionNameById(CommandID));            
            else executeCommandBase(spreadSheet, CommandID,commandParamNames, commandParams);
        }
    }
    function createFormulaCommand(CommandID, commandParamNames) {
        return function(spreadSheet, commandParams) {
            var commandName = commandParams.split(commandSeparator)[1];
            spreadSheet.insertSpecificFunctionToActiveEditor(commandName); 
        }
    }

    function createExportCommand(CommandID, commandParamNames) {
        return function(spreadSheet, commandParams) {
            var requestParams = getRequestParams(CommandID, commandParamNames, commandParams);
            spreadSheet.sendDownloadCopyRequest(requestParams, CommandID);
        }
    }
    function createPrintCommand(CommandID, commandParamNames) {
        return function(spreadSheet, commandParams) {
            var requestParams = getRequestParams(CommandID, commandParamNames, commandParams);
            spreadSheet.sendPrintRequest(requestParams, CommandID);
        }
    }
    function createClientCommand(clientCommand) {
        return function(spreadSheet) {
            
            var ribbonUpdateMethod = spreadSheet.getRibbonManager()[clientCommand.ribbonManagerUpdateMethod];
            if(ribbonUpdateMethod)
                ribbonUpdateMethod(clientCommand.id);

            spreadSheet[clientCommand.spreadsheetExecuteMethod]();
        }
    }

    function createDefaultParamsCommand(CommandID, commandParamNames) {
        return function(spreadSheet, commandParams) {
            executeCommandBase(spreadSheet, CommandID, commandParamNames, commandParams || commandParamNames);
        }
    }

    function executeCommandBase(spreadSheet, CommandID, commandParamNames, commandParams) {
        var requestParams = getRequestParams(CommandID, commandParamNames, commandParams);
        spreadSheet.sendCommandRequest(requestParams, CommandID, commandParams);
    }

    function getRequestParams(CommandID, commandParamNames, commandParams) {
        var requestParams = "CommandID=" + CommandID;
        if (typeof commandParams == 'string' || commandParams instanceof String) {
            var keys = ASPx.GetObjectKeys(commandParamNames);
            requestParams += "&" + keys[0] + "=" + commandParams; 
        } else if(commandParamNames) {
            for(var paramName in commandParamNames) {
                requestParams += "&" + paramName + "=" + encodeURIComponent(commandParams[paramName]);
            }
        }
        return requestParams;
    }

    function getFunctionNameById (commandId) {
        if (CommandIDs.FunctionsInsertSum.id == commandId) return "SUM";
        if (CommandIDs.FunctionsInsertAverage.id == commandId) return "AVERAGE";   
        if (CommandIDs.FunctionsInsertCountNumbers.id == commandId) return "COUNT";
        if (CommandIDs.FunctionsInsertMax.id == commandId) return "MAX";      
        if (CommandIDs.FunctionsInsertMin.id == commandId) return "MIN";
    }

    var commands = {
        CellUpdate                                  : createCommand(CommandIDs.UpdateCell.id,                       {CellPositionColumn:undefined, CellPositionRow:undefined, NewValue:undefined, ReselectAfterCommand:undefined, ConfirmResult:"None"}),
        FormatFontName                              : createCommand(CommandIDs.FormatFontName.id,                   {FontName: undefined}),
        FormatFontSize                              : createCommand(CommandIDs.FormatFontSize.id,                   {FontSize: undefined}),
        ResizeColumn                                : createCommand(CommandIDs.ResizeColumn.id,                     {ColumnIndex: undefined, ColumnWidth: undefined}),
        ResizeRow                                   : createCommand(CommandIDs.ResizeRow.id,                        {RowIndex: undefined, RowHeight: undefined}),
        FormatFontColor                             : createCommand(CommandIDs.FormatFontColor.id,                  {FormatFontColor: undefined}),
        FormatFillColor                             : createCommand(CommandIDs.FormatFillColor.id,                  {FormatFillColor: undefined}),
        FormatBorderLineColor                       : createCommand(CommandIDs.FormatBorderLineColor.id,            {FormatBorderLineColor: undefined}),
        FormatBorderLineStyle                       : createBorderCommand(CommandIDs.FormatBorderLineStyle.id,      {FormatBorderLineStyle: undefined}),
        InsertHyperlink                             : createCommand(CommandIDs.InsertHyperlink.id,                  {HyperLinkDisplayText:undefined, HyperLinkScreenTip:undefined, HyperLinkUrlAddress:undefined}),
        InsertPicture                               : createCommand(CommandIDs.InsertPicture.id,                    {PicturePath:undefined}),
        RenameSheet                                 : createCommand(CommandIDs.RenameSheet.id,                      {NewName:undefined}),
        EditHyperlink                               : createCommand(CommandIDs.InsertHyperlink.id,                  {HyperLinkDisplayText:undefined, HyperLinkScreenTip:undefined, HyperLinkUrlAddress:undefined}),
        ShapeMoveAndResize                          : createCommand(CommandIDs.ShapeMoveAndResize.id,               {shapeOffsetX:undefined, shapeOffsetY:undefined, shapeWidth:undefined, shapeHeight:undefined}),
        FileOpen                                    : createCommand(CommandIDs.FileOpen.id,                         {FilePath:undefined}),
        FileSaveAs                                  : createCommand(CommandIDs.FileSaveAs.id,                       {FilePath:undefined}),
        FileSave                                    : createSaveCommand(CommandIDs.FileSave.id),
        FileNew                                     : createClientCommand(CommandIDs.FileNew),
        FunctionsInsertSum                          : createAutoSumCommand(CommandIDs.FunctionsInsertSum.id),
        FunctionsInsertAverage                      : createAutoSumCommand(CommandIDs.FunctionsInsertAverage.id),
        FunctionsInsertCountNumbers                 : createAutoSumCommand(CommandIDs.FunctionsInsertCountNumbers.id),
        FunctionsInsertMax                          : createAutoSumCommand(CommandIDs.FunctionsInsertMax.id),
        FunctionsInsertMin                          : createAutoSumCommand(CommandIDs.FunctionsInsertMin.id),
        FunctionsInsertSpecificFunction             : createFormulaCommand(CommandIDs.FunctionsInsertSpecificFunction.id),
        FormatRowHeight                             : createCommand(CommandIDs.FormatRowHeight.id,                  {RowHeight: undefined}),       
        FormatColumnWidth                           : createCommand(CommandIDs.FormatColumnWidth.id,                {ColumnWidth: undefined}),      
        FormatDefaultColumnWidth                    : createCommand(CommandIDs.FormatDefaultColumnWidth.id,         {DefaultColumnWidth: undefined}),
        UnhideSheet                                 : createCommand(CommandIDs.UnhideSheet.id,                      {SheetName: undefined}),
        ChartChangeType                             : createCommand(CommandIDs.ChartChangeType.id,                  {ChartType: undefined}),
        ChartSelectData                             : createCommand(CommandIDs.ChartSelectData.id,                  {SelectionRange: undefined}),
        GetLocalizedStringConstant                  : createCommand(CommandIDs.GetLocalizedStringConstant.id,       {XtraSpreadsheetStringId: undefined, CustomResultProcessingFunction: undefined}),
        ModifyChartLayout                           : createCommand(CommandIDs.ModifyChartLayout.id,                {ChartLayoutPreset: undefined}),
        ChartChangeTitle                            : createCommand(CommandIDs.ChartChangeTitle.id,                 {ChartTitle: undefined}),
        ChartChangeHorizontalAxisTitle              : createCommand(CommandIDs.ChartChangeHorizontalAxisTitle.id,   {ChartHorizontalAxisTitle: undefined}),
        ChartChangeVerticalAxisTitle                : createCommand(CommandIDs.ChartChangeVerticalAxisTitle.id,     {ChartVerticalAxisTitle: undefined}),
        ModifyChartStyle                            : createCommand(CommandIDs.ModifyChartStyle.id,                 {ChartPresetStyle: undefined}),
        DownLoadCopy                                : createExportCommand(CommandIDs.DownLoadCopy.id,               {FileFormat: undefined}),
        Print                                       : createPrintCommand(CommandIDs.Print.id),
        SetPaperKind                                : createSetPaperKindCommand(CommandIDs.SetPaperKind.id,         {PaperKind: undefined}),
        AutoFitHeaderSize                           : createCommand(CommandIDs.AutoFitHeaderSize.id,                {Index: undefined, IsColumn: undefined}),
        Paste                                       : createCommand(CommandIDs.Paste.id,                            {PasteValue: undefined, BufferId: undefined}),
        //client
        FullScreen                                  : createClientCommand(CommandIDs.FullScreen),
        //virtual
        LoadInvisibleTilesForFullScreenMode         : createCommand(CommandIDs.LoadInvisibleTiles.id,               {FullScreenMode: undefined}),
		FindAll                                     : createCustomProcessedCommand(CommandIDs.FindAll.id,           
            {
                    FindWhat: undefined, 
                    MatchCase: undefined, 
                    MatchEntireCellContent: undefined, 
                    SearchBy:   undefined, 
                    LookIn:     undefined, 
                    CustomResultProcessingFunction: "OnFindAllCommandResultProcessing"
            }),
        ScrollTo                                    : createCommand(CommandIDs.ScrollTo.id,                         {CellPositionColumn:undefined, CellPositionRow:undefined, SelectAfterScroll:undefined}),

        InsertTable                                 : createCommand(CommandIDs.InsertTable.id,                      {SelectedRange: undefined, HasHeaders: undefined }),
        InsertTableWithStyle                        : createCommand(CommandIDs.InsertTableWithStyle.id,             {SelectedRange: undefined, HasHeaders: undefined, TableStyle: undefined }),
        FetchAutoFilterViewModel                    : createCustomProcessedCommand(CommandIDs.FetchAutoFilterViewModel.id,
            {
                filterCommandId: undefined,
                CustomResultProcessingFunction: "OnAutoFilterViewModelReceived"
            }),

        ApplyAutoFilter                             : createCommand(CommandIDs.ApplyAutoFilter.id,
            {
                filterCommandId: undefined,
                ViewModel: undefined
            }),

        FetchDataValidationViewModel                : createCustomProcessedCommand(CommandIDs.FetchDataValidationViewModel.id,
            {
                CustomResultProcessingFunction: "OnDataValidationViewModelReceived"
            }),
        ApplyDataValidation                         : createCommand(CommandIDs.ApplyDataValidation.id,              { ViewModel: undefined }),
        FetchListAllowedValues                      : createCustomProcessedCommand(CommandIDs.FetchListAllowedValues.id,     
            {
                CustomResultProcessingFunction: "OnListAllowedValuesReceived"
            }),
        DataToolsCircleInvalidData                  : createCommand(CommandIDs.DataToolsCircleInvalidData.id),
        DataToolsClearValidationCircles             : createCommand(CommandIDs.DataToolsClearValidationCircles.id),
        MoveOrCopySheet                             : createCommand(CommandIDs.MoveOrCopySheet.id,                  {BeforeVisibleSheetIndex: undefined, CreateCopy: undefined}),
        FetchMessageForCell                         : createCustomProcessedCommand(CommandIDs.FetchMessageForCell.id,     
            {
                CustomResultProcessingFunction: "OnMessageForCellReceived"
            }),
        TableToolsRenameTable                       : createCommand(CommandIDs.TableToolsRenameTable.id,            { TableName: undefined }),
        ModifyTableStyle                            : createCommand(CommandIDs.ModifyTableStyle.id,                 { StyleName: undefined }),
        FormatAsTable                               : createCommand(CommandIDs.FormatAsTable.id,                    { StyleName: undefined }),
        RemoveSheet                                 : createDefaultParamsCommand(CommandIDs.RemoveSheet.id,         { ForceRemove: false }),
        PageSetup                                   : createCommand(CommandIDs.PageSetup.id,                        { ViewModel: undefined }),
        FetchPageSetupViewModel                     : createCustomProcessedCommand(CommandIDs.FetchPageSetupViewModel.id,
            {
                CustomResultProcessingFunction: "OnPageSetupViewModelReceived"
            }),
        ApplyPageSetupSettings                      : createCommand(CommandIDs.ApplyPageSetupSettings.id,           { pageSetupViewModel: undefined }),
        MoveRange                                   : createCommand(CommandIDs.MoveRange.id,                        { Target: undefined })
    };

    // TODO remove IIFE?
    (function ensureCommands(commands, commandIDs){
        for (var commandID in CommandIDs) {
            if(!commands[commandID])
                commands[commandID] = createCommand(CommandIDs[commandID].id);
        }
    })(commands, CommandIDs);

    var commandsByID;
    function getCommandsByID() {
        if(!commandsByID) {
            commandsByID = {};
            for (var commandID in CommandIDs) {
                commandsByID[CommandIDs[commandID].id] = commandID;
            }
        }
        return commandsByID;
    }

    function createDefaultServerCommandIfCan(commandName) {
        if(CommandIDs[commandName]) {
            var command = createCommand(CommandIDs[commandName]);
            commands[commandName] = command;
            return command;
        }
    }

    commands.getCommandByID = function(commandID){
        if(commands.isFormulaCommand(commandID))
            commandID = CommandIDs.FunctionsInsertSpecificFunction.id;
        else if(commands.isBorderLineStyleCommand(commandID))
            commandID = CommandIDs.FormatBorderLineStyle.id;
        else if(commands.isSetPaperKindCommand(commandID))
            commandID = CommandIDs.SetPaperKind.id;

        var commandName = getCommandsByID()[commandID];
        return this.getCommandByName(commandName);
    };

    commands.getCommandConfigByID = function(commandID) {
        var commandName = getCommandsByID()[commandID];
        return CommandIDs[commandName];
    };

    commands.isFormulaCommand = function(commandID) {
        if (commandID.indexOf(commandSeparator) > 0)
            commandID = commandID.split(commandSeparator)[0]; 
        return commandID == CommandIDs.FunctionsInsertSpecificFunction.id;
    };

    commands.isCellUpdateCommand = function(commandID) {
		return commandID == CommandIDs.UpdateCell.id;
    };

    commands.isBorderLineStyleCommand = function(commandID) {
         if (commandID.indexOf(commandSeparator) > 0)
            commandID = commandID.split(commandSeparator)[0]; 
        return commandID == CommandIDs.FormatBorderLineStyle.id;
    };

    commands.isSetPaperKindCommand = function(commandID) {
         if (commandID.indexOf(commandSeparator) > 0)
            commandID = commandID.split(commandSeparator)[0]; 
        return commandID == CommandIDs.SetPaperKind.id;
    };

    commands.getCommandIDByName = function(commandName){
        return CommandIDs[commandName];
    };

    commands.getCommandByName = function(commandName){
        var command = commands[commandName];
        if(command)
            return command;
        command = createDefaultServerCommandIfCan(commandName);
        return command;
    };

    commands.isUIBlockingCommand = function(commandId) {
        var uiBlockingCommands = this.getUIBlockingCommandsIDs();
        return !!uiBlockingCommands[commandId];
    };


    commands.isCommandCanRequireConfirm = function(commandId) {
        return CommandIDs.FileNew.id == commandId || CommandIDs.FileOpen.id == commandId;
    };

    commands.commandChangesDocument = function(commandId) {
        var commandDoesntChangeDocument = this.getWorkbookNotChangingCommandsIDs()[commandId];
        return !commandDoesntChangeDocument;
    };

    commands.isCommandDisableInReadOnlyMode = function(commandId) {
        var readOnlyEnabledCommands = this.getReadOnlyModeEnabledCommandsIDs();
        return !readOnlyEnabledCommands[commandId];
    };
    commands.isCommandDisabledForLockedSheet = function(commandId) {
        var sheetLockedEnabledCommandIDs = this.getLockedSheetEnabledCommandsIDs();
        return !sheetLockedEnabledCommandIDs[commandId];
    };
    commands.isCommandDisabledForLockedCell = function(commandId) {
        var cellLockedEnabledCommandIDs = this.getLockedCellEnabledCommandsIDs();
        return !cellLockedEnabledCommandIDs[commandId];
    };
    commands.isCommandDisabledForLockedWorkbook = function(commandId) {
        var workbookLockedEnabledCommandIDs = this.getLockedWorkbookEnabledCommandsIDs();
        return !workbookLockedEnabledCommandIDs[commandId];
    };
    

    

    var editModeDisabledCommandsIDs,
        uiBlockingCommandsIDs,
        readOnlyModeEnabledCommandsIDs,
        sheetLockedEnabledCommandIDs,
        cellLockedEnabledCommandIDs,
        workbookLockedEnabledCommandIDs,
        workbookNotChangingCommandsIDs,
        tableCommandsIDs,
        pictureCommandIDs,
        chartCommandIDs;

    commands.getEditModeDisabledCommandsIDs = function() {
        if(!editModeDisabledCommandsIDs)
            fillCommandsArrays();
        return editModeDisabledCommandsIDs;
    };

    commands.getUIBlockingCommandsIDs = function() {
        if(!uiBlockingCommandsIDs)
            fillCommandsArrays();
        return uiBlockingCommandsIDs;
    };

    commands.getReadOnlyModeEnabledCommandsIDs = function() {
        if(!readOnlyModeEnabledCommandsIDs)
           fillCommandsArrays();  
        return readOnlyModeEnabledCommandsIDs;
    };
    commands.getLockedSheetEnabledCommandsIDs = function() {
        if(!sheetLockedEnabledCommandIDs)
           fillCommandsArrays();  
        return sheetLockedEnabledCommandIDs;
    };
    commands.getLockedCellEnabledCommandsIDs = function() {
        if(!cellLockedEnabledCommandIDs)
           fillCommandsArrays();  
        return cellLockedEnabledCommandIDs;
    };
    commands.getLockedWorkbookEnabledCommandsIDs = function() {
        if(!workbookLockedEnabledCommandIDs)
           fillCommandsArrays();  
        return workbookLockedEnabledCommandIDs;
    };

    commands.getWorkbookNotChangingCommandsIDs = function() {
        if(!workbookNotChangingCommandsIDs) {
            workbookNotChangingCommandsIDs = {};
            var ids = [CommandIDs.LoadSheet.id,
	            CommandIDs.LoadInvisibleTiles.id,
	            CommandIDs.FileSave.id,
	            CommandIDs.FileSaveAs.id,
	            CommandIDs.FileOpen.id,
	            CommandIDs.GetLocalizedStringConstant.id,
	            CommandIDs.DownLoadCopy.id,
	            CommandIDs.Print.id,
	            CommandIDs.FindAll.id,
	            CommandIDs.ScrollTo.id,
	            CommandIDs.GetPictureContent.id,
	            CommandIDs.LoadInvisibleTilesForFullScreenMode.id,
	            CommandIDs.FullScreen.id,
	            CommandIDs.CopySelection.id,
                CommandIDs.FetchAutoFilterViewModel.id,
                CommandIDs.FetchDataValidationViewModel.id,
                CommandIDs.FetchListAllowedValues.id,
                CommandIDs.DataToolsCircleInvalidData.id,
                CommandIDs.DataToolsClearValidationCircles.id,
                CommandIDs.FetchMessageForCell.id];
            for(var i in ids)
                workbookNotChangingCommandsIDs[ids[i]] = true;
        }
        return workbookNotChangingCommandsIDs;
    };

    commands.getTableCommandsIDs = function() {
        if(!tableCommandsIDs)
            fillCommandsArrays();
        return tableCommandsIDs;
    };

    commands.getPictureCommandsIDs = function() {
        if(!pictureCommandIDs)
            fillCommandsArrays();
        return pictureCommandIDs;
    };

    commands.getChartCommandsIDs = function() {
        if(!chartCommandIDs)
            fillCommandsArrays();
        return chartCommandIDs;
    };

    function fillCommandsArrays() {
        uiBlockingCommandsIDs = {};
        readOnlyModeEnabledCommandsIDs = {};
        editModeDisabledCommandsIDs = {};
        sheetLockedEnabledCommandIDs = {};
        cellLockedEnabledCommandIDs = {};
        workbookLockedEnabledCommandIDs = {};
        tableCommandsIDs = {};
        pictureCommandIDs = {};
        chartCommandIDs = {};

        for (var commandID in CommandIDs) {
            var command = CommandIDs[commandID];
            if(command.enabled && command.enabled.readonlyMode) 
                readOnlyModeEnabledCommandsIDs[command.id] = commandID;
            if(command.enabled && command.enabled.lockedSheet) 
                sheetLockedEnabledCommandIDs[command.id] = commandID;
            if(!command.disabled || !command.disabled.lockedCell) 
                cellLockedEnabledCommandIDs[command.id] = commandID;
            if(!command.disabled || !command.disabled.lockedWorkbook) 
                workbookLockedEnabledCommandIDs[command.id] = commandID;
            if(command.UIBlocking) 
                uiBlockingCommandsIDs[command.id] = commandID;
            if(!command.enabled || !command.enabled.editMode) 
                editModeDisabledCommandsIDs[command.id] = commandID;
            if(command.isTableCommand)
                tableCommandsIDs[command.id] = commandID;
            if(command.isPictureCommand)
                pictureCommandIDs[command.id] = commandID;
            if(command.isChartCommand)
                chartCommandIDs[command.id] = commandID;
        }
    }

    return commands;
})();