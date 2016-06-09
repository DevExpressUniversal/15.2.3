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

using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	public class PivotTableDefinitionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly Worksheet sheet;
		readonly PivotCache cache;
		PivotTable pivotTable;
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("location", OnLocation);
			result.Add("pivotFields", OnPivotFields);
			result.Add("rowFields", OnRowFields);
			result.Add("rowItems", OnRowItems);
			result.Add("colFields", OnColFields);
			result.Add("colItems", OnColItems);
			result.Add("pageFields", OnPageFields);
			result.Add("dataFields", OnDataFields);
			result.Add("formats", OnFormats);
			result.Add("conditionalFormats", OnConditionalFormats);
			result.Add("chartFormats", OnChartFormats);
			result.Add("pivotHierarchies", OnPivotHierarchies);
			result.Add("pivotTableStyleInfo", OnStyleInfo);
			result.Add("filters", OnFilters);
			result.Add("rowHierarchiesUsage", OnRowHierarchiesUsage);
			result.Add("colHierarchiesUsage", OnColHierarchiesUsage);
			result.Add("extLst", OnExtLst);
			return result;
		}
		#endregion
		public PivotTableDefinitionDestination(SpreadsheetMLBaseImporter importer, Worksheet sheet, PivotCache cache)
			: base(importer) {
			this.sheet = sheet;
			this.cache = cache;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotTable PivotTable { get { return pivotTable; } }
		public Worksheet Worksheet { get { return sheet; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			string name = Importer.GetWpSTXString(reader, "name");
			PivotTableLocation tableLocation = new PivotTableLocation(CellRange.Create(sheet, "A1:B2"));
			pivotTable = new PivotTable(Importer.DocumentModel, name, tableLocation, cache);
			pivotTable.BeginInit();
			ReaderAttributePivotTable(reader, pivotTable);
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			pivotTable.Location.UpdateRangesWithoutHistory(pivotTable);
			sheet.PivotTables.AddCore(pivotTable);
			pivotTable.EndInit();
		}
		static PivotTableDefinitionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTableDefinitionDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnLocation(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDefinitionDestination self = GetThis(importer);
			return new PivotTableLocationDestination(importer, self.PivotTable.Location, self.Worksheet);
		}
		static Destination OnPivotFields(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDefinitionDestination self = GetThis(importer);
			return new PivotTablePivotFieldCollectionDestination(importer, self.PivotTable, self.Worksheet);
		}
		static Destination OnRowFields(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDefinitionDestination self = GetThis(importer);
			return new PivotTableColRowFieldsDestination(importer, self.PivotTable.RowFields);
		}
		static Destination OnColFields(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDefinitionDestination self = GetThis(importer);
			return new PivotTableColRowFieldsDestination(importer, self.PivotTable.ColumnFields);
		}
		static Destination OnColItems(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDefinitionDestination self = GetThis(importer);
			return new PivotTableColRowItemsDestination(importer, self.PivotTable.ColumnItems);
		}
		static Destination OnRowItems(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDefinitionDestination self = GetThis(importer);
			return new PivotTableColRowItemsDestination(importer, self.PivotTable.RowItems);
		}
		static Destination OnPageFields(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDefinitionDestination self = GetThis(importer);
			return new PivotTablePageFieldCollectionDestination(importer, self.PivotTable);
		}
		static Destination OnDataFields(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDefinitionDestination self = GetThis(importer);
			return new PivotTableDataFieldCollectionDestination(importer, self.PivotTable, self.Worksheet);
		}
		static Destination OnFormats(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDefinitionDestination self = GetThis(importer);
			return new PivotTablePivotFormatCollectionDestination(importer, self.PivotTable.Formats, self.Worksheet);
		}
		static Destination OnConditionalFormats(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDefinitionDestination self = GetThis(importer);
			return new PivotTableConditionalFormatCollectionDestination(importer, self.PivotTable.ConditionalFormats, self.Worksheet);
		}
		static Destination OnChartFormats(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDefinitionDestination self = GetThis(importer);
			return new PivotTableChartFormatCollectionDestination(importer, self.PivotTable.ChartFormats, self.Worksheet);
		}
		static Destination OnPivotHierarchies(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDefinitionDestination self = GetThis(importer);
			return new PivotTablePivotHierarchyCollectionDestination(importer, self.PivotTable.Hierarchies, self.Worksheet);
		}
		static Destination OnStyleInfo(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDefinitionDestination self = GetThis(importer);
			return new PivotTableStyleInfoDestination(importer, self.PivotTable);
		}
		static Destination OnFilters(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDefinitionDestination self = GetThis(importer);
			return new PivotTablePivotFilterCollectionDestination(importer, self.PivotTable.Filters, self.Worksheet);
		}
		static Destination OnRowHierarchiesUsage(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDefinitionDestination self = GetThis(importer);
			return new PivotTableHierarchiesUsageDestination(importer, self.PivotTable.RowHierarchiesUsage);
		}
		static Destination OnColHierarchiesUsage(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDefinitionDestination self = GetThis(importer);
			return new PivotTableHierarchiesUsageDestination(importer, self.PivotTable.ColHierarchiesUsage);
		}
		static Destination OnExtLst(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDefinitionDestination self = GetThis(importer);
			return new PivotTableDefinitionExtListDestination(importer, self.PivotTable);
		}
		#endregion
		void ReaderAttributePivotTable(XmlReader reader, PivotTable pivotTable) {
			pivotTable.SetDataCaptionCore(Importer.GetWpSTXString(reader, "dataCaption"));
			pivotTable.SetColHeaderCaptionCore(Importer.GetWpSTXString(reader, "colHeaderCaption"));
			pivotTable.SetErrorCaptionCore(Importer.GetWpSTXString(reader, "errorCaption"));
			pivotTable.SetGrandTotalCaptionCore(Importer.GetWpSTXString(reader, "grandTotalCaption"));
			pivotTable.SetMissingCaptionCore(Importer.GetWpSTXString(reader, "missingCaption"));
			pivotTable.SetPageStyleCore(Importer.GetWpSTXString(reader, "pageStyle"));
			pivotTable.SetPivotTableStyleCore(Importer.GetWpSTXString(reader, "pivotTableStyle"));
			pivotTable.SetRowHeaderCaptionCore(Importer.GetWpSTXString(reader, "rowHeaderCaption"));
			pivotTable.SetTagCore(Importer.GetWpSTXString(reader, "tag"));
			pivotTable.SetVacatedStyleCore(Importer.GetWpSTXString(reader, "vacatedStyle"));
			pivotTable.CreatedVersion = (byte)Importer.GetWpSTIntegerValue(reader, "createdVersion", 0);
			pivotTable.MinRefreshableVersion = (byte)Importer.GetWpSTIntegerValue(reader, "minRefreshableVersion", 0);
			pivotTable.UpdatedVersion = (byte)Importer.GetWpSTIntegerValue(reader, "updatedVersion", 0);
			pivotTable.AutoFormatId = Importer.GetWpSTIntegerValue(reader, "autoFormatId");
			pivotTable.ChartFormat = Importer.GetWpSTIntegerValue(reader, "chartFormat", 0);
			pivotTable.DataPosition = Importer.GetWpSTIntegerValue(reader, "dataPosition", -1);
			int indent = Importer.GetWpSTIntegerValue(reader, "indent", 1) + 1;
			if (indent > 127)
				indent -= 128;
			pivotTable.SetIndent(indent);
			pivotTable.SetPageWrap(Importer.GetWpSTIntegerValue(reader, "pageWrap", 0));
			pivotTable.SetAsteriskTotals(Importer.GetWpSTOnOffValue(reader, "asteriskTotals", false));
			pivotTable.DataOnRows = Importer.GetWpSTOnOffValue(reader, "dataOnRows", false);
			pivotTable.DisableFieldList = Importer.GetWpSTOnOffValue(reader, "disableFieldList", false);
			pivotTable.EditData = Importer.GetWpSTOnOffValue(reader, "editData", false);
			pivotTable.FieldListSortAscending = Importer.GetWpSTOnOffValue(reader, "fieldListSortAscending", false);
			pivotTable.FieldPrintTitles = Importer.GetWpSTOnOffValue(reader, "fieldPrintTitles", false);
			pivotTable.GridDropZones = Importer.GetWpSTOnOffValue(reader, "gridDropZones", false);
			pivotTable.ItemPrintTitles = Importer.GetWpSTOnOffValue(reader, "itemPrintTitles", false);
			pivotTable.MdxSubqueries = Importer.GetWpSTOnOffValue(reader, "mdxSubqueries", false);
			pivotTable.SetMergeItem(Importer.GetWpSTOnOffValue(reader, "mergeItem", false));
			pivotTable.Outline = Importer.GetWpSTOnOffValue(reader, "outline", false);
			pivotTable.SetOutlineData(Importer.GetWpSTOnOffValue(reader, "outlineData", false));
			pivotTable.SetPageOverThenDown(Importer.GetWpSTOnOffValue(reader, "pageOverThenDown", false));
			pivotTable.PrintDrill = Importer.GetWpSTOnOffValue(reader, "printDrill", false);
			pivotTable.Published = Importer.GetWpSTOnOffValue(reader, "published", false);
			pivotTable.SetShowEmptyColumn(Importer.GetWpSTOnOffValue(reader, "showEmptyCol", false));
			pivotTable.SetShowEmptyRow(Importer.GetWpSTOnOffValue(reader, "showEmptyRow", false));
			pivotTable.SetShowError(Importer.GetWpSTOnOffValue(reader, "showError", false));
			pivotTable.SetSubtotalHiddenItems(Importer.GetWpSTOnOffValue(reader, "subtotalHiddenItems", false));
			pivotTable.SetUseAutoFormatting(Importer.GetWpSTOnOffValue(reader, "useAutoFormatting", false));
			pivotTable.SetColumnGrandTotals(Importer.GetWpSTOnOffValue(reader, "rowGrandTotals", true));
			pivotTable.SetRowGrandTotals(Importer.GetWpSTOnOffValue(reader, "colGrandTotals", true));
			pivotTable.Compact = Importer.GetWpSTOnOffValue(reader, "compact", true);
			pivotTable.SetCompactData(Importer.GetWpSTOnOffValue(reader, "compactData", true));
			pivotTable.CustomListSort = Importer.GetWpSTOnOffValue(reader, "customListSort", true);
			pivotTable.EnableDrill = Importer.GetWpSTOnOffValue(reader, "enableDrill", true);
			pivotTable.EnableFieldProperties = Importer.GetWpSTOnOffValue(reader, "enableFieldProperties", true);
			pivotTable.EnableWizard = Importer.GetWpSTOnOffValue(reader, "enableWizard", true);
			pivotTable.Immersive = Importer.GetWpSTOnOffValue(reader, "immersive", true);
			pivotTable.MultipleFieldFilters = Importer.GetWpSTOnOffValue(reader, "multipleFieldFilters", true);
			pivotTable.PreserveFormatting = Importer.GetWpSTOnOffValue(reader, "preserveFormatting", true);
			pivotTable.ShowCalcMbrs = Importer.GetWpSTOnOffValue(reader, "showCalcMbrs", true);
			pivotTable.ShowDataDropDown = Importer.GetWpSTOnOffValue(reader, "showDataDropDown", true);
			pivotTable.ShowDataTips = Importer.GetWpSTOnOffValue(reader, "showDataTips", true);
			pivotTable.SetShowDrill(Importer.GetWpSTOnOffValue(reader, "showDrill", true));
			pivotTable.ShowDropZones = Importer.GetWpSTOnOffValue(reader, "showDropZones", true);
			pivotTable.SetShowHeaders(Importer.GetWpSTOnOffValue(reader, "showHeaders", true));
			pivotTable.ShowItems = Importer.GetWpSTOnOffValue(reader, "showItems", true);
			pivotTable.ShowMemberPropertyTips = Importer.GetWpSTOnOffValue(reader, "showMemberPropertyTips", true);
			pivotTable.ShowMemberPropertyTips = Importer.GetWpSTOnOffValue(reader, "showMemberPropertyTips", true);
			pivotTable.SetShowMissing(Importer.GetWpSTOnOffValue(reader, "showMissing", true));
			pivotTable.ShowMultipleLabel = Importer.GetWpSTOnOffValue(reader, "showMultipleLabel", true);
			pivotTable.VisualTotals = Importer.GetWpSTOnOffValue(reader, "visualTotals", true);
			bool? extBitAttribute = Importer.GetWpSTOnOffNullValue(reader, "applyAlignmentFormats");
			if (extBitAttribute.HasValue)
				pivotTable.ApplyAlignmentFormats = extBitAttribute.Value;
			extBitAttribute = Importer.GetWpSTOnOffNullValue(reader, "applyBorderFormats");
			if (extBitAttribute.HasValue)
				pivotTable.ApplyBorderFormats = extBitAttribute.Value;
			extBitAttribute = Importer.GetWpSTOnOffNullValue(reader, "applyFontFormats");
			if (extBitAttribute.HasValue)
				pivotTable.ApplyFontFormats = extBitAttribute.Value;
			extBitAttribute = Importer.GetWpSTOnOffNullValue(reader, "applyNumberFormats");
			if (extBitAttribute.HasValue)
				pivotTable.ApplyNumberFormats = extBitAttribute.Value;
			extBitAttribute = Importer.GetWpSTOnOffNullValue(reader, "applyPatternFormats");
			if (extBitAttribute.HasValue)
				pivotTable.ApplyPatternFormats = extBitAttribute.Value;
			extBitAttribute = Importer.GetWpSTOnOffNullValue(reader, "applyWidthHeightFormats");
			if (extBitAttribute.HasValue)
				pivotTable.ApplyWidthHeightFormats = extBitAttribute.Value;
		}
	}
	#region PivotTableDefinitionExtListDestination
	public class PivotTableDefinitionExtListDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotTable pivotTable;
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("ext", OnExt);
			return result;
		}
		#endregion
		public PivotTableDefinitionExtListDestination(SpreadsheetMLBaseImporter importer, PivotTable pivotTable)
			: base(importer) {
			this.pivotTable = pivotTable;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotTable PivotTable { get { return pivotTable; } }
		#endregion
		static PivotTableDefinitionExtListDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTableDefinitionExtListDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnExt(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDefinitionExtListDestination self = GetThis(importer);
			return new PivotTableDefinitionExtDestination(importer, self.PivotTable);
		}
		#endregion
	}
	#endregion
	#region PivotTableDefinitionExtDestinationData
	public class PivotTableDefinitionExtDestinationData : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotTable pivotTable;
		public PivotTableDefinitionExtDestinationData(SpreadsheetMLBaseImporter importer, PivotTable pivotTable)
			: base(importer) {
			this.pivotTable = pivotTable;
		}
		#region Properties
		public PivotTable PivotTable { get { return pivotTable; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			PivotTable.SetShowValuesRow(!Importer.GetWpSTOnOffValue(reader, "hideValuesRow", false));
			PivotTable.AltText = Importer.ReadAttribute(reader, "altText");
			PivotTable.AltTextSummary = Importer.ReadAttribute(reader, "altTextSummary");
		}
	}
	#endregion
	#region PivotTableDefinitionExtDestination
	public class PivotTableDefinitionExtDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotTable pivotTable;
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pivotTableDefinition", OnPivotTableDefinition);
			return result;
		}
		#endregion
		public PivotTableDefinitionExtDestination(SpreadsheetMLBaseImporter importer, PivotTable pivotTable)
			: base(importer) {
			this.pivotTable = pivotTable;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotTable PivotTable { get { return pivotTable; } }
		#endregion
		static PivotTableDefinitionExtDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTableDefinitionExtDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnPivotTableDefinition(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDefinitionExtDestination self = GetThis(importer);
			return new PivotTableDefinitionExtDestinationData(importer, self.PivotTable);
		}
		#endregion
	}
	#endregion
}
