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

using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Gesture;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.XtraGrid.Views.Tile.Handler;
using DevExpress.XtraGrid.Views.Tile.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraGrid.Views.Tile {
	[DesignTimeVisible(false)]
	public class TileView : ColumnView, ISupportXtraAnimation, IAppearanceOwner, IContextItemCollectionOwner, IContextItemCollectionOptionsOwner {
		private static object itemClick = new object();
		private static object itemDblClick = new object();
		private static object itemPress = new object();
		private static object itemRightClick = new object();
		private static object itemCheckChanged = new object();
		private static object contextButtonClick = new object();
		private static object contextItemCustomize = new object();
		private static object itemCustomize = new object();
		private static object getThumbnailImage = new object();
		private static object getLoadingImage = new object();
		public TileView() {
			SortInfo.MaxGroupCount = 1;
			this.optionsTiles = new TileViewItemOptions(this);
			this.animateArrival = true;
		}
		protected virtual TileViewColumns CreateColumnSet() {
			return new TileViewColumns(this);
		}
		protected virtual TileViewOptionsImageLoad CreateOptionsImageLoad() {
			return new TileViewOptionsImageLoad(this);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				RemoveScrollBar();
			}
			base.Dispose(disposing);
		}
		void RemoveScrollBar() {
			if(scrollInfo != null) {
				UnSubscribeScollInfoEvents(scrollInfo);
				ScrollInfo.RemoveControls(GridControl);
				ScrollInfo.Dispose();
				scrollInfo = null;
			}
			TileViewControl.ScrollBar = null;
		}
		TileViewColumns columnSet;
		[DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TileViewColumns ColumnSet {
			get {
				if(columnSet == null)
					columnSet = CreateColumnSet();
				return columnSet;
			}
		}
		void ResetColumnSet() { ColumnSet.Reset(); }
		bool ShouldSerializeColumnSet() { return ColumnSet.ShouldSerialize(); }
		TileViewOptionsImageLoad optionsImageLoad;
		[Browsable(false), DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TileViewOptionsImageLoad OptionsImageLoad {
			get {
				if(optionsImageLoad == null)
					optionsImageLoad = CreateOptionsImageLoad();
				return optionsImageLoad;
			}
		}
		protected internal GridColumn GroupColumn { get { return ColumnSet.GroupColumn; } }
		protected internal override void OnColumnOptionsChanged(GridColumn column, BaseOptionChangedEventArgs e) {
			if(!IsDesignMode && !IsTemplateEditingMode)
				ClearItems();
			OnPropertiesChanged();
		}
		protected internal virtual void OnGroupColumnChanged(GridColumn prev) {
			SortInfo.BeginUpdate();
			try {
				if(prev != null) {
					SortInfo.Remove(prev);
					SortInfo.GroupCount = 0;
				}
				if(GroupColumn != null) {
					SortInfo.Insert(0, GroupColumn, ColumnSortOrder.Ascending);
					SortInfo.GroupCount = 1;
				}
			}
			finally {
				SortInfo.EndUpdate();
			}
			ClearItems();
			ResetVirtCalculator();
			OnPropertiesChanged();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Properties)]
		public TileItemElementCollection TileTemplate {
			get { return TemplateItem.Elements; }
		}
		TileViewTemplateItem templateItem;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TileViewTemplateItem TemplateItem {
			get {
				if(templateItem == null) templateItem = new TileViewTemplateItem(this);
				return templateItem;
			}
		}
		protected override void SetupDataController() {
			base.SetupDataController();
			DataController.AutoExpandAllGroups = true;
		}
		protected override void OnDataController_DataSourceChanged(object sender, EventArgs e) {
			base.OnDataController_DataSourceChanged(sender, e);
			PopulateItems();
		}
		protected override void SetRowCellValueCore(int rowHandle, Columns.GridColumn column, object _value, bool fromEditor) {
			base.SetRowCellValueCore(rowHandle, column, _value, fromEditor);
			VisualClientUpdateRow(rowHandle);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetTemplateEditing(bool value) {
			this.isTemplateEditingMode = value;
		}
		bool isTemplateEditingMode;
		protected internal bool IsTemplateEditingMode { 
			get { return isTemplateEditingMode; } 
		}
		protected virtual void PopulateItems() {
			if(IsDesignMode || IsTemplateEditingMode)
				AddDesignItem();
		}
		protected virtual object GetGroupRowValue(int rowHandle, GridColumn column) {
			if(DataController == null || column == null) return null;
			return DataController.GetGroupRowValue(rowHandle, column.ColumnInfo);
		}
		protected internal virtual string GetGroupRowDisplayText(int rowHandle) {
			object val = GetGroupRowValue(rowHandle, GroupColumn);
			return GetGroupRowDisplayText(rowHandle, GroupColumn, val, FormatInfo.Empty);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void AddDesignItem() {
			try {
				TileViewControl.BeginUpdate();
				ClearItems();
				var defGroup = TileViewInfo.DefaultGroup;
				if(defGroup != null)
					defGroup.Items.Add(TemplateItem);
			}
			finally {
				TileViewControl.EndUpdate();
			}
		}
		protected internal virtual void UpdateItem(TileViewItem item, int dataIndex) {
			item.RowHandle = dataIndex;
			item.Elements.Clear();
			item.View = this;
			foreach(TileViewItemElement element in TileTemplate) {
				var newElement = element.Clone() as TileViewItemElement;
				newElement.Column = null;
				item.Elements.Add(newElement);
				if(element.Column == null || string.IsNullOrEmpty(element.Column.FieldName)) continue;
				SetElementCellValue(newElement, dataIndex, element.Column);
			}
			item.BackgroundImage = HasBackImageColumn ? ImageLoader.LoadImage(item.ImageInfo) : null;
			item.Checked = HasCheckedColumn ? GetBooleanCellValue(ColumnSet.CheckedColumn, dataIndex) : false;
			item.Enabled = HasEnabledColumn ? GetBooleanCellValue(ColumnSet.EnabledColumn, dataIndex) : true;
			OnItemCustomize(item);
		}
		internal bool HasCheckedColumn { get { return ColumnSet.CheckedColumn != null && !string.IsNullOrEmpty(ColumnSet.CheckedColumn.FieldName); } }
		internal bool HasBackImageColumn { get { return ColumnSet.BackgroundImageColumn != null && !string.IsNullOrEmpty(ColumnSet.BackgroundImageColumn.FieldName); } }
		internal bool HasEnabledColumn { get { return ColumnSet.EnabledColumn != null && !string.IsNullOrEmpty(ColumnSet.EnabledColumn.FieldName); } }
		BaseImageLoader imageLoader;
		protected internal BaseImageLoader ImageLoader {
			get {
				if(imageLoader == null) {
					if(IsAsyncImageLoader()) imageLoader = new TileViewAsyncImageLoader(TileViewInfoCore);
					else imageLoader = new TileViewSyncImageLoader(TileViewInfoCore);
				}
				return imageLoader;
			}
		}
		protected internal void ResetImageLoader() {
			imageLoader = null;
		}
		protected bool IsAsyncImageLoader() {
			return OptionsImageLoad.AsyncLoad;
		}
		protected bool GetBooleanCellValue(GridColumn gridColumn, int rowHandle) {
			object currentValue = ViewInfo.View.GetRowCellValue(rowHandle, gridColumn);
			if(currentValue is bool)
				return (bool)currentValue;
			if(DBNull.Value == currentValue)
				return false;
			IConvertible cvt = currentValue as IConvertible;
			if(cvt != null) {
				try {
					bool res = cvt.ToBoolean(System.Threading.Thread.CurrentThread.CurrentUICulture);
					return res;
				}
				catch(FormatException) {
				}
			}
			return false;
		}
		protected virtual Image GetImageCellValue(GridColumn gridColumn, int rowHandle) {
			Image img = null;
			object currentValue = ViewInfo.View.GetRowCellValue(rowHandle, gridColumn);
			TryGetImageValue(currentValue, out img);
			return img;
		}
		protected virtual void SetElementCellValue(TileViewItemElement element, int dataIndex, GridColumn column) {
			if(!column.Visible) {
				element.Text = string.Empty;
				return;
			}
			Image img;
			var value = DataController.GetRowValue(dataIndex, column.ColumnInfo);
			if(TryGetImageValue(value, out img)) {
				element.Image = img;
				element.Text = string.Empty;
				return;
			}
			string text = column.OptionsColumn.ShowCaption ? column.GetCaption() + ": " : string.Empty;
			text += GetRowCellDisplayText(dataIndex, column);
			element.Text = text;
		}
		protected virtual bool TryGetImageValue(object value, out Image result) {
			result = null;
			if(value is Image) {
				result = value as Image;
				return true;
			}
			if(value is byte[]) {
				try {
					result = DevExpress.XtraEditors.Controls.ByteImageConverter.FromByteArray(value as byte[]);
					return true;
				}
				catch(ArgumentException) { }
			}
			return false;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual TileViewItem CreateItem() {
			return new TileViewItem();
		}
		void ClearItems() {
			TileGroup defaultGroup = null;
			if(TileViewControl == null) return;
			foreach(TileGroup group in TileViewControl.Groups) {
				foreach(TileItem item in group.Items) {
					item.Dispose();
				}
				group.Items.Clear();
				if(group is TileViewGroup && (group as TileViewGroup).IsDefault)
					defaultGroup = group;
			}
			TileViewControl.Groups.Clear();
			if(defaultGroup != null)
				TileViewControl.Groups.Add(defaultGroup);
		}
		protected virtual TileViewItem FindItemByHandle(int rowHandle) {
			if(rowHandle < 0 || TileViewControl == null) return null;
			TileViewItem item;
			if(TileViewInfoCore.VisibleItems.TryGetValue(rowHandle, out item))
				return item;
			return null;
		}
		void RefreshItem(int rowHandle) {
			try {
				TileViewInfo.SuppressOnPropertiesChanged = true;
				var item = FindItemByHandle(rowHandle);
				if(item != null) UpdateItem(item, rowHandle);
			}
			finally {
				TileViewInfo.SuppressOnPropertiesChanged = false;
			}
		}
		protected override void RefreshRow(int rowHandle, bool updateEditor, bool updateEditorValue) {
			base.RefreshRow(rowHandle, updateEditor, updateEditorValue);
			VisualClientUpdateRow(rowHandle);
		}
		protected internal virtual void ToggleRowCheckValue(TileViewItem item) {
			if(item == null || !HasCheckedColumn || !ColumnSet.CheckedColumn.OptionsColumn.AllowEdit)
				return;
			SetRowCellValue(item.RowHandle, ColumnSet.CheckedColumn, item.Checked);
		}
		public virtual int[] GetCheckedRows() {
			List<int> result = new List<int>();
			if(!HasCheckedColumn)
				return new int[] { };
			for(int n = 0; n < RowCount; n++) {
				if(GetBooleanCellValue(ColumnSet.CheckedColumn, n))
					result.Add(n);
			}
			return result.ToArray();
		}
		protected internal virtual bool AllowEditCheckColumn {
			get {
				if(HasCheckedColumn)
					return ColumnSet.CheckedColumn.OptionsColumn.AllowEdit;
				return false;
			}
		}
		protected override void UpdateFocusedRowHandleOnEnter() { }
		protected internal bool CanHighlightFocusedItem { get; set; }
		protected override void OnCurrentControllerRowChanged(CurrentRowEventArgs e) {
			int newHandle = DataController.CurrentControllerRow;
			if(DataController.IsGroupRowHandle(newHandle)) {
				newHandle = EnsureItemHandle(newHandle);
				DataController.CurrentControllerRow = newHandle;
				return;
			}
			CanHighlightFocusedItem = GridControl.IsLoaded || OptionsTiles.HighlightFocusedTileOnGridLoad;
			if(TileViewInfoCore != null && CanHighlightFocusedItem) {
				TileViewInfoCore.GroupsOffsetCache.CheckCache();
				TileViewInfoCore.FocusedRowHandle = newHandle;
			}
			else { 
				SetFocusedRowHandleCore(newHandle);
				if(TileViewInfoCore != null) TileViewInfoCore.SetFocusedRowHandleCore(newHandle);
			}
		}
		int EnsureItemHandle(int handle) {
			if(DataController.IsGroupRowHandle(handle))
				handle = DataController.GroupInfo.GetChildRow(handle, 0);
			return handle;
		}
		protected override void DoChangeFocusedRow(int currentRowHandle, int newRowHandle, bool raiseUpdateCurrentRow) {
			base.DoChangeFocusedRow(currentRowHandle, newRowHandle, raiseUpdateCurrentRow);
			SetFocusedRowHandleCore(newRowHandle);
			if(IsInitialized) {
				DataController.CurrentControllerRow = FocusedRowHandle;
				if(TileViewInfoCore != null) {
					TileViewInfoCore.FocusedRowHandle = FocusedRowHandle;
				}
			}
		}
		protected internal override void DoColumnChangeGroupIndex(GridColumn column, int newIndex) {
			if(newIndex > -1) {
				if(ColumnSet.GroupColumn != column) {
					if(ColumnSet.GroupColumn != null) {
						SortInfo.BeginUpdate();
						SortInfo.Remove(ColumnSet.GroupColumn);
						SortInfo.GroupCount = 0;
						SortInfo.CancelUpdate();
					}
				}
				ColumnSet.SetGroupColumn(column);
			}
			else {
				if(column == ColumnSet.GroupColumn) ColumnSet.SetGroupColumn(null);
			}
			base.DoColumnChangeGroupIndex(column, newIndex);
			if(ColumnSet.GroupColumn != null && SortInfo.GroupCount == 0) ColumnSet.SetGroupColumn(null);
		}
		TileViewNavigator Navigator { get { return TileViewControl.Navigator as TileViewNavigator; } }
		protected internal override void DoMoveFocusedRow(int delta, KeyEventArgs e) {
			Navigator.MoveFocusedRow(delta);
		}
		public override void MoveBy(int delta) {
			Navigator.MoveFocusedRow(delta);
		}
		public override void MoveFirst() {
			Navigator.MoveStart();
		}
		public override void MoveLast() {
			Navigator.MoveEnd();
		}
		public override void MoveLastVisible() {
			Navigator.MoveEnd();
		}
		public override void MoveNextPage() {
			Navigator.MovePageRight();
		}
		public override void MovePrevPage() {
			Navigator.MovePageLeft();
		}
		public virtual new TileViewHitInfo CalcHitInfo(Point pt) {
			if(TileViewInfo == null)
				return new TileViewHitInfo();
			var tileControlHitInfo = (TileViewInfo as ITileControl).CalcHitInfo(pt);
			return new TileViewHitInfo(tileControlHitInfo);
		}
		TileViewInfo TileViewInfo {
			[DebuggerStepThrough]
			get { return ViewInfo as TileViewInfo; } 
		}
		TileViewInfoCore TileViewInfoCore {
			[DebuggerStepThrough]
			get {
				ITileControl itilecontrol = TileViewInfo as ITileControl;
				return itilecontrol == null ? null : itilecontrol.ViewInfo as TileViewInfoCore;
			} 
		}
		ITileControl TileViewControl {
			[DebuggerStepThrough]
			get { return TileViewInfo as ITileControl; } 
		}
		protected override string ViewName {
			get {
				return "TileView";
			}
		}
		protected override GridColumnCollection CreateColumnCollection() {
			return new TileViewColumnCollection(this);
		}
		bool animateArrival;
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AnimateArrival {
			get { return animateArrival; }
			set { animateArrival = value; }
		}
		TileItemAppearances appearances;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the Appearance properties section instead."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public TileItemAppearances AppearanceItem {
			get {
				if(appearances == null) {
					appearances = new TileItemAppearances(this);
				}
				return appearances;
			}
		}
		AppearanceObject appearanceGroupText;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("Use the Appearance.GroupText property instead."),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public AppearanceObject AppearanceGroupText {
			get {
				if(appearanceGroupText == null) {
					appearanceGroupText = new AppearanceObject();
				}
				return appearanceGroupText;
			}
		}
		TileViewItemOptions optionsTiles;
		[DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TileViewItemOptions OptionsTiles {
			get { return optionsTiles; }
		}
		public override void ShowFilterPopup(Columns.GridColumn column) { }
		protected override void VisualClientUpdateScrollBar() {
			TileViewInfoCore.GroupsOffsetCache.IsDirty = true;
			TileViewInfoCore.UpdateScrollBar();
		}
		protected override int VisualClientTopRowIndex {
			get {
				return TileViewInfoCore.GetTopLeftItemIndex();
			}
		}
		protected override int VisualClientPageRowCount {
			get {
				return TileViewInfoCore.CalcBestScreenItemsCount(); 
			}
		}
		protected override void VisualClientUpdateRow(int controllerRowHandle) {
			RefreshItem(controllerRowHandle);
			UpdateAndRepaintItem(controllerRowHandle);
		}
		void UpdateAndRepaintItem(int controllerRowHandle) {
			var item = FindItemByHandle(controllerRowHandle);
			if(item == null || item.ItemInfo == null) return;
			item.ItemInfo.ForceUpdateAppearanceColors();
			item.ItemInfo.LayoutItem(item.ItemInfo.Bounds);
			TileViewControl.Invalidate(item.ItemInfo.Bounds);
		}
		protected override ViewPrintOptionsBase CreateOptionsPrint() {
			return new ViewPrintOptionsBase();
		}
		[DXCategory(CategoryName.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdAppearance)]
		public new TileViewAppearances Appearance { get { return base.Appearance as TileViewAppearances; } }
		protected override BaseViewAppearanceCollection CreateAppearances() {
			return new TileViewAppearances(this);
		}
		protected override BaseViewAppearanceCollection CreateAppearancesPrint() {
			return new TileViewAppearances(this);
		}
		protected override bool CanLeaveFocusOnTab(bool moveForward) {
			return true;
		}
		protected override void LeaveFocusOnTab(bool moveForward) {  }
		protected override BaseViewInfo CreateNullViewInfo() {
			return new TileViewInfo(this);
		}
		protected override void SetViewRect(Rectangle newValue) {
			this.viewRect = newValue;
			LayoutChanged();
		}
		Rectangle viewRect;
		public override Rectangle ViewRect {
			get { return viewRect; }
		}
		public override bool IsVisible {
			get { return !ViewRect.IsEmpty; }
		}
		protected internal virtual void OnColumnSetChanged() {
			OnPropertiesChanged();
		}
		protected internal override void OnChildLayoutChanged(BaseView childView) { }
		public override void LayoutChanged() {
			TileControlPropertiesChanged();
			if(!CalculateLayout()) return;
			base.LayoutChanged();
		}
		void TileControlPropertiesChanged() {
			if(ViewInfo == null) return;
			(ViewInfo as ITileControl).OnPropertiesChanged();
		}
		protected internal override void OnPropertiesChanged() {
			if(IsInitialized && TileViewInfoCore != null)
				TileViewInfoCore.NeedClearVisibleItems = true;
			base.OnPropertiesChanged();
		}
		protected override bool CalculateLayout() {
			if(ViewInfo == null)
				return false;
			((TileViewInfo)ViewInfo).Calc(null, ViewRect);
			ViewInfo.IsReady = true;
			return true;
		}
		protected override bool CalculateData() {
			var res = base.CalculateData();
			ViewInfo.IsDataDirty = false;
			return res;
		}
		protected override void SetLayoutDirty() {
			base.SetLayoutDirty();
			if(TileViewInfoCore == null) return;
			TileViewInfoCore.SetDirty();
			Invalidate();
		}
		bool ShouldSerializeFocusBorderColor() { return focusBorderColor != Color.Empty; }
		void ResetFocusBorderColor() { FocusBorderColor = Color.Empty; }
		Color focusBorderColor = Color.Empty;
		[Category(CategoryName.Appearance)]
		public Color FocusBorderColor {
			get { return focusBorderColor; }
			set {
				if(focusBorderColor == value) return;
				focusBorderColor = value;
				OnPropertiesChanged();
			}
		}
		int position;
		[DefaultValue(0), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Position {
			get { return position; }
			set {
				value = TileViewInfoCore.ConstraintOffset(value);
				if(Position == value)
					return;
				position = value;
				TileViewInfoCore.Offset = position;
				UpdateSrollBarValue(value);
			}
		}
		void UpdateSrollBarValue(int value) {
			if(TileViewInfoCore.ScrollModeInternal != TileControlScrollMode.ScrollBar) return;
			if(TileViewInfoCore.IsHorizontal)
				ScrollInfo.HScroll.Value = value;
			else
				ScrollInfo.VScroll.Value = value;
		}
		protected internal override void OnLookAndFeelChanged() {
			TileViewControl.ViewInfo.ClearBorderPainter();
			TileViewControl.ScrollBar = null;
			TileViewInfoCore.ResetDefaultAppearances();
			base.OnLookAndFeelChanged();
		}
		bool ISupportXtraAnimation.CanAnimate {
			get { return true; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return GridControl; }
		}
		protected internal virtual void OnOrientationChanged() {
			ResetVirtCalculator();
			OnPropertiesChanged();
		}
		void ResetVirtCalculator() {
			if(this.TileViewInfoCore != null) {
				this.TileViewInfoCore.ResetVirtCalculator();
			}
		}
		protected internal virtual void OnTileTemplateChanged() {
			OnPropertiesChanged();
		}
		public override void Assign(BaseView v, bool copyEvents) {
			base.Assign(v, copyEvents);
			TileView view = v as TileView;
			if(view == null)
				return;
			Appearance.Assign(view.Appearance);
			ColumnSet.Assign(view.ColumnSet);
			TileTemplate.Assign(view.TileTemplate);
			OptionsTiles.Assign(view.OptionsTiles);
			if(copyEvents) {
				Events.AddHandler(itemPress, view.Events[itemPress]);
				Events.AddHandler(itemClick, view.Events[itemClick]);
				Events.AddHandler(itemRightClick, view.Events[itemRightClick]);
				Events.AddHandler(itemDblClick, view.Events[itemDblClick]);
				Events.AddHandler(itemCheckChanged, view.Events[itemCheckChanged]);
				Events.AddHandler(itemCustomize, view.Events[itemCustomize]);
				Events.AddHandler(contextButtonClick, view.Events[contextButtonClick]);
				Events.AddHandler(contextItemCustomize, view.Events[contextItemCustomize]);
			}
		}
		protected internal virtual void OnItemPress(TileItem tileItem) {
			var tvi = tileItem as TileViewItem;
			if(tvi == null) return;
			RaiseItemPress(tvi);
		}
		protected virtual void RaiseItemPress(TileViewItem item) {
			TileViewItemClickEventHandler handler = (TileViewItemClickEventHandler)Events[itemPress];
			if(handler != null) handler(this, new TileViewItemClickEventArgs(item));
		}
		protected internal virtual void OnItemClick(TileItem tileItem) {
			var tvi = tileItem as TileViewItem;
			if(tvi == null) return;
			RaiseItemClick(tvi);
		}
		protected virtual void RaiseItemClick(TileViewItem item) {
			TileViewItemClickEventHandler handler = (TileViewItemClickEventHandler)Events[itemClick];
			if(handler != null) handler(this, new TileViewItemClickEventArgs(item));
		}
		protected internal virtual void OnItemDoubleClick(TileItem tileItem) {
			var tvi = tileItem as TileViewItem;
			if(tvi == null) return;
			RaiseItemDoubleClick(tvi);
		}
		protected virtual void RaiseItemDoubleClick(TileViewItem item) {
			TileViewItemClickEventHandler handler = (TileViewItemClickEventHandler)Events[itemDblClick];
			if(handler != null) handler(this, new TileViewItemClickEventArgs(item));
		}
		protected internal virtual void OnRightItemClick(TileItem tileItem) {
			var tvi = tileItem as TileViewItem;
			if(tvi == null) return;
			RaiseRightItemClick(tvi);
		}
		protected virtual void RaiseRightItemClick(TileViewItem item) {
			TileViewItemClickEventHandler handler = (TileViewItemClickEventHandler)Events[itemRightClick];
			if(handler != null) handler(this, new TileViewItemClickEventArgs(item));
		}
		protected internal virtual void OnItemCheckedChanged(TileItem tileItem) {
			var tvi = tileItem as TileViewItem;
			if(tvi == null) return;
			RaiseItemCheckedChanged(tvi);
		}
		protected virtual void RaiseItemCheckedChanged(TileViewItem item) {
			TileViewItemClickEventHandler handler = (TileViewItemClickEventHandler)Events[itemCheckChanged];
			if(handler != null) handler(this, new TileViewItemClickEventArgs(item));
		}
		protected internal virtual void RaiseContextItemClick(ContextItemClickEventArgs e) {
			ContextItemClickEventHandler handler = Events[contextButtonClick] as ContextItemClickEventHandler;
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseContextItemCustomize(ITileItem tileItem, ContextItem contextItem) {
			TileViewContextButtonCustomizeEventHandler handler = Events[contextItemCustomize] as TileViewContextButtonCustomizeEventHandler;
			var item = tileItem as TileViewItem;
			if(item != null && handler != null) {
				var e = new TileViewContextButtonCustomizeEventArgs(contextItem, item.RowHandle);
				handler(this, e);
			}
		}
		protected internal virtual void OnItemCustomize(TileViewItem item) {
			if(item != null)
				RaiseItemCustomize(item);
		}
		protected virtual void RaiseItemCustomize(TileViewItem item) {
			TileViewItemCustomizeEventHandler handler = Events[itemCustomize] as TileViewItemCustomizeEventHandler;
			if(handler != null) 
				handler(this, new TileViewItemCustomizeEventArgs(item));
		}
		protected internal virtual ThumbnailImageEventArgs RaiseGetThumbnailImage(ThumbnailImageEventArgs e) {
			ThumbnailImageEventHandler handler = (ThumbnailImageEventHandler)Events[getThumbnailImage];
			if(handler != null) handler(this, e);
			return e;
		}
		protected internal virtual Image RaiseGetLoadingImage(GetLoadingImageEventArgs e) {
			GetLoadingImageEventHandler handler = (GetLoadingImageEventHandler)Events[getLoadingImage];
			if(handler != null) handler(this, e);
			return e.LoadingImage;
		}
		[DXCategory(CategoryName.Action)]
		public event TileViewItemClickEventHandler ItemClick {
			add { Events.AddHandler(itemClick, value); }
			remove { Events.RemoveHandler(itemClick, value); }
		}
		[DXCategory(CategoryName.Action)]
		public event TileViewItemClickEventHandler ItemDoubleClick {
			add { Events.AddHandler(itemDblClick, value); }
			remove { Events.RemoveHandler(itemDblClick, value); }
		}
		[DXCategory(CategoryName.Action)]
		public event TileViewItemClickEventHandler ItemRightClick {
			add { Events.AddHandler(itemRightClick, value); }
			remove { Events.RemoveHandler(itemRightClick, value); }
		}
		[DXCategory(CategoryName.Action)]
		public event TileViewItemClickEventHandler ItemPress {
			add { Events.AddHandler(itemPress, value); }
			remove { Events.RemoveHandler(itemPress, value); }
		}
		[DXCategory(CategoryName.Action)]
		public event TileViewItemClickEventHandler ItemCheckedChanged {
			add { Events.AddHandler(itemCheckChanged, value); }
			remove { Events.RemoveHandler(itemCheckChanged, value); }
		}
		[DXCategory(CategoryName.ContextButtons)]
		public event ContextItemClickEventHandler ContextButtonClick {
			add { Events.AddHandler(contextButtonClick, value); }
			remove { Events.RemoveHandler(contextButtonClick, value); }
		}
		[DXCategory(CategoryName.ContextButtons)]
		public event TileViewContextButtonCustomizeEventHandler ContextButtonCustomize {
			add { Events.AddHandler(contextItemCustomize, value); }
			remove { Events.RemoveHandler(contextItemCustomize, value); }
		}
		[DXCategory(CategoryName.Data)]
		public event TileViewItemCustomizeEventHandler ItemCustomize {
			add { Events.AddHandler(itemCustomize, value); }
			remove { Events.RemoveHandler(itemCustomize, value); }
		}
		[DXCategory(CategoryName.Data)]
		public event ThumbnailImageEventHandler GetThumbnailImage {
			add { Events.AddHandler(getThumbnailImage, value); }
			remove { Events.RemoveHandler(getThumbnailImage, value); }
		}
		[DXCategory(CategoryName.Appearance)]
		public event GetLoadingImageEventHandler GetLoadingImage {
			add { Events.AddHandler(getLoadingImage, value); }
			remove { Events.RemoveHandler(getLoadingImage, value); }
		}
		#region TouchSupport
		protected internal override Utils.Gesture.GestureAllowArgs[] CheckAllowGestures(Point point) {
			return ((IGestureClient)((TileViewHandler)Handler).HandlerCore).CheckAllowGestures(point);
		}
		protected internal override void OnTouchScroll(GestureArgs info, Point delta, ref Point overPan) {
			if(info.IsBegin)
				((IGestureClient)((TileViewHandler)Handler).HandlerCore).OnBegin(info);
			((IGestureClient)((TileViewHandler)Handler).HandlerCore).OnPan(info, delta, ref overPan);
		}
		#endregion TouchSupport
		#region ContextButtons
		ContextItemCollection contextButtons;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.ContextButtons)]
		public ContextItemCollection ContextButtons {
			get {
				if(contextButtons == null) {
					contextButtons = CreateContextButtons();
					contextButtons.Options = ContextButtonOptions;
				}
				return contextButtons;
			}
		}
		protected virtual ContextItemCollection CreateContextButtons() {
			return new ContextItemCollection(this);
		}
		ContextItemCollectionOptions contextButtonOptions;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter)), DXCategory(CategoryName.ContextButtons)]
		public ContextItemCollectionOptions ContextButtonOptions {
			get {
				if(contextButtonOptions == null)
					contextButtonOptions = CreateContextButtonOptions();
				return contextButtonOptions;
			}
		}
		protected virtual ContextItemCollectionOptions CreateContextButtonOptions() {
			return new ContextItemCollectionOptions(this);
		}
		bool IContextItemCollectionOwner.IsDesignMode {
			get { return IsDesignMode; }
		}
		bool IContextItemCollectionOwner.IsRightToLeft {
			get { return IsRightToLeft; }
		}
		void IContextItemCollectionOwner.OnCollectionChanged() {
			LayoutChanged();
		}
		void IContextItemCollectionOwner.OnItemChanged(ContextItem item, string propertyName, object oldValue, object newValue) {
			if(propertyName == "Visibility") {
				Invalidate();
				if(GridControl != null)
					GridControl.Update();
			}
			else
				LayoutChanged();
		}
		ContextAnimationType IContextItemCollectionOptionsOwner.AnimationType {
			get { return ContextAnimationType.OpacityAnimation; }
		}
		void IContextItemCollectionOptionsOwner.OnOptionsChanged(string propertyName, object oldValue, object newValue) {
			LayoutChanged();
		}
		#endregion ContextButtons
		#region HiddenProps
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string PaintStyleName {
			get { return base.PaintStyleName; }
			set { base.PaintStyleName = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool CanGroupColumn(GridColumn column) { return true; }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new int GroupCount { get { return base.GroupCount; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BaseAppearanceCollection AppearancePrint { get { return base.AppearancePrint; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public new ColumnViewOptionsFilter OptionsFilter { get { return base.OptionsFilter; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public new ColumnViewOptionsSelection OptionsSelection { get { return base.OptionsSelection; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int DetailHeight {
			get { return base.DetailHeight; }
			set { base.DetailHeight = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override XtraTab.TabHeaderLocation DetailTabHeaderLocation {
			get { return base.DetailTabHeaderLocation; }
			set { base.DetailTabHeaderLocation = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageCollection HtmlImages {
			get { return base.HtmlImages; }
			set { base.HtmlImages = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override int ViewCaptionHeight {
			get { return base.ViewCaptionHeight; }
			set { base.ViewCaptionHeight = value; }
		}
		#endregion HiddenProps
		#region ScrollInfo
		ScrollInfo scrollInfo;
		protected internal virtual ScrollInfo ScrollInfo {
			get {
				if(scrollInfo == null) {
					scrollInfo = CreateScrollInfo();
					SubscribeScollInfoEvents(scrollInfo);
				}
				return scrollInfo;
			}
		}
		protected virtual void SubscribeScollInfoEvents(Scrolling.ScrollInfo scrollInfo) {
			scrollInfo.VScroll_ValueChanged += OnVScroll;
			scrollInfo.HScroll_ValueChanged += OnHScroll;
		}
		protected virtual void UnSubscribeScollInfoEvents(Scrolling.ScrollInfo scrollInfo) {
			scrollInfo.VScroll_ValueChanged -= OnVScroll;
			scrollInfo.HScroll_ValueChanged -= OnHScroll;
		}
		protected virtual ScrollInfo CreateScrollInfo() {
			return new ScrollInfo(this);
		}
		protected void OnVScroll(object sender, EventArgs e) {
			OnScroll(ScrollInfo.VScroll.Value);
		}
		protected void OnHScroll(object sender, EventArgs e) {
			OnScroll(ScrollInfo.HScroll.Value);
		}
		protected virtual void OnScroll(int value) {
			TileViewInfo.OnScroll(value);
		}
		protected internal override void CreateHandles() {
			base.CreateHandles();
			if(GridControl.IsHandleCreated)
				ScrollInfo.CreateHandles();
		}
		protected internal override void AddToGridControl() {
			base.AddToGridControl();
			if(GridControl != null)
				ScrollInfo.AddControls(GridControl);
		}
		protected internal override void RemoveFromGridControl() {
			base.RemoveFromGridControl();
			if(GridControl != null) {
				ScrollInfo.RemoveControls(GridControl);
			}
		}
		protected override void UpdateScrollBars() {
			TileViewInfoCore.UpdateScrollBar();
		}
		#endregion ScrollInfo
	}
	public delegate void TileViewItemCustomizeEventHandler(object sender, TileViewItemCustomizeEventArgs e);
	public delegate void TileViewContextButtonCustomizeEventHandler(object sender, TileViewContextButtonCustomizeEventArgs e);
	public delegate void TileViewItemClickEventHandler(object sender, TileViewItemClickEventArgs e);
	public class TileViewItemCustomizeEventArgs : EventArgs {
		public TileViewItemCustomizeEventArgs(TileViewItem item) {
			Item = item;
			if(item != null)
				RowHandle = item.RowHandle;
		}
		public TileViewItem Item { get; private set; }
		public int RowHandle { get; private set; }
	}
	public class TileViewContextButtonCustomizeEventArgs : EventArgs {
		public TileViewContextButtonCustomizeEventArgs(ContextItem item, int rowHandle) {
			Item = item;
			RowHandle = rowHandle;
		}
		public ContextItem Item { get; private set; }
		public int RowHandle { get; private set; }
	}
	public class TileViewItemClickEventArgs : EventArgs {
		TileViewItem item;
		public TileViewItemClickEventArgs(TileViewItem item) {
			this.item = item;
		}
		public TileViewItem Item { get { return item; } }
	}
	public class TileViewItem : TileItem {
		public TileViewItem() {
			this.rowHandle = GridControl.InvalidRowHandle;
		}
		protected override TileItemViewInfo CreateViewInfo() {
			return new TileViewItemInfo(this);
		}
		int rowHandle;
		public int RowHandle {
			get { return rowHandle; }
			internal set {
				if(rowHandle != value)
					ResetImageInfo();
				rowHandle = value;
			}
		}
		protected override TileItemElement CreateTileItemElement() {
			return new TileViewItemElement();
		}
		protected override TileItemElementCollection CreateElementCollection() {
			return new TileViewItemElementCollection(this);
		}
		protected override bool GetIsMedium() {
			return ItemSize == TileItemSize.Medium;
		}
		protected override bool GetIsLarge() {
			bool b = base.GetIsLarge();
			return b || ItemSize == TileItemSize.Default;
		}
		void ResetAppearanceItem() { AppearanceItem.Reset(); }
		bool ShouldSerializeAppearanceItem() { return AppearanceItem.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Category(CategoryName.Appearance)]
		public new TileViewItemAppearances AppearanceItem {
			get { return base.AppearanceItem as TileViewItemAppearances; }
		}
		protected override TileItemAppearances CreateAppearanceItem() {
			return new TileViewItemAppearances(this);
		}
		TileView view;
		public TileView View {
			get { return view; }
			protected internal set { view = value; }
		}
		ImageLoadInfo imageInfo;
		public ImageLoadInfo ImageInfo {
			get {
				if(imageInfo == null) {
					imageInfo = CreateImageLoadInfo();
				}
				return imageInfo;
			}
			set { imageInfo = value; }
		}
		protected ImageLoadInfo CreateImageLoadInfo() {
			if(View == null) return null;
			int dataSourceIndex = View.ViewRowHandleToDataSourceIndex(RowHandle);
			ImageLayoutMode mode = GetImageLayoutMode();
			ImageContentAnimationType animationType = GetAnimationType();
			Size desiredSize = GetDesiredThumbnailSize();
			return new ImageLoadInfo(dataSourceIndex, RowHandle, animationType, mode, View.OptionsTiles.ItemSize, desiredSize);
		}
		protected void ResetImageInfo() {
			this.imageInfo = null;
		}
		Size GetDesiredThumbnailSize() { 
			if(View != null && View.OptionsImageLoad.DesiredThumbnailSize != Size.Empty)
				return View.OptionsImageLoad.DesiredThumbnailSize;
			return View.OptionsTiles.ItemSize;
		}
		ImageContentAnimationType GetAnimationType() {
			if(View == null) return ImageContentAnimationType.None;
			return View.OptionsImageLoad.AnimationType;
		}
		protected internal ImageLayoutMode GetImageLayoutMode() { 
			switch(View.OptionsTiles.ItemBackgroundImageScaleMode){
				case TileItemImageScaleMode.Stretch : return ImageLayoutMode.Stretch;
				case TileItemImageScaleMode.Squeeze: return ImageLayoutMode.Squeeze;
				case TileItemImageScaleMode.StretchHorizontal: return ImageLayoutMode.StretchHorizontal;
				case TileItemImageScaleMode.StretchVertical: return ImageLayoutMode.StretchVertical;
				case TileItemImageScaleMode.ZoomInside: return ImageLayoutMode.ZoomInside;
				case TileItemImageScaleMode.ZoomOutside: return ImageLayoutMode.ZoomOutside;
			}
			switch(View.OptionsTiles.ItemBackgroundImageAlignment){
				case TileItemContentAlignment.TopLeft: return ImageLayoutMode.TopLeft;
				case TileItemContentAlignment.TopRight: return ImageLayoutMode.TopRight;
				case TileItemContentAlignment.TopCenter: return ImageLayoutMode.TopCenter;
				case TileItemContentAlignment.MiddleLeft: return ImageLayoutMode.MiddleLeft;
				case TileItemContentAlignment.MiddleRight: return ImageLayoutMode.MiddleRight;
				case TileItemContentAlignment.MiddleCenter: return ImageLayoutMode.MiddleCenter;
				case TileItemContentAlignment.BottomLeft: return ImageLayoutMode.BottomLeft;
				case TileItemContentAlignment.BottomRight: return ImageLayoutMode.BottomRight;
				case TileItemContentAlignment.BottomCenter: return ImageLayoutMode.BottomCenter;
				case TileItemContentAlignment.Manual: return ImageLayoutMode.Default;
			}
			return ImageLayoutMode.Default;
		}
	}
	public class TileViewTemplateItem : TileViewItem {
		public TileViewTemplateItem(TileView view) {
			this.View = view;
		}
		protected override void OnPropertiesChanged() {
			base.OnPropertiesChanged();
			OnTileTemplateChanged();
		}
		void OnTileTemplateChanged() {
			if(View != null)
				View.OnTileTemplateChanged();
		}
	}
	public class TileViewItemElement : TileItemElement {
		protected override TileItemElement CreateTileItemElement() {
			return new TileViewItemElement();
		}
		TileView TileView {
			get {
				var item = GetOwnerItem() as TileViewTemplateItem;
				if(item == null) return null;
				return item.View;
			}
		}
		GridColumn column;
		[DXCategory(CategoryName.Data)]
		public GridColumn Column {
			get { return column; }
			set {
				if(column == value) return;
				column = value;
				OnColumnChanged();
			}
		}
		protected virtual void OnColumnChanged() {
			if(column != null && !string.IsNullOrEmpty(column.FieldName))
				this.Text = column.Name;
		}
		protected override string GetDisplayText() {
			if(DrawAsImageField)
				return string.Empty;
			if(DrawAsField)
				return Column.GetCaption();
			return Text;
		}
		protected override Image GetDisplayImage() {
			if(DrawAsImageField)
				return TileViewElementInfo.ImgBindingImage;
			return base.GetDisplayImage();
		}
		public override void Assign(TileItemElement src) {
			base.Assign(src);
			AssignExt(src);
		}
		public override void AssignWithoutDefault(TileItemElement src, bool assignText, bool assignImage) {
			base.AssignWithoutDefault(src, assignText, assignImage);
			AssignExt(src);
		}
		void AssignExt(TileItemElement src) {
			var src2 = src as TileViewItemElement;
			if(src2 != null)
				this.Column = src2.Column;
		}
		protected internal bool DrawAsField {
			get { 
				return Column != null &&
				Column.columns != null &&
				!string.IsNullOrEmpty(Column.FieldName); 
			}
		}
		protected internal bool DrawAsImageField {
			get {
				if(!DrawAsField || TileView == null) return false;
				var repItem = TileView.GetRowCellRepositoryItem(0, this.Column);
				if(repItem != null && string.Equals(repItem.EditorTypeName, "PictureEdit"))
					return true;
				return false;
			}
		}
	}
	public class TileViewGroup : TileGroup {
		public TileViewGroup() {
			this.RowHandle = 1;
		}
		protected internal bool IsDefault { get; set; }
		protected internal int RowHandle { get; set; }
		protected override TileGroupViewInfo CreateViewInfo() {
			return new TileViewGroupInfo(this);
		}
	}
	public class TileViewItemElementCollection : TileItemElementCollection { 
		public TileViewItemElementCollection(INotifyElementPropertiesChanged owner)
			: base(owner) {
		}
	}
	public class TileViewAppearances : BaseViewAppearanceCollection {
		AppearanceObject itemNormal, itemHovered, itemPressed, itemSelected, itemFocused;
		AppearanceObject groupText;
		AppearanceObject viewCaption;
		AppearanceObject emptySpace;
		public TileViewAppearances(BaseView view) : base(view) { }
		protected override void CreateAppearances() {
			itemNormal = CreateAppearance("ItemNormal");
			itemHovered = CreateAppearance("ItemHovered");
			itemPressed = CreateAppearance("ItemPressed");
			itemSelected = CreateAppearance("ItemSelected");
			itemFocused = CreateAppearance("ItemFocused");
			groupText = CreateAppearance("GroupText");
			viewCaption = CreateAppearance("ViewCaption");
			emptySpace = CreateAppearance("EmptySpace");
		}
		void ResetItemNormal() { ItemNormal.Reset(); }
		bool ShouldSerializeItemNormal() { return ItemNormal.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ItemNormal { get { return itemNormal; } }
		void ResetItemHovered() { ItemHovered.Reset(); }
		bool ShouldSerializeItemHovered() { return ItemHovered.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ItemHovered { get { return itemHovered; } }
		void ResetItemPressed() { ItemPressed.Reset(); }
		bool ShouldSerializeItemPressed() { return ItemPressed.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ItemPressed { get { return itemPressed; } }
		void ResetItemSelected() { ItemSelected.Reset(); }
		bool ShouldSerializeItemSelected() { return ItemSelected.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ItemSelected { get { return itemSelected; } }
		void ResetItemFocused() { ItemFocused.Reset(); }
		bool ShouldSerializeItemFocused() { return ItemFocused.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ItemFocused { get { return itemFocused; } }
		void ResetGroupText() { GroupText.Reset(); }
		bool ShouldSerializeGroupText() { return GroupText.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject GroupText { get { return groupText; } }
		void ResetViewCaption() { ViewCaption.Reset(); }
		bool ShouldSerializeViewCaption() { return ViewCaption.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject ViewCaption { get { return viewCaption; } }
		void ResetEmptySpace() { EmptySpace.Reset(); }
		bool ShouldSerializeEmptySpace() { return EmptySpace.ShouldSerialize(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject EmptySpace { get { return emptySpace; } }
	}
	public class TileViewItemAppearances : TileItemAppearances {
		AppearanceObject appearaceFocused;
		public TileViewItemAppearances() : this((IAppearanceOwner)null) { }
		public TileViewItemAppearances(AppearanceDefault defaultApp) : base(defaultApp as AppearanceDefault) { }
		public TileViewItemAppearances(AppearanceObject obj) : base(obj as AppearanceObject) { }
		public TileViewItemAppearances(IAppearanceOwner owner) : base(owner as IAppearanceOwner) {
			this.appearaceFocused = CreateAppearance();
		}
		void ResetFocused() { Focused.Reset(); }
		bool ShouldSerializeFocused() { return this.appearaceFocused != null && Focused.ShouldSerialize(); }
		[Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Focused {
			get { return appearaceFocused; }
		}
		protected override void OnResetCore() {
			base.OnResetCore();
			Focused.Reset();
		}
		public override void Assign(AppearanceDefault app) {
			base.Assign(app);
			Focused.Assign(app);
		}
		public override void Assign(AppearanceObject app) {
			base.Assign(app);
			Focused.Assign(app);
		}
		public override void Assign(TileItemAppearances app) {
			base.Assign(app);
			TileViewItemAppearances tvapp = app as TileViewItemAppearances;
			if(tvapp == null) return;
			Focused.Assign(tvapp.Focused);
		}
		public override void Dispose() {
			base.Dispose();
			DestroyAppearance(Focused);
		}
	}
	public static class TileViewImageLoadHelper {
		public static Image GetImage(TileView view, int rowHandle, GridColumn column) {
			if(view == null || column == null) return null;
			Image img;
			var value = view.DataController.GetRowValue(rowHandle, column.ColumnInfo);
			if(TryGetImageValue(value, out img))
				return img;
			else
				return null;
		}
		public static bool TryGetImageValue(object value, out Image result) {
			result = null;
			if(value is Image) {
				result = value as Image;
				return true;
			}
			if(value is byte[]) {
				try {
					result = DevExpress.XtraEditors.Controls.ByteImageConverter.FromByteArray(value as byte[]);
					return true;
				}
				catch(ArgumentException) { }
			}
			return false;
		}
	}
}
