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
using System.Drawing;
using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.Utils.Commands;
namespace DevExpress.DashboardWin.Bars {
	public class ChartLegendPositionPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupChartLegendPositionCaption); } }
	}
	public class ScatterChartLegendPositionPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupChartLegendPositionCaption); } }
	}
	public abstract class ChartLegendPositionGalleryItem : GalleryItem {
		protected ChartLegendPositionGalleryItem(DashboardWinStringId hintId, string imageName) {
			Hint = DashboardWinLocalizer.GetString(hintId);
			Image = ImageHelper.GetImage(string.Format(@"ChartLegendPositions.{0}", imageName));
		}
	}
	public class ChartLegendInsidePositionGalleryItem : ChartLegendPositionGalleryItem {
		readonly ChartLegendInsidePosition position;
		internal ChartLegendInsidePosition Position { get { return position; } }
		public ChartLegendInsidePositionGalleryItem(ChartLegendInsidePosition position, DashboardWinStringId hintId)
			: base(hintId, string.Format(@"{0}_{1}", position, "Inside")) {
			this.position = position;
		}
	}
	public class ChartLegendOutsidePositionGalleryItem : ChartLegendPositionGalleryItem {
		readonly ChartLegendOutsidePosition position;
		internal ChartLegendOutsidePosition Position { get { return position; } }
		public ChartLegendOutsidePositionGalleryItem(ChartLegendOutsidePosition position, DashboardWinStringId hintId)
			: base(hintId, string.Format(@"{0}_{1}", position, "Outside")) {
			this.position = position;
		}
	}
	public abstract class GalleryChartLegendPositionItemBase : DashboardCommandGalleryBarItem {
		protected override ICommandUIState CreateGalleryItemUIState() {
			return new DashboardChartLegendPositionCommandUIState(this);
		}
		protected override void PrepareGallery() {
			InRibbonGallery gallery = Gallery;
			gallery.ColumnCount = 3;
			gallery.RowCount = 8;
			GalleryItemGroup insideHorizontalGroup = new GalleryItemGroup();
			insideHorizontalGroup.Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.ChartInsideHorizontalGalleryGroupCaption);
			insideHorizontalGroup.Items.Add(new ChartLegendInsidePositionGalleryItem(ChartLegendInsidePosition.TopLeftHorizontal, DashboardWinStringId.ChartTopLeftHorizontalInsideLegendPosition));
			insideHorizontalGroup.Items.Add(new ChartLegendInsidePositionGalleryItem(ChartLegendInsidePosition.TopCenterHorizontal, DashboardWinStringId.ChartTopCenterHorizontalInsideLegendPosition));
			insideHorizontalGroup.Items.Add(new ChartLegendInsidePositionGalleryItem(ChartLegendInsidePosition.TopRightHorizontal, DashboardWinStringId.ChartTopRightHorizontalInsideLegendPosition));
			insideHorizontalGroup.Items.Add(new ChartLegendInsidePositionGalleryItem(ChartLegendInsidePosition.BottomLeftHorizontal, DashboardWinStringId.ChartBottomLeftHorizontalInsideLegendPosition));
			insideHorizontalGroup.Items.Add(new ChartLegendInsidePositionGalleryItem(ChartLegendInsidePosition.BottomCenterHorizontal, DashboardWinStringId.ChartBottomCenterHorizontalInsideLegendPosition));
			insideHorizontalGroup.Items.Add(new ChartLegendInsidePositionGalleryItem(ChartLegendInsidePosition.BottomRightHorizontal, DashboardWinStringId.ChartBottomRightHorizontalInsideLegendPosition));
			gallery.Groups.Add(insideHorizontalGroup);
			GalleryItemGroup insideVerticalGroup = new GalleryItemGroup();
			insideVerticalGroup.Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.ChartInsideVerticalGalleryGroupCaption);
			insideVerticalGroup.Items.Add(new ChartLegendInsidePositionGalleryItem(ChartLegendInsidePosition.TopLeftVertical, DashboardWinStringId.ChartTopLeftVerticalInsideLegendPosition));
			insideVerticalGroup.Items.Add(new ChartLegendInsidePositionGalleryItem(ChartLegendInsidePosition.TopCenterVertical, DashboardWinStringId.ChartTopCenterVerticalInsideLegendPosition));
			insideVerticalGroup.Items.Add(new ChartLegendInsidePositionGalleryItem(ChartLegendInsidePosition.TopRightVertical, DashboardWinStringId.ChartTopRightVerticalInsideLegendPosition));
			insideVerticalGroup.Items.Add(new ChartLegendInsidePositionGalleryItem(ChartLegendInsidePosition.BottomLeftVertical, DashboardWinStringId.ChartBottomLeftVerticalInsideLegendPosition));
			insideVerticalGroup.Items.Add(new ChartLegendInsidePositionGalleryItem(ChartLegendInsidePosition.BottomCenterVertical, DashboardWinStringId.ChartBottomCenterVerticalInsideLegendPosition));
			insideVerticalGroup.Items.Add(new ChartLegendInsidePositionGalleryItem(ChartLegendInsidePosition.BottomRightVertical, DashboardWinStringId.ChartBottomRightVerticalInsideLegendPosition));
			gallery.Groups.Add(insideVerticalGroup);
			GalleryItemGroup outsideHorizontalGroup = new GalleryItemGroup();
			outsideHorizontalGroup.Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.ChartOutsideHorizontalGalleryGroupCaption);
			outsideHorizontalGroup.Items.Add(new ChartLegendOutsidePositionGalleryItem(ChartLegendOutsidePosition.TopLeftHorizontal, DashboardWinStringId.ChartTopLeftHorizontalOutsideLegendPosition));
			outsideHorizontalGroup.Items.Add(new ChartLegendOutsidePositionGalleryItem(ChartLegendOutsidePosition.TopCenterHorizontal, DashboardWinStringId.ChartTopCenterHorizontalOutsideLegendPosition));
			outsideHorizontalGroup.Items.Add(new ChartLegendOutsidePositionGalleryItem(ChartLegendOutsidePosition.TopRightHorizontal, DashboardWinStringId.ChartTopRightHorizontalOutsideLegendPosition));
			outsideHorizontalGroup.Items.Add(new ChartLegendOutsidePositionGalleryItem(ChartLegendOutsidePosition.BottomLeftHorizontal, DashboardWinStringId.ChartBottomLeftHorizontalOutsideLegendPosition));
			outsideHorizontalGroup.Items.Add(new ChartLegendOutsidePositionGalleryItem(ChartLegendOutsidePosition.BottomCenterHorizontal, DashboardWinStringId.ChartBottomCenterHorizontalOutsideLegendPosition));
			outsideHorizontalGroup.Items.Add(new ChartLegendOutsidePositionGalleryItem(ChartLegendOutsidePosition.BottomRightHorizontal, DashboardWinStringId.ChartBottomRightHorizontalOutsideLegendPosition));
			gallery.Groups.Add(outsideHorizontalGroup);
			GalleryItemGroup outsideVerticalGroup = new GalleryItemGroup();
			outsideVerticalGroup.Caption = DashboardWinLocalizer.GetString(DashboardWinStringId.ChartOutsideVerticalGalleryGroupCaption);
			outsideVerticalGroup.Items.Add(new ChartLegendOutsidePositionGalleryItem(ChartLegendOutsidePosition.TopLeftVertical, DashboardWinStringId.ChartTopLeftVerticalOutsideLegendPosition));
			outsideVerticalGroup.Items.Add(new ChartLegendOutsidePositionGalleryItem(ChartLegendOutsidePosition.TopRightVertical, DashboardWinStringId.ChartTopRightVerticalOutsideLegendPosition));
			outsideVerticalGroup.Items.Add(new ChartLegendOutsidePositionGalleryItem(ChartLegendOutsidePosition.BottomLeftVertical, DashboardWinStringId.ChartBottomLeftVerticalOutsideLegendPosition));
			outsideVerticalGroup.Items.Add(new ChartLegendOutsidePositionGalleryItem(ChartLegendOutsidePosition.BottomRightVertical, DashboardWinStringId.ChartBottomRightVerticalOutsideLegendPosition));
			gallery.Groups.Add(outsideVerticalGroup);
		}
	}
	public class GalleryChartLegendPositionItem : GalleryChartLegendPositionItemBase {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ChartLegendPosition; } }
	}
	public class GalleryScatterChartLegendPositionItem : GalleryChartLegendPositionItemBase {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ScatterChartLegendPosition; } }
	}
}
namespace DevExpress.DashboardWin.Native {
	public class ChartLegendPositionItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ChartShowLegendBarItem());
			items.Add(new GalleryChartLegendPositionItem());
		}
	}
	public class ChartLegendPositionBarCreator : DashboardItemDesignBarCreator {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(ChartToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ChartLegendPositionPageGroup); } }
		public override Type SupportedBarType { get { return typeof(ChartToolsBar); } }
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new ChartToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ChartLegendPositionPageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new ChartLegendPositionItemBuilder();
		}
		public override Bar CreateBar() {
			return new ChartToolsBar();
		}
	}
	public class ScatterChartLegendPositionItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ScatterChartShowLegendBarItem());
			items.Add(new GalleryScatterChartLegendPositionItem());
		}
	}
	public class ScatterChartLegendPositionBarCreator : DashboardItemDesignBarCreator {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(ScatterChartToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ScatterChartLegendPositionPageGroup); } }
		public override Type SupportedBarType { get { return typeof(ScatterChartToolsBar); } }
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new ScatterChartToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ScatterChartLegendPositionPageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new ScatterChartLegendPositionItemBuilder();
		}
		public override Bar CreateBar() {
			return new ScatterChartToolsBar();
		}
	}
	public class DashboardChartLegendPositionCommandUIState : ICommandUIState {
		readonly GalleryChartLegendPositionItemBase galleryItem;
		public bool Enabled { get { return galleryItem.Enabled; } set { galleryItem.Enabled = value; } }
		public bool Checked { get { return false; } set { } }
		public object EditValue { get { return null; } set { } }
		public bool Visible {
			get { return galleryItem.Visibility == BarItemVisibility.Always; }
			set { galleryItem.Visibility = value ? BarItemVisibility.Always : BarItemVisibility.Never; }
		}
		public ChartLegendPositionGalleryItem SelectedChartLegendPositionGalleryItem {
			get {
				return (ChartLegendPositionGalleryItem)galleryItem.SelectedItem;
			}
		}
		public DashboardChartLegendPositionCommandUIState(GalleryChartLegendPositionItemBase galleryItem) {
			this.galleryItem = galleryItem;
		}
		void MakeAlignmentItemVisible(ChartLegendPositionGalleryItem alignmentItem) {
			alignmentItem.Checked = true;
			galleryItem.Gallery.MakeVisible(alignmentItem);
		}
		public void UpdateVisualState(ChartLegend chartLegend) {
			foreach (GalleryItemGroup group in galleryItem.Gallery.Groups)
				foreach (GalleryItem item in group.Items) {
					item.Checked = false;
					if (chartLegend.IsInsideDiagram) {
						ChartLegendInsidePositionGalleryItem positionItem = item as ChartLegendInsidePositionGalleryItem;
						if (positionItem != null && positionItem.Position == chartLegend.InsidePosition)
							MakeAlignmentItemVisible(positionItem);
					}
					else {
						ChartLegendOutsidePositionGalleryItem positionItem = item as ChartLegendOutsidePositionGalleryItem;
						if (positionItem != null && positionItem.Position == chartLegend.OutsidePosition)
							MakeAlignmentItemVisible(positionItem);
					}
				}
		}
	}
}
