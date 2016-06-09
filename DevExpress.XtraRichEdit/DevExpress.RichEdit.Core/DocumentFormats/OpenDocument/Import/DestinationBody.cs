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
using DevExpress.XtraRichEdit.Import.OpenDocument;
using System.Xml;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.OpenDocument {
	#region BodyDestination
	public class BodyDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("text", OnText);
			return result;
		}
		public BodyDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnText(OpenDocumentTextImporter importer, XmlReader reader) {
			return new TextDestination(importer);
		}
	}
	#endregion
	#region TextDestination
	public class TextDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("h", OnHeading);
			result.Add("p", OnParagraph);
			result.Add("list", OnList);
			result.Add("section", OnSection);
			result.Add("table-of-content", OnTableOfContent);
			result.Add("table", OnTableDestination);
			result.Add("user-field-decls", OnUserFieldDeclarations);
			FieldHandlers.AddFieldHandlers(result);
			return result;
		}
		public TextDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnParagraph(OpenDocumentTextImporter importer, XmlReader reader) {
			return new ParagraphDestination(importer);
		}
		static Destination OnHeading(OpenDocumentTextImporter importer, XmlReader reader) {
			return new HeadingDestination(importer);
		}
		public static Destination OnList(OpenDocumentTextImporter importer, XmlReader reader) {
			return new ListDestination(importer);
		}
		static Destination OnSection(OpenDocumentTextImporter importer, XmlReader reader) {
			return new SectionDestination(importer);
		}
		public static Destination OnTableOfContent(OpenDocumentTextImporter importer, XmlReader reader) {
			return new TableOfContentDestination(importer);
		}
		static Destination OnTableDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			DocumentCapabilitiesOptions options = importer.DocumentModel.DocumentCapabilities;
			if (options.TablesAllowed && options.ParagraphsAllowed)
				return new TableDestination(importer);
			else
				return new TableDisabledDestination(importer);
		}
		static Destination OnUserFieldDeclarations(OpenDocumentTextImporter importer, XmlReader reader) {
			return new UserFieldDeclarationsDestination(importer);
		}
	}
	#endregion
}
