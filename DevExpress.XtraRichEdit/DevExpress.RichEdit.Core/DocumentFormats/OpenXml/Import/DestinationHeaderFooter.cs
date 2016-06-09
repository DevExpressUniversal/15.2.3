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
using System.IO;
using System.Xml;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Export.OpenXml;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region HeaderFooterDestinationBase<T> (abstract class)
	public abstract class HeaderFooterDestinationBase<T> : BodyDestinationBase where T : SectionHeaderFooterBase {
		T newHeaderFooter;
		protected HeaderFooterDestinationBase(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			this.newHeaderFooter = CalculateTargetHeaderFooter(reader);
			Importer.PushCurrentPieceTable(newHeaderFooter.PieceTable);
		}
		public override void ProcessElementClose(XmlReader reader) {
			PieceTable.FixLastParagraph();
			Importer.InsertBookmarks();
			Importer.InsertRangePermissions();
			PieceTable.FixTables();
			if (newHeaderFooter.PieceTable.IsEmpty)
				RemoveHeaderFooter(newHeaderFooter);
			Importer.PopCurrentPieceTable();
		}
		protected internal virtual void RemoveHeaderFooter(T headerFooter) {
			headerFooter.GetContainer(Importer.CurrentSection).Remove(headerFooter.Type);
		}
		protected internal abstract T CalculateTargetHeaderFooter(XmlReader reader);
	}
	#endregion
	#region HeaderFooterDestination<T> (abstract class)
	public abstract class HeaderFooterDestination<T> : HeaderFooterDestinationBase<T> where T : SectionHeaderFooterBase {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("p", OnParagraph);
			result.Add("tbl", OnTable);
			result.Add("bookmarkStart", OnBookmarkStart);
			result.Add("bookmarkEnd", OnBookmarkEnd);
			result.Add("permStart", OnRangePermissionStart);
			result.Add("permEnd", OnRangePermissionEnd);
			result.Add("sdt", OnStructuredDocument);
			result.Add("customXml", OnCustomXml);
			return result;
		}
		protected HeaderFooterDestination(WordProcessingMLBaseImporter importer)
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
	}
	#endregion
	#region HeaderFooterReferenceDestinationBase (abstract class)
	public abstract class HeaderFooterReferenceDestinationBase<T> : LeafElementDestination where T : SectionHeaderFooterBase {
		protected HeaderFooterReferenceDestinationBase(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		internal virtual new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		protected abstract string RootPieceTableTag { get; }
		public override void ProcessElementOpen(XmlReader reader) {
			OpenXmlImporter importer = (OpenXmlImporter)Importer;
			string id = reader.GetAttribute("id", OpenXmlExporter.RelsNamespace);
			if (String.IsNullOrEmpty(id))
				Importer.ThrowInvalidFile();
			Stream pieceTableStream = importer.LookupPackageFileStreamByRelationId(id, importer.DocumentRootFolder, true);
			if (pieceTableStream == null)
				Importer.ThrowInvalidFile();
			string headerFooterFileName = Importer.LookupRelationTargetById(Importer.DocumentRelations, id, importer.DocumentRootFolder, String.Empty);
			Importer.DocumentRelationsStack.Push(Importer.ImportRelations(Importer.DocumentRootFolder + "/_rels/" + Path.GetFileName(headerFooterFileName) + ".rels"));
			Importer.PackageFileStreamsStack.Push(new PackageFileStreams());
			Importer.PackageImagesStack.Push(new Dictionary<string, OfficeNativeImage>());
			XmlReader headerReader = importer.CreateXmlReader(pieceTableStream);
			if (!importer.ReadToRootElement(headerReader, RootPieceTableTag))
				Importer.ThrowInvalidFile();
			T newPieceTable = CreatePieceTable(reader.GetAttribute("type", Importer.WordProcessingNamespaceConst));
			HeaderFooterDestination<T> destination = CreatePieceTableDestination(newPieceTable);
			destination.ProcessElementOpen(reader);
			importer.ImportContent(headerReader, destination);
			Importer.PackageFileStreamsStack.Pop();
			Importer.DocumentRelationsStack.Pop();
			Importer.PackageImagesStack.Pop();
		}
		protected internal abstract T CreatePieceTable(string type);
		protected internal abstract HeaderFooterDestination<T> CreatePieceTableDestination(T pieceTable);
	}
	#endregion
	#region HeaderReferenceDestination
	public class HeaderReferenceDestination : HeaderFooterReferenceDestinationBase<SectionHeader> {
		public HeaderReferenceDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected override string RootPieceTableTag { get { return "hdr"; } }
		protected internal override HeaderFooterDestination<SectionHeader> CreatePieceTableDestination(SectionHeader pieceTable) {
			return new HeaderDestination(Importer, pieceTable);
		}
		protected internal override SectionHeader CreatePieceTable(string type) {
			if (String.IsNullOrEmpty(type))
				type = "default";
			Section section = Importer.CurrentSection;
			switch (type) {
				case "first":
					section.Headers.Create(HeaderFooterType.First);
					return section.InnerFirstPageHeader;
				case "even":
					section.Headers.Create(HeaderFooterType.Even);
					return section.InnerEvenPageHeader;
				case "default":
					section.Headers.Create(HeaderFooterType.Odd);
					return section.InnerOddPageHeader;
				default:
					Importer.ThrowInvalidFile();
					return null;
			}
		}
	}
	#endregion
	#region FooterReferenceDestination
	public class FooterReferenceDestination : HeaderFooterReferenceDestinationBase<SectionFooter> {
		public FooterReferenceDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected override string RootPieceTableTag { get { return "ftr"; } }
		protected internal override HeaderFooterDestination<SectionFooter> CreatePieceTableDestination(SectionFooter pieceTable) {
			return new FooterDestination(Importer, pieceTable);
		}
		protected internal override SectionFooter CreatePieceTable(string type) {
			if (String.IsNullOrEmpty(type))
				type = "default";
			Section section = Importer.CurrentSection;
			switch (type) {
				case "first":
					section.Footers.Create(HeaderFooterType.First);
					return section.InnerFirstPageFooter;
				case "even":
					section.Footers.Create(HeaderFooterType.Even);
					return section.InnerEvenPageFooter;
				case "default":
					section.Footers.Create(HeaderFooterType.Odd);
					return section.InnerOddPageFooter;
				default:
					Importer.ThrowInvalidFile();
					return null;
			}
		}
	}
	#endregion
	#region HeaderDestination
	public class HeaderDestination : HeaderFooterDestination<SectionHeader> {
		readonly SectionHeader header;
		public HeaderDestination(WordProcessingMLBaseImporter importer, SectionHeader header)
			: base(importer) {
			Guard.ArgumentNotNull(header, "header");
			this.header = header;
		}
		protected internal override SectionHeader CalculateTargetHeaderFooter(XmlReader reader) {
			return header;
		}
	}
	#endregion
	#region FooterDestination
	public class FooterDestination : HeaderFooterDestination<SectionFooter> {
		readonly SectionFooter footer;
		public FooterDestination(WordProcessingMLBaseImporter importer, SectionFooter footer)
			: base(importer) {
			Guard.ArgumentNotNull(footer, "footer");
			this.footer = footer;
		}
		protected internal override SectionFooter CalculateTargetHeaderFooter(XmlReader reader) {
			return footer;
		}
	}
	#endregion
}
