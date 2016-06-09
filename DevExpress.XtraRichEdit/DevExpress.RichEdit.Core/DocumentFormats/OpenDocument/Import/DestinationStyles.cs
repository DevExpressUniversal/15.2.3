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
using DevExpress.XtraRichEdit.Import.OpenDocument;
using System.Xml;
using DevExpress.XtraRichEdit.Export.OpenDocument;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.OpenDocument {
	#region OfficeDocumentStylesDestination
	public partial class DocumentStylesDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("styles", OnStyle);
			result.Add("font-face-decls", OnFontFaceDeclaration);
			result.Add("master-styles", OnMasterStyles);
			result.Add("automatic-styles", OnAutomaticStyles);
			return result;
		}
		public DocumentStylesDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnStyle(OpenDocumentTextImporter importer, XmlReader reader) {
			return new StylesDestination(importer);
		}
		static Destination OnFontFaceDeclaration(OpenDocumentTextImporter importer, XmlReader reader) {
			return new FontFaceDeclarationDestination(importer);
		}
		static Destination OnMasterStyles(OpenDocumentTextImporter importer, XmlReader reader) {
			return new MasterStylesDestination(importer);
		}
		static Destination OnAutomaticStyles(OpenDocumentTextImporter importer, XmlReader reader) {
			return new AutomaticStylesDestination(importer);
		}
	}
	#endregion
	#region MasterStylesDestination
	public class MasterStylesDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("master-page", OnMasterPage);
			return result;
		}
		public MasterStylesDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnMasterPage(OpenDocumentTextImporter importer, XmlReader reader) {
			return new MasterPageStyleDestination(importer);
		}
	}
	#endregion
	#region StylesDestination
	public partial class StylesDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("style", OnStyle);
			result.Add("default-style", OnDefaultStyle);
			result.Add("list-style", OnListStyle);
			result.Add("linenumbering-configuration", OnLineNumberingConfigDestination);
			return result;
		}
		public StylesDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnStyle(OpenDocumentTextImporter importer, XmlReader reader) {
			return new StyleDestination(importer);
		}
		static Destination OnDefaultStyle(OpenDocumentTextImporter importer, XmlReader reader) {
			return new DefaultStyleDestination(importer);
		}
		static Destination OnListStyle(OpenDocumentTextImporter importer, XmlReader reader) {
			return new ListStyleDestination(importer);
		}
		static Destination OnLineNumberingConfigDestination(OpenDocumentTextImporter importer, XmlReader reader) {
			return new LineNumberingConfigDestination(importer);
		}
	}
	#endregion
	#region FontFaceDeclarationDestination
	public partial class FontFaceDeclarationDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("font-face", OnFontFace);
			return result;
		}
		public FontFaceDeclarationDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		static Destination OnFontFace(OpenDocumentTextImporter importer, XmlReader reader) {
			return new FontFaceDestination(importer);
		}
	}
	#endregion
	#region FontTranslationTable
	public class FontTable : Dictionary<string, string> {
	}
	#endregion
	#region FontFaceDeclaration
	public partial class FontFaceDestination : LeafElementDestination {
		public FontFaceDestination(OpenDocumentTextImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			string id = ImportHelper.GetStyleStringAttribute(reader, "name");
			if (String.IsNullOrEmpty(id))
				return;
			if (Importer.FontTable.ContainsKey(id))
				return;
			string fontName = ImportHelper.GetSvgStringAttribute(reader, "font-family");
			fontName = fontName.Replace("\'", String.Empty);
			fontName = DevExpress.XtraRichEdit.Import.Rtf.StyleSheetDestination.GetPrimaryStyleName(fontName);
			if (String.IsNullOrEmpty(fontName))
				return;
			Importer.FontTable.Add(id, fontName);
		}
	}
	#endregion
}
