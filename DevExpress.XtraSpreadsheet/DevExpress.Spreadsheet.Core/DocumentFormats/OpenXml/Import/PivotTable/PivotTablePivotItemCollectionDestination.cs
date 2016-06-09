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
	#region PivotTablePivotItemsDestination
	public class PivotTablePivotItemCollectionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotField pivotField;
		readonly PivotTable pivotTable;
		readonly Worksheet worksheet;
		public static Dictionary<PivotFieldItemType, string> pivotTableItemTypeTable = CreatePivotTableItemTypeTable();
		public static Dictionary<string, PivotFieldItemType> reversePivoItemTypeTable = DictionaryUtils.CreateBackTranslationTable(pivotTableItemTypeTable);
		static Dictionary<PivotFieldItemType, string> CreatePivotTableItemTypeTable() {
			Dictionary<PivotFieldItemType, string> result = new Dictionary<PivotFieldItemType, string>();
			result.Add(PivotFieldItemType.Avg, "avg");
			result.Add(PivotFieldItemType.Blank, "blank");
			result.Add(PivotFieldItemType.Count, "count");
			result.Add(PivotFieldItemType.CountA, "countA");
			result.Add(PivotFieldItemType.Data, "data");
			result.Add(PivotFieldItemType.DefaultValue, "default");
			result.Add(PivotFieldItemType.Grand, "grand");
			result.Add(PivotFieldItemType.Max, "max");
			result.Add(PivotFieldItemType.Min, "min");
			result.Add(PivotFieldItemType.Product, "product");
			result.Add(PivotFieldItemType.StdDev, "stdDev");
			result.Add(PivotFieldItemType.StdDevP, "stdDevP");
			result.Add(PivotFieldItemType.Sum, "sum");
			result.Add(PivotFieldItemType.Var, "var");
			result.Add(PivotFieldItemType.VarP, "varP");
			return result;
		}
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("item", OnPivotItem);
			return result;
		}
		#endregion
		public PivotTablePivotItemCollectionDestination(SpreadsheetMLBaseImporter importer, PivotTable pivotTable, PivotField pivotField, Worksheet worksheet)
			: base(importer) {
			this.pivotField = pivotField;
			this.pivotTable = pivotTable;
			this.worksheet = worksheet;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotField PivotField { get { return pivotField; } }
		public Worksheet Worksheet { get { return worksheet; } }
		#endregion
		static PivotTablePivotItemCollectionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTablePivotItemCollectionDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnPivotItem(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePivotItemCollectionDestination self = GetThis(importer);
			return new PivotTablePivotItemDestination(importer, self.pivotTable, self.PivotField.Items, self.Worksheet);
		}
		#endregion
	}
	#endregion
	#region PivotTablePivotItemDestination
	public class PivotTablePivotItemDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		public static Dictionary<PivotFieldItemType, string> pivotTableItemTypeTable = CreatePivotTableItemTypeTable();
		public static Dictionary<string, PivotFieldItemType> reversePivoItemTypeTable = DictionaryUtils.CreateBackTranslationTable(pivotTableItemTypeTable);
		static Dictionary<PivotFieldItemType, string> CreatePivotTableItemTypeTable() {
			Dictionary<PivotFieldItemType, string> result = new Dictionary<PivotFieldItemType, string>();
			result.Add(PivotFieldItemType.Avg, "avg");
			result.Add(PivotFieldItemType.Blank, "blank");
			result.Add(PivotFieldItemType.Count, "count");
			result.Add(PivotFieldItemType.CountA, "countA");
			result.Add(PivotFieldItemType.Data, "data");
			result.Add(PivotFieldItemType.DefaultValue, "default");
			result.Add(PivotFieldItemType.Grand, "grand");
			result.Add(PivotFieldItemType.Max, "max");
			result.Add(PivotFieldItemType.Min, "min");
			result.Add(PivotFieldItemType.Product, "product");
			result.Add(PivotFieldItemType.StdDev, "stdDev");
			result.Add(PivotFieldItemType.StdDevP, "stdDevP");
			result.Add(PivotFieldItemType.Sum, "sum");
			result.Add(PivotFieldItemType.Var, "var");
			result.Add(PivotFieldItemType.VarP, "varP");
			return result;
		}
		PivotItem pivotItem;
		PivotItemCollection items;
		public PivotTablePivotItemDestination(SpreadsheetMLBaseImporter importer, PivotTable pivotTable, PivotItemCollection items, Worksheet worksheet)
			: base(importer) {
			this.pivotItem = new PivotItem(pivotTable);
			this.items = items;
		}
		public PivotItem PivotItem { get { return pivotItem; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			pivotItem.SetChildItemsCore(Importer.GetWpSTOnOffValue(reader, "c", false));
			pivotItem.SetHasExpandedViewCore(Importer.GetWpSTOnOffValue(reader, "d", false));
			pivotItem.SetDrillAcrossCore(Importer.GetWpSTOnOffValue(reader, "e", true));
			pivotItem.SetCalculatedMemberCore(Importer.GetWpSTOnOffValue(reader, "f", false));
			pivotItem.SetIsHiddenCore(Importer.GetWpSTOnOffValue(reader, "h", false));
			pivotItem.SetHasMissingValueCore(Importer.GetWpSTOnOffValue(reader, "m", false));
			pivotItem.SetHasCharacterValueCore(Importer.GetWpSTOnOffValue(reader, "s", false));
			pivotItem.SetHideDetailsCore(!Importer.GetWpSTOnOffValue(reader, "sd", true));
			pivotItem.SetItemIndexCore(Importer.GetWpSTIntegerValue(reader, "x", -1));
			pivotItem.SetItemUserCaptionCore(Importer.GetWpSTXString(reader, "n"));
			pivotItem.SetItemTypeCore(Importer.GetWpEnumValue<PivotFieldItemType>(reader, "t", reversePivoItemTypeTable, PivotFieldItemType.Data));
		}
		public override void ProcessElementClose(XmlReader reader) {
			items.AddCore(pivotItem);
		}
	}
	#endregion
}
