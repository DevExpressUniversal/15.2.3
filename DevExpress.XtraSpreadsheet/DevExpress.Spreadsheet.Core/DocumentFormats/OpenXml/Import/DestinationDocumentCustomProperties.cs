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
using System.Globalization;
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Spreadsheet;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DocumentCustomPropertiesDestination
	public class DocumentCustomPropertiesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("property", OnProperty);
			return result;
		}
		#endregion
		public DocumentCustomPropertiesDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnProperty(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DocumentCustomPropertyDestination(importer);
		}
	}
	#endregion
	#region DocumentCustomPropertyDestination
	public class DocumentCustomPropertyDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("lpstr", OnUnicodeStringProperty);
			result.Add("lpwstr", OnUnicodeStringProperty);
			result.Add("bstr", OnUnicodeStringProperty);
			result.Add("date", OnDateTimeProperty);
			result.Add("filetime", OnDateTimeProperty);
			result.Add("i1", OnIntegerProperty);
			result.Add("i2", OnIntegerProperty);
			result.Add("i4", OnIntegerProperty);
			result.Add("i8", OnIntegerProperty);
			result.Add("int", OnIntegerProperty);
			result.Add("ui1", OnIntegerProperty);
			result.Add("ui2", OnIntegerProperty);
			result.Add("ui4", OnIntegerProperty);
			result.Add("ui8", OnIntegerProperty);
			result.Add("uint", OnIntegerProperty);
			result.Add("bool", OnBooleanProperty);
			result.Add("cy", OnDoubleProperty);
			result.Add("r4", OnDoubleProperty);
			result.Add("r8", OnDoubleProperty);
			result.Add("decimal", OnDoubleProperty);
			return result;
		}
		#endregion
		string name;
		CellValue value;
		public DocumentCustomPropertyDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		ModelDocumentCustomProperties Properties { get { return Importer.DocumentModel.DocumentCustomProperties; } }
		static DocumentCustomPropertyDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DocumentCustomPropertyDestination)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			name = Importer.ReadAttribute(reader, "name");
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (!String.IsNullOrEmpty(name) && value != null)
				Properties[name] = value;
		}
		void AssignValue(CellValue value) {
			this.value = value;
		}
		static Destination OnUnicodeStringProperty(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new UnicodeStringPropertyValueDestination(importer, GetThis(importer).AssignValue);
		}
		static Destination OnDateTimeProperty(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DateTimePropertyValueDestination(importer, GetThis(importer).AssignValue);
		}
		static Destination OnIntegerProperty(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new IntegerPropertyValueDestination(importer, GetThis(importer).AssignValue);
		}
		static Destination OnBooleanProperty(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new BooleanPropertyValueDestination(importer, GetThis(importer).AssignValue);
		}
		static Destination OnDoubleProperty(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DoublePropertyValueDestination(importer, GetThis(importer).AssignValue);
		}
	}
	#endregion
	public delegate void AssignCellValuePropertyDelegate(CellValue value);
	#region UnicodeStringPropertyValueDestination
	public class UnicodeStringPropertyValueDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly AssignCellValuePropertyDelegate action;
		public UnicodeStringPropertyValueDestination(SpreadsheetMLBaseImporter importer, AssignCellValuePropertyDelegate action)
			: base(importer) {
			Guard.ArgumentNotNull(action, "action");
			this.action = action;
		}
		public override bool ProcessText(XmlReader reader) {
			action(reader.Value);
			return true;
		}
	}
	#endregion
	#region IntegerPropertyValueDestination
	public class IntegerPropertyValueDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly AssignCellValuePropertyDelegate action;
		public IntegerPropertyValueDestination(SpreadsheetMLBaseImporter importer, AssignCellValuePropertyDelegate action)
			: base(importer) {
			Guard.ArgumentNotNull(action, "action");
			this.action = action;
		}
		public override bool ProcessText(XmlReader reader) {
			string text = reader.Value;
			if (!String.IsNullOrEmpty(text)) {
				long value;
				if (Int64.TryParse(text, NumberStyles.Integer, Importer.DocumentModel.Culture, out value))
					action((double)value);
			}
			return true;
		}
	}
	#endregion
	#region DoublePropertyValueDestination
	public class DoublePropertyValueDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly AssignCellValuePropertyDelegate action;
		public DoublePropertyValueDestination(SpreadsheetMLBaseImporter importer, AssignCellValuePropertyDelegate action)
			: base(importer) {
			Guard.ArgumentNotNull(action, "action");
			this.action = action;
		}
		public override bool ProcessText(XmlReader reader) {
			string text = reader.Value;
			if (!String.IsNullOrEmpty(text)) {
				double value;
				if (Double.TryParse(text, System.Globalization.NumberStyles.Float, Importer.DocumentModel.Culture, out value))
					action(value);
			}
			return true;
		}
	}
	#endregion
	#region BooleanPropertyValueDestination
	public class BooleanPropertyValueDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly AssignCellValuePropertyDelegate action;
		public BooleanPropertyValueDestination(SpreadsheetMLBaseImporter importer, AssignCellValuePropertyDelegate action)
			: base(importer) {
			Guard.ArgumentNotNull(action, "action");
			this.action = action;
		}
		public override bool ProcessText(XmlReader reader) {
			string text = reader.Value;
			if (text == "true")
				action(true);
			else if (text == "false")
				action(false);
			return true;
		}
	}
	#endregion
	#region DateTimePropertyValueDestination
	public class DateTimePropertyValueDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly AssignCellValuePropertyDelegate action;
		public DateTimePropertyValueDestination(SpreadsheetMLBaseImporter importer, AssignCellValuePropertyDelegate action)
			: base(importer) {
			Guard.ArgumentNotNull(action, "action");
			this.action = action;
		}
		public override bool ProcessText(XmlReader reader) {
			string text = reader.Value;
			if (!String.IsNullOrEmpty(text)) {
				DateTime dateTime;
				if (DateTime.TryParse(text, Importer.DocumentModel.Culture, DateTimeStyles.None, out dateTime))
					action(dateTime);
			}
			return true;
		}
	}
	#endregion
}
