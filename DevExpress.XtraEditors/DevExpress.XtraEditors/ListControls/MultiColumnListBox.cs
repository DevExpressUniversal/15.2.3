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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Accessibility;
using DevExpress.Data;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ListControls;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Repository;
using System.Linq;
namespace DevExpress.XtraEditors {
	internal class MultiColumnListBox : Control, IMultiColumnListBox, IMouseWheelSupport, ISupportLookAndFeel, ISearchControlClient {
		public event EventHandler ItemDoubleClick;
		public event EventHandler SelectedItemChanged;
		int lineX;
		string searchText;
		object selectedItemCore;
		Timer timerCore;
		ILookUpDataFilter filterCore;
		VScrollBar scrollBar;
		PopupBaseFormPainter painterCore;
		MultiColumnListBoxViewInfo viewInfoCore;
		RepositoryItemLookUpEdit propertiesCore;
		LookUpPopupHitTest pressInfoCore;
		UserLookAndFeel lookAndFeelCore;
		ImageList imageListCore;
		public UserLookAndFeel LookAndFeel { get { return lookAndFeelCore; } }
		public ImageList ImageList { get { return imageListCore; } set { imageListCore = value; } }
		internal bool IsImage { get { return ImageList != null && ImageList.Images.Count > 0; } }
		public int DefaultPopupWidth { get { return 5; } }
		protected virtual PopupBaseFormPainter Painter { get { return painterCore; } }
		public int DefaultDropDownRows { get { return 2; } }
		internal virtual MultiColumnListBoxViewInfo ViewInfo { get { return viewInfoCore; } }
		internal virtual Color RowLineColor { get { return viewInfoCore.IsSkined ? VGridSkins.GetSkin(LookAndFeel)[VGridSkins.SkinGridLine].Color.GetBackColor() : SystemColors.ActiveBorder; } }
		public RepositoryItemLookUpEdit Properties {
			get { return propertiesCore; }
		}
		public ILookUpDataFilter Filter { get { return filterCore; } }
		public bool CanSearchInPopup { get { return false; } }
		int hotImetIndexCore;
		public int HotItemIndex {
			get { return hotImetIndexCore; }
			set {
				if(hotImetIndexCore != value) {
					hotImetIndexCore = value;
					LayoutChanged();
				}
			}
		}
		int selectedImetIndexCore;
		public int SelectedItemIndex {
			get { return selectedImetIndexCore; }
			set {
				if(selectedImetIndexCore != value) {
					selectedImetIndexCore = value;
					LayoutChanged();
					UpdateResultValue(value);
				}
			}
		}
		public object SelectedItem {
			get { return selectedItemCore; }
			set {
				if(selectedItemCore != value) {
					selectedItemCore = value;
					LayoutChanged();
					OnSelectedItemChanged();
				}
			}
		}
		void OnSelectedItemChanged() {
			if(SelectedItem != null)
				SelectedItemIndex = GetNewSelectedItemIndex();
			if(SelectedItemChanged != null)
				SelectedItemChanged(SelectedItem, EventArgs.Empty);
		}
		int GetNewSelectedItemIndex() {
			int newIndex = 0;
			if(ViewInfo != null) {
				newIndex = ViewInfo.TopIndex;
				foreach(MultiColumnListBoxRowInfo row in ViewInfo.Rows) {
					if(row.Tag == SelectedItem)
						break;
					newIndex++;
				}
			}
			return newIndex;
		}
		void UpdateResultValue(int index) {
			SelectedItem = Properties.DataAdapter.GetDataSourceRowAtIndex(index);
		}
		public MultiColumnListBox() {
			DoubleBuffered = true;
			lookAndFeelCore = new LookAndFeel.Helpers.ControlUserLookAndFeel(this);
			InitializeScrollBar();
			pressInfoCore = new LookUpPopupHitTest();
			propertiesCore = new RepositoryItemLookUpEdit();
			propertiesCore.LookAndFeel.ParentLookAndFeel = lookAndFeelCore;
			filterCore = Properties.DataAdapter;
			viewInfoCore = CreateViewInfo();
			painterCore = CreatePainter();
			InitializeTimer();
			selectedImetIndexCore = -1;
			hotImetIndexCore = -1;
			isLoaded = false;
			Filter.FilteredListChanged += FilterCollectionChanged;
			LookAndFeel.StyleChanged += UpdateStyle;
		}
		DefaultBoolean allowGlyphSkinning = DefaultBoolean.Default;
		public DefaultBoolean AllowGlyphSkinning {
			get { return allowGlyphSkinning; }
			set {
				if(AllowGlyphSkinning == value) return;
				allowGlyphSkinning = value;
				LayoutChanged();
			}
		}
		public string FilterPrefix {
			get { return Properties.DataAdapter.FilterPrefix; }
			set { Properties.DataAdapter.FilterPrefix = value; }
		}
		void InitializeTimer() {
			timerCore = new Timer();
			timerCore.Interval = 120;
			timerCore.Tick += Timer_Tick;
		}
		void InitializeScrollBar() {
			scrollBar = new DevExpress.XtraEditors.VScrollBar();
			ScrollBar.Scroll += ScrollBar_Scroll;
			ScrollBar.ScrollBarAutoSize = true;
			ScrollBar.LookAndFeel.Assign(LookAndFeel);
			Controls.Add(scrollBar);
		}
		protected virtual PopupBaseSizeableFormPainter CreatePainter() {
			return new MultiColumnListBoxPainter();
		}
		protected virtual MultiColumnListBoxViewInfo CreateViewInfo() {
			return new MultiColumnListBoxViewInfo(this);
		}
		void UpdateStyle(object sender, EventArgs e) {
			LayoutChanged();
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			scrollBar.Scroll -= ScrollBar_Scroll;
			Filter.FilteredListChanged -= FilterCollectionChanged;
			LookAndFeel.StyleChanged -= UpdateStyle;
			timerCore.Tick -= Timer_Tick;
		}
		protected virtual void FilterCollectionChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		public int TopIndex {
			get { return ViewInfo.TopIndex; }
			set {
				if(value < 0) value = 0;
				if(value > Filter.RowCount - ViewInfo.VisibleRowCount) value = Filter.RowCount - ViewInfo.VisibleRowCount;
				if(TopIndex == value) return;
				ViewInfo.TopIndex = value;
				LayoutChanged();
			}
		}
		protected virtual void Timer_Tick(object sender, EventArgs e) {
			Point ptMouse = PointToClient(Cursor.Position);
			int dx = 0;
			if(ptMouse.Y < ViewInfo.GridRect.Top) dx = -1;
			if(ptMouse.Y >= ViewInfo.GridRect.Bottom) dx = 1;
			HotItemIndex = Math.Max(0, HotItemIndex + dx);
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			LayoutChanged();
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
				Rectangle bounds = Bounds;
				bounds.Location = Point.Empty;
				viewInfoCore.CalcViewInfo(e.Graphics, bounds);
				UpdateScrollBar();
				PopupFormGraphicsInfoArgs args = new PopupFormGraphicsInfoArgs(ViewInfo, cache, ClientRectangle);
				Painter.Draw(args);
			}
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if((e.Button & MouseButtons.Left) == 0 && (e.Button & MouseButtons.Right) == 0) return;
			if(!GetStyle(ControlStyles.UserMouse)) Select();
			pressInfoCore = ViewInfo.GetHitTest(new Point(e.X, e.Y));
			if(pressInfoCore.HitType == LookUpPopupHitType.Row) {
				SelectedItemIndex = pressInfoCore.Index;
			}
			else if(pressInfoCore.HitType == LookUpPopupHitType.Header) {
				if(CanSearchInPopup && Properties.HeaderClickMode == HeaderClickMode.AutoSearch) {
					searchText = string.Empty;
					ViewInfo.AutoSearchHeaderIndex = pressInfoCore.Index;
					LayoutChanged();
				}
			}
			else if(pressInfoCore.HitType == LookUpPopupHitType.HeaderEdge) {
				SetSizingLine(e.X, false);
			}
		}
		protected bool TrueSelectItemIndex(int selectIndex) {
			return selectIndex >= 0 && selectIndex <= Filter.RowCount - 1;
		}
		protected void ChangeSelectedItemIndexOnKeyDown(int offset) {
			int newSelectIndex = SelectedItemIndex + offset;
			if(TrueSelectItemIndex(newSelectIndex)) {
				if(!ViewInfo.GetVisibleRowByIndex(newSelectIndex))
					TopIndex += offset;
				SelectedItemIndex = newSelectIndex;
				return;
			}
			SelectedItemIndex = offset < 0 ? 0 : Filter.RowCount - 1;
		}
		protected override bool IsInputKey(Keys keyData) {
			return true;
		}
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			switch(e.KeyCode) {
				case Keys.Up:
					ChangeSelectedItemIndexOnKeyDown(-1);
					break;
				case Keys.Down:
					ChangeSelectedItemIndexOnKeyDown(1);
					break;
				case Keys.PageUp:
					ChangeSelectedItemIndexOnKeyDown(-ViewInfo.VisibleRowCount);
					break;
				case Keys.PageDown:
					ChangeSelectedItemIndexOnKeyDown(ViewInfo.VisibleRowCount);
					break;
			}
		}
		protected override void OnCreateControl() {
			base.OnCreateControl();
			if(!isLoaded)
				LoadPopupParams();
		}
		internal void LayoutChanged() {
			Invalidate();
			Update();
		}
		void IMultiColumnListBox.Update() {
			if(Properties != null) {
				Properties.DataAdapter.RefreshData();
				if(IsInboundPropertyDescriptor)
					Properties.DataAdapter.PopulateColumns();
			}
			if(SelectedItemIndex != -1)
				UpdateResultValue(SelectedItemIndex);
			LayoutChanged();
		}
		bool IsInboundPropertyDescriptor {
			get {
				bool result = false;
				if(Properties.DataAdapter.Helper != null && Properties.DataAdapter.Helper.DescriptorCollection != null) {
					foreach(var item in Properties.DataAdapter.Helper.DescriptorCollection) {
						result |= item is DevExpress.Data.Access.UnboundPropertyDescriptor;
					}
				}
				return result;
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			base.OnMouseMove(ee);
			if(!ee.Handled) {
				LookUpPopupHitTest ht = ViewInfo.GetHitTest(new Point(e.X, e.Y));
				CheckMouseCursor(ht);
				if(IsHeaderSizing) {
					SetSizingLine(e.X, true);
				}
				else if(pressInfoCore.HitType == LookUpPopupHitType.Row) {
					if(ht.HitType == LookUpPopupHitType.Row) HotItemIndex = ht.Index;
					CheckChangeScrollDelay(ht.Point);
				}
				else if(Properties.HotTrackItems && ht.HitType == LookUpPopupHitType.Row)
					HotItemIndex = ht.Index;
				else
					HotItemIndex = -1;
			}
		}
		protected override void OnMouseDoubleClick(MouseEventArgs e) {
			if((e.Button & MouseButtons.Left) == 0) return;
			LookUpPopupHitTest hitTest = ViewInfo.GetHitTest(new Point(e.X, e.Y));
			if(hitTest.HitType == LookUpPopupHitType.Row) {
				SelectedItemIndex = pressInfoCore.Index;
				if(ItemDoubleClick != null)
					ItemDoubleClick(this, EventArgs.Empty);
			}
		}
		public new void OnMouseWheel(MouseEventArgs e) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(e);
			try {
				base.OnMouseWheel(ee);
				if(ee.Handled || !Properties.AllowMouseWheel) return;
				ee.Handled = true;
				TopIndex += (ee.Delta > 0 ? -SystemInformation.MouseWheelScrollLines : SystemInformation.MouseWheelScrollLines);
			}
			finally {
				ee.Sync();
			}
		}
		protected virtual void CheckMouseCursor(LookUpPopupHitTest ht) {
			bool resetCursor = true;
			if(IsHeaderSizing || (ht.HitType == LookUpPopupHitType.HeaderEdge && pressInfoCore.HitType == LookUpPopupHitType.Unknown)) {
				Cursor.Current = Cursors.SizeWE;
				resetCursor = false;
			}
			if(resetCursor)
				Cursor.Current = this.Cursor;
		}
		void SetSizingLine(int x, bool erasePrev) {
			LookUpColumnHeaderObjectInfoArgs header = (LookUpColumnHeaderObjectInfoArgs)ViewInfo.Headers[pressInfoCore.Index];
			Point min = new Point(header.Bounds.Left + 5, ViewInfo.GridRect.Top);
			Point max = new Point(ViewInfo.GetMaxPossibleRightCoord(pressInfoCore.Index), ViewInfo.GridRect.Bottom);
			int X = new Point(x, 0).X;
			X = Math.Max(min.X, Math.Min(X, max.X));
			if(erasePrev) {
				SplitterLineHelper.Default.DrawReversibleLine(Handle, new Point(lineX, min.Y), new Point(lineX, max.Y));
			}
			lineX = X;
			SplitterLineHelper.Default.DrawReversibleLine(Handle, new Point(lineX, min.Y), new Point(lineX, max.Y));
		}
		protected internal virtual void EnterValue() {
			SetResultValue(null);
		}
		protected internal void SetResultValue(object value) {
			selectedItemCore = value;
			ViewInfo.HotItemIndex = -1;
			Invalidate();
		}
		protected override void OnMouseLeave(EventArgs e) {
			HotItemIndex = -1;
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(timerCore.Enabled && (e.Button & MouseButtons.Left) != 0) {
				int upIndex = ViewInfo.GetItemIndexByPoint(new Point(e.X, e.Y));
				if(upIndex != -1 && upIndex == HotItemIndex) EnterValue();
			}
			if(pressInfoCore.HitType == LookUpPopupHitType.Header) {
				LookUpPopupHitTest upTest = ViewInfo.GetHitTest(new Point(e.X, e.Y));
				if(upTest.HitType == pressInfoCore.HitType && upTest.Index == pressInfoCore.Index &&
					Math.Abs(upTest.Point.X - pressInfoCore.Point.X) < SystemInformation.DragSize.Width &&
					Math.Abs(upTest.Point.Y - pressInfoCore.Point.Y) < SystemInformation.DragSize.Height)
					OnClickHeader(e, upTest.Index);
			}
			else if(IsHeaderSizing) {
				SetSizingLine(e.X, false);
				ResizeHeader(pressInfoCore.Index, Math.Min(Math.Max(e.X, ViewInfo.GetMinPossibleRightCoord(pressInfoCore.Index)),
					ViewInfo.GetMaxPossibleRightCoord(pressInfoCore.Index)) - ViewInfo.GetRightCoord(pressInfoCore.Index));
			}
			SetDefaultState();
		}
		private void SetDefaultState() {
			timerCore.Enabled = false;
			pressInfoCore = new LookUpPopupHitTest();
			lineX = -10000;
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
		protected virtual void OnClickHeader(MouseEventArgs e, int headerIndex) {
			if(Properties.HeaderClickMode != HeaderClickMode.Sorting) return;
			LookUpColumnHeaderObjectInfoArgs headerInfo = ViewInfo.GetHeaderInfo(headerIndex);
			LookUpColumnPopupSaveInfo colInfo = ViewInfo.GetColumnSaveInfo(headerInfo.Column);
			if((ModifierKeys & Keys.Control) != 0 && ViewInfo.SortHeaderIndex != headerIndex) return;
			if(!Properties.DataAdapter.CanSortColumn(headerInfo.Column)) return;
			ColumnSortOrder newOrder = GetNextSortOrderOnMouseClick(colInfo.SortOrder, headerIndex);
			if(newOrder == colInfo.SortOrder && ViewInfo.SortHeaderIndex == headerIndex) return;
			colInfo.SortOrder = newOrder;
			headerInfo.Column.SortOrder = newOrder;
			ViewInfo.SortHeaderIndex = headerIndex;
			Properties.DataAdapter.SortByColumn(headerInfo.Column);
			SetSelectedIndex();
			LayoutChanged();
		}
		ColumnSortOrder GetNextSortOrderOnMouseClick(ColumnSortOrder sortOrder, int headerIndex) {
			if((ModifierKeys & Keys.Control) != 0) return ColumnSortOrder.None;
			if(sortOrder != ColumnSortOrder.None && headerIndex != ViewInfo.SortHeaderIndex) return sortOrder;
			ColumnSortOrder result = ColumnSortOrder.None;
			switch(sortOrder) {
				case ColumnSortOrder.None:
				case ColumnSortOrder.Descending:
					result = ColumnSortOrder.Ascending;
					break;
				case ColumnSortOrder.Ascending:
					result = ColumnSortOrder.Descending;
					break;
			}
			return result;
		}
		protected virtual void SetSelectedIndex() {
			for(int i = 0; i < Filter.RowCount; i++) {
				if(SelectedItem != null && SelectedItem.Equals(Properties.DataAdapter.GetDataSourceRowAtIndex(i))) {
					SelectedItemIndex = i;
					return;
				}
			}
		}
		protected bool IsHeaderSizing { get { return pressInfoCore.HitType == LookUpPopupHitType.HeaderEdge; } }
		private void CheckChangeScrollDelay(Point pt) {
			int distance = 0;
			if(pt.Y < ViewInfo.GridRect.Top) distance = ViewInfo.GridRect.Top - pt.Y;
			if(pt.Y >= ViewInfo.GridRect.Bottom) distance = pt.Y - ViewInfo.GridRect.Bottom;
		}
		protected internal DevExpress.XtraEditors.VScrollBar ScrollBar { get { return scrollBar; } }
		protected virtual void UpdateScrollBar() {
			ScrollBar.SuspendLayout();
			ScrollBar.BeginUpdate();
			try {
				ScrollBar.Minimum = 0;
				ScrollBar.Maximum = Filter.RowCount - 1;
				ScrollBar.SmallChange = 1;
				ScrollBar.LargeChange = ViewInfo.VisibleRowCount;
				ScrollBar.Visible = ViewInfo.CanShowScrollBar;
				ScrollBar.Value = TopIndex;
				ScrollBar.Location = new Point(ViewInfo.ContentRect.Right - ScrollBar.Width, ViewInfo.ContentRect.Top);
				ScrollBar.Size = new Size(ScrollBar.Width, ViewInfo.ContentRect.Height);
			}
			finally {
				ScrollBar.EndUpdate();
				ScrollBar.ResumeLayout();
			}
		}
		protected virtual void ScrollBar_Scroll(object sender, ScrollEventArgs e) {
			TopIndex = e.NewValue;
		}
		public void MakeItemVisible(int itemIndex) {
			if(itemIndex < TopIndex) TopIndex = itemIndex;
			else if(itemIndex >= TopIndex + ViewInfo.VisibleRowCount) TopIndex = itemIndex - ViewInfo.VisibleRowCount + 1;
			else Invalidate();
		}
		bool isLoaded;
		protected virtual void LoadPopupParams() {
			ViewInfo.ColumnsInfo.Clear();
			MultiColumnListBoxParams popupParams = CreatePopupParams();
			popupParams.Load(this);
			isLoaded = true;
		}
		protected virtual MultiColumnListBoxParams CreatePopupParams() {
			return new MultiColumnListBoxStandaloneParams(Properties);
		}
		protected virtual void SavePopupParams() {
			MultiColumnListBoxParams popupParams = CreatePopupParams();
			popupParams.Save(ViewInfo, false);
		}
		Control IMultiColumnListBox.MultiColumnListBox {
			get { return this; }
		}
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		protected virtual bool CanFilterItems { get { return this.Properties.DataAdapter != null; } }
		protected virtual void ApplyItemsFilter(DevExpress.Data.Filtering.CriteriaOperator criteriaOperator) {
			if(!CanFilterItems) return;
			this.Properties.DataAdapter.FilterCriteria = criteriaOperator;
			this.Invalidate();
			RaiseSelectionChanged();
		}
		protected virtual void RaiseSelectionChanged() {
			IList visibleRows = this.Properties.DataAdapter.GetAllFilteredAndSortedRows();
			int selectedIndex = visibleRows.IndexOf(this.SelectedItem);
			if(selectedIndex == -1 && visibleRows.Count != 0) {
				SelectedItem = visibleRows[0];
				selectedIndex = 0;
			}
			this.SelectedItemIndex = selectedIndex;
		}
		ISearchControl searchControl;
		bool ISearchControlClient.IsAttachedToSearchControl { get { return searchControl != null; } }
		void ISearchControlClient.ApplyFindFilter(SearchInfoBase searchInfo) {
			SearchCriteriaInfo infoArgs = searchInfo as SearchCriteriaInfo;
			DevExpress.Data.Filtering.CriteriaOperator criteriaOperator = infoArgs != null ? infoArgs.CriteriaOperator : null;
			ApplyItemsFilter(criteriaOperator);
		}
		SearchControlProviderBase ISearchControlClient.CreateSearchProvider() {
			return new MultiColumnListBoxCriteriaProvider(this);
		}
		void ISearchControlClient.SetSearchControl(ISearchControl searchControl) {
			if(this.searchControl == searchControl) return;
			this.searchControl = searchControl;
			ApplyItemsFilter(null);
		}
		class MultiColumnListBoxCriteriaProvider : SearchControlCriteriaProviderBase {
			MultiColumnListBox listBox;
			public MultiColumnListBoxCriteriaProvider(MultiColumnListBox listBox)
				: base() {
				this.listBox = listBox;
			}
			protected override Data.Filtering.CriteriaOperator CalcActiveCriteriaOperatorCore(SearchControlQueryParamsEventArgs args, Data.Helpers.FindSearchParserResults result) {
				string columnName = "Column";
				string[] columnCaptions = new string[listBox.Properties.Columns.Count];
				if(listBox.Properties.DataSource != null) {
					for(int i = 0; i < listBox.Properties.Columns.Count; i++) {
						columnCaptions[i] = !string.IsNullOrEmpty(listBox.Properties.Columns[i].Caption) ? listBox.Properties.Columns[i].Caption : columnName;
					}
				}
				return DevExpress.Data.Filtering.DxFtsContainsHelper.Create(columnCaptions, result, args.FilterCondition);
			}
			protected override Data.Helpers.FindSearchParserResults CalcResultCore(SearchControlQueryParamsEventArgs args) {
				return new DevExpress.Data.Helpers.FindSearchParser().Parse(args.SearchText);
			}
			protected override void DisposeCore() { listBox = null; }
		}
	}
	internal class MultiColumnListBoxViewInfo : CustomBlobPopupFormViewInfo {
		protected Rectangle fHeaderRect, fGridRect;
		AppearanceObject appearanceDropDownHeader;
		int topIndex, autoSearchHeaderIndex, sortHeaderIndex, textHeight, headerHeight;
		ArrayList headers, rows;
		LookUpColumnPopupInfoTable columnsInfo;
		[ThreadStatic]
		static ImageCollection images;
		StringFormat cellFormat;
		HeaderObjectPainter headerPainter;
		MultiColumnListBox formCore;
		static int DefaultSizeBarHeight = 19;
		public MultiColumnListBoxViewInfo(MultiColumnListBox newLookUpControl)
			: base(null) {
			formCore = newLookUpControl;
			this.ShowSizeBar = Form.Properties.ShowFooter;
			this.textHeight = this.headerHeight = this.topIndex = 0;
			this.appearanceDropDownHeader = new AppearanceObject();
			this.autoSearchHeaderIndex = Form.Properties.AutoSearchColumnIndex;
			this.sortHeaderIndex = Form.Properties.SortColumnIndex;
			this.headerPainter = CreatePainter();
			this.headers = new ArrayList();
			this.rows = new ArrayList();
			this.columnsInfo = new LookUpColumnPopupInfoTable();
			this.cellFormat = new StringFormat();
			this.cellFormat.Trimming = StringTrimming.EllipsisCharacter;
			this.cellFormat.FormatFlags |= StringFormatFlags.NoWrap;
			this.cellFormat.LineAlignment = StringAlignment.Center;
		}
		protected override void UpdatePainters() {
			if(Form == null) return;
			switch(Form.Properties.PopupBorderStyle) {
				case PopupBorderStyles.NoBorder: this.fBorderPainter = new EmptyBorderPainter(); break;
				case PopupBorderStyles.Flat: this.fBorderPainter = new HotFlatBorderPainter(); break;
				case PopupBorderStyles.Style3D: this.fBorderPainter = new Border3DRaisedPainter(); break;
				case PopupBorderStyles.Simple: this.fBorderPainter = new SimpleBorderPainter(); break;
				default:
					if(Form.Properties.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin)
						this.fBorderPainter = new SkinPopupFormBorderPainter(Form.Properties.LookAndFeel.ActiveLookAndFeel);
					else
						this.fBorderPainter = new HotFlatBorderPainter();
					break;
			}
			headerPainter = CreatePainter();
		}
		public override AppearanceObject Appearance { get { return Form.Properties.AppearanceDropDown; } }
		public override bool ShowSizeGrip { get { return false; } }
		public override bool ShowSizeBar { get { return false; } set { } }
		public override bool IsTopSizeBar { get { return false; } set { } }
		protected override AppearanceDefault DefaultAppearance {
			get {
				AppearanceDefault res = new AppearanceDefault(GetSystemColor(SystemColors.ControlText), Form.BackColor);
				if(Form != null && Form.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Office2003) {
					res.BackColor = Office2003Colors.Default[Office2003Color.TabPageClient];
				}
				return res;
			}
		}
		public override Color GetSystemColor(Color color) {
			return LookAndFeelHelper.GetSystemColor(Form.LookAndFeel, color);
		}
		protected override void CheckFont(AppearanceDefault appearance) {
			LookAndFeelHelper.CheckFont(Form.LookAndFeel, appearance);
		}
		protected override bool IsLeftSizeGrip { get { return false; } }
		public override SizeGripObjectPainter GripPainter { get { return SizeGripHelper.GetPainter(Form.LookAndFeel); } }
		protected override void CalcButtonSize() {
			return;
		}
		protected override int CalcSizeBarHeight(Size gripSize) {
			return DefaultSizeBarHeight;
		}
		public override void CalcViewInfo(Graphics g, Rectangle bounds) {
			Clear();
			UpdateFromForm();
			UpdatePainters();
			UpdatePaintAppearance();
			GInfo.AddGraphics(g);
			try {
				this.fBounds = bounds;
				CalcRects();
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected override void CalcRects() {
			CalcClientRect(Bounds);
			CalcContentRect(ClientRect);
		}
		protected internal bool IsSkined { get { return Form.Properties.LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin; } }
		protected virtual HeaderObjectPainter CreatePainter() {
			if(IsSkined) return new SkinHeaderObjectPainter(Form.LookAndFeel);
			return Form.Properties.LookAndFeel.Painter.Header;
		}
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
			this.headerHeight = 0;
		}
		protected internal IAccessibleGridRow CreateRow(int index) {
			if(index >= TopIndex && index < TopIndex + Rows.Count) {
				MultiColumnListBoxRowInfo row = FindRow(index);
				if(row != null) return row;
			}
			if(index > -1 && index < Filter.RowCount) return new DummyAccessibleRow(this, index);
			return null;
		}
		protected class DummyAccessibleRow : MultiColumnListBoxRowInfo {
			public DummyAccessibleRow(MultiColumnListBoxViewInfo viewInfo, int rowIndex)
				: base(viewInfo, rowIndex) {
			}
		}
		protected internal IAccessibleGridHeaderCell CreateHeader(int index) {
			if(index >= Headers.Count) return null;
			return Headers[index] as IAccessibleGridHeaderCell;
		}
		protected virtual AppearanceDefault PopupHeaderDefault {
			get {
				if(IsSkined) {
					return GridSkins.GetSkin(Form.LookAndFeel)[GridSkins.SkinHeader].GetAppearanceDefault(Form.LookAndFeel);
				}
				return new AppearanceDefault(GetSystemColor(SystemColors.ControlText),
					GetSystemColor(SystemColors.Control), HorzAlignment.Center, VertAlignment.Center);
			}
		}
		public override void UpdatePaintAppearance() {
			PaintAppearance.Assign(DefaultAppearance);
			AppearanceHelper.Combine(PaintAppearanceContent, new AppearanceObject[] { Appearance, StyleController == null ? null :
				StyleController.AppearanceDropDown }, DefaultAppearanceContent);
			AppearanceHelper.Combine(AppearanceDropDownHeader, new AppearanceObject[] { Form.Properties.AppearanceDropDownHeader, StyleController == null ?
				null : StyleController.AppearanceDropDownHeader}, PopupHeaderDefault);
		}
		void CalcTextHeight() {
			GInfo.AddGraphics(null);
			try { textHeight = AppearanceGrid.CalcTextSize(GInfo.Graphics, "Wg", 0).ToSize().Height; }
			finally { GInfo.ReleaseGraphics(); }
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
				fGridRect.Width = Math.Max(0, GridRect.Width - Form.ScrollBar.Width);
				fHeaderRect.Width = GridRect.Width;
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
			foreach(LookUpColumnInfo column in Form.Properties.Columns) {
				if(!column.Visible) continue;
				args = CreateColumnHeader(column);
				LookUpColumnPopupSaveInfo colInfo = GetColumnSaveInfo(column);
				if(counter == 0) args.HeaderPosition = HeaderPositionKind.Left;
				args.Caption = column.Caption;
				args.SetAppearance(AppearanceDropDownHeader);
				Headers.Add(args);
				if(HeaderHeight > 0) {
					if(counter == AutoSearchHeaderIndex && Form.CanSearchInPopup)
						args.InnerElements.Add(new DrawElementInfo(new GlyphElementPainter(), new GlyphElementInfoArgs(Images, 0, null), StringAlignment.Near));
					if(counter == SortHeaderIndex && colInfo.SortOrder != ColumnSortOrder.None) {
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
			}
			else
				ScaleHeaderPanelRects(Convert.ToDouble(HeaderRect.Width) / Convert.ToDouble(totalHeadersWidth));
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
			if(GetColumnSaveInfo(GetHeaderInfo(headerIndex).Column).Width + cx < GetMinVisibleWidth(headerIndex))
				cx = GetColumnSaveInfo(GetHeaderInfo(headerIndex).Column).Width - GetMinVisibleWidth(headerIndex);
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
				MultiColumnListBoxRowInfo ri = CreateRow();
				ri.RowIndex = i;
				ri.Bounds = new Rectangle(GridRect.Left, y, GridRect.Width, RowHeight);
				foreach(LookUpColumnHeaderObjectInfoArgs header in Headers) {
					ri.RecordStrings.Add(GetCellString(header, ri.RowIndex));
					ri.ImageIndex = Convert.ToInt32(Form.Properties.DataAdapter.GetValueAtIndex("ImageIndex", ri.RowIndex));
					ri.Tag = GetDataSourceRow(ri.RowIndex);
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
		protected virtual object GetDataSourceRow(int rowIndex) {
			return Form.Properties.DataAdapter.GetDataSourceRowAtIndex(rowIndex);
		}
		public LookUpColumnHeaderObjectInfoArgs GetHeaderInfo(int index) {
			if(index >= Headers.Count || index < 0) return null;
			return (LookUpColumnHeaderObjectInfoArgs)Headers[index];
		}
		public Rectangle GetCellBounds(MultiColumnListBoxRowInfo row, int cellIndex) {
			LookUpColumnHeaderObjectInfoArgs header = GetHeaderInfo(cellIndex);
			if(header == null) return Rectangle.Empty;
			Rectangle cell = new Rectangle(header.Bounds.Left, row.Bounds.Top, header.Bounds.Width, row.Bounds.Height);
			return cell;
		}
		public MultiColumnListBoxRowInfo FindRow(int index) {
			for(int i = 0; i < Rows.Count; i++) {
				MultiColumnListBoxRowInfo ri = (MultiColumnListBoxRowInfo)Rows[i];
				if(ri.RowIndex == index) return ri;
			}
			return null;
		}
		public MultiColumnListBoxRowInfo FindRow(Point pt) {
			if(!GridRect.Contains(pt)) return null;
			for(int i = 0; i < Rows.Count; i++) {
				MultiColumnListBoxRowInfo ri = (MultiColumnListBoxRowInfo)Rows[i];
				if(ri.Bounds.Contains(pt)) return ri; ;
			}
			return null;
		}
		public virtual int GetVisibleRowIndexByPoint(Point pt) {
			if(!GridRect.Contains(pt)) return -1;
			for(int i = 0; i < Rows.Count; i++) {
				MultiColumnListBoxRowInfo ri = (MultiColumnListBoxRowInfo)Rows[i];
				if(ri.Bounds.Contains(pt)) return i;
			}
			return -1;
		}
		public virtual Rectangle GetRowBoundsByIndex(int index) {
			MultiColumnListBoxRowInfo info = FindRow(index);
			if(info == null) return Rectangle.Empty;
			return info.Bounds;
		}
		public virtual bool GetVisibleRowByIndex(int index) {
			return GridRect.Contains(GetRowBoundsByIndex(index));
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
			return GridRect.Right - (Headers.Count - (headerIndex + 1)) * 5;
		}
		protected internal int GetMinPossibleRightCoord(int headerIndex) {
			LookUpColumnHeaderObjectInfoArgs header = (LookUpColumnHeaderObjectInfoArgs)Headers[headerIndex];
			return header.Bounds.Left + GetMinVisibleWidth(headerIndex);
		}
		protected internal int GetRightCoord(int headerIndex) {
			return ((LookUpColumnHeaderObjectInfoArgs)Headers[headerIndex]).Bounds.Right;
		}
		protected internal int GetMinVisibleWidth(int headerIndex) {
			return 5;
		}
		protected virtual MultiColumnListBoxRowInfo CreateRow() { return new MultiColumnListBoxRowInfo(this, -1); }
		public int HeaderHeight {
			get {
				if(!Form.Properties.ShowHeader) return 0;
				if(headerHeight == 0)
					CalcHeaderHeight();
				return headerHeight;
			}
		}
		public int RowHeight { get { return (Form.Properties.DropDownItemHeight == 0 ? TextHeight + 4 : Form.Properties.DropDownItemHeight); } }
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
		public int TopIndex { get { return topIndex; } set { topIndex = value; } }
		public int HotItemIndex { get { return Form.HotItemIndex; } set { Form.HotItemIndex = value; } }
		public int SelectedItemIndex { get { return Form.SelectedItemIndex; } set { Form.SelectedItemIndex = value; } }
		public int AutoSearchHeaderIndex { get { return autoSearchHeaderIndex; } set { autoSearchHeaderIndex = value; } }
		public int SortHeaderIndex { get { return sortHeaderIndex; } set { sortHeaderIndex = value; } }
		public new MultiColumnListBox Form { get { return formCore; } }
		public HeaderObjectPainter HeaderPainter { get { return headerPainter; } }
		public ILookUpDataFilter Filter { get { return Form.Filter; } }
		public virtual Size DefaultContentSize {
			get {
				Size res = new Size(Form.DefaultPopupWidth, Form.DefaultDropDownRows * RowHeight + HeaderHeight);
				res.Width = Math.Max(res.Width, Form.Bounds.Width - CalcBorderSize().Width); 
				return res;
			}
		}
		public int VisibleRowCount { get { return Math.Min(Filter.RowCount, GridRect.Height / RowHeight); } }
		public bool CanShowScrollBar { get { return (Filter.RowCount != VisibleRowCount) || (TopIndex != 0); } }
		public int TextHeight {
			get {
				if(textHeight == 0) CalcTextHeight();
				return textHeight;
			}
		}
		public LookUpColumnHeaderObjectInfoArgs AutoSearchHeader {
			get {
				if(AutoSearchHeaderIndex > -1 && AutoSearchHeaderIndex < Headers.Count)
					return (LookUpColumnHeaderObjectInfoArgs)Headers[AutoSearchHeaderIndex];
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
	internal class MultiColumnListBoxRowInfo : IAccessibleGridRow {
		System.Collections.Specialized.StringCollection recordStrings;
		MultiColumnListBoxViewInfo viewInfoCore;
		int rowIndex;
		int imageIndex;
		public virtual ObjectState State { get; set; }
		Rectangle boundsCore;
		object tagCore;
		public MultiColumnListBoxRowInfo(MultiColumnListBoxViewInfo viewInfo, int rowIndex) {
			viewInfoCore = viewInfo;
			this.recordStrings = new System.Collections.Specialized.StringCollection();
			this.rowIndex = rowIndex;
			boundsCore = Rectangle.Empty;
			State = ObjectState.Normal;
			tagCore = null;
		}
		public int ImageIndex { get { return imageIndex; } set { imageIndex = value; } }
		public object Tag { get { return tagCore; } set { tagCore = value; } }
		public int RowIndex { get { return rowIndex; } set { rowIndex = value; } }
		public System.Collections.Specialized.StringCollection RecordStrings { get { return recordStrings; } }
		public virtual Rectangle Bounds { get { return boundsCore; } set { boundsCore = value; } }
		int IAccessibleGridRow.Index { get { return RowIndex; } }
		string IAccessibleGridRow.GetName() { return string.Format(AccLocalizer.Active.GetLocalizedString(AccStringId.GridRow), RowIndex); }
		string IAccessibleGridRow.GetValue() {
			string text = "";
			for(int i = 0; i < Form.Properties.Columns.VisibleCount; i++) {
				text += GetCellValue(i) + ";";
			}
			return text;
		}
		MultiColumnListBox Form { get { return viewInfoCore.Form; } }
		AccessibleStates IAccessibleGridRow.GetState() { return AccessibleStates.None; }
		int IAccessibleGridRow.CellCount { get { return Form.Properties.Columns.VisibleCount; } }
		IAccessibleGridRowCell IAccessibleGridRow.GetCell(int index) {
			return new MultiColumnListBoxCell(this, index);
		}
		public string GetCellName(int cellIndex) {
			LookUpColumnHeaderObjectInfoArgs header = viewInfoCore.GetHeaderInfo(cellIndex);
			return header == null ? null : header.Caption;
		}
		public string GetCellValue(int cellIndex) {
			if(cellIndex >= RecordStrings.Count) return null;
			return RecordStrings[cellIndex];
		}
		public Rectangle GetCellBounds(int cellIndex) {
			return viewInfoCore.GetCellBounds(this, cellIndex);
		}
		string IAccessibleGridRow.GetDefaultAction() { return AccLocalizer.Active.GetLocalizedString(AccStringId.GridRowActivate); }
		void IAccessibleGridRow.DoDefaultAction() { Form.HotItemIndex = rowIndex; }
		public bool Disabled {
			get { return GetState(ObjectState.Disabled); }
			set { SetState(ObjectState.Disabled, value); }
		}
		public bool Hot {
			get { return GetState(ObjectState.Hot); }
			set { SetState(ObjectState.Hot, value); }
		}
		public bool Pressed {
			get { return GetState(ObjectState.Pressed); }
			set { SetState(ObjectState.Pressed, value); }
		}
		public bool Selected {
			get { return GetState(ObjectState.Selected); }
			set { SetState(ObjectState.Selected, value); }
		}
		bool GetState(ObjectState state) {
			return (State & state) != 0;
		}
		void SetState(ObjectState state, bool value) {
			if(value)
				State |= state;
			else
				State &= ~state;
		}
	}
	internal class MultiColumnListBoxCell : IAccessibleGridRowCell {
		MultiColumnListBoxRowInfo row;
		int cellIndex;
		public MultiColumnListBoxCell(MultiColumnListBoxRowInfo row, int cellIndex) {
			this.cellIndex = cellIndex;
			this.row = row;
		}
		public virtual Rectangle Bounds { get { return row.GetCellBounds(cellIndex); } }
		IAccessibleGridRow IRow { get { return row as IAccessibleGridRow; } }
		string IAccessibleGridRowCell.GetDefaultAction() { return IRow.GetDefaultAction(); }
		BaseAccessible IAccessibleGridRowCell.GetEdit() { return null; }
		void IAccessibleGridRowCell.DoDefaultAction() { IRow.DoDefaultAction(); }
		string IAccessibleGridRowCell.GetName() {
			return row.GetCellName(cellIndex);
		}
		string IAccessibleGridRowCell.GetValue() {
			return row.GetCellValue(cellIndex);
		}
		AccessibleStates IAccessibleGridRowCell.GetState() {
			AccessibleStates state = AccessibleStates.ReadOnly;
			if(Bounds.IsEmpty) state |= AccessibleStates.Invisible;
			return state;
		}
	}
	internal class MultiColumnListBoxPainter : PopupBaseSizeableFormPainter {
		protected override void DrawContent(PopupFormGraphicsInfoArgs info) {
			base.DrawContent(info);
			DrawHeaderPanel(info);
			DrawGrid(info);
			DrawSizeBar(info);
		}
		protected override void DrawBorder(PopupFormGraphicsInfoArgs info) {
			MultiColumnListBoxViewInfo vi = info.ViewInfo as MultiColumnListBoxViewInfo;
			if(vi != null && vi.Form != null && vi.IsSkined) {
				Color color = VGridSkins.GetSkin(vi.Form.LookAndFeel)[VGridSkins.SkinGridLine].Color.GetBackColor();
				info.Cache.DrawRectangle(new Pen(color), info.Bounds);
			}
			else
				base.DrawBorder(info);
		}
		protected override Rectangle GetSizeBarRect(PopupFormGraphicsInfoArgs info) {
			MultiColumnListBoxViewInfo vi = (MultiColumnListBoxViewInfo)info.ViewInfo;
			Rectangle rect = vi.ClientRect;
			rect.Height = rect.Bottom - vi.GridRect.Bottom;
			rect.Y = vi.GridRect.Bottom;
			return rect;
		}
		protected virtual void DrawHeaderPanel(PopupFormGraphicsInfoArgs info) {
			MultiColumnListBoxViewInfo vi = info.ViewInfo as MultiColumnListBoxViewInfo;
			if(vi.HeaderRect.IsEmpty || vi.HeaderHeight < 1) return;
			for(int i = 0; i < vi.Headers.Count; i++) {
				ObjectInfoArgs args = (ObjectInfoArgs)vi.Headers[i];
				args.Cache = info.Cache;
				vi.HeaderPainter.DrawObject(args);
				DrawBorderScrollBar(info.Graphics, vi, vi.HeaderRect);
				args.Cache = null;
			}
		}
		protected void DrawBorderScrollBar(Graphics g, MultiColumnListBoxViewInfo info, Rectangle Bounds) {
			if(info.CanShowScrollBar)
				g.FillRectangle(new SolidBrush(info.Form.RowLineColor), Bounds.Width, Bounds.Y, 1, Bounds.Height);
		}
		protected virtual void DrawGrid(PopupFormGraphicsInfoArgs info) {
			MultiColumnListBoxViewInfo vi = info.ViewInfo as MultiColumnListBoxViewInfo;
			if(vi.GridRect.IsEmpty) return;
			GraphicsClipState state = info.Cache.ClipInfo.SaveAndSetClip(vi.GridRect);
			try {
				info.Graphics.FillRectangle(vi.AppearanceGrid.GetBackBrush(info.Cache), vi.GridRect);
				DrawRows(info, vi);
				DrawGridLines(info.Cache, vi);
				DrawBorderScrollBar(info.Graphics, vi, vi.GridRect);
			}
			finally { info.Cache.ClipInfo.RestoreClipRelease(state); }
		}
		protected virtual void DrawRows(PopupFormGraphicsInfoArgs info, MultiColumnListBoxViewInfo vi) {
			for(int i = 0; i < vi.Rows.Count; i++) {
				DrawRow(info, (MultiColumnListBoxRowInfo)vi.Rows[i], vi);
			}
		}
		protected override void DrawSizeBar(PopupFormGraphicsInfoArgs info) {
			MultiColumnListBoxViewInfo vi = info.ViewInfo as MultiColumnListBoxViewInfo;
			if(!vi.ShowSizeBar) return;
			Rectangle boundsBar = GetSizeBarRect(info);
			if(boundsBar != Rectangle.Empty)
				vi.PaintAppearance.DrawBackground(info.Graphics, info.Cache, boundsBar);
		}
		bool CanDrawImage(MultiColumnListBoxViewInfo vi, MultiColumnListBoxRowInfo row) {
			return vi.Form.IsImage && row.ImageIndex > -1;
		}
		protected virtual void DrawRow(PopupFormGraphicsInfoArgs info, MultiColumnListBoxRowInfo row, MultiColumnListBoxViewInfo vi) {
			Graphics g = info.Graphics;
			Brush textBrush = vi.AppearanceGrid.GetForeBrush(info.Cache);
			if(row.RowIndex == vi.HotItemIndex) {
				row.State = ObjectState.Hot;
				if(!DrawBarCore(info, row, vi)) {
					g.FillRectangle(info.Cache.GetSolidBrush(vi.GetSystemColor(SystemColors.Highlight)), row.Bounds);
				}
				textBrush = info.Cache.GetSolidBrush(GetHighlightedForeColor(vi));
			}
			if(row.RowIndex == vi.SelectedItemIndex) {
				row.State = ObjectState.Selected;
				if(!DrawBarCore(info, row, vi)) {
					g.FillRectangle(info.Cache.GetSolidBrush(vi.GetSystemColor(SystemColors.HotTrack)), row.Bounds);
				}
				textBrush = info.Cache.GetSolidBrush(GetSelectedForeColor(vi));
			}
			for(int i = 0; i < vi.Headers.Count; i++) {
				LookUpColumnHeaderObjectInfoArgs header = (LookUpColumnHeaderObjectInfoArgs)vi.Headers[i];
				Rectangle stringRect = Rectangle.Empty, imageRect = Rectangle.Empty;
				Image image = null;
				Rectangle headerBounds = header.Bounds;
				vi.CellFormat.Alignment = header.Alignment;
				string cellString = row.RecordStrings[i];
				if(i == 0 && CanDrawImage(vi, row)) {
					image = vi.Form.ImageList.Images[row.ImageIndex];
					imageRect = new Rectangle(headerBounds.Left + 2, row.Bounds.Top, image.Width, image.Height);
					stringRect = new Rectangle(imageRect.Width + 5, row.Bounds.Top, headerBounds.Width - imageRect.Width - 5, row.Bounds.Height - 1);
					if(GetAllowGlyphSkinning(vi)) {
						var coloredAttributes = ImageColorizer.GetColoredAttributes(vi.AppearanceGrid.GetForeColor());
						g.DrawImage(image, imageRect, 0, 0, imageRect.Width, imageRect.Height, GraphicsUnit.Pixel, coloredAttributes);
					}
					else
						g.DrawImage(image, imageRect);
				}
				else
					stringRect = new Rectangle(headerBounds.Left + 2, row.Bounds.Top, headerBounds.Width - 4, row.Bounds.Height - 1);
				info.Cache.DrawString(cellString, vi.AppearanceGrid.Font, textBrush, stringRect, vi.CellFormat);
			}
		}
		protected virtual internal bool GetAllowGlyphSkinning(MultiColumnListBoxViewInfo viewInfo) {
			return viewInfo.Form != null && viewInfo.Form.AllowGlyphSkinning == DefaultBoolean.True;
		}
		protected bool IsHightlightedItemStyleEnabled(LookUpEdit edit) {
			if(edit == null) return false;
			return BaseListBoxControl.GetHighlightedItemStyle(edit.LookAndFeel, edit.Properties.HighlightedItemStyle) == HighlightStyle.Skinned;
		}
		public bool DrawBarCore(PopupFormGraphicsInfoArgs info, MultiColumnListBoxRowInfo row, MultiColumnListBoxViewInfo vi) {
			if(!vi.IsSkined) return false;
			Color color = GetPressedItemColor(vi);
			if(row.Hot) color = GetHighlightedItemColor(vi);
			info.Graphics.FillRectangle(new SolidBrush(color), new Rectangle(row.Bounds.X, row.Bounds.Y, row.Bounds.Width, row.Bounds.Height));
			return true;
		}
		protected virtual Color GetHighlightedForeColor(MultiColumnListBoxViewInfo vi) {
			Skin si = CommonSkins.GetSkin(vi.Form.LookAndFeel);
			if(si != null && vi.IsSkined) {
				Color color = si.TranslateColor(SystemColors.ControlText);
				return color.IsEmpty ? vi.AppearanceGrid.ForeColor : color;
			}
			return vi.GetSystemColor(SystemColors.ActiveBorder);
		}
		protected virtual Color GetSelectedForeColor(MultiColumnListBoxViewInfo vi) {
			Skin si = CommonSkins.GetSkin(vi.Form.LookAndFeel);
			if(si != null && vi.IsSkined) {
				Color color = si.TranslateColor(SystemColors.HighlightText);
				return color.IsEmpty ? vi.AppearanceGrid.ForeColor : color;
			}
			return vi.GetSystemColor(SystemColors.HighlightText);
		}
		protected virtual Color GetHighlightedItemColor(MultiColumnListBoxViewInfo vi) {
			Skin si = CommonSkins.GetSkin(vi.Form.LookAndFeel);
			if(si != null && vi.IsSkined) {
				Color color = si.TranslateColor(SystemColors.ControlDark);
				return color.IsEmpty ? SystemColors.ControlDark : color;
			}
			return vi.GetSystemColor(SystemColors.ControlDark);
		}
		protected virtual Color GetPressedItemColor(MultiColumnListBoxViewInfo vi) {
			Skin si = CommonSkins.GetSkin(vi.Form.LookAndFeel);
			if(si != null && vi.IsSkined) {
				Color color = si.TranslateColor(SystemColors.Highlight);
				return color.IsEmpty ? SystemColors.Highlight : color;
			}
			return vi.GetSystemColor(SystemColors.Highlight);
		}
		private void DrawGridLines(GraphicsCache cache, MultiColumnListBoxViewInfo vi) {
			if(!vi.Form.Properties.ShowLines) return;
			Graphics g = cache.Graphics;
			Color color = vi.IsSkined ? VGridSkins.GetSkin(vi.Form.LookAndFeel)[VGridSkins.SkinGridLine].Color.GetBackColor() : SystemColors.ActiveBorder;
			for(int i = 0; i < vi.Headers.Count - 1; i++) {
				LookUpColumnHeaderObjectInfoArgs header = (LookUpColumnHeaderObjectInfoArgs)vi.Headers[i];
				g.FillRectangle(new SolidBrush(color), new Rectangle(header.Bounds.Right - 1, vi.GridRect.Top, 1, vi.GridRect.Height));
			}
			int y = vi.RowHeight + vi.GridRect.Y - 1;
			while(y <= vi.ContentRect.Height) {
				g.FillRectangle(new SolidBrush(color), vi.GridRect.X, y, vi.GridRect.Width, 1);
				y += vi.RowHeight;
			}
			if(!vi.IsTopSizeBar) g.FillRectangle(new SolidBrush(color), new Rectangle(vi.SizeBarRect.Left, vi.SizeBarRect.Top, vi.SizeBarRect.Width, 1));
		}
	}
	internal class MultiColumnListBoxParams {
		RepositoryItemLookUpEdit item;
		protected MultiColumnListBoxParams(RepositoryItemLookUpEdit item) {
			this.item = item;
		}
		protected void SetInteger(string storedName, int value) { Item.PropertyStore[storedName] = value; }
		public virtual void Save(MultiColumnListBoxViewInfo viewInfo, bool formResized) {
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
		public virtual void Load(MultiColumnListBox control) {
			MultiColumnListBoxViewInfo viewInfo = control.ViewInfo;
			foreach(LookUpColumnInfo column in Item.Columns) {
				if(!column.Visible) continue;
				viewInfo.ColumnsInfo[column] = GetColumnSaveInfo(column);
			}
			viewInfo.AutoSearchHeaderIndex = ColumnIndexFromRepositoryToViewInfo(GetAutoSearchColumnIndex());
			viewInfo.SortHeaderIndex = ColumnIndexFromRepositoryToViewInfo(GetSortColumnIndex());
		}
		protected virtual LookUpColumnPopupSaveInfo GetColumnSaveInfo(LookUpColumnInfo column) {
			return new LookUpColumnPopupSaveInfo(column.Width, column.SortOrder);
		}
		protected virtual int GetAutoSearchColumnIndex() { return Item.AutoSearchColumnIndex; }
		protected virtual int GetSortColumnIndex() { return Item.SortColumnIndex; }
		protected virtual void SetAutoSearchColumnIndex(int value) { Item.AutoSearchColumnIndex = value; }
		protected virtual void SetSortColumnIndex(int value) { Item.SortColumnIndex = value; }
		protected virtual void SetDropDownRows(int value) { Item.DropDownRows = value; }
		protected virtual void SetPopupWidth(int value) { SetInteger(LookUpPropertyNames.PopupWidth, value); }
		protected virtual void SetColumnSaveInfo(LookUpColumnInfo column, LookUpColumnPopupSaveInfo colInfo) { column.Init(colInfo); }
		protected RepositoryItemLookUpEdit Item { get { return item; } }
		protected int ColumnIndexFromViewInfoToRepository(int visibleIndex) {
			int visibleItemsProcessedIndex = -1;
			for(int i = 0; i < Item.Columns.Count; ++i) {
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
			for(int i = 0; i <= repositoryItemColumnsIndex && i < Item.Columns.Count; ++i) {
				if(Item.Columns[i].Visible)
					++visibleResult;
			}
			return visibleResult;
		}
	}
	internal class MultiColumnListBoxStandaloneParams : MultiColumnListBoxParams {
		public MultiColumnListBoxStandaloneParams(RepositoryItemLookUpEdit item) : base(item) { }
		public override void Save(MultiColumnListBoxViewInfo viewInfo, bool formResized) {
			Item.BeginUpdate();
			try {
				base.Save(viewInfo, formResized);
			}
			finally {
				Item.CancelUpdate();
			}
		}
	}
	public interface IMultiColumnListBox {
		Control MultiColumnListBox { get; }
		ImageList ImageList { get; set; }
		RepositoryItemLookUpEdit Properties { get; }
		void Update();
		object SelectedItem { get; set; }
		event EventHandler ItemDoubleClick;
		event EventHandler SelectedItemChanged;
		string FilterPrefix { get; set; }
		DefaultBoolean AllowGlyphSkinning { get; set; }
	}
	public class MultiColumnListBoxCreator {
		public static IMultiColumnListBox CreateMultiColumnListBox() {
			return new MultiColumnListBox();
		}
	}
}
