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
	#region PivotTableConditionalFormatCollectionDestination
	public class PivotTableConditionalFormatCollectionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly Worksheet worksheet;
		readonly PivotConditionalFormatCollection conditionalFormats;
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("conditionalFormat", OnConditionalFormat);
			return result;
		}
		#endregion
		public PivotTableConditionalFormatCollectionDestination(SpreadsheetMLBaseImporter importer, PivotConditionalFormatCollection conditionalFormats, Worksheet worksheet)
			: base(importer) {
			this.worksheet = worksheet;
			this.conditionalFormats = conditionalFormats;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public Worksheet Worksheet { get { return worksheet; } }
		public PivotConditionalFormatCollection ConditionalFormats { get { return conditionalFormats; } }
		#endregion
		static PivotTableConditionalFormatCollectionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTableConditionalFormatCollectionDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnConditionalFormat(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableConditionalFormatCollectionDestination self = GetThis(importer);
			return new PivotTableConditionalFormatDestination(importer, self.ConditionalFormats, self.Worksheet);
		}
		#endregion
	}
	#endregion
	#region PivotTableConditionalFormatDestination
	public class PivotTableConditionalFormatDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		public static Dictionary<ConditionalFormatScope, string> pivotTableConditionalFormatScopeTable = CreatePivotTableConditionalFormatScopeTable();
		public static Dictionary<string, ConditionalFormatScope> reversePivotTableConditionalFormatScopeTable = DictionaryUtils.CreateBackTranslationTable(pivotTableConditionalFormatScopeTable);
		static Dictionary<ConditionalFormatScope, string> CreatePivotTableConditionalFormatScopeTable() {
			Dictionary<ConditionalFormatScope, string> result = new Dictionary<ConditionalFormatScope, string>();
			result.Add(ConditionalFormatScope.Data, "data");
			result.Add(ConditionalFormatScope.Field, "field");
			result.Add(ConditionalFormatScope.Selection, "selection");
			return result;
		}
		public static Dictionary<ConditionalFormatType, string> pivotTableConditionalFormatTypeTable = CreatePivotTableConditionalFormatTypeTable();
		public static Dictionary<string, ConditionalFormatType> reversePivotTableConditionalFormatTypeTable = DictionaryUtils.CreateBackTranslationTable(pivotTableConditionalFormatTypeTable);
		static Dictionary<ConditionalFormatType, string> CreatePivotTableConditionalFormatTypeTable() {
			Dictionary<ConditionalFormatType, string> result = new Dictionary<ConditionalFormatType, string>();
			result.Add(ConditionalFormatType.All, "all");
			result.Add(ConditionalFormatType.Column, "column");
			result.Add(ConditionalFormatType.None, "none");
			result.Add(ConditionalFormatType.Row, "row");
			return result;
		}
		readonly Worksheet worksheet;
		readonly PivotConditionalFormatCollection conditionalFormats;
		PivotConditionalFormat conditionalFormat;
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pivotAreas", OnPivotAreas);
			return result;
		}
		#endregion
		public PivotTableConditionalFormatDestination(SpreadsheetMLBaseImporter importer, PivotConditionalFormatCollection conditionalFormats, Worksheet worksheet)
			: base(importer) {
			this.worksheet = worksheet;
			this.conditionalFormats = conditionalFormats;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
				conditionalFormat = new PivotConditionalFormat(Worksheet.Workbook);
				ConditionalFormats.Add(conditionalFormat);
				ConditionalFormat.SetScopeCore((int)Importer.GetWpEnumValue<ConditionalFormatScope>(reader, "scope", pivotTableConditionalFormatScopeTable, ConditionalFormatScope.Selection));
				ConditionalFormat.SetTypeCore((int)Importer.GetWpEnumValue<ConditionalFormatType>(reader, "type", pivotTableConditionalFormatTypeTable, ConditionalFormatType.None));
				ConditionalFormat.SetPriorityCore(Importer.GetWpSTIntegerValue(reader, "priority"));
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public Worksheet Worksheet { get { return worksheet; } }
		public PivotConditionalFormatCollection ConditionalFormats { get { return conditionalFormats; } }
		public PivotConditionalFormat ConditionalFormat { get { return conditionalFormat; } set { conditionalFormat = value; } }
		#endregion
		static PivotTableConditionalFormatDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTableConditionalFormatDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnPivotAreas(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTableConditionalFormatDestination self = GetThis(importer);
			return new PivotTablePivotAreaCollectionDestination(importer, self.ConditionalFormat, self.Worksheet);
		}
		#endregion
	}
	#endregion
	#region PivotTablePivotAreaCollectionDestination
	public class PivotTablePivotAreaCollectionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly Worksheet worksheet;
		readonly PivotConditionalFormat conditionalFormat;
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pivotArea", OnPivotArea);
			return result;
		}
		#endregion
		public PivotTablePivotAreaCollectionDestination(SpreadsheetMLBaseImporter importer, PivotConditionalFormat conditionalFormat, Worksheet worksheet)
			: base(importer) {
			this.worksheet = worksheet;
			this.conditionalFormat = conditionalFormat;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public Worksheet Worksheet { get { return worksheet; } }
		public PivotConditionalFormat ConditionalFormat { get { return conditionalFormat; } }
		#endregion
		static PivotTablePivotAreaCollectionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTablePivotAreaCollectionDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnPivotArea(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePivotAreaCollectionDestination self = GetThis(importer);
			PivotArea pivotArea = new PivotArea(self.Worksheet.Workbook);
			self.ConditionalFormat.PivotAreas.AddCore(pivotArea);
			return new PivotTablePivotAreaDestination(importer, pivotArea, self.Worksheet);
		}
		#endregion
	}
	#endregion
}
