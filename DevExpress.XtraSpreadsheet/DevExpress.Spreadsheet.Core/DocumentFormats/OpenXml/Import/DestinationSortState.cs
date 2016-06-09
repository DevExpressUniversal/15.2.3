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
using DevExpress.Utils;
using System.Xml;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.Office;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region SortStateDestination
	public class SortStateDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("sortCondition", OnSortCondition);
			return result;
		}
		static Dictionary<SortMethod, string> sortMethodTable = OpenXmlExporter.SortMethodTable;
		static SortStateDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (SortStateDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly SortState sortState;
		readonly Worksheet sheet;
		#endregion
		public SortStateDestination(SpreadsheetMLBaseImporter importer, SortState sortState, Worksheet sheet)
			: base(importer) {
			Guard.ArgumentNotNull(sortState, "sortState");
			Guard.ArgumentNotNull(sheet, "sheet");
			this.sortState = sortState;
			this.sheet = sheet;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal SortState SortState { get { return sortState; } }
		protected internal Worksheet Sheet { get { return sheet; } }
		#endregion
		static Destination OnSortCondition(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new SortConditionDestination(importer, GetThis(importer).SortState);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CellRange sortRange = Importer.ReadCellRange(reader, "ref", Sheet);
			if (sortRange == null)
				Importer.ThrowInvalidFile("SortState ref absent");
			SortState.BeginUpdate();
			try {
				SortState.SortRange = sortRange;
				SortState.SortByColumns = Importer.GetWpSTOnOffValue(reader, "columnSort", false);
				SortState.CaseSensitive = Importer.GetWpSTOnOffValue(reader, "caseSensitive", false);
				SortState.SortMethod = Importer.GetWpEnumValue<SortMethod>(reader, "sortMethod", sortMethodTable, SortMethod.None);
			}
			finally {
				SortState.EndUpdate();
			}
		}
	}
	#endregion
	#region SortConditionDestination
	public class SortConditionDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Static members
		static Dictionary<SortBy, string> sortByTable = OpenXmlExporter.SortByTable;
		static Dictionary<IconSetType, string> iconSetTypeTable = OpenXmlExporter.IconSetTypeTable;
		#endregion
		#region Fields
		readonly SortState sortState;
		#endregion
		public SortConditionDestination(SpreadsheetMLBaseImporter importer, SortState sortState)
			: base(importer) {
			this.sortState = sortState;
		}
		#region Properties
		protected internal SortState SortState { get { return sortState; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			CellRange sortReference = Importer.ReadCellRange(reader, "ref", SortState.SortRange.Worksheet);
			if (sortReference == null)
				Importer.ThrowInvalidFile("Sort condition ref absent");
			SortCondition sortCondition = new SortCondition(Importer.CurrentWorksheet, sortReference);
			sortCondition.BeginUpdate();
			try {
				sortCondition.Descending = Importer.GetWpSTOnOffValue(reader, "descending", false);
				sortCondition.SortBy = Importer.GetWpEnumValue<SortBy>(reader, "sortBy", sortByTable, SortBy.Value);
				sortCondition.IconSet = Importer.GetWpEnumValue<IconSetType>(reader, "iconSet", iconSetTypeTable, IconSetType.None);
				int iconId = Importer.GetIntegerValue(reader, "iconId", Int32.MinValue);
				if (iconId != Int32.MinValue)
					sortCondition.IconId = iconId;
				string customList = Importer.ReadAttribute(reader, "customList");
				if (!String.IsNullOrEmpty(customList))
					sortCondition.CustomList = customList;
			}
			finally {
				sortCondition.EndUpdate();
			}
			SortState.SortConditions.AddCore(sortCondition);
		}
	}
	#endregion
}
