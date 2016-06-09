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
using System.Text;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Text;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Export.Rtf {
	#region RtfExporter
	public class RtfExporter : IRtfExporter {
		#region Fields
		readonly RtfExportHelper rtfExportHelper;
		readonly RtfContentExporter contentExporter;
		readonly RtfBuilder rtfBuilder;
		readonly StringBuilder fontNameStringBuilder = new StringBuilder();
		#endregion
		public RtfExporter(DocumentModel documentModel, RtfDocumentExporterOptions options) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.rtfExportHelper = new RtfExportHelper();
			this.contentExporter = CreateContentExporter(documentModel, options, RtfExportHelper);
			this.rtfBuilder = ContentExporter.CreateRtfBuilder();
		}
		#region Properties
		protected internal RtfExportHelper RtfExportHelper { get { return rtfExportHelper; } }
		protected internal RtfContentExporter ContentExporter { get { return contentExporter; } }
		protected internal RtfBuilder RtfBuilder { get { return rtfBuilder; } }
		public bool LastParagraphRunNotSelected { get { return ContentExporter.LastParagraphRunNotSelected; } set { ContentExporter.LastParagraphRunNotSelected = value; } }
		public bool KeepFieldCodeViewState { get { return ContentExporter.KeepFieldCodeViewState; } set { ContentExporter.KeepFieldCodeViewState = value; } }
		#endregion
		protected virtual RtfContentExporter CreateContentExporter(DocumentModel documentModel, RtfDocumentExporterOptions options, RtfExportHelper rtfExportHelper) {
			return new RtfContentExporter(documentModel, options, rtfExportHelper);
		}
		public virtual ChunkedStringBuilder ExportSaveMemory() {
			ExportCore();
			return RtfBuilder.RtfContent;
		}
		public virtual string Export() {
			ExportCore();
			return RtfBuilder.RtfContent.ToString();
		}
		protected internal virtual void ExportCore() {
			ContentExporter.Export();
			ChunkedStringBuilder content = ContentExporter.RtfBuilder.RtfContent;
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.RtfSignature);
			RtfBuilder.WriteCommand(RtfExportSR.DefaultFontIndex, RtfExportHelper.DefaultFontIndex);
			ExportFontTable();
			ExportColorTable();
			ExportDefaultProperties();
			ExportStyleTable();
			ExportListTable();
			ExportListOverrideTable();
			ExportParagraphGroupProperties();
			ExportUsersTable();
			ExportDocumentVariables();
			RtfBuilder.WriteTextDirectUnsafe(content);
			RtfBuilder.CloseGroup();
		}
		protected internal virtual void ExportFontTable() {
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.FontTable);
			List<string> fontNames = RtfExportHelper.FontNamesCollection;
			int count = fontNames.Count;
			for (int i = 0; i < count; i++)
				ExportFontTableEntry(fontNames[i], i);
			RtfBuilder.CloseGroup();
		}
		protected internal virtual void ExportFontTableEntry(string fontName, int fontIndex) {
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.FontNumber, fontIndex);
			Encoding encoding = CalculateFontNameEncoding(fontName, fontIndex);
			if (encoding != null) {
				int fontCharset = DXEncoding.CharsetFromCodePage(DXEncoding.GetEncodingCodePage(encoding));
				if(fontCharset == 0)
					if (!RtfExportHelper.FontCharsetTable.TryGetValue(fontIndex, out fontCharset))
						fontCharset = 0;
				if (fontCharset > 1) 
					RtfBuilder.WriteCommand(RtfExportSR.FontCharset, fontCharset);
			}
			else
				encoding = contentExporter.Options.ActualEncoding;
			fontName = EscapeSpecialCharacters(fontName);
			if (CanBeLoselesslyEncoded(fontName, DXEncoding.ASCII)) 
				RtfBuilder.WriteTextDirect(fontName);
			else
				RtfBuilder.WriteTextDirect(EscapeFontName(fontName, encoding));
			RtfBuilder.WriteTextDirect(";");
			RtfBuilder.CloseGroup();
		}
		string EscapeSpecialCharacters(string fontName) {
			bool containsSpecialSymbol = false;
			int length = fontName.Length;
			for (int i = 0; i < length; i++) {
				char ch = fontName[i];
				if (ch != ';' && ch != '\\' && ch != '{' && ch != '}') {
					if (containsSpecialSymbol)
						fontNameStringBuilder.Append(ch);
					continue;
				}
				if (!containsSpecialSymbol) {
					fontNameStringBuilder.Clear();
					if (i > 0)
						fontNameStringBuilder.Append(fontName.Substring(0, i));
					containsSpecialSymbol = true;
				}
				fontNameStringBuilder.Append(@"\'");
				fontNameStringBuilder.Append(RtfBuilder.byteToHexString[(int)ch % 256]);
			}
			if (containsSpecialSymbol)
				return fontNameStringBuilder.ToString();
			else
				return fontName;
		}
		protected internal virtual Encoding CalculateFontNameEncoding(string fontName, int fontIndex) {
			if (CanBeLoselesslyEncoded(fontName, DXEncoding.ASCII))
				return DXEncoding.ASCII;
			int fontCharset;
			if (!RtfExportHelper.FontCharsetTable.TryGetValue(fontIndex, out fontCharset))
				fontCharset = -1;
			if (fontCharset >= 0) {
				int codePage = DXEncoding.CodePageFromCharset(fontCharset);
				try {
					Encoding encoding = DXEncoding.GetEncodingFromCodePage(codePage);
					if (CanBeLoselesslyEncoded(fontName, encoding))
						return encoding;
				}
				catch {
				}
			}
			if (CanBeLoselesslyEncoded(fontName, contentExporter.Options.ActualEncoding))
				return contentExporter.Options.ActualEncoding;
			EncodingInfo[] encodings = DXEncoding.GetEncodings();
			int count = encodings.Length;
			for (int i = 0; i < count; i++) {
				try {
					Encoding enc = encodings[i].GetEncoding();
					if (enc.CodePage != 65000 && 
						enc.CodePage != 65001 && 
						enc.CodePage != 1200 && 
						enc.CodePage != 1201) { 
						if (CanBeLoselesslyEncoded(fontName, enc)) {
							return enc;
						}
					}
				}
				catch {
				}
			}
			return null;
		}
		protected internal bool CanBeLoselesslyEncoded(string value, Encoding encoding) {
			byte[] bytes = encoding.GetBytes(value);
			string decodedValue = encoding.GetString(bytes, 0, bytes.Length);
			return String.Compare(value, decodedValue) == 0;
		}
		protected internal virtual string EscapeFontName(string fontName, Encoding encoding) {
			byte[] bytes = encoding.GetBytes(fontName);			
			int count = bytes.Length;
			fontNameStringBuilder.Clear();
			for (int i = 0; i < count; i++) {
				fontNameStringBuilder.Append(@"\'");
				fontNameStringBuilder.Append(RtfBuilder.byteToHexString[(int)bytes[i] % 256]);
			}
			return fontNameStringBuilder.ToString();
		}
		protected internal virtual void ExportDefaultProperties() {
			ExportDefaultCharacterProperties();
			ExportDefaultParagraphProperties();
		}
		protected internal virtual void ExportDefaultCharacterProperties() {
			if (String.IsNullOrEmpty(RtfExportHelper.DefaultCharacterProperties))
				return;
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.DefaultCharacterProperties);
			RtfBuilder.WriteTextDirect(RtfExportHelper.DefaultCharacterProperties);
			RtfBuilder.CloseGroup();
		}
		protected internal virtual void ExportDefaultParagraphProperties() {
			if (String.IsNullOrEmpty(RtfExportHelper.DefaultParagraphProperties))
				return;
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.DefaultParagraphProperties);
			RtfBuilder.WriteTextDirect(RtfExportHelper.DefaultParagraphProperties);
			RtfBuilder.CloseGroup();
		}
		protected internal virtual void ExportColorTable() {
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.ColorTable);
			RtfExportHelper.ColorCollection.ForEach(ExportColorTableEntry);
			RtfBuilder.CloseGroup();
		}
		protected internal virtual void ExportColorTableEntry(Color color) {
			if (color != DXColor.Empty) {
				RtfBuilder.WriteCommand(RtfExportSR.ColorRed, color.R);
				RtfBuilder.WriteCommand(RtfExportSR.ColorGreen, color.G);
				RtfBuilder.WriteCommand(RtfExportSR.ColorBlue, color.B);
			}
			RtfBuilder.WriteTextDirect(";");
		}
		protected internal virtual void ExportListTable() {
			Dictionary<int, string> listCollection = RtfExportHelper.ListCollection;
			if (listCollection.Count <= 0)
				return;
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.NumberingListTable);
			foreach (string value in listCollection.Values)
				RtfBuilder.WriteTextDirect(value);
			RtfBuilder.CloseGroup();
		}
		protected internal virtual void ExportListOverrideTable() {
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.ListOverrideTable);
			List<string> overrides = RtfExportHelper.ListOverrideCollection;
			int count = overrides.Count;
			for (int i = 0; i < count; i++)
				RtfBuilder.WriteTextDirect(overrides[i]);
			RtfBuilder.CloseGroup();
		}
		protected internal virtual void ExportStyleTable() {
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.StyleTable);
			List<string> styles = RtfExportHelper.StylesCollection;
			int count = styles.Count;
			for (int i = 0; i < count; i++)
				RtfBuilder.WriteTextDirect(styles[i]);
			RtfBuilder.CloseGroup();
		}
		protected internal virtual void ExportParagraphGroupProperties() {
			WebSettings webSettings = ContentExporter.DocumentModel.WebSettings;
			if (!webSettings.IsBodyMarginsSet())
				return;
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.ParagraphGroupPropertiesTable);
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.ParagraphGroupProperties);
			RtfBuilder.WriteCommand(RtfExportSR.ParagraphGroupPropertiesId, 0); 
			Office.DocumentModelUnitConverter unitConverter = ContentExporter.UnitConverter;
			RtfBuilder.WriteCommand(RtfExportSR.LeftIndentInTwips, unitConverter.ModelUnitsToTwips(webSettings.LeftMargin));
			RtfBuilder.WriteCommand(RtfExportSR.RightIndentInTwips, unitConverter.ModelUnitsToTwips(webSettings.RightMargin));
			RtfBuilder.WriteCommand(RtfExportSR.SpaceBefore, unitConverter.ModelUnitsToTwips(webSettings.TopMargin));
			RtfBuilder.WriteCommand(RtfExportSR.SpaceAfter, unitConverter.ModelUnitsToTwips(webSettings.BottomMargin));
			RtfBuilder.CloseGroup();
			RtfBuilder.CloseGroup();
		}
		protected internal virtual void ExportUsersTable() {
			List<string> users = RtfExportHelper.UserCollection;
			int count = users.Count;
			if (count <= 0)
				return;
			RtfBuilder.OpenGroup();
			RtfBuilder.WriteCommand(RtfExportSR.UserTable);
			for (int i = 0; i < count; i++) {
				RtfBuilder.OpenGroup();
				RtfBuilder.WriteText(users[i]);
				RtfBuilder.CloseGroup();
			}
			RtfBuilder.CloseGroup();
		}
		protected internal virtual void ExportDocumentVariables() {
			DocumentVariableCollection variables = ContentExporter.DocumentModel.Variables;
			if (variables.Count == 0 && ContentExporter.DocumentModel.DocumentProperties.UpdateDocVariablesBeforePrint == UpdateDocVariablesBeforePrint.Auto)
				return;
			foreach (string name in variables.GetVariableNames()) {
				RtfBuilder.OpenGroup();
				RtfBuilder.WriteCommand(RtfExportSR.DocumentVariable);
				RtfBuilder.OpenGroup();
				RtfBuilder.WriteText(name);
				RtfBuilder.CloseGroup();
				RtfBuilder.OpenGroup();
				object value = variables[name];
				if (value == null || value == DocVariableValue.Current)
					value = String.Empty;
				RtfBuilder.WritePCData(value.ToString());
				RtfBuilder.CloseGroup();
				RtfBuilder.CloseGroup();
			}
			UpdateDocVariablesBeforePrint updateDocVariablesBeforePrint = ContentExporter.DocumentModel.DocumentProperties.UpdateDocVariablesBeforePrint;
			if (updateDocVariablesBeforePrint != UpdateDocVariablesBeforePrint.Auto) {
				RtfBuilder.OpenGroup();
				RtfBuilder.WriteCommand(RtfExportSR.DocumentVariable);
				RtfBuilder.OpenGroup();
				RtfBuilder.WriteText(DocumentProperties.UpdateDocVariableFieldsBeforePrintDocVarName);
				RtfBuilder.CloseGroup();
				RtfBuilder.OpenGroup();
				RtfBuilder.WritePCData(ContentExporter.DocumentModel.DocumentProperties.GetUpdateFieldsBeforePrintDocVarValue());
				RtfBuilder.CloseGroup();
				RtfBuilder.CloseGroup();
			}
		}
	}
	#endregion
}
