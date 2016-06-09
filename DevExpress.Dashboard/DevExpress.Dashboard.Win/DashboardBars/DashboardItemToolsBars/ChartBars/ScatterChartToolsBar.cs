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
	public class ScatterChartToolsBar : DataDashboardItemToolsBar {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryScatterChartToolsName); } }
		protected internal override CommandBasedBarItemBuilder MasterFilterBuilder { get { return new MasterFilterItemBuilder(); } }
		protected internal override CommandBasedBarItemBuilder DrillDownBuilder { get { return null; } }
		protected internal override CommandBasedBarItemBuilder TargetDimensionsBuilder { get { return null; } }
		protected override bool CheckBarVisibility(DashboardItemViewer viewer) {
			return viewer is ScatterChartDashboardItemViewer;
		}
	}
	public class ScatterChartToolsRibbonPageCategory : DashboardRibbonPageCategory {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryScatterChartToolsName); } }
		protected override DashboardCommandId EmptyCommandId { get { return DashboardCommandId.None; } }
		protected internal override bool CheckCurrentPage(string dashboardItemType) {
			return dashboardItemType == DashboardItemType.ScatterChart;
		}
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new ScatterChartToolsRibbonPageCategory();
		}
	}
	public class ScatterChartLayoutPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupChartLayoutCaption); } }
	}
	public class ScatterChartPointLabelPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupScatterChartLabelsCaption); } }
	}
	public class ScatterChartRotateBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ScatterChartRotate; } }
	}
	public class ScatterChartShowLegendBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ScatterChartShowLegend; } }
	}
	public class ScatterChartXAxisSettingsBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ScatterChartXAxisSettings; } }
	}
	public class ScatterChartYAxisSettingsBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ScatterChartYAxisSettings; } }
	}
	public class ScatterChartPointLabelOptionsBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ScatterChartLabelOptions; } }
	}
}
namespace DevExpress.DashboardWin.Native {
	public class ScatterChartLayoutBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ScatterChartRotateBarItem());
			items.Add(new ScatterChartXAxisSettingsBarItem());
			items.Add(new ScatterChartYAxisSettingsBarItem());
		}
	}
	public class ScatterChartLayoutBarCreator : DashboardItemDesignBarCreator {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(ScatterChartToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ScatterChartLayoutPageGroup); } }
		public override Type SupportedBarType { get { return typeof(ScatterChartToolsBar); } }
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new ScatterChartToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ScatterChartLayoutPageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new ScatterChartLayoutBarItemBuilder();
		}
		public override Bar CreateBar() {
			return new ScatterChartToolsBar();
		}
	}
	public class ScatterChartLabelOptionsBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new ScatterChartPointLabelOptionsBarItem());
		}
	}
	public class ScatterChartPointLabelOptionsCreator : DashboardItemDesignBarCreator {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(ScatterChartToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(ScatterChartPointLabelPageGroup); } }
		public override Type SupportedBarType { get { return typeof(ScatterChartToolsBar); } }
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new ScatterChartToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ScatterChartPointLabelPageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new ScatterChartLabelOptionsBarItemBuilder();
		}
		public override Bar CreateBar() {
			return new ScatterChartToolsBar();
		}
	}
}
