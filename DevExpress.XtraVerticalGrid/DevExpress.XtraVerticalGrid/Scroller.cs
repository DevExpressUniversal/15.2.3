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

using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraVerticalGrid.ViewInfo;
using DevExpress.XtraVerticalGrid.Rows;
using System.Collections;
using DevExpress.Utils.Drawing.Animation;
using System.Collections.Generic;
namespace DevExpress.XtraVerticalGrid.Internal {
	[ToolboxItem(false)]
	public class VGridVertScrollBar : DevExpress.XtraEditors.VScrollBar, IScrollStyleChangeListener {
		internal VGridVertScrollBar() {
			this.BackColor = SystemColors.ScrollBar;
		}
		protected override void OnMouseEnter(EventArgs e) {
			this.Cursor = Cursors.Arrow;
			base.OnMouseEnter(e);
		}
		void IScrollStyleChangeListener.OnScrollStyleChanged(VGridScrollStylesController styleController) {
			this.BeginUpdate();
			try {
				this.ForeColor = styleController.ForeColor;
				this.BackColor = styleController.BackColor;
			}
			finally {
				this.EndUpdate();
			}
		}
	}
	[ToolboxItem(false)]
	public class VGridHorzScrollBar : DevExpress.XtraEditors.HScrollBar, IScrollStyleChangeListener {
		internal VGridHorzScrollBar() {
			this.BackColor = SystemColors.ScrollBar;
		}
		protected override void OnMouseEnter(EventArgs e) {
			this.Cursor = Cursors.Arrow;
			base.OnMouseEnter(e);
		}
		void IScrollStyleChangeListener.OnScrollStyleChanged(VGridScrollStylesController styleController) {
			this.BeginUpdate();
			try {
				this.ForeColor = styleController.ForeColor;
				this.BackColor = styleController.BackColor;
			}
			finally {
				this.EndUpdate();
			}
		}
	}
	public class ScrollInfo {
		internal readonly VGridHorzScrollBar HScroll;
		internal readonly VGridVertScrollBar VScroll;
		public readonly VGridScrollStylesController scrollStyleController;
		private bool hScrollVisible, vScrollVisible;
		public ScrollInfo() {
			this.scrollStyleController = new VGridScrollStylesController();
			this.HScroll = new VGridHorzScrollBar();
			this.VScroll = new VGridVertScrollBar();
			this.scrollStyleController.AddListener(HScroll);
			this.scrollStyleController.AddListener(VScroll);
			this.HScroll.SetVisibility(false);
			this.HScroll.Anchor = AnchorStyles.None;
			this.VScroll.SetVisibility(false);
			this.VScroll.Anchor = AnchorStyles.None;
			this.hScrollVisible = this.vScrollVisible = false;
			ScrollBarBase.ApplyUIMode(HScroll);
			ScrollBarBase.ApplyUIMode(VScroll);
		}
		public int VScrollWidth { get { return VScroll.GetDefaultVerticalScrollBarWidth(); } }
		public int HScrollHeight { get { return HScroll.GetDefaultHorizontalScrollBarHeight(); } }
		protected internal void CreateHandles() {
			if(VScroll.TouchMode) return;
			IntPtr fakeHandle;
			if(!VScroll.IsHandleCreated) fakeHandle = VScroll.Handle;
			if(!HScroll.IsHandleCreated) fakeHandle = HScroll.Handle;
		}
		public void UpdateScrollerLocation(Rectangle clientRect, bool isRightToLeft = false) {
			if(HScroll.Anchor == AnchorStyles.None) {
				Rectangle hr = new Rectangle(clientRect.X, clientRect.Bottom - (HScroll.IsOverlapScrollBar ? HScrollHeight : 0), clientRect.Width, HScrollHeight);
				HScroll.Bounds = hr;
			}
			if(VScroll.Anchor == AnchorStyles.None) {
				int x = isRightToLeft ? clientRect.Left - VScrollWidth : clientRect.Right - (VScroll.IsOverlapScrollBar ? VScrollWidth : 0);
				Rectangle vr = new Rectangle(x, clientRect.Y, VScrollWidth, clientRect.Height);
				VScroll.Bounds = vr;
			}
		}
		public bool HScrollVisible {
			get { return hScrollVisible; }
			set {
				hScrollVisible = value;
				HScroll.SetVisibility(value);
			}
		}
		public bool VScrollVisible {
			get { return vScrollVisible; }
			set {
				vScrollVisible = value;
				VScroll.SetVisibility(value);
			}
		}
		public ScrollArgs HScrollArgs {
			get { return new ScrollArgs(HScroll); }
			set { value.AssignTo(HScroll); }
		}
		public ScrollArgs VScrollArgs {
			get { return new ScrollArgs(VScroll); }
			set { value.AssignTo(VScroll); }
		}
		protected internal void UpdateStyle(LookAndFeel.UserLookAndFeel lookAndFeel) {
			HScroll.LookAndFeel.ParentLookAndFeel = lookAndFeel;
			VScroll.LookAndFeel.ParentLookAndFeel = lookAndFeel;
		}
		public bool IsOverlapScrollbar { get { return HScroll.IsOverlapScrollBar || this.VScroll.IsOverlapScrollBar; } }
		internal void OnAction(ScrollNotifyAction action) {
			VScroll.OnAction(action);
			HScroll.OnAction(action);
		}
	}
	public class VGridScroller: ISupportAnimatedScroll {
		VGridControlBase grid;
		ScrollInfo scrollInfo;
		VGridScrollStrategy scrollStrategy;
		internal protected VGridScrollStrategy Strategy { get { return scrollStrategy; } }
		VGridControlBase Grid { get { return grid; } }
		protected AnimatedScrollHelper AnimatedScrollHelper { get; private set; }
		public VGridScroller(VGridControlBase grid) {
			this.grid = grid;
			this.scrollInfo = new ScrollInfo();
			ScrollInfo.VScroll.Scroll += new ScrollEventHandler(OnVScroll);
			ScrollInfo.VScroll.ValueChanged += VScrollValueChanged;
			ScrollInfo.HScroll.Scroll += new ScrollEventHandler(OnHScroll);
			ScrollInfo.UpdateStyle(grid.ElementsLookAndFeel);
			this.grid.Controls.AddRange(new Control[] {ScrollInfo.VScroll, ScrollInfo.HScroll});
			this.scrollStrategy = CreateScrollStrategy();
			this.AnimatedScrollHelper = new AnimatedScrollHelper(this);
		}
		public int VScrollValue {
			get { return ScrollInfo.VScroll.Value; }
			set { ScrollInfo.VScroll.Value = CheckVScrollValue(value); }
		}
		public int HScrollValue {
			get { return ScrollInfo.HScroll.Value; }
			set { ScrollInfo.HScroll.Value = CheckHScrollValue(value); }
		}
		public virtual int CheckVScrollValue(int value) {
			int minValue = ScrollInfo.VScroll.Minimum;
			if (value < minValue)
				return minValue;
			int maxValue = Math.Max(minValue, ScrollInfo.VScroll.Maximum - ViewPortHeight);
			if (maxValue < value)
				return maxValue;
			return value;
		}
		public virtual int CheckHScrollValue(int value) {
			int minValue = ScrollInfo.HScroll.Minimum;
			if (value < minValue)
				return minValue;
			int maxValue = Math.Max(minValue, ScrollInfo.HScroll.Maximum - ViewPortWidth);
			if (maxValue < value)
				return maxValue;
			return value;
		}
		protected virtual void VScrollValueChanged(object sender, EventArgs e) {
			TopVisibleRowIndexPixel = ScrollInfo.VScroll.Value;
		}
		public ScrollInfo ScrollInfo { get { return scrollInfo; } }
		internal List<BandInfo> BandsInfo { get { return scrollStrategy.BandsInfo; } }
		BaseViewInfo ViewInfo { get { return grid.ViewInfo; } }
		protected virtual VGridScrollStrategy CreateScrollStrategy() {
			switch(grid.LayoutStyle) {
				case LayoutViewStyle.SingleRecordView: return new SingleRecordScrollStrategy(ViewInfo);
				case LayoutViewStyle.MultiRecordView: return new MultiRecordScrollStrategy(ViewInfo);
				case LayoutViewStyle.BandsView: return new BandsScrollStrategy(ViewInfo);
				default: throw new ApplicationException("Illegal layout view style.");
			}
		}
		protected internal virtual void CreateHandles() {
			ScrollInfo.CreateHandles();
		}
		public virtual void Update() {
			try {
				InvalidateCanScroll();
				UpdateHScrollBar();
				UpdateVScrollBar();
				ViewInfo.ViewRects.ReCalcActualClient();
				ScrollInfo.UpdateScrollerLocation(ScrollInfo.IsOverlapScrollbar ? ViewInfo.ViewRects.ScrollableRectangle : ViewInfo.ViewRects.Client, ViewInfo.IsRightToLeft);
				ViewInfo.ViewRects.ScrollSquare = ScrollSquare;
				int leftRecordPixel = LeftVisibleRecordPixel,
					topRowIndexPixel = TopVisibleRowIndexPixel;
				LeftVisibleRecordPixel = leftRecordPixel;
				TopVisibleRowIndexPixel = topRowIndexPixel;
				if (leftRecordPixel != LeftVisibleRecordPixel || topRowIndexPixel != TopVisibleRowIndexPixel) {
					grid.LayoutChanged();
				}
			}
			finally {
			}
		}
		public virtual void UpdateStyle() { ScrollInfo.UpdateStyle(grid.ElementsLookAndFeel); }
		public virtual void UpdateHScrollBar() {
			ScrollInfo.HScrollVisible = IsNeededHScrollBar;
			if(CanHorizontalScroll) {
				ScrollArgs args = new ScrollArgs();
				args.Maximum = GetTotalRowsWidth();
				args.Value = LeftVisibleRecordPixel;
				args.SmallChange = Strategy.GetHorizontalSmallChange();
				args.LargeChange = Strategy.GetHorizontalLargeChange();
				if(!args.IsEquals(ScrollInfo.HScrollArgs)) {
					args.AssignTo(ScrollInfo.HScroll);
				}
			}
		}
		public virtual void UpdateVScrollBar() {
			ScrollInfo.VScrollVisible = IsNeededVScrollBar;
			if(CanVerticalScroll) {
				ScrollArgs args = new ScrollArgs();
				args.Maximum = GetTotalRowsHeight();
				args.Value = TopVisibleRowIndexPixel;
				args.SmallChange = 10;
				args.LargeChange = ViewPortHeight;
				if(!args.IsEquals(ScrollInfo.VScrollArgs)) {
					args.AssignTo(ScrollInfo.VScroll);
				}
			}
		}
		void InvalidateCanScroll() {
			this.canVerticalScroll = null;
		}
		int GetTotalRowsHeight() {
			return scrollStrategy.GetVisibleCountInternal(grid.NotFixedRows, 0, int.MaxValue, 1, i => i < grid.NotFixedRows.Count).Height;
		}
		int GetTotalRowsWidth() {
			return Strategy.GetTotalRowsWidth();
		}
		internal bool UpdateVScrollBarOnExpandRow() {
			if(ScrollInfo.VScrollVisible) {
				UpdateVScrollBar();
				return true;
			}
			return false;
		}
		public void VertScrollPixel(int pixelCount) {
			TopVisibleRowIndexPixel += pixelCount;
		}
		public void HorzScrollPixel(int pixelCount) {
			LeftVisibleRecordPixel += pixelCount;
		}
		public void VertScroll(int rowsCount) {
			TopVisibleRowIndex += rowsCount;
		}
		public void HorzScroll(int recordsCount) {
			LeftVisibleRecord += recordsCount;
		}
		public void SystemColorsChanged() {
			ScrollInfo.HScroll.BackColor = SystemColors.ScrollBar;
			ScrollInfo.VScroll.BackColor = SystemColors.ScrollBar;
		}
		public void LayoutViewStyleChanged() {
			scrollStrategy = CreateScrollStrategy();
			LeftVisibleRecord = grid.FocusedRecord;
			TopVisibleRowIndex = 0;
		}
		public int GetScrollDistanceToMakeVisible(BaseRow row) {
			int notFixedIndex = Grid.NotFixedRows.IndexOf(row);
			if (notFixedIndex == -1)
				return 0;
			int newYCoord = GetYCoord(notFixedIndex);
			int topVisiblePixel = TopVisibleRowIndexPixel;
			if (newYCoord < topVisiblePixel)
				return newYCoord - topVisiblePixel;
			int bottomVisiblePixel = topVisiblePixel + ViewPortHeight - Strategy.GetTotalVisibleRowHeight(row);
			if (bottomVisiblePixel < newYCoord)
				return newYCoord - bottomVisiblePixel;
			return 0;
		}
		public virtual void OnHScroll(object sender, ScrollEventArgs e) {
			if(!CheckScroll(e.Type)) return;
			BeginAllowHideException();
			try {
				LeftVisibleRecordPixel = e.NewValue;
			}
			catch(HideException) {
				e.NewValue = LeftVisibleRecordPixel;
			}
			finally {
				EndAllowHideException();
			}
		}
		public virtual void OnVScroll(object sender, ScrollEventArgs e) {
			if(!CheckScroll(e.Type)) return;
			BeginAllowHideException();
			try {
				TopVisibleRowIndexPixel = e.NewValue;
			}
			catch(HideException) {
				e.NewValue = TopVisibleRowIndexPixel;
			}
			finally {
				EndAllowHideException();
			}
		}
		bool CheckScroll(ScrollEventType scrollType) {
			switch(scrollType) {
				case ScrollEventType.First:
				case ScrollEventType.Last:
				case ScrollEventType.ThumbPosition:
					return false;
			}
			return true;
		}
		void BeginAllowHideException() { grid.ContainerHelper.BeginAllowHideException(); }
		void EndAllowHideException() { grid.ContainerHelper.EndAllowHideException(); }
		internal bool TrySetRecordVisible(int recordIndex) {
			int xCoord = GetXCoord(recordIndex);
			int oldLeftVisibleRecordPixel = LeftVisibleRecordPixel;
			if (xCoord < LeftVisibleRecordPixel)
				LeftVisibleRecordPixel = xCoord;
			if (LeftVisibleRecordPixel + ViewPortWidth < xCoord + ViewInfo.FullRecordValueWidth)
				LeftVisibleRecordPixel = xCoord + ViewInfo.FullRecordValueWidth - ViewPortWidth;
			return LeftVisibleRecordPixel != oldLeftVisibleRecordPixel;
		}
		int ViewPortWidth { get { return Strategy.GetViewPortWidth(); } }
		bool? canVerticalScroll;
		int? leftVisibleRecordPixelOffset;
		public virtual bool IsNeededVScrollBar {
			get {
				if(VScrollNeeding == VScrollNeeding.No) return false;
				if(VScrollNeeding == VScrollNeeding.Yes) return true;
				return CanVerticalScroll;
			}
		}
		public virtual bool CanVerticalScroll {
			get {
				if (canVerticalScroll == null) {
					canVerticalScroll = scrollStrategy.CanVerticalScroll(ViewInfo.ViewRects.ScrollableRectangle.Height);
					if (canVerticalScroll.Value) {
						Grid.ClearAutoHeights();
						ViewInfo.IsReady = false;
						ViewInfo.Calc();
					}
				}
				return canVerticalScroll.Value;
			}
		}
		public virtual bool CanHorizontalScroll {
			get {
				return scrollStrategy.CanHorizontalScroll();
			}
		}
		public virtual bool IsNeededHScrollBar {
			get {
				if(HScrollNeeding == HScrollNeeding.No) return false;
				if(HScrollNeeding == HScrollNeeding.Yes) return true;
				if(grid.VisibleRows.Count == 0) return false;
				return CanHorizontalScroll;
			}
		}
		public virtual Rectangle ScrollSquare {
			get {
				if (!ScrollInfo.IsOverlapScrollbar && IsNeededHScrollBar && IsNeededVScrollBar)
					return new Rectangle(ScrollInfo.HScroll.Right, ScrollInfo.HScroll.Top, ScrollInfo.VScroll.Width, ScrollInfo.HScroll.Height);
				return Rectangle.Empty;
			}
		}
		public HScrollNeeding HScrollNeeding { get { return grid.scrollBehavior.HScrollNeeding; } }
		public VScrollNeeding VScrollNeeding { get { return grid.scrollBehavior.VScrollNeeding; } }
		public virtual HitInfoTypeEnum GetHitTest(Point pt) {
			if(ScrollInfo.HScrollVisible && ScrollInfo.HScroll.Bounds.Contains(pt))
				return HitInfoTypeEnum.HorzScrollBar;
			if(ScrollInfo.VScrollVisible && ScrollInfo.VScroll.Bounds.Contains(pt))
				return HitInfoTypeEnum.VertScrollBar;
			if(ViewInfo.ViewRects.ScrollSquare.Contains(pt))
				return HitInfoTypeEnum.GripPlace;
			return HitInfoTypeEnum.None;
		}
		internal int GetBandIndexByRowIndex(int rowIndex) {
			return scrollStrategy.GetBandIndexByRowIndex(rowIndex);
		}
		internal bool SetRowVisible(BaseRow row) {
			int distance = GetScrollDistanceToMakeVisible(row);
			if (distance == 0)
				return false;
			TopVisibleRowIndexPixel += distance;
			return true;
		}
		internal void SetRowMaxVisible(BaseRow row) {
			if(row == null) return;
			if(!row.Expanded) {
				SetRowVisible(row);
				return;
			}
			scrollStrategy.SetRowMaxVisible(row, this);
		}
		public int ViewPortHeight { get { return Strategy.ScrollRectHeight; } }
		public int LeftVisibleRecordPixel {
			get { return scrollStrategy.LeftVisibleRecordPixel; }
			set {
				grid.BeginLockGridLayout();
				try {
					scrollStrategy.LeftVisibleRecordPixel = value;
				}
				finally {
					grid.CancelLockGridLayout();
				}
			}
		}
		public int TopVisibleRowIndexPixel {
			get { return scrollStrategy.GetTopVisibleRowIndexPixel(); }
			set {
				if(!Grid.AllowPixelScrolling) return;
				grid.BeginLockGridLayout();
				try {
					scrollStrategy.SetTopVisibleRowIndexPixel(value);
				}
				finally {
					grid.CancelLockGridLayout();
				}
			}
		}
		public int TopVisibleRowIndex {
			get { return scrollStrategy.GetTopVisibleRowIndex(); }
			set {
				grid.BeginLockGridLayout();
				try {
					scrollStrategy.SetTopVisibleRowIndex(ViewPortHeight, value);
				}
				finally {
					grid.CancelLockGridLayout();
				}
			}
		}
		public int LeftVisibleRecord {
			get { return scrollStrategy.LeftVisibleRecord; }
			set {
				grid.BeginLockGridLayout();
				try {
					scrollStrategy.LeftVisibleRecord = value;
				}
				finally {
					grid.CancelLockGridLayout();
				}
			}
		}
		public VGridScrollStylesController StylesController { get { return scrollInfo.scrollStyleController; } }
		public int GetTopVisibleRowPixelOffset() {
			return TopVisibleRowIndexPixel - scrollStrategy.GetVisibleCountInternal(grid.NotFixedRows, 0, int.MaxValue, 1, i => i < TopVisibleRowIndex).Height;
		}
		public int LeftVisibleRecordPixelOffset {
			get {
				if (leftVisibleRecordPixelOffset == null)
					leftVisibleRecordPixelOffset = GetLeftVisibleRecordPixelOffset();
				return leftVisibleRecordPixelOffset.Value;
			}
		}
		public int GetLeftVisibleRecordPixelOffset() {
			return LeftVisibleRecordPixel - this.GetVisibleRecordPixelCount(LeftVisibleRecord);
		}
		public void InvalidateBandsInfo() {
			scrollStrategy.Invalidate();
			this.canVerticalScroll = null;
			if (Grid.State != VGridState.RecordSizing)
				this.leftVisibleRecordPixelOffset = null;
		}
		public bool IsScrollAnimationInProgress { get { return AnimatedScrollHelper.Animating; } }
		void ISupportAnimatedScroll.OnScroll(double currentScrollValue) {
			if (Grid.GridDisposing)
				return;
			VScrollValue = (int)Math.Round(currentScrollValue);
		}
		void ISupportAnimatedScroll.OnScrollFinish() {
			Grid.Handler.ScrollFinish();
		}
		public virtual void CancelAnimatedScroll() {
			AnimatedScrollHelper.Cancel();
		}
		int toValue = int.MaxValue;
		public virtual void AnimateScroll(int distance) {
			if (ScrollInfo.VScroll == null)
				return;
			int currentValue = ScrollInfo.VScroll.Value;
			if (!AnimatedScrollHelper.Animating) {
				toValue = currentValue + distance;
			}
			else {
				bool sameDirection = Math.Sign(distance) == Math.Sign(toValue - currentValue);
				if (sameDirection)
					toValue += distance;
				else
					toValue = currentValue + distance;
			}
			float time = Math.Abs(distance) < 30 ? 0.6f : 1.0f;
			AnimatedScrollHelper.Scroll(currentValue, toValue, time, true);
		}
		internal void OnAction(ScrollNotifyAction action) {
			this.scrollInfo.OnAction(action);
		}
		public int GetYCoord(int index) {
			return scrollStrategy.GetVisibleCountInternal(grid.NotFixedRows, 0, int.MaxValue, 1, i => i < index).Height;
		}
		public int GetXCoord(int correctedValue) {
			return ViewInfo.FullRecordValueWidth * correctedValue;
		}
		public int GetVisibleRecordCount(int pixelCount) {
			if (ViewInfo.FullRecordValueWidth <= 0)
				return 0;
			return pixelCount / ViewInfo.FullRecordValueWidth;
		}
		public int GetVisibleRecordPixelCount(int count) {
			return count * ViewInfo.FullRecordValueWidth;
		}
	}
	public abstract class VGridScrollStrategy {
		bool isReady;
		BaseViewInfo viewInfo;
		List<BandInfo> bandsInfo = new List<BandInfo>();
		internal List<BandInfo> BandsInfo {
			get {
				if(!IsReady) {
					RecalcBandsInfo();
				}
				return bandsInfo;
			}
		}
		bool IsReady { get { return isReady; } }
		int topVisibleRowIndex;
		int leftVisibleRecord;
		int leftVisibleRecordPixel;
		int topVisibleRowIndexPixel;
		protected VGridScrollStrategy(BaseViewInfo viewInfo) {
			this.viewInfo = viewInfo;
			this.topVisibleRowIndex = 0;
			this.leftVisibleRecord = 0;
		}
		public virtual void Invalidate() {
			this.isReady = false;
		}
		void RecalcBandsInfo() {
			if(IsReady)
				return;
			this.isReady = true;
			RecalcBandsInfoInternal();
		}
		protected abstract void RecalcBandsInfoInternal();
		public abstract int GetBandIndexByRowIndex(int rowIndex);
		public RowsScrollInfo GetVisibleCountInternal(GridRowReadOnlyCollection rows, int beginIndex, int areaHeight, int step, Func<int, bool> indexStopCriterion, bool continueBand = false) {
			int count = 0, rowCount = 0, bandIndex = 0;
			int bandHeight = 0;
			int left = ViewInfo.ViewRects.Client.Left;
			for (int i = beginIndex; indexStopCriterion(i); i += step) {
				BaseRow row = rows[i];
				if (row == null)
					break;
				row.HeaderInfo.Calc(ViewInfo.ViewRects.Client, ViewInfo, rows[i + 1], true, null);
				int currentRowHeight = GetTotalVisibleRowHeight(row);
				int testedBandHeight = bandHeight + currentRowHeight;
				if (testedBandHeight > areaHeight) {
					count += rowCount;
					left += BandWidth + BandInterval;
					bandIndex++;
					if (!continueBand || !CanCalcRowsOnTheNextBand(left, bandIndex)) {
						rowCount = 0;
						break;
					}
					else {
						rowCount = 1;
						bandHeight = currentRowHeight;
						continue;
					}
				}
				bandHeight = testedBandHeight;
				rowCount++;
			}
			count += rowCount;
			return new RowsScrollInfo() { Count = count, Height = bandHeight, BandCount = bandIndex + 1 };
		}
		protected virtual int GetVisibleCount(int beginIndex, int areaHeight, int step, Func<int, bool> indexCriterion) {
			return GetVisibleCountInternal(Grid.NotFixedRows, beginIndex, areaHeight, step, indexCriterion).Count;
		}
		public int GetTotalVisibleRowHeight(BaseRow row) {
			return ViewInfo.RC.HorzLineWidth + ViewInfo.GetVisibleRowHeight(row);
		}
		public int GetVisibleCountFromBottom(int bottomIndex, int height) {
			return GetVisibleCount(bottomIndex, height, -1, CanContinueBackward);
		}
		public int GetVisibleCountFromTop(int topIndex, int height) {
			return GetVisibleCount(topIndex, height, 1, CanContinueForward);
		}
		public virtual BaseRow GetFirstRowByBandIndex(int bandIndex) {
			if(bandIndex < 0 || bandIndex > BandsInfo.Count - 1) return null;
			return ((BandInfo)BandsInfo[bandIndex]).firstRow;
		}
		public virtual int GetCorrectTopIndex(int scrollRectHeight, int value) {
			if(ViewInfo.GetBottomScrollableRow() == null)
				return 0;
			int countFromBottom = GetVisibleCountFromBottom(Grid.NotFixedRows.Count - 1, scrollRectHeight);
			return Math.Max(0, Math.Min(value, Grid.NotFixedRows.Count - countFromBottom));
		}
		public virtual int GetCorrectLeftIndex(int leftIndex) {
			return Math.Max(0, Math.Min(leftIndex, Grid.RecordCount - ViewInfo.CompleteVisibleValuesCount));
		}
		public abstract int VisibleBandsCount { get; }
		public virtual int LeftVisibleRecordPixel {
			get { return leftVisibleRecordPixel; }
			set {
				value = Scroller.CheckHScrollValue(value);
				if (leftVisibleRecordPixel == value)
					return;
				Grid.CloseEditor();
				int oldLeftVisibleRecordPixel = leftVisibleRecordPixel;
				this.leftVisibleRecordPixel = value;
				int oldLeftVisibleRecord = this.leftVisibleRecord;
				SetLeftVisibleRecordCore(Scroller.GetVisibleRecordCount(value));
				if (!Grid.IsUpdateLocked) {
					Grid.RaiseLeftVisibleRecordChanged(leftVisibleRecordPixel, oldLeftVisibleRecordPixel, this.leftVisibleRecord, oldLeftVisibleRecord);
					Grid.InvalidateUpdate();
				}
			}
		}
		public virtual int LeftVisibleRecord {
			get { return leftVisibleRecord; }
			set {
				int correctedValue = GetCorrectLeftIndex(value),
					oldValue = LeftVisibleRecord;
				if(oldValue == correctedValue)
					return;
				Grid.CloseEditor();
				int oldLeftVisibleRecordPixel = LeftVisibleRecordPixel;
				SetLeftVisibleRecordCore(correctedValue);
				this.leftVisibleRecordPixel = Scroller.GetXCoord(correctedValue);
				if(!Grid.IsUpdateLocked) {
					Grid.RaiseLeftVisibleRecordChanged(LeftVisibleRecordPixel, oldLeftVisibleRecordPixel, correctedValue, oldValue);
					Grid.InvalidateUpdate();
				}
			}
		}
		protected void SetLeftVisibleRecord(int value) {
			leftVisibleRecord = value;
		}
		protected void SetTopVisibleRowIndex(int value) {
			topVisibleRowIndex = value;
		}
		protected virtual void SetLeftVisibleRecordCore(int value) {
			Grid.FocusedRecord = value;
			SetLeftVisibleRecord(Grid.DataModeHelper.Position);
		}
		public virtual bool CanVerticalScroll(int scrollHeight) {
			if(Scroller.TopVisibleRowIndexPixel != 0)
				return true;
			return GetVisibleCountInternal(Grid.NotFixedRows, 0, scrollHeight, 1, i => i < Grid.NotFixedRows.Count, false).Count < Grid.NotFixedRows.Count;
		}
		public virtual bool CanHorizontalScroll() { 
			return Grid.RecordCount > 1;
		}
		public virtual int GetTopVisibleRowIndexPixel() { return topVisibleRowIndexPixel; }
		public virtual void SetTopVisibleRowIndexPixel(int value) {
			value = Scroller.CheckVScrollValue(value);
			if (topVisibleRowIndexPixel == value)
				return;
			Grid.CloseEditor();
			int oldTopVisibleRowIndexPixel = topVisibleRowIndexPixel;
			topVisibleRowIndexPixel = value;
			int oldTopVisibleRowIndex = topVisibleRowIndex;
			topVisibleRowIndex =  GetVisibleCountInternal(Grid.NotFixedRows, 0, value, 1, i => i < Grid.NotFixedRows.Count, false).Count;
			if (!Grid.IsUpdateLocked) {
				Grid.RaiseTopVisibleRowIndexChanged(topVisibleRowIndexPixel, oldTopVisibleRowIndexPixel, topVisibleRowIndex, oldTopVisibleRowIndex);
				Grid.InvalidateUpdate();
			}
		}
		public virtual int GetTopVisibleRowIndex() { return topVisibleRowIndex; }
		public virtual void SetTopVisibleRowIndex(int scrollRectHeight, int value) {
			int correctedValue = GetCorrectTopIndex(scrollRectHeight, value),
				oldValue = GetTopVisibleRowIndex();
			if(oldValue == correctedValue)
				return;
			Grid.CloseEditor();
			int oldTopVisibleRowIndexPixel = topVisibleRowIndexPixel;
			topVisibleRowIndex = correctedValue;
			topVisibleRowIndexPixel = GetVisibleCountInternal(Grid.NotFixedRows, 0, int.MaxValue, 1, i => i < topVisibleRowIndex).Height;
			if(!Grid.IsUpdateLocked) {
				Grid.RaiseTopVisibleRowIndexChanged(topVisibleRowIndexPixel, oldTopVisibleRowIndexPixel, correctedValue, oldValue);
				Grid.InvalidateUpdate();
			}
		}
		public virtual void SetRowMaxVisible(BaseRow row, VGridScroller scroller) {
			int childCount;
			int groupHeight = GetFullRowHeight(row, out childCount);
			int clientHeight = BandsInfo.Count * scroller.ViewPortHeight;
			if(groupHeight >= clientHeight)
				SetTopVisibleRowIndex(scroller.ViewPortHeight, Grid.NotFixedRows.IndexOf(row));
			else {
				if(scroller.UpdateVScrollBarOnExpandRow()) {
					int n = GetVisibleCountFromTop(GetTopVisibleRowIndex(), scroller.ViewPortHeight);
					int topIndex = row.VisibleIndex + childCount - n + 1;
					if(GetTopVisibleRowIndex() < topIndex)
						SetTopVisibleRowIndex(scroller.ViewPortHeight, topIndex);
				}
				else {
					BaseRow bottomChild = GetBottomVisibleChild(row);
					if(bottomChild != null && bottomChild != row) {
						int count = GetVisibleCountFromBottom(bottomChild.VisibleIndex, scroller.ViewPortHeight);
						int topIndex = bottomChild.VisibleIndex - count + 1;
						if(GetTopVisibleRowIndex() < topIndex)
							SetTopVisibleRowIndex(scroller.ViewPortHeight, topIndex);
					}
				}
			}
		}
		protected BaseRow GetBottomVisibleChild(BaseRow row) {
			if(row == null || !row.Expanded) return null;
			if(row.ChildRows.LastVisible == null) return null;
			while(row.Expanded && row.HasChildren) {
				BaseRow lastChild = row.ChildRows.LastVisible;
				if(lastChild != null)
					row = lastChild;
				else return row;
			}
			return row;
		}
		protected abstract int BandWidth { get; }
		protected abstract int BandInterval { get; }
		protected virtual bool CanContinueForward(int index) { return index < Grid.NotFixedRows.Count; }
		protected virtual bool CanContinueBackward(int index) { return index > -1; }
		protected virtual bool CanCalcRowsOnTheNextBand(int left, int nextBandIndex) { return !IsBehindRightClientEdge(left + BandWidth); }
		protected bool IsBehindRightClientEdge(int x) { return x > ViewInfo.ViewRects.Client.Right; }
		protected int GetFullRowHeight(BaseRow row, out int childCount) {
			childCount = 0;
			if(row == null || row.Grid == null) return 0;
			int height = GetTotalVisibleRowHeight(row);
			if(row.Expanded && row.HasChildren) {
				CalcRowHeight op = new CalcRowHeight(ViewInfo.RC.HorzLineWidth);
				Grid.RowsIterator.DoLocalOperation(op, row.ChildRows);
				height += op.SumHeight;
				childCount = op.RowCount;
			}
			return height;
		}
		protected VGridScroller Scroller { get { return Grid.Scroller; } }
		protected VGridControlBase Grid { get { return ViewInfo.Grid; } }
		public BaseViewInfo ViewInfo { get { return viewInfo; } }
		public virtual int ScrollRectHeight { get { return ViewInfo.ViewRects.ScrollableRectangle.Height; } }
		public virtual int GetViewPortWidth() {
			return ViewInfo.ValuesWidth;
		}
		public virtual int GetTotalRowsWidth() {
			return Grid.RecordCount * ViewInfo.FullRecordValueWidth;
		}
		public virtual int GetHorizontalLargeChange() {
			return ViewInfo.ValuesWidth;
		}
		public virtual int GetHorizontalSmallChange() {
			return 10;
		}
	}
	public class SingleRecordScrollStrategy : VGridScrollStrategy {
		public SingleRecordScrollStrategy(BaseViewInfo viewInfo) : base(viewInfo) {}
		protected override void RecalcBandsInfoInternal() {
			BandsInfo.Clear();
			BandsInfo.Add(new BandInfo(0, Grid.VisibleRows.Count, -1, Grid.VisibleRows[0]));
		}
		public override int GetBandIndexByRowIndex(int rowIndex) { return 0; }
		public override int VisibleBandsCount { get { return 1; } }
		protected override int BandWidth { get { return ViewInfo.ViewRects.Client.Width + 1; } }
		protected override int BandInterval { get { return 0; } }
		public override int GetHorizontalSmallChange() {
			return ViewInfo.FullRecordValueWidth;
		}
	}
	public class MultiRecordScrollStrategy : VGridScrollStrategy {
		public MultiRecordScrollStrategy(BaseViewInfo viewInfo) : base(viewInfo) {}
		protected override void RecalcBandsInfoInternal() {
			BandsInfo.Clear();
			BandsInfo.Add(new BandInfo(0, Grid.VisibleRows.Count, -1, Grid.VisibleRows[0]));
		}
		public override int GetBandIndexByRowIndex(int rowIndex) { return 0; }
		public override bool CanHorizontalScroll() {
			return ViewInfo.CompleteVisibleValuesCount < Grid.RecordCount || LeftVisibleRecordPixel != 0;
		}
		protected override void SetLeftVisibleRecordCore(int value) {
			SetLeftVisibleRecord(value);
		}
		public override int VisibleBandsCount { get { return 1; } }
		protected override int BandWidth { get { return ViewInfo.ViewRects.Client.Width + 1; } }
		protected override int BandInterval { get { return 0; } }
		protected new MultiRecordViewInfo ViewInfo { get { return (MultiRecordViewInfo)base.ViewInfo; } }
		public override int GetTotalRowsWidth() {
			return Grid.RecordCount * ViewInfo.FullRecordValueWidth - ((Grid.RecordCount > 0) ? ViewInfo.RecordsInterval : 0);
		}
	}
	public class BandsScrollStrategy : VGridScrollStrategy {
		int? scrollRectHeight;
		public BandsScrollStrategy(BaseViewInfo viewInfo) : base(viewInfo) {
		}
		public override int ScrollRectHeight {
			get {
				if (!scrollRectHeight.HasValue)
					scrollRectHeight = GetScrollRectHeight();
				return scrollRectHeight.Value;
			}
		}
		int GetScrollRectHeight() {
			int height = 0;
			for(int i = 0; i < VisibleBandsCount; i++) {
				height += BandsInfo[i].bandHeight - ViewInfo.RC.HorzLineWidth;
			}
			return height;
		}
		public override void Invalidate() {
			base.Invalidate();
			this.scrollRectHeight = null;
		}
		public override int GetHorizontalLargeChange() {
			return ViewInfo.FullRecordValueWidth;
		}
		protected override void RecalcBandsInfoInternal() {
			BandsInfo.Clear();
			int rowCount = 0;
			int height = - Scroller.GetTopVisibleRowPixelOffset();
			int bandIndex = 0;
			int i = GetTopVisibleRowIndex();
			for(; i < Grid.NotFixedRows.Count; i++) {
				BaseRow row = Grid.NotFixedRows[i];
				if (height + GetTotalVisibleRowHeight(row) >= ViewInfo.ViewRects.Client.Height) {
					BaseRow firstRow = Grid.NotFixedRows[i - rowCount];
					if(rowCount == 0) { 
						rowCount = 1;
						firstRow = Grid.NotFixedRows[i];
						height = GetTotalVisibleRowHeight(row) + ViewInfo.RC.HorzLineWidth;
					}
					else
						i--;
					BandInfo bi = new BandInfo(bandIndex, rowCount, height, firstRow);
					BandsInfo.Add(bi);
					height = ViewInfo.RC.HorzLineWidth;
					rowCount = 0;
					bandIndex++;
					continue;
				}
				height += GetTotalVisibleRowHeight(row);
				rowCount++;
			}
			height += ViewInfo.RC.HorzLineWidth;
			if(rowCount != 0) {
				BandsInfo.Add(new BandInfo(bandIndex, rowCount, height, Grid.VisibleRows[i - rowCount]));
			}
		}
		public override bool CanVerticalScroll(int scrollHeight) {
			if (Scroller.TopVisibleRowIndexPixel != 0)
				return true;
			if (Grid.OptionsView.AutoScaleBands)
				return false;
			return GetVisibleCountInternal(Grid.NotFixedRows, 0, scrollHeight, 1, i => i < Grid.NotFixedRows.Count, true).Count < Grid.NotFixedRows.Count;
		}
		public override int GetBandIndexByRowIndex(int rowIndex) {
			for(int i = 0; i < BandsInfo.Count; i++) {
				BandInfo bi = (BandInfo)BandsInfo[i];
				int firstIndex = bi.firstRow.VisibleIndex;
				if(rowIndex >= firstIndex && rowIndex < firstIndex + bi.rowsCount)
					return i;
			}
			return -1;
		}
		public override void SetRowMaxVisible(BaseRow row, VGridScroller scroller) {
			int topIndex = GetBandViewRowMaxVisibleTopIndex(row);
			SetTopVisibleRowIndex(scroller.ViewPortHeight, topIndex);
		}
		public override int GetViewPortWidth() {
			return ViewInfo.FullRecordValueWidth;
		}
		public override int VisibleBandsCount { 
			get {
				if(ViewInfo.ViewRects.BandWidth == 0)
					return 0;
				if (Grid.OptionsView.AutoScaleBands)
					return BandsInfo.Count;
				return Math.Min(BandsInfo.Count, Math.Max(1, ViewInfo.ViewRects.Client.Width / (ViewInfo.ViewRects.BandWidth + Grid.BandsInterval)));
			} 
		}
		protected override bool CanCalcRowsOnTheNextBand(int left, int nextBandIndex) { 
			if(IsBehindRightClientEdge(left + BandWidth)) {
				if(Grid.IsAutoScaleBands) {
					int cx = (left + BandWidth) - ViewInfo.ViewRects.ScrollableRectangle.Right;
					if(cx > nextBandIndex) return false;
				}
				else return false;
			}
			return true; 
		}
		protected int GetBandViewRowMaxVisibleTopIndex(BaseRow row) {
			int firstBandIndex = GetBandIndexByRowIndex(row.VisibleIndex);
			BaseRow bottomChild = GetBottomVisibleChild(row);
			if(bottomChild != null && bottomChild != row) {
				int lastBandIndex = GetBandIndexByRowIndex(bottomChild.VisibleIndex);
				int visBandsCount = VisibleBandsCount;
				if(lastBandIndex - firstBandIndex > visBandsCount) return row.VisibleIndex;
				if(firstBandIndex + visBandsCount > lastBandIndex) {
					firstBandIndex = Math.Max(0, lastBandIndex - visBandsCount + 1);
					return GetFirstRowByBandIndex(firstBandIndex).VisibleIndex;
				}
			}
			return row.VisibleIndex;
		}
		protected override int BandWidth { get { return ViewInfo.ViewRects.BandWidth; } }
		protected override int BandInterval { get { return Grid.BandsInterval; } }
		protected new BandsViewInfo ViewInfo { get { return (BandsViewInfo)base.ViewInfo; } }
	}
	public class RowsScrollInfo {
		public int Count { get; set; }
		public int Height { get; set; }
		public int BandCount { get; set; }
	}
}
