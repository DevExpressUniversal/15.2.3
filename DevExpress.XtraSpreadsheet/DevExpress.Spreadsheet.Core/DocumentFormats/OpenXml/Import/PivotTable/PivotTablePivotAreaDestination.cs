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
	#region PivotTablePivotAreaDestination
	public class PivotTablePivotAreaDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotArea pivotArea;
		readonly Worksheet worksheet;
		public static Dictionary<PivotAreaType, string> pivotTableAreaTypeTable = CreatePivotTableAreaTypeTable();
		public static Dictionary<string, PivotAreaType> reversePivoAreaTypeTable = DictionaryUtils.CreateBackTranslationTable(pivotTableAreaTypeTable);
		static Dictionary<PivotAreaType, string> CreatePivotTableAreaTypeTable() {
			Dictionary<PivotAreaType, string> result = new Dictionary<PivotAreaType, string>();
			result.Add(PivotAreaType.None, "none");
			result.Add(PivotAreaType.Normal, "normal");
			result.Add(PivotAreaType.Data, "data");
			result.Add(PivotAreaType.All, "all");
			result.Add(PivotAreaType.Origin, "origin");
			result.Add(PivotAreaType.Button, "button");
			result.Add(PivotAreaType.TopRight, "topRight");
			return result;
		}
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("references", OnPivotReferences);
			return result;
		}
		#endregion
		public PivotTablePivotAreaDestination(SpreadsheetMLBaseImporter importer, PivotArea pivotArea, Worksheet worksheet)
			: base(importer) {
			this.worksheet = worksheet;
			this.pivotArea = pivotArea;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			PivotArea.SetCacheIndexCore(Importer.GetWpSTOnOffValue(reader, "cacheIndex", false));
			PivotArea.SetCollapsedLevelsAreSubtotalsCore(Importer.GetWpSTOnOffValue(reader, "collapsedLevelsAreSubtotals", false));
			PivotArea.SetGrandColCore(Importer.GetWpSTOnOffValue(reader, "grandCol", false));
			PivotArea.SetGrandRowCore(Importer.GetWpSTOnOffValue(reader, "grandRow", false));
			PivotArea.SetLabelOnlyCore(Importer.GetWpSTOnOffValue(reader, "labelOnly", false));
			PivotArea.SetDataOnlyCore(Importer.GetWpSTOnOffValue(reader, "dataOnly", true));
			PivotArea.SetOutlineCore(Importer.GetWpSTOnOffValue(reader, "outline", true));
			int value = Importer.GetWpSTIntegerValue(reader, "field");
			if (value >= 0)
				PivotArea.SetFieldCore(value);
			value = Importer.GetWpSTIntegerValue(reader, "fieldPosition");
			if (value >= 0)
				PivotArea.SetFieldPositionCore(value);
			PivotArea.SetAxisCore(Importer.GetWpEnumValue<PivotTableAxis>(reader, "axis", PivotTablePivotFieldDestination.reversePivotTableAxisTable, PivotTableAxis.None));
			PivotArea.SetTypeCore(Importer.GetWpEnumValue<PivotAreaType>(reader, "type", reversePivoAreaTypeTable, PivotAreaType.Normal));
			PivotArea.SetRangeCore(Importer.ReadCellRange(reader, "offset", Worksheet));
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotArea PivotArea { get { return pivotArea; } }
		public Worksheet Worksheet { get { return worksheet; } }
		#endregion
		static PivotTablePivotAreaDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTablePivotAreaDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnPivotReferences(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePivotAreaDestination self = GetThis(importer);
			return new PivotTablePivotAreaReferenceCollectionDestination(importer, self.PivotArea, self.Worksheet);
		}
		#endregion
	}
	#endregion
	#region PivotTablePivotAreaReferenceCollectionDestination
	public class PivotTablePivotAreaReferenceCollectionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotAreaReferenceCollection references;
		readonly Worksheet worksheet;
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("reference", OnPivotReference);
			return result;
		}
		#endregion
		public PivotTablePivotAreaReferenceCollectionDestination(SpreadsheetMLBaseImporter importer, PivotArea pivotArea, Worksheet worksheet)
			: base(importer) {
			this.worksheet = worksheet;
			this.references = pivotArea.References;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public Worksheet Worksheet { get { return worksheet; } }
		public PivotAreaReferenceCollection References { get { return references; } }
		#endregion
		static PivotTablePivotAreaReferenceCollectionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTablePivotAreaReferenceCollectionDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnPivotReference(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePivotAreaReferenceCollectionDestination self = GetThis(importer);
			return new PivotTablePivotAreaReferenceDestination(importer, self.References, self.Worksheet);
		}
		#endregion
	}
	#endregion
	#region PivotTablePivotAreaReferenceDestination
	public class PivotTablePivotAreaReferenceDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotAreaReference reference;
		readonly Worksheet worksheet;
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("x", OnSharedItemsIndex);
			return result;
		}
		#endregion
		public PivotTablePivotAreaReferenceDestination(SpreadsheetMLBaseImporter importer, PivotAreaReferenceCollection references, Worksheet worksheet)
			: base(importer) {
			this.worksheet = worksheet;
			reference = new PivotAreaReference(worksheet.Workbook);
			references.AddCore(reference);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			long? value = Importer.GetLongValue(reader, "field");
			if (value >= 0)
				Reference.Field = value;
			Reference.SetSelectedCore(Importer.GetWpSTOnOffValue(reader, "selected", true));
			Reference.SetByPositionCore(Importer.GetWpSTOnOffValue(reader, "byPosition", false));
			Reference.SetRelativeCore(Importer.GetWpSTOnOffValue(reader, "relative", false));
			Reference.SetSubtotalCore((int)ReadSubtotal(reader));
		}
		PivotFieldItemType ReadSubtotal(XmlReader reader) {
			PivotFieldItemType subtotal = PivotFieldItemType.Blank;
			if (Importer.GetWpSTOnOffValue(reader, "defaultSubtotal", false))
				subtotal |= PivotFieldItemType.DefaultValue;
			if (Importer.GetWpSTOnOffValue(reader, "sumSubtotal", false))
				subtotal |= PivotFieldItemType.Sum;
			if (Importer.GetWpSTOnOffValue(reader, "countASubtotal", false))
				subtotal |= PivotFieldItemType.CountA;
			if (Importer.GetWpSTOnOffValue(reader, "avgSubtotal", false))
				subtotal |= PivotFieldItemType.Avg;
			if (Importer.GetWpSTOnOffValue(reader, "maxSubtotal", false))
				subtotal |= PivotFieldItemType.Max;
			if (Importer.GetWpSTOnOffValue(reader, "minSubtotal", false))
				subtotal |= PivotFieldItemType.Min;
			if (Importer.GetWpSTOnOffValue(reader, "productSubtotal", false))
				subtotal |= PivotFieldItemType.Product;
			if (Importer.GetWpSTOnOffValue(reader, "countSubtotal", false))
				subtotal |= PivotFieldItemType.Count;
			if (Importer.GetWpSTOnOffValue(reader, "stdDevSubtotal", false))
				subtotal |= PivotFieldItemType.StdDev;
			if (Importer.GetWpSTOnOffValue(reader, "stdDevPSubtotal", false))
				subtotal |= PivotFieldItemType.StdDevP;
			if (Importer.GetWpSTOnOffValue(reader, "varSubtotal", false))
				subtotal |= PivotFieldItemType.Var;
			if (Importer.GetWpSTOnOffValue(reader, "varPSubtotal", false))
				subtotal |= PivotFieldItemType.VarP;
			return subtotal;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public Worksheet Worksheet { get { return worksheet; } }
		public PivotAreaReference Reference { get { return reference; } }
		#endregion
		static PivotTablePivotAreaReferenceDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTablePivotAreaReferenceDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnSharedItemsIndex(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePivotAreaReferenceDestination self = GetThis(importer);
			return new IntegerValueDestination(importer, self.ReadingSingleAttribute, "v", 0);
		}
		#endregion
		void ReadingSingleAttribute(int value) { 
			Reference.SharedItemsIndex.Add((long)value);
		}
	}
	#endregion
}
