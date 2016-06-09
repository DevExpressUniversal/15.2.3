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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Layout;
using ModelUnit = System.Int32;
using LayoutUnit = System.Int32;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using System.Drawing;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Model {
	#region EnhancedSelectionManager
	public class EnhancedSelectionManager {
		#region Static
		public static int ColumnResizeDeltaInPixels = 8;
		public static int RowResizeDeltaInPixels = 5;
		#endregion
		#region Fields
		readonly Worksheet worksheet;
		readonly InnerSpreadsheetControl innerControl;
		#endregion
		public EnhancedSelectionManager(Worksheet worksheet, InnerSpreadsheetControl innerControl) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			Guard.ArgumentNotNull(innerControl, "innerControl");
			this.worksheet = worksheet;
			this.innerControl = innerControl;
		}
		#region Properties
		protected internal DocumentModel DocumentModel { get { return worksheet.Workbook; } }
		protected internal Worksheet Worksheet { get { return worksheet; } }
		protected internal bool IsContentEditable { get { return innerControl.IsEditable; } }
		protected internal bool IsInplaceEditorActive { get { return innerControl.IsInplaceEditorActive; } }
		#endregion
		public virtual bool ShouldSelectPicture(SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult == null || !IsContentEditable)
				return false;
			if (hitTestResult.PictureBox == null || DocumentModel.ReferenceEditMode)
				return false;
			ChartBox chartBox = hitTestResult.PictureBox as ChartBox;
			if (chartBox != null && chartBox.Chart.Protection != ChartSpaceProtection.None)
				return false;
			WorksheetProtectionOptions protection = Worksheet.Properties.Protection;
			if (protection.SheetLocked && protection.ObjectsLocked) {
				if (!hitTestResult.PictureBox.Drawing.LocksWithSheet)
					return true;
				return false;
			}
			else
				return true;
		}
		public bool ShouldSelectComment(SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult == null || !IsContentEditable)
				return false;
			if (hitTestResult.CommentBox == null)
				return false;
			WorksheetProtectionOptions protection = Worksheet.Properties.Protection;
			if (protection.SheetLocked && protection.ObjectsLocked)
				return false;
			return true;
		}
		public virtual bool IsHyperlinkActive(SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult == null)
				return false;
			if (hitTestResult.PictureBox == null)
				return IsHyperlinkActiveCore(hitTestResult);
			IDrawingObject drawing = Worksheet.DrawingObjects[hitTestResult.PictureBox.DrawingIndex];
			if (drawing == null || KeyboardHandler.IsControlPressed)
				return false;
			bool isDrawingSelected = Worksheet.Selection.SelectedDrawingIndexes.Contains(drawing.IndexInCollection);
			if (isDrawingSelected) 
				return false;
			return !String.IsNullOrEmpty(drawing.DrawingObject.Properties.HyperlinkClickUrl);
		}
		bool IsHyperlinkActiveCore(SpreadsheetHitTestResult hitTestResult) {
			ICell hostCell = GetHostCell(hitTestResult);
			int hyperlinkIndex = Worksheet.Hyperlinks.GetHyperlink(hostCell);
			if (hyperlinkIndex < 0)
				return false;
			IActualCellAlignmentInfo alignmentInfo = hostCell.ActualAlignment;
			if (alignmentInfo.Indent != 0 || alignmentInfo.WrapText)
				return true;
			NumberFormatResult formatResult = GetFormatResult(hostCell);
			if (String.IsNullOrEmpty(formatResult.Text))
				return true;
			int index = Algorithms.BinarySearch(hitTestResult.Page.Boxes, new SingleCellTextBoxCellPositionComparable(hitTestResult.Page, hitTestResult.CellPosition));
			if (index >= 0) {
				SingleCellTextBox box = hitTestResult.Page.Boxes[index];
				Rectangle cellBounds = box.GetBounds(hitTestResult.Page);
				return IsPointOverSingleLineCellText(hostCell, formatResult.Text, cellBounds, hitTestResult, box);
			}
			foreach (ComplexCellTextBox complexBox in hitTestResult.Page.ComplexBoxes) {
				Rectangle cellBounds = complexBox.GetBounds(hitTestResult.Page);
				if (cellBounds.Contains(hitTestResult.LogicalPoint))
					return IsPointOverSingleLineCellText(hostCell, formatResult.Text, cellBounds, hitTestResult, complexBox);
			}
			return false;
		}
		NumberFormatResult GetFormatResult(ICell hostCell) {
			NumberFormatParameters parameters = new NumberFormatParameters();
			parameters.Measurer = new CellFormatStringMeasurer(hostCell);
			parameters.AvailableSpaceWidth = Int32.MaxValue;
			NumberFormatResult formatResult = hostCell.GetFormatResult(parameters);
			return formatResult;
		}
		ICell GetHostCell(SpreadsheetHitTestResult hitTestResult) {
			ICell hostCell = Worksheet.TryGetCell(hitTestResult.CellPosition.Column, hitTestResult.CellPosition.Row) as ICell;
			if (hostCell == null)
				hostCell = new FakeCell(hitTestResult.CellPosition, Worksheet);
			CellRange mergedRange = Worksheet.MergedCells.GetMergedCellRange(hostCell);
			if (mergedRange != null)
				hostCell = Worksheet[mergedRange.TopLeft];
			return hostCell;
		}
		bool IsPointOverSingleLineCellText(ICell cell, string text, Rectangle cellBounds, SpreadsheetHitTestResult hitTestResult, ICellTextBox textBox) {
			int textWidth = GetTextWidth(cell, text);
			Rectangle horizontalTextArea = textBox.CalculateActualTextBounds(hitTestResult.Page, hitTestResult.DocumentLayout, textWidth, cell.ActualHorizontalAlignment);
			int textHeight = cell.ActualFont.GetFontInfo().LineSpacing;
			int y = GetYCoordinade(horizontalTextArea, textHeight, cell.ActualAlignment.Vertical);
			Rectangle resultTextArea = new Rectangle(new Point(horizontalTextArea.X, y), new Size(horizontalTextArea.Width, textHeight));
			return resultTextArea.Contains(hitTestResult.LogicalPoint);
		}
		int GetTextWidth(ICell cell, string text) {
			CellFormatStringMeasurer measurer = new CellFormatStringMeasurer(cell);
			NumberFormatParameters parameters = new NumberFormatParameters();
			parameters.Measurer = measurer;
			parameters.AvailableSpaceWidth = Int32.MaxValue;
			int textWidth = CalculateSingleTextWidth(text, measurer);
			return textWidth;
		}
		int GetYCoordinade(Rectangle horizontalTextArea, int height, XlVerticalAlignment verticalAlignment) {
			switch (verticalAlignment) {
				case XlVerticalAlignment.Top:
					return horizontalTextArea.Y;
				case XlVerticalAlignment.Justify:
				case XlVerticalAlignment.Distributed:
				case XlVerticalAlignment.Bottom:
					return horizontalTextArea.Y + horizontalTextArea.Height - height;
				default:
					return horizontalTextArea.Y + (horizontalTextArea.Height - height) / 2;
			}
		}
		int CalculateSingleTextWidth(string text, CellFormatStringMeasurer measurer) {
			int width = measurer.MeasureStringWidth(text);
			if (width <= 0)
				return -1;
			return width;
		}
		public virtual bool ShouldSelectColumn(SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult == null)
				return false;
			return hitTestResult.HeaderBox != null && hitTestResult.HeaderBox.BoxType == HeaderBoxType.ColumnHeader;
		}
		public virtual bool ShouldResizeColumn(SpreadsheetHitTestResult hitTestResult, bool returnNearHeader) {
			if (!IsContentEditable || IsInplaceEditorActive)
				return false;
			WorksheetProtectionOptions protection = Worksheet.Properties.Protection;
			if (protection.SheetLocked && protection.FormatColumnsLocked)
				return false;
			if (!DocumentModel.BehaviorOptions.Column.ResizeAllowed)
				return false;
			if (!ShouldSelectColumn(hitTestResult))
				return false;
			Rectangle headerBounds = hitTestResult.HeaderBox.Bounds;
			int columnCorrection = CalculateActualColumnResizeDelta(headerBounds.Width);
			int onePixel = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(1, DocumentModel.DpiX);
			if (hitTestResult.LogicalPoint.X >= (headerBounds.Right - columnCorrection - onePixel))
				return true;
			if (hitTestResult.HeaderBox.Previous.BoxType == HeaderBoxType.SelectAllButton && hitTestResult.HeaderBox.ModelIndex == 0)
				return false;
			if (headerBounds.Left + columnCorrection > hitTestResult.LogicalPoint.X) {
				if (Worksheet.ActiveView.IsFrozen()
					&& PreviousColumnNotResized(hitTestResult))
					return false;
				if (returnNearHeader)
					hitTestResult.HeaderBox = hitTestResult.HeaderBox.Previous;
				return true;
			}
			return false;
		}
		internal int CalculateActualColumnResizeDelta(int columnHeaderWidth) {
			return CalculateActualResizeDelta(columnHeaderWidth, ColumnResizeDeltaInPixels, DocumentModel.DpiX);
		}
		bool PreviousColumnNotResized(SpreadsheetHitTestResult hitTestResult) {
			Column column = Worksheet.Columns.TryGetColumn(hitTestResult.HeaderBox.ModelIndex - 1);
			if (column != null)
				return !column.IsHidden;
			return Worksheet.ActiveView.SplitTopLeftCell.Column == hitTestResult.HeaderBox.ModelIndex;
		}
		public virtual bool ShouldSelectRow(SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult == null)
				return false;
			return hitTestResult.HeaderBox != null && hitTestResult.HeaderBox.BoxType == HeaderBoxType.RowHeader;
		}
		public virtual bool ShouldResizeRow(SpreadsheetHitTestResult hitTestResult, bool returnNearHeader) {
			if (!IsContentEditable || IsInplaceEditorActive)
				return false;
			WorksheetProtectionOptions protection = Worksheet.Properties.Protection;
			if (protection.SheetLocked && protection.FormatColumnsLocked)
				return false;
			if (!DocumentModel.BehaviorOptions.Row.ResizeAllowed)
				return false;
			if (!ShouldSelectRow(hitTestResult))
				return false;
			Rectangle headerBounds = hitTestResult.HeaderBox.Bounds;
			int rowCorrection = CalculateActualRowResizeDelta(headerBounds.Height);
			int onePixel = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(1, DocumentModel.DpiX);
			if (hitTestResult.LogicalPoint.Y >= (headerBounds.Bottom - rowCorrection - onePixel))
				return true;
			if (hitTestResult.HeaderBox.Previous.BoxType == HeaderBoxType.SelectAllButton && hitTestResult.HeaderBox.ModelIndex == 0)
				return false;
			if (headerBounds.Top + rowCorrection > hitTestResult.LogicalPoint.Y) {
				if (Worksheet.ActiveView.IsFrozen()
					&& PreviousRowNotResized(hitTestResult))
					return false;
				if (returnNearHeader)
					hitTestResult.HeaderBox = hitTestResult.HeaderBox.Previous;
				return true;
			}
			return false;
		}
		internal int CalculateActualRowResizeDelta(int rowHeaderHeight) {
			return CalculateActualResizeDelta(rowHeaderHeight, RowResizeDeltaInPixels, DocumentModel.DpiY);
		}
		int CalculateActualResizeDelta(int size, int maxDelta, float dpi) {
			int delta = DocumentModel.LayoutUnitConverter.PixelsToLayoutUnits(maxDelta, dpi);
			int piece = (int)Math.Floor(size / 3.0f);
			return Math.Min(piece, delta);
		}
		bool PreviousRowNotResized(SpreadsheetHitTestResult hitTestResult) {
			Row row = Worksheet.Rows.TryGetRow(hitTestResult.HeaderBox.ModelIndex - 1);
			if (row != null)
				return !row.IsHidden;
			return Worksheet.ActiveView.SplitTopLeftCell.Row == hitTestResult.HeaderBox.ModelIndex;
		}
		public virtual bool ShouldSelectAll(SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult == null || hitTestResult.HeaderBox == null)
				return false;
			return hitTestResult.HeaderBox.BoxType == HeaderBoxType.SelectAllButton;
		}
		public virtual bool ShouldSelectGroup(SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult == null || hitTestResult.GroupBox == null)
				return false;
			return hitTestResult.GroupBox != null;
		}
	}
	#endregion
}
