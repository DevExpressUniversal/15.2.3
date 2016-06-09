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
using System.Windows.Forms;
using DevExpress.DashboardWin.Bars;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.DashboardWin.Native {
	public static class DashboardBarsUtils {
		static readonly ControlCommandBarCreator[] barCreators = new ControlCommandBarCreator[] {
			new FileBarCreator(),
			new HistoryBarCreator(),
			new DataSourceBarCreator(),
			new SqlDataSourceQueryBarCreator(),
			new DataSourceFilteringBarCreator(),
			new InsertBarCreator(),
			new ItemOperationBarCreator(),
			new GroupOperationBarCreator(),
			new DashboardDesignBarCreator(),
			new SkinsBarCreator(),
			new FilteringBarCreator<PivotToolsRibbonPageCategory, PivotToolsBar>(),
			new FilteringBarCreator<GridToolsRibbonPageCategory, GridToolsBar>(),
			new FilteringBarCreator<ChartToolsRibbonPageCategory, ChartToolsBar>(),
			new FilteringBarCreator<ScatterChartToolsRibbonPageCategory, ScatterChartToolsBar>(),
			new FilteringBarCreator<PiesToolsRibbonPageCategory, PiesToolsBar>(),
			new FilteringBarCreator<GaugesToolsRibbonPageCategory, GaugesToolsBar>(),
			new FilteringBarCreator<CardsToolsRibbonPageCategory, CardsToolsBar>(),
			new FilteringBarCreator<RangeFilterToolsRibbonPageCategory, RangeFilterToolsBar>(),
			new FilteringBarCreator<ChoroplethMapToolsRibbonPageCategory, ChoroplethMapToolsBar>(),
			new FilteringBarCreator<GeoPointMapToolsRibbonPageCategory, GeoPointMapToolsBar>(), 
			new FilteringBarCreator<BubbleMapToolsRibbonPageCategory, BubbleMapToolsBar>(),
			new FilteringBarCreator<PieMapToolsRibbonPageCategory, PieMapToolsBar>(),
			new FilteringBarCreator<FilterElementToolsRibbonPageCategory, FilterElementToolsBar>(),
			new MasterFilterBarCreator<GridToolsRibbonPageCategory, GridToolsBar>(),
			new MasterFilterBarCreator<ChartToolsRibbonPageCategory, ChartToolsBar>(),
			new MasterFilterBarCreator<ScatterChartToolsRibbonPageCategory, ScatterChartToolsBar>(),
			new MasterFilterBarCreator<PiesToolsRibbonPageCategory, PiesToolsBar>(),
			new MasterFilterBarCreator<GaugesToolsRibbonPageCategory, GaugesToolsBar>(),
			new MasterFilterBarCreator<CardsToolsRibbonPageCategory, CardsToolsBar>(),
			new MasterFilterBarCreator<ChoroplethMapToolsRibbonPageCategory, ChoroplethMapToolsBar>(),
			new MasterFilterBarCreator<GeoPointMapToolsRibbonPageCategory, GeoPointMapToolsBar>(),
			new MasterFilterBarCreator<BubbleMapToolsRibbonPageCategory, BubbleMapToolsBar>(),
			new MasterFilterBarCreator<PieMapToolsRibbonPageCategory, PieMapToolsBar>(),
			new MasterFilterBarCreator<GroupToolsRibbonPageCategory, GroupToolsBar>(),
			new MasterFilterOptionsBarCreator<PivotToolsRibbonPageCategory, PivotToolsBar>(),
			new MasterFilterOptionsBarCreator<GridToolsRibbonPageCategory, GridToolsBar>(),
			new MasterFilterOptionsBarCreator<ChartToolsRibbonPageCategory, ChartToolsBar>(),
			new MasterFilterOptionsBarCreator<ScatterChartToolsRibbonPageCategory, ScatterChartToolsBar>(),
			new MasterFilterOptionsBarCreator<PiesToolsRibbonPageCategory, PiesToolsBar>(),
			new MasterFilterOptionsBarCreator<GaugesToolsRibbonPageCategory, GaugesToolsBar>(),
			new MasterFilterOptionsBarCreator<CardsToolsRibbonPageCategory, CardsToolsBar>(),
			new MasterFilterOptionsBarCreator<RangeFilterToolsRibbonPageCategory, RangeFilterToolsBar>(),
			new MasterFilterOptionsBarCreator<ChoroplethMapToolsRibbonPageCategory, ChoroplethMapToolsBar>(),
			new MasterFilterOptionsBarCreator<GeoPointMapToolsRibbonPageCategory, GeoPointMapToolsBar>(),
			new MasterFilterOptionsBarCreator<BubbleMapToolsRibbonPageCategory, BubbleMapToolsBar>(),
			new MasterFilterOptionsBarCreator<PieMapToolsRibbonPageCategory, PieMapToolsBar>(),
			new MasterFilterOptionsBarCreator<FilterElementToolsRibbonPageCategory, FilterElementToolsBar>(),
			new TargetDimensionsBarCreator<ChartToolsRibbonPageCategory, ChartToolsBar>(),
			new TargetDimensionsBarCreator<PiesToolsRibbonPageCategory, PiesToolsBar>(),
			new GeoPointMapClusterizationBarCreator<GeoPointMapToolsRibbonPageCategory, GeoPointMapToolsBar>(),
			new GeoPointMapClusterizationBarCreator<BubbleMapToolsRibbonPageCategory, BubbleMapToolsBar>(),
			new GeoPointMapClusterizationBarCreator<PieMapToolsRibbonPageCategory, PieMapToolsBar>(),
			new CommonItemDesignBarCreator<GridToolsRibbonPageCategory, GridToolsBar>(), 
			new CommonItemDesignBarCreator<ChartToolsRibbonPageCategory, ChartToolsBar>(), 
			new CommonItemDesignBarCreator<ScatterChartToolsRibbonPageCategory, ScatterChartToolsBar>(), 
			new CommonItemDesignBarCreator<PiesToolsRibbonPageCategory, PiesToolsBar>(), 
			new CommonItemDesignBarCreator<GaugesToolsRibbonPageCategory, GaugesToolsBar>(), 
			new CommonItemDesignBarCreator<CardsToolsRibbonPageCategory, CardsToolsBar>(), 
			new CommonItemDesignBarCreator<ImageToolsRibbonPageCategory, ImageToolsBar>(), 
			new CommonItemDesignBarCreator<TextBoxToolsRibbonPageCategory, TextBoxToolsBar>(), 
			new CommonItemDesignBarCreator<RangeFilterToolsRibbonPageCategory, RangeFilterToolsBar>(), 
			new CommonItemDesignBarCreator<PivotToolsRibbonPageCategory, PivotToolsBar>(), 
			new CommonItemDesignBarCreator<ChoroplethMapToolsRibbonPageCategory, ChoroplethMapToolsBar>(), 
			new CommonItemDesignBarCreator<GeoPointMapToolsRibbonPageCategory, GeoPointMapToolsBar>(),
			new CommonItemDesignBarCreator<BubbleMapToolsRibbonPageCategory, BubbleMapToolsBar>(),
			new CommonItemDesignBarCreator<PieMapToolsRibbonPageCategory, PieMapToolsBar>(),
			new CommonItemDesignBarCreator<GroupToolsRibbonPageCategory, GroupToolsBar>(),
			new CommonItemDesignBarCreator<FilterElementToolsRibbonPageCategory, FilterElementToolsBar>(),
			new ContentArrangementBarCreator<PiesToolsRibbonPageCategory, PiesToolsBar>(),
			new ContentArrangementBarCreator<GaugesToolsRibbonPageCategory, GaugesToolsBar>(),
			new ContentArrangementBarCreator<CardsToolsRibbonPageCategory, CardsToolsBar>(),
			new GridStyleBarCreator(),
			new GridLayoutBarCreator(),
			new GridAutoFitToContentsColumnWidthModeBarCreator(),
			new ChartLayoutBarCreator(),
			new ScatterChartLayoutBarCreator(),
			new ScatterChartPointLabelOptionsCreator(),
			new ChartLegendPositionBarCreator(),
			new ScatterChartLegendPositionBarCreator(),			
			new ChartSeriesTypeBarCreator(),
			new PieLabelsBarCreator(),
			new PieStyleBarCreator(),
			new PieShowCaptionsBarCreator(),
			new GaugeStyleBarCreator(),
			new GaugeShowCaptionsBarCreator(),
			new ImageOpenBarCreator(),
			new ImageSizeModeBarCreator(), 
			new ImageAlignmentBarCreator(),
			new TextBoxFormatBarCreator(),
			new RangeFilterSeriesTypeBarCreator(),
			new PivotLayoutToolsBarCreator(),
			new FilterElementTypeBarCreator<ComboBoxTypeBarItemBuilder>(),
			new FilterElementTypeBarCreator<ListBoxTypeBarItemBuilder>(),
			new TreeViewLayoutBarCreator(),
			new FilterElementItemOptionsBarCreator(),
			new MapShapefileBarCreator<ChoroplethMapToolsRibbonPageCategory, ChoroplethMapToolsBar>(),
			new MapShapefileBarCreator<GeoPointMapToolsRibbonPageCategory, GeoPointMapToolsBar>(),
			new MapShapefileBarCreator<BubbleMapToolsRibbonPageCategory, BubbleMapToolsBar>(),
			new MapShapefileBarCreator<PieMapToolsRibbonPageCategory, PieMapToolsBar>(),
			new MapNavigationOptionsBarCreator<ChoroplethMapToolsRibbonPageCategory, ChoroplethMapToolsBar>(),
			new MapNavigationOptionsBarCreator<GeoPointMapToolsRibbonPageCategory, GeoPointMapToolsBar>(),
			new MapNavigationOptionsBarCreator<BubbleMapToolsRibbonPageCategory, BubbleMapToolsBar>(),
			new MapNavigationOptionsBarCreator<PieMapToolsRibbonPageCategory, PieMapToolsBar>(),
			new ChoroplethMapShapeLabelsAttributeBarCreator(),
			new MapShapeLabelsAttributeBarCreator<GeoPointMapToolsRibbonPageCategory, GeoPointMapToolsBar>(),
			new MapShapeLabelsAttributeBarCreator<BubbleMapToolsRibbonPageCategory, BubbleMapToolsBar>(),
			new MapShapeLabelsAttributeBarCreator<PieMapToolsRibbonPageCategory, PieMapToolsBar>(),
			new MapLegendBarCreator<ChoroplethMapToolsRibbonPageCategory, ChoroplethMapToolsBar>(),
			new MapLegendBarCreator<BubbleMapToolsRibbonPageCategory, BubbleMapToolsBar>(),
			new MapLegendBarCreator<PieMapToolsRibbonPageCategory, PieMapToolsBar>(),
			new WeightedLegendBarCreator<BubbleMapToolsRibbonPageCategory, BubbleMapToolsBar>(),
			new WeightedLegendBarCreator<PieMapToolsRibbonPageCategory, PieMapToolsBar>(),
			new PieMapOptionsBarCreator<PieMapToolsRibbonPageCategory>(),
			new ColoringBarCreator<ChartToolsRibbonPageCategory, ChartToolsBar>(),
			new ColoringBarCreator<ScatterChartToolsRibbonPageCategory, ScatterChartToolsBar>(),
			new ColoringBarCreator<PiesToolsRibbonPageCategory, PiesToolsBar>()
		};
		public static Type DashboardSaveBarItemType { get { return typeof(FileSaveBarItem); } }
		static RibbonPage FindPageInternal(RibbonPageCollection ribbonPages, Type pageType) {
			foreach (RibbonPage page in ribbonPages)
				if (page.GetType() == pageType)
					return page;
			return null;
		}
		public static RibbonPage FindPage(RibbonControl ribbonControl, Type pageType) {
			RibbonPage page = FindPageInternal(ribbonControl.Pages, pageType);
			if (page != null)
				return page;
			foreach (RibbonPageCategory pageCategory in ribbonControl.PageCategories) {
				page = FindPageInternal(pageCategory.Pages, pageType);
				if (page != null)
					return page;
			}
			return null;
		}
		public static void SetupRibbon(RibbonControl ribbon, DashboardBackstageViewControl viewControl, IServiceProvider serviceProvider) {
			ribbon.RibbonStyle = RibbonControlStyle.Office2010;
			ribbon.ToolbarLocation = RibbonQuickAccessToolbarLocation.Above;
			ribbon.Dock = DockStyle.Top;
			ribbon.ApplicationButtonDropDownControl = viewControl;
			viewControl.Initialize(serviceProvider);
			viewControl.DashboardRecentTab.Selected = true;
		}
		public static ControlCommandBarCreator[] GetBarCreators(bool createRibbon) {
			if (!createRibbon)
				return barCreators;
			List<ControlCommandBarCreator> creators = new List<ControlCommandBarCreator>(barCreators);
			creators.Add(new QuickAccessHistoryBarCreator());
			return creators.ToArray();
		}
		static BarItem FindBarItem(BarManager barManager, Type barItemType) {
			foreach(BarItem barItem in barManager.Items)
				if(barItem.GetType() == barItemType)
					return barItem;
			return null;
		}
		public static void SetupDesignerPopupMenu(DashboardPopupMenu popupMenu) {
			List<DashboardDesignerPopupMenuCreator> creators = new List<DashboardDesignerPopupMenuCreator>();
			creators.Add(new ShowItemCaptionBarItemPopupMenuCreator());
			creators.Add(new CommonDashboardItemBarItemsPopupMenuCreator());
			creators.Add(new EditNamesBarItemsPopupMenuCreator());
			creators.Add(new DashboardItemGroupBarItemsPopupMenuCreator());
			creators.Add(new FilterOperationsBarItemsPopupMenuCreator());
			creators.Add(new CommonMapBarItemsPopupMenuCreator());
			creators.Add(new MapFullExtentBarItemPopupMenuCreator());
			creators.Add(new ImageOperationsBarItemsPopupMenuCreator());
			creators.Add(new TextBoxEditTextBarItemPopupMenuCreator());
			BarManager barManager = popupMenu.Manager;
			for(int i = 0; i < creators.Count; i++) {
				List<Type> barItemsTypes = creators[i].GetBarItemTypes();
				for(int j = 0; j < barItemsTypes.Count; j++) {
					Type barItemType = barItemsTypes[j];
					BarItem item = FindBarItem(barManager, barItemType);
					if(item != null) {
						BarItemLink itemLink = popupMenu.AddItem(item);
						if(i != 0 && j == 0)
							itemLink.BeginGroup = true;
					}
				}
			}
		}
	}
}
