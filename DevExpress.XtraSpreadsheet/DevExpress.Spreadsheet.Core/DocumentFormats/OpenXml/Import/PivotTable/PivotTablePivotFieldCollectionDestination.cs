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
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region PivotTablePivotFieldCollectionDestination
	public class PivotTablePivotFieldCollectionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotTable pivotTable;
		readonly Worksheet worksheet;
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pivotField", OnPivotField);
			return result;
		}
		#endregion
		public PivotTablePivotFieldCollectionDestination(SpreadsheetMLBaseImporter importer, PivotTable pivotTable, Worksheet worksheet)
			: base(importer) {
				this.pivotTable = pivotTable;
			this.worksheet = worksheet;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public Worksheet Worksheet { get { return worksheet; } }
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			PivotGenerateItemsCommand generateItemsCommand = new PivotGenerateItemsCommand(pivotTable);
			generateItemsCommand.GenerateItemsAfterImport();
		}
		static PivotTablePivotFieldCollectionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTablePivotFieldCollectionDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnPivotField(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePivotFieldCollectionDestination self = GetThis(importer);
			return new PivotTablePivotFieldDestination(importer, self.pivotTable, self.Worksheet);
		}
		#endregion
	}
	#endregion
	#region PivotTablePivotFieldDestination
	public class PivotTablePivotFieldDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly Worksheet worksheet;
		readonly PivotField pivotField;
		readonly PivotTable pivotTable;
		#region PivotTableAxis & PivotTableSortTypeField
		public static Dictionary<PivotTableSortTypeField, string> pivotTableFieldSortTypeTable = CreatePivotTableFieldSortTypeTable();
		public static Dictionary<string, PivotTableSortTypeField> reversePivotTableFieldSortTypeTable = DictionaryUtils.CreateBackTranslationTable(pivotTableFieldSortTypeTable);
		static Dictionary<PivotTableSortTypeField, string> CreatePivotTableFieldSortTypeTable() {
			Dictionary<PivotTableSortTypeField, string> result = new Dictionary<PivotTableSortTypeField, string>();
			result.Add(PivotTableSortTypeField.Ascending, "ascending");
			result.Add(PivotTableSortTypeField.Descending, "descending");
			result.Add(PivotTableSortTypeField.Manual, "manual");
			return result;
		}
		public static Dictionary<PivotTableAxis, string> pivotTableAxisTable = CreatePivotTableAxisTable();
		public static Dictionary<string, PivotTableAxis> reversePivotTableAxisTable = DictionaryUtils.CreateBackTranslationTable(pivotTableAxisTable);
		static Dictionary<PivotTableAxis, string> CreatePivotTableAxisTable() {
			Dictionary<PivotTableAxis, string> result = new Dictionary<PivotTableAxis, string>();
			result.Add(PivotTableAxis.None, "");
			result.Add(PivotTableAxis.Value, "axisValues");
			result.Add(PivotTableAxis.Row, "axisRow");
			result.Add(PivotTableAxis.Column, "axisCol");
			result.Add(PivotTableAxis.Page, "axisPage");
			return result;
		}
		#endregion
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("items", OnPivotItems);
			result.Add("autoSortScope", OnAutoSortScope);
			result.Add("extLst", OnExtLst);
			return result;
		}
		#endregion
		public PivotTablePivotFieldDestination(SpreadsheetMLBaseImporter importer, PivotTable pivotTable, Worksheet worksheet)
			: base(importer) {
			this.worksheet = worksheet;
			this.pivotTable = pivotTable;
			pivotField = new PivotField(pivotTable);
			pivotTable.Fields.AddCore(pivotField);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			pivotField.BeginInit();
			ReaderBoolAttribute(reader);
			ReaderOtherAtribute(reader);
		}
		public override void ProcessElementClose(XmlReader reader) {
			pivotField.EndInit();
		}
		void ReaderBoolAttribute(XmlReader reader) {
			Field.AllDrilled = Importer.GetWpSTOnOffValue(reader, "allDrilled", false);
			Field.AutoShow = Importer.GetWpSTOnOffValue(reader, "autoShow", false);
			Field.SetSubtotal(ReadSubtotal(reader));
			Field.IsDataField = Importer.GetWpSTOnOffValue(reader, "dataField", false);
			Field.DefaultAttributeDrillState = Importer.GetWpSTOnOffValue(reader, "defaultAttributeDrillState", false);
			Field.HiddenLevel = Importer.GetWpSTOnOffValue(reader, "hiddenLevel", false);
			Field.HideNewItems = Importer.GetWpSTOnOffValue(reader, "hideNewItems", false);
			Field.IncludeNewItemsInFilter = Importer.GetWpSTOnOffValue(reader, "includeNewItemsInFilter", false);
			Field.SetInsertBlankRow(Importer.GetWpSTOnOffValue(reader, "insertBlankRow", false));
			Field.InsertPageBreak = Importer.GetWpSTOnOffValue(reader, "insertPageBreak", false);
			Field.MeasureFilter = Importer.GetWpSTOnOffValue(reader, "measureFilter", false);
			Field.NonAutoSortDefault = Importer.GetWpSTOnOffValue(reader, "nonAutoSortDefault", false);
			Field.ServerField = Importer.GetWpSTOnOffValue(reader, "serverField", false);
			Field.ShowPropAsCaption = Importer.GetWpSTOnOffValue(reader, "showPropAsCaption", false);
			Field.ShowPropCell = Importer.GetWpSTOnOffValue(reader, "showPropCell", false);
			Field.ShowPropTip = Importer.GetWpSTOnOffValue(reader, "showPropTip", false);
			Field.SetCompact(Importer.GetWpSTOnOffValue(reader, "compact", true));
			Field.DragOff = Importer.GetWpSTOnOffValue(reader, "dragOff", true);
			Field.DragToCol = Importer.GetWpSTOnOffValue(reader, "dragToCol", true);
			Field.DragToData = Importer.GetWpSTOnOffValue(reader, "dragToData", true);
			Field.DragToPage = Importer.GetWpSTOnOffValue(reader, "dragToPage", true);
			Field.DragToRow = Importer.GetWpSTOnOffValue(reader, "dragToRow", true);
			Field.MultipleItemSelectionAllowed = Importer.GetWpSTOnOffValue(reader, "multipleItemSelectionAllowed", false);
			Field.SetOutline(Importer.GetWpSTOnOffValue(reader, "outline", true));
			Field.SetShowItemsWithNoData(Importer.GetWpSTOnOffValue(reader, "showAll", true));
			Field.ShowDropDowns = Importer.GetWpSTOnOffValue(reader, "showDropDowns", true);
			Field.SetSubtotalTop(Importer.GetWpSTOnOffValue(reader, "subtotalTop", true));
			Field.TopAutoShow = Importer.GetWpSTOnOffValue(reader, "topAutoShow", true);
			bool? extBitAttribute = Importer.GetWpSTOnOffNullValue(reader, "dataSourceSort");
			if (extBitAttribute.HasValue)
				Field.DataSourceSort = extBitAttribute.Value;
		}
		PivotFieldItemType ReadSubtotal(XmlReader reader) {
			PivotFieldItemType subtotal = PivotFieldItemType.Blank;
			if (Importer.GetWpSTOnOffValue(reader, "avgSubtotal", false))
				subtotal |= PivotFieldItemType.Avg;
			if (Importer.GetWpSTOnOffValue(reader, "stdDevPSubtotal", false))
				subtotal |= PivotFieldItemType.StdDevP;
			if (Importer.GetWpSTOnOffValue(reader, "stdDevSubtotal", false))
				subtotal |= PivotFieldItemType.StdDev;
			if (Importer.GetWpSTOnOffValue(reader, "sumSubtotal", false))
				subtotal |= PivotFieldItemType.Sum;
			if (Importer.GetWpSTOnOffValue(reader, "varPSubtotal", false))
				subtotal |= PivotFieldItemType.VarP;
			if (Importer.GetWpSTOnOffValue(reader, "varSubtotal", false))
				subtotal |= PivotFieldItemType.Var;
			if (Importer.GetWpSTOnOffValue(reader, "minSubtotal", false))
				subtotal |= PivotFieldItemType.Min;
			if (Importer.GetWpSTOnOffValue(reader, "countASubtotal", false))
				subtotal |= PivotFieldItemType.CountA;
			if (Importer.GetWpSTOnOffValue(reader, "countSubtotal", false))
				subtotal |= PivotFieldItemType.Count;
			if (Importer.GetWpSTOnOffValue(reader, "maxSubtotal", false))
				subtotal |= PivotFieldItemType.Max;
			if (Importer.GetWpSTOnOffValue(reader, "defaultSubtotal", true))
				subtotal |= PivotFieldItemType.DefaultValue;
			if (Importer.GetWpSTOnOffValue(reader, "productSubtotal", false))
				subtotal |= PivotFieldItemType.Product;
			return subtotal;
		}
		void ReaderOtherAtribute(XmlReader reader) {
			Field.SetNameCore(Importer.GetWpSTXString(reader, "name"));
			Field.SetSubtotalCaptionCore(Importer.GetWpSTXString(reader, "subtotalCaption"));
			Field.UniqueMemberProperty = Importer.GetWpSTXString(reader, "uniqueMemberProperty");
			Field.ItemPageCount = Importer.GetWpSTIntegerValue(reader, "itemPageCount", 10);
			int value = -1;
			value = Importer.GetWpSTIntegerValue(reader, "rankBy");
			if (value >= 0)
				Field.RankBy = value;
			value = Importer.GetWpSTIntegerValue(reader, "numFmtId", -1);
			if (value >= 0)
				Field.SetNumberFormatIndex(Importer.StyleSheet.GetNumberFormatIndex(value));
			Field.Axis = Importer.GetWpEnumValue<PivotTableAxis>(reader, "axis", reversePivotTableAxisTable, PivotTableAxis.None);
			Field.SetSortType(Importer.GetWpEnumValue<PivotTableSortTypeField>(reader, "sortType", reversePivotTableFieldSortTypeTable, PivotTableSortTypeField.Manual));
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotField Field { get { return pivotField; } }
		public Worksheet Worksheet { get { return worksheet; } }
		#endregion
		static PivotTablePivotFieldDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTablePivotFieldDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnPivotItems(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePivotFieldDestination self = GetThis(importer);
			return new PivotTablePivotItemCollectionDestination(importer, self.pivotTable, self.Field, self.Worksheet);
		}
		static Destination OnAutoSortScope(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePivotFieldDestination self = GetThis(importer);
			return new PivotTablePivotFieldAutoSortScopeDestination(importer, self.Field, self.Worksheet);
		}
		static Destination OnExtLst(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePivotFieldDestination self = GetThis(importer);
			return new PivotTablePivotFieldsExtListDestination(importer, self.Field, self.Worksheet);
		}
		#endregion
	}
	#endregion
	#region PivotTablePivotFieldsExtListDestination
	public class PivotTablePivotFieldsExtListDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly Worksheet worksheet;
		readonly PivotField pivotField;
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("ext", OnExt);
			return result;
		}
		#endregion
		public PivotTablePivotFieldsExtListDestination(SpreadsheetMLBaseImporter importer, PivotField pivotField, Worksheet worksheet)
			: base(importer) {
				this.worksheet = worksheet;
				this.pivotField = pivotField;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotField PivotField { get { return pivotField; } }
		public Worksheet Worksheet { get { return worksheet; } }
		#endregion
		static PivotTablePivotFieldsExtListDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTablePivotFieldsExtListDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnExt(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePivotFieldsExtListDestination self = GetThis(importer);
			return new PivotTablePivotFieldsExtDestination(importer, self.PivotField);
		}
		#endregion
	}
	#endregion
	#region PivotTablePivotFieldsExtDestination
	public class PivotTablePivotFieldsExtDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotField pivotField;
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pivotField", OnPivotField);
			return result;
		}
		#endregion
		public PivotTablePivotFieldsExtDestination(SpreadsheetMLBaseImporter importer, PivotField pivotField)
			: base(importer) {
			this.pivotField = pivotField;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotField PivotField { get { return pivotField; } }
		#endregion
		static PivotTablePivotFieldsExtDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTablePivotFieldsExtDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnPivotField(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePivotFieldsExtDestination self = GetThis(importer);
			return new OnOffValueDestination(importer, self.ReadingSingleAttribute, "fillDownLabels", true);
		}
		#endregion
		void ReadingSingleAttribute(bool value) { 
			PivotField.SetFillDownLabels(value);
		}
	}
	#endregion
	#region PivotTablePivotFieldAutoSortScopeDestination
	public class PivotTablePivotFieldAutoSortScopeDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotField pivotField;
		readonly Worksheet worksheet;
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pivotArea", OnPivotArea);
			return result;
		}
		#endregion
		public PivotTablePivotFieldAutoSortScopeDestination(SpreadsheetMLBaseImporter importer, PivotField pivotField, Worksheet worksheet)
			: base(importer) {
				this.worksheet = worksheet;
				this.pivotField = pivotField;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotArea PivotArea { get { return pivotField.PivotArea; } }
		public Worksheet Worksheet { get { return worksheet; } }
		#endregion
		static PivotTablePivotFieldAutoSortScopeDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTablePivotFieldAutoSortScopeDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnPivotArea(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePivotFieldAutoSortScopeDestination self = GetThis(importer);
			return new PivotTablePivotAreaDestination(importer, self.PivotArea, self.Worksheet);
		}
		#endregion
	}
	#endregion
}
