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
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Office.Layout;
using DevExpress.XtraPrinting;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Printing;
#if !SL
using System.Security.Permissions;
using System.Windows.Forms;
using DevExpress.Data.Helpers;
using DevExpress.XtraPrinting.Native;
using DevExpress.Office.Printing;
using DevExpress.Office.Drawing;
#else
using System.Windows.Controls;
#endif
namespace DevExpress.XtraSpreadsheet.Printing {
	using System.Collections.Generic;
	using DevExpress.XtraPrinting.Native;
	#region SpreadsheetPrinter
	public class SpreadsheetPrinter : IPrintable {
		readonly InnerSpreadsheetDocumentServer server;
		readonly DocumentModel documentModel;
		DocumentLayout documentLayout;
		public SpreadsheetPrinter(InnerSpreadsheetDocumentServer server) {
			Guard.ArgumentNotNull(server, "server");
			this.server = server;
			this.documentModel = server.DocumentModel;
		}
		public DocumentModel DocumentModel { get { return documentModel; } }
		protected internal virtual bool UseGdiPlus { get { return DocumentModel.PrintOptions.RenderMode == SpreadsheetPrintRenderMode.GdiPlus; } }
		#region IPrintable Members
		public bool CreatesIntersectedBricks { get { return true; } }
		public UserControl PropertyEditorControl { get { return null; } }
		public bool HasPropertyEditor() {
			return false;
		}
		public void AcceptChanges() {
		}
		public void RejectChanges() {
		}
		public void ShowHelp() {
		}
		public bool SupportsHelp() {
			return false;
		}
		#endregion
		#region IBasePrintable Members
		public void CreateArea(string areaName, IBrickGraphics graph) {
			if (areaName == SR.Detail) {
				PrintingDocumentExporter exporter = CreateDocumentExporter(documentLayout.DocumentModel);
				BrickGraphics brickGraphics = (BrickGraphics)graph;
				exporter.Export(documentLayout, brickGraphics.PrintingSystem);
			}
		}
		protected virtual PrintingDocumentExporter CreateDocumentExporter(DocumentModel documentModel) {
			PrintingDocumentExporter exporter = new PrintingDocumentExporter(documentModel);
			return exporter;
		}
		public void Initialize(IPrintingSystem ps, ILink link) {
			PrintingSystemBase printingSystem = (PrintingSystemBase)ps;
			this.documentLayout = CalculatePrintDocumentLayout();
			if (!UseGdiPlus) {
				DocumentLayoutUnitConverter unitConverter = documentLayout.DocumentModel.LayoutUnitConverter;
				if (unitConverter is DocumentLayoutUnitPixelsConverter)
					unitConverter = new DocumentLayoutUnitDocumentConverter();
				SwitchToGdiMode(printingSystem, unitConverter);
			}
			else
				SwitchToGdiPlusMode(printingSystem);
			HideUnsupportedCommands(printingSystem);
			printingSystem.Graph.PageUnit = GraphicsUnit.Document;
			printingSystem.ShowMarginsWarning = DocumentModel.PrintOptions.ShowMarginsWarning;
		}
		public void Finalize(IPrintingSystem ps, ILink link) {
		}
		#endregion
		protected internal virtual DocumentLayout CalculatePrintDocumentLayout() {
			DocumentModel targetDocumentModel = CreatePrintingDocumentModel(DocumentModel);
			DocumentLayout documentLayout = new DocumentLayout(targetDocumentModel);
			BeginDocumentRendering();
			try {
				WorksheetCollection sheets = targetDocumentModel.Sheets;
				int count = sheets.Count;
				for (int i = 0; i < count; i++)
					CalculateSheetLayout(sheets[i], i, documentLayout);
			}
			finally {
				EndDocumentRendering();
			}
			return documentLayout;
		}
		protected void CalculateSheetLayout(Worksheet sheet, int sheetIndex, DocumentLayout documentLayout) {
			CellRangeBase printRange = ShouldPrintSheet(sheet, sheetIndex);
			if (printRange != null) {
				DocumentLayoutCalculatorBase calculator = new DocumentLayoutCalculatorBase(documentLayout, sheet);
				foreach (var innerRange in printRange.GetAreasEnumerable()) {
					calculator.CalculateLayout(innerRange);
				}
			}
		}
		CellRangeBase ShouldPrintSheet(Worksheet sheet, int sheetIndex) {
			if ((SheetVisibleState)DocumentModel.PrintOptions.PrintSheetVisibility != sheet.VisibleState)
				return null;
			bool fastConditionToCheckIfSheetIsEmpty = sheet.Rows.Count <= 0 && sheet.DrawingObjects.Count <= 0
				&& (new PrintAreaCalculator(sheet).GetDefinedNameRange() == null);
			if (fastConditionToCheckIfSheetIsEmpty)
				return null;
			CellRangeBase printRange = sheet.GetPrintRangeUsingDefinedNameForPrinting();
			if (printRange == null)
				return null;
			DevExpress.Spreadsheet.BeforePrintSheetEventArgs args = new DevExpress.Spreadsheet.BeforePrintSheetEventArgs(sheet.Name, sheetIndex);
			bool allowed = server.RaiseBeforePrintSheet(args);
			if (!allowed)
				return null;
			return printRange;
		}
		protected internal virtual DocumentModel CreatePrintingDocumentModel(DocumentModel sourceDocumentModel) {
			sourceDocumentModel.DocumentCoreProperties.LastPrinted = DateTime.Now;
			server.RaiseDocumentPropertiesChanged(true, false);
			DocumentModel documentModelCopy = sourceDocumentModel.Clone(false);
			documentModelCopy.LayoutUnit = Office.DocumentLayoutUnit.Document;
			return documentModelCopy;
		}
		protected internal virtual void BeginDocumentRendering() {
		}
		protected internal virtual void EndDocumentRendering() {
		}
		protected internal virtual void SwitchToGdiMode(PrintingSystemBase ps, DocumentLayoutUnitConverter unitConverter) {
#if !SL
			IServiceContainer serviceContainer = ps;
			if (SecurityHelper.IsPermissionGranted(new SecurityPermission(SecurityPermissionFlag.UnmanagedCode))) {
				serviceContainer.RemoveService(typeof(GraphicsModifier));
				serviceContainer.AddService(typeof(GraphicsModifier), new SpreadsheetGdiGraphicsModifier(unitConverter));
				serviceContainer.RemoveService(typeof(Measurer));
				serviceContainer.AddService(typeof(Measurer), new SpreadsheetGdiMeasurer(unitConverter));
			}
#endif
		}
		protected internal virtual void SwitchToGdiPlusMode(PrintingSystemBase ps) {
#if !SL
			IServiceContainer serviceContainer = ps;
			serviceContainer.RemoveService(typeof(GraphicsModifier));
			serviceContainer.AddService(typeof(GraphicsModifier), new GdiPlusGraphicsModifier());
			serviceContainer.RemoveService(typeof(Measurer));
			serviceContainer.AddService(typeof(Measurer), new GdiPlusMeasurer(serviceContainer));
#endif
		}
		#region Unsupported commands list
		static PrintingSystemCommand[] unsupportedCommands = new PrintingSystemCommand[] {
			PrintingSystemCommand.DocumentMap,
			PrintingSystemCommand.EditPageHF,
			PrintingSystemCommand.ExportCsv,
			PrintingSystemCommand.ExportHtm,
			PrintingSystemCommand.ExportMht,
			PrintingSystemCommand.ExportRtf,
			PrintingSystemCommand.ExportTxt,
			PrintingSystemCommand.ExportXls,
			PrintingSystemCommand.Open,
			PrintingSystemCommand.Save,
			PrintingSystemCommand.PageMargins,
			PrintingSystemCommand.PageOrientation,
			PrintingSystemCommand.PageSetup,
			PrintingSystemCommand.PaperSize,
			PrintingSystemCommand.Scale,
			PrintingSystemCommand.SendCsv,
			PrintingSystemCommand.SendMht,
			PrintingSystemCommand.SendRtf,
			PrintingSystemCommand.SendTxt,
			PrintingSystemCommand.SendXls,
			PrintingSystemCommand.Watermark,
			PrintingSystemCommand.Parameters,
			PrintingSystemCommand.Customize
		};
		#endregion
		protected internal virtual void HideUnsupportedCommands(PrintingSystemBase ps) {
			ps.SetCommandVisibility(unsupportedCommands, CommandVisibility.None);
		}
	}
	#endregion
	public class SpreadsheetWorksheetPrinter : SpreadsheetPrinter {
		readonly int sheetIndex;
		public SpreadsheetWorksheetPrinter(InnerSpreadsheetDocumentServer server, int sheetIndex)
			: base(server) {
			this.sheetIndex = sheetIndex;
		}
		#region Properties
		protected internal int SheetIndex { get { return sheetIndex; } }
		#endregion
		protected internal override DocumentLayout CalculatePrintDocumentLayout() {
			DocumentModel targetDocumentModel = CreatePrintingDocumentModel(DocumentModel);
			DocumentLayout documentLayout = new DocumentLayout(targetDocumentModel);
			BeginDocumentRendering();
			try {
				CalculateSheetLayout(targetDocumentModel.Sheets[sheetIndex], sheetIndex, documentLayout);
			}
			finally {
				EndDocumentRendering();
			}
			return documentLayout;
		}
	}
	#region SpreadsheetGdiMeasurer
	public class SpreadsheetGdiMeasurer : GdiMeasurer {
		readonly DocumentLayoutUnitConverter unitConverter;
		public SpreadsheetGdiMeasurer(DocumentLayoutUnitConverter unitConverter)
			: base(unitConverter) {
			this.unitConverter = unitConverter;
		}
		public override SizeF MeasureString(string text, Font font, SizeF size, StringFormat stringFormat, GraphicsUnit pageUnit) {
			SizeF result = base.MeasureString(text, font, size, stringFormat, pageUnit);
			bool wrap = stringFormat != null && (stringFormat.FormatFlags & StringFormatFlags.NoWrap) == 0;
			if (wrap && size.Width > 0.0 && result.Width >= size.Width) {
				InnerSpreadsheetGdiMeasurer measurer = new InnerSpreadsheetGdiMeasurer(this.unitConverter);
				TextFormatter formatter = new TextFormatter(pageUnit, measurer);
				string[] lines = formatter.FormatMultilineText(text, font, size.Width, size.Height, stringFormat);
				float height = TextFormatter.CalculateHeightOfLines(font, lines.Length, pageUnit);
				float width = 0.0f;
				foreach (string line in lines) {
					SizeF lineSize = base.MeasureString(line, font, size, stringFormat, pageUnit);
					width = Math.Max(width, lineSize.Width);
				}
				result = new SizeF(width, height);
			}
			return result;
		}
	}
	#endregion
	#region InnerSpreadsheetGdiMeasurer
	class InnerSpreadsheetGdiMeasurer : GdiMeasurer {
		public InnerSpreadsheetGdiMeasurer(DocumentLayoutUnitConverter unitConverter)
			: base(unitConverter) {
		}
		public override SizeF MeasureString(string text, Font font, SizeF size, StringFormat stringFormat, GraphicsUnit pageUnit) {
			SizeF result = base.MeasureString(text, font, size, stringFormat, pageUnit);
			bool wrap = (stringFormat.FormatFlags & StringFormatFlags.NoWrap) == 0;
			if (wrap && size.Width > 0.0 && result.Width >= size.Width)
				result = new SizeF(size.Width, TextFormatter.CalculateHeightOfLines(font, 2, pageUnit));
			return result;
		}
	}
	#endregion
	#region SpreadsheetGdiGraphicsModifier
	public class SpreadsheetGdiGraphicsModifier : GdiGraphicsModifier {
		readonly DocumentLayoutUnitConverter unitConverter;
		public SpreadsheetGdiGraphicsModifier(DocumentLayoutUnitConverter unitConverter)
			: base(unitConverter) {
			UseGlyphs = false;
			UseClipBoundsWithoutGlyphs = false;
			this.unitConverter = unitConverter;
		}
		public override void DrawString(Graphics gr, string text, Font font, Brush brush, RectangleF bounds, StringFormat format) {
			bool noWrap = (format.FormatFlags & StringFormatFlags.NoWrap) != 0;
			if (noWrap)
				base.DrawString(gr, text, font, brush, bounds, format);
			else {
				Measurer measurer = new InnerSpreadsheetGdiMeasurer(this.unitConverter);
				TextFormatter formatter = new TextFormatter(gr.PageUnit, measurer);
				string[] lines = formatter.FormatMultilineText(text, font, bounds.Width, bounds.Height, format);
				format.FormatFlags |= StringFormatFlags.NoWrap;
				try {
					int linesCount = lines.Length;
					if (linesCount == 1)
						base.DrawString(gr, lines[0], font, brush, bounds, format);
					else {
						float height = TextFormatter.CalculateHeightOfLines(font, linesCount, gr.PageUnit);
						float lineSpacingInUnits = FontHelper.GetLineSpacing(font);
						float lineSpacing = GraphicsUnitConverter.Convert(lineSpacingInUnits, font.Unit, gr.PageUnit);
						StringAlignment savedLiveAlign = format.LineAlignment;
						if (height > bounds.Height && savedLiveAlign != StringAlignment.Near)
							format.LineAlignment = StringAlignment.Near;
						try {
							float yOffset = 0;
							if (format.LineAlignment == StringAlignment.Center)
								yOffset = (bounds.Height - height) / 2;
							else if (format.LineAlignment == StringAlignment.Far)
								yOffset = bounds.Height - height;
							format.LineAlignment = StringAlignment.Near;
							bounds.Offset(0, yOffset);
							foreach (string line in lines) {
								base.DrawString(gr, line, font, brush, bounds, format);
								bounds.Offset(0, lineSpacing);
							}
						}
						finally {
							format.LineAlignment = savedLiveAlign;
						}
					}
				}
				finally {
					format.FormatFlags &= ~StringFormatFlags.NoWrap;
				}
			}
		}
	}
	#endregion
}
