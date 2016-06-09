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
using DevExpress.Office;
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap.Core.Import {
	#region SnapBookmarkElementDestination
	public abstract class SnapBookmarkElementDestination : SnapLeafElementDestination {
		protected SnapBookmarkElementDestination(SnapImporter importer)
			: base(importer) {
		}
		protected internal ImportSnapBookmarkInfo BookmarkInfo { get; private set; }
		public override void ProcessElementOpen(XmlReader reader) {
			string id = Importer.ReadDxStringAttr("id", reader);
			if (String.IsNullOrEmpty(id))
				return;
			BookmarkInfo = GetBookmarkInfo(id, reader);
			AssignSnapBookmarkPosition(BookmarkInfo);
		}
		ImportSnapBookmarkInfo GetBookmarkInfo(string id, XmlReader reader) {
			ImportSnapBookmarkInfo snapBookmark;
			if (!Importer.SnapBookmarks.TryGetValue(id, out snapBookmark)) {
				snapBookmark = new ImportSnapBookmarkInfo(id);
				snapBookmark.ParentId = Importer.ReadDxStringAttr("parentId", reader);
				snapBookmark.TemplateIntervalId = Importer.ReadDxStringAttr("templateIntervalId", reader);
				snapBookmark.FieldContextId = Importer.ReadDxIntAttr("fieldContextId", reader);
				snapBookmark.HeaderBookmarkId = Importer.ReadDxStringAttr("headerBookmarkId", reader);
				snapBookmark.FooterBookmarkId = Importer.ReadDxStringAttr("footerBookmarkId", reader);
				snapBookmark.TemplateInfo = new ImportSnapTemplateInfo();
				Importer.SnapBookmarks.Add(id, snapBookmark);
			}
			return snapBookmark;
		}
		protected internal abstract void AssignSnapBookmarkPosition(ImportSnapBookmarkInfo bookmark);
	}
	#endregion
	#region SnapBookmarkStartElementDestination
	public class SnapBookmarkStartElementDestination : SnapBookmarkElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHadlerTable();
		static ElementHandlerTable CreateElementHadlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("firstGroupBookmark", OnFirstGroupBookmark);
			result.Add("lastGroupBookmark", OnLastGroupBookmark);
			result.Add("firstListBookmark", OnFirstListBookmark);
			result.Add("lastListBookmark", OnLastListBookmark);
			result.Add("fieldInGroupCount", OnFieldInGroupCount);
			result.Add("firstGroupIndex", OnFirstGroupIndex);
			result.Add("lastGroupIndex", OnLastGroupIndex);
			result.Add("templateType", OnTemplateType);
			return result;
		}
		public SnapBookmarkStartElementDestination(SnapImporter importer)
			: base(importer) {
		}
		protected internal override void AssignSnapBookmarkPosition(ImportSnapBookmarkInfo bookmark) {
			bookmark.Start = Importer.Position.LogPosition;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnFirstGroupBookmark(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FirstGroupBookmarkDestination((SnapImporter)importer, GetTemplateIntervalInfo(importer));
		}
		static Destination OnLastGroupBookmark(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new LastGroupBookmarkDestination((SnapImporter)importer, GetTemplateIntervalInfo(importer));
		}
		static Destination OnFirstListBookmark(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FirstListBookmarkDestination((SnapImporter)importer, GetTemplateIntervalInfo(importer));
		}
		static Destination OnLastListBookmark(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new LastListBookmarkDestination((SnapImporter)importer, GetTemplateIntervalInfo(importer));
		}
		static Destination OnFieldInGroupCount(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FieldInGroupCountDestination((SnapImporter)importer, GetTemplateIntervalInfo(importer));
		}
		static Destination OnFirstGroupIndex(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FirstGroupIndexDestination((SnapImporter)importer, GetTemplateIntervalInfo(importer));
		}
		static Destination OnLastGroupIndex(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new LastGroupIndexDestination((SnapImporter)importer, GetTemplateIntervalInfo(importer));
		}
		static Destination OnTemplateType(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new TemplateTypeDestination((SnapImporter)importer, GetTemplateIntervalInfo(importer));
		}
		protected static ImportSnapTemplateInfo GetTemplateIntervalInfo(WordProcessingMLBaseImporter importer) {
			SnapBookmarkStartElementDestination thisObject = (SnapBookmarkStartElementDestination)importer.PeekDestination();
			return thisObject.BookmarkInfo.TemplateInfo;
		}
	}
	#endregion
	#region SnapBookmarkEndElementDestination
	public class SnapBookmarkEndElementDestination : SnapBookmarkElementDestination {
		public SnapBookmarkEndElementDestination(SnapImporter importer)
			: base(importer) {
		}
		protected internal override void AssignSnapBookmarkPosition(ImportSnapBookmarkInfo bookmark) {
			bookmark.End = Importer.Position.LogPosition;
		}
	}
	#endregion
	#region FirstGroupBookmarkDestination
	public class FirstGroupBookmarkDestination : TemplateIntervalInfoLeafElementDestination {
		public FirstGroupBookmarkDestination(SnapImporter importer, ImportSnapTemplateInfo templateIntervalInfo)
			: base(importer, templateIntervalInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string text = Importer.ReadDxStringAttr("val", reader);
			if (!String.IsNullOrEmpty(text))
				TemplateIntervalInfo.FirstGroupBookmarkId = text;
		}
	}
	#endregion
	#region LastGroupBookmarkDestination
	public class LastGroupBookmarkDestination : TemplateIntervalInfoLeafElementDestination {
		public LastGroupBookmarkDestination(SnapImporter importer, ImportSnapTemplateInfo templateIntervalInfo)
			: base(importer, templateIntervalInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string text = Importer.ReadDxStringAttr("val", reader);
			if (!String.IsNullOrEmpty(text))
				TemplateIntervalInfo.LastGroupBookmarkId = text;
		}
	}
	#endregion
	#region FirstListBookmarkDestination
	public class FirstListBookmarkDestination : TemplateIntervalInfoLeafElementDestination {
		public FirstListBookmarkDestination(SnapImporter importer, ImportSnapTemplateInfo templateIntervalInfo)
			: base(importer, templateIntervalInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string text = Importer.ReadDxStringAttr("val", reader);
			if (!String.IsNullOrEmpty(text))
				TemplateIntervalInfo.FirstListBookmarkId = text;
		}
	}
	#endregion
	#region LastListBookmarkDestination
	public class LastListBookmarkDestination : TemplateIntervalInfoLeafElementDestination {
		public LastListBookmarkDestination(SnapImporter importer, ImportSnapTemplateInfo templateIntervalInfo)
			: base(importer, templateIntervalInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string text = Importer.ReadDxStringAttr("val", reader);
			if (!String.IsNullOrEmpty(text))
				TemplateIntervalInfo.LastListBookmarkId = text;
		}
	}
	#endregion
	#region FieldInGroupCountDestination
	public class FieldInGroupCountDestination : TemplateIntervalInfoLeafElementDestination {
		public FieldInGroupCountDestination(SnapImporter importer, ImportSnapTemplateInfo templateIntervalInfo)
			: base(importer, templateIntervalInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			TemplateIntervalInfo.FieldInGroupCount = Importer.ReadDxIntAttr("val", reader);
		}
	}
	#endregion
	#region FirstGroupIndexDestination
	public class FirstGroupIndexDestination : TemplateIntervalInfoLeafElementDestination {
		public FirstGroupIndexDestination(SnapImporter importer, ImportSnapTemplateInfo templateIntervalInfo)
			: base(importer, templateIntervalInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			TemplateIntervalInfo.FirstGroupIndex = Importer.ReadDxIntAttr("val", reader);
		}
	}
	#endregion
	#region LastGroupIndexDestination
	public class LastGroupIndexDestination : TemplateIntervalInfoLeafElementDestination {
		public LastGroupIndexDestination(SnapImporter importer, ImportSnapTemplateInfo templateIntervalInfo)
			: base(importer, templateIntervalInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			TemplateIntervalInfo.LastGroupIndex = Importer.ReadDxIntAttr("val", reader);
		}
	}
	#endregion
	#region TemplateTypeDestination
	public class TemplateTypeDestination : TemplateIntervalInfoLeafElementDestination {
		public TemplateTypeDestination(SnapImporter importer, ImportSnapTemplateInfo templateIntervalInfo)
			: base(importer, templateIntervalInfo) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string templateType = Importer.ReadDxStringAttr("val", reader);
			if (!String.IsNullOrEmpty(templateType))
				TemplateIntervalInfo.TemplateType = (SnapTemplateIntervalType)Enum.Parse(typeof(SnapTemplateIntervalType), templateType);
		}
	}
	#endregion
}
