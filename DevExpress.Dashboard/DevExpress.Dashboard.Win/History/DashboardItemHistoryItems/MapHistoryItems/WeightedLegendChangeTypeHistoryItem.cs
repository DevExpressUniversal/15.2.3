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
	public class WeightedLegendChangeTypeHistoryItem : DashboardItemHistoryItem<MapDashboardItem> {
		readonly WeightedLegend legend;
		readonly WeightedLegendType typeNext;
		readonly WeightedLegendType typePrev;
		readonly bool visiblePrev;
		readonly bool visibleNext = true;
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemChangeWeightedLegendType; } }
		public WeightedLegendChangeTypeHistoryItem(MapDashboardItem dashboardItem, WeightedLegend legend, WeightedLegendType typeNext) 
			: base(dashboardItem) {
			this.legend = legend;
			this.typeNext = typeNext;
			typePrev = legend.Type;
			visiblePrev = legend.Visible;
		}
		protected override void PerformUndo() {
			legend.Type = typePrev;
			legend.Visible = visiblePrev;
		}
		protected override void PerformRedo() {
			legend.Type = typeNext;
			legend.Visible = visibleNext;
		}
	}
	public class DisableWeightedLegendHistoryItem : DashboardItemHistoryItem<MapDashboardItem> {
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemDisableWeightedLegend; } }
		readonly WeightedLegend legend;
		public DisableWeightedLegendHistoryItem(MapDashboardItem dashboardItem, WeightedLegend legend) 
			: base(dashboardItem) {
			this.legend = legend;
		}
		protected override void PerformUndo() {
			legend.Visible = true;
		}
		protected override void PerformRedo() {
			legend.Visible = false;
		}
	}
}
