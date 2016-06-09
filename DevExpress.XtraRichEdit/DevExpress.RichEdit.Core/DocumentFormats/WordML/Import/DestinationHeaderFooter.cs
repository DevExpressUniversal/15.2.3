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
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.WordML {
	#region HeaderFooterDestination<T> (abstract class)
	public abstract class HeaderFooterDestination<T> : DevExpress.XtraRichEdit.Import.OpenXml.HeaderFooterDestinationBase<T> where T : SectionHeaderFooterBase {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("p", OnParagraph);
			result.Add("tbl", OnTable);
			result.Add("annotation", OnAnnotation);
			result.Add("permStart", OnRangePermissionStart);
			result.Add("permEnd", OnRangePermissionEnd);
			result.Add("hlink", OnHlink);
			return result;
		}
		protected HeaderFooterDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected static Destination OnTable(WordProcessingMLBaseImporter importer, XmlReader reader) {
			if (importer.DocumentModel.DocumentCapabilities.TablesAllowed)
				return new DevExpress.XtraRichEdit.Import.WordML.TableDestination(importer);
			else
				return new DevExpress.XtraRichEdit.Import.OpenXml.TableDisabledDestination(importer);
		}
		static Destination OnParagraph(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DevExpress.XtraRichEdit.Import.WordML.ParagraphDestination(importer);
		}
		static Destination OnAnnotation(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DevExpress.XtraRichEdit.Import.WordML.AnnotationElementDestination(importer);
		}
		static Destination OnHlink(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DevExpress.XtraRichEdit.Import.WordML.HyperlinkDestination(importer);
		}
	}
	#endregion
	#region HeaderDestination
	public class HeaderDestination : HeaderFooterDestination<SectionHeader> {
		public HeaderDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override SectionHeader CalculateTargetHeaderFooter(XmlReader reader) {
			string headerType = reader.GetAttribute("type", Importer.WordProcessingNamespaceConst);
			Section section = Importer.CurrentSection;
			switch (headerType) {
				case "first":
					section.Headers.Create(HeaderFooterType.First);
					return section.InnerFirstPageHeader;
				case "even":
					section.Headers.Create(HeaderFooterType.Even);
					return section.InnerEvenPageHeader;
				case "odd":
					section.Headers.Create(HeaderFooterType.Odd);
					return section.InnerOddPageHeader;
				default:
					Importer.ThrowInvalidFile();
					return null;
			}
		}
	}
	#endregion
	#region FooterDestination
	public class FooterDestination : HeaderFooterDestination<SectionFooter> {
		public FooterDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override SectionFooter CalculateTargetHeaderFooter(XmlReader reader) {
			string footerType = reader.GetAttribute("type", Importer.WordProcessingNamespaceConst);
			Section section = Importer.CurrentSection;
			switch (footerType) {
				case "first":
					section.Footers.Create(HeaderFooterType.First);
					return section.InnerFirstPageFooter;
				case "even":
					section.Footers.Create(HeaderFooterType.Even);
					return section.InnerEvenPageFooter;
				case "odd":
					section.Footers.Create(HeaderFooterType.Odd);
					return section.InnerOddPageFooter;
				default:
					Importer.ThrowInvalidFile();
					return null;
			}
		}
	}
	#endregion
}
