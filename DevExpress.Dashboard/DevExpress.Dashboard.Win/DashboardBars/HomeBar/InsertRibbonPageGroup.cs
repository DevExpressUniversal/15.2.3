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
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Localization;
using DevExpress.XtraBars.Commands.Internal;
namespace DevExpress.DashboardWin.Bars {
	public class InsertRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupInsertCaption); } }
	}
	public class InsertPivotBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.InsertPivot; } }
	}
	public class InsertGridBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.InsertGrid; } }
	}
	public class InsertChartBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.InsertChart; } }
	}
	public class InsertScatterChartBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.InsertScatterChart; } }
	}
	public class InsertPiesBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.InsertPies; } }
	}
	public class InsertGaugesBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.InsertGauges; } }
	}
	public class InsertCardsBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.InsertCards; } }
	}
	public class InsertImageBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.InsertImage; } }
	}
	public class InsertTextBoxBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.InsertTextBox; } }
	}
	public class InsertChoroplethMapBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.InsertChoroplethMap; } }
	}
	public class InsertRangeFilterBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.InsertRangeFilter; } }
	}
	public class InsertGeoPointMapBarSubItem : CommandBarSubItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.SelectGeoPointDashboardItemType; } }
	}
	public class InsertGeoPointMapBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.InsertGeoPointMap; } }
	}
	public class InsertBubbleMapBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.InsertBubbleMap; } }
	}
	public class InsertPieMapBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.InsertPieMap; } }
	}
	public class InsertFilterElementSubItem : CommandBarSubItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.SelectFilterElementType; } }
	}
	public class InsertComboBoxBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.InsertComboBox; } }
	}
	public class InsertListBoxBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.InsertListBox; } }
	}
	public class InsertTreeViewBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.InsertTreeView; } }
	}
	public class InsertGroupBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.InsertGroup; } }
	}
}
namespace DevExpress.DashboardWin.Native {
	public class InsertBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new InsertPivotBarItem());
			items.Add(new InsertGridBarItem());
			items.Add(new InsertChartBarItem());
			items.Add(new InsertScatterChartBarItem());
			items.Add(new InsertPiesBarItem());
			items.Add(new InsertGaugesBarItem());
			items.Add(new InsertCardsBarItem());
			items.Add(new InsertChoroplethMapBarItem());
			InsertGeoPointMapBarSubItem mapItems = new InsertGeoPointMapBarSubItem();
			mapItems.AddBarItem(new InsertGeoPointMapBarItem());
			mapItems.AddBarItem(new InsertBubbleMapBarItem());
			mapItems.AddBarItem(new InsertPieMapBarItem());
			items.Add(mapItems);
			items.Add(new InsertRangeFilterBarItem());
			InsertFilterElementSubItem filterElementItems = new InsertFilterElementSubItem();
			filterElementItems.AddBarItem(new InsertComboBoxBarItem());
			filterElementItems.AddBarItem(new InsertListBoxBarItem());
			filterElementItems.AddBarItem(new InsertTreeViewBarItem());
			items.Add(filterElementItems);
			items.Add(new InsertImageBarItem());
			items.Add(new InsertTextBoxBarItem());
			items.Add(new InsertGroupBarItem());
		}
	}
	public class InsertBarCreator : HomeBarCreator {
		public override Type SupportedRibbonPageGroupType { get { return typeof(InsertRibbonPageGroup); } }
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new InsertBarItemBuilder();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new InsertRibbonPageGroup();
		}
	}
}
