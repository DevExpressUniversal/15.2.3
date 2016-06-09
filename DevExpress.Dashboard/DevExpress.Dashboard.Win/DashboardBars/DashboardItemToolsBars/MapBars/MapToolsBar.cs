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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Viewer;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.DashboardWin.Bars {
	public abstract class MapToolsBarBase : DataDashboardItemToolsBar {
		protected internal override CommandBasedBarItemBuilder MasterFilterBuilder { get { return new MapDashboardItemMasterFilterItemBuilder(); } }
		protected internal override CommandBasedBarItemBuilder DrillDownBuilder { get { return null; } }
		protected internal override CommandBasedBarItemBuilder TargetDimensionsBuilder { get { return null; } }
	}
	public class MapToolsBar : MapToolsBarBase {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryChoroplethMapToolsName); } }
	}
	public class ChoroplethMapToolsBar : MapToolsBarBase {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryChoroplethMapToolsName); } }
		protected override bool CheckBarVisibility(DashboardItemViewer viewer) {
			return viewer is ChoroplethMapDashboardItemViewer;
		}
	}
	public abstract class GeoPointMapBaseToolsBar : MapToolsBarBase {
		protected abstract GeoPointMapKind GeoPointMapKind { get; }
		protected override bool CheckBarVisibility(DashboardItemViewer viewer) {
			GeoPointMapDashboardItemViewerBase mapViewer = viewer as GeoPointMapDashboardItemViewerBase;
			if(mapViewer != null) {
				GeoPointMapDashboardItemViewControlBase viewControl = mapViewer.GeoPointViewControlBase;
				if(viewControl != null) {
					GeoPointMapDashboardItemViewModelBase viewModel = viewControl.CurrentGeoPointViewModel;
					if(viewModel != null)
						return viewModel.MapKind == GeoPointMapKind;
				}
			}
			return false;
		}
	}
	public class GeoPointMapToolsBar : GeoPointMapBaseToolsBar {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryGeoPointMapToolsName); } }
		protected override GeoPointMapKind GeoPointMapKind { get { return GeoPointMapKind.GeoPoint; } }
	}
	public class BubbleMapToolsBar : GeoPointMapBaseToolsBar {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryBubbleMapToolsName); } }
		protected override GeoPointMapKind GeoPointMapKind { get { return GeoPointMapKind.BubbleMap; } }
	}
	public class PieMapToolsBar : GeoPointMapBaseToolsBar {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryPieMapToolsName); } }
		protected override GeoPointMapKind GeoPointMapKind { get { return GeoPointMapKind.PieMap; } }
	}
	public abstract class BaseMapToolsRibbonPageCategory : DashboardRibbonPageCategory {
		protected override DashboardCommandId EmptyCommandId { get { return DashboardCommandId.None; } }
		protected BaseMapToolsRibbonPageCategory() {
			Visible = false;
		}
	}
	public class ChoroplethMapToolsRibbonPageCategory : BaseMapToolsRibbonPageCategory {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryChoroplethMapToolsName); } }
		protected internal override bool CheckCurrentPage(string dashboardItemType) {
			return dashboardItemType == DashboardItemType.ChoroplethMap;
		}
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new ChoroplethMapToolsRibbonPageCategory();
		}
	}
	public class GeoPointMapToolsRibbonPageCategory : BaseMapToolsRibbonPageCategory {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryGeoPointMapToolsName); } }
		protected internal override bool CheckCurrentPage(string dashboardItemType) {
			return dashboardItemType == DashboardItemType.GeoPointMap;
		}
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new GeoPointMapToolsRibbonPageCategory();
		}
	}
	public class BubbleMapToolsRibbonPageCategory : BaseMapToolsRibbonPageCategory {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryBubbleMapToolsName); } }
		protected internal override bool CheckCurrentPage(string dashboardItemType) {
			return dashboardItemType == DashboardItemType.BubbleMap;
		}
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new BubbleMapToolsRibbonPageCategory();
		}
	}
	public class PieMapToolsRibbonPageCategory : BaseMapToolsRibbonPageCategory {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryPieMapToolsName); } }
		protected internal override bool CheckCurrentPage(string dashboardItemType) {
			return dashboardItemType == DashboardItemType.PieMap;
		}
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new PieMapToolsRibbonPageCategory();
		}
	}
	public class MapShowLegendBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.MapShowLegend; } }
	}
	public class WeightedLegendNoneBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.WeightedLegendNoneType; } }
	}
}
