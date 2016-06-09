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
namespace DevExpress.DashboardWin.Bars {
	public class GridStyleRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupGridStyleCaption); } }
	}
	public class GridLayoutRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupGridLayoutCaption); } }
	}
	public class GridColumnWidthModeRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupGridColumnWidthModeCaption); } }
	}
	public class GridHorizontalLinesBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.GridHorizontalLines; } }
	}
	public class GridVerticalLinesBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.GridVerticalLines; } }
	}
	public class GridMergeCellsBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.GridMergeCells; } }
	}
	public class GridBandedRowsBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.GridBandedRows; } }
	}
	public class GridColumnHeadersBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.GridColumnHeaders; } }
	}
	public class GridWordWrapBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.GridWordWrap; } }
	}
	public class GridAutoFitToContentsColumnWidthModeBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.GridAutoFitToContentsColumnWidthMode; } }
	}
	public class GridAutoFitToGridColumnWidthModeBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.GridAutoFitToGridColumnWidthMode; } }
	}
	public class ManualGridColumnWidthModeBarItem : CommandBarCheckItem {
		protected override DashboardCommandId CommandId { get { return DashboardCommandId.GridManualGridColumnWidthMode; } }
	}
}
namespace DevExpress.DashboardWin.Native {
	public class GridStyleBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new GridHorizontalLinesBarItem());
			items.Add(new GridVerticalLinesBarItem());
			items.Add(new GridBandedRowsBarItem());
		}
	}
	public class GridLayoutBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new GridMergeCellsBarItem());
			items.Add(new GridColumnHeadersBarItem());
			items.Add(new GridWordWrapBarItem());
		}
	}
	public class GridColumnWidthModeBarItemBuilder : CommandBasedBarItemBuilder {
		public override void PopulateItems(List<BarItem> items, BarCreationContextBase creationContext) {
			items.Add(new GridAutoFitToContentsColumnWidthModeBarItem());
			items.Add(new GridAutoFitToGridColumnWidthModeBarItem());
			items.Add(new ManualGridColumnWidthModeBarItem());
		}
	}
	public class GridStyleBarCreator : DashboardItemDesignBarCreator {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(GridToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(GridStyleRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(GridToolsBar); } }
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new GridToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new GridStyleRibbonPageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new GridStyleBarItemBuilder();
		}
		public override Bar CreateBar() {
			return new GridToolsBar();
		}
	}
	public class GridLayoutBarCreator : DashboardItemDesignBarCreator {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(GridToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(GridLayoutRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(GridToolsBar); } }
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new GridToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new GridLayoutRibbonPageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new GridLayoutBarItemBuilder();
		}
		public override Bar CreateBar() {
			return new GridToolsBar();
		}
	}
	public class GridAutoFitToContentsColumnWidthModeBarCreator : DashboardItemDesignBarCreator {
		public override Type SupportedRibbonPageCategoryType { get { return typeof(GridToolsRibbonPageCategory); } }
		public override Type SupportedRibbonPageGroupType { get { return typeof(GridColumnWidthModeRibbonPageGroup); } }
		public override Type SupportedBarType { get { return typeof(GridToolsBar); } }
		public override CommandBasedRibbonPageCategory CreateRibbonPageCategoryInstance() {
			return new GridToolsRibbonPageCategory();
		}
		public override CommandBasedRibbonPageGroup CreateRibbonPageGroupInstance() {
			return new GridColumnWidthModeRibbonPageGroup();
		}
		public override CommandBasedBarItemBuilder CreateBarItemBuilder() {
			return new GridColumnWidthModeBarItemBuilder();
		}
		public override Bar CreateBar() {
			return new GridToolsBar();
		}
	}
}
