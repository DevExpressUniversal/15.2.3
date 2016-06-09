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
	public class GaugesToolsBar : DataDashboardItemToolsBar {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryGaugesToolsName); } }
		protected internal override CommandBasedBarItemBuilder MasterFilterBuilder { get { return new MasterFilterItemBuilder(); } }
		protected internal override CommandBasedBarItemBuilder DrillDownBuilder { get { return null; } }
		protected internal override CommandBasedBarItemBuilder TargetDimensionsBuilder { get { return null; } }
		protected override bool CheckBarVisibility(DashboardItemViewer viewer) {
			return viewer is GaugeDashboardItemViewer;
		}
	}
	public class GaugesToolsRibbonPageCategory : DashboardRibbonPageCategory {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryGaugesToolsName); } }
		protected override DashboardCommandId EmptyCommandId { get { return DashboardCommandId.None; } }
		protected internal override bool CheckCurrentPage(string dashboardItemType) {
			return dashboardItemType == DashboardItemType.Gauge;
		}
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new GaugesToolsRibbonPageCategory();
		}
	}
	public class GaugeStyleRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupGaugeStyleCaption); } }
	}
	public class GaugeStyleFullCircularBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.GaugeStyleFullCircular; } }
	}
	public class GaugeStyleHalfCircularBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.GaugeStyleHalfCircular; } }
	}
	public class GaugeStyleLeftQuarterCircularBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.GaugeStyleLeftQuarterCircular; } }
	}
	public class GaugeStyleRightQuarterCircularBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.GaugeStyleRightQuarterCircular; } }
	}
	public class GaugeStyleThreeForthCircularBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.GaugeStyleThreeFourthCircular; } }
	}
	public class GaugeStyleLinearHorizontalBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.GaugeStyleLinearHorizontal; } }
	}
	public class GaugeStyleLinearVerticalBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.GaugeStyleLinearVertical; } }
	}
}
namespace DevExpress.DashboardWin.Native {
	public class GaugeStyleBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new GaugeStyleFullCircularBarItem());
			items.Add(new GaugeStyleHalfCircularBarItem());
			items.Add(new GaugeStyleLeftQuarterCircularBarItem());
			items.Add(new GaugeStyleRightQuarterCircularBarItem());
			items.Add(new GaugeStyleThreeForthCircularBarItem());
			items.Add(new GaugeStyleLinearHorizontalBarItem());
			items.Add(new GaugeStyleLinearVerticalBarItem());
		}
	}
	public class GaugeStyleBarCreator : DashboardItemDesignBarCreator {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(GaugesToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(GaugeStyleRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(GaugesToolsBar); } }
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new GaugesToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new GaugeStyleRibbonPageGroup();
		}		
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new GaugeStyleBarItemBuilder();
		}
		public override Bar CreateBar() {
			return new GaugesToolsBar();
		}
	}
}
