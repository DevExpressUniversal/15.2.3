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
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.WordML {
	#region BodyDestination
	public class BodyDestination : DevExpress.XtraRichEdit.Import.OpenXml.BodyDestinationBase {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("p", OnParagraph);
			result.Add("tbl", OnTable);
			result.Add("sectPr", OnSection);
			result.Add("sect", OnOptionalSection);
			result.Add("sub-section", OnOptionalSubSection);
			result.Add("annotation", OnAnnotation);
			result.Add("permStart", OnRangePermissionStart);
			result.Add("permEnd", OnRangePermissionEnd);
			result.Add("hlink", OnHlink);
			return result;
		}
		public BodyDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnTable(WordProcessingMLBaseImporter importer, XmlReader reader) {
			if (importer.DocumentModel.DocumentCapabilities.TablesAllowed)
				return new DevExpress.XtraRichEdit.Import.WordML.TableDestination(importer);
			else
				return new DevExpress.XtraRichEdit.Import.OpenXml.TableDisabledDestination(importer);
		}
		static Destination OnSection(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DevExpress.XtraRichEdit.Import.WordML.LastSectionDestination(importer);
		}
		static Destination OnOptionalSection(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DevExpress.XtraRichEdit.Import.WordML.BodyDestination(importer);
		}
		static Destination OnOptionalSubSection(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new DevExpress.XtraRichEdit.Import.WordML.BodyDestination(importer);
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
	#region LastSectionDestination
	public class LastSectionDestination : SectionDestination {
		public LastSectionDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
	}
	#endregion
}
