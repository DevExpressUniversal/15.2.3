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
using DevExpress.XtraRichEdit.Export.OpenDocument;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Import.OpenDocument {
	#region BookmarkElementDestination (abstract class)
	public abstract class BookmarkElementDestinationBase : LeafElementDestination {
		protected BookmarkElementDestinationBase(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string name = ImportHelper.GetTextStringAttribute(reader, "name");
			if (!String.IsNullOrEmpty(name)) {
				name = name.Trim();
			}
			ImportBookmarkInfo bookmark;
			if (!Importer.Bookmarks.TryGetValue(name, out bookmark)) {
				bookmark = new ImportBookmarkInfo();
				bookmark.Name = name;
				Importer.Bookmarks.Add(name, bookmark);
			}
			AssignBookmarkPosition(bookmark);
		}
		protected internal abstract void AssignBookmarkPosition(ImportBookmarkInfo bookmark);
	}
	#endregion
	public class BookmarkElementDestination : BookmarkElementDestinationBase {
		public BookmarkElementDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override void AssignBookmarkPosition(ImportBookmarkInfo bookmark) {
			bookmark.Start = Importer.InputPosition.LogPosition;
			bookmark.End = Importer.InputPosition.LogPosition;
		}
	}
	public class BookmarkStartElementDestination : BookmarkElementDestinationBase {
		public BookmarkStartElementDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override void AssignBookmarkPosition(ImportBookmarkInfo bookmark) {
			bookmark.Start = Importer.InputPosition.LogPosition;
		}
	}
	public class BookmarkEndElementDestination : BookmarkElementDestinationBase {
		public BookmarkEndElementDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override void AssignBookmarkPosition(ImportBookmarkInfo bookmark) {
			bookmark.End = Importer.InputPosition.LogPosition;
		}
	}
}
