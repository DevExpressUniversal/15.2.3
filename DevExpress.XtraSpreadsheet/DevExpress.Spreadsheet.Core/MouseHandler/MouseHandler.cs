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
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Compatibility.System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Compatibility.System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Windows.Forms;
namespace DevExpress.XtraSpreadsheet.Mouse {
	#region SpreadsheetMouseHandler
	public class SpreadsheetMouseHandler : MouseHandler {
		#region Fields
		readonly ISpreadsheetControl control;
		readonly SpreadsheetMouseHandlerStrategy platformStrategy;
		object activeObject;
		const int showCommentInterval = 1000;
		Timer commentTimer;
		CommentBox hoveredCommentBox;
		#endregion
		public SpreadsheetMouseHandler(ISpreadsheetControl control) {
			Guard.ArgumentNotNull(control, "control");
			this.control = control;
			this.platformStrategy = CreatePlatformStrategy();
			InitializeCommentTimer();
			this.LastPositionAtMove = CellPosition.InvalidValue;
		}
		#region Properties
		protected override bool SupportsTripleClick { get { return true; } }
		protected internal bool IsClicked { get { return ClickCount == 1; } }
		protected internal CommentBox HoveredCommentBox { get { return hoveredCommentBox; } set { hoveredCommentBox = value; } }
		protected internal CellPosition LastPositionAtMove { get; set; }
		public ISpreadsheetControl Control { get { return control; } }
		public SpreadsheetMouseHandlerStrategy PlatformStrategy { get { return platformStrategy; } }
		public object ActiveObject { get { return activeObject; } }
		#endregion
		protected internal void SetActiveObject(object modelObject) {
			this.activeObject = modelObject;
		}
		protected internal virtual SpreadsheetMouseHandlerStrategyFactory GetPlatformStrategyFactory() {
			return control.CreateMouseHandlerStrategyFactory();
		}
		protected virtual SpreadsheetMouseHandlerStrategy CreatePlatformStrategy() {
			SpreadsheetMouseHandlerStrategyFactory factory = GetPlatformStrategyFactory();
			return factory.CreateMouseHandlerStrategy(this);
		}
		protected internal virtual MouseEventArgs CreateFakeMouseMoveEventArgs() {
			return platformStrategy.CreateFakeMouseMoveEventArgs();
		}
		public virtual SpreadsheetHitTestResult CalculateHitTest(Point point) {
			return Control.InnerControl.ActiveView.CalculateHitTest(point);
		}
		protected override void CalculateAndSaveHitInfo(MouseEventArgs e) {
		}
		protected override void StartOfficeScroller(Point clientPoint) {
			platformStrategy.StartOfficeScroller(clientPoint);
		}
		protected override IOfficeScroller CreateOfficeScroller() {
			if (platformStrategy != null)
				return platformStrategy.CreateOfficeScroller();
			else
				return null;
		}
		protected override MouseEventArgs ConvertMouseEventArgs(MouseEventArgs screenMouseEventArgs) {
			return platformStrategy.ConvertMouseEventArgs(screenMouseEventArgs);
		}
		protected override void HandleMouseWheel(MouseEventArgs e) {
			SpreadsheetMouseHandlerState state = State as SpreadsheetMouseHandlerState;
			if (state == null || !state.SuppressDefaultMouseWheelProcessing) {
				if (KeyboardHandler.IsControlPressed)
					PerformWheelZoom(e);
				else
					PerformWheelScroll(e);
			}
			State.OnMouseWheel(e);
		}
		protected override void HandleClickTimerTick() {
			StopClickTimer();
			if (Suspended)
				return;
			State.OnLongMouseDown();
		}
		#region SwitchToDefaultState
		public override void SwitchToDefaultState() {
			MouseHandlerState newState = CreateDefaultState();
			SwitchStateCore(newState, Point.Empty);
		}
		#endregion
		protected internal virtual MouseHandlerState CreateDefaultState() {
			return new DefaultMouseHandlerState(this);
		}
		protected internal virtual SpreadsheetRectangularObjectResizeMouseHandlerState CreateRectangularObjectResizeState(DrawingObjectHotZone hotZone, SpreadsheetHitTestResult result) {
			return new SpreadsheetRectangularObjectResizeMouseHandlerState(this, hotZone, result);
		}
		protected internal virtual SpreadsheetRectangularObjectRotateMouseHandlerState CreateRectangularObjectRotateState(DrawingObjectRotationHotZone hotZone, SpreadsheetHitTestResult result) {
			return new SpreadsheetRectangularObjectRotateMouseHandlerState(this, hotZone, result);
		}
		protected internal virtual MouseHandlerState CreateDragRangeState(SpreadsheetHitTestResult result) {
			return new DragRangeManuallyMouseHandlerState(this, result);
		}
		protected internal virtual MouseHandlerState CreateResizeRangeState(SpreadsheetHitTestResult result) {
			return new ResizeRangeManuallyMouseHandlerState(this); 
		}
		protected internal virtual MouseHandlerState CreateMailMergreDragRangeState(SpreadsheetHitTestResult result, string mailMergeDefinedName) {
			return new MailMergeMoveRangeMouseState(this, result, mailMergeDefinedName);
		}
		protected internal virtual MouseHandlerState CreateMailMergreResizeRangeState(SpreadsheetHitTestResult result, bool top, bool left, string mailMergeDefinedName) {
			return new MailMergeResizeRangeMouseState(this, result, top, left, mailMergeDefinedName);
		}
		protected internal virtual MouseHandlerState CreateCommentDragState(SpreadsheetHitTestResult result) {
			return new CommentDragMouseHandlerState(this, result);
		}
		protected internal virtual MouseHandlerState CreateCommentResizeState(SpreadsheetHitTestResult result, CommentResizeHotZoneBase hotZone) {
			return new CommentResizeMouseHandlerState(this, result, hotZone);
		}
		protected override AutoScroller CreateAutoScroller() {
			return new SpreadsheetAutoScroller(this);
		}
		protected internal virtual void PerformWheelScroll(MouseEventArgs e) {
			OfficeMouseWheelEventArgs ea = e as OfficeMouseWheelEventArgs;
			bool isHorizontal = (ea != null) && ea.IsHorizontal;
			int delta = e.Delta;
			if (isHorizontal)
				ScrollHorizontal(delta);
			else
				ScrollVertical(delta);
		}
		void ScrollHorizontal(int delta) {
			if (delta > 0)
				SmallScrollLeft(delta);
			else
				SmallScrollRight(delta);
		}
		void ScrollVertical(int delta) {
			if (delta > 0)
				SmallScrollUp(delta);
			else
				SmallScrollDown(delta);
		}
		protected internal virtual void PerformWheelZoom(MouseEventArgs e) {
			Command command = CreateZoomCommand(e);
			command.Execute();
		}
		protected internal virtual Command CreateZoomCommand(MouseEventArgs e) {
			if (e.Delta > 0)
				return new ViewZoomInCommand(Control);
			else
				return new ViewZoomOutCommand(Control);
		}
		protected internal virtual void SmallScrollDown(int wheelDelta) {
			int delta = GetScrollDelta(wheelDelta);
			SmallScrollCore(Math.Max(1, delta));
		}
		protected internal virtual void SmallScrollUp(int wheelDelta) {
			int delta = -GetScrollDelta(wheelDelta);
			SmallScrollCore(Math.Min(-1, delta));
		}
		protected internal int GetScrollDelta(int wheelDelta) {
#if !SL && !DXPORTABLE
			return SystemInformation.MouseWheelScrollLines * (Math.Max(1, Math.Abs(wheelDelta) / Math.Max(1, SystemInformation.MouseWheelScrollDelta)));
#else
			return -3;
#endif
		}
		protected internal virtual void SmallScrollCore(int direction) {
			Control.InnerControl.ActiveView.ScrollLineUpDown(direction);
		}
		protected internal virtual void SmallScrollRight(int wheelDelta) {
			int delta = -GetScrollDelta(wheelDelta);
			SmallHorizontalScrollCore(Math.Max(-1, delta));
		}
		protected internal virtual void SmallScrollLeft(int wheelDelta) {
			int delta = GetScrollDelta(wheelDelta);
			SmallHorizontalScrollCore(Math.Max(1, delta));
		}
		void SmallHorizontalScrollCore(int direction) {
			Control.InnerControl.ActiveView.ScrollLineLeftRight(direction);
		}
		protected internal virtual DragEventArgs ConvertDragEventArgs(DragEventArgs screenDragEventArgs) {
			return platformStrategy.ConvertDragEventArgs(screenDragEventArgs);
		}
		protected internal virtual void ApplyDragEventArgs(DragEventArgs modifiedArgs, DragEventArgs originalArgs) {
			originalArgs.Effect = modifiedArgs.Effect;
		}
		Point lastDragPoint;
		public virtual void OnDragEnter(DragEventArgs e) {
			lastDragPoint = Point.Empty;
			DragEventArgs args = ConvertDragEventArgs(e);
			State.OnDragEnter(args);
			ApplyDragEventArgs(args, e);
		}
		public virtual void OnDragOver(DragEventArgs e) {
			Point pt = new Point(e.X, e.Y);
			if (pt != lastDragPoint) {
				DragEventArgs args = ConvertDragEventArgs(e);
				State.OnDragOver(args);
				ApplyDragEventArgs(args, e);
				AutoScrollerOnDragOver(pt);
				lastDragPoint = pt;
			}
		}
		protected internal virtual void AutoScrollerOnDragOver(Point pt) {
			platformStrategy.AutoScrollerOnDragOver(pt);
		}
		public virtual void OnDragDrop(DragEventArgs e) {
			lastDragPoint = Point.Empty;
			DragEventArgs args = ConvertDragEventArgs(e);
			State.OnDragDrop(args);
			ApplyDragEventArgs(args, e);
		}
		public virtual void OnDragLeave(EventArgs e) {
			lastDragPoint = Point.Empty;
			State.OnDragLeave();
		}
		public virtual void OnGiveFeedback(GiveFeedbackEventArgs e) {
			State.OnGiveFeedback(e);
		}
		public virtual void OnQueryContinueDrag(QueryContinueDragEventArgs e) {
			State.OnQueryContinueDrag(e);
		}
		public override void OnMouseUp(MouseEventArgs e) {
			platformStrategy.OnMouseUp(e);
			base.OnMouseUp(e);
		}
#region Comment hovering
		void InitializeCommentTimer() {
			if (!Control.InnerControl.AllowShowingForms) 
				return;
			this.commentTimer = new Timer();
			this.commentTimer.Interval = showCommentInterval;
			this.commentTimer.Tick += OnCommentTimerTick;
		}
		internal void StartCommentTimer() {
			if (commentTimer != null)
				this.commentTimer.Start();
		}
		internal void StopCommentTimer() {
			if (commentTimer != null)
				this.commentTimer.Stop();
		}
		internal void ResetCommentTimer() {
			StopCommentTimer();
			StartCommentTimer();
		}
		void OnCommentTimerTick(object sender, EventArgs e) {
			StopCommentTimer();
			if (LastPositionAtMove.EqualsPosition(CellPosition.InvalidValue) || IsInplaceEditorActive())
				return;
			CommentBox commentBox = FindCommentBox();
			if (commentBox != null && commentBox.IsHidden) {
				commentBox.IsHovered = true;
				hoveredCommentBox = commentBox;
				Control.InnerControl.Owner.Redraw();
			}
		}
		CommentBox FindCommentBox() {
			DocumentLayout layout = Control.InnerControl.InnerDocumentLayout;
			if (layout == null)
				return null;
			foreach (Page page in layout.Pages) {
				foreach (CommentBox box in page.CommentBoxes) {
					if (box.Reference.EqualsPosition(LastPositionAtMove))
						return box;
				}
			}
			return null;
		}
		bool IsInplaceEditorActive() {
			InnerSpreadsheetControl innerControl = Control.InnerControl;
			return  innerControl.IsCommentInplaceEditorActive || innerControl.IsDataValidationInplaceEditorActive;
		}
		internal void ProcessComment(SpreadsheetHitTestResult hitTestResult) {
			if (hitTestResult == null || hitTestResult.CellPosition.EqualsPosition(CellPosition.InvalidValue)) {
				HideHoveredComment(true);
				return;
			}
			SheetViewSelection selection = Control.InnerControl.DocumentModel.ActiveSheet.Selection;
			CellPosition currentPosition = selection.GetActualCellRange(hitTestResult.CellPosition).TopLeft;
			if (!LastPositionAtMove.EqualsPosition(currentPosition)) {
				TryHideComment();
				LastPositionAtMove = currentPosition;
				ResetCommentTimer();
			}
		}
		internal void HideHoveredComment(bool invalidateLastPosition) {
			StopCommentTimer();
			TryHideComment();
			if (invalidateLastPosition)
				LastPositionAtMove = CellPosition.InvalidValue;
		}
		void TryHideComment() {
			if (hoveredCommentBox == null)
				return;
			hoveredCommentBox.IsHovered = false;
			hoveredCommentBox = null;
			Control.InnerControl.Owner.Redraw();
		}
#endregion
		protected override void Dispose(bool disposing) {
			try {
				if (disposing) {
					if (commentTimer != null) {
						commentTimer.Stop();
						commentTimer.Tick -= OnClickTimerTick;
						commentTimer.Dispose();
						commentTimer = null;
					}
					hoveredCommentBox = null;
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
	}
#endregion
}
