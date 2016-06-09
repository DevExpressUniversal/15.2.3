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
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region BodyDestinationBase (abstract class)
	public abstract class BodyDestinationBase : ElementDestination {
		protected BodyDestinationBase(WordProcessingMLBaseImporter importer)
			: base(importer) {
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
		protected static Destination OnCommentStart(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new CommentStartElementDestination(importer);
		}
		protected static Destination OnCommentEnd(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new CommentEndElementDestination(importer);
		}
		protected static Destination OnStructuredDocument(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new StructuredDocumentDestination(importer);
		}
		protected static Destination OnCustomXml(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new CustomXmlDestination(importer);
		}
	}
	#endregion
	#region BodyDestination
	public class BodyDestination : BodyDestinationBase {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("p", OnParagraph);
			result.Add("tbl", OnTable);
			result.Add("sectPr", OnSection);
			result.Add("bookmarkStart", OnBookmarkStart);
			result.Add("bookmarkEnd", OnBookmarkEnd);
			result.Add("permStart", OnRangePermissionStart);
			result.Add("permEnd", OnRangePermissionEnd);
			result.Add("commentRangeStart", OnCommentStart);
			result.Add("commentRangeEnd", OnCommentEnd);
			result.Add("sdt", OnStructuredDocument);
			result.Add("altChunk", OnAltChunk);
			result.Add("customXml", OnCustomXml);
			return result;
		}
		public BodyDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnTable(WordProcessingMLBaseImporter importer, XmlReader reader) {
			if (importer.DocumentModel.DocumentCapabilities.TablesAllowed)
				return new TableDestination(importer);
			else
				return new TableDisabledDestination(importer);
		}
		static Destination OnParagraph(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateParagraphDestination();
		}
		static Destination OnAltChunk(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new AltChunkDestination(importer);
		}
		static Destination OnSection(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new LastSectionDestination(importer);
		}
	}
	#endregion
	#region LastSectionDestination
	public class LastSectionDestination : SectionDestination {
		public LastSectionDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
	}
	#endregion
}
