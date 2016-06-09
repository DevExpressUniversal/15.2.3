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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Xml;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.XtraRichEdit.Export.WordML;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Import.OpenXml;
using DevExpress.XtraRichEdit.Export.OpenXml;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office;
using DevExpress.Office.Utils;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.WordML {
	#region WordMLImporter
	public class WordMLImporter : WordProcessingMLBaseImporter, IWordMLImporter {
		readonly Dictionary<string, OfficeNativeImage> picturesTable = new Dictionary<string, OfficeNativeImage>();
		public WordMLImporter(DocumentModel documentModel, WordMLDocumentImporterOptions options)
			: base(documentModel, options) {
		}
		public override string WordProcessingNamespaceConst { get { return WordMLExporter.WordProcessingNamespaceConst; } }
		public override string OfficeNamespace { get { return WordMLExporter.OfficeNamespaceConst; } }
		public override string W14NamespaceConst { get { return WordProcessingMLBaseExporter.W14NamespaceConst; } }
		public override string W15NamespaceConst { get { return WordProcessingMLBaseExporter.W15NamespaceConst; } }
		public override string DrawingMLNamespaceConst { get { return WordProcessingMLBaseExporter.DrawingMLNamespaceConst; } }
		public override string RelationsNamespace { get { return String.Empty; } }
		public override string DocumentRootFolder { get; set; }
		public override DevExpress.Office.OpenXmlRelationCollection DocumentRelations { get { return null; } }
		public Dictionary<string, OfficeNativeImage> PicturesTable { get { return picturesTable; } }
		protected override void PrepareOfficeTheme() {
		}
		internal static void ThrowInvalidWordMLFile() {
			throw new ArgumentException("Invalid WordML file");
		}
		public void Import(Stream stream) {
			XmlReader reader = CreateXmlReader(stream);
			if (!ReadToRootElement(reader, "wordDocument", WordMLExporter.WordProcessingNamespaceConst)) {
				ThrowInvalidWordMLFile();
				return;
			}
			ImportMainDocument(reader, stream);
		}
		protected override Destination CreateMainDocumentDestination() {
			return new DocumentDestination(this);
		}
		protected override void AfterImportMainDocument() {
			LinkParagraphStylesWithNumberingLists();
			base.AfterImportMainDocument();
		}
		#region Conversion and Parsing utilities
		public override bool ConvertToBool(string value) {
			if (value == "on")
				return true;
			if (value == "off")
				return false;
			ThrowInvalidFile();
			return false;
		}
		#endregion
		public override void ThrowInvalidFile() {
			throw new Exception("Invalid WordXml file");
		}
		protected internal override string GetWordProcessingMLValue(WordProcessingMLValue value) {
			return value.WordMLValue;
		}
		protected internal override SectionTextDirectionDestination CreateOpenXmlSectionTextDirectionDestination() {
			return null;
		}
		protected internal override SectionTextDirectionDestination CreateWordMLSectionTextDirectionDestination() {
			return new SectionTextDirectionDestination(this);
		}
		protected internal override ParagraphPropertiesBaseDestination CreateStyleParagraphPropertiesDestination(StyleDestinationBase styleDestination, ParagraphFormattingBase paragraphFormatting, TabFormattingInfo tabs) {
			return new StyleParagraphPropertiesDestination(this, styleDestination, paragraphFormatting, tabs);
		}
		protected internal override OpenXml.RunDestination CreateRunDestination() {
			return new RunDestination(this);
		}
		protected internal override OpenXml.ParagraphDestination CreateParagraphDestination() {
			return new ParagraphDestination(this);
		}
		protected internal override int RegisterFootNote(FootNote note, string id) {
			int index = DocumentModel.FootNotes.Count;
			DocumentModel.FootNotes.Add(note);
			PieceTable.InsertFootNoteRun(Position, index);
			return index;
		}
		protected internal override int RegisterEndNote(EndNote note, string id) {
			int index = DocumentModel.EndNotes.Count;
			DocumentModel.EndNotes.Add(note);
			PieceTable.InsertEndNoteRun(Position, index);
			return index;
		}
	}
	#endregion
}
