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
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DrawingTextParagraphDestination
	public class DrawingTextParagraphDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pPr", OnTextParagraphProperties);
			result.Add("endParaRPr", OnEndParagraphRunProperties);
			result.Add("br", OnTextLineBreak);
			result.Add("fld", OnTextField);
			result.Add("r", OnTextRun);
			return result;
		}
		static DrawingTextParagraphDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DrawingTextParagraphDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly DrawingTextParagraph paragraph;
		#endregion
		public DrawingTextParagraphDestination(SpreadsheetMLBaseImporter importer, DrawingTextParagraph paragraph)
			: base(importer) {
			this.paragraph = paragraph;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnTextParagraphProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DrawingTextParagraph paragraph = GetThis(importer).paragraph;
			paragraph.ApplyParagraphProperties = true;
			return new DrawingTextParagraphPropertiesDestination(importer, paragraph.ParagraphProperties);
		}
		static Destination OnEndParagraphRunProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DrawingTextParagraph paragraph = GetThis(importer).paragraph;
			paragraph.ApplyEndRunProperties = true;
			return new DrawingTextCharacterPropertiesDestination(importer, paragraph.EndRunProperties);
		}
		static Destination OnTextLineBreak(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DrawingTextLineBreak lineBreak = new DrawingTextLineBreak(importer.ActualDocumentModel);
			GetThis(importer).paragraph.Runs.AddCore(lineBreak);
			return new DrawingTextLineBreakDestination(importer, lineBreak);
		}
		static Destination OnTextField(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DrawingTextField textField = new DrawingTextField(importer.ActualDocumentModel);
			GetThis(importer).paragraph.Runs.AddCore(textField);
			return new DrawingTextFieldDestination(importer, textField);
		}
		static Destination OnTextRun(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DrawingTextRun run = new DrawingTextRun(importer.ActualDocumentModel);
			GetThis(importer).paragraph.Runs.AddCore(run);
			return new DrawingTextRunDestination(importer, run);
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			paragraph.DocumentModel.BeginUpdate();
		}
		public override void ProcessElementClose(XmlReader reader) {
			paragraph.DocumentModel.EndUpdate();
		}
	}
	#endregion
	#region DrawingTextLineBreakDestination
	public class DrawingTextLineBreakDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("rPr", OnTextRunProperties);
			return result;
		}
		static DrawingTextLineBreakDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DrawingTextLineBreakDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly DrawingTextLineBreak lineBreak;
		#endregion
		public DrawingTextLineBreakDestination(SpreadsheetMLBaseImporter importer, DrawingTextLineBreak lineBreak)
			: base(importer) {
			this.lineBreak = lineBreak;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnTextRunProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingTextCharacterPropertiesDestination(importer, GetThis(importer).lineBreak.RunProperties);
		}
		#endregion
	}
	#endregion
	#region DrawingTextFieldDestination
	public class DrawingTextFieldDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("rPr", OnTextRunProperties);
			result.Add("pPr", OnTextParagraphProperties);
			result.Add("t", OnTextRunString);
			return result;
		}
		static DrawingTextFieldDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DrawingTextFieldDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly DrawingTextField textField;
		#endregion
		public DrawingTextFieldDestination(SpreadsheetMLBaseImporter importer, DrawingTextField textField)
			: base(importer) {
			this.textField = textField;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnTextRunProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingTextCharacterPropertiesDestination(importer, GetThis(importer).textField.RunProperties);
		}
		static Destination OnTextParagraphProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingTextParagraphPropertiesDestination(importer, GetThis(importer).textField.ParagraphProperties);
		}
		static Destination OnTextRunString(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DrawingTextField textField = GetThis(importer).textField;
			return new StringValueTagDestination(importer, delegate(string value) { textField.SetTextCore(value); return true; });
		}
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			Importer.DocumentModel.BeginUpdate();
			Guid fieldId;
			if (Guid.TryParse(Importer.ReadAttribute(reader, "id"), out fieldId))
				textField.FieldId = fieldId;
			else
				textField.FieldId = Guid.Empty;
			textField.FieldType = Importer.ReadAttribute(reader, "type", string.Empty);
		}
		public override void ProcessElementClose(XmlReader reader) {
			Importer.DocumentModel.EndUpdate();
		}
	}
	#endregion
	#region DrawingTextRunDestination
	public class DrawingTextRunDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("rPr", OnTextRunProperties);
			result.Add("t", OnTextRunString);
			return result;
		}
		static DrawingTextRunDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DrawingTextRunDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly DrawingTextRun run;
		#endregion
		public DrawingTextRunDestination(SpreadsheetMLBaseImporter importer, DrawingTextRun run)
			: base(importer) {
			this.run = run;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		#region Handlers
		static Destination OnTextRunProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new DrawingTextCharacterPropertiesDestination(importer, GetThis(importer).run.RunProperties);
		}
		static Destination OnTextRunString(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DrawingTextRun run = GetThis(importer).run;
			return new StringValueTagDestination(importer, delegate(string value) { run.SetTextCore(value); return true; });
		}
		#endregion
	}
	#endregion
}
