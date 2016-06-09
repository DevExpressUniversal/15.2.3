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

using System.Linq;
using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin.Localization;
using DevExpress.DashboardWin.Bars;
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardWin.Native {
	public abstract class ChartLegendPositionHistoryItem : DashboardItemHistoryItem<IChartDashboardItem> {
		readonly bool initialIsInsideDiagram;
		readonly bool isInsideDiagram;
		protected override DashboardWinStringId CaptionId { get { return DashboardWinStringId.HistoryItemChartLegendPosition; } }
		protected ChartLegend Legend { get { return DashboardItem.Legend; } }
		protected ChartLegendPositionHistoryItem(IChartDashboardItem dashboardItem, bool isInsideDiagram)
			: base(dashboardItem) {
			initialIsInsideDiagram = Legend.IsInsideDiagram;
			this.isInsideDiagram = isInsideDiagram;
		}
		protected override void PerformUndo() {
			Legend.IsInsideDiagram = initialIsInsideDiagram;
		}
		protected override void PerformRedo() {
			Legend.IsInsideDiagram = isInsideDiagram;
		}
	}
	public class ChartLegendOutsidePositionHistoryItem : ChartLegendPositionHistoryItem {
		readonly ChartLegendOutsidePosition initialPosition;
		readonly ChartLegendOutsidePosition position;
		public ChartLegendOutsidePositionHistoryItem(IChartDashboardItem dashboardItem, ChartLegendOutsidePosition position)
			: base(dashboardItem, false) {
			initialPosition = Legend.OutsidePosition;
			this.position = position;
		}
		protected override void PerformUndo() {
			base.PerformUndo();
			Legend.OutsidePosition = initialPosition;
		}
		protected override void PerformRedo() {
			base.PerformRedo();
			Legend.OutsidePosition = position;
		}
	}
	public class ChartLegendInsidePositionHistoryItem : ChartLegendPositionHistoryItem {
		readonly ChartLegendInsidePosition initialPosition;
		readonly ChartLegendInsidePosition position;
		public ChartLegendInsidePositionHistoryItem(IChartDashboardItem dashboardItem, ChartLegendInsidePosition position)
			: base(dashboardItem, true) {
			initialPosition = Legend.InsidePosition;
			this.position = position;
		}
		protected override void PerformUndo() {
			base.PerformUndo();
			Legend.InsidePosition = initialPosition;
		}
		protected override void PerformRedo() {
			base.PerformRedo();
			Legend.InsidePosition = position;
		}
	}
}
