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
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using DevExpress.Accessibility;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Calendar;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ListControls;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
namespace DevExpress.XtraEditors.Popup {
	[ToolboxItem(false)]
	public class PopupLookUpEditForm : CustomBlobPopupForm, IAccessibleGrid {
		protected const int smallDelay = 30, bigDelay = 120;
		int lineX;
		string searchText;
		ILookUpDataFilter filter;
		VScrollBar scrollBar;
		Timer timer;
		object resultValue;
		LookUpPopupHitTest _pressInfo;
		bool isLoaded;
		public PopupLookUpEditForm(LookUpEdit ownerEdit) : base(ownerEdit) {
			SetResultValue(OwnerEdit.EditValue);
			this.searchText = string.Empty;
			this.lineX = -10000;
			this.filter = CreateLookUpFilter();
			this.filter.FilteredListChanged += new EventHandler(Filter_CollectionChanged);
			this.scrollBar = new VScrollBar();
			this.scrollBar.Scroll += new ScrollEventHandler(ScrollBar_Scroll);
			this.scrollBar.ScrollBarAutoSize = true;
			this.scrollBar.LookAndFeel.Assign(Properties.LookAndFeel);
			this.Controls.Add(scrollBar);
			ScrollBarBase.ApplyUIMode(scrollBar);
			this.timer = new Timer();
			this.timer.Interval = bigDelay;
			this._pressInfo = new LookUpPopupHitTest();
			this.isLoaded = false;
		}
#if DXWhidbey
		public virtual void AccessibleNotifyClients(AccessibleEvents accEvent, int objectID, int childID) {
			AccessibilityNotifyClients(accEvent, objectID, childID);
		}
		public static int AccessibleRowId = 1;
		protected override AccessibleObject GetAccessibilityObjectById(int objectId) {
			if(objectId == AccessibleRowId && DXAccessible.ChildCount >= 2) return DXAccessible.Children[1].Accessible;
			return base.GetAccessibilityObjectById(objectId);
		}
#endif		
		protected override void Dispose(bool disposing) {
			if(disposing) {
				timer.Dispose();
				scrollBar.Scroll -= new ScrollEventHandler(ScrollBar_Scroll);
				filter.FilteredListChanged -= new EventHandler(Filter_CollectionChanged);
			}
			base.Dispose(disposing);
		}
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() { return new LookupPopupAccessibleObject(this); }
		ScrollBarBase IAccessibleGrid.HScroll { get { return null; } }
		ScrollBarBase IAccessibleGrid.VScroll { get { return ScrollBar != null && ScrollBar.Visible ? ScrollBar : null; } }
		int IAccessibleGrid.HeaderCount { get { return ViewInfo.Headers.Count; } }
		int IAccessibleGrid.RowCount { get { return ViewInfo.Filter.RowCount; } }
		int IAccessibleGrid.SelectedRow { get { return ViewInfo.SelectedIndex; } }
		int IAccessibleGrid.FindRow(int x, int y) {
			return ViewInfo.GetItemIndexByPoint(new Point(x, y));
		}
		IAccessibleGridRow IAccessibleGrid.GetRow(int index) {
			return ViewInfo.CreateRow(index);
		}
		protected virtual ILookUpDataFilter CreateLookUpFilter() {
			return Properties.DataAdapter;
		}
		protected virtual Size UpdateFormSize(Size sz) {
			Size minSize = MinFormSize;
			return new Size(Math.Max(sz.Width, minSize.Width), sz.Height);
		}
		public override void ShowPopupForm() {
			searchText = string.Empty;
			LoadPopupParams();
			if(Properties.SearchMode == SearchMode.AutoFilter)
				Filter.FilterPrefix = OwnerEdit.AutoSearchText;
			SetSelectedIndex();
			timer.Tick += new EventHandler(Timer_Tick);
			Size = UpdateFormSize(Size);
			if(OwnerEdit != null) Location = OwnerEdit.CalcPopupFormBounds(Size).Location;
			base.ShowPopupForm();
#if DXWhidbey
			AccessibleNotifyClients(AccessibleEvents.SystemMenuPopupStart, -1, -1);
#endif		
		}
		public override void HidePopupForm() {
			timer.Tick -= new EventHandler(Timer_Tick);
			ScrollBar.OnAction(ScrollNotifyAction.Hide);
			SavePopupParams();
			base.HidePopupForm();
			Properties.PropertyStore.Remove(LookUpPropertyNames.BlobSize);
#if DXWhidbey
			AccessibleNotifyClients(AccessibleEvents.SystemMenuPopupEnd, -1, -1);
#endif	
		}
		protected virtual void SavePopupParams() {
			LookUpPopupParams popupParams = CreatePopupParams();
			popupParams.Save(ViewInfo, FormResized);
		}
		protected internal void SetResultValue(object value) {
			resultValue = value;
			ViewInfo.SelectedIndex = -1;
			Invalidate();
		}
		protected virtual void LoadPopupParams() {
			SetResultValue(OwnerEdit.EditValue);
			ViewInfo.ColumnsInfo.Clear();
			LookUpPopupParams popupParams = CreatePopupParams();
			popupParams.Load(this);
			isLoaded = true;
		}
		protected virtual LookUpPopupParams CreatePopupParams() {
			if(OwnerEdit.InplaceType == InplaceType.Standalone) return new LookUpStandalonePopupParams(Properties);
			return new LookUpInplacePopupParams(Properties);
		}
		protected override void LayoutChanged() {
			if(!IsLoaded) LoadPopupParams();
			base.LayoutChanged();
		}
		public override void ProcessKeyDown(KeyEventArgs e) {
			base.ProcessKeyDown(e);
			if(e.Handled) return;
			e.Handled = true;
			int cy = 0;
			switch(e.KeyCode) {
				case Keys.PageUp: cy = -ViewInfo.VisibleRowCount; break;
				case Keys.PageDown: cy = ViewInfo.VisibleRowCount; break;
				case Keys.Up: cy = -1; break;
				case Keys.Down: cy = 1; break;
				case Keys.Home: cy = -Filter.RowCount; break;
				case Keys.End: cy = Filter.RowCount; break;
				case Keys.Return:
					if(!e.Control && !e.Shift) {
						e.Handled = SelectedIndex != -1;
						if(e.Handled)
							EnterValue();
						else {
							OwnerEdit.ProcessPopupAutoSearchValue();
							ShutPopupForm();
						}
					}
					else DoIncrementalSearch('\n', e.Control, e.Shift);
					return;
				default:
					e.Handled = false;
					return;
			}
			SelectedIndex += cy;
		}
		public override void ProcessKeyPress(KeyPressEventArgs e) {
			base.ProcessKeyPress(e);
			if(e.Handled) return;
			e.Handled = DoIncrementalSearch(e.KeyChar, false, false);
		}
		protected virtual bool DoIncrementalSearch(char keyChar, bool ctrl, bool shift) {
			if(!CanSearchInPopup) return false;
			const char backSpaceChar = '\b';
			int startIndex = 0, endIndex = Filter.RowCount, step = 1;
			string text = searchText;
			if(!Char.IsControl(keyChar) || keyChar == backSpaceChar) {
				KeyPressHelper helper = new KeyPressHelper(text, Properties.MaxLength);
				helper.ProcessChar(keyChar);
				text = helper.Text;
			}
			else {
				if(keyChar == '\n' && ctrl) startIndex = SelectedIndex + 1;
				else if(keyChar == '\n' && shift) {
					startIndex = SelectedIndex - 1;
					step = -1;
					endIndex = -1;
				}
				else return false;
			}
			LookUpColumnInfo searchColumn = ViewInfo.AutoSearchHeader.Column;
			text = (Properties.CaseSensitiveSearch ? text : text.ToLower());
			for(int i = startIndex; i != endIndex; i += step) {
				string itemText = Properties.GetValueDisplayText(Properties.ActiveFormat, Properties.DataAdapter.GetCellString(searchColumn, Filter.GetRecordIndex(i)));
				if(!Properties.CaseSensitiveSearch) itemText = itemText.ToLower();
				if(text == itemText.Substring(0, Math.Min(itemText.Length, text.Length))) {
					SelectedIndex = i;
					searchText = text;
					LayoutChanged();
					break;
				}
			}
			Invalidate();
			return true;
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if((e.Button & MouseButtons.Left) == 0) return;
			_pressInfo = ViewInfo.GetHitTest(new Point(e.X, e.Y));
			if(_pressInfo.HitType == LookUpPopupHitType.Row) {
				SelectedIndex = _pressInfo.Index;
				timer.Enabled = true;
			}
			else if(_pressInfo.HitType == LookUpPopupHitType.Header) {
				if(CanSearchInPopup && Properties.HeaderClickMode == HeaderClickMode.AutoSearch) {
					searchText = string.Empty;
					ViewInfo.AutoSearchHeaderIndex = ViewInfo.GetColumnVisibleIndex(_pressInfo.Index);
					LayoutChanged();
				}
			}
			else if(_pressInfo.HitType == LookUpPopupHitType.HeaderEdge) {
				SetSizingLine(e.X, false);
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseMove(ee);
			ScrollBar.OnAction(ScrollNotifyAction.MouseMove);
			if(!ee.Handled) {
				LookUpPopupHitTest ht = ViewInfo.GetHitTest(new Point(e.X, e.Y));
				CheckMouseCursor(ht);
				if(IsHeaderSizing) {
					SetSizingLine(e.X, true);
				}
				else if(_pressInfo.HitType == LookUpPopupHitType.Row) {
					if(ht.HitType == LookUpPopupHitType.Row) SelectedIndex = ht.Index;
					CheckChangeScrollDelay(ht.Point);
				}
				else if(Properties.HotTrackItems && ht.HitType == LookUpPopupHitType.Row)
					SelectedIndex = ht.Index;
			}
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(timer.Enabled && (e.Button & MouseButtons.Left) != 0) {
				int upIndex = ViewInfo.GetItemIndexByPoint(new Point(e.X, e.Y));
				if(upIndex != -1 && upIndex == SelectedIndex) EnterValue();
			}
			if(_pressInfo.HitType == LookUpPopupHitType.Header) {
				LookUpPopupHitTest upTest = ViewInfo.GetHitTest(new Point(e.X, e.Y));
				if(upTest.HitType == _pressInfo.HitType && upTest.Index == _pressInfo.Index &&
					Math.Abs(upTest.Point.X - _pressInfo.Point.X) < SystemInformation.DragSize.Width &&
					Math.Abs(upTest.Point.Y - _pressInfo.Point.Y) < SystemInformation.DragSize.Height)
					OnClickHeader(e, upTest.Index);
			}
			else if(IsHeaderSizing) {
				SetSizingLine(e.X, false);
				ResizeHeader(_pressInfo.Index, Math.Min(Math.Max(e.X, ViewInfo.GetMinPossibleRightCoord(_pressInfo.Index)), ViewInfo.GetMaxPossibleRightCoord(_pressInfo.Index)) - ViewInfo.GetRightCoord(_pressInfo.Index));
			}
			SetDefaultState();
		}
		protected virtual void OnClickHeader(MouseEventArgs e, int headerIndex) {
			if(Properties.HeaderClickMode != HeaderClickMode.Sorting) return;
			LookUpColumnHeaderObjectInfoArgs headerInfo = ViewInfo.GetHeaderInfo(headerIndex);
			LookUpColumnPopupSaveInfo colInfo = ViewInfo.GetColumnSaveInfo(headerInfo.Column);
			if((ModifierKeys & Keys.Control) != 0 && ViewInfo.SortHeaderIndex != ViewInfo.GetColumnIndex(headerIndex)) return;
			if(!Properties.DataAdapter.CanSortColumn(headerInfo.Column)) return;
			ColumnSortOrder newOrder = GetNextSortOrderOnMouseClick(colInfo.SortOrder, ViewInfo.GetColumnIndex(headerIndex));
			if(newOrder == colInfo.SortOrder && ViewInfo.SortHeaderIndex == ViewInfo.GetColumnIndex(headerIndex)) return;
			colInfo.SortOrder = newOrder;
			headerInfo.Column.SortOrder = newOrder;
			ViewInfo.SortHeaderIndex = ViewInfo.GetColumnIndex(headerIndex);
			Properties.DataAdapter.SortByColumn(headerInfo.Column);
			SetSelectedIndex();
			LayoutChanged();
		}
		ColumnSortOrder GetNextSortOrderOnMouseClick(ColumnSortOrder sortOrder, int headerIndex) {
			if((ModifierKeys & Keys.Control) != 0) return ColumnSortOrder.None;
			if(sortOrder != ColumnSortOrder.None && headerIndex != ViewInfo.SortHeaderIndex) return sortOrder;
			ColumnSortOrder result = ColumnSortOrder.None;
			switch(sortOrder) {
				case ColumnSortOrder.None : 
				case ColumnSortOrder.Descending : 
					result = ColumnSortOrder.Ascending;
					break;
				case ColumnSortOrder.Ascending : 
					result = ColumnSortOrder.Descending;
					break;
			}
			return result;
		}
		private void SetDefaultState() { 
			timer.Enabled = false; 
			_pressInfo = new LookUpPopupHitTest();
			lineX = -10000;
		}
		private void SetSizingLine(int x, bool erasePrev) {
			LookUpColumnHeaderObjectInfoArgs header = (LookUpColumnHeaderObjectInfoArgs)ViewInfo.Headers[_pressInfo.Index];
			Point min = new Point(header.Bounds.Left + LookUpColumnInfo.MinColumnWidth, ViewInfo.GridRect.Top);
			Point max = new Point(ViewInfo.GetMaxPossibleRightCoord(_pressInfo.Index), ViewInfo.GridRect.Bottom);
			int X = new Point(x, 0).X;
			X = Math.Max(min.X, Math.Min(X, max.X));
			if(erasePrev) {
				SplitterLineHelper.Default.DrawReversibleLine(Handle, new Point(lineX, min.Y), new Point(lineX, max.Y));
			}
			lineX = X;
			SplitterLineHelper.Default.DrawReversibleLine(Handle, new Point(lineX, min.Y), new Point(lineX, max.Y));
		}
		private void ResizeHeader(int headerIndex, int cx) {
			ViewInfo.ResizeHeader(headerIndex, cx);
			if(ViewInfo.SortHeaderIndex != Properties.SortColumnIndex) { 
				LookUpColumnHeaderObjectInfoArgs headerInfo = ViewInfo.GetHeaderInfo(ViewInfo.SortHeaderIndex);
				if(headerInfo != null)
					Properties.DataAdapter.SortByColumn(headerInfo.Column);
			}
			LayoutChanged();
		}
		private void CheckChangeScrollDelay(Point pt) {
			int distance = 0;
			if(pt.Y < ViewInfo.GridRect.Top) distance = ViewInfo.GridRect.Top - pt.Y;
			if(pt.Y >= ViewInfo.GridRect.Bottom) distance = pt.Y - ViewInfo.GridRect.Bottom;
			timer.Interval = (distance > ViewInfo.RowHeight ? smallDelay : bigDelay);
		}
		protected override void UpdateControlPositionsCore() {
			base.UpdateControlPositionsCore();
			UpdateScrollBar();
		}
		protected virtual void UpdateScrollBar() {
			ScrollBar.SuspendLayout();
			ScrollBar.BeginUpdate();
			try {
				ScrollBar.Minimum = 0;
				ScrollBar.Maximum = Filter.RowCount - 1;
				ScrollBar.SmallChange = 1;
				ScrollBar.LargeChange = ViewInfo.VisibleRowCount;
				ScrollBar.Value = TopIndex;
				Rectangle r = new Rectangle(ViewInfo.IsRightToLeft ? ViewInfo.ContentRect.Left : ViewInfo.ContentRect.Right - ScrollBar.Width, ViewInfo.ContentRect.Top,
											ScrollBar.Width, ViewInfo.ContentRect.Height);
				scrollBar.Bounds = r;
				ScrollBar.SetVisibility(ViewInfo.CanShowScrollBar);
			}
			finally { 
				ScrollBar.EndUpdate(); 
				ScrollBar.ResumeLayout();
			}
		}
		protected virtual void ScrollBar_Scroll(object sender, ScrollEventArgs e) {
			TopIndex = e.NewValue;
		}
		protected virtual void Timer_Tick(object sender, EventArgs e) {
			Point ptMouse = PointToClient(Cursor.Position);
			int dx = 0;
			if(ptMouse.Y < ViewInfo.GridRect.Top) dx = -1;
			if(ptMouse.Y >= ViewInfo.GridRect.Bottom) dx = 1;
			SelectedIndex = Math.Max(0, SelectedIndex + dx);
		}
		protected virtual void Filter_CollectionChanged(object sender, EventArgs e) {
			ViewInfo.SelectedIndex = -1;
			LayoutChanged();
			MakeItemVisible(0);
			OwnerEdit.Refresh(false);
		}
		protected virtual Rectangle CheckSizingBoundsHeight(Rectangle bounds) {
			if(!Sizing) return bounds;
			int cy = bounds.Height - ViewInfo.Bounds.Height;
			int offset = cy - (cy / ViewInfo.RowHeight) * ViewInfo.RowHeight;
			bounds.Height -= offset;
			if(ViewInfo.IsTopSizeBar) bounds.Y += offset;
			return bounds;
		}
		protected virtual void SetSelectedIndex() {
			ViewInfo.SelectedIndex = -1;
			if(!OwnerEdit.IsDisplayTextValid) return;
			for(int i = 0; i < Filter.RowCount; i++) {
				int index = Filter.GetRecordIndex(i);
				object v = Properties.DataAdapter.GetKeyValue(index);
				if(ResultValue == null) {
					if(v == null) {
						SelectedIndex = i;
						return;
					}
				}
				else if(ResultValue.Equals(v)) {
					SelectedIndex = i;
					return;
				}
			}
		}
		protected virtual void CheckMouseCursor(LookUpPopupHitTest ht) {
			bool resetCursor = true;
			if(IsHeaderSizing || (ht.HitType == LookUpPopupHitType.HeaderEdge && _pressInfo.HitType == LookUpPopupHitType.Unknown)) {
				Cursor.Current = Cursors.SizeWE;
				resetCursor = false;
			}
			if(resetCursor)
				Cursor.Current = this.Cursor;
		}
		protected internal virtual void EnterValue() {
			SetResultValue(CurrentValue);
			OwnerEdit.PopupFormResultValueEntered();
			ShutPopupForm();
		}
		protected virtual void ShutPopupForm() {
			Properties.BeginLockFormatParseUpdate();
			try {
				Properties.DataAdapter.FilterPrefix = string.Empty;
				OwnerEdit.ClosePopup();
			}
			finally {
				Properties.CancelLockFormatParseUpdate();
			}
		}
		public void MakeItemVisible(int itemIndex) {
			if(itemIndex < TopIndex) TopIndex = itemIndex;
			else if(itemIndex >= TopIndex + ViewInfo.VisibleRowCount) TopIndex = itemIndex - ViewInfo.VisibleRowCount + 1; 
			else Invalidate();
		}
		protected override PopupBaseFormViewInfo CreateViewInfo() { return new PopupLookUpEditFormViewInfo(this); }
		protected override PopupBaseFormPainter CreatePainter() { return new PopupLookUpFormPainter(); }
		protected internal new PopupLookUpEditFormViewInfo ViewInfo { get { return base.ViewInfo as PopupLookUpEditFormViewInfo; } }
		[DXCategory(CategoryName.Properties)]
		public new RepositoryItemLookUpEdit  Properties { get { return OwnerEdit.Properties; } }
		[Browsable(false)]
		public new LookUpEdit OwnerEdit { get { return base.OwnerEdit as LookUpEdit; } }
		[DXCategory(CategoryName.Behavior)]
		public override bool AllowSizing { get { return Properties.PopupSizeable; } set {} }
		[DXCategory(CategoryName.Behavior)]
		public ILookUpDataFilter Filter { get { return filter; } }
		[DXCategory(CategoryName.Appearance)]
		public override object ResultValue { get { return resultValue; } }
		[DXCategory(CategoryName.Appearance)]
		public virtual object CurrentValue {
			get {
				if(SelectedIndex == -1) return ResultValue;
				return Properties.DataAdapter.GetKeyValue(Filter.GetRecordIndex(SelectedIndex)); 
			}
		}
		[DXCategory(CategoryName.Behavior)]
		public virtual bool CanSearchInPopup { 
			get {
				return (Properties.SearchMode != SearchMode.AutoFilter && Filter.RowCount != 0 && ViewInfo.AutoSearchHeader != null);
			} 
		}
		protected internal string SearchText { get { return searchText; } }
		protected override Size DefaultBlobFormSize { 
			get { 
				return ViewInfo.DefaultContentSize; } 
		}
		protected override Size DefaultMinFormSize { 
			get {
				Size defSize = DefaultBlobFormSize;
				Size res = new Size(200, Math.Min(100, defSize.Height));
				return res; 
			} 
		}
		protected override Size MinFormSize { 
			get { 
				return new Size(Math.Max(base.MinFormSize.Width, ViewInfo.Headers.Count * LookUpColumnInfo.MinColumnWidth) + 
					(scrollBar.IsOverlapScrollBar ? 0 :  scrollBar.Width), 
					base.MinFormSize.Height);
			}
		}
		protected override Rectangle SizingBounds { 
			get { return base.SizingBounds; } 
			set { base.SizingBounds = CheckSizingBoundsHeight(value); } 
		}
		protected internal VScrollBar ScrollBar { get { return scrollBar; } }
		[DXCategory(CategoryName.Appearance)]
		public int SelectedIndex {
			get { return ViewInfo.SelectedIndex; }
			set {
				if(value < 0) value  = 0;
				if(value > Filter.RowCount - 1) value = Filter.RowCount - 1;
				if(SelectedIndex == value) return;
				ViewInfo.SelectedIndex = value;
				searchText = string.Empty;
				ViewInfo.SearchText = string.Empty;
				MakeItemVisible(SelectedIndex);
#if DXWhidbey
				AccessibleNotifyClients(AccessibleEvents.Focus, 1, SelectedIndex);
				AccessibleNotifyClients(AccessibleEvents.Selection, 1, SelectedIndex);
#endif			    
			}
		}
		[DXCategory(CategoryName.Appearance)]
		public int TopIndex {
			get { return ViewInfo.TopIndex; }
			set {
				if(value < 0) value  = 0;
				if(value > Filter.RowCount - ViewInfo.VisibleRowCount) value = Filter.RowCount - ViewInfo.VisibleRowCount;
				if(TopIndex == value) return;
				ViewInfo.TopIndex = value;
				LayoutChanged();
			}
		}
		[DXCategory(CategoryName.Behavior)]
		public int DefaultDropDownRows {
			get {
				object rowsCount = Properties.PropertyStore[LookUpPropertyNames.DropDownRows];
				return (rowsCount == null ? Properties.GetDropDownRows() : (int)rowsCount);
			}
		}
		[DXCategory(CategoryName.Behavior)]
		public int DefaultPopupWidth {
			get {
				object popupWidth = Properties.PropertyStore[LookUpPropertyNames.PopupWidth];
				if(popupWidth != null) return (int)popupWidth;
				int bestWidth = CalcBestWidth();
				if(bestWidth == 0) return Properties.PopupFormSize.Width;
				return bestWidth;
			}
		}
		protected override bool IsPopupWidthStored { get { return Properties.PropertyStore[LookUpPropertyNames.PopupWidth] != null; } }
		public int CalcBestWidth() {
			if(Properties.BestFitMode == BestFitMode.None || IsPopupWidthStored) return 0;
			object bw = Properties.PropertyStore[LookUpPropertyNames.PopupBestWidth];
			if(bw != null) return (int)bw;
			int bestFitWidth = Properties.BestFit() + ViewInfo.CalcBorderSize().Width;
			bestFitWidth = Math.Min(bestFitWidth, Math.Max(100, Screen.GetWorkingArea(OwnerEdit).Width - 100));
			if(Properties.BestFitMode == BestFitMode.BestFit) bestFitWidth = 0;
			Properties.PropertyStore[LookUpPropertyNames.PopupBestWidth] = bestFitWidth;
			return bestFitWidth;
		}
		protected bool IsHeaderSizing { get { return _pressInfo.HitType == LookUpPopupHitType.HeaderEdge; } }
		protected internal bool IsLoaded { get { return isLoaded; } }
	}
	public class PopupLookUpEditFormViewInfo : CustomBlobPopupFormViewInfo {
		protected Rectangle fHeaderRect, fGridRect;
		AppearanceObject appearanceDropDownHeader;
		int topIndex, selectedIndex, autoSearchHeaderIndex, sortHeaderIndex, textHeight, headerHeight;
		ArrayList headers, rows;
		LookUpColumnPopupInfoTable columnsInfo;
		[ThreadStatic]
		static ImageCollection images;
		StringFormat cellFormat;
		HeaderObjectPainter headerPainter;
		string searchText;
		int rowHeight;
		public PopupLookUpEditFormViewInfo(PopupLookUpEditForm form) : base(form) {
			this.ShowSizeBar = Form.Properties.ShowFooter;
			this.textHeight = this.headerHeight = this.topIndex = 0;
			this.rowHeight = 0;
			this.selectedIndex = -1;
			this.appearanceDropDownHeader = new AppearanceObject();
			this.autoSearchHeaderIndex = Form.Properties.AutoSearchColumnIndex;
			this.sortHeaderIndex = Form.Properties.SortColumnIndex;
			this.headerPainter = CreatePainter();
			this.headers = new ArrayList();
			this.rows = new ArrayList();
			this.columnsInfo = new LookUpColumnPopupInfoTable();
			this.searchText = string.Empty;
			this.cellFormat = new StringFormat();
			this.cellFormat.Trimming = StringTrimming.EllipsisCharacter;
			this.cellFormat.FormatFlags |= StringFormatFlags.NoWrap;
			this.cellFormat.LineAlignment = StringAlignment.Center;
			if(IsRightToLeft) 
				this.cellFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
		}
		protected internal bool IsSkined { get { return Form.Properties.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin; } }
		protected virtual HeaderObjectPainter CreatePainter() {
			if(IsSkined) return new SkinHeaderObjectPainter(Form.LookAndFeel);
			return Form.Properties.LookAndFeel.Painter.Header;
		}
		protected LookUpEdit Edit { get { return Form.OwnerEdit; } }
		static ImageCollection Images {
			get {
				if(images == null) {
					images = DevExpress.Utils.Controls.ImageHelper.CreateImageCollectionFromResources(
						"DevExpress.XtraEditors.Images.Find.bmp", typeof(RepositoryItemLookUpEdit).Assembly,
						new Size(11, 10), Color.Magenta);
				}
				return images;
			}
		}
		protected override void Clear() {
			base.Clear();
			this.textHeight = 0;
			this.fHeaderRect = fGridRect = Rectangle.Empty;
			this.Headers.Clear();
			this.Rows.Clear();
			this.SearchText = string.Empty;
			this.headerHeight = 0;
		}
		protected internal IAccessibleGridRow CreateRow(int index) {
			if(index >= TopIndex && index < TopIndex + Rows.Count) {
				LookUpRowInfo row = FindRow(index);
				if(row != null) return row;
			}
			if(index > -1 && index < Filter.RowCount) return new DummyAccessibleRow(this, index);
			return null;
		}
		protected class DummyAccessibleRow : LookUpRowInfo {
			public DummyAccessibleRow(PopupLookUpEditFormViewInfo viewInfo, int rowIndex) : base(viewInfo, rowIndex) {
			}
			protected override AccessibleStates GetState() { return base.GetState() | AccessibleStates.Invisible; }
		}
		protected internal IAccessibleGridHeaderCell CreateHeader(int index) {
			if(index >= Headers.Count) return null;
			return Headers[index] as IAccessibleGridHeaderCell;
		}
		protected virtual AppearanceDefault PopupHeaderDefault { 
			get {
				if(IsSkined) {
					return GridSkins.GetSkin(Form.LookAndFeel)[GridSkins.SkinHeader].GetAppearanceDefault();
				}
				return new AppearanceDefault(GetSystemColor(SystemColors.ControlText), GetSystemColor(SystemColors.Control), HorzAlignment.Center, VertAlignment.Center); } 
		}
		public override void UpdatePaintAppearance() { 
			base.UpdatePaintAppearance();
			AppearanceHelper.Combine(AppearanceDropDownHeader, new AppearanceObject[] { Form.Properties.AppearanceDropDownHeader, StyleController == null ? null :
																						  StyleController.AppearanceDropDownHeader}, PopupHeaderDefault);
			if(IsRightToLeft) {
				AppearanceDropDownHeader.TextOptions.RightToLeft = true;
				AppearanceGrid.TextOptions.RightToLeft = true;
			}
		}
		private void CalcTextHeight() {
			Graphics g = GInfo.AddGraphics(null);
			try {
				textHeight = AppearanceGrid.CalcTextSize(g, "Wg", 0).ToSize().Height;
				CalcRowHeight(g);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected internal bool IsHighlightedItemStyleEnabled() {
			return BaseListBoxControl.GetHighlightedItemStyle(Edit.LookAndFeel, Edit.Properties.HighlightedItemStyle) == HighlightStyle.Skinned;
		}
		protected internal SkinElementInfo GetHighlightedItemSkinInfo() {
			if(!IsHighlightedItemStyleEnabled()) return null;
			SkinElement element = CommonSkins.GetSkin(Edit.LookAndFeel)[CommonSkins.SkinHighlightedItem];
			if(element == null) return null;
			return new SkinElementInfo(element);
		}
		protected virtual void CalcRowHeight(Graphics g) {
			this.rowHeight = TextHeight + 2;
			if(Form.Properties.DropDownItemHeight != 0) rowHeight = Form.Properties.DropDownItemHeight;
			var info = GetHighlightedItemSkinInfo();
			if(info == null) return;
			this.rowHeight += info.Element.ContentMargins.ToPadding().Vertical;
		}
		private void CalcHeaderHeight() {
			LookUpColumnHeaderObjectInfoArgs hArgs = CreateColumnHeader(null);
			GInfo.AddGraphics(null);
			try {
				hArgs.Caption = "Wg";
				hArgs.Cache = GInfo.Cache;
				hArgs.SetAppearance(AppearanceDropDownHeader);
				headerHeight = HeaderPainter.CalcObjectMinBounds(hArgs).Height;
			}
			finally {
				hArgs.Cache = null;
				GInfo.ReleaseGraphics();
			}
		}
		protected override void UpdateFromForm() {
			base.UpdateFromForm();
			this.SearchText = Form.SearchText;
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			CalcTableRects();
			CalcHeaderInfo();
			CalcGridInfo();
		}
		protected virtual void CalcTableRects() {
			fHeaderRect = ContentRect;
			fHeaderRect.Height = HeaderHeight;
			if(HeaderRect.Bottom >= ContentRect.Bottom) return;
			fGridRect = new Rectangle(HeaderRect.Left, HeaderRect.Bottom, HeaderRect.Width, ContentRect.Height - HeaderRect.Height);
			CheckDecreaseTopIndex();
			if(CanShowScrollBar) {
				if(!Form.ScrollBar.IsOverlapScrollBar) {
					fGridRect.Width = Math.Max(0, GridRect.Width - Form.ScrollBar.Width);
					if(IsRightToLeft) fGridRect.X += Form.ScrollBar.Width;
				}
				fHeaderRect.Width = GridRect.Width;
				fHeaderRect.X = fGridRect.X;
			}
		}
		protected virtual void CalcHeaderInfo() {
			CreateColumnHeaders();
		}
		protected virtual void CalcGridInfo() {
			CalcGridRows();
		}
		protected virtual void CreateColumnHeaders() {
			int totalHeadersWidth = 0;
			int counter = 0;
			HeaderObjectInfoArgs args = null;
			for(int i = 0; i < Form.Properties.Columns.Count; i++) {
				LookUpColumnInfo column = GetColumnInfo(i);
				if(!column.Visible) continue;
				args = CreateColumnHeader(column);
				LookUpColumnPopupSaveInfo colInfo = GetColumnSaveInfo(column);
				if(counter == 0) args.HeaderPosition = HeaderPositionKind.Left;
				args.Caption = column.Caption;
				args.SetAppearance(AppearanceDropDownHeader);
				if(IsRightToLeft) args.RightToLeft = true;
				Headers.Add(args);
				if(HeaderHeight > 0) {
					if(counter == GetColumnVisibleIndex(AutoSearchHeaderIndex) && Form.CanSearchInPopup)
						args.InnerElements.Add(new DrawElementInfo(new GlyphElementPainter(), new GlyphElementInfoArgs(Images, 0, null), StringAlignment.Near));
					if(counter == GetColumnIndex(SortHeaderIndex) && colInfo.SortOrder != ColumnSortOrder.None) {
						SortedShapeObjectInfoArgs sortArgs = new SortedShapeObjectInfoArgs();
						sortArgs.Ascending = (colInfo.SortOrder == ColumnSortOrder.Ascending);
						args.InnerElements.Add(new DrawElementInfo(SortedShapeHelper.GetPainter(Form.Properties.LookAndFeel.ActiveLookAndFeel), sortArgs));
					}
				}
				totalHeadersWidth += colInfo.Width;
				counter++;
			}
			if(args != null && counter > 0) args.HeaderPosition = HeaderPositionKind.Right;
			if(totalHeadersWidth == 0) {
				foreach(LookUpColumnInfo column in Form.Properties.Columns)
					column.Width = 20;
				ScaleHeaderPanelRects(0);
			} else
				ScaleHeaderPanelRects(Convert.ToDouble(HeaderRect.Width) / Convert.ToDouble(totalHeadersWidth));
		}
		LookUpColumnInfo GetColumnInfo(int index) {
			return Form.Properties.Columns[GetColumnIndex(index)];
		}
		internal int GetColumnIndex(int index) {
			return IsRightToLeft ? Form.Properties.Columns.Count - index - 1 : index;
		}
		internal int GetColumnVisibleIndex(int index) {
			return IsRightToLeft ? Form.Properties.Columns.VisibleCount - index - 1 : index;
		}
		protected virtual void ScaleHeaderPanelRects(double scaleCoeff) {
			int left = HeaderRect.Left;
			for(int i = 0; i < Headers.Count; i++) {
				LookUpColumnHeaderObjectInfoArgs header = (LookUpColumnHeaderObjectInfoArgs)Headers[i];
				Rectangle r = new Rectangle(left, HeaderRect.Top, 
					Math.Max(GetMinVisibleWidth(i), Convert.ToInt32(GetColumnSaveInfo(header.Column).Width * scaleCoeff)), HeaderRect.Height);
				left += r.Width;
				header.Bounds = r;
			}
			int cx = HeaderRect.Right - left;
			for(int i = Headers.Count - 1; i > -1; i--) {
				LookUpColumnHeaderObjectInfoArgs header = (LookUpColumnHeaderObjectInfoArgs)Headers[i];
				Rectangle r = header.Bounds;
				if(cx != 0 && (r.Width + cx >= GetMinVisibleWidth(i))) {
					r.Width += cx;
					cx = 0;
				}
				else r.X += cx;
				header.Bounds = r;
				HeaderPainter.CalcObjectBounds(header);
			}
		}
		protected internal virtual void ResizeHeader(int headerIndex, int cx) {
			for(int i = 0; i < Headers.Count; i++) {
				LookUpColumnHeaderObjectInfoArgs headerInfo = GetHeaderInfo(i);
				GetColumnSaveInfo(headerInfo.Column).Width = headerInfo.Bounds.Width;
			}
			if(GetColumnSaveInfo(GetHeaderInfo(headerIndex).Column).Width + cx < GetMinVisibleWidth(headerIndex)) cx = GetColumnSaveInfo(GetHeaderInfo(headerIndex).Column).Width - GetMinVisibleWidth(headerIndex);
			if(cx == 0) return;
			GetColumnSaveInfo(GetHeaderInfo(headerIndex).Column).Width += cx;
			do {
				int step = cx / (Headers.Count - (headerIndex + 1));
				if(step == 0) step = (cx > 0 ? 1 : -1);
				bool minimized = true;
				int penaltySum = 0;
				for(int i = headerIndex + 1; i < Headers.Count; i++) {
					int minVisibleWidth = GetMinVisibleWidth(i);
					LookUpColumnPopupSaveInfo colInfo = GetColumnSaveInfo(GetHeaderInfo(i).Column);
					if(colInfo.Width - step < minVisibleWidth) {
						int dummy = minVisibleWidth - colInfo.Width - step;
						penaltySum += dummy;
						cx -= dummy;
						colInfo.Width = minVisibleWidth;
						continue;
					}
					colInfo.Width -= step;
					cx -= step;
					minimized = false;
					if(cx == 0) break;
				}
				cx += penaltySum;
				penaltySum = 0;
				if(minimized) break;
			} while(cx != 0);
			foreach(LookUpColumnHeaderObjectInfoArgs hdr in Headers)
				hdr.Column.Width = GetColumnSaveInfo(hdr.Column).Width;
		}
		protected virtual void CalcGridRows() {
			if(Headers.Count == 0 || GridRect.IsEmpty) return;
			int y = GridRect.Top;
			for(int i = TopIndex; i < Filter.RowCount && y < GridRect.Bottom; i++) {
				LookUpRowInfo ri = CreateRow();
				ri.RowIndex = i;
				ri.Bounds = new Rectangle(GridRect.Left, y, GridRect.Width, RowHeight);
				foreach(LookUpColumnHeaderObjectInfoArgs header in Headers) {
					ri.RecordStrings.Add(GetCellString(header, ri.RowIndex));
				}
				Rows.Add(ri);
				y += ri.Bounds.Height;
			}
		}
		private void CheckDecreaseTopIndex() {
			if(TopIndex == 0) return;
			if(Filter.RowCount - VisibleRowCount < TopIndex)
				TopIndex = Math.Max(0, Filter.RowCount - VisibleRowCount);
		}
		protected virtual LookUpColumnHeaderObjectInfoArgs CreateColumnHeader(LookUpColumnInfo column) {
			return new LookUpColumnHeaderObjectInfoArgs(column);
		}
		protected virtual string GetCellString(LookUpColumnHeaderObjectInfoArgs header, int rowIndex) {
			return Form.Properties.DataAdapter.GetCellString(header.Column, Filter.GetRecordIndex(rowIndex));
		}
		public LookUpColumnHeaderObjectInfoArgs GetHeaderInfo(int index) {
			if(index >= Headers.Count || index < 0) return null;
			return (LookUpColumnHeaderObjectInfoArgs)Headers[index];
		}
		public Rectangle GetCellBounds(LookUpRowInfo row, int cellIndex) {
			LookUpColumnHeaderObjectInfoArgs header = GetHeaderInfo(cellIndex);
			if(header == null) return Rectangle.Empty;
			Rectangle cell = new Rectangle(header.Bounds.Left, row.Bounds.Top, header.Bounds.Width, row.Bounds.Height);
			return cell;
		}
		public LookUpRowInfo FindRow(int index) {
			for(int i = 0; i < Rows.Count; i++) {
				LookUpRowInfo ri = (LookUpRowInfo)Rows[i];
				if(ri.RowIndex == index) return ri;
			}
			return null;
		}
		public LookUpRowInfo FindRow(Point pt) {
			if(!GridRect.Contains(pt)) return null;
			for(int i = 0; i < Rows.Count; i++) {
				LookUpRowInfo ri = (LookUpRowInfo)Rows[i];
				if(ri.Bounds.Contains(pt)) return ri;;
			}
			return null;
		}
		public virtual int GetVisibleRowIndexByPoint(Point pt) {
			if(!GridRect.Contains(pt)) return -1;
			for(int i = 0; i < Rows.Count; i++) {
				LookUpRowInfo ri = (LookUpRowInfo)Rows[i];
				if(ri.Bounds.Contains(pt)) return i;
			}
			return -1;
		}
		public virtual int GetItemIndexByPoint(Point pt) {
			int visIndex = GetVisibleRowIndexByPoint(pt);
			if(visIndex == -1) return -1;
			return visIndex + (TopIndex == -1 ? 0 : TopIndex);
		}
		public LookUpPopupHitTest GetHitTest(Point pt) {
			LookUpPopupHitTest ht = new LookUpPopupHitTest(pt);
			int index = GetItemIndexByPoint(pt);
			if(index != -1) {
				ht.HitType = LookUpPopupHitType.Row;
				ht.Index = index;
				return ht;
			}
			if(HeaderRect.Contains(pt)) {
				for(index = 0; index < Headers.Count; index++) {
					LookUpColumnHeaderObjectInfoArgs header = (LookUpColumnHeaderObjectInfoArgs)Headers[index];
					if(index != Headers.Count - 1) {
						int edgeOffset = ControlUtils.ColumnResizeEdgeSize;
						if(pt.X >= header.Bounds.Right - edgeOffset && pt.X <= header.Bounds.Right + edgeOffset) {
							ht.HitType = LookUpPopupHitType.HeaderEdge;
							ht.Index = index;
							return ht;
						}
					}
					if(header.Bounds.Contains(pt)) {
						ht.HitType = LookUpPopupHitType.Header;
						ht.Index = index;
						return ht;
					}
				}
				return ht;
			}
			return ht;
		}
		protected internal int GetMaxPossibleRightCoord(int headerIndex) {
			return GridRect.Right - (Headers.Count - (headerIndex + 1)) * LookUpColumnInfo.MinColumnWidth;
		}
		protected internal int GetMinPossibleRightCoord(int headerIndex) {
			LookUpColumnHeaderObjectInfoArgs header = (LookUpColumnHeaderObjectInfoArgs)Headers[headerIndex];
			return header.Bounds.Left + GetMinVisibleWidth(headerIndex);
		}
		protected internal int GetRightCoord(int headerIndex) {
			return ((LookUpColumnHeaderObjectInfoArgs)Headers[headerIndex]).Bounds.Right;
		}
		protected internal int GetMinVisibleWidth(int headerIndex) {
			return LookUpColumnInfo.MinColumnWidth;
		}
		protected virtual LookUpRowInfo CreateRow() { return new LookUpRowInfo(this, -1); }
		public int HeaderHeight { 
			get {
				if(!Form.Properties.ShowHeader) return 0;
				if(headerHeight == 0)
					CalcHeaderHeight();
				return headerHeight;
			}
		}
		public int RowHeight {
			get {
				if(TextHeight == 0) return rowHeight;
				return rowHeight;
			}
		}
		public ArrayList Headers { get { return headers; } }
		protected internal virtual LookUpColumnPopupSaveInfo GetColumnSaveInfo(LookUpColumnInfo column) {
			LookUpColumnPopupSaveInfo result = ColumnsInfo[column];
			if(result == null && column != null)
				result = new LookUpColumnPopupSaveInfo(column.Width, column.SortOrder);
			return result;
		}
		public ArrayList Rows { get { return rows; } }
		public LookUpColumnPopupInfoTable ColumnsInfo { get { return columnsInfo; } }
		public Rectangle HeaderRect { get { return fHeaderRect; } }
		public Rectangle GridRect { get { return fGridRect; } }
		public StringFormat CellFormat { get { return cellFormat; } }
		public AppearanceObject AppearanceGrid { get { return PaintAppearanceContent; } }
		public AppearanceObject AppearanceDropDownHeader { get { return appearanceDropDownHeader; } }
		public string SearchText { get { return searchText; } set { searchText = value; } }
		public int TopIndex { get { return topIndex; } set { topIndex = value; } }
		public int SelectedIndex { get { return selectedIndex; } set { selectedIndex = value; } }
		public int AutoSearchHeaderIndex  { get { return autoSearchHeaderIndex; } set { autoSearchHeaderIndex = value; } }
		public int SortHeaderIndex  { get { return sortHeaderIndex; } set { sortHeaderIndex = value; } }
		public new PopupLookUpEditForm Form { get { return base.Form as PopupLookUpEditForm; } }
		public HeaderObjectPainter HeaderPainter { get { return headerPainter; } }
		public ILookUpDataFilter Filter { get { return Form.Filter; } }
		public virtual Size DefaultContentSize {
			get {
				Size res = new Size(Form.DefaultPopupWidth, Form.DefaultDropDownRows * RowHeight + HeaderHeight);
				res.Width = Math.Max(res.Width, Form.OwnerEdit.Width - CalcBorderSize().Width);
				return res;
			}
		}
		public int VisibleRowCount { get { return Math.Min(Filter.RowCount, GridRect.Height / RowHeight); } }
		public bool CanShowScrollBar { get { return (Filter.RowCount != VisibleRowCount) || (TopIndex != 0);} }
		public int TextHeight {
			get {
				if(textHeight == 0) CalcTextHeight();
				return textHeight;
			}
		}
		public LookUpColumnHeaderObjectInfoArgs AutoSearchHeader {
			get {
				int searchIndex = GetColumnVisibleIndex(AutoSearchHeaderIndex);
				if(AutoSearchHeaderIndex > -1 && searchIndex < Headers.Count)
					return (LookUpColumnHeaderObjectInfoArgs)Headers[searchIndex];
				return null;
			}
		}
		public int FixedWidth {
			get {
				int result = 0;
				for(int i = 0; i < Headers.Count; i++) {
					if(((LookUpColumnHeaderObjectInfoArgs)Headers[i]).Bounds.Width == GetMinVisibleWidth(i)) result += GetMinVisibleWidth(i);
				}
				return result;
			}
		}
	}
	public class LookUpColumnHeaderObjectInfoArgs : HeaderObjectInfoArgs {
		LookUpColumnInfo column;
		StringAlignment alignment;
		public LookUpColumnHeaderObjectInfoArgs(LookUpColumnInfo column) { 
			this.column = column; 
			CalcHeaderAlignment();
		}
		public LookUpColumnInfo Column { get { return column; } }
		public StringAlignment Alignment { get { return alignment; } } 
		private void CalcHeaderAlignment() {
			if(Column == null) {
				alignment = StringAlignment.Near;
				return;
			}
			switch(Column.Alignment) {
				case HorzAlignment.Near: alignment = StringAlignment.Near; break;
				case HorzAlignment.Center: alignment = StringAlignment.Center; break;
				case HorzAlignment.Far: alignment = StringAlignment.Far; break;
				case HorzAlignment.Default: alignment = (Column.FormatType == FormatType.Numeric ? StringAlignment.Far : StringAlignment.Near); break;
			}
		}
	}
	public class LookUpRowInfo : IAccessibleGridRow {
		System.Collections.Specialized.StringCollection recordStrings;
		PopupLookUpEditFormViewInfo viewInfo;
		int rowIndex;
		Rectangle bounds;
		public LookUpRowInfo(PopupLookUpEditFormViewInfo viewInfo, int rowIndex) {
			this.viewInfo = viewInfo;
			this.recordStrings = new System.Collections.Specialized.StringCollection();
			this.rowIndex = rowIndex;
			this.bounds = Rectangle.Empty;
		}
		public int RowIndex { get { return rowIndex; } set { rowIndex = value; } }
		public System.Collections.Specialized.StringCollection RecordStrings { get { return recordStrings; } }
		public virtual Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		int IAccessibleGridRow.Index { get { return RowIndex; } }
		string IAccessibleGridRow.GetName() { return string.Format(AccLocalizer.Active.GetLocalizedString(AccStringId.GridRow), RowIndex); }
		string IAccessibleGridRow.GetValue() {
			string text = "";
			for(int i = 0; i < Form.Properties.Columns.VisibleCount; i++) {
				text += GetCellValue(i) + ";";	
			}
			return text;
		}
		PopupLookUpEditForm Form { get { return viewInfo.Form; } }
		AccessibleStates IAccessibleGridRow.GetState() { return GetState(); }
		int IAccessibleGridRow.CellCount { get { return Form.Properties.Columns.VisibleCount; } }
		IAccessibleGridRowCell IAccessibleGridRow.GetCell(int index) { 
			return new LookupPopupAccessibleObject.LookUpCell(this, index);
		}
		public string GetCellName(int cellIndex) {
			LookUpColumnHeaderObjectInfoArgs header = viewInfo.GetHeaderInfo(cellIndex);
			return header == null ? null : header.Caption;
		}
		public string GetCellValue(int cellIndex) {
			if(cellIndex >= RecordStrings.Count) return null;
			return RecordStrings[cellIndex];
		}
		public Rectangle GetCellBounds(int cellIndex) {
			return viewInfo.GetCellBounds(this, cellIndex);
		}
		string IAccessibleGridRow.GetDefaultAction() { return AccLocalizer.Active.GetLocalizedString(AccStringId.GridRowActivate); }
		void IAccessibleGridRow.DoDefaultAction() { Form.SelectedIndex = rowIndex; }
		protected virtual AccessibleStates GetState() { 
			AccessibleStates res = AccessibleStates.Selectable | AccessibleStates.Focusable; 
			if(this.viewInfo.SelectedIndex == RowIndex) res |= AccessibleStates.Selected | AccessibleStates.Focused;
			return res;
		}
	}
	public class LookUpColumnPopupInfoTable : Hashtable {
		public LookUpColumnPopupSaveInfo this[LookUpColumnInfo column] { 
			get { return (LookUpColumnPopupSaveInfo)base[column]; }
			set { base[column] = value; }
		}
	}
	public enum LookUpPopupHitType { Header, HeaderEdge, Row, Unknown }
	public class LookUpPopupHitTest {
		LookUpPopupHitType type;
		Point pt;
		int index;
		public LookUpPopupHitTest() : this(new Point(-10000, -10000)) {}
		public LookUpPopupHitTest(Point pt) {
			this.type = LookUpPopupHitType.Unknown;
			this.pt = pt;
			this.index = -1;
		}
		public LookUpPopupHitType HitType { get { return type; } set { type = value; } }
		public Point Point { get { return pt; } }
		public int Index { get { return index; } set { index = value; } }
	}
	public class PopupLookUpFormPainter : PopupBaseSizeableFormPainter {
		protected override void DrawContent(PopupFormGraphicsInfoArgs info) {
			base.DrawContent(info);
			DrawHeaderPanel(info);
			DrawGrid(info);
			DrawSizeBar(info);
		}
		protected override Rectangle GetSizeBarRect(PopupFormGraphicsInfoArgs info) {
			PopupLookUpEditFormViewInfo vi = (PopupLookUpEditFormViewInfo)info.ViewInfo;
			Rectangle rect = vi.ClientRect;
			rect.Height = rect.Bottom - vi.GridRect.Bottom;
			rect.Y = vi.GridRect.Bottom;
			return rect;
		}
		protected virtual void DrawHeaderPanel(PopupFormGraphicsInfoArgs info) {
			PopupLookUpEditFormViewInfo vi = info.ViewInfo as PopupLookUpEditFormViewInfo;
			if(vi.HeaderRect.IsEmpty || vi.HeaderHeight < 1) return;
			for(int i = 0; i < vi.Headers.Count; i++) {
				ObjectInfoArgs args = (ObjectInfoArgs)vi.Headers[i];
				args.Cache = info.Cache;
				vi.HeaderPainter.DrawObject(args);
				args.Cache = null;
			}
		}
		protected virtual void DrawGrid(PopupFormGraphicsInfoArgs info) {
			PopupLookUpEditFormViewInfo vi = info.ViewInfo as PopupLookUpEditFormViewInfo;
			if(vi.GridRect.IsEmpty) return;
			GraphicsClipState state = info.Cache.ClipInfo.SaveAndSetClip(vi.GridRect);
			try {
				info.Graphics.FillRectangle(vi.AppearanceGrid.GetBackBrush(info.Cache), vi.GridRect);
				DrawRows(info, vi);
				DrawGridLines(info.Cache, vi);
			}
			finally { info.Cache.ClipInfo.RestoreClipRelease(state); }
		}
		protected virtual void DrawRows(PopupFormGraphicsInfoArgs info, PopupLookUpEditFormViewInfo vi) {
			for(int i = 0; i < vi.Rows.Count; i++) {
				DrawRow(info, (LookUpRowInfo)vi.Rows[i], vi);
			}
		}
		protected virtual void DrawRow(PopupFormGraphicsInfoArgs info, LookUpRowInfo row, PopupLookUpEditFormViewInfo vi) {
			Graphics g = info.Graphics;
			Brush textBrush = vi.AppearanceGrid.GetForeBrush(info.Cache);
			if(row.RowIndex == vi.SelectedIndex) {
				if(!DrawHighlightedBar(info, row, vi)) {
					g.FillRectangle(info.Cache.GetSolidBrush(vi.GetSystemColor(SystemColors.Highlight)), row.Bounds);
				}
				textBrush = info.Cache.GetSolidBrush(GetHighlightedForeColor(vi));
			}
			for(int i = 0; i < vi.Headers.Count; i++) {
				LookUpColumnHeaderObjectInfoArgs header = (LookUpColumnHeaderObjectInfoArgs)vi.Headers[i];
				Rectangle headerBounds = header.Bounds;
				Rectangle stringRect = new Rectangle(headerBounds.Left, row.Bounds.Top, headerBounds.Width, row.Bounds.Height);
				var hi = vi.GetHighlightedItemSkinInfo();
				if(hi != null)
					stringRect = RectangleHelper.Deflate(stringRect, hi.Element.ContentMargins.ToPadding());
				else
					stringRect.Inflate(-2, 0);
				vi.CellFormat.Alignment = header.Alignment;
				string cellString = row.RecordStrings[i];
				if(row.RowIndex == vi.SelectedIndex && i == vi.GetColumnVisibleIndex(vi.AutoSearchHeaderIndex) && vi.SearchText.Length > 0)
					DrawSelectedCell(info.Cache, cellString, stringRect, vi);
				else info.Cache.DrawString(cellString, vi.AppearanceGrid.Font,
					textBrush, stringRect, vi.CellFormat);
			}
		}
		public bool DrawHighlightedBar(PopupFormGraphicsInfoArgs info, LookUpRowInfo row, PopupLookUpEditFormViewInfo vi) {
			SkinElementInfo si = vi.GetHighlightedItemSkinInfo();
			if(si == null) return false;
			si.ImageIndex = 1;
			si.Bounds = row.Bounds;
			ObjectPainter.DrawObject(info.Cache, DevExpress.Skins.SkinElementPainter.Default, si);
			return true;
		}
		protected virtual Color GetHighlightedForeColor(PopupLookUpEditFormViewInfo vi) {
			SkinElementInfo si = vi.GetHighlightedItemSkinInfo();
			if(si != null) {
				Color color = GetElementColor(si.Element, new string[] { "SelectedTextColor", "ForeColorSelected" });
				return color.IsEmpty ? vi.AppearanceGrid.ForeColor : color;
			}
			return vi.GetSystemColor(SystemColors.HighlightText);
		}
		static Color GetElementColor(SkinElement element, string[] propNames) {
			object prop = null;
			foreach(string name in propNames) {
				prop = element.Properties[name];
				if(prop != null) return (Color)prop;
			}
			return Color.Empty;
		}
		private void DrawSelectedCell(GraphicsCache cache, string text, Rectangle bounds, PopupLookUpEditFormViewInfo vi) {
			Rectangle fillBounds = bounds;
			fillBounds.Inflate(1, 0);
			bool isHighlightedItemStyleEnabled = vi.IsHighlightedItemStyleEnabled();
			if(!isHighlightedItemStyleEnabled) 
				vi.AppearanceGrid.FillRectangle(cache, fillBounds);
			Color highlightColor = isHighlightedItemStyleEnabled ? CommonColors.GetSystemColor(CommonColors.Highlight) : vi.GetSystemColor(SystemColors.Highlight);  
			Color highlightTextColor = isHighlightedItemStyleEnabled ? CommonColors.GetSystemColor(CommonColors.HighlightText) : vi.GetSystemColor(SystemColors.HighlightText);
			cache.Paint.DrawMultiColorString(cache, bounds, text, vi.SearchText, vi.AppearanceGrid, highlightTextColor, highlightColor, true);
		}
		private void DrawGridLines(GraphicsCache cache, PopupLookUpEditFormViewInfo vi) {
			if(!vi.Form.Properties.ShowLines) return;
			Graphics g = cache.Graphics;
			Brush gridLine = cache.GetSolidBrush(vi.IsSkined ? GridSkins.GetSkin(vi.Form.LookAndFeel)[GridSkins.SkinGridLine].Color.ForeColor : SystemColors.ActiveBorder);
			for(int i = 0; i < vi.Headers.Count; i++) {
				LookUpColumnHeaderObjectInfoArgs header = (LookUpColumnHeaderObjectInfoArgs)vi.Headers[i];
				if(i == vi.Headers.Count - 1) break;
				g.FillRectangle(gridLine, new Rectangle(header.Bounds.Right - 1, vi.GridRect.Top, 1, vi.GridRect.Height));
			}
			if(!vi.IsTopSizeBar) g.FillRectangle(gridLine, new Rectangle(vi.SizeBarRect.Left, vi.SizeBarRect.Top, vi.SizeBarRect.Width, 1));
		}
	}
	public class LookUpPropertyNames {
		public const string
			DropDownRows = "DropDownRows",
			PopupWidth = "PopupWidth",
			PopupBestWidth = "PopupBestWidth",
			AutoSearchColumnIndex = "AutoSearchColumnIndex",
			SortColumnIndex = "SortColumnIndex",
			BlobSize = "BlobSize";
	}
	public class LookUpPopupParams {
		RepositoryItemLookUpEdit item;
		protected LookUpPopupParams(RepositoryItemLookUpEdit item) {
			this.item = item;
		}
		protected void SetInteger(string storedName, int value) { Item.PropertyStore[storedName] = value; }
		public virtual void Save(PopupLookUpEditFormViewInfo viewInfo, bool formResized) {
			SetAutoSearchColumnIndex(ColumnIndexFromViewInfoToRepository(viewInfo.AutoSearchHeaderIndex));
			SetSortColumnIndex(ColumnIndexFromViewInfoToRepository(viewInfo.SortHeaderIndex));
			if(formResized) {
			SetDropDownRows(viewInfo.GridRect.Height / viewInfo.RowHeight);
				SetPopupWidth(viewInfo.ContentRect.Width);
			}
			foreach(LookUpColumnInfo column in viewInfo.ColumnsInfo.Keys) {
				if(Item.Columns.IndexOf(column) == -1) continue;
				SetColumnSaveInfo(column, viewInfo.GetColumnSaveInfo(column));
			}
		}
		public virtual void Load(PopupLookUpEditForm form) {
			PopupLookUpEditFormViewInfo viewInfo = form.ViewInfo;
			foreach(LookUpColumnInfo column in Item.Columns) {
				if(!column.Visible) continue;
				viewInfo.ColumnsInfo[column] = GetColumnSaveInfo(column);
			}
			if(form.IsLoaded) return;
			viewInfo.AutoSearchHeaderIndex = ColumnIndexFromRepositoryToViewInfo(GetAutoSearchColumnIndex());
			viewInfo.SortHeaderIndex = ColumnIndexFromRepositoryToViewInfo(GetSortColumnIndex());
		}
		protected virtual LookUpColumnPopupSaveInfo GetColumnSaveInfo(LookUpColumnInfo column) {
			return new LookUpColumnPopupSaveInfo(column.Width, column.SortOrder);
		}
		protected virtual int GetAutoSearchColumnIndex() { return Item.AutoSearchColumnIndex; }
		protected virtual int GetSortColumnIndex() { return  Item.SortColumnIndex; }
		protected virtual void SetAutoSearchColumnIndex(int value) { Item.AutoSearchColumnIndex = value; }
		protected virtual void SetSortColumnIndex(int value) { Item.SortColumnIndex = value; }
		protected virtual void SetDropDownRows(int value) { Item.DropDownRows = value; }
		protected virtual void SetPopupWidth(int value) { SetInteger(LookUpPropertyNames.PopupWidth, value); }
		protected virtual void SetColumnSaveInfo(LookUpColumnInfo column, LookUpColumnPopupSaveInfo colInfo) { column.Init(colInfo); }
		protected RepositoryItemLookUpEdit Item { get { return item; } }
		protected int ColumnIndexFromViewInfoToRepository(int visibleIndex) {
			int visibleItemsProcessedIndex = -1;
			for(int i = 0; i<Item.Columns.Count; ++i) {
				if(!Item.Columns[i].Visible)
					continue;
				++visibleItemsProcessedIndex;
				if(visibleItemsProcessedIndex == visibleIndex)
					return i;
			}
			return visibleIndex;
		}
		protected int ColumnIndexFromRepositoryToViewInfo(int repositoryItemColumnsIndex) {
			int visibleResult = -1;
			for(int i = 0; i <= repositoryItemColumnsIndex && i<Item.Columns.Count; ++i) {
				if(Item.Columns[i].Visible)
					++visibleResult;
			}
			return visibleResult;
		}
	}
	public class LookUpInplacePopupParams : LookUpPopupParams {
		public LookUpInplacePopupParams(RepositoryItemLookUpEdit item) : base(item) {}
		protected override LookUpColumnPopupSaveInfo GetColumnSaveInfo(LookUpColumnInfo column) {
			if(Item.PropertyStore.Contains(column.FieldName))
				return (LookUpColumnPopupSaveInfo)Item.PropertyStore[column.FieldName];
			return base.GetColumnSaveInfo(column);
		}
		protected override int GetAutoSearchColumnIndex() { return GetInteger(LookUpPropertyNames.AutoSearchColumnIndex, base.GetAutoSearchColumnIndex()); }
		protected override int GetSortColumnIndex() { return GetInteger(LookUpPropertyNames.SortColumnIndex, base.GetSortColumnIndex()); }
		protected override void SetAutoSearchColumnIndex(int value) { SetInteger(LookUpPropertyNames.AutoSearchColumnIndex, value); }
		protected override void SetSortColumnIndex(int value) { SetInteger(LookUpPropertyNames.SortColumnIndex, value); }
		protected override void SetDropDownRows(int value) { SetInteger(LookUpPropertyNames.DropDownRows, value); }
		protected override void SetColumnSaveInfo(LookUpColumnInfo column, LookUpColumnPopupSaveInfo colInfo) { Item.PropertyStore[column.FieldName] = colInfo; }
		int GetInteger(string storedName, int valueInProperties) {
			object index = Item.PropertyStore[storedName];
			return (index == null ? valueInProperties : (int)index);
		}
		public override void Load(PopupLookUpEditForm form) {
			base.Load(form);
			BaseEdit be = form.OwnerEdit;
			be.IncVisualLayoutUpdate();
			try {
			Item.SortColumnIndex = ColumnIndexFromViewInfoToRepository(form.ViewInfo.SortHeaderIndex);
			Item.DataAdapter.SortBySortColumn();
		}
			finally {
				be.DecVisualLayoutUpdate();
			}
		}
	}
	public class LookUpStandalonePopupParams : LookUpPopupParams {
		public LookUpStandalonePopupParams(RepositoryItemLookUpEdit item) : base(item) {}
		public override void Save(PopupLookUpEditFormViewInfo viewInfo, bool formResized) {
			Item.BeginUpdate();
			try {
				base.Save(viewInfo, formResized);
			}
			finally {
				Item.CancelUpdate();
			}
		}
	}
}
