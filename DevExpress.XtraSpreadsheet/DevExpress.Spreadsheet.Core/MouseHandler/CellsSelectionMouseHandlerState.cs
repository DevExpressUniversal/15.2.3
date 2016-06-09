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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.Utils.Commands;
using DevExpress.Services;
using DevExpress.Compatibility.System.Drawing;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Mouse {
	#region ContinueSelectionMouseHandlerStateBase (abstract class)
	public abstract class ContinueSelectionMouseHandlerStateBase : SpreadsheetMouseHandlerState, IKeyboardHandlerService {
		SpreadsheetHitTestResult lastTestResult;
		MouseEventArgs lastMouseMoveEventArgs;
		Page lastPage;
		Timer timer;
		Size scrollOffset;
		IKeyboardHandlerService keyboardService;
		bool selectionEditStarted;
		protected ContinueSelectionMouseHandlerStateBase(SpreadsheetMouseHandler mouseHandler)
			: base(mouseHandler) {
			INameBoxControl nameBox = (INameBoxControl)Control.GetService(typeof(INameBoxControl));
			if (nameBox != null)
				nameBox.SelectionMode = true;
		}
		protected SheetViewSelection ActiveSelection { get { return DocumentModel.ActiveSheet.GetActualSelection(); } }
		protected Page LastPage { get { return lastPage; } }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility", "CA1601")]
		public override void Start() {
			base.Start();
			this.timer = new Timer();
#if !SL
			this.timer.Interval = 100; 
#else
			this.timer.Interval = TimeSpan.FromMilliseconds(100);
#endif
			this.timer.Tick += OnScrollTimerTick;
			this.timer.Start();
			this.keyboardService = (IKeyboardHandlerService)Control.GetService(typeof(IKeyboardHandlerService));
			Control.RemoveService(typeof(IKeyboardHandlerService));
			Control.AddService(typeof(IKeyboardHandlerService), this);
		}
		public override void Finish() {
			ActiveSelection.FinishSelectionEdit();
			Control.RemoveService(typeof(IKeyboardHandlerService));
			Control.AddService(typeof(IKeyboardHandlerService), keyboardService);
			selectionEditStarted = false;
			this.timer.Stop();
			this.timer.Tick -= OnScrollTimerTick;
			this.timer.Dispose();
			base.Finish();
		}
		void OnScrollTimerTick(object sender, EventArgs e) {
			if (scrollOffset == Size.Empty)
				return;
			SpreadsheetView view = Control.InnerControl.ActiveView;
			if (scrollOffset.Width != 0)
				view.ScrollLineLeftRight(scrollOffset.Width);
			if (scrollOffset.Height != 0)
				view.ScrollLineUpDown(scrollOffset.Height);
			if (lastMouseMoveEventArgs != null)
				ContinueSelection(lastMouseMoveEventArgs);
		}
		protected SpreadsheetHitTestResult CalculateHitTest(MouseEventArgs e) {
			this.lastTestResult = CalculateHitTestCore(e);
			if (lastTestResult != null && lastTestResult.Page != null)
				this.lastPage = lastTestResult.Page;
			return this.lastTestResult;
		}
		protected virtual SpreadsheetHitTestResult CalculateHitTestCore(MouseEventArgs e) {
			this.scrollOffset = Size.Empty;
			Point physicalPoint = new Point(e.X, e.Y);
			SpreadsheetHitTestResult result = CalculateHitTest(physicalPoint);
			if (!ShouldCalculateAutoScrollHitTestResult(result))
				return result;
			return CalculateAutoScrollHitTestCore(physicalPoint);
		}
		protected Point GetLogicalPoint(Point physicalPoint) {
			return Control.InnerControl.ActiveView.CreateLogicalPoint(physicalPoint);
		}
		protected Point GetPhysicalPoint(Point logicalPoint) {
			return GetPhysicalPoint(logicalPoint.X, logicalPoint.Y);
		}
		protected Point GetPhysicalPoint(int logicalX, int logicalY) {
			float zoomFactor = Control.InnerControl.ActiveView.ZoomFactor;
			if (zoomFactor == 1.0f)
				return new Point(logicalX, logicalY);
			int x = (int)(Math.Round(logicalX * zoomFactor));
			int y = (int)(Math.Round(logicalY * zoomFactor));
			return new Point(x, y);
		}
		protected abstract bool ShouldCalculateAutoScrollHitTestResult(SpreadsheetHitTestResult result);
		protected virtual void CalculateAutoScrollParameters(Point point, Rectangle bounds) {
			CalculateHorizontalAutoScrollParameters(point, bounds);
			CalculateVerticalAutoScrollParameters(point, bounds);
		}
		protected virtual void CalculateHorizontalAutoScrollParameters(Point point, Rectangle bounds) {
			if (point.X < bounds.Left)
				scrollOffset.Width = -CalculateScrollValue(bounds.Left - point.X);
			if (point.X > bounds.Right)
				scrollOffset.Width = CalculateScrollValue(point.X - bounds.Right);
		}
		protected virtual void CalculateVerticalAutoScrollParameters(Point point, Rectangle bounds) {
			if (point.Y < bounds.Top)
				scrollOffset.Height = -CalculateScrollValue(bounds.Top - point.Y);
			if (point.Y > bounds.Bottom)
				scrollOffset.Height = CalculateScrollValue(point.Y - bounds.Bottom);
		}
		int CalculateScrollValue(int offset) {
			return 1 + (offset / 50);
		}
		protected internal virtual void ContinueSelection(MouseEventArgs e) {
			this.lastMouseMoveEventArgs = e;
			if (!selectionEditStarted && ActiveSelection.ActiveRange.CellCount > 1) {
				ActiveSelection.StartSelectionEdit();
				selectionEditStarted = true;
			}
		}
		public override void OnMouseUp(MouseEventArgs e) {
			INameBoxControl nameBox = (INameBoxControl)Control.GetService(typeof(INameBoxControl));
			if (nameBox != null)
				nameBox.SelectionMode = false;
			MouseHandler.SwitchToDefaultState();
		}
		public override void OnMouseMove(MouseEventArgs e) {
			ContinueSelection(e);
		}
		protected internal abstract SpreadsheetHitTestResult CalculateHitTest(Point pt);
		protected abstract SpreadsheetHitTestResult CalculateAutoScrollHitTestCore(Point point);
		protected Rectangle ValidateViewBounds(Rectangle viewBounds) {
			GroupItemsPage page = Control.InnerControl.DesignDocumentLayout.GroupItemsPage;
			if (page == null)
				return viewBounds;
			Size offset = page.GroupItemsOffset;
			int left = viewBounds.Left + (int)Math.Round(offset.Width * Control.InnerControl.ActiveView.ZoomFactor);
			int top = viewBounds.Top + (int)Math.Round(offset.Height * Control.InnerControl.ActiveView.ZoomFactor);
			return Rectangle.FromLTRB(left, top, viewBounds.Right, viewBounds.Bottom);
		}
		#region IKeyboardHandlerService
		void IKeyboardHandlerService.OnKeyDown(KeyEventArgs e) {
		}
		void IKeyboardHandlerService.OnKeyPress(KeyPressEventArgs e) {
		}
		void IKeyboardHandlerService.OnKeyUp(KeyEventArgs e) {
		}
		#endregion
	}
	#endregion
	#region ContinueSelectionByCellsMouseHandlerState
	public class ContinueSelectionByCellsMouseHandlerState : ContinueSelectionMouseHandlerStateBase {
		public ContinueSelectionByCellsMouseHandlerState(SpreadsheetMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		#region Properties
		public override bool AutoScrollEnabled { get { return true; } }
		public override bool CanShowToolTip { get { return true; } }
		public override bool StopClickTimerOnStart { get { return false; } }
		#endregion
		protected internal override SpreadsheetHitTestResult CalculateHitTest(Point pt) {
			return Control.InnerControl.ActiveView.CalculatePageHitTest(pt);
		}
		protected override bool ShouldCalculateAutoScrollHitTestResult(SpreadsheetHitTestResult result) {
			Rectangle viewBounds = ValidateViewBounds(Control.ViewBounds);
			if (result == null || result.Page == null)
				return true;
			if (result.HeaderBox == null && viewBounds.Contains(result.PhysicalPoint))
				return false;
			return true;
		}
		protected override SpreadsheetHitTestResult CalculateAutoScrollHitTestCore(Point point) {
			if (LastPage == null)
				return null;
			point = GetLogicalPoint(point);
			Rectangle bounds = Rectangle.FromLTRB(LastPage.GridColumns.ActualFirst.Near, LastPage.GridRows.ActualFirst.Near, LastPage.GridColumns.ActualLast.Near, LastPage.GridRows.ActualLast.Near);
			Rectangle viewBounds = ValidateViewBounds(Control.ViewBounds);
			int x = Math.Min(viewBounds.Right, Math.Min(Math.Max(point.X, bounds.Left), bounds.Right));
			int y = Math.Min(viewBounds.Bottom, Math.Min(Math.Max(point.Y, bounds.Top), bounds.Bottom));
			float zoomFactor = Control.InnerControl.ActiveView.ZoomFactor;
			Rectangle autoScrollArea = Rectangle.FromLTRB(bounds.Left, bounds.Top, (int)Math.Round(viewBounds.Right / zoomFactor), (int)Math.Round(viewBounds.Bottom / zoomFactor));
			CalculateAutoScrollParameters(point, autoScrollArea);
			return CalculateHitTest(GetPhysicalPoint(x, y));
		}
		protected internal override void ContinueSelection(MouseEventArgs e) {
			base.ContinueSelection(e);
			SpreadsheetHitTestResult hitTestResult = CalculateHitTest(e);
			if (hitTestResult == null)
				return;
			DocumentModel.BeginUpdateFromUI(); 
			try {
				ContinueSelectionCore(hitTestResult);
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected virtual void ContinueSelectionCore(SpreadsheetHitTestResult hitTestResult) {
			ActiveSelection.ExtendActiveRangeToPosition(hitTestResult.CellPosition);
		}
	}
	#endregion
	#region ContinueSelectionByColumnsMouseHandlerState
	public class ContinueSelectionByColumnsMouseHandlerState : ContinueSelectionMouseHandlerStateBase {
		public ContinueSelectionByColumnsMouseHandlerState(SpreadsheetMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		#region Properties
		public override bool StopClickTimerOnStart { get { return true; } }
		#endregion
		protected override void CalculateVerticalAutoScrollParameters(Point point, Rectangle bounds) {
		}
		Rectangle CalculateHeadersBounds() {
			Rectangle bounds = ValidateViewBounds(Control.ViewBounds);
			HeaderPage page = Control.InnerControl.DesignDocumentLayout.HeaderPage;
			if (page == null)
				return bounds;
			List<HeaderTextBox> boxes = page.ColumnBoxes;
			if (boxes == null || boxes.Count <= 0)
				return bounds;
			int x = Math.Min(bounds.Right, Math.Max(bounds.X, boxes[0].Bounds.Left));
			x = (int)Math.Round(x * Control.InnerControl.ActiveView.ZoomFactor);
			return Rectangle.FromLTRB(x, bounds.Top, bounds.Right, boxes[0].Bounds.Bottom);
		}
		protected override SpreadsheetHitTestResult CalculateAutoScrollHitTestCore(Point point) {
			Rectangle bounds = CalculateHeadersBounds();
			SpreadsheetView view = Control.InnerControl.ActiveView;
			int x = Math.Min(bounds.Right, view.HorizontalScrollController.ScrollBarAdapter.Value == 0 ? point.X : Math.Max(point.X, bounds.Left));
			int y = Math.Min(bounds.Bottom, Math.Max(point.Y, bounds.Top));
			CalculateAutoScrollParameters(point, bounds);
			return CalculateHitTest(new Point(x, y));
		}
		protected internal override SpreadsheetHitTestResult CalculateHitTest(Point pt) {
			return Control.InnerControl.ActiveView.CalculatePageHitTest(pt);
		}
		protected override bool ShouldCalculateAutoScrollHitTestResult(SpreadsheetHitTestResult result) {
			if (result == null || result.Page == null)
				return true;
			Point point = result.PhysicalPoint;
			Rectangle bounds = CalculateHeadersBounds();
			return point.X < bounds.Left || point.X > bounds.Right;
		}
		protected internal override void ContinueSelection(MouseEventArgs e) {
			base.ContinueSelection(e);
			SpreadsheetHitTestResult hitTestResult = CalculateHitTest(e);
			if (hitTestResult == null)
				return;
			int column;
			if (hitTestResult.HeaderBox == null)
				column = hitTestResult.CellPosition.Column;
			else if (hitTestResult.HeaderBox.BoxType == HeaderBoxType.ColumnHeader)
				column = hitTestResult.HeaderBox.ModelIndex;
			else if (hitTestResult.HeaderBox.BoxType == HeaderBoxType.SelectAllButton)
				column = 0;
			else
				return;
			DocumentModel.BeginUpdateFromUI();
			try {
				ContinueSelectionCore(column);
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected virtual void ContinueSelectionCore(int column) {
			ActiveSelection.ExtendActiveRangeToColumn(column);
		}
	}
	#endregion
	#region ContinueSelectionByRowsMouseHandlerState
	public class ContinueSelectionByRowsMouseHandlerState : ContinueSelectionMouseHandlerStateBase {
		public ContinueSelectionByRowsMouseHandlerState(SpreadsheetMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		#region Properties
		public override bool StopClickTimerOnStart { get { return true; } }
		#endregion
		protected override void CalculateHorizontalAutoScrollParameters(Point point, Rectangle bounds) {
		}
		Rectangle CalculateHeadersBounds() {
			Rectangle bounds = ValidateViewBounds(Control.ViewBounds);
			HeaderPage page = Control.InnerControl.DesignDocumentLayout.HeaderPage;
			if (page == null)
				return bounds;
			List<HeaderTextBox> boxes = page.RowBoxes;
			if (boxes == null || boxes.Count <= 0)
				return bounds;
			int y = Math.Min(bounds.Bottom, Math.Max(bounds.Y, boxes[0].Bounds.Top));
			y = (int)Math.Round(y * Control.InnerControl.ActiveView.ZoomFactor);
			return Rectangle.FromLTRB(bounds.Left, y, boxes[0].Bounds.Right, bounds.Bottom);
		}
		protected override SpreadsheetHitTestResult CalculateAutoScrollHitTestCore(Point point) {
			Rectangle bounds = CalculateHeadersBounds();
			CalculateAutoScrollParameters(point, bounds);
			SpreadsheetView view = Control.InnerControl.ActiveView;
			int y = Math.Min(bounds.Bottom, view.VerticalScrollController.ScrollBarAdapter.Value == 0 ? point.Y : Math.Max(point.Y, bounds.Top));
			int x = Math.Min(Math.Max(point.X, bounds.Left), bounds.Right);
			return CalculateHitTest(new Point(x, y));
		}
		protected internal override SpreadsheetHitTestResult CalculateHitTest(Point pt) {
			return Control.InnerControl.ActiveView.CalculatePageHitTest(pt);
		}
		protected override bool ShouldCalculateAutoScrollHitTestResult(SpreadsheetHitTestResult result) {
			if (result == null || result.Page == null)
				return true;
			Point point = result.PhysicalPoint;
			Rectangle bounds = CalculateHeadersBounds();
			return point.Y < bounds.Top || point.Y > bounds.Bottom;
		}
		protected internal override void ContinueSelection(MouseEventArgs e) {
			base.ContinueSelection(e);
			SpreadsheetHitTestResult hitTestResult = CalculateHitTest(e);
			if (hitTestResult == null)
				return;
			int row;
			if (hitTestResult.HeaderBox == null)
				row = hitTestResult.CellPosition.Row;
			else if (hitTestResult.HeaderBox.BoxType == HeaderBoxType.RowHeader)
				row = hitTestResult.HeaderBox.ModelIndex;
			else if (hitTestResult.HeaderBox.BoxType == HeaderBoxType.SelectAllButton)
				row = 0;
			else 
				return;
			DocumentModel.BeginUpdateFromUI();
			try {
				ContinueSelectionCore(row);
			}
			finally {
				DocumentModel.EndUpdateFromUI();
			}
		}
		protected virtual void ContinueSelectionCore(int row) {
			ActiveSelection.ExtendActiveRangeToRow(row);
		}
	}
	#endregion
}
