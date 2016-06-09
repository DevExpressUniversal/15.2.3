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
using DevExpress.DashboardWin.Localization;
using DevExpress.XtraBars.Commands.Ribbon;
using DevExpress.DashboardWin.Native;
namespace DevExpress.DashboardWin.Bars {
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class ItemsRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupItemsCaption); } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class FormatRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupDashboardCaption); } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class DrillDownRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return String.Empty; } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class DashboardBar : DashboardCommandBar {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.BarDashboardCaption); } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class DashboardRibbonPage : ControlCommandBasedRibbonPage {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageDashboardCaption); } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class LayoutAndStyleRibbonPage : ControlCommandBasedRibbonPage {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageLayoutAndStyleCaption); } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class ChartLayoutAndStyleRibbonPage : ControlCommandBasedRibbonPage {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageLayoutAndStyleCaption); } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class ImageOptionsRibbonPage : ControlCommandBasedRibbonPage {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageLayoutAndStyleCaption); } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class TextBoxFormatRibbonPage : ControlCommandBasedRibbonPage {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageLayoutAndStyleCaption); } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class RangeFilterStyleRibbonPage : ControlCommandBasedRibbonPage {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageLayoutAndStyleCaption); } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class FilterByChartSeriesBarItem : CommandBarCheckItem {
		protected override Commands.DashboardCommandId CommandId { get { return Commands.DashboardCommandId.None; } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class FilterByChartArgumentsBarItem : CommandBarCheckItem {
		protected override Commands.DashboardCommandId CommandId { get { return Commands.DashboardCommandId.None; } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class FilterByPieSeriesBarItem : CommandBarCheckItem {
		protected override Commands.DashboardCommandId CommandId { get { return Commands.DashboardCommandId.None; } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class FilterByPieArgumentsBarItem : CommandBarCheckItem {
		protected override Commands.DashboardCommandId CommandId { get { return Commands.DashboardCommandId.None; } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class DrillDownOnChartSeriesBarItem : CommandBarCheckItem {
		protected override Commands.DashboardCommandId CommandId { get { return Commands.DashboardCommandId.None; } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class DrillDownOnChartArgumentsBarItem : CommandBarCheckItem {
		protected override Commands.DashboardCommandId CommandId { get { return Commands.DashboardCommandId.None; } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class DrillDownOnPieSeriesBarItem : CommandBarCheckItem {
		protected override Commands.DashboardCommandId CommandId { get { return Commands.DashboardCommandId.None; } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class DrillDownOnPieArgumentsBarItem : CommandBarCheckItem {
		protected override Commands.DashboardCommandId CommandId { get { return Commands.DashboardCommandId.None; } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class GridCellsRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonGroupGridCellsCaption); } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class EditDataSourceBarItem : DashboardBarButtonItem {
		protected override Commands.DashboardCommandId CommandId { get { return Commands.DashboardCommandId.None; } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class DataSourceElementsRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return string.Empty; } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class SqlDataSourceQueriesRibbonPageGroup : DashboardRibbonPageGroup {
		public override string DefaultText { get { return string.Empty; } }
	}
	[Obsolete(DashboardWinHelper.ObsoleteBarItemMessage, false)]
	public class EditQueriesBarItem : DashboardBarButtonItem {
		protected override Commands.DashboardCommandId CommandId { get { return Commands.DashboardCommandId.None; } }
	}
}
