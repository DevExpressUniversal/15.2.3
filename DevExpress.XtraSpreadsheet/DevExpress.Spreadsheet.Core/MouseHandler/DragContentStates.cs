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
using System.Diagnostics;
using DevExpress.Services;
using DevExpress.Office.Utils;
using DevExpress.Office.Commands.Internal;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using LayoutDrawingBox = DevExpress.XtraSpreadsheet.Layout.DrawingBox;
using LayoutPictureBox = DevExpress.XtraSpreadsheet.Layout.PictureBox;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
using System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Mouse {
	#region CancellableMouseHandlerStateBase
	public abstract class CancellableMouseHandlerStateBase : SpreadsheetMouseHandlerState, IKeyboardHandlerService {
		#region Fields
		SpreadsheetHitTestResult currentHitTestResult;
		IKeyboardHandlerService previousKeyboardService;
		#endregion
		protected CancellableMouseHandlerStateBase(SpreadsheetMouseHandler mouseHandler, SpreadsheetHitTestResult hitTestResult)
			: base(mouseHandler) {
			Guard.ArgumentNotNull(hitTestResult, "hitTestResult");
			this.currentHitTestResult = hitTestResult;
		}
		#region Properties
		public override bool AutoScrollEnabled { get { return true; } }
		public override bool CanShowToolTip { get { return true; } }
		public override bool StopClickTimerOnStart { get { return false; } }
		protected internal abstract DocumentLayoutDetailsLevel HitTestDetailsLevel { get; }
		public virtual SpreadsheetHitTestResult CurrentHitTestResult { get { return currentHitTestResult; } set { currentHitTestResult = value; } }
		protected virtual bool AllowCancel { get { return true; } }
		#endregion
		public override void Start() {
			base.Start();
			this.previousKeyboardService = Control.InnerControl.GetService<IKeyboardHandlerService>();
			Control.RemoveService(typeof(IKeyboardHandlerService));
			Control.AddService(typeof(IKeyboardHandlerService), this);
			SetMouseCursor(CalculateMouseCursor());
			StartCore();
			BeginVisualFeedback();
		}
		protected internal virtual void StartCore() {
		}
		public override void Finish() {
			EndVisualFeedback();
			Control.RemoveService(typeof(IKeyboardHandlerService));
			Control.AddService(typeof(IKeyboardHandlerService), previousKeyboardService);
			base.Finish();
		}
		#region IKeyboardHandlerService Members
		public void OnKeyDown(KeyEventArgs e) {
			Keys key = e.KeyCode;
			if (AllowCancel && key.Equals(Keys.Escape)) {
				HideVisualFeedback();
				MouseHandler.SwitchToDefaultState();
				MouseHandler.State.OnMouseMove(MouseHandler.CreateFakeMouseMoveEventArgs());
			}
		}
		public void OnKeyPress(KeyPressEventArgs e) {
		}
		public void OnKeyUp(KeyEventArgs e) {
		}
		#endregion
		protected internal virtual SpreadsheetHitTestResult CalculateHitTest(Point point) {
			SpreadsheetHitTestRequest request = new SpreadsheetHitTestRequest();
			request.PhysicalPoint = point;
			request.DetailsLevel = HitTestDetailsLevel;
			request.Accuracy = HitTestAccuracy.NearestPageArea | HitTestAccuracy.NearestCell;
			SpreadsheetHitTestResult result = Control.InnerControl.ActiveView.HitTest(request);
			if (!result.IsValid(HitTestDetailsLevel))
				return null;
			else
				return result;
		}
		public override void OnMouseMove(MouseEventArgs e) {
			this.currentHitTestResult = CalculateHitTest(new Point(e.X, e.Y));
			if (currentHitTestResult == null)
				return;
			UpdateObjectProperties();
			HideVisualFeedback();
			ShowVisualFeedback();
		}
		protected internal virtual void UpdateObjectProperties() {
		}
		public override void OnMouseUp(MouseEventArgs e) {
			ApplyChanges(e);
		}
		protected internal void ApplyChanges(MouseEventArgs e) {
			HideVisualFeedback();
			CommitChanges(new Point(e.X, e.Y));
			MouseHandler.SwitchToDefaultState();
		}
		protected internal virtual void BeginVisualFeedback() {
		}
		protected internal virtual void EndVisualFeedback() {
		}
		protected internal abstract SpreadsheetCursor CalculateMouseCursor();
		protected internal abstract void CommitChanges(Point point);
		protected internal abstract void ShowVisualFeedback();
		protected internal abstract void HideVisualFeedback();
	}
	#endregion
	#region CancellableDragMouseHandlerStateBase (abstract class)
	public abstract class CancellableDragMouseHandlerStateBase : CancellableMouseHandlerStateBase {
		DataObject dataObject;
		protected CancellableDragMouseHandlerStateBase(SpreadsheetMouseHandler mouseHandler, SpreadsheetHitTestResult hitTestResult)
			: base(mouseHandler, hitTestResult) {
		}
		#region Properties
		protected internal virtual bool HandleDragDropManually { get { return true; } }
		protected internal DataObject DataObject { get { return dataObject; } }
		#endregion
		protected internal override void StartCore() {
			this.dataObject = CreateDataObject();
		}
		protected internal override void CommitChanges(Point point) {
			CommitDrag(point, null); 
		}
		public override void OnMouseMove(MouseEventArgs e) {
			if (HandleDragDropManually)
				ContinueDrag(new Point(e.X, e.Y), DragDropEffects.Move | DragDropEffects.Copy, DataObject);
		}
		public override void OnMouseWheel(MouseEventArgs e) {
			base.OnMouseWheel(e);
			ContinueDrag(new Point(e.X, e.Y), DragDropEffects.Move | DragDropEffects.Copy, null);
		}
		public override bool OnPopupMenu(MouseEventArgs e) {
			MouseHandler.SwitchToDefaultState();
			return false;
		}
		protected internal abstract DataObject CreateDataObject();
		protected internal abstract bool CommitDrag(Point point, IDataObject dataObject);
		protected internal abstract DragDropEffects ContinueDrag(Point point, DragDropEffects allowedEffects, IDataObject dataObject);
	}
	#endregion
	#region DragFloatingObjectManuallyMouseHandlerState
	public class DragFloatingObjectManuallyMouseHandlerState : CancellableDragMouseHandlerStateBase {
		#region Fields
		static readonly Point Unassigned = new Point(Int32.MinValue, Int32.MinValue);
		Point currentTopLeftCorner;
		Point initialLogicalClickPoint;
		OfficeImage feedbackImage;
		LayoutDrawingBox box;
		Point clickPointLogicalOffset;
		SpreadsheetHitTestResult currentHitTestResult;
		Rectangle initialShapeBounds;
		Rectangle initialContentBounds;
		readonly DragFloatingObjectManuallyMouseHandlerStateStrategy platformStrategy;
		float rotationAngle;
		Page page;
		#endregion
		public DragFloatingObjectManuallyMouseHandlerState(SpreadsheetMouseHandler mouseHandler, SpreadsheetHitTestResult hitTestResult)
			: base(mouseHandler, hitTestResult) {
			this.box = hitTestResult.PictureBox;
			System.Diagnostics.Debug.Assert(box != null);
			this.platformStrategy = CreatePlatformStrategy();
			this.initialLogicalClickPoint = hitTestResult.LogicalPoint;
			Rectangle boxBounds = box.Bounds;
			Point topLeftCorner = boxBounds.Location;
			this.clickPointLogicalOffset = new Point(topLeftCorner.X - initialLogicalClickPoint.X, topLeftCorner.Y - initialLogicalClickPoint.Y);
			this.initialShapeBounds = boxBounds;
			this.initialContentBounds = boxBounds;
			this.rotationAngle = DocumentModel.GetBoxRotationAngleInDegrees(box);
		}
		#region Properties
		protected internal override DocumentLayoutDetailsLevel HitTestDetailsLevel { get { return DocumentLayoutDetailsLevel.Cell; } }
		public Rectangle InitialShapeBounds { get { return initialShapeBounds; } }
		public Rectangle InitialContentBounds { get { return initialContentBounds; } }
		public float RotationAngle { get { return rotationAngle; } }
		public LayoutDrawingBox Box { get { return box; } }
		#endregion
		protected internal virtual DragFloatingObjectManuallyMouseHandlerStateStrategy CreatePlatformStrategy() {
			return MouseHandler.GetPlatformStrategyFactory().CreateDragFloatingObjectManuallyMouseHandlerStateStrategy(this);
		}
		protected internal override void StartCore() {
			base.StartCore();
		}
		protected internal override SpreadsheetCursor CalculateMouseCursor() {
			return SpreadsheetCursors.SizeAll;
		}
		protected internal override DataObject CreateDataObject() {
			return null;
		}
		bool CalculateHitTestAndCurrentPoint(Point point) {
			currentHitTestResult = CalculateHitTest(point);
			if (currentHitTestResult == null)
				return false;
			page = currentHitTestResult.Page;
			if (!IsValid())
				return false;
			currentTopLeftCorner = currentHitTestResult.LogicalPoint;
			currentTopLeftCorner.X += clickPointLogicalOffset.X;
			currentTopLeftCorner.Y += clickPointLogicalOffset.Y;
			return true;
		}
		public override bool OnPopupMenu(MouseEventArgs e) {
			ApplyChanges(e);
			return true;
		}
		protected internal override bool CommitDrag(Point point, IDataObject dataObject) {
			if (!CalculateHitTestAndCurrentPoint(point))
				return false;
			Control.BeginUpdate();
			try {
				Point logicalPoint = currentHitTestResult.LogicalPoint;
				int offsetX = logicalPoint.X - initialLogicalClickPoint.X;
				int offsetY = logicalPoint.Y - initialLogicalClickPoint.Y;
				DocumentModel.ActiveSheet.MoveDrawingInLayoutUnits(Box.DrawingIndex, offsetX, offsetY);
			}
			finally {
				Control.EndUpdate();
			}
			return true;
		}
		protected internal override DragDropEffects ContinueDrag(Point point, DragDropEffects allowedEffects, IDataObject dataObject) {
			if (!CalculateHitTestAndCurrentPoint(point))
				return DragDropEffects.None;
			HideVisualFeedback();
			ShowVisualFeedback();
			return allowedEffects & DragDropEffects.Move;
		}
		public virtual Matrix CreateVisualFeedbackTransform() {
			if (!IsValid())
				return null;
			return TransformMatrixExtensions.CreateTransformUnsafe(rotationAngle, new Rectangle(currentTopLeftCorner, initialShapeBounds.Size));
		}
		protected internal override void ShowVisualFeedback() {
			if (!IsValid())
				return;
			ShowVisualFeedbackCore(new Rectangle(currentTopLeftCorner, initialShapeBounds.Size), page, feedbackImage);
		}
		protected internal override void HideVisualFeedback() {
			HideVisualFeedbackCore(Rectangle.Empty, page);
		}
		protected internal bool IsValid() {
			return currentTopLeftCorner != Unassigned && page != null  && currentHitTestResult != null;
		}
		protected internal virtual void ShowVisualFeedbackCore(Rectangle bounds, Page page, OfficeImage image) {
			platformStrategy.ShowVisualFeedback(bounds, page, image);
		}
		protected internal virtual void HideVisualFeedbackCore(Rectangle bounds, Page page) {
			platformStrategy.HideVisualFeedback(bounds, page);
		}
		protected internal override void BeginVisualFeedback() {
			FeedbackImageFactory factory = new FeedbackImageFactory();
			this.feedbackImage = factory.CreateImage(Box);
			platformStrategy.BeginVisualFeedback();
		}
		protected internal override void EndVisualFeedback() {
			platformStrategy.EndVisualFeedback();
			if (this.feedbackImage != null) {
				this.feedbackImage.Dispose();
				this.feedbackImage = null;
			}
		}
	}
	#endregion
	#region DragRangeManuallyMouseHandlerStateBase (abstract class)
	public abstract class DragRangeManuallyMouseHandlerStateBase : CancellableDragMouseHandlerStateBase {
		#region Fields
		readonly DragRangeManuallyMouseHandlerStateStrategy platformStrategy;
		Page lastPage;
		Point cellOffset;
		Size originalRangeSize;
		Size scrollOffset;
		Timer timer;
		Point lastHittestPoint;
		#endregion
		protected DragRangeManuallyMouseHandlerStateBase(SpreadsheetMouseHandler mouseHandler, SpreadsheetHitTestResult hitTestResult)
			: base(mouseHandler, hitTestResult) {
			this.platformStrategy = CreatePlatformStrategy();
			Initialize();
		}
		#region Properties
		protected internal override DocumentLayoutDetailsLevel HitTestDetailsLevel { get { return DocumentLayoutDetailsLevel.Cell; } }
		protected abstract CellRange OriginalRange { get; }
		protected Rectangle LastFeedbackBounds { get; set; }
		protected Page LastPage { get { return lastPage; } set { lastPage = value; } }
		protected Point CellOffset { get { return this.cellOffset; } set { this.cellOffset = value; } }
		protected Size OriginalRangeSize { get { return originalRangeSize; } set { originalRangeSize = value; } }
		int MaxRowCount { get { return OriginalRange.Worksheet.MaxRowCount - 1; } }
		int MaxColumnCount { get { return OriginalRange.Worksheet.MaxColumnCount - 1; } }
		int RowOffset { get { return originalRangeSize.Height - 1; } }
		int ColumnOffset { get { return originalRangeSize.Width - 1; } }
		#endregion
		protected virtual void Initialize() {
			CellRange originalRange = OriginalRange;
			if (originalRange == null) {
				this.cellOffset = Point.Empty;
				this.originalRangeSize = new Size(1, 1);
			}
			else {
				CellPosition topLeft = originalRange.TopLeft;
				CellPosition currentPosition = CurrentHitTestResult.CellPosition;
				if (CellPosition.InvalidValue.EqualsPosition(currentPosition))
					this.cellOffset = Point.Empty;
				else
					this.cellOffset = new Point(currentPosition.Column - topLeft.Column, currentPosition.Row - topLeft.Row);
				this.originalRangeSize = new Size(originalRange.Width, originalRange.Height);
			}
		}
		protected internal virtual Rectangle CalculateCellBounds(SpreadsheetHitTestResult hitTestResult) {
			CellPosition position = hitTestResult.CellPosition;
			int rowIndex = CorrectPosition(position.Row, cellOffset.Y, RowOffset, MaxRowCount);
			int columnIndex = CorrectPosition(position.Column, cellOffset.X, ColumnOffset, MaxColumnCount);
			PageGrid gridRows = hitTestResult.Page.GridRows;
			PageGrid gridColumns = hitTestResult.Page.GridColumns;
			int firstRowGridIndex = GetFirstGridIndex(gridRows, rowIndex);
			int firstColumnGridIndex = GetFirstGridIndex(gridColumns, columnIndex);
			if (firstRowGridIndex < 0 || firstColumnGridIndex < 0)
				return Rectangle.Empty;
			int rectangleY = gridRows[firstRowGridIndex].Near;
			int rectangleX = gridColumns[firstColumnGridIndex].Near;
			int rectangleHeight = GetRectangleSize(gridRows, firstRowGridIndex, RowOffset);
			int rectangleWidth = GetRectangleSize(gridColumns, firstColumnGridIndex, ColumnOffset);
			return Normalize(Rectangle.FromLTRB(rectangleX, rectangleY, rectangleWidth, rectangleHeight));
		}
		int CorrectPosition(int index, int cellOffset, int rangeSize, int maxIndex) {
			if (index == 0)
				return 0;
			int correctedIndex = index - cellOffset;
			if (correctedIndex + rangeSize > maxIndex)
				return maxIndex - rangeSize;
			return correctedIndex;
		}
		int GetFirstGridIndex(PageGrid grid, int index) {
			int firstGridIndex = grid.LookupFarItem(index);
			if (firstGridIndex < 0 || grid.ActualLastIndex - firstGridIndex < 1)
				firstGridIndex = grid.ActualLastIndex - 1;
			return firstGridIndex;
		}
		int GetRectangleSize(PageGrid grid, int startIndex, int rangeSize) {
			int lastIndex = grid[startIndex].ModelIndex + rangeSize;
			int lastGridIndex = grid.LookupNearItem(lastIndex);
			if (lastGridIndex < 0) {
				grid = grid.OffsetGrid(rangeSize);
				lastGridIndex = grid.ActualLastIndex;
			}
			return grid[lastGridIndex].Far;
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Mobility", "CA1601")]
		public override void Start() {
			base.Start();
			Control.CaptureMouse();
			this.timer = new Timer();
#if !SL
			this.timer.Interval = 100; 
#else
			this.timer.Interval = TimeSpan.FromMilliseconds(100);
#endif
			this.timer.Tick += OnScrollTimerTick;
			this.timer.Start();
		}
		public override void Finish() {
			this.timer.Stop();
			this.timer.Tick -= OnScrollTimerTick;
			this.timer.Dispose();
			Control.ReleaseMouse();
			base.Finish();
		}
		protected internal virtual DragRangeManuallyMouseHandlerStateStrategy CreatePlatformStrategy() {
			return MouseHandler.GetPlatformStrategyFactory().CreateDragRangeManuallyMouseHandlerStateStrategy(this);
		}
		protected internal override DataObject CreateDataObject() {
			return null;
		}
		protected internal bool IsValid() {
			return LastPage != null && CurrentHitTestResult != null;
		}
		protected internal override DragDropEffects ContinueDrag(Point point, DragDropEffects allowedEffects, IDataObject dataObject) {
			SpreadsheetHitTestResult hitTestResult = CalculateHitTest(point);
			if (!IsValid())
				return DragDropEffects.None;
			UpdateVisualFeedback();
			return allowedEffects & DragDropEffects.Move;
		}
		protected internal override SpreadsheetCursor CalculateMouseCursor() {
			return SpreadsheetCursors.Default;
		}
		protected bool UpdateVisualFeedback() {
			Rectangle newFeedbackBounds = CalculateCellBounds(CurrentHitTestResult);
			if (LastFeedbackBounds == newFeedbackBounds)
				return false;
			this.LastFeedbackBounds = newFeedbackBounds;
			ShowVisualFeedbackCore(LastFeedbackBounds, LastPage);
			return true;
		}
		protected internal override void ShowVisualFeedback() {
		}
		protected internal override void HideVisualFeedback() {
		}
		protected internal virtual void ShowVisualFeedbackCore(Rectangle bounds, Page page) {
			platformStrategy.ShowVisualFeedback(bounds, page);
		}
		protected internal virtual void HideVisualFeedbackCore() {
			platformStrategy.HideVisualFeedback();
		}
		protected internal override void EndVisualFeedback() {
			HideVisualFeedbackCore();
		}
		protected internal override bool CommitDrag(Point point, IDataObject dataObject) {
			SpreadsheetHitTestResult hitTestResult = CalculateHitTest(point);
			if (!IsValid())
				return false;
			CellPosition position = hitTestResult.CellPosition;
			int rowIndex = CorrectPosition(position.Row, cellOffset.Y, RowOffset, MaxRowCount);
			int columnIndex = CorrectPosition(position.Column, cellOffset.X, ColumnOffset, MaxColumnCount);
			PageGrid gridRows = hitTestResult.Page.GridRows;
			PageGrid gridColumns = hitTestResult.Page.GridColumns;
			int firstRowGridIndex = GetFirstGridIndex(gridRows, rowIndex);
			int firstColumnGridIndex = GetFirstGridIndex(gridColumns, columnIndex);
			if (firstRowGridIndex < 0 || firstColumnGridIndex < 0)
				return false;
			CellPosition commitPosition = new CellPosition(gridColumns[firstColumnGridIndex].ModelIndex, gridRows[firstRowGridIndex].ModelIndex);
			return CommitDrag(commitPosition, dataObject);
		}
		protected internal abstract bool CommitDrag(CellPosition commitPosition, IDataObject dataObject);
		#region Scrolling
		public override void OnMouseMove(MouseEventArgs e) {
			Point p = new Point(e.X, e.Y);
			this.lastHittestPoint = p;
			CalculateHitTest(p);
			base.OnMouseMove(e);
		}
		void OnScrollTimerTick(object sender, EventArgs e) {
			if (scrollOffset == Size.Empty)
				return;
			SpreadsheetView view = Control.InnerControl.ActiveView;
			if (scrollOffset.Width != 0) {
				view.ScrollLineLeftRight(scrollOffset.Width);
			}
			if (scrollOffset.Height != 0) {
				view.ScrollLineUpDown(scrollOffset.Height);
			}
			CalculateHitTest(lastHittestPoint);
			LastFeedbackBounds = Rectangle.Empty;
			ContinueDrag(lastHittestPoint, DragDropEffects.Move | DragDropEffects.Copy, DataObject);
		}
		protected internal override SpreadsheetHitTestResult CalculateHitTest(Point pt) {
			SpreadsheetHitTestResult hitTestResult = CalculateHitTestCore(pt);
			SetCurrentHitTestResult(hitTestResult);
			return hitTestResult;
		}
		void SetCurrentHitTestResult(SpreadsheetHitTestResult hitTestResult) {
			CurrentHitTestResult = hitTestResult;
			if (CurrentHitTestResult != null) {
				if (CurrentHitTestResult.Page != null)
					LastPage = CurrentHitTestResult.Page;
				else
					LastFeedbackBounds = Rectangle.Empty;
			}
		}
		SpreadsheetHitTestResult CalculateHitTestOnActiveView(Point p) {
			return Control.InnerControl.ActiveView.CalculatePageHitTest(p);
		}
		protected virtual SpreadsheetHitTestResult CalculateHitTestCore(Point p) {
			this.scrollOffset = Size.Empty;
			SpreadsheetHitTestResult result = CalculateHitTestOnActiveView(p);
			if (!ShouldCalculateAutoScrollHitTestResult(result))
				return result;
			return CalculateAutoScrollHitTestCore(p);
		}
		protected virtual SpreadsheetHitTestResult CalculateAutoScrollHitTestCore(Point p) {
			if (LastPage == null)
				return null;
			Rectangle bounds = Rectangle.FromLTRB(LastPage.GridColumns.ActualFirst.Near,
												  LastPage.GridRows.ActualFirst.Near,
												  LastPage.GridColumns.ActualLast.Near,
												  LastPage.GridRows.ActualLast.Near);
			Rectangle viewBounds = Control.ViewBounds;
			int x = Math.Min(viewBounds.Right, Math.Min(Math.Max(p.X, bounds.Left), bounds.Right));
			int y = Math.Min(viewBounds.Bottom, Math.Min(Math.Max(p.Y, bounds.Top), bounds.Bottom));
			CalculateAutoScrollParameters(p, bounds);
			return CalculateHitTestOnActiveView(new Point(x, y));
		}
		protected bool ShouldCalculateAutoScrollHitTestResult(SpreadsheetHitTestResult result) {
			Rectangle viewBounds = Control.ViewBounds;
			if (result != null && result.HeaderBox == null) {
				if (viewBounds.Contains(result.PhysicalPoint))
					return false;
			}
			return true;
		}
		protected virtual void CalculateAutoScrollParameters(Point p, Rectangle bounds) {
			CalculateHorizontalAutoScrollParameters(p, bounds);
			CalculateVerticalAutoScrollParameters(p, bounds);
		}
		protected virtual void CalculateHorizontalAutoScrollParameters(Point p,
																	   Rectangle bounds) {
			if (p.X < bounds.Left)
				scrollOffset.Width = -CalculateScrollValue(bounds.Left - p.X);
			if (p.X > bounds.Right)
				scrollOffset.Width = CalculateScrollValue(p.X - bounds.Right);
		}
		protected virtual void CalculateVerticalAutoScrollParameters(Point p,
																	 Rectangle bounds) {
			if (p.Y < bounds.Top)
				scrollOffset.Height = -CalculateScrollValue(bounds.Top - p.Y);
			if (p.Y > bounds.Bottom)
				scrollOffset.Height = CalculateScrollValue(p.Y - bounds.Bottom);
		}
		int CalculateScrollValue(int offset) {
			return 1 + (offset / 50);
		}
		#endregion
	}
	#endregion
	#region DragRangeManuallyMouseHandlerState
	public class DragRangeManuallyMouseHandlerState : DragRangeManuallyMouseHandlerStateBase {
		public DragRangeManuallyMouseHandlerState(SpreadsheetMouseHandler mouseHandler, SpreadsheetHitTestResult hitTestResult)
			: base(mouseHandler, hitTestResult) {
		}
		protected override CellRange OriginalRange { get { return Control.InnerControl.DocumentModel.ActiveSheet.Selection.ActiveRange; } }
		protected internal override bool CommitDrag(CellPosition commitPosition, IDataObject dataObject) {
			DocumentModel documentModel = Control.InnerControl.DocumentModel;
			Worksheet sheet = Control.InnerControl.DocumentModel.ActiveSheet;
			CellRange sourceRange = sheet.Selection.ActiveRange;
			int right = commitPosition.Column + sourceRange.Width - 1;
			int bottom = commitPosition.Row + sourceRange.Height - 1;
			CellRange targetRange;
			if (commitPosition.Row == 0 && bottom == IndicesChecker.MaxRowIndex)
				targetRange = CellIntervalRange.CreateColumnInterval(sheet, commitPosition.Column, PositionType.Relative, right, PositionType.Relative);
			else if (commitPosition.Column == 0 && right == IndicesChecker.MaxColumnIndex)
				targetRange = CellIntervalRange.CreateRowInterval(sheet, commitPosition.Row, PositionType.Relative, bottom, PositionType.Relative);
			else
				targetRange = new CellRange(sheet, commitPosition, new CellPosition(right, bottom));
			if (IsTargetRangeIsAvailableToEraseContent(targetRange, sourceRange, sheet))
				MoveRange(documentModel, sheet, sourceRange, targetRange);
			else {
				LastFeedbackBounds = Rectangle.Empty;  
			}
			LastFeedbackBounds = Rectangle.Empty;
			return true;
		}
		void MoveRange(DocumentModel documentModel, Worksheet sheet, CellRange sourceRange, CellRange targetRange) {
			if (sheet.ReadOnly)
				return;
			if (sheet.Properties.Protection.SheetLocked && 
				(sourceRange.ContainsLockedCells() || targetRange.ContainsLockedCells())) {
				Control.InnerControl.ShowReadOnlyObjectMessage();
				return;
			}
			var ranges = new Model.CopyOperation.SourceTargetRangesForCopy(sourceRange, targetRange);
			Model.CopyOperation.RangeCopyOperation operation = (!KeyboardHandler.IsControlPressed) ?
				new Model.CopyOperation.CutRangeOperation(ranges)
				: new Model.CopyOperation.RangeCopyOperation(ranges, ModelPasteSpecialFlags.All);
			if (operation.RangesInfo.First.SourceAndTargetEquals)
				return;
			operation.ErrorHandler = Control.InnerControl.ErrorHandler;
			bool valid = operation.Validate();
			if (!valid)
				return;
			documentModel.BeginUpdate();
			try {
				operation.Execute();
				bool excecuted = true;
				if (!excecuted)
					targetRange = sourceRange;
				sheet.Selection.SetSelection(targetRange);
				if (excecuted && operation.CutMode) {
					Model.CellRangeBase rangeToClear = operation.GetRangeToClearAfterCut();
					System.Diagnostics.Debug.Assert(rangeToClear != null);
					sheet.ClearAll(rangeToClear, Control.InnerControl.ErrorHandler);
					sheet.ClearCellsNoShift(rangeToClear);
				}
			}
			catch (InvalidOperationException e) {
				Control.ShowWarningMessage(e.Message);
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		bool IsTargetRangeIsAvailableToEraseContent(CellRange targetRange, CellRange sourceRange, Worksheet sheet) {
			CellRangeBase rangeToCheck = targetRange;
			if (targetRange.EqualsPosition(sourceRange) && Object.ReferenceEquals(targetRange.Worksheet, sourceRange.Worksheet))
				return true;
			if (targetRange.Intersects(sourceRange))
				rangeToCheck = targetRange.ExcludeRange(sourceRange);
			bool allowReplaceTargetContent = true;
			bool rangeIsNotEmpty = rangeToCheck.Exists(range => sheet.RangeContainsNotEmptyCell(range));
			if (rangeIsNotEmpty)
				allowReplaceTargetContent = Control.ShowOkCancelMessage(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_CanReplaceTheContentsOfTheDestinationCells));
			return allowReplaceTargetContent;
		}
	}
	#endregion
	#region ResizeRangeManuallyMouseHandlerState
	public class ResizeRangeManuallyMouseHandlerState : DragRangeManuallyMouseHandlerStateBase {
		public ResizeRangeManuallyMouseHandlerState(SpreadsheetMouseHandler mouseHandler)
			: base(mouseHandler, new SpreadsheetHitTestResult(mouseHandler.Control.InnerControl.DesignDocumentLayout)) {
			this.CellOffset = Point.Empty;
			this.OriginalRangeSize = new Size(1, 1);
		}
		protected override CellRange OriginalRange { get { return Control.InnerControl.DocumentModel.ActiveSheet.Selection.ActiveRange; } }
		public override bool SuppressDefaultMouseWheelProcessing { get { return true; } }
		protected internal override SpreadsheetCursor CalculateMouseCursor() {
			return SpreadsheetCursors.SmallCross;
		}
		Tuple<int, int> CalculateTargetRange(int physicalPoint, PageGrid grid, PageGrid originalGrid, int originalRangeTopLeft, int hitTestResultCellPosition, int originalRangeBottomRight) {
			int limit;
			int delta;
			int firstCol;
			int lastCol;
			if (originalRangeTopLeft <= hitTestResultCellPosition) {
				firstCol = originalRangeTopLeft;
				lastCol = hitTestResultCellPosition;
				limit = grid.LookupNearItem(lastCol);
				if (limit != -1) {
					delta = physicalPoint - grid[limit].Near;
					if (delta < grid[limit].Extent / 2)
						if (firstCol != lastCol)
							lastCol--;
				}
			}
			else {
				firstCol = hitTestResultCellPosition;
				lastCol = originalRangeBottomRight;
				limit = originalGrid.LookupFarItem(firstCol);
				if (limit != -1) {
					delta = grid[limit].Far - physicalPoint;
					if (delta < grid[limit].Extent / 2)
						firstCol++;
				}
			}
			return new Tuple<int, int>(firstCol, lastCol);
		}
		Rectangle CalculateTargetRange(SpreadsheetHitTestResult hitTestResult) {
			int firstRow = OriginalRange.TopLeft.Row;
			int firstCol = OriginalRange.TopLeft.Column;
			int lastRow = OriginalRange.BottomRight.Row;
			int lastCol = OriginalRange.BottomRight.Column;
			if (IsHorizontalSelection(hitTestResult)) {
				PageGrid gridColumns = hitTestResult.Page.GridColumns;
				PageGrid originalGridColumns = LastPage != null ? LastPage.GridColumns : LastPage.GridColumns;
				Tuple<int, int> columnRange = CalculateTargetRange(hitTestResult.PhysicalPoint.X, gridColumns, originalGridColumns, firstCol, hitTestResult.CellPosition.Column, lastCol);
				firstCol = columnRange.Item1;
				lastCol = columnRange.Item2;
			}
			else {
				PageGrid gridRows = hitTestResult.Page.GridRows;
				PageGrid originalGridRows = LastPage != null ? LastPage.GridRows : LastPage.GridRows;
				Tuple<int, int> rowRange = CalculateTargetRange(hitTestResult.PhysicalPoint.Y, gridRows, originalGridRows, firstRow, hitTestResult.CellPosition.Row, lastRow);
				firstRow = rowRange.Item1;
				lastRow = rowRange.Item2;
			}
			return Rectangle.FromLTRB(firstCol, firstRow, lastCol, lastRow);
		}
		protected internal override bool CommitDrag(CellPosition commitPosition, IDataObject dataObject) {
			DocumentModel documentModel = Control.InnerControl.DocumentModel;
			Worksheet sheet = documentModel.ActiveSheet;
			CellRange sourceRange = sheet.Selection.ActiveRange;
			Rectangle targetRangeRect = CalculateTargetRange(CurrentHitTestResult);
			CellPosition targetTopLeft = new CellPosition(targetRangeRect.Left, targetRangeRect.Top);
			CellPosition targetBottomRight = new CellPosition(targetRangeRect.Right, targetRangeRect.Bottom);
			CellRange targetRange = new CellRange(sheet, targetTopLeft, targetBottomRight);
			ResizeRange(documentModel, sheet, sourceRange, targetRange);
			LastFeedbackBounds = Rectangle.Empty;
			return true;
		}
		void ResizeRange(DocumentModel documentModel, Worksheet sheet, CellRange sourceRange, CellRange targetRange) {
			if (sheet.ReadOnly || targetRange.Equals(sourceRange))
				return;
			if (!Control.InnerControl.TryEditRangeContent(targetRange))
				return;
			DevExpress.XtraSpreadsheet.Model.CopyOperation.RangeResizeOperation operation = new DevExpress.XtraSpreadsheet.Model.CopyOperation.RangeResizeOperation(sheet, sourceRange, targetRange, Control.InnerControl.ErrorHandler);
			documentModel.BeginUpdateFromUI();
			try {
				if (!operation.Execute())
					targetRange = sourceRange;
				sheet.Selection.SetSelection(targetRange);
				documentModel.ApplyChanges(DocumentModelChangeActions.Redraw | DocumentModelChangeActions.ResetInvalidDataCircles);
			}
			finally {
				documentModel.EndUpdateFromUI();
			}
			foreach(CellContentSnapshot snapshot in operation.AffectedCells.Values)
				documentModel.InternalAPI.RaiseCellValueChanged(snapshot);
		}
		PageGrid GetColumnsGrid() {
			if (LastPage != null)
				return LastPage.GridColumns;
			else
				return LastPage.GridColumns;
		}
		float AccumulateColumnWidth(int from, int to) {
			AccumulatedOffset accumulatedOffset = new AccumulatedOffset(Control.InnerControl.DocumentModel.ActiveSheet);
			accumulatedOffset.AddColumns(from, to);
			return accumulatedOffset.ToLayout();
		}
		float CalculateSelectionWidth(SpreadsheetHitTestResult hitTestResult) {
			PageGrid gridColumns = GetColumnsGrid();
			int nearColumn = gridColumns.LookupNearItem(hitTestResult.CellPosition.Column);
			if (OriginalRange.ContainsCell(hitTestResult.CellPosition.Column, hitTestResult.CellPosition.Row)) {
				float width = AccumulateColumnWidth(hitTestResult.CellPosition.Column, OriginalRange.BottomRight.Column);
				if (nearColumn != -1)
					width -= hitTestResult.PhysicalPoint.X - gridColumns[nearColumn].Near;
				return width;
			}
			else {
				int left = 0;
				int right = 0;
				int delta = 0;
				if (OriginalRange.BottomRight.Column < hitTestResult.CellPosition.Column) {
					left = OriginalRange.BottomRight.Column + 1;
					right = hitTestResult.CellPosition.Column;
					if (nearColumn != -1)
						delta = hitTestResult.PhysicalPoint.X - gridColumns[nearColumn].Near;
				}
				else if (OriginalRange.TopLeft.Column > hitTestResult.CellPosition.Column) {
					left = hitTestResult.CellPosition.Column + 1;
					right = OriginalRange.TopLeft.Column;
					if (nearColumn != -1)
						delta = gridColumns[nearColumn].Far - hitTestResult.PhysicalPoint.X;
				}
				float width = AccumulateColumnWidth(left, right - 1);
				if (delta >= gridColumns[nearColumn].Extent / 2)
					width += delta;
				return width;
			}
		}
		PageGrid GetRowsGrid() {
			if (LastPage != null)
				return LastPage.GridRows;
			else
				return LastPage.GridRows;
		}
		float AccumulateRowHeight(int from, int to) {
			AccumulatedOffset accumulatedOffset = new AccumulatedOffset(Control.InnerControl.DocumentModel.ActiveSheet);
			accumulatedOffset.AddRows(from, to);
			return accumulatedOffset.ToLayout();
		}
		float CalculateSelectionHeight(SpreadsheetHitTestResult hitTestResult) {
			PageGrid gridRows = GetRowsGrid();
			int nearRow = gridRows.LookupNearItem(hitTestResult.CellPosition.Row);
			if (OriginalRange.ContainsCell(hitTestResult.CellPosition.Column, hitTestResult.CellPosition.Row)) {
				float height = AccumulateRowHeight(hitTestResult.CellPosition.Row, OriginalRange.BottomRight.Row);
				if (nearRow != -1)
					height -= hitTestResult.PhysicalPoint.Y - gridRows[nearRow].Near;
				return height;
			}
			else {
				int left = 0;
				int right = 0;
				int delta = 0;
				if (OriginalRange.BottomRight.Row < hitTestResult.CellPosition.Row) {
					left = OriginalRange.BottomRight.Row + 1;
					right = hitTestResult.CellPosition.Row;
					if (nearRow != -1)
						delta = hitTestResult.PhysicalPoint.Y - gridRows[nearRow].Near;
				}
				else if (OriginalRange.TopLeft.Row > hitTestResult.CellPosition.Row) {
					left = hitTestResult.CellPosition.Row + 1;
					right = OriginalRange.TopLeft.Row;
					if (nearRow != -1)
						delta = gridRows[nearRow].Far - hitTestResult.PhysicalPoint.Y;
				}
				float height = AccumulateRowHeight(left, right - 1);
				if (delta >= gridRows[nearRow].Extent / 2)
					height += delta;
				return height;
			}
		}
		bool IsHorizontalSelection(SpreadsheetHitTestResult hitTestResult) {
			return CalculateSelectionWidth(hitTestResult) > CalculateSelectionHeight(hitTestResult);
		}
		protected internal override Rectangle CalculateCellBounds(SpreadsheetHitTestResult hitTestResult) {
			Rectangle target = CalculateTargetRange(hitTestResult);
			return hitTestResult.Page.CalculateRangeBounds(target.Left, target.Top, target.Right, target.Bottom);
		}
		#region Scrolling
		protected override void CalculateAutoScrollParameters(Point p, Rectangle bounds) {
			bool allowHorizontalScroll = false;
			bool allowVerticalScroll = false;
			if (CurrentHitTestResult != null) {
				if (LastPage != null) {
					int brColumn = Math.Min(OriginalRange.BottomRight.Column + 2, Control.InnerControl.DocumentModel.ActiveSheet.MaxColumnCount);
					int brRow = Math.Min(OriginalRange.BottomRight.Row + 2, Control.InnerControl.DocumentModel.ActiveSheet.MaxRowCount);
					int tlColumn = Math.Max(OriginalRange.TopLeft.Column - 2, 0);
					int tlRow = Math.Max(OriginalRange.TopLeft.Row - 2, 0);
					bool isHorizontalSelection = IsHorizontalSelection(CurrentHitTestResult);
					if (isHorizontalSelection) {
						allowHorizontalScroll = true;
						allowVerticalScroll = false;
					}
					else {
						allowHorizontalScroll = false;
						allowVerticalScroll = true;
					}
					if (tlColumn <= LastPage.GridColumns.First.ModelIndex || brColumn >= LastPage.GridColumns.Last.ModelIndex ||
					   tlRow <= LastPage.GridRows.First.ModelIndex || brRow >= LastPage.GridRows.Last.ModelIndex) {
						if ((p.Y < bounds.Top && tlRow < LastPage.GridRows.First.ModelIndex) ||
						   (p.Y > bounds.Bottom && brRow > LastPage.GridRows.Last.ModelIndex))
							allowVerticalScroll = true;
						if ((p.X < bounds.Left && tlColumn < LastPage.GridColumns.First.ModelIndex) ||
						   (p.X > bounds.Right && brColumn > LastPage.GridColumns.Last.ModelIndex))
							allowHorizontalScroll = true;
					}
				}
				else {
					allowHorizontalScroll = true;
					allowVerticalScroll = true;
				}
			}
			else {
				allowHorizontalScroll = true;
				allowVerticalScroll = true;
			}
			if (allowHorizontalScroll)
				CalculateHorizontalAutoScrollParameters(p, bounds);
			if (allowVerticalScroll)
				CalculateVerticalAutoScrollParameters(p, bounds);
		}
		#endregion
	}
	#endregion
	#region DragContentStandardMouseHandlerStateBase (abstract class)
	public abstract class DragContentStandardMouseHandlerStateBase : CancellableDragMouseHandlerStateBase  {
		protected DragContentStandardMouseHandlerStateBase(SpreadsheetMouseHandler mouseHandler)
			: base(mouseHandler, new SpreadsheetHitTestResult(mouseHandler.Control.InnerControl.DesignDocumentLayout)) {
		}
		protected internal override bool HandleDragDropManually { get { return false; } }
		public override void OnDragOver(DragEventArgs e) {
			e.Effect = ContinueDrag(new Point(e.X, e.Y), e.AllowedEffect, e.Data);
		}
		public override void OnDragDrop(DragEventArgs e) {
			base.OnDragDrop(e);
			OnDragDropCore(e);
			MouseHandler.SwitchToDefaultState();
		}
		protected virtual void OnDragDropCore(DragEventArgs e) {
			if (Control.InnerControl.Options.InnerBehavior.DropAllowed)
				CommitDrag(new Point(e.X, e.Y), GetDataObject(e));
		}
		public override void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			bool isCommandDisabled = !Control.InnerControl.Options.InnerBehavior.DragAllowed;
			if (e.EscapePressed || isCommandDisabled) {
				e.Action = DragAction.Cancel;
				MouseHandler.SwitchToDefaultState();
			}
		}
		protected internal override void SetMouseCursor(SpreadsheetCursor cursor) {
		}
		protected internal override SpreadsheetCursor CalculateMouseCursor() {
			return SpreadsheetCursors.GetCursor(CalculateDragDropEffects());
		}
		protected internal override void ShowVisualFeedback() {
		}
		protected internal override void HideVisualFeedback() {
		}
		protected internal abstract DragDropEffects CalculateDragDropEffects();
		protected internal abstract IDataObject GetDataObject(DragEventArgs e);
	}
	#endregion
	#region DragExternalContentMouseHandlerState
	public class DragExternalContentMouseHandlerState : DragContentStandardMouseHandlerStateBase {
		public DragExternalContentMouseHandlerState(SpreadsheetMouseHandler mouseHandler)
			: base(mouseHandler) {
		}
		protected internal override DocumentLayoutDetailsLevel HitTestDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		public override void OnDragLeave() {
			MouseHandler.SwitchToDefaultState();
		}
		protected internal override DragDropEffects CalculateDragDropEffects() {
			return DragDropEffects.Copy;
		}
		protected internal override IDataObject GetDataObject(DragEventArgs e) {
			return e.Data;
		}
		protected internal override DataObject CreateDataObject() {
			return null;
		}
		protected internal override DragDropEffects ContinueDrag(Point point, DragDropEffects allowedEffects, IDataObject dataObject) {
			CellPosition cell = CellPosition.InvalidValue;
			DragDropEffects result = DragDropEffects.None;
			SpreadsheetHitTestResult hitTestResult = Control.InnerControl.ActiveView.CalculatePageHitTest(point);
			if (hitTestResult != null && hitTestResult.IsValid(DocumentLayoutDetailsLevel.Cell))
				cell = hitTestResult.CellPosition;
			PasteExternalContentCommand command = new PasteExternalContentCommand(Control, new DataObjectPasteSource(dataObject), cell);
			if (command.CanExecute())
				result = CalculateDragDropEffects();
			return result;
		}
		protected internal override bool CommitDrag(Point point, IDataObject dataObject) {
			CellPosition cell = CellPosition.InvalidValue;
			SpreadsheetHitTestResult hitTestResult = Control.InnerControl.ActiveView.CalculatePageHitTest(point);
			if (hitTestResult != null && hitTestResult.IsValid(DocumentLayoutDetailsLevel.Cell))
				cell = hitTestResult.CellPosition;
			PasteExternalContentCommand command = new PasteExternalContentCommand(Control, new DataObjectPasteSource(dataObject), cell);
			command.Execute();
			return true;
		}
	}
	#endregion
}
