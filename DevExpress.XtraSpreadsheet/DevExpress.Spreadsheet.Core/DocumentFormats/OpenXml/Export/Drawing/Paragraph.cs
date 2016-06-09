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
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Utils;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter : IDrawingTextRunVisitor {
		protected internal void GenerateDrawingTextParagraphContent(DrawingTextParagraph paragraph) {
			WriteStartElement("p", DrawingMLNamespace);
			try {
				if (paragraph.ApplyParagraphProperties)
					GenerateDrawingTextParagraphPropertiesContent(paragraph.ParagraphProperties);
				paragraph.Runs.ForEach(ExportTextRun);
				if (paragraph.ApplyEndRunProperties)
					GenerateCharacterPropertiesContent("endParaRPr", paragraph.EndRunProperties);
			}
			finally {
				WriteEndElement();
			}
		}
		protected internal void ExportTextRun(IDrawingTextRun run) {
			run.Visit(this);
		}
		#region IDrawingTextRunVisitor Members
		void IDrawingTextRunVisitor.Visit(DrawingTextLineBreak lineBreak) {
			WriteStartElement("br", DrawingMLNamespace);
			try {
				if (!lineBreak.RunProperties.IsDefault)
					GenerateCharacterPropertiesContent("rPr", lineBreak.RunProperties);
			}
			finally {
				WriteEndElement();
			}
		}
		void IDrawingTextRunVisitor.Visit(DrawingTextField textField) {
			WriteStartElement("fld", DrawingMLNamespace);
			try {
				string fieldId = "{" + textField.FieldId.ToString() + "}";
				WriteStringValue("id", fieldId.ToUpper());
				WriteStringValue("type", textField.FieldType, !String.IsNullOrEmpty(textField.FieldType));
				if (!textField.RunProperties.IsDefault)
					GenerateCharacterPropertiesContent("rPr", textField.RunProperties);
				DrawingTextParagraphProperties paragraphProperties = textField.ParagraphProperties;
				if (!paragraphProperties.IsDefault)
					GenerateDrawingTextParagraphPropertiesContent(paragraphProperties);
				GenerateDrawingTextStringContent(textField.Text);
			}
			finally {
				WriteEndElement();
			}
		}
		void IDrawingTextRunVisitor.Visit(DrawingTextRun run) {
			if (string.IsNullOrEmpty(run.Text))
				return;
			WriteStartElement("r", DrawingMLNamespace);
			try {
				if (!run.RunProperties.IsDefault)
					GenerateCharacterPropertiesContent("rPr", run.RunProperties);
				GenerateDrawingTextStringContent(run.Text);
			}
			finally {
				WriteEndElement();
			}
		}
		void GenerateDrawingTextStringContent(string text) {
			if (String.IsNullOrEmpty(text))
				return;
			WriteStartElement("t", DrawingMLNamespace);
			try {
				WriteShString(text);
			}
			finally {
				WriteEndElement();
			}
		}
		#endregion
	}
}
