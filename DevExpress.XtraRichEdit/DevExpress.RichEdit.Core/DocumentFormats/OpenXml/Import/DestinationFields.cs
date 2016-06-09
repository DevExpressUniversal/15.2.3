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
using System.Xml;
using DevExpress.XtraRichEdit.Export.OpenXml;
using DevExpress.XtraRichEdit.Model;
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region FieldDestinationBase (abstract class)
	public abstract class FieldDestinationBase : ElementDestination {
		readonly ImportFieldHelper importFieldHelper;
		protected FieldDestinationBase(WordProcessingMLBaseImporter importer)
			: base(importer) {
			this.importFieldHelper = new ImportFieldHelper(Importer.PieceTable); 
		}
		protected ImportFieldHelper ImportFieldHelper { get { return importFieldHelper; } }
		protected internal virtual void ProcessFieldBegin(bool disableUpdate, bool locked) {
			ImportFieldInfo fieldInfo = new ImportFieldInfo(Importer.PieceTable) { DisableUpdate = disableUpdate, Locked = locked };
			importFieldHelper.ProcessFieldBegin(fieldInfo, Importer.Position);
			Importer.FieldInfoStack.Push(fieldInfo);
		}
		protected internal virtual void ProcessFieldSeparator() {
			ImportFieldInfo fieldInfo = Importer.FieldInfoStack.Peek();
			importFieldHelper.ProcessFieldSeparator(fieldInfo, Importer.Position);
		}
		protected internal virtual void ProcessFieldEnd() {
			ImportFieldInfo fieldInfo = Importer.FieldInfoStack.Pop();
			importFieldHelper.ProcessFieldEnd(fieldInfo, Importer.Position);
			if (Importer.FieldInfoStack.Count > 0)
				fieldInfo.Field.Parent = Importer.FieldInfoStack.Peek().Field;
		}
	}
	#endregion
	#region FieldSimpleDestination
	public class FieldSimpleDestination : FieldDestinationBase {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("r", OnRun);
			result.Add("fldSimple", OnFieldSimple);
			result.Add("hyperlink", OnHyperlink);
			result.Add("sdt", OnStructuredDocument);
			result.Add("customXml", OnCustomXml);
			return result;
		}
		string fieldCode;
		public FieldSimpleDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			this.fieldCode = reader.GetAttribute("instr", Importer.WordProcessingNamespaceConst);
			if (String.IsNullOrEmpty(fieldCode))
				return;
			string fldLockAttr = reader.GetAttribute("fldLock", Importer.WordProcessingNamespaceConst);
			bool locked = !String.IsNullOrEmpty(fldLockAttr) ? Importer.ConvertToBool(fldLockAttr) : false;
			ProcessFieldBegin(false, locked);
			Importer.PieceTable.InsertTextCore(Importer.Position, fieldCode);
			ProcessFieldSeparator();
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (!String.IsNullOrEmpty(fieldCode))
				ProcessFieldEnd();
		}
		static Destination OnRun(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateRunDestination();
		}
		static Destination OnFieldSimple(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FieldSimpleDestination(importer);
		}
		static Destination OnHyperlink(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new HyperlinkDestination(importer);
		}
		static Destination OnStructuredDocument(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new StructuredDocumentDestination(importer);
		}
		static Destination OnCustomXml(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new CustomXmlDestination(importer);
		}
	}
	#endregion
	#region FieldCharDestination
	public class FieldCharDestination : FieldDestinationBase {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			return result;
		}
		public FieldCharDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			string type = reader.GetAttribute("fldCharType", Importer.WordProcessingNamespaceConst);
			string attr = reader.GetAttribute("disableUpdate", Importer.WordProcessingNamespaceConst);
			bool disableUpdate = !string.IsNullOrEmpty(attr) ? Importer.ConvertToBool(attr) : false;
			string lockedAttr = reader.GetAttribute("fldLock", Importer.WordProcessingNamespaceConst);
			bool locked = !string.IsNullOrEmpty(lockedAttr) ? Importer.ConvertToBool(lockedAttr) : false;
			switch (type) {
				case "begin":
					ProcessFieldBegin(disableUpdate, locked);
					break;
				case "separate":
					ProcessFieldSeparator();
					break;
				case "end":
					ProcessFieldEnd();
					break;
				default:
					break;
			}
		}
	}
	#endregion
	public delegate void HyperlinkAttributeHandler(WordProcessingMLBaseImporter importer, HyperlinkInfo info, string value);
	public class HyperlinkAttributeHandlerTable : Dictionary<string, HyperlinkAttributeHandler> { }
	#region HyperlinkDestination
	public class HyperlinkDestination : FieldDestinationBase {
		static readonly HyperlinkAttributeHandlerTable attributeHandlerTable = CreateAttributeHandlerTable();
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("r", OnRun);
			result.Add("fldSimple", OnFieldSimple);
			result.Add("hyperlink", OnHyperlink);
			result.Add("bookmarkStart", OnBookmarkStart);
			result.Add("bookmarkEnd", OnBookmarkEnd);
			result.Add("permStart", OnRangePermissionStart);
			result.Add("permEnd", OnRangePermissionEnd);
			result.Add("sdt", OnStructuredDocument);
			result.Add("customXml", OnCustomXml);
			return result;
		}
		static HyperlinkAttributeHandlerTable CreateAttributeHandlerTable() {
			HyperlinkAttributeHandlerTable result = new HyperlinkAttributeHandlerTable();
			result.Add("id", OnIdAttribute);
			result.Add("anchor", OnAnchorAttribute);
			result.Add("tgtFrame", OnTargetFrameAttribute);
			result.Add("tooltip", OnTooltipAttribute);
			result.Add("history", OnHistoryAttribute);
			result.Add("docLocation", OnDocLocationAttribute);
			return result;
		}
		public HyperlinkDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal virtual HyperlinkAttributeHandlerTable AttributeHandlerTable { get { return attributeHandlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			if (!DocumentFormatsHelper.ShouldInsertHyperlink(DocumentModel))
				return;
			string attr = reader.GetAttribute("fldLock", Importer.WordProcessingNamespaceConst);
			bool locked = !string.IsNullOrEmpty(attr) ? Importer.ConvertToBool(attr) : false;
			ProcessFieldBegin(false, locked);
			HyperlinkInfo hyperlinkInfo = CreateHyperlinkInfo(reader);
			ImportFieldHelper.InsertHyperlinkInstruction(hyperlinkInfo, Importer.Position);
			ProcessFieldSeparator();
		}
		protected internal virtual HyperlinkInfo CreateHyperlinkInfo(XmlReader reader) {
			HyperlinkInfo hyperlinkInfo = new HyperlinkInfo();
			while (reader.MoveToNextAttribute()) {
				HyperlinkAttributeHandler handler;
				if (AttributeHandlerTable.TryGetValue(reader.LocalName, out handler) && reader.HasValue)
					handler(Importer, hyperlinkInfo, reader.Value);
				else if (reader.Name.IndexOf("xmlns") < 0)
					Importer.ThrowInvalidFile();
			}
			return hyperlinkInfo;
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (!DocumentFormatsHelper.ShouldInsertHyperlink(DocumentModel))
				return;
			ProcessFieldEnd();
		}
		static Destination OnRun(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateRunDestination();
		}
		static Destination OnFieldSimple(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FieldSimpleDestination(importer);
		}
		static Destination OnHyperlink(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new HyperlinkDestination(importer);
		}
		protected static Destination OnBookmarkStart(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateBookmarkStartElementDestination(reader);
		}
		protected static Destination OnBookmarkEnd(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateBookmarkEndElementDestination(reader);
		}
		protected static Destination OnRangePermissionStart(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new RangePermissionStartElementDestination(importer);
		}
		protected static Destination OnRangePermissionEnd(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new RangePermissionEndElementDestination(importer);
		}
		static Destination OnStructuredDocument(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new StructuredDocumentDestination(importer);
		}
		static Destination OnCustomXml(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new CustomXmlDestination(importer);
		}
		#region Attribute Handlers
		static void OnIdAttribute(WordProcessingMLBaseImporter importer, HyperlinkInfo info, string value) {
			OpenXmlRelation relation = ((OpenXmlImporter)importer).DocumentRelations.LookupRelationById(value);
			info.NavigateUri = relation.Target;
		}
		protected static void OnAnchorAttribute(WordProcessingMLBaseImporter importer, HyperlinkInfo info, string value) {
			info.Anchor = value;
		}
		protected static void OnTargetFrameAttribute(WordProcessingMLBaseImporter importer, HyperlinkInfo info, string value) {
			info.Target = value;
		}
		protected static void OnTooltipAttribute(WordProcessingMLBaseImporter importer, HyperlinkInfo info, string value) {
			info.ToolTip = value;
		}
		protected static void OnHistoryAttribute(WordProcessingMLBaseImporter importer, HyperlinkInfo info, string value) {
			info.Visited = !importer.ConvertToBool(value);
		}
		protected static void OnDocLocationAttribute(WordProcessingMLBaseImporter importer, HyperlinkInfo info, string value) {
		}
		#endregion
	}
	#endregion
}
