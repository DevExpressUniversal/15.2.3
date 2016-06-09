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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraVerticalGrid.ViewInfo;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.XtraVerticalGrid {
	public class BaseHandler : IDisposable, IMouseWheelScrollClient {
		VGridControlBase grid;
		int lockStateCounter;
		ControlState controlState;
		ControlStateData stateData;
		protected MouseWheelScrollHelper MouseWheelHelper { get; private set; } 
		public BaseHandler(VGridControlBase grid) {
			this.grid = grid;
			this.lockStateCounter = 0;
			this.stateData = CreateStateData(grid);
			this.controlState = CreateState(VGridState.Regular);
			this.controlState.Init();
			this.MouseWheelHelper = new MouseWheelScrollHelper(this);
		}
		protected virtual ControlState CreateState(VGridState state) {
			ControlState result = null;
			switch(state) {
				case VGridState.Disposed:			result = new DisposedState(this); break;
				case VGridState.Editing:			result = new EditingState(this); break;
				case VGridState.FocusedRowChanging:	result = new FocusedRowChangingState(this); break;
				case VGridState.HeaderPanelSizing:	result = new HeaderPanelSizingState(this); break;
				case VGridState.MultiEditorRowCellSizing: result = new MultiEditorRowCellSizingState(this); break;
				case VGridState.RecordSizing:		result = new RecordSizingState(this); break;
				case VGridState.Regular:			result = new RegularState(this); break;
				case VGridState.RowDragging:		result = new RowDraggingState(this); break;
				case VGridState.RowSizing:			result = new RowSizingState(this); break;
			}
			return result;
		}
		protected virtual ControlStateData CreateStateData(VGridControlBase grid) {
			return new ControlStateData(grid);
		}
		public void Dispose() {
			ControlState.Dispose();
		}
		public virtual void LockState() { lockStateCounter++; }
		public virtual void UnlockState() { lockStateCounter--; }
		public virtual void Resize() {
			ControlState.Resize();
		}
		public void MouseDown(MouseEventArgs e) {
			BeginAllowHideException();
			try {
				ControlState.MouseDown(e);
			}
			finally {
				EndAllowHideException();
			}
		}
		public void MouseMove(MouseEventArgs e) { ControlState.MouseMove(e); }
		public void MouseUp(MouseEventArgs e) {
			BeginAllowHideException();
			try {
				ControlState.MouseUp(e);
			}
			finally {
				EndAllowHideException();
			}
		}
		public void DoubleClick(Point pt) { ControlState.DoubleClick(pt); }
		public void MouseWheel(MouseWheelScrollClientArgs e) { ControlState.MouseWheel(e); }
		public void MouseEnter(Point pt) { ControlState.MouseEnter(pt); }
		public void MouseLeave() { ControlState.MouseLeave(); }
		public void KeyDown(KeyEventArgs e) { ControlState.KeyDown(e); }
		public void KeyUp(KeyEventArgs e) { ControlState.KeyUp(e); }
		public void KeyPress(KeyPressEventArgs e) { ControlState.KeyPress(e); }
		public bool ProcessChildControlKey(KeyEventArgs e) { return ControlState.ProcessChildControlKey(e); }
		internal bool ProcessChildControlKeyUp(KeyEventArgs e) { return ControlState.ProcessChildControlKeyUp(e); }
		public void GotFocus() { ControlState.GotFocus(); }
		public void LostFocus() { ControlState.LostFocus(); }
		public void LostCapture() { ControlState.LostCapture(); }
		public ToolTipControlInfo GetObjectTipInfo(Point pt) { return ControlState.GetObjectTipInfo(pt); }
		internal void CheckPreserveChildRows(BaseRow source, HitInfoTypeEnum dragFrom) {
			if(dragFrom != HitInfoTypeEnum.CustomizationForm) return;
			if(!Grid.OptionsBehavior.PreserveChildRows) return;
			if(source == null) return;
			while(source.ChildRows.Count > 0) {
				BaseRow row = source.ChildRows[0];
				source.ChildRows.RemoveAt(0);
				row.Visible = false;
				Grid.Rows.Add(row);
			}
		}
		protected BaseViewInfo ViewInfo { get { return Grid.ViewInfo; } }
		protected internal void SetControlState(VGridState state) {
			if(state == State || IsStateLocked) return;
			ControlState newState = CreateState(state);
			ControlState.Dispose();
			this.controlState = newState;
			ControlState.Init();
			Grid.RaiseStateChanged();
		}
		protected internal void BeginAllowHideException() { Grid.ContainerHelper.BeginAllowHideException(); }
		protected internal void EndAllowHideException() { Grid.ContainerHelper.EndAllowHideException(); }
		protected internal ControlState ControlState { get { return controlState; } }
		protected internal ControlStateData StateData { get { return stateData; } }
		protected internal VGridControlBase Grid { get { return grid; } }
		public VGridState State { get { return ControlState.State; } }
		public virtual bool IsStateLocked { get { return lockStateCounter != 0; } }
		public class ControlStateData {
			DragMaster dragMaster;
			VGridHitTest downHitTest;
			DragInfo dragInfo;
			PressInfo pressInfo;
			internal RowSizeInfo rowSizeInfo;
			internal MouseHover mouseHover;
			public Point lastDownPoint;
			public ControlStateData(VGridControlBase grid) {
				this.dragMaster = new DragMaster();
				this.downHitTest = new VGridHitTest();
				this.dragInfo = new DragInfo();
				this.pressInfo = new PressInfo();
				this.rowSizeInfo = new RowSizeInfo();
				this.mouseHover = new MouseHover(grid);
				this.lastDownPoint = Point.Empty;
			}
			public void InitPressInfo(bool editing) {
				PressInfo.PressedRow = PressedRow;
				PressInfo.IsEditing = editing;
			}
			protected internal BaseRow PressedRow { get { return DownHitTest.Row; } }
			public void SetDownHitTest(VGridHitTest value) { this.downHitTest = value; }
			public DragMaster DragMaster { get { return dragMaster; } }
			public VGridHitTest DownHitTest { get { return downHitTest; } }
			public DragInfo DragInfo { get { return dragInfo; } }
			public PressInfo PressInfo { get { return pressInfo; } }
		}
		bool IMouseWheelScrollClient.PixelModeHorz { get { return true; } }
		bool IMouseWheelScrollClient.PixelModeVert { get { return true; } }
		void IMouseWheelScrollClient.OnMouseWheel(MouseWheelScrollClientArgs e) {
			ControlState.MouseWheel(e);
		}
		public virtual void OnMouseWheel(MouseEventArgs e) {
			MouseWheelHelper.OnMouseWheel(e);
		}
		public void ScrollFinish() {
			ControlState.ScrollFinish();
		}
	}
	public abstract class ControlState : IDisposable {
		protected BaseHandler handler;
		protected ControlState(BaseHandler handler) { this.handler = handler; }
		public virtual void Init() {}
		public virtual void Dispose() {}
		public virtual void MouseDown(MouseEventArgs e) {}
		public virtual void MouseMove(MouseEventArgs e) {}
		public virtual void MouseUp(MouseEventArgs e) {
			if((e.Button & MouseButtons.Left) == 0) return;
			handler.ControlState.SetDefaultState();
		}
		public virtual void DoubleClick(Point pt) {}
		public virtual void MouseWheel(MouseWheelScrollClientArgs e) { }
		public virtual void MouseEnter(Point pt) {
			VGridHitTest ht = CalcHitTest(pt);
			Data.mouseHover.CheckMouseHotTrack(ht);
		}
		public virtual void MouseLeave() {
			Data.mouseHover.CheckMouseHotTrack(null);
		}
		public virtual void KeyDown(KeyEventArgs e) {
			if(e.KeyData != Keys.Escape) return;
			OnEscapeDown();
		}
		public virtual void KeyUp(KeyEventArgs e) {
		}
		public virtual void KeyPress(KeyPressEventArgs e) {}
		protected virtual void OnEscapeDown() {
			SetDefaultState();
			Grid.Invalidate();
		}
		public virtual void GotFocus() {
			if(Grid.FocusedRow == null || Grid.FocusedRow.IsLoadingCore)
				return;
			Grid.InvalidateRow(Grid.FocusedRow);
		}
		public virtual void LostFocus() {
			if(!Grid.HasFocus) {
				Grid.InvalidateRow(Grid.FocusedRow);
			}
		}
		public virtual void LostCapture() {
			SetDefaultState();
		}
		public virtual void Resize() {
			Grid.InvalidateUpdate();
		}
		public virtual ToolTipControlInfo GetObjectTipInfo(Point pt) { return null; }
		protected virtual RowDragEffect GetDragRowEffect(VGridHitTest ht, ref BaseRow dest) {
			BaseRow row = (Data.DownHitTest.RowInfo == null ? Data.DownHitTest.CustomizationHeaderInfo.Row : Data.DownHitTest.RowInfo.Row);
			if(ht.HitInfoType == HitInfoTypeEnum.CustomizationForm) {
				if(Data.DownHitTest.HitInfoType == HitInfoTypeEnum.CustomizationForm) return RowDragEffect.None;
				if(row.Visible && row.OptionsRow.AllowMoveToCustomizationForm)
					return RowDragEffect.InsertBefore;
				else return RowDragEffect.None;
			}
			BaseRow destRow = null;
			RowDragEffect effect = RowDragEffect.None;
			if(ht.RowInfo != null) {
				Rectangle fullRowRect = new Rectangle(ht.RowInfo.HeaderInfo.HeaderRect.Location,
					new Size(ht.RowInfo.HeaderInfo.HeaderRect.Width, ht.RowInfo.HeaderInfo.HeaderRect.Height + ViewInfo.RC.HorzLineWidth));
				if(fullRowRect.Contains(ht.PtMouse)) {
					if(ht.RowInfo.HeaderInfo.IndentBounds.Contains(ht.PtMouse)) {
						effect = RowDragEffect.InsertBefore;
					} else {
						if(ht.RowInfo.HeaderInfo.InEnd(ht.PtMouse)) {
							effect = RowDragEffect.InsertAfter;
						} else{
							effect = RowDragEffect.MoveChild;
						}
					}
					destRow = ht.RowInfo.Row;
				}
			} else {
				if(ViewInfo.ViewRects.Client.Contains(ht.PtMouse)) {
					if(Grid.VisibleRows.Count == 0)
						effect = RowDragEffect.MoveToEnd;
					else {
						Rectangle r = (Rectangle)ViewInfo.ViewRects.BandRects[ViewInfo.ViewRects.BandRects.Count - 1];
						if(r.Bottom < ViewInfo.ViewRects.Client.Bottom) {
							r = new Rectangle(r.Left, r.Bottom, r.Width, ViewInfo.ViewRects.Client.Bottom - r.Bottom);
							if(r.Contains(ht.PtMouse))
								effect = RowDragEffect.MoveToEnd;
						}
					}
				}
			}
			if(!Grid.CanMoveRow(row, destRow, effect))
				effect = RowDragEffect.None;
			dest = destRow;
			return effect;
		}
		protected virtual void SetFocusedRow(BaseRow row, RowCellInfo cellInfo, bool showEditor) {
			if(row == null) return;
			Grid.FocusedRow = row;
			if(cellInfo != null &&
				FocusedRecordCellIndex != cellInfo.RowCellIndex) {
				Grid.CloseEditor();
				FocusedRecordCellIndex = cellInfo.RowCellIndex;
			}
			if(showEditor) Grid.ShowEditor();
		}
		public virtual bool ProcessChildControlKey(KeyEventArgs e) {
			if(IsNeededKey(e.KeyCode)) {
				if((Control.ModifierKeys & Keys.Alt) != 0)
					return false;
				try {
					handler.BeginAllowHideException();
					KeyDown(e);
					return true;
				}
				catch(DevExpress.Utils.HideException) { }
				finally {
					handler.EndAllowHideException();
				}
			}
			return false;
		}
		public virtual bool ProcessChildControlKeyUp(KeyEventArgs e) {
			if(IsNeededKey(e.KeyCode)) {
				if((Control.ModifierKeys & Keys.Alt) != 0)
					return false;
				try {
					handler.BeginAllowHideException();
					KeyUp(e);
					return true;
				} catch(DevExpress.Utils.HideException) {
				} finally {
					handler.EndAllowHideException();
				}
			}
			return false;
		}
		public virtual void ScrollFinish() {
		}
		protected virtual bool IsNeededKey(Keys key) {
			foreach(Keys item in navKeys) {
				if(item == key) return true;
			}
			if(key == Keys.Tab) return Grid.OptionsBehavior.UseTabKey;
			return false;
		}
		protected virtual Keys[] navKeys { 
			get { return new Keys[] {
										Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.PageDown, Keys.PageUp,
										Keys.End, Keys.Home, Keys.Escape, Keys.Enter};
			}
		}
		protected virtual int FocusedRecordCellIndex {
			get { return Grid.FocusedRecordCellIndex; }
			set {
				if(!Grid.scrollBehavior.CanChangeCurrentRecord(Grid.LayoutStyle) &&
					(value < 0 || value >= Grid.CurrentNumCells)) return;
				Grid.FocusedRecordCellIndex = value;
			}
		}
		protected int LastRecord { get { return Grid.RecordCount - 1; } }
		protected virtual int FocusedRecord {
			get { return Grid.FocusedRecord; }
			set {
				if(!Grid.scrollBehavior.CanChangeCurrentRecord(Grid.LayoutStyle)) return;
				Grid.FocusedRecord = value;
			}
		}
		protected void ChangeState(VGridState state) { handler.SetControlState(state); }
		protected VGridHitTest CalcHitTest(Point pt) { return ViewInfo.CalcHitTest(pt); }
		protected void LockState() { handler.LockState(); }
		protected void UnlockState() { handler.UnlockState(); }
		protected virtual void SetDefaultState() {
			Data.DownHitTest.Clear();
			Grid.ResetCursor();
			bool showEditor = Data.PressInfo.IsEditing;
			Data.PressInfo.Clear();
			ChangeState(VGridState.Regular);
			if(showEditor) Grid.ShowEditor();
		}
		public abstract VGridState State { get; }
		protected VGridControlBase Grid { get { return handler.Grid; } }
		protected BaseViewInfo ViewInfo { get { return Grid.ViewInfo; } }
		protected BaseHandler.ControlStateData Data { get { return handler.StateData; } }
		protected virtual int CalcDelta(int delta) { return ViewInfo.IsRightToLeft ? -delta : delta; }
	}
	public abstract class NormalState : ControlState {
		public NormalState(BaseHandler handler) : base(handler) {}
		protected bool Expandable { get { return Grid.FocusedRow != null && Grid.FocusedRow.HasChildren; } }
		protected bool CanExpand { get { return Expandable && !Grid.FocusedRow.Expanded; } }
		protected bool CanCollapse { get { return Expandable && Grid.FocusedRow.Expanded; } }
		protected bool SuppressNextKeyPress { get; set; }
		public override void MouseDown(MouseEventArgs e) {
			Point pt = new Point(e.X, e.Y);
			Data.SetDownHitTest(CalcHitTest(pt));
			CheckMouseCursor(Data.DownHitTest.HitInfoType);
			Data.lastDownPoint = pt;
			if(e.Button == MouseButtons.Right) {
				if(Data.DownHitTest.HitInfoType == HitInfoTypeEnum.Row ||
					Data.DownHitTest.HitInfoType == HitInfoTypeEnum.ExpandButton ||
					Data.DownHitTest.HitInfoType == HitInfoTypeEnum.HeaderCell ||
					Data.DownHitTest.HitInfoType == HitInfoTypeEnum.HeaderCellImage ||
					Data.DownHitTest.HitInfoType == HitInfoTypeEnum.ValueCell)
					Grid.ShowContextMenu(pt, Data.DownHitTest);
			}
			if(e.Button == MouseButtons.Left) {
				if(Data.DownHitTest.HitInfoType == HitInfoTypeEnum.ExpandButton) {
					Grid.ChangeExpanded(Data.DownHitTest.RowInfo.Row);
				}
				if(Data.DownHitTest.HitInfoType == HitInfoTypeEnum.HeaderCell ||
					Data.DownHitTest.HitInfoType == HitInfoTypeEnum.HeaderCellImage ||
					Data.DownHitTest.HitInfoType == HitInfoTypeEnum.CustomizationForm ||
					Data.DownHitTest.HitInfoType == HitInfoTypeEnum.Row) {
					Data.InitPressInfo(State == VGridState.Editing);
					if(Data.DownHitTest.HitInfoType != HitInfoTypeEnum.CustomizationForm)
						SetFocusedRow(Data.PressInfo.PressedRow, Data.DownHitTest.CaptionInfo, false);
				}
				if(Data.DownHitTest.HitInfoType == HitInfoTypeEnum.ValueCell) {
					if(!Data.DownHitTest.RowInfo.Row.OptionsRow.AllowFocus) return;
					Grid.CloseEditor();
					VGridHitInfo hi = Data.DownHitTest.ToHitInfo();
					Grid.FocusedRow = hi.Row;
					FocusedRecord = hi.RecordIndex; 
					FocusedRecordCellIndex = hi.CellIndex;
					if(!Grid.OptionsBehavior.ShowEditorOnMouseUp)
						Grid.ShowEditor();
					if(!CheckPressEditorButton(Data.DownHitTest) && Data.DownHitTest.RowInfo != null)
						Data.InitPressInfo(State == VGridState.Editing);
					return;
				}
				if(Data.DownHitTest.HitInfoType == HitInfoTypeEnum.HeaderSeparator) {
					if(e.Clicks == 2)
						Grid.BestFit();
					BeginRowHeaderPanelSizing();
					return;
				}
				if(Data.DownHitTest.HitInfoType == HitInfoTypeEnum.MultiEditorCellSeparator) {
					BeginMultiEditorRowCellSizing(Data.DownHitTest);
					return;
				}
				if(Data.DownHitTest.HitInfoType == HitInfoTypeEnum.RowEdge) {
					BeginRowSizing(Data.DownHitTest);
					return;
				}
				if(Data.DownHitTest.HitInfoType == HitInfoTypeEnum.RecordValueEdge || Data.DownHitTest.HitInfoType == HitInfoTypeEnum.BandEdge) {
					BeginRecordSizing();
					return;
				}
			}
		}
		public override void MouseMove(MouseEventArgs e) {
			if(Grid.ActiveEditor == null)
				ViewInfo.Scroller.OnAction(DevExpress.XtraEditors.ScrollNotifyAction.MouseMove);
			VGridHitTest ht = CalcHitTest(new Point(e.X, e.Y));
			if(Data.PressInfo.PressedRow != null && (e.Button & MouseButtons.Left) != 0) {
				if(CanBeginDrag(ht.PtMouse, Data.DownHitTest)) {
					Grid.CloseEditor();
					BeginDragRow(ht);
					return;
				}
				else if(Grid.CustomizationForm == null && CanBeginMove(ht.PtMouse, Data.DownHitTest)) {
					ChangeState(VGridState.FocusedRowChanging);
					return;
				}
			}
			Data.mouseHover.CheckMouseHotTrack(ht);
			CheckMouseCursor(ht.HitInfoType);
		}
		public override void DoubleClick(Point pt) {
			if(Data.lastDownPoint != pt)
				return;
			VGridHitTest ht = CalcHitTest(pt);
			if(Data.DownHitTest.HitInfoType == HitInfoTypeEnum.HeaderCell || 
				Data.DownHitTest.HitInfoType == HitInfoTypeEnum.HeaderCellImage ||
				Data.DownHitTest.HitInfoType == HitInfoTypeEnum.Row) {
				if(ht.Row == Data.DownHitTest.Row)
					Grid.RowDoubleClick(ht.Row);
			}
		}
		public override void KeyDown(KeyEventArgs e) {
			switch(e.KeyData) {
				case Keys.Enter: InvertEditing(); break;
				case Keys.Escape: base.KeyDown(e); break;
				default: KeyDownCore(e); break;
			}
		}
		public override ToolTipControlInfo GetObjectTipInfo(Point pt) {
			VGridHitTest ht = CalcHitTest(pt);
			if(ht.HitInfoType == HitInfoTypeEnum.ValueCell) {
				if(ht.ValueInfo.EditorViewInfo.ErrorIconBounds.Contains(pt) && ht.ValueInfo.IsValidErrorIconText) {
					ToolTipControlInfo res = new ToolTipControlInfo(ht.ValueInfo.EditorViewInfo, ht.ValueInfo.EditorViewInfo.ErrorIconText);
					res.ToolTipImage = ht.ValueInfo.EditorViewInfo.ErrorIcon;
					return res;
				}
				ToolTipControlInfo info = ht.ValueInfo.EditorViewInfo.GetToolTipInfo(pt);
				if(info != null) {
					if(Grid.OptionsHint.ShowCellHints) {
						info.Object = new VGridCellToolTipInfo(ht.ToHitInfo().RecordIndex, ht.ToHitInfo().Row, info.Object);
						return info;
					} else {
						return null;
					}
				}
			}
			if (IsIndent(ht) && Grid.OptionsHint.ShowRowHeaderHints && ht.RowInfo.HeaderInfo.CaptionsInfo.Count != 0) {
				ToolTipControlInfo info = GetToolTipInfo((RowCaptionInfo)ht.RowInfo.HeaderInfo.CaptionsInfo[0]);
				if (info != null)
					return info;
			}
			if(ht.HitInfoType == HitInfoTypeEnum.HeaderCell && Grid.OptionsHint.ShowRowHeaderHints) {
				ToolTipControlInfo info = GetToolTipInfo(ht.CaptionInfo);
				if (info != null)
					return info;
			}
			return null;
		}
		bool IsIndent(VGridHitTest ht) {
			return (ht.HitInfoType == HitInfoTypeEnum.Row || ht.HitInfoType == HitInfoTypeEnum.HeaderCellImage) && ht.HitInfoType != HitInfoTypeEnum.HeaderCell && ht.HitInfoType != HitInfoTypeEnum.ValueCell;
		}
		ToolTipControlInfo GetToolTipInfo(RowCaptionInfo captionInfo) {
			RowProperties properties = captionInfo.Row.GetRowProperties(captionInfo.RowCellIndex);
			if((!captionInfo.IsCaptionFit) ||
				(properties != null && !string.IsNullOrEmpty(properties.ToolTip))) {
				ToolTipControlInfo info = new ToolTipControlInfo(captionInfo, captionInfo.ToolTip);
				return info;
			}
			return null;
		}
		int OneRecordModeNavigateLeft(BaseRow row) {
			if(row == null)
				return 0;
			if(Grid.FocusedRow.HasChildren && row.Expanded) {
				Grid.ChangeExpanded(Grid.FocusedRow);
				return 0;
			} else {
				return -1;
			}
		}
		int OneRecordModeNavigateRight(BaseRow row) {
			if(row == null)
				return 0;
			if(Grid.FocusedRow.HasChildren && !row.Expanded) {
				Grid.ChangeExpanded(Grid.FocusedRow);
				return 0;
			} else {
				return +1;
			}
		}
		#region Keyboard Left Key Process
		protected virtual bool CanMoveFocusPrevRow() {
			return FocusedRecord == 0 && Grid.FocusedRow != Grid.GetFirstVisible() && FocusedRecordCellIndex == 0;
		}
		protected virtual void MoveFocusLeft(ref int delta){
			if((Grid.RecordCount == 1 && Grid.CurrentNumCells == 1) || (Grid.FocusedRow is CategoryRow)) {
				delta = OneRecordModeNavigateLeft(Grid.FocusedRow);
				if(delta < 0) {
					FocusedRecord = Grid.RecordCount - 1;
				}
				return;
			}
			if(Grid.FocusedRow != null && !(Grid.FocusedRow is CategoryRow)) {
				if(CanMoveFocusPrevRow()) {
					FocusedRecord = Grid.RecordCount - 1;
					delta--;
				}
				else
					FocusedRecordCellIndex--;
			}
		}
		#endregion
		#region Keyboard Right Key Process
		protected virtual bool CanMoveFocusNextRow() {
			return FocusedRecord == Grid.RecordCount - 1 && Grid.FocusedRow != Grid.GetLastVisible() && FocusedRecordCellIndex == Grid.CurrentNumCells - 1;
		}
		protected virtual void MoveFocusRight(ref int delta) {
			if((Grid.RecordCount == 1 && Grid.CurrentNumCells == 1) || (Grid.FocusedRow is CategoryRow)) {
				delta = OneRecordModeNavigateRight(Grid.FocusedRow);
				if(0 < delta) {
					FocusedRecord = 0;
				}
				return;
			}
			if(Grid.FocusedRow != null && !(Grid.FocusedRow is CategoryRow)) {
				if(CanMoveFocusNextRow()) {
					FocusedRecord = 0;
					delta++;
				}
				else {
					FocusedRecordCellIndex++;
				}
			}
		}
		#endregion
		protected virtual bool KeyboardNavigationProcess(KeyEventArgs e) {
			int delta = 0;
			switch(e.KeyCode) {
				case Keys.Left:
					if(Grid.ViewInfo.IsRightToLeft) MoveFocusRight(ref delta);
					else MoveFocusLeft(ref delta);
					break;
				case Keys.Right:
					if(Grid.ViewInfo.IsRightToLeft) MoveFocusLeft(ref delta);
					else MoveFocusRight(ref delta);
					break;
				case Keys.Up:
					delta = -1;
					break;
				case Keys.Down:
					delta = 1;
					break;
				case Keys.PageUp:
					delta = GetPageUpDelta();
					break;
				case Keys.PageDown:
					delta = GetPageDownDelta();
					break;
				case Keys.Home:
					FocusedRecord = 0;
					if(e.Control) Grid.FocusFirst();
					break;
				case Keys.End:
					FocusedRecord = Grid.RecordCount - 1;
					if(e.Control) Grid.FocusLast();
					break;
				case Keys.Tab:
					if(!Grid.OptionsBehavior.UseTabKey) break;
					DoNavigationByNavigationInfo(GetNextTabNavigatonInfo(!e.Shift));
					break;
			}
			if(delta == 0) return false;
			Grid.MoveFocusedRow(delta, true);
			return true;
		}
		protected virtual bool ResizeHeaderPanelProcess(KeyEventArgs e) {
			if(!e.Control || !CanResizeHeaderPanel) return false;
			switch(e.KeyCode) {
				case Keys.Left:
					Grid.RowHeaderWidth -= CalcDelta(Grid.RowHeaderWidthChangeStep);
					break;
				case Keys.Right:
					Grid.RowHeaderWidth += CalcDelta(Grid.RowHeaderWidthChangeStep);
					break;
			}
			return true;
		}
		protected virtual void NavigateControl(KeyEventArgs e) {
			if(ResizeHeaderPanelProcess(e)) return;
			if(KeyboardNavigationProcess(e)) return;
			switch(e.KeyCode) {
				case Keys.Subtract:
					if(CanCollapse) {
						Grid.FocusedRow.Expanded = false;
						SuppressNextKeyPress = true;
					}
					break;
				case Keys.Add:
					if(CanExpand) {
						Grid.ExpandRow(Grid.FocusedRow);
						SuppressNextKeyPress = true;
					}
					break;
				case Keys.Multiply:
					if (Expandable) {
						Grid.FullExpandRow(Grid.FocusedRow);
						SuppressNextKeyPress = true;
					}
					break;
				case Keys.C:
					if(e.Control) Grid.CopyToClipboard();
					break;
			}
		}
		int GetPageDownDelta() {
			if (Grid.FocusedRow == null)
				return 1;
			int delta = Math.Max(0, Grid.Scroller.TopVisibleRowIndex + PageRowCountStep - Grid.FocusedRow.VisibleIndex);
			if (delta == 0)
				return PageRowCountStep;
			return delta;
		}
		int GetPageUpDelta() {
			if (Grid.FocusedRow == null)
				return -1;
			int delta = Grid.Scroller.TopVisibleRowIndex - Grid.FocusedRow.VisibleIndex;
			if (delta == 0)
				return -PageRowCountStep;
			return delta;
		}
		int PageRowCountStep { get { return Math.Max(1, ViewInfo.VisibleRowCount - 2); } }
		protected void DoNavigationByNavigationInfo(NavigationInfo navInfo) {
			if(navInfo == null) return;
			Grid.MoveFocusedRow(navInfo.Row.VisibleIndex - (Grid.FocusedRow == null ? 0 : Grid.FocusedRow.VisibleIndex), false);
			Grid.FocusedRecordCellIndex = navInfo.CellIndex;
			Grid.FocusedRecord = navInfo.RecordIndex;
		}
		protected virtual NavigationInfo GetNextTabNavigatonInfo(bool forward) {
			if(Grid.VisibleRows.Count == 0) return null;
			NavigationInfo result = null;
			int beginIndex = (Grid.FocusedRow == null ? 0 : Grid.FocusedRow.VisibleIndex);
			bool canChangeRecord = Grid.scrollBehavior.CanChangeCurrentRecord(Grid.LayoutStyle);
			if(forward) {
				int focusedPropCount = (Grid.FocusedRow == null ? 1 : Grid.FocusedRow.RowPropertiesCount);
				if(beginIndex + 1 == Grid.VisibleRows.Count && Grid.FocusedRecordCellIndex == focusedPropCount - 1)
					beginIndex++;
				result = GetFocusedRecordNavigatonInfo(beginIndex, Grid.VisibleRows.Count, 1, true);
				if(result == null) {
					if(canChangeRecord && (Grid.RecordCount <= Grid.FocusedRecord + 1 && Grid.RecordCount > 1)) return null;
					for(int i = 0; i <= beginIndex; i++) {
						BaseRow row = Grid.VisibleRows[i];
						if(row == null || !row.TabStop) continue;
						return new NavigationInfo(row, Grid.FocusedRecord + (canChangeRecord ? 1 : 0), 0);
					}
				}
			}
			else {
				if(beginIndex == 0 && Grid.FocusedRecordCellIndex == 0)
					beginIndex = -1;
				result = GetFocusedRecordNavigatonInfo(beginIndex, -1, -1, false);
				if(result == null) {
					if(canChangeRecord && (Grid.FocusedRecord == 0 && Grid.RecordCount > 1)) return null;
					for(int i = Grid.VisibleRows.Count - 1; i >= beginIndex; i--) {
						BaseRow row = Grid.VisibleRows[i];
						if(row == null || !row.TabStop) continue;
						return new NavigationInfo(row, Grid.FocusedRecord + (canChangeRecord ? -1 : 0), row.RowPropertiesCount - 1);
					}
				}
			}
			return result;
		}
		NavigationInfo GetFocusedRecordNavigatonInfo(int beginIndex, int endIndex, int step, bool forward) {
			for(int i = beginIndex; i != endIndex; i += step) {
				BaseRow row = Grid.VisibleRows[i];
				if(row == null || !row.TabStop || !row.OptionsRow.AllowFocus) continue;
				int minValue = (forward ? 0 : row.RowPropertiesCount - 1);
				int cellIndex = (row == Grid.FocusedRow ? Grid.FocusedRecordCellIndex + step : minValue);
				if(cellIndex >= row.RowPropertiesCount || cellIndex < 0) continue;
				return new NavigationInfo(row, Grid.FocusedRecord, cellIndex);
			}
			return null;
		}
		protected virtual bool CanBeginDrag(Point pt, VGridHitTest ht) {
			if(ht.HitInfoType == HitInfoTypeEnum.ValueCell) return false;
			if(Grid.CustomizationForm == null) {
				if(!Grid.OptionsBehavior.DragRowHeaders) return false;
				if(!Data.PressInfo.PressedRow.OptionsRow.AllowMove) return false;
			}
			return CanBeginMove(pt, ht);
		}
		protected virtual bool CanBeginMove(Point pt, VGridHitTest ht) {
			Point down = ht.PtMouse;
			return (Math.Abs(down.X - pt.X) > SystemInformation.DragSize.Width || Math.Abs(down.Y - pt.Y) > SystemInformation.DragSize.Height);
		}
		protected override void OnEscapeDown() {
		}
		private void BeginDragRow(VGridHitTest ht) {
			if(Data.PressedRow.OptionsRow.AllowMove || Grid.CustomizationForm != null) {
				BaseRow dest = null;
				DoStartDragRow(Data.PressedRow, ht.PtMouse, GetDragRowEffect(ht, ref dest), RowDragSource.Control);
				Grid.OpenRightCustomizationTabPage(Data.PressedRow);
			}
		}
		private void BeginRowHeaderPanelSizing() {
			Data.rowSizeInfo.oldSize = Grid.RowHeaderWidth;
			Data.rowSizeInfo.oldSize2 = Grid.RecordWidth;
			Data.rowSizeInfo.scaleCoeff = ViewInfo.HorzScaleCoeff;
			ChangeState(VGridState.HeaderPanelSizing);
		}
		private void BeginMultiEditorRowCellSizing(VGridHitTest ht) {
			int cellWidth = 0;
			if(ht.ValueInfo != null) {
				Data.rowSizeInfo.cellIndex = ht.ValueInfo.RowCellIndex;
				cellWidth = ht.ValueInfo.Bounds.Width;
			}
			else {
				Data.rowSizeInfo.cellIndex = ht.CaptionInfo.RowCellIndex;
				cellWidth = ht.CaptionInfo.CaptionRect.Width;
			}
			Data.rowSizeInfo.row = ht.RowInfo.Row;
			FillViewWidths(ht);
			if(ht.ValueInfo != null) {
				Data.rowSizeInfo.oldSize = Data.rowSizeInfo.editorRow.PropertiesCollection[Data.rowSizeInfo.cellIndex].CellWidth;
				Data.rowSizeInfo.oldSize2 = Data.rowSizeInfo.editorRow.PropertiesCollection[Data.rowSizeInfo.cellIndex + 1].CellWidth;
			}
			else {
				MultiEditorRowProperties firstProperties = Data.rowSizeInfo.editorRow.PropertiesCollection[Data.rowSizeInfo.cellIndex];
				Data.rowSizeInfo.oldSize = firstProperties.Width;
				MultiEditorRowProperties nextVisibleProperties = Data.rowSizeInfo.editorRow.GetNextVisibleProperties(Data.rowSizeInfo.cellIndex + 1);
				Data.rowSizeInfo.oldSize2 = nextVisibleProperties.Width;
			}
			Data.rowSizeInfo.scaleCoeff = (float)Data.rowSizeInfo.oldSize / (float)cellWidth;
			ChangeState(VGridState.MultiEditorRowCellSizing);
		}
		private void FillViewWidths(VGridHitTest ht) {
			MultiEditorRow row = ht.RowInfo.Row as MultiEditorRow;
			if(ht.ValueInfo != null) {
				for(int i = 0; i < ht.RowInfo.ValuesInfo.Count; i++) {
					RowValueInfo rowInfo = ht.RowInfo.ValuesInfo[i];
					int index = row.PropertiesCollection.IndexOf(rowInfo.Properties);
					if(index < 0) continue;
					row.PropertiesCollection[index].SetCellWidth(ht.RowInfo.ValuesInfo[i].Bounds.Width);
				}
			}
			else {
				for (int i = 0; i < ht.RowInfo.HeaderInfo.CaptionsInfo.Count; i++) {
					RowCaptionInfo rc = (RowCaptionInfo)ht.RowInfo.HeaderInfo.CaptionsInfo[i];
					row.PropertiesCollection[rc.RowCellIndex].SetWidth(rc.CaptionRect.Width);
				}
			}
		}
		void BeginRowSizing(VGridHitTest ht) {
			Data.rowSizeInfo.row = ht.RowInfo.Row;
			Data.rowSizeInfo.oldSize = ViewInfo.GetVisibleRowHeight(ht.RowInfo.Row);
			ChangeState(VGridState.RowSizing);
		}
		void BeginRecordSizing() {
			Data.rowSizeInfo.oldSize = Grid.RecordWidth;
			ChangeState(VGridState.RecordSizing);
		}
		protected virtual void DoStartDragRow(BaseRow row, Point ptClient, RowDragEffect effect, RowDragSource source) {
			Point screen = Grid.PointToScreen(ptClient);
			DevExpress.XtraVerticalGrid.Events.StartDragRowEventArgs e = new DevExpress.XtraVerticalGrid.Events.StartDragRowEventArgs(row, screen, effect, source);
			ChangeState(VGridState.RowDragging);
			Grid.RaiseStartDragRow(e);
			Bitmap img = GetRowDragBitmap(row); 
			Data.DragMaster.StartDrag(img, screen, e.Effect);
		}
		private void CheckMouseCursor(HitInfoTypeEnum hitInfoType) {
			if(Data.PressInfo.PressedRow != null) {
				Grid.Cursor = Cursors.Arrow;
				return;
			}
			bool resetCursor = Grid.CanResetCursor;
			if(hitInfoType == HitInfoTypeEnum.HeaderSeparator ||
				hitInfoType == HitInfoTypeEnum.MultiEditorCellSeparator ||
				hitInfoType == HitInfoTypeEnum.RecordValueEdge ||
				hitInfoType == HitInfoTypeEnum.BandEdge) {
				Grid.Cursor = Cursors.SizeWE;
				resetCursor = false;
			}
			if(hitInfoType == HitInfoTypeEnum.RowEdge) {
				Grid.Cursor = Cursors.SizeNS;
				resetCursor = false;
			}
			if(hitInfoType == HitInfoTypeEnum.ExpandButton) {
				Grid.Cursor = Cursors.Hand;
				resetCursor = false;
			}
			if (resetCursor)
				Grid.Cursor = Cursors.Arrow;
		}
		private bool CheckPressEditorButton(VGridHitTest ht) {
			if(Grid.ActiveEditor != null && ht.ValueInfo != null) {
				int button = (int)ht.ValueInfo.EditorViewInfo.CalcHitInfo(ht.PtMouse).HitTest;
				if(button > -1 || Grid.ActiveEditor is DevExpress.XtraEditors.CheckEdit) {
					Grid.ContainerHelper.ActiveEditor_LMouseDown();
					return true;
				}
			}
			return false;
		}
		private Bitmap GetRowDragBitmap(BaseRow row) {
			Size size = new Size(160, ViewInfo.GetVisibleRowHeight(row));
			return Grid.GetRowDragBitmap(row, size);
		}
		protected virtual void InvertEditing() {}
		protected virtual void KeyDownCore(KeyEventArgs e) {}
		protected bool CanResizeHeaderPanel { get { return Grid.OptionsBehavior.ResizeHeaderPanel; } }
	}
	public class RegularState : NormalState {
		public RegularState(BaseHandler handler) : base(handler) {}
		public override void MouseWheel(MouseWheelScrollClientArgs e) {
			if (e.Horizontal || Grid.OptionsBehavior.RecordsMouseWheel || !Grid.Scroller.IsNeededVScrollBar) {
				if (Grid.scrollBehavior.CanChangeCurrentRecord(Grid.LayoutStyle))
					Grid.Scroller.HorzScrollPixel(-e.Delta);
			}
			else {
				if ((e.InPixels && !Grid.Scroller.IsScrollAnimationInProgress) || !Grid.OptionsBehavior.AllowAnimatedScrolling)
					Grid.Scroller.VertScrollPixel(-e.Delta);
				else
					Grid.Scroller.AnimateScroll(GetWheelDistance(-e.Delta));
			}
		}
		int GetWheelDistance(int delta) {
			int height = Grid.Scroller.ViewPortHeight;
			return Math.Sign(delta) * Math.Min(height, Math.Max(300, height / 3));
		}
		protected override void InvertEditing() { Grid.ShowEditor(); }
		protected override void KeyDownCore(KeyEventArgs e) {
			if(e.KeyCode == Keys.F2 || ViewInfo.IsActivateKey(e.KeyData, Grid.FocusedRow, Grid.DataModeHelper.Position, Grid.FocusedRecordCellIndex)) {
				InvertEditing();
				if(Grid.ActiveEditor != null && e.KeyCode != Keys.Enter) {
					Grid.ActiveEditor.SendKey(e);
				}
				return;
			}
			if (e.KeyData == (Keys.F | Keys.Control)) {
				if(Grid.SearchControlFocus()) return;
				if (Grid.OptionsFind.Visibility != FindPanelVisibility.Never) {
					Grid.ShowFindPanel();
					return;
				}
			}
			NavigateControl(e);
		}
		public override void MouseUp(MouseEventArgs e) {
			if(e.Button != MouseButtons.Left) return;
			if(Data.DownHitTest.HitInfoType == HitInfoTypeEnum.ValueCell) {
				VGridHitTest hitTest = CalcHitTest(new Point(e.X, e.Y));
				if(hitTest.HitInfoType == HitInfoTypeEnum.ValueCell && Grid.OptionsBehavior.ShowEditorOnMouseUp) {
					Grid.ShowEditor();
				}
			}
			Data.PressInfo.Clear();
			Data.DownHitTest.Clear();
		}
		public override void KeyPress(KeyPressEventArgs e) {
			if(char.IsControl(e.KeyChar) || IsExpandCollapseAction(e))
				return;
			if (SuppressNextKeyPress) {
				SuppressNextKeyPress = false;
				return;
			}
			InvertEditing();
			if(Grid.ActiveEditor != null) {
				Grid.ActiveEditor.SendKey(Grid.lastKeyMessage, e);
			}
		}
		private bool IsExpandCollapseAction(KeyPressEventArgs e) {
			return (e.KeyChar == '-' && CanCollapse) ||
				(e.KeyChar == '+' && CanExpand) ||
				(e.KeyChar == '*' && Expandable);
		}
		public override sealed VGridState State { get { return VGridState.Regular; } }
	}
	public class EditingState : NormalState {
		public EditingState(BaseHandler handler) : base(handler) {}
		public override void Dispose() {
			handler.LockState();
			try { Grid.CloseEditor(); }
			finally { handler.UnlockState(); }
		}
		protected override void OnEscapeDown() {
			Grid.HideEditor();
			base.OnEscapeDown();
		}
		protected override void InvertEditing() { Grid.CloseEditor(); }
		public override void LostFocus() {
			if(Form.ActiveForm == null || Form.ActiveForm.ActiveControl == null) return;
			if(Form.ActiveForm.ActiveControl.Contains(Grid) || Form.ActiveForm.ActiveControl.FindForm() == Grid.FindForm())
				base.LostFocus();
		}
		public override void MouseUp(MouseEventArgs e) {
			if(e.Clicks != 1 || e.Button != MouseButtons.Left) return;
			if(Data.DownHitTest.HitInfoType == HitInfoTypeEnum.ValueCell) {
				VGridHitTest hitTest = CalcHitTest(new Point(e.X, e.Y));
				if(hitTest.HitInfoType == HitInfoTypeEnum.ValueCell && Grid.OptionsBehavior.ShowEditorOnMouseUp) {
					SetDefaultState();
				}
			}
		}
		public override void LostCapture() {}
		public override void Resize() {
			Grid.CloseEditor();
			base.Resize();
		}
		public override void KeyDown(KeyEventArgs e) {
			if(e.KeyCode == Keys.Enter && Grid.OptionsBehavior.UseEnterAsTab) {
				KeyDownCore(e);
				return;
			}
			base.KeyDown(e);
		}
		public override void KeyUp(KeyEventArgs e) {
			KeyUpCore(e);
		}
		public override void ScrollFinish() {
			Grid.ShowEditor();
		}
		protected override void KeyDownCore(KeyEventArgs e) {
			BehaveControl(e, false);
		}
		void KeyUpCore(KeyEventArgs e) {
			BehaveControl(e, true);
		}
		bool CanMoveEditor(KeyEventArgs e) {
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
					canMoveEditor = true;
					break;
				case Keys.Tab:
					canMoveEditor = Grid.OptionsBehavior.UseTabKey;
					break;
				case Keys.Enter:
					canMoveEditor = Grid.OptionsBehavior.UseEnterAsTab;
					break;
			}
			return canMoveEditor;
		}
		void BehaveControl(KeyEventArgs e, bool showEditor) {
			if(!CanMoveEditor(e))
				return;
			try {
				LockState();
				if(showEditor) {
					if (!Grid.Scroller.IsScrollAnimationInProgress)
						Grid.ShowEditor();
				} else {
					NavigateControl(e);
				}
			} finally {
				UnlockState();
			}
		}
		protected override void NavigateControl(KeyEventArgs e) {
			Grid.CloseEditor();
			base.NavigateControl(e);
			if(e.KeyCode == Keys.Enter && Grid.OptionsBehavior.UseEnterAsTab)
				DoNavigationByNavigationInfo(GetNextTabNavigatonInfo(true));
		}
		protected override bool IsNeededKey(Keys key) {
			if(key == Keys.Enter && Grid.OptionsBehavior.UseEnterAsTab)
				return true;
			return base.IsNeededKey(key);
		}
		public override bool ProcessChildControlKey(KeyEventArgs e) {
			if(Grid.ActiveEditor.IsNeededKey(e)) return false;
			return base.ProcessChildControlKey(e);
		}
		public override bool ProcessChildControlKeyUp(KeyEventArgs e) {
			if(Grid.ActiveEditor.IsNeededKey(e)) return false;
			return base.ProcessChildControlKeyUp(e);
		}
		public override sealed VGridState State { get { return VGridState.Editing; } }
	}
	public class RowDraggingState : BaseDragState {
		public RowDraggingState(BaseHandler handler) : base(handler) {
			Data.DragInfo.DragScroll += new EventHandler(ScrollTimer_Tick);
		}
		public override void Dispose() {
			Data.DragInfo.DragScroll -= new EventHandler(ScrollTimer_Tick);
			Data.DragInfo.Stop();
		}
		public override void MouseMove(MouseEventArgs e) {
			VGridHitTest ht = CalcHitTest(new Point(e.X, e.Y));
			ScrollMove(ht);
			DoDragRow(ht);
		}
		public override void MouseUp(MouseEventArgs e) {
			if((e.Button & MouseButtons.Left) == 0) return;
			VGridHitTest ht = CalcHitTest(new Point(e.X, e.Y));
			EndDragRow(ht);
			Grid.lastMouseUpX = Grid.PointToScreen(ht.PtMouse).X; 
			Grid.lastMouseUpY = Grid.PointToScreen(ht.PtMouse).Y;
			base.MouseUp(e);
		}
		protected virtual RowDragEffect DoEndDragRow(BaseRow row, Point ptClient, bool canceled, RowDragEffect effect) {
			Point screen = (canceled ? Grid.PointToScreen(ptClient) : Point.Empty);
			DevExpress.XtraVerticalGrid.Events.EndDragRowEventArgs e = new DevExpress.XtraVerticalGrid.Events.EndDragRowEventArgs(row, screen, effect, canceled);
			Grid.RaiseEndDragRow(e);
			return e.Effect;
		}
		protected virtual void DoDragRow(VGridHitTest ht) {
			BaseRow dest = null;
			RowDragEffect effect = GetDragRowEffect(ht, ref dest);
			Point screen = Grid.PointToScreen(ht.PtMouse);
			DevExpress.XtraVerticalGrid.Events.DragRowEventArgs e = new DevExpress.XtraVerticalGrid.Events.DragRowEventArgs(Data.PressInfo.PressedRow, screen, effect);
			Grid.RaiseProcessDragRow(e);
			Data.DragMaster.DoDrag(screen, e.Effect, true);
		}
		internal void EndDragRow(VGridHitTest ht) {
			Data.DragMaster.EndDrag();
			Data.DragInfo.Stop();
			BaseRow dest = null;
			RowDragEffect effect = DoEndDragRow(Data.PressInfo.PressedRow, ht.PtMouse, true, GetDragRowEffect(ht, ref dest));
			if(effect != RowDragEffect.None) {
				if(ht.HitInfoType == HitInfoTypeEnum.CustomizationForm) {
					if(Data.PressInfo.PressedRow.OptionsRow.AllowMoveToCustomizationForm)
						Data.PressInfo.PressedRow.Visible = false;
				}
				else {
					Grid.BeginUpdate();
					try {
						Grid.MoveRow(Data.PressInfo.PressedRow, dest, effect);
						handler.CheckPreserveChildRows(Data.PressInfo.PressedRow, Data.DownHitTest.HitInfoType);
						Data.PressInfo.PressedRow.Visible = true;
					}
					finally {
						Grid.EndUpdate();
					}
				}
			}
			Data.PressInfo.Clear();
			ChangeState(VGridState.Regular);
		}
		internal void CancelDragRow() {
			Data.DragMaster.EndDrag();
			Data.DragInfo.Stop();
			ChangeState(VGridState.Regular);
			DoEndDragRow(Data.PressInfo.PressedRow, Point.Empty, false, RowDragEffect.None);
			Data.PressInfo.Clear();
		}
		protected virtual void ScrollTimer_Tick(object sender, EventArgs e) {
			if(Data.DragInfo.cy != 0) Grid.VertScroll(Data.DragInfo.cy);
		}
		protected override void SetDefaultState() {
			base.SetDefaultState();
			CancelDragRow();
		}
		public override sealed VGridState State { get { return VGridState.RowDragging; } }
	}
	public class RowSizingState : ControlState {
		public RowSizingState(BaseHandler handler) : base(handler) {}
		public override void MouseMove(MouseEventArgs e) {
			VGridHitTest ht = CalcHitTest(new Point(e.X, e.Y));
			UpdateRowHeight(ht.PtMouse.Y - Data.DownHitTest.PtMouse.Y);
		}
		public override void MouseUp(MouseEventArgs e) {
			if((e.Button & MouseButtons.Left) == 0) return;
			UpdateRowHeight(e.Y - Data.DownHitTest.PtMouse.Y);
			ChangeState(VGridState.Regular);
			base.MouseUp(e);
		}
		public override void DoubleClick(Point pt) {
			if(Data.lastDownPoint != pt) return;
			VGridHitTest ht = CalcHitTest(pt);
			if(Data.DownHitTest.HitInfoType == HitInfoTypeEnum.RowEdge) {
				Data.DownHitTest.RowInfo.Row.Height = -1;
				Data.rowSizeInfo.oldSize = Data.DownHitTest.RowInfo.Row.Height;
				ChangeState(VGridState.Regular);
			}
		}
		void UpdateRowHeight(int cy) {
			LockState();
			Grid.BeginUpdate();
			try {
				int newHeight = Data.rowSizeInfo.oldSize + ((Data.rowSizeInfo.row.Fixed == FixedStyle.Bottom) ? - cy : cy);
				Data.rowSizeInfo.row.Height = Math.Max(newHeight, BaseRow.MinHeight);
			} finally {
				Grid.CancelUpdate();
				UnlockState();
			}
			Grid.InvalidateUpdate();
		}
		protected override void SetDefaultState() {
			base.SetDefaultState();
			Data.rowSizeInfo.row.Height = Data.rowSizeInfo.oldSize;
			Data.rowSizeInfo.Reset();
		}
		public override sealed VGridState State { get { return VGridState.RowSizing; } }
	}
	public class MultiEditorRowCellSizingState : ControlState {
		public MultiEditorRowCellSizingState(BaseHandler handler) : base(handler) {}
		public override void MouseMove(MouseEventArgs e) {
			VGridHitTest ht = CalcHitTest(new Point(e.X, e.Y));
			UpdateMultiEditorRowCellWidth(ht.PtMouse.X - Data.DownHitTest.PtMouse.X);
		}
		public override void MouseUp(MouseEventArgs e) {
			if((e.Button & MouseButtons.Left) == 0) return;
			UpdateMultiEditorRowCellWidth(e.X - Data.DownHitTest.PtMouse.X);
			ChangeState(VGridState.Regular);
			base.MouseUp(e);
		}
		protected override void SetDefaultState() {
			base.SetDefaultState();
			Data.rowSizeInfo.editorRow.PropertiesCollection[Data.rowSizeInfo.cellIndex].Width = Data.rowSizeInfo.oldSize;
			Data.rowSizeInfo.editorRow.GetNextVisibleProperties(Data.rowSizeInfo.cellIndex + 1).Width = Data.rowSizeInfo.oldSize2;
			Data.rowSizeInfo.Reset();
		}
		private void UpdateMultiEditorRowCellWidth(int cx) {
			cx = CalcDelta(cx);
			Grid.BeginUpdate();
			try {
				int newLeftWidth = Math.Max(MultiEditorRowProperties.MinWidth, Data.rowSizeInfo.oldSize + cx);
				int newRightWidth = (Data.rowSizeInfo.oldSize + Data.rowSizeInfo.oldSize2) - newLeftWidth;
				if(newRightWidth < MultiEditorRowProperties.MinWidth) {
					newLeftWidth -= MultiEditorRowProperties.MinWidth - newRightWidth;
					newRightWidth = MultiEditorRowProperties.MinWidth;
				}
				LockState();
				if(Data.DownHitTest.ValueInfo != null) {
					Data.rowSizeInfo.editorRow.PropertiesCollection[Data.rowSizeInfo.cellIndex].CellWidth = newLeftWidth;
					Data.rowSizeInfo.editorRow.PropertiesCollection[Data.rowSizeInfo.cellIndex + 1].CellWidth = newRightWidth;
				}
				else {
					Data.rowSizeInfo.editorRow.PropertiesCollection[Data.rowSizeInfo.cellIndex].Width = newLeftWidth;
					Data.rowSizeInfo.editorRow.GetNextVisibleProperties(Data.rowSizeInfo.cellIndex + 1).Width = newRightWidth;
				}
			}
			finally {
				UnlockState();
				if(Data.rowSizeInfo.editorRow.Height == -1)
					Grid.EndUpdate();
				else {
					Grid.CancelUpdate();
					Grid.InvalidateRow(Data.rowSizeInfo.editorRow);
				}
			}
		}
		public override sealed VGridState State { get { return VGridState.MultiEditorRowCellSizing; } }
	}
	public class HeaderPanelSizingState : ControlState {
		public HeaderPanelSizingState(BaseHandler handler) : base(handler) {}
		public override void MouseMove(MouseEventArgs e) {
			VGridHitTest ht = CalcHitTest(new Point(e.X, e.Y));
			UpdateRowHeaderPanelWidth(ht.PtMouse.X - Data.DownHitTest.PtMouse.X);
		}
		public override void MouseUp(MouseEventArgs e) {
			if((e.Button & MouseButtons.Left) == 0) return;
			UpdateRowHeaderPanelWidth(e.X - Data.DownHitTest.PtMouse.X);
			ChangeState(VGridState.Regular);
			base.MouseUp(e);
		}
		protected override void SetDefaultState() {
			base.SetDefaultState();
			Grid.RowHeaderWidth = Data.rowSizeInfo.oldSize;
			Data.rowSizeInfo.Reset();
		}
		private void UpdateRowHeaderPanelWidth(int cx) {
			LockState();
			try {
				Grid.RowHeaderWidth = Data.rowSizeInfo.oldSize + Convert.ToInt32(CalcDelta(cx) * Data.rowSizeInfo.scaleCoeff);
			}
			finally { UnlockState(); }
		}
		public override sealed VGridState State { get { return VGridState.HeaderPanelSizing; } }
	}
	public class RecordSizingState : ControlState {
		public RecordSizingState(BaseHandler handler) : base(handler) {}
		public override void MouseMove(MouseEventArgs e) {
			UpdateRecordWidth(e.X - Data.DownHitTest.PtMouse.X);
		}
		public override void MouseUp(MouseEventArgs e) {
			if((e.Button & MouseButtons.Left) == 0)
				return;
			EndSizing(e.X - Data.DownHitTest.PtMouse.X);
			base.MouseUp(e);
		}
		void EndSizing(int cx) {
			UpdateRecordWidth(cx);
			Grid.LeftVisibleRecordPixel += Grid.Scroller.LeftVisibleRecordPixelOffset - Grid.Scroller.GetLeftVisibleRecordPixelOffset();
			ChangeState(VGridState.Regular);
		}
		void UpdateRecordWidth(int cx) {
			LockState();
			try {
				int newWidth = Math.Max(Grid.Scroller.LeftVisibleRecordPixelOffset, Data.rowSizeInfo.oldSize + CalcDelta(cx));
				Grid.RecordWidth = newWidth;
			}
			finally { UnlockState(); }
		}
		protected override void SetDefaultState() {
			base.SetDefaultState();
			Grid.RecordWidth = Data.rowSizeInfo.oldSize;
			Data.rowSizeInfo.Reset();
		}
		public override sealed VGridState State { get { return VGridState.RecordSizing; } }
	}
	public abstract class BaseDragState : ControlState {
		public BaseDragState(BaseHandler handler) : base(handler) {}
		protected virtual void ScrollMove(VGridHitTest ht) {
			if(TestDragScrollEffect(ht.HitInfoType, Data.DragInfo)) Data.DragInfo.Go();
			else Data.DragInfo.Stop();
		}
		private bool TestDragScrollEffect(HitInfoTypeEnum t, DragInfo di) {
			switch(t) {
				case HitInfoTypeEnum.OutLeftSide: di.cx = -1; return true;
				case HitInfoTypeEnum.OutTopSide: di.cy = -1; return true;
				case HitInfoTypeEnum.OutRightSide: di.cx = 1; return true;
				case HitInfoTypeEnum.OutBottomSide: di.cy = 1; return true;
			}
			return false;
		}
	}
	public class FocusedRowChangingState : BaseDragState {
		public FocusedRowChangingState(BaseHandler handler) : base(handler) {}
		public override void MouseMove(MouseEventArgs e) {
			DoFocusedRowChanging(CalcHitTest(new Point(e.X, e.Y)));
		}
		private void DoFocusedRowChanging(VGridHitTest ht) {
			BaseRow row = ht.Row;
			if(row != null) {
				Data.PressInfo.PressedRow = row;
				RowCellInfo cellInfo = ht.ValueInfo;
				if(cellInfo == null) cellInfo = ht.CaptionInfo;
				SetFocusedRow(Data.PressInfo.PressedRow, cellInfo, Data.PressInfo.IsEditing);
			}
			else ScrollMove(ht);
		}
		protected override void SetDefaultState() {
			base.SetDefaultState();
			Data.DragInfo.Stop();
		}
		public override sealed VGridState State { get { return VGridState.FocusedRowChanging; } }
	}
	public class DisposedState : ControlState {
		public DisposedState(BaseHandler handler) : base(handler) {}
		public override void MouseUp(MouseEventArgs e) {}
		public override void DoubleClick(Point pt) {}
		public override void MouseWheel(MouseWheelScrollClientArgs e) { }
		public override void MouseEnter(Point pt) {}
		public override void MouseLeave() {}
		public override void KeyDown(KeyEventArgs e) {}
		public override void GotFocus() {}
		public override void LostFocus() {}
		public override void LostCapture() {}
		public override bool ProcessChildControlKey(KeyEventArgs e) { return false; }
		protected override void SetDefaultState() {}
		public override sealed VGridState State { get { return VGridState.Disposed; } }
	}
}
