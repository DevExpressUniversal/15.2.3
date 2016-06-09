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
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System.Xml;
using DevExpress.Office;
using DevExpress.Utils;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region WorksheetDestination
	public class WorksheetDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("sheetPr", OnSheetProperties);
			result.Add("sheetViews", OnSheetViews);
			result.Add("sheetFormatPr", OnSheetFormatProperties);
			result.Add("cols", OnColumns);
			result.Add("sheetData", OnSheetData);
			result.Add("sheetProtection", OnSheetProtection);
			result.Add("protectedRanges", OnProtectedRanges);
			result.Add("autoFilter", OnAutoFilter);
			result.Add("mergeCells", OnMergeCells);
			result.Add("conditionalFormatting", OnConditionalFormatting);
			result.Add("dataValidations", OnDataValidations);
			result.Add("hyperlinks", OnHyperlinks);
			result.Add("pageMargins", OnMargins);
			result.Add("pageSetup", OnPageSetup);
			result.Add("rowBreaks", OnRowBreaks);
			result.Add("colBreaks", OnColumnBreaks);
			result.Add("ignoredErrors", OnIgnoredErrors);
			result.Add("drawing", OnDrawing);
			result.Add("legacyDrawing", OnVmlDrawingRelation);
			result.Add("tableParts", OnTablesParts);
			result.Add("extLst", OnExtensionList);
			result.Add("headerFooter", OnHeaderFooter);
			result.Add("printOptions", OnPrintOptions);
			return result;
		}
		#region Fields
		readonly Worksheet previousWorksheet;
		#endregion
		public WorksheetDestination(SpreadsheetMLBaseImporter importer, Worksheet worksheet)
			: base(importer) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			this.previousWorksheet = Importer.CurrentWorksheet;
			Importer.CurrentWorksheet = worksheet;
			Importer.SharedFormulaIds.Clear();
			Importer.ImportWorksheetRelations();
		}
		#region Properties
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			IList<CellRange> selectedRanges = Importer.CurrentWorksheet.Selection.SelectedRanges;
			bool shouldExpand = false;
			foreach (CellRange range in selectedRanges) {
				if (range.RangeType != CellRangeType.IntervalRange) {
					shouldExpand = true;
					break;
				}
			}
			if(shouldExpand)
				Importer.CurrentWorksheet.Selection.ExpandToMergedCells();
			Importer.CurrentWorksheet = previousWorksheet;
		}
		static Destination OnSheetViews(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new SheetViewsDestination(importer);
		}
		static Destination OnSheetData(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new SheetDataDestination(importer);
		}
		static Destination OnColumns(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new SheetColumnsDestination(importer);
		}
		static Destination OnMergeCells(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new MergeCellsDestination(importer);
		}
		static Destination OnHyperlinks(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new HyperlinksDestination(importer);
		}
		static Destination OnTablesParts(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new TableReferencesDestination(importer);
		}
		static Destination OnDrawing(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new WorksheetDrawingDestination(importer);
		}
		static Destination OnMargins(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new SheetMarginsDestination(importer, importer.CurrentWorksheet.Margins);
		}
		static Destination OnPageSetup(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PageSetupDestination(importer, importer.CurrentWorksheet.PrintSetup);
		}
		static Destination OnColumnBreaks(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PageBreaksDestination(importer, importer.CurrentWorksheet.ColumnBreaks);
		}
		static Destination OnRowBreaks(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PageBreaksDestination(importer, importer.CurrentWorksheet.RowBreaks);
		}
		static Destination OnSheetProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new WorksheetPropertiesDestination(importer);
		}
		static Destination OnSheetFormatProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new WorksheetFormatPropertiesDestination(importer);
		}
		static Destination OnVmlDrawingRelation(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new VmlDrawingRelationDestination(importer);
		}
		static Destination OnSheetProtection(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new SheetProtectionDestination(importer);
		}
		static Destination OnProtectedRanges(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ProtectedRangesDestination(importer);
		}
		static Destination OnDataValidations(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DataValidationsDestination(importer);
		}
		static Destination OnConditionalFormatting(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ConditionalFormattingDestination(importer);
		}
		static Destination OnAutoFilter(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			Worksheet currentWorksheet = importer.CurrentWorksheet;
			return new AutoFilterDestination(importer, currentWorksheet.AutoFilter, currentWorksheet);
		}
		static Destination OnExtensionList(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new WorksheetExtensionListDestination(importer);
		}
		static Destination OnHeaderFooter(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new HeaderFooterDestination(importer, importer.CurrentWorksheet.Properties.HeaderFooter);
		}
		static Destination OnPrintOptions(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PrintOptionsDestination(importer, importer.CurrentWorksheet.PrintSetup);
		}
		static Destination OnIgnoredErrors(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new IgnoredErrorsDestination(importer);
		}
	}
	#endregion
	#region SheetDataDestination
	public class SheetDataDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("row", OnRow);
			return result;
		}
		public SheetDataDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnRow(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new RowDestination(importer);
		}
	}
	#endregion
	#region WorksheetFormatPropertiesDestination
	public class WorksheetFormatPropertiesDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		public WorksheetFormatPropertiesDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			SheetFormatProperties formatProperties = Importer.CurrentWorksheet.Properties.FormatProperties;
			formatProperties.BeginUpdate();
			try {
				int baseWidth = Importer.GetWpSTIntegerValue(reader, "baseColWidth", 8);
				if (baseWidth > 0)
					formatProperties.BaseColumnWidth = baseWidth;
				float value = Importer.GetWpSTFloatValue(reader, "defaultColWidth", -1);
				if (value >= 0) {
					value = Importer.DocumentModel.GetService<IColumnWidthCalculationService>().RemoveGaps(Importer.CurrentWorksheet, value);
					formatProperties.DefaultColumnWidth = value;
				}
				value = Importer.GetWpSTFloatValue(reader, "defaultRowHeight", -1);
				if (value >= 0)
					formatProperties.DefaultRowHeight = Math.Min(Importer.DocumentModel.UnitConverter.PointsToModelUnitsF(value), Importer.DocumentModel.UnitConverter.TwipsToModelUnits(SheetFormatProperties.MaxDefaultHeightInTwips));
				formatProperties.IsCustomHeight = Importer.GetWpSTOnOffValue(reader, "customHeight", false);
				formatProperties.ZeroHeight = Importer.GetWpSTOnOffValue(reader, "zeroHeight", false);
				formatProperties.ThickTopBorder = Importer.GetWpSTOnOffValue(reader, "thickTop", false);
				formatProperties.ThickBottomBorder = Importer.GetWpSTOnOffValue(reader, "thickBottom", false);
				formatProperties.OutlineLevelCol = Importer.GetIntegerValue(reader, "outlineLevelCol", 0);
				formatProperties.OutlineLevelRow = Importer.GetIntegerValue(reader, "outlineLevelRow", 0);
			}
			finally {
				formatProperties.EndUpdate();
			}
		}
	}
	#endregion
}
