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
	public class PivotToolsBar : DataDashboardItemToolsBar {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryPivotToolsName); } }
		protected internal override CommandBasedBarItemBuilder MasterFilterBuilder { get { return null; } }
		protected internal override CommandBasedBarItemBuilder DrillDownBuilder { get { return null; } }
		protected internal override CommandBasedBarItemBuilder TargetDimensionsBuilder { get { return null; } }
		protected internal override CommandBasedBarItemBuilder MasterFilterOptionsBuilder { get { return new PivotDashboardItemMasterFilterOptionsBuilder(); } }
		protected override bool CheckBarVisibility(DashboardItemViewer viewer) {
			return viewer is PivotDashboardItemViewer;
		}
	}
	public class PivotToolsRibbonPageCategory : DashboardRibbonPageCategory {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryPivotToolsName); } }
		protected override DashboardCommandId EmptyCommandId { get { return DashboardCommandId.None; } }
		protected internal override bool CheckCurrentPage(string dashboardItemType) {
			return dashboardItemType == DashboardItemType.Pivot;
		}
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new PivotToolsRibbonPageCategory();
		}
	}
	public class PivotLayoutRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupPivotLayoutCaption); } }
	}
	public class PivotInitialStateBarItem : CommandBarSubItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PivotInitialState; } }
	}
	public class PivotAutoExpandColumnBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PivotAutoExpandColumn; } }
	}
	public class PivotAutoExpandRowBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PivotAutoExpandRow; } }
	}
	public class PivotShowGrandTotalsBarItem : CommandBarSubItem {
		protected override DashboardCommandId CommandId { get {return DashboardCommandId.PivotShowGrandTotals; } }
	}
	public class PivotShowColumnGrandTotalsBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PivotShowColumnGrandTotals; } }
	}
	public class PivotShowRowGrandTotalsBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PivotShowRowGrandTotals; } }
	}
	public class PivotShowTotalsBarItem : CommandBarSubItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PivotShowTotals; } }
	}
	public class PivotShowColumnTotalsBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PivotShowColumnTotals; } }
	}
	public class PivotShowRowTotalsBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PivotShowRowTotals; } }
	}
}
namespace DevExpress.DashboardWin.Native {
	public class PivotLayoutBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			PivotInitialStateBarItem initialStateBarItem = new PivotInitialStateBarItem();
			initialStateBarItem.AddBarItem(new PivotAutoExpandColumnBarItem());
			initialStateBarItem.AddBarItem(new PivotAutoExpandRowBarItem());
			items.Add(initialStateBarItem);
			PivotShowTotalsBarItem totalsBarItem = new PivotShowTotalsBarItem();
			totalsBarItem.AddBarItem(new PivotShowColumnTotalsBarItem());
			totalsBarItem.AddBarItem(new PivotShowRowTotalsBarItem());
			items.Add(totalsBarItem);
			PivotShowGrandTotalsBarItem grandTotalsBarItem = new PivotShowGrandTotalsBarItem();
			grandTotalsBarItem.AddBarItem(new PivotShowColumnGrandTotalsBarItem());
			grandTotalsBarItem.AddBarItem(new PivotShowRowGrandTotalsBarItem());
			items.Add(grandTotalsBarItem);
		}
	}
	public class PivotLayoutToolsBarCreator : DataBarCreator {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(PivotToolsRibbonPageCategory); } }
		public override Type SupportedBarType { get { return typeof(PivotToolsBar); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(PivotLayoutRibbonPageGroup); } }
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new PivotLayoutBarItemBuilder();
		}
		public override Bar CreateBar() {
			return new PivotToolsBar();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new PivotLayoutRibbonPageGroup();
		}
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new PivotToolsRibbonPageCategory();
		}
	}
}
