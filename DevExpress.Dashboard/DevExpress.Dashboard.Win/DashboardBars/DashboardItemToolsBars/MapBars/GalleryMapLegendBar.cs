#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.Utils.Commands;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
namespace DevExpress.DashboardWin.Bars {
	public class MapLegendPositionPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupMapLegendPositionCaption); } }
	}
	public class WeightedLegendPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupWeightedLegendPositionCaption); } }
	}
	public abstract class MapLegendPositionGalleryItemBase : GalleryItem {
		readonly MapLegendPosition position;
		internal MapLegendPosition Position { get { return position; } }
		protected MapLegendPositionGalleryItemBase(MapLegendPosition position, DashboardWinStringId hintId) {
			this.position = position;
			Hint = DashboardWinLocalizer.GetString(hintId);
		}
	}
	public class MapLegendPositionGalleryItem : MapLegendPositionGalleryItemBase {
		readonly MapLegendOrientation orientation;
		internal MapLegendOrientation Orientation { get { return orientation; } }
		public MapLegendPositionGalleryItem(MapLegendPosition position, MapLegendOrientation orientation, DashboardWinStringId hintId)
			: base(position, hintId) {
			this.orientation = orientation;
			Image = ImageHelper.GetImage(string.Format(@"MapLegendPositions.{0}_{1}", position, orientation));
		}
	}
	public class WeightedLegendPositionGalleryItem : MapLegendPositionGalleryItemBase {
		public WeightedLegendPositionGalleryItem(MapLegendPosition position, DashboardWinStringId hintId)
			: base(position, hintId) {
			Image = ImageHelper.GetImage(string.Format(@"MapLegendPositions.{0}_{1}", position, MapLegendOrientation.Vertical));
		}
	}
	public class GalleryMapLegendPositionItem : DashboardCommandGalleryBarItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.MapLegendPosition; } }
		protected override ICommandUIState CreateGalleryItemUIState() {
			return new MapLegendPositionCommandUIState(this);
		}
		protected override void PrepareGallery() {
			InRibbonGallery gallery = Gallery;
			gallery.ColumnCount = 3;
			gallery.RowCount = 4;
			GalleryItemGroup verticalGroup = new GalleryItemGroup();
			verticalGroup.Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.MapGalleryVerticalGroupCaption);
			verticalGroup.Items.Add(new MapLegendPositionGalleryItem(MapLegendPosition.TopLeft, MapLegendOrientation.Vertical, DashboardWinStringId.MapTopLeftVerticalLegendPosition));
			verticalGroup.Items.Add(new MapLegendPositionGalleryItem(MapLegendPosition.TopCenter, MapLegendOrientation.Vertical, DashboardWinStringId.MapTopCenterVerticalLegendPosition));
			verticalGroup.Items.Add(new MapLegendPositionGalleryItem(MapLegendPosition.TopRight, MapLegendOrientation.Vertical, DashboardWinStringId.MapTopRightVerticalLegendPosition));
			verticalGroup.Items.Add(new MapLegendPositionGalleryItem(MapLegendPosition.BottomLeft, MapLegendOrientation.Vertical, DashboardWinStringId.MapBottomLeftVerticalLegendPosition));
			verticalGroup.Items.Add(new MapLegendPositionGalleryItem(MapLegendPosition.BottomCenter, MapLegendOrientation.Vertical, DashboardWinStringId.MapBottomCenterVerticalLegendPosition));
			verticalGroup.Items.Add(new MapLegendPositionGalleryItem(MapLegendPosition.BottomRight, MapLegendOrientation.Vertical, DashboardWinStringId.MapBottomRightVerticalLegendPosition));
			gallery.Groups.Add(verticalGroup);
			GalleryItemGroup horizontalGroup = new GalleryItemGroup();
			horizontalGroup.Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.MapGalleryHorizontalGroupCaption);
			horizontalGroup.Items.Add(new MapLegendPositionGalleryItem(MapLegendPosition.TopLeft, MapLegendOrientation.Horizontal, DashboardWinStringId.MapTopLeftHorizontalLegendPosition));
			horizontalGroup.Items.Add(new MapLegendPositionGalleryItem(MapLegendPosition.TopCenter, MapLegendOrientation.Horizontal, DashboardWinStringId.MapTopCenterHorizontalLegendPosition));
			horizontalGroup.Items.Add(new MapLegendPositionGalleryItem(MapLegendPosition.TopRight, MapLegendOrientation.Horizontal, DashboardWinStringId.MapTopRightHorizontalLegendPosition));
			horizontalGroup.Items.Add(new MapLegendPositionGalleryItem(MapLegendPosition.BottomLeft, MapLegendOrientation.Horizontal, DashboardWinStringId.MapBottomLeftHorizontalLegendPosition));
			horizontalGroup.Items.Add(new MapLegendPositionGalleryItem(MapLegendPosition.BottomCenter, MapLegendOrientation.Horizontal, DashboardWinStringId.MapBottomCenterHorizontalLegendPosition));
			horizontalGroup.Items.Add(new MapLegendPositionGalleryItem(MapLegendPosition.BottomRight, MapLegendOrientation.Horizontal, DashboardWinStringId.MapBottomRightHorizontalLegendPosition));
			gallery.Groups.Add(horizontalGroup);   
		}
	}
	public class GalleryWeightedLegendPositionItem : DashboardCommandGalleryBarItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.WeightedLegendPosition; } }
		protected override ICommandUIState CreateGalleryItemUIState() {
			return new WeightedLegendPositionCommandUIState(this);
		}
		protected override void PrepareGallery() {
			InRibbonGallery gallery = Gallery;
			gallery.ColumnCount = 3;
			gallery.RowCount = 2;
			GalleryItemGroup group = new GalleryItemGroup();
			group.Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.WeightedLegendGalleryGroupCaption);
			group.Items.Add(new WeightedLegendPositionGalleryItem(MapLegendPosition.TopLeft, DashboardWinStringId.MapTopLeftWeightedLegendPosition));
			group.Items.Add(new WeightedLegendPositionGalleryItem(MapLegendPosition.TopCenter, DashboardWinStringId.MapTopCenterWeightedLegendPosition));
			group.Items.Add(new WeightedLegendPositionGalleryItem(MapLegendPosition.TopRight, DashboardWinStringId.MapTopRightWeightedLegendPosition));
			group.Items.Add(new WeightedLegendPositionGalleryItem(MapLegendPosition.BottomLeft, DashboardWinStringId.MapBottomLeftWeightedLegendPosition));
			group.Items.Add(new WeightedLegendPositionGalleryItem(MapLegendPosition.BottomCenter, DashboardWinStringId.MapBottomCenterWeightedLegendPosition));
			group.Items.Add(new WeightedLegendPositionGalleryItem(MapLegendPosition.BottomRight, DashboardWinStringId.MapBottomRightWeightedLegendPosition));
			gallery.Groups.Add(group);   
		}
	}
}
namespace DevExpress.DashboardWin.Native {
	public class MapLegendItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new MapShowLegendBarItem());
			items.Add(new GalleryMapLegendPositionItem());
		}
	}
	public class WeightedLegendItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			ChangeWeightedLegendTypeBarItem typeItems = new ChangeWeightedLegendTypeBarItem();
			typeItems.AddBarItem(new WeightedLegendNoneBarItem());
			typeItems.AddBarItem(new WeightedLegendLinearBarItem());
			typeItems.AddBarItem(new WeightedLegendNestedBarItem());
			items.Add(typeItems);
			items.Add(new GalleryWeightedLegendPositionItem());
		}
	}
	public class WeightedLegendBarCreator<TPageCategory, TBar> : DashboardItemDesignBarCreator
		where TPageCategory : DashboardRibbonPageCategory, new()
		where TBar : MapToolsBarBase, new() {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(TPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(WeightedLegendPageGroup); } }
		public override Type SupportedBarType { get { return typeof(TBar); } }
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new TPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new WeightedLegendPageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new WeightedLegendItemBuilder();
		}
		public override Bar CreateBar() {
			return new TBar();
		}
	}
	public class MapLegendBarCreator<TPageCategory, TBar> : DashboardItemDesignBarCreator
		where TPageCategory : DashboardRibbonPageCategory, new()
		where TBar : MapToolsBarBase, new() {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(TPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(MapLegendPositionPageGroup); } }
		public override Type SupportedBarType { get { return typeof(TBar); } }
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new TPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new MapLegendPositionPageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new MapLegendItemBuilder();
		}
		public override Bar CreateBar() {
			return new TBar();
		}
	}
	public abstract class DashboardGalleryPositionCommandUIState<TGalleryItem, TObject> : ICommandUIState where TGalleryItem : DashboardCommandGalleryBarItem {
		readonly TGalleryItem galleryItem;
		public GalleryItem SelectedGalleryItem { get { return galleryItem.SelectedItem; } }
		public bool Enabled { get { return galleryItem.Enabled; } set { galleryItem.Enabled = value; } }
		public bool Checked { get { return false; } set { } }
		public object EditValue { get { return null; } set { } }
		public bool Visible {
			get { return galleryItem.Visibility == BarItemVisibility.Always; }
			set { galleryItem.Visibility = value ? BarItemVisibility.Always : BarItemVisibility.Never; }
		}
		protected DashboardGalleryPositionCommandUIState(TGalleryItem galleryItem) {
			this.galleryItem = galleryItem;
		}
		public void UpdateVisualState(TObject obj) {
			foreach(GalleryItemGroup group in galleryItem.Gallery.Groups)
				foreach(GalleryItem item in group.Items) {
					item.Checked = false;
					CheckGalleryItem(item, obj);
				}
		}
		protected abstract void CheckGalleryItem(GalleryItem item, TObject obj);
	}
	public class MapLegendPositionCommandUIState : DashboardGalleryPositionCommandUIState<GalleryMapLegendPositionItem, MapLegend> {
		public MapLegendPositionCommandUIState(GalleryMapLegendPositionItem galleryItem)
			: base(galleryItem) {
		}
		protected override void CheckGalleryItem(GalleryItem item, MapLegend obj) {
			MapLegendPositionGalleryItem positionItem = item as MapLegendPositionGalleryItem;
			if(positionItem != null && positionItem.Position == obj.Position && positionItem.Orientation == obj.Orientation)
				positionItem.Checked = true;
		}
	}
	public class WeightedLegendPositionCommandUIState : DashboardGalleryPositionCommandUIState<GalleryWeightedLegendPositionItem, WeightedLegend> {
		public WeightedLegendPositionCommandUIState(GalleryWeightedLegendPositionItem galleryItem)
			: base(galleryItem) {
		}
		protected override void CheckGalleryItem(GalleryItem item, WeightedLegend obj) {
			WeightedLegendPositionGalleryItem positionItem = item as WeightedLegendPositionGalleryItem;
			if(positionItem != null && positionItem.Position == obj.Position)
				positionItem.Checked = true;
		}
	}
}
