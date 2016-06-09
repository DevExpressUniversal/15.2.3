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
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Localization;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Commands.Ribbon;
namespace DevExpress.DashboardWin.Bars {
	public class ItemOperationRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupItemsCaption); } }
		public ItemOperationRibbonPageGroup() {
			Visible = false;
		}
		protected override void OnDashboardItemSelected(DashboardItem dashboardItem) {
			base.OnDashboardItemSelected(dashboardItem);
			Visible = dashboardItem != null && !dashboardItem.IsGroup;
		}
	}
	public class DuplicateItemBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.DuplicateItem; } }
	}
	public class DeleteItemBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.DeleteItem; } }
	}
	public class RemoveDataItemsBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.RemoveDataItems; } }
	}
	public class EditRulesBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.EditRules; } }
	}
	public class TransposeItemBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.TransposeItem; } }
		protected override void OnControlUpdateUI(object sender, EventArgs e) {
			base.OnControlUpdateUI(sender, e);
			UpdateSuperTipAndShortCut();
		}
	}
	public class ConvertDashboardItemTypeBarItem : CommandBarSubItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ConvertDashboardItemType; } }
		public ConvertDashboardItemTypeBarItem() : base() {
			PaintStyle = BarItemPaintStyle.Standard;
		}
	}
	public class ConvertToPivotBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ConvertToPivot; } }
	}
	public class ConvertToGridBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ConvertToGrid; } }
	}
	public class ConvertToChartBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ConvertToChart; } }
	}
	public class ConvertToScatterChartBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ConvertToScatterChart; } }
	}
	public class ConvertToPieBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ConvertToPie; } }
	}
	public class ConvertToGaugeBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ConvertToGauge; } }
	}
	public class ConvertToCardBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ConvertToCard; } }
	}
	public class ConvertToChoroplethMapBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ConvertToChoroplethMap; } }
	}
	public class ConvertGeoPointMapBaseBarItem : CommandBarSubItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ConvertGeoPointMapBase; } }
		public ConvertGeoPointMapBaseBarItem() : base() {
			PaintStyle = BarItemPaintStyle.Standard;
		}
	}
	public class ConvertToGeoPointMapBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ConvertToGeoPointMap; } }
	}
	public class ConvertToBubbleMapBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ConvertToBubbleMap; } }
	}
	public class ConvertToPieMapBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ConvertToPieMap; } }
	}
	public class ConvertToRangeFilterBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ConvertToRangeFilter; } }
	}
	public class ConvertToComboBoxBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ConvertToComboBox; } }
	}
	public class ConvertToListBoxBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ConvertToListBox; } }
	}
	public class ConvertToTreeViewBarItem : DashboardBarButtonItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.ConvertToTreeView; } }
	}
}
namespace DevExpress.DashboardWin.Native {
	public class ItemOperationBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new DuplicateItemBarItem());
			items.Add(new DeleteItemBarItem());
			ConvertDashboardItemTypeBarItem barItem = new ConvertDashboardItemTypeBarItem();
			barItem.AddBarItem(new ConvertToPivotBarItem());
			barItem.AddBarItem(new ConvertToGridBarItem());
			barItem.AddBarItem(new ConvertToChartBarItem());
			barItem.AddBarItem(new ConvertToScatterChartBarItem());
			barItem.AddBarItem(new ConvertToPieBarItem());
			barItem.AddBarItem(new ConvertToGaugeBarItem());
			barItem.AddBarItem(new ConvertToCardBarItem());
			barItem.AddBarItem(new ConvertToChoroplethMapBarItem());
			ConvertGeoPointMapBaseBarItem geoPointMapBaseBarItem = new ConvertGeoPointMapBaseBarItem();
			geoPointMapBaseBarItem.AddBarItem(new ConvertToGeoPointMapBarItem());
			geoPointMapBaseBarItem.AddBarItem(new ConvertToBubbleMapBarItem());
			geoPointMapBaseBarItem.AddBarItem(new ConvertToPieMapBarItem());
			barItem.AddBarItem(geoPointMapBaseBarItem);
			barItem.AddBarItem(new ConvertToRangeFilterBarItem());
			barItem.AddBarItem(new ConvertToComboBoxBarItem());
			barItem.AddBarItem(new ConvertToListBoxBarItem());
			barItem.AddBarItem(new ConvertToTreeViewBarItem());
			items.Add(barItem);
			items.Add(new RemoveDataItemsBarItem());
			items.Add(new TransposeItemBarItem());
			items.Add(new EditRulesBarItem());
		}
	}
	public class ItemOperationBarCreator : HomeBarCreator {
		public override Type SupportedRibbonPageGroupType { get { return typeof(ItemOperationRibbonPageGroup); } }
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new ItemOperationBarItemBuilder();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new ItemOperationRibbonPageGroup();
		}
	}
}
