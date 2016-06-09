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
using System.Collections;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Drawing;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Dragging;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid.Handler;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Internal;
namespace DevExpress.XtraGrid.Dragging {
	public class DragStartArgs {
		object dragObject;
		Bitmap bmp;
		Point startPoint;
		DragDropEffects allowedEffects;
		public DragStartArgs(object dragObject, Bitmap bmp, Point startPoint, DragDropEffects allowedEffects) {
			this.dragObject = dragObject;
			this.bmp = bmp;
			this.startPoint = startPoint;
			this.allowedEffects = allowedEffects;
			this.CaptureMouse = true;
			this.AllowCancelOnDragging = true;
		}
		public bool CaptureMouse { get; set; }
		public bool AllowCancelOnDragging { get; set; }
		public object DragObject { get { return dragObject; } }
		public Bitmap Bmp { get { return bmp; } }
		public Point StartPoint { get { return startPoint; } set { startPoint = value; } }
		public DragDropEffects AllowedEffects { get { return allowedEffects; } }
	}
	public class GridDragStartArgs : DragStartArgs {
		GridHitInfo startHitInfo;
		public GridDragStartArgs(GridHitInfo startHitInfo, object dragObject, Bitmap bmp, Point startPoint, DragDropEffects allowedEffects) 
		   : base(dragObject, bmp, startPoint, allowedEffects) {
			this.startHitInfo = startHitInfo;
		}
		public GridHitInfo StartHitInfo { get { return startHitInfo; } }
	}
	public class DragController {
		GridControl control;
		DragManager activeDragManager, prevDragManager;
		static Cursor dragRemoveCursor;
		public static Cursor DragRemoveCursor {
			get {
				if(dragRemoveCursor == null) dragRemoveCursor = ResourceImageHelper.CreateCursorFromResources("DevExpress.XtraGrid.Images.DragRemove.cur", typeof(DragManager).Assembly);
				return dragRemoveCursor;
			}
		}
		public DragController(GridControl control) {
			this.control = control;
			this.activeDragManager = null;
			this.prevDragManager = null;
		}
		protected DragManager PrevDragManager {
			get { return prevDragManager; }
		}
		public GridControl Control { get { return control; } }
		public DragManager ActiveDragManager {
			get { return activeDragManager; }
		}
		public virtual bool IsDragging { get { return ActiveDragManager != null; } }
		public virtual void DoDragging(MouseEventArgs e) {
			if(ActiveDragManager == null) return;
			if(e.Button != MouseButtons.Left) {
				if(ActiveDragManager.StartArgs.AllowCancelOnDragging)
					CancelDrag();
				return;
			} 
			Point screenPoint = Control.PointToScreen(new Point(e.X, e.Y));
			DragManager man = ActiveDragManager.GetDragManager(screenPoint);
			if(man != PrevDragManager && PrevDragManager != null) {
				ActiveDragManager.Assign(PrevDragManager);
				PrevDragManager.PauseDrag();
				prevDragManager = null;
			}
			if(PrevDragManager != null) {
				PrevDragManager.DoDragging(screenPoint);
				return;
			}
			if(man == ActiveDragManager) {
				ActiveDragManager.DoDragging(screenPoint);
				return;
			}
			if(PrevDragManager == null && man != ActiveDragManager) {
				prevDragManager = man;
				man.Assign(ActiveDragManager);
				man.DragObject = man.FindLocalObject(ActiveDragManager.DragObject);
				man.DoDragging(man.StartArgs.StartPoint);
			}
		}
		public virtual void CancelDrag() {
			if(ActiveDragManager == null) return;
			DragManager prev = PrevDragManager;
			DragManager man = ActiveDragManager;
			this.prevDragManager = null;
			this.activeDragManager = null;
			if(prev != null) {
				prev.CancelDrag();
				StopDrag(prev);
			}
			man.CancelDrag();
			StopDrag(man);
			Control.Invalidate();
		}
		public virtual void EndDrag(MouseEventArgs e) {
			if(ActiveDragManager == null) return;
			DragManager prev = PrevDragManager;
			DragManager man = ActiveDragManager;
			this.prevDragManager = null;
			this.activeDragManager = null;
			try {
				if(prev != null) {
					prev.EndDrag();
					man.CancelDrag();
					StopDrag(prev);
				}
				else {
					man.EndDrag();
				}
			}
			finally {
				StopDrag(man);
			}
			Control.Invalidate();
		}
		public virtual void StartDragging(DragManager manager, DragStartArgs args) {
			if(args.CaptureMouse) {
				Control.MouseCaptureOwner = null;
				Control.Capture = true;
			}
			if(ActiveDragManager != null) CancelDrag();
			this.activeDragManager = manager;
			if(ActiveDragManager != null)
				ActiveDragManager.StartDragging(args);
		}
		public virtual object DragObject { 
			get {
				if(ActiveDragManager == null) return null;
				return ActiveDragManager.DragObject;
			}
		}
		protected virtual void StopDrag(DragManager manager) {
			manager.View.SetDefaultState();
			Control.Capture = false;
		}
	}
	public class DragManager : IDisposable {
		BaseView view;
		object dragObject;
		static DragMaster dragMaster;
		DragStartArgs startArgs;
		public DragManager(BaseView view) {
			this.startArgs = null;
			this.dragObject = null;
			this.view = view;
		}
		public virtual void Dispose() {
			CancelDrag();
		}
		public virtual object FindLocalObject(object dragObject) { return dragObject; }
		public virtual DragManager GetDragManager(Point screenPoint) { return this; } 
		public virtual void Assign(DragManager manager) {
			this.startArgs = manager.StartArgs;
			this.StartArgs.StartPoint = DragManager.DragMaster.LastPosition;
		}
		public static DragMaster DragMaster { 
			get { 
				if(dragMaster == null) dragMaster = new DragMaster();
				return dragMaster; 
			} 
		}
		public BaseView View { get { return view; } }
		public BaseViewInfo ViewInfo { get { return View.ViewInfo; } }
		public object DragObject { get { return dragObject; } set { dragObject = value; } }
		public virtual void StartDragging(DragStartArgs args) {
			this.startArgs = args;
		}
		public virtual void DoDragging(Point screenPoint) {
		}
		public virtual void CancelDrag() {
			StopDragging();
		}
		public virtual void EndDrag() {
		}
		public virtual void PauseDrag() {
			StopDragging(false);
		}
		protected void StopDragging() { StopDragging(true); }
		protected virtual void StopDragging(bool hideDrag) {
			this.startArgs = null;
		}
		public virtual DragStartArgs StartArgs { get { return startArgs; } }
	}
	public class GridDragManager : DragManager {
		public const int HideElementPosition = -100, CustomizationFormPosition = -101;
		const int ScrollStartDelay = 500, ScrollDelay = 50, ScrollHorzDelta = 7;
		public enum DropPosition { Left, Up, Right, Down, Current, None }
		protected DragMaster fDragMaster;
		PositionInfo lastPositionInfo;
		bool _dropTargetHighlighted = false;
		Timer autoScrollTimer;
		DropPosition scrollDirection;
		GridHitInfo _dragStartHitInfo;
		public GridDragManager(GridView view) : base(view) {
			this._dragStartHitInfo = null;
			this.scrollDirection = DropPosition.None;
			this.lastPositionInfo = new PositionInfo();
			this.autoScrollTimer = new Timer();
			this.autoScrollTimer.Tick += new EventHandler(OnScrollTimerTick);
		}
		protected virtual bool UseArrows {
			get { return DragArrowsHelper.IsAllow && View != null && View.GridControl != null; }
		}
		public override DragManager GetDragManager(Point screenPoint) { 
			Point pt = View.GridControl.PointToClient(screenPoint);
			GridView gv = View.GridControl.ViewByPoint(pt) as GridView;
			if(View.IsLevelDefault && View.ViewRect.IsEmpty) {
				if(!(DragObject is GridColumn)) return this;
				if(gv != null && gv.SourceView == View) {
					return (gv.Handler as GridHandler).DragManager;
				}
				return this;
			}
			if(IsInCustomizationForm(screenPoint)) return this;
			if(gv != null && gv != View && View.SourceView != null && View.SourceView.IsLevelDefault && View.SourceView.SynchronizeClones) {
				if(!gv.OptionsView.ShowChildrenInGroupPanel) return this;
				if(!gv.HasAsChild(View)) return this;
				return (gv.Handler as GridHandler).DragManager;
			}
			return this; 
		}
		public override void Assign(DragManager manager) {
			base.Assign(manager);
			GridDragManager gm = manager as GridDragManager;
			if(gm == null) return;
			_dragStartHitInfo = gm._dragStartHitInfo;
		}
		protected bool DropTargetHighlighted { get { return _dropTargetHighlighted; } }
		protected virtual ColumnPositionInfo CreateColumnPositionInfo() {
			return new ColumnPositionInfo();
		}
		public GridHitInfo DragStartHitInfo { get { return _dragStartHitInfo; } }
		protected virtual void OnScrollTimerTick(object sender, EventArgs e) {
			if(ScrollDirection == DropPosition.None) return;
			Point pt = View.GridControl.PointToClient(Cursor.Position);
			if(ViewRectContainsPoint(pt)) {
				ScrollDirection = DropPosition.None;
				DoDragging(Cursor.Position);
				return;
			}
			if(ScrollDirection == DropPosition.Left || ScrollDirection == DropPosition.Right) {
				if(View.OptionsView.ColumnAutoWidth) return;
				int minDelta = Math.Max(ScrollHorzDelta, Math.Abs((ViewInfo.ViewRects.ColumnTotalWidth - ViewInfo.ViewRects.ColumnPanel.Width) / 20));
				int delta = ScrollDirection == DropPosition.Left ? -minDelta : minDelta;
				View.LeftCoord += delta;
			} 
			AutoScrollTimer.Interval = ScrollDelay;
		}
		protected Timer AutoScrollTimer { get { return autoScrollTimer; } }
		protected DropPosition ScrollDirection { 
			get { return scrollDirection; } 
			set { 
				if(ScrollDirection == value) return;
				DropPosition prev = ScrollDirection;
				scrollDirection = value;
				AutoScrollTimer.Stop();
				if(ScrollDirection != DropPosition.None) {
					HideDropTargetHighlighting();
					ClearLastPosition();
					AutoScrollTimer.Interval = ScrollStartDelay;
					AutoScrollTimer.Start();
				}
			}
		}
		protected new GridDragStartArgs StartArgs { get { return base.StartArgs as GridDragStartArgs; } }
		public virtual PositionInfo LastPosition { get { return lastPositionInfo; } }
		public new GridView View { get { return base.View as GridView; } }
		public new GridViewInfo ViewInfo { get { return base.ViewInfo as GridViewInfo; } }
		public override void StartDragging(DragStartArgs args) {
			ClearLastPosition();
			base.StartDragging(args);
			this._dragStartHitInfo = StartArgs.StartHitInfo;
			this.DragObject = StartArgs.DragObject;
			DragMaster.StartDrag(StartArgs.Bmp, StartArgs.StartPoint, StartArgs.AllowedEffects);
		}
		protected virtual bool ViewRectContainsPoint(Point pt) {
			Rectangle r = ViewInfo.ViewRects.Client;
			if(IsLeftScrollArea(pt) || IsRightScrollArea(pt)) return false;
			r.Inflate(-2, 0);
			return r.Contains(pt);
		}
		bool IsLeftScrollArea(Point pt) {
			if(View.IsRightToLeft) {
				if(pt.X > ViewInfo.ViewRects.Client.Right - 6 - Math.Max(ViewInfo.ViewRects.IndicatorWidth - 5, 0)) {
					return ViewInfo.ViewRects.ColumnTotalWidth > ViewInfo.ViewRects.ColumnPanelWidth;
				}
			}
			else {
			if(pt.X < ViewInfo.ViewRects.ColumnPanel.Left + 4 + Math.Max(ViewInfo.ViewRects.IndicatorWidth - 5, 0)) return View.LeftCoord > 0;
			}
			return false;
		}
		bool IsRightScrollArea(Point pt) {
			if(View.IsRightToLeft) {
				if(pt.X < ViewInfo.ViewRects.ColumnPanel.Left + 4) return true;
			}
			else {
			if(pt.X > ViewInfo.ViewRects.Client.Right - 6) {
				return ViewInfo.ViewRects.ColumnTotalWidth > ViewInfo.ViewRects.ColumnPanelWidth;
			}
			}
			return false;
		}
		public bool IsInCustomizationForm(Point screenPoint) {
			if(View.CustomizationForm == null || !View.CustomizationForm.IsHandleCreated) return false;
			return View.CustomizationForm.RectangleToScreen(new Rectangle(Point.Empty, View.CustomizationForm.Bounds.Size)).Contains(screenPoint);
		}
		public override void DoDragging(Point screenPoint) {
			Point pt = View.GridControl.PointToClient(screenPoint);
			if(IsInCustomizationForm(screenPoint)) 
				ScrollDirection = DropPosition.None;
			else {
				if(!ViewRectContainsPoint(pt)) {
					if(IsLeftScrollArea(pt)) {
						ScrollDirection = DropPosition.Left;
					} else {
						if(IsRightScrollArea(pt)) {
							ScrollDirection = DropPosition.Right;
						}
					}
				} else {
					ScrollDirection = DropPosition.None;
				}
			}
			if(ScrollDirection == DropPosition.None) {
				PositionInfo pos = Calc(ViewInfo.CalcHitInfo(pt));
				View.RaiseDragObjectOver(new DragObjectOverEventArgs(DragObject, pos));
				if(!pos.IsEquals(LastPosition)) {
					HideDropTargetHighlighting();
					this.lastPositionInfo = pos;
					ShowDropTargetHighlighting();
				}
			}
			DragDropEffects effect = LastPosition.Valid ? DragDropEffects.Copy : DragDropEffects.None;
			if(LastPosition.Valid && LastPosition.Index <= HideElementPosition) {
				effect = LastPosition.Index == HideElementPosition ? DragDropEffects.Link : DragDropEffects.Move;
			}
			DragMaster.DoDrag(screenPoint, effect);
		}
		public virtual void ShowDropTargetHighlighting() {
			if(DropTargetHighlighted && !UseArrows) return;
			ShowTargetCore(LastPosition, false);
		}
		public virtual bool HideDropTargetHighlighting() {
			if(!DropTargetHighlighted && !UseArrows) return false;
			ShowTargetCore(LastPosition, true);
			return true;
		}
		protected virtual void ShowTargetCore(PositionInfo info, bool hide) {
			if(UseArrows) {
				ShowTargetCoreArrows(info, hide);
				return;
			}
			if(info.Bounds.IsEmpty || !info.Valid) return;
			Rectangle r = info.Bounds;
			if(r.Right > ViewInfo.ViewRects.Client.Right)
				r.Width = ViewInfo.ViewRects.Client.Right - r.X;
			if(r.Left < ViewInfo.ViewRects.ColumnPanelLeft) {
				r.X = ViewInfo.ViewRects.ColumnPanelLeft;
				r.Width = info.Bounds.Right - r.Left;
			}
			if(r.Width < 1) return;
			if(NativeVista.IsVista && !NativeVista.IsCompositionEnabled()) {
				r.Inflate(-1, -1);
				ViewInfo.DragFrameRect = hide ? Rectangle.Empty : r;
				ViewInfo.View.InvalidateRect(ViewInfo.GetTargetDragRect(r.Height));
			}
			else {
				SplitterLineHelper.Default.DrawReversibleFrame(View.GridControl.Handle, r);
			}
			_dropTargetHighlighted = !hide;
		}
		DragArrowsHelper DragArrows {
			get {
				if(View.GridControl == null) return null;
				return View.GridControl.CreateArrowsHelper();
			}
		}
		Point PointToScreen(Point point) {
			return View.GridControl.PointToScreen(point);
		}
		void ShowTargetCoreArrows(PositionInfo info, bool hide) {
			if(hide || info.Bounds.IsEmpty || !info.Valid) {
				DragArrows.Hide();
				return;
			}
			Rectangle r = info.Bounds;
			if(r.X < ViewInfo.ViewRects.ColumnPanelLeft)
				r.X = ViewInfo.ViewRects.ColumnPanelLeft;
			if(r.X > ViewInfo.ViewRects.ColumnPanelRight) 
				r.X = ViewInfo.ViewRects.ColumnPanelRight - 1;
			if(r.Right > ViewInfo.ViewRects.ColumnPanelRight)
				r.Width = ViewInfo.ViewRects.ColumnPanelRight - r.X;
			Point p1 = PointToScreen(r.Location), p2 = PointToScreen(new Point(r.X, r.Bottom));
			if(info.IsHorizontal) {
				r.Y += r.Height / 2;
				p1 = PointToScreen(r.Location);
				p2 = PointToScreen(new Point(r.Right, r.Y));
			}
			DragArrows.Show(!info.IsHorizontal, p1, p2);
		}
		protected override void StopDragging(bool hideDrag) {
			ScrollDirection = DropPosition.None;
			HideDropTargetHighlighting();
			this._dragStartHitInfo = null;
			this.DragObject = null;
			if(hideDrag) DragMaster.EndDrag();
			ClearLastPosition();
			base.StopDragging(hideDrag);
		}
		protected virtual void ClearLastPosition() {
			HideDropTargetHighlighting();
			this.lastPositionInfo = new PositionInfo();
		}
		public override void CancelDrag() {
			if(StartArgs == null && DragObject == null) return; 
			HideDropTargetHighlighting();
			LastPosition.Valid = false;
			View.RaiseDragObjectDrop(new DragObjectDropEventArgs(DragObject, LastPosition));
			base.CancelDrag();
		}
		public override void EndDrag() {
			try {
				if(DragObject is GridColumn)
					EndDragColumn(DragObject as GridColumn);
				View.RaiseDragObjectDrop(new DragObjectDropEventArgs(DragObject, LastPosition));
			}
			finally {
				StopDragging();
			}
		}
		public override object FindLocalObject(object dragObject) { 
			GridColumn col = dragObject as GridColumn;
			if(col == null || col.View == View) return base.FindLocalObject(dragObject);
			if(View.SourceView == col.View) {
				return View.Columns.FindLocalColumn(col);
			}
			if(col.View.SourceView != null) {
				GridView view = col.View.SourceView as GridView;
				if(view != null) return view.Columns.FindLocalColumn(col);
			}
			return base.FindLocalObject(dragObject);
		}
		protected virtual GridColumn FindLocalColumn(GridColumn col) {
			if(col.View == View) return col;
			return null;
		}
		protected virtual void EndDragOuterColumn(GridColumn col) {
			GridColumn lc = FindLocalColumn(col);
			if(lc == null) return;
			if(lc == col) return;
			EndDragColumn(lc);
		}
		protected void EndDragColumn(GridColumn col) {
			EndDragColumnCore(col);
			View.FireChangedColumns();
		}
		protected virtual void EndDragColumnCore(GridColumn col) {
			ColumnPositionInfo pInfo = LastPosition as ColumnPositionInfo;
			if(pInfo == null || col == null || !pInfo.Valid) return;
			col.View.BeginUpdate();
			try {
				if(pInfo.InGroupPanel) {
					if(col.GroupIndex > -1)
						col.GroupIndex = pInfo.Index;
					else {
						col.sortedBeforeGrouping = col.SortIndex >= 0;
						col.View.SortInfo.InsertGroup(pInfo.Index, col);
					}
					return;
				} else {
					if(DragStartHitInfo != null && DragStartHitInfo.InGroupColumn) {
						if(col.GroupIndex > -1) {
							col.UnGroup(pInfo.Index >= 0 ? pInfo.Index : -1);
						}
					}
					if(pInfo.Index <= HideElementPosition) {
						col.UnGroup();
					}
					if(pInfo.Index == -1) {
						col.UnGroup();
						col.Visible = true;
						return;
					}
				}
				if(pInfo.Index <= HideElementPosition) 
					col.Visible = false;
				else
					col.VisibleIndex = pInfo.Index;
			} finally {
				col.View.EndUpdate();
				col.View.RaiseColumnPositionChanged(col);
			}
		}
		protected virtual PositionInfo CalcInGroupPanel(GridHitInfo hit, GridColumn column) {
			ColumnPositionInfo res = CreateColumnPositionInfo();
			GroupPanelRow row = null;
			Point p = hit.HitPoint;
			if(column == null || column.View == null) return res;
			if(!column.View.CanGroupColumn(column)) return res;
			res.SetInGroupPanel(true);
			Rectangle r = Rectangle.Empty;
			r.Width = GridViewInfo.GroupLineDWidth;
			r.Height = ViewInfo.ColumnRowHeight;
			row = ViewInfo.GroupPanel.Rows.RowByView(column.View as GridView);
			if(row == null || row.ColumnsInfo.Count == 0) { 
				res.SetIndex(0);
				res.Valid = true;
				if(row == null || row.CaptionInfo == null) {
					r.X = ViewInfo.ViewRects.GroupPanel.Left + GridViewInfo.GroupLineDX;
					r.Y = ViewInfo.ViewRects.GroupPanel.Top + GridViewInfo.GroupLineDY;
					if(ViewInfo.IsRightToLeft) {
						r.X = ViewInfo.ViewRects.GroupPanel.Right - GridViewInfo.GroupLineDX;
					}
					if(ViewInfo.GroupPanel.Rows.Count > 0) r.Y += GridViewInfo.GroupLineDY;
				} else {
					r.Y = row.CaptionInfo.Bounds.Y;
					r.X = row.CaptionInfo.Bounds.Right + GridViewInfo.GroupLineDX;
					if(ViewInfo.IsRightToLeft) {
						r.X = row.CaptionInfo.Bounds.Left - GridViewInfo.GroupLineDX;
					}
				}
				res.Bounds = r;
				return res;
			}
			int newIndex = 0;
			bool left = false;
			GridColumnInfoArgs ci = null;
			if(row != null && row.ColumnsInfo.Count > 0) {
				for(int i = 0; i < row.ColumnsInfo.Count; i++) {
					ci = row.ColumnsInfo[i];
					if(ViewInfo.IsRightToLeft) {
						if(p.X > ci.Bounds.Left + ci.Bounds.Width / 2) {
							left = true;
							break;
						}
						else
							newIndex++;
					}
					else {
					if(p.X < ci.Bounds.Left + ci.Bounds.Width / 2) {
						left = true;
						break;
					}
					else
						newIndex++;
				}
				}
			} else {
				return res;
			}
			ci = row.ColumnsInfo[newIndex - (newIndex >= row.ColumnsInfo.Count ? 1 : 0)];
			r.Y = ci.Bounds.Top;
			if(!row.LineStyle) r.Y += ViewInfo.ColumnRowHeight / 2;
			if(left) {
				r.X = ci.Bounds.Left;
				r.Y = ci.Bounds.Top;
				if(ViewInfo.IsRightToLeft)
					r.X = ci.Bounds.Right;
			}
			else {
				r.X = ci.Bounds.Right + GridViewInfo.GroupLineDX;
				if(ViewInfo.IsRightToLeft) r.X = ci.Bounds.Left - GridViewInfo.GroupLineDX;
			}
			res.Bounds = r;
			res.SetIndex(newIndex);
			if(res.Index != column.GroupIndex) {
				if(column.GroupIndex > -1 && column.GroupIndex + 1 == res.Index)
					return res;
				res.Valid = true;
			}
			return res;
		}
		protected virtual bool IsPointOutsideControl(Point point) {
			if(ViewInfo.Bounds.Contains(point)) return false;
			return true;
		}
		static int counter = 0;
		protected virtual PositionInfo CalcColumnDrag(GridHitInfo hit, GridColumn column) {
			ColumnPositionInfo res = CreateColumnPositionInfo();
			counter++;
			if(!View.GridControl.Visible) return res;
			if(hit.InGroupPanel) {
				return CalcInGroupPanel(hit, column);
			}
			if(column == null) return res;
			if(column.View != View) return res;
			ColumnPositionInfo emptyRes = CreateColumnPositionInfo();
			if(column == null) return res;
			if(hit.HitTest == GridHitTest.CustomizationForm) {
				res.SetIndex(CustomizationFormPosition);
				res.Valid = column.OptionsColumn.AllowShowHide;
				return res;
			}
			if(hit.HitPoint.Y > ViewInfo.ViewRects.ColumnPanel.Bottom + 50 || IsPointOutsideControl(hit.HitPoint)) {
				res.SetIndex(HideElementPosition);
				res.Valid = column.OptionsColumn.AllowShowHide && View.OptionsCustomization.AllowQuickHideColumns;
				return res;
			}
			if(hit.HitPoint.Y < ViewInfo.ViewRects.ColumnPanel.Top) return res;
			if(column.GroupIndex >= 0) {
				emptyRes.SetIndex(-1);
				emptyRes.Valid = true;
			}
			Rectangle testBounds = ViewInfo.ViewRects.ColumnPanel;
			testBounds.Width = column.Width;
			if(View.VisibleColumns.Count > 0) {
				res = CalcInColumnPanel(hit, column, res, emptyRes, testBounds);
			} else {
				res.SetIndex(0);
				res.Bounds = testBounds;
				res.Valid = true;
			}
			return res;
		}
		ColumnPositionInfo CalcInColumnPanel(GridHitInfo hit, GridColumn column, ColumnPositionInfo res, ColumnPositionInfo emptyRes, Rectangle testBounds) {
			ColumnDropInfo drInfo = CalcColumnInfoCore(ViewInfo.ColumnsInfo, hit, testBounds);
			if(!drInfo.PositionInfo.Valid) return res;
			res = drInfo.PositionInfo;
			if(drInfo.PositionInfo.Index != 9999) {
				int index = drInfo.Column.VisibleIndex;
				if(index == -1) return emptyRes;
				res.SetIndex(drInfo.Pos == DropPosition.Left ? index : index + 1);
			}
			else {
				res.SetIndex(0);
			}
			int curIndex = column.VisibleIndex;
			if(curIndex == res.Index) {
				if(column.GroupIndex != -1) {
					GridColumnInfoArgs ci = ViewInfo.ColumnsInfo[column];
					if(ci != null) res.Bounds = ci.Bounds;
					return res;
				}
				return emptyRes;
			}
			if(curIndex > -1 && curIndex + 1 == res.Index) {
				if(column.GroupIndex != -1 && !View.OptionsView.ShowGroupedColumns) return res;
				return emptyRes;
			}
			if(!CheckColumnOptions(column, res)) return emptyRes;
			return res;
		}
		protected virtual bool CheckColumnOptions(GridColumn drColumn, ColumnPositionInfo pInfo) {
			int fLeftIndex = ViewInfo.FixedLeftColumn == null ? -1 : ViewInfo.FixedLeftColumn.VisibleIndex,
				fRightIndex = ViewInfo.FixedRightColumn == null ? -1 : ViewInfo.FixedRightColumn.VisibleIndex;
			if(fLeftIndex == -1 && fRightIndex == -1) return true;
			if(pInfo.InGroupPanel || pInfo.Index < 0) return true;
			if(fLeftIndex != -1 && pInfo.Index <= fLeftIndex && drColumn.Fixed != FixedStyle.Left) return false;
			if(fLeftIndex != -1 && pInfo.Index > fLeftIndex + 1 && drColumn.Fixed == FixedStyle.Left) return false;
			if(fRightIndex != -1 && pInfo.Index > fRightIndex && drColumn.Fixed != FixedStyle.Right) return false;
			if(fRightIndex != -1 && pInfo.Index < fRightIndex && drColumn.Fixed == FixedStyle.Right) return false;
			return true;
		}
		public virtual PositionInfo Calc(GridHitInfo hit) {
			if(DragObject is GridColumn) {
				return CalcColumnDrag(hit, DragObject as GridColumn);
			}
			return new PositionInfo();
		}
		protected virtual ColumnDropInfo CalcColumnInfoCore(GridColumnsInfo vcols, GridHitInfo hit, Rectangle panel) {
			GridColumnsInfo cols = new GridColumnsInfo();
			for(int n = 0; n < vcols.Count; n++) {
				GridColumnInfoArgs args = vcols[n];
				if(args.Column == null) continue;
				cols.Add(args);
			}
			GridColumn drCol = DragObject as GridColumn;
			Point pt = hit.HitPoint;
			ColumnDropInfo drInfo = new ColumnDropInfo();
			drInfo.PositionInfo = CreateColumnPositionInfo();
			ColumnPositionInfo res = drInfo.PositionInfo;
			GridColumnInfoArgs ci = ViewInfo.CalcColumnHitInfo(pt, cols);
			GridColumnInfoArgs ciF = cols.FirstColumnInfo, ciL = cols.LastColumnInfo;
			Rectangle r = panel;
			if(hit.HitTest == GridHitTest.FixedRightDiv) {
				ci = cols[ViewInfo.FindFixedRightColumn()];
			}
			if(hit.HitTest == GridHitTest.FixedLeftDiv) {
				ci = cols[ViewInfo.FindFixedLeftColumn()];
			}
			if(ci != null && ci.Column == null) ci = null;
			if(ci == null) {
				ci = cols.LastColumnInfo;
				if(ci == null) {
					res.SetIndex(9999);
					res.Bounds = r;
					res.Valid = true;
					return drInfo;
				}
				if(ci.Bounds.Left < pt.X) {
					res.SetIndex(cols.IndexOf(ci) + 1);
					r = ci.Bounds;
					if(UseArrows) {
						r.X = ci.Bounds.Right - 1;
					}
					else {
						r.Width = r.Width / 2;
						r.X = ci.Bounds.Right - r.Width;
					}
					res.Bounds = r;
					res.Valid = true;
					drInfo.Column = ci.Column;
					drInfo.Pos = DropPosition.Right;
					return drInfo;
				}
				ci = ciF;
			}
			drInfo.Column = ci.Column;
			res.SetIndex(cols.IndexOf(ci));
			DropPosition drPos = CalcByBounds(ci.Bounds, pt, false);
			if(ci.Column == View.CheckboxSelectorColumn && View.CheckboxSelectorColumn != null) drPos = DropPosition.Right;
			if(drPos == DropPosition.Right) {
				if(ci != ciL) {
					res.SetIndex(res.Index + 1);
					GridColumnInfoArgs newCi = cols[res.Index];
					if(newCi != null && ci.Column.Fixed == newCi.Column.Fixed) {
						ci = newCi;
						drInfo.Column = ci.Column;
						drPos = DropPosition.Left;
					} 
				}
			} else {
				if(drPos == DropPosition.Left) {
					if(ci.Column.Fixed == FixedStyle.Right && drCol.Fixed != FixedStyle.Right) {
						GridColumnInfoArgs nci = cols[cols.IndexOf(ci) - 1];
						if(nci != null && nci.Column != null) {
							ci = nci;
							drPos = DropPosition.Right;
						}
					}
				}
			}
			r = ci.Bounds;
			r.Width = r.Width / 2;
			if(View.IsRightToLeft) {
				r.X = ci.Bounds.Right - 1;
				r.Width = 1;
			}
			if(drPos == DropPosition.Right) {
				drInfo.Pos = DropPosition.Right;
				res.SetIndex(res.Index + 1);
				if(UseArrows) {
					r.Width = 1;
					if(!View.IsRightToLeft) r.X = ci.Bounds.Right - 1;
					else r.X = ci.Bounds.X;
				}
				else {
					r.X = ci.Bounds.Right - r.Width;
				}
			} 
			res.Bounds = r;
			res.Valid = true;
			return drInfo;
		}
		public class ColumnDropInfo {
			public ColumnPositionInfo PositionInfo;
			public GridColumn Column;
			public DropPosition Pos;
			public ColumnDropInfo() {
				Column = null;
				Pos = DropPosition.Left;
				PositionInfo = null;
			}
		}
		protected DropPosition CalcByBounds(Rectangle bounds, Point p) {
			return CalcByBounds(bounds, p, true); 
		}
		protected DropPosition CalcByBounds(Rectangle bounds, Point p, bool allowUpDown) {
			var res = CalcByBoundsCore(bounds, p, allowUpDown);
			if(View.IsRightToLeft) {
				if(res == DropPosition.Left) return DropPosition.Right;
				if(res == DropPosition.Right) return DropPosition.Left;
			}
			return res;
		}
		protected virtual DropPosition CalcByBoundsCore(Rectangle bounds, Point p, bool allowUpDown) {
			if(bounds.Contains(p) || !allowUpDown) {
				int dx = bounds.Width / 2, dy = bounds.Height / 4;
				if(allowUpDown) {
					if(p.Y > bounds.Bottom - dy) return DropPosition.Down;
				}
				if(p.X < bounds.Left + dx) return DropPosition.Left;
				return DropPosition.Right;
			}
			return p.Y < bounds.Top ? DropPosition.Up : DropPosition.Down;
		}
	}
	public class PositionInfo {
		protected bool fValid;
		protected int fIndex;
		protected Rectangle fBounds;
		bool isHorizontal;
		public PositionInfo() {
			this.isHorizontal = false;
			this.fValid = false;
			this.fBounds = Rectangle.Empty;
			this.fIndex = -1;
		}
		public virtual bool IsEquals(PositionInfo pi) {
			return this.Valid == pi.Valid && this.Bounds == pi.Bounds && this.Index == pi.Index;
		}
		public virtual bool Valid { get { return fValid; } set { fValid = value; } }
		public Rectangle Bounds { get { return fBounds; } set { fBounds = value; } }
		public int Index { get { return fIndex; } }
		public bool IsHorizontal { 
			get { return isHorizontal; } 
			internal set { isHorizontal = value; }
		}
		protected internal void SetIndex(int newIndex) { this.fIndex = newIndex; }
	}
	public class ColumnPositionInfo : PositionInfo {
		bool inGroupPanel;
		public ColumnPositionInfo() {
			this.inGroupPanel = false;
		}
		public override bool IsEquals(PositionInfo pi) {
			ColumnPositionInfo cpi = pi as ColumnPositionInfo;
			bool res = base.IsEquals(pi);
			if(!res || cpi == null) return false;
			return this.InGroupPanel == cpi.InGroupPanel;
		}
		public virtual bool InGroupPanel { get { return inGroupPanel; } }
		protected internal void SetInGroupPanel(bool newValue) { this.inGroupPanel = newValue; } 
	}
}
