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
	#region PivotTablePivotFormatsDestination
	public class PivotTablePivotFormatCollectionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotFormatCollection formats;
		readonly Worksheet worksheet;
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("format", OnPivotFormat);
			return result;
		}
		#endregion
		public PivotTablePivotFormatCollectionDestination(SpreadsheetMLBaseImporter importer, PivotFormatCollection formats, Worksheet worksheet)
			: base(importer) {
			this.formats = formats;
			this.worksheet = worksheet;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotFormatCollection Formats { get { return formats; } }
		public Worksheet Worksheet { get { return worksheet; } }
		#endregion
		static PivotTablePivotFormatCollectionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTablePivotFormatCollectionDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnPivotFormat(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePivotFormatCollectionDestination self = GetThis(importer);
			return new PivotTablePivotFormatDestination(importer, self.Formats, self.Worksheet);
		}
		#endregion
	}
	#endregion
	#region PivotTablePivotFormatDestination
	public class PivotTablePivotFormatDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		public static Dictionary<FormatAction, string> pivotTableFormatActionTable = CreatePivotTableFormatActionTable();
		public static Dictionary<string, FormatAction> reversePivotTableFormatActionTable = DictionaryUtils.CreateBackTranslationTable(pivotTableFormatActionTable);
		static Dictionary<FormatAction, string> CreatePivotTableFormatActionTable() {
			Dictionary<FormatAction, string> result = new Dictionary<FormatAction, string>();
			result.Add(FormatAction.Blank, "blank");
			result.Add(FormatAction.Drill, "drill");
			result.Add(FormatAction.Formatting, "formatting");
			result.Add(FormatAction.Formula, "formula");
			return result;
		}
		readonly PivotFormatCollection formats;
		readonly Worksheet worksheet;
		PivotFormat pivotFormat;
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pivotArea", OnPivotArea);
			return result;
		}
		#endregion
		public PivotTablePivotFormatDestination(SpreadsheetMLBaseImporter importer, PivotFormatCollection formats, Worksheet worksheet)
			: base(importer) {
			this.worksheet = worksheet;
			this.formats = formats;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
				int value = Importer.GetWpSTIntegerValue(reader, "dxfId", -1);
				this.pivotFormat = new PivotFormat(Worksheet.Workbook, Importer.StyleSheet.GetDifferentialFormatIndex(value));
				this.formats.Add(pivotFormat);
				PivotFormat.SetFormatActionCore((int)Importer.GetWpEnumValue<FormatAction>(reader, "action", pivotTableFormatActionTable, FormatAction.Formatting));
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public PivotFormatCollection Formats { get { return formats; } }
		public PivotFormat PivotFormat { get { return pivotFormat; } }
		public Worksheet Worksheet { get { return worksheet; } }
		#endregion
		static PivotTablePivotFormatDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTablePivotFormatDestination)importer.PeekDestination();
		}
		#region Handlers
		static Destination OnPivotArea(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTablePivotFormatDestination self = GetThis(importer);
			return new PivotTablePivotAreaDestination(importer, self.PivotFormat.PivotArea, self.Worksheet);
		}
		#endregion
	}
	#endregion
}
