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
using System.IO;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraExport.Helpers {
	public class ClipboardRTFExporter<TCol, TRow> : IClipboardExporter<TCol,TRow>
		where TRow : class, IRowBase
		where TCol : class, IColumn {
		public ClipboardRTFExporter(bool exportRtf, bool exportHtml) {																					   
			this.exportRtf = exportRtf;
			this.exportHtml = exportHtml;
		}					
		bool exportRtf;
		bool exportHtml;
		RichEditDocumentServer provider;
		DocumentModel documentModel;
		PieceTable pieceTable;
		InputPosition inputPosition;
		Table table;
		List<TableRow> groupRows;
		public void BeginExport() {
			groupRows = new List<TableRow>();
			provider = new RichEditDocumentServer();
			provider.CreateNewDocument();
			documentModel = provider.Model;
			pieceTable = documentModel.MainPieceTable;
			documentModel.BeginSetContent();
			inputPosition = new InputPosition(pieceTable);
			table = new Table(pieceTable, null, 0, 0);
			table.TableProperties.BeginInit();
			table.TableProperties.PreferredWidth.Value = 100 * 50;
			table.TableProperties.PreferredWidth.Type = WidthUnitType.FiftiethsOfPercent;
			table.TableProperties.EndInit();
			table.SetTableStyleIndexCore(documentModel.TableStyles.GetStyleIndexByName(TableStyleCollection.TableSimpleStyleName));
			table.TableProperties.TableLayout = TableLayoutType.Autofit;
		}
		public void EndExport() {
			pieceTable.Tables.Add(table);
			mergeGroupRows();
			documentModel.EndSetContent(DocumentModelChangeType.LoadNewDocument, false, new FieldUpdateOnLoadOptions(false, false));
		}
		public void AddHeaders(IEnumerable<TCol> selectedColumns, IEnumerable<Export.Xl.XlCellFormatting> appearance) {
			if(selectedColumns.Count() == 0) return;
			TableRow row = new TableRow(table, 0);
			table.Rows.AddInternal(row);
			int count = selectedColumns.Count();
			for(int n = 0; n < selectedColumns.Count(); n++) {
				Export.Xl.XlCellFormatting format = appearance.ElementAt(n);
				ParagraphIndex startParagraphIndex = inputPosition.ParagraphIndex;
				MergedProperties<CharacterFormattingInfo, CharacterFormattingOptions> cp = getFontProperties(format);
				inputPosition.CharacterFormatting.CopyFrom(cp.Info, cp.Options);
				pieceTable.InsertTextCore(inputPosition, selectedColumns.ElementAt(n).Header);
				pieceTable.InsertParagraphCore(inputPosition);
				ParagraphIndex lastParagraphIndex = inputPosition.ParagraphIndex - 1;
				TableCell cell = new TableCell(row);
				row.Cells.AddInternal(cell);
				cell.Properties.BeginInit();
				cell.Properties.PreferredWidth.Value = 100 * 50 / count;
				cell.Properties.PreferredWidth.Type = WidthUnitType.FiftiethsOfPercent;
				cell.Properties.EndInit();
				pieceTable.TableCellsManager.InitializeTableCell(cell, startParagraphIndex, lastParagraphIndex);
				if(format != null) {
					cell.BackgroundColor = format.Fill.BackColor;
					cell.ForegroundColor = format.Fill.ForeColor;
				} else {
				}
			}
		}
		public void AddGroupHeader(string groupHeader, Export.Xl.XlCellFormatting appearance, int columnsCount) {
			TableRow row = new TableRow(table, 0);
			table.Rows.AddInternal(row);
			groupRows.Add(row);
			InputPosition oldFormatting = inputPosition.Clone();
			inputPosition.CharacterFormatting.FontBold = true;
			bool added = false;
			for(int i = 0; i < columnsCount; i++) {
				ParagraphIndex startParagraphIndex = inputPosition.ParagraphIndex;
				if(!added) {
					added = true;
					MergedProperties<CharacterFormattingInfo, CharacterFormattingOptions> cp = getFontProperties(appearance);
					inputPosition.CharacterFormatting.CopyFrom(cp.Info, cp.Options);
					pieceTable.InsertTextCore(inputPosition, groupHeader);
				}
				pieceTable.InsertParagraphCore(inputPosition);
				ParagraphIndex lastParagraphIndex = inputPosition.ParagraphIndex - 1;
				TableCell cell = new TableCell(row);
				row.Cells.AddInternal(cell);
				cell.Properties.BeginInit();
				cell.Properties.PreferredWidth.Value = 100 * 50 / columnsCount;
				cell.Properties.PreferredWidth.Type = WidthUnitType.FiftiethsOfPercent;
				cell.Properties.EndInit();
				pieceTable.TableCellsManager.InitializeTableCell(cell, startParagraphIndex, lastParagraphIndex);
				if(appearance != null) {
					cell.BackgroundColor = appearance.Fill.BackColor;
					cell.ForegroundColor = appearance.Fill.ForeColor;
				}
			}
			inputPosition.CopyFormattingFrom(oldFormatting);
		}
		public void AddRow(IEnumerable<ClipboardCellInfo> rowInfo) {
			TableRow row = new TableRow(table, 0);
			table.Rows.AddInternal(row);
			int count = rowInfo.Count();
			InputPosition oldFormatting = inputPosition.Clone();
			for(int n = 0; n < rowInfo.Count(); n++) {
				ClipboardCellInfo cellInfo = rowInfo.ElementAt(n);
				ParagraphIndex startParagraphIndex = inputPosition.ParagraphIndex;
				Export.Xl.XlCellFormatting format = cellInfo.Formatting;
				if(!String.IsNullOrEmpty(cellInfo.DisplayValue)) {
					if(format != null) {
						MergedProperties<CharacterFormattingInfo, CharacterFormattingOptions> cp = getFontProperties(format);
						inputPosition.CharacterFormatting.CopyFrom(cp.Info, cp.Options);
					}
					pieceTable.InsertTextCore(inputPosition, cellInfo.DisplayValue);
				} else {
					if(isValidImage(cellInfo.Value as byte[]))
						pieceTable.AppendImage(inputPosition, Office.Utils.OfficeImage.CreateImage(new System.IO.MemoryStream(cellInfo.Value as byte[])), 100.0f, 100.0f);
				}
				pieceTable.InsertParagraphCore(inputPosition);
				Paragraph paragraph = pieceTable.Paragraphs[inputPosition.ParagraphIndex - 1];
				if(format != null) {
					paragraph.Alignment = getAlignment(format);
					paragraph.LeftIndent = (int)DevExpress.Office.Utils.Units.InchesToDocumentsF((float)format.Alignment.Indent);
				}
				ParagraphIndex lastParagraphIndex = inputPosition.ParagraphIndex - 1;
				TableCell cell = new TableCell(row);
				row.Cells.AddInternal(cell);
				cell.Properties.BeginInit();
				cell.Properties.PreferredWidth.Value = 100 * 50 / count;
				cell.Properties.PreferredWidth.Type = WidthUnitType.FiftiethsOfPercent;
				cell.Properties.EndInit();
				pieceTable.TableCellsManager.InitializeTableCell(cell, startParagraphIndex, lastParagraphIndex);
				if(format != null) {
					cell.BackgroundColor = format.Fill.BackColor;
					cell.ForegroundColor = format.Fill.ForeColor;
				}
			}
			inputPosition.CopyFormattingFrom(oldFormatting);
		}
		public void SetDataObject(DataObject data) {
			if(!provider.Document.IsEmpty) {
				if(exportRtf) data.SetText(provider.Document.RtfText, TextDataFormat.Rtf);
				if(exportHtml) {
					HtmlToClipboardConverter converter = new HtmlToClipboardConverter();
					data.SetData(DataFormats.Html, converter.Convert(provider.Document.HtmlText));
				}
			}
			#region include images to HTML
			#endregion
			if(provider != null) {
				provider.Dispose();
				provider = null;
			}
		}
		MergedProperties<CharacterFormattingInfo, CharacterFormattingOptions> getFontProperties(Export.Xl.XlCellFormatting format) {
			CharacterFormattingInfo fi = new CharacterFormattingInfo();
			CharacterFormattingOptions fo = new CharacterFormattingOptions();
			MergedProperties<CharacterFormattingInfo, CharacterFormattingOptions> result = new MergedProperties<CharacterFormattingInfo, CharacterFormattingOptions>(fi, fo);
			try {
				fo.UseFontBold = fi.FontBold = format.Font.Bold;
				fo.UseFontItalic = fi.FontItalic = format.Font.Italic;
				fo.UseFontStrikeoutType = format.Font.StrikeThrough;
				fi.FontStrikeoutType = format.Font.StrikeThrough ? StrikeoutType.Single : StrikeoutType.None;
				fo.UseFontName = true;
				fi.FontName = format.Font.Name;
				fo.UseFontUnderlineType = true;
				fi.FontUnderlineType = format.Font.Underline == Export.Xl.XlUnderlineType.Single ? UnderlineType.Single : UnderlineType.None;
				fo.UseForeColor = true;
				fi.ForeColor = format.Font.Color.Rgb;
				fi.DoubleFontSize = Convert.ToInt32(2 * format.Font.Size);
				fo.UseDoubleFontSize = true;
				result = new MergedProperties<CharacterFormattingInfo, CharacterFormattingOptions>(fi, fo);
			} catch { }
			return result;
		}
		ParagraphAlignment getAlignment(Export.Xl.XlCellFormatting format) {
			if(format.Alignment == null)
				return ParagraphAlignment.Justify;
			switch(format.Alignment.HorizontalAlignment) {
				case Export.Xl.XlHorizontalAlignment.Center:
				return ParagraphAlignment.Center;
				case Export.Xl.XlHorizontalAlignment.Left:
				return ParagraphAlignment.Left;
				case Export.Xl.XlHorizontalAlignment.Right:
				return ParagraphAlignment.Right;
				default:
				return ParagraphAlignment.Justify;
			}
		}
		bool isValidImage(byte[] bytes) {
			if(bytes == null) return false;
			try {
				using(MemoryStream ms = new MemoryStream(bytes))
					Image.FromStream(ms);
			} catch(ArgumentException) {
				return false;
			}
			return true;
		}
		void mergeGroupRows() {
			XtraRichEdit.Tables.Native.TableStructureBySelectionCalculator collectionCalculator = new XtraRichEdit.Tables.Native.TableStructureBySelectionCalculator(pieceTable);
			foreach(TableRow row in groupRows) {
				pieceTable.MergeCells(collectionCalculator.Calculate(row.FirstCell, row.LastCell));
			}
			groupRows = null;
		}
	}
	#region CF_HTML format helpers
	struct CFHtmlOffsets {
		public int StartHtmlByteOffset { get; set; }
		public int EndHTMLByteOffset { get; set; }
		public int StartFragmentByteOffset { get; set; }
		public int EndFragmentByteOffset { get; set; }
	}
	class HtmlToClipboardConverter {
		const string headerFormat = @"Version:0.9
StartHTML:{0:0000000000}
EndHTML:{1:0000000000}
StartFragment:{2:0000000000}
EndFragment:{3:0000000000}
";
		const string startFragmentComment = "<!--StartFragment-->";
		const string endFragmentComment = "<!--EndFragment-->";
		CFHtmlOffsets offsets;
		public HtmlToClipboardConverter() { }
		string getFormattedHeader() {
			return string.Format(headerFormat, 0, 0, 0, 0);
		}
		string prepareHtmlText(string text) {
			string res = text;
			res = res.Insert(res.IndexOf("<body>" + Environment.NewLine) + ("<body>" + Environment.NewLine).Length, startFragmentComment);
			res = res.Insert(res.IndexOf("</body>"), endFragmentComment);
			return res;
		}
		CFHtmlOffsets calcOffsets(string text) {
			CFHtmlOffsets offsets = new CFHtmlOffsets();
			offsets.StartHtmlByteOffset = Encoding.UTF8.GetByteCount(getFormattedHeader());
			offsets.EndHTMLByteOffset = offsets.StartHtmlByteOffset + Encoding.UTF8.GetByteCount(text);
			offsets.StartFragmentByteOffset = offsets.StartHtmlByteOffset + text.IndexOf(startFragmentComment) + Encoding.UTF8.GetByteCount(startFragmentComment);
			string fragment = text.Substring(0, text.IndexOf(endFragmentComment));
			offsets.EndFragmentByteOffset = offsets.StartHtmlByteOffset + Encoding.UTF8.GetByteCount(fragment);
			return offsets;
		}
		public string Convert(string htmlText) {
			string text = prepareHtmlText(htmlText);
			StringBuilder result = new StringBuilder();
			offsets = calcOffsets(text);
			result.Append(String.Format(headerFormat, offsets.StartHtmlByteOffset, offsets.EndHTMLByteOffset, offsets.StartFragmentByteOffset, offsets.EndFragmentByteOffset));
			result.Append(text);
			return Encoding.Default.GetString(Encoding.UTF8.GetBytes(result.ToString()));
		}
	}
	#endregion
}
