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
using System.Linq;
using System.Text;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.WinExplorer;
using System.ComponentModel;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.Utils.Serializing;
using DevExpress.Utils;
using System.Drawing;
namespace DevExpress.XtraGrid.WinExplorer {
	public class WinExplorerViewOptionsSelection : ColumnViewOptionsSelection { 
		public WinExplorerViewOptionsSelection() : base() {
			ItemSelectionMode = IconItemSelectionMode.None;
		}
		[DefaultValue(IconItemSelectionMode.None), XtraSerializableProperty]
		public IconItemSelectionMode ItemSelectionMode { get; set; }
		[DefaultValue(false), XtraSerializableProperty]
		public bool AllowMarqueeSelection { get; set; }
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			WinExplorerViewOptionsSelection winExplorerViewOptions = options as WinExplorerViewOptionsSelection;
			if(winExplorerViewOptions != null) {
				ItemSelectionMode = winExplorerViewOptions.ItemSelectionMode;
				AllowMarqueeSelection = winExplorerViewOptions.AllowMarqueeSelection;
			}
		}
	}
	public class WinExplorerViewOptionsNavigation : GridOptionsNavigation {
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AutoFocusNewRow {
			get { return base.AutoFocusNewRow; }
			set { base.AutoFocusNewRow = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AutoMoveRowFocus {
			get { return base.AutoMoveRowFocus; }
			set { base.AutoMoveRowFocus = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool EnterMoveNextColumn {
			get { return base.EnterMoveNextColumn; }
			set { base.EnterMoveNextColumn = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool UseOfficePageNavigation {
			get { return base.UseOfficePageNavigation; }
			set { base.UseOfficePageNavigation = value; }
		}
	}
	public class WinExplorerViewOptionsBehavior : ColumnViewOptionsBehavior {
		public WinExplorerViewOptionsBehavior() : this(null) { }
		public WinExplorerViewOptionsBehavior(WinExplorerView view)
			: base(view) {
			this.EnableSmoothScrolling = true;
			this.UseOptimizedScrolling = true;
			this.AutoScrollItemOnMouseClick = true;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Utils.DefaultBoolean AllowAddRows {
			get { return base.AllowAddRows; }
			set { base.AllowAddRows = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Utils.DefaultBoolean AllowDeleteRows {
			get { return base.AllowDeleteRows; }
			set { base.AllowDeleteRows = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AutoSelectAllInEditor {
			get { return base.AutoSelectAllInEditor; }
			set { base.AutoSelectAllInEditor = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Utils.EditorShowMode EditorShowMode {
			get { return base.EditorShowMode; }
			set { base.EditorShowMode = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override DevExpress.Data.CacheRowValuesMode CacheValuesOnRowUpdating {
			get { return base.CacheValuesOnRowUpdating; }
			set { base.CacheValuesOnRowUpdating = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool FocusLeaveOnTab {
			get { return base.FocusLeaveOnTab; }
			set { base.FocusLeaveOnTab = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ImmediateUpdateRowPosition {
			get { return base.ImmediateUpdateRowPosition; }
			set { base.ImmediateUpdateRowPosition = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool KeepFocusedRowOnUpdate {
			get { return base.KeepFocusedRowOnUpdate; }
			set { base.KeepFocusedRowOnUpdate = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool ReadOnly {
			get { return base.ReadOnly; }
			set { base.ReadOnly = value; }
		}
		[DefaultValue(true), XtraSerializableProperty]
		public bool EnableSmoothScrolling { get; set; }
		[DefaultValue(true), XtraSerializableProperty]
		public bool UseOptimizedScrolling { get; set; }
		[DefaultValue(true), XtraSerializableProperty]
		public bool AutoScrollItemOnMouseClick { get; set; }
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			WinExplorerViewOptionsBehavior winExplorerViewOptions = options as WinExplorerViewOptionsBehavior;
			if(winExplorerViewOptions != null) {
				EnableSmoothScrolling = winExplorerViewOptions.EnableSmoothScrolling;
				UseOptimizedScrolling = winExplorerViewOptions.UseOptimizedScrolling;
				AutoScrollItemOnMouseClick = winExplorerViewOptions.AutoScrollItemOnMouseClick;
			}
		}
		protected WinExplorerView WinExplorerView { get { return View as WinExplorerView; } }
		[Obsolete("This property is now obsolete and no longer in use"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool EnableDetailToolTip { get; set; }
	}
	public enum ItemHoverBordersShowMode { Default, Always, Never, ContextButtons }
	public class WinExplorerViewOptionsView : ColumnViewOptionsView {
		WinExplorerView view;
		ImageLayoutMode imageLayoutMode = ImageLayoutMode.Default;
		public WinExplorerViewOptionsView() : this(null) { }
		public WinExplorerViewOptionsView(WinExplorerView view) {
			this.view = view;
		}
		HorzAlignment contentHorizontalAlignment = HorzAlignment.Default;
		[DefaultValue(HorzAlignment.Default)]
		public HorzAlignment ContentHorizontalAlignment {
			get { return contentHorizontalAlignment; }
			set {
				if(ContentHorizontalAlignment == value)
					return;
				HorzAlignment prev = ContentHorizontalAlignment;
				contentHorizontalAlignment = value;
				RaiseChanged("ContentHorizontalAlignment", prev, ContentHorizontalAlignment);
			}
		}
		[DefaultValue(ImageLayoutMode.Default), XtraSerializableProperty]
		public ImageLayoutMode ImageLayoutMode {
			get { return imageLayoutMode; }
			set {
				if(ImageLayoutMode == value)
					return;
				ImageLayoutMode prev = ImageLayoutMode;
				imageLayoutMode = value;
				RaiseChanged("ImageLayoutMode", prev, ImageLayoutMode);
			}
		}
		ItemHoverBordersShowMode itemHoverBordersShowMode = ItemHoverBordersShowMode.Default;
		[DefaultValue(ItemHoverBordersShowMode.Default)]
		public ItemHoverBordersShowMode ItemHoverBordersShowMode {
			get { return itemHoverBordersShowMode; }
			set {
				if(ItemHoverBordersShowMode == value)
					return;
				ItemHoverBordersShowMode prev = ItemHoverBordersShowMode;
				itemHoverBordersShowMode = value;
				RaiseChanged("ItemHoverBordersShowMode", prev, ItemHoverBordersShowMode);
			}
		}
		[DefaultValue(false)]
		public bool DrawCheckedItemsAsSelected {
			get;
			set;
		}
		bool allowHtmlText = false;
		[DefaultValue(false), XtraSerializableProperty]
		public bool AllowHtmlText {
			get { return allowHtmlText; }
			set {
				if(AllowHtmlText == value)
					return;
				bool prevValue = AllowHtmlText;
				allowHtmlText = value;
				RaiseChanged("AllowHtmlText", prevValue, AllowHtmlText);
			}
		}
		WinExplorerViewStyle style = WinExplorerViewStyle.Default;
		[DefaultValue(WinExplorerViewStyle.Default), XtraSerializableProperty]
		public WinExplorerViewStyle Style {
			get { return style; }
			set {
				if(Style == value)
					return;
				WinExplorerViewStyle prevStyle = Style;
				style = value;
				OnItemTypeChanged(prevStyle);
			}
		}
		protected virtual void OnItemTypeChanged(WinExplorerViewStyle prevStyle) {
			if(Style == WinExplorerViewStyle.List) {
				WinExplorerView.DataController.ExpandAll();
			}
			if(WinExplorerView.WinExplorerViewInfo != null)
				WinExplorerView.WinExplorerViewInfo.SetPaintAppearanceDirty();
			WinExplorerView.ResetPosition();
			RaiseChanged("ItemType", prevStyle, Style);
		}
		ScrollVisibility scrollVisibility = ScrollVisibility.Auto;
		[DefaultValue(ScrollVisibility.Auto), XtraSerializableProperty]
		public ScrollVisibility ScrollVisibility {
			get { return scrollVisibility; }
			set {
				if(ScrollVisibility == value)
					return;
				ScrollVisibility prevValue = ScrollVisibility;
				scrollVisibility = value;
				RaiseChanged("AllowHtmlText", prevValue, ScrollVisibility);
			}
		}
		bool showCheckBoxes = false;
		[DefaultValue(false), XtraSerializableProperty]
		public bool ShowCheckBoxes {
			get { return showCheckBoxes; }
			set {
				if(ShowCheckBoxes == value)
					return;
				bool prevValue = ShowCheckBoxes;
				showCheckBoxes = value;
				RaiseChanged("ShowCheckBoxes", prevValue, ShowCheckBoxes);
			}
		}
		bool showExpandCollapseButtons = false;
		[DefaultValue(false), XtraSerializableProperty]
		public bool ShowExpandCollapseButtons {
			get { return showExpandCollapseButtons; }
			set {
				if(ShowExpandCollapseButtons == value)
					return;
				bool prevValue = ShowExpandCollapseButtons;
				showExpandCollapseButtons = value;
				RaiseChanged("ShowExpandCollapseButtons", prevValue, ShowExpandCollapseButtons);
			}
		}
		bool showCheckBoxInGroupCaption = false;
		[DefaultValue(false), XtraSerializableProperty]
		public bool ShowCheckBoxInGroupCaption {
			get { return showCheckBoxInGroupCaption; }
			set {
				if(ShowCheckBoxInGroupCaption == value)
					return;
				bool prevValue = ShowCheckBoxInGroupCaption;
				showCheckBoxInGroupCaption = value;
				RaiseChanged("ShowCheckBoxInGroupCaption", prevValue, ShowCheckBoxInGroupCaption);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ShowButtonModeEnum ShowButtonMode {
			get { return base.ShowButtonMode; }
			set { base.ShowButtonMode = value; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ShowFilterPanelMode ShowFilterPanelMode {
			get { return base.ShowFilterPanelMode; }
			set { base.ShowFilterPanelMode = value; }
		}
		protected virtual void RaiseChanged(string propertyName, object prevValue, object nextValue) {
			OnChanged(new BaseOptionChangedEventArgs(propertyName, prevValue, nextValue));
		}
		public override void Assign(BaseOptions options) {
			base.Assign(options);
			WinExplorerViewOptionsView explorerViewOptions = options as WinExplorerViewOptionsView;
			if(explorerViewOptions != null) {
				ImageLayoutMode = explorerViewOptions.ImageLayoutMode;
				AllowHtmlText = explorerViewOptions.AllowHtmlText;
				Style = explorerViewOptions.Style;
				ScrollVisibility = explorerViewOptions.ScrollVisibility;
				ShowCheckBoxes = explorerViewOptions.ShowCheckBoxes;
				ShowExpandCollapseButtons = explorerViewOptions.ShowExpandCollapseButtons;
			}
		}
		protected WinExplorerView WinExplorerView { get { return view; } }
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class WinExplorerViewOptionsImageLoad : BaseOptions {
		bool asyncLoad, randowShow, loadThumbnailImagesFromDataSource, cacheThumbnails, clearCacheOnReset;
		Size desiredThumbnailSize; 
		ImageContentAnimationType animationType;
		WinExplorerView view;
		public WinExplorerViewOptionsImageLoad(WinExplorerView view) {
			this.asyncLoad = false;
			this.randowShow = this.cacheThumbnails = this.loadThumbnailImagesFromDataSource = true;
			this.animationType = ImageContentAnimationType.None;
			this.desiredThumbnailSize = Size.Empty;
			this.view = view;
			this.clearCacheOnReset = true;
		}
		protected internal bool ShouldSerializeCore(IComponent owner) { return ShouldSerialize(owner); }
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("WinExplorerViewOptionsImageLoadAsyncLoad"),
#endif
 DefaultValue(false)]
		public bool AsyncLoad {
			get { return asyncLoad; }
			set {
				if(AsyncLoad == value) return;
				asyncLoad = value;
				OnAsyncLoadChanged();
			}
		}
		bool smartImageLoad = true;
		[ DefaultValue(true)]
		public bool AllowReplaceableImages {
			get { return smartImageLoad; }
			set {
				if(AllowReplaceableImages == value)
					return;
				smartImageLoad = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowReplaceableImages", !AllowReplaceableImages, AllowReplaceableImages));
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ClearCacheOnDataSourceUpdate {
			get { return clearCacheOnReset; }
			set { clearCacheOnReset = value; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("WinExplorerViewOptionsImageLoadCacheThumbnails"),
#endif
 DefaultValue(true)]
		public bool CacheThumbnails {
			get { return cacheThumbnails; }
			set { cacheThumbnails = value; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("WinExplorerViewOptionsImageLoadLoadThumbnailImagesFromDataSource"),
#endif
 DefaultValue(true)]
		public bool LoadThumbnailImagesFromDataSource {
			get { return loadThumbnailImagesFromDataSource; }
			set { loadThumbnailImagesFromDataSource = value; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("WinExplorerViewOptionsImageLoadAnimationType"),
#endif
 DefaultValue(ImageContentAnimationType.None)]
		public ImageContentAnimationType AnimationType {
			get { return animationType; }
			set { animationType = value; }
		}
		[
#if !SL
	DevExpressXtraGridLocalizedDescription("WinExplorerViewOptionsImageLoadRandomShow"),
#endif
 DefaultValue(true)]
		public bool RandomShow {
			get { return randowShow; }
			set { randowShow = value; }
		}
		void ResetDesiredThumbnailSize() { DesiredThumbnailSize = Size.Empty; }
		bool ShouldSerializeDesiredThumbnailSize() { return DesiredThumbnailSize != Size.Empty; }
#if !SL
	[DevExpressXtraGridLocalizedDescription("WinExplorerViewOptionsImageLoadDesiredThumbnailSize")]
#endif
		public Size DesiredThumbnailSize {
			get { return desiredThumbnailSize; }
			set { desiredThumbnailSize = value; }
		}
		protected void OnAsyncLoadChanged() {
			if(view == null || view.WinExplorerViewInfo == null) return;
			view.WinExplorerViewInfo.ResetImageLoader();
		}
		public override void Assign(BaseOptions options) {
			WinExplorerViewOptionsImageLoad optionsImageLoad = options as WinExplorerViewOptionsImageLoad;
			if(optionsImageLoad == null)
				return;
			this.animationType = optionsImageLoad.animationType;
			this.asyncLoad = optionsImageLoad.asyncLoad;
			this.randowShow = optionsImageLoad.randowShow;
			this.cacheThumbnails = optionsImageLoad.cacheThumbnails;
			this.loadThumbnailImagesFromDataSource = optionsImageLoad.loadThumbnailImagesFromDataSource;
			this.desiredThumbnailSize = optionsImageLoad.desiredThumbnailSize;
			this.clearCacheOnReset = optionsImageLoad.clearCacheOnReset;
		}
	}
}
