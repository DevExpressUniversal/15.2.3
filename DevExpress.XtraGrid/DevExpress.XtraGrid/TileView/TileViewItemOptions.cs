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

using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile.ViewInfo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraGrid.Views.Tile {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class TileViewItemOptions : ViewBaseOptions, ITileControlProperties {
		TileView view;
		public TileViewItemOptions(TileView tileview) {
			this.view = tileview;
			this.itemSize = defaultItemSize;
		}
		protected internal TileView View { get { return view; } }
		protected virtual void TileViewItemOptionsChanged() {
			if(View != null)
				View.OnPropertiesChanged();
		}
		bool ITileControlProperties.AllowDrag {
			get { return false; }
			set { }
		}
		bool ITileControlProperties.AllowGroupHighlighting {
			get { return false; }
			set { }
		}
		bool ITileControlProperties.AllowHtmlDraw {
			get { return true; }
			set { }
		}
		bool ITileControlProperties.AllowItemHover {
			get { return false; }
			set { }
		}
		bool ITileControlProperties.AllowSelectedItem {
			get { return false; }
			set { }
		}
		GroupHighlightingProperties ITileControlProperties.AppearanceGroupHighlighting {
			get { return new GroupHighlightingProperties(); }
			set { }
		}
		int ITileControlProperties.ColumnCount {
			get { return GetColumnCount(); }
			set { }
		}
		int ITileControlProperties.ItemSize {
			get { return this.ItemSize.Height; }
			set { }
		}
		int GetColumnCount() {
			if(View != null && View.ViewInfo is TileViewInfo) {
				var itemSize = ((View.ViewInfo as ITileControl).ViewInfo as TileViewInfoCore).GetItemType();
				if(itemSize == TileItemSize.Wide || itemSize == TileItemSize.Large)
					return ColumnCount * 2;
				return ColumnCount;
			}
			return 0;
		}
		void ITileControlProperties.Assign(ITileControlProperties source) { }
		ImageLayout ITileControlProperties.BackgroundImageLayout {
			get { return View.GridControl.BackgroundImageLayout; }
			set { }
		}
		bool highlightFocusedTileOnGridLoad;
		[DefaultValue(false), Category(CategoryName.Behavior)]
		public bool HighlightFocusedTileOnGridLoad {
			get { return highlightFocusedTileOnGridLoad; }
			set {
				if(highlightFocusedTileOnGridLoad == value)
					return;
				highlightFocusedTileOnGridLoad = value;
				TileViewItemOptionsChanged();
			}
		}
		TileControlScrollMode scrollMode = TileControlScrollMode.Default;
		[DefaultValue(TileControlScrollMode.Default), Category(CategoryName.Behavior)]
		public TileControlScrollMode ScrollMode {
			get { return scrollMode; }
			set {
				if(ScrollMode == value)
					return;
				scrollMode = value;
				TileViewItemOptionsChanged();
			}
		}
		bool showGroupText = true;
		[DefaultValue(true), Category(CategoryName.Appearance)]
		public bool ShowGroupText {
			get { return showGroupText; }
			set {
				if(ShowGroupText == value)
					return;
				showGroupText = value;
				TileViewItemOptionsChanged();
			}
		}
		int columnCount = 0;
		[DefaultValue(0)]
		public int ColumnCount {
			get { return columnCount; }
			set {
				if(columnCount == value) return;
				columnCount = value;
				TileViewItemOptionsChanged();
			}
		}
		HorzAlignment horizontalContentAlignment = HorzAlignment.Default;
		[DefaultValue(HorzAlignment.Default)]
		public HorzAlignment HorizontalContentAlignment {
			get { return horizontalContentAlignment; }
			set {
				if(horizontalContentAlignment == value) return;
				horizontalContentAlignment = value;
				TileViewItemOptionsChanged();
			}
		}
		const int indentBetweenGroupsDefault = 56;
		int indentBetweenGroups = indentBetweenGroupsDefault;
		[DefaultValue(indentBetweenGroupsDefault)]
		public int IndentBetweenGroups {
			get { return indentBetweenGroups; }
			set {
				if(indentBetweenGroups == value) return;
				indentBetweenGroups = value;
				TileViewItemOptionsChanged();
			}
		}
		const int indentBetweenItemsDefault = 8;
		int indentBetweenItems = indentBetweenItemsDefault;
		[DefaultValue(indentBetweenItemsDefault)]
		public int IndentBetweenItems {
			get { return indentBetweenItems; }
			set {
				if(indentBetweenItems == value) return;
				indentBetweenItems = value;
				TileViewItemOptionsChanged();
			}
		}
		TileItemCheckMode ITileControlProperties.ItemCheckMode {
			get { return GetItemCheckMode(); }
			set { }
		}
		TileItemCheckMode GetItemCheckMode() {
			if(View != null)
			if(View.HasCheckedColumn) 
				return TileItemCheckMode.Multiple;
			return TileItemCheckMode.None;
		}
		TileItemContentAnimationType ITileControlProperties.ItemContentAnimation {
			get { return TileItemContentAnimationType.Default; }
			set { }
		}
		TileItemContentAlignment ITileControlProperties.ItemImageAlignment {
			get { return TileItemContentAlignment.Default; }
			set { }
		}
		TileItemImageScaleMode ITileControlProperties.ItemImageScaleMode {
			get { return TileItemImageScaleMode.Default; }
			set { }
		}
		Padding itemPadding = DefaultItemPadding;
		public static Padding DefaultItemPadding { get { return new Padding(12, 8, 12, 8); } }
		void ResetItemPadding() { ItemPadding = DefaultItemPadding; }
		bool ShouldSerializeItemPadding() { return ItemPadding != DefaultItemPadding; }
		public Padding ItemPadding {
			get { return itemPadding; }
			set {
				if(itemPadding == value) return;
				itemPadding = value;
				TileViewItemOptionsChanged();
			}
		}
		Size defaultItemSize = new Size(248, 120);
		Size itemSize;
		public Size ItemSize {
			get { return itemSize; }
			set {
				if(itemSize == value) return;
				itemSize = value;
				TileViewItemOptionsChanged();
			}
		}
		void ResetItemSize() { ItemSize = defaultItemSize; }
		bool ShouldSerializeItemSize() { return ItemSize != defaultItemSize; }
		TileItemContentShowMode ITileControlProperties.ItemTextShowMode {
			get { return TileItemContentShowMode.Default; }
			set { }
		}
		int ITileControlProperties.LargeItemWidth {
			get { return this.ItemSize.Width; }
			set { }
		}
		Orientation orientation = Orientation.Horizontal;
		[DefaultValue(Orientation.Horizontal)]
		public Orientation Orientation {
			get { return orientation; }
			set {
				if(orientation == value) return;
				orientation = value;
				View.OnOrientationChanged();
			}
		}
		Padding padding = DefaultPadding;
		public static Padding DefaultPadding { get { return new Padding(10); } }
		void ResetPadding() { this.Padding = DefaultPadding; }
		bool ShouldSerializePadding() { return Padding != DefaultPadding; }
		public Padding Padding {
			get { return padding; }
			set {
				if(padding == value) return;
				padding = value;
				TileViewItemOptionsChanged();
			}
		}
		const int rowCountDefault = 1;
		int rowCount = rowCountDefault;
		[DefaultValue(rowCountDefault)]
		public int RowCount {
			get { return rowCount; }
			set {
				if(rowCount == value) return;
				rowCount = value;
				TileViewItemOptionsChanged();
			}
		}
		bool ITileControlProperties.ShowText {
			get { return View.OptionsView.ShowViewCaption; }
			set { }
		}
		VertAlignment verticalContentAlignment = VertAlignment.Default;
		[DefaultValue(VertAlignment.Default)]
		public VertAlignment VerticalContentAlignment {
			get { return verticalContentAlignment; }
			set {
				if(verticalContentAlignment == value) return;
				verticalContentAlignment = value;
				TileViewItemOptionsChanged();
			}
		}
		TileItemContentAlignment itemBackgroundImageAlignment = TileItemContentAlignment.Default;
		[DefaultValue(TileItemContentAlignment.Default), Category(CategoryName.Appearance)]
		public TileItemContentAlignment ItemBackgroundImageAlignment {
			get { return itemBackgroundImageAlignment; }
			set {
				if(ItemBackgroundImageAlignment == value)
					return;
				itemBackgroundImageAlignment = value;
				TileViewItemOptionsChanged();
			}
		}
		TileItemImageScaleMode itemBackgroundImageScaleMode = TileItemImageScaleMode.Default;
		[DefaultValue(TileItemImageScaleMode.Default), Category(CategoryName.Appearance)]
		public TileItemImageScaleMode ItemBackgroundImageScaleMode {
			get { return itemBackgroundImageScaleMode; }
			set {
				if(ItemBackgroundImageScaleMode == value)
					return;
				itemBackgroundImageScaleMode = value;
				TileViewItemOptionsChanged();
			}
		}
		TileItemBorderVisibility itemBorderVisibility = TileItemBorderVisibility.Auto;
		[DefaultValue(TileItemBorderVisibility.Auto), Category(CategoryName.Appearance)]
		public TileItemBorderVisibility ItemBorderVisibility {
			get { return itemBorderVisibility; }
			set {
				if(itemBorderVisibility == value)
					return;
				itemBorderVisibility = value;
				TileViewItemOptionsChanged();
			}
		}
		bool allowPressAnimation = true;
		[DefaultValue(true)]
		public bool AllowPressAnimation {
			get { return allowPressAnimation; }
			set {
				if(allowPressAnimation == value)
					return;
				allowPressAnimation = value;
			}
		}
		public override void Assign(DevExpress.Utils.Controls.BaseOptions options) {
			var src = options as TileViewItemOptions;
			if(src == null) return;
			BeginUpdate();
			try {
				this.ColumnCount = src.ColumnCount;
				this.HorizontalContentAlignment = src.HorizontalContentAlignment;
				this.IndentBetweenGroups = src.IndentBetweenGroups;
				this.IndentBetweenItems = src.IndentBetweenItems;
				this.ItemPadding = src.ItemPadding;
				this.ItemSize = src.ItemSize;
				this.Orientation = src.Orientation;
				this.Padding = src.Padding;
				this.RowCount = src.RowCount;
				this.ScrollMode = src.ScrollMode;
				this.ItemBackgroundImageAlignment = src.ItemBackgroundImageAlignment;
				this.ItemBackgroundImageScaleMode = src.ItemBackgroundImageScaleMode;
				this.ItemBorderVisibility = src.ItemBorderVisibility;
				this.HighlightFocusedTileOnGridLoad = src.HighlightFocusedTileOnGridLoad;
			}
			finally{
				EndUpdate();
			}
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class TileViewOptionsImageLoad : BaseOptions {
		bool asyncLoad, randowShow, loadThumbnailImagesFromDataSource, cacheThumbnails;
		Size desiredThumbnailSize;
		ImageContentAnimationType animationType;
		TileView view;
		public TileViewOptionsImageLoad(TileView view) {
			this.asyncLoad = false;
			this.randowShow = this.cacheThumbnails = this.loadThumbnailImagesFromDataSource = true;
			this.animationType = ImageContentAnimationType.None;
			this.desiredThumbnailSize = Size.Empty;
			this.view = view;
		}
		protected internal bool ShouldSerializeCore(IComponent owner) { return ShouldSerialize(owner); }
		[DefaultValue(false)]
		public bool AsyncLoad {
			get { return asyncLoad; }
			set {
				if(asyncLoad == value) return;
				asyncLoad = value;
				OnAsyncLoadChanged();
			}
		}
		[DefaultValue(true)]
		public bool CacheThumbnails {
			get { return cacheThumbnails; }
			set { cacheThumbnails = value; }
		}
		[DefaultValue(true)]
		public bool LoadThumbnailImagesFromDataSource {
			get { return loadThumbnailImagesFromDataSource; }
			set { loadThumbnailImagesFromDataSource = value; }
		}
		[DefaultValue(ImageContentAnimationType.None)]
		public ImageContentAnimationType AnimationType {
			get { return animationType; }
			set { animationType = value; }
		}
		[DefaultValue(true)]
		public bool RandomShow {
			get { return randowShow; }
			set { randowShow = value; }
		}
		void ResetDesiredThumbnailSize() { DesiredThumbnailSize = Size.Empty; }
		bool ShouldSerializeDesiredThumbnailSize() { return DesiredThumbnailSize != Size.Empty; }
		public Size DesiredThumbnailSize {
			get { return desiredThumbnailSize; }
			set { desiredThumbnailSize = value; }
		}
		protected void OnAsyncLoadChanged() {
			if(view == null) return;
			view.ResetImageLoader();
		}
		public override void Assign(BaseOptions options) {
			TileViewOptionsImageLoad optionsImageLoad = options as TileViewOptionsImageLoad;
			if(optionsImageLoad == null)
				return;
			this.animationType = optionsImageLoad.animationType;
			this.asyncLoad = optionsImageLoad.asyncLoad;
			this.randowShow = optionsImageLoad.randowShow;
			this.cacheThumbnails = optionsImageLoad.cacheThumbnails;
			this.loadThumbnailImagesFromDataSource = optionsImageLoad.loadThumbnailImagesFromDataSource;
			this.desiredThumbnailSize = optionsImageLoad.desiredThumbnailSize;
		}
	}
}
