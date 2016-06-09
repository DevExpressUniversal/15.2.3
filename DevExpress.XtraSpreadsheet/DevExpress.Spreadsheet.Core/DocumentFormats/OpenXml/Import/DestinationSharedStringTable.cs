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
using System.Xml;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region SharedStringTableDestination
	public class SharedStringTableDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		protected static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("si", OnStringItem);
			return result;
		}
		static SharedStringTableDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (SharedStringTableDestination)importer.PeekDestination();
		}
		SharedStringTable sharedStringTable;
		public SharedStringTableDestination(SpreadsheetMLBaseImporter importer, SharedStringTable sharedStringTable)
			: base(importer) {
			Guard.ArgumentNotNull(sharedStringTable, "sharedStringTable");
			this.sharedStringTable = sharedStringTable;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public SharedStringTable SharedStringTable { get { return sharedStringTable; } }
		static Destination OnStringItem(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SharedStringTable table = GetThis(importer).SharedStringTable;
			SharedStringItemDestination result = new SharedStringItemDestination(importer, table);
			return result;
		}
	}
	#endregion
	public class InlineStringDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Element handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		protected static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("t", OnTextString);
			result.Add("r", OnRun);
			return result;
		}
		#endregion
		#region Handlers
		static Destination OnTextString(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new TextDestination(importer, GetThis(importer).PlainTextStringItem);
		}
		static Destination OnRun(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			InlineStringDestination thisDestination = GetThis(importer);
			FormattedStringItemPart itemPart = thisDestination.FormattedStringItem.AddNewFormattedStringItemPart();
			return new TextRunDestination(importer, itemPart);
		}
		#endregion
		readonly ISharedStringItem plainTextStringItem;
		readonly FormattedStringItem formattedStringItem;
		readonly CellDestination cellDestination;
		public InlineStringDestination(SpreadsheetMLBaseImporter importer, CellDestination cellDestination)
			: base(importer) {
			this.plainTextStringItem = new PlainTextStringItem();
			this.formattedStringItem = new FormattedStringItem(Importer.DocumentModel);
			this.cellDestination = cellDestination;
		}
		public ISharedStringItem PlainTextStringItem { get { return plainTextStringItem; } }
		public FormattedStringItem FormattedStringItem { get { return formattedStringItem; } }
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementClose(XmlReader reader) {
			if (FormattedStringItem.RunsCount > 0) {
				SharedStringTable sharedStringTable = Importer.DocumentModel.SharedStringTable;
				sharedStringTable.Add(FormattedStringItem);
				SharedStringIndex index = new SharedStringIndex(sharedStringTable.Count - 1);
				cellDestination.SetSharedStringIndex(index);
			}
			else
				cellDestination.Value = PlainTextStringItem.Content;
		}
		static InlineStringDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (InlineStringDestination)importer.PeekDestination();
		}
	}
	#region TextRunDestination
	public class TextRunDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		protected static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("rPr", OnRunProperties);
			result.Add("t", OnTextString);
			return result;
		}
		static TextRunDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (TextRunDestination)importer.PeekDestination();
		}
		readonly FormattedStringItemPart sharedStringItem;
		public TextRunDestination(SpreadsheetMLBaseImporter importer, FormattedStringItemPart sharedStringItem)
			: base(importer) {
			this.sharedStringItem = sharedStringItem;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public FormattedStringItemPart SharedStringItem { get { return sharedStringItem; } }
		static Destination OnRunProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new RunPropertiesDestination(importer, GetThis(importer).SharedStringItem.Font );
		}
		static Destination OnTextString(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new TextDestination(importer, GetThis(importer).SharedStringItem);
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			sharedStringItem.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			sharedStringItem.EndUpdate();
		}
	}
	#endregion
	public class SharedStringItemDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		protected static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("t", OnTextString);	  
			result.Add("r", OnRun);			 
			return result;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		static SharedStringItemDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (SharedStringItemDestination)importer.PeekDestination();
		}
		FormattedStringItem formattedStringItem;
		PlainTextStringItem plainTextStringItem;
		SharedStringTable sharedStringTable;
		internal SharedStringItemDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
			this.formattedStringItem = new FormattedStringItem(Importer.DocumentModel);
			this.plainTextStringItem = new PlainTextStringItem();
			this.sharedStringTable = null;
		}
		public SharedStringItemDestination(SpreadsheetMLBaseImporter importer, SharedStringTable sharedStringTable)
			: this(importer) {
			Guard.ArgumentNotNull(sharedStringTable, "sharedStringTable");
			this.sharedStringTable = sharedStringTable;
		}
		public FormattedStringItem FormattedStringItem { get { return formattedStringItem; } }
		internal List<FormattedStringItemPart> InnerRuns { get { return formattedStringItem.Items; } }
		public PlainTextStringItem PlainTextStringItem { get { return plainTextStringItem; } }
		public SharedStringTable SharedStringTable { get { return sharedStringTable; } }
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			if (InnerRuns.Count != 0)
				AddStringItemToSharedStringTable(formattedStringItem);
			else
				AddStringItemToSharedStringTable(plainTextStringItem);
		}
		protected internal virtual bool IfWasEmptySharedStringItem() {
			return InnerRuns.Count == 0
				&& !String.IsNullOrEmpty(plainTextStringItem.Content);
		}
		static Destination OnRun(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SharedStringItemDestination thisDestination = GetThis(importer);
			FormattedStringItemPart itemPart = thisDestination.FormattedStringItem.AddNewFormattedStringItemPart();
			return new TextRunDestination(importer, itemPart);
		}
		static Destination OnTextString(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			SharedStringItemDestination thisDestination = GetThis(importer);
			if (!String.IsNullOrEmpty(thisDestination.PlainTextStringItem.Content))
				importer.ThrowInvalidFile("Null or empty shared string content");
			return new TextDestination(importer, thisDestination.PlainTextStringItem);
		}
		protected internal virtual void AddStringItemToSharedStringTable(FormattedStringItem item) {
			SharedStringTable.Add(item);
		}
		protected internal virtual void AddStringItemToSharedStringTable(PlainTextStringItem item) {
			SharedStringTable.Add(item);
		}
	}
}
