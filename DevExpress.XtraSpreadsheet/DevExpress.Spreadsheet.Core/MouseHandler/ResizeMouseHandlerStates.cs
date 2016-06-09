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
using System.Drawing;
using DevExpress.Utils.Commands;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.Compatibility.System.Drawing;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Mouse {
	#region ContinueResizeMouseHandlerState (abstract class)
	public abstract class ContinueResizeMouseHandlerState : SpreadsheetMouseHandlerState {
		#region fields
		readonly ResizeMouseHandlerStateStrategy platformStrategy;
		readonly HeaderTextBox resizableBox;
		readonly Point startPoint;
		readonly Size initialHeaderSize;
		readonly int zoomFactor;
		SpreadsheetHitTestResult lastTestResult;
		#endregion
		protected ContinueResizeMouseHandlerState(SpreadsheetMouseHandler mouseHandler, SpreadsheetHitTestResult hitTestResult)
			: base(mouseHandler) {
			this.platformStrategy = CreatePlatformStrategy();
			if (hitTestResult == null || hitTestResult.HeaderBox == null)
				return;
			this.resizableBox = hitTestResult.HeaderBox;
			this.initialHeaderSize = resizableBox.Bounds.Size;
			this.startPoint = hitTestResult.PhysicalPoint;
			this.zoomFactor = this.DocumentModel.ActiveSheet.ActiveView.ZoomScale;
		}
		protected Point StartPoint { get { return startPoint; } }
		protected Size InitialHeaderSize { get { return initialHeaderSize; } }
		protected int ZoomFactor { get { return zoomFactor; } } 
		public override bool StopClickTimerOnStart { get { return false; } }
		public override void Start() {
			base.Start();
			Control.CaptureMouse();
			platformStrategy.DrawReversibleLine(GetLineCoordinate(resizableBox.Bounds));
		}
		public override void Finish() {
			EndVisualFeedback();
			Control.ReleaseMouse();
			base.Finish();
		}
		public override void OnMouseUp(MouseEventArgs e) {
			HideVisualFeedback();
			SpreadsheetHitTestResult hitTestResult = CalculateHitTest(e);
			if (hitTestResult != null) {
				lastTestResult = hitTestResult;
				hitTestResult.HeaderBox = resizableBox;
			}
			DocumentModel.BeginUpdate();
			try {
				CommitResize(resizableBox, new Point(e.X, e.Y));
			}
			finally {
				DocumentModel.EndUpdate();
			}
			Control.InnerControl.Owner.Redraw();
			MouseHandler.SwitchToDefaultState();
		}
		public override void OnMouseMove(MouseEventArgs e) {
			SpreadsheetHitTestResult hitTestResult = CalculateHitTest(e);
			if (hitTestResult != null) {
				lastTestResult = hitTestResult;
				hitTestResult.HeaderBox = resizableBox;
			}
			bool isNeedRedraw = Resize(resizableBox, new Point(e.X, e.Y));
			if (!isNeedRedraw)
				return;
			Control.InnerControl.Owner.Redraw();
			platformStrategy.DrawReversibleLine(GetLineCoordinate(resizableBox.Bounds));
		}
		protected SpreadsheetHitTestResult CalculateHitTest(MouseEventArgs e) {
			return CalculateHitTest(new Point(e.X, e.Y));
		}
		protected SpreadsheetHitTestResult CalculateHitTest(Point pt) {
			SpreadsheetHitTestResult hitTestResult = Control.InnerControl.ActiveView.CalculatePageHitTest(pt);
			if (hitTestResult != null)
				return hitTestResult;
			if (lastTestResult == null)
				return null;
			Point point = new Point(pt.X, lastTestResult.LogicalPoint.Y);
			hitTestResult = Control.InnerControl.ActiveView.CalculatePageHitTest(point);
			if (hitTestResult != null)
				return hitTestResult;
			point = new Point(lastTestResult.LogicalPoint.X, pt.Y);
			return Control.InnerControl.ActiveView.CalculatePageHitTest(point);
		}
		protected internal void BeginVisualFeedback() {
			platformStrategy.BeginVisualFeedback();
		}
		protected internal void EndVisualFeedback() {
			platformStrategy.EndVisualFeedback();
		}
		protected internal void ShowVisualFeedback() {
			platformStrategy.ShowVisualFeedback();
		}
		protected internal void HideVisualFeedback() {
			platformStrategy.HideVisualFeedback();
		}
		protected abstract bool Resize(HeaderTextBox resizableBox, Point phisicalPoint);
		protected abstract void CommitResize(HeaderTextBox resizableBox, Point phisicalPoint);
		protected internal abstract ResizeMouseHandlerStateStrategy CreatePlatformStrategy();
		protected internal abstract int GetLineCoordinate(Rectangle bounds);
	}
	#endregion
	#region ContinueResizeColumnsMouseHandlerState
	public class ContinueResizeColumnsMouseHandlerState : ContinueResizeMouseHandlerState {
		public ContinueResizeColumnsMouseHandlerState(SpreadsheetMouseHandler mouseHandler, SpreadsheetHitTestResult hitTestResult)
			: base(mouseHandler, hitTestResult) {
		}
		protected internal override ResizeMouseHandlerStateStrategy CreatePlatformStrategy() {
			return MouseHandler.GetPlatformStrategyFactory().CreateResizeColumnMouseHandlerStateStrategy(this);
		}
		protected override bool Resize(HeaderTextBox resizableBox, Point phisicalPoint) {
			HeaderTextBox columnHeader = resizableBox;
			int resizeDelta = phisicalPoint.X - StartPoint.X;
			int resizeWidth = InitialHeaderSize.Width + (100 * resizeDelta) / ZoomFactor;
			if (resizeWidth < 0 && columnHeader.Width == 0)
				return false;
			resizeWidth = Math.Max(resizeWidth, 0);
			columnHeader.Width = resizeWidth;
			return true;
		}
		protected override void CommitResize(HeaderTextBox resizableBox, Point phisicalPoint) {
			if (phisicalPoint.X == StartPoint.X)
				return;
			int columnIndex = resizableBox.ModelIndex;
			float widthInCharacters = CalculateColumnWidthInCharacters(resizableBox.Width);
			bool isFirstColumn = resizableBox.Previous.BoxType == HeaderBoxType.SelectAllButton;
			CommitResizeCore(columnIndex, widthInCharacters, isFirstColumn);
		}
		float CalculateColumnWidthInCharacters(float widthInLayoutUnits) {
			int modelWidth = (int)DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(widthInLayoutUnits);
			int inPixels = DocumentModel.UnitConverter.ModelUnitsToPixels(modelWidth, DocumentModel.Dpi);
			IColumnWidthCalculationService service = DocumentModel.GetService<IColumnWidthCalculationService>();
			float widthInCharacters = service.ConvertLayoutsToCharacters(DocumentModel.ActiveSheet, widthInLayoutUnits, inPixels);
			return service.RemoveGaps(DocumentModel.ActiveSheet, widthInCharacters);
		}
		void CommitResizeCore(int columnIndex, float widthInCharacters, bool isFirstColumn) {
			if (widthInCharacters < 0 || widthInCharacters > 255)
				Control.InnerControl.ResetDocumentLayout();
			ResizeColumnCommand command = new ResizeColumnCommand(Control);
			ResizeColumnCommandArgument argument = new ResizeColumnCommandArgument();
			argument.ColumnIndex = columnIndex;
			argument.WidthInCharacters = Math.Max(0, Math.Min(255, widthInCharacters));
			argument.IsFirstColumn = isFirstColumn;
			IValueBasedCommandUIState<ResizeColumnCommandArgument> state = (IValueBasedCommandUIState<ResizeColumnCommandArgument>)command.CreateDefaultCommandUIState();
			state.EditValue = argument;
			command.ForceExecute(state);
		}
		protected internal override int GetLineCoordinate(Rectangle bounds) {
			return bounds.Right - 1;
		}
	}
	#endregion
	#region ContinueResizeRowsMouseHandlerState
	public class ContinueResizeRowsMouseHandlerState : ContinueResizeMouseHandlerState {
		public ContinueResizeRowsMouseHandlerState(SpreadsheetMouseHandler mouseHandler, SpreadsheetHitTestResult hitTestResult)
			: base(mouseHandler, hitTestResult) {
		}
		protected internal override ResizeMouseHandlerStateStrategy CreatePlatformStrategy() {
			return MouseHandler.GetPlatformStrategyFactory().CreateResizeRowMouseHandlerStateStrategy(this);
		}
		protected override bool Resize(HeaderTextBox resizableBox, Point phisicalPoint) {
			HeaderTextBox rowHeader = resizableBox;
			int resizeDelta = phisicalPoint.Y - StartPoint.Y;
			int resizeHeight = InitialHeaderSize.Height + (100 * resizeDelta) / ZoomFactor;
			if (resizeHeight < 0 && rowHeader.Height == 0)
				return false;
			resizeHeight = Math.Max(resizeHeight, 0);
			rowHeader.Height = resizeHeight;
			return true;
		}
		protected override void CommitResize(HeaderTextBox resizableBox, Point phisicalPoint) {
			if (phisicalPoint.Y == StartPoint.Y)
				return;
			int rowIndex = resizableBox.ModelIndex;
			float heightInModelUnits = DocumentModel.ToDocumentLayoutUnitConverter.ToModelUnits(resizableBox.Height);
			bool isFirstRow = resizableBox.Previous.BoxType == HeaderBoxType.SelectAllButton;
			CommitResizeCore(rowIndex, heightInModelUnits, isFirstRow);
		}
		void CommitResizeCore(int rowIndex, float height, bool isFirstRow) {
			int maxHeight = DocumentModel.UnitConverter.TwipsToModelUnits(Row.MaxHeightInTwips);
			if (height < 0 || height > maxHeight)
				Control.InnerControl.ResetDocumentLayout();
			ResizeRowCommand command = new ResizeRowCommand(Control);
			ResizeRowCommandArgument argument = new ResizeRowCommandArgument();
			argument.RowIndex = rowIndex;
			argument.Height = Math.Max(0, Math.Min(maxHeight, height));
			argument.IsFirstRow = isFirstRow;
			IValueBasedCommandUIState<ResizeRowCommandArgument> state = (IValueBasedCommandUIState<ResizeRowCommandArgument>)command.CreateDefaultCommandUIState();
			state.EditValue = argument;
			command.ForceExecute(state);
		}
		protected internal override int GetLineCoordinate(Rectangle bounds) {
			return bounds.Bottom - 1;
		}
	}
	#endregion
}
