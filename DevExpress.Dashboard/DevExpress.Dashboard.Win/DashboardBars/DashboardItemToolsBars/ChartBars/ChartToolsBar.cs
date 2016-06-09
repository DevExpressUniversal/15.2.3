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
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.DashboardWin.Bars {
	public class ChartToolsBar : DataDashboardItemToolsBar {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryChartToolsName); } }
		protected internal override CommandBasedBarItemBuilder MasterFilterBuilder { get { return new MasterFilterItemBuilder(); } }
		protected internal override CommandBasedBarItemBuilder DrillDownBuilder { get { return null; } }
		protected internal override CommandBasedBarItemBuilder TargetDimensionsBuilder { get { return new TargetDimensionsChartItemBuilder(); } }
		protected override bool CheckBarVisibility(DashboardItemViewer viewer) {
			return viewer is ChartDashboardItemViewer;
		}
	}
	public class ChartToolsRibbonPageCategory : DashboardRibbonPageCategory {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryChartToolsName); } }
		protected override DashboardCommandId EmptyCommandId { get { return DashboardCommandId.None; } }
		protected internal override bool CheckCurrentPage(string dashboardItemType) {
			return dashboardItemType == DashboardItemType.Chart;
		}
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new ChartToolsRibbonPageCategory();
		}
	}
	public class ChartLayoutPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupChartLayoutCaption); } }
	}
	public class ChartRotateBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ChartRotate; } }
	}
	public class ChartShowLegendBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ChartShowLegend; } }
	}
	public class ChartXAxisSettingsBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ChartXAxisSettings; } }
	}
	public class ChartYAxisSettingsBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ChartYAxisSettings; } }
	}
}
namespace DevExpress.DashboardWin.Native {
	public class ChartLayoutBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ChartRotateBarItem());
			items.Add(new ChartXAxisSettingsBarItem());
			items.Add(new ChartYAxisSettingsBarItem());
		}
	}
	public class ChartLayoutBarCreator : DashboardItemDesignBarCreator {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(ChartToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ChartLayoutPageGroup); } }
		public override Type SupportedBarType { get { return typeof(ChartToolsBar); } }
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new ChartToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ChartLayoutPageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new ChartLayoutBarItemBuilder();
		}
		public override Bar CreateBar() {
			return new ChartToolsBar();
		}
	}
}
