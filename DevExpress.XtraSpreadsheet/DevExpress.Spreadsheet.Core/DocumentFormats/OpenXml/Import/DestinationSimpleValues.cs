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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region OnOffValueDestination
	public class OnOffValueDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		Action<bool> action;
		string attributeName;
		bool defaultValue;
		public OnOffValueDestination(SpreadsheetMLBaseImporter importer, Action<bool> action, string attributeName, bool defaultValue)
			: base(importer) {
			this.action = action;
			this.attributeName = attributeName;
			this.defaultValue = defaultValue;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			this.action(Importer.GetOnOffValue(reader, this.attributeName, this.defaultValue));
		}
	}
	#endregion
	#region IntegerValueDestination
	public class IntegerValueDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		Action<int> action;
		string attributeName;
		int defaultValue;
		public IntegerValueDestination(SpreadsheetMLBaseImporter importer, Action<int> action, string attributeName, int defaultValue)
			: base(importer) {
			this.action = action;
			this.attributeName = attributeName;
			this.defaultValue = defaultValue;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			this.action(Importer.GetIntegerValue(reader, this.attributeName, this.defaultValue));
		}
	}
	#endregion
	#region StringValueDestination
	public class StringValueDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		Action<string> action;
		string attributeName;
		public StringValueDestination(SpreadsheetMLBaseImporter importer, Action<string> action, string attributeName)
			: base(importer) {
			this.action = action;
			this.attributeName = attributeName;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			this.action(reader.GetAttribute(this.attributeName));
		}
	}
	#endregion
	#region FloatValueDestination
	public class FloatValueDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		Action<float> action;
		string attributeName;
		public FloatValueDestination(SpreadsheetMLBaseImporter importer, Action<float> action, string attributeName)
			: base(importer) {
			this.action = action;
			this.attributeName = attributeName;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			this.action(Importer.GetWpSTFloatValue(reader, this.attributeName));
		}
	}
	#endregion
	#region EnumValueDestination
	public class EnumValueDestination<T> : LeafElementDestination<SpreadsheetMLBaseImporter> where T : struct {
		readonly Dictionary<string, T> enumTable;
		readonly Action<T> action;
		readonly string attributeName;
		readonly T defaultValue;
		public EnumValueDestination(SpreadsheetMLBaseImporter importer, Dictionary<string, T> enumTable, Action<T> action, string attributeName, T defaultValue)
			: base(importer) {
			this.action = action;
			this.attributeName = attributeName;
			this.enumTable = enumTable;
			this.defaultValue = defaultValue;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			T value = Importer.GetWpEnumValue<T>(reader, attributeName, enumTable, defaultValue);
			this.action(value);
		}
	}
	#endregion
	#region OnOffValueTagDestination
	public class OnOffValueTagDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		Func<bool,bool> action;
		bool defaultValue;
		public OnOffValueTagDestination(SpreadsheetMLBaseImporter importer, Func<bool, bool> action, bool defaultValue)
			: base(importer) {
			this.action = action;
			this.defaultValue = defaultValue;
		}
		public override bool ProcessText(XmlReader reader) {
			return action(Importer.GetOnOffValue(reader.Value.ToLower(), defaultValue));
		}
	}
	#endregion
	#region IntegerValueTagDestination
	public class IntegerValueTagDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		Func<int, bool> action;
		public IntegerValueTagDestination(SpreadsheetMLBaseImporter importer, Func<int, bool> action)
			: base(importer) {
			this.action = action;
		}
		public override bool ProcessText(XmlReader reader) {
			int value = Importer.GetIntegerValue(reader.Value, NumberStyles.Integer, -1);
			if (value < 0)
				Importer.ThrowInvalidFile("Integer value is not specified");
			return action(value);
		}
	}
	#endregion
	#region StringValueTagDestination
	public class StringValueTagDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		string value = string.Empty;
		Func<string, bool> action;
		public StringValueTagDestination(SpreadsheetMLBaseImporter importer, Func<string, bool> action)
			: base(importer) {
			this.action = action;
		}
		public override bool ProcessText(XmlReader reader) {
			value = reader.Value;
			if (!String.IsNullOrEmpty(value))
				value = Importer.DecodeXmlChars(value);
			return true;
		}
		public override bool ShouldProcessWhitespaces(XmlReader reader) {
			return true;
		}
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			action(value);
		}
	}
	#endregion
}
