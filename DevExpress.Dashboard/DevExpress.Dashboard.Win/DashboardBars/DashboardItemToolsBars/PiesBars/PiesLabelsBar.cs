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
using DevExpress.XtraBars.Commands.Internal;
using DevExpress.DashboardWin.Commands;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Bars {
	public class PieLabelsRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupPieLabelsCaption); } }
	}
	public class PieLabelsDataLabelsBarItem : CommandBarSubItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PieLabelsDataLabels; } }
	}
	public class PieTooltipsBarItem : CommandBarSubItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PieLabelsTooltips; } }
	}
	public class PieLabelsDataLabelsNoneBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PieLabelsDataLabelsNone; } }
	}
	public class PieLabelsDataLabelArgumentBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PieLabelsDataLabelsArgument; } }
	}
	public class PieLabelsDataLabelsValueBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PieLabelsDataLabelsValue; } }
	}
	public class PieLabelsDataLabelsArgumentAndValueBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PieLabelsDataLabelsArgumentAndValue; } }
	}
	public class PieLabelsDataLabelsPercentBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PieLabelsDataLabelsPercent; } }
	}
	public class PieLabelsDataLabelsValueAndPercentBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PieLabelsDataLabelsValueAndPercent; } }
	}
	public class PieLabelsDataLabelsArgumentAndPercentBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PieLabelsDataLabelsArgumentAndPercent; } }
	}
	public class PieLabelsDataLabelsArgumentValueAndPercentBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PieLabelsDataLabelsArgumentValueAndPercent; } }
	}
	public class PieLabelsTooltipsNoneBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PieLabelsTooltipsNone; } }
	}
	public class PieLabelsTooltipsArgumentBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PieLabelsTooltipsArgument; } }
	}
	public class PieLabelsTooltipsValueBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PieLabelsTooltipsValue; } }
	}
	public class PieLabelsTooltipsArgumentAndValueBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PieLabelsTooltipsArgumentAndValue; } }
	}
	public class PieLabelsTooltipsPercentBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PieLabelsTooltipsPercent; } }
	}
	public class PieLabelsTooltipsValueAndPercentBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PieLabelsTooltipsValueAndPercent; } }
	}
	public class PieLabelsTooltipsArgumentAndPercentBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PieLabelsTooltipsArgumentAndPercent; } }
	}
	public class PieLabelsTooltipsArgumentValueAndPercentBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.PieLabelsTooltipsArgumentValueAndPercent; } }
	}
}
namespace DevExpress.DashboardWin.Native {
	public class PieLabelsBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			PieLabelsDataLabelsBarItem dataLabelsBarItem = new PieLabelsDataLabelsBarItem();
			dataLabelsBarItem.AddBarItem(new PieLabelsDataLabelsNoneBarItem());
			dataLabelsBarItem.AddBarItem(new PieLabelsDataLabelArgumentBarItem());
			dataLabelsBarItem.AddBarItem(new PieLabelsDataLabelsValueBarItem());
			dataLabelsBarItem.AddBarItem(new PieLabelsDataLabelsArgumentAndValueBarItem());
			dataLabelsBarItem.AddBarItem(new PieLabelsDataLabelsPercentBarItem());
			dataLabelsBarItem.AddBarItem(new PieLabelsDataLabelsValueAndPercentBarItem());
			dataLabelsBarItem.AddBarItem(new PieLabelsDataLabelsArgumentAndPercentBarItem());
			dataLabelsBarItem.AddBarItem(new PieLabelsDataLabelsArgumentValueAndPercentBarItem());
			items.Add(dataLabelsBarItem);
			PieTooltipsBarItem tooltipsBarItem = new PieTooltipsBarItem();
			tooltipsBarItem.AddBarItem(new PieLabelsTooltipsNoneBarItem());
			tooltipsBarItem.AddBarItem(new PieLabelsTooltipsArgumentBarItem());
			tooltipsBarItem.AddBarItem(new PieLabelsTooltipsValueBarItem());
			tooltipsBarItem.AddBarItem(new PieLabelsTooltipsArgumentAndValueBarItem());
			tooltipsBarItem.AddBarItem(new PieLabelsTooltipsPercentBarItem());
			tooltipsBarItem.AddBarItem(new PieLabelsTooltipsValueAndPercentBarItem());
			tooltipsBarItem.AddBarItem(new PieLabelsTooltipsArgumentAndPercentBarItem());
			tooltipsBarItem.AddBarItem(new PieLabelsTooltipsArgumentValueAndPercentBarItem());
			items.Add(tooltipsBarItem);
		}
	}
	public class PieLabelsBarCreator : DashboardItemDesignBarCreator {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(PiesToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(PieLabelsRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(PiesToolsBar); } }
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new PiesToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new PieLabelsRibbonPageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new PieLabelsBarItemBuilder();
		}
		public override Bar CreateBar() {
			return new PiesToolsBar();
		}
	}
}
