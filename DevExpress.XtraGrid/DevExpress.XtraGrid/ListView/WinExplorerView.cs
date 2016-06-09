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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.WinExplorer.Handler;
using DevExpress.XtraGrid.Views.WinExplorer.ViewInfo;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Gesture;
using DevExpress.XtraGrid.WinExplorer;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Views.WinExplorer.Drawing;
namespace DevExpress.XtraGrid.Views.WinExplorer {
	[
	DesignTimeVisible(false), ToolboxItem(false),
	Designer("DevExpress.XtraGrid.Design.WinExplorerViewComponentDesigner, " + AssemblyInfo.SRAssemblyGridDesign)
	]
	public class WinExplorerView : ColumnView, IContextItemCollectionOwner, IContextItemCollectionOptionsOwner {
		private Dictionary<int, CheckState> checkedGroupCache;
		private static readonly object itemDrag = new object();
		private static object itemClick = new object();
		private static readonly object contextButtonCustomize = new object();
		private static readonly object contextButtonClick = new object();
		private static readonly object customContextButtonToolTip = new object();
		private static object itemDoubleClick = new object();
		private static object itemKeyDown = new object();
		private static object customDrawItem = new object();
		private static object customDrawGroupItem = new object();
		private static object getThumbnailImage = new object();
		private static object getLoadingImage = new object();
		private static object marqueeSelectionCompleted = new object();
		private static object marqueeSelectionStarted = new object();
		WinExplorerViewOptionsNavigation _optionsNavigation;
		public WinExplorerView() {
			this.optionsViewStyles = new WinExplorerViewStyleOptionsCollection(this);
			SortInfo.MaxGroupCount = 1;
			_optionsNavigation = CreateOptionsNavigation();
			this._optionsNavigation.Changed += new BaseOptionChangedEventHandler(OnOptionChanged); 
			checkedGroupCache = new Dictionary<int, CheckState>();
		}
		protected virtual WinExplorerViewOptionsNavigation CreateOptionsNavigation() {
			return new WinExplorerViewOptionsNavigation();
		}
		protected internal override ToolTipControlInfo GetToolTipObjectInfo(Point p) {
			return base.GetToolTipObjectInfo(p);
		}
		protected internal override bool CheckViewInfo() {
			if(WinExplorerViewInfo.GetIsDataDirty())
				ViewInfo.IsDataDirty = true;
			return base.CheckViewInfo();
		}
		protected override string ViewName {
			get {
				return "WinExplorerView";
			}
		}
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
		WinExplorerContextItemCollectionOptions contextButtonOptions;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter)), DXCategory(CategoryName.ContextButtons)]
		public WinExplorerContextItemCollectionOptions ContextButtonOptions {
			get {
				if(contextButtonOptions == null)
					contextButtonOptions = CreateContextButtonOptions();
				return contextButtonOptions;
			}
		}
		protected virtual WinExplorerContextItemCollectionOptions CreateContextButtonOptions() {
			return new WinExplorerContextItemCollectionOptions(this);
		}
		protected override void OnDataController_DataSourceChanged(object sender, EventArgs e) {
			if(WinExplorerViewInfo != null) {
				WinExplorerViewInfo.ClearItemsCache = true;
				WinExplorerViewInfo.Calculator.InvalidateCache();
				if(DataController.GroupInfo.Count > 0 && OptionsView.ShowCheckBoxInGroupCaption) {
					ResetGroupCheckedCache();
				}
			}
			base.OnDataController_DataSourceChanged(sender, e);
		}
		protected override void OnDataController_ListChanged(object sender, ListChangedEventArgs e) {
			if(e.ListChangedType == ListChangedType.Reset && OptionsImageLoad.ClearCacheOnDataSourceUpdate) 
				ClearImageLoader();
			base.OnDataController_ListChanged(sender, e);
		}
		protected internal override void ApplyFindFilterCore(string filter) {
			if(filter == null) filter = string.Empty;
			if(FindFilterText != filter) {
				ResetThumbnailCache();
			}
			base.ApplyFindFilterCore(filter);
		}
		public void ResetGroupCheckedCache() {
			WinExplorerViewInfo.CheckedCache.Clear();
			foreach(GroupRowInfo group in DataController.GroupInfo) {
				WinExplorerViewInfo.CheckedCache.Add(group.Handle, CheckState.Indeterminate);
			}
		}
		protected override void UpdateFocusedRowHandleOnEnter() {
		}
		protected internal override void OnDataSourceChanging() {
			ClearImageLoader();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ClearImageLoader() {
			if(WinExplorerViewInfo != null) WinExplorerViewInfo.ClearImageLoader();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void ResetThumbnailCache() {
			ClearLoadInfo();
		}
		protected internal virtual void ClearLoadInfo() {
			if(WinExplorerViewInfo != null) WinExplorerViewInfo.ClearLoadInfo();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(WinExplorerViewInfo != null) {
					WinExplorerViewInfo.ImageLoader.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		protected override void OnCurrentControllerRowChanged(CurrentRowEventArgs e) {
			WinExplorerViewInfo.AllowMakeItemVisible = false;
			try {
				if(WinExplorerViewInfo != null && GridControl.IsLoaded) {
					WinExplorerViewInfo.FocusedRowHandle = DataController.CurrentControllerRow;
					if(!FocusedRowChangedByHandler) {
						ClearSelection();
						if(DataController.CurrentControllerRow != GridControl.InvalidRowHandle)
							DataController.Selection.SetSelected(DataController.CurrentControllerRow, true);
					}
				}
				if(!IsInitialized) return;
			}
			finally {
				WinExplorerViewInfo.AllowMakeItemVisible = true;
			}
		}
		internal WinExplorerViewHandler WinExplorerViewHandler { get { return Handler as WinExplorerViewHandler; } }
		internal WinExplorerViewPainter WinExplorerViewPainter {
			get {
				return Painter as WinExplorerViewPainter;
			}
		}
		public override void DeleteSelectedRows() {
			if(!IsMultiSelect && WinExplorerViewInfo.FocusedRowHandle == GridControl.InvalidRowHandle)
				return;
			base.DeleteSelectedRows();
		}
		protected override BaseHitInfo CalcHitInfoCore(Point pt) {
			if(WinExplorerViewInfo != null)
				return WinExplorerViewInfo.CalcHitInfo(pt);
			return base.CalcHitInfoCore(pt);
		}
		public override void RefreshData() {
			WinExplorerViewInfo.ClearImageLoader();
			base.RefreshData();
			WinExplorerViewInfo.ActiveEditorInfo = null;
		}
		public override void Assign(BaseView v, bool copyEvents) {
			base.Assign(v, copyEvents);
			WinExplorerView wev = v as WinExplorerView;
			if(wev == null)
				return;
			ColumnSet.Assign(wev.ColumnSet);
			if(copyEvents) {
				Events.AddHandler(itemDrag, wev.Events[itemDrag]);
				Events.AddHandler(itemClick, wev.Events[itemClick]);
				Events.AddHandler(itemDoubleClick, wev.Events[itemDoubleClick]);
				Events.AddHandler(itemKeyDown, wev.Events[itemKeyDown]);
				Events.AddHandler(customDrawItem, wev.Events[customDrawItem]);
				Events.AddHandler(getThumbnailImage, wev.Events[getThumbnailImage]);
				Events.AddHandler(getLoadingImage, wev.Events[getLoadingImage]);
			}
		}
		protected override List<IDataColumnInfo> GetFindToColumnsCollection() {
			return base.GetFindToColumnsCollection();
		}
		protected override bool ClearSelectionAllowed {
			get { return true; }
		}
		protected internal override void RefreshVisibleColumnsList() {
			if(IsLoading)
				return;
			WinExplorerViewStyle style = OptionsView.Style;
			VisibleColumnsCore.ClearCore();
			if(HasTextField) {
				VisibleColumnsCore.AddCore(ColumnSet.TextColumn);
			}
			if(HasDescriptionField && WinExplorerViewInfo != null && WinExplorerViewInfo.GetShowDescription()) {
				VisibleColumnsCore.AddCore(ColumnSet.DescriptionColumn);
			}
		}
		void ResetOptionsNavigation() { OptionsNavigation.Reset(); }
		bool ShouldSerializeOptionsNavigation() { return OptionsNavigation.ShouldSerializeCore(this); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("WinExplorerViewOptionsNavigation"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdOptionsView)]
		public GridOptionsNavigation OptionsNavigation {
			get { return _optionsNavigation; }
		}
		bool ShouldSerializeOptionsTouch() { return OptionsImageLoad.ShouldSerializeCore(this); }
		void ResetOptionsTouch() { OptionsImageLoad.Reset(); }
		WinExplorerViewOptionsImageLoad optionsImageLoad;
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("WinExplorerViewOptionsImageLoad"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public WinExplorerViewOptionsImageLoad OptionsImageLoad {
			get {
				if(optionsImageLoad == null)
					optionsImageLoad = CreateOptionsImageLoad();
				return optionsImageLoad;
			}
		}
		bool ShouldSerializeOptionsItem() { return OptionsViewStyles.ShouldSerializeCore(this); }
		void ResetOptionsItem() { OptionsViewStyles.Reset(); }
		WinExplorerViewStyleOptionsCollection optionsViewStyles;
		[DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdOptionsView)]
		public WinExplorerViewStyleOptionsCollection OptionsViewStyles {
			get { return optionsViewStyles; }
		}
		protected internal object GetRowKeyCore(int rowHandle) {
			return GetRowKey(rowHandle);
		}
		WinExplorerViewColumns columnSet;
		void ResetColumnSet() { ColumnSet.Reset(); }
		bool ShouldSerializeColumnSet() { return ColumnSet.ShouldSerialize(); }
		[DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public WinExplorerViewColumns ColumnSet {
			get {
				if(columnSet == null)
					columnSet = CreateColumnSet();
				return columnSet;
			}
		}
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
		protected virtual WinExplorerViewColumns CreateColumnSet() {
			return new WinExplorerViewColumns(this);
		}
		protected virtual WinExplorerViewOptionsImageLoad CreateOptionsImageLoad() {
			return new WinExplorerViewOptionsImageLoad(this);
		}
		internal bool HasTextField { get { return ColumnSet.HasTextField; } }
		internal bool HasDescriptionField { get { return ColumnSet.HasDescriptionField; } }
		internal bool HasExtraLargeImageField { get { return ColumnSet.HasExtraLargeImageField; } }
		internal bool HasLargeImageField { get { return ColumnSet.HasLargeImageField; } }
		internal bool HasMediumImageField { get { return ColumnSet.HasMediumImageField; } }
		internal bool HasSmallImageField { get { return ColumnSet.HasSmallImageField; } }
		internal bool HasExtraLargeImageIndexField { get { return ColumnSet.HasExtraLargeImageIndexField; } }
		internal bool HasLargeImageIndexField { get { return ColumnSet.HasLargeImageIndexField; } }
		internal bool HasMediumImageIndexField { get { return ColumnSet.HasMediumImageIndexField; } }
		internal bool HasSmallImageIndexField { get { return ColumnSet.HasSmallImageIndexField; } }
		internal bool HasCheckField { get { return ColumnSet.HasCheckField; } }
		internal bool HasEnabledField { get { return ColumnSet.HasEnabledField; } }
		internal bool HasGroupField { get { return ColumnSet.HasGroupField; } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("WinExplorerViewOptionsBehavior"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdOptionsView)]
		public new WinExplorerViewOptionsBehavior OptionsBehavior { get { return base.OptionsBehavior as WinExplorerViewOptionsBehavior; } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("WinExplorerViewOptionsView"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdOptionsView)]
		public new WinExplorerViewOptionsView OptionsView { get { return base.OptionsView as WinExplorerViewOptionsView; } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("WinExplorerViewOptionsSelection"),
#endif
 DXCategory(CategoryName.Options), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue), XtraSerializablePropertyId(LayoutIdOptionsView)]
		public new WinExplorerViewOptionsSelection OptionsSelection { get { return base.OptionsSelection as WinExplorerViewOptionsSelection; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public new ColumnViewOptionsFilter OptionsFilter { get { return base.OptionsFilter; } }
		protected override ColumnViewOptionsBehavior CreateOptionsBehavior() {
			return new WinExplorerViewOptionsBehavior(this);
		}
		protected override ColumnViewOptionsView CreateOptionsView() {
			return new WinExplorerViewOptionsView(this);
		}
		protected override ColumnViewOptionsSelection CreateOptionsSelection() {
			return new WinExplorerViewOptionsSelection();
		}
		protected override ToolTipControlInfo GetToolTipObjectInfoCore(Utils.Drawing.GraphicsCache cache, Point p) {
			WinExplorerViewHitInfo hi = CalcHitInfo(p);
			ToolTipControlInfo res = null;
			if(hi.InItem) {
				res = hi.ItemInfo.ContextButtonsViewInfo.GetToolTipInfo(p);
				if(res != null)
					return res;
			}
			if(!hi.InItem || !hi.ItemInfo.ShowToolTip) return null;
			res = new ToolTipControlInfo();
			res.ToolTipLocation = ToolTipLocation.RightBottom;
			GridControl.EditorHelper.RealToolTipController.AllowHtmlText = OptionsView.AllowHtmlText;
			res.Text = GetToolTipText(hi.ItemInfo, p);
			res.Title = hi.ItemInfo.Text;
			res.Object = hi.ItemInfo;
			return res;
		}
		protected internal override string GetToolTipText(object hintObject, Point p) {
			if(hintObject is WinExplorerItemViewInfo)
				return (hintObject as WinExplorerItemViewInfo).Description;
			return base.GetToolTipText(hintObject, p);
		}
		protected internal override void OnActionScroll(ScrollNotifyAction action) {
			if(!CanActionScroll(action)) return;
			ScrollInfo.OnAction(action);
		}
		public void Invalidate(Rectangle rect) {
			if(GridControl != null)
				GridControl.Invalidate(rect);
		}
		protected internal virtual bool AllowMarqueeSelection { get { return OptionsSelection.AllowMarqueeSelection && IsMultiSelect; } }
		protected internal override bool ShouldProcessOuterMouseEvents {
			get {
				return AllowMarqueeSelection && !WinExplorerViewInfo.SelectionRect.Size.IsEmpty;
			}
		}
		protected internal override GestureAllowArgs[] CheckAllowGestures(Point point) {
			if(OptionsView.Style == WinExplorerViewStyle.List)
				return new GestureAllowArgs[] { GestureAllowArgs.Pan };
			return new GestureAllowArgs[] { GestureAllowArgs.PanVertical };
		}
		protected internal override void OnTouchScroll(GestureArgs info, Point delta, ref Point overPan) {
			WinExplorerViewInfo.Calculator.OnTouchScroll(info, delta, overPan);
		}
		public GridColumn GetColumn(WinExplorerViewFieldType fieldType) {
			switch(fieldType) {
				case WinExplorerViewFieldType.Text:
					return ColumnSet.TextColumn;
				case WinExplorerViewFieldType.Description:
					return ColumnSet.DescriptionColumn;
				case WinExplorerViewFieldType.CheckBox:
					return ColumnSet.CheckBoxColumn;
				case WinExplorerViewFieldType.Image:
					return GetImageColumn();
				default: throw new ArgumentException("Unknown fieldType");
			}
		}
		protected GridColumn GetGroupColumn() {
			return SortInfo.GroupCount > 0 && SortInfo.Count > 0 ? SortInfo[0].Column : null;
		}
		public void AssignTextField(string fieldName) {
			AssignFieldCore(WinExplorerViewFieldType.Text, fieldName);
		}
		public void AssignDescriptionField(string fieldName) {
			AssignFieldCore(WinExplorerViewFieldType.Description, fieldName);
		}
		public void AssignCheckBoxField(string fieldName) {
			AssignFieldCore(WinExplorerViewFieldType.CheckBox, fieldName);
		}
		public void AssignExtraLargeImageField(string fieldName) {
			AssignImageFieldCore(WinExplorerViewStyle.ExtraLarge, fieldName);
		}
		public void AssignLargeImageField(string fieldName) {
			AssignImageFieldCore(WinExplorerViewStyle.Large, fieldName);
		}
		public void AssignMediumImageField(string fieldName) {
			AssignImageFieldCore(WinExplorerViewStyle.Medium, fieldName);
		}
		public void AssignSmallImageField(string fieldName) {
			AssignImageFieldCore(WinExplorerViewStyle.Small, fieldName);
		}
		protected virtual void AssignFieldCore(WinExplorerViewFieldType fieldType, string fieldName) {
			GridColumn column = GetColumn(fieldType);
			if(column != null) column.FieldName = fieldName;
		}
		protected virtual void AssignImageFieldCore(WinExplorerViewStyle itemType, string fieldName) {
			GridColumn column = GetImageColumn(itemType);
			if(column != null) column.FieldName = fieldName;
		}
		protected internal GridColumn GetImageColumn() {
			return GetImageColumn(OptionsView.Style);
		}
		protected internal GridColumn GetImageColumn(WinExplorerViewStyle itemType) {
			switch(itemType) {
				case WinExplorerViewStyle.ExtraLarge:
					return ColumnSet.ExtraLargeImageColumn;
				case WinExplorerViewStyle.Large:
					return ColumnSet.LargeImageColumn;
				case WinExplorerViewStyle.Default:
				case WinExplorerViewStyle.Medium:
				case WinExplorerViewStyle.Tiles:
				case WinExplorerViewStyle.Content:
					return ColumnSet.MediumImageColumn;
				case WinExplorerViewStyle.Small:
				case WinExplorerViewStyle.List:
					return ColumnSet.SmallImageColumn;
				default: throw new ArgumentException("Unknown ItemType");
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override GridColumnSortInfoCollection SortInfo {
			get {
				return base.SortInfo;
			}
		}
		#region Events
		[DXCategory(CategoryName.Action)]   
		public event WinExplorerViewDragEventHandler ItemDrag {
			add { Events.AddHandler(itemDrag, value); }
			remove { Events.RemoveHandler(itemDrag, value); }
		}
		protected internal virtual WinExplorerViewDragEventArgs RaiseItemDrag(MouseEventArgs e) {
			WinExplorerViewDragEventHandler handler = Events[itemDrag] as WinExplorerViewDragEventHandler;
			if(handler != null) {
				WinExplorerViewDragEventArgs de = new WinExplorerViewDragEventArgs() { Button = e.Button, Items = DataController.Selection.GetSelectedRows() };
				handler(this, de);
				return de;
			}
			return null;
		}
		[DXCategory(CategoryName.Action)]
		public event WinExplorerViewItemClickEventHandler ItemClick {
			add { Events.AddHandler(itemClick, value); }
			remove { Events.RemoveHandler(itemClick, value); }
		}
		[DXCategory(CategoryName.ContextButtons)]
		public event WinExplorerViewContextButtonCustomizeEventHandler ContextButtonCustomize {
			add { Events.AddHandler(contextButtonCustomize, value); }
			remove { Events.RemoveHandler(contextButtonCustomize, value); }
		}
		[DXCategory(CategoryName.ContextButtons)]
		public event ContextItemClickEventHandler ContextButtonClick {
			add { Events.AddHandler(contextButtonClick, value); }
			remove { Events.RemoveHandler(contextButtonClick, value); }
		}
		[DXCategory(CategoryName.ContextButtons)]
		public event WinExplorerViewContextButtonToolTipEventHandler CustomContextButtonToolTip {
			add { Events.AddHandler(customContextButtonToolTip, value); }
			remove { Events.RemoveHandler(customContextButtonToolTip, value); }
		}
		protected internal virtual void RaiseCustomContextButtonToolTip(WinExplorerViewContextButtonToolTipEventArgs e) {
			WinExplorerViewContextButtonToolTipEventHandler handler = (WinExplorerViewContextButtonToolTipEventHandler)Events[customContextButtonToolTip];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseContextButtonCustomize(WinExplorerViewContextButtonCustomizeEventArgs e) {
			WinExplorerViewContextButtonCustomizeEventHandler handler = (WinExplorerViewContextButtonCustomizeEventHandler)Events[contextButtonCustomize];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseContextItemClick(ContextItemClickEventArgs e) {
			ContextItemClickEventHandler handler = Events[contextButtonClick] as ContextItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal virtual void RaiseItemClick(WinExplorerViewItemClickEventArgs e) {
			WinExplorerViewItemClickEventHandler handler = (WinExplorerViewItemClickEventHandler)Events[itemClick];
			if(handler != null) handler(this, e);
		}
		[DXCategory(CategoryName.Action)]
		public event WinExplorerViewItemDoubleClickEventHandler ItemDoubleClick {
			add { Events.AddHandler(itemDoubleClick, value); }
			remove { Events.RemoveHandler(itemDoubleClick, value); }
		}
		protected internal virtual void RaiseMarqueeSelectionCompleted(EventArgs e) {
			EventHandler handler = (EventHandler)Events[marqueeSelectionCompleted];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseMarqueeSelectionStarted(EventArgs e) {
			EventHandler handler = (EventHandler)Events[marqueeSelectionStarted];
			if(handler != null) handler(this, e);
		}
		[DXCategory(CategoryName.Action)]
		public event EventHandler MarqueeSelectionCompleted {
			add { Events.AddHandler(marqueeSelectionCompleted, value); }
			remove { Events.RemoveHandler(marqueeSelectionCompleted, value); }
		}
		[DXCategory(CategoryName.Action)]
		public event EventHandler MarqueeSelectionStarted {
			add { Events.AddHandler(marqueeSelectionStarted, value); }
			remove { Events.RemoveHandler(marqueeSelectionStarted, value); }
		}
		protected internal virtual void RaiseItemDoubleClick(WinExplorerViewItemDoubleClickEventArgs e) {
			WinExplorerViewItemDoubleClickEventHandler handler = (WinExplorerViewItemDoubleClickEventHandler)Events[itemDoubleClick];
			if(handler != null) handler(this, e);
		}
		[DXCategory(CategoryName.Data)]
		public event ThumbnailImageEventHandler GetThumbnailImage {
			add { Events.AddHandler(getThumbnailImage, value); }
			remove { Events.RemoveHandler(getThumbnailImage, value); }
		}
		protected internal virtual ThumbnailImageEventArgs RaiseGetThumbnailImage(ThumbnailImageEventArgs e) {
			ThumbnailImageEventHandler handler = (ThumbnailImageEventHandler)Events[getThumbnailImage];
			if(handler != null) handler(this, e);
			return e;
		}
		[DXCategory(CategoryName.Appearance)]
		public event GetLoadingImageEventHandler GetLoadingImage {
			add { Events.AddHandler(getLoadingImage, value); }
			remove { Events.RemoveHandler(getLoadingImage, value); }
		}
		protected internal virtual Image RaiseGetLoadingImage(GetLoadingImageEventArgs e) {
			GetLoadingImageEventHandler handler = (GetLoadingImageEventHandler)Events[getLoadingImage];
			if(handler != null) handler(this, e);
			return e.LoadingImage;
		}
		[DXCategory(CategoryName.Action)]
		public event WinExplorerViewItemKeyDownEventHandler ItemKeyDown {
			add { Events.AddHandler(itemKeyDown, value); }
			remove { Events.RemoveHandler(itemKeyDown, value); }
		}
		protected internal virtual void RaiseItemKeyDown(WinExplorerViewItemKeyEventArgs e){
			WinExplorerViewItemKeyDownEventHandler handler = (WinExplorerViewItemKeyDownEventHandler)Events[itemKeyDown];
			if(handler != null) handler(this, e);
		}
		[DXCategory(CategoryName.CustomDraw)]
		public event WinExplorerViewCustomDrawItemEventHandler CustomDrawItem {
			add { Events.AddHandler(customDrawItem, value); }
			remove { Events.RemoveHandler(customDrawItem, value); }
		}
		protected internal virtual void RaiseCustomDrawItem(WinExplorerViewCustomDrawItemEventArgs e) {
			WinExplorerViewCustomDrawItemEventHandler handler = (WinExplorerViewCustomDrawItemEventHandler)Events[customDrawItem];
			if(handler != null)
				handler(this, e);
		}
		[DXCategory(CategoryName.CustomDraw)]
		public event WinExplorerViewCustomDrawGroupItemEventHandler CustomDrawGroupItem {
			add { Events.AddHandler(customDrawGroupItem, value); }
			remove { Events.RemoveHandler(customDrawGroupItem, value); }
		}
		protected internal virtual void RaiseCustomDrawGroupItem(WinExplorerViewCustomDrawGroupItemEventArgs e) {
			WinExplorerViewCustomDrawGroupItemEventHandler handler = (WinExplorerViewCustomDrawGroupItemEventHandler)Events[customDrawGroupItem];
			if (handler != null)
				handler(this, e);
		}
		#endregion
		public override void ShowEditor() {
			ShowEditor(WinExplorerViewColumnType.Text);
		}
		internal GridColumn EditedColumn { get; set; }
		public virtual void ShowEditor(WinExplorerViewColumnType columnType) {
			ShowEditor(columnType, false);
		}
		protected bool ShouldShowEditorDelayed(WinExplorerItemViewInfo itemInfo, bool byMouse) {
			if(itemInfo == null)
				return true;
			if(!itemInfo.IsFullyVisible)
				return !byMouse;
			return false;
		}
		public virtual void ShowEditor(WinExplorerViewColumnType columnType, bool byMouse) {
			EditedColumn = columnType == WinExplorerViewColumnType.Text ? ColumnSet.TextColumn : ColumnSet.DescriptionColumn;
			if (IsDisposing || !GetCanShowEditor(EditedColumn)) return;
			WinExplorerItemViewInfo itemInfo = WinExplorerViewInfo.GetItemByRow(WinExplorerViewInfo.FocusedRowHandle);
			if (ShouldShowEditorDelayed(itemInfo, byMouse)) {
				WinExplorerViewInfo.ShowEditorWhenItemBecomesVisible = true;
				EditedColumn = null;
				return;
			}
			ActivateEditor(GetActiveEditItemInfo(), columnType);
		}
		protected virtual WinExplorerItemViewInfo GetActiveEditItemInfo() {
			if(WinExplorerViewInfo.ActiveEditorInfo.ItemInfo != null)
				return WinExplorerViewInfo.ActiveEditorInfo.ItemInfo;
			if(WinExplorerViewInfo.FocusedRowHandle != GridControl.InvalidRowHandle)
				return WinExplorerViewInfo.GetItemByRow(WinExplorerViewInfo.FocusedRowHandle);
			return null;
		}
		protected internal bool DelayRaiseFocusedRowChangedEvent { get; set; }
		protected internal int DelayedPrevFocusedRowHandle { get; set; }
		protected internal int DelayedFocusedRowHandle { get; set; }
		internal void RaiseDelayedFocusedRowChanged() {
			if(!DelayRaiseFocusedRowChangedEvent)
				return;
			DelayRaiseFocusedRowChangedEvent = false;
			RaiseFocusedRowChanged(DelayedPrevFocusedRowHandle, DelayedFocusedRowHandle);
		}
		protected override void RaiseFocusedRowChanged(int prevFocused, int focusedRowHandle) {
			if(DelayRaiseFocusedRowChangedEvent) {
				DelayedPrevFocusedRowHandle = prevFocused;
				DelayedFocusedRowHandle = focusedRowHandle;
				return;
			}
			base.RaiseFocusedRowChanged(prevFocused, focusedRowHandle);
		}
		protected override void DoChangeFocusedRow(int currentRowHandle, int newRowHandle, bool raiseUpdateCurrentRow) {
			base.DoChangeFocusedRow(currentRowHandle, newRowHandle, raiseUpdateCurrentRow);
			SetFocusedRowHandleCore(newRowHandle);
			if(IsInitialized) {
				DataController.CurrentControllerRow = FocusedRowHandle;
				if(WinExplorerViewInfo != null) {
					WinExplorerViewInfo.FocusedRowHandle = FocusedRowHandle;
					if(WinExplorerViewInfo.IsPressedInfoChanged && !OptionsBehavior.AutoScrollItemOnMouseClick)
						return;
					WinExplorerViewInfo.Calculator.ScrollToItem(FocusedRowHandle);
				}
			}
		}
		protected virtual void ActivateEditor(WinExplorerItemViewInfo vi, WinExplorerViewColumnType itemEdit) {
			GridColumn editedColumn = itemEdit == WinExplorerViewColumnType.Text ? ColumnSet.TextColumn : ColumnSet.DescriptionColumn;
			if (vi == null || !editedColumn.OptionsColumn.AllowEdit)
				return;
			Rectangle bounds = vi.GetEditorBounds(itemEdit);
			if(bounds.IsEmpty) return;
			RepositoryItem itemEditor = editedColumn.ColumnEdit;
			if (itemEditor == null) itemEditor = GetColumnDefaultRepositoryItem(editedColumn);
			string appearanceName = itemEdit == WinExplorerViewColumnType.Text ? "ItemNormal" : "ItemDescriptionNormal";
			UpdateEditor(itemEditor, new UpdateEditorInfoArgs(GetColumnReadOnly(editedColumn), bounds, ViewInfo.PaintAppearance.GetAppearance(appearanceName), GetRowCellValue(vi.Row.RowHandle, editedColumn), ElementsLookAndFeel, "", null));
			WinExplorerViewInfo.ActiveEditorInfo.ItemInfo = vi;
		}
		RepositoryItem defaultDescriptionRepositoryItem;
		protected virtual RepositoryItem DefaultDescriptionRepositoryItem {
			get {
				if (defaultDescriptionRepositoryItem == null) {
					defaultDescriptionRepositoryItem = new RepositoryItemMemoEdit();
					GridControl.EditorHelper.DefaultRepository.Items.Add(defaultDescriptionRepositoryItem);
				}
				return defaultDescriptionRepositoryItem;
			}
		}
		protected internal override RepositoryItem GetColumnDefaultRepositoryItem(GridColumn column) {
			if (GridControl == null) return null;
			if (column != ColumnSet.DescriptionColumn)
				return base.GetColumnDefaultRepositoryItem(column);
			return DefaultDescriptionRepositoryItem;
		}
		protected override void OnActiveFilterChanged(object sender, EventArgs e) {
			ResetThumbnailCache();
			base.OnActiveFilterChanged(sender, e);
		}
		public override void AddNewRow() {
			ClearImageLoader();
			base.AddNewRow();
		}
		protected override void DeleteRowCore(int rowHandle) {
			ClearImageLoader();
			base.DeleteRowCore(rowHandle);
		}
		protected override void SetRowCellValueCore(int rowHandle, GridColumn column, object _value, bool fromEditor) {
			if(WinExplorerViewInfo != null) WinExplorerViewInfo.ClearItemLoadInfo(rowHandle, column);
			base.SetRowCellValueCore(rowHandle, column, _value, fromEditor);
			UpdateItemViewInfo(rowHandle);
			if(column == ColumnSet.CheckBoxColumn && OptionsView.ShowCheckBoxInGroupCaption && DataController.GroupInfo.Count > 0) {
				int groupRowHandle = GetParentRowHandle(rowHandle);
				WinExplorerGroupViewInfo groupInfo = WinExplorerViewInfo.GetItemByRow(groupRowHandle) as WinExplorerGroupViewInfo;
				if(groupInfo != null && !groupInfo.Clicked) {
					if(WinExplorerViewInfo.CheckedCache.ContainsKey(groupRowHandle)) {
						WinExplorerViewInfo.CheckedCache[groupRowHandle] = CheckState.Indeterminate;
					}
				}
			}
			InvalidateItem(rowHandle);
		}
		protected virtual void InvalidateItem(int rowHandle) {
			WinExplorerItemViewInfo itemInfo = WinExplorerViewInfo.GetItemByRow(rowHandle);
			if(itemInfo != null)
				GridControl.Invalidate(itemInfo.Bounds);
		}
		protected virtual void UpdateItemViewInfo(int rowHandle) {
			WinExplorerItemViewInfo itemInfo = WinExplorerViewInfo.GetItemByRow(rowHandle);
			if(itemInfo == null)
				return;
			bool useNullGraphics = ViewInfo.GInfo.Graphics == null;
			if(useNullGraphics)
				ViewInfo.GInfo.AddGraphics(null);
			try {
				itemInfo.CalcFieldsInfo();
				itemInfo.Arrange();
			}
			finally {
				if(useNullGraphics)
					ViewInfo.GInfo.ReleaseGraphics();
			}
		}
		protected internal override void DoMoveFocusedRow(int delta, KeyEventArgs e) {
			((WinExplorerViewHandler)Handler).Navigator.DoMoveFocusedRow(delta);
		}
		WinExplorerViewNavigatorBase Navigator { get { return ((WinExplorerViewHandler)Handler).Navigator; } }
		public override void MoveFirst() {
			Navigator.ClearPreferredColumnIndex();
			Navigator.MoveFirst();
		}
		public override void MoveNext() {
			Navigator.ClearPreferredColumnIndex();
			Navigator.MoveNext();
		}
		public override void MovePrev() {
			Navigator.ClearPreferredColumnIndex();
			Navigator.MovePrev();
		}
		public override void MoveLast() {
			Navigator.ClearPreferredColumnIndex();
			Navigator.MoveLast();
		}
		public override void MoveNextPage() {
			Navigator.ClearPreferredColumnIndex();
			Navigator.MoveNextPage();
		}
		public override void MovePrevPage() {
			Navigator.ClearPreferredColumnIndex();
			Navigator.MovePrevPage();
		}
		public override void MoveBy(int delta) {
			Navigator.ClearPreferredColumnIndex();
			Navigator.MoveBy(delta);
		}
		public override void MoveLastVisible() {
			Navigator.ClearPreferredColumnIndex();
			Navigator.MoveLastVisible();
		}
		protected internal override bool IsNavigatorActionEnabled(NavigatorButtonType type) {
			if(Handler is WinExplorerViewHandler) {
				if(type == NavigatorButtonType.Prev || type == NavigatorButtonType.First)
					return ((WinExplorerViewHandler)Handler).Navigator.GetAllowMoveItem(-1);
				if(type == NavigatorButtonType.Next || type == NavigatorButtonType.Last)
					return ((WinExplorerViewHandler)Handler).Navigator.GetAllowMoveItem(+1);
				if(type == NavigatorButtonType.NextPage)
					return ((WinExplorerViewHandler)Handler).Navigator.GetAllowMoveNextPage();
				if(type == NavigatorButtonType.PrevPage)
					return ((WinExplorerViewHandler)Handler).Navigator.GetAllowMovePrevPage();
			}
			return base.IsNavigatorActionEnabled(type);
		}
		public override bool IsEditing {
			get {
				return ActiveEditor != null;
			}
		}
		protected override void UpdateEditor(RepositoryItem ritem, UpdateEditorInfoArgs args) {
			ritem.DefaultBorderStyleInGrid = BorderStyles.Default;
			base.UpdateEditor(ritem, args);
		}
		protected override void UpdateEditorProperties(BaseEdit editor) {
			editor.Properties.AllowInplaceBorderPainter = false;
			editor.Properties.DefaultBorderStyleInGrid = BorderStyles.Default;
			base.UpdateEditorProperties(editor);
		}
		public virtual object GetGroupRowValue(int rowHandle, GridColumn column) {
			return DataController.GetGroupRowValue(rowHandle, column.ColumnInfo);
		}
		public virtual string GetGroupRowDisplayText(int rowHandle) {
			object val = GetGroupRowValue(rowHandle, ColumnSet.GroupColumn);
			return GetGroupRowDisplayText(rowHandle, ColumnSet.GroupColumn, val, FormatInfo.Empty);
		}
		protected internal override void OnLookAndFeelChanged() {
			WinExplorerViewInfo.Calculator.RemoveAnimations();
			base.OnLookAndFeelChanged();
		}
		protected override void SetupDataController() {
			base.SetupDataController();
			DataController.AutoExpandAllGroups = true;
		}
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
		protected virtual ScrollInfo CreateScrollInfo() {
			return new ScrollInfo(this);
		}
		protected bool ShouldInvalidateAllClient { get; set; }
		protected void OnVScroll(object sender, EventArgs e) {
			OnScroll(ScrollInfo.VScroll.Value);
		}
		protected void OnHScroll(object sender, EventArgs e) {
			OnScroll(ScrollInfo.HScroll.Value);
		}
		protected virtual void OnScroll(int value) {
			if(WinExplorerViewInfo.Calculator.SuppressPositionUpdate)
				return;
			ShouldInvalidateAllClient = Position != value;
			Position = value;
		}
		protected internal override void CreateHandles() {
			base.CreateHandles();
			if(GridControl.IsHandleCreated)
				ScrollInfo.CreateHandles();
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
		protected override void OnPopulateColumns() {
			CheckParseColumns();
			base.OnPopulateColumns();
		}
		void CheckParseColumns() {
			ColumnSet.BeginUpdate();
			try {
				ColumnSet.Reset();
				if(DataController.IsReady && DataController.Columns["Text"] != null && Columns["Text"] != null) {
					TryParseColumns();
				}
			}
			finally {
				ColumnSet.EndUpdate();
			}
		}
		protected virtual void TryParseColumns() {
			var column = DataController.Columns["Text"];
			if(column != null && column.PropertyDescriptor.ComponentType.IsAssignableFrom(typeof(WinExplorerViewItem))) {
				ColumnSet.TextColumn = Columns[WinExplorerViewItem.TextColumn];
				ColumnSet.ExtraLargeImageIndexColumn = Columns[WinExplorerViewItem.ExtraLargeImageIndexColumn];
				ColumnSet.CheckBoxColumn = Columns[WinExplorerViewItem.CheckColumn];
				ColumnSet.EnabledColumn = Columns[WinExplorerViewItem.EnabledColumn];
				ColumnSet.DescriptionColumn = Columns[WinExplorerViewItem.DescriptionColumn];
				ColumnSet.ExtraLargeImageColumn = Columns[WinExplorerViewItem.ExtraLargeImageColumn];
				ColumnSet.LargeImageColumn = Columns[WinExplorerViewItem.LargeImageColumn];
				ColumnSet.MediumImageColumn = Columns[WinExplorerViewItem.ImageColumn];
				ColumnSet.SmallImageColumn = Columns[WinExplorerViewItem.SmallImageColumn];
				ColumnSet.ExtraLargeImageIndexColumn = Columns[WinExplorerViewItem.ExtraLargeImageIndexColumn];
				ColumnSet.LargeImageIndexColumn = Columns[WinExplorerViewItem.LargeImageIndexColumn];
				ColumnSet.MediumImageIndexColumn = Columns[WinExplorerViewItem.ImageIndexColumn];
				ColumnSet.SmallImageIndexColumn = Columns[WinExplorerViewItem.SmallImageIndexColumn];
				if(Columns[WinExplorerViewItem.GroupColumn] != null) {
					Columns[WinExplorerViewItem.GroupColumn].OptionsColumn.AllowSort = DefaultBoolean.True;
					Columns[WinExplorerViewItem.GroupColumn].OptionsColumn.AllowGroup = DefaultBoolean.True;
				}
			}
		}
		protected override void OnColumnChangedCore(GridColumn column) {
			base.OnColumnChangedCore(column);
			if(column.GroupIndex >= 0)
				OnSortGroupPropertyChanged();
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
			WinExplorerViewInfo.Calculator.UpdateScrollBar();
		}
		protected internal override void OnPropertiesChanged() {
			if(!IsInitialized)
				return;
			WinExplorerViewInfo.ClearItemsCache = true;
			base.OnPropertiesChanged();
		}
		int postingEditorValue = 0;
		protected internal override bool PostEditor(bool causeValidation) {
			if(this.postingEditorValue != 0) return true;
			try {
				this.postingEditorValue++;
				if(ActiveEditor == null || !EditingValueModified || WinExplorerViewInfo.ActiveEditorInfo == null) return true;
				if((causeValidation && !ValidateEditor()) || WinExplorerViewInfo.ActiveEditorInfo == null) return false;
				object value = ExtractEditingValue(GetEditingColumn(), EditingValue);
				SetRowCellValueCore(WinExplorerViewInfo.ActiveEditorInfo.ItemInfo.Row.RowHandle, GetEditingColumn(), value, true);
			}
			finally {
				this.postingEditorValue--;
			}
			return true;
		}
		protected GridColumn GetEditingColumn() {
			return EditedColumn;
		}
		object extraLargeImages;
		[DXCategory(CategoryName.Appearance), DefaultValue(null), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public virtual object ExtraLargeImages {
			get { return extraLargeImages; }
			set {
				if(ExtraLargeImages == value)
					return;
				extraLargeImages = value;
				OnPositionChanged();
			}
		}
		object largeImages;
		[DXCategory(CategoryName.Appearance), DefaultValue(null), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public virtual object LargeImages {
			get { return largeImages; }
			set {
				if(LargeImages == value)
					return;
				largeImages = value;
				OnPositionChanged();
			}
		}
		object mediumImages;
		[DXCategory(CategoryName.Appearance), DefaultValue(null), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public virtual object MediumImages {
			get { return mediumImages; }
			set {
				if(MediumImages == value)
					return;
				mediumImages = value;
				OnPositionChanged();
			}
		}
		object smallImages;
		[DXCategory(CategoryName.Appearance), DefaultValue(null), TypeConverter(typeof(DevExpress.Utils.Design.ImageCollectionImagesConverter))]
		public virtual object SmallImages {
			get { return smallImages; }
			set {
				if(SmallImages == value)
					return;
				smallImages = value;
				OnPositionChanged();
			}
		}
		int position = 0;
		[DefaultValue(0), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int Position {
			get { return position; }
			set {
				value = ConstrainPosition(value);
				if(Position == value)
					return;
				int prev = Position;
				position = value;
				OnPositionChanged(prev);
			}
		}
		protected internal void InternalSetPosition(int position) {
			this.position = position;
		}
		internal bool SkipInvalidatePositionCache { get; set; }
		protected virtual void OnPositionChanged() {
			if(UseOptimizedScrolling && SkipScrollingUntilPaint)
				return;
			OnPositionChanged(int.MaxValue);
		}
		protected virtual void OnPositionChanged(int oldValue) {
			CloseEditor();
			try {
				SkipInvalidatePositionCache = true;
				CalculateLayout();
			}
			finally {
				OnWinExplorerViewScroll(oldValue);
				SkipInvalidatePositionCache = false;
			}
			if(WinExplorerViewInfo.ShowEditorWhenItemBecomesVisible && !WinExplorerViewInfo.ScrollHelper.Animating) {
				WinExplorerItemViewInfo itemInfo = WinExplorerViewInfo.GetItemByRow(WinExplorerViewInfo.FocusedRowHandle);
				if (itemInfo != null && itemInfo.IsFullyVisible) {
					WinExplorerViewColumnType columnType = ModifierKeysHelper.IsShiftPressed ? WinExplorerViewColumnType.Description : WinExplorerViewColumnType.Text;
					ShowEditor(columnType);
				}
			}
		}
		protected internal bool UseOptimizedScrolling {
			get {
				if(GridControl != null && GridControl.BackgroundImage != null)
					return false;
				return OptionsBehavior.UseOptimizedScrolling;
			}
		}
		protected virtual void OnWinExplorerViewScroll(int oldValue) {
			if(oldValue == int.MaxValue || ShouldInvalidateAllClient || !UseOptimizedScrolling) {
				if(GridControl != null) {
					GridControl.Invalidate();
					GridControl.Update();
				}
				ShouldInvalidateAllClient = false;
				return;
			}
			if(UseOptimizedScrolling) {
				ScrollWindow(oldValue);
			}
		}
		internal bool SkipScrollingUntilPaint { get; set; }
		protected virtual void ScrollWindow(int oldValue) {
			if(WinExplorerViewInfo.IsHorizontal) {
				Invalidate();
				return;
			}
			int delta = oldValue - Position;
			int position = delta < 0 ? -delta + WinExplorerViewInfo.ClientBounds.Y : WinExplorerViewInfo.ClientBounds.Y;
			SkipScrollingUntilPaint = true;
			WindowScroller.ScrollVertical(GridControl, WinExplorerViewInfo.ClientBounds, position, delta);
		}
		protected internal override void CloseEditor(bool causeValidation) {
			if(!IsEditing)
				return;
			base.CloseEditor(causeValidation);
			EditedColumn = null;
			WinExplorerViewInfo.ActiveEditorInfo = null;
		}
		public override void HideEditor() {
			base.HideEditor();
			EditedColumn = null;
			if(WinExplorerViewInfo != null) WinExplorerViewInfo.ActiveEditorInfo = null;
		}
		protected internal virtual int ConstrainPosition(int value) {
			if(OptionsBehavior.EnableSmoothScrolling && UseOptimizedScrolling && SkipScrollingUntilPaint) {
				return Position;
			}
			if(WinExplorerViewInfo == null)
				return value;
			return WinExplorerViewInfo.ConstrainPosition(value);
		}
		protected internal WinExplorerViewInfo WinExplorerViewInfo { get { return (WinExplorerViewInfo)ViewInfo; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int TopLeftItemIndex {
			get {
				WinExplorerViewInfo.Calculator.CheckCache();
				return WinExplorerViewInfo.Calculator.CalcTopLeftIconIndexByPosition(Position); 
			}
		}
		protected internal virtual void OnItemTypeChanged(WinExplorerViewStyle prevType) {
		}
		protected internal virtual void ResetPosition() {
			Position = 0;
		}
		protected virtual void DoTopLeftIconIndexChanged() {
		}
		protected virtual int CheckTopIconIndex(int value) {
			return value;
		}
		protected internal void InternalSetViewRectangleCore(Rectangle rect) {
			SetViewRect(rect);
		}
		protected override bool CanLeaveFocusOnTab(bool moveForward) {
			return false;
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("WinExplorerViewAppearance"),
#endif
 DXCategory(CategoryName.Appearance),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdAppearance)
		]
		public new WinExplorerViewAppearances Appearance { get { return base.Appearance as WinExplorerViewAppearances; } }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("WinExplorerViewAppearancePrint"),
#endif
 DXCategory(CategoryName.Appearance), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdAppearance)
		]
		public new WinExplorerViewAppearances AppearancePrint { get { return base.AppearancePrint as WinExplorerViewAppearances; } }
		protected override BaseViewAppearanceCollection CreateAppearances() {
			return new WinExplorerViewAppearances(this);
		}
		protected override BaseViewAppearanceCollection CreateAppearancesPrint() {
			return new WinExplorerViewAppearances(this);
		}
		protected override BaseViewInfo CreateNullViewInfo() {
			return new WinExplorerViewInfo(this);
		}
		protected override ViewPrintOptionsBase CreateOptionsPrint() {
			return new ViewPrintOptionsBase();
		}
		public override bool IsZoomedView {
			get { return false; }
		}
		protected override void LeaveFocusOnTab(bool moveForward) {
		}
		public override void NormalView() {
		}
		protected internal override void OnChildLayoutChanged(BaseView childView) {
		}
		protected internal override void OnVisibleChanged() {
		}
		protected internal override void ResetLookUp(bool sameDataSource) {
		}
		Rectangle viewRect;
		protected override void SetViewRect(Rectangle newValue) {
			this.viewRect = newValue;
			LayoutChanged();
		}
		public override Rectangle ViewRect {
			get { return viewRect; }
		}
		protected internal override void ZoomView(BaseView prevView) {
		}
		public override bool IsVisible {
			get { return ViewRect.X > -10000 && !ViewRect.IsEmpty && ViewRect.Right > 0 && ViewRect.Bottom > 0; }
		}
		protected override bool CalculateLayout() {
			if(ViewInfo == null)
				return false; 
			((WinExplorerViewInfo)ViewInfo).Calc(null, ViewRect);
			UpdateScrollBars();
			RefreshVisibleColumnsList();
			ViewInfo.IsReady = true;
			return true;
		}
		public override void ShowFilterPopup(DevExpress.XtraGrid.Columns.GridColumn column) { 
			throw new NotImplementedException();
		}
		protected override int VisualClientPageRowCount {
			get { return (WinExplorerViewInfo.Calculator.CalcScreenRowsCount() + 1) * WinExplorerViewInfo.AvailableColumnCount; }
		}
		protected override int VisualClientTopRowIndex {
			get { 
				return TopLeftItemIndex; 
			}
		}
		protected override void VisualClientUpdateScrollBar() {
			WinExplorerViewInfo.Calculator.UpdateScrollBar();
		}
		public override bool IsMultiSelect { 
			get { return OptionsSelection.MultiSelect; } 
		}
		public override void LayoutChanged() {
			if(!CalculateLayout()) return;
			base.LayoutChanged();
		}
		protected internal virtual int CalcVSmallChange() {
			return ScrollInfo.VScroll.SmallChange;
		}
		public virtual bool IsGroupRow(int rowHandle) {
			return DataController.IsGroupRowHandle(rowHandle);
		}
		public override bool IsDataRow(int rowHandle) {
			if(IsGroupRow(rowHandle)) return false;
			return base.IsDataRow(rowHandle);
		}
		public bool GetRowExpanded(int rowHandle) {
			return DataController.IsRowExpanded(rowHandle);
		}
		public virtual int GetParentRowHandle(int rowHandle) {
			if(IsGroupRow(rowHandle)) return DataController.GroupInfo.GetParentRow(rowHandle);
			GroupRowInfo group = DataController.GroupInfo.GetGroupRowInfoByControllerRowHandle(rowHandle);
			return group != null ? group.Handle : InvalidRowHandle;
		}
		protected internal virtual void ToggleItemCheck(WinExplorerItemViewInfo itemInfo) {
			if(itemInfo == null || ColumnSet.CheckBoxColumn == null || !ColumnSet.CheckBoxColumn.OptionsColumn.AllowEdit)
				return;
			itemInfo.IsChecked = !itemInfo.IsChecked;
			SetRowCellValueCore(itemInfo.Row.RowHandle, ColumnSet.CheckBoxColumn, itemInfo.IsChecked, true);
			CheckGroupCaptionCheckState(itemInfo);
			Invalidate();
		}
		protected void CheckGroupCaptionCheckState(WinExplorerItemViewInfo itemInfo) {
			if(ColumnSet.CheckBoxColumn == null || !ColumnSet.CheckBoxColumn.OptionsColumn.AllowEdit)
				return;
			WinExplorerViewInfo viewInfo = ViewInfo as WinExplorerViewInfo;
			CheckState state = CheckState.Indeterminate;
			GroupRowInfo group = DataController.GroupInfo.GetGroupRowInfoByControllerRowHandle(itemInfo.Row.RowHandle);
			if(group == null) return;
			if(viewInfo.CheckedCache.TryGetValue(group.Handle, out state)) {
				state = itemInfo.IsChecked ? CheckState.Checked : CheckState.Unchecked;
				int childCount = DataController.GroupInfo.GetChildCount(group.Handle);
				for(int i = 0; i < childCount; i++) {
					int rowHandle = DataController.GroupInfo.GetChildRow(group.Handle, i);
					WinExplorerItemViewInfo item = WinExplorerViewInfo.GetItemByRow(rowHandle);
					if(item == null) continue;
					if(WinExplorerViewInfo.GetItemByRow(rowHandle).IsChecked != itemInfo.IsChecked) {
						state = CheckState.Indeterminate;
						break;
					}
				}
				viewInfo.CheckedCache[group.Handle] = state;
			}
		}
		public virtual new WinExplorerViewHitInfo CalcHitInfo(Point pt) {
			if(WinExplorerViewInfo == null)
				return new WinExplorerViewHitInfo();
			return WinExplorerViewInfo.CalcHitInfo(pt);
		}
		public void SetRowExpanded(int rowHandle, bool expanded) {
			if(GetRowExpanded(rowHandle) == expanded) return;
			if(expanded)
				DataController.ExpandRow(rowHandle);
			else
				DataController.CollapseRow(rowHandle);
			WinExplorerViewInfo.UpdateCacheFromHandle(rowHandle);
			OnPositionChanged();
		}
		public void ToggleGroupExpanded(int groupRowHandle) { 
			SetRowExpanded(groupRowHandle, !GetRowExpanded(groupRowHandle));
		}
		protected internal void ToggleGroupChecked(int groupRowHandle) {
			if(ColumnSet.CheckBoxColumn == null || !ColumnSet.CheckBoxColumn.OptionsColumn.AllowEdit)
				return;
			WinExplorerViewInfo viewInfo = ViewInfo as WinExplorerViewInfo;
			CheckState state = CheckState.Indeterminate;
			if(viewInfo.CheckedCache.TryGetValue(groupRowHandle, out state)) {
				if(state != CheckState.Checked) {
					viewInfo.CheckedCache[groupRowHandle] = CheckState.Checked;
					SetAllGroupItemsChecked(groupRowHandle, true);
				} else {
					viewInfo.CheckedCache[groupRowHandle] = CheckState.Unchecked;
					SetAllGroupItemsChecked(groupRowHandle, false);
				}
			}
		}
		protected void SetAllGroupItemsChecked(int groupRowHandle, bool check) {
			int childCount = DataController.GroupInfo.GetChildCount(groupRowHandle);
			for(int i = 0; i < childCount; i++) {
				int rowHandle = DataController.GroupInfo.GetChildRow(groupRowHandle, i);
				WinExplorerItemViewInfo itemInfo = WinExplorerViewInfo.GetItemByRow(rowHandle);
				if(itemInfo != null)
					WinExplorerViewInfo.GetItemByRow(rowHandle).IsChecked = check;
				SetRowCellValue(rowHandle, ColumnSet.CheckBoxColumn, check);
			}
			WinExplorerGroupViewInfo groupInfo = WinExplorerViewInfo.GetItemByRow(groupRowHandle) as WinExplorerGroupViewInfo;
			groupInfo.Clicked = false;
		}
		public void ExpandAllGroups() {
			DataController.ExpandAll();
			WinExplorerViewInfo.Calculator.InvalidateCache();
			OnPositionChanged();
		}
		public void CollapseAllGroups() {
			DataController.CollapseAll();
			WinExplorerViewInfo.Calculator.InvalidateCache();
			OnPositionChanged();
		}
		protected internal void SelectGroupRowChildren(int groupRowHandle) {
			if(!OptionsSelection.MultiSelect) return;
			DataController.Selection.BeginSelection();
			try {
				DataController.Selection.Clear();
				DataController.Selection.SetSelected(groupRowHandle, true);
				int firstChild = DataController.GroupInfo.GetChildRow(groupRowHandle, 0);
				int childCount = DataController.GroupInfo.GetChildCount(groupRowHandle);
				for(int i = firstChild; i < firstChild + childCount; i++) {
					DataController.Selection.SetSelected(i, true);
				}
			}
			finally {
				DataController.Selection.EndSelection();
				Invalidate();
			}
		}
		protected internal bool SuppressInvalidate { get; set; }
		public override void InvalidateRect(Rectangle r) {
			if(SuppressInvalidate)
				return;
			base.InvalidateRect(r);
		}
		protected internal virtual void OnOptionChangedCore(object sender, BaseOptionChangedEventArgs e) {
			OnOptionChanged(sender, e);
		}
		protected internal virtual void OnColumnSetChanged() {
			OnPropertiesChanged();
		}
		protected internal override void OnColumnSortInfoCollectionChanged(CollectionChangeEventArgs e) {
			ResetThumbnailCache();
			base.OnColumnSortInfoCollectionChanged(e);
		}
		protected internal virtual void OnGroupColumnChanged(GridColumn prev) {
			SortInfo.BeginUpdate();
			ResetThumbnailCache();
			try {
				if(prev != null) {
					SortInfo.Remove(prev);
					SortInfo.GroupCount = 0;
				}
				if(ColumnSet.GroupColumn != null) {
					SortInfo.Insert(0, ColumnSet.GroupColumn, ColumnSortOrder.Ascending);
					SortInfo.GroupCount = 1;
				}
			}
			finally {
				SortInfo.EndUpdate();
				if(!IsDesignMode) {
					ResetGroupCheckedCache();
				}
			}
		}
		protected internal override int CalcRealViewHeight(Rectangle viewRect) {
			if(RequireSynchronization != SynchronizationMode.None) {
				CheckSynchronize();
				calculatedRealViewHeight = -1;
			}
			if(calculatedRealViewHeight != -1 && viewRect.Size == ViewRect.Size)
				return calculatedRealViewHeight;
			int result = viewRect.Height;
			WinExplorerViewInfo tempViewInfo = BaseInfo.CreateViewInfo(this) as WinExplorerViewInfo,
				oldViewInfo = WinExplorerViewInfo, copyFromInfo = WinExplorerViewInfo;
			this.fViewInfo = tempViewInfo;
			RefreshVisibleColumnsList();
			for(int i = 0; i < (oldViewInfo.IsReady ? 1 : 2); i++) {
				try {
					WinExplorerViewInfo.PrepareCalcRealViewHeight(viewRect, copyFromInfo);
					result = WinExplorerViewInfo.CalcRealViewHeight(viewRect);
				}
				catch {
				}
				copyFromInfo = WinExplorerViewInfo;
			}
			this.fViewInfo = oldViewInfo;
			calculatedRealViewHeight = result;
			return result;
		}
		internal bool FocusedRowChangedByHandler { get; set; }
		protected internal Dictionary<int, CheckState> CheckedGroupCache { get { return checkedGroupCache; } }
		void IContextItemCollectionOwner.OnCollectionChanged() {
			LayoutChanged();
		}
		void IContextItemCollectionOwner.OnItemChanged(ContextItem item, string propertyName, object oldValue, object newValue) {
			if(propertyName == "Visibility" || propertyName == "Value") {
				Invalidate();
				if(GridControl != null)
					GridControl.Update();
			}
			else
				LayoutChanged();
		}
		bool IContextItemCollectionOwner.IsRightToLeft { get { return IsRightToLeft; } }
		bool IContextItemCollectionOwner.IsDesignMode { get { return IsDesignMode; } }
		void IContextItemCollectionOptionsOwner.OnOptionsChanged(string propertyName, object oldValue, object newValue) {
			LayoutChanged();
		}
		ContextAnimationType IContextItemCollectionOptionsOwner.AnimationType { get { return ContextAnimationType.OpacityAnimation; } }
	}
	public delegate void WinExplorerViewDragEventHandler(object sender, WinExplorerViewDragEventArgs e);
	public class WinExplorerViewDragEventArgs : EventArgs {
		public MouseButtons Button { get; internal set; }
		public int[] Items { get; internal set; }
	}
	public delegate void WinExplorerViewItemClickEventHandler(object sender, WinExplorerViewItemClickEventArgs e);
	public delegate void WinExplorerViewContextButtonCustomizeEventHandler(object sender, WinExplorerViewContextButtonCustomizeEventArgs e);
	public class WinExplorerViewItemClickEventArgs : EventArgs {
		WinExplorerItemViewInfo itemInfo;
		MouseEventArgs mouseInfo;
		public WinExplorerViewItemClickEventArgs(WinExplorerItemViewInfo itemInfo, MouseEventArgs mouseInfo) {
			this.itemInfo = itemInfo;
			this.mouseInfo = mouseInfo;
		}
		public WinExplorerItemViewInfo ItemInfo { get { return itemInfo; } }
		public MouseEventArgs MouseInfo { get { return mouseInfo; } }
	}
	public class WinExplorerViewContextButtonCustomizeEventArgs : EventArgs {
		public WinExplorerViewContextButtonCustomizeEventArgs(ContextItem item, int rowHandle) {
			Item = item;
			RowHandle = rowHandle;
		}
		public ContextItem Item { get; private set; }
		public int RowHandle { get; private set; }
	}
	public delegate void WinExplorerViewContextButtonToolTipEventHandler(object sender, WinExplorerViewContextButtonToolTipEventArgs e);
	public class WinExplorerViewContextButtonToolTipEventArgs : EventArgs {
		int rowHandle;
		ContextButtonToolTipEventArgs contextToolTipArgs;
		public WinExplorerViewContextButtonToolTipEventArgs(int rowHandle, ContextButtonToolTipEventArgs contextToolTipArgs) {
			this.rowHandle = rowHandle;
			this.contextToolTipArgs = contextToolTipArgs;
		}
		public int RowHandle { get { return rowHandle; } }
		public ContextItem Item { get { return contextToolTipArgs.Item; } }
		public object Value { get { return contextToolTipArgs.Value; } }
		public string Text {
			get { return contextToolTipArgs.Text; }
			set { contextToolTipArgs.Text = value; }
		}
	}
	public delegate void WinExplorerViewItemDoubleClickEventHandler(object sender, WinExplorerViewItemDoubleClickEventArgs e);
	public class WinExplorerViewItemDoubleClickEventArgs : EventArgs {
		WinExplorerItemViewInfo itemInfo;
		MouseEventArgs mouseInfo;
		public WinExplorerViewItemDoubleClickEventArgs(WinExplorerItemViewInfo itemInfo, MouseEventArgs mouseInfo) {
			this.itemInfo = itemInfo;
			this.mouseInfo = mouseInfo;
		}
		public WinExplorerItemViewInfo ItemInfo { get { return itemInfo; } }
		public MouseEventArgs MouseInfo { get { return mouseInfo; } } 
	}
	public delegate void WinExplorerViewItemKeyDownEventHandler(object sender, WinExplorerViewItemKeyEventArgs e);
	public class WinExplorerViewItemKeyEventArgs : EventArgs {
		WinExplorerItemViewInfo itemInfo;
		KeyEventArgs keyInfo;
		public WinExplorerViewItemKeyEventArgs(WinExplorerItemViewInfo itemInfo, KeyEventArgs keyInfo) {
			this.itemInfo = itemInfo;
			this.keyInfo = keyInfo;
		}
		public WinExplorerItemViewInfo ItemInfo { get { return itemInfo; } }
		public KeyEventArgs KeyInfo { get { return keyInfo; } }
	}
	public enum WinExplorerViewColumnType { Text, Description }
	public delegate void WinExplorerViewCustomDrawItemEventHandler(object sender, WinExplorerViewCustomDrawItemEventArgs e);
	public delegate void WinExplorerViewCustomDrawGroupItemEventHandler(object sender, WinExplorerViewCustomDrawGroupItemEventArgs e);
	public class WinExplorerViewCustomDrawItemEventArgs : EventArgs {
		public WinExplorerViewCustomDrawItemEventArgs(ViewDrawArgs viewDrawArgs, WinExplorerItemViewInfo itemInfo) {
			ItemInfo = itemInfo;
			ViewDrawArgs = viewDrawArgs;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public WinExplorerItemViewInfo ItemInfo { get; set; }
		ViewDrawArgs ViewDrawArgs { get; set; }
		WinExplorerViewPainter Painter {
			get {
				return ItemInfo.ViewInfo.WinExplorerView.WinExplorerViewPainter;
			}
		}
		public bool IsAnimated { get; internal set; }
		public bool Handled { get; set; }
		public GraphicsCache Cache { get { return ViewDrawArgs.Cache;} }
		public Graphics Graphics { get { return Cache.Graphics; } }
		public Rectangle Bounds {
			get {
				return ItemInfo.Bounds;
			}
		}
		public Rectangle TextBounds {
			get {
				return ItemInfo.TextBounds;
			}
		}
		public Rectangle DescriptionBounds {
			get {
				return ItemInfo.DescriptionBounds;
			}
		}
		public Rectangle ImageBounds {
			get {
				return ItemInfo.ImageBounds;
			}
		}
		public Rectangle ImageContentBounds {
			get { return ItemInfo.ImageContentBounds; }
		}
		public Rectangle CheckBoxBounds {
			get {
				return ItemInfo.CheckBoxBounds;
			}
		}
		public Rectangle SelectionBounds {
			get { return ItemInfo.SelectionBounds; }
		}
		public Rectangle ItemSeparatorBounds {
			get {
				return ItemInfo.ItemSeparatorBounds;
			}
		}
		public string DisplayText {
			get {
				return ItemInfo.Text;
			}
			set {
				ItemInfo.Text = value;
			}
		}
		public string Description {
			get {
				return ItemInfo.Description;
			}
			set {
				ItemInfo.Description = value;
			}
		}
		public Image Image {
			get {
				return ItemInfo.Image;
			}
		}
		public int ImageIndex {
			get {
				return ItemInfo.ImageIndex;
			}
		}
		public bool AllowDescription {
			get {
				return ItemInfo.AllowDescription;
			}
		}
		public bool AllowDrawCheckBox {
			get {
				return ItemInfo.AllowDrawCheckBox;
			}
		}
		public int RowHandle {
			get {
				return ItemInfo.Row.RowHandle;
			}
		}
		public bool IsChecked {
			get {
				return ItemInfo.IsChecked;
			}
		}
		public bool IsHovered {
			get {
				return ItemInfo.IsHovered;
			}
		}
		public bool IsPressed {
			get {
				return ItemInfo.IsPressed;
			}
		}
		public bool IsSelected {
			get {
				return ItemInfo.IsSelected;
			}
		}
		public bool IsFocused {
			get {
				return ItemInfo.IsFocused;
			}
		}
		public AppearanceObject AppearanceText {
			get {
				return ItemInfo.ViewInfo.GetTextAppearance(ItemInfo);
			}
		}
		public AppearanceObject AppearanceDescription {
			get {
				return ItemInfo.ViewInfo.GetDescriptionAppearance(ItemInfo);
			}
		}
		public void DrawContextButtons() {
			Painter.DrawContextButtons(ViewDrawArgs, ItemInfo);
		}
		public void DrawItemBackground() {
			Painter.DrawItemBackground(ViewDrawArgs, ItemInfo);
		}
		public void DrawItemImage() {
			Painter.DrawItemImage(ViewDrawArgs, ItemInfo);
		}
		public void DrawItemCheckBox() {
			Painter.DrawItemCheck(ViewDrawArgs, ItemInfo);
		}
		public void DrawItemText() {
			Painter.DrawItemText(ViewDrawArgs, ItemInfo, this);
		}
		public void DrawItemSeparator() {
			Painter.DrawItemSeparator(ViewDrawArgs, ItemInfo);
		}
		public void Draw() {
			DrawItemBackground();
			DrawItemImage();
			if(ItemInfo.AllowDrawCheckBox)
				DrawItemCheckBox();
			DrawItemText();
			DrawItemSeparator();
			DrawContextButtons();
		}
	}
	public class WinExplorerViewCustomDrawGroupItemEventArgs : EventArgs {
		public WinExplorerViewCustomDrawGroupItemEventArgs(ViewDrawArgs viewDrawArgs, WinExplorerGroupViewInfo itemInfo) {
			ItemInfo = itemInfo;
			ViewDrawArgs = viewDrawArgs;
		}
		WinExplorerGroupViewInfo ItemInfo { get; set; }
		ViewDrawArgs ViewDrawArgs { get; set; }
		WinExplorerViewPainter Painter {
			get {
				return ItemInfo.ViewInfo.WinExplorerView.WinExplorerViewPainter;
			}
		}
		public bool Handled { get; set; }
		public GraphicsCache Cache { get { return ViewDrawArgs.Cache;} }
		public Graphics Graphics { get { return Cache.Graphics; } }
		public Rectangle Bounds {
			get {
				return ItemInfo.Bounds;
			}
		}
		public Rectangle TextBounds {
			get {
				return ItemInfo.TextBounds;
			}
		}
		public Rectangle CaptionButtonBounds {
			get {
				return ItemInfo.CaptionButtonBounds;
			}
		}
		public Rectangle LineBounds {
			get {
				return ItemInfo.LineBounds;
			}
		}
		public string DisplayText {
			get {
				return ItemInfo.Text;
			}
			set {
				ItemInfo.Text = value;
			}
		}
		public int RowHandle {
			get {
				return ItemInfo.Row.RowHandle;
			}
		}
		public bool IsHovered {
			get {
				return ItemInfo.IsHovered;
			}
		}
		public bool IsPressed {
			get {
				return ItemInfo.IsPressed;
			}
		}
		public bool IsSelected {
			get {
				return ItemInfo.IsSelected;
			}
		}
		public bool IsFocused {
			get {
				return ItemInfo.IsFocused;
			}
		}
		public AppearanceObject AppearanceText {
			get {
				return ItemInfo.ViewInfo.GetGroupCaptionAppearance(ItemInfo);
			}
		}
		public void DrawGroupBackground() {
			Painter.DrawGroupBackground(ViewDrawArgs, ItemInfo);
		}
		public void DrawGroupCaptionLine() {
			Painter.DrawGroupCaptionLine(ViewDrawArgs, ItemInfo);
		}
		public void DrawGroupCaptionButton() {
			Painter.DrawGroupCaptionButton(ViewDrawArgs, ItemInfo);
		}
		public void DrawGroupCaption() {
			Painter.DrawGroupCaption(ViewDrawArgs, ItemInfo);
		}
		public void Draw() {
			DrawGroupBackground();
			DrawGroupCaptionLine();
			if (ItemInfo.ViewInfo.ShowExpandCollapseButtons)
				DrawGroupCaptionButton();
			DrawGroupCaption();
		}
	}
	public class WinExplorerContextItemCollectionOptions : ContextItemCollectionOptions {
		public WinExplorerContextItemCollectionOptions(IContextItemCollectionOptionsOwner owner) : base(owner) { }
		public WinExplorerContextItemCollectionOptions() : base() { }
		bool showOutsideDisplayBounds = false;
		[DefaultValue(false)]
		public bool ShowOutsideDisplayBounds {
			get { return showOutsideDisplayBounds; }
			set {
				if(ShowOutsideDisplayBounds == value)
					return;
				bool prev = ShowOutsideDisplayBounds; 
				showOutsideDisplayBounds = value;
				OnOptionsChanged("ShowOutsideDisplayBounds", prev, ShowOutsideDisplayBounds);
			}
		}
		protected override ContextItemCollectionOptions CreateOptions() { return new WinExplorerContextItemCollectionOptions(); }
		public override void Assign(ContextItemCollectionOptions options) {
			base.Assign(options);
			WinExplorerContextItemCollectionOptions opt = options as WinExplorerContextItemCollectionOptions;
			if(opt != null)
				this.showOutsideDisplayBounds = opt.ShowOutsideDisplayBounds;
		}
	}
}
