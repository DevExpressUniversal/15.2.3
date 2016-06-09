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
using DevExpress.SpreadsheetSource.Xlsx.Import.Internal;
using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Xml;
namespace DevExpress.SpreadsheetSource.Xlsx.Import {
	#region SharedStringsDestination
	public class SharedStringsDestination : ElementDestination<XlsxSpreadsheetSourceImporter> {
		static readonly ElementHandlerTable<XlsxSpreadsheetSourceImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<XlsxSpreadsheetSourceImporter> CreateElementHandlerTable() {
			ElementHandlerTable<XlsxSpreadsheetSourceImporter> result = new ElementHandlerTable<XlsxSpreadsheetSourceImporter>();
			result.Add("si", OnSharedString);
			return result;
		}
		public SharedStringsDestination(XlsxSpreadsheetSourceImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable<XlsxSpreadsheetSourceImporter> ElementHandlerTable { get { return handlerTable; } }
		static Destination OnSharedString(XlsxSpreadsheetSourceImporter importer, XmlReader reader) {
			return SharedStringDestination.GetInstance(importer);
		}
		public override void ProcessElementClose(XmlReader reader) {
			SharedStringDestination.ClearInstance();
		}
	}
	#endregion
	#region SharedStringDestination
	public class SharedStringDestination : ElementDestination<XlsxSpreadsheetSourceImporter> {
		static readonly ElementHandlerTable<XlsxSpreadsheetSourceImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<XlsxSpreadsheetSourceImporter> CreateElementHandlerTable() {
			ElementHandlerTable<XlsxSpreadsheetSourceImporter> result = new ElementHandlerTable<XlsxSpreadsheetSourceImporter>();
			result.Add("t", OnText);
			result.Add("r", OnRun);
			return result;
		}
		[ThreadStatic]
		static SharedStringDestination instance;
		List<SharedStringItem> runs = new List<SharedStringItem>();
		SharedStringItem stringItem = new SharedStringItem();
		public SharedStringDestination(XlsxSpreadsheetSourceImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable<XlsxSpreadsheetSourceImporter> ElementHandlerTable { get { return handlerTable; } }
		static SharedStringDestination GetThis(XlsxSpreadsheetSourceImporter importer) {
			return (SharedStringDestination)importer.PeekDestination();
		}
		static Destination OnText(XlsxSpreadsheetSourceImporter importer, XmlReader reader) {
			return TextDestination.GetInstance(importer, GetThis(importer).stringItem);
		}
		static Destination OnRun(XlsxSpreadsheetSourceImporter importer, XmlReader reader) {
			return TextRunDestination.GetInstance(importer, GetThis(importer).runs);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (runs == null || runs.Count > 0)
				MergeRuns();
			else
				ApplyText(stringItem.Text);
		}
		void MergeRuns() {
			string result = String.Empty;
			foreach (SharedStringItem item in runs) {
				result += item.Text;
			}
			ApplyText(result);
		}
		protected virtual void ApplyText(string item) {
			item = item == null ? String.Empty : item;
			Importer.Source.SharedStrings.Add(item);
		}
		public static SharedStringDestination GetInstance(XlsxSpreadsheetSourceImporter importer) {
			if (instance == null || instance.Importer != importer)
				instance = new SharedStringDestination(importer);
			else
				instance.Reset();
			return instance;
		}
		void Reset() {
			runs.Clear();
			stringItem.Text = null;
		}
		public static void ClearInstance() {
			instance = null;
			TextDestination.ClearInstance();
			TextRunDestination.ClearInstance();
		}
	}
	#endregion
	#region TextDestination
	public class TextDestination : LeafElementDestination<XlsxSpreadsheetSourceImporter> {
		SharedStringItem stringItem;
		[ThreadStatic]
		static TextDestination instance;
		public TextDestination(XlsxSpreadsheetSourceImporter importer, SharedStringItem stringItem)
			: base(importer) {
			Guard.ArgumentNotNull(stringItem, "stringItem");
			this.stringItem = stringItem;
		}
		public override bool ProcessText(XmlReader reader) {
			string text = reader.Value;
			if (!String.IsNullOrEmpty(text))
				this.stringItem.Text = Importer.DecodeXmlChars(text);
			return true;
		}
		public static TextDestination GetInstance(XlsxSpreadsheetSourceImporter importer, SharedStringItem stringItem) {
			if (instance == null || instance.Importer != importer)
				instance = new TextDestination(importer, stringItem);
			else
				instance.stringItem = stringItem;
			return instance;
		}
		public static void ClearInstance() {
			if (instance != null)
				instance.stringItem = null;
			instance = null;
		}
	}
	#endregion
	#region TextRunDestination
	public class TextRunDestination : ElementDestination<XlsxSpreadsheetSourceImporter> {
		static readonly ElementHandlerTable<XlsxSpreadsheetSourceImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<XlsxSpreadsheetSourceImporter> CreateElementHandlerTable() {
			ElementHandlerTable<XlsxSpreadsheetSourceImporter> result = new ElementHandlerTable<XlsxSpreadsheetSourceImporter>();
			result.Add("t", OnText);
			return result;
		}
		[ThreadStatic]
		static TextRunDestination instance;
		List<SharedStringItem> stringItems;
		public TextRunDestination(XlsxSpreadsheetSourceImporter importer, List<SharedStringItem> stringItems)
			: base(importer) {
			Guard.ArgumentNotNull(stringItems, "stringItems");
			this.stringItems = stringItems;
		}
		protected internal override ElementHandlerTable<XlsxSpreadsheetSourceImporter> ElementHandlerTable { get { return handlerTable; } }
		static TextRunDestination GetThis(XlsxSpreadsheetSourceImporter importer) {
			return (TextRunDestination)importer.PeekDestination();
		}
		static Destination OnText(XlsxSpreadsheetSourceImporter importer, XmlReader reader) {
			SharedStringItem item = new SharedStringItem();
			GetThis(importer).stringItems.Add(item);
			return TextDestination.GetInstance(importer, item);
		}
		public static TextRunDestination GetInstance(XlsxSpreadsheetSourceImporter importer, List<SharedStringItem> stringItems) {
			if (instance == null || instance.Importer != importer)
				instance = new TextRunDestination(importer, stringItems);
			else
				instance.stringItems = stringItems;
			return instance;
		}
		public static void ClearInstance() {
			if (instance != null)
				instance.stringItems = null;
			instance = null;
		}
	}
	#endregion
}
namespace DevExpress.SpreadsheetSource.Xlsx.Import.Internal {
	#region SharedStringItem
	public class SharedStringItem {
		public string Text { get; set; }
	}
	#endregion
}
