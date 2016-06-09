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
using System.Windows.Forms;
using System.Drawing;
using System.Timers;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraTreeList.ViewInfo;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Helpers;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.Nodes.Operations;
using DevExpress.XtraTreeList.Dragging;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraTreeList.Menu;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Internal;
using DevExpress.XtraTreeList.Internal;
using DevExpress.XtraEditors.Repository;
using DevExpress.Utils.Drawing.Animation;
using System.Collections.Generic;
namespace DevExpress.XtraTreeList.Handler {
	public class StateData {
		TreeListHitTest downHitTest;
		DragMaster dragMaster;
		DragScrollInfo dragInfo;
		MouseHover mouseHover;
		int sizingLineX, sizingLineY;
		Rectangle dragColumnRect;
		Point lastDownPoint;
		TreeListCell selectionAnchor;
		TreeListCell selectionOldCell;
		Rectangle selectionBounds;
		TreeListNode anchorNode;
		bool canChangeAnchor, canImmediateEditor;
		TreeList treeList;
		int lockLostCaptureCounter;
		public StateData(TreeList treeList) {
			this.downHitTest = new TreeListHitTest();
			this.sizingLineX = sizingLineY = -1;
			this.dragColumnRect = Rectangle.Empty;
			this.lastDownPoint = Point.Empty;
			this.dragMaster = new DragMaster();
			this.dragInfo = new DragScrollInfo();
			this.treeList = treeList;
			this.mouseHover = new MouseHover(this);
			this.anchorNode = null;
			this.canChangeAnchor = true;
			this.canImmediateEditor = true;
			this.lockLostCaptureCounter = 0;
		}
		public void BeginLockLostCapture() { lockLostCaptureCounter++; }
		public void CancelLockLostCapture() { lockLostCaptureCounter--; }
		public Bitmap GetColumnDragBitmap(ColumnInfo ci) {
			Bitmap bmp = null;
			Graphics g = null;
			try {
				bmp = new Bitmap(ci.Bounds.Width, ci.Bounds.Height);
				g = Graphics.FromImage(bmp);
				treeList.Painter.DrawDragColumn(new DrawDragColumnEventArgs(g, treeList.ViewInfo.CloneDragInfo(ci)));
			}
			catch { }
			finally { if(g != null) g.Dispose(); }
			return bmp;
		}
		public Bitmap GetBandDragBitmap(BandInfo bi) {
			Bitmap bmp = null;
			Graphics g = null;
			try {
				bmp = new Bitmap(bi.Bounds.Width, bi.Bounds.Height);
				g = Graphics.FromImage(bmp);
				BandInfo dragInfo = new BandInfo(bi.Band);
				dragInfo.MouseOver = true;
				dragInfo.Bounds = new Rectangle(Point.Empty, bi.Bounds.Size);
				dragInfo.SetAppearance(treeList.ViewInfo.PaintAppearance.BandPanel);
				dragInfo.HeaderPosition = HeaderPositionKind.Special;
				treeList.ViewInfo.ElementPainters.BandPainter.CalcObjectBounds(dragInfo);
				treeList.Painter.DrawDragBand(g, dragInfo);
			}
			catch { }
			finally { if(g != null) g.Dispose(); }
			return bmp;
		}		
		public void RefreshDragArrow(RowInfo ri, DragEventArgs drgevent) {
			if(dragInfo.RowInfo != ri) {
				dragInfo.RowInfo = ri;
				RefreshDragArrowCore(drgevent);
			}
		}
		public void RefreshDragArrow(DragInsertPosition dragInsertPosition, bool copy, DragEventArgs drgevent) {
			if(dragInsertPosition != dragInfo.dragInsertPosition) {
				dragInfo.dragInsertPosition = dragInsertPosition;
				RefreshDragArrowCore(drgevent);
			}
			if(copy != dragInfo.copy) {
				dragInfo.copy = copy;
				RefreshDragArrowCore(drgevent);
			}
		}
		void RefreshDragArrowCore(DragEventArgs drgevent) {
			RefreshNodeDragImageIndex(drgevent);
			TreeList.ViewInfo.PaintAnimatedItems = false;
			treeList.InvalidateDragArrow();
		}
		private void RefreshNodeDragImageIndex(DragEventArgs drgevent) {
			CalcNodeDragImageIndexEventArgs e = GetCalcNodeDragImageIndexEventArgs(drgevent);
			if(e != null)
				dragInfo.imageIndex = treeList.InternalCalcNodeDragImageIndex(e);
		}
		private CalcNodeDragImageIndexEventArgs GetCalcNodeDragImageIndexEventArgs(DragEventArgs drgevent) {
			if(dragInfo.RowInfo == null) return null;
			Point ptClient = treeList.PointToClient(new Point(drgevent.X, drgevent.Y));
			int index = 0;
			if(dragInfo.dragInsertPosition == DragInsertPosition.After)
				index = 2;
			else if(dragInfo.dragInsertPosition == DragInsertPosition.Before)
				index = 1;
			return new CalcNodeDragImageIndexEventArgs(dragInfo.RowInfo.Node, index, ptClient, drgevent);
		}
		public bool IsLostCaptureLocked { get { return lockLostCaptureCounter != 0; } }
		public TreeListHitTest DownHitTest { get { return downHitTest; } set { downHitTest = value; } }
		public DragMaster DragMaster { get { return dragMaster; } }
		public DragScrollInfo DragInfo { get { return dragInfo; } }
		internal MouseHover MouseHover { get { return mouseHover; } }
		public TreeList TreeList { get { return treeList; } }
		public int SizingLineX { get { return sizingLineX; } set { sizingLineX = value; } }
		public int SizingLineY { get { return sizingLineY; } set { sizingLineY = value; } }
		public Rectangle DragColumnRect { get { return dragColumnRect; } set { dragColumnRect = value; } }
		public Point LastDownPoint { get { return lastDownPoint; } set { lastDownPoint = value; } }
		public TreeListNode AnchorNode { get { return anchorNode; } set { anchorNode = value; } }
		public TreeListCell SelectionOldCell { get { return selectionOldCell; } set { selectionOldCell = value; } }
		public TreeListCell SelectionAnchor { get { return selectionAnchor; } set { selectionAnchor = value; } }
		public Rectangle SelectionBounds { get { return selectionBounds; } set { selectionBounds = value; } }
		public bool CanChangeAnchor { get { return canChangeAnchor; } set { canChangeAnchor = value; } }
		public bool CanImmediateEditor { get { return canImmediateEditor; } set { canImmediateEditor = value; } }
	}
	public class TreeListHandler : IDisposable, IDesignNotified, IMouseWheelScrollClient, ISupportAnimatedScroll {
		public static int DefaultMouseWheelHorizontalScrollDelta = 40;
		protected TreeListControlState fControlState;
		protected StateData fStateData;
		int lockSetState;
		ISelectionService selectionService;
		MouseWheelScrollHelper mouseWheelHelper;
		public TreeListHandler(TreeList treeList) {
			this.lockSetState = 0;
			this.selectionService = null;
			this.mouseWheelHelper = new MouseWheelScrollHelper(this);
			this.AnimatedScrollHelper = new AnimatedScrollHelper(this);
			this.fStateData = CreateStateData(treeList);
			this.fControlState = CreateState(Regular);
			this.fControlState.Init();
		}
		protected AnimatedScrollHelper AnimatedScrollHelper { get; private set; }
		protected virtual TreeListControlState CreateState(TreeListState state) {
			TreeListControlState result = null;
			switch(state) {
				case TreeListState.Regular: result = new RegularState(this); break;
				case TreeListState.Editing: result = new EditingState(this); break;
				case TreeListState.OuterDragging: result = new OuterDraggingState(this); break;
				case TreeListState.NodeDragging: result = new NodeDraggingState(this); break;
				case TreeListState.NodePressed: result = new NodePressedState(this); break;
				case TreeListState.NodeSizing: result = new NodeSizingState(this); break;
				case TreeListState.ColumnDragging: result = TreeList.HasBands ? new MultiRowColumnDraggingState(this) : new ColumnDraggingState(this); break;
				case TreeListState.ColumnPressed: result = new ColumnPressedState(this); break;
				case TreeListState.ColumnSizing: result = new ColumnSizingState(this); break;
				case TreeListState.ColumnButtonPressed: result = new ColumnButtonPressedState(this); break;
				case TreeListState.Design: result = new DesignState(this); break;
				case TreeListState.MultiSelection: result = new MultiSelectionState(this); break;
				case TreeListState.CellSelection: result = new MultiCellSelectionState(this); break;
				case TreeListState.IncrementalSearch: result = new IncrementalSearchState(this); break;
				case TreeListState.BandPressed: result = new BandPressedState(this); break;
				case TreeListState.BandButtonPressed: result = new BandButtonPressedState(this); break;
				case TreeListState.BandDragging: result = new BandDraggingState(this); break;
				case TreeListState.BandSizing: result = new BandSizingState(this); break;
			}
			return result;
		}
		protected virtual StateData CreateStateData(TreeList treeList) {
			return new StateData(treeList);
		}
		public virtual void Dispose() {
			fStateData.DragInfo.Dispose();
			fControlState.Dispose();
			this.selectionService = null;
		}
		public void BeginLockSetState() { ++lockSetState; }
		public void EndLockSetState() { --lockSetState; }
		public TreeListHitTest GetHitTest(Point pt) {
			if(TreeList.CustomizationForm != null) {
				Point ptCust = TreeList.PointToScreen(pt);
				if(TreeList.CustomizationForm.RectangleToScreen(new Rectangle(Point.Empty, TreeList.CustomizationForm.Bounds.Size)).Contains(ptCust)) {
					TreeListHitTest ht = new TreeListHitTest();
					ht.MousePoint = pt;
					ht.HitInfoType = HitInfoType.CustomizationForm;
					TreeListBandCustomizationForm bandCustomizationForm = TreeList.CustomizationForm as TreeListBandCustomizationForm;
					if(bandCustomizationForm != null && bandCustomizationForm.IsBandsListBoxActive) {
						BandInfo band = bandCustomizationForm.BandsListBox.GetBandByPoint(ptCust);
						if(band != null)
							ht.BandInfo = band;
						return ht;
					}
					ht.ColumnInfo = TreeList.CustomizationForm.GetColumnInfoByPoint(TreeList.CustomizationForm.PointToClient(ptCust));
					return ht;
				}
			}
			TreeListHitTest scrollResult = TreeList.ScrollInfo.GetHitTest(pt);
			if(scrollResult.HitInfoType != HitInfoType.None)
				return scrollResult;
			return TreeList.ViewInfo.GetHitTest(pt);
		}
		public virtual void OnMouseDown(MouseEventArgs e) {
			if(TreeList.IsDisposed) return;
			TreeList.ContainerHelper.BeginAllowHideException();
			try {
				fControlState.MouseDown(e, GetHitTest(new Point(e.X, e.Y)));
			}
			finally {
				TreeList.ContainerHelper.EndAllowHideException();
			}
		}
		public virtual void OnMouseMove(MouseEventArgs e) { fControlState.MouseMove(e, GetHitTest(new Point(e.X, e.Y))); }
		public virtual void OnMouseUp(MouseEventArgs e) {
			if(TreeList.IsDisposed || !e.Button.HasLeft()) return;
			TreeList.ContainerHelper.BeginAllowHideException();
			try {
				TreeListHitTest ht = GetHitTest(new Point(e.X, e.Y));
				fControlState.MouseUp(e, ht);
				fStateData.DownHitTest.Clear();
				fStateData.MouseHover.CheckMouseHotTrack(ht);
			}
			finally {
				TreeList.ContainerHelper.EndAllowHideException();
			}
		}
		public virtual void OnDoubleClick() {
			if(TreeList.IsDisposed) return;
			Point pt = TreeList.PointToClient(Control.MousePosition);
			fControlState.DoubleClick(pt, GetHitTest(pt));
		}
		public virtual void OnMouseWheel(MouseEventArgs e) {
			mouseWheelHelper.OnMouseWheel(e);
		}
		public virtual void OnMouseEnter(Point pt) { fControlState.MouseEnter(pt); }
		public virtual void OnMouseLeave() { fControlState.MouseLeave(); }
		public virtual void OnKeyDown(KeyEventArgs e) { fControlState.KeyDown(e); }
		public virtual void OnKeyPress(KeyPressEventArgs e) { fControlState.KeyPress(e); }
		public virtual void OnKeyUp(KeyEventArgs e) { fControlState.KeyUp(e); }
		public virtual void OnResize() { fControlState.Resize(); }
		public virtual void OnGotFocus() { fControlState.GotFocus(); }
		public virtual void OnLostFocus() { fControlState.LostFocus(); }
		public virtual void OnLostCapture() { fControlState.LostCapture(); }
		public virtual void OnDragEnter(DragEventArgs e) { fControlState.DragEnter(e); }
		public virtual void OnDragOver(DragEventArgs e) { fControlState.DragOver(e); }
		public virtual void OnDragLeave() { fControlState.DragLeave(); }
		public virtual void OnQueryContinueDrag(QueryContinueDragEventArgs e) { fControlState.QueryContinueDrag(e); }
		public virtual void OnDragDrop(DragEventArgs e) { fControlState.DragDrop(e); }
		public virtual void OnSelectionChanged() { fControlState.SelectionChanged(); }
		public virtual void OnPositionChanged() { fControlState.PositionChanged(); }
		public virtual bool ProcessChildControlKey(KeyEventArgs e) { return fControlState.ProcessChildControlKey(e); }
		public virtual ToolTipControlInfo GetObjectTipInfo(Point point) { return fControlState.GetObjectTipInfo(point); }
		void IDesignNotified.OnMouseEnter() { OnMouseEnter(TreeList.PointToClient(Control.MousePosition)); }
		void IDesignNotified.OnSelectionChanged(ISelectionService selService) {
			this.selectionService = selService;
			TreeList.ViewInfo.PaintAnimatedItems = false;
			TreeList.Invalidate();
		}
		protected internal virtual bool IsComponentSelected(object component) {
			if(component == null || SelectionService == null) return false;
			return SelectionService.GetComponentSelected(component);
		}
		protected virtual void SelectComponent(object component) {
			SelectionService.SetSelectedComponents(new object[] { component }, ControlConstants.SelectionClick);
		}
		internal void RegisterLastDragEffect(DragDropEffects effect) { fStateData.DragInfo.CheckCustomDragDrop(effect); }
		protected internal void SetControlState(TreeListState state) {
			if(state == State || lockSetState != 0) return;
			TreeListControlState newState = CreateState(state);
			fControlState.Dispose();
			fControlState = newState;
			fControlState.Init();
			TreeList.RaiseStateChanged();
		}
		internal TreeListControlState GetControlState() { return fControlState; }
		protected TreeList TreeList { get { return fStateData.TreeList; } }
		public TreeListState State { get { return fControlState.State; } }
		public StateData StateData { get { return fStateData; } }
		protected internal TreeListState Regular { get { return (TreeList.IsDesignMode ? TreeListState.Design : TreeListState.Regular); } }
		protected ISelectionService SelectionService { get { return selectionService; } }
		protected internal DragInsertPosition GetCurrentDragInsertPosition() {
			return StateData.DragInfo.DragInsertPosition;
		}
		#region IMouseWheelScrollClient Members
		void IMouseWheelScrollClient.OnMouseWheel(MouseWheelScrollClientArgs e) {
			fControlState.MouseWheel(e);
		}
		bool IMouseWheelScrollClient.PixelModeHorz { get { return true; } }
		bool IMouseWheelScrollClient.PixelModeVert { get { return TreeList.IsPixelScrolling; } }
		#endregion
		public abstract class TreeListControlState : IDisposable {
			TreeListHandler handler;
			protected int fLockPositionChanged;
			public TreeListControlState(TreeListHandler handler) {
				this.handler = handler;
				this.fLockPositionChanged = 0;
			}
			public virtual void Init() { }
			bool isDisposing;
			protected virtual void OnDispose(){ }
			public void Dispose() {
				if(isDisposing) return;
				isDisposing = true;
				OnDispose();
			}
			protected void SetState(TreeListState state) { Handler.SetControlState(state); }
			protected virtual void SetNormalState() {
				if(ControlState == TreeListState.Editing) return;
				SetState(TreeListState.Regular);
			}
			public TreeListHitTest GetHitTest(Point pt) { return Handler.GetHitTest(pt); }
			public virtual void MouseDown(MouseEventArgs e, TreeListHitTest ht) { }
			public virtual void MouseMove(MouseEventArgs e, TreeListHitTest ht) { }
			public virtual void MouseUp(MouseEventArgs e, TreeListHitTest ht) { }
			public virtual void DoubleClick(Point pt, TreeListHitTest ht) {
				if(Data.LastDownPoint != pt) return;
				if(ht.HitInfoType == HitInfoType.ColumnEdge) {
					if(ht.ColumnInfo.Column != null && TreeList.CanResizeColumn(ht.ColumnInfo.Column) && Data.DownHitTest.ColumnInfo != null) {
						SetState(Regular);
						int bestWidth = TreeList.CalcColumnBestWidth(ht.ColumnInfo.Column);
						int index = Data.DownHitTest.ColumnInfo.Column.VisibleIndex;
						TreeList.ResizeColumn(index, bestWidth - ht.ColumnInfo.Column.VisibleWidth,
							ViewInfo.ViewRects.Rows.Right - ht.MousePoint.X);
					}
				}
				if(ht.HitInfoType == HitInfoType.RowIndicatorEdge && CanResizeNodes) {
					TreeList.SetDefaultRowHeight();
				}
				if(TreeList.OptionsBehavior.AllowExpandOnDblClick && IsHitTestNodeArea(ht.HitInfoType))
					ht.RowInfo.Node.Expanded = !ht.RowInfo.Node.Expanded;
			}
			protected bool IsHitTestNodeArea(HitInfoType hitInfoType) {
				return (hitInfoType == HitInfoType.Cell || hitInfoType == HitInfoType.Row ||
					hitInfoType == HitInfoType.RowIndent || hitInfoType == HitInfoType.RowIndicator ||
					hitInfoType == HitInfoType.RowPreview || hitInfoType == HitInfoType.SelectImage ||
					hitInfoType == HitInfoType.StateImage);
			}
			public virtual void MouseWheel(MouseEventArgs e) { }
			public virtual void MouseEnter(Point pt) { }
			public virtual void MouseLeave() { }
			public virtual bool ProcessChildControlKey(KeyEventArgs e) { return false; }
			public virtual void KeyDown(KeyEventArgs e) {
				if(e.KeyData == Keys.Escape)
					SetState(Regular);
			}
			public virtual void KeyPress(KeyPressEventArgs e) {
			}
			public virtual void KeyUp(KeyEventArgs e) {
			}
			public virtual void Resize() {
				SetState(Regular);
				TreeList.ScrollInfo.OnAction(DevExpress.XtraEditors.ScrollNotifyAction.Resize);
				ViewInfo.PixelScrollingInfo.Invalidate();
				ViewInfo.SummaryFooterInfo.NeedsRecalcRects = true;
				ViewInfo.SummaryFooterInfo.NeedsRecalcAll = true;
				if(TreeList.IsColumnHeaderAutoHeight)
					ViewInfo.RC.NeedsRestore = true;
				TreeList.autoHeights.Clear();
				TreeList.LayoutChanged();
				TreeList.CheckIncreaseVisibleRows();
			}
			public virtual void GotFocus() {
				TreeList.ContainerHelper.FocusEditor();
				if(TreeList.ContainerHelper.InternalFocusLock == 0) {
					TreeList.ViewInfo.PaintAnimatedItems = false;
					TreeList.Invalidate();
					TreeList.ViewInfo.PaintAnimatedItems = false;
					TreeList.Update();
				}
			}
			public void LostFocus() {
				if(!TreeList.HasFocus) {
					LostFocusCore();
				}
			}
			public virtual void LostCapture() {
				if(!CanSetNormalStateOnLostCapture) return;
				SetRegularState();
			}
			protected virtual void LostFocusCore() {
				SetRegularState();
			}
			private void SetRegularState() {
				Data.DownHitTest.Clear();
				TreeList.RefreshRowsInfo();
				if(State != TreeListState.IncrementalSearch)
					SetState(Regular);
			}
			#region DragDrop
			protected bool CanSetDragMode(int dx, int dy) {
				return (Math.Abs(dx) > SystemInformation.DragSize.Width || Math.Abs(dy) > SystemInformation.DragSize.Height);
			}
			public virtual void DragEnter(DragEventArgs drgevent) {
				drgevent.Effect = DragDropEffects.None;
				IDragNodesProvider dragProvider = GetDragData<IDragNodesProvider>(drgevent);
				if(dragProvider == null) return;
				if(dragProvider.TreeList != TreeList) {
					SetState(TreeListState.OuterDragging);
				}
				else {
					TreeListNode pressNode = TreeList.PressedNode;
					SetState(TreeListState.NodeDragging);
					TreeList.PressedNode = pressNode;
				}
				Handler.fControlState.DragEnter(drgevent);
			}
			protected T GetDragData<T>(DragEventArgs drgevent) {
				object data = drgevent.Data.GetData(typeof(T));
				if(data == null) return default(T);
				return (T)data;
			}
			public virtual void DragOver(DragEventArgs drgevent) { }
			public virtual void DragLeave() { }
			public virtual void QueryContinueDrag(QueryContinueDragEventArgs qcdevent) { }
			public virtual void DragDrop(DragEventArgs drgevent) { }
			public virtual ToolTipControlInfo GetObjectTipInfo(Point point) { return null; }
			#endregion
			public virtual void SelectionChanged() {
				if(TreeList.Selection.Count == 0)
					Data.AnchorNode = (TreeList.FocusedNode != null && TreeList.Nodes.Count > 0) ? TreeList.FocusedNode : null;
				TreeList.RaiseSelectionChanged();
			}
			public virtual void PositionChanged() {
				if(fLockPositionChanged != 0) return;
				if(TreeList.OptionsSelection.MultiSelect)
					TreeList.Selection.Set(TreeList.FocusedNode);
			}
			protected virtual void SetSelection(TreeListNode from, TreeListNode to, bool merge) {
				if(from == null || to == null) return;
				TreeListOperationAccumulateNodes op = new TreeListOperationAccumulateNodes();
				TreeList.NodesIterator.DoVisibleNodesOperation(op, from, to);
				if(merge) TreeList.Selection.Add(op.Nodes);
				else TreeList.Selection.Set(op.Nodes);
			}
			protected void FireChanged() { TreeList.FireChanged(); }
			protected Rectangle VisibleTLScreenRect {
				get {
					if(!TreeList.Visible) return Rectangle.Empty;
					if(TreeList.TopLevelControl == null) return TreeList.RectangleToScreen(TreeList.ClientRectangle);
					Rectangle topLevelRect = TreeList.TopLevelControl.RectangleToScreen(TreeList.TopLevelControl.ClientRectangle);
					Rectangle fullTLRect = TreeList.RectangleToScreen(TreeList.ClientRectangle);
					return Rectangle.Intersect(topLevelRect, fullTLRect);
				}
			}
			protected virtual bool CanResizeNodes {
				get {
					return (TreeList.OptionsBehavior.ResizeNodes &&
						!TreeList.ActualAutoNodeHeight);
				}
			}
			protected bool IsControl { get { return (Control.ModifierKeys & Keys.Control) != 0; } }
			protected bool IsShift { get { return (Control.ModifierKeys & Keys.Shift) != 0; } }
			protected bool IsAlt { get { return (Control.ModifierKeys & Keys.Alt) != 0; } }
			protected virtual bool CanSetNormalStateOnLostCapture { get { return !Data.IsLostCaptureLocked; } }
			protected TreeList TreeList { get { return Data.TreeList; } }
			protected TreeListState ControlState { get { return TreeList.State; } }
			protected TreeListViewInfo ViewInfo { get { return TreeList.ViewInfo; } }
			protected StateData Data { get { return Handler.fStateData; } }
			protected TreeListHandler Handler { get { return handler; } }
			protected TreeListState Regular { get { return Handler.Regular; } }
			public abstract TreeListState State { get; }
			protected virtual void ChangeSelection(RowInfo pressRowInfo, bool isLeftButton) {
				bool firstSelection = TreeList.Selection.Count == 0 && TreeList.FocusedRowIndex != pressRowInfo.VisibleIndex;
				if(pressRowInfo.VisibleIndex < TreeList.RowCount) { 
					if(TreeList.OptionsSelection.MultiSelect) {
						fLockPositionChanged++;
						try {
							TreeList.InvokeSelectionAction(() => { TreeList.FocusedRowIndex = pressRowInfo.VisibleIndex; });
						}
						finally {
							fLockPositionChanged--; 
						}
					}
					else {
						TreeList.FocusedRowIndex = pressRowInfo.VisibleIndex;
					}
				}
				if(TreeList.FocusedRowIndex == pressRowInfo.VisibleIndex) {
					if(TreeList.OptionsSelection.MultiSelect) {
						if(TreeList.IsCellSelect)
							MouseCellMultiSelect(pressRowInfo.Node, isLeftButton);
						else
							MouseMultiSelect(pressRowInfo.Node, firstSelection, isLeftButton);
					}
				}
			}
#region cellselection
			protected virtual void MouseCellMultiSelect(TreeListNode pressedNode, bool isLeftButtonDown) {
				if(TreeListAutoFilterNode.IsAutoFilterNode(pressedNode)) return;
				if(!IsShift) {
					Data.SelectionAnchor = Data.SelectionOldCell = new TreeListCell(TreeList.FocusedNode, TreeList.FocusedColumn);
					Data.SelectionBounds = Rectangle.Empty;
				}
				if(!IsControl && !IsShift) {
					if(TreeList.OptionsBehavior.KeepSelectedOnClick && TreeList.IsCellSelected(pressedNode, TreeList.FocusedColumn))
						return;
					TreeList.Selection.InternalClear();
				}
				if(!IsShift) {
					if(IsControl) {
						InvertSelectionCore(pressedNode, Data.DownHitTest.HitInfoType == HitInfoType.RowIndicator);
					}
					else {
						if(Data.DownHitTest.HitInfoType == HitInfoType.RowIndicator)
							TreeList.SelectNode(pressedNode);
						else
							TreeList.SelectCell(pressedNode, TreeList.FocusedColumn);
					}
				}
				else {
					if(Data.SelectionAnchor == null)
						Data.SelectionAnchor = Data.SelectionOldCell != null ? Data.SelectionAnchor = Data.SelectionOldCell : new TreeListCell(TreeList.FocusedNode, TreeList.FocusedColumn);
					if(Data.DownHitTest.HitInfoType != HitInfoType.RowIndicator)
						SelectAnchorRange(new TreeListCell(pressedNode, TreeList.FocusedColumn), Data.SelectionAnchor);
					else
						SelectAnchorRange(new TreeListCell(pressedNode, null), new TreeListCell(Data.SelectionAnchor.Node, TreeList.VisibleColumns[TreeList.VisibleColumns.Count - 1]));
				}
			}
			protected internal virtual void SelectAnchorRange(TreeListCell start, TreeListCell end) {
				if(start.Node == null || start.Column == null || end.Node == null || end.Column == null) return;
				Rectangle oldSelectionRectangle = Data.SelectionBounds;
				int visibleIndex1 = TreeList.GetVisibleIndexByNode(start.Node), visibleIndex2 = TreeList.GetVisibleIndexByNode(end.Node);
				if(visibleIndex1 > visibleIndex2) {
					int a = visibleIndex1; visibleIndex1 = visibleIndex2; visibleIndex2 = a;
				}
				int colIndex1 = start.Column.VisibleIndex, colIndex2 = end.Column.VisibleIndex;
				if(colIndex1 > colIndex2) {
					int a = colIndex1; colIndex1 = colIndex2; colIndex2 = a;
				}
				TreeList.BeginSelection();
				try {
					Rectangle rect = new Rectangle(colIndex1, visibleIndex1, colIndex2 - colIndex1 + 1, visibleIndex2 - visibleIndex1 + 1);
					if(!oldSelectionRectangle.IsEmpty && oldSelectionRectangle.IntersectsWith(rect)) {
						TreeList.SetCellSelectionCore(oldSelectionRectangle.Top, oldSelectionRectangle.Left, oldSelectionRectangle.Bottom - 1, oldSelectionRectangle.Right - 1, false, true);
					}
					Data.SelectionBounds = rect;
					TreeList.SetCellSelectionCore(visibleIndex1, colIndex1, visibleIndex2, colIndex2, true, true);
				}
				finally {
					TreeList.EndSelection();
				}
			}
			protected void InvertSelectionCore(TreeListNode pressedNode, bool invertSelectionForEntireRow) {
				if(invertSelectionForEntireRow) {
					if(TreeList.Selection.Contains(pressedNode))
						TreeList.UnselectNode(pressedNode);
					else
						TreeList.SelectNode(pressedNode);
					return;
				}
				if(TreeList.IsCellSelected(pressedNode, TreeList.FocusedColumn))
					TreeList.UnselectCell(pressedNode, TreeList.FocusedColumn);
				else
					TreeList.SelectCell(pressedNode, TreeList.FocusedColumn);
			}
#endregion
			protected virtual void MouseMultiSelect(TreeListNode pressNode, bool firstSelection, bool isLeftButton) {
				if(TreeListAutoFilterNode.IsAutoFilterNode(pressNode)) return;
				if(IsControl) {
					if(IsShift) {
						SetSelection(Data.AnchorNode, pressNode, true);
					}
					else {
						Data.AnchorNode = pressNode;
						if(!firstSelection && isLeftButton)
							TreeList.Selection.AddRemove(pressNode);
					}
				}
				else if(IsShift) {
					if(Data.AnchorNode == null) {
						Data.AnchorNode = TreeList.Selection.Count > 0 ? TreeList.Selection[0] : TreeList.FocusedNode;
					}
					SetSelection(Data.AnchorNode, pressNode, false);
				}
				else if(!TreeList.Selection.Contains(pressNode) || !TreeList.OptionsBehavior.KeepSelectedOnClick) {
					TreeList.Selection.Set(pressNode);
					Data.AnchorNode = pressNode;
				}
				else if(firstSelection && TreeList.Selection.Contains(pressNode)) {
					Data.AnchorNode = pressNode;
				}
			}
		}
		public abstract class HotTrackState : TreeListControlState {
			protected HotTrackState(TreeListHandler handler) : base(handler) { }
			public override void MouseEnter(Point pt) {
				TreeListHitTest ht = GetHitTest(pt);
				Data.MouseHover.CheckMouseHotTrack(ht);
			}
			public override void MouseLeave() {
				Data.MouseHover.CheckMouseHotTrack(null);
			}
			public override void MouseMove(MouseEventArgs e, TreeListHitTest ht) {
				Data.MouseHover.CheckMouseHotTrack(ht);
				CheckMouseCursor(ht);
			}
			protected virtual void CheckMouseCursor(TreeListHitTest ht) {
				bool resetCursor = true;
				if(ht.HitInfoType == HitInfoType.ColumnEdge) {
					if(ht.ColumnInfo.Column != null && TreeList.CanResizeColumn(ht.ColumnInfo.Column)) {
						Cursor.Current = Cursors.SizeWE;
						resetCursor = false;
					}
				}
				if(ht.HitInfoType == HitInfoType.BandEdge && TreeList.CanResizeBand(ht.Band)) {
					Cursor.Current = Cursors.SizeWE;
					resetCursor = false;
				}
				if(ht.HitInfoType == HitInfoType.RowIndicatorEdge && CanResizeNodes) {
					Cursor.Current = Cursors.SizeNS;
					resetCursor = false;
				}
				if(ht.HitInfoType == HitInfoType.Cell) {
					CellInfo cell = ht.CellInfo;
					if(cell != null) {
						BaseEditViewInfo editInfo = cell.EditorViewInfo;
						if(editInfo != null) {
							Cursor cursor = editInfo.GetMouseCursor(ht.MousePoint);
							if(cursor != Cursors.Default) {
								Cursor.Current = cursor;
								resetCursor = false;
							}
						}
					}
				}
				if(resetCursor)
					Cursor.Current = TreeList.Cursor;
			}
		}
		public abstract class NormalState : HotTrackState {
			protected int fLockSelectionChanged;
			public NormalState(TreeListHandler handler)
				: base(handler) {
				this.fLockSelectionChanged = 0;
			}
			public override void MouseDown(MouseEventArgs e, TreeListHitTest ht) {
				if(e.Button.IsRight()) {
					if(TreeList.OptionsSelection.SelectNodesOnRightClick && ht.InRow && ht.RowInfo != null) {
						if(ht.CellInfo != null && ht.CellInfo.Column != null)
							TreeList.FocusedCellIndex = TreeList.VisibleColumns.IndexOf(ht.CellInfo.Column);
						ChangeSelection(ht.RowInfo, false);
					}
					DoCheckShowMenu(ht);
					return;
				}
				Data.LastDownPoint = new Point(e.X, e.Y);
				Data.DownHitTest = ht; 
				Data.MouseHover.CheckMouseHotTrack(Data.DownHitTest);
				CheckMouseCursor(Data.DownHitTest);
				if(Data.DownHitTest.HitInfoType == HitInfoType.Column || (Data.DownHitTest.HitInfoType == HitInfoType.CustomizationForm && Data.DownHitTest.ColumnInfo != null)) {
					SetState(TreeListState.ColumnPressed);
					return;
				}
				if(Data.DownHitTest.HitInfoType == HitInfoType.Band || (Data.DownHitTest.HitInfoType == HitInfoType.CustomizationForm && Data.DownHitTest.BandInfo != null)) {
					SetState(TreeListState.BandPressed);
					return;
				}
				if(Data.DownHitTest.HitInfoType == HitInfoType.ColumnButton) {
					SetState(TreeListState.ColumnButtonPressed);
					return;
				}
				if(Data.DownHitTest.HitInfoType == HitInfoType.BandButton) {
					SetState(TreeListState.BandButtonPressed);
					return;
				}
				if(Data.DownHitTest.InRow || Data.DownHitTest.HitInfoType == HitInfoType.RowIndicatorEdge && !CanResizeNodes) {
					if(e.Clicks > 1) {
						if(!TreeList.OptionsBehavior.ImmediateEditor && !TreeList.ShowEditorOnMouseUp)
							return;
					}
					if(e.Clicks == 1)
						OnPressNode();
					if(e.Button.IsLeft() && ht.HitInfoType == HitInfoType.RowIndicator && TreeList.OptionsSelection.UseIndicatorForSelection) {
						if(TreeList.OptionsSelection.MultiSelect) {
							if(TreeList.IsCellSelect)
								SetState(TreeListState.CellSelection);
							else
								SetState(TreeListState.MultiSelection);
						}
					}
					return;
				}
				if(Data.DownHitTest.HitInfoType == HitInfoType.ColumnEdge && Data.DownHitTest.Column != null) {
					TreeListBand parentBand = Data.DownHitTest.Column.ParentBand;
					if(parentBand != null) {
						TreeListBand band = null;
						if(TreeList.AllowBandColumnsMultiRow) {
							TreeListBandRow row = TreeList.GetBandRows(parentBand).FindRow(Data.DownHitTest.Column);
							if(row != null) {
								int index = row.Columns.IndexOf(Data.DownHitTest.Column);
								if(index == row.Columns.Count - 1)
									band = parentBand;
							}
						}
						else {
							int index = parentBand.Columns.IndexOf(Data.DownHitTest.Column);
							if(index == parentBand.Columns.Count - 1)
								band = parentBand;
						}
						if(band != null && TreeList.CanResizeBand(band)) {
							Data.DownHitTest.BandInfo = ViewInfo.BandsInfo.FindBand(band);
							if(Data.DownHitTest.BandInfo != null) {
								SetState(TreeListState.BandSizing);
								return;
							}
						}
					}
					if(TreeList.CanResizeColumn(Data.DownHitTest.Column))
						SetState(TreeListState.ColumnSizing);
					return;
				}
				if(Data.DownHitTest.HitInfoType == HitInfoType.BandEdge) {
					TreeListBand band = Data.DownHitTest.Band;
					if(TreeList.CanResizeBand(band))
						SetState(TreeListState.BandSizing);
					return;
				}
				if(Data.DownHitTest.HitInfoType == HitInfoType.RowIndicatorEdge && CanResizeNodes) {
					SetState(TreeListState.NodeSizing);
					return;
				}
				if(Data.DownHitTest.HitInfoType == HitInfoType.Button) {
					OnExpandNode(Data.DownHitTest.RowInfo.Node);
					return;
				}
				if(Data.DownHitTest.HitInfoType == HitInfoType.NodeCheckBox) {
					OnCheckNode(Data.DownHitTest.RowInfo.Node);
					return;
				}
			}
			protected virtual void OnPressNode() {
				SetState(TreeListState.NodePressed);
			}
			protected virtual void OnExpandNode(TreeListNode node) {
				node.Expanded = !node.Expanded;
			}
			protected virtual void OnCheckNode(TreeListNode node) {
				if(node == null) return;
				CheckNodeEventArgs e = TreeList.RaiseBeforeCheckNode(node, node.CheckState, GetNextCheckState(node.CheckState));
				if(!e.CanCheck) return;
				TreeList.SetNodeCheckState(node, e.State, TreeList.OptionsBehavior.AllowRecursiveNodeChecking);
				TreeList.RaiseAfterCheckNode(node);
			}
			protected virtual CheckState GetNextCheckState(CheckState state) {
				if(state == CheckState.Checked)
					return CheckState.Unchecked;
				if(state == CheckState.Unchecked) {
					if(TreeList.OptionsBehavior.AllowIndeterminateCheckState)
						return CheckState.Indeterminate;
					else
						return CheckState.Checked;
				}
				return CheckState.Checked; 
			}
			public override void MouseWheel(MouseEventArgs e) {
				TreeList.GetToolTipController().HideHint();
			}
			public override void KeyDown(KeyEventArgs e) {
				TreeList.ContainerHelper.BeginAllowHideException();
				try {
					KeyEventArgs ee = TranslateRTLKeys(e);
					KeyDownCore(ee);
				}
				catch(HideException) { }
				finally {
					TreeList.ContainerHelper.EndAllowHideException();
				}
			}
			public override ToolTipControlInfo GetObjectTipInfo(Point point) {
				TreeListHitTest ht = GetHitTest(point);
				if(ht.HitInfoType == HitInfoType.Column && ht.ColumnInfo != null && ht.Column != null) {
					string description = ht.Column.GetDescription();
					if(!string.IsNullOrEmpty(description) && string.IsNullOrEmpty(ht.Column.ToolTip))
						return new ToolTipControlInfo(ht.Column, description, ht.Column.GetCaption());
					ViewInfo.GInfo.AddGraphics(null);
					try {
						if(!ViewInfo.ViewRects.IsColumnRectVisible(ht.ColumnInfo.Bounds) || !ViewInfo.ElementPainters.HeaderPainter.IsCaptionFit(ViewInfo.GInfo.Cache, ht.ColumnInfo) || !string.IsNullOrEmpty(ht.Column.ToolTip))
							return new ToolTipControlInfo(ht.Column, string.IsNullOrEmpty(ht.Column.ToolTip) ? ht.ColumnInfo.GetTextCaption() : ht.Column.ToolTip);
					}
					finally { ViewInfo.GInfo.ReleaseGraphics(); }
				}
				if(ht.HitInfoType == HitInfoType.Cell) {
					if(ht.CellInfo.EditorViewInfo != null) {
						if(ht.CellInfo.EditorViewInfo.ErrorIconBounds.Contains(point)) {
							if(ht.CellInfo.EditorViewInfo.ErrorIconText == null || ht.CellInfo.EditorViewInfo.ErrorIconText == string.Empty) return null;
							ToolTipControlInfo res = new ToolTipControlInfo(ht.CellInfo.EditorViewInfo, ht.CellInfo.EditorViewInfo.ErrorIconText);
							res.ToolTipImage = ht.CellInfo.EditorViewInfo.ErrorIcon;
							return res;
						}
						ToolTipControlInfo info = ht.CellInfo.EditorViewInfo.GetToolTipInfo(point);
						if(info != null) {
							info.Object = new TreeListCellToolTipInfo(ht.Node, ht.Column, info.Object);
							return info;
						}
					}
				}
				if(ht.HitInfoType == HitInfoType.RowIndicator && (ht.RowInfo.ErrorText != null && ht.RowInfo.ErrorText != string.Empty)) {
					ToolTipControlInfo res = new ToolTipControlInfo(ht.RowInfo, ht.RowInfo.ErrorText);
					res.ToolTipImage = ht.RowInfo.ErrorIcon;
					return res;
				}
				return null;
			}
			public override void MouseUp(MouseEventArgs e, TreeListHitTest ht) {
				base.MouseUp(e, ht);
				if(State == TreeListState.Regular) {
					if(Control.ModifierKeys == Keys.None) {
						if(ht.HitInfoType == HitInfoType.Cell && ht.Column == TreeList.FocusedColumn && ht.Node == TreeList.FocusedNode) {
							if(Data.CanImmediateEditor && ((!TreeList.OptionsBehavior.ImmediateEditor && !TreeList.ShowEditorOnMouseUp) || TreeList.IsCellSelect)) {
								TreeList.ShowEditor();
							}
						}
					}
					if(ht.HitInfoType == HitInfoType.FilterPanelCustomizeButton)
						TreeList.ShowFilterEditor(null);
					if(ht.HitInfoType == HitInfoType.FilterPanelCloseButton)
						TreeList.ClearColumnsFilter();
					if(ht.HitInfoType == HitInfoType.FilterPanelActiveButton)
						TreeList.ActiveFilterEnabled = !TreeList.ActiveFilterEnabled;
					if(ht.HitInfoType == HitInfoType.FilterPanelMRUButton || ht.HitInfoType == HitInfoType.FilterPanelText)
						TreeList.ShowMRUFilterPopup();
					if(ht.HitInfoType == HitInfoType.ColumnFilterButton && Data.DownHitTest != null && Data.DownHitTest.HitInfoType == HitInfoType.ColumnFilterButton) {
						TreeList.ShowFilterPopup(ht.Column);
					}
				}
			}
			protected bool isExpandCollapse = false;
			bool CanNavigateLeftRight(KeyEventArgs e, bool checkLeft) {
				if(e.Control || (checkLeft && e.KeyCode == Keys.Subtract) || (!checkLeft && e.KeyCode == Keys.Add) ) {
					if(TreeList.FocusedRow != null && TreeList.FocusedRow.Node.HasChildren && TreeList.FocusedRow.Node.Expanded == checkLeft) 
						return false;
				}
				return true;
			}
			protected virtual void NavigateControl(KeyEventArgs e) {
				int delta = 0;
				isExpandCollapse = false;
				switch(e.KeyCode) {
					case Keys.Left:
					case Keys.Subtract:
						if(CanNavigateLeftRight(e, true)) {
							if(!ProcessBandHorizontalNavigation(e, -1))
								MoveFocusedCellCore(-1);
						}
						else {
							TreeList.FocusedRow.Node.Expanded = false;
							isExpandCollapse = true;
						}
						break;
					case Keys.Right:
					case Keys.Add:
						if(CanNavigateLeftRight(e, false)) {
							if(!ProcessBandHorizontalNavigation(e, 1))
								MoveFocusedCellCore(1);
						}
						else {
							TreeList.FocusedRow.Node.Expanded = true;
							isExpandCollapse = true;
						}
						break;
					case Keys.Tab:
						MoveFocusedCellCore(e.Shift ? -1 : 1, true);
						break;
					case Keys.Up:
						delta = -1; break;
					case Keys.Down:
						delta = 1; break;
					case Keys.PageUp:
						delta = -ViewInfo.VisibleRowCount; break;
					case Keys.PageDown:
						delta = ViewInfo.VisibleRowCount; break;
					case Keys.Home:
						TreeList.FocusedCellIndex = 0;
						if(e.Control || TreeList.VisibleColumns.Count == 1)
							MoveFirstOrLast(true, e.Shift, e.Control);
						break;
					case Keys.End:
						TreeList.FocusedCellIndex = TreeList.VisibleColumns.Count - 1;
						if(e.Control || TreeList.VisibleColumns.Count == 1)
							MoveFirstOrLast(false, e.Shift, e.Control);
						break;
				}
				if(delta != 0) {
					bool shouldLockSelectionChanged = (TreeList.Selection.Count != 0); 
					if(shouldLockSelectionChanged) fLockSelectionChanged++;
					fLockPositionChanged++;
					try {
						if(!ProcessBandVerticalNavigation(e, delta, (args, d) => { TreeList.InvokeSelectionAction(() => { TreeList.MoveFocusedRow(d, args); }); }))
							TreeList.InvokeSelectionAction(() => { TreeList.MoveFocusedRow(delta, e); });
					}
					finally {
						if(shouldLockSelectionChanged) fLockSelectionChanged--;
						fLockPositionChanged--;
					}
					if(TreeList.OptionsSelection.MultiSelect) {
						if(TreeList.IsCellSelect)
							KeyboardMultiCellSelect(TreeList.FocusedNode, e.Shift, e.Control, e.KeyCode == Keys.Tab);
						else
							KeyboardMultiSelect(TreeList.FocusedNode, e.Shift, e.Control);
					}
				}
			}
			protected virtual bool AllowBandNavigation { get { return TreeList.HasBands && TreeList.AllowBandColumnsMultiRow; } }
			protected virtual bool ProcessBandHorizontalNavigation(KeyEventArgs e, int delta) {
				if(!AllowBandNavigation || !TreeList.UseBandsAdvancedHorizontalNavigation || TreeList.VisibleColumns.Count == 0 || delta == 0)
					return false;
				TreeListColumn focusedColumn = TreeList.FocusedColumn ?? TreeList.VisibleColumns[0];
				if(e.KeyCode == Keys.Right || e.KeyCode == Keys.Left) {
					TreeListBandRowCollection rows = TreeList.GetFocusableBandRows(focusedColumn.ParentBand);
					TreeListBandRow row = rows.FindRow(focusedColumn);
					if(row == null) return false;
					int columnIndex = row.Columns.IndexOf(focusedColumn);
					int rowIndex = rows.IndexOf(row);
					columnIndex += delta;
					if(columnIndex >= 0 && columnIndex < row.Columns.Count) {
						TreeList.FocusedColumn = row.Columns[columnIndex];
						return true;
					}
					TreeListColumn newColumn = FindNearestBandColumn(focusedColumn.ParentBand, delta, rowIndex);
					if(newColumn != null) {
						TreeList.FocusedColumn = newColumn;
						return true;
					}
				}
				return false;
			}
			protected virtual TreeListColumn FindNearestBandColumn(TreeListBand band, int delta, int rowIndex) {
				if(band == null) {
					if(TreeList.VisibleColumns.Count == 0) return null;
					TreeListColumn column = TreeList.VisibleColumns[delta > 0 ? 0 : TreeList.VisibleColumns.Count - 1];
					return GetNearestBandColumnCore(column.ParentBand, rowIndex, delta > 0);
				}
				int bandIndex = TreeList.GetBandVisibleIndex(band);
				if(delta > 0) {
					for(int i = bandIndex + 1; i < band.OwnedCollection.Count; i++) {
						if(!band.OwnedCollection[i].Visible) continue;
						TreeListColumn column = GetNearestBandColumnCore(band.OwnedCollection[i], rowIndex, delta > 0);
						if(column != null)
							return column;
					}
					return FindNearestBandColumn(band.OwnedCollection.OwnerBand, delta, rowIndex);
				}
				else {
					for(int i = bandIndex - 1; i >= 0; i--) {
						if(!band.OwnedCollection[i].Visible) continue;
						TreeListColumn column = GetNearestBandColumnCore(band.OwnedCollection[i], rowIndex, delta > 0);
						if(column != null)
							return column;
					}
					return FindNearestBandColumn(band.OwnedCollection.OwnerBand, delta, rowIndex);
				}
			}
			TreeListColumn GetNearestBandColumnCore(TreeListBand band, int rowIndex, bool right) {
				if(band.Bands.VisibleCount != 0) {
					if(right) {
						for(int i = 0; i < band.Bands.Count; i++) {
							if(!band.Bands[i].Visible) continue;
							TreeListColumn column = GetNearestBandColumnCore(band.Bands[i], rowIndex, right);
							if(column != null)
								return column;
						}
					}
					else {
						for(int i = band.Bands.Count - 1; i >= 0; i--) {
							if(!band.Bands[i].Visible) continue;
							TreeListColumn column = GetNearestBandColumnCore(band.Bands[i], rowIndex, right);
							if(column != null)
								return column;
						}
					}
				}
				TreeListBandRowCollection rows = TreeList.GetFocusableBandRows(band);
				rowIndex = Math.Min(rowIndex, rows.Count - 1);
				if(rowIndex >= 0)
					return rows[rowIndex].Columns[right ? 0 : rows[rowIndex].Columns.Count - 1];
				return null;
			}
			protected virtual bool ProcessBandVerticalNavigation(KeyEventArgs e, int delta, Action<KeyEventArgs, int> defaultAction) {
				if(!AllowBandNavigation || !TreeList.UseBandsAdvancedVerticalNavigation || TreeList.VisibleColumns.Count == 0 || delta == 0)
					return false;
				TreeListColumn focusedColumn = TreeList.FocusedColumn ?? TreeList.VisibleColumns[0];
				if(e.KeyCode == Keys.Down || e.KeyCode == Keys.Up) {
					TreeListBandRowCollection rows = TreeList.GetFocusableBandRows(focusedColumn.ParentBand);
					TreeListBandRow row = rows.FindRow(focusedColumn);
					if(row == null) return false;
					int rowIndex = rows.IndexOf(row);
					int columnIndex = row.Columns.IndexOf(focusedColumn);
					rowIndex += delta;
					if(rowIndex != -1 && rowIndex < rows.Count) {
						TreeListBandRow newRow = rows[rowIndex];
						if(newRow.Columns.Count > 0) {
							TreeList.FocusedColumn = newRow.Columns[Math.Min(columnIndex, newRow.Columns.Count - 1)];
							return true;
						}
					}
					int prevRowIndex = TreeList.FocusedRowIndex;
					defaultAction(e, delta);
					if(prevRowIndex != TreeList.FocusedRowIndex) {
						rows = TreeList.GetFocusableBandRows(focusedColumn.ParentBand);
						if(rows.Count > 0) {
							row = rows[delta > 0 ? 0 : rows.Count - 1];
							if(row.Columns.Count > 0)
								TreeList.FocusedColumn = row.Columns[Math.Min(columnIndex, row.Columns.Count - 1)];
						}
					}
					return true;
				}
				return false;
			}
			void MoveFirstOrLast(bool first, bool shift, bool control) {
				bool useRangeSelection = (Data.AnchorNode != null || Data.SelectionAnchor != null) && shift && control;
				if(useRangeSelection) fLockSelectionChanged++;
				try {
					if(first)
						TreeList.MoveFirst();
					else
						TreeList.MoveLastVisible();
				}
				finally {
					if(useRangeSelection) fLockSelectionChanged--;
				}
				if(useRangeSelection) {
					if(TreeList.IsCellSelect)
						KeyboardMultiCellSelect(TreeList.FocusedNode, true, true, false);
					else
						KeyboardMultiSelect(TreeList.FocusedNode, true, true);
				}
				Data.AnchorNode = TreeList.FocusedNode;
			}
			public override bool ProcessChildControlKey(KeyEventArgs e) {
				if(InKeys(navKeys, e.KeyCode) || e.KeyCode == Keys.Escape || e.KeyCode == Keys.Enter) {
					if(IsAlt)
						return false;
					KeyDown(e);
					return true;
				}
				return base.ProcessChildControlKey(e);
			}
			protected virtual void KeyboardMultiSelect(TreeListNode node, bool shift, bool ctrl) {
				if(!TreeList.OptionsSelection.MultiSelect || TreeList.OptionsSelection.MultiSelectMode != TreeListMultiSelectMode.RowSelect) return;
				if(ctrl) {
					if(shift) SetSelection(Data.AnchorNode, node, true);
					else Data.AnchorNode = node;
				}
				else if(shift) {
					if(Data.AnchorNode == null) {
						Data.AnchorNode = TreeList.Selection.Count > 0 ? TreeList.Selection[0] : TreeList.FocusedNode;
					}
					SetSelection(Data.AnchorNode, node, false);
				}
				else {
					Data.AnchorNode = node;
					TreeList.Selection.Set(node);
				}
			}
			protected void MoveFocusedCellCore(int delta, bool isTab = false) {
				TreeList.InvokeSelectionAction(() => { TreeList.MoveFocusedCell(delta); });
				if(TreeList.IsCellSelect)
					KeyboardMultiCellSelect(TreeList.FocusedNode, IsShift, IsControl, isTab);
			}
			protected virtual void KeyboardMultiCellSelect(TreeListNode node, bool shift, bool ctrl, bool isTab) {
				if(node == null) return;
				TreeList.BeginSelection();
				try {
					if(!ctrl && !(shift && !isTab))
						TreeList.Selection.InternalClear();
					if(!shift || isTab) {
						if(!ctrl)
							TreeList.SelectCell(node, TreeList.FocusedColumn);
						Data.SelectionAnchor = Data.SelectionOldCell = new TreeListCell(TreeList.FocusedNode, TreeList.FocusedColumn);
						Data.SelectionBounds = Rectangle.Empty;
					}
					else {
						if(Data.SelectionAnchor == null)
							Data.SelectionAnchor = Data.SelectionOldCell != null ? Data.SelectionAnchor = Data.SelectionOldCell : new TreeListCell(TreeList.FocusedNode, TreeList.FocusedColumn);
						SelectAnchorRange(new TreeListCell(node, TreeList.FocusedColumn), Data.SelectionAnchor);
					}
				}
				finally {
					TreeList.EndSelection();
				}
			}
			public override void SelectionChanged() {
				if(fLockSelectionChanged != 0) return;
				TreeList.RefreshRowsInfo(false, TreeList.IsCellSelect);
				base.SelectionChanged();
			}
			private bool InKeys(Keys[] keys, Keys key) {
				foreach(Keys item in keys) {
					if(item == key) return true;
				}
				return false;
			}
			protected virtual Keys[] navKeys {
				get {
					return new Keys[] {
											Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.PageDown, Keys.PageUp,
											Keys.End, Keys.Home, Keys.Tab};
				}
			}
			public Keys TranslateRTLKeyCode(Keys e) {
				if(ViewInfo == null || !ViewInfo.IsRightToLeft) return e;
				if(e == Keys.Right) return Keys.Left;
				if(e == Keys.Left) return Keys.Right;
				return e;
			}
			public Keys TranslateRTLKeyData(Keys e) {
				if(ViewInfo == null || !ViewInfo.IsRightToLeft) return e;
				Keys keyCode = e & (~Keys.Modifiers);
				Keys res = TranslateRTLKeyCode(keyCode);
				if(res != keyCode) return res | (e & Keys.Modifiers);
				return e;
			}
			public KeyEventArgs TranslateRTLKeys(KeyEventArgs e) {
				if(ViewInfo == null || !ViewInfo.IsRightToLeft) return e;
				Keys data = TranslateRTLKeyData(e.KeyData);
				if(data != e.KeyData) return new KeyEventArgs(data);
				return e;
			}
			protected virtual bool CanMoveByEnter { get { return TreeList.OptionsNavigation.EnterMovesNextColumn; } }
			protected virtual bool CanFocusControl { get { return true; } }
			protected override bool CanSetNormalStateOnLostCapture { get { return false; } }
			protected abstract void KeyDownCore(KeyEventArgs e);
			#region Menu
			protected virtual void ShowTreeListMenu(TreeListMenu menu, Point p) {
				if(menu != null) {
					TreeList.DoShowTreeListMenu(menu, p);
				}
			}
			protected virtual void DoCheckShowMenu(TreeListHitTest ht) {
				TreeListMenu menu = null;
				if(IsHitTestNodeArea(ht.HitInfoType) || ht.HitInfoType == HitInfoType.Button || ht.HitInfoType == HitInfoType.NodeCheckBox) {
					menu = new TreeListNodeMenu(TreeList);
					menu.Init(ht.Node);
				}
				if(TreeList.OptionsMenu.EnableColumnMenu) {
					if(ht.HitInfoType == HitInfoType.BehindColumn || ht.HitInfoType == HitInfoType.Column) {
						menu = new TreeListColumnMenu(TreeList);
						menu.Init(ht.Column);
					}
					if(ht.HitInfoType == HitInfoType.Band && ht.Band != null) {
						menu = new TreeListBandMenu(TreeList);
						menu.Init(ht.Band);
					}
				}
				if(TreeList.OptionsMenu.EnableFooterMenu && ht.FooterItem != null) {
					menu = new TreeListFooterMenu(TreeList);
					menu.Init(ht);
				}
				ShowTreeListMenu(menu, ht.MousePoint);
			}
			#endregion
		}
		public class RegularState : NormalState {
			public RegularState(TreeListHandler handler)
				: base(handler) {
			}
			public override void MouseMove(MouseEventArgs e, TreeListHitTest ht) {
				TreeList.ScrollInfo.OnAction(DevExpress.XtraEditors.ScrollNotifyAction.MouseMove);
				base.MouseMove(e, ht);
			}
			public override void MouseWheel(MouseEventArgs e) {
				DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
				base.MouseWheel(e);
				if(ee.Handled) return;
				MouseWheelScrollClientArgs clientArgs = (MouseWheelScrollClientArgs)ee;
				if(clientArgs.Horizontal) {
					int distance = clientArgs.InPixels ? clientArgs.Distance : DefaultMouseWheelHorizontalScrollDelta * (clientArgs.Distance < 0 ? -1 : 1);
					TreeList.LeftCoord += distance;
					return;
				}
				if(TreeList.IsPixelScrolling) {
					int scrollPixels = Math.Min(ViewInfo.ViewRects.Rows.Height, TreeListViewInfo.DefaultMouseWheelScrollPixels);
					scrollPixels = (clientArgs.Delta > 0 ? -scrollPixels : scrollPixels);
					if(scrollPixels + TreeList.TopVisibleNodePixel + ViewInfo.ViewRects.Rows.Height > ViewInfo.CalcTotalScrollableRowsHeight())
						scrollPixels = Math.Max(0, ViewInfo.CalcTotalScrollableRowsHeight() - TreeList.TopVisibleNodePixel - ViewInfo.ViewRects.Rows.Height);
					if(TreeList.AllowAnimatedScrolling)
						Handler.AnimateScroll(scrollPixels);
					else
						TreeList.TopVisibleNodePixel += scrollPixels;
				}
				else {
					int scrollLines = SystemInformation.MouseWheelScrollLines;
					if(scrollLines == -1) scrollLines = TreeList.ViewInfo.VisibleRowCount;
					TreeList.TopVisibleNodeIndex += (clientArgs.Delta > 0 ? -scrollLines : scrollLines);
				}
			}
			public override void KeyPress(KeyPressEventArgs e) {
				if(char.IsControl(e.KeyChar) || IsExpandCollapseAction(e) || IsCheckBoxPressedAction(e)) return;
				if(e.KeyChar == '*' && TreeList.FocusedNode != null && !TreeList.FocusedNode.Expanded && TreeList.FocusedNode.HasChildren) {
					TreeList.FocusedNode.ExpandAll();
					return;
				}
				if(!isUnselectRow) {
					TreeList.ShowEditor();
					if(TreeList.ActiveEditor != null) {
						TreeList.ActiveEditor.SendKey(TreeList.lastKeyboardMessage, e);
					}
				}
				isUnselectRow = false;
			}
			bool isUnselectRow = false;
			protected virtual bool IsSpaceKeyHandledByLookUp() {
				if(!TreeList.IsLookUpMode || TreeList.LookUpOwner.OwnerEdit == null) return false;
				return !string.IsNullOrEmpty(TreeList.LookUpOwner.OwnerEdit.AutoSearchText);
			}
			protected override void KeyDownCore(KeyEventArgs e) {
				if(TreeList.OptionsSelection.MultiSelect) {
					if(e.KeyCode == Keys.A && e.Control) {
						TreeList.SelectAll();
						return;
					}
					if(e.KeyCode == Keys.Space && e.Control && TreeList.FocusedNode != null) {
						if(TreeList.IsCellSelect)
							InvertSelectionCore(TreeList.FocusedNode, false);
						else
							TreeList.FocusedNode.Selected = (TreeList.FocusedNode.Selected == false) ? true : false;
						isUnselectRow = true;
						return;
					}
				}
				if(e.KeyData == (Keys.C | Keys.Control) || e.KeyData == (Keys.Insert | Keys.Control)) {
					if(TreeList.OptionsClipboard.AllowCopy != DefaultBoolean.False)
						TreeList.CopyToClipboard();
					return;
				}
				if(e.KeyData == Keys.Enter && CanMoveByEnter) {
					TreeList.MoveFocusedCell(1);
					return;
				}
				if(e.KeyData == Keys.Space && ShowCheckBoxes && !IsSpaceKeyHandledByLookUp()) {
					OnCheckNode(TreeList.FocusedNode);
					return;
				}
				if(e.KeyData == (Keys.F | Keys.Control)) {
					if(((ISearchControlClient)TreeList).IsAttachedToSearchControl) {
						TreeList.SetFocusSearchControl();
						return;
					}
					if(TreeList.OptionsFind.AllowFindPanel) {
						TreeList.ShowFindPanel();
						return;
					}
				}
				if(TreeList.FocusedNode != null && TreeList.FocusedColumn != null) {
					RepositoryItem repositoryItem = TreeList.InternalGetCustomNodeCellEdit(TreeList.FocusedColumn, TreeList.FocusedNode);
					if(repositoryItem != null && repositoryItem.IsActivateKey(e.KeyData)) {
						ShowEditorByKey(e);
						return;
					}
				}
				switch(e.KeyData) {
					case Keys.Enter:
					case Keys.ProcessKey:
					case Keys.F2:
						TreeList.ShowEditor();
						break;
					case Keys.Escape:
						TreeList.RejectCurrentChanges();
						break;
					default:
						if(Data.AnchorNode != null && Data.AnchorNode.TreeList == null) {
							Data.AnchorNode = TreeList.FocusedNode;
						}
						NavigateControl(e);
						if(TreeList.FocusedColumn != null && TreeList.CanIncrementalSearch(TreeList.FocusedColumn)) {
							if((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) ||
								(e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9) || (e.KeyCode >= Keys.OemSemicolon && e.KeyCode <= Keys.OemBackslash)) {
								this.SetState(TreeListState.IncrementalSearch);
								return;
							}
						}
						break;
				}
			}
			protected void ShowEditorByKey(KeyEventArgs e) {
				TreeList.ShowEditor();
				if(TreeList.ActiveEditor != null) {
					DevExpress.XtraEditors.BaseEdit be = TreeList.ActiveEditor as DevExpress.XtraEditors.BaseEdit;
					if(be != null) {
						if(e.KeyCode != Keys.Enter)
							be.SendKey(e);
					}
				}
			}
			protected virtual bool IsExpandCollapseAction(KeyPressEventArgs e) {
				return (isExpandCollapse && (e.KeyChar == '-' || e.KeyChar == '+'));
			}
			protected virtual bool IsCheckBoxPressedAction(KeyPressEventArgs e) {
				return ShowCheckBoxes && e.KeyChar == 32; 
			}
			protected bool ShowCheckBoxes { get { return TreeList.OptionsView.ShowCheckBoxes; } }
			protected override bool CanMoveByEnter { get { return (base.CanMoveByEnter && !TreeList.Editable); } }
			public override TreeListState State { get { return TreeListState.Regular; } }
		}
		public class EditingState : NormalState {
			public EditingState(TreeListHandler handler) : base(handler) { }
			protected override void OnDispose() {
				Handler.BeginLockSetState();
				try { TreeList.CloseEditor(); }
				finally { Handler.EndLockSetState(); }
			}
			public override bool ProcessChildControlKey(KeyEventArgs e) {
				if(TreeList.ActiveEditor != null && TreeList.ActiveEditor.IsNeededKey(e)) return false;
				return base.ProcessChildControlKey(e);
			}
			protected override void KeyDownCore(KeyEventArgs e) {
				switch(e.KeyData) {
					case Keys.Enter:
					case Keys.ProcessKey:
						TreeList.CloseEditor();
						if(e.KeyData == Keys.Enter && CanMoveByEnter) {
							TreeList.MoveFocusedCell(1);
							TreeList.ShowEditor();
						}
						break;
					case Keys.Escape:
						TreeList.HideEditor();
						SetState(Regular);
						TreeList.RefreshRowsInfo();
						break;
					default:
						NavigateControl(e);
						break;
				}
			}
			protected override void NavigateControl(KeyEventArgs e) {
				if(!TreeList.OptionsNavigation.MoveOnEdit) {
					if(e.KeyCode == Keys.Left || e.KeyCode == Keys.Right) {
						if(TreeList.ActiveEditor != null)
							TreeList.ActiveEditor.DeselectAll();
					}
					return;
				}
				if(e.KeyCode == Keys.Left && TreeList.FocusedCellIndex == 0) return;
				if(e.KeyCode == Keys.Right && TreeList.FocusedCellIndex == TreeList.VisibleColumns.Count - 1) return;
				TreeList.CloseEditor();
				bool canMoveEditor = false;
				switch(e.KeyCode) {
					case Keys.Left:
					case Keys.Right:
						canMoveEditor = !e.Control;
						break;
					case Keys.Up:
					case Keys.Down:
					case Keys.PageUp:
					case Keys.PageDown:
					case Keys.Home:
					case Keys.End:
					case Keys.Tab:
						canMoveEditor = true;
						break;
				}
				if(canMoveEditor) {
					base.NavigateControl(e);
					Application.DoEvents();
					TreeList.ShowEditor();
				}
			}
			sealed public override TreeListState State { get { return TreeListState.Editing; } }
		}
		public class NodePressedState : TreeListControlState {
			protected TreeListNode oldFocusedNode;
			public NodePressedState(TreeListHandler handler) : base(handler) { }
			public override void Init() {
				RowInfo press_ri = Data.DownHitTest.RowInfo;
				if(press_ri == null) return; 
				TreeList.PressedNode = press_ri.Node;
				oldFocusedNode = TreeList.FocusedNode;
				int oldFocusedCell = TreeList.FocusedCellIndex;
				if(Data.DownHitTest.HitInfoType == HitInfoType.Cell) {
					TreeList.PresetFocusedNode(TreeList.PressedNode);
					TreeList.InvokeSelectionAction(() => { TreeList.FocusedCellIndex = TreeList.VisibleColumns.IndexOf(Data.DownHitTest.CellInfo.ColumnInfo.Column); });
					TreeList.PresetFocusedNode(oldFocusedNode);
				}
				ChangeSelection(press_ri, true);
				if(Data.DownHitTest.HitInfoType == HitInfoType.Cell && ControlState == TreeListState.NodePressed) {
					Data.CanImmediateEditor = (oldFocusedNode == TreeList.PressedNode && oldFocusedCell == TreeList.FocusedCellIndex);
					if(!TreeList.ShowEditorOnMouseUp) {
						if(TreeList.OptionsSelection.MultiSelect && TreeList.Selection.Count > 1) {
							if(IsShift || IsControl)
								return;
						}
						ActivateEditorByMouse(Data.DownHitTest);
						if(ControlState == TreeListState.Editing) CheckPressEditorButton(Data.DownHitTest);
					}
				}
			}
			protected override void OnDispose() {
				TreeList.PressedNode = null;
				Data.CanImmediateEditor = true;
			}
			public override void MouseMove(MouseEventArgs e, TreeListHitTest ht) {
				if(!e.Button.HasLeft() || Data.DownHitTest.RowInfo == null || Data.DownHitTest.RowInfo.Node == null ||
					!CanSetDragMode(e.X - Data.DownHitTest.MousePoint.X, e.Y - Data.DownHitTest.MousePoint.Y)) return;
				if(TreeList.OptionsDragAndDrop.DragNodesMode != DragNodesMode.None) {
					using(IDragNodesProvider provider = BeginDragDrop()) {
						if(provider.Count > 0) {
							Data.BeginLockLostCapture();
							try {
								DoDragDrop(provider);
							}
							finally {
								EndDragDrop(provider);
								Data.CancelLockLostCapture();
							}
							SetNormalState();
							return;
						}
					}
				}
				if(TreeList.IsCellSelect) {
					Point p = new Point(e.X, e.Y);
					RowInfo row = ViewInfo.GetNearestRow(p);
					TreeListColumn column = ViewInfo.GetNearestColumn(p);
					if(Data.DownHitTest.InRow && ((row != null && Data.DownHitTest.Node != row.Node) || Data.DownHitTest.Column != column)) {
						if(Data.DownHitTest.HitInfoType == HitInfoType.RowIndicator && !TreeList.OptionsSelection.UseIndicatorForSelection) return;
						SetState(TreeListState.CellSelection);
						return;
					}
				}
			}
			protected virtual IDragNodesProvider BeginDragDrop() {
				List<TreeListNode> nodes = new List<TreeListNode>();
				bool allowMultiDrag = TreeList.OptionsDragAndDrop.DragNodesMode == DragNodesMode.Multiple && TreeList.OptionsSelection.MultiSelect;
				if(allowMultiDrag)
					nodes.AddRange(TreeList.Selection);
				else
					nodes.Add(TreeList.PressedNode);
				BeforeDragNodeEventArgs e = new BeforeDragNodeEventArgs(nodes, TreeList.PressedNode);
				TreeList.RaiseBeforeDragNode(e);
				if(!e.CanDrag) e.Nodes.Clear();
				if(e.Nodes.Count > 0 && !allowMultiDrag) {
					e.Nodes.Clear();
					e.Nodes.Add(TreeList.PressedNode);
				}
				return new DragNodesProvider(TreeList, e.Nodes);
			}
			protected virtual void DoDragDrop(IDragNodesProvider provider) {
				DataObject dataObject = new DataObject();
				dataObject.SetData(typeof(IDragNodesProvider), provider);
				dataObject.SetData(typeof(TreeListNode), TreeList.PressedNode);
				TreeList.DoDragDrop(dataObject, DragDropEffects.All | DragDropEffects.Link);
			}
			protected virtual void EndDragDrop(IDragNodesProvider provider) {
				AfterDragNodeEventArgs e = new AfterDragNodeEventArgs(provider.DragNodes, provider.PressedNode);
				TreeList.RaiseAfterDragNode(e);
			}
			public override void MouseUp(MouseEventArgs e, TreeListHitTest ht) {
				if(!ht.IsEqual(Data.DownHitTest)) {
					SetState(Regular);
					return;
				}
				if(ht.HitInfoType == HitInfoType.SelectImage) {
					TreeList.InternalSelectImageClick(ht);
				}
				if(ht.HitInfoType == HitInfoType.StateImage) {
					TreeList.InternalStateImageClick(ht);
				}
				if(ht.HitInfoType == HitInfoType.Cell && TreeList.ShowEditorOnMouseUp) {
					if(TreeList.IsCellSelect && (!Data.CanImmediateEditor || IsControl || IsShift) && ht.RowInfo != ViewInfo.AutoFilterRowInfo) {
						SetState(Regular);
						return;
					}
					TreeList.PressedNode = null;
					ActivateEditorByMouse(ht);
					if(ControlState == TreeListState.Editing)
						return;
				}
				if(ht.Node != null && TreeList.IsLookUpMode)
					TreeList.LookUpOwner.ClosePopup();
				SetState(Regular);
			}
			public override void SelectionChanged() {
				TreeList.RefreshRowsInfo(false, TreeList.IsCellSelect);
				base.SelectionChanged();
			}
			private void ActivateEditorByMouse(TreeListHitTest ht) {
				if(ht.ColumnInfo.Column == null) return;
				if(!ht.ColumnInfo.Column.OptionsColumn.AllowFocus && !TreeListAutoFilterNode.IsAutoFilterNode(ht.Node)) return;
				if(TreeList.OptionsBehavior.ImmediateEditor || Data.CanImmediateEditor)
					TreeList.ShowEditor();
				if(ControlState == TreeListState.Editing) {
					if(!IsShift)
						Data.AnchorNode = TreeList.FocusedNode;
				}
			}
			private void CheckPressEditorButton(TreeListHitTest ht) {
				if(TreeList.ActiveEditor != null && ht.CellInfo != null) {
					TreeList.ContainerHelper.BeginLockFocus();
					try {
						TreeList.ActiveEditor.SendMouse(TreeList.ActiveEditor.PointToClient(Control.MousePosition), MouseButtons.Left);
					}
					finally {
						TreeList.ContainerHelper.EndLockFocus();
					}
				}
			}
			sealed public override TreeListState State { get { return TreeListState.NodePressed; } }
		}
		public class NodeSizingState : TreeListControlState {
			public NodeSizingState(TreeListHandler handler) : base(handler) { }
			public override void Init() {
				SetSizingLineToY(Data.DownHitTest.MouseDest.Bottom - 1, false);
			}
			protected override void OnDispose() {
				CheckSizingState();
				Data.SizingLineY = -1;
			}
			public override void MouseMove(MouseEventArgs e, TreeListHitTest ht) {
				SetSizingLineToY(e.Y, false);
			}
			public override void MouseUp(MouseEventArgs e, TreeListHitTest ht) {
				SetSizingLineToY(Data.SizingLineY, true);
				Data.SizingLineY = -1;
				int cy = (e.Y - Data.DownHitTest.MousePoint.Y) / ViewInfo.RowLineCount;
				if(cy != 0) {
					TreeList.RowHeight = ViewInfo.RowHeight + cy;
					FireChanged();
				}
				SetState(Regular);
			}
			private void CheckSizingState() {
				if(Data.SizingLineY != -1) SetSizingLineToY(Data.SizingLineY, true);
			}
			private void SetSizingLineToY(int mouseY, bool erase) {
				if(mouseY > ViewInfo.ViewRects.Client.Bottom)
					mouseY = ViewInfo.ViewRects.Client.Bottom;
				if(mouseY < ViewInfo.ViewRects.Rows.Top)
					mouseY = ViewInfo.ViewRects.Rows.Top;
				Point start, end;
				if(mouseY != Data.SizingLineY && Data.SizingLineY != -1) {
					start = TreeList.PointToScreen(new Point(ViewInfo.ViewRects.Rows.Left + ViewInfo.ViewRects.IndicatorWidth, Data.SizingLineY));
					end = TreeList.PointToScreen(new Point(ViewInfo.ViewRects.Client.Right, Data.SizingLineY));
					if(end.X > VisibleTLScreenRect.Right) end.X = VisibleTLScreenRect.Right;
					DevExpress.XtraEditors.Drawing.SplitterLineHelper.Default.DrawReversibleLine(TreeList.Handle, TreeList.PointToClient(start), TreeList.PointToClient(end));
				}
				if(mouseY != Data.SizingLineY || erase) {
					start = TreeList.PointToScreen(new Point(ViewInfo.ViewRects.Rows.Left + ViewInfo.ViewRects.IndicatorWidth, mouseY));
					end = TreeList.PointToScreen(new Point(ViewInfo.ViewRects.Client.Right, mouseY));
					if(end.X > VisibleTLScreenRect.Right) end.X = VisibleTLScreenRect.Right;
					DevExpress.XtraEditors.Drawing.SplitterLineHelper.Default.DrawReversibleLine(TreeList.Handle, TreeList.PointToClient(start), TreeList.PointToClient(end));
				}
				Data.SizingLineY = mouseY;
			}
			sealed public override TreeListState State { get { return TreeListState.NodeSizing; } }
		}
		public abstract class OLEDraggingState : TreeListControlState {
			public OLEDraggingState(TreeListHandler handler) : base(handler) { }
			public override void Init() {
				Data.DragInfo.scrollTimer.Elapsed += new ElapsedEventHandler(OnDragScrollTimer);
				Data.DragInfo.expandTimer.Tick += new EventHandler(OnDragExpandTimer);
				Data.DragInfo.expandTimer.Interval = TreeList.OptionsDragAndDrop.DragNodesExpandDelay;
			}
			protected override void OnDispose() {
				Data.DragInfo.scrollTimer.Elapsed -= new ElapsedEventHandler(OnDragScrollTimer);
				Data.DragInfo.expandTimer.Tick -= new EventHandler(OnDragExpandTimer);
				Data.DragInfo.expandTimer.Enabled = Data.DragInfo.scrollTimer.Enabled = false;
				ViewInfo.dragInfo = null;
				Data.DragMaster.CancelDrag();
				TreeList.PressedNode = null;
			}
			delegate void ScrollDelegate();
			private void ScrollProc() {
				int dx = Data.DragInfo.mouse.Y - ViewInfo.ViewRects.Rows.Top;
				if(dx < ViewInfo.ViewRects.Rows.Height / 2)
					TreeList.TopVisibleNodeIndex--;
				else
					TreeList.TopVisibleNodeIndex++;
			}
			protected virtual void OnDragScrollTimer(object sender, ElapsedEventArgs e) {
				if(Data.DragInfo.scrollLock == true) {
					Data.DragInfo.scrollLock = false;
					return;
				}
				if(TreeList.RowCount == ViewInfo.VisibleRowCount) return;
				ScrollDelegate d = new ScrollDelegate(ScrollProc);
				TreeList.BeginInvoke(d);
			}
			protected virtual void OnDragExpandTimer(object sender, EventArgs e) {
				TreeListNode overNode = Data.DragInfo.RowInfo == null ? null : Data.DragInfo.RowInfo.Node;
				if(overNode != null && overNode.HasChildren && TreeList.OptionsDragAndDrop.ExpandNodeOnDrag) {
					overNode.Expanded = true;
					Data.DragInfo.expandTimer.Enabled = false;
				}
			}
			public override void DragOver(DragEventArgs drgevent) {
				Data.RefreshDragArrow(GetDragInsertPosition(drgevent), (drgevent.KeyState & 8) != 0, drgevent);
				Point dragPoint = new Point(drgevent.X, drgevent.Y);
				TreeListHitTest hitTest = OnNodesDragging(drgevent);
				TreeListNode overNode = hitTest.Node;
				if(!Data.DragInfo.CustomDragDrop)
					drgevent.Effect = GetDragNodeEffect(GetDragData<IDragNodesProvider>(drgevent), overNode, drgevent.KeyState);
				DoDragCenteredImage(dragPoint, drgevent.Effect);
				Data.DragInfo.LastEffect = drgevent.Effect;
			}
			protected virtual void DoDragCenteredImage(Point leftSidePoint, DragDropEffects effect) {
				Data.DragMaster.DoDrag(new Point(leftSidePoint.X, leftSidePoint.Y), effect, false);
			}
			protected DragDropEffects GetDragNodeEffect(IDragNodesProvider dragNodesProvider, TreeListNode destNode, int keyState) {
				bool copy = (keyState & 8) != 0;
				if(dragNodesProvider == null) return DragDropEffects.None;
				if(destNode == null) return DragDropEffects.None;
				if(TreeListAutoFilterNode.IsAutoFilterNode(destNode)) return DragDropEffects.None;
				if(copy && TreeList.OptionsDragAndDrop.CanCloneNodesOnDrop) return DragDropEffects.Copy;
				return GetDragNodeEffectCore(dragNodesProvider, destNode, keyState);
			}
			protected virtual DragDropEffects GetDragNodeEffectCore(IDragNodesProvider dragNodesProvider, TreeListNode destNode, int keyState) {
				return DragDropEffects.Move;
			}
			protected virtual DragInsertPosition GetDragInsertPosition(DragEventArgs e) {
				RowInfo ri = Data.DragInfo.RowInfo;
				bool canInsert = (e.KeyState & 4) != 0;
				Point mousePoint = TreeList.PointToClient(new Point(e.X, e.Y));
				DragInsertPosition direction = DragInsertPosition.Before;
				if(TreeList.OptionsDragAndDrop.DropNodesMode != DropNodesMode.Standard) {
					if(Data.DownHitTest.RowInfo != null && mousePoint.Y >= Data.DownHitTest.RowInfo.Bounds.Bottom) direction = DragInsertPosition.After;
					if(ri != null && !canInsert) {
						int offset = ri.Bounds.Height / 3;
						if(direction == DragInsertPosition.After) {
							canInsert = (mousePoint.Y <= ri.Bounds.Bottom && mousePoint.Y >= ri.Bounds.Bottom - offset);
						}
						else {
							canInsert = (mousePoint.Y >= ri.Bounds.Top && mousePoint.Y <= ri.Bounds.Top + offset);
						}
					}
				}
				if(!canInsert) direction = DragInsertPosition.AsChild;
				return direction;
			}
			protected virtual TreeListHitTest OnNodesDragging(DragEventArgs drgevent) {
				Point pt = TreeList.PointToClient(new Point(drgevent.X, drgevent.Y));
				if(!Data.DragInfo.mouse.Equals(pt)) {
					Data.DragInfo.mouse = pt;
					Data.DragInfo.scrollLock = true;
				}
				TreeListHitTest ht = GetHitTest(Data.DragInfo.mouse);
				Data.RefreshDragArrow(ht.RowInfo, drgevent);
				if(ViewInfo.ViewRects.Rows.Contains(pt)) {
					int dy = 0;
					if(pt.Y < ViewInfo.ViewRects.Rows.Top + ViewInfo.RowHeight)
						dy = -1;
					else if(pt.Y > ViewInfo.ViewRects.Rows.Bottom - ViewInfo.RowHeight &&
						ViewInfo.ViewRects.EmptyRows.Height == 0)
						dy = 1;
					if(dy != 0) {
						Data.DragInfo.Go(DragScrollInterval);
					}
					else Data.DragInfo.Stop();
					if(ViewInfo.dragInfo == null) ViewInfo.dragInfo = Data.DragInfo;
				}
				else {
					HideArrow(false);
				}
				return ht;
			}
			public override void DragLeave() {
				Data.DragMaster.EndDrag();
				Data.DragInfo.Reset();
				ViewInfo.dragInfo = null;
				TreeList.ViewInfo.PaintAnimatedItems = false;
				TreeList.InvalidateDragArrow();
				SetState(Regular);
			}
			public override void DragDrop(DragEventArgs drgevent) {
				OnEndNodesDragging(drgevent);
				Data.DragInfo.Reset();
				ViewInfo.dragInfo = null;
				HideArrow(false);
				SetNormalState();
			}
			protected virtual void OnEndNodesDragging(DragEventArgs drgevent) {
				Data.DragMaster.EndDrag();
				TreeListNode destNode = GetDestNode(drgevent);
				IDragNodesProvider dragNodes = GetDragData<IDragNodesProvider>(drgevent);
				DragDropEffects eff = GetDragNodeEffect(dragNodes, destNode, drgevent.KeyState);
				bool drop = (eff != DragDropEffects.None);
				if(destNode == null || !drop) {
					Data.DragInfo.mouse = TreeList.PointToClient(new Point(drgevent.X, drgevent.Y));
					OnCancelNodesDragging(false);
					return;
				}				
				DropNodes(dragNodes, destNode, eff == DragDropEffects.Copy);
			}
			protected virtual TreeListNode GetDestNode(DragEventArgs drgevent) {
				Point hitPoint = TreeList.PointToClient(new Point(drgevent.X, drgevent.Y));
				TreeListHitTest ht = GetHitTest(hitPoint);
				TreeListNode destNode = ht.Node;
				if(drgevent.Effect == DragDropEffects.None || TreeListAutoFilterNode.IsAutoFilterNode(destNode)) return null;
				return destNode;
			}
			protected virtual int CalcDropIndex(TreeListNode dropNode, TreeListNode destNode, bool isCopy) {
				if(!Data.DragInfo.CanInsert) return -1;
				int index = destNode.owner.IndexOf(destNode);
				if(Data.DragInfo.dragInsertPosition == DragInsertPosition.After) {
					if(isCopy) return ++index;
					if(destNode.ParentNode != dropNode.ParentNode) ++index;
				}
				return index;
			}
			protected virtual void DropNodes(IDragNodesProvider provider, TreeListNode destNode, bool isCopy) {
				TreeList.BeginUpdate();
				TreeListNode dropNode = null;
				TreeListNode nodeTo = Data.DragInfo.CanInsert ? destNode.ParentNode : destNode;
				foreach(TreeListNode node in provider.DragNodes) {
					int index = CalcDropIndex(node, destNode, isCopy);
					TreeListNode resultNode = DropNode(node, nodeTo, index, isCopy);
					if(resultNode != null)
						dropNode = resultNode;
				}
				if(TreeList.OptionsDragAndDrop.ExpandNodeOnDrag)
					if(!Data.DragInfo.CanInsert)
						nodeTo.Expanded = true;
				SetDroppedNodeFocused(dropNode);
				TreeList.PressedNode = null;
				TreeList.CheckIncreaseVisibleRows();
				TreeList.EndUpdate();
			}
			protected virtual TreeListNode DropNode(TreeListNode dropNode, TreeListNode nodeTo, int dropIndex, bool isCopy) {
				bool success = false;
				BeforeDropNodeEventArgs args = RaiseBeforeDropNode(dropNode, nodeTo, isCopy);
				if(args.Cancel) return null;
				try {
					if(isCopy) {
						dropNode = TreeList.CopyNode(dropNode, nodeTo, true);
						success = dropNode != null;
					}
					else {
						TreeListNode newTop = null;
						if(dropNode == TreeList.TopVisibleNode)
							newTop = TreeListNodesIterator.GetNextVisibleParent(TreeList.TopVisibleNode);
						success = TreeList.MoveNode(dropNode, nodeTo);
						if(newTop != null && success) TreeList.SetTopVisibleNodeIndexCore(TreeList.GetVisibleIndexByNode(newTop));
					}
					if(dropIndex != -1) {
						TreeList.SetNodeIndex(dropNode, dropIndex);
						success = (dropIndex == TreeList.GetNodeIndex(dropNode) || TreeList.SortedColumnCount > 0);
					}
					return dropNode;
				}
				finally {
					if(success) {
						object parId = nodeTo == null ? TreeList.RootValue : nodeTo[TreeList.KeyFieldName];
						if(AutoChangeParent(dropNode, parId))
							ViewInfo.SummaryFooterInfo.NeedsRecalcAll = true;
					}
					RaiseAfterDropNode(dropNode, nodeTo, success);
				}
			}
			protected virtual bool AutoChangeParent(TreeListNode node, object parId) {
				if(TreeList.OptionsBehavior.AutoChangeParent && !TreeList.IsUnboundMode) {
					if(TreeList.IsValidColumnName(TreeList.ParentFieldName)) {
						TreeList.BeginLockListChanged();
						try {
							node[TreeList.ParentFieldName] = parId;
							TreeList.Data.EndDataRowEdit(node.Id); 
						}
						finally { TreeList.CancelLockListChanged(); }
						return true;
					}
				}
				return false;
			}
			protected void SetDroppedNodeFocused(TreeListNode node) {
				if(node == null) return;
				if(node.ParentNode != null && !node.ParentNode.Expanded)
					node = node.ParentNode;
				TreeList.FocusedRowIndex = TreeList.GetVisibleIndexByNode(node);
			}
			protected BeforeDropNodeEventArgs RaiseBeforeDropNode(TreeListNode dropNode, TreeListNode nodeTo, bool isCopy) {
				BeforeDropNodeEventArgs e = new BeforeDropNodeEventArgs(dropNode, nodeTo, isCopy);
				TreeList.RaiseBeforeDropNode(e);
				return e;
			}
			protected void RaiseAfterDropNode(TreeListNode dropNode, TreeListNode destNode, bool success) {
				AfterDropNodeEventArgs e = new AfterDropNodeEventArgs(dropNode, destNode, success);
				TreeList.RaiseAfterDropNode(e);
			}		 
			public override void QueryContinueDrag(QueryContinueDragEventArgs qcdevent) {
				if(qcdevent.EscapePressed) {
					qcdevent.Action = DragAction.Cancel;
					OnCancelNodesDragging(true);
					SetState(Regular);
				}
			}
			protected virtual void OnCancelNodesDragging(bool canceled) {
				HideArrow(true);
				if(TreeList.PressedNode != null && canceled) {
					TreeList.RaiseDragCancelNode();
				}
				TreeList.PressedNode = null;
			}
			protected virtual void HideArrow(bool endDrag) {
				ViewInfo.dragInfo = null;
				if(endDrag || !Data.DragInfo.CustomDragDrop)
					Data.DragInfo.Reset();
				TreeList.ViewInfo.PaintAnimatedItems = false;
				TreeList.InvalidateDragArrow();
			}
			protected virtual int DragScrollInterval { get { return 50; } }
			public override void DragEnter(DragEventArgs drgevent) {
				if(!Data.DragMaster.DragInProgress)
					OnStartNodesDragging(drgevent);
			}
			protected virtual void OnStartNodesDragging(DragEventArgs drgevent) {
				ViewInfo.dragInfo = Data.DragInfo;
				IDragNodesProvider dragNodesProvider = GetDragData<IDragNodesProvider>(drgevent);
				IDragRowsInfoProvider dragRowsProvider = dragNodesProvider as IDragRowsInfoProvider;
				if(dragRowsProvider != null) {
					Data.RefreshDragArrow(dragRowsProvider.PressedRowInfo, drgevent);
					Bitmap img = dragRowsProvider.GetDragPreview();
					if(img != null)
						Data.DragMaster.StartDrag(TreeList, img, new Point(drgevent.X, drgevent.Y), drgevent.Effect);
				}
				Data.DragInfo.LastEffect = drgevent.Effect;
			}
		}
		public class OuterDraggingState : OLEDraggingState {
			public OuterDraggingState(TreeListHandler handler) : base(handler) { }
			public override void DragEnter(DragEventArgs drgevent) {
				if(!CanDrag(drgevent))
					OnCancelNodesDragging(false);
				else
					base.DragEnter(drgevent);
			}
			protected override void DropNodes(IDragNodesProvider provider, TreeListNode destNode, bool isCopy) {
				provider.TreeList.BeginUpdate();
				base.DropNodes(provider, destNode, isCopy);
				provider.TreeList.PressedNode = null;
				provider.TreeList.CheckIncreaseVisibleRows();
				provider.TreeList.EndUpdate();
			}
			bool CanDrag(DragEventArgs drgevent) {
				IDragNodesProvider provider = GetDragData<IDragNodesProvider>(drgevent);
				if(provider == null) return false;
				if(!TreeList.OptionsDragAndDrop.AcceptOuterNodes) return false;
				if(TreeList.IsVirtualMode || provider.TreeList.IsVirtualMode) return false;
				if(provider.TreeList.OptionsDragAndDrop.DragNodesMode != TreeList.OptionsDragAndDrop.DragNodesMode) return false;
				return TreeList.OptionsDragAndDrop.DragNodesMode != DragNodesMode.None;
			}
			protected override void OnCancelNodesDragging(bool canceled) {
				base.OnCancelNodesDragging(canceled);
				SetState(TreeListState.Regular);
			}
			sealed public override TreeListState State { get { return TreeListState.OuterDragging; } }
		}
		public class NodeDraggingState : OLEDraggingState {
			public NodeDraggingState(TreeListHandler handler) : base(handler) { }
			protected override DragDropEffects GetDragNodeEffectCore(IDragNodesProvider dragNodesProvider, TreeListNode destNode, int keyState) {
				foreach(var node in dragNodesProvider.DragNodes) {
					if(node == destNode) return DragDropEffects.None;
					if(destNode.HasAsParent(node)) return DragDropEffects.None;
					if(!Data.DragInfo.CanInsert) {
						TreeListNodes destNodes = (destNode == null ? TreeList.Nodes : destNode.Nodes);
						if(node.owner == destNodes) return DragDropEffects.None;
					}
				}
				return base.GetDragNodeEffectCore(dragNodesProvider, destNode, keyState);
			}
			sealed public override TreeListState State { get { return TreeListState.NodeDragging; } }
		}
		public abstract class ColumnDraggingStateBase : TreeListControlState {
			public enum DropPosition { Left, Up, Right, Down }
			public enum ColumnAutoScrollDirection { None, Left, Right }
			public const int HideElementPosition = -100, CustomizationFormPosition = -101, EmptyElementPosition = 100000;
			public const int ScrollStartDelay = 500, ScrollDelay = 50, ScrollHorzDelta = 7;
			System.Windows.Forms.Timer autoScrollTimer;
			ColumnAutoScrollDirection scrollDirection;
			protected PositionInfo lastPosition = null;
			protected bool dropTargetHighlighted = false;
			public ColumnDraggingStateBase(TreeListHandler handler)
				: base(handler) {
				this.scrollDirection = ColumnAutoScrollDirection.None;
				this.autoScrollTimer = new System.Windows.Forms.Timer();
				this.autoScrollTimer.Tick += new EventHandler(OnScrollTimerTick);
			}
			protected Point GetCursorLocation(Size size, Point location) {
				return new Point(location.X - size.Width / 2, location.Y - size.Height / 2);
			}
			protected override void OnDispose() {
				AutoScrollDirection = ColumnAutoScrollDirection.None;
				this.autoScrollTimer.Tick -= new EventHandler(OnScrollTimerTick);
				this.autoScrollTimer.Dispose();
				if(TreeList.CustomizationForm != null)
					TreeList.CustomizationForm.PressedItem = null;
				Data.DragColumnRect = Rectangle.Empty;
				Data.DragMaster.CancelDrag();
				Arrows.Hide();
			}
			public override void MouseDown(MouseEventArgs e, TreeListHitTest ht) {
				if(!e.Button.IsLeft())
					SetState(Regular);
			}
			public override void MouseMove(MouseEventArgs e, TreeListHitTest ht) {
				Point p = new Point(e.X, e.Y);
				if(!e.Button.IsLeft())
					SetState(Regular);
				else
					DoColumnDragging(p, ht.HitInfoType);
			}
			public override void MouseUp(MouseEventArgs e, TreeListHitTest ht) {
				DoEndColumnDragging(new Point(e.X, e.Y), ht.HitInfoType);
			}
			protected ColumnAutoScrollDirection AutoScrollDirection {
				get { return scrollDirection; }
				set {
					if(AutoScrollDirection == value) return;
					scrollDirection = value;
					autoScrollTimer.Stop();
					if(AutoScrollDirection != ColumnAutoScrollDirection.None) {
						HideDragTargetHighlighting();
						autoScrollTimer.Interval = ScrollStartDelay;
						autoScrollTimer.Start();
					}
				}
			}
			protected virtual void HideDragTargetHighlighting() {
				this.dropTargetHighlighted = false;
				this.lastPosition = null;
			}
			protected virtual void OnScrollTimerTick(object sender, EventArgs e) {
				if(AutoScrollDirection == ColumnAutoScrollDirection.None) return;
				Point pt = TreeList.PointToClient(Cursor.Position);
				if(IsClientPoint(pt)) {
					AutoScrollDirection = ColumnAutoScrollDirection.None;
					TreeListHitInfo ht = TreeList.CalcHitInfo(pt);
					if(ht != null)
						DoColumnDragging(pt, ht.HitInfoType);
					return;
				}
				if(AutoScrollDirection == ColumnAutoScrollDirection.Left || AutoScrollDirection == ColumnAutoScrollDirection.Right) {
					if(TreeList.OptionsView.AutoWidth) return;
					int delta = Math.Max(ScrollHorzDelta, CalcAutoScrollDelta());
					TreeList.LeftCoord += AutoScrollDirection == ColumnAutoScrollDirection.Left ? -delta : delta;
				}
				autoScrollTimer.Interval = ScrollDelay;
			}
			protected bool IsClientPoint(Point pt) {
				Rectangle r = ViewInfo.ViewRects.Window;
				if(IsLeftScrollArea(pt) || IsRightScrollArea(pt)) return false;
				r.Inflate(-2, 0);
				return r.Contains(pt);
			}
			protected virtual bool IsLeftScrollArea(Point pt) {
				if(pt.X < ViewInfo.ViewRects.ColumnPanel.Left + 4 + Math.Max(ViewInfo.ViewRects.IndicatorWidth - 5, 0)) return TreeList.LeftCoord > 0;
				return false;
			}
			protected virtual bool IsRightScrollArea(Point pt) {
				if(pt.X > ViewInfo.ViewRects.Client.Right - 6) {
					return ViewInfo.ViewRects.ColumnTotalWidth - ViewInfo.ViewRects.IndicatorWidth > ViewInfo.ViewRects.ColumnPanelWidth;
				}
				return false;
			}
			protected int IncreasedColumnHeight { get { return 50; } }
			protected bool IsInCustomizationZone(Point pt) {
				return pt.Y > ViewInfo.ViewRects.ColumnPanel.Bottom + IncreasedColumnHeight || !IsClientPoint(pt);
			}
			protected virtual bool UseArrows { get { return DragArrowsHelper.IsAllow; } }
			DragArrowsHelper Arrows { get { return TreeList.CreateArrowsHelper(); } }
			protected void DrawReversibleFrame(Rectangle rect) {
				if(UseArrows) return;
				if(NativeVista.IsVista && !NativeVista.IsCompositionEnabled()) return;
				rect.Location = TreeList.PointToClient(rect.Location);
				DevExpress.XtraEditors.Drawing.SplitterLineHelper.Default.DrawReversibleFrame(TreeList.Handle, rect);
			}
			protected void DrawArrows(Rectangle rect, bool isVertical = true) {
				if(rect.IsEmpty) {
					Arrows.Hide();
					return;
				}
				Point p1 = rect.Location, p2 = new Point(rect.X, rect.Bottom);
				if(!isVertical) {
					rect.Y += rect.Height / 2;
					p1 = rect.Location;
					p2 = new Point(rect.Right, rect.Y);
				}
				Arrows.Show(isVertical, p1, p2);
			}
			protected virtual DropPosition CalcDropPosition(Rectangle bounds, Point p, bool allowUpDown) {
				if(bounds.Contains(p) || !allowUpDown) {
					int dx = bounds.Width / 2, dy = bounds.Height / 3;
					if(allowUpDown)
						if(p.Y > bounds.Bottom - dy) return DropPosition.Down;
					if(p.X < bounds.Left + dx) return DropPosition.Left;
					return DropPosition.Right;
				}
				return p.Y < bounds.Top ? DropPosition.Up : DropPosition.Down;
			}
			protected virtual DropPosition ConvertDropPositionRigthToLeft(DropPosition position) {
				if(!ViewInfo.IsRightToLeft) return position;
				if(position == DropPosition.Left) return DropPosition.Right;
				if(position == DropPosition.Right) return DropPosition.Left;
				return position;
			}
			protected virtual int CalcAutoScrollDelta() { return Math.Abs((ViewInfo.ViewRects.ColumnTotalWidth - ViewInfo.ViewRects.IndicatorWidth - ViewInfo.ViewRects.ColumnPanel.Width) / 20); }
			public virtual void DoColumnDragging(Point p, HitInfoType ht) {
				if(ht == HitInfoType.CustomizationForm) {
					AutoScrollDirection = ColumnAutoScrollDirection.None;
				}
				else {
					if(!IsClientPoint(p)) {
						if(IsLeftScrollArea(p))
							AutoScrollDirection = ColumnAutoScrollDirection.Left;
						else {
							if(IsRightScrollArea(p))
								AutoScrollDirection = ColumnAutoScrollDirection.Right;
						}
					}
					else {
						AutoScrollDirection = ColumnAutoScrollDirection.None;
					}
				}
				DragDropEffects effect = DragDropEffects.Move;
				if(AutoScrollDirection == ColumnAutoScrollDirection.None) {
					PositionInfo pi = CalcColumnPositionInfo(p, ht);
					TreeList.RaiseDragObjectOver(new DragObjectOverEventArgs(DragColumn, pi));
					if(dropTargetHighlighted && !UseArrows) DrawReversibleFrame(Data.DragColumnRect);
					Rectangle rect = CalcDragColumnRect(pi);
					if(pi.Valid) {
						DrawReversibleFrame(rect);
						UpdateDragFrame(rect, pi.IsHorizontal);
						dropTargetHighlighted = true;
					}
					else {
						dropTargetHighlighted = false;
						UpdateDragFrame(rect, pi.IsHorizontal);
					}
					Data.DragColumnRect = rect;
					lastPosition = pi;
					if(pi.Valid)
						effect = GetDragEffect(ht, pi.Index, IsInCustomizationZone(p));
				}
				Data.DragMaster.DoDrag(GetCursorLocation(Data.DragMaster.DragSize, TreeList.PointToScreen(p)), effect, true);
			}
			protected virtual Rectangle CalcDragColumnRect(PositionInfo positionInfo) {
				Rectangle rect = positionInfo.Bounds;
				if(!rect.IsEmpty) {
					Rectangle visibleBounds = ColumnPanelBounds;
					visibleBounds.Width -= ViewInfo.ViewRects.IndicatorWidth;
					if(!ViewInfo.IsRightToLeft)
						visibleBounds.X += ViewInfo.ViewRects.IndicatorWidth;
					if(rect.X < visibleBounds.Left)
						rect.X = visibleBounds.Left;
					if(rect.X > visibleBounds.Right)
						rect.X = visibleBounds.Right - 1;
					if(rect.Right > visibleBounds.Right)
						rect.Width = visibleBounds.Right - rect.X;
					rect = TreeList.RectangleToScreen(rect);
				}
				return rect;
			}
			protected virtual PositionInfo GetCustomizationFormPosition() {
				PositionInfo pi = new PositionInfo();
				pi.SetIndex(CustomizationFormPosition);
				pi.Valid = true;
				return pi;
			}
			protected virtual PositionInfo GetEmptyElementPosition() {
				PositionInfo pi = new PositionInfo();
				pi.SetIndex(0);
				pi.Valid = true;
				return pi;
			}
			protected virtual PositionInfo GetHideElementPosition() {
				PositionInfo pi = new PositionInfo();
				pi.SetIndex(HideElementPosition);
				pi.Valid = TreeList.OptionsCustomization.AllowQuickHideColumns;
				return pi;
			}
			protected virtual void UpdateDragFrame(Rectangle rect, bool isHorizontal) {
				if(UseArrows) {
					DrawArrows(rect, !isHorizontal);
					return;
				}
				ViewInfo.ColumnDragFrameRect = Rectangle.Empty;
				if(NativeVista.IsVista && !NativeVista.IsCompositionEnabled()) {
					if(!rect.IsEmpty) {
						rect.Location = TreeList.PointToClient(rect.Location);
						rect.Inflate(-1, -1);
					}
					ViewInfo.ColumnDragFrameRect = rect;
					ViewInfo.TreeList.Invalidate(ColumnPanelBounds);
				}
			}
			protected virtual PositionInfo CalcColumnPositionInfo(Point p, HitInfoType ht) {
				if(ht == HitInfoType.CustomizationForm)
					return GetCustomizationFormPosition();
				Rectangle r = new Rectangle(ColumnPanelBounds.Left + ViewInfo.ViewRects.IndicatorWidth, ColumnPanelBounds.Top,
				   ColumnPanelBounds.Width - ViewInfo.ViewRects.IndicatorWidth, ColumnPanelBounds.Height + IncreasedColumnHeight);
				if(!r.Contains(p) || !ViewInfo.ViewRects.Window.Contains(p))
					return GetHideElementPosition();
				if(!ColumnPanelBounds.Contains(p))
					p.Y = ColumnPanelBounds.Bottom - 1;
				using(DragHeaderObjectInfoArgs dragInfoArgs = new DragHeaderObjectInfoArgs(p, ht, r)) {
					IHeaderObjectInfo dropHeaderInfo = GetDropColumnObjectInfo(dragInfoArgs);
					return CalcColumnPositionInfoCore(dragInfoArgs, dropHeaderInfo);
				}
			}
			protected abstract Rectangle ColumnPanelBounds { get; }
			protected abstract Rectangle CalcColumnPositionInfoBounds(PositionInfo pi, Rectangle bounds, DropPosition position);
			protected abstract IHeaderObject DragColumn { get; }
			protected abstract PositionInfo CalcColumnPositionInfoCore(DragHeaderObjectInfoArgs dragInfoArgs, IHeaderObjectInfo dropHeaderInfo);
			protected abstract IHeaderObjectInfo GetDropColumnObjectInfo(DragHeaderObjectInfoArgs args);
			protected abstract DragDropEffects GetDragEffect(HitInfoType ht, int pos, bool customizationZone);
			public abstract void DoEndColumnDragging(Point p, HitInfoType ht);
		}
		public class DragHeaderObjectInfoArgs : IDisposable {
			Point hitPoint;
			HitInfoType hitInfoType;
			Rectangle dragBounds;
			IHeaderObjectInfo dropHeaderInfo;
			bool isDisposable;
			public DragHeaderObjectInfoArgs(Point p, HitInfoType ht, Rectangle dragBounds) {
				this.hitPoint = p;
				this.hitInfoType = ht;
				this.dragBounds = dragBounds;
			}
			public Point HitPoint { get { return hitPoint; } }
			public HitInfoType HitInfoType { get { return hitInfoType; } }
			public Rectangle DragBounds { get { return dragBounds; } }
			public IHeaderObjectInfo DropHeaderInfo { get { return dropHeaderInfo; } set { dropHeaderInfo = value; } }
			public void Dispose() {
				if(!isDisposable) {
					isDisposable = true;
					hitPoint = Point.Empty;
					dragBounds = Rectangle.Empty;
				}
			}
		}
		public class ColumnDraggingState : ColumnDraggingStateBase {
			public ColumnDraggingState(TreeListHandler handler)
				: base(handler) {
			}
			public override void Init() {
				Bitmap img = Data.GetColumnDragBitmap(Data.DownHitTest.ColumnInfo);
				Data.DragMaster.Alpha = (byte)(DevExpress.Utils.DragDrop.DragWindow.DefaultOpacity * 255);
				Data.DragMaster.StartDrag(TreeList, img, GetCursorLocation(img.Size, Cursor.Position), GetDragEffect(Data.DownHitTest.HitInfoType, Data.DownHitTest.Column.VisibleIndex, false));
			}
			protected override void OnDispose() {
				base.OnDispose();
				TreeList.PressedColumn = null;
			}
			public override void DoEndColumnDragging(Point p, HitInfoType ht) {
				AutoScrollDirection = ColumnAutoScrollDirection.None;
				Data.DragMaster.EndDrag();
				if(lastPosition != null) {
					if(ht == HitInfoType.CustomizationForm && lastPosition.Valid) {
						MoveDragColumnToCustomizationForm();
					}
					else {
						PositionInfo pos = new PositionInfo();
						pos.Assign(lastPosition);
						TreeList.RaiseDragObjectDrop(new DragObjectDropEventArgs(TreeList.PressedColumn, pos));
						if(lastPosition.Valid) {
							if(pos.Index >= 0)
								MoveDragColumn(lastPosition);
							else if(IsInCustomizationZone(p))
								MoveDragColumnToCustomizationForm();
						}
					}
				}
				TreeList.PressedColumn = null;
				if(lastPosition != null)
					UpdateDragFrame(Rectangle.Empty, lastPosition.IsHorizontal);
				SetState(Regular);
			}
			protected virtual void MoveDragColumnToCustomizationForm() {
				if(!TreeList.PressedColumn.Visible) return;
				if(TreeList.PressedColumn.OptionsColumn.AllowMoveToCustomizationForm) {
					TreeList.PressedColumn.Visible = false;
					TreeList.PressedColumn.FireChanged();
				}
			}
			protected virtual void MoveDragColumn(PositionInfo position) {
				TreeListColumn dragColumn = TreeList.PressedColumn;
				int index = position.Index;
				if(!TreeList.HasBands) {
					dragColumn.VisibleIndex = index;
				}
				else {
					dragColumn.Visible = true;
					if(dragColumn.ParentBand == position.Band)
						position.Band.Columns.SetColumnIndex(index, dragColumn);
					else
						position.Band.Columns.Insert(index, dragColumn);
				}
				dragColumn.FireChanged();
			}
			protected virtual PositionInfo CalcEmptyColumnPositionInfo(DragHeaderObjectInfoArgs dragInfoArgs, IHeaderObjectInfo dropHeaderInfo) {
				PositionInfo pi = GetEmptyElementPosition();
				pi.Bounds = CalcColumnPositionInfoBounds(pi, ColumnPanelBounds, ViewInfo.IsRightToLeft ? DropPosition.Right : DropPosition.Left);
				return pi;
			}
			protected override PositionInfo CalcColumnPositionInfoCore(DragHeaderObjectInfoArgs dragInfoArgs, IHeaderObjectInfo dropHeaderInfo) {
				ColumnInfo dropColumnInfo = dropHeaderInfo as ColumnInfo;
				if(dropColumnInfo == null)
					return CalcEmptyColumnPositionInfo(dragInfoArgs, dropHeaderInfo);
				PositionInfo pi = new PositionInfo();
				DropPosition position = CalcDropPosition(dropColumnInfo.Bounds, dragInfoArgs.HitPoint, AllowUpDown);
				position = CorrectColumnInfo(ref dropColumnInfo, position);
				int index = CalcDropIndex(dropColumnInfo, position);
				pi.SetIndex(index);
				if(index == -1)
					return PositionInfo.Empty;
				pi.Valid = true;
				SetRowAndColumnIndex(pi, dropColumnInfo.Column, position, dragInfoArgs.HitPoint);
				pi.Bounds = CalcColumnPositionInfoBounds(pi, dropColumnInfo.Bounds, position);
				if(!pi.Valid)
					return pi;
				return CorrectPositionInfo(pi, DragColumn as TreeListColumn, dropColumnInfo.Column);
			}
			protected override Rectangle CalcColumnPositionInfoBounds(PositionInfo pi, Rectangle bounds, DropPosition position) {
				if(bounds.IsEmpty) return bounds;
				Rectangle r = bounds;
				r.Width = r.Width / 2;
				r.X = r.X - 1;
				if(position == DropPosition.Right) {
					if(UseArrows) {
						r.Width = 1;
						r.X = bounds.Right - 1;
					}
					else {
						r.X = bounds.Right - r.Width;
					}
				}
				return r;
			}
			protected virtual PositionInfo CorrectPositionInfo(PositionInfo positionInfo, TreeListColumn dragColumn, TreeListColumn dropColumn) {
				if(positionInfo.Index > -1) {
					int index = dropColumn != null ? dropColumn.VisibleIndex : -1;
					if(index == -1) return PositionInfo.Empty;
					int dragIndex = dragColumn.VisibleIndex;
					if(dragIndex == positionInfo.Index || (dragIndex > -1 && dragIndex + 1 == positionInfo.Index))
						return PositionInfo.Empty;
					if(!CheckColumnOptions(dragColumn, positionInfo))
						return PositionInfo.Empty;
				}
				return positionInfo;
			}
			protected virtual int CalcDropIndex(ColumnInfo dropColumnInfo, DropPosition position) {
				ColumnsInfo columns = new ColumnsInfo();
				for(int n = 0; n < ViewInfo.ColumnsInfo.Columns.Count; n++) {
					ColumnInfo info = ViewInfo.ColumnsInfo[n];
					if(info.Column == null) continue;
					columns.Columns.Add(info);
				}
				if(!columns.Columns.Contains(dropColumnInfo)) return -1;
				if(dropColumnInfo.Column == null) return 0;
				int index = dropColumnInfo.Column.VisibleIndex;
				if(ViewInfo.IsRightToLeft)
					return position == DropPosition.Right ? index : index + 1;
				return position == DropPosition.Left ? index : index + 1;
			}
			protected virtual bool CheckColumnOptions(TreeListColumn drColumn, PositionInfo pi) {
				int index = pi.Index;
				if(index < 0) return true;
				if(!TreeList.OptionsCustomization.AllowChangeColumnParent) {
					if(drColumn.ParentBand == null || pi.Band == null) return true;
					if(drColumn.ParentBand != pi.Band) return false;
				}
				int fLeftIndex = ViewInfo.FixedLeftColumn == null ? -1 : ViewInfo.FixedLeftColumn.VisibleIndex,
					fRightIndex = ViewInfo.FixedRightColumn == null ? -1 : ViewInfo.FixedRightColumn.VisibleIndex;
				if(fLeftIndex == -1 && fRightIndex == -1) return true;
				return true;
			}
			protected override DragDropEffects GetDragEffect(HitInfoType ht, int pos, bool customizationZone) {
				if(ht == HitInfoType.CustomizationForm) return DragDropEffects.Move;
				if(pos >= 0) return DragDropEffects.Move;
				if(customizationZone) return DragDropEffects.None;
				return (TreeList.PressedColumn.VisibleIndex == -1 ? DragDropEffects.None : DragDropEffects.Move);
			}
			DropPosition CorrectFixedColumnInfo(ref ColumnInfo dropInfo, TreeListColumn rootBand, DropPosition position) {
				ColumnsInfo headersInfo = ViewInfo.ColumnsInfo;
				if(rootBand.Fixed == FixedStyle.Left) {
					dropInfo = headersInfo[ViewInfo.FindFixedLeftColumn()];
					return ConvertDropPositionRigthToLeft(DropPosition.Right);
				}
				dropInfo = headersInfo[ViewInfo.FixedRightColumn];
				return ConvertDropPositionRigthToLeft(DropPosition.Left);
			}
			protected virtual DropPosition CorrectColumnInfo(ref ColumnInfo dropInfo, DropPosition position) {
				if(DragColumn.Fixed == dropInfo.Column.Fixed) return position;
				ColumnsInfo headersInfo = ViewInfo.ColumnsInfo;
				if(DragColumn.Fixed != FixedStyle.None)
					return CorrectFixedColumnInfo(ref dropInfo, DragColumn as TreeListColumn, position);
				if(dropInfo.Column.Fixed != FixedStyle.None)
					return CorrectFixedColumnInfo(ref dropInfo, dropInfo.Column, position);
				return position;
			}
			protected IHeaderObjectInfo GetDropColumnObjectInfoCore(DragHeaderObjectInfoArgs args, ColumnsInfo headersInfo) {
				ColumnInfo ci = null;
				if(args.HitInfoType == HitInfoType.FixedRightDiv)
					ci = headersInfo[ViewInfo.FixedRightColumn];
				if(args.HitInfoType == HitInfoType.FixedLeftDiv)
					ci = headersInfo[ViewInfo.FindFixedLeftColumn()];
				if(ci == null)
					ci = ViewInfo.GetColumnInfoByPoint(args.HitPoint, 0, headersInfo);
				if(ci != null && ci.Column == null) {
					ci = headersInfo.LastColumnInfo;
					if(ci == null) return ci;
					if(ViewInfo.Direction * ci.Bounds.Left < ViewInfo.Direction * args.HitPoint.X) return ci;
					ci = headersInfo.FirstColumnInfo;
				}
				return ci;
			}
			protected override void HideDragTargetHighlighting() {
				UpdateDragFrame(Rectangle.Empty, false);
				base.HideDragTargetHighlighting();
			}
			protected override IHeaderObjectInfo GetDropColumnObjectInfo(DragHeaderObjectInfoArgs args) {
				return GetDropColumnObjectInfoCore(args, ViewInfo.ColumnsInfo);
			}
			protected virtual void SetRowAndColumnIndex(PositionInfo positionInfo, TreeListColumn dropColumn, DropPosition position, Point hitPoint) { }
			protected virtual bool AllowUpDown { get { return false; } }
			protected virtual bool AllowDragToSamePosition { get { return false; } }
			protected override IHeaderObject DragColumn { get { return TreeList.PressedColumn; } }
			protected override Rectangle ColumnPanelBounds { get { return ViewInfo.ViewRects.ColumnPanel; } }
			sealed public override TreeListState State { get { return TreeListState.ColumnDragging; } }
		}
		public class MultiRowColumnDraggingState : ColumnDraggingState {
			public MultiRowColumnDraggingState(TreeListHandler handler)
				: base(handler) {
			}
			protected override bool AllowDragToSamePosition {
				get {
					if(!TreeList.AllowBandColumnsMultiRow) return false;
					return true;
				}
			}
			protected override Rectangle CalcColumnPositionInfoBounds(PositionInfo pi, Rectangle bounds, ColumnDraggingStateBase.DropPosition position) {
				if(pi.Band == null || pi.Index == -1)
					return base.CalcColumnPositionInfoBounds(pi, bounds, position);
				if(position == DropPosition.Right || position == DropPosition.Left)
					return base.CalcColumnPositionInfoBounds(pi, bounds, position);
				Rectangle rowRect =
					GetColumnRowRectangle(pi.Band, (position == DropPosition.Up ? pi.RowIndex : pi.RowIndex - 1), pi.ColumnIndex);
				if(rowRect.IsEmpty) {
					pi.Valid = false;
					return Rectangle.Empty;
				}
				pi.IsHorizontal = true;
				if(position == DropPosition.Down) rowRect.Y = rowRect.Bottom - ViewInfo.ColumnPanelHeight / 2;
				else rowRect.Y = rowRect.Y - ViewInfo.ColumnPanelHeight / 2;
				rowRect.Height = ViewInfo.ColumnPanelHeight;
				return rowRect;
			}
			protected override int CalcDropIndex(ColumnInfo dropColumnInfo, ColumnDraggingStateBase.DropPosition position) {
				if(dropColumnInfo.Column == null) return -1;
				BandInfo bi = dropColumnInfo.ParentBandInfo;
				if(bi == null) return -1;
				int index = bi.Band.Columns.IndexOf(dropColumnInfo.Column);
				if(index == -1) return -1;
				if(ViewInfo.IsRightToLeft)
					return position == DropPosition.Right ? index : index + 1;
				return position == DropPosition.Left ? index : index + 1;
			}
			protected override PositionInfo CorrectPositionInfo(PositionInfo positionInfo, TreeListColumn dragColumn, TreeListColumn dropColumn) {
				if(dragColumn.ParentBand == positionInfo.Band && dragColumn.Visible) {
					int curIndex = positionInfo.Band.Columns.IndexOf(dragColumn);
					if(curIndex == positionInfo.Index || curIndex + 1 == positionInfo.Index) {
						if(!AllowDragToSamePosition)
							return PositionInfo.Empty;
					}
				}
				if(positionInfo.RowIndex == TreeList.PressedColumn.RowIndex && positionInfo.Band == TreeList.PressedColumn.ParentBand && positionInfo.ColumnIndex > -1) {
					int colIndex = TreeList.PressedColumn.ColumnIndex;
					if(positionInfo.ColumnIndex == colIndex || positionInfo.ColumnIndex == colIndex + 1) {
						if(!TreeList.PressedColumn.Visible) {
							ColumnInfo ci = ViewInfo.ColumnsInfo[TreeList.PressedColumn];
							if(ci != null) positionInfo.Bounds = ci.Bounds;
							return positionInfo;
						}
						return PositionInfo.Empty;
					}
				}
				if(!CheckColumnOptions(dragColumn, positionInfo))
					return PositionInfo.Empty;
				return positionInfo;
			}
			protected override void SetRowAndColumnIndex(PositionInfo positionInfo, TreeListColumn dropColumn, DropPosition position, Point hitPoint) {
				if(dropColumn == null) return;
				positionInfo.SetBand(dropColumn.ParentBand);
				if(positionInfo.Band == null || positionInfo.Band.Columns.VisibleCount == 0) return;
				Rectangle rowRect;
				if(position == DropPosition.Right || position == DropPosition.Left) {
					int columnIndex = dropColumn.ColumnIndex;
					DropPosition correctPosition = ViewInfo.IsRightToLeft ? DropPosition.Right : DropPosition.Left;
					positionInfo.SetRowAndColumnIndex(dropColumn.RowIndex, position == correctPosition ? columnIndex : columnIndex + 1);
					return;
				}
				for(int n = 0; ; n++) {
					rowRect = GetColumnRowRectangle(positionInfo.Band, n, -1);
					if(rowRect.IsEmpty) {
						positionInfo.SetRowAndColumnIndex(n, -1);
						break;
					}
					if(!rowRect.Contains(hitPoint)) continue;
					if(position == DropPosition.Up) {
						positionInfo.SetRowAndColumnIndex(n, -1);
						break;
					}
					if(position == DropPosition.Down) {
						positionInfo.SetRowAndColumnIndex(n + 1, -1);
						break;
					}
				}
			}
			protected override PositionInfo CalcEmptyColumnPositionInfo(DragHeaderObjectInfoArgs dragInfoArgs, IHeaderObjectInfo dropHeaderInfo) {
				if(dropHeaderInfo == null) return GetEmptyElementPosition();
				BandInfo bi = dropHeaderInfo as BandInfo;
				if(bi == null) return base.CalcEmptyColumnPositionInfo(dragInfoArgs, dropHeaderInfo);
				Rectangle r = new Rectangle(bi.Bounds.Left, ViewInfo.ViewRects.ColumnPanel.Top, bi.Bounds.Width, ViewInfo.ViewRects.ColumnPanel.Height);
				PositionInfo pi = GetEmptyElementPosition();
				pi.Bounds = CalcColumnPositionInfoBounds(pi, r, ViewInfo.IsRightToLeft ? DropPosition.Right : DropPosition.Left);
				pi.SetBand(bi.Band);
				return pi;
			}
			protected override PositionInfo CalcColumnPositionInfoCore(DragHeaderObjectInfoArgs dragInfoArgs, IHeaderObjectInfo dropHeaderInfo) {
				if(TreeList.ActualShowBands && dragInfoArgs.HitPoint.Y < ViewInfo.ViewRects.BandPanel.Top)
					return GetHideElementPosition();
				BandInfo bi = dropHeaderInfo as BandInfo;
				ColumnInfo dropColumnInfo = dropHeaderInfo as ColumnInfo;
				if(dropColumnInfo != null)
					bi = dropColumnInfo.ParentBandInfo;
				if(bi == null || bi.Columns.Columns.Count > 0)
					return base.CalcColumnPositionInfoCore(dragInfoArgs, dropHeaderInfo);
				PositionInfo pos = CalcEmptyColumnPositionInfo(dragInfoArgs, bi);
				return CorrectPositionInfo(pos, DragColumn as TreeListColumn, dropColumnInfo.Column);
			}
			protected override IHeaderObjectInfo GetDropColumnObjectInfo(DragHeaderObjectInfoArgs args) {
				BandInfo bi = ViewInfo.CalcBandHitInfo(args.HitPoint);
				if(bi == null || bi.Band == null || bi.Band.HasChildren) return null;
				IHeaderObjectInfo headerInfo = GetDropColumnObjectInfoCore(args, bi.Columns);
				if(headerInfo == null)
					return bi;
				return headerInfo;
			}
			protected virtual ColumnsInfo GetRowColumnsInfo(TreeListBand band, int rowIndex) {
				ColumnsInfo info = new ColumnsInfo();
				BandInfo bi = ViewInfo.BandsInfo.FindBand(band);
				if(bi == null) return info;
				for(int n = 0; n < bi.Columns.Columns.Count; n++) {
					ColumnInfo columnInfo = bi.Columns[n];
					if(columnInfo.Column == null) continue;
					TreeListColumn col = columnInfo.Column;
					if(col.RowIndex == rowIndex) {
						info.Columns.Add(columnInfo);
					}
				}
				return info;
			}
			protected virtual Rectangle GetColumnRowRectangle(TreeListBand band, int rowIndex, int columnIndex) {
				BandInfo bi = ViewInfo.BandsInfo.FindBand(band);
				if(bi == null)
					return Rectangle.Empty;
				Rectangle rect = bi.Bounds;
				rect.Y = ViewInfo.ViewRects.ColumnPanel.Top;
				int currentTop, currentBottom, currentLeft, currentRight;
				currentTop = currentBottom = currentLeft = currentRight = int.MinValue;
				for(int n = 0; n < bi.Columns.Columns.Count; n++) {
					ColumnInfo columnInfo = bi.Columns[n];
					if(columnInfo.Column == null) continue;
					TreeListColumn column = columnInfo.Column;
					if(column.RowIndex == rowIndex) {
						currentTop = columnInfo.Bounds.Top;
						currentBottom = Math.Max(currentBottom, columnInfo.Bounds.Bottom);
						if(column.ColumnIndex == columnIndex) {
							currentLeft = columnInfo.Bounds.Left;
							currentRight = columnInfo.Bounds.Right;
						}
					}
				}
				if(currentTop != int.MinValue) {
					rect.Y = currentTop;
					rect.Height = currentBottom - currentTop;
					if(columnIndex != -1) {
						if(currentLeft == int.MinValue)
							return Rectangle.Empty;
						rect.X = currentLeft;
						rect.Width = currentRight - currentLeft;
					}
					return rect;
				}
				return Rectangle.Empty;
			}
			public override void DoEndColumnDragging(Point p, HitInfoType ht) {
				AutoScrollDirection = ColumnAutoScrollDirection.None;
				Data.DragMaster.EndDrag();
				if(lastPosition != null) {
					if(ht == HitInfoType.CustomizationForm && lastPosition.Valid) {
						MoveDragColumnToCustomizationForm();
					}
					else {
						PositionInfo pos = new PositionInfo();
						pos.Assign(lastPosition);
						TreeList.RaiseDragObjectDrop(new DragObjectDropEventArgs(TreeList.PressedColumn, pos));
						if(lastPosition.Valid) {
							if(pos.Index >= 0) {
								TreeListColumn dragColumn = TreeList.PressedColumn;
								if(pos.Band == dragColumn.ParentBand) {
									TreeList.SetColumnPosition(dragColumn, pos.RowIndex, pos.ColumnIndex);
								}
								else {
									TreeList.BeginUpdate();
									try {
										pos.Band.Columns.Add(dragColumn);
										TreeList.SetColumnPosition(dragColumn, pos.RowIndex, pos.ColumnIndex);
									}
									finally {
										TreeList.EndUpdate();
										TreeList.InvalidatePixelScrollingInfo();
									}
								}
							}
							else if(IsInCustomizationZone(p))
								MoveDragColumnToCustomizationForm();
						}
					}
				}
				TreeList.PressedColumn = null;
				if(lastPosition != null)
					UpdateDragFrame(Rectangle.Empty, lastPosition.IsHorizontal);
				SetState(Regular);
			}
			protected override bool AllowUpDown {
				get {
					if(!TreeList.AllowBandColumnsMultiRow) return false;
					return true;
				}
			}
		}
		public class ColumnPressedState : TreeListControlState {
			public ColumnPressedState(TreeListHandler handler) : base(handler) { }
			public override void Init() {
				if(Data.DownHitTest != null && Data.DownHitTest.ColumnInfo != null)
					TreeList.PressedColumn = Data.DownHitTest.ColumnInfo.Column;
			}
			protected override void OnDispose() {
				TreeList.PressedColumn = null;
			}
			public override void MouseMove(MouseEventArgs e, TreeListHitTest ht) {
				if(e.Button.HasLeft() && Data.DownHitTest != null && Data.DownHitTest.ColumnInfo != null && Data.DownHitTest.ColumnInfo.Column != null) {
					if(TreeList.CanMoveColumn(Data.DownHitTest.ColumnInfo.Column) && CanSetDragMode(e.X - Data.DownHitTest.MousePoint.X, e.Y - Data.DownHitTest.MousePoint.Y)) {
						TreeListColumn pressCol = TreeList.PressedColumn;
						if(TreeList.RaiseDragObjectStart(new DragObjectStartEventArgs(pressCol))) {
							SetState(TreeListState.ColumnDragging);
							if(TreeList.CustomizationForm != null) {
								TreeListBandCustomizationForm bandCustomizationForm = TreeList.CustomizationForm as TreeListBandCustomizationForm;
								if(bandCustomizationForm != null)
									bandCustomizationForm.ShowColumnsPage();
							}
						}
						TreeList.PressedColumn = pressCol;
					}
				}
			}
			public override void MouseUp(MouseEventArgs e, TreeListHitTest ht) {
				if(ht.HitInfoType == HitInfoType.Column && TreeList.PressedColumn != null && TreeList.PressedColumn == ht.ColumnInfo.Column) {
					if(TreeList.IsDesignMode)
						Handler.SelectComponent(ht.Column);
					else
						SetSortOrder(TreeList.PressedColumn);
				}
				SetState(Regular);
			}
			protected virtual void SetSortOrder(TreeListColumn column) {
				if(!column.OptionsColumn.AllowSort) return;
				SortOrder newSort = SortOrder.None;
				TreeList.BeginUpdate();
				try {
					if(!IsControl) {
						switch(column.SortOrder) {
							case SortOrder.None:
							case SortOrder.Descending:
								newSort = SortOrder.Ascending;
								break;
							case SortOrder.Ascending:
								newSort = SortOrder.Descending;
								break;
						}
					}
					column.sortOrder = newSort;
					TreeList.RecalcSortColumns(column, !IsShift && !IsControl);
				}
				finally {
					TreeList.EndUpdate();
				}
			}
			sealed public override TreeListState State { get { return TreeListState.ColumnPressed; } }
		}
		public class ColumnSizingState : ColumnSizingStateBase {
			public ColumnSizingState(TreeListHandler handler) : base(handler) { }
			public override void Init() {
				TreeList.SizedColumn = Data.DownHitTest.ColumnInfo.Column;
				base.Init();
			}
			protected override void OnDispose() {
				TreeList.SizedColumn = null;
				base.OnDispose();
			}
			protected override void ResizeCore(TreeListHitTest ht, int delta) {
				if(!(ht.HitInfoType == HitInfoType.ColumnEdge && Data.DownHitTest.ColumnInfo == ht.ColumnInfo)) {
					int index = Data.DownHitTest.ColumnInfo.Column.VisibleIndex;
					TreeList.ResizeColumn(index, delta, ViewInfo.ViewRects.Rows.Right - Data.DownHitTest.MouseDest.Right);
					TreeList.Columns.FireChanged();
				}
			}
			protected override int CalcSizingLineY() {
				return ViewInfo.ViewRects.ColumnPanel.Bottom;
			}
			sealed public override TreeListState State { get { return TreeListState.ColumnSizing; } }
		}
		public abstract class ColumnButtonPressedStateBase : TreeListControlState {
			public ColumnButtonPressedStateBase(TreeListHandler handler) : base(handler) { }
			public override void Init() { PressColumnButton = true; }
			protected override void OnDispose() { PressColumnButton = false; }
			public override void MouseUp(MouseEventArgs e, TreeListHitTest ht) {
				PressColumnButton = false;
				SetState(Regular);
				TreeList.RaiseColumnButtonClick();
			}
			protected virtual bool PressColumnButton {
				set {
					Rectangle r = PressColumnButtonCore(value);
					if(!r.IsEmpty) {
						TreeList.ViewInfo.PaintAnimatedItems = false;
						TreeList.Invalidate(r);
					}
				}
			}
			protected abstract Rectangle PressColumnButtonCore(bool pressed);
		}
		public abstract class ColumnSizingStateBase : TreeListControlState {
			public ColumnSizingStateBase(TreeListHandler handler) : base(handler) { }
			public override void Init() {
				SetSizingLineToX(Data.DownHitTest.MousePoint.X, false);
			}
			protected override void OnDispose() {
				CheckSizingState();
				Data.SizingLineX = -1;
			}
			public override void MouseMove(MouseEventArgs e, TreeListHitTest ht) {
				SetSizingLineToX(e.X, false);
			}
			public override void MouseUp(MouseEventArgs e, TreeListHitTest ht) {
				SetSizingLineToX(Data.SizingLineX, true);
				Data.SizingLineX = -1;
				ResizeCore(ht, ViewInfo.Direction * (e.X - Data.DownHitTest.MousePoint.X));
				SetState(Regular);
			}
			protected abstract void ResizeCore(TreeListHitTest ht, int delta);
			protected abstract int CalcSizingLineY();
			protected void CheckSizingState() {
				if(Data.SizingLineX != -1) SetSizingLineToX(Data.SizingLineX, true);
			}
			protected virtual void SetSizingLineToX(int mouseX, bool erase) {
				if(mouseX > ViewInfo.ViewRects.Client.Right)
					mouseX = ViewInfo.ViewRects.Client.Right;
				if(mouseX < ViewInfo.ViewRects.Client.Left + ViewInfo.ViewRects.IndicatorWidth)
					mouseX = ViewInfo.ViewRects.Client.Left + ViewInfo.ViewRects.IndicatorWidth;
				Point start, end;
				if(mouseX != Data.SizingLineX && Data.SizingLineX != -1) {
					start = TreeList.PointToScreen(new Point(Data.SizingLineX, CalcSizingLineY()));
					end = TreeList.PointToScreen(new Point(Data.SizingLineX, ViewInfo.ViewRects.Client.Bottom));
					if(end.Y > VisibleTLScreenRect.Bottom) end.Y = VisibleTLScreenRect.Bottom;
					DevExpress.XtraEditors.Drawing.SplitterLineHelper.Default.DrawReversibleLine(TreeList.Handle, TreeList.PointToClient(start), TreeList.PointToClient(end));
				}
				if(mouseX != Data.SizingLineX || erase) {
					start = TreeList.PointToScreen(new Point(mouseX, CalcSizingLineY()));
					end = TreeList.PointToScreen(new Point(mouseX, ViewInfo.ViewRects.Client.Bottom));
					if(end.Y > VisibleTLScreenRect.Bottom) end.Y = VisibleTLScreenRect.Bottom;
					DevExpress.XtraEditors.Drawing.SplitterLineHelper.Default.DrawReversibleLine(TreeList.Handle, TreeList.PointToClient(start), TreeList.PointToClient(end));
				}
				Data.SizingLineX = mouseX;
			}
		}
		public class ColumnButtonPressedState : ColumnButtonPressedStateBase {
			public ColumnButtonPressedState(TreeListHandler handler) : base(handler) { }
			protected override Rectangle PressColumnButtonCore(bool pressed) {
				return ViewInfo.PressColumnButton(pressed);
			}
			sealed public override TreeListState State { get { return TreeListState.ColumnButtonPressed; } }
		}
		public class BandButtonPressedState : ColumnButtonPressedStateBase {
			public BandButtonPressedState(TreeListHandler handler) : base(handler) { }
			protected override Rectangle PressColumnButtonCore(bool pressed) {
				return ViewInfo.PressBandButton(pressed);
			}
			sealed public override TreeListState State { get { return TreeListState.BandButtonPressed; } }
		}
		public class BandPressedState : TreeListControlState {
			public BandPressedState(TreeListHandler handler) : base(handler) { }
			public override void Init() {
				if(Data.DownHitTest != null && Data.DownHitTest.BandInfo != null)
					TreeList.PressedBand = Data.DownHitTest.BandInfo.Band;
			}
			protected override void OnDispose() {
				TreeList.PressedBand = null;
			}
			public override void MouseMove(MouseEventArgs e, TreeListHitTest ht) {
				if(e.Button.HasLeft() && Data.DownHitTest != null && Data.DownHitTest.BandInfo != null && Data.DownHitTest.Band != null && TreeList.CanMoveBand(Data.DownHitTest.Band)) {
					if(CanSetDragMode(e.X - Data.DownHitTest.MousePoint.X, e.Y - Data.DownHitTest.MousePoint.Y)) {
						TreeListBand pressedBand = TreeList.PressedBand;
						if(TreeList.RaiseDragObjectStart(new DragObjectStartEventArgs(pressedBand))) {
							SetState(TreeListState.BandDragging);
							if(TreeList.CustomizationForm != null) {
								TreeListBandCustomizationForm bandCustomizationForm = TreeList.CustomizationForm as TreeListBandCustomizationForm;
								if(bandCustomizationForm != null)
									bandCustomizationForm.ShowBandsPage();
							}
						}
						TreeList.PressedBand = pressedBand;
					}
				}
			}
			public override void MouseUp(MouseEventArgs e, TreeListHitTest ht) {
				if(TreeList.IsDesignMode && ht.HitInfoType == HitInfoType.Band && TreeList.PressedBand != null && TreeList.PressedBand == ht.BandInfo.Band)
					Handler.SelectComponent(TreeList.PressedBand);
				SetState(Regular);
			}
			sealed public override TreeListState State { get { return TreeListState.BandPressed; } }
		}
		public class BandDraggingState : ColumnDraggingStateBase {
			public BandDraggingState(TreeListHandler handler)
				: base(handler) { }
			public override void Init() {
				Data.DragMaster.Alpha = (byte)(DevExpress.Utils.DragDrop.DragWindow.DefaultOpacity * 255);
				Bitmap img = Data.GetBandDragBitmap(Data.DownHitTest.BandInfo);
				Data.DragMaster.StartDrag(TreeList, img, GetCursorLocation(img.Size, Cursor.Position), GetDragEffect(Data.DownHitTest.HitInfoType, Data.DownHitTest.Band.Index, false));
			}
			protected override DragDropEffects GetDragEffect(HitInfoType ht, int index, bool customizationZone) {
				if(customizationZone) return DragDropEffects.None;
				return DragDropEffects.Move;
			}
			protected override void OnDispose() {
				base.OnDispose();
				TreeList.PressedBand = null;
			}
			public override void DoEndColumnDragging(Point p, HitInfoType ht) {
				AutoScrollDirection = ColumnAutoScrollDirection.None;
				Data.DragMaster.EndDrag();
				TreeListBand dragBand = TreeList.PressedBand;
				if(lastPosition != null && lastPosition.Valid) {
					PositionInfo pos = new PositionInfo();
					pos.Assign(lastPosition);
					if(ht != HitInfoType.CustomizationForm)
						TreeList.RaiseDragObjectDrop(new DragObjectDropEventArgs(TreeList.PressedBand, pos));
					if(lastPosition.Index < 0) {
						if(ht == HitInfoType.CustomizationForm || lastPosition.TargetCollection == null) {
							TreeList.PressedBand.Visible = false;
						}
						else {
							lastPosition.TargetCollection.ReplaceParent(dragBand);
							dragBand.Visible = true;
						}
					}
					else {
						if(lastPosition.TargetCollection == dragBand.OwnedCollection)
							dragBand.OwnedCollection.SetBandIndex(lastPosition.Index, dragBand);
						else
							lastPosition.TargetCollection.Insert(lastPosition.Index, dragBand);
						dragBand.Visible = true;
					}
				}
				if(lastPosition != null)
					UpdateDragFrame(Rectangle.Empty, lastPosition.IsHorizontal);
				SetState(Regular);
			}
			protected override void HideDragTargetHighlighting() {
				UpdateDragFrame(Rectangle.Empty, false);
				base.HideDragTargetHighlighting();
			}
			protected virtual bool CheckBandOptions(TreeListBand dragBand, PositionInfo pi) {
				if(!pi.TargetCollection.CanAddBand(dragBand) && pi.TargetCollection != dragBand.OwnedCollection) return false;
				if(pi.TargetCollection == dragBand.OwnedCollection && dragBand.Visible) {
					int dragIndex = dragBand.Index;
					if(dragIndex == pi.Index || dragIndex + 1 == pi.Index) return false;
				}
				if(!TreeList.GetAllowChangeBandParent()) {
					if(dragBand.OwnedCollection == pi.TargetCollection && pi.Index >= 0) {
						if(dragBand.OwnedCollection != TreeList.Bands)
							return true;
					}
					else {
						return false;
					}
				}
				if(pi.Index < 0) return true;
				TreeListBand rootDragBand = dragBand.RootBand;
				if(rootDragBand == null) return true;
				TreeListBand rootTargetBand = pi.TargetCollection.OwnerBand == null ? null : pi.TargetCollection.OwnerBand.RootBand;
				if(rootTargetBand != null)
					return rootDragBand.Fixed == rootTargetBand.Fixed;
				int fLeftIndex = ViewInfo.FixedLeftBand == null ? -1 : ViewInfo.FixedLeftBand.Index,
					fRightIndex = ViewInfo.FixedRightBand == null ? -1 : ViewInfo.FixedRightBand.Index;
				if(fLeftIndex == -1 && fRightIndex == -1) return true;
				return true;
			}
			Rectangle GetVerticalPositionInfoBounds(Rectangle bounds, DropPosition position) {
				Rectangle r = bounds;
				r.Width = bounds.Width / 2;
				r.Height = ViewInfo.ViewRects.BandPanel.Bottom - r.Top;
				if(position == DropPosition.Right) {
					if(UseArrows) {
						r.X = bounds.Right - 1;
						r.Width = 1;
					}
					else
						r.X = bounds.Right - r.Width;
				}
				return r;
			}
			Rectangle GetHorizontalPositionInfoBounds(PositionInfo pi, BandInfo bi, Rectangle bounds) {
				if(bi == null) return bounds;
				int height = ViewInfo.BandPanelRowHeight;
				pi.IsHorizontal = true;
				return new Rectangle(bi.Bounds.Left, bi.Bounds.Bottom - height / 2, bi.Bounds.Width, height);
			}
			protected override Rectangle CalcColumnPositionInfoBounds(PositionInfo pi, Rectangle bounds, DropPosition position) {
				if(bounds.IsEmpty) return bounds;
				if(pi.TargetCollection.VisibleCount == 0 || pi.Index == -1) {
					if(pi.TargetCollection.OwnerBand == null)
						return GetVerticalPositionInfoBounds(bounds, position);
					BandInfo bi = ViewInfo.BandsInfo.FindBand(pi.TargetCollection.OwnerBand);
					return GetHorizontalPositionInfoBounds(pi, bi, bounds);
				}
				return GetVerticalPositionInfoBounds(bounds, position);
			}
			protected override PositionInfo CalcColumnPositionInfoCore(DragHeaderObjectInfoArgs dragInfoArgs, IHeaderObjectInfo dropHeaderInfo) {
				PositionInfo pi = new PositionInfo();
				BandInfo bi = dropHeaderInfo as BandInfo;
				if(bi == null) {
					pi.SetIndex(-1);
					return pi;
				}
				TreeListBand dragBand = DragColumn as TreeListBand;
				if(bi.Band == null) {
					if(TreeList.Bands.VisibleCount == 0 && (!dragBand.Visible || dragBand.TreeList == null))
						return GetEmptyElementPosition();
				}
				DropPosition position = CalcDropPosition(bi.Bounds, dragInfoArgs.HitPoint, true);
				position = CorrectBandInfo(ref bi, position);
				int index = CalcDropIndex(bi, position);
				if(index == int.MinValue) return pi;
				pi.SetIndex(index);
				pi.Valid = true;
				SetTargetCollection(bi, pi, position);
				if(!pi.Valid) return pi;
				if(!CheckBandOptions(dragBand, pi))
					return PositionInfo.Empty;
				pi.Bounds = CalcColumnPositionInfoBounds(pi, bi.Bounds, position);
				pi.Valid = !pi.Bounds.IsEmpty;
				return pi;
			}
			DropPosition CorrectFixedBandInfo(ref BandInfo dropInfo, TreeListBand rootBand, DropPosition position) {
				BandsInfo headersInfo = ViewInfo.BandsInfo;
				if(rootBand.Fixed == FixedStyle.Left) {
					if(ViewInfo.FixedLeftBand == null)
						return ConvertDropPositionRigthToLeft(DropPosition.Left);
					dropInfo = headersInfo[ViewInfo.FixedLeftBand];
					return ConvertDropPositionRigthToLeft(DropPosition.Right);
				}
				if(ViewInfo.FixedRightBand == null)
					return ConvertDropPositionRigthToLeft(DropPosition.Right);
				dropInfo = headersInfo[ViewInfo.FixedRightBand];
				return ConvertDropPositionRigthToLeft(DropPosition.Left);
			}
			protected virtual DropPosition CorrectBandInfo(ref BandInfo dropInfo, DropPosition position) {
				TreeListBand rootTargetBand = dropInfo.Band.RootBand;
				if(rootTargetBand == null) return position;
				TreeListBand rootDragBand = (DragColumn as TreeListBand).RootBand;
				if(rootDragBand == null) return position;
				BandsInfo headersInfo = ViewInfo.BandsInfo;
				if(rootTargetBand.Fixed == rootDragBand.Fixed) return position;
				if(rootDragBand.Fixed != FixedStyle.None)
					return CorrectFixedBandInfo(ref dropInfo, rootDragBand, position);
				if(rootTargetBand.Fixed != FixedStyle.None)
					return CorrectFixedBandInfo(ref dropInfo, rootTargetBand, position);
				return position;
			}
			protected virtual void SetTargetCollection(BandInfo dropBandInfo, PositionInfo positionInfo, DropPosition position) {
				TreeListBand destBand = dropBandInfo.Band;
				TreeListBandCollection targetCollection = position == DropPosition.Down ? destBand.Bands : destBand.OwnedCollection;
				positionInfo.SetTargetCollection(targetCollection);
			}
			protected virtual int CalcDropIndex(BandInfo dropColumnInfo, DropPosition position) {
				TreeListBand destBand = dropColumnInfo.Band;
				if(position == DropPosition.Up) return int.MinValue;
				if(position == DropPosition.Down) return destBand.Bands.VisibleCount > 0 ? -1 : 0;
				int index = dropColumnInfo.Band.Index;
				if(ViewInfo.IsRightToLeft)
					return position == DropPosition.Right ? index : index + 1;
				return position == DropPosition.Left ? index : index + 1;
			}
			protected override PositionInfo GetEmptyElementPosition() {
				PositionInfo pi = new PositionInfo();
				pi.SetIndex(0);
				pi.SetTargetCollection(TreeList.Bands);
				pi.Bounds = CalcColumnPositionInfoBounds(pi, ViewInfo.ViewRects.BandPanel, ViewInfo.IsRightToLeft ? DropPosition.Right : DropPosition.Left);
				pi.Valid = true;
				return pi;
			}
			protected override IHeaderObjectInfo GetDropColumnObjectInfo(DragHeaderObjectInfoArgs args) {
				return ViewInfo.CalcBandHitInfo(args.HitPoint);
			}
			protected override IHeaderObject DragColumn { get { return TreeList.PressedBand; } }
			protected override Rectangle ColumnPanelBounds { get { return ViewInfo.ViewRects.BandPanel; } }
			sealed public override TreeListState State { get { return TreeListState.BandDragging; } }
		}
		public class BandSizingState : ColumnSizingStateBase {
			public BandSizingState(TreeListHandler handler)
				: base(handler) {
			}
			public override void Init() {
				TreeList.SizedBand = Data.DownHitTest.BandInfo.Band;
				base.Init();
			}
			protected override void OnDispose() {
				TreeList.SizedBand = null;
				base.OnDispose();
			}
			protected override void ResizeCore(TreeListHitTest ht, int delta) {
				if(!(ht.HitInfoType == HitInfoType.BandEdge && Data.DownHitTest.BandInfo == ht.BandInfo)) {
					if(delta != 0) {
						TreeList.ResizeBand(Data.DownHitTest.Band, delta, ViewInfo.ViewRects.Rows.Right - Data.DownHitTest.MouseDest.Right);
						FireChanged();
					}
				}
			}
			protected override int CalcSizingLineY() {
				BandInfo bi = ViewInfo.BandsInfo.FindBand(TreeList.SizedBand);
				return bi == null ? ViewInfo.ViewRects.BandPanel.Y : bi.Bounds.Y;
			}
			sealed public override TreeListState State { get { return TreeListState.BandSizing; } }
		}
		public class DesignState : NormalState {
			public DesignState(TreeListHandler handler) : base(handler) { }
			protected override void KeyDownCore(KeyEventArgs e) { }
			protected override void OnPressNode() { }
			protected override void OnExpandNode(TreeListNode node) { }
			protected override bool CanFocusControl { get { return false; } }
			sealed public override TreeListState State { get { return TreeListState.Design; } }
		}
		public class MultiCellSelectionState : TreeListControlState {
			const int ScrollTimerInterval = 10;
			sealed public override TreeListState State { get { return TreeListState.CellSelection; } }
			System.Windows.Forms.Timer scrollTimer;
			public MultiCellSelectionState(TreeListHandler handler) : base(handler) {
				this.scrollTimer = new System.Windows.Forms.Timer();
				this.scrollTimer.Interval = ScrollTimerInterval;
				this.scrollTimer.Tick += OnScrollTimerTick;
			}
			protected virtual void OnScrollTimerTick(object sender, EventArgs e) {
				Rectangle r = ViewInfo.ViewRects.Rows;
				Point p = TreeList.PointToClient(Control.MousePosition);
				int vScrollDelta = 0, hScrollDelta = 0;
				if(p.Y < r.Top + 10) {
					vScrollDelta = -1;
				}
				if(p.Y > r.Bottom - 10) {
					vScrollDelta = 1;
				}
				if(!TreeList.OptionsView.AutoWidth) {
					if(p.X <= r.Left) hScrollDelta = -10;
					if(p.X >= r.Right) hScrollDelta = 10;
				}
				int prevIndex = TreeList.TopVisibleNodeIndex;
				int prevLeftCoord = TreeList.LeftCoord;
				TreeList.TopVisibleNodeIndex += vScrollDelta;
				TreeList.LeftCoord += hScrollDelta;
				if(!ViewInfo.IsValid) {
					ViewInfo.CalcViewInfo();
					TreeList.UpdateScrollBars();
				}
				if(prevIndex != TreeList.TopVisibleNodeIndex && ViewInfo.RowsInfo.Rows.Count > 0) {
					if(vScrollDelta < 0)
						UpdateSelection(ViewInfo.RowsInfo[0].Node, ViewInfo.GetNearestColumn(p), Data.DownHitTest.HitInfoType == HitInfoType.RowIndicator);
					else
						UpdateSelection(ViewInfo.RowsInfo[ViewInfo.RowsInfo.Rows.Count - 1].Node, ViewInfo.GetNearestColumn(p), Data.DownHitTest.HitInfoType == HitInfoType.RowIndicator);
				}
				if(prevLeftCoord != TreeList.LeftCoord && ViewInfo.RowsInfo.Rows.Count > 0 && TreeList.VisibleColumns.Count > 1) {
					RowInfo ri = ViewInfo.GetNearestRow(p);
					if(ri != null && ri.Node != null) {
						if(hScrollDelta < 0)
							UpdateSelection(ri.Node, TreeList.VisibleColumns[0], Data.DownHitTest.HitInfoType == HitInfoType.RowIndicator);
						else
							UpdateSelection(ri.Node, TreeList.VisibleColumns[TreeList.VisibleColumns.Count - 1], Data.DownHitTest.HitInfoType == HitInfoType.RowIndicator);
					}
				}
			}
			public override void Init() {
				base.Init();
				if(Data.SelectionAnchor == null || Data.SelectionAnchor.Node != TreeList.FocusedNode || Data.SelectionAnchor.Column != TreeList.FocusedColumn) {
					Data.SelectionAnchor = new TreeListCell(TreeList.FocusedNode, TreeList.FocusedColumn);
					Data.SelectionBounds = Rectangle.Empty;
				}
			}
			public override void MouseMove(MouseEventArgs e, TreeListHitTest ht) {
				Point p = new Point(e.X, e.Y);
				RowInfo ri = TreeList.ViewInfo.GetNearestRow(p);
				if(ri != null) 
					UpdateSelection(ri.Node, TreeList.ViewInfo.GetNearestColumn(p), Data.DownHitTest.HitInfoType == HitInfoType.RowIndicator);
			}
			protected virtual void UpdateSelection(TreeListNode node, TreeListColumn column, bool indicator) {
				if(node == null || column == null) return;
				TreeListCell selectionCell = new TreeListCell(node, column);
				if(Data.SelectionOldCell.Equals(selectionCell)) return;
				scrollTimer.Enabled = false;
				try {
					if(indicator) {
						if(!IsControl) TreeList.Selection.InternalClear();
						TreeList.SelectRange(Data.SelectionAnchor.Node, node);
					}
					else {
						SelectAnchorRange(Data.SelectionAnchor, selectionCell);
					}
					Data.SelectionOldCell = selectionCell;
				}
				finally {
					scrollTimer.Enabled = true;
				}
			}
			public override void MouseUp(MouseEventArgs e, TreeListHitTest ht) {
				scrollTimer.Enabled = false;
				SetState(Regular);
			}
			protected override void OnDispose() {
				scrollTimer.Dispose();
				scrollTimer = null;
			}
			public override void SelectionChanged() {
				TreeList.RefreshRowsInfo(false, true);
				base.SelectionChanged();
			}
		}
		public class MultiSelectionState : TreeListControlState {
			bool isModifierKeyPressed;
			public MultiSelectionState(TreeListHandler handler) : base(handler) { }
			sealed public override TreeListState State { get { return TreeListState.MultiSelection; } }
			public override void Init() {
				isModifierKeyPressed = IsControl || IsShift;
			}
			protected override void OnDispose() { }
			RowInfo GetNearestRow(Point pt) {
				RowInfo res = null;
				int minDelta = 10000;
				TreeListOperationAccumulateNodes nodes1 = new TreeListOperationAccumulateNodes();
				TreeList.NodesIterator.DoOperation(nodes1);
				foreach(TreeListNode node in nodes1.Nodes) {
					RowInfo row = TreeList.ViewInfo.RowsInfo[node];
					if(row == null) continue;
					if(row.Bounds.Contains(pt)) return row;
					int delta = Math.Abs(pt.Y - row.Bounds.Top);
					if(delta < minDelta) {
						res = row;
						minDelta = delta;
					}
				}
				return res;
			}
			public override void MouseMove(MouseEventArgs e, TreeListHitTest ht) {
				if(isModifierKeyPressed) return;
				Rectangle r = ViewInfo.ViewRects.Rows;
				Point p = TreeList.PointToClient(Control.MousePosition);
				if(p.Y < r.Top + 10) {
					TreeList.TopVisibleNodeIndex--;
				}
				if(p.Y > r.Bottom - 10) {
					TreeList.TopVisibleNodeIndex++;
				}
				RowInfo ri = GetNearestRow(new Point(e.X, e.Y));
				if(ri == null)
					return;
				SetSelection(TreeList.FocusedNode, ri.Node, false);
			}
			public override void MouseUp(MouseEventArgs e, TreeListHitTest ht) {
				SetState(Regular);
			}
			public override void SelectionChanged() {
				TreeList.RefreshRowsInfo(false);
				base.SelectionChanged();
			}
		}
		public class IncrementalSearchState : RegularState {
			public IncrementalSearchState(TreeListHandler handler)
				: base(handler) {
				TreeList.IncrementalText = "";
			}
			public override TreeListState State { get { return TreeListState.IncrementalSearch; } }
			protected virtual bool IsNavigationKey(Keys key) {
				return IsNavKey(key);
			}
			protected bool IsNavKey(Keys key) {
				return Array.IndexOf(navKeys, key) != -1;
			}
			protected virtual bool DoIncrementalSearchKeyDown(KeyEventArgs e) {
				if(e.KeyCode == Keys.Escape) {
					TreeList.ViewInfo.PaintAnimatedItems = false;
					TreeList.InvalidateNode(TreeList.FocusedNode);
					SetState(TreeListState.Regular);
					return true;
				}
				if(e.KeyCode == Keys.Enter || e.KeyCode == Keys.F2) {
					SetState(TreeListState.Regular);
					TreeList.ShowEditor();
					return true;
				}
				if(IsNavigationKey(e.KeyCode)) {
					if(DoIncrementalSearchNavigation(e)) return true;
					return false;
				}
				return true;
			}
			protected internal virtual bool DoIncrementalSearchNavigation(KeyEventArgs e) {
				switch(e.KeyCode) {
					case Keys.Up:
					case Keys.Down:
						if(e.Modifiers == Keys.Control) {
							TreeListNode newNode = FindNode(new FindNodeArgs(TreeList.FocusedNode, TreeList.FocusedColumn, TreeList.IncrementalText, true, false, e.KeyCode == Keys.Down));
							if(newNode != null) {
								TreeList.FocusedNode = newNode;
								if(TreeList.OptionsSelection.MultiSelect) TreeList.Selection.Set(TreeList.FocusedNode);
							}
							return true;
						}
						break;
					case Keys.Left:
					case Keys.Right:
						if(e.Modifiers == Keys.Control)
							return false; 
						break;
				}
				SetState(TreeListState.Regular);
				return false;
			}
			public override void KeyDown(KeyEventArgs e) {
				if(!DoIncrementalSearchKeyDown(e)) {
					base.KeyDown(e);
				}
			}
			public override void KeyPress(KeyPressEventArgs e) {
				if(!DoIncrementalSearch(e)) {
					base.KeyPress(e);
				}
			}
			public override void MouseDown(MouseEventArgs e, TreeListHitTest ht) {
				base.MouseDown(e, ht);
				TreeList.ViewInfo.PaintAnimatedItems = false;
				TreeList.InvalidateNode(TreeList.FocusedNode);
			}
			protected virtual bool DoIncrementalSearch(KeyPressEventArgs e) {
				string text = TreeList.IncrementalText;
				if(e.KeyChar == 8) {
					if(text.Length < 2) {
						TreeList.ViewInfo.PaintAnimatedItems = false;
						TreeList.InvalidateNode(TreeList.FocusedNode);
						SetState(TreeListState.Regular);
						return true;
					}
					text = text.Substring(0, text.Length - 1);
				}
				else {
					if(e.KeyChar > 31)
						text += e.KeyChar;
				}
				return DoIncrementalSearch(text);
			}
			protected internal virtual bool DoIncrementalSearch(string text) {
				if(text == TreeList.IncrementalText) {
					return false;
				}
				if(text != "") {
					if(TreeList.FocusedNode == null || TreeListAutoFilterNode.IsAutoFilterNode(TreeList.FocusedNode)) return false;
					TreeListNode newNode = FindNode(new FindNodeArgs(TreeList.FocusedNode, TreeList.FocusedColumn, text, true));
					if(newNode == null) return false;
					TreeList.IncrementalText = text;
					TreeList.FocusedNode = Data.AnchorNode = newNode;
					if(TreeList.OptionsSelection.MultiSelect) TreeList.Selection.Set(TreeList.FocusedNode);
					TreeList.ViewInfo.PaintAnimatedItems = false;
					TreeList.InvalidateNode(newNode);
				}
				return true;
			}
			protected TreeListNode GetNextNode(TreeListNode node) {
				TreeListNode nextNode = GetNextNodeCore(node);
				while(nextNode != null && !TreeListFilterHelper.IsNodeVisible(nextNode)) {
					nextNode = GetNextNodeCore(nextNode);
				}
				return nextNode;
			}
			protected TreeListNode GetPrevNode(TreeListNode node) {
				TreeListNode prevNode = GetPrevNodeCore(node);
				while(prevNode != null && !TreeListFilterHelper.IsNodeVisible(prevNode)) {
					prevNode = GetPrevNodeCore(prevNode);
				}
				return prevNode;
			}
			protected TreeListNode GetNextNodeCore(TreeListNode node) {
				if(node == null) return null;
				if(node.Nodes.Count > 0) return node.Nodes[0];
				TreeList treeList = node.TreeList;
				if(node.ParentNode != null) {
					TreeListNodes owner = node.owner;
					while(node == owner.LastNode) {
						if(owner == treeList.Nodes) return null;
						if(node.ParentNode == null) return null;
						node = node.ParentNode;
						owner = node.owner;
					}
					int index = owner.IndexOf(node);
					return owner[index + 1];
				}
				else {
					if(treeList.Nodes.LastNode == node) return null;
					else {
						int index = treeList.Nodes.IndexOf(node);
						return treeList.Nodes[index + 1];
					}
				}
			}
			protected TreeListNode GetPrevNodeCore(TreeListNode node) {
				if(node == null) return null;
				if(node == node.TreeList.Nodes.FirstNode) return null;
				TreeListNodes owner = node.owner;
				if(node != owner.FirstNode) {
					int index = owner.IndexOf(node);
					node = owner[index - 1];
					while(node.HasChildren) {
						node = node.Nodes.LastNode;
					}
					return node;
				}
				return node.ParentNode;
			}
			protected virtual TreeListNode FindNode(FindNodeArgs e) {
				string text = e.Text;
				TreeListNode startNode = e.StartNode;
				if(text == "") return null;
				text = text.ToLower();
				DevExpress.XtraEditors.ViewInfo.BaseEditViewInfo bev = TreeList.CreateColumnEditViewInfo(e.Column);
				if(bev == null || !bev.IsSupportIncrementalSearch) return null;
				TreeListNode curNode = startNode;
				bool firstRow = true;
				while(true) {
					if(firstRow && e.IgnoreStartNode) {
						firstRow = false;
					}
					else {
						bev.EditValue = curNode.GetValue(e.Column.AbsoluteIndex);
						if(DevExpress.XtraEditors.BaseEdit.StringStartsWidth(bev.DisplayText.ToLower(), text)) {
							return curNode;
						}
					}
					if(e.Down) {
						if(TreeList.CanExpandNodesOnIncrementalSearch)
							curNode = GetNextNode(curNode);
						else curNode = curNode.NextVisibleNode;
						if(curNode == null) {
							if(!e.AllowLoop) return null;
							curNode = TreeList.GetNodeByVisibleIndex(0);
						}
					}
					else {
						if(TreeList.CanExpandNodesOnIncrementalSearch)
							curNode = GetPrevNode(curNode);
						else curNode = curNode.PrevVisibleNode;
						if(curNode == null) {
							if(!e.AllowLoop) return null;
							curNode = TreeList.GetNodeByVisibleIndex(TreeList.VisibleNodesCount - 1);
						}
					}
					if(curNode == startNode) return null;
				}
			}
			protected class FindNodeArgs {
				TreeListNode startNode;
				TreeListColumn column;
				string text;
				bool ignoreStartNode, allowLoop, down;
				public FindNodeArgs(TreeListNode startNode, TreeListColumn column, string text, bool down) : this(startNode, column, text, false, true, down) { }
				public FindNodeArgs(TreeListNode startNode, TreeListColumn column, string text, bool ignoreStartNode, bool allowLoop, bool down) {
					this.startNode = startNode;
					this.column = column;
					this.text = text;
					this.ignoreStartNode = ignoreStartNode;
					this.down = down;
					this.allowLoop = allowLoop;
				}
				public TreeListNode StartNode { get { return startNode; } }
				public TreeListColumn Column { get { return column; } }
				public string Text { get { return text; } }
				public bool IgnoreStartNode { get { return ignoreStartNode; } }
				public bool AllowLoop { get { return allowLoop; } }
				public bool Down { get { return down; } }
			}
		}
		#region ISupportAnimatedScroll Members
		void ISupportAnimatedScroll.OnScroll(double currentScrollValue) {
			if(TreeList.ScrollInfo.VScroll == null) return;
			TreeList.ScrollInfo.VScroll.Value = (int)Math.Round(currentScrollValue);
		}
		void ISupportAnimatedScroll.OnScrollFinish() {
			if(TreeList.ShowEditorAfterAnimatedScroll) {
				TreeList.ShowEditorAfterAnimatedScroll = false;
				TreeList.ShowEditor();
			}
		}
		public bool IsScrollAnimationInProgress { get { return AnimatedScrollHelper.Animating; } }
		public virtual void CancelAnimatedScroll() {
			AnimatedScrollHelper.Cancel();
		}
		public virtual void AnimateScroll(int distance) {
			if(TreeList.ScrollInfo.VScroll == null) return;
			int toValue = TreeList.CheckTopVisibleNodePixelValue(TreeList.ScrollInfo.VScroll.Value + distance);
			AnimatedScrollHelper.Scroll(TreeList.ScrollInfo.VScroll.Value, toValue, 1f, true);
		}
		#endregion
	}
}
