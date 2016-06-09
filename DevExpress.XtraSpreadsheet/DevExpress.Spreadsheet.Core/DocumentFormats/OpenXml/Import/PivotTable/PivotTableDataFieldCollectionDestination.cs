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
	#region PivotTableDataFieldCollectionDestination
	public class PivotTableDataFieldCollectionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly Worksheet worksheet;
		readonly PivotTable pivotTable;
		#region Handler
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("dataField", OnDataField);
			return result;
		}
		#endregion
		public PivotTableDataFieldCollectionDestination(SpreadsheetMLBaseImporter importer, PivotTable pivotTable, Worksheet worksheet)
			: base(importer) {
			this.worksheet = worksheet;
			this.pivotTable = pivotTable;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotTable PivotTable { get { return pivotTable; } }
		public Worksheet Worksheet { get { return worksheet; } }
		#endregion
		static PivotTableDataFieldCollectionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTableDataFieldCollectionDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnDataField(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDataFieldCollectionDestination self = GetThis(importer);
			return new PivotTableDataFieldDestination(importer, self.PivotTable, self.Worksheet);
		}
		#endregion
	}
	#endregion
	#region PivotTableDataFieldDestination
	public class PivotTableDataFieldDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		public static Dictionary<PivotDataConsolidateFunction, string> pivotDataConsolidateFunctionTable = CreatePivotDataConsolidateFunctionTable();
		public static Dictionary<string, PivotDataConsolidateFunction> reversePivotDataConsolidateFunctionTable = DictionaryUtils.CreateBackTranslationTable(pivotDataConsolidateFunctionTable);
		static Dictionary<PivotDataConsolidateFunction, string> CreatePivotDataConsolidateFunctionTable() {
			Dictionary<PivotDataConsolidateFunction, string> result = new Dictionary<PivotDataConsolidateFunction, string>();
			result.Add(PivotDataConsolidateFunction.Average, "average");
			result.Add(PivotDataConsolidateFunction.Count, "count");
			result.Add(PivotDataConsolidateFunction.CountNums, "countNums");
			result.Add(PivotDataConsolidateFunction.Max, "max");
			result.Add(PivotDataConsolidateFunction.Min, "min");
			result.Add(PivotDataConsolidateFunction.Product, "product");
			result.Add(PivotDataConsolidateFunction.StdDev, "stdDev");
			result.Add(PivotDataConsolidateFunction.StdDevp, "stdDevp");
			result.Add(PivotDataConsolidateFunction.Sum, "sum");
			result.Add(PivotDataConsolidateFunction.Var, "var");
			result.Add(PivotDataConsolidateFunction.Varp, "varp");
			return result;
		}
		public static Dictionary<PivotShowDataAs, string> pivotShowDataAsTable = CreatePivotShowDataAsTable();
		public static Dictionary<string, PivotShowDataAs> reversePivotShowDataAsTable = DictionaryUtils.CreateBackTranslationTable(pivotShowDataAsTable);
		static Dictionary<PivotShowDataAs, string> CreatePivotShowDataAsTable() {
			Dictionary<PivotShowDataAs, string> result = new Dictionary<PivotShowDataAs, string>();
			result.Add(PivotShowDataAs.Difference, "difference");
			result.Add(PivotShowDataAs.Index, "index");
			result.Add(PivotShowDataAs.Normal, "normal");
			result.Add(PivotShowDataAs.Percent, "percent");
			result.Add(PivotShowDataAs.PercentDifference, "percentDiff");
			result.Add(PivotShowDataAs.PercentOfColumn, "percentOfCol");
			result.Add(PivotShowDataAs.PercentOfParent, "percentOfParent");
			result.Add(PivotShowDataAs.PercentOfParentColumn, "percentOfParentCol");
			result.Add(PivotShowDataAs.PercentOfParentRow, "percentOfParentRow");
			result.Add(PivotShowDataAs.PercentOfRow, "percentOfRow");
			result.Add(PivotShowDataAs.PercentOfRunningTotal, "percentOfRunningTotal");
			result.Add(PivotShowDataAs.PercentOfTotal, "percentOfTotal");
			result.Add(PivotShowDataAs.RankAscending, "rankAscending");
			result.Add(PivotShowDataAs.RankDescending, "rankDescending");
			result.Add(PivotShowDataAs.RunningTotal, "runTotal");
			return result;
		}
		readonly Worksheet worksheet;
		readonly PivotTable pivotTable;
		PivotDataField pivotDataField;
		#region Handler
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("extLst", OnExtList);
			return result;
		}
		#endregion
		public PivotTableDataFieldDestination(SpreadsheetMLBaseImporter importer, PivotTable pivotTable, Worksheet worksheet)
			: base(importer) {
			this.worksheet = worksheet;
			this.pivotTable = pivotTable;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotTable PivotTable { get { return pivotTable; } }
		public PivotDataField PivotDataField { get { return pivotDataField; } }
		public Worksheet Worksheet { get { return worksheet; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			pivotDataField = new PivotDataField(PivotTable, Importer.GetWpSTIntegerValue(reader, "fld"));
			PivotTable.DataFields.AddCore(pivotDataField);
			string name = Importer.GetWpSTXString(reader, "name");
			if (!String.IsNullOrEmpty(name))
				pivotDataField.SetNameCore(name);
			pivotDataField.SetSubtotalCore(Importer.GetWpEnumValue<PivotDataConsolidateFunction>(reader, "subtotal", reversePivotDataConsolidateFunctionTable, PivotDataConsolidateFunction.Sum));
			pivotDataField.SetShowDataAsCore(Importer.GetWpEnumValue<PivotShowDataAs>(reader, "showDataAs", reversePivotShowDataAsTable, PivotShowDataAs.Normal));
			pivotDataField.SetBaseFieldCore(Importer.GetWpSTIntegerValue(reader, "baseField", -1));
			pivotDataField.SetBaseItemCore(Importer.GetWpSTIntegerValue(reader, "baseItem", 1048832));
			int numberFormatId = Importer.GetWpSTIntegerValue(reader, "numFmtId", -1);
			if (numberFormatId >= 0)
				pivotDataField.SetNumberFormatIndexCore(Importer.StyleSheet.GetNumberFormatIndex(numberFormatId));
		}
		static PivotTableDataFieldDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTableDataFieldDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnExtList(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableDataFieldDestination self = GetThis(importer);
			return new PivotTableExtListDataFieldDestination(importer, self.PivotDataField);
		}
		#endregion
	}
	#endregion
	#region PivotTableExtListDataFieldDestination
	public class PivotTableExtListDataFieldDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		PivotDataField pivotDataFild;
		#region Handler
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("ext", OnExt);
			return result;
		}
		#endregion
		public PivotTableExtListDataFieldDestination(SpreadsheetMLBaseImporter importer, PivotDataField pivotDataFild)
			: base(importer) {
			this.pivotDataFild = pivotDataFild;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotDataField PivotDataFild { get { return pivotDataFild; } }
		#endregion
		static PivotTableExtListDataFieldDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTableExtListDataFieldDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnExt(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableExtListDataFieldDestination self = GetThis(importer);
			return new PivotTableExtDataFieldDestination(importer, self.PivotDataFild);
		}
		#endregion
	}
	#endregion
	#region PivotTableExtDataFieldDestination
	public class PivotTableExtDataFieldDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		PivotDataField pivotDataFild;
		#region Handler
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("dataField", OnDataField);
			return result;
		}
		#endregion
		public PivotTableExtDataFieldDestination(SpreadsheetMLBaseImporter importer, PivotDataField pivotDataFild)
			: base(importer) {
			this.pivotDataFild = pivotDataFild;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotDataField PivotDataFild { get { return pivotDataFild; } }
		#endregion
		static PivotTableExtDataFieldDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTableExtDataFieldDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnDataField(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableExtDataFieldDestination self = GetThis(importer);
			return new EnumValueDestination<PivotShowDataAs>(importer, PivotTableDataFieldDestination.reversePivotShowDataAsTable, self.ReadAttribute, "pivotShowAs", PivotShowDataAs.Normal);
		}
		#endregion
		void ReadAttribute(PivotShowDataAs value) {
			PivotDataFild.SetShowDataAsCore(value);
		}
	}
	#endregion
}
