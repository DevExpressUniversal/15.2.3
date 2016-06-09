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
using System.Drawing;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
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
	public class GalleryChartSeriesTypeItem : DashboardCommandGalleryBarItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ChartSeriesType; } }
		protected override ICommandUIState CreateGalleryItemUIState() {
			DashboardChartSeriesTypeCommandUIState state = new DashboardChartSeriesTypeCommandUIState(this);
			ChartSeriesTypeGalleryItem chartSeriesTypeGalleryItem = SelectedItem as ChartSeriesTypeGalleryItem;
			if(chartSeriesTypeGalleryItem != null)
				state.SeriesTypeCaption = chartSeriesTypeGalleryItem.SeriesTypeCaption;
			return state;
		}
		protected override void PrepareGallery() {
			InRibbonGallery gallery = Gallery;
			gallery.ColumnCount = 5;
			gallery.RowCount = 8;
			foreach(SeriesViewGroup seriesViewGroup in ChartDashboardItem.SeriesViewGroups) {
				GalleryItemGroup group = new GalleryItemGroup();
				group.Caption = seriesViewGroup.Name;
				gallery.Groups.Add(group);
				foreach(ChartSeriesConverter converter in seriesViewGroup.Converters) {
					ChartSeriesTypeGalleryItem item = new ChartSeriesTypeGalleryItem();
					item.SeriesTypeCaption = converter.Caption;
					item.Hint = converter.Caption;
					item.Image = ImageHelper.GetImage(converter.GalleryImagePath);
					group.Items.Add(item);
				}
			}   
		}
	}
	public class ChartStylePageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupChartSeriesTypeCaption); } }
	}
	public class ChartSeriesTypeGalleryItem :GalleryItem {		
		public string SeriesTypeCaption { get; set; }
	}
}
namespace DevExpress.DashboardWin.Native {
	public class ChartSeriesTypeItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new GalleryChartSeriesTypeItem());
		}
	}
	public class ChartSeriesTypeBarCreator : DashboardItemDesignBarCreator {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(ChartToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ChartStylePageGroup); } }
		public override Type SupportedBarType { get { return typeof(ChartToolsBar); } }
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new ChartToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ChartStylePageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new ChartSeriesTypeItemBuilder();
		}
		public override Bar CreateBar() {
			return new ChartToolsBar();
		}
	}
	public class DashboardChartSeriesTypeCommandUIState : ICommandUIState {
		readonly RibbonGalleryBarItem item;		
		string seriesTypeCaption;
		public virtual bool Enabled { get { return item.Enabled; } set { item.Enabled = value; } }
		public virtual bool Visible { get { return item.Visibility == BarItemVisibility.Always; } set { ChangeVisible(value); } }
		public virtual bool Checked { get { return false; } set { } }
		public virtual object EditValue { get { return null; } set { } }
		public string SeriesTypeCaption {
			get { return seriesTypeCaption; }
			set {
				if(seriesTypeCaption != value) {
					seriesTypeCaption = value;
					UpdateVisualState();
				}
			}
		}
		public DashboardChartSeriesTypeCommandUIState(RibbonGalleryBarItem item) {
			this.item = item;
		}
		protected internal virtual void ChangeVisible(bool visible) {
			if (visible)
				item.Visibility = BarItemVisibility.Always;
			else
				item.Visibility = BarItemVisibility.Never;
		}
		void UpdateVisualState() {
			foreach (GalleryItemGroup group in item.Gallery.Groups) {
				int count = group.Items.Count;
				for (int i = 0; i < count; i++) {
					GalleryItem currentItem = group.Items[i];
					ChartSeriesTypeGalleryItem chartSeriesTypeItem = currentItem as ChartSeriesTypeGalleryItem;
					if (chartSeriesTypeItem != null && chartSeriesTypeItem.SeriesTypeCaption == seriesTypeCaption) {
						chartSeriesTypeItem.Checked = true;
						item.Gallery.MakeVisible(chartSeriesTypeItem);
					}
					else
						currentItem.Checked = false;
				}
			}
		}
	}
}
