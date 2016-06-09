#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DevExpress.Utils;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandInfo
	internal class XlsCommandInfo {
		short typeCode;
		Type commandType;
		public XlsCommandInfo(short typeCode, Type commandType) {
			this.typeCode = typeCode;
			this.commandType = commandType;
		}
		public short TypeCode { get { return this.typeCode; } }
		public Type CommandType { get { return this.commandType; } }
	}
	#endregion
	#region XlsCommandFactory
	public static class XlsCommandFactory {
		const int minRecordSize = 4; 
		static List<XlsCommandInfo> infos;
		static Dictionary<Type, short> typeCodes;
		[ThreadStatic]
		static Dictionary<short, IXlsCommand> commandInstances;
		static XlsCommandFactory() {
			infos = new List<XlsCommandInfo>();
			typeCodes = new Dictionary<Type, short>();
			AddCommandInfo(new XlsCommandInfo(0x0000, typeof(XlsCommandEmpty)));
			AddCommandInfo(new XlsCommandInfo(0x0809, typeof(XlsCommandBeginOfSubstream)));
			AddCommandInfo(new XlsCommandInfo(0x000a, typeof(XlsCommandEndOfSubstream)));
			AddCommandInfo(new XlsCommandInfo(0x002f, typeof(XlsCommandFilePassword)));
			AddCommandInfo(new XlsCommandInfo(0x00e1, typeof(XlsCommandInterfaceHeader)));
			AddCommandInfo(new XlsCommandInfo(0x00c1, typeof(XlsCommandAddDelMenuItems)));
			AddCommandInfo(new XlsCommandInfo(0x00e2, typeof(XlsCommandInterfaceEnd)));
			AddCommandInfo(new XlsCommandInfo(0x0042, typeof(XlsCommandEncoding)));
			AddCommandInfo(new XlsCommandInfo(0x013d, typeof(XlsCommandSheetIdTable)));
			AddCommandInfo(new XlsCommandInfo(0x009c, typeof(XlsCommandBuiltInFunctionGroupCount)));
			AddCommandInfo(new XlsCommandInfo(0x00de, typeof(XlsCommandOleObjectSize)));
			AddCommandInfo(new XlsCommandInfo(0x0019, typeof(XlsCommandWindowsProtected)));
			AddCommandInfo(new XlsCommandInfo(0x0012, typeof(XlsCommandProtected)));
			AddCommandInfo(new XlsCommandInfo(0x0013, typeof(XlsCommandPasswordVerifier)));
			AddCommandInfo(new XlsCommandInfo(0x01af, typeof(XlsCommandProtectForRevisions)));
			AddCommandInfo(new XlsCommandInfo(0x01bc, typeof(XlsCommandProtectForRevisionsPasswordVerifier)));
			AddCommandInfo(new XlsCommandInfo(0x003d, typeof(XlsCommandWorkbookWindowInformation)));
			AddCommandInfo(new XlsCommandInfo(0x0040, typeof(XlsCommandShouldSaveBackup)));
			AddCommandInfo(new XlsCommandInfo(0x008d, typeof(XlsCommandDisplayObjectsOptions)));
			AddCommandInfo(new XlsCommandInfo(0x0022, typeof(XlsCommandIs1904DateSystemUsed)));
			AddCommandInfo(new XlsCommandInfo(0x000e, typeof(XlsCommandPrecisionAsDisplayed)));
			AddCommandInfo(new XlsCommandInfo(0x01b7, typeof(XlsCommandRefreshAllOnLoading)));
			AddCommandInfo(new XlsCommandInfo(0x00da, typeof(XlsCommandWorkbookBoolProperties)));
			AddCommandInfo(new XlsCommandInfo(0x0031, typeof(XlsCommandFont)));
			AddCommandInfo(new XlsCommandInfo(0x041e, typeof(XlsCommandNumberFormat)));
			AddCommandInfo(new XlsCommandInfo(0x00e0, typeof(XlsCommandFormat)));
			AddCommandInfo(new XlsCommandInfo(0x087c, typeof(XlsCommandFormatCrc)));
			AddCommandInfo(new XlsCommandInfo(0x087d, typeof(XlsCommandFormatExt)));
			AddCommandInfo(new XlsCommandInfo(0x088d, typeof(XlsCommandDifferentialFormat)));
			AddCommandInfo(new XlsCommandInfo(0x088e, typeof(XlsCommandTableStyles)));
			AddCommandInfo(new XlsCommandInfo(0x088f, typeof(XlsCommandTableStyle)));
			AddCommandInfo(new XlsCommandInfo(0x0890, typeof(XlsCommandTableStyleElement)));
			AddCommandInfo(new XlsCommandInfo(0x0293, typeof(XlsCommandCellStyle)));
			AddCommandInfo(new XlsCommandInfo(0x0892, typeof(XlsCommandCellStyleExt)));
			AddCommandInfo(new XlsCommandInfo(0x0160, typeof(XlsCommandSupportNaturalLanguagesFormulaInput)));
			AddCommandInfo(new XlsCommandInfo(0x0085, typeof(XlsCommandSheetInformation)));
			AddCommandInfo(new XlsCommandInfo(0x01c1, typeof(XlsCommandRecalcInformation)));
			AddCommandInfo(new XlsCommandInfo(0x005c, typeof(XlsCommandWriteAccess)));
			AddCommandInfo(new XlsCommandInfo(0x0161, typeof(XlsCommandDSF)));
			AddCommandInfo(new XlsCommandInfo(0x01c0, typeof(XlsCommandExcel9File)));
			AddCommandInfo(new XlsCommandInfo(0x008c, typeof(XlsCommandCountry)));
			AddCommandInfo(new XlsCommandInfo(0x00fc, typeof(XlsCommandSharedStrings)));
			AddCommandInfo(new XlsCommandInfo(0x003c, typeof(XlsCommandContinue)));
			AddCommandInfo(new XlsCommandInfo(0x00ff, typeof(XlsCommandExtendedSST)));
			AddCommandInfo(new XlsCommandInfo(0x0092, typeof(XlsCommandPalette)));
			AddCommandInfo(new XlsCommandInfo(0x01ae, typeof(XlsCommandSupBook)));
			AddCommandInfo(new XlsCommandInfo(0x0023, typeof(XlsCommandExternName)));
			AddCommandInfo(new XlsCommandInfo(0x0059, typeof(XlsCommandExternCacheStart)));
			AddCommandInfo(new XlsCommandInfo(0x005a, typeof(XlsCommandExternCacheItem)));
			AddCommandInfo(new XlsCommandInfo(0x0017, typeof(XlsCommandExternSheet)));
			AddCommandInfo(new XlsCommandInfo(0x0018, typeof(XlsCommandDefinedName)));
			AddCommandInfo(new XlsCommandInfo(0x0894, typeof(XlsCommandDefinedNameComment)));
			AddCommandInfo(new XlsCommandInfo(0x00eb, typeof(XlsCommandMsoDrawingGroup)));
			AddCommandInfo(new XlsCommandInfo(0x0896, typeof(XlsCommandTheme)));
			AddCommandInfo(new XlsCommandInfo(0x0812, typeof(XlsCommandContinueFrt)));
			AddCommandInfo(new XlsCommandInfo(0x0875, typeof(XlsCommandContinueFrt11)));
			AddCommandInfo(new XlsCommandInfo(0x087f, typeof(XlsCommandContinueFrt12)));
			AddCommandInfo(new XlsCommandInfo(0x00d3, typeof(XlsCommandVBAProject)));
			AddCommandInfo(new XlsCommandInfo(0x01ba, typeof(XlsCommandCodeName)));
			AddCommandInfo(new XlsCommandInfo(0x01bd, typeof(XlsCommandVBAProjectHasNoMacros)));
			AddCommandInfo(new XlsCommandInfo(0x0813, typeof(XlsCommandRealTimeData)));
			AddCommandInfo(new XlsCommandInfo(0x0863, typeof(XlsCommandWorkbookExtendedProperties)));
			AddCommandInfo(new XlsCommandInfo(0x0060, typeof(XlsCommandTemplate)));
			AddCommandInfo(new XlsCommandInfo(0x00d5, typeof(XlsCommandPivotCacheStreamId)));
			AddCommandInfo(new XlsCommandInfo(0x00e3, typeof(XlsCommandPivotCacheType)));
			AddCommandInfo(new XlsCommandInfo(0x0051, typeof(XlsCommandDataConsolidationReference)));
			AddCommandInfo(new XlsCommandInfo(0x0052, typeof(XlsCommandDataConsolidationName)));
			AddCommandInfo(new XlsCommandInfo(0x01b5, typeof(XlsCommandDataConsolidationBuiltInName)));
			AddCommandInfo(new XlsCommandInfo(0x00d0, typeof(XlsCommandPivotCacheMultiRange)));
			AddCommandInfo(new XlsCommandInfo(0x00d2, typeof(XlsCommandPivotCacheMultiRangeMap)));
			AddCommandInfo(new XlsCommandInfo(0x00d1, typeof(XlsCommandPivotCacheStringItems)));
			AddCommandInfo(new XlsCommandInfo(0x00cd, typeof(XlsCommandPivotCacheStringSegment)));
			AddCommandInfo(new XlsCommandInfo(0x00dc, typeof(XlsCommandDbOrParamQuery)));
			AddCommandInfo(new XlsCommandInfo(0x0864, typeof(XlsCommandPivotAddl)));
			AddCommandInfo(new XlsCommandInfo(0x020b, typeof(XlsCommandIndex)));
			AddCommandInfo(new XlsCommandInfo(0x000d, typeof(XlsCommandCalculationMode)));
			AddCommandInfo(new XlsCommandInfo(0x000c, typeof(XlsCommandIterationCount)));
			AddCommandInfo(new XlsCommandInfo(0x000f, typeof(XlsCommandReferenceMode)));
			AddCommandInfo(new XlsCommandInfo(0x0011, typeof(XlsCommandIterationsEnabled)));
			AddCommandInfo(new XlsCommandInfo(0x0010, typeof(XlsCommandCalculationDelta)));
			AddCommandInfo(new XlsCommandInfo(0x005f, typeof(XlsCommandRecalculateBeforeSaved)));
			AddCommandInfo(new XlsCommandInfo(0x002a, typeof(XlsCommandPrintRowColHeadings)));
			AddCommandInfo(new XlsCommandInfo(0x002b, typeof(XlsCommandPrintGridLines)));
			AddCommandInfo(new XlsCommandInfo(0x0082, typeof(XlsCommandPrintGridLinesSet)));
			AddCommandInfo(new XlsCommandInfo(0x0080, typeof(XlsCommandGuts)));
			AddCommandInfo(new XlsCommandInfo(0x0225, typeof(XlsCommandDefaultRowHeight)));
			AddCommandInfo(new XlsCommandInfo(0x0081, typeof(XlsCommandAdditionalWorksheetInformation)));
			AddCommandInfo(new XlsCommandInfo(0x001b, typeof(XlsCommandHorizontalPageBreaks)));
			AddCommandInfo(new XlsCommandInfo(0x001a, typeof(XlsCommandVerticalPageBreaks)));
			AddCommandInfo(new XlsCommandInfo(0x0014, typeof(XlsCommandPageHeader)));
			AddCommandInfo(new XlsCommandInfo(0x0015, typeof(XlsCommandPageFooter)));
			AddCommandInfo(new XlsCommandInfo(0x0083, typeof(XlsCommandPageHCenter)));
			AddCommandInfo(new XlsCommandInfo(0x0084, typeof(XlsCommandPageVCenter)));
			AddCommandInfo(new XlsCommandInfo(0x0026, typeof(XlsCommandPageLeftMargin)));
			AddCommandInfo(new XlsCommandInfo(0x0027, typeof(XlsCommandPageRightMargin)));
			AddCommandInfo(new XlsCommandInfo(0x0028, typeof(XlsCommandPageTopMargin)));
			AddCommandInfo(new XlsCommandInfo(0x0029, typeof(XlsCommandPageBottomMargin)));
			AddCommandInfo(new XlsCommandInfo(0x00a1, typeof(XlsCommandPageSetup)));
			AddCommandInfo(new XlsCommandInfo(0x089c, typeof(XlsCommandHeaderFooter)));
			AddCommandInfo(new XlsCommandInfo(0x00dd, typeof(XlsCommandScenarioProtected)));
			AddCommandInfo(new XlsCommandInfo(0x0063, typeof(XlsCommandObjectsProtected)));
			AddCommandInfo(new XlsCommandInfo(0x0055, typeof(XlsCommandDefaultColumnWidth)));
			AddCommandInfo(new XlsCommandInfo(0x007d, typeof(XlsCommandColumnInfo)));
			AddCommandInfo(new XlsCommandInfo(0x0200, typeof(XlsCommandDimensions)));
			AddCommandInfo(new XlsCommandInfo(0x0208, typeof(XlsCommandRow)));
			AddCommandInfo(new XlsCommandInfo(0x0006, typeof(XlsCommandFormula)));
			AddCommandInfo(new XlsCommandInfo(0x0221, typeof(XlsCommandArrayFormula)));
			AddCommandInfo(new XlsCommandInfo(0x0236, typeof(XlsCommandTable)));
			AddCommandInfo(new XlsCommandInfo(0x04bc, typeof(XlsCommandSharedFormula)));
			AddCommandInfo(new XlsCommandInfo(0x0207, typeof(XlsCommandString)));
			AddCommandInfo(new XlsCommandInfo(0x0201, typeof(XlsCommandBlank)));
			AddCommandInfo(new XlsCommandInfo(0x00be, typeof(XlsCommandMulBlank)));
			AddCommandInfo(new XlsCommandInfo(0x0205, typeof(XlsCommandBoolErr)));
			AddCommandInfo(new XlsCommandInfo(0x0203, typeof(XlsCommandNumber)));
			AddCommandInfo(new XlsCommandInfo(0x0204, typeof(XlsCommandLabel)));
			AddCommandInfo(new XlsCommandInfo(0x00fd, typeof(XlsCommandLabelSst)));
			AddCommandInfo(new XlsCommandInfo(0x00d7, typeof(XlsCommandDbCell)));
			AddCommandInfo(new XlsCommandInfo(0x00bd, typeof(XlsCommandMulRk)));
			AddCommandInfo(new XlsCommandInfo(0x027e, typeof(XlsCommandRk)));
			AddCommandInfo(new XlsCommandInfo(0x00ec, typeof(XlsCommandMsoDrawing)));
			AddCommandInfo(new XlsCommandInfo(0x005d, typeof(XlsCommandObject)));
			AddCommandInfo(new XlsCommandInfo(0x01b6, typeof(XlsCommandTextObject)));
			AddCommandInfo(new XlsCommandInfo(0x001c, typeof(XlsCommandNote)));
			AddCommandInfo(new XlsCommandInfo(0x023e, typeof(XlsCommandSheetViewInformation)));
			AddCommandInfo(new XlsCommandInfo(0x088b, typeof(XlsCommandPageLayoutView)));
			AddCommandInfo(new XlsCommandInfo(0x00a0, typeof(XlsCommandSheetViewScale)));
			AddCommandInfo(new XlsCommandInfo(0x0041, typeof(XlsCommandPane)));
			AddCommandInfo(new XlsCommandInfo(0x001d, typeof(XlsCommandSelection)));
			AddCommandInfo(new XlsCommandInfo(0x01aa, typeof(XlsCommandCustomViewBegin)));
			AddCommandInfo(new XlsCommandInfo(0x01ab, typeof(XlsCommandCustomViewEnd)));
			AddCommandInfo(new XlsCommandInfo(0x00e5, typeof(XlsCommandMergeCells)));
			AddCommandInfo(new XlsCommandInfo(0x01b0, typeof(XlsCommandConditionalFormat)));
			AddCommandInfo(new XlsCommandInfo(0x01b1, typeof(XlsCommandCF)));
			AddCommandInfo(new XlsCommandInfo(0x0879, typeof(XlsCommandConditionalFormat12)));
			AddCommandInfo(new XlsCommandInfo(0x087a, typeof(XlsCommandCF12)));
			AddCommandInfo(new XlsCommandInfo(0x087b, typeof(XlsCommandCFEx)));
			AddCommandInfo(new XlsCommandInfo(0x01b8, typeof(XlsCommandHyperlink)));
			AddCommandInfo(new XlsCommandInfo(0x0800, typeof(XlsCommandHyperlinkTooltip)));
			AddCommandInfo(new XlsCommandInfo(0x0867, typeof(XlsCommandSharedFeatureHeader)));
			AddCommandInfo(new XlsCommandInfo(0x0868, typeof(XlsCommandSharedFeature)));
			AddCommandInfo(new XlsCommandInfo(0x0871, typeof(XlsCommandSharedFeatureHeader11)));
			AddCommandInfo(new XlsCommandInfo(0x0872, typeof(XlsCommandSharedFeature11)));
			AddCommandInfo(new XlsCommandInfo(0x0878, typeof(XlsCommandSharedFeature12)));
			AddCommandInfo(new XlsCommandInfo(0x0877, typeof(XlsCommandList12)));
			AddCommandInfo(new XlsCommandInfo(0x01b2, typeof(XlsCommandDataValidations)));
			AddCommandInfo(new XlsCommandInfo(0x01be, typeof(XlsCommandDataValidation)));
			AddCommandInfo(new XlsCommandInfo(0x009b, typeof(XlsCommandFilterMode)));
			AddCommandInfo(new XlsCommandInfo(0x009d, typeof(XlsCommandAutoFilterInfo)));
			AddCommandInfo(new XlsCommandInfo(0x009e, typeof(XlsCommandAutoFilter)));
			AddCommandInfo(new XlsCommandInfo(0x087e, typeof(XlsCommandAutoFilter12)));
			AddCommandInfo(new XlsCommandInfo(0x0895, typeof(XlsCommandSortData)));
			AddCommandInfo(new XlsCommandInfo(0x00b0, typeof(XlsCommandPivotView)));
			AddCommandInfo(new XlsCommandInfo(0x00b1, typeof(XlsCommandPivotField)));
			AddCommandInfo(new XlsCommandInfo(0x00b2, typeof(XlsCommandPivotItem)));
			AddCommandInfo(new XlsCommandInfo(0x0100, typeof(XlsCommandPivotFieldExt)));
			AddCommandInfo(new XlsCommandInfo(0x00C5, typeof(XlsCommandPivotViewDataItem)));
			AddCommandInfo(new XlsCommandInfo(0x00B4, typeof(XlsCommandPivotAxis)));
			AddCommandInfo(new XlsCommandInfo(0x00B5, typeof(XlsCommandPivotLines)));
			AddCommandInfo(new XlsCommandInfo(0x00B6, typeof(XlsCommandPivotPageAxis)));
			AddCommandInfo(new XlsCommandInfo(0x00F1, typeof(XlsCommandPivotAdditionalProperties)));
			AddCommandInfo(new XlsCommandInfo(0x00F7, typeof(XlsCommandPivotSelection)));
			AddCommandInfo(new XlsCommandInfo(0x00F0, typeof(XlsCommandPivotRule)));
			AddCommandInfo(new XlsCommandInfo(0x00F2, typeof(XlsCommandPivotFilter)));
			AddCommandInfo(new XlsCommandInfo(0x00F5, typeof(XlsCommandPivotItemReferences)));
			AddCommandInfo(new XlsCommandInfo(0x00FB, typeof(XlsCommandPivotFormat)));
			AddCommandInfo(new XlsCommandInfo(0x00F4, typeof(XlsCommandPivotDifferentialFormat)));
			AddCommandInfo(new XlsCommandInfo(0x0802, typeof(XlsCommandPivotViewExtTag)));
			AddCommandInfo(new XlsCommandInfo(0x080C, typeof(XlsCommandPivotViewExt)));
			AddCommandInfo(new XlsCommandInfo(0x080D, typeof(XlsCommandPivotHierarchy)));
			AddCommandInfo(new XlsCommandInfo(0x080E, typeof(XlsCommandPivotPageAxisExt)));
			AddCommandInfo(new XlsCommandInfo(0x080F, typeof(XlsCommandPivotFieldOLAPExt)));
			AddCommandInfo(new XlsCommandInfo(0x0810, typeof(XlsCommandPivotViewExt9)));
			AddCommandInfo(new XlsCommandInfo(0x0033, typeof(XlsCommandChartPrintSize)));
			AddCommandInfo(new XlsCommandInfo(0x1033, typeof(XlsCommandChartPropertiesBegin)));
			AddCommandInfo(new XlsCommandInfo(0x1034, typeof(XlsCommandChartPropertiesEnd)));
			AddCommandInfo(new XlsCommandInfo(0x0854, typeof(XlsCommandChartStartObject)));
			AddCommandInfo(new XlsCommandInfo(0x0855, typeof(XlsCommandChartEndObject)));
			AddCommandInfo(new XlsCommandInfo(0x1001, typeof(XlsCommandChartUnits)));
			AddCommandInfo(new XlsCommandInfo(0x1002, typeof(XlsCommandChart)));
			AddCommandInfo(new XlsCommandInfo(0x1064, typeof(XlsCommandChartPlotGrowth)));
			AddCommandInfo(new XlsCommandInfo(0x1044, typeof(XlsCommandChartSpaceProperties))); 
			AddCommandInfo(new XlsCommandInfo(0x1035, typeof(XlsCommandChartPlotArea)));
			AddCommandInfo(new XlsCommandInfo(0x1046, typeof(XlsCommandChartAxesUsed)));
			AddCommandInfo(new XlsCommandInfo(0x1041, typeof(XlsCommandChartAxisParent)));
			AddCommandInfo(new XlsCommandInfo(0x101d, typeof(XlsCommandChartAxis)));
			AddCommandInfo(new XlsCommandInfo(0x1020, typeof(XlsCommandChartAxisCatSerRange)));
			AddCommandInfo(new XlsCommandInfo(0x1062, typeof(XlsCommandChartAxisExt)));
			AddCommandInfo(new XlsCommandInfo(0x101f, typeof(XlsCommandChartAxisValueRange)));
			AddCommandInfo(new XlsCommandInfo(0x0856, typeof(XlsCommandChartAxisCatLabels)));
			AddCommandInfo(new XlsCommandInfo(0x104e, typeof(XlsCommandChartAxisNumberFormat)));
			AddCommandInfo(new XlsCommandInfo(0x101e, typeof(XlsCommandChartAxisTick)));
			AddCommandInfo(new XlsCommandInfo(0x1021, typeof(XlsCommandChartAxisLine)));
			AddCommandInfo(new XlsCommandInfo(0x0857, typeof(XlsCommandChartAxisYMult)));
			AddCommandInfo(new XlsCommandInfo(0x1014, typeof(XlsCommandChartViewFormat))); 
			AddCommandInfo(new XlsCommandInfo(0x1017, typeof(XlsCommandChartViewBar)));
			AddCommandInfo(new XlsCommandInfo(0x1018, typeof(XlsCommandChartViewLine)));
			AddCommandInfo(new XlsCommandInfo(0x1019, typeof(XlsCommandChartViewPie)));
			AddCommandInfo(new XlsCommandInfo(0x101a, typeof(XlsCommandChartViewArea)));
			AddCommandInfo(new XlsCommandInfo(0x101b, typeof(XlsCommandChartViewScatter)));
			AddCommandInfo(new XlsCommandInfo(0x103e, typeof(XlsCommandChartViewRadar)));
			AddCommandInfo(new XlsCommandInfo(0x1040, typeof(XlsCommandChartViewRadarArea)));
			AddCommandInfo(new XlsCommandInfo(0x103f, typeof(XlsCommandChartViewSurface)));
			AddCommandInfo(new XlsCommandInfo(0x1061, typeof(XlsCommandChartViewOfPie)));
			AddCommandInfo(new XlsCommandInfo(0x1067, typeof(XlsCommandChartViewOfPieCustom)));
			AddCommandInfo(new XlsCommandInfo(0x103a, typeof(XlsCommandChartView3D)));
			AddCommandInfo(new XlsCommandInfo(0x1022, typeof(XlsCommandCrtLink)));
			AddCommandInfo(new XlsCommandInfo(0x101c, typeof(XlsCommandChartCrtLine)));
			AddCommandInfo(new XlsCommandInfo(0x1003, typeof(XlsCommandChartSeries)));
			AddCommandInfo(new XlsCommandInfo(0x1051, typeof(XlsCommandChartDataRef))); 
			AddCommandInfo(new XlsCommandInfo(0x100d, typeof(XlsCommandChartSeriesText)));
			AddCommandInfo(new XlsCommandInfo(0x1045, typeof(XlsCommandChartSeriesToView)));
			AddCommandInfo(new XlsCommandInfo(0x104a, typeof(XlsCommandChartSeriesParent)));
			AddCommandInfo(new XlsCommandInfo(0x104b, typeof(XlsCommandChartSeriesAuxTrend)));
			AddCommandInfo(new XlsCommandInfo(0x1065, typeof(XlsCommandChartSeriesDataIndex)));
			AddCommandInfo(new XlsCommandInfo(0x1043, typeof(XlsCommandChartLegendException)));
			AddCommandInfo(new XlsCommandInfo(0x1007, typeof(XlsCommandChartLineFormat)));
			AddCommandInfo(new XlsCommandInfo(0x100a, typeof(XlsCommandChartAreaFormat)));
			AddCommandInfo(new XlsCommandInfo(0x1006, typeof(XlsCommandChartDataFormat)));
			AddCommandInfo(new XlsCommandInfo(0x105f, typeof(XlsCommandChart3DBarShape)));
			AddCommandInfo(new XlsCommandInfo(0x100b, typeof(XlsCommandChartPieExplosion)));
			AddCommandInfo(new XlsCommandInfo(0x105d, typeof(XlsCommandChartSeriesFormat)));
			AddCommandInfo(new XlsCommandInfo(0x1009, typeof(XlsCommandChartMarkerFormat)));
			AddCommandInfo(new XlsCommandInfo(0x1015, typeof(XlsCommandChartLegend)));
			AddCommandInfo(new XlsCommandInfo(0x104f, typeof(XlsCommandChartPos)));
			AddCommandInfo(new XlsCommandInfo(0x089d, typeof(XlsCommandChartLayout12)));
			AddCommandInfo(new XlsCommandInfo(0x1032, typeof(XlsCommandChartFrame)));
			AddCommandInfo(new XlsCommandInfo(0x089e, typeof(XlsCommandChartML)));
			AddCommandInfo(new XlsCommandInfo(0x089f, typeof(XlsCommandChartMLContinue)));
			AddCommandInfo(new XlsCommandInfo(0x1025, typeof(XlsCommandChartText)));
			AddCommandInfo(new XlsCommandInfo(0x1027, typeof(XlsCommandChartTextObjectLink)));
			AddCommandInfo(new XlsCommandInfo(0x1050, typeof(XlsCommandChartTextRuns)));
			AddCommandInfo(new XlsCommandInfo(0x1026, typeof(XlsCommandChartFontX)));
			AddCommandInfo(new XlsCommandInfo(0x0851, typeof(XlsCommandFrtWrapper)));
			AddCommandInfo(new XlsCommandInfo(0x100c, typeof(XlsCommandChartAttachedLabel)));
			AddCommandInfo(new XlsCommandInfo(0x086a, typeof(XlsCommandChartDataLabExt)));
			AddCommandInfo(new XlsCommandInfo(0x086b, typeof(XlsCommandChartDataLabelExtContents)));
			AddCommandInfo(new XlsCommandInfo(0x1024, typeof(XlsCommandChartDefaultText)));
			AddCommandInfo(new XlsCommandInfo(0x08a7, typeof(XlsCommandChartLayout12A)));
			AddCommandInfo(new XlsCommandInfo(0x08a4, typeof(XlsCommandChartShapeProperties)));
			AddCommandInfo(new XlsCommandInfo(0x08a5, typeof(XlsCommandChartTextProperties)));
			AddCommandInfo(new XlsCommandInfo(0x1066, typeof(XlsCommandChartGelFrame)));
			AddCommandInfo(new XlsCommandInfo(0x0852, typeof(XlsCommandChartStartBlock)));
			AddCommandInfo(new XlsCommandInfo(0x0853, typeof(XlsCommandChartEndBlock)));
			AddCommandInfo(new XlsCommandInfo(0x085a, typeof(XlsCommandChartFrtFontList)));
			AddCommandInfo(new XlsCommandInfo(0x1060, typeof(XlsCommandChartFbi)));
			AddCommandInfo(new XlsCommandInfo(0x0850, typeof(XlsCommandChartFrtInfo)));
			AddCommandInfo(new XlsCommandInfo(0x00c6, typeof(XlsCommandPivotCacheProperties)));
			AddCommandInfo(new XlsCommandInfo(0x0122, typeof(XlsCommandPivotCachePropertiesExt)));
			AddCommandInfo(new XlsCommandInfo(0x00f9, typeof(XlsCommandPivotCacheFormula)));
			AddCommandInfo(new XlsCommandInfo(0x00f6, typeof(XlsCommandPivotCacheItemName)));
			AddCommandInfo(new XlsCommandInfo(0x00f8, typeof(XlsCommandPivotCacheItemPair)));
			AddCommandInfo(new XlsCommandInfo(0x0103, typeof(XlsCommandPivotCacheCalculatedItem)));
			AddCommandInfo(new XlsCommandInfo(0x00c7, typeof(XlsCommandPivotCacheFieldProperties)));
			AddCommandInfo(new XlsCommandInfo(0x01bb, typeof(XlsCommandPivotCacheFieldDataType)));
			AddCommandInfo(new XlsCommandInfo(0x00cf, typeof(XlsCommandPivotCacheValueNil)));
			AddCommandInfo(new XlsCommandInfo(0x00c9, typeof(XlsCommandPivotCacheValueNum)));
			AddCommandInfo(new XlsCommandInfo(0x00ca, typeof(XlsCommandPivotCacheValueBool)));
			AddCommandInfo(new XlsCommandInfo(0x00cb, typeof(XlsCommandPivotCacheValueErr)));
			AddCommandInfo(new XlsCommandInfo(0x00ce, typeof(XlsCommandPivotCacheValueDateTime)));
			AddCommandInfo(new XlsCommandInfo(0x00cc, typeof(XlsCommandPivotCacheValueInt)));
			AddCommandInfo(new XlsCommandInfo(0x00d8, typeof(XlsCommandPivotCacheRangeGroup)));
			AddCommandInfo(new XlsCommandInfo(0x00d9, typeof(XlsCommandPivotCacheMapping)));
			AddCommandInfo(new XlsCommandInfo(0x00c8, typeof(XlsCommandPivotCacheItemIndexes)));
		}
		static void AddCommandInfo(XlsCommandInfo info) {
			infos.Add(info);
			typeCodes.Add(info.CommandType, info.TypeCode);
		}
		static Dictionary<short, IXlsCommand> CommandInstances {
			get {
				if (commandInstances == null) {
					commandInstances = new Dictionary<short, IXlsCommand>();
					PopulateCommandInstances(CommandInstances, infos);
				}
				return commandInstances;
			}
		}
		static void PopulateCommandInstances(Dictionary<short, IXlsCommand> instances, List<XlsCommandInfo> infos) {
			for (int i = 0; i < infos.Count; i++) {
				ConstructorInfo commandConstructor = infos[i].CommandType.GetConstructor(new Type[] { });
				IXlsCommand commandInstance = commandConstructor.Invoke(new object[] { }) as IXlsCommand;
				instances.Add(infos[i].TypeCode, commandInstance);
			}
		}
		public static short GetTypeCodeByType(Type commandType) {
			short typeCode;
			if (!typeCodes.TryGetValue(commandType, out typeCode))
				typeCode = 0x0000;
			return typeCode;
		}
		public static IXlsCommand CreateCommand(XlsReader reader) {
			long bytesToRead = reader.StreamLength - reader.Position;
			if(bytesToRead < minRecordSize) {
				reader.Seek(bytesToRead, SeekOrigin.Current);
				return null;
			}
			short typeCode = reader.ReadNotCryptedInt16();
			if (!CommandInstances.ContainsKey(typeCode))
				typeCode = 0x0000;
			IXlsCommand command = CommandInstances[typeCode];
			return command.GetInstance();
		}
	}
	#endregion
}
