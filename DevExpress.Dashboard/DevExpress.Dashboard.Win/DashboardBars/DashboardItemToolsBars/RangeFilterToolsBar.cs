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
	public class RangeFilterToolsBar : DataDashboardItemToolsBar {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryRangeFilterToolsName); } }
		protected internal override CommandBasedBarItemBuilder MasterFilterBuilder { get { return null; } }
		protected internal override CommandBasedBarItemBuilder DrillDownBuilder { get { return null; } }
		protected internal override CommandBasedBarItemBuilder TargetDimensionsBuilder { get { return null; } }
		protected override bool CheckBarVisibility(DashboardItemViewer viewer) {
			return viewer is RangeFilterDashboardItemViewer;
		}
	}
	public class RangeFilterToolsRibbonPageCategory : DashboardRibbonPageCategory {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryRangeFilterToolsName); } }
		protected override DashboardCommandId EmptyCommandId { get { return DashboardCommandId.None; } }
		protected internal override bool CheckCurrentPage(string dashboardItemType) {
			return dashboardItemType == DashboardItemType.RangeFilter;
		}
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new RangeFilterToolsRibbonPageCategory();
		}
	}
	public class RangeFilterSeriesTypeRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupRangeFilterSeriesTypeCaption); } }
	}
	public class RangeFilterLineSeriesTypeBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.RangeFilterSeriesTypeLine; } }
	}
	public class RangeFilterStackedLineSeriesTypeBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.RangeFilterSeriesTypeStackedLine; } }
	}
	public class RangeFilterFullStackedLineSeriesTypeBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.RangeFilterSeriesTypeFullStackedLine; } }
	}
	public class RangeFilterAreaSeriesTypeBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.RangeFilterSeriesTypeArea; } }
	}
	public class RangeFilterStackedAreaSeriesTypeBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.RangeFilterSeriesTypeStackedArea; } }
	}
	public class RangeFilterFullStackedAreaSeriesTypeBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.RangeFilterSeriesTypeFullStackedArea; } }
	}
}
namespace DevExpress.DashboardWin.Native {
	public class RangeFilterSeriesTypeBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new RangeFilterLineSeriesTypeBarItem());
			items.Add(new RangeFilterStackedLineSeriesTypeBarItem());
			items.Add(new RangeFilterFullStackedLineSeriesTypeBarItem());
			items.Add(new RangeFilterAreaSeriesTypeBarItem());
			items.Add(new RangeFilterStackedAreaSeriesTypeBarItem());
			items.Add(new RangeFilterFullStackedAreaSeriesTypeBarItem());
		}
	}
	public class RangeFilterSeriesTypeBarCreator : DashboardItemDesignBarCreator {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(RangeFilterToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(RangeFilterSeriesTypeRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(RangeFilterToolsBar); } }
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new RangeFilterToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new RangeFilterSeriesTypeRibbonPageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new RangeFilterSeriesTypeBarItemBuilder();
		}
		public override Bar CreateBar() {
			return new RangeFilterToolsBar();
		}
	}
}
