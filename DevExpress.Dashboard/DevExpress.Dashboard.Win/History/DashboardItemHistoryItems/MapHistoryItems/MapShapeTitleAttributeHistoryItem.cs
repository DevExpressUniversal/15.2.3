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

using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
namespace DevExpress.DashboardWin.Native {
	public class MapShapeTitleAttributeHistoryItem : DashboardItemHistoryItem<MapDashboardItem> {
		readonly string prev;
		readonly string next;
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemMapShapeTitleAttribute; } }
		public MapShapeTitleAttributeHistoryItem(MapDashboardItem dashboardItem, string shapeTittleAttributeName, string prevShapeTittleAttributeName)
			: base(dashboardItem) {
			prev = prevShapeTittleAttributeName;
			next = shapeTittleAttributeName;
		}
		protected override void PerformUndo() {
			DashboardItem.ShapeTitleAttributeName = prev;
		}
		protected override void PerformRedo() {
			DashboardItem.ShapeTitleAttributeName = next;
		}
	}
	public class ChoroplethMapShapeLabelsAttributeHistoryItem : MapShapeTitleAttributeHistoryItem {
		readonly string prevToolTip;
		readonly string nextToolTip;
		ChoroplethMapDashboardItem ChoroplethMapDashboardItem { get { return (ChoroplethMapDashboardItem)DashboardItem; } }
		public ChoroplethMapShapeLabelsAttributeHistoryItem(ChoroplethMapDashboardItem dashboardItem, string shapeTittleAttributeName, string prevShapeTittleAttributeName,
			string toolTipAttributeName, string prevToolTipAttributeName)
			: base(dashboardItem, shapeTittleAttributeName, prevShapeTittleAttributeName) {
			prevToolTip = prevToolTipAttributeName;
			nextToolTip = toolTipAttributeName;
		}
		protected override void PerformUndo() {
			ChoroplethMapDashboardItem.LockChanging();
			base.PerformUndo();
			ChoroplethMapDashboardItem.TooltipAttributeName = prevToolTip;
			ChoroplethMapDashboardItem.UnlockChanging();
			ChoroplethMapDashboardItem.RaiseChangedAttributeName();
		}
		protected override void PerformRedo() {
			ChoroplethMapDashboardItem.LockChanging();
			base.PerformRedo();
			ChoroplethMapDashboardItem.TooltipAttributeName = nextToolTip;
			ChoroplethMapDashboardItem.UnlockChanging();
			ChoroplethMapDashboardItem.RaiseChangedAttributeName();
		}
	}
}
