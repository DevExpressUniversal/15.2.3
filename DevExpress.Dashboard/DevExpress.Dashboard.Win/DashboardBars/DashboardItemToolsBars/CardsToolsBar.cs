﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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

using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Native;
using DevExpress.XtraBars.Commands;
using DevExpress.XtraBars.Ribbon;
namespace DevExpress.DashboardWin.Bars {
	public class CardsToolsBar : DataDashboardItemToolsBar {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryCardsToolsName); } }
		protected internal override CommandBasedBarItemBuilder MasterFilterBuilder { get { return new MasterFilterItemBuilder(); } }
		protected internal override CommandBasedBarItemBuilder DrillDownBuilder { get { return null; } }
		protected internal override CommandBasedBarItemBuilder TargetDimensionsBuilder { get { return null; } }
		protected override bool CheckBarVisibility(DashboardItemViewer viewer) {
			return viewer is CardDashboardItemViewer;
		}
	}
	public class CardsToolsRibbonPageCategory : DashboardRibbonPageCategory {
		public override string DefaultText { get { return DashboardWinLocalizer.GetString(DashboardWinStringId.RibbonPageCategoryCardsToolsName); } }
		protected override Commands.DashboardCommandId EmptyCommandId { get { return Commands.DashboardCommandId.None; } }
		protected internal override bool CheckCurrentPage(string dashboardItemType) {
			return dashboardItemType == DashboardItemType.Card;
		}
		protected override RibbonPageCategory CreateRibbonPageCategory() {
			return new CardsToolsRibbonPageCategory();
		}
	}
}
